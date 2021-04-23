#include <iostream>
#include <boost/asio.hpp>
#include <regex>

using boost::asio::ip::tcp;
using namespace std::chrono_literals;

boost::asio::io_context io_context;
tcp::socket sock = tcp::socket(io_context);
std::string buffer = "";
std::condition_variable cv;

// Forward declarations
bool TestConnection(std::string ip, int port);
bool TestSendName(std::string ip, int port);
bool TestReceiveSpreadsheets(std::string ip, int port);
bool TestSendSpreadsheetName(std::string ip, int port);
bool TestSendNewSpreadsheetName(std::string ip, int port);

int main(int argc, char** argv)
{
    std::string ipPort = argv[2];
    int testNum = std::stoi(argv[1]);
    int i = ipPort.find(":");

    std::string ip = ipPort.substr(0, i);
    int port = std::stoi(ipPort.substr(i + 1, ipPort.size()));

    bool test = false;

    switch (testNum)
    {
    case 1:
    {
        std::cout << "5 seconds" << std::endl;

        std::mutex m;
        std::condition_variable cv;
        int retValue;

        std::thread t([&cv, &retValue, &ip, &port]()
            {
                retValue = TestConnection(ip, port);
                cv.notify_one();
            });

        t.detach();

        {
            std::unique_lock<std::mutex> l(m);
            if (cv.wait_for(l, 5s) == std::cv_status::timeout)
                throw std::runtime_error("False");
        }

        std::cout << "Returned with value: " << retValue << std::endl;

        return retValue;

        // auto task = std::async(TestConnection);

        break;
    }

    case 2:
    {
        std::cout << "5 seconds" << std::endl;

        std::mutex m;
        std::condition_variable cv;
        int retValue;

        std::thread t([&cv, &retValue, &ip, &port]()
            {
                retValue = TestSendName(ip, port);
                cv.notify_one();
            });

        t.detach();

        {
            std::unique_lock<std::mutex> l(m);
            if (cv.wait_for(l, 5s) == std::cv_status::timeout)
                throw std::runtime_error("False");
        }

        std::cout << "Returned with value: " << retValue << std::endl;

        return retValue;

        // auto task = std::async(TestConnection);

        break;
    }

    case 3:
    {
        std::cout << "5 seconds" << std::endl;

        std::mutex m;
        std::condition_variable cv;
        int retValue;

        std::thread t([&cv, &retValue, &ip, &port]()
            {
                retValue = TestReceiveSpreadsheets(ip, port);
                cv.notify_one();
            });

        t.detach();

        {
            std::unique_lock<std::mutex> l(m);
            if (cv.wait_for(l, 5s) == std::cv_status::timeout)
                throw std::runtime_error("False");
        }

        std::cout << "Returned with value: " << retValue << std::endl;

        return retValue;

        // auto task = std::async(TestConnection);

        break;
    }

    case 4:
    {
        std::cout << "5 seconds" << std::endl;

        std::mutex m;
        std::condition_variable cv;
        int retValue;

        std::thread t([&cv, &retValue, &ip, &port]()
            {
                retValue = TestSendSpreadsheetName(ip, port);
                cv.notify_one();
            });

        t.detach();

        {
            std::unique_lock<std::mutex> l(m);
            if (cv.wait_for(l, 5s) == std::cv_status::timeout)
                throw std::runtime_error("False");
        }

        std::cout << "Returned with value: " << retValue << std::endl;

        return retValue;

        // auto task = std::async(TestConnection);

        break;
    }

    case 5:
    {
        std::cout << "5 seconds" << std::endl;

        std::mutex m;
        std::condition_variable cv;
        int retValue;

        std::thread t([&cv, &retValue, &ip, &port]()
            {
                retValue = TestSendNewSpreadsheetName(ip, port);
                cv.notify_one();
            });

        t.detach();

        {
            std::unique_lock<std::mutex> l(m);
            if (cv.wait_for(l, 5s) == std::cv_status::timeout)
                throw std::runtime_error("False");
        }

        std::cout << "Returned with value: " << retValue << std::endl;

        return retValue;

        // auto task = std::async(TestConnection);

        break;
    }

    case 6:
    {
        std::cout << "5 seconds" << std::endl;

        std::mutex m;
        std::condition_variable cv;
        int retValue;

        std::thread t([&cv, &retValue, &ip, &port]()
            {
                retValue = TestReceiveSpreadsheetCells(ip, port);
                cv.notify_one();
            });

        t.detach();

        {
            std::unique_lock<std::mutex> l(m);
            if (cv.wait_for(l, 5s) == std::cv_status::timeout)
                throw std::runtime_error("False");
        }

        std::cout << "Returned with value: " << retValue << std::endl;

        return retValue;

        // auto task = std::async(TestConnection);

        break;
    }
    }
}

bool TestConnection(std::string ip, int port)
{
  try
  {
    sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));
  }
  catch (std::exception e)
  {
    return false;
  }

  sock.close();
  return true;
}

bool TestSendName(std::string ip, int port)
{
    try
    {
        // Connect to server
        sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));

        // Send name
        sock.send("Chad\n");
    }
    catch (std::exception e)
    {
        return false;
    }

    sock.close();
    return true;
}

bool TestReceiveSpreadsheets(std::string ip, int port)
{
    try
    {
        // Connect to server
        sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));

        // Send name
        sock.send("Chad\n");

        // Receive the names of the spreadsheets
        sock.receive(buffer);
    }
    catch (std::exception e)
    {
        return false;
    }

    sock.close();
    return true;
}

bool TestSendSpreadsheetName(std::string ip, int port)
{
    try
    {
        // Connect to server
        sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));

        // Send name
        sock.send("Chad\n");

        // Receive the names of the spreadsheets
        sock.receive(buffer);

        // Send a name of existing spreadsheet
        sock.send("test1\n");
    }
    catch (std::exception e)
    {
        return false;
    }

    sock.close();
    return true;
}

bool TestSendNewSpreadsheet(std::string ip, int port)
{
    try
    {
        // Connect to server
        sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));

        // Send name
        sock.send("Chad\n");

        // Receive the names of the spreadsheets
        sock.receive(buffer);

        // Send a name of new spreadsheet
        sock.send("newspreadsheet\n");
    }
    catch (std::exception e)
    {
        return false;
    }

    sock.close();
    return true;
}

// Finish seperating cells the server sends into individual cells (whether that is with regex or some other magic)
bool TestReceiveSpreadsheetCells(std::string ip, int port)
{
    try
    {
        // Connect to server
        sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));

        // Send name
        sock.send("Chad\n");

        // Receive the names of the spreadsheets
        sock.receive(buffer);

        // Send a name of new spreadsheet
        sock.send("newspreadsheet\n");

        bool done = false;
        while (done == false)
        {
            buffer = "";
            sock.receive(buffer);

        }
    }
    catch (std::exception e)
    {
        return false;
    }

    sock.close();
    return true;
}
/*
    public static bool TestReceiveSpreadsheetCells()
    {
            bool done = false;
            while (done == false)
            {
                socket.Receive(bytes);
                //Console.WriteLine(Encoding.UTF8.GetString(bytes));
                string names = Encoding.UTF8.GetString(bytes);
                string[] parts = Regex.Split(names, @"(?<=[\n])");

                foreach (string part in parts)
                {
                    if (part == "3\n")
                    {
                        done = true;
                    }
                }
            }
    }
}*/