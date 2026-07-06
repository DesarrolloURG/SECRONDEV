using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{   
    internal class Ctrl_FixedAssetTransferStatus
    {

        public static List<Mdl_FixedAssetTransferStatus> MostrarEstados(
            string statusCode = null,
            string statusName = null,
            bool? isActive = null)
        {
            List<Mdl_FixedAssetTransferStatus> lista = new List<Mdl_FixedAssetTransferStatus>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferStatus_Select", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@StatusCode", (object)statusCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@StatusName", (object)statusName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", isActive.HasValue ? (object)isActive.Value : DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearEstado(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener estados: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }


        public static List<KeyValuePair<int, string>> ObtenerEstadosParaCombo(bool soloActivos = true)
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferStatus_Select", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@StatusCode", DBNull.Value);
                        cmd.Parameters.AddWithValue("@StatusName", DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", soloActivos ? (object)true : DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(new KeyValuePair<int, string>(
                                    reader.GetInt32(reader.GetOrdinal("TransferStatusId")),
                                    reader["StatusName"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener estados para combo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static int RegistrarEstado(Mdl_FixedAssetTransferStatus estado)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferStatus_Insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@StatusCode", estado.StatusCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@StatusName", estado.StatusName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)estado.Description?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Order", estado.Order);
                        cmd.Parameters.AddWithValue("@IsFinal", estado.IsFinal);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)estado.CreatedBy ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar estado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarEstado(Mdl_FixedAssetTransferStatus estado)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferStatus_Update", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TransferStatusId", estado.TransferStatusId);
                        cmd.Parameters.AddWithValue("@StatusCode", estado.StatusCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@StatusName", estado.StatusName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)estado.Description?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Order", estado.Order);
                        cmd.Parameters.AddWithValue("@IsFinal", estado.IsFinal);
                        cmd.Parameters.AddWithValue("@IsActive", estado.IsActive);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)estado.ModifiedBy ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar estado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }


        public static int InactivarEstado(int transferStatusId, int? modifiedBy = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferStatus_Inactive", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TransferStatusId", transferStatusId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar estado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private static Mdl_FixedAssetTransferStatus MapearEstado(SqlDataReader reader)
        {
            return new Mdl_FixedAssetTransferStatus
            {
                TransferStatusId = reader.GetInt32(reader.GetOrdinal("TransferStatusId")),
                StatusCode = reader["StatusCode"].ToString(),
                StatusName = reader["StatusName"].ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                Order = reader.GetInt32(reader.GetOrdinal("Order")),
                IsFinal = reader.GetBoolean(reader.GetOrdinal("IsFinal")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ModifiedBy"))
            };
        }

        public static int EliminarEstado(int transferStatusId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferStatus_Delete", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TransferStatusId", transferStatusId);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar estado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
}