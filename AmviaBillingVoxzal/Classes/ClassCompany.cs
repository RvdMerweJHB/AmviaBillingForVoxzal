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
    class ClassCompany:ClassBase
    {
        public int company_ID { get; set; }
        public string GroupCode { get; set; }
        public List<ClassUser> users { get; set; }
        public bool Stratus { get; set; }
        public string BillingType { get; set; }

        public ClassCompany(DataRow row)
        {
            company_ID = Convert.ToInt32(row["Company_ID"]);
            GroupCode = Convert.ToString(row["CustNmbr"]);
            Company = Convert.ToString(row["Name"]);
            Stratus = Convert.ToBoolean(row["Stratus"]);
            BillingType = Convert.ToString(row["Type"]);
        }

        public void CreateNormalMinBillCompany(ref ClassCompany company, string connectionstring)
        {
            company.UserCount = ClassData.GetUserCount(connectionstring, company);
            company.Username = "";
            company.Name = "";
            company.Cost = ClassData.GetCompanyLevelCost(connectionstring, company);
            company.MinBill = ClassData.GetMinBilling(connectionstring, company);
            company.CompanyMinBill = ClassData.GetMinBilCost(connectionstring, company);

            if (company.MinBill == -1)
            {
                company.BillingType = "Sliding Scale";
                company.MinBill = (ClassData.GetMinBilCost(connectionstring, company) / company.UserCount);
            }
            else
                company.BillingType = "Per User";

            if (company.Cost >= company.CompanyMinBill)
                company.UsageVSMinBill = company.Cost;
            else
                company.UsageVSMinBill = company.CompanyMinBill;

            company.FinalBill = 0;

            if (company.CompanyMinBill == 0)
                company.BillingType = "Usage Only";


            company.Company = company.Company + " - " + company.GroupCode;
        }

        public void CreateStratusMinBillCompany(ref ClassCompany company, string connectionstring)
        {
            company.UserCount = ClassData.GetUserCount(connectionstring, company);
            company.Username = "";
            company.Name = "";
            company.Cost = ClassData.GetCompanyLevelCost(connectionstring, company);
            company.MinBill = ClassData.GetMinBilling(connectionstring, company);
            company.CompanyMinBill = ClassData.GetMinBilCost(connectionstring, company);

            if (company.MinBill == -1)
                company.MinBill = (ClassData.GetMinBilCost(connectionstring, company));
            else
                company.BillingType = "Fixed";

            if (company.Cost >= company.CompanyMinBill)
                company.UsageVSMinBill = company.Cost;
            else
                company.UsageVSMinBill = company.CompanyMinBill;

            company.FinalBill = 0;

            company.Company = company.Company + " - " + company.GroupCode;
        }
    }
}
