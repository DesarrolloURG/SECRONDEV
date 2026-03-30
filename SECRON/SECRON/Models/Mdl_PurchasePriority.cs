using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_PurchasePriority
    {
        public int PriorityId { get; set; }
        public string PriorityCode { get; set; }
        public string PriorityName { get; set; }
        public bool IsActive { get; set; }

        public Mdl_PurchasePriority()
        {
            IsActive = true;
        }
    }
}