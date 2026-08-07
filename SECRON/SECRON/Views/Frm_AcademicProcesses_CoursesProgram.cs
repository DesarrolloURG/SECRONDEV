using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_CoursesProgram : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_Courses> cursosList;
        private Mdl_Courses _cursoSeleccionado = null;

        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;

        private string _ultimoCampo = "TODOS";
        private string _ultimoValor = null;
        private string _ultimoEstado = "TODOS";
        private string _ultimoComun = "TODOS";

        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        private Ctrl_Security_Auth authController = new Ctrl_Security_Auth();
        private List<string> permisosUsuario = new List<string>();

        #endregion

        public Frm_AcademicProcesses_CoursesProgram()
        {
            InitializeComponent();
        }

        private async void Frm_AcademicProcesses_CoursesProgram_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarFiltros();
                ConfigurarNumericos();
                Txt_IsActive.ReadOnly = true;
                ConfigurarTabla();
                CrearToolStripPaginacion();
                BuscarCursos();

                if (UserData != null)
                {
                    await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
                    ConfigurarBotonesPorPermisos();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("ERROR AL CARGAR FORMULARIO: " + ex.Message, "ERROR SECRON",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region SistemaDePermisos

        private async Task CargarPermisosUsuario(int userId, int roleId)
        {
            try
            {
                permisosUsuario = await authController.ObtenerPermisosUsuarioAsync(userId, roleId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CARGAR PERMISOS: " + ex.Message, "ERROR SECRON",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TienePermiso(string permissionCode)
        {
            return permisosUsuario != null && permisosUsuario.Contains(permissionCode);
        }

        private void ConfigurarBotonesPorPermisos()
        {
            Btn_Save.Enabled = TienePermiso("ACADEMICPROCESSES_COURSESPROGRAM_CREATE");
            if (!Btn_Save.Enabled) { Btn_Save.BackColor = Color.FromArgb(200, 200, 200); Btn_Save.ForeColor = Color.Gray; Btn_Save.Cursor = Cursors.No; }

            Btn_Update.Enabled = TienePermiso("ACADEMICPROCESSES_COURSESPROGRAM_UPDATE");
            if (!Btn_Update.Enabled) { Btn_Update.BackColor = Color.FromArgb(200, 200, 200); Btn_Update.ForeColor = Color.Gray; Btn_Update.Cursor = Cursors.No; }

            Btn_IsActive.Enabled = TienePermiso("ACADEMICPROCESSES_COURSESPROGRAM_INACTIVE");
            if (!Btn_IsActive.Enabled) { Btn_IsActive.BackColor = Color.FromArgb(200, 200, 200); Btn_IsActive.ForeColor = Color.Gray; Btn_IsActive.Cursor = Cursors.No; }

            Btn_Import.Enabled = TienePermiso("ACADEMICPROCESSES_COURSESPROGRAM_IMPORT");
            if (!Btn_Import.Enabled) { Btn_Import.BackColor = Color.FromArgb(200, 200, 200); Btn_Import.ForeColor = Color.Gray; Btn_Import.Cursor = Cursors.No; }
        }

        #endregion

        #region ConfigurarFiltros

        private void ConfigurarFiltros()
        {
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("CODIGO");
            Filtro1.Items.Add("NOMBRE");
            Filtro1.Items.Add("CREDITOS");
            Filtro1.Items.Add("SESIONES");
            Filtro1.SelectedIndex = 0;

            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS");
            Filtro2.Items.Add("ACTIVA");
            Filtro2.Items.Add("INACTIVA");
            Filtro2.SelectedIndex = 0;

            // Filtro3 = ¿Curso en común?
            Filtro3.Items.Clear();
            Filtro3.Items.Add("TODOS");
            Filtro3.Items.Add("SI");
            Filtro3.Items.Add("NO");
            Filtro3.SelectedIndex = 0;

            Txt_ValorBuscado.Text = "";
        }

        #endregion

        #region ConfigurarNumericos

        private void ConfigurarNumericos()
        {
            numericUpDownTotalCredits.Minimum = 0;
            numericUpDownTotalCredits.Maximum = 20;

            // TheoryHours / PracticeHours / LabHours: 0 = "SIN INFORMACIÓN" (se guarda como NULL)
            numericUpDownTheoryHours.Minimum = 0;
            numericUpDownTheoryHours.Maximum = 40;

            numericUpDownPracticeHours.Minimum = 0;
            numericUpDownPracticeHours.Maximum = 40;

            numericUpDownLabHours.Minimum = 0;
            numericUpDownLabHours.Maximum = 40;

            numericUpDownSessions.Minimum = 0;
            numericUpDownSessions.Maximum = 60;
        }

        #endregion

        #region ConfigurarTabla

        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;

            Tabla.Columns.Add("CourseId", "ID");
            Tabla.Columns.Add("CourseCode", "CÓDIGO");
            Tabla.Columns.Add("CourseName", "NOMBRE");
            Tabla.Columns.Add("Credits", "CRÉDITOS");
            Tabla.Columns.Add("Sessions", "SESIONES");
            Tabla.Columns.Add("TotalHours", "HORAS TOTALES");
            Tabla.Columns.Add("IsCommon", "EN COMÚN");
            Tabla.Columns.Add("IsActive", "ESTADO");

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.RowHeadersVisible = false;

            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            Tabla.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            Tabla.RowTemplate.Height = 30;
            Tabla.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            Tabla.Columns["CourseId"].Visible = false;

            Tabla.Columns["CourseName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CourseName"].FillWeight = 30;
            Tabla.Columns["CourseCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CourseCode"].FillWeight = 15;
            Tabla.Columns["Credits"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["Credits"].FillWeight = 12;
            Tabla.Columns["Sessions"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["Sessions"].FillWeight = 12;
            Tabla.Columns["TotalHours"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["TotalHours"].FillWeight = 13;
            Tabla.Columns["IsCommon"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["IsCommon"].FillWeight = 10;
            Tabla.Columns["IsActive"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["IsActive"].FillWeight = 12;

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void MostrarCursosEnTabla()
        {
            Tabla.Rows.Clear();

            foreach (var curso in cursosList)
            {
                Tabla.Rows.Add(
                    curso.CourseId,
                    curso.CourseCode,
                    curso.CourseName,
                    curso.Credits,
                    curso.Sessions.HasValue ? curso.Sessions.ToString() : "N/A",
                    curso.TotalHours.HasValue ? curso.TotalHours.ToString() : "N/A",
                    curso.IsCommon ? "SI" : "NO",
                    curso.IsActive ? "ACTIVA" : "INACTIVA"
                );
            }
        }

        #endregion

        #region Busqueda

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string campo = Filtro1.SelectedItem?.ToString() ?? "TODOS";
            string valor = Txt_ValorBuscado.Text.Trim();
            string estado = Filtro2.SelectedItem?.ToString() ?? "TODOS";
            string comun = Filtro3.SelectedItem?.ToString() ?? "TODOS";

            if (campo != "TODOS" && string.IsNullOrWhiteSpace(valor))
            {
                MessageBox.Show("INGRESE UN VALOR PARA BUSCAR.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if ((campo == "CREDITOS" || campo == "SESIONES") && !int.TryParse(valor, out _))
            {
                MessageBox.Show("PARA ESTE CAMPO EL VALOR DEBE SER NUMÉRICO.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _ultimoCampo = campo;
            _ultimoValor = campo == "TODOS" ? null : valor;
            _ultimoEstado = estado;
            _ultimoComun = comun;

            paginaActual = 1;
            BuscarCursos();
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 0;
            Txt_ValorBuscado.Text = "";

            _ultimoCampo = "TODOS";
            _ultimoValor = null;
            _ultimoEstado = "TODOS";
            _ultimoComun = "TODOS";

            paginaActual = 1;
            BuscarCursos();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }

        private void BuscarCursos()
        {
            try
            {
                cursosList = Ctrl_Courses.ObtenerCursos(
                    _ultimoCampo, _ultimoValor, _ultimoEstado, _ultimoComun,
                    paginaActual, registrosPorPagina, out totalRegistros);

                MostrarCursosEnTabla();

                totalPaginas = totalRegistros == 0 ? 0 : (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
                ActualizarInfoPaginacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL BUSCAR CURSOS: " + ex.Message, "ERROR SECRON",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Paginacion

        private void CrearToolStripPaginacion()
        {
            toolStripPaginacion = new ToolStrip();
            toolStripPaginacion.Dock = DockStyle.None;
            toolStripPaginacion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            toolStripPaginacion.GripStyle = ToolStripGripStyle.Hidden;
            toolStripPaginacion.BackColor = Color.FromArgb(248, 249, 250);
            toolStripPaginacion.Height = 39;
            toolStripPaginacion.AutoSize = true;

            btnAnterior = new ToolStripButton();
            btnAnterior.Text = "❮ Anterior";
            btnAnterior.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnterior.ForeColor = Color.White;
            btnAnterior.BackColor = Color.FromArgb(51, 140, 255);
            btnAnterior.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAnterior.Margin = new Padding(2);
            btnAnterior.Padding = new Padding(8, 4, 8, 4);
            btnAnterior.Click += (s, e) => CambiarPagina(paginaActual - 1);
            toolStripPaginacion.Items.Add(btnAnterior);

            btnSiguiente = new ToolStripButton();
            btnSiguiente.Text = "Siguiente ❯";
            btnSiguiente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSiguiente.ForeColor = Color.White;
            btnSiguiente.BackColor = Color.FromArgb(238, 143, 109);
            btnSiguiente.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSiguiente.Margin = new Padding(2);
            btnSiguiente.Padding = new Padding(8, 4, 8, 4);
            btnSiguiente.Click += (s, e) => CambiarPagina(paginaActual + 1);
            toolStripPaginacion.Items.Add(btnSiguiente);

            PanelToolStrip.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.Location = new Point(PanelToolStrip.Width - 260, 0);
            toolStripPaginacion.BringToFront();
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginas)
            {
                paginaActual = nuevaPagina;
                BuscarCursos();
            }
        }

        private void ActualizarInfoPaginacion()
        {
            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;

            if (totalRegistros == 0)
            {
                Lbl_Paginas.Text = "NO HAY CURSOS PARA MOSTRAR";
                return;
            }

            int inicioRango = (paginaActual - 1) * registrosPorPagina + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);
            Lbl_Paginas.Text = $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} CURSOS";
        }

        #endregion

        #region SeleccionYCampos

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;

            int courseId = Convert.ToInt32(Tabla.SelectedRows[0].Cells["CourseId"].Value);
            _cursoSeleccionado = cursosList.FirstOrDefault(c => c.CourseId == courseId);

            if (_cursoSeleccionado == null) return;

            Txt_Code.Text = _cursoSeleccionado.CourseCode;
            Txt_Name.Text = _cursoSeleccionado.CourseName;
            Txt_Description.Text = _cursoSeleccionado.Description;
            numericUpDownTotalCredits.Value = _cursoSeleccionado.Credits;
            numericUpDownTheoryHours.Value = _cursoSeleccionado.TheoryHours ?? 0;
            numericUpDownPracticeHours.Value = _cursoSeleccionado.PracticeHours ?? 0;
            numericUpDownLabHours.Value = _cursoSeleccionado.LabHours ?? 0;
            numericUpDownSessions.Value = _cursoSeleccionado.Sessions ?? 0;
            checkBoxCursoComun.Checked = _cursoSeleccionado.IsCommon;
            Txt_IsActive.Text = _cursoSeleccionado.IsActive ? "ACTIVO" : "INACTIVO";

            Btn_IsActive.Text = _cursoSeleccionado.IsActive ? "INACTIVAR" : "ACTIVAR";
        }

        private void LimpiarCampos()
        {
            Txt_Code.Text = "";
            Txt_Name.Text = "";
            Txt_Description.Text = "";
            numericUpDownTotalCredits.Value = 0;
            numericUpDownTheoryHours.Value = 0;
            numericUpDownPracticeHours.Value = 0;
            numericUpDownLabHours.Value = 0;
            numericUpDownSessions.Value = 0;
            checkBoxCursoComun.Checked = false;
            Txt_IsActive.Text = "";
            Btn_IsActive.Text = "ACTIVAR/INACTIVAR";
            _cursoSeleccionado = null;
            Tabla.ClearSelection();
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        #endregion

        #region Guardar

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            var curso = ConstruirCursoDesdeCampos();

            var confirmacion = MessageBox.Show(
                $"¿DESEA GUARDAR EL CURSO '{curso.CourseName}'?",
                "CONFIRMAR GUARDADO", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            int resultado = Ctrl_Courses.InsertarCurso(curso, UserData?.UserId ?? 0);

            if (resultado > 0)
            {
                MessageBox.Show("CURSO GUARDADO CORRECTAMENTE.", "ÉXITO",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                paginaActual = 1;
                BuscarCursos();
            }
            else
            {
                MessageBox.Show("NO SE PUDO GUARDAR EL CURSO. VERIFIQUE QUE EL CÓDIGO NO ESTÉ DUPLICADO.",
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Actualizar

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (_cursoSeleccionado == null)
            {
                MessageBox.Show("SELECCIONE UN CURSO DE LA TABLA PARA EDITAR.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos()) return;

            var curso = ConstruirCursoDesdeCampos();
            curso.CourseId = _cursoSeleccionado.CourseId;

            var confirmacion = MessageBox.Show(
                $"¿DESEA ACTUALIZAR EL CURSO '{curso.CourseName}'?",
                "CONFIRMAR ACTUALIZACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            int resultado = Ctrl_Courses.ActualizarCurso(curso, UserData?.UserId ?? 0);

            if (resultado > 0)
            {
                MessageBox.Show("CURSO ACTUALIZADO CORRECTAMENTE.", "ÉXITO",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                BuscarCursos();
            }
            else
            {
                MessageBox.Show("NO SE PUDO ACTUALIZAR EL CURSO.", "ERROR",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region CambiarEstado

        private void Btn_IsActive_Click(object sender, EventArgs e)
        {
            if (_cursoSeleccionado == null)
            {
                MessageBox.Show("SELECCIONE UN CURSO DE LA TABLA.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool estaActivo = _cursoSeleccionado.IsActive;
            int modo = estaActivo ? 1 : 2;
            string accion = estaActivo ? "INACTIVAR" : "ACTIVAR";

            var confirmacion = MessageBox.Show(
                $"¿DESEA {accion} EL CURSO '{_cursoSeleccionado.CourseName}'?",
                "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            int resultado = Ctrl_Courses.CambiarEstadoCurso(_cursoSeleccionado.CourseId, modo, UserData?.UserId ?? 0);

            if (resultado > 0)
            {
                MessageBox.Show($"CURSO {accion}DO CORRECTAMENTE.", "ÉXITO",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                BuscarCursos();
            }
            else
            {
                MessageBox.Show("NO SE PUDO CAMBIAR EL ESTADO DEL CURSO.", "ERROR",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Validacion

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(Txt_Code.Text))
            {
                MessageBox.Show("EL CÓDIGO ES OBLIGATORIO.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Txt_Name.Text))
            {
                MessageBox.Show("EL NOMBRE ES OBLIGATORIO.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (numericUpDownTotalCredits.Value == 0)
            {
                MessageBox.Show("LOS CRÉDITOS SON OBLIGATORIOS.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (numericUpDownSessions.Value == 0)
            {
                MessageBox.Show("LAS SESIONES SON OBLIGATORIAS.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private Mdl_Courses ConstruirCursoDesdeCampos()
        {
            return new Mdl_Courses
            {
                CourseCode = Txt_Code.Text.Trim(),
                CourseName = Txt_Name.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(Txt_Description.Text) ? null : Txt_Description.Text.Trim(),
                Credits = (int)numericUpDownTotalCredits.Value,
                // 0 = SIN INFORMACIÓN -> se guarda como NULL (dato no disponible actualmente)
                TheoryHours = numericUpDownTheoryHours.Value == 0 ? (int?)null : (int)numericUpDownTheoryHours.Value,
                PracticeHours = numericUpDownPracticeHours.Value == 0 ? (int?)null : (int)numericUpDownPracticeHours.Value,
                LabHours = numericUpDownLabHours.Value == 0 ? (int?)null : (int)numericUpDownLabHours.Value,
                Sessions = (int)numericUpDownSessions.Value,
                IsCommon = checkBoxCursoComun.Checked,
                IsActive = true
            };
        }

        #endregion

        #region Exportar

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            List<Mdl_Courses> listaExportar = Ctrl_Courses.ObtenerCursosParaExportar(_ultimoCampo, _ultimoValor, _ultimoEstado, _ultimoComun);

            if (listaExportar.Count == 0)
            {
                MessageBox.Show("NO HAY REGISTROS PARA EXPORTAR CON EL FILTRO ACTUAL.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Guardar listado de cursos",
                FileName = "CURSOS_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                Excel.Application excelApp = null;
                Excel.Workbook workbook = null;
                Excel.Worksheet worksheet = null;

                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    excelApp = new Excel.Application { Visible = false };
                    workbook = excelApp.Workbooks.Add();
                    worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "CURSOS";

                    string[] headers = {
                        "CODIGO", "NOMBRE", "DESCRIPCION", "CREDITOS", "SESIONES", "EN_COMUN", "ACTIVO"
                    };

                    for (int i = 0; i < headers.Length; i++)
                        worksheet.Cells[1, i + 1] = headers[i];

                    var headerRange = worksheet.Range["A1:G1"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Color = ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    int fila = 2;
                    foreach (var c in listaExportar)
                    {
                        worksheet.Cells[fila, 1] = c.CourseCode;
                        worksheet.Cells[fila, 2] = c.CourseName;
                        worksheet.Cells[fila, 3] = c.Description ?? "";
                        worksheet.Cells[fila, 4] = c.Credits.ToString();
                        worksheet.Cells[fila, 5] = c.Sessions?.ToString() ?? "";
                        worksheet.Cells[fila, 6] = c.IsCommon ? "SI" : "NO";
                        worksheet.Cells[fila, 7] = c.IsActive ? "SI" : "NO";
                        fila++;
                    }

                    worksheet.Columns.AutoFit();
                    workbook.SaveAs(dlg.FileName);

                    this.Cursor = Cursors.Default;
                    MessageBox.Show("LISTADO EXPORTADO CORRECTAMENTE.", "ÉXITO",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("ERROR AL EXPORTAR: " + ex.Message, "ERROR",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        #region Importar

        private void Btn_Import_Click(object sender, EventArgs e)
        {
            Frm_AcademicProcesses_CoursesProgram_Import frmImport = new Frm_AcademicProcesses_CoursesProgram_Import
            {
                UserData = this.UserData
            };

            DialogResult resultado = frmImport.ShowDialog(this);

            if (resultado == DialogResult.OK)
            {
                paginaActual = 1;
                BuscarCursos();
            }
        }

        #endregion
    }
}