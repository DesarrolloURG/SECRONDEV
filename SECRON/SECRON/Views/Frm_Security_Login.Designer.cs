namespace SECRON
{
    partial class Frm_Security_Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Security_Login));
            this.Panel_Contenedor = new System.Windows.Forms.Panel();
            this.Lbl_Versionamiento = new System.Windows.Forms.Label();
            this.Btn_Visible = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Btn_Next = new System.Windows.Forms.Button();
            this.Btn_Unlock = new System.Windows.Forms.Button();
            this.Btn_ForgetPassword = new System.Windows.Forms.Button();
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
            this.Panel_Contenedor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(22)))), ((int)(((byte)(27)))));
            this.Panel_Contenedor.Controls.Add(this.Lbl_Versionamiento);
            this.Panel_Contenedor.Controls.Add(this.Btn_Visible);
            this.Panel_Contenedor.Controls.Add(this.pictureBox1);
            this.Panel_Contenedor.Controls.Add(this.Btn_Next);
            this.Panel_Contenedor.Controls.Add(this.Btn_Unlock);
            this.Panel_Contenedor.Controls.Add(this.Btn_ForgetPassword);
            this.Panel_Contenedor.Controls.Add(this.TxtPassword);
            this.Panel_Contenedor.Controls.Add(this.Lbl2);
            this.Panel_Contenedor.Controls.Add(this.TxtUser);
            this.Panel_Contenedor.Controls.Add(this.Lbl1);
            this.Panel_Contenedor.Location = new System.Drawing.Point(12, 12);
            this.Panel_Contenedor.Name = "Panel_Contenedor";
            this.Panel_Contenedor.Size = new System.Drawing.Size(435, 560);
            this.Panel_Contenedor.TabIndex = 1;
            // 
            // Lbl_Versionamiento
            // 
            this.Lbl_Versionamiento.AutoSize = true;
            this.Lbl_Versionamiento.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Versionamiento.ForeColor = System.Drawing.Color.White;
            this.Lbl_Versionamiento.Location = new System.Drawing.Point(318, 533);
            this.Lbl_Versionamiento.Name = "Lbl_Versionamiento";
            this.Lbl_Versionamiento.Size = new System.Drawing.Size(107, 16);
            this.Lbl_Versionamiento.TabIndex = 5;
            this.Lbl_Versionamiento.Text = "VERSIÓN 2.0.0.0";
            // 
            // Btn_Visible
            // 
            this.Btn_Visible.BackColor = System.Drawing.Color.Transparent;
            this.Btn_Visible.FlatAppearance.BorderSize = 0;
            this.Btn_Visible.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Visible.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Visible.ForeColor = System.Drawing.Color.White;
            this.Btn_Visible.Image = global::SECRON.Properties.Resources.Visible_25x25;
            this.Btn_Visible.Location = new System.Drawing.Point(383, 277);
            this.Btn_Visible.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_Visible.Name = "Btn_Visible";
            this.Btn_Visible.Size = new System.Drawing.Size(43, 51);
            this.Btn_Visible.TabIndex = 2;
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
            // Btn_Next
            // 
            this.Btn_Next.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.Btn_Next.Location = new System.Drawing.Point(235, 375);
            this.Btn_Next.Name = "Btn_Next";
            this.Btn_Next.Size = new System.Drawing.Size(124, 38);
            this.Btn_Next.TabIndex = 3;
            this.Btn_Next.Text = "INICIAR SESIÓN";
            this.Btn_Next.UseVisualStyleBackColor = true;
            this.Btn_Next.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // Btn_Unlock
            // 
            this.Btn_Unlock.BackColor = System.Drawing.Color.Transparent;
            this.Btn_Unlock.Cursor = System.Windows.Forms.Cursors.Default;
            this.Btn_Unlock.FlatAppearance.BorderSize = 0;
            this.Btn_Unlock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Unlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Unlock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(184)))), ((int)(((byte)(255)))));
            this.Btn_Unlock.Location = new System.Drawing.Point(47, 449);
            this.Btn_Unlock.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Btn_Unlock.Name = "Btn_Unlock";
            this.Btn_Unlock.Size = new System.Drawing.Size(208, 46);
            this.Btn_Unlock.TabIndex = 4;
            this.Btn_Unlock.Text = "Desbloqueo de usuario";
            this.Btn_Unlock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Unlock.UseVisualStyleBackColor = false;
            this.Btn_Unlock.Click += new System.EventHandler(this.Btn_Unlock_Click);
            this.Btn_Unlock.MouseEnter += new System.EventHandler(this.Btn_Unlock_MouseEnter);
            this.Btn_Unlock.MouseLeave += new System.EventHandler(this.Btn_Unlock_MouseLeave);
            // 
            // Btn_ForgetPassword
            // 
            this.Btn_ForgetPassword.BackColor = System.Drawing.Color.Transparent;
            this.Btn_ForgetPassword.Cursor = System.Windows.Forms.Cursors.Default;
            this.Btn_ForgetPassword.FlatAppearance.BorderSize = 0;
            this.Btn_ForgetPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_ForgetPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_ForgetPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(184)))), ((int)(((byte)(255)))));
            this.Btn_ForgetPassword.Location = new System.Drawing.Point(47, 505);
            this.Btn_ForgetPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Btn_ForgetPassword.Name = "Btn_ForgetPassword";
            this.Btn_ForgetPassword.Size = new System.Drawing.Size(208, 46);
            this.Btn_ForgetPassword.TabIndex = 5;
            this.Btn_ForgetPassword.Text = "He olvidado mi contraseña";
            this.Btn_ForgetPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_ForgetPassword.UseVisualStyleBackColor = false;
            this.Btn_ForgetPassword.Click += new System.EventHandler(this.Btn_ForgetPassword_Click);
            this.Btn_ForgetPassword.MouseEnter += new System.EventHandler(this.Btn_ForgetPassword_MouseEnter);
            this.Btn_ForgetPassword.MouseLeave += new System.EventHandler(this.Btn_ForgetPassword_MouseLeave);
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
            this.Lbl2.Size = new System.Drawing.Size(142, 24);
            this.Lbl2.TabIndex = 1;
            this.Lbl2.Text = "CONTRASEÑA";
            // 
            // TxtUser
            // 
            this.TxtUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtUser.Location = new System.Drawing.Point(47, 184);
            this.TxtUser.MaxLength = 15;
            this.TxtUser.Name = "TxtUser";
            this.TxtUser.Size = new System.Drawing.Size(328, 29);
            this.TxtUser.TabIndex = 0;
            this.TxtUser.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtUser_KeyPress);
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
            // Frm_Security_Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(22)))), ((int)(((byte)(27)))));
            this.ClientSize = new System.Drawing.Size(459, 581);
            this.Controls.Add(this.Panel_Contenedor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(475, 620);
            this.MinimumSize = new System.Drawing.Size(475, 620);
            this.Name = "Frm_Security_Login";
            this.Opacity = 0.85D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON";
            this.Load += new System.EventHandler(this.Frm_Seguridad_Login_Load);
            this.Panel_Contenedor.ResumeLayout(false);
            this.Panel_Contenedor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel Panel_Contenedor;
        private System.Windows.Forms.Label Lbl1;
        private System.Windows.Forms.TextBox TxtUser;
        private System.Windows.Forms.TextBox TxtPassword;
        private System.Windows.Forms.Label Lbl2;
        private System.Windows.Forms.Button Btn_ForgetPassword;
        private System.Windows.Forms.Button Btn_Unlock;
        private System.Windows.Forms.Button Btn_Next;
        private System.Windows.Forms.Label Lbl_Versionamiento;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button Btn_Visible;
    }
}