using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace SS
{
    /// <summary>
    /// File name: Program.cs
    /// Author: Tyler Allen
    /// Created on: 9/19/20
    /// Description: Spreadsheet is a class that hold the core functionality of a spreadsheet, like excel.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// Holds all of the cells contained in the spreadsheet
        private Dictionary<string, Cell> cells;

        /// Holds the dependencygraph held in the spreadsheet
        private DependencyGraph graph;

        private HashSet<User> users;

        /// Tracks whether the spreadsheet has been changed since the last save
        private bool changed;

        /// <summary>
        /// Spreadsheet constructor with no parameters. Initializes a new spreadsheet with the isValid always being
        /// true, the normalizer setting the string to itself, and default version.
        /// </summary>
        public Spreadsheet() :
            base(s => true, s => s, "default")
        {
            // Instantiate the spreadsheet's cells/changed/graph
            cells = new Dictionary<string, Cell>();
            changed = false;
            graph = new DependencyGraph();
            users = new HashSet<User>();
        }

        /// <summary>
        /// Spreadsheet constructor with isValid, normalize, and version parameters. Initializes a new spreadsheet
        /// with the given isValid/normalize delegates, and the given version.
        /// </summary>
        /// <param name="isValid"> Given validation delegate to check if variable is valid. </param>
        /// <param name="normalize"> Given normalize delegate that normalizes a given variable. </param>
        /// <param name="version"> Given version of the spreadsheet. </param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) :
            base(isValid, normalize, version)
        {
            // Instantiate the spreadsheet's cells/changed/graph
            cells = new Dictionary<string, Cell>();
            changed = false;
            graph = new DependencyGraph();
            users = new HashSet<User>();
        }

        /// <summary>
        /// Spreadsheet constructor initialized from an already existing XML file. Initializes a spreadsheet
        /// from the given XML file given by filePath, then calls the three parameter spreadsheet with the isValid,
        /// normalize, and version parameters.
        /// </summary>
        /// <param name="filePath"> XML file to initialize this spreadsheet from </param>
        /// <param name="isValid"> Given validation delegate to check if variable is valid. </param>
        /// <param name="normalize"> Given normalize delegate that normalizes a given variable. </param>
        /// <param name="version"> Given version of the spreadsheet. </param>
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) :
            base(isValid, normalize, version)
        {
            // If the file does not exist, then throw a SpreadsheetReadWriteException 
            if (!File.Exists(filePath))
                throw new SpreadsheetReadWriteException("File does not exist!");

            // If the file's version does not match the provided version, then throw a SpreadsheetReadWriteException 
            if (GetSavedVersion(filePath) != version)
                throw new SpreadsheetReadWriteException("File versions do not match!");

            // Instantiate the spreadsheet's cells/changed/graph
            cells = new Dictionary<string, Cell>();
            changed = false;
            graph = new DependencyGraph();

            // Read through entire Xml document for any pending issues. This method will also initialize/set any cell contents
            ReadXml(filePath, false);
            changed = false;
        }

        // Changed keeps track if the spreadsheet was changed after saving/instantiating. Used the variable changed to help
        // keep update/get Changed.
        public override bool Changed { get => changed; protected set => changed = value; }

        //public string GetUserSelection(int ID, out string name)
        //{
        //    return users.ElementAt(ID).getSelected(name);
        //}

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            // Creates a list of the non empty cells that the method will return
            List<string> nonEmptyCells = new List<string>();

            // For each key value pair in cells, it checks if the cell is empty or null.
            // If the cell is not empty or null, add it to nonEmptyCells list
            foreach (string cell in cells.Keys)
            {
                if (!string.IsNullOrEmpty(cells[cell].GetContent().ToString()))
                    nonEmptyCells.Add(cell);
            }

            return nonEmptyCells;
        }

        public List<int> GetUsers()
        {
            List<int> list = new List<int>();
            foreach (User user in users)
            {
                list.Add(user.getID());
            }
            return list;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            // If name is null or invalid, throw an InvalidNameException.
            if (ReferenceEquals(name, null) || !IsValidName(name))
                throw new InvalidNameException();

            name = Normalize(name);

            // If cells does not contain name, return an empty string.
            if (!cells.ContainsKey(name))
                return "";

            // Holds the content of the cell 
            object cellContent = cells[name].GetContent();           

            // Each if statement checks if the cell's content is a string, double, or Formula, then returns it
            if (cellContent.GetType() == typeof(string))
                return cellContent.ToString();
            else if (cellContent.GetType() == typeof(double))
                return double.Parse(cellContent.ToString());
            else if (cellContent.GetType() == typeof(Formula))
                return new Formula(cellContent.ToString());

            return "";
        }


        public void SetSelected(string cell, int ID, string name)
        {
            User user = new User(ID, name, cell);
            if (users.Contains(user))
            {
                user.setSelected(cell);
            }
            else
            {
                users.Add(user);
                user.setSelected(cell);
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            name = Normalize(name);

            // If cells did not previously contain this cell, add it to cells
            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell(number, null, null));

            // Sets the content of this cell to the number, and just in case the content was
            // previously a formula, remove all dependents of this cell
            else
                cells[name].SetContent(number);

            cells[name].SetValue(number);
            graph.ReplaceDependees(name, new HashSet<string>());

            // Returns the list of all cells whose value dependes on this cell
            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            name = Normalize(name);

            // If cells did not previously contain this cell, add it to cells
            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell(text, null, null));

            // Sets the content of this cell to the number, and just in case the content was
            // previously a formula, remove all dependees of this cell
            else
                cells[name].SetContent(text);

            // Sets the value of the cell to the string
            cells[name].SetValue(text);

            // Replaces the dependees of this cell to an empty set, as it has no dependees anymore
            graph.ReplaceDependees(name, new HashSet<string>());

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            name = Normalize(name);

            // In case of a circular dependency, we have to keep track of this cell's previous dependents
            IEnumerable<string> oldDependees = graph.GetDependees(name);

            // Replaces all dependents of the given cell with the variabes in the formula
            graph.ReplaceDependees(name, formula.GetVariables());

            // The try catch helps catch the potential circular dependency thrown by GetCellsToRecalculate()
            try
            {
                List<string> recalculateCells = new List<string>(GetCellsToRecalculate(name));
                
                // If GetCellsToRecalculate does not throw a CircularException, set the content of this cell to formula
                // and return recalculateCells
                
                // If cells does not contain name, create a new cell that contains the formula. Otherwise, overwrite its content to formula
                if (!cells.ContainsKey(name))
                    cells.Add(name, new Cell(formula, null, VariableLookup));
                else
                    cells[name].SetContent(formula);

                return recalculateCells;
            }

            // If GetCellsToRecalculate does throw a CircularException, replaces the dependees of the cell
            // and throws the CircularException.
            catch(CircularException)
            {
                graph.ReplaceDependees(name, oldDependees);
                throw new CircularException();
            }
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            name = Normalize(name);

            // If cells contains this cell, returns the dependents of this cell. 
            // Otherwise, return an empty list
            if (cells.ContainsKey(name))
                return graph.GetDependents(name);
            return new List<string>();
        }

        // ADDED FOR PS5
        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            // Calls on ReadXml, a helper method that will provide the version from the file
            return ReadXml(filename, true);
        }

        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            // Took basic comments and skeleton from XMLDemo from Examples Repository.

            // If the filename is empty or null, throw a SpreadsheetReadWriteException
            if (ReferenceEquals(filename, null) || filename == "")
                throw new SpreadsheetReadWriteException("File name is null or empty!");
          
            // We want some non-default settings for our XML writer.
            // Specifically, use indentation to make it more readable.
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            try
            {
                // Create an XmlWriter inside this block, and automatically Dispose() it at the end.
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    // Starts the document
                    writer.WriteStartDocument();

                    // Starts the outer-most element, spreadsheet
                    writer.WriteStartElement("spreadsheet");

                    // This adds an attribute to the spreadsheet element
                    writer.WriteAttributeString("version", Version);

                    // Write the cells one-by-one until there are no more non-empty cells
                    foreach (string cell in GetNamesOfAllNonemptyCells())
                    {
                        // Starts the cell element
                        writer.WriteStartElement("cell");

                        // Starts the name element, normalized according to the Normalize function
                        writer.WriteElementString("name", Normalize(cell));

                        // The following if statements write the contents of this cell according to it's respective content type
                        if (cells[cell].GetContent().GetType() == typeof(string))
                            writer.WriteElementString("contents", cells[cell].GetContent().ToString());
                        else if (cells[cell].GetContent().GetType() == typeof(double))
                            writer.WriteElementString("contents", double.Parse(cells[cell].GetContent().ToString()).ToString());
                        else if (cells[cell].GetContent().GetType() == typeof(Formula))
                            writer.WriteElementString("contents", "=" + cells[cell].GetContent().ToString());

                        // Ends the cell element
                        writer.WriteEndElement();
                    }

                    // Ends the outer-most element, spreadsheet
                    writer.WriteEndElement(); 

                    // Ends the document
                    writer.WriteEndDocument();

                    // At the end of saving, update spreadsheet to not been changed.
                    Changed = false;
                }
            }

            catch (XmlException e)
            {
                throw new SpreadsheetReadWriteException(e.ToString());
            }
            catch (IOException e)
            {
                throw new SpreadsheetReadWriteException(e.ToString());
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            // If the name is null or invalid, throw an InvalidNameException
            if (ReferenceEquals(name, null) || !IsValidName(name))
                throw new InvalidNameException();

            name = Normalize(name);

            // If the cell has a value and it isn't null, return the cells value
            if (cells.TryGetValue(name, out Cell cell) && !ReferenceEquals(cell.GetValue(), null))
                return cell.GetValue();

            return "";
        }

        // ADDED FOR PS5
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            // If content is null, throw an ArgumentNullException
            if (ReferenceEquals(content, null))
                throw new ArgumentNullException();

            // If name is null or invalid, throw an InvalidNameException
            if (ReferenceEquals(name, null) || !IsValidName(name))
                throw new InvalidNameException();

            name = Normalize(name);

            // Holds the results of GetCellsToRecalculate
            List<string> cellsToRecalculate = new List<string>();

            // If content is null or empty, immediately set the content of the cell to content
            if (string.IsNullOrEmpty(content))
            {
                SetCellContents(name, content);

                // Sets change to true, as the spreadsheet has been modified.
                Changed = true;
            }

            // If the content's type is double, set the content to the double.Parse result of content
            else if (double.TryParse(content, out double d))
            {
                SetCellContents(name, double.Parse(content));

                // Sets change to true, as the spreadsheet has been modified.
                Changed = true;
            }

            // If the content is a formula, creates a new formula from content to check for an invalid formula.
            // If no exception is thrown, set cellsToRecalculate to the result of SetCellContents of name and formula
            else if (content.Substring(0, 1).Equals("="))
            {
                Formula formula = new Formula(content.Substring(1, content.Length - 1), Normalize, IsValid);
                cellsToRecalculate = (List<string>)SetCellContents(name, formula);

                // Sets change to true, as the spreadsheet has been modified. Will only reach here if a circular dependency is not detected.
                Changed = true;
            }

            // If the content is not a double or a formula, it is a string, so set the cells content to that string.
            else
            {
                SetCellContents(name, content.ToString());
                Changed = true;
            }

            // Recalculates the values of each cell in cellsToRecalculate for further use
            foreach (string cell in GetCellsToRecalculate(name))
            {
                if(cells[cell].GetContent().GetType() == typeof(Formula))
                    cells[cell].SetFormulaValue(VariableLookup, Normalize, IsValid);
            }           

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// Reads the contents of an XML file represnting a spreadsheet.
        /// </summary>
        /// <param name="filename">The name of the file containing the XML to read.</param>
        /// <param name="version">The version of the spreadsheet the user wants.</param>
        /// <param name="getVersion">True/False if the user only wants the version of this spreadsheet.</param>
        private string ReadXml(string filename, bool getVersion)
        {
            // Took basic comments and skeleton from XMLDemo from Examples Repository.
            try
            {
                // Create an XmlReader inside this block, and automatically Dispose() it at the end.
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    // Initializes a name and content variable that holds the name/content of a cell
                    // to be used in SetContentsOfCell
                    string name = "";
                    string content = "";

                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                // In the case of reader.Name being "spreadsheet", if the getVersion bool is true, simply return the
                                // version and exit the method. Otherwise, set the Version to "version"
                                case "spreadsheet":
                                    if (getVersion)
                                        return reader.GetAttribute("version");
                                    else
                                        Version = reader.GetAttribute("version");
                                    break;

                                // In the case of reader.Name being "cell", do nothing
                                case "cell":
                                    break;

                                // In the case of reader.Name being "name", set name variable to the name
                                case "name":
                                    reader.Read();
                                    name = reader.Value;
                                    name = Normalize(name);
                                    break;

                                // In the case of reader.Name being "contents", set the content variable to content, and call
                                // SetContentsOfCell using name and content variables.
                                case "contents":
                                    reader.Read();
                                    content = reader.Value;
                                    SetContentsOfCell(name, content);
                                    break;
                            }
                        }
                    }
                }
            }

            catch (XmlException e)
            {
                throw new SpreadsheetReadWriteException(e.ToString());
            }
            catch (IOException e)
            {
                throw new SpreadsheetReadWriteException(e.ToString());
            }

            return "";
        }

        /// <summary>
        /// Returns whether the cell name is a valid way to represent a cell.
        /// </summary>
        /// <param name="name"> The given cell's name </param>
        /// <returns> True/False if the cell's name is valid. </returns>
        public bool IsValidName(string name)
        {
            // Checks if the name matches the name validation (any number of letters followed by any number of numbers),
            // and checks if IsValid returns true.
            return Regex.IsMatch(name, "^[A-Za-z]+[0-9]+$") && IsValid(name);
        }

        /// <summary>
        /// Returns the value of the given cell. This method is specifically used for the lookup
        /// delegate used for the Evaluate method. It calls GetCellValue to get the value of the cell, 
        /// and will only return a double if it is not a string/FormulaError
        /// </summary>
        /// <param name="cell"> Name of the cell whose value is returned. </param>
        /// <returns> The cell's value as a double. </returns>
        private double VariableLookup(string cell)
        { 
            // Holds the cell value
            object cellValue = GetCellValue(cell);

            // If the value of cellValue is a string or FormulaError, throw an ArgumentException. This method should
            // only be called on doubles.
            if (cellValue.GetType() == typeof(string) || cellValue.GetType() == typeof(FormulaError))
                throw new ArgumentException();

            return double.Parse(cellValue.ToString());
        }


        private class User
        {
            private int ID;
            private string name;
            private string cellSelected;

            public User (int ID, string name, string cell)
            {
                this.ID = ID;
                this.name = name;
                this.cellSelected = cell;
            }

            public int getID()
            {
                return ID;
            }

            public string getName()
            {
                return name;
            }

            // Returns which cell this user has selected
            public string getSelected(out string name)
            {
                name = this.name;
                return cellSelected;
            }

            public void setSelected(string cell)
            {
                this.cellSelected = cell;
            }

        }


        /// <summary>
        /// This Cell class holds the content, value, and DependencyGraph of each Cell in the Spreadsheet.
        /// </summary>
        private class Cell
        {
            private object content;
            private object value;

            /// <summary>
            /// Cell constructor that sets the content, value, and instantiates a DependencyGraph.
            /// </summary>
            public Cell(object content, object value, Func<string, double> lookup)
            {
                this.content = content;
                if (!ReferenceEquals(lookup, null))
                    this.value = ((Formula)content).Evaluate(lookup);
                this.value = value;
            }

            /// <summary>
            /// Returns the content of this cell.
            /// </summary>
            public object GetContent()
            {
                if (content.GetType() == typeof(Formula))
                    return (Formula)this.content;
                else if (content.GetType() == typeof(double))
                    return double.Parse(this.content.ToString());
                return this.content.ToString();
            }

            /// <summary>
            /// Returns the value of this cell.
            /// </summary>
            public object GetValue()
            {
                return this.value;
            }

            /// <summary>
            /// Sets the content of this cell to the content parameter.
            /// </summary>
            public void SetContent(object content)
            {
                this.content = content;
            }

            /// <summary>
            /// Sets the value of this cell to the value parameter.
            /// </summary>
            public void SetValue(object value)
            {
                this.value = value;
            }     
            
            /// <summary>
            /// Sets the cell's value to the double value of the Formula
            /// </summary>
            /// <param name="lookup"> Lookup delegate needed to be passed into Formula.Evaluate </param>
            public void SetFormulaValue(Func<string, double> lookup, Func<string, string> Normalize, Func<string, bool> IsValid)
            {
                Formula formula = new Formula(content.ToString(), Normalize, IsValid);
                value = formula.Evaluate(lookup);
            }
        }
    }
}