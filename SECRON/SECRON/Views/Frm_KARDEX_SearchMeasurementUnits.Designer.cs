namespace SECRON.Views
{
    partial class Frm_KARDEX_SearchMeasurementUnits
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_KARDEX_SearchMeasurementUnits));
            this.panel1 = new System.Windows.Forms.Panel();
            this.Btn_No = new System.Windows.Forms.Button();
            this.Btn_Yes = new System.Windows.Forms.Button();
            this.Lbl_Beneficiario = new System.Windows.Forms.Label();
            this.Txt_Selected = new System.Windows.Forms.TextBox();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Panel_DetalleTabla = new System.Windows.Forms.Panel();
            this.ComboBox_BuscarPor = new System.Windows.Forms.ComboBox();
            this.Lbl_BuscarPor = new System.Windows.Forms.Label();
            this.Btn_ClearSearch = new System.Windows.Forms.Button();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Lbl_ValorBuscado = new System.Windows.Forms.Label();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_1 = new System.Windows.Forms.Panel();
            this.Txt_Abbreviation = new System.Windows.Forms.TextBox();
            this.Lbl_Abbreviation = new System.Windows.Forms.Label();
            this.Lbl_Subtitulo1 = new System.Windows.Forms.Label();
            this.Txt_UnitName = new System.Windows.Forms.TextBox();
            this.Txt_Codigo = new System.Windows.Forms.TextBox();
            this.Lbl_UnitName = new System.Windows.Forms.Label();
            this.Lbl_Codigo = new System.Windows.Forms.Label();
            this.Panel_CRUD = new System.Windows.Forms.Panel();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Update = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Btn_Inactive = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.Panel_DetalleTabla.SuspendLayout();
            this.Panel_Superior.SuspendLayout();
            this.Panel_1.SuspendLayout();
            this.Panel_CRUD.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.Btn_No);
            this.panel1.Controls.Add(this.Btn_Yes);
            this.panel1.Controls.Add(this.Lbl_Beneficiario);
            this.panel1.Controls.Add(this.Txt_Selected);
            this.panel1.Location = new System.Drawing.Point(307, 488);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(665, 114);
            this.panel1.TabIndex = 82;
            // 
            // Btn_No
            // 
            this.Btn_No.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_No.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_No.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_No.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_No.Location = new System.Drawing.Point(406, 65);
            this.Btn_No.Name = "Btn_No";
            this.Btn_No.Size = new System.Drawing.Size(124, 37);
            this.Btn_No.TabIndex = 66;
            this.Btn_No.Text = "CANCELAR";
            this.Btn_No.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_No.UseVisualStyleBackColor = true;
            this.Btn_No.Click += new System.EventHandler(this.Btn_No_Click);
            // 
            // Btn_Yes
            // 
            this.Btn_Yes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Yes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Yes.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Yes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Yes.Location = new System.Drawing.Point(536, 65);
            this.Btn_Yes.Name = "Btn_Yes";
            this.Btn_Yes.Size = new System.Drawing.Size(117, 37);
            this.Btn_Yes.TabIndex = 65;
            this.Btn_Yes.Text = "ACEPTAR";
            this.Btn_Yes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Yes.UseVisualStyleBackColor = true;
            this.Btn_Yes.Click += new System.EventHandler(this.Btn_Yes_Click);
            // 
            // Lbl_Beneficiario
            // 
            this.Lbl_Beneficiario.AutoSize = true;
            this.Lbl_Beneficiario.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Beneficiario.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Beneficiario.Location = new System.Drawing.Point(10, 12);
            this.Lbl_Beneficiario.Name = "Lbl_Beneficiario";
            this.Lbl_Beneficiario.Size = new System.Drawing.Size(276, 20);
            this.Lbl_Beneficiario.TabIndex = 61;
            this.Lbl_Beneficiario.Text = "UNIDAD DE MEDIDA SELECCIONADA:";
            // 
            // Txt_Selected
            // 
            this.Txt_Selected.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Selected.Location = new System.Drawing.Point(14, 35);
            this.Txt_Selected.MaxLength = 15;
            this.Txt_Selected.Name = "Txt_Selected";
            this.Txt_Selected.Size = new System.Drawing.Size(639, 27);
            this.Txt_Selected.TabIndex = 60;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(307, 199);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(665, 279);
            this.PanelTabla.TabIndex = 81;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(665, 279);
            this.Tabla.TabIndex = 1;
            // 
            // Panel_DetalleTabla
            // 
            this.Panel_DetalleTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_DetalleTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_DetalleTabla.Controls.Add(this.ComboBox_BuscarPor);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_BuscarPor);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_ClearSearch);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Search);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_ValorBuscado);
            this.Panel_DetalleTabla.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_DetalleTabla.Location = new System.Drawing.Point(307, 61);
            this.Panel_DetalleTabla.Name = "Panel_DetalleTabla";
            this.Panel_DetalleTabla.Size = new System.Drawing.Size(665, 125);
            this.Panel_DetalleTabla.TabIndex = 80;
            // 
            // ComboBox_BuscarPor
            // 
            this.ComboBox_BuscarPor.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ComboBox_BuscarPor.FormattingEnabled = true;
            this.ComboBox_BuscarPor.Location = new System.Drawing.Point(14, 35);
            this.ComboBox_BuscarPor.Name = "ComboBox_BuscarPor";
            this.ComboBox_BuscarPor.Size = new System.Drawing.Size(557, 26);
            this.ComboBox_BuscarPor.TabIndex = 71;
            // 
            // Lbl_BuscarPor
            // 
            this.Lbl_BuscarPor.AutoSize = true;
            this.Lbl_BuscarPor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_BuscarPor.ForeColor = System.Drawing.Color.Black;
            this.Lbl_BuscarPor.Location = new System.Drawing.Point(10, 12);
            this.Lbl_BuscarPor.Name = "Lbl_BuscarPor";
            this.Lbl_BuscarPor.Size = new System.Drawing.Size(106, 20);
            this.Lbl_BuscarPor.TabIndex = 64;
            this.Lbl_BuscarPor.Text = "BUSCAR POR:";
            // 
            // Btn_ClearSearch
            // 
            this.Btn_ClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_ClearSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ClearSearch.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_ClearSearch.Location = new System.Drawing.Point(620, 16);
            this.Btn_ClearSearch.Name = "Btn_ClearSearch";
            this.Btn_ClearSearch.Size = new System.Drawing.Size(35, 45);
            this.Btn_ClearSearch.TabIndex = 63;
            this.Btn_ClearSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_ClearSearch.UseVisualStyleBackColor = true;
            this.Btn_ClearSearch.Click += new System.EventHandler(this.Btn_ClearSearch_Click);
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.Location = new System.Drawing.Point(579, 16);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(35, 45);
            this.Btn_Search.TabIndex = 62;
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Lbl_ValorBuscado
            // 
            this.Lbl_ValorBuscado.AutoSize = true;
            this.Lbl_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ValorBuscado.ForeColor = System.Drawing.Color.Black;
            this.Lbl_ValorBuscado.Location = new System.Drawing.Point(10, 68);
            this.Lbl_ValorBuscado.Name = "Lbl_ValorBuscado";
            this.Lbl_ValorBuscado.Size = new System.Drawing.Size(224, 20);
            this.Lbl_ValorBuscado.TabIndex = 61;
            this.Lbl_ValorBuscado.Text = "BUSCAR UNIDAD DE MEDIDA:";
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(14, 90);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(639, 27);
            this.Txt_ValorBuscado.TabIndex = 60;
            this.Txt_ValorBuscado.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado_KeyDown);
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(984, 55);
            this.Panel_Superior.TabIndex = 79;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(351, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "CATÁLOGO DE UNIDADES DE MEDIDA";
            // 
            // Panel_1
            // 
            this.Panel_1.BackColor = System.Drawing.Color.White;
            this.Panel_1.Controls.Add(this.Txt_Abbreviation);
            this.Panel_1.Controls.Add(this.Lbl_Abbreviation);
            this.Panel_1.Controls.Add(this.Lbl_Subtitulo1);
            this.Panel_1.Controls.Add(this.Txt_UnitName);
            this.Panel_1.Controls.Add(this.Txt_Codigo);
            this.Panel_1.Controls.Add(this.Lbl_UnitName);
            this.Panel_1.Controls.Add(this.Lbl_Codigo);
            this.Panel_1.Location = new System.Drawing.Point(7, 61);
            this.Panel_1.Name = "Panel_1";
            this.Panel_1.Size = new System.Drawing.Size(294, 220);
            this.Panel_1.TabIndex = 83;
            // 
            // Txt_Abbreviation
            // 
            this.Txt_Abbreviation.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Abbreviation.Location = new System.Drawing.Point(14, 175);
            this.Txt_Abbreviation.MaxLength = 15;
            this.Txt_Abbreviation.Name = "Txt_Abbreviation";
            this.Txt_Abbreviation.Size = new System.Drawing.Size(267, 27);
            this.Txt_Abbreviation.TabIndex = 62;
            // 
            // Lbl_Abbreviation
            // 
            this.Lbl_Abbreviation.AutoSize = true;
            this.Lbl_Abbreviation.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Abbreviation.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Abbreviation.Location = new System.Drawing.Point(10, 152);
            this.Lbl_Abbreviation.Name = "Lbl_Abbreviation";
            this.Lbl_Abbreviation.Size = new System.Drawing.Size(125, 20);
            this.Lbl_Abbreviation.TabIndex = 63;
            this.Lbl_Abbreviation.Text = "ABREVIATURA *";
            // 
            // Lbl_Subtitulo1
            // 
            this.Lbl_Subtitulo1.AutoSize = true;
            this.Lbl_Subtitulo1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo1.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo1.Image = global::SECRON.Properties.Resources.DescripcionItemBlanco25x25;
            this.Lbl_Subtitulo1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo1.Location = new System.Drawing.Point(10, 7);
            this.Lbl_Subtitulo1.Name = "Lbl_Subtitulo1";
            this.Lbl_Subtitulo1.Size = new System.Drawing.Size(103, 20);
            this.Lbl_Subtitulo1.TabIndex = 61;
            this.Lbl_Subtitulo1.Text = "      DETALLES";
            this.Lbl_Subtitulo1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Txt_UnitName
            // 
            this.Txt_UnitName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_UnitName.Location = new System.Drawing.Point(14, 113);
            this.Txt_UnitName.MaxLength = 15;
            this.Txt_UnitName.Name = "Txt_UnitName";
            this.Txt_UnitName.Size = new System.Drawing.Size(267, 27);
            this.Txt_UnitName.TabIndex = 2;
            // 
            // Txt_Codigo
            // 
            this.Txt_Codigo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Codigo.Location = new System.Drawing.Point(14, 55);
            this.Txt_Codigo.MaxLength = 15;
            this.Txt_Codigo.Name = "Txt_Codigo";
            this.Txt_Codigo.Size = new System.Drawing.Size(267, 27);
            this.Txt_Codigo.TabIndex = 1;
            // 
            // Lbl_UnitName
            // 
            this.Lbl_UnitName.AutoSize = true;
            this.Lbl_UnitName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_UnitName.ForeColor = System.Drawing.Color.Black;
            this.Lbl_UnitName.Location = new System.Drawing.Point(10, 90);
            this.Lbl_UnitName.Name = "Lbl_UnitName";
            this.Lbl_UnitName.Size = new System.Drawing.Size(194, 20);
            this.Lbl_UnitName.TabIndex = 2;
            this.Lbl_UnitName.Text = "NOMBRE DEL ARTÍCULO *";
            // 
            // Lbl_Codigo
            // 
            this.Lbl_Codigo.AutoSize = true;
            this.Lbl_Codigo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Codigo.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Codigo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Codigo.Location = new System.Drawing.Point(10, 32);
            this.Lbl_Codigo.Name = "Lbl_Codigo";
            this.Lbl_Codigo.Size = new System.Drawing.Size(230, 20);
            this.Lbl_Codigo.TabIndex = 1;
            this.Lbl_Codigo.Text = "CÓDIGO UNIDAD DE MEDIDA *";
            // 
            // Panel_CRUD
            // 
            this.Panel_CRUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_CRUD.Controls.Add(this.Btn_Clear);
            this.Panel_CRUD.Controls.Add(this.Btn_Update);
            this.Panel_CRUD.Controls.Add(this.Btn_Save);
            this.Panel_CRUD.Location = new System.Drawing.Point(7, 298);
            this.Panel_CRUD.Name = "Panel_CRUD";
            this.Panel_CRUD.Size = new System.Drawing.Size(294, 47);
            this.Panel_CRUD.TabIndex = 84;
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Clear.Location = new System.Drawing.Point(248, 5);
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
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Btn_Update.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Update.Image = global::SECRON.Properties.Resources.UpdateAzul25x25;
            this.Btn_Update.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Update.Location = new System.Drawing.Point(129, 5);
            this.Btn_Update.Name = "Btn_Update";
            this.Btn_Update.Size = new System.Drawing.Size(113, 37);
            this.Btn_Update.TabIndex = 55;
            this.Btn_Update.Text = "EDITAR";
            this.Btn_Update.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Update.UseVisualStyleBackColor = true;
            this.Btn_Update.Click += new System.EventHandler(this.Btn_Update_Click);
            // 
            // Btn_Save
            // 
            this.Btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Btn_Save.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Save.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Save.Location = new System.Drawing.Point(6, 5);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(117, 37);
            this.Btn_Save.TabIndex = 54;
            this.Btn_Save.Text = "GUARDAR";
            this.Btn_Save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel2.Controls.Add(this.Btn_Inactive);
            this.panel2.Location = new System.Drawing.Point(7, 361);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(294, 47);
            this.panel2.TabIndex = 85;
            // 
            // Btn_Inactive
            // 
            this.Btn_Inactive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Btn_Inactive.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Inactive.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Inactive.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Inactive.Location = new System.Drawing.Point(6, 3);
            this.Btn_Inactive.Name = "Btn_Inactive";
            this.Btn_Inactive.Size = new System.Drawing.Size(124, 37);
            this.Btn_Inactive.TabIndex = 56;
            this.Btn_Inactive.Text = "INACTIVAR";
            this.Btn_Inactive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Inactive.UseVisualStyleBackColor = true;
            this.Btn_Inactive.Click += new System.EventHandler(this.Btn_Inactive_Click);
            // 
            // Frm_KARDEX_SearchMeasurementUnits
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.Panel_CRUD);
            this.Controls.Add(this.Panel_1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_DetalleTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_KARDEX_SearchMeasurementUnits";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - CATÁLOGO DE UNIDADES DE MEDIDA";
            this.Load += new System.EventHandler(this.Frm_KARDEX_SearchMeasurementUnits_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.Panel_DetalleTabla.ResumeLayout(false);
            this.Panel_DetalleTabla.PerformLayout();
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_1.ResumeLayout(false);
            this.Panel_1.PerformLayout();
            this.Panel_CRUD.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Btn_No;
        private System.Windows.Forms.Button Btn_Yes;
        private System.Windows.Forms.Label Lbl_Beneficiario;
        private System.Windows.Forms.TextBox Txt_Selected;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel Panel_DetalleTabla;
        private System.Windows.Forms.ComboBox ComboBox_BuscarPor;
        private System.Windows.Forms.Label Lbl_BuscarPor;
        private System.Windows.Forms.Button Btn_ClearSearch;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Label Lbl_ValorBuscado;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_1;
        private System.Windows.Forms.Label Lbl_Subtitulo1;
        private System.Windows.Forms.TextBox Txt_UnitName;
        private System.Windows.Forms.TextBox Txt_Codigo;
        private System.Windows.Forms.Label Lbl_UnitName;
        private System.Windows.Forms.Label Lbl_Codigo;
        private System.Windows.Forms.TextBox Txt_Abbreviation;
        private System.Windows.Forms.Label Lbl_Abbreviation;
        private System.Windows.Forms.Panel Panel_CRUD;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Inactive;
        private System.Windows.Forms.Button Btn_Update;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Panel panel2;
    }
}