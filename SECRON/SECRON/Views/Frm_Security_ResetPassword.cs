using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Security_ResetPassword : Form
    {
        #region PropiedadesIniciales
        // Propiedades para recibir datos desde Frm_Users_Managment
        public string UsuarioRestablecerPassword { get; set; }
        public int? UserId { get; set; }
        public Mdl_Security_UserInfo UserData { get; set; } // Usuario administrador que hace el cambio

        private bool isLoading = false;

        // Importaciones de la API de Windows para personalizar la barra de título
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        // Constantes para los atributos de DWM
        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;
        

        public Frm_Security_ResetPassword()
        {
            InitializeComponent();
            ConfigurarBarraTitulo();
            this.Load += Frm_Security_ResetPassword_Load;
        }
        private void Frm_Security_ResetPassword_Load(object sender, EventArgs e)
        {
            ConfigurarOrdenTabulacion();
            AplicarEstiloBoton(Btn_OK);
            this.BackColor = Color.FromArgb(25, 22, 27);
            ConfigurarTextBox();

            this.BeginInvoke(new Action(() =>
            {
                SetTitleBarColor(Color.FromArgb(25, 22, 27), Color.White);

                // Mostrar el nombre de usuario en el TextBox (solo lectura)
                if (!string.IsNullOrEmpty(UsuarioRestablecerPassword))
                {
                    Txt_User.Text = UsuarioRestablecerPassword;
                    Txt_User.ReadOnly = true;
                }

                Txt_Password.Focus();
            }));

            AplicarBordesRedondeadosPanel(Panel_Contenedor, 15);
        }
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
            Txt_User.TabIndex = 0;
            Txt_Password.TabIndex = 1;
            Btn_OK.TabIndex = 2;
        }
        #endregion PropiedadesIniciales
        #region EstilosYFormato
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
            CrearTextBoxConPadding(Txt_User);
            CrearTextBoxConPadding(Txt_Password);
            CrearTextBoxConPadding(Txt_NewPassword1);
            CrearTextBoxConPadding(Txt_NewPassword2);
            Txt_Password.UseSystemPasswordChar = true; // Mostrar la contraseña temporal
            Txt_NewPassword1.UseSystemPasswordChar = true;
            Txt_NewPassword2.UseSystemPasswordChar = true;
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
        #endregion EstilosYFormato
        #region EventosYValidacion
        private async void Btn_OK_Click(object sender, EventArgs e)
        {
            await RestablecerPasswordAsync();
        }
        private void Btn_Visible_Click(object sender, EventArgs e)
        {
            if (Txt_Password.UseSystemPasswordChar == true)
            {
                Txt_Password.UseSystemPasswordChar = false;
                Btn_Visible.Image = Properties.Resources.VisibleBlack_25x25;

                //Validamos todos los demás también
                Txt_NewPassword1.UseSystemPasswordChar = false;
                Txt_NewPassword2.UseSystemPasswordChar = false;
            }
            else
            {
                Txt_Password.UseSystemPasswordChar = true;
                Btn_Visible.Image = Properties.Resources.Visible_25x25;

                //Validamos todos los demás también
                Txt_NewPassword1.UseSystemPasswordChar = true;
                Txt_NewPassword2.UseSystemPasswordChar = true;
            }
        }
        private async Task RestablecerPasswordAsync()
        {
            if (isLoading) return;

            try
            {
                // ⭐ VALIDAR CONTRASEÑA TEMPORAL ACTUAL
                if (string.IsNullOrWhiteSpace(Txt_Password.Text))
                {
                    MessageBox.Show("Por favor ingrese su contraseña temporal actual", "Campo requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_Password.Focus();
                    return;
                }

                // ⭐ VALIDAR NUEVA CONTRASEÑA
                if (string.IsNullOrWhiteSpace(Txt_NewPassword1.Text))
                {
                    MessageBox.Show("Por favor ingrese su nueva contraseña", "Campo requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_NewPassword1.Focus();
                    return;
                }

                if (Txt_NewPassword1.Text.Length < 6)
                {
                    MessageBox.Show("La nueva contraseña debe tener al menos 6 caracteres", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_NewPassword1.Focus();
                    return;
                }

                // ⭐ VALIDAR CONFIRMACIÓN DE CONTRASEÑA
                if (string.IsNullOrWhiteSpace(Txt_NewPassword2.Text))
                {
                    MessageBox.Show("Por favor confirme su nueva contraseña", "Campo requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_NewPassword2.Focus();
                    return;
                }

                if (Txt_NewPassword1.Text != Txt_NewPassword2.Text)
                {
                    MessageBox.Show("Las contraseñas no coinciden", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_NewPassword2.Clear();
                    Txt_NewPassword2.Focus();
                    return;
                }

                if (!UserId.HasValue)
                {
                    MessageBox.Show("No se pudo identificar al usuario", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                EnableControls(false);
                isLoading = true;

                // ⭐ PASO 1: VALIDAR que la contraseña temporal actual sea correcta
                var usuario = Ctrl_Users.ValidarLogin(UsuarioRestablecerPassword, Txt_Password.Text);

                if (usuario == null)
                {
                    MessageBox.Show("La contraseña temporal actual es incorrecta", "Error de validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_Password.Clear();
                    Txt_Password.Focus();
                    EnableControls(true);
                    isLoading = false;
                    return;
                }

                // ⭐ PASO 2: Cambiar a la nueva contraseña (NO temporal)
                int resultado = Ctrl_Users.CambiarPassword(
                    UserId.Value,
                    Txt_NewPassword1.Text,
                    false // ⭐ Ya NO es temporal
                );

                if (resultado > 0)
                {
                    MessageBox.Show(
                        "¡Contraseña cambiada exitosamente!\n\n" +
                        "Ahora puede iniciar sesión con su nueva contraseña.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo cambiar la contraseña. Intente nuevamente.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    EnableControls(true);
                    isLoading = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al restablecer contraseña: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableControls(true);
                isLoading = false;
            }
        }
        private void EnableControls(bool enabled)
        {
            Txt_Password.Enabled = enabled;
            Btn_OK.Enabled = enabled;
            this.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
        }
        #endregion EventosYValidacion
        #region AsignacionFocus
        private void ConfigurarTabIndexYFocus()
        {
            Txt_User.TabIndex = 0;
            Txt_Password.TabIndex = 1;
            Txt_NewPassword1.TabIndex = 2;
            Txt_NewPassword2.TabIndex = 3;
            Btn_OK.TabIndex = 4;
            Btn_Visible.TabIndex = 5;

            Txt_User.Focus();
        }
        #endregion AsignacionFocus
    }
}
