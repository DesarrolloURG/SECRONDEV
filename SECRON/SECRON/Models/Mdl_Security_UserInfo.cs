using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    public class Mdl_Security_UserInfo
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
        public bool IsTemporaryPassword { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public string InstitutionalEmail { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedDate { get; set; }

        // Propiedades adicionales útiles
        public string RoleName { get; set; }        // Se puede cargar con JOIN
        public string StatusName { get; set; }     // Se puede cargar con JOIN
        public bool NotificationsEnabled { get; set; }

        // Constructor vacío
        public Mdl_Security_UserInfo()
        {
        }

        // Constructor con parámetros básicos
        public Mdl_Security_UserInfo(int userId, string username, string fullName, int roleId)
        {
            UserId = userId;
            Username = username;
            FullName = fullName;
            RoleId = roleId;
        }

        // Método para verificar si la contraseña ha expirado
        public bool IsPasswordExpired()
        {
            return PasswordExpiryDate.HasValue && PasswordExpiryDate.Value <= DateTime.Now;
        }

        // Método para obtener nombre para mostrar
        public string GetDisplayName()
        {
            return !string.IsNullOrWhiteSpace(FullName) ? FullName : Username;
        }
    }
}
