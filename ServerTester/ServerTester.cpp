#include <iostream>
#include <boost/asio.hpp>

using boost::asio::ip::tcp;
using namespace std::chrono_literals;

boost::asio::io_context io_context;
tcp::socket sock = tcp::socket(io_context);
std::string buffer = "";
std::string r_buffer = "";
std::string s_buffer = "";
std::condition_variable cv;
enum { max_length = 1024 };

// Forward declarations
std::string split_and_delete(std::string& s);
bool TestConnection(std::string ip, int port);
bool TestSendName(std::string ip, int port);
bool TestReceiveSpreadsheets(std::string ip, int port);
bool TestSendSpreadsheetName(std::string ip, int port);
bool TestSendNewSpreadsheetName(std::string ip, int port);
bool TestSelectCell(std::string ip, int port);
bool TestChangeString(std::string ip, int port);
bool TestChangeFormula(std::string ip, int port);
bool TestUndo(std::string ip, int port);
bool TestRevertCell(std::string ip, int port);


std::string split_and_delete(std::string& s)
{
	int loc = s.find("\n");
	std::string before = s.substr(0, loc);
	s = s.substr(loc + 1, std::string::npos);
	return before;
}

int main(int argc, char** argv)
{
	if (argc == 0)
	{
		// AS MORE TESTS ARE ADDED, CHANGE THIS NUMBER TO PRINT CORRECT NUMBER OF TESTS
		std::cout << "10" << std::endl;
		return 0;
	}

	//std::string ipPort = argv[2];
	//int testNum = std::stoi(argv[1]);
	//int i = ipPort.find(":");

	//std::string ip = ipPort.substr(0, i);
	//int port = std::stoi(ipPort.substr(i + 1, ipPort.size()));

	// For local testing
	std::string ip = "127.0.0.1";
	int port = 1100;
	int testNum = 10;

	switch (testNum)
	{
	case 1:
	{
		try
		{
			std::cout << "5" << std::endl;
			std::cout << "TestConnection" << std::endl;

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

			if (retValue == 1)
				std::cout << "Pass" << std::endl << std::endl;
			else
				std::cout << "Fail" << std::endl << std::endl;

			return retValue;
		}
		catch (std::runtime_error e)
		{
			std::cout << "Fail" << std::endl << std::endl;
			return false;
		}

		// auto task = std::async(TestConnection);

		break;
	}

	case 2:
	{
		try
		{
			std::cout << "5" << std::endl;
			std::cout << "TestSendName" << std::endl;

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

			if (retValue == 1)
				std::cout << "Pass" << std::endl << std::endl;
			else
				std::cout << "Fail" << std::endl << std::endl;

			return retValue;
		}
		catch (std::runtime_error e)
		{
			std::cout << "Fail" << std::endl << std::endl;
			return false;
		}

		// auto task = std::async(TestConnection);

		break;
	}

	case 3:
	{
		try
		{
			std::cout << "5" << std::endl;
			std::cout << "TestReceiveSpreadsheets" << std::endl;

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

			if (retValue == 1)
				std::cout << "Pass" << std::endl << std::endl;
			else
				std::cout << "Fail" << std::endl << std::endl;

			return retValue;
		}
		catch (std::runtime_error e)
		{
			std::cout << "Fail" << std::endl << std::endl;
			return false;
		}

		// auto task = std::async(TestConnection);

		break;
	}

	case 4:
	{
		try
		{
			std::cout << "5" << std::endl;
			std::cout << "TestSendSpreadsheetName" << std::endl;

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

			if (retValue == 1)
				std::cout << "Pass" << std::endl << std::endl;
			else
				std::cout << "Fail" << std::endl << std::endl;

			return retValue;
		}
		catch (std::runtime_error e)
		{
			std::cout << "Fail" << std::endl << std::endl;
			return false;
		}

		// auto task = std::async(TestConnection);

		break;
	}

	case 5:
	{
		try
		{
			std::cout << "5" << std::endl;
			std::cout << "TestSendSpreadsheetName" << std::endl;

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

			if (retValue == 1)
				std::cout << "Pass" << std::endl << std::endl;
			else
				std::cout << "Fail" << std::endl << std::endl;

			return retValue;
		}
		catch (std::runtime_error e)
		{
			std::cout << "Fail" << std::endl << std::endl;
			return false;
		}

		// auto task = std::async(TestConnection);

		break;
	}

	case 6:
	{
		try
		{
			std::cout << "5" << std::endl;
			std::cout << "TestSelectCell" << std::endl;

			std::mutex m;
			std::condition_variable cv;
			int retValue;

			std::thread t([&cv, &retValue, &ip, &port]()
				{
					retValue = TestSelectCell(ip, port);
					cv.notify_one();
				});

			t.detach();

			{
				std::unique_lock<std::mutex> l(m);
				if (cv.wait_for(l, 5s) == std::cv_status::timeout)
					throw std::runtime_error("False");
			}

			if (retValue == 1)
				std::cout << "Pass" << std::endl << std::endl;
			else
				std::cout << "Fail" << std::endl << std::endl;

			return retValue;
		}
		catch (std::runtime_error e)
		{
			std::cout << "Fail" << std::endl << std::endl;
			return false;
		}

		// auto task = std::async(TestConnection);

		break;
	}
	case 7:
	{
		try
		{
			std::cout << "5" << std::endl;
			std::cout << "TestChangeString" << std::endl;

			std::mutex m;
			std::condition_variable cv;
			int retValue;

			std::thread t([&cv, &retValue, &ip, &port]()
				{
					retValue = TestChangeString(ip, port);
					cv.notify_one();
				});

			t.detach();

			{
				std::unique_lock<std::mutex> l(m);
				if (cv.wait_for(l, 5s) == std::cv_status::timeout)
					throw std::runtime_error("False");
			}

			if (retValue == 1)
				std::cout << "Pass" << std::endl << std::endl;
			else
				std::cout << "Fail" << std::endl << std::endl;

			return retValue;
		}
		catch (std::runtime_error e)
		{
			std::cout << "Fail" << std::endl << std::endl;
			return false;
		}

		// auto task = std::async(TestConnection);

		break;
	}
	case 8:
	{
		try
		{
			std::cout << "5" << std::endl;
			std::cout << "TestChangeFormula" << std::endl;

			std::mutex m;
			std::condition_variable cv;
			int retValue;

			std::thread t([&cv, &retValue, &ip, &port]()
				{
					retValue = TestChangeFormula(ip, port);
					cv.notify_one();
				});

			t.detach();

			{
				std::unique_lock<std::mutex> l(m);
				if (cv.wait_for(l, 5s) == std::cv_status::timeout)
					throw std::runtime_error("False");
			}

			if (retValue == 1)
				std::cout << "Pass" << std::endl << std::endl;
			else
				std::cout << "Fail" << std::endl << std::endl;

			return retValue;
		}
		catch (std::runtime_error e)
		{
			std::cout << "Fail" << std::endl << std::endl;
			return false;
		}

		// auto task = std::async(TestConnection);

		break;
	}
	case 9:
	{
		try
		{
			std::cout << "5" << std::endl;
			std::cout << "TestUndo" << std::endl;

			std::mutex m;
			std::condition_variable cv;
			int retValue;

			std::thread t([&cv, &retValue, &ip, &port]()
				{
					retValue = TestUndo(ip, port);
					cv.notify_one();
				});

			t.detach();

			{
				std::unique_lock<std::mutex> l(m);
				if (cv.wait_for(l, 5s) == std::cv_status::timeout)
					throw std::runtime_error("False");
			}

			if (retValue == 1)
				std::cout << "Pass" << std::endl << std::endl;
			else
				std::cout << "Fail" << std::endl << std::endl;

			return retValue;
		}
		catch (std::runtime_error e)
		{
			std::cout << "Fail" << std::endl << std::endl;
			return false;
		}

		// auto task = std::async(TestConnection);

		break;
	}
	case 10:
	{
		try
		{
			std::cout << "5" << std::endl;
			std::cout << "TestRevertCell" << std::endl;

			std::mutex m;
			std::condition_variable cv;
			int retValue;

			std::thread t([&cv, &retValue, &ip, &port]()
				{
					retValue = TestRevertCell(ip, port);
					cv.notify_one();
				});

			t.detach();

			{
				std::unique_lock<std::mutex> l(m);
				if (cv.wait_for(l, 5s) == std::cv_status::timeout)
					throw std::runtime_error("False");
			}

			if (retValue == 1)
				std::cout << "Pass" << std::endl << std::endl;
			else
				std::cout << "Fail" << std::endl << std::endl;

			return retValue;
		}
		catch (std::runtime_error e)
		{
			std::cout << "Fail" << std::endl << std::endl;
			return false;
		}

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

	s_buffer.clear();
	r_buffer.clear();
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
		std::string message = "Chad\n";
		sock.send(boost::asio::buffer(message, max_length));
	}
	catch (std::exception e)
	{
		return false;
	}

	//TURN INTO CLEANUP METHOD
	s_buffer.clear();
	r_buffer.clear();
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
		std::string message = "Chad\n";
		sock.send(boost::asio::buffer(message, max_length));

		// Receive the names of the spreadsheets
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), "\n\n");
	}
	catch (std::exception e)
	{
		return false;
	}

	s_buffer.clear();
	r_buffer.clear();
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
		std::string message = "Chad\n";
		sock.send(boost::asio::buffer(message, max_length));

		// Receive the names of the spreadsheets
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), "\n\n");

		// Send a name of existing spreadsheet
		message = "test1\n";
		sock.send(boost::asio::buffer(message, max_length));

		r_buffer.clear();

		// Make sure we recieve ID back
		bool foundInt = false;
		while (!foundInt)
		{
			// TURN INTO METHOD
			boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

			// add s_buffer (the storage buffer) to the beginning of the recieved buffer
			r_buffer = s_buffer.append(r_buffer);

			// split by newline every time there is one, then store the leftover into s_buffer
			while (r_buffer.find('\n') != std::string::npos)
			{
				std::string command = split_and_delete(r_buffer);

				try
				{
					std::stoi(command);
					foundInt = true;
					break;
				}
				catch (std::invalid_argument e)
				{
				}
			}

			s_buffer = r_buffer;
		}
	}
	catch (std::exception e)
	{
		return false;
	}

	s_buffer.clear();
	r_buffer.clear();
	sock.close();
	return true;
}

