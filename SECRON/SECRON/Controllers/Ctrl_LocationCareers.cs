using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_LocationCareers
    {
        #region Consultas

        // Carreras YA asignadas (activas) a una sede específica — plantilla activa
        public static List<Mdl_LocationCareers> MostrarCarrerasPorSede(int locationId)
        {
            List<Mdl_LocationCareers> lista = new List<Mdl_LocationCareers>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT
                            lc.LocationCareerId,
                            lc.LocationId,
                            lc.CareerId,
                            c.CareerCode,
                            c.CareerName,
                            lc.IsActive,
                            lc.CreatedDate,
                            lc.CreatedBy,
                            lc.ModifiedDate,
                            lc.ModifiedBy
                        FROM LocationCareers lc
                        INNER JOIN Careers c ON lc.CareerId = c.CareerId
                        WHERE lc.LocationId = @LocationId
                          AND lc.IsActive = 1
                        ORDER BY c.CareerName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearAsignacion(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER CARRERAS DE LA SEDE: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static bool ExisteAsignacionActiva(int locationId, int careerId, int modalityId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT COUNT(1)
                          FROM LocationCareers
                         WHERE LocationId = @LocationId
                           AND CareerId = @CareerId
                           AND ModalityId = @ModalityId
                           AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        cmd.Parameters.AddWithValue("@CareerId", careerId);
                        cmd.Parameters.AddWithValue("@ModalityId", modalityId);

                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL VALIDAR ASIGNACIÓN DE CARRERA EN LA SEDE: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region CRUD

        public static int RegistrarAsignacion(Mdl_LocationCareers asignacion)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_LocationCareers_Insert", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LocationId", asignacion.LocationId);
                    cmd.Parameters.AddWithValue("@CareerId", asignacion.CareerId);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object)asignacion.CreatedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ASIGNAR CARRERA A LA SEDE: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarAsignacion(Mdl_LocationCareers asignacion)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_LocationCareers_Update", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LocationCareerId", asignacion.LocationCareerId);
                    cmd.Parameters.AddWithValue("@LocationId", asignacion.LocationId);
                    cmd.Parameters.AddWithValue("@CareerId", asignacion.CareerId);
                    cmd.Parameters.AddWithValue("@IsActive", asignacion.IsActive);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)asignacion.ModifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR ASIGNACIÓN: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Activa o inactiva una asignación existente (SP_LocationCareers_Delete maneja ambos sentidos vía @IsActive)
        public static int CambiarEstadoAsignacion(int locationCareerId, bool isActive, int? modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_LocationCareers_Delete", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LocationCareerId", locationCareerId);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CAMBIAR ESTADO DE LA ASIGNACIÓN: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Métodos auxiliares

        private static Mdl_LocationCareers MapearAsignacion(SqlDataReader reader)
        {
            return new Mdl_LocationCareers
            {
                LocationCareerId = Convert.ToInt32(reader["LocationCareerId"]),
                LocationId = Convert.ToInt32(reader["LocationId"]),
                CareerId = Convert.ToInt32(reader["CareerId"]),
                CareerCode = reader["CareerCode"]?.ToString(),
                CareerName = reader["CareerName"]?.ToString(),
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