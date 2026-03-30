using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_LocationsInventary_Templates : Form
    {
        #region PropiedadesIniciales

        // Datos del usuario autenticado
        public Mdl_Security_UserInfo UserData { get; set; }

        // Sede activa seleccionada para configurar (lado izquierdo)
        private int _sedeActivaId = 0;
        private string _sedeActivaNombre = "";
        private int _categoriaActivaId = 0;

        // Lista izquierda: stock actual de la sede activa
        private List<Mdl_ItemStockByLocation> _stockEnSede;

        // Cambios pendientes hasta Btn_Yes
        private List<Mdl_ItemStockByLocation> _stockPendienteAgregar = new List<Mdl_ItemStockByLocation>();
        private List<int> _stockIdsParaEliminar = new List<int>();

        // Filtros
        private string _ultimoTextoSede = "";
        private string _ultimoTextoBase = "";

        public Frm_KARDEX_LocationsInventary_Templates()
        {
            InitializeComponent();
        }

        private void Frm_KARDEX_LocationsInventary_Templates_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarTamañoFormulario();
                ConfigurarPlaceHolders();
                ConfigurarFiltros();
                CargarComboSedes();
                ConfigurarTablas();

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
        #region ConfiguracionInicial

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(1200, 900);
            this.MinimumSize = new Size(1200, 900);
            this.MaximumSize = new Size(1200, 900);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        private void ConfigurarPlaceHolders()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR ARTÍCULO EN SEDE...");
            ConfigurarPlaceHolder(Txt_ValorBuscado2, "BUSCAR EN PLANTILLA BASE...");
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

        private void ConfigurarFiltros()
        {
            // --- LADO IZQUIERDO ---
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.Items.Add("POR NOMBRE");
            Filtro1.SelectedIndex = 0;

            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS");
            Filtro2.SelectedIndex = 0;
            Filtro2.Enabled = false;

            Filtro3.Items.Clear();
            Filtro3.Items.Add("TODOS");
            Filtro3.SelectedIndex = 0;
            Filtro3.Enabled = false;

            // --- LADO DERECHO ---
            Filtro4.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro5.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro6.DropDownStyle = ComboBoxStyle.DropDownList;

            Filtro4.Items.Clear();
            Filtro4.Items.Add("TODOS");
            Filtro4.Items.Add("POR CÓDIGO");
            Filtro4.Items.Add("POR NOMBRE");
            Filtro4.SelectedIndex = 0;

            Filtro5.Items.Clear();
            Filtro5.Items.Add("TODOS");
            Filtro5.SelectedIndex = 0;
            Filtro5.Enabled = false;

            Filtro6.Items.Clear();
            Filtro6.Items.Add("TODOS");
            Filtro6.SelectedIndex = 0;
            Filtro6.Enabled = false;
        }

        private void CargarComboSedes()
        {
            var sedes = Ctrl_ItemStockByLocation.ObtenerSedesParaCombo();

            // --- COMBO IZQUIERDO: Sede a configurar ---
            ComboBox_Template.SelectedIndexChanged -= ComboBox_Template_SelectedIndexChanged;
            ComboBox_Template.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Template.Items.Clear();
            ComboBox_Template.Items.Add("SELECCIONAR SEDE A CONFIGURAR...");

            foreach (var sede in sedes)
                ComboBox_Template.Items.Add(new CategoriaItem(sede.Key, sede.Value));

            ComboBox_Template.DisplayMember = "Nombre";
            ComboBox_Template.SelectedIndex = 0;
            ComboBox_Template.SelectedIndexChanged += ComboBox_Template_SelectedIndexChanged;

            // --- COMBO DERECHO: comienza vacío hasta que se seleccione una sede ---
            ComboBox_TemplateBase.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_TemplateBase.Items.Clear();
            ComboBox_TemplateBase.Items.Add("SELECCIONE UNA SEDE PRIMERO...");
            ComboBox_TemplateBase.SelectedIndex = 0;
            ComboBox_TemplateBase.Enabled = false;
        }

        private void CargarComboBase(int locationId)
        {
            ComboBox_TemplateBase.SelectedIndexChanged -= ComboBox_TemplateBase_SelectedIndexChanged;
            ComboBox_TemplateBase.Items.Clear();
            ComboBox_TemplateBase.Items.Add("SELECCIONAR PLANTILLA BASE...");

            // Obtener la categoría de la sede seleccionada
            _categoriaActivaId = Ctrl_ItemStockByLocation.ObtenerCategoriaIdDeSede(locationId);

            // Agregar la plantilla de categoría si existe
            if (_categoriaActivaId > 0)
            {
                var categorias = Ctrl_LocationCategories.ObtenerCategoriasParaCombo();
                var categoriaActiva = categorias.FirstOrDefault(c => c.Key == _categoriaActivaId);
                if (categoriaActiva.Key > 0)
                    ComboBox_TemplateBase.Items.Add(
                        new CategoriaItem(-categoriaActiva.Key, $"PLANTILLA: {categoriaActiva.Value}"));
            }

            // Agregar sedes de la misma categoría (excluyendo la sede activa)
            var sedesMismaCategoria = Ctrl_ItemStockByLocation.ObtenerSedesMismaCategoria(locationId);
            foreach (var sede in sedesMismaCategoria)
                ComboBox_TemplateBase.Items.Add(new CategoriaItem(sede.Key, sede.Value));

            ComboBox_TemplateBase.DisplayMember = "Nombre";
            ComboBox_TemplateBase.Enabled = true;
            ComboBox_TemplateBase.SelectedIndex = 0;
            ComboBox_TemplateBase.SelectedIndexChanged += ComboBox_TemplateBase_SelectedIndexChanged;
        }

        #endregion ConfiguracionInicial
        #region EventosCombos

        private void ComboBox_Template_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_Template.SelectedItem is CategoriaItem sede))
            {
                _sedeActivaId = 0;
                _sedeActivaNombre = "";
                _categoriaActivaId = 0;
                Tabla.DataSource = null;
                Lbl_Conteo.Text = "SELECCIONE UNA SEDE A CONFIGURAR";
                ComboBox_TemplateBase.Enabled = false;
                return;
            }

            // Validar que no sea la misma sede seleccionada en la base
            if (ComboBox_TemplateBase.SelectedItem is CategoriaItem baseActual &&
                baseActual.Id > 0 && baseActual.Id == sede.Id)
            {
                MessageBox.Show("NO PUEDE CONFIGURAR LA MISMA SEDE QUE ESTÁ USANDO COMO BASE.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Template.SelectedIndex = 0;
                return;
            }

            _sedeActivaId = sede.Id;
            _sedeActivaNombre = sede.Nombre;

            _stockPendienteAgregar.Clear();
            _stockIdsParaEliminar.Clear();

            // Recargar combo derecho según la categoría de la sede
            CargarComboBase(_sedeActivaId);

            // Cargar Tabla (izquierda) con el stock actual de la sede
            CargarStockDeSede();
        }

        private void ComboBox_TemplateBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_TemplateBase.SelectedItem is CategoriaItem baseSeleccionada) ||
                ComboBox_TemplateBase.SelectedIndex <= 0)
            {
                Tabla2.DataSource = null;
                Lbl_Conteo2.Text = "SELECCIONE UNA PLANTILLA BASE";
                return;
            }

            // Validar que no sea la misma sede activa
            if (baseSeleccionada.Id > 0 && baseSeleccionada.Id == _sedeActivaId)
            {
                MessageBox.Show("NO PUEDE USAR LA MISMA SEDE COMO PLANTILLA BASE.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_TemplateBase.SelectedIndex = 0;
                return;
            }

            CargarTablaBase(baseSeleccionada);
        }

        #endregion EventosCombos
        #region ConfiguracionTablas

        private void ConfigurarTablas()
        {
            // Tabla izquierda — editable a nivel de DataGridView
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = true;
            Tabla.ReadOnly = false;  // Permite edición
            Tabla.AllowUserToResizeRows = false;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            Tabla2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla2.MultiSelect = true;
            Tabla2.ReadOnly = true;  // Solo lectura siempre
            Tabla2.AllowUserToResizeRows = false;
            Tabla2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        private void CargarStockDeSede()
        {
            if (_sedeActivaId <= 0) return;

            _stockEnSede = Ctrl_ItemStockByLocation.ObtenerStockPorUbicacionConDetalle(_sedeActivaId);

            // Incluir pendientes en memoria
            var pendientes = _stockPendienteAgregar
                .Where(p => !_stockEnSede.Any(s => s.ItemId == p.ItemId))
                .ToList();
            _stockEnSede.AddRange(pendientes);

            // Excluir los marcados para eliminar
            _stockEnSede = _stockEnSede
                .Where(s => !_stockIdsParaEliminar.Contains(s.ItemStockLocationId))
                .ToList();

            AplicarFiltroSede();
            Lbl_Conteo.Text = $"ARTÍCULOS EN SEDE \"{_sedeActivaNombre}\": {_stockEnSede.Count}";
        }

        private void AplicarFiltroSede()
        {
            var datos = _stockEnSede ?? new List<Mdl_ItemStockByLocation>();

            if (!string.IsNullOrWhiteSpace(_ultimoTextoSede))
            {
                string texto = _ultimoTextoSede.ToUpper();
                datos = datos.Where(s =>
                    (s.ItemCode?.ToUpper().Contains(texto) ?? false) ||
                    (s.ItemName?.ToUpper().Contains(texto) ?? false)).ToList();
            }

            var data = datos.Select(s => new
            {
                s.ItemStockLocationId,
                s.ItemId,
                s.ItemCode,
                s.ItemName,
                s.CurrentStock,
                s.ReservedStock,
                s.AvailableStock,
                s.MinimumStock,
                s.MaximumStock
            }).ToList();

            Tabla.DataSource = data;
            ConfigurarColumnaTabla();
        }

        private void ConfigurarColumnaTabla()
        {
            if (Tabla.Columns.Count == 0) return;

            if (Tabla.Columns.Contains("ItemStockLocationId"))
                Tabla.Columns["ItemStockLocationId"].Visible = false;
            if (Tabla.Columns.Contains("ItemId"))
                Tabla.Columns["ItemId"].Visible = false;

            void SetCol(string col, string header, float weight)
            {
                if (!Tabla.Columns.Contains(col)) return;
                Tabla.Columns[col].HeaderText = header;
                Tabla.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns[col].FillWeight = weight;
            }

            SetCol("ItemCode", "CÓDIGO", 12);
            SetCol("ItemName", "NOMBRE DEL ARTÍCULO", 30);
            SetCol("CurrentStock", "STOCK ACTUAL", 12);
            SetCol("ReservedStock", "RESERVADO", 10);
            SetCol("AvailableStock", "DISPONIBLE", 10);
            SetCol("MinimumStock", "STOCK MÍN.", 10);
            SetCol("MaximumStock", "STOCK MÁX.", 10);

            // Columnas de solo lectura explícitas
            if (Tabla.Columns.Contains("ItemCode"))
                Tabla.Columns["ItemCode"].ReadOnly = true;
            if (Tabla.Columns.Contains("ItemName"))
                Tabla.Columns["ItemName"].ReadOnly = true;
            if (Tabla.Columns.Contains("ReservedStock"))
                Tabla.Columns["ReservedStock"].ReadOnly = true;
            if (Tabla.Columns.Contains("AvailableStock"))
                Tabla.Columns["AvailableStock"].ReadOnly = true;

            // CurrentStock, MinimumStock, MaximumStock quedan editables (ReadOnly = false por defecto)
        }

        private void CargarTablaBase(CategoriaItem baseSeleccionada)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Id negativo = es una plantilla de categoría (ItemStockTemplates)
                if (baseSeleccionada.Id < 0)
                {
                    int categoriaId = Math.Abs(baseSeleccionada.Id);
                    var plantilla = Ctrl_ItemStockTemplates.MostrarPlantillasPorCategoria(categoriaId);

                    if (plantilla.Count == 0)
                    {
                        Tabla2.DataSource = null;
                        Lbl_Conteo2.Text = $"\"{baseSeleccionada.Nombre}\" ESTÁ VACÍA";
                        this.Cursor = Cursors.Default;
                        MessageBox.Show($"LA PLANTILLA \"{baseSeleccionada.Nombre}\" NO TIENE ARTÍCULOS CONFIGURADOS.",
                            "PLANTILLA VACÍA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var data = plantilla.Select(p => new
                    {
                        ItemStockLocationId = 0,
                        p.ItemId,
                        p.ItemCode,
                        p.ItemName,
                        CurrentStock = (decimal)0,
                        ReservedStock = (decimal)0,
                        AvailableStock = (decimal)0,
                        p.MinimumStock,
                        p.MaximumStock
                    }).ToList();

                    Tabla2.DataSource = data;
                    ConfigurarColumnaTabla2();
                    Lbl_Conteo2.Text = $"{baseSeleccionada.Nombre}: {plantilla.Count} ARTÍCULOS";
                }
                else
                {
                    // Es otra sede — carga su stock desde ItemStockByLocation
                    var stockBase = Ctrl_ItemStockByLocation
                        .ObtenerStockPorUbicacionConDetalle(baseSeleccionada.Id);

                    if (stockBase.Count == 0)
                    {
                        Tabla2.DataSource = null;
                        Lbl_Conteo2.Text = $"SEDE \"{baseSeleccionada.Nombre}\" SIN ARTÍCULOS";
                        this.Cursor = Cursors.Default;
                        MessageBox.Show($"LA SEDE \"{baseSeleccionada.Nombre}\" NO TIENE ARTÍCULOS CONFIGURADOS.",
                            "SIN ARTÍCULOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var data = stockBase.Select(s => new
                    {
                        s.ItemStockLocationId,
                        s.ItemId,
                        s.ItemCode,
                        s.ItemName,
                        s.CurrentStock,
                        s.ReservedStock,
                        s.AvailableStock,
                        s.MinimumStock,
                        s.MaximumStock
                    }).ToList();

                    Tabla2.DataSource = data;
                    ConfigurarColumnaTabla2();
                    Lbl_Conteo2.Text = $"SEDE \"{baseSeleccionada.Nombre}\": {stockBase.Count} ARTÍCULOS";
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar base: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColumnaTabla2()
        {
            if (Tabla2.Columns.Count == 0) return;

            if (Tabla2.Columns.Contains("ItemStockLocationId"))
                Tabla2.Columns["ItemStockLocationId"].Visible = false;
            if (Tabla2.Columns.Contains("ItemId"))
                Tabla2.Columns["ItemId"].Visible = false;

            void SetCol(string col, string header, float weight)
            {
                if (!Tabla2.Columns.Contains(col)) return;
                Tabla2.Columns[col].HeaderText = header;
                Tabla2.Columns[col].ReadOnly = true;
                Tabla2.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns[col].FillWeight = weight;
            }

            SetCol("ItemCode", "CÓDIGO", 12);
            SetCol("ItemName", "NOMBRE DEL ARTÍCULO", 30);
            SetCol("CurrentStock", "STOCK ACTUAL", 12);
            SetCol("ReservedStock", "RESERVADO", 10);
            SetCol("AvailableStock", "DISPONIBLE", 10);
            SetCol("MinimumStock", "STOCK MÍN.", 10);
            SetCol("MaximumStock", "STOCK MÁX.", 10);
        }

        private void LimpiarTablas()
        {
            Tabla.DataSource = null;
            Tabla2.DataSource = null;
            Lbl_Conteo.Text = "SELECCIONE UNA SEDE";
            Lbl_Conteo2.Text = "SELECCIONE UNA PLANTILLA BASE";
        }

        #endregion ConfiguracionTablas
        #region Search

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                _ultimoTextoSede = (Txt_ValorBuscado.ForeColor != Color.Gray)
                    ? Txt_ValorBuscado.Text.Trim() : "";
                AplicarFiltroSede();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_Search_Click(sender, e); }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR ARTÍCULO EN SEDE...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Filtro1.SelectedIndex = 0;
            _ultimoTextoSede = "";
            AplicarFiltroSede();
        }

        private void Btn_Search2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(ComboBox_TemplateBase.SelectedItem is CategoriaItem baseSeleccionada) ||
                    ComboBox_TemplateBase.SelectedIndex <= 0)
                    return;

                _ultimoTextoBase = (Txt_ValorBuscado2.ForeColor != Color.Gray)
                    ? Txt_ValorBuscado2.Text.Trim().ToUpper() : "";

                CargarTablaBaseConFiltro(baseSeleccionada, _ultimoTextoBase);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Txt_ValorBuscado2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_Search2_Click(sender, e); }
        }

        private void Btn_Clear2_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado2.Text = "BUSCAR EN PLANTILLA BASE...";
            Txt_ValorBuscado2.ForeColor = Color.Gray;
            Filtro4.SelectedIndex = 0;
            _ultimoTextoBase = "";

            if (!(ComboBox_TemplateBase.SelectedItem is CategoriaItem baseSeleccionada) ||
                ComboBox_TemplateBase.SelectedIndex <= 0)
                return;

            CargarTablaBaseConFiltro(baseSeleccionada, "");
        }

        private void CargarTablaBaseConFiltro(CategoriaItem baseSeleccionada, string filtro)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (baseSeleccionada.Id < 0)
                {
                    int categoriaId = Math.Abs(baseSeleccionada.Id);
                    var plantilla = Ctrl_ItemStockTemplates.MostrarPlantillasPorCategoria(categoriaId);

                    if (!string.IsNullOrWhiteSpace(filtro))
                        plantilla = plantilla.Where(p =>
                            (p.ItemCode?.ToUpper().Contains(filtro) ?? false) ||
                            (p.ItemName?.ToUpper().Contains(filtro) ?? false)).ToList();

                    var data = plantilla.Select(p => new
                    {
                        ItemStockLocationId = 0,
                        p.ItemId,
                        p.ItemCode,
                        p.ItemName,
                        CurrentStock = (decimal)0,
                        ReservedStock = (decimal)0,
                        AvailableStock = (decimal)0,
                        p.MinimumStock,
                        p.MaximumStock
                    }).ToList();

                    Tabla2.DataSource = data;
                    ConfigurarColumnaTabla2();
                    Lbl_Conteo2.Text = $"{baseSeleccionada.Nombre}: {data.Count} ARTÍCULOS";
                }
                else
                {
                    var stockBase = Ctrl_ItemStockByLocation
                        .ObtenerStockPorUbicacionConDetalle(baseSeleccionada.Id);

                    if (!string.IsNullOrWhiteSpace(filtro))
                        stockBase = stockBase.Where(s =>
                            (s.ItemCode?.ToUpper().Contains(filtro) ?? false) ||
                            (s.ItemName?.ToUpper().Contains(filtro) ?? false)).ToList();

                    var data = stockBase.Select(s => new
                    {
                        s.ItemStockLocationId,
                        s.ItemId,
                        s.ItemCode,
                        s.ItemName,
                        s.CurrentStock,
                        s.ReservedStock,
                        s.AvailableStock,
                        s.MinimumStock,
                        s.MaximumStock
                    }).ToList();

                    Tabla2.DataSource = data;
                    ConfigurarColumnaTabla2();
                    Lbl_Conteo2.Text = $"SEDE \"{baseSeleccionada.Nombre}\": {data.Count} ARTÍCULOS";
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al filtrar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Search
        #region AccionesArticulos

        private void Btn_CopySelected_Click(object sender, EventArgs e)
        {
            try
            {
                if (_sedeActivaId <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA SEDE A CONFIGURAR PRIMERO.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Tabla2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN ARTÍCULO DE LA PLANTILLA BASE.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow fila in Tabla2.SelectedRows)
                {
                    if (fila.Cells["ItemId"].Value == null) continue;

                    int itemId = (int)fila.Cells["ItemId"].Value;
                    string itemCode = fila.Cells["ItemCode"].Value?.ToString() ?? "";
                    string itemName = fila.Cells["ItemName"].Value?.ToString() ?? "";

                    decimal currentStock = 0, reservedStock = 0, minStock = 0, maxStock = 0;
                    if (fila.Cells["CurrentStock"].Value != null)
                        decimal.TryParse(fila.Cells["CurrentStock"].Value.ToString(), out currentStock);
                    if (fila.Cells["ReservedStock"].Value != null)
                        decimal.TryParse(fila.Cells["ReservedStock"].Value.ToString(), out reservedStock);
                    if (fila.Cells["MinimumStock"].Value != null)
                        decimal.TryParse(fila.Cells["MinimumStock"].Value.ToString(), out minStock);
                    if (fila.Cells["MaximumStock"].Value != null)
                        decimal.TryParse(fila.Cells["MaximumStock"].Value.ToString(), out maxStock);

                    bool yaEnSede = _stockEnSede?.Any(s => s.ItemId == itemId) ?? false;
                    bool yaPendiente = _stockPendienteAgregar.Any(s => s.ItemId == itemId);

                    if (!yaEnSede && !yaPendiente)
                    {
                        _stockPendienteAgregar.Add(new Mdl_ItemStockByLocation
                        {
                            ItemStockLocationId = 0,
                            ItemId = itemId,
                            LocationId = _sedeActivaId,
                            CurrentStock = currentStock,
                            ReservedStock = reservedStock,
                            MinimumStock = minStock,
                            MaximumStock = maxStock,
                            IsActive = true,
                            ItemCode = itemCode,
                            ItemName = itemName
                        });
                    }
                }

                CargarStockDeSede();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al copiar artículo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_CopyAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (_sedeActivaId <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA SEDE A CONFIGURAR PRIMERO.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Tabla2.Rows.Count == 0)
                {
                    MessageBox.Show("LA PLANTILLA BASE NO TIENE ARTÍCULOS PARA COPIAR.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE COPIAR TODOS LOS ARTÍCULOS A LA SEDE \"{_sedeActivaNombre}\"?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                foreach (DataGridViewRow fila in Tabla2.Rows)
                {
                    if (fila.IsNewRow || fila.Cells["ItemId"].Value == null) continue;

                    int itemId = (int)fila.Cells["ItemId"].Value;
                    string itemCode = fila.Cells["ItemCode"].Value?.ToString() ?? "";
                    string itemName = fila.Cells["ItemName"].Value?.ToString() ?? "";

                    decimal currentStock = 0, reservedStock = 0, minStock = 0, maxStock = 0;
                    if (fila.Cells["CurrentStock"].Value != null)
                        decimal.TryParse(fila.Cells["CurrentStock"].Value.ToString(), out currentStock);
                    if (fila.Cells["ReservedStock"].Value != null)
                        decimal.TryParse(fila.Cells["ReservedStock"].Value.ToString(), out reservedStock);
                    if (fila.Cells["MinimumStock"].Value != null)
                        decimal.TryParse(fila.Cells["MinimumStock"].Value.ToString(), out minStock);
                    if (fila.Cells["MaximumStock"].Value != null)
                        decimal.TryParse(fila.Cells["MaximumStock"].Value.ToString(), out maxStock);

                    bool yaEnSede = _stockEnSede?.Any(s => s.ItemId == itemId) ?? false;
                    bool yaPendiente = _stockPendienteAgregar.Any(s => s.ItemId == itemId);

                    if (!yaEnSede && !yaPendiente)
                    {
                        _stockPendienteAgregar.Add(new Mdl_ItemStockByLocation
                        {
                            ItemStockLocationId = 0,
                            ItemId = itemId,
                            LocationId = _sedeActivaId,
                            CurrentStock = currentStock,
                            ReservedStock = reservedStock,
                            MinimumStock = minStock,
                            MaximumStock = maxStock,
                            IsActive = true,
                            ItemCode = itemCode,
                            ItemName = itemName
                        });
                    }
                }

                CargarStockDeSede();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al copiar todos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_RemoveSelected_Click(object sender, EventArgs e)
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN ARTÍCULO DE LA SEDE.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow fila in Tabla.SelectedRows)
                {
                    if (fila.Cells["ItemId"].Value == null) continue;

                    int itemId = (int)fila.Cells["ItemId"].Value;
                    int stockLocationId = fila.Cells["ItemStockLocationId"].Value != null
                        ? (int)fila.Cells["ItemStockLocationId"].Value : 0;

                    var enSede = _stockEnSede?.FirstOrDefault(s => s.ItemId == itemId);
                    if (enSede == null) continue;

                    if (stockLocationId > 0)
                    {
                        // Existe en BD — marcar para eliminar
                        if (!_stockIdsParaEliminar.Contains(stockLocationId))
                            _stockIdsParaEliminar.Add(stockLocationId);
                    }
                    else
                    {
                        // Solo en memoria — quitar de pendientes
                        var pendiente = _stockPendienteAgregar.FirstOrDefault(s => s.ItemId == itemId);
                        if (pendiente != null)
                            _stockPendienteAgregar.Remove(pendiente);
                    }
                }

                CargarStockDeSede();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al quitar artículo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_RemoveAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (_stockEnSede == null || _stockEnSede.Count == 0)
                {
                    MessageBox.Show("LA SEDE NO TIENE ARTÍCULOS CONFIGURADOS.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE QUITAR TODOS LOS ARTÍCULOS DE LA SEDE \"{_sedeActivaNombre}\"?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion != DialogResult.Yes) return;

                foreach (var stock in _stockEnSede)
                {
                    if (stock.ItemStockLocationId > 0 &&
                        !_stockIdsParaEliminar.Contains(stock.ItemStockLocationId))
                        _stockIdsParaEliminar.Add(stock.ItemStockLocationId);
                }

                _stockPendienteAgregar.Clear();
                CargarStockDeSede();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al quitar todos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion AccionesArticulos
        #region GuardarCambios

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            try
            {
                if (_sedeActivaId <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA SEDE ACTIVA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string mensaje = $"SE GUARDARÁN LOS SIGUIENTES CAMBIOS EN LA SEDE \"{_sedeActivaNombre}\":\n\n" +
                    $"• ARTÍCULOS A AGREGAR: {_stockPendienteAgregar.Count}\n" +
                    $"• ARTÍCULOS A ELIMINAR: {_stockIdsParaEliminar.Count}\n\n¿CONFIRMAR CAMBIOS?";

                var confirmacion = MessageBox.Show(mensaje, "CONFIRMAR GUARDADO",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                this.Cursor = Cursors.WaitCursor;

                // Capturar ediciones de la grilla Tabla
                ActualizarStocksDesdeGrilla();

                int errores = 0;

                // Eliminar los marcados
                foreach (int stockId in _stockIdsParaEliminar)
                {
                    int res = Ctrl_ItemStockByLocation.EliminarStockDeUbicacion(stockId);
                    if (res <= 0) errores++;
                }

                // Insertar / actualizar los nuevos
                foreach (var stock in _stockPendienteAgregar)
                {
                    int res = Ctrl_ItemStockByLocation.RegistrarStockInicial(stock);
                    if (res <= 0) errores++;
                }

                this.Cursor = Cursors.Default;

                if (errores == 0)
                    MessageBox.Show("CAMBIOS GUARDADOS EXITOSAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show($"SE GUARDARON LOS CAMBIOS CON {errores} ERROR(ES).", "ADVERTENCIA",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                _stockPendienteAgregar.Clear();
                _stockIdsParaEliminar.Clear();
                CargarStockDeSede();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarStocksDesdeGrilla()
        {
            if (Tabla.Rows.Count == 0) return;

            foreach (DataGridViewRow fila in Tabla.Rows)
            {
                if (fila.IsNewRow) continue;
                if (!Tabla.Columns.Contains("ItemStockLocationId")) break;

                int stockLocationId = 0;
                if (fila.Cells["ItemStockLocationId"].Value != null)
                    int.TryParse(fila.Cells["ItemStockLocationId"].Value.ToString(), out stockLocationId);

                if (stockLocationId <= 0) continue;

                decimal currentStock = 0, minStock = 0, maxStock = 0;

                if (fila.Cells["CurrentStock"].Value != null)
                    decimal.TryParse(fila.Cells["CurrentStock"].Value.ToString(), out currentStock);
                if (fila.Cells["MinimumStock"].Value != null)
                    decimal.TryParse(fila.Cells["MinimumStock"].Value.ToString(), out minStock);
                if (fila.Cells["MaximumStock"].Value != null)
                    decimal.TryParse(fila.Cells["MaximumStock"].Value.ToString(), out maxStock);

                Ctrl_ItemStockByLocation.ActualizarStockCompleto(new Mdl_ItemStockByLocation
                {
                    ItemStockLocationId = stockLocationId,
                    CurrentStock = currentStock,
                    MinimumStock = minStock,
                    MaximumStock = maxStock
                });
            }
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            var confirmacion = MessageBox.Show(
                "¿ESTÁ SEGURO QUE DESEA REVERTIR TODOS LOS CAMBIOS NO GUARDADOS?",
                "CONFIRMAR REVERTIR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes) return;

            _stockPendienteAgregar.Clear();
            _stockIdsParaEliminar.Clear();

            if (_sedeActivaId > 0)
                CargarStockDeSede();
        }

        private void Btn_Match_Click(object sender, EventArgs e)
        {
            // Conciliación — se implementará en una fase posterior
            MessageBox.Show("LA FUNCIÓN DE CONCILIACIÓN ESTARÁ DISPONIBLE PRÓXIMAMENTE.",
                "EN DESARROLLO", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion GuardarCambios
    }
}