namespace SECRON.Views
{
    partial class Frm_AcademicProcesses_Reviser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_AcademicProcesses_Reviser));
            this.panel1 = new System.Windows.Forms.Panel();
            this.Lbl_Paginas = new System.Windows.Forms.Label();
            this.Btn_Remove = new System.Windows.Forms.Button();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Btn_LimpiarSeleccion = new System.Windows.Forms.Button();
            this.ComboBox_Grupo = new System.Windows.Forms.ComboBox();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Btn_AsignarSedes = new System.Windows.Forms.Button();
            this.Btn_Permisos = new System.Windows.Forms.Button();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.PanelCargosAbonos = new System.Windows.Forms.Panel();
            this.Filtro3 = new System.Windows.Forms.ComboBox();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Lbl_Search = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.Panel_Superior.SuspendLayout();
            this.panel2.SuspendLayout();
            this.PanelCargosAbonos.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.Lbl_Paginas);
            this.panel1.Controls.Add(this.Btn_Remove);
            this.panel1.Controls.Add(this.Btn_Add);
            this.panel1.Location = new System.Drawing.Point(9, 806);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1165, 49);
            this.panel1.TabIndex = 86;
            // 
            // Lbl_Paginas
            // 
            this.Lbl_Paginas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Paginas.AutoSize = true;
            this.Lbl_Paginas.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Paginas.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Paginas.Location = new System.Drawing.Point(10, 12);
            this.Lbl_Paginas.Name = "Lbl_Paginas";
            this.Lbl_Paginas.Size = new System.Drawing.Size(197, 20);
            this.Lbl_Paginas.TabIndex = 77;
            this.Lbl_Paginas.Text = "MOSTRANDO 1-10 DE 100";
            // 
            // Btn_Remove
            // 
            this.Btn_Remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Remove.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Remove.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Remove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Remove.Location = new System.Drawing.Point(906, 3);
            this.Btn_Remove.Name = "Btn_Remove";
            this.Btn_Remove.Size = new System.Drawing.Size(124, 37);
            this.Btn_Remove.TabIndex = 66;
            this.Btn_Remove.Text = "REMOVER";
            this.Btn_Remove.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Remove.UseVisualStyleBackColor = true;
            // 
            // Btn_Add
            // 
            this.Btn_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Add.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Add.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Add.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Add.Location = new System.Drawing.Point(1036, 3);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(117, 37);
            this.Btn_Add.TabIndex = 65;
            this.Btn_Add.Text = "AGREGAR";
            this.Btn_Add.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Add.UseVisualStyleBackColor = true;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(9, 267);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(1165, 522);
            this.PanelTabla.TabIndex = 85;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(1165, 522);
            this.Tabla.TabIndex = 1;
            // 
            // Btn_LimpiarSeleccion
            // 
            this.Btn_LimpiarSeleccion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_LimpiarSeleccion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_LimpiarSeleccion.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_LimpiarSeleccion.Location = new System.Drawing.Point(1115, 19);
            this.Btn_LimpiarSeleccion.Name = "Btn_LimpiarSeleccion";
            this.Btn_LimpiarSeleccion.Size = new System.Drawing.Size(35, 45);
            this.Btn_LimpiarSeleccion.TabIndex = 90;
            this.Btn_LimpiarSeleccion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_LimpiarSeleccion.UseVisualStyleBackColor = true;
            // 
            // ComboBox_Grupo
            // 
            this.ComboBox_Grupo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ComboBox_Grupo.FormattingEnabled = true;
            this.ComboBox_Grupo.Location = new System.Drawing.Point(17, 38);
            this.ComboBox_Grupo.Name = "ComboBox_Grupo";
            this.ComboBox_Grupo.Size = new System.Drawing.Size(446, 26);
            this.ComboBox_Grupo.TabIndex = 74;
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.Location = new System.Drawing.Point(1074, 19);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(35, 45);
            this.Btn_Clear.TabIndex = 63;
            this.Btn_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.Location = new System.Drawing.Point(1033, 19);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(35, 45);
            this.Btn_Search.TabIndex = 62;
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.Panel_Superior.Controls.Add(this.Btn_AsignarSedes);
            this.Panel_Superior.Controls.Add(this.Btn_Permisos);
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1184, 55);
            this.Panel_Superior.TabIndex = 83;
            // 
            // Btn_AsignarSedes
            // 
            this.Btn_AsignarSedes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_AsignarSedes.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_AsignarSedes.Image = global::SECRON.Properties.Resources.LocationNegro20x20;
            this.Btn_AsignarSedes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_AsignarSedes.Location = new System.Drawing.Point(894, 13);
            this.Btn_AsignarSedes.Name = "Btn_AsignarSedes";
            this.Btn_AsignarSedes.Size = new System.Drawing.Size(157, 30);
            this.Btn_AsignarSedes.TabIndex = 57;
            this.Btn_AsignarSedes.Text = "ASIGNAR SEDES";
            this.Btn_AsignarSedes.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.Btn_AsignarSedes.UseVisualStyleBackColor = true;
            // 
            // Btn_Permisos
            // 
            this.Btn_Permisos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Permisos.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Btn_Permisos.Image = global::SECRON.Properties.Resources.AjustesNegro25x25;
            this.Btn_Permisos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Permisos.Location = new System.Drawing.Point(1057, 13);
            this.Btn_Permisos.Name = "Btn_Permisos";
            this.Btn_Permisos.Size = new System.Drawing.Size(115, 30);
            this.Btn_Permisos.TabIndex = 56;
            this.Btn_Permisos.Text = "PERMISOS";
            this.Btn_Permisos.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.Btn_Permisos.UseVisualStyleBackColor = true;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(498, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "CONFIGURACIÓN PERFIL DE REVISORES DE HORARIOS";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel2.Controls.Add(this.Btn_LimpiarSeleccion);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.PanelCargosAbonos);
            this.panel2.Controls.Add(this.ComboBox_Grupo);
            this.panel2.Controls.Add(this.Btn_Clear);
            this.panel2.Controls.Add(this.Btn_Search);
            this.panel2.Location = new System.Drawing.Point(13, 67);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1159, 194);
            this.panel2.TabIndex = 87;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(13, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 20);
            this.label1.TabIndex = 51;
            this.label1.Text = "GRUPO DE SELECCION DE REVISORES";
            // 
            // PanelCargosAbonos
            // 
            this.PanelCargosAbonos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelCargosAbonos.Controls.Add(this.Filtro3);
            this.PanelCargosAbonos.Controls.Add(this.Txt_ValorBuscado);
            this.PanelCargosAbonos.Controls.Add(this.Lbl_Search);
            this.PanelCargosAbonos.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PanelCargosAbonos.Location = new System.Drawing.Point(0, 70);
            this.PanelCargosAbonos.Name = "PanelCargosAbonos";
            this.PanelCargosAbonos.Size = new System.Drawing.Size(1159, 124);
            this.PanelCargosAbonos.TabIndex = 72;
            // 
            // Filtro3
            // 
            this.Filtro3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Filtro3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Filtro3.FormattingEnabled = true;
            this.Filtro3.Location = new System.Drawing.Point(17, 86);
            this.Filtro3.Name = "Filtro3";
            this.Filtro3.Size = new System.Drawing.Size(446, 26);
            this.Filtro3.TabIndex = 77;
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(17, 43);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(1132, 27);
            this.Txt_ValorBuscado.TabIndex = 60;
            // 
            // Lbl_Search
            // 
            this.Lbl_Search.AutoSize = true;
            this.Lbl_Search.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Search.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Search.Location = new System.Drawing.Point(13, 12);
            this.Lbl_Search.Name = "Lbl_Search";
            this.Lbl_Search.Size = new System.Drawing.Size(101, 20);
            this.Lbl_Search.TabIndex = 61;
            this.Lbl_Search.Text = "BÚSQUEDA *";
            // 
            // Frm_AcademicProcesses_Reviser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_AcademicProcesses_Reviser";
            this.Text = "SECRON - CONFIGURACIÓN PERFIL DE REVISORES DE HORARIOS";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.PanelCargosAbonos.ResumeLayout(false);
            this.PanelCargosAbonos.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Lbl_Paginas;
        private System.Windows.Forms.Button Btn_Remove;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Button Btn_LimpiarSeleccion;
        private System.Windows.Forms.ComboBox ComboBox_Grupo;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Button Btn_Permisos;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Button Btn_AsignarSedes;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel PanelCargosAbonos;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Label Lbl_Search;
        private System.Windows.Forms.ComboBox Filtro3;
    }
}