namespace SECRON.Views
{
    partial class Frm_KARDEX_WarehouseInventory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_KARDEX_WarehouseInventory));
            this.Panel_Derecho = new System.Windows.Forms.Panel();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.PanelToolStrip = new System.Windows.Forms.Panel();
            this.Lbl_Paginas = new System.Windows.Forms.Label();
            this.Panel_Busqueda = new System.Windows.Forms.Panel();
            this.Btn_CleanSearch = new System.Windows.Forms.Button();
            this.Filtro3 = new System.Windows.Forms.ComboBox();
            this.Filtro2 = new System.Windows.Forms.ComboBox();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Filtro1 = new System.Windows.Forms.ComboBox();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Panel_Izquierdo = new System.Windows.Forms.Panel();
            this.ComboBox_Warehouse = new System.Windows.Forms.ComboBox();
            this.Lbl_Titulo = new System.Windows.Forms.Label();
            this.Panel_CRUD = new System.Windows.Forms.Panel();
            this.Btn_Ingreso = new System.Windows.Forms.Button();
            this.Btn_Despacho = new System.Windows.Forms.Button();
            this.Btn_Update = new System.Windows.Forms.Button();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Panel_2 = new System.Windows.Forms.Panel();
            this.Lbl_Subtitulo6 = new System.Windows.Forms.Label();
            this.Txt_ReorderPoint = new System.Windows.Forms.TextBox();
            this.Lbl_ReorderPoint = new System.Windows.Forms.Label();
            this.Txt_MaximumStock = new System.Windows.Forms.TextBox();
            this.Lbl_MaximumStock = new System.Windows.Forms.Label();
            this.Txt_MinimumStock = new System.Windows.Forms.TextBox();
            this.Lbl_MinimumStock = new System.Windows.Forms.Label();
            this.Txt_CurrentStock = new System.Windows.Forms.TextBox();
            this.Lbl_CurrentStock = new System.Windows.Forms.Label();
            this.Txt_MovementCounter = new System.Windows.Forms.TextBox();
            this.Lbl_MovementCounter = new System.Windows.Forms.Label();
            this.Txt_LastMovementDate = new System.Windows.Forms.TextBox();
            this.Lbl_LastMovementDate = new System.Windows.Forms.Label();
            this.Panel_1 = new System.Windows.Forms.Panel();
            this.Txt_Descripcion = new System.Windows.Forms.TextBox();
            this.Lbl_Subtitulo1 = new System.Windows.Forms.Label();
            this.Txt_Codigo = new System.Windows.Forms.TextBox();
            this.Lbl_Codigo = new System.Windows.Forms.Label();
            this.Txt_Articulo = new System.Windows.Forms.TextBox();
            this.Lbl_Articulo = new System.Windows.Forms.Label();
            this.Lbl_Descripcion = new System.Windows.Forms.Label();
            this.Txt_Category = new System.Windows.Forms.TextBox();
            this.Lbl_Category = new System.Windows.Forms.Label();
            this.Txt_MeasurementUnits = new System.Windows.Forms.TextBox();
            this.Lbl_MeasurementUnits = new System.Windows.Forms.Label();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Btn_Permisos = new System.Windows.Forms.Button();
            this.Btn_Export = new System.Windows.Forms.Button();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_Derecho.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.PanelToolStrip.SuspendLayout();
            this.Panel_Busqueda.SuspendLayout();
            this.Panel_Izquierdo.SuspendLayout();
            this.Panel_CRUD.SuspendLayout();
            this.Panel_2.SuspendLayout();
            this.Panel_1.SuspendLayout();
            this.Panel_Superior.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_Derecho
            // 
            this.Panel_Derecho.Controls.Add(this.PanelTabla);
            this.Panel_Derecho.Controls.Add(this.PanelToolStrip);
            this.Panel_Derecho.Controls.Add(this.Panel_Busqueda);
            this.Panel_Derecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Derecho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Derecho.ForeColor = System.Drawing.Color.Black;
            this.Panel_Derecho.Location = new System.Drawing.Point(581, 68);
            this.Panel_Derecho.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_Derecho.Name = "Panel_Derecho";
            this.Panel_Derecho.Size = new System.Drawing.Size(998, 992);
            this.Panel_Derecho.TabIndex = 14;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(16, 235);
            this.PanelTabla.Margin = new System.Windows.Forms.Padding(4);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(969, 708);
            this.PanelTabla.TabIndex = 75;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Margin = new System.Windows.Forms.Padding(4);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(969, 708);
            this.Tabla.TabIndex = 2;
            // 
            // PanelToolStrip
            // 
            this.PanelToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelToolStrip.Controls.Add(this.Lbl_Paginas);
            this.PanelToolStrip.Location = new System.Drawing.Point(16, 180);
            this.PanelToolStrip.Margin = new System.Windows.Forms.Padding(4);
            this.PanelToolStrip.Name = "PanelToolStrip";
            this.PanelToolStrip.Size = new System.Drawing.Size(969, 48);
            this.PanelToolStrip.TabIndex = 74;
            // 
            // Lbl_Paginas
            // 
            this.Lbl_Paginas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Paginas.AutoSize = true;
            this.Lbl_Paginas.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Paginas.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Paginas.Location = new System.Drawing.Point(16, 14);
            this.Lbl_Paginas.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Paginas.Name = "Lbl_Paginas";
            this.Lbl_Paginas.Size = new System.Drawing.Size(283, 20);
            this.Lbl_Paginas.TabIndex = 51;
            this.Lbl_Paginas.Text = "MOSTRANDO 1-10 DE 100 ARTÍCULOS";
            // 
            // Panel_Busqueda
            // 
            this.Panel_Busqueda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Busqueda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Busqueda.Controls.Add(this.Btn_CleanSearch);
            this.Panel_Busqueda.Controls.Add(this.Filtro3);
            this.Panel_Busqueda.Controls.Add(this.Filtro2);
            this.Panel_Busqueda.Controls.Add(this.Btn_Search);
            this.Panel_Busqueda.Controls.Add(this.Filtro1);
            this.Panel_Busqueda.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_Busqueda.Location = new System.Drawing.Point(16, 25);
            this.Panel_Busqueda.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_Busqueda.Name = "Panel_Busqueda";
            this.Panel_Busqueda.Size = new System.Drawing.Size(969, 148);
            this.Panel_Busqueda.TabIndex = 73;
            // 
            // Btn_CleanSearch
            // 
            this.Btn_CleanSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_CleanSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_CleanSearch.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_CleanSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_CleanSearch.Location = new System.Drawing.Point(902, 25);
            this.Btn_CleanSearch.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_CleanSearch.Name = "Btn_CleanSearch";
            this.Btn_CleanSearch.Size = new System.Drawing.Size(40, 38);
            this.Btn_CleanSearch.TabIndex = 71;
            this.Btn_CleanSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_CleanSearch.UseVisualStyleBackColor = true;
            this.Btn_CleanSearch.Click += new System.EventHandler(this.Btn_CleanSearch_Click);
            // 
            // Filtro3
            // 
            this.Filtro3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Filtro3.FormattingEnabled = true;
            this.Filtro3.Location = new System.Drawing.Point(648, 82);
            this.Filtro3.Margin = new System.Windows.Forms.Padding(4);
            this.Filtro3.Name = "Filtro3";
            this.Filtro3.Size = new System.Drawing.Size(291, 28);
            this.Filtro3.TabIndex = 70;
            // 
            // Filtro2
            // 
            this.Filtro2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Filtro2.FormattingEnabled = true;
            this.Filtro2.Location = new System.Drawing.Point(336, 82);
            this.Filtro2.Margin = new System.Windows.Forms.Padding(4);
            this.Filtro2.Name = "Filtro2";
            this.Filtro2.Size = new System.Drawing.Size(291, 28);
            this.Filtro2.TabIndex = 69;
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Search.Location = new System.Drawing.Point(758, 25);
            this.Btn_Search.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(135, 38);
            this.Btn_Search.TabIndex = 54;
            this.Btn_Search.Text = "BUSCAR";
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Filtro1
            // 
            this.Filtro1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Filtro1.FormattingEnabled = true;
            this.Filtro1.Location = new System.Drawing.Point(21, 82);
            this.Filtro1.Margin = new System.Windows.Forms.Padding(4);
            this.Filtro1.Name = "Filtro1";
            this.Filtro1.Size = new System.Drawing.Size(291, 28);
            this.Filtro1.TabIndex = 68;
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(21, 28);
            this.Txt_ValorBuscado.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_ValorBuscado.MaxLength = 100;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(721, 27);
            this.Txt_ValorBuscado.TabIndex = 59;
            this.Txt_ValorBuscado.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado_KeyDown);
            // 
            // Panel_Izquierdo
            // 
            this.Panel_Izquierdo.AutoScroll = true;
            this.Panel_Izquierdo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Izquierdo.Controls.Add(this.ComboBox_Warehouse);
            this.Panel_Izquierdo.Controls.Add(this.Lbl_Titulo);
            this.Panel_Izquierdo.Controls.Add(this.Panel_CRUD);
            this.Panel_Izquierdo.Controls.Add(this.Panel_2);
            this.Panel_Izquierdo.Controls.Add(this.Panel_1);
            this.Panel_Izquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.Panel_Izquierdo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Izquierdo.ForeColor = System.Drawing.Color.Black;
            this.Panel_Izquierdo.Location = new System.Drawing.Point(0, 68);
            this.Panel_Izquierdo.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_Izquierdo.Name = "Panel_Izquierdo";
            this.Panel_Izquierdo.Size = new System.Drawing.Size(581, 992);
            this.Panel_Izquierdo.TabIndex = 13;
            // 
            // ComboBox_Warehouse
            // 
            this.ComboBox_Warehouse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Warehouse.FormattingEnabled = true;
            this.ComboBox_Warehouse.Location = new System.Drawing.Point(154, 7);
            this.ComboBox_Warehouse.Margin = new System.Windows.Forms.Padding(4);
            this.ComboBox_Warehouse.Name = "ComboBox_Warehouse";
            this.ComboBox_Warehouse.Size = new System.Drawing.Size(404, 28);
            this.ComboBox_Warehouse.TabIndex = 99;
            // 
            // Lbl_Titulo
            // 
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Titulo.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Titulo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Titulo.Location = new System.Drawing.Point(5, 11);
            this.Lbl_Titulo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(75, 20);
            this.Lbl_Titulo.TabIndex = 98;
            this.Lbl_Titulo.Text = "BODEGA:";
            this.Lbl_Titulo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Panel_CRUD
            // 
            this.Panel_CRUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_CRUD.Controls.Add(this.Btn_Ingreso);
            this.Panel_CRUD.Controls.Add(this.Btn_Despacho);
            this.Panel_CRUD.Controls.Add(this.Btn_Update);
            this.Panel_CRUD.Controls.Add(this.Btn_Clear);
            this.Panel_CRUD.Location = new System.Drawing.Point(5, 49);
            this.Panel_CRUD.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_CRUD.Name = "Panel_CRUD";
            this.Panel_CRUD.Size = new System.Drawing.Size(555, 62);
            this.Panel_CRUD.TabIndex = 79;
            // 
            // Btn_Ingreso
            // 
            this.Btn_Ingreso.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_Ingreso.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Ingreso.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Ingreso.Location = new System.Drawing.Point(5, 6);
            this.Btn_Ingreso.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Ingreso.Name = "Btn_Ingreso";
            this.Btn_Ingreso.Size = new System.Drawing.Size(152, 46);
            this.Btn_Ingreso.TabIndex = 55;
            this.Btn_Ingreso.Text = "INGRESO";
            this.Btn_Ingreso.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Ingreso.UseVisualStyleBackColor = true;
            this.Btn_Ingreso.Click += new System.EventHandler(this.Btn_Ingreso_Click);
            // 
            // Btn_Despacho
            // 
            this.Btn_Despacho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_Despacho.Image = global::SECRON.Properties.Resources.KardexNegro25x25;
            this.Btn_Despacho.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Despacho.Location = new System.Drawing.Point(329, 6);
            this.Btn_Despacho.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Despacho.Name = "Btn_Despacho";
            this.Btn_Despacho.Size = new System.Drawing.Size(173, 46);
            this.Btn_Despacho.TabIndex = 57;
            this.Btn_Despacho.Text = "DESPACHO";
            this.Btn_Despacho.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Despacho.UseVisualStyleBackColor = true;
            this.Btn_Despacho.Click += new System.EventHandler(this.Btn_Despacho_Click);
            // 
            // Btn_Update
            // 
            this.Btn_Update.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_Update.Image = global::SECRON.Properties.Resources.UpdateAzul25x25;
            this.Btn_Update.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Update.Location = new System.Drawing.Point(165, 7);
            this.Btn_Update.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Update.Name = "Btn_Update";
            this.Btn_Update.Size = new System.Drawing.Size(152, 46);
            this.Btn_Update.TabIndex = 58;
            this.Btn_Update.Text = "EDITAR";
            this.Btn_Update.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Update.UseVisualStyleBackColor = true;
            this.Btn_Update.Click += new System.EventHandler(this.Btn_Update_Click);
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.Location = new System.Drawing.Point(506, 6);
            this.Btn_Clear.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(44, 46);
            this.Btn_Clear.TabIndex = 59;
            this.Btn_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            this.Btn_Clear.Click += new System.EventHandler(this.Btn_Clear_Click);
            // 
            // Panel_2
            // 
            this.Panel_2.BackColor = System.Drawing.Color.White;
            this.Panel_2.Controls.Add(this.Lbl_Subtitulo6);
            this.Panel_2.Controls.Add(this.Txt_ReorderPoint);
            this.Panel_2.Controls.Add(this.Lbl_ReorderPoint);
            this.Panel_2.Controls.Add(this.Txt_MaximumStock);
            this.Panel_2.Controls.Add(this.Lbl_MaximumStock);
            this.Panel_2.Controls.Add(this.Txt_MinimumStock);
            this.Panel_2.Controls.Add(this.Lbl_MinimumStock);
            this.Panel_2.Controls.Add(this.Txt_CurrentStock);
            this.Panel_2.Controls.Add(this.Lbl_CurrentStock);
            this.Panel_2.Controls.Add(this.Txt_MovementCounter);
            this.Panel_2.Controls.Add(this.Lbl_MovementCounter);
            this.Panel_2.Controls.Add(this.Txt_LastMovementDate);
            this.Panel_2.Controls.Add(this.Lbl_LastMovementDate);
            this.Panel_2.Location = new System.Drawing.Point(5, 449);
            this.Panel_2.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_2.Name = "Panel_2";
            this.Panel_2.Size = new System.Drawing.Size(557, 350);
            this.Panel_2.TabIndex = 78;
            // 
            // Lbl_Subtitulo6
            // 
            this.Lbl_Subtitulo6.AutoSize = true;
            this.Lbl_Subtitulo6.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo6.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo6.Location = new System.Drawing.Point(13, 12);
            this.Lbl_Subtitulo6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Subtitulo6.Name = "Lbl_Subtitulo6";
            this.Lbl_Subtitulo6.Size = new System.Drawing.Size(204, 21);
            this.Lbl_Subtitulo6.TabIndex = 80;
            this.Lbl_Subtitulo6.Text = "EXISTENCIAS EN BODEGA";
            // 
            // Txt_ReorderPoint
            // 
            this.Txt_ReorderPoint.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ReorderPoint.Location = new System.Drawing.Point(19, 225);
            this.Txt_ReorderPoint.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_ReorderPoint.MaxLength = 18;
            this.Txt_ReorderPoint.Name = "Txt_ReorderPoint";
            this.Txt_ReorderPoint.Size = new System.Drawing.Size(261, 27);
            this.Txt_ReorderPoint.TabIndex = 90;
            // 
            // Lbl_ReorderPoint
            // 
            this.Lbl_ReorderPoint.AutoSize = true;
            this.Lbl_ReorderPoint.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ReorderPoint.ForeColor = System.Drawing.Color.Black;
            this.Lbl_ReorderPoint.Location = new System.Drawing.Point(15, 197);
            this.Lbl_ReorderPoint.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_ReorderPoint.Name = "Lbl_ReorderPoint";
            this.Lbl_ReorderPoint.Size = new System.Drawing.Size(158, 20);
            this.Lbl_ReorderPoint.TabIndex = 89;
            this.Lbl_ReorderPoint.Text = "ALERTA DE PEDIDO *";
            // 
            // Txt_MaximumStock
            // 
            this.Txt_MaximumStock.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_MaximumStock.Location = new System.Drawing.Point(295, 150);
            this.Txt_MaximumStock.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_MaximumStock.MaxLength = 18;
            this.Txt_MaximumStock.Name = "Txt_MaximumStock";
            this.Txt_MaximumStock.Size = new System.Drawing.Size(245, 27);
            this.Txt_MaximumStock.TabIndex = 92;
            // 
            // Lbl_MaximumStock
            // 
            this.Lbl_MaximumStock.AutoSize = true;
            this.Lbl_MaximumStock.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_MaximumStock.ForeColor = System.Drawing.Color.Black;
            this.Lbl_MaximumStock.Location = new System.Drawing.Point(291, 122);
            this.Lbl_MaximumStock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_MaximumStock.Name = "Lbl_MaximumStock";
            this.Lbl_MaximumStock.Size = new System.Drawing.Size(124, 20);
            this.Lbl_MaximumStock.TabIndex = 91;
            this.Lbl_MaximumStock.Text = "STOCK MÁXIMO";
            // 
            // Txt_MinimumStock
            // 
            this.Txt_MinimumStock.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_MinimumStock.Location = new System.Drawing.Point(19, 150);
            this.Txt_MinimumStock.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_MinimumStock.MaxLength = 18;
            this.Txt_MinimumStock.Name = "Txt_MinimumStock";
            this.Txt_MinimumStock.Size = new System.Drawing.Size(261, 27);
            this.Txt_MinimumStock.TabIndex = 94;
            // 
            // Lbl_MinimumStock
            // 
            this.Lbl_MinimumStock.AutoSize = true;
            this.Lbl_MinimumStock.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_MinimumStock.ForeColor = System.Drawing.Color.Black;
            this.Lbl_MinimumStock.Location = new System.Drawing.Point(15, 122);
            this.Lbl_MinimumStock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_MinimumStock.Name = "Lbl_MinimumStock";
            this.Lbl_MinimumStock.Size = new System.Drawing.Size(120, 20);
            this.Lbl_MinimumStock.TabIndex = 93;
            this.Lbl_MinimumStock.Text = "STOCK MÍNIMO";
            // 
            // Txt_CurrentStock
            // 
            this.Txt_CurrentStock.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_CurrentStock.Location = new System.Drawing.Point(19, 75);
            this.Txt_CurrentStock.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_CurrentStock.MaxLength = 18;
            this.Txt_CurrentStock.Name = "Txt_CurrentStock";
            this.Txt_CurrentStock.ReadOnly = true;
            this.Txt_CurrentStock.Size = new System.Drawing.Size(261, 27);
            this.Txt_CurrentStock.TabIndex = 97;
            // 
            // Lbl_CurrentStock
            // 
            this.Lbl_CurrentStock.AutoSize = true;
            this.Lbl_CurrentStock.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_CurrentStock.ForeColor = System.Drawing.Color.Black;
            this.Lbl_CurrentStock.Location = new System.Drawing.Point(13, 47);
            this.Lbl_CurrentStock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_CurrentStock.Name = "Lbl_CurrentStock";
            this.Lbl_CurrentStock.Size = new System.Drawing.Size(118, 20);
            this.Lbl_CurrentStock.TabIndex = 98;
            this.Lbl_CurrentStock.Text = "STOCK ACTUAL";
            // 
            // Txt_MovementCounter
            // 
            this.Txt_MovementCounter.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_MovementCounter.Location = new System.Drawing.Point(295, 75);
            this.Txt_MovementCounter.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_MovementCounter.MaxLength = 10;
            this.Txt_MovementCounter.Name = "Txt_MovementCounter";
            this.Txt_MovementCounter.ReadOnly = true;
            this.Txt_MovementCounter.Size = new System.Drawing.Size(245, 27);
            this.Txt_MovementCounter.TabIndex = 96;
            // 
            // Lbl_MovementCounter
            // 
            this.Lbl_MovementCounter.AutoSize = true;
            this.Lbl_MovementCounter.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_MovementCounter.ForeColor = System.Drawing.Color.Black;
            this.Lbl_MovementCounter.Location = new System.Drawing.Point(291, 47);
            this.Lbl_MovementCounter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_MovementCounter.Name = "Lbl_MovementCounter";
            this.Lbl_MovementCounter.Size = new System.Drawing.Size(187, 20);
            this.Lbl_MovementCounter.TabIndex = 95;
            this.Lbl_MovementCounter.Text = "TOTAL DE MOVIMIENTOS";
            // 
            // Txt_LastMovementDate
            // 
            this.Txt_LastMovementDate.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_LastMovementDate.Location = new System.Drawing.Point(295, 225);
            this.Txt_LastMovementDate.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_LastMovementDate.MaxLength = 30;
            this.Txt_LastMovementDate.Name = "Txt_LastMovementDate";
            this.Txt_LastMovementDate.ReadOnly = true;
            this.Txt_LastMovementDate.Size = new System.Drawing.Size(245, 27);
            this.Txt_LastMovementDate.TabIndex = 88;
            // 
            // Lbl_LastMovementDate
            // 
            this.Lbl_LastMovementDate.AutoSize = true;
            this.Lbl_LastMovementDate.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_LastMovementDate.ForeColor = System.Drawing.Color.Black;
            this.Lbl_LastMovementDate.Location = new System.Drawing.Point(291, 197);
            this.Lbl_LastMovementDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_LastMovementDate.Name = "Lbl_LastMovementDate";
            this.Lbl_LastMovementDate.Size = new System.Drawing.Size(168, 20);
            this.Lbl_LastMovementDate.TabIndex = 87;
            this.Lbl_LastMovementDate.Text = "ÚLTIMO MOVIMIENTO";
            // 
            // Panel_1
            // 
            this.Panel_1.BackColor = System.Drawing.Color.White;
            this.Panel_1.Controls.Add(this.Txt_Descripcion);
            this.Panel_1.Controls.Add(this.Lbl_Subtitulo1);
            this.Panel_1.Controls.Add(this.Txt_Codigo);
            this.Panel_1.Controls.Add(this.Lbl_Codigo);
            this.Panel_1.Controls.Add(this.Txt_Articulo);
            this.Panel_1.Controls.Add(this.Lbl_Articulo);
            this.Panel_1.Controls.Add(this.Lbl_Descripcion);
            this.Panel_1.Controls.Add(this.Txt_Category);
            this.Panel_1.Controls.Add(this.Lbl_Category);
            this.Panel_1.Controls.Add(this.Txt_MeasurementUnits);
            this.Panel_1.Controls.Add(this.Lbl_MeasurementUnits);
            this.Panel_1.Location = new System.Drawing.Point(5, 116);
            this.Panel_1.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_1.Name = "Panel_1";
            this.Panel_1.Size = new System.Drawing.Size(557, 328);
            this.Panel_1.TabIndex = 77;
            // 
            // Txt_Descripcion
            // 
            this.Txt_Descripcion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Descripcion.Location = new System.Drawing.Point(19, 150);
            this.Txt_Descripcion.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_Descripcion.MaxLength = 15;
            this.Txt_Descripcion.Multiline = true;
            this.Txt_Descripcion.Name = "Txt_Descripcion";
            this.Txt_Descripcion.Size = new System.Drawing.Size(527, 99);
            this.Txt_Descripcion.TabIndex = 77;
            // 
            // Lbl_Subtitulo1
            // 
            this.Lbl_Subtitulo1.AutoSize = true;
            this.Lbl_Subtitulo1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo1.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo1.Location = new System.Drawing.Point(13, 12);
            this.Lbl_Subtitulo1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Subtitulo1.Name = "Lbl_Subtitulo1";
            this.Lbl_Subtitulo1.Size = new System.Drawing.Size(175, 21);
            this.Lbl_Subtitulo1.TabIndex = 76;
            this.Lbl_Subtitulo1.Text = "DATOS DEL ARTÍCULO";
            // 
            // Txt_Codigo
            // 
            this.Txt_Codigo.Enabled = false;
            this.Txt_Codigo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Codigo.Location = new System.Drawing.Point(19, 75);
            this.Txt_Codigo.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_Codigo.MaxLength = 50;
            this.Txt_Codigo.Name = "Txt_Codigo";
            this.Txt_Codigo.Size = new System.Drawing.Size(261, 27);
            this.Txt_Codigo.TabIndex = 60;
            // 
            // Lbl_Codigo
            // 
            this.Lbl_Codigo.AutoSize = true;
            this.Lbl_Codigo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Codigo.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Codigo.Location = new System.Drawing.Point(15, 47);
            this.Lbl_Codigo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Codigo.Name = "Lbl_Codigo";
            this.Lbl_Codigo.Size = new System.Drawing.Size(67, 20);
            this.Lbl_Codigo.TabIndex = 61;
            this.Lbl_Codigo.Text = "CÓDIGO";
            // 
            // Txt_Articulo
            // 
            this.Txt_Articulo.Enabled = false;
            this.Txt_Articulo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Articulo.Location = new System.Drawing.Point(295, 75);
            this.Txt_Articulo.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_Articulo.MaxLength = 200;
            this.Txt_Articulo.Name = "Txt_Articulo";
            this.Txt_Articulo.Size = new System.Drawing.Size(245, 27);
            this.Txt_Articulo.TabIndex = 62;
            // 
            // Lbl_Articulo
            // 
            this.Lbl_Articulo.AutoSize = true;
            this.Lbl_Articulo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Articulo.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Articulo.Location = new System.Drawing.Point(291, 47);
            this.Lbl_Articulo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Articulo.Name = "Lbl_Articulo";
            this.Lbl_Articulo.Size = new System.Drawing.Size(83, 20);
            this.Lbl_Articulo.TabIndex = 63;
            this.Lbl_Articulo.Text = "ARTÍCULO";
            // 
            // Lbl_Descripcion
            // 
            this.Lbl_Descripcion.AutoSize = true;
            this.Lbl_Descripcion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Descripcion.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Descripcion.Location = new System.Drawing.Point(15, 122);
            this.Lbl_Descripcion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Descripcion.Name = "Lbl_Descripcion";
            this.Lbl_Descripcion.Size = new System.Drawing.Size(106, 20);
            this.Lbl_Descripcion.TabIndex = 65;
            this.Lbl_Descripcion.Text = "DESCRIPCIÓN";
            // 
            // Txt_Category
            // 
            this.Txt_Category.Enabled = false;
            this.Txt_Category.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Category.Location = new System.Drawing.Point(19, 292);
            this.Txt_Category.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_Category.MaxLength = 100;
            this.Txt_Category.Name = "Txt_Category";
            this.Txt_Category.Size = new System.Drawing.Size(261, 27);
            this.Txt_Category.TabIndex = 66;
            // 
            // Lbl_Category
            // 
            this.Lbl_Category.AutoSize = true;
            this.Lbl_Category.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Category.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Category.Location = new System.Drawing.Point(15, 264);
            this.Lbl_Category.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Category.Name = "Lbl_Category";
            this.Lbl_Category.Size = new System.Drawing.Size(93, 20);
            this.Lbl_Category.TabIndex = 67;
            this.Lbl_Category.Text = "CATEGORÍA";
            // 
            // Txt_MeasurementUnits
            // 
            this.Txt_MeasurementUnits.Enabled = false;
            this.Txt_MeasurementUnits.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_MeasurementUnits.Location = new System.Drawing.Point(295, 292);
            this.Txt_MeasurementUnits.Margin = new System.Windows.Forms.Padding(4);
            this.Txt_MeasurementUnits.MaxLength = 50;
            this.Txt_MeasurementUnits.Name = "Txt_MeasurementUnits";
            this.Txt_MeasurementUnits.Size = new System.Drawing.Size(245, 27);
            this.Txt_MeasurementUnits.TabIndex = 53;
            // 
            // Lbl_MeasurementUnits
            // 
            this.Lbl_MeasurementUnits.AutoSize = true;
            this.Lbl_MeasurementUnits.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_MeasurementUnits.ForeColor = System.Drawing.Color.Black;
            this.Lbl_MeasurementUnits.Location = new System.Drawing.Point(291, 264);
            this.Lbl_MeasurementUnits.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_MeasurementUnits.Name = "Lbl_MeasurementUnits";
            this.Lbl_MeasurementUnits.Size = new System.Drawing.Size(157, 20);
            this.Lbl_MeasurementUnits.TabIndex = 52;
            this.Lbl_MeasurementUnits.Text = "UNIDAD DE MEDIDA";
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.Panel_Superior.Controls.Add(this.Btn_Permisos);
            this.Panel_Superior.Controls.Add(this.Btn_Export);
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1579, 68);
            this.Panel_Superior.TabIndex = 12;
            // 
            // Btn_Permisos
            // 
            this.Btn_Permisos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Permisos.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Permisos.Image = global::SECRON.Properties.Resources.AjustesNegro25x25;
            this.Btn_Permisos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Permisos.Location = new System.Drawing.Point(1238, 16);
            this.Btn_Permisos.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Permisos.Name = "Btn_Permisos";
            this.Btn_Permisos.Size = new System.Drawing.Size(159, 37);
            this.Btn_Permisos.TabIndex = 55;
            this.Btn_Permisos.Text = "PERMISOS";
            this.Btn_Permisos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Permisos.UseVisualStyleBackColor = true;
            this.Btn_Permisos.Click += new System.EventHandler(this.Btn_Permisos_Click);
            // 
            // Btn_Export
            // 
            this.Btn_Export.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Export.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Export.Image = global::SECRON.Properties.Resources.ExportarExcelNegro25x25;
            this.Btn_Export.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Export.Location = new System.Drawing.Point(1405, 15);
            this.Btn_Export.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Export.Name = "Btn_Export";
            this.Btn_Export.Size = new System.Drawing.Size(159, 37);
            this.Btn_Export.TabIndex = 54;
            this.Btn_Export.Text = "EXPORTAR";
            this.Btn_Export.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Export.UseVisualStyleBackColor = true;
            this.Btn_Export.Click += new System.EventHandler(this.Btn_Export_Click);
            // 
            // Lbl_Formulario
            // 
            this.Lbl_Formulario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Formulario.AutoSize = true;
            this.Lbl_Formulario.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.Lbl_Formulario.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Formulario.Location = new System.Drawing.Point(11, 16);
            this.Lbl_Formulario.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Formulario.Name = "Lbl_Formulario";
            this.Lbl_Formulario.Size = new System.Drawing.Size(373, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "CONTROL DE INVENTARIO POR BODEGA";
            // 
            // Frm_KARDEX_WarehouseInventory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1579, 1060);
            this.Controls.Add(this.Panel_Derecho);
            this.Controls.Add(this.Panel_Izquierdo);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Frm_KARDEX_WarehouseInventory";
            this.Text = "SECRON - CONTROL DE INVENTARIO POR BODEGA";
            this.Load += new System.EventHandler(this.Frm_KARDEX_WarehouseInventory_Load);
            this.Panel_Derecho.ResumeLayout(false);
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.PanelToolStrip.ResumeLayout(false);
            this.PanelToolStrip.PerformLayout();
            this.Panel_Busqueda.ResumeLayout(false);
            this.Panel_Busqueda.PerformLayout();
            this.Panel_Izquierdo.ResumeLayout(false);
            this.Panel_Izquierdo.PerformLayout();
            this.Panel_CRUD.ResumeLayout(false);
            this.Panel_2.ResumeLayout(false);
            this.Panel_2.PerformLayout();
            this.Panel_1.ResumeLayout(false);
            this.Panel_1.PerformLayout();
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Derecho;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.Panel PanelToolStrip;
        private System.Windows.Forms.Label Lbl_Paginas;
        private System.Windows.Forms.Panel Panel_Busqueda;
        private System.Windows.Forms.Button Btn_CleanSearch;
        private System.Windows.Forms.ComboBox Filtro3;
        private System.Windows.Forms.ComboBox Filtro2;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.ComboBox Filtro1;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Panel Panel_Izquierdo;
        private System.Windows.Forms.ComboBox ComboBox_Warehouse;
        private System.Windows.Forms.Label Lbl_Titulo;
        private System.Windows.Forms.Panel Panel_CRUD;
        private System.Windows.Forms.Button Btn_Ingreso;
        private System.Windows.Forms.Button Btn_Despacho;
        private System.Windows.Forms.Button Btn_Update;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Panel Panel_2;
        private System.Windows.Forms.Label Lbl_Subtitulo6;
        private System.Windows.Forms.TextBox Txt_ReorderPoint;
        private System.Windows.Forms.Label Lbl_ReorderPoint;
        private System.Windows.Forms.TextBox Txt_MaximumStock;
        private System.Windows.Forms.Label Lbl_MaximumStock;
        private System.Windows.Forms.TextBox Txt_MinimumStock;
        private System.Windows.Forms.Label Lbl_MinimumStock;
        private System.Windows.Forms.TextBox Txt_CurrentStock;
        private System.Windows.Forms.Label Lbl_CurrentStock;
        private System.Windows.Forms.TextBox Txt_MovementCounter;
        private System.Windows.Forms.Label Lbl_MovementCounter;
        private System.Windows.Forms.TextBox Txt_LastMovementDate;
        private System.Windows.Forms.Label Lbl_LastMovementDate;
        private System.Windows.Forms.Panel Panel_1;
        private System.Windows.Forms.Label Lbl_Subtitulo1;
        private System.Windows.Forms.TextBox Txt_Codigo;
        private System.Windows.Forms.Label Lbl_Codigo;
        private System.Windows.Forms.TextBox Txt_Articulo;
        private System.Windows.Forms.Label Lbl_Articulo;
        private System.Windows.Forms.Label Lbl_Descripcion;
        private System.Windows.Forms.TextBox Txt_Category;
        private System.Windows.Forms.Label Lbl_Category;
        private System.Windows.Forms.TextBox Txt_MeasurementUnits;
        private System.Windows.Forms.Label Lbl_MeasurementUnits;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Button Btn_Export;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.TextBox Txt_Descripcion;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Button Btn_Permisos;
    }
}