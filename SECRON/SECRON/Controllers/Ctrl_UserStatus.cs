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
    internal class Ctrl_UserStatus
    {
        // MÉTODO PRINCIPAL: Registrar estado de usuario
        public static int RegistrarEstadoUsuario(Mdl_UserStatus estado)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO UserStatus (StatusName, Description, IsActive) 
                        VALUES (@StatusName, @Description, @IsActive)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StatusName", estado.StatusName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)estado.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", estado.IsActive);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar estado de usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todos los estados de usuario con paginación
        public static List<Mdl_UserStatus> MostrarEstadosUsuario(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_UserStatus> lista = new List<Mdl_UserStatus>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM UserStatus WHERE IsActive = 1 
                        ORDER BY StatusName 
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearEstadoUsuario(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener estados de usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Búsqueda con filtro de texto
        public static List<Mdl_UserStatus> BuscarEstadosUsuario(
            string textoBusqueda = "",
            int pageNumber = 1,
            int pageSize = 100)
        {
            List<Mdl_UserStatus> lista = new List<Mdl_UserStatus>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM UserStatus WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    // Filtro por texto general
                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += " AND (StatusName LIKE @texto OR Description LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    query += " ORDER BY StatusName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearEstadoUsuario(reader));
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

        // MÉTODO PRINCIPAL: Actualizar estado de usuario
        public static int ActualizarEstadoUsuario(Mdl_UserStatus estado)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE UserStatus SET StatusName = @StatusName, 
                        Description = @Description 
                        WHERE StatusId = @StatusId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StatusId", estado.StatusId);
                        cmd.Parameters.AddWithValue("@StatusName", estado.StatusName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)estado.Description ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar estado de usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar estado de usuario
        public static int InactivarEstadoUsuario(int statusId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE UserStatus SET IsActive = 0 WHERE StatusId = @StatusId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StatusId", statusId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar estado de usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener estado de usuario por ID
        public static Mdl_UserStatus ObtenerEstadoUsuarioPorId(int statusId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM UserStatus WHERE StatusId = @StatusId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StatusId", statusId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearEstadoUsuario(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener estado de usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO AUXILIAR: Mapear SqlDataReader a Mdl_UserStatus
        // Orden de campos en SELECT: StatusId(0), StatusName(1), Description(2), IsActive(3)
        private static Mdl_UserStatus MapearEstadoUsuario(SqlDataReader reader)
        {
            return new Mdl_UserStatus
            {
                StatusId = reader.GetInt32(0),
                StatusName = reader[1].ToString(),
                Description = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                IsActive = reader.GetBoolean(3)
            };
        }

        // MÉTODOS DE VALIDACIÓN
        public static bool ValidarNombreEstadoUnico(string statusName, int? excludeStatusId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM UserStatus WHERE StatusName = @StatusName";
                    if (excludeStatusId.HasValue)
                        query += " AND StatusId != @StatusId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StatusName", statusName ?? "");
                        if (excludeStatusId.HasValue)
                            cmd.Parameters.AddWithValue("@StatusId", excludeStatusId.Value);

                        return (int)cmd.ExecuteScalar() == 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODO PARA OBTENER TODOS LOS ESTADOS (SIN PAGINACIÓN - PARA COMBOBOX)
        public static List<KeyValuePair<int, string>> ObtenerTodosLosEstados()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT StatusId, StatusName FROM UserStatus WHERE IsActive = 1 ORDER BY StatusName";
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
                MessageBox.Show("Error al obtener estados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA CONTAR TOTAL DE REGISTROS (PARA PAGINACIÓN)
        public static int ContarTotalEstadosUsuario(string textoBusqueda = "")
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM UserStatus WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += " AND (StatusName LIKE @texto OR Description LIKE @texto)";
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
    }
}