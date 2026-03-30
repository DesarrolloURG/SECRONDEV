using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Departments
    {
        // Campos principales
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public int? ManagerEmployeeId { get; set; }
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Constructor vacío
        public Mdl_Departments()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_Departments(int locationId, string departmentCode, string departmentName)
        {
            this.LocationId = locationId;
            this.DepartmentCode = departmentCode;
            this.DepartmentName = departmentName;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}