using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_WarehouseDispatch
    {
        // MÉTODO: Despachar artículos desde la bodega propia del encargado
        // No registra destinatario. Valida permiso y límite por artículo dentro del SP.
        // MÉTODO: Despachar artículos desde la bodega propia del encargado (regla 6 y 7)
        // destinationEmployeeId es opcional: identifica a quién se le entregó (nivel master)
        public static int RegistrarDespacho(
            int warehouseId,
            List<Mdl_ItemMovementDetailInput> items,
            int createdBy,
            string referenceDocument = null,
            string remarks = null,
            int? destinationEmployeeId = null)
        {
            string itemsJson = Ctrl_ItemMovement.ConstruirItemsJson(items);

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_WarehouseDispatch_Create", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                    cmd.Parameters.AddWithValue("@ReferenceDocument",
                        string.IsNullOrWhiteSpace(referenceDocument) ? (object)DBNull.Value : referenceDocument);
                    cmd.Parameters.AddWithValue("@Remarks",
                        string.IsNullOrWhiteSpace(remarks) ? (object)DBNull.Value : remarks);
                    cmd.Parameters.AddWithValue("@ItemsJson", itemsJson);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                    cmd.Parameters.AddWithValue("@DestinationEmployeeId",
                        destinationEmployeeId.HasValue ? (object)destinationEmployeeId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@PermissionCode", "DESPACHO_EMPLEADO");
                    cmd.Parameters.AddWithValue("@DestinationWarehouseId", DBNull.Value);

                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar despacho: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
}