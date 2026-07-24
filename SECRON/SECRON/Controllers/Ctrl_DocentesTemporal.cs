using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    public static class Ctrl_DocentesTemporal
    {
        // Obtiene el próximo código de contrato (UR2-CSP-XXX-AAAA) para el año indicado
        public static string ObtenerProximoCodigo(int anio)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_DocentesTemporal_ObtenerProximoCodigo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Anio", anio);

                    conn.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER PRÓXIMO CÓDIGO DE CONTRATO: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        // Busca si ya existe un docente (maestro) cargado con ese DPI.
        // Devuelve null si no existe.
        public static Mdl_DocentesTemporal ObtenerPorDPI(string dpi)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_DocentesTemporal_ObtenerPorDPI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DPI", dpi);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Mdl_DocentesTemporal
                            {
                                TeacherTempId = Convert.ToInt32(reader["TeacherTempId"]),
                                ContractCode = reader["ContractCode"]?.ToString()
                            };
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL BUSCAR DOCENTE POR DPI: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Inserta el docente (maestro). Devuelve el TeacherTempId recién creado (0 si falló).
        public static int Insert(Mdl_DocentesTemporal docente)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_DocentesTemporal_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ContractCode", docente.ContractCode);
                    cmd.Parameters.AddWithValue("@DPI", docente.DPI);
                    cmd.Parameters.AddWithValue("@FirstName", docente.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", docente.LastName);
                    cmd.Parameters.AddWithValue("@BirthDate", (object)docente.BirthDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaritalStatus", (object)docente.MaritalStatus ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", (object)docente.Gender ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)docente.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nationality", (object)docente.Nationality ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CollegiateNumber", (object)docente.CollegiateNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NIT", (object)docente.NIT ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cycle", (object)docente.Cycle ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContractYear", (object)docente.ContractYear ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IssueDate", (object)docente.IssueDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UsuarioId", (object)docente.CreatedBy ?? DBNull.Value);

                    conn.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL REGISTRAR DOCENTE (CONTRATO): " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Lista todos los docentes (maestro) cargados, con el total de cursos de cada uno
        public static List<Mdl_DocentesTemporal> Select()
        {
            var lista = new List<Mdl_DocentesTemporal>();
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_DocentesTemporal_Select", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearDocente(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR DOCENTES (CONTRATOS): " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // Lista los docentes (una fila por docente) que tienen al menos un curso en la sede indicada.
        // El documento a generar por cada docente siempre incluye TODOS sus cursos (todas las sedes) --
        // este método solo sirve para UBICARLO en el grid filtrado por sede.
        public static List<Mdl_DocentesTemporal> ObtenerPorSede(string academicLocation)
        {
            var lista = new List<Mdl_DocentesTemporal>();
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_DocentesTemporal_SelectPorSede", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AcademicLocation", academicLocation);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearDocente(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR DOCENTES POR SEDE: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // Mapeo común de un registro de DocentesTemporal (usado por Select y ObtenerPorSede)
        private static Mdl_DocentesTemporal MapearDocente(SqlDataReader reader)
        {
            return new Mdl_DocentesTemporal
            {
                TeacherTempId = Convert.ToInt32(reader["TeacherTempId"]),
                ContractCode = reader["ContractCode"]?.ToString(),
                DPI = reader["DPI"]?.ToString(),
                FirstName = reader["FirstName"]?.ToString(),
                LastName = reader["LastName"]?.ToString(),
                BirthDate = reader["BirthDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["BirthDate"]),
                MaritalStatus = reader["MaritalStatus"]?.ToString(),
                Gender = reader["Gender"]?.ToString(),
                Address = reader["Address"]?.ToString(),
                Nationality = reader["Nationality"]?.ToString(),
                CollegiateNumber = reader["CollegiateNumber"]?.ToString(),
                NIT = reader["NIT"]?.ToString(),
                Cycle = reader["Cycle"]?.ToString(),
                ContractYear = reader["ContractYear"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ContractYear"]),
                IssueDate = reader["IssueDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["IssueDate"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CreatedBy"]),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["CreatedDate"]),
                TotalCursos = Convert.ToInt32(reader["TotalCursos"])
            };
        }
    }
}