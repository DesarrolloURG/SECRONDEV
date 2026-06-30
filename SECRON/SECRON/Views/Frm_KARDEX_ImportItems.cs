using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_ImportItems : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<FilaImportacion> _filas = new List<FilaImportacion>();

        private class FilaImportacion
        {
            public int NumeroFila { get; set; }
            public string NombreArticulo { get; set; }
            public string Descripcion { get; set; }
            public string NombreCategoria { get; set; }
            public string NombreSubCategoria { get; set; }
            public string NombreUnidad { get; set; }
            public string StockMinimo { get; set; }
            public string StockMaximo { get; set; }
            public string PuntoReorden { get; set; }
            public string CostoUnitario { get; set; }
            public string UltimoPrecio { get; set; }
            public string ControlLotes { get; set; }
            public string ManejaCaducidad { get; set; }

            // Resueltos
            public int? CategoriaId { get; set; }
            public int? SubCategoriaId { get; set; }
            public int? UnidadId { get; set; }

            // Resultado
            public string Estado { get; set; } = "PENDIENTE";
            public string Motivo { get; set; } = "";
        }

        #endregion

        public Frm_KARDEX_ImportItems()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            Txt_RutaArchivo.ReadOnly = true;
        }

        private void Frm_KARDEX_ImportItems_Load(object sender, EventArgs e)
        {
            ConfigurarTabla();
            Lbl_Resultado.Text = "";
            Btn_Importar.Enabled = false;
        }

        #region ConfigurarTabla

        private void ConfigurarTabla()
        {
            Tabla.AutoGenerateColumns = false;
            Tabla.Columns.Clear();
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToResizeRows = false;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            AgregarColumna("NumeroFila", "FILA", 50);
            AgregarColumna("NombreArticulo", "NOMBRE ARTÍCULO", 180);
            AgregarColumna("NombreCategoria", "CATEGORÍA", 120);
            AgregarColumna("NombreSubCategoria", "SUBCATEGORÍA", 120);
            AgregarColumna("NombreUnidad", "UNIDAD", 90);
            AgregarColumna("Descripcion", "DESCRIPCIÓN", 160);
            AgregarColumna("StockMinimo", "STOCK MÍN.", 80);
            AgregarColumna("StockMaximo", "STOCK MÁX.", 80);
            AgregarColumna("CostoUnitario", "COSTO", 80);
            AgregarColumna("ControlLotes", "LOTES", 60);
            AgregarColumna("ManejaCaducidad", "CADUC.", 60);
            AgregarColumna("Estado", "ESTADO", 90);
            AgregarColumna("Motivo", "MOTIVO RECHAZO", 200);
        }

        private void AgregarColumna(string dataProperty, string header, int width)
        {
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = dataProperty,
                HeaderText = header,
                Width = width,
                ReadOnly = true
            });
        }

        private void RefrescarTabla()
        {
            Tabla.DataSource = null;
            Tabla.DataSource = _filas;

            foreach (DataGridViewRow row in Tabla.Rows)
            {
                if (row.DataBoundItem is FilaImportacion fila)
                {
                    if (fila.Estado == "RECHAZADO")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                    else if (fila.Estado == "VÁLIDO")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
                    else if (fila.Estado == "INSERTADO")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(200, 240, 200);
                    else if (fila.Estado == "ACTUALIZADO")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(200, 220, 255);
                    else if (fila.Estado == "ERROR BD")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 180, 180);
                    else
                        row.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        #endregion

        #region CargarExcel

        private void Btn_SeleccionarArchivo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls",
                Title = "Seleccionar archivo de importación"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                Txt_RutaArchivo.Text = dlg.FileName;
                LeerArchivoExcel(dlg.FileName);
            }
        }

        private void LeerArchivoExcel(string rutaArchivo)
        {
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                _filas.Clear();
                Btn_Importar.Enabled = false;
                Lbl_Resultado.Text = "LEYENDO ARCHIVO...";

                excelApp = new Excel.Application { Visible = false };
                workbook = excelApp.Workbooks.Open(rutaArchivo);
                worksheet = (Excel.Worksheet)workbook.Sheets[1];

                int totalColumnas = worksheet.UsedRange.Columns.Count;
                var encabezados = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int col = 1; col <= totalColumnas; col++)
                {
                    string header = worksheet.Cells[1, col]?.Value?.ToString()?.Trim().ToUpper() ?? "";
                    if (!string.IsNullOrEmpty(header))
                        encabezados[header] = col;
                }

                int totalFilas = worksheet.UsedRange.Rows.Count;

                var categorias = Ctrl_ItemCategories.ObtenerCategoriasParaCombo();
                var subCategorias = Ctrl_ItemSubCategories.ObtenerTodasParaCombo();
                var unidades = Ctrl_MeasurementUnits.ObtenerUnidadesParaCombo();

                for (int row = 2; row <= totalFilas; row++)
                {
                    string ObtenerCelda(string columna)
                    {
                        if (!encabezados.ContainsKey(columna)) return "";
                        return worksheet.Cells[row, encabezados[columna]]?.Value?.ToString()?.Trim() ?? "";
                    }

                    var fila = new FilaImportacion
                    {
                        NumeroFila = row,
                        NombreArticulo = ObtenerCelda("NOMBRE_ARTICULO").ToUpper(),
                        Descripcion = ObtenerCelda("DESCRIPCION").ToUpper(),
                        NombreCategoria = ObtenerCelda("CATEGORIA").ToUpper(),
                        NombreSubCategoria = ObtenerCelda("SUBCATEGORIA").ToUpper(),
                        NombreUnidad = ObtenerCelda("UNIDAD_MEDIDA").ToUpper(),
                        StockMinimo = ObtenerCelda("STOCK_MINIMO"),
                        StockMaximo = ObtenerCelda("STOCK_MAXIMO"),
                        PuntoReorden = ObtenerCelda("PUNTO_REORDEN"),
                        CostoUnitario = ObtenerCelda("COSTO_UNITARIO"),
                        UltimoPrecio = ObtenerCelda("ULTIMO_PRECIO_COMPRA"),
                        ControlLotes = ObtenerCelda("CONTROL_LOTES").ToUpper(),
                        ManejaCaducidad = ObtenerCelda("MANEJA_CADUCIDAD").ToUpper()
                    };

                    if (string.IsNullOrWhiteSpace(fila.NombreArticulo) &&
                        string.IsNullOrWhiteSpace(fila.NombreCategoria))
                        continue;

                    var motivos = new List<string>();

                    if (string.IsNullOrWhiteSpace(fila.NombreArticulo))
                        motivos.Add("NOMBRE_ARTICULO requerido");

                    // Resolver Categoría
                    if (!string.IsNullOrWhiteSpace(fila.NombreCategoria))
                    {
                        var cat = categorias.FirstOrDefault(c =>
                            c.Value.Equals(fila.NombreCategoria, StringComparison.OrdinalIgnoreCase));
                        if (cat.Key > 0)
                            fila.CategoriaId = cat.Key;
                        else
                            motivos.Add($"CATEGORÍA '{fila.NombreCategoria}' no existe");
                    }
                    else
                    {
                        motivos.Add("CATEGORIA requerida");
                    }

                    // Resolver SubCategoría (opcional)
                    if (!string.IsNullOrWhiteSpace(fila.NombreSubCategoria) && fila.CategoriaId.HasValue)
                    {
                        var sub = subCategorias.FirstOrDefault(s =>
                        s.CategoryId == fila.CategoriaId.Value &&
                        s.SubCategoryName.Equals(fila.NombreSubCategoria, StringComparison.OrdinalIgnoreCase));
                        if (sub != null)
                            fila.SubCategoriaId = sub.SubCategoryId;
                        if (sub != null)
                            fila.SubCategoriaId = sub.SubCategoryId;
                        else
                            motivos.Add($"SUBCATEGORÍA '{fila.NombreSubCategoria}' no existe en esa categoría");
                    }

                    // Resolver Unidad
                    if (!string.IsNullOrWhiteSpace(fila.NombreUnidad))
                    {
                        var uni = unidades.FirstOrDefault(u =>
                            u.Value.Equals(fila.NombreUnidad, StringComparison.OrdinalIgnoreCase));
                        if (uni.Key > 0)
                            fila.UnidadId = uni.Key;
                        else
                            motivos.Add($"UNIDAD '{fila.NombreUnidad}' no existe");
                    }
                    else
                    {
                        motivos.Add("UNIDAD_MEDIDA requerida");
                    }

                    fila.Estado = motivos.Count > 0 ? "RECHAZADO" : "VÁLIDO";
                    fila.Motivo = string.Join(" | ", motivos);

                    _filas.Add(fila);
                }

                int validos = _filas.Count(f => f.Estado == "VÁLIDO");
                int rechazados = _filas.Count(f => f.Estado == "RECHAZADO");

                Lbl_Resultado.Text = $"ARCHIVO LEÍDO: {_filas.Count} FILAS — {validos} VÁLIDAS — {rechazados} RECHAZADAS";
                Btn_Importar.Enabled = validos > 0;

                RefrescarTabla();
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("ERROR AL LEER ARCHIVO: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                if (workbook != null) { workbook.Close(false); System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook); }
                if (excelApp != null) { excelApp.Quit(); System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp); }
            }
        }

        #endregion

        #region Importar

        private void Btn_Importar_Click(object sender, EventArgs e)
        {
            if (!Btn_Importar.Enabled) return;

            int validos = _filas.Count(f => f.Estado == "VÁLIDO");

            var confirmacion = MessageBox.Show(
                $"SE IMPORTARÁN {validos} ARTÍCULO(S) VÁLIDOS.\n\n¿DESEA CONTINUAR?",
                "CONFIRMAR IMPORTACIÓN",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            this.Cursor = Cursors.WaitCursor;
            Btn_Importar.Enabled = false;

            int insertados = 0;
            int actualizados = 0;
            int errores = 0;

            foreach (var fila in _filas.Where(f => f.Estado == "VÁLIDO"))
            {
                try
                {
                    var item = new Mdl_Items
                    {
                        ItemName = fila.NombreArticulo,
                        Description = string.IsNullOrWhiteSpace(fila.Descripcion) ? null : fila.Descripcion,
                        CategoryId = fila.CategoriaId ?? 0,
                        SubCategoryId = fila.SubCategoriaId ?? 0,
                        UnitId = fila.UnidadId ?? 0,
                        MinimumStock = ParseDecimal(fila.StockMinimo),
                        MaximumStock = ParseDecimal(fila.StockMaximo),
                        ReorderPoint = ParseDecimal(fila.PuntoReorden),
                        UnitCost = ParseDecimal(fila.CostoUnitario),
                        LastPurchasePrice = ParseDecimal(fila.UltimoPrecio),
                        HasLotControl = fila.ControlLotes == "SI",
                        HasExpiryDate = fila.ManejaCaducidad == "SI",
                        CreatedBy = UserData?.UserId ?? 1
                    };

                    int resultado = Ctrl_Items.ImportarArticulo(item, out string codigoGenerado);

                    if (resultado == 1)
                    {
                        fila.Estado = "INSERTADO";
                        fila.Motivo = $"Código: {codigoGenerado}";
                        insertados++;
                    }
                    else if (resultado == 2)
                    {
                        fila.Estado = "ACTUALIZADO";
                        fila.Motivo = $"Código: {codigoGenerado}";
                        actualizados++;
                    }
                    else
                    {
                        fila.Estado = "ERROR BD";
                        fila.Motivo = "Error en base de datos";
                        errores++;
                    }
                }
                catch (Exception ex)
                {
                    fila.Estado = "ERROR BD";
                    fila.Motivo = ex.Message;
                    errores++;
                }
            }

            this.Cursor = Cursors.Default;
            RefrescarTabla();

            int rechazados = _filas.Count(f => f.Estado == "RECHAZADO");

            Lbl_Resultado.Text =
                $"RESULTADO: {insertados} INSERTADOS  |  {actualizados} ACTUALIZADOS  |  " +
                $"{rechazados} RECHAZADOS  |  {errores} ERRORES BD";

            MessageBox.Show(
                $"IMPORTACIÓN COMPLETADA.\n\n" +
                $"Insertados:   {insertados}\n" +
                $"Actualizados: {actualizados}\n" +
                $"Rechazados:   {rechazados}\n" +
                $"Errores BD:   {errores}",
                "RESULTADO DE IMPORTACIÓN",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion

        #region DescargarPlantilla

        private void Btn_DescargarPlantilla_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Guardar plantilla",
                FileName = "PLANTILLA_IMPORTACION_KARDEX.xlsx"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                Excel.Application excelApp = null;
                Excel.Workbook workbook = null;
                Excel.Worksheet worksheet = null;

                try
                {
                    excelApp = new Excel.Application { Visible = false };
                    workbook = excelApp.Workbooks.Add();
                    worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "IMPORTACION";

                    string[] headers = {
                        "NOMBRE_ARTICULO", "DESCRIPCION", "CATEGORIA", "SUBCATEGORIA",
                        "UNIDAD_MEDIDA", "STOCK_MINIMO", "STOCK_MAXIMO", "PUNTO_REORDEN",
                        "COSTO_UNITARIO", "ULTIMO_PRECIO_COMPRA", "CONTROL_LOTES", "MANEJA_CADUCIDAD"
                    };

                    for (int i = 0; i < headers.Length; i++)
                        worksheet.Cells[1, i + 1] = headers[i];

                    var headerRange = worksheet.Range[$"A1:L1"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    // Fila de ejemplo
                    worksheet.Cells[2, 1] = "PAPEL BOND CARTA";
                    worksheet.Cells[2, 2] = "RESMA DE PAPEL BOND TAMAÑO CARTA";
                    worksheet.Cells[2, 3] = "PAPELERIA";
                    worksheet.Cells[2, 4] = "SUMINISTROS";
                    worksheet.Cells[2, 5] = "RESMA";
                    worksheet.Cells[2, 6] = "10";
                    worksheet.Cells[2, 7] = "100";
                    worksheet.Cells[2, 8] = "20";
                    worksheet.Cells[2, 9] = "45.00";
                    worksheet.Cells[2, 10] = "44.50";
                    worksheet.Cells[2, 11] = "NO";
                    worksheet.Cells[2, 12] = "NO";

                    worksheet.Columns.AutoFit();
                    workbook.SaveAs(dlg.FileName);

                    MessageBox.Show("PLANTILLA GUARDADA EXITOSAMENTE.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR AL GENERAR PLANTILLA: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    if (workbook != null) { workbook.Close(false); System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook); }
                    if (excelApp != null) { excelApp.Quit(); System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp); }
                }
            }
        }

        #endregion

        #region Helpers

        private decimal ParseDecimal(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return 0;
            return decimal.TryParse(valor, out decimal result) ? result : 0;
        }

        #endregion
    }
}