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
        public int RoleId { get; set; }
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
        public int? EmployeeId { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public int FailedLoginAttempts { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LastLoginDate { get; set; }

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
        public Mdl_Users(string username, string passwordHash, string fullName, int roleId, int statusId)
        {
            this.Username = username;
            this.PasswordHash = passwordHash;
            this.FullName = fullName;
            this.RoleId = roleId;
            this.StatusId = statusId;
            this.NotificationsEnabled = true;
            this.IsTemporaryPassword = false;
            this.FailedLoginAttempts = 0;
            this.IsLocked = false;
            this.CreatedDate = DateTime.Now;
        }
    }
}