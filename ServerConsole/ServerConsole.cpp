#include <iostream>
#include <boost/bind/bind.hpp>
#include <boost/asio.hpp>
#include "../Controller/server_controller.h"

using boost::asio::ip::tcp;

//typedef boost::shared_ptr<Server> server_ptr;
//typedef std::list<server_ptr> server_list;

int main()
{ 
    try
    {
        boost::asio::io_context io_context;      

        Server server(io_context);
        std::cout << "[SERVER] Server started!" << std::endl;
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

//std::map<std::string, Spreadsheet> spreadsheets = Server::get_spreadsheets();
//for (std::map<std::string, Spreadsheet>::iterator it = spreadsheets.begin(); it != spreadsheets.end(); ++it)
//{
//    connection->socket().write_some(boost::asio::buffer(it->first, 1024));
//}