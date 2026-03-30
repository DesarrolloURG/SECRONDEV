using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_PurchaseRequisitionMaster
    {
        public int RequisitionMasterId { get; set; }
        public string RequisitionNumber { get; set; }
        public DateTime RequisitionDate { get; set; }
        public int ResponsibleUserId { get; set; }
        public int StatusId { get; set; }
        public decimal TotalBudget { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        public Mdl_PurchaseRequisitionMaster()
        {
            RequisitionDate = DateTime.Now;
            CreatedDate = DateTime.Now;
            IsActive = true;
            TotalBudget = 0;
        }
    }
}