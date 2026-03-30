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
    public partial class Frm_KARDEX_ItemsManagment : Form
    {
        #region PropiedadesIniciales

        // Datos del usuario autenticado
        public Mdl_Security_UserInfo UserData { get; set; }

        // Filtros de búsqueda
        private string _ultimoTextoBusqueda = "";
        private int? _ultimaCategoriaFiltroId = null;
        private string _ultimoFiltro1 = "TODOS";
        private string _ultimoFiltro2 = "TODOS";
        private string _ultimoFiltro3 = "TODOS";

        // Selección actual
        private List<Mdl_Items> _itemsList;
        private Mdl_Items _itemSeleccionado = null;

        // Categoría y unidad seleccionadas en el detalle
        private int? _categoriaSeleccionadaId = null;
        private int? _unidadSeleccionadaId = null;

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        public Frm_KARDEX_ItemsManagment()
        {
            InitializeComponent();

            this.Resize += FormularioResize;
            this.Resize += (s, e) =>
            {
                if (toolStripPaginacion != null)
                {
                    toolStripPaginacion.Location = new Point(this.Width - 300, 225);
                }
            };
        }

        private void Frm_KARDEX_Managment_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarTabIndexYFocus();
                ConfigurarMaxLengthTextBox();
                ConfigurarComponentesDeshabilitados();
                ConfigurarPlaceHoldersTextbox();
                ConfigurarFiltros();
                ConfigurarCombosHas();
                CrearToolStripPaginacion();

                CargarArticulos();
                ActualizarInfoPaginacion();
                CargarProximoCodigoItem();

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
            {
                Tabla.Refresh();
            }
        }

        #endregion PropiedadesIniciales
        #region ConfigurarTextBox

        private void ConfigurarMaxLengthTextBox()
        {
            // Buscador
            Txt_ValorBuscado.MaxLength = 100;

            // Detalle del artículo
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
            Txt_Codigo.Enabled = false;
            // Estos los controla el sistema (búsqueda de formularios), normalmente estarán ReadOnly/Enabled = false
            Txt_Category.Enabled = false;
            Txt_MeasurementUnits.Enabled = false;
        }
        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR POR CÓDIGO, NOMBRE O DESCRIPCIÓN...");

            ConfigurarPlaceHolder(Txt_Codigo, "CÓDIGO DEL ARTÍCULO");
            ConfigurarPlaceHolder(Txt_Articulo, "NOMBRE DEL ARTÍCULO");
            ConfigurarPlaceHolder(Txt_Descripcion, "DESCRIPCIÓN DEL ARTÍCULO");

            ConfigurarPlaceHolder(Txt_MinimumStock, "0.00");
            ConfigurarPlaceHolder(Txt_MaximumStock, "0.00");
            ConfigurarPlaceHolder(Txt_ReorderPoint, "0.00");
            ConfigurarPlaceHolder(Txt_UnitCost, "0.00");
            ConfigurarPlaceHolder(Txt_LastPurchasePrice, "0.00");

            ConfigurarPlaceHolder(Txt_Category, "SELECCIONAR CATEGORÍA");
            ConfigurarPlaceHolder(Txt_MeasurementUnits, "SELECCIONAR UNIDAD DE MEDIDA");
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
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            CargarFiltros();
        }

        private void CargarFiltros()
        {
            // Filtro1: campo de búsqueda
            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.Items.Add("POR NOMBRE");
            Filtro1.Items.Add("POR DESCRIPCIÓN");
            Filtro1.SelectedIndex = 0;

            // Filtro2: reservado (por ahora general, puedes cambiarlo luego)
            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS");
            Filtro2.Items.Add("CON CONTROL DE LOTE");
            Filtro2.Items.Add("SIN CONTROL DE LOTE");
            Filtro2.SelectedIndex = 0;

            // Filtro3: estado
            Filtro3.Items.Clear();
            Filtro3.Items.Add("TODOS");
            Filtro3.Items.Add("SOLO ACTIVOS");
            Filtro3.Items.Add("SOLO INACTIVOS");
            Filtro3.SelectedIndex = 1; // Por defecto activos
        }

        #endregion Filtros
        #region CodigoItemAutomatico
        // ⭐ MÉTODO PARA CARGAR EL PRÓXIMO CÓDIGO DE ARTÍCULO
        private void CargarProximoCodigoItem()
        {
            try
            {
                string proximoCodigo = Ctrl_Items.ObtenerProximoCodigoArticulo();
                Txt_Codigo.Text = proximoCodigo;
                Txt_Codigo.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de artículo: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Txt_Codigo.Text = "ERROR";
            }
        }
        #endregion CodigoItemAutomatico
        #region ConfigurarCombobox

        private void ConfigurarCombosHas()
        {
            ComboBox_HasLotControl.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_HasExpiryDate.DropDownStyle = ComboBoxStyle.DropDownList;

            ComboBox_HasLotControl.Items.Clear();
            ComboBox_HasLotControl.Items.Add("No");
            ComboBox_HasLotControl.Items.Add("Si");
            ComboBox_HasLotControl.SelectedIndex = 0;

            ComboBox_HasExpiryDate.Items.Clear();
            ComboBox_HasExpiryDate.Items.Add("No");
            ComboBox_HasExpiryDate.Items.Add("Si");
            ComboBox_HasExpiryDate.SelectedIndex = 0;
        }

        #endregion ConfigurarCombobox
        #region ConfiguracionesTabla

        private void CargarArticulos()
        {
            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
        }

        private void RefrescarListado()
        {
            _itemsList = Ctrl_Items.MostrarArticulos(paginaActual, registrosPorPagina);
            AsignarDataSourceArticulos();
        }


        private void ConfigurarTabla()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["ItemCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["ItemName"].HeaderText = "NOMBRE DEL ARTÍCULO";
                Tabla.Columns["Description"].HeaderText = "DESCRIPCIÓN";

                // 👇 Nuevas columnas visibles
                Tabla.Columns["CategoryName"].HeaderText = "CATEGORÍA";
                Tabla.Columns["UnitName"].HeaderText = "UNIDAD DE MEDIDA";

                Tabla.Columns["MinimumStock"].HeaderText = "STOCK MÍNIMO";
                Tabla.Columns["MaximumStock"].HeaderText = "STOCK MÁXIMO";
                Tabla.Columns["ReorderPoint"].HeaderText = "PUNTO REORDEN";
                Tabla.Columns["UnitCost"].HeaderText = "COSTO UNITARIO";
                Tabla.Columns["LastPurchasePrice"].HeaderText = "ÚLTIMO PRECIO COMPRA";
                Tabla.Columns["HasLotControl"].HeaderText = "CONTROL LOTES";
                Tabla.Columns["HasExpiryDate"].HeaderText = "FECHA CADUCIDAD";

                // Ocultar campos que no necesito ver en la vista
                Tabla.Columns["ItemId"].Visible = false;
                Tabla.Columns["CategoryId"].Visible = false;
                Tabla.Columns["UnitId"].Visible = false;
                Tabla.Columns["IsActive"].Visible = false;
                Tabla.Columns["CreatedDate"].Visible = false;
                Tabla.Columns["CreatedBy"].Visible = false;
                Tabla.Columns["ModifiedDate"].Visible = false;
                Tabla.Columns["ModifiedBy"].Visible = false;
            }

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToResizeRows = false;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }
        public void AjustarColumnas()
        {
            if (Tabla.Columns.Count == 0)
                return;

            // Código
            if (Tabla.Columns.Contains("ItemCode"))
            {
                Tabla.Columns["ItemCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["ItemCode"].FillWeight = 10; // un poco angosta
            }

            // Nombre del artículo
            if (Tabla.Columns.Contains("ItemName"))
            {
                Tabla.Columns["ItemName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["ItemName"].FillWeight = 22;
            }

            // Descripción
            if (Tabla.Columns.Contains("Description"))
            {
                Tabla.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Description"].FillWeight = 25;
            }

            // ⭐ Categoría (nombre)
            if (Tabla.Columns.Contains("CategoryName"))
            {
                Tabla.Columns["CategoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CategoryName"].FillWeight = 13;
            }

            // ⭐ Unidad de Medida (nombre)
            if (Tabla.Columns.Contains("UnitName"))
            {
                Tabla.Columns["UnitName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["UnitName"].FillWeight = 13;
            }

            // Stock mínimo
            if (Tabla.Columns.Contains("MinimumStock"))
            {
                Tabla.Columns["MinimumStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["MinimumStock"].FillWeight = 8;
            }

            // Stock máximo
            if (Tabla.Columns.Contains("MaximumStock"))
            {
                Tabla.Columns["MaximumStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["MaximumStock"].FillWeight = 8;
            }

            // Punto de reorden
            if (Tabla.Columns.Contains("ReorderPoint"))
            {
                Tabla.Columns["ReorderPoint"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["ReorderPoint"].FillWeight = 8;
            }

            // Costo unitario
            if (Tabla.Columns.Contains("UnitCost"))
            {
                Tabla.Columns["UnitCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["UnitCost"].FillWeight = 8;
            }

            // Último precio de compra
            if (Tabla.Columns.Contains("LastPurchasePrice"))
            {
                Tabla.Columns["LastPurchasePrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["LastPurchasePrice"].FillWeight = 8;
            }

            // Control de lotes
            if (Tabla.Columns.Contains("HasLotControl"))
            {
                Tabla.Columns["HasLotControl"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["HasLotControl"].FillWeight = 7;
            }

            // Maneja caducidad
            if (Tabla.Columns.Contains("HasExpiryDate"))
            {
                Tabla.Columns["HasExpiryDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["HasExpiryDate"].FillWeight = 7;
            }
        }


        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count > 0)
            {
                CargarDatosItemSeleccionado();
            }
        }

        private void CargarDatosItemSeleccionado()
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0)
                    return;

                DataGridViewRow fila = Tabla.SelectedRows[0];
                int itemId = Convert.ToInt32(fila.Cells["ItemId"].Value);
                _itemSeleccionado = _itemsList.FirstOrDefault(p => p.ItemId == itemId);

                if (_itemSeleccionado == null)
                    return;

                // Campos de texto
                SetTextBoxFromValue(Txt_Codigo, _itemSeleccionado.ItemCode, "CÓDIGO DEL ARTÍCULO");
                SetTextBoxFromValue(Txt_Articulo, _itemSeleccionado.ItemName, "NOMBRE DEL ARTÍCULO");
                SetTextBoxFromValue(Txt_Descripcion, _itemSeleccionado.Description, "DESCRIPCIÓN DEL ARTÍCULO");

                SetTextBoxFromValue(Txt_MinimumStock, _itemSeleccionado.MinimumStock.ToString("0.00"), "0.00");
                SetTextBoxFromValue(Txt_MaximumStock, _itemSeleccionado.MaximumStock.ToString("0.00"), "0.00");
                SetTextBoxFromValue(Txt_ReorderPoint, _itemSeleccionado.ReorderPoint.ToString("0.00"), "0.00");
                SetTextBoxFromValue(Txt_UnitCost, _itemSeleccionado.UnitCost.ToString("0.00"), "0.00");
                SetTextBoxFromValue(Txt_LastPurchasePrice, _itemSeleccionado.LastPurchasePrice.ToString("0.00"), "0.00");

                // Guardar IDs para detalle
                _categoriaSeleccionadaId = _itemSeleccionado.CategoryId;
                _unidadSeleccionadaId = _itemSeleccionado.UnitId;

                // Mostrar nombres de categoría/unidad en Txt_Category / Txt_MeasurementUnits
                // Aquí los traemos con ayuda de los controllers (búsqueda simple)
                string nombreCategoria = ObtenerNombreCategoria(_categoriaSeleccionadaId);
                string nombreUnidad = ObtenerNombreUnidad(_unidadSeleccionadaId);

                SetTextBoxFromValue(Txt_Category, nombreCategoria, "SELECCIONAR CATEGORÍA");
                SetTextBoxFromValue(Txt_MeasurementUnits, nombreUnidad, "SELECCIONAR UNIDAD DE MEDIDA");

                ComboBox_HasLotControl.SelectedItem = _itemSeleccionado.HasLotControl ? "Si" : "No";
                ComboBox_HasExpiryDate.SelectedItem = _itemSeleccionado.HasExpiryDate ? "Si" : "No";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del artículo: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ObtenerNombreCategoria(int? categoriaId)
        {
            if (!categoriaId.HasValue || categoriaId <= 0)
                return "SELECCIONAR CATEGORÍA";

            var categoriasCombo = Ctrl_ItemCategories.ObtenerCategoriasParaCombo();
            var item = categoriasCombo.FirstOrDefault(c => c.Key == categoriaId.Value);
            return item.Equals(default(KeyValuePair<int, string>)) ? "SELECCIONAR CATEGORÍA" : item.Value;
        }

        private string ObtenerNombreUnidad(int? unidadId)
        {
            if (!unidadId.HasValue || unidadId <= 0)
                return "SELECCIONAR UNIDAD DE MEDIDA";

            var unidadesCombo = Ctrl_MeasurementUnits.ObtenerUnidadesParaCombo();
            var item = unidadesCombo.FirstOrDefault(c => c.Key == unidadId.Value);
            return item.Equals(default(KeyValuePair<int, string>)) ? "SELECCIONAR UNIDAD DE MEDIDA" : item.Value;
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
        private void AsignarDataSourceArticulos()
        {
            if (_itemsList == null)
            {
                Tabla.DataSource = null;
                return;
            }

            // Catálogos de categorías y unidades
            var categorias = Ctrl_ItemCategories.ObtenerCategoriasParaCombo();
            var unidades = Ctrl_MeasurementUnits.ObtenerUnidadesParaCombo();

            var data = _itemsList.Select(i => new
            {
                i.ItemId,
                i.ItemCode,
                i.ItemName,
                i.Description,
                i.CategoryId,
                i.UnitId,
                i.MinimumStock,
                i.MaximumStock,
                i.ReorderPoint,
                i.UnitCost,
                i.LastPurchasePrice,
                i.HasLotControl,
                i.HasExpiryDate,
                i.IsActive,
                i.CreatedDate,
                i.CreatedBy,
                i.ModifiedDate,
                i.ModifiedBy,

                // 👇 Nombres calculados
                CategoryName = categorias.FirstOrDefault(c => c.Key == i.CategoryId).Value ?? "SIN CATEGORÍA",
                UnitName = unidades.FirstOrDefault(u => u.Key == i.UnitId).Value ?? "SIN UNIDAD"
            }).ToList();

            Tabla.DataSource = data;
        }

        #endregion ConfiguracionesTabla
        #region Search

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string valorBusqueda = "";
                if (Txt_ValorBuscado.Text != "BUSCAR POR CÓDIGO, NOMBRE O DESCRIPCIÓN..." &&
                    !string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                {
                    valorBusqueda = Txt_ValorBuscado.Text.Trim();
                }

                string filtro1 = Filtro1.SelectedItem?.ToString() ?? "TODOS";
                string filtro2 = Filtro2.SelectedItem?.ToString() ?? "TODOS";
                string filtro3 = Filtro3.SelectedItem?.ToString() ?? "TODOS";

                // Por ahora, el filtro de BD será solo texto + categoría (si la quieres usar después)
                int? categoriaId = null; // Filtro a futuro por categoría, si deseas

                _ultimoTextoBusqueda = valorBusqueda;
                _ultimoFiltro1 = filtro1;
                _ultimoFiltro2 = filtro2;
                _ultimoFiltro3 = filtro3;
                _ultimaCategoriaFiltroId = categoriaId;

                paginaActual = 1;

                // Traer desde BD
                _itemsList = Ctrl_Items.BuscarArticulos(
                    textoBusqueda: valorBusqueda,
                    categoryId: categoriaId,
                    pageNumber: paginaActual,
                    pageSize: registrosPorPagina
                );

                // filtros Filtro2 / Filtro3...
                if (filtro2 == "CON CONTROL DE LOTE")
                    _itemsList = _itemsList.Where(i => i.HasLotControl).ToList();
                else if (filtro2 == "SIN CONTROL DE LOTE")
                    _itemsList = _itemsList.Where(i => !i.HasLotControl).ToList();

                if (filtro3 == "SOLO ACTIVOS")
                    _itemsList = _itemsList.Where(i => i.IsActive).ToList();
                else if (filtro3 == "SOLO INACTIVOS")
                    _itemsList = _itemsList.Where(i => !i.IsActive).ToList();

                AsignarDataSourceArticulos();
                ConfigurarTabla();
                AjustarColumnas();


                totalRegistros = Ctrl_Items.ContarTotalArticulos(valorBusqueda, categoriaId);
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
            Txt_ValorBuscado.Text = "BUSCAR POR CÓDIGO, NOMBRE O DESCRIPCIÓN...";
            Txt_ValorBuscado.ForeColor = Color.Gray;

            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 1;

            _ultimoTextoBusqueda = "";
            _ultimoFiltro1 = "TODOS";
            _ultimoFiltro2 = "TODOS";
            _ultimoFiltro3 = "SOLO ACTIVOS";
            _ultimaCategoriaFiltroId = null;

            paginaActual = 1;
            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
            ActualizarInfoPaginacion();
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
            toolStripPaginacion.Height = 40;
            toolStripPaginacion.AutoSize = true;
            toolStripPaginacion.Location = new Point(this.Width - 400, 225);

            btnAnterior = new ToolStripButton();
            btnAnterior.Text = "❮ Anterior";
            btnAnterior.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnterior.ForeColor = Color.White;
            btnAnterior.BackColor = Color.FromArgb(51, 140, 255);
            btnAnterior.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAnterior.Margin = new Padding(2);
            btnAnterior.Padding = new Padding(8, 4, 8, 4);
            btnAnterior.Click += (s, e) => CambiarPagina(paginaActual - 1);

            toolStripPaginacion.Items.Add(btnAnterior);

            btnSiguiente = new ToolStripButton();
            btnSiguiente.Text = "Siguiente ❯";
            btnSiguiente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSiguiente.ForeColor = Color.White;
            btnSiguiente.BackColor = Color.FromArgb(238, 143, 109);
            btnSiguiente.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSiguiente.Margin = new Padding(2);
            btnSiguiente.Padding = new Padding(8, 4, 8, 4);
            btnSiguiente.Click += (s, e) => CambiarPagina(paginaActual + 1);

            toolStripPaginacion.Items.Add(btnSiguiente);

            this.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.BringToFront();
        }

        private void ActualizarBotonesNumerados()
        {
            var itemsToRemove = toolStripPaginacion.Items.Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton").ToList();

            foreach (var item in itemsToRemove)
            {
                toolStripPaginacion.Items.Remove(item);
            }

            if (totalPaginas <= 1)
                return;

            int inicioRango = Math.Max(1, paginaActual - 1);
            int finRango = Math.Min(totalPaginas, paginaActual + 1);
            int posicionInsertar = toolStripPaginacion.Items.IndexOf(btnSiguiente);

            for (int i = inicioRango; i <= finRango; i++)
            {
                ToolStripButton btnPagina = new ToolStripButton();
                btnPagina.Text = i.ToString();
                btnPagina.Tag = "PageButton";
                btnPagina.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnPagina.Margin = new Padding(1);
                btnPagina.Padding = new Padding(6, 4, 6, 4);

                if (i == paginaActual)
                {
                    btnPagina.BackColor = Color.FromArgb(238, 143, 109);
                    btnPagina.ForeColor = Color.White;
                }
                else
                {
                    btnPagina.BackColor = Color.FromArgb(240, 240, 240);
                    btnPagina.ForeColor = Color.FromArgb(51, 140, 255);
                }

                int numeroPagina = i;
                btnPagina.Click += (s, e) => CambiarPagina(numeroPagina);

                toolStripPaginacion.Items.Insert(posicionInsertar++, btnPagina);
            }
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1 || nuevaPagina > totalPaginas)
                return;

            paginaActual = nuevaPagina;

            if (!string.IsNullOrEmpty(_ultimoTextoBusqueda) || _ultimaCategoriaFiltroId.HasValue)
            {
                _itemsList = Ctrl_Items.BuscarArticulos(
                    textoBusqueda: _ultimoTextoBusqueda,
                    categoryId: _ultimaCategoriaFiltroId,
                    pageNumber: paginaActual,
                    pageSize: registrosPorPagina
                );

                AsignarDataSourceArticulos();
                ConfigurarTabla();
                AjustarColumnas();

            }
            else
            {
                RefrescarListado();
                ConfigurarTabla();
                AjustarColumnas();
            }

            ActualizarInfoPaginacion();
        }

        private void ActualizarInfoPaginacion()
        {
            if (totalRegistros == 0)
            {
                totalRegistros = Ctrl_Items.ContarTotalArticulos(_ultimoTextoBusqueda, _ultimaCategoriaFiltroId);
            }

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;

            ActualizarBotonesNumerados();

            int inicioRango = (paginaActual - 1) * registrosPorPagina + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            if (Lbl_Paginas != null)
            {
                if (totalRegistros == 0)
                {
                    Lbl_Paginas.Text = "NO HAY ARTÍCULOS PARA MOSTRAR";
                }
                else
                {
                    Lbl_Paginas.Text = $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} ARTÍCULOS";
                }
            }
        }

        #endregion ToolStrip_Paginacion
        #region AsignacionFocus

        private void ConfigurarTabIndexYFocus()
        {
            // Filtros
            Txt_ValorBuscado.TabIndex = 0;
            Filtro1.TabIndex = 1;
            Filtro2.TabIndex = 2;
            Filtro3.TabIndex = 3;

            // Detalle
            Txt_Codigo.TabIndex = 5;
            Txt_Articulo.TabIndex = 6;
            Txt_Descripcion.TabIndex = 7;

            Btn_SearchCategory.TabIndex = 8;
            Txt_Category.TabIndex = 9;

            Btn_SearchMeasurementUnits.TabIndex = 10;
            Txt_MeasurementUnits.TabIndex = 11;

            Txt_MinimumStock.TabIndex = 12;
            Txt_MaximumStock.TabIndex = 13;
            Txt_ReorderPoint.TabIndex = 14;
            Txt_UnitCost.TabIndex = 15;
            Txt_LastPurchasePrice.TabIndex = 16;

            ComboBox_HasLotControl.TabIndex = 17;
            ComboBox_HasExpiryDate.TabIndex = 18;

            Btn_Save.TabIndex = 19;
            Btn_Update.TabIndex = 20;
            Btn_Inactive.TabIndex = 21;

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

        private bool ValidarCamposObligatorios()
        {
            if (TienePlaceholder(Txt_Codigo, "CÓDIGO DEL ARTÍCULO"))
            {
                MessageBox.Show("El campo CÓDIGO DEL ARTÍCULO es obligatorio", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Codigo.Focus();
                return false;
            }

            if (TienePlaceholder(Txt_Articulo, "NOMBRE DEL ARTÍCULO"))
            {
                MessageBox.Show("El campo NOMBRE DEL ARTÍCULO es obligatorio", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Articulo.Focus();
                return false;
            }

            if (TienePlaceholder(Txt_Descripcion, "DESCRIPCIÓN DEL ARTÍCULO"))
            {
                MessageBox.Show("El campo DESCRIPCIÓN DEL ARTÍCULO es obligatorio", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Descripcion.Focus();
                return false;
            }

            if (!_categoriaSeleccionadaId.HasValue || _categoriaSeleccionadaId <= 0)
            {
                MessageBox.Show("Debe seleccionar una CATEGORÍA", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Btn_SearchCategory.Focus();
                return false;
            }

            if (!_unidadSeleccionadaId.HasValue || _unidadSeleccionadaId <= 0)
            {
                MessageBox.Show("Debe seleccionar una UNIDAD DE MEDIDA", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Btn_SearchMeasurementUnits.Focus();
                return false;
            }

            return true;
        }

        private decimal ObtenerDecimalDesdeTextBox(TextBox txt, string placeholder, string nombreCampo)
        {
            if (TienePlaceholder(txt, placeholder))
                return 0;

            if (!decimal.TryParse(txt.Text.Trim(), out decimal valor))
            {
                MessageBox.Show($"El valor del campo {nombreCampo} no es válido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt.Focus();
                throw new Exception($"Valor no numérico en {nombreCampo}");
            }

            return valor;
        }

        private Mdl_Items ConvertirArticuloAMayusculas(Mdl_Items item)
        {
            if (item == null) return null;

            item.ItemCode = item.ItemCode?.ToUpper();
            item.ItemName = item.ItemName?.ToUpper();
            item.Description = item.Description?.ToUpper();

            return item;
        }

        #endregion Validaciones
        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCamposObligatorios())
                    return;

                var confirmacion = MessageBox.Show(
                    "¿Está seguro que desea registrar este artículo?",
                    "Confirmar Registro",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                var nuevoItem = new Mdl_Items
                {
                    ItemCode = Ctrl_Items.ObtenerProximoCodigoArticulo(),
                    ItemName = Txt_Articulo.Text.Trim(),
                    Description = Txt_Descripcion.Text.Trim(),
                    CategoryId = _categoriaSeleccionadaId ?? 0,
                    UnitId = _unidadSeleccionadaId ?? 0,

                    MinimumStock = ObtenerDecimalDesdeTextBox(Txt_MinimumStock, "0.00", "STOCK MÍNIMO"),
                    MaximumStock = ObtenerDecimalDesdeTextBox(Txt_MaximumStock, "0.00", "STOCK MÁXIMO"),
                    ReorderPoint = ObtenerDecimalDesdeTextBox(Txt_ReorderPoint, "0.00", "PUNTO DE REORDEN"),

                    UnitCost = ObtenerDecimalDesdeTextBox(Txt_UnitCost, "0.00", "COSTO UNITARIO"),
                    LastPurchasePrice = ObtenerDecimalDesdeTextBox(Txt_LastPurchasePrice, "0.00", "PRECIO ÚLTIMA COMPRA"),

                    HasLotControl = ComboBox_HasLotControl.SelectedItem?.ToString() == "Si",
                    HasExpiryDate = ComboBox_HasExpiryDate.SelectedItem?.ToString() == "Si",

                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy = UserData?.UserId ?? 1
                };

                nuevoItem = ConvertirArticuloAMayusculas(nuevoItem);

                int resultado = Ctrl_Items.RegistrarArticulo(nuevoItem);

                if (resultado > 0)
                {
                    MessageBox.Show("Artículo registrado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ActualizarInfoPaginacion();
                    // Cargar el próximo código automático
                    CargarProximoCodigoItem();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el artículo", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            try
            {
                if (_itemSeleccionado == null || _itemSeleccionado.ItemId == 0)
                {
                    MessageBox.Show("Debe seleccionar un artículo para actualizar", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarCamposObligatorios())
                    return;

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea actualizar el artículo {_itemSeleccionado.ItemName}?",
                    "Confirmar Actualización",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                _itemSeleccionado.ItemName = Txt_Articulo.Text.Trim();
                _itemSeleccionado.Description = Txt_Descripcion.Text.Trim();
                _itemSeleccionado.CategoryId = _categoriaSeleccionadaId ?? 0;
                _itemSeleccionado.UnitId = _unidadSeleccionadaId ?? 0;

                _itemSeleccionado.MinimumStock = ObtenerDecimalDesdeTextBox(Txt_MinimumStock, "0.00", "STOCK MÍNIMO");
                _itemSeleccionado.MaximumStock = ObtenerDecimalDesdeTextBox(Txt_MaximumStock, "0.00", "STOCK MÁXIMO");
                _itemSeleccionado.ReorderPoint = ObtenerDecimalDesdeTextBox(Txt_ReorderPoint, "0.00", "PUNTO DE REORDEN");

                _itemSeleccionado.UnitCost = ObtenerDecimalDesdeTextBox(Txt_UnitCost, "0.00", "COSTO UNITARIO");
                _itemSeleccionado.LastPurchasePrice = ObtenerDecimalDesdeTextBox(Txt_LastPurchasePrice, "0.00", "PRECIO ÚLTIMA COMPRA");

                _itemSeleccionado.HasLotControl = ComboBox_HasLotControl.SelectedItem?.ToString() == "Si";
                _itemSeleccionado.HasExpiryDate = ComboBox_HasExpiryDate.SelectedItem?.ToString() == "Si";

                _itemSeleccionado.ModifiedBy = UserData?.UserId ?? 1;
                _itemSeleccionado.ModifiedDate = DateTime.Now;

                _itemSeleccionado = ConvertirArticuloAMayusculas(_itemSeleccionado);

                int resultado = Ctrl_Items.ActualizarArticulo(_itemSeleccionado);

                if (resultado > 0)
                {
                    MessageBox.Show("Artículo actualizado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ActualizarInfoPaginacion();

                    // Cargar el próximo código automático
                    CargarProximoCodigoItem();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el artículo", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            try
            {
                if (_itemSeleccionado == null || _itemSeleccionado.ItemId == 0)
                {
                    MessageBox.Show("Debe seleccionar un artículo para inactivar", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!_itemSeleccionado.IsActive)
                {
                    MessageBox.Show("Este artículo ya se encuentra inactivo", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea INACTIVAR el artículo {_itemSeleccionado.ItemName}?\n\n" +
                    "El artículo no aparecerá en las listas activas pero sus datos se conservarán.",
                    "Confirmar Inactivación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                int modifiedBy = UserData?.UserId ?? 1;
                int resultado = Ctrl_Items.InactivarArticulo(_itemSeleccionado.ItemId, modifiedBy);

                if (resultado > 0)
                {
                    MessageBox.Show("Artículo inactivado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ActualizarInfoPaginacion();

                    // Cargar el próximo código automático
                    CargarProximoCodigoItem();
                }
                else
                {
                    MessageBox.Show("No se pudo inactivar el artículo", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inactivar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion CRUD
        #region Limpieza

        private void LimpiarFormulario()
        {
            _itemSeleccionado = null;
            _categoriaSeleccionadaId = null;
            _unidadSeleccionadaId = null;

            ConfigurarPlaceHoldersTextbox();
            ConfigurarCombosHas();

            // Cargar el próximo código automático
            CargarProximoCodigoItem();
            Txt_Codigo.Focus();
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        #endregion Limpieza
        #region BuscarCategoria_Unidad

        private void Btn_SearchCategory_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new Frm_KARDEX_SearchCategory())
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        _categoriaSeleccionadaId = frm.SelectedCategoryId;
                        SetTextBoxFromValue(Txt_Category, frm.SelectedCategoryName, "SELECCIONAR CATEGORÍA");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar categoría: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_SearchMeasurementUnits_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new Frm_KARDEX_SearchMeasurementUnits())
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        _unidadSeleccionadaId = frm.SelectedUnitId;
                        SetTextBoxFromValue(Txt_MeasurementUnits, frm.SelectedUnitName, "SELECCIONAR UNIDAD DE MEDIDA");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar unidad de medida: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion BuscarCategoria_Unidad
        #region ExportarExcel
        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                // Si en el futuro agregas permisos para KARDEX, aquí iría:
                /*
                if (!TienePermiso("KARDEX_EXPORT"))
                {
                    MessageBox.Show("NO TIENES PERMISO PARA EXPORTAR ARTÍCULOS",
                                   "ACCESO DENEGADO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                */

                List<Mdl_Items> todosLosArticulos;

                // Indicador de si hay filtros activos
                bool hayFiltros =
                    !string.IsNullOrWhiteSpace(_ultimoTextoBusqueda) ||
                    _ultimaCategoriaFiltroId.HasValue ||
                    _ultimoFiltro2 != "TODOS" ||
                    _ultimoFiltro3 != "TODOS";

                // 1) OBTENER LOS REGISTROS
                // Ctrl_Items.BuscarArticulos SOLO acepta: (string, int?, int, int) :contentReference[oaicite:2]{index=2}
                if (hayFiltros)
                {
                    todosLosArticulos = Ctrl_Items.BuscarArticulos(
                        _ultimoTextoBusqueda,
                        _ultimaCategoriaFiltroId,
                        1,
                        int.MaxValue
                    );
                }
                else
                {
                    todosLosArticulos = Ctrl_Items.MostrarArticulos(1, int.MaxValue);
                }

                if (todosLosArticulos == null || todosLosArticulos.Count == 0)
                {
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR", "INFORMACIÓN",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 2) APLICAR EN MEMORIA LOS FILTROS QUE NO EXISTEN EN EL CONTROLLER

                // Filtro2: Control por lotes (asumiendo que tus combos llenan _ultimoFiltro2 así)
                if (_ultimoFiltro2 == "CON CONTROL DE LOTE")
                    todosLosArticulos = todosLosArticulos.Where(i => i.HasLotControl).ToList();
                else if (_ultimoFiltro2 == "SIN CONTROL DE LOTE")
                    todosLosArticulos = todosLosArticulos.Where(i => !i.HasLotControl).ToList();

                // Filtro3: Estado (activos / inactivos)
                if (_ultimoFiltro3 == "SOLO ACTIVOS")
                    todosLosArticulos = todosLosArticulos.Where(i => i.IsActive).ToList();
                else if (_ultimoFiltro3 == "SOLO INACTIVOS")
                    todosLosArticulos = todosLosArticulos.Where(i => !i.IsActive).ToList();

                if (todosLosArticulos.Count == 0)
                {
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR LUEGO DE APLICAR LOS FILTROS", "INFORMACIÓN",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 3) CATÁLOGOS PARA NOMBRE DE CATEGORÍA Y UNIDAD 
                var categorias = Ctrl_ItemCategories.ObtenerCategoriasParaCombo();
                var unidades = Ctrl_MeasurementUnits.ObtenerUnidadesParaCombo();

                // 4) DIÁLOGO PARA GUARDAR ARCHIVO
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Catálogo de Artículos",
                    FileName = $"KARDEX_Articulos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                this.Cursor = Cursors.WaitCursor;

                var excelApp = new Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                worksheet.Name = "Artículos";

                // 5) TÍTULO
                worksheet.Cells[1, 1] = "CATÁLOGO COMPLETO DE ARTÍCULOS - KARDEX";
                worksheet.Range["A1:Q1"].Merge();
                worksheet.Range["A1:Q1"].Font.Size = 16;
                worksheet.Range["A1:Q1"].Font.Bold = true;
                worksheet.Range["A1:Q1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                worksheet.Range["A1:Q1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                worksheet.Range["A1:Q1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                // 6) INFO ADICIONAL
                worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SECRON"}";
                worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {todosLosArticulos.Count}";

                worksheet.Range["A2:A4"].Font.Size = 10;
                worksheet.Range["A2:A4"].Font.Bold = true;

                // 7) ENCABEZADOS
                int headerRow = 6;

                string[] headers = {
                    "CÓDIGO",
                    "NOMBRE ARTÍCULO",
                    "DESCRIPCIÓN",
                    "CATEGORÍA",
                    "UNIDAD DE MEDIDA",
                    "STOCK MÍNIMO",
                    "STOCK MÁXIMO",
                    "PUNTO REORDEN",
                    "COSTO UNITARIO",
                    "ÚLTIMO PRECIO COMPRA",
                    "CONTROL POR LOTES",
                    "MANEJA CADUCIDAD",
                    "ESTADO",
                    "FECHA CREACIÓN",
                    "USUARIO CREACIÓN",
                    "FECHA MODIFICACIÓN",
                    "USUARIO MODIFICACIÓN"
                };

                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cells[headerRow, i + 1] = headers[i];

                var headerRange = worksheet.Range[$"A{headerRow}:Q{headerRow}"];
                headerRange.Font.Bold = true;
                headerRange.Font.Size = 11;
                headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(51, 140, 255));
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                // 8) LLENAR DATOS
                int row = headerRow + 1;

                foreach (var item in todosLosArticulos)
                {
                    string categoria = categorias.FirstOrDefault(c => c.Key == item.CategoryId).Value ?? "SIN CATEGORÍA";
                    string unidad = unidades.FirstOrDefault(u => u.Key == item.UnitId).Value ?? "SIN UNIDAD";

                    string creadoPor = "N/A";
                    if (item.CreatedBy.HasValue && item.CreatedBy.Value > 0)
                    {
                        creadoPor = Ctrl_Users.ObtenerNombreCompletoPorId(item.CreatedBy.Value);
                        if (string.IsNullOrWhiteSpace(creadoPor))
                            creadoPor = "SIN USUARIO";
                    }

                    string modificadoPor = "N/A";
                    if (item.ModifiedBy.HasValue && item.ModifiedBy.Value > 0)
                    {
                        modificadoPor = Ctrl_Users.ObtenerNombreCompletoPorId(item.ModifiedBy.Value);
                        if (string.IsNullOrWhiteSpace(modificadoPor))
                            modificadoPor = "SIN USUARIO";
                    }

                    worksheet.Cells[row, 1] = item.ItemCode;
                    worksheet.Cells[row, 2] = item.ItemName;
                    worksheet.Cells[row, 3] = item.Description;
                    worksheet.Cells[row, 4] = categoria;
                    worksheet.Cells[row, 5] = unidad;
                    worksheet.Cells[row, 6] = item.MinimumStock.ToString("N2");
                    worksheet.Cells[row, 7] = item.MaximumStock.ToString("N2");
                    worksheet.Cells[row, 8] = item.ReorderPoint.ToString("N2");
                    worksheet.Cells[row, 9] = item.UnitCost.ToString("N2");
                    worksheet.Cells[row, 10] = item.LastPurchasePrice.ToString("N2");
                    worksheet.Cells[row, 11] = item.HasLotControl ? "SI" : "NO";
                    worksheet.Cells[row, 12] = item.HasExpiryDate ? "SI" : "NO";
                    worksheet.Cells[row, 13] = item.IsActive ? "ACTIVO" : "INACTIVO";

                    // 🔴 AQUÍ CORREGIMOS EL ERROR CS0023:
                    // CreatedDate es DateTime NO nullable, así que usamos ToString directo (sin '?')
                    worksheet.Cells[row, 14] = item.CreatedDate.ToString("dd/MM/yyyy HH:mm");

                    worksheet.Cells[row, 15] = creadoPor;
                    worksheet.Cells[row, 16] = item.ModifiedDate.HasValue
                        ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm")
                        : "";
                    worksheet.Cells[row, 17] = modificadoPor;

                    if (row % 2 == 0)
                    {
                        worksheet.Range[$"A{row}:Q{row}"].Interior.Color =
                            System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(240, 240, 240));
                    }

                    row++;
                }

                // 9) BORDES
                var dataRange = worksheet.Range[$"A{headerRow}:Q{row - 1}"];
                dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                // 10) AJUSTE DE COLUMNAS
                worksheet.Columns.AutoFit();
                worksheet.Columns[2].ColumnWidth = 40; // Nombre artículo
                worksheet.Columns[3].ColumnWidth = 50; // Descripción

                // 11) CONGELAR ENCABEZADOS
                worksheet.Activate();
                excelApp.ActiveWindow.SplitRow = headerRow;
                excelApp.ActiveWindow.FreezePanes = true;

                // 12) PIE
                worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
                worksheet.Range[$"A{row + 1}:Q{row + 1}"].Merge();
                worksheet.Range[$"A{row + 1}:Q{row + 1}"].Font.Italic = true;
                worksheet.Range[$"A{row + 1}:Q{row + 1}"].Font.Size = 9;
                worksheet.Range[$"A{row + 1}:Q{row + 1}"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                // 13) GUARDAR Y CERRAR
                workbook.SaveAs(saveFileDialog.FileName);
                workbook.Close();
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                this.Cursor = Cursors.Default;

                var result = MessageBox.Show(
                    "ARCHIVO EXPORTADO EXITOSAMENTE.\n\n¿DESEA ABRIR EL ARCHIVO AHORA?",
                    "EXPORTACIÓN EXITOSA",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
                }
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
