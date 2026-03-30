using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Items
    {
        // Campos principales
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int UnitId { get; set; }

        // Control de Stock
        public decimal MinimumStock { get; set; }
        public decimal MaximumStock { get; set; }
        public decimal ReorderPoint { get; set; }

        // Valorización
        public decimal UnitCost { get; set; }
        public decimal LastPurchasePrice { get; set; }

        // Control
        public bool HasLotControl { get; set; }
        public bool HasExpiryDate { get; set; }
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Constructor vacío
        public Mdl_Items()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
            HasLotControl = false;
            HasExpiryDate = false;
            MinimumStock = 0;
            UnitCost = 0;
        }

        // Constructor con parámetros
        public Mdl_Items(string itemCode, string itemName, int categoryId, int unitId)
        {
            this.ItemCode = itemCode;
            this.ItemName = itemName;
            this.CategoryId = categoryId;
            this.UnitId = unitId;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
            this.HasLotControl = false;
            this.HasExpiryDate = false;
            this.MinimumStock = 0;
            this.UnitCost = 0;
        }
    }
}