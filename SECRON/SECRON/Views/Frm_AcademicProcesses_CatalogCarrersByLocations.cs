using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_CatalogCarrersByLocations : Form
    {
        #region PropiedadesIniciales

        // Datos del usuario autenticado
        public Mdl_Security_UserInfo UserData { get; set; }

        // Sede activa seleccionada para configurar su catálogo de carreras
        private int _locationIdActiva = 0;
        private string _locationNombreActiva = "";

        // Carreras YA asignadas a la sede activa (plantilla activa)
        private List<Mdl_LocationCareers> _carrerasEnPlantilla;

        // Cambios pendientes de guardar (en memoria hasta Btn_Yes)
        private List<Mdl_LocationCareers> _asignacionesPendientesAgregar = new List<Mdl_LocationCareers>();
        private List<int> _locationCareerIdsParaEliminar = new List<int>();

        // Filtros búsqueda izquierda (plantilla activa)
        private string _ultimoTextoEnPlantilla = "";

        // Filtros búsqueda derecha (plantilla base)
        private string _ultimoTextoBase = "";

        public Frm_AcademicProcesses_CatalogCarrersByLocations()
        {
            InitializeComponent();

            this.Load += Frm_AcademicProcesses_CatalogCarrersByLocations_Load;
            ComboBox_Template.SelectedIndexChanged += ComboBox_Template_SelectedIndexChanged;
            ComboBox_TemplateBase.SelectedIndexChanged += ComboBox_TemplateBase_SelectedIndexChanged;
            Btn_Search.Click += Btn_Search_Click;
            Btn_Clear.Click += Btn_Clear_Click;
            Btn_Search2.Click += Btn_Search2_Click;
            Btn_Clear2.Click += Btn_Clear2_Click;
            Txt_ValorBuscado.KeyDown += Txt_ValorBuscado_KeyDown;
            Txt_ValorBuscado2.KeyDown += Txt_ValorBuscado2_KeyDown;
            Btn_CopySelected.Click += Btn_CopySelected_Click;
            Btn_CopyAll.Click += Btn_CopyAll_Click;
            Btn_RemoveSelected.Click += Btn_RemoveSelected_Click;
            Btn_RemoveAll.Click += Btn_RemoveAll_Click;
            Btn_Yes.Click += Btn_Yes_Click;
            Btn_No.Click += Btn_No_Click;
        }

        private void Frm_AcademicProcesses_CatalogCarrersByLocations_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarTamañoFormulario();
                ConfigurarPlaceHolders();
                ConfigurarFiltros();
                CargarCombosSedes();
                ConfigurarTablas();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("ERROR AL CARGAR EL FORMULARIO: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR CARRERA...");
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
            // --- LADO IZQUIERDO (carreras en la plantilla activa) ---
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.Items.Add("POR NOMBRE");
            Filtro1.SelectedIndex = 0;

            // Filtro2/Filtro3: no aplican en este formulario (sin clasificaciones adicionales)
            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODAS");
            Filtro2.SelectedIndex = 0;
            Filtro2.Enabled = false;

            Filtro3.Items.Clear();
            Filtro3.Items.Add("TODOS");
            Filtro3.SelectedIndex = 0;
            Filtro3.Enabled = false;

            // --- LADO DERECHO (carreras en la plantilla base) ---
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

        private void CargarCombosSedes()
        {
            var sedes = Ctrl_Locations.ObtenerLocationsActivas();

            // --- COMBO IZQUIERDO: Sede activa (la que vamos a configurar) ---
            ComboBox_Template.SelectedIndexChanged -= ComboBox_Template_SelectedIndexChanged;
            ComboBox_Template.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Template.Items.Clear();
            ComboBox_Template.Items.Add("SELECCIONAR SEDE A CONFIGURAR...");

            foreach (var sede in sedes)
                ComboBox_Template.Items.Add(new SedeItem(sede.Key, sede.Value));

            ComboBox_Template.DisplayMember = "Nombre";
            ComboBox_Template.SelectedIndex = 0;
            ComboBox_Template.SelectedIndexChanged += ComboBox_Template_SelectedIndexChanged;

            // --- COMBO DERECHO: Plantilla base (de dónde copiamos) ---
            ComboBox_TemplateBase.SelectedIndexChanged -= ComboBox_TemplateBase_SelectedIndexChanged;
            ComboBox_TemplateBase.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_TemplateBase.Items.Clear();
            ComboBox_TemplateBase.Items.Add("SELECCIONAR PLANTILLA BASE...");
            ComboBox_TemplateBase.Items.Add(new SedeItem(0, "CATÁLOGO PRINCIPAL"));

            foreach (var sede in sedes)
                ComboBox_TemplateBase.Items.Add(new SedeItem(sede.Key, sede.Value));

            ComboBox_TemplateBase.DisplayMember = "Nombre";
            ComboBox_TemplateBase.SelectedIndex = 0;
            ComboBox_TemplateBase.SelectedIndexChanged += ComboBox_TemplateBase_SelectedIndexChanged;
        }

        // Cambia la sede activa a configurar, recarga Tabla (izquierda)
        private void ComboBox_Template_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_Template.SelectedItem is SedeItem sede) || ComboBox_Template.SelectedIndex <= 0)
            {
                _locationIdActiva = 0;
                _locationNombreActiva = "";
                Tabla.DataSource = null;
                Lbl_Conteo.Text = "SELECCIONE UNA SEDE A CONFIGURAR";
                return;
            }

            // Validar que no sea la misma que está seleccionada como plantilla base
            if (ComboBox_TemplateBase.SelectedItem is SedeItem baseActual &&
                baseActual.Id > 0 && baseActual.Id == sede.Id)
            {
                MessageBox.Show("NO PUEDE CONFIGURAR LA MISMA SEDE QUE ESTÁ USANDO COMO PLANTILLA BASE.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Template.SelectedIndex = 0;
                return;
            }

            _locationIdActiva = sede.Id;
            _locationNombreActiva = sede.Nombre;

            _asignacionesPendientesAgregar.Clear();
            _locationCareerIdsParaEliminar.Clear();

            CargarCarrerasEnPlantilla();
        }

        // Cambia la plantilla base, recarga Tabla2 (derecha)
        private void ComboBox_TemplateBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_TemplateBase.SelectedItem is SedeItem baseSeleccionada) ||
                ComboBox_TemplateBase.SelectedIndex <= 0)
            {
                Tabla2.DataSource = null;
                Lbl_Conteo2.Text = "SELECCIONE UNA PLANTILLA BASE";
                return;
            }

            if (baseSeleccionada.Id > 0 && baseSeleccionada.Id == _locationIdActiva)
            {
                MessageBox.Show("NO PUEDE SELECCIONAR LA MISMA SEDE QUE ESTÁ CONFIGURANDO.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_TemplateBase.SelectedIndex = 0;
                return;
            }

            CargarTablaBase(baseSeleccionada, "");
        }

        private void CargarTablaBase(SedeItem baseSeleccionada, string filtroTexto)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (baseSeleccionada.Id == 0)
                {
                    var data = Ctrl_Careers.ObtenerCarrerasParaExportar("TODOS", null, "ACTIVA");

                    if (!string.IsNullOrWhiteSpace(filtroTexto))
                        data = data.Where(c =>
                            (c.CareerCode?.ToUpper().Contains(filtroTexto) ?? false) ||
                            (c.CareerName?.ToUpper().Contains(filtroTexto) ?? false)).ToList();

                    if (data.Count == 0)
                    {
                        Tabla2.DataSource = null;
                        Lbl_Conteo2.Text = "EL CATÁLOGO PRINCIPAL ESTÁ VACÍO";
                        this.Cursor = Cursors.Default;
                        return;
                    }

                    var dataCarreras = data.Select(c => new { c.CareerId, c.CareerCode, c.CareerName }).ToList();
                    Tabla2.DataSource = dataCarreras;
                    ConfigurarColumnaTablaBase();
                    Lbl_Conteo2.Text = $"CATÁLOGO GENERAL DE CARRERAS: {data.Count} CARRERAS";
                }
                else
                {
                    var plantilla = Ctrl_LocationCareers.MostrarCarrerasPorSede(baseSeleccionada.Id);

                    if (!string.IsNullOrWhiteSpace(filtroTexto))
                        plantilla = plantilla.Where(p =>
                            (p.CareerCode?.ToUpper().Contains(filtroTexto) ?? false) ||
                            (p.CareerName?.ToUpper().Contains(filtroTexto) ?? false)).ToList();

                    if (plantilla.Count == 0)
                    {
                        Tabla2.DataSource = null;
                        Lbl_Conteo2.Text = $"LA SEDE \"{baseSeleccionada.Nombre}\" NO TIENE CARRERAS ASIGNADAS";
                        this.Cursor = Cursors.Default;
                        return;
                    }

                    var dataPlantilla = plantilla.Select(p => new { p.CareerId, p.CareerCode, p.CareerName }).ToList();
                    Tabla2.DataSource = dataPlantilla;
                    ConfigurarColumnaTablaBase();
                    Lbl_Conteo2.Text = $"CARRERAS DE \"{baseSeleccionada.Nombre}\": {plantilla.Count}";
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("ERROR AL CARGAR PLANTILLA BASE: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColumnaTablaBase()
        {
            if (Tabla2.Columns.Count == 0) return;

            if (Tabla2.Columns.Contains("CareerId"))
                Tabla2.Columns["CareerId"].Visible = false;

            if (Tabla2.Columns.Contains("CareerCode"))
            {
                Tabla2.Columns["CareerCode"].HeaderText = "CÓDIGO";
                Tabla2.Columns["CareerCode"].ReadOnly = true;
                Tabla2.Columns["CareerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["CareerCode"].FillWeight = 25;
            }
            if (Tabla2.Columns.Contains("CareerName"))
            {
                Tabla2.Columns["CareerName"].HeaderText = "NOMBRE DE LA CARRERA";
                Tabla2.Columns["CareerName"].ReadOnly = true;
                Tabla2.Columns["CareerName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["CareerName"].FillWeight = 75;
            }
        }

        #endregion ConfiguracionInicial
        #region ConfiguracionTablas

        private void ConfigurarTablas()
        {
            // Tabla izquierda (carreras en la plantilla activa de la sede)
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = true;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToResizeRows = false;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            // Tabla derecha (carreras en la plantilla base seleccionada)
            Tabla2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla2.MultiSelect = true;
            Tabla2.ReadOnly = true;
            Tabla2.AllowUserToResizeRows = false;
            Tabla2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        private void CargarCarrerasEnPlantilla()
        {
            _carrerasEnPlantilla = Ctrl_LocationCareers.MostrarCarrerasPorSede(_locationIdActiva);

            // Incluir los pendientes de agregar en memoria
            var pendientes = _asignacionesPendientesAgregar
                .Where(p => !_carrerasEnPlantilla.Any(e => e.CareerId == p.CareerId))
                .ToList();
            _carrerasEnPlantilla.AddRange(pendientes);

            // Excluir los marcados para eliminar
            _carrerasEnPlantilla = _carrerasEnPlantilla
                .Where(p => !_locationCareerIdsParaEliminar.Contains(p.LocationCareerId))
                .ToList();

            AplicarFiltroEnPlantilla();

            Lbl_Conteo.Text = $"CARRERAS ASIGNADAS A \"{_locationNombreActiva}\": {_carrerasEnPlantilla.Count}";
        }

        private void AplicarFiltroEnPlantilla()
        {
            var datos = _carrerasEnPlantilla ?? new List<Mdl_LocationCareers>();

            if (!string.IsNullOrWhiteSpace(_ultimoTextoEnPlantilla))
            {
                string texto = _ultimoTextoEnPlantilla.ToUpper();
                datos = datos.Where(p =>
                    (p.CareerCode?.ToUpper().Contains(texto) ?? false) ||
                    (p.CareerName?.ToUpper().Contains(texto) ?? false)).ToList();
            }

            var data = datos.Select(p => new
            {
                p.LocationCareerId,
                p.CareerId,
                p.CareerCode,
                p.CareerName
            }).ToList();

            Tabla.DataSource = data;
            ConfigurarColumnaTabla();
        }

        private void ConfigurarColumnaTabla()
        {
            if (Tabla.Columns.Count == 0) return;

            if (Tabla.Columns.Contains("LocationCareerId"))
                Tabla.Columns["LocationCareerId"].Visible = false;
            if (Tabla.Columns.Contains("CareerId"))
                Tabla.Columns["CareerId"].Visible = false;

            if (Tabla.Columns.Contains("CareerCode"))
            {
                Tabla.Columns["CareerCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["CareerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CareerCode"].FillWeight = 25;
            }
            if (Tabla.Columns.Contains("CareerName"))
            {
                Tabla.Columns["CareerName"].HeaderText = "NOMBRE DE LA CARRERA";
                Tabla.Columns["CareerName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CareerName"].FillWeight = 75;
            }
        }

        #endregion ConfiguracionTablas
        #region Search

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                _ultimoTextoEnPlantilla = (Txt_ValorBuscado.ForeColor != Color.Gray)
                    ? Txt_ValorBuscado.Text.Trim() : "";

                AplicarFiltroEnPlantilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL BUSCAR: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_Search_Click(sender, e); }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR CARRERA...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Filtro1.SelectedIndex = 0;
            _ultimoTextoEnPlantilla = "";
            AplicarFiltroEnPlantilla();
        }

        private void Btn_Search2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(ComboBox_TemplateBase.SelectedItem is SedeItem baseSeleccionada) ||
                    ComboBox_TemplateBase.SelectedIndex <= 0)
                    return;

                string texto = (Txt_ValorBuscado2.ForeColor != Color.Gray)
                    ? Txt_ValorBuscado2.Text.Trim().ToUpper() : "";

                _ultimoTextoBase = texto;
                CargarTablaBase(baseSeleccionada, texto);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL BUSCAR: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (!(ComboBox_TemplateBase.SelectedItem is SedeItem baseSeleccionada) ||
                ComboBox_TemplateBase.SelectedIndex <= 0)
                return;

            CargarTablaBase(baseSeleccionada, "");
        }

        #endregion Search
        #region AccionesCarreras

        private void Btn_CopySelected_Click(object sender, EventArgs e)
        {
            try
            {
                if (_locationIdActiva <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA SEDE ACTIVA PRIMERO.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Tabla2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UNA CARRERA DE LA PLANTILLA BASE.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow fila in Tabla2.SelectedRows)
                {
                    AgregarPendiente(fila);
                }

                CargarCarrerasEnPlantilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL AGREGAR CARRERA: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_CopyAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (_locationIdActiva <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA SEDE ACTIVA PRIMERO.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Tabla2.Rows.Count == 0)
                {
                    MessageBox.Show("LA PLANTILLA BASE NO TIENE CARRERAS PARA COPIAR.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE COPIAR TODAS LAS CARRERAS DE LA PLANTILLA BASE A \"{_locationNombreActiva}\"?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                foreach (DataGridViewRow fila in Tabla2.Rows)
                {
                    if (fila.IsNewRow) continue;
                    AgregarPendiente(fila);
                }

                CargarCarrerasEnPlantilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL AGREGAR TODAS LAS CARRERAS: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Agrega una fila de Tabla2 a la lista de pendientes, si no está ya asignada ni pendiente
        private void AgregarPendiente(DataGridViewRow fila)
        {
            if (fila.Cells["CareerId"].Value == null) return;

            int careerId = Convert.ToInt32(fila.Cells["CareerId"].Value);
            string careerCode = fila.Cells["CareerCode"].Value?.ToString() ?? "";
            string careerName = fila.Cells["CareerName"].Value?.ToString() ?? "";

            bool yaEnPlantilla = _carrerasEnPlantilla?.Any(p => p.CareerId == careerId) ?? false;
            bool yaPendiente = _asignacionesPendientesAgregar.Any(p => p.CareerId == careerId);

            if (!yaEnPlantilla && !yaPendiente)
            {
                _asignacionesPendientesAgregar.Add(new Mdl_LocationCareers
                {
                    LocationCareerId = 0,
                    LocationId = _locationIdActiva,
                    CareerId = careerId,
                    CareerCode = careerCode,
                    CareerName = careerName,
                    IsActive = true,
                    CreatedBy = UserData?.UserId
                });
            }
        }

        private void Btn_RemoveSelected_Click(object sender, EventArgs e)
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UNA CARRERA DE LA PLANTILLA ACTIVA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow fila in Tabla.SelectedRows)
                {
                    if (fila.Cells["CareerId"].Value == null) continue;

                    int careerId = Convert.ToInt32(fila.Cells["CareerId"].Value);

                    var enPlantilla = _carrerasEnPlantilla?.FirstOrDefault(p => p.CareerId == careerId);
                    if (enPlantilla == null) continue;

                    if (enPlantilla.LocationCareerId > 0)
                    {
                        // Existe en BD — marcar para eliminar al guardar
                        if (!_locationCareerIdsParaEliminar.Contains(enPlantilla.LocationCareerId))
                            _locationCareerIdsParaEliminar.Add(enPlantilla.LocationCareerId);
                    }
                    else
                    {
                        // Solo está en memoria — quitar de pendientes
                        var pendiente = _asignacionesPendientesAgregar.FirstOrDefault(p => p.CareerId == careerId);
                        if (pendiente != null)
                            _asignacionesPendientesAgregar.Remove(pendiente);
                    }
                }

                CargarCarrerasEnPlantilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL QUITAR CARRERA: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_RemoveAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (_carrerasEnPlantilla == null || _carrerasEnPlantilla.Count == 0)
                {
                    MessageBox.Show("LA PLANTILLA ACTIVA ESTÁ VACÍA.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE QUITAR TODAS LAS CARRERAS DE LA SEDE \"{_locationNombreActiva}\"?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion != DialogResult.Yes) return;

                foreach (var asignacion in _carrerasEnPlantilla)
                {
                    if (asignacion.LocationCareerId > 0 && !_locationCareerIdsParaEliminar.Contains(asignacion.LocationCareerId))
                        _locationCareerIdsParaEliminar.Add(asignacion.LocationCareerId);
                }

                _asignacionesPendientesAgregar.Clear();

                CargarCarrerasEnPlantilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL QUITAR TODAS LAS CARRERAS: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion AccionesCarreras
        #region GuardarCambios

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            try
            {
                if (_locationIdActiva <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA SEDE ACTIVA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_asignacionesPendientesAgregar.Count == 0 && _locationCareerIdsParaEliminar.Count == 0)
                {
                    MessageBox.Show("NO HAY CAMBIOS PENDIENTES POR GUARDAR.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string mensaje = "SE GUARDARÁN LOS SIGUIENTES CAMBIOS:\n\n" +
                    $"• CARRERAS A ASIGNAR: {_asignacionesPendientesAgregar.Count}\n" +
                    $"• CARRERAS A QUITAR: {_locationCareerIdsParaEliminar.Count}\n\n¿CONFIRMAR CAMBIOS?";

                var confirmacion = MessageBox.Show(mensaje, "CONFIRMAR GUARDADO",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                this.Cursor = Cursors.WaitCursor;

                int errores = 0;

                // Quitar (inactivar) las marcadas para eliminar
                foreach (int locationCareerId in _locationCareerIdsParaEliminar)
                {
                    int res = Ctrl_LocationCareers.CambiarEstadoAsignacion(locationCareerId, false, UserData?.UserId);
                    if (res <= 0) errores++;
                }

                // Insertar las nuevas asignaciones
                foreach (var asignacion in _asignacionesPendientesAgregar)
                {
                    int res = Ctrl_LocationCareers.RegistrarAsignacion(asignacion);
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

                _asignacionesPendientesAgregar.Clear();
                _locationCareerIdsParaEliminar.Clear();
                CargarCarrerasEnPlantilla();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("ERROR AL GUARDAR: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            if (_asignacionesPendientesAgregar.Count == 0 && _locationCareerIdsParaEliminar.Count == 0)
            {
                MessageBox.Show("NO HAY CAMBIOS PENDIENTES POR REVERTIR.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmacion = MessageBox.Show(
                "¿ESTÁ SEGURO QUE DESEA REVERTIR TODOS LOS CAMBIOS NO GUARDADOS EN LA PLANTILLA ACTIVA?",
                "CONFIRMAR REVERTIR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes) return;

            _asignacionesPendientesAgregar.Clear();
            _locationCareerIdsParaEliminar.Clear();

            if (_locationIdActiva > 0)
                CargarCarrerasEnPlantilla();
        }

        #endregion GuardarCambios
    }

    // CLASE AUXILIAR PARA MANEJAR SEDES EN LOS COMBOS DE ESTE FORMULARIO
    internal class SedeItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public SedeItem(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public override string ToString() => Nombre;
    }
}