using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_ITSM_Technology_ResponsabilityLetter : Form
    {
        #region PropiedadesIniciales

        public Mdl_Security_UserInfo UserData { get; set; }

        private int _selectedAssetId = 0;
        private int _selectedEmployeeId = 0;

        // Guarda las selecciones de atributos por assetId
        // Key: assetId, Value: lista de (etiqueta, valor, imprimirCheck)
        private Dictionary<int, List<(string Etiqueta, string Valor, bool Imprimir)>>
            _seleccionesAtributos = new Dictionary<int, List<(string, string, bool)>>();

        // Estado de impresión
        private int _filaImpresionActual = 0;
        private int _activoImpresionActual = 0;
        private bool _encabezadoGlobalImpreso = false;

        // Datos para impresión — lista de activos con sus filas seleccionadas
        private List<(Mdl_FixedAsset Activo, List<(string Etiqueta, string Valor)> Filas)>
            _datosImpresion;

        #endregion

        #region Constructor

        public Frm_ITSM_Technology_ResponsabilityLetter()
        {
            InitializeComponent();
        }

        #endregion

        #region Load

        private void Frm_ITSM_Technology_ResponsabilityLetter_Load(object sender, EventArgs e)
        {
            ConfigurarTamañoFormulario();
            ConfigurarComboEmpleado();
            ConfigurarTabla();
            ConfigurarTablaDetalle();
            EstadoInicial();
        }

        #endregion

        #region TamañoFormulario

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(1184, 861);
            this.MinimumSize = new Size(1184, 861);
            this.MaximumSize = new Size(1184, 861);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        #endregion

        #region Configuración

        private void ConfigurarComboEmpleado()
        {
            ComboBox_Employee.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Employee.Items.Clear();
            ComboBox_Employee.Items.Add(
                new KeyValuePair<int, string>(0, "— SELECCIONE UN EMPLEADO —"));

            var empleados = Ctrl_Employees.ObtenerEmpleadosParaCombo();
            foreach (var emp in empleados)
                ComboBox_Employee.Items.Add(emp);

            ComboBox_Employee.DisplayMember = "Value";
            ComboBox_Employee.SelectedIndex = 0;
            ComboBox_Employee.SelectedIndexChanged += ComboBox_Empleado_SelectedIndexChanged;
        }

        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = true;
            Tabla.ReadOnly = false;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            Tabla.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colImprimir",
                HeaderText = "IMPRIMIR",
                Width = 80,
                TrueValue = true,
                FalseValue = false
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                Visible = false
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCodigo",
                HeaderText = "CÓDIGO",
                Width = 120,
                ReadOnly = true
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                HeaderText = "NOMBRE DEL EQUIPO",
                Width = 280,
                ReadOnly = true
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCategoria",
                HeaderText = "CATEGORÍA",
                Width = 150,
                ReadOnly = true
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEstado",
                HeaderText = "ESTADO",
                Width = 100,
                ReadOnly = true
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colBodega",
                HeaderText = "BODEGA",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCategoryId",
                Visible = false
            });

            Tabla.CellClick += Tabla_CellClick;
        }

        private void ConfigurarTablaDetalle()
        {
            TablaDetalle.Columns.Clear();
            TablaDetalle.AutoGenerateColumns = false;
            TablaDetalle.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TablaDetalle.MultiSelect = true;
            TablaDetalle.ReadOnly = false;
            TablaDetalle.AllowUserToAddRows = false;
            TablaDetalle.RowHeadersVisible = false;

            TablaDetalle.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colImprimir",
                HeaderText = "IMPRIMIR",
                Width = 80,
                TrueValue = true,
                FalseValue = false
            });
            TablaDetalle.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEtiqueta",
                HeaderText = "ESPECIFICACIÓN",
                Width = 250,
                ReadOnly = true
            });
            TablaDetalle.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colValor",
                HeaderText = "DETALLE",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            });

            // Guardar selecciones al cambiar checkbox
            TablaDetalle.CellValueChanged += TablaDetalle_CellValueChanged;
            TablaDetalle.CurrentCellDirtyStateChanged += TablaDetalle_DirtyStateChanged;
        }

        private void EstadoInicial()
        {
            _selectedAssetId = 0;
            _selectedEmployeeId = 0;
            _seleccionesAtributos.Clear();
            Tabla.Rows.Clear();
            TablaDetalle.Rows.Clear();
            Btn_Print.Enabled = false;
            Lbl_Info.Text = "SELECCIONE UN EMPLEADO PARA VER SUS EQUIPOS ASIGNADOS.";
        }

        #endregion

        #region EventosCombos

        private void ComboBox_Empleado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBox_Employee.SelectedItem is KeyValuePair<int, string> kvp && kvp.Key > 0)
            {
                _selectedEmployeeId = kvp.Key;
                _seleccionesAtributos.Clear();
                CargarActivosPorEmpleado(_selectedEmployeeId);
            }
            else
            {
                EstadoInicial();
            }
        }

        #endregion

        #region CargaDeDatos

        private void CargarActivosPorEmpleado(int employeeId)
        {
            try
            {
                Tabla.Rows.Clear();
                TablaDetalle.Rows.Clear();

                var activos = Ctrl_FixedAssets.BuscarActivos(
                    filtroEstado: "SOLO ACTIVOS",
                    employeeId: employeeId);

                if (activos == null || activos.Count == 0)
                {
                    Lbl_Info.Text = "ESTE EMPLEADO NO TIENE ACTIVOS ASIGNADOS.";
                    Btn_Print.Enabled = false;
                    return;
                }

                foreach (var activo in activos)
                {
                    int rowIndex = Tabla.Rows.Add();
                    Tabla.Rows[rowIndex].Cells["colImprimir"].Value = true;
                    Tabla.Rows[rowIndex].Cells["colId"].Value = activo.AssetId;
                    Tabla.Rows[rowIndex].Cells["colCodigo"].Value = activo.AssetCode;
                    Tabla.Rows[rowIndex].Cells["colNombre"].Value = activo.AssetName;
                    Tabla.Rows[rowIndex].Cells["colCategoria"].Value = activo.CategoryName;
                    Tabla.Rows[rowIndex].Cells["colEstado"].Value = activo.AssetStatus;
                    Tabla.Rows[rowIndex].Cells["colBodega"].Value = activo.WarehouseName ?? "";
                    Tabla.Rows[rowIndex].Cells["colCategoryId"].Value = activo.AssetCategoryId;
                }

                Lbl_Info.Text = $"TOTAL: {activos.Count} EQUIPO(S) ASIGNADOS.";
                Btn_Print.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar activos: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarAtributosDeActivo(int assetId, int categoryId)
        {
            try
            {
                TablaDetalle.CellValueChanged -= TablaDetalle_CellValueChanged;

                TablaDetalle.Rows.Clear();

                var atributos = Ctrl_FixedAssetAttributeValues
                    .ObtenerValoresConPlantilla(assetId, categoryId);

                if (atributos == null || atributos.Count == 0)
                {
                    TablaDetalle.CellValueChanged += TablaDetalle_CellValueChanged;
                    return;
                }

                // Si ya hay selecciones guardadas para este activo, usarlas
                bool haySeleccionGuardada = _seleccionesAtributos.ContainsKey(assetId);

                foreach (var attr in atributos.OrderBy(a => a.AttributeDefId))
                {
                    bool imprimir;
                    if (haySeleccionGuardada)
                    {
                        var guardado = _seleccionesAtributos[assetId]
                            .FirstOrDefault(s => s.Etiqueta == attr.AttributeLabel.ToUpper());
                        imprimir = guardado.Etiqueta != null
                            ? guardado.Imprimir
                            : !string.IsNullOrWhiteSpace(attr.Value);
                    }
                    else
                    {
                        imprimir = !string.IsNullOrWhiteSpace(attr.Value);
                    }

                    int rowIndex = TablaDetalle.Rows.Add();
                    TablaDetalle.Rows[rowIndex].Cells["colImprimir"].Value = imprimir;
                    TablaDetalle.Rows[rowIndex].Cells["colEtiqueta"].Value =
                        attr.AttributeLabel.ToUpper();
                    TablaDetalle.Rows[rowIndex].Cells["colValor"].Value =
                        attr.Value ?? "— SIN VALOR —";
                }

                TablaDetalle.CellValueChanged += TablaDetalle_CellValueChanged;
            }
            catch (Exception ex)
            {
                TablaDetalle.CellValueChanged += TablaDetalle_CellValueChanged;
                MessageBox.Show($"Error al cargar atributos: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region SelecciónEnTabla

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Guardar selecciones actuales antes de cambiar
            if (_selectedAssetId > 0)
                GuardarSeleccionesActuales();

            DataGridViewRow row = Tabla.Rows[e.RowIndex];
            _selectedAssetId = Convert.ToInt32(row.Cells["colId"].Value);
            int categoryId = Convert.ToInt32(row.Cells["colCategoryId"].Value);

            // Solo cargar atributos si NO es clic en el checkbox
            if (e.ColumnIndex != Tabla.Columns["colImprimir"].Index)
                CargarAtributosDeActivo(_selectedAssetId, categoryId);
        }

        private void GuardarSeleccionesActuales()
        {
            if (_selectedAssetId == 0 || TablaDetalle.Rows.Count == 0) return;

            var selecciones = new List<(string, string, bool)>();
            foreach (DataGridViewRow row in TablaDetalle.Rows)
            {
                string etiqueta = row.Cells["colEtiqueta"].Value?.ToString() ?? "";
                string valor = row.Cells["colValor"].Value?.ToString() ?? "";
                bool impr = row.Cells["colImprimir"].Value is bool b && b;
                selecciones.Add((etiqueta, valor, impr));
            }
            _seleccionesAtributos[_selectedAssetId] = selecciones;
        }

        #endregion

        #region EventosTablaDetalle

        private void TablaDetalle_DirtyStateChanged(object sender, EventArgs e)
        {
            if (TablaDetalle.IsCurrentCellDirty)
                TablaDetalle.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void TablaDetalle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            // Guardar inmediatamente al cambiar cualquier checkbox
            if (_selectedAssetId > 0)
                GuardarSeleccionesActuales();
        }

        #endregion

        #region Impresion

        private void Btn_Print_Click(object sender, EventArgs e)
        {
            // Guardar selecciones del activo actual antes de imprimir
            if (_selectedAssetId > 0)
                GuardarSeleccionesActuales();

            bool hayActivos = false;
            foreach (DataGridViewRow row in Tabla.Rows)
                if (row.Cells["colImprimir"].Value is bool v && v) { hayActivos = true; break; }

            if (!hayActivos)
            {
                MessageBox.Show("Debe seleccionar al menos un equipo para imprimir.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Construir datos de impresión — todos los activos en una sola carta
            _datosImpresion = new List<(Mdl_FixedAsset, List<(string, string)>)>();

            string nombreEmpleado = "";

            foreach (DataGridViewRow row in Tabla.Rows)
            {
                if (!(row.Cells["colImprimir"].Value is bool imprimir) || !imprimir) continue;

                int assetId = Convert.ToInt32(row.Cells["colId"].Value);
                int categoryId = Convert.ToInt32(row.Cells["colCategoryId"].Value);

                var activo = Ctrl_FixedAssets.ObtenerActivoPorId(assetId);
                if (activo == null) continue;

                if (string.IsNullOrEmpty(nombreEmpleado))
                    nombreEmpleado = activo.EmployeeName ?? "";

                var filas = new List<(string, string)>();

                // Usar selecciones guardadas si existen
                if (_seleccionesAtributos.ContainsKey(assetId))
                {
                    foreach (var sel in _seleccionesAtributos[assetId])
                        if (sel.Imprimir && sel.Valor != "— SIN VALOR —")
                            filas.Add((sel.Etiqueta, sel.Valor));
                }
                else
                {
                    // Sin selección previa — incluir todos con valor
                    var atributos = Ctrl_FixedAssetAttributeValues
                        .ObtenerValoresConPlantilla(assetId, categoryId);
                    if (atributos != null)
                        foreach (var attr in atributos
                            .Where(a => !string.IsNullOrWhiteSpace(a.Value))
                            .OrderBy(a => a.AttributeDefId))
                            filas.Add((attr.AttributeLabel.ToUpper(), attr.Value));
                }

                _datosImpresion.Add((activo, filas));
            }

            if (_datosImpresion.Count == 0)
            {
                MessageBox.Show("No hay equipos válidos para imprimir.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Resetear estado de impresión
            _filaImpresionActual = 0;
            _activoImpresionActual = 0;
            _encabezadoGlobalImpreso = false;

            var printDoc = new PrintDocument();
            printDoc.DocumentName = $"Carta_Responsabilidad_{DateTime.Now:yyyyMMdd_HHmmss}";
            printDoc.DefaultPageSettings.PaperSize = new PaperSize("Letter", 850, 1100);
            printDoc.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
            printDoc.PrintPage += PrintDoc_PrintPage;

            var preview = new PrintPreviewDialog
            {
                Document = printDoc,
                WindowState = FormWindowState.Maximized,
                Text = "CARTA DE RESPONSABILIDAD — VISTA PREVIA"
            };
            preview.ShowDialog(this);
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            ImprimirCarta(e);
        }

        private void ImprimirCarta(PrintPageEventArgs e)
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
            int pageHeight = e.PageBounds.Height;
            int pieY = pageHeight - 130;
            float valorX = leftMargin + 200;
            int tableX = leftMargin;
            int rowH = 20;
            int[] colWidths = { 220, pageWidth - 220 };

            int firmaLineaY = pieY - 75;
            int selloInicio = firmaLineaY - 200;
            int limiteUltima = selloInicio - 20;
            int limiteNormal = pieY - 20;

            // Verificar si este es el último activo y la última página
            bool EsUltimaSeccion(int yActual)
            {
                if (_activoImpresionActual < _datosImpresion.Count - 1) return false;
                var filasActuales = _datosImpresion[_activoImpresionActual].Filas;
                int filasRestantes = filasActuales.Count - _filaImpresionActual;
                int alturaRestante = (filasRestantes * rowH) + rowH + 65 + 30;
                return (yActual + alturaRestante) <= limiteUltima;
            }

            void DibujarPie()
            {
                try
                {
                    Image pie = Properties.Resources.MembretadoPiePagina;
                    g.DrawImage(pie, leftMargin - 10, pieY, pageWidth + 20, 110);
                }
                catch { }
            }

            void DibujarLogo()
            {
                try
                {
                    Image logo = Properties.Resources.LogoMembretadoEncabezado;
                    int logoW = 240, logoH = 80;
                    g.DrawImage(logo, leftMargin + (pageWidth - logoW) / 2, 35, logoW, logoH);
                }
                catch { }
            }

            void LiberarRecursos()
            {
                fontTitulo.Dispose(); fontBold.Dispose(); fontNormal.Dispose();
                fontSmall.Dispose(); fontSmallBold.Dispose();
                fontTable.Dispose(); fontTableBold.Dispose();
                pen.Dispose(); penGris.Dispose();
            }

            int currentY = 20;
            var primerActivo = _datosImpresion[0].Activo;

            // ── Encabezado global — solo una vez ──────────────────────
            if (!_encabezadoGlobalImpreso)
            {
                DibujarLogo();
                currentY += 50;

                string fecha = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy",
                    new System.Globalization.CultureInfo("es-GT")).ToUpper();
                g.DrawString($"Guatemala, {fecha}", fontSmall, brush,
                    rightMargin - 200, currentY + 80);
                currentY += 105;

                string titulo = "CARTA DE RESPONSABILIDAD DE EQUIPOS DE TECNOLOGÍA";
                SizeF tSize = g.MeasureString(titulo, fontTitulo);
                g.DrawString(titulo, fontTitulo, brush,
                    leftMargin + (pageWidth - tSize.Width) / 2, currentY);
                currentY += 28;
                g.DrawLine(new Pen(Color.FromArgb(230, 115, 40), 2),
                    leftMargin, currentY, rightMargin, currentY);
                currentY += 18;

                g.DrawString("ASIGNADO A:", fontBold, brush, leftMargin, currentY);
                g.DrawString(primerActivo.EmployeeName?.ToUpper() ?? "___________________________",
                    fontNormal, brush, valorX, currentY);
                currentY += 27;

                g.DrawString("DE:", fontBold, brush, leftMargin, currentY);
                g.DrawString(UserData?.FullName?.ToUpper() ?? "", fontNormal, brush, valorX, currentY);
                currentY += 18;
                g.DrawString("UNIVERSIDAD REGIONAL DE GUATEMALA", fontNormal, brush, valorX, currentY);
                currentY += 35;

                string nombreResp = primerActivo.EmployeeName?.ToUpper() ?? "___________________________";
                string cuerpo =
                    $"Por medio de la presente, yo, {nombreResp}, " +
                    $"en mi calidad de empleado(a) de la Universidad Regional Región 02 de Guatemala, " +
                    $"manifiesto haber recibido los equipos de tecnología descritos a continuación " +
                    $"en perfectas condiciones de funcionamiento, comprometiéndome a darles el uso " +
                    $"adecuado, mantenerlos en buen estado y responder por cualquier daño, pérdida " +
                    $"o extravío de los mismos.";
                g.DrawString(cuerpo, fontNormal, brush,
                    new RectangleF(leftMargin, currentY, pageWidth, 70));
                currentY += 80;

                g.DrawLine(penGris, leftMargin, currentY, rightMargin, currentY);
                currentY += 15;

                _encabezadoGlobalImpreso = true;
            }
            else
            {
                DibujarLogo();
                currentY = 150;
            }

            // ── Iterar activos desde donde quedó ─────────────────────
            while (_activoImpresionActual < _datosImpresion.Count)
            {
                var (activo, filas) = _datosImpresion[_activoImpresionActual];

                // Subtítulo del activo
                if (_filaImpresionActual == 0)
                {
                    // Verificar espacio para subtítulo + al menos 2 filas
                    if (currentY + rowH * 3 > limiteNormal)
                    {
                        DibujarPie();
                        LiberarRecursos();
                        e.HasMorePages = true;
                        return;
                    }

                    string subtitulo = $"  {activo.AssetCode} — {activo.AssetName?.ToUpper()}";
                    g.FillRectangle(new SolidBrush(Color.FromArgb(51, 140, 255)),
                        tableX, currentY, pageWidth, rowH);
                    g.DrawString(subtitulo, fontTableBold, Brushes.White,
                        tableX + 3, currentY + 4);
                    currentY += rowH;

                    // Encabezado columnas
                    g.FillRectangle(new SolidBrush(Color.FromArgb(30, 80, 160)),
                        tableX, currentY, pageWidth, rowH);
                    g.DrawString("ESPECIFICACIÓN", fontTableBold, Brushes.White,
                        tableX + 3, currentY + 4);
                    g.DrawString("DETALLE", fontTableBold, Brushes.White,
                        tableX + colWidths[0] + 3, currentY + 4);
                    g.DrawRectangle(pen, tableX, currentY, pageWidth, rowH);
                    currentY += rowH;
                }

                bool limiteEsUltima = EsUltimaSeccion(currentY);
                int limiteContenido = limiteEsUltima ? limiteUltima : limiteNormal;

                bool altRow = false;
                int tableBodyStart = currentY;
                int filasEnPagina = 0;

                while (_filaImpresionActual < filas.Count)
                {
                    if (currentY + rowH > limiteContenido)
                    {
                        g.DrawRectangle(pen, tableX, tableBodyStart - rowH * 2, pageWidth,
                            rowH * (filasEnPagina + 2));
                        DibujarPie();
                        LiberarRecursos();
                        e.HasMorePages = true;
                        return;
                    }

                    var fila = filas[_filaImpresionActual];
                    if (altRow)
                        g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 248)),
                            tableX, currentY, pageWidth, rowH);
                    g.DrawString(fila.Etiqueta, fontTableBold, brush, tableX + 3, currentY + 4);
                    g.DrawString(fila.Valor ?? "—", fontTable, brush,
                        tableX + colWidths[0] + 3, currentY + 4);
                    g.DrawRectangle(penGris, tableX, currentY, pageWidth, rowH);
                    currentY += rowH;
                    altRow = !altRow;
                    filasEnPagina++;
                    _filaImpresionActual++;

                    limiteEsUltima = EsUltimaSeccion(currentY);
                    limiteContenido = limiteEsUltima ? limiteUltima : limiteNormal;
                }

                // Borde exterior tabla del activo
                g.DrawRectangle(pen, tableX, tableBodyStart - rowH * 2, pageWidth,
                    rowH * (filasEnPagina + 2));

                currentY += 10;

                // Pasar al siguiente activo
                _activoImpresionActual++;
                _filaImpresionActual = 0;
            }

            // ── Compromiso final y firmas — solo una vez ──────────────
            currentY += 5;

            if (currentY + 65 > limiteUltima)
            {
                DibujarPie();
                LiberarRecursos();
                e.HasMorePages = true;
                return;
            }

            string compromisoFinal =
                "El suscrito se compromete a utilizar los equipos únicamente para las actividades " +
                "institucionales asignadas, a notificar de inmediato cualquier falla o incidente, " +
                "y a devolverlos en las mismas condiciones en que fueron recibidos al finalizar su " +
                "relación laboral o cuando la institución así lo requiera.";
            g.DrawString(compromisoFinal, fontNormal, brush,
                new RectangleF(leftMargin, currentY, pageWidth, 60));

            DibujarPie();

            int firma1X = leftMargin;
            int firma3X = leftMargin + 480;
            int firmaAncho = 200;

            try
            {
                Image sello = Properties.Resources.SelloCoordinacion_Black;
                g.DrawImage(sello, firma3X, selloInicio, firmaAncho, 200);
            }
            catch { }

            g.DrawLine(pen, firma1X, firmaLineaY, firma1X + firmaAncho, firmaLineaY);
            g.DrawLine(pen, firma3X, firmaLineaY, firma3X + firmaAncho, firmaLineaY);

            int textoFirmaY = firmaLineaY + 5;
            string nombreFirma = primerActivo.EmployeeName?.ToUpper() ?? "NOMBRE DEL RESPONSABLE";
            SizeF sF1 = g.MeasureString(nombreFirma, fontSmallBold);
            g.DrawString(nombreFirma, fontSmallBold, brush,
                firma1X + (firmaAncho / 2f) - (sF1.Width / 2f), textoFirmaY);
            SizeF sCargo = g.MeasureString("RESPONSABLE DEL EQUIPO", fontSmall);
            g.DrawString("RESPONSABLE DEL EQUIPO", fontSmall, brush,
                firma1X + (firmaAncho / 2f) - (sCargo.Width / 2f), textoFirmaY + 14);

            SizeF sF3T = g.MeasureString("COORDINACIÓN GENERAL", fontSmallBold);
            SizeF sF3O = g.MeasureString("OFICINAS CENTRALES", fontSmall);
            g.DrawString("COORDINACIÓN GENERAL", fontSmallBold, brush,
                firma3X + (firmaAncho / 2f) - (sF3T.Width / 2f), textoFirmaY);
            g.DrawString("OFICINAS CENTRALES", fontSmall, brush,
                firma3X + (firmaAncho / 2f) - (sF3O.Width / 2f), textoFirmaY + 14);

            LiberarRecursos();
            e.HasMorePages = false;
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion
    }
}