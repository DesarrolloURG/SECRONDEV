namespace SECRON.Views
{
    partial class Frm_FixedAsset_TransferStatusTransitions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_FixedAsset_TransferStatusTransitions));
            this.Btn_Delete = new System.Windows.Forms.Button();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Panel_1 = new System.Windows.Forms.Panel();
            this.Lbl_Subtitulo1 = new System.Windows.Forms.Label();
            this.Lbl_BackStatus = new System.Windows.Forms.Label();
            this.Lbl_StatusSelected = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.ComboBox_StatusSelected = new System.Windows.Forms.ComboBox();
            this.ComboBox_FromStatus = new System.Windows.Forms.ComboBox();
            this.ComboBox_ToStatus = new System.Windows.Forms.ComboBox();
            this.Lbl_ToStatus = new System.Windows.Forms.Label();
            this.Panel_1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.Panel_Superior.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn_Delete
            // 
            this.Btn_Delete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Btn_Delete.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Delete.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Delete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Delete.Location = new System.Drawing.Point(501, 4);
            this.Btn_Delete.Name = "Btn_Delete";
            this.Btn_Delete.Size = new System.Drawing.Size(225, 37);
            this.Btn_Delete.TabIndex = 56;
            this.Btn_Delete.Text = "ELIMINAR TRANSICIÓN";
            this.Btn_Delete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Delete.UseVisualStyleBackColor = true;
            this.Btn_Delete.Click += new System.EventHandler(this.Btn_DELETE_Click);
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Clear.Location = new System.Drawing.Point(920, 7);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(33, 37);
            this.Btn_Clear.TabIndex = 57;
            this.Btn_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            this.Btn_Clear.Click += new System.EventHandler(this.Btn_Clear_Click);
            // 
            // Btn_Save
            // 
            this.Btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Btn_Save.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Save.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Save.Location = new System.Drawing.Point(732, 4);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(221, 37);
            this.Btn_Save.TabIndex = 54;
            this.Btn_Save.Text = "GUARDAR TRANSICIÓN";
            this.Btn_Save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Panel_1
            // 
            this.Panel_1.BackColor = System.Drawing.Color.White;
            this.Panel_1.Controls.Add(this.Btn_Clear);
            this.Panel_1.Controls.Add(this.ComboBox_ToStatus);
            this.Panel_1.Controls.Add(this.Lbl_ToStatus);
            this.Panel_1.Controls.Add(this.ComboBox_FromStatus);
            this.Panel_1.Controls.Add(this.ComboBox_StatusSelected);
            this.Panel_1.Controls.Add(this.Lbl_Subtitulo1);
            this.Panel_1.Controls.Add(this.Lbl_BackStatus);
            this.Panel_1.Controls.Add(this.Lbl_StatusSelected);
            this.Panel_1.Location = new System.Drawing.Point(7, 61);
            this.Panel_1.Name = "Panel_1";
            this.Panel_1.Size = new System.Drawing.Size(965, 93);
            this.Panel_1.TabIndex = 104;
            // 
            // Lbl_Subtitulo1
            // 
            this.Lbl_Subtitulo1.AutoSize = true;
            this.Lbl_Subtitulo1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo1.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo1.Image = global::SECRON.Properties.Resources.DescripcionItemBlanco25x25;
            this.Lbl_Subtitulo1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo1.Location = new System.Drawing.Point(10, 7);
            this.Lbl_Subtitulo1.Name = "Lbl_Subtitulo1";
            this.Lbl_Subtitulo1.Size = new System.Drawing.Size(245, 20);
            this.Lbl_Subtitulo1.TabIndex = 61;
            this.Lbl_Subtitulo1.Text = "      DETALLES DE LA TRANSICIÓN";
            this.Lbl_Subtitulo1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Lbl_BackStatus
            // 
            this.Lbl_BackStatus.AutoSize = true;
            this.Lbl_BackStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_BackStatus.ForeColor = System.Drawing.Color.Black;
            this.Lbl_BackStatus.Location = new System.Drawing.Point(328, 32);
            this.Lbl_BackStatus.Name = "Lbl_BackStatus";
            this.Lbl_BackStatus.Size = new System.Drawing.Size(157, 20);
            this.Lbl_BackStatus.TabIndex = 2;
            this.Lbl_BackStatus.Text = "ESTADO ANTERIOR *";
            // 
            // Lbl_StatusSelected
            // 
            this.Lbl_StatusSelected.AutoSize = true;
            this.Lbl_StatusSelected.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_StatusSelected.ForeColor = System.Drawing.Color.Black;
            this.Lbl_StatusSelected.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_StatusSelected.Location = new System.Drawing.Point(10, 32);
            this.Lbl_StatusSelected.Name = "Lbl_StatusSelected";
            this.Lbl_StatusSelected.Size = new System.Drawing.Size(192, 20);
            this.Lbl_StatusSelected.TabIndex = 1;
            this.Lbl_StatusSelected.Text = "ESTADO SELECCIONADO *";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.Btn_Delete);
            this.panel1.Controls.Add(this.Btn_Save);
            this.panel1.Location = new System.Drawing.Point(7, 558);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(965, 47);
            this.panel1.TabIndex = 103;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(7, 160);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(965, 392);
            this.PanelTabla.TabIndex = 102;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.RowHeadersWidth = 51;
            this.Tabla.Size = new System.Drawing.Size(965, 392);
            this.Tabla.TabIndex = 1;
            this.Tabla.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tabla_CellClick);
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(984, 55);
            this.Panel_Superior.TabIndex = 100;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(547, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "TRANSICIONES PARA ESTADOS DE TRASLADOS DE ACTIVOS";
            // 
            // ComboBox_StatusSelected
            // 
            this.ComboBox_StatusSelected.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.ComboBox_StatusSelected.FormattingEnabled = true;
            this.ComboBox_StatusSelected.Location = new System.Drawing.Point(14, 55);
            this.ComboBox_StatusSelected.Name = "ComboBox_StatusSelected";
            this.ComboBox_StatusSelected.Size = new System.Drawing.Size(300, 28);
            this.ComboBox_StatusSelected.TabIndex = 69;
            this.ComboBox_StatusSelected.SelectedIndexChanged += new System.EventHandler(this.ComboBox_StatusSelected_SelectedIndexChanged);
            // 
            // ComboBox_FromStatus
            // 
            this.ComboBox_FromStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.ComboBox_FromStatus.FormattingEnabled = true;
            this.ComboBox_FromStatus.Location = new System.Drawing.Point(332, 55);
            this.ComboBox_FromStatus.Name = "ComboBox_FromStatus";
            this.ComboBox_FromStatus.Size = new System.Drawing.Size(300, 28);
            this.ComboBox_FromStatus.TabIndex = 70;
            // 
            // ComboBox_ToStatus
            // 
            this.ComboBox_ToStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.ComboBox_ToStatus.FormattingEnabled = true;
            this.ComboBox_ToStatus.Location = new System.Drawing.Point(653, 55);
            this.ComboBox_ToStatus.Name = "ComboBox_ToStatus";
            this.ComboBox_ToStatus.Size = new System.Drawing.Size(300, 28);
            this.ComboBox_ToStatus.TabIndex = 72;
            // 
            // Lbl_ToStatus
            // 
            this.Lbl_ToStatus.AutoSize = true;
            this.Lbl_ToStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ToStatus.ForeColor = System.Drawing.Color.Black;
            this.Lbl_ToStatus.Location = new System.Drawing.Point(649, 32);
            this.Lbl_ToStatus.Name = "Lbl_ToStatus";
            this.Lbl_ToStatus.Size = new System.Drawing.Size(157, 20);
            this.Lbl_ToStatus.TabIndex = 71;
            this.Lbl_ToStatus.Text = "ESTADO ANTERIOR *";
            // 
            // Frm_FixedAsset_TransferStatusTransitions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this.Panel_1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_FixedAsset_TransferStatusTransitions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - TRANSICIONES PARA ESTADOS DE TRASLADOS DE ACTIVOS";
            this.Load += new System.EventHandler(this.Frm_FixedAsset_TransferStatusTransitions_Load);
            this.Panel_1.ResumeLayout(false);
            this.Panel_1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Btn_Delete;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Panel Panel_1;
        private System.Windows.Forms.Label Lbl_Subtitulo1;
        private System.Windows.Forms.Label Lbl_BackStatus;
        private System.Windows.Forms.Label Lbl_StatusSelected;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.ComboBox ComboBox_FromStatus;
        private System.Windows.Forms.ComboBox ComboBox_StatusSelected;
        private System.Windows.Forms.ComboBox ComboBox_ToStatus;
        private System.Windows.Forms.Label Lbl_ToStatus;
    }
}