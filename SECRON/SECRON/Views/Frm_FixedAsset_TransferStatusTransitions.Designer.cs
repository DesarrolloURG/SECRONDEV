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
            this.Btn_DELETE = new System.Windows.Forms.Button();
            this.Panel_CRUD = new System.Windows.Forms.Panel();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Panel_1 = new System.Windows.Forms.Panel();
            this.ComboBox_ToStatus = new System.Windows.Forms.ComboBox();
            this.Lbl_Subtitulo1 = new System.Windows.Forms.Label();
            this.Txt_FromStatus = new System.Windows.Forms.TextBox();
            this.Lbl_ToStatus = new System.Windows.Forms.Label();
            this.Lbl_FromStatus = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Btn_No = new System.Windows.Forms.Button();
            this.Btn_Yes = new System.Windows.Forms.Button();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Panel_DetalleTabla = new System.Windows.Forms.Panel();
            this.ComboBox_BuscarPor = new System.Windows.Forms.ComboBox();
            this.Lbl_BuscarPor = new System.Windows.Forms.Label();
            this.Btn_ClearSearch = new System.Windows.Forms.Button();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Lbl_ValorBuscado = new System.Windows.Forms.Label();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_CRUD.SuspendLayout();
            this.Panel_1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.Panel_DetalleTabla.SuspendLayout();
            this.Panel_Superior.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn_DELETE
            // 
            this.Btn_DELETE.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Btn_DELETE.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_DELETE.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_DELETE.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_DELETE.Location = new System.Drawing.Point(4, 6);
            this.Btn_DELETE.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Btn_DELETE.Name = "Btn_DELETE";
            this.Btn_DELETE.Size = new System.Drawing.Size(165, 46);
            this.Btn_DELETE.TabIndex = 56;
            this.Btn_DELETE.Text = "ELIMINAR";
            this.Btn_DELETE.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_DELETE.UseVisualStyleBackColor = true;
            this.Btn_DELETE.Click += new System.EventHandler(this.Btn_DELETE_Click);
            // 
            // Panel_CRUD
            // 
            this.Panel_CRUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_CRUD.Controls.Add(this.Btn_DELETE);
            this.Panel_CRUD.Controls.Add(this.Btn_Clear);
            this.Panel_CRUD.Controls.Add(this.Btn_Save);
            this.Panel_CRUD.Location = new System.Drawing.Point(9, 609);
            this.Panel_CRUD.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Panel_CRUD.Name = "Panel_CRUD";
            this.Panel_CRUD.Size = new System.Drawing.Size(392, 58);
            this.Panel_CRUD.TabIndex = 105;
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Clear.Location = new System.Drawing.Point(341, 6);
            this.Btn_Clear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(44, 46);
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
            this.Btn_Save.Location = new System.Drawing.Point(177, 6);
            this.Btn_Save.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(156, 46);
            this.Btn_Save.TabIndex = 54;
            this.Btn_Save.Text = "GUARDAR";
            this.Btn_Save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Panel_1
            // 
            this.Panel_1.BackColor = System.Drawing.Color.White;
            this.Panel_1.Controls.Add(this.ComboBox_ToStatus);
            this.Panel_1.Controls.Add(this.Lbl_Subtitulo1);
            this.Panel_1.Controls.Add(this.Txt_FromStatus);
            this.Panel_1.Controls.Add(this.Lbl_ToStatus);
            this.Panel_1.Controls.Add(this.Lbl_FromStatus);
            this.Panel_1.Location = new System.Drawing.Point(9, 84);
            this.Panel_1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Panel_1.Name = "Panel_1";
            this.Panel_1.Size = new System.Drawing.Size(392, 513);
            this.Panel_1.TabIndex = 104;
            // 
            // ComboBox_ToStatus
            // 
            this.ComboBox_ToStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ComboBox_ToStatus.FormattingEnabled = true;
            this.ComboBox_ToStatus.Location = new System.Drawing.Point(19, 139);
            this.ComboBox_ToStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ComboBox_ToStatus.Name = "ComboBox_ToStatus";
            this.ComboBox_ToStatus.Size = new System.Drawing.Size(355, 30);
            this.ComboBox_ToStatus.TabIndex = 72;
            // 
            // Lbl_Subtitulo1
            // 
            this.Lbl_Subtitulo1.AutoSize = true;
            this.Lbl_Subtitulo1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo1.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo1.Image = global::SECRON.Properties.Resources.DescripcionItemBlanco25x25;
            this.Lbl_Subtitulo1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo1.Location = new System.Drawing.Point(13, 9);
            this.Lbl_Subtitulo1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_Subtitulo1.Name = "Lbl_Subtitulo1";
            this.Lbl_Subtitulo1.Size = new System.Drawing.Size(307, 25);
            this.Lbl_Subtitulo1.TabIndex = 61;
            this.Lbl_Subtitulo1.Text = "      DETALLES DE LA TRANSICIÓN";
            this.Lbl_Subtitulo1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Txt_FromStatus
            // 
            this.Txt_FromStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_FromStatus.Location = new System.Drawing.Point(19, 68);
            this.Txt_FromStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Txt_FromStatus.MaxLength = 15;
            this.Txt_FromStatus.Name = "Txt_FromStatus";
            this.Txt_FromStatus.Size = new System.Drawing.Size(355, 32);
            this.Txt_FromStatus.TabIndex = 1;
            // 
            // Lbl_ToStatus
            // 
            this.Lbl_ToStatus.AutoSize = true;
            this.Lbl_ToStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ToStatus.ForeColor = System.Drawing.Color.Black;
            this.Lbl_ToStatus.Location = new System.Drawing.Point(13, 111);
            this.Lbl_ToStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_ToStatus.Name = "Lbl_ToStatus";
            this.Lbl_ToStatus.Size = new System.Drawing.Size(226, 25);
            this.Lbl_ToStatus.TabIndex = 2;
            this.Lbl_ToStatus.Text = "ESTADO QUE LE SIGUE *";
            // 
            // Lbl_FromStatus
            // 
            this.Lbl_FromStatus.AutoSize = true;
            this.Lbl_FromStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_FromStatus.ForeColor = System.Drawing.Color.Black;
            this.Lbl_FromStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_FromStatus.Location = new System.Drawing.Point(13, 39);
            this.Lbl_FromStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_FromStatus.Name = "Lbl_FromStatus";
            this.Lbl_FromStatus.Size = new System.Drawing.Size(218, 25);
            this.Lbl_FromStatus.TabIndex = 1;
            this.Lbl_FromStatus.Text = "ESTADO EN PROCESO *";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.Btn_No);
            this.panel1.Controls.Add(this.Btn_Yes);
            this.panel1.Location = new System.Drawing.Point(9, 687);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1287, 58);
            this.panel1.TabIndex = 103;
            // 
            // Btn_No
            // 
            this.Btn_No.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_No.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_No.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_No.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_No.Location = new System.Drawing.Point(941, 5);
            this.Btn_No.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Btn_No.Name = "Btn_No";
            this.Btn_No.Size = new System.Drawing.Size(165, 46);
            this.Btn_No.TabIndex = 66;
            this.Btn_No.Text = "CANCELAR";
            this.Btn_No.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_No.UseVisualStyleBackColor = true;
            this.Btn_No.Click += new System.EventHandler(this.Btn_No_Click);
            // 
            // Btn_Yes
            // 
            this.Btn_Yes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Yes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Yes.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Yes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Yes.Location = new System.Drawing.Point(1115, 5);
            this.Btn_Yes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Btn_Yes.Name = "Btn_Yes";
            this.Btn_Yes.Size = new System.Drawing.Size(156, 46);
            this.Btn_Yes.TabIndex = 65;
            this.Btn_Yes.Text = "ACEPTAR";
            this.Btn_Yes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Yes.UseVisualStyleBackColor = true;
            this.Btn_Yes.Click += new System.EventHandler(this.Btn_Yes_Click);
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(409, 254);
            this.PanelTabla.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(887, 414);
            this.PanelTabla.TabIndex = 102;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Tabla.Name = "Tabla";
            this.Tabla.RowHeadersWidth = 51;
            this.Tabla.Size = new System.Drawing.Size(887, 414);
            this.Tabla.TabIndex = 1;
            // 
            // Panel_DetalleTabla
            // 
            this.Panel_DetalleTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_DetalleTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_DetalleTabla.Controls.Add(this.ComboBox_BuscarPor);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_BuscarPor);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_ClearSearch);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Search);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_ValorBuscado);
            this.Panel_DetalleTabla.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_DetalleTabla.Location = new System.Drawing.Point(409, 84);
            this.Panel_DetalleTabla.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Panel_DetalleTabla.Name = "Panel_DetalleTabla";
            this.Panel_DetalleTabla.Size = new System.Drawing.Size(887, 154);
            this.Panel_DetalleTabla.TabIndex = 101;
            // 
            // ComboBox_BuscarPor
            // 
            this.ComboBox_BuscarPor.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ComboBox_BuscarPor.FormattingEnabled = true;
            this.ComboBox_BuscarPor.Location = new System.Drawing.Point(19, 43);
            this.ComboBox_BuscarPor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ComboBox_BuscarPor.Name = "ComboBox_BuscarPor";
            this.ComboBox_BuscarPor.Size = new System.Drawing.Size(741, 30);
            this.ComboBox_BuscarPor.TabIndex = 71;
            // 
            // Lbl_BuscarPor
            // 
            this.Lbl_BuscarPor.AutoSize = true;
            this.Lbl_BuscarPor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_BuscarPor.ForeColor = System.Drawing.Color.Black;
            this.Lbl_BuscarPor.Location = new System.Drawing.Point(13, 15);
            this.Lbl_BuscarPor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_BuscarPor.Name = "Lbl_BuscarPor";
            this.Lbl_BuscarPor.Size = new System.Drawing.Size(134, 25);
            this.Lbl_BuscarPor.TabIndex = 64;
            this.Lbl_BuscarPor.Text = "BUSCAR POR:";
            // 
            // Btn_ClearSearch
            // 
            this.Btn_ClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_ClearSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ClearSearch.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_ClearSearch.Location = new System.Drawing.Point(827, 20);
            this.Btn_ClearSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Btn_ClearSearch.Name = "Btn_ClearSearch";
            this.Btn_ClearSearch.Size = new System.Drawing.Size(47, 55);
            this.Btn_ClearSearch.TabIndex = 63;
            this.Btn_ClearSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_ClearSearch.UseVisualStyleBackColor = true;
            this.Btn_ClearSearch.Click += new System.EventHandler(this.Btn_ClearSearch_Click);
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.Location = new System.Drawing.Point(772, 20);
            this.Btn_Search.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(47, 55);
            this.Btn_Search.TabIndex = 62;
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Lbl_ValorBuscado
            // 
            this.Lbl_ValorBuscado.AutoSize = true;
            this.Lbl_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ValorBuscado.ForeColor = System.Drawing.Color.Black;
            this.Lbl_ValorBuscado.Location = new System.Drawing.Point(13, 84);
            this.Lbl_ValorBuscado.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lbl_ValorBuscado.Name = "Lbl_ValorBuscado";
            this.Lbl_ValorBuscado.Size = new System.Drawing.Size(211, 25);
            this.Lbl_ValorBuscado.TabIndex = 61;
            this.Lbl_ValorBuscado.Text = "BUSCAR TRANSICIÓN:";
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(19, 111);
            this.Txt_ValorBuscado.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(851, 32);
            this.Txt_ValorBuscado.TabIndex = 60;
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1312, 68);
            this.Panel_Superior.TabIndex = 100;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(699, 32);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "TRANSICIONES PARA ESTADOS DE TRASLADOS DE ACTIVOS";
            // 
            // Frm_FixedAsset_TransferStatusTransitions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1312, 752);
            this.Controls.Add(this.Panel_CRUD);
            this.Controls.Add(this.Panel_1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_DetalleTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Frm_FixedAsset_TransferStatusTransitions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - TRANSICIONES PARA ESTADOS DE TRASLADOS DE ACTIVOS";
            this.Panel_CRUD.ResumeLayout(false);
            this.Panel_1.ResumeLayout(false);
            this.Panel_1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.Panel_DetalleTabla.ResumeLayout(false);
            this.Panel_DetalleTabla.PerformLayout();
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Btn_DELETE;
        private System.Windows.Forms.Panel Panel_CRUD;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Panel Panel_1;
        private System.Windows.Forms.Label Lbl_Subtitulo1;
        private System.Windows.Forms.TextBox Txt_FromStatus;
        private System.Windows.Forms.Label Lbl_ToStatus;
        private System.Windows.Forms.Label Lbl_FromStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Btn_No;
        private System.Windows.Forms.Button Btn_Yes;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel Panel_DetalleTabla;
        private System.Windows.Forms.ComboBox ComboBox_BuscarPor;
        private System.Windows.Forms.Label Lbl_BuscarPor;
        private System.Windows.Forms.Button Btn_ClearSearch;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Label Lbl_ValorBuscado;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.ComboBox ComboBox_ToStatus;
    }
}