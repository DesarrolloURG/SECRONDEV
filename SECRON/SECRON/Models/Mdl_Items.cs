using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Items
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int UnitId { get; set; }

        public decimal MinimumStock { get; set; }
        public decimal MaximumStock { get; set; }
        public decimal ReorderPoint { get; set; }

        public decimal UnitCost { get; set; }
        public decimal LastPurchasePrice { get; set; }

        public bool HasLotControl { get; set; }
        public bool HasExpiryDate { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Campos de joins
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string UnitName { get; set; }

        public Mdl_Items()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
            HasLotControl = false;
            HasExpiryDate = false;
            MinimumStock = 0;
            UnitCost = 0;
        }

        public Mdl_Items(string itemCode, string itemName, int categoryId, int subCategoryId, int unitId)
        {
            this.ItemCode = itemCode;
            this.ItemName = itemName;
            this.CategoryId = categoryId;
            this.SubCategoryId = subCategoryId;
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