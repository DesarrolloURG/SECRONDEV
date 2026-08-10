using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_ITSM_AuditLogs : Form
    {
        #region PropiedadesIniciales

        public Mdl_Security_UserInfo UserData { get; set; }

        private readonly Dictionary<TextBox, string> _placeholders = new Dictionary<TextBox, string>();

        private List<Mdl_AuditLog> auditLogsList = new List<Mdl_AuditLog>();

        private string _ultimoTextoBusqueda = "";
        private string _ultimoTipoFiltro = "TODOS";
        private string _ultimaAccion = "TODOS";

        // Variables de paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;

        // ToolStrip de paginación
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        public Frm_ITSM_AuditLogs()
        {
            InitializeComponent();
            this.Load += Frm_ITSM_AuditLogs_Load;

            CargarFiltrosBusqueda();
            ConfigurarPlaceHoldersTextbox();
        }

        #endregion

        #region CargaInicial

        private async void Frm_ITSM_AuditLogs_Load(object sender, EventArgs e)
        {
            ConfigurarTabIndexYFocus();
            CrearToolStripPaginacion();
            paginaActual = 1;

            if (UserData != null)
            {
                await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
                ConfigurarControlesPorPermisos();
            }

            RefrescarListado();

            if (Tabla.Rows.Count > 0)
                Tabla.ClearSelection();
        }

        #endregion

        #region Tabla

        public void RefrescarListado()
        {
            if (!Btn_Search.Enabled) return;

            int total;
            auditLogsList = Ctrl_AuditLogs.BuscarAuditLogs(
                _ultimoTextoBusqueda, _ultimoTipoFiltro, _ultimaAccion,
                paginaActual, registrosPorPagina, out total);

            totalRegistros = total;
            totalPaginas = totalRegistros == 0 ? 0 : (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            Tabla.DataSource = null;
            Tabla.DataSource = auditLogsList;

            ConfigurarTabla();
            ActualizarInfoPaginacion();
        }

        public void ConfigurarTabla()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["Tabla"].HeaderText = "TABLA";
                Tabla.Columns["Campo"].HeaderText = "CAMPO";
                Tabla.Columns["ValorAnterior"].HeaderText = "VALOR ANTERIOR";
                Tabla.Columns["ValorNuevo"].HeaderText = "VALOR NUEVO";
                Tabla.Columns["Action"].HeaderText = "ACCIÓN";
                Tabla.Columns["ActionDate"].HeaderText = "FECHA";
                Tabla.Columns["HostName"].HeaderText = "EQUIPO";
                Tabla.Columns["IPAddress"].HeaderText = "IP";
                Tabla.Columns["Username"].HeaderText = "USUARIO";
                Tabla.Columns["FullName"].HeaderText = "NOMBRE COMPLETO";
                Tabla.Columns["Rol"].HeaderText = "ROL";

                Tabla.Columns["TotalRegistros"].Visible = false;

                Tabla.Columns["ActionDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            }

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToDeleteRows = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        }

        private void LimpiarSeleccionTabla()
        {
            if (Tabla.Rows.Count > 0)
                Tabla.ClearSelection();
        }

        #endregion

        #region FiltrosBusqueda

        private void CargarFiltrosBusqueda()
        {
            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("TABLA");
            Filtro1.Items.Add("CAMPO");
            Filtro1.Items.Add("USUARIO");
            Filtro1.Items.Add("VALOR ANTERIOR");
            Filtro1.Items.Add("VALOR NUEVO");
            Filtro1.SelectedIndex = 0;

            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS");
            Filtro2.Items.Add("INSERT");
            Filtro2.Items.Add("UPDATE");
            Filtro2.Items.Add("DELETE");
            Filtro2.SelectedIndex = 0;
        }

        private void EjecutarBusqueda()
        {
            if (!Btn_Search.Enabled) return;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                string valorBusqueda = TienePlaceholder(Txt_ValorBuscado)
                    ? ""
                    : Txt_ValorBuscado.Text.Trim();

                _ultimoTextoBusqueda = valorBusqueda;
                _ultimoTipoFiltro = Filtro1.SelectedItem?.ToString() ?? "TODOS";
                _ultimaAccion = Filtro2.SelectedItem?.ToString() ?? "TODOS";
                paginaActual = 1;

                RefrescarListado();
                LimpiarSeleccionTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error en búsqueda: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            if (!Btn_Search.Enabled) return;
            EjecutarBusqueda();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Btn_Search.Enabled) return;

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                EjecutarBusqueda();
            }
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            if (!Btn_CleanSearch.Enabled) return;

            Txt_ValorBuscado.Text = "";
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;

            _ultimoTextoBusqueda = "";
            _ultimoTipoFiltro = "TODOS";
            _ultimaAccion = "TODOS";
            paginaActual = 1;

            AplicarPlaceHolderSiVacio(Txt_ValorBuscado);
            RefrescarListado();
            LimpiarSeleccionTabla();
        }

        #endregion

        #region Placeholders

        private void ConfigurarTabIndexYFocus()
        {
            Txt_ValorBuscado.TabIndex = 0;
            Filtro1.TabIndex = 1;
            Filtro2.TabIndex = 2;
            Btn_Search.TabIndex = 3;
            Btn_CleanSearch.TabIndex = 4;
            Tabla.TabIndex = 5;

            Panel_Superior.TabStop = false;
            Panel_Derecho.TabStop = false;
            Panel_Izquierdo.TabStop = false;
            PanelToolStrip.TabStop = false;
            Panel_Busqueda.TabStop = false;

            Txt_ValorBuscado.Focus();
        }

        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR...");
        }

        private void ConfigurarPlaceHolder(TextBox txt, string placeholder)
        {
            if (_placeholders.ContainsKey(txt))
                _placeholders[txt] = placeholder;
            else
                _placeholders.Add(txt, placeholder);

            txt.Enter -= TextBox_EnterPlaceholder;
            txt.Leave -= TextBox_LeavePlaceholder;

            txt.Enter += TextBox_EnterPlaceholder;
            txt.Leave += TextBox_LeavePlaceholder;

            AplicarPlaceHolderSiVacio(txt);
        }

        private void TextBox_EnterPlaceholder(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null || !_placeholders.ContainsKey(txt))
                return;

            if (txt.Text == _placeholders[txt])
            {
                txt.Text = "";
                txt.ForeColor = Color.Black;
            }
        }

        private void TextBox_LeavePlaceholder(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null)
                return;

            AplicarPlaceHolderSiVacio(txt);
        }

        private void AplicarPlaceHolderSiVacio(TextBox txt)
        {
            if (!_placeholders.ContainsKey(txt))
                return;

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = _placeholders[txt];
                txt.ForeColor = Color.Gray;
            }
        }

        private bool TienePlaceholder(TextBox txt)
        {
            return _placeholders.ContainsKey(txt) && txt.Text == _placeholders[txt];
        }

        #endregion

        #region Paginacion

        private void CrearToolStripPaginacion()
        {
            if (toolStripPaginacion != null)
                return;

            toolStripPaginacion = new ToolStrip
            {
                Dock = DockStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                GripStyle = ToolStripGripStyle.Hidden,
                BackColor = Color.FromArgb(248, 249, 250),
                Height = 35,
                AutoSize = true,
                Location = new Point(PanelToolStrip.Width - 260, 5)
            };

            btnAnterior = new ToolStripButton
            {
                Text = "❮ Anterior",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(51, 140, 255),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Margin = new Padding(2),
                Padding = new Padding(8, 4, 8, 4)
            };
            btnAnterior.Click += (s, e) => CambiarPagina(paginaActual - 1);

            btnSiguiente = new ToolStripButton
            {
                Text = "Siguiente ❯",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(238, 143, 109),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Margin = new Padding(2),
                Padding = new Padding(8, 4, 8, 4)
            };
            btnSiguiente.Click += (s, e) => CambiarPagina(paginaActual + 1);

            toolStripPaginacion.Items.Add(btnAnterior);
            toolStripPaginacion.Items.Add(btnSiguiente);

            PanelToolStrip.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.BringToFront();

            PanelToolStrip.Resize += (s, e) =>
            {
                if (toolStripPaginacion != null)
                    toolStripPaginacion.Location = new Point(PanelToolStrip.Width - 260, 5);
            };
        }

        private void ActualizarBotonesNumerados()
        {
            if (toolStripPaginacion == null)
                return;

            var itemsToRemove = toolStripPaginacion.Items
                .Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton")
                .ToList();

            foreach (var item in itemsToRemove)
                toolStripPaginacion.Items.Remove(item);

            if (totalPaginas <= 1)
                return;

            int inicioRango = Math.Max(1, paginaActual - 1);
            int finRango = Math.Min(totalPaginas, paginaActual + 1);
            int posicionInsertar = toolStripPaginacion.Items.IndexOf(btnSiguiente);

            for (int i = inicioRango; i <= finRango; i++)
            {
                ToolStripButton btnPagina = new ToolStripButton
                {
                    Text = i.ToString(),
                    Tag = "PageButton",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Margin = new Padding(1),
                    Padding = new Padding(6, 4, 6, 4)
                };

                if (i == paginaActual)
                {
                    btnPagina.BackColor = Color.FromArgb(238, 143, 109);
                    btnPagina.ForeColor = Color.White;
                }
                else
                {
                    btnPagina.BackColor = Color.FromArgb(240, 240, 240);
                    btnPagina.ForeColor = Color.FromArgb(51, 140, 255);
                }

                int numeroPagina = i;
                btnPagina.Click += (s, e) => CambiarPagina(numeroPagina);

                toolStripPaginacion.Items.Insert(posicionInsertar++, btnPagina);
            }
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1 || nuevaPagina > totalPaginas)
                return;

            paginaActual = nuevaPagina;
            RefrescarListado();
            LimpiarSeleccionTabla();
        }

        private void ActualizarInfoPaginacion()
        {
            if (btnAnterior != null)
                btnAnterior.Enabled = paginaActual > 1;

            if (btnSiguiente != null)
                btnSiguiente.Enabled = paginaActual < totalPaginas;

            ActualizarBotonesNumerados();

            int inicioRango = totalRegistros == 0 ? 0 : ((paginaActual - 1) * registrosPorPagina) + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            if (totalRegistros == 0)
                Lbl_Paginas.Text = "NO HAY LOGS PARA MOSTRAR";
            else
                Lbl_Paginas.Text = $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} LOGS";
        }

        #endregion

        #region SistemaDePermisos

        private Ctrl_Security_Auth authController;
        private HashSet<string> permisosUsuario = new HashSet<string>();

        protected virtual async Task CargarPermisosUsuario(int userId, int roleId)
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
            AplicarEstadoBotonPorPermiso(Btn_Search, "ITSM_LOGSAUDIT_READ");
            AplicarEstadoBotonPorPermiso(Btn_CleanSearch, "ITSM_LOGSAUDIT_READ");
            AplicarEstadoBotonPorPermiso(Btn_Export, "ITSM_LOGSAUDIT_EXPORT");
        }

        #endregion SistemaDePermisos

        #region Exportacion

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Btn_Export.Enabled) return;

                int totalExportar;
                var listaExportar = Ctrl_AuditLogs.BuscarAuditLogs(
                    _ultimoTextoBusqueda, _ultimoTipoFiltro, _ultimaAccion,
                    1, Math.Max(totalRegistros, 1), out totalExportar);

                if (listaExportar == null || listaExportar.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar", "Información",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Logs de Auditoría",
                    FileName = $"LogsAuditoria_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    var excelApp = new Microsoft.Office.Interop.Excel.Application();
                    var workbook = excelApp.Workbooks.Add();
                    var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Logs Auditoría";

                    // ============ ENCABEZADO PRINCIPAL ============
                    worksheet.Cells[1, 1] = "REPORTE DE LOGS DE AUDITORÍA - SECRON";
                    worksheet.Range["A1:K1"].Merge();
                    worksheet.Range["A1:K1"].Font.Size = 16;
                    worksheet.Range["A1:K1"].Font.Bold = true;
                    worksheet.Range["A1:K1"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Range["A1:K1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(238, 143, 109));
                    worksheet.Range["A1:K1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                    // ============ INFORMACIÓN DEL REPORTE ============
                    worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SISTEMA"}";
                    worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {listaExportar.Count}";

                    worksheet.Range["A2:A4"].Font.Size = 10;
                    worksheet.Range["A2:A4"].Font.Bold = true;

                    // ============ ENCABEZADOS DE COLUMNAS ============
                    int headerRow = 6;
                    string[] headers = { "TABLA", "CAMPO", "VALOR ANTERIOR", "VALOR NUEVO", "ACCIÓN",
                        "FECHA", "EQUIPO", "IP", "USUARIO", "NOMBRE COMPLETO", "ROL" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[headerRow, i + 1] = headers[i];
                    }

                    var headerRange = worksheet.Range[$"A{headerRow}:K{headerRow}"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Size = 11;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    headerRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                    // ============ DATOS ============
                    int row = headerRow + 1;
                    foreach (var log in listaExportar)
                    {
                        worksheet.Cells[row, 1] = log.Tabla ?? "";
                        worksheet.Cells[row, 2] = log.Campo ?? "";
                        worksheet.Cells[row, 3] = log.ValorAnterior ?? "";
                        worksheet.Cells[row, 4] = log.ValorNuevo ?? "";
                        worksheet.Cells[row, 5] = log.Action ?? "";
                        worksheet.Cells[row, 6] = log.ActionDate.ToString("dd/MM/yyyy HH:mm:ss");
                        worksheet.Cells[row, 7] = log.HostName ?? "";
                        worksheet.Cells[row, 8] = log.IPAddress ?? "";
                        worksheet.Cells[row, 9] = log.Username ?? "";
                        worksheet.Cells[row, 10] = log.FullName ?? "";
                        worksheet.Cells[row, 11] = log.Rol ?? "";

                        // Alternar color de filas
                        if (row % 2 == 0)
                        {
                            worksheet.Range[$"A{row}:K{row}"].Interior.Color =
                                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                        }

                        row++;
                    }

                    // ============ FORMATO FINAL ============
                    var dataRange = worksheet.Range[$"A{headerRow}:K{row - 1}"];
                    dataRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    dataRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;

                    worksheet.Columns.AutoFit();

                    worksheet.Activate();
                    excelApp.ActiveWindow.SplitRow = headerRow;
                    excelApp.ActiveWindow.FreezePanes = true;

                    // ============ PIE DE PÁGINA ============
                    worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
                    worksheet.Range[$"A{row + 1}:K{row + 1}"].Merge();
                    worksheet.Range[$"A{row + 1}:K{row + 1}"].Font.Italic = true;
                    worksheet.Range[$"A{row + 1}:K{row + 1}"].Font.Size = 9;
                    worksheet.Range[$"A{row + 1}:K{row + 1}"].HorizontalAlignment =
                        Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    workbook.SaveAs(saveFileDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                    this.Cursor = Cursors.Default;

                    var result = MessageBox.Show(
                        "Archivo exportado exitosamente.\n\n¿Desea abrir el archivo ahora?",
                        "Exportación Exitosa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
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