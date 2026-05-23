using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_FixedAssetAttributeDefinitions
    {
        public static List<Mdl_FixedAssetAttributeDefinition> MostrarAtributosPorCategoria(int categoryId)
        {
            List<Mdl_FixedAssetAttributeDefinition> lista = new List<Mdl_FixedAssetAttributeDefinition>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT AttributeDefId, AssetCategoryId, AttributeKey, AttributeLabel,
                                       DataType, IsRequired, IsActive, IsSystem
                                FROM   FixedAssetAttributeDefinitions
                                WHERE  AssetCategoryId = @CategoryId
                                ORDER  BY AttributeDefId, AttributeKey";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearAtributo(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener atributos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static int RegistrarAtributo(Mdl_FixedAssetAttributeDefinition attr)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetAttributeDefinitions_Insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AssetCategoryId", attr.AssetCategoryId);
                        cmd.Parameters.AddWithValue("@AttributeKey", attr.AttributeKey?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@AttributeLabel", attr.AttributeLabel?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@DataType", attr.DataType ?? "TEXT");
                        cmd.Parameters.AddWithValue("@IsRequired", attr.IsRequired);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar atributo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarAtributo(Mdl_FixedAssetAttributeDefinition attr)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetAttributeDefinitions_Update", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AttributeDefId", attr.AttributeDefId);
                        cmd.Parameters.AddWithValue("@AttributeKey", attr.AttributeKey?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@AttributeLabel", attr.AttributeLabel?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@DataType", attr.DataType ?? "TEXTO");
                        cmd.Parameters.AddWithValue("@IsRequired", attr.IsRequired);
                        cmd.Parameters.AddWithValue("@IsActive", attr.IsActive);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar atributo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private static Mdl_FixedAssetAttributeDefinition MapearAtributo(SqlDataReader reader)
        {
            return new Mdl_FixedAssetAttributeDefinition
            {
                AttributeDefId = reader.GetInt32(reader.GetOrdinal("AttributeDefId")),
                AssetCategoryId = reader.GetInt32(reader.GetOrdinal("AssetCategoryId")),
                AttributeKey = reader["AttributeKey"].ToString(),
                AttributeLabel = reader["AttributeLabel"].ToString(),
                DataType = reader["DataType"].ToString(),
                IsRequired = reader.GetBoolean(reader.GetOrdinal("IsRequired")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                IsSystem = reader.GetBoolean(reader.GetOrdinal("IsSystem"))
            };
        }
    }
}