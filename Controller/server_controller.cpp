#pragma once

#include <iostream>
#include <boost/algorithm/string.hpp>
#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/enable_shared_from_this.hpp>
#include "server_controller.h"
#include "../Model/ServerSpreadsheet.h"
#include <map>

using boost::asio::ip::tcp;
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
	Spreadsheet *test1 = new Spreadsheet();
	//test1->set_cell("a1", "jingle");
	//test1->add_user("chad", 2);
	//test1->select_cell(2, "a1");
	spreadsheets->insert( std::pair<std::string, Spreadsheet>("test1", *test1 ));
	spreadsheets->insert( std::pair<std::string, Spreadsheet>("test2", *(new Spreadsheet())) );
	spreadsheets->insert( std::pair<std::string, Spreadsheet>("test3", *(new Spreadsheet())) );
	//Server::next_ID = 0;
	start_accept();
}

//socket creation
tcp::socket& Server::connection_handler::socket()
{
	return sock;
}

void Server::connection_handler::start()
{
	boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(buffer),
		'\n', boost::bind(&connection_handler::on_name, shared_from_this(), boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
}

void Server::connection_handler::on_name(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		
		//std::cout << "Enter spreadsheet name" << std::endl;
		client_name = buffer.substr(0, buffer.size() - 2);
		//std::cout << client_name << std::endl;

		buffer.clear();

		// LOCK next_ID
		server->connections.insert(std::pair<int, connection_handler::pointer>(next_ID, this));
		ID = next_ID;
		next_ID++;

		std::cout << "New client connected" << std::endl;

		// Send list of spreadsheet names to client
		sock.write_some(boost::asio::buffer(server->get_spreadsheets(), max_length));

		// Continue asynchronous handshake, look for spreadsheet choice
		boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(buffer), '\n',
			boost::bind(&connection_handler::on_spreadsheet, shared_from_this(),
				boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	}
	else {
		std::cerr << "error: " << err.message() << std::endl;
		sock.close();
	}
}

void Server::connection_handler::on_spreadsheet(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		std::string spreadsheet_name = buffer.substr(0, buffer.size() - 2);;

		buffer.clear();

		curr_spreadsheet = spreadsheet_name;

		//std::cout << spreadsheet_name << std::endl;

		//std::cout << server->spreadsheets->size() << std::endl;


		// LOCK THE SPREADSHEET

		// Send data of chosen spreadsheet
		Spreadsheet &spreadsheet = server->spreadsheets->at(spreadsheet_name);
		std::map<std::string, Cell> cells = spreadsheet.get_cells();

		//std::cout << "here" << std::endl;
		// Send every edited cell
		for (std::map<std::string, Cell>::iterator it = cells.begin(); it != cells.end(); ++it)
		{
			std::string message = "{ messageType: \"cellUpdated\", cellName: \"" + it->first + "\", contents: \"" + it->second.get_contents() + "\" }\n";
			//std::cout << "haha" << std::endl;
			sock.write_some(boost::asio::buffer(message, max_length));
		}

		std::map<int, User> users = spreadsheet.get_users();
		// Send every selected cell
		for (std::map<int, User>::iterator it = users.begin(); it != users.end(); ++it)
		{
			//std::string message = "{ messageType: \"cellSelected\", cellName: \"" + it->second.get_selected() + "\" selector: " + std::to_string(it->first) + ", selectorName: \"" + it->second.get_name() + "\"}";
			std::string message = "a person\n";
			//std::cout << "haha 2 electric bugaloo" << std::endl;
			sock.write_some(boost::asio::buffer(message, max_length));
		}

		spreadsheet.add_user(client_name, ID);

		// END OF LOCK

		boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(buffer), '\n',
			boost::bind(&connection_handler::handle_read, shared_from_this(),
				boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	}
	else {
		std::cerr << "error: " << err.message() << std::endl;
		sock.close();
	}
}

void Server::connection_handler::handle_read(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		std::cout << "sending " << buffer << std::endl;

		// maybe LOCK this?

		// go through each connection and send the data to those on the same spreadsheet
		std::map<int, connection_handler::pointer> connections = server->connections;
		for (std::map<int, connection_handler::pointer>::iterator it = connections.begin(); it != connections.end(); ++it)
		{
			//maybe get wierd errors with other connection_handlers sending stuff at same time
			if (it->second.get()->curr_spreadsheet == curr_spreadsheet && it->second.get()->sock.is_open())
				it->second.get()->sock.write_some(boost::asio::buffer(buffer, max_length));
		}

		// end of LOCK

		buffer.clear();

		boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(buffer), '\n',
			boost::bind(&connection_handler::handle_read, shared_from_this(),
				boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	}
	else {
		std::cerr << "error: " << err.message() << std::endl;
		sock.close();
	}
}

void Server::connection_handler::handle_write(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		std::cout << "Server sent Hello message!" << std::endl;
	}
	else {
		std::cerr << "error: " << err.message() << std::endl;
		sock.close();
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
	std::string spreadsheets = "";
	for (std::map<std::string, Spreadsheet>::iterator it = Server::spreadsheets->begin(); it != Server::spreadsheets->end(); ++it)
	{
		spreadsheets.append(it->first + "\n");
	}
	return spreadsheets + "\n";
}