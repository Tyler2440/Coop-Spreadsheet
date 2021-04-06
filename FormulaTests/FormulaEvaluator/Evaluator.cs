using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author: Tyler Allen u1211154
    /// Purpose: Provide a library that uses infix expression evaluation to solve a given equation in the correct order.
    /// </summary>
    public class Evaluator
    {
        /// <summary>
        /// Delegate declaration. Takes in a string as its parameter and processes it to find
        /// the integer value of that string.
        /// </summary>
        /// <param name="v"> String that user wants to find integer value of. </param>
        /// <returns> Integer value of the given string. </returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// Breaks down given equation, given as string 's', into its parts to solve it using
        /// correct notation.
        /// </summary>
        /// <param name="s"> Equation needed to be solved. </param>
        /// <param name="variableEvaluator"> Given way of providing variables integer values. </param>
        /// <returns> Integer value from the evaluation of equation. </returns>
        public static int Evaluate(string s, Lookup variableEvaluator)
        {
            string[] substrings = Regex.Split(s, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Stack<int> valueStack = new Stack<int>();
            Stack<string> operatorStack = new Stack<string>();

            foreach (string token in substrings)
            {
                // Trims token to rid of extra whitespace
                String tokenValue = token.Trim();

                // Deals with extra whitespace
                if (tokenValue.Equals(" ") || tokenValue.Equals(""))
                    continue;

                // Deals with +/- operators
                if (tokenValue.Equals("+") || tokenValue.Equals("-"))
                {
                    if (operatorStack.Count == 0 || (!operatorStack.IsOperator("+") && !operatorStack.IsOperator("-")))
                    {
                        operatorStack.Push(tokenValue);
                        continue;
                    }

                    // Checks whether valueStack has enough values to process
                    if (valueStack.Count < 2)
                    {
                        throw new ArgumentException("Value stack contains fewer than 2 values!");
                    }

                    // Pops top 2 values on valueStack.
                    int popValueTop = int.Parse(valueStack.Pop().ToString());
                    int popValueBottom = int.Parse(valueStack.Pop().ToString());

                    // Switch statement to evaluate appropriate operator to push onto valueStack
                    switch (operatorStack.Pop())
                    {
                        case "+":
                            {
                                valueStack.Push(popValueTop + popValueBottom);
                                operatorStack.Push(tokenValue);
                                continue;
                            }
                        case "-":
                            {
                                valueStack.Push(popValueBottom - popValueTop);
                                operatorStack.Push(tokenValue);
                                continue;
                            }
                    }
                }

                // Deals with */'/'/'(' operators
                else if (tokenValue.Equals("*") || tokenValue.Equals("/") || tokenValue.Equals("("))
                    operatorStack.Push(tokenValue);

                // Deals with ')' operator
                else if (tokenValue.Equals(")"))
                {
                    // Checks whether operatorStack has a +/- operator on top
                    if (operatorStack.IsOperator("+") || operatorStack.IsOperator("-"))
                    {
                        // Checks whether valueStack has enough values to process
                        if (valueStack.Count >= 2)
                        {
                            int popValueTop = int.Parse(valueStack.Pop().ToString());
                            int popValueBottom = int.Parse(valueStack.Pop().ToString());

                            // If statements to evaluate which operator to use to push correct values onto valueStack
                            string popOperator = operatorStack.Pop().ToString();
                            if (popOperator == "+")
                                valueStack.Push(popValueTop + popValueBottom);
                            else if (popOperator == "-")
                                valueStack.Push(popValueBottom - popValueTop);
                        }
                    }

                    // Popping '(' from operator stack
                    if (operatorStack.Count == 0 || operatorStack.Pop().ToString() != "(")
                    {
                        throw new ArgumentException();
                    }

                    // Checks whether operatorStack has any items, and then if */'/' operator was on top
                    if (operatorStack.IsOperator("*") || operatorStack.IsOperator("/"))
                    {
                        // Checks whether valueStack has enough values to process
                        if (valueStack.Count < 2)
                        {
                            throw new ArgumentException("Value stack contains fewer than 2 values!");
                        }

                        int popValueTop = int.Parse(valueStack.Pop().ToString());
                        int popValueBottom = int.Parse(valueStack.Pop().ToString());

                        // Switch statement to evaluate appropriate operator to push onto valueStack
                        switch (operatorStack.Pop().ToString())
                        {
                            case "*":
                                {
                                    valueStack.Push(popValueTop * popValueBottom);
                                    continue;
                                }
                            case "/":
                                {
                                    try
                                    {
                                        valueStack.Push(popValueTop / popValueBottom);
                                    }
                                    catch (DivideByZeroException )
                                    {
                                        throw new ArgumentException("Cannot divide by 0!");
                                    }
                                    continue;
                                }
                        }
                    }
                }

                // Deals with integer values
                else if (int.TryParse(tokenValue, out int num))
                {
                    // Checks whether operatorStack has any items, and then if */'/' operator was on top
                    if (operatorStack.IsOperator("*") || operatorStack.IsOperator("/"))
                    {

                        int popValue = int.Parse(valueStack.Pop().ToString());

                        // Switch statement to evaluate which operator to use to push correct value onto valueStack
                        switch (operatorStack.Pop().ToString())
                        {
                            case "*":
                                {
                                    valueStack.Push(popValue * int.Parse(tokenValue));
                                    continue;
                                }
                            case "/":
                                {
                                    try
                                    {
                                        valueStack.Push(popValue / int.Parse(tokenValue));
                                    }
                                    catch (DivideByZeroException)
                                    {
                                        throw new ArgumentException("Cannot divide by 0!");
                                    }
                                    continue;
                                }
                        }
                    }
                    else
                    {
                        valueStack.Push(int.Parse(tokenValue));
                    }
                }

                // Deals with variable values
                else
                {
                    if (!IsVariable(tokenValue))
                        throw new ArgumentException();
                    // Checks whether variableEvaluator finds value for variable.
                    try
                    {
                        
                        int variableValue = variableEvaluator(tokenValue);
                    }
                    catch (NullReferenceException)
                    {
                        throw new ArgumentException("Invalid variable!");
                    }

                    // Checks whether operatorStack has any items, and then if */'/' operator was on top
                    if (operatorStack.IsOperator("*") || operatorStack.IsOperator("/"))
                    {
                        int popValue = int.Parse(valueStack.Pop().ToString()); 

                        // Switch statement to evaluate appropriate operator to push onto valueStack
                        switch (operatorStack.Pop().ToString())
                        {
                            case "*":
                                {

                                    valueStack.Push(popValue * variableEvaluator(tokenValue));
                                    continue;
                                }
                            case "/":
                                {
                                    try
                                    {
                                        valueStack.Push(popValue / variableEvaluator(tokenValue));
                                    }
                                    catch (DivideByZeroException)
                                    {
                                        throw new ArgumentException("Cannot divide by 0!");
                                    }
                                    continue;
                                }
                        }
                    }
                    else
                    {
                        valueStack.Push(variableEvaluator(tokenValue));
                        continue;
                    }
                    throw new ArgumentException("Unrecognized/inappropriate value!");
                }
            }

            // Every token is processed by this point. The following if statements check for 
            // remaining operators in operatorStack, and deals with them accordingly to reach an empty operatorStack.
            if (valueStack.Count > 2)
                throw new ArgumentException("Too many values on the stack!");

            if (operatorStack.Count == 0)
            {
                if (valueStack.Count != 1)
                    throw new ArgumentException();

                return int.Parse(valueStack.Pop().ToString());
            }

            else if (operatorStack.Count == 1)
            {
                if (valueStack.Count < 2)
                    throw new ArgumentException("Value stack contains fewer than 2 values!");
                int popValueTop = int.Parse(valueStack.Pop().ToString());
                int popValueBottom = int.Parse(valueStack.Pop().ToString());

                // Switch statement to evaluate appropriate operator to push onto valueStack
                switch (operatorStack.Peek().ToString())
                {
                    case "+":
                        {
                            return popValueTop + popValueBottom;
                        }
                    case "-":
                        {
                            return popValueBottom - popValueTop;
                        }
                }
                throw new ArgumentException("Not the appropriate operator!");
            }
            else
                throw new ArgumentException("Too many operators in the operator stack!");
        }

        /// <summary>
        /// Checks whether the string is a variable that fits the requirements
        /// of a variable (x number of letters followed by x number of numbers).
        /// </summary>
        /// <param name="s"> Given variable. </param>
        /// <returns> True/False whether string is variable. </returns>
        private static bool IsVariable(string s)
        {
            bool foundLetter = false;
            bool foundNumber = false;

            int i;
            for (i = 0; i < s.Length; i++)
            {
                // Check if s[i] is a letter
                if (Char.IsLetter(s[i]))
                    foundLetter = true;
                else
                    break;
            }

            for (; i < s.Length; i++)
            {
                // Check if s[i] is a number
                if (Char.IsDigit(s[i]))
                    foundNumber = true;
                else
                    throw new ArgumentException("Invalid variable!");
            }

            return foundLetter && foundNumber;
        }
    }

    /// <summary>
    /// Extension class that helps check whether the top operator on stack
    /// is the correct operator.
    /// </summary>
    static class EvaluatorExtension
    {
        /// <summary>
        /// Checks whether top operator on stack fits the
        /// wanted operator.
        /// </summary>
        /// <typeparam name="T"> Generic type of stack. </typeparam>
        /// <param name="stack"> Stack to check for appropriate operator. </param>
        /// <param name="c"> Wanted operator. </param>
        /// <returns> Whether the wanted operator fits the stack's top operator. </returns>
        public static bool IsOperator<T>(this Stack<T> stack, string c)
        {
            if (stack.Count == 0)
                return false;

            return stack.Peek().ToString().Equals(c);
        }
    }
}
