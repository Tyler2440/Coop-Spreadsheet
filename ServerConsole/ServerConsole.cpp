#include <iostream>
#include <fstream>
#include <boost/bind/bind.hpp>
#include <boost/asio.hpp>
#include <boost/thread.hpp>
#include "../Controller/server_controller.h"
#include "../Model/ServerSpreadsheet.h"
#include <boost/json.hpp>

using boost::asio::ip::tcp;

//typedef boost::shared_ptr<Server> server_ptr;
//typedef std::list<server_ptr> server_list;

int main(int argc, char **argv)
{  
  // create class instance
  Spreadsheet s("test");

  boost::json::object obj;                                                     // construct an empty object
  obj["pi"] = 3.141;                                            // insert a double
  obj["happy"] = true;                                          // insert a bool
  obj["name"] = "Boost";                                        // insert a string
  obj["nothing"] = nullptr;                                     // insert a null
  obj["answer"].emplace_object()["everything"] = 42;            // insert an object with 1 element
  obj["list"] = { 1, 0, 2 };                                    // insert an array with 3 elements
  obj["object"] = { {"currency", "USD"}, {"value", 42.99} };    // insert an object with 2 elements

  // Let's parse and serialize JSON
  std::cout << obj << std::endl;

  
  //boost::asio::io_context io_context;

  //Server server(io_context);

  //server.save_to_file();
    //try
    //{
    //    boost::asio::io_context io_context;      

    //    Server server(io_context);

    //    boost::thread main_loop([&]() { io_context.run(); });

    //    std::cout << "[SERVER] Server started!" << std::endl;
    //  
    //    std::string cmd;

    //    while (true)
    //    {      
    //        std::cin >> cmd;
    //       
    //        if (!cmd.empty())
    //        {
    //            if (cmd == "exit")
    //            {
    //                server.stop();
    //                return 0;
    //            }
    //        }
    //    }
    //}
    //catch (std::exception& e)
    //{
    //    std::cerr << e.what() << std::endl;
    //}
    return 0;
}