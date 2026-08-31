using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_FixedAsset_ResponsabilityLetterHistory : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        // Grid superior: listado de activos fijos
        private List<Mdl_FixedAsset> _assetsList;
        private Mdl_FixedAsset _activoSeleccionado = null;

        private string _ultimoTextoBusqueda = "";
        private string _ultimoFiltro1 = "TODOS";
        private string _ultimoFiltroEstado = "SOLO ACTIVOS";
        private int? _ultimoCategoriaId = null;

        // Grid inferior: histórico de cartas de responsabilidad del activo seleccionado
        private List<Mdl_ResponsibilityLetterMaster> _historialList = new List<Mdl_ResponsibilityLetterMaster>();

        // Paginación (grid superior)
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        private HashSet<string> permisosUsuario = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        #endregion

        public Frm_FixedAsset_ResponsabilityLetterHistory()
        {
            InitializeComponent();
        }

        private async void Frm_FixedAsset_ResponsabilityLetterHistory_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Panel de edición de características (heredado del formulario copiado) no aplica aquí:
                // esta pantalla es de solo consulta del histórico de cartas.
                Panel_AtributosCRUD.Visible = false;

                ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR POR CÓDIGO, NOMBRE O SERIE...");
                ConfigurarPlaceHolder(Txt_BuscarAtributo, "BUSCAR EN EL HISTÓRICO...");

                ConfigurarFiltros();
                ConfigurarTabla();
                ConfigurarFiltrosHistorial();
                ConfigurarTablaHistorial();
                CrearToolStripPaginacion();

                if (UserData != null)
                {
                    await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
                }

                if (TienePermiso("FA_ASSETS_READ"))
                {
                    CargarActivos();
                    ActualizarInfoPaginacion();
                }
                else
                {
                    Lbl_Paginas.Text = "SIN PERMISOS PARA VER ACTIVOS FIJOS";
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Permisos

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

        private bool TienePermiso(string permissionCode)
        {
            return !string.IsNullOrWhiteSpace(permissionCode) &&
                   permisosUsuario != null &&
                   permisosUsuario.Contains(permissionCode);
        }

        #endregion

        #region PlaceHolder

        private void ConfigurarPlaceHolder(TextBox textBox, string placeholder)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder && textBox.ForeColor == Color.Gray)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private bool TienePlaceholder(TextBox textBox, string placeholder)
        {
            return textBox.Text == placeholder && textBox.ForeColor == Color.Gray;
        }

        private void SetTextBoxFromValue(TextBox textBox, string valor, string placeholder)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = Color.Gray;
            }
            else
            {
                textBox.Text = valor;
                textBox.ForeColor = Color.Black;
            }
        }

        #endregion

        #region Grid superior — Listado de activos

        private void ConfigurarFiltros()
        {
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            FiltroEstado.DropDownStyle = ComboBoxStyle.DropDownList;

            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("POR CÓDIGO");
            Filtro1.Items.Add("POR NOMBRE");
            Filtro1.Items.Add("POR SERIE");
            Filtro1.SelectedIndex = 0;

            FiltroEstado.Items.Clear();
            FiltroEstado.Items.Add("TODOS");
            FiltroEstado.Items.Add("SOLO ACTIVOS");
            FiltroEstado.Items.Add("SOLO INACTIVOS");
            FiltroEstado.SelectedIndex = 1;
        }

        private void ConfigurarTabla()
        {
            Tabla.AutoGenerateColumns = false;
            Tabla.Columns.Clear();

            var colEstado = new DataGridViewTextBoxColumn();
            colEstado.Name = "ColEstadoRegistro";
            colEstado.HeaderText = "ESTADO";
            colEstado.DataPropertyName = "IsActiveText";
            colEstado.Width = 90;
            colEstado.ReadOnly = true;
            colEstado.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.Columns.Add(colEstado);

            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAssetId", HeaderText = "ID", DataPropertyName = "AssetId", Visible = false });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "CÓDIGO", DataPropertyName = "AssetCode", Width = 110 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "NOMBRE", DataPropertyName = "AssetName", Width = 220 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCategoria", HeaderText = "CATEGORÍA", DataPropertyName = "CategoryName", Width = 150 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstadoActivo", HeaderText = "ESTADO ACTIVO", DataPropertyName = "AssetStatus", Width = 130 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBodega", HeaderText = "BODEGA", DataPropertyName = "WarehouseName", Width = 150 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAsignado",
                HeaderText = "ASIGNADO A",
                DataPropertyName = "EmployeeName",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.RowHeadersVisible = false;
            Tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            Tabla.ScrollBars = ScrollBars.Both;

            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            Tabla.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            Tabla.RowTemplate.Height = 25;

            Tabla.CellFormatting -= Tabla_CellFormatting_Estado;
            Tabla.CellFormatting += Tabla_CellFormatting_Estado;

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void Tabla_CellFormatting_Estado(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (Tabla.Columns[e.ColumnIndex].Name != "ColEstadoRegistro") return;
            if (e.Value == null) return;

            bool activo = e.Value.ToString() == "ACTIVO";
            e.CellStyle.ForeColor = activo ? Color.FromArgb(0, 128, 0) : Color.FromArgb(200, 0, 0);
            e.CellStyle.Font = new Font(Tabla.Font, FontStyle.Bold);
        }

        private void AsignarDataSourceActivos()
        {
            if (_assetsList == null) { Tabla.DataSource = null; return; }

            var data = _assetsList.Select(a => new
            {
                a.AssetId,
                a.AssetCode,
                a.AssetName,
                CategoryName = a.CategoryName ?? "",
                a.AssetStatus,
                WarehouseName = a.WarehouseName ?? "",
                EmployeeName = a.EmployeeName ?? "",
                IsActiveText = a.IsActive ? "ACTIVO" : "INACTIVO"
            }).ToList();

            Tabla.DataSource = data;
        }

        private void CargarActivos()
        {
            RefrescarListado();

            if (Tabla.Rows.Count > 0)
                Tabla.Rows[0].Selected = true;
        }

        private void RefrescarListado()
        {
            _assetsList = Ctrl_FixedAssets.MostrarActivos(paginaActual, registrosPorPagina);
            AsignarDataSourceActivos();
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string valorBusqueda = TienePlaceholder(Txt_ValorBuscado, "BUSCAR POR CÓDIGO, NOMBRE O SERIE...")
                    ? "" : Txt_ValorBuscado.Text.Trim();

                _ultimoTextoBusqueda = valorBusqueda;
                _ultimoFiltro1 = Filtro1.SelectedItem?.ToString() ?? "TODOS";
                _ultimoFiltroEstado = FiltroEstado.SelectedItem?.ToString() ?? "TODOS";

                paginaActual = 1;

                _assetsList = string.IsNullOrEmpty(_ultimoTextoBusqueda)
                    ? Ctrl_FixedAssets.MostrarActivos(paginaActual, registrosPorPagina)
                    : Ctrl_FixedAssets.BuscarActivos(
                        textoBusqueda: _ultimoTextoBusqueda,
                        filtro1: _ultimoFiltro1,
                        filtroEstado: _ultimoFiltroEstado,
                        categoriaId: _ultimoCategoriaId,
                        pageNumber: paginaActual,
                        pageSize: registrosPorPagina);

                AsignarDataSourceActivos();

                if (Tabla.Rows.Count > 0)
                    Tabla.Rows[0].Selected = true;
                else
                    LimpiarHistorial();

                totalRegistros = Ctrl_FixedAssets.ContarTotalActivos(_ultimoTextoBusqueda, _ultimoFiltroEstado, _ultimoCategoriaId);
                ActualizarInfoPaginacion();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Tabla.SelectionChanged -= Tabla_SelectionChanged;

            SetTextBoxFromValue(Txt_ValorBuscado, "", "BUSCAR POR CÓDIGO, NOMBRE O SERIE...");
            Filtro1.SelectedIndex = 0;
            FiltroEstado.SelectedIndex = 1;

            _ultimoTextoBusqueda = "";
            _ultimoFiltro1 = "TODOS";
            _ultimoFiltroEstado = "SOLO ACTIVOS";
            _ultimoCategoriaId = null;
            paginaActual = 1;

            RefrescarListado();
            ActualizarInfoPaginacion();

            Tabla.SelectionChanged += Tabla_SelectionChanged;

            if (Tabla.Rows.Count > 0)
                Tabla.Rows[0].Selected = true;
            else
                LimpiarHistorial();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_Search_Click(sender, e); }
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;

            object valorId = Tabla.SelectedRows[0].Cells["colAssetId"].Value;
            if (valorId == null || valorId == DBNull.Value) return;

            int assetId = Convert.ToInt32(valorId);
            _activoSeleccionado = _assetsList?.FirstOrDefault(a => a.AssetId == assetId);
            if (_activoSeleccionado == null) return;

            Lbl_AtributosHeader.Text = $"HISTÓRICO DE CARTAS — {_activoSeleccionado.AssetCode} — {_activoSeleccionado.AssetName}";

            CargarHistorialDelActivo(_activoSeleccionado.AssetId);
        }

        #endregion

        #region ToolStrip_Paginacion

        private void CrearToolStripPaginacion()
        {
            toolStripPaginacion = new ToolStrip();
            toolStripPaginacion.Dock = DockStyle.None;
            toolStripPaginacion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            toolStripPaginacion.GripStyle = ToolStripGripStyle.Hidden;
            toolStripPaginacion.BackColor = Color.FromArgb(248, 249, 250);
            toolStripPaginacion.AutoSize = true;
            toolStripPaginacion.Location = new Point(PanelToolStrip.Width - 300, 0);

            btnAnterior = new ToolStripButton { Text = "❮ Anterior" };
            btnAnterior.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnterior.ForeColor = Color.White;
            btnAnterior.BackColor = Color.FromArgb(51, 140, 255);
            btnAnterior.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAnterior.Margin = new Padding(2);
            btnAnterior.Padding = new Padding(8, 4, 8, 4);
            btnAnterior.Click += (s, ev) => CambiarPagina(paginaActual - 1);
            toolStripPaginacion.Items.Add(btnAnterior);

            btnSiguiente = new ToolStripButton { Text = "Siguiente ❯" };
            btnSiguiente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSiguiente.ForeColor = Color.White;
            btnSiguiente.BackColor = Color.FromArgb(238, 143, 109);
            btnSiguiente.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSiguiente.Margin = new Padding(2);
            btnSiguiente.Padding = new Padding(8, 4, 8, 4);
            btnSiguiente.Click += (s, ev) => CambiarPagina(paginaActual + 1);
            toolStripPaginacion.Items.Add(btnSiguiente);

            PanelToolStrip.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.BringToFront();

            PanelToolStrip.Resize += (s, e) =>
            {
                if (toolStripPaginacion != null)
                    toolStripPaginacion.Location = new Point(PanelToolStrip.Width - toolStripPaginacion.Width, 0);
            };
        }

        private void ActualizarBotonesNumerados()
        {
            var toRemove = toolStripPaginacion.Items.Cast<ToolStripItem>()
                .Where(i => i.Tag?.ToString() == "PageButton").ToList();
            foreach (var i in toRemove)
                toolStripPaginacion.Items.Remove(i);

            if (totalPaginas <= 1) return;

            int inicio = Math.Max(1, paginaActual - 1);
            int fin = Math.Min(totalPaginas, paginaActual + 1);
            int posicion = toolStripPaginacion.Items.IndexOf(btnAnterior) + 1;

            for (int i = inicio; i <= fin; i++)
            {
                var btn = new ToolStripButton { Text = i.ToString(), Tag = "PageButton" };
                btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btn.Margin = new Padding(1);
                btn.Padding = new Padding(6, 4, 6, 4);
                btn.BackColor = i == paginaActual ? Color.FromArgb(238, 143, 109) : Color.FromArgb(240, 240, 240);
                btn.ForeColor = i == paginaActual ? Color.White : Color.Black;
                btn.DisplayStyle = ToolStripItemDisplayStyle.Text;
                int pagina = i;
                btn.Click += (s, ev) => CambiarPagina(pagina);
                toolStripPaginacion.Items.Insert(posicion + (i - inicio), btn);
            }

            toolStripPaginacion.Location = new Point(PanelToolStrip.Width - toolStripPaginacion.Width, 0);
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1 || nuevaPagina > totalPaginas) return;
            paginaActual = nuevaPagina;

            if (!string.IsNullOrEmpty(_ultimoTextoBusqueda))
            {
                _assetsList = Ctrl_FixedAssets.BuscarActivos(
                    textoBusqueda: _ultimoTextoBusqueda,
                    filtro1: _ultimoFiltro1,
                    filtroEstado: _ultimoFiltroEstado,
                    categoriaId: _ultimoCategoriaId,
                    pageNumber: paginaActual,
                    pageSize: registrosPorPagina);
                AsignarDataSourceActivos();
            }
            else
            {
                RefrescarListado();
            }

            if (Tabla.Rows.Count > 0)
                Tabla.Rows[0].Selected = true;
            else
                LimpiarHistorial();

            ActualizarInfoPaginacion();
        }

        private void ActualizarInfoPaginacion()
        {
            if (totalRegistros == 0)
                totalRegistros = Ctrl_FixedAssets.ContarTotalActivos(_ultimoTextoBusqueda, _ultimoFiltroEstado, _ultimoCategoriaId);

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;
            ActualizarBotonesNumerados();

            int inicio = totalRegistros == 0 ? 0 : (paginaActual - 1) * registrosPorPagina + 1;
            int fin = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            Lbl_Paginas.Text = totalRegistros == 0
                ? "NO HAY ACTIVOS PARA MOSTRAR"
                : $"MOSTRANDO {inicio}-{fin} DE {totalRegistros} ACTIVOS";
        }

        #endregion

        #region Grid inferior — Histórico de cartas de responsabilidad

        private void ConfigurarFiltrosHistorial()
        {
            FiltroAtributoTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            FiltroAtributoTipo.Items.Clear();
            FiltroAtributoTipo.Items.Add("TODOS");
            FiltroAtributoTipo.Items.Add("VIGENTE");
            FiltroAtributoTipo.Items.Add("NO VIGENTE");
            FiltroAtributoTipo.SelectedIndex = 0;
        }

        private void ConfigurarTablaHistorial()
        {
            TablaAtributos.AutoGenerateColumns = false;
            TablaAtributos.Columns.Clear();

            var colAbrir = new DataGridViewImageColumn();
            colAbrir.Name = "colAbrirHistorial";
            colAbrir.HeaderText = "CARTA";
            colAbrir.Width = 70;
            colAbrir.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colAbrir.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colAbrir.Resizable = DataGridViewTriState.False;
            colAbrir.DefaultCellStyle.NullValue = Properties.Resources.SearchNegro25x25;
            colAbrir.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colAbrir.ToolTipText = "ABRIR CARTA DE RESPONSABILIDAD";
            TablaAtributos.Columns.Add(colAbrir);

            var colVigente = new DataGridViewTextBoxColumn();
            colVigente.Name = "colVigente";
            colVigente.HeaderText = "VIGENTE";
            colVigente.DataPropertyName = "VigenteText";
            colVigente.Width = 100;
            colVigente.ReadOnly = true;
            colVigente.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            TablaAtributos.Columns.Add(colVigente);

            TablaAtributos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEmpleado", HeaderText = "EMPLEADO", DataPropertyName = "EmployeeName", Width = 220 });
            TablaAtributos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colArchivo", HeaderText = "ARCHIVO", DataPropertyName = "FileName", Width = 260 });
            TablaAtributos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFechaCarga", HeaderText = "FECHA DE CARGA", DataPropertyName = "UploadDateText", Width = 150 });
            TablaAtributos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSubidoPor", HeaderText = "SUBIDO POR", DataPropertyName = "UploadedByName", Width = 200 });

            var colEstado = new DataGridViewTextBoxColumn();
            colEstado.Name = "colEstadoCarta";
            colEstado.HeaderText = "ESTADO";
            colEstado.DataPropertyName = "IsActiveText";
            colEstado.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colEstado.ReadOnly = true;
            colEstado.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            TablaAtributos.Columns.Add(colEstado);

            // Columna oculta con la ruta física del archivo, usada por colAbrirHistorial al hacer clic
            TablaAtributos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFilePath", DataPropertyName = "FilePath", Visible = false });

            TablaAtributos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TablaAtributos.MultiSelect = false;
            TablaAtributos.ReadOnly = true;
            TablaAtributos.AllowUserToAddRows = false;
            TablaAtributos.AllowUserToResizeRows = false;
            TablaAtributos.RowHeadersVisible = false;
            TablaAtributos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            TablaAtributos.ScrollBars = ScrollBars.Both;

            TablaAtributos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            TablaAtributos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            TablaAtributos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            TablaAtributos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            TablaAtributos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            TablaAtributos.DefaultCellStyle.SelectionForeColor = Color.White;
            TablaAtributos.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            TablaAtributos.RowTemplate.Height = 30;

            TablaAtributos.CellFormatting -= TablaAtributos_CellFormatting_Vigente;
            TablaAtributos.CellFormatting += TablaAtributos_CellFormatting_Vigente;
            TablaAtributos.CellFormatting -= TablaAtributos_CellFormatting_Estado;
            TablaAtributos.CellFormatting += TablaAtributos_CellFormatting_Estado;

            TablaAtributos.CellClick -= TablaAtributos_CellClick_Abrir;
            TablaAtributos.CellClick += TablaAtributos_CellClick_Abrir;
        }

        private void TablaAtributos_CellFormatting_Vigente(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (TablaAtributos.Columns[e.ColumnIndex].Name != "colVigente") return;
            if (e.Value == null) return;

            e.CellStyle.ForeColor = Color.Black;
            e.CellStyle.Font = new Font(TablaAtributos.Font, FontStyle.Bold);
        }

        private void TablaAtributos_CellFormatting_Estado(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (TablaAtributos.Columns[e.ColumnIndex].Name != "colEstadoCarta") return;
            if (e.Value == null) return;

            bool activo = e.Value.ToString() == "ACTIVO";
            e.CellStyle.ForeColor = activo ? Color.FromArgb(0, 128, 0) : Color.FromArgb(200, 0, 0);
            e.CellStyle.Font = new Font(TablaAtributos.Font, FontStyle.Bold);
        }

        private void TablaAtributos_CellClick_Abrir(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (TablaAtributos.Columns[e.ColumnIndex].Name != "colAbrirHistorial") return;

            object valor = TablaAtributos.Rows[e.RowIndex].Cells["colFilePath"].Value;
            string filePath = (valor == null || valor == DBNull.Value) ? null : valor.ToString();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("ESTE REGISTRO NO TIENE UN ARCHIVO VINCULADO.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!System.IO.File.Exists(filePath))
            {
                MessageBox.Show("EL ARCHIVO NO SE ENCUENTRA EN LA RUTA INDICADA.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            System.Diagnostics.Process.Start(filePath);
        }

        private void CargarHistorialDelActivo(int assetId)
        {
            try
            {
                _historialList = Ctrl_ResponsibilityLetters.ObtenerHistorialPorActivo(assetId);
                AplicarFiltroHistorial();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el histórico de cartas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarHistorial()
        {
            _activoSeleccionado = null;
            _historialList = new List<Mdl_ResponsibilityLetterMaster>();
            Lbl_AtributosHeader.Text = "HISTÓRICO DE CARTAS — SELECCIONE UN ACTIVO FIJO";
            TablaAtributos.DataSource = null;
        }

        private void AplicarFiltroHistorial()
        {
            string valor = TienePlaceholder(Txt_BuscarAtributo, "BUSCAR EN EL HISTÓRICO...")
                ? "" : Txt_BuscarAtributo.Text.Trim().ToUpper();
            string tipo = FiltroAtributoTipo.SelectedItem?.ToString() ?? "TODOS";

            var filtrados = _historialList.Where(h =>
                (string.IsNullOrEmpty(valor) ||
                    (h.FileName ?? "").ToUpper().Contains(valor) ||
                    (h.EmployeeName ?? "").ToUpper().Contains(valor) ||
                    (h.UploadedByName ?? "").ToUpper().Contains(valor)) &&
                (tipo == "TODOS" ||
                    (tipo == "VIGENTE" && h.IsCurrent) ||
                    (tipo == "NO VIGENTE" && !h.IsCurrent))
            ).ToList();

            var data = filtrados.Select(h => new
            {
                h.ResponsibilityLetterId,
                VigenteText = h.IsCurrent ? "VIGENTE" : "NO VIGENTE",
                EmployeeName = h.EmployeeName ?? "",
                h.FileName,
                UploadDateText = h.UploadDate.ToString("dd/MM/yyyy HH:mm"),
                UploadedByName = h.UploadedByName ?? "",
                IsActiveText = h.IsActive ? "ACTIVO" : "INACTIVO",
                h.FilePath
            }).ToList();

            TablaAtributos.DataSource = null;
            TablaAtributos.DataSource = data;
            ConfigurarTablaHistorial();
        }

        private void Btn_SearchAtributo_Click(object sender, EventArgs e)
        {
            AplicarFiltroHistorial();
        }

        private void Txt_BuscarAtributo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; AplicarFiltroHistorial(); }
        }

        private void Btn_CleanSearchAtributo_Click(object sender, EventArgs e)
        {
            SetTextBoxFromValue(Txt_BuscarAtributo, "", "BUSCAR EN EL HISTÓRICO...");
            FiltroAtributoTipo.SelectedIndex = 0;
            AplicarFiltroHistorial();
        }

        // Panel_AtributosCRUD está oculto (Visible = false) por no aplicar a una pantalla de solo consulta,
        // pero el Designer aún cablea estos 2 eventos, así que quedan como no-op hasta que se borren del diseño.
        private void Btn_UpdateAtributo_Click(object sender, EventArgs e) { }
        private void Btn_ClearAtributo_Click(object sender, EventArgs e) { }

        #endregion
    }
}