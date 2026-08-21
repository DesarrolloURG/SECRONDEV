using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_CatalogCoursesByCareer : Form
    {
        #region PropiedadesIniciales

        // Datos del usuario autenticado
        public Mdl_Security_UserInfo UserData { get; set; }

        // Carrera activa seleccionada para configurar su pensum de cursos
        private int _careerIdActiva = 0;
        private string _careerNombreActiva = "";

        // Sede usada como filtro para limitar el combo de carreras (0 = todas las sedes)
        private int _sedeFiltroId = 0;

        // Sede usada como filtro para limitar el combo de plantilla base (0 = todas las sedes)
        private int _sedeFiltroBaseId = 0;

        // Cursos YA asignados a la carrera activa (plantilla activa)
        private List<Mdl_CareerCourses> _cursosEnPlantilla;

        // Cambios pendientes de guardar (en memoria hasta Btn_Yes)
        private List<Mdl_CareerCourses> _asignacionesPendientesAgregar = new List<Mdl_CareerCourses>();
        private List<int> _careerCourseIdsParaEliminar = new List<int>();

        // Filtros búsqueda izquierda (plantilla activa)
        private string _ultimoTextoEnPlantilla = "";

        // Filtros búsqueda derecha (plantilla base)
        private string _ultimoTextoBase = "";

        public Frm_AcademicProcesses_CatalogCoursesByCareer()
        {
            InitializeComponent();

            this.Load += Frm_AcademicProcesses_CatalogCoursesByCareer_Load;
            ComboBox_Template.SelectedIndexChanged += ComboBox_Template_SelectedIndexChanged;
            ComboBox_TemplateBase.SelectedIndexChanged += ComboBox_TemplateBase_SelectedIndexChanged;
            Filtro2.SelectedIndexChanged += Filtro2_SelectedIndexChanged;
            Filtro5.SelectedIndexChanged += Filtro5_SelectedIndexChanged;
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

        private void Frm_AcademicProcesses_CatalogCoursesByCareer_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarTamañoFormulario();
                ConfigurarPlaceHolders();
                ConfigurarFiltros();
                CargarCombosCarreras();
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
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR CURSO...");
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
            // --- LADO IZQUIERDO (cursos en la plantilla activa) ---
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.Items.Add("POR NOMBRE");
            Filtro1.SelectedIndex = 0;

            // Filtro2: filtro por SEDE — limita ComboBox_Template a las carreras de esa sede
            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODAS LAS SEDES");
            foreach (var sede in Ctrl_Locations.ObtenerLocationsActivas())
                Filtro2.Items.Add(new SedeItem(sede.Key, sede.Value));
            Filtro2.DisplayMember = "Nombre"; // no aplica al string "TODAS LAS SEDES", ToString() lo cubre
            Filtro2.SelectedIndex = 0;
            Filtro2.Enabled = true;

            // Filtro3: no aplica en este formulario
            Filtro3.Items.Clear();
            Filtro3.Items.Add("TODOS");
            Filtro3.SelectedIndex = 0;
            Filtro3.Enabled = false;

            // --- LADO DERECHO (cursos en la plantilla base) ---
            Filtro4.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro5.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro6.DropDownStyle = ComboBoxStyle.DropDownList;

            Filtro4.Items.Clear();
            Filtro4.Items.Add("TODOS");
            Filtro4.Items.Add("POR CÓDIGO");
            Filtro4.Items.Add("POR NOMBRE");
            Filtro4.SelectedIndex = 0;

            Filtro5.Items.Clear();
            Filtro5.Items.Add("TODAS LAS SEDES");
            foreach (var sede in Ctrl_Locations.ObtenerLocationsActivas())
                Filtro5.Items.Add(new SedeItem(sede.Key, sede.Value));
            Filtro5.DisplayMember = "Nombre";
            Filtro5.SelectedIndex = 0;
            Filtro5.Enabled = true;

            Filtro6.Items.Clear();
            Filtro6.Items.Add("TODOS");
            Filtro6.SelectedIndex = 0;
            Filtro6.Enabled = false;
        }

        private void CargarCombosCarreras()
        {
            CargarComboCarreraActiva(0);
            CargarComboCarreraBase(0);
        }

        // Carga el combo de "Plantilla Base", limitado a una sede si locationId > 0.
        // "CATÁLOGO PRINCIPAL" siempre está disponible, sin importar el filtro de sede.
        private void CargarComboCarreraBase(int locationId)
        {
            List<CareerItem> items;

            if (locationId <= 0)
            {
                var carreras = Ctrl_Careers.ObtenerCarrerasParaExportar("TODOS", null, "ACTIVA");
                items = carreras.Select(c => new CareerItem(c.CareerId, c.CareerName)).ToList();
            }
            else
            {
                var carrerasDeSede = Ctrl_LocationCareers.MostrarCarrerasPorSede(locationId);
                items = carrerasDeSede.Select(c => new CareerItem(c.CareerId, c.CareerName)).ToList();
            }

            ComboBox_TemplateBase.SelectedIndexChanged -= ComboBox_TemplateBase_SelectedIndexChanged;
            ComboBox_TemplateBase.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_TemplateBase.Items.Clear();
            ComboBox_TemplateBase.Items.Add("SELECCIONAR PLANTILLA BASE...");
            ComboBox_TemplateBase.Items.Add(new CareerItem(0, "CATÁLOGO PRINCIPAL"));

            foreach (var item in items)
                ComboBox_TemplateBase.Items.Add(item);

            ComboBox_TemplateBase.DisplayMember = "Nombre";
            ComboBox_TemplateBase.SelectedIndex = 0;
            ComboBox_TemplateBase.SelectedIndexChanged += ComboBox_TemplateBase_SelectedIndexChanged;

            Tabla2.DataSource = null;
            Lbl_Conteo2.Text = "SELECCIONE UNA PLANTILLA BASE";
        }

        // Filtro5: al elegir una sede, limita ComboBox_TemplateBase a sus carreras asignadas
        private void Filtro5_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Filtro5.SelectedItem is SedeItem sede)
                {
                    _sedeFiltroBaseId = sede.Id;
                    CargarComboCarreraBase(sede.Id);
                }
                else
                {
                    // "TODAS LAS SEDES"
                    _sedeFiltroBaseId = 0;
                    CargarComboCarreraBase(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL FILTRAR POR SEDE: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Carga el combo de "Carrera a Configurar", limitado a una sede si locationId > 0
        private void CargarComboCarreraActiva(int locationId)
        {
            List<CareerItem> items;

            if (locationId <= 0)
            {
                var carreras = Ctrl_Careers.ObtenerCarrerasParaExportar("TODOS", null, "ACTIVA");
                items = carreras.Select(c => new CareerItem(c.CareerId, c.CareerName)).ToList();
            }
            else
            {
                var carrerasDeSede = Ctrl_LocationCareers.MostrarCarrerasPorSede(locationId);
                items = carrerasDeSede.Select(c => new CareerItem(c.CareerId, c.CareerName)).ToList();
            }

            ComboBox_Template.SelectedIndexChanged -= ComboBox_Template_SelectedIndexChanged;
            ComboBox_Template.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Template.Items.Clear();
            ComboBox_Template.Items.Add("SELECCIONAR CARRERA A CONFIGURAR...");

            foreach (var item in items)
                ComboBox_Template.Items.Add(item);

            ComboBox_Template.DisplayMember = "Nombre";
            ComboBox_Template.SelectedIndex = 0;
            ComboBox_Template.SelectedIndexChanged += ComboBox_Template_SelectedIndexChanged;

            // La carrera activa ya no es válida bajo el nuevo filtro de sede
            _careerIdActiva = 0;
            _careerNombreActiva = "";
            Tabla.DataSource = null;
            Lbl_Conteo.Text = locationId <= 0
                ? "SELECCIONE UNA CARRERA A CONFIGURAR"
                : $"SELECCIONE UNA CARRERA DE LA SEDE FILTRADA ({items.Count} DISPONIBLES)";
        }

        // Filtro2: al elegir una sede, limita ComboBox_Template a sus carreras asignadas
        private void Filtro2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _asignacionesPendientesAgregar.Clear();
                _careerCourseIdsParaEliminar.Clear();

                if (Filtro2.SelectedItem is SedeItem sede)
                {
                    _sedeFiltroId = sede.Id;
                    CargarComboCarreraActiva(sede.Id);
                }
                else
                {
                    // "TODAS LAS SEDES"
                    _sedeFiltroId = 0;
                    CargarComboCarreraActiva(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL FILTRAR POR SEDE: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cambia la carrera activa a configurar, recarga Tabla (izquierda)
        private void ComboBox_Template_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_Template.SelectedItem is CareerItem carrera) || ComboBox_Template.SelectedIndex <= 0)
            {
                _careerIdActiva = 0;
                _careerNombreActiva = "";
                Tabla.DataSource = null;
                Lbl_Conteo.Text = "SELECCIONE UNA CARRERA A CONFIGURAR";
                return;
            }

            // Validar que no sea la misma que está seleccionada como plantilla base
            if (ComboBox_TemplateBase.SelectedItem is CareerItem baseActual &&
                baseActual.Id > 0 && baseActual.Id == carrera.Id)
            {
                MessageBox.Show("NO PUEDE CONFIGURAR LA MISMA CARRERA QUE ESTÁ USANDO COMO PLANTILLA BASE.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Template.SelectedIndex = 0;
                return;
            }

            _careerIdActiva = carrera.Id;
            _careerNombreActiva = carrera.Nombre;

            _asignacionesPendientesAgregar.Clear();
            _careerCourseIdsParaEliminar.Clear();

            CargarCursosEnPlantilla();
        }

        // Cambia la plantilla base, recarga Tabla2 (derecha)
        private void ComboBox_TemplateBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_TemplateBase.SelectedItem is CareerItem baseSeleccionada) ||
                ComboBox_TemplateBase.SelectedIndex <= 0)
            {
                Tabla2.DataSource = null;
                Lbl_Conteo2.Text = "SELECCIONE UNA PLANTILLA BASE";
                return;
            }

            if (baseSeleccionada.Id > 0 && baseSeleccionada.Id == _careerIdActiva)
            {
                MessageBox.Show("NO PUEDE SELECCIONAR LA MISMA CARRERA QUE ESTÁ CONFIGURANDO.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_TemplateBase.SelectedIndex = 0;
                return;
            }

            CargarTablaBase(baseSeleccionada, "");
        }

        private void CargarTablaBase(CareerItem baseSeleccionada, string filtroTexto)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (baseSeleccionada.Id == 0)
                {
                    var data = Ctrl_Courses.ObtenerCursosParaExportar("TODOS", null, "ACTIVA", "TODOS");

                    if (!string.IsNullOrWhiteSpace(filtroTexto))
                        data = data.Where(c =>
                            (c.CourseCode?.ToUpper().Contains(filtroTexto) ?? false) ||
                            (c.CourseName?.ToUpper().Contains(filtroTexto) ?? false)).ToList();

                    if (data.Count == 0)
                    {
                        Tabla2.DataSource = null;
                        Lbl_Conteo2.Text = "EL CATÁLOGO PRINCIPAL ESTÁ VACÍO";
                        this.Cursor = Cursors.Default;
                        return;
                    }

                    var dataCursos = data.Select(c => new { c.CourseId, c.CourseCode, c.CourseName }).ToList();
                    Tabla2.DataSource = dataCursos;
                    ConfigurarColumnaTablaBase();
                    Lbl_Conteo2.Text = $"CATÁLOGO GENERAL DE CURSOS: {data.Count} CURSOS";
                }
                else
                {
                    var plantilla = Ctrl_CareerCourses.MostrarCursosPorCarrera(baseSeleccionada.Id, "ACTIVA");

                    if (!string.IsNullOrWhiteSpace(filtroTexto))
                        plantilla = plantilla.Where(p =>
                            (p.CourseCode?.ToUpper().Contains(filtroTexto) ?? false) ||
                            (p.CourseName?.ToUpper().Contains(filtroTexto) ?? false)).ToList();

                    if (plantilla.Count == 0)
                    {
                        Tabla2.DataSource = null;
                        Lbl_Conteo2.Text = $"LA CARRERA \"{baseSeleccionada.Nombre}\" NO TIENE CURSOS ASIGNADOS";
                        this.Cursor = Cursors.Default;
                        return;
                    }

                    var dataPlantilla = plantilla.Select(p => new { p.CourseId, p.CourseCode, p.CourseName }).ToList();
                    Tabla2.DataSource = dataPlantilla;
                    ConfigurarColumnaTablaBase();
                    Lbl_Conteo2.Text = $"CURSOS DE \"{baseSeleccionada.Nombre}\": {plantilla.Count}";
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

            if (Tabla2.Columns.Contains("CourseId"))
                Tabla2.Columns["CourseId"].Visible = false;

            if (Tabla2.Columns.Contains("CourseCode"))
            {
                Tabla2.Columns["CourseCode"].HeaderText = "CÓDIGO";
                Tabla2.Columns["CourseCode"].ReadOnly = true;
                Tabla2.Columns["CourseCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["CourseCode"].FillWeight = 30;
            }
            if (Tabla2.Columns.Contains("CourseName"))
            {
                Tabla2.Columns["CourseName"].HeaderText = "NOMBRE DEL CURSO";
                Tabla2.Columns["CourseName"].ReadOnly = true;
                Tabla2.Columns["CourseName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["CourseName"].FillWeight = 70;
            }
        }

        #endregion ConfiguracionInicial
        #region ConfiguracionTablas

        private void ConfigurarTablas()
        {
            // Tabla izquierda (cursos en la plantilla activa) — Semester/IsRequired/Prerequisites SÍ editables
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = true;
            Tabla.ReadOnly = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            Tabla.CellValidating -= Tabla_CellValidating;
            Tabla.CellValidating += Tabla_CellValidating;

            // Tabla derecha (cursos en la plantilla base seleccionada) — solo lectura
            Tabla2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla2.MultiSelect = true;
            Tabla2.ReadOnly = true;
            Tabla2.AllowUserToResizeRows = false;
            Tabla2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        // Valida que SEMESTRE solo acepte números enteros o quede vacío (nulo permitido)
        private void Tabla_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (Tabla.Columns[e.ColumnIndex].Name != "Semester") return;
            if (string.IsNullOrWhiteSpace(e.FormattedValue?.ToString())) return;

            if (!int.TryParse(e.FormattedValue.ToString(), out _))
            {
                MessageBox.Show("EL SEMESTRE DEBE SER UN NÚMERO ENTERO (O DEJARSE VACÍO).",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private void CargarCursosEnPlantilla()
        {
            _cursosEnPlantilla = Ctrl_CareerCourses.MostrarCursosPorCarrera(_careerIdActiva, "ACTIVA");

            // Incluir los pendientes de agregar en memoria
            var pendientes = _asignacionesPendientesAgregar
                .Where(p => !_cursosEnPlantilla.Any(e => e.CourseId == p.CourseId))
                .ToList();
            _cursosEnPlantilla.AddRange(pendientes);

            // Excluir los marcados para eliminar
            _cursosEnPlantilla = _cursosEnPlantilla
                .Where(p => !_careerCourseIdsParaEliminar.Contains(p.CareerCourseId))
                .ToList();

            AplicarFiltroEnPlantilla();

            Lbl_Conteo.Text = $"CURSOS ASIGNADOS A \"{_careerNombreActiva}\": {_cursosEnPlantilla.Count}";
        }

        private void AplicarFiltroEnPlantilla()
        {
            var datos = _cursosEnPlantilla ?? new List<Mdl_CareerCourses>();

            if (!string.IsNullOrWhiteSpace(_ultimoTextoEnPlantilla))
            {
                string texto = _ultimoTextoEnPlantilla.ToUpper();
                datos = datos.Where(p =>
                    (p.CourseCode?.ToUpper().Contains(texto) ?? false) ||
                    (p.CourseName?.ToUpper().Contains(texto) ?? false)).ToList();
            }

            var data = datos.Select(p => new
            {
                p.CareerCourseId,
                p.CourseId,
                p.CourseCode,
                p.CourseName,
                p.Semester,
                p.IsRequired,
                p.Prerequisites
            }).ToList();

            Tabla.DataSource = data;
            ConfigurarColumnaTabla();
        }

        private void ConfigurarColumnaTabla()
        {
            if (Tabla.Columns.Count == 0) return;

            if (Tabla.Columns.Contains("CareerCourseId"))
                Tabla.Columns["CareerCourseId"].Visible = false;
            if (Tabla.Columns.Contains("CourseId"))
                Tabla.Columns["CourseId"].Visible = false;

            if (Tabla.Columns.Contains("CourseCode"))
            {
                Tabla.Columns["CourseCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["CourseCode"].ReadOnly = true;
                Tabla.Columns["CourseCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CourseCode"].FillWeight = 20;
            }
            if (Tabla.Columns.Contains("CourseName"))
            {
                Tabla.Columns["CourseName"].HeaderText = "CURSO";
                Tabla.Columns["CourseName"].ReadOnly = true;
                Tabla.Columns["CourseName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CourseName"].FillWeight = 33;
            }
            if (Tabla.Columns.Contains("Semester"))
            {
                Tabla.Columns["Semester"].HeaderText = "SEMESTRE";
                Tabla.Columns["Semester"].ReadOnly = false;
                Tabla.Columns["Semester"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Tabla.Columns["Semester"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Semester"].FillWeight = 12;
            }
            if (Tabla.Columns.Contains("IsRequired"))
            {
                Tabla.Columns["IsRequired"].HeaderText = "OBLIGATORIO";
                Tabla.Columns["IsRequired"].ReadOnly = false;
                if (Tabla.Columns["IsRequired"] is DataGridViewCheckBoxColumn chk)
                    chk.ThreeState = true; // nulo = indeterminado (aún no definido)
                Tabla.Columns["IsRequired"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["IsRequired"].FillWeight = 15;
            }
            if (Tabla.Columns.Contains("Prerequisites"))
            {
                Tabla.Columns["Prerequisites"].HeaderText = "PREREQUISITOS";
                Tabla.Columns["Prerequisites"].ReadOnly = false;
                Tabla.Columns["Prerequisites"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Prerequisites"].FillWeight = 20;
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
            Txt_ValorBuscado.Text = "BUSCAR CURSO...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Filtro1.SelectedIndex = 0;
            _ultimoTextoEnPlantilla = "";
            AplicarFiltroEnPlantilla();
        }

        private void Btn_Search2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(ComboBox_TemplateBase.SelectedItem is CareerItem baseSeleccionada) ||
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

            if (!(ComboBox_TemplateBase.SelectedItem is CareerItem baseSeleccionada) ||
                ComboBox_TemplateBase.SelectedIndex <= 0)
                return;

            CargarTablaBase(baseSeleccionada, "");
        }

        #endregion Search
        #region AccionesCursos

        private void Btn_CopySelected_Click(object sender, EventArgs e)
        {
            try
            {
                if (_careerIdActiva <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CARRERA ACTIVA PRIMERO.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Tabla2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN CURSO DE LA PLANTILLA BASE.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow fila in Tabla2.SelectedRows)
                {
                    AgregarPendiente(fila);
                }

                CargarCursosEnPlantilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL AGREGAR CURSO: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_CopyAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (_careerIdActiva <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CARRERA ACTIVA PRIMERO.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Tabla2.Rows.Count == 0)
                {
                    MessageBox.Show("LA PLANTILLA BASE NO TIENE CURSOS PARA COPIAR.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE COPIAR TODOS LOS CURSOS DE LA PLANTILLA BASE A \"{_careerNombreActiva}\"?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                foreach (DataGridViewRow fila in Tabla2.Rows)
                {
                    if (fila.IsNewRow) continue;
                    AgregarPendiente(fila);
                }

                CargarCursosEnPlantilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL AGREGAR TODOS LOS CURSOS: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Agrega una fila de Tabla2 a la lista de pendientes, si no está ya asignada ni pendiente
        // Semester/IsRequired/Prerequisites quedan en null: se completan editando la celda antes de guardar
        private void AgregarPendiente(DataGridViewRow fila)
        {
            if (fila.Cells["CourseId"].Value == null) return;

            int courseId = Convert.ToInt32(fila.Cells["CourseId"].Value);
            string courseCode = fila.Cells["CourseCode"].Value?.ToString() ?? "";
            string courseName = fila.Cells["CourseName"].Value?.ToString() ?? "";

            bool yaEnPlantilla = _cursosEnPlantilla?.Any(p => p.CourseId == courseId) ?? false;
            bool yaPendiente = _asignacionesPendientesAgregar.Any(p => p.CourseId == courseId);

            if (!yaEnPlantilla && !yaPendiente)
            {
                _asignacionesPendientesAgregar.Add(new Mdl_CareerCourses
                {
                    CareerCourseId = 0,
                    CareerId = _careerIdActiva,
                    CourseId = courseId,
                    CourseCode = courseCode,
                    CourseName = courseName,
                    Semester = null,
                    IsRequired = null,
                    Prerequisites = null,
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
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN CURSO DE LA PLANTILLA ACTIVA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow fila in Tabla.SelectedRows)
                {
                    if (fila.Cells["CourseId"].Value == null) continue;

                    int courseId = Convert.ToInt32(fila.Cells["CourseId"].Value);

                    var enPlantilla = _cursosEnPlantilla?.FirstOrDefault(p => p.CourseId == courseId);
                    if (enPlantilla == null) continue;

                    if (enPlantilla.CareerCourseId > 0)
                    {
                        // Existe en BD — marcar para eliminar al guardar
                        if (!_careerCourseIdsParaEliminar.Contains(enPlantilla.CareerCourseId))
                            _careerCourseIdsParaEliminar.Add(enPlantilla.CareerCourseId);
                    }
                    else
                    {
                        // Solo está en memoria — quitar de pendientes
                        var pendiente = _asignacionesPendientesAgregar.FirstOrDefault(p => p.CourseId == courseId);
                        if (pendiente != null)
                            _asignacionesPendientesAgregar.Remove(pendiente);
                    }
                }

                CargarCursosEnPlantilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL QUITAR CURSO: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_RemoveAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (_cursosEnPlantilla == null || _cursosEnPlantilla.Count == 0)
                {
                    MessageBox.Show("LA PLANTILLA ACTIVA ESTÁ VACÍA.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE QUITAR TODOS LOS CURSOS DE LA CARRERA \"{_careerNombreActiva}\"?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion != DialogResult.Yes) return;

                foreach (var asignacion in _cursosEnPlantilla)
                {
                    if (asignacion.CareerCourseId > 0 && !_careerCourseIdsParaEliminar.Contains(asignacion.CareerCourseId))
                        _careerCourseIdsParaEliminar.Add(asignacion.CareerCourseId);
                }

                _asignacionesPendientesAgregar.Clear();

                CargarCursosEnPlantilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL QUITAR TODOS LOS CURSOS: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion AccionesCursos
        #region GuardarCambios

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            try
            {
                if (_careerIdActiva <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CARRERA ACTIVA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_asignacionesPendientesAgregar.Count == 0 && _careerCourseIdsParaEliminar.Count == 0)
                {
                    MessageBox.Show("NO HAY CAMBIOS PENDIENTES POR GUARDAR.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string mensaje = "SE GUARDARÁN LOS SIGUIENTES CAMBIOS:\n\n" +
                    $"• CURSOS A ASIGNAR: {_asignacionesPendientesAgregar.Count}\n" +
                    $"• CURSOS A QUITAR: {_careerCourseIdsParaEliminar.Count}\n\n¿CONFIRMAR CAMBIOS?";

                var confirmacion = MessageBox.Show(mensaje, "CONFIRMAR GUARDADO",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                this.Cursor = Cursors.WaitCursor;

                int usuarioId = UserData?.UserId ?? 0;

                // Primero capturar Semester/IsRequired/Prerequisites editados en la grilla para los ya existentes
                ActualizarCursosDesdeGrilla(usuarioId);

                int errores = 0;

                // Quitar (inactivar) los marcados para eliminar
                foreach (int careerCourseId in _careerCourseIdsParaEliminar)
                {
                    int res = Ctrl_CareerCourses.CambiarEstadoCurso(careerCourseId, 1, usuarioId);
                    if (res <= 0) errores++;
                }

                // Insertar las nuevas asignaciones
                foreach (var asignacion in _asignacionesPendientesAgregar)
                {
                    int res = Ctrl_CareerCourses.RegistrarCurso(asignacion, usuarioId);
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
                _careerCourseIdsParaEliminar.Clear();
                CargarCursosEnPlantilla();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("ERROR AL GUARDAR: " + ex.Message,
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Actualiza en BD los registros existentes en Tabla (plantilla activa) con lo editado en la grilla
        private void ActualizarCursosDesdeGrilla(int usuarioId)
        {
            if (Tabla.Rows.Count == 0) return;

            foreach (DataGridViewRow fila in Tabla.Rows)
            {
                if (fila.IsNewRow) continue;
                if (!Tabla.Columns.Contains("CareerCourseId")) break;

                int careerCourseId = 0;
                if (fila.Cells["CareerCourseId"].Value != null)
                    int.TryParse(fila.Cells["CareerCourseId"].Value.ToString(), out careerCourseId);

                if (careerCourseId <= 0) continue;

                int courseId = Convert.ToInt32(fila.Cells["CourseId"].Value);

                int? semester = null;
                if (fila.Cells["Semester"].Value != null &&
                    int.TryParse(fila.Cells["Semester"].Value.ToString(), out int sem))
                    semester = sem;

                bool? isRequired = fila.Cells["IsRequired"].Value as bool?;

                string prerequisites = fila.Cells["Prerequisites"].Value?.ToString();

                Ctrl_CareerCourses.ActualizarCurso(new Mdl_CareerCourses
                {
                    CareerCourseId = careerCourseId,
                    CareerId = _careerIdActiva,
                    CourseId = courseId,
                    Semester = semester,
                    IsRequired = isRequired,
                    Prerequisites = string.IsNullOrWhiteSpace(prerequisites) ? null : prerequisites
                }, usuarioId);
            }
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            if (_asignacionesPendientesAgregar.Count == 0 && _careerCourseIdsParaEliminar.Count == 0)
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
            _careerCourseIdsParaEliminar.Clear();

            if (_careerIdActiva > 0)
                CargarCursosEnPlantilla();
        }

        #endregion GuardarCambios
    }

    // CLASE AUXILIAR PARA MANEJAR CARRERAS EN LOS COMBOS DE ESTE FORMULARIO
    internal class CareerItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public CareerItem(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public override string ToString() => Nombre;
    }
}