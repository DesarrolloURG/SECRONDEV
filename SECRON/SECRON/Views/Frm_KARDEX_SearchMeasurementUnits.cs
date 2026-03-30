using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_SearchMeasurementUnits : Form
    {
        #region PropiedadesIniciales

        // Lista en memoria de unidades
        private List<Mdl_MeasurementUnits> _unidades = new List<Mdl_MeasurementUnits>();

        // Unidad seleccionada para devolver al formulario padre
        public int? SelectedUnitId { get; private set; }
        public string SelectedUnitCode { get; private set; }
        public string SelectedUnitName { get; private set; }
        public string SelectedAbbreviation { get; private set; }

        public Frm_KARDEX_SearchMeasurementUnits()
        {
            InitializeComponent();
            // Configurar Medidas Formulario
            ConfigurarTamañoFormulario();
        }

        private void Frm_KARDEX_SearchMeasurementUnits_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarComboBoxBuscarPor();
                ConfigurarPlaceHolders();
                ConfigurarTabla();
                CargarUnidades();
                CargarProximoCodigoItem();
                ConfigurarMaxLengthTextBox();
                ConfigurarComponentesDeshabilitados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar formulario: {ex.Message}",
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Configurar Medidas Formulario
        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(1000, 650);           // Tamaño fijo
            this.MinimumSize = new Size(1000, 650);    // Tamaño mínimo
            this.MaximumSize = new Size(1000, 650);    // Tamaño máximo
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // No redimensionable
            this.StartPosition = FormStartPosition.CenterParent; // Centrado en el padre
            this.MaximizeBox = false;                 // Sin botón maximizar
        }
        #endregion PropiedadesIniciales
        #region ConfigurarTextBox

        private void ConfigurarMaxLengthTextBox()
        {
            // Buscador
            Txt_ValorBuscado.MaxLength = 100;

            // Detalle del artículo
            Txt_Codigo.MaxLength = 50;
            Txt_UnitName.MaxLength = 50;
            Txt_Abbreviation.MaxLength = 10;
        }
        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_Codigo.Enabled = false;
            Txt_Selected.Enabled = false;
        }
        #endregion ConfigurarTextBox
        #region CodigoItemAutomatico
        // MÉTODO PARA CARGAR EL PRÓXIMO CÓDIGO DE ARTÍCULO
        private void CargarProximoCodigoItem()
        {
            try
            {
                string proximoCodigo = Ctrl_MeasurementUnits.ObtenerProximoCodigoUnidad();
                Txt_Codigo.Text = proximoCodigo;
                Txt_Codigo.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de artículo: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Txt_Codigo.Text = "ERROR";
            }
        }
        #endregion CodigoItemAutomatico
        #region ConfiguracionInicial

        private void ConfigurarComboBoxBuscarPor()
        {
            ComboBox_BuscarPor.Items.Clear();
            ComboBox_BuscarPor.Items.Add("TODOS");
            ComboBox_BuscarPor.Items.Add("CÓDIGO");
            ComboBox_BuscarPor.Items.Add("NOMBRE");
            ComboBox_BuscarPor.Items.Add("ABREVIATURA");

            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_BuscarPor.SelectedIndex = 0;
        }

        private void ConfigurarPlaceHolders()
        {
            ConfigurarPlaceHolderTextBox(Txt_ValorBuscado, "BUSCAR UNIDAD DE MEDIDA...");
            ConfigurarPlaceHolderTextBox(Txt_Codigo, "CÓDIGO UNIDAD DE MEDIDA *");
            ConfigurarPlaceHolderTextBox(Txt_UnitName, "NOMBRE DEL ARTÍCULO *");
            ConfigurarPlaceHolderTextBox(Txt_Abbreviation, "ABREVIATURA *");
            ConfigurarPlaceHolderTextBox(Txt_Selected, "UNIDAD DE MEDIDA SELECCIONADA");
        }

        private void ConfigurarPlaceHolderTextBox(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder && textBox.ForeColor == Color.Gray)
                {
                    textBox.Text = string.Empty;
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private void ConfigurarTabla()
        {
            Tabla.AutoGenerateColumns = true;
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToDeleteRows = false;
            Tabla.AllowUserToResizeRows = false;

            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 143, 109);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla.EnableHeadersVisualStyles = false;

            Tabla.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.Gainsboro;

            Tabla.SelectionChanged += Tabla_SelectionChanged;
            Tabla.CellBeginEdit += (s, e) => e.Cancel = true;
            Tabla.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete) e.Handled = true; };
        }

        #endregion ConfiguracionInicial
        #region CargarYRefrescarDatos

        private void CargarUnidades()
        {
            try
            {
                _unidades = Ctrl_MeasurementUnits.MostrarUnidades();
                RefrescarTabla(_unidades);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener unidades: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefrescarTabla(List<Mdl_MeasurementUnits> lista)
        {
            Tabla.DataSource = null;
            Tabla.DataSource = lista;

            if (Tabla.Columns.Count > 0)
            {
                foreach (DataGridViewColumn col in Tabla.Columns)
                {
                    col.Visible = false;
                }

                if (Tabla.Columns.Contains("UnitCode"))
                {
                    Tabla.Columns["UnitCode"].Visible = true;
                    Tabla.Columns["UnitCode"].HeaderText = "CÓDIGO";
                    Tabla.Columns["UnitCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                if (Tabla.Columns.Contains("UnitName"))
                {
                    Tabla.Columns["UnitName"].Visible = true;
                    Tabla.Columns["UnitName"].HeaderText = "NOMBRE DE LA UNIDAD";
                    Tabla.Columns["UnitName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (Tabla.Columns.Contains("Abbreviation"))
                {
                    Tabla.Columns["Abbreviation"].Visible = true;
                    Tabla.Columns["Abbreviation"].HeaderText = "ABREVIATURA";
                    Tabla.Columns["Abbreviation"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
        }

        #endregion CargarYRefrescarDatos
        #region BuscarUnidades

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                string texto = Txt_ValorBuscado.Text;

                if (string.IsNullOrWhiteSpace(texto) || texto == "BUSCAR UNIDAD DE MEDIDA...")
                {
                    RefrescarTabla(_unidades);
                    return;
                }

                texto = texto.Trim().ToUpper();
                string filtro = ComboBox_BuscarPor.SelectedItem.ToString();

                IEnumerable<Mdl_MeasurementUnits> consulta = _unidades;

                if (filtro == "CÓDIGO")
                {
                    consulta = consulta.Where(u => (u.UnitCode ?? "").ToUpper().Contains(texto));
                }
                else if (filtro == "NOMBRE")
                {
                    consulta = consulta.Where(u => (u.UnitName ?? "").ToUpper().Contains(texto));
                }
                else if (filtro == "ABREVIATURA")
                {
                    consulta = consulta.Where(u => (u.Abbreviation ?? "").ToUpper().Contains(texto));
                }
                else // TODOS
                {
                    consulta = consulta.Where(u =>
                        (u.UnitCode ?? "").ToUpper().Contains(texto) ||
                        (u.UnitName ?? "").ToUpper().Contains(texto) ||
                        (u.Abbreviation ?? "").ToUpper().Contains(texto));
                }

                var resultados = consulta.ToList();
                RefrescarTabla(resultados);

                if (resultados.Count == 0)
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

        private void Btn_ClearSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR UNIDAD DE MEDIDA...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            RefrescarTabla(_unidades);
        }

        #endregion BuscarUnidades
        #region SeleccionarUnidad

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count > 0)
            {
                var row = Tabla.SelectedRows[0];

                int unitId = Convert.ToInt32(row.Cells["UnitId"].Value);
                string unitCode = row.Cells["UnitCode"].Value?.ToString() ?? "";
                string unitName = row.Cells["UnitName"].Value?.ToString() ?? "";
                string abbreviation = row.Cells["Abbreviation"].Value?.ToString() ?? "";

                SelectedUnitId = unitId;
                SelectedUnitCode = unitCode;
                SelectedUnitName = unitName;
                SelectedAbbreviation = abbreviation;

                // Mostrar en Txt_Selected
                Txt_Selected.ForeColor = Color.Black;
                Txt_Selected.Text = $"{unitName} ({abbreviation})";

                // Cargar también al panel de detalles
                Txt_Codigo.ForeColor = Color.Black;
                Txt_UnitName.ForeColor = Color.Black;
                Txt_Abbreviation.ForeColor = Color.Black;

                Txt_Codigo.Text = unitCode;
                Txt_UnitName.Text = unitName;
                Txt_Abbreviation.Text = abbreviation;
            }
        }

        #endregion SeleccionarUnidad
        #region CRUD_Unidades

        private bool TienePlaceholder(TextBox txt, string placeholder)
        {
            return string.IsNullOrWhiteSpace(txt.Text) ||
                   txt.Text == placeholder ||
                   txt.ForeColor == Color.Gray;
        }

        private bool ValidarDetalle()
        {
            if (TienePlaceholder(Txt_Codigo, "CÓDIGO UNIDAD DE MEDIDA *"))
            {
                MessageBox.Show("El campo CÓDIGO UNIDAD DE MEDIDA es obligatorio", "VALIDACIÓN",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Codigo.Focus();
                return false;
            }

            if (TienePlaceholder(Txt_UnitName, "NOMBRE DEL ARTÍCULO *"))
            {
                MessageBox.Show("El campo NOMBRE DEL ARTÍCULO es obligatorio", "VALIDACIÓN",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_UnitName.Focus();
                return false;
            }

            if (TienePlaceholder(Txt_Abbreviation, "ABREVIATURA *"))
            {
                MessageBox.Show("El campo ABREVIATURA es obligatorio", "VALIDACIÓN",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Abbreviation.Focus();
                return false;
            }

            return true;
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarDetalle())
                    return;

                var confirm = MessageBox.Show("¿Desea registrar esta unidad de medida?",
                                              "CONFIRMAR REGISTRO",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                var nuevaUnidad = new Mdl_MeasurementUnits
                {
                    UnitCode = Ctrl_MeasurementUnits.ObtenerProximoCodigoUnidad(),
                    UnitName = Txt_UnitName.Text.Trim().ToUpper(),
                    Abbreviation = Txt_Abbreviation.Text.Trim().ToUpper(),
                    IsActive = true
                };

                int resultado = Ctrl_MeasurementUnits.RegistrarUnidad(nuevaUnidad);

                if (resultado > 0)
                {
                    MessageBox.Show("Unidad registrada correctamente", "ÉXITO",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarDetalle();
                    CargarUnidades();
                    CargarProximoCodigoItem();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar la unidad", "ERROR",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar unidad: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SelectedUnitId.HasValue || SelectedUnitId.Value <= 0)
                {
                    MessageBox.Show("Debe seleccionar una unidad de la tabla para actualizar",
                                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarDetalle())
                    return;

                var confirm = MessageBox.Show("¿Desea actualizar los datos de esta unidad?",
                                              "CONFIRMAR ACTUALIZACIÓN",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                var unidad = new Mdl_MeasurementUnits
                {
                    UnitId = SelectedUnitId.Value,
                    UnitCode = SelectedUnitCode ?? "",
                    UnitName = Txt_UnitName.Text.Trim().ToUpper(),
                    Abbreviation = Txt_Abbreviation.Text.Trim().ToUpper(),
                    IsActive = true
                };

                int resultado = Ctrl_MeasurementUnits.ActualizarUnidad(unidad);

                if (resultado > 0)
                {
                    MessageBox.Show("Unidad actualizada correctamente", "ÉXITO",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarDetalle();
                    CargarUnidades();
                    CargarProximoCodigoItem();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar la unidad", "ERROR",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar unidad: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SelectedUnitId.HasValue || SelectedUnitId.Value <= 0)
                {
                    MessageBox.Show("Debe seleccionar una unidad de la tabla para inactivar",
                                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show("¿Está seguro que desea INACTIVAR esta unidad?",
                                              "CONFIRMAR INACTIVACIÓN",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                int resultado = Ctrl_MeasurementUnits.InactivarUnidad(SelectedUnitId.Value);

                if (resultado > 0)
                {
                    MessageBox.Show("Unidad inactivada correctamente", "ÉXITO",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarDetalle();
                    CargarUnidades();
                    CargarProximoCodigoItem();
                }
                else
                {
                    MessageBox.Show("No se pudo inactivar la unidad", "ERROR",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar unidad: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarDetalle();
            CargarProximoCodigoItem();
        }

        private void LimpiarDetalle()
        {
            SelectedUnitId = null;
            SelectedUnitCode = null;
            SelectedUnitName = null;
            SelectedAbbreviation = null;

            Txt_Codigo.Text = "CÓDIGO UNIDAD DE MEDIDA *";
            Txt_UnitName.Text = "NOMBRE DEL ARTÍCULO *";
            Txt_Abbreviation.Text = "ABREVIATURA *";

            Txt_Codigo.ForeColor = Color.Gray;
            Txt_UnitName.ForeColor = Color.Gray;
            Txt_Abbreviation.ForeColor = Color.Gray;

            Txt_Selected.Text = "UNIDAD DE MEDIDA SELECCIONADA";
            Txt_Selected.ForeColor = Color.Gray;
        }

        #endregion CRUD_Unidades
        #region BotonesAceptarCancelar

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (!SelectedUnitId.HasValue || SelectedUnitId.Value <= 0)
            {
                MessageBox.Show("Debe seleccionar una unidad de medida", "VALIDACIÓN",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        #endregion BotonesAceptarCancelar
    }
}
