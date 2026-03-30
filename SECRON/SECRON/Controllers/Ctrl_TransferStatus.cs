using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_TransferStatus
    {
        // Registrar estado
        public static int RegistrarEstado(Mdl_TransferStatus estado)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO TransferStatus (StatusName, Description, IsActive)
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
                MessageBox.Show("Error al registrar estado de transferencia: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Listar todos los estados
        public static List<Mdl_TransferStatus> MostrarEstados(bool soloActivos = true)
        {
            var lista = new List<Mdl_TransferStatus>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT StatusId, StatusName, Description, IsActive FROM TransferStatus WHERE 1=1");

                    if (soloActivos)
                        query.Append(" AND IsActive = 1");

                    query.Append(" ORDER BY StatusId ASC");

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Mdl_TransferStatus
                            {
                                StatusId = reader.GetInt32(0),
                                StatusName = reader[1].ToString(),
                                Description = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                                IsActive = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar estados de transferencia: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        // Para combos (Id, Nombre)
        public static List<KeyValuePair<int, string>> ObtenerEstadosParaCombo(bool soloActivos = true)
        {
            var lista = new List<KeyValuePair<int, string>>();
            foreach (var est in MostrarEstados(soloActivos))
            {
                lista.Add(new KeyValuePair<int, string>(est.StatusId, est.StatusName));
            }
            return lista;
        }

        // Obtener nombre de estado por Id
        public static string ObtenerNombreEstado(int statusId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT StatusName FROM TransferStatus WHERE StatusId = @StatusId";

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

        // Helper: Obtener Id del estado COMPLETADA
        public static int ObtenerStatusCompletadaId()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT StatusId FROM TransferStatus WHERE UPPER(StatusName) = 'COMPLETADA' AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object result = cmd.ExecuteScalar();
                        return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
                    }
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
