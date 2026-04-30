using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_Accounts_Managment : Form
    {
        #region PropiedadesIniciales
        // Variables de entorno
        public Mdl_Security_UserInfo UserData { get; set; }

        // Cuenta seleccionada para editar
        private Mdl_Accounts _cuentaSeleccionada = null;

        // Ancho base del formulario para ajuste dinámico de columna
        private int _anchoBase = 1200;

        public Frm_Accounts_Managment()
        {
            InitializeComponent();
        }

        private void Frm_Accounts_Managment_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarComboBoxes();
                ConfigurarTabla();
                RefrescarTabla();
                ConfigurarEstadoInicialBotones();
                BloquearCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR FORMULARIO: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Frm_Accounts_Managment_Resize(object sender, EventArgs e)
        {
            AjustarColumnasTabla();
        }
        #endregion PropiedadesIniciales
        #region ConfigurarTabla

        private void RefrescarTabla()
        {
            Tabla.DataSource = null;
            Tabla.DataSource = Ctrl_Accounts.MostrarCuentas();
            ConfigurarTabla();
        }

        private void ConfigurarTabla()
        {
            if (Tabla.Columns.Count == 0) return;

            // Ocultar todas las columnas
            foreach (DataGridViewColumn col in Tabla.Columns)
                col.Visible = false;

            // Mostrar solo las columnas requeridas (estética v1)
            string[] visibles = { "Code", "Name", "Sign", "Balance" };
            foreach (string nombre in visibles)
                if (Tabla.Columns.Contains(nombre))
                    Tabla.Columns[nombre].Visible = true;

            // Renombrar encabezados
            if (Tabla.Columns.Contains("Code")) Tabla.Columns["Code"].HeaderText = "CÓDIGO";
            if (Tabla.Columns.Contains("Name")) Tabla.Columns["Name"].HeaderText = "CUENTAS";
            if (Tabla.Columns.Contains("Sign")) Tabla.Columns["Sign"].HeaderText = "SIGNO";
            if (Tabla.Columns.Contains("Balance")) Tabla.Columns["Balance"].HeaderText = "SALDO";

            // Comportamiento general
            Tabla.MultiSelect = false;
            Tabla.RowHeadersVisible = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.ReadOnly = true;
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Estilos encabezados (consistente con Frm_Checks_Managment)
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            // Estilos filas
            Tabla.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 140, 255);
            Tabla.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla.RowTemplate.Height = 28;

            // Selección cambiada evento
            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;

            AjustarColumnasTabla();
        }

        private void AjustarColumnasTabla()
        {
            if (Tabla.Columns.Count == 0 || !Tabla.Columns.Contains("Name")) return;

            int extraAncho = this.Width > _anchoBase ? this.Width - _anchoBase : 0;

            Tabla.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Tabla.Columns["Name"].Width = 400 + extraAncho;

            if (Tabla.Columns.Contains("Code")) Tabla.Columns["Code"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (Tabla.Columns.Contains("Sign")) Tabla.Columns["Sign"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (Tabla.Columns.Contains("Balance")) Tabla.Columns["Balance"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Tabla_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int nivel = 0;
            if (Tabla.Columns.Contains("Level"))
                nivel = Convert.ToInt32(Tabla.Rows[e.RowIndex].Cells["Level"].Value);

            string colName = Tabla.Columns[e.ColumnIndex].Name;

            // Alineaciones por columna
            if (colName == "Sign")
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            else if (colName == "Balance" || colName == "Code")
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Nivel 1 — fondo verde oscuro
            if (nivel == 1)
            {
                e.CellStyle.BackColor = Color.DarkGreen;
                e.CellStyle.ForeColor = Color.White;

                if (colName == "Name")
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Negrita niveles 1 y 2
            if (colName != "Level")
            {
                e.CellStyle.Font = (nivel == 1 || nivel == 2)
                    ? new Font(Tabla.DefaultCellStyle.Font, FontStyle.Bold)
                    : new Font(Tabla.DefaultCellStyle.Font, FontStyle.Regular);
            }

            // Sangría proporcional al nivel en columna Name
            if (colName == "Name" && e.Value != null)
            {
                string sangria = new string(' ', nivel * 4);
                e.Value = sangria + e.Value.ToString();
                e.FormattingApplied = true;
            }
        }

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Obtener AccountId de la fila seleccionada
            if (!Tabla.Columns.Contains("AccountId")) return;
            int accountId = Convert.ToInt32(Tabla.Rows[e.RowIndex].Cells["AccountId"].Value);

            // Obtener cuenta completa y cargar en formulario
            _cuentaSeleccionada = Ctrl_Accounts.ObtenerCuentaPorId(accountId);
            if (_cuentaSeleccionada != null)
                CargarCuentaEnFormulario(_cuentaSeleccionada);

            // Habilitar edición
            HabilitarCampos();
            Btn_Save.Enabled = false;
            Btn_Update.Enabled = true;
            Btn_Inactive.Enabled = true;
            Btn_Clear.Enabled = true;
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;

            DataGridViewRow fila = Tabla.SelectedRows[0];
            if (!Tabla.Columns.Contains("AccountId")) return;

            int accountId = Convert.ToInt32(fila.Cells["AccountId"].Value);
            _cuentaSeleccionada = Ctrl_Accounts.ObtenerCuentaPorId(accountId);

            if (_cuentaSeleccionada != null)
                CargarCuentaEnFormulario(_cuentaSeleccionada);

            HabilitarCampos();
            Btn_Save.Enabled = false;
            Btn_Update.Enabled = true;
            Btn_Inactive.Enabled = true;
            Btn_Clear.Enabled = true;
        }

        #endregion ConfigurarTabla
        #region ConfigurarComboBoxes
        private void ConfigurarComboBoxes()
        {
            // Estilo DropDownList para todos
            ComboBox_Type.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Level.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Sign.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_BankAccountType.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Currency.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_BankName.DropDownStyle = ComboBoxStyle.DropDownList;

            // TIPO — mismos valores del legacy (comboBoxTipo)
            ComboBox_Type.Items.AddRange(new object[]
            {
                "ACTIVO", "PASIVO", "CAPITAL", "INGRESO", "GASTO", "COSTO"
            });
            ComboBox_Type.SelectedIndex = 0;

            // NIVEL — del 1 al 6, mismo que comboBoxNivel del legacy
            for (int i = 1; i <= 6; i++)
                ComboBox_Level.Items.Add(i.ToString());
            ComboBox_Level.SelectedIndex = 0;

            // SIGNO — mismo que comboBoxSigno del legacy
            ComboBox_Sign.Items.AddRange(new object[] { "+", "-" });
            ComboBox_Sign.SelectedIndex = 0;

            // TIPO DE CUENTA BANCARIA — mismo que comboBoxTipoCuentas del legacy
            ComboBox_BankAccountType.Items.AddRange(new object[]
            {
                "MONETARIA", "AHORRO", "N/A"
            });
            ComboBox_BankAccountType.SelectedIndex = 0;

            // MONEDA — mismo que comboBoxMoneda del legacy
            ComboBox_Currency.Items.AddRange(new object[] { "Q", "USD" });
            ComboBox_Currency.SelectedIndex = 0;

            // BANCO — cargado desde BD usando el método existente
            CargarBancos();
        }

        private void CargarBancos()
        {
            ComboBox_BankName.Items.Clear();

            try
            {
                var bancos = Ctrl_Banks.ObtenerBancosParaCombo();
                foreach (var banco in bancos)
                    ComboBox_BankName.Items.Add(banco.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar bancos: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ComboBox_BankName.SelectedIndex = ComboBox_BankName.Items.Count > 0 ? 0 : -1;
        }
        #endregion ConfigurarComboBoxes
        #region EstadoBotones

        private void ConfigurarEstadoInicialBotones()
        {
            Btn_Save.Enabled = false;
            Btn_Update.Enabled = false;
            Btn_Inactive.Enabled = false;
            Btn_Clear.Enabled = false;
        }

        #endregion EstadoBotones
        #region BloquearHabilitarCampos

        private void BloquearCampos()
        {
            Panel_Informacion.Enabled = false;
        }

        private void HabilitarCampos()
        {
            Panel_Informacion.Enabled = true;
        }

        #endregion BloquearHabilitarCampos
        #region LlenarYLimpiarFormulario

        private void CargarCuentaEnFormulario(Mdl_Accounts cuenta)
        {
            Txt_Code.Text = cuenta.Code ?? "";
            Txt_ParentAccountCode.Text = cuenta.ParentAccountCode ?? "";
            Txt_Name.Text = cuenta.Name ?? "";
            Txt_Balance.Text = cuenta.Balance.ToString("F2");
            Txt_CheckNumber.Text = cuenta.CheckNumber.ToString();
            Txt_CurrencyName.Text = cuenta.CurrencyName ?? "";

            AsignarComboBox(ComboBox_Type, cuenta.Type);
            AsignarComboBox(ComboBox_Level, cuenta.Level.ToString());
            AsignarComboBox(ComboBox_Sign, cuenta.Sign);
            AsignarComboBox(ComboBox_BankName, cuenta.BankName);
            AsignarComboBox(ComboBox_BankAccountType, cuenta.BankAccountType);
            AsignarComboBox(ComboBox_Currency, cuenta.Currency);
        }

        private void LimpiarFormulario()
        {
            Txt_Code.Text = "";
            Txt_ParentAccountCode.Text = "";
            Txt_Name.Text = "";
            Txt_Balance.Text = "0.00";
            Txt_CheckNumber.Text = "0";
            Txt_CurrencyName.Text = "";

            ComboBox_Type.SelectedIndex = -1;
            ComboBox_Level.SelectedIndex = -1;
            ComboBox_Sign.SelectedIndex = -1;
            ComboBox_BankName.SelectedIndex = 0;
            ComboBox_BankAccountType.SelectedIndex = -1;
            ComboBox_Currency.SelectedIndex = -1;

            _cuentaSeleccionada = null;
        }

        private void AsignarComboBox(ComboBox combo, string valor)
        {
            if (string.IsNullOrEmpty(valor)) { combo.SelectedIndex = -1; return; }

            // Buscar ignorando mayúsculas/minúsculas y espacios
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i].ToString().Trim().Equals(valor.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            // Si no encontró coincidencia, agregar el valor temporalmente y seleccionarlo
            combo.Items.Add(valor);
            combo.SelectedIndex = combo.Items.Count - 1;
        }

        #endregion LlenarYLimpiarFormulario
        #region ObtenerModelo

        private Mdl_Accounts ObtenerModeloDesdeFormulario()
        {
            return new Mdl_Accounts
            {
                AccountId = _cuentaSeleccionada?.AccountId ?? 0,
                Code = Txt_Code.Text.Trim(),
                ParentAccountCode = Txt_ParentAccountCode.Text.Trim(),
                Name = Txt_Name.Text.Trim(),
                Type = ComboBox_Type.SelectedItem?.ToString() ?? "",
                Level = ComboBox_Level.SelectedIndex >= 0
                                        ? int.Parse(ComboBox_Level.SelectedItem.ToString()) : 1,
                Sign = ComboBox_Sign.SelectedItem?.ToString() ?? "+",
                Balance = decimal.TryParse(Txt_Balance.Text, out decimal bal) ? bal : 0,
                BankName = ComboBox_BankName.SelectedItem?.ToString() ?? "N/A",
                BankCode = Ctrl_Banks.ObtenerIdPorNombreBanco(ComboBox_BankName.SelectedItem?.ToString()) ?? 0,
                BankAccountType = ComboBox_BankAccountType.SelectedItem?.ToString() ?? "",
                CheckNumber = int.TryParse(Txt_CheckNumber.Text, out int chk) ? chk : 0,
                Currency = ComboBox_Currency.SelectedItem?.ToString() ?? "",
                CurrencyName = Txt_CurrencyName.Text.Trim()
            };
        }

        #endregion ObtenerModelo
        #region Validaciones

        private bool ValidarCamposObligatorios()
        {
            if (string.IsNullOrWhiteSpace(Txt_Code.Text))
            {
                MessageBox.Show("EL CÓDIGO DE CUENTA ES OBLIGATORIO.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Code.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(Txt_Name.Text))
            {
                MessageBox.Show("EL NOMBRE DE CUENTA ES OBLIGATORIO.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Name.Focus();
                return false;
            }
            if (ComboBox_Type.SelectedIndex < 0)
            {
                MessageBox.Show("DEBE SELECCIONAR EL TIPO DE CUENTA.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (ComboBox_Level.SelectedIndex < 0)
            {
                MessageBox.Show("DEBE SELECCIONAR EL NIVEL DE CUENTA.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (ComboBox_Sign.SelectedIndex < 0)
            {
                MessageBox.Show("DEBE SELECCIONAR EL SIGNO DE CUENTA.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        #endregion Validaciones
        #region BotonesAccion

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            
            LimpiarFormulario();
            HabilitarCampos();
            Btn_Save.Enabled = true;
            Btn_Update.Enabled = false;
            Btn_Inactive.Enabled = false;
            Btn_Clear.Enabled = true;
            Txt_Code.Focus();
        }


        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!ValidarCamposObligatorios()) return;

            if (Ctrl_Accounts.ValidarRegistro("Code", Txt_Code.Text.Trim()))
            {
                MessageBox.Show("YA EXISTE UNA CUENTA CON ESE CÓDIGO.", "REGISTRO DUPLICADO",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Mdl_Accounts cuenta = ObtenerModeloDesdeFormulario();
            int resultado = Ctrl_Accounts.RegistrarCuenta(cuenta);

            if (resultado > 0)
            {
                if (!string.IsNullOrEmpty(cuenta.ParentAccountCode))
                    Ctrl_Accounts.ActualizarNiveles(cuenta.ParentAccountCode, cuenta.Level - 1);

                MessageBox.Show("CUENTA REGISTRADA CORRECTAMENTE.", "ÉXITO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefrescarTabla();
                LimpiarFormulario();
                BloquearCampos();
                ConfigurarEstadoInicialBotones();
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (_cuentaSeleccionada == null)
            {
                MessageBox.Show("SELECCIONE UNA CUENTA DE LA TABLA PARA EDITAR.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCamposObligatorios()) return;

            var confirmacion = MessageBox.Show(
                "¿DESEA ACTUALIZAR ESTA CUENTA?",
                "CONFIRMAR ACTUALIZACIÓN",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.No) return;

            Mdl_Accounts cuenta = ObtenerModeloDesdeFormulario();
            int resultado = Ctrl_Accounts.ModificarCuenta(cuenta);

            if (resultado > 0)
            {
                if (!string.IsNullOrEmpty(cuenta.ParentAccountCode))
                    Ctrl_Accounts.ActualizarNiveles(cuenta.ParentAccountCode, cuenta.Level - 1);

                MessageBox.Show("CUENTA ACTUALIZADA CORRECTAMENTE.", "ÉXITO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefrescarTabla();
                LimpiarFormulario();
                BloquearCampos();
                ConfigurarEstadoInicialBotones();
            }
        }

        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            if (_cuentaSeleccionada == null)
            {
                MessageBox.Show("SELECCIONE UNA CUENTA DE LA TABLA PRIMERO.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿DESEA INACTIVAR LA CUENTA:\n\n{_cuentaSeleccionada.Name}?",
                "CONFIRMAR INACTIVACIÓN",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                // Ctrl_Accounts.InactivarCuenta(_cuentaSeleccionada.AccountId);
                // Pendiente de implementar en Ctrl_Accounts
                MessageBox.Show("FUNCIONALIDAD DE INACTIVACIÓN PENDIENTE.", "INFO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            Txt_ValorBuscado.Clear();
            RefrescarTabla();
            BloquearCampos();
            ConfigurarEstadoInicialBotones();
        }

        #endregion BotonesAccion
        #region Busqueda
        private void Txt_ValorBuscado_TextChanged(object sender, EventArgs e)
        {
            EjecutarBusqueda();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                EjecutarBusqueda();
            }
        }
        private void EjecutarBusqueda()
        {
            try
            {
                if (Txt_ValorBuscado.Text.Length > 0)
                    Tabla.DataSource = Ctrl_Accounts.BuscarCuentas(Txt_ValorBuscado.Text);
                else
                    Tabla.DataSource = Ctrl_Accounts.MostrarCuentas();

                ConfigurarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR EN BÚSQUEDA: " + ex.Message, "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Busqueda
        #region ImportarExportar

        private void Btn_Import_Click(object sender, EventArgs e)
        {
            MessageBox.Show("FUNCIONALIDAD DE IMPORTACIÓN EN DESARROLLO.", "PRÓXIMAMENTE",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener lista completa de cuentas
                List<Mdl_Accounts> todasLasCuentas = Ctrl_Accounts.MostrarCuentas();

                if (todasLasCuentas == null || todasLasCuentas.Count == 0)
                {
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR.", "INFORMACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Diálogo para guardar archivo
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Catálogo de Cuentas",
                    FileName = $"CatalogoCuentas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

                this.Cursor = Cursors.WaitCursor;

                GenerarReporteExcel(todasLasCuentas, saveFileDialog.FileName);

                this.Cursor = Cursors.Default;

                var resultado = MessageBox.Show(
                    "REPORTE EXPORTADO EXITOSAMENTE.\n\n¿DESEA ABRIR EL ARCHIVO AHORA?",
                    "EXPORTACIÓN EXITOSA",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (resultado == DialogResult.Yes)
                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL EXPORTAR: {ex.Message}", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerarReporteExcel(List<Mdl_Accounts> cuentas, string rutaArchivo)
        {
            var excelApp = new Excel.Application();
            var workbook = excelApp.Workbooks.Add();
            var worksheet = (Excel.Worksheet)workbook.Sheets[1];
            worksheet.Name = "Catálogo de Cuentas";

            // ── TÍTULO ──────────────────────────────────────────────
            worksheet.Cells[1, 1] = "CATÁLOGO DE CUENTAS CONTABLES";
            worksheet.Range["A1:N1"].Merge();
            worksheet.Range["A1:N1"].Font.Size = 16;
            worksheet.Range["A1:N1"].Font.Bold = true;
            worksheet.Range["A1:N1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            worksheet.Range["A1:N1"].Interior.Color =
                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
            worksheet.Range["A1:N1"].Font.Color =
                System.Drawing.ColorTranslator.ToOle(Color.White);

            // ── INFORMACIÓN DEL REPORTE ─────────────────────────────
            worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SECRON"}";
            worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            worksheet.Cells[4, 1] = $"TOTAL CUENTAS: {cuentas.Count}";
            worksheet.Range["A2:A4"].Font.Size = 10;
            worksheet.Range["A2:A4"].Font.Bold = true;

            // ── ENCABEZADOS ─────────────────────────────────────────
            int headerRow = 6;
            string[] headers =
            {
        "CÓDIGO", "NOMBRE", "TIPO", "CÓD.CUENTA PADRE", "NIVEL",
        "SIGNO", "SALDO", "CÓD.BANCO", "BANCO",
        "TIPO CUENTA BANCARIA", "PRÓX.CHEQUE",
        "MONEDA", "NOMBRE MONEDA"
    };

            for (int i = 0; i < headers.Length; i++)
                worksheet.Cells[headerRow, i + 1] = headers[i];

            var headerRange = worksheet.Range[$"A{headerRow}:M{headerRow}"];
            headerRange.Font.Bold = true;
            headerRange.Font.Size = 11;
            headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
            headerRange.Interior.Color =
                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
            headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

            // ── DATOS ───────────────────────────────────────────────
            int row = headerRow + 1;
            foreach (var cuenta in cuentas)
            {
                // Sangría visual en nombre según nivel (igual que en la tabla)
                string sangria = new string(' ', cuenta.Level * 2);

                worksheet.Cells[row, 1] = cuenta.Code ?? "";
                worksheet.Cells[row, 2] = sangria + (cuenta.Name ?? "");
                worksheet.Cells[row, 3] = cuenta.Type ?? "";
                worksheet.Cells[row, 4] = cuenta.ParentAccountCode ?? "";
                worksheet.Cells[row, 5] = cuenta.Level;
                worksheet.Cells[row, 6] = cuenta.Sign ?? "";
                worksheet.Cells[row, 7] = cuenta.Balance.ToString("N2");
                worksheet.Cells[row, 8] = cuenta.BankCode;
                worksheet.Cells[row, 9] = cuenta.BankName ?? "";
                worksheet.Cells[row, 10] = cuenta.BankAccountType ?? "";
                worksheet.Cells[row, 11] = cuenta.CheckNumber;
                worksheet.Cells[row, 12] = cuenta.Currency ?? "";
                worksheet.Cells[row, 13] = cuenta.CurrencyName ?? "";

                // Color verde oscuro para nivel 1 (igual que en la tabla visual)
                if (cuenta.Level == 1)
                {
                    worksheet.Range[$"A{row}:M{row}"].Interior.Color =
                        System.Drawing.ColorTranslator.ToOle(Color.DarkGreen);
                    worksheet.Range[$"A{row}:M{row}"].Font.Color =
                        System.Drawing.ColorTranslator.ToOle(Color.White);
                    worksheet.Range[$"A{row}:M{row}"].Font.Bold = true;
                }
                else if (cuenta.Level == 2)
                {
                    worksheet.Range[$"A{row}:M{row}"].Font.Bold = true;

                    // Filas alternas solo para niveles > 1
                    if (row % 2 == 0)
                        worksheet.Range[$"A{row}:M{row}"].Interior.Color =
                            System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                }
                else
                {
                    if (row % 2 == 0)
                        worksheet.Range[$"A{row}:M{row}"].Interior.Color =
                            System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                }

                row++;
            }

            // ── BORDES Y FORMATO FINAL ──────────────────────────────
            var dataRange = worksheet.Range[$"A{headerRow}:M{row - 1}"];
            dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

            worksheet.Columns.AutoFit();
            worksheet.Columns[2].ColumnWidth = 45; // Nombre de cuenta más ancho

            // Congelar encabezados
            worksheet.Activate();
            excelApp.ActiveWindow.SplitRow = headerRow;
            excelApp.ActiveWindow.FreezePanes = true;

            // ── PIE DE PÁGINA ───────────────────────────────────────
            worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
            worksheet.Range[$"A{row + 1}:M{row + 1}"].Merge();
            worksheet.Range[$"A{row + 1}:M{row + 1}"].Font.Italic = true;
            worksheet.Range[$"A{row + 1}:M{row + 1}"].Font.Size = 9;
            worksheet.Range[$"A{row + 1}:M{row + 1}"].HorizontalAlignment =
                Excel.XlHAlign.xlHAlignCenter;

            // ── GUARDAR Y LIBERAR ───────────────────────────────────
            workbook.SaveAs(rutaArchivo);
            workbook.Close();
            excelApp.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }

        #endregion ImportarExportar
    }
}