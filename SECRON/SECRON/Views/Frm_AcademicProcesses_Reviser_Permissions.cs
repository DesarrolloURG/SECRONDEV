using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_Reviser_Permissions : Form
    {
        private const string MODULE_PREFIX = "ACADEMICPROCESSES";
        private const string ROL_REVISOR_HORARIOS = "REVISOR DE HORARIOS";

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_AcademicProcesses_ReviserPermission> _todosLosPermisos = new List<Mdl_AcademicProcesses_ReviserPermission>();
        private int _roleId = 0;

        public Frm_AcademicProcesses_Reviser_Permissions()
        {
            InitializeComponent();

            this.Size = new Size(1200, 700);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.Load -= Frm_AcademicProcesses_Reviser_Permissions_Load;
            this.Load += Frm_AcademicProcesses_Reviser_Permissions_Load;
        }

        private void Frm_AcademicProcesses_Reviser_Permissions_Load(object sender, EventArgs e)
        {
            var rol = Ctrl_Roles.ObtenerRolPorNombre(ROL_REVISOR_HORARIOS);
            if (rol == null)
            {
                MessageBox.Show($"NO SE ENCONTRÓ EL ROL '{ROL_REVISOR_HORARIOS}' EN EL SISTEMA.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            _roleId = rol.RoleId;

            ConfigurarTablas();
            ConfigurarFiltros();
            ConfigurarEventos();
            CargarPermisos();
        }

        #region ConfigurarTablas
        private void ConfigurarTablas()
        {
            ConfigurarTabla(TablaPermisosRol, incluirRolePermissionId: true);
            ConfigurarTabla(TablaPermisosGenerales, incluirRolePermissionId: false);
        }

        private void ConfigurarTabla(DataGridView tabla, bool incluirRolePermissionId)
        {
            tabla.Columns.Clear();

            DataGridViewCheckBoxColumn colCheck = new DataGridViewCheckBoxColumn
            {
                Name = "Seleccionar",
                HeaderText = "☑",
                Width = 50,
                ReadOnly = false
            };
            tabla.Columns.Add(colCheck);

            if (incluirRolePermissionId)
                tabla.Columns.Add("RolePermissionId", "RPID");

            tabla.Columns.Add("PermissionId", "ID");
            tabla.Columns.Add("PermissionCode", "CÓDIGO");
            tabla.Columns.Add("PermissionName", "PERMISO");
            tabla.Columns.Add("ModuleName", "MÓDULO");
            tabla.Columns.Add("ActionType", "ACCIÓN");

            foreach (DataGridViewColumn col in tabla.Columns)
            {
                if (col.Name != "Seleccionar")
                    col.ReadOnly = true;
            }

            tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tabla.MultiSelect = true;
            tabla.AllowUserToAddRows = false;
            tabla.RowHeadersVisible = false;

            if (incluirRolePermissionId)
                tabla.Columns["RolePermissionId"].Visible = false;

            tabla.Columns["PermissionId"].Visible = false;

            tabla.Columns["PermissionCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tabla.Columns["PermissionCode"].Width = 90;

            tabla.Columns["PermissionName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tabla.Columns["PermissionName"].FillWeight = 50;

            tabla.Columns["ModuleName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tabla.Columns["ModuleName"].FillWeight = 30;

            tabla.Columns["ActionType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tabla.Columns["ActionType"].Width = 100;

            tabla.CellContentClick -= Tabla_CellContentClick;
            tabla.CellContentClick += Tabla_CellContentClick;
        }

        private void Tabla_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView tabla = (DataGridView)sender;
            if (e.RowIndex >= 0 && e.ColumnIndex == tabla.Columns["Seleccionar"].Index)
            {
                tabla.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        #endregion ConfigurarTablas

        #region ConfigurarFiltros
        private void ConfigurarFiltros()
        {
            FiltroRol.DropDownStyle = ComboBoxStyle.DropDownList;
            FiltroGeneral.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LlenarFiltroAcciones(ComboBox combo, List<Mdl_AcademicProcesses_ReviserPermission> permisos)
        {
            string seleccionActual = combo.SelectedItem?.ToString() ?? "TODOS";

            combo.Items.Clear();
            combo.Items.Add("TODOS");

            foreach (var accion in permisos.Select(p => p.ActionType).Distinct().OrderBy(a => a))
            {
                if (!string.IsNullOrWhiteSpace(accion))
                    combo.Items.Add(accion);
            }

            int idx = combo.Items.IndexOf(seleccionActual);
            combo.SelectedIndex = idx >= 0 ? idx : 0;
        }
        #endregion ConfigurarFiltros

        #region ConfigurarEventos
        private void ConfigurarEventos()
        {
            Btn_SearchPermisoRol.Click -= Btn_SearchPermisoRol_Click;
            Btn_SearchPermisoRol.Click += Btn_SearchPermisoRol_Click;

            Btn_ClearPermisoRol.Click -= Btn_ClearPermisoRol_Click;
            Btn_ClearPermisoRol.Click += Btn_ClearPermisoRol_Click;

            FiltroRol.SelectedIndexChanged -= Btn_SearchPermisoRol_Click;
            FiltroRol.SelectedIndexChanged += Btn_SearchPermisoRol_Click;

            Txt_ValorBuscadoPermisosAsignados.KeyDown -= TxtBuscarRol_KeyDown;
            Txt_ValorBuscadoPermisosAsignados.KeyDown += TxtBuscarRol_KeyDown;

            Btn_SearchPermisosGeneral.Click -= Btn_SearchPermisosGeneral_Click;
            Btn_SearchPermisosGeneral.Click += Btn_SearchPermisosGeneral_Click;

            Btn_ClearPermisoGeneral.Click -= Btn_ClearPermisoGeneral_Click;
            Btn_ClearPermisoGeneral.Click += Btn_ClearPermisoGeneral_Click;

            FiltroGeneral.SelectedIndexChanged -= Btn_SearchPermisosGeneral_Click;
            FiltroGeneral.SelectedIndexChanged += Btn_SearchPermisosGeneral_Click;

            Txt_ValorBuscadoPermisosGenerales.KeyDown -= TxtBuscarGeneral_KeyDown;
            Txt_ValorBuscadoPermisosGenerales.KeyDown += TxtBuscarGeneral_KeyDown;

            Btn_Add.Click -= Btn_Add_Click;
            Btn_Add.Click += Btn_Add_Click;

            Btn_Remove.Click -= Btn_Remove_Click;
            Btn_Remove.Click += Btn_Remove_Click;
        }

        private void TxtBuscarRol_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_SearchPermisoRol_Click(sender, e);
            }
        }

        private void TxtBuscarGeneral_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_SearchPermisosGeneral_Click(sender, e);
            }
        }

        private void Btn_SearchPermisoRol_Click(object sender, EventArgs e)
        {
            MostrarPermisosEnTabla(TablaPermisosRol, filtrarAsignados: true);
        }

        private void Btn_ClearPermisoRol_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscadoPermisosAsignados.Clear();
            if (FiltroRol.Items.Count > 0) FiltroRol.SelectedIndex = 0;
            MostrarPermisosEnTabla(TablaPermisosRol, filtrarAsignados: true);
        }

        private void Btn_SearchPermisosGeneral_Click(object sender, EventArgs e)
        {
            MostrarPermisosEnTabla(TablaPermisosGenerales, filtrarAsignados: false);
        }

        private void Btn_ClearPermisoGeneral_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscadoPermisosGenerales.Clear();
            if (FiltroGeneral.Items.Count > 0) FiltroGeneral.SelectedIndex = 0;
            MostrarPermisosEnTabla(TablaPermisosGenerales, filtrarAsignados: false);
        }
        #endregion ConfigurarEventos

        #region CargarDatos
        private void CargarPermisos()
        {
            try
            {
                _todosLosPermisos = Ctrl_AcademicProcesses_ReviserPermissions.BuscarPermisosModulo(MODULE_PREFIX, _roleId);

                LlenarFiltroAcciones(FiltroRol, _todosLosPermisos);
                LlenarFiltroAcciones(FiltroGeneral, _todosLosPermisos);

                MostrarPermisosEnTabla(TablaPermisosRol, filtrarAsignados: true);
                MostrarPermisosEnTabla(TablaPermisosGenerales, filtrarAsignados: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CARGAR PERMISOS: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarPermisosEnTabla(DataGridView tabla, bool filtrarAsignados)
        {
            tabla.Rows.Clear();

            string texto = (filtrarAsignados ? Txt_ValorBuscadoPermisosAsignados.Text : Txt_ValorBuscadoPermisosGenerales.Text)
                .Trim().ToUpper();
            string accion = (filtrarAsignados ? FiltroRol.SelectedItem?.ToString() : FiltroGeneral.SelectedItem?.ToString()) ?? "TODOS";

            var permisos = _todosLosPermisos.Where(p => p.EstaAsignado == filtrarAsignados);

            if (!string.IsNullOrEmpty(texto))
                permisos = permisos.Where(p =>
                    (p.PermissionCode?.ToUpper().Contains(texto) ?? false) ||
                    (p.PermissionName?.ToUpper().Contains(texto) ?? false));

            if (accion != "TODOS")
                permisos = permisos.Where(p => p.ActionType == accion);

            foreach (var permiso in permisos)
            {
                if (filtrarAsignados)
                {
                    tabla.Rows.Add(false, permiso.RolePermissionId, permiso.PermissionId,
                        permiso.PermissionCode, permiso.PermissionName, permiso.ModuleName, permiso.ActionType);
                }
                else
                {
                    tabla.Rows.Add(false, permiso.PermissionId,
                        permiso.PermissionCode, permiso.PermissionName, permiso.ModuleName, permiso.ActionType);
                }
            }
        }
        #endregion CargarDatos

        #region Acciones
        private void Btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                var seleccionados = TablaPermisosGenerales.Rows.Cast<DataGridViewRow>()
                    .Where(r => r.Cells["Seleccionar"].Value != null && (bool)r.Cells["Seleccionar"].Value)
                    .Select(r => Convert.ToInt32(r.Cells["PermissionId"].Value))
                    .ToList();

                if (seleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN PERMISO.", "SECRON",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿DESEA ASIGNAR {seleccionados.Count} PERMISO(S) AL ROL '{ROL_REVISOR_HORARIOS}'?",
                    "CONFIRMAR ASIGNACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                int exitosos = 0;

                foreach (int permissionId in seleccionados)
                {
                    if (!Ctrl_RolePermissions.ExisteAsignacion(_roleId, permissionId))
                    {
                        Mdl_RolePermissions rp = new Mdl_RolePermissions
                        {
                            RoleId = _roleId,
                            PermissionId = permissionId,
                            IsGranted = true,
                            CreatedBy = UserData.UserId
                        };

                        if (Ctrl_RolePermissions.AsignarPermisoARol(rp) > 0)
                            exitosos++;
                    }
                    else
                    {
                        var existente = Ctrl_RolePermissions.ObtenerPermisosPorRol(_roleId)
                            .FirstOrDefault(rp => rp.PermissionId == permissionId);

                        if (existente != null && !existente.IsGranted)
                        {
                            if (Ctrl_RolePermissions.ActualizarEstadoPermiso(existente.RolePermissionId, true, UserData.UserId) > 0)
                                exitosos++;
                        }
                    }
                }

                MessageBox.Show($"PERMISOS ASIGNADOS: {exitosos}", "RESULTADO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarPermisos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ASIGNAR PERMISOS: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            try
            {
                var seleccionados = TablaPermisosRol.Rows.Cast<DataGridViewRow>()
                    .Where(r => r.Cells["Seleccionar"].Value != null && (bool)r.Cells["Seleccionar"].Value)
                    .Select(r => Convert.ToInt32(r.Cells["RolePermissionId"].Value))
                    .ToList();

                if (seleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UN PERMISO.", "SECRON",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿DESEA QUITAR {seleccionados.Count} PERMISO(S) DEL ROL '{ROL_REVISOR_HORARIOS}'?",
                    "CONFIRMAR REMOCIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                int exitosos = 0;

                foreach (int rolePermissionId in seleccionados)
                {
                    if (Ctrl_RolePermissions.ActualizarEstadoPermiso(rolePermissionId, false, UserData.UserId) > 0)
                        exitosos++;
                }

                MessageBox.Show($"PERMISOS REMOVIDOS: {exitosos}", "RESULTADO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarPermisos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL QUITAR PERMISOS: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Acciones
    }
}