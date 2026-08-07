using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading.Tasks;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_CareerProgram : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_Careers> carrerasList;
        private Mdl_Careers _carreraSeleccionada = null;

        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;

        private string _ultimoCampo = "TODOS";
        private string _ultimoValor = null;
        private string _ultimoEstado = "TODOS";

        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        #endregion

        public Frm_AcademicProcesses_CareerProgram()
        {
            InitializeComponent();
        }

        private async void Frm_AcademicProcesses_CareerProgram_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarFiltros();
                ConfigurarNumericos();
                Txt_IsActive.ReadOnly = true;
                ConfigurarTabla();
                CrearToolStripPaginacion();
                BuscarCarreras();

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

        private Ctrl_Security_Auth authController = new Ctrl_Security_Auth();
        private List<string> permisosUsuario = new List<string>();

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
            Btn_Save.Enabled = TienePermiso("ACADEMICPROCESSES_CAREERPROGRAM_CREATE");
            if (!Btn_Save.Enabled) { Btn_Save.BackColor = Color.FromArgb(200, 200, 200); Btn_Save.ForeColor = Color.Gray; Btn_Save.Cursor = Cursors.No; }

            Btn_Update.Enabled = TienePermiso("ACADEMICPROCESSES_CAREERPROGRAM_UPDATE");
            if (!Btn_Update.Enabled) { Btn_Update.BackColor = Color.FromArgb(200, 200, 200); Btn_Update.ForeColor = Color.Gray; Btn_Update.Cursor = Cursors.No; }

            Btn_Inactive.Enabled = TienePermiso("ACADEMICPROCESSES_CAREERPROGRAM_INACTIVE");
            if (!Btn_Inactive.Enabled) { Btn_Inactive.BackColor = Color.FromArgb(200, 200, 200); Btn_Inactive.ForeColor = Color.Gray; Btn_Inactive.Cursor = Cursors.No; }

            Btn_Import.Enabled = TienePermiso("ACADEMICPROCESSES_CAREERPROGRAM_IMPORT");
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
            Filtro1.Items.Add("DURACION");
            Filtro1.Items.Add("SEMESTRES");
            Filtro1.Items.Add("CREDITOS");
            Filtro1.SelectedIndex = 0;

            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS");
            Filtro2.Items.Add("ACTIVA");
            Filtro2.Items.Add("INACTIVA");
            Filtro2.SelectedIndex = 0;

            // Filtro3 reservado sin uso por ahora
            Filtro3.Items.Clear();
            Filtro3.Items.Add("N/A");
            Filtro3.SelectedIndex = 0;
            Filtro3.Enabled = false;

            Txt_ValorBuscado.Text = "";
        }

        #endregion

        #region ConfigurarNumericos

        private void ConfigurarNumericos()
        {
            numericUpDownDurationYears.Minimum = 0;
            numericUpDownDurationYears.Maximum = 10;

            numericUpDownTotalSemesters.Minimum = 0;
            numericUpDownTotalSemesters.Maximum = 20;

            // TotalCredits: 0 = "SIN INFORMACIÓN" (se guarda como NULL)
            numericUpDownTotalCredits.Minimum = 0;
            numericUpDownTotalCredits.Maximum = 500;
        }

        #endregion

        #region ConfigurarTabla

        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;

            Tabla.Columns.Add("CareerId", "ID");
            Tabla.Columns.Add("CareerCode", "CÓDIGO");
            Tabla.Columns.Add("CareerName", "NOMBRE");
            Tabla.Columns.Add("DurationYears", "DURACIÓN (AÑOS)");
            Tabla.Columns.Add("TotalSemesters", "SEMESTRES");
            Tabla.Columns.Add("TotalCredits", "CRÉDITOS");
            Tabla.Columns.Add("IsActive", "ESTADO");
            Tabla.Columns.Add("CreatedDate", "FECHA CREACIÓN");

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
            Tabla.Columns["CareerId"].Visible = false;

            Tabla.Columns["CareerName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CareerName"].FillWeight = 30;
            Tabla.Columns["CareerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CareerCode"].FillWeight = 15;
            Tabla.Columns["DurationYears"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["DurationYears"].FillWeight = 15;
            Tabla.Columns["TotalSemesters"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["TotalSemesters"].FillWeight = 15;
            Tabla.Columns["TotalCredits"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["TotalCredits"].FillWeight = 15;
            Tabla.Columns["IsActive"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["IsActive"].FillWeight = 15;
            Tabla.Columns["CreatedDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CreatedDate"].FillWeight = 20;

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void MostrarCarrerasEnTabla()
        {
            Tabla.Rows.Clear();

            foreach (var carrera in carrerasList)
            {
                Tabla.Rows.Add(
                    carrera.CareerId,
                    carrera.CareerCode,
                    carrera.CareerName,
                    carrera.DurationYears.HasValue ? carrera.DurationYears.ToString() : "N/A",
                    carrera.TotalSemesters.HasValue ? carrera.TotalSemesters.ToString() : "N/A",
                    carrera.TotalCredits.HasValue ? carrera.TotalCredits.ToString() : "N/A",
                    carrera.IsActive ? "ACTIVA" : "INACTIVA",
                    carrera.CreatedDate.ToString("dd/MM/yyyy")
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

            if (campo != "TODOS" && string.IsNullOrWhiteSpace(valor))
            {
                MessageBox.Show("INGRESE UN VALOR PARA BUSCAR.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if ((campo == "DURACION" || campo == "SEMESTRES" || campo == "CREDITOS")
                && !int.TryParse(valor, out _))
            {
                MessageBox.Show("PARA ESTE CAMPO EL VALOR DEBE SER NUMÉRICO.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _ultimoCampo = campo;
            _ultimoValor = campo == "TODOS" ? null : valor;
            _ultimoEstado = estado;

            paginaActual = 1;
            BuscarCarreras();
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Txt_ValorBuscado.Text = "";

            _ultimoCampo = "TODOS";
            _ultimoValor = null;
            _ultimoEstado = "TODOS";

            paginaActual = 1;
            BuscarCarreras();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }

        private void BuscarCarreras()
        {
            try
            {
                carrerasList = Ctrl_Careers.ObtenerCarreras(
                    _ultimoCampo, _ultimoValor, _ultimoEstado,
                    paginaActual, registrosPorPagina, out totalRegistros);

                MostrarCarrerasEnTabla();

                totalPaginas = totalRegistros == 0 ? 0 : (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
                ActualizarInfoPaginacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL BUSCAR CARRERAS: " + ex.Message, "ERROR SECRON",
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

            // Se agrega DENTRO de PanelToolStrip (mismo panel que Lbl_Paginas), no en Panel_Derecho
            PanelToolStrip.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.Location = new Point(PanelToolStrip.Width - 260, 0);
            toolStripPaginacion.BringToFront();
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginas)
            {
                paginaActual = nuevaPagina;
                BuscarCarreras();
            }
        }

        private void ActualizarInfoPaginacion()
        {
            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;

            if (totalRegistros == 0)
            {
                Lbl_Paginas.Text = "NO HAY CARRERAS PARA MOSTRAR";
                return;
            }

            int inicioRango = (paginaActual - 1) * registrosPorPagina + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);
            Lbl_Paginas.Text = $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} CARRERAS";
        }

        #endregion

        #region SeleccionYCampos

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;

            int careerId = Convert.ToInt32(Tabla.SelectedRows[0].Cells["CareerId"].Value);
            _carreraSeleccionada = carrerasList.FirstOrDefault(c => c.CareerId == careerId);

            if (_carreraSeleccionada == null) return;

            Txt_Code.Text = _carreraSeleccionada.CareerCode;
            Txt_Name.Text = _carreraSeleccionada.CareerName;
            Txt_Description.Text = _carreraSeleccionada.Description;
            numericUpDownDurationYears.Value = _carreraSeleccionada.DurationYears ?? 0;
            numericUpDownTotalSemesters.Value = _carreraSeleccionada.TotalSemesters ?? 0;
            numericUpDownTotalCredits.Value = _carreraSeleccionada.TotalCredits ?? 0;
            Txt_IsActive.Text = _carreraSeleccionada.IsActive ? "ACTIVO" : "INACTIVO";

            Btn_Inactive.Text = _carreraSeleccionada.IsActive ? "INACTIVAR" : "ACTIVAR";
        }

        private void LimpiarCampos()
        {
            Txt_Code.Text = "";
            Txt_Name.Text = "";
            Txt_Description.Text = "";
            numericUpDownDurationYears.Value = 0;
            numericUpDownTotalSemesters.Value = 0;
            numericUpDownTotalCredits.Value = 0;
            Txt_IsActive.Text = "";
            Btn_Inactive.Text = "INACTIVAR";
            _carreraSeleccionada = null;
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
            if (_carreraSeleccionada != null)
            {
                MessageBox.Show("YA HAY UNA CARRERA SELECCIONADA. USE 'EDITAR' O 'LIMPIAR' PARA CREAR UNA NUEVA.",
                                "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos()) return;

            var carrera = ConstruirCarreraDesdeCampos();

            var confirmacion = MessageBox.Show(
                $"¿DESEA GUARDAR LA CARRERA '{carrera.CareerName}'?",
                "CONFIRMAR GUARDADO", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            int resultado = Ctrl_Careers.InsertarCarrera(carrera, UserData?.UserId ?? 0);

            if (resultado > 0)
            {
                MessageBox.Show("CARRERA GUARDADA CORRECTAMENTE.", "ÉXITO",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                paginaActual = 1;
                BuscarCarreras();
            }
            else
            {
                MessageBox.Show("NO SE PUDO GUARDAR LA CARRERA. VERIFIQUE QUE EL CÓDIGO NO ESTÉ DUPLICADO.",
                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Actualizar

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (_carreraSeleccionada == null)
            {
                MessageBox.Show("SELECCIONE UNA CARRERA DE LA TABLA PARA EDITAR.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos()) return;

            var carrera = ConstruirCarreraDesdeCampos();
            carrera.CareerId = _carreraSeleccionada.CareerId;

            var confirmacion = MessageBox.Show(
                $"¿DESEA ACTUALIZAR LA CARRERA '{carrera.CareerName}'?",
                "CONFIRMAR ACTUALIZACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            int resultado = Ctrl_Careers.ActualizarCarrera(carrera, UserData?.UserId ?? 0);

            if (resultado > 0)
            {
                MessageBox.Show("CARRERA ACTUALIZADA CORRECTAMENTE.", "ÉXITO",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                BuscarCarreras();
            }
            else
            {
                MessageBox.Show("NO SE PUDO ACTUALIZAR LA CARRERA.", "ERROR",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region CambiarEstado

        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            if (_carreraSeleccionada == null)
            {
                MessageBox.Show("SELECCIONE UNA CARRERA DE LA TABLA.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool estaActiva = _carreraSeleccionada.IsActive;
            int modo = estaActiva ? 1 : 2; // 1 = Inactivar, 2 = Reactivar
            string accion = estaActiva ? "INACTIVAR" : "ACTIVAR";

            var confirmacion = MessageBox.Show(
                $"¿DESEA {accion} LA CARRERA '{_carreraSeleccionada.CareerName}'?",
                "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            int resultado = Ctrl_Careers.CambiarEstadoCarrera(_carreraSeleccionada.CareerId, modo, UserData?.UserId ?? 0);

            if (resultado > 0)
            {
                MessageBox.Show($"CARRERA {accion}DA CORRECTAMENTE.", "ÉXITO",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                BuscarCarreras();
            }
            else
            {
                MessageBox.Show("NO SE PUDO CAMBIAR EL ESTADO DE LA CARRERA.", "ERROR",
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

            if (numericUpDownDurationYears.Value == 0)
            {
                MessageBox.Show("LA DURACIÓN EN AÑOS ES OBLIGATORIA.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (numericUpDownTotalSemesters.Value == 0)
            {
                MessageBox.Show("EL TOTAL DE SEMESTRES ES OBLIGATORIO.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private Mdl_Careers ConstruirCarreraDesdeCampos()
        {
            return new Mdl_Careers
            {
                CareerCode = Txt_Code.Text.Trim(),
                CareerName = Txt_Name.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(Txt_Description.Text) ? null : Txt_Description.Text.Trim(),
                DurationYears = (int)numericUpDownDurationYears.Value,
                TotalSemesters = (int)numericUpDownTotalSemesters.Value,
                TotalCredits = numericUpDownTotalCredits.Value == 0 ? (int?)null : (int)numericUpDownTotalCredits.Value,
                IsActive = true
            };
        }

        #endregion

        #region Exportar

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            List<Mdl_Careers> listaExportar = Ctrl_Careers.ObtenerCarrerasParaExportar(_ultimoCampo, _ultimoValor, _ultimoEstado);

            if (listaExportar.Count == 0)
            {
                MessageBox.Show("NO HAY REGISTROS PARA EXPORTAR CON EL FILTRO ACTUAL.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Guardar listado de carreras",
                FileName = "CARRERAS_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx"
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
                    worksheet.Name = "CARRERAS";

                    string[] headers = {
                        "CODIGO", "NOMBRE", "DESCRIPCION", "DURACION_ANIOS",
                        "TOTAL_SEMESTRES", "TOTAL_CREDITOS", "ACTIVA"
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
                        worksheet.Cells[fila, 1] = c.CareerCode;
                        worksheet.Cells[fila, 2] = c.CareerName;
                        worksheet.Cells[fila, 3] = c.Description ?? "";
                        worksheet.Cells[fila, 4] = c.DurationYears?.ToString() ?? "";
                        worksheet.Cells[fila, 5] = c.TotalSemesters?.ToString() ?? "";
                        worksheet.Cells[fila, 6] = c.TotalCredits?.ToString() ?? "";
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
            Frm_AcademicProcesses_CareerProgram_Import frmImport = new Frm_AcademicProcesses_CareerProgram_Import
            {
                UserData = this.UserData
            };

            DialogResult resultado = frmImport.ShowDialog(this);

            if (resultado == DialogResult.OK)
            {
                paginaActual = 1;
                BuscarCarreras();
            }
        }

        #endregion
    }
}