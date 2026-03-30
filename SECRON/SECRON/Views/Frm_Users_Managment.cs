using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCrypt.Net;
// Agregar Referencias necesarias
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_Users_Managment : Form
    {
        #region PropiedadesIniciales
        // Variables de entorno
        public Mdl_Security_UserInfo UserData { get; set; }

        // Usuario seleccionado para editar
        private Mdl_Users _usuarioSeleccionado = null;

        // Colaborador seleccionado de Tabla2
        private Mdl_Employees _colaboradorSeleccionado = null;

        // Listas para almacenar datos
        private List<Mdl_Users> usuariosList;
        private List<Mdl_Employees> colaboradoresList;

        // Variables para paginación - TABLA 1 (USUARIOS)
        private int paginaActualUsuarios = 1;
        private int registrosPorPaginaUsuarios = 100;
        private int totalRegistrosUsuarios = 0;
        private int totalPaginasUsuarios = 0;

        // Variables para paginación - TABLA 2 (COLABORADORES)
        private int paginaActualColaboradores = 1;
        private int registrosPorPaginaColaboradores = 100;
        private int totalRegistrosColaboradores = 0;
        private int totalPaginasColaboradores = 0;

        // ToolStrip para paginación - USUARIOS
        private ToolStrip toolStripPaginacionUsuarios;
        private ToolStripButton btnAnteriorUsuarios;
        private ToolStripButton btnSiguienteUsuarios;

        // ToolStrip para paginación - COLABORADORES
        private ToolStrip toolStripPaginacionColaboradores;
        private ToolStripButton btnAnteriorColaboradores;
        private ToolStripButton btnSiguienteColaboradores;

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse);

        // Evento Load del formulario
        private void Frm_Users_Managment_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Crear ToolStrip de paginación para ambas tablas
                CrearToolStripPaginacionUsuarios();
                CrearToolStripPaginacionColaboradores();

                // Cargar todos los usuarios y colaboradores
                CargarUsuarios();
                CargarColaboradores();

                // Actualizar información de paginación
                ActualizarInfoPaginacionUsuarios();
                ActualizarInfoPaginacionColaboradores();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                              "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Configurar PlaceHolders en los TextBox
            ConfigurarPlaceHoldersTextbox();

            // Configurar tamaño máximo de los TextBox
            ConfigurarMaxLengthTextBox();

            // Cargar filtros
            CargarFiltros();

            // Configurar ComboBoxes
            ConfigurarComboBoxes();

            // Configurar TabIndex y Focus inicial
            ConfigurarTabIndexYFocus();

            //Configurar ScrollBar del Panel_Derecho
            InicializarScrollPanelDerecho();
            ConfigurarEventosScrollPanelDerecho();
            AplicarEstilosBotones();
        }

        // Método separado para Resize
        private void FormularioResize(object sender, EventArgs e)
        {
            if (Tabla1 != null && Tabla1.DataSource != null)
            {
                Tabla1.Refresh();
            }
            if (Tabla2 != null && Tabla2.DataSource != null)
            {
                Tabla2.Refresh();
            }
        }

        // Constructor del formulario
        public Frm_Users_Managment()
        {
            InitializeComponent();
            this.Resize += FormularioResize;
            this.Resize += (s, e) => {
                if (toolStripPaginacionUsuarios != null)
                {
                    toolStripPaginacionUsuarios.Location = new Point(this.Width - 300, 225);
                }
                if (toolStripPaginacionColaboradores != null)
                {
                    toolStripPaginacionColaboradores.Location = new Point(this.Width - 300, 550);
                }
            };
        }
        #endregion PropiedadesIniciales
        #region ConfigurarTextBox
        // Método para asignar tamaño máximo de los cuadros de Texto
        private void ConfigurarMaxLengthTextBox()
        {
            Txt_ValorBuscado1.MaxLength = 100;
            Txt_Usuario.MaxLength = 50;
            Txt_Password.MaxLength = 100;
            Txt_Colaborador.MaxLength = 100; // Solo lectura, se llena automáticamente
            Txt_CorreoInstitucional.MaxLength = 100;
        }
        // Metodo para configurar placeholders en los TextBox
        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado1, "BUSCAR USUARIO POR NOMBRE, USERNAME, EMAIL...");
            ConfigurarPlaceHolder(Txt_ValorBuscado2, "BUSCAR POR NOMBRE, DEPARTAMENTO, DPI...");
            ConfigurarPlaceHolder(Txt_Usuario, "NOMBRE DE USUARIO");
            ConfigurarPlaceHolder(Txt_Password, "CONTRASEÑA");
            ConfigurarPlaceHolder(Txt_Colaborador, "SELECCIONE UN COLABORADOR DE LA TABLA");
            ConfigurarPlaceHolder(Txt_CorreoInstitucional, "CORREO INSTITUCIONAL");
        }

        // Método para configurar placeholder en cualquier TextBox estándar de WinForms
        private void ConfigurarPlaceHolder(TextBox textBox, string placeholder)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
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
        #region ConfigurarComboBox
        // Método para configurar ComboBoxes
        private void ConfigurarComboBoxes()
        {
            // Configurar propiedades de los ComboBox para que no permitan escritura
            ComboBox_Rol.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_UserStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Bloqueado.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Bloqueado.Enabled = false; // Deshabilitado por defecto

            FiltroU1.DropDownStyle = ComboBoxStyle.DropDownList;
            FiltroU2.DropDownStyle = ComboBoxStyle.DropDownList;
            FiltroU3.DropDownStyle = ComboBoxStyle.DropDownList;
            FiltroC1.DropDownStyle = ComboBoxStyle.DropDownList;
            FiltroC2.DropDownStyle = ComboBoxStyle.DropDownList;
            FiltroC3.DropDownStyle = ComboBoxStyle.DropDownList;

            // Cargar datos
            CargarRoles();
            CargarEstadosUsuario();
            CargarOpcionesBloqueado();
        }

        // Cargar Roles desde Ctrl_Roles
        private void CargarRoles()
        {
            try
            {
                var roles = Ctrl_Roles.ObtenerTodosLosRoles();

                ComboBox_Rol.DataSource = new BindingSource(roles, null);
                ComboBox_Rol.DisplayMember = "Value"; // El nombre del rol
                ComboBox_Rol.ValueMember = "Key";     // El ID del rol

                if (ComboBox_Rol.Items.Count > 0)
                    ComboBox_Rol.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cargar Estados de Usuario desde Ctrl_UserStatus
        private void CargarEstadosUsuario()
        {
            try
            {
                var estados = Ctrl_UserStatus.ObtenerTodosLosEstados();

                ComboBox_UserStatus.DataSource = new BindingSource(estados, null);
                ComboBox_UserStatus.DisplayMember = "Value"; // El nombre del estado
                ComboBox_UserStatus.ValueMember = "Key";     // El ID del estado

                if (ComboBox_UserStatus.Items.Count > 0)
                    ComboBox_UserStatus.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar estados de usuario: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Cargar opciones para el ComboBox de Bloqueado
        private void CargarOpcionesBloqueado()
        {
            try
            {
                var opciones = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(0, "NO"),
                    new KeyValuePair<int, string>(1, "SÍ")
                };

                ComboBox_Bloqueado.DataSource = new BindingSource(opciones, null);
                ComboBox_Bloqueado.DisplayMember = "Value";
                ComboBox_Bloqueado.ValueMember = "Key";
                ComboBox_Bloqueado.SelectedIndex = 0; // Por defecto "NO"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar opciones de bloqueado: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion ConfigurarComboBox
        #region Filtros
        // Cargar opciones en los ComboBox de filtros
        private void CargarFiltros()
        {
            // FILTROS DE USUARIOS (Tabla1) 

            // FiltroU1 - Tipo de búsqueda principal
            FiltroU1.Items.AddRange(new object[]
            {
                "TODOS",
                "POR NOMBRE",
                "POR USERNAME",
                "POR EMAIL"
            });
            FiltroU1.SelectedIndex = 0;

            // FiltroU2 - Rol del usuario
            var roles = Ctrl_Roles.ObtenerTodosLosRoles();
            var listaRoles = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, "TODOS")
            };
            listaRoles.AddRange(roles);

            FiltroU2.DataSource = new BindingSource(listaRoles, null);
            FiltroU2.DisplayMember = "Value";
            FiltroU2.ValueMember = "Key";
            FiltroU2.SelectedIndex = 0;

            // FiltroU3 - Estado bloqueado
            FiltroU3.Items.AddRange(new object[]
            {
                "TODOS",
                "BLOQUEADOS",
                "NO BLOQUEADOS"
            });
            FiltroU3.SelectedIndex = 0;

            // FILTROS DE COLABORADORES (Tabla2)

            // FiltroC1 - Tipo de búsqueda
            FiltroC1.Items.AddRange(new object[]
            {
                "TODOS",
                "POR NOMBRE",
                "POR DEPARTAMENTO",
                "POR PUESTO"
            });
            FiltroC1.SelectedIndex = 0;

            // FiltroC2 - Estado del empleado
            var estadosEmpleado = Ctrl_Employees.ObtenerEstadosEmpleado();
            var listaEstadosEmp = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, "TODOS")
            };
            listaEstadosEmp.AddRange(estadosEmpleado);

            FiltroC2.DataSource = new BindingSource(listaEstadosEmp, null);
            FiltroC2.DisplayMember = "Value";
            FiltroC2.ValueMember = "Key";
            FiltroC2.SelectedIndex = 0;

            // FiltroC3 - Con/Sin usuario asignado
            FiltroC3.Items.AddRange(new object[]
            {
                "TODOS",
                "CON USUARIO",
                "SIN USUARIO"
            });
            FiltroC3.SelectedIndex = 0;
        }
        #endregion Filtros
        #region SearchUsers
        // Evento Click del botón Buscar Usuarios
        private void Btn_SearchUsers_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Obtener valor de búsqueda
                string valorBusqueda = "";
                if (Txt_ValorBuscado1.Text != "BUSCAR USUARIO POR NOMBRE, USERNAME, EMAIL..." &&
                    !string.IsNullOrWhiteSpace(Txt_ValorBuscado1.Text))
                {
                    valorBusqueda = Txt_ValorBuscado1.Text.Trim();
                }

                // Obtener filtro 2 - Rol
                int? roleId = null;
                if (FiltroU2.SelectedValue != null && (int)FiltroU2.SelectedValue > 0)
                {
                    roleId = (int)FiltroU2.SelectedValue;
                }

                // Obtener filtro 3 - Bloqueado
                string filtroBloqueado = FiltroU3.SelectedItem?.ToString() ?? "TODOS";
                bool? isLocked = null;
                if (filtroBloqueado == "BLOQUEADOS")
                    isLocked = true;
                else if (filtroBloqueado == "NO BLOQUEADOS")
                    isLocked = false;

                // Realizar búsqueda
                List<Mdl_Users> resultados = Ctrl_Users.BuscarUsuarios(
                    textoBusqueda: valorBusqueda,
                    roleId: roleId,
                    statusId: null,
                    isLocked: isLocked,
                    pageNumber: paginaActualUsuarios,
                    pageSize: registrosPorPaginaUsuarios
                );

                // Mostrar resultados
                usuariosList = resultados;
                Tabla1.DataSource = usuariosList;
                ConfigurarTabla1();
                AjustarColumnasTabla1();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento Enter del TextBox de búsqueda de usuarios
        private void Txt_ValorBuscado1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_SearchUsers_Click(sender, e);
            }
        }

        // Evento Click del botón Limpiar Búsqueda Usuarios
        private void Btn_ClearUsers_Click(object sender, EventArgs e)
        {
            // Limpiar búsqueda
            Txt_ValorBuscado1.Text = "BUSCAR USUARIO POR NOMBRE, USERNAME, EMAIL...";
            Txt_ValorBuscado1.ForeColor = Color.Gray;

            // Resetear filtros
            FiltroU1.SelectedIndex = 0;
            FiltroU2.SelectedIndex = 0;
            FiltroU3.SelectedIndex = 0;

            // Recargar todos los usuarios
            paginaActualUsuarios = 1;
            RefrescarListadoUsuarios();
            ConfigurarTabla1();
            AjustarColumnasTabla1();
            ActualizarInfoPaginacionUsuarios();
        }
        #endregion SearchUsers
        #region SearchEmployees
        // Evento Click del botón Buscar Colaboradores
        private void Btn_SearchEmployees_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Obtener valor de búsqueda
                string valorBusqueda = "";
                if (Txt_ValorBuscado2.Text != "BUSCAR POR NOMBRE, DEPARTAMENTO, DPI..." &&
                    !string.IsNullOrWhiteSpace(Txt_ValorBuscado2.Text))
                {
                    valorBusqueda = Txt_ValorBuscado2.Text.Trim();
                }

                // Obtener filtro 1 - Tipo de búsqueda
                string tipoFiltro = FiltroC1.SelectedItem?.ToString() ?? "TODOS";

                // Obtener filtro 2 - Estado del empleado
                int? estadoId = null;
                if (FiltroC2.SelectedValue != null && (int)FiltroC2.SelectedValue > 0)
                {
                    estadoId = (int)FiltroC2.SelectedValue;
                }

                // Obtener filtro 3 - Con/Sin Usuario
                string filtroUsuario = FiltroC3.SelectedItem?.ToString() ?? "TODOS";

                // Variables para el método de búsqueda
                int? departmentId = null;
                int? positionId = null;
                string textoBusqueda = "";

                // LÓGICA SEGÚN FILTRO 1
                switch (tipoFiltro)
                {
                    case "TODOS":
                        textoBusqueda = valorBusqueda;
                        break;

                    case "POR NOMBRE":
                        textoBusqueda = valorBusqueda;
                        break;

                    case "POR DEPARTAMENTO":
                        var departamentos = Ctrl_Employees.ObtenerDepartamentos();
                        var dept = departamentos.FirstOrDefault(d =>
                            d.Value.ToUpper().Contains(valorBusqueda.ToUpper()));
                        if (dept.Key > 0)
                            departmentId = dept.Key;
                        break;

                    case "POR PUESTO":
                        var puestos = Ctrl_Employees.ObtenerPuestos();
                        var puesto = puestos.FirstOrDefault(p =>
                            p.Value.ToUpper().Contains(valorBusqueda.ToUpper()));
                        if (puesto.Key > 0)
                            positionId = puesto.Key;
                        break;
                }

                // Realizar búsqueda
                List<Mdl_Employees> resultados = Ctrl_Employees.BuscarEmpleados(
                    textoBusqueda: textoBusqueda,
                    departmentId: departmentId,
                    positionId: positionId,
                    employeeStatusId: estadoId,
                    pageNumber: paginaActualColaboradores,
                    pageSize: registrosPorPaginaColaboradores
                );

                // APLICAR FILTRO 3 - Con/Sin Usuario (en memoria)
                if (filtroUsuario == "CON USUARIO")
                {
                    // Filtrar solo empleados que YA tienen usuario asignado
                    var empleadosConUsuario = Ctrl_Users.ObtenerEmpleadosConUsuario();
                    resultados = resultados.Where(emp =>
                        empleadosConUsuario.Contains(emp.EmployeeId)).ToList();
                }
                else if (filtroUsuario == "SIN USUARIO")
                {
                    // Filtrar solo empleados SIN usuario asignado
                    var empleadosConUsuario = Ctrl_Users.ObtenerEmpleadosConUsuario();
                    resultados = resultados.Where(emp =>
                        !empleadosConUsuario.Contains(emp.EmployeeId)).ToList();
                }

                // Mostrar resultados
                colaboradoresList = resultados;
                Tabla2.DataSource = colaboradoresList;
                ConfigurarTabla2();
                AjustarColumnasTabla2();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento Enter del TextBox de búsqueda de colaboradores
        private void Txt_ValorBuscado2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_SearchEmployees_Click(sender, e);
            }
        }

        // Evento Click del botón Limpiar Búsqueda Colaboradores
        private void Btn_ClearEmployees_Click(object sender, EventArgs e)
        {
            // Limpiar búsqueda
            Txt_ValorBuscado2.Text = "BUSCAR POR NOMBRE, DEPARTAMENTO, DPI...";
            Txt_ValorBuscado2.ForeColor = Color.Gray;

            // Resetear filtros
            FiltroC1.SelectedIndex = 0;
            FiltroC2.SelectedIndex = 0;
            FiltroC3.SelectedIndex = 0;

            // Recargar todos los colaboradores
            paginaActualColaboradores = 1;
            RefrescarListadoColaboradores();
            ConfigurarTabla2();
            AjustarColumnasTabla2();
            ActualizarInfoPaginacionColaboradores();
        }
        #endregion SearchEmployees
        #region ConfiguracionesTabla1_Usuarios
        // Método para cargar usuarios desde el controlador
        private void CargarUsuarios()
        {
            try
            {
                RefrescarListadoUsuarios();
                ConfigurarTabla1();
                AjustarColumnasTabla1();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}");
            }
        }

        // Método para refrescar la tabla de usuarios
        public void RefrescarListadoUsuarios()
        {
            usuariosList = Ctrl_Users.MostrarUsuarios(paginaActualUsuarios, registrosPorPaginaUsuarios);
            Tabla1.DataSource = usuariosList;
        }

        // Método para configurar la tabla de usuarios
        public void ConfigurarTabla1()
        {
            if (Tabla1.Columns.Count > 0)
            {
                // Configurar títulos de columnas
                Tabla1.Columns["Username"].HeaderText = "USERNAME";
                Tabla1.Columns["FullName"].HeaderText = "NOMBRE COMPLETO";
                Tabla1.Columns["InstitutionalEmail"].HeaderText = "EMAIL";
                Tabla1.Columns["CreatedDate"].HeaderText = "FECHA CREACIÓN";
                Tabla1.Columns["LastLoginDate"].HeaderText = "ÚLTIMO LOGIN";
                Tabla1.Columns["IsLocked"].HeaderText = "BLOQUEADO";

                // Ocultar columnas no necesarias
                Tabla1.Columns["UserId"].Visible = false;
                Tabla1.Columns["PasswordHash"].Visible = false;
                Tabla1.Columns["RoleId"].Visible = false;
                Tabla1.Columns["StatusId"].Visible = false;
                Tabla1.Columns["NotificationsEnabled"].Visible = false;
                Tabla1.Columns["LastConnectionDate"].Visible = false;
                Tabla1.Columns["IsTemporaryPassword"].Visible = false;
                Tabla1.Columns["CreatedBy"].Visible = false;
                Tabla1.Columns["ModifiedDate"].Visible = false;
                Tabla1.Columns["ModifiedBy"].Visible = false;
                Tabla1.Columns["EmployeeId"].Visible = false;
                Tabla1.Columns["PasswordExpiryDate"].Visible = false;
                Tabla1.Columns["FailedLoginAttempts"].Visible = false;
            }

            // Configuración de selección y formato
            Tabla1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla1.MultiSelect = false;
            Tabla1.ReadOnly = true;
            Tabla1.AllowUserToResizeRows = false;
            Tabla1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold);

            if (Tabla1.Columns.Contains("CreatedDate"))
                Tabla1.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            if (Tabla1.Columns.Contains("LastLoginDate"))
                Tabla1.Columns["LastLoginDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

            // Agregar evento de selección
            Tabla1.SelectionChanged -= Tabla1_SelectionChanged;
            Tabla1.SelectionChanged += Tabla1_SelectionChanged;
        }

        // Método para cargar datos del usuario seleccionado
        private void CargarDatosUsuarioSeleccionado()
        {
            try
            {
                if (Tabla1.SelectedRows.Count == 0) return;

                DataGridViewRow fila = Tabla1.SelectedRows[0];
                int userId = Convert.ToInt32(fila.Cells["UserId"].Value);

                _usuarioSeleccionado = Ctrl_Users.ObtenerUsuarioPorId(userId);

                if (_usuarioSeleccionado != null)
                {
                    // Cargar USERNAME
                    Txt_Usuario.Text = _usuarioSeleccionado.Username;
                    Txt_Usuario.ForeColor = Color.Black;

                    // NO cargar contraseña (por seguridad)
                    Txt_Password.Text = "CONTRASEÑA";
                    Txt_Password.ForeColor = Color.Gray;

                    // Cargar ROL
                    ComboBox_Rol.SelectedValue = _usuarioSeleccionado.RoleId;

                    // Cargar ESTADO
                    ComboBox_UserStatus.SelectedValue = _usuarioSeleccionado.StatusId;

                    // Cargar BLOQUEADO
                    ComboBox_Bloqueado.SelectedValue = _usuarioSeleccionado.IsLocked ? 1 : 0;

                    // Cargar CORREO INSTITUCIONAL
                    if (!string.IsNullOrWhiteSpace(_usuarioSeleccionado.InstitutionalEmail))
                    {
                        Txt_CorreoInstitucional.Text = _usuarioSeleccionado.InstitutionalEmail;
                        Txt_CorreoInstitucional.ForeColor = Color.Black;
                    }

                    // Cargar CHECKBOX de contraseña temporal
                    CheckBox_PasswordTemp.Checked = _usuarioSeleccionado.IsTemporaryPassword;

                    // Cargar COLABORADOR (si tiene)
                    if (_usuarioSeleccionado.EmployeeId.HasValue)
                    {
                        var empleado = Ctrl_Employees.ObtenerEmpleadoPorId(_usuarioSeleccionado.EmployeeId.Value);
                        if (empleado != null)
                        {
                            _colaboradorSeleccionado = empleado;
                            Txt_Colaborador.Text = empleado.FullName;
                            Txt_Colaborador.ForeColor = Color.Black;
                        }
                    }
                    else
                    {
                        _colaboradorSeleccionado = null;
                        Txt_Colaborador.Text = "SELECCIONE UN COLABORADOR DE LA TABLA";
                        Txt_Colaborador.ForeColor = Color.Gray;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del usuario: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para ajustar columnas de Tabla1
        public void AjustarColumnasTabla1()
        {
            if (Tabla1.Columns.Contains("Username"))
                Tabla1.Columns["Username"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (Tabla1.Columns.Contains("FullName"))
                Tabla1.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (Tabla1.Columns.Contains("InstitutionalEmail"))
                Tabla1.Columns["InstitutionalEmail"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (Tabla1.Columns.Contains("CreatedDate"))
                Tabla1.Columns["CreatedDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (Tabla1.Columns.Contains("LastLoginDate"))
                Tabla1.Columns["LastLoginDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (Tabla1.Columns.Contains("IsLocked"))
                Tabla1.Columns["IsLocked"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        // Evento al cambiar selección en Tabla1
        private void Tabla1_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla1.SelectedRows.Count > 0)
            {
                CargarDatosUsuarioSeleccionado();
            }
        }
        #endregion ConfiguracionesTabla1_Usuarios
        #region ConfiguracionesTabla2_Colaboradores
        // Método para cargar colaboradores desde el controlador
        private void CargarColaboradores()
        {
            try
            {
                RefrescarListadoColaboradores();
                ConfigurarTabla2();
                AjustarColumnasTabla2();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar colaboradores: {ex.Message}");
            }
        }

        // Método para refrescar la tabla de colaboradores
        public void RefrescarListadoColaboradores()
        {
            colaboradoresList = Ctrl_Employees.MostrarEmpleados(paginaActualColaboradores, registrosPorPaginaColaboradores);
            Tabla2.DataSource = colaboradoresList;
        }

        // Método para configurar la tabla de colaboradores
        public void ConfigurarTabla2()
        {
            if (Tabla2.Columns.Count > 0)
            {
                void Ocultar(string nombreColumna)
                {
                    if (Tabla2.Columns.Contains(nombreColumna))
                        Tabla2.Columns[nombreColumna].Visible = false;
                }

                void Encabezado(string nombreColumna, string texto)
                {
                    if (Tabla2.Columns.Contains(nombreColumna))
                        Tabla2.Columns[nombreColumna].HeaderText = texto;
                }

                // Configurar títulos de columnas
                Encabezado("EmployeeCode", "CÓDIGO");
                Encabezado("FullName", "NOMBRE COMPLETO");
                Encabezado("IdentificationNumber", "DPI");
                Encabezado("InstitutionalEmail", "EMAIL");
                Encabezado("Phone", "TELÉFONO");

                // Ocultar columnas no necesarias
                Ocultar("EmployeeId");
                Ocultar("FirstName");
                Ocultar("LastName");
                Ocultar("Email");
                Ocultar("MobilePhone");
                Ocultar("Address");
                Ocultar("BirthDate");
                Ocultar("HireDate");
                Ocultar("TerminationDate");
                Ocultar("DepartmentId");
                Ocultar("PositionId");
                Ocultar("DirectSupervisorId");
                Ocultar("EmployeeStatusId");
                Ocultar("EmergencyContactName");
                Ocultar("EmergencyContactPhone");
                Ocultar("EmergencyContactRelation");

                // Campos salariales viejos y nuevos
                Ocultar("NominalSalary");
                Ocultar("BaseSalary");
                Ocultar("AdditionalBonus");
                Ocultar("LegalBonus");
                Ocultar("IGSS");
                Ocultar("ISR");
                Ocultar("NetSalary");
                Ocultar("IGSSManual");

                // Otros campos internos
                Ocultar("IsActive");
                Ocultar("CreatedDate");
                Ocultar("CreatedBy");
                Ocultar("ModifiedDate");
                Ocultar("ModifiedBy");
                Ocultar("LocationId");
                Ocultar("TipoContratacion");
            }

            // Configuración de selección y formato
            Tabla2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla2.MultiSelect = false;
            Tabla2.ReadOnly = true;
            Tabla2.AllowUserToResizeRows = false;
            Tabla2.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold);

            // Agregar evento de selección
            Tabla2.SelectionChanged -= Tabla2_SelectionChanged;
            Tabla2.SelectionChanged += Tabla2_SelectionChanged;
        }

        // Método para asignar colaborador seleccionado al usuario
        private void AsignarColaboradorSeleccionado()
        {
            try
            {
                if (Tabla2.SelectedRows.Count == 0) return;

                DataGridViewRow fila = Tabla2.SelectedRows[0];
                int employeeId = Convert.ToInt32(fila.Cells["EmployeeId"].Value);

                _colaboradorSeleccionado = Ctrl_Employees.ObtenerEmpleadoPorId(employeeId);

                if (_colaboradorSeleccionado != null)
                {
                    // Mostrar nombre en TextBox
                    Txt_Colaborador.Text = _colaboradorSeleccionado.FullName;
                    Txt_Colaborador.ForeColor = Color.Black;

                    // ⭐ SIEMPRE copiar el correo institucional del colaborador seleccionado
                    if (!string.IsNullOrWhiteSpace(_colaboradorSeleccionado.InstitutionalEmail))
                    {
                        Txt_CorreoInstitucional.Text = _colaboradorSeleccionado.InstitutionalEmail;
                        Txt_CorreoInstitucional.ForeColor = Color.Black;
                    }
                    else
                    {
                        // Si el colaborador no tiene correo, limpiar el campo
                        Txt_CorreoInstitucional.Text = "CORREO INSTITUCIONAL";
                        Txt_CorreoInstitucional.ForeColor = Color.Gray;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al asignar colaborador: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para ajustar columnas de Tabla2
        public void AjustarColumnasTabla2()
        {
            if (Tabla2.Columns.Contains("EmployeeCode"))
                Tabla2.Columns["EmployeeCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (Tabla2.Columns.Contains("FullName"))
                Tabla2.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (Tabla2.Columns.Contains("IdentificationNumber"))
                Tabla2.Columns["IdentificationNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (Tabla2.Columns.Contains("InstitutionalEmail"))
                Tabla2.Columns["InstitutionalEmail"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (Tabla2.Columns.Contains("Phone"))
                Tabla2.Columns["Phone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        // Evento al cambiar selección en Tabla2
        private void Tabla2_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla2.SelectedRows.Count > 0)
            {
                AsignarColaboradorSeleccionado();
            }
        }
        #endregion ConfiguracionesTabla2_Colaboradores
        #region ToolStripPaginacionUsuarios
        // Método para crear ToolStrip de paginación de Usuarios
        private void CrearToolStripPaginacionUsuarios()
        {
            toolStripPaginacionUsuarios = new ToolStrip();
            toolStripPaginacionUsuarios.Dock = DockStyle.Bottom; // ⭐ Dock en el panel
            toolStripPaginacionUsuarios.GripStyle = ToolStripGripStyle.Hidden;
            toolStripPaginacionUsuarios.BackColor = Color.FromArgb(248, 249, 250);
            toolStripPaginacionUsuarios.Height = 40;
            toolStripPaginacionUsuarios.AutoSize = true;

            // Botón Anterior
            btnAnteriorUsuarios = new ToolStripButton();
            btnAnteriorUsuarios.Text = "❮ Anterior";
            btnAnteriorUsuarios.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnteriorUsuarios.ForeColor = Color.White;
            btnAnteriorUsuarios.BackColor = Color.FromArgb(51, 140, 255);
            btnAnteriorUsuarios.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAnteriorUsuarios.Margin = new Padding(2);
            btnAnteriorUsuarios.Padding = new Padding(8, 4, 8, 4);
            btnAnteriorUsuarios.Click += (s, e) => CambiarPaginaUsuarios(paginaActualUsuarios - 1);

            toolStripPaginacionUsuarios.Items.Add(btnAnteriorUsuarios);
            ActualizarBotonesNumeradosUsuarios();

            // Botón Siguiente
            btnSiguienteUsuarios = new ToolStripButton();
            btnSiguienteUsuarios.Text = "Siguiente ❯";
            btnSiguienteUsuarios.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSiguienteUsuarios.ForeColor = Color.White;
            btnSiguienteUsuarios.BackColor = Color.FromArgb(238, 143, 109);
            btnSiguienteUsuarios.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSiguienteUsuarios.Margin = new Padding(2);
            btnSiguienteUsuarios.Padding = new Padding(8, 4, 8, 4);
            btnSiguienteUsuarios.Click += (s, e) => CambiarPaginaUsuarios(paginaActualUsuarios + 1);

            toolStripPaginacionUsuarios.Items.Add(btnSiguienteUsuarios);

            // ⭐ AGREGAR AL PANEL
            PanelToolStrip1.Controls.Add(toolStripPaginacionUsuarios);
            toolStripPaginacionUsuarios.BringToFront();
        }

        private void ActualizarBotonesNumeradosUsuarios()
        {
            var itemsToRemove = toolStripPaginacionUsuarios.Items.Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton").ToList();

            foreach (var item in itemsToRemove)
            {
                toolStripPaginacionUsuarios.Items.Remove(item);
            }

            if (totalPaginasUsuarios <= 1) return;

            int inicioRango = Math.Max(1, paginaActualUsuarios - 1);
            int finRango = Math.Min(totalPaginasUsuarios, paginaActualUsuarios + 1);
            int posicionInsertar = toolStripPaginacionUsuarios.Items.IndexOf(btnSiguienteUsuarios);

            for (int i = inicioRango; i <= finRango; i++)
            {
                ToolStripButton btnPagina = new ToolStripButton();
                btnPagina.Text = i.ToString();
                btnPagina.Tag = "PageButton";
                btnPagina.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnPagina.Margin = new Padding(1);
                btnPagina.Padding = new Padding(6, 4, 6, 4);

                if (i == paginaActualUsuarios)
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
                btnPagina.Click += (s, e) => CambiarPaginaUsuarios(numeroPagina);

                toolStripPaginacionUsuarios.Items.Insert(posicionInsertar++, btnPagina);
            }
        }

        private void CambiarPaginaUsuarios(int nuevaPagina)
        {
            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginasUsuarios)
            {
                paginaActualUsuarios = nuevaPagina;
                RefrescarListadoUsuarios();
                ConfigurarTabla1();
                AjustarColumnasTabla1();
                ActualizarInfoPaginacionUsuarios();
            }
        }

        private void ActualizarInfoPaginacionUsuarios()
        {
            totalRegistrosUsuarios = Ctrl_Users.ContarTotalUsuarios();
            totalPaginasUsuarios = (int)Math.Ceiling((double)totalRegistrosUsuarios / registrosPorPaginaUsuarios);

            btnAnteriorUsuarios.Enabled = paginaActualUsuarios > 1;
            btnSiguienteUsuarios.Enabled = paginaActualUsuarios < totalPaginasUsuarios;

            ActualizarBotonesNumeradosUsuarios();

            int inicioRango = (paginaActualUsuarios - 1) * registrosPorPaginaUsuarios + 1;
            int finRango = Math.Min(paginaActualUsuarios * registrosPorPaginaUsuarios, totalRegistrosUsuarios);
        }
        #endregion ToolStripPaginacionUsuarios
        #region ToolStripPaginacionColaboradores
        private void CrearToolStripPaginacionColaboradores()
        {
            toolStripPaginacionColaboradores = new ToolStrip();
            toolStripPaginacionColaboradores.Dock = DockStyle.Bottom; // ⭐ Dock en el panel
            toolStripPaginacionColaboradores.GripStyle = ToolStripGripStyle.Hidden;
            toolStripPaginacionColaboradores.BackColor = Color.FromArgb(248, 249, 250);
            toolStripPaginacionColaboradores.Height = 40;
            toolStripPaginacionColaboradores.AutoSize = true;

            btnAnteriorColaboradores = new ToolStripButton();
            btnAnteriorColaboradores.Text = "❮ Anterior";
            btnAnteriorColaboradores.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnteriorColaboradores.ForeColor = Color.White;
            btnAnteriorColaboradores.BackColor = Color.FromArgb(51, 140, 255);
            btnAnteriorColaboradores.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAnteriorColaboradores.Margin = new Padding(2);
            btnAnteriorColaboradores.Padding = new Padding(8, 4, 8, 4);
            btnAnteriorColaboradores.Click += (s, e) => CambiarPaginaColaboradores(paginaActualColaboradores - 1);

            toolStripPaginacionColaboradores.Items.Add(btnAnteriorColaboradores);
            ActualizarBotonesNumeradosColaboradores();

            btnSiguienteColaboradores = new ToolStripButton();
            btnSiguienteColaboradores.Text = "Siguiente ❯";
            btnSiguienteColaboradores.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSiguienteColaboradores.ForeColor = Color.White;
            btnSiguienteColaboradores.BackColor = Color.FromArgb(238, 143, 109);
            btnSiguienteColaboradores.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSiguienteColaboradores.Margin = new Padding(2);
            btnSiguienteColaboradores.Padding = new Padding(8, 4, 8, 4);
            btnSiguienteColaboradores.Click += (s, e) => CambiarPaginaColaboradores(paginaActualColaboradores + 1);

            toolStripPaginacionColaboradores.Items.Add(btnSiguienteColaboradores);

            // ⭐ AGREGAR AL PANEL
            PanelToolStrip2.Controls.Add(toolStripPaginacionColaboradores);
            toolStripPaginacionColaboradores.BringToFront();
        }

        private void ActualizarBotonesNumeradosColaboradores()
        {
            var itemsToRemove = toolStripPaginacionColaboradores.Items.Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton").ToList();

            foreach (var item in itemsToRemove)
            {
                toolStripPaginacionColaboradores.Items.Remove(item);
            }

            if (totalPaginasColaboradores <= 1) return;

            int inicioRango = Math.Max(1, paginaActualColaboradores - 1);
            int finRango = Math.Min(totalPaginasColaboradores, paginaActualColaboradores + 1);
            int posicionInsertar = toolStripPaginacionColaboradores.Items.IndexOf(btnSiguienteColaboradores);

            for (int i = inicioRango; i <= finRango; i++)
            {
                ToolStripButton btnPagina = new ToolStripButton();
                btnPagina.Text = i.ToString();
                btnPagina.Tag = "PageButton";
                btnPagina.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnPagina.Margin = new Padding(1);
                btnPagina.Padding = new Padding(6, 4, 6, 4);

                if (i == paginaActualColaboradores)
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
                btnPagina.Click += (s, e) => CambiarPaginaColaboradores(numeroPagina);

                toolStripPaginacionColaboradores.Items.Insert(posicionInsertar++, btnPagina);
            }
        }

        private void CambiarPaginaColaboradores(int nuevaPagina)
        {
            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginasColaboradores)
            {
                paginaActualColaboradores = nuevaPagina;
                RefrescarListadoColaboradores();
                ConfigurarTabla2();
                AjustarColumnasTabla2();
                ActualizarInfoPaginacionColaboradores();
            }
        }

        private void ActualizarInfoPaginacionColaboradores()
        {
            totalRegistrosColaboradores = Ctrl_Employees.ContarTotalEmpleados();
            totalPaginasColaboradores = (int)Math.Ceiling((double)totalRegistrosColaboradores / registrosPorPaginaColaboradores);

            btnAnteriorColaboradores.Enabled = paginaActualColaboradores > 1;
            btnSiguienteColaboradores.Enabled = paginaActualColaboradores < totalPaginasColaboradores;

            ActualizarBotonesNumeradosColaboradores();
        }
        #endregion ToolStripPaginacionColaboradores
        #region ValidarCamposObligatorios
        // Método para validar campos obligatorios antes de guardar
        private bool ValidarCamposObligatorios()
        {
            bool TienePlaceholder(TextBox txt, string placeholder)
            {
                return string.IsNullOrWhiteSpace(txt.Text) || txt.Text == placeholder || txt.ForeColor == Color.Gray;
            }

            // 1. Validar USERNAME*
            if (TienePlaceholder(Txt_Usuario, "NOMBRE DE USUARIO"))
            {
                MessageBox.Show("El campo USERNAME es obligatorio", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Usuario.Focus();
                return false;
            }

            // Validar username único
            if (!Ctrl_Users.ValidarUsernameUnico(Txt_Usuario.Text, _usuarioSeleccionado?.UserId))
            {
                MessageBox.Show("El USERNAME ya existe en el sistema", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Usuario.Focus();
                return false;
            }

            // 2. Validar PASSWORD* (solo para nuevos usuarios)
            if (_usuarioSeleccionado == null && TienePlaceholder(Txt_Password, "CONTRASEÑA"))
            {
                MessageBox.Show("El campo CONTRASEÑA es obligatorio para nuevos usuarios", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Password.Focus();
                return false;
            }

            // 3. Validar ROL*
            if (ComboBox_Rol.SelectedIndex < 0 || ComboBox_Rol.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un ROL", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Rol.Focus();
                return false;
            }

            // 4. Validar ESTADO*
            if (ComboBox_UserStatus.SelectedIndex < 0 || ComboBox_UserStatus.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un ESTADO", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_UserStatus.Focus();
                return false;
            }

            // 5. Validar CORREO INSTITUCIONAL*
            if (TienePlaceholder(Txt_CorreoInstitucional, "CORREO INSTITUCIONAL"))
            {
                MessageBox.Show("El campo CORREO INSTITUCIONAL es obligatorio", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_CorreoInstitucional.Focus();
                return false;
            }

            if (!Txt_CorreoInstitucional.Text.Contains("@"))
            {
                MessageBox.Show("El CORREO INSTITUCIONAL debe tener un formato válido", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_CorreoInstitucional.Focus();
                return false;
            }

            // 6. Validar COLABORADOR* (opcional pero recomendado)
            if (_colaboradorSeleccionado == null)
            {
                var confirmacion = MessageBox.Show(
                    "No ha seleccionado un colaborador. ¿Desea continuar sin asignar colaborador?",
                    "Advertencia",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return false;
            }

            return true;
        }
        #endregion ValidarCamposObligatorios
        #region AsignacionFocus
        private void ConfigurarTabIndexYFocus()
        {
            Txt_ValorBuscado1.TabIndex = 0;
            Txt_Usuario.TabIndex = 1;
            Txt_Password.TabIndex = 2;
            ComboBox_Rol.TabIndex = 3;
            ComboBox_UserStatus.TabIndex = 4;
            CheckBox_PasswordTemp.TabIndex = 5;
            Txt_Colaborador.TabIndex = 6;
            Txt_CorreoInstitucional.TabIndex = 7;
            Btn_Save.TabIndex = 8;
            Btn_Update.TabIndex = 9;
            Btn_Send.TabIndex = 10;

            Txt_ValorBuscado1.Focus();
        }
        #endregion AsignacionFocus
        #region CRUD
        // Evento para botón Guardar
        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCamposObligatorios())
                    return;

                var confirmacion = MessageBox.Show(
                    "¿Está seguro que desea registrar este usuario?",
                    "Confirmar Registro",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                bool TienePlaceholder(TextBox txt, string placeholder)
                {
                    return string.IsNullOrWhiteSpace(txt.Text) || txt.Text == placeholder || txt.ForeColor == Color.Gray;
                }

                string passwordPlainText = Txt_Password.Text.Trim();

                var nuevoUsuario = new Mdl_Users
                {
                    Username = Txt_Usuario.Text.Trim().ToUpper(),
                    FullName = _colaboradorSeleccionado?.FullName?.ToUpper() ?? Txt_Usuario.Text.Trim().ToUpper(),
                    RoleId = (int)ComboBox_Rol.SelectedValue,
                    StatusId = (int)ComboBox_UserStatus.SelectedValue,
                    IsLocked = (int)ComboBox_Bloqueado.SelectedValue == 1,
                    NotificationsEnabled = true,
                    IsTemporaryPassword = CheckBox_PasswordTemp.Checked,
                    InstitutionalEmail = Txt_CorreoInstitucional.Text.Trim().ToUpper(),
                    EmployeeId = _colaboradorSeleccionado?.EmployeeId,
                    PasswordExpiryDate = CheckBox_PasswordTemp.Checked ? DateTime.Now.AddDays(30) : (DateTime?)null,
                    CreatedBy = UserData?.UserId ?? 1
                };

                int resultado = Ctrl_Users.RegistrarUsuario(nuevoUsuario, passwordPlainText);

                if (resultado > 0)
                {
                    MessageBox.Show("Usuario registrado exitosamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListadoUsuarios();
                    ActualizarInfoPaginacionUsuarios();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el usuario", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento para botón Actualizar
        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioSeleccionado == null || _usuarioSeleccionado.UserId == 0)
                {
                    MessageBox.Show("Debe seleccionar un usuario para actualizar", "Validación",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarCamposObligatorios())
                    return;

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea actualizar los datos del usuario {_usuarioSeleccionado.Username}?",
                    "Confirmar Actualización",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                _usuarioSeleccionado.Username = Txt_Usuario.Text.Trim().ToUpper();
                _usuarioSeleccionado.FullName = _colaboradorSeleccionado?.FullName?.ToUpper() ?? Txt_Usuario.Text.Trim().ToUpper();
                _usuarioSeleccionado.RoleId = (int)ComboBox_Rol.SelectedValue;
                _usuarioSeleccionado.StatusId = (int)ComboBox_UserStatus.SelectedValue;
                _usuarioSeleccionado.IsLocked = (int)ComboBox_Bloqueado.SelectedValue == 1;
                _usuarioSeleccionado.InstitutionalEmail = Txt_CorreoInstitucional.Text.Trim().ToUpper();
                _usuarioSeleccionado.EmployeeId = _colaboradorSeleccionado?.EmployeeId;
                _usuarioSeleccionado.ModifiedBy = UserData?.UserId ?? 1;

                int resultado = Ctrl_Users.ActualizarUsuario(_usuarioSeleccionado);

                if (resultado > 0)
                {
                    MessageBox.Show("Usuario actualizado exitosamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListadoUsuarios();
                    ActualizarInfoPaginacionUsuarios();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el usuario", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion CRUD
        #region Limpieza
        private void LimpiarFormulario()
        {
            _usuarioSeleccionado = null;
            _colaboradorSeleccionado = null;

            ConfigurarPlaceHoldersTextbox();

            if (ComboBox_Rol.Items.Count > 0)
                ComboBox_Rol.SelectedIndex = 0;

            if (ComboBox_UserStatus.Items.Count > 0)
                ComboBox_UserStatus.SelectedIndex = 0;

            if (ComboBox_Bloqueado.Items.Count > 0) 
                ComboBox_Bloqueado.SelectedIndex = 0;

            CheckBox_PasswordTemp.Checked = true;

            Txt_Usuario.Focus();
        }
        #endregion Limpieza
        #region EnviarCorreo
        // Evento para botón Enviar Credenciales por Correo
        private void Btn_Send_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioSeleccionado == null)
                {
                    MessageBox.Show("Debe seleccionar un usuario para enviar credenciales", "Validación",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_usuarioSeleccionado.InstitutionalEmail))
                {
                    MessageBox.Show("El usuario no tiene correo institucional registrado", "Validación",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿Desea enviar las credenciales del usuario {_usuarioSeleccionado.Username} al correo {_usuarioSeleccionado.InstitutionalEmail}?",
                    "Confirmar Envío de Credenciales",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                // ===== CONFIGURACIÓN DEL CORREO =====
                string correoEmisor = "notificaciones@uregionalregion2.edu.gt";
                string contraseñaEmisor = "F0rza01.";

                SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(correoEmisor, contraseñaEmisor),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(correoEmisor, "SECRON - Sistema de Control Regional"),
                    Subject = "Credenciales de Acceso - SECRON",
                    IsBodyHtml = true,
                };

                mail.Body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd;'>
                        <h2 style='color: #2A7AE2; text-align: center;'>SECRON - Sistema de Control Regional</h2>
                        <p>Estimado/a <strong>{_usuarioSeleccionado.FullName}</strong>,</p>
                        <p>Le hacemos llegar sus credenciales de acceso al sistema SECRON:</p>
                        <table style='border-collapse: collapse; width: 100%; margin: 20px 0;'>
                            <tr>
                                <td style='border: 1px solid #ddd; padding: 12px; background-color: #f9f9f9;'><strong>Usuario:</strong></td>
                                <td style='border: 1px solid #ddd; padding: 12px;'>{_usuarioSeleccionado.Username}</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid #ddd; padding: 12px; background-color: #f9f9f9;'><strong>Contraseña:</strong></td>
                                <td style='border: 1px solid #ddd; padding: 12px;'>**********</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid #ddd; padding: 12px; background-color: #f9f9f9;'><strong>Estado:</strong></td>
                                <td style='border: 1px solid #ddd; padding: 12px;'>{(ComboBox_UserStatus.Text ?? "ACTIVO")}</td>
                            </tr>
                        </table>
                        
                        <div style='background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0;'>
                            <p style='margin: 0;'><strong>⚠️ IMPORTANTE:</strong></p>
                            <ul style='margin: 10px 0;'>
                                <li>Sus credenciales son personales e intransferibles</li>
                                <li>NO comparta su contraseña con nadie</li>
                                <li>Se recomienda cambiar la contraseña al primer inicio de sesión</li>
                                <li>En caso de olvido, contacte al administrador del sistema</li>
                            </ul>
                        </div>
                        
                        <p style='color: #555; margin-top: 30px;'>Atentamente,</p>
                        <p><strong>Equipo de Desarrollo SECRON</strong></p>
                        <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                        <p style='font-size: 12px; color: #888; text-align: center;'>
                            Este es un mensaje automático del Sistema de Control Regional (SECRON)<br>
                            No responder a este correo
                        </p>
                    </div>
                </body>
                </html>";

                mail.To.Add(_usuarioSeleccionado.InstitutionalEmail);
                smtpClient.Send(mail);

                MessageBox.Show("Las credenciales han sido enviadas exitosamente al correo institucional", "Éxito",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar el correo: {ex.Message}\n\nVerifique que el correo sea válido y que tenga conexión a internet.",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion EnviarCorreo
        #region ExportarExcel
        // Evento para botón Exportar a Excel
        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                if (usuariosList == null || usuariosList.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar", "Información",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Lista de Usuarios",
                    FileName = $"Usuarios_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    var excelApp = new Microsoft.Office.Interop.Excel.Application();
                    var workbook = excelApp.Workbooks.Add();
                    var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Usuarios";

                    // ============ ENCABEZADO PRINCIPAL ============
                    worksheet.Cells[1, 1] = "REPORTE DE USUARIOS - SECRON";
                    worksheet.Range["A1:G1"].Merge();
                    worksheet.Range["A1:G1"].Font.Size = 16;
                    worksheet.Range["A1:G1"].Font.Bold = true;
                    worksheet.Range["A1:G1"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Range["A1:G1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(238, 143, 109));
                    worksheet.Range["A1:G1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                    // ============ INFORMACIÓN DEL REPORTE ============
                    worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SISTEMA"}";
                    worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {usuariosList.Count}";

                    worksheet.Range["A2:A4"].Font.Size = 10;
                    worksheet.Range["A2:A4"].Font.Bold = true;

                    // ============ ENCABEZADOS DE COLUMNAS ============
                    int headerRow = 6;
                    string[] headers = { "USERNAME", "NOMBRE COMPLETO", "EMAIL INSTITUCIONAL",
                                "FECHA CREACIÓN", "ÚLTIMO LOGIN", "BLOQUEADO", "ESTADO" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[headerRow, i + 1] = headers[i];
                    }

                    var headerRange = worksheet.Range[$"A{headerRow}:G{headerRow}"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Size = 11;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    headerRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                    // ============ DATOS ============
                    int row = headerRow + 1;
                    foreach (var user in usuariosList)
                    {
                        worksheet.Cells[row, 1] = user.Username ?? "";
                        worksheet.Cells[row, 2] = user.FullName ?? "";
                        worksheet.Cells[row, 3] = user.InstitutionalEmail ?? "";
                        worksheet.Cells[row, 4] = user.CreatedDate.ToString("dd/MM/yyyy");
                        worksheet.Cells[row, 5] = user.LastLoginDate?.ToString("dd/MM/yyyy HH:mm") ?? "SIN LOGIN";
                        worksheet.Cells[row, 6] = user.IsLocked ? "SÍ" : "NO";

                        // Obtener nombre del estado
                        var estado = Ctrl_UserStatus.ObtenerTodosLosEstados()
                            .FirstOrDefault(s => s.Key == user.StatusId);
                        worksheet.Cells[row, 7] = estado.Value ?? "";

                        // Alternar color de filas
                        if (row % 2 == 0)
                        {
                            worksheet.Range[$"A{row}:G{row}"].Interior.Color =
                                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                        }

                        row++;
                    }

                    // ============ FORMATO FINAL ============
                    var dataRange = worksheet.Range[$"A{headerRow}:G{row - 1}"];
                    dataRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    dataRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;

                    worksheet.Columns.AutoFit();

                    worksheet.Activate();
                    excelApp.ActiveWindow.SplitRow = headerRow;
                    excelApp.ActiveWindow.FreezePanes = true;

                    // ============ PIE DE PÁGINA ============
                    worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].Merge();
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].Font.Italic = true;
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].Font.Size = 9;
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].HorizontalAlignment =
                        Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    workbook.SaveAs(saveFileDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                    this.Cursor = Cursors.Default;

                    var result = MessageBox.Show(
                        "Archivo exportado exitosamente.\n\n¿Desea abrir el archivo ahora?",
                        "Exportación Exitosa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion ExportarExcel
        #region vScrollBar
        private void Panel_Derecho_MouseEnter(object sender, EventArgs e)
        {
            Panel_Derecho.Focus();
        }

        // Manejar evento Scroll del vScrollBar
        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            int scrollPosition = vScrollBar.Value;

            foreach (Control ctrl in Panel_Derecho.Controls)
            {
                // Obtener posición original guardada en Tag
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                {
                    ctrl.Tag = "OrigY:" + ctrl.Top;
                }

                string[] parts = ctrl.Tag.ToString().Split(':');
                int originalY = int.Parse(parts[1]);

                // Aplicar scroll: posición original menos desplazamiento
                ctrl.Top = originalY - scrollPosition;
            }

            Panel_Derecho.Invalidate();
        }

        // Manejar evento MouseWheel para Panel_Derecho
        private void Panel_Derecho_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!vScrollBar.Visible) return;

            int delta = e.Delta / 120;
            int newValue = vScrollBar.Value - (delta * 30); // 30 píxeles por scroll

            if (newValue < 0) newValue = 0;
            if (newValue > vScrollBar.Maximum) newValue = vScrollBar.Maximum;

            vScrollBar.Value = newValue;

            // Mover el contenido manualmente
            MoverContenidoPanelDerecho(newValue);
        }

        // Método para mover contenido del Panel_Derecho
        private void MoverContenidoPanelDerecho(int scrollPosition)
        {
            foreach (Control ctrl in Panel_Derecho.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                {
                    ctrl.Tag = "OrigY:" + ctrl.Top;
                }
                string[] parts = ctrl.Tag.ToString().Split(':');
                int originalY = int.Parse(parts[1]);
                ctrl.Top = originalY - scrollPosition;
            }
            Panel_Derecho.Invalidate();
        }

        // Configurar eventos MouseWheel para Panel_Derecho y sus controles hijos
        private void ConfigurarEventosScrollPanelDerecho()
        {
            Panel_Derecho.TabStop = true;
            Panel_Derecho.MouseWheel += Panel_Derecho_MouseWheel;
            Panel_Derecho.MouseEnter += Panel_Derecho_MouseEnter;

            foreach (Control ctrl in Panel_Derecho.Controls)
            {
                ctrl.MouseWheel += Panel_Derecho_MouseWheel;
            }
        }

        // Inicializar configuración del vScrollBar
        private void InicializarScrollPanelDerecho()
        {
            // Primero guardar posiciones originales
            foreach (Control ctrl in Panel_Derecho.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                {
                    ctrl.Tag = "OrigY:" + ctrl.Top;
                }
            }

            // Calcular altura total del contenido
            int maxBottom = 0;
            foreach (Control ctrl in Panel_Derecho.Controls)
            {
                maxBottom = Math.Max(maxBottom, ctrl.Bottom);
            }

            int totalContentHeight = maxBottom + (Panel_Derecho.Height / 3); // Dinámico

            // Si no necesita scroll, ocultar scrollbar
            if (totalContentHeight <= Panel_Derecho.Height)
            {
                vScrollBar.Visible = false;
                return;
            }

            // Configurar scrollbar
            vScrollBar.Visible = true;
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = totalContentHeight - Panel_Derecho.Height;
            vScrollBar.SmallChange = 30;
            vScrollBar.LargeChange = Panel_Derecho.Height / 4;

            vScrollBar.Scroll -= vScrollBar_Scroll;
            vScrollBar.Scroll += vScrollBar_Scroll;
            vScrollBar.Value = 0;
        }
        #endregion vScrollBar
        #region EstiloBotones
        private void AplicarEstiloBotonAgregar(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(9, 184, 255);
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            boton.Height = 45;
            boton.Width = Math.Max(boton.Width, 180);
            boton.Cursor = Cursors.Hand;
            boton.TextAlign = ContentAlignment.MiddleCenter;

            // Esquinas redondeadas
            boton.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, boton.Width, boton.Height, 20, 20));

            // Efectos hover
            boton.MouseEnter += (s, e) =>
            {
                boton.BackColor = Color.FromArgb(0, 150, 220);
            };

            boton.MouseLeave += (s, e) =>
            {
                boton.BackColor = Color.FromArgb(9, 184, 255);
            };
        }

        private void AplicarEstilosBotones()
        {
            AplicarEstiloBotonAgregar(Btn_ResetPassword);
            AplicarEstiloBotonAgregar(Btn_DesbloquearUsuario);
        }
        #endregion EstiloBotones
        #region RestablecerContraseña
        // Evento para botón Restablecer Contraseña Temporal
        private void Btn_ResetPassword_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionado == null || _usuarioSeleccionado.UserId == 0)
            {
                MessageBox.Show("Debe seleccionar un usuario de la tabla", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Abrir formulario de contraseña temporal
            var frmTempPassword = new Frm_Security_TemporalPassword
            {
                UsuarioRestablecerPassword = _usuarioSeleccionado.Username,
                UserId = _usuarioSeleccionado.UserId,
                UserData = this.UserData // Usuario administrador que hace el cambio (sesión activa)
            };

            if (frmTempPassword.ShowDialog() == DialogResult.OK)
            {
                // Refrescar tabla si se completó exitosamente
                RefrescarListadoUsuarios();
                ConfigurarTabla1();
                AjustarColumnasTabla1();
            }
        }
        #endregion RestablecerContraseña
        #region DesbloquearUsuario
        // Evento para boton Desbloquear Usuario
        private void Btn_DesbloquearUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que haya un usuario seleccionado
                if (_usuarioSeleccionado == null || _usuarioSeleccionado.UserId == 0)
                {
                    MessageBox.Show(
                        "DEBE SELECCIONAR UN USUARIO PARA DESBLOQUEAR",
                        "VALIDACIÓN",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Guardar el UserId antes de cualquier operacion
                int userIdADesbloquear = _usuarioSeleccionado.UserId;
                string usernameADesbloquear = _usuarioSeleccionado.Username;

                // Validar que el usuario este realmente bloqueado
                if (!_usuarioSeleccionado.IsLocked)
                {
                    MessageBox.Show(
                        $"EL USUARIO {usernameADesbloquear} NO ESTÁ BLOQUEADO",
                        "INFORMACIÓN",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Confirmacion de desbloqueo
                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO QUE DESEA DESBLOQUEAR AL USUARIO {usernameADesbloquear}?\n\n" +
                    $"ESTO PERMITIRÁ QUE EL USUARIO PUEDA VOLVER A INICIAR SESIÓN EN EL SISTEMA.",
                    "CONFIRMAR DESBLOQUEO",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes)
                    return;

                this.Cursor = Cursors.WaitCursor;

                // Usar metodo especifico de desbloqueo
                int resultado = Ctrl_Users.DesbloquearUsuario(userIdADesbloquear, UserData?.UserId ?? 1);

                if (resultado > 0)
                {
                    // Recargar el usuario desde la base de datos para asegurar que los cambios se reflejen
                    _usuarioSeleccionado = Ctrl_Users.ObtenerUsuarioPorId(userIdADesbloquear);

                    // Actualizar el ComboBox_Bloqueado en el formulario
                    if (_usuarioSeleccionado != null)
                    {
                        ComboBox_Bloqueado.SelectedValue = _usuarioSeleccionado.IsLocked ? 1 : 0;
                    }

                    // Refrescar listado
                    RefrescarListadoUsuarios();
                    ActualizarInfoPaginacionUsuarios();

                    this.Cursor = Cursors.Default;

                    MessageBox.Show(
                        $"USUARIO {usernameADesbloquear} DESBLOQUEADO EXITOSAMENTE",
                        "ÉXITO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(
                        "NO SE PUDO DESBLOQUEAR EL USUARIO",
                        "ERROR",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(
                    $"ERROR AL DESBLOQUEAR USUARIO: {ex.Message}",
                    "ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion DesbloquearUsuario
    }
}