bool TestSendNewSpreadsheetName(std::string ip, int port)
{
	try
	{
		// Connect to server
		sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));

		// Send name
		std::string message = "Chad\n";
		sock.send(boost::asio::buffer(message, max_length));

		// Receive the names of the spreadsheets
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), "\n\n");

		// Send a name of new spreadsheet
		message = "newspreadsheet\n";
		sock.send(boost::asio::buffer(message, max_length));

		r_buffer.clear();

		// Make sure we recieve ID back
		bool foundInt = false;
		while (!foundInt)
		{
			// TURN INTO METHOD
			boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');
			// add s_buffer (the storage buffer) to the beginning of the recieved buffer
			r_buffer = s_buffer.append(r_buffer);

			// split by newline every time there is one, then store the leftover into s_buffer
			while (r_buffer.find('\n') >= 0)
			{
				std::string command = split_and_delete(r_buffer);
				try
				{
					std::stoi(command);
					foundInt = true;
					break;
				}
				catch (std::invalid_argument e)
				{
				}
			}

			s_buffer = r_buffer;
		}
	}
	catch (std::exception e)
	{
		return false;
	}

	s_buffer.clear();
	r_buffer.clear();
	sock.close();
	return true;
}

// Finish seperating cells the server sends into individual cells (whether that is with regex or some other magic)
//NOT WORKING (MIGHT BE FIXED)
//bool TestReceiveSpreadsheetCells(std::string ip, int port)
//{
//	try
//	{
//		// Connect to server
//		sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));
//
//		// Send name
//		sock.send(boost::asio::buffer("Chad\n", max_length));
//
//		// Receive the names of the spreadsheets
//		sock.receive(boost::asio::buffer(r_buffer, max_length));
//
//		// Send a name of new spreadsheet
//		sock.send(boost::asio::buffer("test1\n", max_length));
//
//		// Make sure we recieve ID back
//		bool foundInt = false;
//		while (!foundInt)
//		{
//			// TURN INTO METHOD
//			boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');
//			// add s_buffer (the storage buffer) to the beginning of the recieved buffer
//			r_buffer = s_buffer.append(r_buffer);
//
//			// split by newline every time there is one, then store the leftover into s_buffer
//			while (r_buffer.find('\n') >= 0)
//			{
//				std::string command = split_and_delete(r_buffer);
//				try
//				{
//					std::stoi(command);
//					foundInt = true;
//					break;
//				}
//				catch (std::invalid_argument e)
//				{
//				}
//			}
//
//			s_buffer = r_buffer;
//		}
//	}
//	catch (std::exception e)
//	{
//		return false;
//	}
//
//	s_buffer.clear();
//	r_buffer.clear();
//	sock.close();
//	return true;
//}

