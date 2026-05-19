using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_FixedAssetTransferStatusTransitions
    {
        // ─────────────────────────────────────────────
        // READ - por estado origen (uso principal desde el formulario)
        // ─────────────────────────────────────────────
        public static List<Mdl_FixedAssetTransferStatusTransition> MostrarTransiciones(
            int? fromStatusId = null,
            int? toStatusId = null)
        {
            List<Mdl_FixedAssetTransferStatusTransition> lista = new List<Mdl_FixedAssetTransferStatusTransition>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferStatusTransitions_Select", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@FromStatusId", fromStatusId.HasValue ? (object)fromStatusId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToStatusId", toStatusId.HasValue ? (object)toStatusId.Value : DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearTransicion(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener transiciones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // ─────────────────────────────────────────────
        // CREATE
        // ─────────────────────────────────────────────
        public static int RegistrarTransicion(int fromStatusId, int toStatusId, int? createdBy = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferStatusTransitions_Insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@FromStatusId", fromStatusId);
                        cmd.Parameters.AddWithValue("@ToStatusId", toStatusId);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)createdBy ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar transición: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // ─────────────────────────────────────────────
        // DELETE físico
        // ─────────────────────────────────────────────
        public static int EliminarTransicion(int transitionId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetTransferStatusTransitions_Delete", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TransitionId", transitionId);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar transición: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // ─────────────────────────────────────────────
        // MAPPER
        // ─────────────────────────────────────────────
        private static Mdl_FixedAssetTransferStatusTransition MapearTransicion(SqlDataReader reader)
        {
            return new Mdl_FixedAssetTransferStatusTransition
            {
                TransitionId = reader.GetInt32(reader.GetOrdinal("TransitionId")),
                FromStatusId = reader.GetInt32(reader.GetOrdinal("FromStatusId")),
                ToStatusId = reader.GetInt32(reader.GetOrdinal("ToStatusId")),
                FromStatusCode = reader["FromStatusCode"].ToString(),
                FromStatusName = reader["FromStatusName"].ToString(),
                ToStatusCode = reader["ToStatusCode"].ToString(),
                ToStatusName = reader["ToStatusName"].ToString(),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("CreatedBy"))
            };
        }
    }
}