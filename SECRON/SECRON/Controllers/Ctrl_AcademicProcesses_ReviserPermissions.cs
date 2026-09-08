using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_AcademicProcesses_ReviserPermissions
    {
        public static List<Mdl_AcademicProcesses_ReviserPermission> BuscarPermisosModulo(string modulePrefix, int roleId)
        {
            var lista = new List<Mdl_AcademicProcesses_ReviserPermission>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_Permissions_SearchByModulePrefix", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ModulePrefix", modulePrefix);
                    cmd.Parameters.AddWithValue("@RoleId", roleId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Mdl_AcademicProcesses_ReviserPermission
                            {
                                PermissionId = Convert.ToInt32(reader["PermissionId"]),
                                PermissionCode = reader["PermissionCode"]?.ToString(),
                                PermissionName = reader["PermissionName"]?.ToString(),
                                Description = reader["Description"]?.ToString(),
                                ModuleName = reader["ModuleName"]?.ToString(),
                                ActionType = reader["ActionType"]?.ToString(),
                                RolePermissionId = reader["RolePermissionId"] == DBNull.Value ? (int?)null : (int?)Convert.ToInt32(reader["RolePermissionId"]),
                                EstaAsignado = Convert.ToBoolean(reader["EstaAsignado"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL BUSCAR PERMISOS: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }
    }
}