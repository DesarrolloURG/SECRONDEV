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
    internal class Ctrl_ItemCategories
    {
        // MÉTODO PRINCIPAL: Registrar categoría
        public static int RegistrarCategoria(Mdl_ItemCategories categoria)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO ItemCategories (CategoryCode, CategoryName, Description, IsActive, CreatedBy) 
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
                MessageBox.Show("Error al registrar categoría: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todas las categorías
        public static List<Mdl_ItemCategories> MostrarCategorias()
        {
            List<Mdl_ItemCategories> lista = new List<Mdl_ItemCategories>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM ItemCategories WHERE IsActive = 1 ORDER BY CategoryName";
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
                MessageBox.Show("Error al obtener categorías: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Actualizar categoría
        public static int ActualizarCategoria(Mdl_ItemCategories categoria)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE ItemCategories SET CategoryCode = @CategoryCode, 
                        CategoryName = @CategoryName, Description = @Description 
                        WHERE CategoryId = @CategoryId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", categoria.CategoryId);
                        cmd.Parameters.AddWithValue("@CategoryCode", categoria.CategoryCode ?? "");
                        cmd.Parameters.AddWithValue("@CategoryName", categoria.CategoryName ?? "");
                        cmd.Parameters.AddWithValue("@Description", (object)categoria.Description ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar categoría: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar categoría
        public static int InactivarCategoria(int categoryId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE ItemCategories SET IsActive = 0 WHERE CategoryId = @CategoryId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar categoría: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear categoría
        private static Mdl_ItemCategories MapearCategoria(SqlDataReader reader)
        {
            return new Mdl_ItemCategories
            {
                CategoryId = reader.GetInt32(0),
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
                    string query = "SELECT CategoryId, CategoryName FROM ItemCategories WHERE IsActive = 1 ORDER BY CategoryName";
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
                MessageBox.Show("Error al obtener categorías: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO OBTENER PROXIMO CODIGO DE CATEGORIA
        public static string ObtenerProximoCodigoCategoria()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT TOP 1 CategoryCode
                             FROM ItemCategories
                             WHERE CategoryCode IS NOT NULL
                             ORDER BY CategoryId DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && !string.IsNullOrWhiteSpace(resultado.ToString()))
                        {
                            string ultimoCodigo = resultado.ToString();

                            // Si el código es numérico puro (000001, 000234, etc.)
                            if (int.TryParse(ultimoCodigo, out int numeroActual))
                            {
                                int proximoNumero = numeroActual + 1;
                                return proximoNumero.ToString("D6");
                            }
                            else
                            {
                                // Si contiene letras + números (ej: CAT000123)
                                string soloNumeros = new string(ultimoCodigo.Where(char.IsDigit).ToArray());

                                if (!string.IsNullOrWhiteSpace(soloNumeros) &&
                                    int.TryParse(soloNumeros, out int numExtraido))
                                {
                                    int proximoNumero = numExtraido + 1;

                                    // Mantener prefijo de letras (ej: CAT)
                                    string prefijo = new string(ultimoCodigo.Where(char.IsLetter).ToArray());
                                    return $"{prefijo}{proximoNumero:D6}";
                                }
                                else
                                {
                                    // No se pudo extraer número, comenzar en 000001
                                    return "000001";
                                }
                            }
                        }
                        else
                        {
                            // No hay registros todavía
                            return "000001";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de categoría: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "ERROR";
            }
        }
    }
}