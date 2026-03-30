using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_TransferStatus
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public Mdl_TransferStatus()
        {
            IsActive = true;
        }

        public Mdl_TransferStatus(string statusName)
        {
            StatusName = statusName;
            IsActive = true;
        }
    }
}
