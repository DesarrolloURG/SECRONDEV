using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_Checks_Reports : Form
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

        private async void Frm_Checks_Reports_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                ConfigurarComboBoxes();
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

        public Frm_Checks_Reports()
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
            // ⭐ CHECKS_REPORTS_UPDATE - Cambiar Estado (CHK_019)
            Btn_ChangeState.Enabled = TienePermiso("CHECKS_REPORTS_UPDATE");
            if (!Btn_ChangeState.Enabled)
            {
                Btn_ChangeState.BackColor = Color.FromArgb(200, 200, 200);
                Btn_ChangeState.ForeColor = Color.Gray;
                Btn_ChangeState.Cursor = Cursors.No;
            }

            // ⭐ CHECKS_REPORTS_UPDATENUMBER - Cambiar Correlativo (CHK_030)
            Btn_ChangeCheckNumber.Enabled = TienePermiso("CHECKS_REPORTS_UPDATENUMBER");
            if (!Btn_ChangeCheckNumber.Enabled)
            {
                Btn_ChangeCheckNumber.BackColor = Color.FromArgb(200, 200, 200);
                Btn_ChangeCheckNumber.ForeColor = Color.Gray;
                Btn_ChangeCheckNumber.Cursor = Cursors.No;
            }

            // ⭐ CHECKS_REPORTS_CREATEPREDECLARATION - Predeclaración (CHK_031)
            Btn_Predeclaration.Enabled = TienePermiso("CHECKS_REPORTS_CREATEPREDECLARATION");
            if (!Btn_Predeclaration.Enabled)
            {
                Btn_Predeclaration.BackColor = Color.FromArgb(200, 200, 200);
                Btn_Predeclaration.ForeColor = Color.Gray;
                Btn_Predeclaration.Cursor = Cursors.No;
            }

            // ⭐ CHECKS_REPORTS_EXPORT - Exportar (CHK_021)
            Btn_Export.Enabled = TienePermiso("CHECKS_REPORTS_EXPORT");
            if (!Btn_Export.Enabled)
            {
                Btn_Export.BackColor = Color.FromArgb(200, 200, 200);
                Btn_Export.ForeColor = Color.Gray;
                Btn_Export.Cursor = Cursors.No;
            }

            // ⭐ CHECKS_REPORTS_BASEFLUJO - Exportar Base de Flujo (NUEVO)
            Btn_ExportBaseFlujo.Enabled = TienePermiso("CHECKS_REPORTS_BASEFLUJO");
            if (!Btn_ExportBaseFlujo.Enabled)
            {
                Btn_ExportBaseFlujo.BackColor = Color.FromArgb(200, 200, 200);
                Btn_ExportBaseFlujo.ForeColor = Color.Gray;
                Btn_ExportBaseFlujo.Cursor = Cursors.No;
            }
        }
        #endregion SistemaDePermisos
        #region ConfigurarComboBoxes
        private void ConfigurarComboBoxes()
        {
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            // ⭐ FILTRO 1 - PERIODOS
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

            // ⭐ FILTRO 2 - LOCATIONS (SEDES)
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

            // ⭐ FILTRO 3 - ESTADOS DE CHEQUES (DINÁMICO DESDE BD)
            var estados = Ctrl_CheckStatus.ObtenerEstadosParaCombo();
            Filtro3.Items.Clear();
            Filtro3.Items.Add(new KeyValuePair<int, string>(0, "TODOS LOS ESTADOS"));
            foreach (var estado in estados)
            {
                Filtro3.Items.Add(estado);
            }
            Filtro3.DisplayMember = "Value";
            Filtro3.ValueMember = "Key";
            Filtro3.SelectedIndex = 0;
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
            Tabla.Columns.Add("CheckId", "ID");
            Tabla.Columns.Add("CheckNumber", "NO. CHEQUE");
            Tabla.Columns.Add("IssueDate", "FECHA EMISIÓN");
            Tabla.Columns.Add("BeneficiaryName", "BENEFICIARIO");
            Tabla.Columns.Add("Concept", "CONCEPTO");
            Tabla.Columns.Add("PrintedAmount", "VALOR IMPRESO");
            Tabla.Columns.Add("Period", "PERIODO");
            Tabla.Columns.Add("Location", "SEDE");
            Tabla.Columns.Add("Status", "ESTADO");
            Tabla.Columns.Add("CreatedBy", "EMITIDO POR");
            Tabla.Columns.Add("Predeclared", "PREDECLARADO");

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToResizeRows = false;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

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
        }

        private void AjustarColumnas()
        {
            if (Tabla.Columns.Count > 0)
            {
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
                Tabla.Columns["CreatedBy"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CreatedBy"].FillWeight = 15;
                Tabla.Columns["Predeclared"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Predeclared"].FillWeight = 10;
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

                // OBTENER NOMBRE DEL ESTADO
                string estadoNombre = Ctrl_CheckStatus.ObtenerNombreEstado(cheque.StatusId);
                // Nombre del usuario que emitió el cheque (CreatedBy)
                string emitidoPor = "N/A";
                if (cheque.CreatedBy.HasValue && cheque.CreatedBy.Value > 0)
                {
                    string nombreUsuario = Ctrl_Users.ObtenerNombreCompletoPorId(cheque.CreatedBy.Value);
                    emitidoPor = string.IsNullOrWhiteSpace(nombreUsuario) ? "SIN USUARIO" : nombreUsuario;
                }

                Tabla.Rows.Add(
                    cheque.CheckId,
                    cheque.CheckNumber,
                    cheque.IssueDate.ToString("dd/MM/yyyy"),
                    cheque.BeneficiaryName,
                    cheque.Concept,
                    cheque.PrintedAmount.ToString("N2"),
                    cheque.Period,
                    location,
                    estadoNombre,
                    emitidoPor,
                    cheque.Predeclared ? "SI" : "NO"
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

                    // ⭐ MOSTRAR NOMBRE DEL ESTADO
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
        private void Btn_Search_Click(object sender, EventArgs e)
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

                int? statusId = null;
                if (Filtro3.SelectedIndex > 0)
                {
                    var selectedItem = (KeyValuePair<int, string>)Filtro3.SelectedItem;
                    statusId = selectedItem.Key;
                }

                DateTime? fechaInicio = CheckBox_FiltroFechas.Checked ? (DateTime?)DTP_FechaInicio.Value : null;
                DateTime? fechaFin = CheckBox_FiltroFechas.Checked ? (DateTime?)DTP_FechaFin.Value : null;

                // ⭐⭐⭐ NUEVO: Obtener valores del rango de cheques
                string rangoInicio = null;
                string rangoFin = null;

                if (CheckBox_Rango.Checked)
                {
                    // Obtener valor de Txt_Li
                    if (!string.IsNullOrWhiteSpace(Txt_Li.Text) &&
                        Txt_Li.Text != "DESDE (NO. CHEQUE)" &&
                        Txt_Li.ForeColor != Color.Gray)
                    {
                        rangoInicio = Txt_Li.Text.Trim();
                    }

                    // Obtener valor de Txt_Fin
                    if (!string.IsNullOrWhiteSpace(Txt_Fin.Text) &&
                        Txt_Fin.Text != "HASTA (NO. CHEQUE)" &&
                        Txt_Fin.ForeColor != Color.Gray)
                    {
                        rangoFin = Txt_Fin.Text.Trim();
                    }

                    // Validar que el rango sea válido (inicio <= fin) si ambos son numéricos
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

                // ⭐ GUARDAR FILTROS
                _ultimoTextoBusqueda = textoBusqueda;
                _ultimoPeriodo = periodo;
                _ultimoLocationId = locationId;
                _ultimoStatusId = statusId;
                _ultimaFechaInicio = fechaInicio;
                _ultimaFechaFin = fechaFin;
                _ultimoRangoInicio = rangoInicio;  // ⭐ NUEVO
                _ultimoRangoFin = rangoFin;        // ⭐ NUEVO

                paginaActual = 1;

                chequesList = Ctrl_Checks.BuscarCheques(
                        _ultimoTextoBusqueda,
                        _ultimoPeriodo,
                        _ultimoLocationId,
                        _ultimoStatusId,
                        _ultimaFechaInicio,
                        _ultimaFechaFin,
                        _ultimoRangoInicio,     // ⭐ AGREGAR ESTA LÍNEA
                        _ultimoRangoFin,        // ⭐ AGREGAR ESTA LÍNEA
                        paginaActual,
                        registrosPorPagina
                );

                MostrarChequesEnTabla();

                totalRegistros = Ctrl_Checks.ContarChequesFiltrados(
                    textoBusqueda,
                    periodo,
                    locationId,
                    statusId,
                    fechaInicio,
                    fechaFin,
                    rangoInicio,        // ⭐ NUEVO PARÁMETRO
                    rangoFin            // ⭐ NUEVO PARÁMETRO
                );

                // ⭐ Guardar lista completa filtrada para exportar
                _listaCompletaFiltrada = Ctrl_Checks.BuscarCheques(
                    textoBusqueda,
                    periodo,
                    locationId,
                    statusId,
                    fechaInicio,
                    fechaFin,
                    rangoInicio,        // ⭐ NUEVO PARÁMETRO
                    rangoFin,           // ⭐ NUEVO PARÁMETRO
                    1,
                    int.MaxValue
                );

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
        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR POR NO.CHEQUE, BENEFICIARIO, CONCEPTO...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 0;
            CheckBox_FiltroFechas.Checked = false;

            // ⭐⭐⭐ NUEVO: Limpiar filtro de rango
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
            _ultimoRangoInicio = null;      // ⭐ NUEVO
            _ultimoRangoFin = null;         // ⭐ NUEVO
            _listaCompletaFiltrada = null;

            paginaActual = 1;
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
                        registrosPorPagina
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
                if (!TienePermiso("CHECKS_REPORTS_EXPORT"))
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
                        int.MaxValue
                    );
                }
                else
                {
                    todosLosCheques = Ctrl_Checks.MostrarCheques(1, int.MaxValue);
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
                    worksheet.Range["A1:Z1"].Merge(); // Se amplía hasta la nueva última columna (Y)
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
                    "VIÁTICOS", "TIMBRES", "NO. ORDEN COMPRA", "COMPLEMENTO", "ESTADO", "OBSERVACIONES",
                    "EMITIDO POR", "PREDECLARADO"
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
                        worksheet.Cells[row, 24] = cheque.DetailDescription ?? "";
                        worksheet.Cells[row, 25] = emitidoPor;
                        worksheet.Cells[row, 26] = cheque.Predeclared ? "SI" : "NO";

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
                    worksheet.Columns[24].ColumnWidth = 50; // Observaciones
                    worksheet.Columns[25].ColumnWidth = 30; // Emitido por

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

        private void Btn_ExportBaseFlujo_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TienePermiso("CHECKS_REPORTS_BASEFLUJO"))
                {
                    MessageBox.Show("NO TIENES PERMISO PARA EXPORTAR BASE DE FLUJO",
                                   "ACCESO DENEGADO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<Mdl_Checks> todosLosCheques;

                // Obtener cheques con filtros o todos
                if (!string.IsNullOrEmpty(_ultimoTextoBusqueda) ||
                    !string.IsNullOrEmpty(_ultimoPeriodo) ||
                    _ultimoLocationId.HasValue ||
                    _ultimoStatusId.HasValue ||
                    _ultimaFechaInicio.HasValue ||
                    _ultimaFechaFin.HasValue ||
                    !string.IsNullOrEmpty(_ultimoRangoInicio) ||   // ⭐ AGREGAR
                    !string.IsNullOrEmpty(_ultimoRangoFin))        // ⭐ AGREGAR
                {
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
                        int.MaxValue
                    );
                }
                else
                {
                    todosLosCheques = Ctrl_Checks.MostrarCheques(1, int.MaxValue);
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
                    Title = "Exportar Base de Flujo de Cheques",
                    FileName = $"BaseDeFlujo_Cheques_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    var excelApp = new Excel.Application();
                    var workbook = excelApp.Workbooks.Add();
                    var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Base de Flujo";

                    // ⭐ TÍTULO
                    worksheet.Cells[1, 1] = "BASE DE FLUJO CHEQUES";
                    worksheet.Range["A1:K1"].Merge();
                    worksheet.Range["A1:K1"].Font.Size = 16;
                    worksheet.Range["A1:K1"].Font.Bold = true;
                    worksheet.Range["A1:K1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Range["A1:K1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    worksheet.Range["A1:K1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                    worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SISTEMA"}";
                    worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {todosLosCheques.Count}";

                    worksheet.Range["A2:A4"].Font.Size = 10;
                    worksheet.Range["A2:A4"].Font.Bold = true;

                    // ⭐ ENCABEZADOS (EN EL ORDEN EXACTO DE LA IMAGEN)
                    int headerRow = 6;
                    string[] headers = {
                "FECHA",                    // A
                "DESCRIPCIÓN",              // B
                "CHEQUE",                   // C
                "MONTO (Q)",               // D
                "SEDE",                     // E
                "NOMENCLATURA CONTABLE",   // F
                "CUENTA CONTABLE",         // G
                "PROVEEDOR",               // H
                "FACTURA",                 // I
                "VALOR DE FACTURA",        // J
                "EXENCIÓN DE IVA"          // K
            };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[headerRow, i + 1] = headers[i];
                    }

                    var headerRange = worksheet.Range[$"A{headerRow}:K{headerRow}"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Size = 11;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(255, 192, 0)); // Naranja como en la imagen
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    // ⭐ CUENTAS A EXCLUIR (las que siempre se repiten)
                    List<string> cuentasExcluidas = new List<string> { "1110204", "1120301", "1120302" };

                    // ⭐ LLENAR DATOS
                    int row = headerRow + 1;
                    foreach (var cheque in todosLosCheques)
                    {
                        string location = cheque.LocationId.HasValue
                            ? Ctrl_Locations.ObtenerNombreLocation(cheque.LocationId.Value)
                            : "N/A";

                        // ⭐ OBTENER LA CUENTA CONTABLE VARIABLE (excluyendo las repetidas)
                        string nomenclaturaContable = "";
                        string cuentaContable = "";

                        try
                        {
                            // 1. Buscar el EntryMasterId del cheque usando la tabla de vínculo
                            int entryMasterId = Ctrl_AccountingEntryChecks.BuscarIdPorCheque(cheque.CheckId);

                            if (entryMasterId > 0)
                            {
                                // 2. Obtener todos los detalles de la partida
                                var detalles = Ctrl_AccountingEntryDetails.MostrarDetallesPorPartida(entryMasterId);

                                // 3. Buscar la cuenta que NO esté en la lista de excluidas
                                foreach (var detalle in detalles)
                                {
                                    // Obtener el código de cuenta desde Ctrl_Accounts
                                    string codigoCuenta = Ctrl_Accounts.ObtenerCodigoCuenta(detalle.AccountId);

                                    // Si NO está en las excluidas, es la cuenta variable
                                    if (!cuentasExcluidas.Contains(codigoCuenta))
                                    {
                                        nomenclaturaContable = codigoCuenta;
                                        cuentaContable = Ctrl_Accounts.ObtenerNombreCuenta(detalle.AccountId);
                                        break; // Encontramos la cuenta, salimos del loop
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Si hay error, dejar vacío
                            System.Diagnostics.Debug.WriteLine($"Error al obtener cuenta para CheckId {cheque.CheckId}: {ex.Message}");
                        }

                        // Si no se encontró cuenta, dejar "N/A"
                        if (string.IsNullOrEmpty(nomenclaturaContable))
                        {
                            nomenclaturaContable = "N/A";
                            cuentaContable = "N/A";
                        }

                        // ⭐ LLENAR FILA
                        worksheet.Cells[row, 1] = cheque.IssueDate.ToString("dd/MM/yyyy");  // FECHA
                        worksheet.Cells[row, 2] = cheque.DetailDescription ?? "";                      // DESCRIPCIÓN
                        worksheet.Cells[row, 3] = cheque.CheckNumber ?? "";                  // CHEQUE
                        worksheet.Cells[row, 4] = cheque.PrintedAmount.ToString("N2");             // MONTO (Q)
                        worksheet.Cells[row, 5] = location;                                  // SEDE
                        worksheet.Cells[row, 6] = nomenclaturaContable;                      // NOMENCLATURA CONTABLE
                        worksheet.Cells[row, 7] = cuentaContable;                            // CUENTA CONTABLE
                        worksheet.Cells[row, 8] = cheque.BeneficiaryName ?? "";             // PROVEEDOR
                        worksheet.Cells[row, 9] = cheque.Bill ?? "N/A";                     // FACTURA
                        worksheet.Cells[row, 10] = cheque.Bill ?? "N/A";                    // VALOR DE FACTURA (mismo por ahora)
                        worksheet.Cells[row, 11] = cheque.Exemption.ToString("N2");         // EXENCIÓN DE IVA

                        // ⭐ COLOR ALTERNADO EN FILAS
                        if (row % 2 == 0)
                        {
                            worksheet.Range[$"A{row}:K{row}"].Interior.Color =
                                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                        }

                        row++;
                    }

                    // ⭐ BORDES
                    var dataRange = worksheet.Range[$"A{headerRow}:K{row - 1}"];
                    dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                    // ⭐ AJUSTAR COLUMNAS
                    worksheet.Columns.AutoFit();
                    worksheet.Columns[2].ColumnWidth = 50;  // DESCRIPCIÓN más ancha
                    worksheet.Columns[7].ColumnWidth = 45;  // CUENTA CONTABLE más ancha
                    worksheet.Columns[8].ColumnWidth = 35;  // PROVEEDOR

                    // ⭐ CONGELAR PANELES
                    worksheet.Activate();
                    excelApp.ActiveWindow.SplitRow = headerRow;
                    excelApp.ActiveWindow.FreezePanes = true;

                    // ⭐ PIE DE PÁGINA
                    worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
                    worksheet.Range[$"A{row + 1}:K{row + 1}"].Merge();
                    worksheet.Range[$"A{row + 1}:K{row + 1}"].Font.Italic = true;
                    worksheet.Range[$"A{row + 1}:K{row + 1}"].Font.Size = 9;
                    worksheet.Range[$"A{row + 1}:K{row + 1}"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    // ⭐ GUARDAR Y CERRAR
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
        #region ChangeState
        private void Btn_ChangeState_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                Frm_Checks_ChangeStatus frmChangeState = new Frm_Checks_ChangeStatus
                {
                    UserData = this.UserData
                };

                this.Cursor = Cursors.Default;

                DialogResult resultado = frmChangeState.ShowDialog(this);

                if (resultado == DialogResult.OK)
                {
                    MessageBox.Show(
                        "CAMBIOS DE ESTADO APLICADOS CORRECTAMENTE",
                        "ÉXITO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    paginaActual = 1;
                    CargarCheques();
                    ActualizarInfoPaginacion();
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL ABRIR CAMBIO DE ESTADO: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion ChangeState
        #region Correlativo
        private void Btn_ChangeCheckNumber_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                Frm_Checks_ChangeCorrelative frmChangeCorrelative = new Frm_Checks_ChangeCorrelative
                {
                    UserData = this.UserData
                };

                this.Cursor = Cursors.Default;

                DialogResult resultado = frmChangeCorrelative.ShowDialog(this);

                if (resultado == DialogResult.OK)
                {
                    MessageBox.Show(
                        "CORRELATIVO ACTUALIZADO CORRECTAMENTE",
                        "ÉXITO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    paginaActual = 1;
                    CargarCheques();
                    ActualizarInfoPaginacion();
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL ABRIR CAMBIO DE CORRELATIVO: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Correlativo
        #region Predeclaration
        private void Btn_Predeclaration_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                Frm_Checks_Predeclarations frmPredeclaration = new Frm_Checks_Predeclarations
                {
                    UserData = this.UserData
                };

                this.Cursor = Cursors.Default;

                DialogResult resultado = frmPredeclaration.ShowDialog(this);

                if (resultado == DialogResult.OK)
                {
                    MessageBox.Show(
                        "PREDECLARACIÓN GENERADA CORRECTAMENTE",
                        "ÉXITO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    paginaActual = 1;
                    CargarCheques();
                    ActualizarInfoPaginacion();
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL ABRIR PREDECLARACIÓN: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Predeclaration
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
        #region Portapepeles
        private void Btn_Copy_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Txt_Observaciones.Text))
                {
                    Clipboard.SetText(Txt_Observaciones.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al copiar al portapapeles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Portapepeles
    }
}