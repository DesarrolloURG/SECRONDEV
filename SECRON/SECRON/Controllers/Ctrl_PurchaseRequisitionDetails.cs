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
    internal class Ctrl_PurchaseRequisitionDetails
    {
        // MÉTODO PRINCIPAL: Registrar detalle
        public static int RegistrarDetalle(Mdl_PurchaseRequisitionDetails detalle)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO PurchaseRequisitionDetails (RequisitionMasterId, RequestDetailId, 
                        ItemId, SupplierId, Quantity, UnitCost, PriorityId, StatusId, RequestReason) 
                        VALUES (@RequisitionMasterId, @RequestDetailId, @ItemId, @SupplierId, @Quantity, 
                        @UnitCost, @PriorityId, @StatusId, @RequestReason)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionMasterId", detalle.RequisitionMasterId);
                        cmd.Parameters.AddWithValue("@RequestDetailId", (object)detalle.RequestDetailId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ItemId", detalle.ItemId);
                        cmd.Parameters.AddWithValue("@SupplierId", detalle.SupplierId);
                        cmd.Parameters.AddWithValue("@Quantity", detalle.Quantity);
                        cmd.Parameters.AddWithValue("@UnitCost", detalle.UnitCost);
                        cmd.Parameters.AddWithValue("@PriorityId", detalle.PriorityId);
                        cmd.Parameters.AddWithValue("@StatusId", detalle.StatusId);
                        cmd.Parameters.AddWithValue("@RequestReason", (object)detalle.RequestReason ?? DBNull.Value);

                        int result = cmd.ExecuteNonQuery();

                        // Actualizar total del master
                        string queryUpdateTotal = @"UPDATE PurchaseRequisitionMaster SET TotalBudget = 
                            (SELECT ISNULL(SUM(Quantity * UnitCost), 0) FROM PurchaseRequisitionDetails 
                            WHERE RequisitionMasterId = @RequisitionMasterId) 
                            WHERE RequisitionMasterId = @RequisitionMasterId";

                        using (SqlCommand cmdTotal = new SqlCommand(queryUpdateTotal, connection))
                        {
                            cmdTotal.Parameters.AddWithValue("@RequisitionMasterId", detalle.RequisitionMasterId);
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

        // MÉTODO PRINCIPAL: Mostrar detalles por requisición
        public static List<Mdl_PurchaseRequisitionDetails> MostrarDetallesPorRequisicion(int requisitionMasterId)
        {
            List<Mdl_PurchaseRequisitionDetails> lista = new List<Mdl_PurchaseRequisitionDetails>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM PurchaseRequisitionDetails WHERE RequisitionMasterId = @RequisitionMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionMasterId", requisitionMasterId);
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
        public static int EliminarDetalle(int requisitionDetailId, int requisitionMasterId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "DELETE FROM PurchaseRequisitionDetails WHERE RequisitionDetailId = @RequisitionDetailId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionDetailId", requisitionDetailId);
                        int result = cmd.ExecuteNonQuery();

                        // Actualizar total del master
                        string queryUpdateTotal = @"UPDATE PurchaseRequisitionMaster SET TotalBudget = 
                            (SELECT ISNULL(SUM(Quantity * UnitCost), 0) FROM PurchaseRequisitionDetails 
                            WHERE RequisitionMasterId = @RequisitionMasterId) 
                            WHERE RequisitionMasterId = @RequisitionMasterId";

                        using (SqlCommand cmdTotal = new SqlCommand(queryUpdateTotal, connection))
                        {
                            cmdTotal.Parameters.AddWithValue("@RequisitionMasterId", requisitionMasterId);
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
        private static Mdl_PurchaseRequisitionDetails MapearDetalle(SqlDataReader reader)
        {
            return new Mdl_PurchaseRequisitionDetails
            {
                RequisitionDetailId = reader.GetInt32(0),
                RequisitionMasterId = reader.GetInt32(1),
                RequestDetailId = reader[2] == DBNull.Value ? null : (int?)reader.GetInt32(2),
                ItemId = reader.GetInt32(3),
                SupplierId = reader.GetInt32(4),
                Quantity = reader.GetDecimal(5),
                UnitCost = reader.GetDecimal(6),
                TotalCost = reader.GetDecimal(7),
                PriorityId = reader.GetInt32(8),
                StatusId = reader.GetInt32(9),
                RequestReason = reader[10] == DBNull.Value ? null : reader[10].ToString()
            };
        }
    }
}