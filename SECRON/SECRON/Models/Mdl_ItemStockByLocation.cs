using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_ItemStockByLocation
    {
        public int ItemStockLocationId { get; set; }
        public int ItemId { get; set; }
        public int LocationId { get; set; }

        public decimal CurrentStock { get; set; }
        public decimal ReservedStock { get; set; }
        public decimal AvailableStock { get; set; }  // Calculado: CurrentStock - ReservedStock

        public decimal MinimumStock { get; set; }
        public decimal MaximumStock { get; set; }

        public DateTime? LastMovementDate { get; set; }

        public bool IsActive { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public Mdl_ItemStockByLocation()
        {
            IsActive = true;
            CurrentStock = 0;
            ReservedStock = 0;
            MinimumStock = 0;
        }
    }
}