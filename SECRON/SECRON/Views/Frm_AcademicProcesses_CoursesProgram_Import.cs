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
    public partial class Frm_AcademicProcesses_CoursesProgram_Import : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<FilaImportacion> _filas = new List<FilaImportacion>();

        private class FilaImportacion
        {
            public int NumeroFila { get; set; }
            public string CourseCode { get; set; }
            public string CourseName { get; set; }
            public string Description { get; set; }
            public string CreditsTexto { get; set; }
            public string SessionsTexto { get; set; }
            public string IsCommonTexto { get; set; }
            public string IsActiveTexto { get; set; }

            // Resueltos
            public int Credits { get; set; }
            public int Sessions { get; set; }
            public bool IsCommon { get; set; } = false;
            public bool IsActive { get; set; } = true;

            // Resultado
            public string Estado { get; set; } = "PENDIENTE";
            public string Motivo { get; set; } = "";
        }

        #endregion

        public Frm_AcademicProcesses_CoursesProgram_Import()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            Txt_RutaArchivo.ReadOnly = true;
        }

        private void Frm_AcademicProcesses_CoursesProgram_Import_Load(object sender, EventArgs e)
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
            AgregarColumna("CourseCode", "CÓDIGO", 90);
            AgregarColumna("CourseName", "NOMBRE", 180);
            AgregarColumna("Description", "DESCRIPCIÓN", 160);
            AgregarColumna("CreditsTexto", "CRÉDITOS", 70);
            AgregarColumna("SessionsTexto", "SESIONES", 70);
            AgregarColumna("IsCommonTexto", "EN COMÚN", 70);
            AgregarColumna("IsActiveTexto", "ACTIVO", 60);
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
                        CourseCode = ObtenerCelda("CODIGO").ToUpper(),
                        CourseName = ObtenerCelda("NOMBRE").ToUpper(),
                        Description = ObtenerCelda("DESCRIPCION").ToUpper(),
                        CreditsTexto = ObtenerCelda("CREDITOS"),
                        SessionsTexto = ObtenerCelda("SESIONES"),
                        IsCommonTexto = ObtenerCelda("EN_COMUN").ToUpper(),
                        IsActiveTexto = ObtenerCelda("ACTIVO").ToUpper()
                    };

                    if (string.IsNullOrWhiteSpace(fila.CourseCode) && string.IsNullOrWhiteSpace(fila.CourseName))
                        continue;

                    var motivos = new List<string>();

                    // CODIGO
                    if (string.IsNullOrWhiteSpace(fila.CourseCode))
                        motivos.Add("CODIGO requerido");
                    else if (!codigosVistosEnArchivo.Add(fila.CourseCode))
                        motivos.Add($"CÓDIGO '{fila.CourseCode}' duplicado dentro del mismo archivo");

                    // NOMBRE
                    if (string.IsNullOrWhiteSpace(fila.CourseName))
                        motivos.Add("NOMBRE requerido");

                    // CREDITOS (obligatorio)
                    if (string.IsNullOrWhiteSpace(fila.CreditsTexto))
                    {
                        motivos.Add("CREDITOS requerido");
                    }
                    else if (int.TryParse(fila.CreditsTexto, out int creditos))
                    {
                        fila.Credits = creditos;
                    }
                    else
                    {
                        motivos.Add("CREDITOS debe ser numérico");
                    }

                    // SESIONES (obligatorio)
                    if (string.IsNullOrWhiteSpace(fila.SessionsTexto))
                    {
                        motivos.Add("SESIONES requerido");
                    }
                    else if (int.TryParse(fila.SessionsTexto, out int sesiones))
                    {
                        fila.Sessions = sesiones;
                    }
                    else
                    {
                        motivos.Add("SESIONES debe ser numérico");
                    }

                    // EN_COMUN (opcional, default NO)
                    if (string.IsNullOrWhiteSpace(fila.IsCommonTexto))
                    {
                        fila.IsCommon = false;
                        fila.IsCommonTexto = "NO";
                    }
                    else if (fila.IsCommonTexto == "SI" || fila.IsCommonTexto == "NO")
                    {
                        fila.IsCommon = fila.IsCommonTexto == "SI";
                    }
                    else
                    {
                        motivos.Add("EN_COMUN debe ser 'SI' o 'NO'");
                    }

                    // ACTIVO (opcional, default SI)
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
                        motivos.Add("ACTIVO debe ser 'SI' o 'NO'");
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
                $"SE IMPORTARÁN {validos} CURSO(S) VÁLIDOS.\n\n" +
                "LOS CÓDIGOS QUE YA EXISTAN EN EL SISTEMA SERÁN RECHAZADOS (NO SE ACTUALIZARÁN).\n\n" +
                "NOTA: LAS HORAS TEÓRICAS, PRÁCTICAS Y DE LABORATORIO QUEDARÁN SIN INFORMACIÓN (NULL), " +
                "YA QUE ESTE ARCHIVO NO LAS INCLUYE.\n\n¿DESEA CONTINUAR?",
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
                    var curso = new Mdl_Courses
                    {
                        CourseCode = fila.CourseCode,
                        CourseName = fila.CourseName,
                        Description = string.IsNullOrWhiteSpace(fila.Description) ? null : fila.Description,
                        Credits = fila.Credits,
                        TheoryHours = null,
                        PracticeHours = null,
                        LabHours = null,
                        Sessions = fila.Sessions,
                        IsCommon = fila.IsCommon,
                        IsActive = fila.IsActive
                    };

                    int resultado = Ctrl_Courses.ImportarCurso(curso, UserData?.UserId ?? 1);

                    if (resultado > 0)
                    {
                        fila.Estado = "INSERTADO";
                        fila.Motivo = "";
                        insertados++;
                    }
                    else
                    {
                        fila.Estado = "RECHAZADO POR DUPLICADO";
                        fila.Motivo = $"El código '{fila.CourseCode}' ya existe en el sistema";
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
                FileName = "PLANTILLA_IMPORTACION_CURSOS.xlsx"
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
                        "CODIGO", "NOMBRE", "DESCRIPCION", "CREDITOS", "SESIONES", "EN_COMUN", "ACTIVO"
                    };

                    for (int i = 0; i < headers.Length; i++)
                        worksheet.Cells[1, i + 1] = headers[i];

                    var headerRange = worksheet.Range["A1:G1"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    // Fila de ejemplo
                    worksheet.Cells[2, 1] = "MAT-101";
                    worksheet.Cells[2, 2] = "MATEMÁTICA BÁSICA";
                    worksheet.Cells[2, 3] = "CURSO INTRODUCTORIO DE MATEMÁTICA";
                    worksheet.Cells[2, 4] = "4";
                    worksheet.Cells[2, 5] = "10";
                    worksheet.Cells[2, 6] = "SI";
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