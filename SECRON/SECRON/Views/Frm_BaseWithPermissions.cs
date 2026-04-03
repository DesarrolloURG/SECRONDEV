using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public class Frm_BaseWithPermissions : Form
    {
        protected Ctrl_Security_Auth authController = new Ctrl_Security_Auth();
        protected HashSet<string> permisosUsuario = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public Mdl_Security_UserInfo UserData { get; set; }

        protected async Task InicializarPermisosAsync()
        {
            if (UserData == null)
                return;

            await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
            ConfigurarControlesPorPermisos();
        }

        protected virtual async Task CargarPermisosUsuario(int userId, int roleId)
        {
            try
            {
                var permisos = await authController.ObtenerPermisosUsuarioAsync(userId, roleId);

                permisosUsuario = permisos != null
                    ? new HashSet<string>(permisos, StringComparer.OrdinalIgnoreCase)
                    : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                permisosUsuario = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                MessageBox.Show(
                    $"ERROR AL CARGAR PERMISOS: {ex.Message}",
                    "ERROR SECRON",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
            if (boton == null)
                return;

            bool habilitado = TienePermiso(permissionCode);

            boton.Enabled = habilitado;

            if (!habilitado)
            {
                boton.BackColor = Color.FromArgb(200, 200, 200);
                boton.ForeColor = Color.Gray;
                boton.Cursor = Cursors.No;
            }
        }

        protected virtual void ConfigurarControlesPorPermisos()
        {
        }
    }
}