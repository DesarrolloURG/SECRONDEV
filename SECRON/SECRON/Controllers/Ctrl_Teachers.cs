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
    internal class Ctrl_Teachers
    {
        // MÉTODO AUXILIAR: Generar próximo código de docente
        // Obtiene el último código registrado, lo incrementa y retorna el nuevo código
        // Formato: 000001, 000002, etc. o con prefijo si existe
        public static string ObtenerProximoCodigoDocente()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // Obtener el último código registrado
                    string query = @"SELECT TOP 1 TeacherCode 
                                   FROM Teachers 
                                   WHERE TeacherCode IS NOT NULL 
                                   ORDER BY TeacherId DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && !string.IsNullOrWhiteSpace(resultado.ToString()))
                        {
                            string ultimoCodigo = resultado.ToString();

                            // Intentar convertir a número
                            if (int.TryParse(ultimoCodigo, out int numeroActual))
                            {
                                // Si es número, sumar 1
                                int proximoNumero = numeroActual + 1;
                                return proximoNumero.ToString("D6"); // Formato: 000001, 000002, etc.
                            }
                            else
                            {
                                // Si contiene letras y números, intentar extraer el número
                                string soloNumeros = new string(ultimoCodigo.Where(char.IsDigit).ToArray());

                                if (!string.IsNullOrWhiteSpace(soloNumeros) && int.TryParse(soloNumeros, out int numExtraido))
                                {
                                    int proximoNumero = numExtraido + 1;
                                    // Mantener el prefijo de letras si existe
                                    string prefijo = new string(ultimoCodigo.Where(char.IsLetter).ToArray());
                                    return $"{prefijo}{proximoNumero:D6}";
                                }
                                else
                                {
                                    // Si no se puede extraer número, empezar desde 1
                                    return "000001";
                                }
                            }
                        }
                        else
                        {
                            // Si no hay registros, empezar desde 000001
                            return "000001";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de docente: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "ERROR";
            }
        }

        // MÉTODO PRINCIPAL: Registrar docente
        // Inserta un nuevo registro de docente en la base de datos con todos sus datos
        public static int RegistrarDocente(Mdl_Teachers docente)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Teachers (TeacherCode, FullName, Phone, Email, DPI, NIT, 
                        Address, AcademicTitle, Specialization, IsCollegiateActive, CollegiateNumber, 
                        BankAccountNumber, BankId, LocationId, HireDate, ContractType, UserId, 
                        RegisteredByCoordinatorId, IsActive, CreatedDate, CreatedBy) 
                        VALUES (@TeacherCode, @FullName, @Phone, @Email, @DPI, @NIT, @Address, 
                        @AcademicTitle, @Specialization, @IsCollegiateActive, @CollegiateNumber, 
                        @BankAccountNumber, @BankId, @LocationId, @HireDate, @ContractType, @UserId, 
                        @RegisteredByCoordinatorId, @IsActive, GETDATE(), @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TeacherCode", docente.TeacherCode ?? "");
                        cmd.Parameters.AddWithValue("@FullName", docente.FullName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@Phone", (object)docente.Phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", (object)docente.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DPI", (object)docente.DPI ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NIT", (object)docente.NIT ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object)docente.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicTitle", (object)docente.AcademicTitle?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Specialization", (object)docente.Specialization ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsCollegiateActive", docente.IsCollegiateActive);
                        cmd.Parameters.AddWithValue("@CollegiateNumber", (object)docente.CollegiateNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankAccountNumber", (object)docente.BankAccountNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankId", (object)docente.BankId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LocationId", docente.LocationId);
                        cmd.Parameters.AddWithValue("@HireDate", (object)docente.HireDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContractType", (object)docente.ContractType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", (object)docente.UserId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RegisteredByCoordinatorId", (object)docente.RegisteredByCoordinatorId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", docente.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)docente.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar docente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar docentes activos
        // Obtiene lista de todos los docentes activos ordenados por nombre
        public static List<Mdl_Teachers> MostrarDocentes()
        {
            List<Mdl_Teachers> lista = new List<Mdl_Teachers>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Teachers WHERE IsActive = 1 ORDER BY FullName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearDocente(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener docentes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Mostrar todos los docentes (incluyendo inactivos)
        // Obtiene lista completa de docentes ordenados por estado y nombre
        public static List<Mdl_Teachers> MostrarTodosDocentes()
        {
            List<Mdl_Teachers> lista = new List<Mdl_Teachers>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Teachers ORDER BY IsActive DESC, FullName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearDocente(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener docentes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Buscar docente por ID
        // Obtiene un docente específico por su identificador único
        public static Mdl_Teachers ObtenerDocentePorId(int teacherId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Teachers WHERE TeacherId = @TeacherId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearDocente(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener docente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Buscar docente por código
        // Obtiene un docente específico por su código único
        public static Mdl_Teachers ObtenerDocentePorCodigo(string teacherCode)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Teachers WHERE TeacherCode = @TeacherCode";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TeacherCode", teacherCode ?? "");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearDocente(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar docente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Buscar docente por DPI
        // Obtiene un docente específico por su número de DPI
        public static Mdl_Teachers ObtenerDocentePorDPI(string dpi)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Teachers WHERE DPI = @DPI";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DPI", dpi ?? "");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearDocente(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar docente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Buscar docentes por nombre
        // Obtiene lista de docentes que coincidan con el criterio de búsqueda en el nombre
        public static List<Mdl_Teachers> BuscarPorNombre(string nombre)
        {
            List<Mdl_Teachers> lista = new List<Mdl_Teachers>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Teachers WHERE FullName LIKE @Nombre AND IsActive = 1 ORDER BY FullName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", "%" + (nombre?.ToUpper() ?? "") + "%");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearDocente(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar docentes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Buscar docentes por sede
        // Obtiene lista de docentes asignados a una sede específica
        public static List<Mdl_Teachers> ObtenerDocentesPorSede(int locationId)
        {
            List<Mdl_Teachers> lista = new List<Mdl_Teachers>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Teachers WHERE LocationId = @LocationId AND IsActive = 1 ORDER BY FullName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearDocente(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener docentes por sede: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Actualizar docente
        // Actualiza todos los datos de un docente existente
        public static int ActualizarDocente(Mdl_Teachers docente)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Teachers SET TeacherCode = @TeacherCode, FullName = @FullName, 
                        Phone = @Phone, Email = @Email, DPI = @DPI, NIT = @NIT, Address = @Address, 
                        AcademicTitle = @AcademicTitle, Specialization = @Specialization, 
                        IsCollegiateActive = @IsCollegiateActive, CollegiateNumber = @CollegiateNumber, 
                        BankAccountNumber = @BankAccountNumber, BankId = @BankId, LocationId = @LocationId, 
                        HireDate = @HireDate, ContractType = @ContractType, UserId = @UserId, 
                        RegisteredByCoordinatorId = @RegisteredByCoordinatorId, 
                        ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy 
                        WHERE TeacherId = @TeacherId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TeacherId", docente.TeacherId);
                        cmd.Parameters.AddWithValue("@TeacherCode", docente.TeacherCode ?? "");
                        cmd.Parameters.AddWithValue("@FullName", docente.FullName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@Phone", (object)docente.Phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", (object)docente.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DPI", (object)docente.DPI ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NIT", (object)docente.NIT ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object)docente.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicTitle", (object)docente.AcademicTitle?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Specialization", (object)docente.Specialization ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsCollegiateActive", docente.IsCollegiateActive);
                        cmd.Parameters.AddWithValue("@CollegiateNumber", (object)docente.CollegiateNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankAccountNumber", (object)docente.BankAccountNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankId", (object)docente.BankId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LocationId", docente.LocationId);
                        cmd.Parameters.AddWithValue("@HireDate", (object)docente.HireDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContractType", (object)docente.ContractType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", (object)docente.UserId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RegisteredByCoordinatorId", (object)docente.RegisteredByCoordinatorId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)docente.ModifiedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar docente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar docente
        // Marca un docente como inactivo (borrado lógico)
        public static int InactivarDocente(int teacherId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Teachers SET IsActive = 0, ModifiedDate = GETDATE(), 
                        ModifiedBy = @ModifiedBy WHERE TeacherId = @TeacherId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar docente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Reactivar docente
        // Marca un docente inactivo como activo nuevamente
        public static int ReactivarDocente(int teacherId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Teachers SET IsActive = 1, ModifiedDate = GETDATE(), 
                        ModifiedBy = @ModifiedBy WHERE TeacherId = @TeacherId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al reactivar docente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PARA COMBOBOX: Obtener lista simple de docentes
        // Retorna pares clave-valor para poblar ComboBox (ID, Nombre)
        public static List<KeyValuePair<int, string>> ObtenerDocentesParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT TeacherId, FullName FROM Teachers WHERE IsActive = 1 ORDER BY FullName";
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
                MessageBox.Show("Error al obtener docentes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO AUXILIAR: Mapear datos del SqlDataReader al modelo Mdl_Teachers
        // Convierte cada registro de la base de datos en un objeto del modelo
        private static Mdl_Teachers MapearDocente(SqlDataReader reader)
        {
            return new Mdl_Teachers
            {
                TeacherId = reader.GetInt32(0),
                TeacherCode = reader[1].ToString(),
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
                LocationId = reader.GetInt32(14),
                HireDate = reader[15] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(15),
                ContractType = reader[16] == DBNull.Value ? null : reader[16].ToString(),
                UserId = reader[17] == DBNull.Value ? null : (int?)reader.GetInt32(17),
                RegisteredByCoordinatorId = reader[18] == DBNull.Value ? null : (int?)reader.GetInt32(18),
                IsActive = reader.GetBoolean(19),
                CreatedDate = reader.GetDateTime(20),
                CreatedBy = reader[21] == DBNull.Value ? null : (int?)reader.GetInt32(21),
                ModifiedDate = reader[22] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(22),
                ModifiedBy = reader[23] == DBNull.Value ? null : (int?)reader.GetInt32(23)
            };
        }
    }
}