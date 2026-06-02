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

        private Ctrl_Security_Auth authController;
        private List<string> permisosUsuario = new List<string>();

        #endregion

        #region Constructor

        public Frm_FixedAsset_StatusMovements()
        {
            InitializeComponent();
            ConfigurarTamañoFormulario();
        }

        #endregion

        #region ComponentesDeshabilitados

        private void ConfigurarComponentesDeshabilitados()
        {
            // Componentes que nunca deben ser editables por el usuario
            Txt_Selected.Enabled = false;
        }

        #endregion

        #region TamañoFormulario

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(1000, 650);
            this.MinimumSize = new Size(1000, 650);
            this.MaximumSize = new Size(1000, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        #endregion

        #region Load

        private async void Frm_FixedAsset_StatusMovements_Load(object sender, EventArgs e)
        {
            ConfigurarComboBoxBuscarPor();
            ConfigurarTabla();
            CargarEstados();
            ConfigurarComponentesDeshabilitados();
            EstadoInicial();
            ConfigurarComboBoxes();
            ConfigurarTabIndexYFocus();

            // Permisos — se cargan una sola vez; nunca se vuelven a tocar
            authController = new Ctrl_Security_Auth();
            await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
            ConfigurarBotonesPorPermisos();
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

            Btn_Save.Visible = true;
            Btn_Update.Visible = true;
            Btn_Delete.Visible = true;
            Btn_TransferStatusTransition.Visible = true;
            Lbl_Beneficiario.Visible = true;
            Txt_Selected.Visible = true;

            // Botones dependientes de selección — siempre deshabilitados al limpiar
            Btn_Update.Enabled = false;
            Btn_Delete.Enabled = false;
            Lbl_Beneficiario.Enabled = false;

            // Btn_Save y Btn_TransferStatusTransition respetan permisos ya cargados
            Btn_Save.Enabled = TienePermiso("FA_MOVEMENTSSTATES_CREATE");
            Btn_TransferStatusTransition.Enabled = TienePermiso("FA_MOVEMENTSSTATES_UPDATE");

            Txt_StatusCode.ReadOnly = _modoEdicion;

            // Reasegurar componentes que deben permanecer deshabilitados
            ConfigurarComponentesDeshabilitados();
        }

        private void ConfigurarTabIndexYFocus()
        {
            // Sección de búsqueda
            ComboBox_BuscarPor.TabIndex = 0;
            Txt_ValorBuscado.TabIndex = 1;
            Btn_Search.TabIndex = 2;
            Btn_ClearSearch.TabIndex = 3;

            // Detalle del estado
            Txt_StatusCode.TabIndex = 4;
            Txt_StatusName.TabIndex = 5;
            Txt_Order.TabIndex = 6;
            CheckBox_IsFinal.TabIndex = 7;
            CheckBox_IsActive.TabIndex = 8;
            Txt_Description.TabIndex = 9;

            // Botones CRUD
            Btn_Save.TabIndex = 10;
            Btn_Update.TabIndex = 11;
            Btn_Clear.TabIndex = 12;

            // Botones de acción secundaria
            Btn_Delete.TabIndex = 13;
            Btn_TransferStatusTransition.TabIndex = 14;

            // Foco inicial
            ComboBox_BuscarPor.Select();
        }

        #endregion

        #region ConfigurarComboBox
        // Método para configurar ComboBoxes
        private void ConfigurarComboBoxes()
        {
            // Configurar propiedades de los ComboBox para que no permitan escritura
            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        #endregion ConfigurarComboBox

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

            Lbl_Beneficiario.Text = "";
            Lbl_Beneficiario.Enabled = true;
            Txt_Selected.Enabled = true;

            Txt_StatusCode.ReadOnly = true;
            _modoEdicion = true;

            // Enabled respetando permisos ya cargados
            Btn_Save.Enabled = TienePermiso("FA_MOVEMENTSSTATES_CREATE");
            Btn_Update.Enabled = TienePermiso("FA_MOVEMENTSSTATES_UPDATE");
            Btn_Delete.Enabled = TienePermiso("FA_MOVEMENTSSTATES_INACTIVE");
            Btn_TransferStatusTransition.Enabled = TienePermiso("FA_MOVEMENTSSTATES_UPDATE");
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
                default:
                    MessageBox.Show("Ocurrió un error al actualizar el estado.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }


        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            if (_selectedStatusId == 0) return;

            string nombreEstado = Txt_StatusName.Text.Trim();

            DialogResult confirmacion = MessageBox.Show(
                $"¿Está seguro de que desea inactivar el estado \"{nombreEstado}\"?\n\nEl estado dejará de estar disponible para nuevos traslados.",
                "Confirmar inactivación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes) return;

            int resultado = Ctrl_FixedAssetTransferStatus.InactivarEstado(_selectedStatusId, UserData?.UserId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Estado inactivado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarEstados();
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("El estado no existe o ya fue eliminado.", "Aviso",
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
        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            EstadoInicial();
        }

        #endregion

        #region Transiciones
        private void Btn_TransferStatusTransition_Click(object sender, EventArgs e)
        {
            // Abre el formulario de transiciones; si no hay estado seleccionado
            // FromStatusId llega en 0 y el formulario lo maneja en modo general
            Frm_FixedAsset_TransferStatusTransitions frm = new Frm_FixedAsset_TransferStatusTransitions
            {
                UserData = UserData,
                FromStatusId = _selectedStatusId,
                FromStatusName = _selectedStatusId > 0 ? Txt_StatusName.Text.Trim() : string.Empty,
                IsFinalStatus = _selectedStatusId > 0 && CheckBox_IsFinal.Checked
            };
            frm.ShowDialog();

            // Recargar por si hubo cambios en transiciones
            CargarEstados();
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

        #region SistemaDePermisos

        private async System.Threading.Tasks.Task CargarPermisosUsuario(int userId, int roleId)
        {
            try
            {
                permisosUsuario = await authController.ObtenerPermisosUsuarioAsync(userId, roleId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR PERMISOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TienePermiso(string permissionCode)
        {
            if (permisosUsuario == null || permisosUsuario.Count == 0)
                return false;

            return permisosUsuario.Contains(permissionCode);
        }

        private void ConfigurarBotonesPorPermisos()
        {
            // FA_020 - Registrar
            Btn_Save.Enabled = TienePermiso("FA_MOVEMENTSSTATES_CREATE");

            // FA_021 - Modificar / FA_022 - Eliminar: se habilitan al seleccionar fila
            Btn_Update.Enabled = false;
            Btn_Delete.Enabled = false;

            // FA_021 - Transiciones: siempre activo si tiene permiso
            Btn_TransferStatusTransition.Enabled = TienePermiso("FA_MOVEMENTSSTATES_UPDATE");

            // FA_024 - Buscar
            Btn_Search.Enabled = TienePermiso("FA_MOVEMENTSSTATES_READ");
            Btn_ClearSearch.Enabled = TienePermiso("FA_MOVEMENTSSTATES_READ");
        }


        #endregion

        
    }
}