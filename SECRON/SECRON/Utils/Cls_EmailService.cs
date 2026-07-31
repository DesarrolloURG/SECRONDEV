using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using SECRON.Utils;

namespace SECRON.Controllers
{
    // Punto único de envío de correo para toda la aplicación.
    // Evita repetir la construcción de SmtpClient/credenciales en cada controlador.
    internal static class Cls_EmailService
    {
        /// <summary>
        /// Envía un correo usando la configuración SMTP cacheada.
        /// </summary>
        /// <param name="destinatarios">Lista de correos en el campo "Para".</param>
        /// <param name="asunto">Asunto del correo.</param>
        /// <param name="cuerpoHtml">Cuerpo del correo en HTML.</param>
        /// <param name="conCopia">Lista opcional de correos en copia (CC).</param>
        /// <param name="nombreRemitente">Nombre visible del remitente (por defecto "Notificaciones SECRON").</param>
        /// <param name="prioridadAlta">Si es true, marca el correo como prioridad alta.</param>
        /// <returns>true si se envió correctamente; false si falló (no lanza excepción).</returns>
        public static bool EnviarCorreo(
            List<string> destinatarios,
            string asunto,
            string cuerpoHtml,
            List<string> conCopia = null,
            string nombreRemitente = "Notificaciones SECRON",
            bool prioridadAlta = false)
        {
            if (destinatarios == null || destinatarios.Count == 0)
                return false;

            if (!Cls_EmailConfigCache.IsLoaded)
            {
                System.Diagnostics.Debug.WriteLine("Configuración SMTP no cargada. No se pudo enviar el correo.");
                return false;
            }

            try
            {
                string correoEmisor = Cls_EmailConfigCache.SmtpUser;

                using (var smtpClient = new SmtpClient(Cls_EmailConfigCache.SmtpServer)
                {
                    Port = Cls_EmailConfigCache.SmtpPort,
                    Credentials = new NetworkCredential(correoEmisor, Cls_EmailConfigCache.ObtenerPasswordDescifrado()),
                    EnableSsl = Cls_EmailConfigCache.SmtpEnableSsl
                })
                using (var mail = new MailMessage
                {
                    From = new MailAddress(correoEmisor, nombreRemitente),
                    Subject = asunto,
                    Body = cuerpoHtml,
                    IsBodyHtml = true,
                    Priority = prioridadAlta ? MailPriority.High : MailPriority.Normal
                })
                {
                    foreach (string correo in destinatarios)
                        mail.To.Add(correo);

                    if (conCopia != null)
                    {
                        foreach (string correo in conCopia)
                            mail.CC.Add(correo);
                    }

                    smtpClient.Send(mail);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar correo: {ex.Message}");
                return false;
            }
        }
    }
}