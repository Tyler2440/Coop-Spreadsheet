#pragma once

#include <iostream>
#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/enable_shared_from_this.hpp>
#include "server_controller.h"

// Code came from: https://www.codeproject.com/Articles/1264257/Socket-Programming-in-Cplusplus-using-boost-asio-T

using boost::asio::ip::tcp;
typedef boost::shared_ptr<connection_handler> pointer;

int Server::next_ID;

connection_handler::connection_handler(boost::asio::io_context& io_context) : sock(io_context) 
{
	ID = 0;
};

//constructor for accepting connection from client
Server::Server(boost::asio::io_context& io_context) : io_context_(io_context), acceptor(io_context, tcp::endpoint(tcp::v4(), 1100))
{
  Server::next_ID = 0;
	start_accept();
}

// creating the pointer
pointer connection_handler::create(boost::asio::io_context& io_context)
{
	return pointer(new connection_handler(io_context));
}

//socket creation
tcp::socket& connection_handler::socket()
{
	return sock;
}

void connection_handler::start()
{
	sock.read_some(boost::asio::buffer(data, max_length));

	name = data;

	//LOCK THIS
	ID = Server::next_ID;
	Server::next_ID++;

	std::string message = std::to_string(ID) + " Fake list of spreadsheets!";

	sock.write_some(boost::asio::buffer(message));

	sock.read_some(boost::asio::buffer(data, max_length));

	std::cout << data << std::endl;
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
	connection_handler::pointer connection = connection_handler::create(io_context_);

	// asynchronous accept operation and wait for a new connection.
	acceptor.async_accept(connection->socket(),
		boost::bind(&Server::handle_accept, this, connection,
			boost::asio::placeholders::error));
}

void Server::handle_accept(connection_handler::pointer connection, const boost::system::error_code& err)
{
	if (!err) {
		connection->start();
	}
	start_accept();
}