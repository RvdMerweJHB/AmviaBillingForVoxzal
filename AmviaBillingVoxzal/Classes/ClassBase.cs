using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmviaBillingVoxzal.Classes
{
    class ClassBase
    {
        #region Properties
        public string Company { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public int UserCount { get; set; }
        public double Cost { get; set; }
        public double UsageVSMinBill { get; set; }
        public double MinBill { get; set; }
        public double FinalBill { get; set; }
        public double CompanyMinBill { get; set; }
        #endregion 

        #region Constuctor
        public ClassBase()
        {
        }
        #endregion

        #region Methods
        
        #endregion
    }
}
