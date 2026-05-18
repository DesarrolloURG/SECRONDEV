namespace SECRON.Views
{
    partial class Frm_FA_AssetAttributeValues
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_FA_AssetAttributeValues));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Lbl_Info = new System.Windows.Forms.Label();
            this.Panel_Campos = new System.Windows.Forms.Panel();
            this.Panel_Botones = new System.Windows.Forms.Panel();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Btn_Cancel = new System.Windows.Forms.Button();

            this.Panel_Superior.SuspendLayout();
            this.Panel_Botones.SuspendLayout();
            this.SuspendLayout();

            // ── Panel_Superior ──────────────────────────────────────────────
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(238, 143, 109);
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(604, 55);
            this.Panel_Superior.TabIndex = 0;

            this.Lbl_Formulario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Formulario.AutoSize = true;
            this.Lbl_Formulario.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.Lbl_Formulario.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Formulario.Location = new System.Drawing.Point(8, 13);
            this.Lbl_Formulario.Name = "Lbl_Formulario";
            this.Lbl_Formulario.TabIndex = 0;
            this.Lbl_Formulario.Text = "CARACTERÍSTICAS DEL ACTIVO";

            // ── Lbl_Info ────────────────────────────────────────────────────
            this.Lbl_Info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.Lbl_Info.AutoSize = false;
            this.Lbl_Info.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.Lbl_Info.ForeColor = System.Drawing.Color.FromArgb(51, 140, 255);
            this.Lbl_Info.Location = new System.Drawing.Point(10, 65);
            this.Lbl_Info.Name = "Lbl_Info";
            this.Lbl_Info.Size = new System.Drawing.Size(584, 22);
            this.Lbl_Info.TabIndex = 1;
            this.Lbl_Info.Text = "Cargando características...";

            // ── Panel_Campos (scrollable, se llena dinámicamente) ───────────
            this.Panel_Campos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Campos.AutoScroll = true;
            this.Panel_Campos.BackColor = System.Drawing.Color.White;
            this.Panel_Campos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_Campos.Location = new System.Drawing.Point(10, 95);
            this.Panel_Campos.Name = "Panel_Campos";
            this.Panel_Campos.Size = new System.Drawing.Size(584, 460);
            this.Panel_Campos.TabIndex = 2;

            // ── Panel_Botones ───────────────────────────────────────────────
            this.Panel_Botones.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Botones.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.Panel_Botones.Controls.Add(this.Btn_Cancel);
            this.Panel_Botones.Controls.Add(this.Btn_Save);
            this.Panel_Botones.Location = new System.Drawing.Point(10, 565);
            this.Panel_Botones.Name = "Panel_Botones";
            this.Panel_Botones.Size = new System.Drawing.Size(584, 55);
            this.Panel_Botones.TabIndex = 3;

            this.Btn_Save.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Save.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Save.Location = new System.Drawing.Point(344, 8);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(115, 40);
            this.Btn_Save.TabIndex = 0;
            this.Btn_Save.Text = "GUARDAR";
            this.Btn_Save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);

            this.Btn_Cancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Cancel.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Cancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Cancel.Location = new System.Drawing.Point(467, 8);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(115, 40);
            this.Btn_Cancel.TabIndex = 1;
            this.Btn_Cancel.Text = "CERRAR";
            this.Btn_Cancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Cancel.UseVisualStyleBackColor = true;
            this.Btn_Cancel.Click += new System.EventHandler(this.Btn_Cancel_Click);

            // ── Form ────────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 630);
            this.Controls.Add(this.Panel_Botones);
            this.Controls.Add(this.Panel_Campos);
            this.Controls.Add(this.Lbl_Info);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_FA_AssetAttributeValues";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SECRON - CARACTERÍSTICAS DEL ACTIVO";
            this.Load += new System.EventHandler(this.Frm_FA_AssetAttributeValues_Load);

            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Botones.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Label Lbl_Info;
        private System.Windows.Forms.Panel Panel_Campos;
        private System.Windows.Forms.Panel Panel_Botones;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Button Btn_Cancel;
    }
}