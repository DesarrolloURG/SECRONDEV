using SECRON.Controllers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_SearchEmployee : Form
    {
        #region PropiedadesIniciales
        private Frm_KARDEX_WarehouseDispatch _frmPadre;
        private int _employeeIdSeleccionado = 0;

        public Frm_KARDEX_SearchEmployee(Frm_KARDEX_WarehouseDispatch frmPadre)
        {
            InitializeComponent();
            _frmPadre = frmPadre;
            ConfigurarTamañoFormulario();
        }

        private void Frm_KARDEX_SearchEmployee_Load(object sender, EventArgs e)
        {
            ConfigurarComponentesDeshabilitados();
            ConfigurarPlaceHolder();
            ConfigurarTabla();
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
            Txt_ValorBuscado.Text = "BUSCAR EMPLEADO...";

            Txt_ValorBuscado.GotFocus += (s, e) =>
            {
                if (Txt_ValorBuscado.Text == "BUSCAR EMPLEADO...")
                {
                    Txt_ValorBuscado.Text = "";
                    Txt_ValorBuscado.ForeColor = Color.Black;
                }
            };

            Txt_ValorBuscado.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                {
                    Txt_ValorBuscado.Text = "BUSCAR EMPLEADO...";
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
                var empleados = Ctrl_Employees.MostrarEmpleados();
                AsignarDataSource(empleados);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR EMPLEADOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AsignarDataSource(System.Collections.Generic.List<Models.Mdl_Employees> lista)
        {
            Tabla.DataSource = null;
            Tabla.DataSource = lista;

            if (Tabla.Columns.Count == 0) return;

            foreach (DataGridViewColumn col in Tabla.Columns)
                col.Visible = false;

            if (Tabla.Columns.Contains("EmployeeId"))
                Tabla.Columns["EmployeeId"].Visible = false;

            if (Tabla.Columns.Contains("EmployeeCode"))
            {
                Tabla.Columns["EmployeeCode"].Visible = true;
                Tabla.Columns["EmployeeCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["EmployeeCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["EmployeeCode"].FillWeight = 20;
            }
            if (Tabla.Columns.Contains("FullName"))
            {
                Tabla.Columns["FullName"].Visible = true;
                Tabla.Columns["FullName"].HeaderText = "NOMBRE DEL EMPLEADO";
                Tabla.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["FullName"].FillWeight = 80;
            }
        }
        #endregion CargarDatos

        #region BuscarEmpleado
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                string texto = Txt_ValorBuscado.Text == "BUSCAR EMPLEADO..."
                    ? "" : Txt_ValorBuscado.Text.Trim();

                if (string.IsNullOrWhiteSpace(texto)) { CargarDatos(); return; }

                var resultados = Ctrl_Employees.BuscarEmpleados(texto);
                AsignarDataSource(resultados);

                if (resultados.Count == 0)
                    MessageBox.Show("No se encontraron empleados con ese criterio.",
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
        #endregion BuscarEmpleado

        #region SeleccionarEmpleado
        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;

            var row = Tabla.SelectedRows[0];
            _employeeIdSeleccionado = Convert.ToInt32(row.Cells["EmployeeId"].Value);
            Txt_Beneficiario.Text = row.Cells["FullName"].Value?.ToString() ?? "";
        }
        #endregion SeleccionarEmpleado

        #region BotonesAccion
        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (_employeeIdSeleccionado == 0)
            {
                MessageBox.Show("Debe seleccionar un empleado.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_frmPadre != null && !_frmPadre.IsDisposed)
                _frmPadre.ActualizarEmpleadoDestino(_employeeIdSeleccionado, Txt_Beneficiario.Text);

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
            Txt_ValorBuscado.Text = "BUSCAR EMPLEADO...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_Beneficiario.Clear();
            _employeeIdSeleccionado = 0;
            CargarDatos();
        }
        #endregion BotonesAccion
    }
}