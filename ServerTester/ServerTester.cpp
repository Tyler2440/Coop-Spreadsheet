#include <iostream>
#include <boost/asio.hpp>
#include <regex>

using boost::asio::ip::tcp;
using namespace std::chrono_literals;

boost::asio::io_context io_context;
tcp::socket sock= tcp::socket(io_context);
std::string buffer = "";
std::condition_variable cv;

// Forward declarations
bool TestConnection(std::string ip, int port);

int main(int testNum, std::string ipPort)
{
    int i = ipPort.find(":");
    std::string ip = ipPort.substr(0, i);
    int port = stoi(ipPort.substr(i + 1, ipPort.size()));
     
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
    return true;
}

/*
            case 2:
                {
                    Console.WriteLine("5 seconds");

                    var task = Task.Run(() =>
                    {
                        test = TestSendName();
                    });

                    bool isCompletedSucessfully = task.Wait(TimeSpan.FromSeconds(5));

                    if (isCompletedSucessfully)
                        Console.WriteLine(test);
                    else
                        Console.WriteLine("False");
                    break;
                }
            case 3:
                {
                    Console.WriteLine("5 seconds");

                    var task = Task.Run(() =>
                    {
                        test = TestReceiveSpreadsheets();
                    });

                    bool isCompletedSucessfully = task.Wait(TimeSpan.FromSeconds(5));

                    if (isCompletedSucessfully)
                        Console.WriteLine(test);
                    else
                        Console.WriteLine("False");
                    break;
                }
            case 4:
                {
                    Console.WriteLine("5 seconds");

                    var task = Task.Run(() =>
                    {
                        test = TestSendSpreadsheetName();
                    });

                    bool isCompletedSucessfully = task.Wait(TimeSpan.FromSeconds(5));

                    if (isCompletedSucessfully)
                        Console.WriteLine(test);
                    else
                        Console.WriteLine("False");
                    break;
                }
            case 5:
                {
                    Console.WriteLine("5 seconds");

                    var task = Task.Run(() =>
                    {
                        test = TestSendNewSpreadsheet();
                    });

                    bool isCompletedSucessfully = task.Wait(TimeSpan.FromSeconds(5));

                    if (isCompletedSucessfully)
                        Console.WriteLine(test);
                    else
                        Console.WriteLine("False");
                    break;
                }
            case 6:
                {
                    //Console.WriteLine(TestReceiveSpreadsheetCells() + "\n");
                    Console.WriteLine("5 seconds");

                    var task = Task.Run(() =>
                    {
                        test = TestReceiveSpreadsheetCells();
                    });

                    bool isCompletedSucessfully = task.Wait(TimeSpan.FromSeconds(5));

                    if (isCompletedSucessfully)
                        Console.WriteLine(test);
                    else
                        Console.WriteLine("Doesn't Finish");

                    break;
                }        
        }
    }

    public static bool TestConnection()
    {
        try { 
            socket.Connect(address, int.Parse(ipPortArray[1]));
        }
        catch (Exception e)
        {
            return false;
        }

        socket.Close();
        return true;
    }

    public static bool TestSendName()
    {
        try {
            // Connect
            socket.Connect(address, int.Parse(ipPortArray[1]));

            byte[] msg = Encoding.UTF8.GetBytes("Chad\n");
            byte[] bytes = new byte[1024];

            socket.Send(msg);
        }
        catch (Exception e)
        {
            return false;
        }

        socket.Close();
        return true;
    }

    public static bool TestReceiveSpreadsheets()
    {
        try {
            // Connect
            socket.Connect(address, int.Parse(ipPortArray[1]));

            // Send name after connecting
            byte[] msg = Encoding.UTF8.GetBytes("Chad\n");
            byte[] bytes = new byte[1024];
            socket.Send(msg);


            socket.Receive(bytes);
            Console.WriteLine(Encoding.UTF8.GetString(bytes));
        }
        catch (Exception e)
        {
            return false;
        }

        socket.Close();
        return true;
    }

    public static bool TestSendSpreadsheetName()
    {
        try
        {
            // Connect
            socket.Connect(address, int.Parse(ipPortArray[1]));

            // Send name after connecting
            byte[] msg = Encoding.UTF8.GetBytes("Chad\n");
            byte[] bytes = new byte[1024];
            socket.Send(msg);

            // Receive spreadsheet names
            socket.Receive(bytes);
            Console.WriteLine(Encoding.UTF8.GetString(bytes));


            byte[] msg2 = Encoding.UTF8.GetBytes("test1\n");
            Console.WriteLine(Encoding.UTF8.GetString(msg2));
            socket.Send(msg2);
        }
        catch (Exception e)
        {
            return false;
        }

        socket.Close();
        return true;
    }

    public static bool TestSendNewSpreadsheet()
    {
        try {
            // Connect
            socket.Connect(address, int.Parse(ipPortArray[1]));

            // Send name after connecting
            byte[] msg = Encoding.UTF8.GetBytes("Chad\n");
            byte[] bytes = new byte[1024];
            socket.Send(msg);

            // Receive spreadsheet names
            socket.Receive(bytes);
            Console.WriteLine(Encoding.UTF8.GetString(bytes));


            byte[] msg2 = Encoding.UTF8.GetBytes("newspreadsheet\n");
            Console.WriteLine(Encoding.UTF8.GetString(msg2));
            socket.Send(msg2);
        }
        catch (Exception e)
        {
            return false;
        }

        socket.Close();
        return true;
    }

    public static bool TestReceiveSpreadsheetCells()
    {
        try
        {
            // Connect
            socket.Connect(address, int.Parse(ipPortArray[1]));

            // Send name after connecting
            byte[] msg = Encoding.UTF8.GetBytes("Chad\n");
            byte[] bytes = new byte[1024];
            socket.Send(msg);

            // Receive spreadsheet names
            socket.Receive(bytes);
            //Console.WriteLine(Encoding.UTF8.GetString(bytes));

            // Sends spreadsheet name
            byte[] msg2 = Encoding.UTF8.GetBytes("test1\n");
            //Console.WriteLine(Encoding.UTF8.GetString(msg2));
            socket.Send(msg2);           


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
        catch (Exception e)
        {
            return false;
        }

        socket.Close();
        return true;
    }
}*/