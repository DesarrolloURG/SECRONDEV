using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_PurchaseRequestStatus
    {
        public int StatusId { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public bool IsActive { get; set; }

        public Mdl_PurchaseRequestStatus()
        {
            IsActive = true;
        }
    }
}