#pragma once

#include <iostream>
#include <vector>
#include <stack>
#include <list>


class Formula
{
	//keeps a list of normalized varialbes
	std::vector<std::string> normalized_variables;

public:
	// Creates a Formula from a string 
	Formula(std::string formula);
	// Checks if the passed in formula is valid
	bool isValid(std::string s);
	//returns the tokens that compose the passed in string
	static std::vector<std::string> get_tokens(std::string formula);
	//returns the captilized version of the passed in string
	static std::string normalize(std::string variable);
	//checks if the passed in string is a variable
	static bool is_variable(std::string& s);
	//returns the normalized_varialbes for the formula
	std::vector<std::string> get_variables();
};