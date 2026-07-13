using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_LocationStaffAssignments_SearchUsers : Form
    {
        #region PropiedadesIniciales
        private const int PAGE_SIZE = 20;

        private Frm_LocationStaffAssignments _frmPadre;
        private int _locationId;
        private int _userIdSeleccionado = 0;
        private string _nombreSeleccionado = "";

        private List<Mdl_LocationStaffAssignments> _usuariosList;
        private int paginaActual = 1;
        private int totalRegistros = 0;
        private string _ultimoTexto = "";

        private ToolStrip toolStripPaginacion;
        private Label lblPaginacion;

        public Frm_LocationStaffAssignments_SearchUsers(Frm_LocationStaffAssignments frmPadre, int locationId)
        {
            InitializeComponent();
            _frmPadre = frmPadre;
            _locationId = locationId;
            ConfigurarTamañoFormulario();
        }

        private void Frm_LocationStaffAssignments_SearchUsers_Load(object sender, EventArgs e)
        {
            ConfigurarComponentesDeshabilitados();
            ConfigurarPlaceHolder();
            ConfigurarTabla();
            CrearToolStripPaginacion();
            CargarDatos();
        }

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(700, 650);
            this.MinimumSize = new Size(700, 650);
            this.MaximumSize = new Size(700, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_Beneficiario.Enabled = false;
        }
        #endregion PropiedadesIniciales

        #region ConfigurarPlaceHolder
        private void ConfigurarPlaceHolder()
        {
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_ValorBuscado.Text = "BUSCAR USUARIO...";

            Txt_ValorBuscado.GotFocus += (s, e) =>
            {
                if (Txt_ValorBuscado.Text == "BUSCAR USUARIO...")
                {
                    Txt_ValorBuscado.Text = "";
                    Txt_ValorBuscado.ForeColor = Color.Black;
                }
            };

            Txt_ValorBuscado.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                {
                    Txt_ValorBuscado.Text = "BUSCAR USUARIO...";
                    Txt_ValorBuscado.ForeColor = Color.Gray;
                }
            };
        }
        #endregion ConfigurarPlaceHolder

        #region ConfigurarTabla
        private void ConfigurarTabla()
        {
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.RowHeadersVisible = false;

            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            Tabla.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
            Tabla.CellBeginEdit += (s, e) => e.Cancel = true;
            Tabla.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete) e.Handled = true; };
        }
        #endregion ConfigurarTabla

        #region CargarDatos
        private void CargarDatos()
        {
            try
            {
                _usuariosList = Ctrl_LocationStaffAssignments.GetAvailableUsersForLocation(
                    _locationId, _ultimoTexto, paginaActual, PAGE_SIZE);

                totalRegistros = Ctrl_LocationStaffAssignments.CountAvailableUsersForLocation(
                    _locationId, _ultimoTexto);

                AsignarDataSource(_usuariosList);
                ActualizarInfoPaginacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR USUARIOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AsignarDataSource(List<Mdl_LocationStaffAssignments> lista)
        {
            Tabla.DataSource = null;
            Tabla.DataSource = lista;

            if (Tabla.Columns.Count == 0) return;

            foreach (DataGridViewColumn col in Tabla.Columns)
                col.Visible = false;

            MostrarColumna("EmployeeCode", "CÓDIGO", 15);
            MostrarColumna("FullName", "NOMBRE DEL USUARIO", 35);
            MostrarColumna("InstitutionalEmail", "CORREO INSTITUCIONAL", 30);
            MostrarColumna("Phone", "TELÉFONO", 20);
        }

        private void MostrarColumna(string col, string header, int weight)
        {
            if (!Tabla.Columns.Contains(col)) return;
            Tabla.Columns[col].Visible = true;
            Tabla.Columns[col].HeaderText = header;
            Tabla.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns[col].FillWeight = weight;
        }
        #endregion CargarDatos

        #region BuscarUsuario
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                _ultimoTexto = Txt_ValorBuscado.Text == "BUSCAR USUARIO..."
                    ? "" : Txt_ValorBuscado.Text.Trim();

                paginaActual = 1;
                CargarDatos();

                if (_usuariosList.Count == 0)
                    MessageBox.Show("No se encontraron usuarios disponibles con ese criterio.",
                        "BÚSQUEDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR EN BÚSQUEDA: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }
        #endregion BuscarUsuario

        #region Paginacion
        private void CrearToolStripPaginacion()
        {
            lblPaginacion = new Label();
            lblPaginacion.AutoSize = true;
            lblPaginacion.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPaginacion.Location = new Point(4, 4);
            lblPaginacion.Text = "MOSTRANDO 0-0 DE 0 USUARIOS";

            toolStripPaginacion = new ToolStrip();
            toolStripPaginacion.GripStyle = ToolStripGripStyle.Hidden;
            toolStripPaginacion.RenderMode = ToolStripRenderMode.System;

            var btnAnterior = new ToolStripButton("◄ ANTERIOR");
            btnAnterior.Click += (s, e) =>
            {
                if (paginaActual > 1) { paginaActual--; CargarDatos(); }
            };

            var btnSiguiente = new ToolStripButton("SIGUIENTE ►");
            btnSiguiente.Click += (s, e) =>
            {
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)PAGE_SIZE);
                if (paginaActual < totalPaginas) { paginaActual++; CargarDatos(); }
            };

            toolStripPaginacion.Items.Add(btnAnterior);
            toolStripPaginacion.Items.Add(btnSiguiente);
            toolStripPaginacion.Location = new Point(PanelTabla.Width - 260, 2);

            var panelPaginacion = new Panel();
            panelPaginacion.Dock = DockStyle.Bottom;
            panelPaginacion.Height = 32;
            panelPaginacion.BackColor = Color.WhiteSmoke;
            panelPaginacion.Controls.Add(lblPaginacion);
            panelPaginacion.Controls.Add(toolStripPaginacion);

            PanelTabla.Controls.Add(panelPaginacion);
        }

        private void ActualizarInfoPaginacion()
        {
            int desde = totalRegistros == 0 ? 0 : ((paginaActual - 1) * PAGE_SIZE) + 1;
            int hasta = Math.Min(paginaActual * PAGE_SIZE, totalRegistros);
            lblPaginacion.Text = $"MOSTRANDO {desde}-{hasta} DE {totalRegistros} USUARIOS";
        }
        #endregion Paginacion

        #region SeleccionarUsuario
        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;

            var row = Tabla.SelectedRows[0];
            _userIdSeleccionado = Convert.ToInt32(row.Cells["UserId"].Value);
            _nombreSeleccionado = row.Cells["FullName"].Value?.ToString() ?? "";
            Txt_Beneficiario.Text = _nombreSeleccionado;
        }
        #endregion SeleccionarUsuario

        #region BotonesAccion
        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (_userIdSeleccionado == 0)
            {
                MessageBox.Show("Debe seleccionar un usuario.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_frmPadre != null && !_frmPadre.IsDisposed)
                _frmPadre.SetUsuarioSeleccionado(_userIdSeleccionado, _nombreSeleccionado);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR USUARIO...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_Beneficiario.Clear();
            _userIdSeleccionado = 0;
            _ultimoTexto = "";
            paginaActual = 1;
            CargarDatos();
        }
        #endregion BotonesAccion
    }
}