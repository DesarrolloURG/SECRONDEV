using SECRON.Configuration;
using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_Checks_FileControl : Form
    {
        #region PropiedadesIniciales
        // Propiedades para resize panel izquierdo
        private bool _isResizingPanel = false;
        private int _resizeStartX = 0;
        private const int PANEL_WIDTH_MAX = 477;
        private const int PANEL_WIDTH_MIN = 200;
        private const int RESIZE_BORDER_WIDTH = 5;

        // Variables globales para mantener filtros activos
        private string _ultimoTextoBusqueda = "";
        private string _ultimoPeriodo = "";
        private int? _ultimoLocationId = null;
        private int? _ultimoStatusId = null; // ⭐ CAMBIO: de bool? _ultimoIsActive a int? _ultimoStatusId
        private DateTime? _ultimaFechaInicio = null;
        private DateTime? _ultimaFechaFin = null;
        private List<Mdl_Checks> _listaCompletaFiltrada = null;
        private string _ultimoRangoInicio = null;
        private string _ultimoRangoFin = null;
        private List<DateTime> _mesesExcluidos = new List<DateTime>();
        private List<DateTime> _fechasExcluidas = new List<DateTime>();

        public Mdl_Security_UserInfo UserData { get; set; }
        private List<Mdl_Checks> chequesList;
        private Mdl_Checks _chequeSeleccionado = null;
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        public Frm_Checks_FileControl()
        {
            InitializeComponent();

            authController = new Ctrl_Security_Auth();

            this.Resize += (s, e) =>
            {
                if (toolStripPaginacion != null)
                {
                    toolStripPaginacion.Location = new Point(this.Width - 300, 225);
                }
            };
        }
        private async void Frm_Checks_FileControl_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                ConfigurarComboBoxes();
                ConfigurarComboFileState();
                ConfigurarPlaceHoldersTextbox();
                ConfigurarDateTimePickers();
                ConfigurarRangoCheques();
                BloquearTodosCampos();
                InicializarScroll();
                ConfigurarEventosScroll();
                CrearToolStripPaginacion();
                ConfigurarTabla();
                CargarCheques();
                ActualizarInfoPaginacion();
                ConfigurarSplitter();

                //CARGAR PERMISOS DEL USUARIO
                if (UserData != null)
                {
                    await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
                    ConfigurarBotonesPorPermisos();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL CARGAR FORMULARIO: {ex.Message}",
                              "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion PropiedadesIniciales
        #region SistemaDePermisos
        private Ctrl_Security_Auth authController;
        private List<string> permisosUsuario = new List<string>();

        private async Task CargarPermisosUsuario(int userId, int roleId)
        {
            try
            {
                permisosUsuario = await authController.ObtenerPermisosUsuarioAsync(userId, roleId);
                System.Diagnostics.Debug.WriteLine($"Permisos cargados en Checks Reports: {permisosUsuario.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR PERMISOS: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TienePermiso(string permissionCode)
        {
            if (permisosUsuario == null || permisosUsuario.Count == 0)
                return false;

            return permisosUsuario.Contains(permissionCode);
        }

        private void ConfigurarBotonesPorPermisos()
        {
            //CHECKS_REPORTS_EXPORT - Exportar (CHK_021)
            Btn_Export.Enabled = TienePermiso("CHECKS_FILECONTROL_EXPORT");
            if (!Btn_Export.Enabled)
            {
                Btn_Export.BackColor = Color.FromArgb(200, 200, 200);
                Btn_Export.ForeColor = Color.Gray;
                Btn_Export.Cursor = Cursors.No;
            }
        }
        #endregion SistemaDePermisos
        #region ConfigurarComboBoxes
        private void ConfigurarComboBoxes()
        {
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            // FILTRO 1 - PERIODOS
            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("ENERO");
            Filtro1.Items.Add("FEBRERO");
            Filtro1.Items.Add("MARZO");
            Filtro1.Items.Add("ABRIL");
            Filtro1.Items.Add("MAYO");
            Filtro1.Items.Add("JUNIO");
            Filtro1.Items.Add("JULIO");
            Filtro1.Items.Add("AGOSTO");
            Filtro1.Items.Add("SEPTIEMBRE");
            Filtro1.Items.Add("OCTUBRE");
            Filtro1.Items.Add("NOVIEMBRE");
            Filtro1.Items.Add("DICIEMBRE");
            Filtro1.SelectedIndex = 0;

            // FILTRO 2 - LOCATIONS (SEDES)
            var locations = Ctrl_Locations.ObtenerLocationsActivas();
            Filtro2.Items.Clear();
            Filtro2.Items.Add(new KeyValuePair<int, string>(0, "TODAS LAS SEDES"));
            foreach (var loc in locations)
            {
                Filtro2.Items.Add(loc);
            }
            Filtro2.DisplayMember = "Value";
            Filtro2.ValueMember = "Key";
            Filtro2.SelectedIndex = 0;

            // FILTRO 3 - ESTADO EN ARCHIVO (FILECONTROL)
            Filtro3.Items.Clear();
            Filtro3.Items.Add("TODOS");
            Filtro3.Items.Add("PENDIENTE");
            Filtro3.Items.Add("TRASLADADO");
            Filtro3.Items.Add("RECIBIDO");
            Filtro3.Items.Add("ARCHIVADO");
            Filtro3.SelectedIndex = 0;
        }

        private void ConfigurarComboFileState()
        {
            ComboBox_FileState.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_FileState.Items.Clear();
            ComboBox_FileState.Items.Add("PENDIENTE");
            ComboBox_FileState.Items.Add("TRASLADADO");
            ComboBox_FileState.Items.Add("RECIBIDO");
            ComboBox_FileState.Items.Add("ARCHIVADO");
            ComboBox_FileState.SelectedIndex = 0;
        }

        #endregion ConfigurarComboBoxes
        #region ConfigurarDateTimePickers
        private void ConfigurarDateTimePickers()
        {
            DTP_Emision.Format = DateTimePickerFormat.Long;
            DTP_Emision.Enabled = false;

            DTP_FechaInicio.Format = DateTimePickerFormat.Short;
            DTP_FechaInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            DTP_FechaFin.Format = DateTimePickerFormat.Short;
            DTP_FechaFin.Value = DateTime.Now;

            CheckBox_FiltroFechas.Checked = false;
            CheckBox_FiltroFechas.CheckedChanged += CheckBox_FiltroFechas_CheckedChanged;

            DTP_FechaInicio.Enabled = false;
            DTP_FechaFin.Enabled = false;
        }

        private void CheckBox_FiltroFechas_CheckedChanged(object sender, EventArgs e)
        {
            DTP_FechaInicio.Enabled = CheckBox_FiltroFechas.Checked;
            DTP_FechaFin.Enabled = CheckBox_FiltroFechas.Checked;
            Btn_FiltrosFechas.Enabled = CheckBox_FiltroFechas.Checked;
        }
        #endregion ConfigurarDateTimePickers
        #region ConfigurarRangoCheques
        private void ConfigurarRangoCheques()
        {
            // Configurar CheckBox
            CheckBox_Rango.Checked = false;
            CheckBox_Rango.CheckedChanged += CheckBox_Rango_CheckedChanged;

            // Configurar TextBox de inicio
            Txt_Li.Enabled = false;
            ConfigurarPlaceHolder(Txt_Li, "INICIO");
            Txt_Li.BackColor = Color.White;
            Txt_Li.ForeColor = Color.Black;
            Txt_Li.KeyPress += Txt_RangoCheque_KeyPress;

            // Configurar TextBox de fin
            Txt_Fin.Enabled = false;
            ConfigurarPlaceHolder(Txt_Fin, "FIN");
            Txt_Fin.BackColor = Color.White;
            Txt_Fin.ForeColor = Color.Black;
            Txt_Fin.KeyPress += Txt_RangoCheque_KeyPress;
        }

        private void CheckBox_Rango_CheckedChanged(object sender, EventArgs e)
        {
            Txt_Li.Enabled = CheckBox_Rango.Checked;
            Txt_Li.ForeColor = Color.Black;
            Txt_Fin.Enabled = CheckBox_Rango.Checked;
            Txt_Fin.ForeColor = Color.Black;

            // Si se deshabilita, limpiar los valores
            if (!CheckBox_Rango.Checked)
            {
                Txt_Li.Text = "INICIO";
                Txt_Li.ForeColor = Color.Gray;
                Txt_Li.BackColor = Color.White;

                Txt_Fin.Text = "FIN";
                Txt_Fin.ForeColor = Color.Gray;
                Txt_Fin.BackColor = Color.White;
            }
        }

        private void Txt_RangoCheque_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir números, letras, guiones y backspace
            if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            // Si presiona Enter, ejecutar búsqueda
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                Btn_Search_Click(sender, e);
            }
        }
        #endregion ConfigurarRangoCheques
        #region ConfigurarTextBox
        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR POR NO.CHEQUE, BENEFICIARIO, CONCEPTO...");
        }

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
        #region BloquearCampos
        private void BloquearTodosCampos()
        {
            Txt_NoCheque.Enabled = false;
            Txt_Banco.Enabled = false;
            DTP_Emision.Enabled = false;
            Txt_Periodo.Enabled = false;
            Txt_State.Enabled = false;
            Txt_Beneficiario.Enabled = false;
            Txt_Location.Enabled = false;
            Txt_Concepto.Enabled = false;
            Txt_Observaciones.Enabled = false;
            Txt_Alimentacion.Enabled = false;
            Txt_Anticipos.Enabled = false;
            Txt_Bonificacion.Enabled = false;
            Txt_Complemento.Enabled = false;
            Txt_Descuentos.Enabled = false;
            Txt_Exencion.Enabled = false;
            Txt_IGSS.Enabled = false;
            Txt_ITH.Enabled = false;
            Txt_MontoSinITH.Enabled = false;
            Txt_MontoTotal.Enabled = false;
            Txt_OrdenCompra.Enabled = false;
            Txt_RetencionISR.Enabled = false;
            Txt_Stamps.Enabled = false;
            Txt_ValorImpresoCheque.Enabled = false;
            Txt_ValorLetras.Enabled = false;
            Txt_Viaticos.Enabled = false;

            ConfigurarEstiloDeshabilitado(Txt_NoCheque);
            ConfigurarEstiloDeshabilitado(Txt_Banco);
            ConfigurarEstiloDeshabilitado(Txt_Periodo);
            ConfigurarEstiloDeshabilitado(Txt_State);
            ConfigurarEstiloDeshabilitado(Txt_Beneficiario);
            ConfigurarEstiloDeshabilitado(Txt_Location);
            ConfigurarEstiloDeshabilitado(Txt_Concepto);
            ConfigurarEstiloDeshabilitado(Txt_Observaciones);
            ConfigurarEstiloDeshabilitado(Txt_Alimentacion);
            ConfigurarEstiloDeshabilitado(Txt_Anticipos);
            ConfigurarEstiloDeshabilitado(Txt_Bonificacion);
            ConfigurarEstiloDeshabilitado(Txt_Complemento);
            ConfigurarEstiloDeshabilitado(Txt_Descuentos);
            ConfigurarEstiloDeshabilitado(Txt_Exencion);
            ConfigurarEstiloDeshabilitado(Txt_IGSS);
            ConfigurarEstiloDeshabilitado(Txt_ITH);
            ConfigurarEstiloDeshabilitado(Txt_MontoSinITH);
            ConfigurarEstiloDeshabilitado(Txt_MontoTotal);
            ConfigurarEstiloDeshabilitado(Txt_OrdenCompra);
            ConfigurarEstiloDeshabilitado(Txt_RetencionISR);
            ConfigurarEstiloDeshabilitado(Txt_Stamps);
            ConfigurarEstiloDeshabilitado(Txt_ValorImpresoCheque);
            ConfigurarEstiloDeshabilitado(Txt_ValorLetras);
            ConfigurarEstiloDeshabilitado(Txt_Viaticos);
        }

        private void ConfigurarEstiloDeshabilitado(TextBox textBox)
        {
            textBox.BackColor = Color.FromArgb(240, 240, 240);
            textBox.ForeColor = Color.FromArgb(100, 100, 100);
            textBox.Cursor = Cursors.No;
        }
        #endregion BloquearCampos
        #region ConfigurarTabla
        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();

            // Columna de selección (checkbox)
            DataGridViewCheckBoxColumn colSeleccion = new DataGridViewCheckBoxColumn
            {
                Name = "Seleccionar",
                HeaderText = "☑",
                Width = 50,
                ReadOnly = false
            };
            Tabla.Columns.Add(colSeleccion);

            // Columnas de datos
            Tabla.Columns.Add("CheckId", "ID");
            Tabla.Columns.Add("CheckNumber", "NO. CHEQUE");
            Tabla.Columns.Add("IssueDate", "FECHA EMISIÓN");
            Tabla.Columns.Add("BeneficiaryName", "BENEFICIARIO");
            Tabla.Columns.Add("Concept", "CONCEPTO");
            Tabla.Columns.Add("PrintedAmount", "VALOR IMPRESO");
            Tabla.Columns.Add("Period", "PERIODO");
            Tabla.Columns.Add("Location", "SEDE");
            Tabla.Columns.Add("Status", "ESTADO");
            Tabla.Columns.Add("FileControl", "CONTROL ARCHIVO");
            Tabla.Columns.Add("CreatedBy", "EMITIDO POR");

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = true;            // Permitir múltiples filas seleccionadas
            Tabla.ReadOnly = false;              // Para que el checkbox sea editable
            Tabla.AllowUserToResizeRows = false;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            // Marcar todas las columnas de solo lectura excepto el checkbox
            foreach (DataGridViewColumn col in Tabla.Columns)
            {
                if (col.Name != "Seleccionar")
                    col.ReadOnly = true;
            }

            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            Tabla.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            Tabla.RowTemplate.Height = 30;
            Tabla.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            Tabla.Columns["CheckId"].Visible = false;

            AjustarColumnas();

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;

            // Para que el checkbox confirme el cambio al hacer clic
            Tabla.CellContentClick -= Tabla_CellContentClick;
            Tabla.CellContentClick += Tabla_CellContentClick;
        }
        private void Tabla_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == Tabla.Columns["Seleccionar"].Index)
            {
                Tabla.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void AjustarColumnas()
        {
            if (Tabla.Columns.Count > 0)
            {
                // "Seleccionar" ya tiene Width fijo (50)
                Tabla.Columns["CheckNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CheckNumber"].FillWeight = 10;

                Tabla.Columns["IssueDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["IssueDate"].FillWeight = 15;

                Tabla.Columns["BeneficiaryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["BeneficiaryName"].FillWeight = 25;

                Tabla.Columns["Concept"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Concept"].FillWeight = 25;

                Tabla.Columns["PrintedAmount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["PrintedAmount"].FillWeight = 10;

                Tabla.Columns["Period"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Period"].FillWeight = 10;

                Tabla.Columns["Location"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Location"].FillWeight = 10;

                Tabla.Columns["Status"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Status"].FillWeight = 10;

                Tabla.Columns["FileControl"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["FileControl"].FillWeight = 10;

                Tabla.Columns["CreatedBy"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CreatedBy"].FillWeight = 15;
            }
        }

        #endregion ConfigurarTabla
        #region CargarDatos
        private void CargarCheques()
        {
            try
            {
                chequesList = Ctrl_Checks.MostrarCheques(paginaActual, registrosPorPagina);
                MostrarChequesEnTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR CHEQUES: {ex.Message}", "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarChequesEnTabla()
        {
            Tabla.Rows.Clear();

            foreach (var cheque in chequesList)
            {
                string location = cheque.LocationId.HasValue
                    ? Ctrl_Locations.ObtenerNombreLocation(cheque.LocationId.Value)
                    : "N/A";

                string estadoNombre = Ctrl_CheckStatus.ObtenerNombreEstado(cheque.StatusId);

                string emitidoPor = "N/A";
                if (cheque.CreatedBy.HasValue && cheque.CreatedBy.Value > 0)
                {
                    string nombreUsuario = Ctrl_Users.ObtenerNombreCompletoPorId(cheque.CreatedBy.Value);
                    emitidoPor = string.IsNullOrWhiteSpace(nombreUsuario) ? "SIN USUARIO" : nombreUsuario;
                }

                Tabla.Rows.Add(
                    false, //  Seleccionar (checkbox desmarcado por defecto)
                    cheque.CheckId,
                    cheque.CheckNumber,
                    cheque.IssueDate.ToString("dd/MM/yyyy"),
                    cheque.BeneficiaryName,
                    cheque.Concept,
                    cheque.PrintedAmount.ToString("N2"),
                    cheque.Period,
                    location,
                    estadoNombre,
                    cheque.FileControl ?? "PENDIENTE", //CONTROL ARCHIVO
                    emitidoPor
                );
            }
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count > 0)
            {
                CargarDatosChequeSeleccionado();
            }
        }

        private void CargarDatosChequeSeleccionado()
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0) return;

                DataGridViewRow fila = Tabla.SelectedRows[0];
                int checkId = Convert.ToInt32(fila.Cells["CheckId"].Value);

                _chequeSeleccionado = chequesList.FirstOrDefault(c => c.CheckId == checkId);

                if (_chequeSeleccionado != null)
                {
                    Txt_NoCheque.Text = _chequeSeleccionado.CheckNumber;
                    Txt_Banco.Text = "BANCO BANRURAL";
                    DTP_Emision.Value = _chequeSeleccionado.IssueDate;
                    Txt_Periodo.Text = _chequeSeleccionado.Period;

                    // MOSTRAR NOMBRE DEL ESTADO
                    Txt_State.Text = Ctrl_CheckStatus.ObtenerNombreEstado(_chequeSeleccionado.StatusId);

                    Txt_Beneficiario.Text = _chequeSeleccionado.BeneficiaryName;

                    if (_chequeSeleccionado.LocationId.HasValue)
                        Txt_Location.Text = Ctrl_Locations.ObtenerNombreLocation(_chequeSeleccionado.LocationId.Value);
                    else
                        Txt_Location.Text = "N/A";

                    Txt_Concepto.Text = _chequeSeleccionado.Concept;
                    Txt_Observaciones.Text = _chequeSeleccionado.DetailDescription ?? "";
                    Txt_MontoTotal.Text = _chequeSeleccionado.Amount.ToString("N2");
                    Txt_ValorImpresoCheque.Text = _chequeSeleccionado.PrintedAmount.ToString("N2");
                    Txt_Exencion.Text = _chequeSeleccionado.Exemption.ToString("N2");
                    Txt_MontoSinITH.Text = _chequeSeleccionado.TaxFreeAmount.ToString("N2");
                    Txt_Alimentacion.Text = _chequeSeleccionado.FoodAllowance.ToString("N2");
                    Txt_IGSS.Text = _chequeSeleccionado.IGSS.ToString("N2");
                    Txt_ITH.Text = _chequeSeleccionado.WithholdingTax.ToString("N2");
                    Txt_RetencionISR.Text = _chequeSeleccionado.Retention.ToString("N2");
                    Txt_Bonificacion.Text = _chequeSeleccionado.Bonus.ToString("N2");
                    Txt_Descuentos.Text = _chequeSeleccionado.Discounts.ToString("N2");
                    Txt_Anticipos.Text = _chequeSeleccionado.Advances.ToString("N2");
                    Txt_Viaticos.Text = _chequeSeleccionado.Viaticos.ToString("N2");
                    Txt_Stamps.Text = _chequeSeleccionado.Stamps.ToString("N2");
                    Txt_OrdenCompra.Text = _chequeSeleccionado.PurchaseOrderNumber ?? "0";
                    Txt_Complemento.Text = _chequeSeleccionado.Complement ?? "N/A";
                    Txt_ValorLetras.Text = NumeroALetras(_chequeSeleccionado.PrintedAmount);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR CHEQUE: {ex.Message}", "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion CargarDatos
        #region Busqueda
        private string ObtenerFiltroFileControl()
        {
            if (Filtro3.SelectedItem == null)
                return null; // equivale a "TODOS"

            string valor = Filtro3.SelectedItem.ToString().Trim().ToUpper();

            if (valor == "PENDIENTE" || valor == "TRASLADADO" || valor == "RECIBIDO" || valor == "ARCHIVADO")
                return valor;

            return null; // "TODOS"
        }
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            ejecutaConsulta();
        }

        private void ejecutaConsulta()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string textoBusqueda = Txt_ValorBuscado.Text == "BUSCAR POR NO.CHEQUE, BENEFICIARIO, CONCEPTO..."
                    ? "" : Txt_ValorBuscado.Text.Trim();

                string periodo = Filtro1.SelectedItem?.ToString() == "TODOS"
                    ? "" : Filtro1.SelectedItem?.ToString();

                int? locationId = null;
                if (Filtro2.SelectedIndex > 0)
                {
                    var selectedItem = (KeyValuePair<int, string>)Filtro2.SelectedItem;
                    locationId = selectedItem.Key;
                }

                // Filtro de estado de cheque (StatusId) ya NO se usa aquí.
                int? statusId = null;

                DateTime? fechaInicio = CheckBox_FiltroFechas.Checked ? (DateTime?)DTP_FechaInicio.Value : null;
                DateTime? fechaFin = CheckBox_FiltroFechas.Checked ? (DateTime?)DTP_FechaFin.Value : null;

                // Rango de cheques
                string rangoInicio = null;
                string rangoFin = null;

                if (CheckBox_Rango.Checked)
                {
                    if (!string.IsNullOrWhiteSpace(Txt_Li.Text) &&
                        Txt_Li.Text != "DESDE (NO. CHEQUE)" &&
                        Txt_Li.ForeColor != Color.Gray)
                    {
                        rangoInicio = Txt_Li.Text.Trim();
                    }

                    if (!string.IsNullOrWhiteSpace(Txt_Fin.Text) &&
                        Txt_Fin.Text != "HASTA (NO. CHEQUE)" &&
                        Txt_Fin.ForeColor != Color.Gray)
                    {
                        rangoFin = Txt_Fin.Text.Trim();
                    }

                    if (rangoInicio != null && rangoFin != null)
                    {
                        bool inicioEsNumerico = long.TryParse(rangoInicio, out long numInicio);
                        bool finEsNumerico = long.TryParse(rangoFin, out long numFin);

                        if (inicioEsNumerico && finEsNumerico && numInicio > numFin)
                        {
                            MessageBox.Show(
                                "EL RANGO INICIAL NO PUEDE SER MAYOR QUE EL RANGO FINAL",
                                "VALIDACIÓN",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                            this.Cursor = Cursors.Default;
                            return;
                        }
                    }
                }

                // Filtro por CONTROL ARCHIVO (FileControl) - AHORA SE ENVÍA AL CONTROLADOR
                string filtroFileControl = ObtenerFiltroFileControl();

                // GUARDAR FILTROS
                _ultimoTextoBusqueda = textoBusqueda;
                _ultimoPeriodo = periodo;
                _ultimoLocationId = locationId;
                _ultimoStatusId = statusId;
                _ultimaFechaInicio = fechaInicio;
                _ultimaFechaFin = fechaFin;
                _ultimoRangoInicio = rangoInicio;
                _ultimoRangoFin = rangoFin;

                paginaActual = 1;

                // 1️Obtener página actual desde BD CON FILTRO FILECONTROL
                chequesList = Ctrl_Checks.BuscarCheques(
                    _ultimoTextoBusqueda,
                    _ultimoPeriodo,
                    _ultimoLocationId,
                    _ultimoStatusId,
                    _ultimaFechaInicio,
                    _ultimaFechaFin,
                    _ultimoRangoInicio,
                    _ultimoRangoFin,
                    paginaActual,
                    registrosPorPagina,
                    filtroFileControl  // AHORA SE ENVÍA AQUÍ
                );
                chequesList = AplicarExclusiones(chequesList);

                MostrarChequesEnTabla();

                // 2️Obtener lista COMPLETA filtrada (para total y exportar)
                _listaCompletaFiltrada = Ctrl_Checks.BuscarCheques(
                    textoBusqueda,
                    periodo,
                    locationId,
                    statusId,
                    fechaInicio,
                    fechaFin,
                    rangoInicio,
                    rangoFin,
                    1,
                    int.MaxValue,
                    filtroFileControl  //AHORA SE ENVÍA AQUÍ
                );
                _listaCompletaFiltrada = AplicarExclusiones(_listaCompletaFiltrada);

                // 3️El total de registros YA viene filtrado desde la BD
                totalRegistros = _listaCompletaFiltrada?.Count ?? 0;
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                ActualizarInfoPaginacion();
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL BUSCAR: {ex.Message}", "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Mdl_Checks> AplicarExclusiones(List<Mdl_Checks> lista)
        {
            if (lista == null) return lista;
            if (_mesesExcluidos.Count == 0 && _fechasExcluidas.Count == 0) return lista;

            return lista.Where(c =>
            {
                DateTime fecha = c.IssueDate.Date;

                bool mesExcluido = _mesesExcluidos.Any(m =>
                    m.Year == fecha.Year && m.Month == fecha.Month);
                if (mesExcluido) return false;

                bool diaExcluido = _fechasExcluidas.Any(f => f.Date == fecha);
                if (diaExcluido) return false;

                return true;
            }).ToList();
        }
        private void Btn_FiltrosFechas_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = new Frm_Checks_Reports_Filters_Date
                {
                    FechaInicioInicial = DTP_FechaInicio.Value.Date,
                    FechaFinInicial = DTP_FechaFin.Value.Date,
                    MesesExcluidosIniciales = new List<DateTime>(_mesesExcluidos),
                    FechasExcluidasIniciales = new List<DateTime>(_fechasExcluidas),
                    StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
                };

                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    DTP_FechaInicio.Value = frm.FechaInicioSeleccionada;
                    DTP_FechaFin.Value = frm.FechaFinSeleccionada;
                    _mesesExcluidos = frm.MesesExcluidosSeleccionados;
                    _fechasExcluidas = frm.FechasExcluidasSeleccionadas;
                    ActualizarResumenFiltroFecha();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir filtros de fechas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarResumenFiltroFecha()
        {
            int totalExclusiones = _mesesExcluidos.Count + _fechasExcluidas.Count;
            if (totalExclusiones == 0)
            {
                Lbl_ResumenExclusiones.Text = "";
                Lbl_ResumenExclusiones.Visible = false;
                return;
            }

            var partes = new List<string>();
            if (_mesesExcluidos.Count > 0)
                partes.Add($"{_mesesExcluidos.Count} mes(es)");
            if (_fechasExcluidas.Count > 0)
                partes.Add($"{_fechasExcluidas.Count} día(s)");

            Lbl_ResumenExclusiones.Text = $"Excluidos: {string.Join(" + ", partes)}";
            Lbl_ResumenExclusiones.Visible = true;
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR POR NO.CHEQUE, BENEFICIARIO, CONCEPTO...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 0;
            CheckBox_FiltroFechas.Checked = false;

            // NUEVO: Limpiar filtro de rango
            CheckBox_Rango.Checked = false;
            Txt_Li.Text = "DESDE (NO. CHEQUE)";
            Txt_Li.ForeColor = Color.Gray;
            Txt_Fin.Text = "HASTA (NO. CHEQUE)";
            Txt_Fin.ForeColor = Color.Gray;

            _ultimoTextoBusqueda = "";
            _ultimoPeriodo = "";
            _ultimoLocationId = null;
            _ultimoStatusId = null;
            _ultimaFechaInicio = null;
            _ultimaFechaFin = null;
            _ultimoRangoInicio = null;
            _ultimoRangoFin = null;
            _listaCompletaFiltrada = null;
            paginaActual = 1;

            _mesesExcluidos.Clear();
            _fechasExcluidas.Clear();
            ActualizarResumenFiltroFecha();

            CargarCheques();
            ActualizarInfoPaginacion();
        }
        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }

        #endregion Busqueda
        #region ToolStrip
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

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginas)
            {
                paginaActual = nuevaPagina;

                if (!string.IsNullOrEmpty(_ultimoTextoBusqueda) ||
                    !string.IsNullOrEmpty(_ultimoPeriodo) ||
                    _ultimoLocationId.HasValue ||
                    _ultimoStatusId.HasValue ||
                    _ultimaFechaInicio.HasValue ||
                    _ultimaFechaFin.HasValue ||
                    !string.IsNullOrEmpty(_ultimoRangoInicio) ||
                    !string.IsNullOrEmpty(_ultimoRangoFin))
                {
                    // Obtener el filtro FileControl actual
                    string filtroFileControl = ObtenerFiltroFileControl();

                    chequesList = Ctrl_Checks.BuscarCheques(
                        _ultimoTextoBusqueda,
                        _ultimoPeriodo,
                        _ultimoLocationId,
                        _ultimoStatusId,
                        _ultimaFechaInicio,
                        _ultimaFechaFin,
                        _ultimoRangoInicio,
                        _ultimoRangoFin,
                        paginaActual,
                        registrosPorPagina,
                        filtroFileControl  // ENVIAR FILECONTROL
                    );
                    MostrarChequesEnTabla();
                }
                else
                {
                    CargarCheques();
                }

                ActualizarInfoPaginacion();
            }
        }

        private void ActualizarInfoPaginacion()
        {
            if (string.IsNullOrEmpty(_ultimoTextoBusqueda) &&
                string.IsNullOrEmpty(_ultimoPeriodo) &&
                !_ultimoLocationId.HasValue &&
                !_ultimoStatusId.HasValue &&
                !_ultimaFechaInicio.HasValue &&
                !_ultimaFechaFin.HasValue &&
                totalRegistros == 0)
            {
                totalRegistros = Ctrl_Checks.ContarTotalCheques();
            }

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;

            int inicioRango = (paginaActual - 1) * registrosPorPagina + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            if (Lbl_Paginas != null)
            {
                if (totalRegistros == 0)
                {
                    Lbl_Paginas.Text = "NO HAY CHEQUES PARA MOSTRAR";
                }
                else
                {
                    Lbl_Paginas.Text = $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} CHEQUES";
                }
            }
        }
        #endregion ToolStrip
        #region ExportarExcel
        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TienePermiso("CHECKS_FILECONTROL_EXPORT"))
                {
                    MessageBox.Show("NO TIENES PERMISO PARA EXPORTAR REPORTES DE CONTROL DE ARCHIVO",
                                   "ACCESO DENEGADO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 1️ Obtener lista completa de cheques según filtros activos
                List<Mdl_Checks> todosLosCheques;

                if (!string.IsNullOrEmpty(_ultimoTextoBusqueda) ||
                    !string.IsNullOrEmpty(_ultimoPeriodo) ||
                    _ultimoLocationId.HasValue ||
                    _ultimoStatusId.HasValue ||
                    _ultimaFechaInicio.HasValue ||
                    _ultimaFechaFin.HasValue ||
                    !string.IsNullOrEmpty(_ultimoRangoInicio) ||
                    !string.IsNullOrEmpty(_ultimoRangoFin))
                {
                    // Obtener el filtro FileControl actual
                    string filtroFileControl = ObtenerFiltroFileControl();

                    todosLosCheques = Ctrl_Checks.BuscarCheques(
                        _ultimoTextoBusqueda,
                        _ultimoPeriodo,
                        _ultimoLocationId,
                        _ultimoStatusId,
                        _ultimaFechaInicio,
                        _ultimaFechaFin,
                        _ultimoRangoInicio,
                        _ultimoRangoFin,
                        1,
                        int.MaxValue,
                        filtroFileControl  // ENVIAR FILECONTROL
                    );
                    todosLosCheques = AplicarExclusiones(todosLosCheques);
                }
                else
                {
                    todosLosCheques = Ctrl_Checks.MostrarCheques(1, int.MaxValue);
                    todosLosCheques = AplicarExclusiones(todosLosCheques);
                }

                if (todosLosCheques == null || todosLosCheques.Count == 0)
                {
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR", "INFORMACIÓN",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 2️ Abrir formulario de configuración
                Frm_Checks_FileControl_ReportConfig frmConfig =
                    new Frm_Checks_FileControl_ReportConfig(todosLosCheques);

                DialogResult resultado = frmConfig.ShowDialog(this);

                if (resultado != DialogResult.OK)
                {
                    // Usuario canceló la exportación
                    return;
                }

                // 3 Obtener configuración seleccionada
                var config = frmConfig.ConfiguracionSeleccionada;

                // 4️ Diálogo para guardar archivo
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Reporte de Control de Archivo",
                    FileName = $"ReporteControlArchivo_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                this.Cursor = Cursors.WaitCursor;

                // 5️ Generar reporte con configuración personalizada
                GenerarReporteExcel(todosLosCheques, config, saveFileDialog.FileName);

                this.Cursor = Cursors.Default;

                var resultadoAbrir = MessageBox.Show(
                    "REPORTE EXPORTADO EXITOSAMENTE.\n\n¿DESEA ABRIR EL ARCHIVO AHORA?",
                    "EXPORTACIÓN EXITOSA",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (resultadoAbrir == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL EXPORTAR: {ex.Message}", "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Btn_ExportList_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TienePermiso("CHECKS_FILECONTROL_EXPORT"))
                {
                    MessageBox.Show("NO TIENES PERMISO PARA EXPORTAR CHEQUES",
                                   "ACCESO DENEGADO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<Mdl_Checks> todosLosCheques;

                if (!string.IsNullOrEmpty(_ultimoTextoBusqueda) ||
                    !string.IsNullOrEmpty(_ultimoPeriodo) ||
                    _ultimoLocationId.HasValue ||
                    _ultimoStatusId.HasValue ||
                    _ultimaFechaInicio.HasValue ||
                    _ultimaFechaFin.HasValue ||
                    !string.IsNullOrEmpty(_ultimoRangoInicio) ||
                    !string.IsNullOrEmpty(_ultimoRangoFin))
                {
                    // OBTENER FILTRO FILECONTROL ACTUAL
                    string filtroFileControl = ObtenerFiltroFileControl();

                    todosLosCheques = Ctrl_Checks.BuscarCheques(
                        _ultimoTextoBusqueda,
                        _ultimoPeriodo,
                        _ultimoLocationId,
                        _ultimoStatusId,
                        _ultimaFechaInicio,
                        _ultimaFechaFin,
                        _ultimoRangoInicio,
                        _ultimoRangoFin,
                        1,
                        int.MaxValue,
                        filtroFileControl  // ENVIAR FILTRO FILECONTROL
                    );
                    todosLosCheques = AplicarExclusiones(todosLosCheques);
                }
                else
                {
                    todosLosCheques = Ctrl_Checks.MostrarCheques(1, int.MaxValue);
                    todosLosCheques = AplicarExclusiones(todosLosCheques);
                }

                if (todosLosCheques == null || todosLosCheques.Count == 0)
                {
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR", "INFORMACIÓN",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Reporte de Cheques",
                    FileName = $"Cheques_Completo_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    var excelApp = new Excel.Application();
                    var workbook = excelApp.Workbooks.Add();
                    var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Cheques";

                    // Título
                    worksheet.Cells[1, 1] = "REPORTE COMPLETO DE CHEQUES";
                    worksheet.Range["A1:Z1"].Merge();
                    worksheet.Range["A1:Z1"].Font.Size = 16;
                    worksheet.Range["A1:Z1"].Font.Bold = true;
                    worksheet.Range["A1:Z1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Range["A1:Z1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    worksheet.Range["A1:Z1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                    // Información adicional
                    worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SECRON"}";
                    worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {todosLosCheques.Count}";

                    worksheet.Range["A2:A4"].Font.Size = 10;
                    worksheet.Range["A2:A4"].Font.Bold = true;

                    int headerRow = 6;

                    string[] headers = {
                "NO. CHEQUE", "FECHA EMISIÓN", "BENEFICIARIO", "CONCEPTO", "PERIODO", "SEDE", "BANCO",
                "MONTO TOTAL", "VALOR IMPRESO", "EXENCIÓN", "MONTO SIN ITH", "ALIMENTACIÓN",
                "IGSS", "ITH", "RETENCIÓN ISR", "BONIFICACIÓN", "DESCUENTOS", "ANTICIPOS",
                "VIÁTICOS", "TIMBRES", "NO. ORDEN COMPRA", "COMPLEMENTO", "ESTADO",
                "CONTROL ARCHIVO", "OBSERVACIONES","EMITIDO POR"
            };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[headerRow, i + 1] = headers[i];
                    }

                    var headerRange = worksheet.Range[$"A{headerRow}:Z{headerRow}"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Size = 11;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    int row = headerRow + 1;
                    foreach (var cheque in todosLosCheques)
                    {
                        string location = cheque.LocationId.HasValue
                            ? Ctrl_Locations.ObtenerNombreLocation(cheque.LocationId.Value)
                            : "N/A";

                        string estadoNombre = Ctrl_CheckStatus.ObtenerNombreEstado(cheque.StatusId);

                        // Nombre del usuario que emitió el cheque (CreatedBy)
                        string emitidoPor = "N/A";
                        if (cheque.CreatedBy.HasValue && cheque.CreatedBy.Value > 0)
                        {
                            string nombreUsuario = Ctrl_Users.ObtenerNombreCompletoPorId(cheque.CreatedBy.Value);
                            emitidoPor = string.IsNullOrWhiteSpace(nombreUsuario) ? "SIN USUARIO" : nombreUsuario;
                        }

                        worksheet.Cells[row, 1] = cheque.CheckNumber ?? "";
                        worksheet.Cells[row, 2] = cheque.IssueDate.ToString("dd/MM/yyyy");
                        worksheet.Cells[row, 3] = cheque.BeneficiaryName ?? "";
                        worksheet.Cells[row, 4] = cheque.Concept ?? "";
                        worksheet.Cells[row, 5] = cheque.Period ?? "";
                        worksheet.Cells[row, 6] = location;
                        worksheet.Cells[row, 7] = "BANRURAL";
                        worksheet.Cells[row, 8] = cheque.Amount.ToString("N2");
                        worksheet.Cells[row, 9] = cheque.PrintedAmount.ToString("N2");
                        worksheet.Cells[row, 10] = cheque.Exemption.ToString("N2");
                        worksheet.Cells[row, 11] = cheque.TaxFreeAmount.ToString("N2");
                        worksheet.Cells[row, 12] = cheque.FoodAllowance.ToString("N2");
                        worksheet.Cells[row, 13] = cheque.IGSS.ToString("N2");
                        worksheet.Cells[row, 14] = cheque.WithholdingTax.ToString("N2");
                        worksheet.Cells[row, 15] = cheque.Retention.ToString("N2");
                        worksheet.Cells[row, 16] = cheque.Bonus.ToString("N2");
                        worksheet.Cells[row, 17] = cheque.Discounts.ToString("N2");
                        worksheet.Cells[row, 18] = cheque.Advances.ToString("N2");
                        worksheet.Cells[row, 19] = cheque.Viaticos.ToString("N2");
                        worksheet.Cells[row, 20] = cheque.Stamps.ToString("N2");
                        worksheet.Cells[row, 21] = cheque.PurchaseOrderNumber ?? "N/A";
                        worksheet.Cells[row, 22] = cheque.Complement ?? "N/A";
                        worksheet.Cells[row, 23] = estadoNombre;
                        worksheet.Cells[row, 24] = cheque.FileControl ?? "PENDIENTE";
                        worksheet.Cells[row, 25] = cheque.DetailDescription ?? "";
                        worksheet.Cells[row, 26] = emitidoPor;

                        if (row % 2 == 0)
                        {
                            worksheet.Range[$"A{row}:Z{row}"].Interior.Color =
                                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                        }

                        row++;
                    }

                    var dataRange = worksheet.Range[$"A{headerRow}:Z{row - 1}"];
                    dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                    worksheet.Columns.AutoFit();
                    worksheet.Columns[3].ColumnWidth = 35;  // Beneficiario
                    worksheet.Columns[4].ColumnWidth = 40;  // Concepto
                    worksheet.Columns[25].ColumnWidth = 50; // Observaciones
                    worksheet.Columns[26].ColumnWidth = 30; // Emitido por

                    worksheet.Activate();
                    excelApp.ActiveWindow.SplitRow = headerRow;
                    excelApp.ActiveWindow.FreezePanes = true;

                    worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
                    worksheet.Range[$"A{row + 1}:Z{row + 1}"].Merge();
                    worksheet.Range[$"A{row + 1}:Z{row + 1}"].Font.Italic = true;
                    worksheet.Range[$"A{row + 1}:Z{row + 1}"].Font.Size = 9;
                    worksheet.Range[$"A{row + 1}:Z{row + 1}"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    workbook.SaveAs(saveFileDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                    this.Cursor = Cursors.Default;

                    var result = MessageBox.Show(
                        "ARCHIVO EXPORTADO EXITOSAMENTE.\n\n¿DESEA ABRIR EL ARCHIVO AHORA?",
                        "EXPORTACIÓN EXITOSA",
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
                MessageBox.Show($"ERROR AL EXPORTAR: {ex.Message}", "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void GenerarReporteExcel(
        List<Mdl_Checks> cheques,
        Frm_Checks_FileControl_ReportConfig.ReportColumnConfig config,
        string rutaArchivo)
        {
            var excelApp = new Excel.Application();
            var workbook = excelApp.Workbooks.Add();
            var worksheet = (Excel.Worksheet)workbook.Sheets[1];
            worksheet.Name = "Control de Archivo";

            // ============ ENCABEZADO PRINCIPAL ============
            worksheet.Cells[1, 1] = "CONTROL DE CHEQUES - ARCHIVO";
            int totalColumnas = ContarColumnasSeleccionadas(config);
            string rangoTitulo = $"A1:{GetColumnName(totalColumnas)}1";

            worksheet.Range[rangoTitulo].Merge();
            worksheet.Range[rangoTitulo].Font.Size = 16;
            worksheet.Range[rangoTitulo].Font.Bold = true;
            worksheet.Range[rangoTitulo].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            worksheet.Range[rangoTitulo].Interior.Color =
                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
            worksheet.Range[rangoTitulo].Font.Color =
                System.Drawing.ColorTranslator.ToOle(Color.White);

            // ============ INFORMACIÓN DEL REPORTE ============
            worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SISTEMA"}";
            worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            worksheet.Cells[4, 1] = $"TOTAL CHEQUES: {cheques.Count}";

            worksheet.Range["A2:A4"].Font.Size = 10;
            worksheet.Range["A2:A4"].Font.Bold = true;

            // ============ AGRUPAR CHEQUES POR MES ============
            var chequesPorMes = cheques
                .GroupBy(c => new { c.IssueDate.Year, c.IssueDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToList();

            // ============ ENCABEZADOS DE COLUMNAS ============
            int headerRow = 6;
            int col = 1;

            if (config.IncluirMes)
            {
                worksheet.Cells[headerRow, col] = "MES";
                col++;
            }

            if (config.IncluirEmitidos)
            {
                worksheet.Cells[headerRow, col] = "CHEQUES EMITIDOS";
                col++;
            }

            if (config.IncluirPendientes)
            {
                worksheet.Cells[headerRow, col] = "PENDIENTES";
                col++;
            }

            if (config.IncluirPendientesPorcentaje)
            {
                worksheet.Cells[headerRow, col] = "PENDIENTES %";
                col++;
            }

            if (config.IncluirTrasladados)
            {
                worksheet.Cells[headerRow, col] = "TRASLADADOS";
                col++;
            }

            if (config.IncluirTrasladadosPorcentaje)
            {
                worksheet.Cells[headerRow, col] = "TRASLADADOS %";
                col++;
            }

            if (config.IncluirRecibidos)
            {
                worksheet.Cells[headerRow, col] = "RECIBIDOS";
                col++;
            }

            if (config.IncluirRecibidosPorcentaje)
            {
                worksheet.Cells[headerRow, col] = "RECIBIDOS %";
                col++;
            }

            if (config.IncluirArchivados)
            {
                worksheet.Cells[headerRow, col] = "ARCHIVADOS";
                col++;
            }

            if (config.IncluirArchivadosPorcentaje)
            {
                worksheet.Cells[headerRow, col] = "ARCHIVADOS %";
                col++;
            }

            // Estilo de encabezados
            string rangoHeader = $"A{headerRow}:{GetColumnName(totalColumnas)}{headerRow}";
            var headerRange = worksheet.Range[rangoHeader];
            headerRange.Font.Bold = true;
            headerRange.Font.Size = 11;
            headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
            headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(255, 192, 0));
            headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

            // ============ DATOS ============
            int row = headerRow + 1;

            foreach (var grupo in chequesPorMes)
            {
                col = 1;

                int totalEmitidos = grupo.Count();

                // LÓGICA CORREGIDA DE ESTADOS
                // PENDIENTE: Solo cuenta cuando está exactamente en estado PENDIENTE (no suma los demás)
                int pendientes = grupo.Count(c =>
                    string.IsNullOrWhiteSpace(c.FileControl) ||
                    c.FileControl == "PENDIENTE");

                // TRASLADADO: Cuenta TRASLADADO, RECIBIDO y ARCHIVADO
                int trasladados = grupo.Count(c =>
                    c.FileControl == "TRASLADADO" ||
                    c.FileControl == "RECIBIDO" ||
                    c.FileControl == "ARCHIVADO");

                // RECIBIDO: Cuenta RECIBIDO y ARCHIVADO
                int recibidos = grupo.Count(c =>
                    c.FileControl == "RECIBIDO" ||
                    c.FileControl == "ARCHIVADO");

                // ARCHIVADO: Solo cuenta ARCHIVADO
                int archivados = grupo.Count(c =>
                    c.FileControl == "ARCHIVADO");

                // Calcular porcentajes
                double porcPendientes = totalEmitidos > 0 ? (pendientes * 100.0 / totalEmitidos) : 0;
                double porcTrasladados = totalEmitidos > 0 ? (trasladados * 100.0 / totalEmitidos) : 0;
                double porcRecibidos = totalEmitidos > 0 ? (recibidos * 100.0 / totalEmitidos) : 0;
                double porcArchivados = totalEmitidos > 0 ? (archivados * 100.0 / totalEmitidos) : 0;

                // Nombre del mes
                string nombreMes = ObtenerNombreMes(grupo.Key.Month);

                // LLENAR FILA SEGÚN CONFIGURACIÓN EN EL ORDEN CORRECTO
                if (config.IncluirMes)
                {
                    worksheet.Cells[row, col] = nombreMes;
                    col++;
                }

                if (config.IncluirEmitidos)
                {
                    worksheet.Cells[row, col] = totalEmitidos;
                    col++;
                }

                if (config.IncluirPendientes)
                {
                    worksheet.Cells[row, col] = pendientes;
                    col++;
                }

                if (config.IncluirPendientesPorcentaje)
                {
                    worksheet.Cells[row, col] = $"{porcPendientes:F2}%";
                    col++;
                }

                if (config.IncluirTrasladados)
                {
                    worksheet.Cells[row, col] = trasladados;
                    col++;
                }

                if (config.IncluirTrasladadosPorcentaje)
                {
                    worksheet.Cells[row, col] = $"{porcTrasladados:F2}%";
                    col++;
                }

                if (config.IncluirRecibidos)
                {
                    worksheet.Cells[row, col] = recibidos;
                    col++;
                }

                if (config.IncluirRecibidosPorcentaje)
                {
                    worksheet.Cells[row, col] = $"{porcRecibidos:F2}%";
                    col++;
                }

                if (config.IncluirArchivados)
                {
                    worksheet.Cells[row, col] = archivados;
                    col++;
                }

                if (config.IncluirArchivadosPorcentaje)
                {
                    worksheet.Cells[row, col] = $"{porcArchivados:F2}%";
                    col++;
                }

                // Alternar color de filas
                if (row % 2 == 0)
                {
                    worksheet.Range[$"A{row}:{GetColumnName(totalColumnas)}{row}"].Interior.Color =
                        System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                }

                row++;
            }

            // ============ FORMATO FINAL ============
            var dataRange = worksheet.Range[$"A{headerRow}:{GetColumnName(totalColumnas)}{row - 1}"];
            dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

            worksheet.Columns.AutoFit();

            // Congelar paneles
            worksheet.Activate();
            excelApp.ActiveWindow.SplitRow = headerRow;
            excelApp.ActiveWindow.FreezePanes = true;

            // PIE DE PÁGINA
            worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
            worksheet.Range[$"A{row + 1}:{GetColumnName(totalColumnas)}{row + 1}"].Merge();
            worksheet.Range[$"A{row + 1}:{GetColumnName(totalColumnas)}{row + 1}"].Font.Italic = true;
            worksheet.Range[$"A{row + 1}:{GetColumnName(totalColumnas)}{row + 1}"].Font.Size = 9;
            worksheet.Range[$"A{row + 1}:{GetColumnName(totalColumnas)}{row + 1}"].HorizontalAlignment =
                Excel.XlHAlign.xlHAlignCenter;

            // Guardar y cerrar
            workbook.SaveAs(rutaArchivo);
            workbook.Close();
            excelApp.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }
        private string ObtenerNombreMes(int mes)
        {
            string[] meses = {
        "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO",
        "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
    };
            return meses[mes - 1];
        }

        private int ContarColumnasSeleccionadas(Frm_Checks_FileControl_ReportConfig.ReportColumnConfig config)
        {
            int count = 0;
            if (config.IncluirMes) count++;
            if (config.IncluirEmitidos) count++;
            if (config.IncluirTrasladados) count++;
            if (config.IncluirTrasladadosPorcentaje) count++;
            if (config.IncluirRecibidos) count++;
            if (config.IncluirRecibidosPorcentaje) count++;
            if (config.IncluirPendientes) count++;
            if (config.IncluirPendientesPorcentaje) count++;
            if (config.IncluirArchivados) count++;
            if (config.IncluirArchivadosPorcentaje) count++;
            return count;
        }

        private string GetColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }
        #endregion ExportarExcel
        #region NumeroALetras
        private string NumeroALetras(decimal numero)
        {
            if (numero == 0) return "CERO QUETZALES CON 00/100";

            long parteEntera = (long)Math.Floor(numero);
            int parteDecimal = (int)Math.Round((numero - parteEntera) * 100);

            if (parteDecimal > 99) parteDecimal = 99;

            string resultado = ConvertirEntero(parteEntera);
            resultado = resultado.Trim();
            resultado += $" QUETZALES CON {parteDecimal:00}/100";

            return resultado.ToUpper();
        }

        private string ConvertirEntero(long numero)
        {
            if (numero == 0) return "";

            string[] unidades = { "", "UNO", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE" };
            string[] decenas = { "", "DIEZ", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };
            string[] especiales = { "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISÉIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE" };
            string[] centenas = { "", "CIENTO", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS", "SEISCIENTOS", "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS" };

            string resultado = "";

            if (numero >= 1000000)
            {
                long millones = numero / 1000000;
                if (millones == 1)
                    resultado += "UN MILLÓN ";
                else
                    resultado += ConvertirGrupo((int)millones, unidades, decenas, especiales, centenas) + " MILLONES ";
                numero %= 1000000;
            }

            if (numero >= 1000)
            {
                long miles = numero / 1000;
                if (miles == 1)
                    resultado += "UN MIL ";
                else
                {
                    string grupoMiles = ConvertirGrupo((int)miles, unidades, decenas, especiales, centenas);
                    if (grupoMiles.EndsWith("UNO"))
                        grupoMiles = grupoMiles.Substring(0, grupoMiles.Length - 3) + "UN";
                    resultado += grupoMiles + " MIL ";
                }
                numero %= 1000;
            }

            if (numero > 0)
            {
                resultado += ConvertirGrupo((int)numero, unidades, decenas, especiales, centenas);
            }

            return resultado.Trim();
        }

        private string ConvertirGrupo(int numero, string[] unidades, string[] decenas, string[] especiales, string[] centenas)
        {
            string resultado = "";

            int centena = numero / 100;
            if (centena > 0)
            {
                if (centena == 1 && numero == 100)
                    resultado += "CIEN";
                else
                    resultado += centenas[centena];
            }

            numero %= 100;

            if (numero >= 10 && numero <= 19)
            {
                if (resultado.Length > 0) resultado += " ";
                resultado += especiales[numero - 10];
            }
            else if (numero >= 20 && numero <= 29)
            {
                if (resultado.Length > 0) resultado += " ";
                int unidad = numero % 10;
                if (unidad == 0)
                    resultado += "VEINTE";
                else
                    resultado += "VEINTI" + unidades[unidad];
            }
            else
            {
                int decena = numero / 10;
                int unidad = numero % 10;

                if (decena > 0)
                {
                    if (resultado.Length > 0) resultado += " ";
                    resultado += decenas[decena];
                }

                if (unidad > 0)
                {
                    if (decena > 0)
                        resultado += " Y ";
                    else if (resultado.Length > 0)
                        resultado += " ";
                    resultado += unidades[unidad];
                }
            }

            return resultado;
        }
        #endregion NumeroALetras
        #region vScrollBar
        private void Panel_Izquierdo_MouseEnter(object sender, EventArgs e)
        {
            Panel_Izquierdo.Focus();
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            int scrollPosition = vScrollBar.Value;

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
        #endregion vScrollBar
        #region ConfigurarSplitter
        private void ConfigurarSplitter()
        {
            Splitter.Dock = DockStyle.Left;
            Splitter.BackColor = Color.FromArgb(200, 200, 200);
            Splitter.Cursor = Cursors.VSplit;

            Panel_Izquierdo.Dock = DockStyle.Left;
            Panel_Derecho.Dock = DockStyle.Fill;

            Splitter.MouseEnter += (s, e) =>
            {
                Splitter.BackColor = Color.FromArgb(51, 140, 255);
            };

            Splitter.MouseLeave += (s, e) =>
            {
                Splitter.BackColor = Color.FromArgb(200, 200, 200);
            };

            Splitter.SplitterMoved += Splitter_SplitterMoved;
        }

        private void Splitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (toolStripPaginacion != null)
            {
                toolStripPaginacion.Location = new Point(this.Width - 300, 225);
            }

            this.Refresh();
        }
        #endregion ConfigurarSplitter
        #region ActualizarFileControl
        private bool EsTransicionValida(string estadoActual, string nuevoEstado)
        {
            estadoActual = (estadoActual ?? "PENDIENTE").ToUpper();
            nuevoEstado = nuevoEstado.ToUpper();

            // PROCESOTOTAL puede todo
            if (TienePermiso("CHECKS_FILECONTROL_PROCESOTOTAL"))
                return true;

            switch (estadoActual)
            {
                case "PENDIENTE":
                    return nuevoEstado == "TRASLADADO";

                case "TRASLADADO":
                    return nuevoEstado == "RECIBIDO";

                case "RECIBIDO":
                    return nuevoEstado == "ARCHIVADO";

                case "ARCHIVADO":
                    return false;

                default:
                    return false;
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // 1. Validar estado seleccionado en combo
                if (ComboBox_FileState.SelectedItem == null)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("DEBE SELECCIONAR UN ESTADO DE CONTROL DE ARCHIVO.",
                                   "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string nuevoEstado = ComboBox_FileState.SelectedItem.ToString();

                // VALIDAR PERMISOS SEGÚN ESTADO SELECCIONADO
                if (!TienePermisoParaCambiarEstado(nuevoEstado))
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(
                        $"NO TIENES PERMISO PARA CAMBIAR AL ESTADO: {nuevoEstado}\n\n" +
                        "CONTACTA AL ADMINISTRADOR SI NECESITAS ACCESO.",
                        "ACCESO DENEGADO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 2. Obtener cheques seleccionados en la tabla
                List<int> chequesSeleccionados = new List<int>();
                foreach (DataGridViewRow row in Tabla.Rows)
                {
                    bool marcado = row.Cells["Seleccionar"].Value != null &&
                                   (bool)row.Cells["Seleccionar"].Value == true;

                    if (marcado)
                    {
                        int checkId = Convert.ToInt32(row.Cells["CheckId"].Value);
                        chequesSeleccionados.Add(checkId);
                    }
                }

                // VALIDAR TRANSICIÓN DE ESTADOS (NEGOCIO)
                foreach (DataGridViewRow row in Tabla.Rows)
                {
                    bool marcado = row.Cells["Seleccionar"].Value != null &&
                                   (bool)row.Cells["Seleccionar"].Value == true;

                    if (!marcado) continue;

                    string estadoActual = row.Cells["FileControl"].Value?.ToString() ?? "PENDIENTE";

                    if (!EsTransicionValida(estadoActual, nuevoEstado))
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show(
                            $"TRANSICIÓN NO PERMITIDA:\n\n" +
                            $"ESTADO ACTUAL: {estadoActual}\n" +
                            $"NUEVO ESTADO: {nuevoEstado}\n\n" +
                            "REVISE LA SECUENCIA DEL CONTROL DE ARCHIVO.",
                            "VALIDACIÓN DE ESTADO",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }
                }

                // 3. Validaciones de cantidad
                if (chequesSeleccionados.Count == 0)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN CHEQUE.",
                                   "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (chequesSeleccionados.Count > 500)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(
                        "NO SE PUEDEN ACTUALIZAR MÁS DE 500 CHEQUES A LA VEZ.\n\n" +
                        $"CHEQUES SELECCIONADOS: {chequesSeleccionados.Count}\n" +
                        "LÍMITE MÁXIMO SUPERADO",
                        "VALIDACIÓN",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 4. Confirmación
                var confirmacion = MessageBox.Show(
                    $"¿DESEA ACTUALIZAR EL ESTADO DE CONTROL DE ARCHIVO A '{nuevoEstado}' " +
                    $"PARA {chequesSeleccionados.Count} CHEQUE(S)?",
                    "CONFIRMAR ACTUALIZACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                // 5. Procesar cambios
                int exitosos = 0;
                int fallidos = 0;

                int userId = UserData?.UserId ?? 0;

                foreach (int checkId in chequesSeleccionados)
                {
                    bool ok = Ctrl_Checks.ActualizarFileControl(checkId, nuevoEstado, userId);
                    if (ok) exitosos++;
                    else fallidos++;
                }

                this.Cursor = Cursors.Default;

                // 6. Mostrar resultado
                string mensaje = $"PROCESO COMPLETADO:\n\n" +
                                 $"✅ Exitosos: {exitosos}\n" +
                                 $"❌ Fallidos: {fallidos}";

                MessageBox.Show(mensaje, "RESULTADO",
                                MessageBoxButtons.OK,
                                fallidos > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                // 7. Desmarcar todos los seleccionados
                foreach (DataGridViewRow row in Tabla.Rows)
                {
                    row.Cells["Seleccionar"].Value = false;
                }

                // 8. Recargar datos respetando filtros activos
                if (!string.IsNullOrEmpty(_ultimoTextoBusqueda) ||
                    !string.IsNullOrEmpty(_ultimoPeriodo) ||
                    _ultimoLocationId.HasValue ||
                    _ultimoStatusId.HasValue ||
                    _ultimaFechaInicio.HasValue ||
                    _ultimaFechaFin.HasValue ||
                    !string.IsNullOrEmpty(_ultimoRangoInicio) ||
                    !string.IsNullOrEmpty(_ultimoRangoFin))
                {
                    // Obtener el filtro FileControl actual
                    string filtroFileControl = ObtenerFiltroFileControl();

                    chequesList = Ctrl_Checks.BuscarCheques(
                        _ultimoTextoBusqueda,
                        _ultimoPeriodo,
                        _ultimoLocationId,
                        _ultimoStatusId,
                        _ultimaFechaInicio,
                        _ultimaFechaFin,
                        _ultimoRangoInicio,
                        _ultimoRangoFin,
                        paginaActual,
                        registrosPorPagina,
                        filtroFileControl  //  ENVIAR FILECONTROL
                    );
                    chequesList = AplicarExclusiones(chequesList);
                    MostrarChequesEnTabla();
                }
                else
                {
                    CargarCheques();
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL ACTUALIZAR CONTROL DE ARCHIVO: {ex.Message}",
                               "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool TienePermisoParaCambiarEstado(string estado)
        {
            //CHK_036 = ACCESO TOTAL (puede cambiar a cualquier estado)
            if (TienePermiso("CHECKS_FILECONTROL_PROCESOTOTAL"))
                return true;

            //Validar permiso específico según estado
            switch (estado.ToUpper())
            {
                case "PENDIENTE":
                    return TienePermiso("CHECKS_FILECONTROL_PROCESOPENDIENTE"); // CHK_037

                case "TRASLADADO":
                    return TienePermiso("CHECKS_FILECONTROL_PROCESOTRASLADADO"); // CHK_038

                case "RECIBIDO":
                    return TienePermiso("CHECKS_FILECONTROL_PROCESORECIBIDO"); // CHK_039

                case "ARCHIVADO":
                    return TienePermiso("CHECKS_FILECONTROL_PROCESOARCHIVADO"); // CHK_040

                default:
                    return false;
            }
        }

        #endregion ActualizarFileControl

    }
}