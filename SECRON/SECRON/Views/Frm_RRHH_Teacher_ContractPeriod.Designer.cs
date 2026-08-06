namespace SECRON.Views
{
    partial class Frm_RRHH_Teacher_ContractPeriod
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_RRHH_Teacher_ContractPeriod));
            this.PanelInferior = new System.Windows.Forms.Panel();
            this.Lbl_Dato = new System.Windows.Forms.Label();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Panel_DetalleTabla = new System.Windows.Forms.Panel();
            this.Btn_ActivarPeriodo = new System.Windows.Forms.Button();
            this.DTP_Fin = new System.Windows.Forms.DateTimePicker();
            this.LblFin = new System.Windows.Forms.Label();
            this.DTP_Inicio = new System.Windows.Forms.DateTimePicker();
            this.Lbl_Emision = new System.Windows.Forms.Label();
            this.Btn_EliminarPeriodo = new System.Windows.Forms.Button();
            this.Lbl_BuscarPor = new System.Windows.Forms.Label();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.PanelInferior.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.Panel_DetalleTabla.SuspendLayout();
            this.Panel_Superior.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelInferior
            // 
            this.PanelInferior.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelInferior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelInferior.Controls.Add(this.Lbl_Dato);
            this.PanelInferior.Location = new System.Drawing.Point(9, 557);
            this.PanelInferior.Name = "PanelInferior";
            this.PanelInferior.Size = new System.Drawing.Size(665, 53);
            this.PanelInferior.TabIndex = 94;
            // 
            // Lbl_Dato
            // 
            this.Lbl_Dato.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Dato.AutoSize = true;
            this.Lbl_Dato.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Dato.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Dato.Location = new System.Drawing.Point(10, 15);
            this.Lbl_Dato.Name = "Lbl_Dato";
            this.Lbl_Dato.Size = new System.Drawing.Size(162, 20);
            this.Lbl_Dato.TabIndex = 78;
            this.Lbl_Dato.Text = "ESTADO DE PERIODO:";
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(9, 188);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(665, 363);
            this.PanelTabla.TabIndex = 93;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(665, 363);
            this.Tabla.TabIndex = 1;
            this.Tabla.SelectionChanged += new System.EventHandler(this.Tabla_SelectionChanged);
            // 
            // Panel_DetalleTabla
            // 
            this.Panel_DetalleTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_DetalleTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_DetalleTabla.Controls.Add(this.Btn_ActivarPeriodo);
            this.Panel_DetalleTabla.Controls.Add(this.DTP_Fin);
            this.Panel_DetalleTabla.Controls.Add(this.LblFin);
            this.Panel_DetalleTabla.Controls.Add(this.DTP_Inicio);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_Emision);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_EliminarPeriodo);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_BuscarPor);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Add);
            this.Panel_DetalleTabla.Location = new System.Drawing.Point(9, 71);
            this.Panel_DetalleTabla.Name = "Panel_DetalleTabla";
            this.Panel_DetalleTabla.Size = new System.Drawing.Size(663, 111);
            this.Panel_DetalleTabla.TabIndex = 92;
            // 
            // Btn_ActivarPeriodo
            // 
            this.Btn_ActivarPeriodo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_ActivarPeriodo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ActivarPeriodo.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_ActivarPeriodo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_ActivarPeriodo.Location = new System.Drawing.Point(446, 58);
            this.Btn_ActivarPeriodo.Name = "Btn_ActivarPeriodo";
            this.Btn_ActivarPeriodo.Size = new System.Drawing.Size(207, 45);
            this.Btn_ActivarPeriodo.TabIndex = 78;
            this.Btn_ActivarPeriodo.Text = "ACTIVAR PERIODO";
            this.Btn_ActivarPeriodo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_ActivarPeriodo.UseVisualStyleBackColor = true;
            // 
            // DTP_Fin
            // 
            this.DTP_Fin.Location = new System.Drawing.Point(63, 76);
            this.DTP_Fin.Name = "DTP_Fin";
            this.DTP_Fin.Size = new System.Drawing.Size(225, 20);
            this.DTP_Fin.TabIndex = 76;
            // 
            // LblFin
            // 
            this.LblFin.AutoSize = true;
            this.LblFin.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.LblFin.ForeColor = System.Drawing.Color.Black;
            this.LblFin.Location = new System.Drawing.Point(10, 76);
            this.LblFin.Name = "LblFin";
            this.LblFin.Size = new System.Drawing.Size(39, 20);
            this.LblFin.TabIndex = 77;
            this.LblFin.Text = "AL *";
            // 
            // DTP_Inicio
            // 
            this.DTP_Inicio.Location = new System.Drawing.Point(63, 39);
            this.DTP_Inicio.Name = "DTP_Inicio";
            this.DTP_Inicio.Size = new System.Drawing.Size(225, 20);
            this.DTP_Inicio.TabIndex = 74;
            // 
            // Lbl_Emision
            // 
            this.Lbl_Emision.AutoSize = true;
            this.Lbl_Emision.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Emision.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Emision.Location = new System.Drawing.Point(10, 39);
            this.Lbl_Emision.Name = "Lbl_Emision";
            this.Lbl_Emision.Size = new System.Drawing.Size(47, 20);
            this.Lbl_Emision.TabIndex = 75;
            this.Lbl_Emision.Text = "DEL *";
            // 
            // Btn_EliminarPeriodo
            // 
            this.Btn_EliminarPeriodo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_EliminarPeriodo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_EliminarPeriodo.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_EliminarPeriodo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_EliminarPeriodo.Location = new System.Drawing.Point(446, 7);
            this.Btn_EliminarPeriodo.Name = "Btn_EliminarPeriodo";
            this.Btn_EliminarPeriodo.Size = new System.Drawing.Size(207, 45);
            this.Btn_EliminarPeriodo.TabIndex = 73;
            this.Btn_EliminarPeriodo.Text = "INACTIVAR PERIODO";
            this.Btn_EliminarPeriodo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_EliminarPeriodo.UseVisualStyleBackColor = true;
            // 
            // Lbl_BuscarPor
            // 
            this.Lbl_BuscarPor.AutoSize = true;
            this.Lbl_BuscarPor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_BuscarPor.ForeColor = System.Drawing.Color.Black;
            this.Lbl_BuscarPor.Location = new System.Drawing.Point(10, 12);
            this.Lbl_BuscarPor.Name = "Lbl_BuscarPor";
            this.Lbl_BuscarPor.Size = new System.Drawing.Size(134, 20);
            this.Lbl_BuscarPor.TabIndex = 64;
            this.Lbl_BuscarPor.Text = "NUEVO PERIODO:";
            // 
            // Btn_Add
            // 
            this.Btn_Add.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Add.Image = global::SECRON.Properties.Resources.AddNegro25x25;
            this.Btn_Add.Location = new System.Drawing.Point(294, 39);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(35, 45);
            this.Btn_Add.TabIndex = 62;
            this.Btn_Add.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Add.UseVisualStyleBackColor = true;
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(684, 55);
            this.Panel_Superior.TabIndex = 91;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(484, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "PERIODO HABILITADO PARA CONTRATOS DOCENTES";
            // 
            // Frm_RRHH_Teacher_ContractPeriod
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 611);
            this.Controls.Add(this.PanelInferior);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_DetalleTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_RRHH_Teacher_ContractPeriod";
            this.Text = "SECRON - PERIODO HABILITADO PARA CONTRATOS DOCENTES";
            this.Load += new System.EventHandler(this.Frm_RRHH_Teacher_ContractPeriod_Load);
            this.PanelInferior.ResumeLayout(false);
            this.PanelInferior.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.Panel_DetalleTabla.ResumeLayout(false);
            this.Panel_DetalleTabla.PerformLayout();
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelInferior;
        private System.Windows.Forms.Label Lbl_Dato;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel Panel_DetalleTabla;
        private System.Windows.Forms.Button Btn_EliminarPeriodo;
        private System.Windows.Forms.Label Lbl_BuscarPor;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.DateTimePicker DTP_Fin;
        private System.Windows.Forms.Label LblFin;
        private System.Windows.Forms.DateTimePicker DTP_Inicio;
        private System.Windows.Forms.Label Lbl_Emision;
        private System.Windows.Forms.Button Btn_ActivarPeriodo;
    }
}