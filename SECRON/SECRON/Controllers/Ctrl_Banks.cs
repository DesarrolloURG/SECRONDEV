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
    internal class Ctrl_Banks
    {
        // MÉTODO PRINCIPAL: Registrar banco
        public static int RegistrarBanco(Mdl_Banks banco)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Banks (BankCode, BankName, IsActive) 
                        VALUES (@BankCode, @BankName, @IsActive)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@BankCode", banco.BankCode ?? "");
                        cmd.Parameters.AddWithValue("@BankName", banco.BankName ?? "");
                        cmd.Parameters.AddWithValue("@IsActive", banco.IsActive);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar banco: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todos los bancos
        public static List<Mdl_Banks> MostrarBancos()
        {
            List<Mdl_Banks> lista = new List<Mdl_Banks>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Banks WHERE IsActive = 1 ORDER BY BankName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearBanco(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener bancos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Actualizar banco
        public static int ActualizarBanco(Mdl_Banks banco)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Banks SET BankCode = @BankCode, BankName = @BankName 
                        WHERE BankId = @BankId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@BankId", banco.BankId);
                        cmd.Parameters.AddWithValue("@BankCode", banco.BankCode ?? "");
                        cmd.Parameters.AddWithValue("@BankName", banco.BankName ?? "");

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar banco: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar banco
        public static int InactivarBanco(int bankId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "UPDATE Banks SET IsActive = 0 WHERE BankId = @BankId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@BankId", bankId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar banco: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear banco
        private static Mdl_Banks MapearBanco(SqlDataReader reader)
        {
            return new Mdl_Banks
            {
                BankId = reader.GetInt32(0),
                BankCode = reader[1].ToString(),
                BankName = reader[2].ToString(),
                IsActive = reader.GetBoolean(3)
            };
        }

        // MÉTODO PARA OBTENER BANCOS PARA COMBOBOX
        public static List<KeyValuePair<int, string>> ObtenerBancosParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT BankId, BankName FROM Banks WHERE IsActive = 1 ORDER BY BankName";
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
                MessageBox.Show("Error al obtener bancos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
    }
}