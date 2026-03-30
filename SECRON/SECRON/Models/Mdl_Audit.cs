using System;

namespace SECRON.Models
{
    internal class Mdl_Audit
    {
        public int AuditId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public int? RecordId { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public DateTime ActionDate { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }

        public Mdl_Audit()
        {
            ActionDate = DateTime.Now;
        }
    }
}