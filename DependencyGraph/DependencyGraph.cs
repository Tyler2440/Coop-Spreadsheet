
// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        // Holds set of dependents that a set of dependees depend on
        private Dictionary<string, HashSet<string>> dependentsGraph;

        // Holds set of dependees that depend on a set of dependents
        private Dictionary<string, HashSet<string>> dependeesGraph;

        // Counts the number of ordered pairs in the DependencyGraph
        private int orderedPairs = 0;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependentsGraph = new Dictionary<string, HashSet<string>>();
            dependeesGraph = new Dictionary<string, HashSet<string>>();
            orderedPairs = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return orderedPairs; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                return GetDependees(s).Count();
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependentsGraph.ContainsKey(s))
                return dependentsGraph[s].Count > 0;
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (dependeesGraph.ContainsKey(s))
                return dependeesGraph[s].Count > 0;
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            // Checks whether dependeesGraph contains [s], if so, return a IEnumberable the size of its dependee set.
            if (dependentsGraph.ContainsKey(s))
                return new HashSet<string>(dependentsGraph[s]);

            // Returns empty HashSet if dependeesGraph does not contain [s].
            return new HashSet<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            // Checks whether dependentsGraph contains [s], if so, return enumerable the size of its set.
            if (dependeesGraph.ContainsKey(s))
                return new HashSet<string>(dependeesGraph[s]);

            // Returns empty HashSet if dependeesGraph does not contain [s].
            return new HashSet<string>();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param> 
        public void AddDependency(string s, string t)
        {
            // If the pair does not already exist, add to total number of ordered pairs.
            if (!dependentsGraph.ContainsKey(s) || !dependeesGraph.ContainsKey(t))
                orderedPairs++;

            // If s already exists in dependentsGraph, add t to its dependees map.
            if (dependentsGraph.ContainsKey(s))

                // Adds t to the dependees map of s. Will not add if s already contains t.
                dependentsGraph[s].Add(t);

            // If s does not exist in dependeesGraph, create a new set of dependees with t, add it to its dependent, s.
            else
            {
                HashSet<string> dependees = new HashSet<string>();
                dependees.Add(t);
                dependentsGraph.Add(s, dependees);
            }

            // If t already exists in dependeesGraph, add s to its dependents map.
            if (dependeesGraph.ContainsKey(t))

                // Adds s to the dependents map of t. Will not add if t already contains s.
                dependeesGraph[t].Add(s);

            // If t does not exist in dependentsGraph, create a new set of dependents with s, add it to its dependee, t.
            else
            {
                HashSet<string> dependents = new HashSet<string>();
                dependents.Add(s);
                dependeesGraph.Add(t, dependents);
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            // Only removes s/t from the graphs if they are contained in those graphs. Otherwise,
            // there is no need to add/remove new sets of dependees/dependents.

            // If the pair already exists, subtract from total number of ordered pairs.
            if (dependentsGraph.ContainsKey(s) && dependeesGraph.ContainsKey(t))
                orderedPairs--;

            // If s exists in dependeesGraph, remove t from its set of dependees.
            if (dependentsGraph.ContainsKey(s))
            {
                dependentsGraph[s].Remove(t);
                if (dependentsGraph[s].Count == 0)
                    dependentsGraph.Remove(s);
            }

            // If t exists in dependentsGraph, remove s from its set of dependents.
            if (dependeesGraph.ContainsKey(t))
            {
                dependeesGraph[t].Remove(s);
                if (dependeesGraph[t].Count == 0)
                    dependeesGraph.Remove(t);
            }
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            // Removes every dependent of string s
            foreach (string oldDependentString in GetDependents(s))
                RemoveDependency(s, oldDependentString);

            // Adds a new dependency of string s for each dependent in newDependents
            foreach (string newDependentString in newDependents)
                AddDependency(s, newDependentString);
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            // Removes every dependee of string s
            foreach (string oldDependentString in GetDependees(s))
                RemoveDependency(oldDependentString, s);

            // Adds a new dependee of string s for each dependee in newDependees
            foreach (string newDependentString in newDependees)
                AddDependency(newDependentString, s);
        }

    }

}
