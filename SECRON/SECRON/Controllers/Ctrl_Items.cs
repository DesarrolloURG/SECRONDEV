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
    internal class Ctrl_Items
    {
        // MÉTODO PRINCIPAL: Registrar artículo
        public static int RegistrarArticulo(Mdl_Items item)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Items (ItemCode, ItemName, Description, CategoryId, UnitId, 
                        MinimumStock, MaximumStock, ReorderPoint, UnitCost, LastPurchasePrice, 
                        HasLotControl, HasExpiryDate, IsActive, CreatedBy) 
                        VALUES (@ItemCode, @ItemName, @Description, @CategoryId, @UnitId, @MinimumStock, 
                        @MaximumStock, @ReorderPoint, @UnitCost, @LastPurchasePrice, @HasLotControl, 
                        @HasExpiryDate, @IsActive, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemCode", item.ItemCode ?? "");
                        cmd.Parameters.AddWithValue("@ItemName", item.ItemName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
                        cmd.Parameters.AddWithValue("@UnitId", item.UnitId);
                        cmd.Parameters.AddWithValue("@MinimumStock", item.MinimumStock);
                        cmd.Parameters.AddWithValue("@MaximumStock", (object)item.MaximumStock ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReorderPoint", (object)item.ReorderPoint ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UnitCost", item.UnitCost);
                        cmd.Parameters.AddWithValue("@LastPurchasePrice", (object)item.LastPurchasePrice ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HasLotControl", item.HasLotControl);
                        cmd.Parameters.AddWithValue("@HasExpiryDate", item.HasExpiryDate);
                        cmd.Parameters.AddWithValue("@IsActive", item.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)item.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar artículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar artículos con paginación
        public static List<Mdl_Items> MostrarArticulos(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_Items> lista = new List<Mdl_Items>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM Items WHERE IsActive = 1 
                        ORDER BY ItemName 
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearArticulo(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener artículos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Búsqueda con filtros
        public static List<Mdl_Items> BuscarArticulos(
    string textoBusqueda = "",
    int? categoryId = null,
    string filtro1 = "TODOS",
    string filtro3 = "TODOS",
    int pageNumber = 1,
    int pageSize = 100)
        {
            List<Mdl_Items> lista = new List<Mdl_Items>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Items WHERE 1=1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    // Filtro3: estado
                    if (filtro3 == "SOLO ACTIVOS")
                        query += " AND IsActive = 1";
                    else if (filtro3 == "SOLO INACTIVOS")
                        query += " AND IsActive = 0";

                    // Filtro1: campo de búsqueda
                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        if (filtro1 == "POR CÓDIGO")
                            query += " AND ItemCode LIKE @texto";
                        else if (filtro1 == "POR NOMBRE")
                            query += " AND ItemName LIKE @texto";
                        else if (filtro1 == "POR DESCRIPCIÓN")
                            query += " AND Description LIKE @texto";
                        else
                            query += " AND (ItemCode LIKE @texto OR ItemName LIKE @texto OR Description LIKE @texto)";

                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (categoryId.HasValue && categoryId > 0)
                    {
                        query += " AND CategoryId = @categoryId";
                        parametros.Add(new SqlParameter("@categoryId", categoryId.Value));
                    }

                    query += " ORDER BY ItemName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearArticulo(reader));
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

        // MÉTODO PRINCIPAL: Actualizar artículo
        public static int ActualizarArticulo(Mdl_Items item)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Items SET ItemCode = @ItemCode, ItemName = @ItemName, 
                        Description = @Description, CategoryId = @CategoryId, UnitId = @UnitId, 
                        MinimumStock = @MinimumStock, MaximumStock = @MaximumStock, ReorderPoint = @ReorderPoint, 
                        UnitCost = @UnitCost, LastPurchasePrice = @LastPurchasePrice, HasLotControl = @HasLotControl, 
                        HasExpiryDate = @HasExpiryDate, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy 
                        WHERE ItemId = @ItemId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", item.ItemId);
                        cmd.Parameters.AddWithValue("@ItemCode", item.ItemCode ?? "");
                        cmd.Parameters.AddWithValue("@ItemName", item.ItemName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
                        cmd.Parameters.AddWithValue("@UnitId", item.UnitId);
                        cmd.Parameters.AddWithValue("@MinimumStock", item.MinimumStock);
                        cmd.Parameters.AddWithValue("@MaximumStock", (object)item.MaximumStock ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReorderPoint", (object)item.ReorderPoint ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UnitCost", item.UnitCost);
                        cmd.Parameters.AddWithValue("@LastPurchasePrice", (object)item.LastPurchasePrice ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HasLotControl", item.HasLotControl);
                        cmd.Parameters.AddWithValue("@HasExpiryDate", item.HasExpiryDate);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)item.ModifiedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar artículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar artículo
        public static int InactivarArticulo(int itemId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Items SET IsActive = 0, ModifiedDate = GETDATE(), 
                        ModifiedBy = @ModifiedBy WHERE ItemId = @ItemId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar artículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener artículo por ID
        public static Mdl_Items ObtenerArticuloPorId(int itemId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Items WHERE ItemId = @ItemId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearArticulo(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener artículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO AUXILIAR: Mapear artículo
        private static Mdl_Items MapearArticulo(SqlDataReader reader)
        {
            return new Mdl_Items
            {
                ItemId = reader.GetInt32(0),
                ItemCode = reader[1].ToString(),
                ItemName = reader[2].ToString(),
                Description = reader[3] == DBNull.Value ? null : reader[3].ToString(),
                CategoryId = reader.GetInt32(4),
                UnitId = reader.GetInt32(5),
                MinimumStock = reader.GetDecimal(6),
                MaximumStock = reader[7] == DBNull.Value ? 0 : reader.GetDecimal(7),
                ReorderPoint = reader[8] == DBNull.Value ? 0 : reader.GetDecimal(8),
                UnitCost = reader.GetDecimal(9),
                LastPurchasePrice = reader[10] == DBNull.Value ? 0 : reader.GetDecimal(10),
                HasLotControl = reader.GetBoolean(11),
                HasExpiryDate = reader.GetBoolean(12),
                IsActive = reader.GetBoolean(13),
                CreatedDate = reader.GetDateTime(14),
                CreatedBy = reader[15] == DBNull.Value ? null : (int?)reader.GetInt32(15),
                ModifiedDate = reader[16] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(16),
                ModifiedBy = reader[17] == DBNull.Value ? null : (int?)reader.GetInt32(17)
            };
        }

        // MÉTODO PARA OBTENER ARTÍCULOS PARA COMBOBOX
        public static List<KeyValuePair<int, string>> ObtenerArticulosParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT ItemId, ItemName FROM Items WHERE IsActive = 1 ORDER BY ItemName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener artículos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA CONTAR TOTAL
        public static int ContarTotalArticulos(string textoBusqueda = "", int? categoryId = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Items WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += " AND (ItemCode LIKE @texto OR ItemName LIKE @texto OR Description LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (categoryId.HasValue && categoryId > 0)
                    {
                        query += " AND CategoryId = @categoryId";
                        parametros.Add(new SqlParameter("@categoryId", categoryId.Value));
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
        public static string ObtenerProximoCodigoArticulo()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // Obtener el último código registrado
                    string query = @"SELECT TOP 1 ItemCode 
                             FROM Items 
                             WHERE ItemCode IS NOT NULL 
                             ORDER BY ItemId DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && !string.IsNullOrWhiteSpace(resultado.ToString()))
                        {
                            string ultimoCodigo = resultado.ToString();

                            // Intentar convertir completamente a número (ej: 000001, 000234)
                            if (int.TryParse(ultimoCodigo, out int numeroActual))
                            {
                                int proximoNumero = numeroActual + 1;
                                // Formato de 6 dígitos: 000001, 000002, etc.
                                return proximoNumero.ToString("D6");
                            }
                            else
                            {
                                // Si contiene letras + números (ej: ART000123)
                                string soloNumeros = new string(ultimoCodigo.Where(char.IsDigit).ToArray());

                                if (!string.IsNullOrWhiteSpace(soloNumeros) &&
                                    int.TryParse(soloNumeros, out int numExtraido))
                                {
                                    int proximoNumero = numExtraido + 1;

                                    // Mantener el prefijo de letras (ej: ART)
                                    string prefijo = new string(ultimoCodigo.Where(char.IsLetter).ToArray());
                                    return $"{prefijo}{proximoNumero:D6}";
                                }
                                else
                                {
                                    // Si no se puede extraer número, empezar desde 000001
                                    return "000001";
                                }
                            }
                        }
                        else
                        {
                            // Si no hay registros, empezar desde 000001
                            return "000001";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de artículo: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "ERROR";
            }
        }

    }
}