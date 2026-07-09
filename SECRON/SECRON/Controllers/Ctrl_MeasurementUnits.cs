using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_MeasurementUnits
    {
        // MÉTODO PRINCIPAL: Registrar unidad
        public static int RegistrarUnidad(Mdl_MeasurementUnits unidad, int createdBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_MeasurementUnits_Insert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UnitCode", unidad.UnitCode ?? "");
                    cmd.Parameters.AddWithValue("@UnitName", unidad.UnitName ?? "");
                    cmd.Parameters.AddWithValue("@Abbreviation", unidad.Abbreviation ?? "");
                    cmd.Parameters.AddWithValue("@IsActive", unidad.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar unidad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todas las unidades
        public static List<Mdl_MeasurementUnits> MostrarUnidades()
        {
            List<Mdl_MeasurementUnits> lista = new List<Mdl_MeasurementUnits>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM MeasurementUnits WHERE IsActive = 1 ORDER BY UnitName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearUnidad(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener unidades: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Actualizar unidad
        public static int ActualizarUnidad(Mdl_MeasurementUnits unidad, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_MeasurementUnits_Update", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UnitId", unidad.UnitId);
                    cmd.Parameters.AddWithValue("@IsInactivation", false);
                    cmd.Parameters.AddWithValue("@UnitCode", unidad.UnitCode ?? "");
                    cmd.Parameters.AddWithValue("@UnitName", unidad.UnitName ?? "");
                    cmd.Parameters.AddWithValue("@Abbreviation", unidad.Abbreviation ?? "");
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar unidad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar unidad
        public static int InactivarUnidad(int unitId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_MeasurementUnits_Update", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UnitId", unitId);
                    cmd.Parameters.AddWithValue("@IsInactivation", true);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar unidad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear unidad
        private static Mdl_MeasurementUnits MapearUnidad(SqlDataReader reader)
        {
            return new Mdl_MeasurementUnits
            {
                UnitId = reader.GetInt32(0),
                UnitCode = reader[1].ToString(),
                UnitName = reader[2].ToString(),
                Abbreviation = reader[3].ToString(),
                IsActive = reader.GetBoolean(4)
            };
        }

        // MÉTODO PARA OBTENER UNIDADES PARA COMBOBOX
        public static List<KeyValuePair<int, string>> ObtenerUnidadesParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT UnitId, UnitName FROM MeasurementUnits WHERE IsActive = 1 ORDER BY UnitName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener unidades: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA OBTENER EL PRÓXIMO CÓDIGO DE UNIDAD
        public static string ObtenerProximoCodigoUnidad()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT TOP 1 UnitCode
                             FROM MeasurementUnits
                             WHERE UnitCode IS NOT NULL
                             ORDER BY UnitId DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && !string.IsNullOrWhiteSpace(resultado.ToString()))
                        {
                            string ultimoCodigo = resultado.ToString();

                            // Si el código es numérico puro, se incrementa
                            if (int.TryParse(ultimoCodigo, out int numeroActual))
                            {
                                int proximoNumero = numeroActual + 1;
                                return proximoNumero.ToString("D6");
                            }
                            else
                            {
                                // Si contiene letras + números (ej: UM000123)
                                string soloNumeros = new string(ultimoCodigo.Where(char.IsDigit).ToArray());

                                if (!string.IsNullOrWhiteSpace(soloNumeros) &&
                                    int.TryParse(soloNumeros, out int numExtraido))
                                {
                                    int proximoNumero = numExtraido + 1;

                                    // Prefijo (ej: UM)
                                    string prefijo = new string(ultimoCodigo.Where(char.IsLetter).ToArray());
                                    return $"{prefijo}{proximoNumero:D6}";
                                }
                                else
                                {
                                    // No se pudo extraer nada numérico
                                    return "000001";
                                }
                            }
                        }
                        else
                        {
                            // Si no hay registros, iniciar desde 000001
                            return "000001";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de unidad: {ex.Message}",
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "ERROR";
            }
        }

    }
}