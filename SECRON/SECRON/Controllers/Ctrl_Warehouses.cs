using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_Warehouses
    {
        public static List<Mdl_Warehouse> MostrarBodegas()
        {
            List<Mdl_Warehouse> lista = new List<Mdl_Warehouse>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT WarehouseId, WarehouseCode, WarehouseName, Description,
                               Address, PhoneNumber, ManagerUserId, WarehouseType,
                               LocationId, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy
                        FROM   Warehouses
                        WHERE  IsActive = 1
                        ORDER  BY WarehouseName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(MapearBodega(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener bodegas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static List<Mdl_Warehouse> BuscarBodegas(string textoBusqueda)
        {
            List<Mdl_Warehouse> lista = new List<Mdl_Warehouse>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT WarehouseId, WarehouseCode, WarehouseName, Description,
                               Address, PhoneNumber, ManagerUserId, WarehouseType,
                               LocationId, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy
                        FROM   Warehouses
                        WHERE  IsActive = 1
                        AND   (WarehouseCode LIKE @texto OR WarehouseName LIKE @texto)
                        ORDER  BY WarehouseName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@texto", "%" + textoBusqueda.Trim() + "%");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearBodega(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar bodegas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static List<KeyValuePair<int, string>> ObtenerBodegasParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT WarehouseId, WarehouseName FROM Warehouses WHERE IsActive = 1 ORDER BY WarehouseName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener bodegas para combo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static List<KeyValuePair<int, string>> ObtenerBodegasPorLocation(int locationId)
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT WarehouseId, WarehouseName
                        FROM   Warehouses
                        WHERE  IsActive = 1
                        AND    LocationId = @LocationId
                        ORDER  BY WarehouseName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
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
                MessageBox.Show("Error al obtener bodegas por sede: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        private static Mdl_Warehouse MapearBodega(SqlDataReader reader)
        {
            return new Mdl_Warehouse
            {
                WarehouseId = reader.GetInt32(reader.GetOrdinal("WarehouseId")),
                WarehouseCode = reader["WarehouseCode"].ToString(),
                WarehouseName = reader["WarehouseName"].ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                Address = reader["Address"] == DBNull.Value ? null : reader["Address"].ToString(),
                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : reader["PhoneNumber"].ToString(),
                ManagerUserId = reader["ManagerUserId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ManagerUserId")),
                WarehouseType = reader["WarehouseType"] == DBNull.Value ? null : reader["WarehouseType"].ToString(),
                LocationId = reader["LocationId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("LocationId")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ModifiedBy")),
                LocationCode = reader["LocationCode"] == DBNull.Value ? null : reader["LocationCode"].ToString(),
                LocationName = reader["LocationName"] == DBNull.Value ? null : reader["LocationName"].ToString()
            };
        }

        // MÉTODO: Obtener el WarehouseId de la bodega central (Locations.LocationCode = '1')
        public static int? ObtenerBodegaCentralId()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT w.WarehouseId
                        FROM   Warehouses w
                        INNER JOIN Locations l ON l.LocationId = w.LocationId
                        WHERE  l.LocationCode = '1'
                        AND    w.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object resultado = cmd.ExecuteScalar();
                        return resultado == null || resultado == DBNull.Value
                            ? (int?)null
                            : Convert.ToInt32(resultado);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener la bodega central: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // MÉTODO: Obtener bodegas asignadas a un usuario (o todas si esAdmin=true), con su LocationId/LocationName resuelto
        // Usado para llenar combos dependientes Sede -> Bodega filtrados por asignación (WarehouseManagers)
        public static List<Mdl_WarehouseWithLocation> ObtenerBodegasAsignadasConLocation(int userId, bool esAdmin = false)
        {
            List<Mdl_WarehouseWithLocation> lista = new List<Mdl_WarehouseWithLocation>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = esAdmin
                        ? @"SELECT w.WarehouseId, w.WarehouseName, l.LocationId, l.LocationName
                    FROM Warehouses w
                    INNER JOIN Locations l ON l.LocationId = w.LocationId
                    WHERE w.IsActive = 1 AND l.IsActive = 1
                    ORDER BY l.LocationName, w.WarehouseName"
                        : @"SELECT w.WarehouseId, w.WarehouseName, l.LocationId, l.LocationName
                    FROM WarehouseManagers wm
                    INNER JOIN Warehouses w ON w.WarehouseId = wm.WarehouseId
                    INNER JOIN Locations l ON l.LocationId = w.LocationId
                    WHERE wm.UserId = @UserId AND wm.IsActive = 1
                      AND w.IsActive = 1 AND l.IsActive = 1
                    ORDER BY l.LocationName, w.WarehouseName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        if (!esAdmin)
                            cmd.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Mdl_WarehouseWithLocation
                                {
                                    WarehouseId = reader.GetInt32(0),
                                    WarehouseName = reader.GetString(1),
                                    LocationId = reader.GetInt32(2),
                                    LocationName = reader.GetString(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener bodegas asignadas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static int Create(Mdl_Warehouse warehouse)
        {
            int resultado = 0;

            using (SqlConnection conn = DatabaseConfig.StartConection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_Warehouses_Create", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@LocationId", warehouse.LocationId);
                    cmd.Parameters.AddWithValue("@WarehouseName", warehouse.WarehouseName);
                    cmd.Parameters.AddWithValue("@Description", (object)warehouse.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)warehouse.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)warehouse.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@WarehouseType", warehouse.WarehouseType);
                    cmd.Parameters.AddWithValue("@CreatedBy", warehouse.CreatedBy);

                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnValue", System.Data.SqlDbType.Int);
                    returnParam.Direction = System.Data.ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();

                    resultado = (int)returnParam.Value;
                }
            }

            return resultado;
        }

        public static int Update(Mdl_Warehouse warehouse, bool isInactivation)
        {
            int resultado = 0;

            using (SqlConnection conn = DatabaseConfig.StartConection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_Warehouses_Update", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@WarehouseId", warehouse.WarehouseId);
                    cmd.Parameters.AddWithValue("@WarehouseName", warehouse.WarehouseName);
                    cmd.Parameters.AddWithValue("@Description", (object)warehouse.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)warehouse.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)warehouse.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@WarehouseType", warehouse.WarehouseType);
                    cmd.Parameters.AddWithValue("@IsInactivation", isInactivation);
                    cmd.Parameters.AddWithValue("@ModifiedBy", warehouse.ModifiedBy);

                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnValue", System.Data.SqlDbType.Int);
                    returnParam.Direction = System.Data.ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();

                    resultado = (int)returnParam.Value;
                }
            }

            return resultado;
        }

        public static List<Mdl_Warehouse> BuscarBodegas(
            string textoBusqueda, string tipoFiltro, bool? isActive, int pageNumber, int pageSize)
        {
            List<Mdl_Warehouse> lista = new List<Mdl_Warehouse>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    int offset = (pageNumber - 1) * pageSize;
                    string texto = (textoBusqueda ?? "").Trim();

                    string condicionTexto = tipoFiltro == "BUSCAR POR NOMBRE"
                        ? "w.WarehouseName LIKE @Texto"
                        : tipoFiltro == "BUSCAR POR CODIGO"
                            ? "w.WarehouseCode LIKE @Texto"
                            : "(w.WarehouseCode LIKE @Texto OR w.WarehouseName LIKE @Texto)";

                    string query = $@"
                        SELECT w.WarehouseId, w.WarehouseCode, w.WarehouseName, w.Description,
                               w.Address, w.PhoneNumber, w.ManagerUserId, w.WarehouseType,
                               w.LocationId, w.IsActive, w.CreatedDate, w.CreatedBy, w.ModifiedDate, w.ModifiedBy,
                               l.LocationCode, l.LocationName
                        FROM   Warehouses w
                        INNER JOIN Locations l ON l.LocationId = w.LocationId
                        WHERE  (@IsActive IS NULL OR w.IsActive = @IsActive)
                          AND  (@TextoVacio = 1 OR {condicionTexto})
                        ORDER  BY w.WarehouseName
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@IsActive", (object)isActive ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TextoVacio", string.IsNullOrEmpty(texto));
                        cmd.Parameters.AddWithValue("@Texto", "%" + texto + "%");
                        cmd.Parameters.AddWithValue("@Offset", offset);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearBodega(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar bodegas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static int ContarTotalBodegas(string textoBusqueda, string tipoFiltro, bool? isActive)
        {
            int total = 0;
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string texto = (textoBusqueda ?? "").Trim();

                    string condicionTexto = tipoFiltro == "BUSCAR POR NOMBRE"
                        ? "w.WarehouseName LIKE @Texto"
                        : tipoFiltro == "BUSCAR POR CODIGO"
                            ? "w.WarehouseCode LIKE @Texto"
                            : "(w.WarehouseCode LIKE @Texto OR w.WarehouseName LIKE @Texto)";

                    string query = $@"
                        SELECT COUNT(1)
                        FROM   Warehouses w
                        WHERE  (@IsActive IS NULL OR w.IsActive = @IsActive)
                          AND  (@TextoVacio = 1 OR {condicionTexto})";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@IsActive", (object)isActive ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TextoVacio", string.IsNullOrEmpty(texto));
                        cmd.Parameters.AddWithValue("@Texto", "%" + texto + "%");

                        total = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al contar bodegas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return total;
        }
    }
}