using SECRON.Configuration;
using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_WarehousePermissions : Form
    {
        public Mdl_Security_UserInfo UserData { get; set; }
        public HashSet<string> PermisosUsuario { get; set; }

        private TabControl tabControl;
        private HashSet<string> _permisosUsuarioActual = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private bool _esAdminGlobal = false;
        private List<int> _bodegasPermitidas = new List<int>();

        private List<Mdl_Warehouse> _catalogoBodegas;
        private List<Mdl_WarehousePermission> _catalogoPermisos;

        private int _usuarioSeleccionado1Id = 0;
        private bool _usuarioSeleccionado2EsSuperAdmin = false;

        private int _usuarioSeleccionado2Id = 0;
        private int? _warehouseManagerIdActual = null;
        private Dictionary<string, Mdl_WarehouseManagerPermission> _permisosAsignadosActual = new Dictionary<string, Mdl_WarehouseManagerPermission>(StringComparer.OrdinalIgnoreCase);

        public Frm_KARDEX_WarehousePermissions()
        {
            InitializeComponent();
        }

        private void Frm_KARDEX_WarehousePermissions_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                CargarAlcanceDeAdministracion();

                if (_bodegasPermitidas.Count == 0)
                {
                    MessageBox.Show("NO TIENE PERMISOS PARA ADMINISTRAR NINGUNA BODEGA.", "Sin acceso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    this.Close();
                    return;
                }

                _catalogoBodegas = Ctrl_Warehouses.MostrarBodegas()
                    .Where(w => _esAdminGlobal || _bodegasPermitidas.Contains(w.WarehouseId))
                    .ToList();

                _catalogoPermisos = Ctrl_WarehouseManagerPermissions.ObtenerCatalogoPermisos();

                ConfigurarTabla_Usuarios();
                ConfigurarTabla_BodegasDisponibles();
                ConfigurarTabla_BodegasAsignadas();
                ConfigurarTabla_Usuarios2();

                CargarUsuariosEnTabla1();
                AsignarResultadosBodegasDisponibles(_catalogoBodegas);

                CargarUsuariosEnTabla2();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region AlcanceDeAdministracion

        private void CargarAlcanceDeAdministracion()
        {
            if (UserData == null) return;

            _permisosUsuarioActual = PermisosUsuario ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _esAdminGlobal = _permisosUsuarioActual.Contains("KARDEX_WAREHOUSE_ADMIN");

            if (_esAdminGlobal)
            {
                _bodegasPermitidas = Ctrl_Warehouses.MostrarBodegas().Select(w => w.WarehouseId).ToList();
            }
            else
            {
                _bodegasPermitidas = Ctrl_WarehouseManagerPermissions.ObtenerBodegasComoAdminBodega(UserData.UserId);
            }
        }

        #endregion AlcanceDeAdministracion
        #region Pestana1_ConfigurarTablas

        private void ConfigurarTabla_Usuarios()
        {
            Tabla_Usuarios.Columns.Clear();
            Tabla_Usuarios.Columns.Add("UserId", "ID");
            Tabla_Usuarios.Columns.Add("Username", "USUARIO");
            Tabla_Usuarios.Columns.Add("FullName", "NOMBRE COMPLETO");
            Tabla_Usuarios.Columns["UserId"].Visible = false;
            Tabla_Usuarios.Columns["Username"].Width = 140;
            Tabla_Usuarios.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla_Usuarios.Columns["FullName"].MinimumWidth = 200;

            AplicarEstiloTabla(Tabla_Usuarios);
            Tabla_Usuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla_Usuarios.MultiSelect = false;
            Tabla_Usuarios.ReadOnly = true;
            Tabla_Usuarios.AllowUserToAddRows = false;
        }

        private void ConfigurarTabla_BodegasDisponibles()
        {
            Tabla_BodegasDisponibles.Columns.Clear();
            Tabla_BodegasDisponibles.Columns.Add("WarehouseId", "ID");
            Tabla_BodegasDisponibles.Columns.Add("WarehouseCode", "CÓDIGO");
            Tabla_BodegasDisponibles.Columns.Add("WarehouseName", "BODEGA");
            Tabla_BodegasDisponibles.Columns["WarehouseId"].Visible = false;
            Tabla_BodegasDisponibles.Columns["WarehouseName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            AplicarEstiloTabla(Tabla_BodegasDisponibles);
            Tabla_BodegasDisponibles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla_BodegasDisponibles.MultiSelect = false;
            Tabla_BodegasDisponibles.ReadOnly = true;
            Tabla_BodegasDisponibles.AllowUserToAddRows = false;
        }

        private void ConfigurarTabla_BodegasAsignadas()
        {
            Tabla_BodegasAsignadas.AllowUserToAddRows = false;
            Tabla_BodegasAsignadas.RowHeadersVisible = false;
            Tabla_BodegasAsignadas.CellPainting += Tabla_BodegasAsignadas_CellPainting;
            AplicarEstiloTabla(Tabla_BodegasAsignadas);
        }

        private void AplicarEstiloTabla(DataGridView tabla)
        {
            tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tabla.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            tabla.DefaultCellStyle.SelectionForeColor = Color.White;
            tabla.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            tabla.RowHeadersVisible = false;
            tabla.AllowUserToResizeRows = false;
            tabla.RowTemplate.Height = 34;
            tabla.ColumnHeadersHeight = 38;
        }

        #endregion Pestana1_ConfigurarTablas
        #region Pestana1_CargaUsuarios

        private void CargarUsuariosEnTabla1(string filtro = "")
        {
            try
            {
                Tabla_Usuarios.SelectionChanged -= Tabla_Usuarios_SelectionChanged;

                Tabla_Usuarios.Rows.Clear();

                List<Mdl_Users> usuarios = string.IsNullOrWhiteSpace(filtro)
                    ? Ctrl_Users.MostrarUsuarios(1, 500)
                    : Ctrl_Users.BuscarUsuarios(filtro, null, null, null, 1, 500);

                foreach (var usuario in usuarios)
                    Tabla_Usuarios.Rows.Add(usuario.UserId, usuario.Username, usuario.FullName);

                Tabla_Usuarios.ClearSelection();
                _usuarioSeleccionado1Id = 0;
                Tabla_BodegasAsignadas.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR USUARIOS: {ex.Message}", "Error SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Tabla_Usuarios.SelectionChanged += Tabla_Usuarios_SelectionChanged;
            }
        }

        private void Btn_SearchUsuario_Click(object sender, EventArgs e)
        {
            CargarUsuariosEnTabla1(Txt_BuscarUsuario.Text.Trim());
        }

        private void Btn_ClearUsuario_Click(object sender, EventArgs e)
        {
            Txt_BuscarUsuario.Clear();
            CargarUsuariosEnTabla1();
        }

        private void Txt_BuscarUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_SearchUsuario_Click(sender, e); }
        }

        private void Tabla_Usuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla_Usuarios.SelectedRows.Count == 0)
            {
                _usuarioSeleccionado1Id = 0;
                Tabla_BodegasAsignadas.DataSource = null;
                return;
            }

            _usuarioSeleccionado1Id = Convert.ToInt32(Tabla_Usuarios.SelectedRows[0].Cells["UserId"].Value);
            CargarBodegasAsignadas();
        }

        #endregion Pestana1_CargaUsuarios
        #region Pestana1_BodegasDisponibles

        private void AsignarResultadosBodegasDisponibles(List<Mdl_Warehouse> lista)
        {
            Tabla_BodegasDisponibles.Rows.Clear();
            foreach (var w in lista)
                Tabla_BodegasDisponibles.Rows.Add(w.WarehouseId, w.WarehouseCode, w.WarehouseName);
        }

        #endregion Pestana1_BodegasDisponibles
        #region Pestana1_BodegasAsignadas

        private void CargarBodegasAsignadas()
        {
            try
            {
                var asignadas = Ctrl_WarehouseManager.ObtenerBodegasPorUsuario(_usuarioSeleccionado1Id);

                var data = asignadas.Select(a => new
                {
                    a.WarehouseManagerId,
                    a.WarehouseId,
                    a.WarehouseName
                }).ToList();

                Tabla_BodegasAsignadas.DataSource = data;

                if (Tabla_BodegasAsignadas.Columns.Contains("WarehouseManagerId"))
                    Tabla_BodegasAsignadas.Columns["WarehouseManagerId"].Visible = false;
                if (Tabla_BodegasAsignadas.Columns.Contains("WarehouseId"))
                    Tabla_BodegasAsignadas.Columns["WarehouseId"].Visible = false;
                if (Tabla_BodegasAsignadas.Columns.Contains("WarehouseName"))
                {
                    Tabla_BodegasAsignadas.Columns["WarehouseName"].HeaderText = "BODEGA ASIGNADA";
                    Tabla_BodegasAsignadas.Columns["WarehouseName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                if (Tabla_BodegasAsignadas.Columns.Contains("Col_QuitarBodega"))
                    Tabla_BodegasAsignadas.Columns["Col_QuitarBodega"].DisplayIndex = Tabla_BodegasAsignadas.Columns.Count - 1;

                // Botón gris y bloqueado en bodegas fuera del alcance del admin actual
                foreach (DataGridViewRow row in Tabla_BodegasAsignadas.Rows)
                {
                    if (row.Cells["WarehouseId"].Value == null) continue;
                    int warehouseId = Convert.ToInt32(row.Cells["WarehouseId"].Value);
                    bool dentroDeAlcance = _esAdminGlobal || _bodegasPermitidas.Contains(warehouseId);

                    row.Cells["Col_QuitarBodega"].ReadOnly = !dentroDeAlcance;
                    row.Cells["Col_QuitarBodega"].Style.BackColor = dentroDeAlcance
                        ? Color.White : Color.FromArgb(220, 220, 220);
                    row.Cells["Col_QuitarBodega"].Style.ForeColor = dentroDeAlcance
                        ? Color.Black : Color.FromArgb(150, 150, 150);
                    row.Cells["Col_QuitarBodega"].Style.SelectionBackColor = dentroDeAlcance
                        ? row.Cells["Col_QuitarBodega"].Style.SelectionBackColor : Color.FromArgb(220, 220, 220);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR BODEGAS ASIGNADAS: {ex.Message}", "Error SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_AsignarBodega_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioSeleccionado1Id == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN USUARIO.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Tabla_BodegasDisponibles.SelectedRows.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA BODEGA DE LA LISTA DE DISPONIBLES.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int warehouseId = Convert.ToInt32(Tabla_BodegasDisponibles.SelectedRows[0].Cells["WarehouseId"].Value);
                string warehouseName = Tabla_BodegasDisponibles.SelectedRows[0].Cells["WarehouseName"].Value.ToString();

                int resultado = Ctrl_WarehouseManager.AsignarBodega(warehouseId, _usuarioSeleccionado1Id, UserData.UserId);

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show($"BODEGA \"{warehouseName.ToUpper()}\" ASIGNADA CORRECTAMENTE.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarBodegasAsignadas();
                        break;
                    case -1:
                        MessageBox.Show("ESTA BODEGA YA TIENE UN ENCARGADO ACTIVO.", "Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("NO SE PUDO ASIGNAR LA BODEGA.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ASIGNAR BODEGA: {ex.Message}", "Error SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Tabla_BodegasAsignadas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (Tabla_BodegasAsignadas.Columns[e.ColumnIndex].Name != "Col_QuitarBodega") return;

            var fila = Tabla_BodegasAsignadas.Rows[e.RowIndex];
            if (fila.Cells["WarehouseManagerId"].Value == null) return;
            if (fila.Cells["WarehouseId"].Value == null) return;

            int warehouseId = Convert.ToInt32(fila.Cells["WarehouseId"].Value);
            bool dentroDeAlcance = _esAdminGlobal || _bodegasPermitidas.Contains(warehouseId);

            // Doble defensa: aunque el ReadOnly debería bloquear el click, validamos explícitamente
            if (!dentroDeAlcance)
            {
                MessageBox.Show("NO TIENE PERMISOS PARA QUITAR ESTA BODEGA. SOLO PUEDE GESTIONAR LAS BODEGAS QUE ADMINISTRA.",
                    "Sin permiso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int warehouseManagerId = Convert.ToInt32(fila.Cells["WarehouseManagerId"].Value);

            var confirmacion = MessageBox.Show("¿DESEA QUITAR ESTA BODEGA AL USUARIO?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            int resultado = Ctrl_WarehouseManager.QuitarBodega(warehouseManagerId, UserData.UserId);
            if (resultado > 0)
            {
                MessageBox.Show("BODEGA QUITADA CORRECTAMENTE.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarBodegasAsignadas();
            }
            else
            {
                MessageBox.Show("NO SE PUDO QUITAR LA BODEGA.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Tabla_BodegasAsignadas_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (Tabla_BodegasAsignadas.Columns[e.ColumnIndex].Name != "Col_QuitarBodega") return;

            var fila = Tabla_BodegasAsignadas.Rows[e.RowIndex];
            if (fila.Cells["WarehouseId"].Value == null) return;

            int warehouseId = Convert.ToInt32(fila.Cells["WarehouseId"].Value);
            bool dentroDeAlcance = _esAdminGlobal || _bodegasPermitidas.Contains(warehouseId);

            if (dentroDeAlcance) return; // deja el dibujo normal del botón

            e.PaintBackground(e.CellBounds, true);

            Rectangle botonRect = Rectangle.Inflate(e.CellBounds, -4, -4);
            using (var fondoGris = new SolidBrush(Color.FromArgb(225, 225, 225)))
                e.Graphics.FillRectangle(fondoGris, botonRect);

            using (var bordeGris = new Pen(Color.FromArgb(190, 190, 190)))
                e.Graphics.DrawRectangle(bordeGris, botonRect);

            TextRenderer.DrawText(e.Graphics, "QUITAR", e.CellStyle.Font, botonRect,
                Color.FromArgb(160, 160, 160), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            e.Handled = true;
        }

        #endregion Pestana1_BodegasAsignadas
        #region Pestana2_ConfigurarTablas

        private void ConfigurarTabla_Usuarios2()
        {
            Tabla_Usuarios2.Columns.Clear();
            Tabla_Usuarios2.Columns.Add("UserId", "ID");
            Tabla_Usuarios2.Columns.Add("Username", "USUARIO");
            Tabla_Usuarios2.Columns.Add("FullName", "NOMBRE COMPLETO");
            Tabla_Usuarios2.Columns["UserId"].Visible = false;
            Tabla_Usuarios2.Columns["Username"].Width = 140;
            Tabla_Usuarios2.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla_Usuarios2.Columns["FullName"].MinimumWidth = 200;

            AplicarEstiloTabla(Tabla_Usuarios2);
            Tabla_Usuarios2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla_Usuarios2.MultiSelect = false;
            Tabla_Usuarios2.ReadOnly = true;
            Tabla_Usuarios2.AllowUserToAddRows = false;
        }

        #endregion Pestana2_ConfigurarTablas
        #region Pestana2_CargaUsuarios

        private void CargarUsuariosEnTabla2(string filtro = "")
        {
            try
            {
                Tabla_Usuarios2.SelectionChanged -= Tabla_Usuarios2_SelectionChanged;

                Tabla_Usuarios2.Rows.Clear();

                var usuarios = Ctrl_WarehouseManager.ObtenerUsuariosConBodegaEnLista(_bodegasPermitidas);

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    string filtroUpper = filtro.Trim().ToUpper();
                    usuarios = usuarios.Where(u =>
                        (u.Username?.ToUpper().Contains(filtroUpper) ?? false) ||
                        (u.FullName?.ToUpper().Contains(filtroUpper) ?? false)).ToList();
                }

                foreach (var usuario in usuarios)
                    Tabla_Usuarios2.Rows.Add(usuario.UserId, usuario.Username, usuario.FullName);

                Tabla_Usuarios2.ClearSelection();
                _usuarioSeleccionado2Id = 0;
                ComboBox_BodegaUsuario.Items.Clear();
                LimpiarPanelPermisos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR USUARIOS: {ex.Message}", "Error SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Tabla_Usuarios2.SelectionChanged += Tabla_Usuarios2_SelectionChanged;
            }
        }

        private void Btn_SearchUsuario2_Click(object sender, EventArgs e)
        {
            CargarUsuariosEnTabla2(Txt_BuscarUsuario2.Text.Trim());
        }

        private void Btn_ClearUsuario2_Click(object sender, EventArgs e)
        {
            Txt_BuscarUsuario2.Clear();
            CargarUsuariosEnTabla2();
        }

        private void Txt_BuscarUsuario2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Btn_SearchUsuario2_Click(sender, e); }
        }

        private void Tabla_Usuarios2_SelectionChanged(object sender, EventArgs e)
        {
            _warehouseManagerIdActual = null;
            LimpiarPanelPermisos();

            if (Tabla_Usuarios2.SelectedRows.Count == 0)
            {
                _usuarioSeleccionado2Id = 0;
                ComboBox_BodegaUsuario.Items.Clear();
                return;
            }

            _usuarioSeleccionado2Id = Convert.ToInt32(Tabla_Usuarios2.SelectedRows[0].Cells["UserId"].Value);
            CargarBodegasDelUsuarioEnCombo();
        }

        #endregion Pestana2_CargaUsuarios
        #region Pestana2_ComboBodega

        private void CargarBodegasDelUsuarioEnCombo()
        {
            ComboBox_BodegaUsuario.Items.Clear();

            var bodegas = Ctrl_WarehouseManager.ObtenerBodegasPorUsuario(_usuarioSeleccionado2Id)
                .Where(b => _esAdminGlobal || _bodegasPermitidas.Contains(b.WarehouseId))
                .ToList();

            foreach (var b in bodegas)
                ComboBox_BodegaUsuario.Items.Add(new ClasificacionItem(b.WarehouseManagerId, b.WarehouseName));

            Panel_Derecho2.Enabled = bodegas.Count > 0;

            if (bodegas.Count == 0)
            {
                MessageBox.Show("ESTE USUARIO NO TIENE BODEGAS ASIGNADAS DENTRO DE SU ALCANCE.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ComboBox_BodegaUsuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(ComboBox_BodegaUsuario.SelectedItem is ClasificacionItem seleccionado))
            {
                _warehouseManagerIdActual = null;
                LimpiarPanelPermisos();
                return;
            }

            _warehouseManagerIdActual = seleccionado.Id;
            CargarPermisosDeLaBodega();
        }

        #endregion Pestana2_ComboBodega
        #region Pestana2_GestionPermisos

        private void LimpiarPanelPermisos()
        {
            CheckBox_Registro.Checked = false;
            CheckBox_DespachoEmpleado.Checked = false;
            CheckBox_DespachoBodega.Checked = false;
            CheckBox_AdminBodega.Checked = false;
            Txt_LimiteDespacho.Text = "";
            Txt_LimiteDespacho.Enabled = false;
            _permisosAsignadosActual.Clear();

            bool hayBodegaSeleccionada = _warehouseManagerIdActual.HasValue;

            CheckBox_Registro.Enabled = hayBodegaSeleccionada;
            CheckBox_DespachoEmpleado.Enabled = hayBodegaSeleccionada;
            CheckBox_DespachoBodega.Enabled = hayBodegaSeleccionada;
            CheckBox_AdminBodega.Enabled = hayBodegaSeleccionada && _esAdminGlobal;
            Btn_GuardarPermisos.Enabled = hayBodegaSeleccionada;
        }

        private void CargarPermisosDeLaBodega()
        {
            LimpiarPanelPermisos();

            if (!_warehouseManagerIdActual.HasValue) return;

            var asignados = Ctrl_WarehouseManagerPermissions.ObtenerPermisosAsignados(_warehouseManagerIdActual.Value);
            _permisosAsignadosActual = asignados.ToDictionary(p => p.PermissionCode, p => p, StringComparer.OrdinalIgnoreCase);

            if (_permisosAsignadosActual.ContainsKey("REGISTRO"))
                CheckBox_Registro.Checked = true;

            if (_permisosAsignadosActual.ContainsKey("DESPACHO_EMPLEADO"))
            {
                CheckBox_DespachoEmpleado.Checked = true;
                var permiso = _permisosAsignadosActual["DESPACHO_EMPLEADO"];
                if (permiso.MaxQuantityPerDispatch.HasValue)
                    Txt_LimiteDespacho.Text = permiso.MaxQuantityPerDispatch.Value.ToString("0.##", CultureInfo.InvariantCulture);
            }

            if (_permisosAsignadosActual.ContainsKey("DESPACHO_BODEGA"))
                CheckBox_DespachoBodega.Checked = true;

            if (_permisosAsignadosActual.ContainsKey("ADMIN_BODEGA"))
                CheckBox_AdminBodega.Checked = true;
        }

        private void CheckBox_DespachoEmpleado_CheckedChanged(object sender, EventArgs e)
        {
            Txt_LimiteDespacho.Enabled = CheckBox_DespachoEmpleado.Checked;
            if (!CheckBox_DespachoEmpleado.Checked)
                Txt_LimiteDespacho.Text = "";
        }

        private void Btn_GuardarPermisos_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_warehouseManagerIdActual.HasValue)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA BODEGA.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal? limiteDespacho = null;
                if (CheckBox_DespachoEmpleado.Checked && !string.IsNullOrWhiteSpace(Txt_LimiteDespacho.Text))
                {
                    if (!decimal.TryParse(Txt_LimiteDespacho.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor) || valor <= 0)
                    {
                        MessageBox.Show("EL LÍMITE DE DESPACHO DEBE SER UN NÚMERO VÁLIDO MAYOR A CERO.", "Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    limiteDespacho = valor;
                }

                var confirmacion = MessageBox.Show("¿DESEA GUARDAR LOS PERMISOS DE ESTA BODEGA?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmacion != DialogResult.Yes) return;

                this.Cursor = Cursors.WaitCursor;

                bool bloqueadoPorSuperAdmin =
                    !GuardarPermisoIndividual("REGISTRO", CheckBox_Registro.Checked, null);

                if (!bloqueadoPorSuperAdmin)
                    bloqueadoPorSuperAdmin = !GuardarPermisoIndividual("DESPACHO_EMPLEADO", CheckBox_DespachoEmpleado.Checked, limiteDespacho);

                if (!bloqueadoPorSuperAdmin)
                    bloqueadoPorSuperAdmin = !GuardarPermisoIndividual("DESPACHO_BODEGA", CheckBox_DespachoBodega.Checked, null);

                if (!bloqueadoPorSuperAdmin && _esAdminGlobal)
                    bloqueadoPorSuperAdmin = !GuardarPermisoIndividual("ADMIN_BODEGA", CheckBox_AdminBodega.Checked, null);

                this.Cursor = Cursors.Default;

                if (bloqueadoPorSuperAdmin)
                {
                    MessageBox.Show("NO TIENE PERMISOS PARA MODIFICAR LOS PERMISOS DE UN USUARIO SUPERADMINISTRADOR.",
                        "Sin permiso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("PERMISOS ACTUALIZADOS CORRECTAMENTE.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CargarPermisosDeLaBodega();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL GUARDAR PERMISOS: {ex.Message}", "Error SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Retorna false si fue bloqueado por la regla de SUPERADMIN, true en cualquier otro caso (éxito o no aplicable)
        private bool GuardarPermisoIndividual(string permissionCode, bool marcado, decimal? maxQuantityPerDispatch)
        {
            bool yaExiste = _permisosAsignadosActual.ContainsKey(permissionCode);
            int resultado;

            if (marcado && !yaExiste)
            {
                var permisoCatalogo = _catalogoPermisos.FirstOrDefault(p =>
                    string.Equals(p.PermissionCode, permissionCode, StringComparison.OrdinalIgnoreCase));
                if (permisoCatalogo == null) return true;

                resultado = Ctrl_WarehouseManagerPermissions.AsignarPermiso(
                    _warehouseManagerIdActual.Value, permisoCatalogo.WarehousePermissionId, maxQuantityPerDispatch, UserData.UserId);

                return resultado != -3;
            }
            else if (marcado && yaExiste)
            {
                var actual = _permisosAsignadosActual[permissionCode];
                resultado = Ctrl_WarehouseManagerPermissions.ActualizarPermiso(
                    actual.WarehouseManagerPermissionId, true, maxQuantityPerDispatch);

                return resultado != -2;
            }
            else if (!marcado && yaExiste)
            {
                var actual = _permisosAsignadosActual[permissionCode];
                resultado = Ctrl_WarehouseManagerPermissions.ActualizarPermiso(
                    actual.WarehouseManagerPermissionId, false, null);

                return resultado != -2;
            }

            return true; 
        }

        #endregion Pestana2_GestionPermisos
    }
}