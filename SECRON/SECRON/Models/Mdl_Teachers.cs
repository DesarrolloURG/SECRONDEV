using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Teachers
    {
        public int TeacherId { get; set; }
        public string TeacherCode { get; set; }

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

        // Asignación de Sede principal
        public int LocationId { get; set; }

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

        public Mdl_Teachers()
        {
            IsActive = true;
            IsCollegiateActive = false;
            CreatedDate = DateTime.Now;
        }
    }
}