using System;

namespace SECRON.Models
{
    public class Mdl_LocationCareers
    {
        public int LocationCareerId { get; set; }
        public int LocationId { get; set; }
        public int CareerId { get; set; }

        // Datos denormalizados de Careers, solo para mostrar en grillas sin hacer JOIN en cada consulta
        public string CareerCode { get; set; }
        public string CareerName { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}