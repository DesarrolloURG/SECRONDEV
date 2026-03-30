using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_CatalogLocationsCategories_Templates : Form
    {
        #region PropiedadesIniciales

        // Datos del usuario autenticado
        public Mdl_Security_UserInfo UserData { get; set; }

        // Categoría activa seleccionada para editar su plantilla
        private int _categoriaActivaId = 0;
        private string _categoriaActivaNombre = "";

        // Lista izquierda: artículos del catálogo maestro NO asignados a la categoría activa
        private List<Mdl_Items> _articulosDisponibles;

        // Lista derecha: artículos YA en la plantilla de la categoría activa
        private List<Mdl_ItemStockTemplates> _articulosEnPlantilla;

        // Cambios pendientes de guardar (en memoria hasta Btn_Yes)
        private List<Mdl_ItemStockTemplates> _plantillasPendientesAgregar = new List<Mdl_ItemStockTemplates>();
        private List<int> _templateIdsParaEliminar = new List<int>();

        // Filtros búsqueda izquierda
        private string _ultimoTextoDisponibles = "";

        // Filtros búsqueda derecha
        private string _ultimoTextoEnPlantilla = "";

        public Frm_KARDEX_CatalogLocationsCategories_Templates()
        {
            InitializeComponent();
        }

        private void Frm_KARDEX_CatalogLocationsCategories_Templates_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarTamañoFormulario();
                ConfigurarPlaceHolders();
                ConfigurarFiltros();
                CargarComboCategorias();
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
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR ARTÍCULO...");
            ConfigurarPlaceHolder(Txt_ValorBuscado2, "BUSCAR EN PLANTILLA...");
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
            // --- LADO IZQUIERDO (artículos disponibles del catálogo) ---
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            // Filtro1: campo de búsqueda
            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.Items.Add("POR NOMBRE");
            Filtro1.SelectedIndex = 0;

            // Filtro2: clasificación de artículos (categorías de items)
            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODAS LAS CLASIFICACIONES");
            var clasificaciones = Ctrl_ItemCategories.ObtenerCategoriasParaCombo();
            foreach (var c in clasificaciones)
                Filtro2.Items.Add(new ClasificacionItem(c.Key, c.Value));
            Filtro2.DisplayMember = "Nombre";
            Filtro2.SelectedIndex = 0;

            // Filtro3: no aplica en este formulario
            Filtro3.Items.Clear();
            Filtro3.Items.Add("TODOS");
            Filtro3.SelectedIndex = 0;
            Filtro3.Enabled = false;

            // --- LADO DERECHO (artículos en la plantilla activa) ---
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

        private void CargarComboCategorias()
        {
            var categorias = Ctrl_LocationCategories.ObtenerCategoriasParaCombo();

            // --- COMBO IZQUIERDO: Categoría activa (la que vamos a configurar) ---
            ComboBox_Template.SelectedIndexChanged -= ComboBox_Template_SelectedIndexChanged;
            ComboBox_Template.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Template.Items.Clear();
            ComboBox_Template.Items.Add("SELECCIONAR CATEGORÍA A CONFIGURAR...");

            foreach (var cat in categorias)
                ComboBox_Template.Items.Add(new CategoriaItem(cat.Key, cat.Value));

            ComboBox_Template.DisplayMember = "Nombre";
            ComboBox_Template.SelectedIndex = 0;
            ComboBox_Template.SelectedIndexChanged += ComboBox_Template_SelectedIndexChanged;

            // --- COMBO DERECHO: Plantilla base (de dónde copiamos) ---
            ComboBox_TemplateBase.SelectedIndexChanged -= ComboBox_TemplateBase_SelectedIndexChanged;
            ComboBox_TemplateBase.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_TemplateBase.Items.Clear();
            ComboBox_TemplateBase.Items.Add("SELECCIONAR PLANTILLA BASE...");
            ComboBox_TemplateBase.Items.Add(new CategoriaItem(0, "CATÁLOGO PRINCIPAL"));

            foreach (var cat in categorias)
                ComboBox_TemplateBase.Items.Add(new CategoriaItem(cat.Key, cat.Value));

            ComboBox_TemplateBase.DisplayMember = "Nombre";
            ComboBox_TemplateBase.SelectedIndex = 0;
            ComboBox_TemplateBase.SelectedIndexChanged += ComboBox_TemplateBase_SelectedIndexChanged;
        }
        //  cambia la plantilla base, recarga Tabla2
        private void ComboBox_TemplateBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_TemplateBase.SelectedItem is CategoriaItem baseSeleccionada) ||
                ComboBox_TemplateBase.SelectedIndex <= 0)
            {
                Tabla2.DataSource = null;
                Lbl_Conteo2.Text = "SELECCIONE UNA PLANTILLA BASE";
                return;
            }

            // Validar que no sea la misma categoría activa
            if (baseSeleccionada.Id > 0 && baseSeleccionada.Id == _categoriaActivaId)
            {
                MessageBox.Show("NO PUEDE SELECCIONAR LA MISMA CATEGORÍA QUE ESTÁ CONFIGURANDO.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_TemplateBase.SelectedIndex = 0;
                return;
            }

            CargarTablaBase(baseSeleccionada);
        }
        // Cambia la categoría configurada, refresca la tabla
        private void ComboBox_Template_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_Template.SelectedItem is CategoriaItem cat))
            {
                _categoriaActivaId = 0;
                _categoriaActivaNombre = "";
                Tabla.DataSource = null;
                Lbl_Conteo.Text = "SELECCIONE UNA CATEGORÍA A CONFIGURAR";
                return;
            }

            // Validar que no sea la misma que está seleccionada como plantilla base
            if (ComboBox_TemplateBase.SelectedItem is CategoriaItem baseActual &&
                baseActual.Id > 0 && baseActual.Id == cat.Id)
            {
                MessageBox.Show("NO PUEDE CONFIGURAR LA MISMA CATEGORÍA QUE ESTÁ USANDO COMO PLANTILLA BASE.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Template.SelectedIndex = 0;
                return;
            }

            _categoriaActivaId = cat.Id;
            _categoriaActivaNombre = cat.Nombre;

            _plantillasPendientesAgregar.Clear();
            _templateIdsParaEliminar.Clear();

            // Solo recarga Tabla (izquierda) — Tabla2 NO se toca
            CargarArticulosEnPlantilla();
        }
        private void CargarTablaBase(CategoriaItem baseSeleccionada)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (baseSeleccionada.Id == 0)
                {
                    var data = Ctrl_Items.MostrarArticulos(1, 9999);

                    if (data.Count == 0)
                    {
                        Tabla2.DataSource = null;
                        Lbl_Conteo2.Text = "EL CATÁLOGO PRINCIPAL ESTÁ VACÍO";
                        this.Cursor = Cursors.Default;
                        return;
                    }

                    // Incluir stocks para que las columnas sean consistentes con las demás plantillas
                    var dataItems = data.Select(i => new
                    {
                        i.ItemId,
                        i.ItemCode,
                        i.ItemName,
                        MinimumStock = i.MinimumStock,
                        MaximumStock = i.MaximumStock,
                        ReorderPoint = i.ReorderPoint
                    }).ToList();

                    Tabla2.DataSource = dataItems;
                    ConfigurarColumnaTablaBase(esPlantilla: true);
                    Lbl_Conteo2.Text = $"CATÁLOGO GENERAL DE ARTÍCULOS: {data.Count} ARTÍCULOS";
                }
                else
                {
                    var plantilla = Ctrl_ItemStockTemplates.MostrarPlantillasPorCategoria(baseSeleccionada.Id);

                    if (plantilla.Count == 0)
                    {
                        Tabla2.DataSource = null;
                        Lbl_Conteo2.Text = $"LA PLANTILLA \"{baseSeleccionada.Nombre}\" ESTÁ VACÍA";
                        this.Cursor = Cursors.Default;
                        MessageBox.Show($"LA PLANTILLA \"{baseSeleccionada.Nombre}\" NO TIENE ARTÍCULOS CONFIGURADOS.",
                            "PLANTILLA VACÍA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var dataPlantilla = plantilla.Select(p => new
                    {
                        p.ItemId,
                        p.ItemCode,
                        p.ItemName,
                        p.MinimumStock,
                        p.MaximumStock,
                        p.ReorderPoint
                    }).ToList();

                    Tabla2.DataSource = dataPlantilla;
                    ConfigurarColumnaTablaBase(esPlantilla: true);
                    Lbl_Conteo2.Text = $"PLANTILLA \"{baseSeleccionada.Nombre}\": {plantilla.Count} ARTÍCULOS";
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar plantilla base: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConfigurarColumnaTablaBase(bool esPlantilla)
        {
            if (Tabla2.Columns.Count == 0) return;

            if (Tabla2.Columns.Contains("ItemId"))
                Tabla2.Columns["ItemId"].Visible = false;

            if (Tabla2.Columns.Contains("ItemCode"))
            {
                Tabla2.Columns["ItemCode"].HeaderText = "CÓDIGO";
                Tabla2.Columns["ItemCode"].ReadOnly = true;
                Tabla2.Columns["ItemCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["ItemCode"].FillWeight = 20;
            }
            if (Tabla2.Columns.Contains("ItemName"))
            {
                Tabla2.Columns["ItemName"].HeaderText = "NOMBRE DEL ARTÍCULO";
                Tabla2.Columns["ItemName"].ReadOnly = true;
                Tabla2.Columns["ItemName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["ItemName"].FillWeight = esPlantilla ? 40 : 80;
            }

            if (esPlantilla)
            {
                if (Tabla2.Columns.Contains("MinimumStock"))
                {
                    Tabla2.Columns["MinimumStock"].HeaderText = "STOCK MÍN.";
                    Tabla2.Columns["MinimumStock"].ReadOnly = true;
                    Tabla2.Columns["MinimumStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    Tabla2.Columns["MinimumStock"].FillWeight = 13;
                }
                if (Tabla2.Columns.Contains("MaximumStock"))
                {
                    Tabla2.Columns["MaximumStock"].HeaderText = "STOCK MÁX.";
                    Tabla2.Columns["MaximumStock"].ReadOnly = true;
                    Tabla2.Columns["MaximumStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    Tabla2.Columns["MaximumStock"].FillWeight = 13;
                }
                if (Tabla2.Columns.Contains("ReorderPoint"))
                {
                    Tabla2.Columns["ReorderPoint"].HeaderText = "PUNTO REORDEN";
                    Tabla2.Columns["ReorderPoint"].ReadOnly = true;
                    Tabla2.Columns["ReorderPoint"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    Tabla2.Columns["ReorderPoint"].FillWeight = 14;
                }
            }
        }
        #endregion ConfiguracionInicial
        #region ConfiguracionTablas

        private void ConfigurarTablas()
        {
            // Tabla izquierda (artículos disponibles del catálogo)
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = true;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToResizeRows = false;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            // Tabla derecha (artículos en la plantilla activa)
            Tabla2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla2.MultiSelect = true;
            Tabla2.ReadOnly = false;
            Tabla2.AllowUserToResizeRows = false;
            Tabla2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        private void CargarTablasParaCategoria()
        {
            if (_categoriaActivaId <= 0) return;

            this.Cursor = Cursors.WaitCursor;
            CargarArticulosDisponibles();
            CargarArticulosEnPlantilla();
            this.Cursor = Cursors.Default;
        }

        private void CargarArticulosDisponibles()
        {
            // Lista en memoria de artículos del catálogo que NO están en la plantilla activa
            // Se usa para validar duplicados en Btn_CopyAll y Btn_CopySelected
            var idsYaEnPlantilla = ObtenerIdsEnPlantilla();

            _articulosDisponibles = Ctrl_Items.MostrarArticulos(1, 9999)
                .Where(i => !idsYaEnPlantilla.Contains(i.ItemId))
                .ToList();

            // No se asigna a ninguna grilla — es solo referencia en memoria
        }

        private void AplicarFiltroDisponibles()
        {
            var datos = _articulosDisponibles ?? new List<Mdl_Items>();

            if (!string.IsNullOrWhiteSpace(_ultimoTextoDisponibles))
            {
                string texto = _ultimoTextoDisponibles.ToUpper();
                datos = datos.Where(i =>
                    (i.ItemCode?.ToUpper().Contains(texto) ?? false) ||
                    (i.ItemName?.ToUpper().Contains(texto) ?? false)).ToList();
            }

            var data = datos.Select(i => new
            {
                i.ItemId,
                i.ItemCode,
                i.ItemName
            }).ToList();

            Tabla.DataSource = data;
            ConfigurarColumnaTablaDisponibles();
        }

        private void ConfigurarColumnaTablaDisponibles()
        {
            if (Tabla.Columns.Count == 0) return;

            Tabla.Columns["ItemId"].Visible = false;
            if (Tabla.Columns.Contains("ItemCode"))
            {
                Tabla.Columns["ItemCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["ItemCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["ItemCode"].FillWeight = 25;
            }
            if (Tabla.Columns.Contains("ItemName"))
            {
                Tabla.Columns["ItemName"].HeaderText = "NOMBRE DEL ARTÍCULO";
                Tabla.Columns["ItemName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["ItemName"].FillWeight = 75;
            }
        }

        private void CargarArticulosEnPlantilla()
        {
            _articulosEnPlantilla = Ctrl_ItemStockTemplates.MostrarPlantillasPorCategoria(_categoriaActivaId);

            // Incluir los pendientes de agregar en memoria
            var pendientes = _plantillasPendientesAgregar
                .Where(p => !_articulosEnPlantilla.Any(e => e.ItemId == p.ItemId))
                .ToList();
            _articulosEnPlantilla.AddRange(pendientes);

            // Excluir los marcados para eliminar
            _articulosEnPlantilla = _articulosEnPlantilla
                .Where(p => !_templateIdsParaEliminar.Contains(p.TemplateId))
                .ToList();

            AplicarFiltroEnPlantilla();

            Lbl_Conteo.Text = $"ARTÍCULOS EN PLANTILLA \"{_categoriaActivaNombre}\": {_articulosEnPlantilla.Count}";
        }

        private void AplicarFiltroEnPlantilla()
        {
            var datos = _articulosEnPlantilla ?? new List<Mdl_ItemStockTemplates>();

            if (!string.IsNullOrWhiteSpace(_ultimoTextoEnPlantilla))
            {
                string texto = _ultimoTextoEnPlantilla.ToUpper();
                datos = datos.Where(p =>
                    (p.ItemCode?.ToUpper().Contains(texto) ?? false) ||
                    (p.ItemName?.ToUpper().Contains(texto) ?? false)).ToList();
            }

            var data = datos.Select(p => new
            {
                p.TemplateId,
                p.ItemId,
                p.ItemCode,
                p.ItemName,
                p.MinimumStock,
                p.MaximumStock,
                p.ReorderPoint
            }).ToList();

            // Tabla izquierda — esta es la plantilla activa que se está configurando
            Tabla.DataSource = data;
            ConfigurarColumnaTabla();
        }
        private void ConfigurarColumnaTabla()
        {
            if (Tabla.Columns.Count == 0) return;

            if (Tabla.Columns.Contains("TemplateId"))
                Tabla.Columns["TemplateId"].Visible = false;
            if (Tabla.Columns.Contains("ItemId"))
                Tabla.Columns["ItemId"].Visible = false;

            if (Tabla.Columns.Contains("ItemCode"))
            {
                Tabla.Columns["ItemCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["ItemCode"].ReadOnly = true;
                Tabla.Columns["ItemCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["ItemCode"].FillWeight = 15;
            }
            if (Tabla.Columns.Contains("ItemName"))
            {
                Tabla.Columns["ItemName"].HeaderText = "NOMBRE DEL ARTÍCULO";
                Tabla.Columns["ItemName"].ReadOnly = true;
                Tabla.Columns["ItemName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["ItemName"].FillWeight = 43;
            }
            if (Tabla.Columns.Contains("MinimumStock"))
            {
                Tabla.Columns["MinimumStock"].HeaderText = "STOCK MÍN.";
                Tabla.Columns["MinimumStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["MinimumStock"].FillWeight = 14;
            }
            if (Tabla.Columns.Contains("MaximumStock"))
            {
                Tabla.Columns["MaximumStock"].HeaderText = "STOCK MÁX.";
                Tabla.Columns["MaximumStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["MaximumStock"].FillWeight = 14;
            }
            if (Tabla.Columns.Contains("ReorderPoint"))
            {
                Tabla.Columns["ReorderPoint"].HeaderText = "PUNTO REORDEN";
                Tabla.Columns["ReorderPoint"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["ReorderPoint"].FillWeight = 14;
            }
        }
        private void ConfigurarColumnaTablaPlantilla()
        {
            if (Tabla2.Columns.Count == 0) return;

            Tabla2.Columns["TemplateId"].Visible = false;
            Tabla2.Columns["ItemId"].Visible = false;

            if (Tabla2.Columns.Contains("ItemCode"))
            {
                Tabla2.Columns["ItemCode"].HeaderText = "CÓDIGO";
                Tabla2.Columns["ItemCode"].ReadOnly = true;
                Tabla2.Columns["ItemCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["ItemCode"].FillWeight = 15;
            }
            if (Tabla2.Columns.Contains("ItemName"))
            {
                Tabla2.Columns["ItemName"].HeaderText = "ARTÍCULO";
                Tabla2.Columns["ItemName"].ReadOnly = true;
                Tabla2.Columns["ItemName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["ItemName"].FillWeight = 45;
            }
            if (Tabla2.Columns.Contains("MinimumStock"))
            {
                Tabla2.Columns["MinimumStock"].HeaderText = "STOCK MÍN.";
                Tabla2.Columns["MinimumStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["MinimumStock"].FillWeight = 13;
            }
            if (Tabla2.Columns.Contains("MaximumStock"))
            {
                Tabla2.Columns["MaximumStock"].HeaderText = "STOCK MÁX.";
                Tabla2.Columns["MaximumStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["MaximumStock"].FillWeight = 13;
            }
            if (Tabla2.Columns.Contains("ReorderPoint"))
            {
                Tabla2.Columns["ReorderPoint"].HeaderText = "PUNTO REORDEN";
                Tabla2.Columns["ReorderPoint"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["ReorderPoint"].FillWeight = 14;
            }
        }

        private HashSet<int> ObtenerIdsEnPlantilla()
        {
            var idsDB = Ctrl_ItemStockTemplates.MostrarPlantillasPorCategoria(_categoriaActivaId)
                .Where(p => !_templateIdsParaEliminar.Contains(p.TemplateId))
                .Select(p => p.ItemId);

            var idsPendientes = _plantillasPendientesAgregar.Select(p => p.ItemId);

            return new HashSet<int>(idsDB.Union(idsPendientes));
        }

        private void LimpiarTablas()
        {
            Tabla.DataSource = null;
            Tabla2.DataSource = null;
            Lbl_Conteo.Text = "MOSTRANDO 0 ARTÍCULOS DISPONIBLES";
            Lbl_Conteo2.Text = "MOSTRANDO 0 ARTÍCULOS EN PLANTILLA";
        }

        private void ActualizarConteos()
        {
            int disponibles = _articulosDisponibles?.Count ?? 0;
            Lbl_Conteo.Text = $"ARTÍCULOS DISPONIBLES: {disponibles}";
            // Lbl_Conteo2 lo manejan CargarTablaBase y CargarArticulosEnPlantilla de forma independiente
        }

        #endregion ConfiguracionTablas
        #region Search

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                _ultimoTextoDisponibles = (Txt_ValorBuscado.ForeColor != Color.Gray)
                    ? Txt_ValorBuscado.Text.Trim() : "";

                ActualizarConteos();
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
            Txt_ValorBuscado.Text = "BUSCAR ARTÍCULO...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 0;
            _ultimoTextoDisponibles = "";
            ActualizarConteos();
        }

        private void Btn_Search2_Click(object sender, EventArgs e)
        {
            try
            {
                // Busca en Tabla2 (plantilla base derecha) filtrando el DataSource actual
                string texto = (Txt_ValorBuscado2.ForeColor != Color.Gray)
                    ? Txt_ValorBuscado2.Text.Trim().ToUpper() : "";

                if (Tabla2.DataSource == null) return;

                // Refrescar la plantilla base con el filtro aplicado
                if (!(ComboBox_TemplateBase.SelectedItem is CategoriaItem baseSeleccionada) ||
                    ComboBox_TemplateBase.SelectedIndex <= 0)
                    return;

                _ultimoTextoEnPlantilla = texto;
                CargarTablaBaseConFiltro(baseSeleccionada, texto);
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
            Txt_ValorBuscado2.Text = "BUSCAR EN PLANTILLA...";
            Txt_ValorBuscado2.ForeColor = Color.Gray;
            Filtro4.SelectedIndex = 0;
            Filtro5.SelectedIndex = 0;
            Filtro6.SelectedIndex = 0;
            _ultimoTextoEnPlantilla = "";

            if (!(ComboBox_TemplateBase.SelectedItem is CategoriaItem baseSeleccionada) ||
                ComboBox_TemplateBase.SelectedIndex <= 0)
                return;

            CargarTablaBaseConFiltro(baseSeleccionada, "");
        }
        private void CargarTablaBaseConFiltro(CategoriaItem baseSeleccionada, string filtroTexto)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (baseSeleccionada.Id == 0)
                {
                    var data = Ctrl_Items.MostrarArticulos(1, 9999);

                    if (!string.IsNullOrWhiteSpace(filtroTexto))
                        data = data.Where(i =>
                            (i.ItemCode?.ToUpper().Contains(filtroTexto) ?? false) ||
                            (i.ItemName?.ToUpper().Contains(filtroTexto) ?? false)).ToList();

                    var dataItems = data.Select(i => new { i.ItemId, i.ItemCode, i.ItemName }).ToList();
                    Tabla2.DataSource = dataItems;
                    ConfigurarColumnaTablaBase(esPlantilla: false);
                    Lbl_Conteo2.Text = $"CATÁLOGO GENERAL DE ARTÍCULOS: {data.Count} ARTÍCULOS";
                }
                else
                {
                    var plantilla = Ctrl_ItemStockTemplates.MostrarPlantillasPorCategoria(baseSeleccionada.Id);

                    if (!string.IsNullOrWhiteSpace(filtroTexto))
                        plantilla = plantilla.Where(p =>
                            (p.ItemCode?.ToUpper().Contains(filtroTexto) ?? false) ||
                            (p.ItemName?.ToUpper().Contains(filtroTexto) ?? false)).ToList();

                    var dataPlantilla = plantilla.Select(p => new
                    {
                        p.ItemId,
                        p.ItemCode,
                        p.ItemName,
                        p.MinimumStock,
                        p.MaximumStock,
                        p.ReorderPoint
                    }).ToList();

                    Tabla2.DataSource = dataPlantilla;
                    ConfigurarColumnaTablaBase(esPlantilla: true);
                    Lbl_Conteo2.Text = $"PLANTILLA \"{baseSeleccionada.Nombre}\": {plantilla.Count} ARTÍCULOS";
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al filtrar plantilla base: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Search
        #region AccionesArticulos

        private void Btn_CopySelected_Click(object sender, EventArgs e)
        {
            try
            {
                if (_categoriaActivaId <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CATEGORÍA ACTIVA PRIMERO.",
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

                    bool yaEnPlantilla = _articulosEnPlantilla?.Any(p => p.ItemId == itemId) ?? false;
                    bool yaPendiente = _plantillasPendientesAgregar.Any(p => p.ItemId == itemId);

                    if (!yaEnPlantilla && !yaPendiente)
                    {
                        _plantillasPendientesAgregar.Add(new Mdl_ItemStockTemplates
                        {
                            TemplateId = 0,
                            LocationCategoryId = _categoriaActivaId,
                            ItemId = itemId,
                            ItemCode = itemCode,
                            ItemName = itemName,
                            MinimumStock = 0,
                            MaximumStock = 0,
                            ReorderPoint = null,
                            IsActive = true,
                            CreatedBy = UserData?.UserId
                        });
                    }
                }

                CargarArticulosDisponibles();
                CargarArticulosEnPlantilla();
                ActualizarConteos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar artículo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_CopyAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (_categoriaActivaId <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CATEGORÍA ACTIVA PRIMERO.",
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
                    $"¿ESTÁ SEGURO DE COPIAR TODOS LOS ARTÍCULOS DE LA PLANTILLA BASE A \"{_categoriaActivaNombre}\"?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                foreach (DataGridViewRow fila in Tabla2.Rows)
                {
                    if (fila.IsNewRow) continue;
                    if (fila.Cells["ItemId"].Value == null) continue;

                    int itemId = (int)fila.Cells["ItemId"].Value;
                    string itemCode = fila.Cells["ItemCode"].Value?.ToString() ?? "";
                    string itemName = fila.Cells["ItemName"].Value?.ToString() ?? "";

                    bool yaEnPlantilla = _articulosEnPlantilla?.Any(p => p.ItemId == itemId) ?? false;
                    bool yaPendiente = _plantillasPendientesAgregar.Any(p => p.ItemId == itemId);

                    if (!yaEnPlantilla && !yaPendiente)
                    {
                        _plantillasPendientesAgregar.Add(new Mdl_ItemStockTemplates
                        {
                            TemplateId = 0,
                            LocationCategoryId = _categoriaActivaId,
                            ItemId = itemId,
                            ItemCode = itemCode,
                            ItemName = itemName,
                            MinimumStock = 0,
                            MaximumStock = 0,
                            ReorderPoint = null,
                            IsActive = true,
                            CreatedBy = UserData?.UserId
                        });
                    }
                }

                CargarArticulosDisponibles();
                CargarArticulosEnPlantilla();
                ActualizarConteos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar todos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_RemoveSelected_Click(object sender, EventArgs e)
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN ARTÍCULO DE LA PLANTILLA ACTIVA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow fila in Tabla.SelectedRows)
                {
                    if (fila.Cells["ItemId"].Value == null) continue;

                    int itemId = (int)fila.Cells["ItemId"].Value;

                    // Buscar en plantilla activa en memoria
                    var enPlantilla = _articulosEnPlantilla?.FirstOrDefault(p => p.ItemId == itemId);
                    if (enPlantilla == null) continue;

                    if (enPlantilla.TemplateId > 0)
                    {
                        // Existe en BD — marcar para eliminar al guardar
                        if (!_templateIdsParaEliminar.Contains(enPlantilla.TemplateId))
                            _templateIdsParaEliminar.Add(enPlantilla.TemplateId);
                    }
                    else
                    {
                        // Solo está en memoria — quitar de pendientes
                        var pendiente = _plantillasPendientesAgregar.FirstOrDefault(p => p.ItemId == itemId);
                        if (pendiente != null)
                            _plantillasPendientesAgregar.Remove(pendiente);
                    }
                }

                CargarArticulosDisponibles();
                CargarArticulosEnPlantilla();
                ActualizarConteos();
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
                if (_articulosEnPlantilla == null || _articulosEnPlantilla.Count == 0)
                {
                    MessageBox.Show("LA PLANTILLA ACTIVA ESTÁ VACÍA.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE QUITAR TODOS LOS ARTÍCULOS DE LA PLANTILLA \"{_categoriaActivaNombre}\"?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion != DialogResult.Yes) return;

                // Marcar para eliminar los que existen en BD
                foreach (var plantilla in _articulosEnPlantilla)
                {
                    if (plantilla.TemplateId > 0 && !_templateIdsParaEliminar.Contains(plantilla.TemplateId))
                        _templateIdsParaEliminar.Add(plantilla.TemplateId);
                }

                // Limpiar pendientes en memoria
                _plantillasPendientesAgregar.Clear();

                CargarArticulosDisponibles();
                CargarArticulosEnPlantilla();
                ActualizarConteos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al quitar todos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Match_Click(object sender, EventArgs e)
        {
            try
            {
                if (_categoriaActivaId <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CATEGORÍA ACTIVA PRIMERO.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!(ComboBox_Template.SelectedItem is CategoriaItem baseSeleccionada) ||
                    ComboBox_Template.SelectedIndex <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA PLANTILLA BASE.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar que no sea la misma categoría activa
                if (baseSeleccionada.Id > 0 && baseSeleccionada.Id == _categoriaActivaId)
                {
                    MessageBox.Show("NO PUEDE USAR LA MISMA CATEGORÍA ACTIVA COMO PLANTILLA BASE.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var idsYaEnActiva = ObtenerIdsEnPlantilla();

                // CATÁLOGO PRINCIPAL: jala todos los artículos de la tabla Items
                if (baseSeleccionada.Id == 0)
                {
                    var todosLosItems = Ctrl_Items.MostrarArticulos(1, 9999);

                    if (todosLosItems.Count == 0)
                    {
                        MessageBox.Show("EL CATÁLOGO PRINCIPAL NO TIENE ARTÍCULOS REGISTRADOS.",
                            "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var confirmacion = MessageBox.Show(
                        $"SE COPIARÁN {todosLosItems.Count} ARTÍCULOS DESDE EL CATÁLOGO PRINCIPAL.\n\n¿CONTINUAR?",
                        "CONFIRMAR COPIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmacion != DialogResult.Yes) return;

                    foreach (var item in todosLosItems)
                    {
                        if (!idsYaEnActiva.Contains(item.ItemId))
                        {
                            _plantillasPendientesAgregar.Add(new Mdl_ItemStockTemplates
                            {
                                TemplateId = 0,
                                LocationCategoryId = _categoriaActivaId,
                                ItemId = item.ItemId,
                                ItemCode = item.ItemCode,
                                ItemName = item.ItemName,
                                MinimumStock = 0,
                                MaximumStock = 0,
                                ReorderPoint = null,
                                IsActive = true,
                                CreatedBy = UserData?.UserId
                            });
                        }
                    }
                }
                else
                {
                    // Plantilla de otra categoría
                    var plantillaBase = Ctrl_ItemStockTemplates.MostrarPlantillasPorCategoria(baseSeleccionada.Id);

                    if (plantillaBase.Count == 0)
                    {
                        MessageBox.Show("LA PLANTILLA BASE SELECCIONADA ESTÁ VACÍA.",
                            "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var confirmacion = MessageBox.Show(
                        $"SE COPIARÁN {plantillaBase.Count} ARTÍCULOS DESDE LA PLANTILLA \"{baseSeleccionada.Nombre}\".\n\n¿CONTINUAR?",
                        "CONFIRMAR COPIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmacion != DialogResult.Yes) return;

                    foreach (var item in plantillaBase)
                    {
                        if (!idsYaEnActiva.Contains(item.ItemId))
                        {
                            _plantillasPendientesAgregar.Add(new Mdl_ItemStockTemplates
                            {
                                TemplateId = 0,
                                LocationCategoryId = _categoriaActivaId,
                                ItemId = item.ItemId,
                                ItemCode = item.ItemCode,
                                ItemName = item.ItemName,
                                MinimumStock = item.MinimumStock,
                                MaximumStock = item.MaximumStock,
                                ReorderPoint = item.ReorderPoint,
                                IsActive = true,
                                CreatedBy = UserData?.UserId
                            });
                        }
                    }
                }

                CargarArticulosDisponibles();
                CargarArticulosEnPlantilla();
                ActualizarConteos();

                MessageBox.Show("ARTÍCULOS COPIADOS. RECUERDE GUARDAR LOS CAMBIOS.",
                    "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al copiar plantilla: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion AccionesArticulos
        #region GuardarCambios

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            // Confirmar y persistir todos los cambios en BD
            try
            {
                if (_categoriaActivaId <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CATEGORÍA ACTIVA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool hayInsercionesConStocksEdicion = false;

                // Verificar si hay stocks en 0 en Tabla2 que el usuario debería revisar
                if (_articulosEnPlantilla != null)
                {
                    hayInsercionesConStocksEdicion = _articulosEnPlantilla
                        .Any(p => p.MinimumStock == 0 && p.MaximumStock == 0);
                }

                string mensaje = $"SE GUARDARÁN LOS SIGUIENTES CAMBIOS:\n\n" +
                    $"• ARTÍCULOS A AGREGAR: {_plantillasPendientesAgregar.Count}\n" +
                    $"• ARTÍCULOS A ELIMINAR: {_templateIdsParaEliminar.Count}\n";

                if (hayInsercionesConStocksEdicion)
                    mensaje += "\nATENCIÓN: ALGUNOS ARTÍCULOS TIENEN STOCK MÍN./MÁX. EN 0. PUEDE EDITARLOS EN LA GRILLA.";

                mensaje += "\n\n¿CONFIRMAR CAMBIOS?";

                var confirmacion = MessageBox.Show(mensaje, "CONFIRMAR GUARDADO",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                this.Cursor = Cursors.WaitCursor;

                // Primero capturar los valores editados en Tabla2 (stocks modificados en grilla)
                ActualizarStocksDesdeGrilla();

                int errores = 0;

                // Inactivar los marcados para eliminar
                foreach (int templateId in _templateIdsParaEliminar)
                {
                    int res = Ctrl_ItemStockTemplates.InactivarPlantilla(templateId);
                    if (res <= 0) errores++;
                }

                // Insertar los nuevos
                foreach (var plantilla in _plantillasPendientesAgregar)
                {
                    int res = Ctrl_ItemStockTemplates.RegistrarPlantilla(plantilla);
                    if (res <= 0) errores++;
                }

                this.Cursor = Cursors.Default;

                if (errores == 0)
                {
                    MessageBox.Show("CAMBIOS GUARDADOS EXITOSAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"SE GUARDARON LOS CAMBIOS CON {errores} ERROR(ES).", "ADVERTENCIA",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                _plantillasPendientesAgregar.Clear();
                _templateIdsParaEliminar.Clear();
                CargarTablasParaCategoria();
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
            // Actualiza en BD los registros existentes en Tabla (plantilla activa)
            if (Tabla.Rows.Count == 0) return;

            foreach (DataGridViewRow fila in Tabla.Rows)
            {
                if (fila.IsNewRow) continue;
                if (!Tabla.Columns.Contains("TemplateId")) break;

                int templateId = 0;
                if (fila.Cells["TemplateId"].Value != null)
                    int.TryParse(fila.Cells["TemplateId"].Value.ToString(), out templateId);

                if (templateId <= 0) continue;

                decimal minStock = 0;
                decimal maxStock = 0;
                decimal? reorderPoint = null;

                if (fila.Cells["MinimumStock"].Value != null)
                    decimal.TryParse(fila.Cells["MinimumStock"].Value.ToString(), out minStock);

                if (fila.Cells["MaximumStock"].Value != null)
                    decimal.TryParse(fila.Cells["MaximumStock"].Value.ToString(), out maxStock);

                if (fila.Cells["ReorderPoint"].Value != null &&
                    !string.IsNullOrWhiteSpace(fila.Cells["ReorderPoint"].Value.ToString()))
                {
                    if (decimal.TryParse(fila.Cells["ReorderPoint"].Value.ToString(), out decimal rp))
                        reorderPoint = rp;
                }

                Ctrl_ItemStockTemplates.ActualizarPlantilla(new Mdl_ItemStockTemplates
                {
                    TemplateId = templateId,
                    MinimumStock = minStock,
                    MaximumStock = maxStock,
                    ReorderPoint = reorderPoint,
                    ModifiedBy = UserData?.UserId
                });
            }
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            var confirmacion = MessageBox.Show(
                "¿ESTÁ SEGURO QUE DESEA REVERTIR TODOS LOS CAMBIOS NO GUARDADOS EN LA PLANTILLA ACTIVA?",
                "CONFIRMAR REVERTIR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes) return;

            _plantillasPendientesAgregar.Clear();
            _templateIdsParaEliminar.Clear();

            // Solo recarga Tabla (izquierda) — Tabla2 NO se toca
            if (_categoriaActivaId > 0)
                CargarArticulosEnPlantilla();
        }

        #endregion GuardarCambios
    }
    //CLASE AUXILIAR PARA MANEJAR CLASIFICACIÓN DE ITEM
    internal class ClasificacionItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public ClasificacionItem(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public override string ToString() => Nombre;
    }
}