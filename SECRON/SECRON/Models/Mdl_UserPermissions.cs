using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_UserPermissions
    {
        // Campos principales
        public int UserPermissionId { get; set; }
        public int UserId { get; set; }
        public int PermissionId { get; set; }
        public bool IsGranted { get; set; }

        // Auditoría
        public DateTime GrantedDate { get; set; }
        public int GrantedBy { get; set; }

        // Constructor vacío
        public Mdl_UserPermissions()
        {
            GrantedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_UserPermissions(int userId, int permissionId, bool isGranted, int grantedBy)
        {
            this.UserId = userId;
            this.PermissionId = permissionId;
            this.IsGranted = isGranted;
            this.GrantedBy = grantedBy;
            this.GrantedDate = DateTime.Now;
        }
    }
}