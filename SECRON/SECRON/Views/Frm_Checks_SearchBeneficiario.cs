using SECRON.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Checks_SearchBeneficiario : Form
    {
        #region PropiedadesIniciales
        private Frm_Checks_Managment _frmPadre;

        public Frm_Checks_SearchBeneficiario(Frm_Checks_Managment frmPadre)
        {
            InitializeComponent();
            _frmPadre = frmPadre;

            // Configurar tamaño de formulario
            ConfigurarTamañoFormulario();
        }

        private void Frm_Checks_SearchBeneficiario_Load(object sender, EventArgs e)
        {
            ConfigurarComponentesDeshabilitados();
            ConfigurarComboBox();
            ConfigurarPlaceHolder();
            ConfigurarTabla();
            CargarDatos(ComboBox_BuscarPor.SelectedItem.ToString()); 
        }
        // Configurar Medidas Formulario
        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(700, 650);           // Tamaño fijo
            this.MinimumSize = new Size(700, 650);    // Tamaño mínimo
            this.MaximumSize = new Size(700, 650);    // Tamaño máximo
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // No redimensionable
            this.StartPosition = FormStartPosition.CenterParent; // Centrado en el padre
            this.MaximizeBox = false;                 // Sin botón maximizar
        }
        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_Beneficiario.Enabled = false;
        }
        #endregion PropiedadesIniciales
        #region ConfigurarComboBox
        private void ConfigurarComboBox()
        {
            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_BuscarPor.Items.Clear();
            ComboBox_BuscarPor.Items.AddRange(new object[]
            {
                "PROVEEDORES - RAZÓN SOCIAL",
                "PROVEEDORES - NOMBRE COMERCIAL",
                "COLABORADORES",
                "DOCENCIA"
            });
            ComboBox_BuscarPor.SelectedIndex = 0;
            ComboBox_BuscarPor.SelectedIndexChanged += ComboBox_BuscarPor_SelectedIndexChanged;
        }
        #endregion ConfigurarComboBox
        #region EventoComboBox
        private void ComboBox_BuscarPor_SelectedIndexChanged(object sender, EventArgs e)
        {
            Txt_Beneficiario.Clear();
            CargarDatos(ComboBox_BuscarPor.SelectedItem.ToString());
        }
        #endregion EventoComboBox
        #region ConfigurarPlaceHolder
        private void ConfigurarPlaceHolder()
        {
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_ValorBuscado.Text = "BUSCAR BENEFICIARIO...";

            Txt_ValorBuscado.GotFocus += (s, e) =>
            {
                if (Txt_ValorBuscado.Text == "BUSCAR BENEFICIARIO...")
                {
                    Txt_ValorBuscado.Text = "";
                    Txt_ValorBuscado.ForeColor = Color.Black;
                }
            };

            Txt_ValorBuscado.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                {
                    Txt_ValorBuscado.Text = "BUSCAR BENEFICIARIO...";
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
        private void CargarDatos(string tipo)
        {
            try
            {
                Tabla.DataSource = null;

                if (tipo == "PROVEEDORES - RAZÓN SOCIAL")
                {
                    var proveedores = Ctrl_Suppliers.MostrarProveedores();
                    Tabla.DataSource = proveedores;

                    if (Tabla.Columns.Count > 0)
                    {
                        foreach (DataGridViewColumn col in Tabla.Columns)
                        {
                            col.Visible = false;
                        }

                        if (Tabla.Columns.Contains("LegalName"))
                        {
                            Tabla.Columns["LegalName"].Visible = true;
                            Tabla.Columns["LegalName"].HeaderText = "RAZÓN SOCIAL - PROVEEDOR";
                            Tabla.Columns["LegalName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }
                }
                else if (tipo == "PROVEEDORES - NOMBRE COMERCIAL")
                {
                    var proveedores = Ctrl_Suppliers.MostrarProveedores();
                    Tabla.DataSource = proveedores;

                    if (Tabla.Columns.Count > 0)
                    {
                        foreach (DataGridViewColumn col in Tabla.Columns)
                        {
                            col.Visible = false;
                        }

                        if (Tabla.Columns.Contains("SupplierName"))
                        {
                            Tabla.Columns["SupplierName"].Visible = true;
                            Tabla.Columns["SupplierName"].HeaderText = "NOMBRE COMERCIAL - PROVEEDOR";
                            Tabla.Columns["SupplierName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }
                }
                else if (tipo == "COLABORADORES")
                {
                    var empleados = Ctrl_Employees.MostrarEmpleados();
                    Tabla.DataSource = empleados;

                    if (Tabla.Columns.Count > 0)
                    {
                        foreach (DataGridViewColumn col in Tabla.Columns)
                        {
                            col.Visible = false;
                        }

                        if (Tabla.Columns.Contains("FullName"))
                        {
                            Tabla.Columns["FullName"].Visible = true;
                            Tabla.Columns["FullName"].HeaderText = "NOMBRE DEL COLABORADOR";
                            Tabla.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }
                }
                else if (tipo == "DOCENCIA")
                {
                    var docentes = Ctrl_Teachers.MostrarDocentes();
                    Tabla.DataSource = docentes;

                    if (Tabla.Columns.Count > 0)
                    {
                        foreach (DataGridViewColumn col in Tabla.Columns)
                        {
                            col.Visible = false;
                        }

                        if (Tabla.Columns.Contains("FullName"))
                        {
                            Tabla.Columns["FullName"].Visible = true;
                            Tabla.Columns["FullName"].HeaderText = "NOMBRE DEL DOCENTE";
                            Tabla.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR DATOS: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion CargarDatos
        #region BuscarBeneficiario
        // Evento click del botón Buscar
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text) ||
                    Txt_ValorBuscado.Text == "BUSCAR BENEFICIARIO...")
                {
                    CargarDatos(ComboBox_BuscarPor.SelectedItem.ToString());
                    return;
                }

                Tabla.DataSource = null;
                string tipo = ComboBox_BuscarPor.SelectedItem.ToString();

                if (tipo == "PROVEEDORES - RAZÓN SOCIAL")
                {
                    var resultados = Ctrl_Suppliers.BuscarProveedores(Txt_ValorBuscado.Text);
                    Tabla.DataSource = resultados;

                    if (Tabla.Columns.Count > 0)
                    {
                        foreach (DataGridViewColumn col in Tabla.Columns)
                        {
                            col.Visible = false;
                        }

                        if (Tabla.Columns.Contains("LegalName"))
                        {
                            Tabla.Columns["LegalName"].Visible = true;
                            Tabla.Columns["LegalName"].HeaderText = "RAZÓN SOCIAL - PROVEEDOR";
                            Tabla.Columns["LegalName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }
                }
                else if (tipo == "PROVEEDORES - NOMBRE COMERCIAL")
                {
                    var resultados = Ctrl_Suppliers.BuscarProveedores(Txt_ValorBuscado.Text);
                    Tabla.DataSource = resultados;

                    if (Tabla.Columns.Count > 0)
                    {
                        foreach (DataGridViewColumn col in Tabla.Columns)
                        {
                            col.Visible = false;
                        }

                        if (Tabla.Columns.Contains("SupplierName"))
                        {
                            Tabla.Columns["SupplierName"].Visible = true;
                            Tabla.Columns["SupplierName"].HeaderText = "NOMBRE COMERCIAL - PROVEEDOR";
                            Tabla.Columns["SupplierName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }
                }
                else if (tipo == "COLABORADORES")
                {
                    var resultados = Ctrl_Employees.BuscarEmpleados(Txt_ValorBuscado.Text);
                    Tabla.DataSource = resultados;

                    if (Tabla.Columns.Count > 0)
                    {
                        foreach (DataGridViewColumn col in Tabla.Columns)
                        {
                            col.Visible = false;
                        }

                        if (Tabla.Columns.Contains("FullName"))
                        {
                            Tabla.Columns["FullName"].Visible = true;
                            Tabla.Columns["FullName"].HeaderText = "NOMBRE DEL COLABORADOR";
                            Tabla.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }
                }
                else if (tipo == "DOCENCIA")
                {
                    var resultados = Ctrl_Teachers.BuscarPorNombre(Txt_ValorBuscado.Text);
                    Tabla.DataSource = resultados;

                    if (Tabla.Columns.Count > 0)
                    {
                        foreach (DataGridViewColumn col in Tabla.Columns)
                        {
                            col.Visible = false;
                        }

                        if (Tabla.Columns.Contains("FullName"))
                        {
                            Tabla.Columns["FullName"].Visible = true;
                            Tabla.Columns["FullName"].HeaderText = "NOMBRE DEL DOCENTE";
                            Tabla.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }
                }

                if (Tabla.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontraron resultados", "BÚSQUEDA",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR EN BÚSQUEDA: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Evento al presionar tecla Enter en el TextBox de búsqueda
        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }
        #endregion BuscarBeneficiario
        #region SeleccionarBeneficiario
        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count > 0)
            {
                var row = Tabla.SelectedRows[0];
                string tipo = ComboBox_BuscarPor.SelectedItem.ToString();

                if (tipo == "PROVEEDORES - RAZÓN SOCIAL")
                {
                    Txt_Beneficiario.Text = row.Cells["LegalName"].Value?.ToString() ?? "";
                }
                else if (tipo == "PROVEEDORES - NOMBRE COMERCIAL")
                {
                    Txt_Beneficiario.Text = row.Cells["SupplierName"].Value?.ToString() ?? "";
                }
                else if (tipo == "COLABORADORES" || tipo == "DOCENCIA")
                {
                    Txt_Beneficiario.Text = row.Cells["FullName"].Value?.ToString() ?? "";
                }
            }
        }
        #endregion SeleccionarBeneficiario
        #region BotonesAccion
        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txt_Beneficiario.Text))
            {
                MessageBox.Show("Debe seleccionar un beneficiario", "VALIDACIÓN",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_frmPadre != null && !_frmPadre.IsDisposed)
            {
                _frmPadre.ActualizarBeneficiario(Txt_Beneficiario.Text);
            }

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
            Txt_ValorBuscado.Text = "BUSCAR BENEFICIARIO...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_Beneficiario.Clear();
            CargarDatos(ComboBox_BuscarPor.SelectedItem.ToString());
        }
        #endregion BotonesAccion
    }
}
