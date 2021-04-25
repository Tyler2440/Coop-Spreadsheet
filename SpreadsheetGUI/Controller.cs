using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetUtilities;
using System.Drawing;
using SS;

namespace SpreadsheetGUI
{
    class Controller
    {
        /// Spreadsheet object, handles internal logic
        Spreadsheet spreadsheet;

        Dictionary<int, User> users;

        HashSet<Color> colorsInUse;
        
        /// <summary>
        /// Creates a new Controller object that instantiates a Spreadsheet object with default parameters, and the version "ps6"
        /// </summary>
        public Controller()
        {
            spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "ps6");
            users = new Dictionary<int, User>();
            colorsInUse = new HashSet<Color>();
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
            return ((char)(col + 65)).ToString() + (row+1).ToString();
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
            
            return spreadsheet.SetContentsOfCell(name, text).ToList();
        }

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
        /// Gets all non-empty cells inside of the spreadsheet.
        /// </summary>
        public List<string> GetNonEmptyCells()
        {
            return spreadsheet.GetNamesOfAllNonemptyCells().ToList();
        }

        /// <summary>
        /// Updates model when other user selects new cell 
        /// </summary>
        /// <param name="ID">ID of user selecting new cell </param>
        /// <param name="username">username of user selecting new cell</param>
        /// <param name="cellName">selected cell name</param>
        /// <returns></returns>
        public Dictionary<int, User> UpdateUserCellSelection(int ID, string username, string cellName)
        {
            int col;
            int row;

            GetColRow(cellName, out col, out row);

            if (users.ContainsKey(ID))
            {
                users[ID].setCol(col);
                users[ID].setRow(row);
            }
            else
            {
                Color newColor = GetNewUserColor();
                colorsInUse.Add(newColor);
                users.Add(ID, new User(ID, username, col, row, newColor));
            }

            return users;
        }

        /// <summary>
        /// Helper method to find unused color for new user
        /// </summary>
        /// <returns>color</returns>
        private Color GetNewUserColor()
        {
            while (true)
            {
                Random rand = new Random();
                int r = rand.Next(256);
                int g = rand.Next(256);
                int b = rand.Next(256);
                Color col = Color.FromArgb(r, g, b);

                if(!colorsInUse.Contains(col))
                {
                    return col;
                }
            }
        }

        /// <summary>
        /// Removes connected user from model when they disconnect
        /// </summary>
        /// <param name="ID">ID of user that disconnected</param>
        public void RemoveUser(int ID)
        {
            users.Remove(ID);
        }
    }
}
