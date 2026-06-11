namespace SECRON.Views
{
    partial class Frm_ITSM_Technology_ResponsabilityLetter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ITSM_Technology_ResponsabilityLetter));
            this.Panel_Print = new System.Windows.Forms.Panel();
            this.Btn_Cancel = new System.Windows.Forms.Button();
            this.Lbl_Info = new System.Windows.Forms.Label();
            this.Btn_Print = new System.Windows.Forms.Button();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.ComboBox_Employee = new System.Windows.Forms.ComboBox();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Lbl_Subtitulo2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.Lbl_Subtitulo3 = new System.Windows.Forms.Label();
            this.Panel_TablaDetalle = new System.Windows.Forms.Panel();
            this.TablaDetalle = new System.Windows.Forms.DataGridView();
            this.Panel_Print.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.Panel_Superior.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.Panel_TablaDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TablaDetalle)).BeginInit();
            this.SuspendLayout();
            // 
            // Panel_Print
            // 
            this.Panel_Print.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Print.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Print.Controls.Add(this.Btn_Cancel);
            this.Panel_Print.Controls.Add(this.Lbl_Info);
            this.Panel_Print.Controls.Add(this.Btn_Print);
            this.Panel_Print.Location = new System.Drawing.Point(9, 806);
            this.Panel_Print.Name = "Panel_Print";
            this.Panel_Print.Size = new System.Drawing.Size(1165, 49);
            this.Panel_Print.TabIndex = 86;
            // 
            // Btn_Cancel
            // 
            this.Btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Cancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Cancel.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Cancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Cancel.Location = new System.Drawing.Point(931, 6);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(124, 37);
            this.Btn_Cancel.TabIndex = 66;
            this.Btn_Cancel.Text = "CANCELAR";
            this.Btn_Cancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Cancel.UseVisualStyleBackColor = true;
            this.Btn_Cancel.Click += new System.EventHandler(this.Btn_Cancel_Click);
            // 
            // Lbl_Info
            // 
            this.Lbl_Info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Info.AutoSize = true;
            this.Lbl_Info.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Info.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Info.Location = new System.Drawing.Point(10, 15);
            this.Lbl_Info.Name = "Lbl_Info";
            this.Lbl_Info.Size = new System.Drawing.Size(264, 20);
            this.Lbl_Info.TabIndex = 89;
            this.Lbl_Info.Text = "MOSTRANDO 1-10 DE 100 ACTIVOS";
            // 
            // Btn_Print
            // 
            this.Btn_Print.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Print.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Print.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Print.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Print.Location = new System.Drawing.Point(1061, 6);
            this.Btn_Print.Name = "Btn_Print";
            this.Btn_Print.Size = new System.Drawing.Size(92, 37);
            this.Btn_Print.TabIndex = 65;
            this.Btn_Print.Text = "EMITIR";
            this.Btn_Print.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Print.UseVisualStyleBackColor = true;
            this.Btn_Print.Click += new System.EventHandler(this.Btn_Print_Click);
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(13, 146);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(1161, 199);
            this.PanelTabla.TabIndex = 85;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(1161, 199);
            this.Tabla.TabIndex = 1;
            // 
            // ComboBox_Employee
            // 
            this.ComboBox_Employee.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ComboBox_Employee.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ComboBox_Employee.FormattingEnabled = true;
            this.ComboBox_Employee.Location = new System.Drawing.Point(14, 36);
            this.ComboBox_Employee.Name = "ComboBox_Employee";
            this.ComboBox_Employee.Size = new System.Drawing.Size(555, 26);
            this.ComboBox_Employee.TabIndex = 74;
            this.ComboBox_Employee.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Empleado_SelectedIndexChanged);
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(656, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "EQUIPOS DE TECNOLOGÍA - EMISIÓN DE CARTAS DE RESPONSABILIDAD";
            // 
            // Lbl_Subtitulo2
            // 
            this.Lbl_Subtitulo2.AutoSize = true;
            this.Lbl_Subtitulo2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo2.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo2.Image = global::SECRON.Properties.Resources.InfoNegro20x20;
            this.Lbl_Subtitulo2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo2.Location = new System.Drawing.Point(10, 10);
            this.Lbl_Subtitulo2.Name = "Lbl_Subtitulo2";
            this.Lbl_Subtitulo2.Size = new System.Drawing.Size(396, 20);
            this.Lbl_Subtitulo2.TabIndex = 6;
            this.Lbl_Subtitulo2.Text = "      LISTADOS DE EQUIPOS ASIGNADOS A LA PERSONA";
            this.Lbl_Subtitulo2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.Lbl_Subtitulo2);
            this.panel2.Controls.Add(this.ComboBox_Employee);
            this.panel2.Location = new System.Drawing.Point(13, 67);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1161, 73);
            this.panel2.TabIndex = 88;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.Lbl_Subtitulo3);
            this.panel3.Location = new System.Drawing.Point(9, 351);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1165, 47);
            this.panel3.TabIndex = 89;
            // 
            // Lbl_Subtitulo3
            // 
            this.Lbl_Subtitulo3.AutoSize = true;
            this.Lbl_Subtitulo3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo3.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo3.Image = global::SECRON.Properties.Resources.InfoNegro20x20;
            this.Lbl_Subtitulo3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo3.Location = new System.Drawing.Point(10, 15);
            this.Lbl_Subtitulo3.Name = "Lbl_Subtitulo3";
            this.Lbl_Subtitulo3.Size = new System.Drawing.Size(418, 20);
            this.Lbl_Subtitulo3.TabIndex = 6;
            this.Lbl_Subtitulo3.Text = "      DETALLES POR IMPRIMIR DEL EQUIPO SELECCIONADO";
            this.Lbl_Subtitulo3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Panel_TablaDetalle
            // 
            this.Panel_TablaDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_TablaDetalle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_TablaDetalle.Controls.Add(this.TablaDetalle);
            this.Panel_TablaDetalle.Location = new System.Drawing.Point(9, 404);
            this.Panel_TablaDetalle.Name = "Panel_TablaDetalle";
            this.Panel_TablaDetalle.Size = new System.Drawing.Size(1165, 394);
            this.Panel_TablaDetalle.TabIndex = 90;
            // 
            // TablaDetalle
            // 
            this.TablaDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TablaDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TablaDetalle.Location = new System.Drawing.Point(0, 0);
            this.TablaDetalle.Name = "TablaDetalle";
            this.TablaDetalle.Size = new System.Drawing.Size(1165, 394);
            this.TablaDetalle.TabIndex = 1;
            // 
            // Frm_ITSM_Technology_ResponsabilityLetter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.Panel_TablaDetalle);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.Panel_Print);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_ITSM_Technology_ResponsabilityLetter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - EQUIPOS DE TECNOLOGÍA, EMISIÓN DE CARTA DE RESPONSABILIDAD";
            this.Load += new System.EventHandler(this.Frm_ITSM_Technology_ResponsabilityLetter_Load);
            this.Panel_Print.ResumeLayout(false);
            this.Panel_Print.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.Panel_TablaDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TablaDetalle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Print;
        private System.Windows.Forms.Button Btn_Cancel;
        private System.Windows.Forms.Button Btn_Print;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Label Lbl_Info;
        private System.Windows.Forms.ComboBox ComboBox_Employee;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Label Lbl_Subtitulo2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label Lbl_Subtitulo3;
        private System.Windows.Forms.Panel Panel_TablaDetalle;
        private System.Windows.Forms.DataGridView TablaDetalle;
    }
}