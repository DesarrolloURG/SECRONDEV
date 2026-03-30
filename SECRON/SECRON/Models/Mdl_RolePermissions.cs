using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_RolePermissions
    {
        // Campos principales
        public int RolePermissionId { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public bool IsGranted { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        // Constructor vacío
        public Mdl_RolePermissions()
        {
            IsGranted = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_RolePermissions(int roleId, int permissionId)
        {
            this.RoleId = roleId;
            this.PermissionId = permissionId;
            this.IsGranted = true;
            this.CreatedDate = DateTime.Now;
        }

        // Constructor completo
        public Mdl_RolePermissions(int roleId, int permissionId, bool isGranted, int? createdBy)
        {
            this.RoleId = roleId;
            this.PermissionId = permissionId;
            this.IsGranted = isGranted;
            this.CreatedBy = createdBy;
            this.CreatedDate = DateTime.Now;
        }
    }
}