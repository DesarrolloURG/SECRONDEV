using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_FixedAsset_Movements : Form
    {
        #region PropiedadesIniciales

        private int _selectedTransferId = 0;
        private bool _modoEdicion = false;
        private List<Mdl_FixedAssetTransfer> _movimientosList;
        public Mdl_Security_UserInfo UserData { get; set; }

        #endregion

        #region Constructor

        public Frm_FixedAsset_Movements()
        {
            InitializeComponent();
        }

        #endregion

        #region Load

        private void Frm_FixedAsset_Movements_Load(object sender, EventArgs e)
        {
            ConfigurarTabla();
            ConfigurarFiltros();
            CargarComboChangeState();
            CargarMovimientos();
            EstadoInicial();
            InicializarScroll();
            ConfigurarEventosScroll();
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
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "CÓDIGO", DataPropertyName = "TransferCode", Width = 120 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colActivoCodigo", HeaderText = "COD. ACTIVO", DataPropertyName = "AssetCode", Width = 100 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colActivoNombre", HeaderText = "ACTIVO", DataPropertyName = "AssetName", Width = 180 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "ESTADO", DataPropertyName = "StatusName", Width = 110 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "FECHA", DataPropertyName = "TransferDate", Width = 100 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOrigen", HeaderText = "ORIGEN", DataPropertyName = "FromWarehouseName", Width = 140 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDestino", HeaderText = "DESTINO", DataPropertyName = "ToWarehouseName", Width = 140 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMotivo", HeaderText = "MOTIVO", DataPropertyName = "Reason", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        }

        private void ConfigurarFiltros()
        {
            Filtro1.Items.Clear();
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.Items.Add("POR ACTIVO");
            Filtro1.SelectedIndex = 0;

            Filtro2.DataSource = null;
            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS LOS ESTADOS");
            List<KeyValuePair<int, string>> estados =
                Ctrl_FixedAssetTransferStatus.ObtenerEstadosParaCombo(soloActivos: false);
            foreach (var estado in estados)
                Filtro2.Items.Add(estado);
            Filtro2.SelectedIndex = 0;
            Filtro2.DisplayMember = "Value";

            Filtro3.Items.Clear();
            Filtro3.Items.Add("SIN FILTRO DE FECHA");
            Filtro3.Items.Add("CON FILTRO DE FECHA");
            Filtro3.SelectedIndex = 0;

            CheckBox_FiltroFechas.Checked = false;
            DTP_FechaInicio.Enabled = false;
            DTP_FechaFin.Enabled = false;
            DTP_FechaInicio.Value = DateTime.Today.AddMonths(-1);
            DTP_FechaFin.Value = DateTime.Today;

            ComboBox_SelectTo.Items.Clear();
            ComboBox_SelectTo.Items.Add("BODEGA");
            ComboBox_SelectTo.Items.Add("EMPLEADO");
            ComboBox_SelectTo.SelectedIndex = 0;
        }

        private void CargarComboChangeState()
        {
            ComboBox_ChangeState.DataSource = null;
            List<KeyValuePair<int, string>> estados =
                Ctrl_FixedAssetTransferStatus.ObtenerEstadosParaCombo(soloActivos: true);
            ComboBox_ChangeState.DataSource = estados;
            ComboBox_ChangeState.DisplayMember = "Value";
            ComboBox_ChangeState.ValueMember = "Key";
        }

        private void EstadoInicial()
        {
            _selectedTransferId = 0;
            _modoEdicion = false;

            Txt_TransferId.Clear();
            Txt_TransferId.ReadOnly = _modoEdicion;
            DTP_TransferDate.Value = DateTime.Today;
            Txt_Asset.Clear();
            Txt_AssetId.Clear();
            Txt_FromWarehouse.Clear();
            Txt_FromEmployee.Clear();
            Txt_TransferState.Clear();
            Txt_Reason.Clear();

            ComboBox_SelectTo.SelectedIndex = 0;
            ComboBox_ToWarehouse.Enabled = true;
            ComboBox_ToEmployee.Enabled = false;

            Btn_Save.Visible = true;
            Btn_Update.Visible = false;
            Btn_Clear.Visible = true;
            Btn_Cancel.Visible = false;
        }

        #endregion

        #region Carga de datos

        private void CargarMovimientos(
            string transferCode = null,
            string assetCode = null,
            int? transferStatusId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            try
            {
                _movimientosList = Ctrl_FixedAssetMovements.MostrarMovimientos(
                    transferCode: transferCode,
                    assetCode: assetCode,
                    transferStatusId: transferStatusId,
                    fechaInicio: fechaInicio,
                    fechaFin: fechaFin);

                Tabla.DataSource = null;
                Tabla.DataSource = _movimientosList;

                Lbl_Paginas.Text = $"Total: {_movimientosList.Count} registro(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar movimientos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Búsqueda

        private void Btn_Search_Click(object sender, EventArgs e)
        {

        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Clear();
            Filtro2.SelectedIndex = 0;
            CheckBox_FiltroFechas.Checked = false;
            CargarMovimientos();
        }

        private void CheckBox_FiltroFechas_CheckedChanged(object sender, EventArgs e)
        {
            DTP_FechaInicio.Enabled = CheckBox_FiltroFechas.Checked;
            DTP_FechaFin.Enabled = CheckBox_FiltroFechas.Checked;
        }

        #endregion

        #region Selección en tabla

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = Tabla.Rows[e.RowIndex];

            _selectedTransferId = Convert.ToInt32(row.Cells["colId"].Value);
            Txt_TransferId.Text = row.Cells["colCodigo"].Value?.ToString();
            Txt_Asset.Text = row.Cells["colActivoNombre"].Value?.ToString();
            Txt_AssetId.Text = row.Cells["colActivoCodigo"].Value?.ToString();
            Txt_FromWarehouse.Text = row.Cells["colOrigen"].Value?.ToString();
            Txt_TransferState.Text = row.Cells["colEstado"].Value?.ToString();
            Txt_Reason.Text = row.Cells["colMotivo"].Value?.ToString();

            if (DateTime.TryParse(row.Cells["colFecha"].Value?.ToString(), out DateTime fecha))
                DTP_TransferDate.Value = fecha;

            Txt_TransferId.ReadOnly = true;
            _modoEdicion = true;

            string statusName = row.Cells["colEstado"].Value?.ToString();
            Btn_Cancel.Visible = statusName == "PENDIENTE";
            Btn_Save.Visible = false;
            Btn_Update.Visible = true;
        }

        #endregion

        #region Cambio de destino

        private void ComboBox_SelectTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool esBodega = ComboBox_SelectTo.SelectedItem?.ToString() == "BODEGA";
            ComboBox_ToWarehouse.Enabled = esBodega;
            ComboBox_ToEmployee.Enabled = !esBodega;
        }

        #endregion

        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            // Por implementar — lógica de nuevo traslado
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (_selectedTransferId == 0) return;

            if (ComboBox_ToWarehouse.SelectedValue == null && ComboBox_ToEmployee.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un destino (bodega o empleado).", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Mdl_FixedAssetTransfer transfer = new Mdl_FixedAssetTransfer
            {
                TransferId = _selectedTransferId,
                TransferCode = Txt_TransferId.Text.Trim().ToUpper(),
                TransferDate = DTP_TransferDate.Value.Date,
                TransferStatusId = ComboBox_ChangeState.SelectedValue != null
                                   ? (int)ComboBox_ChangeState.SelectedValue : 0,
                Reason = Txt_Reason.Text.Trim(),
                ToWarehouseId = ComboBox_SelectTo.SelectedItem?.ToString() == "BODEGA" &&
                                   ComboBox_ToWarehouse.SelectedValue != null
                                   ? (int?)ComboBox_ToWarehouse.SelectedValue : null,
                ToEmployeeId = ComboBox_SelectTo.SelectedItem?.ToString() == "EMPLEADO" &&
                                   ComboBox_ToEmployee.SelectedValue != null
                                   ? (int?)ComboBox_ToEmployee.SelectedValue : null,
                ModifiedBy = UserData?.UserId
            };

            int resultado = Ctrl_FixedAssetMovements.ActualizarMovimiento(transfer);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Traslado actualizado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarMovimientos();
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("El traslado no existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("El código de traslado ya está en uso por otro registro.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -3:
                    MessageBox.Show("Debe definir al menos un destino (bodega o empleado).", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al actualizar el traslado.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            if (_selectedTransferId == 0) return;

            DialogResult confirm = MessageBox.Show(
                "¿Está seguro que desea cancelar este traslado?",
                "Confirmar cancelación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            int resultado = Ctrl_FixedAssetMovements.InactivarMovimiento(
                _selectedTransferId, UserData?.UserId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Traslado cancelado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarMovimientos();
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("El traslado no existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("Solo se pueden cancelar traslados en estado PENDIENTE.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al cancelar el traslado.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            EstadoInicial();
        }

        #endregion

        #region Estados

        private void Btn_States_Click(object sender, EventArgs e)
        {
            Frm_FixedAsset_StatusMovements frm = new Frm_FixedAsset_StatusMovements
            {
                UserData = UserData
            };
            frm.ShowDialog();

            CargarComboChangeState();
            ConfigurarFiltros();
        }

        private void Btn_Transfer_Click(object sender, EventArgs e)
        {
            // Por implementar — lógica de nuevo traslado
        }

        #endregion

        #region ExportarExcel

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string valor = Txt_ValorBuscado.Text.Trim().ToUpper();
                string filtro = Filtro1.SelectedItem?.ToString();
                string transferCode = filtro == "POR CÓDIGO" ? valor : null;
                string assetCode = filtro == "POR ACTIVO" ? valor : null;

                int? statusId = null;
                if (Filtro2.SelectedIndex > 0 && Filtro2.SelectedItem is KeyValuePair<int, string> kvp)
                    statusId = kvp.Key;

                DateTime? fechaInicio = null;
                DateTime? fechaFin = null;
                if (CheckBox_FiltroFechas.Checked)
                {
                    fechaInicio = DTP_FechaInicio.Value.Date;
                    fechaFin = DTP_FechaFin.Value.Date;
                }

                List<Mdl_FixedAssetTransfer> lista = Ctrl_FixedAssetMovements.MostrarMovimientos(
                    transferCode: transferCode,
                    assetCode: assetCode,
                    transferStatusId: statusId,
                    fechaInicio: fechaInicio,
                    fechaFin: fechaFin);

                if (lista == null || lista.Count == 0)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("NO HAY DATOS PARA EXPORTAR.", "INFORMACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Traslados de Activos Fijos",
                    FileName = $"ActivosFijos_Traslados_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                var excelApp = new Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                worksheet.Name = "Traslados";

                worksheet.Cells[1, 1] = "TRASLADOS DE ACTIVOS FIJOS - SECRON";
                worksheet.Range["A1:I1"].Merge();
                worksheet.Range["A1:I1"].Font.Size = 16;
                worksheet.Range["A1:I1"].Font.Bold = true;
                worksheet.Range["A1:I1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                worksheet.Range["A1:I1"].Interior.Color =
                    System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                worksheet.Range["A1:I1"].Font.Color =
                    System.Drawing.ColorTranslator.ToOle(Color.White);

                worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SECRON"}";
                worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {lista.Count}";

                int headerRow = 6;
                string[] headers = {
                    "CÓDIGO TRASLADO", "COD. ACTIVO", "NOMBRE ACTIVO",
                    "ESTADO", "FECHA TRASLADO",
                    "ORIGEN", "DESTINO",
                    "MOTIVO", "CREADO POR"
                };

                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cells[headerRow, i + 1] = headers[i];

                var headerRange = worksheet.Range[$"A{headerRow}:I{headerRow}"];
                headerRange.Font.Bold = true;
                headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                headerRange.Interior.Color =
                    System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                int row = headerRow + 1;
                foreach (var t in lista)
                {
                    worksheet.Cells[row, 1] = t.TransferCode;
                    worksheet.Cells[row, 2] = t.AssetCode;
                    worksheet.Cells[row, 3] = t.AssetName;
                    worksheet.Cells[row, 4] = t.StatusName;
                    worksheet.Cells[row, 5] = t.TransferDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 6] = t.FromWarehouseName ?? t.FromEmployeeName ?? "";
                    worksheet.Cells[row, 7] = t.ToWarehouseName ?? t.ToEmployeeName ?? "";
                    worksheet.Cells[row, 8] = t.Reason ?? "";
                    worksheet.Cells[row, 9] = t.CreatedByName ?? "";

                    if (row % 2 == 0)
                        worksheet.Range[$"A{row}:I{row}"].Interior.Color =
                            System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));

                    row++;
                }

                var dataRange = worksheet.Range[$"A{headerRow}:I{row - 1}"];
                dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                worksheet.Columns.AutoFit();
                worksheet.Columns[3].ColumnWidth = 35;
                worksheet.Columns[8].ColumnWidth = 40;

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

        #region ScrollBar

        private void InicializarScroll()
        {
            if (Panel_Izquierdo == null || vScrollBar == null)
                return;

            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                    ctrl.Tag = "OrigY:" + ctrl.Top;
            }

            int maxBottom = 0;
            foreach (Control ctrl in Panel_Izquierdo.Controls)
                maxBottom = Math.Max(maxBottom, ctrl.Bottom);

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

        private void ConfigurarEventosScroll()
        {
            if (Panel_Izquierdo == null || vScrollBar == null)
                return;

            Panel_Izquierdo.TabStop = true;
            Panel_Izquierdo.MouseWheel += Panel_Izquierdo_MouseWheel;
            Panel_Izquierdo.MouseEnter += Panel_Izquierdo_MouseEnter;

            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (!(ctrl is ComboBox))
                    ctrl.MouseWheel += Panel_Izquierdo_MouseWheel;
            }
        }

        private void Panel_Izquierdo_MouseEnter(object sender, EventArgs e)
        {
            Panel_Izquierdo.Focus();
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            int scrollPosition = vScrollBar.Value;

            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (ctrl is ComboBox) continue;

                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                    ctrl.Tag = "OrigY:" + ctrl.Top;

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
                if (ctrl is ComboBox) continue;

                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                    ctrl.Tag = "OrigY:" + ctrl.Top;

                string[] parts = ctrl.Tag.ToString().Split(':');
                int originalY = int.Parse(parts[1]);
                ctrl.Top = originalY - scrollPosition;
            }

            Panel_Izquierdo.Invalidate();
        }

        #endregion
    }
}