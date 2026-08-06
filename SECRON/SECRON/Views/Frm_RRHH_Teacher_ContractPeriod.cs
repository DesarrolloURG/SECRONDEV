using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_RRHH_Teacher_ContractPeriod : Form
    {
        #region PropiedadesIniciales
        public Mdl_Security_UserInfo UserData { get; set; }

        private List<Mdl_Portal_Contratos_Vigencia> _periodos = new List<Mdl_Portal_Contratos_Vigencia>();

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(700, 650);
            this.MinimumSize = new Size(700, 650);
            this.MaximumSize = new Size(700, 650);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }
        #endregion PropiedadesIniciales

        #region Constructor
        public Frm_RRHH_Teacher_ContractPeriod()
        {
            InitializeComponent();
            ConfigurarTamañoFormulario();

            this.Load += Frm_RRHH_Teacher_ContractPeriod_Load;
            this.Btn_Add.Click += Btn_Add_Click;
            this.Btn_EliminarPeriodo.Click += Btn_EliminarPeriodo_Click;
            this.Btn_ActivarPeriodo.Click += Btn_ActivarPeriodo_Click;
        }

        private void Frm_RRHH_Teacher_ContractPeriod_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarTabla();
                ConfigurarDateTimePickers();
                Tabla.SelectionChanged += Tabla_SelectionChanged;

                CargarPeriodos();
                ActualizarEstadoActivo();
                ActualizarBotonesSeleccion();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("ERROR AL CARGAR FORMULARIO: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Constructor

        #region ConfigurarControles
        private void ConfigurarTabla()
        {
            Tabla.Columns.Clear();
            Tabla.AutoGenerateColumns = false;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToDeleteRows = false;
            Tabla.ReadOnly = true;
            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;

            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VigenciaId",
                HeaderText = "ID",
                Name = "VigenciaId",
                Visible = false
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FechaInicio",
                HeaderText = "DEL",
                Name = "FechaInicio",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FechaFin",
                HeaderText = "AL",
                Name = "FechaFin",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            Tabla.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "Activo",
                HeaderText = "ACTIVO",
                Name = "Activo"
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Observaciones",
                HeaderText = "OBSERVACIONES",
                Name = "Observaciones",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            Tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CreatedDate",
                HeaderText = "CREADO",
                Name = "CreatedDate",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });
        }

        private void ConfigurarDateTimePickers()
        {
            DTP_Inicio.Format = DateTimePickerFormat.Short;
            DTP_Inicio.Value = DateTime.Now;

            DTP_Fin.Format = DateTimePickerFormat.Short;
            DTP_Fin.Value = DateTime.Now;
        }
        #endregion ConfigurarControles

        #region CargarDatos
        private void CargarPeriodos()
        {
            _periodos = Ctrl_Portal_Contratos_Vigencia.Select();
            Tabla.DataSource = null;
            Tabla.DataSource = _periodos;
        }

        private void ActualizarEstadoActivo()
        {
            var vigente = Ctrl_Portal_Contratos_Vigencia.ObtenerVigente();
            if (vigente != null)
            {
                Lbl_Dato.Text = "ESTADO DE PERIODO: ACTIVO";
                Lbl_Dato.ForeColor = Color.Green;
            }
            else
            {
                Lbl_Dato.Text = "ESTADO DE PERIODO: NO ACTIVO";
                Lbl_Dato.ForeColor = Color.Red;
            }
        }
        #endregion CargarDatos

        #region SeleccionTabla
        private Mdl_Portal_Contratos_Vigencia ObtenerPeriodoSeleccionado()
        {
            if (Tabla.CurrentRow == null) return null;
            return Tabla.CurrentRow.DataBoundItem as Mdl_Portal_Contratos_Vigencia;
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBotonesSeleccion();
        }

        private void ActualizarBotonesSeleccion()
        {
            var periodo = ObtenerPeriodoSeleccionado();

            if (periodo == null)
            {
                Btn_EliminarPeriodo.Enabled = false;
                Btn_ActivarPeriodo.Enabled = false;
                return;
            }

            Btn_EliminarPeriodo.Enabled = periodo.Activo;
            Btn_ActivarPeriodo.Enabled = !periodo.Activo;
        }
        #endregion SeleccionTabla

        #region Btn_Add
        private void Btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (DTP_Inicio.Value.Date > DTP_Fin.Value.Date)
                {
                    MessageBox.Show("LA FECHA DE INICIO NO PUEDE SER MAYOR A LA FECHA DE FIN.",
                        "ATENCIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    "AL CREAR ESTE PERIODO SE ACTIVARÁ AUTOMÁTICAMENTE Y SE DESACTIVARÁ CUALQUIER OTRO PERIODO ACTIVO. ¿DESEA CONTINUAR?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                var nuevoPeriodo = new Mdl_Portal_Contratos_Vigencia
                {
                    FechaInicio = DTP_Inicio.Value.Date,
                    FechaFin = DTP_Fin.Value.Date,
                    Activo = true,
                    Observaciones = null,
                    CreatedBy = UserData?.UserId
                };

                int resultado = Ctrl_Portal_Contratos_Vigencia.Insert(nuevoPeriodo);

                if (resultado > 0)
                {
                    CargarPeriodos();
                    ActualizarEstadoActivo();
                    ActualizarBotonesSeleccion();
                    MessageBox.Show("PERIODO CREADO CORRECTAMENTE.",
                        "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("NO SE PUDO CREAR EL PERIODO.",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CREAR PERIODO: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Btn_Add
        #region Btn_EliminarPeriodo
        private void Btn_EliminarPeriodo_Click(object sender, EventArgs e)
        {
            try
            {
                var periodo = ObtenerPeriodoSeleccionado();
                if (periodo == null)
                {
                    MessageBox.Show("SELECCIONE UN PERIODO DE LA TABLA.",
                        "ATENCIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!periodo.Activo)
                {
                    MessageBox.Show("ESTE PERIODO YA ESTÁ INACTIVO.",
                        "ATENCIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    "¿DESEA INACTIVAR ESTE PERIODO?",
                    "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                int resultado = Ctrl_Portal_Contratos_Vigencia.Update(
                    periodo.VigenciaId, 1, null, null, periodo.Observaciones, UserData?.UserId ?? 0);

                if (resultado > 0)
                {
                    CargarPeriodos();
                    ActualizarEstadoActivo();
                    ActualizarBotonesSeleccion();
                    MessageBox.Show("PERIODO INACTIVADO CORRECTAMENTE.",
                        "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("NO SE PUDO INACTIVAR EL PERIODO.",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL INACTIVAR PERIODO: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Btn_EliminarPeriodo

        #region Btn_ActivarPeriodo
        private void Btn_ActivarPeriodo_Click(object sender, EventArgs e)
        {
            try
            {
                var periodo = ObtenerPeriodoSeleccionado();
                if (periodo == null)
                {
                    MessageBox.Show("SELECCIONE UN PERIODO DE LA TABLA.",
                        "ATENCIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int resultado = Ctrl_Portal_Contratos_Vigencia.Update(
                    periodo.VigenciaId, 2, null, null, periodo.Observaciones, UserData?.UserId ?? 0);

                if (resultado > 0)
                {
                    CargarPeriodos();
                    ActualizarEstadoActivo();
                    ActualizarBotonesSeleccion();
                    MessageBox.Show("PERIODO ACTIVADO CORRECTAMENTE.",
                        "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("NO SE PUDO ACTIVAR EL PERIODO.",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTIVAR PERIODO: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Btn_ActivarPeriodo
    }
}