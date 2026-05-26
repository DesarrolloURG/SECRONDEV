namespace SECRON.Views
{
    partial class Frm_FixedAsset_Movements
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_FixedAsset_Movements));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Btn_States = new System.Windows.Forms.Button();
            this.Btn_Export = new System.Windows.Forms.Button();
            this.Btn_Transfer = new System.Windows.Forms.Button();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_Derecho = new System.Windows.Forms.Panel();
            this.Panel_1 = new System.Windows.Forms.Panel();
            this.Txt_Reason = new System.Windows.Forms.TextBox();
            this.DTP_TransferDate = new System.Windows.Forms.DateTimePicker();
            this.Lbl_Reason = new System.Windows.Forms.Label();
            this.Lbl_TransferDate = new System.Windows.Forms.Label();
            this.Txt_TransferId = new System.Windows.Forms.TextBox();
            this.Lbl_TransferId = new System.Windows.Forms.Label();
            this.Lbl_Subtitulo1 = new System.Windows.Forms.Label();
            this.PanelToolStrip = new System.Windows.Forms.Panel();
            this.Lbl_Paginas = new System.Windows.Forms.Label();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Panel_Busqueda = new System.Windows.Forms.Panel();
            this.Btn_CleanSearch = new System.Windows.Forms.Button();
            this.Filtro3 = new System.Windows.Forms.ComboBox();
            this.Filtro2 = new System.Windows.Forms.ComboBox();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Filtro1 = new System.Windows.Forms.ComboBox();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Panel_Izquierdo = new System.Windows.Forms.Panel();
            this.Panel_CRUD = new System.Windows.Forms.Panel();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Update = new System.Windows.Forms.Button();
            this.Btn_AddAsset = new System.Windows.Forms.Button();
            this.Panel_2 = new System.Windows.Forms.Panel();
            this.ComboBox_ToEmployee = new System.Windows.Forms.ComboBox();
            this.Txt_FromEmployee = new System.Windows.Forms.TextBox();
            this.Lbl_ToEmployee = new System.Windows.Forms.Label();
            this.Btn_SearchAsset = new System.Windows.Forms.Button();
            this.ComboBox_ToWarehouse = new System.Windows.Forms.ComboBox();
            this.Lbl_FromEmployee = new System.Windows.Forms.Label();
            this.Lbl_ToWarehouse = new System.Windows.Forms.Label();
            this.Lbl_Subtitulo3 = new System.Windows.Forms.Label();
            this.ComboBox_SelectTo = new System.Windows.Forms.ComboBox();
            this.Txt_Asset = new System.Windows.Forms.TextBox();
            this.Lbl_Asset = new System.Windows.Forms.Label();
            this.Lbl_SelectTo = new System.Windows.Forms.Label();
            this.Txt_AssetId = new System.Windows.Forms.TextBox();
            this.Lbl_AssetId = new System.Windows.Forms.Label();
            this.Lbl_Subtitulo2 = new System.Windows.Forms.Label();
            this.Lbl_FromWarehouse = new System.Windows.Forms.Label();
            this.Txt_FromWarehouse = new System.Windows.Forms.TextBox();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.Btn_RemoveAsset = new System.Windows.Forms.Button();
            this.Panel_Superior.SuspendLayout();
            this.Panel_Derecho.SuspendLayout();
            this.Panel_1.SuspendLayout();
            this.PanelToolStrip.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.Panel_Busqueda.SuspendLayout();
            this.Panel_Izquierdo.SuspendLayout();
            this.Panel_CRUD.SuspendLayout();
            this.Panel_2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.Panel_Superior.Controls.Add(this.Btn_States);
            this.Panel_Superior.Controls.Add(this.Btn_Export);
            this.Panel_Superior.Controls.Add(this.Btn_Transfer);
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1155, 55);
            this.Panel_Superior.TabIndex = 5;
            // 
            // Btn_States
            // 
            this.Btn_States.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_States.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_States.Image = global::SECRON.Properties.Resources.ArticleNegro25x25;
            this.Btn_States.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_States.Location = new System.Drawing.Point(577, 12);
            this.Btn_States.Name = "Btn_States";
            this.Btn_States.Size = new System.Drawing.Size(209, 30);
            this.Btn_States.TabIndex = 56;
            this.Btn_States.Text = "ESTADOS DE TRASLADO";
            this.Btn_States.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_States.UseVisualStyleBackColor = true;
            this.Btn_States.Click += new System.EventHandler(this.Btn_States_Click);
            // 
            // Btn_Export
            // 
            this.Btn_Export.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Export.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_Export.Image = global::SECRON.Properties.Resources.ExportarExcelNegro25x25;
            this.Btn_Export.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Export.Location = new System.Drawing.Point(1014, 12);
            this.Btn_Export.Name = "Btn_Export";
            this.Btn_Export.Size = new System.Drawing.Size(119, 30);
            this.Btn_Export.TabIndex = 55;
            this.Btn_Export.Text = "EXPORTAR";
            this.Btn_Export.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Export.UseVisualStyleBackColor = true;
            this.Btn_Export.Click += new System.EventHandler(this.Btn_Export_Click);
            // 
            // Btn_Transfer
            // 
            this.Btn_Transfer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Transfer.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_Transfer.Image = global::SECRON.Properties.Resources.KardexNegro25x25;
            this.Btn_Transfer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Transfer.Location = new System.Drawing.Point(792, 13);
            this.Btn_Transfer.Name = "Btn_Transfer";
            this.Btn_Transfer.Size = new System.Drawing.Size(216, 30);
            this.Btn_Transfer.TabIndex = 54;
            this.Btn_Transfer.Text = "VISUALIZAR TRASLADOS";
            this.Btn_Transfer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Transfer.UseVisualStyleBackColor = true;
            this.Btn_Transfer.Click += new System.EventHandler(this.Btn_Transfer_Click);
            // 
            // Lbl_Formulario
            // 
            this.Lbl_Formulario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Formulario.AutoSize = true;
            this.Lbl_Formulario.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.Lbl_Formulario.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Formulario.Location = new System.Drawing.Point(8, 13);
            this.Lbl_Formulario.Name = "Lbl_Formulario";
            this.Lbl_Formulario.Size = new System.Drawing.Size(235, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "TRASLADOS DE ACTIVOS";
            // 
            // Panel_Derecho
            // 
            this.Panel_Derecho.Controls.Add(this.Panel_1);
            this.Panel_Derecho.Controls.Add(this.PanelToolStrip);
            this.Panel_Derecho.Controls.Add(this.PanelTabla);
            this.Panel_Derecho.Controls.Add(this.Panel_Busqueda);
            this.Panel_Derecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Derecho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Derecho.ForeColor = System.Drawing.Color.Black;
            this.Panel_Derecho.Location = new System.Drawing.Point(477, 55);
            this.Panel_Derecho.Name = "Panel_Derecho";
            this.Panel_Derecho.Size = new System.Drawing.Size(678, 632);
            this.Panel_Derecho.TabIndex = 12;
            // 
            // Panel_1
            // 
            this.Panel_1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_1.BackColor = System.Drawing.Color.White;
            this.Panel_1.Controls.Add(this.Txt_Reason);
            this.Panel_1.Controls.Add(this.DTP_TransferDate);
            this.Panel_1.Controls.Add(this.Lbl_Reason);
            this.Panel_1.Controls.Add(this.Lbl_TransferDate);
            this.Panel_1.Controls.Add(this.Txt_TransferId);
            this.Panel_1.Controls.Add(this.Lbl_TransferId);
            this.Panel_1.Controls.Add(this.Lbl_Subtitulo1);
            this.Panel_1.Location = new System.Drawing.Point(22, 407);
            this.Panel_1.Name = "Panel_1";
            this.Panel_1.Size = new System.Drawing.Size(634, 216);
            this.Panel_1.TabIndex = 64;
            // 
            // Txt_Reason
            // 
            this.Txt_Reason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_Reason.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Reason.Location = new System.Drawing.Point(14, 125);
            this.Txt_Reason.MaxLength = 15;
            this.Txt_Reason.Multiline = true;
            this.Txt_Reason.Name = "Txt_Reason";
            this.Txt_Reason.Size = new System.Drawing.Size(607, 82);
            this.Txt_Reason.TabIndex = 70;
            // 
            // DTP_TransferDate
            // 
            this.DTP_TransferDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DTP_TransferDate.Location = new System.Drawing.Point(309, 68);
            this.DTP_TransferDate.Name = "DTP_TransferDate";
            this.DTP_TransferDate.Size = new System.Drawing.Size(312, 27);
            this.DTP_TransferDate.TabIndex = 11;
            // 
            // Lbl_Reason
            // 
            this.Lbl_Reason.AutoSize = true;
            this.Lbl_Reason.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Reason.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Reason.Location = new System.Drawing.Point(10, 102);
            this.Lbl_Reason.Name = "Lbl_Reason";
            this.Lbl_Reason.Size = new System.Drawing.Size(79, 20);
            this.Lbl_Reason.TabIndex = 71;
            this.Lbl_Reason.Text = "MOTIVO *";
            // 
            // Lbl_TransferDate
            // 
            this.Lbl_TransferDate.AutoSize = true;
            this.Lbl_TransferDate.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_TransferDate.ForeColor = System.Drawing.Color.Black;
            this.Lbl_TransferDate.Location = new System.Drawing.Point(305, 45);
            this.Lbl_TransferDate.Name = "Lbl_TransferDate";
            this.Lbl_TransferDate.Size = new System.Drawing.Size(173, 20);
            this.Lbl_TransferDate.TabIndex = 11;
            this.Lbl_TransferDate.Text = "FECHA DE TRASLADO *";
            // 
            // Txt_TransferId
            // 
            this.Txt_TransferId.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_TransferId.Location = new System.Drawing.Point(14, 68);
            this.Txt_TransferId.MaxLength = 15;
            this.Txt_TransferId.Name = "Txt_TransferId";
            this.Txt_TransferId.Size = new System.Drawing.Size(273, 27);
            this.Txt_TransferId.TabIndex = 6;
            // 
            // Lbl_TransferId
            // 
            this.Lbl_TransferId.AutoSize = true;
            this.Lbl_TransferId.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_TransferId.ForeColor = System.Drawing.Color.Black;
            this.Lbl_TransferId.Location = new System.Drawing.Point(10, 45);
            this.Lbl_TransferId.Name = "Lbl_TransferId";
            this.Lbl_TransferId.Size = new System.Drawing.Size(173, 20);
            this.Lbl_TransferId.TabIndex = 6;
            this.Lbl_TransferId.Text = "CÓDIGO DE TRASLADO";
            // 
            // Lbl_Subtitulo1
            // 
            this.Lbl_Subtitulo1.AutoSize = true;
            this.Lbl_Subtitulo1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo1.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo1.Image = global::SECRON.Properties.Resources.InfoNegro20x20;
            this.Lbl_Subtitulo1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo1.Location = new System.Drawing.Point(10, 10);
            this.Lbl_Subtitulo1.Name = "Lbl_Subtitulo1";
            this.Lbl_Subtitulo1.Size = new System.Drawing.Size(217, 20);
            this.Lbl_Subtitulo1.TabIndex = 6;
            this.Lbl_Subtitulo1.Text = "      DETALLES DEL TRASLADO";
            this.Lbl_Subtitulo1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // PanelToolStrip
            // 
            this.PanelToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelToolStrip.Controls.Add(this.Lbl_Paginas);
            this.PanelToolStrip.Location = new System.Drawing.Point(22, 99);
            this.PanelToolStrip.Name = "PanelToolStrip";
            this.PanelToolStrip.Size = new System.Drawing.Size(634, 39);
            this.PanelToolStrip.TabIndex = 76;
            // 
            // Lbl_Paginas
            // 
            this.Lbl_Paginas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Paginas.AutoSize = true;
            this.Lbl_Paginas.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Paginas.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Paginas.Location = new System.Drawing.Point(12, 11);
            this.Lbl_Paginas.Name = "Lbl_Paginas";
            this.Lbl_Paginas.Size = new System.Drawing.Size(401, 20);
            this.Lbl_Paginas.TabIndex = 51;
            this.Lbl_Paginas.Text = "MOSTRANDO 1-10 DE 100 EN DETALLE DEL TRASLADO";
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(22, 144);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(634, 257);
            this.PanelTabla.TabIndex = 75;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.RowHeadersWidth = 51;
            this.Tabla.Size = new System.Drawing.Size(634, 257);
            this.Tabla.TabIndex = 1;
            this.Tabla.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tabla_CellClick);
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
            this.Panel_Busqueda.Location = new System.Drawing.Point(22, 6);
            this.Panel_Busqueda.Name = "Panel_Busqueda";
            this.Panel_Busqueda.Size = new System.Drawing.Size(634, 87);
            this.Panel_Busqueda.TabIndex = 73;
            // 
            // Btn_CleanSearch
            // 
            this.Btn_CleanSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_CleanSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_CleanSearch.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_CleanSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_CleanSearch.Location = new System.Drawing.Point(591, 6);
            this.Btn_CleanSearch.Name = "Btn_CleanSearch";
            this.Btn_CleanSearch.Size = new System.Drawing.Size(30, 31);
            this.Btn_CleanSearch.TabIndex = 71;
            this.Btn_CleanSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_CleanSearch.UseVisualStyleBackColor = true;
            this.Btn_CleanSearch.Click += new System.EventHandler(this.Btn_CleanSearch_Click);
            // 
            // Filtro3
            // 
            this.Filtro3.FormattingEnabled = true;
            this.Filtro3.Location = new System.Drawing.Point(426, 43);
            this.Filtro3.Name = "Filtro3";
            this.Filtro3.Size = new System.Drawing.Size(195, 28);
            this.Filtro3.TabIndex = 70;
            // 
            // Filtro2
            // 
            this.Filtro2.FormattingEnabled = true;
            this.Filtro2.Location = new System.Drawing.Point(222, 43);
            this.Filtro2.Name = "Filtro2";
            this.Filtro2.Size = new System.Drawing.Size(195, 28);
            this.Filtro2.TabIndex = 69;
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Search.Location = new System.Drawing.Point(484, 7);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(101, 31);
            this.Btn_Search.TabIndex = 54;
            this.Btn_Search.Text = "BUSCAR";
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Filtro1
            // 
            this.Filtro1.FormattingEnabled = true;
            this.Filtro1.Location = new System.Drawing.Point(16, 43);
            this.Filtro1.Name = "Filtro1";
            this.Filtro1.Size = new System.Drawing.Size(195, 28);
            this.Filtro1.TabIndex = 68;
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(16, 10);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(462, 27);
            this.Txt_ValorBuscado.TabIndex = 59;
            // 
            // Panel_Izquierdo
            // 
            this.Panel_Izquierdo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Izquierdo.Controls.Add(this.Panel_CRUD);
            this.Panel_Izquierdo.Controls.Add(this.Panel_2);
            this.Panel_Izquierdo.Controls.Add(this.vScrollBar);
            this.Panel_Izquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.Panel_Izquierdo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Izquierdo.ForeColor = System.Drawing.Color.Black;
            this.Panel_Izquierdo.Location = new System.Drawing.Point(0, 55);
            this.Panel_Izquierdo.MaximumSize = new System.Drawing.Size(477, 806);
            this.Panel_Izquierdo.Name = "Panel_Izquierdo";
            this.Panel_Izquierdo.Size = new System.Drawing.Size(477, 632);
            this.Panel_Izquierdo.TabIndex = 11;
            // 
            // Panel_CRUD
            // 
            this.Panel_CRUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_CRUD.Controls.Add(this.Btn_RemoveAsset);
            this.Panel_CRUD.Controls.Add(this.Btn_Clear);
            this.Panel_CRUD.Controls.Add(this.Btn_Update);
            this.Panel_CRUD.Controls.Add(this.Btn_AddAsset);
            this.Panel_CRUD.Location = new System.Drawing.Point(13, 13);
            this.Panel_CRUD.Name = "Panel_CRUD";
            this.Panel_CRUD.Size = new System.Drawing.Size(376, 47);
            this.Panel_CRUD.TabIndex = 81;
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Clear.Location = new System.Drawing.Point(333, 3);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(33, 37);
            this.Btn_Clear.TabIndex = 57;
            this.Btn_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            this.Btn_Clear.Click += new System.EventHandler(this.Btn_Clear_Click);
            // 
            // Btn_Update
            // 
            this.Btn_Update.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Update.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Update.Image = global::SECRON.Properties.Resources.UpdateAzul25x25;
            this.Btn_Update.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Update.Location = new System.Drawing.Point(228, 3);
            this.Btn_Update.Name = "Btn_Update";
            this.Btn_Update.Size = new System.Drawing.Size(99, 37);
            this.Btn_Update.TabIndex = 55;
            this.Btn_Update.Text = "EDITAR";
            this.Btn_Update.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Update.UseVisualStyleBackColor = true;
            this.Btn_Update.Click += new System.EventHandler(this.Btn_Update_Click);
            // 
            // Btn_AddAsset
            // 
            this.Btn_AddAsset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_AddAsset.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_AddAsset.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_AddAsset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_AddAsset.Location = new System.Drawing.Point(4, 3);
            this.Btn_AddAsset.Name = "Btn_AddAsset";
            this.Btn_AddAsset.Size = new System.Drawing.Size(113, 37);
            this.Btn_AddAsset.TabIndex = 54;
            this.Btn_AddAsset.Text = "AGREGAR";
            this.Btn_AddAsset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_AddAsset.UseVisualStyleBackColor = true;
            this.Btn_AddAsset.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Panel_2
            // 
            this.Panel_2.BackColor = System.Drawing.Color.White;
            this.Panel_2.Controls.Add(this.ComboBox_ToEmployee);
            this.Panel_2.Controls.Add(this.Txt_FromEmployee);
            this.Panel_2.Controls.Add(this.Lbl_ToEmployee);
            this.Panel_2.Controls.Add(this.Btn_SearchAsset);
            this.Panel_2.Controls.Add(this.ComboBox_ToWarehouse);
            this.Panel_2.Controls.Add(this.Lbl_FromEmployee);
            this.Panel_2.Controls.Add(this.Lbl_ToWarehouse);
            this.Panel_2.Controls.Add(this.Lbl_Subtitulo3);
            this.Panel_2.Controls.Add(this.ComboBox_SelectTo);
            this.Panel_2.Controls.Add(this.Txt_Asset);
            this.Panel_2.Controls.Add(this.Lbl_Asset);
            this.Panel_2.Controls.Add(this.Lbl_SelectTo);
            this.Panel_2.Controls.Add(this.Txt_AssetId);
            this.Panel_2.Controls.Add(this.Lbl_AssetId);
            this.Panel_2.Controls.Add(this.Lbl_Subtitulo2);
            this.Panel_2.Controls.Add(this.Lbl_FromWarehouse);
            this.Panel_2.Controls.Add(this.Txt_FromWarehouse);
            this.Panel_2.Location = new System.Drawing.Point(13, 66);
            this.Panel_2.Name = "Panel_2";
            this.Panel_2.Size = new System.Drawing.Size(451, 557);
            this.Panel_2.TabIndex = 83;
            // 
            // ComboBox_ToEmployee
            // 
            this.ComboBox_ToEmployee.FormattingEnabled = true;
            this.ComboBox_ToEmployee.Location = new System.Drawing.Point(15, 466);
            this.ComboBox_ToEmployee.Name = "ComboBox_ToEmployee";
            this.ComboBox_ToEmployee.Size = new System.Drawing.Size(424, 28);
            this.ComboBox_ToEmployee.TabIndex = 77;
            // 
            // Txt_FromEmployee
            // 
            this.Txt_FromEmployee.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_FromEmployee.Location = new System.Drawing.Point(14, 409);
            this.Txt_FromEmployee.MaxLength = 15;
            this.Txt_FromEmployee.Name = "Txt_FromEmployee";
            this.Txt_FromEmployee.Size = new System.Drawing.Size(425, 27);
            this.Txt_FromEmployee.TabIndex = 78;
            // 
            // Lbl_ToEmployee
            // 
            this.Lbl_ToEmployee.AutoSize = true;
            this.Lbl_ToEmployee.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ToEmployee.ForeColor = System.Drawing.Color.Black;
            this.Lbl_ToEmployee.Location = new System.Drawing.Point(10, 443);
            this.Lbl_ToEmployee.Name = "Lbl_ToEmployee";
            this.Lbl_ToEmployee.Size = new System.Drawing.Size(103, 20);
            this.Lbl_ToEmployee.TabIndex = 76;
            this.Lbl_ToEmployee.Text = "ASIGNAR A *";
            // 
            // Btn_SearchAsset
            // 
            this.Btn_SearchAsset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_SearchAsset.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SearchAsset.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_SearchAsset.Location = new System.Drawing.Point(404, 10);
            this.Btn_SearchAsset.Name = "Btn_SearchAsset";
            this.Btn_SearchAsset.Size = new System.Drawing.Size(35, 45);
            this.Btn_SearchAsset.TabIndex = 72;
            this.Btn_SearchAsset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SearchAsset.UseVisualStyleBackColor = true;
            this.Btn_SearchAsset.Click += new System.EventHandler(this.Btn_SearchAsset_Click);
            // 
            // ComboBox_ToWarehouse
            // 
            this.ComboBox_ToWarehouse.FormattingEnabled = true;
            this.ComboBox_ToWarehouse.Location = new System.Drawing.Point(14, 241);
            this.ComboBox_ToWarehouse.Name = "ComboBox_ToWarehouse";
            this.ComboBox_ToWarehouse.Size = new System.Drawing.Size(423, 28);
            this.ComboBox_ToWarehouse.TabIndex = 75;
            // 
            // Lbl_FromEmployee
            // 
            this.Lbl_FromEmployee.AutoSize = true;
            this.Lbl_FromEmployee.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_FromEmployee.ForeColor = System.Drawing.Color.Black;
            this.Lbl_FromEmployee.Location = new System.Drawing.Point(10, 386);
            this.Lbl_FromEmployee.Name = "Lbl_FromEmployee";
            this.Lbl_FromEmployee.Size = new System.Drawing.Size(115, 20);
            this.Lbl_FromEmployee.TabIndex = 79;
            this.Lbl_FromEmployee.Text = "ASIGNADO A *";
            // 
            // Lbl_ToWarehouse
            // 
            this.Lbl_ToWarehouse.AutoSize = true;
            this.Lbl_ToWarehouse.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ToWarehouse.ForeColor = System.Drawing.Color.Black;
            this.Lbl_ToWarehouse.Location = new System.Drawing.Point(10, 218);
            this.Lbl_ToWarehouse.Name = "Lbl_ToWarehouse";
            this.Lbl_ToWarehouse.Size = new System.Drawing.Size(173, 20);
            this.Lbl_ToWarehouse.TabIndex = 71;
            this.Lbl_ToWarehouse.Text = "BODEGA DE DESTINO *";
            // 
            // Lbl_Subtitulo3
            // 
            this.Lbl_Subtitulo3.AutoSize = true;
            this.Lbl_Subtitulo3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo3.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo3.Image = global::SECRON.Properties.Resources.LocationNegro20x20;
            this.Lbl_Subtitulo3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo3.Location = new System.Drawing.Point(11, 285);
            this.Lbl_Subtitulo3.Name = "Lbl_Subtitulo3";
            this.Lbl_Subtitulo3.Size = new System.Drawing.Size(168, 20);
            this.Lbl_Subtitulo3.TabIndex = 11;
            this.Lbl_Subtitulo3.Text = "      ORIGEN - DESTINO";
            this.Lbl_Subtitulo3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ComboBox_SelectTo
            // 
            this.ComboBox_SelectTo.FormattingEnabled = true;
            this.ComboBox_SelectTo.Location = new System.Drawing.Point(14, 180);
            this.ComboBox_SelectTo.Name = "ComboBox_SelectTo";
            this.ComboBox_SelectTo.Size = new System.Drawing.Size(423, 28);
            this.ComboBox_SelectTo.TabIndex = 74;
            this.ComboBox_SelectTo.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SelectTo_SelectedIndexChanged);
            // 
            // Txt_Asset
            // 
            this.Txt_Asset.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Asset.Location = new System.Drawing.Point(14, 123);
            this.Txt_Asset.MaxLength = 15;
            this.Txt_Asset.Name = "Txt_Asset";
            this.Txt_Asset.Size = new System.Drawing.Size(422, 27);
            this.Txt_Asset.TabIndex = 71;
            // 
            // Lbl_Asset
            // 
            this.Lbl_Asset.AutoSize = true;
            this.Lbl_Asset.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Asset.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Asset.Location = new System.Drawing.Point(10, 100);
            this.Lbl_Asset.Name = "Lbl_Asset";
            this.Lbl_Asset.Size = new System.Drawing.Size(164, 20);
            this.Lbl_Asset.TabIndex = 70;
            this.Lbl_Asset.Text = "NOMBRE DEL ACTIVO";
            // 
            // Lbl_SelectTo
            // 
            this.Lbl_SelectTo.AutoSize = true;
            this.Lbl_SelectTo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_SelectTo.ForeColor = System.Drawing.Color.Black;
            this.Lbl_SelectTo.Location = new System.Drawing.Point(10, 157);
            this.Lbl_SelectTo.Name = "Lbl_SelectTo";
            this.Lbl_SelectTo.Size = new System.Drawing.Size(258, 20);
            this.Lbl_SelectTo.TabIndex = 56;
            this.Lbl_SelectTo.Text = "SELECCIONA EL TIPO DE DESTINO *";
            // 
            // Txt_AssetId
            // 
            this.Txt_AssetId.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_AssetId.Location = new System.Drawing.Point(14, 67);
            this.Txt_AssetId.MaxLength = 15;
            this.Txt_AssetId.Name = "Txt_AssetId";
            this.Txt_AssetId.Size = new System.Drawing.Size(423, 27);
            this.Txt_AssetId.TabIndex = 6;
            // 
            // Lbl_AssetId
            // 
            this.Lbl_AssetId.AutoSize = true;
            this.Lbl_AssetId.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_AssetId.ForeColor = System.Drawing.Color.Black;
            this.Lbl_AssetId.Location = new System.Drawing.Point(10, 44);
            this.Lbl_AssetId.Name = "Lbl_AssetId";
            this.Lbl_AssetId.Size = new System.Drawing.Size(157, 20);
            this.Lbl_AssetId.TabIndex = 6;
            this.Lbl_AssetId.Text = "CÓDIGO DEL ACTIVO";
            // 
            // Lbl_Subtitulo2
            // 
            this.Lbl_Subtitulo2.AutoSize = true;
            this.Lbl_Subtitulo2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo2.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo2.Image = global::SECRON.Properties.Resources.InventoryNegro25x25;
            this.Lbl_Subtitulo2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo2.Location = new System.Drawing.Point(10, 10);
            this.Lbl_Subtitulo2.Name = "Lbl_Subtitulo2";
            this.Lbl_Subtitulo2.Size = new System.Drawing.Size(193, 20);
            this.Lbl_Subtitulo2.TabIndex = 6;
            this.Lbl_Subtitulo2.Text = "      DETALLES DEL ACTIVO";
            this.Lbl_Subtitulo2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Lbl_FromWarehouse
            // 
            this.Lbl_FromWarehouse.AutoSize = true;
            this.Lbl_FromWarehouse.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_FromWarehouse.ForeColor = System.Drawing.Color.Black;
            this.Lbl_FromWarehouse.Location = new System.Drawing.Point(10, 324);
            this.Lbl_FromWarehouse.Name = "Lbl_FromWarehouse";
            this.Lbl_FromWarehouse.Size = new System.Drawing.Size(166, 20);
            this.Lbl_FromWarehouse.TabIndex = 69;
            this.Lbl_FromWarehouse.Text = "BODEGA DE ORIGEN *";
            // 
            // Txt_FromWarehouse
            // 
            this.Txt_FromWarehouse.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_FromWarehouse.Location = new System.Drawing.Point(14, 347);
            this.Txt_FromWarehouse.MaxLength = 15;
            this.Txt_FromWarehouse.Name = "Txt_FromWarehouse";
            this.Txt_FromWarehouse.Size = new System.Drawing.Size(425, 27);
            this.Txt_FromWarehouse.TabIndex = 68;
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(467, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(10, 632);
            this.vScrollBar.TabIndex = 80;
            // 
            // Btn_RemoveAsset
            // 
            this.Btn_RemoveAsset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_RemoveAsset.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_RemoveAsset.Image = global::SECRON.Properties.Resources.UpdateAzul25x25;
            this.Btn_RemoveAsset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_RemoveAsset.Location = new System.Drawing.Point(123, 3);
            this.Btn_RemoveAsset.Name = "Btn_RemoveAsset";
            this.Btn_RemoveAsset.Size = new System.Drawing.Size(99, 37);
            this.Btn_RemoveAsset.TabIndex = 58;
            this.Btn_RemoveAsset.Text = "EDITAR";
            this.Btn_RemoveAsset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_RemoveAsset.UseVisualStyleBackColor = true;
            // 
            // Frm_FixedAsset_Movements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1155, 687);
            this.Controls.Add(this.Panel_Derecho);
            this.Controls.Add(this.Panel_Izquierdo);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_FixedAsset_Movements";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - TRASLADOS DE ACTIVOS";
            this.Load += new System.EventHandler(this.Frm_FixedAsset_Movements_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Derecho.ResumeLayout(false);
            this.Panel_1.ResumeLayout(false);
            this.Panel_1.PerformLayout();
            this.PanelToolStrip.ResumeLayout(false);
            this.PanelToolStrip.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.Panel_Busqueda.ResumeLayout(false);
            this.Panel_Busqueda.PerformLayout();
            this.Panel_Izquierdo.ResumeLayout(false);
            this.Panel_CRUD.ResumeLayout(false);
            this.Panel_2.ResumeLayout(false);
            this.Panel_2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Button Btn_States;
        private System.Windows.Forms.Button Btn_Export;
        private System.Windows.Forms.Button Btn_Transfer;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_Derecho;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel Panel_Busqueda;
        private System.Windows.Forms.Button Btn_CleanSearch;
        private System.Windows.Forms.ComboBox Filtro3;
        private System.Windows.Forms.ComboBox Filtro2;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.ComboBox Filtro1;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Panel Panel_Izquierdo;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.Label Lbl_ToWarehouse;
        private System.Windows.Forms.Label Lbl_FromWarehouse;
        private System.Windows.Forms.Label Lbl_SelectTo;
        private System.Windows.Forms.Panel Panel_1;
        private System.Windows.Forms.DateTimePicker DTP_TransferDate;
        private System.Windows.Forms.Label Lbl_TransferDate;
        private System.Windows.Forms.TextBox Txt_TransferId;
        private System.Windows.Forms.Label Lbl_TransferId;
        private System.Windows.Forms.Label Lbl_Subtitulo1;
        private System.Windows.Forms.ComboBox ComboBox_SelectTo;
        private System.Windows.Forms.Panel Panel_CRUD;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Update;
        private System.Windows.Forms.Button Btn_AddAsset;
        private System.Windows.Forms.Panel PanelToolStrip;
        private System.Windows.Forms.Label Lbl_Paginas;
        private System.Windows.Forms.TextBox Txt_Reason;
        private System.Windows.Forms.Label Lbl_Reason;
        private System.Windows.Forms.TextBox Txt_FromEmployee;
        private System.Windows.Forms.Label Lbl_FromEmployee;
        private System.Windows.Forms.ComboBox ComboBox_ToEmployee;
        private System.Windows.Forms.Label Lbl_ToEmployee;
        private System.Windows.Forms.ComboBox ComboBox_ToWarehouse;
        private System.Windows.Forms.Panel Panel_2;
        private System.Windows.Forms.TextBox Txt_Asset;
        private System.Windows.Forms.Label Lbl_Asset;
        private System.Windows.Forms.TextBox Txt_AssetId;
        private System.Windows.Forms.Label Lbl_AssetId;
        private System.Windows.Forms.Label Lbl_Subtitulo2;
        private System.Windows.Forms.Button Btn_SearchAsset;
        private System.Windows.Forms.TextBox Txt_FromWarehouse;
        private System.Windows.Forms.Label Lbl_Subtitulo3;
        private System.Windows.Forms.Button Btn_RemoveAsset;
    }
}