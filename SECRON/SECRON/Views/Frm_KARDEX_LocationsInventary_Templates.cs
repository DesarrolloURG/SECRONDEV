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

        // Sede seleccionada (filtro para poblar ComboBox_Warehouse)
        private int _locationActivaId = 0;

        // Bodega activa seleccionada para configurar (lado izquierdo)
        private int _bodegaActivaId = 0;
        private string _bodegaActivaNombre = "";
        private int _categoriaActivaId = 0;

        // Lista izquierda: stock actual de la bodega activa
        private List<Mdl_ItemWarehouseStock> _stockEnBodega;

        // Cambios pendientes hasta Btn_Yes
        private List<Mdl_ItemWarehouseStock> _stockPendienteAgregar = new List<Mdl_ItemWarehouseStock>();
        private List<int> _stockIdsParaEliminar = new List<int>();

        // Filtros
        private string _ultimoTextoBodega = "";
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
                CargarComboLocations();
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
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR ARTÍCULO EN BODEGA...");
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

        // COMBO 1: Sede — todas las sedes activas, sin filtro de permiso
        private void CargarComboLocations()
        {
            var locations = Ctrl_Locations.ObtenerLocationsActivas();

            ComboBox_Location.SelectedIndexChanged -= ComboBox_Location_SelectedIndexChanged;
            ComboBox_Location.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Location.Items.Clear();
            ComboBox_Location.Items.Add("SELECCIONAR SEDE...");

            foreach (var location in locations)
                ComboBox_Location.Items.Add(new CategoriaItem(location.Key, location.Value));

            ComboBox_Location.DisplayMember = "Nombre";
            ComboBox_Location.SelectedIndex = 0;
            ComboBox_Location.SelectedIndexChanged += ComboBox_Location_SelectedIndexChanged;

            // ComboBox_Warehouse comienza vacío hasta seleccionar sede
            ComboBox_Warehouse.SelectedIndexChanged -= ComboBox_Warehouse_SelectedIndexChanged;
            ComboBox_Warehouse.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Warehouse.Items.Clear();
            ComboBox_Warehouse.Items.Add("SELECCIONE UNA SEDE PRIMERO...");
            ComboBox_Warehouse.SelectedIndex = 0;
            ComboBox_Warehouse.Enabled = false;

            // ComboBox_TemplateBase también comienza vacío
            ComboBox_TemplateBase.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_TemplateBase.Items.Clear();
            ComboBox_TemplateBase.Items.Add("SELECCIONE UNA BODEGA PRIMERO...");
            ComboBox_TemplateBase.SelectedIndex = 0;
            ComboBox_TemplateBase.Enabled = false;
        }

        // COMBO 2: Bodega — todas las bodegas activas de la sede seleccionada, sin filtro de permiso
        private void CargarComboWarehouses(int locationId)
        {
            var bodegas = Ctrl_Warehouses.ObtenerBodegasPorLocation(locationId);

            ComboBox_Warehouse.SelectedIndexChanged -= ComboBox_Warehouse_SelectedIndexChanged;
            ComboBox_Warehouse.Items.Clear();
            ComboBox_Warehouse.Items.Add("SELECCIONAR BODEGA A CONFIGURAR...");

            foreach (var bodega in bodegas)
                ComboBox_Warehouse.Items.Add(new CategoriaItem(bodega.Key, bodega.Value));

            ComboBox_Warehouse.DisplayMember = "Nombre";
            ComboBox_Warehouse.Enabled = true;
            ComboBox_Warehouse.SelectedIndex = 0;
            ComboBox_Warehouse.SelectedIndexChanged += ComboBox_Warehouse_SelectedIndexChanged;
        }

        private void CargarComboBase(int warehouseId)
        {
            ComboBox_TemplateBase.SelectedIndexChanged -= ComboBox_TemplateBase_SelectedIndexChanged;
            ComboBox_TemplateBase.Items.Clear();
            ComboBox_TemplateBase.Items.Add("SELECCIONAR PLANTILLA BASE...");

            // Obtener la categoría de la sede padre de la bodega seleccionada
            _categoriaActivaId = Ctrl_Warehouses.ObtenerCategoriaIdDeBodega(warehouseId);

            // Agregar la plantilla de categoría si existe
            if (_categoriaActivaId > 0)
            {
                var categorias = Ctrl_LocationCategories.ObtenerCategoriasParaCombo();
                var categoriaActiva = categorias.FirstOrDefault(c => c.Key == _categoriaActivaId);
                if (categoriaActiva.Key > 0)
                    ComboBox_TemplateBase.Items.Add(
                        new CategoriaItem(-categoriaActiva.Key, $"PLANTILLA: {categoriaActiva.Value}"));
            }

            // Agregar bodegas de la misma categoría (vía sede padre, excluyendo la bodega activa)
            var bodegasMismaCategoria = Ctrl_Warehouses.ObtenerBodegasMismaCategoria(warehouseId);
            foreach (var bodega in bodegasMismaCategoria)
                ComboBox_TemplateBase.Items.Add(new CategoriaItem(bodega.Key, bodega.Value));

            ComboBox_TemplateBase.DisplayMember = "Nombre";
            ComboBox_TemplateBase.Enabled = true;
            ComboBox_TemplateBase.SelectedIndex = 0;
            ComboBox_TemplateBase.SelectedIndexChanged += ComboBox_TemplateBase_SelectedIndexChanged;
        }

        #endregion ConfiguracionInicial
        #region EventosCombos

        private void ComboBox_Location_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_Location.SelectedItem is CategoriaItem location))
            {
                _locationActivaId = 0;
                ComboBox_Warehouse.Items.Clear();
                ComboBox_Warehouse.Items.Add("SELECCIONE UNA SEDE PRIMERO...");
                ComboBox_Warehouse.SelectedIndex = 0;
                ComboBox_Warehouse.Enabled = false;
                return;
            }

            _locationActivaId = location.Id;

            // Reset de bodega activa y plantilla base
            _bodegaActivaId = 0;
            _bodegaActivaNombre = "";
            _categoriaActivaId = 0;
            _stockPendienteAgregar.Clear();
            _stockIdsParaEliminar.Clear();

            Tabla.DataSource = null;
            Tabla2.DataSource = null;
            Lbl_Conteo.Text = "SELECCIONE UNA BODEGA A CONFIGURAR";
            Lbl_Conteo2.Text = "SELECCIONE UNA PLANTILLA BASE";

            ComboBox_TemplateBase.Items.Clear();
            ComboBox_TemplateBase.Items.Add("SELECCIONE UNA BODEGA PRIMERO...");
            ComboBox_TemplateBase.SelectedIndex = 0;
            ComboBox_TemplateBase.Enabled = false;

            CargarComboWarehouses(_locationActivaId);
        }

        private void ComboBox_Warehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_Warehouse.SelectedItem is CategoriaItem bodega))
            {
                _bodegaActivaId = 0;
                _bodegaActivaNombre = "";
                _categoriaActivaId = 0;
                Tabla.DataSource = null;
                Lbl_Conteo.Text = "SELECCIONE UNA BODEGA A CONFIGURAR";
                ComboBox_TemplateBase.Enabled = false;
                return;
            }

            // Validar que no sea la misma bodega seleccionada en la base
            if (ComboBox_TemplateBase.SelectedItem is CategoriaItem baseActual &&
                baseActual.Id > 0 && baseActual.Id == bodega.Id)
            {
                MessageBox.Show("NO PUEDE CONFIGURAR LA MISMA BODEGA QUE ESTÁ USANDO COMO BASE.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Warehouse.SelectedIndex = 0;
                return;
            }

            _bodegaActivaId = bodega.Id;
            _bodegaActivaNombre = bodega.Nombre;

            _stockPendienteAgregar.Clear();
            _stockIdsParaEliminar.Clear();

            // Recargar combo derecho según la categoría de la sede padre de la bodega
            CargarComboBase(_bodegaActivaId);

            // Cargar Tabla (izquierda) con el stock actual de la bodega
            CargarStockDeBodega();
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

            // Validar que no sea la misma bodega activa
            if (baseSeleccionada.Id > 0 && baseSeleccionada.Id == _bodegaActivaId)
            {
                MessageBox.Show("NO PUEDE USAR LA MISMA BODEGA COMO PLANTILLA BASE.",
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

        private void CargarStockDeBodega()
        {
            if (_bodegaActivaId <= 0) return;

            _stockEnBodega = Ctrl_ItemWarehouseStock.ObtenerStockPorBodegaConDetalle(_bodegaActivaId);

            // Incluir pendientes en memoria
            var pendientes = _stockPendienteAgregar
                .Where(p => !_stockEnBodega.Any(s => s.ItemId == p.ItemId))
                .ToList();
            _stockEnBodega.AddRange(pendientes);

            // Excluir los marcados para eliminar
            _stockEnBodega = _stockEnBodega
                .Where(s => !_stockIdsParaEliminar.Contains(s.ItemWarehouseStockId))
                .ToList();

            AplicarFiltroBodega();
            Lbl_Conteo.Text = $"ARTÍCULOS EN BODEGA \"{_bodegaActivaNombre}\": {_stockEnBodega.Count}";
        }

        private void AplicarFiltroBodega()
        {
            var datos = _stockEnBodega ?? new List<Mdl_ItemWarehouseStock>();

            if (!string.IsNullOrWhiteSpace(_ultimoTextoBodega))
            {
                string texto = _ultimoTextoBodega.ToUpper();
                datos = datos.Where(s =>
                    (s.ItemCode?.ToUpper().Contains(texto) ?? false) ||
                    (s.ItemName?.ToUpper().Contains(texto) ?? false)).ToList();
            }

            var data = datos.Select(s => new
            {
                s.ItemWarehouseStockId,
                s.ItemId,
                s.ItemCode,
                s.ItemName,
                s.CurrentStock,
                s.MinimumStock,
                s.MaximumStock
            }).ToList();

            Tabla.DataSource = data;
            ConfigurarColumnaTabla();
        }

        private void ConfigurarColumnaTabla()
        {
            if (Tabla.Columns.Count == 0) return;

            if (Tabla.Columns.Contains("ItemWarehouseStockId"))
                Tabla.Columns["ItemWarehouseStockId"].Visible = false;
            if (Tabla.Columns.Contains("ItemId"))
                Tabla.Columns["ItemId"].Visible = false;

            void SetCol(string col, string header, float weight)
            {
                if (!Tabla.Columns.Contains(col)) return;
                Tabla.Columns[col].HeaderText = header;
                Tabla.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns[col].FillWeight = weight;
            }

            SetCol("ItemCode", "CÓDIGO", 15);
            SetCol("ItemName", "NOMBRE DEL ARTÍCULO", 40);
            SetCol("CurrentStock", "STOCK ACTUAL", 15);
            SetCol("MinimumStock", "STOCK MÍN.", 15);
            SetCol("MaximumStock", "STOCK MÁX.", 15);

            // Columnas de solo lectura explícitas
            if (Tabla.Columns.Contains("ItemCode"))
                Tabla.Columns["ItemCode"].ReadOnly = true;
            if (Tabla.Columns.Contains("ItemName"))
                Tabla.Columns["ItemName"].ReadOnly = true;

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
                        ItemWarehouseStockId = 0,
                        p.ItemId,
                        p.ItemCode,
                        p.ItemName,
                        CurrentStock = (decimal)0,
                        p.MinimumStock,
                        p.MaximumStock
                    }).ToList();

                    Tabla2.DataSource = data;
                    ConfigurarColumnaTabla2();
                    Lbl_Conteo2.Text = $"{baseSeleccionada.Nombre}: {plantilla.Count} ARTÍCULOS";
                }
                else
                {
                    // Es otra bodega — carga su stock desde ItemWarehouseStock
                    var stockBase = Ctrl_ItemWarehouseStock
                        .ObtenerStockPorBodegaConDetalle(baseSeleccionada.Id);

                    if (stockBase.Count == 0)
                    {
                        Tabla2.DataSource = null;
                        Lbl_Conteo2.Text = $"BODEGA \"{baseSeleccionada.Nombre}\" SIN ARTÍCULOS";
                        this.Cursor = Cursors.Default;
                        MessageBox.Show($"LA BODEGA \"{baseSeleccionada.Nombre}\" NO TIENE ARTÍCULOS CONFIGURADOS.",
                            "SIN ARTÍCULOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var data = stockBase.Select(s => new
                    {
                        s.ItemWarehouseStockId,
                        s.ItemId,
                        s.ItemCode,
                        s.ItemName,
                        s.CurrentStock,
                        s.MinimumStock,
                        s.MaximumStock
                    }).ToList();

                    Tabla2.DataSource = data;
                    ConfigurarColumnaTabla2();
                    Lbl_Conteo2.Text = $"BODEGA \"{baseSeleccionada.Nombre}\": {stockBase.Count} ARTÍCULOS";
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

            if (Tabla2.Columns.Contains("ItemWarehouseStockId"))
                Tabla2.Columns["ItemWarehouseStockId"].Visible = false;
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

            SetCol("ItemCode", "CÓDIGO", 15);
            SetCol("ItemName", "NOMBRE DEL ARTÍCULO", 40);
            SetCol("CurrentStock", "STOCK ACTUAL", 15);
            SetCol("MinimumStock", "STOCK MÍN.", 15);
            SetCol("MaximumStock", "STOCK MÁX.", 15);
        }

        private void LimpiarTablas()
        {
            Tabla.DataSource = null;
            Tabla2.DataSource = null;
            Lbl_Conteo.Text = "SELECCIONE UNA BODEGA";
            Lbl_Conteo2.Text = "SELECCIONE UNA PLANTILLA BASE";
        }

        #endregion ConfiguracionTablas
        #region Search

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                _ultimoTextoBodega = (Txt_ValorBuscado.ForeColor != Color.Gray)
                    ? Txt_ValorBuscado.Text.Trim() : "";
                AplicarFiltroBodega();
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
            Txt_ValorBuscado.Text = "BUSCAR ARTÍCULO EN BODEGA...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Filtro1.SelectedIndex = 0;
            _ultimoTextoBodega = "";
            AplicarFiltroBodega();
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
                        ItemWarehouseStockId = 0,
                        p.ItemId,
                        p.ItemCode,
                        p.ItemName,
                        CurrentStock = (decimal)0,
                        p.MinimumStock,
                        p.MaximumStock
                    }).ToList();

                    Tabla2.DataSource = data;
                    ConfigurarColumnaTabla2();
                    Lbl_Conteo2.Text = $"{baseSeleccionada.Nombre}: {data.Count} ARTÍCULOS";
                }
                else
                {
                    var stockBase = Ctrl_ItemWarehouseStock
                        .ObtenerStockPorBodegaConDetalle(baseSeleccionada.Id);

                    if (!string.IsNullOrWhiteSpace(filtro))
                        stockBase = stockBase.Where(s =>
                            (s.ItemCode?.ToUpper().Contains(filtro) ?? false) ||
                            (s.ItemName?.ToUpper().Contains(filtro) ?? false)).ToList();

                    var data = stockBase.Select(s => new
                    {
                        s.ItemWarehouseStockId,
                        s.ItemId,
                        s.ItemCode,
                        s.ItemName,
                        s.CurrentStock,
                        s.MinimumStock,
                        s.MaximumStock
                    }).ToList();

                    Tabla2.DataSource = data;
                    ConfigurarColumnaTabla2();
                    Lbl_Conteo2.Text = $"BODEGA \"{baseSeleccionada.Nombre}\": {data.Count} ARTÍCULOS";
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
                if (_bodegaActivaId <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA BODEGA A CONFIGURAR PRIMERO.",
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

                    decimal currentStock = 0, minStock = 0, maxStock = 0;
                    if (fila.Cells["CurrentStock"].Value != null)
                        decimal.TryParse(fila.Cells["CurrentStock"].Value.ToString(), out currentStock);
                    if (fila.Cells["MinimumStock"].Value != null)
                        decimal.TryParse(fila.Cells["MinimumStock"].Value.ToString(), out minStock);
                    if (fila.Cells["MaximumStock"].Value != null)
                        decimal.TryParse(fila.Cells["MaximumStock"].Value.ToString(), out maxStock);

                    bool yaEnBodega = _stockEnBodega?.Any(s => s.ItemId == itemId) ?? false;
                    bool yaPendiente = _stockPendienteAgregar.Any(s => s.ItemId == itemId);

                    if (!yaEnBodega && !yaPendiente)
                    {
                        _stockPendienteAgregar.Add(new Mdl_ItemWarehouseStock
                        {
                            ItemWarehouseStockId = 0,
                            ItemId = itemId,
                            WarehouseId = _bodegaActivaId,
                            CurrentStock = currentStock,
                            MinimumStock = minStock,
                            MaximumStock = maxStock,
                            ItemCode = itemCode,
                            ItemName = itemName
                        });
                    }
                }

                CargarStockDeBodega();
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
                if (_bodegaActivaId <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA BODEGA A CONFIGURAR PRIMERO.",
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
                    $"¿ESTÁ SEGURO DE COPIAR TODOS LOS ARTÍCULOS A LA BODEGA \"{_bodegaActivaNombre}\"?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                foreach (DataGridViewRow fila in Tabla2.Rows)
                {
                    if (fila.IsNewRow || fila.Cells["ItemId"].Value == null) continue;

                    int itemId = (int)fila.Cells["ItemId"].Value;
                    string itemCode = fila.Cells["ItemCode"].Value?.ToString() ?? "";
                    string itemName = fila.Cells["ItemName"].Value?.ToString() ?? "";

                    decimal currentStock = 0, minStock = 0, maxStock = 0;
                    if (fila.Cells["CurrentStock"].Value != null)
                        decimal.TryParse(fila.Cells["CurrentStock"].Value.ToString(), out currentStock);
                    if (fila.Cells["MinimumStock"].Value != null)
                        decimal.TryParse(fila.Cells["MinimumStock"].Value.ToString(), out minStock);
                    if (fila.Cells["MaximumStock"].Value != null)
                        decimal.TryParse(fila.Cells["MaximumStock"].Value.ToString(), out maxStock);

                    bool yaEnBodega = _stockEnBodega?.Any(s => s.ItemId == itemId) ?? false;
                    bool yaPendiente = _stockPendienteAgregar.Any(s => s.ItemId == itemId);

                    if (!yaEnBodega && !yaPendiente)
                    {
                        _stockPendienteAgregar.Add(new Mdl_ItemWarehouseStock
                        {
                            ItemWarehouseStockId = 0,
                            ItemId = itemId,
                            WarehouseId = _bodegaActivaId,
                            CurrentStock = currentStock,
                            MinimumStock = minStock,
                            MaximumStock = maxStock,
                            ItemCode = itemCode,
                            ItemName = itemName
                        });
                    }
                }

                CargarStockDeBodega();
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
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN ARTÍCULO DE LA BODEGA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow fila in Tabla.SelectedRows)
                {
                    if (fila.Cells["ItemId"].Value == null) continue;

                    int itemId = (int)fila.Cells["ItemId"].Value;
                    int itemWarehouseStockId = fila.Cells["ItemWarehouseStockId"].Value != null
                        ? (int)fila.Cells["ItemWarehouseStockId"].Value : 0;

                    var enBodega = _stockEnBodega?.FirstOrDefault(s => s.ItemId == itemId);
                    if (enBodega == null) continue;

                    if (itemWarehouseStockId > 0)
                    {
                        // Existe en BD — marcar para eliminar
                        if (!_stockIdsParaEliminar.Contains(itemWarehouseStockId))
                            _stockIdsParaEliminar.Add(itemWarehouseStockId);
                    }
                    else
                    {
                        // Solo en memoria — quitar de pendientes
                        var pendiente = _stockPendienteAgregar.FirstOrDefault(s => s.ItemId == itemId);
                        if (pendiente != null)
                            _stockPendienteAgregar.Remove(pendiente);
                    }
                }

                CargarStockDeBodega();
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
                if (_stockEnBodega == null || _stockEnBodega.Count == 0)
                {
                    MessageBox.Show("LA BODEGA NO TIENE ARTÍCULOS CONFIGURADOS.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE QUITAR TODOS LOS ARTÍCULOS DE LA BODEGA \"{_bodegaActivaNombre}\"?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion != DialogResult.Yes) return;

                foreach (var stock in _stockEnBodega)
                {
                    if (stock.ItemWarehouseStockId > 0 &&
                        !_stockIdsParaEliminar.Contains(stock.ItemWarehouseStockId))
                        _stockIdsParaEliminar.Add(stock.ItemWarehouseStockId);
                }

                _stockPendienteAgregar.Clear();
                CargarStockDeBodega();
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
                if (_bodegaActivaId <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA BODEGA ACTIVA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string mensaje = $"SE GUARDARÁN LOS SIGUIENTES CAMBIOS EN LA BODEGA \"{_bodegaActivaNombre}\":\n\n" +
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
                    int res = Ctrl_ItemWarehouseStock.EliminarStockDeBodega(stockId, UserData.UserId);
                    if (res <= 0) errores++;
                }

                // Insertar / actualizar los nuevos
                foreach (var stock in _stockPendienteAgregar)
                {
                    int res = Ctrl_ItemWarehouseStock.RegistrarStockInicial(stock, UserData.UserId);
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
                CargarStockDeBodega();
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
                if (!Tabla.Columns.Contains("ItemWarehouseStockId")) break;

                int itemWarehouseStockId = 0;
                if (fila.Cells["ItemWarehouseStockId"].Value != null)
                    int.TryParse(fila.Cells["ItemWarehouseStockId"].Value.ToString(), out itemWarehouseStockId);

                if (itemWarehouseStockId <= 0) continue;

                decimal currentStock = 0, minStock = 0, maxStock = 0;

                if (fila.Cells["CurrentStock"].Value != null)
                    decimal.TryParse(fila.Cells["CurrentStock"].Value.ToString(), out currentStock);
                if (fila.Cells["MinimumStock"].Value != null)
                    decimal.TryParse(fila.Cells["MinimumStock"].Value.ToString(), out minStock);
                if (fila.Cells["MaximumStock"].Value != null)
                    decimal.TryParse(fila.Cells["MaximumStock"].Value.ToString(), out maxStock);

                Ctrl_ItemWarehouseStock.ActualizarStockCompleto(new Mdl_ItemWarehouseStock
                {
                    ItemWarehouseStockId = itemWarehouseStockId,
                    CurrentStock = currentStock,
                    MinimumStock = minStock,
                    MaximumStock = maxStock
                }, UserData.UserId);
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

            if (_bodegaActivaId > 0)
                CargarStockDeBodega();
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