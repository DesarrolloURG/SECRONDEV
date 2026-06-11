using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_FixedAssets
    {


        public static List<Mdl_FixedAsset> MostrarActivos(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_FixedAsset> lista = new List<Mdl_FixedAsset>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT fa.AssetId, fa.AssetCode, fa.AssetName, fa.Description,
                               fa.AssetCategoryId, 
                               (SELECT av.Value FROM FixedAssetAttributeValues av
                                 INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                                 WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'BRAND' AND ad.IsSystem = 1) AS Brand, 
                               (SELECT av.Value FROM FixedAssetAttributeValues av
                                 INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                                 WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'MODEL' AND ad.IsSystem = 1) AS Model,
                                (SELECT av.Value FROM FixedAssetAttributeValues av
                                 INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                                 WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'SERIAL' AND ad.IsSystem = 1) AS Serial,
                               fa.PurchaseDate, fa.PurchaseValue, fa.ResidualValue,
                               fa.InvoiceNumber, fa.SupplierId,
                               fa.WarrantyDocumentPath, fa.WarrantyExpirationDate,
                               fa.DepreciationStartDate, fa.ResidualValueAct,
                               fa.CurrentWarehouseId, fa.AssignedToEmployeeId,
                               fa.AssetStatus, fa.DisposalDate, fa.DisposalReason,
                               fa.DisposalValue, fa.Notes,
                               fa.IsActive, fa.CreatedDate, fa.CreatedBy,
                               fa.ModifiedDate, fa.ModifiedBy,
                               fac.CategoryName,
                               s.SupplierName,
                               w.WarehouseName,
                               ISNULL(e.FullName, ISNULL(e.FirstName + ' ' + e.LastName, '')) AS EmployeeName
                        FROM   FixedAssets fa
                        LEFT JOIN FixedAssetCategories      fac ON fa.AssetCategoryId     = fac.AssetCategoryId
                        LEFT JOIN Suppliers                 s   ON fa.SupplierId           = s.SupplierId
                        LEFT JOIN Warehouses                w   ON fa.CurrentWarehouseId   = w.WarehouseId
                        LEFT JOIN Employees                 e   ON fa.AssignedToEmployeeId = e.EmployeeId
                        WHERE  fa.IsActive = 1
                        ORDER  BY fa.AssetCode
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearActivo(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener activos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }


        public static List<Mdl_FixedAsset> BuscarActivos(
    string textoBusqueda = "",
    string filtro1 = "TODOS",
    string filtroEstado = "TODOS",
    int? categoriaId = null,
    int? classificationId = null,
    string assetStatus = null,
    int? employeeId = null,
    int pageNumber = 1,
    int pageSize = 100)
        {
            List<Mdl_FixedAsset> lista = new List<Mdl_FixedAsset>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                SELECT fa.AssetId, fa.AssetCode, fa.AssetName, fa.Description,
                       fa.AssetCategoryId,
                       (SELECT av.Value FROM FixedAssetAttributeValues av
                         INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                         WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'BRAND' AND ad.IsSystem = 1) AS Brand,
                       (SELECT av.Value FROM FixedAssetAttributeValues av
                         INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                         WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'MODEL' AND ad.IsSystem = 1) AS Model,
                       (SELECT av.Value FROM FixedAssetAttributeValues av
                         INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                         WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'SERIAL' AND ad.IsSystem = 1) AS Serial,
                       fa.PurchaseDate, fa.PurchaseValue, fa.ResidualValue,
                       fa.InvoiceNumber, fa.SupplierId,
                       fa.WarrantyDocumentPath, fa.WarrantyExpirationDate,
                       fa.DepreciationStartDate, fa.ResidualValueAct,
                       fa.CurrentWarehouseId, fa.AssignedToEmployeeId,
                       fa.AssetStatus, fa.DisposalDate, fa.DisposalReason,
                       fa.DisposalValue, fa.Notes,
                       fa.IsActive, fa.CreatedDate, fa.CreatedBy,
                       fa.ModifiedDate, fa.ModifiedBy,
                       fac.CategoryName,
                       s.SupplierName,
                       w.WarehouseName,
                       ISNULL(e.FullName, ISNULL(e.FirstName + ' ' + e.LastName, '')) AS EmployeeName
                FROM   FixedAssets fa
                LEFT JOIN FixedAssetCategories fac ON fa.AssetCategoryId     = fac.AssetCategoryId
                LEFT JOIN Suppliers            s   ON fa.SupplierId           = s.SupplierId
                LEFT JOIN Warehouses           w   ON fa.CurrentWarehouseId   = w.WarehouseId
                LEFT JOIN Employees            e   ON fa.AssignedToEmployeeId = e.EmployeeId
                WHERE  1=1";

                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (filtroEstado == "SOLO ACTIVOS")
                        query += " AND fa.IsActive = 1";
                    else if (filtroEstado == "SOLO INACTIVOS")
                        query += " AND fa.IsActive = 0";

                    if (!string.IsNullOrWhiteSpace(assetStatus))
                    {
                        query += " AND fa.AssetStatus = @assetStatus";
                        parametros.Add(new SqlParameter("@assetStatus", assetStatus));
                    }

                    if (categoriaId.HasValue)
                    {
                        query += " AND fa.AssetCategoryId = @categoriaId";
                        parametros.Add(new SqlParameter("@categoriaId", categoriaId.Value));
                    }

                    if (classificationId.HasValue)
                    {
                        query += " AND fac.ClassificationId = @classificationId";
                        parametros.Add(new SqlParameter("@classificationId", classificationId.Value));
                    }

                    if (employeeId.HasValue)
                    {
                        query += " AND fa.AssignedToEmployeeId = @employeeId";
                        parametros.Add(new SqlParameter("@employeeId", employeeId.Value));
                    }

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        if (filtro1 == "POR CÓDIGO")
                            query += " AND fa.AssetCode LIKE @texto";
                        else if (filtro1 == "POR NOMBRE")
                            query += " AND fa.AssetName LIKE @texto";
                        else if (filtro1 == "POR SERIE")
                            query += @" AND EXISTS (
                        SELECT 1 FROM FixedAssetAttributeValues av
                        INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                        WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'SERIAL' AND ad.IsSystem = 1
                        AND av.Value LIKE @texto)";
                        else
                            query += @" AND (fa.AssetCode LIKE @texto OR fa.AssetName LIKE @texto
                        OR EXISTS (
                            SELECT 1 FROM FixedAssetAttributeValues av
                            INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                            WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'SERIAL' AND ad.IsSystem = 1
                            AND av.Value LIKE @texto))";

                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    query += @" ORDER BY fa.AssetCode
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearActivo(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar activos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static int ContarTotalActivos(string textoBusqueda = "", string filtroEstado = "SOLO ACTIVOS", int? categoriaId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM FixedAssets fa WHERE 1=1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (filtroEstado == "SOLO ACTIVOS")
                        query += " AND fa.IsActive = 1";
                    else if (filtroEstado == "SOLO INACTIVOS")
                        query += " AND fa.IsActive = 0";

                    if (categoriaId.HasValue)
                    {
                        query += " AND fa.AssetCategoryId = @categoriaId";
                        parametros.Add(new SqlParameter("@categoriaId", categoriaId.Value));
                    }

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (fa.AssetCode LIKE @texto OR fa.AssetName LIKE @texto
                    OR EXISTS (
                        SELECT 1 FROM FixedAssetAttributeValues av
                        INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                        WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'SERIAL' AND ad.IsSystem = 1
                        AND av.Value LIKE @texto))";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
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

        public static Mdl_FixedAsset ObtenerActivoPorId(int assetId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT fa.AssetId, fa.AssetCode, fa.AssetName, fa.Description,
                               fa.AssetCategoryId, 
                                (SELECT av.Value FROM FixedAssetAttributeValues av
                                 INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                                 WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'BRAND' AND ad.IsSystem = 1) AS Brand, 
                               (SELECT av.Value FROM FixedAssetAttributeValues av
                                 INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                                 WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'MODEL' AND ad.IsSystem = 1) AS Model,
                                (SELECT av.Value FROM FixedAssetAttributeValues av
                                 INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
                                 WHERE av.AssetId = fa.AssetId AND ad.AttributeKey = 'SERIAL' AND ad.IsSystem = 1) AS Serial,
                               fa.PurchaseDate, fa.PurchaseValue, fa.ResidualValue,
                               fa.InvoiceNumber, fa.SupplierId,
                               fa.WarrantyDocumentPath, fa.WarrantyExpirationDate,
                               fa.DepreciationStartDate, fa.ResidualValueAct,
                               fa.CurrentWarehouseId, fa.AssignedToEmployeeId,
                               fa.AssetStatus, fa.DisposalDate, fa.DisposalReason,
                               fa.DisposalValue, fa.Notes,
                               fa.IsActive, fa.CreatedDate, fa.CreatedBy,
                               fa.ModifiedDate, fa.ModifiedBy,
                               fac.CategoryName,
                               s.SupplierName,
                               w.WarehouseName,
                               ISNULL(e.FullName, ISNULL(e.FirstName + ' ' + e.LastName, '')) AS EmployeeName
                        FROM   FixedAssets fa
                        LEFT JOIN FixedAssetCategories      fac ON fa.AssetCategoryId     = fac.AssetCategoryId
                        LEFT JOIN Suppliers                 s   ON fa.SupplierId           = s.SupplierId
                        LEFT JOIN Warehouses                w   ON fa.CurrentWarehouseId   = w.WarehouseId
                        LEFT JOIN Employees                 e   ON fa.AssignedToEmployeeId = e.EmployeeId
                        WHERE fa.AssetId = @AssetId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AssetId", assetId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapearActivo(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener activo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }


        public static int RegistrarActivo(Mdl_FixedAsset asset)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssets_Insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        AgregarParametros(cmd, asset);
                        object resultado = cmd.ExecuteScalar();
                        return resultado != null ? Convert.ToInt32(resultado) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar activo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarActivo(Mdl_FixedAsset asset)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssets_Update", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AssetId", asset.AssetId);
                        cmd.Parameters.AddWithValue("@AssetCode", asset.AssetCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@AssetName", asset.AssetName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(asset.Description) ? (object)DBNull.Value : asset.Description.ToUpper());
                        cmd.Parameters.AddWithValue("@AssetCategoryId", asset.AssetCategoryId);
                        cmd.Parameters.AddWithValue("@PurchaseDate", asset.PurchaseDate.HasValue ? (object)asset.PurchaseDate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@PurchaseValue", asset.PurchaseValue);
                        cmd.Parameters.AddWithValue("@ResidualValue", asset.ResidualValue);
                        cmd.Parameters.AddWithValue("@InvoiceNumber", string.IsNullOrWhiteSpace(asset.InvoiceNumber) ? (object)DBNull.Value : asset.InvoiceNumber.ToUpper());
                        cmd.Parameters.AddWithValue("@SupplierId", asset.SupplierId.HasValue ? (object)asset.SupplierId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@WarrantyDocumentPath", string.IsNullOrWhiteSpace(asset.WarrantyDocumentPath) ? (object)DBNull.Value : asset.WarrantyDocumentPath);
                        cmd.Parameters.AddWithValue("@WarrantyExpirationDate", asset.WarrantyExpirationDate.HasValue ? (object)asset.WarrantyExpirationDate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@DepreciationStartDate", asset.DepreciationStartDate.HasValue ? (object)asset.DepreciationStartDate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@CurrentWarehouseId", asset.CurrentWarehouseId.HasValue ? (object)asset.CurrentWarehouseId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@AssignedToEmployeeId", asset.AssignedToEmployeeId.HasValue ? (object)asset.AssignedToEmployeeId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@AssetStatus", asset.AssetStatus ?? "ACTIVO");
                        cmd.Parameters.AddWithValue("@DisposalDate", asset.DisposalDate.HasValue ? (object)asset.DisposalDate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@DisposalReason", string.IsNullOrWhiteSpace(asset.DisposalReason) ? (object)DBNull.Value : asset.DisposalReason.ToUpper());
                        cmd.Parameters.AddWithValue("@DisposalValue", asset.DisposalValue.HasValue ? (object)asset.DisposalValue.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Notes", string.IsNullOrWhiteSpace(asset.Notes) ? (object)DBNull.Value : asset.Notes.ToUpper());
                        cmd.Parameters.AddWithValue("@IsActive", asset.IsActive);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)asset.ModifiedBy ?? DBNull.Value);
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar activo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int EliminarActivo(int assetId, int deletedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssets_Delete", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AssetId", assetId);
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar activo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private static void AgregarParametros(SqlCommand cmd, Mdl_FixedAsset asset)
        {
            cmd.Parameters.AddWithValue("@AssetName", asset.AssetName?.ToUpper() ?? "");
            cmd.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(asset.Description) ? (object)DBNull.Value : asset.Description.ToUpper());
            cmd.Parameters.AddWithValue("@AssetCategoryId", asset.AssetCategoryId);
            cmd.Parameters.AddWithValue("@PurchaseDate", asset.PurchaseDate.HasValue ? (object)asset.PurchaseDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@PurchaseValue", asset.PurchaseValue);
            cmd.Parameters.AddWithValue("@ResidualValue", asset.ResidualValue);
            cmd.Parameters.AddWithValue("@InvoiceNumber", string.IsNullOrWhiteSpace(asset.InvoiceNumber) ? (object)DBNull.Value : asset.InvoiceNumber.ToUpper());
            cmd.Parameters.AddWithValue("@SupplierId", asset.SupplierId.HasValue ? (object)asset.SupplierId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@WarrantyDocumentPath", string.IsNullOrWhiteSpace(asset.WarrantyDocumentPath) ? (object)DBNull.Value : asset.WarrantyDocumentPath);
            cmd.Parameters.AddWithValue("@WarrantyExpirationDate", asset.WarrantyExpirationDate.HasValue ? (object)asset.WarrantyExpirationDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@DepreciationStartDate", asset.DepreciationStartDate.HasValue ? (object)asset.DepreciationStartDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@CurrentWarehouseId", asset.CurrentWarehouseId.HasValue ? (object)asset.CurrentWarehouseId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@AssignedToEmployeeId", asset.AssignedToEmployeeId.HasValue ? (object)asset.AssignedToEmployeeId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@AssetStatus", asset.AssetStatus ?? "ACTIVO");
            cmd.Parameters.AddWithValue("@DisposalDate", asset.DisposalDate.HasValue ? (object)asset.DisposalDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@DisposalReason", string.IsNullOrWhiteSpace(asset.DisposalReason) ? (object)DBNull.Value : asset.DisposalReason.ToUpper());
            cmd.Parameters.AddWithValue("@DisposalValue", asset.DisposalValue.HasValue ? (object)asset.DisposalValue.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", string.IsNullOrWhiteSpace(asset.Notes) ? (object)DBNull.Value : asset.Notes.ToUpper());
            cmd.Parameters.AddWithValue("@CreatedBy", asset.CreatedBy.HasValue ? (object)asset.CreatedBy.Value : DBNull.Value);
        }

        private static Mdl_FixedAsset MapearActivo(SqlDataReader reader)
        {
            return new Mdl_FixedAsset
            {
                AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")),
                AssetCode = reader["AssetCode"].ToString(),
                AssetName = reader["AssetName"].ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                AssetCategoryId = reader.GetInt32(reader.GetOrdinal("AssetCategoryId")),
                Brand = reader["Brand"] == DBNull.Value ? null : reader["Brand"].ToString(),
                Model = reader["Model"] == DBNull.Value ? null : reader["Model"].ToString(),
                Serial = reader["Serial"] == DBNull.Value ? null : reader["Serial"].ToString(),
                PurchaseDate = reader["PurchaseDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                PurchaseValue = reader.GetDecimal(reader.GetOrdinal("PurchaseValue")),
                ResidualValue = reader.GetDecimal(reader.GetOrdinal("ResidualValue")),
                InvoiceNumber = reader["InvoiceNumber"] == DBNull.Value ? null : reader["InvoiceNumber"].ToString(),
                SupplierId = reader["SupplierId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("SupplierId")),
                WarrantyDocumentPath = reader["WarrantyDocumentPath"] == DBNull.Value ? null : reader["WarrantyDocumentPath"].ToString(),
                WarrantyExpirationDate = reader["WarrantyExpirationDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("WarrantyExpirationDate")),
                DepreciationStartDate = reader["DepreciationStartDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DepreciationStartDate")),
                ResidualValueAct = reader.GetDecimal(reader.GetOrdinal("ResidualValueAct")),
                CurrentWarehouseId = reader["CurrentWarehouseId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("CurrentWarehouseId")),
                AssignedToEmployeeId = reader["AssignedToEmployeeId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("AssignedToEmployeeId")),
                AssetStatus = reader["AssetStatus"].ToString(),
                DisposalDate = reader["DisposalDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DisposalDate")),
                DisposalReason = reader["DisposalReason"] == DBNull.Value ? null : reader["DisposalReason"].ToString(),
                DisposalValue = reader["DisposalValue"] == DBNull.Value ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("DisposalValue")),
                Notes = reader["Notes"] == DBNull.Value ? null : reader["Notes"].ToString(),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ModifiedBy")),
                CategoryName = reader["CategoryName"] == DBNull.Value ? null : reader["CategoryName"].ToString(),
                SupplierName = reader["SupplierName"] == DBNull.Value ? null : reader["SupplierName"].ToString(),
                WarehouseName = reader["WarehouseName"] == DBNull.Value ? null : reader["WarehouseName"].ToString(),
                EmployeeName = reader["EmployeeName"] == DBNull.Value ? null : reader["EmployeeName"].ToString()
            };
        }

        public static int ActualizarEstadoActivo(int assetId, string nuevoEstado, int? modifiedBy = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                UPDATE FixedAssets SET
                    AssetStatus  = UPPER(@AssetStatus),
                    ModifiedDate = GETDATE(),
                    ModifiedBy   = @ModifiedBy
                WHERE AssetId = @AssetId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AssetId", assetId);
                        cmd.Parameters.AddWithValue("@AssetStatus", nuevoEstado?.ToUpper() ?? "ACTIVO");
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar estado del activo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
        public static int ActualizarAsignacionActivo(
        int assetId, int? empleadoId, int? bodegaId, int? modifiedBy = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                UPDATE FixedAssets SET
                    AssignedToEmployeeId = @EmpleadoId,
                    CurrentWarehouseId   = @BodegaId,
                    ModifiedDate         = GETDATE(),
                    ModifiedBy           = @ModifiedBy
                WHERE AssetId = @AssetId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AssetId", assetId);
                        cmd.Parameters.AddWithValue("@EmpleadoId", (object)empleadoId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BodegaId", (object)bodegaId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0 ? 1 : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar asignación: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
}