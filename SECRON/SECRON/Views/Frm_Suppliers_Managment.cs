using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SECRON.Views
{
    public partial class Frm_Suppliers_Managment : Form
    {
        #region PropiedadesIniciales
        // Variables Globales para mantener los filtros activos
        private string _ultimoTextoBusqueda = "";
        private string _ultimaClasificacion = "";
        private string _ultimoFiltroBancario = "TODOS";
        private List<Mdl_Suppliers> _listaCompletaFiltrada = null;
        public Mdl_Security_UserInfo UserData { get; set; }
        private Mdl_Suppliers _proveedorSeleccionado = null;
        private List<Mdl_Suppliers> proveedoresList;
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;
        private ToolStripLabel lblPaginaInfo;

        private void Frm_Suppliers_Managment_Load(object sender, EventArgs e)
        {
            ConfigurarTabIndexYFocus();
            try
            {
                this.Cursor = Cursors.WaitCursor;
                CrearToolStripPaginacion();
                CargarProveedores();
                ActualizarInfoPaginacion();
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                              "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ConfigurarPlaceHoldersTextbox();
            ConfigurarMaxLengthTextBox();
            CargarFiltros();
            InicializarScroll();
            ConfigurarEventosScroll();
            ConfigurarComboBoxes();

            // CARGAR CÓDIGO DE PROVEEDOR AUTOMÁTICO
            CargarProximoCodigoProveedor();
        }

        private void FormularioResize(object sender, EventArgs e)
        {
            if (Tabla != null && Tabla.DataSource != null)
            {
                Tabla.Refresh();
            }
        }

        public Frm_Suppliers_Managment()
        {
            InitializeComponent();
            this.Resize += FormularioResize;
            this.Resize += (s, e) => {
                if (toolStripPaginacion != null)
                {
                    toolStripPaginacion.Location = new Point(this.Width - 300, 225);
                }
            };
        }
        #endregion PropiedadesIniciales
        #region ConfigurarTextBox
        private void ConfigurarMaxLengthTextBox()
        {
            Txt_ValorBuscado.MaxLength = 100;
            Txt_Codigo.MaxLength = 50;
            Txt_SupplierName.MaxLength = 200;
            Txt_LegalName.MaxLength = 200;
            Txt_ContactName.MaxLength = 100;
            Txt_Correo.MaxLength = 100;
            Txt_Phone1.MaxLength = 12;
            Txt_Phone2.MaxLength = 12;
            Txt_Address.MaxLength = 300;
            Txt_ComercialActivity.MaxLength = 200;
            Txt_BankAccountNumber.MaxLength = 50;

            // ⭐ BLOQUEAR CÓDIGO DE PROVEEDOR (es automático)
            Txt_Codigo.Enabled = false;
            Txt_Codigo.BackColor = Color.FromArgb(240, 240, 240); // Gris claro
            Txt_Codigo.Cursor = Cursors.No;
        }

        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR POR NOMBRE, NIT, RAZÓN SOCIAL...");
            // ⭐ El Txt_Codigo NO lleva placeholder porque se carga automáticamente
            ConfigurarPlaceHolder(Txt_TaxId, "NIT (NÚMERO DE IDENTIFICACIÓN TRIBUTARIA)");
            ConfigurarPlaceHolder(Txt_SupplierName, "NOMBRE COMERCIAL");
            ConfigurarPlaceHolder(Txt_LegalName, "RAZÓN SOCIAL (NOMBRE LEGAL)");
            ConfigurarPlaceHolder(Txt_ContactName, "PERSONA DE CONTACTO");
            ConfigurarPlaceHolder(Txt_Correo, "CORREO ELECTRÓNICO");
            ConfigurarPlaceHolder(Txt_Phone1, "+502");
            ConfigurarPlaceHolder(Txt_Phone2, "+502");
            ConfigurarPlaceHolder(Txt_Address, "DIRECCIÓN COMPLETA");
            ConfigurarPlaceHolder(Txt_ComercialActivity, "ACTIVIDAD COMERCIAL");
            ConfigurarPlaceHolder(Txt_BankAccountNumber, "NÚMERO DE CUENTA BANCARIA");
        }

        private void ConfigurarPlaceHolder(TextBox textBox, string placeholder)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;
            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
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
        #region CodigoProveedorAutomatico
        // ⭐ MÉTODO PARA CARGAR EL PRÓXIMO CÓDIGO DE PROVEEDOR
        private void CargarProximoCodigoProveedor()
        {
            try
            {
                string proximoCodigo = Ctrl_Suppliers.ObtenerProximoCodigoProveedor();
                Txt_Codigo.Text = proximoCodigo;
                Txt_Codigo.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar código de proveedor: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Txt_Codigo.Text = "ERROR";
            }
        }
        #endregion CodigoProveedorAutomatico
        #region ConfigurarComboBox
        private void ConfigurarComboBoxes()
        {
            // ⭐ CONFIGURAR PARA QUE NO SE PUEDA ESCRIBIR (SOLO SELECCIONAR)
            ComboBox_Classification.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Banks.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro1.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro2.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;

            CargarClasificaciones();
            CargarBancos();
        }

        // ⭐ CARGAR CLASIFICACIONES COMPLETAS (según la imagen)
        private void CargarClasificaciones()
        {
            try
            {
                ComboBox_Classification.Items.Clear();
                ComboBox_Classification.Items.Add("NO ESPECIFICA");
                ComboBox_Classification.Items.Add("ARTÍCULOS DE OFICINA");
                ComboBox_Classification.Items.Add("EQUIPOS DE TECNOLOGÍA");
                ComboBox_Classification.Items.Add("SUMINISTROS DE LIMPIEZA");
                ComboBox_Classification.Items.Add("MOBILIARIO DE OFICINA");
                ComboBox_Classification.Items.Add("SERVICIOS DE MANTENIMIENTO");
                ComboBox_Classification.Items.Add("SERVICIOS DE CONSULTORÍA");
                ComboBox_Classification.Items.Add("SERVICIOS LOGÍSTICOS");
                ComboBox_Classification.Items.Add("MATERIALES DE CONSTRUCCIÓN");
                ComboBox_Classification.Items.Add("UNIFORMES Y VESTIMENTA");
                ComboBox_Classification.Items.Add("TRANSPORTE");
                ComboBox_Classification.Items.Add("SERVICIOS DE TELECOMUNICACIONES");
                ComboBox_Classification.Items.Add("ENERGÍA Y SUMINISTROS ELÉCTRICOS");
                ComboBox_Classification.Items.Add("SERVICIOS DE SEGURIDAD");
                ComboBox_Classification.Items.Add("PRODUCTOS DE PAPELERÍA");
                ComboBox_Classification.Items.Add("SERVICIOS FINANCIEROS");
                ComboBox_Classification.Items.Add("MARKETING Y PUBLICIDAD");
                ComboBox_Classification.Items.Add("SOFTWARE Y LICENCIAS");
                ComboBox_Classification.Items.Add("ALIMENTOS Y BEBIDAS");
                ComboBox_Classification.Items.Add("PRODUCTOS PROMOCIONALES");
                ComboBox_Classification.Items.Add("RECURSOS HUMANOS");
                ComboBox_Classification.Items.Add("HONORARIOS PROFESIONALES");
                ComboBox_Classification.Items.Add("HONORARIOS TÉCNICOS");
                ComboBox_Classification.Items.Add("OTROS HONORARIOS");

                if (ComboBox_Classification.Items.Count > 0)
                    ComboBox_Classification.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clasificaciones: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ⭐ CARGAR BANCOS DESDE LA BASE DE DATOS (usando Ctrl_Banks)
        private void CargarBancos()
        {
            try
            {
                ComboBox_Banks.Items.Clear();
                ComboBox_Banks.Items.Add("-- SIN BANCO --");

                // Obtener bancos desde la base de datos
                var bancos = Ctrl_Banks.ObtenerBancosParaCombo();

                foreach (var banco in bancos)
                {
                    ComboBox_Banks.Items.Add(banco.Value); // banco.Value es el BankName
                }

                if (ComboBox_Banks.Items.Count > 0)
                    ComboBox_Banks.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar bancos: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);

                // ⚠️ FALLBACK: Si falla la BD, cargar bancos de Guatemala por defecto
                ComboBox_Banks.Items.Clear();
                ComboBox_Banks.Items.Add("-- SIN BANCO --");
                ComboBox_Banks.Items.Add("BANCO INDUSTRIAL");
                ComboBox_Banks.Items.Add("BANCO G&T CONTINENTAL");
                ComboBox_Banks.Items.Add("BANCO DE DESARROLLO RURAL (BANRURAL)");
                ComboBox_Banks.Items.Add("BANCO AGROMERCANTIL");
                ComboBox_Banks.Items.Add("BANCO PROMERICA");
                ComboBox_Banks.Items.Add("BANCO DE LOS TRABAJADORES (BANTRAB)");
                ComboBox_Banks.Items.Add("BANCO DE AMÉRICA CENTRAL (BAC)");
                ComboBox_Banks.Items.Add("BANCO FICOHSA");
                ComboBox_Banks.Items.Add("BANCO AZTECA");

                if (ComboBox_Banks.Items.Count > 0)
                    ComboBox_Banks.SelectedIndex = 0;
            }
        }
        #endregion ConfigurarComboBox
        #region Filtros
        private void CargarFiltros()
        {
            Filtro1.Items.AddRange(new object[]
            {
                "TODOS",
                "POR NOMBRE",
                "POR RAZÓN SOCIAL",
                "POR NIT",
                "POR CLASIFICACIÓN"
            });
            Filtro1.SelectedIndex = 0;

            // ⭐ Filtro 2 - Clasificación (actualizado con las nuevas opciones)
            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS");
            Filtro2.Items.Add("NO ESPECIFICA");
            Filtro2.Items.Add("ARTÍCULOS DE OFICINA");
            Filtro2.Items.Add("EQUIPOS DE TECNOLOGÍA");
            Filtro2.Items.Add("SUMINISTROS DE LIMPIEZA");
            Filtro2.Items.Add("MOBILIARIO DE OFICINA");
            Filtro2.Items.Add("SERVICIOS DE MANTENIMIENTO");
            Filtro2.Items.Add("SERVICIOS DE CONSULTORÍA");
            Filtro2.Items.Add("SERVICIOS LOGÍSTICOS");
            Filtro2.Items.Add("MATERIALES DE CONSTRUCCIÓN");
            Filtro2.Items.Add("UNIFORMES Y VESTIMENTA");
            Filtro2.Items.Add("TRANSPORTE");
            Filtro2.Items.Add("SERVICIOS DE TELECOMUNICACIONES");
            Filtro2.Items.Add("ENERGÍA Y SUMINISTROS ELÉCTRICOS");
            Filtro2.Items.Add("SERVICIOS DE SEGURIDAD");
            Filtro2.Items.Add("PRODUCTOS DE PAPELERÍA");
            Filtro2.Items.Add("SERVICIOS FINANCIEROS");
            Filtro2.Items.Add("MARKETING Y PUBLICIDAD");
            Filtro2.Items.Add("SOFTWARE Y LICENCIAS");
            Filtro2.Items.Add("ALIMENTOS Y BEBIDAS");
            Filtro2.Items.Add("PRODUCTOS PROMOCIONALES");
            Filtro2.Items.Add("RECURSOS HUMANOS");
            Filtro2.Items.Add("HONORARIOS PROFESIONALES");
            Filtro2.Items.Add("HONORARIOS TÉCNICOS");
            Filtro2.Items.Add("OTROS HONORARIOS");
            Filtro2.SelectedIndex = 0;

            Filtro3.Items.AddRange(new object[]
            {
                "TODOS",
                "CON DATOS BANCARIOS",
                "SIN DATOS BANCARIOS"
            });
            Filtro3.SelectedIndex = 0;
        }
        #endregion Filtros
        #region Search
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string valorBusqueda = "";
                if (Txt_ValorBuscado.Text != "BUSCAR POR NOMBRE, NIT, RAZÓN SOCIAL..." &&
                    !string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                {
                    valorBusqueda = Txt_ValorBuscado.Text.Trim();
                }

                string clasificacion = "";
                if (Filtro2.SelectedItem?.ToString() != "TODOS")
                {
                    clasificacion = Filtro2.SelectedItem?.ToString() ?? "";
                }

                string filtroBancario = Filtro3.SelectedItem?.ToString() ?? "TODOS";

                // ⭐ GUARDAR FILTROS
                _ultimoTextoBusqueda = valorBusqueda;
                _ultimaClasificacion = clasificacion;
                _ultimoFiltroBancario = filtroBancario;

                // ⭐ REINICIAR A PÁGINA 1
                paginaActual = 1;

                // ⭐ SI HAY FILTRO BANCARIO, TRAER TODOS Y FILTRAR EN MEMORIA
                if (filtroBancario != "TODOS")
                {
                    // Traer TODOS los registros que cumplan los filtros de BD
                    List<Mdl_Suppliers> todosLosResultados = Ctrl_Suppliers.BuscarProveedores(
                        textoBusqueda: valorBusqueda,
                        classification: clasificacion,
                        pageNumber: 1,
                        pageSize: int.MaxValue
                    );

                    // Filtrar por datos bancarios
                    if (filtroBancario == "CON DATOS BANCARIOS")
                    {
                        todosLosResultados = todosLosResultados.Where(p => !string.IsNullOrWhiteSpace(p.BankAccountNumber)).ToList();
                    }
                    else if (filtroBancario == "SIN DATOS BANCARIOS")
                    {
                        todosLosResultados = todosLosResultados.Where(p => string.IsNullOrWhiteSpace(p.BankAccountNumber)).ToList();
                    }

                    // Guardar lista completa filtrada
                    _listaCompletaFiltrada = todosLosResultados;

                    // Aplicar paginación manual
                    int skip = (paginaActual - 1) * registrosPorPagina;
                    proveedoresList = todosLosResultados.Skip(skip).Take(registrosPorPagina).ToList();

                    // Actualizar contador total
                    totalRegistros = todosLosResultados.Count;
                }
                else
                {
                    // ⭐ SIN FILTRO BANCARIO: Búsqueda normal con paginación de BD
                    _listaCompletaFiltrada = null;

                    List<Mdl_Suppliers> resultados = Ctrl_Suppliers.BuscarProveedores(
                        textoBusqueda: valorBusqueda,
                        classification: clasificacion,
                        pageNumber: paginaActual,
                        pageSize: registrosPorPagina
                    );

                    proveedoresList = resultados;

                    // Contar total
                    totalRegistros = Ctrl_Suppliers.ContarTotalProveedores(
                        textoBusqueda: valorBusqueda,
                        classification: clasificacion
                    );
                }

                // Mostrar resultados
                Tabla.DataSource = proveedoresList;
                ConfigurarTabla();
                AjustarColumnas();

                // Calcular total de páginas
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                // ⭐ ACTUALIZAR LA INFORMACIÓN DE PAGINACIÓN
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

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR POR NOMBRE, NIT, RAZÓN SOCIAL...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Filtro1.SelectedIndex = 0;
            Filtro2.SelectedIndex = 0;
            Filtro3.SelectedIndex = 0;

            // ⭐ LIMPIAR FILTROS GUARDADOS
            _ultimoTextoBusqueda = "";
            _ultimaClasificacion = "";
            _ultimoFiltroBancario = "TODOS";
            _listaCompletaFiltrada = null;

            paginaActual = 1;
            RefrescarListado();
            ConfigurarTabla();
            AjustarColumnas();
            ActualizarInfoPaginacion();
        }
        #endregion Search
        #region vScrollBar
        private void Panel_Izquierdo_MouseEnter(object sender, EventArgs e)
        {
            Panel_Izquierdo.Focus();
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            int scrollPosition = vScrollBar.Value;

            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                {
                    ctrl.Tag = "OrigY:" + ctrl.Top;
                }

                string[] parts = ctrl.Tag.ToString().Split(':');
                int originalY = int.Parse(parts[1]);
                ctrl.Top = originalY - scrollPosition;
            }

            Panel_Izquierdo.Invalidate();
        }

        private void Panel_Izquierdo_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!vScrollBar.Visible) return;

            int delta = e.Delta / 120;
            int newValue = vScrollBar.Value - (delta * 30);

            if (newValue < 0) newValue = 0;
            if (newValue > vScrollBar.Maximum) newValue = vScrollBar.Maximum;

            vScrollBar.Value = newValue;
            MoverContenido(newValue);
        }

        private void MoverContenido(int scrollPosition)
        {
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                {
                    ctrl.Tag = "OrigY:" + ctrl.Top;
                }
                string[] parts = ctrl.Tag.ToString().Split(':');
                int originalY = int.Parse(parts[1]);
                ctrl.Top = originalY - scrollPosition;
            }
            Panel_Izquierdo.Invalidate();
        }

        private void ConfigurarEventosScroll()
        {
            Panel_Izquierdo.TabStop = true;
            Panel_Izquierdo.MouseWheel += Panel_Izquierdo_MouseWheel;
            Panel_Izquierdo.MouseEnter += Panel_Izquierdo_MouseEnter;

            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                ctrl.MouseWheel += Panel_Izquierdo_MouseWheel;
            }
        }

        private void InicializarScroll()
        {
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                {
                    ctrl.Tag = "OrigY:" + ctrl.Top;
                }
            }

            int maxBottom = 0;
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                maxBottom = Math.Max(maxBottom, ctrl.Bottom);
            }

            int totalContentHeight = maxBottom + (Panel_Izquierdo.Height / 3);

            if (totalContentHeight <= Panel_Izquierdo.Height)
            {
                vScrollBar.Visible = false;
                return;
            }

            vScrollBar.Visible = true;
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = totalContentHeight - Panel_Izquierdo.Height;
            vScrollBar.SmallChange = 30;
            vScrollBar.LargeChange = Panel_Izquierdo.Height / 4;

            vScrollBar.Scroll -= vScrollBar_Scroll;
            vScrollBar.Scroll += vScrollBar_Scroll;
            vScrollBar.Value = 0;
        }
        #endregion vScrollBar
        #region ConfiguracionesTabla
        private void CargarProveedores()
        {
            try
            {
                RefrescarListado();
                ConfigurarTabla();
                AjustarColumnas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar proveedores: {ex.Message}");
            }
        }

        public void RefrescarListado()
        {
            proveedoresList = Ctrl_Suppliers.MostrarProveedores(paginaActual, registrosPorPagina);
            Tabla.DataSource = proveedoresList;
        }

        public void ConfigurarTabla()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["SupplierCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["SupplierName"].HeaderText = "NOMBRE COMERCIAL";
                Tabla.Columns["LegalName"].HeaderText = "RAZÓN SOCIAL";
                Tabla.Columns["TaxId"].HeaderText = "NIT";
                Tabla.Columns["Phone"].HeaderText = "TELÉFONO";
                Tabla.Columns["Classification"].HeaderText = "CLASIFICACIÓN";

                Tabla.Columns["SupplierId"].Visible = false;
                Tabla.Columns["ContactName"].Visible = false;
                Tabla.Columns["Phone2"].Visible = false;
                Tabla.Columns["Email"].Visible = false;
                Tabla.Columns["Address"].Visible = false;
                Tabla.Columns["CommercialActivity"].Visible = false;
                Tabla.Columns["BankAccountNumber"].Visible = false;
                Tabla.Columns["BankName"].Visible = false;
                Tabla.Columns["IsActive"].Visible = false;
                Tabla.Columns["CreatedDate"].Visible = false;
                Tabla.Columns["CreatedBy"].Visible = false;
                Tabla.Columns["ModifiedDate"].Visible = false;
                Tabla.Columns["ModifiedBy"].Visible = false;
            }

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToResizeRows = false;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void CargarDatosProveedorSeleccionado()
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0) return;

                DataGridViewRow fila = Tabla.SelectedRows[0];
                int supplierId = Convert.ToInt32(fila.Cells["SupplierId"].Value);
                _proveedorSeleccionado = proveedoresList.FirstOrDefault(p => p.SupplierId == supplierId);

                if (_proveedorSeleccionado != null)
                {
                    // ===== CÓDIGO (SIEMPRE TIENE VALOR) =====
                    Txt_Codigo.Text = _proveedorSeleccionado.SupplierCode ?? "";
                    Txt_Codigo.ForeColor = Color.Black;

                    // ===== NOMBRE COMERCIAL (OBLIGATORIO) =====
                    Txt_SupplierName.Text = _proveedorSeleccionado.SupplierName;
                    Txt_SupplierName.ForeColor = Color.Black;

                    // ===== RAZÓN SOCIAL (OBLIGATORIO) =====
                    Txt_LegalName.Text = _proveedorSeleccionado.LegalName;
                    Txt_LegalName.ForeColor = Color.Black;

                    // ===== NIT (OPCIONAL) =====
                    if (!string.IsNullOrWhiteSpace(_proveedorSeleccionado.TaxId))
                    {
                        Txt_TaxId.Text = _proveedorSeleccionado.TaxId;
                        Txt_TaxId.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_TaxId.Text = "NIT (NÚMERO DE IDENTIFICACIÓN TRIBUTARIA)";
                        Txt_TaxId.ForeColor = Color.Gray;
                    }

                    // ===== PERSONA DE CONTACTO (OPCIONAL) =====
                    if (!string.IsNullOrWhiteSpace(_proveedorSeleccionado.ContactName))
                    {
                        Txt_ContactName.Text = _proveedorSeleccionado.ContactName;
                        Txt_ContactName.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_ContactName.Text = "PERSONA DE CONTACTO";
                        Txt_ContactName.ForeColor = Color.Gray;
                    }

                    // ===== EMAIL (OPCIONAL) =====
                    if (!string.IsNullOrWhiteSpace(_proveedorSeleccionado.Email))
                    {
                        Txt_Correo.Text = _proveedorSeleccionado.Email;
                        Txt_Correo.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_Correo.Text = "CORREO ELECTRÓNICO";
                        Txt_Correo.ForeColor = Color.Gray;
                    }

                    // ===== TELÉFONO 1 (OBLIGATORIO) =====
                    if (!string.IsNullOrWhiteSpace(_proveedorSeleccionado.Phone))
                    {
                        Txt_Phone1.Text = _proveedorSeleccionado.Phone;
                        Txt_Phone1.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_Phone1.Text = "+502";
                        Txt_Phone1.ForeColor = Color.Gray;
                    }

                    // ===== TELÉFONO 2 (OPCIONAL) =====
                    if (!string.IsNullOrWhiteSpace(_proveedorSeleccionado.Phone2))
                    {
                        Txt_Phone2.Text = _proveedorSeleccionado.Phone2;
                        Txt_Phone2.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_Phone2.Text = "+502";
                        Txt_Phone2.ForeColor = Color.Gray;
                    }

                    // ===== DIRECCIÓN (OPCIONAL) =====
                    if (!string.IsNullOrWhiteSpace(_proveedorSeleccionado.Address))
                    {
                        Txt_Address.Text = _proveedorSeleccionado.Address;
                        Txt_Address.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_Address.Text = "DIRECCIÓN COMPLETA";
                        Txt_Address.ForeColor = Color.Gray;
                    }

                    // ===== ACTIVIDAD COMERCIAL (OBLIGATORIO) =====
                    Txt_ComercialActivity.Text = _proveedorSeleccionado.CommercialActivity;
                    Txt_ComercialActivity.ForeColor = Color.Black;

                    // ===== CLASIFICACIÓN (OBLIGATORIO) =====
                    if (!string.IsNullOrWhiteSpace(_proveedorSeleccionado.Classification))
                    {
                        int indexClasificacion = ComboBox_Classification.Items.IndexOf(_proveedorSeleccionado.Classification);
                        if (indexClasificacion >= 0)
                            ComboBox_Classification.SelectedIndex = indexClasificacion;
                        else
                            ComboBox_Classification.SelectedIndex = 0;
                    }
                    else
                    {
                        ComboBox_Classification.SelectedIndex = 0;
                    }

                    // ===== BANCO (OPCIONAL) =====
                    if (!string.IsNullOrWhiteSpace(_proveedorSeleccionado.BankName))
                    {
                        int indexBanco = ComboBox_Banks.Items.IndexOf(_proveedorSeleccionado.BankName);
                        if (indexBanco >= 0)
                            ComboBox_Banks.SelectedIndex = indexBanco;
                        else
                            ComboBox_Banks.SelectedIndex = 0; // "SIN BANCO"
                    }
                    else
                    {
                        ComboBox_Banks.SelectedIndex = 0; // "SIN BANCO"
                    }

                    // ===== NÚMERO DE CUENTA BANCARIA (OPCIONAL) =====
                    if (!string.IsNullOrWhiteSpace(_proveedorSeleccionado.BankAccountNumber))
                    {
                        Txt_BankAccountNumber.Text = _proveedorSeleccionado.BankAccountNumber;
                        Txt_BankAccountNumber.ForeColor = Color.Black;
                    }
                    else
                    {
                        Txt_BankAccountNumber.Text = "NÚMERO DE CUENTA BANCARIA";
                        Txt_BankAccountNumber.ForeColor = Color.Gray;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del proveedor: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void AjustarColumnas()
        {
            Tabla.Columns["SupplierCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["SupplierName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["LegalName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["TaxId"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["Phone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["Classification"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count > 0)
            {
                CargarDatosProveedorSeleccionado();
            }
        }
        #endregion ConfiguracionesTabla
        #region ToolsStrip
        private void CrearToolStripPaginacion()
        {
            toolStripPaginacion = new ToolStrip();
            toolStripPaginacion.Dock = DockStyle.None;
            toolStripPaginacion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            toolStripPaginacion.GripStyle = ToolStripGripStyle.Hidden;
            toolStripPaginacion.BackColor = Color.FromArgb(248, 249, 250);
            toolStripPaginacion.Height = 40;
            toolStripPaginacion.AutoSize = true;
            toolStripPaginacion.Location = new Point(this.Width - 400, 225);

            btnAnterior = new ToolStripButton();
            btnAnterior.Text = "❮ Anterior";
            btnAnterior.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnterior.ForeColor = Color.White;
            btnAnterior.BackColor = Color.FromArgb(51, 140, 255);
            btnAnterior.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAnterior.Margin = new Padding(2);
            btnAnterior.Padding = new Padding(8, 4, 8, 4);
            btnAnterior.Click += (s, e) => CambiarPagina(paginaActual - 1);

            toolStripPaginacion.Items.Add(btnAnterior);
            ActualizarBotonesNumerados();

            btnSiguiente = new ToolStripButton();
            btnSiguiente.Text = "Siguiente ❯";
            btnSiguiente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSiguiente.ForeColor = Color.White;
            btnSiguiente.BackColor = Color.FromArgb(238, 143, 109);
            btnSiguiente.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSiguiente.Margin = new Padding(2);
            btnSiguiente.Padding = new Padding(8, 4, 8, 4);
            btnSiguiente.Click += (s, e) => CambiarPagina(paginaActual + 1);

            toolStripPaginacion.Items.Add(btnSiguiente);

            this.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.BringToFront();
        }

        private void ActualizarBotonesNumerados()
        {
            var itemsToRemove = toolStripPaginacion.Items.Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton").ToList();

            foreach (var item in itemsToRemove)
            {
                toolStripPaginacion.Items.Remove(item);
            }

            if (totalPaginas <= 1) return;

            int inicioRango = Math.Max(1, paginaActual - 1);
            int finRango = Math.Min(totalPaginas, paginaActual + 1);

            int posicionInsertar = toolStripPaginacion.Items.IndexOf(btnSiguiente);

            for (int i = inicioRango; i <= finRango; i++)
            {
                ToolStripButton btnPagina = new ToolStripButton();
                btnPagina.Text = i.ToString();
                btnPagina.Tag = "PageButton";
                btnPagina.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnPagina.Margin = new Padding(1);
                btnPagina.Padding = new Padding(6, 4, 6, 4);

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
            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginas)
            {
                paginaActual = nuevaPagina;

                // ⭐ SI HAY FILTRO BANCARIO ACTIVO, USAR LISTA FILTRADA EN MEMORIA
                if (_listaCompletaFiltrada != null)
                {
                    int skip = (paginaActual - 1) * registrosPorPagina;
                    proveedoresList = _listaCompletaFiltrada.Skip(skip).Take(registrosPorPagina).ToList();
                    Tabla.DataSource = proveedoresList;
                }
                // ⭐ SI HAY OTROS FILTROS ACTIVOS (sin bancario), USAR BÚSQUEDA DE BD
                else if (!string.IsNullOrEmpty(_ultimoTextoBusqueda) || !string.IsNullOrEmpty(_ultimaClasificacion))
                {
                    proveedoresList = Ctrl_Suppliers.BuscarProveedores(
                        textoBusqueda: _ultimoTextoBusqueda,
                        classification: _ultimaClasificacion,
                        pageNumber: paginaActual,
                        pageSize: registrosPorPagina
                    );
                    Tabla.DataSource = proveedoresList;
                }
                else
                {
                    // Sin filtros: Cargar normalmente
                    RefrescarListado();
                }

                ConfigurarTabla();
                AjustarColumnas();
                ActualizarInfoPaginacion();
            }
        }

        private void ActualizarInfoPaginacion()
        {
            // ⭐ SI NO HAY FILTROS ACTIVOS Y totalRegistros es 0, CALCULAR
            if (string.IsNullOrEmpty(_ultimoTextoBusqueda) &&
                string.IsNullOrEmpty(_ultimaClasificacion) &&
                _ultimoFiltroBancario == "TODOS" &&
                totalRegistros == 0)
            {
                totalRegistros = Ctrl_Suppliers.ContarTotalProveedores();
            }

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;

            ActualizarBotonesNumerados();

            int inicioRango = (paginaActual - 1) * registrosPorPagina + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            if (Lbl_Paginas != null)
            {
                if (totalRegistros == 0)
                {
                    Lbl_Paginas.Text = "NO HAY PROVEEDORES PARA MOSTRAR";
                }
                else
                {
                    Lbl_Paginas.Text = $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} PROVEEDORES";
                }
            }
        }
        #endregion ToolsStrip
        #region ValidarCamposObligatorios
        private bool ValidarCamposObligatorios()
        {
            bool TienePlaceholder(TextBox txt, string placeholder)
            {
                return string.IsNullOrWhiteSpace(txt.Text) || txt.Text == placeholder || txt.ForeColor == Color.Gray;
            }

            if (TienePlaceholder(Txt_SupplierName, "NOMBRE COMERCIAL"))
            {
                MessageBox.Show("El campo NOMBRE COMERCIAL es obligatorio", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_SupplierName.Focus();
                return false;
            }

            if (TienePlaceholder(Txt_LegalName, "RAZÓN SOCIAL (NOMBRE LEGAL)"))
            {
                MessageBox.Show("El campo RAZÓN SOCIAL es obligatorio", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_LegalName.Focus();
                return false;
            }

            //if (TienePlaceholder(Txt_Phone1, "+502"))
            //{
            //    MessageBox.Show("El campo TELÉFONO es obligatorio", "Validación",
            //                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    Txt_Phone1.Focus();
            //    return false;
            //}

            //if (!TienePlaceholder(Txt_Phone1, "+502") && Txt_Phone1.Text.Length != 12)
            //{
            //    MessageBox.Show("El TELÉFONO debe tener el formato +502 seguido de 8 dígitos", "Validación",
            //                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    Txt_Phone1.Focus();
            //    return false;
            //}

            if (TienePlaceholder(Txt_ComercialActivity, "ACTIVIDAD COMERCIAL"))
            {
                MessageBox.Show("El campo ACTIVIDAD COMERCIAL es obligatorio", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_ComercialActivity.Focus();
                return false;
            }

            if (ComboBox_Classification.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar una CLASIFICACIÓN", "Validación",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ComboBox_Classification.Focus();
                return false;
            }

            //if (!TienePlaceholder(Txt_Correo, "CORREO ELECTRÓNICO") &&
            //    !Txt_Correo.Text.Contains("@"))
            //{
            //    MessageBox.Show("El CORREO ELECTRÓNICO debe tener un formato válido", "Validación",
            //                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    Txt_Correo.Focus();
            //    return false;
            //}

            return true;
        }
        #endregion ValidarCamposObligatorios
        #region AsignacionFocus
        private void ConfigurarTabIndexYFocus()
        {
            Txt_ValorBuscado.TabIndex = 0;
            Filtro1.TabIndex = 1;
            Filtro2.TabIndex = 2;
            Filtro3.TabIndex = 3;
            Txt_Codigo.TabIndex = 5;
            Txt_TaxId.TabIndex = 6;  // ⭐ NIT
            Txt_SupplierName.TabIndex = 7;
            Txt_LegalName.TabIndex = 8;
            Txt_ContactName.TabIndex = 9;
            Txt_Correo.TabIndex = 10;
            Txt_Phone1.TabIndex = 11;
            Txt_Phone2.TabIndex = 12;
            Txt_Address.TabIndex = 13;
            Txt_ComercialActivity.TabIndex = 14;
            ComboBox_Classification.TabIndex = 15;
            ComboBox_Banks.TabIndex = 16;
            Txt_BankAccountNumber.TabIndex = 17;
            Btn_Save.TabIndex = 18;
            Btn_Update.TabIndex = 19;
            Btn_Inactive.TabIndex = 20;

            Txt_ValorBuscado.Focus();
        }

        private void EstablecerFocoPrimerCampo()
        {
            Txt_TaxId.Focus(); // ⭐ El código es automático, el foco va al NIT
        }

        private void EstablecerFocoBusqueda()
        {
            Txt_ValorBuscado.Focus();
        }
        #endregion AsignacionFocus
        #region CRUD
        private Mdl_Suppliers ConvertirProveedorAMayusculas(Mdl_Suppliers proveedor)
        {
            if (proveedor == null) return null;

            proveedor.SupplierCode = proveedor.SupplierCode?.ToUpper();
            proveedor.SupplierName = proveedor.SupplierName?.ToUpper();
            proveedor.LegalName = proveedor.LegalName?.ToUpper();
            proveedor.TaxId = proveedor.TaxId?.ToUpper();
            proveedor.ContactName = proveedor.ContactName?.ToUpper();
            proveedor.Phone = proveedor.Phone?.ToUpper();
            proveedor.Phone2 = proveedor.Phone2?.ToUpper();
            proveedor.Email = proveedor.Email?.ToUpper();
            proveedor.Address = proveedor.Address?.ToUpper();
            proveedor.CommercialActivity = proveedor.CommercialActivity?.ToUpper();
            proveedor.Classification = proveedor.Classification?.ToUpper();
            proveedor.BankAccountNumber = proveedor.BankAccountNumber?.ToUpper();
            proveedor.BankName = proveedor.BankName?.ToUpper();

            return proveedor;
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCamposObligatorios())
                    return;

                var confirmacion = MessageBox.Show(
                    "¿Está seguro que desea registrar este proveedor?",
                    "Confirmar Registro",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                bool TienePlaceholder(TextBox txt, string placeholder)
                {
                    return string.IsNullOrWhiteSpace(txt.Text) || txt.Text == placeholder || txt.ForeColor == Color.Gray;
                }

                // ⭐ OBTENER EL CÓDIGO FRESCO DE LA BD JUSTO ANTES DE GUARDAR
                string codigoFresco = Ctrl_Suppliers.ObtenerProximoCodigoProveedor();

                var nuevoProveedor = new Mdl_Suppliers
                {
                    // ⭐ Usar el código fresco recién obtenido
                    SupplierCode = codigoFresco,
                    SupplierName = Txt_SupplierName.Text.Trim(),
                    LegalName = Txt_LegalName.Text.Trim(),
                    Phone = Txt_Phone1.Text.Trim(),
                    CommercialActivity = Txt_ComercialActivity.Text.Trim(),
                    Classification = ComboBox_Classification.SelectedItem?.ToString() ?? "",

                    TaxId = !TienePlaceholder(Txt_Codigo, "CÓDIGO PROVEEDOR")
                        ? Txt_Codigo.Text.Trim() : null,
                    ContactName = !TienePlaceholder(Txt_ContactName, "PERSONA DE CONTACTO")
                        ? Txt_ContactName.Text.Trim() : null,
                    Email = !TienePlaceholder(Txt_Correo, "CORREO ELECTRÓNICO")
                        ? Txt_Correo.Text.Trim() : null,
                    Phone2 = !TienePlaceholder(Txt_Phone2, "+502")
                        ? Txt_Phone2.Text.Trim() : null,
                    Address = !TienePlaceholder(Txt_Address, "DIRECCIÓN COMPLETA")
                        ? Txt_Address.Text.Trim() : null,
                    BankName = ComboBox_Banks.SelectedIndex > 0
                        ? ComboBox_Banks.SelectedItem?.ToString() : null,
                    BankAccountNumber = !TienePlaceholder(Txt_BankAccountNumber, "NÚMERO DE CUENTA BANCARIA")
                        ? Txt_BankAccountNumber.Text.Trim() : null,

                    IsActive = true,
                    CreatedBy = UserData?.UserId ?? 1
                };

                nuevoProveedor = ConvertirProveedorAMayusculas(nuevoProveedor);

                int resultado = Ctrl_Suppliers.RegistrarProveedor(nuevoProveedor);

                if (resultado > 0)
                {
                    MessageBox.Show("Proveedor registrado exitosamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ActualizarInfoPaginacion();

                    // ⭐ Cargar el próximo código después de registrar exitosamente
                    CargarProximoCodigoProveedor();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el proveedor", "Error",
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
                if (_proveedorSeleccionado == null || _proveedorSeleccionado.SupplierId == 0)
                {
                    MessageBox.Show("Debe seleccionar un proveedor para actualizar", "Validación",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarCamposObligatorios())
                    return;

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea actualizar los datos de {_proveedorSeleccionado.SupplierName}?",
                    "Confirmar Actualización",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                bool TienePlaceholder(TextBox txt, string placeholder)
                {
                    return string.IsNullOrWhiteSpace(txt.Text) || txt.Text == placeholder || txt.ForeColor == Color.Gray;
                }

                _proveedorSeleccionado.SupplierCode = Txt_Codigo.Text.Trim();
                _proveedorSeleccionado.SupplierName = Txt_SupplierName.Text.Trim();
                _proveedorSeleccionado.LegalName = Txt_LegalName.Text.Trim();
                _proveedorSeleccionado.Phone = Txt_Phone1.Text.Trim();
                _proveedorSeleccionado.CommercialActivity = Txt_ComercialActivity.Text.Trim();
                _proveedorSeleccionado.Classification = ComboBox_Classification.SelectedItem?.ToString() ?? "";

                _proveedorSeleccionado.TaxId = !TienePlaceholder(Txt_TaxId, "NIT")
                    ? Txt_TaxId.Text.Trim() : null;
                _proveedorSeleccionado.ContactName = !TienePlaceholder(Txt_ContactName, "PERSONA DE CONTACTO")
                    ? Txt_ContactName.Text.Trim() : null;
                _proveedorSeleccionado.Email = !TienePlaceholder(Txt_Correo, "CORREO ELECTRÓNICO")
                    ? Txt_Correo.Text.Trim() : null;
                _proveedorSeleccionado.Phone2 = !TienePlaceholder(Txt_Phone2, "+502")
                    ? Txt_Phone2.Text.Trim() : null;
                _proveedorSeleccionado.Address = !TienePlaceholder(Txt_Address, "DIRECCIÓN COMPLETA")
                    ? Txt_Address.Text.Trim() : null;
                _proveedorSeleccionado.BankName = ComboBox_Banks.SelectedIndex > 0
                    ? ComboBox_Banks.SelectedItem?.ToString() : null;
                _proveedorSeleccionado.BankAccountNumber = !TienePlaceholder(Txt_BankAccountNumber, "NÚMERO DE CUENTA BANCARIA")
                    ? Txt_BankAccountNumber.Text.Trim() : null;

                _proveedorSeleccionado.ModifiedBy = UserData?.UserId ?? 1;

                _proveedorSeleccionado = ConvertirProveedorAMayusculas(_proveedorSeleccionado);

                int resultado = Ctrl_Suppliers.ActualizarProveedor(_proveedorSeleccionado);

                if (resultado > 0)
                {
                    MessageBox.Show("Proveedor actualizado exitosamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ActualizarInfoPaginacion();

                    // ⭐ Cargar el próximo código después de actualizar exitosamente
                    CargarProximoCodigoProveedor();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el proveedor", "Error",
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
                if (_proveedorSeleccionado == null || _proveedorSeleccionado.SupplierId == 0)
                {
                    MessageBox.Show("Debe seleccionar un proveedor para inactivar", "Validación",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!_proveedorSeleccionado.IsActive)
                {
                    MessageBox.Show("Este proveedor ya se encuentra inactivo", "Información",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea INACTIVAR a {_proveedorSeleccionado.SupplierName}?\n\n" +
                    "El proveedor no aparecerá en las listas activas pero sus datos se conservarán.",
                    "Confirmar Inactivación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                int modifiedBy = UserData?.UserId ?? 1;
                int resultado = Ctrl_Suppliers.InactivarProveedor(_proveedorSeleccionado.SupplierId, modifiedBy);

                if (resultado > 0)
                {
                    MessageBox.Show("Proveedor inactivado exitosamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    RefrescarListado();
                    ActualizarInfoPaginacion();

                    // ⭐ Cargar el próximo código después de inactivar exitosamente
                    CargarProximoCodigoProveedor();
                }
                else
                {
                    MessageBox.Show("No se pudo inactivar el proveedor", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inactivar: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion CRUD
        #region Limpieza
        // ⭐ MÉTODO DE LIMPIEZA CORREGIDO: Vuelve a cargar el próximo código
        private void LimpiarFormulario()
        {
            _proveedorSeleccionado = null;
            ConfigurarPlaceHoldersTextbox();

            if (ComboBox_Classification.Items.Count > 0)
                ComboBox_Classification.SelectedIndex = 0;

            if (ComboBox_Banks.Items.Count > 0)
                ComboBox_Banks.SelectedIndex = 0;

            // ⭐ RECARGAR EL PRÓXIMO CÓDIGO DE PROVEEDOR
            CargarProximoCodigoProveedor();

            Txt_TaxId.Focus(); // ⭐ Foco en el primer campo editable (NIT)
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }
        #endregion Limpieza
        #region ExportarExcel
        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                // ⭐ OBTENER REGISTROS SEGÚN FILTROS ACTIVOS
                List<Mdl_Suppliers> todosLosProveedores;

                // ⭐ SI HAY FILTROS ACTIVOS, USAR LA LISTA FILTRADA
                if (_listaCompletaFiltrada != null)
                {
                    // Ya tenemos la lista filtrada en memoria (con filtro bancario)
                    todosLosProveedores = _listaCompletaFiltrada;
                }
                else if (!string.IsNullOrEmpty(_ultimoTextoBusqueda) || !string.IsNullOrEmpty(_ultimaClasificacion))
                {
                    // Hay filtros de BD activos (búsqueda o clasificación)
                    todosLosProveedores = Ctrl_Suppliers.BuscarProveedores(
                        textoBusqueda: _ultimoTextoBusqueda,
                        classification: _ultimaClasificacion,
                        pageNumber: 1,
                        pageSize: int.MaxValue
                    );
                }
                else
                {
                    // Sin filtros: exportar todos los proveedores
                    todosLosProveedores = Ctrl_Suppliers.BuscarProveedores(
                        pageNumber: 1,
                        pageSize: int.MaxValue
                    );
                }

                if (todosLosProveedores == null || todosLosProveedores.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar", "Información",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Lista de Proveedores",
                    FileName = $"Proveedores_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    var excelApp = new Excel.Application();
                    var workbook = excelApp.Workbooks.Add();
                    var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Proveedores";

                    // ============ ENCABEZADO PRINCIPAL ============
                    worksheet.Cells[1, 1] = "REPORTE COMPLETO DE PROVEEDORES";
                    worksheet.Range["A1:M1"].Merge();
                    worksheet.Range["A1:M1"].Font.Size = 16;
                    worksheet.Range["A1:M1"].Font.Bold = true;
                    worksheet.Range["A1:M1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Range["A1:M1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(238, 143, 109));
                    worksheet.Range["A1:M1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                    // ============ INFORMACIÓN DEL REPORTE ============
                    worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SISTEMA"}";
                    worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {todosLosProveedores.Count}";

                    worksheet.Range["A2:A4"].Font.Size = 10;
                    worksheet.Range["A2:A4"].Font.Bold = true;

                    // ============ ENCABEZADOS DE COLUMNAS (TODAS) ============
                    int headerRow = 6;
                    string[] headers = {
                "CÓDIGO",                    // 1
                "NOMBRE COMERCIAL",          // 2
                "RAZÓN SOCIAL",              // 3
                "NIT",                       // 4
                "PERSONA CONTACTO",          // 5
                "EMAIL",                     // 6
                "TELÉFONO 1",                // 7
                "TELÉFONO 2",                // 8
                "DIRECCIÓN",                 // 9
                "ACTIVIDAD COMERCIAL",       // 10
                "CLASIFICACIÓN",             // 11
                "BANCO",                     // 12
                "NO. CUENTA BANCARIA"        // 13
            };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[headerRow, i + 1] = headers[i];
                    }

                    // Estilo de encabezados
                    var headerRange = worksheet.Range[$"A{headerRow}:M{headerRow}"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Size = 11;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    // ============ DATOS ============
                    int row = headerRow + 1;
                    foreach (var proveedor in todosLosProveedores)
                    {
                        // 1. CÓDIGO
                        worksheet.Cells[row, 1] = proveedor.SupplierCode ?? "N/A";

                        // 2. NOMBRE COMERCIAL
                        worksheet.Cells[row, 2] = proveedor.SupplierName ?? "";

                        // 3. RAZÓN SOCIAL
                        worksheet.Cells[row, 3] = proveedor.LegalName ?? "";

                        // 4. NIT
                        worksheet.Cells[row, 4] = proveedor.TaxId ?? "N/A";

                        // 5. PERSONA CONTACTO
                        worksheet.Cells[row, 5] = proveedor.ContactName ?? "N/A";

                        // 6. EMAIL
                        worksheet.Cells[row, 6] = proveedor.Email ?? "N/A";

                        // 7. TELÉFONO 1
                        worksheet.Cells[row, 7] = proveedor.Phone ?? "N/A";

                        // 8. TELÉFONO 2
                        worksheet.Cells[row, 8] = proveedor.Phone2 ?? "N/A";

                        // 9. DIRECCIÓN
                        worksheet.Cells[row, 9] = proveedor.Address ?? "N/A";

                        // 10. ACTIVIDAD COMERCIAL
                        worksheet.Cells[row, 10] = proveedor.CommercialActivity ?? "N/A";

                        // 11. CLASIFICACIÓN
                        worksheet.Cells[row, 11] = proveedor.Classification ?? "NO ESPECIFICA";

                        // 12. BANCO
                        worksheet.Cells[row, 12] = proveedor.BankName ?? "SIN BANCO";

                        // 13. NO. CUENTA BANCARIA
                        worksheet.Cells[row, 13] = proveedor.BankAccountNumber ?? "N/A";

                        // Alternar color de filas
                        if (row % 2 == 0)
                        {
                            worksheet.Range[$"A{row}:M{row}"].Interior.Color =
                                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                        }

                        row++;
                    }

                    // ============ FORMATO FINAL ============
                    var dataRange = worksheet.Range[$"A{headerRow}:M{row - 1}"];
                    dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                    // Autoajustar columnas
                    worksheet.Columns.AutoFit();

                    // Ajustar ancho específico para columnas largas
                    worksheet.Columns[2].ColumnWidth = 30;  // Nombre Comercial
                    worksheet.Columns[3].ColumnWidth = 35;  // Razón Social
                    worksheet.Columns[6].ColumnWidth = 30;  // Email
                    worksheet.Columns[9].ColumnWidth = 40;  // Dirección
                    worksheet.Columns[10].ColumnWidth = 30; // Actividad Comercial

                    // Congelar paneles en encabezado
                    worksheet.Activate();
                    excelApp.ActiveWindow.SplitRow = headerRow;
                    excelApp.ActiveWindow.FreezePanes = true;

                    // ============ PIE DE PÁGINA ============
                    worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
                    worksheet.Range[$"A{row + 1}:M{row + 1}"].Merge();
                    worksheet.Range[$"A{row + 1}:M{row + 1}"].Font.Italic = true;
                    worksheet.Range[$"A{row + 1}:M{row + 1}"].Font.Size = 9;
                    worksheet.Range[$"A{row + 1}:M{row + 1}"].HorizontalAlignment =
                        Excel.XlHAlign.xlHAlignCenter;

                    // Guardar archivo
                    workbook.SaveAs(saveFileDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    // Liberar objetos COM
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
        #endregion ExportarExcel
    }
}