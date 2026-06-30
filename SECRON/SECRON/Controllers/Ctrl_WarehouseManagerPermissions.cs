using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_WarehouseManagerPermissions
    {
        // MÉTODO: Verificar si un usuario tiene un permiso contextual otorgado en una bodega específica
        public static bool TienePermisoEnBodega(int userId, int warehouseId, string permissionCode)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT 1
                        FROM WarehouseManagers wm
                        INNER JOIN WarehouseManagerPermissions wmp ON wmp.WarehouseManagerId = wm.WarehouseManagerId
                        INNER JOIN WarehousePermissions wp ON wp.WarehousePermissionId = wmp.WarehousePermissionId
                        WHERE wm.UserId = @UserId
                          AND wm.WarehouseId = @WarehouseId
                          AND wm.IsActive = 1
                          AND wmp.IsGranted = 1
                          AND wp.PermissionCode = @PermissionCode
                          AND wp.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                        cmd.Parameters.AddWithValue("@PermissionCode", permissionCode);

                        object resultado = cmd.ExecuteScalar();
                        return resultado != null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar permiso de bodega: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // MÉTODO: Obtener el límite máximo de despacho por artículo para un usuario en una bodega
        // Retorna null si no tiene límite configurado (sin tope)
        public static decimal? ObtenerLimiteDespacho(int userId, int warehouseId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT wmp.MaxQuantityPerDispatch
                        FROM WarehouseManagers wm
                        INNER JOIN WarehouseManagerPermissions wmp ON wmp.WarehouseManagerId = wm.WarehouseManagerId
                        INNER JOIN WarehousePermissions wp ON wp.WarehousePermissionId = wmp.WarehousePermissionId
                        WHERE wm.UserId = @UserId
                          AND wm.WarehouseId = @WarehouseId
                          AND wm.IsActive = 1
                          AND wmp.IsGranted = 1
                          AND wp.PermissionCode = 'DESPACHO_EMPLEADO'
                          AND wp.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);

                        object resultado = cmd.ExecuteScalar();
                        if (resultado == null || resultado == DBNull.Value)
                            return null;

                        return Convert.ToDecimal(resultado);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener límite de despacho: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // MÉTODO: Listar el catálogo completo de permisos contextuales activos (REGISTRO, DESPACHO_EMPLEADO, DESPACHO_BODEGA)
        public static List<Mdl_WarehousePermission> ObtenerCatalogoPermisos()
        {
            List<Mdl_WarehousePermission> lista = new List<Mdl_WarehousePermission>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT WarehousePermissionId, PermissionCode, PermissionName
                        FROM WarehousePermissions
                        WHERE IsActive = 1
                        ORDER BY PermissionName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Mdl_WarehousePermission
                            {
                                WarehousePermissionId = Convert.ToInt32(reader["WarehousePermissionId"]),
                                PermissionCode = reader["PermissionCode"].ToString(),
                                PermissionName = reader["PermissionName"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener catálogo de permisos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO: Listar los permisos ya otorgados (IsGranted=1) para un WarehouseManagerId específico
        public static List<Mdl_WarehouseManagerPermission> ObtenerPermisosAsignados(int warehouseManagerId)
        {
            List<Mdl_WarehouseManagerPermission> lista = new List<Mdl_WarehouseManagerPermission>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT wmp.WarehouseManagerPermissionId, wmp.WarehouseManagerId, wmp.WarehousePermissionId,
                            wp.PermissionCode, wp.PermissionName, wmp.MaxQuantityPerDispatch, wmp.IsGranted
                        FROM WarehouseManagerPermissions wmp
                        INNER JOIN WarehousePermissions wp ON wp.WarehousePermissionId = wmp.WarehousePermissionId
                        WHERE wmp.WarehouseManagerId = @WarehouseManagerId
                          AND wmp.IsGranted = 1
                          AND wp.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@WarehouseManagerId", warehouseManagerId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Mdl_WarehouseManagerPermission
                                {
                                    WarehouseManagerPermissionId = Convert.ToInt32(reader["WarehouseManagerPermissionId"]),
                                    WarehouseManagerId = Convert.ToInt32(reader["WarehouseManagerId"]),
                                    WarehousePermissionId = Convert.ToInt32(reader["WarehousePermissionId"]),
                                    PermissionCode = reader["PermissionCode"].ToString(),
                                    PermissionName = reader["PermissionName"].ToString(),
                                    MaxQuantityPerDispatch = reader["MaxQuantityPerDispatch"] == DBNull.Value
                                        ? (decimal?)null : Convert.ToDecimal(reader["MaxQuantityPerDispatch"]),
                                    IsGranted = Convert.ToBoolean(reader["IsGranted"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener permisos asignados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO: Otorgar un permiso contextual a un WarehouseManager (inserta si no existe, reactiva si existía inactivo)
        // Retornos: 1=éxito, -1=manager inactivo/inexistente, -2=ya estaba otorgado, 0=error
        public static int AsignarPermiso(int warehouseManagerId, int warehousePermissionId, decimal? maxQuantityPerDispatch, int createdBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_WarehouseManagerPermission_Insert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@WarehouseManagerId", warehouseManagerId);
                    cmd.Parameters.AddWithValue("@WarehousePermissionId", warehousePermissionId);
                    cmd.Parameters.AddWithValue("@MaxQuantityPerDispatch",
                        maxQuantityPerDispatch.HasValue ? (object)maxQuantityPerDispatch.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar permiso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO: Actualizar IsGranted y/o MaxQuantityPerDispatch de un permiso ya existente
        public static int ActualizarPermiso(int warehouseManagerPermissionId, bool isGranted, decimal? maxQuantityPerDispatch)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_WarehouseManagerPermission_Update", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@WarehouseManagerPermissionId", warehouseManagerPermissionId);
                    cmd.Parameters.AddWithValue("@IsGranted", isGranted);
                    cmd.Parameters.AddWithValue("@MaxQuantityPerDispatch",
                        maxQuantityPerDispatch.HasValue ? (object)maxQuantityPerDispatch.Value : DBNull.Value);

                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar permiso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO: Quitar (IsGranted=false) un permiso específico por su WarehouseManagerPermissionId
        public static int QuitarPermiso(int warehouseManagerPermissionId)
        {
            return ActualizarPermiso(warehouseManagerPermissionId, false, null);
        }

        // MÉTODO: Obtener las bodegas donde el usuario tiene ADMIN_BODEGA (para filtrar acceso al formulario de gestión)
        public static List<int> ObtenerBodegasComoAdminBodega(int userId)
        {
            List<int> lista = new List<int>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT wm.WarehouseId
                FROM WarehouseManagers wm
                INNER JOIN WarehouseManagerPermissions wmp ON wmp.WarehouseManagerId = wm.WarehouseManagerId
                INNER JOIN WarehousePermissions wp ON wp.WarehousePermissionId = wmp.WarehousePermissionId
                WHERE wm.UserId = @UserId
                  AND wm.IsActive = 1
                  AND wmp.IsGranted = 1
                  AND wp.PermissionCode = 'ADMIN_BODEGA'
                  AND wp.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(Convert.ToInt32(reader["WarehouseId"]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener bodegas administradas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
    }
}