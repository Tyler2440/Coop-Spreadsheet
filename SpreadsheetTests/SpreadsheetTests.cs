using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadSheetTests
    {
        [TestMethod]
        public void TestGetNamesOfAllNonemptyCellsSize()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell("B1", "A1");
            spreadsheet.SetContentsOfCell("C1", "B1");
            spreadsheet.SetContentsOfCell("B1", "2.0");
            Assert.IsTrue(spreadsheet.GetNamesOfAllNonemptyCells().ToList().Count == 3);
        }

        [TestMethod]
        public void TestSetCellContentsString()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(spreadsheet.GetCellContents("B1").ToString() == "hello");
        }

        [TestMethod]
        public void TestSetCellContentsDouble()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(spreadsheet.GetCellContents("A1").Equals(2.1));
        }

        [TestMethod]
        public void TestSetCellContentsFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            Formula formula = new Formula("C1+D1");
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell("B1", formula.ToString());
            Assert.IsTrue(spreadsheet.GetCellContents("B1").ToString().Equals(formula.ToString()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentsNullFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell("B1", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsNullName()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            string s = null;
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.GetCellContents(s);
        }

        [TestMethod]
        public void TestGetNamesOfAllNonemptyCellsCorrectNames()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell("B1", "A1");
            spreadsheet.SetContentsOfCell("C1", "B1");
            IEnumerator<string> cellNames = spreadsheet.GetNamesOfAllNonemptyCells().GetEnumerator();
            cellNames.MoveNext();
            Assert.AreEqual("A1", cellNames.Current);
            cellNames.MoveNext();
            Assert.AreEqual("B1", cellNames.Current);
            cellNames.MoveNext();
            Assert.AreEqual("C1", cellNames.Current);
        }

        [TestMethod]
        public void TestGetNamesOfAllNonemptyCellsDirectDependents()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell("B1", "A1*2");
            List<string> contents = new List<string>(spreadsheet.SetContentsOfCell("C1", "B1+A1"));
            Assert.IsTrue(contents.Count == 1);
            Assert.IsTrue(contents.Contains("C1"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetNamesOfAllNonemptyCellsNullText()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell("B1", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetNamesOfAllNonemptyCellsNullDouble()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            string s = null;
            spreadsheet.SetContentsOfCell(s, "2.1");
            spreadsheet.SetContentsOfCell("B1", "A1");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetNamesOfAllNonemptyCellsNullName()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            string s = null;
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell(s, "A1");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsFormulaInvalidName()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            Formula formula = new Formula("C1+D1");
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell("11", "=" + formula.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsFormulaNullName()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            string s = null;
            Formula formula = new Formula("C1+D1");
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell(s, "=" + formula.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestGetCellContentsFormulaDirectCircularException()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            Formula formula = new Formula("A1+B1");
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell("B1", "=" + formula.ToString());
            spreadsheet.SetContentsOfCell("A1", "B1");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestGetCellContentsFormulaInDirectCircularException()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            Formula formula1 = new Formula("B1");
            Formula formula2 = new Formula("A1+C1");
            spreadsheet.SetContentsOfCell("A1", "=" + formula1.ToString());
            spreadsheet.SetContentsOfCell("B1", "=" + formula2.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestGetCellContentsFormulaHasDependeesIndirectCircularException()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            Formula formula1 = new Formula("B1");
            Formula formula2 = new Formula("C1");
            Formula formula3 = new Formula("A1+D1");
            spreadsheet.SetContentsOfCell("A1", "=" + formula1.ToString());
            spreadsheet.SetContentsOfCell("B1", "=" + formula2.ToString());
            spreadsheet.SetContentsOfCell("B1", "=" + formula3.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestGetCellContentsFormulaHasDependeesDirectCircularException()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            Formula formula = new Formula("A1+D1");
            Formula formula2 = new Formula("B1");
            spreadsheet.SetContentsOfCell("A1", "2.1");
            spreadsheet.SetContentsOfCell("B1", "A1");
            spreadsheet.SetContentsOfCell("B1", "=" + formula.ToString());
            spreadsheet.SetContentsOfCell("A1", "=" + formula2.ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNullGetCellContents()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidGetCellContents()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.GetCellContents("1AA1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidNameGetCellContents()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.GetCellContents(null);
        }

        [TestMethod()]
        public void TestInvalidCellNameGetCellContents()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            Assert.AreEqual("", spreadsheet.GetCellContents("AA1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellNullName()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell(null, "0");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellInvalidCellName()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("1AA1", "0");
        }

        [TestMethod()]
        public void TestSimpleGetCellContentsForDouble()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", "1.5");
            Assert.AreEqual(1.5, (double)spreadsheet.GetCellContents("AA1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSimpleSetContentsofCellNullString()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSimpleSetContentsofCellInvalidCellName()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("!AA1", "yo");
        }

        [TestMethod()]
        public void TestSimpleGetCellContentsForString()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", "yo");
            Assert.AreEqual("yo", spreadsheet.GetCellContents("AA1").ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetContentsOfCellNullFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellFormulaInvalidCellName()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("1AA1", "=" + new Formula("1.5").ToString());
        }

        [TestMethod()]
        public void TestSimpleGetCellContentsForFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", "=" + new Formula("1.5").ToString());
            Assert.AreEqual(1.5, double.Parse(spreadsheet.GetCellContents("AA1").ToString()));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSimpleSetContentsofCellNullCellName()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell(null, "=" + new Formula("1.5").ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSimpleSetContentsofCellInvalidCellNameFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("1AA1", "=" + new Formula("1.5").ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSimpleSetContentsofCellAssertEqualsFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("1AA1", "=" + new Formula("1.5").ToString());
        }

        [TestMethod()]
        public void TestGetNamesOfAllNonEmptyCellsEmptySpreadsheet()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            Assert.IsFalse(spreadsheet.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void TestGetNamesOfAllNonEmptyCellsEmptyCell()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", "");
            Assert.IsFalse(spreadsheet.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void TestGetNamesOfAllNonEmptyCellsSimpleGetSingleCell()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", "yo");
            Assert.IsTrue(spreadsheet.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext(), "AA1");
        }

        [TestMethod()]
        public void TestGetNamesOfAllNonEmptyCellsSimpleGetMultipleCells()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", "yo");
            spreadsheet.SetContentsOfCell("AB1", "=" + new Formula("100").ToString());
            spreadsheet.SetContentsOfCell("AC1", "10");
            Assert.IsTrue(spreadsheet.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext(), "AA1");
            Assert.IsTrue(spreadsheet.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext(), "AB1");
            Assert.IsTrue(spreadsheet.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext(), "AC1");
        }

        [TestMethod()]
        public void TestSetSingletonDouble()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", "yo");
            spreadsheet.SetContentsOfCell("AB1", "=" + new Formula("10").ToString());
            Assert.IsTrue(spreadsheet.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext(), "AC1");
        }

        [TestMethod()]
        public void TestSetContentsOfCellChangeCellContentDouble()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "=" + new Formula("A2+A3").ToString());
            spreadsheet.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)spreadsheet.GetCellContents("A1"));
        }

        [TestMethod()]
        public void TestSetContentsOfCellChangeCellContentString()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "=" + new Formula("A2+A3").ToString());
            spreadsheet.SetContentsOfCell("A1", "yo");
            Assert.AreEqual("yo", spreadsheet.GetCellContents("A1"));
        }

        [TestMethod()]
        public void TestSetContentsOfCellChangeCellContentFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "=" + new Formula("A2+A3").ToString());
            spreadsheet.SetContentsOfCell("A1", "=" + new Formula("A4+A5").ToString());
            Assert.AreEqual("A4+A5", spreadsheet.GetCellContents("A1").ToString());
        }

        [TestMethod()]
        public void TestGetCellValueSimpleFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet(spreadsheet => true, spreadsheet => "3.0", "default");
            spreadsheet.SetContentsOfCell("AA1", "=" + new Formula("AA2+AA3").ToString());
            Assert.AreEqual(6.0, spreadsheet.GetCellValue("AA1"));
        }

        [TestMethod()]
        public void TestGetCellValueComplexFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "=" + new Formula("5").ToString());
            spreadsheet.SetContentsOfCell("A2", "=" + new Formula("10+A1").ToString());
            spreadsheet.SetContentsOfCell("A3", "=" + new Formula("A2+A1").ToString());
            spreadsheet.SetContentsOfCell("A4", "=" + new Formula("A2+A3").ToString());
            spreadsheet.SetContentsOfCell("A5", "=" + new Formula("A4+A3").ToString());
            Assert.AreEqual(55.0, spreadsheet.GetCellValue("A5"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValueInvalidName()
        {
            Spreadsheet spreadsheet = new Spreadsheet(spreadsheet => true, spreadsheet => "3.0", "default");
            spreadsheet.SetContentsOfCell("AA1", "=" + new Formula("AA2+AA3").ToString());
            spreadsheet.GetCellValue("1AA1");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetContentsOfCellInvalidString()
        {
            Spreadsheet spreadsheet = new Spreadsheet(spreadsheet => true, spreadsheet => "3.0", "default");
            spreadsheet.SetContentsOfCell("AA1", (string)null);
        }

        [TestMethod(), Timeout(5000)]
        public void StressTestSetContentsOfCellFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 500; i++)
            {
                cells.Add("A" + i);
                Assert.IsTrue(cells.SetEquals(spreadsheet.SetContentsOfCell("A" + i, "=" + new Formula("A" + (i + 1)).ToString())));
            }
        }

        [TestMethod(), Timeout(2000)]
        public void StressTestSetContentsOfCellString()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            for (int i = 1; i < 500; i++)
            {
                Assert.IsTrue(spreadsheet.SetContentsOfCell("A" + i, "yooo").ElementAt(0) == ("A" + i));
            }
        }

        [TestMethod(), Timeout(2000)]
        public void StressTestSetContentsOfCellDouble()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            for (int i = 1; i < 500; i++)
            {
                Assert.IsTrue(spreadsheet.SetContentsOfCell("A" + i, "3.14152598").ElementAt(0) == ("A" + i));
            }
        }

        [TestMethod()]
        public void TestCreateSpreadsheetFromPremadeFile()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A2");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Spreadsheet spreadsheet = new Spreadsheet("save.txt", s => true, s => s, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestCreateSpreadsheetFromPremadeFileNonMatchingVersion()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A2");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Spreadsheet spreadsheet = new Spreadsheet("save.txt", s => true, s => s, "1.0");
        }

        [TestMethod()]
        public void TestCreateSpreadsheetFromPremadeFileGetNamesOfAllNonEmptyCells()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A2");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Spreadsheet spreadsheet = new Spreadsheet("save.txt", s => true, s => s, "default");
            List<string> cells = new List<string> { "A1", "A2" };
            Assert.IsTrue(spreadsheet.GetNamesOfAllNonemptyCells().All(cells.Contains));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestCreateSpreadsheetFromPremadeFileInvalidPath()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A2");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Spreadsheet spreadsheet = new Spreadsheet("1save.txt", s => true, s => s, "default");
        }

        [TestMethod()]
        public void TestCreateSpreadsheetFromPremadeFileGetCellContents()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A2");
                    writer.WriteElementString("contents", "=A3/1 + (2*A3)");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A3");
                    writer.WriteElementString("contents", "2");

                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Spreadsheet spreadsheet = new Spreadsheet("save.txt", s => true, s => 2.0.ToString(), "default");
            Assert.IsTrue(spreadsheet.GetCellContents("A1").ToString() == "hello");
            Assert.IsTrue(double.Parse(spreadsheet.GetCellValue("A2").ToString()) == 6.0);
        }

        [TestMethod()]
        public void TestCreateSpreadsheetFromPremadeFileGetCellValueWithFormulas()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "=A3-A4");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A2");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Spreadsheet spreadsheet = new Spreadsheet("save.txt", s => true, s => "5.0", "default");
            Assert.IsTrue(double.Parse(spreadsheet.GetCellValue("A1").ToString()) == 0.0);
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestCreateSpreadsheetFromPremadeFileNullFileName()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A2");
                    writer.WriteElementString("contents", "hello");

                    writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Spreadsheet spreadsheet = new Spreadsheet(null, s => true, s => s, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestCreateSpreadsheetFromPremadeFileInvalidXMLFile()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
            }
            Spreadsheet spreadsheet = new Spreadsheet("save.txt", s => true, s => s, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestCreateSpreadsheetFromPremadeFileInvalidXMLFileName()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
            }
            Spreadsheet spreadsheet = new Spreadsheet("/some/nonsense/path.xml", s => true, s => s, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestCreateSpreadsheetFromPremadeFileInvalidXMLFilePath()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("contents", "hello");

                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Spreadsheet spreadsheet = new Spreadsheet("/some/nonsense/path.xml", s => true, s => s, "default");
        }

        [TestMethod()]
        public void TestSaveMethod()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", "yo");
            spreadsheet.SetContentsOfCell("AB1", "2.5");
            spreadsheet.Save("save.txt");
            Spreadsheet newSpreadsheet = new Spreadsheet("save.txt", s => true, s => s, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSaveMethodNullFilePath()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.Save(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSaveMethodXMLException()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.Save("/some/nonsense/path.xml");
        }

        [TestMethod()]
        public void TestSaveMethodGetNamesOfAllNonEmptyCells()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", "yo");
            spreadsheet.SetContentsOfCell("AB1", "2.5");
            spreadsheet.Save("save.txt");
            Spreadsheet newSpreadsheet = new Spreadsheet("save.txt", s => true, s => s, "default");
            List<string> cells = new List<string> { "AA1", "AB1" };
            Assert.IsTrue(newSpreadsheet.GetNamesOfAllNonemptyCells().All(cells.Contains));
        }

        [TestMethod()]
        public void TestSaveMethodGetCellContents()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("AA1", "yo");
            spreadsheet.SetContentsOfCell("AB1", "2.5");
            spreadsheet.SetContentsOfCell("AC1", "=" + new Formula("50 + 5").ToString());
            spreadsheet.Save("save.txt");
            Spreadsheet newSpreadsheet = new Spreadsheet("save.txt", s => true, s => s, "default");
            Assert.IsTrue(newSpreadsheet.GetCellContents("AA1").ToString() == "yo");
            Assert.IsTrue(double.Parse(newSpreadsheet.GetCellContents("AB1").ToString()) == 2.5);
            Assert.IsTrue(newSpreadsheet.GetCellContents("AC1").ToString() == new Formula("50 + 5").ToString());
        }
    }
}