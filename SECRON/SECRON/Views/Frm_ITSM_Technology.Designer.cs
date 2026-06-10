namespace SECRON.Views
{
    partial class Frm_ITSM_Technology
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ITSM_Technology));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Btn_Export = new System.Windows.Forms.Button();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_Derecho = new System.Windows.Forms.Panel();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.PanelToolStrip = new System.Windows.Forms.Panel();
            this.Lbl_Paginas = new System.Windows.Forms.Label();
            this.Panel_Busqueda = new System.Windows.Forms.Panel();
            this.Btn_CleanSearch = new System.Windows.Forms.Button();
            this.Filtro3 = new System.Windows.Forms.ComboBox();
            this.Filtro2 = new System.Windows.Forms.ComboBox();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Filtro1 = new System.Windows.Forms.ComboBox();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Panel_Izquierdo = new System.Windows.Forms.Panel();
            this.Panel_2 = new System.Windows.Forms.Panel();
            this.Btn_Assign = new System.Windows.Forms.Button();
            this.Lbl_TipoDestino = new System.Windows.Forms.Label();
            this.ComboBox_TipoDestino = new System.Windows.Forms.ComboBox();
            this.Lbl_Location = new System.Windows.Forms.Label();
            this.ComboBox_Location = new System.Windows.Forms.ComboBox();
            this.Lbl_ToLocation = new System.Windows.Forms.Label();
            this.ComboBox_ToLocation = new System.Windows.Forms.ComboBox();
            this.Lbl_ToEmployee = new System.Windows.Forms.Label();
            this.ComboBox_ToEmployee = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Btn_Update = new System.Windows.Forms.Button();
            this.Lbl_Subtitulo3 = new System.Windows.Forms.Label();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.Panel_Atributos = new System.Windows.Forms.Panel();
            this.Panel_1 = new System.Windows.Forms.Panel();
            this.Btn_PrintLetter = new System.Windows.Forms.Button();
            this.ComboBox_Categories = new System.Windows.Forms.ComboBox();
            this.Lbl_Subtitulo2 = new System.Windows.Forms.Label();
            this.Panel_Superior.SuspendLayout();
            this.Panel_Derecho.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.PanelToolStrip.SuspendLayout();
            this.Panel_Busqueda.SuspendLayout();
            this.Panel_Izquierdo.SuspendLayout();
            this.Panel_2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.Panel_1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.Panel_Superior.Controls.Add(this.Btn_Export);
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1184, 55);
            this.Panel_Superior.TabIndex = 4;
            // 
            // Btn_Export
            // 
            this.Btn_Export.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Export.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Export.Image = global::SECRON.Properties.Resources.ExportarExcelNegro25x25;
            this.Btn_Export.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Export.Location = new System.Drawing.Point(1043, 13);
            this.Btn_Export.Name = "Btn_Export";
            this.Btn_Export.Size = new System.Drawing.Size(119, 30);
            this.Btn_Export.TabIndex = 52;
            this.Btn_Export.Text = "EXPORTAR";
            this.Btn_Export.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Export.UseVisualStyleBackColor = true;
            this.Btn_Export.Click += new System.EventHandler(this.Btn_Export_Click);
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(361, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "GESTIÓN DE EQUIPOS DE TECNOLOGÍA";
            // 
            // Panel_Derecho
            // 
            this.Panel_Derecho.Controls.Add(this.PanelTabla);
            this.Panel_Derecho.Controls.Add(this.PanelToolStrip);
            this.Panel_Derecho.Controls.Add(this.Panel_Busqueda);
            this.Panel_Derecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Derecho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Derecho.ForeColor = System.Drawing.Color.Black;
            this.Panel_Derecho.Location = new System.Drawing.Point(477, 55);
            this.Panel_Derecho.Name = "Panel_Derecho";
            this.Panel_Derecho.Size = new System.Drawing.Size(707, 806);
            this.Panel_Derecho.TabIndex = 9;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(22, 152);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(663, 635);
            this.PanelTabla.TabIndex = 75;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(663, 635);
            this.Tabla.TabIndex = 1;
            this.Tabla.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tabla_CellClick);
            // 
            // PanelToolStrip
            // 
            this.PanelToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelToolStrip.Controls.Add(this.Lbl_Paginas);
            this.PanelToolStrip.Location = new System.Drawing.Point(22, 107);
            this.PanelToolStrip.Name = "PanelToolStrip";
            this.PanelToolStrip.Size = new System.Drawing.Size(663, 39);
            this.PanelToolStrip.TabIndex = 74;
            // 
            // Lbl_Paginas
            // 
            this.Lbl_Paginas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Paginas.AutoSize = true;
            this.Lbl_Paginas.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Paginas.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Paginas.Location = new System.Drawing.Point(12, 11);
            this.Lbl_Paginas.Name = "Lbl_Paginas";
            this.Lbl_Paginas.Size = new System.Drawing.Size(264, 20);
            this.Lbl_Paginas.TabIndex = 51;
            this.Lbl_Paginas.Text = "MOSTRANDO 1-10 DE 100 EQUIPOS";
            // 
            // Panel_Busqueda
            // 
            this.Panel_Busqueda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Busqueda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Busqueda.Controls.Add(this.Btn_CleanSearch);
            this.Panel_Busqueda.Controls.Add(this.Filtro3);
            this.Panel_Busqueda.Controls.Add(this.Filtro2);
            this.Panel_Busqueda.Controls.Add(this.Btn_Search);
            this.Panel_Busqueda.Controls.Add(this.Filtro1);
            this.Panel_Busqueda.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_Busqueda.Location = new System.Drawing.Point(22, 13);
            this.Panel_Busqueda.Name = "Panel_Busqueda";
            this.Panel_Busqueda.Size = new System.Drawing.Size(663, 88);
            this.Panel_Busqueda.TabIndex = 73;
            // 
            // Btn_CleanSearch
            // 
            this.Btn_CleanSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_CleanSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_CleanSearch.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_CleanSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_CleanSearch.Location = new System.Drawing.Point(620, 7);
            this.Btn_CleanSearch.Name = "Btn_CleanSearch";
            this.Btn_CleanSearch.Size = new System.Drawing.Size(30, 31);
            this.Btn_CleanSearch.TabIndex = 71;
            this.Btn_CleanSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_CleanSearch.UseVisualStyleBackColor = true;
            this.Btn_CleanSearch.Click += new System.EventHandler(this.Btn_CleanSearch_Click);
            // 
            // Filtro3
            // 
            this.Filtro3.FormattingEnabled = true;
            this.Filtro3.Location = new System.Drawing.Point(443, 43);
            this.Filtro3.Name = "Filtro3";
            this.Filtro3.Size = new System.Drawing.Size(207, 28);
            this.Filtro3.TabIndex = 70;
            // 
            // Filtro2
            // 
            this.Filtro2.FormattingEnabled = true;
            this.Filtro2.Location = new System.Drawing.Point(229, 43);
            this.Filtro2.Name = "Filtro2";
            this.Filtro2.Size = new System.Drawing.Size(207, 28);
            this.Filtro2.TabIndex = 69;
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Search.Location = new System.Drawing.Point(513, 7);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(101, 31);
            this.Btn_Search.TabIndex = 54;
            this.Btn_Search.Text = "BUSCAR";
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Filtro1
            // 
            this.Filtro1.FormattingEnabled = true;
            this.Filtro1.Location = new System.Drawing.Point(16, 45);
            this.Filtro1.Name = "Filtro1";
            this.Filtro1.Size = new System.Drawing.Size(207, 28);
            this.Filtro1.TabIndex = 68;
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(16, 10);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(491, 27);
            this.Txt_ValorBuscado.TabIndex = 59;
            // 
            // Panel_Izquierdo
            // 
            this.Panel_Izquierdo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Izquierdo.Controls.Add(this.Panel_2);
            this.Panel_Izquierdo.Controls.Add(this.panel1);
            this.Panel_Izquierdo.Controls.Add(this.vScrollBar);
            this.Panel_Izquierdo.Controls.Add(this.Panel_Atributos);
            this.Panel_Izquierdo.Controls.Add(this.Panel_1);
            this.Panel_Izquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.Panel_Izquierdo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Izquierdo.ForeColor = System.Drawing.Color.Black;
            this.Panel_Izquierdo.Location = new System.Drawing.Point(0, 55);
            this.Panel_Izquierdo.MaximumSize = new System.Drawing.Size(477, 806);
            this.Panel_Izquierdo.Name = "Panel_Izquierdo";
            this.Panel_Izquierdo.Size = new System.Drawing.Size(477, 806);
            this.Panel_Izquierdo.TabIndex = 8;
            // 
            // Panel_2
            // 
            this.Panel_2.BackColor = System.Drawing.Color.White;
            this.Panel_2.Controls.Add(this.Btn_Assign);
            this.Panel_2.Controls.Add(this.Lbl_TipoDestino);
            this.Panel_2.Controls.Add(this.ComboBox_TipoDestino);
            this.Panel_2.Controls.Add(this.Lbl_Location);
            this.Panel_2.Controls.Add(this.ComboBox_Location);
            this.Panel_2.Controls.Add(this.Lbl_ToLocation);
            this.Panel_2.Controls.Add(this.ComboBox_ToLocation);
            this.Panel_2.Controls.Add(this.Lbl_ToEmployee);
            this.Panel_2.Controls.Add(this.ComboBox_ToEmployee);
            this.Panel_2.Controls.Add(this.label1);
            this.Panel_2.Location = new System.Drawing.Point(13, 167);
            this.Panel_2.Name = "Panel_2";
            this.Panel_2.Size = new System.Drawing.Size(435, 278);
            this.Panel_2.TabIndex = 82;
            // 
            // Btn_Assign
            // 
            this.Btn_Assign.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Assign.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Assign.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Assign.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Assign.Location = new System.Drawing.Point(313, 7);
            this.Btn_Assign.Name = "Btn_Assign";
            this.Btn_Assign.Size = new System.Drawing.Size(108, 34);
            this.Btn_Assign.TabIndex = 77;
            this.Btn_Assign.Text = "ASIGNAR";
            this.Btn_Assign.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Assign.UseVisualStyleBackColor = true;
            this.Btn_Assign.Click += new System.EventHandler(this.Btn_Assign_Click);
            // 
            // Lbl_TipoDestino
            // 
            this.Lbl_TipoDestino.AutoSize = true;
            this.Lbl_TipoDestino.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_TipoDestino.Location = new System.Drawing.Point(10, 94);
            this.Lbl_TipoDestino.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Lbl_TipoDestino.Name = "Lbl_TipoDestino";
            this.Lbl_TipoDestino.Size = new System.Drawing.Size(258, 20);
            this.Lbl_TipoDestino.TabIndex = 76;
            this.Lbl_TipoDestino.Text = "SELECCIONA EL TIPO DE DESTINO *";
            // 
            // ComboBox_TipoDestino
            // 
            this.ComboBox_TipoDestino.FormattingEnabled = true;
            this.ComboBox_TipoDestino.Location = new System.Drawing.Point(14, 117);
            this.ComboBox_TipoDestino.Name = "ComboBox_TipoDestino";
            this.ComboBox_TipoDestino.Size = new System.Drawing.Size(407, 28);
            this.ComboBox_TipoDestino.TabIndex = 75;
            // 
            // Lbl_Location
            // 
            this.Lbl_Location.AutoSize = true;
            this.Lbl_Location.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Location.Location = new System.Drawing.Point(10, 40);
            this.Lbl_Location.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Lbl_Location.Name = "Lbl_Location";
            this.Lbl_Location.Size = new System.Drawing.Size(216, 20);
            this.Lbl_Location.TabIndex = 74;
            this.Lbl_Location.Text = "SELECCIONA SEDE DESTINO *";
            // 
            // ComboBox_Location
            // 
            this.ComboBox_Location.FormattingEnabled = true;
            this.ComboBox_Location.Location = new System.Drawing.Point(14, 63);
            this.ComboBox_Location.Name = "ComboBox_Location";
            this.ComboBox_Location.Size = new System.Drawing.Size(407, 28);
            this.ComboBox_Location.TabIndex = 73;
            // 
            // Lbl_ToLocation
            // 
            this.Lbl_ToLocation.AutoSize = true;
            this.Lbl_ToLocation.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ToLocation.Location = new System.Drawing.Point(10, 210);
            this.Lbl_ToLocation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Lbl_ToLocation.Name = "Lbl_ToLocation";
            this.Lbl_ToLocation.Size = new System.Drawing.Size(123, 20);
            this.Lbl_ToLocation.TabIndex = 72;
            this.Lbl_ToLocation.Text = "SEDE DESTINO *";
            // 
            // ComboBox_ToLocation
            // 
            this.ComboBox_ToLocation.FormattingEnabled = true;
            this.ComboBox_ToLocation.Location = new System.Drawing.Point(14, 233);
            this.ComboBox_ToLocation.Name = "ComboBox_ToLocation";
            this.ComboBox_ToLocation.Size = new System.Drawing.Size(407, 28);
            this.ComboBox_ToLocation.TabIndex = 71;
            // 
            // Lbl_ToEmployee
            // 
            this.Lbl_ToEmployee.AutoSize = true;
            this.Lbl_ToEmployee.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_ToEmployee.Location = new System.Drawing.Point(10, 150);
            this.Lbl_ToEmployee.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Lbl_ToEmployee.Name = "Lbl_ToEmployee";
            this.Lbl_ToEmployee.Size = new System.Drawing.Size(103, 20);
            this.Lbl_ToEmployee.TabIndex = 70;
            this.Lbl_ToEmployee.Text = "ASIGNAR A *";
            // 
            // ComboBox_ToEmployee
            // 
            this.ComboBox_ToEmployee.FormattingEnabled = true;
            this.ComboBox_ToEmployee.Location = new System.Drawing.Point(14, 173);
            this.ComboBox_ToEmployee.Name = "ComboBox_ToEmployee";
            this.ComboBox_ToEmployee.Size = new System.Drawing.Size(407, 28);
            this.ComboBox_ToEmployee.TabIndex = 69;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label1.Image = global::SECRON.Properties.Resources.UsersNegro25x25;
            this.label1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label1.Location = new System.Drawing.Point(10, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(298, 20);
            this.label1.TabIndex = 14;
            this.label1.Text = "      ASIGNACIÓN INMEDIATA DE EQUIPO";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.Btn_Update);
            this.panel1.Controls.Add(this.Lbl_Subtitulo3);
            this.panel1.Location = new System.Drawing.Point(13, 451);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(435, 47);
            this.panel1.TabIndex = 81;
            // 
            // Btn_Update
            // 
            this.Btn_Update.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Update.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Update.Image = global::SECRON.Properties.Resources.UpdateAzul25x25;
            this.Btn_Update.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Update.Location = new System.Drawing.Point(281, 5);
            this.Btn_Update.Name = "Btn_Update";
            this.Btn_Update.Size = new System.Drawing.Size(140, 34);
            this.Btn_Update.TabIndex = 55;
            this.Btn_Update.Text = "ACTUALIZAR";
            this.Btn_Update.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Update.UseVisualStyleBackColor = true;
            this.Btn_Update.Click += new System.EventHandler(this.Btn_Update_Click);
            // 
            // Lbl_Subtitulo3
            // 
            this.Lbl_Subtitulo3.AutoSize = true;
            this.Lbl_Subtitulo3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo3.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo3.Image = global::SECRON.Properties.Resources.InfoNegro20x20;
            this.Lbl_Subtitulo3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo3.Location = new System.Drawing.Point(10, 10);
            this.Lbl_Subtitulo3.Name = "Lbl_Subtitulo3";
            this.Lbl_Subtitulo3.Size = new System.Drawing.Size(193, 20);
            this.Lbl_Subtitulo3.TabIndex = 6;
            this.Lbl_Subtitulo3.Text = "      DETALLES DEL EQUIPO";
            this.Lbl_Subtitulo3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(467, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(10, 806);
            this.vScrollBar.TabIndex = 80;
            // 
            // Panel_Atributos
            // 
            this.Panel_Atributos.BackColor = System.Drawing.Color.White;
            this.Panel_Atributos.Location = new System.Drawing.Point(13, 504);
            this.Panel_Atributos.Name = "Panel_Atributos";
            this.Panel_Atributos.Size = new System.Drawing.Size(435, 283);
            this.Panel_Atributos.TabIndex = 65;
            // 
            // Panel_1
            // 
            this.Panel_1.BackColor = System.Drawing.Color.White;
            this.Panel_1.Controls.Add(this.Btn_PrintLetter);
            this.Panel_1.Controls.Add(this.ComboBox_Categories);
            this.Panel_1.Controls.Add(this.Lbl_Subtitulo2);
            this.Panel_1.Location = new System.Drawing.Point(13, 13);
            this.Panel_1.Name = "Panel_1";
            this.Panel_1.Size = new System.Drawing.Size(435, 148);
            this.Panel_1.TabIndex = 64;
            // 
            // Btn_PrintLetter
            // 
            this.Btn_PrintLetter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_PrintLetter.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_PrintLetter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_PrintLetter.Location = new System.Drawing.Point(14, 94);
            this.Btn_PrintLetter.Name = "Btn_PrintLetter";
            this.Btn_PrintLetter.Size = new System.Drawing.Size(407, 30);
            this.Btn_PrintLetter.TabIndex = 79;
            this.Btn_PrintLetter.Text = "GENERAR CARTA DE RESPONSABILIDAD";
            this.Btn_PrintLetter.UseVisualStyleBackColor = true;
            this.Btn_PrintLetter.Click += new System.EventHandler(this.Btn_PrintLetter_Click);
            // 
            // ComboBox_Categories
            // 
            this.ComboBox_Categories.FormattingEnabled = true;
            this.ComboBox_Categories.Location = new System.Drawing.Point(14, 30);
            this.ComboBox_Categories.Name = "ComboBox_Categories";
            this.ComboBox_Categories.Size = new System.Drawing.Size(407, 28);
            this.ComboBox_Categories.TabIndex = 69;
            // 
            // Lbl_Subtitulo2
            // 
            this.Lbl_Subtitulo2.AutoSize = true;
            this.Lbl_Subtitulo2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo2.Image = global::SECRON.Properties.Resources.CategoriesBlack25x25;
            this.Lbl_Subtitulo2.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.Lbl_Subtitulo2.Location = new System.Drawing.Point(10, 7);
            this.Lbl_Subtitulo2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Lbl_Subtitulo2.Name = "Lbl_Subtitulo2";
            this.Lbl_Subtitulo2.Size = new System.Drawing.Size(207, 20);
            this.Lbl_Subtitulo2.TabIndex = 14;
            this.Lbl_Subtitulo2.Text = "      CATEGORÍA DEL EQUIPO";
            // 
            // Frm_ITSM_Technology
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.Panel_Derecho);
            this.Controls.Add(this.Panel_Izquierdo);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_ITSM_Technology";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SECRON - ITSM GESTIÓN EQUIPOS DE TECNOLOGÍA";
            this.Load += new System.EventHandler(this.Frm_ITSM_Technology_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Derecho.ResumeLayout(false);
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.PanelToolStrip.ResumeLayout(false);
            this.PanelToolStrip.PerformLayout();
            this.Panel_Busqueda.ResumeLayout(false);
            this.Panel_Busqueda.PerformLayout();
            this.Panel_Izquierdo.ResumeLayout(false);
            this.Panel_2.ResumeLayout(false);
            this.Panel_2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.Panel_1.ResumeLayout(false);
            this.Panel_1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Button Btn_Export;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_Derecho;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel PanelToolStrip;
        private System.Windows.Forms.Label Lbl_Paginas;
        private System.Windows.Forms.Panel Panel_Busqueda;
        private System.Windows.Forms.Button Btn_CleanSearch;
        private System.Windows.Forms.ComboBox Filtro3;
        private System.Windows.Forms.ComboBox Filtro2;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.ComboBox Filtro1;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Panel Panel_Izquierdo;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.Panel Panel_1;
        private System.Windows.Forms.Label Lbl_Subtitulo3;
        private System.Windows.Forms.Panel Panel_Atributos;
        private System.Windows.Forms.Label Lbl_Subtitulo2;
        private System.Windows.Forms.ComboBox ComboBox_Categories;
        private System.Windows.Forms.Button Btn_PrintLetter;
        private System.Windows.Forms.Button Btn_Update;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel Panel_2;
        private System.Windows.Forms.ComboBox ComboBox_ToEmployee;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Lbl_ToLocation;
        private System.Windows.Forms.ComboBox ComboBox_ToLocation;
        private System.Windows.Forms.Label Lbl_ToEmployee;
        private System.Windows.Forms.Label Lbl_Location;
        private System.Windows.Forms.ComboBox ComboBox_Location;
        private System.Windows.Forms.Label Lbl_TipoDestino;
        private System.Windows.Forms.ComboBox ComboBox_TipoDestino;
        private System.Windows.Forms.Button Btn_Assign;
    }
}