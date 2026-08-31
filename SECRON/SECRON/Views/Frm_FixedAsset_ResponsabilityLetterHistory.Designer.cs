namespace SECRON.Views
{
    partial class Frm_FixedAsset_ResponsabilityLetterHistory
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_FixedAsset_ResponsabilityLetterHistory));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_Busqueda = new System.Windows.Forms.Panel();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Btn_CleanSearch = new System.Windows.Forms.Button();
            this.Filtro1 = new System.Windows.Forms.ComboBox();
            this.FiltroEstado = new System.Windows.Forms.ComboBox();
            this.PanelToolStrip = new System.Windows.Forms.Panel();
            this.Lbl_Paginas = new System.Windows.Forms.Label();
            this.Panel_BusquedaAtributos = new System.Windows.Forms.Panel();
            this.Txt_BuscarAtributo = new System.Windows.Forms.TextBox();
            this.FiltroAtributoTipo = new System.Windows.Forms.ComboBox();
            this.Btn_SearchAtributo = new System.Windows.Forms.Button();
            this.Btn_CleanSearchAtributo = new System.Windows.Forms.Button();
            this.Lbl_AtributosHeader = new System.Windows.Forms.Label();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TablaAtributos = new System.Windows.Forms.DataGridView();
            this.Panel_AtributosCRUD = new System.Windows.Forms.Panel();
            this.Txt_AttributeValue = new System.Windows.Forms.TextBox();
            this.Txt_AttributeType = new System.Windows.Forms.TextBox();
            this.Lbl_SubtituloAtributos = new System.Windows.Forms.Label();
            this.Lbl_AttributeKey = new System.Windows.Forms.Label();
            this.Txt_AttributeKey = new System.Windows.Forms.TextBox();
            this.Lbl_AttributeLabel = new System.Windows.Forms.Label();
            this.Txt_AttributeLabel = new System.Windows.Forms.TextBox();
            this.Lbl_DataType = new System.Windows.Forms.Label();
            this.Lbl_IsRequired = new System.Windows.Forms.Label();
            this.Panel_AtributosBotones = new System.Windows.Forms.Panel();
            this.Btn_UpdateAtributo = new System.Windows.Forms.Button();
            this.Btn_ClearAtributo = new System.Windows.Forms.Button();
            this.Panel_Derecho = new System.Windows.Forms.Panel();
            this.Panel_Superior.SuspendLayout();
            this.Panel_Busqueda.SuspendLayout();
            this.PanelToolStrip.SuspendLayout();
            this.Panel_BusquedaAtributos.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TablaAtributos)).BeginInit();
            this.Panel_AtributosCRUD.SuspendLayout();
            this.Panel_AtributosBotones.SuspendLayout();
            this.Panel_Derecho.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1579, 68);
            this.Panel_Superior.TabIndex = 6;
            // 
            // Lbl_Formulario
            // 
            this.Lbl_Formulario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Formulario.AutoSize = true;
            this.Lbl_Formulario.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.Lbl_Formulario.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Formulario.Location = new System.Drawing.Point(11, 16);
            this.Lbl_Formulario.Name = "Lbl_Formulario";
            this.Lbl_Formulario.Size = new System.Drawing.Size(488, 32);
            this.Lbl_Formulario.TabIndex = 1;
            this.Lbl_Formulario.Text = "HISTORICO CARTAS DE RESONSABILIDAD";
            // 
            // Panel_Busqueda
            // 
            this.Panel_Busqueda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Busqueda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Busqueda.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_Busqueda.Controls.Add(this.Btn_Search);
            this.Panel_Busqueda.Controls.Add(this.Btn_CleanSearch);
            this.Panel_Busqueda.Controls.Add(this.Filtro1);
            this.Panel_Busqueda.Controls.Add(this.FiltroEstado);
            this.Panel_Busqueda.Location = new System.Drawing.Point(16, 7);
            this.Panel_Busqueda.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_Busqueda.Name = "Panel_Busqueda";
            this.Panel_Busqueda.Size = new System.Drawing.Size(1526, 110);
            this.Panel_Busqueda.TabIndex = 2;
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(21, 18);
            this.Txt_ValorBuscado.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(1278, 32);
            this.Txt_ValorBuscado.TabIndex = 0;
            this.Txt_ValorBuscado.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado_KeyDown);
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = ((System.Drawing.Image)(resources.GetObject("Btn_Search.Image")));
            this.Btn_Search.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Search.Location = new System.Drawing.Point(1315, 11);
            this.Btn_Search.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(135, 38);
            this.Btn_Search.TabIndex = 1;
            this.Btn_Search.Text = "BUSCAR";
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Btn_CleanSearch
            // 
            this.Btn_CleanSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_CleanSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_CleanSearch.Image = ((System.Drawing.Image)(resources.GetObject("Btn_CleanSearch.Image")));
            this.Btn_CleanSearch.Location = new System.Drawing.Point(1458, 11);
            this.Btn_CleanSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_CleanSearch.Name = "Btn_CleanSearch";
            this.Btn_CleanSearch.Size = new System.Drawing.Size(40, 38);
            this.Btn_CleanSearch.TabIndex = 2;
            this.Btn_CleanSearch.UseVisualStyleBackColor = true;
            this.Btn_CleanSearch.Click += new System.EventHandler(this.Btn_CleanSearch_Click);
            // 
            // Filtro1
            // 
            this.Filtro1.FormattingEnabled = true;
            this.Filtro1.Location = new System.Drawing.Point(21, 70);
            this.Filtro1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Filtro1.Name = "Filtro1";
            this.Filtro1.Size = new System.Drawing.Size(291, 33);
            this.Filtro1.TabIndex = 3;
            // 
            // FiltroEstado
            // 
            this.FiltroEstado.FormattingEnabled = true;
            this.FiltroEstado.Location = new System.Drawing.Point(336, 70);
            this.FiltroEstado.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FiltroEstado.Name = "FiltroEstado";
            this.FiltroEstado.Size = new System.Drawing.Size(291, 33);
            this.FiltroEstado.TabIndex = 4;
            // 
            // PanelToolStrip
            // 
            this.PanelToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelToolStrip.Controls.Add(this.Lbl_Paginas);
            this.PanelToolStrip.Location = new System.Drawing.Point(16, 118);
            this.PanelToolStrip.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PanelToolStrip.Name = "PanelToolStrip";
            this.PanelToolStrip.Size = new System.Drawing.Size(1526, 39);
            this.PanelToolStrip.TabIndex = 1;
            // 
            // Lbl_Paginas
            // 
            this.Lbl_Paginas.AutoSize = true;
            this.Lbl_Paginas.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Paginas.Location = new System.Drawing.Point(16, 10);
            this.Lbl_Paginas.Name = "Lbl_Paginas";
            this.Lbl_Paginas.Size = new System.Drawing.Size(321, 25);
            this.Lbl_Paginas.TabIndex = 0;
            this.Lbl_Paginas.Text = "MOSTRANDO 1-100 DE 0 ACTIVOS";
            // 
            // Panel_BusquedaAtributos
            // 
            this.Panel_BusquedaAtributos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_BusquedaAtributos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_BusquedaAtributos.Controls.Add(this.Txt_BuscarAtributo);
            this.Panel_BusquedaAtributos.Controls.Add(this.FiltroAtributoTipo);
            this.Panel_BusquedaAtributos.Controls.Add(this.Btn_SearchAtributo);
            this.Panel_BusquedaAtributos.Controls.Add(this.Btn_CleanSearchAtributo);
            this.Panel_BusquedaAtributos.Location = new System.Drawing.Point(16, 447);
            this.Panel_BusquedaAtributos.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_BusquedaAtributos.Name = "Panel_BusquedaAtributos";
            this.Panel_BusquedaAtributos.Size = new System.Drawing.Size(1526, 90);
            this.Panel_BusquedaAtributos.TabIndex = 5;
            // 
            // Txt_BuscarAtributo
            // 
            this.Txt_BuscarAtributo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_BuscarAtributo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_BuscarAtributo.Location = new System.Drawing.Point(4, 9);
            this.Txt_BuscarAtributo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_BuscarAtributo.Name = "Txt_BuscarAtributo";
            this.Txt_BuscarAtributo.Size = new System.Drawing.Size(1239, 32);
            this.Txt_BuscarAtributo.TabIndex = 0;
            this.Txt_BuscarAtributo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_BuscarAtributo_KeyDown);
            // 
            // FiltroAtributoTipo
            // 
            this.FiltroAtributoTipo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.FiltroAtributoTipo.FormattingEnabled = true;
            this.FiltroAtributoTipo.Location = new System.Drawing.Point(4, 47);
            this.FiltroAtributoTipo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FiltroAtributoTipo.Name = "FiltroAtributoTipo";
            this.FiltroAtributoTipo.Size = new System.Drawing.Size(220, 33);
            this.FiltroAtributoTipo.TabIndex = 1;
            // 
            // Btn_SearchAtributo
            // 
            this.Btn_SearchAtributo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_SearchAtributo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SearchAtributo.Image = ((System.Drawing.Image)(resources.GetObject("Btn_SearchAtributo.Image")));
            this.Btn_SearchAtributo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_SearchAtributo.Location = new System.Drawing.Point(1250, 9);
            this.Btn_SearchAtributo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_SearchAtributo.Name = "Btn_SearchAtributo";
            this.Btn_SearchAtributo.Size = new System.Drawing.Size(135, 36);
            this.Btn_SearchAtributo.TabIndex = 2;
            this.Btn_SearchAtributo.Text = "BUSCAR";
            this.Btn_SearchAtributo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SearchAtributo.UseVisualStyleBackColor = true;
            this.Btn_SearchAtributo.Click += new System.EventHandler(this.Btn_SearchAtributo_Click);
            // 
            // Btn_CleanSearchAtributo
            // 
            this.Btn_CleanSearchAtributo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_CleanSearchAtributo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_CleanSearchAtributo.Image = ((System.Drawing.Image)(resources.GetObject("Btn_CleanSearchAtributo.Image")));
            this.Btn_CleanSearchAtributo.Location = new System.Drawing.Point(1393, 9);
            this.Btn_CleanSearchAtributo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_CleanSearchAtributo.Name = "Btn_CleanSearchAtributo";
            this.Btn_CleanSearchAtributo.Size = new System.Drawing.Size(40, 36);
            this.Btn_CleanSearchAtributo.TabIndex = 3;
            this.Btn_CleanSearchAtributo.UseVisualStyleBackColor = true;
            this.Btn_CleanSearchAtributo.Click += new System.EventHandler(this.Btn_CleanSearchAtributo_Click);
            // 
            // Lbl_AtributosHeader
            // 
            this.Lbl_AtributosHeader.BackColor = System.Drawing.Color.White;
            this.Lbl_AtributosHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Lbl_AtributosHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.Lbl_AtributosHeader.Location = new System.Drawing.Point(16, 418);
            this.Lbl_AtributosHeader.Name = "Lbl_AtributosHeader";
            this.Lbl_AtributosHeader.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Lbl_AtributosHeader.Size = new System.Drawing.Size(966, 28);
            this.Lbl_AtributosHeader.TabIndex = 3;
            this.Lbl_AtributosHeader.Text = "HISTORICO - SELECCIONE UN ACTIVO FIJO";
            this.Lbl_AtributosHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(16, 164);
            this.PanelTabla.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(1526, 250);
            this.PanelTabla.TabIndex = 0;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Tabla.Name = "Tabla";
            this.Tabla.RowHeadersWidth = 51;
            this.Tabla.RowTemplate.Height = 24;
            this.Tabla.Size = new System.Drawing.Size(1526, 250);
            this.Tabla.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.TablaAtributos);
            this.panel1.Location = new System.Drawing.Point(16, 542);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1526, 288);
            this.panel1.TabIndex = 6;
            // 
            // TablaAtributos
            // 
            this.TablaAtributos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TablaAtributos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TablaAtributos.Location = new System.Drawing.Point(0, 0);
            this.TablaAtributos.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TablaAtributos.Name = "TablaAtributos";
            this.TablaAtributos.RowHeadersWidth = 51;
            this.TablaAtributos.RowTemplate.Height = 24;
            this.TablaAtributos.Size = new System.Drawing.Size(1526, 288);
            this.TablaAtributos.TabIndex = 0;
            // 
            // Panel_AtributosCRUD
            // 
            this.Panel_AtributosCRUD.BackColor = System.Drawing.Color.White;
            this.Panel_AtributosCRUD.Controls.Add(this.Txt_AttributeValue);
            this.Panel_AtributosCRUD.Controls.Add(this.Txt_AttributeType);
            this.Panel_AtributosCRUD.Controls.Add(this.Lbl_SubtituloAtributos);
            this.Panel_AtributosCRUD.Controls.Add(this.Lbl_AttributeKey);
            this.Panel_AtributosCRUD.Controls.Add(this.Txt_AttributeKey);
            this.Panel_AtributosCRUD.Controls.Add(this.Lbl_AttributeLabel);
            this.Panel_AtributosCRUD.Controls.Add(this.Txt_AttributeLabel);
            this.Panel_AtributosCRUD.Controls.Add(this.Lbl_DataType);
            this.Panel_AtributosCRUD.Controls.Add(this.Lbl_IsRequired);
            this.Panel_AtributosCRUD.Controls.Add(this.Panel_AtributosBotones);
            this.Panel_AtributosCRUD.Location = new System.Drawing.Point(16, 833);
            this.Panel_AtributosCRUD.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_AtributosCRUD.Name = "Panel_AtributosCRUD";
            this.Panel_AtributosCRUD.Size = new System.Drawing.Size(964, 156);
            this.Panel_AtributosCRUD.TabIndex = 7;
            // 
            // Txt_AttributeValue
            // 
            this.Txt_AttributeValue.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_AttributeValue.Location = new System.Drawing.Point(677, 60);
            this.Txt_AttributeValue.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_AttributeValue.Name = "Txt_AttributeValue";
            this.Txt_AttributeValue.Size = new System.Drawing.Size(244, 32);
            this.Txt_AttributeValue.TabIndex = 13;
            // 
            // Txt_AttributeType
            // 
            this.Txt_AttributeType.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_AttributeType.Location = new System.Drawing.Point(428, 60);
            this.Txt_AttributeType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_AttributeType.Name = "Txt_AttributeType";
            this.Txt_AttributeType.ReadOnly = true;
            this.Txt_AttributeType.Size = new System.Drawing.Size(244, 32);
            this.Txt_AttributeType.TabIndex = 12;
            // 
            // Lbl_SubtituloAtributos
            // 
            this.Lbl_SubtituloAtributos.AutoSize = true;
            this.Lbl_SubtituloAtributos.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_SubtituloAtributos.Location = new System.Drawing.Point(8, 6);
            this.Lbl_SubtituloAtributos.Name = "Lbl_SubtituloAtributos";
            this.Lbl_SubtituloAtributos.Size = new System.Drawing.Size(380, 25);
            this.Lbl_SubtituloAtributos.TabIndex = 0;
            this.Lbl_SubtituloAtributos.Text = "EDITAR CARACTERISTICA SELECCIONADA";
            // 
            // Lbl_AttributeKey
            // 
            this.Lbl_AttributeKey.AutoSize = true;
            this.Lbl_AttributeKey.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_AttributeKey.Location = new System.Drawing.Point(8, 36);
            this.Lbl_AttributeKey.Name = "Lbl_AttributeKey";
            this.Lbl_AttributeKey.Size = new System.Drawing.Size(83, 25);
            this.Lbl_AttributeKey.TabIndex = 1;
            this.Lbl_AttributeKey.Text = "CLAVE *";
            // 
            // Txt_AttributeKey
            // 
            this.Txt_AttributeKey.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_AttributeKey.Location = new System.Drawing.Point(8, 60);
            this.Txt_AttributeKey.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_AttributeKey.Name = "Txt_AttributeKey";
            this.Txt_AttributeKey.ReadOnly = true;
            this.Txt_AttributeKey.Size = new System.Drawing.Size(167, 32);
            this.Txt_AttributeKey.TabIndex = 2;
            // 
            // Lbl_AttributeLabel
            // 
            this.Lbl_AttributeLabel.AutoSize = true;
            this.Lbl_AttributeLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_AttributeLabel.Location = new System.Drawing.Point(179, 36);
            this.Lbl_AttributeLabel.Name = "Lbl_AttributeLabel";
            this.Lbl_AttributeLabel.Size = new System.Drawing.Size(114, 25);
            this.Lbl_AttributeLabel.TabIndex = 3;
            this.Lbl_AttributeLabel.Text = "ETIQUETA *";
            // 
            // Txt_AttributeLabel
            // 
            this.Txt_AttributeLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_AttributeLabel.Location = new System.Drawing.Point(179, 60);
            this.Txt_AttributeLabel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_AttributeLabel.Name = "Txt_AttributeLabel";
            this.Txt_AttributeLabel.ReadOnly = true;
            this.Txt_AttributeLabel.Size = new System.Drawing.Size(244, 32);
            this.Txt_AttributeLabel.TabIndex = 4;
            // 
            // Lbl_DataType
            // 
            this.Lbl_DataType.AutoSize = true;
            this.Lbl_DataType.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_DataType.Location = new System.Drawing.Point(428, 30);
            this.Lbl_DataType.Name = "Lbl_DataType";
            this.Lbl_DataType.Size = new System.Drawing.Size(69, 25);
            this.Lbl_DataType.TabIndex = 5;
            this.Lbl_DataType.Text = "TIPO *";
            // 
            // Lbl_IsRequired
            // 
            this.Lbl_IsRequired.AutoSize = true;
            this.Lbl_IsRequired.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_IsRequired.Location = new System.Drawing.Point(677, 36);
            this.Lbl_IsRequired.Name = "Lbl_IsRequired";
            this.Lbl_IsRequired.Size = new System.Drawing.Size(192, 25);
            this.Lbl_IsRequired.TabIndex = 7;
            this.Lbl_IsRequired.Text = "VALOR ASIGNADO *";
            // 
            // Panel_AtributosBotones
            // 
            this.Panel_AtributosBotones.BackColor = System.Drawing.Color.White;
            this.Panel_AtributosBotones.Controls.Add(this.Btn_UpdateAtributo);
            this.Panel_AtributosBotones.Controls.Add(this.Btn_ClearAtributo);
            this.Panel_AtributosBotones.Location = new System.Drawing.Point(8, 94);
            this.Panel_AtributosBotones.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_AtributosBotones.Name = "Panel_AtributosBotones";
            this.Panel_AtributosBotones.Size = new System.Drawing.Size(953, 57);
            this.Panel_AtributosBotones.TabIndex = 11;
            // 
            // Btn_UpdateAtributo
            // 
            this.Btn_UpdateAtributo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_UpdateAtributo.Image = ((System.Drawing.Image)(resources.GetObject("Btn_UpdateAtributo.Image")));
            this.Btn_UpdateAtributo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_UpdateAtributo.Location = new System.Drawing.Point(4, 10);
            this.Btn_UpdateAtributo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_UpdateAtributo.Name = "Btn_UpdateAtributo";
            this.Btn_UpdateAtributo.Size = new System.Drawing.Size(131, 42);
            this.Btn_UpdateAtributo.TabIndex = 1;
            this.Btn_UpdateAtributo.Text = "EDITAR";
            this.Btn_UpdateAtributo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_UpdateAtributo.UseVisualStyleBackColor = true;
            this.Btn_UpdateAtributo.Click += new System.EventHandler(this.Btn_UpdateAtributo_Click);
            // 
            // Btn_ClearAtributo
            // 
            this.Btn_ClearAtributo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_ClearAtributo.Image = ((System.Drawing.Image)(resources.GetObject("Btn_ClearAtributo.Image")));
            this.Btn_ClearAtributo.Location = new System.Drawing.Point(140, 10);
            this.Btn_ClearAtributo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_ClearAtributo.Name = "Btn_ClearAtributo";
            this.Btn_ClearAtributo.Size = new System.Drawing.Size(44, 42);
            this.Btn_ClearAtributo.TabIndex = 3;
            this.Btn_ClearAtributo.UseVisualStyleBackColor = true;
            this.Btn_ClearAtributo.Click += new System.EventHandler(this.Btn_ClearAtributo_Click);
            // 
            // Panel_Derecho
            // 
            this.Panel_Derecho.AutoScroll = true;
            this.Panel_Derecho.Controls.Add(this.Panel_AtributosCRUD);
            this.Panel_Derecho.Controls.Add(this.panel1);
            this.Panel_Derecho.Controls.Add(this.PanelTabla);
            this.Panel_Derecho.Controls.Add(this.Lbl_AtributosHeader);
            this.Panel_Derecho.Controls.Add(this.Panel_BusquedaAtributos);
            this.Panel_Derecho.Controls.Add(this.PanelToolStrip);
            this.Panel_Derecho.Controls.Add(this.Panel_Busqueda);
            this.Panel_Derecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Derecho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Derecho.Location = new System.Drawing.Point(0, 68);
            this.Panel_Derecho.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_Derecho.Name = "Panel_Derecho";
            this.Panel_Derecho.Size = new System.Drawing.Size(1579, 987);
            this.Panel_Derecho.TabIndex = 0;
            // 
            // Frm_FixedAsset_ResponsabilityLetterHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1579, 1055);
            this.Controls.Add(this.Panel_Derecho);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Frm_FixedAsset_ResponsabilityLetterHistory";
            this.Text = "SECRON - GESTIÓN DE ACTIVOS FIJOS";
            this.Load += new System.EventHandler(this.Frm_FixedAsset_ResponsabilityLetterHistory_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Busqueda.ResumeLayout(false);
            this.Panel_Busqueda.PerformLayout();
            this.PanelToolStrip.ResumeLayout(false);
            this.PanelToolStrip.PerformLayout();
            this.Panel_BusquedaAtributos.ResumeLayout(false);
            this.Panel_BusquedaAtributos.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TablaAtributos)).EndInit();
            this.Panel_AtributosCRUD.ResumeLayout(false);
            this.Panel_AtributosCRUD.PerformLayout();
            this.Panel_AtributosBotones.ResumeLayout(false);
            this.Panel_Derecho.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_Busqueda;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Button Btn_CleanSearch;
        private System.Windows.Forms.ComboBox Filtro1;
        private System.Windows.Forms.ComboBox FiltroEstado;
        private System.Windows.Forms.Panel PanelToolStrip;
        private System.Windows.Forms.Label Lbl_Paginas;
        private System.Windows.Forms.Panel Panel_BusquedaAtributos;
        private System.Windows.Forms.TextBox Txt_BuscarAtributo;
        private System.Windows.Forms.ComboBox FiltroAtributoTipo;
        private System.Windows.Forms.Button Btn_SearchAtributo;
        private System.Windows.Forms.Button Btn_CleanSearchAtributo;
        private System.Windows.Forms.Label Lbl_AtributosHeader;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView TablaAtributos;
        private System.Windows.Forms.Panel Panel_AtributosCRUD;
        private System.Windows.Forms.TextBox Txt_AttributeValue;
        private System.Windows.Forms.TextBox Txt_AttributeType;
        private System.Windows.Forms.Label Lbl_SubtituloAtributos;
        private System.Windows.Forms.Label Lbl_AttributeKey;
        private System.Windows.Forms.TextBox Txt_AttributeKey;
        private System.Windows.Forms.Label Lbl_AttributeLabel;
        private System.Windows.Forms.TextBox Txt_AttributeLabel;
        private System.Windows.Forms.Label Lbl_DataType;
        private System.Windows.Forms.Label Lbl_IsRequired;
        private System.Windows.Forms.Panel Panel_AtributosBotones;
        private System.Windows.Forms.Button Btn_UpdateAtributo;
        private System.Windows.Forms.Button Btn_ClearAtributo;
        private System.Windows.Forms.Panel Panel_Derecho;
    }
}