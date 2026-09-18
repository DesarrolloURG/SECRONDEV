using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SECRON.Controllers;
using SECRON.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_Sections : Form
    {
        #region Campos

        public Mdl_Security_UserInfo UserData { get; set; }

        private Ctrl_Security_Auth authController = new Ctrl_Security_Auth();
        private List<string> permisosUsuario = new List<string>();

        private int _sectionIdSeleccionado = 0;
        private int _paginaActual = 1;
        private int _totalPaginas = 1;
        private const int TAMANIO_PAGINA = 100;

        private string _ultimoCampo = "TODOS";
        private string _ultimoValor = null;
        private int? _ultimoLocationId = null;
        private string _ultimoEstado = "TODOS";

        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        // Evita que el combo de Sede dispare la limpieza de Carrera/Coordinador
        // cuando estamos cargando un registro seleccionado del grid (no es un cambio real del usuario).
        private bool _cargandoDesdeSeleccion = false;

        #endregion

        #region Constructor

        public Frm_AcademicProcesses_Sections()
        {
            InitializeComponent();

            this.Load -= Frm_AcademicProcesses_Sections_Load;
            this.Load += Frm_AcademicProcesses_Sections_Load;

            ComboBox_LocationId.SelectedIndexChanged -= ComboBox_LocationId_SelectedIndexChanged;
            ComboBox_LocationId.SelectedIndexChanged += ComboBox_LocationId_SelectedIndexChanged;

            Num_MaxCapacity.ValueChanged -= Num_MaxCapacity_ValueChanged;
            Num_MaxCapacity.ValueChanged += Num_MaxCapacity_ValueChanged;

            Btn_Save.Click -= Btn_Save_Click;
            Btn_Save.Click += Btn_Save_Click;

            Btn_Update.Click -= Btn_Update_Click;
            Btn_Update.Click += Btn_Update_Click;

            Btn_IsActive.Click -= Btn_IsActive_Click;
            Btn_IsActive.Click += Btn_IsActive_Click;

            Btn_Clear.Click -= Btn_Clear_Click;
            Btn_Clear.Click += Btn_Clear_Click;

            Btn_Search.Click -= Btn_Search_Click;
            Btn_Search.Click += Btn_Search_Click;

            Btn_CleanSearch.Click -= Btn_CleanSearch_Click;
            Btn_CleanSearch.Click += Btn_CleanSearch_Click;

            Btn_Export.Click -= Btn_Export_Click;
            Btn_Export.Click += Btn_Export_Click;
        }

        #endregion

        #region Load

        private async void Frm_AcademicProcesses_Sections_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarNumericos();
                ConfigurarComboBoxesFijos();
                ConfigurarFiltros();
                ConfigurarTabla();
                CrearToolStripPaginacion();
                LimpiarCampos();
                BuscarSecciones();

                if (UserData != null)
                {
                    await CargarPermisosUsuario(UserData.UserId);
                    ConfigurarBotonesPorPermisos();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("ERROR AL CARGAR FORMULARIO: " + ex.Message, "ERROR SECRON",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Configuración de combos, numéricos y filtros

        private void ConfigurarNumericos()
        {
            Num_CurrentSemester.Minimum = 1;
            Num_CurrentSemester.Maximum = 20;
            Num_CurrentSemester.Value = 1;

            Num_AcademicYear.Minimum = 2020;
            Num_AcademicYear.Maximum = 2100;
            Num_AcademicYear.Value = DateTime.Now.Year;

            Num_MaxCapacity.Minimum = 1;
            Num_MaxCapacity.Maximum = 500;
            Num_MaxCapacity.Value = 30; // Default de la tabla

            Num_StudentCount.Minimum = 0;
            Num_StudentCount.Maximum = Num_MaxCapacity.Value;
            Num_StudentCount.Value = 0;
        }

        // El máximo de estudiantes no puede pasar la capacidad máxima definida
        private void Num_MaxCapacity_ValueChanged(object sender, EventArgs e)
        {
            Num_StudentCount.Maximum = Num_MaxCapacity.Value;
        }

        private void ConfigurarComboBoxesFijos()
        {
            // Sede (captura) — catálogo completo de sedes activas
            ComboBox_LocationId.DisplayMember = "Value";
            ComboBox_LocationId.ValueMember = "Key";
            ComboBox_LocationId.DataSource = Ctrl_Locations.ObtenerLocationsActivas();
            ComboBox_LocationId.SelectedIndex = -1;

            // Jornada (captura) — catálogo completo de jornadas activas, sin filtro por sede
            var jornadas = Ctrl_ScheduleTypes.ObtenerScheduleTypesParaExportar("TODOS", null, "TODOS", "ACTIVOS");
            ComboBox_ScheduleType.DisplayMember = "ScheduleTypeName";
            ComboBox_ScheduleType.ValueMember = "ScheduleTypeId";
            ComboBox_ScheduleType.DataSource = jornadas;
            ComboBox_ScheduleType.SelectedIndex = -1;

            // Carrera y Coordinador arrancan deshabilitados hasta elegir Sede
            ComboBox_Career.Items.Clear();
            ComboBox_Career.Enabled = false;
            ComboBox_Coordinator.Items.Clear();
            ComboBox_Coordinator.Enabled = false;
        }

        // Cascada: al elegir Sede, se cargan Carrera y Coordinador filtrados por esa sede
        private void ComboBox_LocationId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoDesdeSeleccion) return;

            if (ComboBox_LocationId.SelectedIndex == -1)
            {
                ComboBox_Career.DataSource = null;
                ComboBox_Career.Items.Clear();
                ComboBox_Career.Enabled = false;

                ComboBox_Coordinator.DataSource = null;
                ComboBox_Coordinator.Items.Clear();
                ComboBox_Coordinator.Enabled = false;
                return;
            }

            int locationId = Convert.ToInt32(((KeyValuePair<int, string>)ComboBox_LocationId.SelectedItem).Key);

            var carreras = Ctrl_LocationCareers.MostrarCarrerasPorSede(locationId);
            ComboBox_Career.DisplayMember = "CareerName";
            ComboBox_Career.ValueMember = "CareerId";
            ComboBox_Career.DataSource = carreras;
            ComboBox_Career.SelectedIndex = carreras.Count > 0 ? -1 : -1;
            ComboBox_Career.Enabled = carreras.Count > 0;

            var coordinadores = Ctrl_Coordinators.ObtenerCoordinadoresPorSede(locationId);
            ComboBox_Coordinator.DisplayMember = "FullName";
            ComboBox_Coordinator.ValueMember = "CoordinatorId";
            ComboBox_Coordinator.DataSource = coordinadores;
            ComboBox_Coordinator.SelectedIndex = -1;
            ComboBox_Coordinator.Enabled = coordinadores.Count > 0;

            if (carreras.Count == 0)
                MessageBox.Show("ESTA SEDE NO TIENE CARRERAS ASIGNADAS TODAVÍA.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (coordinadores.Count == 0)
                MessageBox.Show("ESTA SEDE NO TIENE COORDINADORES ASIGNADOS TODAVÍA.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ConfigurarFiltros()
        {
            Filtro1.Items.Clear();
            Filtro1.Items.AddRange(new object[] { "TODOS", "CÓDIGO", "NOMBRE" });
            Filtro1.SelectedIndex = 0;

            // Filtro2 = Sede (para filtrar el listado) — "TODOS" + catálogo de sedes
            var sedes = Ctrl_Locations.ObtenerLocationsActivas();
            Filtro2.DisplayMember = "Value";
            Filtro2.ValueMember = "Key";
            var listaConTodos = new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(0, "TODOS") };
            listaConTodos.AddRange(sedes);
            Filtro2.DataSource = listaConTodos;
            Filtro2.SelectedIndex = 0;

            Filtro3.Items.Clear();
            Filtro3.Items.AddRange(new object[] { "TODOS", "ACTIVOS", "INACTIVOS" });
            Filtro3.SelectedIndex = 0;
        }

        #endregion

        #region Tabla

        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;

            var colEstadoRegistro = new DataGridViewTextBoxColumn();
            colEstadoRegistro.Name = "ColEstadoRegistro";
            colEstadoRegistro.HeaderText = "ESTADO";
            colEstadoRegistro.ReadOnly = true;
            colEstadoRegistro.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.Columns.Add(colEstadoRegistro);

            Tabla.Columns.Add("SectionId", "ID");
            Tabla.Columns.Add("SectionCode", "CÓDIGO");
            Tabla.Columns.Add("SectionName", "NOMBRE");
            Tabla.Columns.Add("CareerName", "CARRERA");
            Tabla.Columns.Add("LocationName", "SEDE");
            Tabla.Columns.Add("ScheduleTypeName", "JORNADA");
            Tabla.Columns.Add("CoordinatorName", "COORDINADOR");
            Tabla.Columns.Add("CurrentSemester", "SEM.");
            Tabla.Columns.Add("AcademicYear", "AÑO");
            Tabla.Columns.Add("StudentCount", "ESTUDIANTES");
            Tabla.Columns.Add("MaxCapacity", "CAPACIDAD");
            Tabla.Columns.Add("StartDate", "INICIO");
            Tabla.Columns.Add("EndDate", "FIN");

            Tabla.Columns["SectionId"].Visible = false;

            Tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Tabla.Columns["ColEstadoRegistro"].FillWeight = 8;
            Tabla.Columns["SectionCode"].FillWeight = 9;
            Tabla.Columns["SectionName"].FillWeight = 12;
            Tabla.Columns["CareerName"].FillWeight = 12;
            Tabla.Columns["LocationName"].FillWeight = 10;
            Tabla.Columns["ScheduleTypeName"].FillWeight = 9;
            Tabla.Columns["CoordinatorName"].FillWeight = 12;
            Tabla.Columns["CurrentSemester"].FillWeight = 5;
            Tabla.Columns["AcademicYear"].FillWeight = 5;
            Tabla.Columns["StudentCount"].FillWeight = 7;
            Tabla.Columns["MaxCapacity"].FillWeight = 7;
            Tabla.Columns["StartDate"].FillWeight = 7;
            Tabla.Columns["EndDate"].FillWeight = 7;

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;

            Tabla.CellFormatting -= Tabla_CellFormatting_Estado;
            Tabla.CellFormatting += Tabla_CellFormatting_Estado;

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void Tabla_CellFormatting_Estado(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (Tabla.Columns[e.ColumnIndex].Name != "ColEstadoRegistro") return;
            if (e.Value == null) return;

            bool activa = e.Value.ToString() == "ACTIVA";
            e.CellStyle.ForeColor = activa ? Color.FromArgb(0, 128, 0) : Color.FromArgb(200, 0, 0);
            e.CellStyle.Font = new Font(Tabla.Font, FontStyle.Bold);
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.CurrentRow == null || Tabla.CurrentRow.Index < 0) return;

            try
            {
                _sectionIdSeleccionado = Convert.ToInt32(Tabla.CurrentRow.Cells["SectionId"].Value);

                int totalRows;
                var lista = Ctrl_Sections.ObtenerSecciones("TODOS", null, null, "TODOS", 1, 0, out totalRows);
                var seleccionada = lista.FirstOrDefault(x => x.SectionId == _sectionIdSeleccionado);
                if (seleccionada == null) return;

                _cargandoDesdeSeleccion = true;

                Txt_Code.Text = seleccionada.SectionCode;
                Txt_Name.Text = seleccionada.SectionName;

                ComboBox_LocationId.SelectedValue = seleccionada.LocationId;

                var carreras = Ctrl_LocationCareers.MostrarCarrerasPorSede(seleccionada.LocationId);
                ComboBox_Career.DisplayMember = "CareerName";
                ComboBox_Career.ValueMember = "CareerId";
                ComboBox_Career.DataSource = carreras;
                ComboBox_Career.Enabled = true;
                ComboBox_Career.SelectedValue = seleccionada.CareerId;

                var coordinadores = Ctrl_Coordinators.ObtenerCoordinadoresPorSede(seleccionada.LocationId);
                ComboBox_Coordinator.DisplayMember = "FullName";
                ComboBox_Coordinator.ValueMember = "CoordinatorId";
                ComboBox_Coordinator.DataSource = coordinadores;
                ComboBox_Coordinator.Enabled = true;
                ComboBox_Coordinator.SelectedValue = seleccionada.CoordinatorId;

                ComboBox_ScheduleType.SelectedValue = seleccionada.ScheduleTypeId;

                Num_CurrentSemester.Value = seleccionada.CurrentSemester;
                Num_AcademicYear.Value = seleccionada.AcademicYear ?? DateTime.Now.Year;
                Num_MaxCapacity.Value = seleccionada.MaxCapacity;
                Num_StudentCount.Value = Math.Min(seleccionada.StudentCount, Num_MaxCapacity.Value);

                if (seleccionada.StartDate.HasValue) Dtp_StartDate.Value = seleccionada.StartDate.Value;
                if (seleccionada.EndDate.HasValue) Dtp_EndDate.Value = seleccionada.EndDate.Value;

                _cargandoDesdeSeleccion = false;
            }
            catch (Exception ex)
            {
                _cargandoDesdeSeleccion = false;
                MessageBox.Show("ERROR AL CARGAR REGISTRO SELECCIONADO: " + ex.Message, "ERROR SECRON",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Búsqueda y listado (paginado)

        private void BuscarSecciones()
        {
            _paginaActual = 1;

            _ultimoCampo = Filtro1.SelectedItem?.ToString() == "CÓDIGO" ? "CODIGO"
                          : Filtro1.SelectedItem?.ToString() == "NOMBRE" ? "NOMBRE"
                          : "TODOS";
            _ultimoValor = string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text) ? null : Txt_ValorBuscado.Text.Trim();

            var sedeSeleccionada = (KeyValuePair<int, string>?)Filtro2.SelectedItem;
            _ultimoLocationId = (sedeSeleccionada.HasValue && sedeSeleccionada.Value.Key != 0) ? sedeSeleccionada.Value.Key : (int?)null;

            _ultimoEstado = Filtro3.SelectedItem?.ToString() ?? "TODOS";

            MostrarSeccionesEnTabla();
        }

        private void MostrarSeccionesEnTabla()
        {
            int totalRows;
            var lista = Ctrl_Sections.ObtenerSecciones(_ultimoCampo, _ultimoValor, _ultimoLocationId, _ultimoEstado,
                                                        _paginaActual, TAMANIO_PAGINA, out totalRows);

            Tabla.Rows.Clear();

            foreach (var s in lista)
            {
                int fila = Tabla.Rows.Add();
                Tabla.Rows[fila].Cells["ColEstadoRegistro"].Value = s.IsActive ? "ACTIVA" : "INACTIVA";
                Tabla.Rows[fila].Cells["SectionId"].Value = s.SectionId;
                Tabla.Rows[fila].Cells["SectionCode"].Value = s.SectionCode;
                Tabla.Rows[fila].Cells["SectionName"].Value = s.SectionName;
                Tabla.Rows[fila].Cells["CareerName"].Value = s.CareerName;
                Tabla.Rows[fila].Cells["LocationName"].Value = s.LocationName;
                Tabla.Rows[fila].Cells["ScheduleTypeName"].Value = s.ScheduleTypeName;
                Tabla.Rows[fila].Cells["CoordinatorName"].Value = s.CoordinatorName;
                Tabla.Rows[fila].Cells["CurrentSemester"].Value = s.CurrentSemester;
                Tabla.Rows[fila].Cells["AcademicYear"].Value = s.AcademicYear;
                Tabla.Rows[fila].Cells["StudentCount"].Value = s.StudentCount;
                Tabla.Rows[fila].Cells["MaxCapacity"].Value = s.MaxCapacity;
                Tabla.Rows[fila].Cells["StartDate"].Value = s.StartDate?.ToString("dd/MM/yyyy");
                Tabla.Rows[fila].Cells["EndDate"].Value = s.EndDate?.ToString("dd/MM/yyyy");
            }

            _totalPaginas = totalRows == 0 ? 1 : (int)Math.Ceiling(totalRows / (double)TAMANIO_PAGINA);

            int desde = totalRows == 0 ? 0 : ((_paginaActual - 1) * TAMANIO_PAGINA) + 1;
            int hasta = Math.Min(_paginaActual * TAMANIO_PAGINA, totalRows);
            Lbl_Paginas.Text = $"MOSTRANDO {desde}-{hasta} DE {totalRows}";

            if (btnAnterior != null) btnAnterior.Enabled = _paginaActual > 1;
            if (btnSiguiente != null) btnSiguiente.Enabled = _paginaActual < _totalPaginas;
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1 || nuevaPagina > _totalPaginas) return;
            _paginaActual = nuevaPagina;
            MostrarSeccionesEnTabla();
        }

        private void CrearToolStripPaginacion()
        {
            toolStripPaginacion = new ToolStrip();
            toolStripPaginacion.Dock = DockStyle.None;
            toolStripPaginacion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            toolStripPaginacion.GripStyle = ToolStripGripStyle.Hidden;
            toolStripPaginacion.BackColor = Color.FromArgb(248, 249, 250);
            toolStripPaginacion.Height = 39;
            toolStripPaginacion.AutoSize = true;

            btnAnterior = new ToolStripButton();
            btnAnterior.Text = "❮ Anterior";
            btnAnterior.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnterior.ForeColor = Color.White;
            btnAnterior.BackColor = Color.FromArgb(51, 140, 255);
            btnAnterior.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAnterior.Margin = new Padding(2);
            btnAnterior.Padding = new Padding(8, 4, 8, 4);
            btnAnterior.Click += (s, e) => CambiarPagina(_paginaActual - 1);
            toolStripPaginacion.Items.Add(btnAnterior);

            btnSiguiente = new ToolStripButton();
            btnSiguiente.Text = "Siguiente ❯";
            btnSiguiente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSiguiente.ForeColor = Color.White;
            btnSiguiente.BackColor = Color.FromArgb(238, 143, 109);
            btnSiguiente.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSiguiente.Margin = new Padding(2);
            btnSiguiente.Padding = new Padding(8, 4, 8, 4);
            btnSiguiente.Click += (s, e) => CambiarPagina(_paginaActual + 1);
            toolStripPaginacion.Items.Add(btnSiguiente);

            PanelToolStrip.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.Location = new Point(PanelToolStrip.Width - 260, 0);
            toolStripPaginacion.BringToFront();
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            BuscarSecciones();
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "";
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 0;
            BuscarSecciones();
        }

        #endregion

        #region CRUD

        private void LimpiarCampos()
        {
            _sectionIdSeleccionado = 0;
            Txt_Code.Text = "";
            Txt_Name.Text = "";

            _cargandoDesdeSeleccion = true;
            ComboBox_LocationId.SelectedIndex = -1;
            ComboBox_Career.DataSource = null;
            ComboBox_Career.Items.Clear();
            ComboBox_Career.Enabled = false;
            ComboBox_Coordinator.DataSource = null;
            ComboBox_Coordinator.Items.Clear();
            ComboBox_Coordinator.Enabled = false;
            ComboBox_ScheduleType.SelectedIndex = -1;
            _cargandoDesdeSeleccion = false;

            Num_CurrentSemester.Value = 1;
            Num_AcademicYear.Value = DateTime.Now.Year;
            Num_MaxCapacity.Value = 30;
            Num_StudentCount.Value = 0;

            // Dtp_StartDate y Dtp_EndDate NO se limpian a propósito (campos "pegajosos"
            // para agilizar el registro por lote de varias secciones del mismo semestre).

            Tabla.ClearSelection();
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(Txt_Code.Text))
            {
                MessageBox.Show("EL CÓDIGO ES OBLIGATORIO.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(Txt_Name.Text))
            {
                MessageBox.Show("EL NOMBRE ES OBLIGATORIO.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (ComboBox_LocationId.SelectedIndex == -1)
            {
                MessageBox.Show("DEBE SELECCIONAR LA SEDE.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (ComboBox_Career.SelectedIndex == -1)
            {
                MessageBox.Show("DEBE SELECCIONAR LA CARRERA.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (ComboBox_ScheduleType.SelectedIndex == -1)
            {
                MessageBox.Show("DEBE SELECCIONAR LA JORNADA.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (ComboBox_Coordinator.SelectedIndex == -1)
            {
                MessageBox.Show("DEBE SELECCIONAR EL COORDINADOR.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (Dtp_EndDate.Value.Date <= Dtp_StartDate.Value.Date)
            {
                MessageBox.Show("LA FECHA FIN DEBE SER POSTERIOR A LA FECHA DE INICIO.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (Num_StudentCount.Value > Num_MaxCapacity.Value)
            {
                MessageBox.Show("LA CANTIDAD DE ESTUDIANTES NO PUEDE SUPERAR LA CAPACIDAD MÁXIMA.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private Mdl_Sections ConstruirModeloDesdeCampos()
        {
            return new Mdl_Sections
            {
                SectionId = _sectionIdSeleccionado,
                SectionCode = Txt_Code.Text.Trim(),
                SectionName = Txt_Name.Text.Trim(),
                LocationId = Convert.ToInt32(ComboBox_LocationId.SelectedValue),
                CareerId = Convert.ToInt32(ComboBox_Career.SelectedValue),
                ScheduleTypeId = Convert.ToInt32(ComboBox_ScheduleType.SelectedValue),
                CoordinatorId = Convert.ToInt32(ComboBox_Coordinator.SelectedValue),
                CurrentSemester = (int)Num_CurrentSemester.Value,
                AcademicYear = (int)Num_AcademicYear.Value,
                StudentCount = (int)Num_StudentCount.Value,
                MaxCapacity = (int)Num_MaxCapacity.Value,
                StartDate = Dtp_StartDate.Value.Date,
                EndDate = Dtp_EndDate.Value.Date
            };
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            var seccion = ConstruirModeloDesdeCampos();
            int usuarioId = UserData?.UserId ?? 0;

            int resultado = Ctrl_Sections.InsertarSeccion(seccion, usuarioId);

            if (resultado > 0)
            {
                MessageBox.Show("SECCIÓN REGISTRADA CORRECTAMENTE.", "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                BuscarSecciones();
            }
            else
            {
                MessageBox.Show("NO SE PUDO REGISTRAR LA SECCIÓN. VERIFIQUE QUE EL CÓDIGO NO ESTÉ DUPLICADO.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (_sectionIdSeleccionado <= 0)
            {
                MessageBox.Show("SELECCIONE UNA SECCIÓN DEL LISTADO PARA EDITAR.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ValidarCampos()) return;

            var seccion = ConstruirModeloDesdeCampos();
            int usuarioId = UserData?.UserId ?? 0;

            int resultado = Ctrl_Sections.ActualizarSeccion(seccion, usuarioId);

            if (resultado > 0)
            {
                MessageBox.Show("SECCIÓN ACTUALIZADA CORRECTAMENTE.", "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                BuscarSecciones();
            }
            else
            {
                MessageBox.Show("NO SE PUDO ACTUALIZAR LA SECCIÓN.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Btn_IsActive_Click(object sender, EventArgs e)
        {
            if (_sectionIdSeleccionado <= 0)
            {
                MessageBox.Show("SELECCIONE UNA SECCIÓN DEL LISTADO.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string estadoActual = Tabla.CurrentRow?.Cells["ColEstadoRegistro"].Value?.ToString();
            bool estaActiva = estadoActual == "ACTIVA";
            int modo = estaActiva ? 1 : 2;
            string accion = estaActiva ? "INACTIVAR" : "REACTIVAR";

            var confirmacion = MessageBox.Show($"¿DESEA {accion} ESTA SECCIÓN?", "CONFIRMAR",
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            int usuarioId = UserData?.UserId ?? 0;
            int resultado = Ctrl_Sections.CambiarEstadoSeccion(_sectionIdSeleccionado, modo, usuarioId);

            if (resultado > 0)
            {
                MessageBox.Show($"SECCIÓN {(estaActiva ? "INACTIVADA" : "REACTIVADA")} CORRECTAMENTE.", "ÉXITO",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                BuscarSecciones();
            }
            else
            {
                MessageBox.Show("NO SE PUDO CAMBIAR EL ESTADO DE LA SECCIÓN.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region Sistema de Permisos

        private async Task CargarPermisosUsuario(int userId)
        {
            try
            {
                // El 2do parámetro (roleId) ya no se usa dentro de ObtenerPermisosUsuarioAsync
                // (los roles se resuelven internamente vía UserRoles) — se conserva solo por compatibilidad de firma.
                permisosUsuario = await authController.ObtenerPermisosUsuarioAsync(userId, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CARGAR PERMISOS: " + ex.Message, "ERROR SECRON",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TienePermiso(string permissionCode)
        {
            return permisosUsuario != null && permisosUsuario.Contains(permissionCode);
        }

        private void ConfigurarBotonesPorPermisos()
        {
            Btn_Save.Enabled = TienePermiso("ACADEMICPROCESSES_SECTIONS_CREATE");
            if (!Btn_Save.Enabled) { Btn_Save.BackColor = Color.FromArgb(200, 200, 200); Btn_Save.ForeColor = Color.Gray; Btn_Save.Cursor = Cursors.No; }

            Btn_Update.Enabled = TienePermiso("ACADEMICPROCESSES_SECTIONS_UPDATE");
            if (!Btn_Update.Enabled) { Btn_Update.BackColor = Color.FromArgb(200, 200, 200); Btn_Update.ForeColor = Color.Gray; Btn_Update.Cursor = Cursors.No; }

            Btn_IsActive.Enabled = TienePermiso("ACADEMICPROCESSES_SECTIONS_INACTIVE");
            if (!Btn_IsActive.Enabled) { Btn_IsActive.BackColor = Color.FromArgb(200, 200, 200); Btn_IsActive.ForeColor = Color.Gray; Btn_IsActive.Cursor = Cursors.No; }
        }

        #endregion

        #region Exportar

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            List<Mdl_Sections> listaExportar = Ctrl_Sections.ObtenerSeccionesParaExportar(
                _ultimoCampo, _ultimoValor, _ultimoLocationId, _ultimoEstado);

            if (listaExportar.Count == 0)
            {
                MessageBox.Show("NO HAY REGISTROS PARA EXPORTAR CON EL FILTRO ACTUAL.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Guardar listado de secciones",
                FileName = "SECCIONES_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                Excel.Application excelApp = null;
                Excel.Workbook workbook = null;
                Excel.Worksheet worksheet = null;

                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    excelApp = new Excel.Application { Visible = false };
                    workbook = excelApp.Workbooks.Add();
                    worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "SECCIONES";

                    string[] headers = {
                        "CODIGO", "NOMBRE", "CARRERA", "SEDE", "JORNADA", "COORDINADOR",
                        "SEMESTRE", "AÑO", "ESTUDIANTES", "CAPACIDAD", "INICIO", "FIN", "ACTIVA"
                    };

                    for (int i = 0; i < headers.Length; i++)
                        worksheet.Cells[1, i + 1] = headers[i];

                    var headerRange = worksheet.Range["A1:M1"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Color = ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    int fila = 2;
                    foreach (var s in listaExportar)
                    {
                        worksheet.Cells[fila, 1] = s.SectionCode;
                        worksheet.Cells[fila, 2] = s.SectionName;
                        worksheet.Cells[fila, 3] = s.CareerName;
                        worksheet.Cells[fila, 4] = s.LocationName;
                        worksheet.Cells[fila, 5] = s.ScheduleTypeName;
                        worksheet.Cells[fila, 6] = s.CoordinatorName;
                        worksheet.Cells[fila, 7] = s.CurrentSemester;
                        worksheet.Cells[fila, 8] = s.AcademicYear;
                        worksheet.Cells[fila, 9] = s.StudentCount;
                        worksheet.Cells[fila, 10] = s.MaxCapacity;
                        worksheet.Cells[fila, 11] = s.StartDate?.ToString("dd/MM/yyyy") ?? "";
                        worksheet.Cells[fila, 12] = s.EndDate?.ToString("dd/MM/yyyy") ?? "";
                        worksheet.Cells[fila, 13] = s.IsActive ? "SI" : "NO";
                        fila++;
                    }

                    worksheet.Columns.AutoFit();
                    workbook.SaveAs(dlg.FileName);

                    this.Cursor = Cursors.Default;
                    MessageBox.Show("LISTADO EXPORTADO CORRECTAMENTE.", "ÉXITO",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("ERROR AL EXPORTAR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                    if (workbook != null) { workbook.Close(false); Marshal.ReleaseComObject(workbook); }
                    if (excelApp != null) { excelApp.Quit(); Marshal.ReleaseComObject(excelApp); }
                }
            }
        }

        #endregion
    }
}