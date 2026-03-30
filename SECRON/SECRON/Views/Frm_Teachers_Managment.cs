using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_Teachers_Managment : Form
    {
        #region PropiedadesIniciales
        // Variables Globales para mantener los filtros activos
        private string _ultimoTextoBusqueda = "";
        private string _ultimaEspecializacion = "";
        private string _ultimoFiltroBancario = "TODOS";
        private List<Mdl_Teachers> _listaCompletaFiltrada = null;
        public Mdl_Security_UserInfo UserData { get; set; }
        private Mdl_Teachers _docenteSeleccionado = null;
        private List<Mdl_Teachers> docentesList;
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        private void Frm_Teachers_Managment_Load(object sender, EventArgs e)
        {
            ConfigurarTabIndexYFocus();
            try
            {
                this.Cursor = Cursors.WaitCursor;
                CrearToolStripPaginacion();
                CargarDocentes();
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                              "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ConfigurarPlaceHoldersTextbox();
            ConfigurarMaxLengthTextBox();
            CargarFiltros();
            InicializarScroll();
            ConfigurarEventosScroll();
            ConfigurarComboBoxes();

            // Cargar código de docente automático
            CargarProximoCodigoDocente();
        }

        private void FormularioResize(object sender, EventArgs e)
        {
            if (Tabla != null && Tabla.DataSource != null)
            {
                Tabla.Refresh();
            }
        }

        public Frm_Teachers_Managment()
        {
            InitializeComponent();
            this.Resize += FormularioResize;
            this.Resize += (s, e) => {
                if (toolStripPaginacion != null)
                {
                    toolStripPaginacion.Location = new Point(this.Width - 400, 225);
                }
            };
        }
        #endregion PropiedadesIniciales
        #region ConfigurarTextBox
        // Configura las longitudes máximas permitidas para cada TextBox
        private void ConfigurarMaxLengthTextBox()
        {
            Txt_ValorBuscado.MaxLength = 100;
            Txt_Code.MaxLength = 50;
            Txt_TeacherName.MaxLength = 200;
            Txt_Phone.MaxLength = 12;
            Txt_Email.MaxLength = 100;
            Txt_Dpi.MaxLength = 20;
            Txt_Nit.MaxLength = 20;
            Txt_Address.MaxLength = 300;
            Txt_AcademicTitle.MaxLength = 200;
            Txt_CollegiateNumber.MaxLength = 50;
            Txt_BankAccountNumber.MaxLength = 50;

            // Bloquear código de docente (es automático)
            Txt_Code.Enabled = false;
            Txt_Code.BackColor = Color.FromArgb(240, 240, 240);
            Txt_Code.Cursor = Cursors.No;
        }

        // Configura los textos de ayuda (placeholders) en los TextBox
        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR POR NOMBRE, DPI, TÍTULO...");
            ConfigurarPlaceHolder(Txt_TeacherName, "NOMBRE COMPLETO DEL DOCENTE");
            ConfigurarPlaceHolder(Txt_Phone, "+502");
            ConfigurarPlaceHolder(Txt_Email, "CORREO ELECTRÓNICO");
            ConfigurarPlaceHolder(Txt_Dpi, "DOCUMENTO PERSONAL DE IDENTIFICACIÓN");
            ConfigurarPlaceHolder(Txt_Nit, "NÚMERO DE IDENTIFICACIÓN TRIBUTARIA");
            ConfigurarPlaceHolder(Txt_Address, "DIRECCIÓN COMPLETA");
            ConfigurarPlaceHolder(Txt_AcademicTitle, "TÍTULO ACADÉMICO (EJ: LICENCIADO EN...)");
            ConfigurarPlaceHolder(Txt_CollegiateNumber, "NÚMERO DE COLEGIADO");
            ConfigurarPlaceHolder(Txt_BankAccountNumber, "NÚMERO DE CUENTA BANCARIA");
        }

        // Configura un placeholder individual para un TextBox específico
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
        #region CodigoDocenteAutomatico
        // Obtiene y muestra el próximo código de docente disponible
        private void CargarProximoCodigoDocente()
        {
            try
            {
                string proximoCodigo = Ctrl_Teachers.ObtenerProximoCodigoDocente();
                Txt_Code.Text = proximoCodigo;
                Txt_Code.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de docente: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Txt_Code.Text = "ERROR";
            }
        }
        #endregion CodigoDocenteAutomatico
        #region ConfigurarComboBox
        // Configura los ComboBox para que solo permitan selección (no escritura)
        private void ConfigurarComboBoxes()
        {
            ComboBox_Specialization.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Bank.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_IsCollegiateActive.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_ContractType.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Location.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            CargarEspecializaciones();
            CargarBancos();
            CargarOpcionesColegiadoActivo();
            CargarTiposContrato();
            CargarSedes();
        }

        // Carga las especializaciones disponibles en el ComboBox
        private void CargarEspecializaciones()
        {
            try
            {
                ComboBox_Specialization.Items.Clear();
                ComboBox_Specialization.Items.Add("NO ESPECIFICA");
                ComboBox_Specialization.Items.Add("MATEMÁTICAS");
                ComboBox_Specialization.Items.Add("LENGUA Y LITERATURA");
                ComboBox_Specialization.Items.Add("CIENCIAS NATURALES");
                ComboBox_Specialization.Items.Add("CIENCIAS SOCIALES");
                ComboBox_Specialization.Items.Add("INGLÉS");
                ComboBox_Specialization.Items.Add("COMPUTACIÓN");
                ComboBox_Specialization.Items.Add("EDUCACIÓN FÍSICA");
                ComboBox_Specialization.Items.Add("MÚSICA");
                ComboBox_Specialization.Items.Add("ARTES PLÁSTICAS");
                ComboBox_Specialization.Items.Add("PSICOLOGÍA");
                ComboBox_Specialization.Items.Add("EDUCACIÓN ESPECIAL");
                ComboBox_Specialization.Items.Add("ADMINISTRACIÓN EDUCATIVA");
                ComboBox_Specialization.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar especializaciones: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Carga los bancos disponibles desde la base de datos
        private void CargarBancos()
        {
            try
            {
                ComboBox_Bank.Items.Clear();
                ComboBox_Bank.Items.Add(new KeyValuePair<int?, string>(null, "SIN BANCO ASIGNADO"));

                var bancos = Ctrl_Banks.ObtenerBancosParaCombo();
                foreach (var banco in bancos)
                {
                    ComboBox_Bank.Items.Add(new KeyValuePair<int?, string>(banco.Key, banco.Value));
                }

                ComboBox_Bank.DisplayMember = "Value";
                ComboBox_Bank.ValueMember = "Key";
                ComboBox_Bank.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar bancos: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Carga las sedes/ubicaciones disponibles desde la base de datos
        private void CargarSedes()
        {
            try
            {
                ComboBox_Location.Items.Clear();

                var sedes = Ctrl_Locations.ObtenerLocationsActivas();
                foreach (var sede in sedes)
                {
                    ComboBox_Location.Items.Add(new KeyValuePair<int, string>(sede.Key, sede.Value));
                }

                ComboBox_Location.DisplayMember = "Value";
                ComboBox_Location.ValueMember = "Key";

                if (ComboBox_Location.Items.Count > 0)
                    ComboBox_Location.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar sedes: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Carga las opciones SI/NO para Colegiado Activo
        private void CargarOpcionesColegiadoActivo()
        {
            try
            {
                ComboBox_IsCollegiateActive.Items.Clear();
                ComboBox_IsCollegiateActive.Items.Add(new KeyValuePair<int, string>(0, "NO"));
                ComboBox_IsCollegiateActive.Items.Add(new KeyValuePair<int, string>(1, "SI"));
                ComboBox_IsCollegiateActive.DisplayMember = "Value";
                ComboBox_IsCollegiateActive.ValueMember = "Key";
                ComboBox_IsCollegiateActive.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar opciones de colegiado: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Carga los tipos de contrato disponibles
        private void CargarTiposContrato()
        {
            try
            {
                ComboBox_ContractType.Items.Clear();
                ComboBox_ContractType.Items.Add("SUELDOS");
                ComboBox_ContractType.Items.Add("HONORARIOS PROFESIONALES");
                ComboBox_ContractType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tipos de contrato: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion ConfigurarComboBox
        #region ConfiguracionTabIndexFocus
        // Configura el orden de tabulación entre controles
        private void ConfigurarTabIndexYFocus()
        {
            Txt_TeacherName.TabIndex = 1;
            Txt_Phone.TabIndex = 2;
            Txt_Email.TabIndex = 3;
            Txt_Dpi.TabIndex = 4;
            Txt_Nit.TabIndex = 5;
            Txt_Address.TabIndex = 6;
            Txt_AcademicTitle.TabIndex = 7;
            ComboBox_Specialization.TabIndex = 8;
            ComboBox_IsCollegiateActive.TabIndex = 9;
            Txt_CollegiateNumber.TabIndex = 10;
            Txt_BankAccountNumber.TabIndex = 11;
            ComboBox_Bank.TabIndex = 12;
            ComboBox_Location.TabIndex = 13;
            DTP_HireDate.TabIndex = 14;
            ComboBox_ContractType.TabIndex = 15;
        }
        #endregion ConfiguracionTabIndexFocus
        #region CargarDatos
        // Carga la lista completa de docentes activos desde la base de datos
        private void CargarDocentes()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                docentesList = Ctrl_Teachers.MostrarTodosDocentes();
                _listaCompletaFiltrada = docentesList;
                totalRegistros = docentesList.Count;
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                if (totalPaginas == 0) totalPaginas = 1;
                if (paginaActual > totalPaginas) paginaActual = totalPaginas;

                MostrarPagina(paginaActual);
                ConfigurarDataGridView();
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar docentes: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Muestra una página específica de docentes en el DataGridView
        private void MostrarPagina(int numeroPagina)
        {
            try
            {
                if (_listaCompletaFiltrada == null || _listaCompletaFiltrada.Count == 0)
                {
                    Tabla.DataSource = null;
                    return;
                }

                int skip = (numeroPagina - 1) * registrosPorPagina;
                var paginaActualData = _listaCompletaFiltrada
                    .Skip(skip)
                    .Take(registrosPorPagina)
                    .ToList();

                Tabla.DataSource = null;
                Tabla.DataSource = paginaActualData;
                ConfigurarDataGridView();
                ActualizarInfoPaginacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar página: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Configura las columnas visibles y sus propiedades en el DataGridView
        private void ConfigurarDataGridView()
        {
            try
            {
                if (Tabla.Columns.Count == 0) return;

                Tabla.Columns["TeacherId"].Visible = false;
                Tabla.Columns["UserId"].Visible = false;
                Tabla.Columns["RegisteredByCoordinatorId"].Visible = false;
                Tabla.Columns["CreatedBy"].Visible = false;
                Tabla.Columns["CreatedDate"].Visible = false;
                Tabla.Columns["ModifiedBy"].Visible = false;
                Tabla.Columns["ModifiedDate"].Visible = false;
                Tabla.Columns["BankId"].Visible = false;
                Tabla.Columns["LocationId"].Visible = false;

                Tabla.Columns["TeacherCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["TeacherCode"].Width = 80;
                Tabla.Columns["TeacherCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                Tabla.Columns["FullName"].HeaderText = "NOMBRE COMPLETO";
                Tabla.Columns["FullName"].Width = 250;

                Tabla.Columns["Phone"].HeaderText = "TELÉFONO";
                Tabla.Columns["Phone"].Width = 100;

                Tabla.Columns["Email"].HeaderText = "CORREO";
                Tabla.Columns["Email"].Width = 200;

                Tabla.Columns["DPI"].HeaderText = "DPI";
                Tabla.Columns["DPI"].Width = 100;

                Tabla.Columns["NIT"].HeaderText = "NIT";
                Tabla.Columns["NIT"].Width = 100;

                Tabla.Columns["Address"].HeaderText = "DIRECCIÓN";
                Tabla.Columns["Address"].Width = 250;

                Tabla.Columns["AcademicTitle"].HeaderText = "TÍTULO ACADÉMICO";
                Tabla.Columns["AcademicTitle"].Width = 200;

                Tabla.Columns["Specialization"].HeaderText = "ESPECIALIZACIÓN";
                Tabla.Columns["Specialization"].Width = 150;

                Tabla.Columns["IsCollegiateActive"].HeaderText = "COLEGIADO ACTIVO";
                Tabla.Columns["IsCollegiateActive"].Width = 120;

                Tabla.Columns["CollegiateNumber"].HeaderText = "No. COLEGIADO";
                Tabla.Columns["CollegiateNumber"].Width = 100;

                Tabla.Columns["BankAccountNumber"].HeaderText = "No. CUENTA BANCARIA";
                Tabla.Columns["BankAccountNumber"].Width = 150;

                Tabla.Columns["HireDate"].HeaderText = "FECHA CONTRATACIÓN";
                Tabla.Columns["HireDate"].Width = 130;
                Tabla.Columns["HireDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                Tabla.Columns["ContractType"].HeaderText = "TIPO CONTRATO";
                Tabla.Columns["ContractType"].Width = 150;

                Tabla.Columns["IsActive"].HeaderText = "ACTIVO";
                Tabla.Columns["IsActive"].Width = 80;

                Tabla.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                Tabla.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                Tabla.RowTemplate.Height = 30;
                Tabla.AllowUserToAddRows = false;
                Tabla.ReadOnly = true;
                Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                Tabla.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al configurar tabla: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion CargarDatos
        #region ToolStrip
        // Crea la barra de herramientas para la paginación - VERSION CORREGIDA DE SUPPLIERS
        private void CrearToolStripPaginacion()
        {
            toolStripPaginacion = new ToolStrip();
            toolStripPaginacion.Dock = DockStyle.None;
            toolStripPaginacion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            toolStripPaginacion.GripStyle = ToolStripGripStyle.Hidden;
            toolStripPaginacion.BackColor = Color.FromArgb(248, 249, 250);
            toolStripPaginacion.Height = 40;
            toolStripPaginacion.AutoSize = true;
            toolStripPaginacion.Location = new Point(this.Width - 400, 225);

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
            //ActualizarBotonesNumerados();

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

        // Actualiza los botones numerados de paginación - VERSION DE SUPPLIERS
        private void ActualizarBotonesNumerados()
        {
            var itemsToRemove = toolStripPaginacion.Items.Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton").ToList();

            foreach (var item in itemsToRemove)
            {
                toolStripPaginacion.Items.Remove(item);
            }

            if (totalPaginas <= 1) return;

            int inicioRango = Math.Max(1, paginaActual - 1);
            int finRango = Math.Min(totalPaginas, paginaActual + 1);

            int posicionInsertar = toolStripPaginacion.Items.IndexOf(btnSiguiente);

            for (int i = inicioRango; i <= finRango; i++)
            {
                ToolStripButton btnPagina = new ToolStripButton();
                btnPagina.Text = i.ToString();
                btnPagina.Tag = "PageButton";
                btnPagina.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnPagina.Margin = new Padding(1);
                btnPagina.Padding = new Padding(6, 4, 6, 4);

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

        // Cambia a una página específica - VERSION DE SUPPLIERS
        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginas)
            {
                paginaActual = nuevaPagina;
                MostrarPagina(paginaActual);
                ActualizarBotonesNumerados();
                //ActualizarInfoPaginacion();
            }
        }

        // Actualiza la información mostrada de la paginación - USA EL LABEL DEL DISEÑADOR
        private void ActualizarInfoPaginacion()
        {
            // Calcular el rango de registros que se están mostrando
            int inicioRango = (paginaActual - 1) * registrosPorPagina + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            // Actualizar el Label del diseñador (Lbl_Paginas)
            if (Lbl_Paginas != null)
            {
                if (totalRegistros == 0)
                {
                    Lbl_Paginas.Text = "NO HAY DOCENTES PARA MOSTRAR";
                }
                else
                {
                    Lbl_Paginas.Text = $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} DOCENTES";
                }
            }

            // Actualizar estado de los botones de navegación
            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;

            // Actualizar botones numerados
            ActualizarBotonesNumerados();
        }
        #endregion ToolStrip
        #region ScrollBar
        // Inicializa las propiedades del scroll vertical - VERSION DE SUPPLIERS
        private void InicializarScroll()
        {
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                {
                    ctrl.Tag = "OrigY:" + ctrl.Top;
                }
            }

            int maxBottom = 0;
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                maxBottom = Math.Max(maxBottom, ctrl.Bottom);
            }

            int totalContentHeight = maxBottom + (Panel_Izquierdo.Height / 3);

            if (totalContentHeight <= Panel_Izquierdo.Height)
            {
                vScrollBar.Visible = false;
                return;
            }

            vScrollBar.Visible = true;
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = totalContentHeight - Panel_Izquierdo.Height;
            vScrollBar.SmallChange = 30;
            vScrollBar.LargeChange = Panel_Izquierdo.Height / 4;

            vScrollBar.Scroll -= vScrollBar_Scroll;
            vScrollBar.Scroll += vScrollBar_Scroll;
            vScrollBar.Value = 0;
        }

        // Configura los eventos del scroll para sincronizarlo con el panel - VERSION DE SUPPLIERS
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

        // Evento cuando el mouse entra al panel - VERSION DE SUPPLIERS
        private void Panel_Izquierdo_MouseEnter(object sender, EventArgs e)
        {
            Panel_Izquierdo.Focus();
        }

        // Evento que sincroniza el scroll con el panel de controles - VERSION DE SUPPLIERS
        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            MoverContenido(e.NewValue);
        }

        // Evento para permitir scroll con la rueda del mouse - VERSION DE SUPPLIERS
        private void Panel_Izquierdo_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!vScrollBar.Visible) return;

            int delta = e.Delta / 120;
            int newValue = vScrollBar.Value - (delta * 30);

            if (newValue < 0) newValue = 0;
            if (newValue > vScrollBar.Maximum) newValue = vScrollBar.Maximum;

            vScrollBar.Value = newValue;
            MoverContenido(newValue);
        }

        // Mueve el contenido del panel según la posición del scroll - VERSION DE SUPPLIERS
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
        #endregion ScrollBar
        #region Filtros
        // Carga las opciones disponibles en los ComboBox de filtros
        private void CargarFiltros()
        {
            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODAS LAS ESPECIALIZACIONES");
            Filtro1.Items.Add("NO ESPECIFICA");
            Filtro1.Items.Add("MATEMÁTICAS");
            Filtro1.Items.Add("LENGUA Y LITERATURA");
            Filtro1.Items.Add("CIENCIAS NATURALES");
            Filtro1.Items.Add("CIENCIAS SOCIALES");
            Filtro1.Items.Add("INGLÉS");
            Filtro1.Items.Add("COMPUTACIÓN");
            Filtro1.Items.Add("EDUCACIÓN FÍSICA");
            Filtro1.Items.Add("MÚSICA");
            Filtro1.Items.Add("ARTES PLÁSTICAS");
            Filtro1.Items.Add("PSICOLOGÍA");
            Filtro1.Items.Add("EDUCACIÓN ESPECIAL");
            Filtro1.Items.Add("ADMINISTRACIÓN EDUCATIVA");
            Filtro1.SelectedIndex = 0;

            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS");
            Filtro2.Items.Add("CON BANCO");
            Filtro2.Items.Add("SIN BANCO");
            Filtro2.SelectedIndex = 0;

            Filtro3.Items.Clear();
            Filtro3.Items.Add("TODOS");
            Filtro3.Items.Add("COLEGIADOS ACTIVOS");
            Filtro3.Items.Add("NO COLEGIADOS");
            Filtro3.SelectedIndex = 0;

            Filtro1.SelectedIndexChanged += AplicarFiltros;
            Filtro2.SelectedIndexChanged += AplicarFiltros;
            Filtro3.SelectedIndexChanged += AplicarFiltros;
        }

        // Aplica los filtros seleccionados a la lista de docentes
        private void AplicarFiltros(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                _ultimaEspecializacion = Filtro1.SelectedItem?.ToString() ?? "TODAS LAS ESPECIALIZACIONES";
                _ultimoFiltroBancario = Filtro2.SelectedItem?.ToString() ?? "TODOS";
                string filtroColegiadoActual = Filtro3.SelectedItem?.ToString() ?? "TODOS";

                var listaFiltrada = docentesList.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(_ultimoTextoBusqueda))
                {
                    listaFiltrada = listaFiltrada.Where(d =>
                        (d.FullName?.ToUpper().Contains(_ultimoTextoBusqueda.ToUpper()) ?? false) ||
                        (d.DPI?.ToUpper().Contains(_ultimoTextoBusqueda.ToUpper()) ?? false) ||
                        (d.Email?.ToUpper().Contains(_ultimoTextoBusqueda.ToUpper()) ?? false) ||
                        (d.AcademicTitle?.ToUpper().Contains(_ultimoTextoBusqueda.ToUpper()) ?? false)
                    );
                }

                if (_ultimaEspecializacion != "TODAS LAS ESPECIALIZACIONES")
                {
                    listaFiltrada = listaFiltrada.Where(d =>
                        string.Equals(d.Specialization, _ultimaEspecializacion, StringComparison.OrdinalIgnoreCase)
                    );
                }

                if (_ultimoFiltroBancario == "CON BANCO")
                {
                    listaFiltrada = listaFiltrada.Where(d => d.BankId.HasValue && d.BankId.Value > 0);
                }
                else if (_ultimoFiltroBancario == "SIN BANCO")
                {
                    listaFiltrada = listaFiltrada.Where(d => !d.BankId.HasValue || d.BankId.Value == 0);
                }

                if (filtroColegiadoActual == "COLEGIADOS ACTIVOS")
                {
                    listaFiltrada = listaFiltrada.Where(d => d.IsCollegiateActive);
                }
                else if (filtroColegiadoActual == "NO COLEGIADOS")
                {
                    listaFiltrada = listaFiltrada.Where(d => !d.IsCollegiateActive);
                }

                _listaCompletaFiltrada = listaFiltrada.ToList();
                totalRegistros = _listaCompletaFiltrada.Count;
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                if (totalPaginas == 0) totalPaginas = 1;
                paginaActual = 1;

                MostrarPagina(paginaActual);
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al aplicar filtros: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Filtros
        #region Busqueda
        // Evento de búsqueda en tiempo real mientras se escribe
        private void Txt_ValorBuscado_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string textoBusqueda = Txt_ValorBuscado.Text.Trim();

                if (textoBusqueda == "BUSCAR POR NOMBRE, DPI, TÍTULO..." || string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    _ultimoTextoBusqueda = "";
                }
                else
                {
                    _ultimoTextoBusqueda = textoBusqueda;
                }

                AplicarFiltros(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en búsqueda: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Busqueda
        #region EventosDataGridView
        // Evento cuando se hace clic en una celda del DataGridView
        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = Tabla.Rows[e.RowIndex];
                    int teacherId = Convert.ToInt32(row.Cells["TeacherId"].Value);

                    var docente = Ctrl_Teachers.ObtenerDocentePorId(teacherId);

                    if (docente != null)
                    {
                        _docenteSeleccionado = docente;
                        CargarDatosEnFormulario(docente);
                        HabilitarBotonesEdicionEliminacion(true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar docente: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Carga los datos del docente seleccionado en los controles del formulario
        private void CargarDatosEnFormulario(Mdl_Teachers docente)
        {
            try
            {
                Txt_Code.Text = docente.TeacherCode ?? "";
                Txt_TeacherName.Text = docente.FullName ?? "";
                Txt_TeacherName.ForeColor = Color.Black;

                Txt_Phone.Text = docente.Phone ?? "";
                if (!string.IsNullOrWhiteSpace(docente.Phone))
                    Txt_Phone.ForeColor = Color.Black;

                Txt_Email.Text = docente.Email ?? "";
                if (!string.IsNullOrWhiteSpace(docente.Email))
                    Txt_Email.ForeColor = Color.Black;

                Txt_Dpi.Text = docente.DPI ?? "";
                if (!string.IsNullOrWhiteSpace(docente.DPI))
                    Txt_Dpi.ForeColor = Color.Black;

                Txt_Nit.Text = docente.NIT ?? "";
                if (!string.IsNullOrWhiteSpace(docente.NIT))
                    Txt_Nit.ForeColor = Color.Black;

                Txt_Address.Text = docente.Address ?? "";
                if (!string.IsNullOrWhiteSpace(docente.Address))
                    Txt_Address.ForeColor = Color.Black;

                Txt_AcademicTitle.Text = docente.AcademicTitle ?? "";
                if (!string.IsNullOrWhiteSpace(docente.AcademicTitle))
                    Txt_AcademicTitle.ForeColor = Color.Black;

                // Cargar Specialization - CON VALIDACIÓN
                if (ComboBox_Specialization.Items.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(docente.Specialization))
                    {
                        bool encontrado = false;
                        for (int i = 0; i < ComboBox_Specialization.Items.Count; i++)
                        {
                            if (ComboBox_Specialization.Items[i].ToString() == docente.Specialization)
                            {
                                ComboBox_Specialization.SelectedIndex = i;
                                encontrado = true;
                                break;
                            }
                        }
                        if (!encontrado)
                        {
                            ComboBox_Specialization.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        ComboBox_Specialization.SelectedIndex = 0;
                    }
                }

                // Cargar IsCollegiateActive en ComboBox (1=SI, 0=NO) - CON VALIDACIÓN
                if (ComboBox_IsCollegiateActive.Items.Count > 0)
                {
                    ComboBox_IsCollegiateActive.SelectedIndex = docente.IsCollegiateActive ? 1 : 0;
                }

                Txt_CollegiateNumber.Text = docente.CollegiateNumber ?? "";
                if (!string.IsNullOrWhiteSpace(docente.CollegiateNumber))
                    Txt_CollegiateNumber.ForeColor = Color.Black;

                Txt_BankAccountNumber.Text = docente.BankAccountNumber ?? "";
                if (!string.IsNullOrWhiteSpace(docente.BankAccountNumber))
                    Txt_BankAccountNumber.ForeColor = Color.Black;

                // Cargar Bank - CON VALIDACIÓN
                if (ComboBox_Bank.Items.Count > 0)
                {
                    if (docente.BankId.HasValue)
                    {
                        bool encontrado = false;
                        for (int i = 0; i < ComboBox_Bank.Items.Count; i++)
                        {
                            var item = (KeyValuePair<int?, string>)ComboBox_Bank.Items[i];
                            if (item.Key == docente.BankId.Value)
                            {
                                ComboBox_Bank.SelectedIndex = i;
                                encontrado = true;
                                break;
                            }
                        }
                        if (!encontrado)
                        {
                            ComboBox_Bank.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        ComboBox_Bank.SelectedIndex = 0;
                    }
                }

                // Cargar Location - CON VALIDACIÓN
                if (ComboBox_Location.Items.Count > 0)
                {
                    bool encontrado = false;
                    for (int i = 0; i < ComboBox_Location.Items.Count; i++)
                    {
                        var item = (KeyValuePair<int, string>)ComboBox_Location.Items[i];
                        if (item.Key == docente.LocationId)
                        {
                            ComboBox_Location.SelectedIndex = i;
                            encontrado = true;
                            break;
                        }
                    }
                    // Si no se encontró la ubicación, seleccionar el primer elemento
                    if (!encontrado)
                    {
                        ComboBox_Location.SelectedIndex = 0;
                    }
                }

                // Cargar HireDate
                if (docente.HireDate.HasValue)
                {
                    DTP_HireDate.Value = docente.HireDate.Value;
                }
                else
                {
                    DTP_HireDate.Value = DateTime.Now;
                }

                // Cargar ContractType - CON VALIDACIÓN
                if (ComboBox_ContractType.Items.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(docente.ContractType))
                    {
                        bool encontrado = false;
                        for (int i = 0; i < ComboBox_ContractType.Items.Count; i++)
                        {
                            if (ComboBox_ContractType.Items[i].ToString() == docente.ContractType)
                            {
                                ComboBox_ContractType.SelectedIndex = i;
                                encontrado = true;
                                break;
                            }
                        }
                        if (!encontrado)
                        {
                            ComboBox_ContractType.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        ComboBox_ContractType.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos en formulario: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion EventosDataGridView
        #region EventosBotones
        // Evento del botón Guardar - SOLO registra docentes NUEVOS
        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos obligatorios antes de proceder
                if (!ValidarCamposObligatorios())
                    return;

                // Obtener datos del formulario
                var docente = ObtenerDatosDelFormulario();

                // MODO REGISTRO NUEVO únicamente
                docente.CreatedBy = UserData?.UserId;
                //docente.RegisteredByCoordinatorId = UserData?.UserId;
                docente.CreatedDate = DateTime.Now;

                int resultado = Ctrl_Teachers.RegistrarDocente(docente);

                if (resultado > 0)
                {
                    MessageBox.Show("Docente registrado correctamente.",
                                  "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Limpiar y recargar después de registrar
                    LimpiarFormulario();
                    CargarDocentes();
                    CargarProximoCodigoDocente();

                    // Los botones Update e Inactive deben estar deshabilitados porque no hay selección
                    HabilitarBotonesEdicionEliminacion(false);
                    _docenteSeleccionado = null;
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el docente.",
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento del botón Update (Actualizar) - Actualiza DIRECTAMENTE el registro seleccionado con confirmación
        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que haya un docente seleccionado
                if (_docenteSeleccionado == null)
                {
                    MessageBox.Show("Debe seleccionar un docente de la tabla para poder actualizarlo.",
                                  "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar campos obligatorios antes de proceder
                if (!ValidarCamposObligatorios())
                    return;

                // Mensaje de confirmación
                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea actualizar los datos del docente?\n\n" +
                    $"Docente: {_docenteSeleccionado.FullName}\n" +
                    $"Código: {_docenteSeleccionado.TeacherCode}",
                    "Confirmar Actualización",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    // Obtener datos actuales del formulario
                    var docente = ObtenerDatosDelFormulario();

                    // Configurar para actualización
                    docente.TeacherId = _docenteSeleccionado.TeacherId;
                    docente.CreatedBy = _docenteSeleccionado.CreatedBy;
                    docente.CreatedDate = _docenteSeleccionado.CreatedDate;
                    docente.ModifiedBy = UserData?.UserId;
                    docente.ModifiedDate = DateTime.Now;

                    int resultado = Ctrl_Teachers.ActualizarDocente(docente);

                    if (resultado > 0)
                    {
                        MessageBox.Show("Docente actualizado correctamente.",
                                      "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Limpiar y recargar después de actualizar
                        LimpiarFormulario();
                        CargarDocentes();
                        CargarProximoCodigoDocente();

                        // Deshabilitar botones de edición y eliminación
                        HabilitarBotonesEdicionEliminacion(false);
                        _docenteSeleccionado = null;

                        // Habilitar el botón Save para nuevos registros
                        Btn_Save.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar el docente.",
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento del botón Inactive (Eliminar) - Inactiva el registro seleccionado
        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            try
            {
                if (_docenteSeleccionado == null)
                {
                    MessageBox.Show("Debe seleccionar un docente para inactivar.",
                                  "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea inactivar al docente {_docenteSeleccionado.FullName}?\n\n" +
                    "Esta acción marcará el registro como inactivo.",
                    "Confirmar inactivación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    int resultado = Ctrl_Teachers.InactivarDocente(
                        _docenteSeleccionado.TeacherId,
                        UserData?.UserId ?? 0);

                    if (resultado > 0)
                    {
                        MessageBox.Show("Docente inactivado correctamente.",
                                      "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarDocentes();
                        HabilitarBotonesEdicionEliminacion(false);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo inactivar el docente.",
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inactivar: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento del botón Clear (Limpiar) - Limpia todos los controles
        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            _docenteSeleccionado = null;
            HabilitarBotonesEdicionEliminacion(false);
            HabilitarControlesEdicion(true);
            Txt_Code.Enabled = false;
            CargarProximoCodigoDocente();
        }
        #endregion EventosBotones
        #region MetodosAuxiliares
        // Limpia todos los controles del formulario
        private void LimpiarFormulario()
        {
            Txt_Code.Clear();

            ConfigurarPlaceHolder(Txt_TeacherName, "NOMBRE COMPLETO DEL DOCENTE");
            ConfigurarPlaceHolder(Txt_Phone, "+502");
            ConfigurarPlaceHolder(Txt_Email, "CORREO ELECTRÓNICO");
            ConfigurarPlaceHolder(Txt_Dpi, "DOCUMENTO PERSONAL DE IDENTIFICACIÓN");
            ConfigurarPlaceHolder(Txt_Nit, "NÚMERO DE IDENTIFICACIÓN TRIBUTARIA");
            ConfigurarPlaceHolder(Txt_Address, "DIRECCIÓN COMPLETA");
            ConfigurarPlaceHolder(Txt_AcademicTitle, "TÍTULO ACADÉMICO (EJ: LICENCIADO EN...)");
            ConfigurarPlaceHolder(Txt_CollegiateNumber, "NÚMERO DE COLEGIADO");
            ConfigurarPlaceHolder(Txt_BankAccountNumber, "NÚMERO DE CUENTA BANCARIA");

            ComboBox_IsCollegiateActive.SelectedIndex = 0;
            ComboBox_Specialization.SelectedIndex = 0;
            ComboBox_Bank.SelectedIndex = 0;
            ComboBox_ContractType.SelectedIndex = 0;
            if (ComboBox_Location.Items.Count > 0)
                ComboBox_Location.SelectedIndex = 0;
            DTP_HireDate.Value = DateTime.Now;

            _docenteSeleccionado = null;
        }

        // Valida que los campos obligatorios estén completos
        private bool ValidarCamposObligatorios()
        {
            if (string.IsNullOrWhiteSpace(Txt_TeacherName.Text) ||
                Txt_TeacherName.Text == "NOMBRE COMPLETO DEL DOCENTE")
            {
                MessageBox.Show("El nombre completo es obligatorio.",
                              "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_TeacherName.Focus();
                return false;
            }

            if (ComboBox_Location.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar una sede.",
                              "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Location.Focus();
                return false;
            }

            return true;
        }

        // Obtiene los datos del formulario y los empaqueta en un objeto Mdl_Teachers
        private Mdl_Teachers ObtenerDatosDelFormulario()
        {
            var docente = new Mdl_Teachers
            {
                TeacherCode = Txt_Code.Text,
                FullName = ObtenerTextoLimpio(Txt_TeacherName, "NOMBRE COMPLETO DEL DOCENTE"),
                Phone = ObtenerTextoLimpio(Txt_Phone, "+502"),
                Email = ObtenerTextoLimpio(Txt_Email, "CORREO ELECTRÓNICO"),
                DPI = ObtenerTextoLimpio(Txt_Dpi, "DOCUMENTO PERSONAL DE IDENTIFICACIÓN"),
                NIT = ObtenerTextoLimpio(Txt_Nit, "NÚMERO DE IDENTIFICACIÓN TRIBUTARIA"),
                Address = ObtenerTextoLimpio(Txt_Address, "DIRECCIÓN COMPLETA"),
                AcademicTitle = ObtenerTextoLimpio(Txt_AcademicTitle, "TÍTULO ACADÉMICO (EJ: LICENCIADO EN...)"),
                Specialization = ComboBox_Specialization.SelectedItem?.ToString() == "NO ESPECIFICA"
                    ? null
                    : ComboBox_Specialization.SelectedItem?.ToString(),
                CollegiateNumber = ObtenerTextoLimpio(Txt_CollegiateNumber, "NÚMERO DE COLEGIADO"),
                BankAccountNumber = ObtenerTextoLimpio(Txt_BankAccountNumber, "NÚMERO DE CUENTA BANCARIA"),
                ContractType = ComboBox_ContractType.SelectedItem?.ToString(),
                HireDate = DTP_HireDate.Value,
                IsActive = true
            };

            // Obtener IsCollegiateActive del ComboBox (1=SI, 0=NO)
            if (ComboBox_IsCollegiateActive.SelectedItem != null)
            {
                var selectedItem = (KeyValuePair<int, string>)ComboBox_IsCollegiateActive.SelectedItem;
                docente.IsCollegiateActive = selectedItem.Key == 1;
            }

            // Obtener BankId del ComboBox
            if (ComboBox_Bank.SelectedIndex > 0)
            {
                var selectedBank = (KeyValuePair<int?, string>)ComboBox_Bank.SelectedItem;
                docente.BankId = selectedBank.Key;
            }

            // Obtener LocationId del ComboBox seleccionado
            if (ComboBox_Location.SelectedIndex >= 0)
            {
                var selectedLocation = (KeyValuePair<int, string>)ComboBox_Location.SelectedItem;
                docente.LocationId = selectedLocation.Key;
            }

            return docente;
        }

        // Obtiene el texto limpio de un TextBox (sin placeholder)
        private string ObtenerTextoLimpio(TextBox textBox, string placeholder)
        {
            if (textBox.Text == placeholder || string.IsNullOrWhiteSpace(textBox.Text))
                return null;
            return textBox.Text.Trim();
        }

        // Habilita o deshabilita los controles de edición
        private void HabilitarControlesEdicion(bool habilitar)
        {
            Txt_TeacherName.Enabled = habilitar;
            Txt_Phone.Enabled = habilitar;
            Txt_Email.Enabled = habilitar;
            Txt_Dpi.Enabled = habilitar;
            Txt_Nit.Enabled = habilitar;
            Txt_Address.Enabled = habilitar;
            Txt_AcademicTitle.Enabled = habilitar;
            ComboBox_Specialization.Enabled = habilitar;
            ComboBox_IsCollegiateActive.Enabled = habilitar;
            Txt_CollegiateNumber.Enabled = habilitar;
            Txt_BankAccountNumber.Enabled = habilitar;
            ComboBox_Bank.Enabled = habilitar;
            ComboBox_Location.Enabled = habilitar;
            DTP_HireDate.Enabled = habilitar;
            ComboBox_ContractType.Enabled = habilitar;
        }

        // Habilita o deshabilita los botones de edición y eliminación
        private void HabilitarBotonesEdicionEliminacion(bool habilitar)
        {
            Btn_Update.Enabled = habilitar;
            Btn_Inactive.Enabled = habilitar;
        }
        #endregion MetodosAuxiliares
        #region ExportarExcel
        // Exporta la lista FILTRADA de docentes a un archivo Excel (solo los que están en la tabla actualmente)
        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                // CORREGIDO: Exportar solo los docentes que están filtrados actualmente
                var docentesAExportar = _listaCompletaFiltrada;

                if (docentesAExportar == null || docentesAExportar.Count == 0)
                {
                    MessageBox.Show("No hay docentes para exportar.",
                                  "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Lista de Docentes",
                    FileName = $"Docentes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    var excelApp = new Excel.Application();
                    var workbook = excelApp.Workbooks.Add();
                    var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Docentes";

                    // Encabezado principal
                    worksheet.Cells[1, 1] = "REPORTE COMPLETO DE DOCENTES";
                    worksheet.Range["A1:O1"].Merge();
                    worksheet.Range["A1:O1"].Font.Size = 16;
                    worksheet.Range["A1:O1"].Font.Bold = true;
                    worksheet.Range["A1:O1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Range["A1:O1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    worksheet.Range["A1:O1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                    // Información del reporte
                    worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SISTEMA"}";
                    worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {docentesAExportar.Count}";

                    worksheet.Range["A2:A4"].Font.Size = 10;
                    worksheet.Range["A2:A4"].Font.Bold = true;

                    // Encabezados de columnas
                    int headerRow = 6;
                    string[] headers = {
                        "CÓDIGO",
                        "NOMBRE COMPLETO",
                        "TELÉFONO",
                        "EMAIL",
                        "DPI",
                        "NIT",
                        "DIRECCIÓN",
                        "TÍTULO ACADÉMICO",
                        "ESPECIALIZACIÓN",
                        "COLEGIADO ACTIVO",
                        "No. COLEGIADO",
                        "No. CUENTA BANCARIA",
                        "FECHA CONTRATACIÓN",
                        "TIPO CONTRATO",
                        "ACTIVO"
                    };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[headerRow, i + 1] = headers[i];
                    }

                    // Estilo de encabezados
                    var headerRange = worksheet.Range[$"A{headerRow}:O{headerRow}"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Size = 11;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    // Datos
                    int row = headerRow + 1;
                    foreach (var docente in docentesAExportar)
                    {
                        worksheet.Cells[row, 1] = docente.TeacherCode ?? "N/A";
                        worksheet.Cells[row, 2] = docente.FullName ?? "";
                        worksheet.Cells[row, 3] = docente.Phone ?? "N/A";
                        worksheet.Cells[row, 4] = docente.Email ?? "N/A";
                        worksheet.Cells[row, 5] = docente.DPI ?? "N/A";
                        worksheet.Cells[row, 6] = docente.NIT ?? "N/A";
                        worksheet.Cells[row, 7] = docente.Address ?? "N/A";
                        worksheet.Cells[row, 8] = docente.AcademicTitle ?? "N/A";
                        worksheet.Cells[row, 9] = docente.Specialization ?? "NO ESPECIFICA";
                        worksheet.Cells[row, 10] = docente.IsCollegiateActive ? "SÍ" : "NO";
                        worksheet.Cells[row, 11] = docente.CollegiateNumber ?? "N/A";
                        worksheet.Cells[row, 12] = docente.BankAccountNumber ?? "N/A";
                        worksheet.Cells[row, 13] = docente.HireDate.HasValue
                            ? docente.HireDate.Value.ToString("dd/MM/yyyy")
                            : "N/A";
                        worksheet.Cells[row, 14] = docente.ContractType ?? "N/A";
                        worksheet.Cells[row, 15] = docente.IsActive ? "SÍ" : "NO";

                        // Alternar color de filas
                        if (row % 2 == 0)
                        {
                            worksheet.Range[$"A{row}:O{row}"].Interior.Color =
                                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                        }

                        row++;
                    }

                    // Formato final
                    var dataRange = worksheet.Range[$"A{headerRow}:O{row - 1}"];
                    dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                    // Autoajustar columnas
                    worksheet.Columns.AutoFit();

                    // Ajustar ancho específico
                    worksheet.Columns[2].ColumnWidth = 35;  // Nombre Completo
                    worksheet.Columns[4].ColumnWidth = 30;  // Email
                    worksheet.Columns[7].ColumnWidth = 40;  // Dirección
                    worksheet.Columns[8].ColumnWidth = 30;  // Título Académico

                    // Congelar paneles
                    worksheet.Activate();
                    excelApp.ActiveWindow.SplitRow = headerRow;
                    excelApp.ActiveWindow.FreezePanes = true;

                    // Pie de página
                    worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
                    worksheet.Range[$"A{row + 1}:O{row + 1}"].Merge();
                    worksheet.Range[$"A{row + 1}:O{row + 1}"].Font.Italic = true;
                    worksheet.Range[$"A{row + 1}:O{row + 1}"].Font.Size = 9;
                    worksheet.Range[$"A{row + 1}:O{row + 1}"].HorizontalAlignment =
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
        #region EventosFaltantes
        // CORRECCIÓN 5: Evento cuando cambia la selección en el DataGridView
        // Este evento llena automáticamente el formulario con los datos del docente seleccionado
        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                // Validar que la tabla tenga datos y que haya una fila seleccionada
                if (Tabla.DataSource == null || Tabla.SelectedRows.Count == 0)
                {
                    return;
                }

                // Validar que exista la columna TeacherId
                if (!Tabla.Columns.Contains("TeacherId"))
                {
                    return;
                }

                DataGridViewRow row = Tabla.SelectedRows[0];

                // Validar que la celda TeacherId no sea nula
                if (row.Cells["TeacherId"].Value == null ||
                    row.Cells["TeacherId"].Value == DBNull.Value)
                {
                    return;
                }

                int teacherId = Convert.ToInt32(row.Cells["TeacherId"].Value);

                var docente = Ctrl_Teachers.ObtenerDocentePorId(teacherId);

                if (docente != null)
                {
                    _docenteSeleccionado = docente;
                    CargarDatosEnFormulario(docente);
                    HabilitarBotonesEdicionEliminacion(true);
                }
            }
            catch (Exception ex)
            {
                // Solo mostrar el error si no es un error de inicialización
                if (!ex.Message.Contains("SelectedIndex") && !ex.Message.Contains("InvalidArgument"))
                {
                    MessageBox.Show($"Error al seleccionar docente: {ex.Message}",
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // CORRECCIÓN 6: Evento del botón de búsqueda en el panel de búsqueda
        // Aplica los filtros cuando se hace clic en el botón buscar
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                AplicarFiltros(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en búsqueda: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // CORRECCIÓN 6: Evento del botón de limpieza en el panel de búsqueda
        // Limpia todos los filtros y recarga todos los docentes
        private void Btn_Limpiar_Click(object sender, EventArgs e)
        {
            try
            {
                // Limpiar el campo de búsqueda
                Txt_ValorBuscado.Text = "BUSCAR POR NOMBRE, DPI, TÍTULO...";
                Txt_ValorBuscado.ForeColor = Color.Gray;
                _ultimoTextoBusqueda = "";

                // Restablecer todos los filtros a su estado inicial
                Filtro1.SelectedIndex = 0;  // TODAS LAS ESPECIALIZACIONES
                Filtro2.SelectedIndex = 0;  // TODOS
                Filtro3.SelectedIndex = 0;  // TODOS

                // Recargar la lista completa sin filtros
                _listaCompletaFiltrada = docentesList;
                totalRegistros = _listaCompletaFiltrada.Count;
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                if (totalPaginas == 0) totalPaginas = 1;
                paginaActual = 1;

                MostrarPagina(paginaActual);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar filtros: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion EventosFaltantes
    }
}