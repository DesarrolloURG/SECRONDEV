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
    internal class Ctrl_PurchaseRequisitionMaster
    {
        // MÉTODO: Generar número de requisición
        public static string GenerarNumeroRequisicion()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT MAX(CAST(SUBSTRING(RequisitionNumber, 4, LEN(RequisitionNumber)) AS INT)) FROM PurchaseRequisitionMaster";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object result = cmd.ExecuteScalar();
                        int nextNumber = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
                        return "REQ" + nextNumber.ToString().PadLeft(6, '0');
                    }
                }
            }
            catch { return "REQ000001"; }
        }

        // MÉTODO PRINCIPAL: Registrar requisición (retorna ID)
        public static int RegistrarRequisicion(Mdl_PurchaseRequisitionMaster requisicion)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO PurchaseRequisitionMaster (RequisitionNumber, RequisitionDate, 
                        ResponsibleUserId, StatusId, TotalBudget, CreatedBy, IsActive) 
                        VALUES (@RequisitionNumber, @RequisitionDate, @ResponsibleUserId, @StatusId, 
                        @TotalBudget, @CreatedBy, @IsActive);
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionNumber", requisicion.RequisitionNumber ?? "");
                        cmd.Parameters.AddWithValue("@RequisitionDate", requisicion.RequisitionDate);
                        cmd.Parameters.AddWithValue("@ResponsibleUserId", requisicion.ResponsibleUserId);
                        cmd.Parameters.AddWithValue("@StatusId", requisicion.StatusId);
                        cmd.Parameters.AddWithValue("@TotalBudget", requisicion.TotalBudget);
                        cmd.Parameters.AddWithValue("@CreatedBy", requisicion.CreatedBy);
                        cmd.Parameters.AddWithValue("@IsActive", requisicion.IsActive);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar requisición: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar requisiciones
        public static List<Mdl_PurchaseRequisitionMaster> MostrarRequisiciones()
        {
            List<Mdl_PurchaseRequisitionMaster> lista = new List<Mdl_PurchaseRequisitionMaster>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM PurchaseRequisitionMaster WHERE IsActive = 1 ORDER BY RequisitionDate DESC";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearRequisicion(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener requisiciones: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Obtener por ID
        public static Mdl_PurchaseRequisitionMaster ObtenerRequisicionPorId(int requisitionMasterId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM PurchaseRequisitionMaster WHERE RequisitionMasterId = @RequisitionMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionMasterId", requisitionMasterId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearRequisicion(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener requisición: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Actualizar total
        public static int ActualizarTotal(int requisitionMasterId, decimal totalBudget)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE PurchaseRequisitionMaster SET TotalBudget = @TotalBudget WHERE RequisitionMasterId = @RequisitionMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionMasterId", requisitionMasterId);
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
        public static int CambiarEstado(int requisitionMasterId, int statusId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE PurchaseRequisitionMaster SET StatusId = @StatusId, 
                        ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy 
                        WHERE RequisitionMasterId = @RequisitionMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RequisitionMasterId", requisitionMasterId);
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

        // MÉTODO AUXILIAR: Mapear requisición
        private static Mdl_PurchaseRequisitionMaster MapearRequisicion(SqlDataReader reader)
        {
            return new Mdl_PurchaseRequisitionMaster
            {
                RequisitionMasterId = reader.GetInt32(0),
                RequisitionNumber = reader[1].ToString(),
                RequisitionDate = reader.GetDateTime(2),
                ResponsibleUserId = reader.GetInt32(3),
                StatusId = reader.GetInt32(4),
                TotalBudget = reader.GetDecimal(5),
                CreatedDate = reader.GetDateTime(6),
                CreatedBy = reader.GetInt32(7),
                ModifiedDate = reader[8] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(8),
                ModifiedBy = reader[9] == DBNull.Value ? null : (int?)reader.GetInt32(9),
                IsActive = reader.GetBoolean(10)
            };
        }
    }
}