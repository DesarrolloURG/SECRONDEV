using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_FixedAsset_StatusMovements : Form
    {
        #region PropiedadesIniciales

        private int _selectedStatusId = 0;
        private bool _modoEdicion = false;
        public Mdl_Security_UserInfo UserData { get; set; }

        #endregion

        #region Constructor

        public Frm_FixedAsset_StatusMovements()
        {
            InitializeComponent();
        }

        #endregion

        #region Load

        private void Frm_FixedAsset_StatusMovements_Load(object sender, EventArgs e)
        {
            ConfigurarComboBoxBuscarPor();
            ConfigurarTabla();
            CargarEstados();
            EstadoInicial();
        }

        #endregion

        #region Configuración

        private void ConfigurarComboBoxBuscarPor()
        {
            ComboBox_BuscarPor.Items.Clear();
            ComboBox_BuscarPor.Items.Add("POR CÓDIGO");
            ComboBox_BuscarPor.Items.Add("POR NOMBRE");
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

            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "ID", DataPropertyName = "TransferStatusId", Visible = false });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "CÓDIGO", DataPropertyName = "StatusCode", Width = 100 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "NOMBRE", DataPropertyName = "StatusName", Width = 180 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOrden", HeaderText = "ORDEN", DataPropertyName = "Order", Width = 70 });
            Tabla.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colFinal", HeaderText = "¿FINAL?", DataPropertyName = "IsFinal", Width = 70 });
            Tabla.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colActivo", HeaderText = "ACTIVO", DataPropertyName = "IsActive", Width = 70 });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescripcion", HeaderText = "DESCRIPCIÓN", DataPropertyName = "Description", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        }

        private void EstadoInicial()
        {
            _selectedStatusId = 0;
            _modoEdicion = false;

            Txt_StatusCode.Clear();
            Txt_StatusName.Clear();
            Txt_Description.Clear();
            Txt_Order.Clear();
            CheckBox_IsFinal.Checked = false;
            CheckBox_IsActive.Checked = true;
            Txt_Selected.Clear();

            // Visibilidad de botones
            Btn_Save.Visible = true;
            Btn_Update.Visible = false;
            Btn_Inactive.Visible = false;
            Btn_TransferStatusTransition.Visible = false;
            Btn_Yes.Visible = false;
            Btn_No.Visible = false;
            Lbl_Beneficiario.Visible = false;
            Txt_Selected.Visible = false;

            Txt_StatusCode.ReadOnly = _modoEdicion;
        }

        #endregion

        #region Carga de datos

        private void CargarEstados(string statusCode = null, string statusName = null)
        {
            try
            {
                List<Mdl_FixedAssetTransferStatus> lista = Ctrl_FixedAssetTransferStatus.MostrarEstados(
                    statusCode: statusCode,
                    statusName: statusName);

                Tabla.DataSource = null;
                Tabla.DataSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar estados: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Búsqueda

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string valor = Txt_ValorBuscado.Text.Trim().ToUpper();
            string filtro = ComboBox_BuscarPor.SelectedItem?.ToString();

            if (filtro == "POR CÓDIGO")
                CargarEstados(statusCode: valor);
            else
                CargarEstados(statusName: valor);
        }

        private void Btn_ClearSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Clear();
            CargarEstados();
        }

        #endregion

        #region Selección en tabla

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = Tabla.Rows[e.RowIndex];

            _selectedStatusId = Convert.ToInt32(row.Cells["colId"].Value);
            Txt_StatusCode.Text = row.Cells["colCodigo"].Value?.ToString();
            Txt_StatusName.Text = row.Cells["colNombre"].Value?.ToString();
            Txt_Order.Text = row.Cells["colOrden"].Value?.ToString();
            Txt_Description.Text = row.Cells["colDescripcion"].Value?.ToString();
            CheckBox_IsFinal.Checked = Convert.ToBoolean(row.Cells["colFinal"].Value);
            CheckBox_IsActive.Checked = Convert.ToBoolean(row.Cells["colActivo"].Value);
            Txt_Selected.Text = row.Cells["colNombre"].Value?.ToString();

            // Mostrar botones de acción
            Btn_Save.Visible = false;
            Btn_Update.Visible = true;
            Btn_Inactive.Visible = true;
            Btn_TransferStatusTransition.Visible = true;
            Btn_Yes.Visible = false;
            Btn_No.Visible = false;
            Lbl_Beneficiario.Visible = true;
            Txt_Selected.Visible = true;

            Txt_StatusCode.ReadOnly = true;
            _modoEdicion = true;
        }

        #endregion

        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            Mdl_FixedAssetTransferStatus estado = new Mdl_FixedAssetTransferStatus
            {
                StatusCode = Txt_StatusCode.Text.Trim().ToUpper(),
                StatusName = Txt_StatusName.Text.Trim().ToUpper(),
                Description = Txt_Description.Text.Trim().ToUpper(),
                Order = int.Parse(Txt_Order.Text.Trim()),
                IsFinal = CheckBox_IsFinal.Checked,
                CreatedBy = UserData?.UserId
            };

            int resultado = Ctrl_FixedAssetTransferStatus.RegistrarEstado(estado);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Estado registrado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarEstados();
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("El código de estado ya existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("El número de orden ya está en uso.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al registrar el estado.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            Mdl_FixedAssetTransferStatus estado = new Mdl_FixedAssetTransferStatus
            {
                TransferStatusId = _selectedStatusId,
                StatusCode = Txt_StatusCode.Text.Trim().ToUpper(),
                StatusName = Txt_StatusName.Text.Trim().ToUpper(),
                Description = Txt_Description.Text.Trim().ToUpper(),
                Order = int.Parse(Txt_Order.Text.Trim()),
                IsFinal = CheckBox_IsFinal.Checked,
                IsActive = CheckBox_IsActive.Checked,
                ModifiedBy = UserData?.UserId
            };

            int resultado = Ctrl_FixedAssetTransferStatus.ActualizarEstado(estado);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Estado actualizado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarEstados();
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("El estado no existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("El código de estado ya está en uso por otro registro.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -3:
                    MessageBox.Show("El número de orden ya está en uso por otro registro.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al actualizar el estado.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
        private void Btn_TransferStatusTransition_Click_1(object sender, EventArgs e)
        {
            if (_selectedStatusId == 0) return;

            Frm_FixedAsset_TransferStatusTransitions frm = new Frm_FixedAsset_TransferStatusTransitions
            {
                UserData = UserData,
                FromStatusId = _selectedStatusId,
                FromStatusName = Txt_StatusName.Text.Trim(),
                IsFinalStatus = CheckBox_IsFinal.Checked
            };
            frm.ShowDialog();

            // Recargar por si hubo cambios
            CargarEstados();
        }
        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            if (_selectedStatusId == 0) return;

            // Mostrar confirmación
            Lbl_Beneficiario.Text = "¿INACTIVAR ESTADO?";
            Btn_Yes.Visible = true;
            Btn_No.Visible = true;
            Btn_Inactive.Visible = false;
            Btn_Update.Visible = false;
        }

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            int resultado = Ctrl_FixedAssetTransferStatus.InactivarEstado(
                _selectedStatusId, UserData?.UserId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Estado inactivado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarEstados();
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("El estado no existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("No se puede inactivar: el estado tiene traslados asociados.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Ocurrió un error al inactivar el estado.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            Btn_Yes.Visible = false;
            Btn_No.Visible = false;
            Btn_Inactive.Visible = true;
            Btn_Update.Visible = true;
            Lbl_Beneficiario.Text = "";
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            EstadoInicial();
        }

        #endregion

        #region Transiciones

        private void Btn_TransferStatusTransition_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Validaciones

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(Txt_StatusCode.Text))
            {
                MessageBox.Show("El código del estado es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_StatusCode.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Txt_StatusName.Text))
            {
                MessageBox.Show("El nombre del estado es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_StatusName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Txt_Order.Text) || !int.TryParse(Txt_Order.Text.Trim(), out _))
            {
                MessageBox.Show("El orden debe ser un número entero válido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Order.Focus();
                return false;
            }

            return true;
        }

        #endregion
    }
}