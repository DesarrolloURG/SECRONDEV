using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_AccountingEntryStatus
    {
        public int StatusId { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public bool IsActive { get; set; }

        public Mdl_AccountingEntryStatus()
        {
            IsActive = true;
        }

        public Mdl_AccountingEntryStatus(string statusCode, string statusName)
        {
            StatusCode = statusCode;
            StatusName = statusName;
            IsActive = true;
        }
    }
}