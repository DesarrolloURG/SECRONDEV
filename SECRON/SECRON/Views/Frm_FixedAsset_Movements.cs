using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing.Printing;
using System.Net;
using System.Net.Mail;

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

        // Detalles para generar carta de responsabilidad
        private PrintDocument _printDocument;
        private PrintPreviewDialog _printPreviewDialog;
        private DatosTraslado _datosTraslado;

        public class DatosTraslado
        {
            public string CodigoTraslado { get; set; }
            public string Fecha { get; set; }
            public string SedeDestino { get; set; }
            public string BodegaDestino { get; set; }
            public string EmpleadoDestino { get; set; }
            public string Motivo { get; set; }
            public string EmitidoPor { get; set; }
            public List<Mdl_FixedAssetTransferDetail> Detalles { get; set; }
        }

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

        #region ConfigImpresion
        private void InicializarImpresion()
        {
            _datosTraslado = new DatosTraslado();

            _printDocument = new PrintDocument();
            _printDocument.PrintPage += PrintDocument_PrintPage;
            _printDocument.DefaultPageSettings.PaperSize = new PaperSize("Letter", 850, 1100);
            _printDocument.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);

            _printPreviewDialog = new PrintPreviewDialog();
            _printPreviewDialog.Document = _printDocument;
            _printPreviewDialog.UseAntiAlias = true;
            _printPreviewDialog.ShowIcon = false;
            _printPreviewDialog.WindowState = FormWindowState.Maximized;
            _printPreviewDialog.Load += PrintPreviewDialog_Load;
        }

        private void PrintPreviewDialog_Load(object sender, EventArgs e)
        {
            PrintPreviewDialog dialog = sender as PrintPreviewDialog;
            if (dialog == null) return;

            foreach (Control control in dialog.Controls)
            {
                if (control is ToolStrip toolStrip)
                {
                    foreach (ToolStripItem item in toolStrip.Items)
                    {
                        if (item is ToolStripButton button &&
                            (button.Text.Contains("Imprimir") || button.Text.Contains("Print")))
                        {
                            var customPrintButton = new ToolStripButton
                            {
                                Image = button.Image,
                                Text = button.Text,
                                ToolTipText = button.ToolTipText,
                                DisplayStyle = button.DisplayStyle
                            };
                            customPrintButton.Click += (s, ev) => ImprimirConDialogo();

                            int index = toolStrip.Items.IndexOf(button);
                            toolStrip.Items.RemoveAt(index);
                            toolStrip.Items.Insert(index, customPrintButton);
                            break;
                        }
                    }
                }
            }
        }

        private void ImprimirConDialogo()
        {
            try
            {
                PrintDialog printDialog = new PrintDialog
                {
                    Document = _printDocument,
                    UseEXDialog = true,
                    AllowPrintToFile = true
                };

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    _printPreviewDialog.Hide();
                    _printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL IMPRIMIR: {ex.Message}", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush brush = Brushes.Black;
            Pen pen = new Pen(Color.Black, 1);
            Pen penGris = new Pen(Color.Gray, 0.5f);

            Font fontTitulo = new Font("Calibri", 13, FontStyle.Bold);
            Font fontBold = new Font("Calibri", 10, FontStyle.Bold);
            Font fontNormal = new Font("Calibri", 10);
            Font fontSmall = new Font("Calibri", 9);
            Font fontSmallBold = new Font("Calibri", 9, FontStyle.Bold);
            Font fontTable = new Font("Calibri", 8);
            Font fontTableBold = new Font("Calibri", 8, FontStyle.Bold);

            int leftMargin = 50;
            int rightMargin = 760;
            int pageWidth = rightMargin - leftMargin;
            int currentY = 20;

            // ===== ENCABEZADO — Logo =====
            try
            {
                Image logoEncabezado = Properties.Resources.LogoMembretadoEncabezado;
                int logoW = 160;
                int logoH = 60;
                g.DrawImage(logoEncabezado, rightMargin - logoW, currentY, logoW, logoH);
            }
            catch { }

            // ===== REFERENCIA Y FECHA (arriba derecha) =====
            string refTexto = $"Ref. {_datosTraslado.CodigoTraslado}";
            g.DrawString(refTexto, fontSmallBold, brush, rightMargin - 200, currentY + 65);
            g.DrawString($"Guatemala, {_datosTraslado.Fecha}", fontSmall, brush, rightMargin - 200, currentY + 80);

            currentY += 100;

            // ===== TÍTULO =====
            string titulo = "NOTA DE TRASLADO DE ACTIVOS FIJOS";
            SizeF tituloSize = g.MeasureString(titulo, fontTitulo);
            float tituloX = leftMargin + (pageWidth - tituloSize.Width) / 2;
            g.DrawString(titulo, fontTitulo, brush, tituloX, currentY);
            currentY += 30;

            // Línea separadora
            g.DrawLine(new Pen(Color.FromArgb(230, 115, 40), 2), leftMargin, currentY, rightMargin, currentY);
            currentY += 15;

            // ===== DATOS DEL TRASLADO =====
            g.DrawString("A:", fontBold, brush, leftMargin, currentY);
            g.DrawString(_datosTraslado.SedeDestino?.ToUpper() ?? "", fontNormal, brush, leftMargin + 80, currentY);
            currentY += 20;

            g.DrawString("BODEGA:", fontBold, brush, leftMargin, currentY);
            g.DrawString(_datosTraslado.BodegaDestino?.ToUpper() ?? "NO ESPECIFICADA", fontNormal, brush, leftMargin + 80, currentY);
            currentY += 20;

            if (!string.IsNullOrEmpty(_datosTraslado.EmpleadoDestino))
            {
                g.DrawString("ASIGNADO A:", fontBold, brush, leftMargin, currentY);
                g.DrawString(_datosTraslado.EmpleadoDestino.ToUpper(), fontNormal, brush, leftMargin + 80, currentY);
                currentY += 20;
            }

            currentY += 5;
            g.DrawString("De:", fontBold, brush, leftMargin, currentY);
            g.DrawString(_datosTraslado.EmitidoPor?.ToUpper() ?? "", fontNormal, brush, leftMargin + 80, currentY);
            currentY += 20;

            g.DrawString("Coordinador General, Región 2", fontNormal, brush, leftMargin + 80, currentY);
            currentY += 15;
            g.DrawString("Universidad Regional de Guatemala", fontNormal, brush, leftMargin + 80, currentY);
            currentY += 25;

            g.DrawString("Asunto:", fontBold, brush, leftMargin, currentY);
            RectangleF rectMotivo = new RectangleF(leftMargin + 80, currentY, pageWidth - 80, 40);
            g.DrawString(_datosTraslado.Motivo?.ToUpper() ?? "", fontNormal, brush, rectMotivo);
            currentY += 50;

            // Línea separadora
            g.DrawLine(penGris, leftMargin, currentY, rightMargin, currentY);
            currentY += 15;

            // ===== TABLA DE ACTIVOS =====
            int[] colWidths = { 90, 180, 160, 160 };
            string[] headers = { "CÓDIGO", "NOMBRE DEL ACTIVO", "UBICACIÓN ORIGEN", "DESTINO" };

            int tableX = leftMargin;
            int rowH = 20;

            // Header de tabla
            g.FillRectangle(new SolidBrush(Color.FromArgb(30, 80, 160)), tableX, currentY, pageWidth, rowH);
            int colX = tableX;
            foreach (var h in headers)
            {
                g.DrawString(h, fontTableBold, Brushes.White, colX + 3, currentY + 4);
                colX += colWidths[System.Array.IndexOf(headers, h)];
            }
            g.DrawRectangle(pen, tableX, currentY, pageWidth, rowH);
            currentY += rowH;

            // Filas de activos
            bool altRow = false;
            foreach (var detalle in _datosTraslado.Detalles)
            {
                if (altRow)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 248)), tableX, currentY, pageWidth, rowH);

                colX = tableX;
                g.DrawString(detalle.AssetCode ?? "", fontTable, brush, colX + 3, currentY + 4);
                colX += colWidths[0];
                g.DrawString(detalle.AssetName ?? "", fontTable, brush, colX + 3, currentY + 4);
                colX += colWidths[1];
                string origen = !string.IsNullOrEmpty(detalle.FromWarehouseName)
                    ? detalle.FromWarehouseName
                    : detalle.FromEmployeeName ?? "";
                g.DrawString(origen, fontTable, brush, colX + 3, currentY + 4);
                colX += colWidths[2];
                string destino = !string.IsNullOrEmpty(_datosTraslado.BodegaDestino)
                    ? _datosTraslado.BodegaDestino
                    : _datosTraslado.EmpleadoDestino ?? "";
                g.DrawString(destino, fontTable, brush, colX + 3, currentY + 4);

                g.DrawRectangle(penGris, tableX, currentY, pageWidth, rowH);
                currentY += rowH;
                altRow = !altRow;
            }

            // Borde exterior tabla
            int tableStartY = currentY - (rowH * _datosTraslado.Detalles.Count) - rowH;
            g.DrawRectangle(pen, tableX, tableStartY, pageWidth, rowH * (_datosTraslado.Detalles.Count + 1));

            currentY += 20;

            // ===== TOTAL DE ACTIVOS =====
            g.DrawString($"TOTAL DE ACTIVOS TRASLADADOS: {_datosTraslado.Detalles.Count}",
                fontBold, brush, leftMargin, currentY);
            currentY += 30;

            // ===== FIRMA =====
            g.DrawLine(pen, leftMargin, currentY + 30, leftMargin + 200, currentY + 30);
            g.DrawString("FIRMA Y SELLO", fontSmall, brush, leftMargin + 50, currentY + 35);
            g.DrawString(_datosTraslado.EmitidoPor?.ToUpper() ?? "", fontSmallBold, brush, leftMargin, currentY + 50);
            g.DrawString("COORDINADOR GENERAL, REGIÓN 2", fontSmall, brush, leftMargin, currentY + 65);

            currentY = e.PageBounds.Height - 100;

            // ===== PIE DE PÁGINA =====
            try
            {
                Image piePagina = Properties.Resources.MembretadoPiePagina;
                g.DrawImage(piePagina, leftMargin - 10, currentY, pageWidth + 20, 70);
            }
            catch { }

            // Liberar recursos
            fontTitulo.Dispose();
            fontBold.Dispose();
            fontNormal.Dispose();
            fontSmall.Dispose();
            fontSmallBold.Dispose();
            fontTable.Dispose();
            fontTableBold.Dispose();
            pen.Dispose();
            penGris.Dispose();
        }

        private void EnviarCorreoTraslado(int transferId, DatosTraslado datos)
        {
            try
            {
                string correoEmisor = "notificaciones@uregionalregion2.edu.gt";
                string contraseña = "F0rza01.";

                SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(correoEmisor, contraseña),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(correoEmisor, "Notificaciones URegional"),
                    Subject = $"TRASLADO INICIADO - {datos.CodigoTraslado}",
                    IsBodyHtml = true
                };

                string detallesHtml = "";
                foreach (var d in datos.Detalles)
                {
                    string origen = !string.IsNullOrEmpty(d.FromWarehouseName)
                        ? d.FromWarehouseName : d.FromEmployeeName ?? "";
                    detallesHtml += $"<tr><td>{d.AssetCode}</td><td>{d.AssetName}</td><td>{origen}</td></tr>";
                }

                mail.Body = $@"
        <html>
        <body style='font-family: Arial, sans-serif; color: #333;'>
            <h2 style='color: #1E50A0;'>PROCESO DE TRASLADO INICIADO</h2>
            <p><strong>Código de Traslado:</strong> {datos.CodigoTraslado}</p>
            <p><strong>Fecha:</strong> {datos.Fecha}</p>
            <p><strong>Sede Destino:</strong> {datos.SedeDestino}</p>
            <p><strong>Bodega Destino:</strong> {datos.BodegaDestino}</p>
            <p><strong>Empleado Destino:</strong> {(string.IsNullOrEmpty(datos.EmpleadoDestino) ? "NO ASIGNADO" : datos.EmpleadoDestino)}</p>
            <p><strong>Motivo:</strong> {datos.Motivo}</p>
            <p><strong>Emitido por:</strong> {datos.EmitidoPor}</p>
            <br/>
            <table border='1' cellpadding='5' cellspacing='0' style='border-collapse:collapse; width:100%;'>
                <tr style='background-color:#1E50A0; color:white;'>
                    <th>CÓDIGO ACTIVO</th><th>NOMBRE</th><th>UBICACIÓN ORIGEN</th>
                </tr>
                {detallesHtml}
            </table>
            <br/>
            <p style='color:#555;'>Servicio automático de notificaciones, <strong>SECRON</strong></p>
        </body>
        </html>";

                // Enviar al correo del usuario que creó el traslado
                if (!string.IsNullOrEmpty(UserData?.InstitutionalEmail))
                    mail.To.Add(UserData.InstitutionalEmail);
                else
                    mail.To.Add(correoEmisor);

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ENVIAR CORREO: {ex.Message}", "ADVERTENCIA",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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

            // Validar que ningún activo esté en estado TRASLADADO
            foreach (var detalle in _detallesTemporales)
            {
                var activo = Ctrl_FixedAssets.ObtenerActivoPorId(detalle.AssetId);
                if (activo != null && activo.AssetStatus?.ToUpper() == "TRASLADADO")
                {
                    MessageBox.Show(
                        $"EL ACTIVO '{detalle.AssetCode} - {detalle.AssetName}' YA SE ENCUENTRA EN ESTADO TRASLADADO Y NO PUEDE INCLUIRSE EN UN NUEVO TRASLADO.",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            bool esBodega = ComboBox_SelectTo.SelectedItem?.ToString() == "BODEGA";
            int? toWarehouseId = null;
            int? toEmployeeId = null;
            string bodegaDestino = "";
            string empleadoDestino = "";
            string sedeDestino = ComboBox_Location.Text;

            if (esBodega)
            {
                if (ComboBox_ToWarehouse.SelectedValue == null || !(ComboBox_ToWarehouse.SelectedValue is int))
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA BODEGA DE DESTINO.",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                toWarehouseId = (int)ComboBox_ToWarehouse.SelectedValue;
                bodegaDestino = ComboBox_ToWarehouse.Text;
            }
            else
            {
                if (ComboBox_ToEmployee.SelectedValue is int empId)
                {
                    toEmployeeId = empId;
                    empleadoDestino = ComboBox_ToEmployee.Text;
                }
            }

            int pendingStatusId = ObtenerStatusPending();
            if (pendingStatusId == 0)
            {
                MessageBox.Show("NO SE PUDO OBTENER EL ESTADO PENDIENTE.",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Preparar datos para impresión
            _datosTraslado = new DatosTraslado
            {
                CodigoTraslado = Txt_TransferId.Text.Trim().ToUpper(),
                Fecha = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy",
                                    new System.Globalization.CultureInfo("es-GT")).ToUpper(),
                SedeDestino = sedeDestino,
                BodegaDestino = bodegaDestino,
                EmpleadoDestino = empleadoDestino,
                Motivo = Txt_Reason.Text.Trim(),
                EmitidoPor = UserData?.FullName ?? "",
                Detalles = new List<Mdl_FixedAssetTransferDetail>(_detallesTemporales)
            };

            // Mostrar vista previa
            InicializarImpresion();

            _printDocument = new PrintDocument();
            _printDocument.PrintPage += PrintDocument_PrintPage;
            _printDocument.DefaultPageSettings.PaperSize = new PaperSize("Letter", 850, 1100);
            _printDocument.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
            _printPreviewDialog.Document = _printDocument;

            _printPreviewDialog.ShowDialog(this);

            // Confirmación para guardar
            var confirmacion = MessageBox.Show(
                "¿DESEA FINALIZAR Y GUARDAR EL TRASLADO EN EL SISTEMA?",
                "CONFIRMAR FINALIZACIÓN",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.No) return;

            // Guardar maestro
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
                        MessageBox.Show("DEBE DEFINIR UN DESTINO.", "AVISO",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("ERROR AL REGISTRAR EL TRASLADO.", "ERROR",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
                return;
            }

            // Guardar detalles y cambiar estado de activos a TRASLADADO
            bool hayErrores = false;
            foreach (var detalle in _detallesTemporales)
            {
                detalle.TransferId = transferId;
                detalle.CreatedBy = UserData?.UserId;

                int resDetalle = Ctrl_FixedAssetTransfers.AgregarDetalle(detalle);
                if (resDetalle <= 0)
                {
                    hayErrores = true;
                    MessageBox.Show($"ERROR AL AGREGAR EL ACTIVO '{detalle.AssetCode}'.",
                        "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                // Cambiar estado del activo a TRASLADADO
                Ctrl_FixedAssets.ActualizarEstadoActivo(detalle.AssetId, "TRASLADADO", UserData?.UserId);
            }

            // Enviar correo de notificación
            EnviarCorreoTraslado(transferId, _datosTraslado);

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