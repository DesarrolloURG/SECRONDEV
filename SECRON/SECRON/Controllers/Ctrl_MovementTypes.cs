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
    internal class Ctrl_MovementTypes
    {
        // MÉTODO PRINCIPAL: Obtener todos los tipos de movimiento
        public static List<Mdl_MovementTypes> MostrarTiposMovimiento()
        {
            List<Mdl_MovementTypes> lista = new List<Mdl_MovementTypes>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM MovementTypes WHERE IsActive = 1 ORDER BY TypeName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Mdl_MovementTypes
                                {
                                    MovementTypeId = reader.GetInt32(0),
                                    TypeCode = reader[1].ToString(),
                                    TypeName = reader[2].ToString(),
                                    AffectsStock = reader[3].ToString(),
                                    RequiresSupplier = reader.GetBoolean(4),
                                    RequiresDestination = reader.GetBoolean(5),
                                    IsActive = reader.GetBoolean(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener tipos de movimiento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA COMBOBOX
        public static List<KeyValuePair<int, string>> ObtenerTiposMovimientoParaCombo()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT MovementTypeId, TypeName FROM MovementTypes WHERE IsActive = 1 ORDER BY TypeName";
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
                MessageBox.Show("Error al obtener tipos de movimiento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PARA OBTENER TIPO POR ID
        public static Mdl_MovementTypes ObtenerTipoPorId(int movementTypeId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM MovementTypes WHERE MovementTypeId = @MovementTypeId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MovementTypeId", movementTypeId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Mdl_MovementTypes
                                {
                                    MovementTypeId = reader.GetInt32(0),
                                    TypeCode = reader[1].ToString(),
                                    TypeName = reader[2].ToString(),
                                    AffectsStock = reader[3].ToString(),
                                    RequiresSupplier = reader.GetBoolean(4),
                                    RequiresDestination = reader.GetBoolean(5),
                                    IsActive = reader.GetBoolean(6)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener tipo de movimiento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }
    }
}