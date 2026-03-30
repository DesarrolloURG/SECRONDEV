namespace SECRON.Views
{
    partial class Frm_Accounts_Managment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Accounts_Managment));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Btn_Import = new System.Windows.Forms.Button();
            this.Btn_Export = new System.Windows.Forms.Button();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_Busqueda = new System.Windows.Forms.Panel();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Inactive = new System.Windows.Forms.Button();
            this.Btn_Update = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Panel_Derecho = new System.Windows.Forms.Panel();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.Panel_Izquierdo = new System.Windows.Forms.Panel();
            this.ComboBox_Type = new System.Windows.Forms.ComboBox();
            this.Txt_Balance = new System.Windows.Forms.TextBox();
            this.Lbl_Type = new System.Windows.Forms.Label();
            this.Lbl_Balance = new System.Windows.Forms.Label();
            this.Panel_Informacion = new System.Windows.Forms.Panel();
            this.Txt_Name = new System.Windows.Forms.TextBox();
            this.Txt_ParentAccountCode = new System.Windows.Forms.TextBox();
            this.Txt_Code = new System.Windows.Forms.TextBox();
            this.Lbl_Name = new System.Windows.Forms.Label();
            this.Lbl_ParentAccountCode = new System.Windows.Forms.Label();
            this.Lbl_Code = new System.Windows.Forms.Label();
            this.Lbl_Subtitulo1 = new System.Windows.Forms.Label();
            this.ComboBox_Level = new System.Windows.Forms.ComboBox();
            this.Lbl_Level = new System.Windows.Forms.Label();
            this.ComboBox_Sign = new System.Windows.Forms.ComboBox();
            this.Lbl_Sign = new System.Windows.Forms.Label();
            this.ComboBox_BankName = new System.Windows.Forms.ComboBox();
            this.Lbl_BankName = new System.Windows.Forms.Label();
            this.ComboBox_BankAccountType = new System.Windows.Forms.ComboBox();
            this.Lbl_BankAccountType = new System.Windows.Forms.Label();
            this.ComboBox_Currency = new System.Windows.Forms.ComboBox();
            this.Lbl_Currency = new System.Windows.Forms.Label();
            this.Lbl_CheckNumber = new System.Windows.Forms.Label();
            this.Txt_CheckNumber = new System.Windows.Forms.TextBox();
            this.Txt_CurrencyName = new System.Windows.Forms.TextBox();
            this.Lbl_CurrencyName = new System.Windows.Forms.Label();
            this.Panel_Superior.SuspendLayout();
            this.Panel_Busqueda.SuspendLayout();
            this.Panel_Derecho.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.Panel_Izquierdo.SuspendLayout();
            this.Panel_Informacion.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.Panel_Superior.Controls.Add(this.Btn_Import);
            this.Panel_Superior.Controls.Add(this.Btn_Export);
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1184, 55);
            this.Panel_Superior.TabIndex = 1;
            // 
            // Btn_Import
            // 
            this.Btn_Import.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Import.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Import.Image = global::SECRON.Properties.Resources.ImportarExcelNegro25x25;
            this.Btn_Import.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Import.Location = new System.Drawing.Point(1053, 12);
            this.Btn_Import.Name = "Btn_Import";
            this.Btn_Import.Size = new System.Drawing.Size(119, 30);
            this.Btn_Import.TabIndex = 53;
            this.Btn_Import.Text = "IMPORTAR";
            this.Btn_Import.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Import.UseVisualStyleBackColor = true;
            // 
            // Btn_Export
            // 
            this.Btn_Export.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Export.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Export.Image = global::SECRON.Properties.Resources.ExportarExcelNegro25x25;
            this.Btn_Export.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Export.Location = new System.Drawing.Point(928, 13);
            this.Btn_Export.Name = "Btn_Export";
            this.Btn_Export.Size = new System.Drawing.Size(119, 30);
            this.Btn_Export.TabIndex = 52;
            this.Btn_Export.Text = "EXPORTAR";
            this.Btn_Export.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Export.UseVisualStyleBackColor = true;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(230, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "CATÁLOGO DE CUENTAS";
            // 
            // Panel_Busqueda
            // 
            this.Panel_Busqueda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Busqueda.Controls.Add(this.Btn_Search);
            this.Panel_Busqueda.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_Busqueda.Controls.Add(this.Btn_Clear);
            this.Panel_Busqueda.Controls.Add(this.Btn_Inactive);
            this.Panel_Busqueda.Controls.Add(this.Btn_Update);
            this.Panel_Busqueda.Controls.Add(this.Btn_Save);
            this.Panel_Busqueda.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Busqueda.Location = new System.Drawing.Point(0, 55);
            this.Panel_Busqueda.Name = "Panel_Busqueda";
            this.Panel_Busqueda.Size = new System.Drawing.Size(1184, 47);
            this.Panel_Busqueda.TabIndex = 76;
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Clear.Location = new System.Drawing.Point(1138, 3);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(33, 37);
            this.Btn_Clear.TabIndex = 57;
            this.Btn_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            // 
            // Btn_Inactive
            // 
            this.Btn_Inactive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Inactive.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Inactive.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Inactive.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Inactive.Location = new System.Drawing.Point(1012, 3);
            this.Btn_Inactive.Name = "Btn_Inactive";
            this.Btn_Inactive.Size = new System.Drawing.Size(124, 37);
            this.Btn_Inactive.TabIndex = 56;
            this.Btn_Inactive.Text = "INACTIVAR";
            this.Btn_Inactive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Inactive.UseVisualStyleBackColor = true;
            // 
            // Btn_Update
            // 
            this.Btn_Update.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Update.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Update.Image = global::SECRON.Properties.Resources.UpdateAzul25x25;
            this.Btn_Update.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Update.Location = new System.Drawing.Point(910, 3);
            this.Btn_Update.Name = "Btn_Update";
            this.Btn_Update.Size = new System.Drawing.Size(98, 37);
            this.Btn_Update.TabIndex = 55;
            this.Btn_Update.Text = "EDITAR";
            this.Btn_Update.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Update.UseVisualStyleBackColor = true;
            // 
            // Btn_Save
            // 
            this.Btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Save.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Save.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Save.Location = new System.Drawing.Point(788, 3);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(117, 37);
            this.Btn_Save.TabIndex = 54;
            this.Btn_Save.Text = "GUARDAR";
            this.Btn_Save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Save.UseVisualStyleBackColor = true;
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(53, 9);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(729, 27);
            this.Txt_ValorBuscado.TabIndex = 58;
            // 
            // Btn_Search
            // 
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Search.Location = new System.Drawing.Point(13, 6);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(34, 31);
            this.Btn_Search.TabIndex = 59;
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
            // 
            // Panel_Derecho
            // 
            this.Panel_Derecho.Controls.Add(this.PanelTabla);
            this.Panel_Derecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Derecho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Derecho.ForeColor = System.Drawing.Color.Black;
            this.Panel_Derecho.Location = new System.Drawing.Point(415, 102);
            this.Panel_Derecho.Name = "Panel_Derecho";
            this.Panel_Derecho.Size = new System.Drawing.Size(769, 759);
            this.Panel_Derecho.TabIndex = 78;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(15, 6);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(741, 741);
            this.PanelTabla.TabIndex = 72;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(741, 741);
            this.Tabla.TabIndex = 1;
            // 
            // Panel_Izquierdo
            // 
            this.Panel_Izquierdo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Izquierdo.Controls.Add(this.Panel_Informacion);
            this.Panel_Izquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.Panel_Izquierdo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Izquierdo.ForeColor = System.Drawing.Color.Black;
            this.Panel_Izquierdo.Location = new System.Drawing.Point(0, 102);
            this.Panel_Izquierdo.Name = "Panel_Izquierdo";
            this.Panel_Izquierdo.Size = new System.Drawing.Size(415, 759);
            this.Panel_Izquierdo.TabIndex = 77;
            // 
            // ComboBox_Type
            // 
            this.ComboBox_Type.FormattingEnabled = true;
            this.ComboBox_Type.Location = new System.Drawing.Point(14, 203);
            this.ComboBox_Type.Name = "ComboBox_Type";
            this.ComboBox_Type.Size = new System.Drawing.Size(351, 28);
            this.ComboBox_Type.TabIndex = 66;
            // 
            // Txt_Balance
            // 
            this.Txt_Balance.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Balance.Location = new System.Drawing.Point(14, 337);
            this.Txt_Balance.MaxLength = 15;
            this.Txt_Balance.Name = "Txt_Balance";
            this.Txt_Balance.Size = new System.Drawing.Size(352, 27);
            this.Txt_Balance.TabIndex = 12;
            this.Txt_Balance.Text = "0.00";
            // 
            // Lbl_Type
            // 
            this.Lbl_Type.AutoSize = true;
            this.Lbl_Type.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Type.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Type.Location = new System.Drawing.Point(10, 181);
            this.Lbl_Type.Name = "Lbl_Type";
            this.Lbl_Type.Size = new System.Drawing.Size(54, 20);
            this.Lbl_Type.TabIndex = 56;
            this.Lbl_Type.Text = "TIPO *";
            // 
            // Lbl_Balance
            // 
            this.Lbl_Balance.AutoSize = true;
            this.Lbl_Balance.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Balance.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Balance.Location = new System.Drawing.Point(10, 314);
            this.Lbl_Balance.Name = "Lbl_Balance";
            this.Lbl_Balance.Size = new System.Drawing.Size(145, 20);
            this.Lbl_Balance.TabIndex = 12;
            this.Lbl_Balance.Text = "SALDO EN CUENTA";
            // 
            // Panel_Informacion
            // 
            this.Panel_Informacion.BackColor = System.Drawing.Color.White;
            this.Panel_Informacion.Controls.Add(this.Txt_CurrencyName);
            this.Panel_Informacion.Controls.Add(this.Lbl_CurrencyName);
            this.Panel_Informacion.Controls.Add(this.Txt_CheckNumber);
            this.Panel_Informacion.Controls.Add(this.ComboBox_Currency);
            this.Panel_Informacion.Controls.Add(this.Lbl_Currency);
            this.Panel_Informacion.Controls.Add(this.Lbl_CheckNumber);
            this.Panel_Informacion.Controls.Add(this.ComboBox_BankAccountType);
            this.Panel_Informacion.Controls.Add(this.Lbl_BankAccountType);
            this.Panel_Informacion.Controls.Add(this.ComboBox_BankName);
            this.Panel_Informacion.Controls.Add(this.Lbl_BankName);
            this.Panel_Informacion.Controls.Add(this.ComboBox_Sign);
            this.Panel_Informacion.Controls.Add(this.Lbl_Sign);
            this.Panel_Informacion.Controls.Add(this.ComboBox_Level);
            this.Panel_Informacion.Controls.Add(this.Lbl_Level);
            this.Panel_Informacion.Controls.Add(this.ComboBox_Type);
            this.Panel_Informacion.Controls.Add(this.Txt_Balance);
            this.Panel_Informacion.Controls.Add(this.Txt_Name);
            this.Panel_Informacion.Controls.Add(this.Txt_ParentAccountCode);
            this.Panel_Informacion.Controls.Add(this.Lbl_Balance);
            this.Panel_Informacion.Controls.Add(this.Txt_Code);
            this.Panel_Informacion.Controls.Add(this.Lbl_Type);
            this.Panel_Informacion.Controls.Add(this.Lbl_Name);
            this.Panel_Informacion.Controls.Add(this.Lbl_ParentAccountCode);
            this.Panel_Informacion.Controls.Add(this.Lbl_Code);
            this.Panel_Informacion.Controls.Add(this.Lbl_Subtitulo1);
            this.Panel_Informacion.Location = new System.Drawing.Point(16, 6);
            this.Panel_Informacion.Name = "Panel_Informacion";
            this.Panel_Informacion.Size = new System.Drawing.Size(383, 741);
            this.Panel_Informacion.TabIndex = 52;
            // 
            // Txt_Name
            // 
            this.Txt_Name.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Name.Location = new System.Drawing.Point(14, 134);
            this.Txt_Name.MaxLength = 15;
            this.Txt_Name.Name = "Txt_Name";
            this.Txt_Name.Size = new System.Drawing.Size(351, 27);
            this.Txt_Name.TabIndex = 3;
            // 
            // Txt_ParentAccountCode
            // 
            this.Txt_ParentAccountCode.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ParentAccountCode.Location = new System.Drawing.Point(196, 68);
            this.Txt_ParentAccountCode.MaxLength = 15;
            this.Txt_ParentAccountCode.Name = "Txt_ParentAccountCode";
            this.Txt_ParentAccountCode.Size = new System.Drawing.Size(170, 27);
            this.Txt_ParentAccountCode.TabIndex = 2;
            // 
            // Txt_Code
            // 
            this.Txt_Code.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Code.Location = new System.Drawing.Point(14, 68);
            this.Txt_Code.MaxLength = 15;
            this.Txt_Code.Name = "Txt_Code";
            this.Txt_Code.Size = new System.Drawing.Size(170, 27);
            this.Txt_Code.TabIndex = 1;
            // 
            // Lbl_Name
            // 
            this.Lbl_Name.AutoSize = true;
            this.Lbl_Name.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Name.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Name.Location = new System.Drawing.Point(10, 111);
            this.Lbl_Name.Name = "Lbl_Name";
            this.Lbl_Name.Size = new System.Drawing.Size(148, 20);
            this.Lbl_Name.TabIndex = 3;
            this.Lbl_Name.Text = "NOMBRE CUENTA *";
            // 
            // Lbl_ParentAccountCode
            // 
            this.Lbl_ParentAccountCode.AutoSize = true;
            this.Lbl_ParentAccountCode.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ParentAccountCode.ForeColor = System.Drawing.Color.Black;
            this.Lbl_ParentAccountCode.Location = new System.Drawing.Point(196, 45);
            this.Lbl_ParentAccountCode.Name = "Lbl_ParentAccountCode";
            this.Lbl_ParentAccountCode.Size = new System.Drawing.Size(169, 20);
            this.Lbl_ParentAccountCode.TabIndex = 2;
            this.Lbl_ParentAccountCode.Text = "COD. CUENTA PADRE *";
            // 
            // Lbl_Code
            // 
            this.Lbl_Code.AutoSize = true;
            this.Lbl_Code.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Code.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Code.Location = new System.Drawing.Point(10, 45);
            this.Lbl_Code.Name = "Lbl_Code";
            this.Lbl_Code.Size = new System.Drawing.Size(130, 20);
            this.Lbl_Code.TabIndex = 1;
            this.Lbl_Code.Text = "CÓDIGO CUENTA";
            // 
            // Lbl_Subtitulo1
            // 
            this.Lbl_Subtitulo1.AutoSize = true;
            this.Lbl_Subtitulo1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo1.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo1.Image = global::SECRON.Properties.Resources.DescripcionItemBlanco25x25;
            this.Lbl_Subtitulo1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo1.Location = new System.Drawing.Point(10, 10);
            this.Lbl_Subtitulo1.Name = "Lbl_Subtitulo1";
            this.Lbl_Subtitulo1.Size = new System.Drawing.Size(250, 20);
            this.Lbl_Subtitulo1.TabIndex = 1;
            this.Lbl_Subtitulo1.Text = "      INFORMACIÓN DE LA CUENTA";
            this.Lbl_Subtitulo1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ComboBox_Level
            // 
            this.ComboBox_Level.FormattingEnabled = true;
            this.ComboBox_Level.Location = new System.Drawing.Point(14, 267);
            this.ComboBox_Level.Name = "ComboBox_Level";
            this.ComboBox_Level.Size = new System.Drawing.Size(170, 28);
            this.ComboBox_Level.TabIndex = 68;
            // 
            // Lbl_Level
            // 
            this.Lbl_Level.AutoSize = true;
            this.Lbl_Level.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Level.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Level.Location = new System.Drawing.Point(10, 245);
            this.Lbl_Level.Name = "Lbl_Level";
            this.Lbl_Level.Size = new System.Drawing.Size(63, 20);
            this.Lbl_Level.TabIndex = 67;
            this.Lbl_Level.Text = "NIVEL *";
            // 
            // ComboBox_Sign
            // 
            this.ComboBox_Sign.FormattingEnabled = true;
            this.ComboBox_Sign.Location = new System.Drawing.Point(200, 267);
            this.ComboBox_Sign.Name = "ComboBox_Sign";
            this.ComboBox_Sign.Size = new System.Drawing.Size(165, 28);
            this.ComboBox_Sign.TabIndex = 70;
            // 
            // Lbl_Sign
            // 
            this.Lbl_Sign.AutoSize = true;
            this.Lbl_Sign.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Sign.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Sign.Location = new System.Drawing.Point(196, 245);
            this.Lbl_Sign.Name = "Lbl_Sign";
            this.Lbl_Sign.Size = new System.Drawing.Size(67, 20);
            this.Lbl_Sign.TabIndex = 69;
            this.Lbl_Sign.Text = "SIGNO *";
            // 
            // ComboBox_BankName
            // 
            this.ComboBox_BankName.FormattingEnabled = true;
            this.ComboBox_BankName.Location = new System.Drawing.Point(14, 403);
            this.ComboBox_BankName.Name = "ComboBox_BankName";
            this.ComboBox_BankName.Size = new System.Drawing.Size(351, 28);
            this.ComboBox_BankName.TabIndex = 72;
            // 
            // Lbl_BankName
            // 
            this.Lbl_BankName.AutoSize = true;
            this.Lbl_BankName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_BankName.ForeColor = System.Drawing.Color.Black;
            this.Lbl_BankName.Location = new System.Drawing.Point(10, 381);
            this.Lbl_BankName.Name = "Lbl_BankName";
            this.Lbl_BankName.Size = new System.Drawing.Size(73, 20);
            this.Lbl_BankName.TabIndex = 71;
            this.Lbl_BankName.Text = "BANCO *";
            // 
            // ComboBox_BankAccountType
            // 
            this.ComboBox_BankAccountType.FormattingEnabled = true;
            this.ComboBox_BankAccountType.Location = new System.Drawing.Point(14, 462);
            this.ComboBox_BankAccountType.Name = "ComboBox_BankAccountType";
            this.ComboBox_BankAccountType.Size = new System.Drawing.Size(351, 28);
            this.ComboBox_BankAccountType.TabIndex = 74;
            // 
            // Lbl_BankAccountType
            // 
            this.Lbl_BankAccountType.AutoSize = true;
            this.Lbl_BankAccountType.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_BankAccountType.ForeColor = System.Drawing.Color.Black;
            this.Lbl_BankAccountType.Location = new System.Drawing.Point(10, 440);
            this.Lbl_BankAccountType.Name = "Lbl_BankAccountType";
            this.Lbl_BankAccountType.Size = new System.Drawing.Size(140, 20);
            this.Lbl_BankAccountType.TabIndex = 73;
            this.Lbl_BankAccountType.Text = "TIPO DE CUENTA *";
            // 
            // ComboBox_Currency
            // 
            this.ComboBox_Currency.FormattingEnabled = true;
            this.ComboBox_Currency.Location = new System.Drawing.Point(200, 524);
            this.ComboBox_Currency.Name = "ComboBox_Currency";
            this.ComboBox_Currency.Size = new System.Drawing.Size(165, 28);
            this.ComboBox_Currency.TabIndex = 78;
            // 
            // Lbl_Currency
            // 
            this.Lbl_Currency.AutoSize = true;
            this.Lbl_Currency.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Currency.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Currency.Location = new System.Drawing.Point(196, 502);
            this.Lbl_Currency.Name = "Lbl_Currency";
            this.Lbl_Currency.Size = new System.Drawing.Size(87, 20);
            this.Lbl_Currency.TabIndex = 77;
            this.Lbl_Currency.Text = "MONEDA *";
            // 
            // Lbl_CheckNumber
            // 
            this.Lbl_CheckNumber.AutoSize = true;
            this.Lbl_CheckNumber.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_CheckNumber.ForeColor = System.Drawing.Color.Black;
            this.Lbl_CheckNumber.Location = new System.Drawing.Point(10, 502);
            this.Lbl_CheckNumber.Name = "Lbl_CheckNumber";
            this.Lbl_CheckNumber.Size = new System.Drawing.Size(171, 20);
            this.Lbl_CheckNumber.TabIndex = 75;
            this.Lbl_CheckNumber.Text = "NUMERO DE CHEQUE *";
            // 
            // Txt_CheckNumber
            // 
            this.Txt_CheckNumber.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_CheckNumber.Location = new System.Drawing.Point(14, 525);
            this.Txt_CheckNumber.MaxLength = 15;
            this.Txt_CheckNumber.Name = "Txt_CheckNumber";
            this.Txt_CheckNumber.Size = new System.Drawing.Size(170, 27);
            this.Txt_CheckNumber.TabIndex = 79;
            this.Txt_CheckNumber.Text = "0.00";
            // 
            // Txt_CurrencyName
            // 
            this.Txt_CurrencyName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_CurrencyName.Location = new System.Drawing.Point(14, 585);
            this.Txt_CurrencyName.MaxLength = 15;
            this.Txt_CurrencyName.Name = "Txt_CurrencyName";
            this.Txt_CurrencyName.Size = new System.Drawing.Size(351, 27);
            this.Txt_CurrencyName.TabIndex = 80;
            // 
            // Lbl_CurrencyName
            // 
            this.Lbl_CurrencyName.AutoSize = true;
            this.Lbl_CurrencyName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_CurrencyName.ForeColor = System.Drawing.Color.Black;
            this.Lbl_CurrencyName.Location = new System.Drawing.Point(10, 562);
            this.Lbl_CurrencyName.Name = "Lbl_CurrencyName";
            this.Lbl_CurrencyName.Size = new System.Drawing.Size(148, 20);
            this.Lbl_CurrencyName.TabIndex = 81;
            this.Lbl_CurrencyName.Text = "NOMBRE CUENTA *";
            // 
            // Frm_Accounts_Managment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.Panel_Derecho);
            this.Controls.Add(this.Panel_Izquierdo);
            this.Controls.Add(this.Panel_Busqueda);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_Accounts_Managment";
            this.Text = "SECRON - GESTIÓN DE CUENTAS CONTABLES";
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Busqueda.ResumeLayout(false);
            this.Panel_Busqueda.PerformLayout();
            this.Panel_Derecho.ResumeLayout(false);
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.Panel_Izquierdo.ResumeLayout(false);
            this.Panel_Informacion.ResumeLayout(false);
            this.Panel_Informacion.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Button Btn_Import;
        private System.Windows.Forms.Button Btn_Export;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_Busqueda;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Inactive;
        private System.Windows.Forms.Button Btn_Update;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Panel Panel_Derecho;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel Panel_Izquierdo;
        private System.Windows.Forms.ComboBox ComboBox_Type;
        private System.Windows.Forms.TextBox Txt_Balance;
        private System.Windows.Forms.Label Lbl_Type;
        private System.Windows.Forms.Label Lbl_Balance;
        private System.Windows.Forms.Panel Panel_Informacion;
        private System.Windows.Forms.TextBox Txt_Name;
        private System.Windows.Forms.TextBox Txt_ParentAccountCode;
        private System.Windows.Forms.TextBox Txt_Code;
        private System.Windows.Forms.Label Lbl_Name;
        private System.Windows.Forms.Label Lbl_ParentAccountCode;
        private System.Windows.Forms.Label Lbl_Code;
        private System.Windows.Forms.Label Lbl_Subtitulo1;
        private System.Windows.Forms.ComboBox ComboBox_Sign;
        private System.Windows.Forms.Label Lbl_Sign;
        private System.Windows.Forms.ComboBox ComboBox_Level;
        private System.Windows.Forms.Label Lbl_Level;
        private System.Windows.Forms.ComboBox ComboBox_BankName;
        private System.Windows.Forms.Label Lbl_BankName;
        private System.Windows.Forms.ComboBox ComboBox_BankAccountType;
        private System.Windows.Forms.Label Lbl_BankAccountType;
        private System.Windows.Forms.ComboBox ComboBox_Currency;
        private System.Windows.Forms.Label Lbl_Currency;
        private System.Windows.Forms.Label Lbl_CheckNumber;
        private System.Windows.Forms.TextBox Txt_CheckNumber;
        private System.Windows.Forms.TextBox Txt_CurrencyName;
        private System.Windows.Forms.Label Lbl_CurrencyName;
    }
}