bool TestSelectCell(std::string ip, int port)
{
	try
	{
		// Connect to server
		sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));

		// Send name
		std::string message = "Chad\n";
		sock.send(boost::asio::buffer(message, max_length));

		// Receive the names of the spreadsheets
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), "\n\n");

		// Send a name of new spreadsheet
		message = "newspreadsheet\n";
		sock.send(boost::asio::buffer(message, max_length));

		r_buffer.clear();

		// Make sure we recieve ID back
		bool foundInt = false;
		while (!foundInt)
		{
			// TURN INTO METHOD
			boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');
			// add s_buffer (the storage buffer) to the beginning of the recieved buffer
			r_buffer = s_buffer.append(r_buffer);

			// split by newline every time there is one, then store the leftover into s_buffer
			while (r_buffer.find('\n') >= 0)
			{
				std::string command = split_and_delete(r_buffer);
				try
				{
					std::stoi(command);
					foundInt = true;
					break;
				}
				catch (std::invalid_argument e)
				{
				}
			}

			s_buffer = r_buffer;
		}

		r_buffer = "";
		std::string cell = "A3";

		// Select a new cell
		sock.send(boost::asio::buffer("{ messageType: \"selectCell\", cellName: \"" + cell + "\" }\n"));

		// Receive server's "cell was selected" message
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

		if (r_buffer != "{ messageType: \"cellSelected\", cellName: \"" + cell + "\", selector: \"" + std::to_string(0) + "\", selectorName: \"" + "Chad" + "\" }\n")
		{
			std::string msg = "Fail";
			throw msg;
		}
	}
	catch (std::string& msg)
	{
		return false;
	}
	
	s_buffer.clear();
	r_buffer.clear();
	sock.close();
	return true;
}

