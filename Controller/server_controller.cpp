#pragma once

#include <fstream>
#include <iostream>
#include <iterator>
#include <boost/algorithm/string.hpp>
#include <boost/asio.hpp>
#include <boost/bind/bind.hpp>
#include <boost/enable_shared_from_this.hpp>
#include <map>
#include <boost/json.hpp>
#include <mutex>
#include <filesystem>
#include <boost/filesystem.hpp>
#include <boost/range/iterator_range.hpp>
#include "server_controller.h"
#include "../Model/ServerSpreadsheet.h"

using boost::asio::ip::tcp;
using namespace boost::placeholders;
typedef boost::shared_ptr<Server::connection_handler> pointer;

int Server::next_ID;
std::mutex connections_lock, next_ID_lock, spreadsheets_lock;

// Code came from: https://www.codeproject.com/Articles/1264257/Socket-Programming-in-Cplusplus-using-boost-asio-T
Server::connection_handler::connection_handler(boost::asio::io_context& io_context, Server* s)
	: sock(io_context)
{
	server = s;
};

Server::connection_handler::~connection_handler()
{
}

//constructor for accepting connection from client
Server::Server(boost::asio::io_context& io_context) : io_context_(io_context), acceptor(io_context, tcp::endpoint(tcp::v4(), 1100))
{
	spreadsheets = new std::map<std::string, Spreadsheet>();
	connections = std::map<int, connection_handler::pointer>();
	std::string path = "./spreadsheet_data";
	for (auto& p : std::filesystem::directory_iterator(path))
	{
		std::string file_path = p.path().string();
		try {
			Spreadsheet s = load_from_file(file_path);

			std::string name = file_path.substr(path.size() + 1, file_path.size() - path.size() - 5);
			spreadsheets->insert_or_assign(name, s);
		}
		catch (std::exception& e) {
		}
	}
	next_ID = 0;
	start_accept();
}

//socket creation
tcp::socket& Server::connection_handler::socket()
{
	return sock;
}

