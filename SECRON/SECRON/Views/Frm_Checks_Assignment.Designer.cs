namespace SECRON.Views
{
    partial class Frm_Checks_Assignment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Checks_Assignment));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.Txt_SiguienteCheque = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.Btn_Clear2 = new System.Windows.Forms.Button();
            this.Btn_Delete = new System.Windows.Forms.Button();
            this.Btn_Update = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.CheckBox_Compartido = new System.Windows.Forms.CheckBox();
            this.Lbl_Guion = new System.Windows.Forms.Label();
            this.Lbl_fin = new System.Windows.Forms.Label();
            this.Txt_Fin = new System.Windows.Forms.TextBox();
            this.Lbl_Li = new System.Windows.Forms.Label();
            this.Txt_Li = new System.Windows.Forms.TextBox();
            this.Lbl_Beneficiario = new System.Windows.Forms.Label();
            this.Txt_Seleccionado = new System.Windows.Forms.TextBox();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla1 = new System.Windows.Forms.DataGridView();
            this.Panel_DetalleTabla = new System.Windows.Forms.Panel();
            this.ComboBox_BuscarPor = new System.Windows.Forms.ComboBox();
            this.Lbl_BuscarPor = new System.Windows.Forms.Label();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Lbl_ValorBuscado = new System.Windows.Forms.Label();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.Lbl_Tabla1 = new System.Windows.Forms.Label();
            this.Panel_Tabla2 = new System.Windows.Forms.Panel();
            this.Tabla2 = new System.Windows.Forms.DataGridView();
            this.CheckBox_AltaPrioridad = new System.Windows.Forms.CheckBox();
            this.Panel_Superior.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla1)).BeginInit();
            this.Panel_DetalleTabla.SuspendLayout();
            this.panel2.SuspendLayout();
            this.Panel_Tabla2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla2)).BeginInit();
            this.SuspendLayout();
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(143)))), ((int)(((byte)(109)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(684, 55);
            this.Panel_Superior.TabIndex = 76;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(509, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "ASIGNACIÓN DE CHEQUES - USUARIOS CON PERMISOS";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.CheckBox_AltaPrioridad);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.Txt_SiguienteCheque);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.CheckBox_Compartido);
            this.panel1.Controls.Add(this.Lbl_Guion);
            this.panel1.Controls.Add(this.Lbl_fin);
            this.panel1.Controls.Add(this.Txt_Fin);
            this.panel1.Controls.Add(this.Lbl_Li);
            this.panel1.Controls.Add(this.Txt_Li);
            this.panel1.Controls.Add(this.Lbl_Beneficiario);
            this.panel1.Controls.Add(this.Txt_Seleccionado);
            this.panel1.Location = new System.Drawing.Point(9, 426);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(665, 176);
            this.panel1.TabIndex = 81;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(459, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(177, 20);
            this.label2.TabIndex = 84;
            this.label2.Text = "CHEQUE POR IMPRIMIR";
            // 
            // Txt_SiguienteCheque
            // 
            this.Txt_SiguienteCheque.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_SiguienteCheque.Location = new System.Drawing.Point(463, 84);
            this.Txt_SiguienteCheque.MaxLength = 15;
            this.Txt_SiguienteCheque.Name = "Txt_SiguienteCheque";
            this.Txt_SiguienteCheque.Size = new System.Drawing.Size(190, 27);
            this.Txt_SiguienteCheque.TabIndex = 83;
            this.Txt_SiguienteCheque.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Txt_SiguienteCheque_KeyPress);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel3.Controls.Add(this.Btn_Clear2);
            this.panel3.Controls.Add(this.Btn_Delete);
            this.panel3.Controls.Add(this.Btn_Update);
            this.panel3.Controls.Add(this.Btn_Save);
            this.panel3.Location = new System.Drawing.Point(270, 117);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(384, 47);
            this.panel3.TabIndex = 82;
            // 
            // Btn_Clear2
            // 
            this.Btn_Clear2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Clear2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear2.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Clear2.Location = new System.Drawing.Point(350, 3);
            this.Btn_Clear2.Name = "Btn_Clear2";
            this.Btn_Clear2.Size = new System.Drawing.Size(33, 37);
            this.Btn_Clear2.TabIndex = 57;
            this.Btn_Clear2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Clear2.UseVisualStyleBackColor = true;
            this.Btn_Clear2.Click += new System.EventHandler(this.Btn_Clear2_Click);
            // 
            // Btn_Delete
            // 
            this.Btn_Delete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Delete.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Delete.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Delete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Delete.Location = new System.Drawing.Point(224, 3);
            this.Btn_Delete.Name = "Btn_Delete";
            this.Btn_Delete.Size = new System.Drawing.Size(124, 37);
            this.Btn_Delete.TabIndex = 56;
            this.Btn_Delete.Text = "ELIMINAR";
            this.Btn_Delete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Delete.UseVisualStyleBackColor = true;
            this.Btn_Delete.Click += new System.EventHandler(this.Btn_Delete_Click);
            // 
            // Btn_Update
            // 
            this.Btn_Update.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Update.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Update.Image = global::SECRON.Properties.Resources.UpdateAzul25x25;
            this.Btn_Update.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Update.Location = new System.Drawing.Point(122, 3);
            this.Btn_Update.Name = "Btn_Update";
            this.Btn_Update.Size = new System.Drawing.Size(98, 37);
            this.Btn_Update.TabIndex = 55;
            this.Btn_Update.Text = "EDITAR";
            this.Btn_Update.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Update.UseVisualStyleBackColor = true;
            this.Btn_Update.Click += new System.EventHandler(this.Btn_Update_Click);
            // 
            // Btn_Save
            // 
            this.Btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Save.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Save.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Save.Location = new System.Drawing.Point(0, 3);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(117, 37);
            this.Btn_Save.TabIndex = 54;
            this.Btn_Save.Text = "GUARDAR";
            this.Btn_Save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // CheckBox_Compartido
            // 
            this.CheckBox_Compartido.AutoSize = true;
            this.CheckBox_Compartido.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.CheckBox_Compartido.Location = new System.Drawing.Point(381, 5);
            this.CheckBox_Compartido.Name = "CheckBox_Compartido";
            this.CheckBox_Compartido.Size = new System.Drawing.Size(272, 23);
            this.CheckBox_Compartido.TabIndex = 81;
            this.CheckBox_Compartido.Text = "RANGO DE CHEQUES COMPARTIDOS";
            this.CheckBox_Compartido.UseVisualStyleBackColor = true;
            // 
            // Lbl_Guion
            // 
            this.Lbl_Guion.AutoSize = true;
            this.Lbl_Guion.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.Lbl_Guion.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Guion.Location = new System.Drawing.Point(211, 73);
            this.Lbl_Guion.Name = "Lbl_Guion";
            this.Lbl_Guion.Size = new System.Drawing.Size(31, 30);
            this.Lbl_Guion.TabIndex = 79;
            this.Lbl_Guion.Text = "__";
            // 
            // Lbl_fin
            // 
            this.Lbl_fin.AutoSize = true;
            this.Lbl_fin.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_fin.ForeColor = System.Drawing.Color.Black;
            this.Lbl_fin.Location = new System.Drawing.Point(248, 62);
            this.Lbl_fin.Name = "Lbl_fin";
            this.Lbl_fin.Size = new System.Drawing.Size(106, 20);
            this.Lbl_fin.TabIndex = 78;
            this.Lbl_fin.Text = "LIMITE FINAL";
            // 
            // Txt_Fin
            // 
            this.Txt_Fin.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Fin.Location = new System.Drawing.Point(252, 84);
            this.Txt_Fin.MaxLength = 15;
            this.Txt_Fin.Name = "Txt_Fin";
            this.Txt_Fin.Size = new System.Drawing.Size(190, 27);
            this.Txt_Fin.TabIndex = 77;
            this.Txt_Fin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Txt_Fin_KeyPress);
            // 
            // Lbl_Li
            // 
            this.Lbl_Li.AutoSize = true;
            this.Lbl_Li.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Li.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Li.Location = new System.Drawing.Point(10, 62);
            this.Lbl_Li.Name = "Lbl_Li";
            this.Lbl_Li.Size = new System.Drawing.Size(117, 20);
            this.Lbl_Li.TabIndex = 76;
            this.Lbl_Li.Text = "LIMITE INICIAL";
            // 
            // Txt_Li
            // 
            this.Txt_Li.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Li.Location = new System.Drawing.Point(14, 84);
            this.Txt_Li.MaxLength = 15;
            this.Txt_Li.Name = "Txt_Li";
            this.Txt_Li.Size = new System.Drawing.Size(190, 27);
            this.Txt_Li.TabIndex = 75;
            this.Txt_Li.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Txt_Li_KeyPress);
            // 
            // Lbl_Beneficiario
            // 
            this.Lbl_Beneficiario.AutoSize = true;
            this.Lbl_Beneficiario.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Beneficiario.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Beneficiario.Location = new System.Drawing.Point(10, 8);
            this.Lbl_Beneficiario.Name = "Lbl_Beneficiario";
            this.Lbl_Beneficiario.Size = new System.Drawing.Size(195, 20);
            this.Lbl_Beneficiario.TabIndex = 61;
            this.Lbl_Beneficiario.Text = "USUARIO SELECCIONADO:";
            // 
            // Txt_Seleccionado
            // 
            this.Txt_Seleccionado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Seleccionado.Location = new System.Drawing.Point(14, 31);
            this.Txt_Seleccionado.MaxLength = 15;
            this.Txt_Seleccionado.Name = "Txt_Seleccionado";
            this.Txt_Seleccionado.Size = new System.Drawing.Size(639, 27);
            this.Txt_Seleccionado.TabIndex = 60;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla1);
            this.PanelTabla.Location = new System.Drawing.Point(9, 222);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(266, 196);
            this.PanelTabla.TabIndex = 80;
            // 
            // Tabla1
            // 
            this.Tabla1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla1.Location = new System.Drawing.Point(0, 0);
            this.Tabla1.Name = "Tabla1";
            this.Tabla1.Size = new System.Drawing.Size(266, 196);
            this.Tabla1.TabIndex = 1;
            // 
            // Panel_DetalleTabla
            // 
            this.Panel_DetalleTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_DetalleTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_DetalleTabla.Controls.Add(this.ComboBox_BuscarPor);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_BuscarPor);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Clear);
            this.Panel_DetalleTabla.Controls.Add(this.Btn_Search);
            this.Panel_DetalleTabla.Controls.Add(this.Lbl_ValorBuscado);
            this.Panel_DetalleTabla.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_DetalleTabla.Location = new System.Drawing.Point(9, 61);
            this.Panel_DetalleTabla.Name = "Panel_DetalleTabla";
            this.Panel_DetalleTabla.Size = new System.Drawing.Size(663, 112);
            this.Panel_DetalleTabla.TabIndex = 79;
            // 
            // ComboBox_BuscarPor
            // 
            this.ComboBox_BuscarPor.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ComboBox_BuscarPor.FormattingEnabled = true;
            this.ComboBox_BuscarPor.Location = new System.Drawing.Point(15, 27);
            this.ComboBox_BuscarPor.Name = "ComboBox_BuscarPor";
            this.ComboBox_BuscarPor.Size = new System.Drawing.Size(557, 26);
            this.ComboBox_BuscarPor.TabIndex = 71;
            // 
            // Lbl_BuscarPor
            // 
            this.Lbl_BuscarPor.AutoSize = true;
            this.Lbl_BuscarPor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_BuscarPor.ForeColor = System.Drawing.Color.Black;
            this.Lbl_BuscarPor.Location = new System.Drawing.Point(11, 4);
            this.Lbl_BuscarPor.Name = "Lbl_BuscarPor";
            this.Lbl_BuscarPor.Size = new System.Drawing.Size(106, 20);
            this.Lbl_BuscarPor.TabIndex = 64;
            this.Lbl_BuscarPor.Text = "BUSCAR POR:";
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.Location = new System.Drawing.Point(619, 8);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(35, 45);
            this.Btn_Clear.TabIndex = 63;
            this.Btn_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            this.Btn_Clear.Click += new System.EventHandler(this.Btn_Clear_Click);
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.Location = new System.Drawing.Point(578, 8);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(35, 45);
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
            this.Lbl_ValorBuscado.Location = new System.Drawing.Point(10, 56);
            this.Lbl_ValorBuscado.Name = "Lbl_ValorBuscado";
            this.Lbl_ValorBuscado.Size = new System.Drawing.Size(178, 20);
            this.Lbl_ValorBuscado.TabIndex = 61;
            this.Lbl_ValorBuscado.Text = "BUSCAR BENEFICIARIO:";
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(14, 78);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(639, 27);
            this.Txt_ValorBuscado.TabIndex = 60;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.Lbl_Tabla1);
            this.panel2.Location = new System.Drawing.Point(9, 179);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(663, 37);
            this.panel2.TabIndex = 82;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(268, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 20);
            this.label1.TabIndex = 52;
            this.label1.Text = "RANGOS EN EL SISTEMA";
            // 
            // Lbl_Tabla1
            // 
            this.Lbl_Tabla1.AutoSize = true;
            this.Lbl_Tabla1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Tabla1.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Tabla1.Location = new System.Drawing.Point(4, 7);
            this.Lbl_Tabla1.Name = "Lbl_Tabla1";
            this.Lbl_Tabla1.Size = new System.Drawing.Size(197, 20);
            this.Lbl_Tabla1.TabIndex = 51;
            this.Lbl_Tabla1.Text = "USUARIOS CON PERMISOS";
            // 
            // Panel_Tabla2
            // 
            this.Panel_Tabla2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Tabla2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Tabla2.Controls.Add(this.Tabla2);
            this.Panel_Tabla2.Location = new System.Drawing.Point(281, 222);
            this.Panel_Tabla2.Name = "Panel_Tabla2";
            this.Panel_Tabla2.Size = new System.Drawing.Size(391, 196);
            this.Panel_Tabla2.TabIndex = 83;
            // 
            // Tabla2
            // 
            this.Tabla2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla2.Location = new System.Drawing.Point(0, 0);
            this.Tabla2.Name = "Tabla2";
            this.Tabla2.Size = new System.Drawing.Size(391, 196);
            this.Tabla2.TabIndex = 1;
            // 
            // CheckBox_AltaPrioridad
            // 
            this.CheckBox_AltaPrioridad.AutoSize = true;
            this.CheckBox_AltaPrioridad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.CheckBox_AltaPrioridad.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.CheckBox_AltaPrioridad.Location = new System.Drawing.Point(15, 129);
            this.CheckBox_AltaPrioridad.Name = "CheckBox_AltaPrioridad";
            this.CheckBox_AltaPrioridad.Size = new System.Drawing.Size(231, 23);
            this.CheckBox_AltaPrioridad.TabIndex = 85;
            this.CheckBox_AltaPrioridad.Text = "RANGO CON ALTA PRIORIDAD";
            this.CheckBox_AltaPrioridad.UseVisualStyleBackColor = false;
            // 
            // Frm_Checks_Assignment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(684, 611);
            this.Controls.Add(this.Panel_Tabla2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PanelTabla);
            this.Controls.Add(this.Panel_DetalleTabla);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_Checks_Assignment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - ASIGNACIÓN DE CHEQUES";
            this.Load += new System.EventHandler(this.Frm_Checks_Assignment_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla1)).EndInit();
            this.Panel_DetalleTabla.ResumeLayout(false);
            this.Panel_DetalleTabla.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.Panel_Tabla2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Lbl_Beneficiario;
        private System.Windows.Forms.TextBox Txt_Seleccionado;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla1;
        private System.Windows.Forms.Panel Panel_DetalleTabla;
        private System.Windows.Forms.ComboBox ComboBox_BuscarPor;
        private System.Windows.Forms.Label Lbl_BuscarPor;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Label Lbl_ValorBuscado;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Label Lbl_Guion;
        private System.Windows.Forms.Label Lbl_fin;
        private System.Windows.Forms.TextBox Txt_Fin;
        private System.Windows.Forms.Label Lbl_Li;
        private System.Windows.Forms.TextBox Txt_Li;
        private System.Windows.Forms.CheckBox CheckBox_Compartido;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label Lbl_Tabla1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel Panel_Tabla2;
        private System.Windows.Forms.DataGridView Tabla2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button Btn_Clear2;
        private System.Windows.Forms.Button Btn_Delete;
        private System.Windows.Forms.Button Btn_Update;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Txt_SiguienteCheque;
        private System.Windows.Forms.CheckBox CheckBox_AltaPrioridad;
    }
}