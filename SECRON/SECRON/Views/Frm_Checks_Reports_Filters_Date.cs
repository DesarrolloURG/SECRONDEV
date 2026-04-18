using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Checks_Reports_Filters_Date : Form
    {
        #region PropiedadesPublicas
        public List<DateTime> MesesExcluidosSeleccionados
        {
            get { return ObtenerMesesSeleccionados(); }
        }

        public List<DateTime> FechasExcluidasSeleccionadas
        {
            get { return ObtenerFechasSeleccionadas(); }
        }

        public DateTime FechaInicioSeleccionada
        {
            get { return DTP_FechaInicio.Value.Date; }
        }

        public DateTime FechaFinSeleccionada
        {
            get { return DTP_FechaFin.Value.Date; }
        }

        public DateTime FechaInicioInicial { get; set; }
        public DateTime FechaFinInicial { get; set; }
        public List<DateTime> MesesExcluidosIniciales { get; set; } = new List<DateTime>();
        public List<DateTime> FechasExcluidasIniciales { get; set; } = new List<DateTime>();

        private bool _precargaAplicada = false;

        #endregion

        #region PropiedadesIniciales
        private bool _cargandoExclusiones = false;

        private class OpcionMesExclusion
        {
            public DateTime Mes { get; set; }
            public override string ToString()
            {
                var cultura = new System.Globalization.CultureInfo("es-GT");
                return cultura.TextInfo.ToTitleCase(Mes.ToString("MMMM yyyy", cultura));
            }
        }

        private class OpcionFechaExclusion
        {
            public DateTime Fecha { get; set; }
            public override string ToString()
            {
                return Fecha.ToString("dd/MM/yyyy");
            }
        }

        #endregion PropiedadesIniciales
        #region Constructor

        private void ConfigurarTamañoFormulario()
        {
            int Ancho= 440;
            int Alto = 465;
            this.Size = new Size(Ancho, Alto);
            this.MinimumSize = new Size(Ancho, Alto);
            this.MaximumSize = new Size(Ancho, Alto);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        public Frm_Checks_Reports_Filters_Date()
        {
            InitializeComponent();
            ConfigurarTamañoFormulario();
            ConfigurarDateTimePickers();
            this.Load += Frm_Checks_Reports_Filters_Date_Load;
        }

        private void Frm_Checks_Reports_Filters_Date_Load(object sender, EventArgs e)
        {
            DTP_FechaInicio.Value = FechaInicioInicial;
            DTP_FechaFin.Value = FechaFinInicial;

            Clb_MesesExcluir.CheckOnClick = true;
            Clb_FechasExcluir.CheckOnClick = true;

            DTP_FechaInicio.ValueChanged += DTP_FechaInicio_ValueChanged;
            DTP_FechaFin.ValueChanged += DTP_FechaFin_ValueChanged;
            Clb_MesesExcluir.ItemCheck += Clb_MesesExcluir_ItemCheck;

            Btn_CleanSelect.Click += Btn_CleanSelect_Click;
            Btn_No.Click += Btn_No_Click;
            Btn_Yes.Click += Btn_Yes_Click;

            CargarExclusionesRango();
        }

        private void CargarExclusionesRango()
        {
            if (_cargandoExclusiones) return;

            try
            {
                _cargandoExclusiones = true;

                DateTime fechaInicio = DTP_FechaInicio.Value.Date;
                DateTime fechaFin = DTP_FechaFin.Value.Date;

                if (fechaInicio > fechaFin)
                {
                    Clb_MesesExcluir.Items.Clear();
                    Clb_FechasExcluir.Items.Clear();
                    return;
                }

                List<DateTime> mesesMarcados;
                List<DateTime> fechasMarcadas;

                if (!_precargaAplicada)
                {
                    mesesMarcados = MesesExcluidosIniciales ?? new List<DateTime>();
                    fechasMarcadas = FechasExcluidasIniciales ?? new List<DateTime>();
                }
                else
                {
                    mesesMarcados = ObtenerMesesSeleccionados();
                    fechasMarcadas = ObtenerFechasSeleccionadas();
                }

                CargarMesesExcluir(fechaInicio, fechaFin, mesesMarcados);
                CargarFechasExcluir(fechaInicio, fechaFin, mesesMarcados, fechasMarcadas);

                _precargaAplicada = true;
            }
            finally
            {
                _cargandoExclusiones = false;
            }
        }

        private void CargarMesesExcluir(DateTime fechaInicio, DateTime fechaFin, List<DateTime> mesesMarcados)
        {
            Clb_MesesExcluir.BeginUpdate();
            Clb_MesesExcluir.Items.Clear();

            DateTime cursorMes = new DateTime(fechaInicio.Year, fechaInicio.Month, 1);
            DateTime ultimoMes = new DateTime(fechaFin.Year, fechaFin.Month, 1);

            while (cursorMes <= ultimoMes)
            {
                var opcion = new OpcionMesExclusion
                {
                    Mes = cursorMes
                };

                int index = Clb_MesesExcluir.Items.Add(opcion);

                bool marcado = mesesMarcados.Any(m => m.Year == cursorMes.Year && m.Month == cursorMes.Month);
                if (marcado)
                    Clb_MesesExcluir.SetItemChecked(index, true);

                cursorMes = cursorMes.AddMonths(1);
            }

            Clb_MesesExcluir.EndUpdate();
        }
        private void CargarFechasExcluir(DateTime fechaInicio, DateTime fechaFin, List<DateTime> mesesMarcados, List<DateTime> fechasMarcadas)
        {
            Clb_FechasExcluir.BeginUpdate();
            Clb_FechasExcluir.Items.Clear();

            DateTime cursorFecha = fechaInicio;

            while (cursorFecha <= fechaFin)
            {
                bool mesExcluido = mesesMarcados.Any(m => m.Year == cursorFecha.Year && m.Month == cursorFecha.Month);

                if (!mesExcluido)
                {
                    var opcion = new OpcionFechaExclusion
                    {
                        Fecha = cursorFecha
                    };

                    int index = Clb_FechasExcluir.Items.Add(opcion);

                    bool marcada = fechasMarcadas.Any(f => f.Date == cursorFecha.Date);
                    if (marcada)
                        Clb_FechasExcluir.SetItemChecked(index, true);
                }

                cursorFecha = cursorFecha.AddDays(1);
            }

            Clb_FechasExcluir.EndUpdate();
        }

        private List<DateTime> ObtenerMesesSeleccionados()
        {
            List<DateTime> meses = new List<DateTime>();

            foreach (var item in Clb_MesesExcluir.CheckedItems)
            {
                if (item is OpcionMesExclusion opcion)
                {
                    meses.Add(new DateTime(opcion.Mes.Year, opcion.Mes.Month, 1));
                }
            }

            return meses;
        }

        private List<DateTime> ObtenerFechasSeleccionadas()
        {
            List<DateTime> fechas = new List<DateTime>();

            foreach (var item in Clb_FechasExcluir.CheckedItems)
            {
                if (item is OpcionFechaExclusion opcion)
                {
                    fechas.Add(opcion.Fecha.Date);
                }
            }

            return fechas;
        }

        private void DTP_FechaInicio_ValueChanged(object sender, EventArgs e)
        {
            ReiniciarExclusiones();
        }

        private void DTP_FechaFin_ValueChanged(object sender, EventArgs e)
        {
            ReiniciarExclusiones();
        }

        private void ReiniciarExclusiones()
        {
            try
            {
                _cargandoExclusiones = true;
                _precargaAplicada = true;

                Clb_MesesExcluir.Items.Clear();
                Clb_FechasExcluir.Items.Clear();
            }
            finally
            {
                _cargandoExclusiones = false;
            }

            CargarExclusionesRango();
        }

        private void Clb_MesesExcluir_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_cargandoExclusiones) return;

            BeginInvoke(new Action(() =>
            {
                if (_cargandoExclusiones) return;

                try
                {
                    _cargandoExclusiones = true;

                    DateTime fechaInicio = DTP_FechaInicio.Value.Date;
                    DateTime fechaFin = DTP_FechaFin.Value.Date;

                    List<DateTime> mesesMarcados = ObtenerMesesSeleccionados();
                    List<DateTime> fechasMarcadas = ObtenerFechasSeleccionadas();

                    CargarFechasExcluir(fechaInicio, fechaFin, mesesMarcados, fechasMarcadas);
                }
                finally
                {
                    _cargandoExclusiones = false;
                }
            }));
        }

        private void Btn_CleanSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Clb_MesesExcluir.Items.Count; i++)
                Clb_MesesExcluir.SetItemChecked(i, false);

            for (int i = 0; i < Clb_FechasExcluir.Items.Count; i++)
                Clb_FechasExcluir.SetItemChecked(i, false);

            CargarExclusionesRango();
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion Constructor

        private void ConfigurarDateTimePickers()
        {
            DTP_FechaInicio.Format = DateTimePickerFormat.Short;
            DTP_FechaInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            DTP_FechaFin.Format = DateTimePickerFormat.Short;
            DTP_FechaFin.Value = DateTime.Now;

        }    
    }
}