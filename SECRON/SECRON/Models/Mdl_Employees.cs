using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Employees
    {
        // Campos principales
        public int EmployeeId { get; set; }
        public int? LocationId { get; set; }           
        public string TipoContratacion { get; set; }   
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; } // Campo calculado
        public string IdentificationNumber { get; set; } // DPI/Cédula

        // Información de contacto
        public string Email { get; set; }
        public string InstitutionalEmail { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Address { get; set; }

        // Fechas
        public DateTime? BirthDate { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        // Referencias a otras tablas
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public int? DirectSupervisorId { get; set; }
        public int EmployeeStatusId { get; set; }

        // Contacto de emergencia
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string EmergencyContactRelation { get; set; }

        // Información laboral
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        //Informacion salarial
        public decimal? NominalSalary { get; set; }
        public decimal? BaseSalary { get; set; }
        public decimal? AdditionalBonus { get; set; }
        public decimal? LegalBonus { get; set; }
        public decimal? IGSS { get; set; }
        public decimal? ISR { get; set; }
        public decimal? NetSalary { get; set; }
        public bool? IGSSManual { get; set; }

        // Constructor vacío
        public Mdl_Employees()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_Employees(string employeeCode, string firstName, string lastName,
            string identificationNumber, string institutionalEmail, DateTime hireDate,
            int departmentId, int positionId, int employeeStatusId)
        {
            this.EmployeeCode = employeeCode;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.IdentificationNumber = identificationNumber;
            this.InstitutionalEmail = institutionalEmail;
            this.HireDate = hireDate;
            this.DepartmentId = departmentId;
            this.PositionId = positionId;
            this.EmployeeStatusId = employeeStatusId;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}