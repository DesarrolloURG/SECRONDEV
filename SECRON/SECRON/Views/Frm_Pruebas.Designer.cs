namespace SECRON.Views
{
    partial class Frm_Pruebas
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
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.Btn_Remove = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.SuspendLayout();
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Location = new System.Drawing.Point(12, 105);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(560, 328);
            this.Tabla.TabIndex = 0;
            // 
            // Btn_Add
            // 
            this.Btn_Add.Location = new System.Drawing.Point(146, 57);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(128, 23);
            this.Btn_Add.TabIndex = 1;
            this.Btn_Add.Text = "CARGAR ARCHIVO";
            this.Btn_Add.UseVisualStyleBackColor = true;
            this.Btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // Btn_Remove
            // 
            this.Btn_Remove.Location = new System.Drawing.Point(12, 57);
            this.Btn_Remove.Name = "Btn_Remove";
            this.Btn_Remove.Size = new System.Drawing.Size(128, 23);
            this.Btn_Remove.TabIndex = 2;
            this.Btn_Remove.Text = "QUITAR ARCHIVO";
            this.Btn_Remove.UseVisualStyleBackColor = true;
            this.Btn_Remove.Click += new System.EventHandler(this.Btn_Remove_Click);
            // 
            // Frm_Pruebas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.Btn_Remove);
            this.Controls.Add(this.Btn_Add);
            this.Controls.Add(this.Tabla);
            this.Name = "Frm_Pruebas";
            this.Text = "Frm_Pruebas";
            this.Load += new System.EventHandler(this.Frm_Pruebas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.Button Btn_Remove;
    }
}