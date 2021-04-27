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
#include "server_controller.h"
#include "../Model/ServerSpreadsheet.h"

using boost::asio::ip::tcp;
using namespace boost::placeholders;
typedef boost::shared_ptr<Server::connection_handler> pointer;

int Server::next_ID;

// Code came from: https://www.codeproject.com/Articles/1264257/Socket-Programming-in-Cplusplus-using-boost-asio-T
Server::connection_handler::connection_handler(boost::asio::io_context& io_context, Server * s)
	: sock(io_context)
{
	server = s;
};

//constructor for accepting connection from client
Server::Server(boost::asio::io_context& io_context) : io_context_(io_context), acceptor(io_context, tcp::endpoint(tcp::v4(), 1100))
{
	spreadsheets = new std::map<std::string, Spreadsheet>();
	connections = new std::map<int, connection_handler::pointer>();
	Spreadsheet *test1 = new Spreadsheet("test1");
	test1->set_cell("A1", "jingle");
	test1->add_user("chad", 2);
	test1->select_cell(2, "B1");
	test1->add_user("abracadabra", 3);
	test1->select_cell(3, "B2");
	test1->add_user("d", 4);
	test1->select_cell(4, "B3");
	test1->set_cell("A2", "jangle");
	test1->set_cell("A3", "jongle");
	test1->set_cell("A4", "jungle");
	test1->set_cell("A5", "jyngle");
	spreadsheets->insert( std::pair<std::string, Spreadsheet>("test1", *test1 ));
	spreadsheets->insert( std::pair<std::string, Spreadsheet>("test2", *(new Spreadsheet("test2"))) );
	spreadsheets->insert( std::pair<std::string, Spreadsheet>("test3", *(new Spreadsheet("test3"))) );
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
	for (std::pair<int, connection_handler::pointer> connection : *connections)
	{
		connection.second.get()->socket().close();
	}
	io_context_.stop();
}

void Server::connection_handler::on_name(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		std::cout << "Client sent: \"" << r_buffer << "\"" << std::endl;
		//std::cout << "Enter spreadsheet name" << std::endl;
		client_name = r_buffer.substr(0, r_buffer.size() - 1);
		//std::cout << client_name << std::endl;

		r_buffer.clear();

		// LOCK next_ID
		server->connections->insert(std::pair<int, connection_handler::pointer>(next_ID, this));
		ID = next_ID;
		next_ID++;

		std::cout << "New client connected with ID: " << ID << " and Name: " << client_name << std::endl;

		// Send list of spreadsheet names to client
		sock.write_some(boost::asio::buffer(server->get_spreadsheets(), max_length));

		// Continue asynchronous handshake, look for spreadsheet choice
		boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(r_buffer), '\n',
			boost::bind(&connection_handler::on_spreadsheet, shared_from_this(),
				boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	}
	else {
		std::cerr << "error: " << err.message() << std::endl;
		client_disconnected();
	}
}

