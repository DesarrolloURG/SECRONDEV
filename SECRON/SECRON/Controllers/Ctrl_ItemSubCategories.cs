using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_ItemSubCategories
    {
        public static List<Mdl_ItemSubCategories> MostrarSubCategorias(int categoryId, string textoBusqueda = "", string buscarPor = "TODOS")
        {
            List<Mdl_ItemSubCategories> lista = new List<Mdl_ItemSubCategories>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT SubCategoryId, CategoryId, SubCategoryCode, SubCategoryName,
                               IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy
                        FROM   ItemSubCategories
                        WHERE  CategoryId = @CategoryId
                          AND  IsActive = 1";

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        switch (buscarPor)
                        {
                            case "CÓDIGO":
                                query += " AND SubCategoryCode LIKE @texto";
                                break;
                            case "NOMBRE":
                                query += " AND SubCategoryName LIKE @texto";
                                break;
                            default:
                                query += " AND (SubCategoryCode LIKE @texto OR SubCategoryName LIKE @texto)";
                                break;
                        }
                    }

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

        public static int RegistrarSubCategoria(Mdl_ItemSubCategories sub)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        IF EXISTS (SELECT 1 FROM ItemSubCategories 
                                   WHERE CategoryId = @CategoryId AND SubCategoryCode = @SubCategoryCode)
                            SELECT -1
                        ELSE
                        BEGIN
                            INSERT INTO ItemSubCategories
                                (CategoryId, SubCategoryCode, SubCategoryName, IsActive, CreatedBy)
                            VALUES
                                (@CategoryId, @SubCategoryCode, @SubCategoryName, 1, @CreatedBy)
                            SELECT 1
                        END";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", sub.CategoryId);
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

        public static int ActualizarSubCategoria(Mdl_ItemSubCategories sub)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                IF NOT EXISTS (SELECT 1 FROM ItemSubCategories 
                               WHERE SubCategoryId = @SubCategoryId)
                    SELECT -2
                ELSE
                BEGIN
                    UPDATE ItemSubCategories SET
                        SubCategoryName = @SubCategoryName,
                        IsActive        = @IsActive,
                        ModifiedDate    = GETDATE(),
                        ModifiedBy      = @ModifiedBy
                    WHERE SubCategoryId = @SubCategoryId
                    SELECT 1
                END";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@SubCategoryId", sub.SubCategoryId);
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

        public static int InactivarSubCategoria(int subCategoryId, int? modifiedBy = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        UPDATE ItemSubCategories SET
                            IsActive     = 0,
                            ModifiedDate = GETDATE(),
                            ModifiedBy   = @ModifiedBy
                        WHERE SubCategoryId = @SubCategoryId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@SubCategoryId", subCategoryId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar subcategoría: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private static Mdl_ItemSubCategories MapearSubCategoria(SqlDataReader reader)
        {
            return new Mdl_ItemSubCategories
            {
                SubCategoryId = reader.GetInt32(reader.GetOrdinal("SubCategoryId")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
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