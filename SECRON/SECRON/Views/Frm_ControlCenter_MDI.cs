using SECRON.Configuration;
using SECRON.Controllers;
using SECRON.Models;
using SECRON.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SECRON.Utils;

namespace SECRON.Views
{
    public partial class Frm_ControlCenter_MDI : Form
    {
        #region PropiedadesRecibidas
        //Propiedades del formulario previo
        public string Usuario { get; set; }
        public Mdl_Security_UserInfo UserData { get; set; }
        #endregion PropiedadesRecibidas
        #region PropiedadesIniciales
        //Propiedades iniciales
        private Ctrl_Security_Auth authController;
        private System.Windows.Forms.Timer timerInactividad;
        private DateTime ultimaActividad;
        private int tiempoSesionActivaMinutos = 15;
        private Cls_ActivityMessageFilter activityFilter;
        private bool sesionCerradaPorInactividad = false;

        // Importaciones de la API de Windows para personalizar la barra de título
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        // Constantes para los atributos de DWM
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;
        // Configurar Vista Inicial del MDI
        private void ConfigurarVistaInicialMDI()
        {
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_Home formulario = new Frm_Home();
            formulario.Text = "Inicio";
            formulario.BackColor = Color.White;

            AbrirFormularioConPestana(formulario, "Inicio", "Home");
        }
        // Configura la apariencia de la barra de título
        private void ConfigurarBarraTitulo()
        {
            // Solo botón cerrar, sin título visible
            //this.Text = "";  // Ocultar el texto del título
            this.MaximizeBox = true;  // Mantener botón maximizar
            this.MinimizeBox = true;  // Mantener botón minimizar
            this.ControlBox = true;    // Mantener el botón cerrar (X)
        }
        // Configura los botones con enlaces desde el Load del formulario
        private void ConfigurarBotonesEnlace()
        {
            // Boton Configurado
            // ConfigurarBotonEnlace(Btn_ForgetPassword);
        }
        // Método para configurar un botón como enlace
        private void ConfigurarBotonEnlace(Button boton)
        {
            // Configuración básica del botón
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 255, 255); // Mismo color del fondo
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 255, 255); // Mismo color del fondo
            boton.BackColor = Color.FromArgb(255, 255, 255); // Mismo color del fondo
            boton.ForeColor = Color.FromArgb(9, 184, 255); // Color azul inicial
            boton.TextAlign = ContentAlignment.MiddleLeft; // Alineado a la izquierda
            boton.Cursor = Cursors.Default;

