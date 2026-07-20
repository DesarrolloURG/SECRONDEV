namespace SECRON.Views
{
    partial class Frm_Security_TwoFactorSetup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Security_TwoFactorSetup));
            this.Panel_Contenedor = new System.Windows.Forms.Panel();
            this.Btn_Confirmar = new System.Windows.Forms.Button();
            this.Txt_Codigo = new System.Windows.Forms.TextBox();
            this.Lbl_Codigo = new System.Windows.Forms.Label();
            this.PicQR = new System.Windows.Forms.PictureBox();
            this.Lbl_Instrucciones = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Panel_Contenedor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicQR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Panel_Contenedor
            // 
            this.Panel_Contenedor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Contenedor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(22)))), ((int)(((byte)(27)))));
            this.Panel_Contenedor.Controls.Add(this.Btn_Confirmar);
            this.Panel_Contenedor.Controls.Add(this.Txt_Codigo);
            this.Panel_Contenedor.Controls.Add(this.Lbl_Codigo);
            this.Panel_Contenedor.Controls.Add(this.PicQR);
            this.Panel_Contenedor.Controls.Add(this.Lbl_Instrucciones);
            this.Panel_Contenedor.Controls.Add(this.pictureBox1);
            this.Panel_Contenedor.Location = new System.Drawing.Point(16, 12);
            this.Panel_Contenedor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Panel_Contenedor.Name = "Panel_Contenedor";
            this.Panel_Contenedor.Size = new System.Drawing.Size(580, 689);
            this.Panel_Contenedor.TabIndex = 2;
            // 
            // Btn_Confirmar
            // 
            this.Btn_Confirmar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.Btn_Confirmar.Location = new System.Drawing.Point(187, 609);
            this.Btn_Confirmar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Btn_Confirmar.Name = "Btn_Confirmar";
            this.Btn_Confirmar.Size = new System.Drawing.Size(200, 47);
            this.Btn_Confirmar.TabIndex = 1;
            this.Btn_Confirmar.Text = "CONFIRMAR";
            this.Btn_Confirmar.UseVisualStyleBackColor = true;
            this.Btn_Confirmar.Click += new System.EventHandler(this.Btn_Confirmar_Click);
            // 
            // Txt_Codigo
            // 
            this.Txt_Codigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Txt_Codigo.Location = new System.Drawing.Point(63, 542);
            this.Txt_Codigo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Txt_Codigo.MaxLength = 6;
            this.Txt_Codigo.Name = "Txt_Codigo";
            this.Txt_Codigo.Size = new System.Drawing.Size(436, 32);
            this.Txt_Codigo.TabIndex = 0;
            this.Txt_Codigo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Lbl_Codigo
            // 
            this.Lbl_Codigo.AutoSize = true;
            this.Lbl_Codigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Codigo.ForeColor = System.Drawing.Color.White;
            this.Lbl_Codigo.Location = new System.Drawing.Point(57, 498);
            this.Lbl_Codigo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Codigo.Name = "Lbl_Codigo";
            this.Lbl_Codigo.Size = new System.Drawing.Size(287, 20);
            this.Lbl_Codigo.TabIndex = 3;
            this.Lbl_Codigo.Text = "INGRESE EL CÓDIGO DE 6 DÍGITOS";
            // 
            // PicQR
            // 
            this.PicQR.BackColor = System.Drawing.Color.White;
            this.PicQR.Location = new System.Drawing.Point(156, 228);
            this.PicQR.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PicQR.Name = "PicQR";
            this.PicQR.Size = new System.Drawing.Size(267, 246);
            this.PicQR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicQR.TabIndex = 2;
            this.PicQR.TabStop = false;
            // 
            // Lbl_Instrucciones
            // 
            this.Lbl_Instrucciones.AutoSize = true;
            this.Lbl_Instrucciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Instrucciones.ForeColor = System.Drawing.Color.White;
            this.Lbl_Instrucciones.Location = new System.Drawing.Point(57, 160);
            this.Lbl_Instrucciones.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Instrucciones.MaximumSize = new System.Drawing.Size(464, 0);
            this.Lbl_Instrucciones.Name = "Lbl_Instrucciones";
            this.Lbl_Instrucciones.Size = new System.Drawing.Size(451, 36);
            this.Lbl_Instrucciones.TabIndex = 1;
            this.Lbl_Instrucciones.Text = "Escanee este código QR con Microsoft Authenticator para vincular su cuenta.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::SECRON.Properties.Resources.URG_LOGO_BLANCO;
            this.pictureBox1.Location = new System.Drawing.Point(103, 25);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(373, 111);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // Frm_Security_TwoFactorSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(22)))), ((int)(((byte)(27)))));
            this.ClientSize = new System.Drawing.Size(612, 715);
            this.Controls.Add(this.Panel_Contenedor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_Security_TwoFactorSetup";
            this.Opacity = 0.85D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - VINCULAR AUTENTICACIÓN EN DOS PASOS";
            this.Load += new System.EventHandler(this.Frm_Security_TwoFactorSetup_Load);
            this.Panel_Contenedor.ResumeLayout(false);
            this.Panel_Contenedor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicQR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Contenedor;
        private System.Windows.Forms.Label Lbl_Instrucciones;
        private System.Windows.Forms.PictureBox PicQR;
        private System.Windows.Forms.Label Lbl_Codigo;
        private System.Windows.Forms.TextBox Txt_Codigo;
        private System.Windows.Forms.Button Btn_Confirmar;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}