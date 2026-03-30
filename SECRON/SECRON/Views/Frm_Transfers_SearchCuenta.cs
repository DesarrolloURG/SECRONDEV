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
    public partial class Frm_Transfers_SearchCuenta : Form
    {
        #region PropiedadesIniciales
        // Referencia al formulario padre
        private Frm_Transfers_Managment _frmPadre;
        // Constructor con referencia al formulario padre
        public Frm_Transfers_SearchCuenta(Frm_Transfers_Managment frmPadre)
        {
            InitializeComponent();
            _frmPadre = frmPadre;
            // Configuraciones iniciales
            ConfigurarTamañoFormulario();
        }

        private void Frm_Transfers_SearchCuenta_Load(object sender, EventArgs e)
        {
            ConfigurarComponentesDeshabilitados();
            ConfigurarPlaceHolder();
            ConfigurarTabla();
            CargarTodasLasCuentas();
        }
        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(700, 650);           // Tamaño fijo
            this.MinimumSize = new Size(700, 650);    // Tamaño mínimo
            this.MaximumSize = new Size(700, 650);    // Tamaño máximo
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // No redimensionable
            this.StartPosition = FormStartPosition.CenterParent; // Centrado en el padre
            this.MaximizeBox = false;                 // Sin botón maximizar
        }
        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_Cuenta.Enabled = false;
            Txt_Codigo.Enabled = false;
        }
        #endregion PropiedadesIniciales
        #region ConfigurarPlaceHolder
        private void ConfigurarPlaceHolder()
        {
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_ValorBuscado.Text = "INGRESE CÓDIGO O NOMBRE DE CUENTA";

            Txt_ValorBuscado.GotFocus += (s, e) =>
            {
                if (Txt_ValorBuscado.Text == "INGRESE CÓDIGO O NOMBRE DE CUENTA")
                {
                    Txt_ValorBuscado.Text = "";
                    Txt_ValorBuscado.ForeColor = Color.Black;
                }
            };

            Txt_ValorBuscado.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                {
                    Txt_ValorBuscado.Text = "INGRESE CÓDIGO O NOMBRE DE CUENTA";
                    Txt_ValorBuscado.ForeColor = Color.Gray;
                }
            };
        }
        #endregion ConfigurarPlaceHolder
        #region ConfigurarTabla
        private void ConfigurarTabla()
        {
            // Configuración visual y de comportamiento
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.RowHeadersVisible = false;

            // Estilos
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            Tabla.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // Eventos
            Tabla.SelectionChanged += Tabla_SelectionChanged;
            Tabla.CellBeginEdit += (s, e) => e.Cancel = true;
            Tabla.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete) e.Handled = true; };
        }
        #endregion ConfigurarTabla
        #region CargarCuentas
        // Cargar todas las cuentas al inicio
        private void CargarTodasLasCuentas()
        {
            try
            {
                // NOTA: Reemplaza con tu método real
                var cuentas = Ctrl_Accounts.ObtenerTodasLasCuentas();
                var resultados = Ctrl_Accounts.BuscarCuentas(Txt_ValorBuscado.Text);

                Tabla.DataSource = null;
                Tabla.DataSource = cuentas;

                if (Tabla.Columns.Count > 0)
                {
                    // Ocultar todas las columnas
                    foreach (DataGridViewColumn col in Tabla.Columns)
                    {
                        col.Visible = false;
                    }

                    // Mostrar solo Código y Cuenta
                    if (Tabla.Columns.Contains("Code"))
                    {
                        Tabla.Columns["Code"].Visible = true;
                        Tabla.Columns["Code"].HeaderText = "CÓDIGO";
                        Tabla.Columns["Code"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        Tabla.Columns["Code"].FillWeight = 20;
                    }
                    // Mostrar solo Código y Cuenta
                    if (Tabla.Columns.Contains("Name"))
                    {
                        Tabla.Columns["Name"].Visible = true;
                        Tabla.Columns["Name"].HeaderText = "CUENTA";
                        Tabla.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        Tabla.Columns["Name"].FillWeight = 80;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR CUENTAS: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion CargarCuentas
        #region BuscarCuentas
        // Evento de buscar cuenta
        private void Btn_SearchCuenta_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text) ||
                    Txt_ValorBuscado.Text == "INGRESE CÓDIGO O NOMBRE DE CUENTA")
                {
                    CargarTodasLasCuentas();
                    return;
                }

                // Buscar cuentas
                var cuentas = Ctrl_Accounts.ObtenerTodasLasCuentas();
                var resultados = Ctrl_Accounts.BuscarCuentas(Txt_ValorBuscado.Text);

                Tabla.DataSource = null;
                Tabla.DataSource = resultados;

                if (Tabla.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn col in Tabla.Columns)
                    {
                        col.Visible = false;
                    }
                    // Mostrar solo Código y Cuenta
                    if (Tabla.Columns.Contains("Code"))
                    {
                        Tabla.Columns["Code"].Visible = true;
                        Tabla.Columns["Code"].HeaderText = "CÓDIGO";
                        Tabla.Columns["Code"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        Tabla.Columns["Code"].FillWeight = 20;
                    }

                    if (Tabla.Columns.Contains("Name"))
                    {
                        Tabla.Columns["Name"].Visible = true;
                        Tabla.Columns["Name"].HeaderText = "CUENTA";
                        Tabla.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        Tabla.Columns["Name"].FillWeight = 80;
                    }
                }

                if (resultados.Count == 0)
                {
                    MessageBox.Show("No se encontraron cuentas con ese criterio",
                                   "BÚSQUEDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR EN BÚSQUEDA: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Evento para dar enter y buscar
        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_SearchCuenta_Click(sender, e);
            }
        }
        #endregion BuscarCuentas
        #region SeleccionarCuenta
        // Evento de seleccion en la tabla
        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count > 0)
            {
                var row = Tabla.SelectedRows[0];

                Txt_Codigo.Text = row.Cells["Code"].Value?.ToString() ?? "";
                Txt_Cuenta.Text = row.Cells["Name"].Value?.ToString() ?? "";
            }
        }
        #endregion SeleccionCuenta
        #region BotonesAccion
        // Confirmar selección
        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txt_Codigo.Text))
            {
                MessageBox.Show("Debe seleccionar una cuenta", "VALIDACIÓN",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_frmPadre != null && !_frmPadre.IsDisposed)
            {
                _frmPadre.ActualizarCuentaContable(Txt_Codigo.Text, Txt_Cuenta.Text);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        // Cerrar sin seleccionar
        private void Btn_No_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        // Limpiar búsqueda y resultados
        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "INGRESE CÓDIGO O NOMBRE DE CUENTA";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_Codigo.Clear();
            Txt_Cuenta.Clear();
            CargarTodasLasCuentas();
        }
        #endregion BotonesAccion
    }
}
