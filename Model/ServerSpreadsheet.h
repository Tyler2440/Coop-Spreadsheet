#ifndef SERVERSPREADSHEET_H
#define SERVERSPREADSHEET_H

#include <string>
#include <map>
#include <stack>
#include <boost/json.hpp>
#include <vector>
#include <boost/property_tree/ptree.hpp>

/*
*
* Holds the value of a cells 
*
*/
class Cell {

	std::string cell_name;
	std::string contents;
	std::stack<std::string>* history;

public:
	Cell();
	Cell(std::string name, std::string content); //constructor 

	std::string get_name();
	std::string get_contents();
	std::stack<std::string>* get_history();

	void set_name(std::string name);
	void set_contents(std::string content);

	std::string get_previous_change();
};

class User 
{	
	private:
		int ID;
		std::string selected;
		std::string name;

	public:
		int get_ID();
		std::string get_selected();
		std::string get_name();
		void select(std::string cell_name);

		User();
		User (int ID, std::string name);
};

/*
* Represents collaborative spreadsheet
*/
class Spreadsheet {

	std::map<std::string, Cell*>* cells;
	std::map<int, User> users;
	std::string name;
	std::stack<Cell*>* history;

	boost::json::array get_json_cells();
	boost::json::array get_json_history();
	boost::json::object get_json_cell(Cell c);
	boost::json::array get_json_cell_history(Cell c);

public:
	Spreadsheet(std::string s);
	Spreadsheet();
	std::map<std::string, Cell*>* get_cells();
	Cell* get_cell(std::string cell_name);
	bool set_cell(std::string cell_name, std::string contents);
	void select_cell(int ID_of_selector, std::string cell_name);
	Cell* undo();
	std::stack<Cell*>* get_history();

	const std::map<int, User> get_users();
	void add_user(std::string name, int ID);
	void delete_user(int ID);

	std::string get_json();
};

#endif
