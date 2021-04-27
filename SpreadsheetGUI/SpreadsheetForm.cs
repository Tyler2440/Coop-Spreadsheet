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


            // Adds two events to SelectionChanged: HighlightCellContentsOnClick. When a cell is
            // clicked on, these two events are run
            spreadsheetPanel1.SelectionChanged += OnCellClick;

            // Initially, highlight cell A1 and update CellNameText to display A1
            spreadsheetPanel1.SetSelection(0, 0);
            CellNameText.Text = "A1";

            networkController.FileSelect += JoinServer;
            networkController.cellSelection += SetUserSelectedCell;
            networkController.RequestError += DisplayRequestError;
            networkController.ServerError += DisplayServerError;
            networkController.UserDisconnected += DisconnectUser;
            networkController.Error += Error;
            networkController.ChangeContents += SetCellContents;
        }



        private void JoinServer(List<string> spreadsheets)
        {
            FileSelector fileselector = new FileSelector(spreadsheets, networkController);
            fileselector.ShowDialog();
        }


        /// <summary>
        /// Event handler for setting cell contents. Updates model with new contents 
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="contents"></param>
        private void SetCellContents(string cellName, string contents)
        {
            int colTemp;
            int rowTemp;
            controller.GetColRow(cellName, out colTemp, out rowTemp);
            controller.SetCellContent(colTemp, rowTemp, contents);
            UpdateSpreadsheetValue(cellName);
        }

        /// <summary>
        /// Upates Spreadsheet Panel with user selected cells 
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="ID"></param>
        /// <param name="userName"></param>
        private void SetUserSelectedCell(string cellName, int ID, string userName)
        {
            Dictionary<int, User> users = controller.UpdateUserCellSelection(ID, userName, cellName);
            spreadsheetPanel1.SetUserSelection(users);
        }

        /// <summary>
        /// Removes other user from model 
        /// </summary>
        /// <param name="ID">ID of user disconnecting</param>
        private void DisconnectUser(int ID)
        {
            controller.RemoveUser(ID);
            spreadsheetPanel1.Refresh();
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
                int colTemp;
                int rowTemp;
                spreadsheetPanel1.GetSelection(out colTemp, out rowTemp);
                networkController.RequestCellEdit(controller.GetName(colTemp, rowTemp), CellContentText.Text);
            }
        }

        /// <summary>
        /// When the "Undo" button is clicked under the "File" tab, sets the spreadsheet to how it was before the
        /// change was made and updates the display accordingly.
        /// </summary>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            networkController.RequestUndo();
        }

        /// <summary>
        /// When the spreadsheet is closing via the Form's exit button, check to see if there was a change to the spreadsheet. If there was,
        /// prompts the user if they would like to close without saving.
        /// </summary>
        private void SpreadsheetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }

        /// <summary>
        /// When the cell is clicked, highlights the contents of CellContentText for easier editability.
        /// </summary>
        private void OnCellClick(SpreadsheetPanel ss, int col, int row)
        {
            this.col = col;
            this.row = row;
            networkController.SendCellSelection(controller.GetName(col, row));
            displaySelection(spreadsheetPanel1);
            CellContentText.SelectAll();
        }

        private void JoinButton_Click(object sender, EventArgs e)
        {
            Connect(sender, e);
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
                UsernameBox.Enabled = true;
            }));
        }

        /// <summary>
        /// Event handler to display error due to change cell content request 
        /// </summary>
        /// <param name="cellName">cell name</param>
        /// <param name="err">error message from server</param>
        private void DisplayRequestError(string cellName, string err)
        {
            MessageBox.Show("Invalid change request" + cellName + "\n" + "Error message: " + err);
        }

        /// <summary>
        /// Event handler to display server closing message 
        /// </summary>
        /// <param name="err"></param>
        private void DisplayServerError(string err)
        {
            MessageBox.Show("Server shut down message: " + err);
            this.Invoke(new MethodInvoker(() =>
            {
                AddressText.Enabled = true;
                JoinButton.Enabled = true;
                UsernameBox.Enabled = true;
            }));
        }

        /// <summary>
        /// tells network controller to rever cell 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int colTemp;
            int rowTemp;
            spreadsheetPanel1.GetSelection(out colTemp, out rowTemp);
            networkController.RequestRevert(controller.GetName(colTemp, rowTemp));
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
