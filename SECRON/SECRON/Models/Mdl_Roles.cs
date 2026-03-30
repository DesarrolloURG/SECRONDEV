using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Roles
    {
        // Campos principales
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        // Constructor vacío
        public Mdl_Roles()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_Roles(string roleName)
        {
            this.RoleName = roleName;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}