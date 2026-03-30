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

        // Constructor vacío
        public Mdl_MeasurementUnits()
        {
            IsActive = true;
        }

        // Constructor con parámetros
        public Mdl_MeasurementUnits(string unitCode, string unitName, string abbreviation)
        {
            this.UnitCode = unitCode;
            this.UnitName = unitName;
            this.Abbreviation = abbreviation;
            this.IsActive = true;
        }
    }
}