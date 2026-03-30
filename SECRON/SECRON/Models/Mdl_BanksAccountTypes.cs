using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_BanksAccountTypes
    {
        public int BanksAccountTypeId { get; set; }
        public string BanksAccountTypeCode { get; set; }   // MON, AHO, etc.
        public string BanksAccountTypeName { get; set; }   // MONETARIA, AHORRO, etc.
        public bool IsActive { get; set; }

        public Mdl_BanksAccountTypes()
        {
            IsActive = true;
        }

        public override string ToString()
        {
            // Para combos: "MON - MONETARIA"
            return $"{BanksAccountTypeCode} - {BanksAccountTypeName}";
        }
    }
}
