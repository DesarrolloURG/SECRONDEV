using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_AccountingEntryTransfers
    {
        #region VÍNCULOS BÁSICOS

        // Registrar vínculo entre una partida contable y una transferencia
        public static bool RegistrarVinculo(int entryMasterId, int transferId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        INSERT INTO AccountingEntryTransfers (EntryMasterId, TransferId)
                        VALUES (@EntryMasterId, @TransferId);";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EntryMasterId", entryMasterId);
                        cmd.Parameters.AddWithValue("@TransferId", transferId);

                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al vincular partida con transferencia: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Obtener EntryMasterId a partir de un TransferId
        // (equivalente a BuscarIdPorCheque, pero para transferencias)
        public static int BuscarIdPorTransfer(int transferId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT EntryMasterId
                        FROM AccountingEntryTransfers
                        WHERE TransferId = @TransferId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransferId", transferId);
                        object result = cmd.ExecuteScalar();

                        return (result == null || result == DBNull.Value)
                            ? 0
                            : Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar partida por transferencia: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Obtener TransferId a partir de un EntryMasterId (trazabilidad inversa)
        public static int ObtenerTransferIdPorEntry(int entryMasterId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT TransferId
                        FROM AccountingEntryTransfers
                        WHERE EntryMasterId = @EntryMasterId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EntryMasterId", entryMasterId);
                        object result = cmd.ExecuteScalar();

                        return (result == null || result == DBNull.Value)
                            ? 0
                            : Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar transferencia por partida: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Eliminar vínculo por EntryMasterId
        public static bool EliminarVinculoPorEntry(int entryMasterId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"DELETE FROM AccountingEntryTransfers WHERE EntryMasterId = @EntryMasterId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EntryMasterId", entryMasterId);
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar vínculo de partida con transferencia: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion
        #region MÉTODOS BASADOS EN TRANSFERID (ANÁLOGOS A LOS DE CHEQUE)

        // Contar cuántas partidas están vinculadas a una transferencia
        // (por diseño debería ser 0 o 1, pero mantenemos la firma)
        public static int ContarPartidasPorTransfer(int transferId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT COUNT(*)
                        FROM AccountingEntryTransfers
                        WHERE TransferId = @TransferId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransferId", transferId);
                        object result = cmd.ExecuteScalar();

                        return (result == null || result == DBNull.Value)
                            ? 0
                            : Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al contar partidas por transferencia: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Actualizar TransferId en todas las partidas vinculadas
        public static int ActualizarTransferIdEnTodasPartidas(int transferIdAntiguo, int transferIdNuevo)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        UPDATE AccountingEntryTransfers
                        SET TransferId = @TransferIdNuevo
                        WHERE TransferId = @TransferIdAntiguo";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransferIdNuevo", transferIdNuevo);
                        cmd.Parameters.AddWithValue("@TransferIdAntiguo", transferIdAntiguo);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar TransferId en partidas: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Buscar el valor de un campo de AccountingEntryMaster usando TransferId
        public static string BuscarCampo(int transferId, string campo)
        {
            if (string.IsNullOrWhiteSpace(campo))
            {
                MessageBox.Show("El nombre del campo no puede estar vacío.",
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = $@"
                        SELECT m.{campo}
                        FROM AccountingEntryMaster m
                        INNER JOIN AccountingEntryTransfers t
                            ON t.EntryMasterId = m.EntryMasterId
                        WHERE t.TransferId = @TransferId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransferId", transferId);
                        object result = cmd.ExecuteScalar();

                        return result == null || result == DBNull.Value
                            ? null
                            : result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar campo de partida por transferencia: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Modificar un campo de AccountingEntryMaster usando TransferId
        public static int ModificarCampo(int transferId, string campo, string nuevoValor)
        {
            if (string.IsNullOrWhiteSpace(campo))
            {
                MessageBox.Show("El nombre del campo no puede estar vacío.",
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = $@"
                        UPDATE m
                        SET m.{campo} = @NuevoValor
                        FROM AccountingEntryMaster m
                        INNER JOIN AccountingEntryTransfers t
                            ON t.EntryMasterId = m.EntryMasterId
                        WHERE t.TransferId = @TransferId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransferId", transferId);
                        cmd.Parameters.AddWithValue("@NuevoValor", (object)nuevoValor ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar campo de partida por transferencia: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion
    }
}
