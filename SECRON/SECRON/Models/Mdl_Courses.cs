using System;

namespace SECRON.Models
{
    public class Mdl_Courses
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public int? TheoryHours { get; set; }
        public int? PracticeHours { get; set; }
        public int? LabHours { get; set; }
        public int? TotalHours { get; set; }   // Calculado por BD, solo lectura
        public int? Sessions { get; set; }
        public bool IsCommon { get; set; }

        // Control
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}