using System;

namespace SECRON.Models
{
    public class Mdl_ResponsibilityLetterMaster
    {
        public int ResponsibilityLetterId { get; set; }
        public int EmployeeId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public int UploadedByUserId { get; set; }
        public bool IsActive { get; set; }
    }
}