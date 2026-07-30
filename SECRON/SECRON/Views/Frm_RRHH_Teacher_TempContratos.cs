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
    public partial class Frm_RRHH_Teacher_TempContratos : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<FilaImportacion> _filas = new List<FilaImportacion>();

        private class FilaImportacion
        {
            public int NumeroFila { get; set; }
            public string TipoFila { get; set; } = ""; // MAESTRO / DETALLE

            // Datos del docente (OBLIGATORIOS EN TODA FILA, sea MAESTRO o DETALLE)
            public string DPI { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string BirthDateStr { get; set; }
            public string MaritalStatus { get; set; }
            public string Gender { get; set; }
            public string Address { get; set; }
            public string Nationality { get; set; }
            public string CollegiateNumber { get; set; }
            public string NIT { get; set; }
            public string Cycle { get; set; }
            public string ContractYearStr { get; set; }
            public string IssueDateStr { get; set; }

            // Datos del curso (OBLIGATORIOS EN TODA FILA)
            public string AcademicLocation { get; set; }
            public string CourseToTeach { get; set; }
            public string Schedule { get; set; }
            public string FeesStr { get; set; }

            // Resultado
            public string Estado { get; set; } = "PENDIENTE";
            public string Motivo { get; set; } = "";
            public string ContractCode { get; set; } = "";
        }

        

        public Frm_RRHH_Teacher_TempContratos()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            Txt_RutaArchivo.ReadOnly = true;
        }

        private void Frm_RRHH_Teacher_TempContratos_Load(object sender, EventArgs e)
        {
            ConfigurarTabla();
            Lbl_Resultado.Text = "";
            Btn_Importar.Enabled = false;
        }
        #endregion Propiedades
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

            AgregarColumna("NumeroFila", "FILA", 45);
            AgregarColumna("TipoFila", "TIPO", 75);
            AgregarColumna("DPI", "DPI", 100);
            AgregarColumna("FirstName", "NOMBRES", 130);
            AgregarColumna("LastName", "APELLIDOS", 130);
            AgregarColumna("AcademicLocation", "SEDE", 120);
            AgregarColumna("CourseToTeach", "CURSO", 140);
            AgregarColumna("Schedule", "HORARIO", 100);
            AgregarColumna("FeesStr", "HONORARIOS", 90);
            AgregarColumna("ContractCode", "CÓDIGO CONTRATO", 130);
            AgregarColumna("Estado", "ESTADO", 90);
            AgregarColumna("Motivo", "MOTIVO", 260);
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

                // Cachés para no repetir consultas a BD por cada fila del mismo DPI
                var dpiExistenteEnBD = new Dictionary<string, Mdl_DocentesTemporal>();
                var cursosExistentesEnBD = new Dictionary<string, int>();
                var dpiYaVistoEnArchivo = new HashSet<string>();
                var cursosAsignadosEnArchivo = new Dictionary<string, int>();
                var sedesValidadasEnArchivo = new Dictionary<string, int?>(); 

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
                        DPI = ObtenerCelda("DPI"),
                        FirstName = ObtenerCelda("NOMBRES").ToUpper(),
                        LastName = ObtenerCelda("APELLIDOS").ToUpper(),
                        BirthDateStr = ObtenerCelda("FECHA_NACIMIENTO"),
                        MaritalStatus = ObtenerCelda("ESTADO_CIVIL").ToUpper(),
                        Gender = ObtenerCelda("SEXO").ToUpper(),
                        Address = ObtenerCelda("DOMICILIO").ToUpper(),
                        Nationality = ObtenerCelda("NACIONALIDAD").ToUpper(),
                        CollegiateNumber = ObtenerCelda("NUMERO_COLEGIADO"),
                        NIT = ObtenerCelda("NIT"),
                        Cycle = ObtenerCelda("CICLO").ToUpper(),
                        ContractYearStr = ObtenerCelda("ANIO"),
                        IssueDateStr = ObtenerCelda("FECHA_EMISION"),
                        AcademicLocation = ObtenerCelda("SEDE_ACADEMICA").ToUpper(),
                        CourseToTeach = ObtenerCelda("CURSO_A_IMPARTIR").ToUpper(),
                        Schedule = ObtenerCelda("HORARIO").ToUpper(),
                        FeesStr = ObtenerCelda("HONORARIOS")
                    };

                    // Fila completamente vacía: se ignora, no se agrega a la tabla
                    bool filaTotalmenteVacia =
                        string.IsNullOrWhiteSpace(fila.DPI) && string.IsNullOrWhiteSpace(fila.FirstName) &&
                        string.IsNullOrWhiteSpace(fila.LastName) && string.IsNullOrWhiteSpace(fila.AcademicLocation) &&
                        string.IsNullOrWhiteSpace(fila.CourseToTeach);
                    if (filaTotalmenteVacia)
                        continue;

                    var motivos = new List<string>();

                    // ---------- TODOS los campos son obligatorios, sin excepción ----------
                    if (string.IsNullOrWhiteSpace(fila.DPI))
                        motivos.Add("DPI requerido");
                    else if (!fila.DPI.All(char.IsDigit))
                        motivos.Add("DPI debe ser solo numérico");

                    if (string.IsNullOrWhiteSpace(fila.FirstName))
                        motivos.Add("NOMBRES requerido");

                    if (string.IsNullOrWhiteSpace(fila.LastName))
                        motivos.Add("APELLIDOS requerido");

                    if (string.IsNullOrWhiteSpace(fila.BirthDateStr))
                        motivos.Add("FECHA_NACIMIENTO requerida");
                    else if (ParseDate(fila.BirthDateStr) == null)
                        motivos.Add("FECHA_NACIMIENTO inválida");

                    if (string.IsNullOrWhiteSpace(fila.MaritalStatus))
                        motivos.Add("ESTADO_CIVIL requerido");

                    if (string.IsNullOrWhiteSpace(fila.Gender))
                        motivos.Add("SEXO requerido");

                    if (string.IsNullOrWhiteSpace(fila.Address))
                        motivos.Add("DOMICILIO requerido");

                    if (string.IsNullOrWhiteSpace(fila.Nationality))
                        motivos.Add("NACIONALIDAD requerida");

                    if (string.IsNullOrWhiteSpace(fila.CollegiateNumber))
                        motivos.Add("NUMERO_COLEGIADO requerido");

                    if (string.IsNullOrWhiteSpace(fila.NIT))
                        motivos.Add("NIT requerido");

                    if (string.IsNullOrWhiteSpace(fila.Cycle))
                        motivos.Add("CICLO requerido");

                    if (string.IsNullOrWhiteSpace(fila.ContractYearStr))
                        motivos.Add("ANIO requerido");
                    else if (ParseInt(fila.ContractYearStr) == null)
                        motivos.Add("ANIO inválido");

                    if (string.IsNullOrWhiteSpace(fila.IssueDateStr))
                        motivos.Add("FECHA_EMISION requerida");
                    else if (ParseDate(fila.IssueDateStr) == null)
                        motivos.Add("FECHA_EMISION inválida");

                    if (string.IsNullOrWhiteSpace(fila.AcademicLocation))
                        motivos.Add("SEDE_ACADEMICA requerida");
                    else
                    {
                        if (!sedesValidadasEnArchivo.ContainsKey(fila.AcademicLocation))
                            sedesValidadasEnArchivo[fila.AcademicLocation] =
                                Ctrl_Locations.ObtenerLocationIdPorNombreExacto(fila.AcademicLocation);

                        if (sedesValidadasEnArchivo[fila.AcademicLocation] == null)
                            motivos.Add($"SEDE_ACADEMICA \"{fila.AcademicLocation}\" no existe en SECRON (debe coincidir EXACTO con el nombre de la sede)");
                    }

                    if (string.IsNullOrWhiteSpace(fila.CourseToTeach))
                        motivos.Add("CURSO_A_IMPARTIR requerido");

                    if (string.IsNullOrWhiteSpace(fila.Schedule))
                        motivos.Add("HORARIO requerido");

                    if (string.IsNullOrWhiteSpace(fila.FeesStr))
                        motivos.Add("HONORARIOS requerido");
                    else if (ParseDecimal(fila.FeesStr) == null)
                        motivos.Add("HONORARIOS inválido");

                    // ---------- Resolver TIPO DE FILA (MAESTRO / DETALLE) y máximo de 5 cursos ----------
                    if (!string.IsNullOrWhiteSpace(fila.DPI))
                    {
                        if (!dpiExistenteEnBD.ContainsKey(fila.DPI))
                            dpiExistenteEnBD[fila.DPI] = Ctrl_DocentesTemporal.ObtenerPorDPI(fila.DPI);

                        if (!cursosExistentesEnBD.ContainsKey(fila.DPI))
                            cursosExistentesEnBD[fila.DPI] = Ctrl_DocentesTemporal_Cursos.ContarPorDPI(fila.DPI);

                        var existente = dpiExistenteEnBD[fila.DPI];
                        bool primeraVezEnArchivo = !dpiYaVistoEnArchivo.Contains(fila.DPI);
                        dpiYaVistoEnArchivo.Add(fila.DPI);

                        fila.TipoFila = existente != null
                            ? "DETALLE"
                            : (primeraVezEnArchivo ? "MAESTRO" : "DETALLE");

                        int totalPrevios = cursosExistentesEnBD[fila.DPI] +
                            (cursosAsignadosEnArchivo.ContainsKey(fila.DPI) ? cursosAsignadosEnArchivo[fila.DPI] : 0);

                        if (totalPrevios >= 5)
                            motivos.Add("DPI ya alcanzó el máximo de 5 cursos");
                    }

                    fila.Estado = motivos.Count > 0 ? "RECHAZADO" : "VÁLIDO";
                    fila.Motivo = string.Join(" | ", motivos);

                    if (fila.Estado == "VÁLIDO" && !string.IsNullOrWhiteSpace(fila.DPI))
                    {
                        cursosAsignadosEnArchivo[fila.DPI] =
                            (cursosAsignadosEnArchivo.ContainsKey(fila.DPI) ? cursosAsignadosEnArchivo[fila.DPI] : 0) + 1;
                    }

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
                $"SE PROCESARÁN {validos} FILA(S) VÁLIDA(S) (DOCENTES NUEVOS + CURSOS).\n\n¿DESEA CONTINUAR?",
                "CONFIRMAR IMPORTACIÓN",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            this.Cursor = Cursors.WaitCursor;
            Btn_Importar.Enabled = false;

            int docentesNuevos = 0;
            int cursosInsertados = 0;
            int errores = 0;

            // DPI -> TeacherTempId creado durante ESTA ejecución de importación
            var teacherTempIdsCreados = new Dictionary<string, int>();
            // DPI -> ContractCode creado durante ESTA ejecución (para mostrarlo en filas DETALLE del mismo lote)
            var contractCodesCreados = new Dictionary<string, string>();

            foreach (var fila in _filas.Where(f => f.Estado == "VÁLIDO"))
            {
                try
                {
                    int teacherTempId;

                    if (fila.TipoFila == "MAESTRO")
                    {
                        int anio = ParseInt(fila.ContractYearStr) ?? DateTime.Now.Year;
                        string contractCode = Ctrl_DocentesTemporal.ObtenerProximoCodigo(anio);

                        if (string.IsNullOrWhiteSpace(contractCode))
                        {
                            fila.Estado = "ERROR BD";
                            fila.Motivo = "No se pudo generar el código de contrato";
                            errores++;
                            continue;
                        }

                        var docente = new Mdl_DocentesTemporal
                        {
                            ContractCode = contractCode,
                            DPI = fila.DPI,
                            FirstName = fila.FirstName,
                            LastName = fila.LastName,
                            BirthDate = ParseDate(fila.BirthDateStr),
                            MaritalStatus = fila.MaritalStatus,
                            Gender = fila.Gender,
                            Address = fila.Address,
                            Nationality = fila.Nationality,
                            CollegiateNumber = fila.CollegiateNumber,
                            NIT = fila.NIT,
                            Cycle = fila.Cycle,
                            ContractYear = anio,
                            IssueDate = ParseDate(fila.IssueDateStr),
                            CreatedBy = UserData?.UserId
                        };

                        int nuevoId = Ctrl_DocentesTemporal.Insert(docente);

                        if (nuevoId <= 0)
                        {
                            fila.Estado = "ERROR BD";
                            fila.Motivo = "No se pudo registrar el docente (maestro)";
                            errores++;
                            continue;
                        }

                        teacherTempId = nuevoId;
                        teacherTempIdsCreados[fila.DPI] = nuevoId;
                        contractCodesCreados[fila.DPI] = contractCode;
                        fila.ContractCode = contractCode;
                        docentesNuevos++;
                    }
                    else // DETALLE
                    {
                        if (teacherTempIdsCreados.ContainsKey(fila.DPI))
                        {
                            teacherTempId = teacherTempIdsCreados[fila.DPI];
                            fila.ContractCode = contractCodesCreados[fila.DPI];
                        }
                        else
                        {
                            var existente = Ctrl_DocentesTemporal.ObtenerPorDPI(fila.DPI);
                            if (existente == null)
                            {
                                fila.Estado = "ERROR BD";
                                fila.Motivo = "No se encontró el docente (maestro) para este DPI";
                                errores++;
                                continue;
                            }
                            teacherTempId = existente.TeacherTempId;
                            fila.ContractCode = existente.ContractCode;
                        }
                    }

                    var curso = new Mdl_DocentesTemporal_Cursos
                    {
                        TeacherTempId = teacherTempId,
                        AcademicLocation = fila.AcademicLocation,
                        CourseToTeach = fila.CourseToTeach,
                        Schedule = fila.Schedule,
                        Fees = ParseDecimal(fila.FeesStr),
                        CreatedBy = UserData?.UserId
                    };

                    int resultado = Ctrl_DocentesTemporal_Cursos.Insert(curso);

                    if (resultado > 0)
                    {
                        fila.Estado = "INSERTADO";
                        fila.Motivo = fila.TipoFila == "MAESTRO"
                            ? $"Docente y curso creados (Código: {fila.ContractCode})"
                            : $"Curso agregado (Código: {fila.ContractCode})";
                        cursosInsertados++;
                    }
                    else
                    {
                        fila.Estado = "ERROR BD";
                        fila.Motivo = "Error al registrar el curso";
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
                $"RESULTADO: {docentesNuevos} DOCENTES NUEVOS  |  {cursosInsertados} CURSOS INSERTADOS  |  " +
                $"{rechazados} RECHAZADOS  |  {errores} ERRORES BD";

            MessageBox.Show(
                $"IMPORTACIÓN COMPLETADA.\n\n" +
                $"Docentes nuevos (contratos generados): {docentesNuevos}\n" +
                $"Cursos insertados (total):              {cursosInsertados}\n" +
                $"Rechazados:                              {rechazados}\n" +
                $"Errores BD:                               {errores}",
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
                FileName = "PLANTILLA_IMPORTACION_CONTRATOS_DOCENTES.xlsx"
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
                        "DPI", "NOMBRES", "APELLIDOS", "FECHA_NACIMIENTO", "ESTADO_CIVIL", "SEXO",
                        "DOMICILIO", "NACIONALIDAD", "NUMERO_COLEGIADO", "NIT", "CICLO", "ANIO",
                        "FECHA_EMISION", "SEDE_ACADEMICA", "CURSO_A_IMPARTIR", "HORARIO", "HONORARIOS"
                    };

                    for (int i = 0; i < headers.Length; i++)
                        worksheet.Cells[1, i + 1] = headers[i];

                    var headerRange = worksheet.Range[$"A1:Q1"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    // Fila de ejemplo 1: primera aparición del docente (MAESTRO), TODOS los campos completos
                    string[] ejemploFila1 = {
                        "1234567890101", "JUAN CARLOS", "PEREZ LOPEZ", "15/03/1985", "SOLTERO", "MASCULINO",
                        "5TA AVENIDA 10-20 ZONA 1", "GUATEMALTECA", "12345", "1234567-8", "SEGUNDO CICLO", "2026",
                        "01/07/2026", "SEDE CENTRAL", "MATEMATICA I", "LUNES A VIERNES 18:00-20:00", "3500.00"
                    };
                    for (int i = 0; i < ejemploFila1.Length; i++)
                        worksheet.Cells[2, i + 1] = ejemploFila1[i];

                    // Fila de ejemplo 2: mismo DPI, segundo curso -> TODOS los campos igual completos (se repiten)
                    string[] ejemploFila2 = {
                        "1234567890101", "JUAN CARLOS", "PEREZ LOPEZ", "15/03/1985", "SOLTERO", "MASCULINO",
                        "5TA AVENIDA 10-20 ZONA 1", "GUATEMALTECA", "12345", "1234567-8", "SEGUNDO CICLO", "2026",
                        "01/07/2026", "SEDE NORTE", "MATEMATICA II", "SABADOS 08:00-12:00", "1800.00"
                    };
                    for (int i = 0; i < ejemploFila2.Length; i++)
                        worksheet.Cells[3, i + 1] = ejemploFila2[i];

                    worksheet.Columns.AutoFit();
                    workbook.SaveAs(dlg.FileName);

                    MessageBox.Show(
                        "PLANTILLA GUARDADA EXITOSAMENTE.\n\n" +
                        "NOTA IMPORTANTE: TODOS los campos son obligatorios en TODAS las filas, sin excepción. " +
                        "Si un mismo docente (DPI) imparte varios cursos, agregue una fila por cada curso, " +
                        "repitiendo los datos personales completos en cada una (solo cambian SEDE_ACADEMICA, " +
                        "CURSO_A_IMPARTIR, HORARIO y HONORARIOS). Una fila incompleta será rechazada.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private decimal? ParseDecimal(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            return decimal.TryParse(valor, out decimal result) ? result : (decimal?)null;
        }

        private int? ParseInt(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            return int.TryParse(valor, out int result) ? result : (int?)null;
        }

        private DateTime? ParseDate(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            return DateTime.TryParse(valor, out DateTime result) ? result : (DateTime?)null;
        }

        #endregion

        private void Btn_Contratos_Click(object sender, EventArgs e)
        {
            try
            {
                using (Frm_RRHH_Teachers_TempGenerarContratos frmGenerar = new Frm_RRHH_Teachers_TempGenerarContratos())
                {
                    frmGenerar.UserData = UserData;
                    frmGenerar.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ABRIR EL FORMULARIO DE GENERAR CONTRATOS: " + ex.Message,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}