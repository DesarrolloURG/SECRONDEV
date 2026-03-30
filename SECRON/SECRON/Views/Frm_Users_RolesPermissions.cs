using SECRON.Configuration;
using SECRON.Controllers;
using SECRON.Models;
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
    public partial class Frm_Users_RolesPermissions : Form
    {
        #region Propiedades
        public Mdl_Security_UserInfo UserData { get; set; }
        private TabControl tabControl;
        #endregion
        #region Constructor
        private void Frm_Users_RolesPermissions_Load(object sender, EventArgs e)
        {
            ConfigurarMaxLengthTextBox();
        }
        public Frm_Users_RolesPermissions()
        {
            InitializeComponent();
            InicializarPestanas();
            CargarContenidoPestanas();
        }
        #endregion
        #region InicializacionPestanas
        // Inicializar pestañas metodo 
        private void InicializarPestanas()
        {
            // Crear pestañas con TabConfig - ¡SOLO 3 LÍNEAS!
            string[] nombresPestanas = new string[]
            {
                "ASIGNAR ROL A USUARIOS",
                "GESTIONAR PERMISOS DE ROLES",
                "GESTIONAR PERMISOS",
                "PERMISOS ADICIONALES"
            };

            tabControl = TabConfig.CrearTabControl(this, nombresPestanas, 275, 50, false);
        }
        // Cargar contenido de las pestañas
        private void CargarContenidoPestanas()
        {
            // Cargar contenido de cada pestaña
            CargarTab_AsignarRoles();
            CargarTab_GestionarPermisosRoles();
            CargarTab_GestionarPermisos();
            CargarTab_PermisosAdicionales();
        }
        #endregion
        #region ContenidoPestanas
        // Asignar el contenido para cada pestaña
        // Pestaña Asignar Roles
        private void CargarTab_AsignarRoles()
        {
            Panel panel = TabConfig.ObtenerPanel(tabControl, 0);
            panel.Controls.Add(Panel_Asignar);

            // Configurar la pestaña completa
            ConfigurarPestaña1_AsignarRoles();
        }
        // Pestaña Gestionar Permisos de Roles
        private void CargarTab_GestionarPermisosRoles()
        {
            Panel panel = TabConfig.ObtenerPanel(tabControl, 1);
            panel.Controls.Add(Panel_PermisosRoles);

            // Configurar la pestaña completa
            ConfigurarPestaña2_GestionarPermisosRoles();
        }
        // Pestaña Gestionar Permisos
        private void CargarTab_GestionarPermisos()
        {
            Panel panel = TabConfig.ObtenerPanel(tabControl, 2);
            panel.Controls.Add(Panel_GestionarPermisos);

            // Configurar la pestaña completa
            ConfigurarPestaña3_GestionarPermisos();
        }
        // Pestaña Permisos Adicionales
        private void CargarTab_PermisosAdicionales()
        {
            Panel panel = TabConfig.ObtenerPanel(tabControl, 3);
            panel.Controls.Add(Panel_PermisosAdicionales);

            // Configurar la pestaña completa
            ConfigurarPestaña4_GestionarPermisos();
        }
        #endregion
        #region ConfiguracionInicialPestaña1
        private void ConfigurarPestaña1_AsignarRoles()
        {
            // Configurar etiquetas informativas
            ConfigurarEtiquetasInformativas();

            // Configurar Tabla1
            ConfigurarTabla1();

            // Configurar Tabla2
            ConfigurarTabla2();

            // Cargar datos
            CargarUsuariosEnTabla1();
            CargarRolesEnTabla2();

            // Asignar eventos
            AsignarEventosPestaña1();
        }

        private void ConfigurarTabla1()
        {
            Tabla1.Columns.Clear();

            // Columna Checkbox
            DataGridViewCheckBoxColumn colCheck = new DataGridViewCheckBoxColumn
            {
                Name = "Seleccionar",
                HeaderText = "☑",
                Width = 50,
                ReadOnly = false
            };
            Tabla1.Columns.Add(colCheck);

            // Columnas de datos
            Tabla1.Columns.Add("UserId", "ID");
            Tabla1.Columns.Add("Username", "USUARIO");
            Tabla1.Columns.Add("FullName", "NOMBRE COMPLETO");
            Tabla1.Columns.Add("RoleName", "ROL ACTUAL");

            // Ocultar columna ID
            Tabla1.Columns["UserId"].Visible = false;

            // Configuración de comportamiento
            Tabla1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla1.MultiSelect = true;
            Tabla1.ReadOnly = false;
            Tabla1.AllowUserToAddRows = false;
            Tabla1.AllowUserToDeleteRows = false;
            Tabla1.RowHeadersVisible = false;
            Tabla1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Estilos visuales
            Tabla1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            Tabla1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Tabla1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla1.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla1.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            Tabla1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            Tabla1.RowTemplate.Height = 35;
            Tabla1.ColumnHeadersHeight = 40;
        }

        private void ConfigurarTabla2()
        {
            Tabla2.Columns.Clear();

            Tabla2.Columns.Add("RoleId", "ID");
            Tabla2.Columns.Add("RoleName", "NOMBRE DEL ROL");
            Tabla2.Columns.Add("Description", "DESCRIPCIÓN");

            Tabla2.Columns["RoleId"].Visible = false;

            Tabla2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla2.MultiSelect = false;
            Tabla2.ReadOnly = true;
            Tabla2.AllowUserToAddRows = false;
            Tabla2.RowHeadersVisible = false;
            Tabla2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Estilos visuales
            Tabla2.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            Tabla2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Tabla2.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla2.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla2.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            Tabla2.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            Tabla2.RowTemplate.Height = 35;
            Tabla2.ColumnHeadersHeight = 40;
        }

        private void AsignarEventosPestaña1()
        {
            // Eventos de botones
            Btn_Search.Click += Btn_Search_Click;
            Btn_Clear.Click += Btn_Clear_Click;
            //Btn_Asignar.Click += Btn_Asignar_Click;

            // Evento Enter en TextBox de busqueda
            Txt_ValorBuscado.KeyDown += Txt_ValorBuscado_KeyDown;

            // Eventos de controles
            CheckBox_All.CheckedChanged += CheckBox_All_CheckedChanged;
            Tabla1.CellContentClick += Tabla1_CellContentClick;
        }
        #endregion
        #region CargaDatos
        private void CargarUsuariosEnTabla1(string filtro = "")
        {
            try
            {
                Tabla1.Rows.Clear();

                // Obtener usuarios según filtro
                List<Mdl_Users> usuarios;
                if (string.IsNullOrWhiteSpace(filtro))
                {
                    usuarios = Ctrl_Users.MostrarUsuarios(1, 500);
                }
                else
                {
                    usuarios = Ctrl_Users.BuscarUsuarios(filtro, null, null, null, 1, 500);
                }

                // Llenar tabla
                foreach (var usuario in usuarios)
                {
                    string roleName = "SIN ROL";

                    if (usuario.RoleId > 0)
                    {
                        var rol = Ctrl_Roles.ObtenerRolPorId(usuario.RoleId);
                        roleName = rol?.RoleName ?? "DESCONOCIDO";
                    }

                    Tabla1.Rows.Add(
                        false,              // Checkbox desmarcado
                        usuario.UserId,
                        usuario.Username,
                        usuario.FullName,
                        roleName
                    );
                }

                ActualizarContador();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR USUARIOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CargarRolesEnTabla2()
        {
            try
            {
                Tabla2.Rows.Clear();
                var roles = Ctrl_Roles.MostrarRoles(1, 100);

                foreach (var rol in roles)
                {
                    Tabla2.Rows.Add(
                        rol.RoleId,
                        rol.RoleName,
                        rol.Description ?? "Sin descripción"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR ROLES: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        #region EventosBotones
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string filtro = Txt_ValorBuscado.Text.Trim();
            CargarUsuariosEnTabla1(filtro);
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Clear();
            CargarUsuariosEnTabla1();
        }

        private void CheckBox_All_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in Tabla1.Rows)
            {
                row.Cells["Seleccionar"].Value = CheckBox_All.Checked;
            }
            ActualizarContador();
        }

        private void Tabla1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Tabla1.Columns["Seleccionar"].Index && e.RowIndex >= 0)
            {
                Tabla1.CommitEdit(DataGridViewDataErrorContexts.Commit);
                ActualizarContador();
            }
        }

        private void ActualizarContador()
        {
            int count = 0;
            foreach (DataGridViewRow row in Tabla1.Rows)
            {
                if (row.Cells["Seleccionar"].Value != null &&
                    (bool)row.Cells["Seleccionar"].Value == true)
                {
                    count++;
                }
            }

            Lbl_Info3.Text = $"        Usuarios Seleccionados: {count}";
        }

        private void Btn_Asignar_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener usuarios seleccionados
                List<int> usuariosSeleccionados = new List<int>();
                foreach (DataGridViewRow row in Tabla1.Rows)
                {
                    if (row.Cells["Seleccionar"].Value != null &&
                        (bool)row.Cells["Seleccionar"].Value == true)
                    {
                        usuariosSeleccionados.Add(Convert.ToInt32(row.Cells["UserId"].Value));
                    }
                }

                // Validar usuarios seleccionados
                if (usuariosSeleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN USUARIO",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar rol seleccionado
                if (Tabla2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN ROL DE LA TABLA",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int roleId = Convert.ToInt32(Tabla2.SelectedRows[0].Cells["RoleId"].Value);
                string roleName = Tabla2.SelectedRows[0].Cells["RoleName"].Value.ToString();

                // Confirmación
                var confirmacion = MessageBox.Show(
                    $"¿DESEA ASIGNAR EL ROL '{roleName}' A {usuariosSeleccionados.Count} USUARIO(S)?\n\n" +
                    "Esta acción:\n" +
                    "• Reemplazará el rol actual de todos los usuarios seleccionados\n" +
                    "• Limpiará los permisos específicos del usuario\n" +
                    "• Asignará automáticamente los permisos del nuevo rol",
                    "CONFIRMAR ASIGNACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                this.Cursor = Cursors.WaitCursor;

                // Asignar rol a cada usuario
                int exitosos = 0;
                foreach (int userId in usuariosSeleccionados)
                {
                    var usuario = Ctrl_Users.ObtenerUsuarioPorId(userId);
                    if (usuario != null)
                    {
                        usuario.RoleId = roleId;
                        usuario.ModifiedBy = UserData?.UserId;

                        if (Ctrl_Users.ActualizarUsuario(usuario) > 0)
                        {
                            Ctrl_UserPermissions.EliminarTodosLosPermisosDeUsuario(userId);
                            exitosos++;
                        }
                    }
                }

                this.Cursor = Cursors.Default;

                MessageBox.Show(
                    $"✓ OPERACIÓN COMPLETADA\n\n" +
                    $"Se asignó el rol '{roleName}' a {exitosos} de {usuariosSeleccionados.Count} usuario(s).\n" +
                    $"Los permisos del rol se aplicarán automáticamente.",
                    "ÉXITO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CheckBox_All.Checked = false;
                CargarUsuariosEnTabla1();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL ASIGNAR ROL: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        #region ConfiguracionEtiquetasInformativas
        private void ConfigurarEtiquetasInformativas()
        {
            // Lbl_Info1 - Información general (azul)
            Lbl_Info1.AutoSize = false;
            Lbl_Info1.Size = new Size(850, 40);
            Lbl_Info1.Text = "       Selecciona uno o varios usuarios y asigna UN ROL a todos los seleccionados. Cada usuario solo puede tener un rol activo.";
            Lbl_Info1.BackColor = Color.FromArgb(217, 237, 247);
            Lbl_Info1.ForeColor = Color.FromArgb(31, 45, 61);
            Lbl_Info1.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            Lbl_Info1.Padding = new Padding(10, 8, 10, 8);
            Lbl_Info1.BorderStyle = BorderStyle.FixedSingle;

            // Lbl_Info2 - Advertencia importante (rojo) - ANCHO DE TABLA2
            Lbl_Info2.AutoSize = false;
            Lbl_Info2.Size = new Size(Tabla2.Width, 50);
            Lbl_Info2.Text = "       IMPORTANTE: el rol seleccionado reemplazará el rol actual\n       de todos los usuarios";
            Lbl_Info2.BackColor = Color.FromArgb(248, 215, 218);
            Lbl_Info2.ForeColor = Color.FromArgb(114, 28, 36);
            Lbl_Info2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            Lbl_Info2.Padding = new Padding(10, 8, 10, 8);
            Lbl_Info2.BorderStyle = BorderStyle.FixedSingle;
            Lbl_Info2.TextAlign = ContentAlignment.MiddleLeft;

            // Lbl_Info3 - Contador de seleccionados (amarillo/naranja) - COMPACTO
            Lbl_Info3.AutoSize = false;
            Lbl_Info3.Size = new Size(300, 30); // Ancho fijo compacto
            Lbl_Info3.Text = "       Usuarios Seleccionados: 0";
            Lbl_Info3.BackColor = Color.FromArgb(255, 243, 205);
            Lbl_Info3.ForeColor = Color.FromArgb(133, 100, 4);
            Lbl_Info3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            Lbl_Info3.Padding = new Padding(8, 6, 8, 6);
            Lbl_Info3.BorderStyle = BorderStyle.FixedSingle;
            Lbl_Info3.TextAlign = ContentAlignment.MiddleLeft;
        }
        #endregion ConfiguracionEtiquetasInformativas
        #region ConfiguracionInicialPestaña2
        private void ConfigurarPestaña2_GestionarPermisosRoles()
        {
            // Configurar etiquetas informativas
            ConfigurarEtiquetasInformativasPestaña2();

            // Configurar ComboBox de Roles
            ConfigurarComboBoxRoles();

            // Configurar Tabla3 (Permisos Disponibles)
            ConfigurarTabla3();

            // Configurar Tabla4 (Permisos Asignados)
            ConfigurarTabla4();

            CargarTodosLosPermisos();

            // Asignar eventos
            AsignarEventosPestaña2();
        }
        // Configurar etiquetas informativas
        private void ConfigurarEtiquetasInformativasPestaña2()
        {
            // Aplicar el mismo estilo que Lbl_Info1
            Lbl_Info4.AutoSize = false;
            Lbl_Info4.Size = new Size(750, 40);
            Lbl_Info4.Location = new Point(335, 20);
            Lbl_Info4.Text = "        Seleccione un rol y asigne o quite permisos. Los permisos controlan qué acciones puede realizar cada rol en el sistema.";
            Lbl_Info4.BackColor = Color.FromArgb(217, 237, 247);
            Lbl_Info4.ForeColor = Color.FromArgb(31, 45, 61);
            Lbl_Info4.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            Lbl_Info4.Padding = new Padding(10, 8, 10, 8);
            Lbl_Info4.BorderStyle = BorderStyle.FixedSingle;
        }
        // Configurar ComboBox de Roles
        private void ConfigurarComboBoxRoles()
        {
            try
            {
                // Limpiar items existentes
                ComboBox_Roles.Items.Clear();

                // AGREGAR ESTA LÍNEA - Hacer el ComboBox de solo lectura
                ComboBox_Roles.DropDownStyle = ComboBoxStyle.DropDownList;

                // Agregar item por defecto
                ComboBox_Roles.Items.Add(new ComboBoxItem { Text = "-- SELECCIONE UN ROL --", Value = 0 });

                // Obtener y agregar roles
                var roles = Ctrl_Roles.ObtenerTodosLosRoles();
                foreach (var rol in roles)
                {
                    ComboBox_Roles.Items.Add(new ComboBoxItem
                    {
                        Text = rol.Value,
                        Value = rol.Key
                    });
                }

                // Seleccionar el primer item
                if (ComboBox_Roles.Items.Count > 0)
                    ComboBox_Roles.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Configurar Tabla3 (Permisos Disponibles)
        private void ConfigurarTabla3()
        {
            Tabla3.Columns.Clear();

            // Columna Checkbox
            DataGridViewCheckBoxColumn colCheck = new DataGridViewCheckBoxColumn
            {
                Name = "Seleccionar",
                HeaderText = "☑",
                Width = 50,
                ReadOnly = false
            };
            Tabla3.Columns.Add(colCheck);

            // Columnas de datos
            Tabla3.Columns.Add("PermissionId", "ID");
            Tabla3.Columns.Add("PermissionCode", "CÓDIGO");
            Tabla3.Columns.Add("PermissionName", "PERMISO");
            Tabla3.Columns.Add("ModuleName", "MÓDULO");
            Tabla3.Columns.Add("ActionType", "ACCIÓN");

            // Ocultar columna ID
            Tabla3.Columns["PermissionId"].Visible = false;

            // Configuración de comportamiento
            Tabla3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla3.MultiSelect = true;
            Tabla3.ReadOnly = false;
            Tabla3.AllowUserToAddRows = false;
            Tabla3.AllowUserToDeleteRows = false;
            Tabla3.RowHeadersVisible = false;
            Tabla3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Estilos visuales
            Tabla3.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            Tabla3.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla3.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla3.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Tabla3.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla3.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla3.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            Tabla3.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            Tabla3.RowTemplate.Height = 35;
            Tabla3.ColumnHeadersHeight = 40;
        }
        // Configurar Tabla4 (Permisos Asignados)
        private void ConfigurarTabla4()
        {
            Tabla4.Columns.Clear();

            // Columna Checkbox
            DataGridViewCheckBoxColumn colCheck = new DataGridViewCheckBoxColumn
            {
                Name = "Seleccionar",
                HeaderText = "☑",
                Width = 50,
                ReadOnly = false
            };
            Tabla4.Columns.Add(colCheck);

            // Columnas de datos
            Tabla4.Columns.Add("RolePermissionId", "RPID");
            Tabla4.Columns.Add("PermissionId", "ID");
            Tabla4.Columns.Add("PermissionCode", "CÓDIGO");
            Tabla4.Columns.Add("PermissionName", "PERMISO");
            Tabla4.Columns.Add("ModuleName", "MÓDULO");
            Tabla4.Columns.Add("ActionType", "ACCIÓN");

            // Ocultar columnas ID
            Tabla4.Columns["RolePermissionId"].Visible = false;
            Tabla4.Columns["PermissionId"].Visible = false;

            // Configuración de comportamiento
            Tabla4.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla4.MultiSelect = true;
            Tabla4.ReadOnly = false;
            Tabla4.AllowUserToAddRows = false;
            Tabla4.AllowUserToDeleteRows = false;
            Tabla4.RowHeadersVisible = false;
            Tabla4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Estilos visuales
            Tabla4.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            Tabla4.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla4.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla4.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Tabla4.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla4.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla4.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            Tabla4.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            Tabla4.RowTemplate.Height = 35;
            Tabla4.ColumnHeadersHeight = 40;
        }
        // Nuevo método para cargar TODOS los permisos del sistema
        private void CargarTodosLosPermisos(string filtro = "")
        {
            try
            {
                Tabla3.Rows.Clear();

                // Obtener todos los permisos activos del sistema
                List<Mdl_Permissions> todosPermisos;

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    todosPermisos = Ctrl_Permissions.BuscarPermisos(filtro.ToUpper(), "", "", 1, 1000);
                }
                else
                {
                    todosPermisos = Ctrl_Permissions.MostrarPermisos(1, 1000);
                }

                // Ordenar por módulo y nombre
                todosPermisos = todosPermisos
                    .OrderBy(p => p.ModuleName)
                    .ThenBy(p => p.PermissionName)
                    .ToList();

                // Llenar tabla con TODOS los permisos
                foreach (var permiso in todosPermisos)
                {
                    Tabla3.Rows.Add(
                        false,  // Checkbox
                        permiso.PermissionId,
                        permiso.PermissionCode,
                        permiso.PermissionName,
                        permiso.ModuleName ?? "Sin módulo",
                        permiso.ActionType ?? "Sin acción"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar permisos del sistema: {ex.Message}",
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Asignar eventos
        private void AsignarEventosPestaña2()
        {
            // Eventos del ComboBox
            ComboBox_Roles.SelectedIndexChanged += ComboBox_Roles_SelectedIndexChanged;

            // Eventos Enter en TextBox de busqueda
            Txt_ValorBuscado2.KeyDown += Txt_ValorBuscado2_KeyDown;
            Txt_ValorBuscado3.KeyDown += Txt_ValorBuscado3_KeyDown;

            // Eventos de búsqueda Tabla3
            //Btn_Search2.Click += Btn_Search2_Click;
            //Btn_Clear2.Click += Btn_Clear2_Click;

            //// Eventos de búsqueda Tabla4
            //Btn_Search3.Click += Btn_Search3_Click;
            //Btn_Clear3.Click += Btn_Clear3_Click;

            // Eventos de checkboxes en tablas
            Tabla3.CellContentClick += Tabla3_CellContentClick;
            Tabla4.CellContentClick += Tabla4_CellContentClick;

            //// Eventos de botones de acción
            //Btn_Add1.Click += Btn_Add1_Click;
            //Btn_AddAll1.Click += Btn_AddAll1_Click;
            //Btn_Remove1.Click += Btn_Remove1_Click;
            //Btn_RemoveAll1.Click += Btn_RemoveAll1_Click;
        }
        // Clase helper para el ComboBox
        public class ComboBoxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        #endregion ConfiguracionInicialPestaña2
        #region CargarDatos2
        // Cargar permisos asignados al rol seleccionado
        private void CargarPermisosAsignados(int roleId, string filtro = "")
        {
            try
            {
                Tabla4.Rows.Clear();

                // Obtener permisos asignados al rol
                var permisosAsignados = Ctrl_RolePermissions.ObtenerPermisosPorRol(roleId)
                    .Where(rp => rp.IsGranted == true)
                    .ToList();

                // Obtener información completa de cada permiso
                var permisosCompletos = new List<(int RolePermissionId, Mdl_Permissions Permiso)>();

                foreach (var rp in permisosAsignados)
                {
                    var permiso = Ctrl_Permissions.ObtenerPermisoPorId(rp.PermissionId);
                    if (permiso != null)
                    {
                        permisosCompletos.Add((rp.RolePermissionId, permiso));
                    }
                }

                // Aplicar filtro de búsqueda si existe
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    filtro = filtro.ToUpper();
                    permisosCompletos = permisosCompletos.Where(pc =>
                        (pc.Permiso.PermissionCode?.ToUpper().Contains(filtro) ?? false) ||
                        (pc.Permiso.PermissionName?.ToUpper().Contains(filtro) ?? false) ||
                        (pc.Permiso.ModuleName?.ToUpper().Contains(filtro) ?? false) ||
                        (pc.Permiso.ActionType?.ToUpper().Contains(filtro) ?? false)
                    ).ToList();
                }

                // Llenar tabla
                foreach (var item in permisosCompletos.OrderBy(pc => pc.Permiso.ModuleName).ThenBy(pc => pc.Permiso.PermissionName))
                {
                    Tabla4.Rows.Add(
                        false,
                        item.RolePermissionId,
                        item.Permiso.PermissionId,
                        item.Permiso.PermissionCode,
                        item.Permiso.PermissionName,
                        item.Permiso.ModuleName ?? "",
                        item.Permiso.ActionType ?? ""
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR PERMISOS ASIGNADOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion CargarDatos2
        #region EventosPestaña2
        // Evento cuando cambia la selección del ComboBox de Roles
        private void ComboBox_Roles_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Verificar que se seleccionó un rol válido
                if (ComboBox_Roles.SelectedItem is ComboBoxItem selectedItem && selectedItem.Value > 0)
                {
                    int roleId = selectedItem.Value;

                    // Cargar permisos asignados (los que SÍ tiene el rol)
                    CargarPermisosAsignados(roleId);
                }
                else
                {
                    // Limpiar tablas si no hay rol seleccionado
                    Tabla4.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar rol: {ex.Message}",
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Eventos de búsqueda y limpieza en ambas tablas
        private void Btn_Search2_Click(object sender, EventArgs e)
        {
            // Buscar en TODOS los permisos, sin importar el rol
            string filtro = Txt_ValorBuscado2.Text.Trim();
            CargarTodosLosPermisos(filtro);
        }
        // Eventos de búsqueda y limpieza en ambas tablas
        private void Btn_Clear2_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado2.Clear();
            // Cargar TODOS los permisos sin filtro
            CargarTodosLosPermisos();
        }
        // Eventos de búsqueda y limpieza en ambas tablas
        private void Btn_Search3_Click(object sender, EventArgs e)
        {
            if (ComboBox_Roles.SelectedIndex > 0 && ComboBox_Roles.SelectedItem is ComboBoxItem selectedItem)
            {
                int roleId = selectedItem.Value;
                string filtro = Txt_ValorBuscado3.Text.Trim();
                CargarPermisosAsignados(roleId, filtro);
            }
        }
        // Eventos de búsqueda y limpieza en ambas tablas
        private void Btn_Clear3_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado3.Clear();
            if (ComboBox_Roles.SelectedIndex > 0 && ComboBox_Roles.SelectedItem is ComboBoxItem selectedItem)
            {
                int roleId = selectedItem.Value;
                CargarPermisosAsignados(roleId);
            }
        }
        // Eventos de checkboxes en tablas
        private void Tabla3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Tabla3.Columns["Seleccionar"].Index && e.RowIndex >= 0)
            {
                Tabla3.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        // Eventos de checkboxes en tablas
        private void Tabla4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Tabla4.Columns["Seleccionar"].Index && e.RowIndex >= 0)
            {
                Tabla4.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        // Evento botón agregar permisos seleccionados
        private void Btn_Add1_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_Roles.SelectedIndex <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN ROL PRIMERO",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtener permisos seleccionados
                List<int> permisosSeleccionados = new List<int>();
                foreach (DataGridViewRow row in Tabla3.Rows)
                {
                    if (row.Cells["Seleccionar"].Value != null &&
                        (bool)row.Cells["Seleccionar"].Value == true)
                    {
                        permisosSeleccionados.Add(Convert.ToInt32(row.Cells["PermissionId"].Value));
                    }
                }

                if (permisosSeleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN PERMISO",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_Roles.SelectedItem;
                int roleId = selectedItem.Value;

                this.Cursor = Cursors.WaitCursor;

                // Asignar permisos
                int exitosos = 0;
                foreach (int permissionId in permisosSeleccionados)
                {
                    // Verificar si ya existe la asignación
                    if (!Ctrl_RolePermissions.ExisteAsignacion(roleId, permissionId))
                    {
                        Mdl_RolePermissions rp = new Mdl_RolePermissions
                        {
                            RoleId = roleId,
                            PermissionId = permissionId,
                            IsGranted = true,
                            CreatedBy = UserData?.UserId ?? 1
                        };

                        if (Ctrl_RolePermissions.AsignarPermisoARol(rp) > 0)
                        {
                            exitosos++;
                        }
                    }
                    else
                    {
                        // Si ya existe, actualizar su estado a IsGranted = true
                        var permisoExistente = Ctrl_RolePermissions.ObtenerPermisosPorRol(roleId)
                            .FirstOrDefault(rp => rp.PermissionId == permissionId);

                        if (permisoExistente != null && !permisoExistente.IsGranted)
                        {
                            if (Ctrl_RolePermissions.ActualizarEstadoPermiso(permisoExistente.RolePermissionId, true) > 0)
                            {
                                exitosos++;
                            }
                        }
                    }
                }

                this.Cursor = Cursors.Default;

                MessageBox.Show($"✓ {exitosos} PERMISO(S) ASIGNADO(S) CORRECTAMENTE",
                    "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar tablas
                CargarPermisosAsignados(roleId);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL AGREGAR PERMISOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Evento botón agregar todos los permisos permisos
        private void Btn_AddAll1_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_Roles.SelectedIndex <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN ROL PRIMERO",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Tabla3.Rows.Count == 0)
                {
                    MessageBox.Show("NO HAY PERMISOS DISPONIBLES PARA AGREGAR",
                        "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_Roles.SelectedItem;
                int roleId = selectedItem.Value;
                string roleName = selectedItem.Text;

                var confirmacion = MessageBox.Show(
                    $"¿DESEA ASIGNAR TODOS LOS PERMISOS DISPONIBLES AL ROL '{roleName}'?\n\n" +
                    $"Se asignarán {Tabla3.Rows.Count} permiso(s).",
                    "CONFIRMAR ASIGNACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                this.Cursor = Cursors.WaitCursor;

                // Asignar todos los permisos
                int exitosos = 0;
                foreach (DataGridViewRow row in Tabla3.Rows)
                {
                    int permissionId = Convert.ToInt32(row.Cells["PermissionId"].Value);

                    if (!Ctrl_RolePermissions.ExisteAsignacion(roleId, permissionId))
                    {
                        Mdl_RolePermissions rp = new Mdl_RolePermissions
                        {
                            RoleId = roleId,
                            PermissionId = permissionId,
                            IsGranted = true,
                            CreatedBy = UserData?.UserId ?? 1
                        };

                        if (Ctrl_RolePermissions.AsignarPermisoARol(rp) > 0)
                        {
                            exitosos++;
                        }
                    }
                    else
                    {
                        var permisoExistente = Ctrl_RolePermissions.ObtenerPermisosPorRol(roleId)
                            .FirstOrDefault(rp => rp.PermissionId == permissionId);

                        if (permisoExistente != null && !permisoExistente.IsGranted)
                        {
                            if (Ctrl_RolePermissions.ActualizarEstadoPermiso(permisoExistente.RolePermissionId, true) > 0)
                            {
                                exitosos++;
                            }
                        }
                    }
                }

                this.Cursor = Cursors.Default;

                MessageBox.Show($"✓ {exitosos} DE {Tabla3.Rows.Count} PERMISO(S) ASIGNADO(S) CORRECTAMENTE",
                    "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar tablas
                CargarPermisosAsignados(roleId);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL AGREGAR TODOS LOS PERMISOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Evento botón quitar permisos seleccionados
        private void Btn_Remove1_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_Roles.SelectedIndex <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN ROL PRIMERO",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtener permisos seleccionados
                List<int> rolePermissionsSeleccionados = new List<int>();
                foreach (DataGridViewRow row in Tabla4.Rows)
                {
                    if (row.Cells["Seleccionar"].Value != null &&
                        (bool)row.Cells["Seleccionar"].Value == true)
                    {
                        rolePermissionsSeleccionados.Add(Convert.ToInt32(row.Cells["RolePermissionId"].Value));
                    }
                }

                if (rolePermissionsSeleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN PERMISO",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE QUITAR {rolePermissionsSeleccionados.Count} PERMISO(S) DEL ROL?",
                    "CONFIRMAR",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_Roles.SelectedItem;
                int roleId = selectedItem.Value;

                this.Cursor = Cursors.WaitCursor;

                // Quitar permisos (actualizar IsGranted = false o eliminar)
                int exitosos = 0;
                foreach (int rolePermissionId in rolePermissionsSeleccionados)
                {
                    // Opción 1: Actualizar IsGranted = false (recomendado para mantener historial)
                    if (Ctrl_RolePermissions.ActualizarEstadoPermiso(rolePermissionId, false) > 0)
                    {
                        exitosos++;
                    }
                    // Opción 2: Eliminar completamente (descomenta si prefieres esta opción)
                    // if (Ctrl_RolePermissions.EliminarPermisoDeRol(rolePermissionId) > 0)
                    // {
                    //     exitosos++;
                    // }
                }

                this.Cursor = Cursors.Default;

                MessageBox.Show($"✓ {exitosos} PERMISO(S) REMOVIDO(S) CORRECTAMENTE",
                    "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarPermisosAsignados(roleId);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL QUITAR PERMISOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Evento botón quitar todos los permisos
        private void Btn_RemoveAll1_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_Roles.SelectedIndex <= 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN ROL PRIMERO",
                        "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Tabla4.Rows.Count == 0)
                {
                    MessageBox.Show("NO HAY PERMISOS ASIGNADOS PARA QUITAR",
                        "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_Roles.SelectedItem;
                int roleId = selectedItem.Value;
                string roleName = selectedItem.Text;

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE QUITAR TODOS LOS PERMISOS DEL ROL '{roleName}'?\n\n" +
                    $"Se quitarán {Tabla4.Rows.Count} permiso(s).",
                    "CONFIRMAR",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.No)
                    return;

                this.Cursor = Cursors.WaitCursor;

                // Opción 1: Actualizar todos a IsGranted = false
                int exitosos = 0;
                foreach (DataGridViewRow row in Tabla4.Rows)
                {
                    int rolePermissionId = Convert.ToInt32(row.Cells["RolePermissionId"].Value);
                    if (Ctrl_RolePermissions.ActualizarEstadoPermiso(rolePermissionId, false) > 0)
                    {
                        exitosos++;
                    }
                }

                // Opción 2: Eliminar todos completamente (descomenta si prefieres esta opción)
                // int exitosos = Ctrl_RolePermissions.EliminarTodosLosPermisosDeRol(roleId);

                this.Cursor = Cursors.Default;

                MessageBox.Show($"✓ {exitosos} DE {Tabla4.Rows.Count} PERMISO(S) REMOVIDO(S) CORRECTAMENTE",
                    "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarPermisosAsignados(roleId);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL QUITAR TODOS LOS PERMISOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion EventosPestaña2
        #region ConfiguracionInicialPestaña3
        private int _permisoSeleccionadoId = 0; // Variable de clase para guardar el ID seleccionado
        // Configuración inicial de la pestaña 3 - Gestión de Permisos
        private void ConfigurarPestaña3_GestionarPermisos()
        {
            // Configurar etiqueta informativa
            ConfigurarEtiquetaInformativaPestaña3();

            // Configurar ComboBox de tipos de acción
            ConfigurarComboBoxActionType();

            // Configurar Tabla5
            ConfigurarTabla5();

            // Cargar filtros de búsqueda
            CargarFiltros();

            // Cargar permisos iniciales
            CargarPermisosEnTabla5();

            // Asignar eventos
            AsignarEventosPestaña3();

            // Limpiar campos al inicio
            LimpiarCamposPermiso();
        }
        // Configurar etiqueta informativa
        private void ConfigurarEtiquetaInformativaPestaña3()
        {
            Lbl_Info5.AutoSize = false;
            Lbl_Info5.Size = new Size(850, 40);
            Lbl_Info5.Text = "        Registre los permisos según el diccionario utilizado para su respectivo ordenamiento. Ej: \"EMPLOYEES\",\"EMP_085\",\"CREATE\"";
            Lbl_Info5.BackColor = Color.FromArgb(217, 237, 247);
            Lbl_Info5.ForeColor = Color.FromArgb(31, 45, 61);
            Lbl_Info5.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            Lbl_Info5.Padding = new Padding(10, 8, 10, 8);
            Lbl_Info5.BorderStyle = BorderStyle.FixedSingle;
        }
        // Configurar ComboBox de tipos de acción
        private void ConfigurarComboBoxActionType()
        {
            try
            {
                ComboBox_ActionType.Items.Clear();
                ComboBox_ActionType.DropDownStyle = ComboBoxStyle.DropDownList;

                // Agregar item por defecto
                ComboBox_ActionType.Items.Add("-- SELECCIONE ACCIÓN --");

                // Agregar tipos de acción
                ComboBox_ActionType.Items.Add("CREATE");
                ComboBox_ActionType.Items.Add("READ");
                ComboBox_ActionType.Items.Add("UPDATE");
                ComboBox_ActionType.Items.Add("INACTIVE");
                ComboBox_ActionType.Items.Add("APPROVE");
                ComboBox_ActionType.Items.Add("EXPORT");
                ComboBox_ActionType.Items.Add("IMPORT");
                ComboBox_ActionType.Items.Add("EXECUTE");
                ComboBox_ActionType.Items.Add("TAB");

                ComboBox_ActionType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al configurar tipos de acción: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Configurar Tabla5 (Gestión de Permisos)
        private void ConfigurarTabla5()
        {
            Tabla5.Columns.Clear();

            // Columnas de datos
            Tabla5.Columns.Add("PermissionId", "ID");
            Tabla5.Columns.Add("PermissionCode", "CÓDIGO");
            Tabla5.Columns.Add("PermissionName", "NOMBRE");
            Tabla5.Columns.Add("ModuleName", "MÓDULO");
            Tabla5.Columns.Add("ActionType", "ACCIÓN");
            Tabla5.Columns.Add("IsActive", "ESTADO");

            // Ocultar columna ID
            Tabla5.Columns["PermissionId"].Visible = false;

            // Configuración de comportamiento
            Tabla5.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla5.MultiSelect = false;
            Tabla5.ReadOnly = true;
            Tabla5.AllowUserToAddRows = false;
            Tabla5.AllowUserToDeleteRows = false;
            Tabla5.RowHeadersVisible = false;
            Tabla5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Estilos visuales
            Tabla5.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            Tabla5.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla5.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla5.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Tabla5.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla5.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla5.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            Tabla5.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            Tabla5.RowTemplate.Height = 35;
            Tabla5.ColumnHeadersHeight = 40;
        }
        // Asignar eventos a controles de la pestaña 3
        private void AsignarEventosPestaña3()
        {
            // Eventos de botones principales
            Btn_Save.Click += Btn_Save_Click;
            Btn_Update.Click += Btn_Update_Click;
            Btn_Inactive.Click += Btn_Inactive_Click;
            Btn_Clear5.Click += Btn_Clear5_Click;

            // Eventos de búsqueda
            Btn_Search5.Click += Btn_Search5_Click;
            Btn_CleanSearch.Click += Btn_CleanSearch_Click;

            // Evento Enter en TextBox de busqueda
            Txt_ValorBuscado5.KeyDown += Txt_ValorBuscado5_KeyDown;

            // Evento de selección en tabla
            Tabla5.SelectionChanged += Tabla5_SelectionChanged;

            // Eventos de filtros
            Filtro1.SelectedIndexChanged += AplicarFiltros;
            Filtro2.SelectedIndexChanged += AplicarFiltros;
            Filtro3.SelectedIndexChanged += AplicarFiltros;
        }
        #endregion ConfiguracionInicialPestaña3
        #region CargaDatos3
        // Evento botón buscar en Tabla5
        private void CargarPermisosEnTabla5(string filtro = "")
        {
            try
            {
                Tabla5.Rows.Clear();

                List<Mdl_Permissions> permisos;

                // Cargar TODOS los registros (sin paginacion - usar tamaño grande)
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    permisos = Ctrl_Permissions.BuscarPermisos(filtro.ToUpper(), "", "", 1, 10000);
                }
                else
                {
                    permisos = Ctrl_Permissions.MostrarPermisos(1, 10000);
                }

                foreach (var permiso in permisos)
                {
                    string estado = permiso.IsActive ? "ACTIVO" : "INACTIVO";

                    int rowIndex = Tabla5.Rows.Add(
                        permiso.PermissionId,
                        permiso.PermissionCode,
                        permiso.PermissionName,
                        permiso.ModuleName ?? "",
                        permiso.ActionType ?? "",
                        estado
                    );

                    // Colorear fila si está inactivo
                    if (!permiso.IsActive)
                    {
                        Tabla5.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                    }
                }

                // Actualizar contador
                int totalPermisos = Ctrl_Permissions.ContarTotalPermisos(filtro, "", "");
                int mostrando = permisos.Count;
                Lbl_Paginas.Text = $"MOSTRANDO {mostrando} DE {totalPermisos} PERMISOS";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar permisos: {ex.Message}",
                               "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Método para limpiar campos del formulario de permisos
        private void LimpiarCamposPermiso()
        {
            _permisoSeleccionadoId = 0;
            Txt_PermissionCode.Clear();
            Txt_PermissionName.Clear();
            Txt_ModuleName.Clear();
            Txt_Description.Clear();
            ComboBox_ActionType.SelectedIndex = 0;

            // Habilitar botón guardar y deshabilitar actualizar/inactivar
            Btn_Save.Enabled = true;
            Btn_Update.Enabled = false;
            Btn_Inactive.Enabled = false;
        }
        #endregion CargaDatos3
        #region EventosPestaña3
        // Evento de tabla selección cambiada
        private void Tabla5_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla5.SelectedRows.Count > 0)
            {
                DataGridViewRow row = Tabla5.SelectedRows[0];

                // Guardar ID del permiso seleccionado
                _permisoSeleccionadoId = Convert.ToInt32(row.Cells["PermissionId"].Value);

                // Obtener el permiso completo
                var permiso = Ctrl_Permissions.ObtenerPermisoPorId(_permisoSeleccionadoId);

                if (permiso != null)
                {
                    // Llenar campos
                    Txt_PermissionCode.Text = permiso.PermissionCode;
                    Txt_PermissionName.Text = permiso.PermissionName;
                    Txt_ModuleName.Text = permiso.ModuleName ?? "";
                    Txt_Description.Text = permiso.Description ?? "";

                    // Seleccionar tipo de acción
                    if (!string.IsNullOrEmpty(permiso.ActionType))
                    {
                        int index = ComboBox_ActionType.FindStringExact(permiso.ActionType);
                        ComboBox_ActionType.SelectedIndex = index >= 0 ? index : 0;
                    }
                    else
                    {
                        ComboBox_ActionType.SelectedIndex = 0;
                    }

                    // Configurar botones
                    Btn_Save.Enabled = false;
                    Btn_Update.Enabled = permiso.IsActive;
                    Btn_Inactive.Enabled = permiso.IsActive;
                }
            }
        }
        // Evento para guardar nuevo permiso
        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(Txt_PermissionCode.Text))
                {
                    MessageBox.Show("EL CÓDIGO DEL PERMISO ES OBLIGATORIO", "VALIDACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_PermissionCode.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(Txt_PermissionName.Text))
                {
                    MessageBox.Show("EL NOMBRE DEL PERMISO ES OBLIGATORIO", "VALIDACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_PermissionName.Focus();
                    return;
                }

                // Validar que el código sea único
                if (!Ctrl_Permissions.ValidarCodigoPermisoUnico(Txt_PermissionCode.Text.Trim().ToUpper()))
                {
                    MessageBox.Show("EL CÓDIGO DEL PERMISO YA EXISTE", "VALIDACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_PermissionCode.Focus();
                    return;
                }

                // Confirmar registro
                var confirmacion = MessageBox.Show(
                    "¿ESTÁ SEGURO DE REGISTRAR ESTE PERMISO?",
                    "CONFIRMAR REGISTRO",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No) return;

                this.Cursor = Cursors.WaitCursor;

                // Crear modelo con datos del formulario
                Mdl_Permissions nuevoPermiso = new Mdl_Permissions
                {
                    PermissionCode = Txt_PermissionCode.Text.Trim().ToUpper(),
                    PermissionName = Txt_PermissionName.Text.Trim().ToUpper(),
                    Description = string.IsNullOrWhiteSpace(Txt_Description.Text) ? null : Txt_Description.Text.Trim(),
                    ModuleName = string.IsNullOrWhiteSpace(Txt_ModuleName.Text) ? null : Txt_ModuleName.Text.Trim().ToUpper(),
                    ActionType = ComboBox_ActionType.SelectedIndex > 0 ? ComboBox_ActionType.SelectedItem.ToString() : null,
                    IsActive = true
                };

                // Registrar en base de datos
                int resultado = Ctrl_Permissions.RegistrarPermiso(nuevoPermiso);

                this.Cursor = Cursors.Default;

                if (resultado > 0)
                {
                    MessageBox.Show("✓ PERMISO REGISTRADO CORRECTAMENTE", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarCamposPermiso();
                    CargarPermisosEnTabla5();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO REGISTRAR EL PERMISO", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL REGISTRAR PERMISO: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Evento para actualizar permiso seleccionado
        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que haya un permiso seleccionado
                if (_permisoSeleccionadoId == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN PERMISO DE LA TABLA", "VALIDACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(Txt_PermissionCode.Text))
                {
                    MessageBox.Show("EL CÓDIGO DEL PERMISO ES OBLIGATORIO", "VALIDACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_PermissionCode.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(Txt_PermissionName.Text))
                {
                    MessageBox.Show("EL NOMBRE DEL PERMISO ES OBLIGATORIO", "VALIDACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_PermissionName.Focus();
                    return;
                }

                // Validar que el código sea único (excluyendo el permiso actual)
                if (!Ctrl_Permissions.ValidarCodigoPermisoUnico(Txt_PermissionCode.Text.Trim().ToUpper(), _permisoSeleccionadoId))
                {
                    MessageBox.Show("EL CÓDIGO DEL PERMISO YA EXISTE", "VALIDACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_PermissionCode.Focus();
                    return;
                }

                // Confirmar actualización
                var confirmacion = MessageBox.Show(
                    "¿ESTÁ SEGURO DE ACTUALIZAR ESTE PERMISO?",
                    "CONFIRMAR ACTUALIZACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No) return;

                this.Cursor = Cursors.WaitCursor;

                // Crear modelo con datos actualizados
                Mdl_Permissions permisoActualizado = new Mdl_Permissions
                {
                    PermissionId = _permisoSeleccionadoId,
                    PermissionCode = Txt_PermissionCode.Text.Trim().ToUpper(),
                    PermissionName = Txt_PermissionName.Text.Trim().ToUpper(),
                    Description = string.IsNullOrWhiteSpace(Txt_Description.Text) ? null : Txt_Description.Text.Trim(),
                    ModuleName = string.IsNullOrWhiteSpace(Txt_ModuleName.Text) ? null : Txt_ModuleName.Text.Trim().ToUpper(),
                    ActionType = ComboBox_ActionType.SelectedIndex > 0 ? ComboBox_ActionType.SelectedItem.ToString() : null
                };

                // Actualizar en base de datos
                int resultado = Ctrl_Permissions.ActualizarPermiso(permisoActualizado);

                this.Cursor = Cursors.Default;

                if (resultado > 0)
                {
                    MessageBox.Show("✓ PERMISO ACTUALIZADO CORRECTAMENTE", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarCamposPermiso();
                    CargarPermisosEnTabla5();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO ACTUALIZAR EL PERMISO", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL ACTUALIZAR PERMISO: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Evento para inactivar permiso seleccionado
        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que haya un permiso seleccionado
                if (_permisoSeleccionadoId == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN PERMISO DE LA TABLA", "VALIDACIÓN",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirmar inactivación
                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE INACTIVAR EL PERMISO '{Txt_PermissionName.Text}'?\n\n" +
                    "NOTA: Los usuarios y roles que tengan este permiso ya no podrán ejecutar esta acción.",
                    "CONFIRMAR INACTIVACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.No) return;

                this.Cursor = Cursors.WaitCursor;

                // Inactivar en base de datos
                int resultado = Ctrl_Permissions.InactivarPermiso(_permisoSeleccionadoId);

                this.Cursor = Cursors.Default;

                if (resultado > 0)
                {
                    MessageBox.Show("✓ PERMISO INACTIVADO CORRECTAMENTE", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarCamposPermiso();
                    CargarPermisosEnTabla5();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO INACTIVAR EL PERMISO", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL INACTIVAR PERMISO: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Evento para limpiar campos
        private void Btn_Clear5_Click(object sender, EventArgs e)
        {
            LimpiarCamposPermiso();
        }
        // Evento botón buscar en Tabla5
        private void Btn_Search5_Click(object sender, EventArgs e)
        {
            string filtro = Txt_ValorBuscado5.Text.Trim();
            CargarPermisosEnTabla5(filtro);
        }
        // Evento botón limpiar búsqueda en Tabla5
        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado5.Clear();

            // Limpiar filtros
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 0;

            CargarPermisosEnTabla5();
        }
        // Evento para aplicar filtros combinados
        private void AplicarFiltros(object sender, EventArgs e)
        {
            try
            {
                Tabla5.Rows.Clear();

                // Obtener valores de filtros
                string moduloSeleccionado = Filtro1.SelectedIndex > 0 ? Filtro1.SelectedItem.ToString() : "";
                string accionSeleccionada = Filtro2.SelectedIndex > 0 ? Filtro2.SelectedItem.ToString() : "";
                string estadoSeleccionado = Filtro3.SelectedItem.ToString();

                // Obtener permisos según filtros
                List<Mdl_Permissions> permisos = Ctrl_Permissions.BuscarPermisos(
                    "", // textoBusqueda (vacío porque usamos los filtros)
                    moduloSeleccionado,
                    accionSeleccionada,
                    1,
                    1000
                );

                // Aplicar filtro de estado localmente
                if (estadoSeleccionado == "ACTIVOS")
                {
                    permisos = permisos.Where(p => p.IsActive).ToList();
                }
                else if (estadoSeleccionado == "INACTIVOS")
                {
                    permisos = permisos.Where(p => !p.IsActive).ToList();
                }

                // Llenar tabla
                foreach (var permiso in permisos)
                {
                    string estado = permiso.IsActive ? "ACTIVO" : "INACTIVO";

                    int rowIndex = Tabla5.Rows.Add(
                        permiso.PermissionId,
                        permiso.PermissionCode,
                        permiso.PermissionName,
                        permiso.ModuleName ?? "",
                        permiso.ActionType ?? "",
                        estado
                    );

                    // Colorear fila si está inactivo
                    if (!permiso.IsActive)
                    {
                        Tabla5.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                    }
                }

                // Actualizar contador
                Lbl_Paginas.Text = $"MOSTRANDO {permisos.Count} PERMISOS";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL APLICAR FILTROS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion EventosPestaña3
        #region FiltrosPestaña3
        // Cargar opciones en los ComboBox de filtros
        private void CargarFiltros()
        {
            // Filtro1 - Filtrar por Módulo
            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS LOS MÓDULOS");

            // Obtener módulos únicos desde la base de datos
            var modulos = Ctrl_Permissions.ObtenerModulos();
            foreach (var modulo in modulos)
            {
                Filtro1.Items.Add(modulo);
            }
            Filtro1.SelectedIndex = 0;

            // Filtro2 - Filtrar por Tipo de Acción
            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODAS LAS ACCIONES");

            // Obtener tipos de acción únicos desde la base de datos
            var tiposAccion = Ctrl_Permissions.ObtenerTiposAccion();
            foreach (var tipo in tiposAccion)
            {
                Filtro2.Items.Add(tipo);
            }
            Filtro2.SelectedIndex = 0;

            // Filtro3 - Filtrar por Estado
            Filtro3.Items.Clear();
            Filtro3.Items.AddRange(new object[]
            {
                "TODOS LOS ESTADOS",
                "ACTIVOS",
                "INACTIVOS"
            });
            Filtro3.SelectedIndex = 0;
        }
        #endregion FiltrosPestaña3
        #region ConfiguracionInicialPestaña4
        private int _usuarioSeleccionadoId = 0; // Variable para guardar el usuario seleccionado

        // Configuración inicial de la pestaña 4 - Permisos Adicionales
        private void ConfigurarPestaña4_GestionarPermisos()
        {
            // Configurar etiqueta informativa
            ConfigurarEtiquetaInformativaPestaña4();

            // Configurar Tabla6 (Usuarios)
            ConfigurarTabla6();

            // Configurar Tabla7 (Permisos Disponibles)
            ConfigurarTabla7Pestaña4();

            // Configurar Tabla8 (Permisos Asignados al Usuario)
            ConfigurarTabla8();

            // Cargar filtros
            CargarFiltrosPestaña4();

            // Cargar datos iniciales
            CargarUsuariosEnTabla6();
            CargarPermisosEnTabla7();

            // Asignar eventos
            AsignarEventosPestaña4();
        }

        // Configurar etiqueta informativa
        private void ConfigurarEtiquetaInformativaPestaña4()
        {
            Lbl_Info6.AutoSize = false;
            Lbl_Info6.Size = new Size(600, 40);
            Lbl_Info6.Text = "        Gestionar permisos específicos adicionales o quitar permisos específicos para usuarios. Estos permisos sobrescriben los permisos del rol.";
            Lbl_Info6.BackColor = Color.FromArgb(217, 237, 247);
            Lbl_Info6.ForeColor = Color.FromArgb(31, 45, 61);
            Lbl_Info6.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            Lbl_Info6.Padding = new Padding(10, 8, 10, 8);
            Lbl_Info6.BorderStyle = BorderStyle.FixedSingle;
        }

        // Configurar Tabla6 (Usuarios)
        private void ConfigurarTabla6()
        {
            Tabla6.Columns.Clear();

            // Columnas de datos
            Tabla6.Columns.Add("UserId", "ID");
            Tabla6.Columns.Add("Username", "USUARIO");
            Tabla6.Columns.Add("FullName", "NOMBRE COMPLETO");
            Tabla6.Columns.Add("RoleName", "ROL");

            // Ocultar columna ID
            Tabla6.Columns["UserId"].Visible = false;

            // Configuración de comportamiento
            Tabla6.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla6.MultiSelect = false;
            Tabla6.ReadOnly = true;
            Tabla6.AllowUserToAddRows = false;
            Tabla6.AllowUserToDeleteRows = false;
            Tabla6.RowHeadersVisible = false;
            Tabla6.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Estilos visuales
            Tabla6.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            Tabla6.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla6.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla6.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Tabla6.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla6.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla6.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            Tabla6.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            Tabla6.RowTemplate.Height = 35;
            Tabla6.ColumnHeadersHeight = 40;
        }

        // Configurar Tabla7 (Permisos Disponibles)
        private void ConfigurarTabla7Pestaña4()
        {
            Tabla7.Columns.Clear();

            // Columna Checkbox
            DataGridViewCheckBoxColumn colCheck = new DataGridViewCheckBoxColumn
            {
                Name = "Seleccionar",
                HeaderText = "☑",
                Width = 50,
                ReadOnly = false
            };
            Tabla7.Columns.Add(colCheck);

            // Columnas de datos
            Tabla7.Columns.Add("PermissionId", "ID");
            Tabla7.Columns.Add("PermissionCode", "CÓDIGO");
            Tabla7.Columns.Add("PermissionName", "PERMISO");
            Tabla7.Columns.Add("ModuleName", "MÓDULO");
            Tabla7.Columns.Add("ActionType", "ACCIÓN");

            // Ocultar columna ID
            Tabla7.Columns["PermissionId"].Visible = false;

            // Configuración de comportamiento
            Tabla7.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla7.MultiSelect = true;
            Tabla7.ReadOnly = false;
            Tabla7.AllowUserToAddRows = false;
            Tabla7.AllowUserToDeleteRows = false;
            Tabla7.RowHeadersVisible = false;
            Tabla7.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Estilos visuales
            Tabla7.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            Tabla7.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla7.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla7.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Tabla7.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla7.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla7.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            Tabla7.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            Tabla7.RowTemplate.Height = 35;
            Tabla7.ColumnHeadersHeight = 40;
        }

        // Configurar Tabla8 (Permisos Asignados al Usuario)
        private void ConfigurarTabla8()
        {
            Tabla8.Columns.Clear();

            // Columna Checkbox
            DataGridViewCheckBoxColumn colCheck = new DataGridViewCheckBoxColumn
            {
                Name = "Seleccionar",
                HeaderText = "☑",
                Width = 50,
                ReadOnly = false
            };
            Tabla8.Columns.Add(colCheck);

            // Columnas de datos
            Tabla8.Columns.Add("UserPermissionId", "UPID");
            Tabla8.Columns.Add("PermissionId", "ID");
            Tabla8.Columns.Add("PermissionCode", "CÓDIGO");
            Tabla8.Columns.Add("PermissionName", "PERMISO");
            Tabla8.Columns.Add("ModuleName", "MÓDULO");
            Tabla8.Columns.Add("ActionType", "ACCIÓN");

            // Ocultar columnas ID
            Tabla8.Columns["UserPermissionId"].Visible = false;
            Tabla8.Columns["PermissionId"].Visible = false;

            // Configuración de comportamiento
            Tabla8.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla8.MultiSelect = true;
            Tabla8.ReadOnly = false;
            Tabla8.AllowUserToAddRows = false;
            Tabla8.AllowUserToDeleteRows = false;
            Tabla8.RowHeadersVisible = false;
            Tabla8.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Estilos visuales
            Tabla8.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(94, 53, 177);
            Tabla8.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Tabla8.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla8.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Tabla8.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 143, 109);
            Tabla8.DefaultCellStyle.SelectionForeColor = Color.White;
            Tabla8.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            Tabla8.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            Tabla8.RowTemplate.Height = 35;
            Tabla8.ColumnHeadersHeight = 40;
        }

        // Asignar eventos
        private void AsignarEventosPestaña4()
        {
            // Eventos de selección en tablas
            Tabla6.SelectionChanged += Tabla6_SelectionChanged;
            Tabla7.CellContentClick += Tabla7_CellContentClick;
            Tabla8.CellContentClick += Tabla8_CellContentClick;

            // Eventos de búsqueda - Usuarios
            Btn_SearchUsuario.Click += Btn_SearchUsuario_Click;
            Btn_ClearUsuario.Click += Btn_ClearUsuario_Click;
            FiltroU1.SelectedIndexChanged += FiltroU1_SelectedIndexChanged;

            // Evento Enter en TextBox de busqueda Usuarios
            Txt_ValorBuscadoUsuario.KeyDown += Txt_ValorBuscadoUsuario_KeyDown;

            // Eventos de búsqueda - Permisos Disponibles
            Btn_SearchPermiso.Click += Btn_SearchPermiso_Click;
            Btn_ClearPermiso.Click += Btn_ClearPermiso_Click;
            FiltroP1.SelectedIndexChanged += AplicarFiltrosTabla7;
            FiltroP2.SelectedIndexChanged += AplicarFiltrosTabla7;
            FiltroP3.SelectedIndexChanged += AplicarFiltrosTabla7;

            // Evento Enter en TextBox de busqueda Permisos
            Txt_ValorBuscadoPermisos.KeyDown += Txt_ValorBuscadoPermisos_KeyDown;

            // Eventos de búsqueda - Permisos Asignados
            Btn_SearchPermisoAsignado.Click += Btn_SearchPermisoAsignado_Click;
            Btn_ClearPermisoAsignado.Click += Btn_ClearPermisoAsignado_Click;
            FiltroPA1.SelectedIndexChanged += AplicarFiltrosTabla8;
            FiltroPA2.SelectedIndexChanged += AplicarFiltrosTabla8;
            FiltroPA3.SelectedIndexChanged += AplicarFiltrosTabla8;

            // Evento Enter en TextBox de busqueda Permisos Asignados
            Txt_ValorBuscadoPermisosAsignados.KeyDown += Txt_ValorBuscadoPermisosAsignados_KeyDown;

            // Eventos de botones de acción
            Btn_Add4.Click += Btn_AgregarPermiso_Click;
            Btn_Remove4.Click += Btn_QuitarPermiso_Click;
        }
        #endregion ConfiguracionInicialPestaña4
        #region CargaDatosPestaña4
        // Cargar usuarios en Tabla6
        // Cargar usuarios en Tabla6
        private void CargarUsuariosEnTabla6(string filtro = "", int? roleId = null)
        {
            try
            {
                Tabla6.Rows.Clear();

                List<Mdl_Users> usuarios;

                if (!string.IsNullOrWhiteSpace(filtro) || (roleId.HasValue && roleId.Value > 0))
                {
                    // Si hay filtro de texto o roleId, usar BuscarUsuarios
                    // Ctrl_Users.BuscarUsuarios(texto, username, email, isActive, pageNumber, pageSize)
                    // Como necesitamos filtrar por rol, usaremos MostrarUsuarios y filtraremos localmente
                    usuarios = Ctrl_Users.MostrarUsuarios(1, 500);

                    // Aplicar filtros localmente
                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        filtro = filtro.ToUpper();
                        usuarios = usuarios.Where(u =>
                            (u.Username?.ToUpper().Contains(filtro) ?? false) ||
                            (u.FullName?.ToUpper().Contains(filtro) ?? false)
                        ).ToList();
                    }

                    if (roleId.HasValue && roleId.Value > 0)
                    {
                        usuarios = usuarios.Where(u => u.RoleId == roleId.Value).ToList();
                    }
                }
                else
                {
                    usuarios = Ctrl_Users.MostrarUsuarios(1, 500);
                }

                foreach (var usuario in usuarios)
                {
                    string roleName = "SIN ROL";
                    if (usuario.RoleId > 0)
                    {
                        var rol = Ctrl_Roles.ObtenerRolPorId(usuario.RoleId);
                        roleName = rol?.RoleName ?? "DESCONOCIDO";
                    }

                    Tabla6.Rows.Add(
                        usuario.UserId,
                        usuario.Username,
                        usuario.FullName,
                        roleName
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR USUARIOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cargar todos los permisos en Tabla7
        private void CargarPermisosEnTabla7(string filtro = "")
        {
            try
            {
                Tabla7.Rows.Clear();

                List<Mdl_Permissions> permisos;
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    permisos = Ctrl_Permissions.BuscarPermisos(filtro.ToUpper(), "", "", 1, 1000);
                }
                else
                {
                    permisos = Ctrl_Permissions.MostrarPermisos(1, 1000);
                }

                permisos = permisos.OrderBy(p => p.ModuleName).ThenBy(p => p.PermissionName).ToList();

                foreach (var permiso in permisos)
                {
                    Tabla7.Rows.Add(
                        false,
                        permiso.PermissionId,
                        permiso.PermissionCode,
                        permiso.PermissionName,
                        permiso.ModuleName ?? "Sin módulo",
                        permiso.ActionType ?? "Sin acción"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR PERMISOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cargar permisos asignados al usuario en Tabla8
        private void CargarPermisosAsignadosTabla8(int userId, string filtro = "")
        {
            try
            {
                Tabla8.Rows.Clear();

                var permisosDetallados = Ctrl_UserPermissions.ObtenerPermisosDetalladosPorUsuario(userId);

                // Aplicar filtro si existe
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    filtro = filtro.ToUpper();
                    permisosDetallados = permisosDetallados.Where(p =>
                        (p.PermissionCode?.ToUpper().Contains(filtro) ?? false) ||
                        (p.PermissionName?.ToUpper().Contains(filtro) ?? false) ||
                        (p.ModuleName?.ToUpper().Contains(filtro) ?? false) ||
                        (p.ActionType?.ToUpper().Contains(filtro) ?? false)
                    ).ToList();
                }

                foreach (var permiso in permisosDetallados)
                {
                    Tabla8.Rows.Add(
                        false,
                        permiso.UserPermissionId,
                        permiso.PermissionId,
                        permiso.PermissionCode,
                        permiso.PermissionName,
                        permiso.ModuleName ?? "",
                        permiso.ActionType ?? ""
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR PERMISOS ASIGNADOS: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion CargaDatosPestaña4
        #region EventosPestaña4
        // Evento cuando se selecciona un usuario en Tabla6
        private void Tabla6_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla6.SelectedRows.Count > 0)
            {
                _usuarioSeleccionadoId = Convert.ToInt32(Tabla6.SelectedRows[0].Cells["UserId"].Value);
                CargarPermisosAsignadosTabla8(_usuarioSeleccionadoId);
            }
            else
            {
                _usuarioSeleccionadoId = 0;
                Tabla8.Rows.Clear();
            }
        }

        // Eventos de checkboxes
        private void Tabla7_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Tabla7.Columns["Seleccionar"].Index && e.RowIndex >= 0)
            {
                Tabla7.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void Tabla8_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Tabla8.Columns["Seleccionar"].Index && e.RowIndex >= 0)
            {
                Tabla8.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // BÚSQUEDA USUARIOS
        private void Btn_SearchUsuario_Click(object sender, EventArgs e)
        {
            string filtro = Txt_ValorBuscadoUsuario.Text.Trim();
            int? roleId = null;

            if (FiltroU1.SelectedIndex > 0 && FiltroU1.SelectedItem is KeyValuePair<int, string> selectedRole)
            {
                roleId = selectedRole.Key;
            }

            CargarUsuariosEnTabla6(filtro, roleId);
        }

        private void Btn_ClearUsuario_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscadoUsuario.Clear();
            FiltroU1.SelectedIndex = 0;
            CargarUsuariosEnTabla6();
        }

        private void FiltroU1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Btn_SearchUsuario_Click(sender, e);
        }

        // BÚSQUEDA PERMISOS DISPONIBLES (Tabla7)
        private void Btn_SearchPermiso_Click(object sender, EventArgs e)
        {
            string filtro = Txt_ValorBuscadoPermisos.Text.Trim();
            CargarPermisosEnTabla7(filtro);
        }

        private void Btn_ClearPermiso_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscadoPermisos.Clear();
            FiltroP1.SelectedIndex = 0;
            FiltroP2.SelectedIndex = 0;
            FiltroP3.SelectedIndex = 0;
            CargarPermisosEnTabla7();
        }

        private void AplicarFiltrosTabla7(object sender, EventArgs e)
        {
            try
            {
                Tabla7.Rows.Clear();

                string moduloSeleccionado = FiltroP1.SelectedIndex > 0 ? FiltroP1.SelectedItem.ToString() : "";
                string accionSeleccionada = FiltroP2.SelectedIndex > 0 ? FiltroP2.SelectedItem.ToString() : "";
                string estadoSeleccionado = FiltroP3.SelectedItem?.ToString() ?? "TODOS LOS ESTADOS";

                List<Mdl_Permissions> permisos = Ctrl_Permissions.BuscarPermisos("", moduloSeleccionado, accionSeleccionada, 1, 1000);

                if (estadoSeleccionado == "ACTIVOS")
                {
                    permisos = permisos.Where(p => p.IsActive).ToList();
                }
                else if (estadoSeleccionado == "INACTIVOS")
                {
                    permisos = permisos.Where(p => !p.IsActive).ToList();
                }

                foreach (var permiso in permisos)
                {
                    Tabla7.Rows.Add(
                        false,
                        permiso.PermissionId,
                        permiso.PermissionCode,
                        permiso.PermissionName,
                        permiso.ModuleName ?? "",
                        permiso.ActionType ?? ""
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL APLICAR FILTROS: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // BÚSQUEDA PERMISOS ASIGNADOS (Tabla8)
        private void Btn_SearchPermisoAsignado_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionadoId == 0)
            {
                MessageBox.Show("DEBE SELECCIONAR UN USUARIO PRIMERO", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string filtro = Txt_ValorBuscadoPermisosAsignados.Text.Trim();
            CargarPermisosAsignadosTabla8(_usuarioSeleccionadoId, filtro);
        }

        private void Btn_ClearPermisoAsignado_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscadoPermisosAsignados.Clear();
            FiltroPA1.SelectedIndex = 0;
            FiltroPA2.SelectedIndex = 0;
            FiltroPA3.SelectedIndex = 0;

            if (_usuarioSeleccionadoId > 0)
            {
                CargarPermisosAsignadosTabla8(_usuarioSeleccionadoId);
            }
        }

        private void AplicarFiltrosTabla8(object sender, EventArgs e)
        {
            if (_usuarioSeleccionadoId == 0) return;

            try
            {
                Tabla8.Rows.Clear();

                string moduloSeleccionado = FiltroPA1.SelectedIndex > 0 ? FiltroPA1.SelectedItem.ToString() : "";
                string accionSeleccionada = FiltroPA2.SelectedIndex > 0 ? FiltroPA2.SelectedItem.ToString() : "";

                var permisosDetallados = Ctrl_UserPermissions.ObtenerPermisosDetalladosPorUsuario(_usuarioSeleccionadoId);

                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(moduloSeleccionado))
                {
                    permisosDetallados = permisosDetallados.Where(p => p.ModuleName == moduloSeleccionado).ToList();
                }

                if (!string.IsNullOrWhiteSpace(accionSeleccionada))
                {
                    permisosDetallados = permisosDetallados.Where(p => p.ActionType == accionSeleccionada).ToList();
                }

                foreach (var permiso in permisosDetallados)
                {
                    Tabla8.Rows.Add(
                        false,
                        permiso.UserPermissionId,
                        permiso.PermissionId,
                        permiso.PermissionCode,
                        permiso.PermissionName,
                        permiso.ModuleName ?? "",
                        permiso.ActionType ?? ""
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL APLICAR FILTROS: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // AGREGAR PERMISOS AL USUARIO
        private void Btn_AgregarPermiso_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioSeleccionadoId == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN USUARIO PRIMERO", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<int> permisosSeleccionados = new List<int>();
                foreach (DataGridViewRow row in Tabla7.Rows)
                {
                    if (row.Cells["Seleccionar"].Value != null && (bool)row.Cells["Seleccionar"].Value == true)
                    {
                        permisosSeleccionados.Add(Convert.ToInt32(row.Cells["PermissionId"].Value));
                    }
                }

                if (permisosSeleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN PERMISO", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var usuario = Ctrl_Users.ObtenerUsuarioPorId(_usuarioSeleccionadoId);
                var confirmacion = MessageBox.Show(
                    $"¿DESEA AGREGAR {permisosSeleccionados.Count} PERMISO(S) ESPECÍFICO(S) AL USUARIO '{usuario.FullName}'?\n\n" +
                    "Estos permisos SOBRESCRIBIRÁN los permisos del rol para este usuario.",
                    "CONFIRMAR ASIGNACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No) return;

                this.Cursor = Cursors.WaitCursor;

                int exitosos = 0;
                foreach (int permissionId in permisosSeleccionados)
                {
                    if (!Ctrl_UserPermissions.ExisteAsignacion(_usuarioSeleccionadoId, permissionId))
                    {
                        Mdl_UserPermissions up = new Mdl_UserPermissions
                        {
                            UserId = _usuarioSeleccionadoId,
                            PermissionId = permissionId,
                            IsGranted = true,
                            GrantedBy = UserData?.UserId ?? 1
                        };

                        if (Ctrl_UserPermissions.AsignarPermisoAUsuario(up) > 0)
                        {
                            exitosos++;
                        }
                    }
                }

                this.Cursor = Cursors.Default;

                MessageBox.Show($"✓ {exitosos} PERMISO(S) ASIGNADO(S) CORRECTAMENTE", "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarPermisosAsignadosTabla8(_usuarioSeleccionadoId);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL AGREGAR PERMISOS: {ex.Message}", "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // QUITAR PERMISOS DEL USUARIO
        private void Btn_QuitarPermiso_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioSeleccionadoId == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN USUARIO PRIMERO", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<int> permisosSeleccionados = new List<int>();
                foreach (DataGridViewRow row in Tabla8.Rows)
                {
                    if (row.Cells["Seleccionar"].Value != null && (bool)row.Cells["Seleccionar"].Value == true)
                    {
                        permisosSeleccionados.Add(Convert.ToInt32(row.Cells["UserPermissionId"].Value));
                    }
                }

                if (permisosSeleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN PERMISO", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO DE QUITAR {permisosSeleccionados.Count} PERMISO(S) ESPECÍFICO(S) DEL USUARIO?",
                    "CONFIRMAR",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No) return;

                this.Cursor = Cursors.WaitCursor;

                int exitosos = 0;
                foreach (int userPermissionId in permisosSeleccionados)
                {
                    if (Ctrl_UserPermissions.EliminarPermisoDeUsuario(userPermissionId) > 0)
                    {
                        exitosos++;
                    }
                }

                this.Cursor = Cursors.Default;

                MessageBox.Show($"✓ {exitosos} PERMISO(S) REMOVIDO(S) CORRECTAMENTE", "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarPermisosAsignadosTabla8(_usuarioSeleccionadoId);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL QUITAR PERMISOS: {ex.Message}", "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion EventosPestaña4
        #region CargarFiltrosPestaña4
        private void CargarFiltrosPestaña4()
        {
            // FiltroU1 - Filtrar usuarios por rol
            FiltroU1.Items.Clear();
            FiltroU1.DisplayMember = "Value";
            FiltroU1.ValueMember = "Key";
            FiltroU1.DropDownStyle = ComboBoxStyle.DropDownList; // ← AGREGAR ESTA LÍNEA

            var rolesLista = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, "TODOS LOS ROLES")
            };
            rolesLista.AddRange(Ctrl_Roles.ObtenerTodosLosRoles());

            FiltroU1.DataSource = rolesLista;
            FiltroU1.SelectedIndex = 0;

            // FiltroP1, FiltroP2, FiltroP3 - Filtros para Tabla7 (Permisos Disponibles)
            FiltroP1.Items.Clear();
            FiltroP1.DropDownStyle = ComboBoxStyle.DropDownList; // ← AGREGAR ESTA LÍNEA
            FiltroP1.Items.Add("TODOS LOS MÓDULOS");
            var modulos = Ctrl_Permissions.ObtenerModulos();
            foreach (var modulo in modulos)
            {
                FiltroP1.Items.Add(modulo);
            }
            FiltroP1.SelectedIndex = 0;

            FiltroP2.Items.Clear();
            FiltroP2.DropDownStyle = ComboBoxStyle.DropDownList; // ← AGREGAR ESTA LÍNEA
            FiltroP2.Items.Add("TODAS LAS ACCIONES");
            var tiposAccion = Ctrl_Permissions.ObtenerTiposAccion();
            foreach (var tipo in tiposAccion)
            {
                FiltroP2.Items.Add(tipo);
            }
            FiltroP2.SelectedIndex = 0;

            FiltroP3.Items.Clear();
            FiltroP3.DropDownStyle = ComboBoxStyle.DropDownList; // ← AGREGAR ESTA LÍNEA
            FiltroP3.Items.AddRange(new object[] { "TODOS LOS ESTADOS", "ACTIVOS", "INACTIVOS" });
            FiltroP3.SelectedIndex = 0;

            // FiltroPA1, FiltroPA2, FiltroPA3 - Filtros para Tabla8 (Permisos Asignados)
            FiltroPA1.Items.Clear();
            FiltroPA1.DropDownStyle = ComboBoxStyle.DropDownList; // ← AGREGAR ESTA LÍNEA
            FiltroPA1.Items.Add("TODOS LOS MÓDULOS");
            foreach (var modulo in modulos)
            {
                FiltroPA1.Items.Add(modulo);
            }
            FiltroPA1.SelectedIndex = 0;

            FiltroPA2.Items.Clear();
            FiltroPA2.DropDownStyle = ComboBoxStyle.DropDownList; // ← AGREGAR ESTA LÍNEA
            FiltroPA2.Items.Add("TODAS LAS ACCIONES");
            foreach (var tipo in tiposAccion)
            {
                FiltroPA2.Items.Add(tipo);
            }
            FiltroPA2.SelectedIndex = 0;

            // FiltroPA3 - Opcional (deshabilitado porque todos los permisos asignados son activos)
            FiltroPA3.Items.Clear();
            FiltroPA3.DropDownStyle = ComboBoxStyle.DropDownList; // ← AGREGAR ESTA LÍNEA
            FiltroPA3.Items.Add("TODOS");
            FiltroPA3.SelectedIndex = 0;
            FiltroPA3.Enabled = false;
        }
        #endregion CargarFiltrosPestaña4
        #region EventosEnterTextBoxBusqueda
        // Metodo para ejecutar busqueda al presionar Enter en Txt_ValorBuscado (Pestaña 1)
        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evitar sonido de beep
                Btn_Search_Click(sender, e);
            }
        }

        // Metodo para ejecutar busqueda al presionar Enter en Txt_ValorBuscado2 (Pestaña 2)
        private void Txt_ValorBuscado2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search2_Click(sender, e);
            }
        }

        // Metodo para ejecutar busqueda al presionar Enter en Txt_ValorBuscado3 (Pestaña 2)
        private void Txt_ValorBuscado3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search3_Click(sender, e);
            }
        }

        // Metodo para ejecutar busqueda al presionar Enter en Txt_ValorBuscado5 (Pestaña 3)
        private void Txt_ValorBuscado5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search5_Click(sender, e);
            }
        }

        // Metodo para ejecutar busqueda al presionar Enter en Txt_ValorBuscadoUsuario (Pestaña 4)
        private void Txt_ValorBuscadoUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_SearchUsuario_Click(sender, e);
            }
        }

        // Metodo para ejecutar busqueda al presionar Enter en Txt_ValorBuscadoPermisos (Pestaña 4)
        private void Txt_ValorBuscadoPermisos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_SearchPermiso_Click(sender, e);
            }
        }

        // Metodo para ejecutar busqueda al presionar Enter en Txt_ValorBuscadoPermisosAsignados (Pestaña 4)
        private void Txt_ValorBuscadoPermisosAsignados_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_SearchPermisoAsignado_Click(sender, e);
            }
        }
        #endregion EventosEnterTextBoxBusqueda
        #region ConfigurarTextBox
        private void ConfigurarMaxLengthTextBox()
        {
            Txt_Description.MaxLength = 200;
            Txt_ModuleName.MaxLength = 100;
            Txt_PermissionCode.MaxLength = 25;
            Txt_PermissionName.MaxLength = 100;
            Txt_ValorBuscado.MaxLength = 150;
            Txt_ValorBuscado2.MaxLength = 150;
            Txt_ValorBuscado3.MaxLength = 150;
            Txt_ValorBuscado5.MaxLength = 150;
            Txt_ValorBuscadoPermisos.MaxLength = 150;
            Txt_ValorBuscadoPermisosAsignados.MaxLength = 150;
            Txt_ValorBuscadoUsuario.MaxLength = 150;
        }
        #endregion ConfigurarTextBox
    }
}