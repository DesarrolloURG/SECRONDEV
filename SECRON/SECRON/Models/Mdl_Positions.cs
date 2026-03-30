using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Positions
    {
        // Campos principales
        public int PositionId { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public string Description { get; set; }
        public int DepartmentId { get; set; }
        public string SalaryRange { get; set; }
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }

        // Constructor vacío
        public Mdl_Positions()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_Positions(string positionCode, string positionName, int departmentId)
        {
            this.PositionCode = positionCode;
            this.PositionName = positionName;
            this.DepartmentId = departmentId;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}