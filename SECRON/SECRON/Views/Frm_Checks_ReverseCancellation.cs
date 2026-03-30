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
    public partial class Frm_Checks_ReverseCancellation : Form
    {
        #region PropiedadesIniciales
        public Mdl_Security_UserInfo UserData { get; set; }

        // Lista para guardar IDs de cheques seleccionados
        private HashSet<int> _chequesSeleccionados = new HashSet<int>();

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
        public Frm_Checks_ReverseCancellation()
        {
            InitializeComponent();
            ConfigurarTamañoFormulario();
        }

        private void Frm_Checks_ReverseCancellation_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarComboBoxes();
                ConfigurarPlaceHoldersTextbox();
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
            // COMBO DE BUSCAR POR (sin "Estado Actual")
            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_BuscarPor.Items.Clear();
            ComboBox_BuscarPor.Items.AddRange(new object[]
            {
                "Número de Cheque",
                "Beneficiario",
                "Concepto",
                "Período",
                "Fecha de Emisión",
                "Monto"
            });
            ComboBox_BuscarPor.SelectedIndex = 0;
        }
        #endregion ConfigurarComboBoxes
        #region ConfigurarTextBox
        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR...");
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

            // Columna de CHECKBOX
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.Name = "Seleccionar";
            checkColumn.HeaderText = "SELECCIONAR";
            checkColumn.Width = 90;
            checkColumn.ReadOnly = false;
            Tabla.Columns.Add(checkColumn);

            // Columnas de datos
            Tabla.Columns.Add("CheckId", "ID");
            Tabla.Columns.Add("CheckNumber", "NO. CHEQUE");
            Tabla.Columns.Add("BeneficiaryName", "BENEFICIARIO");
            Tabla.Columns.Add("Amount", "MONTO");
            Tabla.Columns.Add("IssueDate", "FECHA EMISIÓN");
            Tabla.Columns.Add("StatusName", "ESTADO");
            Tabla.Columns.Add("Concept", "CONCEPTO");

            // Configuración visual
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = false;
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

            // Evento para manejar clicks en checkbox
            Tabla.CellContentClick += Tabla_CellContentClick;
        }

        private void Tabla_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == Tabla.Columns["Seleccionar"].Index)
            {
                Tabla.CommitEdit(DataGridViewDataErrorContexts.Commit);

                int checkId = Convert.ToInt32(Tabla.Rows[e.RowIndex].Cells["CheckId"].Value);
                bool isChecked = Convert.ToBoolean(Tabla.Rows[e.RowIndex].Cells["Seleccionar"].Value);

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

        private void ActualizarContador()
        {
            Lbl_Contador.Text = $"CHEQUES SELECCIONADOS: {_chequesSeleccionados.Count}";
        }
        #endregion ConfigurarTabla
        #region CargarCheques
        private void CargarCheques()
        {
            try
            {
                Tabla.Rows.Clear();

                // Obtener SOLO cheques ANULADOS
                int estadoAnuladoId = ObtenerIdEstado("ANULADO");
                if (estadoAnuladoId == 0)
                {
                    MessageBox.Show("NO SE ENCONTRÓ EL ESTADO 'ANULADO' EN LA BASE DE DATOS",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Cargar todos los cheques anulados
                List<Mdl_Checks> cheques = Ctrl_Checks.MostrarCheques(1, 10000);
                var chequesAnulados = cheques.Where(c => c.StatusId == estadoAnuladoId).ToList();

                foreach (var cheque in chequesAnulados)
                {
                    bool isSelected = _chequesSeleccionados.Contains(cheque.CheckId);

                    Tabla.Rows.Add(
                        isSelected,
                        cheque.CheckId,
                        cheque.CheckNumber,
                        cheque.BeneficiaryName,
                        cheque.Amount.ToString("N2"),
                        cheque.IssueDate.ToString("dd/MM/yyyy"),
                        "ANULADO",
                        cheque.Concept
                    );
                }

                if (Tabla.Rows.Count > 0)
                    Tabla.ClearSelection();

                ActualizarContador();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR CHEQUES: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private int ObtenerIdEstado(string nombreEstado)
        {
            var estados = Ctrl_CheckStatus.MostrarEstados();
            var estado = estados.FirstOrDefault(est => est.StatusName.ToUpper() == nombreEstado.ToUpper());
            return estado?.StatusId ?? 0;
        }
        #endregion CargarCheques
        #region Búsqueda
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                string criterio = ComboBox_BuscarPor.SelectedItem?.ToString() ?? "";
                string textoBusqueda = Txt_ValorBuscado.Text.Trim();

                if (textoBusqueda == "BUSCAR..." || string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    CargarCheques();
                    return;
                }

                BuscarCheques(criterio, textoBusqueda);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR EN BÚSQUEDA: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            ComboBox_BuscarPor.SelectedIndex = 0;
            CargarCheques();
        }

        private void BuscarCheques(string criterio, string busqueda)
        {
            try
            {
                Tabla.Rows.Clear();

                int estadoAnuladoId = ObtenerIdEstado("ANULADO");
                List<Mdl_Checks> cheques = Ctrl_Checks.MostrarCheques(1, 10000);
                var resultado = cheques.Where(c => c.StatusId == estadoAnuladoId);

                busqueda = busqueda.ToUpper();

                switch (criterio)
                {
                    case "Número de Cheque":
                        resultado = resultado.Where(c => c.CheckNumber.ToUpper().Contains(busqueda));
                        break;
                    case "Beneficiario":
                        resultado = resultado.Where(c => c.BeneficiaryName.ToUpper().Contains(busqueda));
                        break;
                    case "Concepto":
                        resultado = resultado.Where(c => c.Concept.ToUpper().Contains(busqueda));
                        break;
                    case "Período":
                        resultado = resultado.Where(c => c.Period?.ToUpper().Contains(busqueda) ?? false);
                        break;
                    case "Fecha de Emisión":
                        resultado = resultado.Where(c => c.IssueDate.ToString("dd/MM/yyyy").Contains(busqueda));
                        break;
                    case "Monto":
                        resultado = resultado.Where(c => c.Amount.ToString("N2").Contains(busqueda));
                        break;
                }

                foreach (var cheque in resultado)
                {
                    bool isSelected = _chequesSeleccionados.Contains(cheque.CheckId);

                    Tabla.Rows.Add(
                        isSelected,
                        cheque.CheckId,
                        cheque.CheckNumber,
                        cheque.BeneficiaryName,
                        cheque.Amount.ToString("N2"),
                        cheque.IssueDate.ToString("dd/MM/yyyy"),
                        "ANULADO",
                        cheque.Concept
                    );
                }

                if (Tabla.Rows.Count > 0)
                    Tabla.ClearSelection();

                ActualizarContador();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL BUSCAR: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Búsqueda
        #region RevertirAnulación
        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDACIÓN 1: Debe haber cheques seleccionados
                if (_chequesSeleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN CHEQUE PARA REVERTIR LA ANULACIÓN",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ⭐ VALIDACIÓN 2: No permitir revertir más de 3 cheques a la vez
                if (_chequesSeleccionados.Count > 3)
                {
                    MessageBox.Show(
                        "NO SE PUEDE REVERTIR LA ANULACIÓN DE MÁS DE 3 CHEQUES A LA VEZ.\n\n" +
                        $"CHEQUES SELECCIONADOS: {_chequesSeleccionados.Count}\n" +
                        "LÍMITE MÁXIMO: 3\n\n" +
                        "POR SEGURIDAD, ESTE PROCESO ESTÁ LIMITADO.",
                        "VALIDACIÓN DE SEGURIDAD",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // CONFIRMACIÓN
                var confirmacion = MessageBox.Show(
                    $"¿DESEA REVERTIR LA ANULACIÓN DE {_chequesSeleccionados.Count} CHEQUE(S) SELECCIONADO(S)?\n\n" +
                    $"ESTE PROCESO REALIZARÁ LO SIGUIENTE:\n" +
                    $"1. Cambiará el estado de los cheques de ANULADO a EMITIDO\n" +
                    $"2. Eliminará las partidas de reversión creadas al anular\n" +
                    $"3. Restaurará los saldos de las cuentas contables\n\n" +
                    $"¿DESEA CONTINUAR?",
                    "CONFIRMAR REVERSIÓN DE ANULACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                this.Cursor = Cursors.WaitCursor;

                int exitosos = 0;
                int fallidos = 0;
                StringBuilder errores = new StringBuilder();

                foreach (int checkId in _chequesSeleccionados)
                {
                    Mdl_Checks cheque = Ctrl_Checks.ObtenerChequePorId(checkId);
                    if (cheque == null)
                    {
                        fallidos++;
                        errores.AppendLine($"- Cheque ID {checkId}: No se encontró en la base de datos");
                        continue;
                    }

                    if (RevertirAnulacionCheque(cheque))
                    {
                        exitosos++;
                    }
                    else
                    {
                        fallidos++;
                        errores.AppendLine($"- Cheque NO. {cheque.CheckNumber}: Error al revertir");
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
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL REVERTIR ANULACIONES: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool RevertirAnulacionCheque(Mdl_Checks cheque)
        {
            try
            {
                // PASO 1: Obtener estado EMITIDO
                int estadoEmitidoId = ObtenerIdEstado("EMITIDO");
                if (estadoEmitidoId == 0)
                {
                    MessageBox.Show("NO SE ENCONTRÓ EL ESTADO 'EMITIDO' EN LA BASE DE DATOS",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // PASO 2: Buscar la partida de REVERSIÓN (la que se creó al anular)
                int partidaReversionId = BuscarPartidaReversionPorCheque(cheque.CheckId);
                if (partidaReversionId == 0)
                {
                    MessageBox.Show($"NO SE ENCONTRÓ PARTIDA DE REVERSIÓN PARA CHEQUE {cheque.CheckNumber}",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // PASO 3: Obtener detalles de la partida de reversión
                List<Mdl_AccountingEntryDetails> detallesReversion =
                    Ctrl_AccountingEntryDetails.MostrarDetallesPorPartida(partidaReversionId);

                if (detallesReversion.Count == 0)
                {
                    MessageBox.Show($"NO SE ENCONTRARON DETALLES PARA LA PARTIDA DE REVERSIÓN DEL CHEQUE {cheque.CheckNumber}",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // PASO 4: REVERTIR SALDOS (inverso a lo que hizo la anulación)
                foreach (var detalleReversion in detallesReversion)
                {
                    string nombreCuenta = ObtenerNombreCuenta(detalleReversion.AccountId);

                    // La partida de reversión tiene Debit y Credit invertidos
                    // Para revertir, invertimos nuevamente (volvemos a los valores originales)
                    decimal debitRevertido = detalleReversion.Credit;  // Invertir
                    decimal creditRevertido = detalleReversion.Debit;  // Invertir

                    // Actualizar con los valores invertidos para cancelar el efecto de la anulación
                    Ctrl_Accounts.ActualizarSaldo(nombreCuenta, debitRevertido, creditRevertido);
                }

                // PASO 5: ELIMINAR DETALLES DE LA PARTIDA DE REVERSIÓN
                foreach (var detalleReversion in detallesReversion)
                {
                    if (Ctrl_AccountingEntryDetails.EliminarDetalle(detalleReversion.EntryDetailId) == 0)
                    {
                        MessageBox.Show($"ERROR AL ELIMINAR DETALLE DE REVERSIÓN",
                            "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                // PASO 6: ELIMINAR LA PARTIDA MAESTRA DE REVERSIÓN
                if (Ctrl_AccountingEntryMaster.EliminarPartida(partidaReversionId) == 0)
                {
                    MessageBox.Show($"ERROR AL ELIMINAR PARTIDA DE REVERSIÓN",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // PASO 7: CAMBIAR ESTADO DEL CHEQUE A EMITIDO
                cheque.StatusId = estadoEmitidoId;
                cheque.ModifiedBy = UserData.UserId;

                if (Ctrl_Checks.ActualizarCheque(cheque) == 0)
                {
                    MessageBox.Show($"ERROR AL ACTUALIZAR ESTADO DEL CHEQUE {cheque.CheckNumber}",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // PASO 8: REGISTRAR AUDITORÍA
                string detalle = $"REVERSIÓN DE ANULACIÓN DE CHEQUE NO. {cheque.CheckNumber}, " +
                                $"BENEFICIARIO: {cheque.BeneficiaryName.ToUpper()}, " +
                                $"MONTO: Q.{cheque.Amount:N2}, " +
                                $"USUARIO: {UserData.Username.ToUpper()}";

                Ctrl_Audit.RegistrarAccion(
                    UserData.UserId,
                    $"REVERSIÓN ANULACIÓN CHEQUE NO. {cheque.CheckNumber}",
                    "Checks",
                    cheque.CheckId,
                    detalle
                );

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL REVERTIR ANULACIÓN DEL CHEQUE {cheque.CheckNumber}: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private int BuscarPartidaReversionPorCheque(int checkId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // Buscar la partida con estado "REVERSIÓN" asociada al cheque,
                    // usando la tabla de vínculo AccountingEntryChecks
                    string query = @"
                SELECT TOP 1 m.EntryMasterId
                FROM AccountingEntryMaster m
                INNER JOIN AccountingEntryChecks c
                    ON c.EntryMasterId = m.EntryMasterId
                WHERE c.CheckId = @CheckId
                  AND m.StatusId = (
                        SELECT StatusId 
                        FROM AccountingEntryStatus 
                        WHERE StatusName = 'REVERSION' 
                           OR StatusCode  = 'REVERSION'
                  )
                ORDER BY m.CreatedDate DESC;";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CheckId", checkId);
                        object result = cmd.ExecuteScalar();
                        return (result == null || result == DBNull.Value)
                            ? 0
                            : Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar partida de reversión: {ex.Message}",
                               "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
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
        #endregion RevertirAnulación
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
    }
}