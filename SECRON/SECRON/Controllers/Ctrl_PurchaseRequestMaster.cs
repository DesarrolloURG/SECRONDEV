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
    internal class Ctrl_PurchaseRequestMaster
    {
        // MÉTODO: Generar número de solicitud
        public static string GenerarNumeroSolicitud()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT MAX(CAST(SUBSTRING(RequestNumber, 4, LEN(RequestNumber)) AS INT)) FROM PurchaseRequestMaster";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object result = cmd.ExecuteScalar();
                        int nextNumber = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
                        return "SOL" + nextNumber.ToString().PadLeft(6, '0');
                    }
                }
            }
            catch { return "SOL000001"; }
        }

        // MÉTODO PRINCIPAL: Registrar solicitud (retorna ID)
        public static int RegistrarSolicitud(Mdl_PurchaseRequestMaster solicitud)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO PurchaseRequestMaster (RequestNumber, RequestDate, ResponsibleUserId, 
                        StatusId, LocationId, DepartmentId, TotalBudget, CreatedBy, IsActive) 
                        VALUES (@RequestNumber, @RequestDate, @ResponsibleUserId, @StatusId, @LocationId, 
                        @DepartmentId, @TotalBudget, @CreatedBy, @IsActive);
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequestNumber", solicitud.RequestNumber ?? "");
                        cmd.Parameters.AddWithValue("@RequestDate", solicitud.RequestDate);
                        cmd.Parameters.AddWithValue("@ResponsibleUserId", solicitud.ResponsibleUserId);
                        cmd.Parameters.AddWithValue("@StatusId", solicitud.StatusId);
                        cmd.Parameters.AddWithValue("@LocationId", solicitud.LocationId);
                        cmd.Parameters.AddWithValue("@DepartmentId", solicitud.DepartmentId);
                        cmd.Parameters.AddWithValue("@TotalBudget", solicitud.TotalBudget);
                        cmd.Parameters.AddWithValue("@CreatedBy", solicitud.CreatedBy);
                        cmd.Parameters.AddWithValue("@IsActive", solicitud.IsActive);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar solicitud: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar solicitudes
        public static List<Mdl_PurchaseRequestMaster> MostrarSolicitudes()
        {
            List<Mdl_PurchaseRequestMaster> lista = new List<Mdl_PurchaseRequestMaster>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM PurchaseRequestMaster WHERE IsActive = 1 ORDER BY RequestDate DESC";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearSolicitud(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener solicitudes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Obtener por ID
        public static Mdl_PurchaseRequestMaster ObtenerSolicitudPorId(int requestMasterId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM PurchaseRequestMaster WHERE RequestMasterId = @RequestMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequestMasterId", requestMasterId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearSolicitud(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener solicitud: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Actualizar total
        public static int ActualizarTotal(int requestMasterId, decimal totalBudget)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE PurchaseRequestMaster SET TotalBudget = @TotalBudget WHERE RequestMasterId = @RequestMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequestMasterId", requestMasterId);
                        cmd.Parameters.AddWithValue("@TotalBudget", totalBudget);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar total: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Cambiar estado
        public static int CambiarEstado(int requestMasterId, int statusId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE PurchaseRequestMaster SET StatusId = @StatusId, 
                        ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy 
                        WHERE RequestMasterId = @RequestMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequestMasterId", requestMasterId);
                        cmd.Parameters.AddWithValue("@StatusId", statusId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar estado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear solicitud
        private static Mdl_PurchaseRequestMaster MapearSolicitud(SqlDataReader reader)
        {
            return new Mdl_PurchaseRequestMaster
            {
                RequestMasterId = reader.GetInt32(0),
                RequestNumber = reader[1].ToString(),
                RequestDate = reader.GetDateTime(2),
                ResponsibleUserId = reader.GetInt32(3),
                StatusId = reader.GetInt32(4),
                LocationId = reader.GetInt32(5),
                DepartmentId = reader.GetInt32(6),
                TotalBudget = reader.GetDecimal(7),
                CreatedDate = reader.GetDateTime(8),
                CreatedBy = reader.GetInt32(9),
                ModifiedDate = reader[10] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(10),
                ModifiedBy = reader[11] == DBNull.Value ? null : (int?)reader.GetInt32(11),
                IsActive = reader.GetBoolean(12)
            };
        }
    }
}