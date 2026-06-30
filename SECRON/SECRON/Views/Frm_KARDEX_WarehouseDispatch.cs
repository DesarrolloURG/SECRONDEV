using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_WarehouseDispatch : Form
    {
        public Mdl_Security_UserInfo UserData { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        private enum ModoDestino { Ninguno, Bodega, Empleado }
        private ModoDestino _modoDestino = ModoDestino.Ninguno;

        private int? _destinoWarehouseId = null;
        private string _destinoWarehouseName = "";
        private int? _destinoEmployeeId = null;
        private string _destinoEmployeeName = "";
        private Dictionary<int, Mdl_ItemWarehouseStock> _stockDestino = new Dictionary<int, Mdl_ItemWarehouseStock>();

        private List<Mdl_ItemWarehouseStock> _stockDisponible;
        private decimal? _limiteDespacho = null;

        private Dictionary<int, decimal> _cantidadesEnGrid = new Dictionary<int, decimal>();
        private List<LineaDespacho> _listaDespacho = new List<LineaDespacho>();

        private class LineaDespacho
        {
            public int ItemId { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public decimal Cantidad { get; set; }

            // Solo aplican en modo Bodega; quedan null/"—" en modo Empleado
            public string StockDestino { get; set; }
            public string MinimoDestino { get; set; }
            public string ReorderDestino { get; set; }
        }

        public Frm_KARDEX_WarehouseDispatch()
        {
            InitializeComponent();
        }

        private void Frm_KARDEX_WarehouseDispatch_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                Lbl_Formulario.Text = $"DESPACHO DE ARTÍCULOS - BODEGA: {WarehouseName?.ToUpper()}";

                if (UserData != null)
                {
                    _limiteDespacho = Ctrl_WarehouseManagerPermissions.ObtenerLimiteDespacho(UserData.UserId, WarehouseId);
                }

                ConfigurarTablaResultados();
                ConfigurarTablaListaDespacho();
                CargarStockDisponible();
                ConfigurarPanelDestino();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region PanelDestino

        private void ConfigurarPanelDestino()
        {
            RadioButton_Bodega.Checked = false;
            RadioButton_Empleado.Checked = false;
            Btn_SeleccionarDestino.Enabled = false;
            Txt_Destino.Text = "";

            bool esBodegaCentral = Ctrl_Warehouses.ObtenerBodegaCentralId() == WarehouseId;
            RadioButton_Bodega.Visible = esBodegaCentral;

            HabilitarFlujoDeDespacho(false);
        }

        private void RadioButton_Destino_CheckedChanged(object sender, EventArgs e)
        {
            VaciarListaDespacho();
            _stockDestino.Clear();

            _destinoWarehouseId = null;
            _destinoWarehouseName = "";
            _destinoEmployeeId = null;
            _destinoEmployeeName = "";
            Txt_Destino.Text = "";

            if (RadioButton_Bodega.Checked)
            {
                _modoDestino = ModoDestino.Bodega;
                Btn_SeleccionarDestino.Enabled = true;
            }
            else if (RadioButton_Empleado.Checked)
            {
                _modoDestino = ModoDestino.Empleado;
                Btn_SeleccionarDestino.Enabled = true;
            }
            else
            {
                _modoDestino = ModoDestino.Ninguno;
                Btn_SeleccionarDestino.Enabled = false;
            }

            HabilitarFlujoDeDespacho(false);
        }

        private void Btn_SeleccionarDestino_Click(object sender, EventArgs e)
        {
            try
            {
                if (_modoDestino == ModoDestino.Bodega)
                {
                    using (var frm = new Frm_KARDEX_SearchWarehouse(this, WarehouseId))
                    {
                        frm.StartPosition = FormStartPosition.CenterParent;
                        frm.ShowDialog(this);
                    }
                }
                else if (_modoDestino == ModoDestino.Empleado)
                {
                    using (var frm = new Frm_KARDEX_SearchEmployee(this))
                    {
                        frm.StartPosition = FormStartPosition.CenterParent;
                        frm.ShowDialog(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al seleccionar destino: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Llamado desde Frm_KARDEX_SearchWarehouse al confirmar selección
        public void ActualizarBodegaDestino(int warehouseId, string warehouseName)
        {
            if (_destinoWarehouseId.HasValue && _destinoWarehouseId.Value != warehouseId)
                VaciarListaDespacho();

            _destinoWarehouseId = warehouseId;
            _destinoWarehouseName = warehouseName;
            Txt_Destino.Text = warehouseName;

            CargarStockDestino(warehouseId);
            HabilitarFlujoDeDespacho(true);
        }


        private void CargarStockDestino(int warehouseId)
        {
            var stockDestino = Ctrl_ItemWarehouseStock.ObtenerStockPorBodegaConDetalle(warehouseId);
            _stockDestino = stockDestino.ToDictionary(s => s.ItemId, s => s);
        }

        // Llamado desde Frm_KARDEX_SearchEmployee al confirmar selección
        public void ActualizarEmpleadoDestino(int employeeId, string employeeName)
        {
            if (_destinoEmployeeId.HasValue && _destinoEmployeeId.Value != employeeId)
                VaciarListaDespacho();

            _destinoEmployeeId = employeeId;
            _destinoEmployeeName = employeeName;
            Txt_Destino.Text = employeeName;
            HabilitarFlujoDeDespacho(true);
        }

        private void HabilitarFlujoDeDespacho(bool habilitado)
        {
            Txt_ValorBuscado.Enabled = habilitado;
            Btn_Search.Enabled = habilitado;
            Btn_ClearSearch.Enabled = habilitado;
            Tabla_Resultados.Enabled = habilitado;

            if (!habilitado)
                VaciarListaDespacho();
        }

        private void VaciarListaDespacho()
        {
            _listaDespacho.Clear();
            AsignarListaDespacho();
        }

        #endregion PanelDestino
        #region CargaYBusqueda

        private void CargarStockDisponible()
        {
            _stockDisponible = Ctrl_ItemWarehouseStock.ObtenerStockPorBodegaConDetalle(WarehouseId)
                .Where(s => s.CurrentStock > 0)
                .ToList();

            AsignarResultados(_stockDisponible);
        }

        private void ConfigurarTablaResultados()
        {
            Tabla_Resultados.AllowUserToAddRows = false;
            Tabla_Resultados.AllowUserToResizeRows = false;
            Tabla_Resultados.RowHeadersVisible = false;
            Tabla_Resultados.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla_Resultados.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            Tabla_Resultados.BackgroundColor = Color.FromArgb(248, 249, 250);
            Tabla_Resultados.ScrollBars = ScrollBars.Both;
            Tabla_Resultados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void AsignarResultados(List<Mdl_ItemWarehouseStock> lista)
        {
            Tabla_Resultados.Rows.Clear();

            foreach (var item in lista)
            {
                int fila = Tabla_Resultados.Rows.Add();
                var row = Tabla_Resultados.Rows[fila];

                row.Cells["Col_ItemId"].Value = item.ItemId;
                row.Cells["Col_Codigo"].Value = item.ItemCode;
                row.Cells["Col_Articulo"].Value = item.ItemName;
                row.Cells["Col_StockDisponible"].Value = item.CurrentStock.ToString("0.##");

                if (!_cantidadesEnGrid.ContainsKey(item.ItemId))
                    _cantidadesEnGrid[item.ItemId] = 1;

                row.Cells["Col_Cantidad"].Value = _cantidadesEnGrid[item.ItemId].ToString("0.##");
            }
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string texto = Txt_ValorBuscado.Text.Trim().ToUpper();

            var filtrado = string.IsNullOrWhiteSpace(texto)
                ? _stockDisponible
                : _stockDisponible.Where(s =>
                    (s.ItemCode?.ToUpper().Contains(texto) ?? false) ||
                    (s.ItemName?.ToUpper().Contains(texto) ?? false)).ToList();

            AsignarResultados(filtrado);
        }

        private void Txt_ValorBuscado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Btn_Search_Click(sender, e);
            }
        }

        private void Btn_ClearSearch_Click(object sender, EventArgs e)
        {
            Txt_ValorBuscado.Text = "";
            AsignarResultados(_stockDisponible);
        }

        #endregion CargaYBusqueda
        #region SelectorCantidadEnGrid

        // Tope máximo según el modo: Bodega = solo stock, Empleado = stock y límite de despacho (el menor de ambos)
        private decimal ObtenerTopeMaximo(Mdl_ItemWarehouseStock articulo)
        {
            decimal topeMaximo = articulo.CurrentStock;

            if (_modoDestino == ModoDestino.Empleado && _limiteDespacho.HasValue && _limiteDespacho.Value < topeMaximo)
                topeMaximo = _limiteDespacho.Value;

            return topeMaximo;
        }

        private void Tabla_Resultados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (_modoDestino == ModoDestino.Ninguno) return;

            string columna = Tabla_Resultados.Columns[e.ColumnIndex].Name;
            if (columna != "Col_Menos" && columna != "Col_Mas" && columna != "Col_Agregar") return;

            var fila = Tabla_Resultados.Rows[e.RowIndex];
            if (fila.Cells["Col_ItemId"].Value == null) return;

            int itemId = (int)fila.Cells["Col_ItemId"].Value;
            var articulo = _stockDisponible.FirstOrDefault(s => s.ItemId == itemId);
            if (articulo == null) return;

            decimal topeMaximo = ObtenerTopeMaximo(articulo);
            decimal cantidadActual = _cantidadesEnGrid.ContainsKey(itemId) ? _cantidadesEnGrid[itemId] : 1;

            switch (columna)
            {
                case "Col_Menos":
                    if (cantidadActual > 1)
                        cantidadActual -= 1;
                    _cantidadesEnGrid[itemId] = cantidadActual;
                    fila.Cells["Col_Cantidad"].Value = cantidadActual.ToString("0.##");
                    break;

                case "Col_Mas":
                    if (cantidadActual < topeMaximo)
                        cantidadActual += 1;
                    else
                    {
                        string motivo = _modoDestino == ModoDestino.Empleado
                            && _limiteDespacho.HasValue && topeMaximo == _limiteDespacho.Value
                            ? $"NO PUEDE SUPERAR EL LÍMITE PERMITIDO DE {_limiteDespacho.Value:0.##} POR DESPACHO."
                            : "NO PUEDE SUPERAR EL STOCK DISPONIBLE.";
                        MessageBox.Show(motivo, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    _cantidadesEnGrid[itemId] = cantidadActual;
                    fila.Cells["Col_Cantidad"].Value = cantidadActual.ToString("0.##");
                    break;

                case "Col_Agregar":
                    AgregarALaListaDeDespacho(articulo, cantidadActual, topeMaximo);
                    break;
            }
        }

        private void Tabla_Resultados_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (Tabla_Resultados.Columns[e.ColumnIndex].Name != "Col_Cantidad") return;

            var fila = Tabla_Resultados.Rows[e.RowIndex];
            if (fila.Cells["Col_ItemId"].Value == null) return;

            int itemId = (int)fila.Cells["Col_ItemId"].Value;
            var articulo = _stockDisponible.FirstOrDefault(s => s.ItemId == itemId);
            if (articulo == null) return;

            decimal topeMaximo = ObtenerTopeMaximo(articulo);

            string valorTexto = fila.Cells["Col_Cantidad"].Value?.ToString() ?? "1";

            if (!decimal.TryParse(valorTexto, out decimal cantidad) || cantidad < 1)
                cantidad = 1;

            if (cantidad > topeMaximo)
            {
                string motivo = _modoDestino == ModoDestino.Empleado
                    && _limiteDespacho.HasValue && topeMaximo == _limiteDespacho.Value
                    ? $"NO PUEDE SUPERAR EL LÍMITE PERMITIDO DE {_limiteDespacho.Value:0.##} POR DESPACHO."
                    : "NO PUEDE SUPERAR EL STOCK DISPONIBLE.";
                MessageBox.Show(motivo, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cantidad = topeMaximo;
            }

            _cantidadesEnGrid[itemId] = cantidad;
            fila.Cells["Col_Cantidad"].Value = cantidad.ToString("0.##");
        }

        #endregion SelectorCantidadEnGrid
        #region ListaDespacho

        private void ConfigurarTablaListaDespacho()
        {
            Tabla_ListaDespacho.AllowUserToAddRows = false;
            Tabla_ListaDespacho.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla_ListaDespacho.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            AsignarListaDespacho();
        }

        private void AsignarListaDespacho()
        {
            var data = _listaDespacho.Select(l => new
            {
                l.ItemId,
                l.ItemCode,
                l.ItemName,
                l.Cantidad,
                l.StockDestino,
                l.MinimoDestino,
                l.ReorderDestino
            }).ToList();

            Tabla_ListaDespacho.DataSource = data;

            if (Tabla_ListaDespacho.Columns.Contains("ItemId"))
                Tabla_ListaDespacho.Columns["ItemId"].Visible = false;
            if (Tabla_ListaDespacho.Columns.Contains("ItemCode"))
                Tabla_ListaDespacho.Columns["ItemCode"].HeaderText = "CÓDIGO";
            if (Tabla_ListaDespacho.Columns.Contains("ItemName"))
            {
                Tabla_ListaDespacho.Columns["ItemName"].HeaderText = "ARTÍCULO";
                Tabla_ListaDespacho.Columns["ItemName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            if (Tabla_ListaDespacho.Columns.Contains("Cantidad"))
                Tabla_ListaDespacho.Columns["Cantidad"].HeaderText = "CANTIDAD";
            if (Tabla_ListaDespacho.Columns.Contains("StockDestino"))
                Tabla_ListaDespacho.Columns["StockDestino"].HeaderText = "DISPONIBLE EN DESTINO";
            if (Tabla_ListaDespacho.Columns.Contains("MinimoDestino"))
                Tabla_ListaDespacho.Columns["MinimoDestino"].HeaderText = "MÍNIMO EN DESTINO";
            if (Tabla_ListaDespacho.Columns.Contains("ReorderDestino"))
                Tabla_ListaDespacho.Columns["ReorderDestino"].HeaderText = "REORDEN EN DESTINO";

            // Las 3 columnas fijas de destino (Col_StockDestino/Col_MinimoDestino/Col_ReorderDestino del Designer)
            // quedan ocultas porque el DataSource trae sus propias columnas con el mismo dato (StockDestino, etc.)
            if (Tabla_ListaDespacho.Columns.Contains("Col_StockDestino"))
                Tabla_ListaDespacho.Columns["Col_StockDestino"].Visible = false;
            if (Tabla_ListaDespacho.Columns.Contains("Col_MinimoDestino"))
                Tabla_ListaDespacho.Columns["Col_MinimoDestino"].Visible = false;
            if (Tabla_ListaDespacho.Columns.Contains("Col_ReorderDestino"))
                Tabla_ListaDespacho.Columns["Col_ReorderDestino"].Visible = false;

            if (Tabla_ListaDespacho.Columns.Contains("Col_Quitar"))
                Tabla_ListaDespacho.Columns["Col_Quitar"].DisplayIndex = Tabla_ListaDespacho.Columns.Count - 1;

            Btn_ConfirmarDespacho.Enabled = _listaDespacho.Count > 0;
        }

        private void AgregarALaListaDeDespacho(Mdl_ItemWarehouseStock articulo, decimal cantidad, decimal topeMaximo)
        {
            if (cantidad <= 0)
            {
                MessageBox.Show("LA CANTIDAD DEBE SER MAYOR A CERO.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existente = _listaDespacho.FirstOrDefault(l => l.ItemId == articulo.ItemId);
            decimal cantidadAcumulada = (existente?.Cantidad ?? 0) + cantidad;

            if (cantidadAcumulada > topeMaximo)
            {
                string motivo = _modoDestino == ModoDestino.Empleado
                    && _limiteDespacho.HasValue && topeMaximo == _limiteDespacho.Value
                    ? $"LA CANTIDAD ACUMULADA EXCEDE EL LÍMITE PERMITIDO ({_limiteDespacho.Value:0.##})."
                    : "LA CANTIDAD ACUMULADA EXCEDE EL STOCK DISPONIBLE.";
                MessageBox.Show(motivo, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string stockDestino = "—", minimoDestino = "—", reorderDestino = "—";

            if (_modoDestino == ModoDestino.Bodega)
            {
                if (_stockDestino.ContainsKey(articulo.ItemId))
                {
                    var destino = _stockDestino[articulo.ItemId];
                    stockDestino = destino.CurrentStock.ToString("0.##");
                    minimoDestino = destino.MinimumStock.ToString("0.##");
                    reorderDestino = destino.ReorderPoint.ToString("0.##");
                }
                else
                {
                    stockDestino = "0";
                }
            }

            if (existente != null)
            {
                existente.Cantidad = cantidadAcumulada;
                existente.StockDestino = stockDestino;
                existente.MinimoDestino = minimoDestino;
                existente.ReorderDestino = reorderDestino;
            }
            else
            {
                _listaDespacho.Add(new LineaDespacho
                {
                    ItemId = articulo.ItemId,
                    ItemCode = articulo.ItemCode,
                    ItemName = articulo.ItemName,
                    Cantidad = cantidad,
                    StockDestino = stockDestino,
                    MinimoDestino = minimoDestino,
                    ReorderDestino = reorderDestino
                });
            }

            AsignarListaDespacho();
        }

        private void Tabla_ListaDespacho_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (Tabla_ListaDespacho.Columns[e.ColumnIndex].Name != "Col_Quitar") return;

            var fila = Tabla_ListaDespacho.Rows[e.RowIndex];
            if (fila.Cells["ItemId"].Value == null) return;

            int itemId = (int)fila.Cells["ItemId"].Value;
            var linea = _listaDespacho.FirstOrDefault(l => l.ItemId == itemId);
            if (linea != null)
            {
                _listaDespacho.Remove(linea);
                AsignarListaDespacho();
            }
        }

        #endregion ListaDespacho
        #region Confirmar

        private void Btn_ConfirmarDespacho_Click(object sender, EventArgs e)
        {
            try
            {
                if (_modoDestino == ModoDestino.Ninguno)
                {
                    MessageBox.Show("DEBE SELECCIONAR UN DESTINO (BODEGA O EMPLEADO).", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_modoDestino == ModoDestino.Bodega && !_destinoWarehouseId.HasValue)
                {
                    MessageBox.Show("DEBE SELECCIONAR LA BODEGA DESTINO.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_modoDestino == ModoDestino.Empleado && !_destinoEmployeeId.HasValue)
                {
                    MessageBox.Show("DEBE SELECCIONAR EL EMPLEADO DESTINO.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_listaDespacho.Count == 0)
                {
                    MessageBox.Show("LA LISTA DE DESPACHO ESTÁ VACÍA.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string destinoTexto = _modoDestino == ModoDestino.Bodega ? _destinoWarehouseName : _destinoEmployeeName;

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO QUE DESEA CONFIRMAR EL DESPACHO DE {_listaDespacho.Count} ARTÍCULO(S) DESDE \"{WarehouseName?.ToUpper()}\" HACIA \"{destinoTexto?.ToUpper()}\"?",
                    "CONFIRMAR DESPACHO", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                this.Cursor = Cursors.WaitCursor;

                int resultado = _modoDestino == ModoDestino.Bodega
                    ? ConfirmarDespachoABodega()
                    : ConfirmarDespachoAEmpleado();

                this.Cursor = Cursors.Default;

                ProcesarResultado(resultado);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al confirmar despacho: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ConfirmarDespachoABodega()
        {
            var items = _listaDespacho.Select(l => new Mdl_ItemMovementDetailInput
            {
                ItemId = l.ItemId,
                Quantity = l.Cantidad
            }).ToList();

            string itemsJson = Ctrl_ItemMovement.ConstruirItemsJson(items);

            return Ctrl_ItemMovement.EjecutarTransferenciaEntreBodegas(
                WarehouseId, _destinoWarehouseId.Value, itemsJson, UserData.UserId);
        }

        private int ConfirmarDespachoAEmpleado()
        {
            var items = _listaDespacho.Select(l => new Mdl_ItemMovementDetailInput
            {
                ItemId = l.ItemId,
                Quantity = l.Cantidad
            }).ToList();

            return Ctrl_WarehouseDispatch.RegistrarDespacho(
                WarehouseId, items, UserData.UserId, destinationEmployeeId: _destinoEmployeeId);
        }

        private void ProcesarResultado(int resultado)
        {
            switch (resultado)
            {
                case 1:
                    MessageBox.Show("DESPACHO REGISTRADO EXITOSAMENTE.", "ÉXITO",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    int warehouseIdOrigen = WarehouseId;
                    int? warehouseIdDestino = _modoDestino == ModoDestino.Bodega ? _destinoWarehouseId : (int?)null;

                    System.Threading.Tasks.Task.Run(() =>
                    {
                        Ctrl_WarehouseReorderNotification.VerificarYNotificar(warehouseIdOrigen);
                        if (warehouseIdDestino.HasValue)
                            Ctrl_WarehouseReorderNotification.VerificarYNotificar(warehouseIdDestino.Value);
                    });

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;
                case -1:
                    MessageBox.Show("NO TIENE PERMISO DE DESPACHO EN ESTA BODEGA, O EL TIPO DE MOVIMIENTO NO ES VÁLIDO.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -2:
                    MessageBox.Show("LA LISTA DE ARTÍCULOS ESTÁ VACÍA, O EL ORIGEN ES IGUAL AL DESTINO.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -3:
                    MessageBox.Show("UNA O MÁS CANTIDADES EXCEDEN EL LÍMITE PERMITIDO.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case -4:
                    MessageBox.Show("NO EXISTE UN TIPO DE MOVIMIENTO VÁLIDO ACTIVO.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case -5:
                    MessageBox.Show("NO SE PUDO PROCESAR EL DESPACHO. VERIFIQUE EL STOCK DISPONIBLE.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show("OCURRIÓ UN ERROR INESPERADO AL PROCESAR EL DESPACHO.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void Btn_Cancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion Confirmar
    }
}