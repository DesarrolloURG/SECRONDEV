namespace SECRON.Views
{
    partial class Frm_Security_TemporalPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Security_TemporalPassword));
            this.Panel_Contenedor = new System.Windows.Forms.Panel();
            this.Btn_Visible = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Btn_OK = new System.Windows.Forms.Button();
            this.TxtPassword = new System.Windows.Forms.TextBox();
            this.Lbl2 = new System.Windows.Forms.Label();
            this.TxtUser = new System.Windows.Forms.TextBox();
            this.Lbl1 = new System.Windows.Forms.Label();
            this.Panel_Contenedor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Panel_Contenedor
            // 
            this.Panel_Contenedor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Contenedor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(22)))), ((int)(((byte)(27)))));
            this.Panel_Contenedor.Controls.Add(this.Btn_Visible);
            this.Panel_Contenedor.Controls.Add(this.pictureBox1);
            this.Panel_Contenedor.Controls.Add(this.Btn_OK);
            this.Panel_Contenedor.Controls.Add(this.TxtPassword);
            this.Panel_Contenedor.Controls.Add(this.Lbl2);
            this.Panel_Contenedor.Controls.Add(this.TxtUser);
            this.Panel_Contenedor.Controls.Add(this.Lbl1);
            this.Panel_Contenedor.Location = new System.Drawing.Point(12, 10);
            this.Panel_Contenedor.Name = "Panel_Contenedor";
            this.Panel_Contenedor.Size = new System.Drawing.Size(435, 442);
            this.Panel_Contenedor.TabIndex = 2;
            // 
            // Btn_Visible
            // 
            this.Btn_Visible.BackColor = System.Drawing.Color.Transparent;
            this.Btn_Visible.FlatAppearance.BorderSize = 0;
            this.Btn_Visible.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Visible.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Visible.ForeColor = System.Drawing.Color.White;
            this.Btn_Visible.Image = global::SECRON.Properties.Resources.Visible_25x25;
            this.Btn_Visible.Location = new System.Drawing.Point(383, 275);
            this.Btn_Visible.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_Visible.Name = "Btn_Visible";
            this.Btn_Visible.Size = new System.Drawing.Size(43, 51);
            this.Btn_Visible.TabIndex = 3;
            this.Btn_Visible.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Visible.UseVisualStyleBackColor = false;
            this.Btn_Visible.Click += new System.EventHandler(this.Btn_Visible_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::SECRON.Properties.Resources.URG_LOGO_BLANCO;
            this.pictureBox1.Location = new System.Drawing.Point(77, 26);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(280, 90);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // Btn_OK
            // 
            this.Btn_OK.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.Btn_OK.Location = new System.Drawing.Point(235, 375);
            this.Btn_OK.Name = "Btn_OK";
            this.Btn_OK.Size = new System.Drawing.Size(124, 38);
            this.Btn_OK.TabIndex = 2;
            this.Btn_OK.Text = "ACEPTAR";
            this.Btn_OK.UseVisualStyleBackColor = true;
            this.Btn_OK.Click += new System.EventHandler(this.Btn_OK_Click);
            // 
            // TxtPassword
            // 
            this.TxtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtPassword.Location = new System.Drawing.Point(47, 285);
            this.TxtPassword.MaxLength = 15;
            this.TxtPassword.Name = "TxtPassword";
            this.TxtPassword.Size = new System.Drawing.Size(328, 29);
            this.TxtPassword.TabIndex = 1;
            this.TxtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtPassword_KeyPress);
            // 
            // Lbl2
            // 
            this.Lbl2.AutoSize = true;
            this.Lbl2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl2.ForeColor = System.Drawing.Color.White;
            this.Lbl2.Location = new System.Drawing.Point(43, 246);
            this.Lbl2.Name = "Lbl2";
            this.Lbl2.Size = new System.Drawing.Size(251, 24);
            this.Lbl2.TabIndex = 1;
            this.Lbl2.Text = "CONTRASEÑA TEMPORAL";
            // 
            // TxtUser
            // 
            this.TxtUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtUser.Location = new System.Drawing.Point(47, 184);
            this.TxtUser.MaxLength = 15;
            this.TxtUser.Name = "TxtUser";
            this.TxtUser.Size = new System.Drawing.Size(328, 29);
            this.TxtUser.TabIndex = 0;
            // 
            // Lbl1
            // 
            this.Lbl1.AutoSize = true;
            this.Lbl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl1.ForeColor = System.Drawing.Color.White;
            this.Lbl1.Location = new System.Drawing.Point(43, 145);
            this.Lbl1.Name = "Lbl1";
            this.Lbl1.Size = new System.Drawing.Size(93, 24);
            this.Lbl1.TabIndex = 0;
            this.Lbl1.Text = "USUARIO";
            // 
            // Frm_Security_TemporalPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(22)))), ((int)(((byte)(27)))));
            this.ClientSize = new System.Drawing.Size(459, 463);
            this.Controls.Add(this.Panel_Contenedor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_Security_TemporalPassword";
            this.Opacity = 0.85D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - CONTRASEÑA TEMPORAL";
            this.Load += new System.EventHandler(this.Frm_Security_TemporalPassword_Load);
            this.Panel_Contenedor.ResumeLayout(false);
            this.Panel_Contenedor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Contenedor;
        private System.Windows.Forms.Button Btn_Visible;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button Btn_OK;
        private System.Windows.Forms.TextBox TxtPassword;
        private System.Windows.Forms.Label Lbl2;
        private System.Windows.Forms.TextBox TxtUser;
        private System.Windows.Forms.Label Lbl1;
    }
}