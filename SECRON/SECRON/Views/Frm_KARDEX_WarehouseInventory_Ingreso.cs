using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_KARDEX_WarehouseInventory_Ingreso : Form
    {
        public Mdl_Security_UserInfo UserData { get; set; }
        public int WarehouseId { get; set; } // Siempre bodega central
        public string WarehouseName { get; set; }

        private List<Mdl_Items> _catalogo;
        private Dictionary<int, decimal> _stockActualCentral = new Dictionary<int, decimal>();

        private class EstadoFila
        {
            public decimal Cantidad { get; set; } = 1;
            public decimal CostoUnitario { get; set; }
            public string Lote { get; set; }
            public DateTime? Caducidad { get; set; }
        }
        private Dictionary<int, EstadoFila> _estadoFilas = new Dictionary<int, EstadoFila>();

        private int? _proveedorId = null;
        private string _proveedorNombre = "";

        private List<LineaIngreso> _listaIngreso = new List<LineaIngreso>();

        private class LineaIngreso
        {
            public int ItemId { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public decimal Cantidad { get; set; }
            public decimal CostoUnitario { get; set; }
            public string Lote { get; set; }
            public DateTime? Caducidad { get; set; }
        }

        public Frm_KARDEX_WarehouseInventory_Ingreso()
        {
            InitializeComponent();
        }

        private void Frm_KARDEX_WarehouseInventory_Ingreso_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                Lbl_Formulario.Text = $"INGRESO DE MERCADERÍA - BODEGA: {WarehouseName?.ToUpper()}";

                ConfigurarComboMovementType();
                ConfigurarTablaResultados();
                ConfigurarTablaListaIngreso();
                CargarStockActualCentral();
                CargarCatalogo();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}",
                    "Error SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Encabezado

        private void ConfigurarComboMovementType()
        {
            var tipos = Ctrl_MovementTypes.ObtenerTiposDeEntrada();

            ComboBox_MovementType.DisplayMember = "Nombre";
            ComboBox_MovementType.Items.Clear();
            foreach (var tipo in tipos)
                ComboBox_MovementType.Items.Add(new ClasificacionItem(tipo.Key, tipo.Value));

            if (ComboBox_MovementType.Items.Count > 0)
                ComboBox_MovementType.SelectedIndex = 0;
        }

        // Llamado desde Frm_FA_SearchSupplier al confirmar selección
        public void ActualizarProveedor(int proveedorId, string proveedorNombre)
        {
            _proveedorId = proveedorId;
            _proveedorNombre = proveedorNombre;
            Txt_Proveedor.Text = proveedorNombre;
        }

        private void Btn_SearchSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new Frm_KARDEX_SearchSupplier(this))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar proveedor: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Encabezado
        #region CargaYBusqueda

        private void CargarStockActualCentral()
        {
            var stockCentral = Ctrl_ItemWarehouseStock.ObtenerStockPorBodegaConDetalle(WarehouseId);
            _stockActualCentral = stockCentral.ToDictionary(s => s.ItemId, s => s.CurrentStock);
        }

        private void CargarCatalogo()
        {
            _catalogo = Ctrl_Items.BuscarArticulos(textoBusqueda: "", filtro3: "SOLO ACTIVOS", pageSize: 1000);
            AsignarResultados(_catalogo);
        }

        private void ConfigurarTablaResultados()
        {
            Tabla_Resultados.AllowUserToAddRows = false;
            Tabla_Resultados.AllowUserToResizeRows = false;
            Tabla_Resultados.RowHeadersVisible = false;
            Tabla_Resultados.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla_Resultados.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
        }

        private void AsignarResultados(List<Mdl_Items> lista)
        {
            Tabla_Resultados.Rows.Clear();

            foreach (var item in lista)
            {
                int fila = Tabla_Resultados.Rows.Add();
                var row = Tabla_Resultados.Rows[fila];

                decimal stockActual = _stockActualCentral.ContainsKey(item.ItemId) ? _stockActualCentral[item.ItemId] : 0;

                if (!_estadoFilas.ContainsKey(item.ItemId))
                {
                    _estadoFilas[item.ItemId] = new EstadoFila
                    {
                        Cantidad = 1,
                        CostoUnitario = item.LastPurchasePrice
                    };
                }
                var estado = _estadoFilas[item.ItemId];

                row.Cells["Col_ItemId"].Value = item.ItemId;
                row.Cells["Col_Codigo"].Value = item.ItemCode;
                row.Cells["Col_Articulo"].Value = item.ItemName;
                row.Cells["Col_StockActual"].Value = stockActual.ToString("0.##");
                row.Cells["Col_Cantidad"].Value = estado.Cantidad.ToString("0.##");
                row.Cells["Col_CostoUnitario"].Value = estado.CostoUnitario.ToString("0.00", CultureInfo.InvariantCulture);

                row.Cells["Col_Lote"].ReadOnly = !item.HasLotControl;
                row.Cells["Col_Lote"].Style.BackColor = item.HasLotControl ? Color.White : Color.FromArgb(230, 230, 230);
                row.Cells["Col_Lote"].Value = item.HasLotControl ? estado.Lote : "";

                row.Cells["Col_Caducidad"].ReadOnly = !item.HasExpiryDate;
                row.Cells["Col_Caducidad"].Style.BackColor = item.HasExpiryDate ? Color.White : Color.FromArgb(230, 230, 230);
                row.Cells["Col_Caducidad"].Value = item.HasExpiryDate && estado.Caducidad.HasValue
                    ? estado.Caducidad.Value.ToString("yyyy-MM-dd") : "";
            }
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            string texto = Txt_ValorBuscado.Text.Trim();
            var filtrado = Ctrl_Items.BuscarArticulos(textoBusqueda: texto, filtro3: "SOLO ACTIVOS", pageSize: 1000);
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
            AsignarResultados(_catalogo);
        }

        #endregion CargaYBusqueda
        #region SelectorEnGrid

        private void Tabla_Resultados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columna = Tabla_Resultados.Columns[e.ColumnIndex].Name;
            if (columna != "Col_Menos" && columna != "Col_Mas" && columna != "Col_Agregar") return;

            var fila = Tabla_Resultados.Rows[e.RowIndex];
            if (fila.Cells["Col_ItemId"].Value == null) return;

            int itemId = (int)fila.Cells["Col_ItemId"].Value;
            var item = _catalogo.FirstOrDefault(i => i.ItemId == itemId);
            if (item == null) return;

            var estado = _estadoFilas[itemId];

            switch (columna)
            {
                case "Col_Menos":
                    if (estado.Cantidad > 1) estado.Cantidad -= 1;
                    fila.Cells["Col_Cantidad"].Value = estado.Cantidad.ToString("0.##");
                    break;

                case "Col_Mas":
                    estado.Cantidad += 1;
                    fila.Cells["Col_Cantidad"].Value = estado.Cantidad.ToString("0.##");
                    break;

                case "Col_Agregar":
                    AgregarALaListaDeIngreso(item, fila);
                    break;
            }
        }

        private void Tabla_Resultados_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columna = Tabla_Resultados.Columns[e.ColumnIndex].Name;
            var fila = Tabla_Resultados.Rows[e.RowIndex];
            if (fila.Cells["Col_ItemId"].Value == null) return;

            int itemId = (int)fila.Cells["Col_ItemId"].Value;
            if (!_estadoFilas.ContainsKey(itemId)) return;
            var estado = _estadoFilas[itemId];

            switch (columna)
            {
                case "Col_Cantidad":
                    string textoCantidad = fila.Cells["Col_Cantidad"].Value?.ToString() ?? "1";
                    if (!decimal.TryParse(textoCantidad, out decimal cantidad) || cantidad < 1)
                        cantidad = 1;
                    estado.Cantidad = cantidad;
                    fila.Cells["Col_Cantidad"].Value = cantidad.ToString("0.##");
                    break;

                case "Col_CostoUnitario":
                    string textoCosto = fila.Cells["Col_CostoUnitario"].Value?.ToString() ?? "0";
                    if (!decimal.TryParse(textoCosto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal costo) || costo < 0)
                        costo = 0;
                    estado.CostoUnitario = costo;
                    fila.Cells["Col_CostoUnitario"].Value = costo.ToString("0.00", CultureInfo.InvariantCulture);
                    break;

                case "Col_Lote":
                    estado.Lote = fila.Cells["Col_Lote"].Value?.ToString();
                    break;

                case "Col_Caducidad":
                    string textoFecha = fila.Cells["Col_Caducidad"].Value?.ToString();
                    if (DateTime.TryParse(textoFecha, out DateTime fecha))
                    {
                        estado.Caducidad = fecha;
                        fila.Cells["Col_Caducidad"].Value = fecha.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        estado.Caducidad = null;
                    }
                    break;
            }
        }

        #endregion SelectorEnGrid
        #region ListaIngreso

        private void ConfigurarTablaListaIngreso()
        {
            Tabla_ListaIngreso.AllowUserToAddRows = false;
            Tabla_ListaIngreso.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Tabla_ListaIngreso.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            AsignarListaIngreso();
        }

        private void AsignarListaIngreso()
        {
            var data = _listaIngreso.Select(l => new
            {
                l.ItemId,
                l.ItemCode,
                l.ItemName,
                l.Cantidad,
                CostoUnitario = l.CostoUnitario.ToString("0.00", CultureInfo.InvariantCulture),
                Lote = l.Lote ?? "",
                Caducidad = l.Caducidad.HasValue ? l.Caducidad.Value.ToString("yyyy-MM-dd") : ""
            }).ToList();

            Tabla_ListaIngreso.DataSource = data;

            if (Tabla_ListaIngreso.Columns.Contains("ItemId"))
                Tabla_ListaIngreso.Columns["ItemId"].Visible = false;
            if (Tabla_ListaIngreso.Columns.Contains("ItemCode"))
                Tabla_ListaIngreso.Columns["ItemCode"].HeaderText = "CÓDIGO";
            if (Tabla_ListaIngreso.Columns.Contains("ItemName"))
            {
                Tabla_ListaIngreso.Columns["ItemName"].HeaderText = "ARTÍCULO";
                Tabla_ListaIngreso.Columns["ItemName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            if (Tabla_ListaIngreso.Columns.Contains("Cantidad"))
                Tabla_ListaIngreso.Columns["Cantidad"].HeaderText = "CANTIDAD";
            if (Tabla_ListaIngreso.Columns.Contains("CostoUnitario"))
                Tabla_ListaIngreso.Columns["CostoUnitario"].HeaderText = "COSTO UNIT.";
            if (Tabla_ListaIngreso.Columns.Contains("Lote"))
                Tabla_ListaIngreso.Columns["Lote"].HeaderText = "LOTE";
            if (Tabla_ListaIngreso.Columns.Contains("Caducidad"))
                Tabla_ListaIngreso.Columns["Caducidad"].HeaderText = "CADUCIDAD";

            if (Tabla_ListaIngreso.Columns.Contains("Col_Quitar"))
                Tabla_ListaIngreso.Columns["Col_Quitar"].DisplayIndex = Tabla_ListaIngreso.Columns.Count - 1;

            Btn_ConfirmarIngreso.Enabled = _listaIngreso.Count > 0;
        }

        private void AgregarALaListaDeIngreso(Mdl_Items item, DataGridViewRow fila)
        {
            var estado = _estadoFilas[item.ItemId];

            if (estado.Cantidad <= 0)
            {
                MessageBox.Show("LA CANTIDAD DEBE SER MAYOR A CERO.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (estado.CostoUnitario < 0)
            {
                MessageBox.Show("EL COSTO UNITARIO NO PUEDE SER NEGATIVO.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (item.HasLotControl && string.IsNullOrWhiteSpace(estado.Lote))
            {
                MessageBox.Show($"EL ARTÍCULO \"{item.ItemName}\" REQUIERE NÚMERO DE LOTE.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (item.HasExpiryDate && !estado.Caducidad.HasValue)
            {
                MessageBox.Show($"EL ARTÍCULO \"{item.ItemName}\" REQUIERE FECHA DE CADUCIDAD.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existente = _listaIngreso.FirstOrDefault(l =>
                l.ItemId == item.ItemId &&
                l.Lote == estado.Lote &&
                l.Caducidad == estado.Caducidad);

            if (existente != null)
            {
                existente.Cantidad += estado.Cantidad;
                existente.CostoUnitario = estado.CostoUnitario;
            }
            else
            {
                _listaIngreso.Add(new LineaIngreso
                {
                    ItemId = item.ItemId,
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    Cantidad = estado.Cantidad,
                    CostoUnitario = estado.CostoUnitario,
                    Lote = item.HasLotControl ? estado.Lote : null,
                    Caducidad = item.HasExpiryDate ? estado.Caducidad : null
                });
            }

            AsignarListaIngreso();

        }

        private void Tabla_ListaIngreso_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (Tabla_ListaIngreso.Columns[e.ColumnIndex].Name != "Col_Quitar") return;

            var fila = Tabla_ListaIngreso.Rows[e.RowIndex];
            if (fila.Cells["ItemId"].Value == null) return;

            int itemId = (int)fila.Cells["ItemId"].Value;
            string lote = fila.Cells["Lote"].Value?.ToString();
            string caducidadTexto = fila.Cells["Caducidad"].Value?.ToString();

            var linea = _listaIngreso.FirstOrDefault(l =>
                l.ItemId == itemId &&
                (l.Lote ?? "") == lote &&
                (l.Caducidad.HasValue ? l.Caducidad.Value.ToString("yyyy-MM-dd") : "") == caducidadTexto);

            if (linea != null)
            {
                _listaIngreso.Remove(linea);
                AsignarListaIngreso();
            }
        }

        #endregion ListaIngreso
        #region Confirmar

        private void Btn_ConfirmarIngreso_Click(object sender, EventArgs e)
        {
            try
            {
                if (_listaIngreso.Count == 0)
                {
                    MessageBox.Show("LA LISTA DE INGRESO ESTÁ VACÍA.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!(ComboBox_MovementType.SelectedItem is ClasificacionItem tipoSeleccionado))
                {
                    MessageBox.Show("DEBE SELECCIONAR UN TIPO DE MOVIMIENTO.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿ESTÁ SEGURO QUE DESEA CONFIRMAR EL INGRESO DE {_listaIngreso.Count} ARTÍCULO(S) A \"{WarehouseName?.ToUpper()}\"?",
                    "CONFIRMAR INGRESO", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                this.Cursor = Cursors.WaitCursor;

                var items = _listaIngreso.Select(l => new Mdl_ItemMovementDetailInput
                {
                    ItemId = l.ItemId,
                    Quantity = l.Cantidad,
                    UnitCost = l.CostoUnitario,
                    LotNumber = l.Lote,
                    ExpiryDate = l.Caducidad
                }).ToList();

                string itemsJson = Ctrl_ItemMovement.ConstruirItemsJson(items);

                string referenceDocument = string.IsNullOrWhiteSpace(Txt_ReferenceDocument.Text)
                    ? null : Txt_ReferenceDocument.Text.Trim();

                DateTime? purchaseDate = DateTimePicker_PurchaseDate.Checked
                    ? DateTimePicker_PurchaseDate.Value.Date : (DateTime?)null;

                int resultado = Ctrl_ItemMovement.EjecutarItemMovementInsertConFecha(
                    tipoSeleccionado.Id, WarehouseId, null, _proveedorId, referenceDocument,
                    purchaseDate, null, itemsJson, UserData.UserId);

                this.Cursor = Cursors.Default;

                switch (resultado)
                {
                    case 1:
                        MessageBox.Show("INGRESO REGISTRADO EXITOSAMENTE.", "ÉXITO",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        break;
                    case -1:
                        MessageBox.Show("EL TIPO DE MOVIMIENTO SELECCIONADO NO ES VÁLIDO O ESTÁ INACTIVO.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case -4:
                        MessageBox.Show("LA LISTA DE ARTÍCULOS ESTÁ VACÍA.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case -5:
                        MessageBox.Show("UNA O MÁS CANTIDADES NO SON VÁLIDAS.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("OCURRIÓ UN ERROR INESPERADO AL PROCESAR EL INGRESO.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al confirmar ingreso: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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