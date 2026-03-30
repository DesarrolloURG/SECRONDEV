using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Home : Form
    {
        #region PropiedadesIniciales

        // Nombre del recurso de imagen usado como fondo/logo
        // Si cambias el nombre del recurso en Properties/Resources,
        // solo debes cambiar este string.
        private readonly string _nombreRecursoLogo = "LogotipoHome2026";

        public Frm_Home()
        {
            InitializeComponent();
            // Configurar la imagen al cargar el formulario
            ConfigurarImagenCentrada();
        }

        #endregion PropiedadesIniciales
        #region EventosFormulario

        // Evento para refrescar/redibujar al redimensionar el formulario
        // (Zoom + Dock Fill se encarga de ajustar proporciones).
        private void Frm_Home_Resize(object sender, EventArgs e)
        {
            if (PicBox1 != null)
            {
                // Forzamos el repintado por si acaso
                PicBox1.Invalidate();
            }
        }

        #endregion EventosFormulario
        #region ConfigurarImagen

        // Método para configurar la imagen ocupando todo el formulario
        // manteniendo la proporción (responsivo).
        private void ConfigurarImagenCentrada()
        {
            try
            {
                // El PictureBox ocupa siempre todo el área del formulario
                PicBox1.Dock = DockStyle.Fill;

                // Intentar obtener la imagen desde Resources usando el nombre como variable
                object recurso = Properties.Resources.ResourceManager.GetObject(_nombreRecursoLogo);

                if (recurso is Image imagenLogo)
                {
                    PicBox1.Image = imagenLogo;
                }
                else
                {
                    // Si no encontró el recurso, crear una imagen por defecto
                    CrearImagenPorDefecto();
                }

                // Muy importante:
                // Zoom mantiene la proporción de la imagen (no se deforma),
                // y la ajusta lo más posible al tamaño del PictureBox.
                // Puede dejar "franjas" si la proporción del formulario
                // no coincide con la de la imagen, pero nunca la deforma.
                PicBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error cargando recurso '{_nombreRecursoLogo}': {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                // En caso de error, usar imagen por defecto
                CrearImagenPorDefecto();
            }
        }

        // Método para crear una imagen por defecto si no se encuentra la principal
        private void CrearImagenPorDefecto()
        {
            // Usamos el tamaño actual del formulario para que también sea responsivo
            int ancho = this.ClientSize.Width > 0 ? this.ClientSize.Width : 1200;
            int alto = this.ClientSize.Height > 0 ? this.ClientSize.Height : 900;

            Bitmap imagen = new Bitmap(ancho, alto);
            using (Graphics g = Graphics.FromImage(imagen))
            {
                // Fondo
                g.FillRectangle(Brushes.LightBlue, 0, 0, ancho, alto);

                // Texto
                string texto = "SECRON";
                using (Font fuente = new Font("Arial", 48, FontStyle.Bold))
                {
                    SizeF tamañoTexto = g.MeasureString(texto, fuente);
                    float x = (ancho - tamañoTexto.Width) / 2;
                    float y = (alto - tamañoTexto.Height) / 2;

                    g.DrawString(texto, fuente, Brushes.DarkBlue, x, y);
                }
            }

            // Configurar el PictureBox para mostrar la imagen por defecto
            PicBox1.Dock = DockStyle.Fill;
            PicBox1.SizeMode = PictureBoxSizeMode.Zoom;
            PicBox1.Image = imagen;
        }

        #endregion ConfigurarImagen
    }
}
