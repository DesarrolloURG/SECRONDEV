namespace SECRON.Views
{
    partial class Frm_LocationStaffAssignments
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_LocationStaffAssignments));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_Izquierdo = new System.Windows.Forms.Panel();
            this.Lbl_Titulo = new System.Windows.Forms.Label();
            this.Panel_CRUD = new System.Windows.Forms.Panel();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Inactive = new System.Windows.Forms.Button();
            this.Btn_Update = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Panel_1 = new System.Windows.Forms.Panel();
            this.Btn_SearchEmployee = new System.Windows.Forms.Button();
            this.Lbl_Location = new System.Windows.Forms.Label();
            this.Txt_LocationName = new System.Windows.Forms.TextBox();
            this.Lbl_Subtitulo1 = new System.Windows.Forms.Label();
            this.Lbl_Employee = new System.Windows.Forms.Label();
            this.Txt_EmployeeName = new System.Windows.Forms.TextBox();
            this.Lbl_RoleType = new System.Windows.Forms.Label();
            this.CBX_RoleType = new System.Windows.Forms.ComboBox();
            this.Panel_Derecho = new System.Windows.Forms.Panel();
            this.Lbl_EmployeesHeader = new System.Windows.Forms.Label();
            this.Panel_GridEmployees = new System.Windows.Forms.Panel();
            this.Panel_SearchEmployees = new System.Windows.Forms.Panel();
            this.Txt_SearchEmployee = new System.Windows.Forms.TextBox();
            this.Btn_SearchGridEmployees = new System.Windows.Forms.Button();
            this.Btn_CleanSearchGridEmployees = new System.Windows.Forms.Button();
            this.CBX_RoleFilter = new System.Windows.Forms.ComboBox();
            this.CBX_EmployeeStatus = new System.Windows.Forms.ComboBox();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Grid_Locations = new System.Windows.Forms.DataGridView();
            this.PanelToolStrip = new System.Windows.Forms.Panel();
            this.Lbl_LocationsPaging = new System.Windows.Forms.Label();
            this.Panel_SearchLocations = new System.Windows.Forms.Panel();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Btn_CleanSearch = new System.Windows.Forms.Button();
            this.CBX_LocationStatus = new System.Windows.Forms.ComboBox();
            this.PanelToolStrip2 = new System.Windows.Forms.Panel();
            this.Lbl_EmployeesPaging = new System.Windows.Forms.Label();
            this.Grid_Employees = new System.Windows.Forms.DataGridView();
            this.Panel_Superior.SuspendLayout();
            this.Panel_Izquierdo.SuspendLayout();
            this.Panel_CRUD.SuspendLayout();
            this.Panel_1.SuspendLayout();
            this.Panel_Derecho.SuspendLayout();
            this.Panel_GridEmployees.SuspendLayout();
            this.Panel_SearchEmployees.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid_Locations)).BeginInit();
            this.PanelToolStrip.SuspendLayout();
            this.Panel_SearchLocations.SuspendLayout();
            this.PanelToolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid_Employees)).BeginInit();
            this.SuspendLayout();
            // 
            // Panel_Superior
            // 
            this.Panel_Superior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.Panel_Superior.Controls.Add(this.Lbl_Formulario);
            this.Panel_Superior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Superior.Location = new System.Drawing.Point(0, 0);
            this.Panel_Superior.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_Superior.Name = "Panel_Superior";
            this.Panel_Superior.Size = new System.Drawing.Size(1579, 68);
            this.Panel_Superior.TabIndex = 6;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(339, 25);
            this.Lbl_Formulario.TabIndex = 1;
            this.Lbl_Formulario.Text = "GESTIÓN DE PERSONAL PARA SEDES";
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
            this.Lbl_Titulo.Location = new System.Drawing.Point(7, 7);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new System.Drawing.Size(265, 20);
            this.Lbl_Titulo.TabIndex = 80;
            this.Lbl_Titulo.Text = "INFORMACIÓN DEL COLABORADOR";
            // 
            // Panel_CRUD
            // 
            this.Panel_CRUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_CRUD.Controls.Add(this.Btn_Clear);
            this.Panel_CRUD.Controls.Add(this.Btn_Inactive);
            this.Panel_CRUD.Controls.Add(this.Btn_Update);
            this.Panel_CRUD.Controls.Add(this.Btn_Save);
            this.Panel_CRUD.Location = new System.Drawing.Point(7, 46);
            this.Panel_CRUD.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_CRUD.Name = "Panel_CRUD";
            this.Panel_CRUD.Size = new System.Drawing.Size(555, 58);
            this.Panel_CRUD.TabIndex = 83;
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.Location = new System.Drawing.Point(503, 6);
            this.Btn_Clear.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(44, 46);
            this.Btn_Clear.TabIndex = 0;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            this.Btn_Clear.Click += new System.EventHandler(this.Btn_Clear_Click);
            // 
            // Btn_Inactive
            // 
            this.Btn_Inactive.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Inactive.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Inactive.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Inactive.Location = new System.Drawing.Point(325, 6);
            this.Btn_Inactive.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_Inactive.Name = "Btn_Inactive";
            this.Btn_Inactive.Size = new System.Drawing.Size(165, 46);
            this.Btn_Inactive.TabIndex = 1;
            this.Btn_Inactive.Text = "INACTIVAR";
            this.Btn_Inactive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Inactive.UseVisualStyleBackColor = true;
            this.Btn_Inactive.Click += new System.EventHandler(this.Btn_Inactive_Click);
            // 
            // Btn_Update
            // 
            this.Btn_Update.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Update.Image = global::SECRON.Properties.Resources.UpdateAzul25x25;
            this.Btn_Update.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Update.Location = new System.Drawing.Point(167, 6);
            this.Btn_Update.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_Update.Name = "Btn_Update";
            this.Btn_Update.Size = new System.Drawing.Size(151, 46);
            this.Btn_Update.TabIndex = 2;
            this.Btn_Update.Text = "EDITAR";
            this.Btn_Update.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Update.UseVisualStyleBackColor = true;
            this.Btn_Update.Click += new System.EventHandler(this.Btn_Update_Click);
            // 
            // Btn_Save
            // 
            this.Btn_Save.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Save.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Save.Location = new System.Drawing.Point(3, 6);
            this.Btn_Save.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(156, 46);
            this.Btn_Save.TabIndex = 3;
            this.Btn_Save.Text = "GUARDAR";
            this.Btn_Save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Panel_1
            // 
            this.Panel_1.BackColor = System.Drawing.Color.White;
            this.Panel_1.Controls.Add(this.Btn_SearchEmployee);
            this.Panel_1.Controls.Add(this.Lbl_Location);
            this.Panel_1.Controls.Add(this.Txt_LocationName);
            this.Panel_1.Controls.Add(this.Lbl_Subtitulo1);
            this.Panel_1.Controls.Add(this.Lbl_Employee);
            this.Panel_1.Controls.Add(this.Txt_EmployeeName);
            this.Panel_1.Controls.Add(this.Lbl_RoleType);
            this.Panel_1.Controls.Add(this.CBX_RoleType);
            this.Panel_1.Location = new System.Drawing.Point(7, 110);
            this.Panel_1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_1.Name = "Panel_1";
            this.Panel_1.Size = new System.Drawing.Size(555, 320);
            this.Panel_1.TabIndex = 81;
            // 
            // Btn_SearchEmployee
            // 
            this.Btn_SearchEmployee.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SearchEmployee.Image = ((System.Drawing.Image)(resources.GetObject("Btn_SearchEmployee.Image")));
            this.Btn_SearchEmployee.Location = new System.Drawing.Point(475, 155);
            this.Btn_SearchEmployee.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_SearchEmployee.Name = "Btn_SearchEmployee";
            this.Btn_SearchEmployee.Size = new System.Drawing.Size(47, 46);
            this.Btn_SearchEmployee.TabIndex = 13;
            this.Btn_SearchEmployee.UseVisualStyleBackColor = true;
            this.Btn_SearchEmployee.Click += new System.EventHandler(this.Btn_SearchEmployee_Click);
            // 
            // Lbl_Location
            // 
            this.Lbl_Location.AutoSize = true;
            this.Lbl_Location.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Location.Location = new System.Drawing.Point(13, 49);
            this.Lbl_Location.Name = "Lbl_Location";
            this.Lbl_Location.Size = new System.Drawing.Size(44, 20);
            this.Lbl_Location.TabIndex = 11;
            this.Lbl_Location.Text = "SEDE";
            // 
            // Txt_LocationName
            // 
            this.Txt_LocationName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_LocationName.Location = new System.Drawing.Point(19, 87);
            this.Txt_LocationName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_LocationName.Name = "Txt_LocationName";
            this.Txt_LocationName.Size = new System.Drawing.Size(503, 27);
            this.Txt_LocationName.TabIndex = 12;
            // 
            // Lbl_Subtitulo1
            // 
            this.Lbl_Subtitulo1.AutoSize = true;
            this.Lbl_Subtitulo1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo1.Image = global::SECRON.Properties.Resources.InfoNegro20x20;
            this.Lbl_Subtitulo1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo1.Location = new System.Drawing.Point(13, 9);
            this.Lbl_Subtitulo1.Name = "Lbl_Subtitulo1";
            this.Lbl_Subtitulo1.Size = new System.Drawing.Size(169, 20);
            this.Lbl_Subtitulo1.TabIndex = 0;
            this.Lbl_Subtitulo1.Text = "      DATOS GENERALES";
            // 
            // Lbl_Employee
            // 
            this.Lbl_Employee.AutoSize = true;
            this.Lbl_Employee.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Employee.Location = new System.Drawing.Point(13, 137);
            this.Lbl_Employee.Name = "Lbl_Employee";
            this.Lbl_Employee.Size = new System.Drawing.Size(89, 20);
            this.Lbl_Employee.TabIndex = 3;
            this.Lbl_Employee.Text = "EMPLEADO";
            // 
            // Txt_EmployeeName
            // 
            this.Txt_EmployeeName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_EmployeeName.Location = new System.Drawing.Point(19, 164);
            this.Txt_EmployeeName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_EmployeeName.Name = "Txt_EmployeeName";
            this.Txt_EmployeeName.Size = new System.Drawing.Size(430, 27);
            this.Txt_EmployeeName.TabIndex = 4;
            // 
            // Lbl_RoleType
            // 
            this.Lbl_RoleType.AutoSize = true;
            this.Lbl_RoleType.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_RoleType.Location = new System.Drawing.Point(13, 215);
            this.Lbl_RoleType.Name = "Lbl_RoleType";
            this.Lbl_RoleType.Size = new System.Drawing.Size(61, 20);
            this.Lbl_RoleType.TabIndex = 9;
            this.Lbl_RoleType.Text = "CARGO";
            // 
            // CBX_RoleType
            // 
            this.CBX_RoleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBX_RoleType.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.CBX_RoleType.Location = new System.Drawing.Point(19, 240);
            this.CBX_RoleType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CBX_RoleType.Name = "CBX_RoleType";
            this.CBX_RoleType.Size = new System.Drawing.Size(503, 28);
            this.CBX_RoleType.TabIndex = 10;
            // 
            // Panel_Derecho
            // 
            this.Panel_Derecho.AutoScroll = true;
            this.Panel_Derecho.Controls.Add(this.PanelToolStrip2);
            this.Panel_Derecho.Controls.Add(this.Lbl_EmployeesHeader);
            this.Panel_Derecho.Controls.Add(this.Panel_GridEmployees);
            this.Panel_Derecho.Controls.Add(this.Panel_SearchEmployees);
            this.Panel_Derecho.Controls.Add(this.PanelTabla);
            this.Panel_Derecho.Controls.Add(this.PanelToolStrip);
            this.Panel_Derecho.Controls.Add(this.Panel_SearchLocations);
            this.Panel_Derecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Derecho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Derecho.Location = new System.Drawing.Point(581, 68);
            this.Panel_Derecho.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_Derecho.Name = "Panel_Derecho";
            this.Panel_Derecho.Size = new System.Drawing.Size(998, 987);
            this.Panel_Derecho.TabIndex = 0;
            // 
            // Lbl_EmployeesHeader
            // 
            this.Lbl_EmployeesHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Lbl_EmployeesHeader.BackColor = System.Drawing.Color.White;
            this.Lbl_EmployeesHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Lbl_EmployeesHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.Lbl_EmployeesHeader.Location = new System.Drawing.Point(5, 433);
            this.Lbl_EmployeesHeader.Name = "Lbl_EmployeesHeader";
            this.Lbl_EmployeesHeader.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Lbl_EmployeesHeader.Size = new System.Drawing.Size(982, 28);
            this.Lbl_EmployeesHeader.TabIndex = 80;
            this.Lbl_EmployeesHeader.Text = "SELECCIONE UNA SEDE";
            this.Lbl_EmployeesHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Panel_GridEmployees
            // 
            this.Panel_GridEmployees.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_GridEmployees.BackColor = System.Drawing.Color.White;
            this.Panel_GridEmployees.Controls.Add(this.Grid_Employees);
            this.Panel_GridEmployees.Location = new System.Drawing.Point(5, 620);
            this.Panel_GridEmployees.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_GridEmployees.Name = "Panel_GridEmployees";
            this.Panel_GridEmployees.Size = new System.Drawing.Size(982, 344);
            this.Panel_GridEmployees.TabIndex = 78;
            // 
            // Panel_SearchEmployees
            // 
            this.Panel_SearchEmployees.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_SearchEmployees.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_SearchEmployees.Controls.Add(this.Txt_SearchEmployee);
            this.Panel_SearchEmployees.Controls.Add(this.Btn_SearchGridEmployees);
            this.Panel_SearchEmployees.Controls.Add(this.Btn_CleanSearchGridEmployees);
            this.Panel_SearchEmployees.Controls.Add(this.CBX_RoleFilter);
            this.Panel_SearchEmployees.Controls.Add(this.CBX_EmployeeStatus);
            this.Panel_SearchEmployees.Location = new System.Drawing.Point(5, 464);
            this.Panel_SearchEmployees.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_SearchEmployees.Name = "Panel_SearchEmployees";
            this.Panel_SearchEmployees.Size = new System.Drawing.Size(982, 100);
            this.Panel_SearchEmployees.TabIndex = 77;
            // 
            // Txt_SearchEmployee
            // 
            this.Txt_SearchEmployee.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_SearchEmployee.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_SearchEmployee.Location = new System.Drawing.Point(21, 18);
            this.Txt_SearchEmployee.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_SearchEmployee.Name = "Txt_SearchEmployee";
            this.Txt_SearchEmployee.Size = new System.Drawing.Size(698, 27);
            this.Txt_SearchEmployee.TabIndex = 0;
            this.Txt_SearchEmployee.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_SearchEmployee_KeyDown);
            // 
            // Btn_SearchGridEmployees
            // 
            this.Btn_SearchGridEmployees.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_SearchGridEmployees.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SearchGridEmployees.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_SearchGridEmployees.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_SearchGridEmployees.Location = new System.Drawing.Point(729, 12);
            this.Btn_SearchGridEmployees.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_SearchGridEmployees.Name = "Btn_SearchGridEmployees";
            this.Btn_SearchGridEmployees.Size = new System.Drawing.Size(135, 38);
            this.Btn_SearchGridEmployees.TabIndex = 1;
            this.Btn_SearchGridEmployees.Text = "BUSCAR";
            this.Btn_SearchGridEmployees.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SearchGridEmployees.UseVisualStyleBackColor = true;
            this.Btn_SearchGridEmployees.Click += new System.EventHandler(this.Btn_SearchGridEmployees_Click);
            // 
            // Btn_CleanSearchGridEmployees
            // 
            this.Btn_CleanSearchGridEmployees.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_CleanSearchGridEmployees.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_CleanSearchGridEmployees.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_CleanSearchGridEmployees.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_CleanSearchGridEmployees.Location = new System.Drawing.Point(872, 12);
            this.Btn_CleanSearchGridEmployees.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_CleanSearchGridEmployees.Name = "Btn_CleanSearchGridEmployees";
            this.Btn_CleanSearchGridEmployees.Size = new System.Drawing.Size(40, 38);
            this.Btn_CleanSearchGridEmployees.TabIndex = 2;
            this.Btn_CleanSearchGridEmployees.UseVisualStyleBackColor = true;
            this.Btn_CleanSearchGridEmployees.Click += new System.EventHandler(this.Btn_CleanSearchGridEmployees_Click);
            // 
            // CBX_RoleFilter
            // 
            this.CBX_RoleFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBX_RoleFilter.FormattingEnabled = true;
            this.CBX_RoleFilter.Location = new System.Drawing.Point(21, 62);
            this.CBX_RoleFilter.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CBX_RoleFilter.Name = "CBX_RoleFilter";
            this.CBX_RoleFilter.Size = new System.Drawing.Size(200, 28);
            this.CBX_RoleFilter.TabIndex = 3;
            // 
            // CBX_EmployeeStatus
            // 
            this.CBX_EmployeeStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBX_EmployeeStatus.FormattingEnabled = true;
            this.CBX_EmployeeStatus.Location = new System.Drawing.Point(231, 62);
            this.CBX_EmployeeStatus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CBX_EmployeeStatus.Name = "CBX_EmployeeStatus";
            this.CBX_EmployeeStatus.Size = new System.Drawing.Size(200, 28);
            this.CBX_EmployeeStatus.TabIndex = 4;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Grid_Locations);
            this.PanelTabla.Location = new System.Drawing.Point(5, 210);
            this.PanelTabla.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(982, 220);
            this.PanelTabla.TabIndex = 76;
            // 
            // Grid_Locations
            // 
            this.Grid_Locations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Grid_Locations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grid_Locations.Location = new System.Drawing.Point(0, 0);
            this.Grid_Locations.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Grid_Locations.Name = "Grid_Locations";
            this.Grid_Locations.RowHeadersWidth = 51;
            this.Grid_Locations.RowTemplate.Height = 24;
            this.Grid_Locations.Size = new System.Drawing.Size(982, 220);
            this.Grid_Locations.TabIndex = 1;
            // 
            // PanelToolStrip
            // 
            this.PanelToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelToolStrip.Controls.Add(this.Lbl_LocationsPaging);
            this.PanelToolStrip.Location = new System.Drawing.Point(5, 158);
            this.PanelToolStrip.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PanelToolStrip.Name = "PanelToolStrip";
            this.PanelToolStrip.Size = new System.Drawing.Size(982, 48);
            this.PanelToolStrip.TabIndex = 75;
            // 
            // Lbl_LocationsPaging
            // 
            this.Lbl_LocationsPaging.AutoSize = true;
            this.Lbl_LocationsPaging.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_LocationsPaging.Location = new System.Drawing.Point(16, 14);
            this.Lbl_LocationsPaging.Name = "Lbl_LocationsPaging";
            this.Lbl_LocationsPaging.Size = new System.Drawing.Size(226, 20);
            this.Lbl_LocationsPaging.TabIndex = 0;
            this.Lbl_LocationsPaging.Text = "MOSTRANDO 1-20 DE 0 SEDES";
            // 
            // Panel_SearchLocations
            // 
            this.Panel_SearchLocations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_SearchLocations.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_SearchLocations.Controls.Add(this.Txt_ValorBuscado);
            this.Panel_SearchLocations.Controls.Add(this.Btn_Search);
            this.Panel_SearchLocations.Controls.Add(this.Btn_CleanSearch);
            this.Panel_SearchLocations.Controls.Add(this.CBX_LocationStatus);
            this.Panel_SearchLocations.Location = new System.Drawing.Point(5, 6);
            this.Panel_SearchLocations.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel_SearchLocations.Name = "Panel_SearchLocations";
            this.Panel_SearchLocations.Size = new System.Drawing.Size(982, 148);
            this.Panel_SearchLocations.TabIndex = 74;
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(21, 28);
            this.Txt_ValorBuscado.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(698, 27);
            this.Txt_ValorBuscado.TabIndex = 0;
            this.Txt_ValorBuscado.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado_KeyDown);
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Search.Location = new System.Drawing.Point(725, 21);
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
            this.Btn_CleanSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_CleanSearch.Location = new System.Drawing.Point(868, 21);
            this.Btn_CleanSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Btn_CleanSearch.Name = "Btn_CleanSearch";
            this.Btn_CleanSearch.Size = new System.Drawing.Size(40, 38);
            this.Btn_CleanSearch.TabIndex = 2;
            this.Btn_CleanSearch.UseVisualStyleBackColor = true;
            this.Btn_CleanSearch.Click += new System.EventHandler(this.Btn_CleanSearch_Click);
            // 
            // CBX_LocationStatus
            // 
            this.CBX_LocationStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBX_LocationStatus.FormattingEnabled = true;
            this.CBX_LocationStatus.Location = new System.Drawing.Point(21, 82);
            this.CBX_LocationStatus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CBX_LocationStatus.Name = "CBX_LocationStatus";
            this.CBX_LocationStatus.Size = new System.Drawing.Size(291, 28);
            this.CBX_LocationStatus.TabIndex = 3;
            // 
            // PanelToolStrip2
            // 
            this.PanelToolStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelToolStrip2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelToolStrip2.Controls.Add(this.Lbl_EmployeesPaging);
            this.PanelToolStrip2.Location = new System.Drawing.Point(5, 568);
            this.PanelToolStrip2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PanelToolStrip2.Name = "PanelToolStrip2";
            this.PanelToolStrip2.Size = new System.Drawing.Size(982, 48);
            this.PanelToolStrip2.TabIndex = 76;
            // 
            // Lbl_EmployeesPaging
            // 
            this.Lbl_EmployeesPaging.AutoSize = true;
            this.Lbl_EmployeesPaging.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_EmployeesPaging.Location = new System.Drawing.Point(16, 14);
            this.Lbl_EmployeesPaging.Name = "Lbl_EmployeesPaging";
            this.Lbl_EmployeesPaging.Size = new System.Drawing.Size(271, 20);
            this.Lbl_EmployeesPaging.TabIndex = 0;
            this.Lbl_EmployeesPaging.Text = "MOSTRANDO 1-20 DE 0 EMPLEADOS";
            // 
            // Grid_Employees
            // 
            this.Grid_Employees.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Grid_Employees.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grid_Employees.Location = new System.Drawing.Point(0, 0);
            this.Grid_Employees.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Grid_Employees.Name = "Grid_Employees";
            this.Grid_Employees.RowHeadersWidth = 51;
            this.Grid_Employees.RowTemplate.Height = 24;
            this.Grid_Employees.Size = new System.Drawing.Size(982, 344);
            this.Grid_Employees.TabIndex = 3;
            // 
            // Frm_LocationStaffAssignments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1579, 1055);
            this.Controls.Add(this.Panel_Derecho);
            this.Controls.Add(this.Panel_Izquierdo);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Frm_LocationStaffAssignments";
            this.Text = "SECRON - GESTIÓN DE PERSONAL PARA SEDES";
            this.Load += new System.EventHandler(this.Frm_LocationStaffAssignments_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Izquierdo.ResumeLayout(false);
            this.Panel_Izquierdo.PerformLayout();
            this.Panel_CRUD.ResumeLayout(false);
            this.Panel_1.ResumeLayout(false);
            this.Panel_1.PerformLayout();
            this.Panel_Derecho.ResumeLayout(false);
            this.Panel_GridEmployees.ResumeLayout(false);
            this.Panel_SearchEmployees.ResumeLayout(false);
            this.Panel_SearchEmployees.PerformLayout();
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Grid_Locations)).EndInit();
            this.PanelToolStrip.ResumeLayout(false);
            this.PanelToolStrip.PerformLayout();
            this.Panel_SearchLocations.ResumeLayout(false);
            this.Panel_SearchLocations.PerformLayout();
            this.PanelToolStrip2.ResumeLayout(false);
            this.PanelToolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid_Employees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_Izquierdo;
        private System.Windows.Forms.Panel Panel_Derecho;
        private System.Windows.Forms.Label Lbl_Titulo;
        private System.Windows.Forms.Panel Panel_CRUD;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_Inactive;
        private System.Windows.Forms.Button Btn_Update;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Panel Panel_1;
        private System.Windows.Forms.Label Lbl_Subtitulo1;
        private System.Windows.Forms.Label Lbl_Employee;
        private System.Windows.Forms.TextBox Txt_EmployeeName;
        private System.Windows.Forms.Label Lbl_RoleType;
        private System.Windows.Forms.ComboBox CBX_RoleType;
        private System.Windows.Forms.Panel Panel_SearchLocations;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Button Btn_CleanSearch;
        private System.Windows.Forms.ComboBox CBX_LocationStatus;
        private System.Windows.Forms.Panel PanelToolStrip;
        private System.Windows.Forms.Label Lbl_LocationsPaging;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.Panel Panel_SearchEmployees;
        private System.Windows.Forms.TextBox Txt_SearchEmployee;
        private System.Windows.Forms.Button Btn_SearchGridEmployees;
        private System.Windows.Forms.Button Btn_CleanSearchGridEmployees;
        private System.Windows.Forms.ComboBox CBX_RoleFilter;
        private System.Windows.Forms.ComboBox CBX_EmployeeStatus;
        private System.Windows.Forms.Panel Panel_GridEmployees;
        private System.Windows.Forms.Label Lbl_EmployeesHeader;
        private System.Windows.Forms.DataGridView Grid_Locations;
        private System.Windows.Forms.Label Lbl_Location;
        private System.Windows.Forms.TextBox Txt_LocationName;
        private System.Windows.Forms.Button Btn_SearchEmployee;
        private System.Windows.Forms.Panel PanelToolStrip2;
        private System.Windows.Forms.Label Lbl_EmployeesPaging;
        private System.Windows.Forms.DataGridView Grid_Employees;
    }
}