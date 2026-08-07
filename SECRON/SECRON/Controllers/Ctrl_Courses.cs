using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    public static class Ctrl_Courses
    {
        #region Insertar

        public static int InsertarCurso(Mdl_Courses curso, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Courses_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CourseCode", curso.CourseCode);
                        cmd.Parameters.AddWithValue("@CourseName", curso.CourseName);
                        cmd.Parameters.AddWithValue("@Description", (object)curso.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Credits", curso.Credits);
                        cmd.Parameters.AddWithValue("@TheoryHours", (object)curso.TheoryHours ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PracticeHours", (object)curso.PracticeHours ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LabHours", (object)curso.LabHours ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Sessions", (object)curso.Sessions ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsCommon", curso.IsCommon);
                        cmd.Parameters.AddWithValue("@IsActive", curso.IsActive);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL GUARDAR CURSO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Actualizar

        public static int ActualizarCurso(Mdl_Courses curso, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Courses_Update", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CourseId", curso.CourseId);
                        cmd.Parameters.AddWithValue("@CourseCode", curso.CourseCode);
                        cmd.Parameters.AddWithValue("@CourseName", curso.CourseName);
                        cmd.Parameters.AddWithValue("@Description", (object)curso.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Credits", curso.Credits);
                        cmd.Parameters.AddWithValue("@TheoryHours", (object)curso.TheoryHours ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PracticeHours", (object)curso.PracticeHours ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LabHours", (object)curso.LabHours ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Sessions", (object)curso.Sessions ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsCommon", curso.IsCommon);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR CURSO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region CambiarEstado

        public static int CambiarEstadoCurso(int courseId, int modo, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Courses_UpdateStatus", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CourseId", courseId);
                        cmd.Parameters.AddWithValue("@Mode", modo);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CAMBIAR ESTADO DE CURSO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Consultar

        public static List<Mdl_Courses> ObtenerCursos(string campo, string valor, string estado, string comun, int pageNumber, int pageSize, out int totalRows)
        {
            List<Mdl_Courses> lista = new List<Mdl_Courses>();
            totalRows = 0;

            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Courses_Select", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Campo", campo);
                        cmd.Parameters.AddWithValue("@Valor", (object)valor ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", estado);
                        cmd.Parameters.AddWithValue("@Comun", comun);
                        cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCurso(reader));
                                totalRows = Convert.ToInt32(reader["TotalRows"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR CURSOS: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static List<Mdl_Courses> ObtenerCursosParaExportar(string campo, string valor, string estado, string comun)
        {
            int totalRows;
            return ObtenerCursos(campo, valor, estado, comun, 1, 0, out totalRows);
        }

        #endregion

        #region Importar

        public static int ImportarCurso(Mdl_Courses curso, int usuarioId)
        {
            return InsertarCurso(curso, usuarioId);
        }

        #endregion

        #region Mapeo

        private static Mdl_Courses MapearCurso(SqlDataReader reader)
        {
            return new Mdl_Courses
            {
                CourseId = Convert.ToInt32(reader["CourseId"]),
                CourseCode = reader["CourseCode"]?.ToString(),
                CourseName = reader["CourseName"]?.ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                Credits = Convert.ToInt32(reader["Credits"]),
                TheoryHours = reader["TheoryHours"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["TheoryHours"]),
                PracticeHours = reader["PracticeHours"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["PracticeHours"]),
                LabHours = reader["LabHours"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["LabHours"]),
                TotalHours = reader["TotalHours"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["TotalHours"]),
                Sessions = reader["Sessions"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["Sessions"]),
                IsCommon = reader["IsCommon"] != DBNull.Value && Convert.ToBoolean(reader["IsCommon"]),
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CreatedBy"]),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ModifiedDate"]),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ModifiedBy"])
            };
        }

        #endregion
    }
}