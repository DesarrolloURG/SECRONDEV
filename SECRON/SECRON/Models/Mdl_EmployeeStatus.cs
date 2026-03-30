using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_EmployeeStatus
    {
        // Campos principales
        public int EmployeeStatusId { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Constructor vacío
        public Mdl_EmployeeStatus()
        {
            IsActive = true;
        }

        // Constructor con parámetros principales
        public Mdl_EmployeeStatus(string statusName)
        {
            this.StatusName = statusName;
            this.IsActive = true;
        }
    }
}