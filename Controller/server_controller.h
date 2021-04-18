#include <iostream>
#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/enable_shared_from_this.hpp>

using boost::asio::ip::tcp;

class connection_handler : public boost::enable_shared_from_this<connection_handler>
{
    private:
        tcp::socket sock;
        //std::string message = "Hello From Server!";
        std::string name;
        int ID;
        enum { max_length = 1024 };
        char data[max_length];

    public:
        typedef boost::shared_ptr<connection_handler> pointer;
        connection_handler(boost::asio::io_context& io_context);

        // creating the pointer
        static pointer create(boost::asio::io_context& io_context);

        //socket creation
        tcp::socket& socket();

        void start();

        void handle_read(const boost::system::error_code& err, size_t bytes_transferred);
        void handle_write(const boost::system::error_code& err, size_t bytes_transferred);
};

class Server
{
    private:
        tcp::acceptor acceptor;
        boost::asio::io_context& io_context_;
        void start_accept();
        void handle_accept(connection_handler::pointer connection_handler, const boost::system::error_code& err);

    public:
        static int next_ID;
        //constructor for accepting clients
        Server(boost::asio::io_context& io_context);
};