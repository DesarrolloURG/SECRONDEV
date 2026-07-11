using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Coordinators
    {
        public int CoordinatorId { get; set; }
        public string CoordinatorCode { get; set; }

        // Información Personal
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string DPI { get; set; }
        public string NIT { get; set; }
        public string Address { get; set; }

        // Información Académica
        public string AcademicTitle { get; set; }
        public string Specialization { get; set; }
        public bool IsCollegiateActive { get; set; }
        public string CollegiateNumber { get; set; }

        // Información Bancaria
        public string BankAccountNumber { get; set; }
        public int? BankId { get; set; }

        // Asignación de Sede principal (NULL: un coordinador puede no tener sede asignada aun)
        public int? LocationId { get; set; }

        // Información de contratación
        public DateTime? HireDate { get; set; }
        public string ContractType { get; set; }

        // Relación con Usuario (opcional)
        public int? UserId { get; set; }

        // Quién lo registró
        public int? RegisteredByCoordinatorId { get; set; }

        // Control
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Rutas de archivos
        public string FilePath_DPI { get; set; }
        public string FilePath_Titulos { get; set; }
        public string FilePath_RTU { get; set; }
        public string FilePath_Colegiado { get; set; }
        public string FilePath_RENAS { get; set; }
        public string FilePath_AntPoliciacos { get; set; }
        public string FilePath_AntPenales { get; set; }

        public Mdl_Coordinators()
        {
            IsActive = true;
            IsCollegiateActive = false;
            CreatedDate = DateTime.Now;
        }
    }
}