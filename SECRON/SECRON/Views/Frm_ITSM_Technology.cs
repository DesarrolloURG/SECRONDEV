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
                ConfigurarVScrollBar();

                // Validar clasificación TECNOLOGÍA antes de cargar
                if (!ValidarClasificacionTecnologia()) return;

                ConfigurarComboBoxCategoria();
                LimpiarPanelAtributos();
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

        private void ConfigurarVScrollBar()
        {
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = 0;
            vScrollBar.Value = 0;
            vScrollBar.Visible = false;
            vScrollBar.Scroll += VScrollBar_Scroll;
        }

        private void ActualizarVScrollBar()
        {
            // Calcular el contenido total del Panel_Atributos
            int alturaTotal = _controlesAtributos != null && _controlesAtributos.Count > 0
                ? _controlesAtributos.Values
                    .Max(kvp => kvp.Entrada.Location.Y + kvp.Entrada.Height) + 30
                : 0;

            int alturaVisible = Panel_Atributos.Height;

            if (alturaTotal > alturaVisible)
            {
                vScrollBar.Maximum = alturaTotal - alturaVisible + 20;
                vScrollBar.Visible = true;
                vScrollBar.Value = 0;
            }
            else
            {
                vScrollBar.Maximum = 0;
                vScrollBar.Visible = false;
                vScrollBar.Value = 0;
            }
        }

        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            int desplazamiento = e.NewValue;

            foreach (var kvp in _posicionesOriginalesY)
            {
                Control ctrl = kvp.Key;
                int yOriginal = kvp.Value;

                // No desplazar el ComboBox (es fijo en Panel_1)
                if (ctrl.Parent == Panel_1) continue;

                if (ctrl.Parent == Panel_Atributos)
                    ctrl.Location = new Point(ctrl.Location.X, yOriginal - desplazamiento);
            }
        }

        #endregion

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
            _controlesAtributos = null;
            _posicionesOriginalesY.Clear();
            _activoSeleccionado = null;
            Btn_PrintLetter.Enabled = false;
            Btn_Update.Enabled = false;
            vScrollBar.Visible = false;

            var lbl = new Label
            {
                Text = "Seleccione un equipo de la lista para ver sus detalles.",
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(10, 40),
                AutoSize = true
            };
            Panel_Atributos.Controls.Add(lbl);
        }

        private void CargarAtributosEditablesEnPanel(int assetId, int categoryId)
        {
            try
            {
                Panel_Atributos.Controls.Clear();
                _controlesAtributos = new Dictionary<int, (Mdl_FixedAssetAttributeValue, Control)>();
                _posicionesOriginalesY = new Dictionary<Control, int>();

                var atributos = Ctrl_FixedAssetAttributeValues
                    .ObtenerValoresConPlantilla(assetId, categoryId);

                if (atributos == null || atributos.Count == 0)
                {
                    var lblVacio = new Label
                    {
                        Text = "Sin características definidas para esta categoría.",
                        Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        Location = new Point(10, 40),
                        AutoSize = true
                    };
                    Panel_Atributos.Controls.Add(lblVacio);
                    return;
                }

                int yPos = 40;
                int ancho = Panel_Atributos.Width - 30;

                foreach (var attr in atributos.OrderBy(a => a.AttributeDefId))
                {
                    // ── Label ────────────────────────────────────────────
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
                    _posicionesOriginalesY[lbl] = yPos;
                    yPos += 22;

                    // ── Hint ─────────────────────────────────────────────
                    string hint = ObtenerHint(attr.DataType, attr.AttributeKey);
                    if (!string.IsNullOrEmpty(hint))
                    {
                        var lblHint = new Label
                        {
                            Text = hint,
                            Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                            ForeColor = Color.Gray,
                            Location = new Point(10, yPos),
                            AutoSize = true
                        };
                        Panel_Atributos.Controls.Add(lblHint);
                        _posicionesOriginalesY[lblHint] = yPos;
                        yPos += 18;
                    }

                    // ── Control dinámico editable ─────────────────────────
                    Control entrada = CrearControlEditable(attr.DataType, attr.Value ?? "", attr.AttributeKey);
                    entrada.Location = new Point(10, yPos);
                    entrada.Width = ancho;
                    Panel_Atributos.Controls.Add(entrada);
                    _posicionesOriginalesY[entrada] = yPos;

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

                ActualizarVScrollBar();
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

            // Validar campos obligatorios
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

            try
            {
                // Guardar cada atributo
                foreach (var kvp in _controlesAtributos)
                {
                    var attr = kvp.Value.Atributo;
                    var ctrl = kvp.Value.Entrada;

                    attr.Value = ObtenerValorDeControlEditable(attr.DataType, ctrl);
                    attr.ModifiedBy = UserData?.UserId;

                    Ctrl_FixedAssetAttributeValues.RegistrarValor(attr);
                }

                MessageBox.Show("Características del equipo actualizadas correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar el panel para reflejar los valores guardados
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
            int margenIzq = 60;
            int margenDer = e.PageBounds.Width - 60;
            int ancho = margenDer - margenIzq;
            int yPos = 60;

            Font fTitulo = new Font("Segoe UI", 16F, FontStyle.Bold);
            Font fSubtitulo = new Font("Segoe UI", 11F, FontStyle.Bold);
            Font fNormal = new Font("Segoe UI", 10F);
            Font fNegrita = new Font("Segoe UI", 10F, FontStyle.Bold);
            Font fPequeña = new Font("Segoe UI", 9F, FontStyle.Italic);
            Brush negro = Brushes.Black;
            Brush gris = Brushes.Gray;
            Brush azul = new SolidBrush(Color.FromArgb(51, 140, 255));

            g.DrawString("UNIVERSIDAD REGIONAL REGIÓN 02 DE GUATEMALA", fNegrita, negro,
                new RectangleF(margenIzq, yPos, ancho, 25),
                new StringFormat { Alignment = StringAlignment.Center });
            yPos += 22;

            g.DrawString("SISTEMA DE CONTROL REGIONAL — SECRON", fPequeña, gris,
                new RectangleF(margenIzq, yPos, ancho, 20),
                new StringFormat { Alignment = StringAlignment.Center });
            yPos += 35;

            g.DrawLine(new Pen(Color.FromArgb(51, 140, 255), 2f), margenIzq, yPos, margenDer, yPos);
            yPos += 15;

            g.DrawString("CARTA DE RESPONSABILIDAD DE EQUIPO DE TECNOLOGÍA", fTitulo, azul,
                new RectangleF(margenIzq, yPos, ancho, 35),
                new StringFormat { Alignment = StringAlignment.Center });
            yPos += 45;

            string fecha = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy",
                new System.Globalization.CultureInfo("es-GT"));
            g.DrawString($"Guatemala, {fecha}", fNormal, negro, new PointF(margenIzq, yPos));
            g.DrawString($"Folio: {activo.AssetCode}-{DateTime.Now:yyyyMMddHHmm}", fNormal, gris,
                new RectangleF(margenIzq, yPos, ancho, 20),
                new StringFormat { Alignment = StringAlignment.Far });
            yPos += 35;

            string cuerpo =
                $"Por medio de la presente, yo, {activo.EmployeeName?.ToUpper() ?? "___________________________"}, " +
                $"en mi calidad de empleado(a) de la Universidad Regional Región 02 de Guatemala, " +
                $"manifiesto haber recibido el equipo de tecnología que a continuación se describe, " +
                $"en perfectas condiciones de funcionamiento, comprometiéndome a darle el uso " +
                $"adecuado, mantenerlo en buen estado y responder por cualquier daño, pérdida " +
                $"o extravío del mismo.";
            g.DrawString(cuerpo, fNormal, negro, new RectangleF(margenIzq, yPos, ancho, 100));
            yPos += 90;

            yPos += 10;
            g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 255)), margenIzq, yPos, ancho, 25);
            g.DrawString("  DATOS DEL EQUIPO", fSubtitulo, azul, new PointF(margenIzq, yPos + 4));
            yPos += 30;

            void FilaDato(string etiqueta, string valor)
            {
                g.DrawString(etiqueta, fNegrita, negro, new PointF(margenIzq + 5, yPos));
                g.DrawString(valor ?? "—", fNormal, negro, new PointF(margenIzq + 180, yPos));
                yPos += 22;
            }

            FilaDato("CÓDIGO DE ACTIVO:", activo.AssetCode);
            FilaDato("NOMBRE DEL EQUIPO:", activo.AssetName);
            FilaDato("CATEGORÍA:", activo.CategoryName);
            FilaDato("BODEGA / UBICACIÓN:", activo.WarehouseName ?? "SIN ASIGNAR");
            FilaDato("ESTADO:", activo.AssetStatus);
            FilaDato("FECHA DE ASIGNACIÓN:", DateTime.Now.ToString("dd/MM/yyyy"));

            if (atributos != null && atributos.Any(a => !string.IsNullOrWhiteSpace(a.Value)))
            {
                yPos += 10;
                g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 255)), margenIzq, yPos, ancho, 25);
                g.DrawString("  ESPECIFICACIONES TÉCNICAS", fSubtitulo, azul, new PointF(margenIzq, yPos + 4));
                yPos += 30;

                foreach (var attr in atributos.Where(a => !string.IsNullOrWhiteSpace(a.Value)).OrderBy(a => a.AttributeDefId))
                {
                    g.DrawString($"{attr.AttributeLabel.ToUpper()}:", fNegrita, negro, new PointF(margenIzq + 5, yPos));
                    g.DrawString(attr.Value, fNormal, negro, new PointF(margenIzq + 180, yPos));
                    yPos += 22;

                    if (yPos > e.PageBounds.Height - 180) { e.HasMorePages = true; return; }
                }
            }

            yPos += 20;
            g.DrawString(
                "El suscrito se compromete a utilizar el equipo únicamente para las actividades " +
                "institucionales asignadas, a notificar de inmediato cualquier falla o incidente, " +
                "y a devolverlo en las mismas condiciones en que fue recibido.",
                fNormal, negro, new RectangleF(margenIzq, yPos, ancho, 80));
            yPos += 80;

            yPos += 30;
            int centroFirma = margenIzq + (ancho / 2) - 100;
            g.DrawLine(Pens.Black, centroFirma, yPos, centroFirma + 200, yPos);
            yPos += 5;
            g.DrawString(activo.EmployeeName?.ToUpper() ?? "NOMBRE DEL RESPONSABLE", fNegrita, negro,
                new RectangleF(centroFirma - 50, yPos, 300, 20),
                new StringFormat { Alignment = StringAlignment.Center });
            yPos += 18;
            g.DrawString("FIRMA DEL RESPONSABLE", fPequeña, gris,
                new RectangleF(centroFirma - 50, yPos, 300, 20),
                new StringFormat { Alignment = StringAlignment.Center });
            yPos += 30;

            g.DrawLine(new Pen(Color.LightGray, 1f), margenIzq, yPos, margenDer, yPos);
            yPos += 8;
            g.DrawString(
                $"Generado por SECRON el {DateTime.Now:dd/MM/yyyy HH:mm} | {UserData?.FullName?.ToUpper() ?? "SISTEMA"}",
                fPequeña, gris,
                new RectangleF(margenIzq, yPos, ancho, 20),
                new StringFormat { Alignment = StringAlignment.Center });

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
    }
}