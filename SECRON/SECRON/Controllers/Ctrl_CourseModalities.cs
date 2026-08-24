using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_CourseModalities
    {
        #region Consultas

        public static List<Mdl_CourseModalities> ObtenerModalidades(bool soloActivas)
        {
            List<Mdl_CourseModalities> lista = new List<Mdl_CourseModalities>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT
                            ModalityId, ModalityCode, ModalityName,
                            IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy
                        FROM CourseModalities
                        WHERE (@SoloActivas = 0 OR IsActive = 1)
                        ORDER BY ModalityName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@SoloActivas", soloActivas);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearModalidad(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER MODALIDADES: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        #endregion

        #region CRUD

        public static int RegistrarModalidad(Mdl_CourseModalities modalidad)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CourseModalities_Insert", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ModalityCode", modalidad.ModalityCode);
                    cmd.Parameters.AddWithValue("@ModalityName", modalidad.ModalityName);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object)modalidad.CreatedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL REGISTRAR MODALIDAD: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarModalidad(Mdl_CourseModalities modalidad)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CourseModalities_Update", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ModalityId", modalidad.ModalityId);
                    cmd.Parameters.AddWithValue("@ModalityCode", modalidad.ModalityCode);
                    cmd.Parameters.AddWithValue("@ModalityName", modalidad.ModalityName);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)modalidad.ModifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR MODALIDAD: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int CambiarEstadoModalidad(int modalityId, bool isActive, int? modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CourseModalities_Delete", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ModalityId", modalityId);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CAMBIAR ESTADO DE LA MODALIDAD: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Métodos auxiliares

        private static Mdl_CourseModalities MapearModalidad(SqlDataReader reader)
        {
            return new Mdl_CourseModalities
            {
                ModalityId = Convert.ToInt32(reader["ModalityId"]),
                ModalityCode = reader["ModalityCode"]?.ToString(),
                ModalityName = reader["ModalityName"]?.ToString(),
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["CreatedDate"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CreatedBy"]),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ModifiedDate"]),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ModifiedBy"])
            };
        }

        #endregion
    }
}