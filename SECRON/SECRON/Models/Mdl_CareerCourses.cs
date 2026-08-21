using System;

namespace SECRON.Models
{
    public class Mdl_CareerCourses
    {
        public int CareerCourseId { get; set; }
        public int CareerId { get; set; }
        public int CourseId { get; set; }

        // Datos denormalizados de Courses, solo para mostrar en grillas sin hacer JOIN en cada consulta
        public string CourseCode { get; set; }
        public string CourseName { get; set; }

        // Opcionales: no siempre se conocen al momento de la carga masiva
        public int? Semester { get; set; }
        public bool? IsRequired { get; set; }
        public string Prerequisites { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}