bool TestChangeString(std::string ip, int port)
{
	try
	{
		// Connect to server
		sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));

		// Send name
		std::string message = "Chad\n";
		sock.send(boost::asio::buffer(message, max_length));

		// Receive the names of the spreadsheets
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), "\n\n");

		// Send a name of new spreadsheet
		message = "newspreadsheet\n";
		sock.send(boost::asio::buffer(message, max_length));

		r_buffer.clear();

		// Make sure we recieve ID back
		bool foundInt = false;
		while (!foundInt)
		{
			// TURN INTO METHOD
			boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');
			// add s_buffer (the storage buffer) to the beginning of the recieved buffer
			r_buffer = s_buffer.append(r_buffer);

			// split by newline every time there is one, then store the leftover into s_buffer
			while (r_buffer.find('\n') >= 0)
			{
				std::string command = split_and_delete(r_buffer);
				try
				{
					std::stoi(command);
					foundInt = true;
					break;
				}
				catch (std::invalid_argument e)
				{
				}
			}

			s_buffer = r_buffer;
		}

		r_buffer = "";
		std::string cell = "A3";

		// Select a new cell
		sock.send(boost::asio::buffer("{ messageType: \"selectCell\", cellName: \"" + cell + "\" }\n"));

		// Receive server's "cell was selected" message
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

		if (r_buffer != "{ messageType: \"cellSelected\", cellName: \"" + cell + "\", selector: \"" + std::to_string(0) + "\", selectorName: \"" + "Chad" + "\" }\n")
		{
			std::string msg = "Fail";
			throw msg;
		}

		r_buffer = "";

		// Send a change request to the server
		std::string new_contents = "testing!";
		sock.send(boost::asio::buffer("{ messageType: \"editCell\", cellName: \"" + cell + "\", contents: \"" + new_contents + "\"" + "}\n"));

		// Receive the change message from server
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

		std::string check_message = "{ messageType: \"cellUpdated\", cellName: \"" + cell + "\", contents: \"" + new_contents + "\"" + "}\n";

		// If the message we receive does not match the required json string, test has failed
		if (r_buffer != check_message)
		{
			std::string msg = "Fail";
			throw msg;
		}

		r_buffer = "";
	}
	catch (std::string& msg)
	{
		return false;
	}

	s_buffer.clear();
	r_buffer.clear();
	sock.close();
	return true;
}

