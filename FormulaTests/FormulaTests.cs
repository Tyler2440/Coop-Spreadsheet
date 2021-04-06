using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;

namespace FormulaTester
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestBadFormulaConstructorEmptyFormula()
        {
            Formula f = new Formula("");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorNullFormula()
        {
            Formula f = new Formula(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorBadToken()
        {
            Formula f = new Formula("&");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorBadStartTokenPercent()
        {
            Formula f = new Formula("% + 5");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorBadEndToken()
        {
            Formula f = new Formula("5 + @");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestBadFormulaConstructorOnlyVariable()
        {
            Formula f = new Formula("2X");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorOnlyOperator()
        {
            Formula f = new Formula("+");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorExtraParentheses()
        {
            Formula f = new Formula("(2X) + 1)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorUndefinedVariable()
        {
            Formula f = new Formula("5X+1");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorMultiplyNoOperators()
        {
            Formula f = new Formula("1 9");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorBadTokenMiddle()
        {
            Formula f = new Formula("5 + & + 5");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorIncorrectPreviousTokenOperator()
        {
            Formula f = new Formula("(5 + )");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorIncorrectPreviousToken()
        {
            Formula f = new Formula("5 + 5 ( + 4");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorExtraNumber()
        {
            Formula f = new Formula("5 5 + 4");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorExteraOperator()
        {
            Formula f = new Formula("5 + + 4");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaConstructorOnlyOneParentheses()
        {
            Formula f = new Formula("5 + 4 + 5)");
        }

        [TestMethod()]
        public void TestEvaluateSimpleFormula()
        {
            Formula f = new Formula("5 + 5");
            Assert.AreEqual((double)f.Evaluate(s => 1), 10);
        }

        [TestMethod()]
        public void TestEvaluateSimpleFormulaWithParentheses()
        {
            Formula f = new Formula("(5) + (5)");
            Assert.AreEqual((double)f.Evaluate(s => 1), 10);
        }

        [TestMethod()]
        public void TestEvaluateSimpleFormulaWithParenthesesVariables()
        {
            Formula f = new Formula("(5) + (X5)");
            Assert.AreEqual((double)f.Evaluate(s => 1), 6);
        }

        [TestMethod()]
        public void TestEvaluateSimpleFormulaTwoVariables()
        {
            Formula f = new Formula("(X5) + (x5)");
            Assert.AreEqual((double)f.Evaluate(s => 3), 6);
        }

        [TestMethod()]
        public void TestEvaluateComplexVariable()
        {
            Formula f = new Formula("____XA1AA5 + 5");
            Assert.AreEqual((double)f.Evaluate(s => 1), 6);
        }

        [TestMethod()]
        public void TestEvaluateDivideByZero()
        {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 1), typeof(FormulaError));
        }

        [TestMethod()]
        public void TestEvaluateDivideByZeroVariable()
        {
            Formula f = new Formula("5/_1");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod()]
        public void TestEvaluateVerySmallRange()
        {
            Formula f = new Formula("5.6-3.6");
            Assert.AreEqual((double)f.Evaluate(s => 2), 2.0, 1e-9);
        }

        [TestMethod()]
        public void TestEvaluateComplicatedFormula()
        {
            Formula f = new Formula("(5+1)/X1 + 1 * (8-J) - 1");
            Assert.AreEqual((double)f.Evaluate(s => 2), 8.0);
        }

        [TestMethod()]
        public void TestGetVariablesNoVariables()
        {
            Formula f = new Formula("5 + 5");
            List<string> variables = new List<string>(f.GetVariables());
            Assert.IsTrue(variables.Count == 0);
        }

        [TestMethod()]
        public void TestGetVariablesTwoVariables()
        {
            Formula f = new Formula("X1 + X2");
            List<string> variables = new List<string>(f.GetVariables());
            Assert.AreEqual(variables[0], "X1");
            Assert.AreEqual(variables[1], "X2");
        }

        [TestMethod()]
        public void TestToString()
        {
            Formula f1 = new Formula("X1 + X2");
            string f1String = f1.ToString();
            Assert.IsTrue(f1String == "X1+X2");
        }

        [TestMethod()]
        public void TestEqualsMethodNoSpaces()
        {
            Formula f1 = new Formula("_1+_2");
            Formula f2 = new Formula("_1+_2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void TestEqualsMethodOneWithSpaces()
        {
            Formula f1 = new Formula("_1  +  _2");
            Formula f2 = new Formula("_1+_2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void TestEqualsMethodBothSpaces()
        {
            Formula f1 = new Formula("                    _1+ _2");
            Formula f2 = new Formula(" _1 +                            _2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void TestEqualsMethodDifferentDoubles()
        {
            Formula f1 = new Formula("5.000+_1");
            Formula f2 = new Formula("5.00000000000+_1");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void TestEqualsMethodOneNullValue()
        {
            Formula f1 = new Formula("5.000+_1");
            Assert.IsFalse(f1.Equals(null));
        }

        [TestMethod()]
        public void TestEqualEqualsOperator()
        {
            Formula f1 = new Formula("5.000+_1");
            Formula f2 = new Formula("5.000+_1");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod()]
        public void TestEqualEqualsOperatorOneWithSpaces()
        {
            Formula f1 = new Formula("5.000            +                  _1                ");
            Formula f2 = new Formula("5.000+_1");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod()]
        public void TestEqualEqualsOperatorTwoWithSpaces()
        {
            Formula f1 = new Formula("5.000            +                  _1                ");
            Formula f2 = new Formula("                  5.000  +  _1");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod()]
        public void TestEqualEqualsOperatorNotEqualFormula()
        {
            Formula f1 = new Formula("X1 + X2");
            Formula f2 = new Formula("X2 + X1");
            Assert.IsFalse(f1 == f2);
        }

        [TestMethod()]
        public void TestEqualEqualsOperatorOneNull()
        {
            Formula f1 = new Formula("X1 + X2");
            Assert.IsFalse(f1 == null);
        }

        [TestMethod()]
        public void TestNotEqualsOperator()
        {
            Formula f1 = new Formula("5.000+_1");
            Formula f2 = new Formula("5.000+_1");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod()]
        public void TestNotEqualsOperatorOneWithSpaces()
        {
            Formula f1 = new Formula("5.000            +                  _1                ");
            Formula f2 = new Formula("5.000+_1");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod()]
        public void TestNotEqualsOperatorTwoWithSpaces()
        {
            Formula f1 = new Formula("5.000            +                  _1                ");
            Formula f2 = new Formula("                  5.000  +  _1");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod()]
        public void TestNotEqualsOperatorNotEqualFormula()
        {
            Formula f1 = new Formula("X1 + X2");
            Formula f2 = new Formula("X2 + X1");
            Assert.IsTrue(f1 != f2);
        }

        [TestMethod()]
        public void TestNotEqualsOperatorOneNull()
        {
            Formula f1 = new Formula("X1 + X2");
            Assert.IsTrue(f1 != null);
        }

        [TestMethod()]
        public void TestGetHashCodeEquals()
        {
            Formula f1 = new Formula("X1 + X2");
            Formula f2 = new Formula("X1 + X2");
            int f1HashCode = f1.GetHashCode();
            int f2HashCode = f2.GetHashCode();
            Assert.IsTrue(f1HashCode == f2HashCode);
        }

        [TestMethod()]
        public void TestGetHashCodeNotEquals()
        {
            Formula f1 = new Formula("X1 + X2");
            Formula f2 = new Formula("X1 + X2");
            int f1HashCode = f1.GetHashCode();
            int f2HashCode = f2.GetHashCode();
            Assert.IsTrue(f1HashCode == f2HashCode);
        }
    }
}
