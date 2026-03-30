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
    internal class Ctrl_PurchaseOrderMaster
    {
        // MÉTODO: Generar número de orden
        public static string GenerarNumeroOrden()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT MAX(CAST(SUBSTRING(OrderNumber, 3, LEN(OrderNumber)) AS INT)) FROM PurchaseOrderMaster";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object result = cmd.ExecuteScalar();
                        int nextNumber = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
                        return "OC" + nextNumber.ToString().PadLeft(6, '0');
                    }
                }
            }
            catch { return "OC000001"; }
        }

        // MÉTODO PRINCIPAL: Registrar orden (retorna ID)
        public static int RegistrarOrden(Mdl_PurchaseOrderMaster orden)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO PurchaseOrderMaster (OrderNumber, OrderDate, RequisitionMasterId, 
                        SupplierId, DeliveryLocationId, ExpectedDeliveryDate, TotalAmount, StatusId, 
                        CreatedBy, IsActive) 
                        VALUES (@OrderNumber, @OrderDate, @RequisitionMasterId, @SupplierId, 
                        @DeliveryLocationId, @ExpectedDeliveryDate, @TotalAmount, @StatusId, @CreatedBy, @IsActive);
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@OrderNumber", orden.OrderNumber ?? "");
                        cmd.Parameters.AddWithValue("@OrderDate", orden.OrderDate);
                        cmd.Parameters.AddWithValue("@RequisitionMasterId", orden.RequisitionMasterId);
                        cmd.Parameters.AddWithValue("@SupplierId", orden.SupplierId);
                        cmd.Parameters.AddWithValue("@DeliveryLocationId", orden.DeliveryLocationId);
                        cmd.Parameters.AddWithValue("@ExpectedDeliveryDate", (object)orden.ExpectedDeliveryDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TotalAmount", orden.TotalAmount);
                        cmd.Parameters.AddWithValue("@StatusId", orden.StatusId);
                        cmd.Parameters.AddWithValue("@CreatedBy", orden.CreatedBy);
                        cmd.Parameters.AddWithValue("@IsActive", orden.IsActive);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar orden: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar órdenes
        public static List<Mdl_PurchaseOrderMaster> MostrarOrdenes()
        {
            List<Mdl_PurchaseOrderMaster> lista = new List<Mdl_PurchaseOrderMaster>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM PurchaseOrderMaster WHERE IsActive = 1 ORDER BY OrderDate DESC";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearOrden(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener órdenes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Obtener por ID
        public static Mdl_PurchaseOrderMaster ObtenerOrdenPorId(int purchaseOrderId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM PurchaseOrderMaster WHERE PurchaseOrderId = @PurchaseOrderId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseOrderId", purchaseOrderId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearOrden(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener orden: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Aprobar orden
        public static int AprobarOrden(int purchaseOrderId, int approvedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE PurchaseOrderMaster SET ApprovedDate = GETDATE(), 
                        ApprovedBy = @ApprovedBy WHERE PurchaseOrderId = @PurchaseOrderId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseOrderId", purchaseOrderId);
                        cmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al aprobar orden: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear orden
        private static Mdl_PurchaseOrderMaster MapearOrden(SqlDataReader reader)
        {
            return new Mdl_PurchaseOrderMaster
            {
                PurchaseOrderId = reader.GetInt32(0),
                OrderNumber = reader[1].ToString(),
                OrderDate = reader.GetDateTime(2),
                RequisitionMasterId = reader.GetInt32(3),
                SupplierId = reader.GetInt32(4),
                DeliveryLocationId = reader.GetInt32(5),
                ExpectedDeliveryDate = reader[6] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(6),
                TotalAmount = reader.GetDecimal(7),
                StatusId = reader.GetInt32(8),
                CreatedDate = reader.GetDateTime(9),
                CreatedBy = reader.GetInt32(10),
                ApprovedDate = reader[11] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(11),
                ApprovedBy = reader[12] == DBNull.Value ? null : (int?)reader.GetInt32(12),
                ModifiedDate = reader[13] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(13),
                ModifiedBy = reader[14] == DBNull.Value ? null : (int?)reader.GetInt32(14),
                IsActive = reader.GetBoolean(15)
            };
        }
    }
}