void Server::connection_handler::start()
{
	boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(r_buffer),
		'\n', boost::bind(&connection_handler::on_name, shared_from_this(), boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
}

void Server::stop()
{
	// disconnect clients
	connections_lock.lock();
	for (std::pair<int, connection_handler::pointer> connection : connections)
	{
		std::string message = "{ messageType:\"serverError\", message: \"server shutdown\" }\n";
		connection.second.get()->socket().write_some(boost::asio::buffer(message, connection_handler::max_length));
		connection.second.get()->socket().close();
	}
	connections_lock.unlock();
	// save spreadsheets to file
	spreadsheets_lock.lock();
	std::map<std::string, Spreadsheet> copy(*spreadsheets);
	for (std::pair<std::string, Spreadsheet> spreadsheet : copy)
	{
		save_to_file(spreadsheet.second);
	}
	spreadsheets_lock.unlock();
	io_context_.stop();
}

void Server::connection_handler::on_name(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		client_name = r_buffer.substr(0, r_buffer.size() - 1);

		r_buffer.clear();

		connections_lock.lock();
		next_ID_lock.lock();
		server->connections.insert(std::pair<int, connection_handler::pointer>(next_ID, shared_from_this()));
		connections_lock.unlock();
		ID = next_ID;
		next_ID++;
		next_ID_lock.unlock();

		// Send list of spreadsheet names to client
		sock.write_some(boost::asio::buffer(server->get_spreadsheets(), max_length));

		// Continue asynchronous handshake, look for spreadsheet choice
		boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(r_buffer), '\n',
			boost::bind(&connection_handler::on_spreadsheet, shared_from_this(),
				boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	}
	else {
		client_disconnected();
	}
}

void Server::connection_handler::on_spreadsheet(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		std::string spreadsheet_name = r_buffer.substr(0, r_buffer.size() - 1);

		r_buffer.clear();

		curr_spreadsheet = spreadsheet_name;

		// LOCK THE SPREADSHEET

		// Send data of chosen spreadsheet
		Spreadsheet* spreadsheet;

		spreadsheets_lock.lock();
		std::map<std::string, Spreadsheet>::iterator it = server->spreadsheets->find(spreadsheet_name);

		if (it != server->spreadsheets->end())
		{
			spreadsheet = &server->spreadsheets->at(spreadsheet_name);
			std::map<std::string, Cell*>* cells = spreadsheet->get_cells();

			// Send every edited cell
			for (std::map<std::string, Cell*>::iterator it = cells->begin(); it != cells->end(); ++it)
			{
				std::string message = "{ messageType: \"cellUpdated\", cellName: \"" + it->first + "\", contents: \"" + it->second->get_contents() + "\" }\n";
				sock.write_some(boost::asio::buffer(message, max_length));
			}

			std::map<int, User> users = spreadsheet->get_users();
			// Send every selected cell
			for (std::map<int, User>::iterator it = users.begin(); it != users.end(); ++it)
			{
				std::string message = "{ messageType: \"cellSelected\", cellName: \"" + it->second.get_selected() + "\", selector: " + std::to_string(it->first) + ", selectorName: \"" + it->second.get_name() + "\"}\n";
				sock.write_some(boost::asio::buffer(message, max_length));
			}
		}

		else
		{
			spreadsheet = new Spreadsheet(spreadsheet_name);
			server->spreadsheets->insert(std::pair<std::string, Spreadsheet>(spreadsheet_name, *(spreadsheet)));
		}

		spreadsheet->add_user(client_name, ID);

		spreadsheets_lock.unlock();

		std::string message = std::to_string(ID) + "\n";
		sock.write_some(boost::asio::buffer(message, max_length));

		// END OF LOCK

		boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(r_buffer), '\n',
			boost::bind(&connection_handler::handle_read, shared_from_this(),
				boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	}
	else {
		client_disconnected();
	}
}

void Server::connection_handler::handle_read(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		// add s_buffer (the storage buffer) to the beginning of the recieved buffer
		r_buffer = s_buffer.append(r_buffer);

		// split by newline every time there is one, then store the leftover into s_buffer
		while (r_buffer.find('\n') != std::string::npos)
		{
			std::string request = split_and_delete(r_buffer);

			std::string cellName;
			std::string contents;
			std::string request_name = find_request_type(request, cellName, contents);

			if (request_name == "selectCell")
			{
				spreadsheets_lock.lock();
				server->spreadsheets->at(curr_spreadsheet).select_cell(ID, cellName);

				connections_lock.lock();
				std::map<int, connection_handler::pointer> connections = server->connections;
				for (std::map<int, connection_handler::pointer>::iterator it = connections.begin(); it != connections.end(); ++it)
				{
					//maybe get weird errors with other connection_handlers sending stuff at same time
					if (it->second.get()->curr_spreadsheet == curr_spreadsheet && it->second.get()->sock.is_open())
					{
						std::string message = "{ messageType: \"cellSelected\", cellName: \"" + cellName + "\", selector: \"" + std::to_string(ID) + "\", selectorName: \"" + client_name + "\" }\n";
						it->second.get()->sock.write_some(boost::asio::buffer(message, max_length));
					}
				}
				connections_lock.unlock();
				spreadsheets_lock.unlock();
			}

			else if (request_name == "editCell")
			{
				spreadsheets_lock.lock();
				Cell* edit = new Cell(cellName, contents);
				if (server->spreadsheets->at(curr_spreadsheet).edit_cell(edit))
				{
					// ADD CELL CHANGE TO UNDO STACK

					connections_lock.lock();
					std::map<int, connection_handler::pointer> connections = server->connections;
					for (std::map<int, connection_handler::pointer>::iterator it = connections.begin(); it != connections.end(); ++it)
					{
						//maybe get weird errors with other connection_handlers sending stuff at same time
						if (it->second.get()->curr_spreadsheet == curr_spreadsheet && it->second.get()->sock.is_open())
						{
							std::string message = "{ messageType: \"cellUpdated\", cellName: \"" + cellName + "\", contents: \"" + contents + "\"" + "}\n";
							it->second.get()->sock.write_some(boost::asio::buffer(message, max_length));
						}
					}
					connections_lock.unlock();
				}
				else
				{
					std::string invalid = "Edit request was invalid!";
					std::string message = "{ messageType: \"requestError\", cellName: \" " + cellName + "\", message: \"" + invalid + "\"" + "}\n";
					sock.write_some(boost::asio::buffer(message, max_length));
				}
				spreadsheets_lock.unlock();
			}

			else if (request_name == "undo")
			{
				spreadsheets_lock.lock();
				bool success = false;
				Cell* cell = server->spreadsheets->at(curr_spreadsheet).undo(success);
				if (success)
				{
					connections_lock.lock();
					std::map<int, connection_handler::pointer> connections = server->connections;
					for (std::map<int, connection_handler::pointer>::iterator it = connections.begin(); it != connections.end(); ++it)
					{
						//maybe get weird errors with other connection_handlers sending stuff at same time
						if (it->second.get()->curr_spreadsheet == curr_spreadsheet && it->second.get()->sock.is_open())
						{
							std::string message = "{ messageType: \"cellUpdated\", cellName: \"" + cell->get_name() + "\", contents: \"" + cell->get_contents() + "\"" + "}\n";
							it->second.get()->sock.write_some(boost::asio::buffer(message, max_length));
						}
					}
					connections_lock.unlock();
				}
				else
				{
					std::string invalid = "Edit request was invalid!";
					std::string message = "{ messageType: \"requestError\", cellName: \" " + cellName + "\", message: \"" + invalid + "\"" + "}\n";
					sock.write_some(boost::asio::buffer(message, max_length));
				}
				spreadsheets_lock.unlock();
			}

			else if (request_name == "revertCell")
			{
				spreadsheets_lock.lock();
				Cell* edit;
				if (server->spreadsheets->at(curr_spreadsheet).get_cells()->find(cellName) == server->spreadsheets->at(curr_spreadsheet).get_cells()->end())
					edit = new Cell(cellName, contents);
				else
					edit = new Cell(server->spreadsheets->at(curr_spreadsheet).get_cells()->at(cellName));
				if (server->spreadsheets->at(curr_spreadsheet).revert(edit))
				{
					Cell* cell = server->spreadsheets->at(curr_spreadsheet).get_cells()->at(cellName);
					connections_lock.lock();
					std::map<int, connection_handler::pointer> connections = server->connections;
					for (std::map<int, connection_handler::pointer>::iterator it = connections.begin(); it != connections.end(); ++it)
					{
						//maybe get weird errors with other connection_handlers sending stuff at same time
						if (it->second.get()->curr_spreadsheet == curr_spreadsheet && it->second.get()->sock.is_open())
						{
							std::string message = "{ messageType: \"cellUpdated\", cellName: \"" + cell->get_name() + "\", contents: \"" + cell->get_contents() + "\"" + "}\n";
							it->second.get()->sock.write_some(boost::asio::buffer(message, max_length));
						}
					}
					connections_lock.unlock();
					// END LOCK HERE
				}
				else
				{
					std::string invalid = "Edit request was invalid!";
					std::string message = "{ messageType: \"requestError\", cellName: \" " + cellName + "\", message: \"" + invalid + "\"" + "}\n";
					sock.write_some(boost::asio::buffer(message, max_length));
				}
				spreadsheets_lock.unlock();
			}
		}

		s_buffer = r_buffer;

		boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(r_buffer), '\n',
			boost::bind(&connection_handler::handle_read, shared_from_this(),
				boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	}
	else
	{
		client_disconnected();
	}
}

void Server::connection_handler::handle_write(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
	}
	else {
		std::cerr << "error: " << err.message() << std::endl;
		client_disconnected();
	}
}

