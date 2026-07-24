using System;

namespace SECRON.Models
{
    public class Mdl_DocentesTemporal_Cursos
    {
        public int TeacherTempCourseId { get; set; }
        public int TeacherTempId { get; set; }
        public string AcademicLocation { get; set; }
        public string CourseToTeach { get; set; }
        public string Schedule { get; set; }
        public decimal? Fees { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
