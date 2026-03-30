using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_UserPermissions
    {
        // MÉTODO PRINCIPAL: Asignar permiso específico a usuario
        public static int AsignarPermisoAUsuario(Mdl_UserPermissions userPermission)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO UserPermissions (UserId, PermissionId, IsGranted, GrantedBy) 
                        VALUES (@UserId, @PermissionId, @IsGranted, @GrantedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userPermission.UserId);
                        cmd.Parameters.AddWithValue("@PermissionId", userPermission.PermissionId);
                        cmd.Parameters.AddWithValue("@IsGranted", userPermission.IsGranted);
                        cmd.Parameters.AddWithValue("@GrantedBy", userPermission.GrantedBy);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar permiso a usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Asignar múltiples permisos a un usuario (Transacción)
        public static int AsignarMultiplesPermisosAUsuario(int userId, List<Tuple<int, bool>> permissionsWithGrant, int grantedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Primero eliminamos los permisos específicos existentes del usuario
                            string deleteQuery = "DELETE FROM UserPermissions WHERE UserId = @UserId";
                            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, connection, transaction))
                            {
                                deleteCmd.Parameters.AddWithValue("@UserId", userId);
                                deleteCmd.ExecuteNonQuery();
                            }

                            // Luego insertamos los nuevos permisos
                            int count = 0;
                            string insertQuery = @"INSERT INTO UserPermissions (UserId, PermissionId, IsGranted, GrantedBy) 
                                VALUES (@UserId, @PermissionId, @IsGranted, @GrantedBy)";

                            foreach (var permission in permissionsWithGrant)
                            {
                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection, transaction))
                                {
                                    insertCmd.Parameters.AddWithValue("@UserId", userId);
                                    insertCmd.Parameters.AddWithValue("@PermissionId", permission.Item1);
                                    insertCmd.Parameters.AddWithValue("@IsGranted", permission.Item2);
                                    insertCmd.Parameters.AddWithValue("@GrantedBy", grantedBy);
                                    count += insertCmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return count;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar permisos al usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener todos los permisos específicos de un usuario
        public static List<Mdl_UserPermissions> ObtenerPermisosPorUsuario(int userId)
        {
            List<Mdl_UserPermissions> lista = new List<Mdl_UserPermissions>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM UserPermissions WHERE UserId = @UserId ORDER BY PermissionId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearUserPermission(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener permisos del usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Actualizar estado de permiso específico (Conceder/Denegar)
        public static int ActualizarEstadoPermiso(int userPermissionId, bool isGranted)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE UserPermissions SET IsGranted = @IsGranted, GrantedDate = GETDATE() 
                        WHERE UserPermissionId = @UserPermissionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserPermissionId", userPermissionId);
                        cmd.Parameters.AddWithValue("@IsGranted", isGranted);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar estado de permiso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Eliminar permiso específico de usuario
        public static int EliminarPermisoDeUsuario(int userPermissionId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "DELETE FROM UserPermissions WHERE UserPermissionId = @UserPermissionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserPermissionId", userPermissionId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar permiso del usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Eliminar todos los permisos específicos de un usuario
        public static int EliminarTodosLosPermisosDeUsuario(int userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "DELETE FROM UserPermissions WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar permisos del usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Verificar permiso específico de usuario (considerando sobrescritura)
        public static bool? VerificarPermisoEspecificoDeUsuario(int userId, int permissionId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT IsGranted FROM UserPermissions 
                        WHERE UserId = @UserId AND PermissionId = @PermissionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@PermissionId", permissionId);

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return (bool)result;
                        }
                        return null; // No existe permiso específico, usar del rol
                    }
                }
            }
            catch { return null; }
        }

        // MÉTODO PRINCIPAL: Obtener permisos efectivos de usuario (Rol + Específicos)
        public static List<dynamic> ObtenerPermisosEfectivosDeUsuario(int userId)
        {
            List<dynamic> lista = new List<dynamic>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        -- Permisos del rol del usuario
                        SELECT DISTINCT
                            p.PermissionId,
                            p.PermissionCode,
                            p.PermissionName,
                            p.ModuleName,
                            p.ActionType,
                            CASE 
                                WHEN up.IsGranted IS NOT NULL THEN up.IsGranted
                                ELSE rp.IsGranted
                            END AS IsGranted,
                            CASE 
                                WHEN up.IsGranted IS NOT NULL THEN 'Específico'
                                ELSE 'Rol'
                            END AS Source
                        FROM Users u
                        INNER JOIN Roles r ON u.RoleId = r.RoleId
                        LEFT JOIN RolePermissions rp ON r.RoleId = rp.RoleId
                        LEFT JOIN Permissions p ON rp.PermissionId = p.PermissionId
                        LEFT JOIN UserPermissions up ON u.UserId = up.UserId AND p.PermissionId = up.PermissionId
                        WHERE u.UserId = @UserId AND p.PermissionId IS NOT NULL
                        
                        UNION
                        
                        -- Permisos específicos que no están en el rol
                        SELECT 
                            p.PermissionId,
                            p.PermissionCode,
                            p.PermissionName,
                            p.ModuleName,
                            p.ActionType,
                            up.IsGranted,
                            'Específico' AS Source
                        FROM UserPermissions up
                        INNER JOIN Permissions p ON up.PermissionId = p.PermissionId
                        LEFT JOIN Users u ON up.UserId = u.UserId
                        LEFT JOIN RolePermissions rp ON u.RoleId = rp.RoleId AND up.PermissionId = rp.PermissionId
                        WHERE up.UserId = @UserId AND rp.PermissionId IS NULL
                        
                        ORDER BY ModuleName, PermissionName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new
                                {
                                    PermissionId = reader.GetInt32(0),
                                    PermissionCode = reader[1].ToString(),
                                    PermissionName = reader[2].ToString(),
                                    ModuleName = reader[3] == DBNull.Value ? null : reader[3].ToString(),
                                    ActionType = reader[4] == DBNull.Value ? null : reader[4].ToString(),
                                    IsGranted = reader.GetBoolean(5),
                                    Source = reader[6].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener permisos efectivos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Obtener permisos con detalles (JOIN con Permissions)
        public static List<dynamic> ObtenerPermisosDetalladosPorUsuario(int userId)
        {
            List<dynamic> lista = new List<dynamic>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT 
                        up.UserPermissionId,
                        up.UserId,
                        up.PermissionId,
                        up.IsGranted,
                        p.PermissionCode,
                        p.PermissionName,
                        p.ModuleName,
                        p.ActionType,
                        up.GrantedDate,
                        u.FullName AS GrantedByName
                    FROM UserPermissions up
                    INNER JOIN Permissions p ON up.PermissionId = p.PermissionId
                    LEFT JOIN Users u ON up.GrantedBy = u.UserId
                    WHERE up.UserId = @UserId
                    ORDER BY p.ModuleName, p.PermissionName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new
                                {
                                    UserPermissionId = reader.GetInt32(0),
                                    UserId = reader.GetInt32(1),
                                    PermissionId = reader.GetInt32(2),
                                    IsGranted = reader.GetBoolean(3),
                                    PermissionCode = reader[4].ToString(),
                                    PermissionName = reader[5].ToString(),
                                    ModuleName = reader[6] == DBNull.Value ? null : reader[6].ToString(),
                                    ActionType = reader[7] == DBNull.Value ? null : reader[7].ToString(),
                                    GrantedDate = reader.GetDateTime(8),
                                    GrantedByName = reader[9] == DBNull.Value ? null : reader[9].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener permisos detallados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO AUXILIAR: Mapear SqlDataReader a Mdl_UserPermissions
        // Orden de campos en SELECT: UserPermissionId(0), UserId(1), PermissionId(2), 
        // IsGranted(3), GrantedDate(4), GrantedBy(5)
        private static Mdl_UserPermissions MapearUserPermission(SqlDataReader reader)
        {
            return new Mdl_UserPermissions
            {
                UserPermissionId = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                PermissionId = reader.GetInt32(2),
                IsGranted = reader.GetBoolean(3),
                GrantedDate = reader.GetDateTime(4),
                GrantedBy = reader.GetInt32(5)
            };
        }

        // MÉTODO PARA VERIFICAR SI YA EXISTE UNA ASIGNACIÓN
        public static bool ExisteAsignacion(int userId, int permissionId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM UserPermissions WHERE UserId = @UserId AND PermissionId = @PermissionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@PermissionId", permissionId);

                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODO PARA CONTAR PERMISOS ESPECÍFICOS POR USUARIO
        public static int ContarPermisosEspecificosPorUsuario(int userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM UserPermissions WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch { return 0; }
        }
    }
}