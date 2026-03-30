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
    internal class Ctrl_ItemMovementMaster
    {
        // MÉTODO: Generar número de movimiento
        public static string GenerarNumeroMovimiento()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT MAX(CAST(SUBSTRING(MovementNumber, 4, LEN(MovementNumber)) AS INT)) FROM ItemMovementMaster";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object result = cmd.ExecuteScalar();
                        int nextNumber = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
                        return "MOV" + nextNumber.ToString().PadLeft(6, '0');
                    }
                }
            }
            catch { return "MOV000001"; }
        }

        // MÉTODO PRINCIPAL: Registrar movimiento maestro (retorna el ID generado)
        public static int RegistrarMovimientoMaster(Mdl_ItemMovementMaster master)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO ItemMovementMaster (MovementNumber, MovementDate, MovementTypeId, 
                        LocationId, SupplierId, ReferenceDocument, DestinationLocationId, Remarks, TotalAmount, 
                        CreatedBy, IsActive) 
                        VALUES (@MovementNumber, @MovementDate, @MovementTypeId, @LocationId, @SupplierId, 
                        @ReferenceDocument, @DestinationLocationId, @Remarks, @TotalAmount, @CreatedBy, @IsActive);
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MovementNumber", master.MovementNumber ?? "");
                        cmd.Parameters.AddWithValue("@MovementDate", master.MovementDate);
                        cmd.Parameters.AddWithValue("@MovementTypeId", master.MovementTypeId);
                        cmd.Parameters.AddWithValue("@LocationId", master.LocationId);
                        cmd.Parameters.AddWithValue("@SupplierId", (object)master.SupplierId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReferenceDocument", (object)master.ReferenceDocument ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DestinationLocationId", (object)master.DestinationLocationId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Remarks", (object)master.Remarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TotalAmount", master.TotalAmount);
                        cmd.Parameters.AddWithValue("@CreatedBy", master.CreatedBy);
                        cmd.Parameters.AddWithValue("@IsActive", master.IsActive);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar movimiento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar movimientos con paginación
        public static List<Mdl_ItemMovementMaster> MostrarMovimientos(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_ItemMovementMaster> lista = new List<Mdl_ItemMovementMaster>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM ItemMovementMaster WHERE IsActive = 1 
                        ORDER BY MovementDate DESC, MovementMasterId DESC 
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearMovimientoMaster(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener movimientos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Búsqueda con filtros
        public static List<Mdl_ItemMovementMaster> BuscarMovimientos(
            string textoBusqueda = "",
            int? movementTypeId = null,
            int? locationId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            int pageNumber = 1,
            int pageSize = 100)
        {
            List<Mdl_ItemMovementMaster> lista = new List<Mdl_ItemMovementMaster>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM ItemMovementMaster WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += " AND (MovementNumber LIKE @texto OR ReferenceDocument LIKE @texto OR Remarks LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (movementTypeId.HasValue && movementTypeId > 0)
                    {
                        query += " AND MovementTypeId = @movementTypeId";
                        parametros.Add(new SqlParameter("@movementTypeId", movementTypeId.Value));
                    }

                    if (locationId.HasValue && locationId > 0)
                    {
                        query += " AND LocationId = @locationId";
                        parametros.Add(new SqlParameter("@locationId", locationId.Value));
                    }

                    if (fechaInicio.HasValue)
                    {
                        query += " AND MovementDate >= @fechaInicio";
                        parametros.Add(new SqlParameter("@fechaInicio", fechaInicio.Value));
                    }

                    if (fechaFin.HasValue)
                    {
                        query += " AND MovementDate <= @fechaFin";
                        parametros.Add(new SqlParameter("@fechaFin", fechaFin.Value));
                    }

                    query += " ORDER BY MovementDate DESC, MovementMasterId DESC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearMovimientoMaster(reader));
                            }
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

        // MÉTODO PRINCIPAL: Obtener por ID
        public static Mdl_ItemMovementMaster ObtenerMovimientoPorId(int movementMasterId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM ItemMovementMaster WHERE MovementMasterId = @MovementMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MovementMasterId", movementMasterId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearMovimientoMaster(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener movimiento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Actualizar total del movimiento
        public static int ActualizarTotalMovimiento(int movementMasterId, decimal totalAmount)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE ItemMovementMaster SET TotalAmount = @TotalAmount WHERE MovementMasterId = @MovementMasterId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MovementMasterId", movementMasterId);
                        cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar total: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Anular movimiento
        public static int AnularMovimiento(int movementMasterId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE ItemMovementMaster SET IsActive = 0, ModifiedDate = GETDATE(), 
                        ModifiedBy = @ModifiedBy WHERE MovementMasterId = @MovementMasterId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MovementMasterId", movementMasterId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al anular movimiento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear movimiento master
        private static Mdl_ItemMovementMaster MapearMovimientoMaster(SqlDataReader reader)
        {
            return new Mdl_ItemMovementMaster
            {
                MovementMasterId = reader.GetInt32(0),
                MovementNumber = reader[1].ToString(),
                MovementDate = reader.GetDateTime(2),
                MovementTypeId = reader.GetInt32(3),
                LocationId = reader.GetInt32(4),
                SupplierId = reader[5] == DBNull.Value ? null : (int?)reader.GetInt32(5),
                ReferenceDocument = reader[6] == DBNull.Value ? null : reader[6].ToString(),
                DestinationLocationId = reader[7] == DBNull.Value ? null : (int?)reader.GetInt32(7),
                Remarks = reader[8] == DBNull.Value ? null : reader[8].ToString(),
                TotalAmount = reader.GetDecimal(9),
                CreatedDate = reader.GetDateTime(10),
                CreatedBy = reader.GetInt32(11),
                ModifiedDate = reader[12] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(12),
                ModifiedBy = reader[13] == DBNull.Value ? null : (int?)reader.GetInt32(13),
                IsActive = reader.GetBoolean(14)
            };
        }

        // MÉTODO PARA CONTAR TOTAL
        public static int ContarTotalMovimientos(
            string textoBusqueda = "",
            int? movementTypeId = null,
            int? locationId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM ItemMovementMaster WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += " AND (MovementNumber LIKE @texto OR ReferenceDocument LIKE @texto OR Remarks LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (movementTypeId.HasValue && movementTypeId > 0)
                    {
                        query += " AND MovementTypeId = @movementTypeId";
                        parametros.Add(new SqlParameter("@movementTypeId", movementTypeId.Value));
                    }

                    if (locationId.HasValue && locationId > 0)
                    {
                        query += " AND LocationId = @locationId";
                        parametros.Add(new SqlParameter("@locationId", locationId.Value));
                    }

                    if (fechaInicio.HasValue)
                    {
                        query += " AND MovementDate >= @fechaInicio";
                        parametros.Add(new SqlParameter("@fechaInicio", fechaInicio.Value));
                    }

                    if (fechaFin.HasValue)
                    {
                        query += " AND MovementDate <= @fechaFin";
                        parametros.Add(new SqlParameter("@fechaFin", fechaFin.Value));
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
    }
}