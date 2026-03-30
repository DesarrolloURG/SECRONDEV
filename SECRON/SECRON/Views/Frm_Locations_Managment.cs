using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Locations_Managment : Form
    {
        #region PropiedadesIniciales
        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_Locations> locationsList = new List<Mdl_Locations>();
        private Mdl_Locations _ubicacionSeleccionada = null;
        private bool _cargandoUbicacion = false;

        public Frm_Locations_Managment()
        {
            InitializeComponent();
            this.Load += Frm_Locations_Managment_Load;
            CargarFiltrosBusqueda();
            ConfigurarPlaceHoldersTextbox();
            CargarLocationCategories();
            //RefrescarListado();
        }
        #endregion PropiedadesIniciales

        private void CargarFiltrosBusqueda()
        {
            // FILTRO 1: Tipo de búsqueda
            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("POR SEDE");
            Filtro1.Items.Add("POR DEPARTAMENTO");
            Filtro1.Items.Add("POR MUNICIPIO");
            Filtro1.SelectedIndex = 0;

            // FILTRO 2: Estado
            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS");
            Filtro2.Items.Add("ACTIVOS");
            Filtro2.Items.Add("INACTIVOS");
            Filtro2.SelectedIndex = 0;
        }

        private void Frm_Locations_Managment_Load(object sender, EventArgs e)
        {
            CargarCountries();
            RefrescarListado();
        }

        public void RefrescarListado()
        {
            locationsList = Ctrl_Locations.MostrarUbicaciones();
            Tabla.DataSource = null;
            Tabla.DataSource = locationsList;
            ConfigurarTabla();
        }

        public void ConfigurarTabla()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["LocationCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["LocationName"].HeaderText = "NOMBRE";
                Tabla.Columns["Address"].HeaderText = "DIRECCIÓN";
                Tabla.Columns["City"].HeaderText = "CIUDAD";
                Tabla.Columns["CountryName"].HeaderText = "PAÍS";
                Tabla.Columns["DepartmentName"].HeaderText = "DEPARTAMENTO";
                Tabla.Columns["MunicipalityName"].HeaderText = "MUNICIPIO";

                Tabla.Columns["LocationId"].Visible = false;
                Tabla.Columns["IsActive"].Visible = false;
                Tabla.Columns["CreatedDate"].Visible = false;
                Tabla.Columns["CreatedBy"].Visible = false;
                Tabla.Columns["ModifiedDate"].Visible = false;
                Tabla.Columns["ModifiedBy"].Visible = false;
                Tabla.Columns["LocationCategoryId"].Visible = false;
                Tabla.Columns["PrimaryWarehouseId"].Visible = false;
                Tabla.Columns["MunicipalityId"].Visible = false;
            }

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToDeleteRows = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            CargarDatosUbicacionSeleccionada();
        }

        private void CargarDatosUbicacionSeleccionada()
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0)
                    return;

                _cargandoUbicacion = true;

                DataGridViewRow fila = Tabla.SelectedRows[0];
                int locationId = Convert.ToInt32(fila.Cells["LocationId"].Value);

                _ubicacionSeleccionada = Ctrl_Locations.ObtenerUbicacionPorId(locationId);

                if (_ubicacionSeleccionada != null)
                {
                    // TextBox principales
                    Txt_Code.Text = _ubicacionSeleccionada.LocationCode ?? "";
                    Txt_Name.Text = _ubicacionSeleccionada.LocationName ?? "";
                    Txt_Address.Text = _ubicacionSeleccionada.Address ?? "";
                    Txt_City.Text = _ubicacionSeleccionada.City ?? "";

                    Txt_Code.ForeColor = Color.Black;
                    Txt_Name.ForeColor = Color.Black;
                    Txt_Address.ForeColor = Color.Black;
                    Txt_City.ForeColor = Color.Black;

                    // Category
                    if (_ubicacionSeleccionada.LocationCategoryId.HasValue)
                        ComboBox_LocationCategoryId.SelectedValue = _ubicacionSeleccionada.LocationCategoryId.Value;
                    else
                        ComboBox_LocationCategoryId.SelectedIndex = 0;

                    // ===== COUNTRY =====
                    int countryIndex = ComboBox_Country.FindStringExact(_ubicacionSeleccionada.CountryName ?? "");
                    ComboBox_Country.SelectedIndex = countryIndex >= 0 ? countryIndex : 0;

                    int countryId = 0;
                    if (ComboBox_Country.SelectedValue != null)
                        int.TryParse(ComboBox_Country.SelectedValue.ToString(), out countryId);

                    // ===== DEPARTMENT =====
                    CargarDepartments(countryId);

                    int departmentIndex = ComboBox_Department.FindStringExact(_ubicacionSeleccionada.DepartmentName ?? "");
                    ComboBox_Department.SelectedIndex = departmentIndex >= 0 ? departmentIndex : 0;

                    int departmentId = 0;
                    if (ComboBox_Department.SelectedValue != null)
                        int.TryParse(ComboBox_Department.SelectedValue.ToString(), out departmentId);

                    // ===== MUNICIPALITY =====
                    CargarMunicipalities(departmentId);

                    if (_ubicacionSeleccionada.MunicipalityId.HasValue)
                        ComboBox_Municipality.SelectedValue = _ubicacionSeleccionada.MunicipalityId.Value;
                    else
                    {
                        int municipalityIndex = ComboBox_Municipality.FindStringExact(_ubicacionSeleccionada.MunicipalityName ?? "");
                        ComboBox_Municipality.SelectedIndex = municipalityIndex >= 0 ? municipalityIndex : 0;
                    }

                    Txt_City.Text = _ubicacionSeleccionada.City ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos de la sede: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoUbicacion = false;
            }
        }

        private void CargarCountries()
        {
            bool estadoAnterior = _cargandoUbicacion;
            _cargandoUbicacion = true;

            try
            {
                var lista = Ctrl_Locations.ObtenerCountriesActivos();
                lista.Insert(0, new Mdl_Country { CountryId = 0, CountryName = "SELECCIONE" });

                ComboBox_Country.DataSource = null;
                ComboBox_Country.DataSource = lista;
                ComboBox_Country.DisplayMember = "CountryName";
                ComboBox_Country.ValueMember = "CountryId";
                ComboBox_Country.SelectedIndex = 0;

                ComboBox_Department.DataSource = null;
                ComboBox_Municipality.DataSource = null;
                Txt_City.Text = "";
            }
            finally
            {
                _cargandoUbicacion = estadoAnterior;
            }
        }

        private void CargarDepartments(int countryId)
        {
            bool estadoAnterior = _cargandoUbicacion;
            _cargandoUbicacion = true;

            try
            {
                var lista = new List<Mdl_Department>();

                if (countryId > 0)
                    lista = Ctrl_Locations.ObtenerDepartmentsPorCountry(countryId);

                lista.Insert(0, new Mdl_Department { DepartmentId = 0, DepartmentName = "SELECCIONE" });

                ComboBox_Department.DataSource = null;
                ComboBox_Department.DataSource = lista;
                ComboBox_Department.DisplayMember = "DepartmentName";
                ComboBox_Department.ValueMember = "DepartmentId";
                ComboBox_Department.SelectedIndex = 0;

                ComboBox_Municipality.DataSource = null;
                Txt_City.Text = "";
            }
            finally
            {
                _cargandoUbicacion = estadoAnterior;
            }
        }

        private void CargarMunicipalities(int departmentId)
        {
            bool estadoAnterior = _cargandoUbicacion;
            _cargandoUbicacion = true;

            try
            {
                var lista = new List<Mdl_Municipality>();

                if (departmentId > 0)
                    lista = Ctrl_Locations.ObtenerMunicipalitiesPorDepartment(departmentId);

                lista.Insert(0, new Mdl_Municipality { MunicipalityId = 0, MunicipalityName = "SELECCIONE" });

                ComboBox_Municipality.DataSource = null;
                ComboBox_Municipality.DataSource = lista;
                ComboBox_Municipality.DisplayMember = "MunicipalityName";
                ComboBox_Municipality.ValueMember = "MunicipalityId";
                ComboBox_Municipality.SelectedIndex = 0;

                Txt_City.Text = "";
            }
            finally
            {
                _cargandoUbicacion = estadoAnterior;
            }
        }

        private void ComboBox_Country_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoUbicacion) return;
            if (ComboBox_Country.SelectedValue == null) return;

            int countryId;
            if (int.TryParse(ComboBox_Country.SelectedValue.ToString(), out countryId))
            {
                CargarDepartments(countryId);
                ActualizarTextoCity();
            }
        }

        private void ComboBox_Department_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoUbicacion) return;
            if (ComboBox_Department.SelectedValue == null) return;

            int departmentId;
            if (int.TryParse(ComboBox_Department.SelectedValue.ToString(), out departmentId))
            {
                CargarMunicipalities(departmentId);
                ActualizarTextoCity();
            }
        }

        private void ComboBox_Municipality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoUbicacion) return;

            ActualizarTextoCity();
        }

        private void ActualizarTextoCity()
        {
            var country = ComboBox_Country.SelectedItem as Mdl_Country;
            var department = ComboBox_Department.SelectedItem as Mdl_Department;
            var municipality = ComboBox_Municipality.SelectedItem as Mdl_Municipality;

            if (country != null && country.CountryId > 0 &&
                department != null && department.DepartmentId > 0 &&
                municipality != null && municipality.MunicipalityId > 0)
            {
                Txt_City.Text = $"{country.CountryName}, {department.DepartmentName}, {municipality.MunicipalityName}";
            }
            else
            {
                Txt_City.Text = "";
            }
        }

        private readonly Dictionary<TextBox, string> _placeholders = new Dictionary<TextBox, string>();

        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR SEDE, DEPARTAMENTO O MUNICIPIO...");
            ConfigurarPlaceHolder(Txt_Code, "CÓDIGO DE SEDE");
            ConfigurarPlaceHolder(Txt_Name, "NOMBRE DE SEDE");
            ConfigurarPlaceHolder(Txt_Address, "DIRECCIÓN");
        }

        private void ConfigurarPlaceHolder(TextBox txt, string placeholder)
        {
            if (_placeholders.ContainsKey(txt))
                _placeholders[txt] = placeholder;
            else
                _placeholders.Add(txt, placeholder);

            txt.Enter -= TextBox_EnterPlaceholder;
            txt.Leave -= TextBox_LeavePlaceholder;

            txt.Enter += TextBox_EnterPlaceholder;
            txt.Leave += TextBox_LeavePlaceholder;

            AplicarPlaceHolderSiVacio(txt);
        }

        private void TextBox_EnterPlaceholder(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (!_placeholders.ContainsKey(txt)) return;

            string placeholder = _placeholders[txt];

            if (txt.ForeColor == Color.Gray && txt.Text == placeholder)
            {
                txt.Text = "";
                txt.ForeColor = Color.Black;
            }
        }

        private void TextBox_LeavePlaceholder(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;

            AplicarPlaceHolderSiVacio(txt);
        }

        private void AplicarPlaceHolderSiVacio(TextBox txt)
        {
            if (txt == null) return;
            if (!_placeholders.ContainsKey(txt)) return;

            string placeholder = _placeholders[txt];

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = placeholder;
                txt.ForeColor = Color.Gray;
            }
            else if (txt.Text != placeholder)
            {
                txt.ForeColor = Color.Black;
            }
        }

        private bool TienePlaceholder(TextBox txt)
        {
            return _placeholders.ContainsKey(txt) &&
                   txt.ForeColor == Color.Gray &&
                   txt.Text == _placeholders[txt];
        }

        private void EjecutarBusqueda()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string valorBusqueda = TienePlaceholder(Txt_ValorBuscado)
                    ? ""
                    : Txt_ValorBuscado.Text.Trim();

                string tipoFiltro = Filtro1.SelectedItem?.ToString() ?? "TODOS";
                string filtroEstado = Filtro2.SelectedItem?.ToString() ?? "TODOS";

                bool? isActive = null;

                if (filtroEstado == "ACTIVOS")
                    isActive = true;
                else if (filtroEstado == "INACTIVOS")
                    isActive = false;

                var resultados = Ctrl_Locations.BuscarUbicaciones(
                    textoBusqueda: valorBusqueda,
                    tipoFiltro: tipoFiltro,
                    isActive: isActive
                );

                Tabla.DataSource = null;
                Tabla.DataSource = resultados;
                ConfigurarTabla();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error en búsqueda: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            EjecutarBusqueda();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                EjecutarBusqueda();
            }
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            try
            {
                _cargandoUbicacion = true;

                Txt_ValorBuscado.Text = "";
                Filtro1.SelectedIndex = 0;
                Filtro2.SelectedIndex = 0;
            }
            finally
            {
                _cargandoUbicacion = false;
            }

            AplicarPlaceHolderSiVacio(Txt_ValorBuscado);
            RefrescarListado();
        }

        private void CargarLocationCategories()
        {
            try
            {
                var lista = Ctrl_Locations.ObtenerLocationCategoriesActivas();
                lista.Insert(0, new Mdl_LocationCategory
                {
                    LocationCategoryId = 0,
                    CategoryCode = "",
                    CategoryName = "SELECCIONE"
                });

                ComboBox_LocationCategoryId.DataSource = null;
                ComboBox_LocationCategoryId.DataSource = lista;
                ComboBox_LocationCategoryId.DisplayMember = "DisplayText";
                ComboBox_LocationCategoryId.ValueMember = "LocationCategoryId";
                ComboBox_LocationCategoryId.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar categorías de sede: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCamposUbicacion())
                    return;

                string codigoSede = TienePlaceholder(Txt_Code) ? "" : Txt_Code.Text.Trim();
                if (Ctrl_Locations.ExisteCodigoUbicacion(codigoSede))
                {
                    MessageBox.Show("Ya existe una sede registrada con ese código. Ingrese un código diferente.",
                        "Código duplicado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    Txt_Code.Focus();
                    return;
                }

                var confirmacion = MessageBox.Show(
                    "¿Está seguro que desea registrar esta sede?",
                    "Confirmar Registro",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;


                var nuevaUbicacion = new Mdl_Locations
                {
                    LocationCode = TienePlaceholder(Txt_Code) ? null : Txt_Code.Text.Trim(),
                    LocationName = TienePlaceholder(Txt_Name) ? null : Txt_Name.Text.Trim(),
                    Address = TienePlaceholder(Txt_Address) ? null : Txt_Address.Text.Trim(),
                    City = Txt_City.Text.Trim(),
                    MunicipalityId = ObtenerValorComboNullable(ComboBox_Municipality),
                    LocationCategoryId = ObtenerValorComboNullable(ComboBox_LocationCategoryId),
                    PrimaryWarehouseId = ObtenerValorComboNullable(ComboBox_PrimaryWarehouseId),
                    IsActive = true,
                    CreatedBy = UserData?.UserId ?? 1
                };

                int resultado = Ctrl_Locations.RegistrarUbicacion(nuevaUbicacion);

                if (resultado > 0)
                {
                    MessageBox.Show("Sede registrada exitosamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarFormularioLocations();
                    RefrescarListado();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar la sede.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la sede: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int? ObtenerValorComboNullable(ComboBox combo)
        {
            if (combo.SelectedValue == null)
                return null;

            int valor;
            if (int.TryParse(combo.SelectedValue.ToString(), out valor) && valor > 0)
                return valor;

            return null;
        }

        private bool ValidarCamposUbicacion()
        {
            if (TienePlaceholder(Txt_Code) || string.IsNullOrWhiteSpace(Txt_Code.Text))
            {
                MessageBox.Show("Debe ingresar el código de la sede.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Code.Focus();
                return false;
            }

            if (TienePlaceholder(Txt_Name) || string.IsNullOrWhiteSpace(Txt_Name.Text))
            {
                MessageBox.Show("Debe ingresar el nombre de la sede.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Name.Focus();
                return false;
            }

            if (TienePlaceholder(Txt_Address) || string.IsNullOrWhiteSpace(Txt_Address.Text))
            {
                MessageBox.Show("Debe ingresar la dirección de la sede.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Address.Focus();
                return false;
            }

            if (ObtenerValorComboNullable(ComboBox_Country) == null)
            {
                MessageBox.Show("Debe seleccionar un país.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Country.Focus();
                return false;
            }

            if (ObtenerValorComboNullable(ComboBox_Department) == null)
            {
                MessageBox.Show("Debe seleccionar un departamento.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Department.Focus();
                return false;
            }

            if (ObtenerValorComboNullable(ComboBox_Municipality) == null)
            {
                MessageBox.Show("Debe seleccionar un municipio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Municipality.Focus();
                return false;
            }

            if (ObtenerValorComboNullable(ComboBox_LocationCategoryId) == null)
            {
                MessageBox.Show("Debe seleccionar la categoría de la sede.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_LocationCategoryId.Focus();
                return false;
            }

            return true;
        }

        private void LimpiarFormularioLocations()
        {
            try
            {
                _cargandoUbicacion = true;

                _ubicacionSeleccionada = null;

                Txt_Code.Text = "";
                Txt_Name.Text = "";
                Txt_Address.Text = "";
                Txt_City.Text = "";

                AplicarPlaceHolderSiVacio(Txt_Code);
                AplicarPlaceHolderSiVacio(Txt_Name);
                AplicarPlaceHolderSiVacio(Txt_Address);

                ComboBox_Country.SelectedIndex = 0;
                ComboBox_Department.DataSource = null;
                ComboBox_Municipality.DataSource = null;
                ComboBox_LocationCategoryId.SelectedIndex = 0;

                if (Tabla.Rows.Count > 0)
                    Tabla.ClearSelection();

                if (ComboBox_PrimaryWarehouseId.Items.Count > 0)
                    ComboBox_PrimaryWarehouseId.SelectedIndex = 0;
            }
            finally
            {
                _cargandoUbicacion = false;
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                if (_ubicacionSeleccionada == null || _ubicacionSeleccionada.LocationId == 0)
                {
                    MessageBox.Show("Debe seleccionar una sede para actualizar.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarCamposUbicacion())
                    return;

                string codigoSede = TienePlaceholder(Txt_Code) ? "" : Txt_Code.Text.Trim();

                if (Ctrl_Locations.ExisteCodigoUbicacion(codigoSede, _ubicacionSeleccionada.LocationId))
                {
                    MessageBox.Show("Ya existe otra sede registrada con ese código. Ingrese un código diferente.",
                        "Código duplicado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    Txt_Code.Focus();
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea actualizar la sede {_ubicacionSeleccionada.LocationName}?",
                    "Confirmar actualización",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                _ubicacionSeleccionada.LocationCode = TienePlaceholder(Txt_Code) ? null : Txt_Code.Text.Trim();
                _ubicacionSeleccionada.LocationName = TienePlaceholder(Txt_Name) ? null : Txt_Name.Text.Trim();
                _ubicacionSeleccionada.Address = TienePlaceholder(Txt_Address) ? null : Txt_Address.Text.Trim();
                _ubicacionSeleccionada.City = Txt_City.Text.Trim();
                _ubicacionSeleccionada.MunicipalityId = ObtenerValorComboNullable(ComboBox_Municipality);
                _ubicacionSeleccionada.LocationCategoryId = ObtenerValorComboNullable(ComboBox_LocationCategoryId);
                _ubicacionSeleccionada.PrimaryWarehouseId = ObtenerValorComboNullable(ComboBox_PrimaryWarehouseId);
                _ubicacionSeleccionada.ModifiedBy = UserData?.UserId ?? 1;

                int resultado = Ctrl_Locations.ActualizarUbicacion(_ubicacionSeleccionada);

                if (resultado > 0)
                {
                    MessageBox.Show("Sede actualizada exitosamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarFormularioLocations();
                    RefrescarListado();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar la sede.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la sede: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            try
            {
                if (_ubicacionSeleccionada == null || _ubicacionSeleccionada.LocationId == 0)
                {
                    MessageBox.Show("Debe seleccionar una sede para inactivar.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea inactivar la sede {_ubicacionSeleccionada.LocationName}?",
                    "Confirmar inactivación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                int resultado = Ctrl_Locations.InactivarUbicacion(
                    _ubicacionSeleccionada.LocationId,
                    UserData?.UserId ?? 1
                );

                if (resultado > 0)
                {
                    MessageBox.Show("Sede inactivada exitosamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarFormularioLocations();
                    RefrescarListado();
                }
                else
                {
                    MessageBox.Show("No se pudo inactivar la sede.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inactivar la sede: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarFormularioLocations();
        }
    }
}