#ifndef DEPENDENCYGRAPH_H
#define DEPENDENCYGRAPH_H

#include <string>
#include <map>
#include <unordered_set>
#include <vector>
    /**
    * Class Description:
    * 
    * (s1,t1) is an ordered pair of strings
    * t1 depends on s1; s1 must be evaluated before t1
    * 
    * A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    * (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    * Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    * set, and the element is already in the set, the set remains unchanged.
    * 
    * Given a DependencyGraph DG:
    * 
    *    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    *        (The set of things that depend on s)    
    *        
    *    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    *        (The set of things that s depends on) 
    * 
    * For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    *     dependents("a") = {"b", "c"}
    *     dependents("b") = {"d"}
    *     dependents("c") = {}
    *     dependents("d") = {"d"}
    *     dependees("a") = {}
    *     dependees("b") = {"a"}
    *     dependees("c") = {"a"}
    *     dependees("d") = {"b", "d"}
    */
class DependencyGraph{
	//Holds set of dependents that a set of dependees depend on
	std::map<std::string, std::unordered_set<std::string>> dependents_graph;

	//Holds set of dependees that depend on a set of dependents
	std::map<std::string, std::unordered_set<std::string>> dependees_graph;

	//Counts the number of ordered pairs in the DependencyGraph
	int ordered_pairs = 0;

public:
    //Builds empty dependency graph
	DependencyGraph();

    //Returns the number of ordered pairs in the DependencyGraph.
	int get_size();

    //Returns the size of dependees(s).
	int get_dependees_count(std::string s);

    //Reports whether dependents(s) is non-empty.
	bool has_dependents(std::string s);

    //Reports whether dependees(s) is non-empty.
	bool has_dependees(std::string s);

    //Returns unordered_set of all dependents of s 
	std::unordered_set<std::string> get_dependents(std::string s);

    //Returns unordered_set of all dependees of s
	std::unordered_set<std::string> get_dependees(std::string s);

    //Adds the ordered pair (s,t), if it doesn't exist
    //This should be thought of as: t depends on s
	void add_dependency(std::string s, std::string t);

    //Removes the ordered pair (s,t), if it exists
	void remove_dependency(std::string s, std::string t);

    //Removes all existing ordered pairs of the form (s,r).  Then, for each
    //t in new_dependents, adds the ordered pair (s,t).
	void replace_dependents(std::string s, std::vector<std::string> new_dependents);

    //Removes all existing ordered pairs of the form (r,s).  Then, for each 
    //t in new_dependees, adds the ordered pair (t,s).
	void replace_dependees(std::string s, std::vector<std::string> new_dependees);

};

#endif