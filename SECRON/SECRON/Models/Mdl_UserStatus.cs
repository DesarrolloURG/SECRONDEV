using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_UserStatus
    {
        // Campos principales
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Constructor vacío
        public Mdl_UserStatus()
        {
            IsActive = true;
        }

        // Constructor con parámetros principales
        public Mdl_UserStatus(string statusName)
        {
            this.StatusName = statusName;
            this.IsActive = true;
        }
    }
}