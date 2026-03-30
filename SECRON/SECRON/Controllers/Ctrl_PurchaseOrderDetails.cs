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
    internal class Ctrl_PurchaseOrderDetails
    {
        // MÉTODO PRINCIPAL: Registrar detalle
        public static int RegistrarDetalle(Mdl_PurchaseOrderDetails detalle)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO PurchaseOrderDetails (PurchaseOrderId, RequisitionDetailId, 
                        ItemId, Quantity, UnitCost) 
                        VALUES (@PurchaseOrderId, @RequisitionDetailId, @ItemId, @Quantity, @UnitCost)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseOrderId", detalle.PurchaseOrderId);
                        cmd.Parameters.AddWithValue("@RequisitionDetailId", (object)detalle.RequisitionDetailId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ItemId", detalle.ItemId);
                        cmd.Parameters.AddWithValue("@Quantity", detalle.Quantity);
                        cmd.Parameters.AddWithValue("@UnitCost", detalle.UnitCost);

                        int result = cmd.ExecuteNonQuery();

                        // Actualizar total del master
                        string queryUpdateTotal = @"UPDATE PurchaseOrderMaster SET TotalAmount = 
                            (SELECT ISNULL(SUM(Quantity * UnitCost), 0) FROM PurchaseOrderDetails 
                            WHERE PurchaseOrderId = @PurchaseOrderId) 
                            WHERE PurchaseOrderId = @PurchaseOrderId";

                        using (SqlCommand cmdTotal = new SqlCommand(queryUpdateTotal, connection))
                        {
                            cmdTotal.Parameters.AddWithValue("@PurchaseOrderId", detalle.PurchaseOrderId);
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

        // MÉTODO PRINCIPAL: Mostrar detalles por orden
        public static List<Mdl_PurchaseOrderDetails> MostrarDetallesPorOrden(int purchaseOrderId)
        {
            List<Mdl_PurchaseOrderDetails> lista = new List<Mdl_PurchaseOrderDetails>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM PurchaseOrderDetails WHERE PurchaseOrderId = @PurchaseOrderId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseOrderId", purchaseOrderId);
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

        // MÉTODO AUXILIAR: Mapear detalle
        private static Mdl_PurchaseOrderDetails MapearDetalle(SqlDataReader reader)
        {
            return new Mdl_PurchaseOrderDetails
            {
                PurchaseOrderDetailId = reader.GetInt32(0),
                PurchaseOrderId = reader.GetInt32(1),
                RequisitionDetailId = reader[2] == DBNull.Value ? null : (int?)reader.GetInt32(2),
                ItemId = reader.GetInt32(3),
                Quantity = reader.GetDecimal(4),
                UnitCost = reader.GetDecimal(5),
                TotalCost = reader.GetDecimal(6)
            };
        }
    }
}