using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_CareerPensum : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_CareerPensums> _pensumsList;
        private Mdl_CareerPensums _pensumSeleccionado = null;

        private HashSet<string> permisosUsuario = new HashSet<string>();

        #endregion

        public Frm_AcademicProcesses_CareerPensum()
        {
            InitializeComponent();
        }

        private async void Frm_AcademicProcesses_CareerPensum_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarCombos();
                ConfigurarFiltros();
                ConfigurarTabla();
                CargarPensums();

                if (UserData != null)
                {
                    await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
                    ConfigurarControlesPorPermisos();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("Error al cargar el formulario: " + ex.Message,
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region SistemaDePermisos

        private Ctrl_Security_Auth authController;

        private async System.Threading.Tasks.Task CargarPermisosUsuario(int userId, int roleId)
        {
            try
            {
                authController = new Ctrl_Security_Auth();
                var permisos = await authController.ObtenerPermisosUsuarioAsync(userId, roleId);
                permisosUsuario = permisos != null
                    ? new HashSet<string>(permisos, StringComparer.OrdinalIgnoreCase)
                    : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                permisosUsuario = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                MessageBox.Show($"ERROR AL CARGAR PERMISOS: {ex.Message}", "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected bool TienePermiso(string permissionCode)
        {
            return !string.IsNullOrWhiteSpace(permissionCode) &&
                   permisosUsuario != null &&
                   permisosUsuario.Contains(permissionCode);
        }

        protected void AplicarEstadoBotonPorPermiso(Button boton, string permissionCode)
        {
            if (boton == null) return;
            bool habilitado = TienePermiso(permissionCode);
            boton.Enabled = habilitado;
            if (habilitado)
            { boton.UseVisualStyleBackColor = true; boton.ForeColor = Color.Black; boton.Cursor = Cursors.Default; }
            else
            { boton.BackColor = Color.FromArgb(200, 200, 200); boton.ForeColor = Color.Gray; boton.Cursor = Cursors.No; }
        }

        protected void ConfigurarControlesPorPermisos()
        {
            AplicarEstadoBotonPorPermiso(Btn_Save, "ACADEMICPROCESSES_PENSUM_CREATE");
            AplicarEstadoBotonPorPermiso(Btn_Update, "ACADEMICPROCESSES_PENSUM_UPDATE");
            AplicarEstadoBotonPorPermiso(Btn_IsActive, "ACADEMICPROCESSES_PENSUM_INACTIVE");
            AplicarEstadoBotonPorPermiso(Btn_Export, "ACADEMICPROCESSES_PENSUM_EXPORT");
        }

        #endregion SistemaDePermisos

        #region Configuración de combos y filtros

        private void ConfigurarCombos()
        {
            var carreras = Ctrl_Careers.ObtenerCarrerasActivas();
            ComboBox_CareerId.DisplayMember = "Value";
            ComboBox_CareerId.ValueMember = "Key";
            ComboBox_CareerId.DataSource = carreras;
            ComboBox_CareerId.SelectedIndex = -1;

            Cbx_EstadoPensum.Items.Clear();
            Cbx_EstadoPensum.Items.Add("NO VIGENTE");
            Cbx_EstadoPensum.Items.Add("VIGENTE");
            Cbx_EstadoPensum.SelectedIndex = 0; // NO VIGENTE por defecto (obligatorio, coincide con default de BD)
        }

        private void ConfigurarFiltros()
        {
            var carreras = new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(0, "TODAS LAS CARRERAS") };
            carreras.AddRange(Ctrl_Careers.ObtenerCarrerasActivas());
            Filtro1.DisplayMember = "Value";
            Filtro1.ValueMember = "Key";
            Filtro1.DataSource = carreras;
            Filtro1.SelectedIndex = 0;

            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS");
            Filtro2.Items.Add("ACTIVO");
            Filtro2.Items.Add("INACTIVO");
            Filtro2.SelectedIndex = 0;
        }

        #endregion

        #region Grid

        private void Tabla_CellFormatting_Estado(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (Tabla.Columns[e.ColumnIndex].Name != "ColEstadoRegistro") return;
            if (e.Value == null) return;

            bool activo = e.Value.ToString() == "ACTIVO";
            e.CellStyle.ForeColor = activo ? Color.FromArgb(0, 128, 0) : Color.FromArgb(200, 0, 0);
            e.CellStyle.Font = new Font(Tabla.Font, FontStyle.Bold);
        }

        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;

            var colEstadoRegistro = new DataGridViewTextBoxColumn();
            colEstadoRegistro.Name = "ColEstadoRegistro";
            colEstadoRegistro.HeaderText = "ESTADO";
            colEstadoRegistro.Width = 90;
            colEstadoRegistro.ReadOnly = true;
            colEstadoRegistro.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.Columns.Add(colEstadoRegistro);

            Tabla.Columns.Add("CareerPensumId", "ID");
            Tabla.Columns.Add("PensumCode", "CÓDIGO");
            Tabla.Columns.Add("PensumName", "NOMBRE");
            Tabla.Columns.Add("CareerName", "CARRERA");
            Tabla.Columns.Add("IsCurrentText", "VIGENTE");

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
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            Tabla.Columns["CareerPensumId"].Visible = false;

            Tabla.Columns["PensumCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["PensumCode"].FillWeight = 15;
            Tabla.Columns["PensumName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["PensumName"].FillWeight = 35;
            Tabla.Columns["CareerName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CareerName"].FillWeight = 30;
            Tabla.Columns["IsCurrentText"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["IsCurrentText"].FillWeight = 15;

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;

            Tabla.CellFormatting -= Tabla_CellFormatting_Estado;
            Tabla.CellFormatting += Tabla_CellFormatting_Estado;
        }

        private void MostrarPensumsEnTabla(List<Mdl_CareerPensums> lista)
        {
            Tabla.Rows.Clear();

            foreach (var p in lista)
            {
                Tabla.Rows.Add(
                    p.IsActive ? "ACTIVO" : "INACTIVO",
                    p.CareerPensumId,
                    p.PensumCode,
                    p.PensumName,
                    p.CareerName,
                    p.IsCurrent ? "VIGENTE" : "NO VIGENTE"
                );
            }
        }

        #endregion

        #region Búsqueda

        private void CargarPensums()
        {
            int? careerId = (Filtro1.SelectedValue is int cid && cid > 0) ? cid : (int?)null;

            bool? isActive = null;
            if (Filtro2.SelectedItem?.ToString() == "ACTIVO") isActive = true;
            else if (Filtro2.SelectedItem?.ToString() == "INACTIVO") isActive = false;

            _pensumsList = Ctrl_CareerPensums.MostrarPensums(Txt_ValorBuscado.Text, careerId, isActive);
            MostrarPensumsEnTabla(_pensumsList);
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            CargarPensums();
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "";
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            CargarPensums();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;

            int careerPensumId = Convert.ToInt32(Tabla.SelectedRows[0].Cells["CareerPensumId"].Value);
            _pensumSeleccionado = _pensumsList.FirstOrDefault(p => p.CareerPensumId == careerPensumId);
            if (_pensumSeleccionado == null) return;

            Txt_Code.Text = _pensumSeleccionado.PensumCode;
            Txt_Name.Text = _pensumSeleccionado.PensumName;
            ComboBox_CareerId.SelectedValue = _pensumSeleccionado.CareerId;
            Cbx_EstadoPensum.SelectedIndex = _pensumSeleccionado.IsCurrent ? 1 : 0;

            Btn_IsActive.Text = _pensumSeleccionado.IsActive ? "INACTIVAR" : "ACTIVAR";
        }

        #endregion

        #region Alta (Nuevo Pensum -> abre el árbol de cursos, nada se guarda aún)

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos(out int careerId, out string code, out string name, out bool isCurrent))
                return;

            // No se guarda nada todavía: se abre el formulario de cursos y solo hasta que
            // el usuario presione "GUARDAR CAMBIOS" ahí se persiste TODO en una sola operación.
            using (var frmCourses = new Frm_AcademicProcesses_CareerPensumCourses(
                careerPensumId: null,
                careerId: careerId,
                pensumCode: code,
                pensumName: name,
                isCurrent: isCurrent))
            {
                frmCourses.UserData = this.UserData;

                if (frmCourses.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Pensum guardado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Btn_Clear_Click(null, null);
                    CargarPensums();
                }
                // Si el usuario cierra/cancela, no se pierde lo escrito en el panel izquierdo.
            }
        }

        #endregion

        #region Editar

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (_pensumSeleccionado == null)
            {
                MessageBox.Show("Seleccione un pensum en la grilla.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos(out int careerId, out string code, out string name, out bool isCurrent))
                return;

            var confirmacion = MessageBox.Show(
                "¿Necesita modificar los cursos de este pensum?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                // El árbol completo (cabecera + cursos) se guarda hasta "GUARDAR CAMBIOS" en el otro formulario
                using (var frmCourses = new Frm_AcademicProcesses_CareerPensumCourses(
                    careerPensumId: _pensumSeleccionado.CareerPensumId,
                    careerId: careerId,
                    pensumCode: code,
                    pensumName: name,
                    isCurrent: isCurrent))
                {
                    frmCourses.UserData = this.UserData;

                    if (frmCourses.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Pensum actualizado correctamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Btn_Clear_Click(null, null);
                        CargarPensums();
                    }
                }
            }
            else
            {
                // Solo cabecera: se guarda de inmediato, no toca cursos ni precios
                var pensum = new Mdl_CareerPensums
                {
                    CareerPensumId = _pensumSeleccionado.CareerPensumId,
                    CareerId = careerId,
                    PensumCode = code,
                    PensumName = name,
                    IsCurrent = isCurrent,
                    ModifiedBy = UserData?.UserId ?? 0
                };

                int resultado = Ctrl_CareerPensums.ActualizarPensum(pensum);

                if (resultado == -1)
                {
                    MessageBox.Show("Ya existe un pensum con ese código para esta carrera.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (resultado <= 0)
                {
                    MessageBox.Show("No se pudo actualizar el pensum.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Pensum actualizado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Btn_Clear_Click(null, null);
                CargarPensums();
            }
        }

        #endregion

        #region Activar/Inactivar

        private void Btn_IsActive_Click(object sender, EventArgs e)
        {
            if (_pensumSeleccionado == null)
            {
                MessageBox.Show("Seleccione un pensum en la grilla.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool estaActivo = _pensumSeleccionado.IsActive;
            bool nuevoEstado = !estaActivo;
            string accion = estaActivo ? "INACTIVAR" : "ACTIVAR";

            var confirmacion = MessageBox.Show(
                $"¿DESEA {accion} EL PENSUM '{_pensumSeleccionado.PensumCode}'?",
                "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            int resultado = Ctrl_CareerPensums.CambiarEstadoPensum(
                _pensumSeleccionado.CareerPensumId, nuevoEstado, UserData?.UserId ?? 0);

            if (resultado > 0)
            {
                MessageBox.Show($"PENSUM {accion}DO CORRECTAMENTE.", "ÉXITO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Btn_Clear_Click(null, null);
                CargarPensums();
            }
            else
            {
                MessageBox.Show("NO SE PUDO CAMBIAR EL ESTADO DEL PENSUM.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Limpiar / Validar

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_Code.Clear();
            Txt_Name.Clear();
            ComboBox_CareerId.SelectedIndex = -1;
            Cbx_EstadoPensum.SelectedIndex = 0;
            Btn_IsActive.Text = "ACTIVAR/INACTIVAR";
            _pensumSeleccionado = null;
            Tabla.ClearSelection();
        }

        private bool ValidarCampos(out int careerId, out string code, out string name, out bool isCurrent)
        {
            careerId = 0; code = null; name = null; isCurrent = false;

            if (ComboBox_CareerId.SelectedValue == null || !(ComboBox_CareerId.SelectedValue is int))
            {
                MessageBox.Show("Seleccione una carrera.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Txt_Code.Text))
            {
                MessageBox.Show("Ingrese el código del pensum.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Txt_Name.Text))
            {
                MessageBox.Show("Ingrese el nombre del pensum.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (Cbx_EstadoPensum.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione el estado del pensum.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            careerId = (int)ComboBox_CareerId.SelectedValue;
            code = Txt_Code.Text.Trim();
            name = Txt_Name.Text.Trim();
            isCurrent = Cbx_EstadoPensum.SelectedItem.ToString() == "VIGENTE";

            return true;
        }

        #endregion

        #region Exportar

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                var listaExportar = Ctrl_CareerPensums.MostrarPensums("", null, null);

                if (listaExportar == null || listaExportar.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Listado de Pensums",
                    FileName = $"Pensums_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

                this.Cursor = Cursors.WaitCursor;

                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                worksheet.Name = "Pensums";

                worksheet.Cells[1, 1] = "REPORTE DE PENSUMS - SECRON";
                worksheet.Range["A1:E1"].Merge();
                worksheet.Range["A1:E1"].Font.Size = 16;
                worksheet.Range["A1:E1"].Font.Bold = true;
                worksheet.Range["A1:E1"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                worksheet.Range["A1:E1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(238, 143, 109));
                worksheet.Range["A1:E1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SISTEMA"}";
                worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {listaExportar.Count}";

                int headerRow = 6;
                string[] headers = { "CÓDIGO", "NOMBRE", "CARRERA", "VIGENTE", "ESTADO" };
                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cells[headerRow, i + 1] = headers[i];

                var headerRange = worksheet.Range[$"A{headerRow}:E{headerRow}"];
                headerRange.Font.Bold = true;
                headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                int row = headerRow + 1;
                foreach (var p in listaExportar)
                {
                    worksheet.Cells[row, 1] = p.PensumCode ?? "";
                    worksheet.Cells[row, 2] = p.PensumName ?? "";
                    worksheet.Cells[row, 3] = p.CareerName ?? "";
                    worksheet.Cells[row, 4] = p.IsCurrent ? "VIGENTE" : "NO VIGENTE";
                    worksheet.Cells[row, 5] = p.IsActive ? "ACTIVO" : "INACTIVO";

                    if (row % 2 == 0)
                        worksheet.Range[$"A{row}:E{row}"].Interior.Color =
                            System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                    row++;
                }

                worksheet.Range[$"A{headerRow}:E{row - 1}"].Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                worksheet.Columns.AutoFit();

                workbook.SaveAs(saveFileDialog.FileName);
                workbook.Close();
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                this.Cursor = Cursors.Default;

                var result = MessageBox.Show("Archivo exportado exitosamente.\n\n¿Desea abrir el archivo ahora?",
                    "Exportación Exitosa", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}