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
    internal class Ctrl_PurchaseRequestDetails
    {
        // MÉTODO PRINCIPAL: Registrar detalle
        public static int RegistrarDetalle(Mdl_PurchaseRequestDetails detalle)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO PurchaseRequestDetails (RequestMasterId, ItemId, SupplierId, 
                        Quantity, UnitCost, PriorityId, StatusId, RequestReason) 
                        VALUES (@RequestMasterId, @ItemId, @SupplierId, @Quantity, @UnitCost, 
                        @PriorityId, @StatusId, @RequestReason)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequestMasterId", detalle.RequestMasterId);
                        cmd.Parameters.AddWithValue("@ItemId", detalle.ItemId);
                        cmd.Parameters.AddWithValue("@SupplierId", detalle.SupplierId);
                        cmd.Parameters.AddWithValue("@Quantity", detalle.Quantity);
                        cmd.Parameters.AddWithValue("@UnitCost", detalle.UnitCost);
                        cmd.Parameters.AddWithValue("@PriorityId", detalle.PriorityId);
                        cmd.Parameters.AddWithValue("@StatusId", detalle.StatusId);
                        cmd.Parameters.AddWithValue("@RequestReason", (object)detalle.RequestReason ?? DBNull.Value);

                        int result = cmd.ExecuteNonQuery();

                        // Actualizar total del master
                        string queryUpdateTotal = @"UPDATE PurchaseRequestMaster SET TotalBudget = 
                            (SELECT ISNULL(SUM(Quantity * UnitCost), 0) FROM PurchaseRequestDetails 
                            WHERE RequestMasterId = @RequestMasterId) 
                            WHERE RequestMasterId = @RequestMasterId";

                        using (SqlCommand cmdTotal = new SqlCommand(queryUpdateTotal, connection))
                        {
                            cmdTotal.Parameters.AddWithValue("@RequestMasterId", detalle.RequestMasterId);
                            cmdTotal.ExecuteNonQuery();
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar detalles por solicitud
        public static List<Mdl_PurchaseRequestDetails> MostrarDetallesPorSolicitud(int requestMasterId)
        {
            List<Mdl_PurchaseRequestDetails> lista = new List<Mdl_PurchaseRequestDetails>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM PurchaseRequestDetails WHERE RequestMasterId = @RequestMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequestMasterId", requestMasterId);
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
        public static int EliminarDetalle(int requestDetailId, int requestMasterId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "DELETE FROM PurchaseRequestDetails WHERE RequestDetailId = @RequestDetailId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequestDetailId", requestDetailId);
                        int result = cmd.ExecuteNonQuery();

                        // Actualizar total del master
                        string queryUpdateTotal = @"UPDATE PurchaseRequestMaster SET TotalBudget = 
                            (SELECT ISNULL(SUM(Quantity * UnitCost), 0) FROM PurchaseRequestDetails 
                            WHERE RequestMasterId = @RequestMasterId) 
                            WHERE RequestMasterId = @RequestMasterId";

                        using (SqlCommand cmdTotal = new SqlCommand(queryUpdateTotal, connection))
                        {
                            cmdTotal.Parameters.AddWithValue("@RequestMasterId", requestMasterId);
                            cmdTotal.ExecuteNonQuery();
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear detalle
        private static Mdl_PurchaseRequestDetails MapearDetalle(SqlDataReader reader)
        {
            return new Mdl_PurchaseRequestDetails
            {
                RequestDetailId = reader.GetInt32(0),
                RequestMasterId = reader.GetInt32(1),
                ItemId = reader.GetInt32(2),
                SupplierId = reader.GetInt32(3),
                Quantity = reader.GetDecimal(4),
                UnitCost = reader.GetDecimal(5),
                TotalCost = reader.GetDecimal(6),
                PriorityId = reader.GetInt32(7),
                StatusId = reader.GetInt32(8),
                RequestReason = reader[9] == DBNull.Value ? null : reader[9].ToString()
            };
        }
    }
}