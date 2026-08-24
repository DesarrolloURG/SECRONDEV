using System;

namespace SECRON.Models
{
    public class Mdl_CareerCourses
    {
        public int CareerCourseId { get; set; }
        public int CareerPensumId { get; set; }
        public int CourseId { get; set; }
        public int Semester { get; set; }
        public bool IsRequired { get; set; }
        public decimal StandardPrice { get; set; }
        // Datos denormalizados del curso, solo para mostrar en grillas/combos sin JOIN adicional
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}