using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SECRON.Models.Mdl_Security_LoginStatus;

namespace SECRON.Models
{
    public class Mdl_Security_UserLoginResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Mdl_Security_UserInfo UserData { get; set; }
        public Mdl_Security_LoginStatus ErrorType { get; set; }
        public DateTime LoginAttemptTime { get; set; }
        public int RemainingAttempts { get; set; }

        // Constructor por defecto
        public Mdl_Security_UserLoginResult()
        {
            IsSuccess = false;
            Message = string.Empty;
            ErrorType = Mdl_Security_LoginStatus.None;
            LoginAttemptTime = DateTime.Now;
            RemainingAttempts = 0;
        }

        // Constructor para resultado exitoso
        public Mdl_Security_UserLoginResult(Mdl_Security_UserInfo userData, string message = "Inicio de sesión exitoso")
        {
            IsSuccess = true;
            Message = message;
            UserData = userData;
            ErrorType = Mdl_Security_LoginStatus.None;
            LoginAttemptTime = DateTime.Now;
            RemainingAttempts = 0;
        }

        // Constructor para resultado fallido
        public Mdl_Security_UserLoginResult(Mdl_Security_LoginStatus errorType, string message, int remainingAttempts = 0)
        {
            IsSuccess = false;
            Message = message;
            ErrorType = errorType;
            LoginAttemptTime = DateTime.Now;
            RemainingAttempts = remainingAttempts;
        }

        // Método para verificar si es un error crítico que requiere acción especial
        public bool IsCriticalError()
        {
            return ErrorType == Mdl_Security_LoginStatus.MaxAttemptsReached ||
                   ErrorType == Mdl_Security_LoginStatus.UserLocked;
        }

        // Método para verificar si requiere cambio de contraseña
        public bool RequiresPasswordChange()
        {
            return ErrorType == Mdl_Security_LoginStatus.PasswordExpired && IsSuccess;
        }
    }
}
