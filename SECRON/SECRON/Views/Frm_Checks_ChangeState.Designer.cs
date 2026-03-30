namespace SECRON.Views
{
    partial class Frm_Checks_ChangeStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Checks_ChangeStatus));
            this.panel1 = new System.Windows.Forms.Panel();
            this.Lbl_Paginas = new System.Windows.Forms.Label();
            this.Btn_No = new System.Windows.Forms.Button();
            this.Btn_Yes = new System.Windows.Forms.Button();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Panel_DetalleTabla = new System.Windows.Forms.Panel();
            this.Btn_LimpiarSeleccion = new System.Windows.Forms.Button();
            this.Lbl_Info = new System.Windows.Forms.Label();
            this.CheckBox_Rango = new System.Windows.Forms.CheckBox();
            this.Lbl_fin = new System.Windows.Forms.Label();
            this.Txt_Fin = new System.Windows.Forms.TextBox();
            this.Lbl_Li = new System.Windows.Forms.Label();
            this.Txt_Li = new System.Windows.Forms.TextBox();
            this.CheckBox_FiltroFechas = new System.Windows.Forms.CheckBox();
            this.DTP_FechaFin = new System.Windows.Forms.DateTimePicker();
            this.Lbl_DTPFin = new System.Windows.Forms.Label();
            this.Lbl_DTPInicio = new System.Windows.Forms.Label();
            this.DTP_FechaInicio = new System.Windows.Forms.DateTimePicker();
            this.Filtro3 = new System.Windows.Forms.ComboBox();
            this.Filtro2 = new System.Windows.Forms.ComboBox();
            this.ComboBox_Estado = new System.Windows.Forms.ComboBox();
            this.LblEstado = new System.Windows.Forms.Label();
            this.Filtro1 = new System.Windows.Forms.ComboBox();
            this.Lbl_BuscarPor = new System.Windows.Forms.Label();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Lbl_ValorBuscado = new System.Windows.Forms.Label();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Btn_RevertirAnulacion = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.Panel_DetalleTabla.SuspendLayout();
            this.Panel_Superior.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.Lbl_Paginas);
            this.panel1.Controls.Add(this.Btn_No);
            this.panel1.Controls.Add(this.Btn_Yes);
            this.panel1.Location = new System.Drawing.Point(9, 800);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1165, 49);
            this.panel1.TabIndex = 82;
            // 
            // Lbl_Paginas
            // 
            this.Lbl_Paginas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Paginas.AutoSize = true;
            this.Lbl_Paginas.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Paginas.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Paginas.Location = new System.Drawing.Point(10, 12);
            this.Lbl_Paginas.Name = "Lbl_Paginas";
            this.Lbl_Paginas.Size = new System.Drawing.Size(267, 20);
            this.Lbl_Paginas.TabIndex = 77;
            this.Lbl_Paginas.Text = "MOSTRANDO 1-10 DE 100 CHEQUES";
            // 
            // Btn_No
            // 
            this.Btn_No.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_No.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_No.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_No.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_No.Location = new System.Drawing.Point(906, 3);
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
            this.Btn_Yes.Location = new System.Drawing.Point(1036, 3);
            this.Btn_Yes.Name = "Btn_Yes";
            this.Btn_Yes.Size = new System.Drawing.Size(117, 37);
            this.Btn_Yes.TabIndex = 65;
            this.Btn_Yes.Text = "ACEPTAR";
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
            this.PanelTabla.Location = new System.Drawing.Point(9, 261);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(1165, 533);
            this.PanelTabla.TabIndex = 81;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(1165, 533);
            this.Tabla.TabIndex = 1;
            // 
            // Panel_DetalleTabla
            // 
            this.Panel_DetalleTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_DetalleTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_DetalleTabla.Controls.Add(this.Btn_LimpiarSeleccion);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_Info);
            this.Panel_DetalleTabla.Controls.Add(this.CheckBox_Rango);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_fin);
            this.Panel_DetalleTabla.Controls.Add(this.Txt_Fin);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_Li);
            this.Panel_DetalleTabla.Controls.Add(this.Txt_Li);
            this.Panel_DetalleTabla.Controls.Add(this.CheckBox_FiltroFechas);
            this.Panel_DetalleTabla.Controls.Add(this.DTP_FechaFin);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_DTPFin);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_DTPInicio);
            this.Panel_DetalleTabla.Controls.Add(this.DTP_FechaInicio);
            this.Panel_DetalleTabla.Controls.Add(this.Filtro3);
            this.Panel_DetalleTabla.Controls.Add(this.Filtro2);
            this.Panel_DetalleTabla.Controls.Add(this.ComboBox_Estado);
            this.Panel_DetalleTabla.Controls.Add(this.LblEstado);
            this.Panel_DetalleTabla.Controls.Add(this.Filtro1);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_BuscarPor);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Clear);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Search);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_ValorBuscado);
            this.Panel_DetalleTabla.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_DetalleTabla.Location = new System.Drawing.Point(9, 61);
            this.Panel_DetalleTabla.Name = "Panel_DetalleTabla";
            this.Panel_DetalleTabla.Size = new System.Drawing.Size(1163, 194);
            this.Panel_DetalleTabla.TabIndex = 80;
            // 
            // Btn_LimpiarSeleccion
            // 
            this.Btn_LimpiarSeleccion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_LimpiarSeleccion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_LimpiarSeleccion.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_LimpiarSeleccion.Location = new System.Drawing.Point(1114, 98);
            this.Btn_LimpiarSeleccion.Name = "Btn_LimpiarSeleccion";
            this.Btn_LimpiarSeleccion.Size = new System.Drawing.Size(35, 45);
            this.Btn_LimpiarSeleccion.TabIndex = 90;
            this.Btn_LimpiarSeleccion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_LimpiarSeleccion.UseVisualStyleBackColor = true;
            this.Btn_LimpiarSeleccion.Click += new System.EventHandler(this.Btn_LimpiarSeleccion_Click);
            // 
            // Lbl_Info
            // 
            this.Lbl_Info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Info.AutoSize = true;
            this.Lbl_Info.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Info.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Info.Location = new System.Drawing.Point(857, 158);
            this.Lbl_Info.Name = "Lbl_Info";
            this.Lbl_Info.Size = new System.Drawing.Size(267, 20);
            this.Lbl_Info.TabIndex = 89;
            this.Lbl_Info.Text = "MOSTRANDO 1-10 DE 100 CHEQUES";
            // 
            // CheckBox_Rango
            // 
            this.CheckBox_Rango.AutoSize = true;
            this.CheckBox_Rango.Checked = true;
            this.CheckBox_Rango.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_Rango.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.CheckBox_Rango.Location = new System.Drawing.Point(401, 68);
            this.CheckBox_Rango.Name = "CheckBox_Rango";
            this.CheckBox_Rango.Size = new System.Drawing.Size(161, 23);
            this.CheckBox_Rango.TabIndex = 88;
            this.CheckBox_Rango.Text = "FILTRO POR RANGO";
            this.CheckBox_Rango.UseVisualStyleBackColor = true;
            // 
            // Lbl_fin
            // 
            this.Lbl_fin.AutoSize = true;
            this.Lbl_fin.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_fin.ForeColor = System.Drawing.Color.Black;
            this.Lbl_fin.Location = new System.Drawing.Point(870, 68);
            this.Lbl_fin.Name = "Lbl_fin";
            this.Lbl_fin.Size = new System.Drawing.Size(106, 20);
            this.Lbl_fin.TabIndex = 87;
            this.Lbl_fin.Text = "LIMITE FINAL";
            // 
            // Txt_Fin
            // 
            this.Txt_Fin.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Fin.Location = new System.Drawing.Point(982, 65);
            this.Txt_Fin.MaxLength = 15;
            this.Txt_Fin.Name = "Txt_Fin";
            this.Txt_Fin.Size = new System.Drawing.Size(167, 27);
            this.Txt_Fin.TabIndex = 86;
            // 
            // Lbl_Li
            // 
            this.Lbl_Li.AutoSize = true;
            this.Lbl_Li.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Li.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Li.Location = new System.Drawing.Point(583, 68);
            this.Lbl_Li.Name = "Lbl_Li";
            this.Lbl_Li.Size = new System.Drawing.Size(117, 20);
            this.Lbl_Li.TabIndex = 85;
            this.Lbl_Li.Text = "LIMITE INICIAL";
            // 
            // Txt_Li
            // 
            this.Txt_Li.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Li.Location = new System.Drawing.Point(706, 65);
            this.Txt_Li.MaxLength = 15;
            this.Txt_Li.Name = "Txt_Li";
            this.Txt_Li.Size = new System.Drawing.Size(156, 27);
            this.Txt_Li.TabIndex = 84;
            // 
            // CheckBox_FiltroFechas
            // 
            this.CheckBox_FiltroFechas.AutoSize = true;
            this.CheckBox_FiltroFechas.Checked = true;
            this.CheckBox_FiltroFechas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_FiltroFechas.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.CheckBox_FiltroFechas.Location = new System.Drawing.Point(401, 6);
            this.CheckBox_FiltroFechas.Name = "CheckBox_FiltroFechas";
            this.CheckBox_FiltroFechas.Size = new System.Drawing.Size(162, 23);
            this.CheckBox_FiltroFechas.TabIndex = 82;
            this.CheckBox_FiltroFechas.Text = "FILTRO POR FECHAS";
            this.CheckBox_FiltroFechas.UseVisualStyleBackColor = true;
            // 
            // DTP_FechaFin
            // 
            this.DTP_FechaFin.Location = new System.Drawing.Point(874, 39);
            this.DTP_FechaFin.Name = "DTP_FechaFin";
            this.DTP_FechaFin.Size = new System.Drawing.Size(275, 20);
            this.DTP_FechaFin.TabIndex = 81;
            // 
            // Lbl_DTPFin
            // 
            this.Lbl_DTPFin.AutoSize = true;
            this.Lbl_DTPFin.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_DTPFin.ForeColor = System.Drawing.Color.Black;
            this.Lbl_DTPFin.Location = new System.Drawing.Point(869, 6);
            this.Lbl_DTPFin.Name = "Lbl_DTPFin";
            this.Lbl_DTPFin.Size = new System.Drawing.Size(157, 20);
            this.Lbl_DTPFin.TabIndex = 80;
            this.Lbl_DTPFin.Text = "FECHA LÍMITE FINAL";
            // 
            // Lbl_DTPInicio
            // 
            this.Lbl_DTPInicio.AutoSize = true;
            this.Lbl_DTPInicio.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_DTPInicio.ForeColor = System.Drawing.Color.Black;
            this.Lbl_DTPInicio.Location = new System.Drawing.Point(583, 6);
            this.Lbl_DTPInicio.Name = "Lbl_DTPInicio";
            this.Lbl_DTPInicio.Size = new System.Drawing.Size(168, 20);
            this.Lbl_DTPInicio.TabIndex = 78;
            this.Lbl_DTPInicio.Text = "FECHA LÍMITE INICIAL";
            // 
            // DTP_FechaInicio
            // 
            this.DTP_FechaInicio.Location = new System.Drawing.Point(587, 39);
            this.DTP_FechaInicio.Name = "DTP_FechaInicio";
            this.DTP_FechaInicio.Size = new System.Drawing.Size(275, 20);
            this.DTP_FechaInicio.TabIndex = 79;
            // 
            // Filtro3
            // 
            this.Filtro3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Filtro3.FormattingEnabled = true;
            this.Filtro3.Location = new System.Drawing.Point(576, 156);
            this.Filtro3.Name = "Filtro3";
            this.Filtro3.Size = new System.Drawing.Size(275, 26);
            this.Filtro3.TabIndex = 77;
            // 
            // Filtro2
            // 
            this.Filtro2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Filtro2.FormattingEnabled = true;
            this.Filtro2.Location = new System.Drawing.Point(295, 156);
            this.Filtro2.Name = "Filtro2";
            this.Filtro2.Size = new System.Drawing.Size(275, 26);
            this.Filtro2.TabIndex = 76;
            // 
            // ComboBox_Estado
            // 
            this.ComboBox_Estado.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ComboBox_Estado.FormattingEnabled = true;
            this.ComboBox_Estado.Location = new System.Drawing.Point(14, 39);
            this.ComboBox_Estado.Name = "ComboBox_Estado";
            this.ComboBox_Estado.Size = new System.Drawing.Size(325, 26);
            this.ComboBox_Estado.TabIndex = 74;
            // 
            // LblEstado
            // 
            this.LblEstado.AutoSize = true;
            this.LblEstado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.LblEstado.ForeColor = System.Drawing.Color.Black;
            this.LblEstado.Location = new System.Drawing.Point(10, 6);
            this.LblEstado.Name = "LblEstado";
            this.LblEstado.Size = new System.Drawing.Size(244, 20);
            this.LblEstado.TabIndex = 73;
            this.LblEstado.Text = "CAMBIAR A ESTADO DE CHEQUE:";
            // 
            // Filtro1
            // 
            this.Filtro1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Filtro1.FormattingEnabled = true;
            this.Filtro1.Location = new System.Drawing.Point(14, 156);
            this.Filtro1.Name = "Filtro1";
            this.Filtro1.Size = new System.Drawing.Size(275, 26);
            this.Filtro1.TabIndex = 71;
            // 
            // Lbl_BuscarPor
            // 
            this.Lbl_BuscarPor.AutoSize = true;
            this.Lbl_BuscarPor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_BuscarPor.ForeColor = System.Drawing.Color.Black;
            this.Lbl_BuscarPor.Location = new System.Drawing.Point(10, 133);
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
            this.Btn_Clear.Location = new System.Drawing.Point(1075, 98);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(35, 45);
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
            this.Btn_Search.Location = new System.Drawing.Point(1034, 98);
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
            this.Lbl_ValorBuscado.Size = new System.Drawing.Size(13, 20);
            this.Lbl_ValorBuscado.TabIndex = 61;
            this.Lbl_ValorBuscado.Text = " ";
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(14, 103);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(1016, 27);
            this.Txt_ValorBuscado.TabIndex = 60;
            this.Txt_ValorBuscado.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado_KeyDown);
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Btn_RevertirAnulacion);
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1184, 55);
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(325, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "ACTUALIZAR ESTADO DE CHEQUES";
            // 
            // Btn_RevertirAnulacion
            // 
            this.Btn_RevertirAnulacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_RevertirAnulacion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_RevertirAnulacion.Image = global::SECRON.Properties.Resources.AlertaNegro25x25;
            this.Btn_RevertirAnulacion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_RevertirAnulacion.Location = new System.Drawing.Point(965, 13);
            this.Btn_RevertirAnulacion.Name = "Btn_RevertirAnulacion";
            this.Btn_RevertirAnulacion.Size = new System.Drawing.Size(207, 30);
            this.Btn_RevertirAnulacion.TabIndex = 56;
            this.Btn_RevertirAnulacion.Text = "REVERTIR ANULACIÓN";
            this.Btn_RevertirAnulacion.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.Btn_RevertirAnulacion.UseVisualStyleBackColor = true;
            this.Btn_RevertirAnulacion.Click += new System.EventHandler(this.Btn_RevertirAnulacion_Click);
            // 
            // Frm_Checks_ChangeStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_DetalleTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_Checks_ChangeStatus";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - ACTUALIZAR ESTADO DE CHEQUES";
            this.Load += new System.EventHandler(this.Frm_Checks_ChangeState_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.Panel_DetalleTabla.ResumeLayout(false);
            this.Panel_DetalleTabla.PerformLayout();
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Btn_No;
        private System.Windows.Forms.Button Btn_Yes;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel Panel_DetalleTabla;
        private System.Windows.Forms.ComboBox Filtro1;
        private System.Windows.Forms.Label Lbl_BuscarPor;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Label Lbl_ValorBuscado;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.ComboBox ComboBox_Estado;
        private System.Windows.Forms.Label LblEstado;
        private System.Windows.Forms.ComboBox Filtro3;
        private System.Windows.Forms.ComboBox Filtro2;
        private System.Windows.Forms.CheckBox CheckBox_FiltroFechas;
        private System.Windows.Forms.DateTimePicker DTP_FechaFin;
        private System.Windows.Forms.Label Lbl_DTPFin;
        private System.Windows.Forms.Label Lbl_DTPInicio;
        private System.Windows.Forms.DateTimePicker DTP_FechaInicio;
        private System.Windows.Forms.Label Lbl_Paginas;
        private System.Windows.Forms.CheckBox CheckBox_Rango;
        private System.Windows.Forms.Label Lbl_fin;
        private System.Windows.Forms.TextBox Txt_Fin;
        private System.Windows.Forms.Label Lbl_Li;
        private System.Windows.Forms.TextBox Txt_Li;
        private System.Windows.Forms.Label Lbl_Info;
        private System.Windows.Forms.Button Btn_LimpiarSeleccion;
        private System.Windows.Forms.Button Btn_RevertirAnulacion;
    }
}