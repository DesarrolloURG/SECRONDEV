using System;

namespace SECRON.Models
{
    internal class Mdl_Warehouse
    {
        public int WarehouseId { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int? ManagerUserId { get; set; }
        public string WarehouseType { get; set; }
        public int? LocationId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public Mdl_Warehouse()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }
    }
}