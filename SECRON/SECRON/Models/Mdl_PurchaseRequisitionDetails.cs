using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_PurchaseRequisitionDetails
    {
        public int RequisitionDetailId { get; set; }
        public int RequisitionMasterId { get; set; }
        public int? RequestDetailId { get; set; }
        public int ItemId { get; set; }
        public int SupplierId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
        public string RequestReason { get; set; }

        public Mdl_PurchaseRequisitionDetails()
        {
            Quantity = 0;
            UnitCost = 0;
            TotalCost = 0;
        }
    }
}