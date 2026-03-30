using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Security_ForgetPassword : Form
    {
        #region PropiedadesIniciales
        private bool isLoading = false;

        // Importaciones de la API de Windows para personalizar la barra de título
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        // Constantes para los atributos de DWM
        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;
        // Métodos necesarios para configuraciones iniciales
        private void ConfigurarBarraTitulo()
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void ConfigurarOrdenTabulacion()
        {
            TxtUser.TabIndex = 0;
            Btn_OK.TabIndex = 1;
        }
        public Frm_Security_ForgetPassword()
        {
            InitializeComponent();
            ConfigurarBarraTitulo();
            this.Load += Frm_Security_ForgetPassword_Load;
        }
        private void Frm_Security_ForgetPassword_Load(object sender, EventArgs e)
        {
            ConfigurarOrdenTabulacion();
            AplicarEstiloBoton(Btn_OK);
            this.BackColor = Color.FromArgb(25, 22, 27);
            ConfigurarTextBox();

            this.BeginInvoke(new Action(() =>
            {
                SetTitleBarColor(Color.FromArgb(25, 22, 27), Color.White);

                TxtUser.Focus();
            }));

            AplicarBordesRedondeadosPanel(Panel_Contenedor, 15);
        }
        #endregion PropiedadesIniciales
        #region ConfiguracionEstilos
        private void AplicarEstiloBoton(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(9, 184, 255);
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            boton.Height = 45;
            boton.Width = Math.Max(boton.Width, 140);
            boton.Cursor = Cursors.Hand;
            boton.TextAlign = ContentAlignment.MiddleCenter;

            boton.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, boton.Width, boton.Height, 20, 20));
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        private void ConfigurarTextBox()
        {
            CrearTextBoxConPadding(TxtUser);
        }

        private void CrearTextBoxConPadding(TextBox textBox)
        {
            Point ubicacionOriginal = textBox.Location;
            Size tamañoOriginal = textBox.Size;
            Control contenedorPadre = textBox.Parent;
            string nombreOriginal = textBox.Name;
            string textoOriginal = textBox.Text;

            Panel panelContenedor = new Panel
            {
                Location = ubicacionOriginal,
                Size = new Size(tamañoOriginal.Width, Math.Max(tamañoOriginal.Height, 45)),
                BackColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.None,
                Padding = new Padding(6, 6, 12, 8),
                Name = "Panel_" + nombreOriginal
            };

            panelContenedor.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, panelContenedor.Width, panelContenedor.Height, 15, 15));

            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = Color.FromArgb(60, 60, 60);
            textBox.ForeColor = Color.White;
            textBox.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            textBox.Dock = DockStyle.Fill;
            textBox.Text = textoOriginal;
            textBox.TextAlign = HorizontalAlignment.Left;

            contenedorPadre.Controls.Remove(textBox);
            panelContenedor.Controls.Add(textBox);
            contenedorPadre.Controls.Add(panelContenedor);

            ConfigurarEventosPanel(panelContenedor, textBox);
        }

        private void ConfigurarEventosPanel(Panel panel, TextBox textBox)
        {
            panel.Click += (sender, e) => textBox.Focus();

            textBox.Enter += (sender, e) =>
            {
                panel.BackColor = Color.FromArgb(70, 70, 70);
                textBox.BackColor = Color.FromArgb(70, 70, 70);
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, 15, 15));
            };

            textBox.Leave += (sender, e) =>
            {
                panel.BackColor = Color.FromArgb(60, 60, 60);
                textBox.BackColor = Color.FromArgb(60, 60, 60);
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, 15, 15));
            };

            panel.Resize += (sender, e) =>
            {
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, 15, 15));
            };
        }

        public void AplicarBordesRedondeadosPanel(Panel panel, int radioEsquinas = 15)
        {
            try
            {
                if (panel == null) return;
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, radioEsquinas, radioEsquinas));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al aplicar bordes redondeados al panel {panel.Name}: {ex.Message}");
            }
        }

        public void SetTitleBarColor(Color backgroundColor, Color textColor)
        {
            try
            {
                IntPtr hWnd = this.Handle;
                if (hWnd == IntPtr.Zero) return;

                int colorValue = ColorToColorRef(backgroundColor);
                int textColorValue = ColorToColorRef(textColor);

                DwmSetWindowAttribute(hWnd, DWMWA_CAPTION_COLOR, ref colorValue, sizeof(int));
                DwmSetWindowAttribute(hWnd, DWMWA_TEXT_COLOR, ref textColorValue, sizeof(int));
            }
            catch { }
        }

        private int ColorToColorRef(Color color)
        {
            return color.R | (color.G << 8) | (color.B << 16);
        }
        #endregion ConfiguracionEstilos
        #region CorreoSolicitud
        private void Btn_OK_Click(object sender, EventArgs e)
        {
            if (isLoading) return;

            try
            {
                // Obtener el usuario ingresado
                string usuario = TxtUser.Text.Trim();

                // Validar que se ingresó un username
                if (string.IsNullOrWhiteSpace(usuario))
                {
                    MessageBox.Show("Por favor ingrese su nombre de usuario", "Campo requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TxtUser.Focus();
                    return;
                }

                isLoading = true;
                EnableControls(false);

                // Validar si el usuario existe
                var usuarioData = Ctrl_Users.ObtenerUsuarioPorUsername(usuario);

                if (usuarioData == null)
                {
                    MessageBox.Show("Usuario no encontrado en el sistema", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TxtUser.Clear();
                    TxtUser.Focus();
                    EnableControls(true);
                    isLoading = false;
                    return;
                }

                // Validar que tenga correo institucional
                if (string.IsNullOrWhiteSpace(usuarioData.InstitutionalEmail))
                {
                    MessageBox.Show(
                        "Este usuario no tiene un correo institucional registrado.\n\n" +
                        "Por favor contacte al administrador del sistema.",
                        "Sin correo registrado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    EnableControls(true);
                    isLoading = false;
                    return;
                }

                // Generar número de gestión aleatorio
                Random random = new Random();
                int noGestion = random.Next(10000, 100000);

                // Configuración del correo emisor
                string correoEmisor = "notificaciones@uregionalregion2.edu.gt";
                string contraseñaEmisor = "F0rza01.";

                // ========== CORREO 1: PARA EL EQUIPO DE SOPORTE ==========
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(correoEmisor, contraseñaEmisor),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(correoEmisor, "Soporte Institucional"),
                    Subject = "Solicitud de Restablecimiento de Contraseña - SECRON",
                    IsBodyHtml = true,
                };

                mail.Body = $@"
        <html>
        <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2 style='color: #2A7AE2;'>Solicitud de Restablecimiento de Contraseña</h2>
            <p><strong>No. de Gestión: {noGestion}</strong></p>
            <p>Estimado equipo de soporte,</p>
            <p>Se ha recibido una solicitud para restablecer la contraseña con los siguientes datos:</p>
            <table style='border-collapse: collapse; width: 100%;'>
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'><strong>Usuario:</strong></td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{usuarioData.Username}</td>
                </tr>
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'><strong>Nombre Completo:</strong></td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{usuarioData.FullName}</td>
                </tr>
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'><strong>Correo Institucional:</strong></td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{usuarioData.InstitutionalEmail}</td>
                </tr>
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'><strong>Fecha de Solicitud:</strong></td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{DateTime.Now:dd/MM/yyyy HH:mm:ss}</td>
                </tr>
            </table>
            <p>Por favor, procedan a verificar y atender la solicitud lo antes posible.</p>
            <p style='color: #555;'>Gracias,</p>
            <p><strong>Servicio automático de notificaciones, SECRON</strong></p>
        </body>
        </html>";

                mail.To.Add("desarrollo@uregionalregion2.edu.gt");
                smtpClient.Send(mail);

                // ========== CORREO 2: PARA EL USUARIO SOLICITANTE ==========
                SmtpClient smtpClient2 = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(correoEmisor, contraseñaEmisor),
                    EnableSsl = true
                };

                MailMessage mail2 = new MailMessage
                {
                    From = new MailAddress(correoEmisor, "Soporte Institucional"),
                    Subject = "Solicitud de Restablecimiento de Contraseña - SECRON",
                    IsBodyHtml = true,
                };

                mail2.Body = $@"
        <html>
        <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2 style='color: #2A7AE2;'>Has Generado una Solicitud de Restablecimiento de Contraseña</h2>
            <p><strong>No. de Gestión: {noGestion}</strong></p>
            <p>Estimado/a {usuarioData.FullName},</p>
            <p>Se ha recibido tu solicitud para restablecer la contraseña con los siguientes datos:</p>
            <table style='border-collapse: collapse; width: 100%;'>
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'><strong>Usuario:</strong></td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{usuarioData.Username}</td>
                </tr>
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'><strong>Correo Institucional:</strong></td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{usuarioData.InstitutionalEmail}</td>
                </tr>
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'><strong>Fecha de Solicitud:</strong></td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{DateTime.Now:dd/MM/yyyy HH:mm:ss}</td>
                </tr>
            </table>
            <p>Nos comunicaremos contigo lo antes posible para dar continuidad a tu gestión.</p>
            <p style='color: #555;'>Atentamente, Equipo de Soporte TI</p>
            <p><strong>Servicio automático de notificaciones, SECRON</strong></p>
        </body>
        </html>";

                mail2.To.Add(usuarioData.InstitutionalEmail);
                smtpClient2.Send(mail2);

                MessageBox.Show("Solicitud enviada con éxito.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al enviar la solicitud: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableControls(true);
                isLoading = false;
            }
        }
        // Habilitar o deshabilitar controles durante la operación
        private void EnableControls(bool enabled)
        {
            TxtUser.Enabled = enabled;
            Btn_OK.Enabled = enabled;
            this.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
        }
        #endregion CorreoSolicitud
    }
}
