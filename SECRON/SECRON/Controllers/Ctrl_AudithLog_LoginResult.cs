using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // Agregado para Application
using SECRON.Models; // Para usar Mdl_AuditLog_LoginResult

namespace SECRON.Controllers
{
    public class Ctrl_AuditLog
    {
        private readonly string connectionString;

        public Ctrl_AuditLog()
        {
            // Obtener connection string desde Configuration
            connectionString = Configuration.DatabaseConfig.GetConnectionString();
        }

        // Método principal para registrar auditoría
        public async Task<bool> LogAuditAsync(Mdl_AudithLog_LoginResult auditLog)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                        INSERT INTO AuditLog (UserId, Action, TableName, RecordId, OldValues, NewValues, ActionDate, IPAddress, UserAgent)
                        VALUES (@UserId, @Action, @TableName, @RecordId, @OldValues, @NewValues, @ActionDate, @IPAddress, @UserAgent)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", auditLog.UserId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Action", auditLog.Action ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TableName", auditLog.TableName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@RecordId", auditLog.RecordId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@OldValues", auditLog.OldValues ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@NewValues", auditLog.NewValues ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ActionDate", auditLog.ActionDate);
                        command.Parameters.AddWithValue("@IPAddress", auditLog.IPAddress ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@UserAgent", auditLog.UserAgent ?? (object)DBNull.Value);

                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log crítico - no mostrar al usuario pero registrar en sistema
                System.Diagnostics.Debug.WriteLine($"Error registrando auditoría: {ex.Message}");

                // Opcional: registrar en archivo de log como fallback
                LogToFile($"Error AuditLog: {ex.Message} - Action: {auditLog.Action}");
                return false;
            }
        }

        // Método síncrono para compatibilidad
        public bool LogAudit(Mdl_AudithLog_LoginResult auditLog)
        {
            try
            {
                return LogAuditAsync(auditLog).Result;
            }
            catch
            {
                return false;
            }
        }

        // Métodos específicos para diferentes tipos de auditoría

        public async Task<bool> LogLoginAttemptAsync(int? userId, bool isSuccess, string username = "")
        {
            var audit = Mdl_AudithLog_LoginResult.CreateLoginAudit(userId, isSuccess, GetLocalIPAddress(), GetUserAgent());

            // Agregar username para referencia
            audit.Username = username;

            // Si es fallido y no tenemos userId, crear log especial
            if (!isSuccess && !userId.HasValue && !string.IsNullOrEmpty(username))
            {
                audit.SetNewValues(new { AttemptedUsername = username, Success = isSuccess, Reason = "Usuario no encontrado o credenciales inválidas" });
            }

            return await LogAuditAsync(audit);
        }

        public async Task<bool> LogLogoutAsync(int userId, string username = "")
        {
            var audit = Mdl_AudithLog_LoginResult.CreateLogoutAudit(userId, GetLocalIPAddress(), GetUserAgent());
            audit.Username = username;
            return await LogAuditAsync(audit);
        }

        public async Task<bool> LogPasswordChangeAsync(int userId, string username = "", bool isTemporary = false)
        {
            var audit = Mdl_AudithLog_LoginResult.CreatePasswordChangeAudit(userId, GetLocalIPAddress(), GetUserAgent());
            audit.Username = username;
            audit.SetNewValues(new { IsTemporary = isTemporary, ChangeDate = DateTime.Now });
            return await LogAuditAsync(audit);
        }

        public async Task<bool> LogUserLockAsync(int? actionUserId, int targetUserId, bool isLocked, string reason = "")
        {
            var audit = Mdl_AudithLog_LoginResult.CreateUserLockAudit(actionUserId, targetUserId, isLocked, GetLocalIPAddress(), GetUserAgent());
            audit.SetNewValues(new { IsLocked = isLocked, Reason = reason, LockDate = DateTime.Now });
            return await LogAuditAsync(audit);
        }

        // Método genérico para operaciones CRUD
        public async Task<bool> LogCrudOperationAsync<T>(int? userId, string action, string tableName, int? recordId,
            T oldValues = null, T newValues = null) where T : class
        {
            var audit = Mdl_AudithLog_LoginResult.CreateCrudAudit(userId, action, tableName, recordId, oldValues, newValues,
                GetLocalIPAddress(), GetUserAgent());
            return await LogAuditAsync(audit);
        }

        // Métodos de utilidad

        private string GetLocalIPAddress()
        {
            try
            {
                // Para aplicaciones desktop, obtenemos la IP local
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                return ip.Address.ToString();
                            }
                        }
                    }
                }
                return "127.0.0.1"; // Fallback
            }
            catch
            {
                return "Unknown";
            }
        }

        private string GetUserAgent()
        {
            try
            {
                // Para Windows Forms, creamos un user agent descriptivo
                var version = Application.ProductVersion ?? "1.0";
                var osVersion = Environment.OSVersion.ToString();
                return $"SECRON Desktop v{version} ({osVersion})";
            }
            catch
            {
                return "SECRON Desktop Application";
            }
        }

        private void LogToFile(string message)
        {
            try
            {
                var logPath = System.IO.Path.Combine(Application.StartupPath, "Logs");
                if (!System.IO.Directory.Exists(logPath))
                    System.IO.Directory.CreateDirectory(logPath);

                var fileName = $"audit_errors_{DateTime.Now:yyyyMMdd}.log";
                var fullPath = System.IO.Path.Combine(logPath, fileName);

                var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                System.IO.File.AppendAllText(fullPath, logEntry);
            }
            catch
            {
                // Si no podemos escribir al archivo, no hay mucho más que hacer
            }
        }

        // Método para limpiar logs antiguos (opcional)
        public async Task<bool> CleanOldLogsAsync(int daysToKeep = 90)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                        DELETE FROM AuditLog 
                        WHERE ActionDate < @cutoffDate";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@cutoffDate", DateTime.Now.AddDays(-daysToKeep));
                        var deletedRows = await command.ExecuteNonQueryAsync();

                        // Log la operación de limpieza
                        if (deletedRows > 0)
                        {
                            var cleanupAudit = new Mdl_AudithLog_LoginResult
                            {
                                Action = "CLEANUP_AUDIT_LOG",
                                TableName = "AuditLog",
                                ActionDate = DateTime.Now,
                                IPAddress = GetLocalIPAddress(),
                                UserAgent = GetUserAgent()
                            };
                            cleanupAudit.SetNewValues(new { DeletedRows = deletedRows, CutoffDate = DateTime.Now.AddDays(-daysToKeep) });

                            await LogAuditAsync(cleanupAudit);
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogToFile($"Error limpiando logs antiguos: {ex.Message}");
                return false;
            }
        }
    }
}