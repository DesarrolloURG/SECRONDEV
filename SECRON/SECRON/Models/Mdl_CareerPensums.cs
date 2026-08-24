using System;

namespace SECRON.Models
{
    public class Mdl_CareerPensums
    {
        public int CareerPensumId { get; set; }
        public int CareerId { get; set; }
        public string PensumCode { get; set; }
        public string PensumName { get; set; }

        // Datos denormalizados de Careers, solo para mostrar en grillas sin JOIN
        public string CareerCode { get; set; }
        public string CareerName { get; set; }

        public bool IsCurrent { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}