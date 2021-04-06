using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace DevelopmentTests
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {
        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        /// Adding null values to DependencyGraph t to get exception
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SimpleNullTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsTrue(t["s"] == 0);
            t.AddDependency(null, null);
        }

        /// <summary>
        /// Adds a null dependee, expects exception
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SimpleAddNullDependentTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("s", null);
        }

        /// <summary>
        /// Adds a null dependent, expects exception
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SimpleAddNullDependeeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency(null, "s");
        }

        /// <summary>
        /// Tries to get dependents of non-existant dependee.
        /// </summary>
        [TestMethod()]
        public void HasDependentsEmptyGraph()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsFalse(t.HasDependents("s"));
            t.AddDependency("s", "a");
            Assert.IsTrue(t.HasDependees("a"));
        }

        /// <summary>
        /// Tries to get dependents of non-existant dependee.
        /// </summary>
        [TestMethod()]
        public void ReplaceDependentsNewSet()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("s", "a");
            t.AddDependency("b", "a");
            t.ReplaceDependees("a", new HashSet<string> { "c", "d" });
            HashSet<string> set = new HashSet<string> { "c", "d" };
            foreach (string s in set)
            {
                foreach (string dependents in t.GetDependents("a"))
                    Assert.IsTrue(dependents == s);
            }
        }

        /// <summary>
        /// Tries to get dependees of non-existant dependent.
        /// </summary>
        [TestMethod()]
        public void HasDependeesEmptyGraph()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsFalse(t.HasDependees("s"));
            t.AddDependency("s", "a");
            Assert.IsTrue(t.HasDependents("s"));
        }

        /// <summary>
        /// Gets a set of empty dependents, asserts it gives empty enumerator
        /// </summary>
        [TestMethod()]
        public void GetEmptyDependents()
        {
            DependencyGraph t = new DependencyGraph();
            IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());
        }

        [TestMethod()]
        public void ReplaceDependents()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("s", "a");
            t.ReplaceDependees("a", new HashSet<string>());
            IEnumerator<string> e = t.GetDependents("s").GetEnumerator();
            IEnumerator<string> f = t.GetDependees("a").GetEnumerator();
            Console.Error.WriteLine(e.MoveNext());
            Assert.IsFalse(e.ToString() == f.MoveNext().ToString());
        }

        /// <summary>
        /// Gets a set of empty dependents, asserts it gives empty enumerator
        /// </summary>
        [TestMethod()]
        public void RemoveFromEmptyGraph()
        {
            DependencyGraph t = new DependencyGraph();
            t.RemoveDependency("s", "a");
            Assert.IsTrue(t.Size == 0);
        }

        /// <summary>
        /// Gets a set of empty dependees, asserts it gives empty enumerator
        /// </summary>
        [TestMethod()]
        public void GetEmptyDependees()
        {
            DependencyGraph t = new DependencyGraph();
            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        /// Empty graph should not be able to remove items.
        /// </summary>
        [TestMethod()]
        public void RemoveOnEmptyGraphTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsTrue(t.Size == 0);
            t.RemoveDependency("x", "y");
            Assert.IsTrue(t.Size == 0);
            t.AddDependency("x", "y");
            Assert.IsTrue(t.Size == 1);
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }



        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());

            bool assert1 = (s1 == "a") && (s2 == "c");
            bool assert2 = (s1 == "c") && (s2 == "a");
            Assert.IsTrue(assert1 || assert2);

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());


            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());

            bool assert1 = ((s1 == "a") && (s2 == "c"));
            bool assert2 = ((s1 == "c") && (s2 == "a"));
            Assert.IsTrue(assert1 || assert2);
            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [TestMethod()]
        public void IndexerMethodTest()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("x", "b");
            Assert.IsTrue(t.HasDependents("x"));
            Assert.IsTrue(t.HasDependees("b"));
            t.RemoveDependency("x", "b");

            t.AddDependency("t", "m");
            Assert.IsTrue(t["m"] == 1);
            Assert.IsTrue(t["t"] == 0);
        }

        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

    }
}