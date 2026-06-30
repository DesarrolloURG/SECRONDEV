namespace SECRON.Views
{
    partial class Frm_KARDEX_WarehouseInventory_Ingreso
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_KARDEX_WarehouseInventory_Ingreso));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_Encabezado = new System.Windows.Forms.Panel();
            this.Lbl_MovementType = new System.Windows.Forms.Label();
            this.ComboBox_MovementType = new System.Windows.Forms.ComboBox();
            this.Lbl_Proveedor = new System.Windows.Forms.Label();
            this.Txt_Proveedor = new System.Windows.Forms.TextBox();
            this.Btn_SearchSupplier = new System.Windows.Forms.Button();
            this.Lbl_ReferenceDocument = new System.Windows.Forms.Label();
            this.Txt_ReferenceDocument = new System.Windows.Forms.TextBox();
            this.Lbl_PurchaseDate = new System.Windows.Forms.Label();
            this.DateTimePicker_PurchaseDate = new System.Windows.Forms.DateTimePicker();
            this.Panel_Busqueda = new System.Windows.Forms.Panel();
            this.Btn_ClearSearch = new System.Windows.Forms.Button();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Lbl_ValorBuscado = new System.Windows.Forms.Label();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.PanelTabla_Resultados = new System.Windows.Forms.Panel();
            this.Tabla_Resultados = new System.Windows.Forms.DataGridView();
            this.Col_ItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Codigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Articulo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_StockActual = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Menos = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Col_Cantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Mas = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Col_CostoUnitario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Lote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Caducidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Agregar = new System.Windows.Forms.DataGridViewButtonColumn();
            this.PanelTabla_Ingreso = new System.Windows.Forms.Panel();
            this.Tabla_ListaIngreso = new System.Windows.Forms.DataGridView();
            this.Col_Quitar = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Lbl_Subtitulo2 = new System.Windows.Forms.Label();
            this.Panel_CRUD = new System.Windows.Forms.Panel();
            this.Btn_Cancelar = new System.Windows.Forms.Button();
            this.Btn_ConfirmarIngreso = new System.Windows.Forms.Button();
            this.Panel_Superior.SuspendLayout();
            this.Panel_Encabezado.SuspendLayout();
            this.Panel_Busqueda.SuspendLayout();
            this.PanelTabla_Resultados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla_Resultados)).BeginInit();
            this.PanelTabla_Ingreso.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla_ListaIngreso)).BeginInit();
            this.Panel_CRUD.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1150, 68);
            this.Panel_Superior.TabIndex = 1;
            // 
            // Lbl_Formulario
            // 
            this.Lbl_Formulario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Formulario.AutoSize = true;
            this.Lbl_Formulario.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.Lbl_Formulario.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Formulario.Location = new System.Drawing.Point(16, 22);
            this.Lbl_Formulario.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Formulario.Name = "Lbl_Formulario";
            this.Lbl_Formulario.Size = new System.Drawing.Size(248, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "INGRESO DE MERCADERÍA";
            // 
            // Panel_Encabezado
            // 
            this.Panel_Encabezado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Encabezado.BackColor = System.Drawing.Color.White;
            this.Panel_Encabezado.Controls.Add(this.Lbl_MovementType);
            this.Panel_Encabezado.Controls.Add(this.ComboBox_MovementType);
            this.Panel_Encabezado.Controls.Add(this.Lbl_Proveedor);
            this.Panel_Encabezado.Controls.Add(this.Txt_Proveedor);
            this.Panel_Encabezado.Controls.Add(this.Btn_SearchSupplier);
            this.Panel_Encabezado.Controls.Add(this.Lbl_ReferenceDocument);
            this.Panel_Encabezado.Controls.Add(this.Txt_ReferenceDocument);
            this.Panel_Encabezado.Controls.Add(this.Lbl_PurchaseDate);
            this.Panel_Encabezado.Controls.Add(this.DateTimePicker_PurchaseDate);
            this.Panel_Encabezado.Location = new System.Drawing.Point(16, 80);
            this.Panel_Encabezado.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_Encabezado.Name = "Panel_Encabezado";
            this.Panel_Encabezado.Size = new System.Drawing.Size(1118, 136);
            this.Panel_Encabezado.TabIndex = 2;
            // 
            // Lbl_MovementType
            // 
            this.Lbl_MovementType.AutoSize = true;
            this.Lbl_MovementType.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.Lbl_MovementType.ForeColor = System.Drawing.Color.Black;
            this.Lbl_MovementType.Location = new System.Drawing.Point(14, 10);
            this.Lbl_MovementType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_MovementType.Name = "Lbl_MovementType";
            this.Lbl_MovementType.Size = new System.Drawing.Size(153, 17);
            this.Lbl_MovementType.TabIndex = 0;
            this.Lbl_MovementType.Text = "TIPO DE MOVIMIENTO:";
            // 
            // ComboBox_MovementType
            // 
            this.ComboBox_MovementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_MovementType.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.ComboBox_MovementType.FormattingEnabled = true;
            this.ComboBox_MovementType.Location = new System.Drawing.Point(14, 32);
            this.ComboBox_MovementType.Margin = new System.Windows.Forms.Padding(4);
            this.ComboBox_MovementType.Name = "ComboBox_MovementType";
            this.ComboBox_MovementType.Size = new System.Drawing.Size(260, 25);
            this.ComboBox_MovementType.TabIndex = 1;
            // 
            // Lbl_Proveedor
            // 
            this.Lbl_Proveedor.AutoSize = true;
            this.Lbl_Proveedor.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.Lbl_Proveedor.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Proveedor.Location = new System.Drawing.Point(14, 76);
            this.Lbl_Proveedor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Proveedor.Name = "Lbl_Proveedor";
            this.Lbl_Proveedor.Size = new System.Drawing.Size(89, 17);
            this.Lbl_Proveedor.TabIndex = 2;
            this.Lbl_Proveedor.Text = "PROVEEDOR:";
            // 
            // Txt_Proveedor
            // 
            this.Txt_Proveedor.Enabled = false;
            this.Txt_Proveedor.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.Txt_Proveedor.Location = new System.Drawing.Point(14, 98);
            this.Txt_Proveedor.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_Proveedor.Name = "Txt_Proveedor";
            this.Txt_Proveedor.Size = new System.Drawing.Size(290, 25);
            this.Txt_Proveedor.TabIndex = 3;
            // 
            // Btn_SearchSupplier
            // 
            this.Btn_SearchSupplier.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.Btn_SearchSupplier.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_SearchSupplier.Location = new System.Drawing.Point(312, 77);
            this.Btn_SearchSupplier.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_SearchSupplier.Name = "Btn_SearchSupplier";
            this.Btn_SearchSupplier.Size = new System.Drawing.Size(47, 55);
            this.Btn_SearchSupplier.TabIndex = 4;
            this.Btn_SearchSupplier.UseVisualStyleBackColor = true;
            this.Btn_SearchSupplier.Click += new System.EventHandler(this.Btn_SearchSupplier_Click);
            // 
            // Lbl_ReferenceDocument
            // 
            this.Lbl_ReferenceDocument.AutoSize = true;
            this.Lbl_ReferenceDocument.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.Lbl_ReferenceDocument.ForeColor = System.Drawing.Color.Black;
            this.Lbl_ReferenceDocument.Location = new System.Drawing.Point(635, 10);
            this.Lbl_ReferenceDocument.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_ReferenceDocument.Name = "Lbl_ReferenceDocument";
            this.Lbl_ReferenceDocument.Size = new System.Drawing.Size(96, 17);
            this.Lbl_ReferenceDocument.TabIndex = 5;
            this.Lbl_ReferenceDocument.Text = "NO. FACTURA:";
            // 
            // Txt_ReferenceDocument
            // 
            this.Txt_ReferenceDocument.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.Txt_ReferenceDocument.Location = new System.Drawing.Point(635, 32);
            this.Txt_ReferenceDocument.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_ReferenceDocument.MaxLength = 100;
            this.Txt_ReferenceDocument.Name = "Txt_ReferenceDocument";
            this.Txt_ReferenceDocument.Size = new System.Drawing.Size(220, 25);
            this.Txt_ReferenceDocument.TabIndex = 6;
            // 
            // Lbl_PurchaseDate
            // 
            this.Lbl_PurchaseDate.AutoSize = true;
            this.Lbl_PurchaseDate.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.Lbl_PurchaseDate.ForeColor = System.Drawing.Color.Black;
            this.Lbl_PurchaseDate.Location = new System.Drawing.Point(632, 76);
            this.Lbl_PurchaseDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_PurchaseDate.Name = "Lbl_PurchaseDate";
            this.Lbl_PurchaseDate.Size = new System.Drawing.Size(114, 17);
            this.Lbl_PurchaseDate.TabIndex = 7;
            this.Lbl_PurchaseDate.Text = "FECHA FACTURA:";
            // 
            // DateTimePicker_PurchaseDate
            // 
            this.DateTimePicker_PurchaseDate.Checked = false;
            this.DateTimePicker_PurchaseDate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.DateTimePicker_PurchaseDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DateTimePicker_PurchaseDate.Location = new System.Drawing.Point(632, 98);
            this.DateTimePicker_PurchaseDate.Margin = new System.Windows.Forms.Padding(4);
            this.DateTimePicker_PurchaseDate.Name = "DateTimePicker_PurchaseDate";
            this.DateTimePicker_PurchaseDate.ShowCheckBox = true;
            this.DateTimePicker_PurchaseDate.Size = new System.Drawing.Size(220, 25);
            this.DateTimePicker_PurchaseDate.TabIndex = 8;
            // 
            // Panel_Busqueda
            // 
            this.Panel_Busqueda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Busqueda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Busqueda.Controls.Add(this.Btn_ClearSearch);
            this.Panel_Busqueda.Controls.Add(this.Btn_Search);
            this.Panel_Busqueda.Controls.Add(this.Lbl_ValorBuscado);
            this.Panel_Busqueda.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_Busqueda.Location = new System.Drawing.Point(16, 224);
            this.Panel_Busqueda.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_Busqueda.Name = "Panel_Busqueda";
            this.Panel_Busqueda.Size = new System.Drawing.Size(1118, 76);
            this.Panel_Busqueda.TabIndex = 3;
            // 
            // Btn_ClearSearch
            // 
            this.Btn_ClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_ClearSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ClearSearch.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_ClearSearch.Location = new System.Drawing.Point(1021, 17);
            this.Btn_ClearSearch.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_ClearSearch.Name = "Btn_ClearSearch";
            this.Btn_ClearSearch.Size = new System.Drawing.Size(47, 55);
            this.Btn_ClearSearch.TabIndex = 11;
            this.Btn_ClearSearch.UseVisualStyleBackColor = true;
            this.Btn_ClearSearch.Click += new System.EventHandler(this.Btn_ClearSearch_Click);
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.Location = new System.Drawing.Point(966, 17);
            this.Btn_Search.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(47, 55);
            this.Btn_Search.TabIndex = 10;
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Lbl_ValorBuscado
            // 
            this.Lbl_ValorBuscado.AutoSize = true;
            this.Lbl_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ValorBuscado.ForeColor = System.Drawing.Color.Black;
            this.Lbl_ValorBuscado.Location = new System.Drawing.Point(16, 12);
            this.Lbl_ValorBuscado.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_ValorBuscado.Name = "Lbl_ValorBuscado";
            this.Lbl_ValorBuscado.Size = new System.Drawing.Size(277, 20);
            this.Lbl_ValorBuscado.TabIndex = 9;
            this.Lbl_ValorBuscado.Text = "BUSCAR ARTÍCULO EN EL CATÁLOGO:";
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(20, 35);
            this.Txt_ValorBuscado.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_ValorBuscado.MaxLength = 100;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(923, 27);
            this.Txt_ValorBuscado.TabIndex = 9;
            this.Txt_ValorBuscado.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado_KeyDown);
            // 
            // PanelTabla_Resultados
            // 
            this.PanelTabla_Resultados.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla_Resultados.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla_Resultados.Controls.Add(this.Tabla_Resultados);
            this.PanelTabla_Resultados.Location = new System.Drawing.Point(16, 308);
            this.PanelTabla_Resultados.Margin = new System.Windows.Forms.Padding(4);
            this.PanelTabla_Resultados.Name = "PanelTabla_Resultados";
            this.PanelTabla_Resultados.Size = new System.Drawing.Size(1118, 280);
            this.PanelTabla_Resultados.TabIndex = 4;
            // 
            // Tabla_Resultados
            // 
            this.Tabla_Resultados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla_Resultados.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_ItemId,
            this.Col_Codigo,
            this.Col_Articulo,
            this.Col_StockActual,
            this.Col_Menos,
            this.Col_Cantidad,
            this.Col_Mas,
            this.Col_CostoUnitario,
            this.Col_Lote,
            this.Col_Caducidad,
            this.Col_Agregar});
            this.Tabla_Resultados.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla_Resultados.Location = new System.Drawing.Point(0, 0);
            this.Tabla_Resultados.Margin = new System.Windows.Forms.Padding(4);
            this.Tabla_Resultados.Name = "Tabla_Resultados";
            this.Tabla_Resultados.Size = new System.Drawing.Size(1118, 280);
            this.Tabla_Resultados.TabIndex = 0;
            this.Tabla_Resultados.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tabla_Resultados_CellClick);
            this.Tabla_Resultados.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tabla_Resultados_CellEndEdit);
            // 
            // Col_ItemId
            // 
            this.Col_ItemId.HeaderText = "ItemId";
            this.Col_ItemId.Name = "Col_ItemId";
            this.Col_ItemId.Visible = false;
            // 
            // Col_Codigo
            // 
            this.Col_Codigo.HeaderText = "CÓDIGO";
            this.Col_Codigo.Name = "Col_Codigo";
            this.Col_Codigo.ReadOnly = true;
            // 
            // Col_Articulo
            // 
            this.Col_Articulo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_Articulo.HeaderText = "ARTÍCULO";
            this.Col_Articulo.Name = "Col_Articulo";
            this.Col_Articulo.ReadOnly = true;
            // 
            // Col_StockActual
            // 
            this.Col_StockActual.HeaderText = "STOCK CENTRAL";
            this.Col_StockActual.Name = "Col_StockActual";
            this.Col_StockActual.ReadOnly = true;
            // 
            // Col_Menos
            // 
            this.Col_Menos.HeaderText = "";
            this.Col_Menos.Name = "Col_Menos";
            this.Col_Menos.Text = "−";
            this.Col_Menos.UseColumnTextForButtonValue = true;
            this.Col_Menos.Width = 35;
            // 
            // Col_Cantidad
            // 
            this.Col_Cantidad.HeaderText = "CANT.";
            this.Col_Cantidad.Name = "Col_Cantidad";
            this.Col_Cantidad.Width = 55;
            // 
            // Col_Mas
            // 
            this.Col_Mas.HeaderText = "";
            this.Col_Mas.Name = "Col_Mas";
            this.Col_Mas.Text = "+";
            this.Col_Mas.UseColumnTextForButtonValue = true;
            this.Col_Mas.Width = 35;
            // 
            // Col_CostoUnitario
            // 
            this.Col_CostoUnitario.HeaderText = "COSTO UNIT.";
            this.Col_CostoUnitario.Name = "Col_CostoUnitario";
            this.Col_CostoUnitario.Width = 90;
            // 
            // Col_Lote
            // 
            this.Col_Lote.HeaderText = "LOTE";
            this.Col_Lote.Name = "Col_Lote";
            this.Col_Lote.Width = 80;
            // 
            // Col_Caducidad
            // 
            this.Col_Caducidad.HeaderText = "CADUCIDAD";
            this.Col_Caducidad.Name = "Col_Caducidad";
            this.Col_Caducidad.Width = 95;
            // 
            // Col_Agregar
            // 
            this.Col_Agregar.HeaderText = "";
            this.Col_Agregar.Name = "Col_Agregar";
            this.Col_Agregar.Text = "AGREGAR";
            this.Col_Agregar.UseColumnTextForButtonValue = true;
            // 
            // PanelTabla_Ingreso
            // 
            this.PanelTabla_Ingreso.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla_Ingreso.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla_Ingreso.Controls.Add(this.Tabla_ListaIngreso);
            this.PanelTabla_Ingreso.Controls.Add(this.Lbl_Subtitulo2);
            this.PanelTabla_Ingreso.Location = new System.Drawing.Point(16, 596);
            this.PanelTabla_Ingreso.Margin = new System.Windows.Forms.Padding(4);
            this.PanelTabla_Ingreso.Name = "PanelTabla_Ingreso";
            this.PanelTabla_Ingreso.Size = new System.Drawing.Size(1118, 290);
            this.PanelTabla_Ingreso.TabIndex = 5;
            // 
            // Tabla_ListaIngreso
            // 
            this.Tabla_ListaIngreso.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla_ListaIngreso.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_Quitar});
            this.Tabla_ListaIngreso.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla_ListaIngreso.Location = new System.Drawing.Point(0, 33);
            this.Tabla_ListaIngreso.Margin = new System.Windows.Forms.Padding(4);
            this.Tabla_ListaIngreso.Name = "Tabla_ListaIngreso";
            this.Tabla_ListaIngreso.Size = new System.Drawing.Size(1118, 257);
            this.Tabla_ListaIngreso.TabIndex = 2;
            this.Tabla_ListaIngreso.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tabla_ListaIngreso_CellClick);
            // 
            // Col_Quitar
            // 
            this.Col_Quitar.HeaderText = "QUITAR";
            this.Col_Quitar.Name = "Col_Quitar";
            this.Col_Quitar.Text = "QUITAR";
            this.Col_Quitar.UseColumnTextForButtonValue = true;
            // 
            // Lbl_Subtitulo2
            // 
            this.Lbl_Subtitulo2.AutoSize = true;
            this.Lbl_Subtitulo2.Dock = System.Windows.Forms.DockStyle.Top;
            this.Lbl_Subtitulo2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Lbl_Subtitulo2.Location = new System.Drawing.Point(0, 0);
            this.Lbl_Subtitulo2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Lbl_Subtitulo2.Name = "Lbl_Subtitulo2";
            this.Lbl_Subtitulo2.Padding = new System.Windows.Forms.Padding(8, 8, 0, 4);
            this.Lbl_Subtitulo2.Size = new System.Drawing.Size(157, 33);
            this.Lbl_Subtitulo2.TabIndex = 1;
            this.Lbl_Subtitulo2.Text = "LISTA DE INGRESO";
            // 
            // Panel_CRUD
            // 
            this.Panel_CRUD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_CRUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_CRUD.Controls.Add(this.Btn_Cancelar);
            this.Panel_CRUD.Controls.Add(this.Btn_ConfirmarIngreso);
            this.Panel_CRUD.Location = new System.Drawing.Point(16, 894);
            this.Panel_CRUD.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_CRUD.Name = "Panel_CRUD";
            this.Panel_CRUD.Size = new System.Drawing.Size(1118, 56);
            this.Panel_CRUD.TabIndex = 6;
            // 
            // Btn_Cancelar
            // 
            this.Btn_Cancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Cancelar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Cancelar.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Cancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Cancelar.Location = new System.Drawing.Point(940, 6);
            this.Btn_Cancelar.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Cancelar.Name = "Btn_Cancelar";
            this.Btn_Cancelar.Size = new System.Drawing.Size(160, 44);
            this.Btn_Cancelar.TabIndex = 1;
            this.Btn_Cancelar.Text = "CANCELAR";
            this.Btn_Cancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Cancelar.UseVisualStyleBackColor = true;
            this.Btn_Cancelar.Click += new System.EventHandler(this.Btn_Cancelar_Click);
            // 
            // Btn_ConfirmarIngreso
            // 
            this.Btn_ConfirmarIngreso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_ConfirmarIngreso.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ConfirmarIngreso.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_ConfirmarIngreso.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_ConfirmarIngreso.Location = new System.Drawing.Point(710, 6);
            this.Btn_ConfirmarIngreso.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_ConfirmarIngreso.Name = "Btn_ConfirmarIngreso";
            this.Btn_ConfirmarIngreso.Size = new System.Drawing.Size(217, 44);
            this.Btn_ConfirmarIngreso.TabIndex = 0;
            this.Btn_ConfirmarIngreso.Text = "CONFIRMAR INGRESO";
            this.Btn_ConfirmarIngreso.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_ConfirmarIngreso.UseVisualStyleBackColor = true;
            this.Btn_ConfirmarIngreso.Click += new System.EventHandler(this.Btn_ConfirmarIngreso_Click);
            // 
            // Frm_KARDEX_WarehouseInventory_Ingreso
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1150, 966);
            this.Controls.Add(this.Panel_CRUD);
            this.Controls.Add(this.PanelTabla_Ingreso);
            this.Controls.Add(this.PanelTabla_Resultados);
            this.Controls.Add(this.Panel_Busqueda);
            this.Controls.Add(this.Panel_Encabezado);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(950, 700);
            this.Name = "Frm_KARDEX_WarehouseInventory_Ingreso";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SECRON - INGRESO DE MERCADERÍA";
            this.Load += new System.EventHandler(this.Frm_KARDEX_WarehouseInventory_Ingreso_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Encabezado.ResumeLayout(false);
            this.Panel_Encabezado.PerformLayout();
            this.Panel_Busqueda.ResumeLayout(false);
            this.Panel_Busqueda.PerformLayout();
            this.PanelTabla_Resultados.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla_Resultados)).EndInit();
            this.PanelTabla_Ingreso.ResumeLayout(false);
            this.PanelTabla_Ingreso.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla_ListaIngreso)).EndInit();
            this.Panel_CRUD.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_Encabezado;
        private System.Windows.Forms.Label Lbl_MovementType;
        private System.Windows.Forms.ComboBox ComboBox_MovementType;
        private System.Windows.Forms.Label Lbl_Proveedor;
        private System.Windows.Forms.TextBox Txt_Proveedor;
        private System.Windows.Forms.Button Btn_SearchSupplier;
        private System.Windows.Forms.Label Lbl_ReferenceDocument;
        private System.Windows.Forms.TextBox Txt_ReferenceDocument;
        private System.Windows.Forms.Label Lbl_PurchaseDate;
        private System.Windows.Forms.DateTimePicker DateTimePicker_PurchaseDate;
        private System.Windows.Forms.Panel Panel_Busqueda;
        private System.Windows.Forms.Button Btn_ClearSearch;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Label Lbl_ValorBuscado;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Panel PanelTabla_Resultados;
        private System.Windows.Forms.DataGridView Tabla_Resultados;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_ItemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Codigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Articulo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_StockActual;
        private System.Windows.Forms.DataGridViewButtonColumn Col_Menos;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Cantidad;
        private System.Windows.Forms.DataGridViewButtonColumn Col_Mas;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_CostoUnitario;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Lote;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Caducidad;
        private System.Windows.Forms.DataGridViewButtonColumn Col_Agregar;
        private System.Windows.Forms.Panel PanelTabla_Ingreso;
        private System.Windows.Forms.Label Lbl_Subtitulo2;
        private System.Windows.Forms.Panel Panel_CRUD;
        private System.Windows.Forms.Button Btn_Cancelar;
        private System.Windows.Forms.Button Btn_ConfirmarIngreso;
        private System.Windows.Forms.DataGridView Tabla_ListaIngreso;
        private System.Windows.Forms.DataGridViewButtonColumn Col_Quitar;
    }
}