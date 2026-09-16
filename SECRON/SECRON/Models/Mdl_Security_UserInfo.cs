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

        // Roles del usuario (reemplaza el antiguo RoleId/RoleName único — un usuario puede tener varios roles)
        public List<KeyValuePair<int, string>> Roles { get; set; } = new List<KeyValuePair<int, string>>();

        // Lista de RoleId, útil para checks de permisos o comparaciones rápidas
        public List<int> RoleIds => Roles?.Select(r => r.Key).ToList() ?? new List<int>();

        // Texto para mostrar en pantalla: nombres de rol separados por coma
        public string RoleNamesText => Roles != null && Roles.Count > 0
            ? string.Join(", ", Roles.Select(r => r.Value))
            : "";

        public int StatusId { get; set; }
        public bool IsTemporaryPassword { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public string InstitutionalEmail { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastPasswordChanged { get; set; }
        public bool PasswordNeverExpires { get; set; }
        public string TwoFactorSecret { get; set; }
        public DateTime? TwoFactorEnabledDate { get; set; }
        public bool TwoFactorExempt { get; set; }

        // Propiedades adicionales útiles
        public string StatusName { get; set; }     // Se puede cargar con JOIN
        public bool NotificationsEnabled { get; set; }

        // Constructor vacío
        public Mdl_Security_UserInfo()
        {
        }

        // Constructor con parámetros básicos
        public Mdl_Security_UserInfo(int userId, string username, string fullName)
        {
            UserId = userId;
            Username = username;
            FullName = fullName;
        }

        // Método para verificar si la contraseña ha expirado (por fecha de expiración explícita, ej. temporal)
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