void Server::start_accept()
{
	// make the connection
	pointer connection(new connection_handler(io_context_, this));

	// asynchronous accept operation and wait for a new connection.
	acceptor.async_accept(connection->socket(),
		boost::bind(&Server::handle_accept, this, connection,
			boost::asio::placeholders::error));
}

void Server::handle_accept(connection_handler::pointer connection, const boost::system::error_code& err)
{
	if (!err)
		connection->start();

	start_accept();
}

std::string Server::get_spreadsheets()
{
	// LOCK HERE
	std::string spreadsheets = "";
	spreadsheets_lock.lock();
	for (std::map<std::string, Spreadsheet>::iterator it = Server::spreadsheets->begin(); it != Server::spreadsheets->end(); ++it)
	{
		spreadsheets.append(it->first + "\n");
	}
	spreadsheets_lock.unlock();
	// END LOCK HERE
	return spreadsheets + "\n";
}

std::string Server::connection_handler::find_request_type(std::string s, std::string& cellName, std::string& contents)
{
	int first = s.find("\"");
	std::string temp = s.substr(first + 1, s.size());
	int second = temp.find("\"");
	std::string val = s.substr(first + 1, second);
	s = s.substr(first + second + 2, s.size());

	if (val == "selectCell")
	{
		first = s.find("\"");
		temp = s.substr(first + 1, s.size());
		second = temp.find("\"");
		cellName = s.substr(first + 1, second);
	}

	else if (val == "editCell")
	{
		first = s.find("\"");
		temp = s.substr(first + 1, s.size());
		second = temp.find("\"");
		cellName = s.substr(first + 1, second);

		s = s.substr(first + second + 2, s.size());
		first = s.find("\"");
		temp = s.substr(first + 1, s.size());
		second = temp.find("\"");
		contents = s.substr(first + 1, second);
	}

	else if (val == "undoCell")
	{
		return val;
	}

	else if (val == "revertCell")
	{
		first = s.find("\"");
		temp = s.substr(first + 1, s.size());
		second = temp.find("\"");
		cellName = s.substr(first + 1, second);
	}

	return val;
}

