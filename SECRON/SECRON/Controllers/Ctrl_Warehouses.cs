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
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ModifiedBy"))
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
    }
}