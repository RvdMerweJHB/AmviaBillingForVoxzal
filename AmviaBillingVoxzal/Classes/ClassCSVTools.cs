using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.OleDb;

namespace AmviaBillingVoxzal.Classes
{
    class ClassCSVTools
    {
        public static string ReadTableFromCSV(string pathName, string sheetName)
        {
            DataTable tbContainer = new DataTable();
            string strConn = string.Empty;
            if (string.IsNullOrEmpty(sheetName)) { sheetName = "Sheet1"; }
            FileInfo file = new FileInfo(pathName);
            if (!file.Exists) { throw new Exception("Error, file doesn't exists!"); }
            string extension = file.Extension;
            switch (extension)
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                    break;
                default:
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
            }
            OleDbConnection cnnxls = new OleDbConnection(strConn);
            OleDbDataAdapter oda = new OleDbDataAdapter(string.Format("select * from [{0}$]", sheetName), cnnxls);
            DataSet ds = new DataSet();
            oda.Fill(tbContainer);
            DataTable test = new DataTable();
            test = tbContainer;
            return "read";
        }

        public static DataTable ReadTableFromCSV(string pathName)
        {
            DataTable dt = new DataTable("VmanBillingFile");
            DataRow Row;
            string CSVFilePathName = pathName;
            string[] Lines = File.ReadAllLines(CSVFilePathName);
            string[] Fields = Lines[0].Split(new char[] { '|' }); ;
            int[] numericColumns = new int[] { 2, 3, 4, 7};
            int Cols = Fields.GetLength(0) - 1;
      
            //1st row must be column names; force lower case to ensure matching later on.
            for (int i = 0; i < Cols; i++)
                dt.Columns.Add(Fields[i].ToLower(), typeof(string));

            for (int i = 1; i < Lines.GetLength(0); i++)
            {
                Fields = Lines[i].Split(new char[] { '|' });
                Row = dt.NewRow();

                for (int f = 0; f < Cols; f++)
                {
                    if (numericColumns.Contains(f))
                    {
                        string inputNumber = Fields[f].Replace("\"", "");

                        if (f == 3)
                            Row[f] = double.Parse(inputNumber);
                        else
                            Row[f] = int.Parse(inputNumber);

                    }
                    else
                        Row[f] = Fields[f].Replace("\"", "");

                }

                dt.Rows.Add(Row);

            }

            return dt;

        }

        public static void SaveTableToCSV(DataTable infoTable, string csvFilePath, string fileName)
        {
            int columns = infoTable.Columns.Count;
            int rows = infoTable.Rows.Count;

            try
            {
                StreamWriter sw = new StreamWriter(csvFilePath + "\\" + fileName, false);

                for (int i = 0; i < columns; i++)
                {
                    sw.Write(infoTable.Columns[i]);

                    if (i < columns - 1)
                    {
                        sw.Write(",");
                    }
                }

                sw.Write(sw.NewLine);

                foreach (DataRow dr in infoTable.Rows)
                {
                    for (int i = 0; i < columns; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            sw.Write(dr[i].ToString());
                        }

                        if (i < columns - 1)
                        {
                            sw.Write(",");
                        }
                    }

                    sw.Write(sw.NewLine);
                }

                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string NewMonth(string pathName, string connection)
        {
            string result = "";
            string[] Lines = File.ReadAllLines(pathName);
            string[] Fields = Lines[1].Split(new char[] { '|' });
            string incommingDatesringValue = Fields[8].Replace("\"", "");
            string existingDatesringValue = ClassData.GetCurrentTableDate(connection);
            string incommingMonth = incommingDatesringValue.Substring(3, 2);
            string incommingYear = incommingDatesringValue.Substring(6, 4);
            DateTime incommingDate = new DateTime(int.Parse(incommingYear), int.Parse(incommingMonth), 1);

            //1. Check if current table does not already contain the incomming data:
            if (incommingDatesringValue == existingDatesringValue)
                result = "Table has already been created for this file, have you selected the correct one?";
            else if (ClassData.CheckExistingTables(connection, incommingDate))
                result = "Table has already been created for this file, have you selected the correct one?";

            return result;

        }

    }
    
}
