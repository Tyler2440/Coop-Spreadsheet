#ifndef BATED_SERVER_SPREADSHEET
#define BATED_SERVER_SPREADSHEET


#include <string>
#include <map>
#include <stack>

/*
*
* Holds the value of a cell
*
*/
class Cell {

	std::string cell_name;
	std::string contents;
	std::stack<std::string> previous_changes;

public:

	Cell(std::string content); //constructor 

	std::string get_name();

	std::string get_contents();

	void set_contents(std::string content);

	std::string get_previous_change();
};


/*
* Represents collaborative spreadsheet
*/
class Spreadsheet {

	std::map<std::string, Cell> cells;

	std::stack<Cell> history;

public:

	std::map<std::string, Cell> get_cells();
	Cell get_cell(std::string cell_name);
};






































#endif
