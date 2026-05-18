using SECRON.Controllers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_FA_SearchWarehouse : Form
    {
        #region PropiedadesIniciales

        private Frm_FixedAsset _frmPadre;

        public Frm_FA_SearchWarehouse(Frm_FixedAsset frmPadre)
        {
            InitializeComponent();
            _frmPadre = frmPadre;
            ConfigurarTamañoFormulario();
        }

        private void Frm_FA_SearchWarehouse_Load(object sender, EventArgs e)
        {
            ConfigurarComponentesDeshabilitados();
            ConfigurarPlaceHolder();
            ConfigurarTabla();
            CargarTodas();
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
            Txt_ValorBuscado.Text = "INGRESE CÓDIGO O NOMBRE DE BODEGA";

            Txt_ValorBuscado.GotFocus += (s, e) =>
            {
                if (Txt_ValorBuscado.Text == "INGRESE CÓDIGO O NOMBRE DE BODEGA")
                { Txt_ValorBuscado.Text = ""; Txt_ValorBuscado.ForeColor = Color.Black; }
            };
            Txt_ValorBuscado.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                { Txt_ValorBuscado.Text = "INGRESE CÓDIGO O NOMBRE DE BODEGA"; Txt_ValorBuscado.ForeColor = Color.Gray; }
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

        private void CargarTodas()
        {
            try
            {
                var lista = Ctrl_Warehouses.MostrarBodegas();
                Tabla.DataSource = null;
                Tabla.DataSource = lista;

                if (Tabla.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn col in Tabla.Columns)
                        col.Visible = false;

                    if (Tabla.Columns.Contains("WarehouseId"))
                        Tabla.Columns["WarehouseId"].Visible = false;

                    if (Tabla.Columns.Contains("WarehouseCode"))
                    {
                        Tabla.Columns["WarehouseCode"].Visible = true;
                        Tabla.Columns["WarehouseCode"].HeaderText = "CÓDIGO";
                        Tabla.Columns["WarehouseCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        Tabla.Columns["WarehouseCode"].FillWeight = 20;
                    }
                    if (Tabla.Columns.Contains("WarehouseName"))
                    {
                        Tabla.Columns["WarehouseName"].Visible = true;
                        Tabla.Columns["WarehouseName"].HeaderText = "BODEGA";
                        Tabla.Columns["WarehouseName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        Tabla.Columns["WarehouseName"].FillWeight = 80;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR BODEGAS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion CargarDatos

        #region Buscar

        private void Btn_SearchCuenta_Click(object sender, EventArgs e)
        {
            try
            {
                string texto = Txt_ValorBuscado.Text == "INGRESE CÓDIGO O NOMBRE DE BODEGA"
                    ? "" : Txt_ValorBuscado.Text.Trim();

                if (string.IsNullOrWhiteSpace(texto)) { CargarTodas(); return; }

                var resultados = Ctrl_Warehouses.BuscarBodegas(texto);
                Tabla.DataSource = null;
                Tabla.DataSource = resultados;

                if (Tabla.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn col in Tabla.Columns)
                        col.Visible = false;

                    if (Tabla.Columns.Contains("WarehouseId"))
                        Tabla.Columns["WarehouseId"].Visible = false;

                    if (Tabla.Columns.Contains("WarehouseCode"))
                    {
                        Tabla.Columns["WarehouseCode"].Visible = true;
                        Tabla.Columns["WarehouseCode"].HeaderText = "CÓDIGO";
                        Tabla.Columns["WarehouseCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        Tabla.Columns["WarehouseCode"].FillWeight = 20;
                    }
                    if (Tabla.Columns.Contains("WarehouseName"))
                    {
                        Tabla.Columns["WarehouseName"].Visible = true;
                        Tabla.Columns["WarehouseName"].HeaderText = "BODEGA";
                        Tabla.Columns["WarehouseName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        Tabla.Columns["WarehouseName"].FillWeight = 80;
                    }
                }

                if (resultados.Count == 0)
                    MessageBox.Show("No se encontraron bodegas con ese criterio.",
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
            Txt_Codigo.Text = row.Cells["WarehouseId"].Value?.ToString() ?? "";
            Txt_Cuenta.Text = row.Cells["WarehouseName"].Value?.ToString() ?? "";
        }

        #endregion Seleccion

        #region BotonesAccion

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txt_Codigo.Text))
            {
                MessageBox.Show("Debe seleccionar una bodega.", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_frmPadre != null && !_frmPadre.IsDisposed)
                _frmPadre.ActualizarBodega(int.Parse(Txt_Codigo.Text), Txt_Cuenta.Text);

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
            Txt_ValorBuscado.Text = "INGRESE CÓDIGO O NOMBRE DE BODEGA";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_Codigo.Clear();
            Txt_Cuenta.Clear();
            CargarTodas();
        }

        #endregion BotonesAccion
    }
}