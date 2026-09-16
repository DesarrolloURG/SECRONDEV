using BCrypt.Net;
using OtpNet;
using QRCoder;
using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SECRON.Utils;

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

                    // 6.6 Verificar doble factor de autenticación (TOTP), salvo usuarios exentos
                    if (!userInfo.TwoFactorExempt)
                    {
                        if (string.IsNullOrEmpty(userInfo.TwoFactorSecret))
                        {
                            return new Mdl_Security_UserLoginResult(userInfo, "Debe vincular su aplicación de autenticación")
                            {
                                ErrorType = Mdl_Security_LoginStatus.TwoFactorSetupRequired
                            };
                        }
                        else
                        {
                            return new Mdl_Security_UserLoginResult(userInfo, "Ingrese el código de su aplicación de autenticación")
                            {
                                ErrorType = Mdl_Security_LoginStatus.TwoFactorRequired
                            };
                        }
                    }

                    // 6.5 Verificar si la contraseña expiró o está por vencer (política de días de vida)
                    if (!userInfo.PasswordNeverExpires)
                    {
                        var (expirada, diasRestantes) = await GetPasswordExpirationInfoAsync(connection, userInfo.UserId);

                        if (expirada)
                        {
                            return new Mdl_Security_UserLoginResult(userInfo, "Su contraseña ha caducado, cámbiela para iniciar sesión")
                            {
                                ErrorType = Mdl_Security_LoginStatus.PasswordExpired
                            };
                        }

                        if (diasRestantes <= 3)
                        {
                            return new Mdl_Security_UserLoginResult(userInfo, "Inicio de sesión exitoso")
                            {
                                DiasRestantesPassword = diasRestantes
                            };
                        }
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

                    using (var command = new SqlCommand("SP_Users_UpdateLastConnection", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);
                        await command.ExecuteScalarAsync();
                    }

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

                    using (var command = new SqlCommand("SP_Users_ChangePasswordAuth", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        command.Parameters.AddWithValue("@IsTemporary", isTemporary);
                        command.Parameters.AddWithValue("@ExpiryDate",
                            isTemporary ? DateTime.Now.AddDays(30) : (object)DBNull.Value);

                        await command.ExecuteScalarAsync();
                    }

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
        // MÉTODO AUXILIAR: Carga los roles activos de un usuario (reemplaza el antiguo RoleId/RoleName único)
        private async Task<List<KeyValuePair<int, string>>> GetUserRolesAsync(SqlConnection connection, int userId)
        {
            var roles = new List<KeyValuePair<int, string>>();
            string query = @"
                SELECT r.RoleId, r.RoleName
                FROM UserRoles ur
                INNER JOIN Roles r ON r.RoleId = ur.RoleId
                WHERE ur.UserId = @UserId AND ur.IsActive = 1
                ORDER BY r.RoleName";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        roles.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                }
            }
            return roles;
        }
        //Obtener datos del usuario
        private async Task<Mdl_Security_UserInfo> GetUserInfoAsync(SqlConnection connection, string username)
        {
            string query = @"
            SELECT u.UserId, u.Username, u.FullName, u.StatusId, 
               u.IsTemporaryPassword, u.PasswordExpiryDate, u.InstitutionalEmail,
               u.LastLoginDate, u.CreatedDate, u.NotificationsEnabled,
               ISNULL(s.StatusName, '') AS StatusName,
               u.LastPasswordChanged, u.PasswordNeverExpires,
               u.TwoFactorSecret, u.TwoFactorEnabledDate, u.TwoFactorExempt
                FROM Users u
                LEFT JOIN UserStatus s ON u.StatusId = s.StatusId
                WHERE u.Username = @username";

            try
            {
                Mdl_Security_UserInfo userInfo = null;

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            userInfo = new Mdl_Security_UserInfo();

                            try { userInfo.UserId = reader.GetInt32(reader.GetOrdinal("UserId")); }
                            catch { System.Diagnostics.Debug.WriteLine("Error leyendo UserId"); }

                            try { userInfo.Username = reader["Username"] as string ?? ""; }
                            catch { userInfo.Username = ""; }

                            try { userInfo.FullName = reader["FullName"] as string ?? ""; }
                            catch { userInfo.FullName = ""; }

                            try { userInfo.StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")); }
                            catch { userInfo.StatusId = 0; }

                            try { userInfo.IsTemporaryPassword = reader.GetBoolean(reader.GetOrdinal("IsTemporaryPassword")); }
                            catch { userInfo.IsTemporaryPassword = false; }

                            try { userInfo.PasswordExpiryDate = reader["PasswordExpiryDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["PasswordExpiryDate"]; }
                            catch { userInfo.PasswordExpiryDate = null; }

                            try { userInfo.InstitutionalEmail = reader["InstitutionalEmail"] == DBNull.Value ? null : (string)reader["InstitutionalEmail"]; }
                            catch { userInfo.InstitutionalEmail = null; }

                            try { userInfo.LastLoginDate = reader["LastLoginDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["LastLoginDate"]; }
                            catch { userInfo.LastLoginDate = null; }

                            try { userInfo.CreatedDate = (DateTime)reader["CreatedDate"]; }
                            catch { userInfo.CreatedDate = DateTime.Now; }

                            try { userInfo.NotificationsEnabled = reader.GetBoolean(reader.GetOrdinal("NotificationsEnabled")); }
                            catch { userInfo.NotificationsEnabled = true; }

                            try { userInfo.StatusName = reader["StatusName"] as string ?? ""; }
                            catch { userInfo.StatusName = ""; }

                            try { userInfo.LastPasswordChanged = reader["LastPasswordChanged"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["LastPasswordChanged"]; }
                            catch { userInfo.LastPasswordChanged = null; }

                            try { userInfo.PasswordNeverExpires = reader.GetBoolean(reader.GetOrdinal("PasswordNeverExpires")); }
                            catch { userInfo.PasswordNeverExpires = false; }

                            try { userInfo.TwoFactorSecret = reader["TwoFactorSecret"] == DBNull.Value ? null : (string)reader["TwoFactorSecret"]; }
                            catch { userInfo.TwoFactorSecret = null; }

                            try { userInfo.TwoFactorEnabledDate = reader["TwoFactorEnabledDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["TwoFactorEnabledDate"]; }
                            catch { userInfo.TwoFactorEnabledDate = null; }

                            try { userInfo.TwoFactorExempt = reader.GetBoolean(reader.GetOrdinal("TwoFactorExempt")); }
                            catch { userInfo.TwoFactorExempt = false; }
                        }
                    }
                }

                // Cargar los roles del usuario (fuera del reader anterior, ya cerrado)
                if (userInfo != null)
                {
                    userInfo.Roles = await GetUserRolesAsync(connection, userInfo.UserId);
                }

                return userInfo;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetUserInfoAsync: {ex.Message}");
                throw;
            }
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
            using (var command = new SqlCommand("SP_Users_IncrementFailedAttempts", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                await command.ExecuteScalarAsync();
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
            using (var command = new SqlCommand("SP_Users_Lock", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                await command.ExecuteScalarAsync();
            }
        }
        // Resetear intentos fallidos Async
        private async Task ResetFailedAttemptsAsync(SqlConnection connection, int userId)
        {
            using (var command = new SqlCommand("SP_Users_ResetFailedAttempts", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                await command.ExecuteScalarAsync();
            }
        }
        // Actualizar fechas de último login y conexión Async
        private async Task UpdateLastLoginAsync(SqlConnection connection, int userId)
        {
            using (var command = new SqlCommand("SP_Users_UpdateLastLoginAuth", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                await command.ExecuteScalarAsync();
            }
        }
        // Verificar expiración de contraseña según la política de días de vida (ParametersConfiguration)
        private async Task<(bool Expirada, int DiasRestantes)> GetPasswordExpirationInfoAsync(SqlConnection connection, int userId)
        {
            using (var command = new SqlCommand("SP_Users_ValidarExpiracionPassword", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return (reader.GetBoolean(0), reader.GetInt32(1));
                    }
                }
            }
            return (false, int.MaxValue);
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
        /// Obtiene todos los permisos efectivos de un usuario (todos sus roles activos + Específicos)
        /// El parámetro roleId se conserva por compatibilidad con las llamadas existentes en los formularios,
        /// pero ya no se usa: los roles del usuario ahora se resuelven internamente desde UserRoles.
        public async Task<List<string>> ObtenerPermisosUsuarioAsync(int userId, int roleId)
        {
            List<string> permisos = new List<string>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                -- Permisos de TODOS los roles activos del usuario
                SELECT DISTINCT p.PermissionName
                FROM RolePermissions rp
                INNER JOIN Permissions p ON rp.PermissionId = p.PermissionId
                WHERE rp.RoleId IN (SELECT RoleId FROM UserRoles WHERE UserId = @UserId AND IsActive = 1)
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
        #region DobleFactorAutenticacion
        // Genera un nuevo secreto Base32 aleatorio para un usuario (160 bits, estándar)
        public string GenerateTwoFactorSecret()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        // Genera el QR (PNG en bytes) para vincular la app autenticadora
        public byte[] GenerateTwoFactorQrCode(string secretBase32, string username)
        {
            string uri = new OtpUri(OtpType.Totp, secretBase32, username, "SECRON").ToString();

            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                return qrCode.GetGraphic(10);
            }
        }

        // Valida un código TOTP de 6 dígitos contra el secreto del usuario
        public bool VerifyTwoFactorCode(string secretBase32, string code)
        {
            if (string.IsNullOrWhiteSpace(secretBase32) || string.IsNullOrWhiteSpace(code))
                return false;

            try
            {
                var secretBytes = Base32Encoding.ToBytes(secretBase32);
                var totp = new Totp(secretBytes);
                return totp.VerifyTotp(code.Trim(), out long timeWindowUsed, VerificationWindow.RfcSpecifiedNetworkDelay);
            }
            catch
            {
                return false;
            }
        }

        // Confirma y guarda el secreto tras validar el primer código (vinculación inicial)
        public async Task<bool> ConfirmTwoFactorSetupAsync(int userId, string secretBase32)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SP_Users_SetTwoFactorSecret", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@TwoFactorSecret", secretBase32);
                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ConfirmTwoFactorSetupAsync: {ex.Message}");
                return false;
            }
        }
        #endregion DobleFactorAutenticacion
        #region CargaInicialConsolidada
        // Carga en una sola llamada: datos de usuario, roles, permisos y parámetro de sesión.
        // Reduce a 1 round-trip lo que antes eran varias llamadas secuenciales (crítico en VPN).
        public async Task<(Mdl_Security_UserInfo UserInfo, List<string> Permisos, int TiempoSesionMinutos)> CargarDatosInicialesAsync(string username)
        {
            Mdl_Security_UserInfo userInfo = null;
            List<string> permisos = new List<string>();
            int tiempoSesion = 15;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SP_Auth_CargaInicialUsuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", username);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // Result Set 1: Usuario (ya sin RoleId/RoleName)
                            if (await reader.ReadAsync())
                            {
                                userInfo = new Mdl_Security_UserInfo();
                                try { userInfo.UserId = reader.GetInt32(reader.GetOrdinal("UserId")); } catch { }
                                try { userInfo.Username = reader["Username"] as string ?? ""; } catch { userInfo.Username = ""; }
                                try { userInfo.FullName = reader["FullName"] as string ?? ""; } catch { userInfo.FullName = ""; }
                                try { userInfo.StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")); } catch { userInfo.StatusId = 0; }
                                try { userInfo.IsTemporaryPassword = reader.GetBoolean(reader.GetOrdinal("IsTemporaryPassword")); } catch { userInfo.IsTemporaryPassword = false; }
                                try { userInfo.PasswordExpiryDate = reader["PasswordExpiryDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["PasswordExpiryDate"]; } catch { userInfo.PasswordExpiryDate = null; }
                                try { userInfo.InstitutionalEmail = reader["InstitutionalEmail"] == DBNull.Value ? null : (string)reader["InstitutionalEmail"]; } catch { userInfo.InstitutionalEmail = null; }
                                try { userInfo.LastLoginDate = reader["LastLoginDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["LastLoginDate"]; } catch { userInfo.LastLoginDate = null; }
                                try { userInfo.CreatedDate = (DateTime)reader["CreatedDate"]; } catch { userInfo.CreatedDate = DateTime.Now; }
                                try { userInfo.NotificationsEnabled = reader.GetBoolean(reader.GetOrdinal("NotificationsEnabled")); } catch { userInfo.NotificationsEnabled = true; }
                                try { userInfo.StatusName = reader["StatusName"] as string ?? ""; } catch { userInfo.StatusName = ""; }
                                try { userInfo.LastPasswordChanged = reader["LastPasswordChanged"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["LastPasswordChanged"]; } catch { userInfo.LastPasswordChanged = null; }
                                try { userInfo.PasswordNeverExpires = reader.GetBoolean(reader.GetOrdinal("PasswordNeverExpires")); } catch { userInfo.PasswordNeverExpires = false; }
                                try { userInfo.TwoFactorSecret = reader["TwoFactorSecret"] == DBNull.Value ? null : (string)reader["TwoFactorSecret"]; } catch { userInfo.TwoFactorSecret = null; }
                                try { userInfo.TwoFactorEnabledDate = reader["TwoFactorEnabledDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["TwoFactorEnabledDate"]; } catch { userInfo.TwoFactorEnabledDate = null; }
                                try { userInfo.TwoFactorExempt = reader.GetBoolean(reader.GetOrdinal("TwoFactorExempt")); } catch { userInfo.TwoFactorExempt = false; }
                            }

                            // Result Set 2 (NUEVO): Roles del usuario
                            if (await reader.NextResultAsync())
                            {
                                var roles = new List<KeyValuePair<int, string>>();
                                while (await reader.ReadAsync())
                                {
                                    roles.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                                }
                                if (userInfo != null) userInfo.Roles = roles;
                            }

                            // Result Set 3: Permisos
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    permisos.Add(reader.GetString(0));
                                }
                            }

                            // Result Set 4: Parámetro de sesión
                            if (await reader.NextResultAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    int.TryParse(reader[0].ToString(), out tiempoSesion);
                                }
                            }

                            // Result Set 5: Configuración SMTP (llena Cls_EmailConfigCache directamente)
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    string nombreParam = reader.GetString(0);
                                    string valorParam = reader.GetString(1);
                                    Cls_EmailConfigCache.AsignarValor(nombreParam, valorParam);
                                }
                                Cls_EmailConfigCache.MarcarComoCargado();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CargarDatosInicialesAsync: {ex.Message}");
            }

            return (userInfo, permisos, tiempoSesion);
        }
        #endregion CargaInicialConsolidada
    }
}