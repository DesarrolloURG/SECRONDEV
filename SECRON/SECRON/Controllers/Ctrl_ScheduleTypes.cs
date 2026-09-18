using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    public static class Ctrl_ScheduleTypes
    {
        #region Insertar

        public static int InsertarScheduleType(Mdl_ScheduleType scheduleType, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ScheduleTypes_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ScheduleTypeCode", scheduleType.ScheduleTypeCode);
                        cmd.Parameters.AddWithValue("@ScheduleTypeName", scheduleType.ScheduleTypeName);
                        cmd.Parameters.AddWithValue("@Description", (object)scheduleType.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IncludesMonday", scheduleType.IncludesMonday);
                        cmd.Parameters.AddWithValue("@IncludesTuesday", scheduleType.IncludesTuesday);
                        cmd.Parameters.AddWithValue("@IncludesWednesday", scheduleType.IncludesWednesday);
                        cmd.Parameters.AddWithValue("@IncludesThursday", scheduleType.IncludesThursday);
                        cmd.Parameters.AddWithValue("@IncludesFriday", scheduleType.IncludesFriday);
                        cmd.Parameters.AddWithValue("@IncludesSaturday", scheduleType.IncludesSaturday);
                        cmd.Parameters.AddWithValue("@IncludesSunday", scheduleType.IncludesSunday);
                        cmd.Parameters.AddWithValue("@TimeShift", (object)scheduleType.TimeShift ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL INSERTAR JORNADA: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Actualizar

        public static int ActualizarScheduleType(Mdl_ScheduleType scheduleType, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ScheduleTypes_Update", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ScheduleTypeId", scheduleType.ScheduleTypeId);
                        cmd.Parameters.AddWithValue("@ScheduleTypeCode", scheduleType.ScheduleTypeCode);
                        cmd.Parameters.AddWithValue("@ScheduleTypeName", scheduleType.ScheduleTypeName);
                        cmd.Parameters.AddWithValue("@Description", (object)scheduleType.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IncludesMonday", scheduleType.IncludesMonday);
                        cmd.Parameters.AddWithValue("@IncludesTuesday", scheduleType.IncludesTuesday);
                        cmd.Parameters.AddWithValue("@IncludesWednesday", scheduleType.IncludesWednesday);
                        cmd.Parameters.AddWithValue("@IncludesThursday", scheduleType.IncludesThursday);
                        cmd.Parameters.AddWithValue("@IncludesFriday", scheduleType.IncludesFriday);
                        cmd.Parameters.AddWithValue("@IncludesSaturday", scheduleType.IncludesSaturday);
                        cmd.Parameters.AddWithValue("@IncludesSunday", scheduleType.IncludesSunday);
                        cmd.Parameters.AddWithValue("@TimeShift", (object)scheduleType.TimeShift ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Mode", 0);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR JORNADA: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region CambiarEstado

        // Modo: 1 = Inactivar, 2 = Reactivar
        public static int CambiarEstadoScheduleType(int scheduleTypeId, int modo, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ScheduleTypes_Update", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ScheduleTypeId", scheduleTypeId);
                        cmd.Parameters.AddWithValue("@Mode", modo);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CAMBIAR ESTADO DE JORNADA: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Consultar (listado con filtros y paginación)

        public static List<Mdl_ScheduleType> ObtenerScheduleTypes(string campo, string valor, string timeShift, string estado, int pageNumber, int pageSize, out int totalRows)
        {
            List<Mdl_ScheduleType> lista = new List<Mdl_ScheduleType>();
            totalRows = 0;

            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ScheduleTypes_Select", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Campo", campo);
                        cmd.Parameters.AddWithValue("@Valor", (object)valor ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TimeShift", timeShift);
                        cmd.Parameters.AddWithValue("@Estado", estado);
                        cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearScheduleType(reader));
                                totalRows = Convert.ToInt32(reader["TotalRows"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR JORNADAS: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        // Usado por EXPORTAR: mismos filtros, sin paginación (PageSize 0 = todos)
        public static List<Mdl_ScheduleType> ObtenerScheduleTypesParaExportar(string campo, string valor, string timeShift, string estado)
        {
            int totalRows;
            return ObtenerScheduleTypes(campo, valor, timeShift, estado, 1, 0, out totalRows);
        }

        #endregion

        #region Mapeo

        private static Mdl_ScheduleType MapearScheduleType(SqlDataReader reader)
        {
            return new Mdl_ScheduleType
            {
                ScheduleTypeId = Convert.ToInt32(reader["ScheduleTypeId"]),
                ScheduleTypeCode = reader["ScheduleTypeCode"]?.ToString(),
                ScheduleTypeName = reader["ScheduleTypeName"]?.ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                IncludesMonday = reader["IncludesMonday"] != DBNull.Value && Convert.ToBoolean(reader["IncludesMonday"]),
                IncludesTuesday = reader["IncludesTuesday"] != DBNull.Value && Convert.ToBoolean(reader["IncludesTuesday"]),
                IncludesWednesday = reader["IncludesWednesday"] != DBNull.Value && Convert.ToBoolean(reader["IncludesWednesday"]),
                IncludesThursday = reader["IncludesThursday"] != DBNull.Value && Convert.ToBoolean(reader["IncludesThursday"]),
                IncludesFriday = reader["IncludesFriday"] != DBNull.Value && Convert.ToBoolean(reader["IncludesFriday"]),
                IncludesSaturday = reader["IncludesSaturday"] != DBNull.Value && Convert.ToBoolean(reader["IncludesSaturday"]),
                IncludesSunday = reader["IncludesSunday"] != DBNull.Value && Convert.ToBoolean(reader["IncludesSunday"]),
                TimeShift = reader["TimeShift"] == DBNull.Value ? null : reader["TimeShift"].ToString(),
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