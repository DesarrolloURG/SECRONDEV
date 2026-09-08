using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_AcademicProcesses_ReviserLocations
    {
        public static List<Mdl_AcademicProcesses_ReviserLocation> ObtenerSedesAsignadas(int reviserId)
        {
            var lista = new List<Mdl_AcademicProcesses_ReviserLocation>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_AcademicProcesses_ReviserLocations_GetByReviser", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReviserId", reviserId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Mdl_AcademicProcesses_ReviserLocation
                            {
                                ReviserLocationId = Convert.ToInt32(reader["ReviserLocationId"]),
                                ReviserId = Convert.ToInt32(reader["ReviserId"]),
                                LocationId = Convert.ToInt32(reader["LocationId"]),
                                LocationCode = reader["LocationCode"]?.ToString(),
                                LocationName = reader["LocationName"]?.ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                AssignedBy = reader["AssignedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["AssignedBy"]),
                                AssignedByName = reader["AssignedByName"]?.ToString(),
                                AssignedDate = Convert.ToDateTime(reader["AssignedDate"]),
                                RemovedBy = reader["RemovedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["RemovedBy"]),
                                RemovedByName = reader["RemovedByName"]?.ToString(),
                                RemovedDate = reader["RemovedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["RemovedDate"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR SEDES ASIGNADAS: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static List<Mdl_AcademicProcesses_ReviserLocation> BuscarSedesDisponibles(int reviserId, string textoBusqueda)
        {
            var lista = new List<Mdl_AcademicProcesses_ReviserLocation>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_AcademicProcesses_ReviserLocations_SearchAvailable", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReviserId", reviserId);
                    cmd.Parameters.AddWithValue("@TextoBusqueda", (object)textoBusqueda ?? DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Mdl_AcademicProcesses_ReviserLocation
                            {
                                ReviserId = reviserId,
                                LocationId = Convert.ToInt32(reader["LocationId"]),
                                LocationCode = reader["LocationCode"]?.ToString(),
                                LocationName = reader["LocationName"]?.ToString(),
                                IsActive = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL BUSCAR SEDES DISPONIBLES: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        // Lógica de negocio: decide Insert vs Reactivar (si ya existía inactiva para esa combinación)
        public static int AsignarSede(int reviserId, int locationId, int assignedBy)
        {
            try
            {
                int? reviserLocationIdInactivo = BuscarReviserLocationIdInactivo(reviserId, locationId);

                if (reviserLocationIdInactivo.HasValue)
                    return CambiarEstadoSede(reviserLocationIdInactivo.Value, 2, assignedBy);

                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_AcademicProcesses_ReviserLocations_Insert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReviserId", reviserId);
                    cmd.Parameters.AddWithValue("@LocationId", locationId);
                    cmd.Parameters.AddWithValue("@AssignedBy", assignedBy);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ASIGNAR SEDE: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int RemoverSede(int reviserLocationId, int removedBy)
        {
            return CambiarEstadoSede(reviserLocationId, 1, removedBy);
        }

        private static int CambiarEstadoSede(int reviserLocationId, int modo, int actionByUserId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_AcademicProcesses_ReviserLocations_Update", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReviserLocationId", reviserLocationId);
                    cmd.Parameters.AddWithValue("@Mode", modo);
                    cmd.Parameters.AddWithValue("@ActionByUserId", actionByUserId);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR SEDE DEL REVISOR: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // Busca si ya existe un registro INACTIVO para esa combinación (para reactivar en vez de duplicar)
        private static int? BuscarReviserLocationIdInactivo(int reviserId, int locationId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT ReviserLocationId FROM AcademicProcesses_ReviserLocations WHERE ReviserId = @ReviserId AND LocationId = @LocationId AND IsActive = 0",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@ReviserId", reviserId);
                    cmd.Parameters.AddWithValue("@LocationId", locationId);

                    object result = cmd.ExecuteScalar();
                    return result == null ? (int?)null : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL VERIFICAR SEDE: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}