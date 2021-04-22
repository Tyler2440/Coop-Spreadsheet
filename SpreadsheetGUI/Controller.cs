using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetGUI
{
    class Controller
    {
        /// Stack to keep track of changes 
        Stack<string> changes;

        /// Spreadsheet object, handles internal logic
        Spreadsheet spreadsheet;

        /// Tracks whether the last change to spreadsheet was an "undo"function
        bool undone;
        
        /// <summary>
        /// Creates a new Controller object that instantiates a Spreadsheet object with default parameters, and the version "ps6"
        /// </summary>
        public Controller()
        {
            spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "ps6");
            changes = new Stack<string>();
        }

        /// <summary>
        /// Creates a new Controller object that instantiates a Spreadsheet object with the given file
        /// and default parameters, with the version "ps6"
        /// </summary>
        public Controller(string file)
        {
            spreadsheet = new Spreadsheet(file, s => true, s => s.ToUpper(), "ps6");
            changes = new Stack<string>();
        }

        public void SetSpreadsheet(Spreadsheet spreadsheet)
        {
            this.spreadsheet = spreadsheet;
        }

        /// <summary>
        /// Returns the cell's content given by its position in the spreadsheet, col/row.
        /// </summary>
        /// <param name="col">The column the cell is in</param>
        /// <param name="row">The row the cell is in</param>
        /// <returns>The cell's content</returns>
        public string GetCellContent(int col, int row)
        {
            string cell = GetName(col, row);
            if(spreadsheet.GetCellContents(cell).GetType() == typeof(Formula))
                return "=" + spreadsheet.GetCellContents(GetName(col, row)).ToString();
            return spreadsheet.GetCellContents(GetName(col, row)).ToString();
        }

        /// <summary>
        /// Returns the cell's value given by its position in the spreadsheet, col/row.
        /// </summary>
        /// <param name="col">The column the cell is in</param>
        /// <param name="row">The row the cell is in</param>
        /// <returns>The cell's value</returns>
        public string GetCellValue(int col, int row)
        {
            return spreadsheet.GetCellValue(GetName(col, row)).ToString();
        }

        /// <summary>
        /// Returns the cell's name in form of letter followed by a number.
        /// </summary>
        /// <param name="col">The column the cell is in</param>
        /// <param name="row">The row the cell is in</param>
        /// <returns>The cell's name in letter followed by number format</returns>
        public string GetName(int col, int row)
        {
            // Translates the col, given as an int, into a letter value via the ASCII table. Adds one to row to
            // display the correct row (accounts for SpreadsheetPanel's row array starting at 0).
            return ((char)(col + 65)).ToString().ToLower() + (row+1).ToString();
        }

        /// <summary>
        /// Sets the cell's content to the given text.
        /// </summary>
        /// <param name="col">The column the cell is in</param>
        /// <param name="row">The row the cell is in</param>
        /// <param name="text">The content to replace the cell's content</param>
        /// <returns>The list of all cells that directly/indirectly depend on this cell</returns>
        public List<string> SetCellContent(int col, int row, string text)
        {
            // Get cell name from col and row 
            string name = GetName(col, row);

            // If we are not undoing the previous change, pushes all changes this edit does onto the changes stack
            if (!undone)
            {
                // Pushes cell content then cell row and col onto changes stack 
                changes.Push(GetCellContent(col, row));
                changes.Push(row.ToString());
                changes.Push(col.ToString());
            }

            // Otherwise, set undone to false
            else
                undone = false;
            
            return spreadsheet.SetContentsOfCell(name, text).ToList();
        }

        //public string GetUserSelection(int ID, out string name)
        //{
        //    return spreadsheet.GetUserSelection(ID, out name);
        //}

        /// <summary>
        /// Converts the numerical values of the cell's position in the spreadsheet. For example,
        /// converts from the coordinates (3, 3) to E4
        /// </summary>
        /// <param name="cell">Cell inside spreadsheet</param>
        /// <param name="col">The column the cell is in</param>
        /// <param name="row">The row the cell is in</param>
        public void GetColRow(string cell, out int col, out int row)
        {
            col = cell[0] - 65;
            row = int.Parse(cell.Substring(1, cell.Length-1))-1;
        }

        /// <summary>
        /// Saves the spreadsheet.
        /// </summary>
        /// <param name="fileName">Name to call the saved file</param>
        public void Save(string fileName)
        {
            spreadsheet.Save(fileName);
        }

        /// <summary>
        /// Gets all non-empty cells inside of the spreadsheet.
        /// </summary>
        public List<string> GetNonEmptyCells()
        {
            return spreadsheet.GetNamesOfAllNonemptyCells().ToList();
        }

        //public List<int> GetUsers()
        //{
        //    return spreadsheet.GetUsers();
        //}

        /// <summary>
        /// Returns whether the spreadsheet has been changed, True if it has, and false otherwise.
        /// </summary>
        public bool Changed()
        {
            return spreadsheet.Changed;
        }

        /// <summary>
        /// Set col, row, and content to match that of the last change made on the spreadsheet.
        /// Undoes the last change, then returns the cells that need to be updated. If there were no changes, 
        /// returns empty list. 
        /// </summary>
        public List<string> Undo(out int col, out int row, out string content)
        {
            // If there has been any changes, take the col/row and content of where the change
            // needs to be done and set undone to true.
            if (changes.Count > 0)
            {
                col = int.Parse(changes.Pop());
                row = int.Parse(changes.Pop());
                content = changes.Pop();
                undone = true;
            }

            // Otherwise, return an empty list, as there are no changes to undo.
            else
            {
                col = 0;
                row = 0;
                content = "";
                return new List<string>();
            }

            // Returns the list of cells that need to be updated, and update the cell at the col/row with the
            // cell's previous content
            return SetCellContent(col, row, content).ToList();
        }
    }
}
