using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Checks_FileControl_ReportConfig : Form
    {
        #region PropiedadesIniciales
        private List<Mdl_Checks> _chequesParaReporte;

        public ReportColumnConfig ConfiguracionSeleccionada { get; private set; }

        // ⭐⭐⭐ NUEVA LISTA PARA RASTREAR EL ORDEN DE SELECCIÓN
        private List<string> _ordenSeleccionColumnas = new List<string>();

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse);

        public class ReportColumnConfig
        {
            public bool IncluirMes { get; set; } = true;
            public bool IncluirEmitidos { get; set; } = true;
            public bool IncluirPendientes { get; set; } = true;
            public bool IncluirPendientesPorcentaje { get; set; } = true;
            public bool IncluirTrasladados { get; set; } = true;
            public bool IncluirTrasladadosPorcentaje { get; set; } = true;
            public bool IncluirRecibidos { get; set; } = true;
            public bool IncluirRecibidosPorcentaje { get; set; } = true;
            public bool IncluirArchivados { get; set; } = true;
            public bool IncluirArchivadosPorcentaje { get; set; } = true;

            // ⭐⭐⭐ NUEVA PROPIEDAD PARA ORDEN DE COLUMNAS
            public List<string> OrdenColumnas { get; set; } = new List<string>();
        }

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(1200, 500);
            this.MinimumSize = new Size(1200, 500);
            this.MaximumSize = new Size(1200, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }
        #endregion PropiedadesIniciales
        #region Constructor
        public Frm_Checks_FileControl_ReportConfig(List<Mdl_Checks> cheques)
        {
            InitializeComponent();
            _chequesParaReporte = cheques ?? new List<Mdl_Checks>();
            ConfiguracionSeleccionada = new ReportColumnConfig();
            ConfigurarTamañoFormulario();
            AplicarEstiloBotones();
        }

        private void Frm_Checks_FileControl_ReportConfig_Load(object sender, EventArgs e)
        {
            ConfigurarCheckBoxes();
            ConfigurarTablaVistPrevia();

            // ⭐ Inicializar orden con todas las columnas en orden natural
            InicializarOrdenColumnas();

            ActualizarVistaPrevia();
        }
        #endregion Constructor
        #region EstilosBotones
        public void AplicarEstiloBotones()
        {
            AplicarEstiloBotonAgregar(Btn_RemoveAll);
            AplicarEstiloBotonAgregar(Btn_SelectAll);
        }

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

            boton.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, boton.Width, boton.Height, 20, 20));

            boton.MouseEnter += (s, e) =>
            {
                boton.BackColor = Color.FromArgb(0, 150, 220);
            };

            boton.MouseLeave += (s, e) =>
            {
                boton.BackColor = Color.FromArgb(9, 184, 255);
            };
        }
        #endregion EstilosBotones
        #region Configuración Inicial
        private void ConfigurarCheckBoxes()
        {
            // Marcar todos por defecto
            CheckBox_Col1.Checked = true;
            CheckBox_Col2.Checked = true;
            CheckBox_Col3.Checked = true;
            CheckBox_Col4.Checked = true;
            CheckBox_Col5.Checked = true;
            CheckBox_Col6.Checked = true;
            CheckBox_Col7.Checked = true;
            CheckBox_Col8.Checked = true;
            CheckBox_Col9.Checked = true;
            CheckBox_Col10.Checked = true;

            // ⭐⭐⭐ EVENTOS MODIFICADOS PARA RASTREAR ORDEN DE SELECCIÓN
            CheckBox_Col1.CheckedChanged += (s, e) => ManejarCambioCheckBox("Mes", CheckBox_Col1.Checked);
            CheckBox_Col2.CheckedChanged += (s, e) => ManejarCambioCheckBox("Emitidos", CheckBox_Col2.Checked);
            CheckBox_Col3.CheckedChanged += (s, e) => ManejarCambioCheckBox("Pendientes", CheckBox_Col3.Checked);
            CheckBox_Col4.CheckedChanged += (s, e) => ManejarCambioCheckBox("PendientesPorc", CheckBox_Col4.Checked);
            CheckBox_Col5.CheckedChanged += (s, e) => ManejarCambioCheckBox("Trasladados", CheckBox_Col5.Checked);
            CheckBox_Col6.CheckedChanged += (s, e) => ManejarCambioCheckBox("TrasladadosPorc", CheckBox_Col6.Checked);
            CheckBox_Col7.CheckedChanged += (s, e) => ManejarCambioCheckBox("Recibidos", CheckBox_Col7.Checked);
            CheckBox_Col8.CheckedChanged += (s, e) => ManejarCambioCheckBox("RecibidosPorc", CheckBox_Col8.Checked);
            CheckBox_Col9.CheckedChanged += (s, e) => ManejarCambioCheckBox("Archivados", CheckBox_Col9.Checked);
            CheckBox_Col10.CheckedChanged += (s, e) => ManejarCambioCheckBox("ArchivadosPorc", CheckBox_Col10.Checked);
        }

        private void ConfigurarTablaVistPrevia()
        {
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.RowHeadersVisible = false;

            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            Tabla.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // ⭐⭐⭐ NUEVO MÉTODO: Inicializar orden con todas las columnas
        private void InicializarOrdenColumnas()
        {
            _ordenSeleccionColumnas.Clear();
            _ordenSeleccionColumnas.Add("Mes");
            _ordenSeleccionColumnas.Add("Emitidos");
            _ordenSeleccionColumnas.Add("Pendientes");
            _ordenSeleccionColumnas.Add("PendientesPorc");
            _ordenSeleccionColumnas.Add("Trasladados");
            _ordenSeleccionColumnas.Add("TrasladadosPorc");
            _ordenSeleccionColumnas.Add("Recibidos");
            _ordenSeleccionColumnas.Add("RecibidosPorc");
            _ordenSeleccionColumnas.Add("Archivados");
            _ordenSeleccionColumnas.Add("ArchivadosPorc");
        }

        // ⭐⭐⭐ NUEVO MÉTODO: Manejar cambios en checkboxes y actualizar orden
        private void ManejarCambioCheckBox(string nombreColumna, bool isChecked)
        {
            if (isChecked)
            {
                // Si se marca, agregar al final si no existe
                if (!_ordenSeleccionColumnas.Contains(nombreColumna))
                {
                    _ordenSeleccionColumnas.Add(nombreColumna);
                }
            }
            else
            {
                // Si se desmarca, remover del orden
                _ordenSeleccionColumnas.Remove(nombreColumna);
            }

            ActualizarVistaPrevia();
        }
        #endregion Configuración Inicial
        #region Vista Previa
        private void ActualizarVistaPrevia()
        {
            Tabla.Columns.Clear();
            Tabla.Rows.Clear();

            // ⭐⭐⭐ AGREGAR COLUMNAS EN EL ORDEN DINÁMICO DE SELECCIÓN
            foreach (string nombreCol in _ordenSeleccionColumnas)
            {
                switch (nombreCol)
                {
                    case "Mes":
                        if (CheckBox_Col1.Checked)
                            Tabla.Columns.Add("Mes", "MES");
                        break;
                    case "Emitidos":
                        if (CheckBox_Col2.Checked)
                            Tabla.Columns.Add("Emitidos", "CHEQUES EMITIDOS");
                        break;
                    case "Pendientes":
                        if (CheckBox_Col3.Checked)
                            Tabla.Columns.Add("Pendientes", "PENDIENTES");
                        break;
                    case "PendientesPorc":
                        if (CheckBox_Col4.Checked)
                            Tabla.Columns.Add("PendientesPorc", "PENDIENTES %");
                        break;
                    case "Trasladados":
                        if (CheckBox_Col5.Checked)
                            Tabla.Columns.Add("Trasladados", "TRASLADADOS");
                        break;
                    case "TrasladadosPorc":
                        if (CheckBox_Col6.Checked)
                            Tabla.Columns.Add("TrasladadosPorc", "TRASLADADOS %");
                        break;
                    case "Recibidos":
                        if (CheckBox_Col7.Checked)
                            Tabla.Columns.Add("Recibidos", "RECIBIDOS");
                        break;
                    case "RecibidosPorc":
                        if (CheckBox_Col8.Checked)
                            Tabla.Columns.Add("RecibidosPorc", "RECIBIDOS %");
                        break;
                    case "Archivados":
                        if (CheckBox_Col9.Checked)
                            Tabla.Columns.Add("Archivados", "ARCHIVADOS");
                        break;
                    case "ArchivadosPorc":
                        if (CheckBox_Col10.Checked)
                            Tabla.Columns.Add("ArchivadosPorc", "ARCHIVADOS %");
                        break;
                }
            }

            // VALIDACIÓN: al menos una columna
            if (Tabla.Columns.Count == 0)
            {
                Tabla.Rows.Clear();
                return;
            }

            // Ajustar columnas
            foreach (DataGridViewColumn col in Tabla.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // Cargar datos reales si hay cheques
            if (_chequesParaReporte != null && _chequesParaReporte.Count > 0)
            {
                CargarDatosRealesEnVistaPrevia();
            }
            else
            {
                CargarFilaEjemplo();
            }
        }

        private void CargarDatosRealesEnVistaPrevia()
        {
            var chequesPorMes = _chequesParaReporte
                .GroupBy(c => new { c.IssueDate.Year, c.IssueDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToList();

            foreach (var grupo in chequesPorMes)
            {
                int totalEmitidos = grupo.Count();

                // ⭐⭐⭐ LÓGICA CORREGIDA DE ESTADOS
                int pendientes = grupo.Count(c =>
                    string.IsNullOrWhiteSpace(c.FileControl) ||
                    c.FileControl == "PENDIENTE");

                int trasladados = grupo.Count(c =>
                    c.FileControl == "TRASLADADO" ||
                    c.FileControl == "RECIBIDO" ||
                    c.FileControl == "ARCHIVADO");

                int recibidos = grupo.Count(c =>
                    c.FileControl == "RECIBIDO" ||
                    c.FileControl == "ARCHIVADO");

                int archivados = grupo.Count(c =>
                    c.FileControl == "ARCHIVADO");

                // Calcular porcentajes
                double porcPendientes = totalEmitidos > 0 ? (pendientes * 100.0 / totalEmitidos) : 0;
                double porcTrasladados = totalEmitidos > 0 ? (trasladados * 100.0 / totalEmitidos) : 0;
                double porcRecibidos = totalEmitidos > 0 ? (recibidos * 100.0 / totalEmitidos) : 0;
                double porcArchivados = totalEmitidos > 0 ? (archivados * 100.0 / totalEmitidos) : 0;

                string nombreMes = ObtenerNombreMes(grupo.Key.Month);

                // ⭐⭐⭐ AGREGAR VALORES EN EL ORDEN DINÁMICO
                List<object> valores = new List<object>();

                foreach (string nombreCol in _ordenSeleccionColumnas)
                {
                    switch (nombreCol)
                    {
                        case "Mes":
                            if (CheckBox_Col1.Checked)
                                valores.Add(nombreMes);
                            break;
                        case "Emitidos":
                            if (CheckBox_Col2.Checked)
                                valores.Add(totalEmitidos);
                            break;
                        case "Pendientes":
                            if (CheckBox_Col3.Checked)
                                valores.Add(pendientes);
                            break;
                        case "PendientesPorc":
                            if (CheckBox_Col4.Checked)
                                valores.Add($"{porcPendientes:F2}%");
                            break;
                        case "Trasladados":
                            if (CheckBox_Col5.Checked)
                                valores.Add(trasladados);
                            break;
                        case "TrasladadosPorc":
                            if (CheckBox_Col6.Checked)
                                valores.Add($"{porcTrasladados:F2}%");
                            break;
                        case "Recibidos":
                            if (CheckBox_Col7.Checked)
                                valores.Add(recibidos);
                            break;
                        case "RecibidosPorc":
                            if (CheckBox_Col8.Checked)
                                valores.Add($"{porcRecibidos:F2}%");
                            break;
                        case "Archivados":
                            if (CheckBox_Col9.Checked)
                                valores.Add(archivados);
                            break;
                        case "ArchivadosPorc":
                            if (CheckBox_Col10.Checked)
                                valores.Add($"{porcArchivados:F2}%");
                            break;
                    }
                }

                Tabla.Rows.Add(valores.ToArray());
            }
        }

        private void CargarFilaEjemplo()
        {
            List<object> valores = new List<object>();

            // ⭐⭐⭐ AGREGAR VALORES EN EL ORDEN DINÁMICO
            foreach (string nombreCol in _ordenSeleccionColumnas)
            {
                switch (nombreCol)
                {
                    case "Mes":
                        if (CheckBox_Col1.Checked)
                            valores.Add("ENERO");
                        break;
                    case "Emitidos":
                        if (CheckBox_Col2.Checked)
                            valores.Add(100);
                        break;
                    case "Pendientes":
                        if (CheckBox_Col3.Checked)
                            valores.Add(5);
                        break;
                    case "PendientesPorc":
                        if (CheckBox_Col4.Checked)
                            valores.Add("5.00%");
                        break;
                    case "Trasladados":
                        if (CheckBox_Col5.Checked)
                            valores.Add(95);
                        break;
                    case "TrasladadosPorc":
                        if (CheckBox_Col6.Checked)
                            valores.Add("95.00%");
                        break;
                    case "Recibidos":
                        if (CheckBox_Col7.Checked)
                            valores.Add(90);
                        break;
                    case "RecibidosPorc":
                        if (CheckBox_Col8.Checked)
                            valores.Add("90.00%");
                        break;
                    case "Archivados":
                        if (CheckBox_Col9.Checked)
                            valores.Add(85);
                        break;
                    case "ArchivadosPorc":
                        if (CheckBox_Col10.Checked)
                            valores.Add("85.00%");
                        break;
                }
            }

            Tabla.Rows.Add(valores.ToArray());
        }

        private string ObtenerNombreMes(int mes)
        {
            string[] meses = {
                "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO",
                "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
            };
            return meses[mes - 1];
        }
        #endregion Vista Previa
        #region Botones Acción
        private void Btn_SelectAll_Click(object sender, EventArgs e)
        {
            // ⭐ Reiniciar orden a natural
            InicializarOrdenColumnas();

            CheckBox_Col1.Checked = true;
            CheckBox_Col2.Checked = true;
            CheckBox_Col3.Checked = true;
            CheckBox_Col4.Checked = true;
            CheckBox_Col5.Checked = true;
            CheckBox_Col6.Checked = true;
            CheckBox_Col7.Checked = true;
            CheckBox_Col8.Checked = true;
            CheckBox_Col9.Checked = true;
            CheckBox_Col10.Checked = true;
        }

        private void Btn_RemoveAll_Click(object sender, EventArgs e)
        {
            CheckBox_Col1.Checked = false;
            CheckBox_Col2.Checked = false;
            CheckBox_Col3.Checked = false;
            CheckBox_Col4.Checked = false;
            CheckBox_Col5.Checked = false;
            CheckBox_Col6.Checked = false;
            CheckBox_Col7.Checked = false;
            CheckBox_Col8.Checked = false;
            CheckBox_Col9.Checked = false;
            CheckBox_Col10.Checked = false;

            // ⭐ Limpiar orden
            _ordenSeleccionColumnas.Clear();
        }

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            // Validar que al menos una columna esté seleccionada
            if (!CheckBox_Col1.Checked && !CheckBox_Col2.Checked && !CheckBox_Col3.Checked &&
                !CheckBox_Col4.Checked && !CheckBox_Col5.Checked && !CheckBox_Col6.Checked &&
                !CheckBox_Col7.Checked && !CheckBox_Col8.Checked && !CheckBox_Col9.Checked &&
                !CheckBox_Col10.Checked)
            {
                MessageBox.Show("DEBE SELECCIONAR AL MENOS UNA COLUMNA PARA EXPORTAR",
                               "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Guardar configuración seleccionada
            ConfiguracionSeleccionada = new ReportColumnConfig
            {
                IncluirMes = CheckBox_Col1.Checked,
                IncluirEmitidos = CheckBox_Col2.Checked,
                IncluirPendientes = CheckBox_Col3.Checked,
                IncluirPendientesPorcentaje = CheckBox_Col4.Checked,
                IncluirTrasladados = CheckBox_Col5.Checked,
                IncluirTrasladadosPorcentaje = CheckBox_Col6.Checked,
                IncluirRecibidos = CheckBox_Col7.Checked,
                IncluirRecibidosPorcentaje = CheckBox_Col8.Checked,
                IncluirArchivados = CheckBox_Col9.Checked,
                IncluirArchivadosPorcentaje = CheckBox_Col10.Checked,
                OrdenColumnas = new List<string>(_ordenSeleccionColumnas) // ⭐⭐⭐ GUARDAR ORDEN
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion Botones Acción
    }
}