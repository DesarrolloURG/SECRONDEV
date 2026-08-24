namespace SECRON.Views
{
    partial class Frm_AcademicProcesses_CoursesPricing
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_AcademicProcesses_CoursesPricing));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Btn_Export = new System.Windows.Forms.Button();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_Izquierdo = new System.Windows.Forms.Panel();
            this.Lbl_Titulo = new System.Windows.Forms.Label();
            this.Panel_CRUD = new System.Windows.Forms.Panel();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Btn_IsActive = new System.Windows.Forms.Button();
            this.Btn_Update = new System.Windows.Forms.Button();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Panel_1 = new System.Windows.Forms.Panel();
            this.Cbx_Pensum = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Cbx_Modalidad = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Cbx_Carrera = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Cbx_Sede = new System.Windows.Forms.ComboBox();
            this.Lbl_Sub1 = new System.Windows.Forms.Label();
            this.Panel_Busqueda = new System.Windows.Forms.Panel();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Btn_CleanSearch = new System.Windows.Forms.Button();
            this.FiltroSede = new System.Windows.Forms.ComboBox();
            this.FiltroCarrera = new System.Windows.Forms.ComboBox();
            this.PanelToolStrip = new System.Windows.Forms.Panel();
            this.Lbl_Paginas = new System.Windows.Forms.Label();
            this.Panel_BusquedaAtributos = new System.Windows.Forms.Panel();
            this.Txt_BuscarAtributo = new System.Windows.Forms.TextBox();
            this.FiltroAtributoTipo = new System.Windows.Forms.ComboBox();
            this.Btn_SearchAtributo = new System.Windows.Forms.Button();
            this.Btn_CleanSearchAtributo = new System.Windows.Forms.Button();
            this.Lbl_AtributosHeader = new System.Windows.Forms.Label();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TablaAtributos = new System.Windows.Forms.DataGridView();
            this.Panel_Derecho = new System.Windows.Forms.Panel();
            this.Panel_Superior.SuspendLayout();
            this.Panel_Izquierdo.SuspendLayout();
            this.Panel_CRUD.SuspendLayout();
            this.Panel_1.SuspendLayout();
            this.Panel_Busqueda.SuspendLayout();
            this.PanelToolStrip.SuspendLayout();
            this.Panel_BusquedaAtributos.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TablaAtributos)).BeginInit();
            this.Panel_Derecho.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.Panel_Superior.Controls.Add(this.Btn_Export);
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1579, 68);
            this.Panel_Superior.TabIndex = 6;
            // 
            // Btn_Export
            // 
            this.Btn_Export.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Export.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Export.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Export.Location = new System.Drawing.Point(1407, 16);
            this.Btn_Export.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_Export.Name = "Btn_Export";
            this.Btn_Export.Size = new System.Drawing.Size(155, 37);
            this.Btn_Export.TabIndex = 0;
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
            this.Lbl_Formulario.Location = new System.Drawing.Point(11, 16);
            this.Lbl_Formulario.Name = "Lbl_Formulario";
            this.Lbl_Formulario.Size = new System.Drawing.Size(426, 25);
            this.Lbl_Formulario.TabIndex = 1;
            this.Lbl_Formulario.Text = "CATÁLOGO DE PRECIOS POR CURSO POR SEDE";
            // 
            // Panel_Izquierdo
            // 
            this.Panel_Izquierdo.AutoScroll = true;
            this.Panel_Izquierdo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Izquierdo.Controls.Add(this.Lbl_Titulo);
            this.Panel_Izquierdo.Controls.Add(this.Panel_CRUD);
            this.Panel_Izquierdo.Controls.Add(this.Panel_1);
            this.Panel_Izquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.Panel_Izquierdo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Izquierdo.Location = new System.Drawing.Point(0, 68);
            this.Panel_Izquierdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_Izquierdo.Name = "Panel_Izquierdo";
            this.Panel_Izquierdo.Size = new System.Drawing.Size(581, 987);
            this.Panel_Izquierdo.TabIndex = 7;
            // 
            // Lbl_Titulo
            // 
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Titulo.Location = new System.Drawing.Point(4, 12);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(201, 20);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "INFORMACIÓN DEL CURSO";
            // 
            // Panel_CRUD
            // 
            this.Panel_CRUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_CRUD.Controls.Add(this.Btn_Save);
            this.Panel_CRUD.Controls.Add(this.Btn_IsActive);
            this.Panel_CRUD.Controls.Add(this.Btn_Update);
            this.Panel_CRUD.Controls.Add(this.Btn_Clear);
            this.Panel_CRUD.Location = new System.Drawing.Point(4, 39);
            this.Panel_CRUD.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_CRUD.Name = "Panel_CRUD";
            this.Panel_CRUD.Size = new System.Drawing.Size(571, 58);
            this.Panel_CRUD.TabIndex = 79;
            // 
            // Btn_Save
            // 
            this.Btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Save.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Save.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Save.Location = new System.Drawing.Point(18, 6);
            this.Btn_Save.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(140, 46);
            this.Btn_Save.TabIndex = 79;
            this.Btn_Save.Text = "GUARDAR";
            this.Btn_Save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Btn_IsActive
            // 
            this.Btn_IsActive.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_IsActive.Image = global::SECRON.Properties.Resources.Alerta25x25;
            this.Btn_IsActive.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_IsActive.Location = new System.Drawing.Point(304, 6);
            this.Btn_IsActive.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_IsActive.Name = "Btn_IsActive";
            this.Btn_IsActive.Size = new System.Drawing.Size(211, 46);
            this.Btn_IsActive.TabIndex = 78;
            this.Btn_IsActive.Text = "ACTIV/INAC";
            this.Btn_IsActive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_IsActive.UseVisualStyleBackColor = true;
            // 
            // Btn_Update
            // 
            this.Btn_Update.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Update.Image = global::SECRON.Properties.Resources.UpdateAzul25x25;
            this.Btn_Update.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Update.Location = new System.Drawing.Point(165, 6);
            this.Btn_Update.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_Update.Name = "Btn_Update";
            this.Btn_Update.Size = new System.Drawing.Size(132, 46);
            this.Btn_Update.TabIndex = 1;
            this.Btn_Update.Text = "EDITAR";
            this.Btn_Update.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Update.UseVisualStyleBackColor = true;
            this.Btn_Update.Click += new System.EventHandler(this.Btn_Update_Click);
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.Location = new System.Drawing.Point(519, 6);
            this.Btn_Clear.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(44, 46);
            this.Btn_Clear.TabIndex = 3;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            this.Btn_Clear.Click += new System.EventHandler(this.Btn_Clear_Click);
            // 
            // Panel_1
            // 
            this.Panel_1.BackColor = System.Drawing.Color.White;
            this.Panel_1.Controls.Add(this.Cbx_Pensum);
            this.Panel_1.Controls.Add(this.label4);
            this.Panel_1.Controls.Add(this.Cbx_Modalidad);
            this.Panel_1.Controls.Add(this.label2);
            this.Panel_1.Controls.Add(this.label1);
            this.Panel_1.Controls.Add(this.Cbx_Carrera);
            this.Panel_1.Controls.Add(this.label3);
            this.Panel_1.Controls.Add(this.Cbx_Sede);
            this.Panel_1.Controls.Add(this.Lbl_Sub1);
            this.Panel_1.Location = new System.Drawing.Point(4, 105);
            this.Panel_1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_1.Name = "Panel_1";
            this.Panel_1.Size = new System.Drawing.Size(571, 341);
            this.Panel_1.TabIndex = 52;
            // 
            // Cbx_Pensum
            // 
            this.Cbx_Pensum.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Cbx_Pensum.Location = new System.Drawing.Point(21, 206);
            this.Cbx_Pensum.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Cbx_Pensum.Name = "Cbx_Pensum";
            this.Cbx_Pensum.Size = new System.Drawing.Size(525, 28);
            this.Cbx_Pensum.TabIndex = 18;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(15, 252);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 20);
            this.label4.TabIndex = 16;
            this.label4.Text = "MODALIDAD *";
            // 
            // Cbx_Modalidad
            // 
            this.Cbx_Modalidad.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Cbx_Modalidad.Location = new System.Drawing.Point(21, 283);
            this.Cbx_Modalidad.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Cbx_Modalidad.Name = "Cbx_Modalidad";
            this.Cbx_Modalidad.Size = new System.Drawing.Size(525, 28);
            this.Cbx_Modalidad.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(15, 175);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 20);
            this.label2.TabIndex = 14;
            this.label2.Text = "PENSUM *";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(15, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 12;
            this.label1.Text = "CARRERA *";
            // 
            // Cbx_Carrera
            // 
            this.Cbx_Carrera.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Cbx_Carrera.Location = new System.Drawing.Point(21, 132);
            this.Cbx_Carrera.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Cbx_Carrera.Name = "Cbx_Carrera";
            this.Cbx_Carrera.Size = new System.Drawing.Size(525, 28);
            this.Cbx_Carrera.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(15, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "SEDE *";
            // 
            // Cbx_Sede
            // 
            this.Cbx_Sede.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Cbx_Sede.Location = new System.Drawing.Point(21, 61);
            this.Cbx_Sede.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Cbx_Sede.Name = "Cbx_Sede";
            this.Cbx_Sede.Size = new System.Drawing.Size(525, 28);
            this.Cbx_Sede.TabIndex = 11;
            // 
            // Lbl_Sub1
            // 
            this.Lbl_Sub1.AutoSize = true;
            this.Lbl_Sub1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Sub1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Sub1.Location = new System.Drawing.Point(13, 3);
            this.Lbl_Sub1.Name = "Lbl_Sub1";
            this.Lbl_Sub1.Size = new System.Drawing.Size(165, 20);
            this.Lbl_Sub1.TabIndex = 0;
            this.Lbl_Sub1.Text = "      DATOS DEL CURSO";
            // 
            // Panel_Busqueda
            // 
            this.Panel_Busqueda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Busqueda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Busqueda.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_Busqueda.Controls.Add(this.Btn_Search);
            this.Panel_Busqueda.Controls.Add(this.Btn_CleanSearch);
            this.Panel_Busqueda.Controls.Add(this.FiltroSede);
            this.Panel_Busqueda.Controls.Add(this.FiltroCarrera);
            this.Panel_Busqueda.Location = new System.Drawing.Point(16, 7);
            this.Panel_Busqueda.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_Busqueda.Name = "Panel_Busqueda";
            this.Panel_Busqueda.Size = new System.Drawing.Size(725, 110);
            this.Panel_Busqueda.TabIndex = 2;
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(21, 18);
            this.Txt_ValorBuscado.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(477, 27);
            this.Txt_ValorBuscado.TabIndex = 0;
            this.Txt_ValorBuscado.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado_KeyDown);
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Search.Location = new System.Drawing.Point(514, 11);
            this.Btn_Search.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(135, 38);
            this.Btn_Search.TabIndex = 1;
            this.Btn_Search.Text = "BUSCAR";
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Btn_CleanSearch
            // 
            this.Btn_CleanSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_CleanSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_CleanSearch.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_CleanSearch.Location = new System.Drawing.Point(657, 11);
            this.Btn_CleanSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_CleanSearch.Name = "Btn_CleanSearch";
            this.Btn_CleanSearch.Size = new System.Drawing.Size(40, 38);
            this.Btn_CleanSearch.TabIndex = 2;
            this.Btn_CleanSearch.UseVisualStyleBackColor = true;
            this.Btn_CleanSearch.Click += new System.EventHandler(this.Btn_CleanSearch_Click);
            // 
            // FiltroSede
            // 
            this.FiltroSede.FormattingEnabled = true;
            this.FiltroSede.Location = new System.Drawing.Point(21, 70);
            this.FiltroSede.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FiltroSede.Name = "FiltroSede";
            this.FiltroSede.Size = new System.Drawing.Size(203, 28);
            this.FiltroSede.TabIndex = 3;
            // 
            // FiltroCarrera
            // 
            this.FiltroCarrera.FormattingEnabled = true;
            this.FiltroCarrera.Location = new System.Drawing.Point(259, 70);
            this.FiltroCarrera.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FiltroCarrera.Name = "FiltroCarrera";
            this.FiltroCarrera.Size = new System.Drawing.Size(203, 28);
            this.FiltroCarrera.TabIndex = 4;
            // 
            // PanelToolStrip
            // 
            this.PanelToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelToolStrip.Controls.Add(this.Lbl_Paginas);
            this.PanelToolStrip.Location = new System.Drawing.Point(16, 118);
            this.PanelToolStrip.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PanelToolStrip.Name = "PanelToolStrip";
            this.PanelToolStrip.Size = new System.Drawing.Size(725, 39);
            this.PanelToolStrip.TabIndex = 1;
            // 
            // Lbl_Paginas
            // 
            this.Lbl_Paginas.AutoSize = true;
            this.Lbl_Paginas.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Paginas.Location = new System.Drawing.Point(16, 10);
            this.Lbl_Paginas.Name = "Lbl_Paginas";
            this.Lbl_Paginas.Size = new System.Drawing.Size(255, 20);
            this.Lbl_Paginas.TabIndex = 0;
            this.Lbl_Paginas.Text = "MOSTRANDO 1-100 DE 0 ACTIVOS";
            // 
            // Panel_BusquedaAtributos
            // 
            this.Panel_BusquedaAtributos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_BusquedaAtributos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_BusquedaAtributos.Controls.Add(this.Txt_BuscarAtributo);
            this.Panel_BusquedaAtributos.Controls.Add(this.FiltroAtributoTipo);
            this.Panel_BusquedaAtributos.Controls.Add(this.Btn_SearchAtributo);
            this.Panel_BusquedaAtributos.Controls.Add(this.Btn_CleanSearchAtributo);
            this.Panel_BusquedaAtributos.Location = new System.Drawing.Point(16, 447);
            this.Panel_BusquedaAtributos.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_BusquedaAtributos.Name = "Panel_BusquedaAtributos";
            this.Panel_BusquedaAtributos.Size = new System.Drawing.Size(725, 90);
            this.Panel_BusquedaAtributos.TabIndex = 5;
            // 
            // Txt_BuscarAtributo
            // 
            this.Txt_BuscarAtributo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_BuscarAtributo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_BuscarAtributo.Location = new System.Drawing.Point(4, 9);
            this.Txt_BuscarAtributo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_BuscarAtributo.Name = "Txt_BuscarAtributo";
            this.Txt_BuscarAtributo.Size = new System.Drawing.Size(438, 27);
            this.Txt_BuscarAtributo.TabIndex = 0;
            this.Txt_BuscarAtributo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_BuscarAtributo_KeyDown);
            // 
            // FiltroAtributoTipo
            // 
            this.FiltroAtributoTipo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.FiltroAtributoTipo.FormattingEnabled = true;
            this.FiltroAtributoTipo.Location = new System.Drawing.Point(4, 47);
            this.FiltroAtributoTipo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FiltroAtributoTipo.Name = "FiltroAtributoTipo";
            this.FiltroAtributoTipo.Size = new System.Drawing.Size(220, 28);
            this.FiltroAtributoTipo.TabIndex = 1;
            // 
            // Btn_SearchAtributo
            // 
            this.Btn_SearchAtributo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_SearchAtributo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SearchAtributo.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_SearchAtributo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_SearchAtributo.Location = new System.Drawing.Point(449, 9);
            this.Btn_SearchAtributo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_SearchAtributo.Name = "Btn_SearchAtributo";
            this.Btn_SearchAtributo.Size = new System.Drawing.Size(135, 36);
            this.Btn_SearchAtributo.TabIndex = 2;
            this.Btn_SearchAtributo.Text = "BUSCAR";
            this.Btn_SearchAtributo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SearchAtributo.UseVisualStyleBackColor = true;
            this.Btn_SearchAtributo.Click += new System.EventHandler(this.Btn_SearchAtributo_Click);
            // 
            // Btn_CleanSearchAtributo
            // 
            this.Btn_CleanSearchAtributo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_CleanSearchAtributo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_CleanSearchAtributo.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_CleanSearchAtributo.Location = new System.Drawing.Point(592, 9);
            this.Btn_CleanSearchAtributo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_CleanSearchAtributo.Name = "Btn_CleanSearchAtributo";
            this.Btn_CleanSearchAtributo.Size = new System.Drawing.Size(40, 36);
            this.Btn_CleanSearchAtributo.TabIndex = 3;
            this.Btn_CleanSearchAtributo.UseVisualStyleBackColor = true;
            this.Btn_CleanSearchAtributo.Click += new System.EventHandler(this.Btn_CleanSearchAtributo_Click);
            // 
            // Lbl_AtributosHeader
            // 
            this.Lbl_AtributosHeader.BackColor = System.Drawing.Color.White;
            this.Lbl_AtributosHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Lbl_AtributosHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.Lbl_AtributosHeader.Location = new System.Drawing.Point(16, 418);
            this.Lbl_AtributosHeader.Name = "Lbl_AtributosHeader";
            this.Lbl_AtributosHeader.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Lbl_AtributosHeader.Size = new System.Drawing.Size(961, 28);
            this.Lbl_AtributosHeader.TabIndex = 3;
            this.Lbl_AtributosHeader.Text = "HISTORIAL DE PRECIOS - SELECCIONE UN CURSO";
            this.Lbl_AtributosHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(16, 164);
            this.PanelTabla.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(725, 250);
            this.PanelTabla.TabIndex = 0;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Tabla.Name = "Tabla";
            this.Tabla.RowHeadersWidth = 51;
            this.Tabla.RowTemplate.Height = 24;
            this.Tabla.Size = new System.Drawing.Size(725, 250);
            this.Tabla.TabIndex = 0;
            this.Tabla.SelectionChanged += new System.EventHandler(this.Tabla_SelectionChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.TablaAtributos);
            this.panel1.Location = new System.Drawing.Point(16, 542);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(725, 388);
            this.panel1.TabIndex = 6;
            // 
            // TablaAtributos
            // 
            this.TablaAtributos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TablaAtributos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TablaAtributos.Location = new System.Drawing.Point(0, 0);
            this.TablaAtributos.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TablaAtributos.Name = "TablaAtributos";
            this.TablaAtributos.RowHeadersWidth = 51;
            this.TablaAtributos.RowTemplate.Height = 24;
            this.TablaAtributos.Size = new System.Drawing.Size(725, 388);
            this.TablaAtributos.TabIndex = 0;
            // 
            // Panel_Derecho
            // 
            this.Panel_Derecho.AutoScroll = true;
            this.Panel_Derecho.Controls.Add(this.panel1);
            this.Panel_Derecho.Controls.Add(this.PanelTabla);
            this.Panel_Derecho.Controls.Add(this.Lbl_AtributosHeader);
            this.Panel_Derecho.Controls.Add(this.Panel_BusquedaAtributos);
            this.Panel_Derecho.Controls.Add(this.PanelToolStrip);
            this.Panel_Derecho.Controls.Add(this.Panel_Busqueda);
            this.Panel_Derecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Derecho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Derecho.Location = new System.Drawing.Point(581, 68);
            this.Panel_Derecho.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_Derecho.Name = "Panel_Derecho";
            this.Panel_Derecho.Size = new System.Drawing.Size(998, 987);
            this.Panel_Derecho.TabIndex = 0;
            // 
            // Frm_AcademicProcesses_CoursesPricing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1579, 1055);
            this.Controls.Add(this.Panel_Derecho);
            this.Controls.Add(this.Panel_Izquierdo);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Frm_AcademicProcesses_CoursesPricing";
            this.Text = "SECRON - GESTIÓN DE CURSOS Y CARRERAS";
            this.Load += new System.EventHandler(this.Frm_AcademicProcesses_CoursesPricing_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Izquierdo.ResumeLayout(false);
            this.Panel_Izquierdo.PerformLayout();
            this.Panel_CRUD.ResumeLayout(false);
            this.Panel_1.ResumeLayout(false);
            this.Panel_1.PerformLayout();
            this.Panel_Busqueda.ResumeLayout(false);
            this.Panel_Busqueda.PerformLayout();
            this.PanelToolStrip.ResumeLayout(false);
            this.PanelToolStrip.PerformLayout();
            this.Panel_BusquedaAtributos.ResumeLayout(false);
            this.Panel_BusquedaAtributos.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TablaAtributos)).EndInit();
            this.Panel_Derecho.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Button Btn_Export;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_Izquierdo;
        private System.Windows.Forms.Label Lbl_Titulo;
        private System.Windows.Forms.Panel Panel_CRUD;
        private System.Windows.Forms.Button Btn_Update;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Panel Panel_1;
        private System.Windows.Forms.Label Lbl_Sub1;
        private System.Windows.Forms.Panel Panel_Busqueda;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Button Btn_CleanSearch;
        private System.Windows.Forms.ComboBox FiltroSede;
        private System.Windows.Forms.ComboBox FiltroCarrera;
        private System.Windows.Forms.Panel PanelToolStrip;
        private System.Windows.Forms.Label Lbl_Paginas;
        private System.Windows.Forms.Panel Panel_BusquedaAtributos;
        private System.Windows.Forms.TextBox Txt_BuscarAtributo;
        private System.Windows.Forms.ComboBox FiltroAtributoTipo;
        private System.Windows.Forms.Button Btn_SearchAtributo;
        private System.Windows.Forms.Button Btn_CleanSearchAtributo;
        private System.Windows.Forms.Label Lbl_AtributosHeader;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel Panel_Derecho;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox Cbx_Modalidad;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Cbx_Carrera;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox Cbx_Sede;
        private System.Windows.Forms.Button Btn_IsActive;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.DataGridView TablaAtributos;
        private System.Windows.Forms.ComboBox Cbx_Pensum;
    }
}