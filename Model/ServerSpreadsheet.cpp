#include <map>
#include "ServerSpreadsheet.h"
<<<<<<< HEAD
#include "Formula.h"
=======
#include <boost/json.hpp>
>>>>>>> 3f420d5a5e33d5975341635c07537a69bf8c2cb8

Cell::Cell()
{
}

Cell::Cell(std::string content)
{
	this->set_contents(content);
}

void Cell::set_contents(std::string content)
{
	this->contents = content;
}

std::string Cell::get_contents()
{
	return this->contents;
}

/*
std::string Cell::get_name()
{
	return this->cell_name;
}
*/

std::string Cell::get_previous_change()
{
	if (!previous_changes.empty())
	{
		std::string change = previous_changes.top();
		previous_changes.pop();
		return change;
	}

	return "";
}

std::map<std::string, Cell> Spreadsheet::get_cells()
{
	return cells;
}

Cell Spreadsheet::get_cell(std::string cell_name)
{
	return cells[cell_name];
}

void Spreadsheet::set_cell(std::string cell_name, std::string contents)
{
	if (contents[0] == '=')
	{
		std::string formula = contents.substr(1, contents.length());
		try
		{
			Formula::isValid(formula);
		}
		catch (std::exception e)
		{
			return;
		}
	}
	
	
	if (cells.find(cell_name) != cells.end())
	{
		cells[cell_name].set_contents(contents);
	}
	else
	{
		cells.insert(std::pair<std::string, Cell>(cell_name, *(new Cell(contents))));
	}
}

const std::map<int, User> Spreadsheet::get_users()
{
	return users;
}

void Spreadsheet::add_user(std::string name, int ID)
{
	users.insert(std::pair<int, User>(ID, *(new User(ID, name))));
}

void Spreadsheet::delete_user(int ID)
{
	users.erase(ID);
}

int User::get_ID()
{
	return ID;
}

std::string User::get_selected()
{
	return selected;
}

User::User()
{
}

User::User(int ID, std::string name) 
{
	this->ID = ID;
	this->name = name;
}

std::string User::get_name()
{
	return name;
}

void Spreadsheet::select_cell(int ID_of_selector, std::string cell_name)
{
	users[ID_of_selector].select(cell_name);
	users.insert(std::pair<int, User>(ID_of_selector, users[ID_of_selector]));
}

void User::select(std::string cell_name)
{
	selected = cell_name;
}

Spreadsheet::Spreadsheet(std::string s)
{
	name = s;
}

Spreadsheet::Spreadsheet()
{
}

std::string Spreadsheet::get_json()
{
	boost::json::object obj;
	obj["name"] = name;
	//obj["cells"] = cells;
	//obj["history"] = history;
}

boost::json::object Spreadsheet::get_json_history()
{
}