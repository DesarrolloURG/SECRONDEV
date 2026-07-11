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
        // NOTA: LocationId (indice 14) se lee como int? porque en Coordinators es NULLABLE
        private static Mdl_Coordinators MapearCoordinador(SqlDataReader reader)
        {
            return new Mdl_Coordinators
            {
                CoordinatorId = reader.GetInt32(0),
                CoordinatorCode = reader[1].ToString(),
                FullName = reader[2].ToString(),
                Phone = reader[3] == DBNull.Value ? null : reader[3].ToString(),
                Email = reader[4] == DBNull.Value ? null : reader[4].ToString(),
                DPI = reader[5] == DBNull.Value ? null : reader[5].ToString(),
                NIT = reader[6] == DBNull.Value ? null : reader[6].ToString(),
                Address = reader[7] == DBNull.Value ? null : reader[7].ToString(),
                AcademicTitle = reader[8] == DBNull.Value ? null : reader[8].ToString(),
                Specialization = reader[9] == DBNull.Value ? null : reader[9].ToString(),
                IsCollegiateActive = reader.GetBoolean(10),
                CollegiateNumber = reader[11] == DBNull.Value ? null : reader[11].ToString(),
                BankAccountNumber = reader[12] == DBNull.Value ? null : reader[12].ToString(),
                BankId = reader[13] == DBNull.Value ? null : (int?)reader.GetInt32(13),
                LocationId = reader[14] == DBNull.Value ? null : (int?)reader.GetInt32(14),
                HireDate = reader[15] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(15),
                ContractType = reader[16] == DBNull.Value ? null : reader[16].ToString(),
                UserId = reader[17] == DBNull.Value ? null : (int?)reader.GetInt32(17),
                RegisteredByCoordinatorId = reader[18] == DBNull.Value ? null : (int?)reader.GetInt32(18),
                IsActive = reader.GetBoolean(19),
                CreatedDate = reader.GetDateTime(20),
                CreatedBy = reader[21] == DBNull.Value ? null : (int?)reader.GetInt32(21),
                ModifiedDate = reader[22] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(22),
                ModifiedBy = reader[23] == DBNull.Value ? null : (int?)reader.GetInt32(23),
                FilePath_DPI = reader[24] == DBNull.Value ? null : reader[24].ToString(),
                FilePath_Titulos = reader[25] == DBNull.Value ? null : reader[25].ToString(),
                FilePath_RTU = reader[26] == DBNull.Value ? null : reader[26].ToString(),
                FilePath_Colegiado = reader[27] == DBNull.Value ? null : reader[27].ToString(),
                FilePath_RENAS = reader[28] == DBNull.Value ? null : reader[28].ToString(),
                FilePath_AntPoliciacos = reader[29] == DBNull.Value ? null : reader[29].ToString(),
                FilePath_AntPenales = reader[30] == DBNull.Value ? null : reader[30].ToString()
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