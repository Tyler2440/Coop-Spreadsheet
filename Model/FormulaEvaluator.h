#pragma once

#include <iostream>
#include <vector>
#include <stack>

class Evaluator
{
	std::vector<std::string> substrings;
	std::stack<int> valueStack;
	std::stack<std::string> operatorStack;
	typedef int Lookup(std::string s);

	static int is_variable(std::string s);

	template <typename T>

	static bool is_operator(std::stack<T> stack, std::string c);

public:

	static int evaluate(std::string formula, Lookup variableEvaluator);

};