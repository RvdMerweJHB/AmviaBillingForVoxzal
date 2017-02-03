using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            dlgFileLocation.ShowDialog();
            fileLocation = dlgFileLocation.FileName.ToString();

            string connectionstring = "Data Source=172.16.88.23;Initial Catalog=TempAtlantic;User ID=amvia;Password=Vh0stDB";

            //Import datatable to database
            Classes.ClassData.InsertTableIntoDB(Classes.ClassCSVTools.ReadTableFromCSV(fileLocation), connectionstring);
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
                string stackTrace = ex.StackTrace;
                MessageBox.Show("CSV File Creation", "There has been an error:" + Environment.NewLine +  ex.Message);
            }

        }

    }

}
