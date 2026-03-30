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
    internal class Ctrl_Departments
    {
        // MÉTODO PRINCIPAL: Registrar departamento
        public static int RegistrarDepartamento(Mdl_Departments departamento)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Departments (LocationId, DepartmentCode, DepartmentName, 
                        Description, ManagerEmployeeId, IsActive, CreatedBy) 
                        VALUES (@LocationId, @DepartmentCode, @DepartmentName, @Description, 
                        @ManagerEmployeeId, @IsActive, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", departamento.LocationId);
                        cmd.Parameters.AddWithValue("@DepartmentCode", departamento.DepartmentCode ?? "");
                        cmd.Parameters.AddWithValue("@DepartmentName", departamento.DepartmentName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)departamento.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ManagerEmployeeId", (object)departamento.ManagerEmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", departamento.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)departamento.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar departamento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todos los departamentos con paginación
        public static List<Mdl_Departments> MostrarDepartamentos(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_Departments> lista = new List<Mdl_Departments>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM Departments WHERE IsActive = 1 
                        ORDER BY DepartmentName 
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearDepartamento(reader));
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

        // MÉTODO PRINCIPAL: Búsqueda con múltiples filtros
        public static List<Mdl_Departments> BuscarDepartamentos(
            string textoBusqueda = "",
            int? locationId = null,
            int pageNumber = 1,
            int pageSize = 100)
        {
            List<Mdl_Departments> lista = new List<Mdl_Departments>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Departments WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    // Filtro por texto general
                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (DepartmentCode LIKE @texto OR DepartmentName LIKE @texto OR 
                            Description LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    // Filtro por ubicación
                    if (locationId.HasValue && locationId > 0)
                    {
                        query += " AND LocationId = @locationId";
                        parametros.Add(new SqlParameter("@locationId", locationId.Value));
                    }

                    query += " ORDER BY DepartmentName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearDepartamento(reader));
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

        // MÉTODO PRINCIPAL: Actualizar departamento
        public static int ActualizarDepartamento(Mdl_Departments departamento)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Departments SET LocationId = @LocationId, DepartmentCode = @DepartmentCode, 
                        DepartmentName = @DepartmentName, Description = @Description, 
                        ManagerEmployeeId = @ManagerEmployeeId, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy 
                        WHERE DepartmentId = @DepartmentId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentId", departamento.DepartmentId);
                        cmd.Parameters.AddWithValue("@LocationId", departamento.LocationId);
                        cmd.Parameters.AddWithValue("@DepartmentCode", departamento.DepartmentCode ?? "");
                        cmd.Parameters.AddWithValue("@DepartmentName", departamento.DepartmentName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)departamento.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ManagerEmployeeId", (object)departamento.ManagerEmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)departamento.ModifiedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar departamento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar departamento
        public static int InactivarDepartamento(int departmentId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Departments SET IsActive = 0, ModifiedDate = GETDATE(), 
                        ModifiedBy = @ModifiedBy WHERE DepartmentId = @DepartmentId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar departamento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener departamento por ID
        public static Mdl_Departments ObtenerDepartamentoPorId(int departmentId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Departments WHERE DepartmentId = @DepartmentId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearDepartamento(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener departamento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO AUXILIAR: Mapear SqlDataReader a Mdl_Departments
        // Orden de campos en SELECT: DepartmentId(0), LocationId(1), DepartmentCode(2), DepartmentName(3), 
        // Description(4), ManagerEmployeeId(5), IsActive(6), CreatedDate(7), CreatedBy(8), ModifiedDate(9), ModifiedBy(10)
        private static Mdl_Departments MapearDepartamento(SqlDataReader reader)
        {
            return new Mdl_Departments
            {
                DepartmentId = reader.GetInt32(0),
                LocationId = reader.GetInt32(1),
                DepartmentCode = reader[2].ToString(),
                DepartmentName = reader[3].ToString(),
                Description = reader[4] == DBNull.Value ? null : reader[4].ToString(),
                ManagerEmployeeId = reader[5] == DBNull.Value ? null : (int?)reader.GetInt32(5),
                IsActive = reader.GetBoolean(6),
                CreatedDate = reader.GetDateTime(7),
                CreatedBy = reader[8] == DBNull.Value ? null : (int?)reader.GetInt32(8),
                ModifiedDate = reader[9] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(9),
                ModifiedBy = reader[10] == DBNull.Value ? null : (int?)reader.GetInt32(10)
            };
        }

        // MÉTODOS DE VALIDACIÓN
        public static bool ValidarCodigoDepartamentoUnico(string codigo, int? excludeDepartmentId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Departments WHERE DepartmentCode = @Codigo";
                    if (excludeDepartmentId.HasValue)
                        query += " AND DepartmentId != @DepartmentId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo ?? "");
                        if (excludeDepartmentId.HasValue)
                            cmd.Parameters.AddWithValue("@DepartmentId", excludeDepartmentId.Value);

                        return (int)cmd.ExecuteScalar() == 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODOS PARA OBTENER DATOS DE FILTROS
        public static List<KeyValuePair<int, string>> ObtenerLocaciones()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT LocationId, LocationName FROM Locations WHERE IsActive = 1 ORDER BY LocationName";
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
                MessageBox.Show("Error al obtener locaciones: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static List<KeyValuePair<int, string>> ObtenerEmpleadosParaManager()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT EmployeeId, FullName FROM Employees WHERE IsActive = 1 ORDER BY FullName";
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
                MessageBox.Show("Error al obtener empleados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA CONTAR TOTAL DE REGISTROS (PARA PAGINACIÓN)
        public static int ContarTotalDepartamentos(
            string textoBusqueda = "",
            int? locationId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Departments WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (DepartmentCode LIKE @texto OR DepartmentName LIKE @texto OR 
                            Description LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (locationId.HasValue && locationId > 0)
                    {
                        query += " AND LocationId = @locationId";
                        parametros.Add(new SqlParameter("@locationId", locationId.Value));
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