            // CLAVE: Eliminar TODOS los bordes posibles
            boton.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255);
            boton.FlatAppearance.CheckedBackColor = Color.FromArgb(255, 255, 255);
        }
        // Configura los bontones con imagen desde el Load del formulario
        private void ConfigurarBotonesImagenesSinFondo()
        {
            // Boton Configurado
            //ConfigurarBotonImagenSinFondo(Btn_Visible);
        }
        // Método para configurar un botón imagen sin fondo
        private void ConfigurarBotonImagenSinFondo(Button boton)
        {
            // Configuración básica del botón
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 255, 255); // Mismo color del fondo
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 255, 255); // Mismo color del fondo
            boton.BackColor = Color.FromArgb(255, 255, 255); // Color del fondo del formulario
            boton.Cursor = Cursors.Default;

            // CLAVE: Eliminar TODOS los bordes posibles
            boton.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255);
            boton.FlatAppearance.CheckedBackColor = Color.FromArgb(255, 255, 255);
        }
        // Establecer orden de los Tabuladores
        private void ConfigurarOrdenTabulacion()
        {
            // Orden de tabulaciones
            //TxtUser.TabIndex = 0;
            //Btn_Visible.TabIndex = 1;
        }
        private void ConfigurarEstiloBotones()
        {
            // Configurar la apariencia de los botones con padding real
            // ConfigurarEstiloBoton(Btn_Next);
        }
        private void ConfigurarEstiloBoton(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(9, 184, 255); // Azul moderno
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            boton.Height = 45;
            boton.Width = Math.Max(boton.Width, 140);
            boton.Cursor = Cursors.Default;
            boton.TextAlign = ContentAlignment.MiddleCenter;

            // Esquinas redondeadas usando región personalizada
            boton.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, boton.Width, boton.Height, 20, 20));
        }
        //Confiugrar todos los botones de Navegacion
        private void ConfigurarBotonesNavegacion()
        {
            // Panel de Navegación Menú
            ConfigurarBotonNavegacion(BtnHome);
            ConfigurarBotonNavegacion(BtnRRHH);
            ConfigurarBotonNavegacion(BtnUsers);
            ConfigurarBotonNavegacion(BtnFinances);
            ConfigurarBotonNavegacion(BtnOrders);
            ConfigurarBotonNavegacion(Btn_Inventory);
            ConfigurarBotonNavegacion(BtnBills);
            ConfigurarBotonNavegacion(BtnLocations);
            ConfigurarBotonNavegacion(BtnProcesosAcademicos);
        }
        // Configurar Botones de Menu de Navegacion
        private void ConfigurarBotonNavegacion(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(255, 255, 255); // Blanco como el Fondo
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(238, 143, 109); // Color naranja
            boton.FlatAppearance.MouseDownBackColor = Color.FromArgb(222, 224, 222); // Gris para submenu
            boton.ForeColor = Color.Black;
            boton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            boton.Height = 40;
            boton.Width = Math.Max(boton.Width, 200);
            boton.Cursor = Cursors.Default;
            boton.TextAlign = ContentAlignment.MiddleLeft;
            boton.Padding = new Padding(5, 0, 0, 0); // 15px de margen izquierdo
            boton.Text = "        " + boton.Text;
        }
        // Configurar botones de Submenus de Navegacion
        private void ConfigurarSubmenuNavegacion()
        {
            // Panel de Navegación Submenú Recursos Humanos
            ConfigurarBotonSubmenuNavegacion(BtnRRHH_Trabajadores);
            ConfigurarBotonSubmenuNavegacion(BtnRRHH_Docencia);
            ConfigurarBotonSubmenuNavegacion(BtnRRHH_Calendario);
            ConfigurarBotonSubmenuNavegacion(BtnRRHH_Planillas);
            ConfigurarBotonSubmenuNavegacion(BtnRRHH_MyProfile);
            // Panel de Navegación Submenú Recursos Humanos - Trabajadores
            ConfigurarBotonSubmenuNavegacion(BtnRRHH_Trabajadores_Ficha);
            // Panel de Navegación Submenú Recursos Humanos - Docencia
            ConfigurarBotonSubmenuNavegacion(BtnRRHH_Docencia_FichaDocente);
            ConfigurarBotonSubmenuNavegacion(BtnRRHH_Docencia_FichaCoordinador);
            ConfigurarBotonSubmenuNavegacion(BtnRRHH_Docencia_ControlAsistencias);
            // Panel de Navegación Submenú Usuarios
            ConfigurarBotonSubmenuNavegacion(BtnUsersManagment);
            ConfigurarBotonSubmenuNavegacion(BtnUsersRolesPermisos);
            ConfigurarBotonSubmenuNavegacion(Btn_ITSM_Technology);
            // Panel de Navegación Submenú Finanzas
            ConfigurarBotonSubmenuNavegacion(BtnFinances_Accounts);
            ConfigurarBotonSubmenuNavegacion(BtnFinances_Checks);
            ConfigurarBotonSubmenuNavegacion(BtnFinances_Transfers);
            ConfigurarBotonSubmenuNavegacion(BtnFinances_Banks);
            ConfigurarBotonSubmenuNavegacion(BtnFinances_AccountingBooks);
            // Panel de Navegación Submenú Compras (Proveedores)
            ConfigurarBotonSubmenuNavegacion(BtnSuppliers);
            // Panel de Navegación Submenú Cuentas
            ConfigurarBotonSubmenuNavegacion(BtnCountsManagment);
            // Panel de Navegación Submenú Compras
            ConfigurarBotonSubmenuNavegacion(BtnOrdersManagment);
            ConfigurarBotonSubmenuNavegacion(BtnOrdersRequisicionManagment);
            ConfigurarBotonSubmenuNavegacion(BtnOrdersSolicitud);
            // Panel de Navegación Submenú Inventario (opciones principales)
            ConfigurarBotonSubmenuNavegacion(BtnInvKardex);
            ConfigurarBotonSubmenuNavegacion(BtnInvStaticItems);
            ConfigurarBotonSubmenuNavegacion(BtnInvWarehouse);
            // Panel de Navegación Submenú Gastos
            ConfigurarBotonSubmenuNavegacion(BtnBillsAuthorization);
            ConfigurarBotonSubmenuNavegacion(BtnBillsBudgetControl);
            ConfigurarBotonSubmenuNavegacion(BtnBillsManagment);
            ConfigurarBotonSubmenuNavegacion(BtnBillsCategorias);
            ConfigurarBotonSubmenuNavegacion(BtnBillsReports);
            ConfigurarBotonSubmenuNavegacion(BtnBillsCatalog);
            // Panel de Navegación Submenú Cheques
            ConfigurarBotonSubmenuNavegacion(BtnChecksManagment);
            ConfigurarBotonSubmenuNavegacion(BtnChecksReports);
            ConfigurarBotonSubmenuNavegacion(BtnChecksConfiguration);
            ConfigurarBotonSubmenuNavegacion(BtnChecksFileControl);
            // Panel de Navegación Submenú Bancos
            ConfigurarBotonSubmenuNavegacion(BtnBanksAccounts);
            ConfigurarBotonSubmenuNavegacion(BtnBanksCashControl);
            ConfigurarBotonSubmenuNavegacion(BtnBanksConciliacion);
            ConfigurarBotonSubmenuNavegacion(BtnBanksConfiguration);
            ConfigurarBotonSubmenuNavegacion(BtnBanksMovements);
            ConfigurarBotonSubmenuNavegacion(BtnBanksReports);
            // Panel de Navegación Submenú Libros Contables
            ConfigurarBotonSubmenuNavegacion(BtnAccountingBooksDiario);
            ConfigurarBotonSubmenuNavegacion(BtnAccountingBooksMayor);
            ConfigurarBotonSubmenuNavegacion(BtnAccountingBooksFinancialStatemants);
            ConfigurarBotonSubmenuNavegacion(BtnAccountingBooksClosing);
            ConfigurarBotonSubmenuNavegacion(BtnAccountingBooksReports);
            // Panel de Navegación Submenú Sedes
            ConfigurarBotonSubmenuNavegacion(BtnLocationsManagment);
            ConfigurarBotonSubmenuNavegacion(BtnLocationsConfiguration);
            ConfigurarBotonSubmenuNavegacion(BtnLocationsDepartments);
            ConfigurarBotonSubmenuNavegacion(BtnLocationsFinancialControl);
            ConfigurarBotonSubmenuNavegacion(BtnLocationsStaff);
            // Panel de Navegación Submenú Activos Fijos
            ConfigurarBotonSubmenuNavegacion(BtnStaticItemsManagment);
            ConfigurarBotonSubmenuNavegacion(BtnStaticItemsCategories);
            ConfigurarBotonSubmenuNavegacion(BtnStaticItemsMaintenance);
            ConfigurarBotonSubmenuNavegacion(BtnStaticItemsMovementsController);
            ConfigurarBotonSubmenuNavegacion(BtnStaticItemsReports);
            ConfigurarBotonSubmenuNavegacion(BtnStaticItemsResponsabilityLetter);
            // Panel de Navegación Submenú Almacenes/Warehouse
            ConfigurarBotonSubmenuNavegacion(BtnWarehouse_Managment);
            ConfigurarBotonSubmenuNavegacion(BtnWarehouse_Reports);
            // Panel de Navegación Submenú Teachers
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_PensumCarreras);
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_PensumCursos);
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_TarifasCursos);
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_RevisoresAprobados);
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_AprobacionDocentes);
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_PreasignacionCursos);
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_RevisionAsignaciones);
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_HorariosOficiales);
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_CalendariosAcademicos);
            // Panel de Navegación Submenú Teachers - Opciones
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_PensumCarreras);
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_PensumCursos);
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_TarifasCursos);
            ConfigurarBotonSubmenuNavegacion(BtnProcesosAcademicos_RevisoresAprobados);
            // Panel de Navegación Submenú Transferencias
            ConfigurarBotonSubmenuNavegacion(BtnTransfersManagment);
            ConfigurarBotonSubmenuNavegacion(BtnTransfersReports);
            // Panel de Navegación Submenú Recursos Humanos
            ConfigurarBotonSubmenuNavegacion(BtnTransfersManagment);
        }
        // Configurar botones de SubSubmenus de Navegacion
        private void ConfigurarSubSubmenuNavegacion()
        {
            // Panel de Navegación SubSubmenú KARDEX - Gestión de Artículos
            ConfigurarBotonSubSubmenuNavegacion(BtnKardex_CatalogLocationsCategories);
            ConfigurarBotonSubSubmenuNavegacion(BtnKardex_ItemsManagment);
            ConfigurarBotonSubSubmenuNavegacion(BtnKardexInventory);
            ConfigurarBotonSubSubmenuNavegacion(BtnKardexInputControl);
            ConfigurarBotonSubSubmenuNavegacion(BtnKardexInventoryReport);

            // Panel de Navegación SubSubmenú ACTIVOS FIJOS
            ConfigurarBotonSubSubmenuNavegacion(BtnStaticItemsManagment);
            ConfigurarBotonSubSubmenuNavegacion(BtnStaticItemsCategories);
            ConfigurarBotonSubSubmenuNavegacion(BtnStaticItemsMaintenance);
            ConfigurarBotonSubSubmenuNavegacion(BtnStaticItemsMovementsController);
            ConfigurarBotonSubSubmenuNavegacion(BtnStaticItemsReports);
            ConfigurarBotonSubSubmenuNavegacion(BtnStaticItemsResponsabilityLetter);

            // Panel de Navegación SubSubmenú Teachers - Academic Configuration
            ConfigurarBotonSubSubmenuNavegacion(BtnProcesosAcademicos_PensumCarreras);
            ConfigurarBotonSubSubmenuNavegacion(BtnProcesosAcademicos_PensumCursos);
            ConfigurarBotonSubSubmenuNavegacion(BtnProcesosAcademicos_TarifasCursos);
            ConfigurarBotonSubSubmenuNavegacion(BtnProcesosAcademicos_RevisoresAprobados);

            // Panel de Navegación SubSubmenú Orders - Suppliers
            ConfigurarBotonSubSubmenuNavegacion(BtnSuppliersManagment);
        }
        // Configurar Botones de Menu de Navegacion
        private void ConfigurarBotonSubSubmenuNavegacion(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(222, 224, 222); // Color del fondo Fondo
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(238, 143, 109); // Pasar mouse por encima
            boton.FlatAppearance.MouseDownBackColor = Color.FromArgb(238, 143, 109); // Click con el mouse
            boton.ForeColor = Color.Black;
            boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            boton.Height = 40;
            boton.Width = Math.Max(boton.Width, 200);
            boton.Cursor = Cursors.Default;
            boton.TextAlign = ContentAlignment.MiddleLeft;
            boton.Padding = new Padding(2, 0, 0, 0); // Padding de margen para el botón
        }
        // Configurar Botones de Menu de Navegacion
        private void ConfigurarBotonSubmenuNavegacion(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(222, 224, 222); // Color del fondo Fondo
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(238, 143, 109); // Pasar mouse por encima
            boton.FlatAppearance.MouseDownBackColor = Color.FromArgb(238, 143, 109); // Click con el mouse
            boton.ForeColor = Color.Black;
            boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            boton.Height = 40;
            boton.Width = Math.Max(boton.Width, 200);
            boton.Cursor = Cursors.Default;
            boton.TextAlign = ContentAlignment.MiddleLeft;
            boton.Padding = new Padding(2, 0, 0, 0); // Padding de margen para el botón
        }
        // Configura los TextBox con padding real usando Panel contenedor
        private void ConfigurarTextBox()
        {
            // Configurar cada TextBox
            // ConfigurarTextBoxConPadding(TxtUser);
            // Configuración específica para el password
            // TxtPassword.UseSystemPasswordChar = true;
        }
        // Configurar imagen del logo de la universidad con padding
        private void ConfigurarLogoUniversidad()
        {
            // Configurar imagen del logo de la universidad con padding
            PicLogo.Padding = new Padding(5, 0, 0, 0); // 5px de margen izquierdo
        }
        // Configura eventos para que el Panel se comporte como un TextBox
        private void ConfigurarEventosPanel(Panel panel, TextBox textBox)
        {
            // Cuando se hace clic en el panel, enfocar el TextBox
            panel.Click += (sender, e) => textBox.Focus();

            // Cambiar color cuando el TextBox obtiene/pierde el foco
            textBox.Enter += (sender, e) =>
            {
                panel.BackColor = Color.FromArgb(70, 70, 70); // Un poco más claro al enfocar
                textBox.BackColor = Color.FromArgb(70, 70, 70);

                // MANTENER LAS ESQUINAS REDONDEADAS al cambiar color
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, 15, 15));
            };

            textBox.Leave += (sender, e) =>
            {
                panel.BackColor = Color.FromArgb(60, 60, 60); // Color original
                textBox.BackColor = Color.FromArgb(60, 60, 60);

                // MANTENER LAS ESQUINAS REDONDEADAS al cambiar color
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, 15, 15));
            };

            // EVENTO ADICIONAL: Si el panel cambia de tamaño, actualizar la región
            panel.Resize += (sender, e) =>
            {
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, 15, 15));
            };
        }
        // Crea un TextBox con padding real usando un Panel contenedor
        private void ConfigurarTextBoxConPadding(TextBox textBox)
        {
            // Guardar información original
            Point ubicacionOriginal = textBox.Location;
            Size tamañoOriginal = textBox.Size;
            Control contenedorPadre = textBox.Parent;
            string nombreOriginal = textBox.Name;
            string textoOriginal = textBox.Text;

            // Crear Panel contenedor que simula el TextBox con padding
            Panel panelContenedor = new Panel
            {
                Location = ubicacionOriginal,
                Size = new Size(tamañoOriginal.Width, Math.Max(tamañoOriginal.Height, 45)), // Mínimo 45px de altura
                BackColor = Color.FromArgb(60, 60, 60), // Mismo color que queremos para el TextBox
                BorderStyle = BorderStyle.None,
                Padding = new Padding(6, 6, 12, 8), // Aquí está el padding que necesitas
                Name = "Panel_" + nombreOriginal
            };

            // APLICAR ESQUINAS REDONDEADAS AL PANEL
            panelContenedor.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, panelContenedor.Width, panelContenedor.Height, 15, 15));

            // Configurar el TextBox para que se vea integrado
            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = Color.FromArgb(60, 60, 60); // Mismo color que el panel
            textBox.ForeColor = Color.White;
            textBox.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            textBox.Dock = DockStyle.Fill; // Llenar el panel respetando el padding
            textBox.Text = textoOriginal;
            textBox.TextAlign = HorizontalAlignment.Left; // Alineación horizontal

            // Remover el TextBox original de su contenedor
            contenedorPadre.Controls.Remove(textBox);

            // Agregar el TextBox al panel
            panelContenedor.Controls.Add(textBox);

            // Agregar el panel al contenedor original
            contenedorPadre.Controls.Add(panelContenedor);

            // Eventos para que el panel se comporte como un TextBox
            ConfigurarEventosPanel(panelContenedor, textBox);
        }
        // Convierte un Color de .NET a formato COLORREF de Windows
        private int ColorToColorRef(Color color)
        {
            return color.R | (color.G << 8) | (color.B << 16);
        }
        /// Obtiene el número de build de Windows
        private string GetWindowsBuild()
        {
            try
            {
                var reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                return reg?.GetValue("CurrentBuild")?.ToString() ?? "Desconocido";
            }
            catch
            {
                return "No disponible";
            }
        }
        // Establece el color de la barra de título del formulario
        public void SetTitleBarColor(Color backgroundColor, Color textColor)
        {
            try
            {
                IntPtr hWnd = this.Handle;

                // Verificar que el handle sea válido
                if (hWnd == IntPtr.Zero)
                {
                    MessageBox.Show("Handle del formulario no válido. Intente después de que el formulario esté completamente cargado.",
                                  "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Convertir el color a formato COLORREF (0x00BBGGRR)
                int colorValue = ColorToColorRef(backgroundColor);
                int textColorValue = ColorToColorRef(textColor);

                // Intentar establecer el color de fondo de la barra de título
                int result1 = DwmSetWindowAttribute(hWnd, DWMWA_CAPTION_COLOR, ref colorValue, sizeof(int));

                // Intentar establecer el color del texto de la barra de título
                int result2 = DwmSetWindowAttribute(hWnd, DWMWA_TEXT_COLOR, ref textColorValue, sizeof(int));

                // Verificar si hubo errores
                if (result1 != 0 && result2 != 0)
                {
                    // Si ambos fallaron, mostrar información de depuración
                    var osInfo = $"OS: {Environment.OSVersion}\nBuild: {GetWindowsBuild()}";
                    MessageBox.Show($"No se pudo establecer el color de la barra de título.\n\n{osInfo}\n\nHRESULT: {result1:X8}",
                                  "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (result1 == 0 || result2 == 0)
                {
                    // Al menos uno funcionó
                    Console.WriteLine("Color de barra de título aplicado correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al establecer el color de la barra de título: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Importar función nativa para esquinas redondeadas
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);
        //Configurar bordes redondeados
        private void ConfigurarBordesRedondeados()
        {
            int radioEsquinas = 15;
            // Aplicar bordes redondeados a los paneles que lo requieran
            ConfigurarBordesRedondeadosPanel(PanelRRHH, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelRRHH_1, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelRRHH_2, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelUsers, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelFinances, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelCounts, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelOrders, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelInventory, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelBills, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelChecks, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelBanks, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelAccountingBooks, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelLocations,    radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelStaticItems, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelWarehouses, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelTransfers, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelInventory_1, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelSuppliers, radioEsquinas);
            ConfigurarBordesRedondeadosPanel(PanelTeachers, radioEsquinas);
        }
        // Aplica bordes redondeados a un Panel específico
        public void ConfigurarBordesRedondeadosPanel(Panel panel, int radioEsquinas = 15)
        {
            try
            {
                if (panel == null) return;

                // Aplicar bordes redondeados usando la función CreateRoundRectRgn existente
                panel.Region = System.Drawing.Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, panel.Width, panel.Height, radioEsquinas, radioEsquinas));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al aplicar bordes redondeados al panel {panel.Name}: {ex.Message}");
            }
        }
        // Configura bordes finos personalizados para los paneles principales   
        private void ConfigurarBordesPaneles()
        {
            // *********** IMPORTANTE ************
            //Ejemplos de como llamar a las sobrecargas de ser necesario

            //ConfigurarBordePanel(PanelContenedor, Color.DimGray, 0.2f, 1, 1, 1, 0);
            //ConfigurarBordePanel(PanelHeader, Color.DimGray, 0.2f, 0, 0, 1, 0);
            //ConfigurarBordePanel(PanelBordeSuperior, Color.DimGray, 0.2f, 1, 0, 1, 0);

            //// Método original
            //ConfigurarBordePanel(miPanel, Color.Blue);

            //// Con HEX
            //ConfigurarBordePanel(miPanel, "#FF5733");
            //ConfigurarBordePanel(miPanel, "#1E90FF", 1.0f, 1, 0, 1, 1);

            //// Con RGB específicos
            //ConfigurarBordePanel(miPanel, 255, 87, 51);
            //ConfigurarBordePanel(miPanel, 30, 144, 255, 0.8f);

            //// Con transparencia
            //ConfigurarBordePanel(miPanel, 255, 87, 51, 180); // 180 = semi-transparente

            ConfigurarBordePanel(PanelHeader, 224, 224, 224, 180, 0.2f, 0, 0, 1, 0); // 180 = semi-transparente
            ConfigurarBordePanel(PanelBordeSuperior, 224, 224, 224, 180, 0.2f, 1, 0, 0, 0); // 180 = semi-transparente
            //ConfigurarBordePanel(PanelNavegacion, 224, 224, 224, 180, 0.2f, 0, 0, 0, 1); // 180 = semi-transparente
            //ConfigurarBordePanel(PanelContenedor, 224, 224, 224, 180, 0.2f, 0, 1, 1, 1); // 180 = semi-transparente
        }
        // Función simple para agregar borde fino a cualquier panel con control de bordes y grosor
        public void ConfigurarBordePanel(Panel panel, Color colorBorde, float grosor = 0.5f, int bordeSuperior = 1, int bordeInferior = 1, int bordeIzquierdo = 1, int bordeDerecho = 1)
        {
            if (panel == null) return;
            panel.Paint += (sender, e) =>
            {
                using (Pen pen = new Pen(colorBorde, grosor))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                    if (bordeSuperior == 1)
                        e.Graphics.DrawLine(pen, 0, 0, panel.Width - 1, 0);
                    if (bordeInferior == 1)
                        e.Graphics.DrawLine(pen, 0, panel.Height - 1, panel.Width - 1, panel.Height - 1);
                    if (bordeIzquierdo == 1)
                        e.Graphics.DrawLine(pen, 0, 0, 0, panel.Height - 1);
                    if (bordeDerecho == 1)
                        e.Graphics.DrawLine(pen, panel.Width - 1, 0, panel.Width - 1, panel.Height - 1);
                }
            };
        }
        // SOBRECARGA para colores HEX
        public void ConfigurarBordePanel(Panel panel, string colorHex, float grosor = 0.5f, int bordeSuperior = 1, int bordeInferior = 1, int bordeIzquierdo = 1, int bordeDerecho = 1)
        {
            Color color = ColorTranslator.FromHtml(colorHex);
            ConfigurarBordePanel(panel, color, grosor, bordeSuperior, bordeInferior, bordeIzquierdo, bordeDerecho);
        }
        // SOBRECARGA para RGB específicos
        public void ConfigurarBordePanel(Panel panel, int r, int g, int b, float grosor = 0.5f, int bordeSuperior = 1, int bordeInferior = 1, int bordeIzquierdo = 1, int bordeDerecho = 1)
        {
            Color color = Color.FromArgb(r, g, b);
            ConfigurarBordePanel(panel, color, grosor, bordeSuperior, bordeInferior, bordeIzquierdo, bordeDerecho);
        }
        // SOBRECARGA para RGBA (con transparencia)
        public void ConfigurarBordePanel(Panel panel, int r, int g, int b, int alpha, float grosor = 0.5f, int bordeSuperior = 1, int bordeInferior = 1, int bordeIzquierdo = 1, int bordeDerecho = 1)
        {
            Color color = Color.FromArgb(alpha, r, g, b);
            ConfigurarBordePanel(panel, color, grosor, bordeSuperior, bordeInferior, bordeIzquierdo, bordeDerecho);
        }
        // Evento Load del Formulario
        private void Frm_ControlCenter_MDI_Load(object sender, EventArgs e)
        {
            // Configurar Orden de Tabulaciones
            ConfigurarOrdenTabulacion();
            // Configurar botones en el MDI desde el Load del Formulario
            ConfigurarBotonesEnlace();
            // Configurar botones imagen sin fondo desde el Load del Formulario
            ConfigurarBotonesImagenesSinFondo();
            // Configurar bordes de los paneles
            ConfigurarEstiloBotones();
            // Configurar los TextBox CON PADDING usando Panel
            ConfigurarTextBox();
            // Configurar los botones de navegación
            ConfigurarBotonesNavegacion();
            // Configurar los botones de Submenu de navegación
            ConfigurarSubmenuNavegacion();
            // Configurar los botones de SubSubmenu de navegación
            ConfigurarSubSubmenuNavegacion();
            // Configurar bordes redondeados en los paneles necesarios
            ConfigurarBordesRedondeados();
            // Cambiar el color de fondo del formulario para que combine mejor
            this.BackColor = Color.FromArgb(255, 255, 255);
            // Configurar bordes de paneles
            ConfigurarBordesPaneles();
            // Configurar imagen del logo de la universidad con padding
            ConfigurarLogoUniversidad();
            // Configurar datos del usuario
            CargarDatosUsuario(Usuario);
            // Configurar la vista inicial del MDI
            ConfigurarVistaInicialMDI();

            // Esperar un momento para que el formulario esté completamente inicializado
            this.BeginInvoke(new Action(() =>
            {
                // Cambiar el color de la barra de título al color personalizado rgb(40,36,44)
                SetTitleBarColor(Color.FromArgb(255, 255, 255), Color.White);
                //TxtUser.Focus(); // Establecer foco
            }));
        }
        // Metodo para establecer tamaños iniciales del MDI
        private void MedidasInicialesMDI()
        {
            // Establecer tamaño inicial del MDI
            this.Size = new Size(1200, 700); // Tamaño inicial
            this.MinimumSize = new Size(1200, 125); // Tamaño mínimo
            this.MaximumSize = new Size(1920, 1080); // Tamaño máximo
            this.StartPosition = FormStartPosition.CenterScreen; // Centrado en pantalla
            this.WindowState = FormWindowState.Maximized; // Maximizado al iniciar
        }
        // Constructor del formulario
        public Frm_ControlCenter_MDI()
        {
            // Inicializar Componentes
            InitializeComponent();
            // Configurar medidas iniciales del MDI
            MedidasInicialesMDI();
            // Configuraciones para personalizar la barra de título
            ConfigurarBarraTitulo();
            // Configurar el authController
            authController = new Ctrl_Security_Auth();
            InicializarControlInactividad();
            // Configurar El Evento Formulario Load
            this.Load += Frm_ControlCenter_MDI_Load;
            // Inicializar Pestañas de Sistema
            InicializarSistemaPestanas();
        }
        #endregion PropiedadesIniciales


        #region ControlInactividadSesion

        // Inicializa el timer y el filtro de actividad global
        private void InicializarControlInactividad()
        {
            ultimaActividad = DateTime.Now;

            activityFilter = new Cls_ActivityMessageFilter();
            activityFilter.OnActivity = () => { ultimaActividad = DateTime.Now; };
            Application.AddMessageFilter(activityFilter);

            timerInactividad = new System.Windows.Forms.Timer();
            timerInactividad.Interval = 10000; // Revisar cada 10 segundos (ajustar a 60000 en producción)
            timerInactividad.Tick += TimerInactividad_Tick;
            timerInactividad.Start();

            // Cargar el tiempo configurado desde ParametersConfiguration
            CargarTiempoSesionActivaAsync();
        }
        // Carga el parámetro TiempoSesionActivaMinutos desde la BD
        private async void CargarTiempoSesionActivaAsync()
        {
            tiempoSesionActivaMinutos = await Ctrl_Parameters.ObtenerValorIntAsync("TiempoSesionActivaMinutos", 15);
        }
        // Revisa periódicamente si el tiempo de inactividad fue superado
        private void TimerInactividad_Tick(object sender, EventArgs e)
        {
            double minutosInactivo = (DateTime.Now - ultimaActividad).TotalMinutes;

            if (minutosInactivo >= tiempoSesionActivaMinutos)
            {
                timerInactividad.Stop();
                CerrarSesionPorInactividad();
            }
        }
        // Cierra la sesión automáticamente por inactividad
        // Cierra la sesión automáticamente por inactividad
        private async void CerrarSesionPorInactividad()
        {
            sesionCerradaPorInactividad = true;
            cierreConfirmado = true; // Evita el MessageBox de confirmación del FormClosing normal

            Application.RemoveMessageFilter(activityFilter);

            MessageBox.Show(
                "Su sesión ha expirado por inactividad. Debe iniciar sesión nuevamente.",
                "Sesión expirada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Registrar logout en auditoría
            if (UserData != null)
            {
                try { await authController.LogoutUserAsync(UserData.UserId, UserData.Username); }
                catch { /* No bloquear el cierre por error de auditoría */ }
            }

            // Cerrar formularios hijos
            foreach (Form childForm in this.MdiChildren)
            {
                childForm.Close();
            }

            // Cerrar todas las pestañas
            if (tabControl != null)
            {
                while (tabControl.TabPages.Count > 0)
                {
                    TabPage tab = tabControl.TabPages[0];
                    Form formulario = tab.Controls.OfType<Form>().FirstOrDefault();
                    if (formulario != null)
                        formulario.Close();
                    tabControl.TabPages.RemoveAt(0);
                }
            }

            Application.Restart();
            Application.Exit();
        }
        #endregion ControlInactividadSesion

        private async void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
                "¿ESTÁS SEGURO QUE DESEAS CERRAR SESIÓN?",
                "CONFIRMAR CIERRE DE SESIÓN",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes) return;

            // Detener el control de inactividad para que no interfiera
            timerInactividad?.Stop();
            Application.RemoveMessageFilter(activityFilter);

            if (UserData != null)
            {
                try { await authController.LogoutUserAsync(UserData.UserId, UserData.Username); }
                catch { /* No bloquear el cierre por error de auditoría */ }
            }

            cierreConfirmado = true;

            // Cerrar formularios hijos
            foreach (Form childForm in this.MdiChildren)
            {
                childForm.Close();
            }

            // Cerrar todas las pestañas
            if (tabControl != null)
            {
                while (tabControl.TabPages.Count > 0)
                {
                    TabPage tab = tabControl.TabPages[0];
                    Form formulario = tab.Controls.OfType<Form>().FirstOrDefault();
                    if (formulario != null)
                        formulario.Close();
                    tabControl.TabPages.RemoveAt(0);
                }
            }

            Application.Restart();
            Application.Exit();
        }


        #region CargarDatosUsuario
        // Método para cargar datos del usuario
        private async void CargarDatosUsuario(string username)
        {
            try
            {
                var userInfo = await authController.ObtenerDatosUsuarioAsync(username);
                if (userInfo != null)
                {
                    // CONVERTIR TODOS LOS DATOS A MAYÚSCULAS
                    ConvertirDatosAMayusculas(userInfo);

                    // Guardar datos del usuario
                    this.UserData = userInfo;

                    // Configurar interfaz con datos del usuario
                    ConfigurarInterfazConDatosUsuario(userInfo);

                    //   CARGAR PERMISOS DEL USUARIO
                    await CargarPermisosUsuario(userInfo.UserId, userInfo.RoleId);

                    //   CONFIGURAR VISIBILIDAD DE BOTONES SEGÚN PERMISOS
                    ConfigurarVisibilidadBotonesPrincipales();
                    ConfigurarVisibilidadSubmenus();
                }
                else
                {
                    MessageBox.Show("No se pudieron cargar los datos del usuario.",
                                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del usuario: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Método para convertir todos los datos string a mayúsculas
        // Método para convertir todos los datos del usuario a MAYÚSCULAS
        private void ConvertirDatosAMayusculas(dynamic userInfo)
        {
            try
            {
                if (userInfo == null) return;

                // Usar reflexión para obtener todas las propiedades de tipo string
                Type userInfoType = userInfo.GetType();
                PropertyInfo[] properties = userInfoType.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    // Solo procesar propiedades de tipo string que se puedan escribir
                    if (property.PropertyType == typeof(string) && property.CanWrite && property.CanRead)
                    {
                        try
                        {
                            // Obtener el valor actual
                            string valorActual = property.GetValue(userInfo) as string;

                            // Si no es null ni vacío, convertir a mayúsculas
                            if (!string.IsNullOrEmpty(valorActual))
                            {
                                // Convertir a mayúsculas manteniendo espacios y caracteres especiales
                                string valorMayusculas = valorActual.ToUpper().Trim();

                                // Asignar el valor convertido de vuelta a la propiedad
                                property.SetValue(userInfo, valorMayusculas);
                            }
                        }
                        catch (Exception propEx)
                        {
                            // Si hay error en una propiedad específica, continuar con las demás
                            Console.WriteLine($"Error al convertir propiedad {property.Name}: {propEx.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al convertir datos a mayúsculas: {ex.Message}",
                               "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Configura la interfaz con los datos del usuario
        private void ConfigurarInterfazConDatosUsuario(Mdl_Security_UserInfo userInfo)
        {
            // Valores iniciales - TODOS con ToUpper() por seguridad
            LblUsername.Text = userInfo.FullName?.ToUpper() ?? "";
            LblRol.Text = "ROL: " + (userInfo.RoleName?.ToUpper() ?? "");
            LblUser.Text = "USUARIO: " + (userInfo.Username?.ToUpper() ?? ""); // ← Cambiado aquí
        }
        #endregion CargarDatosUsuario
        #region GestionPermisos
        // Propiedad para almacenar permisos del usuario
        private List<string> permisosUsuario = new List<string>();

        // Método para cargar permisos del usuario (Rol + Específicos)
        private async Task CargarPermisosUsuario(int userId, int roleId)
        {
            try
            {
                permisosUsuario = await authController.ObtenerPermisosUsuarioAsync(userId, roleId);

                // Debug: Mostrar permisos cargados
                System.Diagnostics.Debug.WriteLine($"Permisos cargados: {permisosUsuario.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar permisos: {ex.Message}",
                               "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para verificar si el usuario tiene un permiso específico
        private bool TienePermiso(string permissionCode)
        {
            if (permisosUsuario == null || permisosUsuario.Count == 0)
                return false;

            return permisosUsuario.Contains(permissionCode);
        }

        // Método para configurar visibilidad de BOTONES PRINCIPALES (Tabs)
        private void ConfigurarVisibilidadBotonesPrincipales()
        {
            // Primero ocultar TODOS los botones
            BtnHome.Visible = false;
            BtnRRHH.Visible = false;
            BtnUsers.Visible = false;
            BtnFinances.Visible = false;
            BtnOrders.Visible = false;
            Btn_Inventory.Visible = false;
            BtnBills.Visible = false;
            BtnLocations.Visible = false;
            BtnProcesosAcademicos.Visible = false;

            // Luego hacerlos visibles en ORDEN INVERSO
            if (TienePermiso("TEACHERS_TAB"))
                BtnProcesosAcademicos.Visible = true;

            if (TienePermiso("LOCATIONS_TAB"))
                BtnLocations.Visible = true;

            if (TienePermiso("BILLS_TAB"))
                BtnBills.Visible = true;

            if (TienePermiso("INVENTORY_TAB"))
                Btn_Inventory.Visible = true;

            if (TienePermiso("PURCHASEORDERS_TAB"))
                BtnOrders.Visible = true;

            if (TienePermiso("FINANCES_TAB"))
                BtnFinances.Visible = true;

            if (TienePermiso("USERS_TAB"))
                BtnUsers.Visible = true;

            if (TienePermiso("EMPLOYEES_TAB"))
                BtnRRHH.Visible = true;

            // Home siempre visible y al final para que quede arriba
            BtnHome.Visible = true;
        }
        // Método para configurar visibilidad de SUBMENÚS
        private void ConfigurarVisibilidadSubmenus()
        {
            // ========== RECURSOS HUMANOS ==========
            BtnRRHH_Trabajadores.Visible = TienePermiso("EMPLOYEES_MANAGMENT");
            BtnRRHH_Docencia.Visible = TienePermiso("TEACHERS_TAB");
            BtnRRHH_Calendario.Visible = TienePermiso("EMPLOYEES_CALENDAR");
            BtnRRHH_Planillas.Visible = TienePermiso("EMPLOYEES_SPREADSHEETS");
            BtnRRHH_MyProfile.Visible = TienePermiso("EMPLOYEES_INFORMATION");

            // ========== USUARIOS ==========
            BtnUsersManagment.Visible = TienePermiso("USERS_MANAGMENT");
            BtnUsersRolesPermisos.Visible = TienePermiso("USERS_ROLEPERMISSIONS");
            Btn_ITSM_Technology.Visible = TienePermiso("USERS_ITSM_TECHONOLOGY");

            // ========== FINANCES ==========
            BtnFinances_Accounts.Visible = TienePermiso("ACCOUNTS_TAB");
            BtnFinances_Checks.Visible = TienePermiso("CHECKS_TAB");
            BtnFinances_Transfers.Visible = TienePermiso("TRANSFERS_TAB");
            BtnFinances_Banks.Visible = TienePermiso("BANKS_TAB");
            BtnFinances_AccountingBooks.Visible = TienePermiso("ACCOUNTINGBOOKS_TAB");

            // ========== PROVEEDORES ==========
            BtnSuppliersManagment.Visible = TienePermiso("SUPPLIERS_MANAGMENT");

            // ========== CUENTAS ==========
            BtnCountsManagment.Visible = TienePermiso("ACCOUNTS_MANAGMENT");

            // ========== COMPRAS ==========
            BtnOrdersManagment.Visible = TienePermiso("PURCHASEORDERS_MANAGMENT");
            BtnOrdersRequisicionManagment.Visible = TienePermiso("PURCHASEORDERS_REQUISITION");
            BtnOrdersSolicitud.Visible = TienePermiso("PURCHASEORDERS_REQUEST");

            // ========== INVENTARIO ==========
            BtnInvKardex.Visible = TienePermiso("KARDEX_TAB");
            BtnInvStaticItems.Visible = TienePermiso("STATICITEMS_TAB");
            BtnInvWarehouse.Visible = TienePermiso("WAREHOUSE_TAB");

            // ========== KARDEX ==========
            BtnKardex_CatalogLocationsCategories.Visible = TienePermiso("KARDEX_CATALOG");
            BtnKardex_ItemsManagment.Visible = TienePermiso("KARDEX_CATALOG");
            BtnKardexInventory.Visible = TienePermiso("KARDEX_INVENTORY");
            BtnKardexInputControl.Visible = TienePermiso("KARDEX_INPUTCONTROL");
            BtnKardexInventoryReport.Visible = TienePermiso("KARDEX_REPORTS");

            // ========== GASTOS ==========
            BtnBillsAuthorization.Visible = TienePermiso("BILLS_AUTHORIZATION");
            BtnBillsBudgetControl.Visible = TienePermiso("BILLS_BUDGETCONTROL");
            BtnBillsManagment.Visible = TienePermiso("BILLS_MANAGMENT");
            BtnBillsCategorias.Visible = TienePermiso("BILLS_CATEGORIES");
            BtnBillsReports.Visible = TienePermiso("BILLS_REPORTS");
            BtnBillsCatalog.Visible = TienePermiso("BILLS_CATALOG");

            // ========== CHEQUES ==========
            BtnChecksManagment.Visible = TienePermiso("CHECKS_MANAGMENT");
            BtnChecksReports.Visible = TienePermiso("CHECKS_REPORTS");
            BtnChecksConfiguration.Visible = TienePermiso("CHECKS_CONFIGURATION");
            BtnChecksFileControl.Visible = TienePermiso("CHECKS_FILECONTROL");

            // ========== BANCOS ==========
            BtnBanksAccounts.Visible = TienePermiso("BANKS_MANAGMENT");
            BtnBanksCashControl.Visible = TienePermiso("BANKS_CASHCONTROL");
            BtnBanksConciliacion.Visible = TienePermiso("BANKS_CONCILIATION");
            BtnBanksConfiguration.Visible = TienePermiso("BANKS_CONFIGURATION");
            BtnBanksMovements.Visible = TienePermiso("BANKS_MOVEMENTS");
            BtnBanksReports.Visible = TienePermiso("BANKS_REPORTS");

            // ========== LIBROS CONTABLES ==========
            BtnAccountingBooksDiario.Visible = TienePermiso("ACCOUNTINGBOOKS_DAILYBOOK");
            BtnAccountingBooksMayor.Visible = TienePermiso("ACCOUNTINGBOOKS_LEDGER");
            BtnAccountingBooksFinancialStatemants.Visible = TienePermiso("ACCOUNTINGBOOKS_FINANTIALSTATEMANTS");
            BtnAccountingBooksClosing.Visible = TienePermiso("ACCOUNTINGBOOKS_CLOSING");
            BtnAccountingBooksReports.Visible = TienePermiso("ACCOUNTINGBOOKS_REPORTS");

            // ========== SEDES ==========
            BtnLocationsManagment.Visible = TienePermiso("LOCATIONS_MANAGMENT");
            BtnLocationsConfiguration.Visible = TienePermiso("LOCATIONS_CONFIGURATION");
            BtnLocationsDepartments.Visible = TienePermiso("LOCATIONS_DEPARTMENTS");
            BtnLocationsFinancialControl.Visible = TienePermiso("LOCATIONS_FINANTIALCONTROL");
            BtnLocationsStaff.Visible = TienePermiso("LOCATIONS_STAFF");

            // ========== ACTIVOS FIJOS ==========
            BtnStaticItemsManagment.Visible = TienePermiso("STATICITEMS_MANAGMENT");
            BtnStaticItemsCategories.Visible = TienePermiso("STATICITEMS_DEPRECIATION");
            BtnStaticItemsMaintenance.Visible = TienePermiso("STATICITEMS_MAINTENANCE");
            BtnStaticItemsMovementsController.Visible = TienePermiso("STATICITEMS_MOVEMENTS");
            BtnStaticItemsReports.Visible = TienePermiso("STATICITEMS_REPORTS");
            BtnStaticItemsResponsabilityLetter.Visible = TienePermiso("STATICITEMS_RESPONSLETTER");

            // ========== DOCENTES ==========
            BtnProcesosAcademicos_PensumCarreras.Visible = TienePermiso("TEACHERS_CONFIGURATION");
            BtnProcesosAcademicos_PensumCursos.Visible = TienePermiso("TEACHERS_PERSONAL");
            BtnProcesosAcademicos_TarifasCursos.Visible = TienePermiso("TEACHERS_SECTIONS");
            BtnProcesosAcademicos_RevisoresAprobados.Visible = TienePermiso("TEACHERS_SCHEDULES");
            BtnProcesosAcademicos_AprobacionDocentes.Visible = TienePermiso("TEACHERS_REPORTS");
            BtnProcesosAcademicos_PreasignacionCursos.Visible = TienePermiso("TEACHERS_REPORTS");
            BtnProcesosAcademicos_RevisionAsignaciones.Visible = TienePermiso("TEACHERS_REPORTS");
            BtnProcesosAcademicos_HorariosOficiales.Visible = TienePermiso("TEACHERS_REPORTS");
            BtnProcesosAcademicos_CalendariosAcademicos.Visible = TienePermiso("TEACHERS_REPORTS");


            // ========== TRANSFERENCIAS ==========
            BtnTransfersReports.Visible = TienePermiso("TRANSFERS_REPORTS");
            BtnTransfersManagment.Visible = TienePermiso("TRANSFERS_MANAGMENT");

            // LLAMADA A REORDENAMIENTO DE SUBMENUS
            //ReordenarTodosLosSubmenus();
        }
        #endregion GestionPermisos
        #region RedimensionarPaneles
        //Tamaños Originales de los Paneles
        //PanelEmployees Size = 300, 280
        //PanelUsers Size = 300, 160
        //PanelSuppliers Size = 300, 41
        //PanelCounts Size = 300, 41
        //PanelOrders Size = 300, 120
        //PanelKardex Size = 300, 200
        //PanelBills Size = 300, 240
        //PanelChecks Size = 300, 200
        //PanelBanks Size = 300, 240
        //PanelAccountingBooks Size = 300, 240
        //PanelLocations Size = 300, 200
        //PanelStaticItems Size = 300, 280

        //Locations Originales de los Paneles
        //PanelEmployees.Location = 195, 140
        //PanelUsers.Location = 195, 180
        //PanelSuppliers.Location = 195, 220
        //PanelCounts.Location = 195, 260
        //PanelOrders.Location = 195, 300
        //PanelKardex.Location = 195, 340
        //PanelBills.Location = 195, 380
        //PanelChecks.Location = 195, 420
        //PanelBanks.Location = 195, 460
        //PanelAccountingBooks.Location = 195, 500
        //PanelLocations.Location = 195, 540
        //PanelStaticItems.Location = 195, 580

        // Función para ocultar todos los paneles excepto el especificado
        private void OcultarTodosLosPaneles(Panel panelExcepcion = null)
        {
            // Lista de todos los paneles y sus botones correspondientes
            var panelesYBotones = new (Panel panel, Button boton)[]
            {
                (PanelRRHH, BtnRRHH),
                (PanelUsers, BtnUsers),
                (PanelFinances, BtnFinances),
                (PanelOrders, BtnOrders),
                (PanelInventory, Btn_Inventory),
                (PanelBills, BtnBills),
                (PanelLocations, BtnLocations),
                (PanelTeachers, BtnProcesosAcademicos)
            };
            // Ocultar todos los paneles excepto el especificado
            foreach (var (panel, boton) in panelesYBotones)
            {
                if (panel != panelExcepcion)
                {
                    panel.Visible = false;
                    panel.Location = new Point(220, 1);
                    panel.Size = new Size(1, 1);
                    boton.BackColor = Color.FromArgb(255, 255, 255);
                }
            }
            // Lista de todos los paneles y sus botones correspondientes
            var subpanelesYBotones = new (Panel subpanel, Button subboton)[]
            {
                //RECURSOS HUMANOS
                (PanelRRHH_1, BtnRRHH),
                (PanelRRHH_2, BtnRRHH),
                //KARDEX
                (PanelInventory_1, Btn_Inventory),
                //ACTIVOS FIJOS
                (PanelStaticItems, Btn_Inventory),
                //ALMACENES/WAREHOUSE
                (PanelWarehouses, Btn_Inventory),
                //ORDERS
                (PanelSuppliers, BtnOrders),
                //FINANCES
                (PanelCounts, BtnFinances),
                (PanelChecks, BtnFinances),
                (PanelTransfers, BtnFinances),
                (PanelBanks, BtnFinances),
                (PanelAccountingBooks, BtnFinances)
            };
            // Ocultar todos los paneles excepto el especificado
            foreach (var (subpanel, subboton) in subpanelesYBotones)
            {
                if (subpanel != panelExcepcion)
                {
                    subpanel.Visible = false;
                    subpanel.Location = new Point(510, 1);
                    subpanel.Size = new Size(1, 1);
                    subboton.BackColor = Color.FromArgb(255, 255, 255);
                }
            }
        }
        // Función para configurar la visibilidad y tamaño de los paneles según el botón presionado
        private void ConfigurarPanelesVisibles(Button button)
        {
            // Diccionario con la configuracion de cada panel (solo tamaño, la posicion se calcula dinamicamente)
            var configuracionPaneles = new Dictionary<Button, (Panel panel, Size tamaño)>
            {
                { BtnRRHH, (PanelRRHH, new Size(300, 200)) },
                { BtnUsers, (PanelUsers, new Size(300, 120)) },
                { BtnFinances, (PanelFinances, new Size(300, 200)) },
                { BtnOrders, (PanelOrders, new Size(300, 160)) },
                { Btn_Inventory, (PanelInventory, new Size(300, 200)) },
                { BtnBills, (PanelBills, new Size(300, 240)) },
                { BtnLocations, (PanelLocations, new Size(300, 200)) },
                { BtnProcesosAcademicos, (PanelTeachers, new Size(300, 360)) },
            };

            // Verificar si el boton existe en la configuracion
            if (!configuracionPaneles.ContainsKey(button)) return;

            var (panel, tamaño) = configuracionPaneles[button];

            // Si el panel ya esta visible, solo lo ocultamos
            if (panel.Visible)
            {
                panel.Visible = false;
                panel.Location = new Point(220, 1);
                panel.Size = new Size(1, 1);
                button.BackColor = Color.FromArgb(255, 255, 255);

                // CORRECCION: Ocultar sub-sub paneles tambien cuando se cierra el panel
                OcultarTodosLosSubPaneles();
            }
            else
            {
                // Ocultar todos los otros paneles primero
                OcultarTodosLosPaneles(panel);

                // CALCULAR POSICION DINAMICA DEL PANEL
                int panelX = 215; // X siempre fijo
                int panelY = button.Location.Y + 1; // 1 punto debajo del boton
                Point ubicacion = new Point(panelX, panelY);

                // Mostrar el panel seleccionado
                panel.Location = ubicacion;
                panel.Size = tamaño;
                panel.Visible = true;
                button.BackColor = Color.FromArgb(222, 224, 222);
            }
        }
        private void ConfigurarSubPanelesVisibles(Button button)
        {
            var configuracionSubPaneles = new Dictionary<Button, (Panel panel, Size tamaño)>
            {
                //INVENTORY
                { BtnInvKardex, (PanelInventory_1, new Size(300, 200)) },
                { BtnInvStaticItems, (PanelStaticItems, new Size(300, 240)) },
                { BtnInvWarehouse, (PanelWarehouses, new Size(300, 80)) },

                //COMPRAS
                { BtnSuppliers, (PanelSuppliers, new Size(300, 40)) },

                //FINANCES
                { BtnFinances_Accounts, (PanelCounts, new Size(300, 40)) },
                { BtnFinances_Checks, (PanelChecks, new Size(300, 160)) },
                { BtnFinances_Transfers, (PanelTransfers, new Size(300, 80)) },
                { BtnFinances_Banks, (PanelBanks, new Size(300, 240)) },
                { BtnFinances_AccountingBooks, (PanelAccountingBooks, new Size(300, 200)) },

                //RECURSOS HUMANOS
                { BtnRRHH_Trabajadores, (PanelRRHH_1, new Size(300, 40)) },
                { BtnRRHH_Docencia, (PanelRRHH_2, new Size(300, 120)) }
            };

            if (!configuracionSubPaneles.ContainsKey(button)) return;

            var (panel, tamaño) = configuracionSubPaneles[button];

            if (panel.Visible)
            {
                panel.Visible = false;
                panel.Location = new Point(510, 1);
                panel.Size = new Size(1, 1);
            }
            else
            {
                // Ocultar todos los demás subpaneles primero
                OcultarTodosLosSubPaneles();

                // Detectar automáticamente el panel principal que contiene el botón
                Panel panelPrincipal = ObtenerPanelPrincipalDelBoton(button);

                if (panelPrincipal != null)
                {
                    int panelX = 510;
                    int panelY = panelPrincipal.Location.Y + button.Location.Y;

                    Point ubicacion = new Point(panelX, panelY);
                    panel.Location = ubicacion;
                    panel.Size = tamaño;
                    panel.Visible = true;
                }
            }
        }
        // Detecta automáticamente el panel principal
        private Panel ObtenerPanelPrincipalDelBoton(Button button)
        {
            // Mapeo de botones a sus paneles principales
            var mapeoBotonPanel = new Dictionary<Button, Panel>
            {
                // KARDEX
                { BtnInvKardex, PanelInventory },
                { BtnInvStaticItems, PanelInventory },
                { BtnInvWarehouse, PanelInventory },

                // TEACHERS
                { BtnProcesosAcademicos_PensumCarreras, PanelTeachers },
                { BtnProcesosAcademicos_PensumCursos, PanelTeachers },
                { BtnProcesosAcademicos_TarifasCursos, PanelTeachers },
                { BtnProcesosAcademicos_RevisoresAprobados, PanelTeachers },
                { BtnProcesosAcademicos_AprobacionDocentes, PanelTeachers },
                { BtnProcesosAcademicos_PreasignacionCursos, PanelTeachers },
                { BtnProcesosAcademicos_RevisionAsignaciones, PanelTeachers },
                { BtnProcesosAcademicos_HorariosOficiales, PanelTeachers },
                { BtnProcesosAcademicos_CalendariosAcademicos, PanelTeachers },

                //ORDERS
                { BtnSuppliers, PanelOrders },

                //FINANCES
                { BtnFinances_Accounts, PanelFinances },
                { BtnFinances_Checks, PanelFinances },
                { BtnFinances_Transfers, PanelFinances },
                { BtnFinances_Banks, PanelFinances },
                { BtnFinances_AccountingBooks, PanelFinances },

                //RECURSOS HUMANOS
                { BtnRRHH_Trabajadores, PanelRRHH },
                { BtnRRHH_Docencia, PanelRRHH },
                { BtnRRHH_Calendario, PanelRRHH },
                { BtnRRHH_Planillas, PanelRRHH },
                { BtnRRHH_MyProfile, PanelRRHH }
            };

            return mapeoBotonPanel.ContainsKey(button) ? mapeoBotonPanel[button] : null;
        }
        // Oculta todos los subpaneles de todos los paneles principales
        private void OcultarTodosLosSubPaneles()
        {
            // INVENTORY
            OcultarSubPanel(PanelInventory_1);
            OcultarSubPanel(PanelStaticItems);
            OcultarSubPanel(PanelWarehouses);

            //FINANCES
            OcultarSubPanel(PanelCounts);
            OcultarSubPanel(PanelChecks);
            OcultarSubPanel(PanelTransfers);
            OcultarSubPanel(PanelBanks);
            OcultarSubPanel(PanelAccountingBooks);

            //ORDERS
            OcultarSubPanel(PanelSuppliers);

            // RECURSOS HUMANOS
            OcultarSubPanel(PanelRRHH_1);
            OcultarSubPanel(PanelRRHH_2);
        }

        // Facilita no repetir código para ocultar subpaneles
        private void OcultarSubPanel(Panel panel)
        {
            panel.Visible = false;
            panel.Location = new Point(510, 1);
            panel.Size = new Size(1, 1);
        }
        // Función específica para el botón Home - cierra todos los paneles
        private void CerrarTodosLosPaneles()
        {
            OcultarTodosLosPaneles(); // No pasamos ningún panel como excepción, así oculta todos
        }
        // Función para el evento MouseEnter de los botones
        private void ConfigurarPanelesMouseEnter(Button button)
        {
            // Verificar si algún panel está visible
            if (!ObtenerFlagPanelVisible()) return; // Si no hay paneles abiertos, no hacer nada

            // Si es el botón Home, cerrar todos los paneles
            if (button == BtnHome)
            {
                CerrarTodosLosPaneles();
                return;
            }

            // Diccionario con la configuración de cada panel (solo tamaño)
            var configuracionPaneles = new Dictionary<Button, (Panel panel, Size tamaño)>
            {
                { BtnRRHH, (PanelRRHH, new Size(300, 280)) },
                { BtnUsers, (PanelUsers, new Size(300, 120)) },
                { BtnFinances, (PanelFinances, new Size(300, 200)) },
                { BtnOrders, (PanelOrders, new Size(300, 160)) },
                { Btn_Inventory, (PanelInventory, new Size(300, 200)) },
                { BtnBills, (PanelBills, new Size(300, 240)) },
                { BtnLocations, (PanelLocations, new Size(300, 200)) },
                { BtnInvStaticItems, (PanelStaticItems, new Size(300, 280)) },
                { BtnInvWarehouse, (PanelWarehouses, new Size(300, 80)) },
                { BtnProcesosAcademicos, (PanelTeachers, new Size(300, 360)) }
            };

            // Verificar si el botón existe en la configuración
            if (!configuracionPaneles.ContainsKey(button)) return;

            var (panel, tamaño) = configuracionPaneles[button];

            // Si el panel del botón ya está visible, no hacer nada
            if (panel.Visible) return;

            // CALCULAR POSICIÓN DINÁMICA DEL PANEL
            int panelX = 215; // X siempre fijo
            int panelY = button.Location.Y + 1; // 1 punto debajo del botón
            Point ubicacion = new Point(panelX, panelY);

            // Ocultar todos los paneles y mostrar el del botón actual
            OcultarTodosLosPaneles(panel);
            panel.Location = ubicacion;
            panel.Size = tamaño;
            panel.Visible = true;
            button.BackColor = Color.FromArgb(222, 224, 222);
        }
        // Función para obtener el flag que indica si algún panel está visible
        private bool ObtenerFlagPanelVisible()
        {
            var paneles = new Panel[]
            {
                PanelRRHH, PanelUsers, PanelOrders, PanelInventory, PanelLocations,
                PanelTeachers, PanelFinances, PanelBills
            };

            return paneles.Any(panel => panel.Visible);
        }
        #endregion RedimensionarPaneles
        #region PanelConfiguracion
        // Variable para controlar si el panel está desplegado
        private bool isPanelProfileVisible = false;
        // Configurar la visibilidad del Panel del Perfil del Usuario
        private void ConfigurarPanelProfile()
        {
            if (isPanelProfileVisible)
            {
                // Ocultar el panel
                PanelProfile.Visible = false;
                isPanelProfileVisible = false;
                // Opcional: mover a posición oculta
                PanelProfile.Location = new Point(200, 1);
                PanelProfile.Size = new Size(1, 1);
            }
            else
            {
                // IMPORTANTE: Primero establecer el tamaño
                PanelProfile.Size = new Size(400, 200);

                // Luego calcular y establecer la posición usando el método dinámico
                isPanelProfileVisible = true; // Marcar como visible ANTES de posicionar
                PositionPanelProfile(); // Usar el cálculo dinámico desde el inicio

                // CLAVE: Traer al frente para que se superponga a todo
                PanelProfile.BringToFront();

                // Asegurar que el panel esté por encima de todos los controles
                PanelProfile.Parent.Controls.SetChildIndex(PanelProfile, 0);

                // Finalmente hacer visible
                PanelProfile.Visible = true;

                // Opcional: Darle foco si tiene controles interactuables
                PanelProfile.Focus();
            }
        }
        // Método para posicionar el panel basado en el ancho actual del formulario
        private void PositionPanelProfile()
        {
            if (isPanelProfileVisible)
            {
                // Calcular la posición X basándose en el ancho del formulario
                int margenDerecho = 45; // Distancia del borde derecho (ajusta según necesites)
                int posicionX = this.Width - PanelProfile.Width - margenDerecho;

                // Asegurar que no se salga del lado izquierdo
                if (posicionX < 10) posicionX = 10;

                PanelProfile.Location = new Point(posicionX, 70);
            }
        }
        private void BtnProfile_Click(object sender, EventArgs e)
        {
            ConfigurarPanelProfile();
        }
        #endregion PanelConfiguracion
        #region EventosClickNavegacion
        // Eventos Click de los botones del menú de navegación
        private void BtnHome_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            ConfigurarVistaInicialMDI();
        }
        private void BtnEmployees_Click(object sender, EventArgs e)
        {
            ConfigurarPanelesVisibles(BtnRRHH);
        }
        private void BtnUsers_Click(object sender, EventArgs e)
        {
            ConfigurarPanelesVisibles(BtnUsers);
        }
        private void BtnFinances_Click(object sender, EventArgs e)
        {
            ConfigurarPanelesVisibles(BtnFinances);
        }
        private void BtnOrders_Click(object sender, EventArgs e)
        {
            ConfigurarPanelesVisibles(BtnOrders);
        }
        private void Btn_Inventory_Click(object sender, EventArgs e)
        {
            ConfigurarPanelesVisibles(Btn_Inventory);
        }
        private void BtnBills_Click(object sender, EventArgs e)
        {
            ConfigurarPanelesVisibles(BtnBills);
        }
        private void BtnLocations_Click(object sender, EventArgs e)
        {
            ConfigurarPanelesVisibles(BtnLocations);
        }
        private void Btn_Teachers_Click(object sender, EventArgs e)
        {
            ConfigurarPanelesVisibles(BtnProcesosAcademicos);
        }
        #endregion EventosClickNavegacion
        #region EventosMouseEnterNavegacion
        private void BtnHome_MouseEnter(object sender, EventArgs e)
        {
            ConfigurarPanelesMouseEnter(BtnHome);
        }
        private void BtnEmployees_MouseEnter(object sender, EventArgs e)
        {
            ConfigurarPanelesMouseEnter(BtnRRHH);
        }

        private void BtnUsers_MouseEnter(object sender, EventArgs e)
        {
            ConfigurarPanelesMouseEnter(BtnUsers);
        }
        private void BtnFinances_MouseEnter(object sender, EventArgs e)
        {
            ConfigurarPanelesMouseEnter(BtnFinances);
        }
        private void BtnOrders_MouseEnter(object sender, EventArgs e)
        {
            ConfigurarPanelesMouseEnter(BtnOrders);
        }

        private void BtnKardex_MouseEnter(object sender, EventArgs e)
        {
            ConfigurarPanelesMouseEnter(Btn_Inventory);
        }

        private void BtnBills_MouseEnter(object sender, EventArgs e)
        {
            ConfigurarPanelesMouseEnter(BtnBills);
        }
        private void BtnLocations_MouseEnter(object sender, EventArgs e)
        {
            ConfigurarPanelesMouseEnter(BtnLocations);
        }
        private void Btn_Teachers_MouseEnter(object sender, EventArgs e)
        {
            ConfigurarPanelesMouseEnter(BtnProcesosAcademicos);
        }
        #endregion EventosMouseEnterNavegacion
        #region EventosClickSubmenuNavegacion
        
        private void BtnAccountingBooksClosing_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Cierre Contable";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Cierre Contable", "AccountingBooksClosing");
        }
        private void BtnAccountingBooksDiario_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Libro Diario";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Libro Diario", "AccountingBooksDiario");
        }
        private void BtnAccountingBooksFinancialStatemants_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Estados Financieros";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Estados Financieros", "AccountingBooksFinancialStatemants");
        }
        private void BtnAccountingBooksMayor_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Libro Mayor";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Libro Mayor", "AccountingBooksMayor");
        }
        private void BtnAccountingBooksReports_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Reportes Contables";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Reportes Contables", "AccountingBooksReports");
        }
        private void BtnBanksAccounts_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Cuentas Bancarias";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Cuentas Bancarias", "BanksAccounts");
        }
        private void BtnBanksCashControl_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Control de Efectivo";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Control de Efectivo", "BanksCashControl");
        }
        private void BtnBanksConciliacion_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Conciliación Bancaria";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Conciliación Bancaria", "BanksConciliacion");
        }
        private void BtnBanksConfiguration_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Configuración Bancaria";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Configuración Bancaria", "BanksConfiguration");
        }
        private void BtnBanksMovements_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Movimientos Bancarios";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Movimientos Bancarios", "BanksMovements");
        }
        private void BtnBanksReports_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Reportes Bancarios";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Reportes Bancarios", "BanksReports");
        }
        private void BtnBillsAuthorization_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Autorización y Flujos";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Autorización y Flujos", "BillsAuthorization");
        }
        private void BtnBillsBudgetControl_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Control Presupuestario";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Control Presupuestario", "BillsBudgetControl");
        }
        private void BtnBillsCatalog_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Catalogos de Gastos";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Catalogos de Gastos", "BillsCatalog");
        }
        private void BtnBillsCategorias_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Categorías y Contabilización";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Categorías y Contabilización", "BillsCategorias");
        }
        private void BtnBillsManagment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Gestión de Gastos";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Gestión de Gastos", "BillsManagment");
        }
        private void BtnBillsReports_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Reportes de Gastos";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Reportes de Gastos", "BillsReports");
        }
        private void BtnChecksConfiguration_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Configurar Formato de Cheques";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Configurar Formato de Cheques", "ChecksConfiguration");
        }
        private void BtnChecksManagment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_Checks_Managment frm = new Frm_Checks_Managment();
            frm.Text = "Emisión de Cheques";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Gestión de Cheques", "ChecksManagment");
        }
        private void BtnChecksReports_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_Checks_Reports frm = new Frm_Checks_Reports();
            frm.Text = "Reportes de Cheques";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Reportes de Cheques", "ChecksReports");
        }
        private void BtnChecksFileControl_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_Checks_FileControl frm = new Frm_Checks_FileControl();
            frm.Text = "Control de Archivo de Cheques";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Control de Archivo de Cheques", "ChecksFileControl");
        }
        private void BtnCountsManagment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_Accounts_Managment frm = new Frm_Accounts_Managment();
            frm.Text = "Gestión de Cuentas";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Gestión de Cuentas", "CountsManagment");
        }
        private void BtnInvKardex_Click(object sender, EventArgs e)
        {
            ConfigurarSubPanelesVisibles(BtnInvKardex);
        }
        private void BtnInvStaticItems_Click(object seder, EventArgs e)
        {
            ConfigurarSubPanelesVisibles(BtnInvStaticItems);
        }
        private void BtnInvWarehouse_Click(object sender, EventArgs e)
        {
            ConfigurarSubPanelesVisibles(BtnInvWarehouse);
        }
        private void BtnLocationsConfiguration_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Configuración Operativa";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Configuración Operativa", "LocationsConfiguration");
        }
        private void BtnLocationsDepartments_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Gestión de Áreas Académicas";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Gestión de Áreas Académicas", "LocationsDepartments");
        }
        private void BtnLocationsFinancialControl_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Control Financiero Por Sede";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Control Financiero Por Sede", "LocationsFinancialControl");
        }
        private void BtnLocationsManagment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_Locations_Managment frm = new Frm_Locations_Managment();
            frm.Text = "Gestión de Sedes";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Gestión de Sedes", "LocationsManagment");
        }
        private void BtnLocationsStaff_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_LocationStaffAssignments frm = new Frm_LocationStaffAssignments();
            frm.Text = "Personal Por Sede";
            frm.BackColor = Color.White;
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Personal Por Sede", "LocationsStaff");
        }
        private void BtnOrdersManagment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Emisión de Órden de Compra";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Emisión de Órden de Compra", "OrdersManagment");
        }
        private void BtnOrdersRequisicionManagment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Gestión de Requisiciones";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Gestión de Requisiciones", "OrdersRequisicionManagment");
        }
        private void BtnOrdersSolicitud_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Solicitudes de Compras";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Solicitudes de Compras", "OrdersSolicitud");
        }
        private void BtnSuppliersManagment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_Suppliers_Managment frm = new Frm_Suppliers_Managment();
            frm.Text = "Gestión de Proveedores";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Gestión de Proveedores", "SuppliersManagment");
        }
        private void BtnTransfersManagment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_Transfers_Managment frm = new Frm_Transfers_Managment();
            frm.Text = "Gestión de Transferencias";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Gestión de Transferencias", "TransfersManagment");
        }
        private void BtnTransfersReports_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_Transfers_Reports frm = new Frm_Transfers_Reports();
            frm.Text = "Reportes de Transferencias";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Reportes de Transferencias", "TransfersReports");
        }
        private void BtnUsersManagment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_ITSM_Users_Managment frm = new Frm_ITSM_Users_Managment();
            frm.Text = "Gestión de Usuarios";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Gestión de Usuarios", "UsersManagment");
        }
        private void BtnUsersRolesPermisos_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_ITSM_Users_RolesPermissions frm = new Frm_ITSM_Users_RolesPermissions();
            frm.Text = "Roles y Permisos";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Roles y Permisos", "UsersRolesPermisos");
        }
        private void Btn_ITSM_Technology_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_ITSM_Technology frm = new Frm_ITSM_Technology();
            frm.Text = "Equipos de Tecnología";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Equipos de Tecnología", "ITSM_Technology");
        }
        private void BtnRRHH_Trabajadores_Click(object sender, EventArgs e)
        {
            ConfigurarSubPanelesVisibles(BtnRRHH_Trabajadores);
        }
        private void BtnRRHH_Click(object sender, EventArgs e)
        {
            ConfigurarPanelesVisibles(BtnRRHH);
        }
        private void BtnRRHH_Docencia_Click(object sender, EventArgs e)
        {
            ConfigurarSubPanelesVisibles(BtnRRHH_Docencia);
        }
        private void BtnRRHH_Calendario_Click(object sender, EventArgs e)
        {

        }
        private void BtnRRHH_Planillas_Click(object sender, EventArgs e)
        {

        }
        private void BtnRRHH_MyProfile_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Mi Perfil de Trabajo";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Mi Perfil de Trabajo", "EmployeesInformation");
        }
        private void BtnProcesosAcademicos_PensumCarreras_Click(object sender, EventArgs e)
        {

        }
        private void BtnProcesosAcademicos_PensumCursos_Click(object sender, EventArgs e)
        {

        }
        private void BtnProcesosAcademicos_TarifasCursos_Click(object sender, EventArgs e)
        {

        }
        private void BtnProcesosAcademicos_RevisoresAprobados_Click(object sender, EventArgs e)
        {

        }
        private void BtnProcesosAcademicos_AprobacionDocentes_Click(object sender, EventArgs e)
        {

        }
        private void BtnProcesosAcademicos_PreasignacionCursos_Click(object sender, EventArgs e)
        {

        }
        private void BtnProcesosAcademicos_RevisionAsignaciones_Click(object sender, EventArgs e)
        {

        }
        private void BtnProcesosAcademicos_HorariosOficiales_Click(object sender, EventArgs e)
        {

        }
        private void BtnProcesosAcademicos_CalendariosAcademicos_Click(object sender, EventArgs e)
        {

        }
        #endregion EventosClickSubmenuNavegacion
        #region EventosClickSubSubmenuNavegacion
        private void BtnKardexInsumosSedes_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_KARDEX_WarehouseInventory frm = new Frm_KARDEX_WarehouseInventory();
            frm.Text = "Control de Movimientos E/S";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Control de Insumos en Sedes", "KardexInsumosSedes");
        }
        private void BtnKardexInventory_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_KARDEX_LocationsInventary frm = new Frm_KARDEX_LocationsInventary();
            frm.Text = "Control de Inventarios por Sede";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Control de Inventarios por Sede", "KardexInventory");
        }
        private void BtnKardexInventoryReport_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Reportes de Inventarios";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Reportes de Inventarios", "KardexInventoryReport");
        }
        private void BtnStaticItemsMaintenance_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Mantenimiento y Soporte";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Mantenimiento y Soporte", "StaticItemsMaintenance");
        }
        private void BtnStaticItemsManagment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            Frm_FixedAsset frm = new Frm_FixedAsset();
            frm.Text = "Catálogo General de Activos";
            frm.BackColor = Color.White;
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Catálogo General de Activos", "StaticItemsManagment");
        }
        private void BtnStaticItemsMovementsController_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_FixedAsset_Movements frm = new Frm_FixedAsset_Movements();
            frm.Text = "Traslados de Activos";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Traslados de Activos", "StaticItemsMovementsController");
        }
        private void BtnStaticItemsReports_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Reportes de Activos";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Reportes de Activos", "StaticItemsReports");
        }
        private void BtnStaticItemsResponsabilityLetter_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Cartas de Responsabilidad";
            frm.BackColor = Color.White;

            AbrirFormularioConPestana(frm, "Cartas de Responsabilidad", "StaticItemsResponsabilityLetter");
        }
        private void BtnStaticItemsCategories_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_FixedAssetCategories frm = new Frm_FixedAssetCategories();
            frm.Text = "Categoría de Activos";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Categoría de Activos", "StaticItemsCategories");
        }
        private void BtnKARDEX_ItemsManagment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_KARDEX_ItemsManagment frm = new Frm_KARDEX_ItemsManagment();
            frm.Text = "Gestión de Artículos";
            frm.BackColor = Color.White;

            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Gestión de Artículos", "KardexCatalogo");
        }
        private void BtnKARDEX_CatalogLocationsCategories_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_KARDEX_CatalogLocationsCategories frm = new Frm_KARDEX_CatalogLocationsCategories();
            frm.Text = "Catálogo de Artículos por Categorías";
            frm.BackColor = Color.White;

            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Catálogo de Artículos por Categorías", "CatalogLocationsCategories");
        }
        private void BtnWarehouse_Managment_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_Warehouse_Managment frm = new Frm_Warehouse_Managment();
            frm.Text = "Administrar Bodegas";
            frm.BackColor = Color.White;

            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Administrar Bodegas", "WarehouseManagment");
        }
        private void BtnWarehouse_Reports_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Form frm = new Form();
            frm.Text = "Reportería Bodegas";
            frm.BackColor = Color.White;

            //Pasamos los datos del usuario
            //frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Reportería Bodegas", "WarehouseReports");
        }
        private void BtnSuppliers_Click(object sender, EventArgs e)
        {
            ConfigurarSubPanelesVisibles(BtnSuppliers);
        }
        private void BtnFinances_Accounts_Click(object sender, EventArgs e)
        {
            ConfigurarSubPanelesVisibles(BtnFinances_Accounts);
        }
        private void BtnFinances_Checks_Click(object sender, EventArgs e)
        {
            ConfigurarSubPanelesVisibles(BtnFinances_Checks);
        }
        private void BtnFinances_Transfers_Click(object sender, EventArgs e)
        {
            ConfigurarSubPanelesVisibles(BtnFinances_Transfers);
        }
        private void BtnFinances_Banks_Click(object sender, EventArgs e)
        {
            ConfigurarSubPanelesVisibles(BtnFinances_Banks);
        }
        private void BtnFinances_AccountingBooks_Click(object sender, EventArgs e)
        {
            ConfigurarSubPanelesVisibles(BtnFinances_AccountingBooks);
        }
        private void BtnRRHH_Docencia_FichaCoordinador_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_RRHH_Coordinator_File frm = new Frm_RRHH_Coordinator_File();
            frm.Text = "Ficha del Coordinador";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;
            AbrirFormularioConPestana(frm, "Ficha del Coordinador", "CoordinatorManagment");
        }
        private void BtnRRHH_Trabajadores_Ficha_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_RRHH_Employee_File frm = new Frm_RRHH_Employee_File();
            frm.Text = "Ficha del Trabajador";
            frm.BackColor = Color.White;
            //Pasamos los datos del usuario
            frm.UserData = this.UserData;
            AbrirFormularioConPestana(frm, "Ficha del Trabajador", "EmployeesManagment");
        }
        private void BtnRRHH_Docencia_FichaDocente_Click(object sender, EventArgs e)
        {
            CerrarTodosLosPaneles();
            // Crear tu formulario específico (reemplaza con el formulario real)
            Frm_RRHH_Teacher_File frm = new Frm_RRHH_Teacher_File();
            frm.Text = "Ficha de Docentes";
            frm.BackColor = Color.White;

            //Pasamos los datos del usuario
            frm.UserData = this.UserData;

            AbrirFormularioConPestana(frm, "Ficha de Docentes", "TeachersManagment");

        }
        #endregion EventosClickSubSubmenuNavegacion
        #region EventoResizeFormulario
        private void Frm_ControlCenter_MDI_Resize(object sender, EventArgs e)
        {
            // Solo reposicionar si el panel está visible
            if (isPanelProfileVisible && PanelProfile.Visible)
            {
                PositionPanelProfile();
            }
        }
        #endregion EventoResizeFormulario
        #region GestionFormularioHijo
        // Inicializar el sistema de pestañas
        // En lugar de NavegadorTabConfig, usar TabControl estándar
        private TabControl tabControl;
        // Método para inicializar el sistema de pestañas
        private void InicializarSistemaPestanas()
        {
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Appearance = TabAppearance.Normal;
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.ItemSize = new Size(150, 25);
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += TabControl_DrawItem;
            tabControl.MouseClick += TabControl_MouseClick; // AGREGAR ESTA LÍNEA

            PanelContenedor.Controls.Add(tabControl);
        }
        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tc = sender as TabControl;
            if (tc == null) return;

            TabPage tabPage = tc.TabPages[e.Index];
            Rectangle tabRect = tc.GetTabRect(e.Index);
            bool isSelected = (e.Index == tc.SelectedIndex);

            // COLORES CON NARANJA PARA PESTAÑA ACTIVA
            Color backColor = isSelected ? Color.FromArgb(238, 143, 109) : Color.FromArgb(245, 245, 245); // Naranja vs gris
            Color textColor = Color.Black;  // Texto blanco en naranja, negro en gris

            // Resto del código igual...
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, tabRect);
            }

            // Área para texto y dibujo del texto (resto igual)
            Rectangle textRect = new Rectangle(
                tabRect.X + 8,
                tabRect.Y + 4,
                tabRect.Width - 30,
                tabRect.Height - 8);

            using (Font customFont = new Font("Segoe UI", 11F, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };

                e.Graphics.DrawString(tabPage.Text, customFont, textBrush, textRect, sf);
            }

            // Botón cerrar (X) - color ajustado para visibilidad
            Rectangle closeRect = new Rectangle(tabRect.Right - 20, tabRect.Y + 6, 14, 14);
            Color closeColor = isSelected ? Color.White : Color.Gray; // Blanco en pestaña activa
            using (Pen closePen = new Pen(closeColor, 2))
            {
                e.Graphics.DrawLine(closePen, closeRect.X + 3, closeRect.Y + 3, closeRect.Right - 3, closeRect.Bottom - 3);
                e.Graphics.DrawLine(closePen, closeRect.Right - 3, closeRect.Y + 3, closeRect.X + 3, closeRect.Bottom - 3);
            }
        }
        // MÉTODO PARA ABRIR FORMULARIO HIJO DENTRO DE UNA PESTAÑA
        public void AbrirFormularioConPestana(Form formHija, string titulo, string claveUnica)
        {
            // VALIDACIÓN: máximo 10 pestañas
            if (tabControl.TabPages.Count >= 10)
            {
                MessageBox.Show("No se pueden abrir más de 10 pestañas simultáneamente.",
                               "Límite de Pestañas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificar si ya existe
            foreach (TabPage existingTab in tabControl.TabPages)
            {
                if (existingTab.Name == claveUnica)
                {
                    tabControl.SelectedTab = existingTab;
                    return;
                }
            }

            // Crear nueva pestaña
            TabPage tabPage = new TabPage(titulo);
            tabPage.Name = claveUnica;
            formHija.TopLevel = false;
            formHija.FormBorderStyle = FormBorderStyle.None;
            formHija.Dock = DockStyle.Fill;
            tabPage.Controls.Add(formHija);
            tabControl.TabPages.Add(tabPage);
            tabControl.SelectedTab = tabPage;
            formHija.Show();
        }
        // EVENTO CLICK PARA DETECTAR CLICK EN X
        private void TabControl_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tabControl.TabPages.Count; i++)
            {
                Rectangle tabRect = tabControl.GetTabRect(i);
                Rectangle closeRect = new Rectangle(tabRect.Right - 20, tabRect.Y + 6, 14, 14);

                if (closeRect.Contains(e.Location))
                {
                    TabPage tabToClose = tabControl.TabPages[i];
                    Form formulario = tabToClose.Controls.OfType<Form>().FirstOrDefault();

                    // DETECTAR SI HAY PROCESOS ACTIVOS
                    bool hayProcesosActivos = DetectarProcesosActivos(formulario);

                    if (hayProcesosActivos)
                    {
                        // Mostrar mensaje solo si hay procesos activos
                        var result = MessageBox.Show(
                            "¿Deseas confirmar el cierre de la pestaña? Se perderán todos los avances.",
                            "Procesos en Ejecución",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (result != DialogResult.Yes)
                            return; // No cerrar si dice No
                    }

                    // Cerrar la pestaña
                    if (formulario != null)
                        formulario.Close();
                    tabControl.TabPages.RemoveAt(i);
                    break;
                }
            }
        }
        // MÉTODO PARA DETECTAR PROCESOS ACTIVOS EN EL FORMULARIO
        private bool DetectarProcesosActivos(Form formulario)
        {
            if (formulario == null) return false;

            // 1. Si es Frm_Home, nunca pedir confirmación
            if (formulario is Frm_Home)
                return false;

            // 2. Verificar si hay controles con datos modificados
            bool tieneControlesConDatos = false;
            foreach (Control control in ObtenerTodosLosControles(formulario))
            {
                if (control is TextBox txt && !string.IsNullOrWhiteSpace(txt.Text))
                    tieneControlesConDatos = true;
                if (control is ComboBox cmb && cmb.SelectedIndex > -1)
                    tieneControlesConDatos = true;
                if (control is CheckBox chk && chk.Checked)
                    tieneControlesConDatos = true;
                if (control is DataGridView dgv && dgv.Rows.Count > 0)
                    tieneControlesConDatos = true;
            }

            // 3. Verificar propiedad personalizada
            var tieneModificaciones = formulario.GetType().GetProperty("TieneModificacionesSinGuardar");
            if (tieneModificaciones != null)
            {
                return (bool)tieneModificaciones.GetValue(formulario);
            }

            // 4. SOLO mostrar confirmación si realmente hay datos o es un tipo específico
            if (tieneControlesConDatos)
                return true;

            // 5. Para formularios genéricos vacíos, NO pedir confirmación
            if (formulario.GetType() == typeof(Form) && formulario.Controls.Count <= 1)
                return false;

            return false; // Por defecto, no pedir confirmación
        }
        // MÉTODO AUXILIAR PARA OBTENER TODOS LOS CONTROLES
        private IEnumerable<Control> ObtenerTodosLosControles(Control contenedor)
        {
            List<Control> controles = new List<Control>();
            foreach (Control control in contenedor.Controls)
            {
                controles.Add(control);
                controles.AddRange(ObtenerTodosLosControles(control));
            }
            return controles;
        }
        // FUNCIÓN SIMPLE PARA AUTOAJUSTAR FORMULARIOS
        private void AutoajustarFormulario(Form formulario)
        {
            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Size = PanelContenedor.Size; // 1184, 855
            formulario.Location = new Point(0, 0);

            // Hacer que los controles principales se adapten
            foreach (Control control in formulario.Controls)
            {
                if (control is Panel && control.Dock == DockStyle.None)
                    control.Dock = DockStyle.Fill;
            }
        }
        #endregion GestionFormularioHijo
        #region EventoFormClosing
        // Bandera para controlar si ya se confirmó el cierre
        private bool cierreConfirmado = false;

        // Manejar el evento FormClosing
        private void Frm_ControlCenter_MDI_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Si ya se confirmó el cierre, permitir cerrar sin preguntar
            if (cierreConfirmado)
            {
                return; // Permitir el cierre sin hacer nada más
            }

            // Mostrar mensaje de confirmación
            DialogResult resultado = MessageBox.Show(
                "¿ESTÁS SEGURO QUE DESEAS CERRAR SESIÓN?",
                "CONFIRMAR CIERRE DE SESIÓN",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                // Usuario confirmó el cierre

                // Cerrar todos los formularios hijos primero
                foreach (Form childForm in this.MdiChildren)
                {
                    childForm.Close();
                }

                //   CERRAR TODAS LAS PESTAÑAS
                if (tabControl != null)
                {
                    while (tabControl.TabPages.Count > 0)
                    {
                        TabPage tab = tabControl.TabPages[0];
                        Form formulario = tab.Controls.OfType<Form>().FirstOrDefault();
                        if (formulario != null)
                            formulario.Close();
                        tabControl.TabPages.RemoveAt(0);
                    }
                }

                // Marcar que el cierre fue confirmado
                cierreConfirmado = true;

                //   OPCIÓN SIMPLE: Reiniciar la aplicación completa
                Application.Restart();
                Application.Exit();
            }
            else
            {
                // Usuario canceló el cierre
                e.Cancel = true;
            }
        }



        #endregion EventoFormClosing
    }
}