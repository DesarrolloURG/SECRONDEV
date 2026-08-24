using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_SearchCourses : Form
    {
        #region Propiedades

        public Mdl_Security_UserInfo UserData { get; set; }

        private readonly int _careerPensumId;
        private readonly int _careerId;
        private readonly int _locationId;
        private readonly int _modalityId;

        private List<Mdl_PensumCoursePricing> _cursosCompletos = new List<Mdl_PensumCoursePricing>();

        // Precio actual mostrado en el grid por cada curso (CareerCourseId -> precio).
        // Se preserva aunque el usuario filtre/busque, hasta que se guarde o se cierre el formulario.
        private readonly Dictionary<int, decimal> _preciosEnEdicion = new Dictionary<int, decimal>();

        #endregion

        public Frm_AcademicProcesses_SearchCourses(int careerPensumId, int careerId, int locationId, int modalityId)
        {
            InitializeComponent();

            _careerPensumId = careerPensumId;
            _careerId = careerId;
            _locationId = locationId;
            _modalityId = modalityId;

            ConfigurarTamañoFormulario();
        }

        private void Frm_AcademicProcesses_SearchCourses_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                Lbl_Formulario.Text = "PRECIOS DE CURSOS DEL PENSUM";
                ConfigurarPlaceHolder();

                ConfigurarTabla();
                CargarDatos();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("Error al cargar el formulario: " + ex.Message,
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private const string PlaceholderBusqueda = "BUSCAR CURSO...";

        private void ConfigurarPlaceHolder()
        {
            Txt_ValorBuscado.ForeColor = Color.Gray;
            Txt_ValorBuscado.Text = PlaceholderBusqueda;

            Txt_ValorBuscado.GotFocus += (s, e) =>
            {
                if (Txt_ValorBuscado.Text == PlaceholderBusqueda)
                {
                    Txt_ValorBuscado.Text = "";
                    Txt_ValorBuscado.ForeColor = Color.Black;
                }
            };

            Txt_ValorBuscado.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(Txt_ValorBuscado.Text))
                {
                    Txt_ValorBuscado.Text = PlaceholderBusqueda;
                    Txt_ValorBuscado.ForeColor = Color.Gray;
                }
            };
        }

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(800, 750);
            this.MinimumSize = new Size(800, 750);
            this.MaximumSize = new Size(800, 750);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        #region ConfigurarTabla

        private void ConfigurarTabla()
        {
            Tabla.AutoGenerateColumns = false;
            Tabla.Columns.Clear();

            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "Semester", HeaderText = "SEM.", Width = 55, ReadOnly = true });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "CourseCode", HeaderText = "CÓDIGO", Width = 90, ReadOnly = true });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "CourseName", HeaderText = "CURSO", Width = 300, ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrecioCurso", HeaderText = "PRECIO DEL CURSO", Width = 160, ReadOnly = false });

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.RowHeadersVisible = false;

            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            Tabla.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            Tabla.CellValidating += Tabla_CellValidating;
            Tabla.CellEndEdit += Tabla_CellEndEdit;
            Tabla.CellBeginEdit += (s, e) =>
            {
                if (Tabla.Columns[e.ColumnIndex].Name != "PrecioCurso") e.Cancel = true;
            };
            Tabla.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete) e.Handled = true; };
        }

        // Solo permite números con hasta 2 decimales
        private void Tabla_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (Tabla.Columns[e.ColumnIndex].Name != "PrecioCurso") return;

            string valor = e.FormattedValue?.ToString() ?? "";

            if (!System.Text.RegularExpressions.Regex.IsMatch(valor, @"^\d+(\.\d{0,2})?$") ||
                !decimal.TryParse(valor, out decimal precio) || precio < 0)
            {
                MessageBox.Show("Ingrese un precio válido (solo números, máximo 2 decimales, no negativo).",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private void Tabla_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Tabla.Columns[e.ColumnIndex].Name != "PrecioCurso") return;
            if (e.RowIndex < 0) return;

            var fila = Tabla.Rows[e.RowIndex];
            int careerCourseId = Convert.ToInt32(fila.Tag);
            string texto = fila.Cells["PrecioCurso"].Value?.ToString();

            if (decimal.TryParse(texto, out decimal precio))
                _preciosEnEdicion[careerCourseId] = precio;
        }

        #endregion

        #region Carga y filtro (preserva ediciones del usuario)

        private void CargarDatos()
        {
            _cursosCompletos = Ctrl_CourseLocationPricingMaster.ObtenerCursosPensumConPrecio(_careerPensumId, _locationId, _modalityId);

            // Inicializar el diccionario con el precio actual o el sugerido de cada curso,
            // solo si aún no tiene un valor (para no pisar ediciones ya hechas por el usuario)
            foreach (var curso in _cursosCompletos)
            {
                if (!_preciosEnEdicion.ContainsKey(curso.CareerCourseId))
                    _preciosEnEdicion[curso.CareerCourseId] = curso.CurrentPrice ?? curso.StandardPrice;
            }

            RefrescarGrid();
        }

        private void RefrescarGrid()
        {
            string texto = (Txt_ValorBuscado.Text ?? "").Trim().ToUpper();
            if (texto == PlaceholderBusqueda) texto = "";

            var filtrados = string.IsNullOrEmpty(texto)
                ? _cursosCompletos
                : _cursosCompletos.Where(c =>
                    (c.CourseName ?? "").ToUpper().Contains(texto) ||
                    (c.CourseCode ?? "").ToUpper().Contains(texto)).ToList();

            Tabla.Rows.Clear();

            foreach (var curso in filtrados)
            {
                decimal precioMostrado = _preciosEnEdicion.TryGetValue(curso.CareerCourseId, out decimal p)
                    ? p
                    : (curso.CurrentPrice ?? curso.StandardPrice);

                int rowIndex = Tabla.Rows.Add(
                    curso.Semester,
                    curso.CourseCode,
                    curso.CourseName,
                    precioMostrado.ToString("0.00")
                );

                Tabla.Rows[rowIndex].Tag = curso.CareerCourseId;
            }
        }

        #endregion

        #region Búsqueda

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            RefrescarGrid();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = PlaceholderBusqueda;
            Txt_ValorBuscado.ForeColor = Color.Gray;
            RefrescarGrid();
        }

        #endregion

        #region Guardar (única transacción atómica: asignación + todos los precios)

        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            Tabla.EndEdit(); // fuerza el commit de la celda que esté en edición en este momento

            if (_cursosCompletos.Count == 0)
            {
                MessageBox.Show("El pensum no tiene cursos.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                "¿Desea guardar los precios de los cursos del pensum?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            string pricesJson = ConstruirPricesJson();

            this.Cursor = Cursors.WaitCursor;
            int resultado = Ctrl_CourseLocationPricingMaster.GuardarAsignacionYPrecios(
                _locationId, _careerId, _modalityId, pricesJson, UserData?.UserId);
            this.Cursor = Cursors.Default;

            if (resultado <= 0)
            {
                MessageBox.Show("No se pudo guardar la asignación ni los precios. No se realizó ningún cambio.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"Asignación y precios guardados correctamente. Cursos procesados: {resultado}.",
                "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Btn_No_Click(object sender, EventArgs e)
        {
            // Si cancela, nada quedó guardado: ni la asignación ni ningún precio (transacción atómica)
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private string ConstruirPricesJson()
        {
            // El grid visible es la fuente de verdad para lo que el usuario ve/editó en este momento.
            // Construimos un mapa CareerCourseId -> precio a partir de las filas actualmente pintadas.
            var preciosDesdeGrid = new Dictionary<int, decimal>();
            foreach (DataGridViewRow fila in Tabla.Rows)
            {
                if (fila.Tag == null) continue;
                int ccId = Convert.ToInt32(fila.Tag);
                string texto = fila.Cells["PrecioCurso"].Value?.ToString();
                if (decimal.TryParse(texto, out decimal precioFila))
                    preciosDesdeGrid[ccId] = precioFila;
            }

            var sb = new System.Text.StringBuilder();
            sb.Append("[");

            var cursos = _cursosCompletos;
            for (int i = 0; i < cursos.Count; i++)
            {
                var curso = cursos[i];

                // Prioridad: 1) valor visible ahora mismo en el grid, 2) edición previa guardada
                // en el diccionario (curso filtrado fuera de pantalla), 3) precio actual o sugerido.
                decimal precio;
                if (preciosDesdeGrid.TryGetValue(curso.CareerCourseId, out decimal pGrid))
                    precio = pGrid;
                else if (_preciosEnEdicion.TryGetValue(curso.CareerCourseId, out decimal pDict))
                    precio = pDict;
                else
                    precio = curso.CurrentPrice ?? curso.StandardPrice;

                if (i > 0) sb.Append(",");
                sb.Append("{");
                sb.Append("\"careerCourseId\":").Append(curso.CareerCourseId).Append(",");
                sb.Append("\"price\":").Append(precio.ToString(System.Globalization.CultureInfo.InvariantCulture));
                sb.Append("}");
            }

            sb.Append("]");
            return sb.ToString();
        }

        #endregion
    }
}