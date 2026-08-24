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
    public partial class Frm_AcademicProcesses_CoursesPricing : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_CourseLocationPricingMaster> _preciosList;
        private Mdl_CourseLocationPricingMaster _precioSeleccionado = null;

        #endregion

        public Frm_AcademicProcesses_CoursesPricing()
        {
            InitializeComponent();
        }

        private async void Frm_AcademicProcesses_CoursesPricing_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarCombos();
                ConfigurarFiltros();
                ConfigurarTabla();
                CargarPrecios();

                if (UserData != null)
                {
                    await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
                    ConfigurarControlesPorPermisos();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("Error al cargar el formulario: " + ex.Message,
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
            AplicarEstadoBotonPorPermiso(Btn_Save, "ACADEMICPROCESSES_COURSESPRICING_CREATE");
            AplicarEstadoBotonPorPermiso(Btn_Update, "ACADEMICPROCESSES_COURSESPRICING_UPDATE");
            AplicarEstadoBotonPorPermiso(Btn_IsActive, "ACADEMICPROCESSES_COURSESPRICING_INACTIVE");
            AplicarEstadoBotonPorPermiso(Btn_Export, "ACADEMICPROCESSES_COURSESPRICING_EXPORT");
        }

        #endregion SistemaDePermisos

        #region Configuración de combos

        private void ConfigurarCombos()
        {
            var sedes = Ctrl_Locations.ObtenerLocationsActivas();
            Cbx_Sede.DisplayMember = "Value";
            Cbx_Sede.ValueMember = "Key";
            Cbx_Sede.DataSource = sedes;
            Cbx_Sede.SelectedIndex = -1;

            var carreras = Ctrl_Careers.ObtenerCarrerasActivas();
            Cbx_Carrera.DisplayMember = "Value";
            Cbx_Carrera.ValueMember = "Key";
            Cbx_Carrera.DataSource = carreras;
            Cbx_Carrera.SelectedIndex = -1;

            var modalidades = Ctrl_CourseModalities.ObtenerModalidades(true);
            Cbx_Modalidad.DisplayMember = "ModalityName";
            Cbx_Modalidad.ValueMember = "ModalityId";
            Cbx_Modalidad.DataSource = modalidades;
            Cbx_Modalidad.SelectedIndex = -1;

            Cbx_Carrera.SelectedIndexChanged += Cbx_Carrera_SelectedIndexChanged;
        }

        private void Cbx_Carrera_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cbx_Pensum.DataSource = null;

            if (!(Cbx_Carrera.SelectedValue is int careerId))
                return;

            var pensums = Ctrl_CareerPensums.MostrarPensums("", careerId, true); // solo IsActive = 1

            var itemsPensum = pensums
                .Select(p => new KeyValuePair<int, string>(
                    p.CareerPensumId,
                    $"{p.PensumCode} - {(p.IsCurrent ? "VIGENTE" : "NO VIGENTE")}"))
                .ToList();

            Cbx_Pensum.DisplayMember = "Value";
            Cbx_Pensum.ValueMember = "Key";
            Cbx_Pensum.DataSource = itemsPensum;
            Cbx_Pensum.SelectedIndex = -1;
        }

        private void ConfigurarFiltros()
        {
            var sedes = new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(0, "TODAS LAS SEDES") };
            sedes.AddRange(Ctrl_Locations.ObtenerLocationsActivas());
            FiltroSede.DisplayMember = "Value";
            FiltroSede.ValueMember = "Key";
            FiltroSede.DataSource = sedes;
            FiltroSede.SelectedIndex = 0;

            var carreras = new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(0, "TODAS LAS CARRERAS") };
            carreras.AddRange(Ctrl_Careers.ObtenerCarrerasActivas());
            FiltroCarrera.DisplayMember = "Value";
            FiltroCarrera.ValueMember = "Key";
            FiltroCarrera.DataSource = carreras;
            FiltroCarrera.SelectedIndex = 0;
        }

        #endregion

        #region ConfigurarTabla

        private void Tabla_CellFormatting_Estado(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (Tabla.Columns[e.ColumnIndex].Name != "ColEstadoRegistro") return;
            if (e.Value == null) return;

            bool activo = e.Value.ToString() == "ACTIVO";
            e.CellStyle.ForeColor = activo ? Color.FromArgb(0, 128, 0) : Color.FromArgb(200, 0, 0);
            e.CellStyle.Font = new Font(Tabla.Font, FontStyle.Bold);
        }

        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;

            var colEstadoRegistro = new DataGridViewTextBoxColumn();
            colEstadoRegistro.Name = "ColEstadoRegistro";
            colEstadoRegistro.HeaderText = "ESTADO";
            colEstadoRegistro.Width = 90;
            colEstadoRegistro.ReadOnly = true;
            colEstadoRegistro.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.Columns.Add(colEstadoRegistro);

            Tabla.Columns.Add("CourseLocationPricingId", "ID");
            Tabla.Columns.Add("CourseCode", "CÓDIGO CURSO");
            Tabla.Columns.Add("CourseName", "CURSO");
            Tabla.Columns.Add("CareerName", "CARRERA");
            Tabla.Columns.Add("LocationName", "SEDE");
            Tabla.Columns.Add("ModalityName", "MODALIDAD");
            Tabla.Columns.Add("CurrentPrice", "PRECIO VIGENTE");
            Tabla.Columns.Add("CurrentPriceEffectiveFrom", "VIGENTE DESDE");

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.RowHeadersVisible = false;

            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            Tabla.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            Tabla.RowTemplate.Height = 30;
            Tabla.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            Tabla.Columns["CourseLocationPricingId"].Visible = false;

            Tabla.Columns["CourseName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CourseName"].FillWeight = 20;
            Tabla.Columns["CourseCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CourseCode"].FillWeight = 12;
            Tabla.Columns["CareerName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CareerName"].FillWeight = 18;
            Tabla.Columns["LocationName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["LocationName"].FillWeight = 15;
            Tabla.Columns["ModalityName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["ModalityName"].FillWeight = 13;
            Tabla.Columns["CurrentPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CurrentPrice"].FillWeight = 12;
            Tabla.Columns["CurrentPriceEffectiveFrom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["CurrentPriceEffectiveFrom"].FillWeight = 15;

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;

            Tabla.CellFormatting -= Tabla_CellFormatting_Estado;
            Tabla.CellFormatting += Tabla_CellFormatting_Estado;

            ConfigurarTablaAtributos();
        }

        // Grid de historial de precios: oculta IDs y usa nombres de columna legibles
        private void ConfigurarTablaAtributos()
        {
            TablaAtributos.AutoGenerateColumns = false;
            TablaAtributos.Columns.Clear();

            var colEstado = new DataGridViewTextBoxColumn
            {
                Name = "ColEstadoAtributo",
                HeaderText = "ESTADO",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 15
            };
            TablaAtributos.Columns.Add(colEstado);

            TablaAtributos.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "PRECIO", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 15 });
            TablaAtributos.Columns.Add(new DataGridViewTextBoxColumn { Name = "EffectiveFrom", HeaderText = "VIGENTE DESDE", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 20 });
            TablaAtributos.Columns.Add(new DataGridViewTextBoxColumn { Name = "EffectiveTo", HeaderText = "VIGENTE HASTA", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 20 });
            TablaAtributos.Columns.Add(new DataGridViewTextBoxColumn { Name = "CreatedDate", HeaderText = "FECHA DE REGISTRO", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 30 });

            TablaAtributos.ReadOnly = true;
            TablaAtributos.AllowUserToAddRows = false;
            TablaAtributos.AllowUserToResizeRows = false;
            TablaAtributos.RowHeadersVisible = false;
            TablaAtributos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TablaAtributos.MultiSelect = false;

            TablaAtributos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            TablaAtributos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            TablaAtributos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            TablaAtributos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            TablaAtributos.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            TablaAtributos.CellFormatting -= TablaAtributos_CellFormatting_Estado;
            TablaAtributos.CellFormatting += TablaAtributos_CellFormatting_Estado;
        }

        // Colorea la columna ESTADO igual que el grid principal: verde ACTIVO, rojo INACTIVO
        private void TablaAtributos_CellFormatting_Estado(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (TablaAtributos.Columns[e.ColumnIndex].Name != "ColEstadoAtributo") return;
            if (e.Value == null) return;

            bool activo = e.Value.ToString() == "ACTIVO";
            e.CellStyle.ForeColor = activo ? Color.FromArgb(0, 128, 0) : Color.FromArgb(200, 0, 0);
            e.CellStyle.Font = new Font(TablaAtributos.Font, FontStyle.Bold);
        }

        // Llena el grid de historial con filas manuales (mismo patrón que el grid principal)
        private void MostrarHistorialEnTabla(List<Mdl_CourseLocationPricingDetail> historial)
        {
            TablaAtributos.Rows.Clear();

            foreach (var detalle in historial)
            {
                TablaAtributos.Rows.Add(
                    detalle.IsActive ? "ACTIVO" : "INACTIVO",
                    detalle.Price.ToString("Q 0.00"),
                    detalle.EffectiveFrom.ToString("dd/MM/yyyy HH:mm"),
                    detalle.EffectiveTo.HasValue ? detalle.EffectiveTo.Value.ToString("dd/MM/yyyy HH:mm") : "VIGENTE",
                    detalle.CreatedDate.ToString("dd/MM/yyyy HH:mm")
                );
            }
        }

        private void MostrarPreciosEnTabla(List<Mdl_CourseLocationPricingMaster> lista)
        {
            Tabla.Rows.Clear();

            foreach (var precio in lista)
            {
                Tabla.Rows.Add(
                    precio.IsActive ? "ACTIVO" : "INACTIVO",
                    precio.CourseLocationPricingId,
                    precio.CourseCode,
                    precio.CourseName,
                    precio.CareerName,
                    precio.LocationName,
                    precio.ModalityName,
                    precio.CurrentPrice.HasValue ? precio.CurrentPrice.Value.ToString("Q 0.00") : "SIN PRECIO",
                    precio.CurrentPriceEffectiveFrom.HasValue ? precio.CurrentPriceEffectiveFrom.Value.ToString("dd/MM/yyyy") : "N/A"
                );
            }
        }

        #endregion

        #region Grilla principal (precio vigente)

        private void CargarPrecios()
        {
            int? locationId = (FiltroSede.SelectedValue is int lid && lid > 0) ? lid : (int?)null;
            int? careerId = (FiltroCarrera.SelectedValue is int cid && cid > 0) ? cid : (int?)null;

            _preciosList = Ctrl_CourseLocationPricingMaster.MostrarPrecios(locationId, careerId, false);

            string texto = Txt_ValorBuscado.Text.Trim().ToUpper();
            var filtrados = string.IsNullOrEmpty(texto)
                ? _preciosList
                : _preciosList.Where(p =>
                    (p.CourseName ?? "").ToUpper().Contains(texto) ||
                    (p.CareerName ?? "").ToUpper().Contains(texto) ||
                    (p.LocationName ?? "").ToUpper().Contains(texto)
                  ).ToList();

            MostrarPreciosEnTabla(filtrados);

            if (Tabla.Rows.Count > 0)
            {
                Tabla.Rows[0].Selected = true;
                Tabla_SelectionChanged(null, null);
            }
            else
            {
                TablaAtributos.Rows.Clear();
                _precioSeleccionado = null;
            }
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            CargarPrecios();
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "";
            FiltroSede.SelectedIndex = 0;
            FiltroCarrera.SelectedIndex = 0;
            CargarPrecios();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 0) return;

            int courseLocationPricingId = Convert.ToInt32(Tabla.SelectedRows[0].Cells["CourseLocationPricingId"].Value);
            _precioSeleccionado = _preciosList.FirstOrDefault(p => p.CourseLocationPricingId == courseLocationPricingId);

            if (_precioSeleccionado == null) return;

            // Actualizar panel izquierdo con los datos de la fila seleccionada
            SeleccionarPorValueMember(Cbx_Sede, _precioSeleccionado.LocationId);
            SeleccionarPorValueMember(Cbx_Modalidad, _precioSeleccionado.ModalityId);

            var carreras = Cbx_Carrera.DataSource as List<KeyValuePair<int, string>>;
            var carreraCoincidente = carreras?.FirstOrDefault(c => c.Value == _precioSeleccionado.CareerName);
            if (carreraCoincidente.HasValue && !carreraCoincidente.Value.Equals(default(KeyValuePair<int, string>)))
                Cbx_Carrera.SelectedValue = carreraCoincidente.Value.Key;
            // Al asignar Cbx_Carrera arriba, se dispara Cbx_Carrera_SelectedIndexChanged,
            // que ya recargó Cbx_Pensum con los pensums de esa carrera. Ahora seleccionamos
            // el pensum específico al que pertenece el curso de la fila elegida.
            SeleccionarPorValueMember(Cbx_Pensum, _precioSeleccionado.CareerPensumId);

            var historial = Ctrl_CourseLocationPricingDetail.MostrarHistorial(_precioSeleccionado.CourseLocationPricingId);
            MostrarHistorialEnTabla(historial.OrderByDescending(h => h.EffectiveFrom).ToList());
        }

        // ComboBox.SelectedValue no genera error si el valor no existe en la lista enlazada:
        // simplemente no cambia. Este método hace explícita esa validación.
        // Selecciona un ítem del combo comparando contra su ValueMember, usando reflexión.
        // Funciona sin importar el tipo real de la lista enlazada (KeyValuePair, Mdl_X, etc.),
        // eliminando el riesgo de castear al tipo equivocado.
        private void SeleccionarPorValueMember(ComboBox combo, int valor)
        {
            if (combo.DataSource == null || string.IsNullOrEmpty(combo.ValueMember))
            {
                combo.SelectedIndex = -1;
                return;
            }

            for (int i = 0; i < combo.Items.Count; i++)
            {
                object item = combo.Items[i];
                var propiedad = item.GetType().GetProperty(combo.ValueMember);
                if (propiedad == null) continue;

                object valorItem = propiedad.GetValue(item);
                if (valorItem != null && Convert.ToInt32(valorItem) == valor)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            combo.SelectedIndex = -1; // no se encontró (probablemente inactivo o filtrado)
        }

        #endregion

        #region Filtros del historial (pendientes, sin lógica funcional aún)

        private void Btn_SearchAtributo_Click(object sender, EventArgs e)
        {
            // Pendiente: definir filtros del historial
        }

        private void Txt_BuscarAtributo_KeyDown(object sender, KeyEventArgs e)
        {
            // Pendiente: definir filtros del historial
        }

        private void Btn_CleanSearchAtributo_Click(object sender, EventArgs e)
        {
            // Pendiente: definir filtros del historial
        }

        #endregion

        #region Asignar carrera a sede (Btn_Save)

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!ValidarSedeCarreraModalidad(out int locationId, out int careerId, out int modalityId))
                return;

            if (!(Cbx_Pensum.SelectedValue is int careerPensumId))
            {
                MessageBox.Show("Seleccione un pensum. No se puede asignar la carrera sin un pensum.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar que no exista ya esta combinación exacta
            if (Ctrl_LocationCareers.ExisteAsignacionActiva(locationId, careerId, modalityId))
            {
                MessageBox.Show("Ya existe esta asignación de carrera para esta sede y modalidad.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // No se guarda nada todavía. La asignación y TODOS los precios de los cursos
            // del pensum se guardan juntos, en una sola transacción, hasta que el usuario
            // presione "GUARDAR" dentro del editor de precios. Si cancela, no queda nada.
            AbrirEdicionPreciosPensum(careerPensumId, careerId, locationId, modalityId);

            Btn_Clear_Click(null, null);
            CargarPrecios();
        }

        #endregion

        #region Actualizar / Limpiar

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (!ValidarSedeCarreraModalidad(out int locationId, out int careerId, out int modalityId))
                return;

            if (!(Cbx_Pensum.SelectedValue is int careerPensumId))
            {
                MessageBox.Show("Seleccione un pensum.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                "¿Desea modificar algún precio de los cursos del pensum?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                AbrirEdicionPreciosPensum(careerPensumId, careerId, locationId, modalityId);
            }
            else
            {
                // Solo se editan los valores del panel izquierdo (sin tocar cursos/precios)
                MessageBox.Show("No se realizaron cambios de precio. Actualice los valores de sede/carrera/modalidad manualmente si corresponde.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            CargarPrecios();
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Cbx_Sede.SelectedIndex = -1;
            Cbx_Carrera.SelectedIndex = -1;
            Cbx_Modalidad.SelectedIndex = -1;
            Cbx_Pensum.DataSource = null;
        }

        #endregion

        #region CambiarEstado

        private void Btn_IsActive_Click(object sender, EventArgs e)
        {
            if (_precioSeleccionado == null)
            {
                MessageBox.Show("SELECCIONE UNA COMBINACIÓN DE LA TABLA.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool estaActivo = _precioSeleccionado.IsActive;
            bool nuevoEstado = !estaActivo;
            string accion = estaActivo ? "INACTIVAR" : "ACTIVAR";

            var confirmacion = MessageBox.Show(
                $"¿DESEA {accion} EL PRECIO DE '{_precioSeleccionado.CourseName}' EN '{_precioSeleccionado.LocationName}'?",
                "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            int resultado = Ctrl_CourseLocationPricingMaster.CambiarEstadoPrecioMaster(
                _precioSeleccionado.CourseLocationPricingId, nuevoEstado, UserData?.UserId ?? 0);

            if (resultado > 0)
            {
                MessageBox.Show($"COMBINACIÓN {accion}DA CORRECTAMENTE.", "ÉXITO",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                Btn_Clear_Click(null, null);
                CargarPrecios();
            }
            else
            {
                MessageBox.Show("NO SE PUDO CAMBIAR EL ESTADO DE LA COMBINACIÓN.", "ERROR",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Validación

        private bool ValidarSedeCarreraModalidad(out int locationId, out int careerId, out int modalityId)
        {
            locationId = careerId = modalityId = 0;

            if (Cbx_Sede.SelectedValue == null || !(Cbx_Sede.SelectedValue is int))
            {
                MessageBox.Show("Seleccione una sede.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (Cbx_Carrera.SelectedValue == null || !(Cbx_Carrera.SelectedValue is int))
            {
                MessageBox.Show("Seleccione una carrera.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (Cbx_Modalidad.SelectedValue == null || !(Cbx_Modalidad.SelectedValue is int))
            {
                MessageBox.Show("Seleccione una modalidad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            locationId = (int)Cbx_Sede.SelectedValue;
            careerId = (int)Cbx_Carrera.SelectedValue;
            modalityId = (int)Cbx_Modalidad.SelectedValue;

            return true;
        }

        #endregion

        #region Formulario de edición de precios del pensum

        // Abre el formulario de edición de precios de los cursos del pensum para la Sede+Modalidad indicada
        private void AbrirEdicionPreciosPensum(int careerPensumId, int careerId, int locationId, int modalityId)
        {
            using (var frm = new Frm_AcademicProcesses_SearchCourses(careerPensumId, careerId, locationId, modalityId))
            {
                frm.UserData = this.UserData;
                frm.ShowDialog(this);
            }
        }

        #endregion
    }
}