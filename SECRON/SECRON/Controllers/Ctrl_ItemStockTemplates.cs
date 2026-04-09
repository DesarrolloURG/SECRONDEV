using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Controllers
{
    internal class Ctrl_ItemStockTemplates
    {
        // MÉTODO PRINCIPAL: Registrar plantilla de stock
        public static int RegistrarPlantilla(Mdl_ItemStockTemplates plantilla)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                IF EXISTS (SELECT 1 FROM ItemStockTemplates 
                           WHERE LocationCategoryId = @LocationCategoryId AND ItemId = @ItemId)
                    UPDATE ItemStockTemplates 
                    SET MinimumStock = @MinimumStock, MaximumStock = @MaximumStock,
                        ReorderPoint = @ReorderPoint, IsActive = 1,
                        ModifiedDate = GETDATE(), ModifiedBy = @CreatedBy
                    WHERE LocationCategoryId = @LocationCategoryId AND ItemId = @ItemId
                ELSE
                    INSERT INTO ItemStockTemplates 
                        (LocationCategoryId, ItemId, MinimumStock, MaximumStock, ReorderPoint, IsActive, CreatedBy)
                    VALUES 
                        (@LocationCategoryId, @ItemId, @MinimumStock, @MaximumStock, @ReorderPoint, 1, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationCategoryId", plantilla.LocationCategoryId);
                        cmd.Parameters.AddWithValue("@ItemId", plantilla.ItemId);
                        cmd.Parameters.AddWithValue("@MinimumStock", plantilla.MinimumStock);
                        cmd.Parameters.AddWithValue("@MaximumStock", plantilla.MaximumStock);
                        cmd.Parameters.AddWithValue("@ReorderPoint", (object)plantilla.ReorderPoint ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)plantilla.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar plantilla: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar plantillas por categoría de sede
        public static List<Mdl_ItemStockTemplates> MostrarPlantillasPorCategoria(int locationCategoryId)
        {
            List<Mdl_ItemStockTemplates> lista = new List<Mdl_ItemStockTemplates>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // Se resuelven los campos de Item para mostrar en grilla
                    string query = @"SELECT t.TemplateId, t.LocationCategoryId, t.ItemId, 
                        t.MinimumStock, t.MaximumStock, t.ReorderPoint, t.IsActive, 
                        t.CreatedDate, t.CreatedBy, t.ModifiedDate, t.ModifiedBy,
                        lc.CategoryName, i.ItemCode, i.ItemName
                        FROM ItemStockTemplates t
                        INNER JOIN LocationCategories lc ON t.LocationCategoryId = lc.LocationCategoryId
                        INNER JOIN Items i ON t.ItemId = i.ItemId
                        WHERE t.LocationCategoryId = @LocationCategoryId AND t.IsActive = 1
                        ORDER BY i.ItemName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationCategoryId", locationCategoryId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearPlantilla(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener plantillas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Buscar plantillas con filtro y paginación
        public static List<Mdl_ItemStockTemplates> BuscarPlantillas(
            int locationCategoryId,
            string textoBusqueda = "",
            string filtro1 = "TODOS",
            int pageNumber = 1,
            int pageSize = 100
            )
        {
            List<Mdl_ItemStockTemplates> lista = new List<Mdl_ItemStockTemplates>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT t.TemplateId, t.LocationCategoryId, t.ItemId, 
                        t.MinimumStock, t.MaximumStock, t.ReorderPoint, t.IsActive,
                        t.CreatedDate, t.CreatedBy, t.ModifiedDate, t.ModifiedBy,
                        lc.CategoryName, i.ItemCode, i.ItemName
                        FROM ItemStockTemplates t
                        INNER JOIN LocationCategories lc ON t.LocationCategoryId = lc.LocationCategoryId
                        INNER JOIN Items i ON t.ItemId = i.ItemId
                        WHERE t.LocationCategoryId = @LocationCategoryId AND t.IsActive = 1";

                    List<SqlParameter> parametros = new List<SqlParameter>();
                    parametros.Add(new SqlParameter("@LocationCategoryId", locationCategoryId));

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        if (filtro1 == "POR CÓDIGO")
                            query += " AND i.ItemCode LIKE @texto";
                        else if (filtro1 == "POR NOMBRE")
                            query += " AND i.ItemName LIKE @texto";
                        else
                            query += " AND (i.ItemCode LIKE @texto OR i.ItemName LIKE @texto)";

                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
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
                            {
                                lista.Add(MapearPlantilla(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en búsqueda de plantillas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Actualizar plantilla de stock
        public static int ActualizarPlantilla(Mdl_ItemStockTemplates plantilla)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE ItemStockTemplates SET 
                        MinimumStock = @MinimumStock, MaximumStock = @MaximumStock, 
                        ReorderPoint = @ReorderPoint, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
                        WHERE TemplateId = @TemplateId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TemplateId", plantilla.TemplateId);
                        cmd.Parameters.AddWithValue("@MinimumStock", plantilla.MinimumStock);
                        cmd.Parameters.AddWithValue("@MaximumStock", plantilla.MaximumStock);
                        cmd.Parameters.AddWithValue("@ReorderPoint", (object)plantilla.ReorderPoint ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)plantilla.ModifiedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar plantilla: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar plantilla (quitar artículo de categoría)
        public static int InactivarPlantilla(int templateId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE ItemStockTemplates SET IsActive = 0 WHERE TemplateId = @TemplateId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TemplateId", templateId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar plantilla: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PARA CONTAR TOTAL DE PLANTILLAS POR CATEGORÍA
        public static int ContarTotalPlantillas(int locationCategoryId, string textoBusqueda = "")
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT COUNT(*) FROM ItemStockTemplates t
                        INNER JOIN Items i ON t.ItemId = i.ItemId
                        WHERE t.LocationCategoryId = @LocationCategoryId AND t.IsActive = 1";

                    List<SqlParameter> parametros = new List<SqlParameter>();
                    parametros.Add(new SqlParameter("@LocationCategoryId", locationCategoryId));

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += " AND (i.ItemCode LIKE @texto OR i.ItemName LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
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

        // MÉTODO: Verificar si ya existe la combinación categoría + artículo
        public static bool ExistePlantilla(int locationCategoryId, int itemId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT COUNT(*) FROM ItemStockTemplates 
                        WHERE LocationCategoryId = @LocationCategoryId AND ItemId = @ItemId AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationCategoryId", locationCategoryId);
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODO AUXILIAR: Mapear plantilla desde reader
        private static Mdl_ItemStockTemplates MapearPlantilla(SqlDataReader reader)
        {
            return new Mdl_ItemStockTemplates
            {
                TemplateId = reader.GetInt32(0),
                LocationCategoryId = reader.GetInt32(1),
                ItemId = reader.GetInt32(2),
                MinimumStock = reader.GetDecimal(3),
                MaximumStock = reader.GetDecimal(4),
                ReorderPoint = reader[5] == DBNull.Value ? (decimal?)null : reader.GetDecimal(5),
                IsActive = reader.GetBoolean(6),
                CreatedDate = reader.GetDateTime(7),
                CreatedBy = reader[8] == DBNull.Value ? null : (int?)reader.GetInt32(8),
                ModifiedDate = reader[9] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(9),
                ModifiedBy = reader[10] == DBNull.Value ? null : (int?)reader.GetInt32(10),
                LocationCategoryName = reader[11].ToString(),
                ItemCode = reader[12].ToString(),
                ItemName = reader[13].ToString()
            };
        }

        // MÉTODO PARA ELIMINAR ITEM DE LA PLANTILLA
        public static int EliminarPlantilla(int templateId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "DELETE FROM ItemStockTemplates WHERE TemplateId = @TemplateId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TemplateId", templateId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar plantilla: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
}