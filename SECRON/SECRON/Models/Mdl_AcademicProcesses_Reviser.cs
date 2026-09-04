using System;

namespace SECRON.Models
{
    internal class Mdl_AcademicProcesses_Reviser
    {
        public int ReviserId { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string UserFullName { get; set; }

        public string PersonType { get; set; }   // DOCENTE | TRABAJADOR | PROVEEDOR | COORDINADOR
        public int PersonId { get; set; }
        public string PersonCode { get; set; }
        public string PersonName { get; set; }

        public bool IsActive { get; set; }          // Revisor activo (AcademicProcesses_Revisers.IsActive)
        public bool PersonIsActive { get; set; }     // Estado de la ficha origen (Teachers/Employees/Suppliers/Coordinators)

        public int? AssignedBy { get; set; }
        public string AssignedByName { get; set; }
        public DateTime AssignedDate { get; set; }

        public int? RemovedBy { get; set; }
        public string RemovedByName { get; set; }
        public DateTime? RemovedDate { get; set; }

        public int TotalRegistros { get; set; } // Solo para paginación, no se persiste

        public Mdl_AcademicProcesses_Reviser()
        {
            IsActive = true;
        }
    }
}