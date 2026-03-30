using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace SECRON
{
    public partial class Frm_System_Splash : Form
    {
        #region PropiedadesIniciales
        // Declaraciones y Asignaciones iniciales
        private int counter;
        private bool isClosing = false;
        public Frm_System_Splash()
        {
            InitializeComponent();
            InitializeSplashProperties();
            counter = 0;
        }
        // Configuración inicial del formulario
        private void InitializeSplashProperties()
        {
            // Configuración profesional del formulario
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;

            // Configurar double buffering para evitar parpadeo
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.DoubleBuffer |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.OptimizedDoubleBuffer, true);

            this.UpdateStyles();
        }
        // Crear región redondeada
        private Region CreateRoundedRegion(int width, int height, int radius)
        {
            GraphicsPath path = null;
            try
            {
                path = new GraphicsPath();

                // Validar parámetros
                if (width <= 0 || height <= 0 || radius <= 0)
                    return new Region(new Rectangle(0, 0, Math.Max(width, 1), Math.Max(height, 1)));

                // Limitar el radio para evitar formas extrañas
                int maxRadius = Math.Min(width, height) / 2;
                radius = Math.Min(radius, maxRadius);

                path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
                path.AddArc(width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
                path.AddArc(width - radius * 2, height - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(0, height - radius * 2, radius * 2, radius * 2, 90, 90);
                path.CloseAllFigures();

                return new Region(path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando región redondeada: {ex.Message}");
                return new Region(new Rectangle(0, 0, width, height));
            }
            finally
            {
                path?.Dispose();
            }
        }
        #endregion PropiedadesIniciales
        #region EventosDeFormulario
        // Evento Load del formulario
        private void Frm_System_Splash_Load(object sender, EventArgs e)
        {
            try
            {
                // Aplicar bordes redondeados
                if (this.Width > 0 && this.Height > 0)
                {
                    this.Region = CreateRoundedRegion(this.Width, this.Height, 15);
                }

                // Asegurar posicionamiento correcto
                this.CenterToScreen();

                // Iniciar con fade in
                FadeIn();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en Load: {ex.Message}");
            }
        }
        // Evento Prevenir que se pueda cerrar el form accidentalmente
        private void Frm_System_Splash_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isClosing && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; // Cancelar el cierre si no es intencional
            }
        }
        #endregion Eventos EventosDeFormulario
        #region TimerSplash
        // Método del timer
        private void timerSplash_Tick(object sender, EventArgs e)
        {
            if (isClosing) return;

            counter++;
            if (counter >= 30) // 3 segundos (30 * 100ms)
            {
                FinalizarSplashYMostrarLogin();
            }
        }
        #endregion TimerSplash
        #region Procedimientos y Funciones
        // Finalizar el splash y mostrar el login
        private void FinalizarSplashYMostrarLogin()
        {
            if (isClosing) return;
            isClosing = true;

            try
            {
                // Detener el timer
                Timer_Splash.Stop();

                // Crear el formulario de login
                var login = new Frm_Security_Login();

                // Suscribirse al evento de cierre del login para saber el resultado
                login.FormClosed += Login_FormClosed;

                // Ocultar el splash
                this.Hide();

                // Mostrar el login con Show() en lugar de ShowDialog()
                login.Show();
            }
            catch (Exception ex)
            {
                // Manejo de errores mejorado
                MessageBox.Show($"Error al inicializar el sistema:\n\n{ex.Message}\n\nLa aplicación se cerrará.",
                              "Error Crítico",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                Application.Exit();
            }
        }
        // Manejar el cierre del formulario de login
        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            var login = sender as Frm_Security_Login;

            if (login != null)
            {
                // Verificar si el login fue exitoso
                // Esto asume que tu login tiene una propiedad LoginExitoso o similar
                // Ajusta según tu implementación
                if (login.DialogResult == DialogResult.OK)
                {
                    // Login exitoso - el login debería haber abierto ya el MDI
                    // Solo cerrar el splash
                    this.Close();
                }
                else
                {
                    // Login cancelado o falló - cerrar aplicación
                    this.Close();
                    Application.Exit();
                }
            }
        }
        // Efecto de fade in
        private void FadeIn()
        {
            // Versión simplificada sin async para evitar advertencias
            // Procedimiento que permite que el formulario aparezca gradualmente, mejorando la experiencia visual
            Timer fadeTimer = new Timer();
            double opacity = 0;

            fadeTimer.Interval = 50;
            fadeTimer.Tick += (s, e) =>
            {
                opacity += 0.1;
                if (opacity >= 1.0)
                {
                    this.Opacity = 1.0;
                    fadeTimer.Stop();
                    fadeTimer.Dispose();
                }
                else
                {
                    this.Opacity = opacity;
                }
            };

            this.Opacity = 0;
            fadeTimer.Start();
        }
        #endregion Procedimientos y Funciones
    }
}