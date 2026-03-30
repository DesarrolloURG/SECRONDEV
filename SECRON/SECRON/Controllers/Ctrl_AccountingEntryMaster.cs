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
    internal class Ctrl_AccountingEntryMaster
    {
        // MÉTODO PRINCIPAL: Registrar partida contable (retorna el ID generado)
        public static int RegistrarPartida(Mdl_AccountingEntryMaster partida)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        INSERT INTO AccountingEntryMaster
                            (EntryDate, Concept, StatusId, TotalAmount, CreatedBy, IsActive)
                        VALUES
                            (@EntryDate, @Concept, @StatusId, @TotalAmount, @CreatedBy, @IsActive);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EntryDate", partida.EntryDate);
                        cmd.Parameters.AddWithValue("@Concept", partida.Concept ?? "");
                        cmd.Parameters.AddWithValue("@StatusId", partida.StatusId);
                        cmd.Parameters.AddWithValue("@TotalAmount", partida.TotalAmount);
                        cmd.Parameters.AddWithValue("@CreatedBy", partida.CreatedBy);
                        cmd.Parameters.AddWithValue("@IsActive", partida.IsActive);

                        object result = cmd.ExecuteScalar();
                        return result == null || result == DBNull.Value
                            ? 0
                            : Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar partida: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener partida por ID
        public static Mdl_AccountingEntryMaster ObtenerPartidaPorId(int entryMasterId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // Importante: columnas en orden para MapearPartida
                    string query = @"
                        SELECT 
                            EntryMasterId,      -- 0
                            EntryDate,          -- 1
                            Concept,            -- 2
                            StatusId,           -- 3
                            TotalAmount,        -- 4
                            CreatedDate,        -- 5
                            CreatedBy,          -- 6
                            ApprovedDate,       -- 7
                            ApprovedBy,         -- 8
                            ModifiedDate,       -- 9
                            ModifiedBy,         -- 10
                            IsActive            -- 11
                        FROM AccountingEntryMaster
                        WHERE EntryMasterId = @EntryMasterId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EntryMasterId", entryMasterId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearPartida(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener partida: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        // MÉTODO PRINCIPAL: Eliminar partida
        public static int EliminarPartida(int entryMasterId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "DELETE FROM AccountingEntryMaster WHERE EntryMasterId = @EntryMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EntryMasterId", entryMasterId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar partida: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // OPCIONAL: Actualizar datos generales de una partida
        public static int ActualizarPartida(Mdl_AccountingEntryMaster partida)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        UPDATE AccountingEntryMaster
                        SET
                            EntryDate    = @EntryDate,
                            Concept      = @Concept,
                            StatusId     = @StatusId,
                            TotalAmount  = @TotalAmount,
                            ModifiedDate = GETDATE(),
                            ModifiedBy   = @ModifiedBy
                        WHERE EntryMasterId = @EntryMasterId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EntryMasterId", partida.EntryMasterId);
                        cmd.Parameters.AddWithValue("@EntryDate", partida.EntryDate);
                        cmd.Parameters.AddWithValue("@Concept", partida.Concept ?? "");
                        cmd.Parameters.AddWithValue("@StatusId", partida.StatusId);
                        cmd.Parameters.AddWithValue("@TotalAmount", partida.TotalAmount);
                        cmd.Parameters.AddWithValue("@ModifiedBy",
                            (object)partida.ModifiedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar partida: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear partida (mismo estilo por índice)
        private static Mdl_AccountingEntryMaster MapearPartida(SqlDataReader reader)
        {
            return new Mdl_AccountingEntryMaster
            {
                EntryMasterId = reader.GetInt32(0),
                EntryDate = reader.GetDateTime(1),
                Concept = reader[2].ToString(),
                StatusId = reader.GetInt32(3),
                TotalAmount = reader.GetDecimal(4),
                CreatedDate = reader.GetDateTime(5),
                CreatedBy = reader.GetInt32(6),
                ApprovedDate = reader[7] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(7),
                ApprovedBy = reader[8] == DBNull.Value ? null : (int?)reader.GetInt32(8),
                ModifiedDate = reader[9] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(9),
                ModifiedBy = reader[10] == DBNull.Value ? null : (int?)reader.GetInt32(10),
                IsActive = reader.GetBoolean(11)
            };
        }
    }
}