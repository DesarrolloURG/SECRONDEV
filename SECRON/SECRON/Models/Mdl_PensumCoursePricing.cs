using System;

namespace SECRON.Models
{
    public class Mdl_PensumCoursePricing
    {
        public int CareerCourseId { get; set; }
        public int Semester { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public decimal StandardPrice { get; set; }

        // Si ya existe un precio real para esta combinación Sede+Modalidad
        public int? CourseLocationPricingId { get; set; }
        public decimal? CurrentPrice { get; set; }
    }
}