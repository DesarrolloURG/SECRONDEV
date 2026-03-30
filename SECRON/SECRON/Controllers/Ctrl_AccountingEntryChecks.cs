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
    internal class Ctrl_AccountingEntryChecks
    {
        #region VÍNCULOS BÁSICOS

        // Registrar vínculo entre una partida contable y un cheque
        public static bool RegistrarVinculo(int entryMasterId, int checkId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        INSERT INTO AccountingEntryChecks (EntryMasterId, CheckId)
                        VALUES (@EntryMasterId, @CheckId);";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@EntryMasterId", entryMasterId);
                        cmd.Parameters.AddWithValue("@CheckId", checkId);

                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al vincular partida con cheque: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Obtener EntryMasterId a partir de un CheckId (equivalente al antiguo BuscarIdPorCheque)
        public static int BuscarIdPorCheque(int checkId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT EntryMasterId
                        FROM AccountingEntryChecks
                        WHERE CheckId = @CheckId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckId", checkId);
                        object result = cmd.ExecuteScalar();

                        return (result == null || result == DBNull.Value)
                            ? 0
                            : Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar partida por cheque: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Obtener CheckId a partir de un EntryMasterId (útil para trazabilidad inversa)
        public static int ObtenerCheckIdPorEntry(int entryMasterId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT CheckId
                        FROM AccountingEntryChecks
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
                MessageBox.Show("Error al buscar cheque por partida: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Eliminar vínculo por EntryMasterId (si eliminas la partida y no tienes ON DELETE CASCADE)
        public static bool EliminarVinculoPorEntry(int entryMasterId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"DELETE FROM AccountingEntryChecks WHERE EntryMasterId = @EntryMasterId";

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
                MessageBox.Show("Error al eliminar vínculo de partida con cheque: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion
        #region MÉTODOS BASADOS EN CHECKID

        // Contar cuántas partidas están vinculadas a un cheque
        // (por diseño debería ser 0 o 1, pero mantenemos la firma)
        public static int ContarPartidasPorCheque(int checkId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT COUNT(*)
                        FROM AccountingEntryChecks
                        WHERE CheckId = @CheckId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckId", checkId);
                        object result = cmd.ExecuteScalar();

                        return (result == null || result == DBNull.Value)
                            ? 0
                            : Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al contar partidas por cheque: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Actualizar CheckId en todas las partidas vinculadas (por ejemplo al renumerar cheques)
        public static int ActualizarCheckIdEnTodasPartidas(int checkIdAntiguo, int checkIdNuevo)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        UPDATE AccountingEntryChecks
                        SET CheckId = @CheckIdNuevo
                        WHERE CheckId = @CheckIdAntiguo";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckIdNuevo", checkIdNuevo);
                        cmd.Parameters.AddWithValue("@CheckIdAntiguo", checkIdAntiguo);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar CheckId en partidas: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Buscar el valor de un campo de AccountingEntryMaster usando CheckId
        // Equivalente al antiguo BuscarCampo(int checkId, string campo)
        public static string BuscarCampo(int checkId, string campo)
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
                    // NOTA: Campo se concatena en la consulta, asumiendo que viene
                    // desde código interno (no input de usuario final).
                    string query = $@"
                        SELECT m.{campo}
                        FROM AccountingEntryMaster m
                        INNER JOIN AccountingEntryChecks c
                            ON c.EntryMasterId = m.EntryMasterId
                        WHERE c.CheckId = @CheckId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckId", checkId);
                        object result = cmd.ExecuteScalar();

                        return result == null || result == DBNull.Value
                            ? null
                            : result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar campo de partida por cheque: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Modificar un campo de AccountingEntryMaster usando CheckId
        // Equivalente al antiguo ModificarCampo(int checkId, string campo, string nuevoValor)
        public static int ModificarCampo(int checkId, string campo, string nuevoValor)
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
                        INNER JOIN AccountingEntryChecks c
                            ON c.EntryMasterId = m.EntryMasterId
                        WHERE c.CheckId = @CheckId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckId", checkId);
                        cmd.Parameters.AddWithValue("@NuevoValor", (object)nuevoValor ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar campo de partida por cheque: " + ex.Message,
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion
    }
}
