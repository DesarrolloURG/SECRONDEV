using System;

namespace SECRON.Models
{
    public class Mdl_Careers
    {
        public int CareerId { get; set; }
        public string CareerCode { get; set; }
        public string CareerName { get; set; }
        public string Description { get; set; }
        public int? DurationYears { get; set; }
        public int? TotalSemesters { get; set; }
        public int? TotalCredits { get; set; }

        // Control
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}