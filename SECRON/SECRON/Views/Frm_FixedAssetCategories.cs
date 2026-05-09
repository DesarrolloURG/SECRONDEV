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
    public partial class Frm_FixedAssetCategories : Form
    {
        #region PropiedadesIniciales

        public Mdl_Security_UserInfo UserData { get; set; }

        // Filtros
        private string _ultimoTextoBusqueda = "";
        private string _ultimoFiltro1 = "TODOS";
        private string _ultimoFiltroEstado = "SOLO ACTIVOS";

        // Categorías
        private List<Mdl_FixedAssetCategory> _categoriesList;
        private Mdl_FixedAssetCategory _categoriaSeleccionada = null;

        // Cuentas contables
        private int? _accountAccumDepId = null;
        private int? _accountExpenseId = null;
        private string _cuentaEnEdicion = ""; // "ACCUM_DEP" o "EXPENSE"


        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        // Atributos (detalle)
        private List<Mdl_FixedAssetAttributeDefinition> _atributosList;
        private Mdl_FixedAssetAttributeDefinition _atributoSeleccionado = null;

        public Frm_FixedAssetCategories()
        {
            InitializeComponent();

            this.Resize += FormularioResize;
            this.Resize += (s, e) =>
            {
                if (toolStripPaginacion != null)
                    toolStripPaginacion.Location = new Point(this.Width - 300, 225);
            };
        }

        private async void Frm_FixedAssetCategories_Load(object sender, EventArgs e)
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

                CargarCategorias();
                ActualizarInfoPaginacion();
                LimpiarPanelAtributos();

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
            if (TablaAtributos != null && TablaAtributos.DataSource != null)
                TablaAtributos.Refresh();
        }

        #endregion PropiedadesIniciales

        #region ConfigurarTextBox

        private void ConfigurarMaxLengthTextBox()
        {
            Txt_ValorBuscado.MaxLength = 100;
            Txt_CategoryCode.MaxLength = 20;
            Txt_CategoryName.MaxLength = 100;
            Txt_Description.MaxLength = 255;
            Txt_DepreciationYears.MaxLength = 5;
            Txt_AccountAccumDep.MaxLength = 200;
            Txt_AccountExpense.MaxLength = 200;
            Txt_AttributeKey.MaxLength = 50;
            Txt_AttributeLabel.MaxLength = 100;
        }

        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_AccountAccumDep.Enabled = false;
            Txt_AccountExpense.Enabled = false;
        }

        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR POR CÓDIGO O NOMBRE...");
            ConfigurarPlaceHolder(Txt_CategoryCode, "CÓDIGO DE CATEGORÍA");
            ConfigurarPlaceHolder(Txt_CategoryName, "NOMBRE DE CATEGORÍA");
            ConfigurarPlaceHolder(Txt_Description, "DESCRIPCIÓN");
            ConfigurarPlaceHolder(Txt_DepreciationYears, "AÑOS (ej. 5)");
            ConfigurarPlaceHolder(Txt_AccountAccumDep, "SELECCIONAR CUENTA DEP. ACUMULADA");
            ConfigurarPlaceHolder(Txt_AccountExpense, "SELECCIONAR CUENTA GASTO DEP.");
            ConfigurarPlaceHolder(Txt_AttributeKey, "CLAVE (ej. VIN, SERIAL)");
            ConfigurarPlaceHolder(Txt_AttributeLabel, "ETIQUETA DEL ATRIBUTO");
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
            ComboBox_DepreciationMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_DepreciationMethod.Items.Clear();
            ComboBox_DepreciationMethod.Items.Add("LINEA_RECTA");
            //ComboBox_DepreciationMethod.Items.Add("DECLINING_BALANCE");
            //ComboBox_DepreciationMethod.Items.Add("SUM_OF_YEARS");
            ComboBox_DepreciationMethod.SelectedIndex = 0;

            ComboBox_DataType.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_DataType.Items.Clear();
            ComboBox_DataType.Items.Add("TEXTO");
            ComboBox_DataType.Items.Add("NUMERO");
            ComboBox_DataType.Items.Add("FECHA");
            ComboBox_DataType.SelectedIndex = 0;

            ComboBox_IsRequired.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_IsRequired.Items.Clear();
            ComboBox_IsRequired.Items.Add("No");
            ComboBox_IsRequired.Items.Add("Si");
            ComboBox_IsRequired.SelectedIndex = 0;
        }

        #endregion ConfigurarCombos

        #region ConfiguracionesTabla_Categorias

        private void CargarCategorias()
        {
            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
        }

        private void RefrescarListado()
        {
            _categoriesList = Ctrl_FixedAssetCategories.MostrarCategorias(paginaActual, registrosPorPagina);
            AsignarDataSourceCategorias();
        }

        private void ConfigurarTabla()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["CategoryCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["CategoryName"].HeaderText = "NOMBRE";
                Tabla.Columns["Description"].HeaderText = "DESCRIPCIÓN";
                Tabla.Columns["DepreciationMethod"].HeaderText = "MÉTODO DEP.";
                Tabla.Columns["DepreciationYears"].HeaderText = "AÑOS DEP.";
                Tabla.Columns["AccountAccumDepName"].HeaderText = "CTA. DEP. ACUM.";
                Tabla.Columns["AccountExpenseName"].HeaderText = "CTA. GASTO DEP.";
                Tabla.Columns["IsActive"].HeaderText = "ESTADO";
                Tabla.Columns["IsActive"].DisplayIndex = Tabla.Columns.Count - 1;

                Tabla.Columns["AssetCategoryId"].Visible = false;
                Tabla.Columns["AccountAccumDepId"].Visible = false;
                Tabla.Columns["AccountExpenseId"].Visible = false;
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

            if (Tabla.Rows.Count > 0)
                Tabla.Rows[0].Selected = true;

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void AjustarColumnas()
        {
            if (Tabla.Columns.Count == 0) return;

            SetFillWeight(Tabla, "CategoryCode", 8);
            SetFillWeight(Tabla, "CategoryName", 20);
            SetFillWeight(Tabla, "Description", 22);
            SetFillWeight(Tabla, "DepreciationMethod", 13);
            SetFillWeight(Tabla, "DepreciationYears", 7);
            SetFillWeight(Tabla, "AccountAccumDepName", 13);
            SetFillWeight(Tabla, "AccountExpenseName", 13);
            SetFillWeight(Tabla, "IsActive", 7);
        }

        private void SetFillWeight(DataGridView grid, string col, int weight)
        {
            if (!grid.Columns.Contains(col)) return;
            grid.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns[col].FillWeight = weight;
        }

        private void AsignarDataSourceCategorias()
        {
            if (_categoriesList == null) { Tabla.DataSource = null; return; }

            var data = _categoriesList.Select(c => new
            {
                c.AssetCategoryId,
                c.CategoryCode,
                c.CategoryName,
                c.Description,
                c.DepreciationMethod,
                c.DepreciationYears,
                c.AccountAccumDepId,
                c.AccountExpenseId,
                IsActive = c.IsActive ? "ACTIVO" : "INACTIVO",
                c.CreatedDate,
                c.CreatedBy,
                c.ModifiedDate,
                c.ModifiedBy,
                
                AccountAccumDepName = Ctrl_Accounts.ObtenerNombreCuenta(c.AccountAccumDepId),
                AccountExpenseName = Ctrl_Accounts.ObtenerNombreCuenta(c.AccountExpenseId)
            }).ToList();

            Tabla.DataSource = data;
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count > 0)
                CargarDatosCategoriaSeleccionada();
        }

        private void CargarDatosCategoriaSeleccionada()
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0) return;

                int categoryId = Convert.ToInt32(Tabla.SelectedRows[0].Cells["AssetCategoryId"].Value);
                _categoriaSeleccionada = _categoriesList.FirstOrDefault(c => c.AssetCategoryId == categoryId);
                if (_categoriaSeleccionada == null) return;

                SetTextBoxFromValue(Txt_CategoryCode, _categoriaSeleccionada.CategoryCode, "CÓDIGO DE CATEGORÍA");
                SetTextBoxFromValue(Txt_CategoryName, _categoriaSeleccionada.CategoryName, "NOMBRE DE CATEGORÍA");
                SetTextBoxFromValue(Txt_Description, _categoriaSeleccionada.Description, "DESCRIPCIÓN");
                SetTextBoxFromValue(Txt_DepreciationYears, _categoriaSeleccionada.DepreciationYears.ToString("0.0"), "AÑOS (ej. 5)");

                int mi = ComboBox_DepreciationMethod.Items.IndexOf(_categoriaSeleccionada.DepreciationMethod ?? "LINEA_RECTA");
                ComboBox_DepreciationMethod.SelectedIndex = mi >= 0 ? mi : 0;

                _accountAccumDepId = _categoriaSeleccionada.AccountAccumDepId;
                _accountExpenseId = _categoriaSeleccionada.AccountExpenseId;

                var cuentaAccum = Ctrl_Accounts.ObtenerCuentaPorId(_accountAccumDepId ?? 0);
                var cuentaExpense = Ctrl_Accounts.ObtenerCuentaPorId(_accountExpenseId ?? 0);
                SetTextBoxFromValue(Txt_AccountAccumDep, cuentaAccum != null ? $"{cuentaAccum.Code} - {cuentaAccum.Name}" : "", "SELECCIONAR CUENTA DEP. ACUMULADA");
                SetTextBoxFromValue(Txt_AccountExpense, cuentaExpense != null ? $"{cuentaExpense.Code} - {cuentaExpense.Name}" : "", "SELECCIONAR CUENTA GASTO DEP.");

                CargarAtributosDeCategoriaSeleccionada(categoryId);
                Lbl_AtributosHeader.Text = $"ATRIBUTOS DE: {_categoriaSeleccionada.CategoryName.ToUpper()}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar categoría: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetTextBoxFromValue(TextBox txt, string value, string placeholder)
        {
            if (!string.IsNullOrWhiteSpace(value) && value != placeholder)
            { txt.Text = value; txt.ForeColor = Color.Black; }
            else
            { txt.Text = placeholder; txt.ForeColor = Color.Gray; }
        }

        #endregion ConfiguracionesTabla_Categorias

        #region ConfiguracionesTabla_Atributos

        private void CargarAtributosDeCategoriaSeleccionada(int categoryId)
        {
            _atributosList = Ctrl_FixedAssetAttributeDefinitions.MostrarAtributosPorCategoria(categoryId);
            AsignarDataSourceAtributos();
            ConfigurarTablaAtributos();
            AjustarColumnasAtributos();
            LimpiarPanelAtributos();
        }

        private void AsignarDataSourceAtributos()
        {
            if (_atributosList == null) { TablaAtributos.DataSource = null; return; }

            var data = _atributosList.Select(a => new
            {
                a.AttributeDefId,
                a.AssetCategoryId,
                a.AttributeKey,
                a.AttributeLabel,
                a.DataType,
                IsRequired = a.IsRequired ? "Si" : "No",
                IsActive = a.IsActive ? "ACTIVO" : "INACTIVO"
            }).ToList();

            TablaAtributos.DataSource = data;
        }

        private void ConfigurarTablaAtributos()
        {
            if (TablaAtributos.Columns.Count > 0)
            {
                TablaAtributos.Columns["AttributeKey"].HeaderText = "CLAVE";
                TablaAtributos.Columns["AttributeLabel"].HeaderText = "ETIQUETA";
                TablaAtributos.Columns["DataType"].HeaderText = "TIPO";
                TablaAtributos.Columns["IsRequired"].HeaderText = "OBLIGATORIO";
                TablaAtributos.Columns["IsActive"].HeaderText = "ESTADO";

                TablaAtributos.Columns["AttributeDefId"].Visible = false;
                TablaAtributos.Columns["AssetCategoryId"].Visible = false;
            }

            TablaAtributos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TablaAtributos.MultiSelect = false;
            TablaAtributos.ReadOnly = true;
            TablaAtributos.AllowUserToResizeRows = false;
            TablaAtributos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            TablaAtributos.SelectionChanged -= TablaAtributos_SelectionChanged;
            TablaAtributos.SelectionChanged += TablaAtributos_SelectionChanged;
        }

        private void AjustarColumnasAtributos()
        {
            if (TablaAtributos.Columns.Count == 0) return;
            SetFillWeight(TablaAtributos, "AttributeKey", 20);
            SetFillWeight(TablaAtributos, "AttributeLabel", 35);
            SetFillWeight(TablaAtributos, "DataType", 15);
            SetFillWeight(TablaAtributos, "IsRequired", 12);
            SetFillWeight(TablaAtributos, "IsActive", 10);
        }

        private void TablaAtributos_SelectionChanged(object sender, EventArgs e)
        {
            if (TablaAtributos.SelectedRows.Count > 0)
                CargarDatosAtributoSeleccionado();
        }

        private void CargarDatosAtributoSeleccionado()
        {
            try
            {
                if (TablaAtributos.SelectedRows.Count == 0) return;

                int attrId = Convert.ToInt32(TablaAtributos.SelectedRows[0].Cells["AttributeDefId"].Value);
                _atributoSeleccionado = _atributosList.FirstOrDefault(a => a.AttributeDefId == attrId);
                if (_atributoSeleccionado == null) return;

                SetTextBoxFromValue(Txt_AttributeKey, _atributoSeleccionado.AttributeKey, "CLAVE (ej. VIN, SERIAL)");
                SetTextBoxFromValue(Txt_AttributeLabel, _atributoSeleccionado.AttributeLabel, "ETIQUETA DEL ATRIBUTO");

                int di = ComboBox_DataType.Items.IndexOf(_atributoSeleccionado.DataType ?? "TEXT");
                ComboBox_DataType.SelectedIndex = di >= 0 ? di : 0;

                ComboBox_IsRequired.SelectedItem = _atributoSeleccionado.IsRequired ? "Si" : "No";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar atributo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarPanelAtributos()
        {
            _atributoSeleccionado = null;
            SetTextBoxFromValue(Txt_AttributeKey, "", "CLAVE (ej. VIN, SERIAL)");
            SetTextBoxFromValue(Txt_AttributeLabel, "", "ETIQUETA DEL ATRIBUTO");
            ComboBox_DataType.SelectedIndex = 0;
            ComboBox_IsRequired.SelectedIndex = 0;
        }

        #endregion ConfiguracionesTabla_Atributos

        #region Search

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            if (!Btn_Search.Enabled) return;
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string valorBusqueda = TienePlaceholder(Txt_ValorBuscado, "BUSCAR POR CÓDIGO O NOMBRE...")
                    ? "" : Txt_ValorBuscado.Text.Trim();

                _ultimoTextoBusqueda = valorBusqueda;
                _ultimoFiltro1 = Filtro1.SelectedItem?.ToString() ?? "TODOS";
                _ultimoFiltroEstado = FiltroEstado.SelectedItem?.ToString() ?? "TODOS";

                paginaActual = 1;

                _categoriesList = Ctrl_FixedAssetCategories.BuscarCategorias(
                    valorBusqueda, _ultimoFiltro1, _ultimoFiltroEstado,
                    paginaActual, registrosPorPagina);

                AsignarDataSourceCategorias();
                ConfigurarTabla();
                AjustarColumnas();

                totalRegistros = Ctrl_FixedAssetCategories.ContarTotalCategorias(valorBusqueda);
                ActualizarInfoPaginacion();

                if (Tabla.SelectedRows.Count > 0)
                    CargarDatosCategoriaSeleccionada();
                else
                {
                    TablaAtributos.DataSource = null;
                    Lbl_AtributosHeader.Text = "ATRIBUTOS — SELECCIONE UNA CATEGORÍA";
                    LimpiarPanelAtributos();
                }

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

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            if (!Btn_CleanSearch.Enabled) return;

            SetTextBoxFromValue(Txt_ValorBuscado, "", "BUSCAR POR CÓDIGO O NOMBRE...");
            Filtro1.SelectedIndex = 0;
            FiltroEstado.SelectedIndex = 1;

            _ultimoTextoBusqueda = "";
            _ultimoFiltro1 = "TODOS";
            _ultimoFiltroEstado = "SOLO ACTIVOS";
            paginaActual = 1;

            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
            ActualizarInfoPaginacion();
            
            if (Tabla.SelectedRows.Count > 0)
                CargarDatosCategoriaSeleccionada();
            else
            {
                TablaAtributos.DataSource = null;
                Lbl_AtributosHeader.Text = "ATRIBUTOS — SELECCIONE UNA CATEGORÍA";
                LimpiarPanelAtributos();
            }

            ConfigurarControlesPorPermisos();
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
            toolStripPaginacion.Location = new Point(this.Width - 400, 225);

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
                _categoriesList = Ctrl_FixedAssetCategories.BuscarCategorias(
                    _ultimoTextoBusqueda, _ultimoFiltro1, _ultimoFiltroEstado,
                    paginaActual, registrosPorPagina);
                AsignarDataSourceCategorias();
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
                totalRegistros = Ctrl_FixedAssetCategories.ContarTotalCategorias(_ultimoTextoBusqueda);

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;

            ActualizarBotonesNumerados();

            int inicio = (paginaActual - 1) * registrosPorPagina + 1;
            int fin = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            Lbl_Paginas.Text = totalRegistros == 0
                ? "NO HAY CATEGORÍAS PARA MOSTRAR"
                : $"MOSTRANDO {inicio}-{fin} DE {totalRegistros} CATEGORÍAS";
        }

        #endregion ToolStrip_Paginacion

        #region AsignacionFocus

        private void ConfigurarTabIndexYFocus()
        {
            int countIndexTabla = 0;
            Txt_ValorBuscado.TabIndex = countIndexTabla; countIndexTabla++; 
            Filtro1.TabIndex = countIndexTabla; countIndexTabla++;
            FiltroEstado.TabIndex = countIndexTabla; countIndexTabla++;
            Txt_CategoryCode.TabIndex = countIndexTabla; countIndexTabla++;
            Txt_CategoryName.TabIndex = countIndexTabla; countIndexTabla++;
            Txt_Description.TabIndex = countIndexTabla; countIndexTabla++;
            Txt_DepreciationYears.TabIndex = countIndexTabla; countIndexTabla++;
            ComboBox_DepreciationMethod.TabIndex = countIndexTabla; countIndexTabla++;
            Btn_SearchAccountAccumDep.TabIndex = countIndexTabla; countIndexTabla++;
            Txt_AccountAccumDep.TabIndex = countIndexTabla; countIndexTabla++;
            Btn_SearchAccountExpense.TabIndex = countIndexTabla; countIndexTabla++;
            Txt_AccountExpense.TabIndex = countIndexTabla; countIndexTabla++;
            Btn_Save.TabIndex = countIndexTabla; countIndexTabla++;
            Btn_Update.TabIndex = countIndexTabla; countIndexTabla++;
            Btn_Inactive.TabIndex = countIndexTabla; countIndexTabla++;
            Btn_Clear.TabIndex = countIndexTabla; countIndexTabla++;
            Txt_AttributeKey.TabIndex = countIndexTabla; countIndexTabla++;
            Txt_AttributeLabel.TabIndex = countIndexTabla; countIndexTabla++;
            ComboBox_DataType.TabIndex = countIndexTabla; countIndexTabla++;
            ComboBox_IsRequired.TabIndex = countIndexTabla; countIndexTabla++;
            Btn_SaveAtributo.TabIndex = countIndexTabla; countIndexTabla++;
            Btn_UpdateAtributo.TabIndex = countIndexTabla; countIndexTabla++;
            Btn_InactiveAtributo.TabIndex = countIndexTabla; countIndexTabla++;
            Btn_ClearAtributo.TabIndex = countIndexTabla; countIndexTabla++;

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

        private bool ValidarCamposCategoria()
        {
            if (TienePlaceholder(Txt_CategoryCode, "CÓDIGO DE CATEGORÍA"))
            {
                MessageBox.Show("El campo CÓDIGO DE CATEGORÍA es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_CategoryCode.Focus(); return false;
            }
            if (TienePlaceholder(Txt_CategoryName, "NOMBRE DE CATEGORÍA"))
            {
                MessageBox.Show("El campo NOMBRE DE CATEGORÍA es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_CategoryName.Focus(); return false;
            }
            if (TienePlaceholder(Txt_DepreciationYears, "AÑOS (ej. 5)") ||
                !decimal.TryParse(Txt_DepreciationYears.Text.Trim(), out _))
            {
                MessageBox.Show("Ingrese un valor numérico válido en AÑOS DE DEPRECIACIÓN.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_DepreciationYears.Focus(); return false;
            }
            if (!_accountAccumDepId.HasValue || _accountAccumDepId <= 0)
            {
                MessageBox.Show("Debe seleccionar la CUENTA DE DEPRECIACIÓN ACUMULADA.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Btn_SearchAccountAccumDep.Focus(); return false;
            }
            if (!_accountExpenseId.HasValue || _accountExpenseId <= 0)
            {
                MessageBox.Show("Debe seleccionar la CUENTA GASTO DEPRECIACIÓN.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Btn_SearchAccountExpense.Focus(); return false;
            }
            return true;
        }

        private bool ValidarCamposAtributo()
        {
            if (_categoriaSeleccionada == null || _categoriaSeleccionada.AssetCategoryId == 0)
            {
                MessageBox.Show("Seleccione una CATEGORÍA antes de gestionar atributos.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (TienePlaceholder(Txt_AttributeKey, "CLAVE (ej. VIN, SERIAL)"))
            {
                MessageBox.Show("El campo CLAVE DEL ATRIBUTO es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_AttributeKey.Focus(); return false;
            }
            if (TienePlaceholder(Txt_AttributeLabel, "ETIQUETA DEL ATRIBUTO"))
            {
                MessageBox.Show("El campo ETIQUETA DEL ATRIBUTO es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_AttributeLabel.Focus(); return false;
            }
            return true;
        }

        private Mdl_FixedAssetCategory ConvertirCategoria(Mdl_FixedAssetCategory c)
        {
            if (c == null) return null;
            c.CategoryCode = c.CategoryCode?.ToUpper();
            c.CategoryName = c.CategoryName?.ToUpper();
            c.Description = c.Description?.ToUpper();
            return c;
        }

        private Mdl_FixedAssetAttributeDefinition ConvertirAtributo(Mdl_FixedAssetAttributeDefinition a)
        {
            if (a == null) return null;
            a.AttributeKey = a.AttributeKey?.ToUpper();
            a.AttributeLabel = a.AttributeLabel?.ToUpper();
            return a;
        }

        #endregion Validaciones

        #region CRUD_Categorias

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!Btn_Save.Enabled) return;
            try
            {
                if (!ValidarCamposCategoria()) return;
                if (MessageBox.Show("¿Registrar esta categoría?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                decimal.TryParse(Txt_DepreciationYears.Text.Trim(), out decimal depYears);

                var nueva = ConvertirCategoria(new Mdl_FixedAssetCategory
                {
                    CategoryCode = Txt_CategoryCode.Text.Trim(),
                    CategoryName = Txt_CategoryName.Text.Trim(),
                    Description = TienePlaceholder(Txt_Description, "DESCRIPCIÓN") ? null : Txt_Description.Text.Trim(),
                    DepreciationMethod = ComboBox_DepreciationMethod.SelectedItem?.ToString() ?? "LINEA_RECTA",
                    DepreciationYears = depYears,
                    AccountAccumDepId = _accountAccumDepId ?? 0,
                    AccountExpenseId = _accountExpenseId ?? 0,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy = UserData?.UserId ?? 1
                });

                int resultado = Ctrl_FixedAssetCategories.RegistrarCategoria(nueva);

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show("Categoría registrada exitosamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormularioCategoria();
                        RefrescarListado();
                        ActualizarInfoPaginacion();
                        break;
                    case -1:
                        MessageBox.Show("Ya existe una categoría con ese código.", "Duplicado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Txt_CategoryCode.Focus();
                        break;
                    default:
                        MessageBox.Show("No se pudo registrar la categoría.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
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
                if (_categoriaSeleccionada == null || _categoriaSeleccionada.AssetCategoryId == 0)
                {
                    MessageBox.Show("Debe seleccionar una categoría para actualizar.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                }
                if (!ValidarCamposCategoria()) return;
                if (MessageBox.Show($"¿Actualizar la categoría {_categoriaSeleccionada.CategoryName}?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                decimal.TryParse(Txt_DepreciationYears.Text.Trim(), out decimal depYears);

                _categoriaSeleccionada.CategoryCode = Txt_CategoryCode.Text.Trim();
                _categoriaSeleccionada.CategoryName = Txt_CategoryName.Text.Trim();
                _categoriaSeleccionada.Description = TienePlaceholder(Txt_Description, "DESCRIPCIÓN") ? null : Txt_Description.Text.Trim();
                _categoriaSeleccionada.DepreciationMethod = ComboBox_DepreciationMethod.SelectedItem?.ToString() ?? "LINEA_RECTA";
                _categoriaSeleccionada.DepreciationYears = depYears;
                _categoriaSeleccionada.AccountAccumDepId = _accountAccumDepId ?? 0;
                _categoriaSeleccionada.AccountExpenseId = _accountExpenseId ?? 0;
                _categoriaSeleccionada.ModifiedDate = DateTime.Now;
                _categoriaSeleccionada.ModifiedBy = UserData?.UserId ?? 1;
                _categoriaSeleccionada = ConvertirCategoria(_categoriaSeleccionada);

                int resultado = Ctrl_FixedAssetCategories.ActualizarCategoria(_categoriaSeleccionada);

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show("Categoría actualizada exitosamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormularioCategoria();
                        RefrescarListado();
                        ActualizarInfoPaginacion();
                        break;
                    case -1:
                        MessageBox.Show("La categoría no fue encontrada.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case -2:
                        MessageBox.Show("Ya existe otra categoría con ese código.", "Duplicado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Txt_CategoryCode.Focus();
                        break;
                    default:
                        MessageBox.Show("No se pudo actualizar la categoría.", "Error",
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
                if (_categoriaSeleccionada == null || _categoriaSeleccionada.AssetCategoryId == 0)
                {
                    MessageBox.Show("Debe seleccionar una categoría para inactivar.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                }
                if (!_categoriaSeleccionada.IsActive)
                {
                    MessageBox.Show("Esta categoría ya se encuentra inactiva.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information); return;
                }
                if (MessageBox.Show(
                    $"¿INACTIVAR la categoría {_categoriaSeleccionada.CategoryName}?\n\nLos activos asociados no podrán referenciarla.",
                    "Confirmar Inactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;


                _categoriaSeleccionada.IsActive = false;
                _categoriaSeleccionada.ModifiedBy = UserData?.UserId ?? 1;

                int resultado = Ctrl_FixedAssetCategories.ActualizarCategoria(_categoriaSeleccionada);

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show("Categoría inactivada exitosamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormularioCategoria();
                        RefrescarListado();
                        ActualizarInfoPaginacion();
                        break;
                    case -1:
                        MessageBox.Show("La categoría no fue encontrada.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("No se pudo inactivar la categoría.", "Error",
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
            LimpiarFormularioCategoria();
            ConfigurarControlesPorPermisos();
        }

        private void LimpiarFormularioCategoria()
        {
            _categoriaSeleccionada = null;
            _accountAccumDepId = null;
            _accountExpenseId = null;

            ConfigurarPlaceHoldersTextbox();
            ComboBox_DepreciationMethod.SelectedIndex = 0;

            TablaAtributos.DataSource = null;
            Lbl_AtributosHeader.Text = "ATRIBUTOS — SELECCIONE UNA CATEGORÍA";
            LimpiarPanelAtributos();

            Txt_CategoryCode.Focus();
        }

        #endregion CRUD_Categorias

        #region CRUD_Atributos

        private void Btn_SaveAtributo_Click(object sender, EventArgs e)
        {
            if (!Btn_SaveAtributo.Enabled) return;
            try
            {
                if (!ValidarCamposAtributo()) return;
                if (MessageBox.Show("¿Registrar este atributo?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                var nuevo = ConvertirAtributo(new Mdl_FixedAssetAttributeDefinition
                {
                    AssetCategoryId = _categoriaSeleccionada.AssetCategoryId,
                    AttributeKey = Txt_AttributeKey.Text.Trim(),
                    AttributeLabel = Txt_AttributeLabel.Text.Trim(),
                    DataType = ComboBox_DataType.SelectedItem?.ToString() ?? "TEXT",
                    IsRequired = ComboBox_IsRequired.SelectedItem?.ToString() == "Si",
                    IsActive = true
                });

                int resultado = Ctrl_FixedAssetAttributeDefinitions.RegistrarAtributo(nuevo);

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show("Atributo registrado exitosamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarPanelAtributos();
                        CargarAtributosDeCategoriaSeleccionada(_categoriaSeleccionada.AssetCategoryId);
                        break;
                    case -1:
                        MessageBox.Show("Ya existe un atributo con esa clave en esta categoría.", "Duplicado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Txt_AttributeKey.Focus();
                        break;
                    default:
                        MessageBox.Show("No se pudo registrar el atributo.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar atributo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_UpdateAtributo_Click(object sender, EventArgs e)
        {
            if (!Btn_UpdateAtributo.Enabled) return;
            try
            {
                if (_atributoSeleccionado == null || _atributoSeleccionado.AttributeDefId == 0)
                {
                    MessageBox.Show("Debe seleccionar un atributo para actualizar.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                }
                if (!ValidarCamposAtributo()) return;
                if (MessageBox.Show($"¿Actualizar el atributo {_atributoSeleccionado.AttributeLabel}?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                _atributoSeleccionado.AttributeKey = Txt_AttributeKey.Text.Trim();
                _atributoSeleccionado.AttributeLabel = Txt_AttributeLabel.Text.Trim();
                _atributoSeleccionado.DataType = ComboBox_DataType.SelectedItem?.ToString() ?? "TEXT";
                _atributoSeleccionado.IsRequired = ComboBox_IsRequired.SelectedItem?.ToString() == "Si";
                _atributoSeleccionado = ConvertirAtributo(_atributoSeleccionado);

                int resultado = Ctrl_FixedAssetAttributeDefinitions.ActualizarAtributo(_atributoSeleccionado);

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show("Atributo actualizado exitosamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarPanelAtributos();
                        CargarAtributosDeCategoriaSeleccionada(_categoriaSeleccionada.AssetCategoryId);
                        break;
                    case -1:
                        MessageBox.Show("El atributo no fue encontrado.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case -2:
                        MessageBox.Show("Ya existe otro atributo con esa clave en esta categoría.", "Duplicado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Txt_AttributeKey.Focus();
                        break;
                    default:
                        MessageBox.Show("No se pudo actualizar el atributo.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar atributo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_InactiveAtributo_Click(object sender, EventArgs e)
        {
            if (!Btn_InactiveAtributo.Enabled) return;
            try
            {
                if (_atributoSeleccionado == null || _atributoSeleccionado.AttributeDefId == 0)
                {
                    MessageBox.Show("Debe seleccionar un atributo para inactivar.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                }
                if (!_atributoSeleccionado.IsActive)
                {
                    MessageBox.Show("Este atributo ya se encuentra inactivo.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information); return;
                }
                if (MessageBox.Show($"¿INACTIVAR el atributo {_atributoSeleccionado.AttributeLabel}?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

                _atributoSeleccionado.IsActive = false;

                int resultado = Ctrl_FixedAssetAttributeDefinitions.ActualizarAtributo(_atributoSeleccionado);

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show("Atributo inactivado exitosamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarPanelAtributos();
                        CargarAtributosDeCategoriaSeleccionada(_categoriaSeleccionada.AssetCategoryId);
                        break;
                    case -1:
                        MessageBox.Show("El atributo no fue encontrado.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("No se pudo inactivar el atributo.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inactivar atributo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_ClearAtributo_Click(object sender, EventArgs e)
        {
            if (!Btn_ClearAtributo.Enabled) return;
            LimpiarPanelAtributos();
        }

        #endregion CRUD_Atributos

        #region BuscarCuentas

        private void Btn_SearchAccountAccumDep_Click(object sender, EventArgs e)
        {
            if (!Btn_SearchAccountAccumDep.Enabled) return;
            try
            {
                _cuentaEnEdicion = "ACCUM_DEP";
                using (var frm = new Frm_FA_SearchAccount(this))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cuenta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_SearchAccountExpense_Click(object sender, EventArgs e)
        {
            if (!Btn_SearchAccountExpense.Enabled) return;
            try
            {
                _cuentaEnEdicion = "EXPENSE";
                using (var frm = new Frm_FA_SearchAccount(this))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cuenta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void ActualizarCuentaContable(string codigo, string nombre)
        {
            var cuenta = Ctrl_Accounts.ObtenerCuentaPorId(
                Ctrl_Accounts.ObtenerTodasLasCuentas().First(c => c.Code == codigo).AccountId);

            if (_cuentaEnEdicion == "ACCUM_DEP")
            {
                _accountAccumDepId = cuenta?.AccountId;
                SetTextBoxFromValue(Txt_AccountAccumDep, $"{codigo} - {nombre}", "SELECCIONAR CUENTA DEP. ACUMULADA");
            }
            else if (_cuentaEnEdicion == "EXPENSE")
            {
                _accountExpenseId = cuenta?.AccountId;
                SetTextBoxFromValue(Txt_AccountExpense, $"{codigo} - {nombre}", "SELECCIONAR CUENTA GASTO DEP.");
            }
        }

        #endregion BuscarCuentas

        #region ExportarExcel

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            if (!Btn_Export.Enabled) return;
            try
            {
                var todas = Ctrl_FixedAssetCategories.MostrarCategorias(1, int.MaxValue);
                if (todas == null || todas.Count == 0)
                {
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information); return;
                }

                var dlg = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Categorías de Activos Fijos",
                    FileName = $"FA_Categorias_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
                if (dlg.ShowDialog() != DialogResult.OK) return;

                this.Cursor = Cursors.WaitCursor;

                var app = new Excel.Application();
                var wb = app.Workbooks.Add();

                // ── Hoja 1: Categorías ──────────────────────────────
                var ws1 = (Excel.Worksheet)wb.Sheets[1];
                ws1.Name = "Categorías";

                ws1.Cells[1, 1] = "CATEGORÍAS DE ACTIVOS FIJOS - SECRON";
                ws1.Range["A1:I1"].Merge();
                ws1.Range["A1:I1"].Font.Bold = true;
                ws1.Range["A1:I1"].Font.Size = 16;
                ws1.Range["A1:I1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                ws1.Range["A1:I1"].Interior.Color = ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                ws1.Range["A1:I1"].Font.Color = ColorTranslator.ToOle(Color.White);

                ws1.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SECRON"}";
                ws1.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                ws1.Cells[4, 1] = $"TOTAL REGISTROS: {todas.Count}";

                string[] h1 = { "CÓDIGO","NOMBRE","DESCRIPCIÓN","MÉTODO DEP.","AÑOS DEP.",
                                "CTA. ACTIVO","CTA. DEP. ACUM.","CTA. GASTO DEP.","ESTADO" };
                for (int i = 0; i < h1.Length; i++) ws1.Cells[6, i + 1] = h1[i];

                var r1 = ws1.Range["A6:I6"];
                r1.Font.Bold = true;
                r1.Font.Size = 11;
                r1.Font.Color = ColorTranslator.ToOle(Color.White);
                r1.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                r1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                int row = 7;
                foreach (var c in todas)
                {
                    ws1.Cells[row, 1] = c.CategoryCode;
                    ws1.Cells[row, 2] = c.CategoryName;
                    ws1.Cells[row, 3] = c.Description;
                    ws1.Cells[row, 4] = c.DepreciationMethod;
                    ws1.Cells[row, 5] = c.DepreciationYears.ToString("0.0");
                    ws1.Cells[row, 6] = Ctrl_Accounts.ObtenerNombreCuenta(c.AccountAccumDepId);
                    ws1.Cells[row, 7] = Ctrl_Accounts.ObtenerNombreCuenta(c.AccountExpenseId);
                    ws1.Cells[row, 8] = c.IsActive ? "ACTIVO" : "INACTIVO";
                    if (row % 2 == 0)
                        ws1.Range[$"A{row}:I{row}"].Interior.Color = ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                    row++;
                }
                ws1.Range[$"A6:I{row - 1}"].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                ws1.Columns.AutoFit();

                // ── Hoja 2: Atributos ───────────────────────────────
                var ws2 = (Excel.Worksheet)wb.Sheets.Add(After: wb.Sheets[wb.Sheets.Count]);
                ws2.Name = "Atributos";

                ws2.Cells[1, 1] = "DEFINICIÓN DE ATRIBUTOS POR CATEGORÍA - SECRON";
                ws2.Range["A1:F1"].Merge();
                ws2.Range["A1:F1"].Font.Bold = true;
                ws2.Range["A1:F1"].Font.Size = 16;
                ws2.Range["A1:F1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                ws2.Range["A1:F1"].Interior.Color = ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                ws2.Range["A1:F1"].Font.Color = ColorTranslator.ToOle(Color.White);

                string[] h2 = { "CATEGORÍA", "CLAVE", "ETIQUETA", "TIPO DE DATO", "OBLIGATORIO", "ESTADO" };
                for (int i = 0; i < h2.Length; i++) ws2.Cells[3, i + 1] = h2[i];

                var r2 = ws2.Range["A3:F3"];
                r2.Font.Bold = true;
                r2.Font.Size = 11;
                r2.Font.Color = ColorTranslator.ToOle(Color.White);
                r2.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                r2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                int row2 = 4;
                foreach (var c in todas)
                {
                    var attrs = Ctrl_FixedAssetAttributeDefinitions.MostrarAtributosPorCategoria(c.AssetCategoryId);
                    foreach (var a in attrs)
                    {
                        ws2.Cells[row2, 1] = c.CategoryName;
                        ws2.Cells[row2, 2] = a.AttributeKey;
                        ws2.Cells[row2, 3] = a.AttributeLabel;
                        ws2.Cells[row2, 4] = a.DataType;
                        ws2.Cells[row2, 5] = a.IsRequired ? "SI" : "NO";
                        ws2.Cells[row2, 6] = a.IsActive ? "ACTIVO" : "INACTIVO";
                        if (row2 % 2 == 0)
                            ws2.Range[$"A{row2}:F{row2}"].Interior.Color = ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                        row2++;
                    }
                }
                ws2.Range[$"A3:F{row2 - 1}"].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                ws2.Columns.AutoFit();

                wb.SaveAs(dlg.FileName);
                wb.Close();
                app.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(ws1);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ws2);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                this.Cursor = Cursors.Default;

                if (MessageBox.Show("EXPORTADO EXITOSAMENTE. ¿DESEA ABRIRLO AHORA?",
                    "EXPORTACIÓN EXITOSA", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(dlg.FileName);
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
            AplicarEstadoBotonPorPermiso(Btn_Save, "FA_CATEGORIES_CREATE");
            AplicarEstadoBotonPorPermiso(Btn_Update, "FA_CATEGORIES_UPDATE");
            AplicarEstadoBotonPorPermiso(Btn_Inactive, "FA_CATEGORIES_INACTIVE");
            AplicarEstadoBotonPorPermiso(Btn_Export, "FA_CATEGORIES_EXPORT");
            AplicarEstadoBotonPorPermiso(Btn_Search, "FA_CATEGORIES_READ");
            AplicarEstadoBotonPorPermiso(Btn_SaveAtributo, "FA_ATTRIBUTES_CREATE");
            AplicarEstadoBotonPorPermiso(Btn_UpdateAtributo, "FA_ATTRIBUTES_UPDATE");
            AplicarEstadoBotonPorPermiso(Btn_InactiveAtributo, "FA_ATTRIBUTES_INACTIVE");
        }

        #endregion SistemaDePermisos
    }
}