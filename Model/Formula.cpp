#include <iostream>
#include <vector>
#include <stack>
#include <list>
#include <string>
#include <cstring>
#include "Formula.h"


bool isValid(std::string formula) 
{
	if (formula.empty()) {
		throw "Formula is invalid! Formula is empty add at least on expression";
	}

	// Keeps a list of tokens as for loop enumerates 'tokens'. This is list will replace all 'tokens' after 
	// the loop is done.
	std::list<std::string> parsed_tokens;

	std::string normalized_formula = "";
	std::list<std::string>tokens = Formula::get_tokens(formula);
	std::list<std::string>normalized_variables;
	int open_parenthesis = 0;
	int closed_parenthesis = 0;

	auto tokens_front = tokens.begin();
	std::advance(tokens_front, tokens.size() - 1);

	std::string last_token = *tokens_front;

	std::string::size_type parse_double1;
	std::string::size_type parse_double2;

	if (!((tokens.front().compare("(") == 0) || Formula::is_variable(tokens.front())) || !(strtod(tokens.front().c_str(), NULL) == 0))
		throw "Formula is invalid! The formula does not begin with a number, variable, or opening parenthesis.";

	if (!((last_token.compare(")") == 0) || !(strtod(last_token.c_str(),NULL))==0) || Formula::is_variable(last_token))
		throw "Formula is invalid! The formula does not end with a number, variable, or opening parenthesis.";

	//Keeps track of the previous string in the loop. Used for following rule checks
	std::string previous_token = "";

	// Checks whether each token is one of the following: (, ), +, -, *, /, variables, and decimal real numbers (including scientific notation).
	for (std::string token : tokens)
	{
		// If the token is "(", add to total open parentheses count
		if (token.compare("(") == 0)
			open_parenthesis++;

		// If the token is ")", add to total closing paretheses count. If adding to it makes number of closing parentheses exceed opening parentheses,
		// throw a FormulaFormatException()
		else if (token.compare(")") == 0)
		{
			closed_parenthesis++;
			if (closed_parenthesis > open_parenthesis)
				throw "Formula is invalid! The number of closing parentheses exceeds number of opening parentheses.";
		}

		// If the token is any of the operators or a double, do nothing
		else if (token.compare("+") == 0 || token.compare("-") == 0 || token.compare("*") == 0 || token.compare("/") == 0)
		{
			//Do nothing
		}

		else if (!(strtod(token.c_str(), NULL) == 0))
		{
			parsed_tokens.push_back(std::to_string(strtod(token.c_str(), NULL)));
		}

		// If the token is a variable, check whether the variable is valid, and if so, add it to the list of normalized variables to be used later
		else if (Formula::is_variable(token)) {
			std::string variable = Formula::normalize(token);
			normalized_variables.push_back(variable);
		}

		// If the token is none of the valid tokens, throw a FormulaFormatException()
		else
			throw "Formula is invalid! At least one token is invalid.";

		if (previous_token.compare("+") == 0 || previous_token.compare("-") == 0 || previous_token.compare("*") == 0 || previous_token.compare("/") == 0 || previous_token.compare("(") == 0)
		{
			if (strtod(token.c_str(), NULL) == 0 || Formula::is_variable(token) || token.compare("(") == 0)
			{
				throw "Formula is invalid! Invalid character following number, variable, or closing parentheses.";
			}
		}

		previous_token = token;

		if (strtod(token.c_str(), NULL))
		{
			parsed_tokens.push_back(token);
		}
	}

	// At the end of the loop, checks whether total number of open parentheses equals total number of closed parentheses. If not, formula is invalid
	// and throws a FormulaFormatException()
	if(open_parenthesis != closed_parenthesis){
		throw "Formula is invalid! Number of open/closed parentheses do not match.";

	// Puts all normalized/parsed tokens into normalizedFormula. This list now only contains the formula with 
	// normalized and parsed doubles
		for (std::string s : parsed_tokens)
		{
			if (Formula::is_variable(s))
			{
				normalized_formula += Formula::normalize(s);
				continue;
			}
			normalized_formula += s;
		}
	}
}

std::vector<std::string> get_tokens(std::string formula, std::stack<std::string>& operatorStack, std::stack<std::string>& parenthesesStack)
{
	std::vector<std::string> tokens;

	// Delete any whitespace
	formula.erase(remove_if(formula.begin(), formula.end(), isspace), formula.end());

	for (int i = 0; i < formula.length(); ++i)
	{
		std::string token = "";
		while (formula[i] != '+' && formula[i] != '-' && formula[i] != '(' && formula[i] != ')' && formula[i] != '*' && formula[i] != '/')
		{
			token += formula[i];
			if (i < formula.length())
				++i;
			else
			{
				tokens.push_back(token);
				return tokens;
			}
		}

		if (token != "")
		{
			--i;
		}
		else
		{
			if (formula[i] == '+' || formula[i] == '-' || formula[i] == '*' || formula[i] == '/')
			{
				tokens.push_back(std::string(1, formula[i]));
				operatorStack.push(std::string(1, formula[i]));
				continue;
			}
			else
			{
				tokens.push_back(std::string(1, formula[i]));
				parenthesesStack.push(std::string(1, formula[i]));
				continue;
			}
		}

		tokens.push_back(token);
	}

	return tokens;
}

static bool is_variable(std::string s)
{
	int i = 0;
	while (std::isalpha(s[i]))
	{
		if (i != s.length())
			++i;
		else
			return false;
	}

	while (std::isalnum(s[i]))
	{
		if (i != s.length())
			++i;
		else
			return true;
	}
	return false;
}

std::string Formula::normalize(std::string variable) {
	std::string s;

	for (int i = 0; i < variable.size(); i++) 
		s += std::toupper(variable[i]);	
	
	return s;
}
