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
    internal class Ctrl_Checks
    {
        // MÉTODO: Obtener nuevo ID de cheque
        public static string ObtenerNuevoNumeroCheque()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT MAX(CAST(CheckNumber AS INT)) FROM Checks WHERE ISNUMERIC(CheckNumber) = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result == DBNull.Value || result == null)
                        {
                            return "1";
                        }
                        return (Convert.ToInt32(result) + 1).ToString();
                    }
                }
            }
            catch { return "1"; }
        }

        // MÉTODO PRINCIPAL: Registrar cheque
        public static int RegistrarCheque(Mdl_Checks cheque)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Checks (CheckNumber, IssueDate, IssuePlace, Amount, PrintedAmount, 
                BeneficiaryName, EmployeeId, BankId, BankAccountNumber, StatusId, Concept, DetailDescription, 
                Period, LocationId, DepartmentId, Exemption, TaxFreeAmount, FoodAllowance, IGSS, 
                WithholdingTax, Retention, Bonus, Discounts, Advances, Viaticos, Stamps, PurchaseOrderNumber, 
                Complement, IsActive, CreatedBy, Predeclared, Compensation, Vacation, Bill, Aguinaldo, LastComplement) 
                VALUES (@CheckNumber, @IssueDate, @IssuePlace, @Amount, @PrintedAmount, @BeneficiaryName, 
                @EmployeeId, @BankId, @BankAccountNumber, @StatusId, @Concept, @DetailDescription, @Period, 
                @LocationId, @DepartmentId, @Exemption, @TaxFreeAmount, @FoodAllowance, @IGSS, 
                @WithholdingTax, @Retention, @Bonus, @Discounts, @Advances, @Viaticos, @Stamps, 
                @PurchaseOrderNumber, @Complement, @IsActive, @CreatedBy, @Predeclared, @Compensation, 
                @Vacation, @Bill, @Aguinaldo, @LastComplement)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckNumber", cheque.CheckNumber ?? "");
                        cmd.Parameters.AddWithValue("@IssueDate", cheque.IssueDate);
                        cmd.Parameters.AddWithValue("@IssuePlace", cheque.IssuePlace ?? "GUATEMALA");
                        cmd.Parameters.AddWithValue("@Amount", cheque.Amount);
                        cmd.Parameters.AddWithValue("@PrintedAmount", cheque.PrintedAmount);
                        cmd.Parameters.AddWithValue("@BeneficiaryName", cheque.BeneficiaryName ?? "");
                        cmd.Parameters.AddWithValue("@EmployeeId", (object)cheque.EmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankId", cheque.BankId);
                        cmd.Parameters.AddWithValue("@BankAccountNumber", (object)cheque.BankAccountNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@StatusId", cheque.StatusId);
                        cmd.Parameters.AddWithValue("@Concept", cheque.Concept ?? "");
                        cmd.Parameters.AddWithValue("@DetailDescription", (object)cheque.DetailDescription ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", cheque.Period ?? "");
                        cmd.Parameters.AddWithValue("@LocationId", (object)cheque.LocationId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DepartmentId", (object)cheque.DepartmentId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Exemption", cheque.Exemption);
                        cmd.Parameters.AddWithValue("@TaxFreeAmount", cheque.TaxFreeAmount);
                        cmd.Parameters.AddWithValue("@FoodAllowance", cheque.FoodAllowance);
                        cmd.Parameters.AddWithValue("@IGSS", cheque.IGSS);
                        cmd.Parameters.AddWithValue("@WithholdingTax", cheque.WithholdingTax);
                        cmd.Parameters.AddWithValue("@Retention", cheque.Retention);
                        cmd.Parameters.AddWithValue("@Bonus", cheque.Bonus);
                        cmd.Parameters.AddWithValue("@Discounts", cheque.Discounts);
                        cmd.Parameters.AddWithValue("@Advances", cheque.Advances);
                        cmd.Parameters.AddWithValue("@Viaticos", cheque.Viaticos);
                        cmd.Parameters.AddWithValue("@Stamps", cheque.Stamps);
                        cmd.Parameters.AddWithValue("@PurchaseOrderNumber", (object)cheque.PurchaseOrderNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Complement", (object)cheque.Complement ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", cheque.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)cheque.CreatedBy ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Predeclared", cheque.Predeclared);
                        cmd.Parameters.AddWithValue("@Compensation", cheque.Compensation);
                        cmd.Parameters.AddWithValue("@Vacation", cheque.Vacation);
                        cmd.Parameters.AddWithValue("@Bill", (object)cheque.Bill ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Aguinaldo", cheque.Aguinaldo);  
                        cmd.Parameters.AddWithValue("@LastComplement", cheque.LastComplement);  

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL REGISTRAR CHEQUE: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO: Actualizar cheque
        public static int ActualizarCheque(Mdl_Checks cheque)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Checks SET CheckNumber = @CheckNumber, IssueDate = @IssueDate, 
                IssuePlace = @IssuePlace, Amount = @Amount, PrintedAmount = @PrintedAmount, 
                BeneficiaryName = @BeneficiaryName, EmployeeId = @EmployeeId, BankId = @BankId, 
                BankAccountNumber = @BankAccountNumber, StatusId = @StatusId, Concept = @Concept, 
                DetailDescription = @DetailDescription, Period = @Period, LocationId = @LocationId, 
                DepartmentId = @DepartmentId, Exemption = @Exemption, TaxFreeAmount = @TaxFreeAmount, 
                FoodAllowance = @FoodAllowance, IGSS = @IGSS, WithholdingTax = @WithholdingTax, 
                Retention = @Retention, Bonus = @Bonus, Discounts = @Discounts, Advances = @Advances, 
                Viaticos = @Viaticos, Stamps = @Stamps, PurchaseOrderNumber = @PurchaseOrderNumber, 
                Complement = @Complement, Predeclared = @Predeclared, Compensation = @Compensation,
                Vacation = @Vacation, Bill = @Bill, Aguinaldo = @Aguinaldo, LastComplement = @LastComplement,
                ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy 
                WHERE CheckId = @CheckId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckId", cheque.CheckId);
                        cmd.Parameters.AddWithValue("@CheckNumber", cheque.CheckNumber ?? "");
                        cmd.Parameters.AddWithValue("@IssueDate", cheque.IssueDate);
                        cmd.Parameters.AddWithValue("@IssuePlace", cheque.IssuePlace ?? "GUATEMALA");
                        cmd.Parameters.AddWithValue("@Amount", cheque.Amount);
                        cmd.Parameters.AddWithValue("@PrintedAmount", cheque.PrintedAmount);
                        cmd.Parameters.AddWithValue("@BeneficiaryName", cheque.BeneficiaryName ?? "");
                        cmd.Parameters.AddWithValue("@EmployeeId", (object)cheque.EmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankId", cheque.BankId);
                        cmd.Parameters.AddWithValue("@BankAccountNumber", (object)cheque.BankAccountNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@StatusId", cheque.StatusId);
                        cmd.Parameters.AddWithValue("@Concept", cheque.Concept ?? "");
                        cmd.Parameters.AddWithValue("@DetailDescription", (object)cheque.DetailDescription ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", cheque.Period ?? "");
                        cmd.Parameters.AddWithValue("@LocationId", (object)cheque.LocationId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DepartmentId", (object)cheque.DepartmentId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Exemption", cheque.Exemption);
                        cmd.Parameters.AddWithValue("@TaxFreeAmount", cheque.TaxFreeAmount);
                        cmd.Parameters.AddWithValue("@FoodAllowance", cheque.FoodAllowance);
                        cmd.Parameters.AddWithValue("@IGSS", cheque.IGSS);
                        cmd.Parameters.AddWithValue("@WithholdingTax", cheque.WithholdingTax);
                        cmd.Parameters.AddWithValue("@Retention", cheque.Retention);
                        cmd.Parameters.AddWithValue("@Bonus", cheque.Bonus);
                        cmd.Parameters.AddWithValue("@Discounts", cheque.Discounts);
                        cmd.Parameters.AddWithValue("@Advances", cheque.Advances);
                        cmd.Parameters.AddWithValue("@Viaticos", cheque.Viaticos);
                        cmd.Parameters.AddWithValue("@Stamps", cheque.Stamps);
                        cmd.Parameters.AddWithValue("@PurchaseOrderNumber", (object)cheque.PurchaseOrderNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Complement", (object)cheque.Complement ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Predeclared", cheque.Predeclared);
                        cmd.Parameters.AddWithValue("@Compensation", cheque.Compensation);
                        cmd.Parameters.AddWithValue("@Vacation", cheque.Vacation);
                        cmd.Parameters.AddWithValue("@Bill", (object)cheque.Bill ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Aguinaldo", cheque.Aguinaldo);  // NUEVO
                        cmd.Parameters.AddWithValue("@LastComplement", cheque.LastComplement);  // NUEVO
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)cheque.ModifiedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR CHEQUE: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear cheque
        private static Mdl_Checks MapearCheque(SqlDataReader reader)
        {
            return new Mdl_Checks
            {
                CheckId = reader.GetInt32(0),
                CheckNumber = reader[1].ToString(),
                IssueDate = reader.GetDateTime(2),
                IssuePlace = reader[3].ToString(),
                Amount = reader.GetDecimal(4),
                PrintedAmount = reader.GetDecimal(5),
                BeneficiaryName = reader[6].ToString(),
                EmployeeId = reader[7] == DBNull.Value ? null : (int?)reader.GetInt32(7),
                BankId = reader.GetInt32(8),
                BankAccountNumber = reader[9] == DBNull.Value ? null : reader[9].ToString(),
                StatusId = reader.GetInt32(10),
                Concept = reader[11].ToString(),
                DetailDescription = reader[12] == DBNull.Value ? null : reader[12].ToString(),
                Period = reader[13].ToString(),
                LocationId = reader[14] == DBNull.Value ? null : (int?)reader.GetInt32(14),
                DepartmentId = reader[15] == DBNull.Value ? null : (int?)reader.GetInt32(15),
                Exemption = reader.GetDecimal(16),
                TaxFreeAmount = reader.GetDecimal(17),
                FoodAllowance = reader.GetDecimal(18),
                IGSS = reader.GetDecimal(19),
                WithholdingTax = reader.GetDecimal(20),
                Retention = reader.GetDecimal(21),
                Bonus = reader.GetDecimal(22),
                Discounts = reader.GetDecimal(23),
                Advances = reader.GetDecimal(24),
                Viaticos = reader.GetDecimal(25),
                Stamps = reader.GetDecimal(26),
                PurchaseOrderNumber = reader[27] == DBNull.Value ? null : reader[27].ToString(),
                Complement = reader[28] == DBNull.Value ? null : reader[28].ToString(),
                IsActive = reader.GetBoolean(29),
                CreatedDate = reader.GetDateTime(30),
                CreatedBy = reader[31] == DBNull.Value ? null : (int?)reader.GetInt32(31),
                ModifiedDate = reader[32] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(32),
                ModifiedBy = reader[33] == DBNull.Value ? null : (int?)reader.GetInt32(33),
                AuthorizedDate = reader[34] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(34),
                AuthorizedBy = reader[35] == DBNull.Value ? null : (int?)reader.GetInt32(35),
                CashedDate = reader[36] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(36),
                Predeclared = reader.GetBoolean(37),
                Compensation = reader.GetDecimal(38),
                Vacation = reader.GetDecimal(39),
                Bill = reader[40] == DBNull.Value ? null : reader[40].ToString(),
                Aguinaldo = reader.GetDecimal(41), 
                LastComplement = reader.GetBoolean(42),
                FileControl = reader[43] == DBNull.Value ? null : reader[43].ToString()
            };
        }

        // MÉTODO: Validar si un cheque existe por número
        public static bool ValidarExistenciaCheque(string numeroCheque)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Checks WHERE CheckNumber = @CheckNumber AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckNumber", numeroCheque);
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // NUEVO MÉTODO: Validar si un cheque tiene LastComplement = 1
        public static bool ValidarLastComplement(string numeroCheque)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT LastComplement FROM Checks WHERE CheckNumber = @CheckNumber AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckNumber", numeroCheque);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToBoolean(result);
                        }
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // NUEVO MÉTODO: Marcar LastComplement como completado
        public static bool MarcarLastComplement(string numeroCheque)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE Checks SET LastComplement = 1 WHERE CheckNumber = @CheckNumber AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckNumber", numeroCheque);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // NUEVO MÉTODO: Obtener todos los cheques que tienen como complemento un cheque específico
        public static List<Mdl_Checks> ObtenerChequesComplemento(string numeroComplemento)
        {
            List<Mdl_Checks> lista = new List<Mdl_Checks>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT CheckId, CheckNumber, IssueDate, IssuePlace, Amount, PrintedAmount, 
                BeneficiaryName, EmployeeId, BankId, BankAccountNumber, StatusId, Concept, DetailDescription, 
                Period, LocationId, DepartmentId, Exemption, TaxFreeAmount, FoodAllowance, IGSS, 
                WithholdingTax, Retention, Bonus, Discounts, Advances, Viaticos, Stamps, PurchaseOrderNumber, 
                Complement, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, AuthorizedDate, 
                AuthorizedBy, CashedDate, Predeclared, Compensation, Vacation, Bill, Aguinaldo, LastComplement,
                FileControl
                FROM Checks 
                WHERE Complement = @NumeroComplemento AND IsActive = 1
                ORDER BY CheckNumber ASC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@NumeroComplemento", numeroComplemento);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCheque(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER CHEQUES COMPLEMENTO: " + ex.Message, "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO: Obtener cheque por número
        public static Mdl_Checks ObtenerChequePorNumero(string numeroCheque)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT CheckId, CheckNumber, IssueDate, IssuePlace, Amount, PrintedAmount, 
                BeneficiaryName, EmployeeId, BankId, BankAccountNumber, StatusId, Concept, DetailDescription, 
                Period, LocationId, DepartmentId, Exemption, TaxFreeAmount, FoodAllowance, IGSS, 
                WithholdingTax, Retention, Bonus, Discounts, Advances, Viaticos, Stamps, PurchaseOrderNumber, 
                Complement, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, AuthorizedDate, 
                AuthorizedBy, CashedDate, Predeclared, Compensation, Vacation, Bill, Aguinaldo, LastComplement,
                FileControl
                FROM Checks 
                WHERE CheckNumber = @CheckNumber AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckNumber", numeroCheque);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearCheque(reader);
                            }
                            return null;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        // MÉTODO: Mostrar cheques con paginación
        public static List<Mdl_Checks> MostrarCheques(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_Checks> lista = new List<Mdl_Checks>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT CheckId, CheckNumber, IssueDate, IssuePlace, Amount, PrintedAmount, 
                BeneficiaryName, EmployeeId, BankId, BankAccountNumber, StatusId, Concept, DetailDescription, 
                Period, LocationId, DepartmentId, Exemption, TaxFreeAmount, FoodAllowance, IGSS, 
                WithholdingTax, Retention, Bonus, Discounts, Advances, Viaticos, Stamps, PurchaseOrderNumber, 
                Complement, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, AuthorizedDate, 
                AuthorizedBy, CashedDate, Predeclared, Compensation, Vacation, Bill, Aguinaldo, LastComplement,
                FileControl
                FROM Checks
                ORDER BY CheckId DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCheque(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER CHEQUES: " + ex.Message, "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
        // MÉTODO: Obtener cheque por ID
        public static Mdl_Checks ObtenerChequePorId(int checkId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT CheckId, CheckNumber, IssueDate, IssuePlace, Amount, PrintedAmount, 
                BeneficiaryName, EmployeeId, BankId, BankAccountNumber, StatusId, Concept, DetailDescription, 
                Period, LocationId, DepartmentId, Exemption, TaxFreeAmount, FoodAllowance, IGSS, 
                WithholdingTax, Retention, Bonus, Discounts, Advances, Viaticos, Stamps, PurchaseOrderNumber, 
                Complement, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, AuthorizedDate, 
                AuthorizedBy, CashedDate, Predeclared, Compensation, Vacation, Bill, Aguinaldo, LastComplement,
                FileControl
                FROM Checks 
                WHERE CheckId = @CheckId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckId", checkId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearCheque(reader);
                            }
                            return null;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        // MÉTODO: Contar total de cheques
        public static int ContarTotalCheques()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Checks";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch { return 0; }
        }

        // MÉTODO: Búsqueda avanzada con múltiples filtros
        // ⭐ MÉTODO CORREGIDO: Búsqueda avanzada (AGREGADO Predeclared)
        // MÉTODO: Búsqueda avanzada con múltiples filtros
        public static List<Mdl_Checks> BuscarCheques(
            string textoBusqueda = "",
            string periodo = "",
            int? locationId = null,
            int? statusId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            string rangoInicio = null,
            string rangoFin = null,
            int pageNumber = 1,
            int pageSize = 100,
            string fileControl = null)  // ⭐⭐⭐ NUEVO PARÁMETRO OPCIONAL
        {
            List<Mdl_Checks> lista = new List<Mdl_Checks>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    StringBuilder query = new StringBuilder();
                    query.Append(@"SELECT CheckId, CheckNumber, IssueDate, IssuePlace, Amount, PrintedAmount, 
                BeneficiaryName, EmployeeId, BankId, BankAccountNumber, StatusId, Concept, DetailDescription, 
                Period, LocationId, DepartmentId, Exemption, TaxFreeAmount, FoodAllowance, IGSS, 
                WithholdingTax, Retention, Bonus, Discounts, Advances, Viaticos, Stamps, PurchaseOrderNumber, 
                Complement, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, AuthorizedDate, 
                AuthorizedBy, CashedDate, Predeclared, Compensation, Vacation, Bill, Aguinaldo, LastComplement,
                FileControl
                FROM Checks WHERE 1=1");

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query.Append(@" AND (CheckNumber LIKE @TextoBusqueda 
            OR BeneficiaryName LIKE @TextoBusqueda 
            OR Concept LIKE @TextoBusqueda)");
                    }

                    if (!string.IsNullOrWhiteSpace(periodo))
                    {
                        query.Append(" AND Period = @Periodo");
                    }

                    if (locationId.HasValue && locationId.Value > 0)
                    {
                        query.Append(" AND LocationId = @LocationId");
                    }

                    if (statusId.HasValue && statusId.Value > 0)
                    {
                        query.Append(" AND StatusId = @StatusId");
                    }

                    if (fechaInicio.HasValue)
                    {
                        query.Append(" AND IssueDate >= @FechaInicio");
                    }

                    if (fechaFin.HasValue)
                    {
                        query.Append(" AND IssueDate <= @FechaFin");
                    }

                    // ⭐⭐⭐ NUEVO FILTRO: FILECONTROL
                    if (!string.IsNullOrWhiteSpace(fileControl))
                    {
                        query.Append(" AND ISNULL(FileControl, 'PENDIENTE') = @FileControl");
                    }

                    // ⭐⭐⭐ FILTRO: RANGO DE NÚMEROS DE CHEQUE
                    if (!string.IsNullOrWhiteSpace(rangoInicio) && !string.IsNullOrWhiteSpace(rangoFin))
                    {
                        // Verificar si ambos valores son numéricos
                        bool inicioEsNumerico = long.TryParse(rangoInicio, out long numInicio);
                        bool finEsNumerico = long.TryParse(rangoFin, out long numFin);

                        if (inicioEsNumerico && finEsNumerico)
                        {
                            // Ambos son numéricos, usar comparación numérica
                            query.Append(@" AND ISNUMERIC(CheckNumber) = 1 
                    AND CAST(CheckNumber AS BIGINT) >= @RangoInicio 
                    AND CAST(CheckNumber AS BIGINT) <= @RangoFin");
                        }
                        else
                        {
                            // Al menos uno es alfanumérico, usar comparación de strings
                            query.Append(" AND CheckNumber >= @RangoInicioStr AND CheckNumber <= @RangoFinStr");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(rangoInicio))
                    {
                        // Solo rango inicio
                        bool esNumerico = long.TryParse(rangoInicio, out long num);
                        if (esNumerico)
                        {
                            query.Append(" AND ISNUMERIC(CheckNumber) = 1 AND CAST(CheckNumber AS BIGINT) >= @RangoInicio");
                        }
                        else
                        {
                            query.Append(" AND CheckNumber >= @RangoInicioStr");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(rangoFin))
                    {
                        // Solo rango fin
                        bool esNumerico = long.TryParse(rangoFin, out long num);
                        if (esNumerico)
                        {
                            query.Append(" AND ISNUMERIC(CheckNumber) = 1 AND CAST(CheckNumber AS BIGINT) <= @RangoFin");
                        }
                        else
                        {
                            query.Append(" AND CheckNumber <= @RangoFinStr");
                        }
                    }

                    query.Append(@" ORDER BY CheckId DESC 
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), connection))
                    {
                        if (!string.IsNullOrWhiteSpace(textoBusqueda))
                            cmd.Parameters.AddWithValue("@TextoBusqueda", "%" + textoBusqueda + "%");

                        if (!string.IsNullOrWhiteSpace(periodo))
                            cmd.Parameters.AddWithValue("@Periodo", periodo);

                        if (locationId.HasValue && locationId.Value > 0)
                            cmd.Parameters.AddWithValue("@LocationId", locationId.Value);

                        if (statusId.HasValue && statusId.Value > 0)
                            cmd.Parameters.AddWithValue("@StatusId", statusId.Value);

                        if (fechaInicio.HasValue)
                            cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value);

                        if (fechaFin.HasValue)
                            cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value);

                        // ⭐⭐⭐ PARÁMETRO FILECONTROL
                        if (!string.IsNullOrWhiteSpace(fileControl))
                            cmd.Parameters.AddWithValue("@FileControl", fileControl);

                        // ⭐ PARÁMETROS DEL RANGO
                        if (!string.IsNullOrWhiteSpace(rangoInicio))
                        {
                            if (long.TryParse(rangoInicio, out long numInicio))
                                cmd.Parameters.AddWithValue("@RangoInicio", numInicio);
                            else
                                cmd.Parameters.AddWithValue("@RangoInicioStr", rangoInicio);
                        }

                        if (!string.IsNullOrWhiteSpace(rangoFin))
                        {
                            if (long.TryParse(rangoFin, out long numFin))
                                cmd.Parameters.AddWithValue("@RangoFin", numFin);
                            else
                                cmd.Parameters.AddWithValue("@RangoFinStr", rangoFin);
                        }

                        cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCheque(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL BUSCAR CHEQUES: " + ex.Message, "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
        // MÉTODO: Contar cheques filtrados
        // MÉTODO: Contar cheques filtrados
        public static int ContarChequesFiltrados(
            string textoBusqueda = "",
            string periodo = "",
            int? locationId = null,
            int? statusId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            string rangoInicio = null,
            string rangoFin = null,
            string fileControl = null)  // ⭐⭐⭐ NUEVO PARÁMETRO OPCIONAL
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT COUNT(*) FROM Checks WHERE 1=1");

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query.Append(@" AND (CheckNumber LIKE @TextoBusqueda 
            OR BeneficiaryName LIKE @TextoBusqueda 
            OR Concept LIKE @TextoBusqueda)");
                    }

                    if (!string.IsNullOrWhiteSpace(periodo))
                        query.Append(" AND Period = @Periodo");

                    if (locationId.HasValue && locationId.Value > 0)
                        query.Append(" AND LocationId = @LocationId");

                    if (statusId.HasValue && statusId.Value > 0)
                        query.Append(" AND StatusId = @StatusId");

                    if (fechaInicio.HasValue)
                        query.Append(" AND IssueDate >= @FechaInicio");

                    if (fechaFin.HasValue)
                        query.Append(" AND IssueDate <= @FechaFin");

                    // ⭐⭐⭐ NUEVO FILTRO: FILECONTROL
                    if (!string.IsNullOrWhiteSpace(fileControl))
                    {
                        query.Append(" AND ISNULL(FileControl, 'PENDIENTE') = @FileControl");
                    }

                    // ⭐⭐⭐ FILTRO: RANGO DE NÚMEROS DE CHEQUE
                    if (!string.IsNullOrWhiteSpace(rangoInicio) && !string.IsNullOrWhiteSpace(rangoFin))
                    {
                        bool inicioEsNumerico = long.TryParse(rangoInicio, out long numInicio);
                        bool finEsNumerico = long.TryParse(rangoFin, out long numFin);

                        if (inicioEsNumerico && finEsNumerico)
                        {
                            query.Append(@" AND ISNUMERIC(CheckNumber) = 1 
                    AND CAST(CheckNumber AS BIGINT) >= @RangoInicio 
                    AND CAST(CheckNumber AS BIGINT) <= @RangoFin");
                        }
                        else
                        {
                            query.Append(" AND CheckNumber >= @RangoInicioStr AND CheckNumber <= @RangoFinStr");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(rangoInicio))
                    {
                        bool esNumerico = long.TryParse(rangoInicio, out long num);
                        if (esNumerico)
                        {
                            query.Append(" AND ISNUMERIC(CheckNumber) = 1 AND CAST(CheckNumber AS BIGINT) >= @RangoInicio");
                        }
                        else
                        {
                            query.Append(" AND CheckNumber >= @RangoInicioStr");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(rangoFin))
                    {
                        bool esNumerico = long.TryParse(rangoFin, out long num);
                        if (esNumerico)
                        {
                            query.Append(" AND ISNUMERIC(CheckNumber) = 1 AND CAST(CheckNumber AS BIGINT) <= @RangoFin");
                        }
                        else
                        {
                            query.Append(" AND CheckNumber <= @RangoFinStr");
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), connection))
                    {
                        if (!string.IsNullOrWhiteSpace(textoBusqueda))
                            cmd.Parameters.AddWithValue("@TextoBusqueda", "%" + textoBusqueda + "%");

                        if (!string.IsNullOrWhiteSpace(periodo))
                            cmd.Parameters.AddWithValue("@Periodo", periodo);

                        if (locationId.HasValue && locationId.Value > 0)
                            cmd.Parameters.AddWithValue("@LocationId", locationId.Value);

                        if (statusId.HasValue && statusId.Value > 0)
                            cmd.Parameters.AddWithValue("@StatusId", statusId.Value);

                        if (fechaInicio.HasValue)
                            cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value);

                        if (fechaFin.HasValue)
                            cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value);

                        // ⭐⭐⭐ PARÁMETRO FILECONTROL
                        if (!string.IsNullOrWhiteSpace(fileControl))
                            cmd.Parameters.AddWithValue("@FileControl", fileControl);

                        // ⭐ PARÁMETROS DEL RANGO
                        if (!string.IsNullOrWhiteSpace(rangoInicio))
                        {
                            if (long.TryParse(rangoInicio, out long numInicio))
                                cmd.Parameters.AddWithValue("@RangoInicio", numInicio);
                            else
                                cmd.Parameters.AddWithValue("@RangoInicioStr", rangoInicio);
                        }

                        if (!string.IsNullOrWhiteSpace(rangoFin))
                        {
                            if (long.TryParse(rangoFin, out long numFin))
                                cmd.Parameters.AddWithValue("@RangoFin", numFin);
                            else
                                cmd.Parameters.AddWithValue("@RangoFinStr", rangoFin);
                        }

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public static List<Mdl_Checks> BuscarChequesNoPredeclarados(string textoBusqueda = "", string periodo = "", int? locationId = null, int? statusId = null, DateTime? fechaInicio = null,DateTime? fechaFin = null)
        {
            List<Mdl_Checks> lista = new List<Mdl_Checks>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    StringBuilder query = new StringBuilder();
                    query.Append(@"SELECT CheckId, CheckNumber, IssueDate, IssuePlace, Amount, PrintedAmount, 
                    BeneficiaryName, EmployeeId, BankId, BankAccountNumber, StatusId, Concept, DetailDescription, 
                    Period, LocationId, DepartmentId, Exemption, TaxFreeAmount, FoodAllowance, IGSS, 
                    WithholdingTax, Retention, Bonus, Discounts, Advances, Viaticos, Stamps, PurchaseOrderNumber, 
                    Complement, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, AuthorizedDate, 
                    AuthorizedBy, CashedDate, Predeclared, Compensation, Vacation, Bill, Aguinaldo, LastComplement,
                    FileControl
                    FROM Checks 
                    WHERE Predeclared = 0");

                    // Filtro de búsqueda de texto
                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query.Append(@" AND (CheckNumber LIKE @TextoBusqueda 
                    OR BeneficiaryName LIKE @TextoBusqueda 
                    OR Concept LIKE @TextoBusqueda)");
                    }

                    // Filtro de periodo
                    if (!string.IsNullOrWhiteSpace(periodo))
                    {
                        query.Append(" AND Period = @Periodo");
                    }

                    // Filtro de location
                    if (locationId.HasValue && locationId.Value > 0)
                    {
                        query.Append(" AND LocationId = @LocationId");
                    }

                    // Filtro de estado
                    if (statusId.HasValue && statusId.Value > 0)
                    {
                        query.Append(" AND StatusId = @StatusId");
                    }

                    // Filtro de rango de fechas
                    if (fechaInicio.HasValue)
                    {
                        query.Append(" AND IssueDate >= @FechaInicio");
                    }

                    if (fechaFin.HasValue)
                    {
                        query.Append(" AND IssueDate <= @FechaFin");
                    }

                    query.Append(" ORDER BY CheckId DESC"); // ⭐ SIN PAGINACIÓN

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), connection))
                    {
                        if (!string.IsNullOrWhiteSpace(textoBusqueda))
                            cmd.Parameters.AddWithValue("@TextoBusqueda", "%" + textoBusqueda + "%");

                        if (!string.IsNullOrWhiteSpace(periodo))
                            cmd.Parameters.AddWithValue("@Periodo", periodo);

                        if (locationId.HasValue && locationId.Value > 0)
                            cmd.Parameters.AddWithValue("@LocationId", locationId.Value);

                        if (statusId.HasValue && statusId.Value > 0)
                            cmd.Parameters.AddWithValue("@StatusId", statusId.Value);

                        if (fechaInicio.HasValue)
                            cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value);

                        if (fechaFin.HasValue)
                            cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCheque(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL BUSCAR CHEQUES NO PREDECLARADOS: " + ex.Message, "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO: Contar cheques NO predeclarados
        public static int ContarChequesNoPredeclarados(string textoBusqueda = "",string periodo = "",int? locationId = null,int? statusId = null,DateTime? fechaInicio = null,DateTime? fechaFin = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT COUNT(*) FROM Checks WHERE Predeclared = 0");

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query.Append(@" AND (CheckNumber LIKE @TextoBusqueda 
                    OR BeneficiaryName LIKE @TextoBusqueda 
                    OR Concept LIKE @TextoBusqueda)");
                    }

                    if (!string.IsNullOrWhiteSpace(periodo))
                        query.Append(" AND Period = @Periodo");

                    if (locationId.HasValue && locationId.Value > 0)
                        query.Append(" AND LocationId = @LocationId");

                    if (statusId.HasValue && statusId.Value > 0)
                        query.Append(" AND StatusId = @StatusId");

                    if (fechaInicio.HasValue)
                        query.Append(" AND IssueDate >= @FechaInicio");

                    if (fechaFin.HasValue)
                        query.Append(" AND IssueDate <= @FechaFin");

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), connection))
                    {
                        if (!string.IsNullOrWhiteSpace(textoBusqueda))
                            cmd.Parameters.AddWithValue("@TextoBusqueda", "%" + textoBusqueda + "%");

                        if (!string.IsNullOrWhiteSpace(periodo))
                            cmd.Parameters.AddWithValue("@Periodo", periodo);

                        if (locationId.HasValue && locationId.Value > 0)
                            cmd.Parameters.AddWithValue("@LocationId", locationId.Value);

                        if (statusId.HasValue && statusId.Value > 0)
                            cmd.Parameters.AddWithValue("@StatusId", statusId.Value);

                        if (fechaInicio.HasValue)
                            cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value);

                        if (fechaFin.HasValue)
                            cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch { return 0; }
        }

        // MÉTODO: Predeclarar cheque individual
        public static bool PredeclararCheque(int checkId, int userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Checks 
                            SET Predeclared = 1, 
                                ModifiedDate = GETDATE(), 
                                ModifiedBy = @UserId 
                            WHERE CheckId = @CheckId AND Predeclared = 0";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckId", checkId);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        int resultado = cmd.ExecuteNonQuery();
                        return resultado > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL PREDECLARAR CHEQUE: {ex.Message}", "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // MÉTODO: Predeclarar múltiples cheques seleccionados
        public static int PredeclararSeleccionados(List<int> checkIds, int userId)
        {
            int exitosos = 0;
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    foreach (int checkId in checkIds)
                    {
                        string query = @"UPDATE Checks 
                                SET Predeclared = 1, 
                                    ModifiedDate = GETDATE(), 
                                    ModifiedBy = @UserId 
                                WHERE CheckId = @CheckId AND Predeclared = 0";

                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@CheckId", checkId);
                            cmd.Parameters.AddWithValue("@UserId", userId);

                            if (cmd.ExecuteNonQuery() > 0)
                                exitosos++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL PREDECLARAR SELECCIONADOS: {ex.Message}", "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return exitosos;
        }

        // MÉTODO: Predeclarar TODOS los cheques no predeclarados
        public static int PredeclararTodos(int userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Checks 
                            SET Predeclared = 1, 
                                ModifiedDate = GETDATE(), 
                                ModifiedBy = @UserId 
                            WHERE Predeclared = 0";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL PREDECLARAR TODOS: {ex.Message}", "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
        // MÉTODO: Actualizar estado de FileControl
        public static bool ActualizarFileControl(int checkId, string nuevoEstado, int userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nuevoEstado))
                {
                    MessageBox.Show("Estado de FileControl inválido.", "ERROR",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Normalizar a mayúsculas
                string estadoUpper = nuevoEstado.Trim().ToUpper();

                // Validar estados permitidos
                if (estadoUpper != "PENDIENTE" &&
                    estadoUpper != "TRASLADADO" &&
                    estadoUpper != "RECIBIDO" &&      // ✅ incluido
                    estadoUpper != "ARCHIVADO")
                {
                    MessageBox.Show("Estado de FileControl inválido.", "ERROR",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Checks
                             SET FileControl = @FileControl,
                                 ModifiedDate = GETDATE(),
                                 ModifiedBy   = @UserId
                             WHERE CheckId = @CheckId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@FileControl", estadoUpper);

                        // 🔥 Evitar romper FK con UserId=0
                        if (userId > 0)
                            cmd.Parameters.AddWithValue("@UserId", userId);
                        else
                            cmd.Parameters.AddWithValue("@UserId", DBNull.Value);

                        cmd.Parameters.AddWithValue("@CheckId", checkId);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ACTUALIZAR FILECONTROL: {ex.Message}",
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}