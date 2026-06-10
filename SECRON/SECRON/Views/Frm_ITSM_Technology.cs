using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_ITSM_Technology : Form
    {
        #region PropiedadesIniciales

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_FixedAsset> _activosList;
        private Mdl_FixedAsset _activoSeleccionado = null;
        private int? _classificationFilterId = null;
        private int _categoryFilterId = 0;

        // Controles dinámicos del panel de atributos
        private Dictionary<int, (Mdl_FixedAssetAttributeValue Atributo, Control Entrada)> _controlesAtributos;

        // Posiciones originales para vScrollBar
        private Dictionary<Control, int> _posicionesOriginalesY = new Dictionary<Control, int>();

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        #endregion

        #region Constructor

        public Frm_ITSM_Technology()
        {
            InitializeComponent();
        }

        #endregion

        #region Load

        private void Frm_ITSM_Technology_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarTabla();
                ConfigurarFiltros();
                AplicarEstilosBotones();
                InicializarScroll();
                ConfigurarEventosScroll();

                // Validar clasificación TECNOLOGÍA antes de cargar
                if (!ValidarClasificacionTecnologia()) return;

                ConfigurarComboBoxCategoria();
                ConfigurarPanel2();
                LimpiarPanelAtributos();
                CargarActivos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region ValidacionInicial

        private bool ValidarClasificacionTecnologia()
        {
            // Verificar que exista la clasificación TECNOLOGÍA
            var clasificaciones = Ctrl_FixedAssetClassificationCategories
                .MostrarClasificaciones(classificationCode: "TECH", isActive: true);

            if (clasificaciones == null || clasificaciones.Count == 0)
            {
                MessageBox.Show(
                    "⚠ La clasificación TECNOLOGÍA (TECH) no existe en el sistema.\n\n" +
                    "Debe registrarla antes de usar este módulo.\n" +
                    "Diríjase a: Módulo de Clasificaciones de Categorías.",
                    "Configuración requerida",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            _classificationFilterId = clasificaciones[0].ClassificationId;

            // Verificar que existan categorías vinculadas a TECNOLOGÍA
            var categorias = Ctrl_FixedAssetCategories
                .ObtenerCategoriasPorClasificacion(_classificationFilterId.Value);

            if (categorias == null || categorias.Count == 0)
            {
                MessageBox.Show(
                    "⚠ No hay categorías vinculadas a la clasificación TECNOLOGÍA.\n\n" +
                    "Debe crear categorías (LAPTOPS, MONITORES, etc.) y asignarles\n" +
                    "la clasificación TECNOLOGÍA antes de usar este módulo.",
                    "Categorías requeridas",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        #endregion

        #region EstiloBotones

        private void AplicarEstiloBotonEspecial(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(9, 184, 255);
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            boton.Height = 45;
            boton.Width = Math.Max(boton.Width, 180);
            boton.Cursor = Cursors.Hand;
            boton.TextAlign = ContentAlignment.MiddleCenter;

            boton.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, boton.Width, boton.Height, 20, 20));

            boton.MouseEnter += (s, ev) => boton.BackColor = Color.FromArgb(0, 150, 220);
            boton.MouseLeave += (s, ev) => boton.BackColor = Color.FromArgb(9, 184, 255);
        }

        private void AplicarEstilosBotones()
        {
            AplicarEstiloBotonEspecial(Btn_PrintLetter);
        }

        #endregion

        #region VScrollBar

        private void InicializarScroll()
        {
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                    ctrl.Tag = "OrigY:" + ctrl.Top;
            }

            int maxBottom = 0;
            foreach (Control ctrl in Panel_Izquierdo.Controls)
                maxBottom = Math.Max(maxBottom, ctrl.Bottom);

            int totalContentHeight = maxBottom + (Panel_Izquierdo.Height / 3);

            if (totalContentHeight <= Panel_Izquierdo.Height)
            {
                vScrollBar.Enabled = false;
                return;
            }

            vScrollBar.Enabled = true;
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = totalContentHeight - Panel_Izquierdo.Height;
            vScrollBar.SmallChange = 30;
            vScrollBar.LargeChange = Panel_Izquierdo.Height / 4;
            vScrollBar.Value = 0;

            vScrollBar.Scroll -= vScrollBar_Scroll;
            vScrollBar.Scroll += vScrollBar_Scroll;
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            MoverContenido(e.NewValue);
        }

        private void MoverContenido(int scrollPosition)
        {
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                    ctrl.Tag = "OrigY:" + ctrl.Top;

                string[] parts = ctrl.Tag.ToString().Split(':');
                int originalY = int.Parse(parts[1]);
                ctrl.Top = originalY - scrollPosition;
            }
            Panel_Izquierdo.Invalidate();
        }

        private void ConfigurarEventosScroll()
        {
            Panel_Izquierdo.TabStop = true;
            Panel_Izquierdo.MouseWheel += Panel_Izquierdo_MouseWheel;
            Panel_Izquierdo.MouseEnter += Panel_Izquierdo_MouseEnter;

            foreach (Control ctrl in Panel_Izquierdo.Controls)
                ctrl.MouseWheel += Panel_Izquierdo_MouseWheel;
        }

        private void Panel_Izquierdo_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!vScrollBar.Enabled) return;

            int newValue = vScrollBar.Value - (e.Delta / 120 * 30);
            newValue = Math.Max(0, Math.Min(newValue, vScrollBar.Maximum));
            vScrollBar.Value = newValue;
            MoverContenido(newValue);
        }

        private void Panel_Izquierdo_MouseEnter(object sender, EventArgs e)
        {
            Panel_Izquierdo.Focus();
        }

        #endregion VScrollBar

        #region Configuración

        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "ID", DataPropertyName = "AssetId", Visible = false });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "CÓDIGO", DataPropertyName = "AssetCode", Width = 110 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "NOMBRE", DataPropertyName = "AssetName", Width = 200 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCategoria", HeaderText = "CATEGORÍA", DataPropertyName = "CategoryName", Width = 130 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "ESTADO", DataPropertyName = "AssetStatus", Width = 100 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBodega", HeaderText = "BODEGA", DataPropertyName = "WarehouseName", Width = 130 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAsignado",
                HeaderText = "ASIGNADO A",
                DataPropertyName = "EmployeeName",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            Tabla.CellFormatting += Tabla_CellFormatting;
        }

        private void Tabla_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string estado = Tabla.Rows[e.RowIndex].Cells["colEstado"].Value?.ToString();
            switch (estado)
            {
                case "ACTIVE":
                    Tabla.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
                    break;
                case "TRANSFERRED":
                    Tabla.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205);
                    break;
                case "MAINTENANCE":
                    Tabla.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(205, 230, 255);
                    break;
                case "DISPOSED":
                    Tabla.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                    break;
            }
        }

        private void ConfigurarFiltros()
        {
            // Filtro1 — Tipo de búsqueda por texto
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro1.Items.Clear();
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.Items.Add("POR NOMBRE");
            Filtro1.Items.Add("POR SERIE");
            Filtro1.SelectedIndex = 0;

            // Filtro2 — Estado del activo
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS LOS ESTADOS");
            Filtro2.Items.Add("ACTIVE");
            Filtro2.Items.Add("TRANSFERRED");
            Filtro2.Items.Add("MAINTENANCE");
            Filtro2.Items.Add("DISPOSED");
            Filtro2.SelectedIndex = 0;

            // Filtro3 — Solo activos o todos
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.Items.Clear();
            Filtro3.Items.Add("ACTIVOS E INACTIVOS");
            Filtro3.Items.Add("SOLO ACTIVOS");
            Filtro3.Items.Add("SOLO INACTIVOS");
            Filtro3.SelectedIndex = 1;
        }

        private void ConfigurarComboBoxCategoria()
        {
            ComboBox_Categories.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Categories.Items.Clear();
            ComboBox_Categories.Items.Add(new KeyValuePair<int, string>(0, "TODAS LAS CATEGORÍAS"));

            if (_classificationFilterId.HasValue)
            {
                var categorias = Ctrl_FixedAssetCategories
                    .ObtenerCategoriasPorClasificacion(_classificationFilterId.Value);

                foreach (var cat in categorias)
                    ComboBox_Categories.Items.Add(cat);
            }

            ComboBox_Categories.DisplayMember = "Value";
            ComboBox_Categories.SelectedIndex = 0;
            ComboBox_Categories.SelectedIndexChanged += ComboBox_Categories_SelectedIndexChanged;
        }

        private void ComboBox_Categories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBox_Categories.SelectedItem is KeyValuePair<int, string> kvp)
                _categoryFilterId = kvp.Key;
            else
                _categoryFilterId = 0;

            CargarActivos();
            LimpiarPanelAtributos();
        }

        #endregion

        #region CargaDeDatos

        private void CargarActivos(string codigo = null, string nombre = null,
            string estado = null, string filtroEstado = "SOLO ACTIVOS")
        {
            try
            {
                string filtro1 = codigo != null ? "POR CÓDIGO" : nombre != null ? "POR NOMBRE" : "TODOS";
                string textoBusqueda = codigo ?? nombre ?? "";

                int? categoryId = _categoryFilterId > 0 ? (int?)_categoryFilterId : null;

                _activosList = Ctrl_FixedAssets.BuscarActivos(
                    textoBusqueda: textoBusqueda,
                    filtro1: filtro1,
                    filtroEstado: filtroEstado,
                    categoriaId: categoryId,
                    classificationId: _classificationFilterId,
                    assetStatus: estado);

                Tabla.DataSource = null;
                Tabla.DataSource = _activosList;

                Lbl_Paginas.Text = $"TOTAL: {_activosList?.Count ?? 0} EQUIPO(S)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar equipos: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region PanelAtributosDinamico

        private void LimpiarPanelAtributos()
        {
            Panel_Atributos.Controls.Clear();
            Panel_Atributos.AutoScroll = false;
            _controlesAtributos = null;
            _activoSeleccionado = null;
            Btn_PrintLetter.Enabled = false;
            Btn_Update.Enabled = false;

            Panel_Atributos.Controls.Add(new Label
            {
                Text = "Seleccione un equipo de la lista para ver sus detalles.",
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(10, 10),
                AutoSize = true
            });
        }

        private void CargarAtributosEditablesEnPanel(int assetId, int categoryId)
        {
            try
            {
                Panel_Atributos.Controls.Clear();
                Panel_Atributos.AutoScroll = true;
                _controlesAtributos = new Dictionary<int, (Mdl_FixedAssetAttributeValue, Control)>();

                var atributos = Ctrl_FixedAssetAttributeValues
                    .ObtenerValoresConPlantilla(assetId, categoryId);

                if (atributos == null || atributos.Count == 0)
                {
                    Panel_Atributos.Controls.Add(new Label
                    {
                        Text = "Sin características definidas para esta categoría.",
                        Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        Location = new Point(10, 10),
                        AutoSize = true
                    });
                    return;
                }

                int yPos = 10;
                int ancho = Panel_Atributos.Width - 25;

                foreach (var attr in atributos.OrderBy(a => a.AttributeDefId))
                {
                    var lbl = new Label
                    {
                        Text = attr.IsRequired
                                        ? $"{attr.AttributeLabel.ToUpper()} *"
                                        : attr.AttributeLabel.ToUpper(),
                        Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(60, 60, 60),
                        Location = new Point(10, yPos),
                        AutoSize = true
                    };
                    Panel_Atributos.Controls.Add(lbl);
                    yPos += 22;

                    string hint = ObtenerHint(attr.DataType, attr.AttributeKey);
                    if (!string.IsNullOrEmpty(hint))
                    {
                        Panel_Atributos.Controls.Add(new Label
                        {
                            Text = hint,
                            Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                            ForeColor = Color.Gray,
                            Location = new Point(10, yPos),
                            AutoSize = true
                        });
                        yPos += 18;
                    }

                    Control entrada = CrearControlEditable(attr.DataType, attr.Value ?? "", attr.AttributeKey);
                    entrada.Location = new Point(10, yPos);
                    entrada.Width = ancho;
                    Panel_Atributos.Controls.Add(entrada);

                    _controlesAtributos[attr.AttributeDefId] = (new Mdl_FixedAssetAttributeValue
                    {
                        AttributeValueId = attr.AttributeValueId,
                        AssetId = assetId,
                        AttributeDefId = attr.AttributeDefId,
                        AttributeKey = attr.AttributeKey,
                        AttributeLabel = attr.AttributeLabel,
                        DataType = attr.DataType,
                        IsRequired = attr.IsRequired,
                        Value = attr.Value ?? "",
                        CreatedBy = UserData?.UserId,
                        ModifiedBy = UserData?.UserId
                    }, entrada);

                    yPos += entrada.Height + 15;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar características: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string ObtenerHint(string dataType, string key)
        {
            switch (dataType?.ToUpper())
            {
                case "LISTA": return "Seleccione una opción.";
                case "RANGO": return "Formato: mínimo|máximo";
                case "EMAIL": return "usuario@dominio.com";
                case "URL": return "https://sitio.com";
                case "TELEFONO": return "+502 12345678";
                case "COLOR": return "#RRGGBB  (ej. #FF5733)";
                default: return null;
            }
        }

        private Control CrearControlEditable(string dataType, string valorActual, string attrKey = "")
        {
            switch (dataType?.ToUpper())
            {
                case "NUMERO":
                    var txtNum = new TextBox { Font = new Font("Segoe UI", 10F), Height = 28, Text = valorActual };
                    txtNum.KeyPress += (s, e) =>
                    {
                        if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b') e.Handled = true;
                    };
                    return txtNum;

                case "FECHA":
                    var dtp = new DateTimePicker
                    {
                        Font = new Font("Segoe UI", 10F),
                        Format = DateTimePickerFormat.Custom,
                        CustomFormat = "dd/MM/yyyy",
                        Height = 28
                    };
                    if (!string.IsNullOrWhiteSpace(valorActual) && DateTime.TryParse(valorActual, out DateTime fp))
                        dtp.Value = fp;
                    return dtp;

                case "FECHAHORA":
                    var dtph = new DateTimePicker
                    {
                        Font = new Font("Segoe UI", 10F),
                        Format = DateTimePickerFormat.Custom,
                        CustomFormat = "dd/MM/yyyy HH:mm",
                        Height = 28
                    };
                    if (!string.IsNullOrWhiteSpace(valorActual) && DateTime.TryParse(valorActual, out DateTime fhp))
                        dtph.Value = fhp;
                    return dtph;

                case "BOOLEAN":
                    var cmbBool = new ComboBox { Font = new Font("Segoe UI", 10F), DropDownStyle = ComboBoxStyle.DropDownList, Height = 28 };
                    cmbBool.Items.AddRange(new object[] { "SI", "NO" });
                    cmbBool.SelectedItem = string.IsNullOrWhiteSpace(valorActual) ? "NO" : (valorActual.ToUpper() == "SI" ? "SI" : "NO");
                    return cmbBool;

                case "LISTA":
                    var cmbLista = new ComboBox { Font = new Font("Segoe UI", 10F), DropDownStyle = ComboBoxStyle.DropDownList, Height = 28 };
                    foreach (var op in (attrKey ?? "").Split('|').Where(o => !string.IsNullOrWhiteSpace(o)))
                        cmbLista.Items.Add(op.Trim().ToUpper());
                    if (!string.IsNullOrWhiteSpace(valorActual) && cmbLista.Items.Contains(valorActual.ToUpper()))
                        cmbLista.SelectedItem = valorActual.ToUpper();
                    else if (cmbLista.Items.Count > 0)
                        cmbLista.SelectedIndex = 0;
                    return cmbLista;

                case "MULTILINEA":
                    return new TextBox { Font = new Font("Segoe UI", 10F), Multiline = true, Height = 70, ScrollBars = ScrollBars.Vertical, Text = valorActual };

                case "PORCENTAJE":
                    var pnlPct = new Panel { Height = 28, BackColor = Color.Transparent };
                    var txtPct = new TextBox { Font = new Font("Segoe UI", 10F), Height = 28, Width = 370, Location = new Point(0, 0), Text = valorActual };
                    var lblPct = new Label { Text = "%", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Location = new Point(375, 5), AutoSize = true, ForeColor = Color.Gray };
                    txtPct.KeyPress += (s, e) => { if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b') e.Handled = true; };
                    pnlPct.Controls.Add(txtPct); pnlPct.Controls.Add(lblPct); pnlPct.Tag = txtPct;
                    return pnlPct;

                case "MONEDA":
                    var pnlMon = new Panel { Height = 28, BackColor = Color.Transparent };
                    var lblMon = new Label { Text = "Q", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Location = new Point(0, 5), AutoSize = true, ForeColor = Color.Gray };
                    var txtMon = new TextBox { Font = new Font("Segoe UI", 10F), Height = 28, Width = 380, Location = new Point(18, 0), Text = valorActual };
                    txtMon.KeyPress += (s, e) => { if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b') e.Handled = true; };
                    pnlMon.Controls.Add(lblMon); pnlMon.Controls.Add(txtMon); pnlMon.Tag = txtMon;
                    return pnlMon;

                case "EMAIL":
                    return new TextBox { Font = new Font("Segoe UI", 10F), Height = 28, Text = valorActual };

                case "URL":
                    return new TextBox { Font = new Font("Segoe UI", 10F), Height = 28, Text = valorActual };

                case "TELEFONO":
                    var txtTel = new TextBox { Font = new Font("Segoe UI", 10F), Height = 28, Text = valorActual };
                    txtTel.KeyPress += (s, e) =>
                    {
                        if (!char.IsDigit(e.KeyChar) && e.KeyChar != '+' && e.KeyChar != ' ' && e.KeyChar != '-' && e.KeyChar != '\b')
                            e.Handled = true;
                    };
                    return txtTel;

                case "RANGO":
                    var pnlR = new Panel { Height = 28, BackColor = Color.Transparent };
                    string[] partes = (valorActual ?? "").Split('|');
                    var txtMin = new TextBox { Font = new Font("Segoe UI", 10F), Height = 28, Width = 175, Location = new Point(0, 0), Text = partes.Length > 0 ? partes[0] : "" };
                    var lblR = new Label { Text = "—", Font = new Font("Segoe UI", 11F, FontStyle.Bold), Location = new Point(180, 5), AutoSize = true, ForeColor = Color.Gray };
                    var txtMax = new TextBox { Font = new Font("Segoe UI", 10F), Height = 28, Width = 175, Location = new Point(200, 0), Text = partes.Length > 1 ? partes[1] : "" };
                    foreach (var t in new[] { txtMin, txtMax })
                    { var tx = t; tx.KeyPress += (s, e) => { if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b') e.Handled = true; }; }
                    pnlR.Controls.Add(txtMin); pnlR.Controls.Add(lblR); pnlR.Controls.Add(txtMax);
                    pnlR.Tag = new[] { txtMin, txtMax };
                    return pnlR;

                case "COLOR":
                    var pnlCol = new Panel { Height = 28, BackColor = Color.Transparent };
                    var txtCol = new TextBox { Font = new Font("Segoe UI", 10F), Height = 28, Width = 360, Location = new Point(0, 0), Text = valorActual ?? "#FFFFFF" };
                    var pnlMuestra = new Panel { Width = 28, Height = 28, Location = new Point(368, 0), BorderStyle = BorderStyle.FixedSingle };
                    txtCol.TextChanged += (s, ev) => { try { pnlMuestra.BackColor = ColorTranslator.FromHtml(txtCol.Text); } catch { pnlMuestra.BackColor = Color.White; } };
                    try { pnlMuestra.BackColor = ColorTranslator.FromHtml(valorActual ?? "#FFFFFF"); } catch { pnlMuestra.BackColor = Color.White; }
                    pnlCol.Controls.Add(txtCol); pnlCol.Controls.Add(pnlMuestra); pnlCol.Tag = txtCol;
                    return pnlCol;

                default:
                    return new TextBox { Font = new Font("Segoe UI", 10F), Height = 28, Text = valorActual ?? "" };
            }
        }

        private string ObtenerValorDeControlEditable(string dataType, Control control)
        {
            switch (dataType?.ToUpper())
            {
                case "FECHA": return ((DateTimePicker)control).Value.ToString("yyyy-MM-dd");
                case "FECHAHORA": return ((DateTimePicker)control).Value.ToString("yyyy-MM-dd HH:mm");
                case "BOOLEAN":
                case "LISTA": return ((ComboBox)control).SelectedItem?.ToString() ?? "";
                case "MULTILINEA": return ((TextBox)control).Text.Trim();
                case "PORCENTAJE":
                case "MONEDA":
                case "COLOR":
                    if (control is Panel pnl && pnl.Tag is TextBox ti) return ti.Text.Trim();
                    return "";
                case "RANGO":
                    if (control is Panel pnlR && pnlR.Tag is TextBox[] campos)
                        return $"{campos[0].Text.Trim()}|{campos[1].Text.Trim()}";
                    return "";
                default:
                    return control is TextBox tb ? tb.Text.Trim() : "";
            }
        }

        #endregion

        #region SelecciónEnTabla

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = Tabla.Rows[e.RowIndex];
            int assetId = Convert.ToInt32(row.Cells["colId"].Value);

            _activoSeleccionado = _activosList?.Find(a => a.AssetId == assetId);
            if (_activoSeleccionado == null) return;

            CargarAtributosEditablesEnPanel(
                _activoSeleccionado.AssetId,
                _activoSeleccionado.AssetCategoryId);

            Btn_PrintLetter.Enabled = _activoSeleccionado.AssignedToEmployeeId.HasValue;
            Btn_Update.Enabled = true;
            Btn_Assign.Enabled = true;
        }

        #endregion

        #region Búsqueda

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string valor = Txt_ValorBuscado.Text.Trim().ToUpper();
            string filtro = Filtro1.SelectedItem?.ToString();

            string codigo = filtro == "POR CÓDIGO" ? valor : null;
            string nombre = filtro == "POR NOMBRE" ? valor : null;
            string serie = filtro == "POR SERIE" ? valor : null;

            string estado = Filtro2.SelectedItem?.ToString();
            if (estado == "TODOS LOS ESTADOS") estado = null;

            string filtroEstado;
            string filtro3Val = Filtro3.SelectedItem?.ToString();
            if (filtro3Val == "SOLO ACTIVOS")
                filtroEstado = "SOLO ACTIVOS";
            else if (filtro3Val == "SOLO INACTIVOS")
                filtroEstado = "SOLO INACTIVOS";
            else
                filtroEstado = "TODOS";

            CargarActivos(
                codigo: codigo ?? serie,
                nombre: nombre,
                estado: estado,
                filtroEstado: filtroEstado);

            LimpiarPanelAtributos();
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Clear();
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 1;
            CargarActivos();
            LimpiarPanelAtributos();
        }

        #endregion

        #region CRUD

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (_activoSeleccionado == null || _controlesAtributos == null)
            {
                MessageBox.Show("Debe seleccionar un equipo de la lista.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar obligatorios
            foreach (var kvp in _controlesAtributos)
            {
                var attr = kvp.Value.Atributo;
                var ctrl = kvp.Value.Entrada;
                string val = ObtenerValorDeControlEditable(attr.DataType, ctrl);

                if (attr.IsRequired && string.IsNullOrWhiteSpace(val))
                {
                    MessageBox.Show($"El campo '{attr.AttributeLabel}' es obligatorio.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ctrl.Focus();
                    return;
                }
            }

            // Confirmación antes de guardar
            if (MessageBox.Show(
                $"¿Confirma actualizar las características del equipo '{_activoSeleccionado.AssetName}'?",
                "Confirmar actualización",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                foreach (var kvp in _controlesAtributos)
                {
                    var attr = kvp.Value.Atributo;
                    var ctrl = kvp.Value.Entrada;

                    attr.Value = ObtenerValorDeControlEditable(attr.DataType, ctrl);
                    attr.ModifiedBy = UserData?.UserId;

                    Ctrl_FixedAssetAttributeValues.RegistrarValor(attr);
                }

                MessageBox.Show("Características actualizadas correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarAtributosEditablesEnPanel(
                    _activoSeleccionado.AssetId,
                    _activoSeleccionado.AssetCategoryId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar características: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region CartaDeResponsabilidad

        private void Btn_PrintLetter_Click(object sender, EventArgs e)
        {
            if (_activoSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un equipo de la lista.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_activoSeleccionado.AssignedToEmployeeId.HasValue)
            {
                MessageBox.Show(
                    "El equipo no tiene un empleado asignado.\n" +
                    "La carta de responsabilidad requiere un responsable.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var atributos = Ctrl_FixedAssetAttributeValues
                    .ObtenerValoresConPlantilla(
                        _activoSeleccionado.AssetId,
                        _activoSeleccionado.AssetCategoryId);

                var printDoc = new PrintDocument();
                printDoc.DocumentName = $"Carta_Responsabilidad_{_activoSeleccionado.AssetCode}";
                printDoc.PrintPage += (s, ev) =>
                    ImprimirCartaDeResponsabilidad(ev, _activoSeleccionado, atributos);

                var preview = new PrintPreviewDialog
                {
                    Document = printDoc,
                    WindowState = FormWindowState.Maximized,
                    Text = $"CARTA DE RESPONSABILIDAD — {_activoSeleccionado.AssetCode}"
                };
                preview.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar la carta: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImprimirCartaDeResponsabilidad(
    PrintPageEventArgs e,
    Mdl_FixedAsset activo,
    List<Mdl_FixedAssetAttributeValue> atributos)
        {
            Graphics g = e.Graphics;
            Brush brush = Brushes.Black;
            Pen pen = new Pen(Color.Black, 1);
            Pen penGris = new Pen(Color.Gray, 0.5f);

            Font fontTitulo = new Font("Calibri", 13, FontStyle.Bold);
            Font fontBold = new Font("Calibri", 10, FontStyle.Bold);
            Font fontNormal = new Font("Calibri", 10);
            Font fontSmall = new Font("Calibri", 9);
            Font fontSmallBold = new Font("Calibri", 9, FontStyle.Bold);
            Font fontTable = new Font("Calibri", 8);
            Font fontTableBold = new Font("Calibri", 8, FontStyle.Bold);

            int leftMargin = 50;
            int rightMargin = 760;
            int pageWidth = rightMargin - leftMargin;
            int currentY = 20;
            float valorX = leftMargin + 200;

            // ===== LOGO ENCABEZADO =====
            try
            {
                Image logoEncabezado = Properties.Resources.LogoMembretadoEncabezado;
                int logoW = 240, logoH = 80;
                int logoX = leftMargin + (pageWidth - logoW) / 2;
                g.DrawImage(logoEncabezado, logoX, currentY + 15, logoW, logoH);
            }
            catch { }

            currentY += 50;

            // ===== REFERENCIA Y FECHA =====
            string fecha = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy",
                new System.Globalization.CultureInfo("es-GT")).ToUpper();
            g.DrawString($"Ref. {activo.AssetCode}", fontSmallBold, brush, rightMargin - 200, currentY + 65);
            g.DrawString($"Guatemala, {fecha}", fontSmall, brush, rightMargin - 200, currentY + 80);

            currentY += 105;

            // ===== TÍTULO =====
            string titulo = "CARTA DE RESPONSABILIDAD DE EQUIPO DE TECNOLOGÍA";
            SizeF tituloSize = g.MeasureString(titulo, fontTitulo);
            float tituloX = leftMargin + (pageWidth - tituloSize.Width) / 2;
            g.DrawString(titulo, fontTitulo, brush, tituloX, currentY);
            currentY += 28;

            // Línea naranja
            g.DrawLine(new Pen(Color.FromArgb(230, 115, 40), 2), leftMargin, currentY, rightMargin, currentY);
            currentY += 18;

            // ===== DATOS DEL EQUIPO =====
            g.DrawString("ASIGNADO A:", fontBold, brush, leftMargin, currentY);
            g.DrawString(activo.EmployeeName?.ToUpper() ?? "___________________________", fontNormal, brush, valorX, currentY);
            currentY += 27;

            g.DrawString("DE:", fontBold, brush, leftMargin, currentY);
            g.DrawString(UserData?.FullName?.ToUpper() ?? "", fontNormal, brush, valorX, currentY);
            currentY += 18;
            g.DrawString("UNIVERSIDAD REGIONAL DE GUATEMALA", fontNormal, brush, valorX, currentY);
            currentY += 28;

            g.DrawString("EQUIPO:", fontBold, brush, leftMargin, currentY);
            g.DrawString($"{activo.AssetCode} — {activo.AssetName?.ToUpper() ?? ""}", fontNormal, brush, valorX, currentY);
            currentY += 27;

            g.DrawString("CATEGORÍA:", fontBold, brush, leftMargin, currentY);
            g.DrawString(activo.CategoryName?.ToUpper() ?? "", fontNormal, brush, valorX, currentY);
            currentY += 27;

            g.DrawString("BODEGA / SEDE:", fontBold, brush, leftMargin, currentY);
            g.DrawString(activo.WarehouseName?.ToUpper() ?? "SIN ASIGNAR", fontNormal, brush, valorX, currentY);
            currentY += 35;

            // ===== CUERPO — compromiso del responsable =====
            string nombreResponsableTexto = activo.EmployeeName?.ToUpper() ?? "___________________________";
            string cuerpo =
                $"Por medio de la presente, yo, {nombreResponsableTexto}, " +
                $"en mi calidad de empleado(a) de la Universidad Regional Región 02 de Guatemala, " +
                $"manifiesto haber recibido el equipo de tecnología descrito a continuación " +
                $"en perfectas condiciones de funcionamiento, comprometiéndome a darle el uso " +
                $"adecuado, mantenerlo en buen estado y responder por cualquier daño, pérdida " +
                $"o extravío del mismo.";
            g.DrawString(cuerpo, fontNormal, brush, new RectangleF(leftMargin, currentY, pageWidth, 70));
            currentY += 80;

            // Línea gris
            g.DrawLine(penGris, leftMargin, currentY, rightMargin, currentY);
            currentY += 18;

            // ===== TABLA DE ESPECIFICACIONES TÉCNICAS =====
            int[] colWidths = { 220, pageWidth - 220 };
            string[] headers = { "CARACTERÍSTICA", "VALOR" };
            int tableX = leftMargin;
            int rowH = 20;

            g.FillRectangle(new SolidBrush(Color.FromArgb(30, 80, 160)), tableX, currentY, pageWidth, rowH);
            int colX = tableX;
            for (int i = 0; i < headers.Length; i++)
            {
                g.DrawString(headers[i], fontTableBold, Brushes.White, colX + 3, currentY + 4);
                colX += colWidths[i];
            }
            g.DrawRectangle(pen, tableX, currentY, pageWidth, rowH);
            currentY += rowH;

            bool altRow = false;
            int tableBodyStart = currentY;
            int filasTotales = 0;

            void FilaTabla(string etiqueta, string valor)
            {
                if (altRow)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 248)),
                        tableX, currentY, pageWidth, rowH);
                g.DrawString(etiqueta, fontTableBold, brush, tableX + 3, currentY + 4);
                g.DrawString(valor ?? "—", fontTable, brush, tableX + colWidths[0] + 3, currentY + 4);
                g.DrawRectangle(penGris, tableX, currentY, pageWidth, rowH);
                currentY += rowH;
                altRow = !altRow;
                filasTotales++;
            }

            // Datos básicos del activo
            FilaTabla("CÓDIGO DE ACTIVO", activo.AssetCode);
            FilaTabla("NOMBRE DEL EQUIPO", activo.AssetName?.ToUpper() ?? "");
            FilaTabla("CATEGORÍA", activo.CategoryName?.ToUpper() ?? "");
            FilaTabla("ESTADO", activo.AssetStatus?.ToUpper() ?? "");
            FilaTabla("BODEGA / UBICACIÓN", activo.WarehouseName?.ToUpper() ?? "SIN ASIGNAR");
            FilaTabla("FECHA DE ASIGNACIÓN", DateTime.Now.ToString("dd/MM/yyyy"));

            // Atributos técnicos del equipo
            if (atributos != null)
            {
                foreach (var attr in atributos
                    .Where(a => !string.IsNullOrWhiteSpace(a.Value))
                    .OrderBy(a => a.AttributeDefId))
                {
                    FilaTabla(attr.AttributeLabel.ToUpper(), attr.Value);

                    if (currentY > e.PageBounds.Height - 180)
                    {
                        e.HasMorePages = true;
                        return;
                    }
                }
            }

            // Borde exterior de la tabla completa
            g.DrawRectangle(pen, tableX, tableBodyStart - rowH, pageWidth, rowH * (filasTotales + 1));

            currentY += 15;

            // ===== COMPROMISO FINAL =====
            string compromiso =
                "El suscrito se compromete a utilizar el equipo únicamente para las actividades " +
                "institucionales asignadas, a notificar de inmediato cualquier falla o incidente, " +
                "y a devolverlo en las mismas condiciones en que fue recibido al finalizar su " +
                "relación laboral o cuando la institución así lo requiera.";
            g.DrawString(compromiso, fontNormal, brush,
                new RectangleF(leftMargin, currentY, pageWidth, 60));
            currentY += 65;

            // ===== PIE DE PÁGINA =====
            int pieY = e.PageBounds.Height - 130;
            try
            {
                Image piePagina = Properties.Resources.MembretadoPiePagina;
                g.DrawImage(piePagina, leftMargin - 10, pieY, pageWidth + 20, 110);
            }
            catch { }

            // ===== FIRMAS =====
            int firmaLineaY = pieY - 75;
            int firma1X = leftMargin;
            int firma3X = leftMargin + 480;
            int firmaAncho = 200;

            // Sello sobre firma 3
            try
            {
                Image sello = Properties.Resources.SelloCoordinacion_Black;
                g.DrawImage(sello, firma3X, firmaLineaY - 200, firmaAncho, 200);
            }
            catch { }

            // Líneas de firma
            g.DrawLine(pen, firma1X, firmaLineaY, firma1X + firmaAncho, firmaLineaY);
            g.DrawLine(pen, firma3X, firmaLineaY, firma3X + firmaAncho, firmaLineaY);

            int textoFirmaY = firmaLineaY + 5;

            // Firma 1 — responsable del equipo
            string nombreResponsable = activo.EmployeeName?.ToUpper() ?? "NOMBRE DEL RESPONSABLE";
            SizeF sizeF1 = g.MeasureString(nombreResponsable, fontSmallBold);
            g.DrawString(nombreResponsable, fontSmallBold, brush,
                firma1X + (firmaAncho / 2f) - (sizeF1.Width / 2f), textoFirmaY);
            SizeF sizeCargo = g.MeasureString("RESPONSABLE DEL EQUIPO", fontSmall);
            g.DrawString("RESPONSABLE DEL EQUIPO", fontSmall, brush,
                firma1X + (firmaAncho / 2f) - (sizeCargo.Width / 2f), textoFirmaY + 14);

            // Firma 3 — coordinación general
            SizeF sizeF3Titulo = g.MeasureString("COORDINACIÓN GENERAL", fontSmallBold);
            SizeF sizeF3Oficina = g.MeasureString("OFICINAS CENTRALES", fontSmall);
            g.DrawString("COORDINACIÓN GENERAL", fontSmallBold, brush,
                firma3X + (firmaAncho / 2f) - (sizeF3Titulo.Width / 2f), textoFirmaY);
            g.DrawString("OFICINAS CENTRALES", fontSmall, brush,
                firma3X + (firmaAncho / 2f) - (sizeF3Oficina.Width / 2f), textoFirmaY + 14);

            // Liberar recursos
            fontTitulo.Dispose(); fontBold.Dispose(); fontNormal.Dispose();
            fontSmall.Dispose(); fontSmallBold.Dispose();
            fontTable.Dispose(); fontTableBold.Dispose();
            pen.Dispose(); penGris.Dispose();

            e.HasMorePages = false;
        }

        #endregion

        #region ExportarExcel

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                if (_activosList == null || _activosList.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Inventario ITSM",
                    FileName = $"ITSM_Tecnologia_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK) return;

                this.Cursor = Cursors.WaitCursor;

                var excelApp = new Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                worksheet.Name = "ITSM Tecnología";

                worksheet.Cells[1, 1] = "INVENTARIO DE EQUIPOS DE TECNOLOGÍA — SECRON";
                worksheet.Range["A1:G1"].Merge();
                worksheet.Range["A1:G1"].Font.Size = 16;
                worksheet.Range["A1:G1"].Font.Bold = true;
                worksheet.Range["A1:G1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                worksheet.Range["A1:G1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                worksheet.Range["A1:G1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SECRON"}";
                worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {_activosList.Count}";

                int headerRow = 6;
                string[] headers = { "CÓDIGO", "NOMBRE", "CATEGORÍA", "ESTADO", "BODEGA", "ASIGNADO A", "VALOR COMPRA" };
                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cells[headerRow, i + 1] = headers[i];

                var headerRange = worksheet.Range[$"A{headerRow}:G{headerRow}"];
                headerRange.Font.Bold = true;
                headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                int row = headerRow + 1;
                foreach (var a in _activosList)
                {
                    worksheet.Cells[row, 1] = a.AssetCode;
                    worksheet.Cells[row, 2] = a.AssetName;
                    worksheet.Cells[row, 3] = a.CategoryName ?? "";
                    worksheet.Cells[row, 4] = a.AssetStatus ?? "";
                    worksheet.Cells[row, 5] = a.WarehouseName ?? "";
                    worksheet.Cells[row, 6] = a.EmployeeName ?? "";
                    worksheet.Cells[row, 7] = a.PurchaseValue;

                    if (row % 2 == 0)
                        worksheet.Range[$"A{row}:G{row}"].Interior.Color =
                            System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                    row++;
                }

                var dataRange = worksheet.Range[$"A{headerRow}:G{row - 1}"];
                dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;
                worksheet.Columns.AutoFit();

                workbook.SaveAs(saveDialog.FileName);
                workbook.Close();
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                this.Cursor = Cursors.Default;

                if (MessageBox.Show("Archivo exportado.\n\n¿Desea abrirlo ahora?",
                    "Éxito", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(saveDialog.FileName);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Asignacion

        private void ConfigurarPanel2()
        {
            ComboBox_TipoDestino.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_TipoDestino.Items.Clear();
            ComboBox_TipoDestino.Items.Add("EMPLEADO");
            ComboBox_TipoDestino.Items.Add("BODEGA");
            ComboBox_TipoDestino.SelectedIndex = 0;
            ComboBox_TipoDestino.SelectedIndexChanged += ComboBox_TipoDestino_SelectedIndexChanged;

            ComboBox_Location.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Location.SelectedIndexChanged += ComboBox_Location_SelectedIndexChanged;

            ComboBox_ToEmployee.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_ToLocation.DropDownStyle = ComboBoxStyle.DropDownList;

            CargarSedes();
            ActualizarVisibilidadDestino();

            Btn_Assign.Enabled = false;
        }

        private void CargarSedes()
        {
            ComboBox_Location.Items.Clear();
            ComboBox_Location.Items.Add(new KeyValuePair<int, string>(0, "TODAS LAS SEDES"));

            var sedes = Ctrl_Locations.ObtenerLocationsActivas();
            foreach (var s in sedes)
                ComboBox_Location.Items.Add(s);

            ComboBox_Location.DisplayMember = "Value";
            ComboBox_Location.SelectedIndex = 0;
        }

        private void ComboBox_TipoDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarVisibilidadDestino();
        }

        private void ActualizarVisibilidadDestino()
        {
            string tipo = ComboBox_TipoDestino.SelectedItem?.ToString();

            Lbl_ToEmployee.Enabled = tipo == "EMPLEADO";
            ComboBox_ToEmployee.Enabled = tipo == "EMPLEADO";
            Lbl_ToLocation.Enabled = tipo == "BODEGA";
            ComboBox_ToLocation.Enabled = tipo == "BODEGA";

            if (tipo == "EMPLEADO")
                CargarEmpleadosPorSede();
            else
                CargarBodegasPorSede();
        }

        private void ComboBox_Location_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarVisibilidadDestino();
        }

        private void CargarEmpleadosPorSede()
        {
            ComboBox_ToEmployee.Items.Clear();
            int locationId = ComboBox_Location.SelectedItem is KeyValuePair<int, string> kvp
                ? kvp.Key : 0;

            var empleados = locationId > 0
                ? Ctrl_Employees.ObtenerEmpleadosPorSede(locationId)
                : Ctrl_Employees.ObtenerEmpleadosParaCombo();

            foreach (var emp in empleados)
                ComboBox_ToEmployee.Items.Add(emp);

            ComboBox_ToEmployee.DisplayMember = "Value";
            if (ComboBox_ToEmployee.Items.Count > 0)
                ComboBox_ToEmployee.SelectedIndex = 0;
        }

        private void CargarBodegasPorSede()
        {
            ComboBox_ToLocation.Items.Clear();
            int locationId = ComboBox_Location.SelectedItem is KeyValuePair<int, string> kvp
                ? kvp.Key : 0;

            var bodegas = locationId > 0
            ? Ctrl_Warehouses.ObtenerBodegasPorLocation(locationId)
            : Ctrl_Warehouses.ObtenerBodegasParaCombo();

            foreach (var b in bodegas)
                ComboBox_ToLocation.Items.Add(b);

            ComboBox_ToLocation.DisplayMember = "Value";
            if (ComboBox_ToLocation.Items.Count > 0)
                ComboBox_ToLocation.SelectedIndex = 0;
        }

        private void Btn_Assign_Click(object sender, EventArgs e)
        {
            if (_activoSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un equipo de la lista.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tipo = ComboBox_TipoDestino.SelectedItem?.ToString();

            int? empleadoId = null;
            int? bodegaId = null;
            string destino = "";

            if (tipo == "EMPLEADO")
            {
                if (ComboBox_ToEmployee.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar un empleado.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var kvp = (KeyValuePair<int, string>)ComboBox_ToEmployee.SelectedItem;
                empleadoId = kvp.Key;
                bodegaId = null; // limpiar bodega
                destino = kvp.Value;
            }
            else // BODEGA
            {
                if (ComboBox_ToLocation.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar una bodega.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var kvp = (KeyValuePair<int, string>)ComboBox_ToLocation.SelectedItem;
                bodegaId = kvp.Key;
                empleadoId = null; // limpiar empleado
                destino = kvp.Value;
            }

            if (MessageBox.Show(
                $"¿Confirma asignar el equipo '{_activoSeleccionado.AssetName}' a '{destino}'?",
                "Confirmar asignación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            int resultado = Ctrl_FixedAssets.ActualizarAsignacionActivo(
                _activoSeleccionado.AssetId,
                empleadoId,
                bodegaId,
                UserData?.UserId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Equipo asignado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarActivos();
                    LimpiarPanelAtributos();
                    break;
                case -1:
                    MessageBox.Show("El activo no existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Error al asignar el equipo.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        #endregion
    }
}