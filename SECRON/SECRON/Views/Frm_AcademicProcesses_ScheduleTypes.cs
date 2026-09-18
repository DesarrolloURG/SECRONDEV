using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SECRON.Controllers;
using SECRON.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_ScheduleTypes : Form
    {
        #region Campos

        public Mdl_Security_UserInfo UserData { get; set; }

        private Ctrl_Security_Auth authController = new Ctrl_Security_Auth();
        private List<string> permisosUsuario = new List<string>();

        private int _scheduleTypeIdSeleccionado = 0;
        private int _paginaActual = 1;
        private int _totalPaginas = 1;
        private const int TAMANIO_PAGINA = 100;

        private string _ultimoCampo = "TODOS";
        private string _ultimoValor = null;
        private string _ultimoTimeShift = "TODOS";
        private string _ultimoEstado = "TODOS";

        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        #endregion

        #region Constructor

        public Frm_AcademicProcesses_ScheduleTypes()
        {
            InitializeComponent();

            this.Load -= Frm_AcademicProcesses_ScheduleTypes_Load;
            this.Load += Frm_AcademicProcesses_ScheduleTypes_Load;

            Btn_Save.Click -= Btn_Save_Click;
            Btn_Save.Click += Btn_Save_Click;

            Btn_Update.Click -= Btn_Update_Click;
            Btn_Update.Click += Btn_Update_Click;

            Btn_IsActive.Click -= Btn_IsActive_Click;
            Btn_IsActive.Click += Btn_IsActive_Click;

            Btn_Clear.Click -= Btn_Clear_Click;
            Btn_Clear.Click += Btn_Clear_Click;

            Btn_Search.Click -= Btn_Search_Click;
            Btn_Search.Click += Btn_Search_Click;

            Btn_CleanSearch.Click -= Btn_CleanSearch_Click;
            Btn_CleanSearch.Click += Btn_CleanSearch_Click;

            Btn_Export.Click -= Btn_Export_Click;
            Btn_Export.Click += Btn_Export_Click;
        }

        #endregion

        #region Load

        private async void Frm_AcademicProcesses_ScheduleTypes_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarFiltros();
                ConfigurarTabla();
                CrearToolStripPaginacion();
                LimpiarCampos();
                BuscarScheduleTypes();

                if (UserData != null)
                {
                    // El 2do parámetro (roleId) ya no se usa dentro de ObtenerPermisosUsuarioAsync
                    // (los roles se resuelven internamente vía UserRoles) — se conserva solo por compatibilidad de firma.
                    await CargarPermisosUsuario(UserData.UserId, 0);
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

        #endregion

        #region Filtros

        private void ConfigurarFiltros()
        {
            Filtro1.Items.Clear();
            Filtro1.Items.AddRange(new object[] { "TODOS", "CÓDIGO", "NOMBRE" });
            Filtro1.SelectedIndex = 0;

            Filtro2.Items.Clear();
            Filtro2.Items.AddRange(new object[] { "TODOS", "MAÑANA", "TARDE", "NOCHE", "MIXTO" });
            Filtro2.SelectedIndex = 0;

            Filtro3.Items.Clear();
            Filtro3.Items.AddRange(new object[] { "TODOS", "ACTIVOS", "INACTIVOS" });
            Filtro3.SelectedIndex = 0;

            ComboBox_TimeShift.Items.Clear();
            ComboBox_TimeShift.Items.AddRange(new object[] { "MAÑANA", "TARDE", "NOCHE", "MIXTO" });
        }

        #endregion

        #region Tabla

        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;

            // ===== COLUMNA ESTADO PRIMERO =====
            var colEstadoRegistro = new DataGridViewTextBoxColumn();
            colEstadoRegistro.Name = "ColEstadoRegistro";
            colEstadoRegistro.HeaderText = "ESTADO";
            colEstadoRegistro.Width = 90;
            colEstadoRegistro.ReadOnly = true;
            colEstadoRegistro.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.Columns.Add(colEstadoRegistro);

            Tabla.Columns.Add("ScheduleTypeId", "ID");
            Tabla.Columns.Add("ScheduleTypeCode", "CÓDIGO");
            Tabla.Columns.Add("ScheduleTypeName", "NOMBRE");
            Tabla.Columns.Add("Description", "DESCRIPCIÓN");
            Tabla.Columns.Add("Dias", "DÍAS QUE ABARCA");
            Tabla.Columns.Add("TimeShift", "JORNADA");
            Tabla.Columns.Add("CreatedDate", "FECHA CREACIÓN");

            Tabla.Columns["ScheduleTypeId"].Visible = false;

            // ===== Reparto proporcional del ancho total (sin espacio gris sobrante) =====
            Tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Tabla.Columns["ColEstadoRegistro"].FillWeight = 10;
            Tabla.Columns["ScheduleTypeCode"].FillWeight = 12;
            Tabla.Columns["ScheduleTypeName"].FillWeight = 18;
            Tabla.Columns["Description"].FillWeight = 25;
            Tabla.Columns["Dias"].FillWeight = 15;
            Tabla.Columns["TimeShift"].FillWeight = 10;
            Tabla.Columns["CreatedDate"].FillWeight = 10;

            // ===== Selección de fila completa, no celda por celda =====
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;

            Tabla.CellFormatting -= Tabla_CellFormatting_Estado;
            Tabla.CellFormatting += Tabla_CellFormatting_Estado;

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        // Colorea la columna ESTADO: ACTIVA en verde, INACTIVA en rojo, ambos en negrita
        private void Tabla_CellFormatting_Estado(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (Tabla.Columns[e.ColumnIndex].Name != "ColEstadoRegistro") return;
            if (e.Value == null) return;

            bool activa = e.Value.ToString() == "ACTIVA";
            e.CellStyle.ForeColor = activa ? Color.FromArgb(0, 128, 0) : Color.FromArgb(200, 0, 0);
            e.CellStyle.Font = new Font(Tabla.Font, FontStyle.Bold);
        }

        private string ObtenerTextoDias(Mdl_ScheduleType st)
        {
            var dias = new List<string>();
            if (st.IncludesMonday) dias.Add("LUN");
            if (st.IncludesTuesday) dias.Add("MAR");
            if (st.IncludesWednesday) dias.Add("MIÉ");
            if (st.IncludesThursday) dias.Add("JUE");
            if (st.IncludesFriday) dias.Add("VIE");
            if (st.IncludesSaturday) dias.Add("SÁB");
            if (st.IncludesSunday) dias.Add("DOM");
            return dias.Count > 0 ? string.Join(", ", dias) : "-";
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.CurrentRow == null || Tabla.CurrentRow.Index < 0) return;

            try
            {
                _scheduleTypeIdSeleccionado = Convert.ToInt32(Tabla.CurrentRow.Cells["ScheduleTypeId"].Value);

                // Buscamos el registro completo (la fila del grid no trae los 7 booleanos individuales)
                int totalRows;
                var lista = Ctrl_ScheduleTypes.ObtenerScheduleTypes("TODOS", null, "TODOS", "TODOS", 1, 0, out totalRows);
                var seleccionado = lista.FirstOrDefault(x => x.ScheduleTypeId == _scheduleTypeIdSeleccionado);
                if (seleccionado == null) return;

                Txt_Code.Text = seleccionado.ScheduleTypeCode;
                Txt_Name.Text = seleccionado.ScheduleTypeName;
                Txt_Descripcion.Text = seleccionado.Description;
                ComboBox_TimeShift.SelectedItem = seleccionado.TimeShift;

                MarcarDiasEnCheckedListBox(seleccionado);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CARGAR REGISTRO SELECCIONADO: " + ex.Message, "ERROR SECRON",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region CheckedListBox de días (orden fijo: Lun, Mar, Mié, Jue, Vie, Sáb, Dom)

        private void MarcarDiasEnCheckedListBox(Mdl_ScheduleType st)
        {
            checkedListBox1.SetItemChecked(0, st.IncludesMonday);
            checkedListBox1.SetItemChecked(1, st.IncludesTuesday);
            checkedListBox1.SetItemChecked(2, st.IncludesWednesday);
            checkedListBox1.SetItemChecked(3, st.IncludesThursday);
            checkedListBox1.SetItemChecked(4, st.IncludesFriday);
            checkedListBox1.SetItemChecked(5, st.IncludesSaturday);
            checkedListBox1.SetItemChecked(6, st.IncludesSunday);
        }

        private void LimpiarCheckedListBoxDias()
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, false);
        }

        #endregion

        #region Búsqueda y listado (paginado)

        private void BuscarScheduleTypes()
        {
            _paginaActual = 1;

            _ultimoCampo = Filtro1.SelectedItem?.ToString() == "CÓDIGO" ? "CODIGO"
                          : Filtro1.SelectedItem?.ToString() == "NOMBRE" ? "NOMBRE"
                          : "TODOS";
            _ultimoValor = string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text) ? null : Txt_ValorBuscado.Text.Trim();
            _ultimoTimeShift = Filtro2.SelectedItem?.ToString() ?? "TODOS";
            _ultimoEstado = Filtro3.SelectedItem?.ToString() ?? "TODOS";

            MostrarScheduleTypesEnTabla();
        }

        private void MostrarScheduleTypesEnTabla()
        {
            int totalRows;
            var lista = Ctrl_ScheduleTypes.ObtenerScheduleTypes(_ultimoCampo, _ultimoValor, _ultimoTimeShift, _ultimoEstado,
                                                                 _paginaActual, TAMANIO_PAGINA, out totalRows);

            Tabla.Rows.Clear();

            foreach (var st in lista)
            {
                int fila = Tabla.Rows.Add();
                Tabla.Rows[fila].Cells["ColEstadoRegistro"].Value = st.IsActive ? "ACTIVA" : "INACTIVA";
                Tabla.Rows[fila].Cells["ScheduleTypeId"].Value = st.ScheduleTypeId;
                Tabla.Rows[fila].Cells["ScheduleTypeCode"].Value = st.ScheduleTypeCode;
                Tabla.Rows[fila].Cells["ScheduleTypeName"].Value = st.ScheduleTypeName;
                Tabla.Rows[fila].Cells["Description"].Value = st.Description;
                Tabla.Rows[fila].Cells["Dias"].Value = ObtenerTextoDias(st);
                Tabla.Rows[fila].Cells["TimeShift"].Value = st.TimeShift;
                Tabla.Rows[fila].Cells["CreatedDate"].Value = st.CreatedDate.ToString("dd/MM/yyyy");
            }

            _totalPaginas = totalRows == 0 ? 1 : (int)Math.Ceiling(totalRows / (double)TAMANIO_PAGINA);

            int desde = totalRows == 0 ? 0 : ((_paginaActual - 1) * TAMANIO_PAGINA) + 1;
            int hasta = Math.Min(_paginaActual * TAMANIO_PAGINA, totalRows);
            Lbl_Paginas.Text = $"MOSTRANDO {desde}-{hasta} DE {totalRows}";

            if (btnAnterior != null) btnAnterior.Enabled = _paginaActual > 1;
            if (btnSiguiente != null) btnSiguiente.Enabled = _paginaActual < _totalPaginas;
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1 || nuevaPagina > _totalPaginas) return;
            _paginaActual = nuevaPagina;
            MostrarScheduleTypesEnTabla();
        }

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
            btnAnterior.Click += (s, e) => CambiarPagina(_paginaActual - 1);
            toolStripPaginacion.Items.Add(btnAnterior);

            btnSiguiente = new ToolStripButton();
            btnSiguiente.Text = "Siguiente ❯";
            btnSiguiente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSiguiente.ForeColor = Color.White;
            btnSiguiente.BackColor = Color.FromArgb(238, 143, 109);
            btnSiguiente.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSiguiente.Margin = new Padding(2);
            btnSiguiente.Padding = new Padding(8, 4, 8, 4);
            btnSiguiente.Click += (s, e) => CambiarPagina(_paginaActual + 1);
            toolStripPaginacion.Items.Add(btnSiguiente);

            PanelToolStrip.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.Location = new Point(PanelToolStrip.Width - 260, 0);
            toolStripPaginacion.BringToFront();
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            BuscarScheduleTypes();
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "";
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 0;
            BuscarScheduleTypes();
        }

        #endregion

        #region CRUD

        private void LimpiarCampos()
        {
            _scheduleTypeIdSeleccionado = 0;
            Txt_Code.Text = "";
            Txt_Name.Text = "";
            Txt_Descripcion.Text = "";
            ComboBox_TimeShift.SelectedIndex = -1;
            LimpiarCheckedListBoxDias();
            Tabla.ClearSelection();
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

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
            if (string.IsNullOrWhiteSpace(Txt_Descripcion.Text))
            {
                MessageBox.Show("LA DESCRIPCIÓN ES OBLIGATORIA.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("DEBE SELECCIONAR AL MENOS UN DÍA DE LA SEMANA.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (ComboBox_TimeShift.SelectedIndex == -1)
            {
                MessageBox.Show("DEBE SELECCIONAR LA JORNADA.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private Mdl_ScheduleType ConstruirModeloDesdeCampos()
        {
            return new Mdl_ScheduleType
            {
                ScheduleTypeId = _scheduleTypeIdSeleccionado,
                ScheduleTypeCode = Txt_Code.Text.Trim(),
                ScheduleTypeName = Txt_Name.Text.Trim(),
                Description = Txt_Descripcion.Text.Trim(),
                IncludesMonday = checkedListBox1.GetItemChecked(0),
                IncludesTuesday = checkedListBox1.GetItemChecked(1),
                IncludesWednesday = checkedListBox1.GetItemChecked(2),
                IncludesThursday = checkedListBox1.GetItemChecked(3),
                IncludesFriday = checkedListBox1.GetItemChecked(4),
                IncludesSaturday = checkedListBox1.GetItemChecked(5),
                IncludesSunday = checkedListBox1.GetItemChecked(6),
                TimeShift = ComboBox_TimeShift.SelectedItem.ToString()
            };
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            var scheduleType = ConstruirModeloDesdeCampos();
            int usuarioId = UserData?.UserId ?? 0;

            int resultado = Ctrl_ScheduleTypes.InsertarScheduleType(scheduleType, usuarioId);

            if (resultado > 0)
            {
                MessageBox.Show("JORNADA REGISTRADA CORRECTAMENTE.", "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                BuscarScheduleTypes();
            }
            else
            {
                MessageBox.Show("NO SE PUDO REGISTRAR LA JORNADA. VERIFIQUE QUE EL CÓDIGO NO ESTÉ DUPLICADO.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (_scheduleTypeIdSeleccionado <= 0)
            {
                MessageBox.Show("SELECCIONE UNA JORNADA DEL LISTADO PARA EDITAR.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ValidarCampos()) return;

            var scheduleType = ConstruirModeloDesdeCampos();
            int usuarioId = UserData?.UserId ?? 0;

            int resultado = Ctrl_ScheduleTypes.ActualizarScheduleType(scheduleType, usuarioId);

            if (resultado > 0)
            {
                MessageBox.Show("JORNADA ACTUALIZADA CORRECTAMENTE.", "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                BuscarScheduleTypes();
            }
            else
            {
                MessageBox.Show("NO SE PUDO ACTUALIZAR LA JORNADA.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Btn_IsActive_Click(object sender, EventArgs e)
        {
            if (_scheduleTypeIdSeleccionado <= 0)
            {
                MessageBox.Show("SELECCIONE UNA JORNADA DEL LISTADO.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string estadoActual = Tabla.CurrentRow?.Cells["ColEstadoRegistro"].Value?.ToString();
            bool estaActiva = estadoActual == "ACTIVA";
            int modo = estaActiva ? 1 : 2; // 1 = Inactivar, 2 = Reactivar
            string accion = estaActiva ? "INACTIVAR" : "REACTIVAR";

            var confirmacion = MessageBox.Show($"¿DESEA {accion} ESTA JORNADA?", "CONFIRMAR",
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            int usuarioId = UserData?.UserId ?? 0;
            int resultado = Ctrl_ScheduleTypes.CambiarEstadoScheduleType(_scheduleTypeIdSeleccionado, modo, usuarioId);

            if (resultado > 0)
            {
                MessageBox.Show($"JORNADA {(estaActiva ? "INACTIVADA" : "REACTIVADA")} CORRECTAMENTE.", "ÉXITO",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                BuscarScheduleTypes();
            }
            else
            {
                MessageBox.Show("NO SE PUDO CAMBIAR EL ESTADO DE LA JORNADA.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region Sistema de Permisos

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
            Btn_Save.Enabled = TienePermiso("ACADEMICPROCESSES_SCHEDULETYPES_CREATE");
            if (!Btn_Save.Enabled) { Btn_Save.BackColor = Color.FromArgb(200, 200, 200); Btn_Save.ForeColor = Color.Gray; Btn_Save.Cursor = Cursors.No; }

            Btn_Update.Enabled = TienePermiso("ACADEMICPROCESSES_SCHEDULETYPES_UPDATE");
            if (!Btn_Update.Enabled) { Btn_Update.BackColor = Color.FromArgb(200, 200, 200); Btn_Update.ForeColor = Color.Gray; Btn_Update.Cursor = Cursors.No; }

            Btn_IsActive.Enabled = TienePermiso("ACADEMICPROCESSES_SCHEDULETYPES_INACTIVE");
            if (!Btn_IsActive.Enabled) { Btn_IsActive.BackColor = Color.FromArgb(200, 200, 200); Btn_IsActive.ForeColor = Color.Gray; Btn_IsActive.Cursor = Cursors.No; }
        }

        #endregion

        #region Exportar

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            List<Mdl_ScheduleType> listaExportar = Ctrl_ScheduleTypes.ObtenerScheduleTypesParaExportar(
                _ultimoCampo, _ultimoValor, _ultimoTimeShift, _ultimoEstado);

            if (listaExportar.Count == 0)
            {
                MessageBox.Show("NO HAY REGISTROS PARA EXPORTAR CON EL FILTRO ACTUAL.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Guardar listado de jornadas de estudio",
                FileName = "JORNADAS_ESTUDIO_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx"
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
                    worksheet.Name = "JORNADAS";

                    string[] headers = {
                        "CODIGO", "NOMBRE", "DESCRIPCION", "DIAS", "JORNADA", "ACTIVA"
                    };

                    for (int i = 0; i < headers.Length; i++)
                        worksheet.Cells[1, i + 1] = headers[i];

                    var headerRange = worksheet.Range["A1:F1"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Color = ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    int fila = 2;
                    foreach (var st in listaExportar)
                    {
                        worksheet.Cells[fila, 1] = st.ScheduleTypeCode;
                        worksheet.Cells[fila, 2] = st.ScheduleTypeName;
                        worksheet.Cells[fila, 3] = st.Description ?? "";
                        worksheet.Cells[fila, 4] = ObtenerTextoDias(st);
                        worksheet.Cells[fila, 5] = st.TimeShift ?? "";
                        worksheet.Cells[fila, 6] = st.IsActive ? "SI" : "NO";
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
                    MessageBox.Show("ERROR AL EXPORTAR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                    if (workbook != null) { workbook.Close(false); Marshal.ReleaseComObject(workbook); }
                    if (excelApp != null) { excelApp.Quit(); Marshal.ReleaseComObject(excelApp); }
                }
            }
        }

        #endregion
    }
}