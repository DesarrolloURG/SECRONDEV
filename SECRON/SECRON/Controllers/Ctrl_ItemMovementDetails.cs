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
    internal class Ctrl_ItemMovementDetails
    {
        // MÉTODO PRINCIPAL: Registrar detalle con actualización de stock
        public static int RegistrarDetalle(Mdl_ItemMovementDetails detalle, Mdl_MovementTypes tipoMovimiento)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Obtener el master para saber la ubicación
                            Mdl_ItemMovementMaster master = null;
                            string queryMaster = "SELECT * FROM ItemMovementMaster WHERE MovementMasterId = @MovementMasterId";
                            using (SqlCommand cmdMaster = new SqlCommand(queryMaster, connection, transaction))
                            {
                                cmdMaster.Parameters.AddWithValue("@MovementMasterId", detalle.MovementMasterId);
                                using (SqlDataReader reader = cmdMaster.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        master = new Mdl_ItemMovementMaster
                                        {
                                            MovementMasterId = reader.GetInt32(0),
                                            LocationId = reader.GetInt32(4),
                                            DestinationLocationId = reader[7] == DBNull.Value ? null : (int?)reader.GetInt32(7)
                                        };
                                    }
                                }
                            }

                            if (master == null)
                            {
                                throw new Exception("Movimiento maestro no encontrado");
                            }

                            // 2. Obtener o crear stock actual
                            Mdl_ItemStockByLocation stockActual = Ctrl_ItemStockByLocation.ObtenerOCrearStock(
                                detalle.ItemId, master.LocationId);

                            detalle.StockBeforeMovement = stockActual.CurrentStock;
                            decimal nuevoStock = stockActual.CurrentStock;

                            // 3. Calcular nuevo stock según tipo de movimiento
                            if (tipoMovimiento.AffectsStock == "+")
                            {
                                nuevoStock += detalle.Quantity;
                            }
                            else if (tipoMovimiento.AffectsStock == "-")
                            {
                                if (nuevoStock < detalle.Quantity)
                                {
                                    throw new Exception($"Stock insuficiente. Disponible: {nuevoStock}, Solicitado: {detalle.Quantity}");
                                }
                                nuevoStock -= detalle.Quantity;
                            }
                            else if (tipoMovimiento.AffectsStock == "0" && tipoMovimiento.TypeCode == "TRANSFERENCIA")
                            {
                                // Transferencia: resta en origen
                                if (nuevoStock < detalle.Quantity)
                                {
                                    throw new Exception($"Stock insuficiente para transferencia. Disponible: {nuevoStock}");
                                }
                                nuevoStock -= detalle.Quantity;

                                // Suma en destino
                                if (master.DestinationLocationId.HasValue)
                                {
                                    Mdl_ItemStockByLocation stockDestino = Ctrl_ItemStockByLocation.ObtenerOCrearStock(
                                        detalle.ItemId, master.DestinationLocationId.Value);

                                    string queryUpdateDestino = @"UPDATE ItemStockByLocation SET CurrentStock = @CurrentStock, 
                                        LastMovementDate = GETDATE() 
                                        WHERE ItemId = @ItemId AND LocationId = @LocationId";

                                    using (SqlCommand cmdDestino = new SqlCommand(queryUpdateDestino, connection, transaction))
                                    {
                                        cmdDestino.Parameters.AddWithValue("@ItemId", detalle.ItemId);
                                        cmdDestino.Parameters.AddWithValue("@LocationId", master.DestinationLocationId.Value);
                                        cmdDestino.Parameters.AddWithValue("@CurrentStock", stockDestino.CurrentStock + detalle.Quantity);
                                        cmdDestino.ExecuteNonQuery();
                                    }
                                }
                            }

                            detalle.StockAfterMovement = nuevoStock;
                            detalle.TotalCost = detalle.Quantity * detalle.UnitCost;

                            // 4. Actualizar stock en ubicación origen
                            string queryUpdateStock = @"UPDATE ItemStockByLocation SET CurrentStock = @CurrentStock, 
                                LastMovementDate = GETDATE() 
                                WHERE ItemId = @ItemId AND LocationId = @LocationId";

                            using (SqlCommand cmdStock = new SqlCommand(queryUpdateStock, connection, transaction))
                            {
                                cmdStock.Parameters.AddWithValue("@ItemId", detalle.ItemId);
                                cmdStock.Parameters.AddWithValue("@LocationId", master.LocationId);
                                cmdStock.Parameters.AddWithValue("@CurrentStock", nuevoStock);
                                cmdStock.ExecuteNonQuery();
                            }

                            // 5. Registrar detalle
                            string query = @"INSERT INTO ItemMovementDetails (MovementMasterId, ItemId, Quantity, 
                                UnitCost, StockBeforeMovement, StockAfterMovement, LotNumber, ExpiryDate, Remarks) 
                                VALUES (@MovementMasterId, @ItemId, @Quantity, @UnitCost, @StockBeforeMovement, 
                                @StockAfterMovement, @LotNumber, @ExpiryDate, @Remarks)";

                            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@MovementMasterId", detalle.MovementMasterId);
                                cmd.Parameters.AddWithValue("@ItemId", detalle.ItemId);
                                cmd.Parameters.AddWithValue("@Quantity", detalle.Quantity);
                                cmd.Parameters.AddWithValue("@UnitCost", detalle.UnitCost);
                                cmd.Parameters.AddWithValue("@StockBeforeMovement", detalle.StockBeforeMovement);
                                cmd.Parameters.AddWithValue("@StockAfterMovement", detalle.StockAfterMovement);
                                cmd.Parameters.AddWithValue("@LotNumber", (object)detalle.LotNumber ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@ExpiryDate", (object)detalle.ExpiryDate ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@Remarks", (object)detalle.Remarks ?? DBNull.Value);

                                int result = cmd.ExecuteNonQuery();

                                // 6. Actualizar total del master
                                string queryUpdateTotal = @"UPDATE ItemMovementMaster SET TotalAmount = 
                                    (SELECT ISNULL(SUM(Quantity * UnitCost), 0) FROM ItemMovementDetails 
                                    WHERE MovementMasterId = @MovementMasterId) 
                                    WHERE MovementMasterId = @MovementMasterId";

                                using (SqlCommand cmdTotal = new SqlCommand(queryUpdateTotal, connection, transaction))
                                {
                                    cmdTotal.Parameters.AddWithValue("@MovementMasterId", detalle.MovementMasterId);
                                    cmdTotal.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                return result;
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener detalles por master
        public static List<Mdl_ItemMovementDetails> ObtenerDetallesPorMaster(int movementMasterId)
        {
            List<Mdl_ItemMovementDetails> lista = new List<Mdl_ItemMovementDetails>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM ItemMovementDetails WHERE MovementMasterId = @MovementMasterId ORDER BY MovementDetailId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MovementMasterId", movementMasterId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearDetalle(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener detalles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Obtener kardex de un artículo (todos los movimientos)
        public static List<Mdl_ItemMovementDetails> ObtenerKardexPorArticulo(int itemId,
            int? locationId = null, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            List<Mdl_ItemMovementDetails> lista = new List<Mdl_ItemMovementDetails>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT d.* FROM ItemMovementDetails d
                        INNER JOIN ItemMovementMaster m ON d.MovementMasterId = m.MovementMasterId
                        WHERE d.ItemId = @ItemId AND m.IsActive = 1";

                    List<SqlParameter> parametros = new List<SqlParameter>();
                    parametros.Add(new SqlParameter("@ItemId", itemId));

                    if (locationId.HasValue)
                    {
                        query += " AND m.LocationId = @LocationId";
                        parametros.Add(new SqlParameter("@LocationId", locationId.Value));
                    }

                    if (fechaInicio.HasValue)
                    {
                        query += " AND m.MovementDate >= @FechaInicio";
                        parametros.Add(new SqlParameter("@FechaInicio", fechaInicio.Value));
                    }

                    if (fechaFin.HasValue)
                    {
                        query += " AND m.MovementDate <= @FechaFin";
                        parametros.Add(new SqlParameter("@FechaFin", fechaFin.Value));
                    }

                    query += " ORDER BY m.MovementDate DESC, d.MovementDetailId DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearDetalle(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener kardex: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Eliminar detalle (con reversión de stock)
        public static int EliminarDetalle(int movementDetailId, Mdl_MovementTypes tipoMovimiento)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Obtener el detalle a eliminar
                            Mdl_ItemMovementDetails detalle = null;
                            string queryDetalle = "SELECT * FROM ItemMovementDetails WHERE MovementDetailId = @MovementDetailId";
                            using (SqlCommand cmdDetalle = new SqlCommand(queryDetalle, connection, transaction))
                            {
                                cmdDetalle.Parameters.AddWithValue("@MovementDetailId", movementDetailId);
                                using (SqlDataReader reader = cmdDetalle.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        detalle = MapearDetalle(reader);
                                    }
                                }
                            }

                            if (detalle == null)
                            {
                                throw new Exception("Detalle no encontrado");
                            }

                            // 2. Obtener el master
                            Mdl_ItemMovementMaster master = null;
                            string queryMaster = "SELECT * FROM ItemMovementMaster WHERE MovementMasterId = @MovementMasterId";
                            using (SqlCommand cmdMaster = new SqlCommand(queryMaster, connection, transaction))
                            {
                                cmdMaster.Parameters.AddWithValue("@MovementMasterId", detalle.MovementMasterId);
                                using (SqlDataReader reader = cmdMaster.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        master = new Mdl_ItemMovementMaster
                                        {
                                            MovementMasterId = reader.GetInt32(0),
                                            LocationId = reader.GetInt32(4),
                                            DestinationLocationId = reader[7] == DBNull.Value ? null : (int?)reader.GetInt32(7)
                                        };
                                    }
                                }
                            }

                            // 3. Revertir stock (operación inversa)
                            Mdl_ItemStockByLocation stock = Ctrl_ItemStockByLocation.ObtenerOCrearStock(
                                detalle.ItemId, master.LocationId);

                            decimal stockRevertido = stock.CurrentStock;

                            if (tipoMovimiento.AffectsStock == "+")
                            {
                                // Si era entrada, ahora restamos
                                stockRevertido -= detalle.Quantity;
                            }
                            else if (tipoMovimiento.AffectsStock == "-")
                            {
                                // Si era salida, ahora sumamos
                                stockRevertido += detalle.Quantity;
                            }
                            else if (tipoMovimiento.AffectsStock == "0" && tipoMovimiento.TypeCode == "TRANSFERENCIA")
                            {
                                // Revertir transferencia
                                stockRevertido += detalle.Quantity;

                                if (master.DestinationLocationId.HasValue)
                                {
                                    Mdl_ItemStockByLocation stockDestino = Ctrl_ItemStockByLocation.ObtenerOCrearStock(
                                        detalle.ItemId, master.DestinationLocationId.Value);

                                    string queryRevertDestino = @"UPDATE ItemStockByLocation SET CurrentStock = @CurrentStock, 
                                        LastMovementDate = GETDATE() 
                                        WHERE ItemId = @ItemId AND LocationId = @LocationId";

                                    using (SqlCommand cmdDestino = new SqlCommand(queryRevertDestino, connection, transaction))
                                    {
                                        cmdDestino.Parameters.AddWithValue("@ItemId", detalle.ItemId);
                                        cmdDestino.Parameters.AddWithValue("@LocationId", master.DestinationLocationId.Value);
                                        cmdDestino.Parameters.AddWithValue("@CurrentStock", stockDestino.CurrentStock - detalle.Quantity);
                                        cmdDestino.ExecuteNonQuery();
                                    }
                                }
                            }

                            // 4. Actualizar stock
                            string queryUpdateStock = @"UPDATE ItemStockByLocation SET CurrentStock = @CurrentStock, 
                                LastMovementDate = GETDATE() 
                                WHERE ItemId = @ItemId AND LocationId = @LocationId";

                            using (SqlCommand cmdStock = new SqlCommand(queryUpdateStock, connection, transaction))
                            {
                                cmdStock.Parameters.AddWithValue("@ItemId", detalle.ItemId);
                                cmdStock.Parameters.AddWithValue("@LocationId", master.LocationId);
                                cmdStock.Parameters.AddWithValue("@CurrentStock", stockRevertido);
                                cmdStock.ExecuteNonQuery();
                            }

                            // 5. Eliminar detalle
                            string queryDelete = "DELETE FROM ItemMovementDetails WHERE MovementDetailId = @MovementDetailId";
                            using (SqlCommand cmdDelete = new SqlCommand(queryDelete, connection, transaction))
                            {
                                cmdDelete.Parameters.AddWithValue("@MovementDetailId", movementDetailId);
                                int result = cmdDelete.ExecuteNonQuery();

                                // 6. Actualizar total del master
                                string queryUpdateTotal = @"UPDATE ItemMovementMaster SET TotalAmount = 
                                    (SELECT ISNULL(SUM(Quantity * UnitCost), 0) FROM ItemMovementDetails 
                                    WHERE MovementMasterId = @MovementMasterId) 
                                    WHERE MovementMasterId = @MovementMasterId";

                                using (SqlCommand cmdTotal = new SqlCommand(queryUpdateTotal, connection, transaction))
                                {
                                    cmdTotal.Parameters.AddWithValue("@MovementMasterId", detalle.MovementMasterId);
                                    cmdTotal.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                return result;
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear detalle
        private static Mdl_ItemMovementDetails MapearDetalle(SqlDataReader reader)
        {
            return new Mdl_ItemMovementDetails
            {
                MovementDetailId = reader.GetInt32(0),
                MovementMasterId = reader.GetInt32(1),
                ItemId = reader.GetInt32(2),
                Quantity = reader.GetDecimal(3),
                UnitCost = reader.GetDecimal(4),
                TotalCost = reader.GetDecimal(5),
                StockBeforeMovement = reader.GetDecimal(6),
                StockAfterMovement = reader.GetDecimal(7),
                LotNumber = reader[8] == DBNull.Value ? null : reader[8].ToString(),
                ExpiryDate = reader[9] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(9),
                Remarks = reader[10] == DBNull.Value ? null : reader[10].ToString()
            };
        }
    }
}