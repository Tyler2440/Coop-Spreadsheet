#include <iostream>
#include <boost/asio.hpp>

//using boost::asio::ip::tcp;

int main()
{
    std::cout << "Hello World!\n";
    try {
        boost::asio::io_service io_service;
        std::cout << "Accepting Clients!" << std::endl;
        boost::asio::ip::tcp::acceptor accepter(io_service, boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), 1100));
        while (true)
        {               
            for (;;)
            {
                boost::asio::ip::tcp::socket * socket =  new boost::asio::ip::tcp::socket(io_service);
                accepter.accept(*socket);
                std::cout << "Client Accepted!" << std::endl;
                std::string message = "yo!";
                boost::system::error_code ignored_error;
                boost::asio::write(*socket, boost::asio::buffer(message), ignored_error);
                //socket.send("yo!");
            }
        }
    }
    catch (std::exception& e)
    {
        std::cout << e.what() << std::endl;
    }

    return 0;
}