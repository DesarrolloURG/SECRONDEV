using SECRON.Controllers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_FA_SearchEmployee : Form
    {
        #region PropiedadesIniciales

        private Frm_FixedAsset _frmPadre;

        public Frm_FA_SearchEmployee(Frm_FixedAsset frmPadre)
        {
            InitializeComponent();
            _frmPadre = frmPadre;
            ConfigurarTamañoFormulario();
        }

        private void Frm_FA_SearchEmployee_Load(object sender, EventArgs e)
        {
            ConfigurarComponentesDeshabilitados();
            ConfigurarPlaceHolder();
            ConfigurarTabla();
            CargarTodos();
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
            Txt_Codigo.Enabled = false;
            Txt_Cuenta.Enabled = false;
        }

        #endregion PropiedadesIniciales

        #region PlaceHolder

        private void ConfigurarPlaceHolder()
        {
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_ValorBuscado.Text = "INGRESE CÓDIGO O NOMBRE DEL EMPLEADO";

            Txt_ValorBuscado.GotFocus += (s, e) =>
            {
                if (Txt_ValorBuscado.Text == "INGRESE CÓDIGO O NOMBRE DEL EMPLEADO")
                { Txt_ValorBuscado.Text = ""; Txt_ValorBuscado.ForeColor = Color.Black; }
            };
            Txt_ValorBuscado.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                { Txt_ValorBuscado.Text = "INGRESE CÓDIGO O NOMBRE DEL EMPLEADO"; Txt_ValorBuscado.ForeColor = Color.Gray; }
            };
        }

        #endregion PlaceHolder

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
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(173, 216, 230);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            Tabla.EnableHeadersVisualStyles = false;

            Tabla.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            Tabla.RowTemplate.Height = 35;
            Tabla.ColumnHeadersHeight = 40;

            Tabla.SelectionChanged += Tabla_SelectionChanged;
            Tabla.CellBeginEdit += (s, e) => e.Cancel = true;
            Tabla.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete) e.Handled = true; };
        }

        #endregion ConfigurarTabla

        #region CargarDatos

        private void CargarTodos()
        {
            try
            {
                var lista = Ctrl_Employees.MostrarEmpleados(1, int.MaxValue);
                Tabla.DataSource = null;
                Tabla.DataSource = lista;

                ConfigurarColumnasTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR EMPLEADOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColumnasTabla()
        {
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
                Tabla.Columns["EmployeeCode"].FillWeight = 15;
            }

            // Mostrar FullName si existe, sino FirstName + LastName
            if (Tabla.Columns.Contains("FullName") && !string.IsNullOrWhiteSpace(
                Tabla.Rows.Count > 0 ? Tabla.Rows[0].Cells["FullName"].Value?.ToString() : "x"))
            {
                Tabla.Columns["FullName"].Visible = true;
                Tabla.Columns["FullName"].HeaderText = "NOMBRE COMPLETO";
                Tabla.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["FullName"].FillWeight = 85;
            }
            else
            {
                if (Tabla.Columns.Contains("FirstName"))
                {
                    Tabla.Columns["FirstName"].Visible = true;
                    Tabla.Columns["FirstName"].HeaderText = "NOMBRE";
                    Tabla.Columns["FirstName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    Tabla.Columns["FirstName"].FillWeight = 42;
                }
                if (Tabla.Columns.Contains("LastName"))
                {
                    Tabla.Columns["LastName"].Visible = true;
                    Tabla.Columns["LastName"].HeaderText = "APELLIDO";
                    Tabla.Columns["LastName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    Tabla.Columns["LastName"].FillWeight = 43;
                }
            }
        }

        #endregion CargarDatos

        #region Buscar

        private void Btn_SearchCuenta_Click(object sender, EventArgs e)
        {
            try
            {
                string texto = Txt_ValorBuscado.Text == "INGRESE CÓDIGO O NOMBRE DEL EMPLEADO"
                    ? "" : Txt_ValorBuscado.Text.Trim();

                if (string.IsNullOrWhiteSpace(texto)) { CargarTodos(); return; }

                var resultados = Ctrl_Employees.BuscarEmpleados(texto);
                Tabla.DataSource = null;
                Tabla.DataSource = resultados;
                ConfigurarColumnasTabla();

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
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_SearchCuenta_Click(sender, e); }
        }

        #endregion Buscar

        #region Seleccion

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;
            var row = Tabla.SelectedRows[0];

            Txt_Codigo.Text = row.Cells["EmployeeId"].Value?.ToString() ?? "";

            string fullName = "";

            if (Tabla.Columns.Contains("FullName"))
                fullName = row.Cells["FullName"].Value?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(fullName))
            {
                string first = Tabla.Columns.Contains("FirstName") ? row.Cells["FirstName"].Value?.ToString() ?? "" : "";
                string last = Tabla.Columns.Contains("LastName") ? row.Cells["LastName"].Value?.ToString() ?? "" : "";
                fullName = $"{first} {last}".Trim();
            }

            Txt_Cuenta.Text = fullName;
        }

        #endregion Seleccion

        #region BotonesAccion

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txt_Codigo.Text))
            {
                MessageBox.Show("Debe seleccionar un empleado.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_frmPadre != null && !_frmPadre.IsDisposed)
                _frmPadre.ActualizarEmpleado(int.Parse(Txt_Codigo.Text), Txt_Cuenta.Text);

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
            Txt_ValorBuscado.Text = "INGRESE CÓDIGO O NOMBRE DEL EMPLEADO";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_Codigo.Clear();
            Txt_Cuenta.Clear();
            CargarTodos();
        }

        #endregion BotonesAccion
    }
}