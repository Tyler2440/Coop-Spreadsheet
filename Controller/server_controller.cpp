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
	spreadsheets->insert( std::pair<std::string, Spreadsheet>("test1", *(new Spreadsheet())) );
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
	sock.async_read_some(boost::asio::buffer(data, max_length),
		boost::bind(&connection_handler::on_connect, shared_from_this(),
			boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	/*
	name = data;

	std::string message = spreadsheets;

	sock.write_some(boost::asio::buffer(message, max_length));
	
	boost::asio::async_read(sock, boost::asio::buffer(data, max_length), 
		boost::bind(&connection_handler::handle_read, shared_from_this(), 
		boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
		*/
}

void Server::connection_handler::on_connect(const boost::system::error_code& err, size_t bytes_transferred)
{
	
	/*
	boost::split(stuff, data, boost::is_any_of("\n"));

	std::cout << stuff[0] << std::endl;
	std::cout << stuff.size() << std::endl;

	if (stuff.size() <= 1)
	{
		sock.async_read_some(boost::asio::buffer(data, max_length),
			boost::bind(&connection_handler::wait_for_newline, shared_from_this(),
				boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	}
	else
		on_name(err, bytes_transferred);*/
	boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(fdsa), 
		'\n', boost::bind(&connection_handler::on_name, shared_from_this(), boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));

		//// Start asynchrounous handshake, look for name
		//sock.async_read_some(boost::asio::buffer(data, max_length),
		//	boost::bind(&connection_handler::on_name, shared_from_this(),
		//		boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
}

void Server::connection_handler::on_name(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		
		std::cout << "Enter spreadsheet name" << std::endl;
		client_name = fdsa;
		std::cout << client_name << std::endl;

		fdsa.clear();

		// LOCK THIS
		ID = next_ID;
		next_ID++;

		std::cout << "New client connected" << std::endl;

		// Send list of spreadsheet names to client
		sock.write_some(boost::asio::buffer(server->get_spreadsheets(), max_length));

		// Continue asynchronous handshake, look for spreadsheet choice
		boost::asio::async_read_until(sock, boost::asio::dynamic_buffer(fdsa), '\n',
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
		std::string spreadsheet_name = fdsa;

		std::cout << spreadsheet_name << std::endl;

		std::cout << server->spreadsheets->size() << std::endl;

		// Send data of chosen spreadsheet
		Spreadsheet &spreadsheet = server->spreadsheets->at(spreadsheet_name);
		std::map<std::string, Cell> cells = spreadsheet.get_cells();

		std::cout << "here" << std::endl;
		// Send every edited cell
		for (std::map<std::string, Cell>::iterator it = cells.begin(); it != cells.end(); ++it)
		{
			std::string message = "{ messageType: \"cellUpdated\", cellName: \"" + it->first + "\", contents: \"" + it->second.get_contents() + "\" }\n";
			std::cout << "haha" << std::endl;
			sock.write_some(boost::asio::buffer(message, max_length));
		}

		// Send every selected cell


		spreadsheet.add_user(client_name, ID);
	}
	else {
		std::cerr << "error: " << err.message() << std::endl;
		sock.close();
	}
}

void Server::connection_handler::handle_read(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		std::cout << data << std::endl;
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