#include <map>
#include <boost/json.hpp>
#include <vector>
#include <boost/property_tree/ptree.hpp>
#include "ServerSpreadsheet.h"
#include "Formula.h"

Cell::Cell()
{
}

Cell::Cell(std::string name, std::string content)
{
	history = new std::stack<std::string>();
	this->set_name(name);
	this->set_contents(content);
}

void Cell::set_name(std::string name)
{
	this->cell_name = name;
}

void Cell::set_contents(std::string content)
{
	history->push(contents);
	this->contents = content;
}

std::string Cell::get_contents()
{
	return this->contents;
}

std::string Cell::get_name()
{
	return this->cell_name;
}

std::stack<std::string>* Cell::get_history()
{
	return history;
}

std::string Cell::get_previous_change()
{
	std::string change = history->top();
	history->pop();
	return change;
}

std::map<std::string, Cell*>* Spreadsheet::get_cells()
{
	return cells;
}

Cell* Spreadsheet::get_cell(std::string cell_name)
{
	if (cells->find(cell_name) == cells->end())
	{
		Cell* cell = new Cell(cell_name, "");
		cells->insert(std::pair<std::string, Cell*>(cell_name, cell));
		return cell;
	}
	return cells->at(cell_name);
}

bool Spreadsheet::set_cell(std::string cell_name, std::string contents)
{
	if (contents[0] == '=')
	{
		std::string formula = contents.substr(1, contents.length());
		try
		{
			Formula::isValid(formula);
		}
		catch (const char* msg)
		{ 
			// NOTIFY SERVER OF INVALID FORMULA
			return false;
		}
		return true;
	}
	
	if (cells->find(cell_name) != cells->end())
	{
		Cell* cell = new Cell(cell_name, cells->at(cell_name)->get_contents());
		history->push(cell);
		cells->at(cell_name)->set_contents(contents);
	}
	else
	{
		Cell* cell = new Cell(cell_name, "");
		history->push(cell);
		cells->insert(std::pair<std::string, Cell*>(cell_name, new Cell(cell_name, contents)));
	}	

	return true;
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
	selected = "A1";
}

User::User(int ID, std::string name) 
{
	this->ID = ID;
	this->name = name;
	selected = "A1";
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

Cell* Spreadsheet::undo()
{
	Cell* cell = history->top();
	history->pop();

	if (cells->find(cell->get_name()) != cells->end())
	{
		cells->at(cell->get_name())->set_contents(cell->get_contents());
	}

	return cell;
}

Spreadsheet::Spreadsheet(std::string s)
{
	name = s;
	cells = new std::map<std::string, Cell*>();
	history = new std::stack<Cell*>();
}

Spreadsheet::Spreadsheet()
{
}

std::stack<Cell*>* Spreadsheet::get_history()
{
	return history;
}

std::string Spreadsheet::get_json()
{
	boost::property_tree::ptree pt;


	//boost::json::object obj;
	//obj["name"] = name;
	//obj["cells"] = get_json_cells();
	//obj["history"] = get_json_history();

	//return boost::json::serialize(obj);
}

boost::json::array Spreadsheet::get_json_cells()
{
	boost::json::array arr(cells->size());

	int i = 0;
	for (std::map<std::string, Cell*>::iterator it = cells->begin(); it != cells->end(); ++it)
	{
		boost::json::object cell = get_json_cell(*it->second);
		arr[i] = cell;
		i++;
	}

	return arr;
}

boost::json::object Spreadsheet::get_json_cell(Cell c)
{
	boost::json::object obj;
	obj["name"] = c.get_name();
	obj["contents"] = c.get_contents();
	obj["history"] = get_json_cell_history(c);
	return obj;
}

boost::json::array Spreadsheet::get_json_cell_history(Cell c)
{
	std::stack<std::string> copy(*c.get_history());
	boost::json::array arr(copy.size());

	int loop_for = copy.size();
	for (int i = 0; i < loop_for; i++)
	{
		arr[i] = copy.top();
		copy.pop();
	}

	return arr;
}

boost::json::array Spreadsheet::get_json_history()
{
	std::stack<Cell*> copy(*history);
	boost::json::array arr(copy.size());

	int loop_for = copy.size();
	for (int i = 0; i < loop_for; i++)
	{
		boost::json::object obj = get_json_cell(*copy.top());
		arr[i] = obj;
		copy.pop();
	}

	return arr;
}