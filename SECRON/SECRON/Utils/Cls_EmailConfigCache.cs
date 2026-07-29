using SECRON.Utils;
using System;

namespace SECRON.Utils
{
    // Cache en memoria de la configuración SMTP. Se carga una vez al iniciar sesión
    // (dentro de Ctrl_Security_Auth.CargarDatosInicialesAsync, vía el SP consolidado)
    // y los controladores de notificación la consumen desde aquí, evitando
    // credenciales quemadas en código y relecturas repetidas de la BD.
    internal static class Cls_EmailConfigCache
    {
        public static string SmtpServer { get; private set; }
        public static int SmtpPort { get; private set; }
        public static string SmtpUser { get; private set; }
        public static string SmtpPasswordEncrypted { get; private set; } // Cifrado; se descifra solo al usarse
        public static bool SmtpEnableSsl { get; private set; }

        public static bool IsLoaded { get; private set; } = false;

        // Descifra la contraseña SMTP solo en el momento de uso, sin mantenerla en memoria
        public static string ObtenerPasswordDescifrado()
        {
            if (string.IsNullOrEmpty(SmtpPasswordEncrypted))
                throw new InvalidOperationException("La configuración SMTP no ha sido cargada.");

            return Cls_EmailEncryption.Decrypt(SmtpPasswordEncrypted);
        }

        // Asigna un valor individual (usado por Ctrl_Security_Auth al leer el result set consolidado)
        public static void AsignarValor(string nombre, string valor)
        {
            switch (nombre)
            {
                case "SmtpServer": SmtpServer = valor; break;
                case "SmtpPort": int.TryParse(valor, out int puerto); SmtpPort = puerto; break;
                case "SmtpUser": SmtpUser = valor; break;
                case "SmtpPasswordEncrypted": SmtpPasswordEncrypted = valor; break;
                case "SmtpEnableSsl": SmtpEnableSsl = valor == "1"; break;
            }
        }

        // Marca el cache como cargado (usado por Ctrl_Security_Auth tras AsignarValor)
        public static void MarcarComoCargado()
        {
            IsLoaded = !string.IsNullOrEmpty(SmtpServer) && !string.IsNullOrEmpty(SmtpPasswordEncrypted);
        }

        // Limpia las credenciales de memoria (llamar al cerrar sesión/aplicación)
        public static void Limpiar()
        {
            SmtpServer = null;
            SmtpPort = 0;
            SmtpUser = null;
            SmtpPasswordEncrypted = null;
            SmtpEnableSsl = false;
            IsLoaded = false;
        }
    }
}