using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_ItemWarehouseStock
    {
        // MÉTODO: Obtener stock por bodega con detalle del artículo resuelto (para grilla)
        public static List<Mdl_ItemWarehouseStock> ObtenerStockPorBodegaConDetalle(int warehouseId)
        {
            List<Mdl_ItemWarehouseStock> lista = new List<Mdl_ItemWarehouseStock>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT s.ItemWarehouseStockId, s.ItemId, s.WarehouseId,
                        s.CurrentStock, s.MinimumStock, s.MaximumStock, s.ReorderPoint,
                        s.MovementCounter, s.LastMovementDate,
                        i.ItemCode, i.ItemName, i.Description, i.UnitCost, i.LastPurchasePrice,
                        i.HasLotControl, i.HasExpiryDate,
                        c.CategoryId, c.CategoryName,
                        u.UnitId, u.UnitName
                        FROM ItemWarehouseStock s
                        INNER JOIN Items i ON s.ItemId = i.ItemId
                        INNER JOIN ItemCategories c ON i.CategoryId = c.CategoryId
                        INNER JOIN MeasurementUnits u ON i.UnitId = u.UnitId
                        WHERE s.WarehouseId = @WarehouseId AND i.IsActive = 1
                        ORDER BY i.ItemName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
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
                MessageBox.Show("Error al obtener stock por bodega: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO: Obtener bodegas asignadas a un usuario
        // Si esAdmin = true (KARDEX_WAREHOUSE_ADMIN), retorna TODAS las bodegas activas
        public static List<KeyValuePair<int, string>> ObtenerBodegasDelUsuario(int userId, bool esAdmin = false)
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = esAdmin
                        ? @"SELECT w.WarehouseId, w.WarehouseName
                    FROM Warehouses w
                    WHERE w.IsActive = 1
                    ORDER BY w.WarehouseName"
                        : @"SELECT w.WarehouseId, w.WarehouseName
                    FROM WarehouseManagers wm
                    INNER JOIN Warehouses w ON w.WarehouseId = wm.WarehouseId
                    WHERE wm.UserId = @UserId AND wm.IsActive = 1 AND w.IsActive = 1
                    ORDER BY w.WarehouseName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        if (!esAdmin)
                            cmd.Parameters.AddWithValue("@UserId", userId);

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
                MessageBox.Show("Error al obtener bodegas del usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO: Registrar stock inicial de un artículo en una bodega (usado al copiar desde plantilla/sede base)
        public static int RegistrarStockInicial(Mdl_ItemWarehouseStock stock, int createdBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_ItemWarehouseStock_Insert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ItemId", stock.ItemId);
                    cmd.Parameters.AddWithValue("@WarehouseId", stock.WarehouseId);
                    cmd.Parameters.AddWithValue("@CurrentStock", stock.CurrentStock);
                    cmd.Parameters.AddWithValue("@MinimumStock", stock.MinimumStock);
                    cmd.Parameters.AddWithValue("@MaximumStock", (object)stock.MaximumStock == null ? DBNull.Value : (object)stock.MaximumStock);
                    cmd.Parameters.AddWithValue("@ReorderPoint", (object)stock.ReorderPoint == null ? DBNull.Value : (object)stock.ReorderPoint);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar stock inicial: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO: Actualizar límites de stock (MinimumStock, MaximumStock, ReorderPoint)
        public static int ActualizarLimitesStock(int itemWarehouseStockId, decimal minimumStock, decimal maximumStock, decimal reorderPoint, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_ItemWarehouseStock_UpdateLimits", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ItemWarehouseStockId", itemWarehouseStockId);
                    cmd.Parameters.AddWithValue("@MinimumStock", minimumStock);
                    cmd.Parameters.AddWithValue("@MaximumStock", maximumStock);
                    cmd.Parameters.AddWithValue("@ReorderPoint", reorderPoint);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar límites de stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear stock con campos de artículo resueltos
        private static Mdl_ItemWarehouseStock MapearStockConDetalle(SqlDataReader reader)
        {
            return new Mdl_ItemWarehouseStock
            {
                ItemWarehouseStockId = Convert.ToInt32(reader["ItemWarehouseStockId"]),
                ItemId = Convert.ToInt32(reader["ItemId"]),
                WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                CurrentStock = Convert.ToDecimal(reader["CurrentStock"]),
                MinimumStock = reader["MinimumStock"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MinimumStock"]),
                MaximumStock = reader["MaximumStock"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MaximumStock"]),
                ReorderPoint = reader["ReorderPoint"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ReorderPoint"]),
                MovementCounter = Convert.ToInt32(reader["MovementCounter"]),
                LastMovementDate = reader["LastMovementDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["LastMovementDate"]),
                ItemCode = reader["ItemCode"]?.ToString() ?? "",
                ItemName = reader["ItemName"]?.ToString() ?? "",
                Description = reader["Description"]?.ToString() ?? "",
                UnitCost = reader["UnitCost"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["UnitCost"]),
                LastPurchasePrice = reader["LastPurchasePrice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["LastPurchasePrice"]),
                HasLotControl = reader["HasLotControl"] != DBNull.Value && Convert.ToBoolean(reader["HasLotControl"]),
                HasExpiryDate = reader["HasExpiryDate"] != DBNull.Value && Convert.ToBoolean(reader["HasExpiryDate"]),
                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                CategoryName = reader["CategoryName"]?.ToString() ?? "",
                UnitId = Convert.ToInt32(reader["UnitId"]),
                UnitName = reader["UnitName"]?.ToString() ?? ""
            };
        }

        public static int ActualizarStockCompleto(Mdl_ItemWarehouseStock stock, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_ItemWarehouseStock_UpdateFull", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ItemWarehouseStockId", stock.ItemWarehouseStockId);
                    cmd.Parameters.AddWithValue("@CurrentStock", stock.CurrentStock);
                    cmd.Parameters.AddWithValue("@MinimumStock", stock.MinimumStock);
                    cmd.Parameters.AddWithValue("@MaximumStock", stock.MaximumStock);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int EliminarStockDeBodega(int itemWarehouseStockId, int deletedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_ItemWarehouseStock_Delete", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ItemWarehouseStockId", itemWarehouseStockId);
                    cmd.Parameters.AddWithValue("@DeletedBy", deletedBy);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
}