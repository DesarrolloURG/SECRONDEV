using System;

namespace SECRON.Models
{
    public class Mdl_WarehouseManager
    {
        public int WarehouseManagerId { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}