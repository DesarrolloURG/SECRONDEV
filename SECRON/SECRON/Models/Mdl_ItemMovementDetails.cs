using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_ItemMovementDetails
    {
        public int MovementDetailId { get; set; }
        public int MovementMasterId { get; set; }

        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }  // Calculado: Quantity * UnitCost

        public decimal StockBeforeMovement { get; set; }
        public decimal StockAfterMovement { get; set; }

        // Lotes
        public string LotNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // Observaciones
        public string Remarks { get; set; }

        // Constructor vacío
        public Mdl_ItemMovementDetails()
        {
            Quantity = 0;
            UnitCost = 0;
            TotalCost = 0;
            StockBeforeMovement = 0;
            StockAfterMovement = 0;
        }

        // Constructor con parámetros
        public Mdl_ItemMovementDetails(int movementMasterId, int itemId, decimal quantity, decimal unitCost)
        {
            this.MovementMasterId = movementMasterId;
            this.ItemId = itemId;
            this.Quantity = quantity;
            this.UnitCost = unitCost;
            this.TotalCost = quantity * unitCost;
        }
    }
}