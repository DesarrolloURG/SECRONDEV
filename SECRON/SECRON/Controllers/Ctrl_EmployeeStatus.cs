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
    internal class Ctrl_EmployeeStatus
    {
        // MÉTODO PRINCIPAL: Registrar estado de empleado
        public static int RegistrarEstadoEmpleado(Mdl_EmployeeStatus estado)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO EmployeeStatus (StatusName, Description, IsActive) 
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
                MessageBox.Show("Error al registrar estado de empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todos los estados de empleado con paginación
        public static List<Mdl_EmployeeStatus> MostrarEstadosEmpleado(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_EmployeeStatus> lista = new List<Mdl_EmployeeStatus>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM EmployeeStatus WHERE IsActive = 1 
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
                                lista.Add(MapearEstadoEmpleado(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener estados de empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Búsqueda con filtro de texto
        public static List<Mdl_EmployeeStatus> BuscarEstadosEmpleado(
            string textoBusqueda = "",
            int pageNumber = 1,
            int pageSize = 100)
        {
            List<Mdl_EmployeeStatus> lista = new List<Mdl_EmployeeStatus>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM EmployeeStatus WHERE IsActive = 1";
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
                                lista.Add(MapearEstadoEmpleado(reader));
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

        // MÉTODO PRINCIPAL: Actualizar estado de empleado
        public static int ActualizarEstadoEmpleado(Mdl_EmployeeStatus estado)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE EmployeeStatus SET StatusName = @StatusName, 
                        Description = @Description 
                        WHERE EmployeeStatusId = @EmployeeStatusId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeStatusId", estado.EmployeeStatusId);
                        cmd.Parameters.AddWithValue("@StatusName", estado.StatusName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)estado.Description ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar estado de empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar estado de empleado
        public static int InactivarEstadoEmpleado(int employeeStatusId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE EmployeeStatus SET IsActive = 0 WHERE EmployeeStatusId = @EmployeeStatusId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeStatusId", employeeStatusId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar estado de empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener estado de empleado por ID
        public static Mdl_EmployeeStatus ObtenerEstadoEmpleadoPorId(int employeeStatusId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM EmployeeStatus WHERE EmployeeStatusId = @EmployeeStatusId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeStatusId", employeeStatusId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearEstadoEmpleado(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener estado de empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO AUXILIAR: Mapear SqlDataReader a Mdl_EmployeeStatus
        // Orden de campos en SELECT: EmployeeStatusId(0), StatusName(1), Description(2), IsActive(3)
        private static Mdl_EmployeeStatus MapearEstadoEmpleado(SqlDataReader reader)
        {
            return new Mdl_EmployeeStatus
            {
                EmployeeStatusId = reader.GetInt32(0),
                StatusName = reader[1].ToString(),
                Description = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                IsActive = reader.GetBoolean(3)
            };
        }

        // MÉTODOS DE VALIDACIÓN
        public static bool ValidarNombreEstadoUnico(string statusName, int? excludeEmployeeStatusId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM EmployeeStatus WHERE StatusName = @StatusName";
                    if (excludeEmployeeStatusId.HasValue)
                        query += " AND EmployeeStatusId != @EmployeeStatusId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StatusName", statusName ?? "");
                        if (excludeEmployeeStatusId.HasValue)
                            cmd.Parameters.AddWithValue("@EmployeeStatusId", excludeEmployeeStatusId.Value);

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
                    string query = "SELECT EmployeeStatusId, StatusName FROM EmployeeStatus WHERE IsActive = 1 ORDER BY StatusName";
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
        public static int ContarTotalEstadosEmpleado(string textoBusqueda = "")
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM EmployeeStatus WHERE IsActive = 1";
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