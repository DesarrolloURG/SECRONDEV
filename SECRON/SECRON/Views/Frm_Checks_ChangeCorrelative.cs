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

namespace SECRON.Views
{
    public partial class Frm_Checks_ChangeCorrelative : Form
    {
        #region PropiedadesIniciales
        // Usuario actual
        public Mdl_Security_UserInfo UserData { get; set; }

        // Cheque seleccionado
        private Mdl_Checks _chequeSeleccionado = null;
        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(700, 650);
            this.MinimumSize = new Size(700, 650);
            this.MaximumSize = new Size(700, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }
        #endregion PropiedadesIniciales
        #region Constructor
        public Frm_Checks_ChangeCorrelative()
        {
            InitializeComponent();
            ConfigurarTamañoFormulario();
        }

        private void Frm_Checks_ChangeCorrelative_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarComboBoxes();
                ConfigurarTabla();
                ConfigurarTextBoxes();
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
            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_BuscarPor.Items.AddRange(new object[]
            {
                "Número de Cheque",
                "Beneficiario",
                "Concepto",
                "Período",
                "Estado Actual",
                "Fecha de Emisión",
                "Monto"
            });
            ComboBox_BuscarPor.SelectedIndex = 0;
        }
        #endregion ConfigurarComboBoxes
        #region ConfigurarTabla
        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();

            // Columnas
            Tabla.Columns.Add("CheckId", "ID");
            Tabla.Columns.Add("CheckNumber", "NO. CHEQUE");
            Tabla.Columns.Add("BeneficiaryName", "BENEFICIARIO");
            Tabla.Columns.Add("Amount", "MONTO");
            Tabla.Columns.Add("IssueDate", "FECHA EMISIÓN");
            Tabla.Columns.Add("StatusName", "ESTADO ACTUAL");
            Tabla.Columns.Add("Concept", "CONCEPTO");

            // Configuración visual
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            // Estilos
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // Ajustar columnas
            Tabla.Columns["CheckId"].Visible = false;
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

