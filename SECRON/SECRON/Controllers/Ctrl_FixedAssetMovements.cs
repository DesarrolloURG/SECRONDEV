using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_FixedAssetMovements
    {
        // ─────────────────────────────────────────────
        // READ - con filtros opcionales
        // ─────────────────────────────────────────────
        public static List<Mdl_FixedAssetTransfer> MostrarMovimientos(
            string transferCode = null,
            string assetCode = null,
            int? transferStatusId = null,
            int? fromWarehouseId = null,
            int? toWarehouseId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            List<Mdl_FixedAssetTransfer> lista = new List<Mdl_FixedAssetTransfer>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetMovements_Select", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TransferCode", (object)transferCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AssetCode", (object)assetCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TransferStatusId", transferStatusId.HasValue ? (object)transferStatusId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@FromWarehouseId", fromWarehouseId.HasValue ? (object)fromWarehouseId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToWarehouseId", toWarehouseId.HasValue ? (object)toWarehouseId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.HasValue ? (object)fechaInicio.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin.HasValue ? (object)fechaFin.Value : DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearMovimiento(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener movimientos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // ─────────────────────────────────────────────
        // CREATE
        // ─────────────────────────────────────────────
        public static int RegistrarMovimiento(Mdl_FixedAssetTransfer transfer)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetMovements_Insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TransferCode", transfer.TransferCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@AssetId", transfer.AssetId);
                        cmd.Parameters.AddWithValue("@TransferDate", transfer.TransferDate);
                        cmd.Parameters.AddWithValue("@FromWarehouseId", (object)transfer.FromWarehouseId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FromEmployeeId", (object)transfer.FromEmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToWarehouseId", (object)transfer.ToWarehouseId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToEmployeeId", (object)transfer.ToEmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TransferStatusId", transfer.TransferStatusId);
                        cmd.Parameters.AddWithValue("@Reason", (object)transfer.Reason?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)transfer.CreatedBy ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar movimiento: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // ─────────────────────────────────────────────
        // UPDATE
        // ─────────────────────────────────────────────
        public static int ActualizarMovimiento(Mdl_FixedAssetTransfer transfer)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetMovements_Update", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TransferId", transfer.TransferId);
                        cmd.Parameters.AddWithValue("@TransferCode", transfer.TransferCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@AssetId", transfer.AssetId);
                        cmd.Parameters.AddWithValue("@TransferDate", transfer.TransferDate);
                        cmd.Parameters.AddWithValue("@FromWarehouseId", (object)transfer.FromWarehouseId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FromEmployeeId", (object)transfer.FromEmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToWarehouseId", (object)transfer.ToWarehouseId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToEmployeeId", (object)transfer.ToEmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TransferStatusId", transfer.TransferStatusId);
                        cmd.Parameters.AddWithValue("@Reason", (object)transfer.Reason?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ApprovedByUserId", (object)transfer.ApprovedByUserId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ApprovedDate", (object)transfer.ApprovedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CompletedDate", (object)transfer.CompletedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)transfer.ModifiedBy ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar movimiento: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // ─────────────────────────────────────────────
        // INACTIVE (cancela — mueve a REJECTED)
        // ─────────────────────────────────────────────
        public static int InactivarMovimiento(int transferId, int? modifiedBy = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetMovements_Inactive", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TransferId", transferId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cancelar movimiento: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // ─────────────────────────────────────────────
        // MAPPER
        // ─────────────────────────────────────────────
        private static Mdl_FixedAssetTransfer MapearMovimiento(SqlDataReader reader)
        {
            return new Mdl_FixedAssetTransfer
            {
                TransferId = reader.GetInt32(reader.GetOrdinal("TransferId")),
                TransferCode = reader["TransferCode"].ToString(),
                AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")),
                AssetCode = reader["AssetCode"].ToString(),
                AssetName = reader["AssetName"].ToString(),
                TransferStatusId = reader.GetInt32(reader.GetOrdinal("TransferStatusId")),
                StatusCode = reader["StatusCode"].ToString(),
                StatusName = reader["StatusName"].ToString(),
                TransferDate = reader.GetDateTime(reader.GetOrdinal("TransferDate")),
                FromWarehouseId = reader["FromWarehouseId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("FromWarehouseId")),
                FromWarehouseName = reader["FromWarehouseName"] == DBNull.Value ? null : reader["FromWarehouseName"].ToString(),
                FromEmployeeId = reader["FromEmployeeId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("FromEmployeeId")),
                FromEmployeeName = reader["FromEmployeeName"] == DBNull.Value ? null : reader["FromEmployeeName"].ToString(),
                ToWarehouseId = reader["ToWarehouseId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ToWarehouseId")),
                ToWarehouseName = reader["ToWarehouseName"] == DBNull.Value ? null : reader["ToWarehouseName"].ToString(),
                ToEmployeeId = reader["ToEmployeeId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ToEmployeeId")),
                ToEmployeeName = reader["ToEmployeeName"] == DBNull.Value ? null : reader["ToEmployeeName"].ToString(),
                Reason = reader["Reason"] == DBNull.Value ? null : reader["Reason"].ToString(),
                ApprovedByUserId = reader["ApprovedByUserId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ApprovedByUserId")),
                ApprovedDate = reader["ApprovedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ApprovedDate")),
                CompletedDate = reader["CompletedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CompletedDate")),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                CreatedByName = reader["CreatedByName"] == DBNull.Value ? null : reader["CreatedByName"].ToString(),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ModifiedBy")),
                ModifiedByName = reader["ModifiedByName"] == DBNull.Value ? null : reader["ModifiedByName"].ToString()
            };
        }
    }
}