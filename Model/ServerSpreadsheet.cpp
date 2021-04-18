#include <map>
#include "ServerSpreadsheet.h"

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


std::string Cell::get_name()
{
	return this->cell_name;
}

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
