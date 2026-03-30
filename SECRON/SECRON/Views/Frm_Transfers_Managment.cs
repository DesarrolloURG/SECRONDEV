using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SECRON.Views.Frm_Checks_Managment;

namespace SECRON.Views
{
    public partial class Frm_Transfers_Managment : Form
    {
        #region PropiedadesIniciales
        public Mdl_Security_UserInfo UserData { get; set; }

        // Transfer seleccionada para editar
        private Mdl_Transfers _transferSeleccionada = null;

        // Lista de cuentas de la partida contable
        private List<PartidaCuentaItem> _partidaContable = new List<PartidaCuentaItem>();

        // Variables para cálculos automáticos
        private decimal _montoTotal = 0;
        private decimal _valorImpreso = 0;
        private int _filaSeleccionadaIndex = -1;
        private bool _advertenciaLimiteMostrada = false;
        private bool _esExencionManual = false;

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse);

        // Clase auxiliar para cuentas de partida
        public class PartidaCuentaItem
        {
            public string Codigo { get; set; }
            public string Cuenta { get; set; }
            public string Descripcion { get; set; }
            public decimal Cargo { get; set; }
            public decimal Abono { get; set; }
        }
        // Constructor
        public Frm_Transfers_Managment()
        {
            InitializeComponent();
            InicializarImpresion();
            authController = new Ctrl_Security_Auth();
        }
        // Evento Load del formulario
        private async void Frm_Transfers_Managment_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarPlaceHoldersTextbox();
                ConfigurarMaxLengthTextBox();
                ConfigurarComboBoxes();
                ConfigurarComboBoxSinScroll();
                ConfigurarDateTimePicker();
                ConfigurarTabla();
                ConfigurarEstadoInicialBotones();
                InicializarScroll();
                ConfigurarEventosScroll();
                BloquearCamposIniciales();
                ConfigurarComplemento();
                ConfigurarTabIndexYFocus();
                AplicarEstiloTextBoxTotales();
                AplicarEstilosBotones();
                InicializarImpresion();
                ConfigurarPeriodoYBanco();
                ConfigurarEventosExencionManual();
                ConfigurarEventosSincronizacion();

                // CARGAR PERMISOS DEL USUARIO
                if (UserData != null)
                {
                    await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
                    ConfigurarBotonesPorPermisos();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL CARGAR FORMULARIO: {ex.Message}",
                              "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Frm_Transfers_Managment_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
        #endregion PropiedadesIniciales
        #region ConfigurarTextBox
        private void ConfigurarMaxLengthTextBox()
        {
            Txt_Codigo.MaxLength = 10;
            Txt_Cuenta.MaxLength = 100;
            Txt_DescripcionCuenta.MaxLength = 200;
            Txt_Cargos.MaxLength = 15;
            Txt_Abonos.MaxLength = 15;
            Txt_Beneficiario.MaxLength = 250;
            Txt_Concepto.MaxLength = 110;
            Txt_Observaciones.MaxLength = 480;
            Txt_MontoTotal.MaxLength = 15;
            Txt_OrdenCompra.MaxLength = 25;
            Txt_ValorLetras.MaxLength = 200;
            Txt_Alimentacion.MaxLength = 15;
            Txt_Bonificacion.MaxLength = 15;
            Txt_Descuentos.MaxLength = 15;
            Txt_Anticipos.MaxLength = 15;
            Txt_Viaticos.MaxLength = 15;
            Txt_Stamps.MaxLength = 15;
            Txt_Indemnizacion.MaxLength = 15;
            Txt_Vacaciones.MaxLength = 15;
            Txt_Aguinaldo.MaxLength = 15;
        }

        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_Codigo, "CÓDIGO CUENTA");
            ConfigurarPlaceHolder(Txt_Cuenta, "NOMBRE DE CUENTA");
            ConfigurarPlaceHolder(Txt_DescripcionCuenta, "DESCRIPCIÓN/DETALLE");
            ConfigurarPlaceHolder(Txt_Beneficiario, "NOMBRE DEL BENEFICIARIO");
            ConfigurarPlaceHolder(Txt_Concepto, "CONCEPTO DE LA TRANSFERENCIA");
            ConfigurarPlaceHolder(Txt_Observaciones, "OBSERVACIONES/DETALLES");
            ConfigurarPlaceHolder(Txt_OrdenCompra, "0");

            // Campos de cálculo con placeholder 0.00
            ConfigurarPlaceHolder(Txt_Cargos, "0.00");
            ConfigurarPlaceHolder(Txt_Abonos, "0.00");
            ConfigurarPlaceHolder(Txt_MontoTotal, "0.00");
            ConfigurarPlaceHolder(Txt_Exencion, "0.00");
            ConfigurarPlaceHolder(Txt_MontoSinITH, "0.00");
            ConfigurarPlaceHolder(Txt_ITH, "0.00");
            ConfigurarPlaceHolder(Txt_Alimentacion, "0.00");
            ConfigurarPlaceHolder(Txt_IGSS, "0.00");
            ConfigurarPlaceHolder(Txt_Bonificacion, "0.00");
            ConfigurarPlaceHolder(Txt_Descuentos, "0.00");
            ConfigurarPlaceHolder(Txt_Anticipos, "0.00");
            ConfigurarPlaceHolder(Txt_RetencionISR, "0.00");
            ConfigurarPlaceHolder(Txt_Viaticos, "0.00");
            ConfigurarPlaceHolder(Txt_Stamps, "0.00");
            ConfigurarPlaceHolder(Txt_Indemnizacion, "0.00");
            ConfigurarPlaceHolder(Txt_Vacaciones, "0.00");
            ConfigurarPlaceHolder(Txt_ValorImpresoTransferencia, "0.00");
            ConfigurarPlaceHolder(Txt_TotalCargos, "0.00");
            ConfigurarPlaceHolder(Txt_TotalAbonos, "0.00");
            ConfigurarPlaceHolder(Txt_Aguinaldo, "0.00");
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
        #region ConfigurarDateTimePicker
        // --- ----- CONFIGURACION CON FECHA LIBRE -------
        private void ConfigurarDateTimePicker()
        {
            // Configurar DTP_Emision para mostrar fecha larga
            DTP_Emision.Format = DateTimePickerFormat.Long;

            // Establecer la fecha actual como valor por defecto
            DTP_Emision.Value = DateTime.Now;

            // Permitir seleccionar cualquier fecha (sin restricciones)
            // Se eliminan MinDate y MaxDate para habilitar todo el rango válido de DateTime
            DTP_Emision.MinDate = DateTimePicker.MinimumDateTime;
            DTP_Emision.MaxDate = DateTimePicker.MaximumDateTime;
        }
        // CONFIGURACIÓN PARA FECHA DEL MES ACTUAL
        //private void ConfigurarDateTimePicker()
        //{
        //    // Configurar DTP_Emision
        //    DTP_Emision.Format = DateTimePickerFormat.Long;
        //    DTP_Emision.Value = DateTime.Now;

        //    // Bloquear selector manual (opcional)
        //    DTP_Emision.MinDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //    DTP_Emision.MaxDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
        //        DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        //}
        #endregion ConfigurarDateTimePicker
        #region ConfigurarComboBox
        private void ConfigurarComboBoxes()
        {
            // ComboBox no editable
            ComboBox_TipoTransferencia.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Location.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Banco.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_TipoCuenta.DropDownStyle = ComboBoxStyle.DropDownList;

            // TIPOS DE TRANSFERENCIA
            ComboBox_TipoTransferencia.Items.AddRange(new object[]
            {
                "HOSPEDAJES",
                "PUBLICIDAD",
                "SERVICIOS",
                "HONORARIOS",
                "HONORARIOS PROFESIONALES",
                "COMPRAS",
                "SUELDOS",
                "LIQUIDACIONES",
                "BONO NAVIDEÑO",
                "ARRENDAMIENTOS",
                "ANTICIPOS DE SUELDOS",
                "ANTICIPOS DE HONORARIOS",
                "ANTICIPOS"
            });
            ComboBox_TipoTransferencia.SelectedIndex = 0;

            // Cargar Sedes/Locations
            var locations = Ctrl_Locations.ObtenerLocationsActivas();
            ComboBox_Location.DataSource = new BindingSource(locations, null);
            ComboBox_Location.DisplayMember = "Value";
            ComboBox_Location.ValueMember = "Key";
            if (ComboBox_Location.Items.Count > 0)
                ComboBox_Location.SelectedIndex = 0;

            // Cargar Tipos de Cuenta Bancaria
            var banksAccountType = Ctrl_BanksAccountTypes.ObtenerTiposCuentaParaCombo();
            ComboBox_TipoCuenta.DataSource = new BindingSource(banksAccountType, null);
            ComboBox_TipoCuenta.DisplayMember = "Value";  
            ComboBox_TipoCuenta.ValueMember = "Key";      
            if (ComboBox_TipoCuenta.Items.Count > 0)
                ComboBox_TipoCuenta.SelectedIndex = 0;

            // Cargar Bancos
            var banks = Ctrl_Banks.ObtenerBancosParaCombo();
            ComboBox_Banco.DataSource = new BindingSource(banks, null);
            ComboBox_Banco.DisplayMember = "Value";
            ComboBox_Banco.ValueMember = "Key";
            if (ComboBox_Banco.Items.Count > 0)
                ComboBox_Banco.SelectedIndex = 0;


            // Evento al cambiar tipo de TRANSFERENCIA
            ComboBox_TipoTransferencia.SelectedIndexChanged += ComboBox_TipoTRANSFERENCIA_SelectedIndexChanged;

            // Evento CheckBox Exención
            CheckBox_Exencion.CheckedChanged += CheckBox_Exencion_CheckedChanged;
        }

        // Evento al cambiar tipo de TRANSFERENCIA
        private void ComboBox_TipoTRANSFERENCIA_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tipoSeleccionado = ComboBox_TipoTransferencia.SelectedItem?.ToString() ?? "";

            // RESETEAR TODOS LOS CAMPOS PRIMERO
            CheckBox_Exencion.Enabled = true;
            CheckBox_Exencion.Checked = false;
            Txt_Stamps.Enabled = false;
            Txt_Stamps.Text = "0.00";
            Txt_Stamps.ForeColor = Color.Gray;
            Txt_RetencionISR.Enabled = false;
            Txt_RetencionISR.Text = "0.00";
            Txt_RetencionISR.ForeColor = Color.Gray;
            Txt_Indemnizacion.Enabled = false;
            Txt_Indemnizacion.Text = "0.00";
            Txt_Indemnizacion.ForeColor = Color.Gray;
            Txt_Vacaciones.Enabled = false;
            Txt_Vacaciones.Text = "0.00";
            Txt_Vacaciones.ForeColor = Color.Gray;
            Txt_IGSS.Enabled = false;
            Txt_IGSS.Text = "0.00";
            Txt_IGSS.ForeColor = Color.Gray;
            Txt_Aguinaldo.Enabled = false;
            Txt_Aguinaldo.Text = "0.00";
            Txt_Aguinaldo.ForeColor = Color.Gray;

            // CONFIGURAR SEGÚN TIPO
            switch (tipoSeleccionado)
            {
                case "SUELDOS":
                    // Sin exención, sin stamps, CON ISR manual, IGSS automático
                    CheckBox_Exencion.Checked = false;
                    CheckBox_Exencion.Enabled = false;
                    Txt_RetencionISR.Enabled = true;
                    Txt_RetencionISR.ForeColor = Color.Black;
                    break;

                case "ANTICIPOS DE SUELDOS":
                case "ANTICIPOS DE HONORARIOS":
                    // Sin exención, sin stamps, CON ISR manual, SIN IGSS
                    CheckBox_Exencion.Checked = false;
                    CheckBox_Exencion.Enabled = false;
                    Txt_RetencionISR.Enabled = true;
                    Txt_RetencionISR.ForeColor = Color.Black;
                    // IGSS queda deshabilitado (sin cálculo ni ingreso manual)
                    break;

                case "LIQUIDACIONES":
                case "BONO NAVIDEÑO":
                    // Sin exención, SIN stamps, SIN ISR, CON Indemnización y Vacaciones
                    CheckBox_Exencion.Checked = false;
                    CheckBox_Exencion.Enabled = false;
                    Txt_Indemnizacion.Enabled = true;
                    Txt_Indemnizacion.ForeColor = Color.Black;
                    Txt_Vacaciones.Enabled = true;
                    Txt_Vacaciones.ForeColor = Color.Black;
                    Txt_Aguinaldo.Enabled = true;
                    Txt_Aguinaldo.ForeColor = Color.Black;
                    break;

                case "PUBLICIDAD":
                    // Con exención, CON stamps, CON ISR manual
                    CheckBox_Exencion.Enabled = true;

                    Txt_Stamps.Enabled = true;
                    Txt_Stamps.ForeColor = Color.Black;

                    // ⭐ NUEVO: ISR MANUAL PARA PUBLICIDAD
                    Txt_RetencionISR.Enabled = true;
                    Txt_RetencionISR.ForeColor = Color.Black;
                    break;

                case "ARRENDAMIENTOS":
                    // Con exención, sin stamps, CON ISR manual
                    CheckBox_Exencion.Enabled = true;
                    Txt_RetencionISR.Enabled = true;
                    Txt_RetencionISR.ForeColor = Color.Black;
                    break;

                default:
                    // OTROS: Con exención, sin stamps, SIN ISR, SIN Indemnización/Vacaciones
                    CheckBox_Exencion.Enabled = true;
                    break;
            }

            // CAMPOS SIEMPRE HABILITADOS
            Txt_Alimentacion.Enabled = true;
            Txt_Bonificacion.Enabled = true;
            Txt_Anticipos.Enabled = true;
            Txt_Viaticos.Enabled = true;
            Txt_Descuentos.Enabled = true;

