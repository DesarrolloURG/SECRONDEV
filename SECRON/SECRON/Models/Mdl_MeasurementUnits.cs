using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_MeasurementUnits
    {
        // Campos principales
        public int UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string Abbreviation { get; set; }
        public bool IsActive { get; set; }

        public string IsActiveText => IsActive ? "ACTIVO" : "INACTIVO";

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Constructor vacío
        public Mdl_MeasurementUnits()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros
        public Mdl_MeasurementUnits(string unitCode, string unitName, string abbreviation)
        {
            this.UnitCode = unitCode;
            this.UnitName = unitName;
            this.Abbreviation = abbreviation;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}