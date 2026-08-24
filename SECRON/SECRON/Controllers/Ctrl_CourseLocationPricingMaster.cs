using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_CourseLocationPricingMaster
    {
        #region Consultas

        public static int GuardarAsignacionYPrecios(int locationId, int careerId, int modalityId, string pricesJson, int? userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_LocationCareers_SaveAssignmentAndPricing", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;
                    cmd.Parameters.AddWithValue("@LocationId", locationId);
                    cmd.Parameters.AddWithValue("@CareerId", careerId);
                    cmd.Parameters.AddWithValue("@ModalityId", modalityId);
                    cmd.Parameters.AddWithValue("@PricesJson", pricesJson);
                    cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL GUARDAR LA ASIGNACIÓN Y LOS PRECIOS: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Grilla principal: combinaciones Curso-Carrera-Sede-Modalidad con su precio vigente
        public static List<Mdl_CourseLocationPricingMaster> MostrarPrecios(int? locationId, int? careerId, bool soloActivas)
        {
            List<Mdl_CourseLocationPricingMaster> lista = new List<Mdl_CourseLocationPricingMaster>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                SELECT
                    m.CourseLocationPricingId,
                    m.CareerCourseId,
                    cc.CareerPensumId,
                    m.LocationId,
                    m.ModalityId,
                    co.CourseCode,
                    co.CourseName,
                    ca.CareerCode,
                    ca.CareerName,
                    l.LocationName,
                    mod.ModalityName,
                    d.Price AS CurrentPrice,
                    d.EffectiveFrom AS CurrentPriceEffectiveFrom,
                    m.IsActive,
                    m.CreatedDate,
                    m.CreatedBy,
                    m.ModifiedDate,
                    m.ModifiedBy
                FROM CourseLocationPricingMaster m
                INNER JOIN CareerCourses cc ON m.CareerCourseId = cc.CareerCourseId
                INNER JOIN Courses co ON cc.CourseId = co.CourseId
                INNER JOIN CareerPensums cp ON cc.CareerPensumId = cp.CareerPensumId
                INNER JOIN Careers ca ON cp.CareerId = ca.CareerId
                INNER JOIN Locations l ON m.LocationId = l.LocationId
                INNER JOIN CourseModalities mod ON m.ModalityId = mod.ModalityId
                LEFT JOIN CourseLocationPricingDetail d
                    ON d.CourseLocationPricingId = m.CourseLocationPricingId
                    AND d.EffectiveTo IS NULL
                    AND d.IsActive = 1
                WHERE (@LocationId IS NULL OR m.LocationId = @LocationId)
                  AND (@CareerId IS NULL OR cp.CareerId = @CareerId)
                  AND (@SoloActivas = 0 OR m.IsActive = 1)
                ORDER BY ca.CareerName, co.CourseName, l.LocationName, mod.ModalityName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", (object)locationId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CareerId", (object)careerId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SoloActivas", soloActivas);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearPrecio(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER PRECIOS: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        #endregion

        #region CRUD

        public static int RegistrarPrecioMaster(Mdl_CourseLocationPricingMaster precio)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CourseLocationPricingMaster_Insert", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CareerCourseId", precio.CareerCourseId);
                    cmd.Parameters.AddWithValue("@LocationId", precio.LocationId);
                    cmd.Parameters.AddWithValue("@ModalityId", precio.ModalityId);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object)precio.CreatedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL REGISTRAR COMBINACIÓN DE PRECIO: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarPrecioMaster(Mdl_CourseLocationPricingMaster precio)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CourseLocationPricingMaster_Update", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CourseLocationPricingId", precio.CourseLocationPricingId);
                    cmd.Parameters.AddWithValue("@CareerCourseId", precio.CareerCourseId);
                    cmd.Parameters.AddWithValue("@LocationId", precio.LocationId);
                    cmd.Parameters.AddWithValue("@ModalityId", precio.ModalityId);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)precio.ModifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR COMBINACIÓN DE PRECIO: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Activa o inactiva una combinación (SP maneja cierre/reactivación del precio en Detail)
        public static int CambiarEstadoPrecioMaster(int courseLocationPricingId, bool isActive, int? modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CourseLocationPricingMaster_Delete", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CourseLocationPricingId", courseLocationPricingId);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CAMBIAR ESTADO DE LA COMBINACIÓN: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Métodos auxiliares

        private static Mdl_CourseLocationPricingMaster MapearPrecio(SqlDataReader reader)
        {
            return new Mdl_CourseLocationPricingMaster
            {
                CourseLocationPricingId = Convert.ToInt32(reader["CourseLocationPricingId"]),
                CareerCourseId = Convert.ToInt32(reader["CareerCourseId"]),
                CareerPensumId = Convert.ToInt32(reader["CareerPensumId"]),
                LocationId = Convert.ToInt32(reader["LocationId"]),
                ModalityId = Convert.ToInt32(reader["ModalityId"]),
                CourseCode = reader["CourseCode"]?.ToString(),
                CourseName = reader["CourseName"]?.ToString(),
                CareerCode = reader["CareerCode"]?.ToString(),
                CareerName = reader["CareerName"]?.ToString(),
                LocationName = reader["LocationName"]?.ToString(),
                ModalityName = reader["ModalityName"]?.ToString(),
                CurrentPrice = reader["CurrentPrice"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["CurrentPrice"]),
                CurrentPriceEffectiveFrom = reader["CurrentPriceEffectiveFrom"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["CurrentPriceEffectiveFrom"]),
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["CreatedDate"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CreatedBy"]),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ModifiedDate"]),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ModifiedBy"])
            };
        }

        #endregion

        public static List<Mdl_PensumCoursePricing> ObtenerCursosPensumConPrecio(int careerPensumId, int locationId, int modalityId)
        {
            List<Mdl_PensumCoursePricing> lista = new List<Mdl_PensumCoursePricing>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                SELECT
                    cc.CareerCourseId,
                    cc.Semester,
                    co.CourseCode,
                    co.CourseName,
                    cc.StandardPrice,
                    m.CourseLocationPricingId,
                    d.Price AS CurrentPrice
                FROM CareerCourses cc
                INNER JOIN Courses co ON co.CourseId = cc.CourseId
                LEFT JOIN CourseLocationPricingMaster m
                    ON m.CareerCourseId = cc.CareerCourseId
                   AND m.LocationId = @LocationId
                   AND m.ModalityId = @ModalityId
                   AND m.IsActive = 1
                LEFT JOIN CourseLocationPricingDetail d
                    ON d.CourseLocationPricingId = m.CourseLocationPricingId
                   AND d.EffectiveTo IS NULL
                   AND d.IsActive = 1
                WHERE cc.CareerPensumId = @CareerPensumId
                  AND cc.IsActive = 1
                ORDER BY cc.Semester, co.CourseName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CareerPensumId", careerPensumId);
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        cmd.Parameters.AddWithValue("@ModalityId", modalityId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Mdl_PensumCoursePricing
                                {
                                    CareerCourseId = Convert.ToInt32(reader["CareerCourseId"]),
                                    Semester = Convert.ToInt32(reader["Semester"]),
                                    CourseCode = reader["CourseCode"]?.ToString(),
                                    CourseName = reader["CourseName"]?.ToString(),
                                    StandardPrice = Convert.ToDecimal(reader["StandardPrice"]),
                                    CourseLocationPricingId = reader["CourseLocationPricingId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CourseLocationPricingId"]),
                                    CurrentPrice = reader["CurrentPrice"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["CurrentPrice"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER PRECIOS DEL PENSUM: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }
    }
}