using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_SearchSupplier : Form
    {
        #region PropiedadesIniciales

        private Frm_KARDEX_WarehouseInventory_Ingreso _frmPadre;
        private int _proveedorId = 0;

        public Frm_KARDEX_SearchSupplier(Frm_KARDEX_WarehouseInventory_Ingreso frmPadre)
        {
            InitializeComponent();
            _frmPadre = frmPadre;
            ConfigurarTamañoFormulario();
        }

        private void Frm_KARDEX_SearchSupplier_Load(object sender, EventArgs e)
        {
            ConfigurarComponentesDeshabilitados();
            ConfigurarComboBuscarPor();
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
            Txt_Cuenta.Enabled = false;
        }

        private void ConfigurarComboBuscarPor()
        {
            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_BuscarPor.Items.Clear();
            ComboBox_BuscarPor.Items.Add("POR NOMBRE");
            ComboBox_BuscarPor.Items.Add("POR RAZÓN SOCIAL");
            ComboBox_BuscarPor.Items.Add("POR ACTIVIDAD COMERCIAL");
            ComboBox_BuscarPor.SelectedIndex = 0;
        }

        #endregion PropiedadesIniciales

        #region PlaceHolder

        private void ConfigurarPlaceHolder()
        {
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_ValorBuscado.Text = "INGRESE EL VALOR A BUSCAR";

            Txt_ValorBuscado.GotFocus += (s, e) =>
            {
                if (Txt_ValorBuscado.Text == "INGRESE EL VALOR A BUSCAR")
                { Txt_ValorBuscado.Text = ""; Txt_ValorBuscado.ForeColor = Color.Black; }
            };
            Txt_ValorBuscado.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                { Txt_ValorBuscado.Text = "INGRESE EL VALOR A BUSCAR"; Txt_ValorBuscado.ForeColor = Color.Gray; }
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
                var lista = Ctrl_Suppliers.MostrarProveedores(1, int.MaxValue);
                AsignarDataSource(lista);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR PROVEEDORES: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AsignarDataSource(List<Mdl_Suppliers> lista)
        {
            Tabla.DataSource = null;
            Tabla.DataSource = lista;

            if (Tabla.Columns.Count == 0) return;

            foreach (DataGridViewColumn col in Tabla.Columns)
                col.Visible = false;

            if (Tabla.Columns.Contains("SupplierId"))
                Tabla.Columns["SupplierId"].Visible = false;

            if (Tabla.Columns.Contains("SupplierCode"))
            {
                Tabla.Columns["SupplierCode"].Visible = true;
                Tabla.Columns["SupplierCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["SupplierCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["SupplierCode"].FillWeight = 15;
            }
            if (Tabla.Columns.Contains("SupplierName"))
            {
                Tabla.Columns["SupplierName"].Visible = true;
                Tabla.Columns["SupplierName"].HeaderText = "NOMBRE";
                Tabla.Columns["SupplierName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["SupplierName"].FillWeight = 35;
            }
            if (Tabla.Columns.Contains("LegalName"))
            {
                Tabla.Columns["LegalName"].Visible = true;
                Tabla.Columns["LegalName"].HeaderText = "RAZÓN SOCIAL";
                Tabla.Columns["LegalName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["LegalName"].FillWeight = 35;
            }
            if (Tabla.Columns.Contains("CommercialActivity"))
            {
                Tabla.Columns["CommercialActivity"].Visible = true;
                Tabla.Columns["CommercialActivity"].HeaderText = "ACTIVIDAD COMERCIAL";
                Tabla.Columns["CommercialActivity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CommercialActivity"].FillWeight = 15;
            }
        }

        #endregion CargarDatos

        #region Buscar

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                string texto = Txt_ValorBuscado.Text == "INGRESE EL VALOR A BUSCAR"
                    ? "" : Txt_ValorBuscado.Text.Trim();

                if (string.IsNullOrWhiteSpace(texto)) { CargarTodos(); return; }

                string filtro = ComboBox_BuscarPor.SelectedItem?.ToString() ?? "NOMBRE";
                var resultados = Ctrl_Suppliers.BuscarProveedores(texto, filtro);

                AsignarDataSource(resultados);

                if (resultados.Count == 0)
                    MessageBox.Show("No se encontraron proveedores con ese criterio.",
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
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_Search_Click(sender, e); }
        }

        #endregion Buscar

        #region Seleccion

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;
            var row = Tabla.SelectedRows[0];
            _proveedorId = Convert.ToInt32(row.Cells["SupplierId"].Value);
            Txt_Cuenta.Text = row.Cells["SupplierName"].Value?.ToString() ?? "";
        }

        #endregion Seleccion

        #region BotonesAccion

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (_proveedorId == 0)
            {
                MessageBox.Show("Debe seleccionar un proveedor.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_frmPadre != null && !_frmPadre.IsDisposed)
                _frmPadre.ActualizarProveedor(_proveedorId, Txt_Cuenta.Text);

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
            Txt_ValorBuscado.Text = "INGRESE EL VALOR A BUSCAR";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_Cuenta.Clear();
            _proveedorId = 0;
            ComboBox_BuscarPor.SelectedIndex = 0;
            CargarTodos();
        }

        #endregion BotonesAccion
    }
}