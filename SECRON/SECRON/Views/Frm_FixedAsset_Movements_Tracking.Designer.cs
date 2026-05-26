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
            this.panel1 = new System.Windows.Forms.Panel();
            this.Lbl_Paginas = new System.Windows.Forms.Label();
            this.Btn_No = new System.Windows.Forms.Button();
            this.Btn_Yes = new System.Windows.Forms.Button();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Panel_DetalleTabla = new System.Windows.Forms.Panel();
            this.Btn_LimpiarSeleccion = new System.Windows.Forms.Button();
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
            this.Btn_RevertirAnulacion = new System.Windows.Forms.Button();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Txt_Reason = new System.Windows.Forms.TextBox();
            this.Lbl_Reason = new System.Windows.Forms.Label();
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
            this.panel1.Location = new System.Drawing.Point(9, 806);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1165, 49);
            this.panel1.TabIndex = 86;
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
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(9, 308);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(1165, 492);
            this.PanelTabla.TabIndex = 85;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(1165, 492);
            this.Tabla.TabIndex = 1;
            // 
            // Panel_DetalleTabla
            // 
            this.Panel_DetalleTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_DetalleTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_DetalleTabla.Controls.Add(this.Txt_Reason);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_Reason);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_LimpiarSeleccion);
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
            // Btn_LimpiarSeleccion
            // 
            this.Btn_LimpiarSeleccion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_LimpiarSeleccion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_LimpiarSeleccion.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_LimpiarSeleccion.Location = new System.Drawing.Point(1113, 149);
            this.Btn_LimpiarSeleccion.Name = "Btn_LimpiarSeleccion";
            this.Btn_LimpiarSeleccion.Size = new System.Drawing.Size(35, 45);
            this.Btn_LimpiarSeleccion.TabIndex = 90;
            this.Btn_LimpiarSeleccion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_LimpiarSeleccion.UseVisualStyleBackColor = true;
            // 
            // Lbl_Info
            // 
            this.Lbl_Info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Info.AutoSize = true;
            this.Lbl_Info.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Info.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Info.Location = new System.Drawing.Point(869, 203);
            this.Lbl_Info.Name = "Lbl_Info";
            this.Lbl_Info.Size = new System.Drawing.Size(267, 20);
            this.Lbl_Info.TabIndex = 89;
            this.Lbl_Info.Text = "MOSTRANDO 1-10 DE 100 CHEQUES";
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
            this.Btn_Clear.Location = new System.Drawing.Point(1073, 149);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(35, 45);
            this.Btn_Clear.TabIndex = 63;
            this.Btn_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.Location = new System.Drawing.Point(1036, 149);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(35, 45);
            this.Btn_Search.TabIndex = 62;
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
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
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(903, 27);
            this.Txt_ValorBuscado.TabIndex = 60;
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
            this.Panel_Superior.TabIndex = 83;
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
            // Frm_FixedAsset_Movements_Tracking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_DetalleTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_FixedAsset_Movements_Tracking";
            this.Text = "SECRON - TRACKING DE TRASLADOS DE ACTIVOS";
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
        private System.Windows.Forms.Label Lbl_Paginas;
        private System.Windows.Forms.Button Btn_No;
        private System.Windows.Forms.Button Btn_Yes;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel Panel_DetalleTabla;
        private System.Windows.Forms.Button Btn_LimpiarSeleccion;
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
        private System.Windows.Forms.Button Btn_RevertirAnulacion;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.TextBox Txt_Reason;
        private System.Windows.Forms.Label Lbl_Reason;
    }
}