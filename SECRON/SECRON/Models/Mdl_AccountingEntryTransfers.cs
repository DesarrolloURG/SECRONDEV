using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_AccountingEntryTransfers
    {
        public int EntryMasterId { get; set; }
        public int TransferId { get; set; }

        public Mdl_AccountingEntryTransfers()
        {
        }

        public Mdl_AccountingEntryTransfers(int entryMasterId, int transferId)
        {
            EntryMasterId = entryMasterId;
            TransferId = transferId;
        }
    }
}
