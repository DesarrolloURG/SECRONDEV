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
    internal class Ctrl_Roles
    {
        // MÉTODO PRINCIPAL: Registrar rol
        public static int RegistrarRol(Mdl_Roles rol)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Roles (RoleName, Description, IsActive, CreatedBy) 
                        VALUES (@RoleName, @Description, @IsActive, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleName", rol.RoleName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)rol.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", rol.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)rol.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todos los roles con paginación
        public static List<Mdl_Roles> MostrarRoles(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_Roles> lista = new List<Mdl_Roles>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM Roles WHERE IsActive = 1 
                        ORDER BY RoleName 
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearRol(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener roles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Búsqueda con filtro de texto
        public static List<Mdl_Roles> BuscarRoles(
            string textoBusqueda = "",
            int pageNumber = 1,
            int pageSize = 100)
        {
            List<Mdl_Roles> lista = new List<Mdl_Roles>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Roles WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    // Filtro por texto general
                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += " AND (RoleName LIKE @texto OR Description LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    query += " ORDER BY RoleName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearRol(reader));
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

        // MÉTODO PRINCIPAL: Actualizar rol
        public static int ActualizarRol(Mdl_Roles rol)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Roles SET RoleName = @RoleName, Description = @Description 
                        WHERE RoleId = @RoleId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", rol.RoleId);
                        cmd.Parameters.AddWithValue("@RoleName", rol.RoleName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)rol.Description ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar rol
        public static int InactivarRol(int roleId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE Roles SET IsActive = 0 WHERE RoleId = @RoleId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener rol por ID
        public static Mdl_Roles ObtenerRolPorId(int roleId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Roles WHERE RoleId = @RoleId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearRol(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO AUXILIAR: Mapear SqlDataReader a Mdl_Roles
        // Orden de campos en SELECT: RoleId(0), RoleName(1), Description(2), IsActive(3), CreatedDate(4), CreatedBy(5)
        private static Mdl_Roles MapearRol(SqlDataReader reader)
        {
            return new Mdl_Roles
            {
                RoleId = reader.GetInt32(0),
                RoleName = reader[1].ToString(),
                Description = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                IsActive = reader.GetBoolean(3),
                CreatedDate = reader.GetDateTime(4),
                CreatedBy = reader[5] == DBNull.Value ? null : (int?)reader.GetInt32(5)
            };
        }

        // MÉTODOS DE VALIDACIÓN
        public static bool ValidarNombreRolUnico(string roleName, int? excludeRoleId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Roles WHERE RoleName = @RoleName";
                    if (excludeRoleId.HasValue)
                        query += " AND RoleId != @RoleId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleName", roleName ?? "");
                        if (excludeRoleId.HasValue)
                            cmd.Parameters.AddWithValue("@RoleId", excludeRoleId.Value);

                        return (int)cmd.ExecuteScalar() == 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODO PARA OBTENER TODOS LOS ROLES (SIN PAGINACIÓN - PARA COMBOBOX)
        public static List<KeyValuePair<int, string>> ObtenerTodosLosRoles()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT RoleId, RoleName FROM Roles WHERE IsActive = 1 ORDER BY RoleName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new KeyValuePair<int, string>(
                                    reader.GetInt32(0),
                                    reader.GetString(1)
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener roles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA CONTAR TOTAL DE REGISTROS (PARA PAGINACIÓN)
        public static int ContarTotalRoles(string textoBusqueda = "")
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Roles WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += " AND (RoleName LIKE @texto OR Description LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
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

        // MÉTODO PARA ASIGNAR PERMISOS A UN ROL
        public static int AsignarPermisosARol(int roleId, List<int> permissionIds)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // Primero eliminamos los permisos existentes del rol
                    string deleteQuery = "DELETE FROM RolePermissions WHERE RoleId = @RoleId";
                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, connection))
                    {
                        deleteCmd.Parameters.AddWithValue("@RoleId", roleId);
                        deleteCmd.ExecuteNonQuery();
                    }

                    // Luego insertamos los nuevos permisos
                    int count = 0;
                    string insertQuery = "INSERT INTO RolePermissions (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)";
                    foreach (int permissionId in permissionIds)
                    {
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@RoleId", roleId);
                            insertCmd.Parameters.AddWithValue("@PermissionId", permissionId);
                            count += insertCmd.ExecuteNonQuery();
                        }
                    }
                    return count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar permisos al rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PARA OBTENER PERMISOS DE UN ROL
        public static List<int> ObtenerPermisosDeRol(int roleId)
        {
            List<int> permisos = new List<int>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT PermissionId FROM RolePermissions WHERE RoleId = @RoleId";
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
                MessageBox.Show("Error al obtener permisos del rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return permisos;
        }
    }
}