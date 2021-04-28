#include <iostream>
#include <fstream>
#include <boost/bind/bind.hpp>
#include <boost/asio.hpp>
#include <boost/thread.hpp>
#include <boost/json/src.hpp>
#include "../Controller/server_controller.h"
#include "../Model/ServerSpreadsheet.h"

using boost::asio::ip::tcp;

int main(int argc, char **argv)
{  
    try
    {
        boost::asio::io_context io_context;      

        Server server(io_context);

        boost::thread main_loop([&]() { io_context.run(); });
      
        std::string cmd;

        while (true)
        {      
            std::cin >> cmd;
           
            if (!cmd.empty())
            {
                if (cmd == "exit")
                {
                    server.stop();
                    return 0;
                }
            }
        }
    }
    catch (std::exception& e)
    {
        std::cerr << e.what() << std::endl;
    }
    return 0;
}