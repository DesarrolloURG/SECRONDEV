using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Department
    {
        // Campos principales
        public int DepartmentId { get; set; }
        public int CountryId { get; set; }
        public string DepartmentName { get; set; }
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Constructor vacío
        public Mdl_Department()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_Department(int countryId, string departmentName)
        {
            this.CountryId = countryId;
            this.DepartmentName = departmentName;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}