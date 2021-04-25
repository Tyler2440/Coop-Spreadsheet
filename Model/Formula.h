#pragma once

#include <iostream>
#include <vector>
#include <stack>
#include <list>

class Formula
{
public:
	static bool isValid(std::string s);
	static std::list<std::string> get_tokens(std::string formula);
	static std::string normalize(std::string variable);
	static bool is_variable(std::string s);
};