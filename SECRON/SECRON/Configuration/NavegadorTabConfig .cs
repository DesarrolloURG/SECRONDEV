using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Configuration
{
    // Clase para manejar información de cada pestaña
    public class TabInfo
    {
        public string Title { get; set; }
        public Form FormInstance { get; set; }
        public string FormKey { get; set; } // Identificador único del formulario
        public bool HasUnsavedChanges { get; set; } = false;
    }

    // Control personalizado para las pestañas - NavegadorTabConfig
    public partial class NavegadorTabConfig : UserControl
    {
        #region Constantes de Configuración
        private const int MAX_TABS = 10;
        private const int TAB_HEIGHT = 35;
        private const int TAB_MIN_WIDTH = 80; // Reducido para permitir más pestañas
        private const int TAB_MAX_WIDTH = 200;
        private const int CLOSE_BUTTON_SIZE = 16;
        private const int TAB_MARGIN = 1; // Reducido para aprovechar mejor el espacio
        #endregion

        #region Variables Privadas
        private List<TabInfo> tabs = new List<TabInfo>();
        private int activeTabIndex = -1;
        private Panel panelContenedor;
        private Rectangle closeButtonHoverRect = Rectangle.Empty;
        private int closeButtonHoverTab = -1;
        #endregion

        #region Eventos Públicos
        public event EventHandler<TabInfo> TabClosed;
        public event EventHandler<TabInfo> TabActivated;
        #endregion

        #region Constructor
        public NavegadorTabConfig()
        {
            InitializeComponent();
            ConfigurarEstiloControl();
            ConfigurarAtajosTeclado();
        }

        private void ConfigurarEstiloControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.Height = TAB_HEIGHT;
            this.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void ConfigurarAtajosTeclado()
        {
            // Hacer el control capaz de recibir eventos de teclado
            this.TabStop = true;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "NavegadorTabConfig";
            this.ResumeLayout(false);
        }
        #endregion

        #region Métodos Públicos de Configuración
        public void SetPanelContenedor(Panel panel)
        {
            this.panelContenedor = panel;
        }

        public bool AbrirFormHija(Form formHija, string title, string formKey)
        {
            // Verificar si ya está abierto
            var existingTab = tabs.FirstOrDefault(t => t.FormKey == formKey);
            if (existingTab != null)
            {
                MessageBox.Show("El panel ya está abierto. Se mostrará el panel existente.",
                              "Panel Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ActivateTab(tabs.IndexOf(existingTab));
                return false;
            }

            // Verificar límite de pestañas
            if (tabs.Count >= MAX_TABS)
            {
                MessageBox.Show($"No se pueden abrir más de {MAX_TABS} pestañas simultáneamente.",
                              "Límite de Pestañas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Crear nueva pestaña
            var tabInfo = new TabInfo
            {
                Title = title,
                FormInstance = formHija,
                FormKey = formKey
            };

            tabs.Add(tabInfo);

            // Configurar el formulario
            ConfigurarFormularioHijo(formHija, tabInfo);
            ActivateTab(tabs.Count - 1);
            this.Invalidate(); // Redibujar las pestañas

            return true;
        }

        public void MarkTabAsSaved(string formKey)
        {
            var tab = tabs.FirstOrDefault(t => t.FormKey == formKey);
            if (tab != null)
            {
                tab.HasUnsavedChanges = false;
                this.Invalidate();
            }
        }

        public void CloseAllTabs()
        {
            while (tabs.Count > 0)
            {
                if (!CloseTab(0))
                    break; // El usuario canceló el cierre
            }
        }

        public int TabCount => tabs.Count;

        public string GetActiveTabKey()
        {
            return activeTabIndex >= 0 && activeTabIndex < tabs.Count ?
                   tabs[activeTabIndex].FormKey : null;
        }
        #endregion

        #region Configuración de Formularios Hijos
        private void ConfigurarFormularioHijo(Form formHija, TabInfo tabInfo)
        {
            formHija.TopLevel = false;
            formHija.Dock = DockStyle.Fill;
            formHija.FormBorderStyle = FormBorderStyle.None;

            // Agregar evento para detectar cambios no guardados
            AttachUnsavedChangesEvents(formHija, tabInfo);
        }

        private void AttachUnsavedChangesEvents(Form form, TabInfo tabInfo)
        {
            // Detectar cambios en controles comunes
            foreach (Control control in GetAllControls(form))
            {
                if (control is TextBox textBox)
                {
                    textBox.TextChanged += (s, e) => tabInfo.HasUnsavedChanges = true;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.SelectedIndexChanged += (s, e) => tabInfo.HasUnsavedChanges = true;
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.CheckedChanged += (s, e) => tabInfo.HasUnsavedChanges = true;
                }
                else if (control is DateTimePicker dateTimePicker)
                {
                    dateTimePicker.ValueChanged += (s, e) => tabInfo.HasUnsavedChanges = true;
                }
                else if (control is NumericUpDown numericUpDown)
                {
                    numericUpDown.ValueChanged += (s, e) => tabInfo.HasUnsavedChanges = true;
                }
            }
        }

        private IEnumerable<Control> GetAllControls(Control container)
        {
            List<Control> controlList = new List<Control>();
            foreach (Control control in container.Controls)
            {
                controlList.Add(control);
                controlList.AddRange(GetAllControls(control));
            }
            return controlList;
        }
        #endregion

        #region Gestión de Pestañas
        private void ActivateTab(int index)
        {
            if (index < 0 || index >= tabs.Count) return;

            activeTabIndex = index;
            var activeTab = tabs[index];

            // Limpiar panel contenedor
            if (panelContenedor != null)
            {
                panelContenedor.Controls.Clear();
                panelContenedor.Controls.Add(activeTab.FormInstance);
                panelContenedor.Tag = activeTab.FormInstance;
                activeTab.FormInstance.Show();
            }

            // Dar foco al NavegadorTabConfig para que funcionen los atajos de teclado
            this.Focus();

            TabActivated?.Invoke(this, activeTab);
            this.Invalidate();
        }

        private bool CloseTab(int index, bool confirmarCierre = true)
        {
            if (index < 0 || index >= tabs.Count) return false;

            var tabToClose = tabs[index];

            // SIEMPRE mostrar confirmación de cierre como solicitas
            if (confirmarCierre)
            {
                string mensaje = tabToClose.HasUnsavedChanges ?
                    "Si cierra esta pestaña se perderán los avances realizados que no haya guardado.\n\n¿Está seguro de que desea continuar?" :
                    "Si cierra esta pestaña se perderán los avances realizados.\n\n¿Está seguro de que desea continuar?";

                var result = MessageBox.Show(
                    mensaje,
                    "Confirmar Cierre de Pestaña",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return false;
            }

            // Cerrar formulario
            tabToClose.FormInstance.Close();
            tabs.RemoveAt(index);

            TabClosed?.Invoke(this, tabToClose);

            // Ajustar pestaña activa
            if (activeTabIndex == index)
            {
                if (tabs.Count > 0)
                {
                    int newActiveIndex = Math.Min(index, tabs.Count - 1);
                    ActivateTab(newActiveIndex);
                }
                else
                {
                    activeTabIndex = -1;
                    if (panelContenedor != null)
                        panelContenedor.Controls.Clear();
                }
            }
            else if (activeTabIndex > index)
            {
                activeTabIndex--;
            }

            this.Invalidate();
            return true;
        }

        // Métodos públicos para navegación por teclado
        public void AvanzarPestanaDerecha()
        {
            if (tabs.Count <= 1) return;

            int nextIndex = (activeTabIndex + 1) % tabs.Count;
            ActivateTab(nextIndex);
        }

        public void AvanzarPestanaIzquierda()
        {
            if (tabs.Count <= 1) return;

            int prevIndex = activeTabIndex - 1;
            if (prevIndex < 0) prevIndex = tabs.Count - 1;
            ActivateTab(prevIndex);
        }

        public void CerrarPestanaActiva()
        {
            if (activeTabIndex >= 0 && activeTabIndex < tabs.Count)
            {
                CloseTab(activeTabIndex, true); // true = siempre confirmar
            }
        }
        #endregion

        #region Dibujo de Pestañas
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(this.BackColor);

            if (tabs.Count == 0) return;

            int tabWidth = CalcularAnchoTab();
            int currentX = 5; // Comenzar más a la izquierda

            for (int i = 0; i < tabs.Count; i++)
            {
                DrawTab(g, tabs[i], new Rectangle(currentX, 5, tabWidth, TAB_HEIGHT - 10),
                       i == activeTabIndex, i);
                currentX += tabWidth + TAB_MARGIN;
            }
        }

        private int CalcularAnchoTab()
        {
            if (tabs.Count == 0) return TAB_MIN_WIDTH;

            // Calcular ancho disponible considerando márgenes
            int availableWidth = this.Width - 20; // 10px margen a cada lado
            int calculatedWidth = availableWidth / tabs.Count;

            // Aplicar límites mínimo y máximo
            if (calculatedWidth < 80) // Mínimo más pequeño para más pestañas
                return 80;
            else if (calculatedWidth > TAB_MAX_WIDTH)
                return TAB_MAX_WIDTH;
            else
                return calculatedWidth;
        }

        private void DrawTab(Graphics g, TabInfo tab, Rectangle bounds, bool isActive, int tabIndex)
        {
            // Colores basados en tu tema con sutileza mejorada
            Color tabColor = isActive ? Color.White : Color.FromArgb(245, 245, 245);
            Color borderColor = Color.FromArgb(51, 224, 224, 224); // RGB(224,224,224) con transparencia 0.2f (51/255)
            Color textColor = Color.Black;

            // Configurar suavizado para mejor calidad visual
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            // Dibujar fondo de pestaña con bordes menos redondeados
            using (SolidBrush brush = new SolidBrush(tabColor))
            {
                g.FillRoundedRectangle(brush, bounds, 4); // Reducido de 8 a 4 para menos redondeo
            }

            // Dibujar borde fino y sutil
            using (Pen pen = new Pen(borderColor, 0.5f)) // Grosor reducido a 0.5f
            {
                g.DrawRoundedRectangle(pen, bounds, 4); // Mismo radio que el fondo
            }

            // Indicador de cambios no guardados (punto naranja más elegante)
            int textStartX = bounds.X + 8;
            if (tab.HasUnsavedChanges)
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(238, 143, 109))) // Tu color naranja
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Punto más suave
                    g.FillEllipse(brush, bounds.X + 6, bounds.Y + bounds.Height / 2 - 2, 4, 4);
                }
                textStartX = bounds.X + 16; // Dejar espacio para el punto
            }

            // Calcular espacio disponible para texto (dejando espacio para botón cerrar)
            int availableTextWidth = bounds.Width - (textStartX - bounds.X) - 25; // 25px para botón cerrar

            // Dibujar texto con truncado automático
            Rectangle textBounds = new Rectangle(textStartX, bounds.Y, availableTextWidth, bounds.Height);

            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter, // Esto añade "..." automáticamente
                    FormatFlags = StringFormatFlags.NoWrap
                };

                // Usar una fuente ligeramente más pequeña si la pestaña es muy estrecha
                Font fontToUse = this.Font;
                if (bounds.Width < 100)
                {
                    fontToUse = new Font(this.Font.FontFamily, 8F, this.Font.Style);
                }

                // Aplicar suavizado de texto para mejor legibilidad
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.DrawString(tab.Title, fontToUse, textBrush, textBounds, sf);

                // Liberar fuente temporal si se creó
                if (fontToUse != this.Font)
                {
                    fontToUse.Dispose();
                }
            }

            // Dibujar botón de cerrar
            DibujarBotonCerrar(g, bounds, tabIndex);
        }

        private void DibujarBotonCerrar(Graphics g, Rectangle bounds, int tabIndex)
        {
            Rectangle closeButtonRect = new Rectangle(bounds.Right - 20,
                                                    bounds.Y + (bounds.Height - CLOSE_BUTTON_SIZE) / 2,
                                                    CLOSE_BUTTON_SIZE,
                                                    CLOSE_BUTTON_SIZE);

            // Aplicar suavizado para mejor calidad visual
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Color más sutil para el botón cerrar
            Color closeButtonColor = (closeButtonHoverTab == tabIndex) ?
                Color.FromArgb(220, 53, 69) : // Rojo más suave en hover
                Color.FromArgb(120, 120, 120); // Gris más suave por defecto

            using (Pen closePen = new Pen(closeButtonColor, 1.5f)) // Línea más fina
            {
                // Dibujar X más elegante con líneas suavizadas
                g.DrawLine(closePen,
                          closeButtonRect.X + 4, closeButtonRect.Y + 4,
                          closeButtonRect.Right - 4, closeButtonRect.Bottom - 4);
                g.DrawLine(closePen,
                          closeButtonRect.Right - 4, closeButtonRect.Y + 4,
                          closeButtonRect.X + 4, closeButtonRect.Bottom - 4);
            }
        }
        #endregion

        #region Eventos de Teclado
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // CTRL + TAB: Avanzar a la derecha
            if (keyData == (Keys.Control | Keys.Tab))
            {
                AvanzarPestanaDerecha();
                return true;
            }

            // CTRL + SHIFT + TAB: Avanzar a la izquierda
            if (keyData == (Keys.Control | Keys.Shift | Keys.Tab))
            {
                AvanzarPestanaIzquierda();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            this.Focus(); // Asegurar que el control tenga foco para recibir eventos de teclado
        }
        #endregion

        #region Eventos de Mouse
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Dar foco al control cuando se hace clic
            this.Focus();

            if (tabs.Count == 0) return;

            int tabWidth = CalcularAnchoTab();
            int currentX = 5;

            for (int i = 0; i < tabs.Count; i++)
            {
                Rectangle tabBounds = new Rectangle(currentX, 5, tabWidth, TAB_HEIGHT - 10);
                Rectangle closeButtonRect = new Rectangle(tabBounds.Right - 20,
                                                        tabBounds.Y + (tabBounds.Height - CLOSE_BUTTON_SIZE) / 2,
                                                        CLOSE_BUTTON_SIZE,
                                                        CLOSE_BUTTON_SIZE);

                if (closeButtonRect.Contains(e.Location))
                {
                    CloseTab(i);
                    return;
                }
                else if (tabBounds.Contains(e.Location))
                {
                    ActivateTab(i);
                    return;
                }

                currentX += tabWidth + TAB_MARGIN;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int previousHoverTab = closeButtonHoverTab;
            closeButtonHoverTab = -1;

            if (tabs.Count == 0) return;

            int tabWidth = CalcularAnchoTab();
            int currentX = 5;

            for (int i = 0; i < tabs.Count; i++)
            {
                Rectangle tabBounds = new Rectangle(currentX, 5, tabWidth, TAB_HEIGHT - 10);
                Rectangle closeButtonRect = new Rectangle(tabBounds.Right - 20,
                                                        tabBounds.Y + (tabBounds.Height - CLOSE_BUTTON_SIZE) / 2,
                                                        CLOSE_BUTTON_SIZE,
                                                        CLOSE_BUTTON_SIZE);

                if (closeButtonRect.Contains(e.Location))
                {
                    closeButtonHoverTab = i;
                    this.Cursor = Cursors.Hand;
                    break;
                }
                else if (tabBounds.Contains(e.Location))
                {
                    this.Cursor = Cursors.Hand;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }

                currentX += tabWidth + TAB_MARGIN;
            }

            if (previousHoverTab != closeButtonHoverTab)
            {
                this.Invalidate();
            }
        }
        #endregion
    }

    // Extensiones para dibujar rectángulos redondeados
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle bounds, int radius)
        {
            if (radius <= 0)
            {
                g.FillRectangle(brush, bounds);
                return;
            }

            using (var path = CreateRoundedRectanglePath(bounds, radius))
            {
                g.FillPath(brush, path);
            }
        }

        public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle bounds, int radius)
        {
            if (radius <= 0)
            {
                g.DrawRectangle(pen, bounds);
                return;
            }

            using (var path = CreateRoundedRectanglePath(bounds, radius))
            {
                g.DrawPath(pen, path);
            }
        }

        private static System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int diameter = radius * 2;

            // Top-left arc
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);

            // Top-right arc
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);

            // Bottom-right arc
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);

            // Bottom-left arc
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}