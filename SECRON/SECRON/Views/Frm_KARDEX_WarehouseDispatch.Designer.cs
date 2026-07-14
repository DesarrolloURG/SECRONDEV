namespace SECRON.Views
{
    partial class Frm_KARDEX_WarehouseDispatch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_KARDEX_WarehouseDispatch));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
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
            this.Col_StockDisponible = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Menos = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Col_Cantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Mas = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Col_Agregar = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Panel_Destino = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.RadioButton_Bodega = new System.Windows.Forms.RadioButton();
            this.RadioButton_Empleado = new System.Windows.Forms.RadioButton();
            this.Btn_SeleccionarDestino = new System.Windows.Forms.Button();
            this.Lbl_Seleccionado = new System.Windows.Forms.Label();
            this.Txt_Destino = new System.Windows.Forms.TextBox();
            this.PanelTabla_Despacho = new System.Windows.Forms.Panel();
            this.Tabla_ListaDespacho = new System.Windows.Forms.DataGridView();
            this.Col_StockDestino = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_MinimoDestino = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_ReorderDestino = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Quitar = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Lbl_Subtitulo2 = new System.Windows.Forms.Label();
            this.Panel_CRUD = new System.Windows.Forms.Panel();
            this.Btn_Cancelar = new System.Windows.Forms.Button();
            this.Btn_ConfirmarDespacho = new System.Windows.Forms.Button();
            this.Panel_Superior.SuspendLayout();
            this.Panel_Busqueda.SuspendLayout();
            this.PanelTabla_Resultados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla_Resultados)).BeginInit();
            this.Panel_Destino.SuspendLayout();
            this.PanelTabla_Despacho.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla_ListaDespacho)).BeginInit();
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
            this.Panel_Superior.Size = new System.Drawing.Size(1100, 68);
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(249, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "DESPACHO DE ARTÍCULOS";
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
            this.Panel_Busqueda.Location = new System.Drawing.Point(16, 158);
            this.Panel_Busqueda.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_Busqueda.Name = "Panel_Busqueda";
            this.Panel_Busqueda.Size = new System.Drawing.Size(1068, 96);
            this.Panel_Busqueda.TabIndex = 2;
            // 
            // Btn_ClearSearch
            // 
            this.Btn_ClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_ClearSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ClearSearch.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_ClearSearch.Location = new System.Drawing.Point(989, 28);
            this.Btn_ClearSearch.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_ClearSearch.Name = "Btn_ClearSearch";
            this.Btn_ClearSearch.Size = new System.Drawing.Size(44, 46);
            this.Btn_ClearSearch.TabIndex = 3;
            this.Btn_ClearSearch.UseVisualStyleBackColor = true;
            this.Btn_ClearSearch.Click += new System.EventHandler(this.Btn_ClearSearch_Click);
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.Location = new System.Drawing.Point(926, 28);
            this.Btn_Search.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(44, 46);
            this.Btn_Search.TabIndex = 2;
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
            this.Lbl_ValorBuscado.Size = new System.Drawing.Size(279, 20);
            this.Lbl_ValorBuscado.TabIndex = 1;
            this.Lbl_ValorBuscado.Text = "BUSCAR ARTÍCULO EN ESTA BODEGA:";
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
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(877, 27);
            this.Txt_ValorBuscado.TabIndex = 0;
            this.Txt_ValorBuscado.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado_KeyDown);
            // 
            // PanelTabla_Resultados
            // 
            this.PanelTabla_Resultados.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla_Resultados.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla_Resultados.Controls.Add(this.Tabla_Resultados);
            this.PanelTabla_Resultados.Location = new System.Drawing.Point(16, 261);
            this.PanelTabla_Resultados.Margin = new System.Windows.Forms.Padding(4);
            this.PanelTabla_Resultados.Name = "PanelTabla_Resultados";
            this.PanelTabla_Resultados.Size = new System.Drawing.Size(1068, 330);
            this.PanelTabla_Resultados.TabIndex = 3;
            // 
            // Tabla_Resultados
            // 
            this.Tabla_Resultados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla_Resultados.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_ItemId,
            this.Col_Codigo,
            this.Col_Articulo,
            this.Col_StockDisponible,
            this.Col_Menos,
            this.Col_Cantidad,
            this.Col_Mas,
            this.Col_Agregar});
            this.Tabla_Resultados.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla_Resultados.Location = new System.Drawing.Point(0, 0);
            this.Tabla_Resultados.Margin = new System.Windows.Forms.Padding(4);
            this.Tabla_Resultados.Name = "Tabla_Resultados";
            this.Tabla_Resultados.Size = new System.Drawing.Size(1068, 330);
            this.Tabla_Resultados.TabIndex = 2;
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
            this.Col_Codigo.Width = 110;
            // 
            // Col_Articulo
            // 
            this.Col_Articulo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_Articulo.HeaderText = "ARTÍCULO";
            this.Col_Articulo.MinimumWidth = 200;
            this.Col_Articulo.Name = "Col_Articulo";
            this.Col_Articulo.ReadOnly = true;
            // 
            // Col_StockDisponible
            // 
            this.Col_StockDisponible.HeaderText = "DISPONIBLE";
            this.Col_StockDisponible.Name = "Col_StockDisponible";
            this.Col_StockDisponible.ReadOnly = true;
            this.Col_StockDisponible.Width = 130;
            // 
            // Col_Menos
            // 
            this.Col_Menos.HeaderText = "";
            this.Col_Menos.Name = "Col_Menos";
            this.Col_Menos.Text = "−";
            this.Col_Menos.UseColumnTextForButtonValue = true;
            this.Col_Menos.Width = 20;
            // 
            // Col_Cantidad
            // 
            this.Col_Cantidad.HeaderText = "CANT.";
            this.Col_Cantidad.Name = "Col_Cantidad";
            this.Col_Cantidad.Width = 40;
            // 
            // Col_Mas
            // 
            this.Col_Mas.HeaderText = "";
            this.Col_Mas.Name = "Col_Mas";
            this.Col_Mas.Text = "+";
            this.Col_Mas.UseColumnTextForButtonValue = true;
            this.Col_Mas.Width = 20;
            // 
            // Col_Agregar
            // 
            this.Col_Agregar.HeaderText = "";
            this.Col_Agregar.Name = "Col_Agregar";
            this.Col_Agregar.Text = "AGREGAR";
            this.Col_Agregar.UseColumnTextForButtonValue = true;
            this.Col_Agregar.Width = 80;
            // 
            // Panel_Destino
            // 
            this.Panel_Destino.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Destino.BackColor = System.Drawing.Color.White;
            this.Panel_Destino.Controls.Add(this.label1);
            this.Panel_Destino.Controls.Add(this.RadioButton_Bodega);
            this.Panel_Destino.Controls.Add(this.RadioButton_Empleado);
            this.Panel_Destino.Controls.Add(this.Btn_SeleccionarDestino);
            this.Panel_Destino.Controls.Add(this.Lbl_Seleccionado);
            this.Panel_Destino.Controls.Add(this.Txt_Destino);
            this.Panel_Destino.Location = new System.Drawing.Point(16, 76);
            this.Panel_Destino.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_Destino.Name = "Panel_Destino";
            this.Panel_Destino.Size = new System.Drawing.Size(1068, 75);
            this.Panel_Destino.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "REALIZAR DESPACHO A:";
            // 
            // RadioButton_Bodega
            // 
            this.RadioButton_Bodega.AutoSize = true;
            this.RadioButton_Bodega.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.RadioButton_Bodega.Location = new System.Drawing.Point(146, 32);
            this.RadioButton_Bodega.Margin = new System.Windows.Forms.Padding(4);
            this.RadioButton_Bodega.Name = "RadioButton_Bodega";
            this.RadioButton_Bodega.Size = new System.Drawing.Size(84, 23);
            this.RadioButton_Bodega.TabIndex = 0;
            this.RadioButton_Bodega.Text = "BODEGA";
            this.RadioButton_Bodega.UseVisualStyleBackColor = true;
            this.RadioButton_Bodega.CheckedChanged += new System.EventHandler(this.RadioButton_Destino_CheckedChanged);
            // 
            // RadioButton_Empleado
            // 
            this.RadioButton_Empleado.AutoSize = true;
            this.RadioButton_Empleado.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.RadioButton_Empleado.Location = new System.Drawing.Point(18, 32);
            this.RadioButton_Empleado.Margin = new System.Windows.Forms.Padding(4);
            this.RadioButton_Empleado.Name = "RadioButton_Empleado";
            this.RadioButton_Empleado.Size = new System.Drawing.Size(101, 23);
            this.RadioButton_Empleado.TabIndex = 1;
            this.RadioButton_Empleado.Text = "EMPLEADO";
            this.RadioButton_Empleado.UseVisualStyleBackColor = true;
            this.RadioButton_Empleado.CheckedChanged += new System.EventHandler(this.RadioButton_Destino_CheckedChanged);
            // 
            // Btn_SeleccionarDestino
            // 
            this.Btn_SeleccionarDestino.Enabled = false;
            this.Btn_SeleccionarDestino.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.Btn_SeleccionarDestino.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_SeleccionarDestino.Location = new System.Drawing.Point(297, 16);
            this.Btn_SeleccionarDestino.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_SeleccionarDestino.Name = "Btn_SeleccionarDestino";
            this.Btn_SeleccionarDestino.Size = new System.Drawing.Size(44, 46);
            this.Btn_SeleccionarDestino.TabIndex = 2;
            this.Btn_SeleccionarDestino.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SeleccionarDestino.UseVisualStyleBackColor = true;
            this.Btn_SeleccionarDestino.Click += new System.EventHandler(this.Btn_SeleccionarDestino_Click);
            // 
            // Lbl_Seleccionado
            // 
            this.Lbl_Seleccionado.AutoSize = true;
            this.Lbl_Seleccionado.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.Lbl_Seleccionado.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Seleccionado.Location = new System.Drawing.Point(366, 35);
            this.Lbl_Seleccionado.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Seleccionado.Name = "Lbl_Seleccionado";
            this.Lbl_Seleccionado.Size = new System.Drawing.Size(117, 19);
            this.Lbl_Seleccionado.TabIndex = 3;
            this.Lbl_Seleccionado.Text = "SELECCIONADO:";
            // 
            // Txt_Destino
            // 
            this.Txt_Destino.Enabled = false;
            this.Txt_Destino.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.Txt_Destino.Location = new System.Drawing.Point(521, 32);
            this.Txt_Destino.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_Destino.Name = "Txt_Destino";
            this.Txt_Destino.Size = new System.Drawing.Size(500, 25);
            this.Txt_Destino.TabIndex = 4;
            // 
            // PanelTabla_Despacho
            // 
            this.PanelTabla_Despacho.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla_Despacho.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla_Despacho.Controls.Add(this.Tabla_ListaDespacho);
            this.PanelTabla_Despacho.Controls.Add(this.Lbl_Subtitulo2);
            this.PanelTabla_Despacho.Location = new System.Drawing.Point(16, 609);
            this.PanelTabla_Despacho.Margin = new System.Windows.Forms.Padding(4);
            this.PanelTabla_Despacho.Name = "PanelTabla_Despacho";
            this.PanelTabla_Despacho.Size = new System.Drawing.Size(1068, 265);
            this.PanelTabla_Despacho.TabIndex = 5;
            // 
            // Tabla_ListaDespacho
            // 
            this.Tabla_ListaDespacho.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla_ListaDespacho.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_StockDestino,
            this.Col_MinimoDestino,
            this.Col_ReorderDestino,
            this.Col_Quitar});
            this.Tabla_ListaDespacho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla_ListaDespacho.Location = new System.Drawing.Point(0, 33);
            this.Tabla_ListaDespacho.Margin = new System.Windows.Forms.Padding(4);
            this.Tabla_ListaDespacho.Name = "Tabla_ListaDespacho";
            this.Tabla_ListaDespacho.Size = new System.Drawing.Size(1068, 232);
            this.Tabla_ListaDespacho.TabIndex = 3;
            this.Tabla_ListaDespacho.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tabla_ListaDespacho_CellClick);
            // 
            // Col_StockDestino
            // 
            this.Col_StockDestino.HeaderText = "DISPONIBLE EN DESTINO";
            this.Col_StockDestino.Name = "Col_StockDestino";
            this.Col_StockDestino.ReadOnly = true;
            this.Col_StockDestino.Width = 110;
            // 
            // Col_MinimoDestino
            // 
            this.Col_MinimoDestino.HeaderText = "MÍNIMO EN DESTINO";
            this.Col_MinimoDestino.Name = "Col_MinimoDestino";
            this.Col_MinimoDestino.ReadOnly = true;
            this.Col_MinimoDestino.Width = 110;
            // 
            // Col_ReorderDestino
            // 
            this.Col_ReorderDestino.HeaderText = "REORDEN EN DESTINO";
            this.Col_ReorderDestino.Name = "Col_ReorderDestino";
            this.Col_ReorderDestino.ReadOnly = true;
            this.Col_ReorderDestino.Width = 120;
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
            this.Lbl_Subtitulo2.Size = new System.Drawing.Size(172, 33);
            this.Lbl_Subtitulo2.TabIndex = 1;
            this.Lbl_Subtitulo2.Text = "LISTA DE DESPACHO";
            // 
            // Panel_CRUD
            // 
            this.Panel_CRUD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_CRUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_CRUD.Controls.Add(this.Btn_Cancelar);
            this.Panel_CRUD.Controls.Add(this.Btn_ConfirmarDespacho);
            this.Panel_CRUD.Location = new System.Drawing.Point(16, 888);
            this.Panel_CRUD.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_CRUD.Name = "Panel_CRUD";
            this.Panel_CRUD.Size = new System.Drawing.Size(1068, 56);
            this.Panel_CRUD.TabIndex = 6;
            // 
            // Btn_Cancelar
            // 
            this.Btn_Cancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Cancelar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Cancelar.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Cancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Cancelar.Location = new System.Drawing.Point(890, 6);
            this.Btn_Cancelar.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Cancelar.Name = "Btn_Cancelar";
            this.Btn_Cancelar.Size = new System.Drawing.Size(160, 44);
            this.Btn_Cancelar.TabIndex = 1;
            this.Btn_Cancelar.Text = "CANCELAR";
            this.Btn_Cancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Cancelar.UseVisualStyleBackColor = true;
            this.Btn_Cancelar.Click += new System.EventHandler(this.Btn_Cancelar_Click);
            // 
            // Btn_ConfirmarDespacho
            // 
            this.Btn_ConfirmarDespacho.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_ConfirmarDespacho.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ConfirmarDespacho.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_ConfirmarDespacho.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_ConfirmarDespacho.Location = new System.Drawing.Point(665, 6);
            this.Btn_ConfirmarDespacho.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_ConfirmarDespacho.Name = "Btn_ConfirmarDespacho";
            this.Btn_ConfirmarDespacho.Size = new System.Drawing.Size(217, 44);
            this.Btn_ConfirmarDespacho.TabIndex = 0;
            this.Btn_ConfirmarDespacho.Text = "CONFIRMAR DESPACHO";
            this.Btn_ConfirmarDespacho.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_ConfirmarDespacho.UseVisualStyleBackColor = true;
            this.Btn_ConfirmarDespacho.Click += new System.EventHandler(this.Btn_ConfirmarDespacho_Click);
            // 
            // Frm_KARDEX_WarehouseDispatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 960);
            this.Controls.Add(this.Panel_CRUD);
            this.Controls.Add(this.Panel_Destino);
            this.Controls.Add(this.PanelTabla_Despacho);
            this.Controls.Add(this.PanelTabla_Resultados);
            this.Controls.Add(this.Panel_Busqueda);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(900, 700);
            this.Name = "Frm_KARDEX_WarehouseDispatch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SECRON - DESPACHO DE ARTÍCULOS";
            this.Load += new System.EventHandler(this.Frm_KARDEX_WarehouseDispatch_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Busqueda.ResumeLayout(false);
            this.Panel_Busqueda.PerformLayout();
            this.PanelTabla_Resultados.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla_Resultados)).EndInit();
            this.Panel_Destino.ResumeLayout(false);
            this.Panel_Destino.PerformLayout();
            this.PanelTabla_Despacho.ResumeLayout(false);
            this.PanelTabla_Despacho.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla_ListaDespacho)).EndInit();
            this.Panel_CRUD.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_Busqueda;
        private System.Windows.Forms.Button Btn_ClearSearch;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Label Lbl_ValorBuscado;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Panel PanelTabla_Resultados;
        private System.Windows.Forms.Panel PanelTabla_Despacho;
        private System.Windows.Forms.Label Lbl_Subtitulo2;
        private System.Windows.Forms.Panel Panel_CRUD;
        private System.Windows.Forms.Button Btn_Cancelar;
        private System.Windows.Forms.Button Btn_ConfirmarDespacho;
        private System.Windows.Forms.DataGridView Tabla_ListaDespacho;
        private System.Windows.Forms.DataGridViewButtonColumn Col_Quitar;
        private System.Windows.Forms.Panel Panel_Destino;
        private System.Windows.Forms.RadioButton RadioButton_Bodega;
        private System.Windows.Forms.RadioButton RadioButton_Empleado;
        private System.Windows.Forms.Button Btn_SeleccionarDestino;
        private System.Windows.Forms.Label Lbl_Seleccionado;
        private System.Windows.Forms.TextBox Txt_Destino;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView Tabla_Resultados;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_ItemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Codigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Articulo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_StockDisponible;

        private System.Windows.Forms.DataGridViewTextBoxColumn Col_StockDestino;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_MinimoDestino;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_ReorderDestino;

        private System.Windows.Forms.DataGridViewButtonColumn Col_Menos;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Cantidad;
        private System.Windows.Forms.DataGridViewButtonColumn Col_Mas;
        private System.Windows.Forms.DataGridViewButtonColumn Col_Agregar;
    }
}