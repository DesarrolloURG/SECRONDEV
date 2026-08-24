using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_CareerPensumCourses : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        private readonly int? _careerPensumId;
        private readonly int _careerId;
        private readonly string _pensumCode;
        private readonly string _pensumName;
        private readonly bool _isCurrent;

        private class TreeCourseItem
        {
            public int CourseId;
            public int? CareerCourseId;
            public int Semester;
            public decimal StandardPrice;
            public string CourseCode;
            public string CourseName;
            public List<int> PrerequisiteCourseIds = new List<int>();
        }

        private List<TreeCourseItem> _treeItems = new List<TreeCourseItem>();
        private List<Mdl_Courses> _catalogoCompleto = new List<Mdl_Courses>();
        private int _semestreCount = 0;
        private int _semestreSeleccionado = 1;
        private decimal _precioSugerido = 0;

        private DataGridView _gridCatalogo;
        private TextBox _txtBuscarCatalogo;
        private ComboBox _cbxSemestre;
        private Button _btnAgregarSemestre;
        private FlowLayoutPanel _panelCursosSemestre;
        private Panel _panelDetalle;

        #endregion

        public Frm_AcademicProcesses_CareerPensumCourses(
            int? careerPensumId, int careerId, string pensumCode, string pensumName, bool isCurrent)
        {
            InitializeComponent();

            _careerPensumId = careerPensumId;
            _careerId = careerId;
            _pensumCode = pensumCode;
            _pensumName = pensumName;
            _isCurrent = isCurrent;
        }

        private async void Frm_AcademicProcesses_CareerPensumCourses_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.AutoScroll = true;

                Lbl_Formulario.Text = $"CONFIGURACIÓN PENSUM DE ESTUDIOS — {_pensumCode} — {_pensumName}";

                string precioTexto = await Ctrl_Parameters.ObtenerValorStringAsync("PrecioSugeridoCursosPensum", "0");
                _precioSugerido = decimal.TryParse(precioTexto, out decimal p) ? p : 0;

                ConstruirLayoutBase();
                CargarCatalogo();

                if (_careerPensumId.HasValue)
                    CargarArbolExistente(_careerPensumId.Value);
                else
                    AgregarSemestre(1);

                RefrescarCatalogoDisponible();
                RefrescarSemestreActual();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("Error al cargar el formulario: " + ex.Message,
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Construcción de layout base (3 paneles)

        private void ConstruirLayoutBase()
        {
            PanelTabla.Controls.Clear();
            PanelTabla.AutoScroll = true;

            var contenedorCatalogo = new Panel { Dock = DockStyle.Left, Width = 400, BackColor = Color.FromArgb(240, 241, 242) };
            var lblCatalogo = new Label
            {
                Text = "CATÁLOGO DE CURSOS",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var panelBusquedaCatalogo = new Panel { Dock = DockStyle.Top, Height = 36, Padding = new Padding(4, 4, 4, 0) };

            _txtBuscarCatalogo = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
            _txtBuscarCatalogo.TextChanged += (s, e) => RefrescarCatalogoDisponible();

            panelBusquedaCatalogo.Controls.Add(_txtBuscarCatalogo);

            _gridCatalogo = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ShowCellToolTips = true,
                ScrollBars = ScrollBars.Both
            };
            _gridCatalogo.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _gridCatalogo.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F);

            _gridCatalogo.Columns.Add(new DataGridViewButtonColumn { Name = "Agregar", HeaderText = "", Text = "+", UseColumnTextForButtonValue = true, Width = 35 });
            _gridCatalogo.Columns.Add(new DataGridViewTextBoxColumn { Name = "CourseCode", HeaderText = "CÓDIGO", Width = 70 });
            _gridCatalogo.Columns.Add(new DataGridViewTextBoxColumn { Name = "CourseName", HeaderText = "CURSO", Width = 150 });
            _gridCatalogo.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "DESCRIPCIÓN", Width = 220 });
            _gridCatalogo.Columns.Add(new DataGridViewTextBoxColumn { Name = "Credits", HeaderText = "CRÉD.", Width = 50 });
            _gridCatalogo.Columns.Add(new DataGridViewTextBoxColumn { Name = "TheoryHours", HeaderText = "H. TEÓR.", Width = 65 });
            _gridCatalogo.Columns.Add(new DataGridViewTextBoxColumn { Name = "PracticeHours", HeaderText = "H. PRÁCT.", Width = 70 });
            _gridCatalogo.Columns.Add(new DataGridViewTextBoxColumn { Name = "LabHours", HeaderText = "H. LAB.", Width = 60 });
            _gridCatalogo.Columns.Add(new DataGridViewTextBoxColumn { Name = "TotalHours", HeaderText = "H. TOTAL", Width = 65 });

            _gridCatalogo.CellContentClick += GridCatalogo_CellContentClick;

            contenedorCatalogo.Controls.Add(_gridCatalogo);
            contenedorCatalogo.Controls.Add(panelBusquedaCatalogo);
            contenedorCatalogo.Controls.Add(lblCatalogo);

            _panelDetalle = new Panel { Dock = DockStyle.Right, Width = 280, BackColor = Color.FromArgb(248, 249, 250), Visible = false };

            var contenedorSemestre = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            var panelSelector = new Panel { Dock = DockStyle.Top, Height = 40 };

            _cbxSemestre = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Left,
                Width = 220,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            _cbxSemestre.SelectedIndexChanged += CbxSemestre_SelectedIndexChanged;

            _btnAgregarSemestre = new Button
            {
                Text = "AGREGAR SEMESTRE",
                Dock = DockStyle.Right,
                Width = 220,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                UseVisualStyleBackColor = true
            };
            _btnAgregarSemestre.Click += (s, e) => AgregarSemestre(_semestreCount + 1);

            panelSelector.Controls.Add(_cbxSemestre);
            panelSelector.Controls.Add(_btnAgregarSemestre);

            var lblVerPensumCompleto = new LinkLabel
            {
                Text = "Ver pensum completo",
                Dock = DockStyle.Top,
                Height = 22,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9F, FontStyle.Underline)
            };
            lblVerPensumCompleto.LinkClicked += (s, e) => MostrarPensumCompleto();

            var contenedorCursosSemestre = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
            var lblCursosSemestre = new Label
            {
                Text = "CURSOS DEL SEMESTRE",
                Dock = DockStyle.Top,
                Height = 26,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Padding = new Padding(6, 4, 0, 0)
            };

            _panelCursosSemestre = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(6)
            };

            contenedorCursosSemestre.Controls.Add(_panelCursosSemestre);
            contenedorCursosSemestre.Controls.Add(lblCursosSemestre);

            contenedorSemestre.Controls.Add(contenedorCursosSemestre);
            contenedorSemestre.Controls.Add(panelSelector);
            contenedorSemestre.Controls.Add(lblVerPensumCompleto);

            PanelTabla.Controls.Add(contenedorSemestre);
            PanelTabla.Controls.Add(_panelDetalle);
            PanelTabla.Controls.Add(contenedorCatalogo);
        }

        #endregion

        #region Catálogo de cursos (izquierda)

        private void CargarCatalogo()
        {
            _catalogoCompleto = Ctrl_Courses.ObtenerCursosParaExportar("TODOS", null, "ACTIVA", "TODOS");
        }

        private void RefrescarCatalogoDisponible()
        {
            _gridCatalogo.Rows.Clear();

            var idsEnArbol = _treeItems.Select(t => t.CourseId).ToHashSet();
            string texto = (_txtBuscarCatalogo?.Text ?? "").Trim().ToUpper();

            var disponibles = _catalogoCompleto
                .Where(c => !idsEnArbol.Contains(c.CourseId))
                .Where(c => string.IsNullOrEmpty(texto) ||
                            (c.CourseName ?? "").ToUpper().Contains(texto) ||
                            (c.CourseCode ?? "").ToUpper().Contains(texto))
                .OrderBy(c => c.CourseName);

            foreach (var curso in disponibles)
            {
                int rowIndex = _gridCatalogo.Rows.Add(
                    "+",
                    curso.CourseCode,
                    curso.CourseName,
                    curso.Description,
                    curso.Credits,
                    curso.TheoryHours?.ToString() ?? "N/A",
                    curso.PracticeHours?.ToString() ?? "N/A",
                    curso.LabHours?.ToString() ?? "N/A",
                    curso.TotalHours?.ToString() ?? "N/A"
                );
                _gridCatalogo.Rows[rowIndex].Tag = curso;
            }
        }

        private void GridCatalogo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (_gridCatalogo.Columns[e.ColumnIndex].Name != "Agregar") return;

            var curso = _gridCatalogo.Rows[e.RowIndex].Tag as Mdl_Courses;
            if (curso == null) return;

            AgregarCursoAlSemestreSeleccionado(curso);
        }

        #endregion

        #region Semestre único (centro)

        private void AgregarSemestre(int numeroSemestre)
        {
            if (numeroSemestre > _semestreCount)
            {
                _semestreCount = numeroSemestre;
                _cbxSemestre.Items.Add($"SEMESTRE {numeroSemestre}");
            }

            _cbxSemestre.SelectedIndex = numeroSemestre - 1;
        }

        private void CbxSemestre_SelectedIndexChanged(object sender, EventArgs e)
        {
            _semestreSeleccionado = _cbxSemestre.SelectedIndex + 1;
            OcultarDetalle();
            RefrescarSemestreActual();
        }

        private void RefrescarSemestreActual()
        {
            _panelCursosSemestre.Controls.Clear();

            var cursosDelSemestre = _treeItems.Where(t => t.Semester == _semestreSeleccionado).OrderBy(t => t.CourseName);

            if (!cursosDelSemestre.Any())
            {
                _panelCursosSemestre.Controls.Add(new Label
                {
                    Text = "No hay cursos asignados a este semestre.",
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(6)
                });
                return;
            }

            foreach (var item in cursosDelSemestre)
                _panelCursosSemestre.Controls.Add(CrearChipCurso(item));
        }

        private Panel CrearChipCurso(TreeCourseItem item)
        {
            var chip = new Panel { Width = 260, Height = 40, Margin = new Padding(2), BackColor = Color.FromArgb(220, 235, 255) };

            var lbl = new Label
            {
                Text = item.CourseName + (item.PrerequisiteCourseIds.Count > 0 ? "  🔗" : ""),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Cursor = Cursors.Hand,
                Tag = item
            };
            lbl.Click += (s, e) => MostrarDetalle(item);

            chip.Controls.Add(lbl);
            return chip;
        }

        #endregion

        #region Agregar / quitar cursos del árbol

        private void AgregarCursoAlSemestreSeleccionado(Mdl_Courses curso)
        {
            var item = new TreeCourseItem
            {
                CourseId = curso.CourseId,
                CareerCourseId = null,
                Semester = _semestreSeleccionado,
                StandardPrice = _precioSugerido,
                CourseCode = curso.CourseCode,
                CourseName = curso.CourseName
            };

            _treeItems.Add(item);
            RefrescarCatalogoDisponible();
            RefrescarSemestreActual();
            MostrarDetalle(item);
        }

        private void QuitarCursoDelArbol(TreeCourseItem item)
        {
            bool esPrerequisitoDeOtro = _treeItems.Any(t => t != item && t.PrerequisiteCourseIds.Contains(item.CourseId));
            if (esPrerequisitoDeOtro)
            {
                MessageBox.Show("Este curso es prerequisito de otro curso del pensum. Quite esa relación antes de eliminarlo.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _treeItems.Remove(item);
            OcultarDetalle();
            RefrescarCatalogoDisponible();
            RefrescarSemestreActual();
        }

        #endregion

        #region Panel de detalle (derecha): precio + prerequisitos

        private void MostrarDetalle(TreeCourseItem item)
        {
            _panelDetalle.Controls.Clear();
            _panelDetalle.Visible = true;

            var lblTitulo = new Label
            {
                Text = item.CourseName,
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };

            var lblSemestre = new Label
            {
                Text = $"Semestre {item.Semester}",
                Dock = DockStyle.Top,
                Height = 22,
                ForeColor = Color.Gray,
                Padding = new Padding(8, 0, 0, 0)
            };

            var lblPrecio = new Label { Text = "PRECIO ESTÁNDAR", Dock = DockStyle.Top, Height = 24, Padding = new Padding(8, 8, 0, 0) };
            var txtPrecio = new TextBox { Dock = DockStyle.Top, Text = item.StandardPrice.ToString("0.00"), Margin = new Padding(8, 0, 8, 8) };
            txtPrecio.KeyPress += (s, e) =>
            {
                if (char.IsControl(e.KeyChar)) return;
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.') { e.Handled = true; return; }
                if (e.KeyChar == '.' && txtPrecio.Text.Contains(".")) e.Handled = true;
            };
            txtPrecio.Leave += (s, e) =>
            {
                if (decimal.TryParse(txtPrecio.Text, out decimal p) && p >= 0)
                    item.StandardPrice = p;
                else
                    txtPrecio.Text = item.StandardPrice.ToString("0.00");
            };

            var lblPrereq = new Label { Text = "PREREQUISITOS (semestres anteriores)", Dock = DockStyle.Top, Height = 24, Padding = new Padding(8, 8, 0, 0) };

            var panelPrereq = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(8, 0, 0, 0)
            };

            var candidatos = _treeItems.Where(t => t != item && t.Semester < item.Semester).OrderBy(t => t.Semester).ThenBy(t => t.CourseName);

            if (!candidatos.Any())
            {
                panelPrereq.Controls.Add(new Label { Text = "No hay cursos en semestres anteriores.", ForeColor = Color.Gray, AutoSize = true });
            }
            else
            {
                foreach (var candidato in candidatos)
                {
                    var chk = new CheckBox
                    {
                        Text = $"{candidato.CourseName} (Sem {candidato.Semester})",
                        Checked = item.PrerequisiteCourseIds.Contains(candidato.CourseId),
                        AutoSize = true
                    };
                    chk.CheckedChanged += (s, e) =>
                    {
                        if (chk.Checked && !item.PrerequisiteCourseIds.Contains(candidato.CourseId))
                            item.PrerequisiteCourseIds.Add(candidato.CourseId);
                        else if (!chk.Checked)
                            item.PrerequisiteCourseIds.Remove(candidato.CourseId);

                        RefrescarSemestreActual();
                    };
                    panelPrereq.Controls.Add(chk);
                }
            }

            var btnQuitar = new Button
            {
                Text = "QUITAR DEL PENSUM",
                Dock = DockStyle.Bottom,
                Height = 40,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                UseVisualStyleBackColor = true
            };
            btnQuitar.Click += (s, e) => QuitarCursoDelArbol(item);

            var btnCerrar = new Button
            {
                Text = "CERRAR",
                Dock = DockStyle.Bottom,
                Height = 36,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                UseVisualStyleBackColor = true
            };
            btnCerrar.Click += (s, e) => OcultarDetalle();

            _panelDetalle.Controls.Add(panelPrereq);
            _panelDetalle.Controls.Add(lblPrereq);
            _panelDetalle.Controls.Add(txtPrecio);
            _panelDetalle.Controls.Add(lblPrecio);
            _panelDetalle.Controls.Add(lblSemestre);
            _panelDetalle.Controls.Add(lblTitulo);
            _panelDetalle.Controls.Add(btnQuitar);
            _panelDetalle.Controls.Add(btnCerrar);
        }

        private void OcultarDetalle()
        {
            _panelDetalle.Controls.Clear();
            _panelDetalle.Visible = false;
        }

        #endregion

        #region Ver pensum completo (resumen modal)

        private void MostrarPensumCompleto()
        {
            using (var frmResumen = new Form())
            {
                frmResumen.Text = $"RESUMEN DEL PENSUM — {_pensumCode} — {_pensumName}";
                frmResumen.Size = new Size(680, 700);
                frmResumen.StartPosition = FormStartPosition.CenterParent;
                frmResumen.FormBorderStyle = FormBorderStyle.FixedDialog;
                frmResumen.MaximizeBox = false;
                frmResumen.MinimizeBox = false;

                var rtb = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    BorderStyle = BorderStyle.None,
                    Font = new Font("Segoe UI", 10.5F),
                    BackColor = Color.White
                };

                var semestresOrdenados = _treeItems.Select(t => t.Semester).Distinct().OrderBy(s => s);

                foreach (var semestre in semestresOrdenados)
                {
                    AgregarTexto(rtb, $"SEMESTRE {semestre}\n", Color.White, Color.FromArgb(51, 140, 255), true, 12F);
                    AgregarTexto(rtb, "\n", Color.Black, Color.White, false, 4F);

                    var cursosDelSemestre = _treeItems.Where(t => t.Semester == semestre).OrderBy(t => t.CourseName);

                    foreach (var item in cursosDelSemestre)
                    {
                        AgregarTexto(rtb, $"    {item.CourseName}", Color.Black, Color.White, true, 11F);
                        AgregarTexto(rtb, $"    Q {item.StandardPrice:0.00}\n", Color.FromArgb(0, 128, 0), Color.White, true, 11F);

                        string prereqTexto;
                        if (item.PrerequisiteCourseIds.Count == 0)
                        {
                            prereqTexto = "Sin prerequisitos";
                        }
                        else
                        {
                            var nombresPrereq = item.PrerequisiteCourseIds
                                .Select(id => _treeItems.FirstOrDefault(t => t.CourseId == id)?.CourseName)
                                .Where(n => n != null);
                            prereqTexto = "Prerequisitos: " + string.Join(", ", nombresPrereq);
                        }

                        AgregarTexto(rtb, $"        {prereqTexto}\n\n", Color.Gray, Color.White, false, 9.5F);
                    }

                    AgregarTexto(rtb, "\n", Color.Black, Color.White, false, 6F);
                }

                var btnCerrarResumen = new Button
                {
                    Text = "CERRAR",
                    Dock = DockStyle.Bottom,
                    Height = 40,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleRight,
                    UseVisualStyleBackColor = true
                };
                btnCerrarResumen.Click += (s, e) => frmResumen.Close();

                frmResumen.Controls.Add(rtb);
                frmResumen.Controls.Add(btnCerrarResumen);

                rtb.SelectionStart = 0;
                rtb.ScrollToCaret();

                frmResumen.ShowDialog(this);
            }
        }

        // Agrega texto formateado al RichTextBox (color de fuente, color de fondo, negrita, tamaño)
        private static void AgregarTexto(RichTextBox rtb, string texto, Color colorFuente, Color colorFondo, bool negrita, float tamaño)
        {
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = colorFuente;
            rtb.SelectionBackColor = colorFondo;
            rtb.SelectionFont = new Font("Segoe UI", tamaño, negrita ? FontStyle.Bold : FontStyle.Regular);
            rtb.AppendText(texto);
        }

        #endregion

        #region Cargar árbol existente (modo edición)

        private void CargarArbolExistente(int careerPensumId)
        {
            var cursosExistentes = Ctrl_CareerCourses.ObtenerCursosActivosPorPensum(careerPensumId);
            var prerequisitos = Ctrl_CareerCoursePrerequisites.ObtenerPrerequisitosPorPensum(careerPensumId);

            int maxSemestre = cursosExistentes.Count > 0 ? cursosExistentes.Max(c => c.Semester) : 1;

            foreach (var curso in cursosExistentes)
            {
                _treeItems.Add(new TreeCourseItem
                {
                    CourseId = curso.CourseId,
                    CareerCourseId = curso.CareerCourseId,
                    Semester = curso.Semester,
                    StandardPrice = curso.StandardPrice,
                    CourseCode = curso.CourseCode,
                    CourseName = curso.CourseName
                });
            }

            foreach (var (courseId, prereqCourseId) in prerequisitos)
            {
                var item = _treeItems.FirstOrDefault(t => t.CourseId == courseId);
                if (item != null && !item.PrerequisiteCourseIds.Contains(prereqCourseId))
                    item.PrerequisiteCourseIds.Add(prereqCourseId);
            }

            for (int s = 1; s <= maxSemestre; s++)
            {
                _semestreCount = s;
                _cbxSemestre.Items.Add($"SEMESTRE {s}");
            }

            _cbxSemestre.SelectedIndex = 0;
        }

        #endregion

        #region Guardar (Btn_Importar = "GUARDAR CAMBIOS")

        private void Btn_Importar_Click(object sender, EventArgs e)
        {
            if (_treeItems.Count == 0)
            {
                MessageBox.Show("Agregue al menos un curso al pensum.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var item in _treeItems)
            {
                foreach (var prereqId in item.PrerequisiteCourseIds)
                {
                    var prereq = _treeItems.FirstOrDefault(t => t.CourseId == prereqId);
                    if (prereq == null || prereq.Semester >= item.Semester)
                    {
                        MessageBox.Show(
                            $"El prerequisito de '{item.CourseName}' no es válido. Debe ser un curso del pensum en un semestre anterior.",
                            "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            var confirmacion = MessageBox.Show(
                "Se guardarán TODOS los cambios del pensum (cursos, semestres y prerequisitos). ¿Desea continuar?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            string json = ConstruirJson();

            this.Cursor = Cursors.WaitCursor;
            int resultado = Ctrl_CareerPensums.GuardarArbolCompleto(json, UserData?.UserId);
            this.Cursor = Cursors.Default;

            if (resultado == -1)
            {
                MessageBox.Show("Ya existe un pensum con ese código para esta carrera.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (resultado == -2)
            {
                MessageBox.Show("Hay un prerequisito inválido (debe pertenecer al pensum y a un semestre anterior).",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (resultado <= 0)
            {
                MessageBox.Show("No se pudo guardar el pensum.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private string ConstruirJson()
        {
            var sbCourses = new System.Text.StringBuilder();
            sbCourses.Append("[");

            for (int i = 0; i < _treeItems.Count; i++)
            {
                var item = _treeItems[i];
                if (i > 0) sbCourses.Append(",");

                sbCourses.Append("{");
                sbCourses.Append("\"careerCourseId\":").Append(item.CareerCourseId.HasValue ? item.CareerCourseId.Value.ToString() : "null").Append(",");
                sbCourses.Append("\"courseId\":").Append(item.CourseId).Append(",");
                sbCourses.Append("\"semester\":").Append(item.Semester).Append(",");
                sbCourses.Append("\"standardPrice\":").Append(item.StandardPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(",");
                sbCourses.Append("\"prerequisiteCourseIds\":[").Append(string.Join(",", item.PrerequisiteCourseIds)).Append("]");
                sbCourses.Append("}");
            }

            sbCourses.Append("]");

            var sb = new System.Text.StringBuilder();
            sb.Append("{");
            sb.Append("\"careerPensumId\":").Append(_careerPensumId.HasValue ? _careerPensumId.Value.ToString() : "null").Append(",");
            sb.Append("\"careerId\":").Append(_careerId).Append(",");
            sb.Append("\"pensumCode\":\"").Append(EscaparJson(_pensumCode)).Append("\",");
            sb.Append("\"pensumName\":\"").Append(EscaparJson(_pensumName)).Append("\",");
            sb.Append("\"isCurrent\":").Append(_isCurrent ? "true" : "false").Append(",");
            sb.Append("\"courses\":").Append(sbCourses);
            sb.Append("}");

            return sb.ToString();
        }

        private static string EscaparJson(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return "";
            return texto
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "")
                .Replace("\n", "\\n");
        }

        #endregion
    }
}