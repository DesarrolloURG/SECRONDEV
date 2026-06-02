using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_FixedAssetTransfers
    {
        // ══════════════════════════════════════════════
        // MAESTRO
        // ══════════════════════════════════════════════

        #region Próximo código automático

        public static string ObtenerProximoCodigo()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransfers_NextCode", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        object resultado = cmd.ExecuteScalar();
                        return resultado?.ToString() ?? "TRA-000001";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener código de traslado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "TRA-000001";
            }
        }

        #endregion

        #region READ Maestro

        public static List<Mdl_FixedAssetTransfer> MostrarTraslados(
            string transferCode = null,
            int? transferStatusId = null,
            int? toWarehouseId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            List<Mdl_FixedAssetTransfer> lista = new List<Mdl_FixedAssetTransfer>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransfers_Select", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransferCode", (object)transferCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TransferStatusId", transferStatusId.HasValue ? (object)transferStatusId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToWarehouseId", toWarehouseId.HasValue ? (object)toWarehouseId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.HasValue ? (object)fechaInicio.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin.HasValue ? (object)fechaFin.Value : DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearTraslado(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener traslados: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        #endregion

        #region CREATE Maestro — retorna el TransferId generado

        public static int RegistrarTraslado(Mdl_FixedAssetTransfer transfer)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransfers_Insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransferCode", transfer.TransferCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@TransferDate", transfer.TransferDate);
                        cmd.Parameters.AddWithValue("@ToWarehouseId", (object)transfer.ToWarehouseId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToEmployeeId", (object)transfer.ToEmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TransferStatusId", transfer.TransferStatusId);
                        cmd.Parameters.AddWithValue("@Reason", (object)transfer.Reason?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)transfer.CreatedBy ?? DBNull.Value);

                        object resultado = cmd.ExecuteScalar();
                        return resultado != null ? Convert.ToInt32(resultado) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar traslado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region UPDATE Maestro

        public static int ActualizarTraslado(Mdl_FixedAssetTransfer transfer)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransfers_Update", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransferId", transfer.TransferId);
                        cmd.Parameters.AddWithValue("@TransferCode", transfer.TransferCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@TransferDate", transfer.TransferDate);
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
                MessageBox.Show("Error al actualizar traslado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region INACTIVE Maestro

        public static int InactivarTraslado(int transferId, int? modifiedBy = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransfers_Inactive", connection))
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
                MessageBox.Show("Error al cancelar traslado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region MAPPER Maestro

        private static Mdl_FixedAssetTransfer MapearTraslado(SqlDataReader reader)
        {
            return new Mdl_FixedAssetTransfer
            {
                TransferId = reader.GetInt32(reader.GetOrdinal("TransferId")),
                TransferCode = reader["TransferCode"].ToString(),
                TransferDate = reader.GetDateTime(reader.GetOrdinal("TransferDate")),
                ToWarehouseId = reader["ToWarehouseId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ToWarehouseId")),
                ToWarehouseName = reader["ToWarehouseName"] == DBNull.Value ? null : reader["ToWarehouseName"].ToString(),
                ToEmployeeId = reader["ToEmployeeId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ToEmployeeId")),
                ToEmployeeName = reader["ToEmployeeName"] == DBNull.Value ? null : reader["ToEmployeeName"].ToString(),
                TransferStatusId = reader.GetInt32(reader.GetOrdinal("TransferStatusId")),
                StatusCode = reader["StatusCode"].ToString(),
                StatusName = reader["StatusName"].ToString(),
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

        #endregion

        // ══════════════════════════════════════════════
        // DETALLE
        // ══════════════════════════════════════════════

        #region READ Detalle

        public static List<Mdl_FixedAssetTransferDetail> MostrarDetalles(int transferId)
        {
            List<Mdl_FixedAssetTransferDetail> lista = new List<Mdl_FixedAssetTransferDetail>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferDetails_Select", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransferId", transferId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearDetalle(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener detalles del traslado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        #endregion

        #region CREATE Detalle

        public static int AgregarDetalle(Mdl_FixedAssetTransferDetail detalle)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferDetails_Insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransferId", detalle.TransferId);
                        cmd.Parameters.AddWithValue("@AssetId", detalle.AssetId);
                        cmd.Parameters.AddWithValue("@FromWarehouseId", (object)detalle.FromWarehouseId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FromEmployeeId", (object)detalle.FromEmployeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)detalle.CreatedBy ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar activo al traslado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region DELETE Detalle

        public static int EliminarDetalle(int transferDetailId, int? modifiedBy = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferDetails_Delete", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransferDetailId", transferDetailId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar activo del traslado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region MAPPER Detalle

        private static Mdl_FixedAssetTransferDetail MapearDetalle(SqlDataReader reader)
        {
            return new Mdl_FixedAssetTransferDetail
            {
                TransferDetailId = reader.GetInt32(reader.GetOrdinal("TransferDetailId")),
                TransferId = reader.GetInt32(reader.GetOrdinal("TransferId")),
                AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")),
                AssetCode = reader["AssetCode"].ToString(),
                AssetName = reader["AssetName"].ToString(),
                FromWarehouseId = reader["FromWarehouseId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("FromWarehouseId")),
                FromWarehouseName = reader["FromWarehouseName"] == DBNull.Value ? null : reader["FromWarehouseName"].ToString(),
                FromEmployeeId = reader["FromEmployeeId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("FromEmployeeId")),
                FromEmployeeName = reader["FromEmployeeName"] == DBNull.Value ? null : reader["FromEmployeeName"].ToString(),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                CreatedByName = reader["CreatedByName"] == DBNull.Value ? null : reader["CreatedByName"].ToString()
            };
        }

        #endregion
    }
}