#include <iostream>
#include <boost/bind/bind.hpp>
#include <boost/asio.hpp>
#include <boost/thread.hpp>
#include "../Controller/server_controller.h"

using boost::asio::ip::tcp;

//typedef boost::shared_ptr<Server> server_ptr;
//typedef std::list<server_ptr> server_list;

int main(int argc, char **argv)
{    
    try
    {
        boost::asio::io_context io_context;      

        Server server(io_context);

        boost::thread main_loop([&]() { io_context.run(); });

        std::cout << "[SERVER] Server started!" << std::endl;
      
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