bool TestChangeFormula(std::string ip, int port)
{
	try
	{
		// Connect to server
		sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));

		// Send name
		std::string message = "Chad\n";
		sock.send(boost::asio::buffer(message, max_length));

		// Receive the names of the spreadsheets
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), "\n\n");

		// Send a name of new spreadsheet
		message = "newspreadsheet\n";
		sock.send(boost::asio::buffer(message, max_length));

		r_buffer.clear();

		// Make sure we recieve ID back
		bool foundInt = false;
		while (!foundInt)
		{
			// TURN INTO METHOD
			boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');
			// add s_buffer (the storage buffer) to the beginning of the recieved buffer
			r_buffer = s_buffer.append(r_buffer);

			// split by newline every time there is one, then store the leftover into s_buffer
			while (r_buffer.find('\n') >= 0)
			{
				std::string command = split_and_delete(r_buffer);
				try
				{
					std::stoi(command);
					foundInt = true;
					break;
				}
				catch (std::invalid_argument e)
				{
				}
			}

			s_buffer = r_buffer;
		}

		r_buffer = "";
		std::string cell = "A3";

		// Select a new cell
		sock.send(boost::asio::buffer("{ messageType: \"selectCell\", cellName: \"" + cell + "\" }\n"));

		// Receive server's "cell was selected" message
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

		if (r_buffer != "{ messageType: \"cellSelected\", cellName: \"" + cell + "\", selector: \"" + std::to_string(0) + "\", selectorName: \"" + "Chad" + "\" }\n")
		{
			std::string msg = "Fail";
			throw msg;
		}

		r_buffer = "";

		// Send a change request to the server
		std::string new_contents = "= 3*2+ (1/1) + 20";
		sock.send(boost::asio::buffer("{ messageType: \"editCell\", cellName: \"" + cell + "\", contents: \"" + new_contents + "\"" + "}\n"));

		// Receive the change message from server
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

		std::string check_message = "{ messageType: \"cellUpdated\", cellName: \"" + cell + "\", contents: \"" + "= 3*2+ (1/1) + 20" + "\"" + "}\n";

		// If the message we receive does not match the required json string, test has failed
		if (r_buffer != check_message)
		{
			std::string msg = "Fail";
			throw msg;
		}

		r_buffer = "";
	}
	catch (std::string& msg)
	{
		return false;
	}

	s_buffer.clear();
	r_buffer.clear();
	sock.close();
	return true;
}

bool TestUndo(std::string ip, int port)
{
	try
	{
		// Connect to server
		sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));

		// Send name
		std::string message = "Chad\n";
		sock.send(boost::asio::buffer(message, max_length));

		// Receive the names of the spreadsheets
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), "\n\n");

		// Send a name of new spreadsheet
		message = "newspreadsheet\n";
		sock.send(boost::asio::buffer(message, max_length));

		r_buffer.clear();

		// Make sure we recieve ID back
		bool foundInt = false;
		while (!foundInt)
		{
			// TURN INTO METHOD
			boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');
			// add s_buffer (the storage buffer) to the beginning of the recieved buffer
			r_buffer = s_buffer.append(r_buffer);

			// split by newline every time there is one, then store the leftover into s_buffer
			while (r_buffer.find('\n') >= 0)
			{
				std::string command = split_and_delete(r_buffer);
				try
				{
					std::stoi(command);
					foundInt = true;
					break;
				}
				catch (std::invalid_argument e)
				{
				}
			}

			s_buffer = r_buffer;
		}

		r_buffer = "";
		std::string cell = "A3";

		// Select a new cell
		sock.send(boost::asio::buffer("{ messageType: \"selectCell\", cellName: \"" + cell + "\" }\n"));

		// Receive server's "cell was selected" message
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

		if (r_buffer != "{ messageType: \"cellSelected\", cellName: \"" + cell + "\", selector: \"" + std::to_string(0) + "\", selectorName: \"" + "Chad" + "\" }\n")
		{
			std::string msg = "Fail";
			throw msg;
		}

		r_buffer = "";

		// Send a change request to the server
		std::string new_contents = "= 3*2+ (1/1) + 20";
		sock.send(boost::asio::buffer("{ messageType: \"editCell\", cellName: \"" + cell + "\", contents: \"" + new_contents + "\"" + "}\n"));

		// Receive the change message from server
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

		std::string check_message = "{ messageType: \"cellUpdated\", cellName: \"" + cell + "\", contents: \"" + "= 3*2+ (1/1) + 20" + "\"" + "}\n";

		// If the message we receive does not match the required json string, test has failed
		if (r_buffer != check_message)
		{
			std::string msg = "Fail";
			throw msg;
		}

		r_buffer = "";

		// Send an undo request to server
		sock.send(boost::asio::buffer("{ requestType: \"undo\" }\n"));

		// Receive the undo contents from server
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

		new_contents = "";
		check_message = "{ messageType: \"cellUpdated\", cellName: \"" + cell + "\", contents: \"" + new_contents + "\"" + "}\n";

		// If the message we receive does not match the required json string, test has failed
		if (r_buffer != check_message)
		{
			std::string msg = "Fail";
			throw msg;
		}

		r_buffer = "";
	}
	catch (std::string& msg)
	{
		return false;
	}

	s_buffer.clear();
	r_buffer.clear();
	sock.close();
	return true;
}

