using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Controllers
{
    internal class Ctrl_LocationCategories
    {
        // MÉTODO PRINCIPAL: Registrar categoría de sede
        public static int RegistrarCategoria(Mdl_LocationCategories categoria)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO LocationCategories (CategoryCode, CategoryName, Description, IsActive, CreatedBy) 
                        VALUES (@CategoryCode, @CategoryName, @Description, @IsActive, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CategoryCode", categoria.CategoryCode ?? "");
                        cmd.Parameters.AddWithValue("@CategoryName", categoria.CategoryName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)categoria.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", categoria.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)categoria.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar categoría de sede: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todas las categorías activas
        public static List<Mdl_LocationCategories> MostrarCategorias()
        {
            List<Mdl_LocationCategories> lista = new List<Mdl_LocationCategories>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM LocationCategories WHERE IsActive = 1 ORDER BY CategoryName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCategoria(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener categorías de sede: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Actualizar categoría de sede
        public static int ActualizarCategoria(Mdl_LocationCategories categoria)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE LocationCategories SET CategoryCode = @CategoryCode, 
                        CategoryName = @CategoryName, Description = @Description 
                        WHERE LocationCategoryId = @LocationCategoryId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationCategoryId", categoria.LocationCategoryId);
                        cmd.Parameters.AddWithValue("@CategoryCode", categoria.CategoryCode ?? "");
                        cmd.Parameters.AddWithValue("@CategoryName", categoria.CategoryName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)categoria.Description ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar categoría de sede: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar categoría de sede
        public static int InactivarCategoria(int locationCategoryId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE LocationCategories SET IsActive = 0 WHERE LocationCategoryId = @LocationCategoryId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationCategoryId", locationCategoryId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar categoría de sede: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear categoría de sede desde reader
        private static Mdl_LocationCategories MapearCategoria(SqlDataReader reader)
        {
            return new Mdl_LocationCategories
            {
                LocationCategoryId = reader.GetInt32(0),
                CategoryCode = reader[1].ToString(),
                CategoryName = reader[2].ToString(),
                Description = reader[3] == DBNull.Value ? null : reader[3].ToString(),
                IsActive = reader.GetBoolean(4),
                CreatedDate = reader.GetDateTime(5),
                CreatedBy = reader[6] == DBNull.Value ? null : (int?)reader.GetInt32(6)
            };
        }

        // MÉTODO PARA OBTENER CATEGORÍAS PARA COMBOBOX
        public static List<KeyValuePair<int, string>> ObtenerCategoriasParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT LocationCategoryId, CategoryName FROM LocationCategories WHERE IsActive = 1 ORDER BY CategoryName";
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
                MessageBox.Show("Error al obtener categorías de sede: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA OBTENER EL PRÓXIMO CÓDIGO DE CATEGORÍA
        public static string ObtenerProximoCodigoCategoria()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT TOP 1 CategoryCode
                             FROM LocationCategories
                             WHERE CategoryCode IS NOT NULL
                             ORDER BY LocationCategoryId DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && !string.IsNullOrWhiteSpace(resultado.ToString()))
                        {
                            string ultimoCodigo = resultado.ToString();

                            if (int.TryParse(ultimoCodigo, out int numeroActual))
                            {
                                return (numeroActual + 1).ToString("D6");
                            }
                            else
                            {
                                string soloNumeros = new string(ultimoCodigo.Where(char.IsDigit).ToArray());

                                if (!string.IsNullOrWhiteSpace(soloNumeros) &&
                                    int.TryParse(soloNumeros, out int numExtraido))
                                {
                                    string prefijo = new string(ultimoCodigo.Where(char.IsLetter).ToArray());
                                    return $"{prefijo}{(numExtraido + 1):D6}";
                                }
                                else
                                {
                                    return "000001";
                                }
                            }
                        }
                        else
                        {
                            return "000001";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "ERROR";
            }
        }

        // MÉTODO PARA CONTAR TOTAL DE CATEGORÍAS
        public static int ContarTotalCategorias(string textoBusqueda = "")
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM LocationCategories WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += " AND (CategoryCode LIKE @texto OR CategoryName LIKE @texto OR Description LIKE @texto)";
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

        // MÉTODO PRINCIPAL: Buscar categorías con filtro y paginación
        public static List<Mdl_LocationCategories> BuscarCategorias(
            string textoBusqueda = "",
            int pageNumber = 1,
            int pageSize = 100)
        {
            List<Mdl_LocationCategories> lista = new List<Mdl_LocationCategories>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM LocationCategories WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += " AND (CategoryCode LIKE @texto OR CategoryName LIKE @texto OR Description LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    query += " ORDER BY CategoryName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCategoria(reader));
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
    }
}