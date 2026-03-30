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
    internal class Ctrl_Positions
    {
        // MÉTODO PRINCIPAL: Registrar posición
        public static int RegistrarPosicion(Mdl_Positions posicion)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Positions (PositionCode, PositionName, Description, 
                        DepartmentId, SalaryRange, IsActive) 
                        VALUES (@PositionCode, @PositionName, @Description, @DepartmentId, 
                        @SalaryRange, @IsActive)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PositionCode", posicion.PositionCode ?? "");
                        cmd.Parameters.AddWithValue("@PositionName", posicion.PositionName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)posicion.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DepartmentId", posicion.DepartmentId);
                        cmd.Parameters.AddWithValue("@SalaryRange", (object)posicion.SalaryRange ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", posicion.IsActive);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar posición: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todas las posiciones con paginación
        public static List<Mdl_Positions> MostrarPosiciones(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_Positions> lista = new List<Mdl_Positions>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM Positions WHERE IsActive = 1 
                        ORDER BY PositionName 
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearPosicion(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener posiciones: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Búsqueda con múltiples filtros
        public static List<Mdl_Positions> BuscarPosiciones(
            string textoBusqueda = "",
            int? departmentId = null,
            int pageNumber = 1,
            int pageSize = 100)
        {
            List<Mdl_Positions> lista = new List<Mdl_Positions>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Positions WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    // Filtro por texto general
                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (PositionCode LIKE @texto OR PositionName LIKE @texto OR 
                            Description LIKE @texto OR SalaryRange LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    // Filtro por departamento
                    if (departmentId.HasValue && departmentId > 0)
                    {
                        query += " AND DepartmentId = @departmentId";
                        parametros.Add(new SqlParameter("@departmentId", departmentId.Value));
                    }

                    query += " ORDER BY PositionName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearPosicion(reader));
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

        // MÉTODO PRINCIPAL: Actualizar posición
        public static int ActualizarPosicion(Mdl_Positions posicion)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Positions SET PositionCode = @PositionCode, 
                        PositionName = @PositionName, Description = @Description, 
                        DepartmentId = @DepartmentId, SalaryRange = @SalaryRange 
                        WHERE PositionId = @PositionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PositionId", posicion.PositionId);
                        cmd.Parameters.AddWithValue("@PositionCode", posicion.PositionCode ?? "");
                        cmd.Parameters.AddWithValue("@PositionName", posicion.PositionName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)posicion.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DepartmentId", posicion.DepartmentId);
                        cmd.Parameters.AddWithValue("@SalaryRange", (object)posicion.SalaryRange ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar posición: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar posición
        public static int InactivarPosicion(int positionId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE Positions SET IsActive = 0 WHERE PositionId = @PositionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PositionId", positionId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar posición: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener posición por ID
        public static Mdl_Positions ObtenerPosicionPorId(int positionId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Positions WHERE PositionId = @PositionId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PositionId", positionId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearPosicion(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener posición: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO AUXILIAR: Mapear SqlDataReader a Mdl_Positions
        // Orden de campos en SELECT: PositionId(0), PositionCode(1), PositionName(2), Description(3), 
        // DepartmentId(4), SalaryRange(5), IsActive(6), CreatedDate(7)
        private static Mdl_Positions MapearPosicion(SqlDataReader reader)
        {
            return new Mdl_Positions
            {
                PositionId = reader.GetInt32(0),
                PositionCode = reader[1].ToString(),
                PositionName = reader[2].ToString(),
                Description = reader[3] == DBNull.Value ? null : reader[3].ToString(),
                DepartmentId = reader.GetInt32(4),
                SalaryRange = reader[5] == DBNull.Value ? null : reader[5].ToString(),
                IsActive = reader.GetBoolean(6),
                CreatedDate = reader.GetDateTime(7)
            };
        }

        // MÉTODOS DE VALIDACIÓN
        public static bool ValidarCodigoPosicionUnico(string codigo, int? excludePositionId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Positions WHERE PositionCode = @Codigo";
                    if (excludePositionId.HasValue)
                        query += " AND PositionId != @PositionId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo ?? "");
                        if (excludePositionId.HasValue)
                            cmd.Parameters.AddWithValue("@PositionId", excludePositionId.Value);

                        return (int)cmd.ExecuteScalar() == 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODOS PARA OBTENER DATOS DE FILTROS
        public static List<KeyValuePair<int, string>> ObtenerDepartamentosParaPosiciones()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT DepartmentId, DepartmentName FROM Departments WHERE IsActive = 1 ORDER BY DepartmentName";
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
                MessageBox.Show("Error al obtener departamentos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA CONTAR TOTAL DE REGISTROS (PARA PAGINACIÓN)
        public static int ContarTotalPosiciones(
            string textoBusqueda = "",
            int? departmentId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Positions WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (PositionCode LIKE @texto OR PositionName LIKE @texto OR 
                            Description LIKE @texto OR SalaryRange LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (departmentId.HasValue && departmentId > 0)
                    {
                        query += " AND DepartmentId = @departmentId";
                        parametros.Add(new SqlParameter("@departmentId", departmentId.Value));
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