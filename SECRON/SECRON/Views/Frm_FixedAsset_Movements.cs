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
        private List<Mdl_FixedAssetTransfer> _trasladosList;

        // Lista temporal de activos agregados al traslado actual (antes de guardar)
        private List<Mdl_FixedAssetTransferDetail> _detallesTemporales = new List<Mdl_FixedAssetTransferDetail>();

        // ID del activo actualmente seleccionado con Btn_SearchAsset
        private int _selectedAssetId = 0;
        private int? _selectedFromWarehouseId = null;
        private int? _selectedFromEmployeeId = null;

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
            ConfigurarTablaDetalles();
            CargarComboBoxDestino();
            EstadoInicial();
            InicializarScroll();
            ConfigurarEventosScroll();
            ComboBox_Location.SelectedIndexChanged += ComboBox_Location_SelectedIndexChanged;
        }

        #endregion

        #region Configuración

        private void ConfigurarTablaDetalles()
        {
            // Tabla izquierda: activos del traslado actual (temporal o guardado)
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetId", HeaderText = "ID", DataPropertyName = "TransferDetailId", Visible = false });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAssetId", HeaderText = "ASSET_ID", DataPropertyName = "AssetId", Visible = false });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodActivo", HeaderText = "CÓDIGO", DataPropertyName = "AssetCode", Width = 100 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombreActivo", HeaderText = "ACTIVO", DataPropertyName = "AssetName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOrigen", HeaderText = "ORIGEN", DataPropertyName = "FromWarehouseName", Width = 140 });
        }


        private void CargarComboBoxDestino()
        {
            // ComboBox_SelectTo — tipo destino
            ComboBox_SelectTo.Items.Clear();
            ComboBox_SelectTo.Items.Add("BODEGA");
            ComboBox_SelectTo.Items.Add("EMPLEADO");
            ComboBox_SelectTo.SelectedIndex = 0;
            ComboBox_SelectTo.DropDownStyle = ComboBoxStyle.DropDownList;

            // ComboBox_Location — sedes activas
            var sedes = Ctrl_Locations.ObtenerLocationsActivas();
            ComboBox_Location.DataSource = null;
            ComboBox_Location.DataSource = sedes;
            ComboBox_Location.DisplayMember = "Value";
            ComboBox_Location.ValueMember = "Key";
            ComboBox_Location.SelectedIndex = -1;
            ComboBox_Location.DropDownStyle = ComboBoxStyle.DropDownList;

            // ComboBox_ToWarehouse — inicia vacío, se llena al seleccionar sede
            ComboBox_ToWarehouse.DataSource = null;
            ComboBox_ToWarehouse.DisplayMember = "Value";
            ComboBox_ToWarehouse.ValueMember = "Key";
            ComboBox_ToWarehouse.SelectedIndex = -1;
            ComboBox_ToWarehouse.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ComboBox_ToWarehouse.AutoCompleteSource = AutoCompleteSource.ListItems;
            ComboBox_ToWarehouse.DropDownStyle = ComboBoxStyle.DropDown;

            // ComboBox_ToEmployee con AutoComplete
            var empleados = Ctrl_Employees.ObtenerEmpleadosParaCombo();
            ComboBox_ToEmployee.DataSource = null;
            ComboBox_ToEmployee.DataSource = empleados;
            ComboBox_ToEmployee.DisplayMember = "Value";
            ComboBox_ToEmployee.ValueMember = "Key";
            ComboBox_ToEmployee.SelectedIndex = -1;
            ComboBox_ToEmployee.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ComboBox_ToEmployee.AutoCompleteSource = AutoCompleteSource.ListItems;
            ComboBox_ToEmployee.DropDownStyle = ComboBoxStyle.DropDown;

            ComboBox_Location.Enabled = true;
            ComboBox_ToWarehouse.Enabled = false;
            ComboBox_ToEmployee.Enabled = false;
        }

        private void CargarProximoCodigo()
        {
            try
            {
                string codigo = Ctrl_FixedAssetTransfers.ObtenerProximoCodigo();
                Txt_TransferId.Text = codigo;
                Txt_TransferId.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar código de traslado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Txt_TransferId.Text = "TRA-000001";
            }
        }

        private void EstadoInicial()
        {
            _selectedTransferId = 0;
            _selectedAssetId = 0;
            _selectedFromWarehouseId = null;
            _selectedFromEmployeeId = null;
            _modoEdicion = false;
            _detallesTemporales.Clear();

            DTP_TransferDate.Value = DateTime.Today;
            Txt_Asset.Clear();
            Txt_AssetId.Clear();
            Txt_AssetId.Enabled = false;
            Txt_Asset.Enabled = false;
            Txt_FromWarehouse.Clear();
            Txt_FromWarehouse.Enabled = false;
            Txt_FromEmployee.Clear();
            Txt_FromEmployee.Enabled = false;
            Txt_TransferId.Enabled = false;
            Txt_Reason.Clear();

            ComboBox_SelectTo.SelectedIndex = 0;
            ComboBox_ToWarehouse.SelectedIndex = -1;
            ComboBox_ToWarehouse.Text = "";
            ComboBox_ToEmployee.SelectedIndex = -1;
            ComboBox_ToEmployee.Text = "";

            Tabla.DataSource = null;

            Btn_AddAsset.Enabled = true;
            Btn_RemoveAsset.Enabled = false;
            Btn_EndTransfer.Enabled = false;
            Btn_Update.Enabled = false;
            Btn_Clear.Enabled = true;

            CargarProximoCodigo();
        }

        #endregion

        #region Carga de datos

        private void CargarTraslados(
            string transferCode = null,
            int? transferStatusId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            try
            {
                _trasladosList = Ctrl_FixedAssetTransfers.MostrarTraslados(
                    transferCode: transferCode,
                    transferStatusId: transferStatusId,
                    fechaInicio: fechaInicio,
                    fechaFin: fechaFin);

                Tabla.DataSource = null;
                Tabla.DataSource = _trasladosList;

                Lbl_Paginas.Text = $"TOTAL: {_trasladosList.Count} TRASLADO(S)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar traslados: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDetallesDeTabla(int transferId)
        {
            try
            {
                var detalles = Ctrl_FixedAssetTransfers.MostrarDetalles(transferId);
                Tabla.DataSource = null;
                Tabla.DataSource = detalles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar activos del traslado: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefrescarTablaDetallesTemporales()
        {
            Tabla.DataSource = null;
            Tabla.DataSource = new List<Mdl_FixedAssetTransferDetail>(_detallesTemporales);
        }

        #endregion

        #region Selección en tabla de traslados

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = Tabla.Rows[e.RowIndex];

            _selectedAssetId = Convert.ToInt32(row.Cells["colAssetId"].Value);
            Txt_AssetId.Text = row.Cells["colCodActivo"].Value?.ToString();
            Txt_Asset.Text = row.Cells["colNombreActivo"].Value?.ToString();
            Txt_FromWarehouse.Text = row.Cells["colOrigen"].Value?.ToString() ?? "";

            Btn_RemoveAsset.Enabled = true;
        }

        #endregion

        #region Búsqueda de activo

        private void Btn_SearchAsset_Click(object sender, EventArgs e)
        {
            var frm = new Frm_FixedAsset_Movements_SearchAsset
            {
                UserData = UserData
            };
            frm.ShowDialog();

            if (frm.AssetSeleccionado != null)
            {
                var asset = frm.AssetSeleccionado;

                _selectedAssetId = asset.AssetId;
                _selectedFromWarehouseId = asset.CurrentWarehouseId;
                _selectedFromEmployeeId = asset.AssignedToEmployeeId;

                Txt_AssetId.Text = asset.AssetCode;
                Txt_Asset.Text = asset.AssetName;
                Txt_FromWarehouse.Text = !string.IsNullOrEmpty(asset.WarehouseName)
                                         ? asset.WarehouseName : "NO ASIGNADO";
                Txt_FromEmployee.Text = !string.IsNullOrEmpty(asset.EmployeeName)
                                         ? asset.EmployeeName : "NO ASIGNADO";
            }
        }

        #endregion

        #region Cambio de destino

        private void ComboBox_SelectTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool esBodega = ComboBox_SelectTo.SelectedItem?.ToString() == "BODEGA";

            // Sede siempre habilitada — es obligatoria independiente del tipo de destino
            ComboBox_Location.Enabled = true;
            ComboBox_ToWarehouse.Enabled = esBodega && ComboBox_Location.SelectedValue is int;
            ComboBox_ToEmployee.Enabled = !esBodega;

            if (esBodega)
            {
                ComboBox_ToEmployee.SelectedIndex = -1;
                ComboBox_ToEmployee.Text = "";
            }
            else
            {
                ComboBox_ToWarehouse.DataSource = null;
                ComboBox_ToWarehouse.Text = "";
            }
        }

        #endregion

        #region CRUD


        private void Btn_AddAsset_Click(object sender, EventArgs e)
        {
            if (_selectedAssetId == 0)
            {
                MessageBox.Show("DEBE BUSCAR Y SELECCIONAR UN ACTIVO PRIMERO.",
                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_detallesTemporales.Exists(d => d.AssetId == _selectedAssetId))
            {
                MessageBox.Show("ESTE ACTIVO YA FUE AGREGADO AL TRASLADO.",
                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _detallesTemporales.Add(new Mdl_FixedAssetTransferDetail
            {
                AssetId = _selectedAssetId,
                FromWarehouseId = _selectedFromWarehouseId,
                FromEmployeeId = _selectedFromEmployeeId,
                AssetCode = Txt_AssetId.Text,
                AssetName = Txt_Asset.Text,
                FromWarehouseName = Txt_FromWarehouse.Text,
                FromEmployeeName = Txt_FromEmployee.Text
            });

            RefrescarTablaDetallesTemporales();

            // Limpiar selección de activo para el siguiente
            _selectedAssetId = 0;
            _selectedFromWarehouseId = null;
            _selectedFromEmployeeId = null;
            Txt_Asset.Clear();
            Txt_AssetId.Clear();
            Txt_FromWarehouse.Clear();
            Txt_FromEmployee.Clear();

            Btn_RemoveAsset.Enabled = true;
            Btn_EndTransfer.Enabled = true;
        }

        private void Btn_RemoveAsset_Click(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;

            DataGridViewRow row = Tabla.SelectedRows[0];
            int detailId = Convert.ToInt32(row.Cells["colDetId"].Value);
            int assetId = Convert.ToInt32(row.Cells["colAssetId"].Value);

            if (_modoEdicion && detailId > 0)
            {
                DialogResult confirm = MessageBox.Show(
                    "¿DESEA QUITAR ESTE ACTIVO DEL TRASLADO?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes) return;

                int resultado = Ctrl_FixedAssetTransfers.EliminarDetalle(detailId);
                if (resultado == 1)
                    CargarDetallesDeTabla(_selectedTransferId);
                else
                    MessageBox.Show("ERROR AL QUITAR EL ACTIVO.", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Modo nuevo — quitar de la lista temporal
                _detallesTemporales.RemoveAll(d => d.AssetId == assetId);
                RefrescarTablaDetallesTemporales();
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (_selectedTransferId == 0) return;

            bool esBodega = ComboBox_SelectTo.SelectedItem?.ToString() == "BODEGA";
            int? toWarehouseId = null;
            int? toEmployeeId = null;

            if (esBodega)
            {
                if (ComboBox_ToWarehouse.SelectedValue == null || !(ComboBox_ToWarehouse.SelectedValue is int))
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA BODEGA DE DESTINO.", "VALIDACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                toWarehouseId = (int)ComboBox_ToWarehouse.SelectedValue;
            }
            else
            {
                if (ComboBox_ToEmployee.SelectedValue == null || !(ComboBox_ToEmployee.SelectedValue is int))
                {
                    MessageBox.Show("DEBE SELECCIONAR UN EMPLEADO DE DESTINO.", "VALIDACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                toEmployeeId = (int)ComboBox_ToEmployee.SelectedValue;
            }

            var transfer = new Mdl_FixedAssetTransfer
            {
                TransferId = _selectedTransferId,
                TransferCode = Txt_TransferId.Text.Trim().ToUpper(),
                TransferDate = DTP_TransferDate.Value.Date,
                ToWarehouseId = toWarehouseId,
                ToEmployeeId = toEmployeeId,
                TransferStatusId = ObtenerStatusPending(),
                Reason = Txt_Reason.Text.Trim(),
                ModifiedBy = UserData?.UserId
            };

            int resultado = Ctrl_FixedAssetTransfers.ActualizarTraslado(transfer);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("TRASLADO ACTUALIZADO CORRECTAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarTraslados();
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("EL TRASLADO NO EXISTE.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("EL CÓDIGO DE TRASLADO YA ESTÁ EN USO.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -3:
                    MessageBox.Show("DEBE DEFINIR AL MENOS UN DESTINO.", "AVISO",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("ERROR AL ACTUALIZAR EL TRASLADO.", "ERROR",
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
        }

        private void Btn_Transfer_Click(object sender, EventArgs e)
        {
            Frm_FixedAsset_Movements_Tracking frm = new Frm_FixedAsset_Movements_Tracking
            {
                UserData = UserData
            };
            frm.ShowDialog();
        }

        #endregion

        #region Helpers

        private int ObtenerStatusPending()
        {
            try
            {
                var estados = Ctrl_FixedAssetTransferStatus.MostrarEstados(isActive: true);
                var pending = estados.Find(s => s.Order == 1);
                return pending?.TransferStatusId ?? 0;
            }
            catch { return 0; }
        }

        #endregion

        #region ExportarExcel

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (_detallesTemporales == null || _detallesTemporales.Count == 0)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("NO HAY ACTIVOS EN EL TRASLADO ACTUAL PARA EXPORTAR.", "INFORMACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Activos del Traslado",
                    FileName = $"Traslado_{Txt_TransferId.Text}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                var excelApp = new Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                worksheet.Name = "Activos";

                worksheet.Cells[1, 1] = "DETALLE DE TRASLADO DE ACTIVOS - SECRON";
                worksheet.Range["A1:E1"].Merge();
                worksheet.Range["A1:E1"].Font.Size = 16;
                worksheet.Range["A1:E1"].Font.Bold = true;
                worksheet.Range["A1:E1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                worksheet.Range["A1:E1"].Interior.Color =
                    System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                worksheet.Range["A1:E1"].Font.Color =
                    System.Drawing.ColorTranslator.ToOle(Color.White);

                worksheet.Cells[2, 1] = $"CÓDIGO TRASLADO: {Txt_TransferId.Text}";
                worksheet.Cells[3, 1] = $"FECHA: {DTP_TransferDate.Value:dd/MM/yyyy}";
                worksheet.Cells[4, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SECRON"}";
                worksheet.Cells[5, 1] = $"TOTAL ACTIVOS: {_detallesTemporales.Count}";

                int headerRow = 7;
                string[] headers = { "CÓDIGO ACTIVO", "NOMBRE ACTIVO", "BODEGA ORIGEN", "EMPLEADO ORIGEN" };
                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cells[headerRow, i + 1] = headers[i];

                var headerRange = worksheet.Range[$"A{headerRow}:D{headerRow}"];
                headerRange.Font.Bold = true;
                headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                headerRange.Interior.Color =
                    System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                int row = headerRow + 1;
                foreach (var d in _detallesTemporales)
                {
                    worksheet.Cells[row, 1] = d.AssetCode;
                    worksheet.Cells[row, 2] = d.AssetName;
                    worksheet.Cells[row, 3] = d.FromWarehouseName ?? "";
                    worksheet.Cells[row, 4] = d.FromEmployeeName ?? "";

                    if (row % 2 == 0)
                        worksheet.Range[$"A{row}:D{row}"].Interior.Color =
                            System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                    row++;
                }

                var dataRange = worksheet.Range[$"A{headerRow}:D{row - 1}"];
                dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;
                worksheet.Columns.AutoFit();

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
            if (Panel_Izquierdo == null || vScrollBar == null) return;

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
            if (Panel_Izquierdo == null || vScrollBar == null) return;

            Panel_Izquierdo.TabStop = true;
            Panel_Izquierdo.MouseWheel += Panel_Izquierdo_MouseWheel;
            Panel_Izquierdo.MouseEnter += Panel_Izquierdo_MouseEnter;

            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (!(ctrl is ComboBox))
                    ctrl.MouseWheel += Panel_Izquierdo_MouseWheel;
            }
        }

        private void Panel_Izquierdo_MouseEnter(object sender, EventArgs e) => Panel_Izquierdo.Focus();

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e) => MoverContenido(vScrollBar.Value);

        private void Panel_Izquierdo_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!vScrollBar.Visible) return;
            int newValue = vScrollBar.Value - (e.Delta / 120 * 30);
            newValue = Math.Max(0, Math.Min(newValue, vScrollBar.Maximum));
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
                ctrl.Top = int.Parse(parts[1]) - scrollPosition;
            }
            Panel_Izquierdo.Invalidate();
        }

        #endregion

        #region Terminar Traslado
        private void Btn_EndTransfer_Click(object sender, EventArgs e)
        {
            if (_detallesTemporales.Count == 0)
            {
                MessageBox.Show("DEBE AGREGAR AL MENOS UN ACTIVO AL TRASLADO.",
                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool esBodega = ComboBox_SelectTo.SelectedItem?.ToString() == "BODEGA";
            int? toWarehouseId = null;
            int? toEmployeeId = null;

            if (esBodega)
            {
                if (ComboBox_ToWarehouse.SelectedValue == null || !(ComboBox_ToWarehouse.SelectedValue is int))
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA BODEGA DE DESTINO.",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                toWarehouseId = (int)ComboBox_ToWarehouse.SelectedValue;
            }
            else
            {
                if (ComboBox_ToEmployee.SelectedValue == null || !(ComboBox_ToEmployee.SelectedValue is int))
                {
                    MessageBox.Show("DEBE SELECCIONAR UN EMPLEADO DE DESTINO.",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                toEmployeeId = (int)ComboBox_ToEmployee.SelectedValue;
            }

            int pendingStatusId = ObtenerStatusPending();
            if (pendingStatusId == 0)
            {
                MessageBox.Show("NO SE PUDO OBTENER EL ESTADO PENDIENTE. VERIFIQUE LA CONFIGURACIÓN.",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var transfer = new Mdl_FixedAssetTransfer
            {
                TransferCode = Txt_TransferId.Text.Trim().ToUpper(),
                TransferDate = DTP_TransferDate.Value.Date,
                ToWarehouseId = toWarehouseId,
                ToEmployeeId = toEmployeeId,
                TransferStatusId = pendingStatusId,
                Reason = Txt_Reason.Text.Trim(),
                CreatedBy = UserData?.UserId
            };

            int transferId = Ctrl_FixedAssetTransfers.RegistrarTraslado(transfer);

            if (transferId <= 0)
            {
                switch (transferId)
                {
                    case -1:
                        MessageBox.Show("EL CÓDIGO DE TRASLADO YA EXISTE.", "AVISO",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case -2:
                        MessageBox.Show("DEBE DEFINIR UN DESTINO (BODEGA O EMPLEADO).", "AVISO",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("ERROR AL REGISTRAR EL TRASLADO.", "ERROR",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
                return;
            }

            bool hayErrores = false;
            foreach (var detalle in _detallesTemporales)
            {
                detalle.TransferId = transferId;
                detalle.CreatedBy = UserData?.UserId;

                int resultado = Ctrl_FixedAssetTransfers.AgregarDetalle(detalle);
                if (resultado <= 0)
                {
                    hayErrores = true;
                    MessageBox.Show($"ERROR AL AGREGAR EL ACTIVO '{detalle.AssetCode}' AL TRASLADO.",
                        "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            if (hayErrores)
                MessageBox.Show("TRASLADO GUARDADO CON ALGUNOS ERRORES EN LOS DETALLES.", "AVISO",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("TRASLADO REGISTRADO CORRECTAMENTE.", "ÉXITO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            EstadoInicial();
        }

        #endregion

        #region Eventos de Sede
        private void ComboBox_Location_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBox_Location.SelectedValue == null || !(ComboBox_Location.SelectedValue is int))
            {
                ComboBox_ToWarehouse.DataSource = null;
                ComboBox_ToWarehouse.Enabled = false;
                return;
            }

            int locationId = (int)ComboBox_Location.SelectedValue;
            var bodegas = Ctrl_Warehouses.ObtenerBodegasPorLocation(locationId);

            ComboBox_ToWarehouse.DataSource = null;
            ComboBox_ToWarehouse.DataSource = bodegas;
            ComboBox_ToWarehouse.DisplayMember = "Value";
            ComboBox_ToWarehouse.ValueMember = "Key";
            ComboBox_ToWarehouse.SelectedIndex = -1;
            ComboBox_ToWarehouse.Text = "";

            bool esBodega = ComboBox_SelectTo.SelectedItem?.ToString() == "BODEGA";
            ComboBox_ToWarehouse.Enabled = esBodega;
        }
        #endregion
    }
}