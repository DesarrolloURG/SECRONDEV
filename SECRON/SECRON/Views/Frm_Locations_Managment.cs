using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Locations_Managment : Form
    {
        #region PropiedadesIniciales

        public Mdl_Security_UserInfo UserData { get; set; }

        private readonly Dictionary<TextBox, string> _placeholders = new Dictionary<TextBox, string>();

        private List<Mdl_Locations> locationsList = new List<Mdl_Locations>();
        private Mdl_Locations _ubicacionSeleccionada = null;
        private bool _cargandoUbicacion = false;

        private string _ultimoTextoBusqueda = "";
        private string _ultimoTipoFiltro = "TODOS";
        private bool? _ultimoIsActive = null;

        // Variables de paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;

        // ToolStrip de paginación
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        public Frm_Locations_Managment()
        {
            InitializeComponent();
            this.Load += Frm_Locations_Managment_Load;

            CargarFiltrosBusqueda();
            ConfigurarPlaceHoldersTextbox();
            CargarLocationCategories();
        }

        #endregion

        #region CargaInicial

        private async void Frm_Locations_Managment_Load(object sender, EventArgs e)
        {
            ConfigurarTabIndexYFocus();
            CargarCountries();
            CrearToolStripPaginacion();
            paginaActual = 1;

            if (UserData != null)
            {
                await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
                ConfigurarControlesPorPermisos();
            }

            RefrescarListado();
            ActualizarInfoPaginacion();

            if (Tabla.Rows.Count > 0)
                Tabla.ClearSelection();
        }

        #endregion

        #region Tabla

        public void RefrescarListado()
        {
            locationsList = Ctrl_Locations.MostrarUbicaciones(paginaActual, registrosPorPagina);

            Tabla.DataSource = null;
            Tabla.DataSource = locationsList;

            ConfigurarTabla();

            totalRegistros = Ctrl_Locations.ContarTotalUbicaciones(
                textoBusqueda: _ultimoTextoBusqueda,
                tipoFiltro: _ultimoTipoFiltro,
                isActive: _ultimoIsActive
            );

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
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

                if (_ubicacionSeleccionada == null)
                    return;

                AsignarTextoReal(Txt_Code, _ubicacionSeleccionada.LocationCode);
                AsignarTextoReal(Txt_Name, _ubicacionSeleccionada.LocationName);
                AsignarTextoReal(Txt_Address, _ubicacionSeleccionada.Address);
                AsignarTextoReal(Txt_City, _ubicacionSeleccionada.City);

                if (_ubicacionSeleccionada.LocationCategoryId.HasValue)
                    ComboBox_LocationCategoryId.SelectedValue = _ubicacionSeleccionada.LocationCategoryId.Value;
                else
                    ComboBox_LocationCategoryId.SelectedIndex = 0;

                CargarBodegasPrimarias(_ubicacionSeleccionada.LocationId);
                if (_ubicacionSeleccionada.PrimaryWarehouseId.HasValue)
                    ComboBox_PrimaryWarehouseId.SelectedValue = _ubicacionSeleccionada.PrimaryWarehouseId.Value;
                else
                    ComboBox_PrimaryWarehouseId.SelectedIndex = 0;

                int countryIndex = ComboBox_Country.FindStringExact(_ubicacionSeleccionada.CountryName ?? "");
                ComboBox_Country.SelectedIndex = countryIndex >= 0 ? countryIndex : 0;

                int countryId = ObtenerValorCombo(ComboBox_Country);
                CargarDepartments(countryId);

                int departmentIndex = ComboBox_Department.FindStringExact(_ubicacionSeleccionada.DepartmentName ?? "");
                ComboBox_Department.SelectedIndex = departmentIndex >= 0 ? departmentIndex : 0;

                int departmentId = ObtenerValorCombo(ComboBox_Department);
                CargarMunicipalities(departmentId);

                if (_ubicacionSeleccionada.MunicipalityId.HasValue)
                {
                    ComboBox_Municipality.SelectedValue = _ubicacionSeleccionada.MunicipalityId.Value;
                }
                else
                {
                    int municipalityIndex = ComboBox_Municipality.FindStringExact(_ubicacionSeleccionada.MunicipalityName ?? "");
                    ComboBox_Municipality.SelectedIndex = municipalityIndex >= 0 ? municipalityIndex : 0;
                }

                Txt_City.Text = _ubicacionSeleccionada.City ?? "";
                Txt_City.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar datos de la sede: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                _cargandoUbicacion = false;
            }
        }

        #endregion

        #region ComboboxesGeograficos

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

                lista.Insert(0, new Mdl_Department
                {
                    DepartmentId = 0,
                    DepartmentName = "SELECCIONE"
                });

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

                lista.Insert(0, new Mdl_Municipality
                {
                    MunicipalityId = 0,
                    MunicipalityName = "SELECCIONE"
                });

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
            if (_cargandoUbicacion || ComboBox_Country.SelectedValue == null)
                return;

            int countryId;
            if (int.TryParse(ComboBox_Country.SelectedValue.ToString(), out countryId))
            {
                CargarDepartments(countryId);
                ActualizarTextoCity();
            }
        }

        private void ComboBox_Department_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoUbicacion || ComboBox_Department.SelectedValue == null)
                return;

            int departmentId;
            if (int.TryParse(ComboBox_Department.SelectedValue.ToString(), out departmentId))
            {
                CargarMunicipalities(departmentId);
                ActualizarTextoCity();
            }
        }

        private void ComboBox_Municipality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoUbicacion)
                return;

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
                MessageBox.Show(
                    "Error al cargar categorías de sede: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // MÉTODO: Cargar el combo de bodega principal, limitado a las bodegas activas de la sede seleccionada.
        // Si no hay sede seleccionada (registro nuevo) el combo queda vacío/deshabilitado,
        // ya que una bodega no puede existir sin una sede (LocationId) previamente creada.
        private void CargarBodegasPrimarias(int? locationId)
        {
            bool estadoAnterior = _cargandoUbicacion;
            _cargandoUbicacion = true;

            try
            {
                ComboBox_PrimaryWarehouseId.DataSource = null;

                if (!locationId.HasValue || locationId.Value <= 0)
                {
                    ComboBox_PrimaryWarehouseId.Items.Clear();
                    ComboBox_PrimaryWarehouseId.Enabled = false;
                    return;
                }

                var bodegas = Ctrl_Warehouses.ObtenerBodegasPorLocation(locationId.Value);
                bodegas.Insert(0, new KeyValuePair<int, string>(0, "SELECCIONE"));

                ComboBox_PrimaryWarehouseId.DataSource = bodegas;
                ComboBox_PrimaryWarehouseId.DisplayMember = "Value";
                ComboBox_PrimaryWarehouseId.ValueMember = "Key";
                ComboBox_PrimaryWarehouseId.Enabled = true;
                ComboBox_PrimaryWarehouseId.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar bodegas de la sede: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                _cargandoUbicacion = estadoAnterior;
            }
        }

        #endregion

        #region Placeholders

        private void ConfigurarTabIndexYFocus()
        {
            // ===== CONTROLES DE BÚSQUEDA =====
            Txt_ValorBuscado.TabIndex = 0;
            Filtro1.TabIndex = 1;
            Filtro2.TabIndex = 2;
            Btn_Search.TabIndex = 3;
            Btn_CleanSearch.TabIndex = 4;

            // ===== FORMULARIO DE SEDE =====
            Txt_Code.TabIndex = 5;
            Txt_Name.TabIndex = 6;
            ComboBox_LocationCategoryId.TabIndex = 7;
            ComboBox_PrimaryWarehouseId.TabIndex = 8;
            ComboBox_Country.TabIndex = 9;
            ComboBox_Department.TabIndex = 10;
            ComboBox_Municipality.TabIndex = 11;
            Txt_Address.TabIndex = 12;

            // Txt_City normalmente no debería entrar al TAB porque es automático
            Txt_City.TabStop = false;

            // ===== BOTONES CRUD =====
            Btn_Save.TabIndex = 13;
            Btn_Update.TabIndex = 14;
            Btn_Inactive.TabIndex = 15;
            Btn_Clear.TabIndex = 16;

            // ===== TABLA =====
            Tabla.TabIndex = 17;

            // Controles no navegables
            Panel_Superior.TabStop = false;
            Panel_Derecho.TabStop = false;
            Panel_Izquierdo.TabStop = false;
            PanelToolStrip.TabStop = false;
            Panel_Busqueda.TabStop = false;
            Panel_CRUD.TabStop = false;
            Panel_Informacion.TabStop = false;
            vScrollBar.TabStop = false;

            // Foco inicial
            Txt_ValorBuscado.Focus();
        }

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

            if (txt == null || !_placeholders.ContainsKey(txt))
                return;

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

            if (txt == null)
                return;

            AplicarPlaceHolderSiVacio(txt);
        }

        private void AplicarPlaceHolderSiVacio(TextBox txt)
        {
            if (txt == null || !_placeholders.ContainsKey(txt))
                return;

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

        #endregion

        #region FiltrosBusqueda

        private void CargarFiltrosBusqueda()
        {
            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("POR SEDE");
            Filtro1.Items.Add("POR DEPARTAMENTO");
            Filtro1.Items.Add("POR MUNICIPIO");
            Filtro1.SelectedIndex = 0;

            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS");
            Filtro2.Items.Add("ACTIVOS");
            Filtro2.Items.Add("INACTIVOS");
            Filtro2.SelectedIndex = 0;
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

                _ultimoTextoBusqueda = valorBusqueda;
                _ultimoTipoFiltro = tipoFiltro;
                _ultimoIsActive = isActive;
                paginaActual = 1;

                locationsList = Ctrl_Locations.BuscarUbicaciones(
                    textoBusqueda: valorBusqueda,
                    tipoFiltro: tipoFiltro,
                    isActive: isActive,
                    pageNumber: paginaActual,
                    pageSize: registrosPorPagina
                );

                Tabla.DataSource = null;
                Tabla.DataSource = locationsList;
                ConfigurarTabla();

                totalRegistros = Ctrl_Locations.ContarTotalUbicaciones(
                    textoBusqueda: valorBusqueda,
                    tipoFiltro: tipoFiltro,
                    isActive: isActive
                );

                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
                ActualizarInfoPaginacion();

                LimpiarSeleccionTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error en búsqueda: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            if (!Btn_Search.Enabled) return;
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
            if (!Btn_CleanSearch.Enabled) return;

            try
            {
                _cargandoUbicacion = true;

                Txt_ValorBuscado.Text = "";
                Filtro1.SelectedIndex = 0;
                Filtro2.SelectedIndex = 0;

                _ultimoTextoBusqueda = "";
                _ultimoTipoFiltro = "TODOS";
                _ultimoIsActive = null;
                paginaActual = 1;
            }
            finally
            {
                _cargandoUbicacion = false;
            }

            AplicarPlaceHolderSiVacio(Txt_ValorBuscado);
            RefrescarListado();
            ActualizarInfoPaginacion();
            LimpiarSeleccionTabla();
        }

        #endregion

        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!Btn_Save.Enabled) return;
            try
            {
                if (!ValidarCamposUbicacion())
                    return;

                string codigoSede = TienePlaceholder(Txt_Code) ? "" : Txt_Code.Text.Trim();

                if (Ctrl_Locations.ExisteCodigoUbicacion(codigoSede))
                {
                    MessageBox.Show(
                        "Ya existe una sede registrada con ese código. Ingrese un código diferente.",
                        "Código duplicado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
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
                    LocationCode = TienePlaceholder(Txt_Code) ? null : Txt_Code.Text.Trim().ToUpper(),
                    LocationName = TienePlaceholder(Txt_Name) ? null : Txt_Name.Text.Trim().ToUpper(),
                    Address = TienePlaceholder(Txt_Address) ? null : Txt_Address.Text.Trim().ToUpper(),
                    City = Txt_City.Text.Trim().ToUpper(),
                    MunicipalityId = ObtenerValorComboNullable(ComboBox_Municipality),
                    LocationCategoryId = ObtenerValorComboNullable(ComboBox_LocationCategoryId),
                    PrimaryWarehouseId = ObtenerValorComboNullable(ComboBox_PrimaryWarehouseId),
                    IsActive = true,
                    CreatedBy = UserData?.UserId ?? 1
                };

                int resultado = Ctrl_Locations.RegistrarUbicacion(nuevaUbicacion);

                if (resultado > 0)
                {
                    MessageBox.Show(
                        "Sede registrada exitosamente.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LimpiarFormularioLocations();
                    RefrescarListado();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo registrar la sede.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al guardar la sede: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (!Btn_Update.Enabled) return;

            try
            {
                if (_ubicacionSeleccionada == null || _ubicacionSeleccionada.LocationId == 0)
                {
                    MessageBox.Show(
                        "Debe seleccionar una sede para actualizar.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (!ValidarCamposUbicacion())
                    return;

                string codigoSede = TienePlaceholder(Txt_Code) ? "" : Txt_Code.Text.Trim();

                if (Ctrl_Locations.ExisteCodigoUbicacion(codigoSede, _ubicacionSeleccionada.LocationId))
                {
                    MessageBox.Show(
                        "Ya existe otra sede registrada con ese código. Ingrese un código diferente.",
                        "Código duplicado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
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

                _ubicacionSeleccionada.LocationCode = TienePlaceholder(Txt_Code) ? null : Txt_Code.Text.Trim().ToUpper();
                _ubicacionSeleccionada.LocationName = TienePlaceholder(Txt_Name) ? null : Txt_Name.Text.Trim().ToUpper();
                _ubicacionSeleccionada.Address = TienePlaceholder(Txt_Address) ? null : Txt_Address.Text.Trim().ToUpper();
                _ubicacionSeleccionada.City = Txt_City.Text.Trim().ToUpper();
                _ubicacionSeleccionada.MunicipalityId = ObtenerValorComboNullable(ComboBox_Municipality);
                _ubicacionSeleccionada.LocationCategoryId = ObtenerValorComboNullable(ComboBox_LocationCategoryId);
                _ubicacionSeleccionada.PrimaryWarehouseId = ObtenerValorComboNullable(ComboBox_PrimaryWarehouseId);
                _ubicacionSeleccionada.ModifiedBy = UserData?.UserId ?? 1;

                int resultado = Ctrl_Locations.ActualizarUbicacion(_ubicacionSeleccionada);

                if (resultado > 0)
                {
                    MessageBox.Show(
                        "Sede actualizada exitosamente.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LimpiarFormularioLocations();
                    RefrescarListado();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo actualizar la sede.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al actualizar la sede: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            if (!Btn_Inactive.Enabled) return;
            try
            {
                if (_ubicacionSeleccionada == null || _ubicacionSeleccionada.LocationId == 0)
                {
                    MessageBox.Show(
                        "Debe seleccionar una sede para inactivar.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
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
                    MessageBox.Show(
                        "Sede inactivada exitosamente.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LimpiarFormularioLocations();
                    RefrescarListado();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo inactivar la sede.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al inactivar la sede: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            if (!Btn_Clear.Enabled) return;

            LimpiarFormularioLocations();
        }

        #endregion

        #region ValidacionesYHelpers

        private int? ObtenerValorComboNullable(ComboBox combo)
        {
            if (combo.SelectedValue == null)
                return null;

            int valor;
            if (int.TryParse(combo.SelectedValue.ToString(), out valor) && valor > 0)
                return valor;

            return null;
        }

        private int ObtenerValorCombo(ComboBox combo)
        {
            if (combo.SelectedValue == null)
                return 0;

            int valor;
            return int.TryParse(combo.SelectedValue.ToString(), out valor) ? valor : 0;
        }

        private bool ValidarCamposUbicacion()
        {
            if (TienePlaceholder(Txt_Code) || string.IsNullOrWhiteSpace(Txt_Code.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el código de la sede.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                Txt_Code.Focus();
                return false;
            }

            if (TienePlaceholder(Txt_Name) || string.IsNullOrWhiteSpace(Txt_Name.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el nombre de la sede.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                Txt_Name.Focus();
                return false;
            }

            if (TienePlaceholder(Txt_Address) || string.IsNullOrWhiteSpace(Txt_Address.Text))
            {
                MessageBox.Show(
                    "Debe ingresar la dirección de la sede.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                Txt_Address.Focus();
                return false;
            }

            if (ObtenerValorComboNullable(ComboBox_Country) == null)
            {
                MessageBox.Show(
                    "Debe seleccionar un país.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                ComboBox_Country.Focus();
                return false;
            }

            if (ObtenerValorComboNullable(ComboBox_Department) == null)
            {
                MessageBox.Show(
                    "Debe seleccionar un departamento.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                ComboBox_Department.Focus();
                return false;
            }

            if (ObtenerValorComboNullable(ComboBox_Municipality) == null)
            {
                MessageBox.Show(
                    "Debe seleccionar un municipio.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                ComboBox_Municipality.Focus();
                return false;
            }

            if (ObtenerValorComboNullable(ComboBox_LocationCategoryId) == null)
            {
                MessageBox.Show(
                    "Debe seleccionar la categoría de la sede.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
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

                CargarBodegasPrimarias(null);
            }
            finally
            {
                _cargandoUbicacion = false;
            }

            LimpiarSeleccionTabla();
        }

        private void AsignarTextoReal(TextBox txt, string valor)
        {
            txt.Text = valor ?? "";
            txt.ForeColor = Color.Black;
        }

        private void LimpiarSeleccionTabla()
        {
            if (Tabla.Rows.Count > 0)
                Tabla.ClearSelection();
        }

        #endregion

        #region Paginacion

        private void CrearToolStripPaginacion()
        {
            if (toolStripPaginacion != null)
                return;

            toolStripPaginacion = new ToolStrip
            {
                Dock = DockStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                GripStyle = ToolStripGripStyle.Hidden,
                BackColor = Color.FromArgb(248, 249, 250),
                Height = 35,
                AutoSize = true,
                Location = new Point(PanelToolStrip.Width - 260, 5)
            };

            btnAnterior = new ToolStripButton
            {
                Text = "❮ Anterior",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(51, 140, 255),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Margin = new Padding(2),
                Padding = new Padding(8, 4, 8, 4)
            };
            btnAnterior.Click += (s, e) => CambiarPagina(paginaActual - 1);

            btnSiguiente = new ToolStripButton
            {
                Text = "Siguiente ❯",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(238, 143, 109),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Margin = new Padding(2),
                Padding = new Padding(8, 4, 8, 4)
            };
            btnSiguiente.Click += (s, e) => CambiarPagina(paginaActual + 1);

            toolStripPaginacion.Items.Add(btnAnterior);
            toolStripPaginacion.Items.Add(btnSiguiente);

            PanelToolStrip.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.BringToFront();

            PanelToolStrip.Resize += (s, e) =>
            {
                if (toolStripPaginacion != null)
                    toolStripPaginacion.Location = new Point(PanelToolStrip.Width - 260, 5);
            };
        }

        private void ActualizarBotonesNumerados()
        {
            if (toolStripPaginacion == null)
                return;

            var itemsToRemove = toolStripPaginacion.Items
                .Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton")
                .ToList();

            foreach (var item in itemsToRemove)
                toolStripPaginacion.Items.Remove(item);

            if (totalPaginas <= 1)
                return;

            int inicioRango = Math.Max(1, paginaActual - 1);
            int finRango = Math.Min(totalPaginas, paginaActual + 1);
            int posicionInsertar = toolStripPaginacion.Items.IndexOf(btnSiguiente);

            for (int i = inicioRango; i <= finRango; i++)
            {
                ToolStripButton btnPagina = new ToolStripButton
                {
                    Text = i.ToString(),
                    Tag = "PageButton",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Margin = new Padding(1),
                    Padding = new Padding(6, 4, 6, 4)
                };

                if (i == paginaActual)
                {
                    btnPagina.BackColor = Color.FromArgb(238, 143, 109);
                    btnPagina.ForeColor = Color.White;
                }
                else
                {
                    btnPagina.BackColor = Color.FromArgb(240, 240, 240);
                    btnPagina.ForeColor = Color.FromArgb(51, 140, 255);
                }

                int numeroPagina = i;
                btnPagina.Click += (s, e) => CambiarPagina(numeroPagina);

                toolStripPaginacion.Items.Insert(posicionInsertar++, btnPagina);
            }
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1 || nuevaPagina > totalPaginas)
                return;

            paginaActual = nuevaPagina;

            if (!string.IsNullOrWhiteSpace(_ultimoTextoBusqueda) ||
                _ultimoTipoFiltro != "TODOS" ||
                _ultimoIsActive.HasValue)
            {
                locationsList = Ctrl_Locations.BuscarUbicaciones(
                    textoBusqueda: _ultimoTextoBusqueda,
                    tipoFiltro: _ultimoTipoFiltro,
                    isActive: _ultimoIsActive,
                    pageNumber: paginaActual,
                    pageSize: registrosPorPagina
                );

                Tabla.DataSource = null;
                Tabla.DataSource = locationsList;
                ConfigurarTabla();

                totalRegistros = Ctrl_Locations.ContarTotalUbicaciones(
                    textoBusqueda: _ultimoTextoBusqueda,
                    tipoFiltro: _ultimoTipoFiltro,
                    isActive: _ultimoIsActive
                );
            }
            else
            {
                RefrescarListado();
            }

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
            ActualizarInfoPaginacion();
            LimpiarSeleccionTabla();
        }

        private void ActualizarInfoPaginacion()
        {
            if (totalRegistros == 0 &&
                string.IsNullOrWhiteSpace(_ultimoTextoBusqueda) &&
                _ultimoTipoFiltro == "TODOS" &&
                !_ultimoIsActive.HasValue)
            {
                totalRegistros = Ctrl_Locations.ContarTotalUbicaciones();
            }

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            if (btnAnterior != null)
                btnAnterior.Enabled = paginaActual > 1;

            if (btnSiguiente != null)
                btnSiguiente.Enabled = paginaActual < totalPaginas;

            ActualizarBotonesNumerados();

            int inicioRango = totalRegistros == 0 ? 0 : ((paginaActual - 1) * registrosPorPagina) + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            if (totalRegistros == 0)
                Lbl_Paginas.Text = "NO HAY SEDES PARA MOSTRAR";
            else
                Lbl_Paginas.Text = $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} SEDES";
        }

        #endregion

        #region SistemaDePermisos

        private Ctrl_Security_Auth authController;
        private HashSet<string> permisosUsuario = new HashSet<string>();

        protected virtual async Task CargarPermisosUsuario(int userId, int roleId)
        {
            try
            {
                authController = new Ctrl_Security_Auth();
                var permisos = await authController.ObtenerPermisosUsuarioAsync(userId, roleId);
                permisosUsuario = permisos != null
                    ? new HashSet<string>(permisos, StringComparer.OrdinalIgnoreCase)
                    : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                permisosUsuario = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                MessageBox.Show($"ERROR AL CARGAR PERMISOS: {ex.Message}", "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected bool TienePermiso(string permissionCode)
        {
            return !string.IsNullOrWhiteSpace(permissionCode) &&
                   permisosUsuario != null &&
                   permisosUsuario.Contains(permissionCode);
        }

        protected void AplicarEstadoBotonPorPermiso(Button boton, string permissionCode)
        {
            if (boton == null) return;
            bool habilitado = TienePermiso(permissionCode);
            boton.Enabled = habilitado;
            if (habilitado)
            { boton.UseVisualStyleBackColor = true; boton.ForeColor = Color.Black; boton.Cursor = Cursors.Default; }
            else
            { boton.BackColor = Color.FromArgb(200, 200, 200); boton.ForeColor = Color.Gray; boton.Cursor = Cursors.No; }
        }

        protected void ConfigurarControlesPorPermisos()
        {
            AplicarEstadoBotonPorPermiso(Btn_Search, "LOCATIONS_MANAGMENT_READ");
            AplicarEstadoBotonPorPermiso(Btn_Save, "LOCATIONS_MANAGMENT_CREATE");
            AplicarEstadoBotonPorPermiso(Btn_Update, "LOCATIONS_MANAGMENT_UPDATE");
            AplicarEstadoBotonPorPermiso(Btn_Inactive, "LOCATIONS_MANAGMENT_INACTIVE");
            AplicarEstadoBotonPorPermiso(Btn_Export, "LOCATIONS_MANAGMENT_EXPORT");
            AplicarEstadoBotonPorPermiso(Btn_Import, "LOCATIONS_MANAGMENT_IMPORT");
        }

        #endregion SistemaDePermisos

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                var listaExportar = Ctrl_Locations.BuscarUbicaciones(
                    _ultimoTextoBusqueda, _ultimoTipoFiltro, _ultimoIsActive, 1, Math.Max(totalRegistros, 1));

                if (listaExportar == null || listaExportar.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar", "Información",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Lista de Sedes",
                    FileName = $"Sedes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    var excelApp = new Microsoft.Office.Interop.Excel.Application();
                    var workbook = excelApp.Workbooks.Add();
                    var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Sedes";

                    // ============ ENCABEZADO PRINCIPAL ============
                    worksheet.Cells[1, 1] = "REPORTE DE SEDES - SECRON";
                    worksheet.Range["A1:G1"].Merge();
                    worksheet.Range["A1:G1"].Font.Size = 16;
                    worksheet.Range["A1:G1"].Font.Bold = true;
                    worksheet.Range["A1:G1"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Range["A1:G1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(238, 143, 109));
                    worksheet.Range["A1:G1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                    // ============ INFORMACIÓN DEL REPORTE ============
                    worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SISTEMA"}";
                    worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {listaExportar.Count}";

                    worksheet.Range["A2:A4"].Font.Size = 10;
                    worksheet.Range["A2:A4"].Font.Bold = true;

                    // ============ ENCABEZADOS DE COLUMNAS ============
                    int headerRow = 6;
                    string[] headers = { "CÓDIGO", "NOMBRE", "DIRECCIÓN", "CIUDAD",
                        "PAÍS", "DEPARTAMENTO", "ESTADO" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[headerRow, i + 1] = headers[i];
                    }

                    var headerRange = worksheet.Range[$"A{headerRow}:G{headerRow}"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Size = 11;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    headerRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                    // ============ DATOS ============
                    int row = headerRow + 1;
                    foreach (var sede in listaExportar)
                    {
                        worksheet.Cells[row, 1] = sede.LocationCode ?? "";
                        worksheet.Cells[row, 2] = sede.LocationName ?? "";
                        worksheet.Cells[row, 3] = sede.Address ?? "";
                        worksheet.Cells[row, 4] = sede.City ?? "";
                        worksheet.Cells[row, 5] = sede.CountryName ?? "";
                        worksheet.Cells[row, 6] = sede.DepartmentName ?? "";
                        worksheet.Cells[row, 7] = sede.IsActive ? "ACTIVO" : "INACTIVO";

                        // Alternar color de filas
                        if (row % 2 == 0)
                        {
                            worksheet.Range[$"A{row}:G{row}"].Interior.Color =
                                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                        }

                        row++;
                    }

                    // ============ FORMATO FINAL ============
                    var dataRange = worksheet.Range[$"A{headerRow}:G{row - 1}"];
                    dataRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    dataRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;

                    worksheet.Columns.AutoFit();

                    worksheet.Activate();
                    excelApp.ActiveWindow.SplitRow = headerRow;
                    excelApp.ActiveWindow.FreezePanes = true;

                    // ============ PIE DE PÁGINA ============
                    worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].Merge();
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].Font.Italic = true;
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].Font.Size = 9;
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].HorizontalAlignment =
                        Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    workbook.SaveAs(saveFileDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                    this.Cursor = Cursors.Default;

                    var result = MessageBox.Show(
                        "Archivo exportado exitosamente.\n\n¿Desea abrir el archivo ahora?",
                        "Exportación Exitosa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}