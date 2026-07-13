using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Warehouse_Managment : Form
    {
        #region PropiedadesIniciales

        public Mdl_Security_UserInfo UserData { get; set; }

        private readonly Dictionary<TextBox, string> _placeholders = new Dictionary<TextBox, string>();

        private List<Mdl_Warehouse> warehousesList = new List<Mdl_Warehouse>();
        private Mdl_Warehouse _bodegaSeleccionada = null;
        private bool _cargandoBodega = false;

        private string _ultimoTextoBusqueda = "";
        private string _ultimoTipoFiltro = "TODOS";
        private bool? _ultimoIsActive = null;

        // Variables de paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 100;
        private int totalRegistros = 0;
        private int totalPaginas = 0;

        // ToolStrip de paginación
        private ToolStrip toolStripPaginacion;
        private ToolStripButton btnAnterior;
        private ToolStripButton btnSiguiente;

        public Frm_Warehouse_Managment()
        {
            InitializeComponent();
            this.Load += Frm_Warehouse_Managment_Load;

            CargarFiltrosBusqueda();
            ConfigurarPlaceHoldersTextbox();
        }

        #endregion

        #region CargaInicial

        private async void Frm_Warehouse_Managment_Load(object sender, EventArgs e)
        {
            ConfigurarTabIndexYFocus();
            CargarLocationsParaCombo();
            CargarWarehouseTypes();
            CrearToolStripPaginacion();
            paginaActual = 1;

            if (UserData != null)
            {
                await CargarPermisosUsuario(UserData.UserId, UserData.RoleId);
                ConfigurarControlesPorPermisos();
            }

            RefrescarListado();
            ActualizarInfoPaginacion();

            if (Tabla.Rows.Count > 0)
                Tabla.ClearSelection();
        }

        #endregion

        #region Tabla

        public void RefrescarListado()
        {
            warehousesList = Ctrl_Warehouses.BuscarBodegas(
                _ultimoTextoBusqueda, _ultimoTipoFiltro, _ultimoIsActive, paginaActual, registrosPorPagina);

            Tabla.DataSource = null;
            Tabla.DataSource = warehousesList;

            ConfigurarTabla();

            totalRegistros = Ctrl_Warehouses.ContarTotalBodegas(
                _ultimoTextoBusqueda, _ultimoTipoFiltro, _ultimoIsActive);

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
        }

        public void ConfigurarTabla()
        {
            if (Tabla.Columns.Count > 0)
            {
                Tabla.Columns["WarehouseCode"].HeaderText = "CÓDIGO";
                Tabla.Columns["WarehouseName"].HeaderText = "NOMBRE";
                Tabla.Columns["LocationCode"].HeaderText = "CÓD. SEDE";
                Tabla.Columns["LocationName"].HeaderText = "SEDE";
                Tabla.Columns["WarehouseType"].HeaderText = "TIPO";
                Tabla.Columns["Address"].HeaderText = "DIRECCIÓN";
                Tabla.Columns["PhoneNumber"].HeaderText = "TELÉFONO";

                Tabla.Columns["WarehouseCode"].DisplayIndex = 0;
                Tabla.Columns["WarehouseName"].DisplayIndex = 1;
                Tabla.Columns["LocationCode"].DisplayIndex = 2;
                Tabla.Columns["LocationName"].DisplayIndex = 3;
                Tabla.Columns["WarehouseType"].DisplayIndex = 4;
                Tabla.Columns["Address"].DisplayIndex = 5;
                Tabla.Columns["PhoneNumber"].DisplayIndex = 6;

                Tabla.Columns["WarehouseId"].Visible = false;
                Tabla.Columns["Description"].Visible = false;
                Tabla.Columns["ManagerUserId"].Visible = false;
                Tabla.Columns["LocationId"].Visible = false;
                Tabla.Columns["IsActive"].Visible = false;
                Tabla.Columns["CreatedDate"].Visible = false;
                Tabla.Columns["CreatedBy"].Visible = false;
                Tabla.Columns["ModifiedDate"].Visible = false;
                Tabla.Columns["ModifiedBy"].Visible = false;
            }

            Tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tabla.MultiSelect = false;
            Tabla.ReadOnly = true;
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToDeleteRows = false;
            Tabla.AllowUserToResizeRows = false;
            Tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Tabla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            Tabla.SelectionChanged -= Tabla_SelectionChanged;
            Tabla.SelectionChanged += Tabla_SelectionChanged;
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            CargarDatosBodegaSeleccionada();
        }

        private void CargarDatosBodegaSeleccionada()
        {
            try
            {
                if (Tabla.SelectedRows.Count == 0)
                    return;

                _cargandoBodega = true;

                DataGridViewRow fila = Tabla.SelectedRows[0];
                int warehouseId = Convert.ToInt32(fila.Cells["WarehouseId"].Value);

                _bodegaSeleccionada = warehousesList.FirstOrDefault(w => w.WarehouseId == warehouseId);

                if (_bodegaSeleccionada == null)
                    return;

                AsignarTextoReal(Txt_Code, _bodegaSeleccionada.WarehouseCode);
                AsignarTextoReal(Txt_Name, _bodegaSeleccionada.WarehouseName);
                AsignarTextoReal(textBox1, _bodegaSeleccionada.PhoneNumber);
                AsignarTextoReal(Txt_Desctiption, _bodegaSeleccionada.Description);
                AsignarTextoReal(Txt_Address, _bodegaSeleccionada.Address);

                if (_bodegaSeleccionada.LocationId.HasValue)
                    ComboBox_LocationId.SelectedValue = _bodegaSeleccionada.LocationId.Value;
                else if (ComboBox_LocationId.Items.Count > 0)
                    ComboBox_LocationId.SelectedIndex = 0;

                int tipoIndex = ComboBox_WarehouseType.FindStringExact(_bodegaSeleccionada.WarehouseType ?? "");
                ComboBox_WarehouseType.SelectedIndex = tipoIndex >= 0 ? tipoIndex : 0;

                // LocationId no es editable una vez creada la bodega
                ComboBox_LocationId.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar datos de la bodega: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                _cargandoBodega = false;
            }
        }

        #endregion

        #region CombosDependientes

        private void CargarLocationsParaCombo()
        {
            try
            {
                var lista = Ctrl_Locations.ObtenerLocationsActivas();
                lista.Insert(0, new KeyValuePair<int, string>(0, "SELECCIONE"));

                ComboBox_LocationId.DataSource = null;
                ComboBox_LocationId.DataSource = lista;
                ComboBox_LocationId.DisplayMember = "Value";
                ComboBox_LocationId.ValueMember = "Key";
                ComboBox_LocationId.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar sedes: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void CargarWarehouseTypes()
        {
            ComboBox_WarehouseType.Items.Clear();
            ComboBox_WarehouseType.Items.Add("REGIONAL");
            ComboBox_WarehouseType.SelectedIndex = 0;
        }

        #endregion

        #region Placeholders

        private void ConfigurarTabIndexYFocus()
        {
            // ===== CONTROLES DE BÚSQUEDA =====
            Txt_ValorBuscado.TabIndex = 0;
            Filtro1.TabIndex = 1;
            Filtro2.TabIndex = 2;
            Btn_Search.TabIndex = 3;
            Btn_CleanSearch.TabIndex = 4;

            // ===== FORMULARIO DE BODEGA =====
            Txt_Name.TabIndex = 5;
            textBox1.TabIndex = 6;
            Txt_Desctiption.TabIndex = 7;
            Txt_Address.TabIndex = 8;
            ComboBox_LocationId.TabIndex = 9;
            ComboBox_WarehouseType.TabIndex = 10;

            Txt_Code.TabStop = false;

            // ===== BOTONES CRUD =====
            Btn_Save.TabIndex = 11;
            Btn_Update.TabIndex = 12;
            Btn_Inactive.TabIndex = 13;
            Btn_Clear.TabIndex = 14;

            // ===== TABLA =====
            Tabla.TabIndex = 15;

            // Controles no navegables
            Panel_Superior.TabStop = false;
            Panel_Derecho.TabStop = false;
            Panel_Izquierdo.TabStop = false;
            PanelToolStrip.TabStop = false;
            Panel_Busqueda.TabStop = false;
            Panel_CRUD.TabStop = false;
            Panel_Informacion.TabStop = false;
            vScrollBar.TabStop = false;

            // Foco inicial
            Txt_ValorBuscado.Focus();
        }

        private void ConfigurarPlaceHoldersTextbox()
        {
            ConfigurarPlaceHolder(Txt_ValorBuscado, "BUSCAR BODEGA POR CÓDIGO O NOMBRE...");
            ConfigurarPlaceHolder(Txt_Name, "NOMBRE DE BODEGA");
            ConfigurarPlaceHolder(textBox1, "TELÉFONO");
            ConfigurarPlaceHolder(Txt_Desctiption, "BREVE DESCRIPCIÓN DE LA BODEGA");
            ConfigurarPlaceHolder(Txt_Address, "DIRECCIÓN COMPLETA");
        }

        private void ConfigurarPlaceHolder(TextBox txt, string placeholder)
        {
            if (_placeholders.ContainsKey(txt))
                _placeholders[txt] = placeholder;
            else
                _placeholders.Add(txt, placeholder);

            txt.Enter -= TextBox_EnterPlaceholder;
            txt.Leave -= TextBox_LeavePlaceholder;

            txt.Enter += TextBox_EnterPlaceholder;
            txt.Leave += TextBox_LeavePlaceholder;

            AplicarPlaceHolderSiVacio(txt);
        }

        private void TextBox_EnterPlaceholder(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (txt == null || !_placeholders.ContainsKey(txt))
                return;

            string placeholder = _placeholders[txt];

            if (txt.ForeColor == Color.Gray && txt.Text == placeholder)
            {
                txt.Text = "";
                txt.ForeColor = Color.Black;
            }
        }

        private void TextBox_LeavePlaceholder(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (txt == null)
                return;

            AplicarPlaceHolderSiVacio(txt);
        }

        private void AplicarPlaceHolderSiVacio(TextBox txt)
        {
            if (txt == null || !_placeholders.ContainsKey(txt))
                return;

            string placeholder = _placeholders[txt];

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = placeholder;
                txt.ForeColor = Color.Gray;
            }
            else if (txt.Text != placeholder)
            {
                txt.ForeColor = Color.Black;
            }
        }

        private bool TienePlaceholder(TextBox txt)
        {
            return _placeholders.ContainsKey(txt) &&
                   txt.ForeColor == Color.Gray &&
                   txt.Text == _placeholders[txt];
        }

        #endregion

        #region FiltrosBusqueda

        private void CargarFiltrosBusqueda()
        {
            Filtro1.Items.Clear();
            Filtro1.Items.Add("TODOS");
            Filtro1.Items.Add("BUSCAR POR NOMBRE");
            Filtro1.Items.Add("BUSCAR POR CODIGO");
            Filtro1.SelectedIndex = 0;

            Filtro2.Items.Clear();
            Filtro2.Items.Add("TODOS");
            Filtro2.Items.Add("SOLO ACTIVOS");
            Filtro2.Items.Add("SOLO DESACTIVOS");
            Filtro2.SelectedIndex = 1;
        }

        private void EjecutarBusqueda()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string valorBusqueda = TienePlaceholder(Txt_ValorBuscado)
                    ? ""
                    : Txt_ValorBuscado.Text.Trim();

                string tipoFiltro = Filtro1.SelectedItem?.ToString() ?? "TODOS";
                string filtroEstado = Filtro2.SelectedItem?.ToString() ?? "TODOS";

                bool? isActive = null;

                if (filtroEstado == "SOLO ACTIVOS")
                    isActive = true;
                else if (filtroEstado == "SOLO DESACTIVOS")
                    isActive = false;

                _ultimoTextoBusqueda = valorBusqueda;
                _ultimoTipoFiltro = tipoFiltro;
                _ultimoIsActive = isActive;
                paginaActual = 1;

                warehousesList = Ctrl_Warehouses.BuscarBodegas(
                    valorBusqueda, tipoFiltro, isActive, paginaActual, registrosPorPagina);

                Tabla.DataSource = null;
                Tabla.DataSource = warehousesList;
                ConfigurarTabla();

                totalRegistros = Ctrl_Warehouses.ContarTotalBodegas(valorBusqueda, tipoFiltro, isActive);

                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
                ActualizarInfoPaginacion();

                LimpiarSeleccionTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error en búsqueda: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            if (!Btn_Search.Enabled) return;
            EjecutarBusqueda();
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                EjecutarBusqueda();
            }
        }

        private void Btn_CleanSearch_Click(object sender, EventArgs e)
        {
            if (!Btn_CleanSearch.Enabled) return;

            try
            {
                _cargandoBodega = true;

                Txt_ValorBuscado.Text = "";
                Filtro1.SelectedIndex = 0;
                Filtro2.SelectedIndex = 1;

                _ultimoTextoBusqueda = "";
                _ultimoTipoFiltro = "TODOS";
                _ultimoIsActive = true;
                paginaActual = 1;
            }
            finally
            {
                _cargandoBodega = false;
            }

            AplicarPlaceHolderSiVacio(Txt_ValorBuscado);
            RefrescarListado();
            ActualizarInfoPaginacion();
            LimpiarSeleccionTabla();
        }

        #endregion

        #region CRUD

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!Btn_Save.Enabled) return;
            try
            {
                if (!ValidarCamposBodega(esActualizacion: false))
                    return;

                var confirmacion = MessageBox.Show(
                    "¿Está seguro que desea registrar esta bodega?",
                    "Confirmar Registro",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                var nuevaBodega = new Mdl_Warehouse
                {
                    LocationId = ObtenerValorComboNullable(ComboBox_LocationId),
                    WarehouseName = TienePlaceholder(Txt_Name) ? null : Txt_Name.Text.Trim().ToUpper(),
                    Description = TienePlaceholder(Txt_Desctiption) ? null : Txt_Desctiption.Text.Trim().ToUpper(),
                    Address = TienePlaceholder(Txt_Address) ? null : Txt_Address.Text.Trim().ToUpper(),
                    PhoneNumber = TienePlaceholder(textBox1) ? null : textBox1.Text.Trim(),
                    WarehouseType = ComboBox_WarehouseType.SelectedItem?.ToString(),
                    CreatedBy = UserData?.UserId ?? 1
                };

                int resultado = Ctrl_Warehouses.Create(nuevaBodega);

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show(
                            "Bodega registrada exitosamente.",
                            "Éxito",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        LimpiarFormularioWarehouse();
                        RefrescarListado();
                        ActualizarInfoPaginacion();
                        break;
                    case -1:
                        MessageBox.Show(
                            "La sede seleccionada no es válida.",
                            "Validación",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        break;
                    default:
                        MessageBox.Show(
                            "No se pudo registrar la bodega.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al guardar la bodega: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (!Btn_Update.Enabled) return;

            try
            {
                if (_bodegaSeleccionada == null || _bodegaSeleccionada.WarehouseId == 0)
                {
                    MessageBox.Show(
                        "Debe seleccionar una bodega para actualizar.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (!ValidarCamposBodega(esActualizacion: true))
                    return;

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea actualizar la bodega {_bodegaSeleccionada.WarehouseName}?",
                    "Confirmar actualización",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                _bodegaSeleccionada.WarehouseName = TienePlaceholder(Txt_Name) ? null : Txt_Name.Text.Trim().ToUpper();
                _bodegaSeleccionada.Description = TienePlaceholder(Txt_Desctiption) ? null : Txt_Desctiption.Text.Trim().ToUpper();
                _bodegaSeleccionada.Address = TienePlaceholder(Txt_Address) ? null : Txt_Address.Text.Trim().ToUpper();
                _bodegaSeleccionada.PhoneNumber = TienePlaceholder(textBox1) ? null : textBox1.Text.Trim();
                _bodegaSeleccionada.WarehouseType = ComboBox_WarehouseType.SelectedItem?.ToString();
                _bodegaSeleccionada.ModifiedBy = UserData?.UserId ?? 1;

                int resultado = Ctrl_Warehouses.Update(_bodegaSeleccionada, isInactivation: false);

                if (resultado == 1)
                {
                    MessageBox.Show(
                        "Bodega actualizada exitosamente.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LimpiarFormularioWarehouse();
                    RefrescarListado();
                    ActualizarInfoPaginacion();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo actualizar la bodega.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al actualizar la bodega: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Btn_Inactive_Click(object sender, EventArgs e)
        {
            if (!Btn_Inactive.Enabled) return;
            try
            {
                if (_bodegaSeleccionada == null || _bodegaSeleccionada.WarehouseId == 0)
                {
                    MessageBox.Show(
                        "Debe seleccionar una bodega para inactivar.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea inactivar la bodega {_bodegaSeleccionada.WarehouseName}?",
                    "Confirmar inactivación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                _bodegaSeleccionada.ModifiedBy = UserData?.UserId ?? 1;

                int resultado = Ctrl_Warehouses.Update(_bodegaSeleccionada, isInactivation: true);

                if (resultado == 1)
                {
                    MessageBox.Show(
                        "Bodega inactivada exitosamente.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LimpiarFormularioWarehouse();
                    RefrescarListado();
                    ActualizarInfoPaginacion();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo inactivar la bodega.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al inactivar la bodega: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            if (!Btn_Clear.Enabled) return;

            LimpiarFormularioWarehouse();
        }

        #endregion

        #region ValidacionesYHelpers

        private int? ObtenerValorComboNullable(ComboBox combo)
        {
            if (combo.SelectedValue == null)
                return null;

            int valor;
            if (int.TryParse(combo.SelectedValue.ToString(), out valor) && valor > 0)
                return valor;

            return null;
        }

        private bool ValidarCamposBodega(bool esActualizacion)
        {
            if (TienePlaceholder(Txt_Name) || string.IsNullOrWhiteSpace(Txt_Name.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el nombre de la bodega.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                Txt_Name.Focus();
                return false;
            }

            if (!esActualizacion && ObtenerValorComboNullable(ComboBox_LocationId) == null)
            {
                MessageBox.Show(
                    "Debe seleccionar una sede.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                ComboBox_LocationId.Focus();
                return false;
            }

            if (ComboBox_WarehouseType.SelectedItem == null)
            {
                MessageBox.Show(
                    "Debe seleccionar el tipo de bodega.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                ComboBox_WarehouseType.Focus();
                return false;
            }

            return true;
        }

        private void LimpiarFormularioWarehouse()
        {
            try
            {
                _cargandoBodega = true;

                _bodegaSeleccionada = null;

                Txt_Code.Text = "";
                Txt_Name.Text = "";
                textBox1.Text = "";
                Txt_Desctiption.Text = "";
                Txt_Address.Text = "";

                AplicarPlaceHolderSiVacio(Txt_Name);
                AplicarPlaceHolderSiVacio(textBox1);
                AplicarPlaceHolderSiVacio(Txt_Desctiption);
                AplicarPlaceHolderSiVacio(Txt_Address);

                ComboBox_LocationId.Enabled = true;
                if (ComboBox_LocationId.Items.Count > 0)
                    ComboBox_LocationId.SelectedIndex = 0;

                if (ComboBox_WarehouseType.Items.Count > 0)
                    ComboBox_WarehouseType.SelectedIndex = 0;
            }
            finally
            {
                _cargandoBodega = false;
            }

            LimpiarSeleccionTabla();
        }

        private void AsignarTextoReal(TextBox txt, string valor)
        {
            txt.Text = valor ?? "";
            txt.ForeColor = Color.Black;
        }

        private void LimpiarSeleccionTabla()
        {
            if (Tabla.Rows.Count > 0)
                Tabla.ClearSelection();
        }

        #endregion

        #region Paginacion

        private void CrearToolStripPaginacion()
        {
            if (toolStripPaginacion != null)
                return;

            toolStripPaginacion = new ToolStrip
            {
                Dock = DockStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                GripStyle = ToolStripGripStyle.Hidden,
                BackColor = Color.FromArgb(248, 249, 250),
                Height = 35,
                AutoSize = true,
                Location = new Point(PanelToolStrip.Width - 260, 5)
            };

            btnAnterior = new ToolStripButton
            {
                Text = "❮ Anterior",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(51, 140, 255),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Margin = new Padding(2),
                Padding = new Padding(8, 4, 8, 4)
            };
            btnAnterior.Click += (s, e) => CambiarPagina(paginaActual - 1);

            btnSiguiente = new ToolStripButton
            {
                Text = "Siguiente ❯",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(238, 143, 109),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Margin = new Padding(2),
                Padding = new Padding(8, 4, 8, 4)
            };
            btnSiguiente.Click += (s, e) => CambiarPagina(paginaActual + 1);

            toolStripPaginacion.Items.Add(btnAnterior);
            toolStripPaginacion.Items.Add(btnSiguiente);

            PanelToolStrip.Controls.Add(toolStripPaginacion);
            toolStripPaginacion.BringToFront();

            PanelToolStrip.Resize += (s, e) =>
            {
                if (toolStripPaginacion != null)
                    toolStripPaginacion.Location = new Point(PanelToolStrip.Width - 260, 5);
            };
        }

        private void ActualizarBotonesNumerados()
        {
            if (toolStripPaginacion == null)
                return;

            var itemsToRemove = toolStripPaginacion.Items
                .Cast<ToolStripItem>()
                .Where(item => item.Tag?.ToString() == "PageButton")
                .ToList();

            foreach (var item in itemsToRemove)
                toolStripPaginacion.Items.Remove(item);

            if (totalPaginas <= 1)
                return;

            int inicioRango = Math.Max(1, paginaActual - 1);
            int finRango = Math.Min(totalPaginas, paginaActual + 1);
            int posicionInsertar = toolStripPaginacion.Items.IndexOf(btnSiguiente);

            for (int i = inicioRango; i <= finRango; i++)
            {
                ToolStripButton btnPagina = new ToolStripButton
                {
                    Text = i.ToString(),
                    Tag = "PageButton",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Margin = new Padding(1),
                    Padding = new Padding(6, 4, 6, 4)
                };

                if (i == paginaActual)
                {
                    btnPagina.BackColor = Color.FromArgb(238, 143, 109);
                    btnPagina.ForeColor = Color.White;
                }
                else
                {
                    btnPagina.BackColor = Color.FromArgb(240, 240, 240);
                    btnPagina.ForeColor = Color.FromArgb(51, 140, 255);
                }

                int numeroPagina = i;
                btnPagina.Click += (s, e) => CambiarPagina(numeroPagina);

                toolStripPaginacion.Items.Insert(posicionInsertar++, btnPagina);
            }
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1 || nuevaPagina > totalPaginas)
                return;

            paginaActual = nuevaPagina;

            warehousesList = Ctrl_Warehouses.BuscarBodegas(
                _ultimoTextoBusqueda, _ultimoTipoFiltro, _ultimoIsActive, paginaActual, registrosPorPagina);

            Tabla.DataSource = null;
            Tabla.DataSource = warehousesList;
            ConfigurarTabla();

            totalRegistros = Ctrl_Warehouses.ContarTotalBodegas(
                _ultimoTextoBusqueda, _ultimoTipoFiltro, _ultimoIsActive);

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
            ActualizarInfoPaginacion();
            LimpiarSeleccionTabla();
        }

        private void ActualizarInfoPaginacion()
        {
            if (btnAnterior != null)
                btnAnterior.Enabled = paginaActual > 1;

            if (btnSiguiente != null)
                btnSiguiente.Enabled = paginaActual < totalPaginas;

            ActualizarBotonesNumerados();

            int inicioRango = totalRegistros == 0 ? 0 : ((paginaActual - 1) * registrosPorPagina) + 1;
            int finRango = Math.Min(paginaActual * registrosPorPagina, totalRegistros);

            if (totalRegistros == 0)
                Lbl_Paginas.Text = "NO HAY BODEGAS PARA MOSTRAR";
            else
                Lbl_Paginas.Text = $"MOSTRANDO {inicioRango}-{finRango} DE {totalRegistros} BODEGAS";
        }

        #endregion

        #region SistemaDePermisos

        private Ctrl_Security_Auth authController;
        private HashSet<string> permisosUsuario = new HashSet<string>();

        protected virtual async Task CargarPermisosUsuario(int userId, int roleId)
        {
            try
            {
                authController = new Ctrl_Security_Auth();
                var permisos = await authController.ObtenerPermisosUsuarioAsync(userId, roleId);
                permisosUsuario = permisos != null
                    ? new HashSet<string>(permisos, StringComparer.OrdinalIgnoreCase)
                    : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                permisosUsuario = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                MessageBox.Show($"ERROR AL CARGAR PERMISOS: {ex.Message}", "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected bool TienePermiso(string permissionCode)
        {
            return !string.IsNullOrWhiteSpace(permissionCode) &&
                   permisosUsuario != null &&
                   permisosUsuario.Contains(permissionCode);
        }

        protected void AplicarEstadoBotonPorPermiso(Button boton, string permissionCode)
        {
            if (boton == null) return;
            bool habilitado = TienePermiso(permissionCode);
            boton.Enabled = habilitado;
            if (habilitado)
            { boton.UseVisualStyleBackColor = true; boton.ForeColor = Color.Black; boton.Cursor = Cursors.Default; }
            else
            { boton.BackColor = Color.FromArgb(200, 200, 200); boton.ForeColor = Color.Gray; boton.Cursor = Cursors.No; }
        }

        protected void ConfigurarControlesPorPermisos()
        {
            AplicarEstadoBotonPorPermiso(Btn_Search, "WAREHOUSE_MANAGMENT_READ");
            AplicarEstadoBotonPorPermiso(Btn_Save, "WAREHOUSE_MANAGMENT_CREATE");
            AplicarEstadoBotonPorPermiso(Btn_Update, "WAREHOUSE_MANAGMENT_UPDATE");
            AplicarEstadoBotonPorPermiso(Btn_Inactive, "WAREHOUSE_MANAGMENT_INACTIVE");
            AplicarEstadoBotonPorPermiso(Btn_Export, "WAREHOUSE_MANAGMENT_EXPORT");
        }

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            try
            {
                var listaExportar = Ctrl_Warehouses.BuscarBodegas(
                    _ultimoTextoBusqueda, _ultimoTipoFiltro, _ultimoIsActive, 1, Math.Max(totalRegistros, 1));

                if (listaExportar == null || listaExportar.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar", "Información",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Exportar Lista de Bodegas",
                    FileName = $"Bodegas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    var excelApp = new Microsoft.Office.Interop.Excel.Application();
                    var workbook = excelApp.Workbooks.Add();
                    var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Bodegas";

                    // ============ ENCABEZADO PRINCIPAL ============
                    worksheet.Cells[1, 1] = "REPORTE DE BODEGAS - SECRON";
                    worksheet.Range["A1:G1"].Merge();
                    worksheet.Range["A1:G1"].Font.Size = 16;
                    worksheet.Range["A1:G1"].Font.Bold = true;
                    worksheet.Range["A1:G1"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Range["A1:G1"].Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(238, 143, 109));
                    worksheet.Range["A1:G1"].Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);

                    // ============ INFORMACIÓN DEL REPORTE ============
                    worksheet.Cells[2, 1] = $"GENERADO POR: {UserData?.FullName?.ToUpper() ?? "SISTEMA"}";
                    worksheet.Cells[3, 1] = $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[4, 1] = $"TOTAL REGISTROS: {listaExportar.Count}";

                    worksheet.Range["A2:A4"].Font.Size = 10;
                    worksheet.Range["A2:A4"].Font.Bold = true;

                    // ============ ENCABEZADOS DE COLUMNAS ============
                    int headerRow = 6;
                    string[] headers = { "CÓDIGO", "NOMBRE", "TIPO", "DIRECCIÓN",
                        "TELÉFONO", "FECHA CREACIÓN", "ESTADO" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[headerRow, i + 1] = headers[i];
                    }

                    var headerRange = worksheet.Range[$"A{headerRow}:G{headerRow}"];
                    headerRange.Font.Bold = true;
                    headerRange.Font.Size = 11;
                    headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(51, 140, 255));
                    headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    headerRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                    // ============ DATOS ============
                    int row = headerRow + 1;
                    foreach (var bodega in listaExportar)
                    {
                        worksheet.Cells[row, 1] = bodega.WarehouseCode ?? "";
                        worksheet.Cells[row, 2] = bodega.WarehouseName ?? "";
                        worksheet.Cells[row, 3] = bodega.WarehouseType ?? "";
                        worksheet.Cells[row, 4] = bodega.Address ?? "";
                        worksheet.Cells[row, 5] = bodega.PhoneNumber ?? "";
                        worksheet.Cells[row, 6] = bodega.CreatedDate.ToString("dd/MM/yyyy");
                        worksheet.Cells[row, 7] = bodega.IsActive ? "ACTIVO" : "INACTIVO";

                        // Alternar color de filas
                        if (row % 2 == 0)
                        {
                            worksheet.Range[$"A{row}:G{row}"].Interior.Color =
                                System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                        }

                        row++;
                    }

                    // ============ FORMATO FINAL ============
                    var dataRange = worksheet.Range[$"A{headerRow}:G{row - 1}"];
                    dataRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    dataRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;

                    worksheet.Columns.AutoFit();

                    worksheet.Activate();
                    excelApp.ActiveWindow.SplitRow = headerRow;
                    excelApp.ActiveWindow.FreezePanes = true;

                    // ============ PIE DE PÁGINA ============
                    worksheet.Cells[row + 1, 1] = "SECRON - Sistema de Control Regional";
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].Merge();
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].Font.Italic = true;
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].Font.Size = 9;
                    worksheet.Range[$"A{row + 1}:G{row + 1}"].HorizontalAlignment =
                        Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    workbook.SaveAs(saveFileDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                    this.Cursor = Cursors.Default;

                    var result = MessageBox.Show(
                        "Archivo exportado exitosamente.\n\n¿Desea abrir el archivo ahora?",
                        "Exportación Exitosa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion SistemaDePermisos
    }
}