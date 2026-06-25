namespace SECRON.Views
{
    partial class Frm_KARDEX_ImportItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_KARDEX_ImportItems));
            this.panel1 = new System.Windows.Forms.Panel();
            this.Lbl_Resultado = new System.Windows.Forms.Label();
            this.Btn_SeleccionarArchivo = new System.Windows.Forms.Button();
            this.Btn_DescargarPlantilla = new System.Windows.Forms.Button();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Btn_Importar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Txt_RutaArchivo = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.Panel_Superior.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.Txt_RutaArchivo);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.Btn_Importar);
            this.panel1.Controls.Add(this.Lbl_Resultado);
            this.panel1.Controls.Add(this.Btn_SeleccionarArchivo);
            this.panel1.Controls.Add(this.Btn_DescargarPlantilla);
            this.panel1.Location = new System.Drawing.Point(7, 559);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1165, 90);
            this.panel1.TabIndex = 86;
            // 
            // Lbl_Resultado
            // 
            this.Lbl_Resultado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Resultado.AutoSize = true;
            this.Lbl_Resultado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Resultado.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Resultado.Location = new System.Drawing.Point(5, 9);
            this.Lbl_Resultado.Name = "Lbl_Resultado";
            this.Lbl_Resultado.Size = new System.Drawing.Size(281, 20);
            this.Lbl_Resultado.TabIndex = 77;
            this.Lbl_Resultado.Text = "MOSTRANDO 1-10 DE 100 REGISTROS";
            // 
            // Btn_SeleccionarArchivo
            // 
            this.Btn_SeleccionarArchivo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_SeleccionarArchivo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SeleccionarArchivo.Image = global::SECRON.Properties.Resources.UploadFileBlack25x25;
            this.Btn_SeleccionarArchivo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_SeleccionarArchivo.Location = new System.Drawing.Point(630, 44);
            this.Btn_SeleccionarArchivo.Name = "Btn_SeleccionarArchivo";
            this.Btn_SeleccionarArchivo.Size = new System.Drawing.Size(182, 37);
            this.Btn_SeleccionarArchivo.TabIndex = 66;
            this.Btn_SeleccionarArchivo.Text = "CARGAR ARCHIVO";
            this.Btn_SeleccionarArchivo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SeleccionarArchivo.UseVisualStyleBackColor = true;
            this.Btn_SeleccionarArchivo.Click += new System.EventHandler(this.Btn_SeleccionarArchivo_Click);
            // 
            // Btn_DescargarPlantilla
            // 
            this.Btn_DescargarPlantilla.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_DescargarPlantilla.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_DescargarPlantilla.Image = global::SECRON.Properties.Resources.ExportarExcelNegro25x25;
            this.Btn_DescargarPlantilla.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_DescargarPlantilla.Location = new System.Drawing.Point(818, 44);
            this.Btn_DescargarPlantilla.Name = "Btn_DescargarPlantilla";
            this.Btn_DescargarPlantilla.Size = new System.Drawing.Size(212, 37);
            this.Btn_DescargarPlantilla.TabIndex = 65;
            this.Btn_DescargarPlantilla.Text = "DESCAGAR PLANTILLA";
            this.Btn_DescargarPlantilla.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_DescargarPlantilla.UseVisualStyleBackColor = true;
            this.Btn_DescargarPlantilla.Click += new System.EventHandler(this.Btn_DescargarPlantilla_Click);
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(7, 61);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(1165, 492);
            this.PanelTabla.TabIndex = 85;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(1165, 492);
            this.Tabla.TabIndex = 1;
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1184, 55);
            this.Panel_Superior.TabIndex = 83;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(551, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "IMPORTAR LISTADO DE CATÁLOGO GENERAL DE ARTÍCULOS";
            // 
            // Btn_Importar
            // 
            this.Btn_Importar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Importar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Importar.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Importar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Importar.Location = new System.Drawing.Point(1034, 44);
            this.Btn_Importar.Name = "Btn_Importar";
            this.Btn_Importar.Size = new System.Drawing.Size(128, 37);
            this.Btn_Importar.TabIndex = 78;
            this.Btn_Importar.Text = "IMPORTAR";
            this.Btn_Importar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Importar.UseVisualStyleBackColor = true;
            this.Btn_Importar.Click += new System.EventHandler(this.Btn_Importar_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(5, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 20);
            this.label1.TabIndex = 79;
            this.label1.Text = "RUTA DEL ARCHIVO";
            // 
            // Txt_RutaArchivo
            // 
            this.Txt_RutaArchivo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_RutaArchivo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_RutaArchivo.Location = new System.Drawing.Point(162, 50);
            this.Txt_RutaArchivo.MaxLength = 15;
            this.Txt_RutaArchivo.Name = "Txt_RutaArchivo";
            this.Txt_RutaArchivo.Size = new System.Drawing.Size(462, 27);
            this.Txt_RutaArchivo.TabIndex = 80;
            // 
            // Frm_KARDEX_ImportItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_KARDEX_ImportItems";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - IMPORTAR LISTADO, CATALOGO GENERAL DE ARTÍCULOS";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Lbl_Resultado;
        private System.Windows.Forms.Button Btn_SeleccionarArchivo;
        private System.Windows.Forms.Button Btn_DescargarPlantilla;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Button Btn_Importar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Txt_RutaArchivo;
    }
}