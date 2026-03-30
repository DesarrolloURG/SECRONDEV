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
    internal class Ctrl_CheckStatus
    {
        // MÉTODO PRINCIPAL: Registrar estado
        public static int RegistrarEstado(Mdl_CheckStatus estado)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO CheckStatus (StatusName, Description, IsActive) 
                        VALUES (@StatusName, @Description, @IsActive)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StatusName", estado.StatusName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)estado.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", estado.IsActive);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar estado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todos los estados
        public static List<Mdl_CheckStatus> MostrarEstados()
        {
            List<Mdl_CheckStatus> lista = new List<Mdl_CheckStatus>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM CheckStatus WHERE IsActive = 1 ORDER BY StatusName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearEstado(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener estados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO AUXILIAR: Mapear estado
        private static Mdl_CheckStatus MapearEstado(SqlDataReader reader)
        {
            return new Mdl_CheckStatus
            {
                StatusId = reader.GetInt32(0),
                StatusName = reader[1].ToString(),
                Description = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                IsActive = reader.GetBoolean(3)
            };
        }

        // MÉTODO PARA OBTENER ESTADOS PARA COMBOBOX
        public static List<KeyValuePair<int, string>> ObtenerEstadosParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT StatusId, StatusName FROM CheckStatus WHERE IsActive = 1 ORDER BY StatusName";
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
                MessageBox.Show("Error al obtener estados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
        // Método para obtener nombre del estado por ID
        public static string ObtenerNombreEstado(int statusId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT StatusName FROM CheckStatus WHERE StatusId = @StatusId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StatusId", statusId);
                        object result = cmd.ExecuteScalar();
                        return result?.ToString() ?? "DESCONOCIDO";
                    }
                }
            }
            catch
            {
                return "ERROR";
            }
        }
    }
}