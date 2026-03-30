namespace SECRON.Views
{
    partial class Frm_Users_Managment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Users_Managment));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Btn_Export = new System.Windows.Forms.Button();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_Izquierdo = new System.Windows.Forms.Panel();
            this.Panel_CRUD = new System.Windows.Forms.Panel();
            this.Btn_ClearInformacion = new System.Windows.Forms.Button();
            this.Btn_Send = new System.Windows.Forms.Button();
            this.Btn_Update = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Panel_Informacion = new System.Windows.Forms.Panel();
            this.Btn_ResetPassword = new System.Windows.Forms.Button();
            this.ComboBox_Bloqueado = new System.Windows.Forms.ComboBox();
            this.Lbl_Bloqueado = new System.Windows.Forms.Label();
            this.ComboBox_UserStatus = new System.Windows.Forms.ComboBox();
            this.Lbl_UserStatus = new System.Windows.Forms.Label();
            this.Txt_Colaborador = new System.Windows.Forms.TextBox();
            this.Lbl_Colaborador = new System.Windows.Forms.Label();
            this.CheckBox_PasswordTemp = new System.Windows.Forms.CheckBox();
            this.Txt_CorreoInstitucional = new System.Windows.Forms.TextBox();
            this.Lbl_CorreoInstitucional = new System.Windows.Forms.Label();
            this.Txt_Password = new System.Windows.Forms.TextBox();
            this.Lbl_Password = new System.Windows.Forms.Label();
            this.ComboBox_Rol = new System.Windows.Forms.ComboBox();
            this.Txt_Usuario = new System.Windows.Forms.TextBox();
            this.Lbl_Rol = new System.Windows.Forms.Label();
            this.Lbl_Usuario = new System.Windows.Forms.Label();
            this.Lbl_Subtitulo1 = new System.Windows.Forms.Label();
            this.Lbl_TituloPanelIzquierdo = new System.Windows.Forms.Label();
            this.Panel_Derecho = new System.Windows.Forms.Panel();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.PanelTabla1 = new System.Windows.Forms.Panel();
            this.Tabla1 = new System.Windows.Forms.DataGridView();
            this.PanelToolStrip1 = new System.Windows.Forms.Panel();
            this.Lbl_PaginasUsuarios = new System.Windows.Forms.Label();
            this.Panel_Busqueda1 = new System.Windows.Forms.Panel();
            this.Lbl_Usuarios = new System.Windows.Forms.Label();
            this.Btn_ClearUsers = new System.Windows.Forms.Button();
            this.FiltroU3 = new System.Windows.Forms.ComboBox();
            this.FiltroU2 = new System.Windows.Forms.ComboBox();
            this.Btn_SearchUsers = new System.Windows.Forms.Button();
            this.FiltroU1 = new System.Windows.Forms.ComboBox();
            this.Txt_ValorBuscado1 = new System.Windows.Forms.TextBox();
            this.PanelTabla2 = new System.Windows.Forms.Panel();
            this.Tabla2 = new System.Windows.Forms.DataGridView();
            this.PanelToolStrip2 = new System.Windows.Forms.Panel();
            this.Lbl_PaginasColaboradores = new System.Windows.Forms.Label();
            this.Panel_Busqueda2 = new System.Windows.Forms.Panel();
            this.Lbl_Colaboradores = new System.Windows.Forms.Label();
            this.Btn_ClearEmployees = new System.Windows.Forms.Button();
            this.FiltroC3 = new System.Windows.Forms.ComboBox();
            this.FiltroC2 = new System.Windows.Forms.ComboBox();
            this.Btn_SearchEmployees = new System.Windows.Forms.Button();
            this.FiltroC1 = new System.Windows.Forms.ComboBox();
            this.Txt_ValorBuscado2 = new System.Windows.Forms.TextBox();
            this.Btn_DesbloquearUsuario = new System.Windows.Forms.Button();
            this.Panel_Superior.SuspendLayout();
            this.Panel_Izquierdo.SuspendLayout();
            this.Panel_CRUD.SuspendLayout();
            this.Panel_Informacion.SuspendLayout();
            this.Panel_Derecho.SuspendLayout();
            this.PanelTabla1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla1)).BeginInit();
            this.PanelToolStrip1.SuspendLayout();
            this.Panel_Busqueda1.SuspendLayout();
            this.PanelTabla2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla2)).BeginInit();
            this.PanelToolStrip2.SuspendLayout();
            this.Panel_Busqueda2.SuspendLayout();
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
            this.Panel_Superior.TabIndex = 1;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(222, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "GESTIÓN DE USUARIOS";
            // 
            // Panel_Izquierdo
            // 
            this.Panel_Izquierdo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Izquierdo.Controls.Add(this.Panel_CRUD);
            this.Panel_Izquierdo.Controls.Add(this.Panel_Informacion);
            this.Panel_Izquierdo.Controls.Add(this.Lbl_TituloPanelIzquierdo);
            this.Panel_Izquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.Panel_Izquierdo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Izquierdo.ForeColor = System.Drawing.Color.Black;
            this.Panel_Izquierdo.Location = new System.Drawing.Point(0, 55);
            this.Panel_Izquierdo.Name = "Panel_Izquierdo";
            this.Panel_Izquierdo.Size = new System.Drawing.Size(415, 806);
            this.Panel_Izquierdo.TabIndex = 2;
            // 
            // Panel_CRUD
            // 
            this.Panel_CRUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_CRUD.Controls.Add(this.Btn_ClearInformacion);
            this.Panel_CRUD.Controls.Add(this.Btn_Send);
            this.Panel_CRUD.Controls.Add(this.Btn_Update);
            this.Panel_CRUD.Controls.Add(this.Btn_Save);
            this.Panel_CRUD.Location = new System.Drawing.Point(16, 40);
            this.Panel_CRUD.Name = "Panel_CRUD";
            this.Panel_CRUD.Size = new System.Drawing.Size(384, 90);
            this.Panel_CRUD.TabIndex = 75;
            // 
            // Btn_ClearInformacion
            // 
            this.Btn_ClearInformacion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ClearInformacion.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_ClearInformacion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_ClearInformacion.Location = new System.Drawing.Point(234, 6);
            this.Btn_ClearInformacion.Name = "Btn_ClearInformacion";
            this.Btn_ClearInformacion.Size = new System.Drawing.Size(33, 37);
            this.Btn_ClearInformacion.TabIndex = 57;
            this.Btn_ClearInformacion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_ClearInformacion.UseVisualStyleBackColor = true;
            // 
            // Btn_Send
            // 
            this.Btn_Send.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Send.Image = global::SECRON.Properties.Resources.MailNegro25x25;
            this.Btn_Send.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Send.Location = new System.Drawing.Point(3, 46);
            this.Btn_Send.Name = "Btn_Send";
            this.Btn_Send.Size = new System.Drawing.Size(207, 37);
            this.Btn_Send.TabIndex = 56;
            this.Btn_Send.Text = "ENVIAR POR CORREO";
            this.Btn_Send.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Send.UseVisualStyleBackColor = true;
            this.Btn_Send.Click += new System.EventHandler(this.Btn_Send_Click);
            // 
            // Btn_Update
            // 
            this.Btn_Update.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Update.Image = global::SECRON.Properties.Resources.UpdateAzul25x25;
            this.Btn_Update.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Update.Location = new System.Drawing.Point(130, 6);
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
            this.Btn_Save.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Save.Image = global::SECRON.Properties.Resources.SaveVerde25x25;
            this.Btn_Save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Save.Location = new System.Drawing.Point(3, 6);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(117, 37);
            this.Btn_Save.TabIndex = 54;
            this.Btn_Save.Text = "GUARDAR";
            this.Btn_Save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Panel_Informacion
            // 
            this.Panel_Informacion.BackColor = System.Drawing.Color.White;
            this.Panel_Informacion.Controls.Add(this.Btn_DesbloquearUsuario);
            this.Panel_Informacion.Controls.Add(this.Btn_ResetPassword);
            this.Panel_Informacion.Controls.Add(this.ComboBox_Bloqueado);
            this.Panel_Informacion.Controls.Add(this.Lbl_Bloqueado);
            this.Panel_Informacion.Controls.Add(this.ComboBox_UserStatus);
            this.Panel_Informacion.Controls.Add(this.Lbl_UserStatus);
            this.Panel_Informacion.Controls.Add(this.Txt_Colaborador);
            this.Panel_Informacion.Controls.Add(this.Lbl_Colaborador);
            this.Panel_Informacion.Controls.Add(this.CheckBox_PasswordTemp);
            this.Panel_Informacion.Controls.Add(this.Txt_CorreoInstitucional);
            this.Panel_Informacion.Controls.Add(this.Lbl_CorreoInstitucional);
            this.Panel_Informacion.Controls.Add(this.Txt_Password);
            this.Panel_Informacion.Controls.Add(this.Lbl_Password);
            this.Panel_Informacion.Controls.Add(this.ComboBox_Rol);
            this.Panel_Informacion.Controls.Add(this.Txt_Usuario);
            this.Panel_Informacion.Controls.Add(this.Lbl_Rol);
            this.Panel_Informacion.Controls.Add(this.Lbl_Usuario);
            this.Panel_Informacion.Controls.Add(this.Lbl_Subtitulo1);
            this.Panel_Informacion.Location = new System.Drawing.Point(16, 147);
            this.Panel_Informacion.Name = "Panel_Informacion";
            this.Panel_Informacion.Size = new System.Drawing.Size(384, 639);
            this.Panel_Informacion.TabIndex = 52;
            // 
            // Btn_ResetPassword
            // 
            this.Btn_ResetPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_ResetPassword.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ResetPassword.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_ResetPassword.Location = new System.Drawing.Point(14, 466);
            this.Btn_ResetPassword.Name = "Btn_ResetPassword";
            this.Btn_ResetPassword.Size = new System.Drawing.Size(356, 30);
            this.Btn_ResetPassword.TabIndex = 78;
            this.Btn_ResetPassword.Text = "RESTABLECER CONTRASEÑA";
            this.Btn_ResetPassword.UseVisualStyleBackColor = true;
            this.Btn_ResetPassword.Click += new System.EventHandler(this.Btn_ResetPassword_Click);
            // 
            // ComboBox_Bloqueado
            // 
            this.ComboBox_Bloqueado.FormattingEnabled = true;
            this.ComboBox_Bloqueado.Location = new System.Drawing.Point(14, 409);
            this.ComboBox_Bloqueado.Name = "ComboBox_Bloqueado";
            this.ComboBox_Bloqueado.Size = new System.Drawing.Size(356, 28);
            this.ComboBox_Bloqueado.TabIndex = 77;
            // 
            // Lbl_Bloqueado
            // 
            this.Lbl_Bloqueado.AutoSize = true;
            this.Lbl_Bloqueado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Bloqueado.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Bloqueado.Location = new System.Drawing.Point(10, 386);
            this.Lbl_Bloqueado.Name = "Lbl_Bloqueado";
            this.Lbl_Bloqueado.Size = new System.Drawing.Size(183, 20);
            this.Lbl_Bloqueado.TabIndex = 76;
            this.Lbl_Bloqueado.Text = "USUARIO BLOQUEADO *";
            // 
            // ComboBox_UserStatus
            // 
            this.ComboBox_UserStatus.FormattingEnabled = true;
            this.ComboBox_UserStatus.Location = new System.Drawing.Point(14, 345);
            this.ComboBox_UserStatus.Name = "ComboBox_UserStatus";
            this.ComboBox_UserStatus.Size = new System.Drawing.Size(356, 28);
            this.ComboBox_UserStatus.TabIndex = 75;
            // 
            // Lbl_UserStatus
            // 
            this.Lbl_UserStatus.AutoSize = true;
            this.Lbl_UserStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_UserStatus.ForeColor = System.Drawing.Color.Black;
            this.Lbl_UserStatus.Location = new System.Drawing.Point(10, 322);
            this.Lbl_UserStatus.Name = "Lbl_UserStatus";
            this.Lbl_UserStatus.Size = new System.Drawing.Size(171, 20);
            this.Lbl_UserStatus.TabIndex = 74;
            this.Lbl_UserStatus.Text = "ESTADO DE USUARIO *";
            // 
            // Txt_Colaborador
            // 
            this.Txt_Colaborador.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Colaborador.Location = new System.Drawing.Point(14, 222);
            this.Txt_Colaborador.MaxLength = 15;
            this.Txt_Colaborador.Name = "Txt_Colaborador";
            this.Txt_Colaborador.Size = new System.Drawing.Size(356, 27);
            this.Txt_Colaborador.TabIndex = 72;
            // 
            // Lbl_Colaborador
            // 
            this.Lbl_Colaborador.AutoSize = true;
            this.Lbl_Colaborador.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Colaborador.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Colaborador.Location = new System.Drawing.Point(10, 199);
            this.Lbl_Colaborador.Name = "Lbl_Colaborador";
            this.Lbl_Colaborador.Size = new System.Drawing.Size(294, 20);
            this.Lbl_Colaborador.TabIndex = 73;
            this.Lbl_Colaborador.Text = "NOMBRE COLABORADOR VINCULADO *";
            // 
            // CheckBox_PasswordTemp
            // 
            this.CheckBox_PasswordTemp.AutoSize = true;
            this.CheckBox_PasswordTemp.Checked = true;
            this.CheckBox_PasswordTemp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_PasswordTemp.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.CheckBox_PasswordTemp.Location = new System.Drawing.Point(14, 162);
            this.CheckBox_PasswordTemp.Name = "CheckBox_PasswordTemp";
            this.CheckBox_PasswordTemp.Size = new System.Drawing.Size(143, 19);
            this.CheckBox_PasswordTemp.TabIndex = 71;
            this.CheckBox_PasswordTemp.Text = "Contraseña Temporal";
            this.CheckBox_PasswordTemp.UseVisualStyleBackColor = true;
            // 
            // Txt_CorreoInstitucional
            // 
            this.Txt_CorreoInstitucional.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_CorreoInstitucional.Location = new System.Drawing.Point(14, 281);
            this.Txt_CorreoInstitucional.MaxLength = 15;
            this.Txt_CorreoInstitucional.Name = "Txt_CorreoInstitucional";
            this.Txt_CorreoInstitucional.Size = new System.Drawing.Size(356, 27);
            this.Txt_CorreoInstitucional.TabIndex = 69;
            // 
            // Lbl_CorreoInstitucional
            // 
            this.Lbl_CorreoInstitucional.AutoSize = true;
            this.Lbl_CorreoInstitucional.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_CorreoInstitucional.ForeColor = System.Drawing.Color.Black;
            this.Lbl_CorreoInstitucional.Location = new System.Drawing.Point(10, 258);
            this.Lbl_CorreoInstitucional.Name = "Lbl_CorreoInstitucional";
            this.Lbl_CorreoInstitucional.Size = new System.Drawing.Size(198, 20);
            this.Lbl_CorreoInstitucional.TabIndex = 70;
            this.Lbl_CorreoInstitucional.Text = "CORREO INSTITUCIONAL *";
            // 
            // Txt_Password
            // 
            this.Txt_Password.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Password.Location = new System.Drawing.Point(14, 129);
            this.Txt_Password.MaxLength = 15;
            this.Txt_Password.Name = "Txt_Password";
            this.Txt_Password.Size = new System.Drawing.Size(356, 27);
            this.Txt_Password.TabIndex = 67;
            // 
            // Lbl_Password
            // 
            this.Lbl_Password.AutoSize = true;
            this.Lbl_Password.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Password.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Password.Location = new System.Drawing.Point(10, 106);
            this.Lbl_Password.Name = "Lbl_Password";
            this.Lbl_Password.Size = new System.Drawing.Size(121, 20);
            this.Lbl_Password.TabIndex = 68;
            this.Lbl_Password.Text = "CONTRASEÑA *";
            // 
            // ComboBox_Rol
            // 
            this.ComboBox_Rol.FormattingEnabled = true;
            this.ComboBox_Rol.Location = new System.Drawing.Point(200, 68);
            this.ComboBox_Rol.Name = "ComboBox_Rol";
            this.ComboBox_Rol.Size = new System.Drawing.Size(170, 28);
            this.ComboBox_Rol.TabIndex = 66;
            // 
            // Txt_Usuario
            // 
            this.Txt_Usuario.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Usuario.Location = new System.Drawing.Point(14, 68);
            this.Txt_Usuario.MaxLength = 15;
            this.Txt_Usuario.Name = "Txt_Usuario";
            this.Txt_Usuario.Size = new System.Drawing.Size(170, 27);
            this.Txt_Usuario.TabIndex = 1;
            // 
            // Lbl_Rol
            // 
            this.Lbl_Rol.AutoSize = true;
            this.Lbl_Rol.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Rol.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Rol.Location = new System.Drawing.Point(196, 45);
            this.Lbl_Rol.Name = "Lbl_Rol";
            this.Lbl_Rol.Size = new System.Drawing.Size(49, 20);
            this.Lbl_Rol.TabIndex = 2;
            this.Lbl_Rol.Text = "ROL *";
            // 
            // Lbl_Usuario
            // 
            this.Lbl_Usuario.AutoSize = true;
            this.Lbl_Usuario.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Usuario.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Usuario.Location = new System.Drawing.Point(10, 45);
            this.Lbl_Usuario.Name = "Lbl_Usuario";
            this.Lbl_Usuario.Size = new System.Drawing.Size(87, 20);
            this.Lbl_Usuario.TabIndex = 1;
            this.Lbl_Usuario.Text = "USUARIO *";
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
            this.Lbl_Subtitulo1.Size = new System.Drawing.Size(243, 20);
            this.Lbl_Subtitulo1.TabIndex = 1;
            this.Lbl_Subtitulo1.Text = "      INFORMACIÓN DEL USUARIO";
            this.Lbl_Subtitulo1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Lbl_TituloPanelIzquierdo
            // 
            this.Lbl_TituloPanelIzquierdo.AutoSize = true;
            this.Lbl_TituloPanelIzquierdo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Lbl_TituloPanelIzquierdo.ForeColor = System.Drawing.Color.Black;
            this.Lbl_TituloPanelIzquierdo.Location = new System.Drawing.Point(12, 10);
            this.Lbl_TituloPanelIzquierdo.Name = "Lbl_TituloPanelIzquierdo";
            this.Lbl_TituloPanelIzquierdo.Size = new System.Drawing.Size(232, 21);
            this.Lbl_TituloPanelIzquierdo.TabIndex = 51;
            this.Lbl_TituloPanelIzquierdo.Text = "INFORMACIÓN DEL USUARIO";
            // 
            // Panel_Derecho
            // 
            this.Panel_Derecho.Controls.Add(this.vScrollBar);
            this.Panel_Derecho.Controls.Add(this.PanelTabla1);
            this.Panel_Derecho.Controls.Add(this.PanelToolStrip1);
            this.Panel_Derecho.Controls.Add(this.Panel_Busqueda1);
            this.Panel_Derecho.Controls.Add(this.PanelTabla2);
            this.Panel_Derecho.Controls.Add(this.PanelToolStrip2);
            this.Panel_Derecho.Controls.Add(this.Panel_Busqueda2);
            this.Panel_Derecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Derecho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Derecho.ForeColor = System.Drawing.Color.Black;
            this.Panel_Derecho.Location = new System.Drawing.Point(415, 55);
            this.Panel_Derecho.Name = "Panel_Derecho";
            this.Panel_Derecho.Size = new System.Drawing.Size(769, 806);
            this.Panel_Derecho.TabIndex = 3;
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(759, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(10, 806);
            this.vScrollBar.TabIndex = 77;
            // 
            // PanelTabla1
            // 
            this.PanelTabla1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla1.Controls.Add(this.Tabla1);
            this.PanelTabla1.Location = new System.Drawing.Point(22, 191);
            this.PanelTabla1.Name = "PanelTabla1";
            this.PanelTabla1.Size = new System.Drawing.Size(725, 203);
            this.PanelTabla1.TabIndex = 75;
            // 
            // Tabla1
            // 
            this.Tabla1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla1.Location = new System.Drawing.Point(0, 0);
            this.Tabla1.Name = "Tabla1";
            this.Tabla1.Size = new System.Drawing.Size(725, 203);
            this.Tabla1.TabIndex = 1;
            this.Tabla1.SelectionChanged += new System.EventHandler(this.Tabla1_SelectionChanged);
            // 
            // PanelToolStrip1
            // 
            this.PanelToolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelToolStrip1.Controls.Add(this.Lbl_PaginasUsuarios);
            this.PanelToolStrip1.Location = new System.Drawing.Point(22, 136);
            this.PanelToolStrip1.Name = "PanelToolStrip1";
            this.PanelToolStrip1.Size = new System.Drawing.Size(725, 39);
            this.PanelToolStrip1.TabIndex = 74;
            // 
            // Lbl_PaginasUsuarios
            // 
            this.Lbl_PaginasUsuarios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_PaginasUsuarios.AutoSize = true;
            this.Lbl_PaginasUsuarios.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_PaginasUsuarios.ForeColor = System.Drawing.Color.Black;
            this.Lbl_PaginasUsuarios.Location = new System.Drawing.Point(12, 11);
            this.Lbl_PaginasUsuarios.Name = "Lbl_PaginasUsuarios";
            this.Lbl_PaginasUsuarios.Size = new System.Drawing.Size(276, 20);
            this.Lbl_PaginasUsuarios.TabIndex = 51;
            this.Lbl_PaginasUsuarios.Text = "MOSTRANDO 1-10 DE 100 USUARIOS";
            // 
            // Panel_Busqueda1
            // 
            this.Panel_Busqueda1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Busqueda1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Busqueda1.Controls.Add(this.Lbl_Usuarios);
            this.Panel_Busqueda1.Controls.Add(this.Btn_ClearUsers);
            this.Panel_Busqueda1.Controls.Add(this.FiltroU3);
            this.Panel_Busqueda1.Controls.Add(this.FiltroU2);
            this.Panel_Busqueda1.Controls.Add(this.Btn_SearchUsers);
            this.Panel_Busqueda1.Controls.Add(this.FiltroU1);
            this.Panel_Busqueda1.Controls.Add(this.Txt_ValorBuscado1);
            this.Panel_Busqueda1.Location = new System.Drawing.Point(22, 22);
            this.Panel_Busqueda1.Name = "Panel_Busqueda1";
            this.Panel_Busqueda1.Size = new System.Drawing.Size(725, 108);
            this.Panel_Busqueda1.TabIndex = 73;
            // 
            // Lbl_Usuarios
            // 
            this.Lbl_Usuarios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_Usuarios.AutoSize = true;
            this.Lbl_Usuarios.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Usuarios.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Usuarios.Location = new System.Drawing.Point(12, 11);
            this.Lbl_Usuarios.Name = "Lbl_Usuarios";
            this.Lbl_Usuarios.Size = new System.Drawing.Size(192, 20);
            this.Lbl_Usuarios.TabIndex = 72;
            this.Lbl_Usuarios.Text = "BUSQUEDA DE USUARIOS";
            // 
            // Btn_ClearUsers
            // 
            this.Btn_ClearUsers.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_ClearUsers.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ClearUsers.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_ClearUsers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_ClearUsers.Location = new System.Drawing.Point(675, 26);
            this.Btn_ClearUsers.Name = "Btn_ClearUsers";
            this.Btn_ClearUsers.Size = new System.Drawing.Size(30, 35);
            this.Btn_ClearUsers.TabIndex = 71;
            this.Btn_ClearUsers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_ClearUsers.UseVisualStyleBackColor = true;
            this.Btn_ClearUsers.Click += new System.EventHandler(this.Btn_ClearUsers_Click);
            // 
            // FiltroU3
            // 
            this.FiltroU3.FormattingEnabled = true;
            this.FiltroU3.Location = new System.Drawing.Point(486, 67);
            this.FiltroU3.Name = "FiltroU3";
            this.FiltroU3.Size = new System.Drawing.Size(219, 28);
            this.FiltroU3.TabIndex = 70;
            // 
            // FiltroU2
            // 
            this.FiltroU2.FormattingEnabled = true;
            this.FiltroU2.Location = new System.Drawing.Point(252, 67);
            this.FiltroU2.Name = "FiltroU2";
            this.FiltroU2.Size = new System.Drawing.Size(219, 28);
            this.FiltroU2.TabIndex = 69;
            // 
            // Btn_SearchUsers
            // 
            this.Btn_SearchUsers.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_SearchUsers.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SearchUsers.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_SearchUsers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_SearchUsers.Location = new System.Drawing.Point(568, 26);
            this.Btn_SearchUsers.Name = "Btn_SearchUsers";
            this.Btn_SearchUsers.Size = new System.Drawing.Size(101, 35);
            this.Btn_SearchUsers.TabIndex = 54;
            this.Btn_SearchUsers.Text = "BUSCAR";
            this.Btn_SearchUsers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SearchUsers.UseVisualStyleBackColor = true;
            this.Btn_SearchUsers.Click += new System.EventHandler(this.Btn_SearchUsers_Click);
            // 
            // FiltroU1
            // 
            this.FiltroU1.FormattingEnabled = true;
            this.FiltroU1.Location = new System.Drawing.Point(16, 67);
            this.FiltroU1.Name = "FiltroU1";
            this.FiltroU1.Size = new System.Drawing.Size(219, 28);
            this.FiltroU1.TabIndex = 68;
            // 
            // Txt_ValorBuscado1
            // 
            this.Txt_ValorBuscado1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado1.Location = new System.Drawing.Point(16, 34);
            this.Txt_ValorBuscado1.MaxLength = 15;
            this.Txt_ValorBuscado1.Name = "Txt_ValorBuscado1";
            this.Txt_ValorBuscado1.Size = new System.Drawing.Size(540, 27);
            this.Txt_ValorBuscado1.TabIndex = 59;
            this.Txt_ValorBuscado1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado1_KeyDown);
            // 
            // PanelTabla2
            // 
            this.PanelTabla2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla2.Controls.Add(this.Tabla2);
            this.PanelTabla2.Location = new System.Drawing.Point(22, 583);
            this.PanelTabla2.Name = "PanelTabla2";
            this.PanelTabla2.Size = new System.Drawing.Size(725, 203);
            this.PanelTabla2.TabIndex = 72;
            // 
            // Tabla2
            // 
            this.Tabla2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla2.Location = new System.Drawing.Point(0, 0);
            this.Tabla2.Name = "Tabla2";
            this.Tabla2.Size = new System.Drawing.Size(725, 203);
            this.Tabla2.TabIndex = 1;
            this.Tabla2.SelectionChanged += new System.EventHandler(this.Tabla2_SelectionChanged);
            // 
            // PanelToolStrip2
            // 
            this.PanelToolStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelToolStrip2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelToolStrip2.Controls.Add(this.Lbl_PaginasColaboradores);
            this.PanelToolStrip2.Location = new System.Drawing.Point(22, 528);
            this.PanelToolStrip2.Name = "PanelToolStrip2";
            this.PanelToolStrip2.Size = new System.Drawing.Size(725, 39);
            this.PanelToolStrip2.TabIndex = 71;
            // 
            // Lbl_PaginasColaboradores
            // 
            this.Lbl_PaginasColaboradores.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_PaginasColaboradores.AutoSize = true;
            this.Lbl_PaginasColaboradores.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_PaginasColaboradores.ForeColor = System.Drawing.Color.Black;
            this.Lbl_PaginasColaboradores.Location = new System.Drawing.Point(12, 11);
            this.Lbl_PaginasColaboradores.Name = "Lbl_PaginasColaboradores";
            this.Lbl_PaginasColaboradores.Size = new System.Drawing.Size(330, 20);
            this.Lbl_PaginasColaboradores.TabIndex = 51;
            this.Lbl_PaginasColaboradores.Text = "MOSTRANDO 1-10 DE 100 COLABORADORES";
            // 
            // Panel_Busqueda2
            // 
            this.Panel_Busqueda2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_Busqueda2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Busqueda2.Controls.Add(this.Lbl_Colaboradores);
            this.Panel_Busqueda2.Controls.Add(this.Btn_ClearEmployees);
            this.Panel_Busqueda2.Controls.Add(this.FiltroC3);
            this.Panel_Busqueda2.Controls.Add(this.FiltroC2);
            this.Panel_Busqueda2.Controls.Add(this.Btn_SearchEmployees);
            this.Panel_Busqueda2.Controls.Add(this.FiltroC1);
            this.Panel_Busqueda2.Controls.Add(this.Txt_ValorBuscado2);
            this.Panel_Busqueda2.Location = new System.Drawing.Point(22, 414);
            this.Panel_Busqueda2.Name = "Panel_Busqueda2";
            this.Panel_Busqueda2.Size = new System.Drawing.Size(725, 108);
            this.Panel_Busqueda2.TabIndex = 53;
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
            this.Lbl_Colaboradores.Size = new System.Drawing.Size(246, 20);
            this.Lbl_Colaboradores.TabIndex = 72;
            this.Lbl_Colaboradores.Text = "BUSQUEDA DE COLABORADORES";
            // 
            // Btn_ClearEmployees
            // 
            this.Btn_ClearEmployees.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_ClearEmployees.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_ClearEmployees.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_ClearEmployees.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_ClearEmployees.Location = new System.Drawing.Point(675, 26);
            this.Btn_ClearEmployees.Name = "Btn_ClearEmployees";
            this.Btn_ClearEmployees.Size = new System.Drawing.Size(30, 35);
            this.Btn_ClearEmployees.TabIndex = 71;
            this.Btn_ClearEmployees.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_ClearEmployees.UseVisualStyleBackColor = true;
            this.Btn_ClearEmployees.Click += new System.EventHandler(this.Btn_ClearEmployees_Click);
            // 
            // FiltroC3
            // 
            this.FiltroC3.FormattingEnabled = true;
            this.FiltroC3.Location = new System.Drawing.Point(486, 67);
            this.FiltroC3.Name = "FiltroC3";
            this.FiltroC3.Size = new System.Drawing.Size(219, 28);
            this.FiltroC3.TabIndex = 70;
            // 
            // FiltroC2
            // 
            this.FiltroC2.FormattingEnabled = true;
            this.FiltroC2.Location = new System.Drawing.Point(252, 67);
            this.FiltroC2.Name = "FiltroC2";
            this.FiltroC2.Size = new System.Drawing.Size(219, 28);
            this.FiltroC2.TabIndex = 69;
            // 
            // Btn_SearchEmployees
            // 
            this.Btn_SearchEmployees.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_SearchEmployees.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_SearchEmployees.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_SearchEmployees.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_SearchEmployees.Location = new System.Drawing.Point(568, 26);
            this.Btn_SearchEmployees.Name = "Btn_SearchEmployees";
            this.Btn_SearchEmployees.Size = new System.Drawing.Size(101, 35);
            this.Btn_SearchEmployees.TabIndex = 54;
            this.Btn_SearchEmployees.Text = "BUSCAR";
            this.Btn_SearchEmployees.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_SearchEmployees.UseVisualStyleBackColor = true;
            this.Btn_SearchEmployees.Click += new System.EventHandler(this.Btn_SearchEmployees_Click);
            // 
            // FiltroC1
            // 
            this.FiltroC1.FormattingEnabled = true;
            this.FiltroC1.Location = new System.Drawing.Point(16, 67);
            this.FiltroC1.Name = "FiltroC1";
            this.FiltroC1.Size = new System.Drawing.Size(219, 28);
            this.FiltroC1.TabIndex = 68;
            // 
            // Txt_ValorBuscado2
            // 
            this.Txt_ValorBuscado2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado2.Location = new System.Drawing.Point(16, 34);
            this.Txt_ValorBuscado2.MaxLength = 15;
            this.Txt_ValorBuscado2.Name = "Txt_ValorBuscado2";
            this.Txt_ValorBuscado2.Size = new System.Drawing.Size(540, 27);
            this.Txt_ValorBuscado2.TabIndex = 59;
            this.Txt_ValorBuscado2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado2_KeyDown);
            // 
            // Btn_DesbloquearUsuario
            // 
            this.Btn_DesbloquearUsuario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_DesbloquearUsuario.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_DesbloquearUsuario.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_DesbloquearUsuario.Location = new System.Drawing.Point(14, 517);
            this.Btn_DesbloquearUsuario.Name = "Btn_DesbloquearUsuario";
            this.Btn_DesbloquearUsuario.Size = new System.Drawing.Size(356, 30);
            this.Btn_DesbloquearUsuario.TabIndex = 79;
            this.Btn_DesbloquearUsuario.Text = "DESBLOQUEAR USUARIO";
            this.Btn_DesbloquearUsuario.UseVisualStyleBackColor = true;
            this.Btn_DesbloquearUsuario.Click += new System.EventHandler(this.Btn_DesbloquearUsuario_Click);
            // 
            // Frm_Users_Managment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.Panel_Derecho);
            this.Controls.Add(this.Panel_Izquierdo);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_Users_Managment";
            this.Text = "SECRON - GESTIÓN DE USUARIOS";
            this.Load += new System.EventHandler(this.Frm_Users_Managment_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Izquierdo.ResumeLayout(false);
            this.Panel_Izquierdo.PerformLayout();
            this.Panel_CRUD.ResumeLayout(false);
            this.Panel_Informacion.ResumeLayout(false);
            this.Panel_Informacion.PerformLayout();
            this.Panel_Derecho.ResumeLayout(false);
            this.PanelTabla1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla1)).EndInit();
            this.PanelToolStrip1.ResumeLayout(false);
            this.PanelToolStrip1.PerformLayout();
            this.Panel_Busqueda1.ResumeLayout(false);
            this.Panel_Busqueda1.PerformLayout();
            this.PanelTabla2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla2)).EndInit();
            this.PanelToolStrip2.ResumeLayout(false);
            this.PanelToolStrip2.PerformLayout();
            this.Panel_Busqueda2.ResumeLayout(false);
            this.Panel_Busqueda2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Button Btn_Export;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Panel Panel_Izquierdo;
        private System.Windows.Forms.Panel Panel_CRUD;
        private System.Windows.Forms.Button Btn_ClearInformacion;
        private System.Windows.Forms.Button Btn_Update;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.ComboBox ComboBox_Rol;
        private System.Windows.Forms.Panel Panel_Informacion;
        private System.Windows.Forms.TextBox Txt_Usuario;
        private System.Windows.Forms.Label Lbl_Rol;
        private System.Windows.Forms.Label Lbl_Usuario;
        private System.Windows.Forms.Label Lbl_Subtitulo1;
        private System.Windows.Forms.Label Lbl_TituloPanelIzquierdo;
        private System.Windows.Forms.Panel Panel_Derecho;
        private System.Windows.Forms.Panel PanelTabla2;
        private System.Windows.Forms.DataGridView Tabla2;
        private System.Windows.Forms.Panel PanelToolStrip2;
        private System.Windows.Forms.Label Lbl_PaginasColaboradores;
        private System.Windows.Forms.Panel Panel_Busqueda2;
        private System.Windows.Forms.Button Btn_ClearEmployees;
        private System.Windows.Forms.ComboBox FiltroC3;
        private System.Windows.Forms.ComboBox FiltroC2;
        private System.Windows.Forms.Button Btn_SearchEmployees;
        private System.Windows.Forms.ComboBox FiltroC1;
        private System.Windows.Forms.TextBox Txt_ValorBuscado2;
        private System.Windows.Forms.Label Lbl_Colaboradores;
        private System.Windows.Forms.Panel PanelTabla1;
        private System.Windows.Forms.DataGridView Tabla1;
        private System.Windows.Forms.Panel PanelToolStrip1;
        private System.Windows.Forms.Label Lbl_PaginasUsuarios;
        private System.Windows.Forms.Panel Panel_Busqueda1;
        private System.Windows.Forms.Label Lbl_Usuarios;
        private System.Windows.Forms.Button Btn_ClearUsers;
        private System.Windows.Forms.ComboBox FiltroU3;
        private System.Windows.Forms.ComboBox FiltroU2;
        private System.Windows.Forms.Button Btn_SearchUsers;
        private System.Windows.Forms.ComboBox FiltroU1;
        private System.Windows.Forms.TextBox Txt_ValorBuscado1;
        private System.Windows.Forms.TextBox Txt_Password;
        private System.Windows.Forms.Label Lbl_Password;
        private System.Windows.Forms.TextBox Txt_CorreoInstitucional;
        private System.Windows.Forms.Label Lbl_CorreoInstitucional;
        private System.Windows.Forms.CheckBox CheckBox_PasswordTemp;
        private System.Windows.Forms.TextBox Txt_Colaborador;
        private System.Windows.Forms.Label Lbl_Colaborador;
        private System.Windows.Forms.Button Btn_Send;
        private System.Windows.Forms.ComboBox ComboBox_UserStatus;
        private System.Windows.Forms.Label Lbl_UserStatus;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.ComboBox ComboBox_Bloqueado;
        private System.Windows.Forms.Label Lbl_Bloqueado;
        private System.Windows.Forms.Button Btn_ResetPassword;
        private System.Windows.Forms.Button Btn_DesbloquearUsuario;
    }
}