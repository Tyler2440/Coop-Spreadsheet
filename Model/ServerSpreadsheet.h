#ifndef SERVERSPREADSHEET_H
#define SERVERSPREADSHEET_H

#include <string>
#include <map>
#include <stack>
#include <boost/json.hpp>

/*
*
* Holds the value of a cell
*
*/
class Cell {

	//std::string cell_name;
	std::string contents;
	std::stack<std::string> previous_changes;

public:
	Cell();
	Cell(std::string content); //constructor 

	//std::string get_name();

	std::string get_contents();

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

	std::map<std::string, Cell> cells;
	std::map<int, User> users;
	std::string name;
	std::stack<Cell> history;

	boost::json::object get_json_cells();
	boost::json::object get_json_history();

public:
	Spreadsheet(std::string s);
	Spreadsheet();
	std::map<std::string, Cell> get_cells();
	Cell get_cell(std::string cell_name);
	bool set_cell(std::string cell_name, std::string contents);
	void select_cell(int ID_of_selector, std::string cell_name);

	const std::map<int, User> get_users();
	void add_user(std::string name, int ID);
	void delete_user(int ID);

	std::string get_json();
};

#endif
