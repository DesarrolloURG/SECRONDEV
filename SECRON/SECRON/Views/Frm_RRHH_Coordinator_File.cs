using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_RRHH_Coordinator_File : Form
    {
        #region PropiedadesIniciales
        // Variables Globales para mantener los filtros activos
        private string _ultimoTextoBusqueda = "";
        private string _ultimaEspecializacion = "";
        private string _ultimoFiltroBancario = "TODOS";
        private List<Mdl_Coordinators> _listaCompletaFiltrada = null;
        public Mdl_Security_UserInfo UserData { get; set; }
        private Mdl_Coordinators _coordinadorSeleccionado = null;
        private List<Mdl_Coordinators> coordinadoresList;
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        private async void Frm_RRHH_Coordinator_File_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Configuraciones visuales primero
                ConfigurarTabIndexYFocus();
                ConfigurarPlaceHoldersTextbox();
                ConfigurarMaxLengthTextBox();
                ConfigurarComboBoxes();
                InicializarScroll();
                ConfigurarEventosScroll();
                CrearToolStripPaginacion();
                CargarProximoCodigoCoordinador();

                // Permisos
                if (UserData != null)
                {
                    await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
                    ConfigurarControlesPorPermisos();
                }

                // Datos
                CargarCoordinadores();

                // Filtros AL FINAL (despues de que coordinadoresList ya este cargada)
                CargarFiltros();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                              "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormularioResize(object sender, EventArgs e)
        {
            if (Tabla != null && Tabla.DataSource != null)
            {
                Tabla.Refresh();
            }
        }

        public Frm_RRHH_Coordinator_File()
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
        // Configura las longitudes maximas permitidas para cada TextBox
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

            // Bloquear codigo de coordinador (es automatico)
            Txt_Code.Enabled = false;
            Txt_Code.BackColor = Color.FromArgb(240, 240, 240);
            Txt_Code.Cursor = Cursors.No;
        }

        // Configura los textos de ayuda (placeholders) en los TextBox
        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR POR NOMBRE, DPI, TÍTULO...");
            ConfigurarPlaceHolder(Txt_TeacherName, "NOMBRE COMPLETO DEL COORDINADOR");
            ConfigurarPlaceHolder(Txt_Phone, "+502");
            ConfigurarPlaceHolder(Txt_Email, "CORREO ELECTRÓNICO");
            ConfigurarPlaceHolder(Txt_Dpi, "DOCUMENTO PERSONAL DE IDENTIFICACIÓN");
            ConfigurarPlaceHolder(Txt_Nit, "NÚMERO DE IDENTIFICACIÓN TRIBUTARIA");
            ConfigurarPlaceHolder(Txt_Address, "DIRECCIÓN COMPLETA");
            ConfigurarPlaceHolder(Txt_AcademicTitle, "TÍTULO ACADÉMICO (EJ: LICENCIADO EN...)");
            ConfigurarPlaceHolder(Txt_CollegiateNumber, "NÚMERO DE COLEGIADO");
            ConfigurarPlaceHolder(Txt_BankAccountNumber, "NÚMERO DE CUENTA BANCARIA");
        }

        // Configura un placeholder individual para un TextBox especifico
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
        #region CodigoCoordinadorAutomatico
        // Obtiene y muestra el proximo codigo de coordinador disponible
        private void CargarProximoCodigoCoordinador()
        {
            try
            {
                string proximoCodigo = Ctrl_Coordinators.ObtenerProximoCodigoCoordinador();
                Txt_Code.Text = proximoCodigo;
                Txt_Code.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de coordinador: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Txt_Code.Text = "ERROR";
            }
        }
        #endregion CodigoCoordinadorAutomatico
        #region ConfigurarComboBox
        // Configura los ComboBox para que solo permitan seleccion (no escritura)
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
        // Se agrega la opcion "SIN SEDE ASIGNADA" porque un coordinador puede no tener sede
        private void CargarSedes()
        {
            try
            {
                ComboBox_Location.Items.Clear();
                ComboBox_Location.Items.Add(new KeyValuePair<int, string>(0, "SIN SEDE ASIGNADA"));

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
        // Configura el orden de tabulacion entre controles
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
        // Carga la lista completa de coordinadores desde la base de datos
        private void CargarCoordinadores()
        {
            if (!Btn_Search.Enabled) return;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                coordinadoresList = Ctrl_Coordinators.MostrarTodosCoordinadores();
                _listaCompletaFiltrada = coordinadoresList;
                totalRegistros = coordinadoresList.Count;
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
                MessageBox.Show($"Error al cargar coordinadores: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Muestra una pagina especifica de coordinadores en el DataGridView
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
                AgregarColumnasArchivos();
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

                Tabla.Columns["CoordinatorId"].Visible = false;
                Tabla.Columns["UserId"].Visible = false;
                Tabla.Columns["RegisteredByCoordinatorId"].Visible = false;
                Tabla.Columns["CreatedBy"].Visible = false;
                Tabla.Columns["CreatedDate"].Visible = false;
                Tabla.Columns["ModifiedBy"].Visible = false;
                Tabla.Columns["ModifiedDate"].Visible = false;
                Tabla.Columns["BankId"].Visible = false;
                Tabla.Columns["LocationId"].Visible = false;

                Tabla.Columns["CoordinatorCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["CoordinatorCode"].Width = 80;
                Tabla.Columns["CoordinatorCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

        // Nombres de los 7 documentos (deben coincidir con los sufijos de FilePath_* del modelo)
        private readonly string[] _docsArchivos = { "DPI", "Titulos", "RTU", "Colegiado", "RENAS", "AntPoliciacos", "AntPenales" };
        private readonly string[] _headersAbrir = { "DPI (ABRIR)", "TÍTULOS (ABRIR)", "RTU (ABRIR)", "COLEGIADO (ABRIR)", "RENAS (ABRIR)", "ANT.POL. (ABRIR)", "ANT.PEN. (ABRIR)" };
        private readonly string[] _headersCargar = { "DPI (CARGAR)", "TÍTULOS (CARGAR)", "RTU (CARGAR)", "COLEGIADO (CARGAR)", "RENAS (CARGAR)", "ANT.POL. (CARGAR)", "ANT.PEN. (CARGAR)" };
        private readonly string[] _headersEstado = { "DPI (ESTADO)", "TÍTULOS (ESTADO)", "RTU (ESTADO)", "COLEGIADO (ESTADO)", "RENAS (ESTADO)", "ANT.POL. (ESTADO)", "ANT.PEN. (ESTADO)" };

        // Agrega las 21 columnas de imagen (ABRIR/CARGAR/ESTADO por documento) al final del grid.
        // Oculta las 7 columnas de texto FilePath_* que el binding genera automaticamente.
        private void AgregarColumnasArchivos()
        {
            if (Tabla.Columns.Count == 0) return;

            // Ocultar las columnas de texto autogeneradas por el DataSource
            foreach (string doc in _docsArchivos)
            {
                string colTexto = "FilePath_" + doc;
                if (Tabla.Columns.Contains(colTexto))
                    Tabla.Columns[colTexto].Visible = false;
            }

            // Evitar duplicar columnas si el metodo se llama varias veces
            if (Tabla.Columns.Contains("ColAbrir_DPI")) return;

            for (int i = 0; i < _docsArchivos.Length; i++)
            {
                var colAbrir = new DataGridViewImageColumn();
                colAbrir.Name = $"ColAbrir_{_docsArchivos[i]}";
                colAbrir.HeaderText = _headersAbrir[i];
                colAbrir.Width = 110;
                colAbrir.ImageLayout = DataGridViewImageCellLayout.Zoom;
                colAbrir.DefaultCellStyle.NullValue = Properties.Resources.SearchNegro25x25;
                colAbrir.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                colAbrir.ToolTipText = _headersAbrir[i];
                Tabla.Columns.Add(colAbrir);

                var colCargar = new DataGridViewImageColumn();
                colCargar.Name = $"ColCargar_{_docsArchivos[i]}";
                colCargar.HeaderText = _headersCargar[i];
                colCargar.Width = 110;
                colCargar.ImageLayout = DataGridViewImageCellLayout.Zoom;
                colCargar.DefaultCellStyle.NullValue = Properties.Resources.UploadFileBlack25x25;
                colCargar.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                colCargar.ToolTipText = _headersCargar[i];
                Tabla.Columns.Add(colCargar);

                var colEstado = new DataGridViewImageColumn();
                colEstado.Name = $"ColEstado_{_docsArchivos[i]}";
                colEstado.HeaderText = _headersEstado[i];
                colEstado.Width = 110;
                colEstado.ImageLayout = DataGridViewImageCellLayout.Zoom;
                colEstado.DefaultCellStyle.NullValue = Properties.Resources.InactivarRojo25x25;
                colEstado.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                colEstado.ToolTipText = _headersEstado[i];
                Tabla.Columns.Add(colEstado);
            }

            // El icono verde de ESTADO se pinta segun haya o no ruta (via CellFormatting)
            Tabla.CellFormatting -= Tabla_CellFormatting_Archivos;
            Tabla.CellFormatting += Tabla_CellFormatting_Archivos;

            Tabla.CellClick -= Tabla_CellClick_Archivos;
            Tabla.CellClick += Tabla_CellClick_Archivos;
        }

        // Pinta el icono de ESTADO (verde si hay archivo) leyendo del modelo enlazado a la fila
        private void Tabla_CellFormatting_Archivos(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = Tabla.Columns[e.ColumnIndex].Name;
            if (!colName.StartsWith("ColEstado_")) return;

            var coordinador = Tabla.Rows[e.RowIndex].DataBoundItem as Mdl_Coordinators;
            if (coordinador == null) return;

            string doc = colName.Substring("ColEstado_".Length);
            string ruta = ObtenerRutaPorDoc(coordinador, doc);

            if (!string.IsNullOrWhiteSpace(ruta))
            {
                e.Value = Properties.Resources.SaveVerde25x25;
                e.FormattingApplied = true;
            }
        }

        // Devuelve la ruta almacenada en el modelo para un documento dado
        private string ObtenerRutaPorDoc(Mdl_Coordinators c, string doc)
        {
            switch (doc)
            {
                case "DPI": return c.FilePath_DPI;
                case "Titulos": return c.FilePath_Titulos;
                case "RTU": return c.FilePath_RTU;
                case "Colegiado": return c.FilePath_Colegiado;
                case "RENAS": return c.FilePath_RENAS;
                case "AntPoliciacos": return c.FilePath_AntPoliciacos;
                case "AntPenales": return c.FilePath_AntPenales;
                default: return null;
            }
        }
        #endregion CargarDatos
        #region ToolStrip
        // Crea la barra de herramientas para la paginacion
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

        // Actualiza los botones numerados de paginacion
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

        // Cambia a una pagina especifica
        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginas)
            {
                paginaActual = nuevaPagina;
                MostrarPagina(paginaActual);
                ActualizarBotonesNumerados();
            }
        }

        // Actualiza la informacion mostrada de la paginacion
        private void ActualizarInfoPaginacion()
        {
            int inicioRango = (paginaActual - 1) * registrosPorPagina + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            if (Lbl_Paginas != null)
            {
                if (totalRegistros == 0)
                {
                    Lbl_Paginas.Text = "NO HAY COORDINADORES PARA MOSTRAR";
                }
                else
                {
                    Lbl_Paginas.Text = $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} COORDINADORES";
                }
            }

            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;

            ActualizarBotonesNumerados();
        }
        #endregion ToolStrip
        #region ScrollBar
        // Inicializa las propiedades del scroll vertical
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

        // Configura los eventos del scroll para sincronizarlo con el panel
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

        private void Panel_Izquierdo_MouseEnter(object sender, EventArgs e)
        {
            Panel_Izquierdo.Focus();
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            MoverContenido(e.NewValue);
        }

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

        // Aplica los filtros seleccionados a la lista de coordinadores
        private void AplicarFiltros(object sender, EventArgs e)
        {
            try
            {
                if (coordinadoresList == null) return;

                this.Cursor = Cursors.WaitCursor;

                _ultimaEspecializacion = Filtro1.SelectedItem?.ToString() ?? "TODAS LAS ESPECIALIZACIONES";
                _ultimoFiltroBancario = Filtro2.SelectedItem?.ToString() ?? "TODOS";
                string filtroColegiadoActual = Filtro3.SelectedItem?.ToString() ?? "TODOS";

                var listaFiltrada = coordinadoresList.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(_ultimoTextoBusqueda))
                {
                    listaFiltrada = listaFiltrada.Where(c =>
                        (c.FullName?.ToUpper().Contains(_ultimoTextoBusqueda.ToUpper()) ?? false) ||
                        (c.DPI?.ToUpper().Contains(_ultimoTextoBusqueda.ToUpper()) ?? false) ||
                        (c.Email?.ToUpper().Contains(_ultimoTextoBusqueda.ToUpper()) ?? false) ||
                        (c.AcademicTitle?.ToUpper().Contains(_ultimoTextoBusqueda.ToUpper()) ?? false)
                    );
                }

                if (_ultimaEspecializacion != "TODAS LAS ESPECIALIZACIONES")
                {
                    listaFiltrada = listaFiltrada.Where(c =>
                        string.Equals(c.Specialization, _ultimaEspecializacion, StringComparison.OrdinalIgnoreCase)
                    );
                }

                if (_ultimoFiltroBancario == "CON BANCO")
                {
                    listaFiltrada = listaFiltrada.Where(c => c.BankId.HasValue && c.BankId.Value > 0);
                }
                else if (_ultimoFiltroBancario == "SIN BANCO")
                {
                    listaFiltrada = listaFiltrada.Where(c => !c.BankId.HasValue || c.BankId.Value == 0);
                }

                if (filtroColegiadoActual == "COLEGIADOS ACTIVOS")
                {
                    listaFiltrada = listaFiltrada.Where(c => c.IsCollegiateActive);
                }
                else if (filtroColegiadoActual == "NO COLEGIADOS")
                {
                    listaFiltrada = listaFiltrada.Where(c => !c.IsCollegiateActive);
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
        // Evento de busqueda en tiempo real mientras se escribe
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
                    int coordinatorId = Convert.ToInt32(row.Cells["CoordinatorId"].Value);

                    var coordinador = Ctrl_Coordinators.ObtenerCoordinadorPorId(coordinatorId);

                    if (coordinador != null)
                    {
                        _coordinadorSeleccionado = coordinador;
                        CargarDatosEnFormulario(coordinador);
                        HabilitarBotonesEdicionEliminacion(true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar coordinador: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Tabla_CellClick_Archivos(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = Tabla.Columns[e.ColumnIndex].Name;
            if (!colName.StartsWith("ColAbrir_") && !colName.StartsWith("ColCargar_")) return;

            var coordinador = Tabla.Rows[e.RowIndex].DataBoundItem as Mdl_Coordinators;
            if (coordinador == null) return;

            int coordinatorId = coordinador.CoordinatorId;

            // ===== ABRIR ARCHIVO =====
            if (colName.StartsWith("ColAbrir_"))
            {
                string doc = colName.Substring("ColAbrir_".Length);
                string campo = "FilePath_" + doc;
                string filePath = ObtenerRutaPorDoc(coordinador, doc);

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    MessageBox.Show("ESTE COORDINADOR NO TIENE ESTE ARCHIVO VINCULADO.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!System.IO.File.Exists(filePath))
                {
                    Ctrl_Coordinators.ActualizarFilePathCoordinador(coordinatorId, campo, null, UserData.UserId);
                    MessageBox.Show("EL ARCHIVO NO SE ENCUENTRA EN LA RUTA INDICADA.\nSE HA LIMPIADO EL VÍNCULO AUTOMÁTICAMENTE.",
                        "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    CargarCoordinadores();
                    return;
                }

                System.Diagnostics.Process.Start(filePath);
                return;
            }

            // ===== CARGAR ARCHIVO =====
            if (colName.StartsWith("ColCargar_"))
            {
                string doc = colName.Substring("ColCargar_".Length);
                string campo = "FilePath_" + doc;
                string rutaActual = ObtenerRutaPorDoc(coordinador, doc);
                string nombreArchivo = NombreArchivoPorDoc(doc);

                if (!string.IsNullOrWhiteSpace(rutaActual))
                {
                    if (MessageBox.Show("ESTE COORDINADOR YA TIENE ESTE ARCHIVO VINCULADO.\n¿DESEA REEMPLAZARLO?",
                        "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                }

                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "SELECCIONAR ARCHIVO PDF";
                    dlg.Filter = "Archivos PDF (*.pdf)|*.pdf";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            string carpetaDestino = $@"\\Uregional\Shared$\SECRONDEV\RECURSOS HUMANOS\DOCENCIA\COORDINADORES\{coordinatorId}\";
                            //string carpetaDestino = $@"\\Uregional\Shared$\SECRONQA\RECURSOS HUMANOS\DOCENCIA\COORDINADORES\{coordinatorId}\";
                            //string carpetaDestino = $@"\\Uregional\Shared$\SECRON\RECURSOS HUMANOS\DOCENCIA\COORDINADORES\{coordinatorId}\";

                            if (!System.IO.Directory.Exists(carpetaDestino))
                                System.IO.Directory.CreateDirectory(carpetaDestino);

                            string rutaDestino = System.IO.Path.Combine(carpetaDestino, nombreArchivo);

                            System.IO.File.Copy(dlg.FileName, rutaDestino, overwrite: true);

                            bool ok = Ctrl_Coordinators.ActualizarFilePathCoordinador(coordinatorId, campo, rutaDestino, UserData.UserId);
                            if (ok)
                            {
                                MessageBox.Show("ARCHIVO VINCULADO CORRECTAMENTE.", "ÉXITO",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                CargarCoordinadores();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("ERROR AL COPIAR ARCHIVO: " + ex.Message, "ERROR SECRON",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        // Nombre fijo del PDF por tipo de documento
        private string NombreArchivoPorDoc(string doc)
        {
            switch (doc)
            {
                case "DPI": return "DPI.pdf";
                case "Titulos": return "TITULOS.pdf";
                case "RTU": return "RTU.pdf";
                case "Colegiado": return "COLEGIADO.pdf";
                case "RENAS": return "RENAS.pdf";
                case "AntPoliciacos": return "ANT_POLICIACOS.pdf";
                case "AntPenales": return "ANT_PENALES.pdf";
                default: return doc + ".pdf";
            }
        }

        // Carga los datos del coordinador seleccionado en los controles del formulario
        private void CargarDatosEnFormulario(Mdl_Coordinators coordinador)
        {
            try
            {
                Txt_Code.Text = coordinador.CoordinatorCode ?? "";
                Txt_TeacherName.Text = coordinador.FullName ?? "";
                Txt_TeacherName.ForeColor = Color.Black;

                Txt_Phone.Text = coordinador.Phone ?? "";
                if (!string.IsNullOrWhiteSpace(coordinador.Phone))
                    Txt_Phone.ForeColor = Color.Black;

                Txt_Email.Text = coordinador.Email ?? "";
                if (!string.IsNullOrWhiteSpace(coordinador.Email))
                    Txt_Email.ForeColor = Color.Black;

                Txt_Dpi.Text = coordinador.DPI ?? "";
                if (!string.IsNullOrWhiteSpace(coordinador.DPI))
                    Txt_Dpi.ForeColor = Color.Black;

                Txt_Nit.Text = coordinador.NIT ?? "";
                if (!string.IsNullOrWhiteSpace(coordinador.NIT))
                    Txt_Nit.ForeColor = Color.Black;

                Txt_Address.Text = coordinador.Address ?? "";
                if (!string.IsNullOrWhiteSpace(coordinador.Address))
                    Txt_Address.ForeColor = Color.Black;

                Txt_AcademicTitle.Text = coordinador.AcademicTitle ?? "";
                if (!string.IsNullOrWhiteSpace(coordinador.AcademicTitle))
                    Txt_AcademicTitle.ForeColor = Color.Black;

                // Cargar Specialization - CON VALIDACION
                if (ComboBox_Specialization.Items.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(coordinador.Specialization))
                    {
                        bool encontrado = false;
                        for (int i = 0; i < ComboBox_Specialization.Items.Count; i++)
                        {
                            if (ComboBox_Specialization.Items[i].ToString() == coordinador.Specialization)
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

                // Cargar IsCollegiateActive en ComboBox (1=SI, 0=NO)
                if (ComboBox_IsCollegiateActive.Items.Count > 0)
                {
                    ComboBox_IsCollegiateActive.SelectedIndex = coordinador.IsCollegiateActive ? 1 : 0;
                }

                Txt_CollegiateNumber.Text = coordinador.CollegiateNumber ?? "";
                if (!string.IsNullOrWhiteSpace(coordinador.CollegiateNumber))
                    Txt_CollegiateNumber.ForeColor = Color.Black;

                Txt_BankAccountNumber.Text = coordinador.BankAccountNumber ?? "";
                if (!string.IsNullOrWhiteSpace(coordinador.BankAccountNumber))
                    Txt_BankAccountNumber.ForeColor = Color.Black;

                // Cargar Bank - CON VALIDACION
                if (ComboBox_Bank.Items.Count > 0)
                {
                    if (coordinador.BankId.HasValue)
                    {
                        bool encontrado = false;
                        for (int i = 0; i < ComboBox_Bank.Items.Count; i++)
                        {
                            var item = (KeyValuePair<int?, string>)ComboBox_Bank.Items[i];
                            if (item.Key == coordinador.BankId.Value)
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

                // Cargar Location - CON VALIDACION (LocationId es NULLABLE)
                // Indice 0 del combo = "SIN SEDE ASIGNADA" (key 0)
                if (ComboBox_Location.Items.Count > 0)
                {
                    if (coordinador.LocationId.HasValue)
                    {
                        bool encontrado = false;
                        for (int i = 0; i < ComboBox_Location.Items.Count; i++)
                        {
                            var item = (KeyValuePair<int, string>)ComboBox_Location.Items[i];
                            if (item.Key == coordinador.LocationId.Value)
                            {
                                ComboBox_Location.SelectedIndex = i;
                                encontrado = true;
                                break;
                            }
                        }
                        if (!encontrado)
                        {
                            ComboBox_Location.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        // Sin sede -> seleccionar "SIN SEDE ASIGNADA"
                        ComboBox_Location.SelectedIndex = 0;
                    }
                }

                // Cargar HireDate
                if (coordinador.HireDate.HasValue)
                {
                    DTP_HireDate.Value = coordinador.HireDate.Value;
                }
                else
                {
                    DTP_HireDate.Value = DateTime.Now;
                }

                // Cargar ContractType - CON VALIDACION
                if (ComboBox_ContractType.Items.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(coordinador.ContractType))
                    {
                        bool encontrado = false;
                        for (int i = 0; i < ComboBox_ContractType.Items.Count; i++)
                        {
                            if (ComboBox_ContractType.Items[i].ToString() == coordinador.ContractType)
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
        // Evento del boton Guardar - SOLO registra coordinadores NUEVOS
        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!Btn_Save.Enabled) return;
            try
            {
                if (!ValidarCamposObligatorios())
                    return;

                var coordinador = ObtenerDatosDelFormulario();

                coordinador.CreatedBy = UserData?.UserId;
                coordinador.CreatedDate = DateTime.Now;

                int resultado = Ctrl_Coordinators.RegistrarCoordinador(coordinador);

                if (resultado > 0)
                {
                    MessageBox.Show("Coordinador registrado correctamente.",
                                  "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarFormulario();
                    CargarCoordinadores();
                    CargarProximoCodigoCoordinador();

                    HabilitarBotonesEdicionEliminacion(false);
                    _coordinadorSeleccionado = null;
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el coordinador.",
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento del boton Update (Actualizar)
        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (!Btn_Update.Enabled) return;

            try
            {
                if (_coordinadorSeleccionado == null)
                {
                    MessageBox.Show("Debe seleccionar un coordinador de la tabla para poder actualizarlo.",
                                  "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarCamposObligatorios())
                    return;

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea actualizar los datos del coordinador?\n\n" +
                    $"Coordinador: {_coordinadorSeleccionado.FullName}\n" +
                    $"Código: {_coordinadorSeleccionado.CoordinatorCode}",
                    "Confirmar Actualización",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    var coordinador = ObtenerDatosDelFormulario();

                    coordinador.CoordinatorId = _coordinadorSeleccionado.CoordinatorId;
                    coordinador.CreatedBy = _coordinadorSeleccionado.CreatedBy;
                    coordinador.CreatedDate = _coordinadorSeleccionado.CreatedDate;
                    coordinador.ModifiedBy = UserData?.UserId;
                    coordinador.ModifiedDate = DateTime.Now;

                    int resultado = Ctrl_Coordinators.ActualizarCoordinador(coordinador);

                    if (resultado > 0)
                    {
                        MessageBox.Show("Coordinador actualizado correctamente.",
                                      "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LimpiarFormulario();
                        CargarCoordinadores();
                        CargarProximoCodigoCoordinador();

                        HabilitarBotonesEdicionEliminacion(false);
                        _coordinadorSeleccionado = null;

                        AplicarEstadoBotonPorPermiso(Btn_Save, "EMPLOYEES_COORDINATORS_CREATE");
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar el coordinador.",
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

        // Evento del boton Inactive (Inactivar)
        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            if (!Btn_Inactive.Enabled) return;

            try
            {
                if (_coordinadorSeleccionado == null)
                {
                    MessageBox.Show("Debe seleccionar un coordinador para inactivar.",
                                  "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea inactivar al coordinador {_coordinadorSeleccionado.FullName}?\n\n" +
                    "Esta acción marcará el registro como inactivo.",
                    "Confirmar inactivación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    int resultado = Ctrl_Coordinators.InactivarCoordinador(
                        _coordinadorSeleccionado.CoordinatorId,
                        UserData?.UserId ?? 0);

                    if (resultado > 0)
                    {
                        MessageBox.Show("Coordinador inactivado correctamente.",
                                      "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarCoordinadores();
                        HabilitarBotonesEdicionEliminacion(false);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo inactivar el coordinador.",
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

        // Evento del boton Clear (Limpiar)
        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            if (!Btn_Clear.Enabled) return;

            LimpiarFormulario();
            _coordinadorSeleccionado = null;
            HabilitarBotonesEdicionEliminacion(false);
            HabilitarControlesEdicion(true);
            Txt_Code.Enabled = false;
            CargarProximoCodigoCoordinador();
        }
        #endregion EventosBotones
        #region MetodosAuxiliares
        // Limpia todos los controles del formulario
        private void LimpiarFormulario()
        {
            Txt_Code.Clear();

            ConfigurarPlaceHolder(Txt_TeacherName, "NOMBRE COMPLETO DEL COORDINADOR");
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

            _coordinadorSeleccionado = null;
        }

        // Valida que los campos obligatorios esten completos
        // NOTA: La sede ya NO es obligatoria (un coordinador puede no tener sede asignada)
        private bool ValidarCamposObligatorios()
        {
            if (string.IsNullOrWhiteSpace(Txt_TeacherName.Text) ||
                Txt_TeacherName.Text == "NOMBRE COMPLETO DEL COORDINADOR")
            {
                MessageBox.Show("El nombre completo es obligatorio.",
                              "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_TeacherName.Focus();
                return false;
            }

            return true;
        }

        // Obtiene los datos del formulario y los empaqueta en un objeto Mdl_Coordinators
        private Mdl_Coordinators ObtenerDatosDelFormulario()
        {
            var coordinador = new Mdl_Coordinators
            {
                CoordinatorCode = Txt_Code.Text,
                FullName = ObtenerTextoLimpio(Txt_TeacherName, "NOMBRE COMPLETO DEL COORDINADOR"),
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
                coordinador.IsCollegiateActive = selectedItem.Key == 1;
            }

            // Obtener BankId del ComboBox
            if (ComboBox_Bank.SelectedIndex > 0)
            {
                var selectedBank = (KeyValuePair<int?, string>)ComboBox_Bank.SelectedItem;
                coordinador.BankId = selectedBank.Key;
            }

            // Obtener LocationId del ComboBox (NULLABLE)
            // Key 0 = "SIN SEDE ASIGNADA" -> se guarda como NULL
            if (ComboBox_Location.SelectedItem != null)
            {
                var selectedLocation = (KeyValuePair<int, string>)ComboBox_Location.SelectedItem;
                coordinador.LocationId = selectedLocation.Key == 0 ? (int?)null : selectedLocation.Key;
            }
            else
            {
                coordinador.LocationId = null;
            }

            return coordinador;
        }

        // Obtiene el texto limpio de un TextBox (sin placeholder)
        private string ObtenerTextoLimpio(TextBox textBox, string placeholder)
        {
            if (textBox.Text == placeholder || string.IsNullOrWhiteSpace(textBox.Text))
                return null;
            return textBox.Text.Trim();
        }

        // Habilita o deshabilita los controles de edicion
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

        // Habilita o deshabilita los botones de edicion e inactivacion
        private void HabilitarBotonesEdicionEliminacion(bool habilitar)
        {
            Btn_Update.Enabled = habilitar && TienePermiso("EMPLOYEES_COORDINATORS_UPDATE");
            Btn_Inactive.Enabled = habilitar && TienePermiso("EMPLOYEES_COORDINATORS_INACTIVE");
        }
        #endregion MetodosAuxiliares
        #region ExportarExcel
        // Exporta la lista FILTRADA de coordinadores a un archivo Excel
        private void Btn_Export_Click(object sender, EventArgs e)
        {
            if (!Btn_Export.Enabled) return;

            try
            {
                var coordinadoresAExportar = _listaCompletaFiltrada;

                if (coordinadoresAExportar == null || coordinadoresAExportar.Count == 0)
                {
                    MessageBox.Show("No hay coordinadores para exportar.",
                                  "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Lista de Coordinadores",
                    FileName = $"Coordinadores_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    var excelApp = new Excel.Application();
                    var workbook = excelApp.Workbooks.Add();
                    var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Coordinadores";

                    worksheet.Cells[1, 1] = "REPORTE COMPLETO DE COORDINADORES";
                    worksheet.Range["A1:O1"].Merge();
                    worksheet.Range["A1:O1"].Font.Size = 16;
                    worksheet.Range["A1:O1"].Font.Bold = true;
                    worksheet.Range["A1:O1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Range["A1:O1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    worksheet.Range["A1:O1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                    worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SISTEMA"}";
                    worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {coordinadoresAExportar.Count}";

                    worksheet.Range["A2:A4"].Font.Size = 10;
                    worksheet.Range["A2:A4"].Font.Bold = true;

                    int headerRow = 6;
                    string[] headers = {
                        "CÓDIGO", "NOMBRE COMPLETO", "TELÉFONO", "EMAIL", "DPI", "NIT",
                        "DIRECCIÓN", "TÍTULO ACADÉMICO", "ESPECIALIZACIÓN", "COLEGIADO ACTIVO",
                        "No. COLEGIADO", "No. CUENTA BANCARIA", "FECHA CONTRATACIÓN", "TIPO CONTRATO", "ACTIVO"
                    };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[headerRow, i + 1] = headers[i];
                    }

                    var headerRange = worksheet.Range[$"A{headerRow}:O{headerRow}"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Size = 11;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    int row = headerRow + 1;
                    foreach (var coordinador in coordinadoresAExportar)
                    {
                        worksheet.Cells[row, 1] = coordinador.CoordinatorCode ?? "N/A";
                        worksheet.Cells[row, 2] = coordinador.FullName ?? "";
                        worksheet.Cells[row, 3] = coordinador.Phone ?? "N/A";
                        worksheet.Cells[row, 4] = coordinador.Email ?? "N/A";
                        worksheet.Cells[row, 5] = coordinador.DPI ?? "N/A";
                        worksheet.Cells[row, 6] = coordinador.NIT ?? "N/A";
                        worksheet.Cells[row, 7] = coordinador.Address ?? "N/A";
                        worksheet.Cells[row, 8] = coordinador.AcademicTitle ?? "N/A";
                        worksheet.Cells[row, 9] = coordinador.Specialization ?? "NO ESPECIFICA";
                        worksheet.Cells[row, 10] = coordinador.IsCollegiateActive ? "SÍ" : "NO";
                        worksheet.Cells[row, 11] = coordinador.CollegiateNumber ?? "N/A";
                        worksheet.Cells[row, 12] = coordinador.BankAccountNumber ?? "N/A";
                        worksheet.Cells[row, 13] = coordinador.HireDate.HasValue
                            ? coordinador.HireDate.Value.ToString("dd/MM/yyyy")
                            : "N/A";
                        worksheet.Cells[row, 14] = coordinador.ContractType ?? "N/A";
                        worksheet.Cells[row, 15] = coordinador.IsActive ? "SÍ" : "NO";

                        if (row % 2 == 0)
                        {
                            worksheet.Range[$"A{row}:O{row}"].Interior.Color =
                                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                        }

                        row++;
                    }

                    var dataRange = worksheet.Range[$"A{headerRow}:O{row - 1}"];
                    dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                    worksheet.Columns.AutoFit();
                    worksheet.Columns[2].ColumnWidth = 35;
                    worksheet.Columns[4].ColumnWidth = 30;
                    worksheet.Columns[7].ColumnWidth = 40;
                    worksheet.Columns[8].ColumnWidth = 30;

                    worksheet.Activate();
                    excelApp.ActiveWindow.SplitRow = headerRow;
                    excelApp.ActiveWindow.FreezePanes = true;

                    worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
                    worksheet.Range[$"A{row + 1}:O{row + 1}"].Merge();
                    worksheet.Range[$"A{row + 1}:O{row + 1}"].Font.Italic = true;
                    worksheet.Range[$"A{row + 1}:O{row + 1}"].Font.Size = 9;
                    worksheet.Range[$"A{row + 1}:O{row + 1}"].HorizontalAlignment =
                        Excel.XlHAlign.xlHAlignCenter;

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
        #region EventosFaltantes
        // Evento cuando cambia la seleccion en el DataGridView
        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {

        }

        // Evento del boton de busqueda en el panel de busqueda
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            if (!Btn_Search.Enabled) return;

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

        // Evento del boton de limpieza en el panel de busqueda
        private void Btn_Limpiar_Click(object sender, EventArgs e)
        {
            if (!Btn_CleanSearch.Enabled) return;

            try
            {
                Txt_ValorBuscado.Text = "BUSCAR POR NOMBRE, DPI, TÍTULO...";
                Txt_ValorBuscado.ForeColor = Color.Gray;
                _ultimoTextoBusqueda = "";

                Filtro1.SelectedIndex = 0;
                Filtro2.SelectedIndex = 0;
                Filtro3.SelectedIndex = 0;

                _listaCompletaFiltrada = coordinadoresList;
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
            AplicarEstadoBotonPorPermiso(Btn_Search, "EMPLOYEES_COORDINATORS_READ");
            AplicarEstadoBotonPorPermiso(Btn_Save, "EMPLOYEES_COORDINATORS_CREATE");
            AplicarEstadoBotonPorPermiso(Btn_Update, "EMPLOYEES_COORDINATORS_UPDATE");
            AplicarEstadoBotonPorPermiso(Btn_Inactive, "EMPLOYEES_COORDINATORS_INACTIVE");
            AplicarEstadoBotonPorPermiso(Btn_Export, "EMPLOYEES_COORDINATORS_EXPORT");
            AplicarEstadoBotonPorPermiso(Btn_Import, "EMPLOYEES_COORDINATORS_IMPORT");
        }

        #endregion SistemaDePermisos
    }
}