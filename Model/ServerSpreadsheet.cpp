#include <map>
#include <boost/json.hpp>
#include <vector>
#include <boost/property_tree/ptree.hpp>
#include "ServerSpreadsheet.h"
#include "Formula.h"
#include "DependencyGraph.h"

Cell::Cell()
{
}

Cell::Cell(std::string name, std::string content)
{
	prev_state = new Cell();
	this->set_name(name);
	this->set_contents(content);
}

void Cell::set_name(std::string name)
{
	this->cell_name = name;
}

void Cell::set_contents(std::string content)
{
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

void Spreadsheet::check_circular_dependency(Formula formula)
{
	std::unordered_set<std::string> visited;
	std::vector<std::string>* variables = formula.get_variables();
	for (int i = 0; i < variables->size(); ++i)
	{
		if (visited.count(variables->at(i)) == 0)
		{
			Visit(variables->at(i), variables->at(i), visited);
		}
	}
}

void Spreadsheet::Visit(std::string start, std::string name, std::unordered_set<std::string> visited)
{
	visited.insert(name);
	std::unordered_set<std::string> dependents = graph->get_dependents(name);

	for (const std::string& n : dependents)
	{
		if (n == start)
		{
			throw "Error: Circular Dependency!";
		}
		else if (visited.count(n) == 0)
		{
			Visit(start, n, visited);
		}
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

Spreadsheet::Spreadsheet(std::string s)
{
	name = s;
	cells = new std::map<std::string, Cell*>();
	history = new std::stack<Cell*>();
	graph = new DependencyGraph();
}

Spreadsheet::Spreadsheet(std::string s, std::map<std::string, Cell*>* cells, std::stack<Cell*>* history, DependencyGraph* graph)
{
	name = s;
	this->cells = cells;
	this->history = history;
	this->graph = graph;
}

std::stack<Cell*>* Spreadsheet::get_history()
{
	return history;
}

std::string Spreadsheet::get_json()
{
	boost::json::object obj;
	obj["name"] = name;
	obj["cells"] = get_json_cells();
	obj["history"] = get_json_history();
	obj["graph"] = get_json_graph();

	return boost::json::serialize(obj);
}

boost::json::array Spreadsheet::get_json_graph()
{
	boost::json::array arr(cells->size());

	int i = 0;
	for (std::pair<std::string, Cell*> cell : *cells)
	{
		boost::json::object obj;
		obj["name"] = cell.first;
		boost::json::array dependents(graph->get_dependents(cell.first).size());
		int j = 0;
		for (std::string dependent : graph->get_dependents(cell.first))
		{
			dependents[j] = dependent;
			j++;
		}
		obj["dependents"] = dependents;
		boost::json::array dependees(graph->get_dependees(cell.first).size());
		j = 0;
		for (std::string dependee : graph->get_dependees(cell.first))
		{
			dependees[j] = dependee;
			j++;
		}
		obj["dependees"] = dependees;
		arr[i] = obj;
		i++;
	}

	return arr;
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
	if (c.get_previous() != NULL)
		obj["previous"] = get_json_cell(c.get_previous());
	return obj;
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

std::string Spreadsheet::get_name()
{
	return name;
}

Cell::Cell(std::string name, std::string content, Cell* prev)
{
	cell_name = name;
	contents = content;
	if (prev != NULL)
		prev_state = new Cell(name, prev->get_contents(), prev->get_previous());
}

bool Spreadsheet::insert_cell(Cell* c)
{
	std::string cell_name = c->get_name();
	std::string contents = c->get_contents();
	if (contents[0] == '=')
	{
		std::unordered_set<std::string> oldDependees = graph->get_dependees(cell_name);
		std::string formula = contents.substr(1, contents.length());
		try
		{
			Formula formula(formula);
			graph->replace_dependees(cell_name, *formula.get_variables());
			check_circular_dependency(formula);
		}
		catch (const char* msg)
		{
			std::vector<std::string> vector;
			for (std::string dependee : oldDependees)
			{
				vector.push_back(dependee);
			}
			// NOTIFY SERVER OF INVALID FORMULA
			graph->replace_dependees(cell_name, vector);
			return false;
		}
	}
	else
		graph->replace_dependees(cell_name, std::vector<std::string>());

	cells->insert_or_assign(cell_name, c);

	return true;
}

bool Spreadsheet::edit_cell(Cell* c)
{
	Cell* prev;
	if (cells->find(c->get_name()) != cells->end())
	{
		prev = cells->at(c->get_name());
	}
	else
	{
		prev = new Cell(c->get_name(), "");
	}

	c->set_previous(prev);

	// insert current into cells
	if (!this->insert_cell(c))
	{
		return false;
	}

	// Push a copy to spreadsheet history
	Cell* cpy = new Cell(prev->get_name(), prev->get_contents(), prev->get_previous());
	history->push(cpy);

	return true;
}

Cell* Spreadsheet::undo(bool& success)
{
	if (history->empty())
	{
		return new Cell();
	}

	Cell* to_insert = new Cell(history->top());

	if (!this->insert_cell(to_insert))
	{
		history->pop();
		return undo(success);
	}

	// Remove from spreadsheet history
	history->pop();

	success = true;
	return cells->at(to_insert->get_name());
}

bool Spreadsheet::revert(Cell* c)
{
	// get the previous state and insert a copy
	Cell* to_insert = new Cell(c->get_previous());

	if (to_insert->get_previous() == NULL)
	{
		return false;
	}

	if (!this->insert_cell(to_insert))
	{
		revert(c->get_previous());
	}

	// push onto spreadsheet history stack
	history->push(c);

	return true;
}

void Cell::set_previous(Cell* c)
{
	prev_state = c;
}

Cell::Cell(Cell* c)
{
	this->cell_name = c->get_name();
	this->contents = c->get_contents();
	this->prev_state = c->get_previous();
}

Cell* Cell::get_previous()
{
	return prev_state;
}