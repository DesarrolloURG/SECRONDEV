namespace SECRON
{
    partial class Frm_System_Splash
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_System_Splash));
            this.Timer_Splash = new System.Windows.Forms.Timer(this.components);
            this.Pic1 = new System.Windows.Forms.PictureBox();
            this.Pic2 = new System.Windows.Forms.PictureBox();
            this.Pic3 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Pic1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pic2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pic3)).BeginInit();
            this.SuspendLayout();
            // 
            // Timer_Splash
            // 
            this.Timer_Splash.Enabled = true;
            this.Timer_Splash.Tick += new System.EventHandler(this.timerSplash_Tick);
            // 
            // Pic1
            // 
            this.Pic1.BackColor = System.Drawing.Color.White;
            this.Pic1.Image = global::SECRON.Properties.Resources.SECRON_LogotipoAzul;
            this.Pic1.Location = new System.Drawing.Point(337, 193);
            this.Pic1.Margin = new System.Windows.Forms.Padding(0);
            this.Pic1.Name = "Pic1";
            this.Pic1.Size = new System.Drawing.Size(179, 176);
            this.Pic1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Pic1.TabIndex = 3;
            this.Pic1.TabStop = false;
            // 
            // Pic2
            // 
            this.Pic2.BackColor = System.Drawing.Color.White;
            this.Pic2.Image = global::SECRON.Properties.Resources.Splash_CirculoCarga;
            this.Pic2.Location = new System.Drawing.Point(275, 127);
            this.Pic2.Name = "Pic2";
            this.Pic2.Size = new System.Drawing.Size(300, 301);
            this.Pic2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Pic2.TabIndex = 4;
            this.Pic2.TabStop = false;
            // 
            // Pic3
            // 
            this.Pic3.Image = global::SECRON.Properties.Resources.Splash_CirculoBlanco;
            this.Pic3.Location = new System.Drawing.Point(180, 42);
            this.Pic3.Name = "Pic3";
            this.Pic3.Size = new System.Drawing.Size(490, 486);
            this.Pic3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Pic3.TabIndex = 5;
            this.Pic3.TabStop = false;
            // 
            // Frm_System_Splash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(850, 571);
            this.Controls.Add(this.Pic1);
            this.Controls.Add(this.Pic2);
            this.Controls.Add(this.Pic3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_System_Splash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON";
            this.TransparencyKey = System.Drawing.Color.Black;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_System_Splash_FormClosing);
            this.Load += new System.EventHandler(this.Frm_System_Splash_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Pic1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pic2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pic3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Pic1;
        private System.Windows.Forms.PictureBox Pic2;
        private System.Windows.Forms.PictureBox Pic3;
        private System.Windows.Forms.Timer Timer_Splash;
    }
}