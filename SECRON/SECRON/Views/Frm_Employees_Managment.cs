using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// Agregar Referencias necesarias
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_Employees_Managment : Form
    {
        #region PropiedadesIniciales
        // Variables globales
        // Variables globales para mantener los filtros activos (agregar al inicio de la clase)
        private string _ultimoTextoBusqueda = "";
        private int? _ultimoDepartmentId = null;
        private int? _ultimoPositionId = null;
        private int? _ultimoEmployeeStatusId = null;
        private int? _ultimoLocationId = null;
        private string _ultimoFiltroSupervisor = "TODOS";
        private List<Mdl_Employees> _listaCompletaFiltrada = null;
        // Modelo de empleados
        public Mdl_Security_UserInfo UserData { get; set; }
        // Empleado seleccionado para editar
        private Mdl_Employees _empleadoSeleccionado = null;
        // Lista para almacenar empleados
        private List<Mdl_Employees> empleadosList;
        // Variables para paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        // ToolStrip para paginación
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;
        private ToolStripLabel lblPaginaInfo;
        private bool _cargandoFormulario = true;
        // Evento Load del formulario
        private void Frm_Employees_Managment_Load(object sender, EventArgs e)
        {
            try
            {
                _cargandoFormulario = true;

                // Configuración visual y de navegación
                ConfigurarTabIndexYFocus();
                ConfigurarPlaceHoldersTextbox();
                ConfigurarMaxLengthTextBox();
                ConfigurarDateTimePickers();

                // Configurar y cargar ComboBox (SIEMPRE primero)
                ConfigurarComboBoxes();
                CargarFiltros();

                // Configuración de tabla y paginación
                CrearToolStripPaginacion();
                CargarEmpleados();
                ActualizarInfoPaginacion();

                // Scroll y eventos auxiliares
                InicializarScroll();
                ConfigurarEventosScroll();

                // Estado inicial del formulario
                CargarProximoCodigo();
                HabilitarControlesEdicion(true, false);

                // Evitar selección automática al iniciar
                Tabla.ClearSelection();

                // Habilitar eventos (a partir de aquí ya es seguro)
                _cargandoFormulario = false;

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
            // Configurar DateTimePicker
            ConfigurarDateTimePickers();
            // Configurar ScrollBar
            InicializarScroll();
            // Configurar eventos del Scroll
            ConfigurarEventosScroll();
            // Configurar ComboBoxes
            ConfigurarComboBoxes();
            // Cargar código de docente automático
            CargarProximoCodigo();
            // Activar/Desactivar controles
            HabilitarControlesEdicion(true, false);
        }
        // Método separado
        private void FormularioResize(object sender, EventArgs e)
        {
            if (Tabla != null && Tabla.DataSource != null)
            {
                Tabla.Refresh();
            }
        }
        // Constructor del formulario
        public Frm_Employees_Managment()
        {
            // Llamar al método InitializeComponent
            InitializeComponent();
            this.Resize += FormularioResize;
            this.Resize += (s, e) => {
                if (toolStripPaginacion != null)
                {
                    toolStripPaginacion.Location = new Point(this.Width - 300, 225);
                }
            };
        }
        #endregion PropiedadesIniciales
        #region ConfigurarTextBox
        // Método para asignar tamaño máximo de los cuadros de Texto
        private void ConfigurarMaxLengthTextBox()
        {
            Txt_ValorBuscado.MaxLength = 100;
            Txt_Codigo.MaxLength = 20;
            Txt_Dpi.MaxLength = 13;  // DPI de Guatemala tiene 13 dígitos
            Txt_Apellidos.MaxLength = 100;
            Txt_CorreoInstitucional.MaxLength = 100;
            Txt_CorreoPersonal.MaxLength = 100;
            Txt_Direccion.MaxLength = 200;
            Txt_Nombres.MaxLength = 100;
            Txt_Parentesco.MaxLength = 50;
            Txt_PersonaEmergencia.MaxLength = 100;
            Txt_Telefono1.MaxLength = 12;  // +502 + 8 dígitos
            Txt_Telefono2.MaxLength = 12;
            Txt_TelefonoEmergencia.MaxLength = 12;
        }
        // Metodo para configurar placeholders en los TextBox
        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR POR NOMBRE, DEPARTAMENTO, DPI...");
            ConfigurarPlaceHolder(Txt_Codigo, "CODIGO EMPLEADO");
            ConfigurarPlaceHolder(Txt_Dpi, "IDENTIFICACION");
            ConfigurarPlaceHolder(Txt_Apellidos, "APELLIDOS");
            ConfigurarPlaceHolder(Txt_CorreoInstitucional, "CORREO INSTITUCIONAL");
            ConfigurarPlaceHolder(Txt_CorreoPersonal, "CORREO PERSONAL");
            ConfigurarPlaceHolder(Txt_Direccion, "DIRECCIÓN COMPLETA");
            ConfigurarPlaceHolder(Txt_Nombres, "NOMBRES");
            ConfigurarPlaceHolder(Txt_Parentesco, "PARENTESCO");
            ConfigurarPlaceHolder(Txt_PersonaEmergencia, "NOMBRE DE CONTACTO");
            ConfigurarPlaceHolder(Txt_Telefono1, "+502");
            ConfigurarPlaceHolder(Txt_Telefono2, "+502");
            ConfigurarPlaceHolder(Txt_TelefonoEmergencia, "+502");
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
        #region CodigoAutomatico
        // Obtiene y muestra el próximo código disponible
        private void CargarProximoCodigo()
        {
            try
            {
                string proximoCodigo = Ctrl_Employees.ObtenerProximoCodigo();
                Txt_Codigo.Text = proximoCodigo;
                Txt_Codigo.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el próximo código: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Txt_Codigo.Text = "ERROR";
            }
        }
        #endregion CodigoAutomatico
        #region ConfigurarDateTimePicker
        // Método para configurar DateTimePicker
        private void ConfigurarDateTimePickers()
        {
            // ===== CONFIGURAR DTP_Nacimiento =====
            // Primero establecer MinDate, luego MaxDate, luego Value
            DTP_Nacimiento.MinDate = new DateTime(1950, 1, 1);
            DTP_Nacimiento.MaxDate = new DateTime(2012, 12, 31);
            DTP_Nacimiento.Value = new DateTime(2000, 1, 1); // Valor por defecto
            DTP_Nacimiento.Format = DateTimePickerFormat.Short;

            // ===== CONFIGURAR DTP_Ingreso =====
            // Primero establecer MinDate, luego MaxDate, luego Value
            DTP_Ingreso.MinDate = new DateTime(2010, 1, 1);
            DTP_Ingreso.MaxDate = DateTime.Now;
            DTP_Ingreso.Value = DateTime.Now; // Fecha actual por defecto
            DTP_Ingreso.Format = DateTimePickerFormat.Short;
        }
        #endregion ConfigurarDateTimePicker
        #region ConfigurarComboBox
        // Método para configurar ComboBoxes
        private void ConfigurarComboBoxes()
        {
            // Configurar propiedades de los ComboBox para que no permitan escritura
            ComboBox_Departamento.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Puesto.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Supervisor.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Sede.DropDownStyle = ComboBoxStyle.DropDownList;             
            ComboBox_TipoContratacion.DropDownStyle = ComboBoxStyle.DropDownList;  
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            // Cargar datos
            CargarDepartamentos();
            CargarPuestos();
            CargarSupervisores();
            CargarSedes();               
            CargarTiposContratacion();  
        }
        // Cargar Sedes desde Ctrl_Locations
        private void CargarSedes()
        {
            try
            {
                var sedes = Ctrl_Locations.ObtenerLocationsActivas();

                // Crear lista con opción inicial "SIN SEDE"
                var listaSedes = new List<KeyValuePair<int, string>>
        {
            new KeyValuePair<int, string>(0, "-- SIN SEDE --")
        };
                listaSedes.AddRange(sedes);

                ComboBox_Sede.DataSource = new BindingSource(listaSedes, null);
                ComboBox_Sede.DisplayMember = "Value"; // LocationName
                ComboBox_Sede.ValueMember = "Key";     // LocationId
                ComboBox_Sede.SelectedIndex = 0; // Por defecto "SIN SEDE"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar sedes: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cargar Tipos de Contratación
        private void CargarTiposContratacion()
        {
            try
            {
                ComboBox_TipoContratacion.Items.AddRange(new object[]
                {
            "-- SELECCIONAR --",
            "SUELDOS",
            "HONORARIOS"
                });
                ComboBox_TipoContratacion.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tipos de contratación: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Cargar Departamentos desde Ctrl_Employees
        private void CargarDepartamentos()
        {
            try
            {
                var departamentos = Ctrl_Employees.ObtenerDepartamentos();

                ComboBox_Departamento.DataSource = new BindingSource(departamentos, null);
                ComboBox_Departamento.DisplayMember = "Value"; // El nombre del departamento
                ComboBox_Departamento.ValueMember = "Key";     // El ID del departamento

                if (ComboBox_Departamento.Items.Count > 0)
                    ComboBox_Departamento.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar departamentos: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Cargar Puestos desde Ctrl_Employees
        private void CargarPuestos()
        {
            try
            {
                var puestos = Ctrl_Employees.ObtenerPuestos();

                ComboBox_Puesto.DataSource = new BindingSource(puestos, null);
                ComboBox_Puesto.DisplayMember = "Value"; // El nombre del puesto
                ComboBox_Puesto.ValueMember = "Key";     // El ID del puesto

                if (ComboBox_Puesto.Items.Count > 0)
                    ComboBox_Puesto.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar puestos: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Cargar Supervisores desde Ctrl_Employees (NO obligatorio)
        private void CargarSupervisores()
        {
            try
            {
                var supervisores = Ctrl_Employees.ObtenerSupervisores();

                // Crear lista con opción inicial "SIN SUPERVISOR"
                var listaSupervisores = new List<KeyValuePair<int, string>>
        {
            new KeyValuePair<int, string>(0, "-- SIN SUPERVISOR --")
        };
                listaSupervisores.AddRange(supervisores);

                ComboBox_Supervisor.DataSource = new BindingSource(listaSupervisores, null);
                ComboBox_Supervisor.DisplayMember = "Value"; // Nombre completo
                ComboBox_Supervisor.ValueMember = "Key";     // EmployeeId
                ComboBox_Supervisor.SelectedIndex = 0; // Por defecto "SIN SUPERVISOR"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar supervisores: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion ConfigurarComboBox
        #region Filtros
        // Cargar opciones en los ComboBox de filtros
        private void CargarFiltros()
        {
            // Filtro 1 - Tipo de búsqueda principal
            Filtro1.Items.AddRange(new object[]
            {
                "TODOS",
                "POR NOMBRE",
                "POR DEPARTAMENTO",
                "POR PUESTO",
                "POR DPI",
                "POR SEDE"
            });
            Filtro1.SelectedIndex = 0;

            // Filtro 2 - Estado del empleado
            var estados = Ctrl_Employees.ObtenerEstadosEmpleado();
            var listaEstados = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, "TODOS")
            };
            listaEstados.AddRange(estados);

            Filtro2.DataSource = new BindingSource(listaEstados, null);
            Filtro2.DisplayMember = "Value";
            Filtro2.ValueMember = "Key";
            Filtro2.SelectedIndex = 0;

            // Filtro 3 - Supervisor
            Filtro3.Items.AddRange(new object[]
            {
                "TODOS",
                "CON SUPERVISOR",
                "SIN SUPERVISOR"
            });
            Filtro3.SelectedIndex = 0;
        }
        #endregion Filtros
        #region Search
        // Evento Click del botón Buscar
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string valorBusqueda = "";
                if (Txt_ValorBuscado.Text != "BUSCAR POR NOMBRE, DEPARTAMENTO, DPI..." &&
                    !string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                {
                    valorBusqueda = Txt_ValorBuscado.Text.Trim();
                }

                string tipoFiltro = Filtro1.SelectedItem?.ToString() ?? "TODOS";

                int? estadoId = null;
                if (Filtro2.SelectedValue != null)
                {
                    if (int.TryParse(Filtro2.SelectedValue.ToString(), out int estadoTmp) && estadoTmp > 0)
                    {
                        estadoId = estadoTmp;
                    }
                }

                string filtroSupervisor = Filtro3.SelectedItem?.ToString() ?? "TODOS";

                int? departmentId = null;
                int? positionId = null;
                string textoBusqueda = "";
                _ultimoLocationId = null;

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
                            !string.IsNullOrWhiteSpace(d.Value) &&
                            d.Value.ToUpper().Contains(valorBusqueda.ToUpper()));
                        if (dept.Key > 0)
                            departmentId = dept.Key;
                        break;

                    case "POR PUESTO":
                        var puestos = Ctrl_Employees.ObtenerPuestos();
                        var puesto = puestos.FirstOrDefault(p =>
                            !string.IsNullOrWhiteSpace(p.Value) &&
                            p.Value.ToUpper().Contains(valorBusqueda.ToUpper()));
                        if (puesto.Key > 0)
                            positionId = puesto.Key;
                        break;

                    case "POR DPI":
                        textoBusqueda = valorBusqueda;
                        break;

                    case "POR SEDE":
                        _ultimoLocationId = null;
                        var sedesFiltro = Ctrl_Locations.ObtenerLocationsActivas();
                        var sedeEncontrada = sedesFiltro.FirstOrDefault(s =>
                            !string.IsNullOrWhiteSpace(s.Value) &&
                            s.Value.ToUpper().Contains(valorBusqueda.ToUpper()));
                        if (sedeEncontrada.Key > 0)
                            _ultimoLocationId = sedeEncontrada.Key;
                        break;
                }

                _ultimoTextoBusqueda = textoBusqueda;
                _ultimoDepartmentId = departmentId;
                _ultimoPositionId = positionId;
                _ultimoEmployeeStatusId = estadoId;
                _ultimoFiltroSupervisor = filtroSupervisor;

                paginaActual = 1;

                if (filtroSupervisor != "TODOS" || _ultimoLocationId.HasValue)
                {
                    List<Mdl_Employees> todosLosResultados = Ctrl_Employees.BuscarEmpleados(
                        textoBusqueda: textoBusqueda,
                        departmentId: departmentId,
                        positionId: positionId,
                        employeeStatusId: estadoId,
                        pageNumber: 1,
                        pageSize: int.MaxValue
                    );

                    if (filtroSupervisor == "CON SUPERVISOR")
                    {
                        todosLosResultados = todosLosResultados.Where(emp => emp.DirectSupervisorId.HasValue).ToList();
                    }
                    else if (filtroSupervisor == "SIN SUPERVISOR")
                    {
                        todosLosResultados = todosLosResultados.Where(emp => !emp.DirectSupervisorId.HasValue).ToList();
                    }

                    if (_ultimoLocationId.HasValue)
                    {
                        todosLosResultados = todosLosResultados.Where(emp => emp.LocationId == _ultimoLocationId).ToList();
                    }

                    _listaCompletaFiltrada = todosLosResultados;

                    int skip = (paginaActual - 1) * registrosPorPagina;
                    empleadosList = todosLosResultados.Skip(skip).Take(registrosPorPagina).ToList();

                    totalRegistros = todosLosResultados.Count;
                }
                else
                {
                    _listaCompletaFiltrada = null;

                    List<Mdl_Employees> resultados = Ctrl_Employees.BuscarEmpleados(
                        textoBusqueda: textoBusqueda,
                        departmentId: departmentId,
                        positionId: positionId,
                        employeeStatusId: estadoId,
                        pageNumber: paginaActual,
                        pageSize: registrosPorPagina
                    );

                    empleadosList = resultados;

                    totalRegistros = Ctrl_Employees.ContarTotalEmpleados(
                        textoBusqueda: textoBusqueda,
                        departmentId: departmentId,
                        positionId: positionId,
                        employeeStatusId: estadoId
                    );
                }

                ConfigurarTabla();
                ReferenciasLlavesForaneas();
                AjustarColumnas();

                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

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
        // Evento Enter del TextBox de búsqueda
        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evita el "beep" del sistema
                Btn_Search_Click(sender, e); // Ejecuta la búsqueda
            }
        }
        // Evento Click del botón Limpiar Búsqueda
        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            // Limpiar búsqueda
            Txt_ValorBuscado.Text = "BUSCAR POR NOMBRE, DEPARTAMENTO, DPI...";
            Txt_ValorBuscado.ForeColor = Color.Gray;

            // Resetear filtros
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 0;

            // ⭐ LIMPIAR FILTROS GUARDADOS
            _ultimoTextoBusqueda = "";
            _ultimoDepartmentId = null;
            _ultimoPositionId = null;
            _ultimoEmployeeStatusId = null;
            _ultimoLocationId = null; 
            _ultimoFiltroSupervisor = "TODOS";
            _listaCompletaFiltrada = null;   

            // Recargar todos los empleados
            paginaActual = 1;
            RefrescarListado();
            ConfigurarTabla();
            ReferenciasLlavesForaneas();
            AjustarColumnas();
            ActualizarInfoPaginacion();
        }
        #endregion Search
        #region vScrollBar
        private void Panel_Izquierdo_MouseEnter(object sender, EventArgs e)
        {
            Panel_Izquierdo.Focus();
        }
        // Manejar evento Scroll del vScrollBar
        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            int scrollPosition = vScrollBar.Value;

            foreach (Control ctrl in Panel_Izquierdo.Controls)
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

            Panel_Izquierdo.Invalidate();
        }
        // Manejar evento MouseWheel Para Panel_Izquierdo
        private void Panel_Izquierdo_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!vScrollBar.Visible) return;

            int delta = e.Delta / 120;
            int newValue = vScrollBar.Value - (delta * 30); // 30 píxeles por scroll

            if (newValue < 0) newValue = 0;
            if (newValue > vScrollBar.Maximum) newValue = vScrollBar.Maximum;

            vScrollBar.Value = newValue;

            // AGREGAR ESTA LÍNEA - Mover el contenido manualmente
            MoverContenido(newValue);
        }
        // AGREGAR ESTE MÉTODO
        private void MoverContenido(int scrollPosition)
        {
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                {
                    ctrl.Tag = "OrigY:" + ctrl.Top;
                }
                string[] parts = ctrl.Tag.ToString().Split(':');
                int originalY = int.Parse(parts[1]);
                ctrl.Top = originalY - scrollPosition;
            }
            Panel_Izquierdo.Invalidate();
        }
        // Configurar eventos MouseWheel para Panel_Izquierdo y sus controles hijos
        private void ConfigurarEventosScroll()
        {
            Panel_Izquierdo.TabStop = true;
            Panel_Izquierdo.MouseWheel += Panel_Izquierdo_MouseWheel;
            Panel_Izquierdo.MouseEnter += Panel_Izquierdo_MouseEnter;

            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                ctrl.MouseWheel += Panel_Izquierdo_MouseWheel;
            }
        }
        // Inicializar configuración del vScrollBar
        private void InicializarScroll()
        {
            // Primero guardar posiciones originales
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                {
                    ctrl.Tag = "OrigY:" + ctrl.Top;
                }
            }

            // Calcular altura total del contenido
            int maxBottom = 0;
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                maxBottom = Math.Max(maxBottom, ctrl.Bottom);
            }

            int totalContentHeight = maxBottom + (Panel_Izquierdo.Height/3); // Dinamico

            // Si no necesita scroll, ocultar scrollbar
            if (totalContentHeight <= Panel_Izquierdo.Height)
            {
                vScrollBar.Visible = false;
                return;
            }

            // Configurar scrollbar
            vScrollBar.Visible = true;
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = totalContentHeight - Panel_Izquierdo.Height;
            vScrollBar.SmallChange = 30;
            vScrollBar.LargeChange = Panel_Izquierdo.Height / 4;

            vScrollBar.Scroll -= vScrollBar_Scroll;
            vScrollBar.Scroll += vScrollBar_Scroll;
            vScrollBar.Value = 0;
        }
        #endregion vScrollBar
        #region ConfiguracionesTabla
        // Método para cargar empleados desde el controlador
        private void CargarEmpleados()
        {
            try
            {
                RefrescarListado();
                ConfigurarTabla();
                ReferenciasLlavesForaneas();
                AjustarColumnas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar empleados: {ex.Message}");
            }
        }
        // Método para refrescar la tabla
        public void RefrescarListado()
        {
            empleadosList = Ctrl_Employees.MostrarEmpleados(paginaActual, registrosPorPagina);
        }
        // Método para configurar la tabla
        public void ConfigurarTabla()
        {
            // LIMPIAR COLUMNAS EXISTENTES
            Tabla.Columns.Clear();

            // AGREGAR COLUMNAS MANUALMENTE
            Tabla.Columns.Add("EmployeeId", "ID");
            Tabla.Columns.Add("EmployeeCode", "CÓDIGO");
            Tabla.Columns.Add("FullName", "NOMBRE COMPLETO");
            Tabla.Columns.Add("IdentificationNumber", "DPI");
            Tabla.Columns.Add("InstitutionalEmail", "EMAIL INSTITUCIONAL");
            Tabla.Columns.Add("Phone", "TELÉFONO");
            Tabla.Columns.Add("HireDate", "FECHA INGRESO");
            Tabla.Columns.Add("Department", "DEPARTAMENTO");
            Tabla.Columns.Add("Position", "PUESTO");
            Tabla.Columns.Add("Supervisor", "SUPERVISOR");
            Tabla.Columns.Add("Location", "SEDE");
            Tabla.Columns.Add("TipoContratacion", "TIPO CONTRATACIÓN");
            Tabla.Columns.Add("Status", "ESTADO");
            Tabla.Columns.Add("CreatedBy", "CREADO POR");

            // OCULTAR COLUMNA ID
            Tabla.Columns["EmployeeId"].Visible = false;

            // CONFIGURACIÓN DE SELECCIÓN Y FORMATO
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToResizeRows = false;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            // ESTILO DE ENCABEZADOS
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            // ESTILO DE CELDAS
            Tabla.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // ALTURA DE FILA Y BORDES
            Tabla.RowTemplate.Height = 30;
            Tabla.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            // AJUSTAR COLUMNAS
            AjustarColumnas();

            // AGREGAR EVENTO DE SELECCIÓN
            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void ReferenciasLlavesForaneas()
        {
            Tabla.Rows.Clear();

            // Obtener listas de referencia UNA SOLA VEZ para optimizar
            var departamentos = Ctrl_Employees.ObtenerDepartamentos();
            var puestos = Ctrl_Employees.ObtenerPuestos();
            var supervisores = Ctrl_Employees.ObtenerSupervisores();
            var sedes = Ctrl_Locations.ObtenerLocationsActivas();
            var estados = Ctrl_Employees.ObtenerEstadosEmpleado();

            foreach (var emp in empleadosList)
            {
                // OBTENER NOMBRE DEL DEPARTAMENTO
                string departamento = "N/A";
                var dept = departamentos.FirstOrDefault(d => d.Key == emp.DepartmentId);
                if (dept.Key > 0)
                    departamento = dept.Value;

                // OBTENER NOMBRE DEL PUESTO
                string puesto = "N/A";
                var pos = puestos.FirstOrDefault(p => p.Key == emp.PositionId);
                if (pos.Key > 0)
                    puesto = pos.Value;

                // OBTENER NOMBRE DEL SUPERVISOR
                string supervisor = "SIN SUPERVISOR";
                if (emp.DirectSupervisorId.HasValue)
                {
                    var sup = supervisores.FirstOrDefault(s => s.Key == emp.DirectSupervisorId.Value);
                    if (sup.Key > 0)
                        supervisor = sup.Value;
                }

                // OBTENER NOMBRE DE LA SEDE
                string sede = "SIN SEDE";
                if (emp.LocationId.HasValue)
                {
                    var loc = sedes.FirstOrDefault(l => l.Key == emp.LocationId.Value);
                    if (loc.Key > 0)
                        sede = loc.Value;
                }

                // OBTENER NOMBRE DEL ESTADO
                string estado = "N/A";
                var est = estados.FirstOrDefault(e => e.Key == emp.EmployeeStatusId);
                if (est.Key > 0)
                    estado = est.Value;

                // Nombre del usuario que creó el registro (CreatedBy)
                string creadoPor = "N/A";
                if (emp.CreatedBy.HasValue && emp.CreatedBy.Value > 0)
                {
                    string nombreUsuario = Ctrl_Users.ObtenerNombreCompletoPorId(emp.CreatedBy.Value);
                    creadoPor = string.IsNullOrWhiteSpace(nombreUsuario) ? "SIN USUARIO" : nombreUsuario;
                }

                // AGREGAR FILA A LA TABLA
                Tabla.Rows.Add(
                    emp.EmployeeId,                                      // ID (oculto)
                    emp.EmployeeCode ?? "N/A",                          // CÓDIGO
                    emp.FullName ?? "",                                  // NOMBRE COMPLETO
                    emp.IdentificationNumber ?? "",                      // DPI
                    emp.InstitutionalEmail ?? "",                        // EMAIL INSTITUCIONAL
                    emp.Phone ?? "N/A",                                  // TELÉFONO
                    emp.HireDate.ToString("dd/MM/yyyy"),                // FECHA INGRESO
                    departamento,                                        // DEPARTAMENTO (nombre)
                    puesto,                                             // PUESTO (nombre)
                    supervisor,                                         // SUPERVISOR (nombre)
                    sede,                                               // SEDE (nombre)
                    emp.TipoContratacion ?? "N/A",                      // TIPO CONTRATACIÓN
                    estado,                                             // ESTADO (nombre)
                    creadoPor                                           // CREADO POR
                );
            }
        }

        // Método para cargar datos del empleado seleccionado
        private void CargarDatosEmpleadoSeleccionado()
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0) return;

                _cargandoEmpleado = true;

                DataGridViewRow fila = Tabla.SelectedRows[0];
                int employeeId = Convert.ToInt32(fila.Cells["EmployeeId"].Value);

                _empleadoSeleccionado = Ctrl_Employees.ObtenerEmpleadoPorId(employeeId);

                if (_empleadoSeleccionado != null)
                {
                    // ===== CAMPOS OBLIGATORIOS =====
                    if (!string.IsNullOrWhiteSpace(_empleadoSeleccionado.EmployeeCode))
                    {
                        Txt_Codigo.Text = _empleadoSeleccionado.EmployeeCode;
                        Txt_Codigo.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_Codigo.Text = "CODIGO EMPLEADO";
                        Txt_Codigo.ForeColor = Color.Gray;
                    }

                    Txt_Nombres.Text = _empleadoSeleccionado.FirstName;
                    Txt_Nombres.ForeColor = Color.Black;

                    Txt_Apellidos.Text = _empleadoSeleccionado.LastName;
                    Txt_Apellidos.ForeColor = Color.Black;

                    Txt_Dpi.Text = _empleadoSeleccionado.IdentificationNumber;
                    Txt_Dpi.ForeColor = Color.Black;

                    Txt_CorreoInstitucional.Text = _empleadoSeleccionado.InstitutionalEmail;
                    Txt_CorreoInstitucional.ForeColor = Color.Black;

                    // ===== CAMPOS OPCIONALES =====
                    if (!string.IsNullOrWhiteSpace(_empleadoSeleccionado.Email))
                    {
                        Txt_CorreoPersonal.Text = _empleadoSeleccionado.Email;
                        Txt_CorreoPersonal.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_CorreoPersonal.Text = "CORREO PERSONAL";
                        Txt_CorreoPersonal.ForeColor = Color.Gray;
                    }

                    if (!string.IsNullOrWhiteSpace(_empleadoSeleccionado.Phone))
                    {
                        Txt_Telefono1.Text = _empleadoSeleccionado.Phone;
                        Txt_Telefono1.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_Telefono1.Text = "+502";
                        Txt_Telefono1.ForeColor = Color.Gray;
                    }

                    if (!string.IsNullOrWhiteSpace(_empleadoSeleccionado.MobilePhone))
                    {
                        Txt_Telefono2.Text = _empleadoSeleccionado.MobilePhone;
                        Txt_Telefono2.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_Telefono2.Text = "+502";
                        Txt_Telefono2.ForeColor = Color.Gray;
                    }

                    if (!string.IsNullOrWhiteSpace(_empleadoSeleccionado.Address))
                    {
                        Txt_Direccion.Text = _empleadoSeleccionado.Address;
                        Txt_Direccion.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_Direccion.Text = "DIRECCIÓN COMPLETA";
                        Txt_Direccion.ForeColor = Color.Gray;
                    }

                    // ===== CONTACTO DE EMERGENCIA =====
                    if (!string.IsNullOrWhiteSpace(_empleadoSeleccionado.EmergencyContactName))
                    {
                        Txt_PersonaEmergencia.Text = _empleadoSeleccionado.EmergencyContactName;
                        Txt_PersonaEmergencia.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_PersonaEmergencia.Text = "NOMBRE DE CONTACTO";
                        Txt_PersonaEmergencia.ForeColor = Color.Gray;
                    }

                    if (!string.IsNullOrWhiteSpace(_empleadoSeleccionado.EmergencyContactRelation))
                    {
                        Txt_Parentesco.Text = _empleadoSeleccionado.EmergencyContactRelation;
                        Txt_Parentesco.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_Parentesco.Text = "PARENTESCO";
                        Txt_Parentesco.ForeColor = Color.Gray;
                    }

                    if (!string.IsNullOrWhiteSpace(_empleadoSeleccionado.EmergencyContactPhone))
                    {
                        Txt_TelefonoEmergencia.Text = _empleadoSeleccionado.EmergencyContactPhone;
                        Txt_TelefonoEmergencia.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_TelefonoEmergencia.Text = "+502";
                        Txt_TelefonoEmergencia.ForeColor = Color.Gray;
                    }

                    // ===== FECHAS =====
                    if (_empleadoSeleccionado.BirthDate.HasValue)
                        DTP_Nacimiento.Value = _empleadoSeleccionado.BirthDate.Value;
                    else
                        DTP_Nacimiento.Value = new DateTime(2000, 1, 1);

                    DTP_Ingreso.Value = _empleadoSeleccionado.HireDate;

                    // ===== SALARIOS =====
                    Txt_Salario.Text = _empleadoSeleccionado.NominalSalary?.ToString("0.00") ?? "0.00";
                    Txt_salario_base.Text = _empleadoSeleccionado.BaseSalary?.ToString("0.00") ?? "0.00";
                    Txt_bono_adicional.Text = _empleadoSeleccionado.AdditionalBonus?.ToString("0.00") ?? "0.00";
                    Txt_bono_ley.Text = _empleadoSeleccionado.LegalBonus?.ToString("0.00") ?? "250.00";
                    Txt_igss.Text = _empleadoSeleccionado.IGSS?.ToString("0.00") ?? "0.00";
                    Txt_ISR.Text = _empleadoSeleccionado.ISR?.ToString("0.00") ?? "0.00";
                    Txt_salario_neto.Text = _empleadoSeleccionado.NetSalary?.ToString("0.00") ?? "0.00";

                    chkb_IGSS.Checked = _empleadoSeleccionado.IGSSManual == true;
                    Txt_igss.ReadOnly = !chkb_IGSS.Checked;

                    // ===== COMBOBOX =====
                    ComboBox_Departamento.SelectedValue = _empleadoSeleccionado.DepartmentId;
                    ComboBox_Puesto.SelectedValue = _empleadoSeleccionado.PositionId;

                    if (_empleadoSeleccionado.DirectSupervisorId.HasValue)
                        ComboBox_Supervisor.SelectedValue = _empleadoSeleccionado.DirectSupervisorId.Value;
                    else
                        ComboBox_Supervisor.SelectedIndex = 0;

                    if (_empleadoSeleccionado.LocationId.HasValue)
                        ComboBox_Sede.SelectedValue = _empleadoSeleccionado.LocationId.Value;
                    else
                        ComboBox_Sede.SelectedIndex = 0;

                    if (!string.IsNullOrWhiteSpace(_empleadoSeleccionado.TipoContratacion))
                    {
                        int index = ComboBox_TipoContratacion.FindString(_empleadoSeleccionado.TipoContratacion);
                        if (index >= 0)
                            ComboBox_TipoContratacion.SelectedIndex = index;
                        else
                            ComboBox_TipoContratacion.SelectedIndex = 0;
                    }
                    else
                    {
                        ComboBox_TipoContratacion.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del empleado: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoEmpleado = false;
            }
        }
        // Método para ajustar columnas
        public void AjustarColumnas()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["EmployeeCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["EmployeeCode"].FillWeight = 8;

                Tabla.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["FullName"].FillWeight = 20;

                Tabla.Columns["IdentificationNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["IdentificationNumber"].FillWeight = 10;

                Tabla.Columns["InstitutionalEmail"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["InstitutionalEmail"].FillWeight = 20;

                Tabla.Columns["Phone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Phone"].FillWeight = 10;

                Tabla.Columns["HireDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["HireDate"].FillWeight = 10;

                Tabla.Columns["Department"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Department"].FillWeight = 12;

                Tabla.Columns["Position"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Position"].FillWeight = 12;

                Tabla.Columns["Supervisor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Supervisor"].FillWeight = 15;

                Tabla.Columns["Location"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Location"].FillWeight = 10;

                Tabla.Columns["TipoContratacion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["TipoContratacion"].FillWeight = 10;

                Tabla.Columns["Status"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Status"].FillWeight = 10;

                Tabla.Columns["CreatedBy"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CreatedBy"].FillWeight = 15;
            }
        }
        // Evento al cambiar selección en la tabla
        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargandoFormulario)
                return;

            if (Tabla.SelectedRows.Count == 0)
                return;

            CargarDatosEmpleadoSeleccionado();
        }
        #endregion ConfiguracionesTabla
        #region ToolsStrip
        // Método para crear ToolStrip de paginación
        private void CrearToolStripPaginacion()
        {
            toolStripPaginacion = new ToolStrip();
            toolStripPaginacion.Dock = DockStyle.None;
            toolStripPaginacion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            toolStripPaginacion.GripStyle = ToolStripGripStyle.Hidden;
            toolStripPaginacion.BackColor = Color.FromArgb(248, 249, 250);
            toolStripPaginacion.Height = 40;
            toolStripPaginacion.AutoSize = true;
            toolStripPaginacion.Location = new Point(this.Width - 400, 225); // Posición Y ajustada

            // Botón Anterior
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

            // Agregar botones numerados dinámicamente
            ActualizarBotonesNumerados();

            // Botón Siguiente
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
        // Método para actualizar botones numerados
        private void ActualizarBotonesNumerados()
        {
            // Remover botones numerados existentes
            var itemsToRemove = toolStripPaginacion.Items.Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton").ToList();

            foreach (var item in itemsToRemove)
            {
                toolStripPaginacion.Items.Remove(item);
            }

            if (totalPaginas <= 1) return;

            // Calcular rango de páginas a mostrar (3 páginas alrededor de la actual)
            int inicioRango = Math.Max(1, paginaActual - 1);
            int finRango = Math.Min(totalPaginas, paginaActual + 1);

            // Agregar botones numerados antes del botón "Siguiente"
            int posicionInsertar = toolStripPaginacion.Items.IndexOf(btnSiguiente);

            for (int i = inicioRango; i <= finRango; i++)
            {
                ToolStripButton btnPagina = new ToolStripButton();
                btnPagina.Text = i.ToString();
                btnPagina.Tag = "PageButton";
                btnPagina.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnPagina.Margin = new Padding(1);
                btnPagina.Padding = new Padding(6, 4, 6, 4);

                // Estilo según si es la página actual
                if (i == paginaActual)
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
                btnPagina.Click += (s, e) => CambiarPagina(numeroPagina);

                toolStripPaginacion.Items.Insert(posicionInsertar++, btnPagina);
            }
        }
        // Método para cambiar de página
        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginas)
            {
                paginaActual = nuevaPagina;

                // ⭐ SI HAY FILTRO DE SUPERVISOR ACTIVO, USAR LISTA FILTRADA EN MEMORIA
                if (_listaCompletaFiltrada != null)
                {
                    int skip = (paginaActual - 1) * registrosPorPagina;
                    empleadosList = _listaCompletaFiltrada.Skip(skip).Take(registrosPorPagina).ToList();
                    ConfigurarTabla();
                    ReferenciasLlavesForaneas();
                    AjustarColumnas();
                }
                // ⭐ SI HAY OTROS FILTROS ACTIVOS (sin supervisor), USAR BÚSQUEDA DE BD
                else if (!string.IsNullOrEmpty(_ultimoTextoBusqueda) ||
                         _ultimoDepartmentId.HasValue ||
                         _ultimoPositionId.HasValue ||
                         _ultimoEmployeeStatusId.HasValue)
                {
                    empleadosList = Ctrl_Employees.BuscarEmpleados(
                        textoBusqueda: _ultimoTextoBusqueda,
                        departmentId: _ultimoDepartmentId,
                        positionId: _ultimoPositionId,
                        employeeStatusId: _ultimoEmployeeStatusId,
                        pageNumber: paginaActual,
                        pageSize: registrosPorPagina
                    );
                    ConfigurarTabla();
                    ReferenciasLlavesForaneas();
                    AjustarColumnas();
                }
                else
                {
                    // Sin filtros: Cargar normalmente
                    RefrescarListado();
                }

                ConfigurarTabla();
                ReferenciasLlavesForaneas();
                AjustarColumnas();
                ActualizarInfoPaginacion();
            }
        }
        // Método para actualizar información de paginación

        // Método para actualizar información de paginación
        private void ActualizarInfoPaginacion()
        {
            // ⭐ SI NO HAY FILTROS ACTIVOS Y totalRegistros es 0, CALCULAR
            if (string.IsNullOrEmpty(_ultimoTextoBusqueda) &&
            !_ultimoDepartmentId.HasValue &&
            !_ultimoPositionId.HasValue &&
            !_ultimoEmployeeStatusId.HasValue &&
            !_ultimoLocationId.HasValue &&    
            _ultimoFiltroSupervisor == "TODOS" &&
            totalRegistros == 0)
            {
                totalRegistros = Ctrl_Employees.ContarTotalEmpleados();
            }

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;

            ActualizarBotonesNumerados();

            int inicioRango = (paginaActual - 1) * registrosPorPagina + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            if (Lbl_Paginas != null)
            {
                if (totalRegistros == 0)
                {
                    Lbl_Paginas.Text = "NO HAY COLABORADORES PARA MOSTRAR";
                }
                else
                {
                    Lbl_Paginas.Text = $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} COLABORADORES";
                }
            }
        }
        #endregion ToolsStrip
        #region ValidarCamposObligatorios
        // Método para validar campos obligatorios antes de guardar
        private bool ValidarCamposObligatorios()
        {
            // Helper para verificar si el TextBox tiene placeholder
            bool TienePlaceholder(TextBox txt, string placeholder)
            {
                return string.IsNullOrWhiteSpace(txt.Text) || txt.Text == placeholder || txt.ForeColor == Color.Gray;
            }

            // 1. Validar NOMBRES*
            if (TienePlaceholder(Txt_Nombres, "NOMBRES"))
            {
                MessageBox.Show("El campo NOMBRES es obligatorio", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Nombres.Focus();
                return false;
            }

            // 2. Validar APELLIDOS*
            if (TienePlaceholder(Txt_Apellidos, "APELLIDOS"))
            {
                MessageBox.Show("El campo APELLIDOS es obligatorio", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Apellidos.Focus();
                return false;
            }

            // 3. Validar DPI*
            if (TienePlaceholder(Txt_Dpi, "IDENTIFICACION"))
            {
                MessageBox.Show("El campo DPI es obligatorio", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Dpi.Focus();
                return false;
            }

            // Validar formato DPI (13 dígitos)
            if (!TienePlaceholder(Txt_Dpi, "IDENTIFICACION") && Txt_Dpi.Text.Length != 13)
            {
                MessageBox.Show("El DPI debe tener exactamente 13 dígitos", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Dpi.Focus();
                return false;
            }

            // 4. Validar CORREO INSTITUCIONAL*
            if (TienePlaceholder(Txt_CorreoInstitucional, "CORREO INSTITUCIONAL"))
            {
                MessageBox.Show("El campo CORREO INSTITUCIONAL es obligatorio", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_CorreoInstitucional.Focus();
                return false;
            }

            // Validar formato de email
            if (!TienePlaceholder(Txt_CorreoInstitucional, "CORREO INSTITUCIONAL") &&
                !Txt_CorreoInstitucional.Text.Contains("@"))
            {
                MessageBox.Show("El CORREO INSTITUCIONAL debe tener un formato válido", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_CorreoInstitucional.Focus();
                return false;
            }

            // 5. Validar TELÉFONO*
            if (TienePlaceholder(Txt_Telefono1, "+502"))
            {
                MessageBox.Show("El campo TELÉFONO es obligatorio", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Telefono1.Focus();
                return false;
            }

            // 6. Validar DEPARTAMENTO*
            if (ComboBox_Departamento.SelectedIndex < 0 || ComboBox_Departamento.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un DEPARTAMENTO", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Departamento.Focus();
                return false;
            }

            // 7. Validar PUESTO*
            if (ComboBox_Puesto.SelectedIndex < 0 || ComboBox_Puesto.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un PUESTO", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Puesto.Focus();
                return false;
            }

            // Nota: Fecha de Nacimiento y Fecha de Ingreso siempre tienen valor por defecto en DateTimePicker

            return true;
        }
        #endregion ValidarCamposObligatorios
        #region AsignacionFocus
        // Método para configurar TabIndex y Focus inicial
        private void ConfigurarTabIndexYFocus()
        {
            // ===== SECCIÓN DE BÚSQUEDA =====
            Txt_ValorBuscado.TabIndex = 0;
            Filtro1.TabIndex = 1;
            Filtro2.TabIndex = 2;
            Filtro3.TabIndex = 3;
            // Btn_Buscar.TabIndex = 4; (si tienes botón de búsqueda)

            // ===== DATOS PERSONALES =====
            Txt_Codigo.TabIndex = 5;
            Txt_Nombres.TabIndex = 6;
            Txt_Apellidos.TabIndex = 7;
            Txt_Dpi.TabIndex = 8;
            DTP_Nacimiento.TabIndex = 9;

            // ===== CONTACTO =====
            Txt_CorreoInstitucional.TabIndex = 10;
            Txt_CorreoPersonal.TabIndex = 11;
            Txt_Telefono1.TabIndex = 12;
            Txt_Telefono2.TabIndex = 13;
            Txt_Direccion.TabIndex = 14;

            // ===== INFORMACIÓN LABORAL =====
            DTP_Ingreso.TabIndex = 15;
            ComboBox_Departamento.TabIndex = 16;
            ComboBox_Puesto.TabIndex = 17;
            ComboBox_Supervisor.TabIndex = 18;

            // ===== CONTACTO DE EMERGENCIA =====
            Txt_PersonaEmergencia.TabIndex = 19;
            Txt_Parentesco.TabIndex = 20;
            Txt_TelefonoEmergencia.TabIndex = 21;

            // ===== BOTONES =====
            Btn_Save.TabIndex = 22;
            Btn_Update.TabIndex = 23;
            Btn_Inactive.TabIndex = 24;

            // ===== DATAGRINDVIEW =====
            // DGV_Empleados.TabIndex = 27; (si quieres que sea accesible con Tab)

            // Establecer el foco inicial en el campo de búsqueda
            Txt_ValorBuscado.Focus();
        }

        // Método para establecer el foco en el primer campo al hacer "Nuevo"
        private void EstablecerFocoPrimerCampo()
        {
            Txt_Codigo.Focus();
        }

        // Método para establecer el foco en búsqueda
        private void EstablecerFocoBusqueda()
        {
            Txt_ValorBuscado.Focus();
        }
        #endregion AsignacionFocus
        #region MetodosAuxiliares
        // Habilita o deshabilita los controles de edición
        private void HabilitarControlesEdicion(bool habilitar, bool deshabilitar)
        {
            Txt_Apellidos.Enabled = habilitar;
            Txt_Codigo.Enabled = deshabilitar;
            Txt_CorreoInstitucional.Enabled = habilitar;
            Txt_CorreoPersonal.Enabled = habilitar;
            Txt_Direccion.Enabled = habilitar;
            Txt_Dpi.Enabled = habilitar;
            Txt_Nombres.Enabled = habilitar;
            Txt_Parentesco.Enabled = habilitar;
            Txt_PersonaEmergencia.Enabled = habilitar;
            Txt_Salario.Enabled = habilitar;
            Txt_Telefono1.Enabled = habilitar;
            Txt_Telefono2.Enabled = habilitar;
            Txt_TelefonoEmergencia.Enabled = habilitar;
            Txt_ValorBuscado.Enabled = habilitar;
        }
        #endregion MetodosAuxiliares
        #region CRUD
        // Método para convertir todos los campos de texto a MAYÚSCULAS
        private Mdl_Employees ConvertirEmpleadoAMayusculas(Mdl_Employees empleado)
        {
            if (empleado == null) return null;

            empleado.EmployeeCode = empleado.EmployeeCode?.ToUpper();
            empleado.FirstName = empleado.FirstName?.ToUpper();
            empleado.LastName = empleado.LastName?.ToUpper();
            empleado.IdentificationNumber = empleado.IdentificationNumber?.ToUpper();
            empleado.Email = empleado.Email?.ToUpper();
            empleado.InstitutionalEmail = empleado.InstitutionalEmail?.ToUpper();
            empleado.Phone = empleado.Phone?.ToUpper();
            empleado.MobilePhone = empleado.MobilePhone?.ToUpper();
            empleado.Address = empleado.Address?.ToUpper();
            empleado.EmergencyContactName = empleado.EmergencyContactName?.ToUpper();
            empleado.EmergencyContactPhone = empleado.EmergencyContactPhone?.ToUpper();
            empleado.EmergencyContactRelation = empleado.EmergencyContactRelation?.ToUpper();

            return empleado;
        }
        // Evento para botón Guardar
        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                // ASEGURAR QUE LOS VALORES ESTÉN ACTUALIZADOS ANTES DE GUARDAR
                CalcularSalarioBase();

                if (!chkb_IGSS.Checked)
                    CalcularIGSS();

                CalcularSalarioNeto();

                if (!ValidarCamposObligatorios())
                    return;

                var confirmacion = MessageBox.Show(
                    "¿Está seguro que desea registrar este empleado?",
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

                var nuevoEmpleado = new Mdl_Employees
                {
                    FirstName = Txt_Nombres.Text.Trim(),
                    LastName = Txt_Apellidos.Text.Trim(),
                    IdentificationNumber = Txt_Dpi.Text.Trim(),
                    InstitutionalEmail = Txt_CorreoInstitucional.Text.Trim(),
                    Phone = Txt_Telefono1.Text.Trim(),
                    BirthDate = DTP_Nacimiento.Value,
                    HireDate = DTP_Ingreso.Value,
                    DepartmentId = (int)ComboBox_Departamento.SelectedValue,
                    PositionId = (int)ComboBox_Puesto.SelectedValue,

                    EmployeeCode = !TienePlaceholder(Txt_Codigo, "CODIGO EMPLEADO")
                        ? Txt_Codigo.Text.Trim() : null,
                    Email = !TienePlaceholder(Txt_CorreoPersonal, "CORREO PERSONAL")
                        ? Txt_CorreoPersonal.Text.Trim() : null,
                    MobilePhone = !TienePlaceholder(Txt_Telefono2, "+502")
                        ? Txt_Telefono2.Text.Trim() : null,
                    Address = !TienePlaceholder(Txt_Direccion, "DIRECCIÓN COMPLETA")
                        ? Txt_Direccion.Text.Trim() : null,
                    EmergencyContactName = !TienePlaceholder(Txt_PersonaEmergencia, "NOMBRE DE CONTACTO")
                        ? Txt_PersonaEmergencia.Text.Trim() : null,
                    EmergencyContactRelation = !TienePlaceholder(Txt_Parentesco, "PARENTESCO")
                        ? Txt_Parentesco.Text.Trim() : null,
                    EmergencyContactPhone = !TienePlaceholder(Txt_TelefonoEmergencia, "+502")
                        ? Txt_TelefonoEmergencia.Text.Trim() : null,
                    DirectSupervisorId = ComboBox_Supervisor.SelectedIndex > 0
                        ? (int?)ComboBox_Supervisor.SelectedValue : null,
                    LocationId = ComboBox_Sede.SelectedIndex > 0
                        ? (int?)ComboBox_Sede.SelectedValue : null,
                    TipoContratacion = ComboBox_TipoContratacion.SelectedIndex > 0
                        ? ComboBox_TipoContratacion.SelectedItem.ToString() : null,

                    // NUEVOS CAMPOS DE SALARIO
                    NominalSalary = ConvertirDecimal(Txt_Salario.Text),
                    BaseSalary = ConvertirDecimal(Txt_salario_base.Text),
                    AdditionalBonus = ConvertirDecimal(Txt_bono_adicional.Text),
                    LegalBonus = ConvertirDecimal(Txt_bono_ley.Text),
                    IGSS = ConvertirDecimal(Txt_igss.Text),
                    ISR = ConvertirDecimal(Txt_ISR.Text),
                    NetSalary = ConvertirDecimal(Txt_salario_neto.Text),
                    IGSSManual = chkb_IGSS.Checked,

                    EmployeeStatusId = 1,
                    IsActive = true,
                    CreatedBy = UserData?.UserId ?? 1
                };

                nuevoEmpleado = ConvertirEmpleadoAMayusculas(nuevoEmpleado);

                int resultado = Ctrl_Employees.RegistrarEmpleado(nuevoEmpleado);

                if (resultado > 0)
                {
                    MessageBox.Show("Empleado registrado exitosamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ConfigurarTabla();
                    ReferenciasLlavesForaneas();
                    ActualizarInfoPaginacion();
                    CargarProximoCodigo();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el empleado", "Error",
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
                if (_empleadoSeleccionado == null || _empleadoSeleccionado.EmployeeId == 0)
                {
                    MessageBox.Show("Debe seleccionar un empleado para actualizar", "Validación",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ASEGURAR QUE LOS CÁLCULOS ESTÉN ACTUALIZADOS
                CalcularSalarioBase();

                if (!chkb_IGSS.Checked)
                    CalcularIGSS();

                CalcularSalarioNeto();

                if (!ValidarCamposObligatorios())
                    return;

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea actualizar los datos de {_empleadoSeleccionado.FirstName} {_empleadoSeleccionado.LastName}?",
                    "Confirmar Actualización",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                bool TienePlaceholder(TextBox txt, string placeholder)
                {
                    return string.IsNullOrWhiteSpace(txt.Text) || txt.Text == placeholder || txt.ForeColor == Color.Gray;
                }

                _empleadoSeleccionado.FirstName = Txt_Nombres.Text.Trim();
                _empleadoSeleccionado.LastName = Txt_Apellidos.Text.Trim();
                _empleadoSeleccionado.IdentificationNumber = Txt_Dpi.Text.Trim();
                _empleadoSeleccionado.InstitutionalEmail = Txt_CorreoInstitucional.Text.Trim();
                _empleadoSeleccionado.Phone = Txt_Telefono1.Text.Trim();
                _empleadoSeleccionado.BirthDate = DTP_Nacimiento.Value;
                _empleadoSeleccionado.HireDate = DTP_Ingreso.Value;
                _empleadoSeleccionado.DepartmentId = (int)ComboBox_Departamento.SelectedValue;
                _empleadoSeleccionado.PositionId = (int)ComboBox_Puesto.SelectedValue;

                _empleadoSeleccionado.EmployeeCode = !TienePlaceholder(Txt_Codigo, "CODIGO EMPLEADO")
                    ? Txt_Codigo.Text.Trim() : null;
                _empleadoSeleccionado.Email = !TienePlaceholder(Txt_CorreoPersonal, "CORREO PERSONAL")
                    ? Txt_CorreoPersonal.Text.Trim() : null;
                _empleadoSeleccionado.MobilePhone = !TienePlaceholder(Txt_Telefono2, "+502")
                    ? Txt_Telefono2.Text.Trim() : null;
                _empleadoSeleccionado.Address = !TienePlaceholder(Txt_Direccion, "DIRECCIÓN COMPLETA")
                    ? Txt_Direccion.Text.Trim() : null;
                _empleadoSeleccionado.EmergencyContactName = !TienePlaceholder(Txt_PersonaEmergencia, "NOMBRE DE CONTACTO")
                    ? Txt_PersonaEmergencia.Text.Trim() : null;
                _empleadoSeleccionado.EmergencyContactRelation = !TienePlaceholder(Txt_Parentesco, "PARENTESCO")
                    ? Txt_Parentesco.Text.Trim() : null;
                _empleadoSeleccionado.EmergencyContactPhone = !TienePlaceholder(Txt_TelefonoEmergencia, "+502")
                    ? Txt_TelefonoEmergencia.Text.Trim() : null;
                _empleadoSeleccionado.DirectSupervisorId = ComboBox_Supervisor.SelectedIndex > 0
                    ? (int?)ComboBox_Supervisor.SelectedValue : null;
                _empleadoSeleccionado.LocationId = ComboBox_Sede.SelectedIndex > 0
                    ? (int?)ComboBox_Sede.SelectedValue : null;
                _empleadoSeleccionado.TipoContratacion = ComboBox_TipoContratacion.SelectedIndex > 0
                    ? ComboBox_TipoContratacion.SelectedItem.ToString() : null;

                // NUEVOS CAMPOS DE SALARIO
                _empleadoSeleccionado.NominalSalary = ConvertirDecimal(Txt_Salario.Text);
                _empleadoSeleccionado.BaseSalary = ConvertirDecimal(Txt_salario_base.Text);
                _empleadoSeleccionado.AdditionalBonus = ConvertirDecimal(Txt_bono_adicional.Text);
                _empleadoSeleccionado.LegalBonus = ConvertirDecimal(Txt_bono_ley.Text);
                _empleadoSeleccionado.IGSS = ConvertirDecimal(Txt_igss.Text);
                _empleadoSeleccionado.ISR = ConvertirDecimal(Txt_ISR.Text);
                _empleadoSeleccionado.NetSalary = ConvertirDecimal(Txt_salario_neto.Text);
                _empleadoSeleccionado.IGSSManual = chkb_IGSS.Checked;

                _empleadoSeleccionado.ModifiedBy = UserData?.UserId ?? 1;

                _empleadoSeleccionado = ConvertirEmpleadoAMayusculas(_empleadoSeleccionado);

                int resultado = Ctrl_Employees.ActualizarEmpleado(_empleadoSeleccionado);

                if (resultado > 0)
                {
                    MessageBox.Show("Empleado actualizado exitosamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ConfigurarTabla();
                    ReferenciasLlavesForaneas();
                    ActualizarInfoPaginacion();
                    CargarProximoCodigo();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el empleado", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Evento para botón Inactivar
        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            try
            {
                if (_empleadoSeleccionado == null || _empleadoSeleccionado.EmployeeId == 0)
                {
                    MessageBox.Show("Debe seleccionar un empleado para inactivar", "Validación",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!_empleadoSeleccionado.IsActive)
                {
                    MessageBox.Show("Este empleado ya se encuentra inactivo", "Información",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea INACTIVAR a {_empleadoSeleccionado.FirstName} {_empleadoSeleccionado.LastName}?\n\n" +
                    "El empleado no aparecerá en las listas activas pero sus datos se conservarán.",
                    "Confirmar Inactivación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                int modifiedBy = UserData?.UserId ?? 1; // ⭐ USAR ID DEL USUARIO LOGUEADO
                int resultado = Ctrl_Employees.InactivarEmpleado(_empleadoSeleccionado.EmployeeId, modifiedBy);

                if (resultado > 0)
                {
                    MessageBox.Show("Empleado inactivado exitosamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ConfigurarTabla();           
                    ReferenciasLlavesForaneas(); 
                    ActualizarInfoPaginacion();
                }
                else
                {
                    MessageBox.Show("No se pudo inactivar el empleado", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inactivar: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion CRUD
        #region Limpieza
        // Método para limpiar el formulario
        private void LimpiarFormulario()
        {
            try
            {
                _cargandoEmpleado = true;

                // Activar/Desactivar controles
                HabilitarControlesEdicion(true, false);

                // Limpiar empleado seleccionado
                _empleadoSeleccionado = null;

                // Restaurar TODOS los placeholders
                ConfigurarPlaceHoldersTextbox();

                // Reiniciar campos de salario
                Txt_Salario.Text = "0.00";
                Txt_salario_base.Text = "0.00";
                Txt_bono_adicional.Text = "0.00";
                Txt_bono_ley.Text = "250.00";
                Txt_igss.Text = "0.00";
                Txt_ISR.Text = "0.00";
                Txt_salario_neto.Text = "0.00";

                // Reiniciar modo IGSS
                chkb_IGSS.Checked = false;
                Txt_igss.ReadOnly = true;

                // Reiniciar fechas
                DTP_Nacimiento.Value = new DateTime(2000, 1, 1);
                DTP_Ingreso.Value = DateTime.Today;

                // Reiniciar ComboBoxes
                if (ComboBox_Departamento.Items.Count > 0)
                    ComboBox_Departamento.SelectedIndex = 0;

                if (ComboBox_Puesto.Items.Count > 0)
                    ComboBox_Puesto.SelectedIndex = 0;

                if (ComboBox_Supervisor.Items.Count > 0)
                    ComboBox_Supervisor.SelectedIndex = 0;

                if (ComboBox_Sede.Items.Count > 0)
                    ComboBox_Sede.SelectedIndex = 0;

                if (ComboBox_TipoContratacion.Items.Count > 0)
                    ComboBox_TipoContratacion.SelectedIndex = 0;
            }
            finally
            {
                _cargandoEmpleado = false;
            }

            // Focus en primer campo
            Txt_Codigo.Focus();

            // Cargar próximo código
            CargarProximoCodigo();
        }

        // Evento para botón Limpiar
        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        #endregion Limpieza
        #region ExportarExcel
        // Evento para botón Exportar a Excel
        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                // ⭐ OBTENER REGISTROS SEGÚN FILTROS ACTIVOS
                List<Mdl_Employees> todosLosEmpleados;

                // ⭐ SI HAY FILTROS ACTIVOS, USAR LA LISTA FILTRADA
                if (_listaCompletaFiltrada != null)
                {
                    // Ya tenemos la lista filtrada en memoria (con filtro supervisor)
                    todosLosEmpleados = _listaCompletaFiltrada;
                }
                else if (!string.IsNullOrEmpty(_ultimoTextoBusqueda) ||
                         _ultimoDepartmentId.HasValue ||
                         _ultimoPositionId.HasValue ||
                         _ultimoEmployeeStatusId.HasValue)
                {
                    // Hay filtros de BD activos (búsqueda, departamento, puesto o estado)
                    todosLosEmpleados = Ctrl_Employees.BuscarEmpleados(
                        textoBusqueda: _ultimoTextoBusqueda,
                        departmentId: _ultimoDepartmentId,
                        positionId: _ultimoPositionId,
                        employeeStatusId: _ultimoEmployeeStatusId,
                        pageNumber: 1,
                        pageSize: int.MaxValue
                    );
                }
                else
                {
                    // Sin filtros: exportar todos los empleados
                    todosLosEmpleados = Ctrl_Employees.BuscarEmpleados(
                        pageNumber: 1,
                        pageSize: int.MaxValue
                    );
                }

                if (todosLosEmpleados == null || todosLosEmpleados.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar", "Información",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Diálogo para guardar archivo
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Lista de Empleados",
                    FileName = $"Empleados_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Crear aplicación Excel
                    var excelApp = new Excel.Application();
                    var workbook = excelApp.Workbooks.Add();
                    var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Empleados";

                    // ============ ENCABEZADO PRINCIPAL ============
                    worksheet.Cells[1, 1] = "REPORTE COMPLETO DE EMPLEADOS";
                    worksheet.Range["A1:U1"].Merge();
                    worksheet.Range["A1:U1"].Font.Size = 16;
                    worksheet.Range["A1:U1"].Font.Bold = true;
                    worksheet.Range["A1:U1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Range["A1:U1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(238, 143, 109));
                    worksheet.Range["A1:U1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                    // ============ INFORMACIÓN DEL REPORTE ============
                    worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SISTEMA"}";
                    worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {todosLosEmpleados.Count}";

                    worksheet.Range["A2:A4"].Font.Size = 10;
                    worksheet.Range["A2:A4"].Font.Bold = true;

                    // ============ ENCABEZADOS DE COLUMNAS (TODAS) ============
                    int headerRow = 6;
                    string[] headers = {
                "CÓDIGO",                    // 1
                "NOMBRES",                   // 2
                "APELLIDOS",                 // 3
                "NOMBRE COMPLETO",           // 4
                "DPI",                       // 5
                "EMAIL INSTITUCIONAL",       // 6
                "EMAIL PERSONAL",            // 7
                "TELÉFONO 1",                // 8
                "TELÉFONO 2",                // 9
                "DIRECCIÓN",                 // 10
                "FECHA NACIMIENTO",          // 11
                "FECHA INGRESO",             // 12
                "FECHA BAJA",                // 13
                "DEPARTAMENTO",              // 14
                "PUESTO",                    // 15
                "SUPERVISOR",                // 16
                "SEDE",                      // 17
                "TIPO CONTRATACIÓN",         // 18
                "ESTADO",                    // 19
                "CONTACTO EMERGENCIA",       // 20
                "TELÉFONO EMERGENCIA",       // 21
                "PARENTESCO EMERGENCIA"      // 22
            };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[headerRow, i + 1] = headers[i];
                    }

                    // Estilo de encabezados
                    var headerRange = worksheet.Range[$"A{headerRow}:V{headerRow}"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Size = 11;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    // ============ OBTENER LISTAS DE REFERENCIA ============
                    var departamentos = Ctrl_Employees.ObtenerDepartamentos();
                    var puestos = Ctrl_Employees.ObtenerPuestos();
                    var supervisores = Ctrl_Employees.ObtenerSupervisores();
                    var sedes = Ctrl_Locations.ObtenerLocationsActivas();
                    var estados = Ctrl_Employees.ObtenerEstadosEmpleado();

                    // ============ DATOS ============
                    int row = headerRow + 1;
                    foreach (var emp in todosLosEmpleados)
                    {
                        // 1. CÓDIGO
                        worksheet.Cells[row, 1] = emp.EmployeeCode ?? "N/A";

                        // 2. NOMBRES
                        worksheet.Cells[row, 2] = emp.FirstName ?? "";

                        // 3. APELLIDOS
                        worksheet.Cells[row, 3] = emp.LastName ?? "";

                        // 4. NOMBRE COMPLETO
                        worksheet.Cells[row, 4] = emp.FullName ?? "";

                        // 5. DPI
                        worksheet.Cells[row, 5] = emp.IdentificationNumber ?? "";

                        // 6. EMAIL INSTITUCIONAL
                        worksheet.Cells[row, 6] = emp.InstitutionalEmail ?? "";

                        // 7. EMAIL PERSONAL
                        worksheet.Cells[row, 7] = emp.Email ?? "N/A";

                        // 8. TELÉFONO 1
                        worksheet.Cells[row, 8] = emp.Phone ?? "N/A";

                        // 9. TELÉFONO 2
                        worksheet.Cells[row, 9] = emp.MobilePhone ?? "N/A";

                        // 10. DIRECCIÓN
                        worksheet.Cells[row, 10] = emp.Address ?? "N/A";

                        // 11. FECHA NACIMIENTO
                        worksheet.Cells[row, 11] = emp.BirthDate.HasValue
                            ? emp.BirthDate.Value.ToString("dd/MM/yyyy")
                            : "N/A";

                        // 12. FECHA INGRESO
                        worksheet.Cells[row, 12] = emp.HireDate.ToString("dd/MM/yyyy");

                        // 13. FECHA BAJA
                        worksheet.Cells[row, 13] = emp.TerminationDate.HasValue
                            ? emp.TerminationDate.Value.ToString("dd/MM/yyyy")
                            : "N/A";

                        // 14. DEPARTAMENTO (nombre, no ID)
                        var dept = departamentos.FirstOrDefault(d => d.Key == emp.DepartmentId);
                        worksheet.Cells[row, 14] = dept.Value ?? "N/A";

                        // 15. PUESTO (nombre, no ID)
                        var puesto = puestos.FirstOrDefault(p => p.Key == emp.PositionId);
                        worksheet.Cells[row, 15] = puesto.Value ?? "N/A";

                        // 16. SUPERVISOR (nombre, no ID)
                        if (emp.DirectSupervisorId.HasValue)
                        {
                            var supervisor = supervisores.FirstOrDefault(s => s.Key == emp.DirectSupervisorId.Value);
                            worksheet.Cells[row, 16] = supervisor.Value ?? "N/A";
                        }
                        else
                        {
                            worksheet.Cells[row, 16] = "SIN SUPERVISOR";
                        }

                        // 17. SEDE (nombre, no ID)
                        if (emp.LocationId.HasValue)
                        {
                            var sede = sedes.FirstOrDefault(l => l.Key == emp.LocationId.Value);
                            worksheet.Cells[row, 17] = sede.Value ?? "N/A";
                        }
                        else
                        {
                            worksheet.Cells[row, 17] = "SIN SEDE";
                        }

                        // 18. TIPO CONTRATACIÓN
                        worksheet.Cells[row, 18] = emp.TipoContratacion ?? "N/A";

                        // 19. ESTADO (nombre, no ID)
                        var estado = estados.FirstOrDefault(est => est.Key == emp.EmployeeStatusId);  // ⭐ Cambiar 'e' por 'est'
                        worksheet.Cells[row, 19] = estado.Value ?? "N/A";

                        // 20. CONTACTO EMERGENCIA
                        worksheet.Cells[row, 20] = emp.EmergencyContactName ?? "N/A";

                        // 21. TELÉFONO EMERGENCIA
                        worksheet.Cells[row, 21] = emp.EmergencyContactPhone ?? "N/A";

                        // 22. PARENTESCO EMERGENCIA
                        worksheet.Cells[row, 22] = emp.EmergencyContactRelation ?? "N/A";

                        // Alternar color de filas
                        if (row % 2 == 0)
                        {
                            worksheet.Range[$"A{row}:V{row}"].Interior.Color =
                                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                        }

                        row++;
                    }

                    // ============ FORMATO FINAL ============
                    var dataRange = worksheet.Range[$"A{headerRow}:V{row - 1}"];
                    dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                    // Autoajustar columnas
                    worksheet.Columns.AutoFit();

                    // Ajustar ancho específico para columnas largas
                    worksheet.Columns[4].ColumnWidth = 30;  // Nombre Completo
                    worksheet.Columns[6].ColumnWidth = 35;  // Email Institucional
                    worksheet.Columns[10].ColumnWidth = 40; // Dirección
                    worksheet.Columns[14].ColumnWidth = 25; // Departamento
                    worksheet.Columns[15].ColumnWidth = 25; // Puesto

                    // Congelar paneles en encabezado
                    worksheet.Activate();
                    excelApp.ActiveWindow.SplitRow = headerRow;
                    excelApp.ActiveWindow.FreezePanes = true;

                    // ============ PIE DE PÁGINA ============
                    worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
                    worksheet.Range[$"A{row + 1}:V{row + 1}"].Merge();
                    worksheet.Range[$"A{row + 1}:V{row + 1}"].Font.Italic = true;
                    worksheet.Range[$"A{row + 1}:V{row + 1}"].Font.Size = 9;
                    worksheet.Range[$"A{row + 1}:V{row + 1}"].HorizontalAlignment =
                        Excel.XlHAlign.xlHAlignCenter;

                    // Guardar archivo
                    workbook.SaveAs(saveFileDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    // Liberar objetos COM
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                    this.Cursor = Cursors.Default;

                    // Preguntar si desea abrir el archivo
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
        #endregion
    }
}
