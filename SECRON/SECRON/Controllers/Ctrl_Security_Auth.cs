using BCrypt.Net;
using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Controllers
{
    public class Ctrl_Security_Auth
    {
        #region PropiedadesIniciales
        private readonly string connectionString;
        private readonly Ctrl_AuditLog auditController;
        private const int MAX_LOGIN_ATTEMPTS = 3;
        public Ctrl_Security_Auth()
        {
            connectionString = DatabaseConfig.GetConnectionString();
            auditController = new Ctrl_AuditLog();
        }
        #endregion PropiedadesIniciales
        #region MetodosPrivados
        // Valida las credenciales del usuario y maneja toda la lógica de autenticación
        public async Task<Mdl_Security_UserLoginResult> ValidateUserAsync(string username, string password)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // 1. Verificar si el usuario existe y obtener su información
                    var userInfo = await GetUserInfoAsync(connection, username);

                    if (userInfo == null)
                    {
                        // Log intento fallido
                        await auditController.LogLoginAttemptAsync(null, false, username);

                        return new Mdl_Security_UserLoginResult(
                            Mdl_Security_LoginStatus.UserNotFound,
                            "Usuario no encontrado"
                        );
                    }

                    // 2. Verificar si el usuario está activo
                    if (!await IsUserActiveAsync(connection, userInfo.UserId))
                    {
                        await auditController.LogLoginAttemptAsync(userInfo.UserId, false, username);

                        return new Mdl_Security_UserLoginResult(
                            Mdl_Security_LoginStatus.UserDisabled,
                            "Tu usuario se encuentra inhabilitado. Comunícate con el administrador."
                        );
                    }

                    // 3. Verificar si el usuario está bloqueado
                    if (await IsUserLockedAsync(connection, userInfo.UserId))
                    {
                        await auditController.LogLoginAttemptAsync(userInfo.UserId, false, username);

                        return new Mdl_Security_UserLoginResult(
                            Mdl_Security_LoginStatus.UserLocked,
                            "Usuario bloqueado. Comunícate con un administrador."
                        );
                    }

                    // 4. Validar contraseña
                    if (!await ValidatePasswordAsync(connection, userInfo.UserId, password))
                    {
                        // Incrementar intentos fallidos
                        await IncrementFailedAttemptsAsync(connection, userInfo.UserId);

                        var failedAttempts = await GetFailedAttemptsAsync(connection, userInfo.UserId);
                        var remainingAttempts = MAX_LOGIN_ATTEMPTS - failedAttempts;

                        // Log intento fallido
                        await auditController.LogLoginAttemptAsync(userInfo.UserId, false, username);

                        if (failedAttempts >= MAX_LOGIN_ATTEMPTS)
                        {
                            await LockUserAsync(connection, userInfo.UserId);
                            await auditController.LogUserLockAsync(userInfo.UserId, userInfo.UserId, true, "Máximo de intentos de login superado");

                            return new Mdl_Security_UserLoginResult(
                                Mdl_Security_LoginStatus.MaxAttemptsReached,
                                "Usuario bloqueado. Ha superado el número de intentos permitidos.",
                                0
                            );
                        }
                        else
                        {
                            return new Mdl_Security_UserLoginResult(
                                Mdl_Security_LoginStatus.InvalidPassword,
                                $"Contraseña incorrecta. Te quedan {remainingAttempts} intentos.",
                                remainingAttempts
                            );
                        }
                    }

                    // 5. Login exitoso - resetear intentos y actualizar fechas
                    await ResetFailedAttemptsAsync(connection, userInfo.UserId);
                    await UpdateLastLoginAsync(connection, userInfo.UserId);

                    // Log login exitoso
                    await auditController.LogLoginAttemptAsync(userInfo.UserId, true, username);

                    // 6. Verificar si tiene contraseña temporal
                    if (userInfo.IsTemporaryPassword)
                    {
                        return new Mdl_Security_UserLoginResult(userInfo, "Debe cambiar su contraseña temporal")
                        {
                            ErrorType = Mdl_Security_LoginStatus.PasswordExpired
                        };
                    }

                    // 7. Login completamente exitoso
                    return new Mdl_Security_UserLoginResult(userInfo, "Inicio de sesión exitoso");
                }
            }
            catch (Exception ex)
            {
                // Log del error
                System.Diagnostics.Debug.WriteLine($"Error en ValidateUserAsync: {ex.Message}");

                return new Mdl_Security_UserLoginResult(
                    Mdl_Security_LoginStatus.None,
                    "Error en el sistema. Contacte al administrador."
                );
            }
        }
        /// Versión síncrona del método de validación
        public Mdl_Security_UserLoginResult ValidateUser(string username, string password)
        {
            try
            {
                return ValidateUserAsync(username, password).Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ValidateUser: {ex.Message}");
                return new Mdl_Security_UserLoginResult(
                    Mdl_Security_LoginStatus.None,
                    "Error en el sistema. Contacte al administrador."
                );
            }
        }
        /// Registra el logout del usuario
        public async Task<bool> LogoutUserAsync(int userId, string username)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Actualizar fecha de desconexión
                    string query = @"
                        UPDATE Users 
                        SET LastConnectionDate = GETDATE()
                        WHERE UserId = @userId";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        await command.ExecuteNonQueryAsync();
                    }

                    // Log del logout
                    await auditController.LogLogoutAsync(userId, username);

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en LogoutUserAsync: {ex.Message}");
                return false;
            }
        }
        /// Cambia la contraseña del usuario
        public async Task<bool> ChangePasswordAsync(int userId, string username, string newPassword, bool isTemporary = false)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string passwordHash = CreatePasswordHash(newPassword);

                    string query = @"
                        UPDATE Users 
                        SET PasswordHash = @passwordHash,
                            IsTemporaryPassword = @isTemporary,
                            PasswordExpiryDate = @expiryDate,
                            ModifiedDate = GETDATE()
                        WHERE UserId = @userId";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@passwordHash", passwordHash);
                        command.Parameters.AddWithValue("@isTemporary", isTemporary);
                        command.Parameters.AddWithValue("@expiryDate",
                            isTemporary ? DateTime.Now.AddDays(30) : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@userId", userId);

                        await command.ExecuteNonQueryAsync();
                    }

                    // Log del cambio de contraseña
                    await auditController.LogPasswordChangeAsync(userId, username, isTemporary);

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ChangePasswordAsync: {ex.Message}");
                return false;
            }
        }
        //Obtener datos del usuario
        private async Task<Mdl_Security_UserInfo> GetUserInfoAsync(SqlConnection connection, string username)
        {
            string query = @"
            SELECT u.UserId, u.Username, u.FullName, u.RoleId, u.StatusId, 
                   u.IsTemporaryPassword, u.PasswordExpiryDate, u.InstitutionalEmail,
                   u.EmployeeId, u.LastLoginDate, u.CreatedDate, u.NotificationsEnabled,
                   ISNULL(r.RoleName, '') AS RoleName, 
                   ISNULL(s.StatusName, '') AS StatusName
            FROM Users u
            LEFT JOIN Roles r ON u.RoleId = r.RoleId
            LEFT JOIN UserStatus s ON u.StatusId = s.StatusId
            WHERE u.Username = @username";

            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var userInfo = new Mdl_Security_UserInfo();

                            // ÍNDICES CORREGIDOS según el orden en tu SELECT:
                            // 0=UserId, 1=Username, 2=FullName, 3=RoleId, 4=StatusId, 
                            // 5=IsTemporaryPassword, 6=PasswordExpiryDate, 7=InstitutionalEmail,
                            // 8=EmployeeId, 9=LastLoginDate, 10=CreatedDate, 11=NotificationsEnabled,
                            // 12=RoleName, 13=StatusName

                            try { userInfo.UserId = reader.GetInt32(0); }
                            catch { System.Diagnostics.Debug.WriteLine("Error leyendo UserId"); }

                            try { userInfo.Username = reader.GetString(1); }
                            catch { userInfo.Username = ""; }

                            try { userInfo.FullName = reader.GetString(2); } // CORREGIDO: era 3, ahora es 2
                            catch { userInfo.FullName = ""; }

                            try { userInfo.RoleId = reader.GetInt32(3); } // CORREGIDO: era 4, ahora es 3
                            catch { userInfo.RoleId = 0; }

                            try { userInfo.StatusId = reader.GetInt32(4); } // CORREGIDO: era 5, ahora es 4
                            catch { userInfo.StatusId = 0; }

                            try { userInfo.IsTemporaryPassword = reader.GetBoolean(5); } // CORREGIDO: era 8, ahora es 5
                            catch { userInfo.IsTemporaryPassword = false; }

                            try { userInfo.PasswordExpiryDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6); } // CORREGIDO: era 15, ahora es 6
                            catch { userInfo.PasswordExpiryDate = null; }

                            try { userInfo.InstitutionalEmail = reader.IsDBNull(7) ? null : reader.GetString(7); } // CORREGIDO: era 13, ahora es 7
                            catch { userInfo.InstitutionalEmail = null; }

                            try { userInfo.EmployeeId = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8); } // CORREGIDO: era 14, ahora es 8
                            catch { userInfo.EmployeeId = null; }

                            try { userInfo.LastLoginDate = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9); } // CORREGIDO: era 18, ahora es 9
                            catch { userInfo.LastLoginDate = null; }

                            try { userInfo.CreatedDate = reader.GetDateTime(10); } // CORREGIDO: era 9, ahora es 10
                            catch { userInfo.CreatedDate = DateTime.Now; }

                            try { userInfo.NotificationsEnabled = reader.GetBoolean(11); } // CORREGIDO: era 6, ahora es 11
                            catch { userInfo.NotificationsEnabled = true; }

                            try { userInfo.RoleName = reader.GetString(12); } // CORREGIDO: usar índice 12
                            catch { userInfo.RoleName = ""; }

                            try { userInfo.StatusName = reader.GetString(13); } // CORREGIDO: usar índice 13
                            catch { userInfo.StatusName = ""; }

                            return userInfo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetUserInfoAsync: {ex.Message}");
                throw;
            }
            return null;
        }
        // Método público para ser llamado desde el formulario
        public async Task<Mdl_Security_UserInfo> ObtenerDatosUsuarioAsync(string username)
        {
            try
            {
                using (var connection = new SqlConnection(DatabaseConfig.GetConnectionString()))
                {
                    await connection.OpenAsync();
                    return await GetUserInfoAsync(connection, username);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerDatosUsuarioAsync: {ex.Message}");
                return null;
            }
        }
        //Identificar si el usuario está activo Async
        private async Task<bool> IsUserActiveAsync(SqlConnection connection, int userId)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM Users u
                INNER JOIN UserStatus us ON u.StatusId = us.StatusId
                WHERE u.UserId = @userId AND (us.StatusName = 'ACTIVO' OR us.StatusName = 'ACTIVE')";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                var count = await command.ExecuteScalarAsync();
                return Convert.ToInt32(count) > 0;
            }
        }
        //Identificar si el usuario está inactivo Async
        private async Task<bool> IsUserLockedAsync(SqlConnection connection, int userId)
        {
            string query = "SELECT IsLocked FROM Users WHERE UserId = @userId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                var result = await command.ExecuteScalarAsync();
                return result != null && Convert.ToBoolean(result);
            }
        }
        //Validar Contraseña Async
        private async Task<bool> ValidatePasswordAsync(SqlConnection connection, int userId, string password)
        {
            string query = "SELECT PasswordHash FROM Users WHERE UserId = @userId";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                var result = await command.ExecuteScalarAsync();
                var storedHash = result?.ToString();

                if (string.IsNullOrEmpty(storedHash))
                    return false;

                return VerifyPassword(password, storedHash);
            }
        }
        // Incrementar intentos fallidos Async
        private async Task IncrementFailedAttemptsAsync(SqlConnection connection, int userId)
        {
            string query = @"
                UPDATE Users 
                SET FailedLoginAttempts = FailedLoginAttempts + 1,
                    ModifiedDate = GETDATE()
                WHERE UserId = @userId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                await command.ExecuteNonQueryAsync();
            }
        }
        // Obtener número de intentos fallidos Async
        private async Task<int> GetFailedAttemptsAsync(SqlConnection connection, int userId)
        {
            string query = "SELECT FailedLoginAttempts FROM Users WHERE UserId = @userId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
        // Bloquear usuario Async
        private async Task LockUserAsync(SqlConnection connection, int userId)
        {
            string query = @"
                UPDATE Users 
                SET IsLocked = 1, ModifiedDate = GETDATE() 
                WHERE UserId = @userId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                await command.ExecuteNonQueryAsync();
            }
        }
        // Resetear intentos fallidos Async
        private async Task ResetFailedAttemptsAsync(SqlConnection connection, int userId)
        {
            string query = @"
                UPDATE Users 
                SET FailedLoginAttempts = 0 
                WHERE UserId = @userId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                await command.ExecuteNonQueryAsync();
            }
        }
        // Actualizar fechas de último login y conexión Async
        private async Task UpdateLastLoginAsync(SqlConnection connection, int userId)
        {
            string query = @"
                UPDATE Users 
                SET LastLoginDate = GETDATE(), 
                    LastConnectionDate = GETDATE(),
                    ModifiedDate = GETDATE()
                WHERE UserId = @userId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                await command.ExecuteNonQueryAsync();
            }
        }
        // Verificar contraseña usando BCrypt
        private bool VerifyPassword(string password, string hash)
        {
            try
            {
                // BCrypt maneja la verificación automáticamente
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch (Exception)
            {
                return false;
            }
        }
        // Crear hash de contraseña usando BCrypt
        private string CreatePasswordHash(string password)
        {
            // BCrypt es mucho más seguro que SHA256 para contraseñas
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }
        #endregion MetodosPrivados
        #region MetodosPermisos
        /// Obtiene todos los permisos efectivos de un usuario (Rol + Específicos)
        public async Task<List<string>> ObtenerPermisosUsuarioAsync(int userId, int roleId)
        {
            List<string> permisos = new List<string>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                -- Permisos del rol del usuario
                SELECT DISTINCT p.PermissionName
                FROM RolePermissions rp
                INNER JOIN Permissions p ON rp.PermissionId = p.PermissionId
                WHERE rp.RoleId = @RoleId 
                  AND rp.IsGranted = 1 
                  AND p.IsActive = 1
                  AND p.PermissionId NOT IN (
                      SELECT PermissionId 
                      FROM UserPermissions 
                      WHERE UserId = @UserId
                  )
                
                UNION
                
                -- Permisos específicos del usuario (sobrescriben los del rol)
                SELECT p.PermissionName
                FROM UserPermissions up
                INNER JOIN Permissions p ON up.PermissionId = p.PermissionId
                WHERE up.UserId = @UserId 
                  AND up.IsGranted = 1 
                  AND p.IsActive = 1
                
                ORDER BY PermissionName";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@RoleId", roleId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                permisos.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerPermisosUsuarioAsync: {ex.Message}");
            }
            return permisos;
        }
        #endregion MetodosPermisos
    }
}