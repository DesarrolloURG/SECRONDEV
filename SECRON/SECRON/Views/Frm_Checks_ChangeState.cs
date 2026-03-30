using SECRON.Configuration;
using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Checks_ChangeStatus : Form
    {
        #region PropiedadesIniciales
        public Mdl_Security_UserInfo UserData { get; set; }

        // ⭐ Variables para filtros activos (igual que en Reports)
        private string _ultimoTextoBusqueda = "";
        private string _ultimoPeriodo = "";
        private int? _ultimoLocationId = null;
        private int? _ultimoStatusId = null;
        private DateTime? _ultimaFechaInicio = null;
        private DateTime? _ultimaFechaFin = null;
        private string _ultimoRangoInicio = null;
        private string _ultimoRangoFin = null;
        // Lista para guardar IDs de cheques seleccionados
        private HashSet<int> _chequesSeleccionados = new HashSet<int>();
        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(1200, 900);              // Tamaño fijo (ajusta según tu diseño)
            this.MinimumSize = new Size(1200, 900);       // Tamaño mínimo
            this.MaximumSize = new Size(1200, 900);       // Tamaño máximo
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // No redimensionable
            this.StartPosition = FormStartPosition.CenterParent; // Centrado en el padre
            this.MaximizeBox = false;                     // Sin botón maximizar
        }
        #endregion PropiedadesIniciales
        #region Constructor
        public Frm_Checks_ChangeStatus()
        {
            InitializeComponent();
            ConfigurarTamañoFormulario();
        }

        private void Frm_Checks_ChangeState_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarComboBoxes();
                ConfigurarDateTimePickers();
                ConfigurarPlaceHoldersTextbox();
                ConfigurarRangoCheques();
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
            // ⭐ FILTRO 1 - PERIODOS (igual que Reports)
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

            // ⭐ FILTRO 2 - LOCATIONS (SEDES)
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

            // ⭐ FILTRO 3 - ESTADOS DE CHEQUES
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

            // ⭐ COMBO DE ESTADO PARA CAMBIAR (el que ya tenías)
            ComboBox_Estado.DropDownStyle = ComboBoxStyle.DropDownList;
            var estadosCambio = Ctrl_CheckStatus.ObtenerEstadosParaCombo();
            ComboBox_Estado.DataSource = new BindingSource(estadosCambio, null);
            ComboBox_Estado.DisplayMember = "Value";
            ComboBox_Estado.ValueMember = "Key";

            if (ComboBox_Estado.Items.Count > 0)
                ComboBox_Estado.SelectedIndex = 0;
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
        #region ConfigurarRangoCheques
        private void ConfigurarRangoCheques()
        {
            CheckBox_Rango.Checked = false;
            CheckBox_Rango.CheckedChanged += CheckBox_Rango_CheckedChanged;

            Txt_Li.Enabled = false;
            ConfigurarPlaceHolder(Txt_Li, "INICIO");
            Txt_Li.BackColor = Color.White;
            Txt_Li.ForeColor = Color.Black;
            Txt_Li.KeyPress += Txt_RangoCheque_KeyPress;

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
            if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

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
        #region ConfigurarTabla
        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();

            // ⭐ Columna Checkbox PRIMERA
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
            Tabla.Columns.Add("BeneficiaryName", "BENEFICIARIO");
            Tabla.Columns.Add("Amount", "MONTO");
            Tabla.Columns.Add("IssueDate", "FECHA EMISIÓN");
            Tabla.Columns.Add("StatusName", "ESTADO ACTUAL");
            Tabla.Columns.Add("Concept", "CONCEPTO");

            // Ocultar columna ID
            Tabla.Columns["CheckId"].Visible = false;

            // Configuración de comportamiento
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = true;
            Tabla.ReadOnly = false;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            // ⭐⭐⭐ ESTILOS VISUALES CON SOMBREADO ⭐⭐⭐

            // Header (encabezado)
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Filas normales
            Tabla.DefaultCellStyle.BackColor = Color.White; // ⭐ Fondo blanco para filas normales
            Tabla.DefaultCellStyle.ForeColor = Color.Black;
            Tabla.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla.DefaultCellStyle.Font = new Font("Segoe UI", 9F);

            // ⭐⭐⭐ FILAS ALTERNADAS (SOMBREADO) ⭐⭐⭐
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240); // Gris claro
            Tabla.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            Tabla.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Altura de filas
            Tabla.RowTemplate.Height = 35;
            Tabla.ColumnHeadersHeight = 40;

            // Ajustar anchos
            Tabla.Columns["CheckNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CheckNumber"].FillWeight = 10;
            Tabla.Columns["BeneficiaryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["BeneficiaryName"].FillWeight = 25;
            Tabla.Columns["Amount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["Amount"].FillWeight = 10;
            Tabla.Columns["IssueDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["IssueDate"].FillWeight = 15;
            Tabla.Columns["StatusName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["StatusName"].FillWeight = 15;
            Tabla.Columns["Concept"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["Concept"].FillWeight = 25;

            // ⭐ Eventos
            Tabla.CellContentClick += Tabla_CellContentClick;
        }

        // Evento para manejar clicks en checkboxes
        private void Tabla_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == Tabla.Columns["Seleccionar"].Index)
            {
                Tabla.CommitEdit(DataGridViewDataErrorContexts.Commit);

                // ⭐ NUEVO: Guardar o quitar el cheque de la lista de seleccionados
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

                ActualizarContador();
            }
        }

        // ⭐ NUEVO: Contador de seleccionados
        private void ActualizarContador()
        {
            int count = 0;
            foreach (DataGridViewRow row in Tabla.Rows)
            {
                if (row.Cells["Seleccionar"].Value != null &&
                    (bool)row.Cells["Seleccionar"].Value == true)
                {
                    count++;
                }
            }

            if (Lbl_Info != null)
            {
                Lbl_Info.Text = $"CHEQUES SELECCIONADOS: {count}";
            }
        }
        #endregion ConfigurarTabla
        #region CargarCheques
        private void CargarCheques()
        {
            try
            {
                Tabla.Rows.Clear();

                // ⭐ CARGAR TODOS LOS CHEQUES (sin paginación)
                List<Mdl_Checks> cheques = Ctrl_Checks.MostrarCheques(1, int.MaxValue);

                foreach (var cheque in cheques)
                {
                    string estadoNombre = Ctrl_CheckStatus.ObtenerNombreEstado(cheque.StatusId);

                    // ⭐ VERIFICAR si este cheque estaba seleccionado
                    bool estaSeleccionado = _chequesSeleccionados.Contains(cheque.CheckId);

                    Tabla.Rows.Add(
                        estaSeleccionado, // ⭐ Restaurar estado del checkbox
                        cheque.CheckId,
                        cheque.CheckNumber,
                        cheque.BeneficiaryName,
                        cheque.Amount.ToString("N2"),
                        cheque.IssueDate.ToString("dd/MM/yyyy"),
                        estadoNombre,
                        cheque.Concept
                    );
                }

                // ⭐ Actualizar label con el total
                ActualizarTotalCheques();
                ActualizarContador(); // ⭐ AGREGAR ESTA LÍNEA

                if (Tabla.Rows.Count > 0)
                    Tabla.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR CHEQUES: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ⭐ NUEVO MÉTODO SIMPLE para actualizar el total
        private void ActualizarTotalCheques()
        {
            if (Lbl_Paginas != null)
            {
                if (Tabla.Rows.Count == 0)
                {
                    Lbl_Paginas.Text = "NO HAY CHEQUES PARA MOSTRAR";
                }
                else
                {
                    Lbl_Paginas.Text = $"TOTAL: {Tabla.Rows.Count} CHEQUE(S)";
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

                // ⭐ RANGO DE CHEQUES
                string rangoInicio = null;
                string rangoFin = null;

                if (CheckBox_Rango.Checked)
                {
                    if (!string.IsNullOrWhiteSpace(Txt_Li.Text) &&
                        Txt_Li.Text != "INICIO" &&
                        Txt_Li.ForeColor != Color.Gray)
                    {
                        rangoInicio = Txt_Li.Text.Trim();
                    }

                    if (!string.IsNullOrWhiteSpace(Txt_Fin.Text) &&
                        Txt_Fin.Text != "FIN" &&
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

                _ultimoTextoBusqueda = textoBusqueda;
                _ultimoPeriodo = periodo;
                _ultimoLocationId = locationId;
                _ultimoStatusId = statusId;
                _ultimaFechaInicio = fechaInicio;
                _ultimaFechaFin = fechaFin;
                _ultimoRangoInicio = rangoInicio;
                _ultimoRangoFin = rangoFin;

                BuscarConFiltros();
                ActualizarTotalCheques();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL BUSCAR: {ex.Message}", "ERROR",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BuscarConFiltros()
        {
            try
            {
                Tabla.Rows.Clear();

                // ⭐ BUSCAR TODOS LOS CHEQUES (sin paginación)
                List<Mdl_Checks> cheques = Ctrl_Checks.BuscarCheques(
                    _ultimoTextoBusqueda,
                    _ultimoPeriodo,
                    _ultimoLocationId,
                    _ultimoStatusId,
                    _ultimaFechaInicio,
                    _ultimaFechaFin,
                    _ultimoRangoInicio,
                    _ultimoRangoFin,
                    1,              // ⭐ Página 1
                    int.MaxValue    // ⭐ Todos los registros
                );

                foreach (var cheque in cheques)
                {
                    string estadoNombre = Ctrl_CheckStatus.ObtenerNombreEstado(cheque.StatusId);

                    // ⭐ VERIFICAR si este cheque estaba seleccionado
                    bool estaSeleccionado = _chequesSeleccionados.Contains(cheque.CheckId);

                    Tabla.Rows.Add(
                        estaSeleccionado, // ⭐ Restaurar estado del checkbox
                        cheque.CheckId,
                        cheque.CheckNumber,
                        cheque.BeneficiaryName,
                        cheque.Amount.ToString("N2"),
                        cheque.IssueDate.ToString("dd/MM/yyyy"),
                        estadoNombre,
                        cheque.Concept
                    );
                }

                ActualizarContador(); // ⭐ AGREGAR ESTA LÍNEA

                if (Tabla.Rows.Count > 0)
                    Tabla.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL BUSCAR CHEQUES: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // ⭐ LIMPIAR RANGO
            CheckBox_Rango.Checked = false;
            Txt_Li.Text = "INICIO";
            Txt_Li.ForeColor = Color.Gray;
            Txt_Fin.Text = "FIN";
            Txt_Fin.ForeColor = Color.Gray;

            _ultimoTextoBusqueda = "";
            _ultimoPeriodo = "";
            _ultimoLocationId = null;
            _ultimoStatusId = null;
            _ultimaFechaInicio = null;
            _ultimaFechaFin = null;
            _ultimoRangoInicio = null;
            _ultimoRangoFin = null;

            CargarCheques();
            ActualizarTotalCheques();
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
        #region AplicarCambios
        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Recolectar CheckIds seleccionados
                List<int> chequesSeleccionados = new List<int>();
                foreach (DataGridViewRow row in Tabla.Rows)
                {
                    if (row.Cells["Seleccionar"].Value != null &&
                        (bool)row.Cells["Seleccionar"].Value == true)
                    {
                        chequesSeleccionados.Add(Convert.ToInt32(row.Cells["CheckId"].Value));
                    }
                }

                // Validar selección
                if (chequesSeleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN CHEQUE",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    return;
                }

                if (ComboBox_Estado.SelectedValue == null)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN ESTADO",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    return;
                }

                int nuevoEstadoId = Convert.ToInt32(ComboBox_Estado.SelectedValue);
                string nuevoEstadoNombre = ComboBox_Estado.Text;

                // ⭐ VALIDACIÓN: No permitir anular más de 3 cheques a la vez
                if (chequesSeleccionados.Count > 3 && nuevoEstadoNombre.ToUpper() == "ANULADO")
                {
                    MessageBox.Show(
                        "NO SE PUEDE ANULAR MÁS DE 3 CHEQUES A LA VEZ.\n\n" +
                        $"CHEQUES SELECCIONADOS: {chequesSeleccionados.Count}\n" +
                        "LÍMITE MÁXIMO: 3",
                        "VALIDACIÓN",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿DESEA CAMBIAR EL ESTADO DE {chequesSeleccionados.Count} CHEQUE(S) A '{nuevoEstadoNombre}'?",
                    "CONFIRMAR CAMBIO",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                // ===== PROCESAR CAMBIOS =====
                int exitosos = 0;
                int fallidos = 0;
                StringBuilder errores = new StringBuilder();

                foreach (int checkId in chequesSeleccionados)
                {
                    Mdl_Checks cheque = Ctrl_Checks.ObtenerChequePorId(checkId);
                    if (cheque == null)
                    {
                        fallidos++;
                        errores.AppendLine($"- Cheque ID {checkId}: No encontrado");
                        continue;
                    }

                    bool resultado = false;

                    // Si el nuevo estado es ANULADO, usar proceso especial
                    if (nuevoEstadoNombre.ToUpper() == "ANULADO")
                    {
                        resultado = AnularCheque(cheque);
                    }
                    else
                    {
                        resultado = CambiarEstadoCheque(cheque, nuevoEstadoId, nuevoEstadoNombre);
                    }

                    if (resultado)
                    {
                        exitosos++;
                    }
                    else
                    {
                        fallidos++;
                        errores.AppendLine($"- Cheque NO. {cheque.CheckNumber}: Error al procesar");
                    }
                }

                this.Cursor = Cursors.Default;

                // Mensaje de resultado
                string mensaje = $"PROCESO COMPLETADO:\n\n" +
                                $"✅ Exitosos: {exitosos}\n" +
                                $"❌ Fallidos: {fallidos}";

                if (fallidos > 0)
                {
                    mensaje += $"\n\nERRORES:\n{errores}";
                }

                MessageBox.Show(mensaje, "RESULTADO",
                    MessageBoxButtons.OK,
                    fallidos > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                // Limpiar selección y recargar
                _chequesSeleccionados.Clear();
                CargarCheques();
                ActualizarContador();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL APLICAR CAMBIOS: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool CambiarEstadoCheque(Mdl_Checks cheque, int nuevoEstadoId, string nuevoEstadoNombre)
        {
            try
            {
                cheque.StatusId = nuevoEstadoId;
                cheque.ModifiedBy = UserData.UserId;

                if (Ctrl_Checks.ActualizarCheque(cheque) == 0)
                {
                    return false;
                }

                string estadoAnterior = Ctrl_CheckStatus.ObtenerNombreEstado(cheque.StatusId);
                string detalle = $"CAMBIO DE ESTADO DE CHEQUE NO. {cheque.CheckNumber}: " +
                                $"{estadoAnterior} → {nuevoEstadoNombre}";

                Ctrl_Audit.RegistrarAccion(
                    UserData.UserId,
                    "CAMBIO DE ESTADO",
                    "Checks",
                    cheque.CheckId,
                    detalle
                );

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR EN CHEQUE {cheque.CheckNumber}: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool AnularCheque(Mdl_Checks cheque)
        {
            try
            {
                int estadoAnuladoChequeId = ObtenerIdEstado("ANULADO");
                if (estadoAnuladoChequeId == 0)
                {
                    MessageBox.Show("NO SE ENCONTRÓ EL ESTADO 'ANULADO' EN LA BASE DE DATOS",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                int estadoReversionPartidaId = ObtenerIdEstadoPartida("REVERSION");
                if (estadoReversionPartidaId == 0)
                {
                    MessageBox.Show("NO SE ENCONTRÓ EL ESTADO 'REVERSIÓN' PARA PARTIDAS",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // ===== 1. ACTUALIZAR ESTADO DEL CHEQUE =====
                cheque.StatusId = estadoAnuladoChequeId;
                cheque.ModifiedBy = UserData.UserId;

                if (Ctrl_Checks.ActualizarCheque(cheque) == 0)
                {
                    return false;
                }

                // ===== 2. BUSCAR PARTIDA ORIGINAL VINCULADA AL CHEQUE =====
                int partidaOriginalId = Ctrl_AccountingEntryChecks.BuscarIdPorCheque(cheque.CheckId);
                if (partidaOriginalId == 0)
                {
                    MessageBox.Show($"NO SE ENCONTRÓ PARTIDA PARA CHEQUE {cheque.CheckNumber}",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                List<Mdl_AccountingEntryDetails> detallesOriginales =
                    Ctrl_AccountingEntryDetails.MostrarDetallesPorPartida(partidaOriginalId);

                if (detallesOriginales.Count == 0)
                {
                    MessageBox.Show($"NO SE ENCONTRARON DETALLES PARA LA PARTIDA DEL CHEQUE {cheque.CheckNumber}",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // ===== 3. CREAR PARTIDA DE REVERSO (MASTER GENÉRICO) =====
                Mdl_AccountingEntryMaster partidaReverso = new Mdl_AccountingEntryMaster
                {
                    // YA NO EXISTE CheckId EN EL MASTER GENÉRICO
                    EntryDate = DateTime.Now,
                    Concept = $"ANULACIÓN DEL CHEQUE NO. {cheque.CheckNumber}",
                    StatusId = estadoReversionPartidaId,
                    TotalAmount = cheque.Amount,
                    CreatedBy = UserData.UserId,
                    IsActive = true
                };

                int partidaReversoId = Ctrl_AccountingEntryMaster.RegistrarPartida(partidaReverso);
                if (partidaReversoId == 0)
                {
                    MessageBox.Show("ERROR AL CREAR PARTIDA DE REVERSO",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // ===== 3.1 VINCULAR PARTIDA DE REVERSO AL CHEQUE =====
                bool vinculoOk = Ctrl_AccountingEntryChecks.RegistrarVinculo(partidaReversoId, cheque.CheckId);
                if (!vinculoOk)
                {
                    MessageBox.Show("ERROR AL VINCULAR LA PARTIDA DE REVERSO CON EL CHEQUE",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // ===== 4. CREAR DETALLES DE REVERSO =====
                foreach (var detalleOriginal in detallesOriginales)
                {
                    Mdl_AccountingEntryDetails detalleReverso = new Mdl_AccountingEntryDetails
                    {
                        EntryMasterId = partidaReversoId,
                        AccountId = detalleOriginal.AccountId,
                        Debit = detalleOriginal.Credit,   // SE INVIERTEN
                        Credit = detalleOriginal.Debit,   // SE INVIERTEN
                        Remarks = $"REVERSO POR ANULACIÓN CHEQUE {cheque.CheckNumber}"
                    };

                    if (Ctrl_AccountingEntryDetails.RegistrarDetalle(detalleReverso) == 0)
                    {
                        MessageBox.Show($"ERROR AL REGISTRAR DETALLE DE REVERSO",
                            "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    string nombreCuenta = ObtenerNombreCuenta(detalleOriginal.AccountId);
                    Ctrl_Accounts.ActualizarSaldo(nombreCuenta, detalleReverso.Debit, detalleReverso.Credit);
                }

                // ===== 5. AUDITORÍA =====
                string detalle = $"ANULACIÓN DE CHEQUE NO. {cheque.CheckNumber} " +
                                $"POR {UserData.Username.ToUpper()}, " +
                                $"BENEFICIARIO: {cheque.BeneficiaryName.ToUpper()}, " +
                                $"MONTO: Q.{cheque.Amount:N2}";

                Ctrl_Audit.RegistrarAccion(
                    UserData.UserId,
                    $"ANULACIÓN DE CHEQUE NO. {cheque.CheckNumber}",
                    "Checks",
                    cheque.CheckId,
                    detalle
                );

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ANULAR CHEQUE {cheque.CheckNumber}: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private int ObtenerIdEstado(string nombreEstado)
        {
            var estados = Ctrl_CheckStatus.MostrarEstados();
            var estado = estados.FirstOrDefault(est => est.StatusName.ToUpper() == nombreEstado.ToUpper());
            return estado?.StatusId ?? 0;
        }

        private string ObtenerNombreCuenta(int accountId)
        {
            try
            {
                var todasCuentas = Ctrl_Accounts.MostrarCuentas();
                var cuenta = todasCuentas.FirstOrDefault(c => c.AccountId == accountId);
                return cuenta?.Name ?? "CUENTA DESCONOCIDA";
            }
            catch
            {
                return "CUENTA DESCONOCIDA";
            }
        }
        #endregion AplicarCambios
        #region BotonesCancelar
        private void Btn_No_Click(object sender, EventArgs e)
        {
            var confirmacion = MessageBox.Show(
                "¿DESEA CANCELAR Y CERRAR SIN GUARDAR CAMBIOS?",
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

            // Desmarcar todos los checkboxes en la tabla
            foreach (DataGridViewRow row in Tabla.Rows)
            {
                row.Cells["Seleccionar"].Value = false;
            }

            ActualizarContador();

            MessageBox.Show("SELECCIÓN LIMPIADA CORRECTAMENTE",
                "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion BotonesCancelar
        #region DatosPartidaAnulacion
        private int ObtenerIdEstadoPartida(string nombreEstado)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT StatusId FROM AccountingEntryStatus 
                           WHERE StatusName = @StatusName 
                           OR StatusCode = @StatusName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StatusName", nombreEstado.ToUpper());
                        object result = cmd.ExecuteScalar();
                        return result == null ? 0 : Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener estado de partida: {ex.Message}",
                               "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
        #endregion DatosPartidaAnulacion
        #region RevertirAnulacion
        private void Btn_RevertirAnulacion_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Crear instancia del formulario
                Frm_Checks_ReverseCancellation frmReverseCancellation = new Frm_Checks_ReverseCancellation();

                // Pasar datos del usuario
                frmReverseCancellation.UserData = this.UserData;

                this.Cursor = Cursors.Default;

                // Mostrar como diálogo
                DialogResult resultado = frmReverseCancellation.ShowDialog(this);

                // Si se realizó algún cambio, recargar la tabla
                if (resultado == DialogResult.OK)
                {
                    CargarCheques();
                    MessageBox.Show("REVERSIÓN DE ANULACIÓN COMPLETADA EXITOSAMENTE",
                        "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL ABRIR FORMULARIO DE REVERSIÓN: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion RevertirAnulacion
    }
}