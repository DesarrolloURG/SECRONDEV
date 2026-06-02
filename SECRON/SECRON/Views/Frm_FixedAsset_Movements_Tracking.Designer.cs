namespace SECRON.Views
{
    partial class Frm_FixedAsset_Movements_Tracking
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_FixedAsset_Movements_Tracking));
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.PanelEncabezadoTabla = new System.Windows.Forms.Panel();
            this.Btn_Yes = new System.Windows.Forms.Button();
            this.ComboBox_NewState = new System.Windows.Forms.ComboBox();
            this.EncabezadoTabla = new System.Windows.Forms.Label();
            this.Lbl_UpdateState = new System.Windows.Forms.Label();
            this.Panel_DetalleTabla = new System.Windows.Forms.Panel();
            this.Txt_Reason = new System.Windows.Forms.TextBox();
            this.Lbl_Reason = new System.Windows.Forms.Label();
            this.Lbl_Info = new System.Windows.Forms.Label();
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
            this.Btn_Export = new System.Windows.Forms.Button();
            this.Btn_CancelTransfer = new System.Windows.Forms.Button();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.PanelTablaDetalle = new System.Windows.Forms.Panel();
            this.TablaDetalles = new System.Windows.Forms.DataGridView();
            this.PanelEncabezadoTablaDetalle = new System.Windows.Forms.Panel();
            this.Btn_RemoveAsset = new System.Windows.Forms.Button();
            this.EncabezadoTablaDetalle = new System.Windows.Forms.Label();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.PanelEncabezadoTabla.SuspendLayout();
            this.Panel_DetalleTabla.SuspendLayout();
            this.Panel_Superior.SuspendLayout();
            this.PanelTablaDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TablaDetalles)).BeginInit();
            this.PanelEncabezadoTablaDetalle.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Controls.Add(this.PanelEncabezadoTabla);
            this.PanelTabla.Location = new System.Drawing.Point(9, 308);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(1163, 181);
            this.PanelTabla.TabIndex = 85;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 37);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(1163, 144);
            this.Tabla.TabIndex = 87;
            this.Tabla.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tabla_CellClick);
            // 
            // PanelEncabezadoTabla
            // 
            this.PanelEncabezadoTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.PanelEncabezadoTabla.Controls.Add(this.Btn_Yes);
            this.PanelEncabezadoTabla.Controls.Add(this.ComboBox_NewState);
            this.PanelEncabezadoTabla.Controls.Add(this.EncabezadoTabla);
            this.PanelEncabezadoTabla.Controls.Add(this.Lbl_UpdateState);
            this.PanelEncabezadoTabla.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelEncabezadoTabla.Location = new System.Drawing.Point(0, 0);
            this.PanelEncabezadoTabla.Name = "PanelEncabezadoTabla";
            this.PanelEncabezadoTabla.Size = new System.Drawing.Size(1163, 37);
            this.PanelEncabezadoTabla.TabIndex = 86;
            // 
            // Btn_Yes
            // 
            this.Btn_Yes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Yes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Yes.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Yes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Yes.Location = new System.Drawing.Point(949, 3);
            this.Btn_Yes.Name = "Btn_Yes";
            this.Btn_Yes.Size = new System.Drawing.Size(211, 30);
            this.Btn_Yes.TabIndex = 65;
            this.Btn_Yes.Text = "CONFIRMAR CAMBIOS";
            this.Btn_Yes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Yes.UseVisualStyleBackColor = true;
            this.Btn_Yes.Click += new System.EventHandler(this.Btn_Yes_Click);
            // 
            // ComboBox_NewState
            // 
            this.ComboBox_NewState.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ComboBox_NewState.FormattingEnabled = true;
            this.ComboBox_NewState.Location = new System.Drawing.Point(668, 5);
            this.ComboBox_NewState.Name = "ComboBox_NewState";
            this.ComboBox_NewState.Size = new System.Drawing.Size(275, 26);
            this.ComboBox_NewState.TabIndex = 78;
            // 
            // EncabezadoTabla
            // 
            this.EncabezadoTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.EncabezadoTabla.AutoSize = true;
            this.EncabezadoTabla.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.EncabezadoTabla.ForeColor = System.Drawing.Color.Black;
            this.EncabezadoTabla.Location = new System.Drawing.Point(9, 5);
            this.EncabezadoTabla.Name = "EncabezadoTabla";
            this.EncabezadoTabla.Size = new System.Drawing.Size(378, 25);
            this.EncabezadoTabla.TabIndex = 50;
            this.EncabezadoTabla.Text = "INFORMACIÓN GENERAL DEL TRASLADO";
            // 
            // Lbl_UpdateState
            // 
            this.Lbl_UpdateState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_UpdateState.AutoSize = true;
            this.Lbl_UpdateState.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_UpdateState.ForeColor = System.Drawing.Color.Black;
            this.Lbl_UpdateState.Location = new System.Drawing.Point(426, 7);
            this.Lbl_UpdateState.Name = "Lbl_UpdateState";
            this.Lbl_UpdateState.Size = new System.Drawing.Size(236, 20);
            this.Lbl_UpdateState.TabIndex = 77;
            this.Lbl_UpdateState.Text = "NUEVO ESTADO DEL TRASLADO";
            // 
            // Panel_DetalleTabla
            // 
            this.Panel_DetalleTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_DetalleTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_DetalleTabla.Controls.Add(this.Txt_Reason);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_Reason);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_Info);
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
            this.Panel_DetalleTabla.Location = new System.Drawing.Point(9, 67);
            this.Panel_DetalleTabla.Name = "Panel_DetalleTabla";
            this.Panel_DetalleTabla.Size = new System.Drawing.Size(1163, 235);
            this.Panel_DetalleTabla.TabIndex = 84;
            // 
            // Txt_Reason
            // 
            this.Txt_Reason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_Reason.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Reason.Location = new System.Drawing.Point(95, 65);
            this.Txt_Reason.MaxLength = 15;
            this.Txt_Reason.Multiline = true;
            this.Txt_Reason.Name = "Txt_Reason";
            this.Txt_Reason.Size = new System.Drawing.Size(1053, 78);
            this.Txt_Reason.TabIndex = 91;
            // 
            // Lbl_Reason
            // 
            this.Lbl_Reason.AutoSize = true;
            this.Lbl_Reason.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Reason.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Reason.Location = new System.Drawing.Point(10, 65);
            this.Lbl_Reason.Name = "Lbl_Reason";
            this.Lbl_Reason.Size = new System.Drawing.Size(79, 20);
            this.Lbl_Reason.TabIndex = 92;
            this.Lbl_Reason.Text = "MOTIVO *";
            // 
            // Lbl_Info
            // 
            this.Lbl_Info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Info.AutoSize = true;
            this.Lbl_Info.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Info.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Info.Location = new System.Drawing.Point(860, 197);
            this.Lbl_Info.Name = "Lbl_Info";
            this.Lbl_Info.Size = new System.Drawing.Size(288, 20);
            this.Lbl_Info.TabIndex = 89;
            this.Lbl_Info.Text = "MOSTRANDO 1-10 DE 100 TRASLADOS";
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
            this.CheckBox_FiltroFechas.CheckedChanged += new System.EventHandler(this.CheckBox_FiltroFechas_CheckedChanged);
            // 
            // DTP_FechaFin
            // 
            this.DTP_FechaFin.Location = new System.Drawing.Point(873, 29);
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
            this.DTP_FechaInicio.Location = new System.Drawing.Point(587, 29);
            this.DTP_FechaInicio.Name = "DTP_FechaInicio";
            this.DTP_FechaInicio.Size = new System.Drawing.Size(275, 20);
            this.DTP_FechaInicio.TabIndex = 79;
            // 
            // Filtro3
            // 
            this.Filtro3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Filtro3.FormattingEnabled = true;
            this.Filtro3.Location = new System.Drawing.Point(576, 197);
            this.Filtro3.Name = "Filtro3";
            this.Filtro3.Size = new System.Drawing.Size(275, 26);
            this.Filtro3.TabIndex = 77;
            // 
            // Filtro2
            // 
            this.Filtro2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Filtro2.FormattingEnabled = true;
            this.Filtro2.Location = new System.Drawing.Point(295, 197);
            this.Filtro2.Name = "Filtro2";
            this.Filtro2.Size = new System.Drawing.Size(275, 26);
            this.Filtro2.TabIndex = 76;
            // 
            // ComboBox_Estado
            // 
            this.ComboBox_Estado.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ComboBox_Estado.FormattingEnabled = true;
            this.ComboBox_Estado.Location = new System.Drawing.Point(14, 28);
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
            this.LblEstado.Size = new System.Drawing.Size(265, 20);
            this.LblEstado.TabIndex = 73;
            this.LblEstado.Text = "CAMBIAR A ESTADO DE TRASLADO:";
            // 
            // Filtro1
            // 
            this.Filtro1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Filtro1.FormattingEnabled = true;
            this.Filtro1.Location = new System.Drawing.Point(14, 197);
            this.Filtro1.Name = "Filtro1";
            this.Filtro1.Size = new System.Drawing.Size(275, 26);
            this.Filtro1.TabIndex = 71;
            // 
            // Lbl_BuscarPor
            // 
            this.Lbl_BuscarPor.AutoSize = true;
            this.Lbl_BuscarPor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_BuscarPor.ForeColor = System.Drawing.Color.Black;
            this.Lbl_BuscarPor.Location = new System.Drawing.Point(10, 159);
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
            this.Btn_Clear.Location = new System.Drawing.Point(1113, 149);
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
            this.Btn_Search.Location = new System.Drawing.Point(1072, 149);
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
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(127, 156);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(939, 27);
            this.Txt_ValorBuscado.TabIndex = 60;
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Btn_Export);
            this.Panel_Superior.Controls.Add(this.Btn_CancelTransfer);
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1184, 55);
            this.Panel_Superior.TabIndex = 83;
            // 
            // Btn_Export
            // 
            this.Btn_Export.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Export.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_Export.Image = global::SECRON.Properties.Resources.ExportarExcelNegro25x25;
            this.Btn_Export.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Export.Location = new System.Drawing.Point(1045, 13);
            this.Btn_Export.Name = "Btn_Export";
            this.Btn_Export.Size = new System.Drawing.Size(127, 30);
            this.Btn_Export.TabIndex = 56;
            this.Btn_Export.Text = "EXPORTAR";
            this.Btn_Export.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.Btn_Export.UseVisualStyleBackColor = true;
            this.Btn_Export.Click += new System.EventHandler(this.Btn_Export_Click);
            // 
            // Btn_CancelTransfer
            // 
            this.Btn_CancelTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_CancelTransfer.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_CancelTransfer.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_CancelTransfer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_CancelTransfer.Location = new System.Drawing.Point(830, 12);
            this.Btn_CancelTransfer.Name = "Btn_CancelTransfer";
            this.Btn_CancelTransfer.Size = new System.Drawing.Size(209, 31);
            this.Btn_CancelTransfer.TabIndex = 66;
            this.Btn_CancelTransfer.Text = "CANCELAR TRASLADO";
            this.Btn_CancelTransfer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_CancelTransfer.UseVisualStyleBackColor = true;
            this.Btn_CancelTransfer.Click += new System.EventHandler(this.Btn_CancelTransfer_Click);
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(364, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "TRACKING DE TRASLADOS DE ACTIVOS";
            // 
            // PanelTablaDetalle
            // 
            this.PanelTablaDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTablaDetalle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTablaDetalle.Controls.Add(this.TablaDetalles);
            this.PanelTablaDetalle.Controls.Add(this.PanelEncabezadoTablaDetalle);
            this.PanelTablaDetalle.Location = new System.Drawing.Point(9, 495);
            this.PanelTablaDetalle.Name = "PanelTablaDetalle";
            this.PanelTablaDetalle.Size = new System.Drawing.Size(1163, 338);
            this.PanelTablaDetalle.TabIndex = 87;
            // 
            // TablaDetalles
            // 
            this.TablaDetalles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TablaDetalles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TablaDetalles.Location = new System.Drawing.Point(0, 38);
            this.TablaDetalles.Name = "TablaDetalles";
            this.TablaDetalles.Size = new System.Drawing.Size(1163, 300);
            this.TablaDetalles.TabIndex = 85;
            this.TablaDetalles.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.TablaDetalles_CellClick);
            // 
            // PanelEncabezadoTablaDetalle
            // 
            this.PanelEncabezadoTablaDetalle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.PanelEncabezadoTablaDetalle.Controls.Add(this.Btn_RemoveAsset);
            this.PanelEncabezadoTablaDetalle.Controls.Add(this.EncabezadoTablaDetalle);
            this.PanelEncabezadoTablaDetalle.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelEncabezadoTablaDetalle.Location = new System.Drawing.Point(0, 0);
            this.PanelEncabezadoTablaDetalle.Name = "PanelEncabezadoTablaDetalle";
            this.PanelEncabezadoTablaDetalle.Size = new System.Drawing.Size(1163, 38);
            this.PanelEncabezadoTablaDetalle.TabIndex = 84;
            // 
            // Btn_RemoveAsset
            // 
            this.Btn_RemoveAsset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_RemoveAsset.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_RemoveAsset.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_RemoveAsset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_RemoveAsset.Location = new System.Drawing.Point(491, 5);
            this.Btn_RemoveAsset.Name = "Btn_RemoveAsset";
            this.Btn_RemoveAsset.Size = new System.Drawing.Size(283, 30);
            this.Btn_RemoveAsset.TabIndex = 79;
            this.Btn_RemoveAsset.Text = "QUITAR ACTIVO DEL TRASLADO";
            this.Btn_RemoveAsset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_RemoveAsset.UseVisualStyleBackColor = true;
            this.Btn_RemoveAsset.Click += new System.EventHandler(this.Btn_RemoveAsset_Click);
            // 
            // EncabezadoTablaDetalle
            // 
            this.EncabezadoTablaDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.EncabezadoTablaDetalle.AutoSize = true;
            this.EncabezadoTablaDetalle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.EncabezadoTablaDetalle.ForeColor = System.Drawing.Color.Black;
            this.EncabezadoTablaDetalle.Location = new System.Drawing.Point(9, 7);
            this.EncabezadoTablaDetalle.Name = "EncabezadoTablaDetalle";
            this.EncabezadoTablaDetalle.Size = new System.Drawing.Size(476, 25);
            this.EncabezadoTablaDetalle.TabIndex = 50;
            this.EncabezadoTablaDetalle.Text = "DETALLES DEL TRASLADO - ACTIVOS MOVILIZADOS";
            // 
            // Frm_FixedAsset_Movements_Tracking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 845);
            this.Controls.Add(this.PanelTablaDetalle);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_DetalleTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_FixedAsset_Movements_Tracking";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - TRACKING DE TRASLADOS DE ACTIVOS";
            this.Load += new System.EventHandler(this.Frm_FixedAsset_Movements_Tracking_Load);
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.PanelEncabezadoTabla.ResumeLayout(false);
            this.PanelEncabezadoTabla.PerformLayout();
            this.Panel_DetalleTabla.ResumeLayout(false);
            this.Panel_DetalleTabla.PerformLayout();
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.PanelTablaDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TablaDetalles)).EndInit();
            this.PanelEncabezadoTablaDetalle.ResumeLayout(false);
            this.PanelEncabezadoTablaDetalle.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.Panel Panel_DetalleTabla;
        private System.Windows.Forms.Label Lbl_Info;
        private System.Windows.Forms.CheckBox CheckBox_FiltroFechas;
        private System.Windows.Forms.DateTimePicker DTP_FechaFin;
        private System.Windows.Forms.Label Lbl_DTPFin;
        private System.Windows.Forms.Label Lbl_DTPInicio;
        private System.Windows.Forms.DateTimePicker DTP_FechaInicio;
        private System.Windows.Forms.ComboBox Filtro3;
        private System.Windows.Forms.ComboBox Filtro2;
        private System.Windows.Forms.ComboBox ComboBox_Estado;
        private System.Windows.Forms.Label LblEstado;
        private System.Windows.Forms.ComboBox Filtro1;
        private System.Windows.Forms.Label Lbl_BuscarPor;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Label Lbl_ValorBuscado;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Button Btn_Export;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.TextBox Txt_Reason;
        private System.Windows.Forms.Label Lbl_Reason;
        private System.Windows.Forms.Panel PanelTablaDetalle;
        private System.Windows.Forms.DataGridView TablaDetalles;
        private System.Windows.Forms.Panel PanelEncabezadoTablaDetalle;
        private System.Windows.Forms.Label EncabezadoTablaDetalle;
        private System.Windows.Forms.Panel PanelEncabezadoTabla;
        private System.Windows.Forms.Label EncabezadoTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Label Lbl_UpdateState;
        private System.Windows.Forms.Button Btn_CancelTransfer;
        private System.Windows.Forms.Button Btn_Yes;
        private System.Windows.Forms.ComboBox ComboBox_NewState;
        private System.Windows.Forms.Button Btn_RemoveAsset;
    }
}