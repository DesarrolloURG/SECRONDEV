using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_CheckControl
    {
        public int CheckControlId { get; set; }
        public int UserId { get; set; }
        public int InitialLimit { get; set; }
        public int FinalLimit { get; set; }
        public int CurrentCounter { get; set; }
        public bool Priority { get; set; }
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public Mdl_CheckControl()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        public Mdl_CheckControl(int userId, int initialLimit, int finalLimit, int currentCounter)
        {
            this.UserId = userId;
            this.InitialLimit = initialLimit;
            this.FinalLimit = finalLimit;
            this.CurrentCounter = currentCounter;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}