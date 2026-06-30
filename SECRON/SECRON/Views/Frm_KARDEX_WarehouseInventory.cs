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
    public partial class Frm_KARDEX_WarehouseInventory : Form
    {
        #region PropiedadesIniciales

        public Mdl_Security_UserInfo UserData { get; set; }

        // Bodega seleccionada para filtrar inventario
        private int? _bodegaActivaId = null;
        private string _bodegaActivaNombre = "";
        private int? _bodegaCentralId = null;

        // Filtros de búsqueda
        private string _ultimoTextoBusqueda = "";
        private string _ultimoFiltro1 = "TODOS";
        private string _ultimoFiltro2 = "TODAS LAS CLASIFICACIONES";
        private string _ultimoFiltro3 = "TODOS";

        // Selección actual en la grilla
        private List<Mdl_ItemWarehouseStock> _stockList;
        private Mdl_ItemWarehouseStock _stockSeleccionado = null;

        // Permisos contextuales de la bodega activa para el usuario actual
        private bool _puedeRegistrarEnBodega = false;
        private bool _puedeDespacharEnBodega = false;

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        public Frm_KARDEX_WarehouseInventory()
        {
            InitializeComponent();

            this.Resize += (s, e) =>
            {
                if (toolStripPaginacion != null)
                    toolStripPaginacion.Location = new Point(this.Width - 225, 205);
            };
        }

        private async void Frm_KARDEX_WarehouseInventory_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarTabIndexYFocus();
                ConfigurarMaxLengthTextBox();
                ConfigurarComponentesDeshabilitados();
                ConfigurarPlaceHoldersTextbox();
                CrearToolStripPaginacion();
                ConfigurarFiltros();
                CargarBodegaCentralId();

                // CARGAR PERMISOS DEL USUARIO (rol global) - debe ir ANTES del combo de bodegas
                if (UserData != null)
                {
                    await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);

                    ConfigurarControlesPorPermisos();
                }

                // La carga la dispara el combo de bodegas (depende de KARDEX_WAREHOUSE_ADMIN ya cargado)
                CargarComboBodegas();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion PropiedadesIniciales
        #region ConfigurarTextBox

        private void ConfigurarMaxLengthTextBox()
        {
            Txt_ValorBuscado.MaxLength = 100;
            Txt_Codigo.MaxLength = 50;
            Txt_Articulo.MaxLength = 200;
            Txt_Descripcion.MaxLength = 500;
            Txt_CurrentStock.MaxLength = 18;
            Txt_MinimumStock.MaxLength = 18;
            Txt_MaximumStock.MaxLength = 18;
            Txt_ReorderPoint.MaxLength = 18;
            Txt_Category.MaxLength = 100;
            Txt_MeasurementUnits.MaxLength = 50;
        }

        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_Codigo.Enabled = false;
            Txt_Articulo.Enabled = false;
            Txt_Descripcion.Enabled = false;
            Txt_Category.Enabled = false;
            Txt_MeasurementUnits.Enabled = false;
            Txt_CurrentStock.Enabled = false;
            Txt_MovementCounter.Enabled = false;
            Txt_LastMovementDate.Enabled = false;

            DeshabilitarTodo();
        }

        private void DeshabilitarTodo()
        {
            Txt_ValorBuscado.Enabled = false;
            Filtro1.Enabled = false;
            Filtro2.Enabled = false;
            Filtro3.Enabled = false;
            Btn_Search.Enabled = false;
            Btn_CleanSearch.Enabled = false;
            Txt_MinimumStock.Enabled = false;
            Txt_MaximumStock.Enabled = false;
            Txt_ReorderPoint.Enabled = false;
            Btn_Ingreso.Enabled = false;
            Btn_Despacho.Enabled = false;
            Btn_Update.Enabled = false;
            Btn_Clear.Enabled = false;
            Btn_Export.Enabled = false;
        }

        private void HabilitarTodo()
        {
            Txt_ValorBuscado.Enabled = true;
            Filtro1.Enabled = true;
            Filtro2.Enabled = true;
            Filtro3.Enabled = true;
            Btn_Search.Enabled = true;
            Btn_CleanSearch.Enabled = true;
            Btn_Clear.Enabled = true;
            Btn_Export.Enabled = true;
            Txt_MinimumStock.Enabled = true;
            Txt_MaximumStock.Enabled = true;
            Txt_ReorderPoint.Enabled = true;

            AplicarPermisosContextualesDeBodega();
        }

        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR POR CÓDIGO O NOMBRE...");
            ConfigurarPlaceHolder(Txt_Codigo, "CÓDIGO DEL ARTÍCULO");
            ConfigurarPlaceHolder(Txt_Articulo, "NOMBRE DEL ARTÍCULO");
            ConfigurarPlaceHolder(Txt_Descripcion, "DESCRIPCIÓN DEL ARTÍCULO");
            ConfigurarPlaceHolder(Txt_MinimumStock, "0.00");
            ConfigurarPlaceHolder(Txt_MaximumStock, "0.00");
            ConfigurarPlaceHolder(Txt_ReorderPoint, "0.00");
            ConfigurarPlaceHolder(Txt_Category, "CATEGORÍA DEL ARTÍCULO");
            ConfigurarPlaceHolder(Txt_MeasurementUnits, "UNIDAD DE MEDIDA");
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
        #region ConfigurarFiltros

        private void ConfigurarFiltros()
        {
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.Items.Add("POR NOMBRE");
            Filtro1.SelectedIndex = 0;

            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODAS LAS CLASIFICACIONES");
            var clasificaciones = Ctrl_ItemCategories.ObtenerCategoriasParaCombo();
            foreach (var c in clasificaciones)
                Filtro2.Items.Add(new ClasificacionItem(c.Key, c.Value));
            Filtro2.DisplayMember = "Nombre";
            Filtro2.SelectedIndex = 0;

            Filtro3.Items.Clear();
            Filtro3.Items.Add("TODOS");
            Filtro3.Items.Add("STOCK BAJO MÍNIMO");
            Filtro3.Items.Add("STOCK SOBRE MÁXIMO");
            Filtro3.SelectedIndex = 0;
        }

        #endregion ConfigurarFiltros
        #region BodegaCentral

        private void CargarBodegaCentralId()
        {
            _bodegaCentralId = Ctrl_Warehouses.ObtenerBodegaCentralId();
        }

        private bool EsBodegaCentral(int? warehouseId)
        {
            return warehouseId.HasValue && _bodegaCentralId.HasValue && warehouseId.Value == _bodegaCentralId.Value;
        }

        #endregion BodegaCentral
        #region ComboBodegas

        private void CargarComboBodegas()
        {
            ComboBox_Warehouse.SelectedIndexChanged -= ComboBox_Warehouse_SelectedIndexChanged;
            ComboBox_Warehouse.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Warehouse.Items.Clear();
            ComboBox_Warehouse.Items.Add("SELECCIONAR BODEGA...");

            if (UserData != null)
            {
                bool esAdmin = TienePermiso("KARDEX_WAREHOUSE_ADMIN");
                var bodegas = Ctrl_ItemWarehouseStock.ObtenerBodegasDelUsuario(UserData.UserId, esAdmin);
                foreach (var bodega in bodegas)
                    ComboBox_Warehouse.Items.Add(new ClasificacionItem(bodega.Key, bodega.Value));
            }

            ComboBox_Warehouse.DisplayMember = "Nombre";
            ComboBox_Warehouse.SelectedIndex = 0;
            ComboBox_Warehouse.SelectedIndexChanged += ComboBox_Warehouse_SelectedIndexChanged;
        }

        private async void ComboBox_Warehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_Warehouse.SelectedItem is ClasificacionItem bodega))
            {
                _bodegaActivaId = null;
                _bodegaActivaNombre = "";
                DeshabilitarTodo();
                Tabla.DataSource = null;
                if (Lbl_Paginas != null)
                    Lbl_Paginas.Text = "SELECCIONE UNA BODEGA";

                return;
            }

            _bodegaActivaId = bodega.Id;
            _bodegaActivaNombre = bodega.Nombre;

            this.Cursor = Cursors.WaitCursor;

            await CargarPermisosContextualesDeBodega();

            LimpiarFormulario();

            paginaActual = 1;
            totalRegistros = 0;

            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
            ActualizarInfoPaginacion();

            HabilitarTodo();

            this.Cursor = Cursors.Default;
        }

        #endregion ComboBodegas
        #region PermisosContextualesDeBodega

        private Task CargarPermisosContextualesDeBodega()
        {
            if (!_bodegaActivaId.HasValue || UserData == null)
            {
                _puedeRegistrarEnBodega = false;
                _puedeDespacharEnBodega = false;
                return Task.CompletedTask;
            }

            _puedeRegistrarEnBodega = Ctrl_WarehouseManagerPermissions.TienePermisoEnBodega(
                UserData.UserId, _bodegaActivaId.Value, "REGISTRO");

            _puedeDespacharEnBodega = Ctrl_WarehouseManagerPermissions.TienePermisoEnBodega(
                UserData.UserId, _bodegaActivaId.Value, "DESPACHO_EMPLEADO");

            if (!_puedeDespacharEnBodega)
            {
                _puedeDespacharEnBodega = Ctrl_WarehouseManagerPermissions.TienePermisoEnBodega(
                UserData.UserId, _bodegaActivaId.Value, "DESPACHO_BODEGA");
            }

            return Task.CompletedTask;
        }

        private void AplicarPermisosContextualesDeBodega()
        {
            bool esAdmin = TienePermiso("KARDEX_WAREHOUSE_ADMIN");
            bool esCentral = EsBodegaCentral(_bodegaActivaId);

            bool habilitarIngreso = esCentral
                && TienePermiso("KARDEX_WAREHOUSE_REGISTER")
                && (esAdmin || _puedeRegistrarEnBodega);

            bool habilitarOrder = esCentral
                && TienePermiso("KARDEX_WAREHOUSE_DISPATCH")
                && (esAdmin || _puedeDespacharEnBodega);

            bool habilitarDespacho = TienePermiso("KARDEX_WAREHOUSE_DISPATCH")
                && (esAdmin || _puedeDespacharEnBodega);

            bool habilitarUpdate = TienePermiso("KARDEX_WAREHOUSE_UPDATE");

            AplicarEstadoBoton(Btn_Ingreso, habilitarIngreso);
            AplicarEstadoBoton(Btn_Despacho, habilitarDespacho);
            AplicarEstadoBoton(Btn_Update, habilitarUpdate);
        }

        private void AplicarEstadoBoton(Button boton, bool habilitado)
        {
            if (boton == null) return;

            boton.Enabled = habilitado;

            if (!habilitado)
            {
                boton.BackColor = Color.FromArgb(200, 200, 200);
                boton.ForeColor = Color.Gray;
                boton.Cursor = Cursors.No;
            }
            else
            {
                boton.UseVisualStyleBackColor = true;
                boton.ForeColor = Color.Black;
                boton.Cursor = Cursors.Default;
            }
        }

        #endregion PermisosContextualesDeBodega
        #region ConfiguracionesTabla

        private void RefrescarListado()
        {
            if (_bodegaActivaId.HasValue)
            {
                _stockList = Ctrl_ItemWarehouseStock.ObtenerStockPorBodegaConDetalle(_bodegaActivaId.Value);
            }
            else
            {
                _stockList = new List<Mdl_ItemWarehouseStock>();
            }

            AplicarFiltrosEnMemoria();
            AsignarDataSource();
        }

        private void AplicarFiltrosEnMemoria()
        {
            if (_stockList == null) return;

            if (!string.IsNullOrWhiteSpace(_ultimoTextoBusqueda))
            {
                string texto = _ultimoTextoBusqueda.ToUpper();

                if (_ultimoFiltro1 == "POR CÓDIGO")
                    _stockList = _stockList.Where(s => s.ItemCode?.ToUpper().Contains(texto) ?? false).ToList();
                else if (_ultimoFiltro1 == "POR NOMBRE")
                    _stockList = _stockList.Where(s => s.ItemName?.ToUpper().Contains(texto) ?? false).ToList();
                else
                    _stockList = _stockList.Where(s =>
                        (s.ItemCode?.ToUpper().Contains(texto) ?? false) ||
                        (s.ItemName?.ToUpper().Contains(texto) ?? false)).ToList();
            }

            if (_ultimoFiltro2 != "TODAS LAS CLASIFICACIONES")
                _stockList = _stockList.Where(s => s.CategoryName == _ultimoFiltro2).ToList();

            if (_ultimoFiltro3 == "STOCK BAJO MÍNIMO")
                _stockList = _stockList.Where(s => s.MinimumStock > 0 && s.CurrentStock <= s.MinimumStock).ToList();
            else if (_ultimoFiltro3 == "STOCK SOBRE MÁXIMO")
                _stockList = _stockList.Where(s => s.MaximumStock > 0 && s.CurrentStock > s.MaximumStock).ToList();

            totalRegistros = _stockList.Count;
        }

        private void AsignarDataSource()
        {
            if (_stockList == null)
            {
                Tabla.DataSource = null;
                return;
            }

            var paginado = _stockList
                .Skip((paginaActual - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            var data = paginado.Select(s => new
            {
                s.ItemWarehouseStockId,
                s.ItemId,
                s.WarehouseId,
                s.ItemCode,
                s.ItemName,
                s.CurrentStock,
                s.MinimumStock,
                s.MaximumStock,
                s.ReorderPoint,
                s.MovementCounter,
                s.LastMovementDate
            }).ToList();

            Tabla.DataSource = data;
        }

        private void ConfigurarTabla()
        {
            if (Tabla.Columns.Count > 0)
            {
                if (Tabla.Columns.Contains("ItemWarehouseStockId"))
                    Tabla.Columns["ItemWarehouseStockId"].Visible = false;
                if (Tabla.Columns.Contains("ItemId"))
                    Tabla.Columns["ItemId"].Visible = false;
                if (Tabla.Columns.Contains("WarehouseId"))
                    Tabla.Columns["WarehouseId"].Visible = false;

                if (Tabla.Columns.Contains("ItemCode"))
                    Tabla.Columns["ItemCode"].HeaderText = "CÓDIGO";
                if (Tabla.Columns.Contains("ItemName"))
                    Tabla.Columns["ItemName"].HeaderText = "ARTÍCULO";
                if (Tabla.Columns.Contains("CurrentStock"))
                    Tabla.Columns["CurrentStock"].HeaderText = "STOCK ACTUAL";
                if (Tabla.Columns.Contains("MinimumStock"))
                    Tabla.Columns["MinimumStock"].HeaderText = "STOCK MÍNIMO";
                if (Tabla.Columns.Contains("MaximumStock"))
                    Tabla.Columns["MaximumStock"].HeaderText = "STOCK MÁXIMO";
                if (Tabla.Columns.Contains("ReorderPoint"))
                    Tabla.Columns["ReorderPoint"].HeaderText = "PUNTO DE REORDEN";
                if (Tabla.Columns.Contains("MovementCounter"))
                    Tabla.Columns["MovementCounter"].HeaderText = "MOVIMIENTOS";
                if (Tabla.Columns.Contains("LastMovementDate"))
                    Tabla.Columns["LastMovementDate"].HeaderText = "ÚLT. MOVIMIENTO";
            }

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToResizeRows = false;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void AjustarColumnas()
        {
            if (Tabla.Columns.Count == 0) return;

            void SetFill(string col, float weight)
            {
                if (Tabla.Columns.Contains(col))
                {
                    Tabla.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    Tabla.Columns[col].FillWeight = weight;
                }
            }

            SetFill("ItemCode", 10);
            SetFill("ItemName", 26);
            SetFill("CurrentStock", 10);
            SetFill("MinimumStock", 9);
            SetFill("MaximumStock", 9);
            SetFill("ReorderPoint", 10);
            SetFill("MovementCounter", 10);
            SetFill("LastMovementDate", 12);
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0)
                {
                    _stockSeleccionado = null;
                    LimpiarDetalle();
                    return;
                }

                var fila = Tabla.SelectedRows[0];
                if (fila.Cells["ItemWarehouseStockId"].Value == null) return;

                int stockId = (int)fila.Cells["ItemWarehouseStockId"].Value;
                _stockSeleccionado = _stockList?.FirstOrDefault(s => s.ItemWarehouseStockId == stockId);

                if (_stockSeleccionado != null)
                    CargarDatosEnFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDatosEnFormulario()
        {
            if (_stockSeleccionado == null) return;

            SetTextBoxFromValue(Txt_Codigo, _stockSeleccionado.ItemCode, "CÓDIGO DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_Articulo, _stockSeleccionado.ItemName, "NOMBRE DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_Descripcion, _stockSeleccionado.Description, "DESCRIPCIÓN DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_Category, _stockSeleccionado.CategoryName, "CATEGORÍA DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_MeasurementUnits, _stockSeleccionado.UnitName, "UNIDAD DE MEDIDA");

            Txt_CurrentStock.Text = _stockSeleccionado.CurrentStock.ToString("0.00");
            Txt_MovementCounter.Text = _stockSeleccionado.MovementCounter.ToString();
            Txt_LastMovementDate.Text = _stockSeleccionado.LastMovementDate.HasValue
                ? _stockSeleccionado.LastMovementDate.Value.ToString("dd/MM/yyyy HH:mm")
                : "SIN MOVIMIENTOS";

            SetTextBoxFromValue(Txt_MinimumStock, _stockSeleccionado.MinimumStock.ToString("0.00"), "0.00");
            SetTextBoxFromValue(Txt_MaximumStock, _stockSeleccionado.MaximumStock.ToString("0.00"), "0.00");
            SetTextBoxFromValue(Txt_ReorderPoint, _stockSeleccionado.ReorderPoint.ToString("0.00"), "0.00");
        }

        private void SetTextBoxFromValue(TextBox txt, string value, string placeholder)
        {
            if (!string.IsNullOrWhiteSpace(value) && value != placeholder)
            {
                txt.Text = value;
                txt.ForeColor = Color.Black;
            }
            else
            {
                txt.Text = placeholder;
                txt.ForeColor = Color.Gray;
            }
        }

        #endregion ConfiguracionesTabla
        #region Paginacion

        private void CrearToolStripPaginacion()
        {
            toolStripPaginacion = new ToolStrip();
            toolStripPaginacion.Dock = DockStyle.None;
            toolStripPaginacion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            toolStripPaginacion.GripStyle = ToolStripGripStyle.Hidden;
            toolStripPaginacion.BackColor = Color.FromArgb(248, 249, 250);
            toolStripPaginacion.Height = 40;
            toolStripPaginacion.AutoSize = true;
            toolStripPaginacion.Location = new Point(this.Width - 225, 225);

            btnAnterior = new ToolStripButton
            {
                Text = "❮ Anterior",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(51, 140, 255),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Margin = new Padding(2),
                Padding = new Padding(8, 4, 8, 4)
            };
            btnAnterior.Click += (s, e) => CambiarPagina(paginaActual - 1);
            toolStripPaginacion.Items.Add(btnAnterior);

            btnSiguiente = new ToolStripButton
            {
                Text = "Siguiente ❯",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(238, 143, 109),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Margin = new Padding(2),
                Padding = new Padding(8, 4, 8, 4)
            };
            btnSiguiente.Click += (s, e) => CambiarPagina(paginaActual + 1);
            toolStripPaginacion.Items.Add(btnSiguiente);

            this.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.BringToFront();
        }

        private void ActualizarBotonesNumerados()
        {
            if (toolStripPaginacion == null) return;

            var itemsToRemove = toolStripPaginacion.Items.Cast<ToolStripItem>()
                .Where(i => i.Tag?.ToString() == "PageButton").ToList();
            foreach (var i in itemsToRemove)
                toolStripPaginacion.Items.Remove(i);

            if (totalPaginas <= 1) return;

            int inicio = Math.Max(1, paginaActual - 1);
            int fin = Math.Min(totalPaginas, paginaActual + 1);
            int posicion = toolStripPaginacion.Items.IndexOf(btnSiguiente);

            for (int i = inicio; i <= fin; i++)
            {
                var btn = new ToolStripButton
                {
                    Text = i.ToString(),
                    Tag = "PageButton",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Margin = new Padding(1),
                    Padding = new Padding(6, 4, 6, 4),
                    BackColor = i == paginaActual ? Color.FromArgb(238, 143, 109) : Color.FromArgb(240, 240, 240),
                    ForeColor = i == paginaActual ? Color.White : Color.FromArgb(51, 140, 255)
                };

                int numeroPagina = i;
                btn.Click += (s, e) => CambiarPagina(numeroPagina);
                toolStripPaginacion.Items.Insert(posicion++, btn);
            }
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1 || nuevaPagina > totalPaginas) return;
            paginaActual = nuevaPagina;
            AsignarDataSource();
            ConfigurarTabla();
            AjustarColumnas();
            ActualizarInfoPaginacion();
        }

        private void ActualizarInfoPaginacion()
        {
            if (toolStripPaginacion == null) return;

            totalPaginas = totalRegistros > 0
                ? (int)Math.Ceiling((double)totalRegistros / registrosPorPagina) : 0;

            if (btnAnterior != null) btnAnterior.Enabled = paginaActual > 1;
            if (btnSiguiente != null) btnSiguiente.Enabled = paginaActual < totalPaginas;

            ActualizarBotonesNumerados();

            int inicioRango = totalRegistros > 0 ? (paginaActual - 1) * registrosPorPagina + 1 : 0;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            if (Lbl_Paginas != null)
            {
                Lbl_Paginas.Text = totalRegistros == 0
                    ? "NO HAY ARTÍCULOS PARA MOSTRAR"
                    : $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} ARTÍCULOS";
            }
        }

        #endregion Paginacion
        #region Search

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                _ultimoTextoBusqueda = (Txt_ValorBuscado.ForeColor != Color.Gray)
                    ? Txt_ValorBuscado.Text.Trim() : "";

                _ultimoFiltro1 = Filtro1.SelectedItem?.ToString() ?? "TODOS";
                _ultimoFiltro2 = Filtro2.SelectedItem?.ToString() ?? "TODAS LAS CLASIFICACIONES";
                _ultimoFiltro3 = Filtro3.SelectedItem?.ToString() ?? "TODOS";

                paginaActual = 1;
                RefrescarListado();
                ConfigurarTabla();
                AjustarColumnas();
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
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR POR CÓDIGO O NOMBRE...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 0;

            _ultimoTextoBusqueda = "";
            _ultimoFiltro1 = "TODOS";
            _ultimoFiltro2 = "TODAS LAS CLASIFICACIONES";
            _ultimoFiltro3 = "TODOS";

            paginaActual = 1;

            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
            ActualizarInfoPaginacion();
        }

        #endregion Search
        #region AsignacionFocus

        private void ConfigurarTabIndexYFocus()
        {
            int contador = 0;
            Txt_ValorBuscado.TabIndex = contador; contador++;
            Filtro1.TabIndex = contador; contador++;
            Filtro2.TabIndex = contador; contador++;
            Filtro3.TabIndex = contador; contador++;
            Txt_MinimumStock.TabIndex = contador; contador++;
            Txt_MaximumStock.TabIndex = contador; contador++;
            Txt_ReorderPoint.TabIndex = contador; contador++;
            Btn_Ingreso.TabIndex = contador; contador++;
            Btn_Despacho.TabIndex = contador; contador++;
            Btn_Update.TabIndex = contador; contador++;

            Txt_ValorBuscado.Focus();
        }

        #endregion AsignacionFocus
        #region Validaciones

        private bool TienePlaceholder(TextBox txt, string placeholder)
        {
            return string.IsNullOrWhiteSpace(txt.Text) ||
                   txt.Text == placeholder ||
                   txt.ForeColor == Color.Gray;
        }

        private decimal ObtenerDecimalDesdeTextBox(TextBox txt, string placeholder, string nombreCampo)
        {
            if (TienePlaceholder(txt, placeholder)) return 0;

            if (!decimal.TryParse(txt.Text.Trim(), out decimal valor))
            {
                MessageBox.Show($"El valor del campo {nombreCampo} no es válido.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt.Focus();
                throw new Exception($"Valor no numérico en {nombreCampo}");
            }
            return valor;
        }

        #endregion Validaciones
        #region AccionesDeMovimiento

        private void Btn_Ingreso_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_bodegaActivaId.HasValue)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA BODEGA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!EsBodegaCentral(_bodegaActivaId))
                {
                    MessageBox.Show("LOS INGRESOS DE MERCADERÍA ÚNICAMENTE SE REGISTRAN EN LA BODEGA CENTRAL.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var frm = new Frm_KARDEX_WarehouseInventory_Ingreso
                {
                    UserData = this.UserData,
                    WarehouseId = _bodegaActivaId.Value,
                    WarehouseName = _bodegaActivaNombre
                };
                frm.ShowDialog(this);

                RefrescarListado();
                ConfigurarTabla();
                AjustarColumnas();
                ActualizarInfoPaginacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir ingreso: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Order_Click(object sender, EventArgs e)
        {/*
            try
            {
                if (!_bodegaActivaId.HasValue)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA BODEGA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!EsBodegaCentral(_bodegaActivaId))
                {
                    MessageBox.Show("ÚNICAMENTE LA BODEGA CENTRAL PUEDE ASIGNAR ARTÍCULOS A OTRAS BODEGAS.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var frm = new Frm_KARDEX_WarehouseOrder
                {
                    UserData = this.UserData,
                    SourceWarehouseId = _bodegaActivaId.Value,
                    SourceWarehouseName = _bodegaActivaNombre
                };
                frm.ShowDialog(this);

                RefrescarListado();
                ConfigurarTabla();
                AjustarColumnas();
                ActualizarInfoPaginacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir asignación: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void Btn_Despacho_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_bodegaActivaId.HasValue)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA BODEGA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_stockList == null || _stockList.Count == 0)
                {
                    MessageBox.Show("LA BODEGA NO TIENE ARTÍCULOS CON EXISTENCIAS.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var frm = new Frm_KARDEX_WarehouseDispatch
                {
                    UserData = this.UserData,
                    WarehouseId = _bodegaActivaId.Value,
                    WarehouseName = _bodegaActivaNombre
                };
                frm.ShowDialog(this);

                RefrescarListado();
                ConfigurarTabla();
                AjustarColumnas();
                ActualizarInfoPaginacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir despacho: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion AccionesDeMovimiento
        #region CRUD

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                if (_stockSeleccionado == null)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN ARTÍCULO DE LA TABLA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal nuevoMinimo = ObtenerDecimalDesdeTextBox(Txt_MinimumStock, "0.00", "STOCK MÍNIMO");
                decimal nuevoMaximo = ObtenerDecimalDesdeTextBox(Txt_MaximumStock, "0.00", "STOCK MÁXIMO");
                decimal nuevoReorden = ObtenerDecimalDesdeTextBox(Txt_ReorderPoint, "0.00", "PUNTO DE REORDEN");

                if (nuevoMaximo > 0 && nuevoMinimo > nuevoMaximo)
                {
                    MessageBox.Show("EL STOCK MÍNIMO NO PUEDE SER MAYOR AL STOCK MÁXIMO.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO QUE DESEA ACTUALIZAR LOS LÍMITES DE \"{_stockSeleccionado.ItemName}\"?",
                    "CONFIRMAR ACTUALIZACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                int resultado = Ctrl_ItemWarehouseStock.ActualizarLimitesStock(
                    _stockSeleccionado.ItemWarehouseStockId, nuevoMinimo, nuevoMaximo, nuevoReorden);

                if (resultado > 0)
                {
                    MessageBox.Show("LÍMITES ACTUALIZADOS EXITOSAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ConfigurarTabla();
                    AjustarColumnas();
                    ActualizarInfoPaginacion();
                }
                else
                {
                    MessageBox.Show("NO SE PUDIERON ACTUALIZAR LOS LÍMITES.", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            _stockSeleccionado = null;
            LimpiarDetalle();

            if (Tabla.Rows.Count > 0)
                Tabla.ClearSelection();
        }

        private void LimpiarDetalle()
        {
            SetTextBoxFromValue(Txt_Codigo, "", "CÓDIGO DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_Articulo, "", "NOMBRE DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_Descripcion, "", "DESCRIPCIÓN DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_Category, "", "CATEGORÍA DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_MeasurementUnits, "", "UNIDAD DE MEDIDA");
            SetTextBoxFromValue(Txt_MinimumStock, "", "0.00");
            SetTextBoxFromValue(Txt_MaximumStock, "", "0.00");
            SetTextBoxFromValue(Txt_ReorderPoint, "", "0.00");
            Txt_CurrentStock.Text = "";
            Txt_MovementCounter.Text = "";
            Txt_LastMovementDate.Text = "";
        }

        private void Btn_Permisos_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new Frm_KARDEX_WarehousePermissions())
                {
                    frm.UserData = UserData;
                    frm.PermisosUsuario = permisosUsuario;
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir permisos de bodega: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion CRUD
        #region ExportarExcel

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (_stockList == null || _stockList.Count == 0)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR.", "INFORMACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Inventario por Bodega",
                    FileName = $"KARDEX_InventarioBodega_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                var excelApp = new Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                worksheet.Name = "Inventario";

                worksheet.Cells[1, 1] = $"CONTROL DE INVENTARIO - BODEGA: {_bodegaActivaNombre}";
                worksheet.Range["A1:H1"].Merge();
                worksheet.Range["A1:H1"].Font.Size = 16;
                worksheet.Range["A1:H1"].Font.Bold = true;
                worksheet.Range["A1:H1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                worksheet.Range["A1:H1"].Interior.Color =
                    System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                worksheet.Range["A1:H1"].Font.Color =
                    System.Drawing.ColorTranslator.ToOle(Color.White);

                worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SECRON"}";
                worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {_stockList.Count}";

                int headerRow = 6;
                string[] headers = {
                    "CÓDIGO", "NOMBRE ARTÍCULO", "STOCK ACTUAL",
                    "STOCK MÍNIMO", "STOCK MÁXIMO", "PUNTO DE REORDEN",
                    "MOVIMIENTOS", "ÚLT. MOVIMIENTO"
                };

                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cells[headerRow, i + 1] = headers[i];

                var headerRange = worksheet.Range[$"A{headerRow}:H{headerRow}"];
                headerRange.Font.Bold = true;
                headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                headerRange.Interior.Color =
                    System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                int row = headerRow + 1;
                foreach (var s in _stockList)
                {
                    worksheet.Cells[row, 1] = s.ItemCode;
                    worksheet.Cells[row, 2] = s.ItemName;
                    worksheet.Cells[row, 3] = s.CurrentStock.ToString("N2");
                    worksheet.Cells[row, 4] = s.MinimumStock.ToString("N2");
                    worksheet.Cells[row, 5] = s.MaximumStock.ToString("N2");
                    worksheet.Cells[row, 6] = s.ReorderPoint.ToString("N2");
                    worksheet.Cells[row, 7] = s.MovementCounter;
                    worksheet.Cells[row, 8] = s.LastMovementDate.HasValue
                        ? s.LastMovementDate.Value.ToString("dd/MM/yyyy HH:mm") : "";

                    if (row % 2 == 0)
                        worksheet.Range[$"A{row}:H{row}"].Interior.Color =
                            System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));

                    row++;
                }

                var dataRange = worksheet.Range[$"A{headerRow}:H{row - 1}"];
                dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                worksheet.Columns.AutoFit();
                worksheet.Columns[2].ColumnWidth = 40;

                workbook.SaveAs(saveDialog.FileName);
                workbook.Close();
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                this.Cursor = Cursors.Default;

                var result = MessageBox.Show(
                    "ARCHIVO EXPORTADO EXITOSAMENTE.\n\n¿DESEA ABRIR EL ARCHIVO AHORA?",
                    "EXPORTACIÓN EXITOSA", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    System.Diagnostics.Process.Start(saveDialog.FileName);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL EXPORTAR: {ex.Message}", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion ExportarExcel
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

                MessageBox.Show(
                    $"ERROR AL CARGAR PERMISOS: {ex.Message}",
                    "ERROR SECRON",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
            AplicarEstadoBoton(boton, TienePermiso(permissionCode));
        }

        protected void ConfigurarControlesPorPermisos()
        {
            AplicarEstadoBotonPorPermiso(Btn_Search, "KARDEX_WAREHOUSE_READ");
            AplicarEstadoBotonPorPermiso(Btn_Export, "KARDEX_WAREHOUSE_READ");
            // Btn_Ingreso, Btn_Despacho, Btn_Update dependen también del
            // permiso contextual de bodega; se evalúan en AplicarPermisosContextualesDeBodega(),
            // que se ejecuta al cambiar de bodega (ComboBox_Warehouse_SelectedIndexChanged).
        }
        #endregion SistemaDePermisos
    }
}