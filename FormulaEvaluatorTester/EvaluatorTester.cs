using FormulaEvaluator;
using System;
using System.Collections.Generic;

namespace FormulaEvaluatorTester
{

    public class FakeSpreadsheet
    {
        private Dictionary<string, int> fake = new Dictionary<string, int>();

        public void AddCell(string cellName, int cellValue)
        {
            fake[cellName] = cellValue;
        }

        public int GetCell(string cellName)
        {
            if (fake.ContainsKey(cellName))
                return fake[cellName];
            throw new ArgumentException("Unknown Cell!");
        }
    }
    class EvaluatorTester
    {
        static FakeSpreadsheet cells = new FakeSpreadsheet();

        static void Main(string[] args)
        {
            cells.AddCell("Aa1", 15);
            cells.AddCell("aA1", 3);
            cells.AddCell("aaa1213213213121321231", -105);
            string test1 = "(3+0)/3 + 1";
            string test2 = "(1)+(1)-(1)+(1)-(1)+(1)+(1)/(1)";
            string test3 = "1/0";
            string test4 = "a  6";
            string test5 = "+ 1";
            string test6 = "+*";
            string test7 = "9+0)/1";
            string test8 = " ";
            string test9 = "AAAAAAAAAAAAAAAAAAAAA612345 + 1";
            string test10 = " 1              1";
            string test11 = "((8+1)/3)/3";
            string test12 = "6/2*(1+2)";
            string test13 = "((3+3)*2)/2";
            string test14 = "6A+ 1";
            string test15 = "1+1-1+                                                                                  1";
            string test16 = " 1111111111 +                                                                            2/1";

            Console.WriteLine("Evaluator 1: " + Evaluator.Evaluate(test1, formulaEvaluator1));
            Console.WriteLine("Evaluator 2: " + Evaluator.Evaluate(test2, formulaEvaluator1));

            Console.WriteLine("Evaluator cell: " + Evaluator.Evaluate("Aa1 / aA1", x => cells.GetCell(x)));
            int num = 3;

            try
            {

                Console.WriteLine("Evaluator 3: " + Evaluator.Evaluate(test3, formulaEvaluator1));

            }

            catch (ArgumentException)
            {
                num++;
                Console.WriteLine("Correctly got Evaluator " + num + " to be an exception.");
            }

            try
            {
                Console.WriteLine("Evaluator 4: " + Evaluator.Evaluate(test4, formulaEvaluator1));


            }

            catch (ArgumentException)
            {
                num++;
                Console.WriteLine("Correctly got Evaluator " + num + " to be an exception.");
            }
            try
            {
                Console.WriteLine("Evaluator 5: " + Evaluator.Evaluate(test5, formulaEvaluator1));
            }

            catch (ArgumentException)
            {
                num++;
                Console.WriteLine("Correctly got Evaluator " + num + " to be an exception.");
            }

            try
            {
                Console.WriteLine("Evaluator 6: " + Evaluator.Evaluate(test6, formulaEvaluator1));
            }

            catch (ArgumentException)
            {
                num++;
                Console.WriteLine("Correctly got Evaluator " + num + " to be an exception.");
            }

            try
            {
                Console.WriteLine("Evaluator 7: " + Evaluator.Evaluate(test7, formulaEvaluator1));
            }

            catch (ArgumentException)
            {
                num++;
                Console.WriteLine("Correctly got Evaluator " + num + " to be an exception.");
            }

            try
            {
                Console.WriteLine("Evaluator 8: " + Evaluator.Evaluate(test8, formulaEvaluator1));
            }

            catch (ArgumentException)
            {
                num++;
                num++;
                Console.WriteLine("Correctly got Evaluator " + num + " to be an exception.");
            }

            try
            {
                Console.WriteLine("Evaluator 10: " + Evaluator.Evaluate(test10, formulaEvaluator1));
            }

            catch (ArgumentException)
            {
                Console.WriteLine("Correctly got Evaluator " + num + " to be an exception.");
            }
            Console.WriteLine("Evaluator 9: " + Evaluator.Evaluate(test9, formulaEvaluator1));

            Console.WriteLine("Evaluator 11: " + Evaluator.Evaluate(test11, formulaEvaluator1));
            Console.WriteLine("Evaluator 12: " + Evaluator.Evaluate(test12, formulaEvaluator1));
            Console.WriteLine("Evaluator 13: " + Evaluator.Evaluate(test13, formulaEvaluator1));
            try
            {
                Console.WriteLine("Evaluator 14: " + Evaluator.Evaluate(test14, formulaEvaluator1));
            }
            catch (ArgumentException)
            {
                num = 14;
                Console.WriteLine("Correctly got Evaluator " + num + " to be an exception.");
            }

            try
            {
                Console.WriteLine("Evaluator 15: " + Evaluator.Evaluate(test15, formulaEvaluator1));
            }
            catch (ArgumentException)
            {
                num++;
                Console.WriteLine("Correctly got Evaluator " + num + " to be an exception.");
            }
            Console.WriteLine("Evaluator 16: " + Evaluator.Evaluate(test16, formulaEvaluator1));
        }

        private static int formulaEvaluator1(string s)
        {
            return 10;
        }
    }
}
