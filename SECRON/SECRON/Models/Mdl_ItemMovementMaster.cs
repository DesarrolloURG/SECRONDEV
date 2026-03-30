using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_ItemMovementMaster
    {
        public int MovementMasterId { get; set; }
        public string MovementNumber { get; set; }
        public DateTime MovementDate { get; set; }
        public int MovementTypeId { get; set; }
        public int LocationId { get; set; }

        // Referencias
        public int? SupplierId { get; set; }
        public string ReferenceDocument { get; set; }
        public int? DestinationLocationId { get; set; }

        // Información general
        public string Remarks { get; set; }
        public decimal TotalAmount { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        // Constructor vacío
        public Mdl_ItemMovementMaster()
        {
            MovementDate = DateTime.Now;
            CreatedDate = DateTime.Now;
            IsActive = true;
            TotalAmount = 0;
        }

        // Constructor con parámetros
        public Mdl_ItemMovementMaster(string movementNumber, int movementTypeId, int locationId, int createdBy)
        {
            this.MovementNumber = movementNumber;
            this.MovementTypeId = movementTypeId;
            this.LocationId = locationId;
            this.CreatedBy = createdBy;
            this.MovementDate = DateTime.Now;
            this.CreatedDate = DateTime.Now;
            this.IsActive = true;
            this.TotalAmount = 0;
        }
    }
}