using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_AccountingEntryDetails
    {
        public int EntryDetailId { get; set; }
        public int EntryMasterId { get; set; }
        public int AccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Remarks { get; set; }

        public Mdl_AccountingEntryDetails()
        {
            Debit = 0;
            Credit = 0;
        }
    }
}