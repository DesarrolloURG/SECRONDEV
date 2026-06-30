using System;

namespace SECRON.Models
{
    // Representa una línea de detalle a enviar al SP de movimientos (vía JSON)
    internal class Mdl_ItemMovementDetailInput
    {
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Remarks { get; set; }
    }
}