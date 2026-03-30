using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_Transfers
    {
        #region MAPEO

        private static Mdl_Transfers MapearTransfer(SqlDataReader reader)
        {
            return new Mdl_Transfers
            {
                TransferId = reader.GetInt32(0),
                TransferNumber = reader[1].ToString(),
                IssueDate = reader.GetDateTime(2),
                IssuePlace = reader[3].ToString(),
                Amount = reader.GetDecimal(4),
                PrintedAmount = reader.GetDecimal(5),
                BeneficiaryName = reader[6].ToString(),
                EmployeeId = reader[7] == DBNull.Value ? null : (int?)reader.GetInt32(7),
                BankId = reader.GetInt32(8),
                BankAccountNumber = reader[9] == DBNull.Value ? null : reader[9].ToString(),
                BanksAccountTypeId = reader.GetInt32(10),
                StatusId = reader.GetInt32(11),
                Concept = reader[12].ToString(),
                DetailDescription = reader[13] == DBNull.Value ? null : reader[13].ToString(),
                Period = reader[14] == DBNull.Value ? null : reader[14].ToString(),
                LocationId = reader[15] == DBNull.Value ? null : (int?)reader.GetInt32(15),
                DepartmentId = reader[16] == DBNull.Value ? null : (int?)reader.GetInt32(16),
                Exemption = reader.GetDecimal(17),
                TaxFreeAmount = reader.GetDecimal(18),
                FoodAllowance = reader.GetDecimal(19),
                IGSS = reader.GetDecimal(20),
                WithholdingTax = reader.GetDecimal(21),
                Retention = reader.GetDecimal(22),
                Bonus = reader.GetDecimal(23),
                Discounts = reader.GetDecimal(24),
                Advances = reader.GetDecimal(25),
                Viaticos = reader.GetDecimal(26),
                Stamps = reader.GetDecimal(27),
                PurchaseOrderNumber = reader[28] == DBNull.Value ? null : reader[28].ToString(),
                Complement = reader[29] == DBNull.Value ? null : reader[29].ToString(),
                IsActive = reader.GetBoolean(30),
                CreatedDate = reader[31] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(31),
                CreatedBy = reader[32] == DBNull.Value ? null : (int?)reader.GetInt32(32),
                ModifiedDate = reader[33] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(33),
                ModifiedBy = reader[34] == DBNull.Value ? null : (int?)reader.GetInt32(34),
                AuthorizedDate = reader[35] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(35),
                AuthorizedBy = reader[36] == DBNull.Value ? null : (int?)reader.GetInt32(36),
                CashedDate = reader[37] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(37),
                Compensation = reader.GetDecimal(38),
                Vacation = reader.GetDecimal(39),
                Bill = reader[40] == DBNull.Value ? null : reader[40].ToString(),
                Aguinaldo = reader.GetDecimal(41),
                LastComplement = reader.GetBoolean(42),
                FileControl = reader[43] == DBNull.Value ? null : reader[43].ToString()
            };
        }

        #endregion
        #region CRUD BÁSICO

        // Registrar nueva transferencia
        public static int RegistrarTransfer(Mdl_Transfers transfer)
        {
            try
            {
                // Si no viene StatusId, poner COMPLETADA
                if (transfer.StatusId <= 0)
                {
                    transfer.StatusId = Ctrl_TransferStatus.ObtenerStatusCompletadaId();
                }

                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Transfers (
                                        TransferNumber, IssueDate, IssuePlace,
                                        Amount, PrintedAmount,
                                        BeneficiaryName, EmployeeId,
                                        BankId, BankAccountNumber, BanksAccountTypeId,
                                        StatusId, Concept, DetailDescription,
                                        Period, LocationId, DepartmentId,
                                        Exemption, TaxFreeAmount, FoodAllowance, IGSS,
                                        WithholdingTax, Retention, Bonus, Discounts, Advances,
                                        Viaticos, Stamps, PurchaseOrderNumber, Complement,
                                        IsActive, CreatedBy, Compensation, Vacation,
                                        Bill, Aguinaldo, LastComplement, FileControl,
                                        CreatedDate
                                     )
                                     VALUES (
                                        @TransferNumber, @IssueDate, @IssuePlace,
                                        @Amount, @PrintedAmount,
                                        @BeneficiaryName, @EmployeeId,
                                        @BankId, @BankAccountNumber, @BanksAccountTypeId,
                                        @StatusId, @Concept, @DetailDescription,
                                        @Period, @LocationId, @DepartmentId,
                                        @Exemption, @TaxFreeAmount, @FoodAllowance, @IGSS,
                                        @WithholdingTax, @Retention, @Bonus, @Discounts, @Advances,
                                        @Viaticos, @Stamps, @PurchaseOrderNumber, @Complement,
                                        @IsActive, @CreatedBy, @Compensation, @Vacation,
                                        @Bill, @Aguinaldo, @LastComplement, @FileControl,
                                        GETDATE()
                                     );
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransferNumber", transfer.TransferNumber ?? "");
                        cmd.Parameters.AddWithValue("@IssueDate", transfer.IssueDate);
                        cmd.Parameters.AddWithValue("@IssuePlace", transfer.IssuePlace ?? "GUATEMALA");
                        cmd.Parameters.AddWithValue("@Amount", transfer.Amount);
                        cmd.Parameters.AddWithValue("@PrintedAmount", transfer.PrintedAmount);
                        cmd.Parameters.AddWithValue("@BeneficiaryName", transfer.BeneficiaryName ?? "");
                        cmd.Parameters.AddWithValue("@EmployeeId", (object)transfer.EmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankId", transfer.BankId);
                        cmd.Parameters.AddWithValue("@BankAccountNumber", (object)transfer.BankAccountNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BanksAccountTypeId", transfer.BanksAccountTypeId);
                        cmd.Parameters.AddWithValue("@StatusId", transfer.StatusId);
                        cmd.Parameters.AddWithValue("@Concept", transfer.Concept ?? "");
                        cmd.Parameters.AddWithValue("@DetailDescription", (object)transfer.DetailDescription ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", transfer.Period ?? "");
                        cmd.Parameters.AddWithValue("@LocationId", (object)transfer.LocationId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DepartmentId", (object)transfer.DepartmentId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Exemption", transfer.Exemption);
                        cmd.Parameters.AddWithValue("@TaxFreeAmount", transfer.TaxFreeAmount);
                        cmd.Parameters.AddWithValue("@FoodAllowance", transfer.FoodAllowance);
                        cmd.Parameters.AddWithValue("@IGSS", transfer.IGSS);
                        cmd.Parameters.AddWithValue("@WithholdingTax", transfer.WithholdingTax);
                        cmd.Parameters.AddWithValue("@Retention", transfer.Retention);
                        cmd.Parameters.AddWithValue("@Bonus", transfer.Bonus);
                        cmd.Parameters.AddWithValue("@Discounts", transfer.Discounts);
                        cmd.Parameters.AddWithValue("@Advances", transfer.Advances);
                        cmd.Parameters.AddWithValue("@Viaticos", transfer.Viaticos);
                        cmd.Parameters.AddWithValue("@Stamps", transfer.Stamps);
                        cmd.Parameters.AddWithValue("@PurchaseOrderNumber", (object)transfer.PurchaseOrderNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Complement", (object)transfer.Complement ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", transfer.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)transfer.CreatedBy ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Compensation", transfer.Compensation);
                        cmd.Parameters.AddWithValue("@Vacation", transfer.Vacation);
                        cmd.Parameters.AddWithValue("@Bill", (object)transfer.Bill ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Aguinaldo", transfer.Aguinaldo);
                        cmd.Parameters.AddWithValue("@LastComplement", transfer.LastComplement);
                        cmd.Parameters.AddWithValue("@FileControl", (object)transfer.FileControl ?? "PENDIENTE");

                        object result = cmd.ExecuteScalar();
                        int newId = result == null || result == DBNull.Value
                            ? 0
                            : Convert.ToInt32(result);

                        // Auditoría simple
                        if (newId > 0 && transfer.CreatedBy.HasValue && transfer.CreatedBy.Value > 0)
                        {
                            Ctrl_Audit.RegistrarAccion(
                                transfer.CreatedBy.Value,
                                "CREAR",
                                "Transfers",
                                newId,
                                $"Se creó la transferencia {transfer.TransferNumber}"
                            );
                        }

                        return newId;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar transferencia: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Actualizar transferencia
        public static bool ActualizarTransfer(Mdl_Transfers transfer)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Transfers SET
                                        TransferNumber = @TransferNumber,
                                        IssueDate = @IssueDate,
                                        IssuePlace = @IssuePlace,
                                        Amount = @Amount,
                                        PrintedAmount = @PrintedAmount,
                                        BeneficiaryName = @BeneficiaryName,
                                        EmployeeId = @EmployeeId,
                                        BankId = @BankId,
                                        BankAccountNumber = @BankAccountNumber,
                                        BanksAccountTypeId = @BanksAccountTypeId,
                                        StatusId = @StatusId,
                                        Concept = @Concept,
                                        DetailDescription = @DetailDescription,
                                        Period = @Period,
                                        LocationId = @LocationId,
                                        DepartmentId = @DepartmentId,
                                        Exemption = @Exemption,
                                        TaxFreeAmount = @TaxFreeAmount,
                                        FoodAllowance = @FoodAllowance,
                                        IGSS = @IGSS,
                                        WithholdingTax = @WithholdingTax,
                                        Retention = @Retention,
                                        Bonus = @Bonus,
                                        Discounts = @Discounts,
                                        Advances = @Advances,
                                        Viaticos = @Viaticos,
                                        Stamps = @Stamps,
                                        PurchaseOrderNumber = @PurchaseOrderNumber,
                                        Complement = @Complement,
                                        Compensation = @Compensation,
                                        Vacation = @Vacation,
                                        Bill = @Bill,
                                        Aguinaldo = @Aguinaldo,
                                        LastComplement = @LastComplement,
                                        FileControl = @FileControl,
                                        ModifiedDate = GETDATE(),
                                        ModifiedBy = @ModifiedBy
                                     WHERE TransferId = @TransferId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransferId", transfer.TransferId);
                        cmd.Parameters.AddWithValue("@TransferNumber", transfer.TransferNumber ?? "");
                        cmd.Parameters.AddWithValue("@IssueDate", transfer.IssueDate);
                        cmd.Parameters.AddWithValue("@IssuePlace", transfer.IssuePlace ?? "GUATEMALA");
                        cmd.Parameters.AddWithValue("@Amount", transfer.Amount);
                        cmd.Parameters.AddWithValue("@PrintedAmount", transfer.PrintedAmount);
                        cmd.Parameters.AddWithValue("@BeneficiaryName", transfer.BeneficiaryName ?? "");
                        cmd.Parameters.AddWithValue("@EmployeeId", (object)transfer.EmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankId", transfer.BankId);
                        cmd.Parameters.AddWithValue("@BankAccountNumber", (object)transfer.BankAccountNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BanksAccountTypeId", transfer.BanksAccountTypeId);
                        cmd.Parameters.AddWithValue("@StatusId", transfer.StatusId);
                        cmd.Parameters.AddWithValue("@Concept", transfer.Concept ?? "");
                        cmd.Parameters.AddWithValue("@DetailDescription", (object)transfer.DetailDescription ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", transfer.Period ?? "");
                        cmd.Parameters.AddWithValue("@LocationId", (object)transfer.LocationId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DepartmentId", (object)transfer.DepartmentId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Exemption", transfer.Exemption);
                        cmd.Parameters.AddWithValue("@TaxFreeAmount", transfer.TaxFreeAmount);
                        cmd.Parameters.AddWithValue("@FoodAllowance", transfer.FoodAllowance);
                        cmd.Parameters.AddWithValue("@IGSS", transfer.IGSS);
                        cmd.Parameters.AddWithValue("@WithholdingTax", transfer.WithholdingTax);
                        cmd.Parameters.AddWithValue("@Retention", transfer.Retention);
                        cmd.Parameters.AddWithValue("@Bonus", transfer.Bonus);
                        cmd.Parameters.AddWithValue("@Discounts", transfer.Discounts);
                        cmd.Parameters.AddWithValue("@Advances", transfer.Advances);
                        cmd.Parameters.AddWithValue("@Viaticos", transfer.Viaticos);
                        cmd.Parameters.AddWithValue("@Stamps", transfer.Stamps);
                        cmd.Parameters.AddWithValue("@PurchaseOrderNumber", (object)transfer.PurchaseOrderNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Complement", (object)transfer.Complement ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Compensation", transfer.Compensation);
                        cmd.Parameters.AddWithValue("@Vacation", transfer.Vacation);
                        cmd.Parameters.AddWithValue("@Bill", (object)transfer.Bill ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Aguinaldo", transfer.Aguinaldo);
                        cmd.Parameters.AddWithValue("@LastComplement", transfer.LastComplement);
                        cmd.Parameters.AddWithValue("@FileControl", (object)transfer.FileControl ?? "PENDIENTE");
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)transfer.ModifiedBy ?? DBNull.Value);

                        bool ok = cmd.ExecuteNonQuery() > 0;

                        if (ok && transfer.ModifiedBy.HasValue && transfer.ModifiedBy.Value > 0)
                        {
                            Ctrl_Audit.RegistrarAccion(
                                transfer.ModifiedBy.Value,
                                "ACTUALIZAR",
                                "Transfers",
                                transfer.TransferId,
                                $"Se actualizó la transferencia {transfer.TransferNumber}"
                            );
                        }

                        return ok;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar transferencia: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool ValidarExistenciaTransfer(string numeroTransfer)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT COUNT(1)
                             FROM Transfers
                             WHERE TransferNumber = @TransferNumber
                               AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransferNumber", numeroTransfer);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar existencia de la transferencia: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion
        #region FILECONTROL

        public static bool ActualizarFileControl(int transferId, string nuevoEstado, int userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nuevoEstado))
                {
                    MessageBox.Show("Estado de FileControl inválido.", "ERROR",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string estadoUpper = nuevoEstado.Trim().ToUpper();

                if (estadoUpper != "PENDIENTE" &&
                    estadoUpper != "TRASLADADO" &&
                    estadoUpper != "RECIBIDO" &&
                    estadoUpper != "ARCHIVADO")
                {
                    MessageBox.Show("Estado de FileControl inválido.", "ERROR",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Transfers
                                     SET FileControl = @FileControl,
                                         ModifiedDate = GETDATE(),
                                         ModifiedBy   = @UserId
                                     WHERE TransferId = @TransferId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@FileControl", estadoUpper);

                        if (userId > 0)
                            cmd.Parameters.AddWithValue("@UserId", userId);
                        else
                            cmd.Parameters.AddWithValue("@UserId", DBNull.Value);

                        cmd.Parameters.AddWithValue("@TransferId", transferId);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ACTUALIZAR FILECONTROL (TRANSFER): {ex.Message}",
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion
        #region COMPLEMENTOS

        public static bool ValidarLastComplement(string numeroTransfer)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT LastComplement FROM Transfers WHERE TransferNumber = @TransferNumber AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransferNumber", numeroTransfer);
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

        public static bool MarcarLastComplement(string numeroTransfer)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE Transfers SET LastComplement = 1 WHERE TransferNumber = @TransferNumber AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransferNumber", numeroTransfer);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static List<Mdl_Transfers> ObtenerTransfersComplemento(string numeroComplemento)
        {
            var lista = new List<Mdl_Transfers>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT 
                                        TransferId, TransferNumber, IssueDate, IssuePlace, Amount, PrintedAmount,
                                        BeneficiaryName, EmployeeId, BankId, BankAccountNumber, BanksAccountTypeId,
                                        StatusId, Concept, DetailDescription,
                                        Period, LocationId, DepartmentId, Exemption, TaxFreeAmount,
                                        FoodAllowance, IGSS, WithholdingTax, Retention, Bonus,
                                        Discounts, Advances, Viaticos, Stamps, PurchaseOrderNumber,
                                        Complement, IsActive, CreatedDate, CreatedBy, ModifiedDate,
                                        ModifiedBy, AuthorizedDate, AuthorizedBy, CashedDate,
                                        Compensation, Vacation, Bill, Aguinaldo, LastComplement,
                                        FileControl
                                    FROM Transfers
                                    WHERE Complement = @NumeroComplemento AND IsActive = 1
                                    ORDER BY TransferNumber ASC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@NumeroComplemento", numeroComplemento);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearTransfer(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener transfers por complemento: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static Mdl_Transfers ObtenerTransferPorNumero(string numeroTransfer)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT 
                                        TransferId, TransferNumber, IssueDate, IssuePlace, Amount, PrintedAmount,
                                        BeneficiaryName, EmployeeId, BankId, BankAccountNumber, BanksAccountTypeId,
                                        StatusId, Concept, DetailDescription,
                                        Period, LocationId, DepartmentId, Exemption, TaxFreeAmount,
                                        FoodAllowance, IGSS, WithholdingTax, Retention, Bonus,
                                        Discounts, Advances, Viaticos, Stamps, PurchaseOrderNumber,
                                        Complement, IsActive, CreatedDate, CreatedBy, ModifiedDate,
                                        ModifiedBy, AuthorizedDate, AuthorizedBy, CashedDate,
                                        Compensation, Vacation, Bill, Aguinaldo, LastComplement,
                                        FileControl
                                    FROM Transfers
                                    WHERE TransferNumber = @TransferNumber";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransferNumber", numeroTransfer);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearTransfer(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener transferencia por número: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        #endregion
        #region BÚSQUEDA + PAGINACIÓN

        public static List<Mdl_Transfers> BuscarTransfers(
            string textoBusqueda = "",
            string periodo = "",
            int? locationId = null,
            int? statusId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            string rangoInicio = null,
            string rangoFin = null,
            int pageNumber = 1,
            int pageSize = 100)
        {
            var lista = new List<Mdl_Transfers>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    StringBuilder query = new StringBuilder();
                    query.Append(@"SELECT 
                            TransferId, TransferNumber, IssueDate, IssuePlace, Amount, PrintedAmount,
                            BeneficiaryName, EmployeeId, BankId, BankAccountNumber, BanksAccountTypeId,
                            StatusId, Concept, DetailDescription,
                            Period, LocationId, DepartmentId, Exemption, TaxFreeAmount,
                            FoodAllowance, IGSS, WithholdingTax, Retention, Bonus,
                            Discounts, Advances, Viaticos, Stamps, PurchaseOrderNumber,
                            Complement, IsActive, CreatedDate, CreatedBy, ModifiedDate,
                            ModifiedBy, AuthorizedDate, AuthorizedBy, CashedDate,
                            Compensation, Vacation, Bill, Aguinaldo, LastComplement,
                            FileControl
                        FROM Transfers
                        WHERE 1=1");

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query.Append(@" AND (
                            TransferNumber LIKE @TextoBusqueda
                            OR BeneficiaryName LIKE @TextoBusqueda
                            OR Concept LIKE @TextoBusqueda
                        )");
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

                    // Rango de número de transferencia (igual lógica que cheques)
                    if (!string.IsNullOrWhiteSpace(rangoInicio) && !string.IsNullOrWhiteSpace(rangoFin))
                    {
                        bool inicioEsNumerico = long.TryParse(rangoInicio, out long numInicio);
                        bool finEsNumerico = long.TryParse(rangoFin, out long numFin);

                        if (inicioEsNumerico && finEsNumerico)
                        {
                            query.Append(@" AND ISNUMERIC(TransferNumber) = 1 
                                AND CAST(TransferNumber AS BIGINT) >= @RangoInicio 
                                AND CAST(TransferNumber AS BIGINT) <= @RangoFin");
                        }
                        else
                        {
                            query.Append(" AND TransferNumber >= @RangoInicioStr AND TransferNumber <= @RangoFinStr");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(rangoInicio))
                    {
                        bool esNumerico = long.TryParse(rangoInicio, out long num);
                        if (esNumerico)
                            query.Append(" AND ISNUMERIC(TransferNumber) = 1 AND CAST(TransferNumber AS BIGINT) >= @RangoInicio");
                        else
                            query.Append(" AND TransferNumber >= @RangoInicioStr");
                    }
                    else if (!string.IsNullOrWhiteSpace(rangoFin))
                    {
                        bool esNumerico = long.TryParse(rangoFin, out long num);
                        if (esNumerico)
                            query.Append(" AND ISNUMERIC(TransferNumber) = 1 AND CAST(TransferNumber AS BIGINT) <= @RangoFin");
                        else
                            query.Append(" AND TransferNumber <= @RangoFinStr");
                    }

                    query.Append(@" ORDER BY TransferId DESC
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
                                lista.Add(MapearTransfer(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar transferencias: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static int ContarTransfersFiltrados(
            string textoBusqueda = "",
            string periodo = "",
            int? locationId = null,
            int? statusId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            string rangoInicio = null,
            string rangoFin = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT COUNT(*) FROM Transfers WHERE 1=1");

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query.Append(@" AND (
                            TransferNumber LIKE @TextoBusqueda
                            OR BeneficiaryName LIKE @TextoBusqueda
                            OR Concept LIKE @TextoBusqueda
                        )");
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

                    if (!string.IsNullOrWhiteSpace(rangoInicio) && !string.IsNullOrWhiteSpace(rangoFin))
                    {
                        bool inicioEsNumerico = long.TryParse(rangoInicio, out long numInicio);
                        bool finEsNumerico = long.TryParse(rangoFin, out long numFin);

                        if (inicioEsNumerico && finEsNumerico)
                        {
                            query.Append(@" AND ISNUMERIC(TransferNumber) = 1
                                AND CAST(TransferNumber AS BIGINT) >= @RangoInicio
                                AND CAST(TransferNumber AS BIGINT) <= @RangoFin");
                        }
                        else
                        {
                            query.Append(" AND TransferNumber >= @RangoInicioStr AND TransferNumber <= @RangoFinStr");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(rangoInicio))
                    {
                        bool esNumerico = long.TryParse(rangoInicio, out long num);
                        if (esNumerico)
                            query.Append(" AND ISNUMERIC(TransferNumber) = 1 AND CAST(TransferNumber AS BIGINT) >= @RangoInicio");
                        else
                            query.Append(" AND TransferNumber >= @RangoInicioStr");
                    }
                    else if (!string.IsNullOrWhiteSpace(rangoFin))
                    {
                        bool esNumerico = long.TryParse(rangoFin, out long num);
                        if (esNumerico)
                            query.Append(" AND ISNUMERIC(TransferNumber) = 1 AND CAST(TransferNumber AS BIGINT) <= @RangoFin");
                        else
                            query.Append(" AND TransferNumber <= @RangoFinStr");
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

                        object result = cmd.ExecuteScalar();
                        return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al contar transferencias: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion
        
    }
}
