using System;

namespace SECRON.Models
{
    internal class Mdl_ItemWarehouseStock
    {
        public int ItemWarehouseStockId { get; set; }
        public int ItemId { get; set; }
        public int WarehouseId { get; set; }

        public decimal CurrentStock { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal MaximumStock { get; set; }
        public decimal ReorderPoint { get; set; }
        public int MovementCounter { get; set; }
        public DateTime? LastMovementDate { get; set; }

        // Resueltos desde Items (globales, solo lectura)
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LastPurchasePrice { get; set; }
        public bool HasLotControl { get; set; }
        public bool HasExpiryDate { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }

        public Mdl_ItemWarehouseStock()
        {
            CurrentStock = 0;
            MinimumStock = 0;
            MovementCounter = 0;
        }
    }
}