using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_AccountingEntryChecks
    {
        public int EntryMasterId { get; set; }
        public int CheckId { get; set; }

        public Mdl_AccountingEntryChecks()
        {
        }

        public Mdl_AccountingEntryChecks(int entryMasterId, int checkId)
        {
            EntryMasterId = entryMasterId;
            CheckId = checkId;
        }
    }
}
