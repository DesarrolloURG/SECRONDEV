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
    internal class Ctrl_Brands
    {
        // MÉTODO PRINCIPAL: Registrar marca
        public static int RegistrarMarca(Mdl_Brands marca)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Brands (BrandName, Category, IsActive) 
                        VALUES (@BrandName, @Category, @IsActive)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@BrandName", marca.BrandName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@Category", (object)marca.Category?.ToUpper() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", marca.IsActive);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar marca: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todas las marcas
        public static List<Mdl_Brands> MostrarMarcas()
        {
            List<Mdl_Brands> lista = new List<Mdl_Brands>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Brands WHERE IsActive = 1 ORDER BY BrandName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Mdl_Brands
                                {
                                    BrandId = reader.GetInt32(0),
                                    BrandName = reader[1].ToString(),
                                    Category = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                                    IsActive = reader.GetBoolean(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener marcas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Mostrar marcas por categoría
        public static List<Mdl_Brands> MostrarMarcasPorCategoria(string categoria)
        {
            List<Mdl_Brands> lista = new List<Mdl_Brands>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Brands WHERE Category = @Category AND IsActive = 1 ORDER BY BrandName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Category", categoria?.ToUpper() ?? "");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Mdl_Brands
                                {
                                    BrandId = reader.GetInt32(0),
                                    BrandName = reader[1].ToString(),
                                    Category = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                                    IsActive = reader.GetBoolean(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener marcas por categoría: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Actualizar marca
        public static int ActualizarMarca(Mdl_Brands marca)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Brands SET BrandName = @BrandName, Category = @Category 
                        WHERE BrandId = @BrandId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@BrandId", marca.BrandId);
                        cmd.Parameters.AddWithValue("@BrandName", marca.BrandName?.ToUpper() ?? "");
                        cmd.Parameters.AddWithValue("@Category", (object)marca.Category?.ToUpper() ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar marca: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar marca
        public static int InactivarMarca(int brandId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE Brands SET IsActive = 0 WHERE BrandId = @BrandId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@BrandId", brandId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar marca: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PARA COMBOBOX
        public static List<KeyValuePair<int, string>> ObtenerMarcasParaCombo(string categoria = null)
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT BrandId, BrandName FROM Brands WHERE IsActive = 1";

                    if (!string.IsNullOrWhiteSpace(categoria))
                    {
                        query += " AND Category = @Category";
                    }

                    query += " ORDER BY BrandName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrWhiteSpace(categoria))
                        {
                            cmd.Parameters.AddWithValue("@Category", categoria.ToUpper());
                        }

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
                MessageBox.Show("Error al obtener marcas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
    }
}