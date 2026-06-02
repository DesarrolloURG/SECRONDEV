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
            ConfigurarTamañoFormulario();
            ConfigurarTabla();
            ConfigurarComboBoxes();
            CargarCombos();
            EstadoInicial();
        }

        #endregion

        #region TamañoFormulario

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new System.Drawing.Size(984, 650);
            this.MinimumSize = new System.Drawing.Size(984, 650);
            this.MaximumSize = new System.Drawing.Size(984, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        #endregion

        #region Configuración

        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                HeaderText = "ID",
                Visible = false
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colActual",
                HeaderText = "ESTADO ACTUAL",
                Width = 450
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSiguiente",
                HeaderText = "ESTADO QUE LE SIGUE",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
        }

        private void ConfigurarComboBoxes()
        {
            ComboBox_StatusSelected.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_ToStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void EstadoInicial()
        {
            _selectedTransitionId = 0;
            Btn_Save.Enabled = true;
            Btn_Delete.Enabled = false;

            // Si viene un estado preseleccionado desde el padre, seleccionarlo
            if (FromStatusId > 0)
                SeleccionarComboById(ComboBox_StatusSelected, FromStatusId);

            CargarTransiciones();
        }

        #endregion

        #region CargaDeCombos

        private void CargarCombos()
        {
            List<KeyValuePair<int, string>> estados =
                Ctrl_FixedAssetTransferStatus.ObtenerEstadosParaCombo(soloActivos: true);

            // StatusSelected (FromStatus) — todos los estados activos
            ComboBox_StatusSelected.DataSource = new List<KeyValuePair<int, string>>(estados);
            ComboBox_StatusSelected.DisplayMember = "Value";
            ComboBox_StatusSelected.ValueMember = "Key";

            // ToStatus se refresca al seleccionar StatusSelected
            ComboBox_ToStatus.DataSource = null;
        }

        private void RefrescarComboToStatus()
        {
            if (ComboBox_StatusSelected.SelectedItem == null) return;

            int fromStatusId =
                ((KeyValuePair<int, string>)ComboBox_StatusSelected.SelectedItem).Key;

            List<KeyValuePair<int, string>> todos =
                Ctrl_FixedAssetTransferStatus.ObtenerEstadosParaCombo(soloActivos: true);

            // Excluir el estado actual del combo de siguiente
            List<KeyValuePair<int, string>> opciones =
                todos.FindAll(e => e.Key != fromStatusId);

            ComboBox_ToStatus.DataSource = new List<KeyValuePair<int, string>>(opciones);
            ComboBox_ToStatus.DisplayMember = "Value";
            ComboBox_ToStatus.ValueMember = "Key";

            if (ComboBox_ToStatus.Items.Count > 0)
                ComboBox_ToStatus.SelectedIndex = 0;
        }

        private void SeleccionarComboById(ComboBox combo, int id)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is KeyValuePair<int, string> item && item.Key == id)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        #endregion

        #region CargaDeDatos

        private void CargarTransiciones()
        {
            Tabla.Rows.Clear();

            try
            {
                // Cargar TODAS las transiciones existentes sin filtro
                List<Mdl_FixedAssetTransferStatusTransition> todas =
                    Ctrl_FixedAssetTransferStatusTransitions.MostrarTransiciones();

                foreach (var t in todas)
                    Tabla.Rows.Add(t.TransitionId, t.FromStatusName, t.ToStatusName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar transiciones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region EventosCombos

        private void ComboBox_StatusSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Solo refrescar el combo ToStatus — la tabla no se toca
            RefrescarComboToStatus();

            // Limpiar selección de tabla
            _selectedTransitionId = 0;
            Btn_Delete.Enabled = false;
        }

        #endregion

        #region SeleccionEnTabla

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = Tabla.Rows[e.RowIndex];
            _selectedTransitionId = Convert.ToInt32(row.Cells["colId"].Value);
            Btn_Delete.Enabled = true;
        }

        #endregion

        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (ComboBox_StatusSelected.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar el estado actual.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ComboBox_ToStatus.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar el estado que le sigue.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int fromStatusId = ((KeyValuePair<int, string>)ComboBox_StatusSelected.SelectedItem).Key;
            int toStatusId = ((KeyValuePair<int, string>)ComboBox_ToStatus.SelectedItem).Key;

            // ── Caso: ToStatus = NO HAY → informar y no hacer nada ──
            if (toStatusId == 0)
            {
                MessageBox.Show(
                    "Un estado final no genera una transición de salida.\n" +
                    "Si desea marcar un estado como final, edítelo desde el módulo de Estados de Traslado.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            

            int resultado = Ctrl_FixedAssetTransferStatusTransitions.RegistrarTransicion(
                fromStatusId, toStatusId, UserData?.UserId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Transición registrada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarTransiciones();
                    break;
                case -1:
                    MessageBox.Show("El estado actual y el estado siguiente no pueden ser iguales.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("El estado actual no es válido o está inactivo.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -3:
                    MessageBox.Show("El estado siguiente no es válido o está inactivo.", "Aviso",
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

            string descripcion = string.Empty;
            if (Tabla.CurrentRow != null)
            {
                string actual = Tabla.CurrentRow.Cells["colActual"].Value?.ToString();
                string siguiente = Tabla.CurrentRow.Cells["colSiguiente"].Value?.ToString();
                descripcion = $"{actual}  →  {siguiente}";
            }

            DialogResult confirmacion = MessageBox.Show(
                $"¿Está seguro de que desea eliminar la siguiente transición?\n\n{descripcion}\n\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes) return;

            int resultado = Ctrl_FixedAssetTransferStatusTransitions.EliminarTransicion(
                _selectedTransitionId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Transición eliminada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarTransiciones();
                    _selectedTransitionId = 0;
                    Btn_Delete.Enabled = false;
                    break;
                case -1:
                    MessageBox.Show("La transición no existe o ya fue eliminada.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    CargarTransiciones();
                    _selectedTransitionId = 0;
                    Btn_Delete.Enabled = false;
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al eliminar la transición.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        #endregion
    }
}