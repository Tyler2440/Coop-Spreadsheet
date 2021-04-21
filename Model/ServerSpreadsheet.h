#ifndef SERVERSPREADSHEET_H
#define SERVERSPREADSHEET_H

#include <string>
#include <map>
#include <stack>

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


/*
* Represents collaborative spreadsheet
*/
class Spreadsheet {

	std::map<std::string, Cell> cells;
	std::map<int, std::string> users;

	std::stack<Cell> history;

public:
	std::map<std::string, Cell> get_cells();
	Cell get_cell(std::string cell_name);
	void set_cell(std::string cell_name, std::string contents);

	const std::map<int, std::string> get_users();
	void add_user(std::string name, int ID);
	void delete_user(int ID);
};

#endif
