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
    internal class Ctrl_CheckControl
    {
        // MÉTODO PRINCIPAL: Registrar control de cheques
        public static int RegistrarControl(Mdl_CheckControl control)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO CheckControl (UserId, InitialLimit, FinalLimit, CurrentCounter, 
                    Priority, IsActive, CreatedBy) 
                    VALUES (@UserId, @InitialLimit, @FinalLimit, @CurrentCounter, @Priority, @IsActive, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", control.UserId);
                        cmd.Parameters.AddWithValue("@InitialLimit", control.InitialLimit);
                        cmd.Parameters.AddWithValue("@FinalLimit", control.FinalLimit);
                        cmd.Parameters.AddWithValue("@CurrentCounter", control.CurrentCounter);
                        cmd.Parameters.AddWithValue("@Priority", control.Priority);
                        cmd.Parameters.AddWithValue("@IsActive", control.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)control.CreatedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar control: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Obtener control por usuario
        public static Mdl_CheckControl ObtenerControlPorUsuario(int userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM CheckControl WHERE UserId = @UserId AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearControl(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener control: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO PRINCIPAL: Incrementar contador
        public static int SiguienteCheque(int userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE CheckControl SET CurrentCounter = CurrentCounter + 1, 
                        ModifiedDate = GETDATE() WHERE UserId = @UserId AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar contador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
        // MÉTODO: Incrementar contador por rango específico (CheckControlId)
        public static int SiguienteChequePorControl(int checkControlId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE CheckControl
                             SET CurrentCounter = CurrentCounter + 1,
                                 ModifiedDate   = GETDATE()
                             WHERE CheckControlId = @CheckControlId
                               AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckControlId", checkControlId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar contador por control: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Actualizar control
        public static int ActualizarControl(Mdl_CheckControl control)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE CheckControl SET InitialLimit = @InitialLimit, 
                    FinalLimit = @FinalLimit, CurrentCounter = @CurrentCounter, Priority = @Priority,
                    ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy 
                    WHERE CheckControlId = @CheckControlId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckControlId", control.CheckControlId);
                        cmd.Parameters.AddWithValue("@InitialLimit", control.InitialLimit);
                        cmd.Parameters.AddWithValue("@FinalLimit", control.FinalLimit);
                        cmd.Parameters.AddWithValue("@CurrentCounter", control.CurrentCounter);
                        cmd.Parameters.AddWithValue("@Priority", control.Priority);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)control.ModifiedBy ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar control: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Inactivar control
        public static int InactivarControl(int checkControlId, int modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE CheckControl SET IsActive = 0, ModifiedDate = GETDATE(), 
                        ModifiedBy = @ModifiedBy WHERE CheckControlId = @CheckControlId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckControlId", checkControlId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar control: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Validar si existe asignación
        public static bool ValidarAsignacion(int userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT COUNT(*) FROM CheckControl WHERE UserId = @UserId AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODO PRINCIPAL: Verificar si llegó al límite
        public static bool HaAlcanzadoLimite(int userId)
        {
            try
            {
                Mdl_CheckControl control = ObtenerControlPorUsuario(userId);
                return control != null && control.CurrentCounter >= control.FinalLimit;
            }
            catch { return false; }
        }

        // MÉTODO PRINCIPAL: Obtener contador actual
        public static int ObtenerContadorActual(int userId)
        {
            try
            {
                Mdl_CheckControl control = ObtenerControlPorUsuario(userId);
                return control?.CurrentCounter ?? 0;
            }
            catch { return 0; }
        }

        // MÉTODO PRINCIPAL: Mostrar todos los controles
        public static List<Mdl_CheckControl> MostrarControles()
        {
            List<Mdl_CheckControl> lista = new List<Mdl_CheckControl>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM CheckControl WHERE IsActive = 1 ORDER BY UserId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearControl(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener controles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO AUXILIAR: Mapear control
        private static Mdl_CheckControl MapearControl(SqlDataReader reader)
        {
            return new Mdl_CheckControl
            {
                CheckControlId = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                InitialLimit = reader.GetInt32(2),
                FinalLimit = reader.GetInt32(3),
                CurrentCounter = reader.GetInt32(4),
                IsActive = reader.GetBoolean(5),
                CreatedDate = reader.GetDateTime(6),
                CreatedBy = reader[7] == DBNull.Value ? null : (int?)reader.GetInt32(7),
                ModifiedDate = reader[8] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(8),
                ModifiedBy = reader[9] == DBNull.Value ? null : (int?)reader.GetInt32(9),
                Priority = reader.GetBoolean(10)
            };
        }
        // MÉTODO: Obtener control por ID
        public static Mdl_CheckControl ObtenerControlPorId(int checkControlId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM CheckControl WHERE CheckControlId = @CheckControlId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckControlId", checkControlId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearControl(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener control: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        // MÉTODO: Eliminar control físicamente de BD
        public static bool EliminarControl(int checkControlId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "DELETE FROM CheckControl WHERE CheckControlId = @CheckControlId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckControlId", checkControlId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar control: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}