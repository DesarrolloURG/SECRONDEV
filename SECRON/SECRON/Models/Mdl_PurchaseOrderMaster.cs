using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_PurchaseOrderMaster
    {
        public int PurchaseOrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int RequisitionMasterId { get; set; }
        public int SupplierId { get; set; }
        public int DeliveryLocationId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int StatusId { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        public Mdl_PurchaseOrderMaster()
        {
            OrderDate = DateTime.Now;
            CreatedDate = DateTime.Now;
            IsActive = true;
            TotalAmount = 0;
        }
    }
}