bool TestRevertCell(std::string ip, int port)
{
	try
	{
		// Connect to server
		sock.connect(tcp::endpoint(boost::asio::ip::address::from_string(ip), port));

		// Send name
		std::string message = "Chad\n";
		sock.send(boost::asio::buffer(message, max_length));

		// Receive the names of the spreadsheets
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), "\n\n");

		// Send a name of new spreadsheet
		message = "newspreadsheet\n";
		sock.send(boost::asio::buffer(message, max_length));

		r_buffer.clear();

		// Make sure we recieve ID back
		bool foundInt = false;
		while (!foundInt)
		{
			// TURN INTO METHOD
			boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');
			// add s_buffer (the storage buffer) to the beginning of the recieved buffer
			r_buffer = s_buffer.append(r_buffer);

			// split by newline every time there is one, then store the leftover into s_buffer
			while (r_buffer.find('\n') >= 0)
			{
				std::string command = split_and_delete(r_buffer);
				try
				{
					std::stoi(command);
					foundInt = true;
					break;
				}
				catch (std::invalid_argument e)
				{
				}
			}

			s_buffer = r_buffer;
		}

		r_buffer = "";
		std::string cell = "A3";

		// Select a new cell
		sock.send(boost::asio::buffer("{ messageType: \"selectCell\", cellName: \"" + cell + "\" }\n"));

		// Receive server's "cell was selected" message
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

		if (r_buffer != "{ messageType: \"cellSelected\", cellName: \"" + cell + "\", selector: \"" + std::to_string(0) + "\", selectorName: \"" + "Chad" + "\" }\n")
		{
			std::string msg = "Fail";
			throw msg;
		}

		r_buffer = "";

		// Send a change request to the server
		std::string new_contents = "= 3*2+ (1/1) + 20";
		sock.send(boost::asio::buffer("{ messageType: \"editCell\", cellName: \"" + cell + "\", contents: \"" + new_contents + "\"" + "}\n"));

		// Receive the change message from server
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

		std::string check_message = "{ messageType: \"cellUpdated\", cellName: \"" + cell + "\", contents: \"" + "= 3*2+ (1/1) + 20" + "\"" + "}\n";

		// If the message we receive does not match the required json string, test has failed
		if (r_buffer != check_message)
		{
			std::string msg = "Fail";
			throw msg;
		}

		r_buffer = "";

		// Send an undo request to server
		sock.send(boost::asio::buffer("{ requestType: \"revertCell\", cellName: \"" + cell + "\" }\n"));

		// Receive the undo contents from server
		boost::asio::read_until(sock, boost::asio::dynamic_buffer(r_buffer, max_length), '\n');

		new_contents = "";
		check_message = "{ messageType: \"cellUpdated\", cellName: \"" + cell + "\", contents: \"" + new_contents + "\"" + "}\n";

		// If the message we receive does not match the required json string, test has failed
		if (r_buffer != check_message)
		{
			std::string msg = "Fail";
			throw msg;
		}

		r_buffer = "";
	}
	catch (std::string msg)
	{
		return false;
	}

	s_buffer.clear();
	r_buffer.clear();
	sock.close();
	return true;
}