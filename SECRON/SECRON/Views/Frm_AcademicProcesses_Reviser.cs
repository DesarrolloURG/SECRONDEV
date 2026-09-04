using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_Reviser : Form
    {
        private const int REGISTROS_POR_PAGINA = 500;

        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_AcademicProcesses_Reviser> _candidatosActuales = new List<Mdl_AcademicProcesses_Reviser>();
        private Dictionary<string, Mdl_AcademicProcesses_Reviser> _seleccionados = new Dictionary<string, Mdl_AcademicProcesses_Reviser>();
        private int _totalRegistros = 0;

        public Frm_AcademicProcesses_Reviser()
        {
            InitializeComponent();

            this.Load -= Frm_AcademicProcesses_Reviser_Load;
            this.Load += Frm_AcademicProcesses_Reviser_Load;
        }

        private void Frm_AcademicProcesses_Reviser_Load(object sender, EventArgs e)
        {
            ConfigurarTabla();
            ConfigurarFiltros();
            ConfigurarEventos();
            CargarCandidatos();
        }

        #region ConfigurarTabla
        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();

            DataGridViewCheckBoxColumn colSeleccion = new DataGridViewCheckBoxColumn
            {
                Name = "Seleccionar",
                HeaderText = "☑",
                Width = 50,
                ReadOnly = false
            };
            Tabla.Columns.Add(colSeleccion);

            Tabla.Columns.Add("PersonId", "ID");
            Tabla.Columns.Add("PersonCode", "CÓDIGO");
            Tabla.Columns.Add("PersonName", "NOMBRE");
            Tabla.Columns.Add("Username", "USUARIO");
            Tabla.Columns.Add("UserFullName", "NOMBRE DE USUARIO");
            Tabla.Columns.Add("EstadoFicha", "ESTADO FICHA");
            Tabla.Columns.Add("EstadoRevisor", "ESTADO REVISOR");

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = true;
            Tabla.ReadOnly = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.AllowUserToAddRows = false;
            Tabla.RowHeadersVisible = false;

            foreach (DataGridViewColumn col in Tabla.Columns)
            {
                if (col.Name != "Seleccionar")
                    col.ReadOnly = true;
            }

            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Tabla.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            Tabla.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Tabla.RowTemplate.Height = 30;
            Tabla.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            Tabla.Columns["PersonId"].Visible = false;

            Tabla.Columns["PersonCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["PersonCode"].FillWeight = 10;
            Tabla.Columns["PersonName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["PersonName"].FillWeight = 25;
            Tabla.Columns["Username"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["Username"].FillWeight = 15;
            Tabla.Columns["UserFullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["UserFullName"].FillWeight = 20;
            Tabla.Columns["EstadoFicha"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["EstadoFicha"].FillWeight = 15;
            Tabla.Columns["EstadoRevisor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Tabla.Columns["EstadoRevisor"].FillWeight = 15;

            Tabla.CellContentClick -= Tabla_CellContentClick;
            Tabla.CellContentClick += Tabla_CellContentClick;
            Tabla.CellValueChanged -= Tabla_CellValueChanged;
            Tabla.CellValueChanged += Tabla_CellValueChanged;
        }

        private void Tabla_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == Tabla.Columns["Seleccionar"].Index)
            {
                Tabla.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void Tabla_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != Tabla.Columns["Seleccionar"].Index)
                return;

            DataGridViewRow fila = Tabla.Rows[e.RowIndex];
            if (fila.Cells["Seleccionar"].ReadOnly)
                return;

            int personId = Convert.ToInt32(fila.Cells["PersonId"].Value);
            string personType = ComboBox_Grupo.SelectedItem?.ToString();
            string clave = ObtenerClave(personType, personId);

            bool marcado = fila.Cells["Seleccionar"].Value != null && (bool)fila.Cells["Seleccionar"].Value;

            if (marcado)
            {
                var candidato = _candidatosActuales.FirstOrDefault(c => c.PersonId == personId && c.PersonType == personType);
                if (candidato != null)
                    _seleccionados[clave] = candidato;
            }
            else
            {
                _seleccionados.Remove(clave);
            }

            ActualizarContadorSeleccionados();
        }

        private string ObtenerClave(string personType, int personId)
        {
            return $"{personType}_{personId}";
        }
        #endregion ConfigurarTabla

        #region ConfigurarFiltros
        private void ConfigurarFiltros()
        {
            ComboBox_Grupo.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Grupo.Items.Clear();
            ComboBox_Grupo.Items.Add("DOCENTE");
            ComboBox_Grupo.Items.Add("TRABAJADOR");
            ComboBox_Grupo.Items.Add("PROVEEDOR");
            ComboBox_Grupo.Items.Add("COORDINADOR");
            ComboBox_Grupo.SelectedIndex = 0;

            Filtro3.DropDownStyle = ComboBoxStyle.DropDownList;
            Filtro3.Items.Clear();
            Filtro3.Items.Add("TODOS");
            Filtro3.Items.Add("ACTIVOS");
            Filtro3.Items.Add("INACTIVOS");
            Filtro3.SelectedIndex = 0;
        }
        #endregion ConfigurarFiltros

        #region ConfigurarEventos
        private void ConfigurarEventos()
        {
            ComboBox_Grupo.SelectedIndexChanged -= Filtro_Cambiado;
            ComboBox_Grupo.SelectedIndexChanged += Filtro_Cambiado;

            Filtro3.SelectedIndexChanged -= Filtro_Cambiado;
            Filtro3.SelectedIndexChanged += Filtro_Cambiado;

            Btn_Search.Click -= Btn_Search_Click;
            Btn_Search.Click += Btn_Search_Click;

            Btn_Clear.Click -= Btn_Clear_Click;
            Btn_Clear.Click += Btn_Clear_Click;

            Txt_ValorBuscado.KeyDown -= Txt_ValorBuscado_KeyDown;
            Txt_ValorBuscado.KeyDown += Txt_ValorBuscado_KeyDown;

            Btn_LimpiarSeleccion.Click -= Btn_LimpiarSeleccion_Click;
            Btn_LimpiarSeleccion.Click += Btn_LimpiarSeleccion_Click;

            Btn_Add.Click -= Btn_Add_Click;
            Btn_Add.Click += Btn_Add_Click;

            Btn_Remove.Click -= Btn_Remove_Click;
            Btn_Remove.Click += Btn_Remove_Click;
        }

        private void Filtro_Cambiado(object sender, EventArgs e)
        {
            CargarCandidatos();
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            CargarCandidatos();
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "";
            Filtro3.SelectedIndex = 0;
            CargarCandidatos();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                CargarCandidatos();
            }
        }

        private void Btn_LimpiarSeleccion_Click(object sender, EventArgs e)
        {
            if (_seleccionados.Count == 0)
                return;

            var confirmacion = MessageBox.Show(
                $"TIENE {_seleccionados.Count} PERSONA(S) SELECCIONADA(S). ¿DESEA LIMPIAR LA SELECCIÓN?",
                "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.No)
                return;

            _seleccionados.Clear();
            MostrarCandidatosEnTabla();
        }
        #endregion ConfigurarEventos

        #region CargarDatos
        private void CargarCandidatos()
        {
            try
            {
                string personType = ComboBox_Grupo.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(personType))
                    return;

                string texto = Txt_ValorBuscado.Text.Trim();
                string estado = Filtro3.SelectedItem?.ToString() ?? "TODOS";

                _candidatosActuales = Ctrl_AcademicProcesses_Revisers.BuscarCandidatos(
                    personType, texto, estado, 1, REGISTROS_POR_PAGINA, out _totalRegistros);

                MostrarCandidatosEnTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CARGAR CANDIDATOS: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarCandidatosEnTabla()
        {
            Tabla.Rows.Clear();

            foreach (var candidato in _candidatosActuales)
            {
                string clave = ObtenerClave(candidato.PersonType, candidato.PersonId);
                bool marcado = _seleccionados.ContainsKey(clave);

                int rowIndex = Tabla.Rows.Add(
                    marcado,
                    candidato.PersonId,
                    candidato.PersonCode,
                    candidato.PersonName,
                    candidato.UserId.HasValue ? candidato.Username : "SIN USUARIO",
                    candidato.UserId.HasValue ? candidato.UserFullName : "-",
                    candidato.PersonIsActive ? "ACTIVO" : "INACTIVO",
                    candidato.IsActive ? "REVISOR ACTIVO" : "NO ASIGNADO"
                );

                DataGridViewRow fila = Tabla.Rows[rowIndex];

                if (!candidato.UserId.HasValue)
                {
                    fila.DefaultCellStyle.BackColor = Color.FromArgb(217, 217, 217);
                    fila.DefaultCellStyle.ForeColor = Color.FromArgb(89, 89, 89);
                    fila.Cells["Seleccionar"].ReadOnly = true;
                    fila.Cells["Seleccionar"].Value = false;
                }
                else if (candidato.IsActive)
                {
                    fila.DefaultCellStyle.BackColor = Color.FromArgb(198, 239, 206);
                    fila.DefaultCellStyle.ForeColor = Color.FromArgb(0, 97, 0);
                }
                else
                {
                    fila.DefaultCellStyle.BackColor = Color.FromArgb(255, 199, 206);
                    fila.DefaultCellStyle.ForeColor = Color.FromArgb(156, 0, 6);
                }
            }

            ActualizarContadorSeleccionados();
        }

        private void ActualizarContadorSeleccionados()
        {
            Lbl_Paginas.Text = $"MOSTRANDO {_candidatosActuales.Count} DE {_totalRegistros}    |    SELECCIONADOS: {_seleccionados.Count}";
        }
        #endregion CargarDatos

        #region Acciones
        private void Btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (_seleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UNA PERSONA.", "SECRON",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var candidatos = _seleccionados.Values.ToList();

                var confirmacion = MessageBox.Show(
                    $"¿DESEA ASIGNAR COMO REVISOR A {candidatos.Count} PERSONA(S) SELECCIONADA(S)?",
                    "CONFIRMAR ASIGNACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                int exitosos = 0, yaEranRevisor = 0, errores = 0;

                foreach (var candidato in candidatos)
                {
                    if (!candidato.UserId.HasValue)
                    {
                        errores++;
                        continue;
                    }

                    int resultado = Ctrl_AcademicProcesses_Revisers.AsignarRevisor(
                        candidato.UserId.Value, candidato.PersonType, candidato.PersonId, UserData.UserId);

                    if (resultado == Ctrl_AcademicProcesses_Revisers.RESULTADO_YA_ES_REVISOR_ACTIVO)
                        yaEranRevisor++;
                    else if (resultado == Ctrl_AcademicProcesses_Revisers.RESULTADO_ERROR)
                        errores++;
                    else
                        exitosos++;
                }

                _seleccionados.Clear();

                MessageBox.Show(
                    $"REVISORES ASIGNADOS: {exitosos}\nYA ERAN REVISOR ACTIVO: {yaEranRevisor}\nERRORES: {errores}",
                    "RESULTADO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarCandidatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL AGREGAR REVISORES: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            try
            {
                if (_seleccionados.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UNA PERSONA.", "SECRON",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var candidatos = _seleccionados.Values.Where(c => c.IsActive && c.ReviserId > 0).ToList();

                if (candidatos.Count == 0)
                {
                    MessageBox.Show("NINGUNA DE LAS PERSONAS SELECCIONADAS ES REVISOR ACTIVO ACTUALMENTE.",
                        "SECRON", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿DESEA REMOVER EL ROL DE REVISOR A {candidatos.Count} PERSONA(S) SELECCIONADA(S)?",
                    "CONFIRMAR REMOCIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                int exitosos = 0, errores = 0;

                foreach (var candidato in candidatos)
                {
                    int resultado = Ctrl_AcademicProcesses_Revisers.RemoverRevisor(candidato.ReviserId, UserData.UserId);

                    if (resultado > 0)
                        exitosos++;
                    else
                        errores++;
                }

                _seleccionados.Clear();

                MessageBox.Show(
                    $"REVISORES REMOVIDOS: {exitosos}\nERRORES: {errores}",
                    "RESULTADO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarCandidatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL REMOVER REVISORES: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Acciones
    }
}