#include "DependencyGraph.h"
#include <string>


DependencyGraph::DependencyGraph() {
	DependencyGraph::dependents_graph = std::map<std::string, std::unordered_set<std::string>>();
	DependencyGraph::dependees_graph = std::map<std::string, std::unordered_set<std::string>>();
	DependencyGraph::ordered_pairs = 0;
}

int DependencyGraph::get_size() {
	return DependencyGraph::ordered_pairs;
}

int DependencyGraph::get_dependees_count(std::string s) {
	return DependencyGraph::get_dependees(s).size();
}

bool DependencyGraph::has_dependents(std::string s) {

	/*if (DependencyGraph::dependents_graph.find(s) != DependencyGraph::dependees_graph.end())
	{

	}*/

	if (DependencyGraph::dependents_graph.count(s) > 0)
		return true;
	return false;
}

bool DependencyGraph::has_dependees(std::string s) {
	if (DependencyGraph::dependees_graph.count(s) > 0)
		return true;
	return false;
}

std::unordered_set<std::string> DependencyGraph::get_dependents(std::string s) {

	if (DependencyGraph::dependents_graph.find(s) != DependencyGraph::dependents_graph.end())

		return std::unordered_set<std::string>(DependencyGraph::dependents_graph[s]);

	return std::unordered_set<std::string>();
}

std::unordered_set<std::string> DependencyGraph::get_dependees(std::string s) {

	if (DependencyGraph::dependees_graph.find(s) != DependencyGraph::dependees_graph.end())
		return std::unordered_set<std::string>(DependencyGraph::dependees_graph[s]);

	return std::unordered_set<std::string>();
}

void DependencyGraph::add_dependency(std::string s, std::string t) {

	// If the pair does not already exist, add to total number of ordered pairs.
	if (!(DependencyGraph::dependents_graph.find(s) != DependencyGraph::dependents_graph.end()) || !(DependencyGraph::dependees_graph.find(t) != DependencyGraph::dependees_graph.end()))
		DependencyGraph::ordered_pairs += 1;

	// If s already exists in dependentsGraph, add t to its dependees map.
	if (DependencyGraph::dependents_graph.find(s) != DependencyGraph::dependents_graph.end())
		DependencyGraph::dependents_graph[s].insert(t);
	// If s does not exist in dependeesGraph, create a new set of dependees with t, add it to its dependent, s.
	else 
	{
		std::unordered_set < std::string> dependees = std::unordered_set<std::string>();
		dependees.insert(t);
		DependencyGraph::dependents_graph.insert(std::pair<std::string, std::unordered_set<std::string>>(s, dependees));
	}

	// If t already exists in dependeesGraph, add s to its dependents map.
	if (DependencyGraph::dependees_graph.find(t) != DependencyGraph::dependees_graph.end())
		DependencyGraph::dependees_graph[t].insert(s);
	// If t does not exist in dependentsGraph, create a new set of dependents with s, add it to its dependee, t.
	else 
	{
		std::unordered_set < std::string> dependents = std::unordered_set<std::string>();
		dependents.insert(s);
		DependencyGraph::dependees_graph.insert(std::pair<std::string, std::unordered_set<std::string>>(t, dependents));
	}
}

void DependencyGraph::remove_dependency(std::string s, std::string t) {
	// Only removes s/t from the graphs if they are contained in those graphs. Otherwise,
	// there is no need to add/remove new sets of dependees/dependents.

	// If the pair already exists, subtract from total number of ordered pairs.
	if ((DependencyGraph::dependents_graph.find(s) != DependencyGraph::dependents_graph.end()) && (DependencyGraph::dependees_graph.find(t) != DependencyGraph::dependees_graph.end()))
		DependencyGraph::ordered_pairs--;

	// If s exists in dependeesGraph, remove t from its set of dependees.
	if (DependencyGraph::dependents_graph.find(s) != DependencyGraph::dependents_graph.end())
	{
		DependencyGraph::dependents_graph[s].erase(t);
		if (DependencyGraph::dependents_graph.count(s) == 0)
			DependencyGraph::dependents_graph.erase(s);
	}

	// If t exists in dependentsGraph, remove s from its set of dependents.
	if (DependencyGraph::dependees_graph.find(t) != DependencyGraph::dependees_graph.end())
	{
		DependencyGraph::dependees_graph[t].erase(s);
		if (DependencyGraph::dependees_graph.count(t) == 0)
			DependencyGraph::dependees_graph.erase(t);
	}

}

void DependencyGraph::replace_dependents(std::string s, std::vector<std::string> new_dependents) {
	for (std::string old_dependent_string : DependencyGraph::get_dependents(s))
		DependencyGraph::remove_dependency(s, old_dependent_string);

	for (std::string new_dependent_string : new_dependents)
		DependencyGraph::add_dependency(s, new_dependent_string);
		
}

void DependencyGraph::replace_dependees(std::string s, std::vector<std::string> new_dependees) {
	for (std::string old_dependent_string : DependencyGraph::get_dependees(s))
		DependencyGraph::remove_dependency(old_dependent_string,s);

	for (std::string new_dependent_string : new_dependees)
		DependencyGraph::add_dependency(new_dependent_string,s);
}