using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SECRON.Controllers;
using SECRON.Models;
using SECRON.Views;

namespace SECRON
{
    public partial class Frm_Security_Login : Form
    {
        #region PropiedadesIniciales
        //Propiedades iniciales
        private Ctrl_Security_Auth authController;
        private bool isLoading = false;

        //Constructor del formulario
        public Frm_Security_Login()
        {
            InitializeComponent();
            // Configuraciones para personalizar la barra de título
            ConfigurarBotonesBarraTitulo(false, false, true);
            // Configurar el formulario
            this.Load += Frm_Seguridad_Login_Load;
            // Configurar el authController
            authController = new Ctrl_Security_Auth();
            // Configurar Label de Versionamiento
            ConfigurarVersionamiento();
        }
        private void Frm_Seguridad_Login_Load(object sender, EventArgs e)
        {
            ConfigurarBotonesEnlace();
            ConfigurarBotonImagenSinFondo(Btn_Visible);
            ConfigurarOrdenTabulacion();
            // Configurar la apariencia de los Botones con padding real
            AplicarEstiloBoton(Btn_Next);

            // Cambiar el color de fondo del formulario para que combine mejor
            this.BackColor = Color.FromArgb(25, 22, 27);

            // Configurar los TextBox CON PADDING usando Panel
            ConfigurarTextBox();

            // Esperar un momento para que el formulario esté completamente inicializado
            this.BeginInvoke(new Action(() =>
            {
                // Cambiar el color de la barra de título al color personalizado rgb(40,36,44)
                SetTitleBarColor(Color.FromArgb(25, 22, 27), Color.White);
                TxtUser.Focus(); // Establecer foco al textbox de usuario
            }));

            AplicarBordesRedondeadosPanel(Panel_Contenedor, 15);
        }
        // Establecer orden de los Tabuladores
        private void ConfigurarOrdenTabulacion()
        {
            TxtUser.TabIndex = 0;
            TxtPassword.TabIndex = 1;
            Btn_Visible.TabIndex = 2;
            Btn_Next.TabIndex = 3;
            Btn_Unlock.TabIndex = 4;
            Btn_ForgetPassword.TabIndex = 5;
        }
        // Configurar estilo para botón "Next" con padding real usando Panel
        private void AplicarEstiloBoton(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(9, 184, 255); // Azul moderno
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            boton.Height = 45;
            boton.Width = Math.Max(boton.Width, 140);
            boton.Cursor = Cursors.Default;
            boton.TextAlign = ContentAlignment.MiddleCenter;

            // Esquinas redondeadas usando región personalizada
            boton.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, boton.Width, boton.Height, 20, 20));
        }
        // Importar función nativa para esquinas redondeadas
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);
        // Configura los TextBox con padding real usando Panel contenedor
        private void ConfigurarTextBox()
        {
            // Configurar cada TextBox
            CrearTextBoxConPadding(TxtUser);
            CrearTextBoxConPadding(TxtPassword);

            // Configuración específica para el password
            TxtPassword.UseSystemPasswordChar = true;
        }
        // Crea un TextBox con padding real usando un Panel contenedor
        private void CrearTextBoxConPadding(TextBox textBox)
        {
            // Guardar información original
            Point ubicacionOriginal = textBox.Location;
            Size tamañoOriginal = textBox.Size;
            Control contenedorPadre = textBox.Parent;
            string nombreOriginal = textBox.Name;
            string textoOriginal = textBox.Text;

            // Crear Panel contenedor que simula el TextBox con padding
            Panel panelContenedor = new Panel
            {
                Location = ubicacionOriginal,
                Size = new Size(tamañoOriginal.Width, Math.Max(tamañoOriginal.Height, 45)), // Mínimo 45px de altura
                BackColor = Color.FromArgb(60, 60, 60), // Mismo color que queremos para el TextBox
                BorderStyle = BorderStyle.None,
                Padding = new Padding(6, 6, 12, 8), // Aquí está el padding que necesitas
                Name = "Panel_" + nombreOriginal
            };

            // APLICAR ESQUINAS REDONDEADAS AL PANEL
            panelContenedor.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, panelContenedor.Width, panelContenedor.Height, 15, 15));

            // Configurar el TextBox para que se vea integrado
            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = Color.FromArgb(60, 60, 60); // Mismo color que el panel
            textBox.ForeColor = Color.White;
            textBox.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            textBox.Dock = DockStyle.Fill; // Llenar el panel respetando el padding
            textBox.Text = textoOriginal;
            textBox.TextAlign = HorizontalAlignment.Left; // Alineación horizontal

            // Remover el TextBox original de su contenedor
            contenedorPadre.Controls.Remove(textBox);

            // Agregar el TextBox al panel
            panelContenedor.Controls.Add(textBox);

            // Agregar el panel al contenedor original
            contenedorPadre.Controls.Add(panelContenedor);

            // Eventos para que el panel se comporte como un TextBox
            ConfigurarEventosPanel(panelContenedor, textBox);
        }
        // Configura los botones desde el Load del formulario
        private void ConfigurarBotonesEnlace()
        {
            // Configurar botón "He olvidado mi contraseña"
            ConfigurarBotonEnlace(Btn_ForgetPassword);

            // Configurar botón "Desbloqueo de usuario" 
            ConfigurarBotonEnlace(Btn_Unlock);
        }
        // Método para configurar un botón imagen sin fondo
        private void ConfigurarBotonImagenSinFondo(Button boton)
        {
            // Configuración básica del botón
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseDownBackColor = Color.FromArgb(25, 22, 27); // Mismo color del fondo
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 22, 27); // Mismo color del fondo
            boton.BackColor = Color.FromArgb(25, 22, 27); // Color del fondo del formulario
            boton.Cursor = Cursors.Default;

            // CLAVE: Eliminar TODOS los bordes posibles
            boton.FlatAppearance.BorderColor = Color.FromArgb(25, 22, 27);
            boton.FlatAppearance.CheckedBackColor = Color.FromArgb(25, 22, 27);
        }
        // Método para configurar un botón como enlace
        private void ConfigurarBotonEnlace(Button boton)
        {
            // Configuración básica del botón
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseDownBackColor = Color.FromArgb(25, 22, 27); // Mismo color del fondo
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 22, 27); // Mismo color del fondo
            boton.BackColor = Color.FromArgb(25, 22, 27); // Color del fondo del formulario
            boton.ForeColor = Color.FromArgb(9, 184, 255); // Color azul inicial
            boton.TextAlign = ContentAlignment.MiddleLeft; // Alineado a la izquierda
            boton.Cursor = Cursors.Default;

            // CLAVE: Eliminar TODOS los bordes posibles
            boton.FlatAppearance.BorderColor = Color.FromArgb(25, 22, 27);
            boton.FlatAppearance.CheckedBackColor = Color.FromArgb(25, 22, 27);
        }
        // Configura eventos para que el Panel se comporte como un TextBox
        private void ConfigurarEventosPanel(Panel panel, TextBox textBox)
        {
            // Cuando se hace clic en el panel, enfocar el TextBox
            panel.Click += (sender, e) => textBox.Focus();

            // Cambiar color cuando el TextBox obtiene/pierde el foco
            textBox.Enter += (sender, e) =>
            {
                panel.BackColor = Color.FromArgb(70, 70, 70); // Un poco más claro al enfocar
                textBox.BackColor = Color.FromArgb(70, 70, 70);

                // MANTENER LAS ESQUINAS REDONDEADAS al cambiar color
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, 15, 15));
            };

            textBox.Leave += (sender, e) =>
            {
                panel.BackColor = Color.FromArgb(60, 60, 60); // Color original
                textBox.BackColor = Color.FromArgb(60, 60, 60);

                // MANTENER LAS ESQUINAS REDONDEADAS al cambiar color
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, 15, 15));
            };

            // EVENTO ADICIONAL: Si el panel cambia de tamaño, actualizar la región
            panel.Resize += (sender, e) =>
            {
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, 15, 15));
            };
        }
        // Aplica bordes redondeados a un Panel específico
        public void AplicarBordesRedondeadosPanel(Panel panel, int radioEsquinas = 15)
        {
            try
            {
                if (panel == null) return;

                // Aplicar bordes redondeados usando la función CreateRoundRectRgn existente
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, radioEsquinas, radioEsquinas));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al aplicar bordes redondeados al panel {panel.Name}: {ex.Message}");
            }
        }
        // Aplica bordes redondeados a múltiples paneles
        public void AplicarBordesRedondeadosPaneles(Panel[] paneles, int radioEsquinas = 15)
        {
            if (paneles == null) return;

            foreach (Panel panel in paneles)
            {
                AplicarBordesRedondeadosPanel(panel, radioEsquinas);
            }
        }
        // Aplica bordes redondeados a un panel con evento de resize automático
        public void ConfigurarPanelRedondeado(Panel panel, int radioEsquinas = 15)
        {
            if (panel == null) return;

            try
            {
                // Aplicar bordes inicialmente
                AplicarBordesRedondeadosPanel(panel, radioEsquinas);

                // Configurar evento para mantener los bordes al redimensionar
                panel.Resize += (sender, e) => {
                    if (sender is Panel p)
                        AplicarBordesRedondeadosPanel(p, radioEsquinas);
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al configurar panel redondeado {panel.Name}: {ex.Message}");
            }
        }
        #endregion PropiedadesIniciales
        #region BarraDeTtuloPersonalizada
        // Código para personalizar la barra de título (colores, botones, etc.)
        // Importaciones de la API de Windows para personalizar la barra de título
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        // Constantes para los atributos de DWM
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;
        
        // Establece el color de la barra de título del formulario
        public void SetTitleBarColor(Color backgroundColor, Color textColor)
        {
            try
            {
                IntPtr hWnd = this.Handle;

                // Verificar que el handle sea válido
                if (hWnd == IntPtr.Zero)
                {
                    MessageBox.Show("Handle del formulario no válido. Intente después de que el formulario esté completamente cargado.",
                                  "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Convertir el color a formato COLORREF (0x00BBGGRR)
                int colorValue = ColorToColorRef(backgroundColor);
                int textColorValue = ColorToColorRef(textColor);

                // Intentar establecer el color de fondo de la barra de título
                int result1 = DwmSetWindowAttribute(hWnd, DWMWA_CAPTION_COLOR, ref colorValue, sizeof(int));

                // Intentar establecer el color del texto de la barra de título
                int result2 = DwmSetWindowAttribute(hWnd, DWMWA_TEXT_COLOR, ref textColorValue, sizeof(int));

                // Verificar si hubo errores
                if (result1 != 0 && result2 != 0)
                {
                    // Si ambos fallaron, mostrar información de depuración
                    var osInfo = $"OS: {Environment.OSVersion}\nBuild: {GetWindowsBuild()}";
                    MessageBox.Show($"No se pudo establecer el color de la barra de título.\n\n{osInfo}\n\nHRESULT: {result1:X8}",
                                  "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (result1 == 0 || result2 == 0)
                {
                    // Al menos uno funcionó
                    Console.WriteLine("Color de barra de título aplicado correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al establecer el color de la barra de título: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Convierte un Color de .NET a formato COLORREF de Windows
        private int ColorToColorRef(Color color)
        {
            return color.R | (color.G << 8) | (color.B << 16);
        }
        // Configura qué botones mostrar en la barra de título
        public void ConfigurarBotonesBarraTitulo(bool mostrarMinimizar, bool mostrarMaximizar, bool mostrarCerrar)
        {
            this.MinimizeBox = mostrarMinimizar;
            this.MaximizeBox = mostrarMaximizar;
            this.ControlBox = mostrarCerrar;
        }
        // Obtiene el número de build de Windows
        private string GetWindowsBuild()
        {
            try
            {
                var reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                return reg?.GetValue("CurrentBuild")?.ToString() ?? "Desconocido";
            }
            catch
            {
                return "No disponible";
            }
        }
        // Método para cambiar el color dinámicamente (puedes llamar este método desde botones u otros eventos)
        public void ChangeToPresetColor(string colorName)
        {
            Color bgColor, textColor;

            switch (colorName.ToLower())
            {
                case "azul":
                    bgColor = Color.FromArgb(0, 120, 215);  // Azul de Windows
                    textColor = Color.White;
                    break;
                case "verde":
                    bgColor = Color.FromArgb(16, 124, 16);  // Verde
                    textColor = Color.White;
                    break;
                case "rojo":
                    bgColor = Color.FromArgb(196, 43, 28);  // Rojo
                    textColor = Color.White;
                    break;
                case "morado":
                    bgColor = Color.FromArgb(135, 100, 184); // Morado
                    textColor = Color.White;
                    break;
                case "naranja":
                    bgColor = Color.FromArgb(255, 140, 0);   // Naranja
                    textColor = Color.Black;
                    break;
                case "negro":
                    bgColor = Color.FromArgb(32, 32, 32);    // Negro oscuro
                    textColor = Color.White;
                    break;
                case "default":
                default:
                    // Restaurar colores por defecto del sistema
                    bgColor = SystemColors.ActiveCaption;
                    textColor = SystemColors.ActiveCaptionText;
                    break;
            }
            SetTitleBarColor(bgColor, textColor);
        }
        // Método para establecer un color personalizado usando valores RGB desde base de datos
        public void SetCustomTitleBarColor(int r, int g, int b, bool textIsWhite = true)
        {
            Color backgroundColor = Color.FromArgb(r, g, b);
            Color textColor = textIsWhite ? Color.White : Color.Black;
            SetTitleBarColor(backgroundColor, textColor);
        }
        #endregion BarraDeTtuloPersonalizada
        #region Events
        private void Btn_ForgetPassword_MouseEnter(object sender, EventArgs e)
        {
            // Cambiar color de texto a rojo y agregar subrayado rojo
            Btn_ForgetPassword.ForeColor = Color.White;
            Btn_ForgetPassword.Font = new Font(Btn_ForgetPassword.Font, FontStyle.Underline);
        }
        private void Btn_ForgetPassword_MouseLeave(object sender, EventArgs e)
        {
            // Restaurar color original del texto (blanco) y quitar subrayado
            Btn_ForgetPassword.ForeColor = Color.FromArgb(9, 184, 255);
            Btn_ForgetPassword.Font = new Font(Btn_ForgetPassword.Font, FontStyle.Regular);
        }
        private void Btn_ForgetPassword_Click(object sender, EventArgs e)
        {
            // Restaurar el borde al color del fondo del formulario
            Btn_ForgetPassword.FlatAppearance.BorderColor = Color.FromArgb(25, 22, 27);
            Btn_ForgetPassword.FlatAppearance.BorderSize = 0;

            // Ocultar login
            this.Hide();

            // Abrir formulario de recuperación de contraseña
            var frmForgetPassword = new Frm_Security_ForgetPassword();
            var result = frmForgetPassword.ShowDialog();

            // Mostrar login nuevamente cuando se cierre el diálogo
            this.Show();
        }

        private void Btn_Unlock_Click(object sender, EventArgs e)
        {
            // Restaurar el borde al color del fondo del formulario
            Btn_Unlock.FlatAppearance.BorderColor = Color.FromArgb(25, 22, 27);
            Btn_Unlock.FlatAppearance.BorderSize = 0;

            // Ocultar login
            this.Hide();

            // Abrir formulario de desbloqueo de usuario
            var frmUnlockUser = new Frm_Security_UnlockUser();
            var result = frmUnlockUser.ShowDialog();

            // Mostrar login nuevamente cuando se cierre el diálogo
            this.Show();
        }
        private void Btn_Unlock_MouseEnter(object sender, EventArgs e)
        {
            // Cambiar color de texto a rojo y agregar subrayado rojo
            Btn_Unlock.ForeColor = Color.White;
            Btn_Unlock.Font = new Font(Btn_Unlock.Font, FontStyle.Underline);
        }
        private void Btn_Unlock_MouseLeave(object sender, EventArgs e)
        {
            // Restaurar color original del texto (blanco) y quitar subrayado
            Btn_Unlock.ForeColor = Color.FromArgb(9, 184, 255);
            Btn_Unlock.Font = new Font(Btn_Unlock.Font, FontStyle.Regular);
        }
        private void Btn_Visible_Click(object sender, EventArgs e)
        {
            if (TxtPassword.UseSystemPasswordChar == true)
            {
                TxtPassword.UseSystemPasswordChar = false;
                Btn_Visible.Image = Properties.Resources.VisibleBlack_25x25;
            }
            else
            {
                TxtPassword.UseSystemPasswordChar = true;
                Btn_Visible.Image = Properties.Resources.Visible_25x25;
            }
        }
        // Event handler para el botón de login
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            await LoginAsync();
        }
        // Event handler para Enter en los textboxes
        private async void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                await LoginAsync();
            }
        }
        // Reemplazo del método TxtUser_KeyPress para mejor lógica y robustez
        private void TxtUser_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo procesar si se presiona Enter y el control de contraseña está disponible
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (TxtPassword != null && TxtPassword.CanFocus)
                {
                    TxtPassword.Focus();
                }
            }
        }
        #endregion Events
        #region ProcedimientosLogin
        //Funciones de inicio de sesión y autenticación
        public async Task LoginAsync()
        {
            if (isLoading) return;

            try
            {
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(TxtUser.Text))
                {
                    MessageBox.Show("Por favor ingrese su usuario", "Campo requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TxtUser.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxtPassword.Text))
                {
                    MessageBox.Show("Por favor ingrese su contraseña", "Campo requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TxtPassword.Focus();
                    return;
                }

                // Deshabilitar controles durante la validación
                EnableControls(false);
                isLoading = true;

                // Validar usuario y contraseña
                var loginResult = await authController.ValidateUserAsync(
                    TxtUser.Text.Trim(),
                    TxtPassword.Text
                );

                if (loginResult.IsSuccess)
                {
                    // Verificar si necesita cambiar contraseña
                    if (loginResult.RequiresPasswordChange())
                    {
                        await HandlePasswordChangeRequired(loginResult.UserData);
                    }
                    else
                    {
                        // Login exitoso, ir al menú principal
                        await OpenMainMenuAsync(loginResult.UserData);
                    }
                }
                else
                {
                    // Manejar error de login
                    await HandleLoginError(loginResult);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado en el sistema. Contacte al administrador.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                System.Diagnostics.Debug.WriteLine($"Error en LoginAsync: {ex.Message}");
            }
            finally
            {
                EnableControls(true);
                isLoading = false;
            }
        }
        // Maneja cuando se requiere cambio de contraseña
        private async Task HandlePasswordChangeRequired(Mdl_Security_UserInfo userData)
        {
            try
            {
                var frm_Security_ResetPassword = new Frm_Security_ResetPassword();
                frm_Security_ResetPassword.UsuarioRestablecerPassword = userData.Username;
                frm_Security_ResetPassword.UserId = userData.UserId;
                frm_Security_ResetPassword.UserData = userData;

                this.Hide();
                var dialogResult = frm_Security_ResetPassword.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    // Si cambió la contraseña exitosamente, proceder al menú principal
                    userData.IsTemporaryPassword = false; // Actualizar el estado
                    await OpenMainMenuAsync(userData);
                }
                else
                {
                    // Si canceló o falló el cambio, volver al login
                    Clear();
                    this.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar cambio de contraseña", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error en HandlePasswordChangeRequired: {ex.Message}");

                Clear();
                this.Show();
            }
        }
        // Maneja los errores de login
        private async Task HandleLoginError(Mdl_Security_UserLoginResult loginResult)
        {
            // Mostrar mensaje de error
            MessageBox.Show(loginResult.Message, "Error de inicio de sesión",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Si es error crítico (usuario bloqueado), reiniciar aplicación
            if (loginResult.IsCriticalError())
            {
                await Task.Delay(2000); // Dar tiempo para que el usuario lea el mensaje
                ReiniciarAplicacion();
                return;
            }

            // Para otros errores, limpiar y permitir reintentar
            Clear();
        }
        // Abre el menú principal después de login exitoso
        private async Task OpenMainMenuAsync(Mdl_Security_UserInfo userData)
        {
            try
            {
                var frm_ControlCenter_MDI = new Frm_ControlCenter_MDI();
                frm_ControlCenter_MDI.Usuario = userData.Username;
                frm_ControlCenter_MDI.UserData = userData; // Pasar toda la información del usuario

                // Registrar procedimiento de conexión
                RegistrarProcedimientoAudith(userData);

                this.Hide();
                frm_ControlCenter_MDI.Show();

                // Esto evita la advertencia
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el menú principal", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error en OpenMainMenuAsync: {ex.Message}");
            }
        }
        // Habilita o deshabilita los controles del formulario
        private void EnableControls(bool enabled)
        {
            TxtUser.Enabled = enabled;
            TxtPassword.Enabled = enabled;

            if (Btn_Next != null)
                Btn_Next.Enabled = enabled;

            // Cambiar cursor
            this.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
        }
        // Limpia los controles del formulario
        private void Clear()
        {
            TxtPassword.Clear();
            TxtPassword.Focus();
            // No limpiar el usuario para facilitar reintentos
        }
        // Registra el procedimiento de login (auditoría adicional)
        private void RegistrarProcedimientoAudith(Mdl_Security_UserInfo userData)
        {
            try
            {
                // Implementar según tu sistema de auditoría específico
                // Por ejemplo, registrar en tabla de sesiones activas
                System.Diagnostics.Debug.WriteLine($"Usuario {userData.Username} se conectó exitosamente");
            }
            catch (Exception ex)
            {
                // No mostrar error al usuario, solo log
                System.Diagnostics.Debug.WriteLine($"Error en RegistrarProcedimiento: {ex.Message}");
            }
        }
        // Reinicia la aplicación
        private void ReiniciarAplicacion()
        {
            try
            {
                Application.Restart();
            }
            catch
            {
                Application.Exit();
            }
        }
        #endregion ProcedimientosLogin
        #region Versionamiento
        private void ConfigurarVersionamiento()
        {
            try
            {
                // Obtener la version del ensamblado (Assembly)
                string version = ObtenerVersionAplicacion();

                // Verificar si existe la Label antes de actualizar
                if (Lbl_Versionamiento != null)
                {
                    Lbl_Versionamiento.Text = version;
                }
                else
                {
                    // Si no existe la label, crear mensaje de debug
                    System.Diagnostics.Debug.WriteLine($"Label Lbl_Versionamiento no encontrada. Version: {version}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al configurar versionamiento: {ex.Message}");
            }
        }
        // Obtiene la version completa de la aplicacion desde el ensamblado
        // Formato: v1.0.0.0 o la version configurada en AssemblyInfo
        private string ObtenerVersionAplicacion()
        {
            try
            {
                // Obtener version desde el Assembly actual
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                // Formato completo: v1.0.0.0
                return $"VERSIÓN: {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

                // ALTERNATIVAS DE FORMATO:
                // Solo Major.Minor: return $"v{version.Major}.{version.Minor}";
                // Mayor.Minor.Build: return $"v{version.Major}.{version.Minor}.{version.Build}";
                // Con fecha: return $"v{version.Major}.{version.Minor}.{version.Build} - {DateTime.Now.Year}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener version: {ex.Message}");
                return "v1.0.0.0"; // Version por defecto en caso de error
            }
        }
        #endregion Versionamiento
    }
}