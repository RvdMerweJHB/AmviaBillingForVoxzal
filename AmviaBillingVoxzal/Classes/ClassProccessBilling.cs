using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.Security.Cryptography;

namespace AmviaBillingVoxzal.Classes
{
    class ClassProccessBilling
    {
        #region Methods

        public void AssembleSections()
        {
            string connectionstring = "Data Source=172.16.88.23;Initial Catalog=Cube;User ID=amvia;Password=Vh0stDB";
            DataTable resultTable = ClassData.GetDataTable(connectionstring);

            DataTable companyTable = new DataTable();
            companyTable = AddRequiredColums(companyTable);
            
            foreach (DataRow row in resultTable.Rows)
            {
                ClassCompany company = new ClassCompany(row);

                #region Secion One(Company)

                if (company.Stratus)
                {
                    company.CreateStratusMinBillCompany(ref company, connectionstring);

                    if (company.Company.StartsWith("CMA") || company.Company.StartsWith("Knutton"))
                        company.users = ClassData.GetUsers(connectionstring, company, company.company_ID);
                    else if (company.Company.StartsWith("Sharp") || company.Company.StartsWith("CAP"))
                        company.users = ClassData.GetUsers(connectionstring, company, company.Company.Substring(0,5));
                    else
                        company.users = ClassData.GetUsers(connectionstring, company);

                }
                else
                {
                    company.CreateNormalMinBillCompany(ref company, connectionstring);
                    company.users = ClassData.GetUsers(connectionstring, company);

                }

                #endregion

                #region Assemble Sections
                DataRow CompanyLevelRow = companyTable.NewRow();
                CompanyLevelRow["Company"] = company.Company;
                CompanyLevelRow["Username"] = "";
                CompanyLevelRow["Name"] = "";
                CompanyLevelRow["UserCount"] = company.UserCount;
                CompanyLevelRow["Cost"] = company.Cost;
                CompanyLevelRow["Usage vs Min Billing"] = company.UsageVSMinBill;

                CompanyLevelRow["Min Billing"] = company.MinBill;
                CompanyLevelRow["Final Bill"] = "";
                CompanyLevelRow["Company Min Billing"] = company.CompanyMinBill;
                CompanyLevelRow["Billing Type"] = company.BillingType;

                companyTable.Rows.Add(CompanyLevelRow);

                foreach (ClassUser user in company.users)
                {
                    DataRow userLevelRow = companyTable.NewRow();
                    userLevelRow["Company"] = user.Company;
                    userLevelRow["Username"] = user.Username;
                    userLevelRow["Name"] = user.Name;
                    userLevelRow["UserCount"] = "";
                    userLevelRow["Cost"] = user.Cost;
                    userLevelRow["Usage vs Min Billing"] = user.UsageVSMinBill;
                    userLevelRow["Min Billing"] = "";
                    userLevelRow["Final Bill"] = "";
                    userLevelRow["Company Min Billing"] =user.CompanyMinBill;

                    companyTable.Rows.Add(userLevelRow);
                        
                }

                DataRow emptyRow = companyTable.NewRow();
                companyTable.Rows.Add(emptyRow);
                companyTable.AcceptChanges();

            }

            companyTable.AcceptChanges();

            #endregion
            
            ClassCSVTools.SaveTableToCSV(companyTable,"C:\\Users\\Developer\\Documents\\AmviaBillingOutput","Test1.csv");
        }
        #endregion

        private DataTable AddRequiredColums(DataTable companyTable)
        {
            DataColumn columnOne = new DataColumn();
            columnOne.ColumnName = "Company";
            companyTable.Columns.Add(columnOne);

            DataColumn columnTwo = new DataColumn();
            columnTwo.ColumnName = "Username";
            companyTable.Columns.Add(columnTwo);

            DataColumn columnThree = new DataColumn();
            columnThree.ColumnName = "Name";
            companyTable.Columns.Add(columnThree);

            DataColumn columnFour = new DataColumn();
            columnFour.ColumnName = "UserCount";
            companyTable.Columns.Add(columnFour);

            DataColumn columnFive = new DataColumn();
            columnFive.ColumnName = "Cost";
            companyTable.Columns.Add(columnFive);

            DataColumn columnSix = new DataColumn();
            columnSix.ColumnName = "Usage vs Min Billing";
            companyTable.Columns.Add(columnSix);

            DataColumn columnSeven = new DataColumn();
            columnSeven.ColumnName = "Min Billing";
            companyTable.Columns.Add(columnSeven);

            DataColumn columnEight = new DataColumn();
            columnEight.ColumnName = "Final Bill";
            companyTable.Columns.Add(columnEight);

            DataColumn columnNine = new DataColumn();
            columnNine.ColumnName = "Company Min Billing";
            companyTable.Columns.Add(columnNine);

            DataColumn columnTen = new DataColumn();
            columnTen.ColumnName = "Billing Type";
            companyTable.Columns.Add(columnTen);

            return companyTable;
        }
    }
}
