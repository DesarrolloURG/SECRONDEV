namespace SECRON.Views
{
    partial class Frm_Checks_ChangeCorrelative
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Checks_ChangeCorrelative));
            this.panel1 = new System.Windows.Forms.Panel();
            this.Btn_No = new System.Windows.Forms.Button();
            this.Btn_Yes = new System.Windows.Forms.Button();
            this.Lbl_Beneficiario = new System.Windows.Forms.Label();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Panel_DetalleTabla = new System.Windows.Forms.Panel();
            this.Lbl_BuscarPor = new System.Windows.Forms.Label();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Txt_ChequeModificar = new System.Windows.Forms.TextBox();
            this.Txt_NuevoNumero = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ComboBox_BuscarPor = new System.Windows.Forms.ComboBox();
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
            this.panel1.Controls.Add(this.Txt_NuevoNumero);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.Txt_ChequeModificar);
            this.panel1.Controls.Add(this.Btn_No);
            this.panel1.Controls.Add(this.Btn_Yes);
            this.panel1.Controls.Add(this.Lbl_Beneficiario);
            this.panel1.Location = new System.Drawing.Point(9, 495);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(665, 114);
            this.panel1.TabIndex = 86;
            // 
            // Btn_No
            // 
            this.Btn_No.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_No.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_No.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_No.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_No.Location = new System.Drawing.Point(406, 67);
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
            this.Btn_Yes.Location = new System.Drawing.Point(536, 67);
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
            this.Lbl_Beneficiario.Size = new System.Drawing.Size(170, 20);
            this.Lbl_Beneficiario.TabIndex = 61;
            this.Lbl_Beneficiario.Text = "CHEQUE A MODIFICAR";
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(9, 187);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(665, 302);
            this.PanelTabla.TabIndex = 85;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(665, 302);
            this.Tabla.TabIndex = 1;
            // 
            // Panel_DetalleTabla
            // 
            this.Panel_DetalleTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_DetalleTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_DetalleTabla.Controls.Add(this.ComboBox_BuscarPor);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_BuscarPor);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Clear);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Search);
            this.Panel_DetalleTabla.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_DetalleTabla.Location = new System.Drawing.Point(9, 70);
            this.Panel_DetalleTabla.Name = "Panel_DetalleTabla";
            this.Panel_DetalleTabla.Size = new System.Drawing.Size(663, 111);
            this.Panel_DetalleTabla.TabIndex = 84;
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
            this.Btn_Clear.Location = new System.Drawing.Point(618, 39);
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
            this.Btn_Search.Location = new System.Drawing.Point(579, 39);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(35, 45);
            this.Btn_Search.TabIndex = 62;
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(684, 55);
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(344, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "CORREGIR CORRELATIVO DE CHEQUE";
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(14, 71);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(557, 27);
            this.Txt_ValorBuscado.TabIndex = 60;
            // 
            // Txt_ChequeModificar
            // 
            this.Txt_ChequeModificar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ChequeModificar.Location = new System.Drawing.Point(14, 34);
            this.Txt_ChequeModificar.MaxLength = 15;
            this.Txt_ChequeModificar.Name = "Txt_ChequeModificar";
            this.Txt_ChequeModificar.Size = new System.Drawing.Size(315, 27);
            this.Txt_ChequeModificar.TabIndex = 77;
            // 
            // Txt_NuevoNumero
            // 
            this.Txt_NuevoNumero.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_NuevoNumero.Location = new System.Drawing.Point(338, 34);
            this.Txt_NuevoNumero.MaxLength = 15;
            this.Txt_NuevoNumero.Name = "Txt_NuevoNumero";
            this.Txt_NuevoNumero.Size = new System.Drawing.Size(315, 27);
            this.Txt_NuevoNumero.TabIndex = 79;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(334, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(292, 20);
            this.label1.TabIndex = 78;
            this.label1.Text = "NUEVO CORRELATIVO PARA EL CHEQUE";
            // 
            // ComboBox_BuscarPor
            // 
            this.ComboBox_BuscarPor.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ComboBox_BuscarPor.FormattingEnabled = true;
            this.ComboBox_BuscarPor.Location = new System.Drawing.Point(14, 39);
            this.ComboBox_BuscarPor.Name = "ComboBox_BuscarPor";
            this.ComboBox_BuscarPor.Size = new System.Drawing.Size(557, 26);
            this.ComboBox_BuscarPor.TabIndex = 72;
            // 
            // Frm_Checks_ChangeCorrelative
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 611);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_DetalleTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_Checks_ChangeCorrelative";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - CORREGIR NÚMERO DE CORRELATIVO DE CHEQUE";
            this.Load += new System.EventHandler(this.Frm_Checks_ChangeCorrelative_Load);
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
        private System.Windows.Forms.Label Lbl_Beneficiario;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel Panel_DetalleTabla;
        private System.Windows.Forms.Label Lbl_BuscarPor;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.TextBox Txt_NuevoNumero;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Txt_ChequeModificar;
        private System.Windows.Forms.ComboBox ComboBox_BuscarPor;
    }
}