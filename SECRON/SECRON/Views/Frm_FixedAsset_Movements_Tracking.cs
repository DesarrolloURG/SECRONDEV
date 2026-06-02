using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_FixedAsset_Movements_Tracking : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_FixedAssetTransfer> _trasladosList;
        private int _selectedTransferId = 0;
        private int _selectedTransferStatusId = 0;
        private int _selectedDetailId = 0;

        #endregion

        #region Constructor

        public Frm_FixedAsset_Movements_Tracking()
        {
            InitializeComponent();
            Btn_RemoveAsset.Click += new EventHandler(Btn_RemoveAsset_Click);
        }

        #endregion

        #region Load

        private void Frm_FixedAsset_Movements_Tracking_Load(object sender, EventArgs e)
        {
            ConfigurarTamañoFormulario();
            ConfigurarTabla();
            ConfigurarTablaDetalles();
            ConfigurarFiltros();
            ConfigurarFechas();
            ConfigurarComponentesDeshabilitados();
            CargarTraslados();
            LimpiarSeleccion();
        }

        #endregion

        #region TamañoFormulario

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new System.Drawing.Size(1200, 880);
            this.MinimumSize = new System.Drawing.Size(1200, 880);
            this.MaximumSize = new System.Drawing.Size(1200, 880);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
        }

        #endregion

        #region ComponentesDeshabilitados

        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_Reason.Enabled = false;
        }

        #endregion

        #region Configuración

        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "ID", DataPropertyName = "TransferId", Visible = false });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatusId", HeaderText = "STATUS_ID", DataPropertyName = "TransferStatusId", Visible = false });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatusCode", HeaderText = "STATUS_CODE", DataPropertyName = "StatusCode", Visible = false });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "CÓDIGO", DataPropertyName = "TransferCode", Width = 120 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "ESTADO", DataPropertyName = "StatusName", Width = 120 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "FECHA", DataPropertyName = "TransferDate", Width = 100 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDestino", HeaderText = "DESTINO", DataPropertyName = "ToWarehouseName", Width = 150 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDestinoEmp", HeaderText = "EMP. DESTINO", DataPropertyName = "ToEmployeeName", Width = 150 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMotivo", HeaderText = "MOTIVO", DataPropertyName = "Reason", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCreadoPor", HeaderText = "CREADO POR", DataPropertyName = "CreatedByName", Width = 140 });

        }

        private void ConfigurarTablaDetalles()
        {
            TablaDetalles.Columns.Clear();
            TablaDetalles.AutoGenerateColumns = false;
            TablaDetalles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TablaDetalles.MultiSelect = false;
            TablaDetalles.ReadOnly = true;
            TablaDetalles.AllowUserToAddRows = false;
            TablaDetalles.RowHeadersVisible = false;

            TablaDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetId", HeaderText = "ID", DataPropertyName = "TransferDetailId", Visible = false });
            TablaDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodActivo", HeaderText = "CÓDIGO ACTIVO", DataPropertyName = "AssetCode", Width = 130 });
            TablaDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombreActivo", HeaderText = "NOMBRE ACTIVO", DataPropertyName = "AssetName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            TablaDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOrigenBodega", HeaderText = "BODEGA ORIGEN", DataPropertyName = "FromWarehouseName", Width = 150 });
            TablaDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOrigenEmp", HeaderText = "EMPLEADO ORIGEN", DataPropertyName = "FromEmployeeName", Width = 150 });

        }

        private void ConfigurarFiltros()
        {
            Filtro1.Items.Clear();
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.SelectedIndex = 0;
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;

            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODAS LAS BODEGAS");
            var bodegas = Ctrl_Warehouses.ObtenerBodegasParaCombo();
            foreach (var b in bodegas)
                Filtro2.Items.Add(b);
            Filtro2.SelectedIndex = 0;
            Filtro2.DisplayMember = "Value";
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;

            Filtro3.Visible = false;

            // ComboBox_Estado — binding correcto con KeyValuePair
            ComboBox_Estado.Items.Clear();
            ComboBox_Estado.Items.Add(new KeyValuePair<int, string>(0, "TODOS LOS ESTADOS"));
            var estados = Ctrl_FixedAssetTransferStatus.ObtenerEstadosParaCombo(soloActivos: false);
            foreach (var est in estados)
                ComboBox_Estado.Items.Add(est);
            ComboBox_Estado.DisplayMember = "Value";
            ComboBox_Estado.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Estado.SelectedIndex = 0;

            ComboBox_NewState.Items.Clear();
            ComboBox_NewState.DisplayMember = "Value";
            ComboBox_NewState.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_NewState.Enabled = false;
        }

        private void ConfigurarFechas()
        {
            DTP_FechaInicio.Value = DateTime.Today.AddMonths(-1);
            DTP_FechaFin.Value = DateTime.Today;
            DTP_FechaInicio.Enabled = CheckBox_FiltroFechas.Checked;
            DTP_FechaFin.Enabled = CheckBox_FiltroFechas.Checked;
        }

        #endregion

        #region CargaDeDatos

        private void CargarTraslados(
            string transferCode = null,
            int? transferStatusId = null,
            int? toWarehouseId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            try
            {
                _trasladosList = Ctrl_FixedAssetTransfers.MostrarTraslados(
                    transferCode: transferCode,
                    transferStatusId: transferStatusId,
                    toWarehouseId: toWarehouseId,
                    fechaInicio: fechaInicio,
                    fechaFin: fechaFin);

                Tabla.DataSource = null;
                Tabla.DataSource = _trasladosList;

                Lbl_Info.Text = $"TOTAL: {_trasladosList.Count} TRASLADO(S)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar traslados: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDetalles(int transferId)
        {
            try
            {
                var detalles = Ctrl_FixedAssetTransfers.MostrarDetalles(transferId);
                TablaDetalles.DataSource = null;
                TablaDetalles.DataSource = detalles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalles del traslado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarTransicionesDisponibles(int fromStatusId)
        {
            try
            {
                ComboBox_NewState.Items.Clear();

                var transiciones = Ctrl_FixedAssetTransferStatusTransitions.MostrarTransiciones(
                    fromStatusId: fromStatusId);

                if (transiciones.Count == 0)
                {
                    ComboBox_NewState.Enabled = false;
                    Btn_Yes.Enabled = false;
                    return;
                }

                var estados = Ctrl_FixedAssetTransferStatus.MostrarEstados(isActive: true);

                foreach (var t in transiciones)
                {
                    var estado = estados.Find(s => s.TransferStatusId == t.ToStatusId);
                    if (estado != null)
                        ComboBox_NewState.Items.Add(
                            new KeyValuePair<int, string>(estado.TransferStatusId, estado.StatusName));
                }

                ComboBox_NewState.DisplayMember = "Value";
                ComboBox_NewState.ValueMember = "Key";

                if (ComboBox_NewState.Items.Count > 0)
                    ComboBox_NewState.SelectedIndex = 0;

                ComboBox_NewState.Enabled = true;
                Btn_Yes.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar transiciones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region SelecciónEnTabla

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = Tabla.Rows[e.RowIndex];

                _selectedTransferId = Convert.ToInt32(row.Cells["colId"].Value);
                _selectedTransferStatusId = Convert.ToInt32(row.Cells["colStatusId"].Value);
                _selectedDetailId = 0;

                string codigo = row.Cells["colCodigo"].Value?.ToString();
                string estado = row.Cells["colEstado"].Value?.ToString();
                string motivo = row.Cells["colMotivo"].Value?.ToString();
                string statusCode = row.Cells["colStatusCode"].Value?.ToString();

                Txt_Reason.Text = motivo ?? "";
                Lbl_Info.Text = $"TRASLADO SELECCIONADO: {codigo} | ESTADO ACTUAL: {estado}";

                CargarDetalles(_selectedTransferId);
                CargarTransicionesDisponibles(_selectedTransferStatusId);

                bool esFinal = statusCode == "COMPLETED" || statusCode == "REJECTED";
                Btn_CancelTransfer.Enabled = !esFinal;
                Btn_RemoveAsset.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR EN SELECCIÓN: {ex.Message}", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TablaDetalles_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = TablaDetalles.Rows[e.RowIndex];
            _selectedDetailId = Convert.ToInt32(row.Cells["colDetId"].Value);

            // Solo permitir quitar activos si el traslado está en PENDING
            var traslado = _trasladosList?.Find(t => t.TransferId == _selectedTransferId);
            Btn_RemoveAsset.Enabled = traslado != null && traslado.StatusCode == "PENDING";
        }

        #endregion

        #region BúsquedaYFiltros

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string valor = Txt_ValorBuscado.Text.Trim().ToUpper();
            string filtro = Filtro1.SelectedItem?.ToString();
            string transferCode = filtro == "POR CÓDIGO" ? valor : null;

            int? statusId = null;
            if (ComboBox_Estado.SelectedItem is KeyValuePair<int, string> kvpEstado && kvpEstado.Key > 0)
                statusId = kvpEstado.Key;

            int? warehouseId = null;
            if (Filtro2.SelectedIndex > 0 && Filtro2.SelectedItem is KeyValuePair<int, string> kvpBodega)
                warehouseId = kvpBodega.Key;

            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;
            if (CheckBox_FiltroFechas.Checked)
            {
                fechaInicio = DTP_FechaInicio.Value.Date;
                fechaFin = DTP_FechaFin.Value.Date;
            }

            CargarTraslados(
                transferCode: transferCode,
                transferStatusId: statusId,
                toWarehouseId: warehouseId,
                fechaInicio: fechaInicio,
                fechaFin: fechaFin);

            LimpiarSeleccion();
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Clear();
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            ComboBox_Estado.SelectedIndex = 0;
            CheckBox_FiltroFechas.Checked = false;
            CargarTraslados();
            LimpiarSeleccion();
        }

        private void CheckBox_FiltroFechas_CheckedChanged(object sender, EventArgs e)
        {
            DTP_FechaInicio.Enabled = CheckBox_FiltroFechas.Checked;
            DTP_FechaFin.Enabled = CheckBox_FiltroFechas.Checked;
        }

        #endregion

        #region Acciones

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (_selectedTransferId == 0)
            {
                MessageBox.Show("DEBE SELECCIONAR UN TRASLADO DE LA LISTA.",
                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(ComboBox_NewState.SelectedItem is KeyValuePair<int, string> kvp))
            {
                MessageBox.Show("DEBE SELECCIONAR UN NUEVO ESTADO PARA EL TRASLADO.",
                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int nuevoStatusId = kvp.Key;
            string nuevoStatusNombre = kvp.Value;

            DialogResult confirm = MessageBox.Show(
                $"¿CONFIRMA CAMBIAR EL ESTADO DEL TRASLADO A '{nuevoStatusNombre}'?",
                "CONFIRMAR CAMBIO", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            var traslado = _trasladosList?.Find(t => t.TransferId == _selectedTransferId);
            if (traslado == null) return;

            traslado.TransferStatusId = nuevoStatusId;
            traslado.ModifiedBy = UserData?.UserId;

            // Registrar aprobación o completado si aplica
            var estados = Ctrl_FixedAssetTransferStatus.MostrarEstados(isActive: true);
            var estadoNuevo = estados.Find(s => s.TransferStatusId == nuevoStatusId);

            if (estadoNuevo?.StatusCode == "APPROVED")
            {
                traslado.ApprovedByUserId = UserData?.UserId;
                traslado.ApprovedDate = DateTime.Now;
            }
            else if (estadoNuevo?.StatusCode == "COMPLETED")
            {
                traslado.CompletedDate = DateTime.Now;
            }

            int resultado = Ctrl_FixedAssetTransfers.ActualizarTraslado(traslado);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("ESTADO DEL TRASLADO ACTUALIZADO CORRECTAMENTE.",
                        "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarTraslados();
                    LimpiarSeleccion();
                    break;
                case -1:
                    MessageBox.Show("EL TRASLADO NO EXISTE.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("EL CÓDIGO DE TRASLADO YA EXISTE EN OTRO REGISTRO.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -3:
                    MessageBox.Show("DEBE DEFINIR UN DESTINO PARA EL TRASLADO.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -4:
                    MessageBox.Show("LA TRANSICIÓN DE ESTADO NO ESTÁ PERMITIDA.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("ERROR AL ACTUALIZAR EL ESTADO.", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_CancelTransfer_Click(object sender, EventArgs e)
        {
            if (_selectedTransferId == 0)
            {
                MessageBox.Show("DEBE SELECCIONAR UN TRASLADO DE LA LISTA.",
                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string codigo = _trasladosList?.Find(t => t.TransferId == _selectedTransferId)?.TransferCode ?? "";

            DialogResult confirm = MessageBox.Show(
                $"¿CONFIRMA CANCELAR EL TRASLADO '{codigo}'?\n\nTodos los activos del traslado regresarán a estado ACTIVO.\nESTA ACCIÓN NO SE PUEDE DESHACER.",
                "CONFIRMAR CANCELACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            int resultado = Ctrl_FixedAssetTransfers.InactivarTraslado(
                _selectedTransferId, UserData?.UserId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("TRASLADO CANCELADO CORRECTAMENTE. LOS ACTIVOS HAN SIDO RESTAURADOS A ESTADO ACTIVO.",
                        "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarTraslados();
                    LimpiarSeleccion();
                    break;
                case -1:
                    MessageBox.Show("EL TRASLADO NO EXISTE.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("NO SE PUEDE CANCELAR UN TRASLADO QUE YA ESTÁ EN ESTADO FINAL.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("ERROR AL CANCELAR EL TRASLADO.", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_RemoveAsset_Click(object sender, EventArgs e)
        {
            if (_selectedDetailId == 0)
            {
                MessageBox.Show("DEBE SELECCIONAR UN ACTIVO DE LA LISTA DE DETALLES.",
                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener nombre del activo para el mensaje
            string nombreActivo = "";
            if (TablaDetalles.CurrentRow != null)
                nombreActivo = TablaDetalles.CurrentRow.Cells["colNombreActivo"].Value?.ToString() ?? "";

            DialogResult confirm = MessageBox.Show(
                $"¿CONFIRMA QUITAR EL ACTIVO '{nombreActivo}' DEL TRASLADO?\n\nEl activo regresará a estado ACTIVO.",
                "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            int resultado = Ctrl_FixedAssetTransfers.EliminarDetalle(_selectedDetailId, UserData?.UserId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("ACTIVO QUITADO DEL TRASLADO CORRECTAMENTE.",
                        "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDetalles(_selectedTransferId);
                    _selectedDetailId = 0;
                    Btn_RemoveAsset.Enabled = false;
                    break;
                case -1:
                    MessageBox.Show("EL DETALLE NO EXISTE.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("SOLO SE PUEDEN QUITAR ACTIVOS DE TRASLADOS EN ESTADO PENDIENTE.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("ERROR AL QUITAR EL ACTIVO DEL TRASLADO.", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_LimpiarSeleccion_Click(object sender, EventArgs e)
        {
            LimpiarSeleccion();
        }

        #endregion

        #region Helpers

        private void LimpiarSeleccion()
        {
            _selectedTransferId = 0;
            _selectedTransferStatusId = 0;
            _selectedDetailId = 0;

            Txt_Reason.Clear();
            TablaDetalles.DataSource = null;
            ComboBox_NewState.Items.Clear();
            ComboBox_NewState.Enabled = false;
            Btn_Yes.Enabled = false;
            Btn_CancelTransfer.Enabled = false;
            Btn_RemoveAsset.Enabled = false;

            Lbl_Info.Text = "SELECCIONE UN TRASLADO DE LA LISTA PARA VER SUS DETALLES.";
        }

        #endregion

        #region ExportarExcel

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (_trasladosList == null || _trasladosList.Count == 0)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR.", "INFORMACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Tracking de Traslados",
                    FileName = $"Tracking_Traslados_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                var excelApp = new Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                worksheet.Name = "Tracking";

                worksheet.Cells[1, 1] = "TRACKING DE TRASLADOS - SECRON";
                worksheet.Range["A1:G1"].Merge();
                worksheet.Range["A1:G1"].Font.Size = 16;
                worksheet.Range["A1:G1"].Font.Bold = true;
                worksheet.Range["A1:G1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                worksheet.Range["A1:G1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                worksheet.Range["A1:G1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SECRON"}";
                worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {_trasladosList.Count}";

                int headerRow = 6;
                string[] headers = { "CÓDIGO", "ESTADO", "FECHA", "BODEGA DESTINO", "EMP. DESTINO", "MOTIVO", "CREADO POR" };
                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cells[headerRow, i + 1] = headers[i];

                var headerRange = worksheet.Range[$"A{headerRow}:G{headerRow}"];
                headerRange.Font.Bold = true;
                headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                int row = headerRow + 1;
                foreach (var t in _trasladosList)
                {
                    worksheet.Cells[row, 1] = t.TransferCode;
                    worksheet.Cells[row, 2] = t.StatusName;
                    worksheet.Cells[row, 3] = t.TransferDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4] = t.ToWarehouseName ?? "";
                    worksheet.Cells[row, 5] = t.ToEmployeeName ?? "";
                    worksheet.Cells[row, 6] = t.Reason ?? "";
                    worksheet.Cells[row, 7] = t.CreatedByName ?? "";

                    if (row % 2 == 0)
                        worksheet.Range[$"A{row}:G{row}"].Interior.Color =
                            System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                    row++;
                }

                var dataRange = worksheet.Range[$"A{headerRow}:G{row - 1}"];
                dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;
                worksheet.Columns.AutoFit();
                worksheet.Columns[6].ColumnWidth = 40;

                workbook.SaveAs(saveDialog.FileName);
                workbook.Close();
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                this.Cursor = Cursors.Default;

                var result = MessageBox.Show(
                    "ARCHIVO EXPORTADO EXITOSAMENTE.\n\n¿DESEA ABRIR EL ARCHIVO AHORA?",
                    "EXPORTACIÓN EXITOSA", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    System.Diagnostics.Process.Start(saveDialog.FileName);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL EXPORTAR: {ex.Message}", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}