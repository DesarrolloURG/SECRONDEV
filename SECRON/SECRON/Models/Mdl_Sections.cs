using System;

namespace SECRON.Models
{
    public class Mdl_Sections
    {
        public int SectionId { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }

        public int CareerId { get; set; }
        public string CareerName { get; set; }        // Denormalizado, solo para mostrar en grid

        public int LocationId { get; set; }
        public string LocationName { get; set; }       // Denormalizado

        public int ScheduleTypeId { get; set; }
        public string ScheduleTypeName { get; set; }    // Denormalizado

        public int CoordinatorId { get; set; }
        public string CoordinatorName { get; set; }     // Denormalizado

        public int CurrentSemester { get; set; }
        public int? AcademicYear { get; set; }

        public int StudentCount { get; set; }
        public int MaxCapacity { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}