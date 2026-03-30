using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Banks
    {
        // Campos principales
        public int BankId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public bool IsActive { get; set; }

        // Constructor vacío
        public Mdl_Banks()
        {
            IsActive = true;
        }

        // Constructor con parámetros
        public Mdl_Banks(string bankCode, string bankName)
        {
            this.BankCode = bankCode;
            this.BankName = bankName;
            this.IsActive = true;
        }
    }
}