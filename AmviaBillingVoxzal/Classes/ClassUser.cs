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
    class ClassUser: ClassBase
    {
        public ClassUser(DataRow row)
        {
            Username = Convert.ToString(row["Username"]);
            Name = Convert.ToString(row["FullName"]);
            Cost = Convert.ToDouble(row["Usage_Cost"]);
        }
    }
}
