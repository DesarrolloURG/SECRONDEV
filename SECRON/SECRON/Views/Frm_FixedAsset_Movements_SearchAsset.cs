using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_FixedAsset_Movements_SearchAsset : Form
    {
        #region Propiedades

        // Propiedad que el formulario padre leerá después de ShowDialog()
        public Mdl_FixedAsset AssetSeleccionado { get; private set; }

        // Usuario en sesión
        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_FixedAsset> _activosList;

        #endregion

        #region Constructor

        public Frm_FixedAsset_Movements_SearchAsset()
        {
            InitializeComponent();
            ConfigurarTamañoFormulario();
        }

        #endregion

        #region Load

        private void Frm_FixedAsset_Movements_SearchAsset_Load(object sender, EventArgs e)
        {
            ConfigurarTabla();
            ConfigurarFiltros();
            CargarActivos();
            InhabilitarTextBox();

            // Eventos de botones
            Btn_Yes.Click += Btn_Yes_Click;
            Btn_No.Click += Btn_No_Click;
            Btn_Search.Click += Btn_Search_Click;
            Btn_Clear.Click += Btn_Clear_Click;
            Tabla.CellClick += Tabla_CellClick;
            Txt_ValorBuscado.KeyDown += Txt_ValorBuscado_KeyDown;
        }

        #endregion

        #region Configuración
        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(700, 650);
            this.MinimumSize = new Size(700, 650);
            this.MaximumSize = new Size(700, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }
        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "ID", DataPropertyName = "AssetId", Visible = false });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "CÓDIGO", DataPropertyName = "AssetCode", Width = 110 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "NOMBRE", DataPropertyName = "AssetName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCategoria", HeaderText = "CATEGORÍA", DataPropertyName = "CategoryName", Width = 140 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBodega", HeaderText = "BODEGA ACTUAL", DataPropertyName = "WarehouseName", Width = 140 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEmpleado", HeaderText = "EMPLEADO ACTUAL", DataPropertyName = "EmployeeName", Width = 150 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "ESTADO", DataPropertyName = "AssetStatus", Width = 100 });
        }

        private void ConfigurarFiltros()
        {
            ComboBox_BuscarPor.Items.Clear();
            ComboBox_BuscarPor.Items.Add("POR CÓDIGO");
            ComboBox_BuscarPor.Items.Add("POR NOMBRE");
            ComboBox_BuscarPor.Items.Add("POR TODOS");
            ComboBox_BuscarPor.SelectedIndex = 0;
            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;

            Txt_Selected.ReadOnly = true;
            Btn_Yes.Enabled = false;
        }
        private void InhabilitarTextBox()
        {
            Txt_Selected.Enabled = false;
        }

        #endregion

        #region Carga de datos

        private void CargarActivos(string textoBusqueda = "", string filtro = "POR TODOS")
        {
            try
            {
                _activosList = Ctrl_FixedAssets.BuscarActivos(
                    textoBusqueda: textoBusqueda,
                    filtro1: filtro);

                Tabla.DataSource = null;
                Tabla.DataSource = _activosList;

                // Limpiar selección al recargar
                Txt_Selected.Clear();
                AssetSeleccionado = null;
                Btn_Yes.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar activos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Eventos tabla

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = Tabla.Rows[e.RowIndex];
            int assetId = Convert.ToInt32(row.Cells["colId"].Value);

            // Buscar el activo en la lista
            AssetSeleccionado = _activosList?.Find(a => a.AssetId == assetId);

            if (AssetSeleccionado != null)
            {
                Txt_Selected.Text = $"{AssetSeleccionado.AssetCode} - {AssetSeleccionado.AssetName}";
                Btn_Yes.Enabled = true;
            }
        }

        #endregion

        #region Búsqueda

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string valor = Txt_ValorBuscado.Text.Trim().ToUpper();
            string filtro = ComboBox_BuscarPor.SelectedItem?.ToString() ?? "POR TODOS";
            CargarActivos(textoBusqueda: valor, filtro: filtro);
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Clear();
            ComboBox_BuscarPor.SelectedIndex = 0;
            CargarActivos();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Btn_Search_Click(sender, e);
        }

        #endregion

        #region Botones Aceptar / Cancelar

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (AssetSeleccionado == null)
            {
                MessageBox.Show("DEBE SELECCIONAR UN ACTIVO DE LA LISTA.",
                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            AssetSeleccionado = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion
    }
}