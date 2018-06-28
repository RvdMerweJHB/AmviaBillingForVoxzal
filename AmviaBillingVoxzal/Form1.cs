using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmviaBillingVoxzal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnImportFile_Click(object sender, EventArgs e)
        {
            string fileLocation = string.Empty;
            string connectionstring = ConfigurationManager.AppSettings["VbillInfoConnectionString"];
            string message = "";

            dlgFileLocation.ShowDialog();
            fileLocation = dlgFileLocation.FileName.ToString();

            message = Classes.ClassCSVTools.NewMonth(fileLocation, connectionstring);
            if (message == "")
            {
                //Check if current table needs to be archived:
                Classes.ClassData.RenameTable(connectionstring);

                //Create new table to be used:
                Classes.ClassData.CreateNewTable(connectionstring);

                //Import datatable to database
                Classes.ClassData.InsertTableIntoDB(Classes.ClassCSVTools.ReadTableFromCSV(fileLocation), connectionstring);
                MessageBox.Show("File has been imported, you can now proceed by clicking 'Run'", "Import Successful");

            }
            else
                MessageBox.Show(message, "Import Failure");


        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            Classes.ClassProccessBilling objBilling = new Classes.ClassProccessBilling();

            try
            {
                objBilling.AssembleSections();
                MessageBox.Show("CSV File Creation","CSV File has been created.");

            }
            catch (Exception ex)
            {
                MessageBox.Show("CSV File Creation", "There has been an error:" + Environment.NewLine +  ex.Message);

            }

        }

    }

}
