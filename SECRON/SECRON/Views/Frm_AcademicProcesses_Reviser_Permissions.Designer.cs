namespace SECRON.Views
{
    partial class Frm_AcademicProcesses_Reviser_Permissions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_AcademicProcesses_Reviser_Permissions));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_Derecho = new System.Windows.Forms.Panel();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.PanelTabla7 = new System.Windows.Forms.Panel();
            this.TablaPermisosRol = new System.Windows.Forms.DataGridView();
            this.Panel_Busqueda1 = new System.Windows.Forms.Panel();
            this.Lbl_Permisos = new System.Windows.Forms.Label();
            this.Btn_ClearPermisoRol = new System.Windows.Forms.Button();
            this.Btn_SearchPermisoRol = new System.Windows.Forms.Button();
            this.FiltroRol = new System.Windows.Forms.ComboBox();
            this.Txt_ValorBuscadoPermisosAsignados = new System.Windows.Forms.TextBox();
            this.Panel_Tabla8 = new System.Windows.Forms.Panel();
            this.TablaPermisosGenerales = new System.Windows.Forms.DataGridView();
            this.panel6 = new System.Windows.Forms.Panel();
            this.Lbl_Colaboradores = new System.Windows.Forms.Label();
            this.Btn_ClearPermisoGeneral = new System.Windows.Forms.Button();
            this.Btn_SearchPermisosGeneral = new System.Windows.Forms.Button();
            this.FiltroGeneral = new System.Windows.Forms.ComboBox();
            this.Txt_ValorBuscadoPermisosGenerales = new System.Windows.Forms.TextBox();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.Btn_Remove = new System.Windows.Forms.Button();
            this.Panel_Superior.SuspendLayout();
            this.Panel_Derecho.SuspendLayout();
            this.PanelTabla7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TablaPermisosRol)).BeginInit();
            this.Panel_Busqueda1.SuspendLayout();
            this.Panel_Tabla8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TablaPermisosGenerales)).BeginInit();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1184, 55);
            this.Panel_Superior.TabIndex = 89;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(359, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "PERMISOS A REVISORES DE HORARIOS";
            // 
            // Panel_Derecho
            // 
            this.Panel_Derecho.Controls.Add(this.vScrollBar);
            this.Panel_Derecho.Controls.Add(this.PanelTabla7);
            this.Panel_Derecho.Controls.Add(this.Panel_Busqueda1);
            this.Panel_Derecho.Controls.Add(this.Panel_Tabla8);
            this.Panel_Derecho.Controls.Add(this.panel6);
            this.Panel_Derecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Derecho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Derecho.ForeColor = System.Drawing.Color.Black;
            this.Panel_Derecho.Location = new System.Drawing.Point(0, 55);
            this.Panel_Derecho.Name = "Panel_Derecho";
            this.Panel_Derecho.Size = new System.Drawing.Size(1184, 806);
            this.Panel_Derecho.TabIndex = 90;
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(1174, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(10, 806);
            this.vScrollBar.TabIndex = 77;
            // 
            // PanelTabla7
            // 
            this.PanelTabla7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla7.Controls.Add(this.TablaPermisosRol);
            this.PanelTabla7.Location = new System.Drawing.Point(22, 136);
            this.PanelTabla7.Name = "PanelTabla7";
            this.PanelTabla7.Size = new System.Drawing.Size(1140, 203);
            this.PanelTabla7.TabIndex = 75;
            // 
            // TablaPermisosRol
            // 
            this.TablaPermisosRol.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TablaPermisosRol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TablaPermisosRol.Location = new System.Drawing.Point(0, 0);
            this.TablaPermisosRol.Name = "TablaPermisosRol";
            this.TablaPermisosRol.Size = new System.Drawing.Size(1140, 203);
            this.TablaPermisosRol.TabIndex = 1;
            // 
            // Panel_Busqueda1
            // 
            this.Panel_Busqueda1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Busqueda1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Busqueda1.Controls.Add(this.Btn_Remove);
            this.Panel_Busqueda1.Controls.Add(this.Lbl_Permisos);
            this.Panel_Busqueda1.Controls.Add(this.Btn_ClearPermisoRol);
            this.Panel_Busqueda1.Controls.Add(this.Btn_SearchPermisoRol);
            this.Panel_Busqueda1.Controls.Add(this.FiltroRol);
            this.Panel_Busqueda1.Controls.Add(this.Txt_ValorBuscadoPermisosAsignados);
            this.Panel_Busqueda1.Location = new System.Drawing.Point(22, 22);
            this.Panel_Busqueda1.Name = "Panel_Busqueda1";
            this.Panel_Busqueda1.Size = new System.Drawing.Size(1140, 108);
            this.Panel_Busqueda1.TabIndex = 73;
            // 
            // Lbl_Permisos
            // 
            this.Lbl_Permisos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Permisos.AutoSize = true;
            this.Lbl_Permisos.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Permisos.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Permisos.Location = new System.Drawing.Point(12, 11);
            this.Lbl_Permisos.Name = "Lbl_Permisos";
            this.Lbl_Permisos.Size = new System.Drawing.Size(539, 20);
            this.Lbl_Permisos.TabIndex = 72;
            this.Lbl_Permisos.Text = "BUSQUEDA DE PERMISOS ACTIVOS PARA EL ROL DE REVISOR DE HORARIO";
            // 
            // Btn_ClearPermisoRol
            // 
            this.Btn_ClearPermisoRol.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_ClearPermisoRol.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ClearPermisoRol.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_ClearPermisoRol.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_ClearPermisoRol.Location = new System.Drawing.Point(1059, 26);
            this.Btn_ClearPermisoRol.Name = "Btn_ClearPermisoRol";
            this.Btn_ClearPermisoRol.Size = new System.Drawing.Size(35, 35);
            this.Btn_ClearPermisoRol.TabIndex = 71;
            this.Btn_ClearPermisoRol.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_ClearPermisoRol.UseVisualStyleBackColor = true;
            // 
            // Btn_SearchPermisoRol
            // 
            this.Btn_SearchPermisoRol.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_SearchPermisoRol.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SearchPermisoRol.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_SearchPermisoRol.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_SearchPermisoRol.Location = new System.Drawing.Point(953, 26);
            this.Btn_SearchPermisoRol.Name = "Btn_SearchPermisoRol";
            this.Btn_SearchPermisoRol.Size = new System.Drawing.Size(101, 35);
            this.Btn_SearchPermisoRol.TabIndex = 54;
            this.Btn_SearchPermisoRol.Text = "BUSCAR";
            this.Btn_SearchPermisoRol.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SearchPermisoRol.UseVisualStyleBackColor = true;
            // 
            // FiltroRol
            // 
            this.FiltroRol.FormattingEnabled = true;
            this.FiltroRol.Location = new System.Drawing.Point(16, 67);
            this.FiltroRol.Name = "FiltroRol";
            this.FiltroRol.Size = new System.Drawing.Size(535, 28);
            this.FiltroRol.TabIndex = 68;
            // 
            // Txt_ValorBuscadoPermisosAsignados
            // 
            this.Txt_ValorBuscadoPermisosAsignados.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscadoPermisosAsignados.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscadoPermisosAsignados.Location = new System.Drawing.Point(16, 34);
            this.Txt_ValorBuscadoPermisosAsignados.MaxLength = 15;
            this.Txt_ValorBuscadoPermisosAsignados.Name = "Txt_ValorBuscadoPermisosAsignados";
            this.Txt_ValorBuscadoPermisosAsignados.Size = new System.Drawing.Size(931, 27);
            this.Txt_ValorBuscadoPermisosAsignados.TabIndex = 59;
            // 
            // Panel_Tabla8
            // 
            this.Panel_Tabla8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Tabla8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Tabla8.Controls.Add(this.TablaPermisosGenerales);
            this.Panel_Tabla8.Location = new System.Drawing.Point(22, 475);
            this.Panel_Tabla8.Name = "Panel_Tabla8";
            this.Panel_Tabla8.Size = new System.Drawing.Size(1140, 311);
            this.Panel_Tabla8.TabIndex = 72;
            // 
            // TablaPermisosGenerales
            // 
            this.TablaPermisosGenerales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TablaPermisosGenerales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TablaPermisosGenerales.Location = new System.Drawing.Point(0, 0);
            this.TablaPermisosGenerales.Name = "TablaPermisosGenerales";
            this.TablaPermisosGenerales.Size = new System.Drawing.Size(1140, 311);
            this.TablaPermisosGenerales.TabIndex = 1;
            // 
            // panel6
            // 
            this.panel6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel6.Controls.Add(this.Btn_Add);
            this.panel6.Controls.Add(this.Lbl_Colaboradores);
            this.panel6.Controls.Add(this.Btn_ClearPermisoGeneral);
            this.panel6.Controls.Add(this.Btn_SearchPermisosGeneral);
            this.panel6.Controls.Add(this.FiltroGeneral);
            this.panel6.Controls.Add(this.Txt_ValorBuscadoPermisosGenerales);
            this.panel6.Location = new System.Drawing.Point(22, 361);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1140, 108);
            this.panel6.TabIndex = 53;
            // 
            // Lbl_Colaboradores
            // 
            this.Lbl_Colaboradores.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Colaboradores.AutoSize = true;
            this.Lbl_Colaboradores.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Colaboradores.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Colaboradores.Location = new System.Drawing.Point(12, 11);
            this.Lbl_Colaboradores.Name = "Lbl_Colaboradores";
            this.Lbl_Colaboradores.Size = new System.Drawing.Size(365, 20);
            this.Lbl_Colaboradores.TabIndex = 72;
            this.Lbl_Colaboradores.Text = "PERMISOS DE MÓDULO DE CONTROL ACADÉMICO";
            // 
            // Btn_ClearPermisoGeneral
            // 
            this.Btn_ClearPermisoGeneral.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_ClearPermisoGeneral.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ClearPermisoGeneral.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_ClearPermisoGeneral.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_ClearPermisoGeneral.Location = new System.Drawing.Point(1059, 27);
            this.Btn_ClearPermisoGeneral.Name = "Btn_ClearPermisoGeneral";
            this.Btn_ClearPermisoGeneral.Size = new System.Drawing.Size(35, 35);
            this.Btn_ClearPermisoGeneral.TabIndex = 71;
            this.Btn_ClearPermisoGeneral.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_ClearPermisoGeneral.UseVisualStyleBackColor = true;
            // 
            // Btn_SearchPermisosGeneral
            // 
            this.Btn_SearchPermisosGeneral.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_SearchPermisosGeneral.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SearchPermisosGeneral.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_SearchPermisosGeneral.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_SearchPermisosGeneral.Location = new System.Drawing.Point(953, 27);
            this.Btn_SearchPermisosGeneral.Name = "Btn_SearchPermisosGeneral";
            this.Btn_SearchPermisosGeneral.Size = new System.Drawing.Size(101, 35);
            this.Btn_SearchPermisosGeneral.TabIndex = 54;
            this.Btn_SearchPermisosGeneral.Text = "BUSCAR";
            this.Btn_SearchPermisosGeneral.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SearchPermisosGeneral.UseVisualStyleBackColor = true;
            // 
            // FiltroGeneral
            // 
            this.FiltroGeneral.FormattingEnabled = true;
            this.FiltroGeneral.Location = new System.Drawing.Point(16, 67);
            this.FiltroGeneral.Name = "FiltroGeneral";
            this.FiltroGeneral.Size = new System.Drawing.Size(535, 28);
            this.FiltroGeneral.TabIndex = 68;
            // 
            // Txt_ValorBuscadoPermisosGenerales
            // 
            this.Txt_ValorBuscadoPermisosGenerales.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscadoPermisosGenerales.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscadoPermisosGenerales.Location = new System.Drawing.Point(16, 34);
            this.Txt_ValorBuscadoPermisosGenerales.MaxLength = 15;
            this.Txt_ValorBuscadoPermisosGenerales.Name = "Txt_ValorBuscadoPermisosGenerales";
            this.Txt_ValorBuscadoPermisosGenerales.Size = new System.Drawing.Size(931, 27);
            this.Txt_ValorBuscadoPermisosGenerales.TabIndex = 59;
            // 
            // Btn_Add
            // 
            this.Btn_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Add.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Add.Image = global::SECRON.Properties.Resources.AddNegro25x25;
            this.Btn_Add.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Add.Location = new System.Drawing.Point(1100, 29);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(35, 35);
            this.Btn_Add.TabIndex = 80;
            this.Btn_Add.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Add.UseVisualStyleBackColor = true;
            // 
            // Btn_Remove
            // 
            this.Btn_Remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Remove.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Remove.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Remove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Remove.Location = new System.Drawing.Point(1097, 26);
            this.Btn_Remove.Name = "Btn_Remove";
            this.Btn_Remove.Size = new System.Drawing.Size(35, 35);
            this.Btn_Remove.TabIndex = 81;
            this.Btn_Remove.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Remove.UseVisualStyleBackColor = true;
            // 
            // Frm_AcademicProcesses_Reviser_Permissions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.Panel_Derecho);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_AcademicProcesses_Reviser_Permissions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON -PERMISOS  A REVISORES DE HORARIOS";
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Derecho.ResumeLayout(false);
            this.PanelTabla7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TablaPermisosRol)).EndInit();
            this.Panel_Busqueda1.ResumeLayout(false);
            this.Panel_Busqueda1.PerformLayout();
            this.Panel_Tabla8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TablaPermisosGenerales)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_Derecho;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.Panel PanelTabla7;
        private System.Windows.Forms.DataGridView TablaPermisosRol;
        private System.Windows.Forms.Panel Panel_Busqueda1;
        private System.Windows.Forms.Label Lbl_Permisos;
        private System.Windows.Forms.Button Btn_ClearPermisoRol;
        private System.Windows.Forms.Button Btn_SearchPermisoRol;
        private System.Windows.Forms.ComboBox FiltroRol;
        private System.Windows.Forms.TextBox Txt_ValorBuscadoPermisosAsignados;
        private System.Windows.Forms.Panel Panel_Tabla8;
        private System.Windows.Forms.DataGridView TablaPermisosGenerales;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label Lbl_Colaboradores;
        private System.Windows.Forms.Button Btn_ClearPermisoGeneral;
        private System.Windows.Forms.Button Btn_SearchPermisosGeneral;
        private System.Windows.Forms.ComboBox FiltroGeneral;
        private System.Windows.Forms.TextBox Txt_ValorBuscadoPermisosGenerales;
        private System.Windows.Forms.Button Btn_Remove;
        private System.Windows.Forms.Button Btn_Add;
    }
}