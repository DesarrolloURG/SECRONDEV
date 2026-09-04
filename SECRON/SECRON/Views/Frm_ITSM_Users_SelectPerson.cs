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
    public partial class Frm_ITSM_Users_SelectPerson : Form
    {
        #region PropiedadesIniciales
        private Frm_ITSM_Users_Managment _frmPadre;

        // Datos de la persona seleccionada en la tabla (null si es modo texto libre)
        private string _personType;
        private int? _personId;
        private int? _personUserId; // UserId que YA tuviera esa persona vinculado (si aplica)
        private int _userIdEnEdicion; // 0 o null si es un Usuario nuevo (Btn_Save), el UserId real si es edición (Btn_Update)

        public Frm_ITSM_Users_SelectPerson(Frm_ITSM_Users_Managment frmPadre, int userIdEnEdicion = 0)
        {
            InitializeComponent();
            _frmPadre = frmPadre;
            _userIdEnEdicion = userIdEnEdicion;

            ConfigurarTamañoFormulario();
        }

        private void Frm_ITSM_Users_SelectPerson_Load(object sender, EventArgs e)
        {
            Txt_Beneficiario.MaxLength = 150;

            ConfigurarComponentesDeshabilitados();
            ConfigurarComboBox();
            ConfigurarPlaceHolder();
            ConfigurarTabla();
            ConfigurarCheckboxVincular();
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
                "DOCENTES",
                "TRABAJADORES",
                "PROVEEDORES",
                "COORDINADORES"
            });
            ComboBox_BuscarPor.SelectedIndex = 0;
            ComboBox_BuscarPor.SelectedIndexChanged += ComboBox_BuscarPor_SelectedIndexChanged;
        }
        #endregion ConfigurarComboBox

        #region EventoComboBox
        private void ComboBox_BuscarPor_SelectedIndexChanged(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "";
            LimpiarSeleccion();
            CargarDatos(ComboBox_BuscarPor.SelectedItem.ToString());
        }
        #endregion EventoComboBox

        #region ConfigurarPlaceHolder
        private void ConfigurarPlaceHolder()
        {
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_ValorBuscado.Text = "BUSCAR PERSONA...";

            Txt_ValorBuscado.GotFocus += (s, e) =>
            {
                if (Txt_ValorBuscado.Text == "BUSCAR PERSONA...")
                {
                    Txt_ValorBuscado.Text = "";
                    Txt_ValorBuscado.ForeColor = Color.Black;
                }
            };

            Txt_ValorBuscado.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                {
                    Txt_ValorBuscado.Text = "BUSCAR PERSONA...";
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

            CargarDatos(ComboBox_BuscarPor.SelectedItem.ToString());
        }
        #endregion ConfigurarTabla

        #region ConfigurarCheckboxVincular
        private void ConfigurarCheckboxVincular()
        {
            checkBox_Vincular.Checked = true; // Por defecto: se vincula a una persona real de la tabla
            checkBox_Vincular.CheckedChanged += CheckBox_Vincular_CheckedChanged;
            AplicarEstadoVinculacion();
        }

        private void CheckBox_Vincular_CheckedChanged(object sender, EventArgs e)
        {
            AplicarEstadoVinculacion();
        }

        // Marcado = selecciona persona real de la tabla (vinculada por UserId)
        // Desmarcado = solo escribe el nombre a mano (sin vínculo, persona no registrada aún)
        private void AplicarEstadoVinculacion()
        {
            bool vincular = checkBox_Vincular.Checked;

            ComboBox_BuscarPor.Enabled = vincular;
            Txt_ValorBuscado.Enabled = vincular;
            Btn_Search.Enabled = vincular;
            Btn_Clear.Enabled = vincular;
            Tabla.Enabled = vincular;

            Txt_Beneficiario.Enabled = !vincular; // Solo editable a mano si NO se vincula
            Txt_Beneficiario.Clear();
            LimpiarSeleccion();

            if (vincular)
            {
                CargarDatos(ComboBox_BuscarPor.SelectedItem.ToString());
            }
            else
            {
                Tabla.DataSource = null;
                Txt_Beneficiario.Focus();
            }
        }

        private void LimpiarSeleccion()
        {
            _personType = null;
            _personId = null;
            _personUserId = null;
        }
        #endregion ConfigurarCheckboxVincular

        #region CargarDatos
        private void CargarDatos(string tipo)
        {
            try
            {
                Tabla.DataSource = null;

                if (tipo == "DOCENTES")
                {
                    Tabla.DataSource = Ctrl_Teachers.MostrarDocentes();
                    MostrarSoloColumna("FullName", "NOMBRE DEL DOCENTE");
                }
                else if (tipo == "TRABAJADORES")
                {
                    Tabla.DataSource = Ctrl_Employees.MostrarEmpleados();
                    MostrarSoloColumna("FullName", "NOMBRE DEL TRABAJADOR");
                }
                else if (tipo == "PROVEEDORES")
                {
                    Tabla.DataSource = Ctrl_Suppliers.MostrarProveedores();
                    MostrarSoloColumna("SupplierName", "NOMBRE DEL PROVEEDOR");
                }
                else if (tipo == "COORDINADORES")
                {
                    Tabla.DataSource = Ctrl_Coordinators.MostrarCoordinadores();
                    MostrarSoloColumna("FullName", "NOMBRE DEL COORDINADOR");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR DATOS: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarSoloColumna(string columnName, string headerText)
        {
            if (Tabla.Columns.Count == 0) return;

            foreach (DataGridViewColumn col in Tabla.Columns)
                col.Visible = false;

            if (Tabla.Columns.Contains(columnName))
            {
                Tabla.Columns[columnName].Visible = true;
                Tabla.Columns[columnName].HeaderText = headerText;
                Tabla.Columns[columnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
        #endregion CargarDatos

        #region BuscarPersona
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text) ||
                    Txt_ValorBuscado.Text == "BUSCAR PERSONA...")
                {
                    CargarDatos(ComboBox_BuscarPor.SelectedItem.ToString());
                    return;
                }

                Tabla.DataSource = null;
                string tipo = ComboBox_BuscarPor.SelectedItem.ToString();

                if (tipo == "DOCENTES")
                {
                    Tabla.DataSource = Ctrl_Teachers.BuscarPorNombre(Txt_ValorBuscado.Text);
                    MostrarSoloColumna("FullName", "NOMBRE DEL DOCENTE");
                }
                else if (tipo == "TRABAJADORES")
                {
                    Tabla.DataSource = Ctrl_Employees.BuscarEmpleados(Txt_ValorBuscado.Text);
                    MostrarSoloColumna("FullName", "NOMBRE DEL TRABAJADOR");
                }
                else if (tipo == "PROVEEDORES")
                {
                    Tabla.DataSource = Ctrl_Suppliers.BuscarProveedores(Txt_ValorBuscado.Text, "POR NOMBRE");
                    MostrarSoloColumna("SupplierName", "NOMBRE DEL PROVEEDOR");
                }
                else if (tipo == "COORDINADORES")
                {
                    Tabla.DataSource = Ctrl_Coordinators.BuscarPorNombre(Txt_ValorBuscado.Text);
                    MostrarSoloColumna("FullName", "NOMBRE DEL COORDINADOR");
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

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }
        #endregion BuscarPersona

        #region SeleccionarPersona
        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count > 0)
            {
                var row = Tabla.SelectedRows[0];
                string tipo = ComboBox_BuscarPor.SelectedItem.ToString();

                if (tipo == "PROVEEDORES")
                {
                    Txt_Beneficiario.Text = row.Cells["SupplierName"].Value?.ToString() ?? "";
                    _personType = "PROVEEDOR";
                    _personId = Convert.ToInt32(row.Cells["SupplierId"].Value);
                }
                else if (tipo == "DOCENTES")
                {
                    Txt_Beneficiario.Text = row.Cells["FullName"].Value?.ToString() ?? "";
                    _personType = "DOCENTE";
                    _personId = Convert.ToInt32(row.Cells["TeacherId"].Value);
                }
                else if (tipo == "TRABAJADORES")
                {
                    Txt_Beneficiario.Text = row.Cells["FullName"].Value?.ToString() ?? "";
                    _personType = "TRABAJADOR";
                    _personId = Convert.ToInt32(row.Cells["EmployeeId"].Value);
                }
                else if (tipo == "COORDINADORES")
                {
                    Txt_Beneficiario.Text = row.Cells["FullName"].Value?.ToString() ?? "";
                    _personType = "COORDINADOR";
                    _personId = Convert.ToInt32(row.Cells["CoordinatorId"].Value);
                }

                _personUserId = row.Cells["UserId"].Value == DBNull.Value || row.Cells["UserId"].Value == null
                    ? (int?)null
                    : Convert.ToInt32(row.Cells["UserId"].Value);
            }
        }
        #endregion SeleccionarPersona

        #region BotonesAccion
        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txt_Beneficiario.Text))
            {
                MessageBox.Show("Debe seleccionar o escribir una persona", "VALIDACIÓN",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkBox_Vincular.Checked && _personId == null)
            {
                MessageBox.Show("Debe seleccionar una fila de la tabla para vincular", "VALIDACIÓN",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkBox_Vincular.Checked && _personUserId.HasValue && _personUserId != _userIdEnEdicion)
            {
                var confirmar = MessageBox.Show(
                    "ESTA PERSONA YA ESTÁ VINCULADA A OTRO USUARIO DEL SISTEMA.\n" +
                    "SI CONTINÚA, SE DESVINCULARÁ AL USUARIO ANTERIOR.\n\n¿DESEA CONTINUAR?",
                    "ADVERTENCIA DE REASIGNACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmar == DialogResult.No) return;
            }

            if (_frmPadre != null && !_frmPadre.IsDisposed)
            {
                if (checkBox_Vincular.Checked)
                    _frmPadre.ActualizarPersonaSeleccionada(Txt_Beneficiario.Text, _personType, _personId, _personUserId);
                else
                    _frmPadre.ActualizarPersonaSeleccionada(Txt_Beneficiario.Text, null, null, null);
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
            Txt_ValorBuscado.Text = "BUSCAR PERSONA...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_Beneficiario.Clear();
            LimpiarSeleccion();
            if (checkBox_Vincular.Checked)
                CargarDatos(ComboBox_BuscarPor.SelectedItem.ToString());
        }
        #endregion BotonesAccion
    }
}