std::string Server::connection_handler::split_and_delete(std::string& s)
{
	int loc = s.find("\n");
	std::string before = s.substr(0, loc);
	s = s.substr(loc + 1, std::string::npos);
	return before;
}

void Server::save_to_file(Spreadsheet s)
{
	std::string file_path = "./spreadsheets/" + s.get_name() + ".txt";
	std::ofstream file(file_path);
	file << s.get_json();
	file.close();
	spreadsheets->erase(spreadsheets->find(s.get_name()));
}

void Server::connection_handler::client_disconnected()
{
	sock.close();

	connections_lock.lock();
	std::map<int, connection_handler::pointer> connections = server->connections;
	for (std::map<int, connection_handler::pointer>::iterator it = connections.begin(); it != connections.end(); ++it)
	{
		//maybe get weird errors with other connection_handlers sending stuff at same time
		if (it->second.get()->curr_spreadsheet == curr_spreadsheet && it->second.get()->sock.is_open())
		{
			std::string message = "{ messageType: \"disconnected\", user: \"" + std::to_string(ID) + "\"" + "}\n";
			it->second.get()->sock.write_some(boost::asio::buffer(message, max_length));
		}
	}

	spreadsheets_lock.lock();
	if (server->spreadsheets->find(curr_spreadsheet) != server->spreadsheets->end())
		server->spreadsheets->at(curr_spreadsheet).delete_user(ID);
	spreadsheets_lock.unlock();
	server->connections.erase(ID);
	connections_lock.unlock();
}

Spreadsheet Server::load_from_file(std::string filename)
{
	std::string json;
	std::ifstream file(filename);
	std::getline(file, json);
	file.close();
	boost::json::value parsed = boost::json::parse(json);
	boost::json::object obj = parsed.as_object();

	// get name
	std::string name = boost::json::value_to<std::string>(obj.at("name"));

	// get cells
	std::map<std::string, Cell*>* cells = new std::map<std::string, Cell*>();
	boost::json::array cell_array = obj.at("cells").as_array();
	boost::json::object cell;
	std::string cell_name;
	std::string contents;
	for (boost::json::value val : cell_array)
	{
		cells->insert_or_assign(boost::json::value_to<std::string>(val.as_object().at("name")), parse_json_cell(val));
	}

	// get history
	std::stack<Cell*>* history = parse_json_history(obj.at("history").as_array());

	// get graph
	DependencyGraph* graph = new DependencyGraph();
	boost::json::array g_arr = obj.at("graph").as_array();
	for (int i = 0; i < g_arr.size(); i++)
	{
		boost::json::object g_obj = g_arr[i].as_object();
		std::string name = boost::json::value_to<std::string>(g_obj.at("name"));
		boost::json::array dpdee_arr = g_obj.at("dependees").as_array();
		std::vector<std::string> dpdees;
		for (int j = 0; j < dpdee_arr.size(); j++)
		{
			dpdees.push_back(boost::json::value_to<std::string>(dpdee_arr[j]));
		}
		graph->replace_dependees(name, dpdees);

		boost::json::array dpdnt_arr = g_obj.at("dependents").as_array();
		std::vector<std::string> dpdnts;
		for (int j = 0; j < dpdnt_arr.size(); j++)
		{
			dpdnts.push_back(boost::json::value_to<std::string>(dpdnt_arr[j]));
		}
		graph->replace_dependents(name, dpdnts);
	}

	Spreadsheet s(name, cells, history, graph);
	return s;
}

Cell* Server::parse_json_cell(boost::json::value c)
{
	boost::json::object cell = c.as_object();
	std::string cell_name = boost::json::value_to<std::string>(cell.at("name"));
	std::string contents = boost::json::value_to<std::string>(cell.at("contents"));

	Cell* prev = new Cell();
	try {
		cell.at("previous").as_object().at("previous");
		prev = parse_json_cell(cell.at("previous"));
	}
	catch (std::exception e) {
	}

	return new Cell(cell_name, contents, prev);
}

std::stack<Cell*>* Server::parse_json_history(boost::json::array c)
{
	std::stack<Cell*>* stack = new std::stack<Cell*>();
	for (int i = c.size() - 1; i >= 0; i--)
	{
		stack->push(parse_json_cell(c[i]));
	}
	return stack;
}