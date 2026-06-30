using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_ItemMovement
    {
        // MÉTODO: Registrar un ingreso de mercadería (siempre bodega central)
        // MovementTypeId debe corresponder a un tipo de entrada (ej. COMPRA, ENTRADA_AJUSTE)
        public static int RegistrarIngreso(
            int movementTypeId,
            int warehouseId,
            List<Mdl_ItemMovementDetailInput> items,
            int createdBy,
            int? supplierId = null,
            string referenceDocument = null,
            string remarks = null)
        {
            string itemsJson = ConstruirItemsJson(items);
            return EjecutarItemMovementInsert(movementTypeId, warehouseId, null, supplierId,
                referenceDocument, remarks, itemsJson, createdBy);
        }

        // MÉTODO AUXILIAR: Construye el JSON de items manualmente (sin librerías externas)
        public static string ConstruirItemsJson(List<Mdl_ItemMovementDetailInput> items)
        {
            var sb = new StringBuilder();
            sb.Append("[");

            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                if (i > 0) sb.Append(",");

                sb.Append("{");
                sb.Append("\"ItemId\":").Append(it.ItemId).Append(",");
                sb.Append("\"Quantity\":").Append(it.Quantity.ToString(CultureInfo.InvariantCulture)).Append(",");
                sb.Append("\"UnitCost\":").Append(it.UnitCost.HasValue
                    ? it.UnitCost.Value.ToString(CultureInfo.InvariantCulture) : "null").Append(",");
                sb.Append("\"LotNumber\":").Append(EscaparONulo(it.LotNumber)).Append(",");
                sb.Append("\"ExpiryDate\":").Append(it.ExpiryDate.HasValue
                    ? "\"" + it.ExpiryDate.Value.ToString("yyyy-MM-dd") + "\"" : "null").Append(",");
                sb.Append("\"Remarks\":").Append(EscaparONulo(it.Remarks));
                sb.Append("}");
            }

            sb.Append("]");
            return sb.ToString();
        }

        private static string EscaparONulo(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return "null";

            string escapado = valor
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "")
                .Replace("\n", " ");

            return "\"" + escapado + "\"";
        }

        // MÉTODO: Obtener tipos de movimiento que afectan stock positivamente (entradas)
        public static List<KeyValuePair<int, string>> ObtenerTiposDeEntrada()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT MovementTypeId, TypeName
                        FROM MovementTypes
                        WHERE AffectsStock = '+' AND IsActive = 1
                        ORDER BY TypeName";

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
                MessageBox.Show("Error al obtener tipos de movimiento: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO: Ejecutar SP_ItemMovement_Insert directamente (sin PurchaseDate, compatibilidad)
        public static int EjecutarItemMovementInsert(
            int movementTypeId,
            int warehouseId,
            int? destinationWarehouseId,
            int? supplierId,
            string referenceDocument,
            string remarks,
            string itemsJson,
            int createdBy)
        {
            return EjecutarItemMovementInsertConFecha(movementTypeId, warehouseId, destinationWarehouseId,
                supplierId, referenceDocument, null, remarks, itemsJson, createdBy);
        }

        // MÉTODO: Ejecutar SP_ItemMovement_Insert con soporte de PurchaseDate (fecha de factura)
        public static int EjecutarItemMovementInsertConFecha(
            int movementTypeId,
            int warehouseId,
            int? destinationWarehouseId,
            int? supplierId,
            string referenceDocument,
            DateTime? purchaseDate,
            string remarks,
            string itemsJson,
            int createdBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_ItemMovement_Insert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovementTypeId", movementTypeId);
                    cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                    cmd.Parameters.AddWithValue("@DestinationWarehouseId",
                        destinationWarehouseId.HasValue ? (object)destinationWarehouseId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@SupplierId",
                        supplierId.HasValue ? (object)supplierId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReferenceDocument",
                        string.IsNullOrWhiteSpace(referenceDocument) ? (object)DBNull.Value : referenceDocument);
                    cmd.Parameters.AddWithValue("@PurchaseDate",
                        purchaseDate.HasValue ? (object)purchaseDate.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks",
                        string.IsNullOrWhiteSpace(remarks) ? (object)DBNull.Value : remarks);
                    cmd.Parameters.AddWithValue("@ItemsJson", itemsJson);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar movimiento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
        // MÉTODO: Transferencia entre bodegas (resuelve MovementTypeId = TRANSFERENCIA internamente)
        public static int EjecutarTransferenciaEntreBodegas(
            int warehouseId, 
            int destinationWarehouseId, 
            string itemsJson, 
            int createdBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_WarehouseDispatch_Create", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                    cmd.Parameters.AddWithValue("@PermissionCode", "DESPACHO_BODEGA");
                    cmd.Parameters.AddWithValue("@DestinationWarehouseId", destinationWarehouseId);
                    cmd.Parameters.AddWithValue("@DestinationEmployeeId", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReferenceDocument", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ItemsJson", itemsJson);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar transferencia: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
}