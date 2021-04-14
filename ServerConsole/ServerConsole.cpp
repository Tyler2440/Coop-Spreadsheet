#include <iostream>
#include <boost/asio.hpp>

using boost::asio::ip::tcp;

std::string read(tcp::socket& socket);
void send(tcp::socket& socket, const std::string& message);

int main()
{
    std::cout << "Hello World!\n";
    try {
        boost::asio::io_service io_service;
        std::cout << "Accepting Clients!" << std::endl;
        tcp::acceptor accepter(io_service,tcp::endpoint(tcp::v4(), 1100));
        boost::asio::ip::tcp::socket* socket;
        while (true)
        {               
            for (;;)
            {
                socket = new boost::asio::ip::tcp::socket(io_service);
                accepter.accept(*socket);
                std::cout << "Client Accepted!" << std::endl;
                std::string messaget = read(*socket);
                std::cout << "MESSAGE FROM CLIENT: " << messaget << std::endl;
                std::string message = "yo!";
                //boost::system::error_code ignored_error;
                send(*socket, message);
                Sleep(1000);
                send(*socket, "Austin is dumb");
                //socket->write_some("yo!");
                break;                
            }
            std::string messagets = read(*socket);
            std::cout << "MESSAGE FROM CLIENT: " << messagets << std::endl;

            messagets = read(*socket);
            std::cout << "MESSAGE FROM CLIENT: " << messagets << std::endl;
            
        }
    }
    catch (std::exception& e)
    {
        std::cout << e.what() << std::endl;
    }

    return 0;
}

std::string read(tcp::socket& socket) {
    boost::asio::streambuf buf;
    boost::asio::read_until(socket, buf, "\n");
    std::string data = boost::asio::buffer_cast<const char*>(buf.data());
    return data;
}

void send(tcp::socket& socket, const std::string& message) {
    const std::string msg = message + "\n";
    boost::asio::write(socket, boost::asio::buffer(message));
}