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
    internal class Ctrl_RolePermissions
    {
        // MÉTODO PRINCIPAL: Asignar permiso a rol
        public static int AsignarPermisoARol(Mdl_RolePermissions rolePermission)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO RolePermissions (RoleId, PermissionId, IsGranted, CreatedBy) 
                        VALUES (@RoleId, @PermissionId, @IsGranted, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", rolePermission.RoleId);
                        cmd.Parameters.AddWithValue("@PermissionId", rolePermission.PermissionId);
                        cmd.Parameters.AddWithValue("@IsGranted", rolePermission.IsGranted);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)rolePermission.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar permiso a rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Asignar múltiples permisos a un rol (Transacción)
        public static int AsignarMultiplesPermisosARol(int roleId, List<int> permissionIds, int? createdBy = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Primero eliminamos los permisos existentes del rol
                            string deleteQuery = "DELETE FROM RolePermissions WHERE RoleId = @RoleId";
                            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, connection, transaction))
                            {
                                deleteCmd.Parameters.AddWithValue("@RoleId", roleId);
                                deleteCmd.ExecuteNonQuery();
                            }

                            // Luego insertamos los nuevos permisos
                            int count = 0;
                            string insertQuery = @"INSERT INTO RolePermissions (RoleId, PermissionId, IsGranted, CreatedBy) 
                                VALUES (@RoleId, @PermissionId, @IsGranted, @CreatedBy)";

                            foreach (int permissionId in permissionIds)
                            {
                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection, transaction))
                                {
                                    insertCmd.Parameters.AddWithValue("@RoleId", roleId);
                                    insertCmd.Parameters.AddWithValue("@PermissionId", permissionId);
                                    insertCmd.Parameters.AddWithValue("@IsGranted", true);
                                    insertCmd.Parameters.AddWithValue("@CreatedBy", (object)createdBy ?? DBNull.Value);
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
                MessageBox.Show("Error al asignar permisos al rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener todos los permisos de un rol
        public static List<Mdl_RolePermissions> ObtenerPermisosPorRol(int roleId)
        {
            List<Mdl_RolePermissions> lista = new List<Mdl_RolePermissions>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM RolePermissions WHERE RoleId = @RoleId ORDER BY PermissionId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearRolePermission(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener permisos del rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Obtener solo IDs de permisos de un rol
        public static List<int> ObtenerPermissionIdsPorRol(int roleId)
        {
            List<int> permisos = new List<int>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT PermissionId FROM RolePermissions WHERE RoleId = @RoleId AND IsGranted = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                permisos.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener IDs de permisos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return permisos;
        }

        // MÉTODO PRINCIPAL: Actualizar estado de permiso (Conceder/Revocar)
        public static int ActualizarEstadoPermiso(int rolePermissionId, bool isGranted)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE RolePermissions SET IsGranted = @IsGranted WHERE RolePermissionId = @RolePermissionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RolePermissionId", rolePermissionId);
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

        // MÉTODO PRINCIPAL: Eliminar permiso de rol
        public static int EliminarPermisoDeRol(int rolePermissionId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "DELETE FROM RolePermissions WHERE RolePermissionId = @RolePermissionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RolePermissionId", rolePermissionId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar permiso del rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Eliminar todos los permisos de un rol
        public static int EliminarTodosLosPermisosDeRol(int roleId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "DELETE FROM RolePermissions WHERE RoleId = @RoleId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar permisos del rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Verificar si un rol tiene un permiso específico
        public static bool VerificarPermisoDeRol(int roleId, int permissionId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT COUNT(*) FROM RolePermissions 
                        WHERE RoleId = @RoleId AND PermissionId = @PermissionId AND IsGranted = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        cmd.Parameters.AddWithValue("@PermissionId", permissionId);

                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODO PRINCIPAL: Obtener permisos con detalles (JOIN con Permissions)
        public static List<dynamic> ObtenerPermisosDetalladosPorRol(int roleId)
        {
            List<dynamic> lista = new List<dynamic>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT 
                        rp.RolePermissionId,
                        rp.RoleId,
                        rp.PermissionId,
                        rp.IsGranted,
                        p.PermissionCode,
                        p.PermissionName,
                        p.ModuleName,
                        p.ActionType
                    FROM RolePermissions rp
                    INNER JOIN Permissions p ON rp.PermissionId = p.PermissionId
                    WHERE rp.RoleId = @RoleId
                    ORDER BY p.ModuleName, p.PermissionName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new
                                {
                                    RolePermissionId = reader.GetInt32(0),
                                    RoleId = reader.GetInt32(1),
                                    PermissionId = reader.GetInt32(2),
                                    IsGranted = reader.GetBoolean(3),
                                    PermissionCode = reader[4].ToString(),
                                    PermissionName = reader[5].ToString(),
                                    ModuleName = reader[6] == DBNull.Value ? null : reader[6].ToString(),
                                    ActionType = reader[7] == DBNull.Value ? null : reader[7].ToString()
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

        // MÉTODO AUXILIAR: Mapear SqlDataReader a Mdl_RolePermissions
        // Orden de campos en SELECT: RolePermissionId(0), RoleId(1), PermissionId(2), 
        // IsGranted(3), CreatedDate(4), CreatedBy(5)
        private static Mdl_RolePermissions MapearRolePermission(SqlDataReader reader)
        {
            return new Mdl_RolePermissions
            {
                RolePermissionId = reader.GetInt32(0),
                RoleId = reader.GetInt32(1),
                PermissionId = reader.GetInt32(2),
                IsGranted = reader.GetBoolean(3),
                CreatedDate = reader.GetDateTime(4),
                CreatedBy = reader[5] == DBNull.Value ? null : (int?)reader.GetInt32(5)
            };
        }

        // MÉTODO PARA VERIFICAR SI YA EXISTE UNA ASIGNACIÓN
        public static bool ExisteAsignacion(int roleId, int permissionId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM RolePermissions WHERE RoleId = @RoleId AND PermissionId = @PermissionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        cmd.Parameters.AddWithValue("@PermissionId", permissionId);

                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODO PARA CONTAR PERMISOS POR ROL
        public static int ContarPermisosPorRol(int roleId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM RolePermissions WHERE RoleId = @RoleId AND IsGranted = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch { return 0; }
        }
    }
}