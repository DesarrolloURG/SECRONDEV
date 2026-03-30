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
    internal class Ctrl_AccountingEntryDetails
    {
        #region CRUD
        // MÉTODO PRINCIPAL: Registrar detalle
        public static int RegistrarDetalle(Mdl_AccountingEntryDetails detalle)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO AccountingEntryDetails (EntryMasterId, AccountId, Debit, Credit, Remarks) 
                        VALUES (@EntryMasterId, @AccountId, @Debit, @Credit, @Remarks)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EntryMasterId", detalle.EntryMasterId);
                        cmd.Parameters.AddWithValue("@AccountId", detalle.AccountId);
                        cmd.Parameters.AddWithValue("@Debit", detalle.Debit);
                        cmd.Parameters.AddWithValue("@Credit", detalle.Credit);
                        cmd.Parameters.AddWithValue("@Remarks", (object)detalle.Remarks ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todos los detalles
        public static List<Mdl_AccountingEntryDetails> MostrarRegistros()
        {
            List<Mdl_AccountingEntryDetails> lista = new List<Mdl_AccountingEntryDetails>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM AccountingEntryDetails";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearDetalle(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener detalles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Mostrar detalles por partida
        public static List<Mdl_AccountingEntryDetails> MostrarDetallesPorPartida(int entryMasterId)
        {
            List<Mdl_AccountingEntryDetails> lista = new List<Mdl_AccountingEntryDetails>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM AccountingEntryDetails WHERE EntryMasterId = @EntryMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EntryMasterId", entryMasterId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearDetalle(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener detalles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
        // MÉTODO PRINCIPAL: Eliminar detalle
        public static int EliminarDetalle(int entryDetailId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "DELETE FROM AccountingEntryDetails WHERE EntryDetailId = @EntryDetailId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EntryDetailId", entryDetailId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
        #endregion CRUD
        #region Mapeo
        // MÉTODO AUXILIAR: Mapear detalle
        private static Mdl_AccountingEntryDetails MapearDetalle(SqlDataReader reader)
        {
            return new Mdl_AccountingEntryDetails
            {
                EntryDetailId = reader.GetInt32(0),
                EntryMasterId = reader.GetInt32(1),
                AccountId = reader.GetInt32(2),
                Debit = reader.GetDecimal(3),
                Credit = reader.GetDecimal(4),
                Remarks = reader[5] == DBNull.Value ? null : reader[5].ToString()
            };
        }
        #endregion Mapeo
    }
}