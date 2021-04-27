#pragma once

#include <iostream>
#include <vector>
#include <stack>
#include <list>

class Formula
{
	std::vector<std::string>* normalized_variables;

public:
	Formula(std::string formula);
	bool isValid(std::string s);
	static std::vector<std::string> get_tokens(std::string formula);
	static std::string normalize(std::string variable);
	static bool is_variable(std::string& s);
	std::vector<std::string>* get_variables();
};