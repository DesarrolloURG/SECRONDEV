using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_SearchSubCategory : Form
    {
        #region PropiedadesIniciales

        private readonly int _categoryId;
        private readonly string _categoryName;

        private List<Mdl_ItemSubCategories> _subCategorias = new List<Mdl_ItemSubCategories>();

        public int? SelectedSubCategoryId { get; private set; }
        public string SelectedSubCategoryCode { get; private set; }
        public string SelectedSubCategoryName { get; private set; }

        public int? UserId { get; set; }

        public Frm_KARDEX_SearchSubCategory(int categoryId, string categoryName)
        {
            InitializeComponent();
            _categoryId = categoryId;
            _categoryName = categoryName;
            ConfigurarTamañoFormulario();
        }

        private void Frm_KARDEX_SearchSubCategory_Load(object sender, EventArgs e)
        {
            try
            {
                Lbl_Formulario.Text = $"CATÁLOGO DE SUBCATEGORÍAS PARA: {_categoryName?.ToUpper()}";
                ConfigurarComboBoxBuscarPor();
                ConfigurarPlaceHolders();
                ConfigurarTabla();
                ConfigurarMaxLengthTextBox();
                ConfigurarComponentesDeshabilitados();
                CargarSubCategorias();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar formulario: {ex.Message}",
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(1000, 650);
            this.MinimumSize = new Size(1000, 650);
            this.MaximumSize = new Size(1000, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        #endregion PropiedadesIniciales

        #region ConfigurarTextBox

        private void ConfigurarMaxLengthTextBox()
        {
            Txt_ValorBuscado.MaxLength = 100;
            Txt_Codigo.MaxLength = 10;
            Txt_UnitName.MaxLength = 200;
        }

        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_Selected.Enabled = false;
            Txt_Description.Visible = false;
            Lbl_Description.Visible = false;
        }

        #endregion ConfigurarTextBox

        #region ConfiguracionInicial

        private void ConfigurarComboBoxBuscarPor()
        {
            ComboBox_BuscarPor.Items.Clear();
            ComboBox_BuscarPor.Items.Add("TODOS");
            ComboBox_BuscarPor.Items.Add("CÓDIGO");
            ComboBox_BuscarPor.Items.Add("NOMBRE");
            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_BuscarPor.SelectedIndex = 0;
        }

        private void ConfigurarPlaceHolders()
        {
            ConfigurarPlaceHolderTextBox(Txt_ValorBuscado, "BUSCAR SUBCATEGORÍA...");
            ConfigurarPlaceHolderTextBox(Txt_Codigo, "CÓDIGO DE SUBCATEGORÍA *");
            ConfigurarPlaceHolderTextBox(Txt_UnitName, "NOMBRE DE LA SUBCATEGORÍA *");
            ConfigurarPlaceHolderTextBox(Txt_Selected, "SUBCATEGORÍA SELECCIONADA");
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

        private void CargarSubCategorias()
        {
            try
            {
                _subCategorias = Ctrl_ItemSubCategories.MostrarSubCategorias(_categoryId);
                RefrescarTabla(_subCategorias);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener subcategorías: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefrescarTabla(List<Mdl_ItemSubCategories> lista)
        {
            Tabla.DataSource = null;
            Tabla.DataSource = lista;

            if (Tabla.Columns.Count > 0)
            {
                foreach (DataGridViewColumn col in Tabla.Columns)
                    col.Visible = false;

                if (Tabla.Columns.Contains("SubCategoryCode"))
                {
                    Tabla.Columns["SubCategoryCode"].Visible = true;
                    Tabla.Columns["SubCategoryCode"].HeaderText = "CÓDIGO";
                    Tabla.Columns["SubCategoryCode"].Width = 150;
                }
                if (Tabla.Columns.Contains("SubCategoryName"))
                {
                    Tabla.Columns["SubCategoryName"].Visible = true;
                    Tabla.Columns["SubCategoryName"].HeaderText = "NOMBRE";
                    Tabla.Columns["SubCategoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;

            var sub = Tabla.SelectedRows[0].DataBoundItem as Mdl_ItemSubCategories;
            if (sub == null) return;

            SelectedSubCategoryId = sub.SubCategoryId;
            SelectedSubCategoryCode = sub.SubCategoryCode;
            SelectedSubCategoryName = sub.SubCategoryName;

            Txt_Codigo.Text = sub.SubCategoryCode;
            Txt_Codigo.ForeColor = Color.Black;
            Txt_UnitName.Text = sub.SubCategoryName;
            Txt_UnitName.ForeColor = Color.Black;
            Txt_Selected.Text = $"{sub.SubCategoryCode} - {sub.SubCategoryName}";
            Txt_Selected.ForeColor = Color.Black;
        }

        #endregion CargarYRefrescarDatos

        #region Busqueda

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string texto = Txt_ValorBuscado.ForeColor == Color.Gray ? "" : Txt_ValorBuscado.Text.Trim();
            string buscarPor = ComboBox_BuscarPor.SelectedItem?.ToString() ?? "TODOS";
            var resultado = Ctrl_ItemSubCategories.MostrarSubCategorias(_categoryId, texto, buscarPor);
            RefrescarTabla(resultado);
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_Search_Click(sender, e); }
        }

        private void Btn_ClearSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR SUBCATEGORÍA...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            ComboBox_BuscarPor.SelectedIndex = 0;
            CargarSubCategorias();
        }

        #endregion Busqueda

        #region CRUD_SubCategorias

        private bool ValidarDetalle()
        {
            if (Txt_Codigo.ForeColor == Color.Gray || string.IsNullOrWhiteSpace(Txt_Codigo.Text))
            {
                MessageBox.Show("El CÓDIGO es obligatorio.", "VALIDACIÓN",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Codigo.Focus();
                return false;
            }
            if (Txt_UnitName.ForeColor == Color.Gray || string.IsNullOrWhiteSpace(Txt_UnitName.Text))
            {
                MessageBox.Show("El NOMBRE es obligatorio.", "VALIDACIÓN",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_UnitName.Focus();
                return false;
            }
            return true;
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarDetalle()) return;

                if (MessageBox.Show("¿Desea registrar esta subcategoría?", "CONFIRMAR REGISTRO",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                var nueva = new Mdl_ItemSubCategories
                {
                    CategoryId = _categoryId,
                    SubCategoryCode = Txt_Codigo.Text.Trim().ToUpper(),
                    SubCategoryName = Txt_UnitName.Text.Trim().ToUpper(),
                    IsActive = true,
                    CreatedBy = UserId
                };

                int resultado = Ctrl_ItemSubCategories.RegistrarSubCategoria(nueva);

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show("Subcategoría registrada correctamente.", "ÉXITO",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarDetalle();
                        CargarSubCategorias();
                        break;
                    case -1:
                        MessageBox.Show("Ya existe una subcategoría con ese código en esta categoría.", "DUPLICADO",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Txt_Codigo.Focus();
                        break;
                    default:
                        MessageBox.Show("No se pudo registrar la subcategoría.", "ERROR",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar subcategoría: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SelectedSubCategoryId.HasValue || SelectedSubCategoryId.Value <= 0)
                {
                    MessageBox.Show("Debe seleccionar una subcategoría de la tabla para actualizar.",
                                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarDetalle()) return;

                if (SelectedSubCategoryCode != Txt_Codigo.Text.Trim().ToUpper())
                {
                    MessageBox.Show("No se puede modificar el código de la subcategoría.", "VALIDACIÓN",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_Codigo.Focus();
                    return;
                }

                if (MessageBox.Show("¿Desea actualizar los datos de esta subcategoría?", "CONFIRMAR ACTUALIZACIÓN",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                var sub = new Mdl_ItemSubCategories
                {
                    SubCategoryId = SelectedSubCategoryId.Value,
                    CategoryId = _categoryId,
                    SubCategoryCode = SelectedSubCategoryCode,
                    SubCategoryName = Txt_UnitName.Text.Trim().ToUpper(),
                    IsActive = true,
                    ModifiedBy = UserId
                };

                int resultado = Ctrl_ItemSubCategories.ActualizarSubCategoria(sub);

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show("Subcategoría actualizada correctamente.", "ÉXITO",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarDetalle();
                        CargarSubCategorias();
                        break;
                    case -2:
                        MessageBox.Show("La subcategoría no fue encontrada.", "ERROR",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("No se pudo actualizar la subcategoría.", "ERROR",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar subcategoría: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SelectedSubCategoryId.HasValue || SelectedSubCategoryId.Value <= 0)
                {
                    MessageBox.Show("Debe seleccionar una subcategoría de la tabla para inactivar.",
                                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("¿Está seguro que desea INACTIVAR esta subcategoría?",
                    "CONFIRMAR INACTIVACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

                int resultado = Ctrl_ItemSubCategories.InactivarSubCategoria(SelectedSubCategoryId.Value, UserId);

                if (resultado > 0)
                {
                    MessageBox.Show("Subcategoría inactivada correctamente.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarDetalle();
                    CargarSubCategorias();
                }
                else
                {
                    MessageBox.Show("No se pudo inactivar la subcategoría.", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar subcategoría: " + ex.Message,
                                "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarDetalle();
        }

        private void LimpiarDetalle()
        {
            SelectedSubCategoryId = null;
            SelectedSubCategoryCode = null;
            SelectedSubCategoryName = null;

            Txt_Codigo.Text = "CÓDIGO DE SUBCATEGORÍA *";
            Txt_UnitName.Text = "NOMBRE DE LA SUBCATEGORÍA *";
            Txt_Selected.Text = "SUBCATEGORÍA SELECCIONADA";

            Txt_Codigo.ForeColor = Color.Gray;
            Txt_UnitName.ForeColor = Color.Gray;
            Txt_Selected.ForeColor = Color.Gray;
        }

        #endregion CRUD_SubCategorias

        #region BotonesAceptarCancelar

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (!SelectedSubCategoryId.HasValue || SelectedSubCategoryId.Value <= 0)
            {
                MessageBox.Show("Debe seleccionar una subcategoría.", "VALIDACIÓN",
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