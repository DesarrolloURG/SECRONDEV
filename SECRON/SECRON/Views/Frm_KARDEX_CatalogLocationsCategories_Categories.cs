using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_CatalogLocationsCategories_Categories : Form
    {
        #region PropiedadesIniciales

        // Datos del usuario autenticado
        public Mdl_Security_UserInfo UserData { get; set; }

        // Filtros de búsqueda
        private string _ultimoTextoBusqueda = "";
        private string _ultimoFiltro1 = "TODOS";

        // Selección actual
        private List<Mdl_LocationCategories> _categoriasList;
        private Mdl_LocationCategories _categoriaSeleccionada = null;

        // Propiedad para devolver la categoría elegida al formulario padre
        public Mdl_LocationCategories CategoriaElegida { get; private set; } = null;

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;

        public Frm_KARDEX_CatalogLocationsCategories_Categories()
        {
            InitializeComponent();
        }

        private void Frm_KARDEX_CatalogLocationsCategories_Categories_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarTabIndexYFocus();
                ConfigurarMaxLengthTextBox();
                ConfigurarComponentesDeshabilitados();
                ConfigurarPlaceHoldersTextbox();
                ConfigurarFiltros();
                ConfigurarPanelSeleccion();

                CargarCategorias();
                CargarProximoCodigoCategoria();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion PropiedadesIniciales
        #region ConfigurarTextBox

        private void ConfigurarMaxLengthTextBox()
        {
            Txt_ValorBuscado.MaxLength = 100;
            Txt_Codigo.MaxLength = 20;
            Txt_UnitName.MaxLength = 100;
            Txt_Description.MaxLength = 255;
        }

        private void ConfigurarComponentesDeshabilitados()
        {
            Txt_Codigo.Enabled = false;
        }

        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR POR CÓDIGO O NOMBRE...");
            ConfigurarPlaceHolder(Txt_Codigo, "CÓDIGO DE CATEGORÍA");
            ConfigurarPlaceHolder(Txt_UnitName, "NOMBRE DE LA CATEGORÍA");
            ConfigurarPlaceHolder(Txt_Description, "DESCRIPCIÓN DE LA CATEGORÍA");
        }

        private void ConfigurarPlaceHolder(TextBox textBox, string placeholder)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder && textBox.ForeColor == Color.Gray)
                {
                    textBox.Text = "";
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

        #endregion ConfigurarTextBox
        #region ConfigurarFiltros

        private void ConfigurarFiltros()
        {
            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_BuscarPor.Items.Clear();
            ComboBox_BuscarPor.Items.Add("TODOS");
            ComboBox_BuscarPor.Items.Add("POR CÓDIGO");
            ComboBox_BuscarPor.Items.Add("POR NOMBRE");
            ComboBox_BuscarPor.SelectedIndex = 0;
        }

        #endregion ConfigurarFiltros
        #region PanelSeleccion

        private void ConfigurarPanelSeleccion()
        {
            // El panel1 con Btn_Yes/Btn_No se muestra solo cuando el padre espera una selección
            // Si UserData es null (modo selección) mostramos el panel, si es gestión lo ocultamos
            bool esModoSeleccion = (CategoriaElegida == null && PanelSeleccion != null);
            PanelSeleccion.Visible = esModoSeleccion;
        }

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            if (_categoriaSeleccionada == null)
            {
                MessageBox.Show("Debe seleccionar una categoría de la tabla.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CategoriaElegida = _categoriaSeleccionada;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            CategoriaElegida = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion PanelSeleccion
        #region CodigoAutomatico

        private void CargarProximoCodigoCategoria()
        {
            try
            {
                string proximoCodigo = Ctrl_LocationCategories.ObtenerProximoCodigoCategoria();
                Txt_Codigo.Text = proximoCodigo;
                Txt_Codigo.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Txt_Codigo.Text = "ERROR";
            }
        }

        #endregion CodigoAutomatico
        #region ConfiguracionesTabla

        private void CargarCategorias()
        {
            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
            ActualizarInfoPaginacion();
        }

        private void RefrescarListado()
        {
            _categoriasList = Ctrl_LocationCategories.MostrarCategorias();
            AsignarDataSource();
        }

        private void AsignarDataSource()
        {
            if (_categoriasList == null)
            {
                Tabla.DataSource = null;
                return;
            }

            var data = _categoriasList.Select(c => new
            {
                c.LocationCategoryId,
                c.CategoryCode,
                c.CategoryName,
                c.Description,
                c.IsActive,
                c.CreatedDate,
                c.CreatedBy
            }).ToList();

            Tabla.DataSource = data;
        }

        private void ConfigurarTabla()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["LocationCategoryId"].Visible = false;
                Tabla.Columns["IsActive"].Visible = false;
                Tabla.Columns["CreatedDate"].Visible = false;
                Tabla.Columns["CreatedBy"].Visible = false;

                Tabla.Columns["CategoryCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["CategoryName"].HeaderText = "NOMBRE DE CATEGORÍA";
                Tabla.Columns["Description"].HeaderText = "DESCRIPCIÓN";
            }

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToResizeRows = false;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void AjustarColumnas()
        {
            if (Tabla.Columns.Count == 0) return;

            if (Tabla.Columns.Contains("CategoryCode"))
            {
                Tabla.Columns["CategoryCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CategoryCode"].FillWeight = 15;
            }
            if (Tabla.Columns.Contains("CategoryName"))
            {
                Tabla.Columns["CategoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["CategoryName"].FillWeight = 35;
            }
            if (Tabla.Columns.Contains("Description"))
            {
                Tabla.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Description"].FillWeight = 50;
            }
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0)
                {
                    _categoriaSeleccionada = null;
                    LimpiarFormulario();
                    return;
                }

                var fila = Tabla.SelectedRows[0];
                int id = (int)fila.Cells["LocationCategoryId"].Value;
                _categoriaSeleccionada = _categoriasList?.FirstOrDefault(c => c.LocationCategoryId == id);

                if (_categoriaSeleccionada != null)
                {
                    CargarDatosEnFormulario();
                    Txt_Selected.Text = _categoriaSeleccionada.CategoryName;
                    Txt_Selected.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar categoría: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDatosEnFormulario()
        {
            if (_categoriaSeleccionada == null) return;

            SetTextBoxFromValue(Txt_Codigo, _categoriaSeleccionada.CategoryCode, "CÓDIGO DE CATEGORÍA");
            SetTextBoxFromValue(Txt_UnitName, _categoriaSeleccionada.CategoryName, "NOMBRE DE LA CATEGORÍA");
            SetTextBoxFromValue(Txt_Description, _categoriaSeleccionada.Description, "DESCRIPCIÓN DE LA CATEGORÍA");
        }

        private void SetTextBoxFromValue(TextBox txt, string value, string placeholder)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                txt.Text = value;
                txt.ForeColor = Color.Black;
            }
            else
            {
                txt.Text = placeholder;
                txt.ForeColor = Color.Gray;
            }
        }

        #endregion ConfiguracionesTabla
        #region Paginacion

        private void ActualizarInfoPaginacion()
        {
            if (totalRegistros == 0)
                totalRegistros = Ctrl_LocationCategories.ContarTotalCategorias(_ultimoTextoBusqueda);

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            int inicioRango = (paginaActual - 1) * registrosPorPagina + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            // Este formulario no tiene Lbl_Paginas en el designer, los conteos van en Lbl_Conteo si existiera
            // Por ahora el conteo se actualiza vía título del formulario
        }

        #endregion Paginacion
        #region Search

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string valorBusqueda = "";
                if (Txt_ValorBuscado.Text != "BUSCAR POR CÓDIGO O NOMBRE..." &&
                    !string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                {
                    valorBusqueda = Txt_ValorBuscado.Text.Trim();
                }

                _ultimoTextoBusqueda = valorBusqueda;
                _ultimoFiltro1 = ComboBox_BuscarPor.SelectedItem?.ToString() ?? "TODOS";
                paginaActual = 1;

                _categoriasList = Ctrl_LocationCategories.BuscarCategorias(
                    textoBusqueda: valorBusqueda,
                    pageNumber: paginaActual,
                    pageSize: registrosPorPagina
                );

                AsignarDataSource();
                ConfigurarTabla();
                AjustarColumnas();

                totalRegistros = Ctrl_LocationCategories.ContarTotalCategorias(valorBusqueda);
                ActualizarInfoPaginacion();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            Txt_ValorBuscado.Text = "BUSCAR POR CÓDIGO O NOMBRE...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            ComboBox_BuscarPor.SelectedIndex = 0;

            _ultimoTextoBusqueda = "";
            _ultimoFiltro1 = "TODOS";
            paginaActual = 1;
            totalRegistros = 0;

            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
            ActualizarInfoPaginacion();
        }

        #endregion Search
        #region AsignacionFocus

        private void ConfigurarTabIndexYFocus()
        {
            Txt_ValorBuscado.TabIndex = 0;
            ComboBox_BuscarPor.TabIndex = 1;
            Txt_UnitName.TabIndex = 2;
            Txt_Description.TabIndex = 3;
            Btn_Save.TabIndex = 4;
            Btn_Update.TabIndex = 5;
            Btn_Inactive.TabIndex = 6;
            Btn_Clear.TabIndex = 7;

            Txt_ValorBuscado.Focus();
        }

        #endregion AsignacionFocus
        #region Validaciones

        private bool TienePlaceholder(TextBox txt, string placeholder)
        {
            return string.IsNullOrWhiteSpace(txt.Text) ||
                   txt.Text == placeholder ||
                   txt.ForeColor == Color.Gray;
        }

        private bool ValidarCamposObligatorios()
        {
            if (TienePlaceholder(Txt_UnitName, "NOMBRE DE LA CATEGORÍA"))
            {
                MessageBox.Show("El campo NOMBRE DE LA CATEGORÍA es obligatorio.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_UnitName.Focus();
                return false;
            }
            return true;
        }

        private Mdl_LocationCategories ConvertirAMayusculas(Mdl_LocationCategories cat)
        {
            if (cat == null) return null;
            cat.CategoryCode = cat.CategoryCode?.ToUpper();
            cat.CategoryName = cat.CategoryName?.ToUpper();
            cat.Description = cat.Description?.ToUpper();
            return cat;
        }

        #endregion Validaciones
        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCamposObligatorios()) return;

                var confirmacion = MessageBox.Show(
                    "¿ESTÁ SEGURO QUE DESEA REGISTRAR ESTA CATEGORÍA?",
                    "CONFIRMAR REGISTRO", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                var nuevaCategoria = new Mdl_LocationCategories
                {
                    CategoryCode = Ctrl_LocationCategories.ObtenerProximoCodigoCategoria(),
                    CategoryName = Txt_UnitName.Text.Trim(),
                    Description = TienePlaceholder(Txt_Description, "DESCRIPCIÓN DE LA CATEGORÍA")
                        ? null : Txt_Description.Text.Trim(),
                    IsActive = true,
                    CreatedBy = UserData?.UserId
                };

                nuevaCategoria = ConvertirAMayusculas(nuevaCategoria);

                int resultado = Ctrl_LocationCategories.RegistrarCategoria(nuevaCategoria);

                if (resultado > 0)
                {
                    MessageBox.Show("CATEGORÍA REGISTRADA EXITOSAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    totalRegistros = 0;
                    RefrescarListado();
                    ConfigurarTabla();
                    AjustarColumnas();
                    ActualizarInfoPaginacion();
                    CargarProximoCodigoCategoria();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO REGISTRAR LA CATEGORÍA.", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                if (_categoriaSeleccionada == null)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CATEGORÍA DE LA TABLA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarCamposObligatorios()) return;

                var confirmacion = MessageBox.Show(
                    "¿ESTÁ SEGURO QUE DESEA ACTUALIZAR ESTA CATEGORÍA?",
                    "CONFIRMAR ACTUALIZACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                _categoriaSeleccionada.CategoryName = Txt_UnitName.Text.Trim();
                _categoriaSeleccionada.Description = TienePlaceholder(Txt_Description, "DESCRIPCIÓN DE LA CATEGORÍA")
                    ? null : Txt_Description.Text.Trim();

                _categoriaSeleccionada = ConvertirAMayusculas(_categoriaSeleccionada);

                int resultado = Ctrl_LocationCategories.ActualizarCategoria(_categoriaSeleccionada);

                if (resultado > 0)
                {
                    MessageBox.Show("CATEGORÍA ACTUALIZADA EXITOSAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    totalRegistros = 0;
                    RefrescarListado();
                    ConfigurarTabla();
                    AjustarColumnas();
                    ActualizarInfoPaginacion();
                    CargarProximoCodigoCategoria();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO ACTUALIZAR LA CATEGORÍA.", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            try
            {
                if (_categoriaSeleccionada == null)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CATEGORÍA DE LA TABLA.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO QUE DESEA INACTIVAR LA CATEGORÍA \"{_categoriaSeleccionada.CategoryName}\"?\n\nESTA ACCIÓN AFECTARÁ LAS SEDES QUE USEN ESTA CATEGORÍA.",
                    "CONFIRMAR INACTIVACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion != DialogResult.Yes) return;

                int resultado = Ctrl_LocationCategories.InactivarCategoria(_categoriaSeleccionada.LocationCategoryId);

                if (resultado > 0)
                {
                    MessageBox.Show("CATEGORÍA INACTIVADA EXITOSAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    totalRegistros = 0;
                    RefrescarListado();
                    ConfigurarTabla();
                    AjustarColumnas();
                    ActualizarInfoPaginacion();
                    CargarProximoCodigoCategoria();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO INACTIVAR LA CATEGORÍA.", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inactivar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            _categoriaSeleccionada = null;

            Txt_Codigo.Text = "CÓDIGO DE CATEGORÍA";
            Txt_Codigo.ForeColor = Color.Gray;

            Txt_UnitName.Text = "NOMBRE DE LA CATEGORÍA";
            Txt_UnitName.ForeColor = Color.Gray;

            Txt_Description.Text = "DESCRIPCIÓN DE LA CATEGORÍA";
            Txt_Description.ForeColor = Color.Gray;

            Txt_Selected.Text = "";

            if (Tabla.Rows.Count > 0)
                Tabla.ClearSelection();

            CargarProximoCodigoCategoria();
        }

        #endregion CRUD
    }
}