void Server::connection_handler::on_spreadsheet(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		std::cout << "Client sent: \"" << r_buffer << "\"" << std::endl;
		std::string spreadsheet_name = r_buffer.substr(0, r_buffer.size() - 1);

		r_buffer.clear();

		curr_spreadsheet = spreadsheet_name;

		//std::cout << spreadsheet_name << std::endl;

		//std::cout << server->spreadsheets->size() << std::endl;


		// LOCK THE SPREADSHEET

		// Send data of chosen spreadsheet
		Spreadsheet* spreadsheet;
		std::map<std::string, Spreadsheet>::iterator it = server->spreadsheets->find(spreadsheet_name);

		if (it != server->spreadsheets->end())
		{
			std::cout << "Client selected: " << spreadsheet_name << std::endl;
			spreadsheet = &server->spreadsheets->at(spreadsheet_name);
			std::map<std::string, Cell*>* cells = spreadsheet->get_cells();

			//std::cout << "here" << std::endl;
			// Send every edited cell
			for (std::map<std::string, Cell*>::iterator it = cells->begin(); it != cells->end(); ++it)
			{
				std::string message = "{ messageType: \"cellUpdated\", cellName: \"" + it->first + "\", contents: \"" + it->second->get_contents() + "\" }\n";
				//std::cout << "haha" << std::endl;
				sock.write_some(boost::asio::buffer(message, max_length));
			}

			std::map<int, User> users = spreadsheet->get_users();
			// Send every selected cell
			for (std::map<int, User>::iterator it = users.begin(); it != users.end(); ++it)
			{
				std::string message = "{ messageType: \"cellSelected\", cellName: \"" + it->second.get_selected() + "\", selector: " + std::to_string(it->first) + ", selectorName: \"" + it->second.get_name() + "\"}\n";
				//std::cout << "haha 2 electric bugaloo" << std::endl;
				sock.write_some(boost::asio::buffer(message, max_length));
			}
		}

		else
		{
			std::cout << "Client created: " << spreadsheet_name << std::endl;
			spreadsheet = new Spreadsheet(spreadsheet_name);
			server->spreadsheets->insert(std::pair<std::string, Spreadsheet>(spreadsheet_name, *(spreadsheet)));
		}

		spreadsheet->add_user(client_name, ID);

		std::string message = std::to_string(ID) + "\n";
		sock.write_some(boost::asio::buffer(message, max_length));

		// END OF LOCK

		boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(r_buffer), '\n',
			boost::bind(&connection_handler::handle_read, shared_from_this(),
				boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	}
	else {
		std::cerr << "error: " << err.message() << std::endl;
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

			std::cout << "Client request: \"" << request << "\"" << std::endl;

			std::string cellName;
			std::string contents;
			std::string request_name = find_request_type(request, cellName, contents);

			if (request_name == "selectCell")
			{
				// LOCK HERE
				server->spreadsheets->at(curr_spreadsheet).select_cell(ID, cellName);

				std::map<int, connection_handler::pointer>* connections = server->connections;
				for (std::map<int, connection_handler::pointer>::iterator it = connections->begin(); it != connections->end(); ++it)
				{
					//maybe get weird errors with other connection_handlers sending stuff at same time
					if (it->second.get()->curr_spreadsheet == curr_spreadsheet && it->second.get()->sock.is_open())
					{
						std::string message = "{ messageType: \"cellSelected\", cellName: \"" + cellName + "\", selector: \"" + std::to_string(ID) + "\", selectorName:  \"" + client_name + "\" }\n";
						std::cout << "Sending: \"" << message << "\"" << std::endl;
						it->second.get()->sock.write_some(boost::asio::buffer(message, max_length));
					}
				}
				// END LOCK HERE
			}

			else if (request_name == "editCell")
			{
				// LOCK HERE
				if (server->spreadsheets->at(curr_spreadsheet).set_cell(cellName, contents))
				{
					// ADD CELL CHANGE TO UNDO STACK

					std::map<int, connection_handler::pointer>* connections = server->connections;
					for (std::map<int, connection_handler::pointer>::iterator it = connections->begin(); it != connections->end(); ++it)
					{
						//maybe get weird errors with other connection_handlers sending stuff at same time
						if (it->second.get()->curr_spreadsheet == curr_spreadsheet && it->second.get()->sock.is_open())
						{
							std::string message = "{ messageType: \"cellUpdated\", cellName: \"" + cellName + "\", contents: \"" + contents + "\"" + "}\n";
							std::cout << message << std::endl;
							it->second.get()->sock.write_some(boost::asio::buffer(message, max_length));
						}
					}
					// END LOCK HERE
				}
				else
				{
					std::string invalid = "Edit request was invalid!";
					std::string message = "{ messageType: \"requestError\", cellName: \" " + cellName + "\", message: \"" + invalid + "\"" + "}\n";
					sock.write_some(boost::asio::buffer(message, max_length));
				}
			}

			else if (request_name == "undo")
			{
				if (!server->spreadsheets->at(curr_spreadsheet).get_history()->empty())
				{
					Cell* cell = server->spreadsheets->at(curr_spreadsheet).undo();

					// LOCK HERE
					//server->spreadsheets->at(curr_spreadsheet).set_cell(cell->get_name(), cell->get_contents());

					std::map<int, connection_handler::pointer>* connections = server->connections;
					for (std::map<int, connection_handler::pointer>::iterator it = connections->begin(); it != connections->end(); ++it)
					{
						//maybe get weird errors with other connection_handlers sending stuff at same time
						if (it->second.get()->curr_spreadsheet == curr_spreadsheet && it->second.get()->sock.is_open())
						{
							std::string message = "{ messageType: \"cellUpdated\", cellName: \"" + cell->get_name() + "\", contents: \"" + cell->get_contents() + "\"" + "}\n";
							std::cout << message << std::endl;
							it->second.get()->sock.write_some(boost::asio::buffer(message, max_length));
						}
					}
					// END LOCK HERE
				}
			}

			else if (request_name == "revertCell")
			{
				// GET REVERT CHANGE FROM THE GIVEN CELL, UPDATE CONTENTS TO CORRECT VALUE (THIS FUNCTION CALL SHOULD REMOVE THE CHANGE)
				Cell* cell = server->spreadsheets->at(curr_spreadsheet).get_cell(cellName);

				if (!cell->get_history()->empty())
				{
					std::string new_contents = cell->get_previous_change();

					// LOCK HERE
					// THIS LINE UTILIZES THE CHANGE FROM THE REVERT STACK TO EDIT THE CELL
					server->spreadsheets->at(curr_spreadsheet).set_cell(cell->get_name(), new_contents);

					// REMOVE CHANGE FROM UNDO STACK

					std::map<int, connection_handler::pointer>* connections = server->connections;
					for (std::map<int, connection_handler::pointer>::iterator it = connections->begin(); it != connections->end(); ++it)
					{
						//maybe get weird errors with other connection_handlers sending stuff at same time
						if (it->second.get()->curr_spreadsheet == curr_spreadsheet && it->second.get()->sock.is_open())
						{
							std::string message = "{ messageType: \"cellUpdated\", cellName: \"" + cell->get_name() + "\", contents: \"" + new_contents + "\"" + "}\n";
							std::cout << message << std::endl;
							it->second.get()->sock.write_some(boost::asio::buffer(message, max_length));
						}
					}
					// END LOCK HERE
				}
			}
		}

		s_buffer = r_buffer;

		boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(r_buffer), '\n',
			boost::bind(&connection_handler::handle_read, shared_from_this(),
				boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	}
	else
	{
		std::cerr << "error: " << err.message() << std::endl;
		client_disconnected();
	}
}

