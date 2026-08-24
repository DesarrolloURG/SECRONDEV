using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_CareerCoursePrerequisites
    {
        // Retorna pares (CourseId, PrerequisiteCourseId) de todos los cursos activos de un pensum,
        // usando CourseId (no CareerCourseId) para que sea fácil de usar en el árbol en memoria del formulario.
        public static List<(int CourseId, int PrerequisiteCourseId)> ObtenerPrerequisitosPorPensum(int careerPensumId)
        {
            var lista = new List<(int, int)>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT cc.CourseId, ccPrereq.CourseId AS PrerequisiteCourseId
                        FROM CareerCoursePrerequisites p
                        INNER JOIN CareerCourses cc ON cc.CareerCourseId = p.CareerCourseId
                        INNER JOIN CareerCourses ccPrereq ON ccPrereq.CareerCourseId = p.PrerequisiteCareerCourseId
                        WHERE cc.CareerPensumId = @CareerPensumId
                          AND p.IsActive = 1
                          AND cc.IsActive = 1
                          AND ccPrereq.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CareerPensumId", careerPensumId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add((
                                    Convert.ToInt32(reader["CourseId"]),
                                    Convert.ToInt32(reader["PrerequisiteCourseId"])
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER PREREQUISITOS DEL PENSUM: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }
    }
}