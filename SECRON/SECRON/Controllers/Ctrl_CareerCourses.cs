using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_CareerCourses
    {
        #region Consultas

        // Cursos activos de un pensum específico
        public static List<Mdl_CareerCourses> ObtenerCursosActivosPorPensum(int careerPensumId)
        {
            List<Mdl_CareerCourses> lista = new List<Mdl_CareerCourses>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT
                            cc.CareerCourseId,
                            cc.CareerPensumId,
                            cc.CourseId,
                            cc.Semester,
                            cc.IsRequired,
                            cc.StandardPrice,
                            co.CourseCode,
                            co.CourseName,
                            cc.IsActive,
                            cc.CreatedDate,
                            cc.CreatedBy,
                            cc.ModifiedDate,
                            cc.ModifiedBy
                        FROM CareerCourses cc
                        INNER JOIN Courses co ON cc.CourseId = co.CourseId
                        WHERE cc.CareerPensumId = @CareerPensumId
                          AND cc.IsActive = 1
                        ORDER BY cc.Semester, co.CourseName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CareerPensumId", careerPensumId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearCareerCourse(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER CURSOS DEL PENSUM: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        #endregion

        #region CRUD

        // NOTA: la creación/edición de cursos dentro de un pensum se hace normalmente
        // a través de SP_CareerPensums_SaveTree (guardado consolidado del árbol completo).
        // Estos métodos quedan disponibles para ajustes puntuales fuera de ese flujo.

        public static int RegistrarCareerCourse(Mdl_CareerCourses careerCourse)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CareerCourses_Insert", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CareerPensumId", careerCourse.CareerPensumId);
                    cmd.Parameters.AddWithValue("@CourseId", careerCourse.CourseId);
                    cmd.Parameters.AddWithValue("@Semester", careerCourse.Semester);
                    cmd.Parameters.AddWithValue("@IsRequired", careerCourse.IsRequired);
                    cmd.Parameters.AddWithValue("@StandardPrice", careerCourse.StandardPrice);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object)careerCourse.CreatedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL REGISTRAR CURSO EN EL PENSUM: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarCareerCourse(Mdl_CareerCourses careerCourse)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CareerCourses_Update", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CareerCourseId", careerCourse.CareerCourseId);
                    cmd.Parameters.AddWithValue("@CareerPensumId", careerCourse.CareerPensumId);
                    cmd.Parameters.AddWithValue("@CourseId", careerCourse.CourseId);
                    cmd.Parameters.AddWithValue("@Semester", careerCourse.Semester);
                    cmd.Parameters.AddWithValue("@IsRequired", careerCourse.IsRequired);
                    cmd.Parameters.AddWithValue("@StandardPrice", careerCourse.StandardPrice);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)careerCourse.ModifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR CURSO DEL PENSUM: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int CambiarEstadoCareerCourse(int careerCourseId, bool isActive, int? modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CareerCourses_Delete", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CareerCourseId", careerCourseId);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CAMBIAR ESTADO DEL CURSO DEL PENSUM: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Métodos auxiliares

        private static Mdl_CareerCourses MapearCareerCourse(SqlDataReader reader)
        {
            return new Mdl_CareerCourses
            {
                CareerCourseId = Convert.ToInt32(reader["CareerCourseId"]),
                CareerPensumId = Convert.ToInt32(reader["CareerPensumId"]),
                CourseId = Convert.ToInt32(reader["CourseId"]),
                Semester = Convert.ToInt32(reader["Semester"]),
                IsRequired = reader["IsRequired"] != DBNull.Value && Convert.ToBoolean(reader["IsRequired"]),
                StandardPrice = Convert.ToDecimal(reader["StandardPrice"]),
                CourseCode = reader["CourseCode"]?.ToString(),
                CourseName = reader["CourseName"]?.ToString(),
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["CreatedDate"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CreatedBy"]),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ModifiedDate"]),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ModifiedBy"])
            };
        }

        // Cursos de una carrera específica (join con Courses para CourseCode/CourseName)
        public static List<Mdl_CareerCourses> MostrarCursosPorCarrera(int careerId, string estado = "TODOS")
        {
            List<Mdl_CareerCourses> lista = new List<Mdl_CareerCourses>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CareerCourses_Select", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CareerId", careerId);
                    cmd.Parameters.AddWithValue("@Estado", estado);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(MapearCareerCourse(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER CURSOS DE LA CARRERA: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        #endregion
    }
}