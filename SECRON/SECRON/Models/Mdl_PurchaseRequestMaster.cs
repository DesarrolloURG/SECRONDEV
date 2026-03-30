using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_PurchaseRequestMaster
    {
        public int RequestMasterId { get; set; }
        public string RequestNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public int ResponsibleUserId { get; set; }
        public int StatusId { get; set; }
        public int LocationId { get; set; }
        public int DepartmentId { get; set; }
        public decimal TotalBudget { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        public Mdl_PurchaseRequestMaster()
        {
            RequestDate = DateTime.Now;
            CreatedDate = DateTime.Now;
            IsActive = true;
            TotalBudget = 0;
        }
    }
}