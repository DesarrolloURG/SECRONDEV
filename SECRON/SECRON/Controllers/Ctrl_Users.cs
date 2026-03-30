using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_Users
    {
        #region SHA256

        //VERSIÓN LEGACY DE HASH DE CONTRASEÑA (NO RECOMENDADO PARA NUEVOS DESARROLLOS)
        // MÉTODO AUXILIAR: Generar Hash de Contraseña BCrypt ya no EN (SHA256)
        //private static string GenerarHashPassword(string password)
        //{
        //    using (SHA256 sha256 = SHA256.Create())
        //    {
        //        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        StringBuilder builder = new StringBuilder();
        //        foreach (byte b in bytes)
        //        {
        //            builder.Append(b.ToString("x2"));
        //        }
        //        return builder.ToString();
        //    }
        //}
        #endregion SHA256
        #region BCRYPT ENCRIPTACION
        // BCRYPT
        private static string GenerarHashPassword(string password)
        {
            // BCrypt con workFactor 12 (recomendado para seguridad)
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }
        #endregion BCRYPT ENCRIPTACION
        #region CRUD
        // MÉTODO PRINCIPAL: Registrar usuario
        public static int RegistrarUsuario(Mdl_Users usuario, string password)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Users (Username, PasswordHash, FullName, RoleId, StatusId, 
                        NotificationsEnabled, IsTemporaryPassword, InstitutionalEmail, EmployeeId, 
                        PasswordExpiryDate, CreatedBy) 
                        VALUES (@Username, @PasswordHash, @FullName, @RoleId, @StatusId, 
                        @NotificationsEnabled, @IsTemporaryPassword, @InstitutionalEmail, @EmployeeId, 
                        @PasswordExpiryDate, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", usuario.Username ?? "");
                        cmd.Parameters.AddWithValue("@PasswordHash", GenerarHashPassword(password));
                        cmd.Parameters.AddWithValue("@FullName", usuario.FullName ?? "");
                        cmd.Parameters.AddWithValue("@RoleId", usuario.RoleId);
                        cmd.Parameters.AddWithValue("@StatusId", usuario.StatusId);
                        cmd.Parameters.AddWithValue("@NotificationsEnabled", usuario.NotificationsEnabled);
                        cmd.Parameters.AddWithValue("@IsTemporaryPassword", usuario.IsTemporaryPassword);
                        cmd.Parameters.AddWithValue("@InstitutionalEmail", (object)usuario.InstitutionalEmail ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmployeeId", (object)usuario.EmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PasswordExpiryDate", (object)usuario.PasswordExpiryDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)usuario.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
        // MÉTODO PRINCIPAL: Mostrar todos los usuarios con paginación
        public static List<Mdl_Users> MostrarUsuarios(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_Users> lista = new List<Mdl_Users>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM Users 
                        ORDER BY FullName 
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearUsuario(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
        #endregion CRUD
        
        // MÉTODO PRINCIPAL: Búsqueda con múltiples filtros
        public static List<Mdl_Users> BuscarUsuarios(string textoBusqueda = "", int? roleId = null,int? statusId = null,bool? isLocked = null,int pageNumber = 1,int pageSize = 100)
        {
            List<Mdl_Users> lista = new List<Mdl_Users>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Users WHERE 1=1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    // Filtro por texto general
                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (Username LIKE @texto OR FullName LIKE @texto OR 
                            InstitutionalEmail LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    // Filtro por rol
                    if (roleId.HasValue && roleId > 0)
                    {
                        query += " AND RoleId = @roleId";
                        parametros.Add(new SqlParameter("@roleId", roleId.Value));
                    }

                    // Filtro por estado
                    if (statusId.HasValue && statusId > 0)
                    {
                        query += " AND StatusId = @statusId";
                        parametros.Add(new SqlParameter("@statusId", statusId.Value));
                    }

                    // Filtro por bloqueado
                    if (isLocked.HasValue)
                    {
                        query += " AND IsLocked = @isLocked";
                        parametros.Add(new SqlParameter("@isLocked", isLocked.Value));
                    }

                    query += " ORDER BY FullName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearUsuario(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en búsqueda: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Actualizar usuario
        public static int ActualizarUsuario(Mdl_Users usuario)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Users SET Username = @Username, FullName = @FullName, 
                        RoleId = @RoleId, StatusId = @StatusId, NotificationsEnabled = @NotificationsEnabled, 
                        InstitutionalEmail = @InstitutionalEmail, EmployeeId = @EmployeeId, 
                        ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy 
                        WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", usuario.UserId);
                        cmd.Parameters.AddWithValue("@Username", usuario.Username ?? "");
                        cmd.Parameters.AddWithValue("@FullName", usuario.FullName ?? "");
                        cmd.Parameters.AddWithValue("@RoleId", usuario.RoleId);
                        cmd.Parameters.AddWithValue("@StatusId", usuario.StatusId);
                        cmd.Parameters.AddWithValue("@NotificationsEnabled", usuario.NotificationsEnabled);
                        cmd.Parameters.AddWithValue("@InstitutionalEmail", (object)usuario.InstitutionalEmail ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmployeeId", (object)usuario.EmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)usuario.ModifiedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Cambiar contraseña
        public static int CambiarPassword(int userId, string newPassword, bool isTemporary = false)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Users SET PasswordHash = @PasswordHash, 
                        IsTemporaryPassword = @IsTemporaryPassword, 
                        PasswordExpiryDate = @PasswordExpiryDate, 
                        ModifiedDate = GETDATE() 
                        WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@PasswordHash", GenerarHashPassword(newPassword));
                        cmd.Parameters.AddWithValue("@IsTemporaryPassword", isTemporary);
                        cmd.Parameters.AddWithValue("@PasswordExpiryDate",
                            isTemporary ? (object)DateTime.Now.AddDays(30) : DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar contraseña: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        //// MÉTODO PRINCIPAL: Validar login PARA SHA256
        //public static Mdl_Users ValidarLogin(string username, string password)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = DatabaseConfig.StartConection())
        //        {
        //            string query = "SELECT * FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
        //            using (SqlCommand cmd = new SqlCommand(query, connection))
        //            {
        //                cmd.Parameters.AddWithValue("@Username", username ?? "");
        //                cmd.Parameters.AddWithValue("@PasswordHash", GenerarHashPassword(password));

        //                using (SqlDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        return MapearUsuario(reader);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error al validar login: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    return null;
        //}

        // MÉTODO PRINCIPAL: Validar login BCRYPT
        public static Mdl_Users ValidarLogin(string username, string password)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // Primero obtenemos el usuario por username
                    string query = "SELECT * FROM Users WHERE Username = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username ?? "");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Obtener el hash almacenado en la BD
                                string storedHash = reader["PasswordHash"].ToString();

                                // Verificar si la contraseña ingresada coincide con el hash
                                if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                                {
                                    // Mapear y retornar el usuario
                                    return MapearUsuario(reader);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar login: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Registrar intento de login fallido
        public static int RegistrarLoginFallido(string username)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Users SET FailedLoginAttempts = FailedLoginAttempts + 1, 
                        IsLocked = CASE WHEN FailedLoginAttempts >= 4 THEN 1 ELSE 0 END 
                        WHERE Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username ?? "");
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { return 0; }
        }

        // MÉTODO PRINCIPAL: Registrar login exitoso
        public static int RegistrarLoginExitoso(int userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Users SET FailedLoginAttempts = 0, LastLoginDate = GETDATE(), 
                        LastConnectionDate = GETDATE() WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { return 0; }
        }

        // MÉTODO PRINCIPAL: Desbloquear usuario
        public static int DesbloquearUsuario(int userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE Users SET IsLocked = 0, FailedLoginAttempts = 0 WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desbloquear usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener usuario por ID
        public static Mdl_Users ObtenerUsuarioPorId(int userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Users WHERE UserId = @UserId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearUsuario(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }
        // MÉTODO AUXILIAR: Obtener el nombre completo del usuario por ID
        public static string ObtenerNombreCompletoPorId(int userId)
        {
            try
            {
                var usuario = ObtenerUsuarioPorId(userId);

                if (usuario == null)
                    return string.Empty;

                return usuario.FullName ?? string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener nombre de usuario: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }


        // MÉTODO AUXILIAR: Mapear SqlDataReader a Mdl_Users
        // Orden de campos en SELECT: UserId(0), Username(1), PasswordHash(2), FullName(3), RoleId(4), 
        // StatusId(5), NotificationsEnabled(6), LastConnectionDate(7), IsTemporaryPassword(8), 
        // CreatedDate(9), CreatedBy(10), ModifiedDate(11), ModifiedBy(12), InstitutionalEmail(13), 
        // EmployeeId(14), PasswordExpiryDate(15), FailedLoginAttempts(16), IsLocked(17), LastLoginDate(18)
        private static Mdl_Users MapearUsuario(SqlDataReader reader)
        {
            return new Mdl_Users
            {
                UserId = reader.GetInt32(0),
                Username = reader[1].ToString(),
                PasswordHash = reader[2].ToString(),
                FullName = reader[3].ToString(),
                RoleId = reader.GetInt32(4),
                StatusId = reader.GetInt32(5),
                NotificationsEnabled = reader.GetBoolean(6),
                LastConnectionDate = reader[7] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(7),
                IsTemporaryPassword = reader.GetBoolean(8),
                CreatedDate = reader.GetDateTime(9),
                CreatedBy = reader[10] == DBNull.Value ? null : (int?)reader.GetInt32(10),
                ModifiedDate = reader[11] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(11),
                ModifiedBy = reader[12] == DBNull.Value ? null : (int?)reader.GetInt32(12),
                InstitutionalEmail = reader[13] == DBNull.Value ? null : reader[13].ToString(),
                EmployeeId = reader[14] == DBNull.Value ? null : (int?)reader.GetInt32(14),
                PasswordExpiryDate = reader[15] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(15),
                FailedLoginAttempts = reader.GetInt32(16),
                IsLocked = reader.GetBoolean(17),
                LastLoginDate = reader[18] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(18)
            };
        }

        // MÉTODOS DE VALIDACIÓN
        public static bool ValidarUsernameUnico(string username, int? excludeUserId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    if (excludeUserId.HasValue)
                        query += " AND UserId != @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username ?? "");
                        if (excludeUserId.HasValue)
                            cmd.Parameters.AddWithValue("@UserId", excludeUserId.Value);

                        return (int)cmd.ExecuteScalar() == 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODO PARA CONTAR TOTAL DE REGISTROS (PARA PAGINACIÓN)
        public static int ContarTotalUsuarios(string textoBusqueda = "",int? roleId = null,int? statusId = null, bool? isLocked = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE 1=1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (Username LIKE @texto OR FullName LIKE @texto OR 
                            InstitutionalEmail LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (roleId.HasValue && roleId > 0)
                    {
                        query += " AND RoleId = @roleId";
                        parametros.Add(new SqlParameter("@roleId", roleId.Value));
                    }

                    if (statusId.HasValue && statusId > 0)
                    {
                        query += " AND StatusId = @statusId";
                        parametros.Add(new SqlParameter("@statusId", statusId.Value));
                    }

                    if (isLocked.HasValue)
                    {
                        query += " AND IsLocked = @isLocked";
                        parametros.Add(new SqlParameter("@isLocked", isLocked.Value));
                    }

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch { return 0; }
        }
        // En Ctrl_Users.cs - CORREGIR ESTE MÉTODO
        public static List<int> ObtenerEmpleadosConUsuario()
        {
            List<int> empleadosConUsuario = new List<int>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection()) // ⭐ USAR DatabaseConfig
                {
                    string query = @"
                SELECT DISTINCT EmployeeId 
                FROM Users 
                WHERE EmployeeId IS NOT NULL"; // ⭐ QUITAR IsActive (no existe en Users)

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                empleadosConUsuario.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener empleados con usuario: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return empleadosConUsuario;
        }
        public static Mdl_Users ObtenerUsuarioPorUsername(string username)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Users WHERE Username = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearUsuario(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar usuario: " + ex.Message);
            }
            return null;
        }
        // MÉTODO: Obtener usuarios con permiso específico
        public static List<Mdl_Users> ObtenerUsuariosConPermisoEspecifico(string permissionCode)
        {
            List<Mdl_Users> lista = new List<Mdl_Users>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                SELECT DISTINCT u.* 
                FROM Users u
                INNER JOIN Roles r ON u.RoleId = r.RoleId
                INNER JOIN RolePermissions rp ON r.RoleId = rp.RoleId
                INNER JOIN Permissions p ON rp.PermissionId = p.PermissionId
                WHERE p.PermissionCode = @PermissionCode 
                AND rp.IsGranted = 1
                AND u.StatusId = 1
                
                UNION
                
                SELECT DISTINCT u.* 
                FROM Users u
                INNER JOIN UserPermissions up ON u.UserId = up.UserId
                INNER JOIN Permissions p ON up.PermissionId = p.PermissionId
                WHERE p.PermissionCode = @PermissionCode 
                AND up.IsGranted = 1
                AND u.StatusId = 1
                
                ORDER BY FullName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PermissionCode", permissionCode);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearUsuario(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener usuarios con permiso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
        // Este metodo actualiza SOLO los campos relacionados con el bloqueo
        public static int DesbloquearUsuario(int userId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Users 
                SET 
                    IsLocked = 0,
                    FailedLoginAttempts = 0,
                    ModifiedDate = GETDATE(),
                    ModifiedBy = @ModifiedBy 
                WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desbloquear usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
}