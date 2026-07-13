using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_LocationStaffAssignments : Form
    {
        #region PropiedadesIniciales

        public Mdl_Security_UserInfo UserData { get; set; }

        private const int PAGE_SIZE = 100;

        // Sedes
        private List<Mdl_Locations> _locationsList;
        private Mdl_Locations _sedeSeleccionada = null;
        private int paginaSedes = 1;
        private int totalSedes = 0;
        private string _ultimoTextoSedes = "";
        private string _ultimoEstadoSedes = "SOLO ACTIVOS";
        private int _displayIndexCounter = 0;

        // Empleados/Asignaciones de la sede
        private List<Mdl_LocationStaffAssignments> _employeesList;
        private Mdl_LocationStaffAssignments _asignacionSeleccionada = null;
        private int paginaEmpleados = 1;
        private int totalEmpleados = 0;
        private string _ultimoTextoEmpleados = "";
        private string _ultimoEstadoEmpleados = "SOLO ACTIVOS";

        // ToolStrip de paginación — Sedes
        private ToolStrip toolStripSedes;
        private ToolStripButton btnAnteriorSedes;
        private ToolStripButton btnSiguienteSedes;

        // ToolStrip de paginación — Empleados
        private ToolStrip toolStripEmpleados;
        private ToolStripButton btnAnteriorEmpleados;
        private ToolStripButton btnSiguienteEmpleados;

        // Roles
        private List<Mdl_LocationStaffRoles> _rolesList;

        // Usuario seleccionado desde Frm_LocationStaffAssignments_SearchUsers
        private int _userIdSeleccionado = 0;

        public Frm_LocationStaffAssignments()
        {
            InitializeComponent();
            this.MinimumSize = new Size(1200, 700);
        }

        private async void Frm_LocationStaffAssignments_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarComponentesDeshabilitados();
                ConfigurarPlaceHolders();
                ConfigurarCombosEstado();
                ConfigurarTablaSedes();
                ConfigurarTablaEmpleados();
                CrearToolStripPaginacionSedes();
                CrearToolStripPaginacionEmpleados();

                CargarRoles();
                CargarSedes();

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

        #endregion PropiedadesIniciales

        #region ConfigurarComponentes

        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_LocationName.Enabled = false;
            Txt_EmployeeName.Enabled = false;
        }

        private void ConfigurarPlaceHolders()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR SEDE POR CÓDIGO O NOMBRE...");
            ConfigurarPlaceHolder(Txt_SearchEmployee, "BUSCAR EMPLEADO POR CÓDIGO O NOMBRE...");
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

        private bool TienePlaceholder(TextBox textBox, string placeholder)
        {
            return textBox.Text == placeholder && textBox.ForeColor == Color.Gray;
        }

        private void ConfigurarCombosEstado()
        {
            CBX_LocationStatus.Items.Clear();
            CBX_LocationStatus.Items.Add("TODOS");
            CBX_LocationStatus.Items.Add("SOLO ACTIVOS");
            CBX_LocationStatus.Items.Add("SOLO INACTIVOS");
            CBX_LocationStatus.SelectedIndex = 1;

            CBX_EmployeeStatus.Items.Clear();
            CBX_EmployeeStatus.Items.Add("TODOS");
            CBX_EmployeeStatus.Items.Add("SOLO ACTIVOS");
            CBX_EmployeeStatus.Items.Add("SOLO INACTIVOS");
            CBX_EmployeeStatus.SelectedIndex = 1;
        }

        #endregion ConfigurarComponentes

        #region Roles

        private void CargarRoles()
        {
            _rolesList = Ctrl_LocationStaffAssignments.GetActiveRoles();

            CBX_RoleType.DropDownStyle = ComboBoxStyle.DropDownList;
            CBX_RoleType.DataSource = null;
            CBX_RoleType.DisplayMember = "RoleName";
            CBX_RoleType.ValueMember = "RoleTypeId";
            CBX_RoleType.DataSource = _rolesList;

            CBX_RoleFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            CBX_RoleFilter.Items.Clear();
            CBX_RoleFilter.Items.Add("TODOS");
            foreach (var rol in _rolesList)
                CBX_RoleFilter.Items.Add(rol.RoleName);
            CBX_RoleFilter.SelectedIndex = 0;
        }

        private byte? ObtenerRoleTypeIdSeleccionado()
        {
            if (CBX_RoleType.SelectedItem is Mdl_LocationStaffRoles rol)
                return rol.RoleTypeId;
            return null;
        }

        #endregion Roles

        #region ConfiguracionesTabla_Sedes

        private void ConfigurarTablaSedes()
        {
            Grid_Locations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Grid_Locations.MultiSelect = false;
            Grid_Locations.ReadOnly = true;
            Grid_Locations.AllowUserToAddRows = false;
            Grid_Locations.AllowUserToResizeRows = false;

            Grid_Locations.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Grid_Locations.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Grid_Locations.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Grid_Locations.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Grid_Locations.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Grid_Locations.DefaultCellStyle.SelectionForeColor = Color.White;
            Grid_Locations.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            Grid_Locations.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            Grid_Locations.RowTemplate.Height = 35;
            Grid_Locations.ColumnHeadersHeight = 40;

            Grid_Locations.SelectionChanged -= Grid_Locations_SelectionChanged;
            Grid_Locations.SelectionChanged += Grid_Locations_SelectionChanged;
        }

        private void AsignarDataSourceSedes()
        {
            Grid_Locations.DataSource = null;
            Grid_Locations.DataSource = _locationsList;

            if (Grid_Locations.Columns.Count == 0) return;

            foreach (DataGridViewColumn col in Grid_Locations.Columns)
                col.Visible = false;

            _displayIndexCounter = 0;
            MostrarColumna(Grid_Locations, "LocationCode", "CÓDIGO", 15);
            MostrarColumna(Grid_Locations, "LocationName", "NOMBRE DE SEDE", 40);
            MostrarColumna(Grid_Locations, "DepartmentName", "DEPARTAMENTO", 25);
            MostrarColumna(Grid_Locations, "IsActive", "ESTADO", 20);

            ConfigurarTablaSedes();

            if (Grid_Locations.Rows.Count > 0)
                Grid_Locations.Rows[0].Selected = true;
        }

        private void MostrarColumna(DataGridView grid, string col, string header, int weight)
        {
            if (!grid.Columns.Contains(col)) return;
            grid.Columns[col].Visible = true;
            grid.Columns[col].HeaderText = header;
            grid.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns[col].FillWeight = weight;
            grid.Columns[col].DisplayIndex = _displayIndexCounter++;
        }

        #endregion ConfiguracionesTabla_Sedes

        #region ConfiguracionesTabla_Empleados

        private void ConfigurarTablaEmpleados()
        {
            Grid_Employees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Grid_Employees.MultiSelect = false;
            Grid_Employees.ReadOnly = true;
            Grid_Employees.AllowUserToAddRows = false;
            Grid_Employees.AllowUserToResizeRows = false;

            Grid_Employees.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Grid_Employees.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Grid_Employees.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Grid_Employees.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Grid_Employees.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Grid_Employees.DefaultCellStyle.SelectionForeColor = Color.White;
            Grid_Employees.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            Grid_Employees.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            Grid_Employees.RowTemplate.Height = 35;
            Grid_Employees.ColumnHeadersHeight = 40;

            Grid_Employees.SelectionChanged -= Grid_Employees_SelectionChanged;
            Grid_Employees.SelectionChanged += Grid_Employees_SelectionChanged;
        }

        private void AsignarDataSourceEmpleados()
        {
            Grid_Employees.DataSource = null;
            Grid_Employees.DataSource = _employeesList;

            if (Grid_Employees.Columns.Count == 0) return;

            foreach (DataGridViewColumn col in Grid_Employees.Columns)
                col.Visible = false;

            _displayIndexCounter = 0;

            MostrarColumna(Grid_Employees, "RoleName", "CARGO", 18);
            MostrarColumna(Grid_Employees, "EmployeeCode", "CÓDIGO", 12);
            MostrarColumna(Grid_Employees, "FullName", "NOMBRE", 30);
            MostrarColumna(Grid_Employees, "InstitutionalEmail", "CORREO INSTITUCIONAL", 25);
            MostrarColumna(Grid_Employees, "Phone", "TELÉFONO", 13);
            MostrarColumna(Grid_Employees, "IsActive", "ESTADO", 20);

            ConfigurarTablaEmpleados();
        }

        #endregion ConfiguracionesTabla_Empleados

        #region CargarSedes

        private void CargarSedes()
        {
            bool? isActive = MapearEstado(_ultimoEstadoSedes);

            _locationsList = Ctrl_Locations.BuscarUbicaciones(
                _ultimoTextoSedes, "TODOS", isActive, paginaSedes, PAGE_SIZE);

            totalSedes = Ctrl_Locations.ContarTotalUbicaciones(_ultimoTextoSedes, "TODOS", isActive);

            AsignarDataSourceSedes();
            ActualizarInfoPaginacionSedes();
        }

        private bool? MapearEstado(string estado)
        {
            if (estado == "SOLO ACTIVOS") return true;
            if (estado == "SOLO INACTIVOS") return false;
            return null;
        }

        private void Grid_Locations_SelectionChanged(object sender, EventArgs e)
        {
            if (Grid_Locations.SelectedRows.Count == 0)
            {
                _sedeSeleccionada = null;
                Txt_LocationName.Text = "";
                _employeesList = new List<Mdl_LocationStaffAssignments>();
                AsignarDataSourceEmpleados();
                Lbl_EmployeesHeader.Text = "SELECCIONE UNA SEDE";
                return;
            }

            int locationId = Convert.ToInt32(Grid_Locations.SelectedRows[0].Cells["LocationId"].Value);
            _sedeSeleccionada = _locationsList.FirstOrDefault(l => l.LocationId == locationId);
            if (_sedeSeleccionada == null) return;

            Txt_LocationName.Text = _sedeSeleccionada.LocationName;
            Lbl_EmployeesHeader.Text = $"PERSONAL ASIGNADO — {_sedeSeleccionada.LocationName}";

            LimpiarPanelIzquierdo(limpiarSede: false);

            paginaEmpleados = 1;
            _ultimoTextoEmpleados = "";
            CBX_EmployeeStatus.SelectedIndex = 1;
            CBX_RoleFilter.SelectedIndex = 0;
            SetTextBoxFromValue(Txt_SearchEmployee, "", "BUSCAR EMPLEADO POR CÓDIGO O NOMBRE...");

            CargarEmpleadosDeLaSede();
        }

        #endregion CargarSedes

        #region CargarEmpleados

        private void CargarEmpleadosDeLaSede()
        {
            if (_sedeSeleccionada == null) return;

            bool? isActive = MapearEstado(_ultimoEstadoEmpleados);

            _employeesList = Ctrl_LocationStaffAssignments.SearchByLocation(
                _sedeSeleccionada.LocationId, _ultimoTextoEmpleados, isActive, paginaEmpleados, PAGE_SIZE);

            totalEmpleados = Ctrl_LocationStaffAssignments.CountByLocation(
                _sedeSeleccionada.LocationId, _ultimoTextoEmpleados, isActive);

            AsignarDataSourceEmpleados();
            ActualizarInfoPaginacionEmpleados();
        }

        private void Grid_Employees_SelectionChanged(object sender, EventArgs e)
        {
            if (Grid_Employees.SelectedRows.Count == 0)
            {
                _asignacionSeleccionada = null;
                return;
            }

            int assignmentId = Convert.ToInt32(Grid_Employees.SelectedRows[0].Cells["AssignmentId"].Value);
            _asignacionSeleccionada = _employeesList.FirstOrDefault(a => a.AssignmentId == assignmentId);
            if (_asignacionSeleccionada == null) return;

            _userIdSeleccionado = _asignacionSeleccionada.UserId;
            Txt_EmployeeName.Text = _asignacionSeleccionada.FullName;

            int idx = _rolesList.FindIndex(r => r.RoleTypeId == _asignacionSeleccionada.RoleTypeId);
            if (idx >= 0) CBX_RoleType.SelectedIndex = idx;
        }

        #endregion CargarEmpleados

        #region Busqueda_Sedes

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            if (!Btn_Search.Enabled) return;

            _ultimoTextoSedes = TienePlaceholder(Txt_ValorBuscado, "BUSCAR SEDE POR CÓDIGO O NOMBRE...")
                ? "" : Txt_ValorBuscado.Text.Trim();
            _ultimoEstadoSedes = CBX_LocationStatus.SelectedItem?.ToString() ?? "TODOS";

            paginaSedes = 1;
            CargarSedes();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_Search_Click(sender, e); }
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            SetTextBoxFromValue(Txt_ValorBuscado, "", "BUSCAR SEDE POR CÓDIGO O NOMBRE...");
            CBX_LocationStatus.SelectedIndex = 1;
            _ultimoTextoSedes = "";
            _ultimoEstadoSedes = "SOLO ACTIVOS";
            paginaSedes = 1;
            CargarSedes();
        }

        #endregion Busqueda_Sedes

        #region Busqueda_Empleados

        private void Btn_SearchGridEmployees_Click(object sender, EventArgs e)
        {
            if (!Btn_SearchGridEmployees.Enabled) return;
            if (_sedeSeleccionada == null) return;

            _ultimoTextoEmpleados = TienePlaceholder(Txt_SearchEmployee, "BUSCAR EMPLEADO POR CÓDIGO O NOMBRE...")
                ? "" : Txt_SearchEmployee.Text.Trim();
            _ultimoEstadoEmpleados = CBX_EmployeeStatus.SelectedItem?.ToString() ?? "TODOS";

            paginaEmpleados = 1;
            CargarEmpleadosDeLaSede();
        }

        private void Txt_SearchEmployee_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_SearchGridEmployees_Click(sender, e); }
        }

        private void Btn_CleanSearchGridEmployees_Click(object sender, EventArgs e)
        {
            SetTextBoxFromValue(Txt_SearchEmployee, "", "BUSCAR EMPLEADO POR CÓDIGO O NOMBRE...");
            CBX_EmployeeStatus.SelectedIndex = 1;
            CBX_RoleFilter.SelectedIndex = 0;
            _ultimoTextoEmpleados = "";
            _ultimoEstadoEmpleados = "SOLO ACTIVOS";
            paginaEmpleados = 1;
            CargarEmpleadosDeLaSede();
        }

        #endregion Busqueda_Empleados

        #region Paginacion_Sedes

        private void CrearToolStripPaginacionSedes()
        {
            toolStripSedes = new ToolStrip();
            toolStripSedes.Dock = DockStyle.None;
            toolStripSedes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            toolStripSedes.GripStyle = ToolStripGripStyle.Hidden;
            toolStripSedes.BackColor = Color.FromArgb(248, 249, 250);
            toolStripSedes.Height = 40;
            toolStripSedes.AutoSize = true;
            toolStripSedes.Location = new Point(PanelToolStrip.Width - 240, 4);

            btnAnteriorSedes = new ToolStripButton();
            btnAnteriorSedes.Text = "❮ Anterior";
            btnAnteriorSedes.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnteriorSedes.ForeColor = Color.White;
            btnAnteriorSedes.BackColor = Color.FromArgb(51, 140, 255);
            btnAnteriorSedes.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAnteriorSedes.Margin = new Padding(2);
            btnAnteriorSedes.Padding = new Padding(8, 4, 8, 4);
            btnAnteriorSedes.Click += (s, e) => CambiarPaginaSedes(paginaSedes - 1);

            toolStripSedes.Items.Add(btnAnteriorSedes);

            ActualizarBotonesNumeradosSedes();

            btnSiguienteSedes = new ToolStripButton();
            btnSiguienteSedes.Text = "Siguiente ❯";
            btnSiguienteSedes.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSiguienteSedes.ForeColor = Color.White;
            btnSiguienteSedes.BackColor = Color.FromArgb(238, 143, 109);
            btnSiguienteSedes.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSiguienteSedes.Margin = new Padding(2);
            btnSiguienteSedes.Padding = new Padding(8, 4, 8, 4);
            btnSiguienteSedes.Click += (s, e) => CambiarPaginaSedes(paginaSedes + 1);

            toolStripSedes.Items.Add(btnSiguienteSedes);

            PanelToolStrip.Controls.Add(toolStripSedes);
            toolStripSedes.BringToFront();
        }

        private void ActualizarBotonesNumeradosSedes()
        {
            var itemsToRemove = toolStripSedes.Items.Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton").ToList();

            foreach (var item in itemsToRemove)
                toolStripSedes.Items.Remove(item);

            int totalPaginasSedes = (int)Math.Ceiling(totalSedes / (double)PAGE_SIZE);
            if (totalPaginasSedes <= 1) return;

            int inicioRango = Math.Max(1, paginaSedes - 1);
            int finRango = Math.Min(totalPaginasSedes, paginaSedes + 1);

            int posicionInsertar = toolStripSedes.Items.IndexOf(btnSiguienteSedes);

            for (int i = inicioRango; i <= finRango; i++)
            {
                ToolStripButton btnPagina = new ToolStripButton();
                btnPagina.Text = i.ToString();
                btnPagina.Tag = "PageButton";
                btnPagina.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnPagina.Margin = new Padding(1);
                btnPagina.Padding = new Padding(6, 4, 6, 4);

                if (i == paginaSedes)
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
                btnPagina.Click += (s, e) => CambiarPaginaSedes(numeroPagina);

                toolStripSedes.Items.Insert(posicionInsertar++, btnPagina);
            }
        }

        private void CambiarPaginaSedes(int nuevaPagina)
        {
            int totalPaginasSedes = (int)Math.Ceiling(totalSedes / (double)PAGE_SIZE);

            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginasSedes)
            {
                paginaSedes = nuevaPagina;
                CargarSedes();
            }
        }

        private void ActualizarInfoPaginacionSedes()
        {
            int totalPaginasSedes = (int)Math.Ceiling(totalSedes / (double)PAGE_SIZE);

            if (btnAnteriorSedes != null)
                btnAnteriorSedes.Enabled = paginaSedes > 1;

            if (btnSiguienteSedes != null)
                btnSiguienteSedes.Enabled = paginaSedes < totalPaginasSedes;

            ActualizarBotonesNumeradosSedes();

            int desde = totalSedes == 0 ? 0 : ((paginaSedes - 1) * PAGE_SIZE) + 1;
            int hasta = Math.Min(paginaSedes * PAGE_SIZE, totalSedes);
            Lbl_LocationsPaging.Text = $"MOSTRANDO {desde}-{hasta} DE {totalSedes} SEDES";
        }

        #endregion Paginacion_Sedes

        #region Paginacion_Empleados

        private void ActualizarBotonesNumeradosEmpleados()
        {
            var itemsToRemove = toolStripEmpleados.Items.Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton").ToList();

            foreach (var item in itemsToRemove)
                toolStripEmpleados.Items.Remove(item);

            int totalPaginasEmpleados = (int)Math.Ceiling(totalEmpleados / (double)PAGE_SIZE);
            if (totalPaginasEmpleados <= 1) return;

            int inicioRango = Math.Max(1, paginaEmpleados - 1);
            int finRango = Math.Min(totalPaginasEmpleados, paginaEmpleados + 1);

            int posicionInsertar = toolStripEmpleados.Items.IndexOf(btnSiguienteEmpleados);

            for (int i = inicioRango; i <= finRango; i++)
            {
                ToolStripButton btnPagina = new ToolStripButton();
                btnPagina.Text = i.ToString();
                btnPagina.Tag = "PageButton";
                btnPagina.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnPagina.Margin = new Padding(1);
                btnPagina.Padding = new Padding(6, 4, 6, 4);

                if (i == paginaEmpleados)
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
                btnPagina.Click += (s, e) => CambiarPaginaEmpleados(numeroPagina);

                toolStripEmpleados.Items.Insert(posicionInsertar++, btnPagina);
            }
        }

        private void CambiarPaginaEmpleados(int nuevaPagina)
        {
            int totalPaginasEmpleados = (int)Math.Ceiling(totalEmpleados / (double)PAGE_SIZE);

            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginasEmpleados)
            {
                paginaEmpleados = nuevaPagina;
                CargarEmpleadosDeLaSede();
            }
        }

        private void CrearToolStripPaginacionEmpleados()
        {
            toolStripEmpleados = new ToolStrip();
            toolStripEmpleados.Dock = DockStyle.None;
            toolStripEmpleados.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            toolStripEmpleados.GripStyle = ToolStripGripStyle.Hidden;
            toolStripEmpleados.BackColor = Color.FromArgb(248, 249, 250);
            toolStripEmpleados.Height = 40;
            toolStripEmpleados.AutoSize = true;
            toolStripEmpleados.Location = new Point(PanelToolStrip2.Width - 240, 4);

            btnAnteriorEmpleados = new ToolStripButton();
            btnAnteriorEmpleados.Text = "❮ Anterior";
            btnAnteriorEmpleados.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnteriorEmpleados.ForeColor = Color.White;
            btnAnteriorEmpleados.BackColor = Color.FromArgb(51, 140, 255);
            btnAnteriorEmpleados.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAnteriorEmpleados.Margin = new Padding(2);
            btnAnteriorEmpleados.Padding = new Padding(8, 4, 8, 4);
            btnAnteriorEmpleados.Click += (s, e) => CambiarPaginaEmpleados(paginaEmpleados - 1);

            toolStripEmpleados.Items.Add(btnAnteriorEmpleados);

            ActualizarBotonesNumeradosEmpleados();

            btnSiguienteEmpleados = new ToolStripButton();
            btnSiguienteEmpleados.Text = "Siguiente ❯";
            btnSiguienteEmpleados.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSiguienteEmpleados.ForeColor = Color.White;
            btnSiguienteEmpleados.BackColor = Color.FromArgb(238, 143, 109);
            btnSiguienteEmpleados.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSiguienteEmpleados.Margin = new Padding(2);
            btnSiguienteEmpleados.Padding = new Padding(8, 4, 8, 4);
            btnSiguienteEmpleados.Click += (s, e) => CambiarPaginaEmpleados(paginaEmpleados + 1);

            toolStripEmpleados.Items.Add(btnSiguienteEmpleados);

            PanelToolStrip2.Controls.Add(toolStripEmpleados);
            toolStripEmpleados.BringToFront();
        }



        private void ActualizarInfoPaginacionEmpleados()
        {
            int totalPaginasEmpleados = (int)Math.Ceiling(totalEmpleados / (double)PAGE_SIZE);

            if (btnAnteriorEmpleados != null)
                btnAnteriorEmpleados.Enabled = paginaEmpleados > 1;

            if (btnSiguienteEmpleados != null)
                btnSiguienteEmpleados.Enabled = paginaEmpleados < totalPaginasEmpleados;

            ActualizarBotonesNumeradosEmpleados();

            int desde = totalEmpleados == 0 ? 0 : ((paginaEmpleados - 1) * PAGE_SIZE) + 1;
            int hasta = Math.Min(paginaEmpleados * PAGE_SIZE, totalEmpleados);
            Lbl_EmployeesPaging.Text = $"MOSTRANDO {desde}-{hasta} DE {totalEmpleados} REGISTROS";
        }

        #endregion Paginacion_Empleados

        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!Btn_Save.Enabled) return;

            if (_sedeSeleccionada == null)
            {
                MessageBox.Show("Debe seleccionar una sede.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_userIdSeleccionado == 0)
            {
                MessageBox.Show("Debe seleccionar un empleado mediante el botón de búsqueda.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte? roleTypeId = ObtenerRoleTypeIdSeleccionado();
            if (roleTypeId == null)
            {
                MessageBox.Show("Debe seleccionar un cargo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var model = new Mdl_LocationStaffAssignments
            {
                LocationId = _sedeSeleccionada.LocationId,
                UserId = _userIdSeleccionado,
                RoleTypeId = roleTypeId.Value,
                CreatedBy = UserData.UserId
            };

            int resultado = Ctrl_LocationStaffAssignments.Create(model);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Asignación registrada correctamente.", "SECRON",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarPanelIzquierdo(limpiarSede: false);
                    CargarEmpleadosDeLaSede();
                    break;
                case -1:
                    MessageBox.Show("El empleado ya se encuentra asignado activamente a esta sede.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al registrar la asignación.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (!Btn_Update.Enabled) return;

            if (_asignacionSeleccionada == null)
            {
                MessageBox.Show("Debe seleccionar un registro del listado.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte? roleTypeId = ObtenerRoleTypeIdSeleccionado();
            if (roleTypeId == null)
            {
                MessageBox.Show("Debe seleccionar un cargo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Confirma actualizar el cargo de este colaborador?", "Confirmación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var model = new Mdl_LocationStaffAssignments
            {
                AssignmentId = _asignacionSeleccionada.AssignmentId,
                LocationId = _asignacionSeleccionada.LocationId,
                UserId = _asignacionSeleccionada.UserId,
                RoleTypeId = roleTypeId.Value,
                ModifiedBy = UserData.UserId
            };

            int resultado = Ctrl_LocationStaffAssignments.Update(model, isInactivation: false);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Registro actualizado correctamente.", "SECRON",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarPanelIzquierdo(limpiarSede: false);
                    CargarEmpleadosDeLaSede();
                    break;
                case -1:
                    MessageBox.Show("Ya existe una asignación activa para este empleado en la sede.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al actualizar el registro.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            if (!Btn_Inactive.Enabled) return;

            if (_asignacionSeleccionada == null)
            {
                MessageBox.Show("Debe seleccionar un registro del listado.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Confirma inactivar esta asignación?", "Confirmación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var model = new Mdl_LocationStaffAssignments
            {
                AssignmentId = _asignacionSeleccionada.AssignmentId,
                LocationId = _asignacionSeleccionada.LocationId,
                UserId = _asignacionSeleccionada.UserId,
                RoleTypeId = _asignacionSeleccionada.RoleTypeId,
                ModifiedBy = UserData.UserId
            };

            int resultado = Ctrl_LocationStaffAssignments.Update(model, isInactivation: true);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Registro inactivado correctamente.", "SECRON",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarPanelIzquierdo(limpiarSede: false);
                    CargarEmpleadosDeLaSede();
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al inactivar el registro.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarPanelIzquierdo(limpiarSede: false);
        }

        private void LimpiarPanelIzquierdo(bool limpiarSede)
        {
            _asignacionSeleccionada = null;
            _userIdSeleccionado = 0;
            Txt_EmployeeName.Clear();
            if (CBX_RoleType.Items.Count > 0) CBX_RoleType.SelectedIndex = 0;

            if (limpiarSede)
            {
                _sedeSeleccionada = null;
                Txt_LocationName.Clear();
            }
        }

        private void SetTextBoxFromValue(TextBox textBox, string value, string placeholder)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = Color.Gray;
            }
            else
            {
                textBox.Text = value;
                textBox.ForeColor = Color.Black;
            }
        }

        #endregion CRUD

        #region BusquedaUsuarios

        private void Btn_SearchEmployee_Click(object sender, EventArgs e)
        {
            if (!Btn_SearchEmployee.Enabled) return;

            if (_sedeSeleccionada == null)
            {
                MessageBox.Show("Debe seleccionar una sede primero.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var frm = new Frm_LocationStaffAssignments_SearchUsers(this, _sedeSeleccionada.LocationId))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar usuario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SetUsuarioSeleccionado(int userId, string fullName)
        {
            _userIdSeleccionado = userId;
            Txt_EmployeeName.Text = fullName;
        }

        #endregion BusquedaUsuarios

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
            AplicarEstadoBotonPorPermiso(Btn_Save, "LOCATIONS_STAFF_CREATE");
            AplicarEstadoBotonPorPermiso(Btn_Update, "LOCATIONS_STAFF_UPDATE");
            AplicarEstadoBotonPorPermiso(Btn_Inactive, "LOCATIONS_STAFF_INACTIVE");
            AplicarEstadoBotonPorPermiso(Btn_Search, "LOCATIONS_STAFF_READ");
            AplicarEstadoBotonPorPermiso(Btn_SearchGridEmployees, "LOCATIONS_STAFF_READ");
            AplicarEstadoBotonPorPermiso(Btn_SearchEmployee, "LOCATIONS_STAFF_CREATE");
        }

        #endregion SistemaDePermisos
    }
}