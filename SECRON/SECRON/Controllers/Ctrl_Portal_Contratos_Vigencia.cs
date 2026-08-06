using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    public static class Ctrl_Portal_Contratos_Vigencia
    {
        // Inserta una nueva ventana de vigencia. Si @Activo = 1, desactiva cualquier otra activa.
        public static int Insert(Mdl_Portal_Contratos_Vigencia vigencia)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_Portal_Contratos_Vigencia_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaInicio", vigencia.FechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", vigencia.FechaFin);
                    cmd.Parameters.AddWithValue("@Activo", vigencia.Activo);
                    cmd.Parameters.AddWithValue("@Observaciones", (object)vigencia.Observaciones?.ToUpper() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object)vigencia.CreatedBy ?? DBNull.Value);

                    conn.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL REGISTRAR VIGENCIA DEL PORTAL: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // @Mode: 0 = actualizar normal, 1 = inactivar, 2 = reactivar
        public static int Update(int vigenciaId, byte mode, DateTime? fechaInicio, DateTime? fechaFin,
                                  string observaciones, int modifiedBy)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_Portal_Contratos_Vigencia_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VigenciaId", vigenciaId);
                    cmd.Parameters.AddWithValue("@Mode", mode);
                    cmd.Parameters.AddWithValue("@FechaInicio", (object)fechaInicio ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaFin", (object)fechaFin ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Observaciones", (object)observaciones?.ToUpper() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    conn.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR VIGENCIA DEL PORTAL: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Lista el historial completo de ventanas de vigencia
        public static List<Mdl_Portal_Contratos_Vigencia> Select()
        {
            var lista = new List<Mdl_Portal_Contratos_Vigencia>();
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_Portal_Contratos_Vigencia_Select", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearVigencia(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR VIGENCIAS DEL PORTAL: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // Devuelve la vigencia activa si la fecha actual está dentro del rango. Null si el portal está cerrado.
        public static Mdl_Portal_Contratos_Vigencia ObtenerVigente()
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_Portal_Contratos_Vigencia_ObtenerVigente", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapearVigencia(reader);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL VERIFICAR VIGENCIA DEL PORTAL: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private static Mdl_Portal_Contratos_Vigencia MapearVigencia(SqlDataReader reader)
        {
            return new Mdl_Portal_Contratos_Vigencia
            {
                VigenciaId = Convert.ToInt32(reader["VigenciaId"]),
                FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                FechaFin = Convert.ToDateTime(reader["FechaFin"]),
                Activo = Convert.ToBoolean(reader["Activo"]),
                Observaciones = reader["Observaciones"] == DBNull.Value ? null : reader["Observaciones"].ToString(),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CreatedBy"]),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ModifiedBy"]),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ModifiedDate"])
            };
        }

        // Elimina físicamente un periodo (solo debe usarse si NO está activo; la validación la hace el formulario)
        public static int Delete(int vigenciaId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SP_Portal_Contratos_Vigencia_Delete", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VigenciaId", vigenciaId);

                    conn.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ELIMINAR PERIODO: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
}