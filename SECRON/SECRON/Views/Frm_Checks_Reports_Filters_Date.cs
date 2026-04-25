using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        #endregion

        #region PropiedadesPrivadas

        private bool _cargandoExclusiones = false;
        private bool _precargaAplicada = false;

        // Mes actualmente visible en el calendario
        private int _viewYear;
        private int _viewMonth;

        // Conjuntos de exclusiones activas
        private HashSet<string> _mesesExcluidos = new HashSet<string>();
        private HashSet<string> _diasExcluidos = new HashSet<string>();

        // Colores
        private static readonly Color ColorMesExcluido = Color.FromArgb(250, 236, 231);
        private static readonly Color ColorMesExcluidoTexto = Color.FromArgb(153, 60, 29);
        private static readonly Color ColorDiaExcluido = Color.FromArgb(250, 238, 218);
        private static readonly Color ColorDiaExcluidoTexto = Color.FromArgb(133, 79, 11);
        private static readonly Color ColorFueraRango = Color.FromArgb(240, 240, 240);
        private static readonly Color ColorFueraRangoTexto = Color.FromArgb(180, 180, 180);
        private static readonly Color ColorNormal = Color.White;
        private static readonly Color ColorNormalTexto = Color.FromArgb(40, 40, 40);
        private static readonly Color ColorHoy = Color.FromArgb(230, 245, 255);
        private static readonly Color ColorHoyTexto = Color.FromArgb(24, 95, 165);

        private Button[] _dayCells = new Button[42];
        private readonly string[] _nombresDias = { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };
        private readonly string[] _nombresMeses = {
            "Enero","Febrero","Marzo","Abril","Mayo","Junio",
            "Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre"
        };

        #endregion

        #region Constructor

        private void ConfigurarTamañoFormulario()
        {
            int Ancho = 700;
            int Alto = 650;
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
            // Load conectado en el Designer
        }

        private void Frm_Checks_Reports_Filters_Date_Load(object sender, EventArgs e)
        {
            DTP_FechaInicio.Value = FechaInicioInicial;
            DTP_FechaFin.Value = FechaFinInicial;

            // Precargar exclusiones iniciales
            foreach (var m in MesesExcluidosIniciales ?? new List<DateTime>())
                _mesesExcluidos.Add(MonthKey(m));

            foreach (var f in FechasExcluidasIniciales ?? new List<DateTime>())
                _diasExcluidos.Add(DayKey(f));

            _precargaAplicada = true;

            _viewYear = DTP_FechaInicio.Value.Year;
            _viewMonth = DTP_FechaInicio.Value.Month - 1;

            DTP_FechaInicio.ValueChanged += DTP_FechaInicio_ValueChanged;
            DTP_FechaFin.ValueChanged += DTP_FechaFin_ValueChanged;
            Btn_PrevMonth.Click += (s, ev) => CambiarMes(-1);
            Btn_NextMonth.Click += (s, ev) => CambiarMes(1);
            Chk_ExcluirMes.CheckedChanged += Chk_ExcluirMes_CheckedChanged;
            Btn_CleanSelect.Click += Btn_CleanSelect_Click;

            ConstruirCeldas();
            RenderizarCalendario();
            ActualizarChips();
        }

        #endregion

        #region Calendario

        // Dimensiones de cada celda del calendario
        private const int CeldaAncho = 88;
        private const int CeldaAlto = 46;
        private const int CeldaGapX = 2;
        private const int CeldaGapY = 2;
        private const int CalPadding = 4;

        private void ConstruirCeldas()
        {
            // Encabezados días de semana (sobre el Pnl_Calendario)
            for (int i = 0; i < 7; i++)
            {
                var lbl = new Label
                {
                    Text = _nombresDias[i],
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(120, 120, 120),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(CeldaAncho, 20),
                    Location = new Point(
                        Pnl_Calendario.Left + CalPadding + i * (CeldaAncho + CeldaGapX),
                        Pnl_Calendario.Top - 22),
                    BackColor = Color.Transparent
                };
                Panel_1.Controls.Add(lbl);
                lbl.BringToFront();
            }

            // Celdas de días
            for (int i = 0; i < 42; i++)
            {
                var btn = new Button
                {
                    Size = new Size(CeldaAncho, CeldaAlto),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10f),
                    TabStop = false,
                    Cursor = Cursors.Hand,
                    Tag = i
                };
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                btn.Click += DayCell_Click;
                _dayCells[i] = btn;
                Pnl_Calendario.Controls.Add(btn);
                btn.Location = new Point(
                    CalPadding + (i % 7) * (CeldaAncho + CeldaGapX),
                    CalPadding + (i / 7) * (CeldaAlto + CeldaGapY));
            }
        }

        private bool _actualizandoChk = false;

        private void RenderizarCalendario()
        {
            Pnl_Calendario.SuspendLayout();

            // Cabecera: nombre mes y año
            Lbl_MesAno.Text = $"{_nombresMeses[_viewMonth]} {_viewYear}";

            // Checkbox del mes — sin disparar el evento
            string mk = MonthKey(_viewYear, _viewMonth + 1);
            bool mesExcluido = _mesesExcluidos.Contains(mk);
            _actualizandoChk = true;
            Chk_ExcluirMes.Checked = mesExcluido;
            Chk_ExcluirMes.Text = mesExcluido ? "Mes excluido completo" : "Excluir todo este mes";
            _actualizandoChk = false;

            // Rango activo
            DateTime inicio = DTP_FechaInicio.Value.Date;
            DateTime fin = DTP_FechaFin.Value.Date;

            // Calcular offsets del mes visible
            int primerDiaSemana = (int)new DateTime(_viewYear, _viewMonth + 1, 1).DayOfWeek;
            int diasEnMes = DateTime.DaysInMonth(_viewYear, _viewMonth + 1);
            int anioMesAnt = _viewMonth == 0 ? _viewYear - 1 : _viewYear;
            int numMesAnt = _viewMonth == 0 ? 12 : _viewMonth;
            int diasMesAnt = DateTime.DaysInMonth(anioMesAnt, numMesAnt);
            int anioMesSig = _viewMonth == 11 ? _viewYear + 1 : _viewYear;
            int numMesSig = _viewMonth == 11 ? 1 : _viewMonth + 2;

            for (int i = 0; i < 42; i++)
            {
                var btn = _dayCells[i];
                DateTime fecha;
                bool esOtroMes;

                if (i < primerDiaSemana)
                {
                    // Días del mes anterior
                    int dia = diasMesAnt - primerDiaSemana + i + 1;
                    fecha = new DateTime(anioMesAnt, numMesAnt, dia);
                    esOtroMes = true;
                }
                else if (i - primerDiaSemana < diasEnMes)
                {
                    // Días del mes actual
                    int dia = i - primerDiaSemana + 1;
                    fecha = new DateTime(_viewYear, _viewMonth + 1, dia);
                    esOtroMes = false;
                }
                else
                {
                    // Días del mes siguiente
                    int dia = i - primerDiaSemana - diasEnMes + 1;
                    fecha = new DateTime(anioMesSig, numMesSig, dia);
                    esOtroMes = true;
                }

                // Siempre actualizar texto y tag
                btn.Text = fecha.Day.ToString();
                btn.Tag = fecha;

                if (esOtroMes)
                {
                    Pintar(btn, ColorFueraRango, ColorFueraRangoTexto, ColorFueraRango, false);
                    continue;
                }

                bool enRango = fecha >= inicio && fecha <= fin;

                if (!enRango)
                    Pintar(btn, ColorFueraRango, ColorFueraRangoTexto, ColorFueraRango, false);
                else if (mesExcluido)
                    Pintar(btn, ColorMesExcluido, ColorMesExcluidoTexto, Color.FromArgb(240, 153, 123), true);
                else if (_diasExcluidos.Contains(DayKey(fecha)))
                    Pintar(btn, ColorDiaExcluido, ColorDiaExcluidoTexto, Color.FromArgb(239, 159, 39), true);
                else if (fecha.Date == DateTime.Today)
                    Pintar(btn, ColorHoy, ColorHoyTexto, Color.FromArgb(135, 183, 235), true);
                else
                    Pintar(btn, ColorNormal, ColorNormalTexto, Color.FromArgb(220, 220, 220), true);
            }

            Pnl_Calendario.ResumeLayout(false);
            foreach (Button b in _dayCells)
                b.Invalidate();
            Pnl_Calendario.Update();
        }

        private void Pintar(Button btn, Color fondo, Color texto, Color borde, bool habilitado)
        {
            btn.BackColor = fondo;
            btn.ForeColor = texto;
            btn.FlatAppearance.BorderColor = borde;
            btn.Enabled = habilitado;
        }

        private void DayCell_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is DateTime fecha)) return;

            string mk = MonthKey(fecha);
            string dk = DayKey(fecha);

            if (_mesesExcluidos.Contains(mk))
            {
                // Mes excluido: convertir a exclusión por días individuales, excepto el día clickeado
                _mesesExcluidos.Remove(mk);

                DateTime inicio = DTP_FechaInicio.Value.Date;
                DateTime fin = DTP_FechaFin.Value.Date;
                int diasEnMes = DateTime.DaysInMonth(fecha.Year, fecha.Month);

                for (int d = 1; d <= diasEnMes; d++)
                {
                    var diaActual = new DateTime(fecha.Year, fecha.Month, d);
                    if (diaActual >= inicio && diaActual <= fin && diaActual.Date != fecha.Date)
                        _diasExcluidos.Add(DayKey(diaActual));
                }
            }
            else
            {
                // Comportamiento normal: toggle del día
                if (_diasExcluidos.Contains(dk))
                    _diasExcluidos.Remove(dk);
                else
                    _diasExcluidos.Add(dk);

                // Si ahora todos los días del mes en rango están excluidos → convertir a mes excluido
                DateTime ini = DTP_FechaInicio.Value.Date;
                DateTime fn = DTP_FechaFin.Value.Date;
                int total = DateTime.DaysInMonth(fecha.Year, fecha.Month);
                bool todosExcluidos = true;

                for (int d = 1; d <= total; d++)
                {
                    var diaActual = new DateTime(fecha.Year, fecha.Month, d);
                    if (diaActual < ini || diaActual > fn) continue;
                    if (!_diasExcluidos.Contains(DayKey(diaActual))) { todosExcluidos = false; break; }
                }

                if (todosExcluidos)
                {
                    _mesesExcluidos.Add(mk);
                    _diasExcluidos.RemoveWhere(x => x.StartsWith($"{fecha.Year:D4}-{fecha.Month:D2}"));
                }
            }

            RenderizarCalendario();
            ActualizarChips();
        }

        private void Chk_ExcluirMes_CheckedChanged(object sender, EventArgs e)
        {
            if (_actualizandoChk) return;

            string mk = MonthKey(_viewYear, _viewMonth + 1);
            if (Chk_ExcluirMes.Checked)
            {
                _mesesExcluidos.Add(mk);
                // Quitar días individuales de ese mes (ya cubiertos por exclusión de mes)
                _diasExcluidos.RemoveWhere(dk =>
                {
                    if (DateTime.TryParseExact(dk, "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime d))
                        return d.Year == _viewYear && d.Month == _viewMonth + 1;
                    return false;
                });
            }
            else
            {
                _mesesExcluidos.Remove(mk);
            }

            RenderizarCalendario();
            ActualizarChips();
        }

        private void CambiarMes(int delta)
        {
            int nuevoMes = _viewMonth + delta;
            int nuevoAnio = _viewYear;
            if (nuevoMes < 0) { nuevoMes = 11; nuevoAnio--; }
            if (nuevoMes > 11) { nuevoMes = 0; nuevoAnio++; }

            // Limitar navegación al rango de fechas
            var inicio = DTP_FechaInicio.Value.Date;
            var fin = DTP_FechaFin.Value.Date;
            var primerDiaNuevoMes = new DateTime(nuevoAnio, nuevoMes + 1, 1);
            var ultimoDiaNuevoMes = new DateTime(nuevoAnio, nuevoMes + 1,
                DateTime.DaysInMonth(nuevoAnio, nuevoMes + 1));

            // No navegar si el mes destino queda completamente fuera del rango
            if (ultimoDiaNuevoMes < inicio || primerDiaNuevoMes > fin) return;

            _viewMonth = nuevoMes;
            _viewYear = nuevoAnio;
            RenderizarCalendario();
        }

        #endregion

        #region Chips / Resumen Exclusiones

        private void ActualizarChips()
        {
            Flp_Chips.Controls.Clear();

            bool hayExclusiones = _mesesExcluidos.Count > 0 || _diasExcluidos.Count > 0;
            Lbl_SinExclusiones.Visible = !hayExclusiones;

            var cultura = new System.Globalization.CultureInfo("es-GT");

            foreach (var mk in _mesesExcluidos.OrderBy(x => x))
            {
                var parts = mk.Split('-');
                var fecha = new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), 1);
                string texto = cultura.TextInfo.ToTitleCase(fecha.ToString("MMMM yyyy", cultura));
                AgregarChip(texto, mk, true);
            }

            foreach (var dk in _diasExcluidos.OrderBy(x => x))
            {
                if (DateTime.TryParseExact(dk, "yyyy-MM-dd", cultura,
                    System.Globalization.DateTimeStyles.None, out DateTime f))
                {
                    string texto = f.ToString("dd/MM/yy");
                    AgregarChip(texto, dk, false);
                }
            }
        }

        private void AgregarChip(string texto, string key, bool esMes)
        {
            var pnl = new Panel
            {
                Height = 24,
                AutoSize = true,
                Margin = new Padding(0, 2, 4, 2),
                BackColor = esMes ? ColorMesExcluido : ColorDiaExcluido
            };
            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = RoundedRect(pnl.ClientRectangle, 10))
                using (var brush = new SolidBrush(esMes ? ColorMesExcluido : ColorDiaExcluido))
                    g.FillPath(brush, path);
            };

            var lbl = new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = esMes ? ColorMesExcluidoTexto : ColorDiaExcluidoTexto,
                AutoSize = true,
                Location = new Point(8, 4),
                BackColor = Color.Transparent
            };

            var btnX = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 7f),
                ForeColor = esMes ? ColorMesExcluidoTexto : ColorDiaExcluidoTexto,
                Size = new Size(16, 16),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                TabStop = false
            };
            btnX.FlatAppearance.BorderSize = 0;
            btnX.Tag = new object[] { key, esMes };
            btnX.Click += BtnChipEliminar_Click;

            pnl.Controls.Add(lbl);
            pnl.Controls.Add(btnX);

            lbl.Location = new Point(8, 4);
            pnl.Width = lbl.PreferredWidth + 32;
            btnX.Location = new Point(pnl.Width - 20, 4);

            Flp_Chips.Controls.Add(pnl);
        }

        private void BtnChipEliminar_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is object[] tag)) return;
            string key = tag[0].ToString();
            bool esMes = (bool)tag[1];

            if (esMes) _mesesExcluidos.Remove(key);
            else _diasExcluidos.Remove(key);

            RenderizarCalendario();
            ActualizarChips();
        }

        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion

        #region Helpers

        private static string MonthKey(DateTime d) => $"{d.Year:D4}-{d.Month:D2}";
        private static string MonthKey(int year, int month) => $"{year:D4}-{month:D2}";
        private static string DayKey(DateTime d) => d.ToString("yyyy-MM-dd");

        private List<DateTime> ObtenerMesesSeleccionados()
        {
            var lista = new List<DateTime>();
            foreach (var mk in _mesesExcluidos)
            {
                var p = mk.Split('-');
                lista.Add(new DateTime(int.Parse(p[0]), int.Parse(p[1]), 1));
            }
            return lista;
        }

        private List<DateTime> ObtenerFechasSeleccionadas()
        {
            var lista = new List<DateTime>();
            var cultura = System.Globalization.CultureInfo.InvariantCulture;
            foreach (var dk in _diasExcluidos)
            {
                if (DateTime.TryParseExact(dk, "yyyy-MM-dd", cultura,
                    System.Globalization.DateTimeStyles.None, out DateTime f))
                    lista.Add(f.Date);
            }
            return lista;
        }

        #endregion

        #region Eventos DTP y botones

        private void DTP_FechaInicio_ValueChanged(object sender, EventArgs e)
        {
            _viewYear = DTP_FechaInicio.Value.Year;
            _viewMonth = DTP_FechaInicio.Value.Month - 1;
            RenderizarCalendario();
            ActualizarChips();
        }

        private void DTP_FechaFin_ValueChanged(object sender, EventArgs e)
        {
            RenderizarCalendario();
            ActualizarChips();
        }

        private void Btn_CleanSelect_Click(object sender, EventArgs e)
        {
            _mesesExcluidos.Clear();
            _diasExcluidos.Clear();
            RenderizarCalendario();
            ActualizarChips();
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

        private void ConfigurarDateTimePickers()
        {
            DTP_FechaInicio.Format = DateTimePickerFormat.Short;
            DTP_FechaInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DTP_FechaFin.Format = DateTimePickerFormat.Short;
            DTP_FechaFin.Value = DateTime.Now;
        }

        #endregion
    }
}