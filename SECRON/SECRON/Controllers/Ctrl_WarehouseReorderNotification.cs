using SECRON.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_WarehouseReorderNotification
    {
        private class ArticuloEnReorden
        {
            public int ItemId { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public decimal CurrentStock { get; set; }
            public decimal ReorderPoint { get; set; }
        }

        // MÉTODO PRINCIPAL: llamar después de cualquier movimiento exitoso que afecte la bodega
        public static void VerificarYNotificar(int warehouseId)
        {
            try
            {
                ResolverArticulosRecuperados(warehouseId);

                List<ArticuloEnReorden> enReorden = ObtenerArticulosEnReorden(warehouseId);
                if (enReorden.Count == 0) return;

                List<int> nuevos = ObtenerArticulosNuevosEnReorden(warehouseId, enReorden);
                if (nuevos.Count == 0) return;

                RegistrarNotificacionesActivas(warehouseId, nuevos);

                bool esCentral = EsBodegaCentral(warehouseId);
                List<string> destinatarios = ObtenerDestinatarios(warehouseId, esCentral);

                if (destinatarios.Count == 0) return;

                string nombreBodega = ObtenerNombreBodega(warehouseId);
                EnviarCorreoReorden(nombreBodega, enReorden, destinatarios);
            }
            catch (Exception ex)
            {
                NotificarErrorAAdministradores(ex, warehouseId);
            }
        }
        #region Deteccion

        private static void NotificarErrorAAdministradores(Exception error, int warehouseId)
        {
            try
            {
                List<string> correosAdmin = ObtenerCorreosAdministradores();
                if (correosAdmin.Count == 0) return;

                string correoEmisor = "notificaciones@uregionalregion2.edu.gt";
                string contraseñaEmisor = "F0rza01.";

                SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(correoEmisor, contraseñaEmisor),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(correoEmisor, "Notificaciones SECRON"),
                    Subject = "ERROR - Falló notificación de reorder point",
                    IsBodyHtml = true
                };

                mail.Body = $@"
        <html>
        <body style='font-family: Calibri, Arial, sans-serif; color:#333;'>
            <p>Ocurrió un error al verificar/notificar el punto de reorden para la bodega con ID <strong>{warehouseId}</strong>.</p>
            <p><strong>Mensaje de error:</strong></p>
            <pre style='background:#F2F2F2; padding:10px; border:1px solid #D9D9D9;'>{System.Net.WebUtility.HtmlEncode(error.ToString())}</pre>
            <p>Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
        </body>
        </html>";

                foreach (string correo in correosAdmin)
                    mail.To.Add(correo);

                smtpClient.Send(mail);
            }
            catch
            {
                // Si también falla el envío del correo de error (ej. SMTP caído), morir en silencio
            }
        }

        private static List<string> ObtenerCorreosAdministradores()
        {
            List<string> correos = new List<string>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT b.InstitutionalEmail
                FROM Roles a, Users b
                WHERE a.RoleId = b.RoleId
                  AND (a.RoleName IN ('SUPERADMIN') OR b.Username IN ('JTORRES','PHERNANDEZ'))
                  AND b.InstitutionalEmail IS NOT NULL";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            correos.Add(reader["InstitutionalEmail"].ToString());
                    }
                }
            }
            catch
            {
                // si ni siquiera se puede consultar la BD, no hay nada más que hacer
            }
            return correos;
        }

        private static List<ArticuloEnReorden> ObtenerArticulosEnReorden(int warehouseId)
        {
            List<ArticuloEnReorden> lista = new List<ArticuloEnReorden>();
            using (SqlConnection connection = DatabaseConfig.StartConection())
            {
                string query = @"SELECT i.ItemId, i.ItemCode, i.ItemName, s.CurrentStock, s.ReorderPoint
                    FROM ItemWarehouseStock s
                    INNER JOIN Items i ON i.ItemId = s.ItemId
                    WHERE s.WarehouseId = @WarehouseId
                      AND s.ReorderPoint IS NOT NULL
                      AND s.CurrentStock <= s.ReorderPoint
                    ORDER BY i.ItemName";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new ArticuloEnReorden
                            {
                                ItemId = Convert.ToInt32(reader["ItemId"]),
                                ItemCode = reader["ItemCode"].ToString(),
                                ItemName = reader["ItemName"].ToString(),
                                CurrentStock = Convert.ToDecimal(reader["CurrentStock"]),
                                ReorderPoint = Convert.ToDecimal(reader["ReorderPoint"])
                            });
                        }
                    }
                }
            }
            return lista;
        }

        // Resuelve (libera) los artículos que ya no están en reorden, para permitir notificar de nuevo a futuro
        private static void ResolverArticulosRecuperados(int warehouseId)
        {
            using (SqlConnection connection = DatabaseConfig.StartConection())
            using (SqlCommand cmd = new SqlCommand("SP_WarehouseReorderNotifications_ResolveRecovered", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                cmd.ExecuteScalar();
            }
        }

        // Determina cuáles de los artículos actualmente en reorden aún no tienen notificación activa
        private static List<int> ObtenerArticulosNuevosEnReorden(int warehouseId, List<ArticuloEnReorden> enReorden)
        {
            List<int> yaNotificados = new List<int>();
            using (SqlConnection connection = DatabaseConfig.StartConection())
            {
                string query = @"SELECT ItemId FROM WarehouseReorderNotifications
                    WHERE WarehouseId = @WarehouseId AND ResolvedDate IS NULL";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            yaNotificados.Add(Convert.ToInt32(reader["ItemId"]));
                    }
                }
            }

            return enReorden
                .Select(a => a.ItemId)
                .Where(id => !yaNotificados.Contains(id))
                .ToList();
        }

        private static void RegistrarNotificacionesActivas(int warehouseId, List<int> itemIds)
        {
            using (SqlConnection connection = DatabaseConfig.StartConection())
            using (SqlCommand cmd = new SqlCommand("SP_WarehouseReorderNotifications_InsertBatch", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                cmd.Parameters.AddWithValue("@ItemIdsJson", "[" + string.Join(",", itemIds) + "]");

                cmd.ExecuteScalar();
            }
        }

        #endregion Deteccion
        #region Destinatarios

        private static bool EsBodegaCentral(int warehouseId)
        {
            int? centralId = Ctrl_Warehouses.ObtenerBodegaCentralId();
            return centralId.HasValue && centralId.Value == warehouseId;
        }

        private static string ObtenerNombreBodega(int warehouseId)
        {
            using (SqlConnection connection = DatabaseConfig.StartConection())
            {
                string query = "SELECT WarehouseName FROM Warehouses WHERE WarehouseId = @WarehouseId";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                    object resultado = cmd.ExecuteScalar();
                    return resultado?.ToString() ?? "BODEGA";
                }
            }
        }

        // Si esCentral=true, solo notifica a encargados de la central
        // Si esCentral=false, notifica a encargados de esa bodega + encargados de la central
        private static List<string> ObtenerDestinatarios(int warehouseId, bool esCentral)
        {
            List<int> warehouseIds = new List<int> { warehouseId };

            if (!esCentral)
            {
                int? centralId = Ctrl_Warehouses.ObtenerBodegaCentralId();
                if (centralId.HasValue)
                    warehouseIds.Add(centralId.Value);
            }

            List<string> correos = new List<string>();
            using (SqlConnection connection = DatabaseConfig.StartConection())
            {
                string query = @"SELECT DISTINCT u.InstitutionalEmail
                    FROM WarehouseManagers wm
                    INNER JOIN Users u ON u.UserId = wm.UserId
                    WHERE wm.WarehouseId IN (" + string.Join(",", warehouseIds) + @")
                      AND wm.IsActive = 1
                      AND u.InstitutionalEmail IS NOT NULL
                      AND u.InstitutionalEmail <> ''";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        correos.Add(reader["InstitutionalEmail"].ToString());
                }
            }
            return correos;
        }

        #endregion Destinatarios
        #region Correo

        private static void EnviarCorreoReorden(string nombreBodega, List<ArticuloEnReorden> articulos, List<string> destinatarios)
        {
            string correoEmisor = "notificaciones@uregionalregion2.edu.gt";
            string contraseñaEmisor = "F0rza01.";

            SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(correoEmisor, contraseñaEmisor),
                EnableSsl = true
            };

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(correoEmisor, "Notificaciones SECRON"),
                Subject = $"Alerta de reabastecimiento - {nombreBodega.ToUpper()}",
                IsBodyHtml = true,
                Priority = MailPriority.High
            };

            StringBuilder filas = new StringBuilder();
            bool alterna = false;
            foreach (var a in articulos)
            {
                string fondoFila = alterna ? "#F2F2F2" : "#FFFFFF";
                alterna = !alterna;

                filas.Append($@"
                <tr style='background-color:{fondoFila};'>
                    <td style='padding:10px 14px; border:1px solid #D9D9D9; font-family:Calibri,Arial,sans-serif; font-size:13px;'>{a.ItemCode}</td>
                    <td style='padding:10px 14px; border:1px solid #D9D9D9; font-family:Calibri,Arial,sans-serif; font-size:13px;'>{a.ItemName}</td>
                    <td style='padding:10px 14px; border:1px solid #D9D9D9; font-family:Calibri,Arial,sans-serif; font-size:13px; text-align:center;'>{a.CurrentStock:0.##}</td>
                    <td style='padding:10px 14px; border:1px solid #D9D9D9; font-family:Calibri,Arial,sans-serif; font-size:13px; text-align:center;'>{a.ReorderPoint:0.##}</td>
                </tr>");
            }

            mail.Body = $@"
            <html>
            <body style='font-family: Calibri, Arial, sans-serif; color:#333; margin:0; padding:0;'>
                <div style='max-width:680px; margin:0 auto; padding:24px;'>

                    <p style='text-align:right; font-size:13px; color:#555; margin:0 0 18px 0;'>
                        Guatemala, {DateTime.Now:d 'DE' MMMM 'DE' yyyy}
                    </p>

                    <h2 style='text-align:center; font-size:18px; letter-spacing:0.5px; color:#1F1F1F; margin:0 0 24px 0; text-transform:uppercase;'>
                        Alerta de Reabastecimiento de Inventario
                    </h2>

                    <table style='width:100%; border-collapse:collapse; margin-bottom:18px; font-size:13px;'>
                        <tr>
                            <td style='padding:4px 0; width:140px; font-weight:bold; color:#1F1F1F;'>BODEGA:</td>
                            <td style='padding:4px 0; color:#1F1F1F;'>{nombreBodega.ToUpper()}</td>
                        </tr>
                        <tr>
                            <td style='padding:4px 0; font-weight:bold; color:#1F1F1F;'>DE:</td>
                            <td style='padding:4px 0; color:#1F1F1F;'>SISTEMA DE CONTROL DE INVENTARIO - SECRON</td>
                        </tr>
                    </table>

                    <p style='font-size:13px; line-height:1.6; color:#333; margin:0 0 18px 0; text-align:justify;'>
                        Por medio de la presente se notifica que los siguientes artículos en la bodega indicada
                        se encuentran en o por debajo de su punto de reorden establecido, por lo que se recomienda
                        gestionar el reabastecimiento correspondiente a la brevedad posible.
                    </p>

                    <table style='width:100%; border-collapse:collapse; margin-bottom:20px;'>
                        <tr style='background-color:#2F5496;'>
                            <th style='padding:10px 14px; border:1px solid #2F5496; color:#FFFFFF; font-size:12px; text-align:left; text-transform:uppercase;'>Código</th>
                            <th style='padding:10px 14px; border:1px solid #2F5496; color:#FFFFFF; font-size:12px; text-align:left; text-transform:uppercase;'>Artículo</th>
                            <th style='padding:10px 14px; border:1px solid #2F5496; color:#FFFFFF; font-size:12px; text-align:center; text-transform:uppercase;'>Stock Actual</th>
                            <th style='padding:10px 14px; border:1px solid #2F5496; color:#FFFFFF; font-size:12px; text-align:center; text-transform:uppercase;'>Punto de Reorden</th>
                        </tr>
                        {filas}
                    </table>

                    <p style='font-size:13px; color:#555; margin:24px 0 4px 0;'>Atentamente,</p>
                    <p style='font-size:13px; color:#1F1F1F; font-weight:bold; margin:0;'>Servicio Automático de Notificaciones, SECRON</p>
                    <p style='font-size:13px; color:#1F1F1F; margin:0;'>Universidad Regional de Guatemala, Región 2</p>

                </div>
            </body>
            </html>";

            //foreach (string correo in destinatarios)
                //mail.To.Add(correo);

            mail.To.Add("phernandez@uregionalregion2.edu.gt");

            smtpClient.Send(mail);
        }

        #endregion Correo
    }
}