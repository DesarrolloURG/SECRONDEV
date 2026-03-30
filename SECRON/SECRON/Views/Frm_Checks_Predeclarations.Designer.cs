namespace SECRON.Views
{
    partial class Frm_Checks_Predeclarations
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Checks_Predeclarations));
            this.panel1 = new System.Windows.Forms.Panel();
            this.Btn_CSV = new System.Windows.Forms.Button();
            this.Btn_No = new System.Windows.Forms.Button();
            this.Btn_Yes = new System.Windows.Forms.Button();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Panel_DetalleTabla = new System.Windows.Forms.Panel();
            this.CheckBox_FiltroFechas = new System.Windows.Forms.CheckBox();
            this.DTP_FechaFin = new System.Windows.Forms.DateTimePicker();
            this.Lbl_DTPFin = new System.Windows.Forms.Label();
            this.Lbl_DTPInicio = new System.Windows.Forms.Label();
            this.DTP_FechaInicio = new System.Windows.Forms.DateTimePicker();
            this.Filtro3 = new System.Windows.Forms.ComboBox();
            this.Filtro2 = new System.Windows.Forms.ComboBox();
            this.Filtro1 = new System.Windows.Forms.ComboBox();
            this.Btn_YesSelected = new System.Windows.Forms.Button();
            this.Lbl_BuscarPor = new System.Windows.Forms.Label();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Lbl_ValorBuscado = new System.Windows.Forms.Label();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.PanelToolStrip = new System.Windows.Forms.Panel();
            this.Btn_LimpiarSeleccion = new System.Windows.Forms.Button();
            this.Lbl_Total = new System.Windows.Forms.Label();
            this.Txt_Total = new System.Windows.Forms.TextBox();
            this.Lbl_Paginas = new System.Windows.Forms.Label();
            this.Btn_SelectAll = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.Panel_DetalleTabla.SuspendLayout();
            this.Panel_Superior.SuspendLayout();
            this.PanelToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.Btn_CSV);
            this.panel1.Controls.Add(this.Btn_No);
            this.panel1.Controls.Add(this.Btn_Yes);
            this.panel1.Location = new System.Drawing.Point(9, 797);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1165, 59);
            this.panel1.TabIndex = 86;
            // 
            // Btn_CSV
            // 
            this.Btn_CSV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_CSV.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_CSV.Image = global::SECRON.Properties.Resources.ExportarExcelNegro25x25;
            this.Btn_CSV.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_CSV.Location = new System.Drawing.Point(735, 10);
            this.Btn_CSV.Name = "Btn_CSV";
            this.Btn_CSV.Size = new System.Drawing.Size(74, 37);
            this.Btn_CSV.TabIndex = 68;
            this.Btn_CSV.Text = "CSV";
            this.Btn_CSV.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_CSV.UseVisualStyleBackColor = true;
            this.Btn_CSV.Click += new System.EventHandler(this.Btn_CSV_Click);
            // 
            // Btn_No
            // 
            this.Btn_No.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_No.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_No.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_No.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_No.Location = new System.Drawing.Point(815, 10);
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
            this.Btn_Yes.Location = new System.Drawing.Point(945, 10);
            this.Btn_Yes.Name = "Btn_Yes";
            this.Btn_Yes.Size = new System.Drawing.Size(208, 37);
            this.Btn_Yes.TabIndex = 65;
            this.Btn_Yes.Text = "PREDECLARAR TODOS";
            this.Btn_Yes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Yes.UseVisualStyleBackColor = true;
            this.Btn_Yes.Click += new System.EventHandler(this.Btn_Yes_Click);
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(9, 314);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(1165, 477);
            this.PanelTabla.TabIndex = 85;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(1165, 477);
            this.Tabla.TabIndex = 1;
            this.Tabla.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tabla_CellContentClick);
            // 
            // Panel_DetalleTabla
            // 
            this.Panel_DetalleTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_DetalleTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_DetalleTabla.Controls.Add(this.CheckBox_FiltroFechas);
            this.Panel_DetalleTabla.Controls.Add(this.DTP_FechaFin);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_DTPFin);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_DTPInicio);
            this.Panel_DetalleTabla.Controls.Add(this.DTP_FechaInicio);
            this.Panel_DetalleTabla.Controls.Add(this.Filtro3);
            this.Panel_DetalleTabla.Controls.Add(this.Filtro2);
            this.Panel_DetalleTabla.Controls.Add(this.Filtro1);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_YesSelected);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_BuscarPor);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Clear);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Search);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_ValorBuscado);
            this.Panel_DetalleTabla.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_DetalleTabla.Location = new System.Drawing.Point(9, 70);
            this.Panel_DetalleTabla.Name = "Panel_DetalleTabla";
            this.Panel_DetalleTabla.Size = new System.Drawing.Size(1163, 175);
            this.Panel_DetalleTabla.TabIndex = 84;
            // 
            // CheckBox_FiltroFechas
            // 
            this.CheckBox_FiltroFechas.AutoSize = true;
            this.CheckBox_FiltroFechas.Checked = true;
            this.CheckBox_FiltroFechas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_FiltroFechas.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.CheckBox_FiltroFechas.Location = new System.Drawing.Point(14, 65);
            this.CheckBox_FiltroFechas.Name = "CheckBox_FiltroFechas";
            this.CheckBox_FiltroFechas.Size = new System.Drawing.Size(162, 23);
            this.CheckBox_FiltroFechas.TabIndex = 83;
            this.CheckBox_FiltroFechas.Text = "FILTRO POR FECHAS";
            this.CheckBox_FiltroFechas.UseVisualStyleBackColor = true;
            // 
            // DTP_FechaFin
            // 
            this.DTP_FechaFin.Location = new System.Drawing.Point(336, 111);
            this.DTP_FechaFin.Name = "DTP_FechaFin";
            this.DTP_FechaFin.Size = new System.Drawing.Size(317, 20);
            this.DTP_FechaFin.TabIndex = 82;
            // 
            // Lbl_DTPFin
            // 
            this.Lbl_DTPFin.AutoSize = true;
            this.Lbl_DTPFin.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_DTPFin.ForeColor = System.Drawing.Color.Black;
            this.Lbl_DTPFin.Location = new System.Drawing.Point(332, 88);
            this.Lbl_DTPFin.Name = "Lbl_DTPFin";
            this.Lbl_DTPFin.Size = new System.Drawing.Size(157, 20);
            this.Lbl_DTPFin.TabIndex = 81;
            this.Lbl_DTPFin.Text = "FECHA LÍMITE FINAL";
            // 
            // Lbl_DTPInicio
            // 
            this.Lbl_DTPInicio.AutoSize = true;
            this.Lbl_DTPInicio.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_DTPInicio.ForeColor = System.Drawing.Color.Black;
            this.Lbl_DTPInicio.Location = new System.Drawing.Point(10, 88);
            this.Lbl_DTPInicio.Name = "Lbl_DTPInicio";
            this.Lbl_DTPInicio.Size = new System.Drawing.Size(168, 20);
            this.Lbl_DTPInicio.TabIndex = 79;
            this.Lbl_DTPInicio.Text = "FECHA LÍMITE INICIAL";
            // 
            // DTP_FechaInicio
            // 
            this.DTP_FechaInicio.Location = new System.Drawing.Point(14, 111);
            this.DTP_FechaInicio.Name = "DTP_FechaInicio";
            this.DTP_FechaInicio.Size = new System.Drawing.Size(298, 20);
            this.DTP_FechaInicio.TabIndex = 80;
            // 
            // Filtro3
            // 
            this.Filtro3.FormattingEnabled = true;
            this.Filtro3.Location = new System.Drawing.Point(446, 40);
            this.Filtro3.Name = "Filtro3";
            this.Filtro3.Size = new System.Drawing.Size(207, 21);
            this.Filtro3.TabIndex = 78;
            // 
            // Filtro2
            // 
            this.Filtro2.FormattingEnabled = true;
            this.Filtro2.Location = new System.Drawing.Point(227, 40);
            this.Filtro2.Name = "Filtro2";
            this.Filtro2.Size = new System.Drawing.Size(207, 21);
            this.Filtro2.TabIndex = 77;
            // 
            // Filtro1
            // 
            this.Filtro1.FormattingEnabled = true;
            this.Filtro1.Location = new System.Drawing.Point(14, 40);
            this.Filtro1.Name = "Filtro1";
            this.Filtro1.Size = new System.Drawing.Size(207, 21);
            this.Filtro1.TabIndex = 76;
            // 
            // Btn_YesSelected
            // 
            this.Btn_YesSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_YesSelected.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_YesSelected.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_YesSelected.Location = new System.Drawing.Point(1118, 135);
            this.Btn_YesSelected.Name = "Btn_YesSelected";
            this.Btn_YesSelected.Size = new System.Drawing.Size(35, 35);
            this.Btn_YesSelected.TabIndex = 75;
            this.Btn_YesSelected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_YesSelected.UseVisualStyleBackColor = true;
            this.Btn_YesSelected.Click += new System.EventHandler(this.Btn_YesSelected_Click);
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
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.Location = new System.Drawing.Point(1078, 135);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(35, 35);
            this.Btn_Clear.TabIndex = 63;
            this.Btn_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            this.Btn_Clear.Click += new System.EventHandler(this.Btn_Clear_Click);
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.Location = new System.Drawing.Point(1037, 135);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(35, 35);
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
            this.Lbl_ValorBuscado.Size = new System.Drawing.Size(13, 20);
            this.Lbl_ValorBuscado.TabIndex = 61;
            this.Lbl_ValorBuscado.Text = " ";
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(12, 137);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(519, 27);
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
            this.Panel_Superior.Size = new System.Drawing.Size(1184, 55);
            this.Panel_Superior.TabIndex = 83;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(297, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "PREDECLARACIÓN DE CHEQUES";
            // 
            // PanelToolStrip
            // 
            this.PanelToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelToolStrip.Controls.Add(this.Btn_SelectAll);
            this.PanelToolStrip.Controls.Add(this.Btn_LimpiarSeleccion);
            this.PanelToolStrip.Controls.Add(this.Lbl_Total);
            this.PanelToolStrip.Controls.Add(this.Txt_Total);
            this.PanelToolStrip.Controls.Add(this.Lbl_Paginas);
            this.PanelToolStrip.Location = new System.Drawing.Point(9, 251);
            this.PanelToolStrip.Name = "PanelToolStrip";
            this.PanelToolStrip.Size = new System.Drawing.Size(1163, 57);
            this.PanelToolStrip.TabIndex = 87;
            // 
            // Btn_LimpiarSeleccion
            // 
            this.Btn_LimpiarSeleccion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_LimpiarSeleccion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_LimpiarSeleccion.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_LimpiarSeleccion.Location = new System.Drawing.Point(12, 11);
            this.Btn_LimpiarSeleccion.Name = "Btn_LimpiarSeleccion";
            this.Btn_LimpiarSeleccion.Size = new System.Drawing.Size(35, 35);
            this.Btn_LimpiarSeleccion.TabIndex = 84;
            this.Btn_LimpiarSeleccion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_LimpiarSeleccion.UseVisualStyleBackColor = true;
            this.Btn_LimpiarSeleccion.Click += new System.EventHandler(this.Btn_LimpiarSeleccion_Click);
            // 
            // Lbl_Total
            // 
            this.Lbl_Total.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Lbl_Total.AutoSize = true;
            this.Lbl_Total.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Total.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Total.Location = new System.Drawing.Point(547, 19);
            this.Lbl_Total.Name = "Lbl_Total";
            this.Lbl_Total.Size = new System.Drawing.Size(239, 20);
            this.Lbl_Total.TabIndex = 69;
            this.Lbl_Total.Text = "SUMATORIA TOTAL DE CHEQUES";
            // 
            // Txt_Total
            // 
            this.Txt_Total.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_Total.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Total.Location = new System.Drawing.Point(815, 12);
            this.Txt_Total.MaxLength = 15;
            this.Txt_Total.Name = "Txt_Total";
            this.Txt_Total.Size = new System.Drawing.Size(319, 27);
            this.Txt_Total.TabIndex = 68;
            this.Txt_Total.Text = "0.00";
            // 
            // Lbl_Paginas
            // 
            this.Lbl_Paginas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Paginas.AutoSize = true;
            this.Lbl_Paginas.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Paginas.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Paginas.Location = new System.Drawing.Point(87, 19);
            this.Lbl_Paginas.Name = "Lbl_Paginas";
            this.Lbl_Paginas.Size = new System.Drawing.Size(267, 20);
            this.Lbl_Paginas.TabIndex = 51;
            this.Lbl_Paginas.Text = "MOSTRANDO 1-10 DE 100 CHEQUES";
            // 
            // Btn_SelectAll
            // 
            this.Btn_SelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_SelectAll.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SelectAll.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_SelectAll.Location = new System.Drawing.Point(51, 11);
            this.Btn_SelectAll.Name = "Btn_SelectAll";
            this.Btn_SelectAll.Size = new System.Drawing.Size(35, 35);
            this.Btn_SelectAll.TabIndex = 85;
            this.Btn_SelectAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SelectAll.UseVisualStyleBackColor = true;
            this.Btn_SelectAll.Click += new System.EventHandler(this.Btn_SelectAll_Click);
            // 
            // Frm_Checks_Predeclarations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.PanelToolStrip);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_DetalleTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_Checks_Predeclarations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - PREDECLARACIÓN DE CHEQUES";
            this.Load += new System.EventHandler(this.Frm_Checks_Predeclarations_Load);
            this.panel1.ResumeLayout(false);
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.Panel_DetalleTabla.ResumeLayout(false);
            this.Panel_DetalleTabla.PerformLayout();
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.PanelToolStrip.ResumeLayout(false);
            this.PanelToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Btn_No;
        private System.Windows.Forms.Button Btn_Yes;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel Panel_DetalleTabla;
        private System.Windows.Forms.Button Btn_YesSelected;
        private System.Windows.Forms.Label Lbl_BuscarPor;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Label Lbl_ValorBuscado;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.ComboBox Filtro3;
        private System.Windows.Forms.ComboBox Filtro2;
        private System.Windows.Forms.ComboBox Filtro1;
        private System.Windows.Forms.CheckBox CheckBox_FiltroFechas;
        private System.Windows.Forms.DateTimePicker DTP_FechaFin;
        private System.Windows.Forms.Label Lbl_DTPFin;
        private System.Windows.Forms.Label Lbl_DTPInicio;
        private System.Windows.Forms.DateTimePicker DTP_FechaInicio;
        private System.Windows.Forms.Panel PanelToolStrip;
        private System.Windows.Forms.Label Lbl_Paginas;
        private System.Windows.Forms.TextBox Txt_Total;
        private System.Windows.Forms.Label Lbl_Total;
        private System.Windows.Forms.Button Btn_CSV;
        private System.Windows.Forms.Button Btn_LimpiarSeleccion;
        private System.Windows.Forms.Button Btn_SelectAll;
    }
}