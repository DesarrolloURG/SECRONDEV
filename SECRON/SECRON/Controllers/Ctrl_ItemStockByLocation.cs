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
    internal class Ctrl_ItemStockByLocation
    {
        // MÉTODO PRINCIPAL: Obtener o crear registro de stock
        public static Mdl_ItemStockByLocation ObtenerOCrearStock(int itemId, int locationId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // Primero intentamos obtener
                    string querySelect = @"SELECT * FROM ItemStockByLocation 
                        WHERE ItemId = @ItemId AND LocationId = @LocationId";

                    using (SqlCommand cmd = new SqlCommand(querySelect, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        cmd.Parameters.AddWithValue("@LocationId", locationId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearStock(reader);
                            }
                        }
                    }

                    // Si no existe, lo creamos
                    string queryInsert = @"INSERT INTO ItemStockByLocation (ItemId, LocationId, CurrentStock, 
                        ReservedStock, MinimumStock, IsActive) 
                        VALUES (@ItemId, @LocationId, 0, 0, 0, 1);
                        SELECT * FROM ItemStockByLocation WHERE ItemStockLocationId = SCOPE_IDENTITY()";

                    using (SqlCommand cmd = new SqlCommand(queryInsert, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        cmd.Parameters.AddWithValue("@LocationId", locationId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearStock(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener/crear stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Actualizar stock
        public static int ActualizarStock(int itemId, int locationId, decimal newStock)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE ItemStockByLocation SET CurrentStock = @CurrentStock, 
                        LastMovementDate = GETDATE() 
                        WHERE ItemId = @ItemId AND LocationId = @LocationId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        cmd.Parameters.AddWithValue("@CurrentStock", newStock);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener stock por artículo (todas las ubicaciones)
        public static List<Mdl_ItemStockByLocation> ObtenerStockPorArticulo(int itemId)
        {
            List<Mdl_ItemStockByLocation> lista = new List<Mdl_ItemStockByLocation>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM ItemStockByLocation WHERE ItemId = @ItemId AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearStock(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener stock por artículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Obtener stock por ubicación
        public static List<Mdl_ItemStockByLocation> ObtenerStockPorUbicacion(int locationId)
        {
            List<Mdl_ItemStockByLocation> lista = new List<Mdl_ItemStockByLocation>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM ItemStockByLocation WHERE LocationId = @LocationId AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearStock(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener stock por ubicación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Obtener stock total de un artículo (suma de todas las ubicaciones)
        public static decimal ObtenerStockTotal(int itemId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT ISNULL(SUM(CurrentStock), 0) FROM ItemStockByLocation WHERE ItemId = @ItemId AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        return Convert.ToDecimal(cmd.ExecuteScalar());
                    }
                }
            }
            catch { return 0; }
        }

        // MÉTODO PRINCIPAL: Verificar si hay stock bajo mínimo
        public static List<Mdl_ItemStockByLocation> ObtenerArticulosBajoMinimo()
        {
            List<Mdl_ItemStockByLocation> lista = new List<Mdl_ItemStockByLocation>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM ItemStockByLocation 
                        WHERE CurrentStock <= MinimumStock AND MinimumStock > 0 AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearStock(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener artículos bajo mínimo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO AUXILIAR: Mapear stock
        private static Mdl_ItemStockByLocation MapearStock(SqlDataReader reader)
        {
            return new Mdl_ItemStockByLocation
            {
                ItemStockLocationId = reader.GetInt32(0),
                ItemId = reader.GetInt32(1),
                LocationId = reader.GetInt32(2),
                CurrentStock = reader.GetDecimal(3),
                ReservedStock = reader.GetDecimal(4),
                AvailableStock = reader.GetDecimal(5),  // Calculado en BD
                MinimumStock = reader.GetDecimal(6),
                MaximumStock = reader[7] == DBNull.Value ? 0 : reader.GetDecimal(7),
                LastMovementDate = reader[8] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(8),
                IsActive = reader.GetBoolean(9)
            };
        }
        // MÉTODO: Registrar stock inicial de un artículo en una sede
        public static int RegistrarStockInicial(Mdl_ItemStockByLocation stock)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                IF EXISTS (SELECT 1 FROM ItemStockByLocation WHERE ItemId = @ItemId AND LocationId = @LocationId)
                    UPDATE ItemStockByLocation SET
                        CurrentStock = @CurrentStock,
                        ReservedStock = @ReservedStock,
                        MinimumStock = @MinimumStock,
                        MaximumStock = @MaximumStock,
                        IsActive = 1,
                        LastMovementDate = GETDATE()
                    WHERE ItemId = @ItemId AND LocationId = @LocationId
                ELSE
                    INSERT INTO ItemStockByLocation 
                        (ItemId, LocationId, CurrentStock, ReservedStock, MinimumStock, MaximumStock, IsActive)
                    VALUES 
                        (@ItemId, @LocationId, @CurrentStock, @ReservedStock, @MinimumStock, @MaximumStock, 1)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", stock.ItemId);
                        cmd.Parameters.AddWithValue("@LocationId", stock.LocationId);
                        cmd.Parameters.AddWithValue("@CurrentStock", stock.CurrentStock);
                        cmd.Parameters.AddWithValue("@ReservedStock", stock.ReservedStock);
                        cmd.Parameters.AddWithValue("@MinimumStock", stock.MinimumStock);
                        cmd.Parameters.AddWithValue("@MaximumStock", (object)stock.MaximumStock == null ? DBNull.Value : (object)stock.MaximumStock);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar stock inicial: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO: Actualizar stock mínimo, máximo y stock actual desde formulario
        public static int ActualizarStockCompleto(Mdl_ItemStockByLocation stock)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE ItemStockByLocation SET
                CurrentStock = @CurrentStock,
                MinimumStock = @MinimumStock,
                MaximumStock = @MaximumStock,
                LastMovementDate = GETDATE()
                WHERE ItemStockLocationId = @ItemStockLocationId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemStockLocationId", stock.ItemStockLocationId);
                        cmd.Parameters.AddWithValue("@CurrentStock", stock.CurrentStock);
                        cmd.Parameters.AddWithValue("@MinimumStock", stock.MinimumStock);
                        cmd.Parameters.AddWithValue("@MaximumStock", (object)stock.MaximumStock == null ? DBNull.Value : (object)stock.MaximumStock);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO: Eliminar artículo de stock de una sede
        public static int EliminarStockDeUbicacion(int itemStockLocationId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "DELETE FROM ItemStockByLocation WHERE ItemStockLocationId = @ItemStockLocationId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemStockLocationId", itemStockLocationId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO: Obtener stock por ubicación con nombre del artículo resuelto (para grilla)
        public static List<Mdl_ItemStockByLocation> ObtenerStockPorUbicacionConDetalle(int locationId)
        {
            List<Mdl_ItemStockByLocation> lista = new List<Mdl_ItemStockByLocation>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT s.ItemStockLocationId, s.ItemId, s.LocationId,
                    s.CurrentStock, s.ReservedStock, s.AvailableStock,
                    s.MinimumStock, s.MaximumStock, s.LastMovementDate, s.IsActive,
                    i.ItemCode, i.ItemName,
                    c.CategoryId, c.CategoryName
                    FROM ItemStockByLocation s
                    INNER JOIN Items i ON s.ItemId = i.ItemId
                    INNER JOIN ItemCategories c ON i.CategoryId = c.CategoryId
                    WHERE s.LocationId = @LocationId AND s.IsActive = 1
                    ORDER BY i.ItemName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearStockConDetalle(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener stock con detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO: Obtener sedes de la misma categoría que una sede dada
        public static List<KeyValuePair<int, string>> ObtenerSedesMismaCategoria(int locationId)
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT l.LocationId, l.LocationName
                FROM Locations l
                WHERE l.LocationCategoryId = (
                    SELECT LocationCategoryId FROM Locations WHERE LocationId = @LocationId
                )
                AND l.LocationId <> @LocationId
                AND l.IsActive = 1
                ORDER BY l.LocationName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new KeyValuePair<int, string>(
                                    reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener sedes de misma categoría: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO: Obtener categoría de una sede
        public static int ObtenerCategoriaIdDeSede(int locationId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT ISNULL(LocationCategoryId, 0) FROM Locations WHERE LocationId = @LocationId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        object resultado = cmd.ExecuteScalar();
                        return resultado == null || resultado == DBNull.Value ? 0 : Convert.ToInt32(resultado);
                    }
                }
            }
            catch { return 0; }
        }

        // MÉTODO: Obtener sedes para combo
        public static List<KeyValuePair<int, string>> ObtenerSedesParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT LocationId, LocationName FROM Locations WHERE IsActive = 1 ORDER BY LocationName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new KeyValuePair<int, string>(
                                    reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener sedes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO AUXILIAR: Mapear stock con campos de artículo resueltos
        private static Mdl_ItemStockByLocation MapearStockConDetalle(SqlDataReader reader)
        {
            var stock = new Mdl_ItemStockByLocation
            {
                ItemStockLocationId = Convert.ToInt32(reader["ItemStockLocationId"]),
                ItemId = Convert.ToInt32(reader["ItemId"]),
                LocationId = Convert.ToInt32(reader["LocationId"]),
                CurrentStock = Convert.ToDecimal(reader["CurrentStock"]),
                ReservedStock = Convert.ToDecimal(reader["ReservedStock"]),
                AvailableStock = Convert.ToDecimal(reader["AvailableStock"]),
                MinimumStock = Convert.ToDecimal(reader["MinimumStock"]),
                MaximumStock = reader["MaximumStock"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MaximumStock"]),
                LastMovementDate = reader["LastMovementDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["LastMovementDate"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                ItemCode = reader["ItemCode"]?.ToString() ?? "",
                ItemName = reader["ItemName"]?.ToString() ?? "",
                CategoryId = reader["CategoryId"] != DBNull.Value ? Convert.ToInt32(reader["CategoryId"]) : 0,
                CategoryName = reader["CategoryName"]?.ToString() ?? ""
            };
            return stock;
        }
    }
}