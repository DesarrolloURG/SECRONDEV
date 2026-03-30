using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace SECRON.Models
{
    public class Mdl_AudithLog_LoginResult
    {
        public int AuditId { get; set; }
        public int? UserId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public int? RecordId { get; set; }
        public string OldValues { get; set; }       // JSON con valores anteriores
        public string NewValues { get; set; }       // JSON con valores nuevos
        public DateTime ActionDate { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }

        // Propiedades adicionales para facilitar el uso (no mapean a BD)
        [JsonIgnore]
        public string Username { get; set; }        // Para mostrar en reportes

        public Mdl_AudithLog_LoginResult()
        {
            ActionDate = DateTime.Now;
        }

        // Constructor completo
        public Mdl_AudithLog_LoginResult(int? userId, string action, string tableName = null, int? recordId = null)
        {
            UserId = userId;
            Action = action;
            TableName = tableName;
            RecordId = recordId;
            ActionDate = DateTime.Now;
        }

        // Métodos para serializar objetos a JSON
        public void SetOldValues<T>(T oldObject)
        {
            OldValues = oldObject != null ? Newtonsoft.Json.JsonConvert.SerializeObject(oldObject, Newtonsoft.Json.Formatting.Indented) : null;
        }

        public void SetNewValues<T>(T newObject)
        {
            NewValues = newObject != null ? Newtonsoft.Json.JsonConvert.SerializeObject(newObject, Newtonsoft.Json.Formatting.Indented) : null;
        }

        // Métodos para deserializar JSON a objetos
        public T GetOldValues<T>() where T : class
        {
            return string.IsNullOrEmpty(OldValues) ? null : JsonConvert.DeserializeObject<T>(OldValues);
        }

        public T GetNewValues<T>() where T : class
        {
            return string.IsNullOrEmpty(NewValues) ? null : JsonConvert.DeserializeObject<T>(NewValues);
        }

        // Métodos estáticos para crear logs específicos

        // Para auditoría de login
        public static Mdl_AudithLog_LoginResult CreateLoginAudit(int? userId, bool isSuccess, string ipAddress = "", string userAgent = "")
        {
            return new Mdl_AudithLog_LoginResult
            {
                UserId = userId,
                Action = isSuccess ? "LOGIN_SUCCESS" : "LOGIN_FAILED",
                TableName = "Users",
                RecordId = userId,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                ActionDate = DateTime.Now
            };
        }

        // Para auditoría de logout
        public static Mdl_AudithLog_LoginResult CreateLogoutAudit(int userId, string ipAddress = "", string userAgent = "")
        {
            return new Mdl_AudithLog_LoginResult
            {
                UserId = userId,
                Action = "LOGOUT",
                TableName = "Users",
                RecordId = userId,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                ActionDate = DateTime.Now
            };
        }

        // Para auditoría de cambio de contraseña
        public static Mdl_AudithLog_LoginResult CreatePasswordChangeAudit(int userId, string ipAddress = "", string userAgent = "")
        {
            return new Mdl_AudithLog_LoginResult
            {
                UserId = userId,
                Action = "PASSWORD_CHANGE",
                TableName = "Users",
                RecordId = userId,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                ActionDate = DateTime.Now
            };
        }

        // Para auditoría de bloqueo/desbloqueo de usuario
        public static Mdl_AudithLog_LoginResult CreateUserLockAudit(int? actionUserId, int targetUserId, bool isLocked, string ipAddress = "", string userAgent = "")
        {
            return new Mdl_AudithLog_LoginResult
            {
                UserId = actionUserId,
                Action = isLocked ? "USER_LOCKED" : "USER_UNLOCKED",
                TableName = "Users",
                RecordId = targetUserId,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                ActionDate = DateTime.Now
            };
        }

        // Para auditoría genérica de CRUD
        public static Mdl_AudithLog_LoginResult CreateCrudAudit<T>(int? userId, string action, string tableName, int? recordId,
            T oldValues = null, T newValues = null, string ipAddress = "", string userAgent = "") where T : class
        {
            var audit = new Mdl_AudithLog_LoginResult
            {
                UserId = userId,
                Action = action, // INSERT, UPDATE, DELETE
                TableName = tableName,
                RecordId = recordId,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                ActionDate = DateTime.Now
            };

            if (oldValues != null)
                audit.SetOldValues(oldValues);

            if (newValues != null)
                audit.SetNewValues(newValues);

            return audit;
        }

        // Método para obtener descripción legible de la acción
        public string GetActionDescription()
        {
            switch (Action)
            {
                case "LOGIN_SUCCESS":
                    return "Inicio de sesión exitoso";
                case "LOGIN_FAILED":
                    return "Intento de inicio de sesión fallido";
                case "LOGOUT":
                    return "Cierre de sesión";
                case "PASSWORD_CHANGE":
                    return "Cambio de contraseña";
                case "USER_LOCKED":
                    return "Usuario bloqueado";
                case "USER_UNLOCKED":
                    return "Usuario desbloqueado";
                case "INSERT":
                    return "Registro creado";
                case "UPDATE":
                    return "Registro actualizado";
                case "DELETE":
                    return "Registro eliminado";
                default:
                    return Action;
            }
        }

        // Método para obtener resumen de cambios (útil para reportes)
        public string GetChangesSummary()
        {
            if (string.IsNullOrEmpty(OldValues) && string.IsNullOrEmpty(NewValues))
                return "Sin cambios registrados";

            if (Action == "INSERT")
                return "Nuevo registro creado";

            if (Action == "DELETE")
                return "Registro eliminado";

            if (Action == "UPDATE")
            {
                // Podrías implementar lógica más sofisticada aquí
                // para comparar OldValues y NewValues y mostrar qué cambió
                return "Registro modificado";
            }

            return GetActionDescription();
        }

    }
}
