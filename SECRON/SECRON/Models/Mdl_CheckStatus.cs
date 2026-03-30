using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_CheckStatus
    {
        // Campos principales
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Constructor vacío
        public Mdl_CheckStatus()
        {
            IsActive = true;
        }

        // Constructor con parámetros
        public Mdl_CheckStatus(string statusName)
        {
            this.StatusName = statusName;
            this.IsActive = true;
        }
    }
}