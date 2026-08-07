using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    public static class Ctrl_Careers
    {
        #region Insertar

        public static int InsertarCarrera(Mdl_Careers carrera, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Careers_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CareerCode", carrera.CareerCode);
                        cmd.Parameters.AddWithValue("@CareerName", carrera.CareerName);
                        cmd.Parameters.AddWithValue("@Description", (object)carrera.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DurationYears", (object)carrera.DurationYears ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TotalSemesters", (object)carrera.TotalSemesters ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TotalCredits", (object)carrera.TotalCredits ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", carrera.IsActive);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL GUARDAR CARRERA: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Actualizar

        public static int ActualizarCarrera(Mdl_Careers carrera, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Careers_Update", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CareerId", carrera.CareerId);
                        cmd.Parameters.AddWithValue("@CareerCode", carrera.CareerCode);
                        cmd.Parameters.AddWithValue("@CareerName", carrera.CareerName);
                        cmd.Parameters.AddWithValue("@Description", (object)carrera.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DurationYears", (object)carrera.DurationYears ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TotalSemesters", (object)carrera.TotalSemesters ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TotalCredits", (object)carrera.TotalCredits ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR CARRERA: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region CambiarEstado

        // Modo: 1 = Inactivar, 2 = Reactivar
        public static int CambiarEstadoCarrera(int careerId, int modo, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Careers_UpdateStatus", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CareerId", careerId);
                        cmd.Parameters.AddWithValue("@Mode", modo);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CAMBIAR ESTADO DE CARRERA: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Consultar (listado con filtros y paginación)

        public static List<Mdl_Careers> ObtenerCarreras(string campo, string valor, string estado, int pageNumber, int pageSize, out int totalRows)
        {
            List<Mdl_Careers> lista = new List<Mdl_Careers>();
            totalRows = 0;

            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Careers_Select", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Campo", campo);
                        cmd.Parameters.AddWithValue("@Valor", (object)valor ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", estado);
                        cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCarrera(reader));
                                totalRows = Convert.ToInt32(reader["TotalRows"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR CARRERAS: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        // Usado por EXPORTAR: mismos filtros, sin paginación (PageSize 0 = todos)
        public static List<Mdl_Careers> ObtenerCarrerasParaExportar(string campo, string valor, string estado)
        {
            int totalRows;
            return ObtenerCarreras(campo, valor, estado, 1, 0, out totalRows);
        }

        #endregion

        #region Importar (reutiliza Insert; rechaza duplicados por UNIQUE)

        public static int ImportarCarrera(Mdl_Careers carrera, int usuarioId)
        {
            return InsertarCarrera(carrera, usuarioId);
        }

        #endregion

        #region Mapeo

        private static Mdl_Careers MapearCarrera(SqlDataReader reader)
        {
            return new Mdl_Careers
            {
                CareerId = Convert.ToInt32(reader["CareerId"]),
                CareerCode = reader["CareerCode"]?.ToString(),
                CareerName = reader["CareerName"]?.ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                DurationYears = reader["DurationYears"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["DurationYears"]),
                TotalSemesters = reader["TotalSemesters"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["TotalSemesters"]),
                TotalCredits = reader["TotalCredits"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["TotalCredits"]),
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