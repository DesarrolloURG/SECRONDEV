using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_FixedAssetAttributeValues
    {
        public static List<Mdl_FixedAssetAttributeValue> ObtenerValoresPorActivo(int assetId)
        {
            List<Mdl_FixedAssetAttributeValue> lista = new List<Mdl_FixedAssetAttributeValue>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT av.AttributeValueId, av.AssetId, av.AttributeDefId, av.Value,
                               av.CreatedDate, av.CreatedBy, av.ModifiedDate, av.ModifiedBy,
                               ad.AttributeKey, ad.AttributeLabel, ad.DataType, ad.IsRequired
                        FROM   FixedAssetAttributeValues av
                        INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                        WHERE  av.AssetId = @AssetId
                        ORDER  BY ad.AttributeKey";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AssetId", assetId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearValor(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener valores del activo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static List<Mdl_FixedAssetAttributeValue> ObtenerPlantillaPorCategoria(int categoryId)
        {
            List<Mdl_FixedAssetAttributeValue> lista = new List<Mdl_FixedAssetAttributeValue>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT 0 AS AttributeValueId, 0 AS AssetId,
                               ad.AttributeDefId, NULL AS Value,
                               GETDATE() AS CreatedDate, NULL AS CreatedBy,
                               NULL AS ModifiedDate, NULL AS ModifiedBy,
                               ad.AttributeKey, ad.AttributeLabel, ad.DataType, ad.IsRequired
                        FROM   FixedAssetAttributeDefinitions ad
                        WHERE  ad.AssetCategoryId = @CategoryId
                        AND    ad.IsActive = 1
                        ORDER  BY ad.AttributeKey";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearValor(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener plantilla de atributos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static int RegistrarValor(Mdl_FixedAssetAttributeValue valor)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetAttributeValues_Insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AssetId", valor.AssetId);
                        cmd.Parameters.AddWithValue("@AttributeDefId", valor.AttributeDefId);
                        cmd.Parameters.AddWithValue("@Value", (object)valor.Value?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)valor.CreatedBy ?? DBNull.Value);
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar valor de atributo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarValor(Mdl_FixedAssetAttributeValue valor)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetAttributeValues_Update", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AttributeValueId", valor.AttributeValueId);
                        cmd.Parameters.AddWithValue("@Value", (object)valor.Value?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)valor.ModifiedBy ?? DBNull.Value);
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar valor de atributo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private static Mdl_FixedAssetAttributeValue MapearValor(SqlDataReader reader)
        {
            return new Mdl_FixedAssetAttributeValue
            {
                AttributeValueId = reader.GetInt32(reader.GetOrdinal("AttributeValueId")),
                AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")),
                AttributeDefId = reader.GetInt32(reader.GetOrdinal("AttributeDefId")),
                Value = reader["Value"] == DBNull.Value ? null : reader["Value"].ToString(),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ModifiedBy")),
                AttributeKey = reader["AttributeKey"].ToString(),
                AttributeLabel = reader["AttributeLabel"].ToString(),
                DataType = reader["DataType"].ToString(),
                IsRequired = reader.GetBoolean(reader.GetOrdinal("IsRequired"))
            };
        }
    }
}