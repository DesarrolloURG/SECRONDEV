using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_AccountingEntryMaster
    {
        public int EntryMasterId { get; set; }
        public DateTime EntryDate { get; set; }
        public string Concept { get; set; }
        public int StatusId { get; set; }
        public decimal TotalAmount { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        public Mdl_AccountingEntryMaster()
        {
            EntryDate = DateTime.Now;
            CreatedDate = DateTime.Now;
            IsActive = true;
            TotalAmount = 0;
        }
    }
}