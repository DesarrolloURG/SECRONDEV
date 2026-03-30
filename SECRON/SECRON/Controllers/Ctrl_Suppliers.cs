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
    internal class Ctrl_Suppliers
    {
        // MÉTODO PRINCIPAL: Registrar proveedor
        public static int RegistrarProveedor(Mdl_Suppliers proveedor)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Suppliers (SupplierCode, SupplierName, LegalName, TaxId, 
                        ContactName, Phone, Phone2, Email, Address, CommercialActivity, Classification, 
                        BankAccountNumber, BankName, IsActive, CreatedBy) 
                        VALUES (@SupplierCode, @SupplierName, @LegalName, @TaxId, @ContactName, @Phone, 
                        @Phone2, @Email, @Address, @CommercialActivity, @Classification, @BankAccountNumber, 
                        @BankName, @IsActive, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@SupplierCode", proveedor.SupplierCode ?? "");
                        cmd.Parameters.AddWithValue("@SupplierName", proveedor.SupplierName ?? "");
                        cmd.Parameters.AddWithValue("@LegalName", proveedor.LegalName ?? "");
                        cmd.Parameters.AddWithValue("@TaxId", (object)proveedor.TaxId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContactName", (object)proveedor.ContactName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Phone", proveedor.Phone ?? "");
                        cmd.Parameters.AddWithValue("@Phone2", (object)proveedor.Phone2 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", (object)proveedor.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object)proveedor.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CommercialActivity", proveedor.CommercialActivity ?? "");
                        cmd.Parameters.AddWithValue("@Classification", proveedor.Classification ?? "");
                        cmd.Parameters.AddWithValue("@BankAccountNumber", (object)proveedor.BankAccountNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankName", (object)proveedor.BankName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", proveedor.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)proveedor.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar proveedor: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar proveedores con paginación
        public static List<Mdl_Suppliers> MostrarProveedores(int pageNumber = 1, int pageSize = 100)
        {
            List<Mdl_Suppliers> lista = new List<Mdl_Suppliers>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM Suppliers WHERE IsActive = 1 
                        ORDER BY SupplierName 
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearProveedor(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener proveedores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Búsqueda con filtros
        public static List<Mdl_Suppliers> BuscarProveedores(
            string textoBusqueda = "",
            string classification = "",
            int pageNumber = 1,
            int pageSize = 100)
        {
            List<Mdl_Suppliers> lista = new List<Mdl_Suppliers>();
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Suppliers WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (SupplierCode LIKE @texto OR SupplierName LIKE @texto OR 
                            LegalName LIKE @texto OR TaxId LIKE @texto OR Phone LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (!string.IsNullOrWhiteSpace(classification))
                    {
                        query += " AND Classification = @classification";
                        parametros.Add(new SqlParameter("@classification", classification.Trim()));
                    }

                    query += " ORDER BY SupplierName OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearProveedor(reader));
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

        // MÉTODO PRINCIPAL: Actualizar proveedor
        public static int ActualizarProveedor(Mdl_Suppliers proveedor)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Suppliers SET SupplierCode = @SupplierCode, SupplierName = @SupplierName, 
                        LegalName = @LegalName, TaxId = @TaxId, ContactName = @ContactName, Phone = @Phone, 
                        Phone2 = @Phone2, Email = @Email, Address = @Address, CommercialActivity = @CommercialActivity, 
                        Classification = @Classification, BankAccountNumber = @BankAccountNumber, BankName = @BankName, 
                        ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy 
                        WHERE SupplierId = @SupplierId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@SupplierId", proveedor.SupplierId);
                        cmd.Parameters.AddWithValue("@SupplierCode", proveedor.SupplierCode ?? "");
                        cmd.Parameters.AddWithValue("@SupplierName", proveedor.SupplierName ?? "");
                        cmd.Parameters.AddWithValue("@LegalName", proveedor.LegalName ?? "");
                        cmd.Parameters.AddWithValue("@TaxId", (object)proveedor.TaxId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContactName", (object)proveedor.ContactName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Phone", proveedor.Phone ?? "");
                        cmd.Parameters.AddWithValue("@Phone2", (object)proveedor.Phone2 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", (object)proveedor.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object)proveedor.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CommercialActivity", proveedor.CommercialActivity ?? "");
                        cmd.Parameters.AddWithValue("@Classification", proveedor.Classification ?? "");
                        cmd.Parameters.AddWithValue("@BankAccountNumber", (object)proveedor.BankAccountNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankName", (object)proveedor.BankName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)proveedor.ModifiedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar proveedor: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar proveedor
        public static int InactivarProveedor(int supplierId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Suppliers SET IsActive = 0, ModifiedDate = GETDATE(), 
                        ModifiedBy = @ModifiedBy WHERE SupplierId = @SupplierId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar proveedor: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear proveedor
        private static Mdl_Suppliers MapearProveedor(SqlDataReader reader)
        {
            return new Mdl_Suppliers
            {
                SupplierId = reader.GetInt32(0),
                SupplierCode = reader[1].ToString(),
                SupplierName = reader[2].ToString(),
                LegalName = reader[3].ToString(),
                TaxId = reader[4] == DBNull.Value ? null : reader[4].ToString(),
                ContactName = reader[5] == DBNull.Value ? null : reader[5].ToString(),
                Phone = reader[6].ToString(),
                Phone2 = reader[7] == DBNull.Value ? null : reader[7].ToString(),
                Email = reader[8] == DBNull.Value ? null : reader[8].ToString(),
                Address = reader[9] == DBNull.Value ? null : reader[9].ToString(),
                CommercialActivity = reader[10].ToString(),
                Classification = reader[11].ToString(),
                BankAccountNumber = reader[12] == DBNull.Value ? null : reader[12].ToString(),
                BankName = reader[13] == DBNull.Value ? null : reader[13].ToString(),
                IsActive = reader.GetBoolean(14),
                CreatedDate = reader.GetDateTime(15),
                CreatedBy = reader[16] == DBNull.Value ? null : (int?)reader.GetInt32(16),
                ModifiedDate = reader[17] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(17),
                ModifiedBy = reader[18] == DBNull.Value ? null : (int?)reader.GetInt32(18)
            };
        }

        // MÉTODO PARA OBTENER PROVEEDORES PARA COMBOBOX
        public static List<KeyValuePair<int, string>> ObtenerProveedoresParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT SupplierId, SupplierName FROM Suppliers WHERE IsActive = 1 ORDER BY SupplierName";
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
                MessageBox.Show("Error al obtener proveedores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA CONTAR TOTAL
        public static int ContarTotalProveedores(string textoBusqueda = "", string classification = "")
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM Suppliers WHERE IsActive = 1";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(textoBusqueda))
                    {
                        query += @" AND (SupplierCode LIKE @texto OR SupplierName LIKE @texto OR 
                            LegalName LIKE @texto OR TaxId LIKE @texto OR Phone LIKE @texto)";
                        parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
                    }

                    if (!string.IsNullOrWhiteSpace(classification))
                    {
                        query += " AND Classification = @classification";
                        parametros.Add(new SqlParameter("@classification", classification.Trim()));
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
        public static string ObtenerProximoCodigoProveedor()
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // Obtener el último código registrado
                    string query = @"SELECT TOP 1 SupplierCode 
                           FROM Suppliers 
                           WHERE SupplierCode IS NOT NULL 
                           ORDER BY SupplierId DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && !string.IsNullOrWhiteSpace(resultado.ToString()))
                        {
                            string ultimoCodigo = resultado.ToString();

                            // Intentar convertir a número
                            if (int.TryParse(ultimoCodigo, out int numeroActual))
                            {
                                // Si es número, sumar 1
                                int proximoNumero = numeroActual + 1;
                                return proximoNumero.ToString("D6"); // Formato: 000001, 000002, etc.
                            }
                            else
                            {
                                // Si contiene letras y números, intentar extraer el número
                                string soloNumeros = new string(ultimoCodigo.Where(char.IsDigit).ToArray());

                                if (!string.IsNullOrWhiteSpace(soloNumeros) && int.TryParse(soloNumeros, out int numExtraido))
                                {
                                    int proximoNumero = numExtraido + 1;
                                    // Mantener el prefijo de letras si existe
                                    string prefijo = new string(ultimoCodigo.Where(char.IsLetter).ToArray());
                                    return $"{prefijo}{proximoNumero:D6}";
                                }
                                else
                                {
                                    // Si no se puede extraer número, empezar desde 1
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
                MessageBox.Show($"Error al generar código de proveedor: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "ERROR";
            }
        }
    }
}