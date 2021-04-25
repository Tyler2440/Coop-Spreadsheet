#ifndef DEPENDENCYGRAPH_H
#define DEPENDENCYGRAPH_H

#include <string>
#include <map>
#include <unordered_set>
#include <vector>

class DependencyGraph{

	std::map<std::string, std::unordered_set<std::string>> dependents_graph;
	std::map<std::string, std::unordered_set<std::string>> dependees_graph;
	int ordered_pairs = 0;

public:
	DependencyGraph();
	int get_size();
	int get_dependees_count(std::string s);
	bool has_dependents(std::string s);
	bool has_dependees(std::string s);
	std::unordered_set<std::string> get_dependents(std::string s);
	std::unordered_set<std::string> get_dependees(std::string s);
	void add_dependency(std::string s, std::string t);
	void remove_dependency(std::string s, std::string t);
	void replace_dependents(std::string s, std::vector<std::string> new_dependents);
	void replace_dependees(std::string s, std::vector<std::string> new_dependees);

};

#endif