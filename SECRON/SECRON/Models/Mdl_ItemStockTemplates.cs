using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_ItemStockTemplates
    {
        // Campos principales
        public int TemplateId { get; set; }
        public int LocationCategoryId { get; set; }
        public int ItemId { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal MaximumStock { get; set; }
        public decimal? ReorderPoint { get; set; }
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Campos resueltos (para mostrar en grilla, no vienen de BD directamente)
        public string LocationCategoryName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }

        // Constructor vacío
        public Mdl_ItemStockTemplates()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
            MinimumStock = 0;
            MaximumStock = 0;
        }

        // Constructor con parámetros
        public Mdl_ItemStockTemplates(int locationCategoryId, int itemId, decimal minimumStock, decimal maximumStock)
        {
            this.LocationCategoryId = locationCategoryId;
            this.ItemId = itemId;
            this.MinimumStock = minimumStock;
            this.MaximumStock = maximumStock;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}
