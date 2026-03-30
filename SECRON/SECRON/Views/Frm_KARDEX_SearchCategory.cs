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
    public partial class Frm_KARDEX_SearchCategory : Form
    {
        #region PropiedadesIniciales

        // Lista en memoria de categorías
        private List<Mdl_ItemCategories> _categorias = new List<Mdl_ItemCategories>();

        // Categoría seleccionada para devolver al formulario padre
        public int? SelectedCategoryId { get; private set; }
        public string SelectedCategoryCode { get; private set; }
        public string SelectedCategoryName { get; private set; }
        public string SelectedDescription { get; private set; }

        public Frm_KARDEX_SearchCategory()
        {
            InitializeComponent();
            // Configurar tamaño de formulario
            ConfigurarTamañoFormulario();
        }

        private void Frm_KARDEX_SearchCategory_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarComboBoxBuscarPor();
                ConfigurarPlaceHolders();
                ConfigurarTabla();
                ConfigurarMaxLengthTextBox();
                ConfigurarComponentesDeshabilitados();

                CargarCategorias();
                CargarProximoCodigoCategoria();
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
            Txt_ValorBuscado.MaxLength = 100;
            Txt_Codigo.MaxLength = 10;
            Txt_UnitName.MaxLength = 100;
            Txt_Description.MaxLength = 500;
        }

        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_Codigo.Enabled = false;    // Código automático
            Txt_Selected.Enabled = false;  // Solo muestra la selección
        }

        #endregion ConfigurarTextBox
        #region CodigoCategoriaAutomatico

        // MÉTODO PARA CARGAR EL PRÓXIMO CÓDIGO DE CATEGORÍA
        private void CargarProximoCodigoCategoria()
        {
            try
            {
                string proximoCodigo = Ctrl_ItemCategories.ObtenerProximoCodigoCategoria();
                Txt_Codigo.Text = proximoCodigo;
                Txt_Codigo.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de categoría: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Txt_Codigo.Text = "ERROR";
            }
        }

        #endregion CodigoCategoriaAutomatico
        #region ConfiguracionInicial

        private void ConfigurarComboBoxBuscarPor()
        {
            ComboBox_BuscarPor.Items.Clear();
            ComboBox_BuscarPor.Items.Add("TODOS");
            ComboBox_BuscarPor.Items.Add("CÓDIGO");
            ComboBox_BuscarPor.Items.Add("NOMBRE");
            ComboBox_BuscarPor.Items.Add("DESCRIPCIÓN");

            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_BuscarPor.SelectedIndex = 0;
        }

        private void ConfigurarPlaceHolders()
        {
            ConfigurarPlaceHolderTextBox(Txt_ValorBuscado, "BUSCAR CATEGORÍA...");
            ConfigurarPlaceHolderTextBox(Txt_Codigo, "CÓDIGO DE CATEGORÍA *");
            ConfigurarPlaceHolderTextBox(Txt_UnitName, "NOMBRE DE LA CATEGORÍA *");
            ConfigurarPlaceHolderTextBox(Txt_Description, "DESCRIPCIÓN *");
            ConfigurarPlaceHolderTextBox(Txt_Selected, "CATEGORÍA SELECCIONADA");
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

        private void CargarCategorias()
        {
            try
            {
                _categorias = Ctrl_ItemCategories.MostrarCategorias();
                RefrescarTabla(_categorias);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener categorías: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefrescarTabla(List<Mdl_ItemCategories> lista)
        {
            Tabla.DataSource = null;
            Tabla.DataSource = lista;

            if (Tabla.Columns.Count > 0)
            {
                foreach (DataGridViewColumn col in Tabla.Columns)
                {
                    col.Visible = false;
                }

                if (Tabla.Columns.Contains("CategoryCode"))
                {
                    Tabla.Columns["CategoryCode"].Visible = true;
                    Tabla.Columns["CategoryCode"].HeaderText = "CÓDIGO";
                    Tabla.Columns["CategoryCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                if (Tabla.Columns.Contains("CategoryName"))
                {
                    Tabla.Columns["CategoryName"].Visible = true;
                    Tabla.Columns["CategoryName"].HeaderText = "NOMBRE DE LA CATEGORÍA";
                    Tabla.Columns["CategoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (Tabla.Columns.Contains("Description"))
                {
                    Tabla.Columns["Description"].Visible = true;
                    Tabla.Columns["Description"].HeaderText = "DESCRIPCIÓN";
                    Tabla.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        #endregion CargarYRefrescarDatos
        #region BuscarCategorias

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                string texto = Txt_ValorBuscado.Text;

                if (string.IsNullOrWhiteSpace(texto) || texto == "BUSCAR CATEGORÍA...")
                {
                    RefrescarTabla(_categorias);
                    return;
                }

                texto = texto.Trim().ToUpper();
                string filtro = ComboBox_BuscarPor.SelectedItem.ToString();

                IEnumerable<Mdl_ItemCategories> consulta = _categorias;

                if (filtro == "CÓDIGO")
                {
                    consulta = consulta.Where(c => (c.CategoryCode ?? "").ToUpper().Contains(texto));
                }
                else if (filtro == "NOMBRE")
                {
                    consulta = consulta.Where(c => (c.CategoryName ?? "").ToUpper().Contains(texto));
                }
                else if (filtro == "DESCRIPCIÓN")
                {
                    consulta = consulta.Where(c => (c.Description ?? "").ToUpper().Contains(texto));
                }
                else // TODOS
                {
                    consulta = consulta.Where(c =>
                        (c.CategoryCode ?? "").ToUpper().Contains(texto) ||
                        (c.CategoryName ?? "").ToUpper().Contains(texto) ||
                        (c.Description ?? "").ToUpper().Contains(texto));
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
            Txt_ValorBuscado.Text = "BUSCAR CATEGORÍA...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            RefrescarTabla(_categorias);
        }

        #endregion BuscarCategorias
        #region SeleccionarCategoria

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count > 0)
            {
                var row = Tabla.SelectedRows[0];

                int categoryId = Convert.ToInt32(row.Cells["CategoryId"].Value);
                string categoryCode = row.Cells["CategoryCode"].Value?.ToString() ?? "";
                string categoryName = row.Cells["CategoryName"].Value?.ToString() ?? "";
                string description = row.Cells["Description"].Value?.ToString() ?? "";

                SelectedCategoryId = categoryId;
                SelectedCategoryCode = categoryCode;
                SelectedCategoryName = categoryName;
                SelectedDescription = description;

                // Mostrar en Txt_Selected
                Txt_Selected.ForeColor = Color.Black;
                Txt_Selected.Text = $"{categoryName}";

                // Cargar también al panel de detalles
                Txt_Codigo.ForeColor = Color.Black;
                Txt_UnitName.ForeColor = Color.Black;
                Txt_Description.ForeColor = Color.Black;

                Txt_Codigo.Text = categoryCode;
                Txt_UnitName.Text = categoryName;
                Txt_Description.Text = description;
            }
        }

        #endregion SeleccionarCategoria
        #region CRUD_Categorias

        private bool TienePlaceholder(TextBox txt, string placeholder)
        {
            return string.IsNullOrWhiteSpace(txt.Text) ||
                   txt.Text == placeholder ||
                   txt.ForeColor == Color.Gray;
        }

        private bool ValidarDetalle()
        {
            if (TienePlaceholder(Txt_Codigo, "CÓDIGO DE CATEGORÍA *"))
            {
                MessageBox.Show("El campo CÓDIGO DE CATEGORÍA es obligatorio", "VALIDACIÓN",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Codigo.Focus();
                return false;
            }

            if (TienePlaceholder(Txt_UnitName, "NOMBRE DE LA CATEGORÍA *"))
            {
                MessageBox.Show("El campo NOMBRE DE LA CATEGORÍA es obligatorio", "VALIDACIÓN",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_UnitName.Focus();
                return false;
            }

            if (TienePlaceholder(Txt_Description, "DESCRIPCIÓN *"))
            {
                MessageBox.Show("El campo DESCRIPCIÓN es obligatorio", "VALIDACIÓN",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Description.Focus();
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

                var confirm = MessageBox.Show("¿Desea registrar esta categoría?",
                                              "CONFIRMAR REGISTRO",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                var nuevaCategoria = new Mdl_ItemCategories
                {
                    CategoryCode = Ctrl_ItemCategories.ObtenerProximoCodigoCategoria(),
                    CategoryName = Txt_UnitName.Text.Trim().ToUpper(),
                    Description = Txt_Description.Text.Trim(),
                    IsActive = true,
                    CreatedBy = null   // Si luego expones UserData, aquí lo asignas
                };

                int resultado = Ctrl_ItemCategories.RegistrarCategoria(nuevaCategoria);

                if (resultado > 0)
                {
                    MessageBox.Show("Categoría registrada correctamente", "ÉXITO",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarDetalle();
                    CargarCategorias();
                    CargarProximoCodigoCategoria();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar la categoría", "ERROR",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar categoría: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SelectedCategoryId.HasValue || SelectedCategoryId.Value <= 0)
                {
                    MessageBox.Show("Debe seleccionar una categoría de la tabla para actualizar",
                                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarDetalle())
                    return;

                var confirm = MessageBox.Show("¿Desea actualizar los datos de esta categoría?",
                                              "CONFIRMAR ACTUALIZACIÓN",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                var categoria = new Mdl_ItemCategories
                {
                    CategoryId = SelectedCategoryId.Value,
                    CategoryCode = SelectedCategoryCode ?? "",
                    CategoryName = Txt_UnitName.Text.Trim().ToUpper(),
                    Description = Txt_Description.Text.Trim(),
                    IsActive = true
                };

                int resultado = Ctrl_ItemCategories.ActualizarCategoria(categoria);

                if (resultado > 0)
                {
                    MessageBox.Show("Categoría actualizada correctamente", "ÉXITO",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarDetalle();
                    CargarCategorias();
                    CargarProximoCodigoCategoria();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar la categoría", "ERROR",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar categoría: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SelectedCategoryId.HasValue || SelectedCategoryId.Value <= 0)
                {
                    MessageBox.Show("Debe seleccionar una categoría de la tabla para inactivar",
                                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show("¿Está seguro que desea INACTIVAR esta categoría?",
                                              "CONFIRMAR INACTIVACIÓN",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                int resultado = Ctrl_ItemCategories.InactivarCategoria(SelectedCategoryId.Value);

                if (resultado > 0)
                {
                    MessageBox.Show("Categoría inactivada correctamente", "ÉXITO",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarDetalle();
                    CargarCategorias();
                    CargarProximoCodigoCategoria();
                }
                else
                {
                    MessageBox.Show("No se pudo inactivar la categoría", "ERROR",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar categoría: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarDetalle();
            CargarProximoCodigoCategoria();
        }

        private void LimpiarDetalle()
        {
            SelectedCategoryId = null;
            SelectedCategoryCode = null;
            SelectedCategoryName = null;
            SelectedDescription = null;

            Txt_Codigo.Text = "CÓDIGO DE CATEGORÍA *";
            Txt_UnitName.Text = "NOMBRE DE LA CATEGORÍA *";
            Txt_Description.Text = "DESCRIPCIÓN *";

            Txt_Codigo.ForeColor = Color.Gray;
            Txt_UnitName.ForeColor = Color.Gray;
            Txt_Description.ForeColor = Color.Gray;

            Txt_Selected.Text = "CATEGORÍA SELECCIONADA";
            Txt_Selected.ForeColor = Color.Gray;
        }

        #endregion CRUD_Categorias
        #region BotonesAceptarCancelar

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (!SelectedCategoryId.HasValue || SelectedCategoryId.Value <= 0)
            {
                MessageBox.Show("Debe seleccionar una categoría", "VALIDACIÓN",
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
