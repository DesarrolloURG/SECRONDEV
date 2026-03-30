using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Checks_Assignment : Form
    {
        #region PropiedadesIniciales
        private Frm_Checks_Managment _frmPadre;
        private int _rangoSeleccionadoId = 0; // ID del rango seleccionado en Tabla2

        // Constructor
        public Frm_Checks_Assignment(Frm_Checks_Managment formularioPadre)
        {
            InitializeComponent();
            _frmPadre = formularioPadre;
            // Configuración inicial del formulario
            ConfigurarTamañoFormulario();
        }

        // Evento Load del formulario
        private void Frm_Checks_Assignment_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarPlaceHolders();
                ConfigurarComboBox();
                ConfigurarTablas();
                ConfigurarMaxLength();
                ConfigurarBotones();
                CargarUsuariosConPermisos();
                CargarRangosExistentes();
                ConfigurarTabIndexYFocus();
                ConfigurarCheckBox();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"ERROR AL CARGAR FORMULARIO: {ex.Message}",
                              "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Configurar Medidas Formulario
        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(700, 650);           // Tamaño fijo
            this.MinimumSize = new Size(700, 650);    // Tamaño mínimo
            this.MaximumSize = new Size(700, 650);    // Tamaño máximo
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // No redimensionable
            this.StartPosition = FormStartPosition.CenterParent; // Centrado en el padre
            this.MaximizeBox = false;                 // Sin botón maximizar
        }
        #endregion PropiedadesIniciales
        #region ConfiguracionInicial
        private void ConfigurarPlaceHolders()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR USUARIO...");
            ConfigurarPlaceHolder(Txt_Seleccionado, "USUARIO SELECCIONADO");
            ConfigurarPlaceHolder(Txt_Li, "0");
            ConfigurarPlaceHolder(Txt_Fin, "0");
            ConfigurarPlaceHolder(Txt_SiguienteCheque, "0");
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

        private void ConfigurarMaxLength()
        {
            Txt_Li.MaxLength = 10;
            Txt_Fin.MaxLength = 10;
            Txt_SiguienteCheque.MaxLength = 10;
            Txt_Seleccionado.Enabled = false;
        }

        private void ConfigurarComboBox()
        {
            ComboBox_BuscarPor.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_BuscarPor.Items.AddRange(new object[] { "USUARIO", "NOMBRE COMPLETO" });
            ComboBox_BuscarPor.SelectedIndex = 0;
        }

        private void ConfigurarCheckBox()
        {
            CheckBox_Compartido.Checked = true;
            CheckBox_Compartido.CheckedChanged += CheckBox_Compartido_CheckedChanged;
            ActualizarEstadoCompartido();
            CheckBox_AltaPrioridad.Checked = false;
        }

        private void ConfigurarBotones()
        {
            // Estado inicial de botones
            Btn_Save.Enabled = true;
            Btn_Update.Enabled = false;
            Btn_Delete.Enabled = false;
        }
        #endregion ConfiguracionInicial
        #region AsignacionFocus
        private void ConfigurarTabIndexYFocus()
        {
            // Establecer TabIndex
            Txt_ValorBuscado.TabIndex = 0;
            ComboBox_BuscarPor.TabIndex = 1;
            Btn_Search.TabIndex = 2;
            Btn_Clear.TabIndex = 3;
            CheckBox_Compartido.TabIndex = 4;
            Txt_Li.TabIndex = 5;
            Txt_Fin.TabIndex = 6;
            Txt_SiguienteCheque.TabIndex = 7;
            CheckBox_AltaPrioridad.TabIndex = 8;
            Btn_Save.TabIndex = 9;
            Btn_Update.TabIndex = 10;
            Btn_Delete.TabIndex = 11;
            Btn_Clear2.TabIndex = 12;

            // Establecer foco inicial
            Txt_ValorBuscado.Focus();
        }
        #endregion AsignacionFocus
        #region ConfigurarTablas
        private void ConfigurarTablas()
        {
            ConfigurarTabla1();
            ConfigurarTabla2();
        }

        // TABLA 1: USUARIOS CON PERMISOS
        private void ConfigurarTabla1()
        {
            Tabla1.Columns.Clear();
            Tabla1.Rows.Clear(); //   LIMPIAR FILAS TAMBIÉN

            Tabla1.Columns.Add("UserId", "ID");
            Tabla1.Columns.Add("Username", "USUARIO");
            Tabla1.Columns.Add("FullName", "NOMBRE COMPLETO");

            Tabla1.Columns["UserId"].Visible = false;

            // CONFIGURACIÓN CRÍTICA PARA SELECCIÓN
            Tabla1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla1.MultiSelect = false;
            Tabla1.ReadOnly = true;
            Tabla1.AllowUserToResizeRows = false;
            Tabla1.AllowUserToAddRows = false;
            Tabla1.RowHeadersVisible = false;
            Tabla1.Enabled = true; //   ASEGURAR QUE ESTÉ HABILITADO
            Tabla1.TabStop = true; //   PERMITIR TAB

            Tabla1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Tabla1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 140, 255);
            Tabla1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            Tabla1.DefaultCellStyle.SelectionBackColor = Color.Azure;
            Tabla1.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla1.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            Tabla1.RowTemplate.Height = 30;
            Tabla1.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            AjustarColumnasTabla1();

            //   REMOVER EVENTOS ANTERIORES ANTES DE AGREGAR
            Tabla1.SelectionChanged -= Tabla1_SelectionChanged;
            Tabla1.SelectionChanged += Tabla1_SelectionChanged;

            //   AGREGAR EVENTO CLICK ADICIONAL
            Tabla1.CellClick -= Tabla1_CellClick;
            Tabla1.CellClick += Tabla1_CellClick;
        }

        //   NUEVO EVENTO PARA FORZAR SELECCIÓN
        private void Tabla1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !CheckBox_Compartido.Checked)
            {
                Tabla1.Rows[e.RowIndex].Selected = true;
            }
        }

        private void AjustarColumnasTabla1()
        {
            if (Tabla1.Columns.Count > 0)
            {
                Tabla1.Columns["Username"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla1.Columns["Username"].FillWeight = 40;

                Tabla1.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla1.Columns["FullName"].FillWeight = 60;
            }
        }

        private void Tabla1_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla1.SelectedRows.Count > 0 && !CheckBox_Compartido.Checked && Tabla1.Enabled)
            {
                DataGridViewRow row = Tabla1.SelectedRows[0];
                Txt_Seleccionado.Text = row.Cells["Username"].Value?.ToString() ?? "";
                Txt_Seleccionado.ForeColor = Color.Black;

                // Limpiar selección de Tabla2 y preparar para NUEVO
                Tabla2.ClearSelection();
                _rangoSeleccionadoId = 0;
                LimpiarCamposRango();
                ConfigurarBotonesParaNuevo();
            }
        }

        // TABLA 2: RANGOS EXISTENTES EN EL SISTEMA
        private void ConfigurarTabla2()
        {
            Tabla2.Columns.Clear();
            Tabla2.Rows.Clear(); //   LIMPIAR FILAS TAMBIÉN

            Tabla2.Columns.Add("CheckControlId", "ID");
            Tabla2.Columns.Add("UserId", "USER_ID");
            Tabla2.Columns.Add("Usuario", "USUARIO");
            Tabla2.Columns.Add("InitialLimit", "LÍMITE INICIAL");
            Tabla2.Columns.Add("FinalLimit", "LÍMITE FINAL");
            Tabla2.Columns.Add("CurrentCounter", "SIGUIENTE CHEQUE");
            Tabla2.Columns.Add("Priority", "PRIORIDAD");
            Tabla2.Columns["CheckControlId"].Visible = false;
            Tabla2.Columns["UserId"].Visible = false;

            //   CONFIGURACIÓN CRÍTICA PARA SELECCIÓN
            Tabla2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla2.MultiSelect = false;
            Tabla2.ReadOnly = true;
            Tabla2.AllowUserToResizeRows = false;
            Tabla2.AllowUserToAddRows = false;
            Tabla2.RowHeadersVisible = false;
            Tabla2.Enabled = true; //   ASEGURAR QUE ESTÉ HABILITADO
            Tabla2.TabStop = true; //   PERMITIR TAB

            Tabla2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Tabla2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla2.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 143, 109);
            Tabla2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            Tabla2.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            Tabla2.DefaultCellStyle.SelectionForeColor = Color.Black;
            Tabla2.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            Tabla2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Tabla2.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            Tabla2.RowTemplate.Height = 30;
            Tabla2.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            AjustarColumnasTabla2();

            //   REMOVER EVENTOS ANTERIORES ANTES DE AGREGAR
            Tabla2.SelectionChanged -= Tabla2_SelectionChanged;
            Tabla2.SelectionChanged += Tabla2_SelectionChanged;

            //   AGREGAR EVENTO CLICK ADICIONAL
            Tabla2.CellClick -= Tabla2_CellClick;
            Tabla2.CellClick += Tabla2_CellClick;
        }

        //   NUEVO EVENTO PARA FORZAR SELECCIÓN
        private void Tabla2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Tabla2.Rows[e.RowIndex].Selected = true;
            }
        }

        private void AjustarColumnasTabla2()
        {
            if (Tabla2.Columns.Count > 0)
            {
                Tabla2.Columns["Usuario"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["Usuario"].FillWeight = 40;

                Tabla2.Columns["InitialLimit"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["InitialLimit"].FillWeight = 20;

                Tabla2.Columns["FinalLimit"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["FinalLimit"].FillWeight = 20;

                Tabla2.Columns["CurrentCounter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Tabla2.Columns["CurrentCounter"].FillWeight = 20;
            }
        }

        private void Tabla2_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla2.SelectedRows.Count > 0 && Tabla2.Enabled)
            {
                DataGridViewRow row = Tabla2.SelectedRows[0];

                _rangoSeleccionadoId = Convert.ToInt32(row.Cells["CheckControlId"].Value);
                int userId = Convert.ToInt32(row.Cells["UserId"].Value);
                string usuario = row.Cells["Usuario"].Value?.ToString() ?? "";

                // Cargar datos del rango
                Txt_Seleccionado.Text = usuario;
                Txt_Seleccionado.ForeColor = Color.Black;

                Txt_Li.Text = row.Cells["InitialLimit"].Value?.ToString() ?? "0";
                Txt_Li.ForeColor = Color.Black;

                Txt_Fin.Text = row.Cells["FinalLimit"].Value?.ToString() ?? "0";
                Txt_Fin.ForeColor = Color.Black;

                Txt_SiguienteCheque.Text = row.Cells["CurrentCounter"].Value?.ToString() ?? "0";
                Txt_SiguienteCheque.ForeColor = Color.Black;
                // Cargar Priority desde la tabla
                Mdl_CheckControl controlCargado = Ctrl_CheckControl.ObtenerControlPorId(_rangoSeleccionadoId);
                CheckBox_AltaPrioridad.Checked = controlCargado?.Priority ?? false;
                // Actualizar CheckBox
                CheckBox_Compartido.Checked = (usuario == "SYS_CHECKS_SHARE");

                // Limpiar selección de Tabla1
                Tabla1.ClearSelection();

                // Configurar botones para EDITAR/ELIMINAR
                ConfigurarBotonesParaEditar();
            }
        }
        #endregion ConfigurarTablas
        #region CargarDatos
        private void CargarUsuariosConPermisos()
        {
            try
            {
                Tabla1.Rows.Clear();

                // Obtener usuarios con permiso CHECKS_MANAGMENT_CREATE
                List<Mdl_Users> usuarios = Ctrl_Users.ObtenerUsuariosConPermisoEspecifico("CHK_003");

                // Filtrar usuarios del sistema
                usuarios = usuarios.Where(u =>
                    !u.Username.ToUpper().StartsWith("SYS_")).ToList();

                foreach (var usuario in usuarios)
                {
                    Tabla1.Rows.Add(
                        usuario.UserId,
                        usuario.Username,
                        usuario.FullName
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR USUARIOS: {ex.Message}",
                              "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarRangosExistentes()
        {
            try
            {
                Tabla2.Rows.Clear();
                List<Mdl_CheckControl> rangos = Ctrl_CheckControl.MostrarControles();

                //   ORDENAR POR PRIORIDADES
                // 1. Propio con Priority=true
                // 2. Compartido con Priority=true
                // 3. Propio con Priority=false
                // 4. Compartido con Priority=false

                // Obtener UserId del usuario compartido
                Mdl_Users usuarioCompartido = Ctrl_Users.ObtenerUsuarioPorUsername("SYS_CHECKS_SHARE");
                int userIdCompartido = usuarioCompartido?.UserId ?? 0;

                rangos = rangos
                    .OrderByDescending(r => r.UserId != userIdCompartido ? 2 : 0) // Primero propios (2), luego compartidos (0)
                    .ThenByDescending(r => r.Priority) // Luego prioridad alta primero
                    .ToList();

                foreach (var rango in rangos)
                {
                    // Obtener nombre de usuario
                    Mdl_Users usuario = Ctrl_Users.ObtenerUsuarioPorId(rango.UserId);
                    string nombreUsuario = usuario?.Username ?? "DESCONOCIDO";
                    string prioridad = rango.Priority ? "ALTA" : "NORMAL";

                    Tabla2.Rows.Add(
                        rango.CheckControlId,
                        rango.UserId,
                        nombreUsuario,
                        rango.InitialLimit,
                        rango.FinalLimit,
                        rango.CurrentCounter,
                        prioridad
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL CARGAR RANGOS: {ex.Message}",
                              "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion CargarDatos
        #region Busqueda
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                string valorBuscado = ObtenerTextoReal(Txt_ValorBuscado);

                if (string.IsNullOrEmpty(valorBuscado))
                {
                    CargarUsuariosConPermisos();
                    return;
                }

                string criterio = ComboBox_BuscarPor.SelectedItem.ToString();

                foreach (DataGridViewRow row in Tabla1.Rows)
                {
                    bool visible = false;

                    if (criterio == "USUARIO")
                    {
                        string username = row.Cells["Username"].Value?.ToString() ?? "";
                        visible = username.ToUpper().Contains(valorBuscado.ToUpper());
                    }
                    else if (criterio == "NOMBRE COMPLETO")
                    {
                        string fullName = row.Cells["FullName"].Value?.ToString() ?? "";
                        visible = fullName.ToUpper().Contains(valorBuscado.ToUpper());
                    }

                    row.Visible = visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR EN BÚSQUEDA: {ex.Message}",
                              "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "BUSCAR USUARIO...";
            Txt_ValorBuscado.ForeColor = Color.Gray;
            CargarUsuariosConPermisos();
        }

        private string ObtenerTextoReal(TextBox txt)
        {
            if (txt.ForeColor == Color.Gray) return "";
            return txt.Text.Trim();
        }
        #endregion Busqueda
        #region CheckBoxCompartido
        private void CheckBox_Compartido_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarEstadoCompartido();
        }

        private void ActualizarEstadoCompartido()
        {
            if (CheckBox_Compartido.Checked)
            {
                Tabla1.Enabled = false; //   DESHABILITAR TABLA1
                Tabla1.ClearSelection();
                Txt_Seleccionado.Text = "SYS_CHECKS_SHARE";
                Txt_Seleccionado.ForeColor = Color.Black;
            }
            else
            {
                Tabla1.Enabled = true; //   HABILITAR TABLA1
                if (Tabla1.SelectedRows.Count == 0)
                {
                    Txt_Seleccionado.Text = "USUARIO SELECCIONADO";
                    Txt_Seleccionado.ForeColor = Color.Gray;
                }
            }
        }
        #endregion CheckBoxCompartido
        #region BotonesAccion
        // GUARDAR NUEVO RANGO
        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDACIONES
                if (!ValidarDatosCompletos())
                    return;

                int limiteInicial = Convert.ToInt32(ObtenerTextoReal(Txt_Li));
                int limiteFinal = Convert.ToInt32(ObtenerTextoReal(Txt_Fin));
                int siguienteCheque = Convert.ToInt32(ObtenerTextoReal(Txt_SiguienteCheque));

                if (siguienteCheque < limiteInicial || siguienteCheque > limiteFinal)
                {
                    MessageBox.Show("EL SIGUIENTE CHEQUE DEBE ESTAR ENTRE EL LÍMITE INICIAL Y FINAL",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (limiteInicial > limiteFinal)
                {
                    MessageBox.Show("EL LÍMITE INICIAL NO PUEDE SER MAYOR QUE EL LÍMITE FINAL",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtener UserId
                int userId = ObtenerUserIdSeleccionado();
                if (userId == 0)
                {
                    MessageBox.Show("ERROR AL OBTENER USUARIO",
                                  "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //   VERIFICAR TODOS LOS RANGOS DEL USUARIO (NO SOLO UNO)
                List<Mdl_CheckControl> controlesUsuario = Ctrl_CheckControl.MostrarControles()
                    .Where(c => c.UserId == userId).ToList();

                bool nuevoEsPrioritario = CheckBox_AltaPrioridad.Checked;

                if (controlesUsuario.Count > 0)
                {
                    //   VERIFICAR SI YA EXISTE UN RANGO CON PRIORIDAD
                    Mdl_CheckControl controlPrioritarioExistente = controlesUsuario
                        .FirstOrDefault(c => c.Priority == true);

                    Mdl_CheckControl controlNormalExistente = controlesUsuario
                        .FirstOrDefault(c => c.Priority == false);

                    //   CASO 1: Intentando agregar rango prioritario cuando ya existe uno prioritario
                    if (nuevoEsPrioritario && controlPrioritarioExistente != null)
                    {
                        var confirmacion = MessageBox.Show(
                            $"YA EXISTE UN RANGO PRIORITARIO PARA ESTE USUARIO:\n\n" +
                            $"Límite: {controlPrioritarioExistente.InitialLimit} - {controlPrioritarioExistente.FinalLimit}\n" +
                            $"Siguiente Cheque: {controlPrioritarioExistente.CurrentCounter}\n\n" +
                            "SOLO PUEDE HABER UN RANGO PRIORITARIO POR USUARIO.\n" +
                            "¿DESEA SUSTITUIR EL RANGO PRIORITARIO EXISTENTE?",
                            "CONFIRMAR SUSTITUCIÓN",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (confirmacion == DialogResult.No)
                            return;

                        // SUSTITUIR: Eliminar el prioritario anterior
                        if (!Ctrl_CheckControl.EliminarControl(controlPrioritarioExistente.CheckControlId))
                        {
                            MessageBox.Show("ERROR AL ELIMINAR RANGO PRIORITARIO ANTERIOR",
                                          "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    //   CASO 2: Intentando agregar rango normal cuando ya existe uno normal
                    else if (!nuevoEsPrioritario && controlNormalExistente != null)
                    {
                        var confirmacion = MessageBox.Show(
                            $"YA EXISTE UN RANGO NORMAL PARA ESTE USUARIO:\n\n" +
                            $"Límite: {controlNormalExistente.InitialLimit} - {controlNormalExistente.FinalLimit}\n" +
                            $"Siguiente Cheque: {controlNormalExistente.CurrentCounter}\n\n" +
                            "¿DESEA SUSTITUIR EL RANGO NORMAL EXISTENTE?",
                            "CONFIRMAR SUSTITUCIÓN",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (confirmacion == DialogResult.No)
                            return;

                        // SUSTITUIR: Eliminar el normal anterior
                        if (!Ctrl_CheckControl.EliminarControl(controlNormalExistente.CheckControlId))
                        {
                            MessageBox.Show("ERROR AL ELIMINAR RANGO ANTERIOR",
                                          "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    //   CASO 3: Agregando rango prioritario cuando solo existe normal → OK, se agrega
                    //   CASO 4: Agregando rango normal cuando solo existe prioritario → OK, se agrega
                }

                // CREAR NUEVO CONTROL
                Mdl_CheckControl nuevoControl = new Mdl_CheckControl
                {
                    UserId = userId,
                    InitialLimit = limiteInicial,
                    FinalLimit = limiteFinal,
                    CurrentCounter = siguienteCheque,
                    Priority = CheckBox_AltaPrioridad.Checked,
                    IsActive = true,
                    CreatedBy = _frmPadre.UserData.UserId
                };

                if (Ctrl_CheckControl.RegistrarControl(nuevoControl) > 0)
                {
                    MessageBox.Show("RANGO DE CHEQUES ASIGNADO CORRECTAMENTE",
                                  "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarRangosExistentes();
                    LimpiarTodo();
                    _frmPadre.CargarControlCheques();
                }
                else
                {
                    MessageBox.Show("ERROR AL ASIGNAR RANGO DE CHEQUES",
                                  "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL GUARDAR: {ex.Message}",
                              "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ACTUALIZAR RANGO EXISTENTE
        private void Btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                if (_rangoSeleccionadoId == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN RANGO DE LA TABLA",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarDatosCompletos())
                    return;

                int limiteInicial = Convert.ToInt32(ObtenerTextoReal(Txt_Li));
                int limiteFinal = Convert.ToInt32(ObtenerTextoReal(Txt_Fin));
                int siguienteCheque = Convert.ToInt32(ObtenerTextoReal(Txt_SiguienteCheque));

                if (limiteInicial > limiteFinal)
                {
                    MessageBox.Show("EL LÍMITE INICIAL NO PUEDE SER MAYOR QUE EL LÍMITE FINAL",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (siguienteCheque < limiteInicial || siguienteCheque > limiteFinal)
                {
                    MessageBox.Show("EL SIGUIENTE CHEQUE DEBE ESTAR ENTRE EL LÍMITE INICIAL Y FINAL",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    "¿DESEA ACTUALIZAR EL RANGO DE CHEQUES?",
                    "CONFIRMAR ACTUALIZACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.No)
                    return;

                // Obtener control existente
                Mdl_CheckControl control = Ctrl_CheckControl.ObtenerControlPorId(_rangoSeleccionadoId);
                if (control == null)
                {
                    MessageBox.Show("ERROR AL OBTENER RANGO",
                                  "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                control.InitialLimit = limiteInicial;
                control.FinalLimit = limiteFinal;
                control.CurrentCounter = siguienteCheque;
                control.Priority = CheckBox_AltaPrioridad.Checked;
                control.ModifiedBy = _frmPadre.UserData.UserId;

                if (Ctrl_CheckControl.ActualizarControl(control) > 0)
                {
                    MessageBox.Show("RANGO ACTUALIZADO CORRECTAMENTE",
                                  "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarRangosExistentes();
                    LimpiarTodo();
                    _frmPadre.CargarControlCheques();
                }
                else
                {
                    MessageBox.Show("ERROR AL ACTUALIZAR RANGO",
                                  "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ACTUALIZAR: {ex.Message}",
                              "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ELIMINAR RANGO
        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_rangoSeleccionadoId == 0)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN RANGO DE LA TABLA",
                                  "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    "¿ESTÁ SEGURO DE ELIMINAR ESTE RANGO DE CHEQUES?\n\n" +
                    "ESTA ACCIÓN NO SE PUEDE DESHACER.",
                    "CONFIRMAR ELIMINACIÓN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.No)
                    return;

                if (Ctrl_CheckControl.EliminarControl(_rangoSeleccionadoId))
                {
                    MessageBox.Show("RANGO ELIMINADO CORRECTAMENTE",
                                  "ÉXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarRangosExistentes();
                    LimpiarTodo();
                    _frmPadre.CargarControlCheques();
                }
                else
                {
                    MessageBox.Show("ERROR AL ELIMINAR RANGO",
                                  "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL ELIMINAR: {ex.Message}",
                              "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // LIMPIAR CAMPOS
        private void Btn_Clear2_Click(object sender, EventArgs e)
        {
            LimpiarTodo();
        }
        #endregion BotonesAccion
        #region MetodosAuxiliares
        private bool ValidarDatosCompletos()
        {
            if (string.IsNullOrEmpty(ObtenerTextoReal(Txt_Seleccionado)))
            {
                MessageBox.Show("DEBE SELECCIONAR UN USUARIO O MARCAR COMPARTIDO",
                              "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(ObtenerTextoReal(Txt_Li)) ||
                string.IsNullOrEmpty(ObtenerTextoReal(Txt_Fin)))
            {
                MessageBox.Show("DEBE INGRESAR LÍMITE INICIAL Y FINAL",
                              "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //   NUEVA VALIDACIÓN: Txt_SiguienteCheque no debe estar vacío
            if (string.IsNullOrEmpty(ObtenerTextoReal(Txt_SiguienteCheque)))
            {
                MessageBox.Show("DEBE INGRESAR EL NÚMERO DEL SIGUIENTE CHEQUE",
                              "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_SiguienteCheque.Focus();
                return false;
            }

            return true;
        }

        private int ObtenerUserIdSeleccionado()
        {
            if (CheckBox_Compartido.Checked)
            {
                // Buscar usuario SYS_CHECKS_SHARE
                Mdl_Users usuario = Ctrl_Users.ObtenerUsuarioPorUsername("SYS_CHECKS_SHARE");
                return usuario?.UserId ?? 0;
            }
            else
            {
                if (Tabla1.SelectedRows.Count > 0)
                {
                    return Convert.ToInt32(Tabla1.SelectedRows[0].Cells["UserId"].Value);
                }
            }

            return 0;
        }

        private void ConfigurarBotonesParaNuevo()
        {
            Btn_Save.Enabled = true;
            Btn_Update.Enabled = false;
            Btn_Delete.Enabled = false;
        }

        private void ConfigurarBotonesParaEditar()
        {
            Btn_Save.Enabled = false;
            Btn_Update.Enabled = true;
            Btn_Delete.Enabled = true;
        }

        private void LimpiarCamposRango()
        {
            Txt_Li.Text = "0";
            Txt_Li.ForeColor = Color.Gray;

            Txt_Fin.Text = "0";
            Txt_Fin.ForeColor = Color.Gray;

            Txt_SiguienteCheque.Text = "0";
            Txt_SiguienteCheque.ForeColor = Color.Gray;
        }

        private void LimpiarTodo()
        {
            Txt_Seleccionado.Text = "USUARIO SELECCIONADO";
            Txt_Seleccionado.ForeColor = Color.Gray;
            CheckBox_AltaPrioridad.Checked = false;

            LimpiarCamposRango();

            CheckBox_Compartido.Checked = true;
            Tabla1.ClearSelection();
            Tabla2.ClearSelection();

            _rangoSeleccionadoId = 0;
            ConfigurarBotonesParaNuevo();
        }
        #endregion MetodosAuxiliares
        #region ValidacionNumerica
        private void Txt_Li_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void Txt_Fin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void Txt_SiguienteCheque_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }
        #endregion ValidacionNumerica
    }
}