using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_LocationsInventary : Form
    {
        #region PropiedadesIniciales

        // Datos del usuario autenticado
        public Mdl_Security_UserInfo UserData { get; set; }

        // Sede seleccionada para filtrar inventario
        private int? _sedeActivaId = null;
        private string _sedeActivaNombre = "";

        // Filtros de búsqueda
        private string _ultimoTextoBusqueda = "";
        private string _ultimoFiltro1 = "TODOS";
        private string _ultimoFiltro2 = "TODOS";

        // Selección actual en la grilla
        private List<Mdl_ItemStockByLocation> _stockList;
        private Mdl_ItemStockByLocation _stockSeleccionado = null;

        // Item seleccionado (para detalle del panel izquierdo)
        private int? _categoriaItemId = null;
        private int? _unidadItemId = null;

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        public Frm_KARDEX_LocationsInventary()
        {
            InitializeComponent();

            this.Resize += (s, e) =>
            {
                if (toolStripPaginacion != null)
                    toolStripPaginacion.Location = new Point(this.Width - 400, 225);
            };
        }

        private void Frm_KARDEX_LocationsInventary_Load(object sender, EventArgs e)
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

                // La carga la dispara el combo de sedes
                CargarComboSedes();

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
            Txt_MinimumStock.MaxLength = 18;
            Txt_MaximumStock.MaxLength = 18;
            Txt_ReorderPoint.MaxLength = 18;
            Txt_UnitCost.MaxLength = 15;
            Txt_LastPurchasePrice.MaxLength = 15;
            Txt_Category.MaxLength = 100;
            Txt_MeasurementUnits.MaxLength = 50;
        }

        private void ConfigurarComponentesDeshabilitados()
        {
            // Siempre solo lectura
            Txt_Codigo.Enabled = false;
            Txt_Articulo.Enabled = false;
            Txt_Descripcion.Enabled = false;
            Txt_Category.Enabled = false;
            Txt_MeasurementUnits.Enabled = false;
            ComboBox_HasExpiryDate.Enabled = false;
            ComboBox_HasLotControl.Enabled = false;
            Btn_SearchCategory.Enabled = false;
            Btn_SearchMeasurementUnits.Enabled = false;

            // Deshabilitar todo hasta que haya sede seleccionada
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
            Txt_UnitCost.Enabled = false;
            Txt_LastPurchasePrice.Enabled = false;
            Btn_Update.Enabled = false;
            Btn_Delete.Enabled = false;
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
            Txt_MinimumStock.Enabled = true;
            Txt_MaximumStock.Enabled = true;
            Txt_ReorderPoint.Enabled = true;
            Txt_UnitCost.Enabled = true;
            Txt_LastPurchasePrice.Enabled = true;
            Btn_Clear.Enabled = true;
            Btn_Export.Enabled = true;
            Btn_Update.Enabled = false;
            Btn_Delete.Enabled = false;
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
            ConfigurarPlaceHolder(Txt_UnitCost, "0.00");
            ConfigurarPlaceHolder(Txt_LastPurchasePrice, "0.00");
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

            // Filtro1: campo de búsqueda
            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.Items.Add("POR NOMBRE");
            Filtro1.SelectedIndex = 0;

            // Filtro2: clasificación de artículo
            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODAS LAS CLASIFICACIONES");
            var clasificaciones = Ctrl_ItemCategories.ObtenerCategoriasParaCombo();
            foreach (var c in clasificaciones)
                Filtro2.Items.Add(new ClasificacionItem(c.Key, c.Value));
            Filtro2.DisplayMember = "Nombre";
            Filtro2.SelectedIndex = 0;

            // Filtro3: estado de stock
            Filtro3.Items.Clear();
            Filtro3.Items.Add("TODOS");
            Filtro3.Items.Add("STOCK BAJO MÍNIMO");
            Filtro3.Items.Add("STOCK SOBRE MÁXIMO");
            Filtro3.SelectedIndex = 0;
        }

        #endregion ConfigurarFiltros
        #region ComboSedes

        private void CargarComboSedes()
        {
            ComboBox_Location.SelectedIndexChanged -= ComboBox_Location_SelectedIndexChanged;
            ComboBox_Location.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Location.Items.Clear();
            ComboBox_Location.Items.Add("SELECCIONAR SEDE...");

            var sedes = Ctrl_Locations.ObtenerLocationsActivas();
            foreach (var sede in sedes)
                ComboBox_Location.Items.Add(new CategoriaItem(sede.Key, sede.Value));

            ComboBox_Location.DisplayMember = "Nombre";
            ComboBox_Location.SelectedIndex = 0;
            ComboBox_Location.SelectedIndexChanged += ComboBox_Location_SelectedIndexChanged;
        }
        private void ComboBox_Location_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_Location.SelectedItem is CategoriaItem sede))
            {
                _sedeActivaId = null;
                _sedeActivaNombre = "";
                DeshabilitarTodo();
                Tabla.DataSource = null;
                if (Lbl_Paginas != null)
                    Lbl_Paginas.Text = "SELECCIONE UNA SEDE";
                return;
            }

            _sedeActivaId = sede.Id;
            _sedeActivaNombre = sede.Nombre;

            HabilitarTodo();
            LimpiarFormulario();

            paginaActual = 1;
            totalRegistros = 0;

            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
            ActualizarInfoPaginacion();
        }
        #endregion ComboSedes
        #region ConfiguracionesTabla

        private void CargarInventario()
        {
            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
        }

        private void RefrescarListado()
        {
            // Obtener todo el stock con detalle, filtrado opcionalmente por sede
            if (_sedeActivaId.HasValue)
            {
                _stockList = Ctrl_ItemStockByLocation.ObtenerStockPorUbicacionConDetalle(_sedeActivaId.Value);
            }
            else
            {
                // Sin sede seleccionada: mostrar todo (todas las sedes)
                _stockList = new List<Mdl_ItemStockByLocation>();
                var sedes = Ctrl_ItemStockByLocation.ObtenerSedesParaCombo();
                foreach (var sede in sedes)
                {
                    var stockSede = Ctrl_ItemStockByLocation.ObtenerStockPorUbicacionConDetalle(sede.Key);
                    _stockList.AddRange(stockSede);
                }
            }

            AplicarFiltrosEnMemoria();
            AsignarDataSource();
        }

        private void AplicarFiltrosEnMemoria()
        {
            if (_stockList == null) return;

            // Filtro texto
            if (!string.IsNullOrWhiteSpace(_ultimoTextoBusqueda))
            {
                string texto = _ultimoTextoBusqueda.ToUpper();
                _stockList = _stockList.Where(s =>
                    (s.ItemCode?.ToUpper().Contains(texto) ?? false) ||
                    (s.ItemName?.ToUpper().Contains(texto) ?? false)).ToList();
            }

            // Filtro3: estado de stock
            if (_ultimoFiltro2 == "STOCK BAJO MÍNIMO")
                _stockList = _stockList.Where(s => s.MinimumStock > 0 && s.CurrentStock <= s.MinimumStock).ToList();
            else if (_ultimoFiltro2 == "STOCK SOBRE MÁXIMO")
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

            // Aplicar paginación en memoria
            var paginado = _stockList
                .Skip((paginaActual - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            var data = paginado.Select(s => new
            {
                s.ItemStockLocationId,
                s.ItemId,
                s.LocationId,
                s.ItemCode,
                s.ItemName,
                s.CurrentStock,
                s.ReservedStock,
                s.AvailableStock,
                s.MinimumStock,
                s.MaximumStock,
                s.LastMovementDate
            }).ToList();

            Tabla.DataSource = data;
        }

        private void ConfigurarTabla()
        {
            if (Tabla.Columns.Count > 0)
            {
                if (Tabla.Columns.Contains("ItemStockLocationId"))
                    Tabla.Columns["ItemStockLocationId"].Visible = false;
                if (Tabla.Columns.Contains("ItemId"))
                    Tabla.Columns["ItemId"].Visible = false;
                if (Tabla.Columns.Contains("LocationId"))
                    Tabla.Columns["LocationId"].Visible = false;

                if (Tabla.Columns.Contains("ItemCode"))
                    Tabla.Columns["ItemCode"].HeaderText = "CÓDIGO";
                if (Tabla.Columns.Contains("ItemName"))
                    Tabla.Columns["ItemName"].HeaderText = "NOMBRE DEL ARTÍCULO";
                if (Tabla.Columns.Contains("CurrentStock"))
                    Tabla.Columns["CurrentStock"].HeaderText = "STOCK ACTUAL";
                if (Tabla.Columns.Contains("ReservedStock"))
                    Tabla.Columns["ReservedStock"].HeaderText = "RESERVADO";
                if (Tabla.Columns.Contains("AvailableStock"))
                    Tabla.Columns["AvailableStock"].HeaderText = "DISPONIBLE";
                if (Tabla.Columns.Contains("MinimumStock"))
                    Tabla.Columns["MinimumStock"].HeaderText = "STOCK MÍNIMO";
                if (Tabla.Columns.Contains("MaximumStock"))
                    Tabla.Columns["MaximumStock"].HeaderText = "STOCK MÁXIMO";
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
            SetFill("ItemName", 32);
            SetFill("CurrentStock", 10);
            SetFill("ReservedStock", 9);
            SetFill("AvailableStock", 9);
            SetFill("MinimumStock", 9);
            SetFill("MaximumStock", 9);
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
                    Btn_Update.Enabled = false;
                    Btn_Delete.Enabled = false;
                    return;
                }

                var fila = Tabla.SelectedRows[0];
                if (fila.Cells["ItemStockLocationId"].Value == null) return;

                int stockId = (int)fila.Cells["ItemStockLocationId"].Value;
                _stockSeleccionado = _stockList?.FirstOrDefault(s => s.ItemStockLocationId == stockId);

                if (_stockSeleccionado != null)
                {
                    CargarDatosEnFormulario();
                    Btn_Update.Enabled = true;
                    Btn_Delete.Enabled = true;
                }
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

            SetTextBoxFromValue(Txt_MinimumStock,
                _stockSeleccionado.MinimumStock.ToString("0.00"), "0.00");
            SetTextBoxFromValue(Txt_MaximumStock,
                _stockSeleccionado.MaximumStock.ToString("0.00"), "0.00");

            // Cargar detalles adicionales del artículo desde Items
            var items = Ctrl_Items.MostrarArticulos(1, 9999);
            var item = items.FirstOrDefault(i => i.ItemId == _stockSeleccionado.ItemId);

            if (item != null)
            {
                _categoriaItemId = item.CategoryId;
                _unidadItemId = item.UnitId;

                SetTextBoxFromValue(Txt_Descripcion, item.Description, "DESCRIPCIÓN DEL ARTÍCULO");
                SetTextBoxFromValue(Txt_ReorderPoint, item.ReorderPoint.ToString("0.00"), "0.00");
                SetTextBoxFromValue(Txt_UnitCost, item.UnitCost.ToString("0.00"), "0.00");
                SetTextBoxFromValue(Txt_LastPurchasePrice, item.LastPurchasePrice.ToString("0.00"), "0.00");

                string nombreCategoria = ObtenerNombreCategoria(item.CategoryId);
                string nombreUnidad = ObtenerNombreUnidad(item.UnitId);

                SetTextBoxFromValue(Txt_Category, nombreCategoria, "CATEGORÍA DEL ARTÍCULO");
                SetTextBoxFromValue(Txt_MeasurementUnits, nombreUnidad, "UNIDAD DE MEDIDA");

                if (ComboBox_HasLotControl.Items.Count > 0)
                    ComboBox_HasLotControl.SelectedItem = item.HasLotControl ? "SI" : "NO";
                if (ComboBox_HasExpiryDate.Items.Count > 0)
                    ComboBox_HasExpiryDate.SelectedItem = item.HasExpiryDate ? "SI" : "NO";
            }
        }

        private string ObtenerNombreCategoria(int categoriaId)
        {
            var lista = Ctrl_ItemCategories.ObtenerCategoriasParaCombo();
            var item = lista.FirstOrDefault(c => c.Key == categoriaId);
            return item.Equals(default(KeyValuePair<int, string>)) ? "CATEGORÍA DEL ARTÍCULO" : item.Value;
        }

        private string ObtenerNombreUnidad(int unidadId)
        {
            var lista = Ctrl_MeasurementUnits.ObtenerUnidadesParaCombo();
            var item = lista.FirstOrDefault(u => u.Key == unidadId);
            return item.Equals(default(KeyValuePair<int, string>)) ? "UNIDAD DE MEDIDA" : item.Value;
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
            toolStripPaginacion.Location = new Point(this.Width - 400, 225);

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
                _ultimoFiltro2 = Filtro3.SelectedItem?.ToString() ?? "TODOS";

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
            _ultimoFiltro2 = "TODOS";
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
            Txt_ValorBuscado.TabIndex = 0;
            Filtro1.TabIndex = 1;
            Filtro2.TabIndex = 2;
            Filtro3.TabIndex = 3;
            Txt_MinimumStock.TabIndex = 4;
            Txt_MaximumStock.TabIndex = 5;
            Txt_ReorderPoint.TabIndex = 6;
            Txt_UnitCost.TabIndex = 7;
            Txt_LastPurchasePrice.TabIndex = 8;
            Btn_Update.TabIndex = 9;
            Btn_Delete.TabIndex = 10;

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

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO QUE DESEA ACTUALIZAR EL STOCK DE \"{_stockSeleccionado.ItemName}\"?",
                    "CONFIRMAR ACTUALIZACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                _stockSeleccionado.MinimumStock = ObtenerDecimalDesdeTextBox(
                    Txt_MinimumStock, "0.00", "STOCK MÍNIMO");
                _stockSeleccionado.MaximumStock = ObtenerDecimalDesdeTextBox(
                    Txt_MaximumStock, "0.00", "STOCK MÁXIMO");

                int resultado = Ctrl_ItemStockByLocation.ActualizarStockCompleto(_stockSeleccionado);

                if (resultado > 0)
                {
                    MessageBox.Show("STOCK ACTUALIZADO EXITOSAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ConfigurarTabla();
                    AjustarColumnas();
                    ActualizarInfoPaginacion();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO ACTUALIZAR EL STOCK.", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_stockSeleccionado == null)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN ARTÍCULO DE LA TABLA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO QUE DESEA ELIMINAR \"{_stockSeleccionado.ItemName}\" DEL INVENTARIO DE ESTA SEDE?\n\n" +
                    "ESTA ACCIÓN NO AFECTA EL CATÁLOGO MAESTRO DE ARTÍCULOS.",
                    "CONFIRMAR ELIMINACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion != DialogResult.Yes) return;

                int resultado = Ctrl_ItemStockByLocation.EliminarStockDeUbicacion(
                    _stockSeleccionado.ItemStockLocationId);

                if (resultado > 0)
                {
                    MessageBox.Show("ARTÍCULO ELIMINADO DEL INVENTARIO EXITOSAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ConfigurarTabla();
                    AjustarColumnas();
                    ActualizarInfoPaginacion();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO ELIMINAR EL ARTÍCULO.", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar: {ex.Message}", "Error",
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
            _categoriaItemId = null;
            _unidadItemId = null;
            LimpiarDetalle();
            Btn_Update.Enabled = false;
            Btn_Delete.Enabled = false;

            if (Tabla.Rows.Count > 0)
                Tabla.ClearSelection();
        }

        private void LimpiarDetalle()
        {
            SetTextBoxFromValue(Txt_Codigo, "", "CÓDIGO DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_Articulo, "", "NOMBRE DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_Descripcion, "", "DESCRIPCIÓN DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_MinimumStock, "", "0.00");
            SetTextBoxFromValue(Txt_MaximumStock, "", "0.00");
            SetTextBoxFromValue(Txt_ReorderPoint, "", "0.00");
            SetTextBoxFromValue(Txt_UnitCost, "", "0.00");
            SetTextBoxFromValue(Txt_LastPurchasePrice, "", "0.00");
            SetTextBoxFromValue(Txt_Category, "", "CATEGORÍA DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_MeasurementUnits, "", "UNIDAD DE MEDIDA");
        }

        #endregion CRUD
        #region Subformularios

        private void Btn_Template_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = new Frm_KARDEX_LocationsInventary_Templates
                {
                    UserData = this.UserData
                };
                frm.ShowDialog(this);

                // Refrescar al volver
                RefrescarListado();
                ConfigurarTabla();
                AjustarColumnas();
                ActualizarInfoPaginacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir plantillas: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Subformularios
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
                    Title = "Exportar Inventario por Sede",
                    FileName = $"KARDEX_Inventario_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
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

                worksheet.Cells[1, 1] = "CONTROL DE INVENTARIOS POR SEDE - KARDEX";
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
                    "CÓDIGO", "NOMBRE ARTÍCULO",
                    "STOCK ACTUAL", "RESERVADO", "DISPONIBLE",
                    "STOCK MÍNIMO", "STOCK MÁXIMO", "ÚLT. MOVIMIENTO"
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
                    worksheet.Cells[row, 4] = s.ReservedStock.ToString("N2");
                    worksheet.Cells[row, 5] = s.AvailableStock.ToString("N2");
                    worksheet.Cells[row, 6] = s.MinimumStock.ToString("N2");
                    worksheet.Cells[row, 7] = s.MaximumStock.ToString("N2");
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
    }
}