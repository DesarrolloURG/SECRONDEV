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
    internal class Ctrl_Permissions
    {
        // MÉTODO PRINCIPAL: Registrar permiso
        public static int RegistrarPermiso(Mdl_Permissions permiso)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Permissions (PermissionCode, PermissionName, Description, 
                        ModuleName, ActionType, IsActive) 
                        VALUES (@PermissionCode, @PermissionName, @Description, @ModuleName, 
                        @ActionType, @IsActive)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PermissionCode", permiso.PermissionCode ?? "");
                        cmd.Parameters.AddWithValue("@PermissionName", permiso.PermissionName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)permiso.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModuleName", (object)permiso.ModuleName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionType", (object)permiso.ActionType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", permiso.IsActive);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar permiso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todos los permisos con paginación
        public static List<Mdl_Permissions> MostrarPermisos(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_Permissions> lista = new List<Mdl_Permissions>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM Permissions WHERE IsActive = 1 
                        ORDER BY ModuleName, PermissionName 
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearPermiso(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener permisos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Búsqueda con múltiples filtros
        public static List<Mdl_Permissions> BuscarPermisos(
            string textoBusqueda = "",
            string moduleName = "",
            string actionType = "",
            int pageNumber = 1,
            int pageSize = 100)
        {
            List<Mdl_Permissions> lista = new List<Mdl_Permissions>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Permissions WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    // Filtro por texto general
                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (PermissionCode LIKE @texto OR PermissionName LIKE @texto OR 
                            Description LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    // Filtro por módulo
                    if (!string.IsNullOrWhiteSpace(moduleName))
                    {
                        query += " AND ModuleName = @moduleName";
                        parametros.Add(new SqlParameter("@moduleName", moduleName.Trim()));
                    }

                    // Filtro por tipo de acción
                    if (!string.IsNullOrWhiteSpace(actionType))
                    {
                        query += " AND ActionType = @actionType";
                        parametros.Add(new SqlParameter("@actionType", actionType.Trim()));
                    }

                    query += " ORDER BY ModuleName, PermissionName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearPermiso(reader));
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

        // MÉTODO PRINCIPAL: Actualizar permiso
        public static int ActualizarPermiso(Mdl_Permissions permiso)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Permissions SET PermissionCode = @PermissionCode, 
                        PermissionName = @PermissionName, Description = @Description, 
                        ModuleName = @ModuleName, ActionType = @ActionType 
                        WHERE PermissionId = @PermissionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PermissionId", permiso.PermissionId);
                        cmd.Parameters.AddWithValue("@PermissionCode", permiso.PermissionCode ?? "");
                        cmd.Parameters.AddWithValue("@PermissionName", permiso.PermissionName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)permiso.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModuleName", (object)permiso.ModuleName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionType", (object)permiso.ActionType ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar permiso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar permiso
        public static int InactivarPermiso(int permissionId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE Permissions SET IsActive = 0 WHERE PermissionId = @PermissionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PermissionId", permissionId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar permiso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener permiso por ID
        public static Mdl_Permissions ObtenerPermisoPorId(int permissionId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Permissions WHERE PermissionId = @PermissionId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PermissionId", permissionId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearPermiso(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener permiso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO AUXILIAR: Mapear SqlDataReader a Mdl_Permissions
        // Orden de campos en SELECT: PermissionId(0), PermissionCode(1), PermissionName(2), Description(3), 
        // ModuleName(4), ActionType(5), IsActive(6), CreatedDate(7)
        private static Mdl_Permissions MapearPermiso(SqlDataReader reader)
        {
            return new Mdl_Permissions
            {
                PermissionId = reader.GetInt32(0),
                PermissionCode = reader[1].ToString(),
                PermissionName = reader[2].ToString(),
                Description = reader[3] == DBNull.Value ? null : reader[3].ToString(),
                ModuleName = reader[4] == DBNull.Value ? null : reader[4].ToString(),
                ActionType = reader[5] == DBNull.Value ? null : reader[5].ToString(),
                IsActive = reader.GetBoolean(6),
                CreatedDate = reader.GetDateTime(7)
            };
        }

        // MÉTODOS DE VALIDACIÓN
        public static bool ValidarCodigoPermisoUnico(string codigo, int? excludePermissionId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Permissions WHERE PermissionCode = @Codigo";
                    if (excludePermissionId.HasValue)
                        query += " AND PermissionId != @PermissionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo ?? "");
                        if (excludePermissionId.HasValue)
                            cmd.Parameters.AddWithValue("@PermissionId", excludePermissionId.Value);

                        return (int)cmd.ExecuteScalar() == 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODO PARA OBTENER TODOS LOS PERMISOS (SIN PAGINACIÓN - PARA ASIGNACIÓN)
        public static List<KeyValuePair<int, string>> ObtenerTodosLosPermisos()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT PermissionId, PermissionName FROM Permissions 
                        WHERE IsActive = 1 ORDER BY ModuleName, PermissionName";
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
                MessageBox.Show("Error al obtener permisos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA OBTENER MÓDULOS ÚNICOS (PARA FILTRO)
        public static List<string> ObtenerModulos()
        {
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT DISTINCT ModuleName FROM Permissions 
                        WHERE IsActive = 1 AND ModuleName IS NOT NULL ORDER BY ModuleName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener módulos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA OBTENER TIPOS DE ACCIÓN ÚNICOS (PARA FILTRO)
        public static List<string> ObtenerTiposAccion()
        {
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT DISTINCT ActionType FROM Permissions 
                        WHERE IsActive = 1 AND ActionType IS NOT NULL ORDER BY ActionType";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener tipos de acción: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA CONTAR TOTAL DE REGISTROS (PARA PAGINACIÓN)
        public static int ContarTotalPermisos(
            string textoBusqueda = "",
            string moduleName = "",
            string actionType = "")
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Permissions WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (PermissionCode LIKE @texto OR PermissionName LIKE @texto OR 
                            Description LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (!string.IsNullOrWhiteSpace(moduleName))
                    {
                        query += " AND ModuleName = @moduleName";
                        parametros.Add(new SqlParameter("@moduleName", moduleName.Trim()));
                    }

                    if (!string.IsNullOrWhiteSpace(actionType))
                    {
                        query += " AND ActionType = @actionType";
                        parametros.Add(new SqlParameter("@actionType", actionType.Trim()));
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
    }
}