using System;

namespace SECRON.Models
{
    public class Mdl_CourseModalities
    {
        public int ModalityId { get; set; }
        public string ModalityCode { get; set; }
        public string ModalityName { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}