// ============================================================
// Ctrl_FixedAssetClassificationCategories.cs
// ============================================================
using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_FixedAssetClassificationCategories
    {
        
        public static List<Mdl_FixedAssetClassificationCategory> MostrarClasificaciones(
            string classificationCode = null,
            string classificationName = null,
            bool? isActive = null)
        {
            var lista = new List<Mdl_FixedAssetClassificationCategory>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "SP_FixedAssetClassificationCategories_Select", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ClassificationCode",
                            (object)classificationCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ClassificationName",
                            (object)classificationName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive",
                            isActive.HasValue ? (object)isActive.Value : DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(Mapear(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener clasificaciones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static List<KeyValuePair<int, string>> ObtenerClasificacionesParaCombo(
            bool soloActivas = true)
        {
            var lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "SP_FixedAssetClassificationCategories_Select", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ClassificationCode", DBNull.Value);
                        cmd.Parameters.AddWithValue("@ClassificationName", DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive",
                            soloActivas ? (object)true : DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(new KeyValuePair<int, string>(
                                    reader.GetInt32(reader.GetOrdinal("ClassificationId")),
                                    reader["ClassificationName"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener clasificaciones para combo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static int RegistrarClasificacion(
            Mdl_FixedAssetClassificationCategory clasificacion)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "SP_FixedAssetClassificationCategories_Insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ClassificationCode",
                            clasificacion.ClassificationCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@ClassificationName",
                            clasificacion.ClassificationName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@Description",
                            (object)clasificacion.Description?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy",
                            (object)clasificacion.CreatedBy ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar clasificación: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarClasificacion(
            Mdl_FixedAssetClassificationCategory clasificacion)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "SP_FixedAssetClassificationCategories_Update", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ClassificationId",
                            clasificacion.ClassificationId);
                        cmd.Parameters.AddWithValue("@ClassificationCode",
                            clasificacion.ClassificationCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@ClassificationName",
                            clasificacion.ClassificationName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@Description",
                            (object)clasificacion.Description?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive",
                            clasificacion.IsActive);
                        cmd.Parameters.AddWithValue("@ModifiedBy",
                            (object)clasificacion.ModifiedBy ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar clasificación: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int InactivarClasificacion(int classificationId, int? modifiedBy = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "SP_FixedAssetClassificationCategories_Inactive", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ClassificationId", classificationId);
                        cmd.Parameters.AddWithValue("@ModifiedBy",
                            (object)modifiedBy ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar clasificación: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        
        private static Mdl_FixedAssetClassificationCategory Mapear(SqlDataReader reader)
        {
            return new Mdl_FixedAssetClassificationCategory
            {
                ClassificationId = reader.GetInt32(reader.GetOrdinal("ClassificationId")),
                ClassificationCode = reader["ClassificationCode"].ToString(),
                ClassificationName = reader["ClassificationName"].ToString(),
                Description = reader["Description"] == DBNull.Value
                                        ? null : reader["Description"].ToString(),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader["CreatedDate"] == DBNull.Value
                                        ? (DateTime?)null
                                        : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value
                                        ? (int?)null
                                        : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value
                                        ? (DateTime?)null
                                        : reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value
                                        ? (int?)null
                                        : reader.GetInt32(reader.GetOrdinal("ModifiedBy"))
            };
        }
    }
}