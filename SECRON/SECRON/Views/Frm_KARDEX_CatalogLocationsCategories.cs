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
    public partial class Frm_KARDEX_CatalogLocationsCategories : Form
    {
        #region PropiedadesIniciales

        // Datos del usuario autenticado
        public Mdl_Security_UserInfo UserData { get; set; }

        // Filtros de búsqueda
        private string _ultimoTextoBusqueda = "";
        private string _ultimoFiltro1 = "TODOS";

        // Categoría de sede activa
        private int? _categoriaActivaId = null;
        private string _categoriaActivaNombre = "";

        // Selección actual — ahora es ItemStockTemplates, no Items
        private List<Mdl_ItemStockTemplates> _plantillasList;
        private Mdl_ItemStockTemplates _itemSeleccionado = null;

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        public Frm_KARDEX_CatalogLocationsCategories()
        {
            InitializeComponent();

            this.Resize += (s, e) =>
            {
                if (toolStripPaginacion != null)
                    toolStripPaginacion.Location = new Point(this.Width - 400, 225);
            };
        }

        private void Frm_KARDEX_CatalogLocationsCategories_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarTabIndexYFocus();
                ConfigurarMaxLengthTextBox();
                ConfigurarComponentesDeshabilitados();
                ConfigurarPlaceHoldersTextbox();
                ConfigurarFiltros();
                CrearToolStripPaginacion();        // Primero el toolstrip
                ConfigurarComboCategoria();        // Luego el combo que dispara la carga

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
            // Siempre deshabilitados — datos del item, no se editan aquí
            Txt_Codigo.Enabled = false;
            Txt_Articulo.Enabled = false;
            Txt_Descripcion.Enabled = false;
            Txt_Category.Enabled = false;
            Txt_MeasurementUnits.Enabled = false;
            ComboBox_HasExpiryDate.Enabled = false;

            // Todo deshabilitado hasta que haya categoría seleccionada
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
            // Update y Delete solo se habilitan al seleccionar fila
            Btn_Update.Enabled = false;
            Btn_Delete.Enabled = false;
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

            // Filtro2 y Filtro3 no aplican en plantillas, quedan deshabilitados visualmente
            Filtro2.Items.Clear();
            Filtro2.Items.Add("N/A");
            Filtro2.SelectedIndex = 0;
            Filtro2.Enabled = false;

            Filtro3.Items.Clear();
            Filtro3.Items.Add("N/A");
            Filtro3.SelectedIndex = 0;
            Filtro3.Enabled = false;
        }

        private void ConfigurarComboCategoria()
        {
            ComboBox_Category.SelectedIndexChanged -= ComboBox_Category_SelectedIndexChanged;

            ComboBox_Category.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Category.Items.Clear();

            var categorias = Ctrl_LocationCategories.ObtenerCategoriasParaCombo();

            if (categorias == null || categorias.Count == 0)
            {
                ComboBox_Category.Items.Add("SIN CATEGORÍAS REGISTRADAS");
                ComboBox_Category.SelectedIndex = 0;
                ComboBox_Category.Enabled = false;
                DeshabilitarTodo();

                var respuesta = MessageBox.Show(
                    "NO EXISTEN CATEGORÍAS DE SEDE REGISTRADAS.\n\n" +
                    "DEBE CREAR AL MENOS UNA CATEGORÍA ANTES DE CONTINUAR.\n\n" +
                    "¿DESEA ABRIR LA GESTIÓN DE CATEGORÍAS AHORA?",
                    "SIN CATEGORÍAS", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (respuesta == DialogResult.Yes)
                    AbrirFormularioCategorias();

                VerificarCategoriasTrasCierre();
                return;
            }

            foreach (var cat in categorias)
                ComboBox_Category.Items.Add(new CategoriaItem(cat.Key, cat.Value));

            ComboBox_Category.DisplayMember = "Nombre";
            ComboBox_Category.Enabled = true;
            ComboBox_Category.SelectedIndex = 0;

            ComboBox_Category.SelectedIndexChanged += ComboBox_Category_SelectedIndexChanged;
            ComboBox_Category_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void VerificarCategoriasTrasCierre()
        {
            var categorias = Ctrl_LocationCategories.ObtenerCategoriasParaCombo();
            if (categorias == null || categorias.Count == 0)
            {
                DeshabilitarTodo();
                return;
            }
            ConfigurarComboCategoria();
        }

        private void ComboBox_Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!(ComboBox_Category.SelectedItem is CategoriaItem cat))
                {
                    _categoriaActivaId = null;
                    _categoriaActivaNombre = "";
                    DeshabilitarTodo();
                    return;
                }

                _categoriaActivaId = cat.Id;
                _categoriaActivaNombre = cat.Nombre;

                HabilitarTodo();
                LimpiarFormulario();

                paginaActual = 1;
                totalRegistros = 0;

                CargarPlantilla();
                ActualizarInfoPaginacion();

                if (Ctrl_ItemStockTemplates.ContarTotalPlantillas(_categoriaActivaId.Value) == 0)
                {
                    var respuesta = MessageBox.Show(
                        $"LA CATEGORÍA \"{_categoriaActivaNombre}\" AÚN NO TIENE ARTÍCULOS EN SU PLANTILLA.\n\n" +
                        "¿DESEA CONFIGURAR LA PLANTILLA AHORA?",
                        "PLANTILLA VACÍA", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (respuesta == DialogResult.Yes)
                        AbrirFormularioPlantillas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar categoría: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion ConfigurarFiltros
        #region Subformularios

        private void Btn_Categories_Click(object sender, EventArgs e)
        {
            try
            {
                AbrirFormularioCategorias();
                int? anteriorId = _categoriaActivaId;
                ConfigurarComboCategoria();

                // Intentar mantener la misma categoría seleccionada al volver
                if (anteriorId.HasValue)
                {
                    for (int i = 0; i < ComboBox_Category.Items.Count; i++)
                    {
                        if (ComboBox_Category.Items[i] is CategoriaItem item && item.Id == anteriorId.Value)
                        {
                            ComboBox_Category.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir categorías: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirFormularioCategorias()
        {
            var frm = new Frm_KARDEX_CatalogLocationsCategories_Categories { UserData = this.UserData };
            frm.ShowDialog(this);
        }

        private void AbrirFormularioPlantillas()
        {
            var frm = new Frm_KARDEX_CatalogLocationsCategories_Templates { UserData = this.UserData };
            frm.ShowDialog(this);

            // Refrescar al volver
            if (_categoriaActivaId.HasValue)
            {
                paginaActual = 1;
                totalRegistros = 0;
                CargarPlantilla();
                ActualizarInfoPaginacion();
            }
        }

        private void Btn_Template_Click(object sender, EventArgs e)
        {
            // Abrir gestión de plantillas de stock por categoría
            try
            {
                var frmTemplates = new Frm_KARDEX_CatalogLocationsCategories_Templates
                {
                    UserData = this.UserData
                };
                frmTemplates.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir plantillas: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Subformularios
        #region ConfiguracionesTabla

        private void CargarPlantilla()
        {
            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
        }

        private void RefrescarListado()
        {
            if (!_categoriaActivaId.HasValue)
            {
                _plantillasList = new List<Mdl_ItemStockTemplates>();
                Tabla.DataSource = null;
                return;
            }

            _plantillasList = Ctrl_ItemStockTemplates.BuscarPlantillas(
                locationCategoryId: _categoriaActivaId.Value,
                textoBusqueda: _ultimoTextoBusqueda,
                pageNumber: paginaActual,
                pageSize: registrosPorPagina);

            AsignarDataSource();
        }

        private void AsignarDataSource()
        {
            if (_plantillasList == null)
            {
                Tabla.DataSource = null;
                return;
            }

            var data = _plantillasList.Select(p => new
            {
                p.TemplateId,
                p.ItemId,
                p.ItemCode,
                p.ItemName,
                p.MinimumStock,
                p.MaximumStock,
                p.ReorderPoint
            }).ToList();

            Tabla.DataSource = data;
        }

        private void ConfigurarTabla()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["TemplateId"].Visible = false;
                Tabla.Columns["ItemId"].Visible = false;

                if (Tabla.Columns.Contains("ItemCode"))
                    Tabla.Columns["ItemCode"].HeaderText = "CÓDIGO";
                if (Tabla.Columns.Contains("ItemName"))
                    Tabla.Columns["ItemName"].HeaderText = "NOMBRE DEL ARTÍCULO";
                if (Tabla.Columns.Contains("MinimumStock"))
                    Tabla.Columns["MinimumStock"].HeaderText = "STOCK MÍNIMO";
                if (Tabla.Columns.Contains("MaximumStock"))
                    Tabla.Columns["MaximumStock"].HeaderText = "STOCK MÁXIMO";
                if (Tabla.Columns.Contains("ReorderPoint"))
                    Tabla.Columns["ReorderPoint"].HeaderText = "PUNTO REORDEN";
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

            SetFill("ItemCode", 15);
            SetFill("ItemName", 55);
            SetFill("MinimumStock", 10);
            SetFill("MaximumStock", 10);
            SetFill("ReorderPoint", 10);
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0)
                {
                    _itemSeleccionado = null;
                    LimpiarDetalle();
                    Btn_Update.Enabled = false;
                    Btn_Delete.Enabled = false;
                    return;
                }

                var fila = Tabla.SelectedRows[0];
                int templateId = (int)fila.Cells["TemplateId"].Value;
                _itemSeleccionado = _plantillasList?.FirstOrDefault(p => p.TemplateId == templateId);

                if (_itemSeleccionado != null)
                {
                    CargarDatosEnFormulario();
                    Btn_Update.Enabled = true;
                    Btn_Delete.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar artículo: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDatosEnFormulario()
        {
            if (_itemSeleccionado == null) return;

            SetTextBoxFromValue(Txt_Codigo, _itemSeleccionado.ItemCode, "CÓDIGO DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_Articulo, _itemSeleccionado.ItemName, "NOMBRE DEL ARTÍCULO");
            SetTextBoxFromValue(Txt_MinimumStock, _itemSeleccionado.MinimumStock.ToString("0.00"), "0.00");
            SetTextBoxFromValue(Txt_MaximumStock, _itemSeleccionado.MaximumStock.ToString("0.00"), "0.00");
            SetTextBoxFromValue(Txt_ReorderPoint,
                _itemSeleccionado.ReorderPoint.HasValue
                    ? _itemSeleccionado.ReorderPoint.Value.ToString("0.00")
                    : "0.00", "0.00");
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
            if (toolStripPaginacion == null) return;

            var itemsToRemove = toolStripPaginacion.Items.Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton").ToList();

            foreach (var item in itemsToRemove)
                toolStripPaginacion.Items.Remove(item);

            if (totalPaginas <= 1) return;

            int inicioRango = Math.Max(1, paginaActual - 1);
            int finRango = Math.Min(totalPaginas, paginaActual + 1);
            int posicionInsertar = toolStripPaginacion.Items.IndexOf(btnSiguiente);

            for (int i = inicioRango; i <= finRango; i++)
            {
                var btnPagina = new ToolStripButton
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
                btnPagina.Click += (s, e) => CambiarPagina(numeroPagina);
                toolStripPaginacion.Items.Insert(posicionInsertar++, btnPagina);
            }
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1 || nuevaPagina > totalPaginas) return;

            paginaActual = nuevaPagina;

            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
            ActualizarInfoPaginacion();
        }

        private void ActualizarInfoPaginacion()
        {
            if (toolStripPaginacion == null) return;

            if (!_categoriaActivaId.HasValue)
            {
                if (Lbl_Paginas != null)
                    Lbl_Paginas.Text = "SELECCIONE UNA CATEGORÍA";
                return;
            }

            totalRegistros = Ctrl_ItemStockTemplates.ContarTotalPlantillas(
                _categoriaActivaId.Value, _ultimoTextoBusqueda);

            totalPaginas = totalRegistros > 0
                ? (int)Math.Ceiling((double)totalRegistros / registrosPorPagina)
                : 0;

            if (btnAnterior != null) btnAnterior.Enabled = paginaActual > 1;
            if (btnSiguiente != null) btnSiguiente.Enabled = paginaActual < totalPaginas;

            ActualizarBotonesNumerados();

            if (Lbl_Paginas != null)
            {
                if (totalRegistros == 0)
                {
                    Lbl_Paginas.Text = $"NO HAY ARTÍCULOS EN LA PLANTILLA \"{_categoriaActivaNombre}\"";
                }
                else
                {
                    int inicio = (paginaActual - 1) * registrosPorPagina + 1;
                    int fin = Math.Min(paginaActual * registrosPorPagina, totalRegistros);
                    Lbl_Paginas.Text = $"MOSTRANDO {inicio}-{fin} DE {totalRegistros} ARTÍCULOS";
                }
            }
        }

        #endregion Paginacion
        #region Search

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_categoriaActivaId.HasValue) return;

                this.Cursor = Cursors.WaitCursor;

                _ultimoTextoBusqueda = (Txt_ValorBuscado.ForeColor != Color.Gray)
                    ? Txt_ValorBuscado.Text.Trim() : "";
                _ultimoFiltro1 = Filtro1.SelectedItem?.ToString() ?? "TODOS";

                paginaActual = 1;
                totalRegistros = 0;

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
            Txt_ValorBuscado.Text = "BUSCAR POR CÓDIGO, NOMBRE O DESCRIPCIÓN...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Filtro1.SelectedIndex = 0;

            _ultimoTextoBusqueda = "";
            _ultimoFiltro1 = "TODOS";
            paginaActual = 1;
            totalRegistros = 0;

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
            ComboBox_Category.TabIndex = 4;

            Txt_Articulo.TabIndex = 5;
            Txt_Descripcion.TabIndex = 6;
            Txt_MinimumStock.TabIndex = 7;
            Txt_MaximumStock.TabIndex = 8;
            Txt_ReorderPoint.TabIndex = 9;
            Txt_UnitCost.TabIndex = 10;
            Txt_LastPurchasePrice.TabIndex = 11;

            Btn_Update.TabIndex = 12;
            Btn_Delete.TabIndex = 13;

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
            if (TienePlaceholder(Txt_Articulo, "NOMBRE DEL ARTÍCULO"))
            {
                MessageBox.Show("El campo NOMBRE DEL ARTÍCULO es obligatorio.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Articulo.Focus();
                return false;
            }
            return true;
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

        private Mdl_Items ConvertirAMayusculas(Mdl_Items item)
        {
            if (item == null) return null;
            item.ItemCode = item.ItemCode?.ToUpper();
            item.ItemName = item.ItemName?.ToUpper();
            item.Description = item.Description?.ToUpper();
            return item;
        }

        #endregion Validaciones
        #region CRUD

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                if (_itemSeleccionado == null)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN ARTÍCULO DE LA TABLA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO QUE DESEA ACTUALIZAR LOS STOCKS DE \"{_itemSeleccionado.ItemName}\" " +
                    $"EN LA PLANTILLA \"{_categoriaActivaNombre}\"?",
                    "CONFIRMAR ACTUALIZACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                _itemSeleccionado.MinimumStock = ObtenerDecimalDesdeTextBox(Txt_MinimumStock, "0.00", "STOCK MÍNIMO");
                _itemSeleccionado.MaximumStock = ObtenerDecimalDesdeTextBox(Txt_MaximumStock, "0.00", "STOCK MÁXIMO");
                _itemSeleccionado.ReorderPoint = TienePlaceholder(Txt_ReorderPoint, "0.00")
                    ? (decimal?)null
                    : ObtenerDecimalDesdeTextBox(Txt_ReorderPoint, "0.00", "PUNTO REORDEN");
                _itemSeleccionado.ModifiedBy = UserData?.UserId;

                int resultado = Ctrl_ItemStockTemplates.ActualizarPlantilla(_itemSeleccionado);

                if (resultado > 0)
                {
                    MessageBox.Show("PLANTILLA ACTUALIZADA EXITOSAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    totalRegistros = 0;
                    RefrescarListado();
                    ConfigurarTabla();
                    AjustarColumnas();
                    ActualizarInfoPaginacion();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO ACTUALIZAR LA PLANTILLA.", "ERROR",
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
                if (_itemSeleccionado == null)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN ARTÍCULO DE LA TABLA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO QUE DESEA ELIMINAR \"{_itemSeleccionado.ItemName}\" " +
                    $"DE LA PLANTILLA \"{_categoriaActivaNombre}\"?\n\n" +
                    "ESTA ACCIÓN NO AFECTA EL CATÁLOGO MAESTRO DE ARTÍCULOS.",
                    "CONFIRMAR ELIMINACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion != DialogResult.Yes) return;

                int resultado = Ctrl_ItemStockTemplates.EliminarPlantilla(_itemSeleccionado.TemplateId);

                if (resultado > 0)
                {
                    MessageBox.Show("ARTÍCULO ELIMINADO DE LA PLANTILLA EXITOSAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    totalRegistros = 0;
                    RefrescarListado();
                    ConfigurarTabla();
                    AjustarColumnas();
                    ActualizarInfoPaginacion();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO ELIMINAR EL ARTÍCULO DE LA PLANTILLA.", "ERROR",
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
            _itemSeleccionado = null;
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
            SetTextBoxFromValue(Txt_MinimumStock, "", "0.00");
            SetTextBoxFromValue(Txt_MaximumStock, "", "0.00");
            SetTextBoxFromValue(Txt_ReorderPoint, "", "0.00");
        }

        #endregion CRUD
        #region ExportarExcel

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_categoriaActivaId.HasValue)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CATEGORÍA PRIMERO.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                var todosLosItems = Ctrl_ItemStockTemplates.BuscarPlantillas(
                    locationCategoryId: _categoriaActivaId.Value,
                    textoBusqueda: _ultimoTextoBusqueda,
                    pageNumber: 1,
                    pageSize: int.MaxValue);

                if (todosLosItems.Count == 0)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR.", "INFORMACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Plantilla de Categoría",
                    FileName = $"KARDEX_Plantilla_{_categoriaActivaNombre}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                var excelApp = new Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                worksheet.Name = "Plantilla";

                worksheet.Cells[1, 1] = $"PLANTILLA DE STOCK - {_categoriaActivaNombre.ToUpper()}";
                worksheet.Range["A1:E1"].Merge();
                worksheet.Range["A1:E1"].Font.Size = 16;
                worksheet.Range["A1:E1"].Font.Bold = true;
                worksheet.Range["A1:E1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                worksheet.Range["A1:E1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                worksheet.Range["A1:E1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SECRON"}";
                worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cells[4, 1] = $"TOTAL ARTÍCULOS: {todosLosItems.Count}";

                int headerRow = 6;
                string[] headers = { "CÓDIGO", "NOMBRE ARTÍCULO", "STOCK MÍNIMO", "STOCK MÁXIMO", "PUNTO REORDEN" };

                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cells[headerRow, i + 1] = headers[i];

                var headerRange = worksheet.Range[$"A{headerRow}:E{headerRow}"];
                headerRange.Font.Bold = true;
                headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                int row = headerRow + 1;
                foreach (var p in todosLosItems)
                {
                    worksheet.Cells[row, 1] = p.ItemCode;
                    worksheet.Cells[row, 2] = p.ItemName;
                    worksheet.Cells[row, 3] = p.MinimumStock.ToString("N2");
                    worksheet.Cells[row, 4] = p.MaximumStock.ToString("N2");
                    worksheet.Cells[row, 5] = p.ReorderPoint.HasValue ? p.ReorderPoint.Value.ToString("N2") : "0.00";

                    if (row % 2 == 0)
                        worksheet.Range[$"A{row}:E{row}"].Interior.Color =
                            System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));

                    row++;
                }

                var dataRange = worksheet.Range[$"A{headerRow}:E{row - 1}"];
                dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                worksheet.Columns.AutoFit();
                worksheet.Columns[2].ColumnWidth = 45;

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
    // CLASE AUXILIAR DE APOYO EN PROCEDIMIENTOS DE CATEGORÍAS
    // Permite almacenar el ID y nombre de la categoría para mostrar en el ComboBox
    internal class CategoriaItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public CategoriaItem(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public override string ToString() => Nombre;
    }
}