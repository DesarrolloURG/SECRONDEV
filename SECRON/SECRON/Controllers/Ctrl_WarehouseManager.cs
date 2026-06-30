using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_WarehouseManager
    {
        // MÉTODO: Listar bodegas asignadas (activas) a un usuario, con nombre resuelto
        public static List<Mdl_WarehouseManager> ObtenerBodegasPorUsuario(int userId)
        {
            List<Mdl_WarehouseManager> lista = new List<Mdl_WarehouseManager>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT wm.WarehouseManagerId, wm.WarehouseId, w.WarehouseName, wm.UserId, wm.IsActive
                        FROM WarehouseManagers wm
                        INNER JOIN Warehouses w ON w.WarehouseId = wm.WarehouseId
                        WHERE wm.UserId = @UserId AND wm.IsActive = 1
                        ORDER BY w.WarehouseName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Mdl_WarehouseManager
                                {
                                    WarehouseManagerId = Convert.ToInt32(reader["WarehouseManagerId"]),
                                    WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                                    WarehouseName = reader["WarehouseName"].ToString(),
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    IsActive = Convert.ToBoolean(reader["IsActive"])
                                });
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

        // MÉTODO: Obtener WarehouseManagerId de una combinación usuario+bodega (activa), o null si no existe
        public static int? ObtenerWarehouseManagerId(int userId, int warehouseId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT WarehouseManagerId FROM WarehouseManagers
                        WHERE UserId = @UserId AND WarehouseId = @WarehouseId AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                        object resultado = cmd.ExecuteScalar();
                        return resultado == null || resultado == DBNull.Value ? (int?)null : Convert.ToInt32(resultado);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener encargado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // MÉTODO: Asignar bodega a usuario. Retornos: 1=éxito, -1=ya estaba asignado activo, 0=error
        public static int AsignarBodega(int warehouseId, int userId, int createdBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_WarehouseManager_Insert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar bodega: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO: Quitar (inactivar) la asignación de bodega a usuario
        public static int QuitarBodega(int warehouseManagerId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_WarehouseManager_Inactive", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@WarehouseManagerId", warehouseManagerId);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al quitar bodega: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO: Listar todas las bodegas disponibles para asignar (todas las activas; el formulario filtra las ya asignadas)
        public static List<Mdl_Warehouse> ObtenerBodegasDisponibles()
        {
            return Ctrl_Warehouses.MostrarBodegas();
        }

        // MÉTODO: Listar usuarios distintos que tienen asignación activa en alguna de las bodegas indicadas
        public static List<Mdl_Users> ObtenerUsuariosConBodegaEnLista(List<int> warehouseIds)
        {
            List<Mdl_Users> lista = new List<Mdl_Users>();
            if (warehouseIds == null || warehouseIds.Count == 0) return lista;

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string idsConcat = string.Join(",", warehouseIds);

                    string query = $@"SELECT DISTINCT u.UserId, u.Username, u.FullName
                FROM WarehouseManagers wm
                INNER JOIN Users u ON u.UserId = wm.UserId
                WHERE wm.WarehouseId IN ({idsConcat})
                  AND wm.IsActive = 1
                ORDER BY u.FullName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Mdl_Users
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Username = reader["Username"].ToString(),
                                FullName = reader["FullName"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener usuarios con bodega asignada: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
    }
}