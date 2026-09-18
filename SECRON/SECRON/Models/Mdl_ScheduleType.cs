using System;

namespace SECRON.Models
{
    public class Mdl_ScheduleType
    {
        public int ScheduleTypeId { get; set; }
        public string ScheduleTypeCode { get; set; }
        public string ScheduleTypeName { get; set; }
        public string Description { get; set; }

        public bool IncludesMonday { get; set; }
        public bool IncludesTuesday { get; set; }
        public bool IncludesWednesday { get; set; }
        public bool IncludesThursday { get; set; }
        public bool IncludesFriday { get; set; }
        public bool IncludesSaturday { get; set; }
        public bool IncludesSunday { get; set; }

        public string TimeShift { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}