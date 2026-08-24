using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_CourseLocationPricingDetail
    {
        #region Consultas

        // Historial completo de precios de una combinación específica
        public static List<Mdl_CourseLocationPricingDetail> MostrarHistorial(int courseLocationPricingId)
        {
            List<Mdl_CourseLocationPricingDetail> lista = new List<Mdl_CourseLocationPricingDetail>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT
                            CourseLocationPricingDetailId,
                            CourseLocationPricingId,
                            Price,
                            EffectiveFrom,
                            EffectiveTo,
                            IsActive,
                            CreatedDate,
                            CreatedBy,
                            ModifiedDate,
                            ModifiedBy
                        FROM CourseLocationPricingDetail
                        WHERE CourseLocationPricingId = @CourseLocationPricingId
                        ORDER BY EffectiveFrom DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CourseLocationPricingId", courseLocationPricingId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearDetalle(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER HISTORIAL DE PRECIOS: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        #endregion

        #region CRUD

        // Registra un nuevo precio (el SP cierra automáticamente el vigente anterior)
        public static int RegistrarPrecio(Mdl_CourseLocationPricingDetail detalle)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CourseLocationPricingDetail_Insert", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CourseLocationPricingId", detalle.CourseLocationPricingId);
                    cmd.Parameters.AddWithValue("@Price", detalle.Price);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object)detalle.CreatedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL REGISTRAR PRECIO: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Corrige el precio del registro vigente (SP valida que sea el vigente antes de actualizar)
        public static int ActualizarPrecioVigente(Mdl_CourseLocationPricingDetail detalle)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CourseLocationPricingDetail_Update", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CourseLocationPricingDetailId", detalle.CourseLocationPricingDetailId);
                    cmd.Parameters.AddWithValue("@Price", detalle.Price);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)detalle.ModifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR PRECIO VIGENTE: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Activa o inactiva un registro histórico (corrección de carga errónea)
        public static int CambiarEstadoPrecio(int courseLocationPricingDetailId, bool isActive, int? modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CourseLocationPricingDetail_Delete", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CourseLocationPricingDetailId", courseLocationPricingDetailId);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CAMBIAR ESTADO DEL PRECIO: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Métodos auxiliares

        private static Mdl_CourseLocationPricingDetail MapearDetalle(SqlDataReader reader)
        {
            return new Mdl_CourseLocationPricingDetail
            {
                CourseLocationPricingDetailId = Convert.ToInt32(reader["CourseLocationPricingDetailId"]),
                CourseLocationPricingId = Convert.ToInt32(reader["CourseLocationPricingId"]),
                Price = Convert.ToDecimal(reader["Price"]),
                EffectiveFrom = Convert.ToDateTime(reader["EffectiveFrom"]),
                EffectiveTo = reader["EffectiveTo"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["EffectiveTo"]),
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["CreatedDate"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CreatedBy"]),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ModifiedDate"]),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ModifiedBy"])
            };
        }

        #endregion
    }
}