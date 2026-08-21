using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_CareerCourses
    {
        #region Consultas

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

        #region CRUD

        public static int RegistrarCurso(Mdl_CareerCourses asignacion, int usuarioId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CareerCourses_Insert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CareerId", asignacion.CareerId);
                    cmd.Parameters.AddWithValue("@CourseId", asignacion.CourseId);
                    cmd.Parameters.AddWithValue("@Semester", (object)asignacion.Semester ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsRequired", (object)asignacion.IsRequired ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Prerequisites", (object)asignacion.Prerequisites ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", asignacion.IsActive);
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ASIGNAR CURSO A LA CARRERA: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarCurso(Mdl_CareerCourses asignacion, int usuarioId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CareerCourses_Update", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CareerCourseId", asignacion.CareerCourseId);
                    cmd.Parameters.AddWithValue("@CareerId", asignacion.CareerId);
                    cmd.Parameters.AddWithValue("@CourseId", asignacion.CourseId);
                    cmd.Parameters.AddWithValue("@Semester", (object)asignacion.Semester ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsRequired", (object)asignacion.IsRequired ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Prerequisites", (object)asignacion.Prerequisites ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

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

        // @Mode: 1 = Inactivar, 2 = Reactivar (mismo patrón que SP_Careers_UpdateStatus / SP_Courses_UpdateStatus)
        public static int CambiarEstadoCurso(int careerCourseId, int modo, int usuarioId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CareerCourses_UpdateStatus", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CareerCourseId", careerCourseId);
                    cmd.Parameters.AddWithValue("@Mode", modo);
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

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

        private static Mdl_CareerCourses MapearCareerCourse(SqlDataReader reader)
        {
            return new Mdl_CareerCourses
            {
                CareerCourseId = Convert.ToInt32(reader["CareerCourseId"]),
                CareerId = Convert.ToInt32(reader["CareerId"]),
                CourseId = Convert.ToInt32(reader["CourseId"]),
                CourseCode = reader["CourseCode"]?.ToString(),
                CourseName = reader["CourseName"]?.ToString(),
                Semester = reader["Semester"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["Semester"]),
                IsRequired = reader["IsRequired"] == DBNull.Value ? null : (bool?)Convert.ToBoolean(reader["IsRequired"]),
                Prerequisites = reader["Prerequisites"] == DBNull.Value ? null : reader["Prerequisites"].ToString(),
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