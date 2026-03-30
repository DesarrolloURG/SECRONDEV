using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Municipality
    {
        // Campos principales
        public int MunicipalityId { get; set; }
        public int DepartmentId { get; set; }
        public string MunicipalityName { get; set; }
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Constructor vacío
        public Mdl_Municipality()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_Municipality(int departmentId, string municipalityName)
        {
            this.DepartmentId = departmentId;
            this.MunicipalityName = municipalityName;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}