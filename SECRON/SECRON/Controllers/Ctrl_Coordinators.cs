using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_Coordinators
    {
        // MÉTODO AUXILIAR: Generar próximo código de coordinador
        public static string ObtenerProximoCodigoCoordinador()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT TOP 1 CoordinatorCode 
                                   FROM Coordinators 
                                   WHERE CoordinatorCode IS NOT NULL 
                                   ORDER BY CoordinatorId DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && !string.IsNullOrWhiteSpace(resultado.ToString()))
                        {
                            string ultimoCodigo = resultado.ToString();

                            if (int.TryParse(ultimoCodigo, out int numeroActual))
                            {
                                int proximoNumero = numeroActual + 1;
                                return proximoNumero.ToString("D6");
                            }
                            else
                            {
                                string soloNumeros = new string(ultimoCodigo.Where(char.IsDigit).ToArray());

                                if (!string.IsNullOrWhiteSpace(soloNumeros) && int.TryParse(soloNumeros, out int numExtraido))
                                {
                                    int proximoNumero = numExtraido + 1;
                                    string prefijo = new string(ultimoCodigo.Where(char.IsLetter).ToArray());
                                    return $"{prefijo}{proximoNumero:D6}";
                                }
                                else
                                {
                                    return "000001";
                                }
                            }
                        }
                        else
                        {
                            return "000001";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de coordinador: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "ERROR";
            }
        }

        // MÉTODO PRINCIPAL: Registrar coordinador
        public static int RegistrarCoordinador(Mdl_Coordinators coordinador)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_Coordinators_Insert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CoordinatorCode", coordinador.CoordinatorCode ?? "");
                    cmd.Parameters.AddWithValue("@FullName", coordinador.FullName?.ToUpper() ?? "");
                    cmd.Parameters.AddWithValue("@Phone", (object)coordinador.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)coordinador.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DPI", (object)coordinador.DPI ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NIT", (object)coordinador.NIT ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)coordinador.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AcademicTitle", (object)coordinador.AcademicTitle?.ToUpper() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Specialization", (object)coordinador.Specialization ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsCollegiateActive", coordinador.IsCollegiateActive);
                    cmd.Parameters.AddWithValue("@CollegiateNumber", (object)coordinador.CollegiateNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankAccountNumber", (object)coordinador.BankAccountNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankId", (object)coordinador.BankId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LocationId", (object)coordinador.LocationId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HireDate", (object)coordinador.HireDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContractType", (object)coordinador.ContractType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", (object)coordinador.UserId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RegisteredByCoordinatorId", (object)coordinador.RegisteredByCoordinatorId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", coordinador.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object)coordinador.CreatedBy ?? DBNull.Value);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar coordinador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar coordinadores activos
        public static List<Mdl_Coordinators> MostrarCoordinadores()
        {
            List<Mdl_Coordinators> lista = new List<Mdl_Coordinators>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Coordinators WHERE IsActive = 1 ORDER BY FullName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCoordinador(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener coordinadores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Mostrar todos los coordinadores (incluyendo inactivos)
        public static List<Mdl_Coordinators> MostrarTodosCoordinadores()
        {
            List<Mdl_Coordinators> lista = new List<Mdl_Coordinators>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Coordinators ORDER BY IsActive DESC, FullName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCoordinador(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener coordinadores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Buscar coordinador por ID
        public static Mdl_Coordinators ObtenerCoordinadorPorId(int coordinatorId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Coordinators WHERE CoordinatorId = @CoordinatorId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CoordinatorId", coordinatorId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearCoordinador(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener coordinador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Buscar coordinador por código
        public static Mdl_Coordinators ObtenerCoordinadorPorCodigo(string coordinatorCode)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Coordinators WHERE CoordinatorCode = @CoordinatorCode";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CoordinatorCode", coordinatorCode ?? "");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearCoordinador(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar coordinador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Buscar coordinador por DPI
        public static Mdl_Coordinators ObtenerCoordinadorPorDPI(string dpi)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Coordinators WHERE DPI = @DPI";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DPI", dpi ?? "");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearCoordinador(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar coordinador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Buscar coordinadores por nombre
        public static List<Mdl_Coordinators> BuscarPorNombre(string nombre)
        {
            List<Mdl_Coordinators> lista = new List<Mdl_Coordinators>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Coordinators WHERE FullName LIKE @Nombre AND IsActive = 1 ORDER BY FullName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", "%" + (nombre?.ToUpper() ?? "") + "%");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCoordinador(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar coordinadores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Buscar coordinadores por sede
        public static List<Mdl_Coordinators> ObtenerCoordinadoresPorSede(int locationId)
        {
            List<Mdl_Coordinators> lista = new List<Mdl_Coordinators>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Coordinators WHERE LocationId = @LocationId AND IsActive = 1 ORDER BY FullName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCoordinador(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener coordinadores por sede: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Actualizar coordinador
        public static int ActualizarCoordinador(Mdl_Coordinators coordinador)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_Coordinators_Update", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CoordinatorId", coordinador.CoordinatorId);
                    cmd.Parameters.AddWithValue("@Mode", 0);
                    cmd.Parameters.AddWithValue("@CoordinatorCode", coordinador.CoordinatorCode ?? "");
                    cmd.Parameters.AddWithValue("@FullName", coordinador.FullName?.ToUpper() ?? "");
                    cmd.Parameters.AddWithValue("@Phone", (object)coordinador.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)coordinador.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DPI", (object)coordinador.DPI ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NIT", (object)coordinador.NIT ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)coordinador.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AcademicTitle", (object)coordinador.AcademicTitle?.ToUpper() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Specialization", (object)coordinador.Specialization ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsCollegiateActive", coordinador.IsCollegiateActive);
                    cmd.Parameters.AddWithValue("@CollegiateNumber", (object)coordinador.CollegiateNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankAccountNumber", (object)coordinador.BankAccountNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankId", (object)coordinador.BankId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LocationId", (object)coordinador.LocationId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HireDate", (object)coordinador.HireDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContractType", (object)coordinador.ContractType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", (object)coordinador.UserId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RegisteredByCoordinatorId", (object)coordinador.RegisteredByCoordinatorId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)coordinador.ModifiedBy ?? DBNull.Value);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar coordinador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar coordinador
        public static int InactivarCoordinador(int coordinatorId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_Coordinators_Update", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CoordinatorId", coordinatorId);
                    cmd.Parameters.AddWithValue("@Mode", 1);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar coordinador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Reactivar coordinador
        public static int ReactivarCoordinador(int coordinatorId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_Coordinators_Update", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CoordinatorId", coordinatorId);
                    cmd.Parameters.AddWithValue("@Mode", 2);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al reactivar coordinador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PARA COMBOBOX: Obtener lista simple de coordinadores
        public static List<KeyValuePair<int, string>> ObtenerCoordinadoresParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT CoordinatorId, FullName FROM Coordinators WHERE IsActive = 1 ORDER BY FullName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener coordinadores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO AUXILIAR: Mapear datos del SqlDataReader al modelo Mdl_Coordinators
        // Lee por NOMBRE de columna para ser inmune al orden fisico de la tabla
        private static Mdl_Coordinators MapearCoordinador(SqlDataReader reader)
        {
            return new Mdl_Coordinators
            {
                CoordinatorId = Convert.ToInt32(reader["CoordinatorId"]),
                CoordinatorCode = reader["CoordinatorCode"]?.ToString(),
                FullName = reader["FullName"]?.ToString(),
                Phone = reader["Phone"] == DBNull.Value ? null : reader["Phone"].ToString(),
                Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                DPI = reader["DPI"] == DBNull.Value ? null : reader["DPI"].ToString(),
                NIT = reader["NIT"] == DBNull.Value ? null : reader["NIT"].ToString(),
                Address = reader["Address"] == DBNull.Value ? null : reader["Address"].ToString(),
                AcademicTitle = reader["AcademicTitle"] == DBNull.Value ? null : reader["AcademicTitle"].ToString(),
                Specialization = reader["Specialization"] == DBNull.Value ? null : reader["Specialization"].ToString(),
                IsCollegiateActive = reader["IsCollegiateActive"] != DBNull.Value && Convert.ToBoolean(reader["IsCollegiateActive"]),
                CollegiateNumber = reader["CollegiateNumber"] == DBNull.Value ? null : reader["CollegiateNumber"].ToString(),
                BankAccountNumber = reader["BankAccountNumber"] == DBNull.Value ? null : reader["BankAccountNumber"].ToString(),
                BankId = reader["BankId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["BankId"]),
                LocationId = reader["LocationId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["LocationId"]),
                HireDate = reader["HireDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["HireDate"]),
                ContractType = reader["ContractType"] == DBNull.Value ? null : reader["ContractType"].ToString(),
                UserId = reader["UserId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["UserId"]),
                RegisteredByCoordinatorId = reader["RegisteredByCoordinatorId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["RegisteredByCoordinatorId"]),
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["CreatedDate"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CreatedBy"]),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ModifiedDate"]),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ModifiedBy"]),
                FilePath_DPI = reader["FilePath_DPI"] == DBNull.Value ? null : reader["FilePath_DPI"].ToString(),
                FilePath_Titulos = reader["FilePath_Titulos"] == DBNull.Value ? null : reader["FilePath_Titulos"].ToString(),
                FilePath_RTU = reader["FilePath_RTU"] == DBNull.Value ? null : reader["FilePath_RTU"].ToString(),
                FilePath_Colegiado = reader["FilePath_Colegiado"] == DBNull.Value ? null : reader["FilePath_Colegiado"].ToString(),
                FilePath_RENAS = reader["FilePath_RENAS"] == DBNull.Value ? null : reader["FilePath_RENAS"].ToString(),
                FilePath_AntPoliciacos = reader["FilePath_AntPoliciacos"] == DBNull.Value ? null : reader["FilePath_AntPoliciacos"].ToString(),
                FilePath_AntPenales = reader["FilePath_AntPenales"] == DBNull.Value ? null : reader["FilePath_AntPenales"].ToString()
            };
        }

        // MÉTODO: Actualizar ruta de archivo de un coordinador
        public static bool ActualizarFilePathCoordinador(int coordinatorId, string campo, string ruta, int modifiedBy)
        {
            try
            {
                string[] camposPermitidos = {
                    "FilePath_DPI", "FilePath_Titulos", "FilePath_RTU",
                    "FilePath_Colegiado", "FilePath_RENAS",
                    "FilePath_AntPoliciacos", "FilePath_AntPenales"
                };

                if (!Array.Exists(camposPermitidos, c => c == campo))
                {
                    MessageBox.Show("CAMPO DE ARCHIVO NO VÁLIDO.", "ERROR SECRON",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_Coordinators_UpdateFilePath", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CoordinatorId", coordinatorId);
                    cmd.Parameters.AddWithValue("@Campo", campo);
                    cmd.Parameters.AddWithValue("@Ruta", (object)ruta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    object result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR ARCHIVO: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}