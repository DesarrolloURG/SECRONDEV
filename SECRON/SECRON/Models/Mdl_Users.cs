using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Users
    {
        // Campos principales
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }

        // Roles del usuario (reemplaza el antiguo RoleId único — un usuario puede tener varios roles)
        public List<KeyValuePair<int, string>> Roles { get; set; } = new List<KeyValuePair<int, string>>();
        public List<int> RoleIds => Roles?.Select(r => r.Key).ToList() ?? new List<int>();
        public string RoleNamesText => Roles != null && Roles.Count > 0
            ? string.Join(", ", Roles.Select(r => r.Value))
            : "";

        public int StatusId { get; set; }
        public bool NotificationsEnabled { get; set; }
        public DateTime? LastConnectionDate { get; set; }
        public bool IsTemporaryPassword { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Campos adicionales
        public string InstitutionalEmail { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public int FailedLoginAttempts { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChanged { get; set; }
        public bool PasswordNeverExpires { get; set; }

        // Constructor vacío
        public Mdl_Users()
        {
            NotificationsEnabled = true;
            IsTemporaryPassword = false;
            FailedLoginAttempts = 0;
            IsLocked = false;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_Users(string username, string passwordHash, string fullName, int statusId)
        {
            this.Username = username;
            this.PasswordHash = passwordHash;
            this.FullName = fullName;
            this.StatusId = statusId;
            this.NotificationsEnabled = true;
            this.IsTemporaryPassword = false;
            this.FailedLoginAttempts = 0;
            this.IsLocked = false;
            this.CreatedDate = DateTime.Now;
        }
    }
}