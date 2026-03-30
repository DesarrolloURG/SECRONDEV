using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    public enum Mdl_Security_LoginStatus
    {
        None,                   // Sin error
        UserNotFound,          // Usuario no encontrado
        InvalidPassword,       // Contraseña incorrecta
        UserDisabled,          // Usuario deshabilitado
        UserLocked,            // Usuario bloqueado por intentos fallidos
        PasswordExpired,       // Contraseña temporal que debe cambiarse
        MaxAttemptsReached     // Se alcanzó el máximo de intentos
    }
}