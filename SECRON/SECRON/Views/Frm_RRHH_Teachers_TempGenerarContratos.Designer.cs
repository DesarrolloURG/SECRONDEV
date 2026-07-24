namespace SECRON.Views
{
    partial class Frm_RRHH_Teachers_TempGenerarContratos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_RRHH_Teachers_TempGenerarContratos));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.ComboBox_Sede = new System.Windows.Forms.ComboBox();
            this.Lbl_Sede = new System.Windows.Forms.Label();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.PanelResumen = new System.Windows.Forms.Panel();
            this.Btn_Siguiente = new System.Windows.Forms.Button();
            this.Btn_Anterior = new System.Windows.Forms.Button();
            this.Lbl_Paginas = new System.Windows.Forms.Label();
            this.Btn_PrintAll = new System.Windows.Forms.Button();
            this.Btn_PrintOne = new System.Windows.Forms.Button();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.PanelContenedor = new System.Windows.Forms.Panel();
            this.PreviewContratos = new System.Windows.Forms.PrintPreviewControl();
            this.Panel_Superior.SuspendLayout();
            this.PanelResumen.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.PanelContenedor.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.ComboBox_Sede);
            this.Panel_Superior.Controls.Add(this.Lbl_Sede);
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1184, 55);
            this.Panel_Superior.TabIndex = 13;
            // 
            // ComboBox_Sede
            // 
            this.ComboBox_Sede.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.ComboBox_Sede.FormattingEnabled = true;
            this.ComboBox_Sede.Location = new System.Drawing.Point(794, 11);
            this.ComboBox_Sede.Name = "ComboBox_Sede";
            this.ComboBox_Sede.Size = new System.Drawing.Size(381, 29);
            this.ComboBox_Sede.TabIndex = 76;
            this.ComboBox_Sede.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Sede_SelectedIndexChanged);
            // 
            // Lbl_Sede
            // 
            this.Lbl_Sede.AutoSize = true;
            this.Lbl_Sede.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Lbl_Sede.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Sede.Location = new System.Drawing.Point(633, 14);
            this.Lbl_Sede.Name = "Lbl_Sede";
            this.Lbl_Sede.Size = new System.Drawing.Size(155, 21);
            this.Lbl_Sede.TabIndex = 75;
            this.Lbl_Sede.Text = "SEDE GESTIONADA";
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(316, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "GENERAR CONTRATOS DOCENTES";
            // 
            // PanelResumen
            // 
            this.PanelResumen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelResumen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelResumen.Controls.Add(this.Btn_Siguiente);
            this.PanelResumen.Controls.Add(this.Btn_Anterior);
            this.PanelResumen.Controls.Add(this.Lbl_Paginas);
            this.PanelResumen.Controls.Add(this.Btn_PrintAll);
            this.PanelResumen.Controls.Add(this.Btn_PrintOne);
            this.PanelResumen.Location = new System.Drawing.Point(10, 800);
            this.PanelResumen.Name = "PanelResumen";
            this.PanelResumen.Size = new System.Drawing.Size(1165, 49);
            this.PanelResumen.TabIndex = 84;
            // 
            // Btn_Siguiente
            // 
            this.Btn_Siguiente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Siguiente.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Siguiente.Image = global::SECRON.Properties.Resources.RightNegro25x25;
            this.Btn_Siguiente.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Siguiente.Location = new System.Drawing.Point(52, 5);
            this.Btn_Siguiente.Name = "Btn_Siguiente";
            this.Btn_Siguiente.Size = new System.Drawing.Size(34, 37);
            this.Btn_Siguiente.TabIndex = 79;
            this.Btn_Siguiente.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Siguiente.UseVisualStyleBackColor = true;
            this.Btn_Siguiente.Click += new System.EventHandler(this.Btn_Siguiente_Click);
            // 
            // Btn_Anterior
            // 
            this.Btn_Anterior.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Anterior.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Anterior.Image = global::SECRON.Properties.Resources.LeftNegro25x25;
            this.Btn_Anterior.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Anterior.Location = new System.Drawing.Point(12, 5);
            this.Btn_Anterior.Name = "Btn_Anterior";
            this.Btn_Anterior.Size = new System.Drawing.Size(34, 37);
            this.Btn_Anterior.TabIndex = 78;
            this.Btn_Anterior.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Anterior.UseVisualStyleBackColor = true;
            this.Btn_Anterior.Click += new System.EventHandler(this.Btn_Anterior_Click);
            // 
            // Lbl_Paginas
            // 
            this.Lbl_Paginas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Paginas.AutoSize = true;
            this.Lbl_Paginas.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Paginas.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Paginas.Location = new System.Drawing.Point(131, 14);
            this.Lbl_Paginas.Name = "Lbl_Paginas";
            this.Lbl_Paginas.Size = new System.Drawing.Size(267, 20);
            this.Lbl_Paginas.TabIndex = 77;
            this.Lbl_Paginas.Text = "MOSTRANDO 1-10 DE 100 CHEQUES";
            // 
            // Btn_PrintAll
            // 
            this.Btn_PrintAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_PrintAll.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_PrintAll.Image = global::SECRON.Properties.Resources.AlertaNegro25x25;
            this.Btn_PrintAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_PrintAll.Location = new System.Drawing.Point(703, 5);
            this.Btn_PrintAll.Name = "Btn_PrintAll";
            this.Btn_PrintAll.Size = new System.Drawing.Size(175, 37);
            this.Btn_PrintAll.TabIndex = 66;
            this.Btn_PrintAll.Text = "GENERAR TODOS";
            this.Btn_PrintAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_PrintAll.UseVisualStyleBackColor = true;
            this.Btn_PrintAll.Click += new System.EventHandler(this.Btn_PrintAll_Click);
            // 
            // Btn_PrintOne
            // 
            this.Btn_PrintOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_PrintOne.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_PrintOne.Image = global::SECRON.Properties.Resources.EmployeesNegro25x25;
            this.Btn_PrintOne.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_PrintOne.Location = new System.Drawing.Point(884, 5);
            this.Btn_PrintOne.Name = "Btn_PrintOne";
            this.Btn_PrintOne.Size = new System.Drawing.Size(269, 37);
            this.Btn_PrintOne.TabIndex = 65;
            this.Btn_PrintOne.Text = "GENERAR EL SEELECCIONADO";
            this.Btn_PrintOne.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_PrintOne.UseVisualStyleBackColor = true;
            this.Btn_PrintOne.Click += new System.EventHandler(this.Btn_PrintOne_Click);
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(10, 541);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(1165, 244);
            this.PanelTabla.TabIndex = 83;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(1165, 244);
            this.Tabla.TabIndex = 1;
            // 
            // PanelContenedor
            // 
            this.PanelContenedor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelContenedor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelContenedor.Controls.Add(this.PreviewContratos);
            this.PanelContenedor.Location = new System.Drawing.Point(10, 65);
            this.PanelContenedor.Name = "PanelContenedor";
            this.PanelContenedor.Size = new System.Drawing.Size(1165, 450);
            this.PanelContenedor.TabIndex = 85;
            // 
            // PreviewContratos
            // 
            this.PreviewContratos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreviewContratos.Location = new System.Drawing.Point(0, 0);
            this.PreviewContratos.Name = "PreviewContratos";
            this.PreviewContratos.Size = new System.Drawing.Size(1165, 450);
            this.PreviewContratos.TabIndex = 0;
            // 
            // Frm_RRHH_Teachers_TempGenerarContratos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.PanelContenedor);
            this.Controls.Add(this.PanelResumen);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_RRHH_Teachers_TempGenerarContratos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - GENERAR CONTRATOS DOCENTES";
            this.Load += new System.EventHandler(this.Frm_RRHH_Teachers_TempGenerarContratos_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.PanelResumen.ResumeLayout(false);
            this.PanelResumen.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.PanelContenedor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel PanelResumen;
        private System.Windows.Forms.Label Lbl_Paginas;
        private System.Windows.Forms.Button Btn_PrintAll;
        private System.Windows.Forms.Button Btn_PrintOne;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel PanelContenedor;
        private System.Windows.Forms.ComboBox ComboBox_Sede;
        private System.Windows.Forms.Label Lbl_Sede;
        private System.Windows.Forms.Button Btn_Siguiente;
        private System.Windows.Forms.Button Btn_Anterior;
        private System.Windows.Forms.PrintPreviewControl PreviewContratos;
    }
}