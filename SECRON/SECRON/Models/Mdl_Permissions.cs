using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Permissions
    {
        // Campos principales
        public int PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }
        public string ModuleName { get; set; }
        public string ActionType { get; set; }
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }

        // Constructor vacío
        public Mdl_Permissions()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_Permissions(string permissionCode, string permissionName, string moduleName, string actionType)
        {
            this.PermissionCode = permissionCode;
            this.PermissionName = permissionName;
            this.ModuleName = moduleName;
            this.ActionType = actionType;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}