using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Checks_Predeclarations : Form
    {
        #region PropiedadesIniciales
        public Mdl_Security_UserInfo UserData { get; set; }

        // Variables para filtros activos
        private string _ultimoTextoBusqueda = "";
        private string _ultimoPeriodo = "";
        private int? _ultimoLocationId = null;
        private int? _ultimoStatusId = null;
        private DateTime? _ultimaFechaInicio = null;
        private DateTime? _ultimaFechaFin = null;
        // Lista para guardar IDs de cheques seleccionados
        private HashSet<int> _chequesSeleccionados = new HashSet<int>();

        // DLL IMPORT PARA ESQUINAS REDONDEADAS
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse);

        private List<Mdl_Checks> _chequesList = new List<Mdl_Checks>();

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(1200, 900);
            this.MinimumSize = new Size(1200, 900);
            this.MaximumSize = new Size(1200, 900);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }
        #endregion PropiedadesIniciales
        #region Constructor
        public Frm_Checks_Predeclarations()
        {
            InitializeComponent();
            ConfigurarTamañoFormulario();
        }

        private void Frm_Checks_Predeclarations_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarComboBoxes();
                ConfigurarDateTimePickers();
                ConfigurarPlaceHoldersTextbox();
                AplicarEstiloTextBoxTotales();
                ConfigurarTabla();
                CargarCheques();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL CARGAR FORMULARIO: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Constructor
        #region ConfigurarComboBoxes
        private void ConfigurarComboBoxes()
        {
            // FILTRO 1 - PERIODOS
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
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

            // FILTRO 2 - LOCATIONS
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
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

            // FILTRO 3 - ESTADOS
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;
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
        #region EstiloTextBoxTotales
        private void CrearTextBoxConPadding(TextBox textBox, bool deshabilitarCompletamente = true)
        {
            // Guardar información original
            Point ubicacionOriginal = textBox.Location;
            Size tamañoOriginal = textBox.Size;
            Control contenedorPadre = textBox.Parent;
            string nombreOriginal = textBox.Name;
            string textoOriginal = textBox.Text;

            // Crear Panel contenedor
            Panel panelContenedor = new Panel
            {
                Location = ubicacionOriginal,
                Size = new Size(tamañoOriginal.Width, Math.Max(tamañoOriginal.Height, 45)),
                BackColor = Color.FromArgb(238, 143, 109),
                BorderStyle = BorderStyle.None,
                Padding = new Padding(6, 6, 12, 8),
                Name = "Panel_" + nombreOriginal
            };

            // Aplicar esquinas redondeadas
            panelContenedor.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, panelContenedor.Width, panelContenedor.Height, 15, 15));

            // Configurar el TextBox
            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = Color.FromArgb(238, 143, 109);
            textBox.ForeColor = Color.Black;
            textBox.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            textBox.Dock = DockStyle.Fill;
            textBox.Text = textoOriginal;
            textBox.TextAlign = HorizontalAlignment.Right;

            // ⭐ CONFIGURACIÓN SEGÚN PARÁMETRO
            if (deshabilitarCompletamente)
            {
                textBox.Enabled = false;
                textBox.Cursor = Cursors.No;
            }
            else
            {
                textBox.Enabled = true;  // ⭐ HABILITADO PARA EDICIÓN
                textBox.ReadOnly = false; // ⭐ PERMITE ESCRIBIR
                textBox.Cursor = Cursors.IBeam; // ⭐ CURSOR DE TEXTO
            }

            // Forzar colores siempre
            textBox.EnabledChanged += (s, e) =>
            {
                textBox.ForeColor = Color.Black;
                textBox.BackColor = Color.FromArgb(238, 143, 109);
            };

            // Remover el TextBox original
            contenedorPadre.Controls.Remove(textBox);

            // Agregar al panel
            panelContenedor.Controls.Add(textBox);

            // Agregar el panel al contenedor original
            contenedorPadre.Controls.Add(panelContenedor);

            // Configurar eventos
            ConfigurarEventosPanel(panelContenedor, textBox);
        }

        // ⭐ MÉTODO QUE FALTABA
        private void ConfigurarEventosPanel(Panel panel, TextBox textBox)
        {
            // Efecto hover
            panel.MouseEnter += (s, e) =>
            {
                panel.BackColor = Color.FromArgb(238, 143, 109);
                textBox.BackColor = Color.FromArgb(238, 143, 109);
                textBox.ForeColor = Color.Black;
            };

            panel.MouseLeave += (s, e) =>
            {
                panel.BackColor = Color.FromArgb(238, 143, 109);
                textBox.BackColor = Color.FromArgb(238, 143, 109);
                textBox.ForeColor = Color.Black;
            };
        }

        // Método para aplicar a todos los TextBox de totales
        private void AplicarEstiloTextBoxTotales()
        {
            CrearTextBoxConPadding(Txt_Total, true);
        }
        #endregion EstiloTextBoxTotales
        #region ConfigurarTabla
        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();

            // ⭐ COLUMNA CHECKBOX PRIMERA
            DataGridViewCheckBoxColumn colCheck = new DataGridViewCheckBoxColumn
            {
                Name = "Seleccionar",
                HeaderText = "☑",
                Width = 50,
                ReadOnly = false
            };
            Tabla.Columns.Add(colCheck);

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

            // Ocultar columna ID
            Tabla.Columns["CheckId"].Visible = false;

            // ⭐ CONFIGURACIÓN PARA CHECKBOXES
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = true;
            Tabla.ReadOnly = false; // ⭐ IMPORTANTE: Permitir edición para checkboxes
            Tabla.AllowUserToResizeRows = false;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            // ⭐ ESTILOS VISUALES CON SOMBREADO
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Filas normales
            Tabla.DefaultCellStyle.BackColor = Color.White;
            Tabla.DefaultCellStyle.ForeColor = Color.Black;
            Tabla.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla.DefaultCellStyle.Font = new Font("Segoe UI", 9F);

            // ⭐ FILAS ALTERNADAS (SOMBREADO)
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            Tabla.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            Tabla.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            Tabla.RowTemplate.Height = 35;
            Tabla.ColumnHeadersHeight = 40;
            Tabla.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            AjustarColumnas();

            // ⭐ EVENTOS PARA CHECKBOXES
            Tabla.CellContentClick += Tabla_CellContentClick;
        }
        private void AjustarColumnas()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["CheckNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CheckNumber"].FillWeight = 10;
                Tabla.Columns["IssueDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["IssueDate"].FillWeight = 12;
                Tabla.Columns["BeneficiaryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["BeneficiaryName"].FillWeight = 22;
                Tabla.Columns["Concept"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Concept"].FillWeight = 22;
                Tabla.Columns["PrintedAmount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["PrintedAmount"].FillWeight = 10;
                Tabla.Columns["Period"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Period"].FillWeight = 8;
                Tabla.Columns["Location"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Location"].FillWeight = 8;
                Tabla.Columns["Status"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Status"].FillWeight = 8;
            }
        }
        private void Tabla_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == Tabla.Columns["Seleccionar"].Index)
            {
                Tabla.CommitEdit(DataGridViewDataErrorContexts.Commit);

                DataGridViewRow row = Tabla.Rows[e.RowIndex];
                int checkId = Convert.ToInt32(row.Cells["CheckId"].Value);
                bool isChecked = row.Cells["Seleccionar"].Value != null &&
                                (bool)row.Cells["Seleccionar"].Value;

                if (isChecked)
                {
                    _chequesSeleccionados.Add(checkId);
                }
                else
                {
                    _chequesSeleccionados.Remove(checkId);
                }

                ActualizarContadorYTotal();
            }
        }
        // Actualizar contador y total de seleccionados
        private void ActualizarContadorYTotal()
        {
            int count = 0;
            decimal totalSeleccionado = 0;

            foreach (DataGridViewRow row in Tabla.Rows)
            {
                if (row.Cells["Seleccionar"].Value != null &&
                    (bool)row.Cells["Seleccionar"].Value == true)
                {
                    count++;

                    // Sumar el monto del cheque seleccionado
                    if (row.Cells["PrintedAmount"].Value != null)
                    {
                        string valorStr = row.Cells["PrintedAmount"].Value.ToString();
                        if (decimal.TryParse(valorStr, out decimal valor))
                        {
                            totalSeleccionado += valor;
                        }
                    }
                }
            }

            // Actualizar label de cantidad
            if (Lbl_Paginas != null)
            {
                if (count == 0)
                {
                    Lbl_Paginas.Text = $"MOSTRANDO {_chequesList.Count} CHEQUES NO PREDECLARADOS";
                }
                else
                {
                    Lbl_Paginas.Text = $"SELECCIONADOS: {count} DE {_chequesList.Count} CHEQUES";
                }
            }

            // ⭐ Actualizar Txt_Total con el total de los seleccionados
            if (Txt_Total != null)
            {
                Txt_Total.Text = totalSeleccionado.ToString("Q #,##0.00");
            }
        }
        #endregion ConfigurarTabla
        #region CargarCheques
        private void CargarCheques()
        {
            try
            {
                Tabla.Rows.Clear();

                _chequesList = Ctrl_Checks.BuscarChequesNoPredeclarados(
                    _ultimoTextoBusqueda,
                    _ultimoPeriodo,
                    _ultimoLocationId,
                    _ultimoStatusId,
                    _ultimaFechaInicio,
                    _ultimaFechaFin
                );

                foreach (var cheque in _chequesList)
                {
                    string location = cheque.LocationId.HasValue
                        ? Ctrl_Locations.ObtenerNombreLocation(cheque.LocationId.Value)
                        : "N/A";

                    string estadoNombre = Ctrl_CheckStatus.ObtenerNombreEstado(cheque.StatusId);

                    // ⭐ VERIFICAR si este cheque estaba seleccionado
                    bool estaSeleccionado = _chequesSeleccionados.Contains(cheque.CheckId);

                    Tabla.Rows.Add(
                        estaSeleccionado, // ⭐ Restaurar estado del checkbox
                        cheque.CheckId,
                        cheque.CheckNumber,
                        cheque.IssueDate.ToString("dd/MM/yyyy"),
                        cheque.BeneficiaryName,
                        cheque.Concept,
                        cheque.PrintedAmount.ToString("N2"),
                        cheque.Period,
                        location,
                        estadoNombre
                    );
                }

                ActualizarContadorYTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR CHEQUES: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ActualizarLabelCantidad()
        {
            if (Lbl_Paginas != null)
            {
                if (_chequesList.Count == 0)
                {
                    Lbl_Paginas.Text = "NO HAY CHEQUES PENDIENTES DE PREDECLARACIÓN";
                }
                else
                {
                    Lbl_Paginas.Text = $"MOSTRANDO {_chequesList.Count} CHEQUES NO PREDECLARADOS";
                }
            }
        }
        #endregion CargarCheques
        #region Búsqueda
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

                _ultimoTextoBusqueda = textoBusqueda;
                _ultimoPeriodo = periodo;
                _ultimoLocationId = locationId;
                _ultimoStatusId = statusId;
                _ultimaFechaInicio = fechaInicio;
                _ultimaFechaFin = fechaFin;

                CargarCheques();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL BUSCAR: {ex.Message}", "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR POR NO.CHEQUE, BENEFICIARIO, CONCEPTO...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 0;
            CheckBox_FiltroFechas.Checked = false;

            _ultimoTextoBusqueda = "";
            _ultimoPeriodo = "";
            _ultimoLocationId = null;
            _ultimoStatusId = null;
            _ultimaFechaInicio = null;
            _ultimaFechaFin = null;

            CargarCheques();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }
        #endregion Búsqueda
        #region BotonesPredeclarar
        // PREDECLARAR SOLO LOS SELECCIONADOS
        private void Btn_YesSelected_Click(object sender, EventArgs e)
        {
            try
            {
                // Recolectar CheckIds seleccionados desde los checkboxes
                List<int> chequesSeleccionados = new List<int>();
                foreach (DataGridViewRow row in Tabla.Rows)
                {
                    if (row.Cells["Seleccionar"].Value != null &&
                        (bool)row.Cells["Seleccionar"].Value == true)
                    {
                        chequesSeleccionados.Add(Convert.ToInt32(row.Cells["CheckId"].Value));
                    }
                }

                if (chequesSeleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN CHEQUE",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿DESEA PREDECLARAR {chequesSeleccionados.Count} CHEQUE(S) SELECCIONADO(S)?",
                    "CONFIRMAR PREDECLARACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                this.Cursor = Cursors.WaitCursor;

                int exitosos = Ctrl_Checks.PredeclararSeleccionados(chequesSeleccionados, UserData.UserId);

                // Registrar auditoría
                foreach (int checkId in chequesSeleccionados)
                {
                    var cheque = _chequesList.FirstOrDefault(c => c.CheckId == checkId);
                    if (cheque != null)
                    {
                        Ctrl_Audit.RegistrarAccion(
                            UserData.UserId,
                            "PREDECLARACIÓN DE CHEQUE",
                            "Checks",
                            checkId,
                            $"CHEQUE NO. {cheque.CheckNumber} PREDECLARADO POR {UserData.Username.ToUpper()}"
                        );
                    }
                }

                this.Cursor = Cursors.Default;

                MessageBox.Show(
                    $"PREDECLARACIÓN COMPLETADA\n\nCHEQUES PREDECLARADOS: {exitosos}",
                    "ÉXITO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Limpiar selección y recargar
                _chequesSeleccionados.Clear();
                CargarCheques();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL PREDECLARAR SELECCIONADOS: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // PREDECLARAR TODOS
        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            try
            {
                if (_chequesList.Count == 0)
                {
                    MessageBox.Show("NO HAY CHEQUES PARA PREDECLARAR",
                        "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE PREDECLARAR TODOS LOS {_chequesList.Count} CHEQUE(S)?\n\n" +
                    "ESTA ACCIÓN AFECTARÁ A TODOS LOS CHEQUES NO PREDECLARADOS.",
                    "CONFIRMAR PREDECLARACIÓN MASIVA",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.No)
                    return;

                this.Cursor = Cursors.WaitCursor;

                int exitosos = Ctrl_Checks.PredeclararTodos(UserData.UserId);

                Ctrl_Audit.RegistrarAccion(
                    UserData.UserId,
                    "PREDECLARACIÓN MASIVA",
                    "Checks",
                    0,
                    $"PREDECLARACIÓN MASIVA DE {exitosos} CHEQUES POR {UserData.Username.ToUpper()}"
                );

                this.Cursor = Cursors.Default;

                MessageBox.Show(
                    $"PREDECLARACIÓN MASIVA COMPLETADA\n\nCHEQUES PREDECLARADOS: {exitosos}",
                    "ÉXITO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL PREDECLARAR TODOS: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // CANCELAR
        private void Btn_No_Click(object sender, EventArgs e)
        {
            var confirmacion = MessageBox.Show(
                "¿DESEA CANCELAR Y CERRAR SIN PREDECLARAR?",
                "CONFIRMAR",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
        private void Btn_LimpiarSeleccion_Click(object sender, EventArgs e)
        {
            _chequesSeleccionados.Clear();

            foreach (DataGridViewRow row in Tabla.Rows)
            {
                row.Cells["Seleccionar"].Value = false;
            }

            ActualizarContadorYTotal();

            MessageBox.Show("SELECCIÓN LIMPIADA CORRECTAMENTE",
                "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void Btn_SelectAll_Click(object sender, EventArgs e)
        {
            _chequesSeleccionados.Clear();

            foreach (DataGridViewRow row in Tabla.Rows)
            {
                row.Cells["Seleccionar"].Value = true;
            }

            ActualizarContadorYTotal();
        }
        #endregion BotonesPredeclarar
        #region TotalCheques
        // Calcula y muestra el total de la columna ValorImpreso en el Txt_Total
        private void CalcularTotalCheques()
        {
            try
            {
                decimal totalCheques = 0;

                // Recorrer todas las filas visibles del DataGridView
                foreach (DataGridViewRow row in Tabla.Rows)
                {
                    // Verificar que la fila no sea la fila de nuevos registros
                    if (!row.IsNewRow)
                    {
                        // Obtener el valor de la columna ValorImpreso (índice 4)
                        if (row.Cells["PrintedAmount"].Value != null)
                        {
                            // Convertir el valor a decimal y sumarlo
                            if (decimal.TryParse(row.Cells["PrintedAmount"].Value.ToString(), out decimal valor))
                            {
                                totalCheques += valor;
                            }
                        }
                    }
                }

                // Mostrar el total formateado en el TextBox
                // Formato: Q #,##0.00 (quetzales con 2 decimales)
                Txt_Total.Text = totalCheques.ToString("Q #,##0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CALCULAR TOTAL: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Txt_Total.Text = "Q 0.00";
            }
        }
        #endregion TotalCheques
        #region ExportarCSV
        /* LEGACY CÓDIGO PREVIO PARA EXPORTAR A CSV BANRURAL
         * // EXPORTAR CSV PARA BANRURAL (FORMATO ESPECÍFICO)
        private void Btn_CSV_Click(object sender, EventArgs e)
        {
            try
            {
                if (_chequesList.Count == 0)
                {
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR",
                        "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                    Title = "Exportar CSV para BANRURAL",
                    FileName = $"PREDECLARACION_{DateTime.Now:dd_MM_yyyy}_1.csv",
                    DefaultExt = "csv"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    try
                    {
                        // ESTRATEGIA: Crear archivo temporal primero, luego moverlo
                        // Esto evita problemas de manipulación que puedan dañar el formato
                        string tempPath = Path.Combine(Path.GetTempPath(), $"temp_banrural_{Guid.NewGuid()}.csv");

                        // PASO 1: Crear archivo con codificación ISO-8859-1 (Latin1) y terminadores CRLF
                        using (StreamWriter writer = new StreamWriter(tempPath, false, Encoding.GetEncoding("ISO-8859-1")))
                        {
                            // Establecer NewLine explícitamente a CRLF (Windows)
                            writer.NewLine = "\r\n";

                            // SIN ENCABEZADOS - SOLO DATOS
                            foreach (var cheque in _chequesList)
                            {
                                // Limpiar y preparar campos - SIN comillas, asteriscos, comas, ni caracteres especiales
                                string checkNumber = LimpiarTextoParaBANRURAL(cheque.CheckNumber ?? "");
                                string beneficiaryName = LimpiarTextoParaBANRURAL(cheque.BeneficiaryName ?? "");

                                // Formato de número sin separador de miles
                                // Si el número es entero, no agregar decimales
                                string amount;
                                if (cheque.PrintedAmount == Math.Floor(cheque.PrintedAmount))
                                {
                                    // Número entero: sin decimales
                                    amount = cheque.PrintedAmount.ToString("0", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    // Número decimal: con decimales necesarios (sin ceros innecesarios)
                                    amount = cheque.PrintedAmount.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
                                }

                                // Escribir línea CSV: NO_CHEQUE,BENEFICIARIO,MONTO
                                // SIN comillas - formato simple y limpio
                                writer.WriteLine($"{checkNumber},{beneficiaryName},{amount}");
                            }
                        }

                        // PASO 2: Mover el archivo temporal al destino final
                        // Esto asegura que el archivo quede "limpio" sin manipulación adicional
                        if (File.Exists(saveFileDialog.FileName))
                        {
                            File.Delete(saveFileDialog.FileName);
                        }
                        File.Move(tempPath, saveFileDialog.FileName);

                        this.Cursor = Cursors.Default;

                        MessageBox.Show(
                            $"ARCHIVO CSV PARA BANRURAL EXPORTADO EXITOSAMENTE\n\n" +
                            $"UBICACIÓN: {saveFileDialog.FileName}\n" +
                            $"REGISTROS EXPORTADOS: {_chequesList.Count}\n\n" +
                            $"FORMATO: ISO-8859-1 (Compatible con Banca Virtual BANRURAL)",
                            "EXPORTACIÓN EXITOSA",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Registrar auditoría
                        Ctrl_Audit.RegistrarAccion(
                            UserData.UserId,
                            "EXPORTACIÓN CSV BANRURAL",
                            "Checks",
                            0,
                            $"EXPORTACIÓN DE {_chequesList.Count} CHEQUES A CSV BANRURAL POR {UserData.Username.ToUpper()}"
                        );
                    }
                    catch (IOException ioEx)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show(
                            $"ERROR DE ENTRADA/SALIDA:\n\n{ioEx.Message}\n\n" +
                            $"Verifique que el archivo no esté abierto en otra aplicación.",
                            "ERROR DE ARCHIVO",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    catch (UnauthorizedAccessException uaEx)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show(
                            $"ERROR DE PERMISOS:\n\n{uaEx.Message}\n\n" +
                            $"Verifique que tenga permisos de escritura en la ubicación seleccionada.",
                            "ERROR DE PERMISOS",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL EXPORTAR CSV PARA BANRURAL: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                if (_chequesList.Count == 0)
                {
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR",
                        "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                    Title = "Exportar Cheques a Excel",
                    FileName = $"Cheques_NoPredeclarados_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    DefaultExt = "xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    try
                    {
                        // Crear archivo temporal primero
                        string tempPath = Path.Combine(Path.GetTempPath(), $"temp_export_{Guid.NewGuid()}.csv");

                        // Crear archivo con codificación ISO-8859-1 (Latin1) y terminadores CRLF
                        using (StreamWriter writer = new StreamWriter(tempPath, false, Encoding.GetEncoding("ISO-8859-1")))
                        {
                            // Establecer NewLine explícitamente a CRLF (Windows)
                            writer.NewLine = "\r\n";

                            // DATOS
                            foreach (var cheque in _chequesList)
                            {
                                // Limpiar y preparar campos - SIN comillas, asteriscos, comas, ni caracteres especiales
                                string checkNumber = LimpiarTextoParaBANRURAL(cheque.CheckNumber ?? "");
                                string beneficiaryName = LimpiarTextoParaBANRURAL(cheque.BeneficiaryName ?? "");

                                // Formato de número sin separador de miles
                                // Si el número es entero, no agregar decimales
                                string amount;
                                if (cheque.PrintedAmount == Math.Floor(cheque.PrintedAmount))
                                {
                                    // Número entero: sin decimales
                                    amount = cheque.PrintedAmount.ToString("0", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    // Número decimal: con decimales necesarios (sin ceros innecesarios)
                                    amount = cheque.PrintedAmount.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
                                }

                                // Escribir línea CSV: NO_CHEQUE,BENEFICIARIO,MONTO
                                writer.WriteLine($"{checkNumber},{beneficiaryName},{amount}");
                            }
                        }

                        // Mover el archivo temporal al destino final
                        if (File.Exists(saveFileDialog.FileName))
                        {
                            File.Delete(saveFileDialog.FileName);
                        }
                        File.Move(tempPath, saveFileDialog.FileName);

                        this.Cursor = Cursors.Default;

                        MessageBox.Show(
                            $"ARCHIVO EXCEL EXPORTADO EXITOSAMENTE\n\n" +
                            $"UBICACIÓN: {saveFileDialog.FileName}\n" +
                            $"REGISTROS EXPORTADOS: {_chequesList.Count}",
                            "EXPORTACIÓN EXITOSA",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Registrar auditoría
                        Ctrl_Audit.RegistrarAccion(
                            UserData.UserId,
                            "EXPORTACIÓN A EXCEL",
                            "Checks",
                            0,
                            $"EXPORTACIÓN DE {_chequesList.Count} CHEQUES A EXCEL POR {UserData.Username.ToUpper()}"
                        );
                    }
                    catch (IOException ioEx)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show(
                            $"ERROR DE ENTRADA/SALIDA:\n\n{ioEx.Message}\n\n" +
                            $"Verifique que el archivo no esté abierto en otra aplicación.",
                            "ERROR DE ARCHIVO",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    catch (UnauthorizedAccessException uaEx)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show(
                            $"ERROR DE PERMISOS:\n\n{uaEx.Message}\n\n" +
                            $"Verifique que tenga permisos de escritura en la ubicación seleccionada.",
                            "ERROR DE PERMISOS",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL EXPORTAR A EXCEL: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_CSV2_Click(object sender, EventArgs e)
        {
            try
            {
                if (_chequesList.Count == 0)
                {
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR",
                        "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                    Title = "Exportar Cheques No Predeclarados",
                    FileName = $"Cheques_NoPredeclarados_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    DefaultExt = "csv"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    try
                    {
                        // SOLUCIÓN: Usar Windows-1252 (ANSI) para compatibilidad con BANRURAL
                        // Este es el formato estándar de "archivo de valores separados por comas de Microsoft Excel (.csv)"
                        using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false, Encoding.GetEncoding(1252)))
                        {
                            // SIN ENCABEZADOS - SOLO DATOS
                            foreach (var cheque in _chequesList)
                            {
                                // Escapar valores que contienen comas, comillas o saltos de línea
                                string checkNumber = EscaparCampoCSV(cheque.CheckNumber ?? "");
                                string beneficiaryName = EscaparCampoCSV(cheque.BeneficiaryName ?? "");

                                // Formato de número con punto como decimal (estándar CSV)
                                string amount = cheque.PrintedAmount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

                                // Escribir línea CSV
                                writer.WriteLine($"{checkNumber},{beneficiaryName},{amount}");
                            }
                        }

                        this.Cursor = Cursors.Default;

                        MessageBox.Show(
                            $"ARCHIVO CSV EXPORTADO EXITOSAMENTE\n\n" +
                            $"UBICACIÓN: {saveFileDialog.FileName}\n" +
                            $"REGISTROS EXPORTADOS: {_chequesList.Count}",
                            "EXPORTACIÓN EXITOSA",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (IOException ioEx)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show(
                            $"ERROR DE ENTRADA/SALIDA:\n\n{ioEx.Message}\n\n" +
                            $"Verifique que el archivo no esté abierto en otra aplicación.",
                            "ERROR DE ARCHIVO",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL EXPORTAR: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
         * 
         * */
        // EXPORTAR A EXCEL NORMAL (.xlsx - mismo formato que CSV BANRURAL)

        // EXPORTAR CSV PARA BANRURAL (FORMATO ESPECÍFICO)
        private void Btn_CSV_Click(object sender, EventArgs e)
        {
            try
            {
                List<Mdl_Checks> chequesAExportar = ObtenerChequesSeleccionados();

                if (chequesAExportar.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN CHEQUE PARA EXPORTAR",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                    Title = "Exportar CSV para BANRURAL (Seleccionados)",
                    FileName = $"PREDECLARACION_{DateTime.Now:dd_MM_yyyy}_1.csv",
                    DefaultExt = "csv"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    try
                    {
                        string tempPath = Path.Combine(Path.GetTempPath(), $"temp_banrural_{Guid.NewGuid()}.csv");

                        using (StreamWriter writer = new StreamWriter(tempPath, false, Encoding.GetEncoding("ISO-8859-1")))
                        {
                            writer.NewLine = "\r\n";

                            foreach (var cheque in chequesAExportar)
                            {
                                string checkNumber = LimpiarTextoParaBANRURAL(cheque.CheckNumber ?? "");
                                string beneficiaryName = LimpiarTextoParaBANRURAL(cheque.BeneficiaryName ?? "");

                                string amount;
                                if (cheque.PrintedAmount == Math.Floor(cheque.PrintedAmount))
                                {
                                    amount = cheque.PrintedAmount.ToString("0", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    amount = cheque.PrintedAmount.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
                                }

                                writer.WriteLine($"{checkNumber},{beneficiaryName},{amount}");
                            }
                        }

                        if (File.Exists(saveFileDialog.FileName))
                        {
                            File.Delete(saveFileDialog.FileName);
                        }
                        File.Move(tempPath, saveFileDialog.FileName);

                        this.Cursor = Cursors.Default;

                        MessageBox.Show(
                            $"ARCHIVO CSV PARA BANRURAL EXPORTADO EXITOSAMENTE\n\n" +
                            $"UBICACIÓN: {saveFileDialog.FileName}\n" +
                            $"REGISTROS EXPORTADOS: {chequesAExportar.Count}\n\n" +
                            $"FORMATO: ISO-8859-1 (Compatible con Banca Virtual BANRURAL)",
                            "EXPORTACIÓN EXITOSA",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        Ctrl_Audit.RegistrarAccion(
                            UserData.UserId,
                            "EXPORTACIÓN CSV BANRURAL",
                            "Checks",
                            0,
                            $"EXPORTACIÓN DE {chequesAExportar.Count} CHEQUES SELECCIONADOS A CSV BANRURAL POR {UserData.Username.ToUpper()}"
                        );
                    }
                    catch (IOException ioEx)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show(
                            $"ERROR DE ENTRADA/SALIDA:\n\n{ioEx.Message}\n\n" +
                            $"Verifique que el archivo no esté abierto en otra aplicación.",
                            "ERROR DE ARCHIVO",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    catch (UnauthorizedAccessException uaEx)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show(
                            $"ERROR DE PERMISOS:\n\n{uaEx.Message}\n\n" +
                            $"Verifique que tenga permisos de escritura en la ubicación seleccionada.",
                            "ERROR DE PERMISOS",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL EXPORTAR CSV PARA BANRURAL: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ⭐ NUEVO: Método para obtener solo los cheques seleccionados
        private List<Mdl_Checks> ObtenerChequesSeleccionados()
        {
            List<Mdl_Checks> seleccionados = new List<Mdl_Checks>();

            foreach (DataGridViewRow row in Tabla.Rows)
            {
                if (row.Cells["Seleccionar"].Value != null &&
                    (bool)row.Cells["Seleccionar"].Value == true)
                {
                    int checkId = Convert.ToInt32(row.Cells["CheckId"].Value);
                    var cheque = _chequesList.FirstOrDefault(c => c.CheckId == checkId);
                    if (cheque != null)
                    {
                        seleccionados.Add(cheque);
                    }
                }
            }

            return seleccionados;
        }

        // Limpia texto eliminando caracteres especiales problemáticos para BANRURAL
        private string LimpiarTextoParaBANRURAL(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return "";

            // Eliminar caracteres problemáticos que rompen el formato CSV
            texto = texto.Replace("\"", "");  // Comillas dobles
            texto = texto.Replace("*", "");   // Asteriscos
            texto = texto.Replace("'", "");   // Comillas simples
            texto = texto.Replace("`", "");   // Acento grave
            texto = texto.Replace("´", "");   // Acento agudo
            texto = texto.Replace(",", "");   // ⭐ COMAS - CRÍTICO para evitar columnas adicionales
            texto = texto.Replace("\r", " "); // Retornos de carro
            texto = texto.Replace("\n", " "); // Saltos de línea
            texto = texto.Replace("\t", " "); // Tabulaciones

            // Reemplazar múltiples espacios por uno solo
            while (texto.Contains("  "))
            {
                texto = texto.Replace("  ", " ");
            }

            // Trim para eliminar espacios al inicio y final
            return texto.Trim();
        }

        // Escapa correctamente un campo para formato CSV (para Excel)
        // Agrega comillas si el campo contiene: comas, comillas dobles o saltos de línea
        private string EscaparCampoCSV(string campo)
        {
            if (string.IsNullOrEmpty(campo))
                return "";

            // Si contiene coma, comillas o salto de línea, debe ir entre comillas
            if (campo.Contains(",") || campo.Contains("\"") || campo.Contains("\n") || campo.Contains("\r"))
            {
                // Duplicar las comillas internas según estándar CSV
                campo = campo.Replace("\"", "\"\"");
                return $"\"{campo}\"";
            }

            return campo;
        }

        #endregion ExportarCSV
    }
}