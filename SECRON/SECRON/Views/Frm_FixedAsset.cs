    using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_FixedAsset : Form
    {
        #region PropiedadesIniciales

        public Mdl_Security_UserInfo UserData { get; set; }
        private List<Mdl_FixedAssetAttributeValue> _atributosValoresList;

        // Filtros búsqueda
        private string _ultimoTextoBusqueda = "";
        private string _ultimoFiltro1 = "TODOS";
        private string _ultimoFiltroEstado = "SOLO ACTIVOS";
        private int? _ultimoCategoriaId = null;

        // Selección actual
        private List<Mdl_FixedAsset> _assetsList;
        private Mdl_FixedAsset _activoSeleccionado = null;

        // IDs seleccionados mediante búsqueda
        private int? _categoriaSeleccionadaId = null;
        private int? _categoriaAnteriorId = null; // Para detectar cambio de categoría
        private int? _proveedorSeleccionadoId = null;
        private int? _bodegaSeleccionadaId = null;
        private int? _empleadoSeleccionadoId = null;

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        public Frm_FixedAsset()
        {
            InitializeComponent();
            this.Resize += FormularioResize;
            this.Resize += (s, e) =>
            {
                if (toolStripPaginacion != null)
                    toolStripPaginacion.Location = new Point(this.Width - 300, 170);
            };
        }

        private async void Frm_FixedAsset_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarTabIndexYFocus();
                ConfigurarMaxLengthTextBox();
                ConfigurarComponentesDeshabilitados();
                ConfigurarPlaceHoldersTextbox();
                ConfigurarFiltros();
                ConfigurarCombos();
                CrearToolStripPaginacion();
                CargarActivos();
                LimpiarTablaAtributos();
                ConfigurarTablaAtributos();
                AjustarColumnasAtributos();
                ConfigurarFiltrosAtributos();

                ActualizarInfoPaginacion();

                if (UserData != null)
                {
                    await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
                    ConfigurarControlesPorPermisos();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormularioResize(object sender, EventArgs e)
        {
            if (Tabla != null && Tabla.DataSource != null)
                Tabla.Refresh();
        }

        #endregion PropiedadesIniciales

        #region ConfigurarTextBox

        private void ConfigurarMaxLengthTextBox()
        {
            Txt_ValorBuscado.MaxLength = 100;
            Txt_AssetCode.MaxLength = 30;
            Txt_AssetName.MaxLength = 150;
            Txt_Description.MaxLength = 500;
            Txt_Brand.MaxLength = 100;
            Txt_Model.MaxLength = 100;
            Txt_Serial.MaxLength = 100;
            Txt_PurchaseValue.MaxLength = 18;
            Txt_ResidualValue.MaxLength = 5;
            Txt_ResidualValueAct.MaxLength = 18;
            Txt_InvoiceNumber.MaxLength = 50;
            Txt_WarrantyDocumentPath.MaxLength = 500;
            Txt_Notes.MaxLength = 1000;
            Txt_DisposalReason.MaxLength = 255;
            Txt_DisposalValue.MaxLength = 18;
            Txt_Category.MaxLength = 200;
            Txt_Supplier.MaxLength = 200;
            Txt_Warehouse.MaxLength = 200;
            Txt_Employee.MaxLength = 200;
        }

        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_AssetCode.Enabled = false;
            Txt_ResidualValueAct.Enabled = false;
            Txt_Category.Enabled = false;
            Txt_Supplier.Enabled = false;
            Txt_Warehouse.Enabled = false;
            Txt_Employee.Enabled = false;
            Txt_WarrantyDocumentPath.Enabled = false;
        }

        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR POR CÓDIGO, NOMBRE O SERIE...");
            ConfigurarPlaceHolder(Txt_AssetCode, "CÓDIGO DEL ACTIVO");
            ConfigurarPlaceHolder(Txt_AssetName, "NOMBRE DEL ACTIVO");
            ConfigurarPlaceHolder(Txt_Description, "DESCRIPCIÓN");
            ConfigurarPlaceHolder(Txt_Brand, "MARCA");
            ConfigurarPlaceHolder(Txt_Model, "MODELO");
            ConfigurarPlaceHolder(Txt_Serial, "NÚMERO DE SERIE");
            ConfigurarPlaceHolder(Txt_PurchaseValue, "0.00");
            ConfigurarPlaceHolder(Txt_ResidualValue, "0.00");
            ConfigurarPlaceHolder(Txt_ResidualValueAct, "0.00");
            ConfigurarPlaceHolder(Txt_InvoiceNumber, "NÚMERO DE FACTURA");
            ConfigurarPlaceHolder(Txt_WarrantyDocumentPath, "RUTA DEL DOCUMENTO");
            ConfigurarPlaceHolder(Txt_Notes, "NOTAS");
            ConfigurarPlaceHolder(Txt_DisposalReason, "MOTIVO DE BAJA");
            ConfigurarPlaceHolder(Txt_DisposalValue, "0.00");
            ConfigurarPlaceHolder(Txt_Category, "SELECCIONAR CATEGORÍA");
            ConfigurarPlaceHolder(Txt_Supplier, "SELECCIONAR PROVEEDOR");
            ConfigurarPlaceHolder(Txt_Warehouse, "SELECCIONAR BODEGA");
            ConfigurarPlaceHolder(Txt_Employee, "SELECCIONAR EMPLEADO");
        }

        private void ConfigurarPlaceHolder(TextBox textBox, string placeholder)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder && textBox.ForeColor == Color.Gray)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        #endregion ConfigurarTextBox

        #region Filtros

        private void ConfigurarFiltros()
        {
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            FiltroEstado.DropDownStyle = ComboBoxStyle.DropDownList;

            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.Items.Add("POR NOMBRE");
            Filtro1.Items.Add("POR SERIE");
            Filtro1.SelectedIndex = 0;

            FiltroEstado.Items.Clear();
            FiltroEstado.Items.Add("TODOS");
            FiltroEstado.Items.Add("SOLO ACTIVOS");
            FiltroEstado.Items.Add("SOLO INACTIVOS");
            FiltroEstado.SelectedIndex = 1;
        }

        #endregion Filtros

        #region ConfigurarCombos

        private void ConfigurarCombos()
        {
            ComboBox_AssetStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_AssetStatus.Items.Clear();
            ComboBox_AssetStatus.Items.Add("ACTIVO");
            ComboBox_AssetStatus.Items.Add("MANTENIMIENTO");
            ComboBox_AssetStatus.Items.Add("BAJA");
            ComboBox_AssetStatus.Items.Add("TRASLADO");
            ComboBox_AssetStatus.Items.Add("EXTRAVIADO");
            ComboBox_AssetStatus.SelectedIndex = 0;
            
            ComboBox_AssetStatus.SelectedIndexChanged += ComboBox_AssetStatus_SelectedIndexChanged;
            ActualizarCamposBaja();
        }

        #endregion ConfigurarCombos

        #region ConfiguracionesTabla

        private void CargarActivos()
        {
            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
        }

        private void RefrescarListado()
        {
            _assetsList = Ctrl_FixedAssets.MostrarActivos(paginaActual, registrosPorPagina);
            AsignarDataSourceActivos();
        }

        private void ConfigurarTabla()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["AssetCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["AssetName"].HeaderText = "NOMBRE";
                Tabla.Columns["CategoryName"].HeaderText = "CATEGORÍA";
                Tabla.Columns["Brand"].HeaderText = "MARCA";
                Tabla.Columns["Model"].HeaderText = "MODELO";
                Tabla.Columns["Serial"].HeaderText = "SERIE";
                Tabla.Columns["AssetStatus"].HeaderText = "ESTADO";
                Tabla.Columns["WarehouseName"].HeaderText = "BODEGA";
                Tabla.Columns["EmployeeName"].HeaderText = "ASIGNADO A";
                Tabla.Columns["PurchaseValue"].HeaderText = "VALOR COMPRA";
                Tabla.Columns["IsActive"].HeaderText = "ACTIVO";

                foreach (DataGridViewColumn col in Tabla.Columns)
                    col.Visible = false;

                Tabla.Columns["AssetCode"].Visible = true;
                Tabla.Columns["AssetName"].Visible = true;
                Tabla.Columns["CategoryName"].Visible = true;
                Tabla.Columns["Brand"].Visible = true;
                Tabla.Columns["Model"].Visible = true;
                Tabla.Columns["Serial"].Visible = true;
                Tabla.Columns["AssetStatus"].Visible = true;
                Tabla.Columns["WarehouseName"].Visible = true;
                Tabla.Columns["EmployeeName"].Visible = true;
                Tabla.Columns["PurchaseValue"].Visible = true;
                Tabla.Columns["IsActive"].Visible = true;

                Tabla.Columns["IsActive"].DisplayIndex = Tabla.Columns.Count - 1;
            }

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToDeleteRows = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            Tabla.RowTemplate.Height = 35;
            Tabla.ColumnHeadersHeight = 40;


            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;

            if (Tabla.Rows.Count > 0)
                Tabla.Rows[0].Selected = true;
        }

        private void AjustarColumnas()
        {
            if (Tabla.Columns.Count == 0) return;

            SetFillWeight(Tabla, "AssetCode", 8);
            SetFillWeight(Tabla, "AssetName", 20);
            SetFillWeight(Tabla, "CategoryName", 13);
            SetFillWeight(Tabla, "Brand", 8);
            SetFillWeight(Tabla, "Model", 8);
            SetFillWeight(Tabla, "Serial", 10);
            SetFillWeight(Tabla, "AssetStatus", 9);
            SetFillWeight(Tabla, "WarehouseName", 10);
            SetFillWeight(Tabla, "EmployeeName", 10);
            SetFillWeight(Tabla, "PurchaseValue", 8);
            SetFillWeight(Tabla, "IsActive", 6);
        }

        private void SetFillWeight(DataGridView grid, string col, int weight)
        {
            if (!grid.Columns.Contains(col)) return;
            grid.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns[col].FillWeight = weight;
        }

        private void AsignarDataSourceActivos()
        {
            if (_assetsList == null) { Tabla.DataSource = null; return; }

            var data = _assetsList.Select(a => new
            {
                a.AssetId,
                a.AssetCode,
                a.AssetName,
                a.AssetCategoryId,
                CategoryName = a.CategoryName ?? "",
                a.Brand,
                a.Model,
                a.Serial,
                a.PurchaseDate,
                a.PurchaseValue,
                a.ResidualValue,
                a.InvoiceNumber,
                a.SupplierId,
                SupplierName = a.SupplierName ?? "",
                a.WarrantyDocumentPath,
                a.WarrantyExpirationDate,
                a.DepreciationStartDate,
                a.ResidualValueAct,
                a.CurrentWarehouseId,
                WarehouseName = a.WarehouseName ?? "",
                a.AssignedToEmployeeId,
                EmployeeName = a.EmployeeName ?? "",
                a.AssetStatus,
                a.DisposalDate,
                a.DisposalReason,
                a.DisposalValue,
                a.Notes,
                IsActive = a.IsActive ? "ACTIVO" : "INACTIVO",
                a.CreatedDate,
                a.CreatedBy,
                a.ModifiedDate,
                a.ModifiedBy
            }).ToList();

            Tabla.DataSource = data;
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count > 0)
                CargarDatosActivoSeleccionado();
        }

        private void CargarDatosActivoSeleccionado()
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0) return;

                var row = Tabla.SelectedRows[0];
                if (!Tabla.Columns.Contains("AssetId")) return;

                object cellValue = row.Cells["AssetId"].Value;
                if (cellValue == null || cellValue == DBNull.Value) return;

                int assetId = Convert.ToInt32(cellValue);
                _activoSeleccionado = _assetsList?.FirstOrDefault(a => a.AssetId == assetId);
                if (_activoSeleccionado == null) return;

                SetTextBoxFromValue(Txt_AssetCode, _activoSeleccionado.AssetCode, "CÓDIGO DEL ACTIVO");
                SetTextBoxFromValue(Txt_AssetName, _activoSeleccionado.AssetName, "NOMBRE DEL ACTIVO");
                SetTextBoxFromValue(Txt_Description, _activoSeleccionado.Description, "DESCRIPCIÓN");

                _categoriaSeleccionadaId = _activoSeleccionado.AssetCategoryId;
                _categoriaAnteriorId = _activoSeleccionado.AssetCategoryId;
                SetTextBoxFromValue(Txt_Category, _activoSeleccionado.CategoryName, "SELECCIONAR CATEGORÍA");

                SetTextBoxFromValue(Txt_Brand, _activoSeleccionado.Brand, "MARCA");
                SetTextBoxFromValue(Txt_Model, _activoSeleccionado.Model, "MODELO");
                SetTextBoxFromValue(Txt_Serial, _activoSeleccionado.Serial, "NÚMERO DE SERIE");

                DTP_PurchaseDate.Checked = _activoSeleccionado.PurchaseDate.HasValue;
                if (_activoSeleccionado.PurchaseDate.HasValue)
                    DTP_PurchaseDate.Value = _activoSeleccionado.PurchaseDate.Value;

                SetTextBoxFromValue(Txt_PurchaseValue, _activoSeleccionado.PurchaseValue.ToString("0.00"), "0.00");
                SetTextBoxFromValue(Txt_ResidualValue, _activoSeleccionado.ResidualValue.ToString("0.00"), "0.00");
                SetTextBoxFromValue(Txt_ResidualValueAct, _activoSeleccionado.ResidualValueAct.ToString("0.00"), "0.00");
                SetTextBoxFromValue(Txt_InvoiceNumber, _activoSeleccionado.InvoiceNumber, "NÚMERO DE FACTURA");

                _proveedorSeleccionadoId = _activoSeleccionado.SupplierId;
                SetTextBoxFromValue(Txt_Supplier, _activoSeleccionado.SupplierName, "SELECCIONAR PROVEEDOR");

                SetTextBoxFromValue(Txt_WarrantyDocumentPath, _activoSeleccionado.WarrantyDocumentPath, "RUTA DEL DOCUMENTO");
                DTP_WarrantyExpirationDate.Checked = _activoSeleccionado.WarrantyExpirationDate.HasValue;
                if (_activoSeleccionado.WarrantyExpirationDate.HasValue)
                    DTP_WarrantyExpirationDate.Value = _activoSeleccionado.WarrantyExpirationDate.Value;

                DTP_DepreciationStartDate.Checked = _activoSeleccionado.DepreciationStartDate.HasValue;
                if (_activoSeleccionado.DepreciationStartDate.HasValue)
                    DTP_DepreciationStartDate.Value = _activoSeleccionado.DepreciationStartDate.Value;

                _bodegaSeleccionadaId = _activoSeleccionado.CurrentWarehouseId;
                _empleadoSeleccionadoId = _activoSeleccionado.AssignedToEmployeeId;
                SetTextBoxFromValue(Txt_Warehouse, _activoSeleccionado.WarehouseName, "SELECCIONAR BODEGA");
                SetTextBoxFromValue(Txt_Employee, _activoSeleccionado.EmployeeName, "SELECCIONAR EMPLEADO");

                int statusIdx = ComboBox_AssetStatus.Items.IndexOf(_activoSeleccionado.AssetStatus ?? "ACTIVO");
                ComboBox_AssetStatus.SelectedIndex = statusIdx >= 0 ? statusIdx : 0;
                ActualizarCamposBaja();

                SetTextBoxFromValue(Txt_Notes, _activoSeleccionado.Notes, "NOTAS");

                DTP_DisposalDate.Checked = _activoSeleccionado.DisposalDate.HasValue;
                if (_activoSeleccionado.DisposalDate.HasValue)
                    DTP_DisposalDate.Value = _activoSeleccionado.DisposalDate.Value;

                SetTextBoxFromValue(Txt_DisposalReason, _activoSeleccionado.DisposalReason, "MOTIVO DE BAJA");
                SetTextBoxFromValue(Txt_DisposalValue,
                    _activoSeleccionado.DisposalValue.HasValue
                        ? _activoSeleccionado.DisposalValue.Value.ToString("0.00") : "",
                    "0.00");

                CargarAtributosDelActivoSeleccionado(_activoSeleccionado.AssetId, _activoSeleccionado.AssetCategoryId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del activo: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetTextBoxFromValue(TextBox txt, string value, string placeholder)
        {
            if (!string.IsNullOrWhiteSpace(value) && value != placeholder)
            { txt.Text = value; txt.ForeColor = Color.Black; }
            else
            { txt.Text = placeholder; txt.ForeColor = Color.Gray; }
        }

        #endregion ConfiguracionesTabla

        #region Search

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            if (!Btn_Search.Enabled) return;
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string valorBusqueda = TienePlaceholder(Txt_ValorBuscado, "BUSCAR POR CÓDIGO, NOMBRE O SERIE...")
                    ? "" : Txt_ValorBuscado.Text.Trim();

                _ultimoTextoBusqueda = valorBusqueda;
                _ultimoFiltro1 = Filtro1.SelectedItem?.ToString() ?? "TODOS";
                _ultimoFiltroEstado = FiltroEstado.SelectedItem?.ToString() ?? "TODOS";

                paginaActual = 1;

                _assetsList = Ctrl_FixedAssets.BuscarActivos(
                    valorBusqueda, _ultimoFiltro1, _ultimoFiltroEstado,
                    _ultimoCategoriaId, paginaActual, registrosPorPagina);

                AsignarDataSourceActivos();
                ConfigurarTabla();
                AjustarColumnas();

                if (Tabla.Rows.Count > 0)
                    CargarDatosActivoSeleccionado();

                totalRegistros = Ctrl_FixedAssets.ContarTotalActivos(valorBusqueda, _ultimoCategoriaId);
                ActualizarInfoPaginacion();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_Search_Click(sender, e); }
        }

        #endregion Search

        #region ToolStrip_Paginacion

        private void CrearToolStripPaginacion()
        {
            toolStripPaginacion = new ToolStrip();
            toolStripPaginacion.Dock = DockStyle.None;
            toolStripPaginacion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            toolStripPaginacion.GripStyle = ToolStripGripStyle.Hidden;
            toolStripPaginacion.BackColor = Color.FromArgb(248, 249, 250);
            toolStripPaginacion.AutoSize = true;
            toolStripPaginacion.Location = new Point(this.Width - 400, 170);

            btnAnterior = new ToolStripButton { Text = "❮ Anterior" };
            btnAnterior.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnterior.ForeColor = Color.White;
            btnAnterior.BackColor = Color.FromArgb(51, 140, 255);
            btnAnterior.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAnterior.Margin = new Padding(2);
            btnAnterior.Padding = new Padding(8, 4, 8, 4);
            btnAnterior.Click += (s, ev) => CambiarPagina(paginaActual - 1);
            toolStripPaginacion.Items.Add(btnAnterior);

            btnSiguiente = new ToolStripButton { Text = "Siguiente ❯" };
            btnSiguiente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSiguiente.ForeColor = Color.White;
            btnSiguiente.BackColor = Color.FromArgb(238, 143, 109);
            btnSiguiente.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSiguiente.Margin = new Padding(2);
            btnSiguiente.Padding = new Padding(8, 4, 8, 4);
            btnSiguiente.Click += (s, ev) => CambiarPagina(paginaActual + 1);
            toolStripPaginacion.Items.Add(btnSiguiente);

            this.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.BringToFront();
        }

        private void ActualizarBotonesNumerados()
        {
            var toRemove = toolStripPaginacion.Items.Cast<ToolStripItem>()
                .Where(i => i.Tag?.ToString() == "PageButton").ToList();
            foreach (var i in toRemove)
                toolStripPaginacion.Items.Remove(i);

            if (totalPaginas <= 1) return;

            int inicio = Math.Max(1, paginaActual - 1);
            int fin = Math.Min(totalPaginas, paginaActual + 1);
            int posicion = toolStripPaginacion.Items.IndexOf(btnSiguiente);

            for (int i = inicio; i <= fin; i++)
            {
                var btn = new ToolStripButton { Text = i.ToString(), Tag = "PageButton" };
                btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btn.Margin = new Padding(1);
                btn.Padding = new Padding(6, 4, 6, 4);
                btn.BackColor = i == paginaActual ? Color.FromArgb(238, 143, 109) : Color.FromArgb(240, 240, 240);
                btn.ForeColor = i == paginaActual ? Color.White : Color.FromArgb(51, 140, 255);
                int num = i;
                btn.Click += (s, ev) => CambiarPagina(num);
                toolStripPaginacion.Items.Insert(posicion++, btn);
            }
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1 || nuevaPagina > totalPaginas) return;
            paginaActual = nuevaPagina;

            if (!string.IsNullOrEmpty(_ultimoTextoBusqueda))
            {
                _assetsList = Ctrl_FixedAssets.BuscarActivos(
                    _ultimoTextoBusqueda, _ultimoFiltro1, _ultimoFiltroEstado,
                    _ultimoCategoriaId, paginaActual, registrosPorPagina);
                AsignarDataSourceActivos();
                ConfigurarTabla();
                AjustarColumnas();
            }
            else
            {
                RefrescarListado();
                ConfigurarTabla();
                AjustarColumnas();
            }

            if (Tabla.Rows.Count > 0)
                CargarDatosActivoSeleccionado();

            ActualizarInfoPaginacion();
        }

        private void ActualizarInfoPaginacion()
        {
            if (totalRegistros == 0)
                totalRegistros = Ctrl_FixedAssets.ContarTotalActivos(_ultimoTextoBusqueda, _ultimoCategoriaId);

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;
            ActualizarBotonesNumerados();

            int inicio = (paginaActual - 1) * registrosPorPagina + 1;
            int fin = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            Lbl_Paginas.Text = totalRegistros == 0
                ? "NO HAY ACTIVOS PARA MOSTRAR"
                : $"MOSTRANDO {inicio}-{fin} DE {totalRegistros} ACTIVOS";
        }

        #endregion ToolStrip_Paginacion

        #region AsignacionFocus

        private void ConfigurarTabIndexYFocus()
        {
            Txt_ValorBuscado.TabIndex = 0;
            Filtro1.TabIndex = 1;
            FiltroEstado.TabIndex = 2;

            Txt_AssetCode.TabIndex = 5;
            Txt_AssetName.TabIndex = 6;
            Txt_Description.TabIndex = 7;
            Btn_SearchCategory.TabIndex = 8;

            Txt_Brand.TabIndex = 9;
            Txt_Model.TabIndex = 10;
            Txt_Serial.TabIndex = 11;

            DTP_PurchaseDate.TabIndex = 12;
            Txt_PurchaseValue.TabIndex = 13;
            Txt_ResidualValue.TabIndex = 14;
            Txt_InvoiceNumber.TabIndex = 15;
            Btn_SearchSupplier.TabIndex = 16;

            Btn_SelectWarrantyDoc.TabIndex = 17;
            DTP_WarrantyExpirationDate.TabIndex = 18;

            DTP_DepreciationStartDate.TabIndex = 19;

            Btn_SearchWarehouse.TabIndex = 20;
            Btn_SearchEmployee.TabIndex = 21;

            ComboBox_AssetStatus.TabIndex = 22;
            Txt_Notes.TabIndex = 23;

            DTP_DisposalDate.TabIndex = 24;
            Txt_DisposalReason.TabIndex = 25;
            Txt_DisposalValue.TabIndex = 26;

            Btn_Save.TabIndex = 27;
            Btn_Update.TabIndex = 28;
            Btn_Inactive.TabIndex = 29;
            Btn_Clear.TabIndex = 30;

            Txt_ValorBuscado.Focus();
        }

        private void ComboBox_AssetStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarCamposBaja();
        }

        private void ActualizarCamposBaja()
        {
            bool esBaja = ComboBox_AssetStatus.SelectedItem?.ToString() == "BAJA";

            DTP_DisposalDate.Enabled = esBaja;
            Txt_DisposalReason.Enabled = esBaja;
            Txt_DisposalValue.Enabled = esBaja;

            if (!esBaja)
            {
                DTP_DisposalDate.Checked = false;
                SetTextBoxFromValue(Txt_DisposalReason, "", "MOTIVO DE BAJA");
                SetTextBoxFromValue(Txt_DisposalValue, "", "0.00");
            }
        }

        #endregion AsignacionFocus

        #region Validaciones

        private bool TienePlaceholder(TextBox txt, string placeholder)
        {
            return string.IsNullOrWhiteSpace(txt.Text) ||
                   txt.Text == placeholder ||
                   txt.ForeColor == Color.Gray;
        }

        private bool ValidarCamposObligatorios()
        {
            if (TienePlaceholder(Txt_AssetName, "NOMBRE DEL ACTIVO"))
            {
                MessageBox.Show("El campo NOMBRE DEL ACTIVO es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_AssetName.Focus(); return false;
            }

            if (!_categoriaSeleccionadaId.HasValue || _categoriaSeleccionadaId <= 0)
            {
                MessageBox.Show("Debe seleccionar una CATEGORÍA.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Btn_SearchCategory.Focus(); return false;
            }

            if (TienePlaceholder(Txt_PurchaseValue, "0.00") ||
                !decimal.TryParse(Txt_PurchaseValue.Text.Trim(), out _))
            {
                MessageBox.Show("Ingrese un VALOR DE COMPRA válido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_PurchaseValue.Focus(); return false;
            }

            if (ComboBox_AssetStatus.SelectedItem?.ToString() == "BAJA")
            {
                if (!DTP_DisposalDate.Checked)
                {
                    MessageBox.Show("Debe ingresar la FECHA DE BAJA.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DTP_DisposalDate.Focus(); return false;
                }
                if (TienePlaceholder(Txt_DisposalReason, "MOTIVO DE BAJA"))
                {
                    MessageBox.Show("Debe ingresar el MOTIVO DE BAJA.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_DisposalReason.Focus(); return false;
                }
                if (TienePlaceholder(Txt_DisposalValue, "0.00"))
                {
                    MessageBox.Show("Debe ingresar el VALOR DE BAJA.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_DisposalValue.Focus(); return false;
                }
            }

            return true;
        }

        private decimal ObtenerDecimal(TextBox txt, string placeholder)
        {
            if (TienePlaceholder(txt, placeholder)) return 0;
            decimal.TryParse(txt.Text.Trim(), out decimal valor);
            return valor;
        }

        #endregion Validaciones

        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!Btn_Save.Enabled) return;
            try
            {
                if (!ValidarCamposObligatorios()) return;

                if (MessageBox.Show("¿Registrar este activo fijo?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                var nuevo = new Mdl_FixedAsset
                {
                    AssetName = Txt_AssetName.Text.Trim(),
                    Description = TienePlaceholder(Txt_Description, "DESCRIPCIÓN") ? null : Txt_Description.Text.Trim(),
                    AssetCategoryId = _categoriaSeleccionadaId ?? 0,
                    Brand = TienePlaceholder(Txt_Brand, "MARCA") ? null : Txt_Brand.Text.Trim(),
                    Model = TienePlaceholder(Txt_Model, "MODELO") ? null : Txt_Model.Text.Trim(),
                    Serial = TienePlaceholder(Txt_Serial, "NÚMERO DE SERIE") ? null : Txt_Serial.Text.Trim(),
                    PurchaseDate = DTP_PurchaseDate.Checked ? DTP_PurchaseDate.Value.Date : (DateTime?)null,
                    PurchaseValue = ObtenerDecimal(Txt_PurchaseValue, "0.00"),
                    ResidualValue = ObtenerDecimal(Txt_ResidualValue, "0.00"),
                    InvoiceNumber = TienePlaceholder(Txt_InvoiceNumber, "NÚMERO DE FACTURA") ? null : Txt_InvoiceNumber.Text.Trim(),
                    SupplierId = _proveedorSeleccionadoId,
                    WarrantyDocumentPath = TienePlaceholder(Txt_WarrantyDocumentPath, "RUTA DEL DOCUMENTO") ? null : Txt_WarrantyDocumentPath.Text.Trim(),
                    WarrantyExpirationDate = DTP_WarrantyExpirationDate.Checked ? DTP_WarrantyExpirationDate.Value.Date : (DateTime?)null,
                    DepreciationStartDate = DTP_DepreciationStartDate.Checked ? DTP_DepreciationStartDate.Value.Date : (DateTime?)null,
                    CurrentWarehouseId = _bodegaSeleccionadaId,
                    AssignedToEmployeeId = _empleadoSeleccionadoId,
                    AssetStatus = ComboBox_AssetStatus.SelectedItem?.ToString() ?? "ACTIVO",
                    DisposalDate = null,
                    DisposalReason = null,
                    DisposalValue = null,
                    Notes = TienePlaceholder(Txt_Notes, "NOTAS") ? null : Txt_Notes.Text.Trim(),
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy = UserData?.UserId ?? 1
                };

                using (var frmAttr = new Frm_FA_AssetAttributeValues(0, _categoriaSeleccionadaId ?? 0, UserData?.UserId ?? 1))
                {
                    frmAttr.StartPosition = FormStartPosition.CenterParent;

                    if (frmAttr.ShowDialog(this) != DialogResult.OK) return;

                    // Usuario confirmó características — ahora insertar el activo
                    int resultado = Ctrl_FixedAssets.RegistrarActivo(nuevo);

                    switch (resultado)
                    {
                        case int id when id > 0:
                            frmAttr.GuardarValores(id);
                            MessageBox.Show("Activo registrado exitosamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimpiarFormulario();
                            RefrescarListado();
                            ActualizarInfoPaginacion();
                            break;
                        case -1:
                            MessageBox.Show("Ya existe un activo con ese código.", "Duplicado",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        case -2:
                            MessageBox.Show("La categoría seleccionada no es válida.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        default:
                            MessageBox.Show("No se pudo registrar el activo.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (!Btn_Update.Enabled) return;
            try
            {
                if (_activoSeleccionado == null || _activoSeleccionado.AssetId == 0)
                {
                    MessageBox.Show("Debe seleccionar un activo para actualizar.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                }

                if (!ValidarCamposObligatorios()) return;

                // Alerta si cambió la categoría
                bool cambioCategoría = _categoriaSeleccionadaId.HasValue &&
                                       _categoriaAnteriorId.HasValue &&
                                       _categoriaSeleccionadaId != _categoriaAnteriorId;

                if (cambioCategoría)
                {
                    if (MessageBox.Show(
                        "Ha cambiado la CATEGORÍA del activo.\n\n" +
                        "Las características actuales del activo se ELIMINARÁN y deberá ingresar " +
                        "nuevas características correspondientes a la nueva categoría.\n\n" +
                        "¿Desea continuar?",
                        "Advertencia — Cambio de Categoría",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                }
                else
                {
                    if (MessageBox.Show($"¿Actualizar el activo {_activoSeleccionado.AssetName}?",
                        "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                }

                _activoSeleccionado.AssetName = Txt_AssetName.Text.Trim();
                _activoSeleccionado.Description = TienePlaceholder(Txt_Description, "DESCRIPCIÓN") ? null : Txt_Description.Text.Trim();
                _activoSeleccionado.AssetCategoryId = _categoriaSeleccionadaId ?? _activoSeleccionado.AssetCategoryId;
                _activoSeleccionado.Brand = TienePlaceholder(Txt_Brand, "MARCA") ? null : Txt_Brand.Text.Trim();
                _activoSeleccionado.Model = TienePlaceholder(Txt_Model, "MODELO") ? null : Txt_Model.Text.Trim();
                _activoSeleccionado.Serial = TienePlaceholder(Txt_Serial, "NÚMERO DE SERIE") ? null : Txt_Serial.Text.Trim();
                _activoSeleccionado.PurchaseDate = DTP_PurchaseDate.Value.Date;
                _activoSeleccionado.PurchaseValue = ObtenerDecimal(Txt_PurchaseValue, "0.00");
                _activoSeleccionado.ResidualValue = ObtenerDecimal(Txt_ResidualValue, "0.00");
                _activoSeleccionado.InvoiceNumber = TienePlaceholder(Txt_InvoiceNumber, "NÚMERO DE FACTURA") ? null : Txt_InvoiceNumber.Text.Trim();
                _activoSeleccionado.SupplierId = _proveedorSeleccionadoId;
                _activoSeleccionado.WarrantyDocumentPath = TienePlaceholder(Txt_WarrantyDocumentPath, "RUTA DEL DOCUMENTO") ? null : Txt_WarrantyDocumentPath.Text.Trim();
                _activoSeleccionado.WarrantyExpirationDate = DTP_WarrantyExpirationDate.Value.Date;
                _activoSeleccionado.DepreciationStartDate = DTP_DepreciationStartDate.Value.Date;
                _activoSeleccionado.CurrentWarehouseId = _bodegaSeleccionadaId;
                _activoSeleccionado.AssignedToEmployeeId = _empleadoSeleccionadoId;
                _activoSeleccionado.AssetStatus = ComboBox_AssetStatus.SelectedItem?.ToString() ?? "ACTIVO";

                string estado = ComboBox_AssetStatus.SelectedItem?.ToString() ?? "ACTIVO";
                _activoSeleccionado.DisposalDate = estado == "BAJA" ? DTP_DisposalDate.Value.Date : (DateTime?)null;
                _activoSeleccionado.DisposalReason = estado == "BAJA" && !TienePlaceholder(Txt_DisposalReason, "MOTIVO DE BAJA") ? Txt_DisposalReason.Text.Trim() : null;
                _activoSeleccionado.DisposalValue = estado == "BAJA" && !TienePlaceholder(Txt_DisposalValue, "0.00") ? ObtenerDecimal(Txt_DisposalValue, "0.00") : (decimal?)null;

                _activoSeleccionado.DisposalReason = TienePlaceholder(Txt_DisposalReason, "MOTIVO DE BAJA") ? null : Txt_DisposalReason.Text.Trim();
                _activoSeleccionado.Notes = TienePlaceholder(Txt_Notes, "NOTAS") ? null : Txt_Notes.Text.Trim();
                _activoSeleccionado.ModifiedDate = DateTime.Now;
                _activoSeleccionado.ModifiedBy = UserData?.UserId ?? 1;


                int resultado = Ctrl_FixedAssets.ActualizarActivo(_activoSeleccionado);

                switch (resultado)
                {
                    case 1:
                        if (cambioCategoría)
                            AbrirVentanaCaracteristicas(_activoSeleccionado.AssetId, _categoriaSeleccionadaId ?? 0, UserData?.UserId ?? 1);

                        MessageBox.Show("Activo actualizado exitosamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        RefrescarListado();
                        ActualizarInfoPaginacion();
                        break;
                    case -1:
                        MessageBox.Show("El activo no fue encontrado.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case -2:
                        MessageBox.Show("Ya existe otro activo con ese código.", "Duplicado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case -3:
                        MessageBox.Show("La categoría seleccionada no es válida.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("No se pudo actualizar el activo.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            if (!Btn_Inactive.Enabled) return;
            try
            {
                if (_activoSeleccionado == null || _activoSeleccionado.AssetId == 0)
                {
                    MessageBox.Show("Debe seleccionar un activo para inactivar.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                }

                if (!_activoSeleccionado.IsActive)
                {
                    MessageBox.Show("Este activo ya se encuentra inactivo.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information); return;
                }

                if (MessageBox.Show($"¿INACTIVAR el activo {_activoSeleccionado.AssetName}?",
                    "Confirmar Inactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

                _activoSeleccionado.IsActive = false;
                _activoSeleccionado.ModifiedDate = DateTime.Now;
                _activoSeleccionado.ModifiedBy = UserData?.UserId ?? 1;

                int resultado = Ctrl_FixedAssets.ActualizarActivo(_activoSeleccionado);

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show("Activo inactivado exitosamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        RefrescarListado();
                        ActualizarInfoPaginacion();
                        break;
                    case -1:
                        MessageBox.Show("El activo no fue encontrado.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("No se pudo inactivar el activo.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inactivar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            if (!Btn_Clear.Enabled) return;
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            Tabla.ClearSelection();
            _activoSeleccionado = null;
            _categoriaSeleccionadaId = null;
            _categoriaAnteriorId = null;
            _proveedorSeleccionadoId = null;
            _bodegaSeleccionadaId = null;
            _empleadoSeleccionadoId = null;

            ConfigurarPlaceHoldersTextbox();
            ConfigurarCombos();
            DTP_PurchaseDate.Value = DateTime.Today;
            DTP_WarrantyExpirationDate.Value = DateTime.Today;
            DTP_DepreciationStartDate.Value = DateTime.Today;
            DTP_DisposalDate.Value = DateTime.Today;
            LimpiarTablaAtributos();

            Txt_AssetName.Focus();
        }

        #endregion CRUD

        #region VentanaCaracteristicas

        private void AbrirVentanaCaracteristicas(int assetId, int categoryId, int userId)
        {
            try
            {
                using (var frm = new Frm_FA_AssetAttributeValues(assetId, categoryId, userId))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir características: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion VentanaCaracteristicas

        #region BuscarEntidades

        private void Btn_SearchCategory_Click(object sender, EventArgs e)
        {
            if (!Btn_SearchCategory.Enabled) return;
            try
            {
                using (var frm = new Frm_FA_SearchCategory(this))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar categoría: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ActualizarCategoria(int categoryId, string categoryName)
        {
            // Si ya había una categoría distinta, alertar
            if (_categoriaAnteriorId.HasValue &&
                _categoriaAnteriorId > 0 &&
                _categoriaAnteriorId != categoryId &&
                _activoSeleccionado != null)
            {
                if (MessageBox.Show(
                    "Está cambiando la CATEGORÍA del activo.\n\n" +
                    "Las características actuales se ELIMINARÁN y deberá ingresar nuevas.\n\n" +
                    "¿Desea continuar?",
                    "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }

            _categoriaSeleccionadaId = categoryId;
            SetTextBoxFromValue(Txt_Category, categoryName, "SELECCIONAR CATEGORÍA");
        }

        private void Btn_SearchSupplier_Click(object sender, EventArgs e)
        {
            if (!Btn_SearchSupplier.Enabled) return;
            try
            {
                using (var frm = new Frm_FA_SearchSupplier(this))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar proveedor: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ActualizarProveedor(int supplierId, string supplierName)
        {
            _proveedorSeleccionadoId = supplierId;
            SetTextBoxFromValue(Txt_Supplier, supplierName, "SELECCIONAR PROVEEDOR");
        }

        private void Btn_SearchWarehouse_Click(object sender, EventArgs e)
        {
            if (!Btn_SearchWarehouse.Enabled) return;
            try
            {
                using (var frm = new Frm_FA_SearchWarehouse(this))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar bodega: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ActualizarBodega(int warehouseId, string warehouseName)
        {
            _bodegaSeleccionadaId = warehouseId;
            SetTextBoxFromValue(Txt_Warehouse, warehouseName, "SELECCIONAR BODEGA");
        }

        private void Btn_SearchEmployee_Click(object sender, EventArgs e)
        {
            if (!Btn_SearchEmployee.Enabled) return;
            try
            {
                using (var frm = new Frm_FA_SearchEmployee(this))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar empleado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ActualizarEmpleado(int employeeId, string employeeName)
        {
            _empleadoSeleccionadoId = employeeId;
            SetTextBoxFromValue(Txt_Employee, employeeName, "SELECCIONAR EMPLEADO");
        }

        #endregion BuscarEntidades

        #region SeleccionDocumento

        private void Btn_SelectWarrantyDoc_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dlg = new OpenFileDialog())
                {
                    dlg.Title = "Seleccionar Documento de Garantía";
                    dlg.Filter = "Documentos|*.pdf;*.doc;*.docx;*.jpg;*.png|Todos los archivos|*.*";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        Txt_WarrantyDocumentPath.Text = dlg.FileName;
                        Txt_WarrantyDocumentPath.ForeColor = Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al seleccionar documento: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion SeleccionDocumento

        #region SistemaDePermisos

        private Ctrl_Security_Auth authController;
        private HashSet<string> permisosUsuario = new HashSet<string>();

        protected virtual async Task CargarPermisosUsuario(int userId, int roleId)
        {
            try
            {
                authController = new Ctrl_Security_Auth();
                var permisos = await authController.ObtenerPermisosUsuarioAsync(userId, roleId);
                permisosUsuario = permisos != null
                    ? new HashSet<string>(permisos, StringComparer.OrdinalIgnoreCase)
                    : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                permisosUsuario = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                MessageBox.Show($"ERROR AL CARGAR PERMISOS: {ex.Message}", "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected bool TienePermiso(string permissionCode)
        {
            return !string.IsNullOrWhiteSpace(permissionCode) &&
                   permisosUsuario != null &&
                   permisosUsuario.Contains(permissionCode);
        }

        protected void AplicarEstadoBotonPorPermiso(Button boton, string permissionCode)
        {
            if (boton == null) return;
            bool habilitado = TienePermiso(permissionCode);
            boton.Enabled = habilitado;
            if (habilitado)
            { boton.UseVisualStyleBackColor = true; boton.ForeColor = Color.Black; boton.Cursor = Cursors.Default; }
            else
            { boton.BackColor = Color.FromArgb(200, 200, 200); boton.ForeColor = Color.Gray; boton.Cursor = Cursors.No; }
        }

        protected void ConfigurarControlesPorPermisos()
        {
            AplicarEstadoBotonPorPermiso(Btn_Save, "FA_ASSETS_CREATE");
            
            AplicarEstadoBotonPorPermiso(Btn_Update, "FA_ASSETS_UPDATE");
            AplicarEstadoBotonPorPermiso(Btn_UpdateAtributo, "FA_ASSETS_UPDATE");

            AplicarEstadoBotonPorPermiso(Btn_Inactive, "FA_ASSETS_INACTIVE");
            AplicarEstadoBotonPorPermiso(Btn_Export, "FA_ASSETS_EXPORT");
            AplicarEstadoBotonPorPermiso(Btn_Search, "FA_ASSETS_READ");
        }

        #endregion SistemaDePermisos

        #region ConfiguracionesTabla_Atributos

        private void CargarAtributosDelActivoSeleccionado(int assetId, int categoryId)
        {
            try
            {
                var plantilla = Ctrl_FixedAssetAttributeValues.ObtenerPlantillaPorCategoria(categoryId);

                var valoresGuardados = Ctrl_FixedAssetAttributeValues.ObtenerValoresPorActivo(assetId);

                _atributosValoresList = plantilla;
                foreach (var item in _atributosValoresList)
                {
                    var guardado = valoresGuardados.FirstOrDefault(v => v.AttributeDefId == item.AttributeDefId);
                    if (guardado != null)
                    {
                        item.AttributeValueId = guardado.AttributeValueId;
                        item.Value = guardado.Value;
                    }
                }

                AsignarDataSourceAtributos();
                ConfigurarTablaAtributos();
                AjustarColumnasAtributos();

                int conValor = _atributosValoresList.Count(a => !string.IsNullOrWhiteSpace(a.Value));
                Lbl_AtributosHeader.Text =
                    $"CARACTERÍSTICAS DE: {_activoSeleccionado.AssetName.ToUpper()} " +
                    $"— {conValor}/{_atributosValoresList.Count} COMPLETADAS";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar atributos: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AsignarDataSourceAtributos()
        {
            if (_atributosValoresList == null || _atributosValoresList.Count == 0)
            {
                TablaAtributos.DataSource = null;
                return;
            }

            var data = _atributosValoresList.Select(a => new
            {
                a.AttributeDefId,
                Etiqueta = a.AttributeLabel,
                Tipo = a.DataType,
                Obligatorio = a.IsRequired ? "SI" : "NO",
                Valor = string.IsNullOrWhiteSpace(a.Value) ? "— SIN VALOR —" : a.Value
            }).ToList();

            TablaAtributos.DataSource = data;
        }

        private void ConfigurarTablaAtributos()
        {
            if (TablaAtributos.Columns.Count > 0)
            {
                if (TablaAtributos.Columns.Contains("AttributeDefId"))
                    TablaAtributos.Columns["AttributeDefId"].Visible = false;

                if (TablaAtributos.Columns.Contains("Etiqueta"))
                    TablaAtributos.Columns["Etiqueta"].HeaderText = "CARACTERÍSTICA";
                if (TablaAtributos.Columns.Contains("Tipo"))
                    TablaAtributos.Columns["Tipo"].HeaderText = "TIPO";
                if (TablaAtributos.Columns.Contains("Obligatorio"))
                    TablaAtributos.Columns["Obligatorio"].HeaderText = "REQUERIDO";
                if (TablaAtributos.Columns.Contains("Valor"))
                    TablaAtributos.Columns["Valor"].HeaderText = "VALOR REGISTRADO";
            }

            TablaAtributos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TablaAtributos.MultiSelect = false;
            TablaAtributos.ReadOnly = true;
            TablaAtributos.AllowUserToResizeRows = false;
            TablaAtributos.AllowUserToAddRows = false;
            TablaAtributos.AllowUserToDeleteRows = false;
            TablaAtributos.RowHeadersVisible = false;

            TablaAtributos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            TablaAtributos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            TablaAtributos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            TablaAtributos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            TablaAtributos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            TablaAtributos.DefaultCellStyle.SelectionForeColor = Color.White;
            TablaAtributos.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            TablaAtributos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            TablaAtributos.RowTemplate.Height = 35;
            TablaAtributos.ColumnHeadersHeight = 40;

            // Colorear filas sin valor en amarillo claro para destacarlas
            TablaAtributos.CellFormatting -= TablaAtributos_CellFormatting;
            TablaAtributos.CellFormatting += TablaAtributos_CellFormatting;
            TablaAtributos.SelectionChanged += TablaAtributos_SelectionChanged;
        }

        private void AjustarColumnasAtributos()
        {
            if (TablaAtributos.Columns.Count == 0) return;

            SetFillWeightAtrib(TablaAtributos, "Etiqueta", 40);
            SetFillWeightAtrib(TablaAtributos, "Tipo", 15);
            SetFillWeightAtrib(TablaAtributos, "Obligatorio", 15);
            SetFillWeightAtrib(TablaAtributos, "Valor", 30);
        }

        private void SetFillWeightAtrib(DataGridView grid, string col, int weight)
        {
            if (!grid.Columns.Contains(col)) return;
            grid.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns[col].FillWeight = weight;
        }

        private void TablaAtributos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = TablaAtributos.Rows[e.RowIndex];

            object valorCell = TablaAtributos.Columns.Contains("Valor")
                ? row.Cells["Valor"].Value
                : null;

            bool sinValor = valorCell == null ||
                            valorCell.ToString() == "— SIN VALOR —" ||
                            string.IsNullOrWhiteSpace(valorCell.ToString());

            if (sinValor && !row.Selected)
                e.CellStyle.BackColor = Color.FromArgb(255, 253, 220);
        }

        private void LimpiarTablaAtributos()
        {
            _atributosValoresList = null;
            TablaAtributos.DataSource = null;
            Lbl_AtributosHeader.Text = "CARACTERÍSTICAS — SELECCIONE UN ACTIVO FIJO";
            Btn_ClearAtributo_Click(null, null);
        }

        private void ConfigurarFiltrosAtributos()
        {
            ConfigurarPlaceHolder(Txt_BuscarAtributo, "BUSCAR CARACTERÍSTICA...");

            FiltroAtributoTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            FiltroAtributoTipo.Items.Clear();
            FiltroAtributoTipo.Items.Add("TODOS");
            FiltroAtributoTipo.Items.Add("TEXTO");
            FiltroAtributoTipo.Items.Add("NUMERO");
            FiltroAtributoTipo.Items.Add("FECHA");
            FiltroAtributoTipo.SelectedIndex = 0;
        }

        private void Btn_SearchAtributo_Click(object sender, EventArgs e)
        {
            if (_atributosValoresList == null) return;

            string texto = TienePlaceholder(Txt_BuscarAtributo, "BUSCAR CARACTERÍSTICA...")
                ? "" : Txt_BuscarAtributo.Text.Trim().ToUpper();
            string filtroTipo = FiltroAtributoTipo.SelectedItem?.ToString() ?? "TODOS";

            var filtrados = _atributosValoresList.AsEnumerable();

            if (!string.IsNullOrEmpty(texto))
                filtrados = filtrados.Where(a =>
                    (a.AttributeLabel?.ToUpper().Contains(texto) == true) ||
                    (a.AttributeKey?.ToUpper().Contains(texto) == true));

            if (filtroTipo != "TODOS")
                filtrados = filtrados.Where(a => a.DataType == filtroTipo);

            var data = filtrados.Select(a => new
            {
                a.AttributeDefId,
                Etiqueta = a.AttributeLabel,
                Tipo = a.DataType,
                Obligatorio = a.IsRequired ? "SÍ" : "NO",
                Valor = string.IsNullOrWhiteSpace(a.Value) ? "— SIN VALOR —" : a.Value
            }).ToList();

            TablaAtributos.DataSource = null;
            TablaAtributos.DataSource = data;
            ConfigurarTablaAtributos();
            AjustarColumnasAtributos();
        }

        private void Txt_BuscarAtributo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_SearchAtributo_Click(sender, e); }
        }

        private void Btn_CleanSearchAtributo_Click(object sender, EventArgs e)
        {
            SetTextBoxFromValue(Txt_BuscarAtributo, "", "BUSCAR CARACTERÍSTICA...");
            FiltroAtributoTipo.SelectedIndex = 0;

            if (_atributosValoresList == null) return;

            AsignarDataSourceAtributos();
            ConfigurarTablaAtributos();
            AjustarColumnasAtributos();
        }

        private void TablaAtributos_SelectionChanged(object sender, EventArgs e)
        {
            if (TablaAtributos.SelectedRows.Count == 0 || _atributosValoresList == null) return;

            int defId = Convert.ToInt32(TablaAtributos.SelectedRows[0].Cells["AttributeDefId"].Value);
            var atrib = _atributosValoresList.FirstOrDefault(a => a.AttributeDefId == defId);
            if (atrib == null) return;

            Txt_AttributeKey.Text = atrib.AttributeKey;
            Txt_AttributeLabel.Text = atrib.AttributeLabel;
            Txt_AttributeType.Text = atrib.DataType;
            Txt_AttributeValue.Text = string.IsNullOrWhiteSpace(atrib.Value) ? "" : atrib.Value;
        }

        private void Btn_UpdateAtributo_Click(object sender, EventArgs e)
        {
            if (_activoSeleccionado == null || _atributosValoresList == null) return;

            try
            {
                int defId = 0;
                if (TablaAtributos.SelectedRows.Count == 0 ||
                    !int.TryParse(TablaAtributos.SelectedRows[0].Cells["AttributeDefId"].Value?.ToString(), out defId))
                {
                    MessageBox.Show("Seleccione una característica de la tabla.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var atrib = _atributosValoresList.FirstOrDefault(a => a.AttributeDefId == defId);
                if (atrib == null) return;

                string valor = Txt_AttributeValue.Text.Trim();

                // Validar obligatorio
                if (atrib.IsRequired && string.IsNullOrWhiteSpace(valor))
                {
                    MessageBox.Show($"El campo '{atrib.AttributeLabel}' es obligatorio.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar tipo si tiene valor
                if (!string.IsNullOrWhiteSpace(valor))
                {
                    switch (atrib.DataType?.ToUpper())
                    {
                        case "NUMBER":
                        case "NUMERO":
                            if (!decimal.TryParse(valor, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out _))
                            {
                                MessageBox.Show($"El campo '{atrib.AttributeLabel}' debe ser un número válido.", "Validación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            break;
                        case "FECHA":
                        case "DATE":
                            if (!DateTime.TryParse(valor, out _))
                            {
                                MessageBox.Show($"El campo '{atrib.AttributeLabel}' debe ser una fecha válida (ej. 31/12/2024).", "Validación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            break;
                    }
                }

                // Confirmación
                if (MessageBox.Show(
                    $"¿Actualizar el valor de '{atrib.AttributeLabel}'?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                atrib.Value = valor.ToUpper();
                atrib.ModifiedBy = UserData?.UserId;
                atrib.ModifiedDate = DateTime.Now;

                int resultado;
                if (atrib.AttributeValueId == 0)
                {
                    atrib.AssetId = _activoSeleccionado.AssetId;
                    atrib.CreatedBy = UserData?.UserId;
                    resultado = Ctrl_FixedAssetAttributeValues.RegistrarValor(atrib);
                    if (resultado > 0) atrib.AttributeValueId = resultado;
                }
                else
                {
                    resultado = Ctrl_FixedAssetAttributeValues.ActualizarValor(atrib);
                }

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show("Característica actualizada exitosamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AsignarDataSourceAtributos();
                        ConfigurarTablaAtributos();
                        AjustarColumnasAtributos();
                        int conValor = _atributosValoresList.Count(a => !string.IsNullOrWhiteSpace(a.Value));
                        Lbl_AtributosHeader.Text =
                            $"CARACTERÍSTICAS DE: {_activoSeleccionado.AssetName.ToUpper()} " +
                            $"— {conValor}/{_atributosValoresList.Count} COMPLETADAS";
                        break;
                    case -1:
                        MessageBox.Show("El registro no fue encontrado.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("No se pudo actualizar la característica.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar característica: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_ClearAtributo_Click(object sender, EventArgs e)
        {
            Txt_AttributeKey.Text = string.Empty;
            Txt_AttributeLabel.Text = string.Empty;
            Txt_AttributeType.Text = string.Empty;
            Txt_AttributeValue.Text = string.Empty;
            TablaAtributos.ClearSelection();
        }

        #endregion ConfiguracionesTabla_Atributos
    }
}