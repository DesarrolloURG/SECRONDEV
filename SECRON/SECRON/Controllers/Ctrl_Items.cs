using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_Items
    {
        // MÉTODO PRINCIPAL: Registrar artículo
        public static int RegistrarArticulo(Mdl_Items item)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Items 
                        (ItemCode, ItemName, Description, CategoryId, SubCategoryId, UnitId, 
                         MinimumStock, MaximumStock, ReorderPoint, UnitCost, LastPurchasePrice, 
                         HasLotControl, HasExpiryDate, IsActive, CreatedBy) 
                        VALUES 
                        (@ItemCode, @ItemName, @Description, @CategoryId, @SubCategoryId, @UnitId, 
                         @MinimumStock, @MaximumStock, @ReorderPoint, @UnitCost, @LastPurchasePrice, 
                         @HasLotControl, @HasExpiryDate, @IsActive, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemCode", item.ItemCode ?? "");
                        cmd.Parameters.AddWithValue("@ItemName", item.ItemName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
                        cmd.Parameters.AddWithValue("@SubCategoryId", item.SubCategoryId);
                        cmd.Parameters.AddWithValue("@UnitId", item.UnitId);
                        cmd.Parameters.AddWithValue("@MinimumStock", item.MinimumStock);
                        cmd.Parameters.AddWithValue("@MaximumStock", (object)item.MaximumStock);
                        cmd.Parameters.AddWithValue("@ReorderPoint", (object)item.ReorderPoint);
                        cmd.Parameters.AddWithValue("@UnitCost", item.UnitCost);
                        cmd.Parameters.AddWithValue("@LastPurchasePrice", (object)item.LastPurchasePrice);
                        cmd.Parameters.AddWithValue("@HasLotControl", item.HasLotControl);
                        cmd.Parameters.AddWithValue("@HasExpiryDate", item.HasExpiryDate);
                        cmd.Parameters.AddWithValue("@IsActive", item.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)item.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar artículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static List<Mdl_Items> MostrarArticulos(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_Items> lista = new List<Mdl_Items>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                SELECT i.ItemId, i.ItemCode, i.ItemName, i.Description,
                       i.CategoryId, c.CategoryName,
                       i.SubCategoryId, s.SubCategoryName,
                       i.UnitId, u.UnitName,
                       i.MinimumStock, i.MaximumStock, i.ReorderPoint,
                       i.UnitCost, i.LastPurchasePrice,
                       i.HasLotControl, i.HasExpiryDate, i.IsActive,
                       i.CreatedDate, i.CreatedBy, i.ModifiedDate, i.ModifiedBy
                FROM   Items i
                INNER JOIN ItemCategories    c ON i.CategoryId    = c.CategoryId
                LEFT  JOIN ItemSubCategories s ON i.SubCategoryId = s.SubCategoryId
                INNER JOIN MeasurementUnits  u ON i.UnitId        = u.UnitId
                WHERE  i.IsActive = 1
                ORDER  BY i.ItemName
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearArticulo(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener artículos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Búsqueda con filtros
        public static List<Mdl_Items> BuscarArticulos(
     string textoBusqueda = "",
     int? categoryId = null,
     string filtro1 = "TODOS",
     string filtro3 = "TODOS",
     int pageNumber = 1,
     int pageSize = 100)
        {
            List<Mdl_Items> lista = new List<Mdl_Items>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                SELECT i.ItemId, i.ItemCode, i.ItemName, i.Description,
                       i.CategoryId, c.CategoryName,
                       i.SubCategoryId, s.SubCategoryName,
                       i.UnitId, u.UnitName,
                       i.MinimumStock, i.MaximumStock, i.ReorderPoint,
                       i.UnitCost, i.LastPurchasePrice,
                       i.HasLotControl, i.HasExpiryDate, i.IsActive,
                       i.CreatedDate, i.CreatedBy, i.ModifiedDate, i.ModifiedBy
                FROM   Items i
                INNER JOIN ItemCategories    c ON i.CategoryId    = c.CategoryId
                LEFT  JOIN ItemSubCategories s ON i.SubCategoryId = s.SubCategoryId
                INNER JOIN MeasurementUnits  u ON i.UnitId        = u.UnitId
                WHERE 1=1";

                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (filtro3 == "SOLO ACTIVOS")
                        query += " AND i.IsActive = 1";
                    else if (filtro3 == "SOLO INACTIVOS")
                        query += " AND i.IsActive = 0";

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        if (filtro1 == "POR CÓDIGO")
                            query += " AND i.ItemCode LIKE @texto";
                        else if (filtro1 == "POR NOMBRE")
                            query += " AND i.ItemName LIKE @texto";
                        else if (filtro1 == "POR DESCRIPCIÓN")
                            query += " AND i.Description LIKE @texto";
                        else
                            query += " AND (i.ItemCode LIKE @texto OR i.ItemName LIKE @texto OR i.Description LIKE @texto)";

                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (categoryId.HasValue && categoryId > 0)
                    {
                        query += " AND i.CategoryId = @categoryId";
                        parametros.Add(new SqlParameter("@categoryId", categoryId.Value));
                    }

                    query += " ORDER BY i.ItemName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearArticulo(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en búsqueda: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Actualizar artículo
        public static int ActualizarArticulo(Mdl_Items item)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Items SET
                        ItemCode          = @ItemCode,
                        ItemName          = @ItemName,
                        Description       = @Description,
                        CategoryId        = @CategoryId,
                        SubCategoryId     = @SubCategoryId,
                        UnitId            = @UnitId,
                        MinimumStock      = @MinimumStock,
                        MaximumStock      = @MaximumStock,
                        ReorderPoint      = @ReorderPoint,
                        UnitCost          = @UnitCost,
                        LastPurchasePrice = @LastPurchasePrice,
                        HasLotControl     = @HasLotControl,
                        HasExpiryDate     = @HasExpiryDate,
                        ModifiedDate      = GETDATE(),
                        ModifiedBy        = @ModifiedBy
                        WHERE ItemId = @ItemId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", item.ItemId);
                        cmd.Parameters.AddWithValue("@ItemCode", item.ItemCode ?? "");
                        cmd.Parameters.AddWithValue("@ItemName", item.ItemName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
                        cmd.Parameters.AddWithValue("@SubCategoryId", item.SubCategoryId);
                        cmd.Parameters.AddWithValue("@UnitId", item.UnitId);
                        cmd.Parameters.AddWithValue("@MinimumStock", item.MinimumStock);
                        cmd.Parameters.AddWithValue("@MaximumStock", (object)item.MaximumStock);
                        cmd.Parameters.AddWithValue("@ReorderPoint", (object)item.ReorderPoint);
                        cmd.Parameters.AddWithValue("@UnitCost", item.UnitCost);
                        cmd.Parameters.AddWithValue("@LastPurchasePrice", (object)item.LastPurchasePrice);
                        cmd.Parameters.AddWithValue("@HasLotControl", item.HasLotControl);
                        cmd.Parameters.AddWithValue("@HasExpiryDate", item.HasExpiryDate);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)item.ModifiedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar artículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO: Obtener artículo por ID
        public static Mdl_Items ObtenerArticuloPorId(int itemId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT i.ItemId, i.ItemCode, i.ItemName, i.Description,
                               i.CategoryId, c.CategoryName,
                               i.SubCategoryId, s.SubCategoryName,
                               i.UnitId, u.UnitName,
                               i.MinimumStock, i.MaximumStock, i.ReorderPoint,
                               i.UnitCost, i.LastPurchasePrice,
                               i.HasLotControl, i.HasExpiryDate, i.IsActive,
                               i.CreatedDate, i.CreatedBy, i.ModifiedDate, i.ModifiedBy
                        FROM   Items i
                        INNER JOIN ItemCategories     c ON i.CategoryId    = c.CategoryId
                        INNER JOIN ItemSubCategories  s ON i.SubCategoryId = s.SubCategoryId
                        INNER JOIN MeasurementUnits   u ON i.UnitId        = u.UnitId
                        WHERE  i.ItemId = @ItemId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapearArticulo(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener artículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PARA OBTENER ARTÍCULOS PARA COMBOBOX
        public static List<KeyValuePair<int, string>> ObtenerArticulosParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT ItemId, ItemName FROM Items WHERE IsActive = 1 ORDER BY ItemName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener artículos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA CONTAR TOTAL
        public static int ContarTotalArticulos(string textoBusqueda = "", int? categoryId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Items WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += " AND (ItemCode LIKE @texto OR ItemName LIKE @texto OR Description LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (categoryId.HasValue && categoryId > 0)
                    {
                        query += " AND CategoryId = @categoryId";
                        parametros.Add(new SqlParameter("@categoryId", categoryId.Value));
                    }

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch { return 0; }
        }

        // MÉTODO: Generar próximo código con formato CategoriaCode-SubCategoriaCode-000001
        public static string ObtenerProximoCodigoArticulo(string categoryCode, string subCategoryCode)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string prefix = $"{categoryCode}-{subCategoryCode}-";

                    string query = @"
                        SELECT ISNULL(MAX(TRY_CAST(SUBSTRING(ItemCode, LEN(@prefix) + 1, LEN(ItemCode)) AS INT)), 0) + 1
                        FROM Items
                        WHERE ItemCode LIKE @prefixLike";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@prefix", prefix);
                        cmd.Parameters.AddWithValue("@prefixLike", prefix + "%");

                        int correlativo = (int)cmd.ExecuteScalar();
                        return $"{prefix}{correlativo:D6}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de artículo: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "ERROR";
            }
        }

        // MÉTODO AUXILIAR: Mapear artículo
        private static Mdl_Items MapearArticulo(SqlDataReader reader)
        {
            return new Mdl_Items
            {
                ItemId = reader.GetInt32(reader.GetOrdinal("ItemId")),
                ItemCode = reader["ItemCode"].ToString(),
                ItemName = reader["ItemName"].ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                CategoryName = reader["CategoryName"] == DBNull.Value ? null : reader["CategoryName"].ToString(),
                SubCategoryId = reader["SubCategoryId"] == DBNull.Value ? 0 : reader.GetInt32(reader.GetOrdinal("SubCategoryId")),
                SubCategoryName = reader["SubCategoryName"] == DBNull.Value ? null : reader["SubCategoryName"].ToString(),
                UnitId = reader.GetInt32(reader.GetOrdinal("UnitId")),
                UnitName = reader["UnitName"] == DBNull.Value ? null : reader["UnitName"].ToString(),
                MinimumStock = reader.GetDecimal(reader.GetOrdinal("MinimumStock")),
                MaximumStock = reader["MaximumStock"] == DBNull.Value ? 0 : reader.GetDecimal(reader.GetOrdinal("MaximumStock")),
                ReorderPoint = reader["ReorderPoint"] == DBNull.Value ? 0 : reader.GetDecimal(reader.GetOrdinal("ReorderPoint")),
                UnitCost = reader.GetDecimal(reader.GetOrdinal("UnitCost")),
                LastPurchasePrice = reader["LastPurchasePrice"] == DBNull.Value ? 0 : reader.GetDecimal(reader.GetOrdinal("LastPurchasePrice")),
                HasLotControl = reader.GetBoolean(reader.GetOrdinal("HasLotControl")),
                HasExpiryDate = reader.GetBoolean(reader.GetOrdinal("HasExpiryDate")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ModifiedBy"))
            };
        }

        public static int InactivarArticulo(int itemId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Items SET
                IsActive     = 0,
                ModifiedDate = GETDATE(),
                ModifiedBy   = @ModifiedBy
            WHERE ItemId = @ItemId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar artículo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
        public static int ImportarArticulo(Mdl_Items item, out string itemCodeGenerado)
        {
            itemCodeGenerado = "";
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Items_ImportUpsert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ItemName", item.ItemName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
                        cmd.Parameters.AddWithValue("@SubCategoryId", item.SubCategoryId > 0
                        ? (object)item.SubCategoryId : DBNull.Value);
                        cmd.Parameters.AddWithValue("@UnitId", item.UnitId);
                        cmd.Parameters.AddWithValue("@MinimumStock", item.MinimumStock);
                        cmd.Parameters.AddWithValue("@MaximumStock", (object)item.MaximumStock ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReorderPoint", (object)item.ReorderPoint ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UnitCost", (object)item.UnitCost ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LastPurchasePrice", (object)item.LastPurchasePrice ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HasLotControl", item.HasLotControl);
                        cmd.Parameters.AddWithValue("@HasExpiryDate", item.HasExpiryDate);
                        cmd.Parameters.AddWithValue("@CreatedBy", item.CreatedBy > 0 ? (object)item.CreatedBy : DBNull.Value);

                        SqlParameter outputCode = new SqlParameter("@ItemCode", SqlDbType.NVarChar, 50);
                        outputCode.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputCode);

                        SqlParameter returnValue = new SqlParameter();
                        returnValue.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(returnValue);

                        cmd.ExecuteNonQuery();

                        itemCodeGenerado = outputCode.Value?.ToString() ?? "";
                        return Convert.ToInt32(returnValue.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al importar artículo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}