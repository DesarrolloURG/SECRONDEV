using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    public class Mdl_Security_SessionInfo
    {
        public Mdl_Security_UserInfo User { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LastActivity { get; set; }
        public string SessionId { get; set; }
        public bool IsActive { get; set; }

        public Mdl_Security_SessionInfo()
        {
            LoginTime = DateTime.Now;
            LastActivity = DateTime.Now;
            SessionId = Guid.NewGuid().ToString();
            IsActive = true;
        }

        public Mdl_Security_SessionInfo(Mdl_Security_UserInfo user) : this()
        {
            User = user;
        }

        // Método para verificar si la sesión ha expirado
        public bool IsExpired(int timeoutMinutes = 30)
        {
            return DateTime.Now.Subtract(LastActivity).TotalMinutes > timeoutMinutes;
        }

        // Método para actualizar la última actividad
        public void UpdateActivity()
        {
            LastActivity = DateTime.Now;
        }

        // Método para cerrar sesión
        public void CloseSession()
        {
            IsActive = false;
        }
    }
}
