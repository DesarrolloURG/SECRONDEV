using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_AcademicProcesses_Reviser_Locations : Form
    {
        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_AcademicProcesses_Reviser> _revisoresActivos = new List<Mdl_AcademicProcesses_Reviser>();

        public Frm_AcademicProcesses_Reviser_Locations()
        {
            InitializeComponent();

            this.Size = new Size(1200, 700);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.Load -= Frm_AcademicProcesses_Reviser_Locations_Load;
            this.Load += Frm_AcademicProcesses_Reviser_Locations_Load;
        }

        private void Frm_AcademicProcesses_Reviser_Locations_Load(object sender, EventArgs e)
        {
            ConfigurarTablas();
            ConfigurarEventos();
            CargarRevisoresEnCombo();
        }

        #region ConfigurarTablas
        private void ConfigurarTablas()
        {
            ConfigurarTabla(TablaSedesAsignadas, incluirAsignadoPor: true);
            ConfigurarTabla(TablaListadoSedes, incluirAsignadoPor: false);
        }

        private void ConfigurarTabla(DataGridView tabla, bool incluirAsignadoPor)
        {
            tabla.Columns.Clear();

            if (incluirAsignadoPor)
                tabla.Columns.Add("ReviserLocationId", "ID");

            tabla.Columns.Add("LocationId", "ID SEDE");
            tabla.Columns.Add("LocationCode", "CÓDIGO");
            tabla.Columns.Add("LocationName", "SEDE");

            if (incluirAsignadoPor)
            {
                tabla.Columns.Add("AssignedByName", "ASIGNADO POR");
                tabla.Columns.Add("AssignedDate", "FECHA ASIGNACIÓN");
            }

            foreach (DataGridViewColumn col in tabla.Columns)
                col.ReadOnly = true;

            tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tabla.MultiSelect = true;
            tabla.AllowUserToAddRows = false;
            tabla.RowHeadersVisible = false;

            if (incluirAsignadoPor)
                tabla.Columns["ReviserLocationId"].Visible = false;

            tabla.Columns["LocationId"].Visible = false;

            tabla.Columns["LocationCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tabla.Columns["LocationCode"].Width = 80;
            tabla.Columns["LocationName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            if (incluirAsignadoPor)
            {
                tabla.Columns["AssignedByName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                tabla.Columns["AssignedByName"].Width = 150;
                tabla.Columns["AssignedDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                tabla.Columns["AssignedDate"].Width = 130;
            }
        }
        #endregion ConfigurarTablas

        #region ConfigurarEventos
        private void ConfigurarEventos()
        {
            ComboBox_Asesor.SelectedIndexChanged -= ComboBox_Asesor_SelectedIndexChanged;
            ComboBox_Asesor.SelectedIndexChanged += ComboBox_Asesor_SelectedIndexChanged;

            Btn_Yes.Click -= Btn_Yes_Click;
            Btn_Yes.Click += Btn_Yes_Click;

            Btn_RemoveAsset.Click -= Btn_RemoveAsset_Click;
            Btn_RemoveAsset.Click += Btn_RemoveAsset_Click;
        }

        private void ComboBox_Asesor_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarSedesDelRevisorSeleccionado();
        }
        #endregion ConfigurarEventos

        #region CargarDatos
        private void CargarRevisoresEnCombo()
        {
            try
            {
                _revisoresActivos = Ctrl_AcademicProcesses_Revisers.BuscarRevisores(
                    null, "ACTIVOS", 1, 1000, out int totalRegistros);

                ComboBox_Asesor.DropDownStyle = ComboBoxStyle.DropDownList;
                ComboBox_Asesor.DataSource = null;
                ComboBox_Asesor.DisplayMember = "PersonName";
                ComboBox_Asesor.ValueMember = "ReviserId";
                ComboBox_Asesor.DataSource = _revisoresActivos;

                CargarSedesDelRevisorSeleccionado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CARGAR REVISORES: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarSedesDelRevisorSeleccionado()
        {
            if (ComboBox_Asesor.SelectedValue == null)
                return;

            int reviserId = Convert.ToInt32(ComboBox_Asesor.SelectedValue);

            var asignadas = Ctrl_AcademicProcesses_ReviserLocations.ObtenerSedesAsignadas(reviserId);
            MostrarSedesEnTabla(TablaSedesAsignadas, asignadas, incluirAsignadoPor: true);

            var disponibles = Ctrl_AcademicProcesses_ReviserLocations.BuscarSedesDisponibles(reviserId, null);
            MostrarSedesEnTabla(TablaListadoSedes, disponibles, incluirAsignadoPor: false);
        }

        private void MostrarSedesEnTabla(DataGridView tabla, List<Mdl_AcademicProcesses_ReviserLocation> sedes, bool incluirAsignadoPor)
        {
            tabla.Rows.Clear();

            foreach (var sede in sedes)
            {
                if (incluirAsignadoPor)
                {
                    tabla.Rows.Add(
                        sede.ReviserLocationId,
                        sede.LocationId,
                        sede.LocationCode,
                        sede.LocationName,
                        sede.AssignedByName,
                        sede.AssignedDate
                    );
                }
                else
                {
                    tabla.Rows.Add(
                        sede.LocationId,
                        sede.LocationCode,
                        sede.LocationName
                    );
                }
            }
        }
        #endregion CargarDatos

        #region Acciones
        private void Btn_Yes_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_Asesor.SelectedValue == null)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN ASESOR.", "SECRON",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var filasSeleccionadas = TablaListadoSedes.SelectedRows.Cast<DataGridViewRow>().ToList();

                if (filasSeleccionadas.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UNA SEDE DEL LISTADO.", "SECRON",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿DESEA ASIGNAR {filasSeleccionadas.Count} SEDE(S) AL ASESOR SELECCIONADO?",
                    "CONFIRMAR ASIGNACIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                int reviserId = Convert.ToInt32(ComboBox_Asesor.SelectedValue);
                int exitosos = 0, errores = 0;

                foreach (var fila in filasSeleccionadas)
                {
                    int locationId = Convert.ToInt32(fila.Cells["LocationId"].Value);

                    if (Ctrl_AcademicProcesses_ReviserLocations.AsignarSede(reviserId, locationId, UserData.UserId) > 0)
                        exitosos++;
                    else
                        errores++;
                }

                MessageBox.Show($"SEDES ASIGNADAS: {exitosos}\nERRORES: {errores}", "RESULTADO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarSedesDelRevisorSeleccionado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ASIGNAR SEDES: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_RemoveAsset_Click(object sender, EventArgs e)
        {
            try
            {
                var filasSeleccionadas = TablaSedesAsignadas.SelectedRows.Cast<DataGridViewRow>().ToList();

                if (filasSeleccionadas.Count == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR AL MENOS UNA SEDE ASIGNADA.", "SECRON",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿DESEA REMOVER {filasSeleccionadas.Count} SEDE(S) DEL ASESOR SELECCIONADO?",
                    "CONFIRMAR REMOCIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                int exitosos = 0, errores = 0;

                foreach (var fila in filasSeleccionadas)
                {
                    int reviserLocationId = Convert.ToInt32(fila.Cells["ReviserLocationId"].Value);

                    if (Ctrl_AcademicProcesses_ReviserLocations.RemoverSede(reviserLocationId, UserData.UserId) > 0)
                        exitosos++;
                    else
                        errores++;
                }

                MessageBox.Show($"SEDES REMOVIDAS: {exitosos}\nERRORES: {errores}", "RESULTADO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarSedesDelRevisorSeleccionado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL REMOVER SEDES: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Acciones
    }
}