            // Recalcular
            CalcularValores();
        }
        // Evento CheckBox Exención
        private void CheckBox_Exencion_CheckedChanged(object sender, EventArgs e)
        {
            ValidarExencionManual(); // ⭐ PRIMERO VALIDAR
            CalcularValores();        // ⭐ LUEGO CALCULAR
        }
        #endregion ConfigurarComboBox
        #region CalculosAutomaticos
        // Método para calcular valores automáticamente
        private void CalcularValores()
        {
            try
            {
                if (_esExencionManual)
                {
                    CalcularValoresConExencionManual();
                    return;
                }

                _montoTotal = ObtenerValorDecimal(Txt_MontoTotal);

                if (_montoTotal == 0)
                {
                    LimpiarCalculos();
                    return;
                }

                string tipoChecked = ComboBox_TipoTransferencia.SelectedItem?.ToString() ?? "";
                bool conExencion = CheckBox_Exencion.Checked;

                decimal alimentacion = ObtenerValorDecimal(Txt_Alimentacion);
                decimal bonificacion = ObtenerValorDecimal(Txt_Bonificacion);
                decimal viaticos = ObtenerValorDecimal(Txt_Viaticos);
                decimal stamps = ObtenerValorDecimal(Txt_Stamps);
                decimal indemnizacion = ObtenerValorDecimal(Txt_Indemnizacion);
                decimal vacaciones = ObtenerValorDecimal(Txt_Vacaciones);
                decimal aguinaldo = ObtenerValorDecimal(Txt_Aguinaldo);

                decimal descuentos = ObtenerValorDecimal(Txt_Descuentos);
                decimal anticipos = ObtenerValorDecimal(Txt_Anticipos);

                decimal excencion = 0;
                decimal montoSinITH = 0;
                decimal ith = 0;
                decimal igss = 0;
                decimal retencionISR = 0;

                // Calcular suma de valores impresos de complementos si LastComplement está activo
                decimal sumaComplementos = 0;
                if (CheckBox_LastComplement.Checked &&
                    CheckBox_Complemento.Checked &&
                    !string.IsNullOrWhiteSpace(Txt_Complemento.Text))
                {
                    string numeroComplemento = Txt_Complemento.Text.Trim();

                    try
                    {
                        // Obtener la TRANSFERENCIA principal (cuando es anticipo)
                        Mdl_Transfers transferenciaPrincipal = Ctrl_Transfers.ObtenerTransferPorNumero(numeroComplemento);
                        if (transferenciaPrincipal != null)
                        {
                            sumaComplementos += transferenciaPrincipal.PrintedAmount;
                        }

                        // Obtener todas las transferencias vinculadas a ese complemento
                        List<Mdl_Transfers> transfersComplemento =
                            Ctrl_Transfers.ObtenerTransfersComplemento(numeroComplemento);

                        if (transfersComplemento != null && transfersComplemento.Count > 0)
                        {
                            foreach (var transferencia in transfersComplemento)
                            {
                                sumaComplementos += transferencia.PrintedAmount;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error al obtener complementos de transferencia: " + ex.Message);
                    }
                }

                // MONTO BASE = MontoTotal - (conceptos incluidos)
                decimal montoBase = _montoTotal - alimentacion - bonificacion - viaticos - stamps - indemnizacion - vacaciones - descuentos - anticipos - aguinaldo;

                switch (tipoChecked)
                {
                    case "HOSPEDAJES":
                        ith = montoBase / 1.22m * 0.10m;
                        montoSinITH = montoBase - ith;

                        if (conExencion)
                        {
                            excencion = (montoSinITH + alimentacion) / 1.12m * 0.12m;
                        }
                        if (CheckBox_LastComplement.Checked)
                        {
                            _valorImpreso = _montoTotal - excencion - descuentos - anticipos - sumaComplementos;
                        }
                        else
                        {
                            _valorImpreso = _montoTotal - excencion - descuentos - anticipos;
                        }
                        _valorImpreso = Math.Round(_valorImpreso, 2);
                        break;

                    case "PUBLICIDAD":
                        // IVA (exención) calculado sobre montoBase
                        if (conExencion)
                        {
                            excencion = montoBase / 1.12m * 0.12m;
                        }

                        // ⭐ ISR MANUAL (igual que ARRENDAMIENTOS)
                        retencionISR = ObtenerValorDecimal(Txt_RetencionISR);

                        // Total a pagar = MontoTotal - IVA - ISR - descuentos - anticipos (y complementos)
                        if (CheckBox_LastComplement.Checked)
                        {
                            _valorImpreso = _montoTotal - excencion - retencionISR - descuentos - anticipos - sumaComplementos;
                        }
                        else
                        {
                            _valorImpreso = _montoTotal - excencion - retencionISR - descuentos - anticipos;
                        }

                        _valorImpreso = Math.Round(_valorImpreso, 2);
                        break;

                    case "SERVICIOS":
                    case "ANTICIPOS":
                    case "HONORARIOS":
                    case "HONORARIOS PROFESIONALES":
                    case "COMPRAS":
                        // IVA (exención) calculado sobre montoBase
                        if (conExencion)
                        {
                            excencion = montoBase / 1.12m * 0.12m;
                        }

                        // ISR se ingresa MANUALMENTE
                        retencionISR = ObtenerValorDecimal(Txt_RetencionISR);

                        // Total a pagar = MontoTotal - IVA - ISR - descuentos - anticipos (y complementos)
                        if (CheckBox_LastComplement.Checked)
                        {
                            _valorImpreso = _montoTotal - excencion - retencionISR - descuentos - anticipos - sumaComplementos;
                        }
                        else
                        {
                            _valorImpreso = _montoTotal - excencion - retencionISR - descuentos - anticipos;
                        }
                        _valorImpreso = Math.Round(_valorImpreso, 2);
                        break;

                    case "SUELDOS":
                        montoBase = _montoTotal - bonificacion - alimentacion - viaticos - vacaciones;
                        igss = montoBase * 0.0483m;
                        retencionISR = ObtenerValorDecimal(Txt_RetencionISR);
                        if (CheckBox_LastComplement.Checked)
                        {
                            _valorImpreso = _montoTotal - igss - retencionISR - descuentos - anticipos - sumaComplementos;
                        }
                        else
                        {
                            _valorImpreso = _montoTotal - igss - retencionISR - descuentos - anticipos;
                        }
                        _valorImpreso = Math.Round(_valorImpreso, 2);
                        break;

                    case "LIQUIDACIONES":
                    case "BONO NAVIDEÑO":
                        if (CheckBox_LastComplement.Checked)
                        {
                            _valorImpreso = _montoTotal - descuentos - anticipos - sumaComplementos;
                        }
                        else
                        {
                            _valorImpreso = _montoTotal - descuentos - anticipos;
                        }
                        _valorImpreso = Math.Round(_valorImpreso, 2);
                        break;

                    case "ARRENDAMIENTOS":
                        // IVA (exención) calculado sobre montoBase
                        if (conExencion)
                        {
                            excencion = montoBase / 1.12m * 0.12m;
                        }

                        // ISR se ingresa MANUALMENTE
                        retencionISR = ObtenerValorDecimal(Txt_RetencionISR);

                        // Total a pagar = MontoTotal - IVA - ISR - descuentos - anticipos (y complementos)
                        if (CheckBox_LastComplement.Checked)
                        {
                            _valorImpreso = _montoTotal - excencion - retencionISR - descuentos - anticipos - sumaComplementos;
                        }
                        else
                        {
                            _valorImpreso = _montoTotal - excencion - retencionISR - descuentos - anticipos;
                        }
                        _valorImpreso = Math.Round(_valorImpreso, 2);
                        break;
                    case "ANTICIPOS DE SUELDOS":
                    case "ANTICIPOS DE HONORARIOS":
                        // Funciona igual que SUELDOS pero SIN IGSS (no se calcula ni se resta)
                        retencionISR = ObtenerValorDecimal(Txt_RetencionISR);
                        if (CheckBox_LastComplement.Checked)
                        {
                            _valorImpreso = _montoTotal - retencionISR - descuentos - anticipos - sumaComplementos;
                        }
                        else
                        {
                            _valorImpreso = _montoTotal - retencionISR - descuentos - anticipos;
                        }
                        _valorImpreso = Math.Round(_valorImpreso, 2);
                        break;
                }

                Txt_Exencion.Text = excencion.ToString("N2");
                Txt_MontoSinITH.Text = montoSinITH.ToString("N2");
                Txt_ITH.Text = ith.ToString("N2");
                Txt_IGSS.Text = igss.ToString("N2");

                // SOLO se resetea ISR en tipos donde NO se usa manual ni automático
                if (tipoChecked != "SUELDOS" && tipoChecked != "ANTICIPOS DE SUELDOS" && tipoChecked != "ANTICIPOS DE HONORARIOS" && tipoChecked != "ARRENDAMIENTOS" && tipoChecked != "HONORARIOS" && tipoChecked != "HONORARIOS PROFESIONALES" && tipoChecked != "COMPRAS" && tipoChecked != "PUBLICIDAD")
                {
                    Txt_RetencionISR.Text = "0.00";
                }

                // ACTUALIZAR EL TEXTBOX CON EL VALOR CORRECTO
                Txt_ValorImpresoTransferencia.Text = _valorImpreso.ToString("N2");

                // CONVERTIR A LETRAS Y ACTUALIZAR
                Txt_ValorLetras.Text = NumeroALetras(_valorImpreso);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR EN CÁLCULOS: {ex.Message}", "ERROR",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Limpiar cálculos
        private void LimpiarCalculos()
        {
            Txt_Exencion.Text = "0.00";
            Txt_MontoSinITH.Text = "0.00";
            Txt_ITH.Text = "0.00";
            Txt_IGSS.Text = "0.00";
            // NO limpiar ISR si está en tipos que lo usan manualmente
            string tipoSeleccionado = ComboBox_TipoTransferencia.SelectedItem?.ToString() ?? "";
            if (tipoSeleccionado != "SUELDOS" &&
                tipoSeleccionado != "ARRENDAMIENTOS" &&
                tipoSeleccionado != "ANTICIPOS DE SUELDOS" &&
                tipoSeleccionado != "ANTICIPOS DE HONORARIOS" &&
                tipoSeleccionado != "PUBLICIDAD")   
            {
                Txt_RetencionISR.Text = "0.00";
            }

            Txt_ValorImpresoTransferencia.Text = "0.00";
            Txt_ValorLetras.Text = "CERO QUETZALES CON 00/100";
        }

        // Obtener valor decimal de TextBox
        private decimal ObtenerValorDecimal(TextBox txt)
        {
            if (decimal.TryParse(txt.Text.Replace(",", ""), out decimal valor))
                return valor;
            return 0;
        }

        // EVENTOS TextChanged para recalcular automáticamente
        private void Txt_MontoTotal_TextChanged(object sender, EventArgs e)
        {
            CalcularValores();
        }

        private void Txt_Alimentacion_TextChanged(object sender, EventArgs e)
        {
            CalcularValores();
        }

        private void Txt_Anticipos_TextChanged(object sender, EventArgs e)
        {
            CalcularValores();
        }

        private void Txt_Descuentos_TextChanged(object sender, EventArgs e)
        {
            CalcularValores();
        }

        private void Txt_Bonificacion_TextChanged(object sender, EventArgs e)
        {
            CalcularValores();
        }

        private void Txt_Viaticos_TextChanged(object sender, EventArgs e)
        {
            CalcularValores();
        }

        private void Txt_Stamps_TextChanged(object sender, EventArgs e)
        {
            CalcularValores();
        }

        private void Txt_RetencionISR_TextChanged(object sender, EventArgs e)
        {
            // Recalcular si estamos en SUELDOS o ARRENDAMIENTOS
            string tipoSeleccionado = ComboBox_TipoTransferencia.SelectedItem?.ToString() ?? "";

            if (tipoSeleccionado == "SUELDOS" ||
                tipoSeleccionado == "ANTICIPOS DE SUELDOS" ||
                tipoSeleccionado == "ANTICIPOS DE HONORARIOS" ||
                tipoSeleccionado == "ARRENDAMIENTOS" ||
                tipoSeleccionado == "HONORARIOS" ||
                tipoSeleccionado == "HONORARIOS PROFESIONALES" ||
                tipoSeleccionado == "COMPRAS" ||
                tipoSeleccionado == "PUBLICIDAD")   // ⭐ NUEVO
            {
                CalcularValores();
            }
        }
        private void Txt_Indemnizacion_TextChanged(object sender, EventArgs e)
        {
            CalcularValores();
        }

        private void Txt_Vacaciones_TextChanged(object sender, EventArgs e)
        {
            CalcularValores();
        }
        private void Txt_Aguinaldo_TextChanged(object sender, EventArgs e)
        {
            CalcularValores();
        }


        // ⭐ VALIDACIÓN: Solo números y punto decimal
        private void ValidarSoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;

            // Permitir números, punto decimal, backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Solo un punto decimal
            if (e.KeyChar == '.' && txt.Text.Contains("."))
            {
                e.Handled = true;
            }
        }
        #endregion CalculosAutomaticos
        #region ConversionNumeroALetras
        // Convertir número a letras
        private string NumeroALetras(decimal numero)
        {
            if (numero == 0) return "CERO QUETZALES CON 00/100";

            long parteEntera = (long)Math.Floor(numero);
            int parteDecimal = (int)Math.Round((numero - parteEntera) * 100);

            if (parteDecimal > 99) parteDecimal = 99;

            string resultado = ConvertirEntero(parteEntera);
            resultado = resultado.Trim();
            resultado += $" QUETZALES CON {parteDecimal:00}/100";

            return resultado.ToUpper();
        }
        /* CODIGO ORIGINAL CONVERTIR ENTERO
         * private string ConvertirEntero(long numero)
        {
            if (numero == 0) return "";

            string[] unidades = { "", "UNO", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE" };
            string[] decenas = { "", "DIEZ", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };
            string[] especiales = { "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISÉIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE" };
            string[] centenas = { "", "CIENTO", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS", "SEISCIENTOS", "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS" };

            string resultado = "";

            // Millones
            if (numero >= 1000000)
            {
                long millones = numero / 1000000;
                if (millones == 1)
                    resultado += "UN MILLÓN ";
                else
                    resultado += ConvertirGrupo((int)millones, unidades, decenas, especiales, centenas) + " MILLONES ";
                numero %= 1000000;
            }

            // Miles
            if (numero >= 1000)
            {
                long miles = numero / 1000;
                if (miles == 1)
                    resultado += "MIL ";
                else
                {
                    string grupoMiles = ConvertirGrupo((int)miles, unidades, decenas, especiales, centenas);
                    if (grupoMiles.EndsWith("UNO"))
                        grupoMiles = grupoMiles.Substring(0, grupoMiles.Length - 3) + "UN";
                    resultado += grupoMiles + " MIL ";
                }
                numero %= 1000;
            }

            // Unidades, decenas y centenas
            if (numero > 0)
            {
                resultado += ConvertirGrupo((int)numero, unidades, decenas, especiales, centenas);
            }

            return resultado.Trim();
        }
         * */
        private string ConvertirEntero(long numero)
        {
            if (numero == 0) return "";

            string[] unidades = { "", "UNO", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE" };
            string[] decenas = { "", "DIEZ", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };
            string[] especiales = { "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISÉIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE" };
            string[] centenas = { "", "CIENTO", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS", "SEISCIENTOS", "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS" };

            string resultado = "";

            // Millones
            if (numero >= 1000000)
            {
                long millones = numero / 1000000;
                if (millones == 1)
                    resultado += "UN MILLÓN ";
                else
                    resultado += ConvertirGrupo((int)millones, unidades, decenas, especiales, centenas) + " MILLONES ";
                numero %= 1000000;
            }

            // Miles
            if (numero >= 1000)
            {
                long miles = numero / 1000;
                if (miles == 1)
                    resultado += "UN MIL ";
                else
                {
                    string grupoMiles = ConvertirGrupo((int)miles, unidades, decenas, especiales, centenas);
                    if (grupoMiles.EndsWith("UNO"))
                        grupoMiles = grupoMiles.Substring(0, grupoMiles.Length - 3) + "UN";
                    resultado += grupoMiles + " MIL ";
                }
                numero %= 1000;
            }

            // Unidades, decenas y centenas
            if (numero > 0)
            {
                resultado += ConvertirGrupo((int)numero, unidades, decenas, especiales, centenas);
            }

            return resultado.Trim();
        }

        private string ConvertirGrupo(int numero, string[] unidades, string[] decenas, string[] especiales, string[] centenas)
        {
            string resultado = "";

            // Centenas
            int centena = numero / 100;
            if (centena > 0)
            {
                if (centena == 1 && numero == 100)
                    resultado += "CIEN";
                else
                    resultado += centenas[centena];
            }

            numero %= 100;

            // Decenas y unidades
            if (numero >= 10 && numero <= 19)
            {
                if (resultado.Length > 0) resultado += " ";
                resultado += especiales[numero - 10];
            }
            else if (numero >= 20 && numero <= 29)
            {
                if (resultado.Length > 0) resultado += " ";
                int unidad = numero % 10;
                if (unidad == 0)
                    resultado += "VEINTE";
                else
                    resultado += "VEINTI" + unidades[unidad];
            }
            else
            {
                int decena = numero / 10;
                int unidad = numero % 10;

                if (decena > 0)
                {
                    if (resultado.Length > 0) resultado += " ";
                    resultado += decenas[decena];
                }

                if (unidad > 0)
                {
                    if (decena > 0)
                        resultado += " Y ";
                    else if (resultado.Length > 0)
                        resultado += " ";
                    resultado += unidades[unidad];
                }
            }

            return resultado;
        }
        #endregion ConversionNumeroALetras
        #region PartidaContable
        // Configurar estado inicial de botones
        private void ConfigurarEstadoInicialBotones()
        {
            Btn_AddCuenta.Enabled = true;
            Btn_Update.Enabled = false;
            Btn_Remove.Enabled = false;
        }
        // Calcular totales de la partida
        private void CalcularTotalesPartida()
        {
            decimal totalCargos = _partidaContable.Sum(p => p.Cargo);
            decimal totalAbonos = _partidaContable.Sum(p => p.Abono);

            Txt_TotalCargos.Text = totalCargos.ToString("N2");
            Txt_TotalAbonos.Text = totalAbonos.ToString("N2");
        }
        // Obtener texto real (sin placeholder)
        private string ObtenerTextoReal(TextBox txt)
        {
            if (txt.ForeColor == Color.Gray) return "";
            return txt.Text.Trim();
        }
        // Agregar cuenta a la partida - EVENTO DEL BOTÓN
        private void Btn_AddCuenta_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener valores de los campos
                string codigo = ObtenerTextoReal(Txt_Codigo);
                string cuenta = ObtenerTextoReal(Txt_Cuenta);
                string descripcion = ObtenerTextoReal(Txt_DescripcionCuenta);
                decimal cargos = ObtenerValorDecimal(Txt_Cargos);
                decimal abonos = ObtenerValorDecimal(Txt_Abonos);

                // VALIDACIÓN 1: Campos obligatorios
                if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(cuenta) ||
                    string.IsNullOrEmpty(descripcion))
                {
                    MessageBox.Show("DEBE INGRESAR TODOS LOS DATOS DE LA CUENTA",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ⭐ VALIDACIÓN 2: Verificar que no sea el placeholder
                if (Txt_Cuenta.Text == "NOMBRE DE CUENTA")
                {
                    MessageBox.Show("DEBE INGRESAR UN NOMBRE DE CUENTA VÁLIDO",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Txt_Cuenta.Focus();
                    return;
                }

                // VALIDACIÓN 3: Solo uno debe tener valor (Cargo O Abono, no ambos)
                if ((cargos > 0 && abonos > 0) || (cargos == 0 && abonos == 0))
                {
                    MessageBox.Show("DEBE INGRESAR CARGO O ABONO (SOLO UNO)",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // VALIDACIÓN 4: Verificar si la cuenta ya existe en la partida
                foreach (DataGridViewRow row in Tabla.Rows)
                {
                    if (row.Cells["Codigo"].Value?.ToString() == codigo)
                    {
                        MessageBox.Show("ESTA CUENTA YA ESTÁ REGISTRADA EN LA PARTIDA",
                                      "CUENTA DUPLICADA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // AGREGAR a la lista interna
                _partidaContable.Add(new PartidaCuentaItem
                {
                    Codigo = codigo,
                    Cuenta = cuenta,
                    Descripcion = descripcion,
                    Cargo = cargos,
                    Abono = abonos
                });

                // AGREGAR a la tabla visual
                Tabla.Rows.Add(codigo, cuenta, descripcion,
                              cargos.ToString("N2"), abonos.ToString("N2"));

                // Actualizar totales
                CalcularTotalesPartida();

                // Limpiar campos de entrada
                LimpiarCamposPartida();
                Tabla.ClearSelection();
                _filaSeleccionadaIndex = -1;

                // Regresar foco al código
                Txt_Codigo.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL AGREGAR CUENTA: {ex.Message}",
                              "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ACTUALIZAR cuenta de la partida - EVENTO DEL BOTÓN
        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDACIÓN 1: Verificar que hay una fila seleccionada
                if (_filaSeleccionadaIndex < 0 || _filaSeleccionadaIndex >= Tabla.Rows.Count)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CUENTA DE LA TABLA",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtener valores de los campos
                string codigo = ObtenerTextoReal(Txt_Codigo);
                string cuenta = ObtenerTextoReal(Txt_Cuenta);
                string descripcion = ObtenerTextoReal(Txt_DescripcionCuenta);
                decimal cargos = ObtenerValorDecimal(Txt_Cargos);
                decimal abonos = ObtenerValorDecimal(Txt_Abonos);

                // VALIDACIÓN 2: Campos obligatorios
                if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(cuenta) ||
                    string.IsNullOrEmpty(descripcion))
                {
                    MessageBox.Show("DEBE INGRESAR TODOS LOS DATOS DE LA CUENTA",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // VALIDACIÓN 3: Solo uno debe tener valor (Cargo O Abono, no ambos)
                if ((cargos > 0 && abonos > 0) || (cargos == 0 && abonos == 0))
                {
                    MessageBox.Show("DEBE INGRESAR CARGO O ABONO (SOLO UNO)",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // VALIDACIÓN 4: Verificar si el código ya existe en OTRA fila
                string codigoOriginal = Tabla.Rows[_filaSeleccionadaIndex].Cells["Codigo"].Value?.ToString();

                if (codigo != codigoOriginal)
                {
                    foreach (DataGridViewRow row in Tabla.Rows)
                    {
                        if (row.Index != _filaSeleccionadaIndex &&
                            row.Cells["Codigo"].Value?.ToString() == codigo)
                        {
                            MessageBox.Show("ESTA CUENTA YA ESTÁ REGISTRADA EN LA PARTIDA",
                                          "CUENTA DUPLICADA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                // CONFIRMACIÓN
                var confirmacion = MessageBox.Show(
                    "¿DESEA ACTUALIZAR LA CUENTA EN LA PARTIDA?",
                    "CONFIRMAR ACTUALIZACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                // ⭐ ACTUALIZAR en la lista interna
                _partidaContable[_filaSeleccionadaIndex] = new PartidaCuentaItem
                {
                    Codigo = codigo,
                    Cuenta = cuenta,
                    Descripcion = descripcion,
                    Cargo = cargos,
                    Abono = abonos
                };

                // ⭐ ACTUALIZAR en la tabla visual
                DataGridViewRow filaActualizar = Tabla.Rows[_filaSeleccionadaIndex];
                filaActualizar.Cells["Codigo"].Value = codigo;
                filaActualizar.Cells["Cuenta"].Value = cuenta;
                filaActualizar.Cells["Descripcion"].Value = descripcion;
                filaActualizar.Cells["Cargo"].Value = cargos.ToString("N2");
                filaActualizar.Cells["Abono"].Value = abonos.ToString("N2");

                // Actualizar totales
                CalcularTotalesPartida();

                // Limpiar campos y selección
                LimpiarCamposPartida();
                Tabla.ClearSelection();
                _filaSeleccionadaIndex = -1;

                // Restaurar estado de botones
                Btn_AddCuenta.Enabled = true;
                Btn_Update.Enabled = false;

                // Regresar foco al código
                Txt_Codigo.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ACTUALIZAR CUENTA: {ex.Message}",
                              "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ELIMINAR cuenta de la partida - EVENTO DEL BOTÓN
        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que hay una fila seleccionada
                if (_filaSeleccionadaIndex < 0 || _filaSeleccionadaIndex >= Tabla.Rows.Count)
                {
                    MessageBox.Show("DEBE SELECCIONAR UNA CUENTA DE LA TABLA",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtener nombre de la cuenta para mostrar en confirmación
                string nombreCuenta = Tabla.Rows[_filaSeleccionadaIndex].Cells["Cuenta"].Value?.ToString() ?? "DESCONOCIDA";

                var confirmacion = MessageBox.Show(
                    $"¿DESEA ELIMINAR LA SIGUIENTE CUENTA DE LA PARTIDA?\n\n{nombreCuenta}",
                    "CONFIRMAR ELIMINACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.No)
                    return;

                // Eliminar de la lista interna
                _partidaContable.RemoveAt(_filaSeleccionadaIndex);

                // Eliminar de la tabla visual
                Tabla.Rows.RemoveAt(_filaSeleccionadaIndex);

                // Actualizar totales
                CalcularTotalesPartida();

                // Limpiar campos y selección
                LimpiarCamposPartida();
                Tabla.ClearSelection();
                _filaSeleccionadaIndex = -1;

                // Restaurar estado de botones
                Btn_AddCuenta.Enabled = true;
                Btn_Update.Enabled = false;
                Btn_Remove.Enabled = false;

                MessageBox.Show("CUENTA ELIMINADA CORRECTAMENTE",
                              "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Txt_Codigo.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ELIMINAR CUENTA: {ex.Message}",
                              "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Método público para actualizar cuenta desde formulario emergente
        public void ActualizarCuentaContable(string codigo, string cuenta)
        {
            Txt_Codigo.Text = codigo;
            Txt_Codigo.ForeColor = Color.Black;

            Txt_Cuenta.Text = cuenta;
            Txt_Cuenta.ForeColor = Color.Black;

            Txt_DescripcionCuenta.Text = cuenta;
            Txt_DescripcionCuenta.ForeColor = Color.Black;

            Txt_Cargos.Focus();
        }
        // Evento para buscar cuenta - ABRIR FORMULARIO DE BÚSQUEDA
        private void Btn_SearchCuenta_Click(object sender, EventArgs e)
        {
            // Asegurar estado inicial de botones
            ConfigurarEstadoInicialBotones();

            try
            {
                // Crear instancia del formulario emergente
                Frm_Transfers_SearchCuenta frmBuscar = new Frm_Transfers_SearchCuenta(this);

                // Mostrar como DIÁLOGO MODAL (solo 1 instancia)
                DialogResult resultado = frmBuscar.ShowDialog();

                if (resultado == DialogResult.OK)
                {
                    // La cuenta ya fue actualizada por el método público
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ABRIR BÚSQUEDA: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion PartidaContable
        #region CheckBoxComplemento
        private void CheckBox_Complemento_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_Complemento.Checked)
            {
                Txt_Complemento.Enabled = true;
                Txt_Complemento.ReadOnly = false;
                Txt_Complemento.Text = "";
                Txt_Complemento.ForeColor = Color.Black;
                Txt_Complemento.Focus();
                CheckBox_LastComplement.Enabled = true;  // HABILITAR LASTCOMPLEMENT
            }
            else
            {
                Txt_Complemento.Enabled = false;
                Txt_Complemento.ReadOnly = true;
                Txt_Complemento.Text = "N/A";
                Txt_Complemento.ForeColor = Color.Gray;
                CheckBox_LastComplement.Enabled = false;  // DESHABILITAR LASTCOMPLEMENT
                CheckBox_LastComplement.Checked = false;
            }

            ValidarExencionManual();
        }
        // Evento cuando cambia el estado del CheckBox_LastComplement
        private void CheckBox_LastComplement_CheckedChanged(object sender, EventArgs e)
        {
            // Solo validar si se está activando (no al desactivar)
            if (CheckBox_LastComplement.Checked)
            {
                // Verificar que haya un complemento activo
                if (!CheckBox_Complemento.Checked || string.IsNullOrWhiteSpace(Txt_Complemento.Text))
                {
                    MessageBox.Show(
                        "DEBE TENER UN COMPLEMENTO ACTIVO PARA MARCAR COMO ÚLTIMO COMPLEMENTO.",
                        "VALIDACIÓN",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    CheckBox_LastComplement.Checked = false;
                    return;
                }

                // Recalcular valores para mostrar el resultado
                CalcularValores();

                // Verificar si el valor impreso es negativo
                decimal valorImpreso = ObtenerValorDecimal(Txt_ValorImpresoTransferencia);
                if (valorImpreso < 0)
                {
                    MessageBox.Show(
                        $"ADVERTENCIA: EL VALOR IMPRESO ES NEGATIVO (Q.{valorImpreso:N2}).\n\n" +
                        "LOS COMPLEMENTOS SUMAN MÁS QUE EL MONTO TOTAL.\n" +
                        "VERIFIQUE LOS MONTOS ANTES DE CONTINUAR.",
                        "FONDOS INSUFICIENTES",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            else
            {
                // Al desactivar, recalcular sin restar complementos
                CalcularValores();
            }
        }
        private void Txt_Complemento_Leave(object sender, EventArgs e)
        {
            // SOLO VALIDAR SI ESTÁ ACTIVO Y TIENE TEXTO
            if (!CheckBox_Complemento.Checked || string.IsNullOrWhiteSpace(Txt_Complemento.Text))
                return;

            string numeroTransferencia = Txt_Complemento.Text.Trim();

            // VALIDACIÓN 1: Solo números
            if (!int.TryParse(numeroTransferencia, out int _))
            {
                MessageBox.Show(
                    "DEBE INGRESAR UN NÚMERO DE TRANSFERENCIA VÁLIDO",
                    "VALIDACIÓN",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Txt_Complemento.Text = "";
                Txt_Complemento.Focus();
                return;
            }

            // VALIDACIÓN 2: Existencia en BD
            if (!Ctrl_Transfers.ValidarExistenciaTransfer(numeroTransferencia))
            {
                MessageBox.Show(
                    $"LA TRANSFERENCIA NO. {numeroTransferencia} NO EXISTE EN LA BASE DE DATOS.\n" +
                    "VERIFIQUE EL NÚMERO E INTENTE NUEVAMENTE.",
                    "TRANSFERENCIA NO ENCONTRADA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Txt_Complemento.Text = "";
                Txt_Complemento.Focus();
                return;
            }

            // VALIDACIÓN 3: Verificar si la transferencia ya tiene LastComplement = 1
            if (Ctrl_Transfers.ValidarLastComplement(numeroTransferencia))
            {
                MessageBox.Show(
                    $"LA TRANSFERENCIA NO. {numeroTransferencia} YA FUE MARCADA COMO COMPLETADA.\n" +
                    "NO SE PUEDEN AGREGAR MÁS COMPLEMENTOS A ESTA TRANSFERENCIA.\n\n" +
                    "El proceso de complementos para esta transferencia ha sido cerrado.",
                    "TRANSFERENCIA COMPLETADA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Txt_Complemento.Text = "";
                Txt_Complemento.Focus();
                return;
            }

            // VALIDACIÓN EXITOSA
            MessageBox.Show(
                $"TRANSFERENCIA NO. {numeroTransferencia} ENCONTRADA Y VALIDADA CORRECTAMENTE.",
                "VALIDACIÓN EXITOSA",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // DESPUÉS DE VALIDAR, VERIFICAR EXENCIÓN MANUAL
            ValidarExencionManual();
        }

        // ⭐ VALIDACIÓN EN TIEMPO REAL (OPCIONAL)
        private void Txt_Complemento_TextChanged(object sender, EventArgs e)
        {
            // Solo mostrar advertencia visual si no es número
            if (!string.IsNullOrEmpty(Txt_Complemento.Text) &&
                !int.TryParse(Txt_Complemento.Text, out _))
            {
                Txt_Complemento.ForeColor = Color.Red;
            }
            else
            {
                Txt_Complemento.ForeColor = Color.Black;
            }
        }
        #endregion CheckBoxComplemento
        #region BloquearCampos
        private void BloquearCamposIniciales()
        {
            // Campos calculados automáticamente - DESHABILITADOS
            
            Txt_Exencion.Enabled = false;
            Txt_MontoSinITH.Enabled = false;
            Txt_ITH.Enabled = false;
            Txt_IGSS.Enabled = false;
            Txt_RetencionISR.Enabled = false; // ⭐ DESHABILITADO INICIALMENTE
            Txt_Stamps.Enabled = false;
            Txt_Indemnizacion.Enabled = false;
            Txt_Vacaciones.Enabled = false;
            Txt_Aguinaldo.Enabled = false;
            Txt_ValorImpresoTransferencia.Enabled = false;

            // Campos desde formularios externos - DESHABILITADOS
            Txt_Cuenta.Enabled = false;
            Txt_Beneficiario.Enabled = false;
            Txt_Codigo.Enabled = false;
            Txt_DescripcionCuenta.Enabled = false;

            // ⭐ CAMPOS SIEMPRE HABILITADOS
            Txt_NoTransferencia.Enabled = true;
            Txt_Alimentacion.Enabled = true;
            Txt_Bonificacion.Enabled = true;
            Txt_Anticipos.Enabled = true;
            Txt_Viaticos.Enabled = true;
            Txt_Descuentos.Enabled = true;

            // ⭐ ISR se habilita/deshabilita según tipo de TRANSFERENCIA (en ComboBox_SelectedIndexChanged)

            // Aplicar estilo a deshabilitados
            ConfigurarEstiloDeshabilitado(Txt_Cuenta);
            ConfigurarEstiloDeshabilitado(Txt_Beneficiario);
            ConfigurarEstiloDeshabilitado(Txt_Exencion);
            ConfigurarEstiloDeshabilitado(Txt_MontoSinITH);
            ConfigurarEstiloDeshabilitado(Txt_ITH);
            ConfigurarEstiloDeshabilitado(Txt_IGSS);
            ConfigurarEstiloDeshabilitado(Txt_Codigo);
            ConfigurarEstiloDeshabilitado(Txt_DescripcionCuenta);
        }
        private void ConfigurarComplemento()
        {
            Txt_Complemento.Enabled = false;
            Txt_Complemento.Text = "N/A";
            Txt_Complemento.ReadOnly = true;
            Txt_Complemento.MaxLength = 10;

            CheckBox_Complemento.Checked = false;
            CheckBox_Complemento.CheckedChanged += CheckBox_Complemento_CheckedChanged;

            Txt_Complemento.Leave += Txt_Complemento_Leave;
            Txt_Complemento.TextChanged += Txt_Complemento_TextChanged; // ⭐ NUEVO
            Txt_Complemento.KeyPress += ValidarSoloNumeros_KeyPress;

            // Configurar CheckBox_LastComplement
            CheckBox_LastComplement.Checked = false;
            CheckBox_LastComplement.Enabled = false;  // Empieza deshabilitado
            CheckBox_LastComplement.CheckedChanged += CheckBox_LastComplement_CheckedChanged;
        }
        private void ConfigurarEstiloDeshabilitado(TextBox textBox)
        {
            textBox.BackColor = Color.FromArgb(240, 240, 240);
            textBox.ForeColor = Color.FromArgb(100, 100, 100);
        }
        #endregion BloquearCampos
        #region EstiloTextBoxTotales
        private void CrearTextBoxConPadding(TextBox textBox, bool deshabilitarCompletamente = true)
        {
            // Guardar información original
            Point ubicacionOriginal = textBox.Location;
            Size tamañoOriginal = textBox.Size;
            Control contenedorPadre = textBox.Parent;
            string nombreOriginal = textBox.Name;
            string textoOriginal = textBox.Text;

            // Crear Panel contenedor
            Panel panelContenedor = new Panel
            {
                Location = ubicacionOriginal,
                Size = new Size(tamañoOriginal.Width, Math.Max(tamañoOriginal.Height, 45)),
                BackColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.None,
                Padding = new Padding(6, 6, 12, 8),
                Name = "Panel_" + nombreOriginal
            };

            // Aplicar esquinas redondeadas
            panelContenedor.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, panelContenedor.Width, panelContenedor.Height, 15, 15));

            // Configurar el TextBox
            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = Color.FromArgb(60, 60, 60);
            textBox.ForeColor = Color.White;
            textBox.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            textBox.Dock = DockStyle.Fill;
            textBox.Text = textoOriginal;
            textBox.TextAlign = HorizontalAlignment.Right;

            // ⭐ CONFIGURACIÓN SEGÚN PARÁMETRO
            if (deshabilitarCompletamente)
            {
                textBox.Enabled = false;
                textBox.Cursor = Cursors.No;
            }
            else
            {
                textBox.Enabled = true;  // ⭐ HABILITADO PARA EDICIÓN
                textBox.ReadOnly = false; // ⭐ PERMITE ESCRIBIR
                textBox.Cursor = Cursors.IBeam; // ⭐ CURSOR DE TEXTO
            }

            // Forzar colores siempre
            textBox.EnabledChanged += (s, e) =>
            {
                textBox.ForeColor = Color.White;
                textBox.BackColor = Color.FromArgb(60, 60, 60);
            };

            // Remover el TextBox original
            contenedorPadre.Controls.Remove(textBox);

            // Agregar al panel
            panelContenedor.Controls.Add(textBox);

            // Agregar el panel al contenedor original
            contenedorPadre.Controls.Add(panelContenedor);

            // Configurar eventos
            ConfigurarEventosPanel(panelContenedor, textBox);
        }

        // ⭐ MÉTODO QUE FALTABA
        private void ConfigurarEventosPanel(Panel panel, TextBox textBox)
        {
            // Efecto hover
            panel.MouseEnter += (s, e) =>
            {
                panel.BackColor = Color.FromArgb(70, 70, 70);
                textBox.BackColor = Color.FromArgb(70, 70, 70);
                textBox.ForeColor = Color.White;
            };

            panel.MouseLeave += (s, e) =>
            {
                panel.BackColor = Color.FromArgb(60, 60, 60);
                textBox.BackColor = Color.FromArgb(60, 60, 60);
                textBox.ForeColor = Color.White;
            };
        }

        // Método para aplicar a todos los TextBox de totales
        private void AplicarEstiloTextBoxTotales()
        {
            CrearTextBoxConPadding(Txt_ValorImpresoTransferencia, true);
            CrearTextBoxConPadding(Txt_TotalCargos, true);
            CrearTextBoxConPadding(Txt_TotalAbonos, true);
            CrearTextBoxConPadding(Txt_ValorLetras, false);
        }
        #endregion EstiloTextBoxTotales
        #region EstiloBotones
        // Aplicar estilo específico al botón Agregar Cuenta
        private void AplicarEstiloBotonAgregar(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(9, 184, 255);
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            boton.Height = 45;
            boton.Width = Math.Max(boton.Width, 180);
            boton.Cursor = Cursors.Hand;
            boton.TextAlign = ContentAlignment.MiddleCenter;

            // Esquinas redondeadas
            boton.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, boton.Width, boton.Height, 20, 20));

            // Efectos hover
            boton.MouseEnter += (s, e) =>
            {
                boton.BackColor = Color.FromArgb(0, 150, 220);
            };

            boton.MouseLeave += (s, e) =>
            {
                boton.BackColor = Color.FromArgb(9, 184, 255);
            };
        }
        // Aplicar estilo a todos los botones
        private void AplicarEstilosBotones()
        {
            AplicarEstiloBotonAgregar(Btn_AddCuenta);
        }
        #endregion EstiloBotones
        #region ConfigurarTabla
        // Método para configurar el DataGridView de Partida Contable
        private void ConfigurarTabla()
        {
            // Limpiar columnas existentes si las hay
            Tabla.Columns.Clear();

            // Agregar columnas manualmente
            Tabla.Columns.Add("Codigo", "CÓDIGO");
            Tabla.Columns.Add("Cuenta", "CUENTA");
            Tabla.Columns.Add("Descripcion", "DESCRIPCIÓN");
            Tabla.Columns.Add("Cargo", "CARGO");
            Tabla.Columns.Add("Abono", "ABONO");

            // Configuración visual y de comportamiento
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToResizeRows = false;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            // Estilos de encabezados
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            // Estilos de filas
            Tabla.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // Altura de filas
            Tabla.RowTemplate.Height = 30;

            // Bordes
            Tabla.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            // Ajustar columnas
            AjustarColumnas();

            // Agregar evento de selección
            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        // Método para ajustar el ancho de las columnas
        private void AjustarColumnas()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["Codigo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Codigo"].FillWeight = 15; // 15% del espacio

                Tabla.Columns["Cuenta"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Cuenta"].FillWeight = 30; // 30% del espacio

                Tabla.Columns["Descripcion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Descripcion"].FillWeight = 35; // 35% del espacio

                Tabla.Columns["Cargo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Cargo"].FillWeight = 10; // 10% del espacio

                Tabla.Columns["Abono"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla.Columns["Abono"].FillWeight = 10; // 10% del espacio
            }
        }

        // Evento al cambiar selección en la tabla
        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count > 0)
            {
                CargarDatosCuentaSeleccionada();

                // Guardar índice de fila seleccionada
                _filaSeleccionadaIndex = Tabla.SelectedRows[0].Index;

                // Cambiar estado de botones
                Btn_AddCuenta.Enabled = false;
                Btn_Update.Enabled = true;
                Btn_Remove.Enabled = true; // Si tienes botón eliminar
            }
            else
            {
                _filaSeleccionadaIndex = -1;
                Btn_AddCuenta.Enabled = true;
                Btn_Update.Enabled = false;
                Btn_Remove.Enabled = false; // Si tienes botón eliminar
            }
        }

        // Método para cargar datos de la cuenta seleccionada
        private void CargarDatosCuentaSeleccionada()
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0) return;

                DataGridViewRow fila = Tabla.SelectedRows[0];

                // Cargar datos en los TextBox
                Txt_Codigo.Text = fila.Cells["Codigo"].Value?.ToString() ?? "";
                Txt_Codigo.ForeColor = Color.Black;

                Txt_Cuenta.Text = fila.Cells["Cuenta"].Value?.ToString() ?? "";
                Txt_Cuenta.ForeColor = Color.Black;

                Txt_DescripcionCuenta.Text = fila.Cells["Descripcion"].Value?.ToString() ?? "";
                Txt_DescripcionCuenta.ForeColor = Color.Black;

                Txt_Cargos.Text = fila.Cells["Cargo"].Value?.ToString() ?? "0.00";
                Txt_Cargos.ForeColor = Color.Black;

                Txt_Abonos.Text = fila.Cells["Abono"].Value?.ToString() ?? "0.00";
                Txt_Abonos.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR CUENTA: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion ConfigurarTabla
        #region ScrollBar
        // Inicializar configuración del vScrollBar
        private void InicializarScroll()
        {
            // NOTA: Reemplaza "Panel_Izquierdo" con el nombre real de tu panel
            // Si NO tienes panel con scroll, puedes eliminar esta región completa

            if (Panel_Izquierdo == null || vScrollBar == null)
                return;

            // Primero guardar posiciones originales
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                if (ctrl.Tag == null || !ctrl.Tag.ToString().StartsWith("OrigY:"))
                {
                    ctrl.Tag = "OrigY:" + ctrl.Top;
                }
            }

            // Calcular altura total del contenido
            int maxBottom = 0;
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                maxBottom = Math.Max(maxBottom, ctrl.Bottom);
            }

            int totalContentHeight = maxBottom + (Panel_Izquierdo.Height / 3);

            // Si no necesita scroll, ocultar scrollbar
            if (totalContentHeight <= Panel_Izquierdo.Height)
            {
                vScrollBar.Visible = false;
                return;
            }

            // Configurar scrollbar
            vScrollBar.Visible = true;
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = totalContentHeight - Panel_Izquierdo.Height;
            vScrollBar.SmallChange = 30;
            vScrollBar.LargeChange = Panel_Izquierdo.Height / 4;

            vScrollBar.Scroll -= vScrollBar_Scroll;
            vScrollBar.Scroll += vScrollBar_Scroll;
            vScrollBar.Value = 0;
        }
        // Configurar eventos MouseWheel para Panel y controles hijos
        private void ConfigurarEventosScroll()
        {
            if (Panel_Izquierdo == null || vScrollBar == null)
                return;

            Panel_Izquierdo.TabStop = true;
            Panel_Izquierdo.MouseWheel += Panel_Izquierdo_MouseWheel;
            Panel_Izquierdo.MouseEnter += Panel_Izquierdo_MouseEnter;

            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                // ⭐ EXCLUIR COMBOBOX DEL SCROLL
                if (!(ctrl is ComboBox))
                {
                    ctrl.MouseWheel += Panel_Izquierdo_MouseWheel;
                }
            }
        }
        // Evento MouseEnter del Panel
        private void Panel_Izquierdo_MouseEnter(object sender, EventArgs e)
        {
            Panel_Izquierdo.Focus();
        }

        // Evento Scroll del vScrollBar
        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            int scrollPosition = vScrollBar.Value;

            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                // ⭐ EXCLUIR COMBOBOX DEL MOVIMIENTO
                if (ctrl is ComboBox)
                    continue;

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

        // Evento MouseWheel del Panel
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

        // Método para mover contenido del panel
        private void MoverContenido(int scrollPosition)
        {
            foreach (Control ctrl in Panel_Izquierdo.Controls)
            {
                // ⭐ EXCLUIR COMBOBOX DEL MOVIMIENTO
                if (ctrl is ComboBox)
                    continue;

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
        #endregion ScrollBar
        #region DeshabilitarScrollComboBox
        private void ConfigurarComboBoxSinScroll()
        {
            // Lista de todos los ComboBox del formulario
            ComboBox_TipoTransferencia.MouseWheel += ComboBox_MouseWheel_Disabled;
            ComboBox_Location.MouseWheel += ComboBox_MouseWheel_Disabled;
            ComboBox_Banco.MouseWheel += ComboBox_MouseWheel_Disabled;
            ComboBox_TipoCuenta.MouseWheel += ComboBox_MouseWheel_Disabled;
            // ⭐ Agrega aquí los demás ComboBox que tengas
        }

        private void ComboBox_MouseWheel_Disabled(object sender, MouseEventArgs e)
        {
            // Cancelar evento de scroll en ComboBox
            ((HandledMouseEventArgs)e).Handled = true;
        }
        #endregion DeshabilitarScrollComboBox
        #region BuscarBeneficiario
        // Método público para actualizar beneficiario
        public void ActualizarBeneficiario(string beneficiario)
        {
            Txt_Beneficiario.Text = beneficiario;
            Txt_Beneficiario.ForeColor = Color.Black;
        }

        // Evento del botón Buscar Beneficiario
        private void Btn_SearchBeneficiario_Click(object sender, EventArgs e)
        {
            try
            {
                Frm_Transfers_SearchBeneficiario frmBuscar = new Frm_Transfers_SearchBeneficiario(this);
                DialogResult resultado = frmBuscar.ShowDialog();

                if (resultado == DialogResult.OK)
                {
                    // El beneficiario ya fue actualizado
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ABRIR BÚSQUEDA: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion BuscarBeneficiario
        #region AsignacionFocus
        private void ConfigurarTabIndexYFocus()
        {
            // CONFIGURAR TAB INDEX DE TODOS LOS CONTROLES

            // Sección: Datos del TRANSFERENCIA
            DTP_Emision.TabIndex = 0;
            Txt_NoTransferencia.TabIndex = 1;
            ComboBox_TipoTransferencia.TabIndex = 2;
            ComboBox_Location.TabIndex = 3;

            // Sección: Beneficiario y Concepto
            Txt_Beneficiario.TabIndex = 4;
            Txt_Concepto.TabIndex = 5;
            Txt_Observaciones.TabIndex = 6;

            // Sección: Partida Contable
            Txt_Codigo.TabIndex = 7;
            Txt_Cuenta.TabIndex = 8;
            Txt_DescripcionCuenta.TabIndex = 9;
            Txt_Cargos.TabIndex = 10;
            Txt_Abonos.TabIndex = 11;
            Btn_AddCuenta.TabIndex = 12;

            // Sección: Cálculos
            Txt_MontoTotal.TabIndex = 13;
            CheckBox_Exencion.TabIndex = 14;
            Txt_Alimentacion.TabIndex = 15;
            Txt_Bonificacion.TabIndex = 16;
            Txt_Descuentos.TabIndex = 17;
            Txt_Anticipos.TabIndex = 18;
            Txt_RetencionISR.TabIndex = 19;
            Txt_Viaticos.TabIndex = 20;
            Txt_Stamps.TabIndex = 21;
            Txt_Indemnizacion.TabIndex = 22;
            Txt_Vacaciones.TabIndex = 23;
            Txt_Aguinaldo.TabIndex = 24;  // NUEVO

            // Sección: Complemento y Orden de Compra
            CheckBox_Complemento.TabIndex = 25;
            Txt_Complemento.TabIndex = 26;
            CheckBox_LastComplement.TabIndex = 27;  // NUEVO
            Txt_OrdenCompra.TabIndex = 28;

            // Botones
            Btn_Imprimir.TabIndex = 29;
            Btn_ClearCuenta.TabIndex = 30;
            Btn_ClearPartida.TabIndex = 31;

            // VALIDACIÓN NUMÉRICA EN TODOS LOS CAMPOS QUE REQUIEREN SOLO NÚMEROS
            Txt_MontoTotal.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_Alimentacion.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_Bonificacion.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_Descuentos.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_Anticipos.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_Viaticos.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_Stamps.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_Indemnizacion.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_Vacaciones.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_Aguinaldo.KeyPress += ValidarSoloNumeros_KeyPress;  // NUEVO
            Txt_Cargos.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_Abonos.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_RetencionISR.KeyPress += ValidarSoloNumeros_KeyPress;
            Txt_Complemento.KeyPress += ValidarSoloNumeros_KeyPress;

            // Establecer foco inicial en el primer campo
            Txt_Codigo.Focus();
        }
        #endregion AsignacionFocus
        #region BotonesLimpieza
        // Limpiar solo los campos de la cuenta que se está ingresando
        private void Btn_ClearCuenta_Click(object sender, EventArgs e)
        {
            LimpiarCamposPartida();
            Tabla.ClearSelection();
            _filaSeleccionadaIndex = -1;

            // Restaurar estado de botones
            Btn_AddCuenta.Enabled = true;
            Btn_Update.Enabled = false;
            Btn_Remove.Enabled = false;
        }

        private void LimpiarCamposPartida()
        {
            Txt_Codigo.Text = "CÓDIGO CUENTA";
            Txt_Codigo.ForeColor = Color.Gray;

            Txt_Cuenta.Text = "NOMBRE DE CUENTA";
            Txt_Cuenta.ForeColor = Color.Gray;

            Txt_DescripcionCuenta.Text = "DESCRIPCIÓN/DETALLE";
            Txt_DescripcionCuenta.ForeColor = Color.Gray;

            Txt_Cargos.Text = "0.00";
            Txt_Cargos.ForeColor = Color.Gray;

            Txt_Abonos.Text = "0.00";
            Txt_Abonos.ForeColor = Color.Gray;

            Txt_Codigo.Focus();
        }

        private void LimpiarInformacionTRANSFERENCIA()
        {
            Txt_Beneficiario.Text = "NOMBRE DEL BENEFICIARIO";
            Txt_Beneficiario.ForeColor = Color.Gray;
            Txt_Concepto.Text = "CONCEPTO DEL TRANSFERENCIA";
            Txt_Concepto.ForeColor = Color.Gray;
            Txt_Observaciones.Text = "OBSERVACIONES/DETALLES";
            Txt_Observaciones.ForeColor = Color.Gray;
            ComboBox_TipoTransferencia.SelectedIndex = 0;
            CheckBox_Exencion.Checked = false;
            Txt_MontoTotal.Text = "0.00";
            Txt_MontoTotal.ForeColor = Color.Gray;
            Txt_Alimentacion.Text = "0.00";
            Txt_Alimentacion.ForeColor = Color.Gray;
            Txt_Bonificacion.Text = "0.00";
            Txt_Bonificacion.ForeColor = Color.Gray;
            Txt_Descuentos.Text = "0.00";
            Txt_Descuentos.ForeColor = Color.Gray;
            Txt_Anticipos.Text = "0.00";
            Txt_Anticipos.ForeColor = Color.Gray;
            Txt_Viaticos.Text = "0.00";
            Txt_Viaticos.ForeColor = Color.Gray;
            Txt_Stamps.Text = "0.00";
            Txt_Stamps.ForeColor = Color.Gray;
            Txt_RetencionISR.Text = "0.00";
            Txt_RetencionISR.ForeColor = Color.Gray;
            Txt_Indemnizacion.Text = "0.00";
            Txt_Indemnizacion.ForeColor = Color.Gray;
            Txt_Vacaciones.Text = "0.00";
            Txt_Vacaciones.ForeColor = Color.Gray;
            Txt_Aguinaldo.Text = "0.00";  // NUEVO
            Txt_Aguinaldo.ForeColor = Color.Gray;
            Txt_OrdenCompra.Text = "0";
            Txt_OrdenCompra.ForeColor = Color.Gray;

            // Limpiar complemento
            CheckBox_Complemento.Checked = false;
            CheckBox_LastComplement.Checked = false;  // NUEVO

            LimpiarCalculos();
        }

        // Limpiar solo la partida contable (tabla)
        private void Btn_ClearPartida_Click(object sender, EventArgs e)
        {
            if (Tabla.Rows.Count == 0)
            {
                MessageBox.Show("LA PARTIDA CONTABLE ESTÁ VACÍA",
                              "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmacion = MessageBox.Show(
                "¿DESEA LIMPIAR TODA LA PARTIDA CONTABLE?",
                "CONFIRMAR LIMPIEZA",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion == DialogResult.Yes)
            {
                Tabla.Rows.Clear();
                _partidaContable.Clear();

                Txt_TotalCargos.Text = "0.00";
                Txt_TotalAbonos.Text = "0.00";

                LimpiarCamposPartida();
            }
        }
        #endregion BotonesLimpieza
        #region SistemaImpresion
        // Variables para impresión
        private PrintDocument printDocument;
        private PrintPreviewDialog printPreviewDialog;
        private DatosRecibo datosRecibo;

        // Clase para datos del recibo
        public class DatosRecibo
        {
            public string Fecha { get; set; }
            public string Entidad { get; set; }
            public string MontoLetras { get; set; }
            public string Concepto { get; set; }
            public string RecibidoDe { get; set; } = "UNIVERSIDAD REGIONAL DE GUATEMALA";
            public string Observaciones { get; set; }
            public string Sede { get; set; }
            public string ValorImpreso { get; set; }

            // Lista dinámica de conceptos (solo los que tienen valor > 0)
            public List<ItemImpresion> Items { get; set; } = new List<ItemImpresion>();
        }

        public class ItemImpresion
        {
            public string Descripcion { get; set; }
            public string Signo { get; set; }
            public string Monto { get; set; }
        }
        // Inicializar sistema de impresión
        private void InicializarImpresion()
        {
            datosRecibo = new DatosRecibo();

            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;
            printDocument.DefaultPageSettings.PaperSize = new PaperSize("Letter", 850, 1100);
            printDocument.DefaultPageSettings.Margins = new Margins(25, 25, 25, 25);

            printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.Width = 800;
            printPreviewDialog.Height = 600;
            printPreviewDialog.UseAntiAlias = true;
            printPreviewDialog.ShowIcon = false;
            printPreviewDialog.WindowState = FormWindowState.Maximized;
            printPreviewDialog.Load += PrintPreviewDialog_Load;
        }
        // Personalizar PrintPreviewDialog
        private void PrintPreviewDialog_Load(object sender, EventArgs e)
        {
            PrintPreviewDialog dialog = sender as PrintPreviewDialog;
            if (dialog != null)
            {
                foreach (Control control in dialog.Controls)
                {
                    if (control is ToolStrip toolStrip)
                    {
                        foreach (ToolStripItem item in toolStrip.Items)
                        {
                            if (item is ToolStripButton button &&
                                (button.Text.Contains("Imprimir") || button.Text.Contains("Print")))
                            {
                                var customPrintButton = new ToolStripButton
                                {
                                    Image = button.Image,
                                    Text = button.Text,
                                    ToolTipText = button.ToolTipText,
                                    DisplayStyle = button.DisplayStyle
                                };
                                customPrintButton.Click += CustomPrintButton_Click;

                                int index = toolStrip.Items.IndexOf(button);
                                toolStrip.Items.RemoveAt(index);
                                toolStrip.Items.Insert(index, customPrintButton);
                                break;
                            }
                        }
                    }
                }
            }
        }
        // Botón personalizado de imprimir
        private void CustomPrintButton_Click(object sender, EventArgs e)
        {
            ImprimirConDialogo();
        }
        // Detectar y preconfigurar FX-890II
        private void DetectarYPreconfigurarFX890II()
        {
            PrinterSettings.StringCollection installedPrinters = PrinterSettings.InstalledPrinters;

            foreach (string printer in installedPrinters)
            {
                if (EsImpresoraFX890II(printer))
                {
                    printDocument.PrinterSettings.PrinterName = printer;
                    ConfigurarResolucionFX890II(printDocument.PrinterSettings);
                    break;
                }
            }
        }
        // Identificar si es FX-890II
        private bool EsImpresoraFX890II(string nombreImpresora)
        {
            string nombre = nombreImpresora.ToUpper();
            return nombre.Contains("FX-890") || nombre.Contains("FX890") ||
                   (nombre.Contains("EPSON") && nombre.Contains("FX") && nombre.Contains("890"));
        }
        // Aplicar optimizaciones según tipo de impresora
        private void AplicarOptimizacionesPorTipoImpresora(PrinterSettings printerSettings)
        {
            string nombreImpresora = printerSettings.PrinterName.ToUpper();

            if (EsImpresoraFX890II(printerSettings.PrinterName))
            {
                ConfigurarFX890IIMaximaCalidad(printerSettings);
            }
            else if (nombreImpresora.Contains("EPSON") && nombreImpresora.Contains("FX"))
            {
                ConfigurarEpsonFXGenerico(printerSettings);
            }
            else
            {
                ConfigurarImpresoraGenerica(printerSettings);
            }
        }
        // Configuración específica FX-890II
        private void ConfigurarFX890IIMaximaCalidad(PrinterSettings printerSettings)
        {
            try
            {
                ConfigurarResolucionFX890II(printerSettings);
                printerSettings.DefaultPageSettings.Color = false;
                printerSettings.Copies = 1;
                printerSettings.Collate = false;
                printDocument.DocumentName = "TRANSFERENCIA_FX890II_MaxCalidad";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error configurando FX-890II: {ex.Message}");
            }
        }
        // Configurar resolución FX-890II
        private void ConfigurarResolucionFX890II(PrinterSettings printerSettings)
        {
            foreach (PrinterResolution resolution in printerSettings.PrinterResolutions)
            {
                if (resolution.X == 180 && resolution.Y == 180)
                {
                    printerSettings.DefaultPageSettings.PrinterResolution = resolution;
                    return;
                }
                else if (resolution.Kind == PrinterResolutionKind.High)
                {
                    printerSettings.DefaultPageSettings.PrinterResolution = resolution;
                }
                else if (resolution.X >= 144 && resolution.Y >= 144)
                {
                    printerSettings.DefaultPageSettings.PrinterResolution = resolution;
                }
            }
        }
        // Configuración EPSON FX genérico
        private void ConfigurarEpsonFXGenerico(PrinterSettings printerSettings)
        {
            printerSettings.DefaultPageSettings.Color = false;

            foreach (PrinterResolution resolution in printerSettings.PrinterResolutions)
            {
                if (resolution.Kind == PrinterResolutionKind.High ||
                    (resolution.X >= 144 && resolution.Y >= 144))
                {
                    printerSettings.DefaultPageSettings.PrinterResolution = resolution;
                    break;
                }
            }
        }
        // Configuración impresora genérica
        private void ConfigurarImpresoraGenerica(PrinterSettings printerSettings)
        {
            foreach (PrinterResolution resolution in printerSettings.PrinterResolutions)
            {
                if (resolution.Kind == PrinterResolutionKind.High)
                {
                    printerSettings.DefaultPageSettings.PrinterResolution = resolution;
                    break;
                }
            }
        }
        // Configuración final antes de imprimir
        private void ConfiguracionFinalAntesDeImprimir()
        {
            printDocument.OriginAtMargins = false;

            if (EsImpresoraFX890II(printDocument.PrinterSettings.PrinterName))
            {
                System.Diagnostics.Debug.WriteLine("✓ Comandos ESC/P configurados para FX-890II");
            }
        }
        // Imprimir con diálogo
        private void ImprimirConDialogo()
        {
            try
            {
                if (PrinterSettings.InstalledPrinters.Count == 0)
                {
                    MessageBox.Show("NO HAY IMPRESORAS INSTALADAS EN EL SISTEMA", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DetectarYPreconfigurarFX890II();

                PrintDialog printDialog = new PrintDialog
                {
                    Document = printDocument,
                    UseEXDialog = true,
                    AllowCurrentPage = true,
                    AllowPrintToFile = true,
                    AllowSelection = false,
                    AllowSomePages = false
                };

                printDialog.PrinterSettings.DefaultPageSettings.Color = false;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.PrinterSettings = printDialog.PrinterSettings;
                    AplicarOptimizacionesPorTipoImpresora(printDialog.PrinterSettings);
                    printPreviewDialog.Hide();
                    ConfiguracionFinalAntesDeImprimir();
                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL IMPRIMIR: {ex.Message}", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // EVENTO PRINCIPAL: Dibujar el recibo en la página
        // EVENTO PRINCIPAL: Dibujar el recibo en la página
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font fontNormal = new Font("Calibri Light", 12);
            Font fontBold = new Font("Calibri Light", 12, FontStyle.Bold);
            Font fontSmall = new Font("Calibri Light", 10);
            Font fontSuperSmall = new Font("Calibri Light", 8);
            Font fontTable = new Font("Calibri", 10);
            Font fontTableBold = new Font("Calibri", 10, FontStyle.Bold);

            Brush brush = Brushes.Black;
            Pen pen = new Pen(Color.Black, 1);

            int currentY = 50;
            int leftMargin = 50;
            int rightMargin = 750;

            // Espacio inicial
            currentY += 18;

            // Tabulaciones
            float tabulacion = leftMargin + 120;
            float tabulacion2 = leftMargin + 5;
            float tabulacion3 = leftMargin + 140;

            // ===== HEADER - Fecha y Monto =====
            g.DrawString($"GUATEMALA, {datosRecibo.Fecha}", fontNormal, brush, tabulacion, currentY);
            currentY -= 5;
            g.DrawString(datosRecibo.ValorImpreso, fontNormal, brush, rightMargin - 65, currentY);
            currentY += 25;

            //// ===== Entidad =====
            //g.DrawString(datosRecibo.Entidad, fontNormal, brush, tabulacion, currentY);
            //currentY += 25;

            // ===== Entidad con autoajuste =====
            // Calcular el ancho disponible para la Entidad
            float anchoDisponibleEntidad = (rightMargin - 65) - tabulacion + 35; // pixeles de margen de seguridad
            RectangleF rectEntidad = new RectangleF(tabulacion, currentY, anchoDisponibleEntidad, 20);

            // Calcular tamaño óptimo de fuente
            Font fontEntidadTemp = CalcularFuenteOptima(g, datosRecibo.Entidad, rectEntidad, 12f, 8f);
            // Crear nueva fuente asegurando que sea Regular (sin negrita)
            Font fontEntidad = new Font("Calibri Light", fontEntidadTemp.Size, FontStyle.Regular);
            fontEntidadTemp.Dispose(); // Liberar la fuente temporal

            g.DrawString(datosRecibo.Entidad, fontEntidad, brush, tabulacion, currentY);
            currentY += 25;

            // ===== Monto en letras =====
            g.DrawString(datosRecibo.MontoLetras, fontNormal, brush, tabulacion, currentY);
            currentY += 80;

            // ===== Emitido =====
            g.DrawString($"EMITIDO:    {datosRecibo.Concepto}", fontSmall, brush, tabulacion2, currentY);
            currentY += 70;

            // ===== SECCIÓN INFERIOR =====
            g.DrawString($"GUATEMALA, {datosRecibo.Fecha}", fontNormal, brush, tabulacion2, currentY);
            currentY += 15;
            g.DrawString($"POR Q.*     {datosRecibo.ValorImpreso}", fontBold, brush, tabulacion3, currentY);
            currentY += 20;

            // YO: Entidad con autoajuste
            string textoYoEntidad = $"YO:  {datosRecibo.Entidad}";
            // Calcular el ancho disponible desde tabulacion2 hasta el margen derecho
            float anchoDisponibleYo = rightMargin - tabulacion2 - 50; // 50 pixeles de margen de seguridad
            RectangleF rectYoEntidad = new RectangleF(tabulacion2, currentY, anchoDisponibleYo, 20);
            // Calcular tamaño óptimo de fuente
            Font fontYoEntidadTemp = CalcularFuenteOptima(g, textoYoEntidad, rectYoEntidad, 12f, 9f);
            // Crear nueva fuente asegurando que sea Regular (sin negrita)
            Font fontYoEntidad = new Font("Calibri Light", fontYoEntidadTemp.Size, FontStyle.Regular);
            fontYoEntidadTemp.Dispose(); // Liberar la fuente temporal
            g.DrawString(textoYoEntidad, fontYoEntidad, brush, tabulacion2, currentY);
            currentY += 20;


            g.DrawString($"RECIBÍ DE:     {datosRecibo.RecibidoDe}", fontNormal, brush, tabulacion2, currentY);
            currentY += 20;
            g.DrawString($"LA CANTIDAD DE:    {datosRecibo.MontoLetras}", fontNormal, brush, tabulacion2, currentY);
            currentY += 25;

            g.DrawString($"CONCEPTO: {datosRecibo.Concepto}", fontSmall, brush, tabulacion2, currentY);
            currentY += 25;

            // ===== TABLA DINÁMICA =====
            int tableX = leftMargin + 35;
            int tableY = currentY;
            int tableWidth = 500;
            int rowHeight = 15;

            // ⭐ CALCULAR NÚMERO DE FILAS DINÁMICAMENTE
            int numeroFilas = datosRecibo.Items.Count;
            int tableHeight = rowHeight * (numeroFilas + 2); // +2 para header y fila final

            // Dibujar bordes de la tabla
            g.DrawRectangle(pen, tableX, tableY, tableWidth, tableHeight);

            // Columnas
            int col1Width = 40;
            int col2Width = 300;
            int col3Width = 40;
            int col4Width = 120;

            // Líneas verticales
            g.DrawLine(pen, tableX + col1Width, tableY, tableX + col1Width, tableY + tableHeight);
            g.DrawLine(pen, tableX + col1Width + col2Width, tableY, tableX + col1Width + col2Width, tableY + tableHeight);
            g.DrawLine(pen, tableX + col1Width + col2Width + col3Width, tableY, tableX + col1Width + col2Width + col3Width, tableY + tableHeight);

            // Header
            g.DrawLine(pen, tableX, tableY + rowHeight, tableX + tableWidth, tableY + rowHeight);
            g.DrawString("NO.", fontTableBold, brush, tableX + 10, tableY);
            g.DrawString("DESCRIPCIÓN", fontTableBold, brush, tableX + col1Width + 80, tableY);
            g.DrawString("±", fontTableBold, brush, tableX + col1Width + col2Width + 15, tableY);
            g.DrawString("SUMA", fontTableBold, brush, tableX + col1Width + col2Width + col3Width + 40, tableY);

            // ⭐ FILAS DINÁMICAS (solo las que tienen datos)
            for (int i = 0; i < numeroFilas; i++)
            {
                int rowY = tableY + rowHeight + (i * rowHeight);

                if (i < numeroFilas - 1)
                    g.DrawLine(pen, tableX, rowY + rowHeight, tableX + tableWidth, rowY + rowHeight);

                g.DrawString((i + 1).ToString(), fontTable, brush, tableX + 15, rowY);
                g.DrawString(datosRecibo.Items[i].Descripcion, fontTable, brush, tableX + col1Width + 5, rowY);
                g.DrawString(datosRecibo.Items[i].Signo, fontTable, brush, tableX + col1Width + col2Width + 15, rowY);
                g.DrawString(datosRecibo.Items[i].Monto, fontTable, brush, tableX + col1Width + col2Width + col3Width + 10, rowY);
            }

            // Fila final
            int finalRowY = tableY + rowHeight + (numeroFilas * rowHeight);
            g.DrawLine(pen, tableX, finalRowY, tableX + tableWidth, finalRowY);
            g.DrawString((numeroFilas + 1).ToString(), fontTable, brush, tableX + 15, finalRowY);
            g.DrawString("SUMA LIQUIDADA ESTE PAGO", fontTable, brush, tableX + col1Width + 5, finalRowY);
            g.DrawString(datosRecibo.ValorImpreso, fontTable, brush, tableX + col1Width + col2Width + col3Width + 10, finalRowY);

            // ===== CUADRO SEDE =====
            int sedeX = tableX + tableWidth + 20;
            int sedeY = tableY;
            int sedeWidth = 170;
            int sedeHeight = tableHeight;

            g.DrawRectangle(pen, sedeX, sedeY, sedeWidth, sedeHeight);

            string textoSede = datosRecibo.Sede;
            if (ComboBox_Location.Text == "CENTRAL")
                textoSede = "OFICINAS CENTRALES";

            StringFormat formato = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.LineLimit
            };

            RectangleF rectTexto = new RectangleF(sedeX + 5, sedeY + 5, sedeWidth - 10, sedeHeight - 10);
            Font fontSede = CalcularFuenteOptima(g, textoSede, rectTexto, 11.5f, 8f);
            g.DrawString(textoSede, fontSede, brush, rectTexto, formato);

            // ===== OBSERVACIONES =====
            currentY = finalRowY + 20;
            string textoObservaciones = $"DETALLE: {datosRecibo.Observaciones}";
            RectangleF rectObservaciones = new RectangleF(tabulacion2, currentY, rightMargin - tabulacion2, 110);
            Font fontObservaciones = CalcularFuenteOptima(g, textoObservaciones, rectObservaciones, 11.5f, 8f);

            StringFormat formatoObs = new StringFormat { FormatFlags = StringFormatFlags.LineLimit };
            g.DrawString(textoObservaciones, fontObservaciones, brush, rectObservaciones, formatoObs);

            // ===== USUARIO RESPONSABLE =====
            currentY += 115;
            Mdl_Users usuarioActual = Ctrl_Users.ObtenerUsuarioPorId(UserData.UserId);
            string nombreUsuario = usuarioActual?.FullName ?? "USUARIO DESCONOCIDO";
            g.DrawString($"USUARIO RESPONSABLE:  {nombreUsuario}", fontSuperSmall, brush, tabulacion2, currentY);

            // Liberar recursos
            fontNormal.Dispose();
            fontBold.Dispose();
            fontSmall.Dispose();
            fontSuperSmall.Dispose();
            fontTable.Dispose();
            fontTableBold.Dispose();
            fontSede?.Dispose();
            fontObservaciones?.Dispose();
            pen.Dispose();
            formato?.Dispose();
            formatoObs?.Dispose();
        }
        // Método auxiliar para calcular fuente óptima
        private Font CalcularFuenteOptima(Graphics g, string texto, RectangleF rectangulo, float tamañoMaximo = 10f, float tamañoMinimo = 6f)
        {
            Font fuenteOptima = new Font("Calibri Light", tamañoMaximo, FontStyle.Bold);
            StringFormat formato = new StringFormat { FormatFlags = StringFormatFlags.LineLimit };

            for (float tamaño = tamañoMaximo; tamaño >= tamañoMinimo; tamaño -= 0.5f)
            {
                if (fuenteOptima != null && tamaño != tamañoMaximo)
                    fuenteOptima.Dispose();

                fuenteOptima = new Font("Calibri Light", tamaño, FontStyle.Bold);
                SizeF tamañoTexto = g.MeasureString(texto, fuenteOptima, (int)rectangulo.Width, formato);

                if (tamañoTexto.Height <= rectangulo.Height)
                {
                    formato.Dispose();
                    return fuenteOptima;
                }
            }

            formato.Dispose();
            return fuenteOptima;
        }
        #endregion SistemaImpresion
        #region ConfiguracionAutomatica
        // Configurar periodo y banco automáticamente
        private void ConfigurarPeriodoYBanco()
        {
            // PERIODO: solo lectura, como en TRANSFERENCIAs
            Txt_Periodo.Text = ObtenerMesActual();
            Txt_Periodo.Enabled = false;
            Txt_Periodo.ReadOnly = true;
            ConfigurarEstiloDeshabilitado(Txt_Periodo);

            // BANCO Y TIPO DE CUENTA:
            // Se seleccionan desde los ComboBox, no desde un TextBox.
            // Si quieres dejar un valor por defecto, puedes forzar el índice aquí.
            if (ComboBox_Banco.Items.Count > 0 && ComboBox_Banco.SelectedIndex < 0)
                ComboBox_Banco.SelectedIndex = 0;

            if (ComboBox_TipoCuenta.Items.Count > 0 && ComboBox_TipoCuenta.SelectedIndex < 0)
                ComboBox_TipoCuenta.SelectedIndex = 0;
        }

        // Obtener nombre del mes actual en español
        private string ObtenerMesActual()
        {
            string[] meses = new string[]
            {
        "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO",
        "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
            };

            int mesActual = DateTime.Now.Month; // 1-12
            return meses[mesActual - 1]; // Ajustar índice (0-11)
        }
        #endregion ConfiguracionAutomatica
        #region FinalizarTransferencia
        // Evento del botón Btn_Imprimir
        private void Btn_Imprimir_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // ===== VALIDACIONES =====
                if (!ValidarDatosCompletos())
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                if (!ValidarPartidaContable())
                {
                    this.Cursor = Cursors.Default;
                    return;
                }
                // VALIDAR NÚMERO DE TRANSFERENCIA
                if (string.IsNullOrWhiteSpace(ObtenerTextoReal(Txt_NoTransferencia)))
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("DEBE INGRESAR EL NÚMERO DE TRANSFERENCIA.",
                                    "VALIDACIÓN DE DATOS",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    Txt_NoTransferencia.Focus();
                    return;
                }

                // ===== ACTUALIZAR DATOS Y MOSTRAR VISTA PREVIA =====
                ActualizarDatosRecibo();

                if (PrinterSettings.InstalledPrinters.Count == 0)
                {
                    MessageBox.Show("NO HAY IMPRESORAS INSTALADAS", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }

                // ⭐ FORZAR REFRESCO DEL DOCUMENTO (SOLUCIÓN AL CACHE)
                printDocument = new PrintDocument();
                printDocument.PrintPage += PrintDocument_PrintPage;
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("Letter", 850, 1100);
                printDocument.DefaultPageSettings.Margins = new Margins(25, 25, 25, 25);
                printPreviewDialog.Document = printDocument;

                // Mostrar vista previa (usuario puede imprimir desde aquí)
                printPreviewDialog.ShowDialog(this);

                // ===== CONFIRMAR FINALIZACIÓN =====
                var confirmacion = MessageBox.Show(
                    "¿DESEA FINALIZAR EL PROCESO DEL TRANSFERENCIA EN EL SISTEMA?",
                    "CONFIRMAR FINALIZACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                // Capturar datos del TRANSFERENCIA ANTES de registrar
                string numeroTRANSFERENCIAActual = Txt_NoTransferencia.Text;
                string beneficiarioActual = ObtenerTextoReal(Txt_Beneficiario).ToUpper();
                decimal valorImpresoActual = ObtenerValorDecimal(Txt_ValorImpresoTransferencia);
                string conceptoActual = ObtenerTextoReal(Txt_Concepto).ToUpper();

                // ===== REGISTRAR EN BASE DE DATOS =====
                if (RegistrarTransferCompleto())
                {
                    MessageBox.Show("ÉXITO AL FINALIZAR LA PARTIDA CONTABLE", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Enviar correo con los datos capturados previamente
                    EnviarCorreoNotificacion(numeroTRANSFERENCIAActual, beneficiarioActual, valorImpresoActual, conceptoActual);

                    // Limpiar formulario
                    LimpiarFormularioCompleto();

                    // Actualizar control de TRANSFERENCIAs
                    CargarControlTransferencias();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL FINALIZAR TRANSFERENCIA: {ex.Message}", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Validar datos completos
        // Validar datos completos
        private bool ValidarDatosCompletos()
        {
            // VALIDAR QUE NO SEA EL PLACEHOLDER
            if (string.IsNullOrEmpty(ObtenerTextoReal(Txt_Beneficiario)) ||
                Txt_Beneficiario.Text == "NOMBRE DEL BENEFICIARIO")
            {
                MessageBox.Show("DEBE INGRESAR EL BENEFICIARIO", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Beneficiario.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(ObtenerTextoReal(Txt_Concepto)))
            {
                MessageBox.Show("DEBE INGRESAR EL CONCEPTO", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (ObtenerValorDecimal(Txt_MontoTotal) == 0)
            {
                MessageBox.Show("EL MONTO TOTAL NO PUEDE SER CERO", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (Tabla.Rows.Count == 0)
            {
                MessageBox.Show("DEBE AGREGAR AL MENOS UNA CUENTA A LA PARTIDA", "VALIDACIÓN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // ==========================
            // VALIDAR COMPLEMENTO
            // ==========================
            // VALIDAR COMPLEMENTO
            if (CheckBox_Complemento.Checked)
            {
                if (string.IsNullOrWhiteSpace(Txt_Complemento.Text))
                {
                    MessageBox.Show(
                        "DEBE INGRESAR EL NÚMERO DE LA TRANSFERENCIA COMPLEMENTO.",
                        "VALIDACIÓN DE COMPLEMENTO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    Txt_Complemento.Focus();
                    return false;
                }

                // Verificar si la transferencia complemento existe
                string numeroComplemento = Txt_Complemento.Text.Trim();
                Mdl_Transfers transferComplemento = Ctrl_Transfers.ObtenerTransferPorNumero(numeroComplemento);

                if (transferComplemento == null)
                {
                    MessageBox.Show(
                        $"LA TRANSFERENCIA NO. {numeroComplemento} NO EXISTE EN EL SISTEMA.",
                        "VALIDACIÓN DE COMPLEMENTO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    Txt_Complemento.Focus();
                    return false;
                }

                // Verificar si la transferencia ya está marcada como completada
                if (transferComplemento.LastComplement)
                {
                    MessageBox.Show(
                        $"LA TRANSFERENCIA NO. {numeroComplemento} YA ESTÁ MARCADA COMO COMPLETADA.\n" +
                        "NO SE PUEDEN AGREGAR MÁS COMPLEMENTOS A ESTA TRANSFERENCIA.",
                        "VALIDACIÓN DE COMPLEMENTO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    Txt_Complemento.Focus();
                    return false;
                }

                // Si además está marcado LastComplement, validar que la suma de complementos no supere el monto total
                if (CheckBox_LastComplement.Checked)
                {
                    decimal montoTotal = ObtenerValorDecimal(Txt_MontoTotal);
                    decimal sumaComplementos = 0m;

                    // Transferencia principal
                    Mdl_Transfers transferPrincipal = Ctrl_Transfers.ObtenerTransferPorNumero(numeroComplemento);
                    if (transferPrincipal != null)
                        sumaComplementos += transferPrincipal.PrintedAmount;

                    // Transferencias complementarias ya registradas
                    List<Mdl_Transfers> transfersComplemento = Ctrl_Transfers.ObtenerTransfersComplemento(numeroComplemento);
                    if (transfersComplemento != null && transfersComplemento.Count > 0)
                    {
                        foreach (var t in transfersComplemento)
                            sumaComplementos += t.PrintedAmount;
                    }

                    if (sumaComplementos > montoTotal)
                    {
                        MessageBox.Show(
                            $"ERROR: EL MONTO TOTAL (Q.{montoTotal:N2}) NO CUBRE LA SUMA DE LOS COMPLEMENTOS (Q.{sumaComplementos:N2}).\n" +
                            "NO SE PUEDE MARCAR ESTA TRANSFERENCIA COMO 'ÚLTIMO COMPLEMENTO'.",
                            "FONDOS INSUFICIENTES",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return false;
                    }
                }
            }


            // VALIDACION: Si el CheckBox_LastComplement está activo, debe estar el complemento
            if (CheckBox_LastComplement.Checked && !CheckBox_Complemento.Checked)
            {
                MessageBox.Show(
                    "NO PUEDE MARCAR 'ÚLTIMO COMPLEMENTO' SIN UNA TRANSFERENCIA COMPLEMENTO ACTIVA.",
                    "VALIDACIÓN",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                CheckBox_LastComplement.Checked = false;
                return false;
            }

            // ==========================
            // VALIDACIÓN ESPECIAL PARA LASTCOMPLEMENT
            // ==========================
            if (CheckBox_LastComplement.Checked &&
                CheckBox_Complemento.Checked &&
                !string.IsNullOrWhiteSpace(Txt_Complemento.Text))
            {
                // 1) Verificar que el valor impreso no sea negativo
                decimal valorImpreso = ObtenerValorDecimal(Txt_ValorImpresoTransferencia);

                if (valorImpreso < 0)
                {
                    MessageBox.Show(
                        $"ERROR: EL VALOR IMPRESO DE LA TRANSFERENCIA ES NEGATIVO (Q.{valorImpreso:N2}).\n\n" +
                        "ESTO SIGNIFICA QUE LOS COMPLEMENTOS SUMAN MÁS QUE EL MONTO TOTAL.\n" +
                        "NO SE PUEDE CONTINUAR CON ESTA OPERACIÓN.\n\n" +
                        "VERIFIQUE LOS MONTOS DE LAS TRANSFERENCIAS VINCULADAS.",
                        "FONDOS INSUFICIENTES",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }

                // 2) Validación adicional: Monto total vs suma de complementos
                decimal montoTotal = ObtenerValorDecimal(Txt_MontoTotal);
                decimal sumaComplementos = 0m;

                string numeroComplemento = Txt_Complemento.Text.Trim();

                // Obtener la transferencia principal
                Mdl_Transfers transferenciaPrincipal = Ctrl_Transfers.ObtenerTransferPorNumero(numeroComplemento);
                if (transferenciaPrincipal != null)
                {
                    sumaComplementos += transferenciaPrincipal.PrintedAmount;
                }

                // Obtener todas las transferencias vinculadas como complemento
                List<Mdl_Transfers> transferenciasComplemento =
                    Ctrl_Transfers.ObtenerTransfersComplemento(numeroComplemento);

                if (transferenciasComplemento != null && transferenciasComplemento.Count > 0)
                {
                    foreach (var transferenciaComp in transferenciasComplemento)
                    {
                        sumaComplementos += transferenciaComp.PrintedAmount;
                    }
                }

                if (montoTotal < sumaComplementos)
                {
                    MessageBox.Show(
                        $"ERROR: EL MONTO TOTAL (Q.{montoTotal:N2}) NO CUBRE LA SUMA DE LOS COMPLEMENTOS (Q.{sumaComplementos:N2}).\n" +
                        "NO SE PUEDE MARCAR ESTA TRANSFERENCIA COMO 'ÚLTIMO COMPLEMENTO'.\n\n" +
                        "VERIFIQUE LOS MONTOS DE LAS TRANSFERENCIAS VINCULADAS.",
                        "FONDOS INSUFICIENTES",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }


        // Validar partida contable cuadrada
        private bool ValidarPartidaContable()
        {
            decimal totalCargos = ObtenerValorDecimal(Txt_TotalCargos);
            decimal totalAbonos = ObtenerValorDecimal(Txt_TotalAbonos);
            decimal montoTotal = ObtenerValorDecimal(Txt_MontoTotal);

            // CORRECCION: Si es LastComplement, la partida NO debe validarse
            // porque el valor impreso es diferente al monto total
            if (CheckBox_LastComplement.Checked)
            {
                // No validar partida contable cuando es último complemento
                // La partida quedará pendiente de ajuste manual
                return true;
            }

            // Validación normal: Cargos y Abonos deben ser iguales al Monto Total
            if (totalCargos != montoTotal || totalAbonos != montoTotal)
            {
                MessageBox.Show(
                    $"LA PARTIDA CONTABLE NO ESTÁ CUADRADA:\n\n" +
                    $"Monto Total: Q.{montoTotal:N2}\n" +
                    $"Total Cargos: Q.{totalCargos:N2}\n" +
                    $"Total Abonos: Q.{totalAbonos:N2}",
                    "ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool RegistrarTransferCompleto()
        {
            try
            {
                // ===== 1. REGISTRAR TRANSFERENCIA =====

                // 1.1 Construir modelo de transferencia desde los controles del formulario
                Mdl_Transfers transfer = new Mdl_Transfers
                {
                    // Número de transferencia: se ingresa manualmente
                    // Cambia Txt_NoTRANSFERENCIA por tu textbox de número de transferencia si se llama distinto
                    TransferNumber = ObtenerTextoReal(Txt_NoTransferencia).ToUpper(),

                    // Fecha / lugar
                    IssueDate = DTP_Emision.Value,
                    IssuePlace = "GUATEMALA",

                    // Monto total e impreso
                    Amount = ObtenerValorDecimal(Txt_MontoTotal),
                    PrintedAmount = ObtenerValorDecimal(Txt_ValorImpresoTransferencia),

                    // Beneficiario
                    BeneficiaryName = ObtenerTextoReal(Txt_Beneficiario).ToUpper(),

                    // Banco y tipo de cuenta
                    BankId = Convert.ToInt32(ComboBox_Banco.SelectedValue),  // ajusta si tu combo se llama distinto

                    // IMPORTANTE: la propiedad correcta según modelo es BanksAccountTypeId
                    BanksAccountTypeId = Convert.ToInt32(ComboBox_TipoCuenta.SelectedValue),

                    // Estado: si mandamos 0, Ctrl_Transfers lo setea a COMPLETADA automáticamente
                    StatusId = 0,

                    // Conceptos
                    Concept = ObtenerTextoReal(Txt_Concepto).ToUpper(),
                    DetailDescription = ObtenerTextoReal(Txt_Observaciones).ToUpper(),

                    // Periodo y sede
                    Period = Txt_Periodo.Text.ToUpper(),
                    LocationId = Convert.ToInt32(ComboBox_Location.SelectedValue),

                    // Campos financieros (igual que TRANSFERENCIAs)
                    Exemption = ObtenerValorDecimal(Txt_Exencion),
                    TaxFreeAmount = ObtenerValorDecimal(Txt_MontoSinITH),
                    FoodAllowance = ObtenerValorDecimal(Txt_Alimentacion),
                    IGSS = ObtenerValorDecimal(Txt_IGSS),
                    WithholdingTax = ObtenerValorDecimal(Txt_RetencionISR), // ISR se guarda aquí
                    Retention = ObtenerValorDecimal(Txt_RetencionISR), // Si Retention también se usa, ajusta según tu diseño
                    Bonus = ObtenerValorDecimal(Txt_Bonificacion),
                    Discounts = ObtenerValorDecimal(Txt_Descuentos),
                    Advances = ObtenerValorDecimal(Txt_Anticipos),
                    Viaticos = ObtenerValorDecimal(Txt_Viaticos),
                    Stamps = ObtenerValorDecimal(Txt_Stamps),
                    Compensation = ObtenerValorDecimal(Txt_Indemnizacion),
                    Vacation = ObtenerValorDecimal(Txt_Vacaciones),
                    Aguinaldo = ObtenerValorDecimal(Txt_Aguinaldo),

                    // Complemento / Último complemento (igual que TRANSFERENCIAs, pero para transfers)
                    Complement = CheckBox_Complemento.Checked ? ObtenerTextoReal(Txt_Complemento).ToUpper() : null,
                    LastComplement = false,   // siempre inicia en falso

                    // Bill y banderas
                    Bill = "NO ASIGNADA",
                    IsActive = true,
                    CreatedBy = UserData.UserId
                };

                // 1.2 Registrar la transferencia en base de datos
                int resultado = Ctrl_Transfers.RegistrarTransfer(transfer);
                if (resultado == 0)
                {
                    MessageBox.Show("ERROR AL REGISTRAR LA TRANSFERENCIA", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // 1.3 Volver a leer la transferencia para obtener su TransferId real
                Mdl_Transfers transferenciaRegistrada = Ctrl_Transfers.ObtenerTransferPorNumero(transfer.TransferNumber);
                if (transferenciaRegistrada == null || transferenciaRegistrada.TransferId == 0)
                {
                    MessageBox.Show("NO SE PUDO RECUPERAR LA TRANSFERENCIA REGISTRADA.",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // 1.4 Marcar LastComplement si procede (misma lógica de TRANSFERENCIAs)
                if (CheckBox_LastComplement.Checked &&
                    CheckBox_Complemento.Checked &&
                    !string.IsNullOrWhiteSpace(Txt_Complemento.Text))
                {
                    string numeroComplemento = Txt_Complemento.Text.Trim();
                    bool marcado = Ctrl_Transfers.MarcarLastComplement(numeroComplemento);

                    if (!marcado)
                    {
                        MessageBox.Show(
                            $"ADVERTENCIA: No se pudo marcar la transferencia {numeroComplemento} como completada.",
                            "ADVERTENCIA",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }

                // ===== 2. REGISTRAR PARTIDA CONTABLE (MASTER GENÉRICO) =====

                Mdl_AccountingEntryMaster partida = new Mdl_AccountingEntryMaster
                {
                    EntryDate = DTP_Emision.Value,
                    Concept = ObtenerTextoReal(Txt_Concepto).ToUpper(),
                    StatusId = 1, // Ajusta al Id real del estado "APROBADA" / "REGISTRADA" que uses
                    TotalAmount = ObtenerValorDecimal(Txt_MontoTotal),
                    CreatedBy = UserData.UserId,
                    IsActive = true
                };

                int partidaId = Ctrl_AccountingEntryMaster.RegistrarPartida(partida);
                if (partidaId == 0)
                {
                    MessageBox.Show("ERROR AL REGISTRAR LA PARTIDA CONTABLE DE LA TRANSFERENCIA", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // ===== 2.1 VINCULAR PARTIDA CON TRANSFERENCIA =====

                bool vinculoOk = Ctrl_AccountingEntryTransfers.RegistrarVinculo(partidaId, transferenciaRegistrada.TransferId);
                if (!vinculoOk)
                {
                    MessageBox.Show("ERROR AL VINCULAR LA PARTIDA CON LA TRANSFERENCIA", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // ===== 3. REGISTRAR DETALLES CONTABLES =====

                foreach (DataGridViewRow row in Tabla.Rows)
                {
                    if (!row.IsNewRow && row.Cells["Codigo"].Value != null)
                    {
                        string codigoCuenta = row.Cells["Codigo"].Value.ToString().ToUpper();
                        string nombreCuenta = row.Cells["Cuenta"].Value.ToString().ToUpper();
                        decimal cargo = Convert.ToDecimal(row.Cells["Cargo"].Value);
                        decimal abono = Convert.ToDecimal(row.Cells["Abono"].Value);

                        // Obtener AccountId desde Accounts
                        int accountId = Convert.ToInt32(Ctrl_Accounts.BuscarCampo(0, nombreCuenta));

                        Mdl_AccountingEntryDetails detalle = new Mdl_AccountingEntryDetails
                        {
                            EntryMasterId = partidaId,
                            AccountId = accountId,
                            Debit = cargo,
                            Credit = abono
                        };

                        if (Ctrl_AccountingEntryDetails.RegistrarDetalle(detalle) == 0)
                        {
                            MessageBox.Show($"ERROR AL REGISTRAR DETALLE CONTABLE: {nombreCuenta}", "ERROR",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        // Actualizar saldo de la cuenta
                        Ctrl_Accounts.ActualizarSaldo(nombreCuenta, cargo, abono);
                    }
                }

                // ===== 4. NO HAY CONTROL DE TRANSFERENCIAS AQUÍ =====
                // En transferencias NO se usan rangos ni CheckControl, así que aquí NO hacemos nada de eso.

                // ===== 5. AUDITORÍA =====

                string detalleAuditoria = $"REGISTRO DE TRANSFERENCIA NO. {transferenciaRegistrada.TransferNumber} " +
                                          $"POR {UserData.Username.ToUpper()}, " +
                                          $"BENEFICIARIO: {transferenciaRegistrada.BeneficiaryName.ToUpper()}, " +
                                          $"MONTO: Q.{transferenciaRegistrada.Amount:N2}";

                Ctrl_Audit.RegistrarAccion(
                    UserData.UserId,
                    $"REGISTRO DE TRANSFERENCIA NO. {transferenciaRegistrada.TransferNumber}",
                    "Transfers",
                    transferenciaRegistrada.TransferId,
                    detalleAuditoria
                );

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR EN REGISTRO DE TRANSFERENCIA: {ex.Message}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        // Registrar auditoría
        private void RegistrarAuditoria()
        {
            try
            {
                string tipoTRANSFERENCIA = ComboBox_TipoTransferencia.SelectedItem?.ToString() ?? "";
                string detalle = $"REGISTRO DE TRANSFERENCIA {tipoTRANSFERENCIA} POR {UserData.Username.ToUpper()}, " +
                                $"EN BENEFICIO DE {ObtenerTextoReal(Txt_Beneficiario).ToUpper()}, " +
                                $"CON UN VALOR DE Q.{ObtenerValorDecimal(Txt_ValorImpresoTransferencia):N2}";

                Ctrl_Audit.RegistrarAccion(
                    UserData.UserId,
                    $"EMISIÓN DE TRANSFERENCIA NO. {Txt_NoTransferencia.Text}",
                    "Checks",
                    Convert.ToInt32(Txt_NoTransferencia.Text),
                    detalle
                );
            }
            catch { }
        }

        // LEGACY Enviar correo de notificación
        //private void EnviarCorreoNotificacion()
        //{
        //    try
        //    {
        //        string correoEmisor = "notificaciones@uregionalregion2.edu.gt";
        //        string contraseñaEmisor = "F0rza01.";

        //        SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
        //        {
        //            Port = 587,
        //            Credentials = new NetworkCredential(correoEmisor, contraseñaEmisor),
        //            EnableSsl = true
        //        };

        //        MailMessage mail = new MailMessage
        //        {
        //            From = new MailAddress(correoEmisor, "Notificaciones URegional"),
        //            Subject = "Información de TRANSFERENCIA Emitido",
        //            IsBodyHtml = true
        //        };

        //        Mdl_Users usuarioActual = Ctrl_Users.ObtenerUsuarioPorId(UserData.UserId);
        //        string nombreUsuario = usuarioActual?.FullName ?? "USUARIO DESCONOCIDO";

        //        mail.Body = $@"
        //<html>
        //<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
        //    <h2 style='color: #2A7AE2;'>Información de TRANSFERENCIA Emitido</h2>
        //    <p><strong>TRANSFERENCIA a nombre de:</strong> {ObtenerTextoReal(Txt_Beneficiario).ToUpper()}</p>
        //    <p><strong>Fecha y Hora de Emisión:</strong> {DateTime.Now}</p>
        //    <p><strong>Valor del TRANSFERENCIA:</strong> Q.{ObtenerValorDecimal(Txt_ValorImpresoTRANSFERENCIA):N2}</p>
        //    <p><strong>Persona que emitió el TRANSFERENCIA:</strong> {nombreUsuario.ToUpper()}</p>
        //    <p><strong>No.TRANSFERENCIA:</strong> {Txt_NoTRANSFERENCIA.Text}</p>
        //    <p><strong>Concepto del TRANSFERENCIA:</strong> {ObtenerTextoReal(Txt_Concepto).ToUpper()}</p>
        //    <p>Por favor, verifique la información y proceda con la gestión correspondiente.</p>
        //    <p style='color: #555;'>Gracias,</p>
        //    <p><strong>Servicio automático de notificaciones, SECRON</strong></p>
        //</body>
        //</html>";

        //        mail.To.Add("TRANSFERENCIAs@uregionalregion2.edu.gt");
        //        mail.CC.Add("afolgar@uregionalregion2.edu.gt");
        //        mail.CC.Add("avfolgar@uregionalregion2.edu.gt");
        //        mail.CC.Add("shvanegas@uregionalregion2.edu.gt");
        //        mail.To.Add("aportillo@uregionalregion2.edu.gt");

        //        smtpClient.Send(mail);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"ERROR AL ENVIAR CORREO: {ex.Message}", "ADVERTENCIA",
        //            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //}
        private void EnviarCorreoNotificacion(string numeroTRANSFERENCIA, string beneficiario, decimal valorImpreso, string concepto)
        {
            try
            {
                string correoEmisor = "notificaciones@uregionalregion2.edu.gt";
                string contraseñaEmisor = "F0rza01.";
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(correoEmisor, contraseñaEmisor),
                    EnableSsl = true
                };

                // ⭐ VERIFICAR SI ES TRANSFERENCIA IMPORTANTE
                bool esTRANSFERENCIAImportante = CheckBox_AlertaPrioridad.Checked; ;

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(correoEmisor, "Notificaciones URegional"),
                    Subject = esTRANSFERENCIAImportante ? "URGENTE - TRANSFERENCIA Importante Emitido" : "Información de TRANSFERENCIA Emitido",
                    IsBodyHtml = true
                };

                // ⭐ ESTABLECER PRIORIDAD ALTA SI ES IMPORTANTE
                if (esTRANSFERENCIAImportante)
                {
                    mail.Priority = MailPriority.High;
                }

                Mdl_Users usuarioActual = Ctrl_Users.ObtenerUsuarioPorId(UserData.UserId);
                string nombreUsuario = usuarioActual?.FullName ?? "USUARIO DESCONOCIDO";

                // ⭐ GENERAR CUERPO DEL CORREO CON ALERTA SI ES IMPORTANTE
                string alertaImportante = esTRANSFERENCIAImportante ? $@"
                <div style='background-color: #FF4444; color: white; padding: 15px; border-radius: 5px; margin-bottom: 20px; text-align: center;'>
                    <h3 style='margin: 0; color: white;'>TRANSFERENCIA MARCADA COMO IMPORTANTE</h3>
                    <p style='margin: 5px 0 0 0; color: white;'>Transferencia requiere atención prioritaria</p>
                </div>" : "";

                string colorTitulo = esTRANSFERENCIAImportante ? "#FF4444" : "#2A7AE2";

                mail.Body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    {alertaImportante}
                    <h2 style='color: {colorTitulo};'>TRANSFERENCIA EMITIDA</h2>
                    <p><strong>Transferencia a nombre de:</strong> {beneficiario}</p>
                    <p><strong>Fecha y Hora de Emisión:</strong> {DateTime.Now}</p>
                    <p><strong>Valor del Transferencia:</strong> Q.{valorImpreso:N2}</p>
                    <p><strong>Persona que emitió la transferencia:</strong> {nombreUsuario.ToUpper()}</p>
                    <p><strong>No.Transferencia:</strong> {numeroTRANSFERENCIA}</p>
                    <p><strong>Concepto de la Transferencia:</strong> {concepto}</p>
                    <p>Por favor, verifique la información y proceda con la gestión correspondiente.</p>
                    <p style='color: #555;'>Gracias,</p>
                    <p><strong>Servicio automático de notificaciones, SECRON</strong></p>
                </body>
                </html>";
                mail.To.Add("transferencias@uregionalregion2.edu.gt");
                mail.CC.Add("afolgar@uregionalregion2.edu.gt");
                mail.CC.Add("avfolgar@uregionalregion2.edu.gt");
                mail.CC.Add("shvanegas@uregionalregion2.edu.gt");
                mail.To.Add("aportillo@uregionalregion2.edu.gt");
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ENVIAR CORREO: {ex.Message}", "ADVERTENCIA",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Limpiar formulario completo
        private void LimpiarFormularioCompleto()
        {
            // Limpiar tabla de partida
            Tabla.Rows.Clear();
            _partidaContable.Clear();

            Txt_TotalCargos.Text = "0.00";
            Txt_TotalAbonos.Text = "0.00";

            // Limpiar campos de partida y de información general
            LimpiarCamposPartida();
            LimpiarInformacionTRANSFERENCIA(); // aquí igual ya lo tienes adaptado a transferencias

            // Número de transferencia en blanco para la siguiente operación
            Txt_NoTransferencia.Text = string.Empty;

            // Si quieres, puedes resetear también los combos:
            ComboBox_TipoTransferencia.SelectedIndex = 0;
            ComboBox_Location.SelectedIndex = 0;
            ComboBox_Banco.SelectedIndex = 0;
            ComboBox_TipoCuenta.SelectedIndex = 0;
        }

        // Cargar control de TRANSFERENCIAs del usuario
        public void CargarControlTransferencias()
        {
            Txt_NoTransferencia.Text = string.Empty;
        }
        // ACTUALIZAR DATOS DEL RECIBO PARA IMPRESIÓN
        private void ActualizarDatosRecibo()
        {
            // LIMPIAR COMPLETAMENTE EL OBJETO (ESTO SOLUCIONA EL PROBLEMA DE DATOS ANTERIORES)
            datosRecibo = new DatosRecibo();

            string tipoChecked = ComboBox_TipoTransferencia.SelectedItem?.ToString() ?? "";

            datosRecibo.Fecha = DTP_Emision.Value.ToString("d 'de' MMMM 'de' yyyy").ToUpper();
            datosRecibo.Entidad = "**" + ObtenerTextoReal(Txt_Beneficiario).ToUpper() + "**";
            datosRecibo.Concepto = ObtenerTextoReal(Txt_Concepto);
            datosRecibo.Observaciones = ObtenerTextoReal(Txt_Observaciones);
            datosRecibo.Sede = "SEDE " + ComboBox_Location.Text;

            // CORRECCION: Calcular el valor impreso REAL considerando complementos
            decimal valorImpresoFinal = ObtenerValorDecimal(Txt_ValorImpresoTransferencia);

            // CORRECCION: Si es LastComplement, recalcular el valor en letras
            // porque el TextBox puede no estar actualizado correctamente
            if (CheckBox_LastComplement.Checked && CheckBox_Complemento.Checked && !string.IsNullOrWhiteSpace(Txt_Complemento.Text))
            {
                // El valor ya fue calculado correctamente en CalcularValores
                // pero por seguridad, lo convertimos nuevamente a letras
                datosRecibo.MontoLetras = "**" + NumeroALetras(valorImpresoFinal) + "**";
            }
            else
            {
                // Usar el valor del TextBox directamente
                datosRecibo.MontoLetras = "**" + ObtenerTextoReal(Txt_ValorLetras).ToUpper() + "**";
            }

            // Asignar el valor impreso final
            datosRecibo.ValorImpreso = valorImpresoFinal.ToString("N2");

            // Obtener valores
            decimal montoTotal = ObtenerValorDecimal(Txt_MontoTotal);
            decimal exencion = ObtenerValorDecimal(Txt_Exencion);
            decimal ith = ObtenerValorDecimal(Txt_ITH);
            decimal alimentacion = ObtenerValorDecimal(Txt_Alimentacion);
            decimal igss = ObtenerValorDecimal(Txt_IGSS);
            decimal bonificacion = ObtenerValorDecimal(Txt_Bonificacion);
            decimal descuentos = ObtenerValorDecimal(Txt_Descuentos);
            decimal anticipos = ObtenerValorDecimal(Txt_Anticipos);
            decimal viaticos = ObtenerValorDecimal(Txt_Viaticos);
            decimal stamps = ObtenerValorDecimal(Txt_Stamps);
            decimal retencionISR = ObtenerValorDecimal(Txt_RetencionISR);
            decimal aguinaldo = ObtenerValorDecimal(Txt_Aguinaldo);
            decimal indemnizacion = ObtenerValorDecimal(Txt_Indemnizacion);
            decimal vacaciones = ObtenerValorDecimal(Txt_Vacaciones);

            // CALCULAR MONTO BASE REAL (lo que realmente se pago por el servicio/producto)
            decimal montoBaseReal = montoTotal - alimentacion - bonificacion - viaticos - stamps - indemnizacion - vacaciones - aguinaldo;


            switch (tipoChecked)
            {
                case "HOSPEDAJES":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            //Si es LastComplement, mostrar "MONTO TOTAL" en lugar de "HOSPEDAJE"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" : "HOSPEDAJE",
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (ith > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "IMPUESTO DE TURISMO (ITH)",
                            Signo = "(-)",
                            Monto = ith.ToString("N2")
                        });

                    if (exencion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "IVA EXENCIÓN",
                            Signo = "(-)",
                            Monto = exencion.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });
                    break;

                case "PUBLICIDAD":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            // Si es LastComplement, mostrar "MONTO TOTAL"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" : "SERVICIO PUBLICITARIO",
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (exencion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "EXENCIÓN DE IVA",
                            Signo = "(-)",
                            Monto = exencion.ToString("N2")
                        });

                    if (retencionISR > 0)   // ⭐ NUEVO: mostrar ISR retenido
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ISR RETENIDO",
                            Signo = "(-)",
                            Monto = retencionISR.ToString("N2")
                        });

                    if (stamps > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "TIMBRES DE PRENSA",
                            Signo = "(+)",
                            Monto = stamps.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });
                    break;


                case "SERVICIOS":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            // Si es LastComplement, mostrar "MONTO TOTAL"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" : "SERVICIO",
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (exencion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "IVA EXENCIÓN",
                            Signo = "(-)",
                            Monto = exencion.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });
                    break;
                case "ANTICIPOS":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            // Si es LastComplement, mostrar "MONTO TOTAL"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" : "ANTICIPOS",
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (exencion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "IVA EXENCIÓN",
                            Signo = "(-)",
                            Monto = exencion.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });
                    break;

                case "HONORARIOS":
                case "HONORARIOS PROFESIONALES":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            // CORRECCION: Si es LastComplement, mostrar "MONTO TOTAL"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" :
                         (tipoChecked == "HONORARIOS" ? "HONORARIOS" : "HONORARIOS PROFESIONALES"),
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (exencion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "IVA EXENCIÓN",
                            Signo = "(-)",
                            Monto = exencion.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });
                    break;

                case "COMPRAS":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            // Si es LastComplement, mostrar "MONTO TOTAL"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" : "COMPRA",
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (exencion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "IVA EXENCIÓN",
                            Signo = "(-)",
                            Monto = exencion.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });
                    break;

                case "SUELDOS":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            // Si es LastComplement, mostrar "MONTO TOTAL"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" : "SUELDO BASE",
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN INCENTIVO",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS DE SUELDOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (igss > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "DESCUENTO DE IGSS",
                            Signo = "(-)",
                            Monto = igss.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "OTROS DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });

                    if (retencionISR > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ISR RETENIDO",
                            Signo = "(-)",
                            Monto = retencionISR.ToString("N2")
                        });
                    break;

                case "LIQUIDACIONES":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            // Si es LastComplement, mostrar "MONTO TOTAL"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" : "LIQUIDACIÓN",
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN ANUAL (DECRETO NO. 42-92",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (indemnizacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "INDEMNIZACIÓN",
                            Signo = "(+)",
                            Monto = indemnizacion.ToString("N2")
                        });

                    if (vacaciones > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VACACIONES",
                            Signo = "(+)",
                            Monto = vacaciones.ToString("N2")
                        });

                    if (aguinaldo > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "AGUINALDO (DECRETO NO. 76-78)",
                            Signo = "(+)",
                            Monto = aguinaldo.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS DE SUELDOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (igss > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "DESCUENTO DE IGSS",
                            Signo = "(-)",
                            Monto = igss.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "OTROS DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });

                    if (retencionISR > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ISR RETENIDO",
                            Signo = "(-)",
                            Monto = retencionISR.ToString("N2")
                        });
                    break;

                case "BONO NAVIDEÑO":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            // Si es LastComplement, mostrar "MONTO TOTAL"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" : "LIQUIDACIÓN",
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN ANUAL (DECRETO NO. 42-92",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (indemnizacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "INDEMNIZACIÓN",
                            Signo = "(+)",
                            Monto = indemnizacion.ToString("N2")
                        });

                    if (vacaciones > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VACACIONES",
                            Signo = "(+)",
                            Monto = vacaciones.ToString("N2")
                        });

                    if (aguinaldo > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONO NAVIDEÑO",
                            Signo = "(+)",
                            Monto = aguinaldo.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS DE SUELDOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (igss > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "DESCUENTO DE IGSS",
                            Signo = "(-)",
                            Monto = igss.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "OTROS DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });

                    if (retencionISR > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ISR RETENIDO",
                            Signo = "(-)",
                            Monto = retencionISR.ToString("N2")
                        });
                    break;

                case "ARRENDAMIENTOS":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            // Si es LastComplement, mostrar "MONTO TOTAL"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" : "ARRENDAMIENTO",
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (exencion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "IVA EXENCIÓN",
                            Signo = "(-)",
                            Monto = exencion.ToString("N2")
                        });

                    if (retencionISR > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ISR RETENIDO",
                            Signo = "(-)",
                            Monto = retencionISR.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });
                    break;

                case "ANTICIPOS DE SUELDOS":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            // Si es LastComplement, mostrar "MONTO TOTAL"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" : "ANTICIPO DE SUELDO",
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN INCENTIVO",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS DE SUELDOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "OTROS DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });

                    if (retencionISR > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ISR RETENIDO",
                            Signo = "(-)",
                            Monto = retencionISR.ToString("N2")
                        });
                    break;

                case "ANTICIPOS DE HONORARIOS":
                    if (montoBaseReal > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            // Si es LastComplement, mostrar "MONTO TOTAL"
                            Descripcion = CheckBox_LastComplement.Checked ? "MONTO TOTAL" : "ANTICIPO DE HONORARIO",
                            Signo = "",
                            Monto = montoBaseReal.ToString("N2")
                        });

                    if (bonificacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "BONIFICACIÓN INCENTIVO",
                            Signo = "(+)",
                            Monto = bonificacion.ToString("N2")
                        });

                    if (alimentacion > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ALIMENTACIÓN",
                            Signo = "(+)",
                            Monto = alimentacion.ToString("N2")
                        });

                    if (viaticos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "VIÁTICOS",
                            Signo = "(+)",
                            Monto = viaticos.ToString("N2")
                        });

                    if (anticipos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ANTICIPOS DE HONORARIOS",
                            Signo = "(-)",
                            Monto = anticipos.ToString("N2")
                        });

                    if (descuentos > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "OTROS DESCUENTOS",
                            Signo = "(-)",
                            Monto = descuentos.ToString("N2")
                        });

                    if (retencionISR > 0)
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = "ISR RETENIDO",
                            Signo = "(-)",
                            Monto = retencionISR.ToString("N2")
                        });
                    break;
            }

            // ===== FUNCIONALIDAD LASTCOMPLEMENT - APLICA PARA CUALQUIER TIPO DE TRANSFERENCIA =====
            // SI LASTCOMPLEMENT ESTA ACTIVADO, AGREGAR TRANSFERENCIAS COMPLEMENTARIAS AL FINAL
            if (CheckBox_LastComplement.Checked && CheckBox_Complemento.Checked && !string.IsNullOrWhiteSpace(Txt_Complemento.Text))
            {
                string numeroComplemento = Txt_Complemento.Text.Trim();

                // Obtener la transferencia principal primero
                Mdl_Transfers transferPrincipal = Ctrl_Transfers.ObtenerTransferPorNumero(numeroComplemento);

                // Obtener todas las transferencias vinculadas a esta principal
                List<Mdl_Transfers> transfersComplemento = Ctrl_Transfers.ObtenerTransfersComplemento(numeroComplemento);

                // Verificar si hay datos que mostrar
                bool hayDatosParaMostrar = (transferPrincipal != null) || (transfersComplemento != null && transfersComplemento.Count > 0);

                if (hayDatosParaMostrar)
                {
                    // Agregar separador
                    datosRecibo.Items.Add(new ItemImpresion
                    {
                        Descripcion = "--- COMPLEMENTOS VINCULADOS ---",
                        Signo = "",
                        Monto = ""
                    });

                    // Mostrar la transferencia principal
                    if (transferPrincipal != null)
                    {
                        datosRecibo.Items.Add(new ItemImpresion
                        {
                            Descripcion = $"ANTICIPO EN TRANSFERENCIA NO. {transferPrincipal.TransferNumber}",
                            Signo = "(-)",
                            Monto = transferPrincipal.PrintedAmount.ToString("N2")
                        });
                    }

                    // Luego mostrar todas las transferencias vinculadas
                    if (transfersComplemento != null && transfersComplemento.Count > 0)
                    {
                        foreach (var transferComp in transfersComplemento)
                        {
                            datosRecibo.Items.Add(new ItemImpresion
                            {
                                Descripcion = $"ANTICIPO EN TRANSFERENCIA NO. {transferComp.TransferNumber}",
                                Signo = "(-)",
                                Monto = transferComp.PrintedAmount.ToString("N2")
                            });
                        }
                    }
                }
            }
        }

        #endregion FinalizarTRANSFERENCIA
        #region AsignarTRANSFERENCIA
        private void Btn_AsignarTRANSFERENCIA_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    this.Cursor = Cursors.WaitCursor;

            //    // Abrir formulario de asignación de TRANSFERENCIAs como MODAL
            //    Frm_Checks_Assignment frmAsignar = new Frm_Checks_Assignment(this);
            //    DialogResult resultado = frmAsignar.ShowDialog();

            //    if (resultado == DialogResult.OK)
            //    {
            //        _advertenciaLimiteMostrada = false;
            //        // Recargar control de TRANSFERENCIAs después de asignar
            //        CargarControlTransferencias();

            //        MessageBox.Show(
            //            "RANGO DE TRANSFERENCIAS ACTUALIZADO CORRECTAMENTE",
            //            "ÉXITO",
            //            MessageBoxButtons.OK,
            //            MessageBoxIcon.Information);
            //    }

            //    this.Cursor = Cursors.Default;
            //}
            //catch (Exception ex)
            //{
            //    this.Cursor = Cursors.Default;
            //    MessageBox.Show($"ERROR AL ABRIR ASIGNACIÓN: {ex.Message}",
            //                   "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
        #endregion AsignarTRANSFERENCIA
        #region RefrescoAutomatico

        // Variable para el Timer
        private System.Windows.Forms.Timer timerRefreshChecks;

        // Configurar Timer de refresco automático
        private void ConfigurarTimerRefresco()
        {
            timerRefreshChecks = new System.Windows.Forms.Timer();
            timerRefreshChecks.Interval = 5000; // 5 segundos (5000 milisegundos)
            timerRefreshChecks.Tick += TimerRefreshChecks_Tick;
            timerRefreshChecks.Start(); // Iniciar automáticamente
        }

        // Evento del Timer
        private void TimerRefreshChecks_Tick(object sender, EventArgs e)
        {
            try
            {
                // Solo refrescar si el formulario está activo y NO se está imprimiendo
                if (this.Visible && !printPreviewDialog.Visible)
                {
                    CargarControlTransferencias();
                }
            }
            catch (Exception ex)
            {
                // Silencioso - no mostrar error si falla el refresco automático
                System.Diagnostics.Debug.WriteLine($"Error en refresco automático: {ex.Message}");
            }
        }

        // Detener Timer (útil si necesitas pausarlo)
        private void DetenerTimerRefresco()
        {
            if (timerRefreshChecks != null && timerRefreshChecks.Enabled)
            {
                timerRefreshChecks.Stop();
            }
        }

        // Reanudar Timer
        private void ReanudarTimerRefresco()
        {
            if (timerRefreshChecks != null && !timerRefreshChecks.Enabled)
            {
                timerRefreshChecks.Start();
            }
        }

        #endregion RefrescoAutomatico
        #region SistemaDePermisos
        // ========== SISTEMA DE PERMISOS ==========
        private Ctrl_Security_Auth authController;
        private List<string> permisosUsuario = new List<string>();

        // Método para cargar permisos del usuario
        private async Task CargarPermisosUsuario(int userId, int roleId)
        {
            try
            {
                permisosUsuario = await authController.ObtenerPermisosUsuarioAsync(userId, roleId);
                System.Diagnostics.Debug.WriteLine($"Permisos cargados en Checks Managment: {permisosUsuario.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR PERMISOS: {ex.Message}",
                               "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para verificar si tiene un permiso específico
        private bool TienePermiso(string permissionCode)
        {
            if (permisosUsuario == null || permisosUsuario.Count == 0)
                return false;

            return permisosUsuario.Contains(permissionCode);
        }

        // Método para configurar botones según permisos
        private void ConfigurarBotonesPorPermisos()
        {
            // ⭐ CHECKS_MANAGMENT_ASSIGNMENT - Asignar TRANSFERENCIAs (CHK_032)
            //Btn_AsignarTRANSFERENCIA.Enabled = TienePermiso("CHECKS_MANAGMENT_ASSIGNMENT");
        }
        #endregion SistemaDePermisos
        #region ExencionManualComplemento
        // Método para validar y configurar exención manual
        private void ValidarExencionManual()
        {
            // ⭐ CONDICIÓN: Complemento ACTIVO y Exención ACTIVA
            if (CheckBox_Complemento.Checked && CheckBox_Exencion.Checked)
            {
                // ✅ HABILITAR EXENCIÓN MANUAL
                _esExencionManual = true;

                Txt_Exencion.Enabled = true;
                Txt_Exencion.ReadOnly = false;
                Txt_Exencion.BackColor = Color.White;
                Txt_Exencion.ForeColor = Color.Black;
                Txt_Exencion.Cursor = Cursors.IBeam;

                if (Txt_Exencion.Text == "0.00")
                {
                    Txt_Exencion.Text = "";
                }
            }
            else
            {
                // ❌ DESHABILITAR EXENCIÓN MANUAL
                _esExencionManual = false;

                Txt_Exencion.Enabled = false;
                Txt_Exencion.ReadOnly = true;
                Txt_Exencion.BackColor = Color.FromArgb(240, 240, 240);
                Txt_Exencion.ForeColor = Color.FromArgb(100, 100, 100);
                Txt_Exencion.Cursor = Cursors.No;
            }
        }
        // Metodo auxiliar para calcular con exencion manual
        private void CalcularValoresConExencionManual()
        {
            try
            {
                decimal montoTotal = ObtenerValorDecimal(Txt_MontoTotal);
                if (montoTotal == 0)
                {
                    Txt_ValorImpresoTransferencia.Text = "0.00";
                    Txt_ValorLetras.Text = "CERO QUETZALES CON 00/100";
                    return;
                }

                // OBTENER EXENCION MANUAL
                decimal exencionManual = ObtenerValorDecimal(Txt_Exencion);

                // Obtener descuentos y anticipos
                decimal descuentos = ObtenerValorDecimal(Txt_Descuentos);
                decimal anticipos = ObtenerValorDecimal(Txt_Anticipos);

                // CORRECCIÓN: Calcular suma de complementos si LastComplement está activo (TRANSFERENCIAS)
                decimal sumaComplementos = 0;
                if (CheckBox_LastComplement.Checked &&
                    CheckBox_Complemento.Checked &&
                    !string.IsNullOrWhiteSpace(Txt_Complemento.Text))
                {
                    string numeroComplemento = Txt_Complemento.Text.Trim();

                    try
                    {
                        // Obtener la transferencia principal (cuando es anticipo)
                        Mdl_Transfers transferenciaPrincipal = Ctrl_Transfers.ObtenerTransferPorNumero(numeroComplemento);
                        if (transferenciaPrincipal != null)
                        {
                            sumaComplementos += transferenciaPrincipal.PrintedAmount;
                        }

                        // Obtener todas las transferencias vinculadas a ese complemento
                        List<Mdl_Transfers> transfersComplemento =
                            Ctrl_Transfers.ObtenerTransfersComplemento(numeroComplemento);

                        if (transfersComplemento != null && transfersComplemento.Count > 0)
                        {
                            foreach (var transferencia in transfersComplemento)
                            {
                                sumaComplementos += transferencia.PrintedAmount;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error al obtener complementos de transferencia (exención manual): " + ex.Message);
                    }
                }


                // CALCULAR VALOR IMPRESO CON EXENCION MANUAL
                _valorImpreso = montoTotal - exencionManual - descuentos - anticipos;

                // CORRECCION: Si LastComplement activo, restar suma de complementos
                if (CheckBox_LastComplement.Checked)
                {
                    _valorImpreso -= sumaComplementos;
                }

                _valorImpreso = Math.Round(_valorImpreso, 2);

                // Actualizar TextBoxes
                Txt_ValorImpresoTransferencia.Text = _valorImpreso.ToString("N2");
                Txt_ValorLetras.Text = NumeroALetras(_valorImpreso);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en calculo con exencion manual: {ex.Message}");
            }
        }
        // Evento cuando cambia el texto de exención
        private void Txt_Exencion_TextChanged_Manual(object sender, EventArgs e)
        {
            // Solo recalcular si es exención manual
            if (_esExencionManual)
            {
                CalcularValoresConExencionManual();
            }
        }

        // Método para configurar eventos de exención manual
        private void ConfigurarEventosExencionManual()
        {
            // ⭐ NO AGREGAR NADA AQUÍ PARA LOS CHECKBOXES
            // Ya están manejados en sus respectivos métodos

            // Evento para validar solo números en exención manual
            Txt_Exencion.KeyPress += ValidarSoloNumeros_KeyPress;

            // Evento cuando cambia el texto de exención
            Txt_Exencion.TextChanged += Txt_Exencion_TextChanged_Manual;
        }
        #endregion ExencionManualComplemento
        #region SincronizacionCampos
        // Metodo para configurar eventos de sincronizacion de campos
        private void ConfigurarEventosSincronizacion()
        {
            // Evento para sincronizar Observaciones con Concepto
            Txt_Observaciones.TextChanged += Txt_Observaciones_TextChanged;
        }
        // Evento para sincronizar Observaciones con Concepto
        private void Txt_Observaciones_TextChanged(object sender, EventArgs e)
        {
            // Obtener el texto real de Observaciones (sin placeholder)
            string textoObservaciones = ObtenerTextoReal(Txt_Observaciones);

            // Si hay texto real, copiarlo a Concepto
            if (!string.IsNullOrWhiteSpace(textoObservaciones))
            {
                // VALIDACION: Solo copiar si no excede el MaxLength de Txt_Concepto
                if (textoObservaciones.Length <= Txt_Concepto.MaxLength)
                {
                    // Copiar el texto a Concepto
                    Txt_Concepto.Text = textoObservaciones;
                    Txt_Concepto.ForeColor = Color.Black;
                }
                else
                {
                    // Si excede el limite, copiar solo hasta el MaxLength permitido
                    Txt_Concepto.Text = textoObservaciones.Substring(0, Txt_Concepto.MaxLength);
                    Txt_Concepto.ForeColor = Color.Black;
                }
            }
            else
            {
                // Si Observaciones esta vacio, restaurar placeholder en Concepto
                Txt_Concepto.Text = "CONCEPTO DEL TRANSFERENCIA";
                Txt_Concepto.ForeColor = Color.Gray;
            }
        }


        #endregion SincronizacionCampos
    }
}
