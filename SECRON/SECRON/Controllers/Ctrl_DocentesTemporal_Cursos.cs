using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    public static class Ctrl_DocentesTemporal_Cursos
    {
        // Cuenta cuántos cursos (detalle) tiene cargados un DPI (para validar el máximo de 5)
        public static int ContarPorDPI(string dpi)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_DocentesTemporal_Cursos_ContarPorDPI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DPI", dpi);

                    conn.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONTAR CURSOS POR DPI: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Inserta un curso (detalle) enlazado a un docente ya existente. Devuelve @rows (1 = éxito, 0 = error)
        public static int Insert(Mdl_DocentesTemporal_Cursos curso)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_DocentesTemporal_Cursos_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeacherTempId", curso.TeacherTempId);
                    cmd.Parameters.AddWithValue("@AcademicLocation", curso.AcademicLocation);
                    cmd.Parameters.AddWithValue("@CourseToTeach", curso.CourseToTeach);
                    cmd.Parameters.AddWithValue("@Schedule", (object)curso.Schedule ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fees", (object)curso.Fees ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UsuarioId", (object)curso.CreatedBy ?? DBNull.Value);

                    conn.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL REGISTRAR CURSO DEL DOCENTE: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Lista los cursos (detalle) de un docente específico
        public static List<Mdl_DocentesTemporal_Cursos> SelectByTeacherTempId(int teacherTempId)
        {
            var lista = new List<Mdl_DocentesTemporal_Cursos>();
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_DocentesTemporal_Cursos_SelectByTeacherTempId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeacherTempId", teacherTempId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Mdl_DocentesTemporal_Cursos
                            {
                                TeacherTempCourseId = Convert.ToInt32(reader["TeacherTempCourseId"]),
                                TeacherTempId = Convert.ToInt32(reader["TeacherTempId"]),
                                AcademicLocation = reader["AcademicLocation"]?.ToString(),
                                CourseToTeach = reader["CourseToTeach"]?.ToString(),
                                Schedule = reader["Schedule"]?.ToString(),
                                Fees = reader["Fees"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["Fees"]),
                                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CreatedBy"]),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["CreatedDate"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR CURSOS DEL DOCENTE: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // Lista las sedes distintas cargadas (para llenar el ComboBox_Sede)
        public static List<string> ObtenerSedes()
        {
            var lista = new List<string>();
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_DocentesTemporal_Cursos_SelectSedes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(reader["AcademicLocation"]?.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR SEDES: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
    }
}