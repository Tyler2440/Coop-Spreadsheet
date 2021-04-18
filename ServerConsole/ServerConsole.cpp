#include <iostream>
#include <boost/asio.hpp>
#include "server_controller.h"

using boost::asio::ip::tcp;

typedef boost::shared_ptr<Server> server_ptr;
typedef std::list<server_ptr> server_list;

int main()
{ 
    try
    {
        boost::asio::io_context io_context;
        //server_ptr server(new Server(io_context));
        Server server(io_context);
        io_context.run();

        std::string cmd;
        while (true)
        {
          std::cin >> cmd;
          if (cmd == "exit")
          {
            return 0;
          }
        }
    }
    catch (std::exception& e)
    {
        std::cerr << e.what() << std::endl;
    }
    return 0;
}

//std::string read(tcp::socket& socket) {
//    boost::asio::streambuf buf;
//    boost::asio::read_until(socket, buf, "\n");
//    std::string data = boost::asio::buffer_cast<const char*>(buf.data());
//    return data;
//}
//
//void send(tcp::socket& socket, const std::string& message) {
//    const std::string msg = message + "\n";
//    boost::asio::write(socket, boost::asio::buffer(message));
//}