using System;

namespace SECRON.Models
{
    public class Mdl_DocentesTemporal
    {
        public int TeacherTempId { get; set; }
        public string ContractCode { get; set; }
        public string DPI { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string MaritalStatus { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Nationality { get; set; }
        public string CollegiateNumber { get; set; }
        public string NIT { get; set; }
        public string Cycle { get; set; }
        public int? ContractYear { get; set; }
        public DateTime? IssueDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        // Propiedad de conveniencia (viene de SP_DocentesTemporal_Select, no es columna de la tabla)
        public int TotalCursos { get; set; }
    }
}
