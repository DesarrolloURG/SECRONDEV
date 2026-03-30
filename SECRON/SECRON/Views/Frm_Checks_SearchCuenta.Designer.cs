namespace SECRON.Views
{
    partial class Frm_Checks_SearchCuenta
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Checks_SearchCuenta));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_DetalleTabla = new System.Windows.Forms.Panel();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_SearchCuenta = new System.Windows.Forms.Button();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Lbl_ValorBuscado = new System.Windows.Forms.Label();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Lbl_CuentaSeleccionada = new System.Windows.Forms.Label();
            this.Txt_Codigo = new System.Windows.Forms.TextBox();
            this.Txt_Cuenta = new System.Windows.Forms.TextBox();
            this.Btn_No = new System.Windows.Forms.Button();
            this.Btn_Yes = new System.Windows.Forms.Button();
            this.Panel_Superior.SuspendLayout();
            this.Panel_DetalleTabla.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(684, 55);
            this.Panel_Superior.TabIndex = 4;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(266, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "BUSCAR CUENTA CONTABLE";
            // 
            // Panel_DetalleTabla
            // 
            this.Panel_DetalleTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_DetalleTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Clear);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_SearchCuenta);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_ValorBuscado);
            this.Panel_DetalleTabla.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_DetalleTabla.Location = new System.Drawing.Point(9, 73);
            this.Panel_DetalleTabla.Name = "Panel_DetalleTabla";
            this.Panel_DetalleTabla.Size = new System.Drawing.Size(663, 75);
            this.Panel_DetalleTabla.TabIndex = 54;
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.Location = new System.Drawing.Point(618, 18);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(35, 45);
            this.Btn_Clear.TabIndex = 63;
            this.Btn_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            this.Btn_Clear.Click += new System.EventHandler(this.Btn_Clear_Click);
            // 
            // Btn_SearchCuenta
            // 
            this.Btn_SearchCuenta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_SearchCuenta.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SearchCuenta.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_SearchCuenta.Location = new System.Drawing.Point(577, 18);
            this.Btn_SearchCuenta.Name = "Btn_SearchCuenta";
            this.Btn_SearchCuenta.Size = new System.Drawing.Size(35, 45);
            this.Btn_SearchCuenta.TabIndex = 62;
            this.Btn_SearchCuenta.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SearchCuenta.UseVisualStyleBackColor = true;
            this.Btn_SearchCuenta.Click += new System.EventHandler(this.Btn_SearchCuenta_Click);
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(14, 36);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(557, 27);
            this.Txt_ValorBuscado.TabIndex = 60;
            this.Txt_ValorBuscado.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado_KeyDown);
            // 
            // Lbl_ValorBuscado
            // 
            this.Lbl_ValorBuscado.AutoSize = true;
            this.Lbl_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ValorBuscado.ForeColor = System.Drawing.Color.Black;
            this.Lbl_ValorBuscado.Location = new System.Drawing.Point(10, 13);
            this.Lbl_ValorBuscado.Name = "Lbl_ValorBuscado";
            this.Lbl_ValorBuscado.Size = new System.Drawing.Size(135, 20);
            this.Lbl_ValorBuscado.TabIndex = 61;
            this.Lbl_ValorBuscado.Text = "BUSCAR CUENTA:";
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(9, 164);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(665, 278);
            this.PanelTabla.TabIndex = 73;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(665, 278);
            this.Tabla.TabIndex = 1;
            this.Tabla.SelectionChanged += new System.EventHandler(this.Tabla_SelectionChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.Btn_No);
            this.panel1.Controls.Add(this.Btn_Yes);
            this.panel1.Controls.Add(this.Txt_Cuenta);
            this.panel1.Controls.Add(this.Lbl_CuentaSeleccionada);
            this.panel1.Controls.Add(this.Txt_Codigo);
            this.panel1.Location = new System.Drawing.Point(9, 448);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(665, 151);
            this.panel1.TabIndex = 74;
            // 
            // Lbl_CuentaSeleccionada
            // 
            this.Lbl_CuentaSeleccionada.AutoSize = true;
            this.Lbl_CuentaSeleccionada.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_CuentaSeleccionada.ForeColor = System.Drawing.Color.Black;
            this.Lbl_CuentaSeleccionada.Location = new System.Drawing.Point(10, 13);
            this.Lbl_CuentaSeleccionada.Name = "Lbl_CuentaSeleccionada";
            this.Lbl_CuentaSeleccionada.Size = new System.Drawing.Size(187, 20);
            this.Lbl_CuentaSeleccionada.TabIndex = 61;
            this.Lbl_CuentaSeleccionada.Text = "CUENTA SELECCIONADA:";
            // 
            // Txt_Codigo
            // 
            this.Txt_Codigo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Codigo.Location = new System.Drawing.Point(14, 36);
            this.Txt_Codigo.MaxLength = 15;
            this.Txt_Codigo.Name = "Txt_Codigo";
            this.Txt_Codigo.Size = new System.Drawing.Size(639, 27);
            this.Txt_Codigo.TabIndex = 60;
            // 
            // Txt_Cuenta
            // 
            this.Txt_Cuenta.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Cuenta.Location = new System.Drawing.Point(14, 69);
            this.Txt_Cuenta.MaxLength = 15;
            this.Txt_Cuenta.Name = "Txt_Cuenta";
            this.Txt_Cuenta.Size = new System.Drawing.Size(639, 27);
            this.Txt_Cuenta.TabIndex = 64;
            // 
            // Btn_No
            // 
            this.Btn_No.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_No.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_No.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_No.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_No.Location = new System.Drawing.Point(406, 102);
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
            this.Btn_Yes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Yes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Yes.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Yes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Yes.Location = new System.Drawing.Point(536, 102);
            this.Btn_Yes.Name = "Btn_Yes";
            this.Btn_Yes.Size = new System.Drawing.Size(117, 37);
            this.Btn_Yes.TabIndex = 65;
            this.Btn_Yes.Text = "ACEPTAR";
            this.Btn_Yes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Yes.UseVisualStyleBackColor = true;
            this.Btn_Yes.Click += new System.EventHandler(this.Btn_Yes_Click);
            // 
            // Frm_Checks_SearchCuenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 611);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_DetalleTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_Checks_SearchCuenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - BUSCAR CUENTA CONTABLE";
            this.Load += new System.EventHandler(this.Frm_Checks_SearchCuenta_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_DetalleTabla.ResumeLayout(false);
            this.Panel_DetalleTabla.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_DetalleTabla;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Label Lbl_ValorBuscado;
        private System.Windows.Forms.Button Btn_SearchCuenta;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox Txt_Cuenta;
        private System.Windows.Forms.Label Lbl_CuentaSeleccionada;
        private System.Windows.Forms.TextBox Txt_Codigo;
        private System.Windows.Forms.Button Btn_No;
        private System.Windows.Forms.Button Btn_Yes;
    }
}