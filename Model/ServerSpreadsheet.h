#ifndef SERVERSPREADSHEET_H
#define SERVERSPREADSHEET_H

#include <string>
#include <map>
#include <stack>
#include <boost/json.hpp>
#include <vector>
#include <boost/property_tree/ptree.hpp>
#include "DependencyGraph.h"
#include "Formula.h"

/*
* Holds the value of a cells 
*/
class Cell {

	std::string cell_name; // Name of the cell
	std::string contents;  // Contents of the cell
	Cell* prev_state; // previous state of the cell
	
public:
	Cell();
	Cell(Cell* c);
	Cell(std::string name, std::string content); //constructor 
	Cell(std::string name, std::string content, Cell* prev); // constructor for load from file

	std::string get_name(); // Get the name of this cell
	std::string get_contents(); // Get the contents of this cell
	Cell* get_previous(); // Get the previous state of this cell
	void set_previous(Cell* c); // Set this cells previous cell state
	//void add_history(Cell cell); // Add to history of this cell

	void set_name(std::string name); // Set the name of this cell
	void set_contents(std::string content); // Set the contents of this cell
};

/*
* Represents a user of a spreadsheet
*/
class User 
{	
	private:
		int ID; // This user's ID
		std::string selected;				// This user's selected cell 
		std::string name;					// This user's name 

	public:
		int get_ID();						// Get the user ID
		std::string get_selected();			// Get the cell this user has selected
		std::string get_name();				// Get the name of this user
		void select(std::string cell_name); // Select a new cell

		User();	// Default constructor
		User (int ID, std::string name); // Constructor given the user's ID and name
};

/*
* Represents collaborative spreadsheet
*/
class Spreadsheet {

	std::map<std::string, Cell*>* cells; // List of this spreadsheet's cells
	std::map<int, User> users;			 // List of this spreadsheet's users
	std::string name;					 // Name of this spreadsheet
	std::stack<Cell*>* history;			 // Holds the change history of this spreadsheet
	DependencyGraph* graph;

	boost::json::array get_json_cells();				// Gets the cells from the file
	boost::json::array get_json_history();				// Gets the history stack from the file
	boost::json::object get_json_cell(Cell c);			// Gets an individual cell's information
	boost::json::array get_json_graph();				// Gets the dependency graph of this spreadsheet from the file
	void Visit(std::string start, std::string name, std::unordered_set<std::string> visited);	// Circular dependency checker; "visits" each of the cells in the dependency graph
																								// to check for circular dependencies

public:
	Spreadsheet(std::string s);	// Spreadsheet constructor, takes in a name
	Spreadsheet(std::string s, std::map<std::string, Cell*>* cells, std::stack<Cell*>* history, DependencyGraph* graph);	// Spreadsheet constructor, takes in a name, list of cells,
	std::map<std::string, Cell*>* get_cells();	// Gets the edited cells of this spreadsheet								// history stack, and dependency graph
	Cell* get_cell(std::string cell_name);		// Gets a cell from the list of cells given a cell name
																	// the formula contains a circular dependency
	bool insert_cell(Cell* c);
	bool edit_cell(Cell* c);
	Cell* undo(bool& success);
	bool revert(Cell* c);

	void select_cell(int ID_of_selector, std::string cell_name);	// Selects a given cell (by cell_name) for the user (by ID_of_selector)
	void check_circular_dependency(Formula formula);				// Checks if the given formula contains a circular dependency
	//Cell undo();								// Reverts the last change to the entire spreadsheet
	Cell revert(std::string s, bool& success);	// Reverts the last change of a given cell, returns true if no errors, and false if client tries to revert on empty cell
	std::stack<Cell*>* get_history();			// Gets the change history of the spreadsheet
	std::string get_name();						// Gets the name of this spreadsheet

	const std::map<int, User> get_users();		// Get the list of users on this spreadsheet
	void add_user(std::string name, int ID);	// Add a user to this spreadsheet
	void delete_user(int ID);					// Remove a user from this spreadsheet

	std::string get_json();						// Serializes this spreadsheet into a json
};

#endif
