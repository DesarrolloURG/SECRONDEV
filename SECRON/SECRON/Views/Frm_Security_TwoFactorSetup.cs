using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Security_TwoFactorSetup : Form
    {
        #region PropiedadesIniciales
        // Propiedades para recibir datos desde Frm_Security_Login
        public string Username { get; set; }
        public int UserId { get; set; }

        private readonly Ctrl_Security_Auth authController;
        private string secretoGenerado;
        private bool isLoading = false;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;

        public Frm_Security_TwoFactorSetup()
        {
            InitializeComponent();
            authController = new Ctrl_Security_Auth();
            ConfigurarBarraTitulo();
            this.Load += Frm_Security_TwoFactorSetup_Load;
        }

        private void Frm_Security_TwoFactorSetup_Load(object sender, EventArgs e)
        {
            AplicarEstiloBoton(Btn_Confirmar);
            this.BackColor = Color.FromArgb(25, 22, 27);
            ConfigurarTextBox();
            GenerarYMostrarQR();

            this.BeginInvoke(new Action(() =>
            {
                SetTitleBarColor(Color.FromArgb(25, 22, 27), Color.White);
                Txt_Codigo.Focus();
            }));

            AplicarBordesRedondeadosPanel(Panel_Contenedor, 15);
        }

        private void ConfigurarBarraTitulo()
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
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
            CrearTextBoxConPadding(Txt_Codigo);
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
            textBox.Dock = DockStyle.Fill;
            textBox.Text = textoOriginal;

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
            catch { }
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
        #region LogicaDobleFactor
        private void GenerarYMostrarQR()
        {
            try
            {
                secretoGenerado = authController.GenerateTwoFactorSecret();
                byte[] qrBytes = authController.GenerateTwoFactorQrCode(secretoGenerado, Username);

                using (var ms = new MemoryStream(qrBytes))
                {
                    PicQR.Image = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el código QR: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void Btn_Confirmar_Click(object sender, EventArgs e)
        {
            if (isLoading) return;

            if (string.IsNullOrWhiteSpace(Txt_Codigo.Text) || Txt_Codigo.Text.Trim().Length != 6)
            {
                MessageBox.Show("Ingrese el código de 6 dígitos que muestra su aplicación autenticadora.",
                    "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Codigo.Focus();
                return;
            }

            if (!authController.VerifyTwoFactorCode(secretoGenerado, Txt_Codigo.Text))
            {
                MessageBox.Show("El código ingresado es incorrecto. Verifique la hora de su dispositivo e intente nuevamente.",
                    "Código inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Codigo.Clear();
                Txt_Codigo.Focus();
                return;
            }

            try
            {
                isLoading = true;
                Btn_Confirmar.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                bool guardado = await authController.ConfirmTwoFactorSetupAsync(UserId, secretoGenerado);

                if (guardado)
                {
                    MessageBox.Show("Autenticación en dos pasos vinculada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo guardar la vinculación. Intente nuevamente.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                isLoading = false;
                Btn_Confirmar.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }
        #endregion LogicaDobleFactor
    }
}