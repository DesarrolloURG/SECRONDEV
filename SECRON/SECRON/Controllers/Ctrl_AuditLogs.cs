using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SECRON.Controllers
{
    public static class Ctrl_AuditLogs
    {
        public static List<Mdl_AuditLog> BuscarAuditLogs(
            string textoBusqueda,
            string tipoFiltro,
            string accion,
            int pageNumber,
            int pageSize,
            out int totalRegistros)
        {
            var lista = new List<Mdl_AuditLog>();
            totalRegistros = 0;

            string busqueda = string.IsNullOrWhiteSpace(textoBusqueda) ? null : textoBusqueda.Trim();
            tipoFiltro = string.IsNullOrWhiteSpace(tipoFiltro) ? "TODOS" : tipoFiltro.ToUpper();
            accion = string.IsNullOrWhiteSpace(accion) ? "TODOS" : accion.ToUpper();

            string query = @"
                SELECT
                    a.TableName AS Tabla,
                    b.FieldName AS Campo,
                    b.OldValue AS ValorAnterior,
                    b.NewValue AS ValorNuevo,
                    a.Action,
                    a.ActionDate,
                    a.HostName,
                    a.IPAddress,
                    ISNULL(c.Username, '<NO REGISTRADO>') AS Username,
                    c.FullName,
                    d.RoleName AS Rol,
                    COUNT(*) OVER() AS TotalRegistros
                FROM AuditMaster a
                INNER JOIN AuditDetail b ON a.AuditId = b.AuditId
                LEFT JOIN Users c ON a.UserId = c.UserId
                LEFT JOIN Roles d ON c.RoleId = d.RoleId
                WHERE
                    (@Accion = 'TODOS' OR a.Action = @Accion)
                    AND (
                        @Busqueda IS NULL
                        OR (
                            (@TipoFiltro = 'TABLA' AND a.TableName LIKE '%' + @Busqueda + '%')
                            OR (@TipoFiltro = 'CAMPO' AND b.FieldName LIKE '%' + @Busqueda + '%')
                            OR (@TipoFiltro = 'USUARIO' AND (c.Username LIKE '%' + @Busqueda + '%' OR c.FullName LIKE '%' + @Busqueda + '%'))
                            OR (@TipoFiltro = 'VALOR ANTERIOR' AND b.OldValue LIKE '%' + @Busqueda + '%')
                            OR (@TipoFiltro = 'VALOR NUEVO' AND b.NewValue LIKE '%' + @Busqueda + '%')
                            OR (@TipoFiltro = 'TODOS' AND (
                                    a.TableName LIKE '%' + @Busqueda + '%'
                                    OR b.FieldName LIKE '%' + @Busqueda + '%'
                                    OR c.Username LIKE '%' + @Busqueda + '%'
                                    OR c.FullName LIKE '%' + @Busqueda + '%'
                                    OR b.OldValue LIKE '%' + @Busqueda + '%'
                                    OR b.NewValue LIKE '%' + @Busqueda + '%'
                                ))
                        )
                    )
                ORDER BY a.ActionDate DESC
                OFFSET (@PageNumber - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY;";

            using (SqlConnection conexion = DatabaseConfig.StartConection())
            using (SqlCommand comando = new SqlCommand(query, conexion))
            {
                comando.CommandType = CommandType.Text;

                comando.Parameters.AddWithValue("@Busqueda", (object)busqueda ?? DBNull.Value);
                comando.Parameters.AddWithValue("@TipoFiltro", tipoFiltro);
                comando.Parameters.AddWithValue("@Accion", accion);
                comando.Parameters.AddWithValue("@PageNumber", pageNumber);
                comando.Parameters.AddWithValue("@PageSize", pageSize);

                using (SqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var log = new Mdl_AuditLog
                        {
                            Tabla = reader["Tabla"] as string,
                            Campo = reader["Campo"] as string,
                            ValorAnterior = reader["ValorAnterior"] as string,
                            ValorNuevo = reader["ValorNuevo"] as string,
                            Action = reader["Action"] as string,
                            ActionDate = reader["ActionDate"] != DBNull.Value ? Convert.ToDateTime(reader["ActionDate"]) : DateTime.MinValue,
                            HostName = reader["HostName"] as string,
                            IPAddress = reader["IPAddress"] as string,
                            Username = reader["Username"] as string,
                            FullName = reader["FullName"] as string,
                            Rol = reader["Rol"] as string,
                            TotalRegistros = Convert.ToInt32(reader["TotalRegistros"])
                        };

                        lista.Add(log);
                    }
                }
            }

            if (lista.Count > 0)
                totalRegistros = lista[0].TotalRegistros;

            return lista;
        }
    }
}