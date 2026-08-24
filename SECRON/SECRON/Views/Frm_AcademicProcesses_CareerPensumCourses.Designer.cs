namespace SECRON.Views
{
    partial class Frm_AcademicProcesses_CareerPensumCourses
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_AcademicProcesses_CareerPensumCourses));
            this.panel1 = new System.Windows.Forms.Panel();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Btn_Importar = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.Panel_Superior.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.Btn_Importar);
            this.panel1.Location = new System.Drawing.Point(9, 749);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1553, 61);
            this.panel1.TabIndex = 92;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Location = new System.Drawing.Point(9, 86);
            this.PanelTabla.Margin = new System.Windows.Forms.Padding(4);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(1553, 655);
            this.PanelTabla.TabIndex = 91;
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Margin = new System.Windows.Forms.Padding(4);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1579, 68);
            this.Panel_Superior.TabIndex = 90;
            // 
            // Lbl_Formulario
            // 
            this.Lbl_Formulario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Formulario.AutoSize = true;
            this.Lbl_Formulario.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.Lbl_Formulario.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Formulario.Location = new System.Drawing.Point(11, 16);
            this.Lbl_Formulario.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Formulario.Name = "Lbl_Formulario";
            this.Lbl_Formulario.Size = new System.Drawing.Size(379, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "CONFIGURACIÓN PENSUM DE ESTUDIOS";
            // 
            // Btn_Importar
            // 
            this.Btn_Importar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Importar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Importar.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Importar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Importar.Location = new System.Drawing.Point(1353, 4);
            this.Btn_Importar.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Importar.Name = "Btn_Importar";
            this.Btn_Importar.Size = new System.Drawing.Size(197, 46);
            this.Btn_Importar.TabIndex = 78;
            this.Btn_Importar.Text = "GUARDAR CAMBIOS";
            this.Btn_Importar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Importar.UseVisualStyleBackColor = true;
            this.Btn_Importar.Click += new System.EventHandler(this.Btn_Importar_Click);
            // 
            // Frm_AcademicProcesses_CareerPensumCourses
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1579, 814);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Frm_AcademicProcesses_CareerPensumCourses";
            this.Text = "SECRON - CONFIGURACIÓN DE PENSUM";
            this.Load += new System.EventHandler(this.Frm_AcademicProcesses_CareerPensumCourses_Load);
            this.panel1.ResumeLayout(false);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Button Btn_Importar;
    }
}