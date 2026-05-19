using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_FixedAsset_TransferStatusTransitions : Form
    {
        #region PropiedadesIniciales

        private int _selectedTransitionId = 0;
        public int FromStatusId { get; set; }
        public string FromStatusName { get; set; }
        public bool IsFinalStatus { get; set; }
        public Mdl_Security_UserInfo UserData { get; set; }

        #endregion

        #region Constructor

        public Frm_FixedAsset_TransferStatusTransitions()
        {
            InitializeComponent();
        }

        #endregion

        #region Load

        private void Frm_FixedAsset_TransferStatusTransitions_Load(object sender, EventArgs e)
        {
            ConfigurarTabla();
            ConfigurarComboBoxBuscarPor();
            CargarComboToStatus();
            CargarTransiciones();
            EstadoInicial();

            // Mostrar estado origen en el campo readonly
            Txt_FromStatus.Text = FromStatusName;
            Txt_FromStatus.ReadOnly = true;

            // Si el estado origen es final, no puede tener transiciones de salida
            if (IsFinalStatus)
            {
                Btn_Save.Enabled = false;
                ComboBox_ToStatus.Enabled = false;
                MessageBox.Show(
                    "Este es un estado final. No puede tener transiciones de salida.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Configuración

        private void ConfigurarComboBoxBuscarPor()
        {
            ComboBox_BuscarPor.Items.Clear();
            ComboBox_BuscarPor.Items.Add("POR ESTADO ORIGEN");
            ComboBox_BuscarPor.Items.Add("POR ESTADO DESTINO");
            ComboBox_BuscarPor.SelectedIndex = 0;
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

            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "ID", DataPropertyName = "TransitionId", Visible = false });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFromId", HeaderText = "FROM ID", DataPropertyName = "FromStatusId", Visible = false });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFromCodigo", HeaderText = "CÓDIGO ORIGEN", DataPropertyName = "FromStatusCode", Width = 130 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFromNombre", HeaderText = "ESTADO ORIGEN", DataPropertyName = "FromStatusName", Width = 160 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colToCodigo", HeaderText = "CÓDIGO DESTINO", DataPropertyName = "ToStatusCode", Width = 130 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colToNombre", HeaderText = "ESTADO DESTINO", DataPropertyName = "ToStatusName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        }

        private void CargarComboToStatus()
        {
            ComboBox_ToStatus.DataSource = null;
            List<KeyValuePair<int, string>> estados =
                Ctrl_FixedAssetTransferStatus.ObtenerEstadosParaCombo(soloActivos: true);

            // Quitar el estado origen de las opciones
            estados.RemoveAll(e => e.Key == FromStatusId);

            ComboBox_ToStatus.DataSource = estados;
            ComboBox_ToStatus.DisplayMember = "Value";
            ComboBox_ToStatus.ValueMember = "Key";
        }

        private void EstadoInicial()
        {
            _selectedTransitionId = 0;

            if (ComboBox_ToStatus.Items.Count > 0)
                ComboBox_ToStatus.SelectedIndex = 0;

            Btn_Save.Visible = true;
            Btn_DELETE.Visible = false;
            Btn_Yes.Visible = false;
            Btn_No.Visible = false;
        }

        #endregion

        #region Carga de datos

        private void CargarTransiciones()
        {
            try
            {
                List<Mdl_FixedAssetTransferStatusTransition> lista =
                    Ctrl_FixedAssetTransferStatusTransitions.MostrarTransiciones(
                        fromStatusId: FromStatusId);

                Tabla.DataSource = null;
                Tabla.DataSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar transiciones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Búsqueda

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string valor = Txt_ValorBuscado.Text.Trim().ToUpper();
            string filtro = ComboBox_BuscarPor.SelectedItem?.ToString();

            // Filtro en memoria sobre la lista ya cargada del estado origen
            List<Mdl_FixedAssetTransferStatusTransition> lista =
                Ctrl_FixedAssetTransferStatusTransitions.MostrarTransiciones(fromStatusId: FromStatusId);

            if (!string.IsNullOrWhiteSpace(valor))
            {
                if (filtro == "POR ESTADO ORIGEN")
                    lista = lista.FindAll(t => t.FromStatusName.Contains(valor));
                else
                    lista = lista.FindAll(t => t.ToStatusName.Contains(valor));
            }

            Tabla.DataSource = null;
            Tabla.DataSource = lista;
        }

        private void Btn_ClearSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Clear();
            CargarTransiciones();
        }

        #endregion

        #region Selección en tabla

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = Tabla.Rows[e.RowIndex];
            _selectedTransitionId = Convert.ToInt32(row.Cells["colId"].Value);

            Btn_Save.Visible = false;
            Btn_DELETE.Visible = true;
            Btn_Yes.Visible = false;
            Btn_No.Visible = false;
        }

        #endregion

        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (ComboBox_ToStatus.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un estado destino.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int toStatusId = (int)ComboBox_ToStatus.SelectedValue;

            int resultado = Ctrl_FixedAssetTransferStatusTransitions.RegistrarTransicion(
                FromStatusId, toStatusId, UserData?.UserId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Transición registrada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarTransiciones();
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("El estado origen y destino no pueden ser iguales.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("El estado origen no es válido o está inactivo.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -3:
                    MessageBox.Show("El estado destino no es válido o está inactivo.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -4:
                    MessageBox.Show("Un estado final no puede tener transiciones de salida.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -5:
                    MessageBox.Show("Esta transición ya existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al registrar la transición.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_DELETE_Click(object sender, EventArgs e)
        {
            if (_selectedTransitionId == 0) return;

            Btn_DELETE.Visible = false;
            Btn_Yes.Visible = true;
            Btn_No.Visible = true;
        }

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            int resultado = Ctrl_FixedAssetTransferStatusTransitions.EliminarTransicion(
                _selectedTransitionId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Transición eliminada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarTransiciones();
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("La transición no existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al eliminar la transición.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            Btn_Yes.Visible = false;
            Btn_No.Visible = false;
            Btn_DELETE.Visible = true;
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            EstadoInicial();
        }

        #endregion
    }
}