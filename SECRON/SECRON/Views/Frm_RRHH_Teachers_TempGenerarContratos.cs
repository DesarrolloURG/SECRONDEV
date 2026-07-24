using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_RRHH_Teachers_TempGenerarContratos : Form
    {
        public Mdl_Security_UserInfo UserData { get; set; }

        private PrintDocument _printDocument;
        private List<LineaImpresion> _lineas;
        private int _lineaActualIndex;
        private Mdl_DocentesTemporal _docenteActual;
        private List<Mdl_DocentesTemporal_Cursos> _cursosActual;

        // ================= TEXTO LEGAL FIJO (extraído literal del contrato oficial) =================
        // NOTA: se preservan tal cual del original algunos detalles del documento fuente:
        // - Falta la cláusula "SÉPTIMA" (el original salta de SEXTA a OCTAVA) -- no es un error mío,
        //   así viene en el .docx oficial. Avisar a Legal si debe corregirse.
        // - Algunas palabras tienen errores de tipeo en el original (ej. "CUMPLIMIEMTO", "PRESTACIONS")
        //   -- se dejan igual, no se corrigen por iniciativa propia.
        // - El párrafo del Rector aparecía DUPLICADO dos veces seguidas en el .docx original; aquí se
        //   incluye UNA sola vez. Confirmar con el usuario si debía repetirse a propósito.

        private const string TituloContrato1 = "CONTRATO DE SERVICIOS PROFESIONALES DE DOCENCIA";
        private const string TituloContrato2 = "A TIEMPO PARCIAL DEFINIDO";
        private const string TituloUniversidad = "UNIVERSIDAD REGIONAL DE GUATEMALA";

        private const string TextoRector =
            "Mynor René Cordón y Cordón, guatemalteco, casado, Médico y Cirujano, de este domicilio y vecindad, se identifica con el Documento Personal de Identificación -DPI- con Código Único de Identificación -CUI- número mil novecientos treinta y seis, diez mil setecientos noventa y cinco, un mil novecientos dos (1936 10795 1902), emitido por el Registro Nacional de las Personas -RENAP- de la Republica de Guatemala,  quien actúa en su calidad de Rector y Representante Legal de la Universidad Regional de Guatemala, calidad que acredita mediante el Punto QUINTO, del Acta 30-09-2025-001 de la sesión celebrada por la Asamblea General Extraordinaria de los Asociados Activos del Patronato de la Universidad Regional de Guatemala, de fecha treinta de septiembre del año 2025, el cual fue inscrito en el Registro de las Personas Jurídicas del Ministerio de Gobernación, bajo la partida número 25, Folio 25, del Libro 3 de Asociaciones Civiles. Lugar que señalo para recibir notificaciones: Oficinas Centrales de la Universidad Regional de Guatemala, ubicadas en Avenida las Américas 20-84 zona 13 de la ciudad de Guatemala del Departamento de Guatemala.";

        private const string ClausulaIntro =
            "Ambos otorgantes manifestamos encontrarnos en el libre ejercicio de sus derechos civiles y que la representación que se ejercita es suficiente conforme a la ley, para la celebración del presente Contrato de Servicios Profesionales de Docencia. Para efectos del presente contrato este instrumento, en adelante los comparecientes se denominarán simplemente como \u201cLA UNIVERSIDAD Y EL PROFESIONAL\u201d; ambos comparecientes convenimos en suscribir el mismo, conforme las siguientes cláusulas:";

        private static readonly (string Ordinal, string Cuerpo)[] Clausulas = new (string, string)[]
        {
            ("PRIMERA", "BASE LEGAL. El presente contrato se suscribe con fundamento legal en:  la Constitución Política de la República de Guatemala, Convenios Internacionales de la OIT, Código de Trabajo de Guatemala, Reglamento Interno de la Universidad Regional de Guatemala y el Reglamento de Régimen Académico de La Universidad Regional de Guatemala, aprobado por el Consejo Directivo de la Universidad Regional de Guatemala;"),
            ("SEGUNDA", "OBJETO DEL CONTRATO. El (LA) PROFESIONAL presta sus servicios profesionales como docente de LA UNIVERSIDAD y a la Universidad, como docente brindará sus conocimientos y experiencia en beneficio de los futuros profesionales (estudiantes) a quienes también se les brindará un apoyo psicológico si es requerido. El complimiento de su desempeño como profesional brindará una enseñanza superior, según el calendario académico aprobado por el Consejo Superior de la Universidad Regional de Guatemala, conjuntamente con el coordinador de la Sede Académica, con quien coordinará el curso a impartir, la enseñanza superior será de forma presencial o virtual  de acuerdo a los horarios autorizados, que darán inicio según el calendario académico aprobado por Ciclos los cuales constituyen Primer Ciclo correspondiente al primer semestre del año en curso; Segundo Ciclo correspondiente al segundo semestres del año en curso;"),
            ("TERCERA", "CUMPLIMIEMTO DEL PROFESIONAL DOCENTE: Se compromete a prestar sus servicios profesionales de docencia, en la Sede Académica ya indicada de la Universidad Regional de Guatemala, y en cumplimiento a el calendario académico autorizado: 1) Cumplir con el desempeño como docente, con base al calendario  académico autorizado por un periodo de clase; 2) Cumplir con las evaluaciones en el tiempo establecido, las cuales quedan bajo su resguardo y únicamente se deben de realizar dentro de los salones de clase, por lo que ésta prohibido compartir por cualquier medio electrónico, la reproducción la debe de realizar por porte del docente, posteriormente coordinar el reembolso económico debidamente comprobado con factura bajo el concepto de copias de evaluaciones, según la cantidad de alumnos que debe realizar dichas evaluaciones;  3) Cumplir con las firmas correspondientes de las actas, posterior a la evaluación correspondiente y la ponderación de zona y examen de los estudiantes, según lo instruido por el coordinador de la Sede Académica; 4) Cumplir con el Reglamento interno Académico y Ético; 5) Cumplir con el horario establecido según el curso a impartir; 6) En caso fortuito de no asistir a impartir el curso como docente, la comunicación debe ser inmediata con el coordinador de la Sede Académica; 7) Cumplir con la entrega de la factura correspondiente según el calendario de pagos autorizado por el Consejo Académico de la Universidad Regional de Guatemala; 8) cumplir con los marcajes que le indicará el coordinador y brindar el apoyo solicitado por el coordinador de la Sede de la Universidad Regional;"),
            ("CUARTA", "HONORARIOS DEL PROFESIONAL DOCENTE Y PLAZO: Los honorario del docente será de acuerdo a la cantidad de cursos a impartir por jornada, la cual debe estar debidamente aprobada por el Consejo Académico de la Universidad Regional de Guatemala, el pago de los honorarios por servicios profesionales se realizará según el calendario financiero, debidamente aprobado, el cual será indicado por el coordinador de la Sede Regional; el presente contrato surte efectos a partir del Segundo Ciclo correspondiente al segundo semestres del año en curso según lo indicado por el coordinador de la Sede Académica;"),
            ("QUINTA", "DOCUMENTACIÓN A PRESENTAR \"EL PROFESIONAL\", se compromete a presenta el Registro Tributario Unificado -RTU- actualizado al mes de enero del año en curso, constancia de colegiado activo del año en curso, hoja de vida actualizada al año en curso y emisor Facturas Electrónicas -FEL-."),
            ("SEXTA", "PROFESIONALISMO, DECORO, VESTIMENTA, RESERVA,  RESGUARDO Y LA NO DIVULGACIÓN DE INFORMACIÓN DE LA UNIVERSIDAD: “EL PROFESIONAL”: a) Se reservará comentarios comparativos, que se consideren desleales a la Universidad Regional de Guatemala, por lo que en todo momento debe de prevalecer el PROFESIONALISMO y respeto; b) Debe demostrar en todo momento el decoro y vestimenta como profesional docente; c) Se responsabiliza respecto de la información que le sea proporcionada por LA UNIVERSIDAD, ya sea de forma oral, escrita, impresa, sonora, visual, electrónica, informática u holográfica, contenida en cualquier tipo de documento, que puede consistir en reportes, estudios, actas, resoluciones, oficios, correspondencia, acuerdos, directivas, directrices, circulares, contratos, instructivos, notas, memorandos o bien, cualquier otro registro que documente el ejercicio de las facultades, funciones y competencias del área universitaria sin importar su fuente o fecha de elaboración; d) La información que le sea proporcionada es considerada, bajo reservada, privilegiada, confidencial, y derechos reservados, por lo que en cualquier momento se aplicará la legislación guatemalteca, en caso sea contrario a su uso;"),
            ("SÉPTIMA", "TERMINACIÓN DEL CONTRATO: El presente contrato se dará por terminado por las siguientes causas: a) Por vencimiento del plazo; b) A solicitud unilateral de las partes; c) Por negligencia en la prestación de sus servicios como docente; d); Incumplimiento del Reglamento Académico; y en los siguientes casos: 1) Abandono por parte del  docente del lugar de trabajo sin permiso correspondiente; 2) Inasistencia al trabajo por dos veces consecutivas injustificadas; 3) Obtener dinero prestados de los estudiantes o efectuar negocios con los mismos; 4) Llegar en estado de embriaguez o ingerir bebidas alcohólicas dentro de las instalaciones de la Universidad;  5) Falta a la moral pública que lesione la dignidad o la autoestima de los estudiantes; 6) Asignar a los estudiantes tareas y trabajos de investigación que estén en oposición con lo que fijan las normas sobre evaluación Institucional; 7) Realizar propaganda de carácter político dentro de cualquier Sede Académica; 8) Se prohíbe cualquier relación sentimental con algún estudiante; 9) Acoso o coacción estudiantil en cualquier aspecto, se realizará las denuncias correspondientes para la aplicación de la Ley en materia Penal como corresponda; en cualquiera de los casos antes descritos la RESCISIÓN contractual será inmediata;"),
            ("OCTAVA", "RESCISIÓN CONTRACTUAL: \"LA UNIVERSIDAD\" se reserva el derecho de rescindir el Contrato en cualquier momento por no convenir a los intereses y sin que ello implique responsabilidad de parte. “EL PROFESIONAL” manifiesta estar en completo acuerdo renunciar a cualquier pretensión posterior en contra de la Universidad Regional de Guatemala, en materia  civil, laboral o administrativo, presente o futura que se derive de la terminación contractual;"),
            ("NOVENA", "EL PROFESIONAL reconoce y acepta que todas las invenciones, descubrimientos, obras y desarrollos realizados durante el período de vigencia de este contrato, que sean resultado directo de actividades realizadas en el marco de su función docente, serán propiedad de LA UNIVERSIDAD. El docente se compromete a ceder los derechos correspondientes a la institución para su uso y publicación."),
            ("DÉCIMA", "PAGO DE PRESTACIONS E INDEMNIZACIÓN: concluido el Segundo Ciclo correspondiente al segundo semestres del año en curso el profesional que prestas sus servicios profesionales para la Universidad Regional de Guatemala, Región 2, y en cumplimento a la Constitución Política de la Republica de Guatemala y al Código de Trabajo de Guatemala,  luego de concluido dicho ciclo y en cumplimiento con las atribuciones como docente, le serán canceladas las prestaciones que por ley le correspondan de forma proporcional así como su respectiva liquidación, para lo cual deberán firmar el finiquito correspondiente y la emisión de la factura que a entera satisfacción de las partes quedan enterados;"),
            ("DECIMA PRIMERA", "ACEPTACIÓN DEL CONTRATO: En los términos y condiciones estipuladas \"LA UNIVERSIDAD\" y \"EL PROFESIONAL” hemos leído íntegramente el presente Contrato de Servicios Profesionales como docente y bien enterados de su contenido, objeto, validez y efectos legales, lo aceptamos, ratificarnos y firmamos para los efectos legales correspondientes."),
        };

        // ================= Estructura de línea para el motor de paginación =================
        private enum TipoLinea { TituloCentrado, Subtitulo, TextoNormal, ClausulaOrdinal, Espaciador, TablaFila, Firmas, Encabezado, CajaTexto, TextoMixto }

        private class FilaTabla
        {
            public string Etiqueta1, Valor1, Etiqueta2, Valor2;
            public bool EsPrimeraFila; // dibuja el borde superior completo de la tabla
        }

        private class LineaImpresion
        {
            public TipoLinea Tipo;
            public string Texto;
            public string Texto2;
            public float Altura;
            public List<string> LineasEnvueltas;
            public FilaTabla Fila;
            public List<(string Texto, bool Negrita)> Palabras;
        }

        public Frm_RRHH_Teachers_TempGenerarContratos()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(1200, 900);
        }

        private void Frm_RRHH_Teachers_TempGenerarContratos_Load(object sender, EventArgs e)
        {
            ComboBox_Sede.DropDownStyle = ComboBoxStyle.DropDownList;

            Btn_PrintOne.Enabled = false;
            Btn_PrintAll.Enabled = false;
            Btn_Anterior.Enabled = false;
            Btn_Siguiente.Enabled = false;
            Lbl_Paginas.Text = "";

            ConfigurarTabla();
            ConfigurarBarraHerramientasPreview();
            CargarSedes();
        }

        #region Carga de combo y tabla

        private const string OpcionTodasLasSedes = "TODOS";

        private void CargarSedes()
        {
            var sedes = new List<string> { OpcionTodasLasSedes };
            sedes.AddRange(Ctrl_DocentesTemporal_Cursos.ObtenerSedes());
            ComboBox_Sede.DataSource = sedes;
        }

        private void ConfigurarTabla()
        {
            Tabla.AutoGenerateColumns = false;
            Tabla.Columns.Clear();
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12F, FontStyle.Bold);
            Tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            AgregarColumna("ContractCode", "CÓDIGO CONTRATO", 140);
            AgregarColumna("DPI", "DPI", 110);
            AgregarColumna("FirstName", "NOMBRES", 160);
            AgregarColumna("LastName", "APELLIDOS", 160);
            AgregarColumna("TotalCursos", "TOTAL CURSOS", 100);

            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void AgregarColumna(string dataProperty, string header, int fillWeight)
        {
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = dataProperty,
                HeaderText = header,
                FillWeight = fillWeight,
                ReadOnly = true
            });
        }

        private void ComboBox_Sede_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBox_Sede.SelectedItem == null) return;

            string sede = ComboBox_Sede.SelectedItem.ToString();
            List<Mdl_DocentesTemporal> docentes = sede == OpcionTodasLasSedes
                ? Ctrl_DocentesTemporal.Select()
                : Ctrl_DocentesTemporal.ObtenerPorSede(sede);

            Tabla.DataSource = null;
            Tabla.DataSource = docentes;

            Btn_PrintAll.Enabled = docentes.Count > 0;
            LimpiarPreview();

            if (docentes.Count > 0)
                SeleccionarFila(0);
        }

        #endregion

        #region Selección de fila -> vista previa automática

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.CurrentRow?.DataBoundItem is Mdl_DocentesTemporal docente)
            {
                Btn_PrintOne.Enabled = true;
                Btn_Anterior.Enabled = Tabla.CurrentRow.Index > 0;
                Btn_Siguiente.Enabled = Tabla.CurrentRow.Index < Tabla.Rows.Count - 1;

                MostrarVistaPrevia(docente);
            }
            else
            {
                Btn_PrintOne.Enabled = false;
            }
        }

        private void Btn_Anterior_Click(object sender, EventArgs e)
        {
            int indiceActual = Tabla.CurrentRow?.Index ?? 0;
            if (indiceActual > 0)
                SeleccionarFila(indiceActual - 1);
        }

        private void Btn_Siguiente_Click(object sender, EventArgs e)
        {
            int indiceActual = Tabla.CurrentRow?.Index ?? -1;
            if (indiceActual >= 0 && indiceActual < Tabla.Rows.Count - 1)
                SeleccionarFila(indiceActual + 1);
        }

        // Selecciona una fila moviendo también el CurrentCell -- .Selected = true por sí solo
        // NO mueve el CurrentCell/CurrentRow, por eso Tabla.CurrentRow.Index quedaba desactualizado
        // al navegar con los botones (aunque sí funcionaba al hacer clic directo en la fila, porque
        // el clic del usuario sí mueve el CurrentCell automáticamente).
        private void SeleccionarFila(int indice)
        {
            if (indice < 0 || indice >= Tabla.Rows.Count) return;
            Tabla.ClearSelection();
            Tabla.CurrentCell = Tabla.Rows[indice].Cells[0];
            Tabla.Rows[indice].Selected = true;
        }

        #endregion

        #region Botones: exportar/guardar PDF a disco

        private void Btn_PrintOne_Click(object sender, EventArgs e)
        {
            if (!(Tabla.CurrentRow?.DataBoundItem is Mdl_DocentesTemporal docente))
            {
                MessageBox.Show("SELECCIONE UN DOCENTE DE LA TABLA.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Archivo PDF|*.pdf",
                Title = "Guardar contrato como",
                FileName = $"CONTRATO_{docente.DPI}_{docente.ContractCode}.pdf"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                GuardarComoPdf(docente, dlg.FileName);
            }
        }

        private void Btn_PrintAll_Click(object sender, EventArgs e)
        {
            var docentes = Tabla.DataSource as List<Mdl_DocentesTemporal>;
            if (docentes == null || docentes.Count == 0)
            {
                MessageBox.Show("NO HAY DOCENTES PARA GENERAR EN ESTA SEDE.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (FolderBrowserDialog dlg = new FolderBrowserDialog
            {
                Description = "Seleccione la carpeta donde se guardarán los contratos"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                int generados = 0, errores = 0;
                foreach (var docente in docentes)
                {
                    string rutaFinal = Path.Combine(dlg.SelectedPath, $"CONTRATO_{docente.DPI}_{docente.ContractCode}.pdf");
                    if (GuardarComoPdf(docente, rutaFinal, mostrarMensaje: false))
                        generados++;
                    else
                        errores++;
                }

                MessageBox.Show(
                    $"GENERACIÓN MASIVA COMPLETADA.\n\nContratos generados: {generados}\nErrores: {errores}\n\nCarpeta: {dlg.SelectedPath}",
                    "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Genera el PDF a partir del PrintDocument (usando Microsoft Print to PDF u otra impresora PDF
        // instalada) y lo guarda en la ruta indicada.
        private bool GuardarComoPdf(Mdl_DocentesTemporal docente, string rutaDestino, bool mostrarMensaje = true)
        {
            try
            {
                var cursos = Ctrl_DocentesTemporal_Cursos.SelectByTeacherTempId(docente.TeacherTempId);
                if (cursos.Count == 0)
                {
                    MessageBox.Show($"EL DOCENTE {docente.FirstName} {docente.LastName} NO TIENE CURSOS REGISTRADOS.",
                        "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                var doc = new PrintDocument();
                doc.DefaultPageSettings.PaperSize = new PaperSize("Letter", 850, 1100);
                doc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                doc.PrinterSettings.PrintToFile = true;
                doc.PrinterSettings.PrintFileName = rutaDestino;

                _lineas = null;
                _lineaActualIndex = 0;
                _docenteActual = docente;
                _cursosActual = cursos;
                doc.PrintPage += PrintDocument_PrintPage;
                doc.Print();

                if (mostrarMensaje)
                    MessageBox.Show("CONTRATO GUARDADO EXITOSAMENTE:\n" + rutaDestino,
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL GUARDAR EL CONTRATO: " + ex.Message +
                    "\n\nVerifica que la impresora virtual 'Microsoft Print to PDF' esté instalada en este equipo.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region Vista previa en pantalla (PrintPreviewControl)

        private void MostrarVistaPrevia(Mdl_DocentesTemporal docente)
        {
            var cursos = Ctrl_DocentesTemporal_Cursos.SelectByTeacherTempId(docente.TeacherTempId);
            if (cursos.Count == 0)
            {
                LimpiarPreview();
                return;
            }

            _docenteActual = docente;
            _cursosActual = cursos;
            _lineas = null;
            _lineaActualIndex = 0;
            _totalPaginasGeneradas = 0;

            _printDocument = new PrintDocument();
            _printDocument.DefaultPageSettings.PaperSize = new PaperSize("Letter", 850, 1100);
            _printDocument.PrintPage += PrintDocument_PrintPage;

            PreviewContratos.Document = _printDocument;
            PreviewContratos.StartPage = 0;
            PreviewContratos.InvalidatePreview();

            Lbl_Paginas.Text = $"{docente.FirstName} {docente.LastName}  ({docente.ContractCode})";
            ActualizarEtiquetaPagina();
        }

        private void LimpiarPreview()
        {
            PreviewContratos.Document = null;
            Lbl_Paginas.Text = "";
            _totalPaginasGeneradas = 0;
            ActualizarEtiquetaPagina();
        }

        // ================= Barra de herramientas de vista previa (creada por código) =================
        private Panel _panelHerramientasPreview;
        private ComboBox _cmbZoom;
        private Label _lblPaginaActual;
        private Button _btnPaginaAnterior;
        private Button _btnPaginaSiguiente;
        private Button _btnImprimir;
        private int _totalPaginasGeneradas;

        private void ConfigurarBarraHerramientasPreview()
        {
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 34,
                BackColor = Color.FromArgb(235, 235, 235),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(6, 4, 6, 4)
            };
            _panelHerramientasPreview = flow;

            var lblZoom = new Label { Text = "ZOOM:", AutoSize = true, Margin = new Padding(2, 6, 4, 0), Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

            _cmbZoom = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 90, Margin = new Padding(0, 2, 16, 0) };
            _cmbZoom.Items.AddRange(new object[] { "50%", "75%", "100%", "125%", "150%", "200%" });
            _cmbZoom.SelectedIndexChanged += (s, e) =>
            {
                if (int.TryParse(_cmbZoom.SelectedItem.ToString().Replace("%", ""), out int porcentaje))
                    PreviewContratos.Zoom = porcentaje / 100.0;
            };
            _cmbZoom.SelectedItem = "100%";

            _btnPaginaAnterior = new Button { Text = "◀ ANTERIOR", Width = 100, Height = 25, Margin = new Padding(0, 0, 8, 0) };
            _btnPaginaAnterior.Click += (s, e) =>
            {
                if (PreviewContratos.StartPage > 0)
                {
                    PreviewContratos.StartPage--;
                    ActualizarEtiquetaPagina();
                }
            };

            _lblPaginaActual = new Label
            {
                Text = "SIN DOCUMENTO CARGADO",
                AutoSize = false,
                Width = 220,
                Height = 25,
                Margin = new Padding(4, 4, 16, 0),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _btnPaginaSiguiente = new Button { Text = "SIGUIENTE ▶", Width = 100, Height = 25, Margin = new Padding(0, 0, 16, 0) };
            _btnPaginaSiguiente.Click += (s, e) =>
            {
                if (PreviewContratos.StartPage < _totalPaginasGeneradas - 1)
                {
                    PreviewContratos.StartPage++;
                    ActualizarEtiquetaPagina();
                }
            };

            _btnImprimir = new Button { Text = "🖶 IMPRIMIR", Width = 110, Height = 25, Margin = new Padding(0) };
            _btnImprimir.Click += (s, e) => ImprimirConDialogo();

            flow.Controls.Add(lblZoom);
            flow.Controls.Add(_cmbZoom);
            flow.Controls.Add(_btnPaginaAnterior);
            flow.Controls.Add(_lblPaginaActual);
            flow.Controls.Add(_btnPaginaSiguiente);
            flow.Controls.Add(_btnImprimir);

            PanelContenedor.Controls.Add(_panelHerramientasPreview);
            _panelHerramientasPreview.BringToFront();

            // Fix: PrintPreviewControl solo responde a la rueda del mouse cuando tiene el foco --
            // por defecto solo lo obtiene si se hace clic en la barra de desplazamiento. Esto hace
            // que el foco se mueva automáticamente con solo pasar el mouse por encima.
            PreviewContratos.MouseEnter += (s, e) => PreviewContratos.Focus();

            // Ctrl + rueda del mouse -> zoom (además del scroll normal sin Ctrl)
            PreviewContratos.MouseWheel += (s, e) =>
            {
                if (ModifierKeys.HasFlag(Keys.Control))
                    AjustarZoom(e.Delta > 0 ? 0.1 : -0.1);
            };

            // Ctrl + / Ctrl - -> zoom
            PreviewContratos.KeyDown += (s, e) =>
            {
                if (e.Control && (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add))
                {
                    AjustarZoom(0.1);
                    e.Handled = true;
                }
                else if (e.Control && (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract))
                {
                    AjustarZoom(-0.1);
                    e.Handled = true;
                }
            };
        }

        private void AjustarZoom(double delta)
        {
            double nuevoZoom = Math.Max(0.25, Math.Min(4.0, PreviewContratos.Zoom + delta));
            PreviewContratos.Zoom = nuevoZoom;
        }

        private void ActualizarEtiquetaPagina()
        {
            if (_lblPaginaActual == null) return;
            if (_totalPaginasGeneradas == 0)
            {
                _lblPaginaActual.Text = "SIN DOCUMENTO CARGADO";
                return;
            }
            _lblPaginaActual.Text = $"PÁGINA {PreviewContratos.StartPage + 1} DE {_totalPaginasGeneradas}";
        }

        private void ImprimirConDialogo()
        {
            if (_printDocument == null)
            {
                MessageBox.Show("NO HAY UN CONTRATO GENERADO PARA IMPRIMIR.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (PrintDialog printDialog = new PrintDialog { Document = _printDocument, UseEXDialog = true, AllowPrintToFile = true })
                {
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        _printDocument.PrinterSettings = printDialog.PrinterSettings;
                        _printDocument.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL IMPRIMIR: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Motor de impresión (GDI+ puro, multi-página)

        private const float MargenIzquierdo = 60, MargenDerecho = 60, MargenSuperior = 50, MargenInferior = 50;

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            float anchoDisponible = e.PageBounds.Width - MargenIzquierdo - MargenDerecho;
            float pageBottom = e.PageBounds.Height - MargenInferior;

            // Construir todas las líneas UNA sola vez (en la primera página del trabajo de impresión),
            // usando el mismo Graphics del trabajo para que las medidas sean consistentes en todas las páginas.
            if (_lineas == null)
                _lineas = ConstruirLineas(g, anchoDisponible, _docenteActual, _cursosActual);

            float y = MargenSuperior;

            while (_lineaActualIndex < _lineas.Count)
            {
                var linea = _lineas[_lineaActualIndex];

                // El bloque de firmas no debe partirse entre páginas
                if (y + linea.Altura > pageBottom)
                    break;

                DibujarLinea(g, linea, MargenIzquierdo, y, anchoDisponible);
                y += linea.Altura;
                _lineaActualIndex++;
            }

            _totalPaginasGeneradas++;

            e.HasMorePages = _lineaActualIndex < _lineas.Count;

            // PrintPreviewControl renderiza en un hilo aparte -- el total de páginas solo se conoce
            // con certeza al llegar a la última. Se marshalea el refresco del label al hilo de la UI
            // para que ya no se quede mostrando "SIN DOCUMENTO CARGADO" después de terminar.
            if (!e.HasMorePages && !this.IsDisposed)
            {
                try { this.BeginInvoke(new Action(ActualizarEtiquetaPagina)); } catch { }
            }
        }

        private List<LineaImpresion> ConstruirLineas(Graphics g, float anchoDisponible,
            Mdl_DocentesTemporal docente, List<Mdl_DocentesTemporal_Cursos> cursos)
        {
            var lineas = new List<LineaImpresion>();
            Font fTitulo = new Font("Arial", 12F, FontStyle.Bold);
            Font fSubtitulo = new Font("Arial", 12F, FontStyle.Bold);
            Font fNormal = new Font("Arial", 12F, FontStyle.Regular);
            Font fClausula = new Font("Arial", 12F, FontStyle.Bold);

            void AgregarCentrado(string texto, Font f)
            {
                lineas.Add(new LineaImpresion { Tipo = TipoLinea.TituloCentrado, Texto = texto, Altura = f.Height + 4 });
            }

            void AgregarSubtitulo(string texto)
            {
                lineas.Add(new LineaImpresion { Tipo = TipoLinea.Subtitulo, Texto = texto, Altura = fSubtitulo.Height + 6 });
            }

            void AgregarEspaciador(float alto = 10)
            {
                lineas.Add(new LineaImpresion { Tipo = TipoLinea.Espaciador, Altura = alto });
            }

            // ---------- Encabezado (logo + código de contrato) ----------
            lineas.Add(new LineaImpresion { Tipo = TipoLinea.Encabezado, Texto = docente.ContractCode, Altura = 84 });
            AgregarEspaciador(6);

            AgregarCentrado(TituloContrato1, fTitulo);
            AgregarCentrado(TituloContrato2, fTitulo);
            AgregarCentrado(TituloUniversidad, fTitulo);
            AgregarEspaciador(14);

            AgregarSubtitulo("I.    Datos generales de la Universidad Regional de Guatemala");
            var lineasRector = EnvolverTexto(g, TextoRector, fNormal, anchoDisponible - 20);
            lineas.Add(new LineaImpresion
            {
                Tipo = TipoLinea.CajaTexto,
                LineasEnvueltas = lineasRector,
                Altura = lineasRector.Count * (fNormal.Height + 2) + 16
            });
            AgregarEspaciador(14);

            // ---------- Tabla de datos del profesional (filas individuales, bordes unidos) ----------
            AgregarSubtitulo("II.   Datos generales de Profesional contratado(a):");

            int edad = CalcularEdad(docente.BirthDate, docente.IssueDate ?? DateTime.Now);

            void Fila(string e1, string v1, string e2, string v2)
            {
                lineas.Add(new LineaImpresion
                {
                    Tipo = TipoLinea.TablaFila,
                    Fila = new FilaTabla { Etiqueta1 = e1, Valor1 = v1, Etiqueta2 = e2, Valor2 = v2, EsPrimeraFila = lineas.Count == 0 || lineas[lineas.Count - 1].Tipo != TipoLinea.TablaFila },
                    Altura = 26
                });
            }

            Fila("NOMBRES:", docente.FirstName, "APELLIDOS:", docente.LastName);
            Fila("EDAD:", edad + " AÑOS", "ESTADO CIVIL:", docente.MaritalStatus);
            Fila("SEXO:", docente.Gender, "DPI (CUI):", docente.DPI);
            Fila("DOMICILIO:", docente.Address, "NACIONALIDAD:", docente.Nationality);
            Fila("NÚMERO DE COLEGIADO ACTIVO:", docente.CollegiateNumber, "NIT:", docente.NIT);

            foreach (var curso in cursos)
            {
                Fila("SEDE ACADÉMICA:", curso.AcademicLocation, "AÑO:", docente.ContractYear?.ToString());
                Fila("CURSO A IMPARTIR:", curso.CourseToTeach, "HORARIO:", curso.Schedule);
                Fila("HONORARIOS:", "Q. " + curso.Fees?.ToString("N2"), null, null);
            }

            AgregarEspaciador(14);

            // ---------- Cláusulas legales: UN SOLO flujo continuo (sin párrafos separados) ----------
            // Solo el ordinal + dos puntos (ej. "PRIMERA:") va en negrita, insertado en línea dentro
            // del mismo flujo de texto -- exactamente como viene en el documento original.
            var palabrasClausulas = new List<(string Texto, bool Negrita)>();

            foreach (var palabra in ClausulaIntro.Split(' '))
                palabrasClausulas.Add((palabra, false));

            foreach (var clausula in Clausulas)
            {
                palabrasClausulas.Add((clausula.Ordinal + ":", true));
                foreach (var palabra in clausula.Cuerpo.Split(' '))
                    palabrasClausulas.Add((palabra, false));
            }

            foreach (var lineaMixta in EnvolverMixto(g, palabrasClausulas, fNormal, fClausula, anchoDisponible))
                lineas.Add(new LineaImpresion { Tipo = TipoLinea.TextoMixto, Palabras = lineaMixta, Altura = fNormal.Height + 2 });

            AgregarEspaciador(130);

            // ---------- Firmas (bloque final, no se parte entre páginas) ----------
            lineas.Add(new LineaImpresion
            {
                Tipo = TipoLinea.Firmas,
                Texto = (docente.FirstName + " " + docente.LastName).Trim(),
                Altura = 150
            });

            return lineas;
        }

        // Envuelve una lista de palabras (con su propia bandera de negrita) en líneas que quepan en
        // el ancho disponible, midiendo cada palabra con la fuente que le corresponda (normal o negrita).
        private List<List<(string Texto, bool Negrita)>> EnvolverMixto(
            Graphics g, List<(string Texto, bool Negrita)> palabras, Font fNormal, Font fNegrita, float anchoMax)
        {
            var lineasResultado = new List<List<(string, bool)>>();
            var lineaActual = new List<(string, bool)>();
            float anchoActual = 0;
            float anchoEspacio = g.MeasureString(" ", fNormal).Width;

            foreach (var (texto, negrita) in palabras)
            {
                if (string.IsNullOrEmpty(texto)) continue;
                float anchoPalabra = g.MeasureString(texto, negrita ? fNegrita : fNormal).Width;

                if (lineaActual.Count > 0 && anchoActual + anchoEspacio + anchoPalabra > anchoMax)
                {
                    lineasResultado.Add(lineaActual);
                    lineaActual = new List<(string, bool)>();
                    anchoActual = 0;
                }

                lineaActual.Add((texto, negrita));
                anchoActual += (lineaActual.Count > 1 ? anchoEspacio : 0) + anchoPalabra;
            }

            if (lineaActual.Count > 0) lineasResultado.Add(lineaActual);
            return lineasResultado;
        }

        private void DibujarLinea(Graphics g, LineaImpresion linea, float x, float y, float anchoDisponible)
        {
            Font fTitulo = new Font("Arial", 12F, FontStyle.Bold);
            Font fSubtitulo = new Font("Arial", 12F, FontStyle.Bold);
            Font fNormal = new Font("Arial", 12F, FontStyle.Regular);
            Font fClausula = new Font("Arial", 12F, FontStyle.Bold);
            Brush brush = Brushes.Black;

            switch (linea.Tipo)
            {
                case TipoLinea.TituloCentrado:
                    var formatoCentrado = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString(linea.Texto, fTitulo, brush, new RectangleF(x, y, anchoDisponible, linea.Altura), formatoCentrado);
                    break;

                case TipoLinea.Subtitulo:
                    g.DrawString(linea.Texto, fSubtitulo, brush, x, y);
                    break;

                case TipoLinea.TextoNormal:
                    g.DrawString(linea.Texto, fNormal, brush, x, y);
                    break;

                case TipoLinea.TextoMixto:
                    float xPalabra = x;
                    float anchoEspacioDibujo = g.MeasureString(" ", fNormal).Width;
                    bool primeraPalabra = true;
                    foreach (var (texto, negrita) in linea.Palabras)
                    {
                        Font fPalabra = negrita ? fClausula : fNormal;
                        if (!primeraPalabra) xPalabra += anchoEspacioDibujo;
                        g.DrawString(texto, fPalabra, brush, xPalabra, y);
                        xPalabra += g.MeasureString(texto, fPalabra).Width;
                        primeraPalabra = false;
                    }
                    break;

                case TipoLinea.ClausulaOrdinal:
                    g.DrawString(linea.Texto, fClausula, brush, x, y);
                    break;

                case TipoLinea.TablaFila:
                    float mitadFila = anchoDisponible / 2;

                    // Bordes verticales (izquierda, divisor central, derecha)
                    g.DrawLine(Pens.Black, x, y, x, y + linea.Altura);
                    g.DrawLine(Pens.Black, x + mitadFila, y, x + mitadFila, y + linea.Altura);
                    g.DrawLine(Pens.Black, x + anchoDisponible, y, x + anchoDisponible, y + linea.Altura);

                    // Borde superior solo en la primera fila de la tabla (evita líneas duplicadas entre filas)
                    if (linea.Fila.EsPrimeraFila)
                        g.DrawLine(Pens.Black, x, y, x + anchoDisponible, y);

                    // Borde inferior siempre (sirve de divisor con la fila siguiente, o cierre si es la última)
                    g.DrawLine(Pens.Black, x, y + linea.Altura, x + anchoDisponible, y + linea.Altura);

                    g.DrawString(linea.Fila.Etiqueta1 + " " + (linea.Fila.Valor1 ?? ""), fNormal, brush,
                        new RectangleF(x + 4, y + 4, mitadFila - 8, linea.Altura - 8));

                    if (!string.IsNullOrEmpty(linea.Fila.Etiqueta2))
                    {
                        g.DrawString(linea.Fila.Etiqueta2 + " " + (linea.Fila.Valor2 ?? ""), fNormal, brush,
                            new RectangleF(x + mitadFila + 4, y + 4, mitadFila - 8, linea.Altura - 8));
                    }
                    break;

                case TipoLinea.Firmas:
                    Font fFirma = new Font("Arial", 12F, FontStyle.Bold);
                    float yLineaFirma = y + 50;

                    // Tres columnas bien separadas: Universidad / Profesional / Huella
                    float col1 = x;
                    float col2 = x + anchoDisponible * 0.36f;
                    float col3 = x + anchoDisponible * 0.72f;

                    string lineaFirma = "_______________________";
                    float anchoLineaUniv = g.MeasureString(lineaFirma, fNormal).Width;

                    g.DrawString("f) " + lineaFirma, fNormal, brush, col1, yLineaFirma);
                    g.DrawString("f) " + lineaFirma, fNormal, brush, col2, yLineaFirma);

                    // "La Universidad" centrado bajo su línea
                    string textoUniv = "La Universidad";
                    float anchoTextoUniv = g.MeasureString(textoUniv, fFirma).Width;
                    g.DrawString(textoUniv, fFirma, brush, col1 + 20 + (anchoLineaUniv - anchoTextoUniv) / 2, yLineaFirma + 22);

                    // Nombre del docente, pegado a la izquierda de su línea de firma
                    g.DrawString(linea.Texto, fFirma, brush, col2 + 2, yLineaFirma + 22);

                    // Recuadro de huella: su borde inferior queda a la altura de la línea de firma,
                    // y su texto se alinea en la misma fila que "La Universidad" y el nombre del docente.
                    float huellaAncho = anchoDisponible - (col3 - x) - 60;
                    float huellaAlto = 130;
                    float huellaTop = yLineaFirma - huellaAlto;
                    g.DrawRectangle(Pens.Black, col3, huellaTop, huellaAncho, huellaAlto);

                    string textoHuella = "Huella pulgar derecho";
                    float anchoTextoHuella = g.MeasureString(textoHuella, fFirma).Width;
                    g.DrawString(textoHuella, fFirma, brush, col3 + (huellaAncho - anchoTextoHuella) / 2, yLineaFirma + 22);
                    break;

                case TipoLinea.Encabezado:
                    // Logo centrado en la parte superior
                    Image logo = SECRON.Properties.Resources.LogoMembretadoEncabezado;
                    int logoAncho = 260, logoAlto = 78;
                    float logoX = x + (anchoDisponible - logoAncho) / 2;
                    g.DrawImage(logo, logoX, y, logoAncho, logoAlto);

                    // Código de contrato a la derecha, sin recuadro (así viene en el original)
                    var formatoDerecha = new StringFormat { Alignment = StringAlignment.Far };
                    Font fCodigo = new Font("Arial", 12F, FontStyle.Bold);
                    var rectCodigo = new RectangleF(x + anchoDisponible - 180, y + 8, 180, 24);
                    g.DrawString(linea.Texto, fCodigo, brush, rectCodigo, formatoDerecha);
                    break;

                case TipoLinea.CajaTexto:
                    float altoCaja = linea.Altura;
                    g.DrawRectangle(Pens.Black, x, y, anchoDisponible, altoCaja);
                    float yTexto = y + 8;
                    foreach (var lineaTexto in linea.LineasEnvueltas)
                    {
                        g.DrawString(lineaTexto, fNormal, brush, x + 10, yTexto);
                        yTexto += fNormal.Height + 2;
                    }
                    break;
            }
        }

        private List<string> EnvolverTexto(Graphics g, string texto, Font font, float anchoMax)
        {
            var lineas = new List<string>();
            var palabras = texto.Split(' ');
            string lineaActual = "";

            foreach (var palabra in palabras)
            {
                string prueba = string.IsNullOrEmpty(lineaActual) ? palabra : lineaActual + " " + palabra;
                if (g.MeasureString(prueba, font).Width > anchoMax && !string.IsNullOrEmpty(lineaActual))
                {
                    lineas.Add(lineaActual);
                    lineaActual = palabra;
                }
                else
                {
                    lineaActual = prueba;
                }
            }
            if (!string.IsNullOrEmpty(lineaActual)) lineas.Add(lineaActual);

            return lineas;
        }

        private int CalcularEdad(DateTime? nacimiento, DateTime referencia)
        {
            if (nacimiento == null) return 0;
            int edad = referencia.Year - nacimiento.Value.Year;
            if (referencia.Date < nacimiento.Value.Date.AddYears(edad)) edad--;
            return edad;
        }

        #endregion
    }
}