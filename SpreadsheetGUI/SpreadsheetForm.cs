using SS;
using SpreadsheetController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SpreadsheetGUI
{
    public partial class SpreadsheetForm : Form
    {
        // Creates a controller object to reference the Controller class, the brains of the spreadsheet GUI
        Controller controller;

        SpreadsheetController.Controller networkController = new SpreadsheetController.Controller();

        // Col/row hold the current cell's place in the spreadsheet
        int col, row;

        

        public SpreadsheetForm()
        {
            InitializeComponent();

            // Initializes the controller object
            controller = new Controller();


            // Adds two events to SelectionChanged: displaySelection and HighlightCellContentsOnClick. When a cell is
            // clicked on, these two events are run
            spreadsheetPanel1.SelectionChanged += displaySelection;
            spreadsheetPanel1.SelectionChanged += HighlightCellContentsOnClick;

            // Initially, highlight cell A1 and update CellNameText to display A1
            spreadsheetPanel1.SetSelection(0, 0);
            CellNameText.Text = "A1";

            networkController.FileSelect += JoinServer;
            networkController.Update += UpdateSpreadsheet;
        }



        private void JoinServer(List<string> spreadsheets, SocketState state)
        {
            FileSelector fileselector = new FileSelector(spreadsheets, state, networkController);
            fileselector.ShowDialog();
        }

        public SpreadsheetForm(string file)
        {
            InitializeComponent();

            // Initializes the controller object
            controller = new Controller();

            // Sets up the spreadsheet's cells from the file
            // SetupSpreadsheet();

            // Initially, fill in CellValueText and CellContentText respectively
            CellValueText.Text = controller.GetCellValue(0, 0);
            CellContentText.Text = controller.GetCellContent(0, 0);

            // Adds two events to SelectionChanged: displaySelection and HighlightCellContentsOnClick. When a cell is
            // clicked on, these two events are run
            spreadsheetPanel1.SelectionChanged += displaySelection;
            spreadsheetPanel1.SelectionChanged += HighlightCellContentsOnClick;

            // Initially, highlight cell A1 and update CellNameText to display A1
            spreadsheetPanel1.SetSelection(0, 0);
            CellNameText.Text = "A1";
        }

        /// <summary>
        /// After Controller reads through the file and sets up it's Spreadsheet object, this method runs 
        /// and updates the GUI to display each cell's content and value.
        /// </summary>
        private void SetupSpreadsheet()
        { 
            foreach (string cell in controller.GetNonEmptyCells())
            {
                UpdateSpreadsheetValue(cell);
            }
        }

        private void UpdateSpreadsheet(Spreadsheet spreadsheet)
        {          
            controller.SetSpreadsheet(spreadsheet); 
            foreach (string cell in controller.GetNonEmptyCells())
            {    
                UpdateSpreadsheetValue(cell);
            }
            Application.DoEvents();

            //foreach (int ID in controller.GetUsers())
            //{
            //    UpdateSpreadsheetUsers(ID);
            //}

        }

        /// <summary>
        /// Upates Spreadsheet Panel with user selected cells 
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="ID"></param>
        /// <param name="userName"></param>
        private void SetUserSelectedCell(String cellName, int ID, String userName)
        {
            int col;
            int row;
            controller.GetColRow(cellName, out col, out row);

            //update list of Users and their selections 
            //tell spreadsheet panel 

        }
        /// <summary>
        /// Helper method to update individual displayed cell value after change to spreadsheet
        /// </summary>
        private void UpdateSpreadsheetValue(string cell)
        {
            this.BeginInvoke((MethodInvoker)delegate ()
            {
                int colTemp = col;
                int rowTemp = row;
                controller.GetColRow(cell, out colTemp, out rowTemp);
                spreadsheetPanel1.SetValue(colTemp, rowTemp, controller.GetCellValue(colTemp, rowTemp)); //updates cell value displayed in cell
                displaySelection(spreadsheetPanel1); // updates all text boxes for current selection 
            });
        }

        //private void UpdateSpreadsheetUsers(int ID)
        //{
        //    string name;
        //    string cell = controller.GetUserSelection(ID, out name);
        //    int colTemp;
        //    int rowTemp;
        //    controller.GetColRow(cell, out colTemp, out rowTemp);
        //    spreadsheetPanel1.SetUserSelection(colTemp, rowTemp, ID, name);
        //}

        /// <summary>
        /// Every time the selection changes, this method is called with the
        /// Spreadsheet as its parameter.  The bottom panel text boxes and the inner cell displayed value 
        /// are updated to show data for selected cell. 
        /// </summary>
        /// <param name="sp"> The form's spreadsheet panel to update </param>
        private void displaySelection(SpreadsheetPanel sp)
        {
            int colTemp;
            int rowTemp;
            
            sp.GetSelection(out colTemp, out rowTemp); // Update col and row variables  to current selection 
            sp.SetSelection(colTemp, rowTemp);
            sp.SetValue(colTemp, rowTemp, controller.GetCellValue(colTemp, rowTemp)); //sets current selected cell's value
           
            this.BeginInvoke((MethodInvoker) delegate()
            {
                //set bottom panel text boxes
                CellNameText.Text = controller.GetName(colTemp, rowTemp);
                CellContentText.Text = controller.GetCellContent(colTemp, rowTemp);
                CellValueText.Text = controller.GetCellValue(colTemp, rowTemp);
                sp.Refresh();
                //this.Refresh();
            });             
        }

        /// <summary>
        /// Deals with the user wanting to update a cell. When the user presses enter with the CellContentText text box selected,
        /// it updates this and every cell that depends on this cell's content/value. It then updates the display to these new values.
        /// </summary>
        private void CellContentText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) 
            {
                try
                {
                    foreach (string cell in controller.SetCellContent(col, row, CellContentText.Text))
                    {
                        UpdateSpreadsheetValue(cell);
                    }
                    e.Handled = true;
                }
                catch(Exception f)
                {
                    MessageBox.Show(f.Message, "Formula at " + controller.GetName(col, row) + " is invalid!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }


        /// <summary>
        /// When the "New" button is clicked under the "File" tab, it runs a new Spreadsheet window.
        /// </summary>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetGUIContext.getAppContext().RunForm(new SpreadsheetForm());
        }

        /// <summary>
        /// When the "Close" button is clicked under the "File" tab, it should close the window.
        /// </summary>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// When the "Save" button is clicked under the "File" tab, open a windows explorer window to choose
        /// where the file should be saved, and what it should be called.
        /// </summary>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Spreadsheet files (*.sprd)|*.sprd";
            saveFileDialog1.FileName = "Spreadsheet";
            saveFileDialog1.Title = "Save a Spreadsheet File";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                controller.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// When the "Open" button is clicked via the "File" tab, opens a windows explorer window to let the user choose which file to 
        /// open.
        /// </summary>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.DefaultExt = "sprd";
            openFileDialog1.FileName = "Select a Spreadsheet File";
            openFileDialog1.Title = "Open a Spreadsheet File";
            openFileDialog1.Filter = "Spreadsheet files (*.sprd)|*.sprd|All Files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SpreadsheetGUIContext.getAppContext().RunForm(new SpreadsheetForm(openFileDialog1.FileName));
                }
                catch (SpreadsheetReadWriteException)
                {
                    MessageBox.Show("There was a problem opening the file! Either it was incomplete or incorrectly formatted!", "There was a problem opening the file!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        /// <summary>
        /// When the "Undo" button is clicked under the "File" tab, sets the spreadsheet to how it was before the
        /// change was made and updates the display accordingly.
        /// </summary>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (string cell in controller.Undo(out int colTemp, out int rowTemp, out string content))
            {
                UpdateSpreadsheetValue(cell);
            }
        }

        /// <summary>
        /// When the spreadsheet is closing via the "Close" button via the "File" tab, checks to see if there was a change to the spreadsheet. If there 
        /// hasn't been a change, close the spreadsheet. If there was a change, prompts the user if they would like to close without saving.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!controller.Changed())
                Close();
            if (MessageBox.Show("You have unsaved changes, would you like to close?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Close();
        }

        /// <summary>
        /// If the "Help" button is clicked, opens the "Help.txt" file.
        /// </summary>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(filePath).FullName;
            filePath = Directory.GetParent(filePath).FullName;
            filePath = Directory.GetParent(filePath).FullName;
            Process.Start(filePath + "\\Help.txt");
        }

        /// <summary>
        /// When the spreadsheet is closing via the Form's exit button, check to see if there was a change to the spreadsheet. If there was,
        /// prompts the user if they would like to close without saving.
        /// </summary>
        private void SpreadsheetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (controller.Changed() && MessageBox.Show("You have unsaved changes, would you like to close?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }

        /// <summary>
        /// When the cell is clicked, highlights the contents of CellContentText for easier editability.
        /// </summary>

        // Debating on whether to keep this feature
        private void HighlightCellContentsOnClick(SpreadsheetPanel ss)
        {
            displaySelection(spreadsheetPanel1);
            CellContentText.SelectAll();
        }

        private void JoinButton_Click(object sender, EventArgs e)
        {
            Connect(sender, e);
            //networkController.Connect("localhost", "Chad\n");
        }

        /// <summary>
        /// if server name is not empty, method updates Controller with server name and player name in order to initialize server connection. 
        /// </summary>
        private void Connect(object sender, EventArgs e)
        {
            if (ServerLabel.Text == "") //check for empty server box 
            {
                MessageBox.Show("Please enter a server address.");
                return;
            }

            // Disable the controls and try connecting
            AddressText.Enabled = false;
            JoinButton.Enabled = false;
            KeyPreview = true;

            networkController.Connect(AddressText.Text, UsernameBox.Text); //initate connection network protocol in controller 
        }

        ///<summary>
        /// Event handler for server updates, updates drawings 
        ///</summary> 
        private void ProcessData()
        {
            try
            {
                MethodInvoker invalidator = new MethodInvoker(() => this.Invalidate(true));
                this.Invoke(invalidator);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Error event handler
        /// Displays error message with given err string 
        /// </summary>
        private void Error(string err)
        {
            MessageBox.Show(err);

            this.Invoke(new MethodInvoker(() =>
            {
                AddressText.Enabled = true;
                JoinButton.Enabled = true;
            }));
        }

        /// <summary>
        /// When a spreadsheet window is opened, automatically put the cursor into the CellContentText for ease of access.
        /// </summary>
        private void SpreadsheetForm_Load(object sender, EventArgs e)
        {
            this.ActiveControl = CellContentText;
        }

        
    }
}