            // Evento selección
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }
        #endregion ConfigurarTabla
        #region ConfigurarTextBoxes
        private void ConfigurarTextBoxes()
        {
            // Txt_ChequeModificar: Solo lectura
            Txt_ChequeModificar.ReadOnly = true;
            Txt_ChequeModificar.BackColor = Color.FromArgb(240, 240, 240);
            Txt_ChequeModificar.ForeColor = Color.FromArgb(100, 100, 100);
            Txt_ChequeModificar.Text = "";

            // Txt_NuevoNumero: Editable, solo números
            Txt_NuevoNumero.MaxLength = 10;
            Txt_NuevoNumero.KeyPress += ValidarSoloNumeros_KeyPress;
        }

        private void ValidarSoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        #endregion ConfigurarTextBoxes
        #region CargarCheques
        private void CargarCheques()
        {
            try
            {
                Tabla.Rows.Clear();

                List<Mdl_Checks> cheques = Ctrl_Checks.MostrarCheques(1, 50);

                foreach (var cheque in cheques)
                {
                    string estadoNombre = ObtenerNombreEstado(cheque.StatusId);

                    Tabla.Rows.Add(
                        cheque.CheckId,
                        cheque.CheckNumber,
                        cheque.BeneficiaryName,
                        cheque.Amount.ToString("N2"),
                        cheque.IssueDate.ToString("dd/MM/yyyy"),
                        estadoNombre,
                        cheque.Concept
                    );
                }

                if (Tabla.Rows.Count > 0)
                    Tabla.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR CHEQUES: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ObtenerNombreEstado(int statusId)
        {
            var estados = Ctrl_CheckStatus.MostrarEstados();
            var estado = estados.FirstOrDefault(est => est.StatusId == statusId);
            return estado?.StatusName ?? "DESCONOCIDO";
        }
        #endregion CargarCheques
        #region Búsqueda
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                string criterio = ComboBox_BuscarPor.SelectedItem?.ToString() ?? "";
                string valorBusqueda = Txt_ValorBuscado.Text.Trim();

                if (string.IsNullOrEmpty(valorBusqueda))
                {
                    MessageBox.Show("INGRESE UN VALOR PARA BUSCAR",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Tabla.Rows.Clear();
                List<Mdl_Checks> resultados = new List<Mdl_Checks>();

                switch (criterio)
                {
                    case "Número de Cheque":
                    case "Beneficiario":
                    case "Concepto":
                        resultados = Ctrl_Checks.BuscarCheques(textoBusqueda: valorBusqueda);
                        break;

                    case "Período":
                        resultados = Ctrl_Checks.BuscarCheques(periodo: valorBusqueda);
                        break;

                    case "Estado Actual":
                        var estados = Ctrl_CheckStatus.MostrarEstados();
                        var estadoEncontrado = estados.FirstOrDefault(estado =>
                            estado.StatusName.ToUpper().Contains(valorBusqueda.ToUpper()));

                        if (estadoEncontrado != null)
                        {
                            var todosCheques = Ctrl_Checks.MostrarCheques(1, 10000);
                            resultados = todosCheques.Where(c => c.StatusId == estadoEncontrado.StatusId).ToList();
                        }
                        break;

                    case "Fecha de Emisión":
                        if (DateTime.TryParse(valorBusqueda, out DateTime fecha))
                        {
                            resultados = Ctrl_Checks.BuscarCheques(
                                fechaInicio: fecha.Date,
                                fechaFin: fecha.Date.AddDays(1).AddSeconds(-1));
                        }
                        else
                        {
                            MessageBox.Show("FORMATO DE FECHA INVÁLIDO. USE: DD/MM/YYYY",
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        break;

                    case "Monto":
                        if (decimal.TryParse(valorBusqueda, out decimal monto))
                        {
                            var todosCheques = Ctrl_Checks.MostrarCheques(1, 10000);
                            resultados = todosCheques.Where(c => c.Amount == monto).ToList();
                        }
                        else
                        {
                            MessageBox.Show("FORMATO DE MONTO INVÁLIDO",
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        break;
                }

                foreach (var cheque in resultados)
                {
                    string estadoNombre = ObtenerNombreEstado(cheque.StatusId);

                    Tabla.Rows.Add(
                        cheque.CheckId,
                        cheque.CheckNumber,
                        cheque.BeneficiaryName,
                        cheque.Amount.ToString("N2"),
                        cheque.IssueDate.ToString("dd/MM/yyyy"),
                        estadoNombre,
                        cheque.Concept
                    );
                }

                if (Tabla.Rows.Count == 0)
                {
                    MessageBox.Show("NO SE ENCONTRARON RESULTADOS",
                        "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Tabla.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR EN BÚSQUEDA: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Clear();
            Txt_ChequeModificar.Clear();
            Txt_NuevoNumero.Clear();
            ComboBox_BuscarPor.SelectedIndex = 0;
            _chequeSeleccionado = null;
            CargarCheques();
        }
        #endregion Búsqueda
        #region SeleccionCheque
        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (Tabla.SelectedRows.Count > 0)
                {
                    DataGridViewRow fila = Tabla.SelectedRows[0];
                    int checkId = Convert.ToInt32(fila.Cells["CheckId"].Value);

                    // Obtener cheque completo
                    _chequeSeleccionado = Ctrl_Checks.ObtenerChequePorId(checkId);

                    if (_chequeSeleccionado != null)
                    {
                        Txt_ChequeModificar.Text = _chequeSeleccionado.CheckNumber;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL SELECCIONAR CHEQUE: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion SeleccionCheque
        #region AplicarCambios
        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDACIÓN 1: Cheque seleccionado
                if (_chequeSeleccionado == null)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN CHEQUE DE LA TABLA",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // VALIDACIÓN 2: Nuevo número ingresado
                string nuevoNumero = Txt_NuevoNumero.Text.Trim();
                if (string.IsNullOrEmpty(nuevoNumero))
                {
                    MessageBox.Show("DEBE INGRESAR EL NUEVO NÚMERO DE CHEQUE",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_NuevoNumero.Focus();
                    return;
                }

                // VALIDACIÓN 3: No puede ser el mismo número
                if (nuevoNumero == _chequeSeleccionado.CheckNumber)
                {
                    MessageBox.Show("EL NUEVO NÚMERO ES IGUAL AL ACTUAL",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_NuevoNumero.Focus();
                    return;
                }

                // VALIDACIÓN 4: Verificar que el nuevo número NO exista
                if (Ctrl_Checks.ValidarExistenciaCheque(nuevoNumero))
                {
                    MessageBox.Show(
                        $"EL CHEQUE NO. {nuevoNumero} YA EXISTE EN LA BASE DE DATOS.\n" +
                        "NO PUEDE ASIGNAR UN NÚMERO DUPLICADO.",
                        "CHEQUE DUPLICADO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Txt_NuevoNumero.Clear();
                    Txt_NuevoNumero.Focus();
                    return;
                }

                // ===== VERIFICAR PARTIDAS EXISTENTES =====
                int cantidadPartidas = Ctrl_AccountingEntryChecks.ContarPartidasPorCheque(_chequeSeleccionado.CheckId);

                string mensajePartidas = cantidadPartidas > 0
                    ? $"\n\nEste cheque tiene {cantidadPartidas} partida(s) contable(s) asociada(s) que también se actualizarán."
                    : "\n\nEste cheque no tiene partidas contables asociadas.";

                // CONFIRMACIÓN
                var confirmacion = MessageBox.Show(
                    $"¿DESEA CAMBIAR EL NÚMERO DEL CHEQUE?\n\n" +
                    $"Número Actual: {_chequeSeleccionado.CheckNumber}\n" +
                    $"Número Nuevo: {nuevoNumero}\n\n" +
                    $"Beneficiario: {_chequeSeleccionado.BeneficiaryName}\n" +
                    $"Monto: Q.{_chequeSeleccionado.Amount:N2}" +
                    mensajePartidas,
                    "CONFIRMAR CAMBIO DE CORRELATIVO",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                this.Cursor = Cursors.WaitCursor;

                // Guardar valores para auditoría
                string numeroAnterior = _chequeSeleccionado.CheckNumber;
                int checkIdAnterior = _chequeSeleccionado.CheckId;

                // ===== PASO 1: ACTUALIZAR NÚMERO EN TABLA CHECKS =====
                _chequeSeleccionado.CheckNumber = nuevoNumero;
                _chequeSeleccionado.ModifiedBy = UserData.UserId;

                if (Ctrl_Checks.ActualizarCheque(_chequeSeleccionado) == 0)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("ERROR AL ACTUALIZAR EL NÚMERO DEL CHEQUE",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ===== PASO 2: ACTUALIZAR TODAS LAS PARTIDAS CONTABLES =====
                int partidasActualizadas = 0;

                if (cantidadPartidas > 0)
                {
                    // IMPORTANTE: Como solo cambió CheckNumber y NO CheckId, 
                    // NO necesitamos actualizar AccountingEntryMaster en este caso
                    // porque esa tabla usa CheckId como referencia, no CheckNumber.

                    // Sin embargo, el código está preparado para cuando sea necesario:
                    // partidasActualizadas = Ctrl_AccountingEntryMaster.ActualizarCheckIdEnTodasPartidas(checkIdAnterior, nuevoCheckId);
                }

                // ===== PASO 3: REGISTRAR AUDITORÍA =====
                string detallePartidas = cantidadPartidas > 0
                    ? $", PARTIDAS ASOCIADAS: {cantidadPartidas}"
                    : ", SIN PARTIDAS ASOCIADAS";

                string detalle = $"CAMBIO DE CORRELATIVO DE CHEQUE: " +
                                $"NO. {numeroAnterior} → NO. {nuevoNumero}, " +
                                $"BENEFICIARIO: {_chequeSeleccionado.BeneficiaryName.ToUpper()}, " +
                                $"MONTO: Q.{_chequeSeleccionado.Amount:N2}" +
                                detallePartidas +
                                $", USUARIO: {UserData.Username.ToUpper()}";

                Ctrl_Audit.RegistrarAccion(
                    UserData.UserId,
                    "CAMBIO DE CORRELATIVO",
                    "Checks",
                    _chequeSeleccionado.CheckId,
                    detalle
                );

                this.Cursor = Cursors.Default;

                // Mensaje de éxito con información de partidas
                string mensajeExito = $"CORRELATIVO ACTUALIZADO CORRECTAMENTE\n\n" +
                                     $"Número Anterior: {numeroAnterior}\n" +
                                     $"Número Nuevo: {nuevoNumero}";

                if (cantidadPartidas > 0)
                {
                    mensajeExito += $"\n\nPartidas Contables Verificadas: {cantidadPartidas}";
                }

                MessageBox.Show(mensajeExito, "ÉXITO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar y recargar
                Txt_ChequeModificar.Clear();
                Txt_NuevoNumero.Clear();
                _chequeSeleccionado = null;
                CargarCheques();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL APLICAR CAMBIOS: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        #endregion BotonesCancelar
    }
}