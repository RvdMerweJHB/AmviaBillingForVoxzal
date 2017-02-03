using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace AmviaBillingVoxzal.Classes
{
    class ClassData
    {
        public static DataTable GetDataTable(string connectionString)
        {
            DataTable resultTable = new DataTable();

            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand sqlCommand = new SqlCommand();

            sqlConnection.ConnectionString = connectionString;
            StringBuilder sqlQuery = new StringBuilder();

            sqlQuery.AppendLine("USE [TempAtlantic]");
            sqlQuery.AppendLine("");
            sqlQuery.AppendLine("SELECT TOP 1000");
            sqlQuery.AppendLine("	[CustNmbr],");
            sqlQuery.AppendLine("   (SELECT TOP 1 Name FROM [CUBE].[dbo].[Company] WHERE Groupcode COLLATE DATABASE_DEFAULT = [CustNmbr] ORDER BY ID ASC) AS 'Name',");
            sqlQuery.AppendLine("   (SELECT TOP 1 ID FROM [CUBE].[dbo].[Company] WHERE Groupcode COLLATE DATABASE_DEFAULT = [CustNmbr] ORDER BY ID ASC) AS 'Company_ID',");
            sqlQuery.AppendLine("   CASE WHEN ([CustNmbr] LIKE 'STL%' OR [CustNmbr] LIKE 'VOD001' OR [CustNmbr] LIKE 'TRA001' OR [CustNmbr] LIKE 'CMA' OR [CustNmbr] LIKE 'Knutton' OR [CustNmbr] LIKE 'Sharp' OR [CustNmbr] LIKE 'CAP') THEN 'TRUE' ELSE 'FALSE' END AS 'Stratus',");
            sqlQuery.AppendLine("   CASE WHEN (SELECT TOP 1 MinimumBilling FROM [CUBE].[dbo].[Company] WHERE GRoupcode COLLATE DATABASE_DEFAULT = [CustNmbr] order BY ID) = -1.00 THEN 'Sliding Scale' Else 'Per User' END AS 'Type'");
            sqlQuery.AppendLine("FROM");
            sqlQuery.AppendLine("	[TempAtlantic].[dbo].[VmanBillingFile]");
            //sqlQuery.AppendLine("WHERE");
            //sqlQuery.AppendLine("   [CustNmbr] LIKE 'VOD001'");
            sqlQuery.AppendLine("GROUP BY");
            sqlQuery.AppendLine("   [CustNmbr]");
            sqlQuery.AppendLine("ORDER BY");
            sqlQuery.AppendLine("   [CustNmbr]");

            sqlCommand.CommandText = sqlQuery.ToString();
            sqlConnection.Open();
            using (SqlDataAdapter sqlAdaptor = new SqlDataAdapter(sqlCommand.CommandText, sqlConnection))
            {
                sqlAdaptor.Fill(resultTable);
            }
            sqlConnection.Close();

            return resultTable;
        }

        public static int GetUserCount(string connectionString, ClassCompany company)
        {
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand sqlCommand = new SqlCommand();

            sqlConnection.ConnectionString = connectionString;
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "SELECT COUNT(*) FROM InterFaxDB.dbo.Users WHERE GroupId IN (SELECT handle FROM InterFaxDB.dbo.Groups WHERE GroupID = '" + company.GroupCode + "')";
            sqlConnection.Open();
            var count = sqlCommand.ExecuteScalar();
            sqlConnection.Close();

            return Convert.ToInt16(count);
        }

        public static double GetMinBilling(string connectionString, ClassCompany company)
        {
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand sqlCommand = new SqlCommand();

            sqlConnection.ConnectionString = connectionString;
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "SELECT MinimumBilling FROM company WHERE ID = '" + company.company_ID + "'";
            sqlConnection.Open();
            var count = sqlCommand.ExecuteScalar();
            sqlConnection.Close();

            return Convert.ToDouble(count);
        }

        public static double GetMinBilCost(string connectionString, ClassCompany company)
        {
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand sqlCommand = new SqlCommand();

            sqlConnection.ConnectionString = connectionString;
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "SELECT [UnitPrice] FROM [TempAtlantic].[dbo].[VmanBillingFile] WHERE CustNmbr = '" + company.GroupCode + "' AND Description = 'Minimum Billing'";
            sqlConnection.Open();
            var count = sqlCommand.ExecuteScalar();
            sqlConnection.Close();

            return Convert.ToDouble(count);
        }

        public static double GetCompanyLevelCost(string connectionString, ClassCompany company)
        {
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand sqlCommand = new SqlCommand();

            sqlConnection.ConnectionString = connectionString;
            sqlCommand.Connection = sqlConnection;

            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.AppendLine("DECLARE");
            sqlQuery.AppendLine("		@dteDateFrom	AS datetime,");
            sqlQuery.AppendLine("		@dteDateTo		AS datetime");
            sqlQuery.AppendLine("SET @dteDateFrom = CAST('01 ' + DATENAME(MONTH, DATEADD(MONTH, -1, GETDATE())) + ' ' + DATENAME(YEAR, DATEADD(MONTH, -1, GETDATE())) AS datetime)");
            sqlQuery.AppendLine("SET @dteDateTo = DATEADD(MONTH, 1, @dteDateFrom)");
            sqlQuery.AppendLine("SELECT");
            sqlQuery.AppendLine(" CASE WHEN SUM(COST) IS NULL THEN '0' ELSE SUM(COST) END AS 'Cost'");
            sqlQuery.AppendLine("FROM");
            sqlQuery.AppendLine("	Fax WHERE");
            sqlQuery.AppendLine("   datesent BETWEEN @dteDateFrom AND @dteDateTo  ");
            sqlQuery.AppendLine("   AND fax.username COLLATE DATABASE_DEFAULT IN (SELECT userid from interfaxdb.dbo.Users WHere groupid in (SELECT handle FROM InterFaxDB.dbo.Groups WHERE GroupID = '" + company.GroupCode + "'))");

            sqlCommand.CommandText = sqlQuery.ToString();
            sqlConnection.Open();
            var count = sqlCommand.ExecuteScalar();
            sqlConnection.Close();

            return Convert.ToDouble(count);
        }

        public static double GetCompanyLevelCost(string connectionString, ClassCompany company, int companyID)
        {
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand sqlCommand = new SqlCommand();

            sqlConnection.ConnectionString = connectionString;
            sqlCommand.Connection = sqlConnection;

            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.AppendLine("DECLARE");
            sqlQuery.AppendLine("		@dteDateFrom	AS datetime,");
            sqlQuery.AppendLine("		@dteDateTo		AS datetime");
            sqlQuery.AppendLine("SET @dteDateFrom = CAST('01 ' + DATENAME(MONTH, DATEADD(MONTH, -1, GETDATE())) + ' ' + DATENAME(YEAR, DATEADD(MONTH, -1, GETDATE())) AS datetime)");
            sqlQuery.AppendLine("SET @dteDateTo = DATEADD(MONTH, 1, @dteDateFrom)");
            sqlQuery.AppendLine("SELECT");
            sqlQuery.AppendLine(" CASE WHEN SUM(COST) IS NULL THEN '0' ELSE SUM(COST) END AS 'Cost'");
            sqlQuery.AppendLine("FROM");
            sqlQuery.AppendLine("	Fax WHERE");
            sqlQuery.AppendLine("   datesent BETWEEN @dteDateFrom AND @dteDateTo  ");
            sqlQuery.AppendLine("   AND companyID = " + companyID + "");

            sqlCommand.CommandText = sqlQuery.ToString();
            sqlConnection.Open();
            var count = sqlCommand.ExecuteScalar();
            sqlConnection.Close();

            return Convert.ToDouble(count);
        }

        public static List<ClassUser> GetUsers(string connectionString, ClassCompany company)
        {
            List<ClassUser>  userList = new List<ClassUser>();
            DataTable resultTable = new DataTable();

            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand sqlCommand = new SqlCommand();

            sqlConnection.ConnectionString = connectionString;
            StringBuilder sqlQuery = new StringBuilder();

            sqlQuery.AppendLine("DECLARE");
            sqlQuery.AppendLine("		@dteDateFrom	AS datetime,");
            sqlQuery.AppendLine("		@dteDateTo		AS datetime");
            sqlQuery.AppendLine("SET @dteDateFrom = CAST('01 ' + DATENAME(MONTH, DATEADD(MONTH, -1, GETDATE())) + ' ' + DATENAME(YEAR, DATEADD(MONTH, -1, GETDATE())) AS datetime)");
            sqlQuery.AppendLine("SET @dteDateTo = DATEADD(MONTH, 1, @dteDateFrom)");
            sqlQuery.AppendLine("SELECT");
            sqlQuery.AppendLine("	RFU.UserID AS 'Username',");
            sqlQuery.AppendLine("	RFU.UserName AS 'FullName',");
            sqlQuery.AppendLine("   (SELECT CASE when sum(cost) is null then 0 else sum(cost) end from Fax as fax where datesent BETWEEN @dteDateFrom AND @dteDateTo and fax.Username = RFU.userid COLLATE DATABASE_DEFAULT) as 'Usage_Cost'");
            sqlQuery.AppendLine("FROM");
            sqlQuery.AppendLine("	InterfaxDB.dbo.Users AS RFU");
            sqlQuery.AppendLine("WHERE ");
            sqlQuery.AppendLine("	GroupID IN (");
            sqlQuery.AppendLine("SELECT handle FROM InterFaxDB.dbo.Groups WHERE GroupID = '" + company.GroupCode + "' ");
            sqlQuery.AppendLine(")");
            sqlConnection.Open();

            sqlCommand.CommandText = sqlQuery.ToString();
            using (SqlDataAdapter sqlAdaptor = new SqlDataAdapter(sqlCommand.CommandText, sqlConnection))
            {
                sqlAdaptor.Fill(resultTable);
            }
            sqlConnection.Close();

            foreach (DataRow row in resultTable.Rows)
            {
                ClassUser user = new ClassUser(row);

                if (user.Cost >= company.MinBill)
                    user.UsageVSMinBill = user.Cost;
                else
                    user.UsageVSMinBill = company.MinBill;

                userList.Add(user);
            }

            return userList;

        }

        public static List<ClassUser> GetUsers(string connectionString, ClassCompany company, int companyId)
        {
            List<ClassUser> userList = new List<ClassUser>();
            DataTable resultTable = new DataTable();

            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand sqlCommand = new SqlCommand();

            sqlConnection.ConnectionString = connectionString;
            StringBuilder sqlQuery = new StringBuilder();

            sqlQuery.AppendLine("DECLARE");
            sqlQuery.AppendLine("		@dteDateFrom	AS datetime,");
            sqlQuery.AppendLine("		@dteDateTo		AS datetime");
            sqlQuery.AppendLine("SET @dteDateFrom = CAST('01 ' + DATENAME(MONTH, DATEADD(MONTH, -1, GETDATE())) + ' ' + DATENAME(YEAR, DATEADD(MONTH, -1, GETDATE())) AS datetime)");
            sqlQuery.AppendLine("SET @dteDateTo = DATEADD(MONTH, 1, @dteDateFrom)");
            sqlQuery.AppendLine("SELECT");
            sqlQuery.AppendLine("	RFU.UserID AS 'Username',");
            sqlQuery.AppendLine("	RFU.UserName AS 'FullName',");
            sqlQuery.AppendLine("   (SELECT CASE when sum(cost) is null then 0 else sum(cost) end from Fax as fax where datesent BETWEEN @dteDateFrom AND @dteDateTo and fax.Username = RFU.userid COLLATE DATABASE_DEFAULT) as 'Usage_Cost'");
            sqlQuery.AppendLine("FROM");
            sqlQuery.AppendLine("	InterfaxDB.dbo.Users AS RFU");
            sqlQuery.AppendLine("WHERE ");
            sqlQuery.AppendLine("	RFU.UserID COLLATE DATABASE_DEFAULT IN (SELECT UserName from UserData WHere companyID = " + companyId + " )");
            sqlConnection.Open();

            sqlCommand.CommandText = sqlQuery.ToString();
            using (SqlDataAdapter sqlAdaptor = new SqlDataAdapter(sqlCommand.CommandText, sqlConnection))
            {
                sqlAdaptor.Fill(resultTable);
            }
            sqlConnection.Close();

            foreach (DataRow row in resultTable.Rows)
            {
                ClassUser user = new ClassUser(row);

                if (user.Cost >= company.MinBill)
                    user.UsageVSMinBill = user.Cost;
                else
                    user.UsageVSMinBill = company.MinBill;

                userList.Add(user);
            }

            return userList;

        }

        public static List<ClassUser> GetUsers(string connectionString, ClassCompany company, string companyName)
        {
            List<ClassUser> userList = new List<ClassUser>();
            DataTable resultTable = new DataTable();

            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand sqlCommand = new SqlCommand();

            sqlConnection.ConnectionString = connectionString;
            StringBuilder sqlQuery = new StringBuilder();

            sqlQuery.AppendLine("DECLARE");
            sqlQuery.AppendLine("		@dteDateFrom	AS datetime,");
            sqlQuery.AppendLine("		@dteDateTo		AS datetime");
            sqlQuery.AppendLine("SET @dteDateFrom = CAST('01 ' + DATENAME(MONTH, DATEADD(MONTH, -1, GETDATE())) + ' ' + DATENAME(YEAR, DATEADD(MONTH, -1, GETDATE())) AS datetime)");
            sqlQuery.AppendLine("SET @dteDateTo = DATEADD(MONTH, 1, @dteDateFrom)");
            sqlQuery.AppendLine("SELECT");
            sqlQuery.AppendLine("	RFU.UserID AS 'Username',");
            sqlQuery.AppendLine("	RFU.UserName AS 'FullName',");
            sqlQuery.AppendLine("   (SELECT CASE when sum(cost) is null then 0 else sum(cost) end from Fax as fax where datesent BETWEEN @dteDateFrom AND @dteDateTo and fax.Username = RFU.userid COLLATE DATABASE_DEFAULT) as 'Usage_Cost'");
            sqlQuery.AppendLine("FROM");
            sqlQuery.AppendLine("	InterfaxDB.dbo.Users AS RFU");
            sqlQuery.AppendLine("WHERE ");
            sqlQuery.AppendLine("	RFU.UserID COLLATE DATABASE_DEFAULT IN (SELECT UserName from UserData WHere companyID IN (SELECT ID FROM COMPANY WHERE NAME LIKE '" + companyName + "%') )");
            sqlConnection.Open();

            sqlCommand.CommandText = sqlQuery.ToString();
            using (SqlDataAdapter sqlAdaptor = new SqlDataAdapter(sqlCommand.CommandText, sqlConnection))
            {
                sqlAdaptor.Fill(resultTable);
            }
            sqlConnection.Close();

            foreach (DataRow row in resultTable.Rows)
            {
                ClassUser user = new ClassUser(row);

                if (user.Cost >= company.MinBill)
                    user.UsageVSMinBill = user.Cost;
                else
                    user.UsageVSMinBill = company.MinBill;

                userList.Add(user);
            }

            return userList;

        }

        public static void InsertTableIntoDB(DataTable table, string connectionString)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = connectionString;
            sqlConnection.Open();
            try
            {
                SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
                bulkCopy.DestinationTableName = table.TableName;
                bulkCopy.WriteToServer(table);
               
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
            sqlConnection.Close();

        }
    }
}
