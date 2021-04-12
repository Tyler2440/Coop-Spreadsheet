#include <iostream>
#include <boost/asio.hpp>
#include <boost/asio/io_service.hpp>

using boost::asio::ip::tcp;

int main()
{
    std::cout << "Hello World!\n";
    try {
        boost::asio::io_service io_service;
        while (true)
        {
            tcp::acceptor accepter(io_service, tcp::endpoint(tcp::v4(), 1100));
            std::cout << "Accepting Clients!" << std::endl;
            for (;;)
            {
                tcp::socket socket(io_service);
                accepter.accept(socket);
                socket.send("yo!");
            }
        }
    }
    catch (std::exception& e)
    {
        std::cout << e.what() << std::endl;
    }

    return 0;
}