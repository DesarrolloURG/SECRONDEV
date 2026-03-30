using System;
using System.Drawing;
using System.Windows.Forms;

namespace SECRON.Configuration
{
    public static class TabConfig
    {
        #region ColoresSECRON
        private static readonly Color COLOR_TAB_ACTIVO = Color.FromArgb(238, 143, 109); // Naranja SECRON
        private static readonly Color COLOR_TAB_INACTIVO = Color.FromArgb(245, 245, 245); // Gris claro
        private static readonly Color COLOR_TEXTO = Color.Black;
        private static readonly Color COLOR_HOVER_CLOSE = Color.FromArgb(220, 53, 69); // Rojo hover
        private static readonly Color COLOR_CLOSE_DEFAULT = Color.FromArgb(120, 120, 120); // Gris
        #endregion
        // Crear TabControl con pestañas personalizadas
        public static TabControl CrearTabControl(Form formulario, string[] nombresPestanas,int anchoTab = 350,int altoTab = 50, bool permitirCerrar = false)
        {
            // Crear TabControl
            TabControl tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.Normal,
                SizeMode = TabSizeMode.Fixed,
                ItemSize = new Size(anchoTab, altoTab),
                DrawMode = TabDrawMode.OwnerDrawFixed,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };

            // Eventos de dibujo personalizado
            tabControl.DrawItem += (sender, e) => DibujarTab(sender, e, permitirCerrar);

            if (permitirCerrar)
            {
                tabControl.MouseClick += CerrarTabClick;
            }

            // Crear las pestañas
            foreach (string nombre in nombresPestanas)
            {
                TabPage tabPage = new TabPage(nombre);
                Panel panel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Padding = new Padding(20)
                };
                tabPage.Controls.Add(panel);
                tabControl.TabPages.Add(tabPage);
            }

            // Agregar al formulario
            formulario.Controls.Add(tabControl);
            tabControl.BringToFront();

            return tabControl;
        }
        // Obtener el panel de una pestaña específica
        public static Panel ObtenerPanel(TabControl tabControl, int indicePestana)
        {
            if (indicePestana < 0 || indicePestana >= tabControl.TabPages.Count)
                return null;

            TabPage tabPage = tabControl.TabPages[indicePestana];
            return tabPage.Controls[0] as Panel;
        }
        // Agrega controles a una pestaña específica
        public static void AgregarControlesAPestana(TabControl tabControl, int indicePestana, params Control[] controles)
        {
            Panel panel = ObtenerPanel(tabControl, indicePestana);
            if (panel != null)
            {
                panel.Controls.AddRange(controles);
            }
        }
        #region MetodosPrivados
        // Métodos para dibujar pestañas y manejar cierre
        private static void DibujarTab(object sender, DrawItemEventArgs e, bool mostrarBotonCerrar)
        {
            TabControl tc = sender as TabControl;
            if (tc == null) return;

            TabPage tabPage = tc.TabPages[e.Index];
            Rectangle tabRect = tc.GetTabRect(e.Index);
            bool isSelected = (e.Index == tc.SelectedIndex);

            // Colores según estado
            Color backColor = isSelected ? COLOR_TAB_ACTIVO : COLOR_TAB_INACTIVO;
            Color textColor = COLOR_TEXTO;

            // Dibujar fondo
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, tabRect);
            }

            // Área de texto
            int anchoDisponible = mostrarBotonCerrar ? tabRect.Width - 30 : tabRect.Width - 8;
            Rectangle textRect = new Rectangle(
                tabRect.X + 8,
                tabRect.Y + 4,
                anchoDisponible,
                tabRect.Height - 8);

            // Dibujar texto
            using (Font customFont = new Font("Segoe UI", 11F, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };

                e.Graphics.DrawString(tabPage.Text, customFont, textBrush, textRect, sf);
            }

            // Dibujar botón cerrar si está habilitado
            if (mostrarBotonCerrar)
            {
                DibujarBotonCerrar(e.Graphics, tabRect, e.Index);
            }
        }
        // Dibujar el botón de cerrar en la pestaña
        private static void DibujarBotonCerrar(Graphics g, Rectangle tabRect, int tabIndex)
        {
            Rectangle closeRect = new Rectangle(
                tabRect.Right - 20,
                tabRect.Y + (tabRect.Height - 14) / 2,
                14, 14);

            Color closeColor = COLOR_CLOSE_DEFAULT;

            using (Pen closePen = new Pen(closeColor, 2))
            {
                g.DrawLine(closePen, closeRect.X + 3, closeRect.Y + 3,
                          closeRect.Right - 3, closeRect.Bottom - 3);
                g.DrawLine(closePen, closeRect.Right - 3, closeRect.Y + 3,
                          closeRect.X + 3, closeRect.Bottom - 3);
            }
        }
        // Manejar el clic en el botón de cerrar
        private static void CerrarTabClick(object sender, MouseEventArgs e)
        {
            TabControl tc = sender as TabControl;
            if (tc == null) return;

            for (int i = 0; i < tc.TabPages.Count; i++)
            {
                Rectangle tabRect = tc.GetTabRect(i);
                Rectangle closeRect = new Rectangle(
                    tabRect.Right - 20,
                    tabRect.Y + (tabRect.Height - 14) / 2,
                    14, 14);

                if (closeRect.Contains(e.Location))
                {
                    var result = MessageBox.Show(
                        "¿Desea cerrar esta pestaña?",
                        "Confirmar Cierre",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        tc.TabPages.RemoveAt(i);
                    }
                    break;
                }
            }
        }
        #endregion
    }
}