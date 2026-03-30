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
    internal class Ctrl_PurchaseRequestStatus
    {
        // MÉTODO PARA COMBOBOX
        public static List<KeyValuePair<int, string>> ObtenerEstadosParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT StatusId, StatusName FROM PurchaseRequestStatus WHERE IsActive = 1 ORDER BY StatusName";
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

        // MÉTODO PRINCIPAL: Mostrar estados
        public static List<Mdl_PurchaseRequestStatus> MostrarEstados()
        {
            List<Mdl_PurchaseRequestStatus> lista = new List<Mdl_PurchaseRequestStatus>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM PurchaseRequestStatus WHERE IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Mdl_PurchaseRequestStatus
                                {
                                    StatusId = reader.GetInt32(0),
                                    StatusCode = reader[1].ToString(),
                                    StatusName = reader[2].ToString(),
                                    IsActive = reader.GetBoolean(3)
                                });
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
    }
}