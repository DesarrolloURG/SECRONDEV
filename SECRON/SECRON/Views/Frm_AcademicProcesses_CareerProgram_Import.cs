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
    public partial class Frm_AcademicProcesses_CareerProgram_Import : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<FilaImportacion> _filas = new List<FilaImportacion>();

        private class FilaImportacion
        {
            public int NumeroFila { get; set; }
            public string CareerCode { get; set; }
            public string CareerName { get; set; }
            public string Description { get; set; }
            public string DurationYearsTexto { get; set; }
            public string TotalSemestersTexto { get; set; }
            public string TotalCreditsTexto { get; set; }
            public string IsActiveTexto { get; set; }

            // Resueltos
            public int? DurationYears { get; set; }
            public int? TotalSemesters { get; set; }
            public int? TotalCredits { get; set; }
            public bool IsActive { get; set; } = true;

            // Resultado
            public string Estado { get; set; } = "PENDIENTE";
            public string Motivo { get; set; } = "";
        }

        #endregion

        public Frm_AcademicProcesses_CareerProgram_Import()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            Txt_RutaArchivo.ReadOnly = true;
        }

        private void Frm_AcademicProcesses_CareerProgram_Import_Load(object sender, EventArgs e)
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
            AgregarColumna("CareerCode", "CÓDIGO", 90);
            AgregarColumna("CareerName", "NOMBRE", 180);
            AgregarColumna("Description", "DESCRIPCIÓN", 160);
            AgregarColumna("DurationYearsTexto", "DURACIÓN", 70);
            AgregarColumna("TotalSemestersTexto", "SEMESTRES", 80);
            AgregarColumna("TotalCreditsTexto", "CRÉDITOS", 70);
            AgregarColumna("IsActiveTexto", "ACTIVA", 60);
            AgregarColumna("Estado", "ESTADO", 90);
            AgregarColumna("Motivo", "MOTIVO RECHAZO", 220);
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
                    else if (fila.Estado == "RECHAZADO POR DUPLICADO")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 150);
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
                var codigosVistosEnArchivo = new HashSet<string>();

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
                        CareerCode = ObtenerCelda("CODIGO").ToUpper(),
                        CareerName = ObtenerCelda("NOMBRE").ToUpper(),
                        Description = ObtenerCelda("DESCRIPCION").ToUpper(),
                        DurationYearsTexto = ObtenerCelda("DURACION_ANIOS"),
                        TotalSemestersTexto = ObtenerCelda("TOTAL_SEMESTRES"),
                        TotalCreditsTexto = ObtenerCelda("TOTAL_CREDITOS"),
                        IsActiveTexto = ObtenerCelda("ACTIVA").ToUpper()
                    };

                    if (string.IsNullOrWhiteSpace(fila.CareerCode) && string.IsNullOrWhiteSpace(fila.CareerName))
                        continue;

                    var motivos = new List<string>();

                    // CODIGO
                    if (string.IsNullOrWhiteSpace(fila.CareerCode))
                        motivos.Add("CODIGO requerido");
                    else if (!codigosVistosEnArchivo.Add(fila.CareerCode))
                        motivos.Add($"CÓDIGO '{fila.CareerCode}' duplicado dentro del mismo archivo");

                    // NOMBRE
                    if (string.IsNullOrWhiteSpace(fila.CareerName))
                        motivos.Add("NOMBRE requerido");

                    // DURACION (obligatorio)
                    if (string.IsNullOrWhiteSpace(fila.DurationYearsTexto))
                    {
                        motivos.Add("DURACION_ANIOS requerido");
                    }
                    else if (int.TryParse(fila.DurationYearsTexto, out int duracion))
                    {
                        fila.DurationYears = duracion;
                    }
                    else
                    {
                        motivos.Add("DURACION_ANIOS debe ser numérico");
                    }

                    // SEMESTRES (obligatorio)
                    if (string.IsNullOrWhiteSpace(fila.TotalSemestersTexto))
                    {
                        motivos.Add("TOTAL_SEMESTRES requerido");
                    }
                    else if (int.TryParse(fila.TotalSemestersTexto, out int semestres))
                    {
                        fila.TotalSemesters = semestres;
                    }
                    else
                    {
                        motivos.Add("TOTAL_SEMESTRES debe ser numérico");
                    }

                    // CREDITOS (opcional -> puede venir vacío)
                    if (!string.IsNullOrWhiteSpace(fila.TotalCreditsTexto))
                    {
                        if (int.TryParse(fila.TotalCreditsTexto, out int creditos))
                            fila.TotalCredits = creditos;
                        else
                            motivos.Add("TOTAL_CREDITOS debe ser numérico si se especifica");
                    }

                    // ACTIVA (opcional, default SI)
                    if (string.IsNullOrWhiteSpace(fila.IsActiveTexto))
                    {
                        fila.IsActive = true;
                        fila.IsActiveTexto = "SI";
                    }
                    else if (fila.IsActiveTexto == "SI" || fila.IsActiveTexto == "NO")
                    {
                        fila.IsActive = fila.IsActiveTexto == "SI";
                    }
                    else
                    {
                        motivos.Add("ACTIVA debe ser 'SI' o 'NO'");
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
                $"SE IMPORTARÁN {validos} CARRERA(S) VÁLIDAS.\n\n" +
                "LOS CÓDIGOS QUE YA EXISTAN EN EL SISTEMA SERÁN RECHAZADOS (NO SE ACTUALIZARÁN).\n\n¿DESEA CONTINUAR?",
                "CONFIRMAR IMPORTACIÓN",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            this.Cursor = Cursors.WaitCursor;
            Btn_Importar.Enabled = false;

            int insertados = 0;
            int duplicados = 0;
            int errores = 0;

            foreach (var fila in _filas.Where(f => f.Estado == "VÁLIDO"))
            {
                try
                {
                    var carrera = new Mdl_Careers
                    {
                        CareerCode = fila.CareerCode,
                        CareerName = fila.CareerName,
                        Description = string.IsNullOrWhiteSpace(fila.Description) ? null : fila.Description,
                        DurationYears = fila.DurationYears,
                        TotalSemesters = fila.TotalSemesters,
                        TotalCredits = fila.TotalCredits,
                        IsActive = fila.IsActive
                    };

                    int resultado = Ctrl_Careers.ImportarCarrera(carrera, UserData?.UserId ?? 1);

                    if (resultado > 0)
                    {
                        fila.Estado = "INSERTADO";
                        fila.Motivo = "";
                        insertados++;
                    }
                    else
                    {
                        // El SP rechaza por UNIQUE (código duplicado en BD) o por otro error controlado
                        fila.Estado = "RECHAZADO POR DUPLICADO";
                        fila.Motivo = $"El código '{fila.CareerCode}' ya existe en el sistema";
                        duplicados++;
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

            int rechazadosOriginal = _filas.Count(f => f.Estado == "RECHAZADO");

            Lbl_Resultado.Text =
                $"RESULTADO: {insertados} INSERTADOS  |  {duplicados} DUPLICADOS  |  " +
                $"{rechazadosOriginal} RECHAZADOS  |  {errores} ERRORES BD";

            MessageBox.Show(
                $"IMPORTACIÓN COMPLETADA.\n\n" +
                $"Insertados:          {insertados}\n" +
                $"Rechazados (duplic): {duplicados}\n" +
                $"Rechazados (validac):{rechazadosOriginal}\n" +
                $"Errores BD:          {errores}",
                "RESULTADO DE IMPORTACIÓN",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            if (insertados > 0)
                this.DialogResult = DialogResult.OK;
        }

        #endregion

        #region DescargarPlantilla

        private void Btn_DescargarPlantilla_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Guardar plantilla",
                FileName = "PLANTILLA_IMPORTACION_CARRERAS.xlsx"
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
                        "CODIGO", "NOMBRE", "DESCRIPCION", "DURACION_ANIOS",
                        "TOTAL_SEMESTRES", "TOTAL_CREDITOS", "ACTIVA"
                    };

                    for (int i = 0; i < headers.Length; i++)
                        worksheet.Cells[1, i + 1] = headers[i];

                    var headerRange = worksheet.Range["A1:G1"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    // Fila de ejemplo
                    worksheet.Cells[2, 1] = "ING-SIS-01";
                    worksheet.Cells[2, 2] = "INGENIERÍA EN SISTEMAS";
                    worksheet.Cells[2, 3] = "CARRERA ENFOCADA EN DESARROLLO DE SOFTWARE";
                    worksheet.Cells[2, 4] = "5";
                    worksheet.Cells[2, 5] = "10";
                    worksheet.Cells[2, 6] = ""; // TOTAL_CREDITOS puede quedar vacío
                    worksheet.Cells[2, 7] = "SI";

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
    }
}