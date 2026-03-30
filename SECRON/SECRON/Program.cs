using SECRON.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON
{
    internal static class Program
    {
        #region Propiedades Globales
        /// Punto de entrada principal para la aplicación
        private static Mutex mutex = null;
        [STAThread]
        static void Main()
        {
            // Configuración básica de la aplicación
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Prevenir múltiples instancias
            const string appName = "SECRON";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("La aplicación ya está ejecutándose.", "Aplicación en uso",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Configurar manejo global de excepciones
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                // Crear y mostrar el splash
                var splash = new Frm_System_Splash();
                splash.Show();

                // Ejecutar el bucle de mensajes - esto mantiene la aplicación viva
                // hasta que todas las ventanas se cierren o se llame a Application.Exit()
                Application.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico en la aplicación:\n\n{ex.Message}",
                               "Error Fatal",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
            }
            finally
            {
                mutex?.ReleaseMutex();
                mutex?.Dispose();
            }
        }
        #endregion Propiedades Globales
        #region Manejo Global de Errores
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show($"Error no manejado:\n\n{e.Exception.Message}",
                           "Error de Aplicación",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"Error crítico no manejado:\n\n{ex.Message}",
                               "Error Fatal",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
            }
        }
        #endregion Manejo Global de Errores
    }
}
