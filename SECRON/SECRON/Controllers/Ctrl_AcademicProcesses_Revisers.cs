using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_AcademicProcesses_Revisers
    {
        // Códigos de resultado de negocio para AsignarRevisor
        public const int RESULTADO_YA_ES_REVISOR_ACTIVO = -1;
        public const int RESULTADO_ERROR = 0;

        private const string ROL_REVISOR_HORARIOS = "REVISOR DE HORARIOS";

        public static Mdl_AcademicProcesses_Reviser ObtenerPorUsuario(int userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_AcademicProcesses_Revisers_GetByUser", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Mdl_AcademicProcesses_Reviser
                            {
                                ReviserId = Convert.ToInt32(reader["ReviserId"]),
                                UserId = Convert.ToInt32(reader["UserId"]),
                                PersonType = reader["PersonType"]?.ToString(),
                                PersonId = Convert.ToInt32(reader["PersonId"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                AssignedDate = Convert.ToDateTime(reader["AssignedDate"]),
                                RemovedDate = reader["RemovedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["RemovedDate"])
                            };
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR REVISOR: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static Mdl_AcademicProcesses_Reviser ObtenerPorId(int reviserId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_AcademicProcesses_Revisers_GetById", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReviserId", reviserId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapearReviser(reader);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CONSULTAR REVISOR: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Lógica de negocio: decide Insert vs Reactivar vs Bloquear
        public static int AsignarRevisor(int userId, string personType, int personId, int assignedBy)
        {
            try
            {
                Mdl_AcademicProcesses_Reviser existente = ObtenerPorUsuario(userId);

                if (existente != null && existente.IsActive)
                    return RESULTADO_YA_ES_REVISOR_ACTIVO;

                if (!ActualizarRolARevisorDeHorarios(userId, assignedBy))
                    return RESULTADO_ERROR;

                if (existente != null && !existente.IsActive)
                    return ReactivarRevisor(existente.ReviserId, assignedBy);

                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_AcademicProcesses_Revisers_Insert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@PersonType", personType);
                    cmd.Parameters.AddWithValue("@PersonId", personId);
                    cmd.Parameters.AddWithValue("@AssignedBy", assignedBy);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ASIGNAR REVISOR: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return RESULTADO_ERROR;
            }
        }

        // Sustituye el rol actual del usuario por "REVISOR DE HORARIOS" (mismo patrón que
        // Frm_ITSM_Users_RolesPermissions: actualiza el usuario y limpia sus permisos específicos,
        // ya que ahora aplican los del nuevo rol).
        private static bool ActualizarRolARevisorDeHorarios(int userId, int modifiedBy)
        {
            var rolRevisor = Ctrl_Roles.ObtenerRolPorNombre(ROL_REVISOR_HORARIOS);
            if (rolRevisor == null)
            {
                MessageBox.Show($"NO SE ENCONTRÓ EL ROL '{ROL_REVISOR_HORARIOS}' EN EL SISTEMA.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var usuario = Ctrl_Users.ObtenerUsuarioPorId(userId);
            if (usuario == null)
                return false;

            usuario.RoleId = rolRevisor.RoleId;
            usuario.ModifiedBy = modifiedBy;

            if (Ctrl_Users.ActualizarUsuario(usuario) <= 0)
                return false;

            Ctrl_UserPermissions.EliminarTodosLosPermisosDeUsuario(userId, modifiedBy);
            return true;
        }

        public static int RemoverRevisor(int reviserId, int removedBy)
        {
            return CambiarEstadoRevisor(reviserId, 1, removedBy);
        }

        public static int ReactivarRevisor(int reviserId, int assignedBy)
        {
            return CambiarEstadoRevisor(reviserId, 2, assignedBy);
        }

        private static int CambiarEstadoRevisor(int reviserId, int modo, int actionByUserId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_AcademicProcesses_Revisers_Update", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReviserId", reviserId);
                    cmd.Parameters.AddWithValue("@Mode", modo);
                    cmd.Parameters.AddWithValue("@ActionByUserId", actionByUserId);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR REVISOR: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static List<Mdl_AcademicProcesses_Reviser> BuscarRevisores(
            string textoBusqueda, string tipoFiltro, int pageNumber, int pageSize, out int totalRegistros)
        {
            var lista = new List<Mdl_AcademicProcesses_Reviser>();
            totalRegistros = 0;

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_AcademicProcesses_Revisers_Select", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TextoBusqueda", (object)textoBusqueda ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoFiltro", (object)tipoFiltro ?? "TODOS");
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var reviser = MapearReviser(reader);
                            reviser.TotalRegistros = Convert.ToInt32(reader["TotalRegistros"]);
                            lista.Add(reviser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL BUSCAR REVISORES: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (lista.Count > 0)
                totalRegistros = lista[0].TotalRegistros;

            return lista;
        }

        // Busca candidatos a revisor entre Docentes/Trabajadores/Proveedores/Coordinadores,
        // indicando si ya son revisor activo (IsActive) y el estado de su ficha origen (PersonIsActive).
        public static List<Mdl_AcademicProcesses_Reviser> BuscarCandidatos(
            string personType, string textoBusqueda, string estadoFiltro,
            int pageNumber, int pageSize, out int totalRegistros)
        {
            var lista = new List<Mdl_AcademicProcesses_Reviser>();
            totalRegistros = 0;

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_AcademicProcesses_Revisers_SearchCandidates", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PersonType", personType);
                    cmd.Parameters.AddWithValue("@TextoBusqueda", (object)textoBusqueda ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EstadoFiltro", (object)estadoFiltro ?? "TODOS");
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var candidato = new Mdl_AcademicProcesses_Reviser
                            {
                                ReviserId = reader["ReviserId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ReviserId"]),
                                UserId = reader["UserId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["UserId"]),
                                Username = reader["Username"]?.ToString(),
                                UserFullName = reader["UserFullName"]?.ToString(),
                                PersonType = personType,
                                PersonId = Convert.ToInt32(reader["PersonId"]),
                                PersonCode = reader["PersonCode"]?.ToString(),
                                PersonName = reader["PersonName"]?.ToString(),
                                PersonIsActive = Convert.ToBoolean(reader["PersonIsActive"]),
                                IsActive = Convert.ToBoolean(reader["EsRevisorActivo"]),
                                TotalRegistros = Convert.ToInt32(reader["TotalRegistros"])
                            };
                            lista.Add(candidato);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL BUSCAR CANDIDATOS: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (lista.Count > 0)
                totalRegistros = lista[0].TotalRegistros;

            return lista;
        }

        private static Mdl_AcademicProcesses_Reviser MapearReviser(SqlDataReader reader)
        {
            return new Mdl_AcademicProcesses_Reviser
            {
                ReviserId = Convert.ToInt32(reader["ReviserId"]),
                UserId = Convert.ToInt32(reader["UserId"]),
                Username = reader["Username"]?.ToString(),
                UserFullName = reader["UserFullName"]?.ToString(),
                PersonType = reader["PersonType"]?.ToString(),
                PersonId = Convert.ToInt32(reader["PersonId"]),
                PersonCode = reader["PersonCode"]?.ToString(),
                PersonName = reader["PersonName"]?.ToString(),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                AssignedBy = reader["AssignedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["AssignedBy"]),
                AssignedByName = reader["AssignedByName"]?.ToString(),
                AssignedDate = Convert.ToDateTime(reader["AssignedDate"]),
                RemovedBy = reader["RemovedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["RemovedBy"]),
                RemovedByName = reader["RemovedByName"]?.ToString(),
                RemovedDate = reader["RemovedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["RemovedDate"])
            };
        }
    }
}