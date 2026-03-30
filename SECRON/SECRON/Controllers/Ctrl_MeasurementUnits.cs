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
    internal class Ctrl_MeasurementUnits
    {
        // MÉTODO PRINCIPAL: Registrar unidad
        public static int RegistrarUnidad(Mdl_MeasurementUnits unidad)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO MeasurementUnits (UnitCode, UnitName, Abbreviation, IsActive) 
                        VALUES (@UnitCode, @UnitName, @Abbreviation, @IsActive)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UnitCode", unidad.UnitCode ?? "");
                        cmd.Parameters.AddWithValue("@UnitName", unidad.UnitName ?? "");
                        cmd.Parameters.AddWithValue("@Abbreviation", unidad.Abbreviation ?? "");
                        cmd.Parameters.AddWithValue("@IsActive", unidad.IsActive);

                        return cmd.ExecuteNonQuery();
                    }
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
        public static int ActualizarUnidad(Mdl_MeasurementUnits unidad)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE MeasurementUnits SET UnitCode = @UnitCode, 
                        UnitName = @UnitName, Abbreviation = @Abbreviation 
                        WHERE UnitId = @UnitId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UnitId", unidad.UnitId);
                        cmd.Parameters.AddWithValue("@UnitCode", unidad.UnitCode ?? "");
                        cmd.Parameters.AddWithValue("@UnitName", unidad.UnitName ?? "");
                        cmd.Parameters.AddWithValue("@Abbreviation", unidad.Abbreviation ?? "");

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar unidad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar unidad
        public static int InactivarUnidad(int unitId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE MeasurementUnits SET IsActive = 0 WHERE UnitId = @UnitId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UnitId", unitId);
                        return cmd.ExecuteNonQuery();
                    }
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