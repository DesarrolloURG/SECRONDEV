using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_FixedAsset_SearchClassificationCategories : Form
    {
        #region PropiedadesIniciales
        private Frm_FixedAssetCategories _parent;
        public Mdl_Security_UserInfo UserData { get; set; }

        // Propiedades que el padre lee después del ShowDialog
        public int SelectedClassificationId { get; private set; } = 0;
        public string SelectedClassificationName { get; private set; } = "";

        // Modo selector — true cuando se abre desde Frm_FixedAssetCategories
        public bool ModoSelector { get; set; } = false;

        private int _selectedClassificationId = 0;
        private bool _selectedIsActive = true;
        private List<Mdl_FixedAssetClassificationCategory> _clasificacionesList;

        #endregion

        #region Constructor

        public Frm_FixedAsset_SearchClassificationCategories(Form parent = null)
        {
            InitializeComponent();
            _parent = parent as Frm_FixedAssetCategories;
        }

        #endregion

        #region Load

        private void Frm_FixedAsset_SearchClassificationCategories_Load(object sender, EventArgs e)
        {
            ConfigurarTamañoFormulario();
            ConfigurarComponentesDeshabilitados();
            ConfigurarCombos();
            ConfigurarTabla();
            CargarClasificaciones(isActive: true);
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

        #region ComponentesDeshabilitados

        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_Selected.Enabled = false;
        }

        #endregion

        #region Configuración

        private void ConfigurarCombos()
        {
            ComboBox_BuscarPor.Items.Clear();
            ComboBox_BuscarPor.Items.Add("POR CÓDIGO");
            ComboBox_BuscarPor.Items.Add("POR NOMBRE");
            ComboBox_BuscarPor.SelectedIndex = 0;
            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;

            ComboBox_Active.Items.Clear();
            ComboBox_Active.Items.Add("TODOS");
            ComboBox_Active.Items.Add("SOLO ACTIVOS");
            ComboBox_Active.Items.Add("SOLO INACTIVOS");
            ComboBox_Active.SelectedIndex = 1;
            ComboBox_Active.DropDownStyle = ComboBoxStyle.DropDownList;
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

            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                HeaderText = "ID",
                DataPropertyName = "ClassificationId",
                Visible = false
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCodigo",
                HeaderText = "CÓDIGO",
                DataPropertyName = "ClassificationCode",
                Width = 120
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                HeaderText = "NOMBRE",
                DataPropertyName = "ClassificationName",
                Width = 200
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDescripcion",
                HeaderText = "DESCRIPCIÓN",
                DataPropertyName = "Description",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colActivo",
                HeaderText = "ESTADO",
                DataPropertyName = "IsActiveText",
                Width = 90,
                ReadOnly = true,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            Tabla.CellFormatting -= Tabla_CellFormatting_Estado;
            Tabla.CellFormatting += Tabla_CellFormatting_Estado;

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void Tabla_CellFormatting_Estado(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (Tabla.Columns[e.ColumnIndex].Name != "colActivo") return;
            if (e.Value == null) return;

            bool activo = e.Value.ToString() == "ACTIVO";
            e.CellStyle.ForeColor = activo ? Color.FromArgb(0, 128, 0) : Color.FromArgb(200, 0, 0);
            e.CellStyle.Font = new Font(Tabla.Font, FontStyle.Bold);
        }

        private void EstadoInicial()
        {
            _selectedClassificationId = 0;
            _selectedIsActive = true;

            Txt_Codigo.Clear();
            Txt_Name.Clear();
            Txt_Selected.Clear();

            Btn_Save.Enabled = true;
            Btn_Update.Enabled = false;
            Btn_IsActive.Enabled = false;
            Btn_IsActive.Text = "ACTIVAR/INACTIVAR";
            Btn_Yes.Enabled = false;
            Btn_No.Enabled = _parent != null || ModoSelector;

            ConfigurarComponentesDeshabilitados();
        }

        #endregion

        #region CargaDeDatos

        private void CargarClasificaciones(
            string codigo = null,
            string nombre = null,
            bool? isActive = null)
        {
            try
            {
                _clasificacionesList = Ctrl_FixedAssetClassificationCategories
                    .MostrarClasificaciones(codigo, nombre, isActive);

                Tabla.DataSource = null;
                Tabla.DataSource = _clasificacionesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clasificaciones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Búsqueda

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string valor = Txt_ValorBuscado.Text.Trim().ToUpper();
            string filtro = ComboBox_BuscarPor.SelectedItem?.ToString();

            string codigo = filtro == "POR CÓDIGO" ? valor : null;
            string nombre = filtro == "POR NOMBRE" ? valor : null;

            bool? isActive = null;
            string estado = ComboBox_Active.SelectedItem?.ToString();
            if (estado == "SOLO ACTIVOS") isActive = true;
            if (estado == "SOLO INACTIVOS") isActive = false;

            CargarClasificaciones(codigo, nombre, isActive);
            EstadoInicial();
        }

        private void Btn_ClearSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Clear();
            ComboBox_BuscarPor.SelectedIndex = 0;
            ComboBox_Active.SelectedIndex = 1;
            CargarClasificaciones(isActive: true);
            EstadoInicial();
        }

        #endregion

        #region SelecciónEnTabla

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            ActualizarSeleccion(Tabla.Rows[e.RowIndex]);
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;
            ActualizarSeleccion(Tabla.SelectedRows[0]);
        }

        private void ActualizarSeleccion(DataGridViewRow row)
        {
            _selectedClassificationId = Convert.ToInt32(row.Cells["colId"].Value);
            string nombre = row.Cells["colNombre"].Value?.ToString();
            string codigo = row.Cells["colCodigo"].Value?.ToString();

            var clasificacion = _clasificacionesList?.Find(c => c.ClassificationId == _selectedClassificationId);
            _selectedIsActive = clasificacion?.IsActive ?? true;

            Txt_Codigo.Text = codigo;
            Txt_Name.Text = nombre;
            Txt_Selected.Text = nombre;

            Btn_Save.Enabled = false;
            Btn_Update.Enabled = true;
            Btn_IsActive.Enabled = true;
            Btn_IsActive.Text = _selectedIsActive ? "INACTIVAR" : "ACTIVAR";
            Btn_Yes.Enabled = _parent != null || ModoSelector;
        }

        #endregion

        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txt_Codigo.Text))
            {
                MessageBox.Show("El código es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Codigo.Focus(); return;
            }

            if (string.IsNullOrWhiteSpace(Txt_Name.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Name.Focus(); return;
            }

            var clasificacion = new Mdl_FixedAssetClassificationCategory
            {
                ClassificationCode = Txt_Codigo.Text.Trim().ToUpper(),
                ClassificationName = Txt_Name.Text.Trim().ToUpper(),
                CreatedBy = UserData?.UserId
            };

            int resultado = Ctrl_FixedAssetClassificationCategories
                .RegistrarClasificacion(clasificacion);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Clasificación registrada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarClasificaciones(isActive: true);
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("El código de clasificación ya existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Error al registrar la clasificación.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (_selectedClassificationId == 0) return;

            if (string.IsNullOrWhiteSpace(Txt_Codigo.Text))
            {
                MessageBox.Show("El código es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Codigo.Focus(); return;
            }

            if (string.IsNullOrWhiteSpace(Txt_Name.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Name.Focus(); return;
            }

            var clasificacion = new Mdl_FixedAssetClassificationCategory
            {
                ClassificationId = _selectedClassificationId,
                ClassificationCode = Txt_Codigo.Text.Trim().ToUpper(),
                ClassificationName = Txt_Name.Text.Trim().ToUpper(),
                IsActive = true,
                ModifiedBy = UserData?.UserId
            };

            int resultado = Ctrl_FixedAssetClassificationCategories
                .ActualizarClasificacion(clasificacion);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show("Clasificación actualizada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarClasificaciones(isActive: true);
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("La clasificación no existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("El código ya está en uso por otra clasificación.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("Error al actualizar la clasificación.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_IsActive_Click(object sender, EventArgs e)
        {
            if (_selectedClassificationId == 0) return;

            string nombre = Txt_Name.Text.Trim();
            bool nuevoEstado = !_selectedIsActive;
            string accion = _selectedIsActive ? "inactivar" : "activar";

            string mensaje = _selectedIsActive
                ? $"¿Está seguro de inactivar la clasificación \"{nombre}\"?\n\n" +
                  "Solo se puede inactivar si no tiene categorías activas asignadas."
                : $"¿Está seguro de activar la clasificación \"{nombre}\"?";

            DialogResult confirm = MessageBox.Show(mensaje,
                $"Confirmar {accion}ción",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            int resultado = Ctrl_FixedAssetClassificationCategories
                .CambiarEstadoClasificacion(_selectedClassificationId, nuevoEstado, UserData?.UserId);

            switch (resultado)
            {
                case 1:
                    MessageBox.Show($"Clasificación {(nuevoEstado ? "activada" : "inactivada")} correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarClasificaciones(isActive: true);
                    EstadoInicial();
                    break;
                case -1:
                    MessageBox.Show("La clasificación no existe.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show(
                        "No se puede inactivar: tiene categorías activas asignadas.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show($"Error al {accion} la clasificación.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            EstadoInicial();
        }

        #endregion

        #region ModoSelector

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (_selectedClassificationId == 0)
            {
                MessageBox.Show("Debe seleccionar una clasificación de la lista.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Escribir directamente en el padre si fue abierto desde él
            if (_parent != null)
            {
                _parent.SetClasificacion(_selectedClassificationId, Txt_Name.Text.Trim());
                this.Close();
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion
    }
}