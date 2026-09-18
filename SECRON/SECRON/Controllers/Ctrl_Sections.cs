using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_Sections
    {
        #region Insertar

        public static int InsertarSeccion(Mdl_Sections seccion, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Sections_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SectionCode", seccion.SectionCode);
                        cmd.Parameters.AddWithValue("@SectionName", seccion.SectionName);
                        cmd.Parameters.AddWithValue("@CareerId", seccion.CareerId);
                        cmd.Parameters.AddWithValue("@LocationId", seccion.LocationId);
                        cmd.Parameters.AddWithValue("@ScheduleTypeId", seccion.ScheduleTypeId);
                        cmd.Parameters.AddWithValue("@CoordinatorId", seccion.CoordinatorId);
                        cmd.Parameters.AddWithValue("@CurrentSemester", seccion.CurrentSemester);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object)seccion.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@StudentCount", seccion.StudentCount);
                        cmd.Parameters.AddWithValue("@MaxCapacity", seccion.MaxCapacity);
                        cmd.Parameters.AddWithValue("@StartDate", (object)seccion.StartDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EndDate", (object)seccion.EndDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL INSERTAR SECCIÓN: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Actualizar

        public static int ActualizarSeccion(Mdl_Sections seccion, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Sections_Update", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SectionId", seccion.SectionId);
                        cmd.Parameters.AddWithValue("@SectionCode", seccion.SectionCode);
                        cmd.Parameters.AddWithValue("@SectionName", seccion.SectionName);
                        cmd.Parameters.AddWithValue("@CareerId", seccion.CareerId);
                        cmd.Parameters.AddWithValue("@LocationId", seccion.LocationId);
                        cmd.Parameters.AddWithValue("@ScheduleTypeId", seccion.ScheduleTypeId);
                        cmd.Parameters.AddWithValue("@CoordinatorId", seccion.CoordinatorId);
                        cmd.Parameters.AddWithValue("@CurrentSemester", seccion.CurrentSemester);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object)seccion.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@StudentCount", seccion.StudentCount);
                        cmd.Parameters.AddWithValue("@MaxCapacity", seccion.MaxCapacity);
                        cmd.Parameters.AddWithValue("@StartDate", (object)seccion.StartDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EndDate", (object)seccion.EndDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Mode", 0);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR SECCIÓN: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region CambiarEstado

        // Modo: 1 = Inactivar, 2 = Reactivar
        public static int CambiarEstadoSeccion(int sectionId, int modo, int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Sections_Update", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SectionId", sectionId);
                        cmd.Parameters.AddWithValue("@Mode", modo);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CAMBIAR ESTADO DE SECCIÓN: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Consultar (listado con filtros y paginación)

        public static List<Mdl_Sections> ObtenerSecciones(string campo, string valor, int? locationId, string estado, int pageNumber, int pageSize, out int totalRows)
        {
            List<Mdl_Sections> lista = new List<Mdl_Sections>();
            totalRows = 0;

            try
            {
                using (SqlConnection conn = DatabaseConfig.StartConection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Sections_Select", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Campo", campo);
                        cmd.Parameters.AddWithValue("@Valor", (object)valor ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LocationId", (object)locationId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", estado);
                        cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearSeccion(reader));
                                totalRows = Convert.ToInt32(reader["TotalRows"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR SECCIONES: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        // Usado por EXPORTAR: mismos filtros, sin paginación (PageSize 0 = todos)
        public static List<Mdl_Sections> ObtenerSeccionesParaExportar(string campo, string valor, int? locationId, string estado)
        {
            int totalRows;
            return ObtenerSecciones(campo, valor, locationId, estado, 1, 0, out totalRows);
        }

        #endregion

        #region Mapeo

        private static Mdl_Sections MapearSeccion(SqlDataReader reader)
        {
            return new Mdl_Sections
            {
                SectionId = Convert.ToInt32(reader["SectionId"]),
                SectionCode = reader["SectionCode"]?.ToString(),
                SectionName = reader["SectionName"]?.ToString(),
                CareerId = Convert.ToInt32(reader["CareerId"]),
                CareerName = reader["CareerName"]?.ToString(),
                LocationId = Convert.ToInt32(reader["LocationId"]),
                LocationName = reader["LocationName"]?.ToString(),
                ScheduleTypeId = Convert.ToInt32(reader["ScheduleTypeId"]),
                ScheduleTypeName = reader["ScheduleTypeName"]?.ToString(),
                CoordinatorId = Convert.ToInt32(reader["CoordinatorId"]),
                CoordinatorName = reader["CoordinatorName"]?.ToString(),
                CurrentSemester = reader["CurrentSemester"] == DBNull.Value ? 1 : Convert.ToInt32(reader["CurrentSemester"]),
                AcademicYear = reader["AcademicYear"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["AcademicYear"]),
                StudentCount = reader["StudentCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["StudentCount"]),
                MaxCapacity = reader["MaxCapacity"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MaxCapacity"]),
                StartDate = reader["StartDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["StartDate"]),
                EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["EndDate"]),
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