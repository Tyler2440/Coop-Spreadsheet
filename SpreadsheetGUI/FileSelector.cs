using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetController;
namespace SpreadsheetGUI
{
    public partial class FileSelector : Form
    {
        ComboBox box;
        SpreadsheetController.Controller controller;

        public FileSelector(List<string> spreadsheets, SpreadsheetController.Controller controller)
        {          
            InitializeComponent();

            box = new ComboBox();

            box.SelectionChangeCommitted += SelectionChange_Committed;
            SelectButton.Enabled = false;

            this.controller = controller;
            box.Location = new Point(67,90);
            box.Size = new Size(123, 21);
            box.Name = "Select Spreadsheet";

            foreach(string spreadsheet in spreadsheets)
            {
                box.Items.Add(spreadsheet);
            }
            box.Text = "Select Spreadsheet";
            this.Controls.Add(box);
        }

        private void select_Click(object sender, EventArgs e)
        {
            controller.SendFileSelect(box.SelectedItem.ToString());
            this.Close();
        }

        private void CreateNew_Click(object sender, EventArgs e)
        {
            controller.SendFileSelect(textBox1.Text + "\n");
            this.Close();
        }

        private void SelectionChange_Committed(object sender, EventArgs e)
        {
           SelectButton.Enabled = true;
        }


    }
}
