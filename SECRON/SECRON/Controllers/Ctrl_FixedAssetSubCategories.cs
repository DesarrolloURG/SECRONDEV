using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_FixedAssetSubCategories
    {
        public static List<Mdl_FixedAssetSubCategory> MostrarSubCategorias(int categoryId, string textoBusqueda = "", string filtroEstado = "SOLO ACTIVOS")
        {
            List<Mdl_FixedAssetSubCategory> lista = new List<Mdl_FixedAssetSubCategory>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT SubCategoryId, AssetCategoryId, SubCategoryCode,
                               SubCategoryName, IsActive, CreatedDate, CreatedBy,
                               ModifiedDate, ModifiedBy
                        FROM   FixedAssetSubCategories
                        WHERE  AssetCategoryId = @CategoryId";

                    if (filtroEstado == "SOLO ACTIVOS")
                        query += " AND IsActive = 1";
                    else if (filtroEstado == "SOLO INACTIVOS")
                        query += " AND IsActive = 0";

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                        query += " AND (SubCategoryCode LIKE @texto OR SubCategoryName LIKE @texto)";

                    query += " ORDER BY SubCategoryCode";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                        if (!string.IsNullOrWhiteSpace(textoBusqueda))
                            cmd.Parameters.AddWithValue("@texto", "%" + textoBusqueda.Trim() + "%");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearSubCategoria(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener subcategorías: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static int RegistrarSubCategoria(Mdl_FixedAssetSubCategory sub)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetSubCategories_Insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AssetCategoryId", sub.AssetCategoryId);
                        cmd.Parameters.AddWithValue("@SubCategoryCode", sub.SubCategoryCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@SubCategoryName", sub.SubCategoryName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)sub.CreatedBy ?? DBNull.Value);
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar subcategoría: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarSubCategoria(Mdl_FixedAssetSubCategory sub)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetSubCategories_Update", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SubCategoryId", sub.SubCategoryId);
                        cmd.Parameters.AddWithValue("@SubCategoryCode", sub.SubCategoryCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@SubCategoryName", sub.SubCategoryName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@IsActive", sub.IsActive);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)sub.ModifiedBy ?? DBNull.Value);
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar subcategoría: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private static Mdl_FixedAssetSubCategory MapearSubCategoria(SqlDataReader reader)
        {
            return new Mdl_FixedAssetSubCategory
            {
                SubCategoryId = reader.GetInt32(reader.GetOrdinal("SubCategoryId")),
                AssetCategoryId = reader.GetInt32(reader.GetOrdinal("AssetCategoryId")),
                SubCategoryCode = reader["SubCategoryCode"].ToString(),
                SubCategoryName = reader["SubCategoryName"].ToString(),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ModifiedBy"))
            };
        }
    }
}