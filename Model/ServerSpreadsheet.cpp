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
	history = new std::stack<Cell*>();
	this->set_name(name);
	this->set_contents(content);
}

void Cell::set_name(std::string name)
{
	this->cell_name = name;
}

void Cell::set_contents(std::string content)
{
	std::stack<Cell*>* h_cpy = new std::stack<Cell*>(*history);
	//history->push(new Cell(cell_name, contents, h_cpy));
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

std::stack<Cell*>* Cell::get_history()
{
	return history;
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

	if (cells->find(cell_name) != cells->end())
	{
		Cell* cell = new Cell(cell_name, cells->at(cell_name)->get_contents(), cells->at(cell_name)->get_history());
		history->push(cell);
		cells->at(cell_name)->set_contents(contents);
		cells->at(cell_name)->add_history(cell);
	}
	else
	{
		Cell* cell = new Cell(cell_name, "");
		history->push(cell);
		cells->insert(std::pair<std::string, Cell*>(cell_name, new Cell(cell_name, contents)));
		cells->at(cell_name)->add_history(cell);
	}

	return true;
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

Cell* Spreadsheet::undo()
{
	while (true)
	{
		// IF WE DETECT HISTORY IS EMPTY, RETURN OUT
		if (history->empty())
		{
			return new Cell();
		}

		Cell* cell = history->top();
		history->pop();
		cells->insert_or_assign(cell->get_name(), cell);
		cell->pop_history();

		if (cells->at(cell->get_name())->get_history()->empty() && !history->empty())
		{
			Cell* new_cell = new Cell(cell->get_name(), "");
			cells->at(new_cell->get_name())->add_history(new_cell);
		}

		try
		{
			Formula formula(cell->get_contents());
			check_circular_dependency(formula);
			return cell;
		}
		catch (const char* msg)
		{
			continue;
		}
	}
	
}

void Cell::pop_history()
{
	if (!history->empty())
		history->pop();
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
	obj["history"] = get_json_cell_history(c);
	return obj;
}

boost::json::array Spreadsheet::get_json_cell_history(Cell c)
{
	std::stack<Cell*> copy(*c.get_history());
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

Cell::Cell(std::string name, std::string content, std::stack<Cell*>* history)
{
	cell_name = name;
	contents = content;
	this->history = history;
}

Cell* Spreadsheet::revert(std::string s, bool& success)
{
	while (true)
	{
		Cell* cell = get_cell(s);
		if (!cell->get_history()->empty())
		{
			success = true;
			std::stack<Cell*>* h_cpy = new std::stack<Cell*>(*cell->get_history());
			Cell* prev = new Cell(cell->get_name(), cell->get_contents(), h_cpy);
			cells->insert_or_assign(s, cell->get_history()->top());
			cell->get_history()->pop();

			history->push(prev);
			if (cell->get_history()->empty())
			{
				Cell* new_cell = new Cell(cell->get_name(), "");
				cell->add_history(new_cell);
			}
		}
		else
			return cell;

		try
		{
			Formula formula(cell->get_contents());
			check_circular_dependency(formula);
			return cells->at(s);
		}
		catch (const char* msg)
		{
			continue;
		}		
	}
}

void Cell::add_history(Cell* cell)
{
	history->push(cell);
}
