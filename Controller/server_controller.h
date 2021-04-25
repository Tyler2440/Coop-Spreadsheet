#ifndef SERVER_CONTROLLER
#define SERVER_CONTROLLER

#include <iostream>
#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/enable_shared_from_this.hpp>
#include "../Model/ServerSpreadsheet.h"
#include <map>

using boost::asio::ip::tcp;



class Server
{
public:
  class connection_handler : public boost::enable_shared_from_this<connection_handler>
  {
  private:
    Server* server;
    tcp::socket sock;
    //std::string message = "Hello From Server!";
    // the spreadsheet this connection/client is currently on
    std::string curr_spreadsheet;
    std::string client_name;
    // buffer for storing incomplete messages
    std::string s_buffer;
    // buffer for storing the recieved string
    std::string r_buffer;
    int ID;
    enum { max_length = 2048 };
    char data[max_length];

  public:
    typedef boost::shared_ptr<connection_handler> pointer;
    //connection_handler(boost::asio::io_context& io_context);
    connection_handler(boost::asio::io_context& io_context, Server * s);

    // creating the pointer
    static pointer create(boost::asio::io_context& io_context);

    //socket creation
    tcp::socket& socket();

    void start();

    void on_spreadsheet(const boost::system::error_code& err, size_t bytes_transferred);
    void on_name(const boost::system::error_code& err, size_t bytes_transferred);
    void handle_read(const boost::system::error_code& err, size_t bytes_transferred);
    void handle_write(const boost::system::error_code& err, size_t bytes_transferred);

  private:
      static std::string find_request_type(std::string s, std::string& cellName, std::string& contents);
      // Split the given string by the first \n character, deleting the part before it and returning it
      static std::string split_and_delete(std::string& s);
  };

private:
  std::map<std::string, Spreadsheet>* spreadsheets;
  std::map<int, connection_handler::pointer>* connections;
  tcp::acceptor acceptor;
  boost::asio::io_context& io_context_;
  void start_accept();
  void handle_accept(connection_handler::pointer connection_handler, const boost::system::error_code& err);
  

public:
  static int next_ID;

  //constructor for accepting clients
  Server(boost::asio::io_context& io_context);
  std::string get_spreadsheets();
  void stop();
  void save_to_file();
};

#endif