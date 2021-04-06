// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        // Keeps the list of tokens provided by GetTokens()
        private List<string> tokens;

        // Kepps the list of normalized variables
        private List<string> normalizedVariables;

        // String that holds the normalized version of the string
        private String normalizedFormula;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            if (string.IsNullOrEmpty(formula))
                throw new FormulaFormatException("Formula is invalid! Formula is empty, add at least one expression");

            // Keeps a list of tokens as for loop enumerates 'tokens'. This is list will replace all 'tokens' after 
            // the loop is done.
            List<string> parsedTokens = new List<string>();

            normalizedFormula = "";
            tokens = new List<string>(GetTokens(formula));
            normalizedVariables = new List<string>();
            int openParentheses = 0;
            int closedParentheses = 0;
            string lastToken = tokens[tokens.Count - 1];

            if (!(tokens[0].Equals("(") || IsVariable(tokens[0]) || double.TryParse(tokens[0], out double parseDouble1)))
                throw new FormulaFormatException("Formula is invalid! The formula does not begin with a number, variable, or opening parenthesis.");

            if (!(lastToken.Equals(")") || double.TryParse(lastToken, out double parseDouble2) || IsVariable(lastToken)))
                throw new FormulaFormatException("Formula is invalid! The formula does not end with a number, variable, or opening parenthesis.");

            // Keeps track of the previous string in the loop. Used for following rule checks
            string previousToken = "";

            // Checks whether each token is one of the following: (, ), +, -, *, /, variables, and decimal real numbers (including scientific notation).
            foreach (string token in tokens)
            {
                // If the token is "(", add to total open parentheses count
                if (token == "(")
                    openParentheses++;

                // If the token is ")", add to total closing paretheses count. If adding to it makes number of closing parentheses exceed opening parentheses,
                // throw a FormulaFormatException()
                else if (token == ")")
                {
                    closedParentheses++;
                    if (closedParentheses > openParentheses)
                        throw new FormulaFormatException("Formula is invalid! The number of closing parentheses exceeds number of opening parentheses.");
                }

                // If the token is any of the operators or a double, do nothing
                else if (token == "+" || token == "-" || token == "*" || token == "/")
                {
                    // Do nothing
                }

                else if (double.TryParse(token, out double parseDouble))
                {
                    parsedTokens.Add(double.Parse(token).ToString());
                }

                // If the token is a variable, check whether the variable is valid, and if so, add it to the list of normalized variables to be used later
                else if (IsVariable(token))
                {
                    if (isValid(token))
                        normalizedVariables.Add(normalize(token));
                    else
                        throw new FormulaFormatException("Formula is invalid! Variable is invalid.");
                }

                // If the token is none of the valid tokens, throw a FormulaFormatException()
                else
                    throw new FormulaFormatException("Formula is invalid! At least one token is invalid.");

                // If the previous token of the current token is an operator or closing parentheses, checks whether the current token is a double, variable, 
                // or closing parentheses. If not, the formula is invalid and it throws a FormulaFormatException()
                if (previousToken.Equals("+") || previousToken.Equals("-") || previousToken.Equals("*") || previousToken.Equals("/") || previousToken.Equals("("))
                {
                    if (!(double.TryParse(token, out double tryParse1) || IsVariable(token) || token.Equals("(")))
                        throw new FormulaFormatException("Formula is invalid! Invalid character following an open parentheses or operator.");
                }

                // If the previous token is whitespace, a closing parentheses, double or variable, checks whether the current token is a closing parentheses or operator.
                // If not, the formula is invalid and it throws a FormulaFormatException()
                else if (!previousToken.Equals("") && (previousToken.Equals(")") || double.TryParse(previousToken, out double tryParse2) || IsVariable(previousToken)))
                    if (!(token.Equals(")") || token.Equals("+") || token.Equals("-") || token.Equals("*") || token.Equals("/")))
                        throw new FormulaFormatException("Formula is invalid! Invalid character following number, variable, or closing parentheses.");

                // At the end of the loop, sets previousToken to the current token
                previousToken = token;

                // If the token was a double, it had already been added to parsedTokens. Thus,
                // if the token is a double, add it to parsedTokens.
                if (!double.TryParse(token, out double num))
                    parsedTokens.Add(token);
            }

            // At the end of the loop, checks whether total number of open parentheses equals total number of closed parentheses. If not, formula is invalid
            // and throws a FormulaFormatException()
            if (openParentheses != closedParentheses)
                throw new FormulaFormatException("Formula is invalid! Number of open/closed parentheses do not match.");

            // Puts all normalized/parsed tokens into normalizedFormula. This list now only contains the formula with 
            // normalized and parsed doubles
            foreach (string s in parsedTokens)
            {
                if(IsVariable(s))
                {
                    normalizedFormula += normalize(s);
                    continue;
                }
                normalizedFormula += s;
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            // This method is just the formula from FormulaEvaluator but tuned to fit the different variable notation and doubles.
            Stack<double> valueStack = new Stack<double>();
            Stack<string> operatorStack = new Stack<string>();

            foreach (string token in tokens)
            {
                // Deals with +/- operators
                if (token.Equals("+") || token.Equals("-"))
                {
                    if (operatorStack.Count == 0 || (!operatorStack.IsOperator("+") && !operatorStack.IsOperator("-")))
                    {
                        operatorStack.Push(token);
                        continue;
                    }

                    // Pops top 2 values on valueStack.
                    double popValueTop = valueStack.Pop();
                    double popValueBottom = valueStack.Pop();

                    // Switch statement to evaluate appropriate operator to push onto valueStack
                    switch (operatorStack.Pop())
                    {
                        case "+":
                            {
                                valueStack.Push(popValueTop + popValueBottom);
                                operatorStack.Push(token);
                                continue;
                            }
                        case "-":
                            {
                                valueStack.Push(popValueBottom - popValueTop);
                                operatorStack.Push(token);
                                continue;
                            }
                    }
                }

                // Deals with */'/'/'(' operators
                else if (token.Equals("*") || token.Equals("/") || token.Equals("("))
                {
                    operatorStack.Push(token);
                    continue;
                }

                // Deals with ')' operator
                else if (token.Equals(")"))
                {
                    // Checks whether operatorStack has a +/- operator on top
                    if (operatorStack.IsOperator("+") || operatorStack.IsOperator("-"))
                    {
                        // Checks whether valueStack has enough values to process
                        if (valueStack.Count >= 2)
                        {
                            double popValueTop = valueStack.Pop();
                            double popValueBottom = valueStack.Pop();

                            // If statements to evaluate which operator to use to push correct values onto valueStack
                            string popOperator = operatorStack.Pop();
                            if (popOperator == "+")
                                valueStack.Push(popValueTop + popValueBottom);
                            else if (popOperator == "-")
                                valueStack.Push(popValueBottom - popValueTop);
                        }
                    }

                    operatorStack.Pop();

                    // Checks whether operatorStack has any items, and then if */'/' operator was on top
                    if (operatorStack.IsOperator("*") || operatorStack.IsOperator("/"))
                    {

                        double popValueBottom = valueStack.Pop();
                        double popValueTop = valueStack.Pop();

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
                                    if (popValueBottom == 0)
                                        return new FormulaError("Formula is invalid! Trying to divide by 0 by variable " + popValueBottom + ".");
                                    valueStack.Push(popValueTop / popValueBottom);
                                    continue;
                                }
                        }
                    }
                }

                // Deals with double values
                else if (double.TryParse(token, out double num))
                {
                    // Checks whether operatorStack has any items, and then if */'/' operator was on top
                    if (operatorStack.IsOperator("*") || operatorStack.IsOperator("/"))
                    {

                        double popValue = valueStack.Pop();

                        // Switch statement to evaluate which operator to use to push correct value onto valueStack
                        switch (operatorStack.Pop().ToString())
                        {
                            case "*":
                                {
                                    valueStack.Push(popValue * double.Parse(token));
                                    continue;
                                }
                            case "/":
                                {
                                    double doubleParse = double.Parse(token);
                                    if (doubleParse == 0)
                                        return new FormulaError("Formula is invalid! Trying to divide by 0 by variable " + token + ".");
                                    valueStack.Push(popValue / doubleParse);
                                    continue;
                                }
                        }
                    }
                    else
                        valueStack.Push(double.Parse(token));
                }

                // Deals with variable values
                else
                {
                    double variableValue = 0.0;
                    try
                    {
                        variableValue = lookup(token);
                    }
                    catch (ArgumentException)
                    {
                        return new FormulaError("Formula is invalid! Variable " + token + " does not have a value.");
                    }

                    // Checks whether operatorStack has any items, and then if */'/' operator was on top
                    if (operatorStack.IsOperator("*") || operatorStack.IsOperator("/"))
                    {
                        double popValue = valueStack.Pop();

                        // Switch statement to evaluate appropriate operator to push onto valueStack
                        switch (operatorStack.Pop())
                        {
                            case "*":
                                {
                                    valueStack.Push(popValue * variableValue);
                                    continue;
                                }
                            case "/":
                                {
                                    if (lookup(token) == 0)
                                        return new FormulaError("Formula is invalid! Trying to divide by zero with variable " + token);
                                    valueStack.Push(popValue / variableValue);
                                    continue;
                                }
                        }
                    }

                    else
                        valueStack.Push(variableValue);
                }
            }

            // Every token is processed by this point. The following if statements checks the remaining operators in operatorStack,
            // and deals with them accordingly to reach an empty operatorStack.
            if (operatorStack.Count == 1)
            {
                double finalPopValueTop = valueStack.Pop();
                double finalPopValueBottom = valueStack.Pop();

                // Switch statement to evaluate appropriate operator to push onto valueStack
                switch (operatorStack.Pop().ToString())
                {
                    case "+":
                        {
                            return finalPopValueTop + finalPopValueBottom;
                        }
                    case "-":
                        {
                            return finalPopValueBottom - finalPopValueTop;
                        }
                }
            }
            return valueStack.Pop();
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            // Returns a new HashSet<> of normalized variables
            return new HashSet<string>(normalizedVariables);
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return normalizedFormula;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            // If the object is null or object is not a formula, return false
            if (ReferenceEquals(obj, null) || obj.GetType() != this.GetType())
                return false;

            // Creates a formula object of obj
            Formula formula = (Formula)obj;

            for (int i = 0; i < this.normalizedFormula.Length; i++)
            {
                // Makes a string of the current index of each formula's tokens
                char thisToken = this.normalizedFormula[i];
                char objToken = formula.normalizedFormula[i];

                // If the strings are not doubles, checks if they are equal. If not, return false
                if (!thisToken.Equals(objToken))
                    return false;
            }

            // If the for loop never returned false, that means the formulas are equal. Returns true
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            // If both formulas are null, return true
            if (ReferenceEquals(f1, null) && ReferenceEquals(f2, null))
                return true;
            // If one formula is null and the other isn't, return false
            if ((ReferenceEquals(f1, null) && !ReferenceEquals(f2, null)) || (!ReferenceEquals(f1, null) && ReferenceEquals(f2, null)))
                return false;

            // Calls the Equals() method to check for equality
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

        /// <summary>
        /// Checks whether a given variable is valid.
        /// </summary>
        /// <param name="variable"> Variable to check. </param>
        /// <returns> True/False if the variable is valid. </returns>
        private static bool IsVariable(string variable)
        {
            // Used the comment on: https://stackoverflow.com/questions/336210/regular-expression-for-alphanumeric-and-underscores by 
            // Danuel O'Neal on Jan 31 '12 as a base regex formula to tweak until it correctly checks for correct variable notation. 
            if (Regex.IsMatch(variable, @"^[a-zA-Z_]([A-Za-z0-9_])*", RegexOptions.Singleline))
                return true;

            return false;
        }
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

/// <summary>
/// Used to report syntactic errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    /// Constructs a FormulaFormatException containing the explanatory message.
    /// </summary>
    public FormulaFormatException(String message)
        : base(message)
    {
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public struct FormulaError
{
    /// <summary>
    /// Constructs a FormulaError containing the explanatory reason.
    /// </summary>
    /// <param name="reason"></param>
    public FormulaError(String reason)
        : this()
    {
        Reason = reason;
    }

    /// <summary>
    ///  The reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}

