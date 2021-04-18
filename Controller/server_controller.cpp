#pragma once

#include <iostream>
#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/enable_shared_from_this.hpp>
#include "server_controller.h"
#include "ServerSpreadsheet.h"
#include <map>

using boost::asio::ip::tcp;
typedef boost::shared_ptr<connection_handler> pointer;

// Code came from: https://www.codeproject.com/Articles/1264257/Socket-Programming-in-Cplusplus-using-boost-asio-T
connection_handler::connection_handler(boost::asio::io_context& io_context)
	: sock(io_context)
{
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
tcp::socket& connection_handler::socket()
{
	return sock;
}

void connection_handler::start(std::string spreadsheets)
{
	sock.read_some(boost::asio::buffer(data, max_length));

	name = data;

	std::string message = spreadsheets;

	sock.write_some(boost::asio::buffer(message, max_length));
	
	boost::asio::async_read(sock, boost::asio::buffer(data, max_length), 
		boost::bind(&connection_handler::handle_read, shared_from_this(), 
		boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
}

void connection_handler::handle_read(const boost::system::error_code& err, size_t bytes_transferred)
{
	if (!err) {
		std::cout << data << std::endl;
	}
	else {
		std::cerr << "error: " << err.message() << std::endl;
		sock.close();
	}
}

void connection_handler::handle_write(const boost::system::error_code& err, size_t bytes_transferred)
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
	connection_handler::pointer connection(new connection_handler(io_context_));

	// asynchronous accept operation and wait for a new connection.
	acceptor.async_accept(connection->socket(),
		boost::bind(&Server::handle_accept, this, connection,
			boost::asio::placeholders::error));
}

void Server::handle_accept(connection_handler::pointer connection, const boost::system::error_code& err)
{
	if (!err)
		connection->start(get_spreadsheets());

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