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
    internal class Ctrl_Employees
    {
        // MÉTODO AUXILIAR: Generar próximo código 
        // Obtiene el último código registrado, lo incrementa y retorna el nuevo código
        // Formato: 000001, 000002, etc. o con prefijo si existe
        public static string ObtenerProximoCodigo()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // Obtener el último código registrado
                    string query = @"SELECT TOP 1 EmployeeCode 
                                   FROM Employees 
                                   WHERE EmployeeCode IS NOT NULL 
                                   ORDER BY EmployeeId DESC";

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
        // MÉTODO PRINCIPAL: Registrar empleado
        public static int RegistrarEmpleado(Mdl_Employees empleado)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Employees
            (
                EmployeeCode,
                FirstName,
                LastName,
                IdentificationNumber,
                Email,
                InstitutionalEmail,
                Phone,
                MobilePhone,
                Address,
                BirthDate,
                HireDate,
                DepartmentId,
                PositionId,
                DirectSupervisorId,
                EmployeeStatusId,
                LocationId,
                TipoContratacion,
                EmergencyContactName,
                EmergencyContactPhone,
                EmergencyContactRelation,
                nominal_salary,
                base_salary,
                additional_bonus,
                legal_bonus,
                IGSS,
                ISR,
                net_salary,
                IGSS_MANUAL,
                IsActive,
                CreatedBy
            )
            VALUES
            (
                @EmployeeCode,
                @FirstName,
                @LastName,
                @IdentificationNumber,
                @Email,
                @InstitutionalEmail,
                @Phone,
                @MobilePhone,
                @Address,
                @BirthDate,
                @HireDate,
                @DepartmentId,
                @PositionId,
                @DirectSupervisorId,
                @EmployeeStatusId,
                @LocationId,
                @TipoContratacion,
                @EmergencyContactName,
                @EmergencyContactPhone,
                @EmergencyContactRelation,
                @NominalSalary,
                @BaseSalary,
                @AdditionalBonus,
                @LegalBonus,
                @IGSS,
                @ISR,
                @NetSalary,
                @IGSSManual,
                @IsActive,
                @CreatedBy
            )";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", empleado.EmployeeCode ?? "");
                        cmd.Parameters.AddWithValue("@FirstName", empleado.FirstName ?? "");
                        cmd.Parameters.AddWithValue("@LastName", empleado.LastName ?? "");
                        cmd.Parameters.AddWithValue("@IdentificationNumber", empleado.IdentificationNumber ?? "");
                        cmd.Parameters.AddWithValue("@Email", (object)empleado.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@InstitutionalEmail", empleado.InstitutionalEmail ?? "");
                        cmd.Parameters.AddWithValue("@Phone", (object)empleado.Phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MobilePhone", (object)empleado.MobilePhone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object)empleado.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BirthDate", (object)empleado.BirthDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HireDate", empleado.HireDate);
                        cmd.Parameters.AddWithValue("@DepartmentId", empleado.DepartmentId);
                        cmd.Parameters.AddWithValue("@PositionId", empleado.PositionId);
                        cmd.Parameters.AddWithValue("@DirectSupervisorId", (object)empleado.DirectSupervisorId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmployeeStatusId", empleado.EmployeeStatusId);
                        cmd.Parameters.AddWithValue("@LocationId", (object)empleado.LocationId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TipoContratacion", (object)empleado.TipoContratacion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmergencyContactName", (object)empleado.EmergencyContactName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmergencyContactPhone", (object)empleado.EmergencyContactPhone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmergencyContactRelation", (object)empleado.EmergencyContactRelation ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@NominalSalary", (object)empleado.NominalSalary ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BaseSalary", (object)empleado.BaseSalary ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AdditionalBonus", (object)empleado.AdditionalBonus ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LegalBonus", (object)empleado.LegalBonus ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IGSS", (object)empleado.IGSS ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ISR", (object)empleado.ISR ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NetSalary", (object)empleado.NetSalary ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IGSSManual", (object)empleado.IGSSManual ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@IsActive", empleado.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)empleado.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todos los empleados con paginación
        public static List<Mdl_Employees> MostrarEmpleados(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_Employees> lista = new List<Mdl_Employees>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // ⭐ SELECT EXPLÍCITO CON ORDEN CORRECTO
                    string query = @"SELECT EmployeeId, EmployeeCode, FirstName, LastName, FullName, 
                        IdentificationNumber, Email, InstitutionalEmail, Phone, MobilePhone, Address, 
                        BirthDate, HireDate, TerminationDate, DepartmentId, PositionId, DirectSupervisorId, 
                        EmployeeStatusId, EmergencyContactName, EmergencyContactPhone, EmergencyContactRelation, 
                        nominal_salary, base_salary, additional_bonus, legal_bonus,
                        IGSS, ISR, net_salary, IGSS_MANUAL,
                        IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, LocationId, TipoContratacion
                        FROM Employees WHERE IsActive = 1 
                        ORDER BY LastName, FirstName 
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearEmpleado(reader));
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

        public static List<Mdl_Employees> BuscarEmpleados(
    string textoBusqueda = "",
    int? departmentId = null,
    int? positionId = null,
    int? employeeStatusId = null,
    DateTime? fechaIngresoDesde = null,
    DateTime? fechaIngresoHasta = null,
    int pageNumber = 1,
    int pageSize = 100)
        {
            List<Mdl_Employees> lista = new List<Mdl_Employees>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT EmployeeId, EmployeeCode, FirstName, LastName, FullName, 
                        IdentificationNumber, Email, InstitutionalEmail, Phone, MobilePhone, Address, 
                        BirthDate, HireDate, TerminationDate, DepartmentId, PositionId, DirectSupervisorId, 
                        EmployeeStatusId, EmergencyContactName, EmergencyContactPhone, EmergencyContactRelation, 
                        nominal_salary, base_salary, additional_bonus, legal_bonus,
                        IGSS, ISR, net_salary, IGSS_MANUAL,
                        IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, LocationId, TipoContratacion
                        FROM Employees 
                        WHERE IsActive = 1";

                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (EmployeeCode LIKE @texto OR FirstName LIKE @texto OR 
                    LastName LIKE @texto OR IdentificationNumber LIKE @texto OR 
                    Email LIKE @texto OR InstitutionalEmail LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (departmentId.HasValue && departmentId > 0)
                    {
                        query += " AND DepartmentId = @departmentId";
                        parametros.Add(new SqlParameter("@departmentId", departmentId.Value));
                    }

                    if (positionId.HasValue && positionId > 0)
                    {
                        query += " AND PositionId = @positionId";
                        parametros.Add(new SqlParameter("@positionId", positionId.Value));
                    }

                    if (employeeStatusId.HasValue && employeeStatusId > 0)
                    {
                        query += " AND EmployeeStatusId = @employeeStatusId";
                        parametros.Add(new SqlParameter("@employeeStatusId", employeeStatusId.Value));
                    }

                    if (fechaIngresoDesde.HasValue)
                    {
                        query += " AND HireDate >= @fechaDesde";
                        parametros.Add(new SqlParameter("@fechaDesde", fechaIngresoDesde.Value));
                    }

                    if (fechaIngresoHasta.HasValue)
                    {
                        query += " AND HireDate <= @fechaHasta";
                        parametros.Add(new SqlParameter("@fechaHasta", fechaIngresoHasta.Value));
                    }

                    query += " ORDER BY LastName, FirstName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearEmpleado(reader));
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

        // MÉTODO PRINCIPAL: Actualizar empleado
        public static int ActualizarEmpleado(Mdl_Employees empleado)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Employees SET
                EmployeeCode = @EmployeeCode,
                FirstName = @FirstName,
                LastName = @LastName,
                IdentificationNumber = @IdentificationNumber,
                Email = @Email,
                InstitutionalEmail = @InstitutionalEmail,
                Phone = @Phone,
                MobilePhone = @MobilePhone,
                Address = @Address,
                BirthDate = @BirthDate,
                HireDate = @HireDate,
                DepartmentId = @DepartmentId,
                PositionId = @PositionId,
                DirectSupervisorId = @DirectSupervisorId,
                EmployeeStatusId = @EmployeeStatusId,
                LocationId = @LocationId,
                TipoContratacion = @TipoContratacion,
                EmergencyContactName = @EmergencyContactName,
                EmergencyContactPhone = @EmergencyContactPhone,
                EmergencyContactRelation = @EmergencyContactRelation,
                nominal_salary = @NominalSalary,
                base_salary = @BaseSalary,
                additional_bonus = @AdditionalBonus,
                legal_bonus = @LegalBonus,
                IGSS = @IGSS,
                ISR = @ISR,
                net_salary = @NetSalary,
                IGSS_MANUAL = @IGSSManual,
                ModifiedDate = GETDATE(),
                ModifiedBy = @ModifiedBy
            WHERE EmployeeId = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", empleado.EmployeeId);
                        cmd.Parameters.AddWithValue("@EmployeeCode", empleado.EmployeeCode ?? "");
                        cmd.Parameters.AddWithValue("@FirstName", empleado.FirstName ?? "");
                        cmd.Parameters.AddWithValue("@LastName", empleado.LastName ?? "");
                        cmd.Parameters.AddWithValue("@IdentificationNumber", empleado.IdentificationNumber ?? "");
                        cmd.Parameters.AddWithValue("@Email", (object)empleado.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@InstitutionalEmail", empleado.InstitutionalEmail ?? "");
                        cmd.Parameters.AddWithValue("@Phone", (object)empleado.Phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MobilePhone", (object)empleado.MobilePhone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object)empleado.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BirthDate", (object)empleado.BirthDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HireDate", empleado.HireDate);
                        cmd.Parameters.AddWithValue("@DepartmentId", empleado.DepartmentId);
                        cmd.Parameters.AddWithValue("@PositionId", empleado.PositionId);
                        cmd.Parameters.AddWithValue("@DirectSupervisorId", (object)empleado.DirectSupervisorId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmployeeStatusId", empleado.EmployeeStatusId);
                        cmd.Parameters.AddWithValue("@LocationId", (object)empleado.LocationId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TipoContratacion", (object)empleado.TipoContratacion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmergencyContactName", (object)empleado.EmergencyContactName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmergencyContactPhone", (object)empleado.EmergencyContactPhone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmergencyContactRelation", (object)empleado.EmergencyContactRelation ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@NominalSalary", (object)empleado.NominalSalary ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BaseSalary", (object)empleado.BaseSalary ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AdditionalBonus", (object)empleado.AdditionalBonus ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LegalBonus", (object)empleado.LegalBonus ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IGSS", (object)empleado.IGSS ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ISR", (object)empleado.ISR ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NetSalary", (object)empleado.NetSalary ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IGSSManual", (object)empleado.IGSSManual ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)empleado.ModifiedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar empleado
        public static int InactivarEmpleado(int employeeId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Employees SET IsActive = 0, TerminationDate = GETDATE(), 
                        ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE EmployeeId = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener empleado por ID
        public static Mdl_Employees ObtenerEmpleadoPorId(int employeeId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // ⭐ SELECT EXPLÍCITO CON ORDEN CORRECTO
                    string query = @"SELECT EmployeeId, EmployeeCode, FirstName, LastName, FullName, 
                    IdentificationNumber, Email, InstitutionalEmail, Phone, MobilePhone, Address, 
                    BirthDate, HireDate, TerminationDate, DepartmentId, PositionId, DirectSupervisorId, 
                    EmployeeStatusId, EmergencyContactName, EmergencyContactPhone, EmergencyContactRelation, 
                    nominal_salary, base_salary, additional_bonus, legal_bonus,
                    IGSS, ISR, net_salary, IGSS_MANUAL,
                    IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, LocationId, TipoContratacion
                    FROM Employees WHERE EmployeeId = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearEmpleado(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }
        private static Mdl_Employees MapearEmpleado(SqlDataReader reader)
        {
            return new Mdl_Employees
            {
                EmployeeId = reader.GetInt32(0),
                EmployeeCode = reader[1] == DBNull.Value ? null : reader[1].ToString(),
                FirstName = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                LastName = reader[3] == DBNull.Value ? null : reader[3].ToString(),
                FullName = reader[4] == DBNull.Value ? null : reader[4].ToString(),
                IdentificationNumber = reader[5] == DBNull.Value ? null : reader[5].ToString(),
                Email = reader[6] == DBNull.Value ? null : reader[6].ToString(),
                InstitutionalEmail = reader[7] == DBNull.Value ? null : reader[7].ToString(),
                Phone = reader[8] == DBNull.Value ? null : reader[8].ToString(),
                MobilePhone = reader[9] == DBNull.Value ? null : reader[9].ToString(),
                Address = reader[10] == DBNull.Value ? null : reader[10].ToString(),
                BirthDate = reader[11] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(11),
                HireDate = reader.GetDateTime(12),
                TerminationDate = reader[13] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(13),
                DepartmentId = reader.GetInt32(14),
                PositionId = reader.GetInt32(15),
                DirectSupervisorId = reader[16] == DBNull.Value ? null : (int?)reader.GetInt32(16),
                EmployeeStatusId = reader.GetInt32(17),
                EmergencyContactName = reader[18] == DBNull.Value ? null : reader[18].ToString(),
                EmergencyContactPhone = reader[19] == DBNull.Value ? null : reader[19].ToString(),
                EmergencyContactRelation = reader[20] == DBNull.Value ? null : reader[20].ToString(),

                NominalSalary = reader[21] == DBNull.Value ? null : (decimal?)reader.GetDecimal(21),
                BaseSalary = reader[22] == DBNull.Value ? null : (decimal?)reader.GetDecimal(22),
                AdditionalBonus = reader[23] == DBNull.Value ? null : (decimal?)reader.GetDecimal(23),
                LegalBonus = reader[24] == DBNull.Value ? null : (decimal?)reader.GetDecimal(24),
                IGSS = reader[25] == DBNull.Value ? null : (decimal?)reader.GetDecimal(25),
                ISR = reader[26] == DBNull.Value ? null : (decimal?)reader.GetDecimal(26),
                NetSalary = reader[27] == DBNull.Value ? null : (decimal?)reader.GetDecimal(27),
                IGSSManual = reader[28] == DBNull.Value ? null : (bool?)reader.GetBoolean(28),

                IsActive = reader.GetBoolean(29),
                CreatedDate = reader[30] == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(30),
                CreatedBy = reader[31] == DBNull.Value ? null : (int?)reader.GetInt32(31),
                ModifiedDate = reader[32] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(32),
                ModifiedBy = reader[33] == DBNull.Value ? null : (int?)reader.GetInt32(33),
                LocationId = reader[34] == DBNull.Value ? null : (int?)reader.GetInt32(34),
                TipoContratacion = reader[35] == DBNull.Value ? null : reader[35].ToString()
            };
        }

        // MÉTODOS DE VALIDACIÓN
        public static bool ValidarDPIUnico(string dpi, int? excludeEmployeeId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Employees WHERE IdentificationNumber = @DPI";
                    if (excludeEmployeeId.HasValue)
                        query += " AND EmployeeId != @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DPI", dpi ?? "");
                        if (excludeEmployeeId.HasValue)
                            cmd.Parameters.AddWithValue("@EmployeeId", excludeEmployeeId.Value);

                        return (int)cmd.ExecuteScalar() == 0;
                    }
                }
            }
            catch { return false; }
        }

        public static bool ValidarCodigoEmpleadoUnico(string codigo, int? excludeEmployeeId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Employees WHERE EmployeeCode = @Codigo";
                    if (excludeEmployeeId.HasValue)
                        query += " AND EmployeeId != @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo ?? "");
                        if (excludeEmployeeId.HasValue)
                            cmd.Parameters.AddWithValue("@EmployeeId", excludeEmployeeId.Value);

                        return (int)cmd.ExecuteScalar() == 0;
                    }
                }
            }
            catch { return false; }
        }

        public static bool ValidarEmailInstitucionalUnico(string email, int? excludeEmployeeId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Employees WHERE InstitutionalEmail = @Email";
                    if (excludeEmployeeId.HasValue)
                        query += " AND EmployeeId != @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email ?? "");
                        if (excludeEmployeeId.HasValue)
                            cmd.Parameters.AddWithValue("@EmployeeId", excludeEmployeeId.Value);

                        return (int)cmd.ExecuteScalar() == 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODOS PARA OBTENER DATOS DE FILTROS
        public static List<KeyValuePair<int, string>> ObtenerDepartamentos()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT DepartmentId, DepartmentName FROM Departments ORDER BY DepartmentName";
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

        public static List<KeyValuePair<int, string>> ObtenerPuestos()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT PositionId, PositionName FROM Positions ORDER BY PositionName";
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
                MessageBox.Show("Error al obtener puestos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static List<KeyValuePair<int, string>> ObtenerEstadosEmpleado()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT EmployeeStatusId, StatusName FROM EmployeeStatus ORDER BY StatusName";
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

        public static List<KeyValuePair<int, string>> ObtenerSupervisores()
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
                MessageBox.Show("Error al obtener supervisores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA CONTAR TOTAL DE REGISTROS (PARA PAGINACIÓN)
        public static int ContarTotalEmpleados(
            string textoBusqueda = "",
            int? departmentId = null,
            int? positionId = null,
            int? employeeStatusId = null,
            DateTime? fechaIngresoDesde = null,
            DateTime? fechaIngresoHasta = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Employees WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (EmployeeCode LIKE @texto OR FirstName LIKE @texto OR 
                            LastName LIKE @texto OR IdentificationNumber LIKE @texto OR 
                            Email LIKE @texto OR InstitutionalEmail LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (departmentId.HasValue && departmentId > 0)
                    {
                        query += " AND DepartmentId = @departmentId";
                        parametros.Add(new SqlParameter("@departmentId", departmentId.Value));
                    }

                    if (positionId.HasValue && positionId > 0)
                    {
                        query += " AND PositionId = @positionId";
                        parametros.Add(new SqlParameter("@positionId", positionId.Value));
                    }

                    if (employeeStatusId.HasValue && employeeStatusId > 0)
                    {
                        query += " AND EmployeeStatusId = @employeeStatusId";
                        parametros.Add(new SqlParameter("@employeeStatusId", employeeStatusId.Value));
                    }

                    if (fechaIngresoDesde.HasValue)
                    {
                        query += " AND HireDate >= @fechaDesde";
                        parametros.Add(new SqlParameter("@fechaDesde", fechaIngresoDesde.Value));
                    }

                    if (fechaIngresoHasta.HasValue)
                    {
                        query += " AND HireDate <= @fechaHasta";
                        parametros.Add(new SqlParameter("@fechaHasta", fechaIngresoHasta.Value));
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