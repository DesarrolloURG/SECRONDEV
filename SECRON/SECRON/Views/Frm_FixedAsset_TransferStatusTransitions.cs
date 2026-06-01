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
                DataPropertyName = "TransitionId",
                Visible = false
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAnterior",
                HeaderText = "ESTADO ANTERIOR",
                Width = 280
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colActual",
                HeaderText = "ESTADO ACTUAL",
                Width = 280
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSiguiente",
                HeaderText = "ESTADO SIGUIENTE",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
        }

        private void ConfigurarComboBoxes()
        {
            ComboBox_StatusSelected.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_FromStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_ToStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void EstadoInicial()
        {
            _selectedTransitionId = 0;

            Btn_Save.Enabled = true;
            Btn_Delete.Enabled = false;
            Btn_Yes.Enabled = false;
            Btn_No.Enabled = false;

            // Btn_Yes y Btn_No solo se habilitan durante confirmación de eliminación
            Btn_Yes.Visible = false;
            Btn_No.Visible = false;

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

            ComboBox_StatusSelected.DataSource = new List<KeyValuePair<int, string>>(estados);
            ComboBox_StatusSelected.DisplayMember = "Value";
            ComboBox_StatusSelected.ValueMember = "Key";

            ComboBox_FromStatus.DataSource = null;
            ComboBox_ToStatus.DataSource = null;
        }

        private void RefrescarCombosFromTo()
        {
            if (ComboBox_StatusSelected.SelectedItem == null) return;

            int statusSelectedId = ((KeyValuePair<int, string>)ComboBox_StatusSelected.SelectedItem).Key;

            List<KeyValuePair<int, string>> todos =
                Ctrl_FixedAssetTransferStatus.ObtenerEstadosParaCombo(soloActivos: true);

            // Excluir el estado actualmente seleccionado de ambos combos
            List<KeyValuePair<int, string>> opciones = todos.FindAll(e => e.Key != statusSelectedId);

            // Agregar opción "NO HAY" al inicio para estados de inicio/fin
            opciones.Insert(0, new KeyValuePair<int, string>(0, "— NO HAY —"));

            ComboBox_FromStatus.DataSource = new List<KeyValuePair<int, string>>(opciones);
            ComboBox_FromStatus.DisplayMember = "Value";
            ComboBox_FromStatus.ValueMember = "Key";

            ComboBox_ToStatus.DataSource = new List<KeyValuePair<int, string>>(opciones);
            ComboBox_ToStatus.DisplayMember = "Value";
            ComboBox_ToStatus.ValueMember = "Key";

            ComboBox_FromStatus.SelectedIndex = 0;
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

            if (ComboBox_StatusSelected.SelectedItem == null) return;

            int statusSelectedId =
                ((KeyValuePair<int, string>)ComboBox_StatusSelected.SelectedItem).Key;
            string statusSelectedName =
                ((KeyValuePair<int, string>)ComboBox_StatusSelected.SelectedItem).Value;

            try
            {
                // Salidas: StatusSelected es origen → conocemos el siguiente
                List<Mdl_FixedAssetTransferStatusTransition> salidas =
                    Ctrl_FixedAssetTransferStatusTransitions.MostrarTransiciones(
                        fromStatusId: statusSelectedId);

                // Entradas: StatusSelected es destino → conocemos el anterior
                List<Mdl_FixedAssetTransferStatusTransition> entradas =
                    Ctrl_FixedAssetTransferStatusTransitions.MostrarTransiciones(
                        toStatusId: statusSelectedId);

                // Pintar salidas — buscar si existe entrada que le dé un "anterior"
                foreach (var t in salidas)
                {
                    string anterior = string.Empty;
                    var entradaRelacionada = entradas.Find(en => en.ToStatusId == t.FromStatusId);
                    if (entradaRelacionada != null)
                        anterior = entradaRelacionada.FromStatusName;

                    Tabla.Rows.Add(t.TransitionId, anterior, statusSelectedName, t.ToStatusName);
                }

                // Pintar entradas que no hayan sido ya registradas como salida
                foreach (var t in entradas)
                {
                    bool yaRegistrada = false;
                    foreach (DataGridViewRow row in Tabla.Rows)
                    {
                        if (Convert.ToInt32(row.Cells["colId"].Value) == t.TransitionId)
                        {
                            yaRegistrada = true;
                            break;
                        }
                    }

                    if (!yaRegistrada)
                        Tabla.Rows.Add(t.TransitionId, t.FromStatusName, statusSelectedName, string.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar transiciones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Validaciones

        private bool ValidarReglasDeNegocio(int fromStatusId, int toStatusId, int statusSelectedId)
        {
            // Cargar todas las transiciones existentes para validar
            List<Mdl_FixedAssetTransferStatusTransition> todas =
                Ctrl_FixedAssetTransferStatusTransitions.MostrarTransiciones();

            // ── Regla 1: Solo puede existir UN estado de inicio (FromStatusId = 0) ──
            if (fromStatusId == 0)
            {
                bool yaExisteInicio = todas.Exists(t => t.FromStatusId == 0);
                if (yaExisteInicio)
                {
                    MessageBox.Show(
                        "Ya existe un estado de inicio en el flujo.\n" +
                        "Solo puede haber un estado inicial (sin estado anterior).",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // ── Regla 3a: Si StatusSelected ya es intermedio, no puede ser inicio ──
            if (fromStatusId == 0)
            {
                bool esIntermedio = todas.Exists(t =>
                    t.FromStatusId != 0 && t.ToStatusId != 0 &&
                    (t.FromStatusId == statusSelectedId || t.ToStatusId == statusSelectedId));

                if (esIntermedio)
                {
                    MessageBox.Show(
                        "Este estado ya participa como estado intermedio en el flujo.\n" +
                        "No puede asignarse también como estado inicial.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // ── Regla 3b: Si StatusSelected ya es intermedio, no puede ser final ──
            if (toStatusId == 0)
            {
                bool esIntermedio = todas.Exists(t =>
                    t.FromStatusId != 0 && t.ToStatusId != 0 &&
                    (t.FromStatusId == statusSelectedId || t.ToStatusId == statusSelectedId));

                if (esIntermedio)
                {
                    MessageBox.Show(
                        "Este estado ya participa como estado intermedio en el flujo.\n" +
                        "No puede asignarse también como estado final.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // ── Regla 3c: Un estado de inicio no puede ser también final ──
            if (fromStatusId == 0 && toStatusId == 0)
            {
                MessageBox.Show(
                    "Un estado no puede ser inicio y final al mismo tiempo.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // ── Regla 3d: Si StatusSelected ya tiene rol de inicio, no puede tener rol de final ──
            if (toStatusId == 0)
            {
                bool yaEsInicio = todas.Exists(t => t.FromStatusId == 0 &&
                    t.ToStatusId == statusSelectedId);
                if (yaEsInicio)
                {
                    MessageBox.Show(
                        "Este estado ya está registrado como estado inicial del flujo.\n" +
                        "No puede ser también un estado final.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // ── Regla 3e: Si StatusSelected ya tiene rol de final, no puede tener rol de inicio ──
            if (fromStatusId == 0)
            {
                bool yaEsFinal = todas.Exists(t => t.ToStatusId == 0 &&
                    t.FromStatusId == statusSelectedId);
                if (yaEsFinal)
                {
                    MessageBox.Show(
                        "Este estado ya está registrado como estado final del flujo.\n" +
                        "No puede ser también un estado inicial.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region EventosCombos

        private void ComboBox_StatusSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefrescarCombosFromTo();
            CargarTransiciones();

            _selectedTransitionId = 0;
            Btn_Delete.Enabled = false;
            Btn_Yes.Visible = false;
            Btn_No.Visible = false;
        }

        #endregion

        #region SeleccionEnTabla

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = Tabla.Rows[e.RowIndex];
            _selectedTransitionId = Convert.ToInt32(row.Cells["colId"].Value);

            Btn_Delete.Enabled = true;
            Btn_Yes.Visible = false;
            Btn_No.Visible = false;
        }

        #endregion

        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (ComboBox_StatusSelected.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar el estado a gestionar.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ComboBox_FromStatus.SelectedItem == null || ComboBox_ToStatus.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar el estado anterior y el estado siguiente.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int statusSelectedId = ((KeyValuePair<int, string>)ComboBox_StatusSelected.SelectedItem).Key;
            int fromStatusId = ((KeyValuePair<int, string>)ComboBox_FromStatus.SelectedItem).Key;
            int toStatusId = ((KeyValuePair<int, string>)ComboBox_ToStatus.SelectedItem).Key;

            // Ambos NO HAY no tiene sentido
            if (fromStatusId == 0 && toStatusId == 0)
            {
                MessageBox.Show("No puede seleccionar '— NO HAY —' en ambos campos a la vez.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // From y To no pueden ser el mismo estado real
            if (fromStatusId != 0 && fromStatusId == toStatusId)
            {
                MessageBox.Show("El estado anterior y el estado siguiente no pueden ser el mismo.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar reglas de negocio del flujo
            if (!ValidarReglasDeNegocio(fromStatusId, toStatusId, statusSelectedId))
                return;

            // El INSERT siempre usa From y To reales;
            // si FromStatus = NO HAY (0), el origen es StatusSelected
            // si ToStatus   = NO HAY (0), el destino es StatusSelected
            int insertFrom = fromStatusId == 0 ? statusSelectedId : fromStatusId;
            int insertTo = toStatusId == 0 ? statusSelectedId : toStatusId;

            int resultado = Ctrl_FixedAssetTransferStatusTransitions.RegistrarTransicion(
                insertFrom, insertTo, UserData?.UserId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Transición registrada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarTransiciones();
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

            // Confirmación inline — solo habilitar/deshabilitar, nunca Visible = false en botones principales
            Btn_Save.Enabled = false;
            Btn_Delete.Enabled = false;
            Btn_Yes.Visible = true;
            Btn_No.Visible = true;
            Btn_Yes.Enabled = true;
            Btn_No.Enabled = true;
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
                    _selectedTransitionId = 0;
                    Btn_Save.Enabled = true;
                    Btn_Delete.Enabled = false;
                    Btn_Yes.Visible = false;
                    Btn_No.Visible = false;
                    break;
                case -1:
                    MessageBox.Show("La transición no existe o ya fue eliminada.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    CargarTransiciones();
                    _selectedTransitionId = 0;
                    Btn_Save.Enabled = true;
                    Btn_Delete.Enabled = false;
                    Btn_Yes.Visible = false;
                    Btn_No.Visible = false;
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al eliminar la transición.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Btn_Save.Enabled = true;
                    Btn_Delete.Enabled = true;
                    Btn_Yes.Visible = false;
                    Btn_No.Visible = false;
                    break;
            }
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            // Cancelar confirmación — restaurar estado de botones
            Btn_Save.Enabled = true;
            Btn_Delete.Enabled = true;
            Btn_Yes.Visible = false;
            Btn_No.Visible = false;
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            EstadoInicial();
        }

        #endregion
    }
}