void Server::connection_handler::handle_write(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		std::cout << "Server sent Hello message!" << std::endl;
	}
	else {
		std::cerr << "error: " << err.message() << std::endl;
		client_disconnected();
	}
}

void Server::start_accept()
{
	// socket
	connection_handler::pointer connection(new connection_handler(io_context_, this));

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
	for (std::map<std::string, Spreadsheet>::iterator it = Server::spreadsheets->begin(); it != Server::spreadsheets->end(); ++it)
	{
		spreadsheets.append(it->first + "\n");
	}
	// END LOCK HERE
	return spreadsheets + "\n";
}

std::string Server::connection_handler::find_request_type(std::string s, std::string& cellName, std::string& contents)
{
	int first = s.find("\"");
	std::string temp = s.substr(first + 1, s.size());
	int second = temp.find("\"");
	std::string val = s.substr(first + 1, second);
	std::cout << val << std::endl;
	s = s.substr(first + second + 2, s.size());

	if (val == "selectCell")
	{
		first = s.find("\"");
		temp = s.substr(first + 1, s.size());
		second = temp.find("\"");
		cellName = s.substr(first + 1, second);
		std::cout << cellName << std::endl;
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

void Server::save_to_file()
{
	//for (std::map<std::string, Spreadsheet>::iterator it = spreadsheets->begin(); it != spreadsheets->end(); ++it)
	//{
	//	it->second.serialize()
	//}
	//boost::json::serialize(boost::json::object());
	//boost::json::serializer s;
	//s.reset()
	//char buffer[1024];
	//s.read(buffer);
}

void Server::connection_handler::client_disconnected()
{
	std::map<int, connection_handler::pointer>* connections = server->connections;
	for (std::map<int, connection_handler::pointer>::iterator it = connections->begin(); it != connections->end(); ++it)
	{
		//maybe get weird errors with other connection_handlers sending stuff at same time
		if (it->second.get()->curr_spreadsheet == curr_spreadsheet && it->second.get()->sock.is_open())
		{
			std::string message = "{ messageType: \"disconnected\", user: \"" + std::to_string(ID) + "\"" + "}\n";
			std::cout << message << std::endl;
			it->second.get()->sock.write_some(boost::asio::buffer(message, max_length));
		}
	}

	sock.close();
}