using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_FixedAssetCategories
    {
        // ─────────────────────────────────────────────
        // READ - paginado
        // ─────────────────────────────────────────────
        public static List<Mdl_FixedAssetCategory> MostrarCategorias(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_FixedAssetCategory> lista = new List<Mdl_FixedAssetCategory>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                SELECT AssetCategoryId, CategoryCode, CategoryName, Description,
                       IsTangible, DepreciationMethod, DepreciationYears,
                       AccountAccumDepId, AccountExpenseId,
                       IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy,
                       ClassificationId
                FROM   FixedAssetCategories
                WHERE  IsActive = 1
                ORDER  BY CategoryName
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearCategoria(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener categorías: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // ─────────────────────────────────────────────
        // SEARCH con filtros
        // ─────────────────────────────────────────────
        public static List<Mdl_FixedAssetCategory> BuscarCategorias(
    string textoBusqueda = "",
    string filtro1 = "TODOS",
    string filtroEstado = "TODOS",
    string filtroTipo = "TODOS",
    int pageNumber = 1,
    int pageSize = 100)
        {
            List<Mdl_FixedAssetCategory> lista = new List<Mdl_FixedAssetCategory>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                SELECT AssetCategoryId, CategoryCode, CategoryName, Description,
                       IsTangible, DepreciationMethod, DepreciationYears,
                       AccountAccumDepId, AccountExpenseId,
                       IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy,
                       ClassificationId
                FROM   FixedAssetCategories
                WHERE  1=1";

                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (filtroEstado == "SOLO ACTIVOS")
                        query += " AND IsActive = 1";
                    else if (filtroEstado == "SOLO INACTIVOS")
                        query += " AND IsActive = 0";

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        if (filtro1 == "POR CÓDIGO")
                            query += " AND CategoryCode LIKE @texto";
                        else if (filtro1 == "POR NOMBRE")
                            query += " AND CategoryName LIKE @texto";
                        else
                            query += " AND (CategoryCode LIKE @texto OR CategoryName LIKE @texto)";

                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (filtroTipo == "TANGIBLE")
                        query += " AND IsTangible = 1";
                    else if (filtroTipo == "INTANGIBLE")
                        query += " AND IsTangible = 0";

                    query += " ORDER BY CategoryName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearCategoria(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar categorías: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        public static int ContarTotalCategorias(string textoBusqueda = "", string filtro1 = "TODOS",
        string filtroEstado = "SOLO ACTIVOS", string filtroTipo = "TODOS")
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM FixedAssetCategories WHERE 1=1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (filtroEstado == "SOLO ACTIVOS")
                        query += " AND IsActive = 1";
                    else if (filtroEstado == "SOLO INACTIVOS")
                        query += " AND IsActive = 0";

                    if (filtroTipo == "TANGIBLE")
                        query += " AND IsTangible = 1";
                    else if (filtroTipo == "INTANGIBLE")
                        query += " AND IsTangible = 0";

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        if (filtro1 == "POR CÓDIGO")
                            query += " AND CategoryCode LIKE @texto";
                        else if (filtro1 == "POR NOMBRE")
                            query += " AND CategoryName LIKE @texto";
                        else
                            query += " AND (CategoryCode LIKE @texto OR CategoryName LIKE @texto)";

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

        // ─────────────────────────────────────────────
        // COMBO — para otros formularios que necesiten categorías de AF
        // ─────────────────────────────────────────────
        public static List<KeyValuePair<int, string>> ObtenerCategoriasParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT AssetCategoryId, CategoryName
                        FROM   FixedAssetCategories
                        WHERE  IsActive = 1
                        ORDER  BY CategoryName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(new KeyValuePair<int, string>(
                                reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener categorías para combo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // ─────────────────────────────────────────────
        // CREATE
        // ─────────────────────────────────────────────
        public static int RegistrarCategoria(Mdl_FixedAssetCategory cat)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetCategories_Insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@CategoryCode", cat.CategoryCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@CategoryName", cat.CategoryName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)cat.Description?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsTangible", cat.IsTangible);
                        cmd.Parameters.AddWithValue("@DepreciationMethod", cat.DepreciationMethod ?? "LINEA_RECTA");
                        cmd.Parameters.AddWithValue("@DepreciationYears", cat.DepreciationYears);
                        cmd.Parameters.AddWithValue("@AccountAccumDepId", cat.AccountAccumDepId);
                        cmd.Parameters.AddWithValue("@AccountExpenseId", cat.AccountExpenseId);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)cat.CreatedBy ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ClassificationId", (object)cat.ClassificationId ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar categoría: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // ─────────────────────────────────────────────
        // UPDATE
        // ─────────────────────────────────────────────
        public static int ActualizarCategoria(Mdl_FixedAssetCategory cat)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FixedAssetCategories_Update", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AssetCategoryId", cat.AssetCategoryId);
                        cmd.Parameters.AddWithValue("@CategoryCode", cat.CategoryCode?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@CategoryName", cat.CategoryName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)cat.Description?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsTangible", cat.IsTangible);
                        cmd.Parameters.AddWithValue("@DepreciationMethod", cat.DepreciationMethod ?? "LINEA_RECTA");
                        cmd.Parameters.AddWithValue("@DepreciationYears", cat.DepreciationYears);
                        cmd.Parameters.AddWithValue("@AccountAccumDepId", cat.AccountAccumDepId);
                        cmd.Parameters.AddWithValue("@AccountExpenseId", cat.AccountExpenseId);
                        cmd.Parameters.AddWithValue("@IsActive", cat.IsActive);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)cat.ModifiedBy ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ClassificationId", (object)cat.ClassificationId ?? DBNull.Value);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar categoría: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private static Mdl_FixedAssetCategory MapearCategoria(SqlDataReader reader)
        {
            return new Mdl_FixedAssetCategory
            {
                AssetCategoryId = reader.GetInt32(reader.GetOrdinal("AssetCategoryId")),
                CategoryCode = reader["CategoryCode"].ToString(),
                CategoryName = reader["CategoryName"].ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                DepreciationMethod = reader["DepreciationMethod"].ToString(),
                IsTangible = reader.GetBoolean(reader.GetOrdinal("IsTangible")),
                DepreciationYears = reader.GetDecimal(reader.GetOrdinal("DepreciationYears")),
                AccountAccumDepId = reader.GetInt32(reader.GetOrdinal("AccountAccumDepId")),
                AccountExpenseId = reader.GetInt32(reader.GetOrdinal("AccountExpenseId")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ModifiedBy")),
                ClassificationId = reader["ClassificationId"] == DBNull.Value ? (int?)null : reader.GetInt32(reader.GetOrdinal("ClassificationId"))
            };
        }
        // Método nuevo — obtener categorías por clasificación para el ComboBox
        public static List<KeyValuePair<int, string>> ObtenerCategoriasPorClasificacion(int classificationId)
        {
            var lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                SELECT AssetCategoryId, CategoryName
                FROM   FixedAssetCategories
                WHERE  ClassificationId = @ClassificationId
                AND    IsActive = 1
                ORDER  BY CategoryName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ClassificationId", classificationId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(new KeyValuePair<int, string>(
                                    reader.GetInt32(0),
                                    reader["CategoryName"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener categorías por clasificación: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
    }
}