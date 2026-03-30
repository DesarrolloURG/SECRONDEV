using System;
using System.Globalization;

namespace SECRON.Views
{
    partial class Frm_Employees_Managment
    {
        private bool _cargandoEmpleado = false;

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

        //Evento para formatear los campos numerios a 2 decimales al perder el foco
        private void FormatearDecimal_Leave(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox txt = sender as System.Windows.Forms.TextBox;

            decimal valor = ConvertirDecimal(txt.Text);
            txt.Text = valor.ToString("0.00");
        }

        //Evento para habilitar o deshabilitar el campo de IGSS y recalcular el salario neto si se deshabilita
        private void Txt_igss_TextChanged(object sender, EventArgs e)
        {
            if (_cargandoEmpleado)
                return;

            if (chkb_IGSS.Checked)
                CalcularSalarioNeto();
        }

        //Evento para recalcular el salario base, IGSS e ISR cada vez que se modifique alguno de los campos relacionados con el salario
        private void CamposSalario_TextChanged(object sender, EventArgs e)
        {
            if (_cargandoEmpleado)
                return;

            CalcularSalarioBase();

            if (!chkb_IGSS.Checked)
                CalcularIGSS();

            CalcularSalarioNeto();
        }

        private void CalcularIGSS()
        {
            decimal salarioBase = ConvertirDecimal(Txt_salario_base.Text);

            decimal igss = salarioBase * 0.0483m;

            Txt_igss.Text = igss.ToString("0.00");
        }
        
        //Método para convertir un string a decimal, manejando el caso de que el string esté vacío o no sea un número válido
        private decimal ConvertirDecimal(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return 0m;

            valor = valor.Replace(",", ".");

            if (decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal resultado))
                return resultado;

            return 0m;
        }

        private void CalcularSalarioNeto()
        {
            decimal salarioBase = ConvertirDecimal(Txt_salario_base.Text);
            decimal bonoLey = ConvertirDecimal(Txt_bono_ley.Text);
            decimal bonoAdicional = ConvertirDecimal(Txt_bono_adicional.Text);
            decimal igss = ConvertirDecimal(Txt_igss.Text);
            decimal isr = ConvertirDecimal(Txt_ISR.Text);

            decimal salarioNeto = (salarioBase + bonoLey + bonoAdicional) - (igss + isr);

            Txt_salario_neto.Text = salarioNeto.ToString("0.00");
        }

        private void CalcularSalarioBase()
        {
            decimal salario = ConvertirDecimal(Txt_Salario.Text);
            decimal bonoAdicional = ConvertirDecimal(Txt_bono_adicional.Text);
            decimal bonoLey = ConvertirDecimal(Txt_bono_ley.Text);

            decimal salarioBase = salario - bonoAdicional - bonoLey;

            Txt_salario_base.Text = salarioBase.ToString("0.00");
        }

        private void chkb_IGSS_CheckedChanged(object sender, EventArgs e)
        {
            if (_cargandoEmpleado)
                return;

            Txt_igss.ReadOnly = !chkb_IGSS.Checked;

            if (!chkb_IGSS.Checked)
            {
                CalcularIGSS();
                CalcularSalarioNeto();
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Employees_Managment));
            this.Panel_Superior = new System.Windows.Forms.Panel();
            this.Btn_Import = new System.Windows.Forms.Button();
            this.Btn_Export = new System.Windows.Forms.Button();
            this.Lbl_Formulario = new System.Windows.Forms.Label();
            this.Panel_Izquierdo = new System.Windows.Forms.Panel();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Btn_Inactive = new System.Windows.Forms.Button();
            this.Btn_Update = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Panel_Emergencia = new System.Windows.Forms.Panel();
            this.Txt_TelefonoEmergencia = new System.Windows.Forms.TextBox();
            this.Txt_Parentesco = new System.Windows.Forms.TextBox();
            this.Txt_PersonaEmergencia = new System.Windows.Forms.TextBox();
            this.Lbl_TelefonoEmergencia = new System.Windows.Forms.Label();
            this.Lbl_Parentesco = new System.Windows.Forms.Label();
            this.Lbl_PersonaEmergencia = new System.Windows.Forms.Label();
            this.Lbl_Subtitulo4 = new System.Windows.Forms.Label();
            this.Panel_Laboral = new System.Windows.Forms.Panel();
            this.chkb_IGSS = new System.Windows.Forms.CheckBox();
            this.Txt_salario_base = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Txt_salario_neto = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Txt_ISR = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Txt_bono_ley = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Txt_bono_adicional = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Txt_igss = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ComboBox_Sede = new System.Windows.Forms.ComboBox();
            this.Lbl_Sede = new System.Windows.Forms.Label();
            this.ComboBox_TipoContratacion = new System.Windows.Forms.ComboBox();
            this.Lbl_Contratacion = new System.Windows.Forms.Label();
            this.ComboBox_Puesto = new System.Windows.Forms.ComboBox();
            this.ComboBox_Departamento = new System.Windows.Forms.ComboBox();
            this.ComboBox_Supervisor = new System.Windows.Forms.ComboBox();
            this.DTP_Ingreso = new System.Windows.Forms.DateTimePicker();
            this.Txt_Salario = new System.Windows.Forms.TextBox();
            this.Lbl_Supervisor = new System.Windows.Forms.Label();
            this.Lbl_Puesto = new System.Windows.Forms.Label();
            this.Lbl_Departamento = new System.Windows.Forms.Label();
            this.Lbl_Salario = new System.Windows.Forms.Label();
            this.Lbl_FechaIngreso = new System.Windows.Forms.Label();
            this.Lbl_Subtitulo3 = new System.Windows.Forms.Label();
            this.Panel_Contacto = new System.Windows.Forms.Panel();
            this.Txt_Direccion = new System.Windows.Forms.TextBox();
            this.Lbl_Direccion = new System.Windows.Forms.Label();
            this.Txt_Telefono2 = new System.Windows.Forms.TextBox();
            this.Lbl_Telefono2 = new System.Windows.Forms.Label();
            this.Txt_Telefono1 = new System.Windows.Forms.TextBox();
            this.Txt_CorreoInstitucional = new System.Windows.Forms.TextBox();
            this.Txt_CorreoPersonal = new System.Windows.Forms.TextBox();
            this.Lbl_Telefono1 = new System.Windows.Forms.Label();
            this.Lbl_CorreoInstitucional = new System.Windows.Forms.Label();
            this.Lbl_CorreoPersonal = new System.Windows.Forms.Label();
            this.Lbl_Subtitulo2 = new System.Windows.Forms.Label();
            this.Panel_Informacion = new System.Windows.Forms.Panel();
            this.DTP_Nacimiento = new System.Windows.Forms.DateTimePicker();
            this.Txt_Apellidos = new System.Windows.Forms.TextBox();
            this.Txt_Nombres = new System.Windows.Forms.TextBox();
            this.Txt_Dpi = new System.Windows.Forms.TextBox();
            this.Txt_Codigo = new System.Windows.Forms.TextBox();
            this.Lbl_FechaNacimiento = new System.Windows.Forms.Label();
            this.Lbl_Apellidos = new System.Windows.Forms.Label();
            this.Lbl_Nombres = new System.Windows.Forms.Label();
            this.Lbl_Dpi = new System.Windows.Forms.Label();
            this.Lbl_Codigo = new System.Windows.Forms.Label();
            this.Lbl_Subtitulo1 = new System.Windows.Forms.Label();
            this.Lbl_TituloPanelIzquierdo = new System.Windows.Forms.Label();
            this.Panel_Derecho = new System.Windows.Forms.Panel();
            this.PanelTabla = new System.Windows.Forms.Panel();
            this.Tabla = new System.Windows.Forms.DataGridView();
            this.PanelToolStrip = new System.Windows.Forms.Panel();
            this.Lbl_Paginas = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Btn_CleanSearch = new System.Windows.Forms.Button();
            this.Filtro3 = new System.Windows.Forms.ComboBox();
            this.Filtro2 = new System.Windows.Forms.ComboBox();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Filtro1 = new System.Windows.Forms.ComboBox();
            this.Txt_ValorBuscado = new System.Windows.Forms.TextBox();
            this.Panel_Superior.SuspendLayout();
            this.Panel_Izquierdo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.Panel_Emergencia.SuspendLayout();
            this.Panel_Laboral.SuspendLayout();
            this.Panel_Contacto.SuspendLayout();
            this.Panel_Informacion.SuspendLayout();
            this.Panel_Derecho.SuspendLayout();
            this.PanelTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).BeginInit();
            this.PanelToolStrip.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.Panel_Superior.TabIndex = 0;
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
            this.Lbl_Formulario.Size = new System.Drawing.Size(288, 25);
            this.Lbl_Formulario.TabIndex = 50;
            this.Lbl_Formulario.Text = "GESTIÓN DE COLABORADORES";
            // 
            // Panel_Izquierdo
            // 
            this.Panel_Izquierdo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Panel_Izquierdo.Controls.Add(this.vScrollBar);
            this.Panel_Izquierdo.Controls.Add(this.panel1);
            this.Panel_Izquierdo.Controls.Add(this.Panel_Emergencia);
            this.Panel_Izquierdo.Controls.Add(this.Panel_Laboral);
            this.Panel_Izquierdo.Controls.Add(this.Panel_Contacto);
            this.Panel_Izquierdo.Controls.Add(this.Panel_Informacion);
            this.Panel_Izquierdo.Controls.Add(this.Lbl_TituloPanelIzquierdo);
            this.Panel_Izquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.Panel_Izquierdo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Izquierdo.ForeColor = System.Drawing.Color.Black;
            this.Panel_Izquierdo.Location = new System.Drawing.Point(0, 55);
            this.Panel_Izquierdo.Name = "Panel_Izquierdo";
            this.Panel_Izquierdo.Size = new System.Drawing.Size(415, 806);
            this.Panel_Izquierdo.TabIndex = 1;
            this.Panel_Izquierdo.MouseEnter += new System.EventHandler(this.Panel_Izquierdo_MouseEnter);
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(405, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(10, 806);
            this.vScrollBar.TabIndex = 76;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.Btn_Clear);
            this.panel1.Controls.Add(this.Btn_Inactive);
            this.panel1.Controls.Add(this.Btn_Update);
            this.panel1.Controls.Add(this.Btn_Save);
            this.panel1.Location = new System.Drawing.Point(16, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(384, 47);
            this.panel1.TabIndex = 75;
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Clear.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_Clear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Clear.Location = new System.Drawing.Point(350, 3);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(33, 37);
            this.Btn_Clear.TabIndex = 57;
            this.Btn_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Clear.UseVisualStyleBackColor = true;
            this.Btn_Clear.Click += new System.EventHandler(this.Btn_Clear_Click);
            // 
            // Btn_Inactive
            // 
            this.Btn_Inactive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Inactive.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Inactive.Image = global::SECRON.Properties.Resources.InactivarRojo25x25;
            this.Btn_Inactive.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Inactive.Location = new System.Drawing.Point(224, 3);
            this.Btn_Inactive.Name = "Btn_Inactive";
            this.Btn_Inactive.Size = new System.Drawing.Size(124, 37);
            this.Btn_Inactive.TabIndex = 56;
            this.Btn_Inactive.Text = "INACTIVAR";
            this.Btn_Inactive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Inactive.UseVisualStyleBackColor = true;
            this.Btn_Inactive.Click += new System.EventHandler(this.Btn_Inactive_Click);
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
            // Panel_Emergencia
            // 
            this.Panel_Emergencia.BackColor = System.Drawing.Color.White;
            this.Panel_Emergencia.Controls.Add(this.Txt_TelefonoEmergencia);
            this.Panel_Emergencia.Controls.Add(this.Txt_Parentesco);
            this.Panel_Emergencia.Controls.Add(this.Txt_PersonaEmergencia);
            this.Panel_Emergencia.Controls.Add(this.Lbl_TelefonoEmergencia);
            this.Panel_Emergencia.Controls.Add(this.Lbl_Parentesco);
            this.Panel_Emergencia.Controls.Add(this.Lbl_PersonaEmergencia);
            this.Panel_Emergencia.Controls.Add(this.Lbl_Subtitulo4);
            this.Panel_Emergencia.Location = new System.Drawing.Point(16, 1286);
            this.Panel_Emergencia.Name = "Panel_Emergencia";
            this.Panel_Emergencia.Size = new System.Drawing.Size(380, 175);
            this.Panel_Emergencia.TabIndex = 74;
            // 
            // Txt_TelefonoEmergencia
            // 
            this.Txt_TelefonoEmergencia.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_TelefonoEmergencia.Location = new System.Drawing.Point(200, 134);
            this.Txt_TelefonoEmergencia.MaxLength = 15;
            this.Txt_TelefonoEmergencia.Name = "Txt_TelefonoEmergencia";
            this.Txt_TelefonoEmergencia.Size = new System.Drawing.Size(166, 27);
            this.Txt_TelefonoEmergencia.TabIndex = 69;
            // 
            // Txt_Parentesco
            // 
            this.Txt_Parentesco.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Parentesco.Location = new System.Drawing.Point(14, 134);
            this.Txt_Parentesco.MaxLength = 15;
            this.Txt_Parentesco.Name = "Txt_Parentesco";
            this.Txt_Parentesco.Size = new System.Drawing.Size(170, 27);
            this.Txt_Parentesco.TabIndex = 68;
            // 
            // Txt_PersonaEmergencia
            // 
            this.Txt_PersonaEmergencia.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_PersonaEmergencia.Location = new System.Drawing.Point(14, 68);
            this.Txt_PersonaEmergencia.MaxLength = 15;
            this.Txt_PersonaEmergencia.Name = "Txt_PersonaEmergencia";
            this.Txt_PersonaEmergencia.Size = new System.Drawing.Size(352, 27);
            this.Txt_PersonaEmergencia.TabIndex = 67;
            // 
            // Lbl_TelefonoEmergencia
            // 
            this.Lbl_TelefonoEmergencia.AutoSize = true;
            this.Lbl_TelefonoEmergencia.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_TelefonoEmergencia.ForeColor = System.Drawing.Color.Black;
            this.Lbl_TelefonoEmergencia.Location = new System.Drawing.Point(196, 111);
            this.Lbl_TelefonoEmergencia.Name = "Lbl_TelefonoEmergencia";
            this.Lbl_TelefonoEmergencia.Size = new System.Drawing.Size(84, 20);
            this.Lbl_TelefonoEmergencia.TabIndex = 58;
            this.Lbl_TelefonoEmergencia.Text = "TELÉFONO";
            // 
            // Lbl_Parentesco
            // 
            this.Lbl_Parentesco.AutoSize = true;
            this.Lbl_Parentesco.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Parentesco.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Parentesco.Location = new System.Drawing.Point(10, 111);
            this.Lbl_Parentesco.Name = "Lbl_Parentesco";
            this.Lbl_Parentesco.Size = new System.Drawing.Size(103, 20);
            this.Lbl_Parentesco.TabIndex = 56;
            this.Lbl_Parentesco.Text = "PARENTESCO";
            // 
            // Lbl_PersonaEmergencia
            // 
            this.Lbl_PersonaEmergencia.AutoSize = true;
            this.Lbl_PersonaEmergencia.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_PersonaEmergencia.ForeColor = System.Drawing.Color.Black;
            this.Lbl_PersonaEmergencia.Location = new System.Drawing.Point(10, 45);
            this.Lbl_PersonaEmergencia.Name = "Lbl_PersonaEmergencia";
            this.Lbl_PersonaEmergencia.Size = new System.Drawing.Size(156, 20);
            this.Lbl_PersonaEmergencia.TabIndex = 54;
            this.Lbl_PersonaEmergencia.Text = "NOMBRE COMPLETO";
            // 
            // Lbl_Subtitulo4
            // 
            this.Lbl_Subtitulo4.AutoSize = true;
            this.Lbl_Subtitulo4.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo4.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo4.Image = global::SECRON.Properties.Resources.AlertaNegro25x25;
            this.Lbl_Subtitulo4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo4.Location = new System.Drawing.Point(10, 10);
            this.Lbl_Subtitulo4.Name = "Lbl_Subtitulo4";
            this.Lbl_Subtitulo4.Size = new System.Drawing.Size(235, 20);
            this.Lbl_Subtitulo4.TabIndex = 53;
            this.Lbl_Subtitulo4.Text = "      CONTACTO DE EMERGENCIA";
            this.Lbl_Subtitulo4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Panel_Laboral
            // 
            this.Panel_Laboral.BackColor = System.Drawing.Color.White;
            this.Panel_Laboral.Controls.Add(this.chkb_IGSS);
            this.Panel_Laboral.Controls.Add(this.Txt_salario_base);
            this.Panel_Laboral.Controls.Add(this.label6);
            this.Panel_Laboral.Controls.Add(this.Txt_salario_neto);
            this.Panel_Laboral.Controls.Add(this.label5);
            this.Panel_Laboral.Controls.Add(this.Txt_ISR);
            this.Panel_Laboral.Controls.Add(this.label4);
            this.Panel_Laboral.Controls.Add(this.Txt_bono_ley);
            this.Panel_Laboral.Controls.Add(this.label3);
            this.Panel_Laboral.Controls.Add(this.Txt_bono_adicional);
            this.Panel_Laboral.Controls.Add(this.label2);
            this.Panel_Laboral.Controls.Add(this.Txt_igss);
            this.Panel_Laboral.Controls.Add(this.label1);
            this.Panel_Laboral.Controls.Add(this.ComboBox_Sede);
            this.Panel_Laboral.Controls.Add(this.Lbl_Sede);
            this.Panel_Laboral.Controls.Add(this.ComboBox_TipoContratacion);
            this.Panel_Laboral.Controls.Add(this.Lbl_Contratacion);
            this.Panel_Laboral.Controls.Add(this.ComboBox_Puesto);
            this.Panel_Laboral.Controls.Add(this.ComboBox_Departamento);
            this.Panel_Laboral.Controls.Add(this.ComboBox_Supervisor);
            this.Panel_Laboral.Controls.Add(this.DTP_Ingreso);
            this.Panel_Laboral.Controls.Add(this.Txt_Salario);
            this.Panel_Laboral.Controls.Add(this.Lbl_Supervisor);
            this.Panel_Laboral.Controls.Add(this.Lbl_Puesto);
            this.Panel_Laboral.Controls.Add(this.Lbl_Departamento);
            this.Panel_Laboral.Controls.Add(this.Lbl_Salario);
            this.Panel_Laboral.Controls.Add(this.Lbl_FechaIngreso);
            this.Panel_Laboral.Controls.Add(this.Lbl_Subtitulo3);
            this.Panel_Laboral.Location = new System.Drawing.Point(16, 685);
            this.Panel_Laboral.Name = "Panel_Laboral";
            this.Panel_Laboral.Size = new System.Drawing.Size(380, 595);
            this.Panel_Laboral.TabIndex = 65;
            // 
            // chkb_IGSS
            // 
            this.chkb_IGSS.AutoSize = true;
            this.chkb_IGSS.Location = new System.Drawing.Point(199, 271);
            this.chkb_IGSS.Name = "chkb_IGSS";
            this.chkb_IGSS.Size = new System.Drawing.Size(178, 24);
            this.chkb_IGSS.TabIndex = 85;
            this.chkb_IGSS.Text = "Ingresar IGSS manual";
            this.chkb_IGSS.UseVisualStyleBackColor = true;
            this.chkb_IGSS.CheckedChanged += new System.EventHandler(this.chkb_IGSS_CheckedChanged);
            // 
            // Txt_salario_base
            // 
            this.Txt_salario_base.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_salario_base.Location = new System.Drawing.Point(14, 238);
            this.Txt_salario_base.MaxLength = 15;
            this.Txt_salario_base.Name = "Txt_salario_base";
            this.Txt_salario_base.Size = new System.Drawing.Size(166, 27);
            this.Txt_salario_base.TabIndex = 84;
            this.Txt_salario_base.Text = "0.00";
            this.Txt_salario_base.TextChanged += new System.EventHandler(this.CamposSalario_TextChanged);
            this.Txt_salario_base.Leave += new System.EventHandler(this.FormatearDecimal_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(11, 215);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 20);
            this.label6.TabIndex = 83;
            this.label6.Text = "SALARIO BASE";
            // 
            // Txt_salario_neto
            // 
            this.Txt_salario_neto.Enabled = false;
            this.Txt_salario_neto.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_salario_neto.Location = new System.Drawing.Point(15, 302);
            this.Txt_salario_neto.MaxLength = 15;
            this.Txt_salario_neto.Name = "Txt_salario_neto";
            this.Txt_salario_neto.Size = new System.Drawing.Size(351, 27);
            this.Txt_salario_neto.TabIndex = 18;
            this.Txt_salario_neto.Text = "0.00";
            this.Txt_salario_neto.Leave += new System.EventHandler(this.FormatearDecimal_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(11, 279);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 20);
            this.label5.TabIndex = 81;
            this.label5.Text = "SALARIO NETO";
            // 
            // Txt_ISR
            // 
            this.Txt_ISR.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ISR.Location = new System.Drawing.Point(200, 184);
            this.Txt_ISR.MaxLength = 15;
            this.Txt_ISR.Name = "Txt_ISR";
            this.Txt_ISR.Size = new System.Drawing.Size(166, 27);
            this.Txt_ISR.TabIndex = 17;
            this.Txt_ISR.Text = "0.00";
            this.Txt_ISR.TextChanged += new System.EventHandler(this.CamposSalario_TextChanged);
            this.Txt_ISR.Leave += new System.EventHandler(this.FormatearDecimal_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(196, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 20);
            this.label4.TabIndex = 79;
            this.label4.Text = "ISR";
            // 
            // Txt_bono_ley
            // 
            this.Txt_bono_ley.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_bono_ley.Location = new System.Drawing.Point(200, 130);
            this.Txt_bono_ley.MaxLength = 15;
            this.Txt_bono_ley.Name = "Txt_bono_ley";
            this.Txt_bono_ley.Size = new System.Drawing.Size(166, 27);
            this.Txt_bono_ley.TabIndex = 15;
            this.Txt_bono_ley.Text = "250.00";
            this.Txt_bono_ley.TextChanged += new System.EventHandler(this.CamposSalario_TextChanged);
            this.Txt_bono_ley.Leave += new System.EventHandler(this.FormatearDecimal_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(196, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 20);
            this.label3.TabIndex = 77;
            this.label3.Text = "BONO DE LEY";
            // 
            // Txt_bono_adicional
            // 
            this.Txt_bono_adicional.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_bono_adicional.Location = new System.Drawing.Point(15, 184);
            this.Txt_bono_adicional.MaxLength = 15;
            this.Txt_bono_adicional.Name = "Txt_bono_adicional";
            this.Txt_bono_adicional.Size = new System.Drawing.Size(166, 27);
            this.Txt_bono_adicional.TabIndex = 14;
            this.Txt_bono_adicional.Text = "0.00";
            this.Txt_bono_adicional.TextChanged += new System.EventHandler(this.CamposSalario_TextChanged);
            this.Txt_bono_adicional.Leave += new System.EventHandler(this.FormatearDecimal_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(11, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 20);
            this.label2.TabIndex = 75;
            this.label2.Text = "BONO ADICIONAL";
            // 
            // Txt_igss
            // 
            this.Txt_igss.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_igss.Location = new System.Drawing.Point(200, 238);
            this.Txt_igss.MaxLength = 15;
            this.Txt_igss.Name = "Txt_igss";
            this.Txt_igss.ReadOnly = true;
            this.Txt_igss.Size = new System.Drawing.Size(166, 27);
            this.Txt_igss.TabIndex = 16;
            this.Txt_igss.Text = "0.00";
            this.Txt_igss.TextChanged += new System.EventHandler(this.CamposSalario_TextChanged);
            this.Txt_igss.Leave += new System.EventHandler(this.FormatearDecimal_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(196, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 20);
            this.label1.TabIndex = 73;
            this.label1.Text = "IGSS";
            // 
            // ComboBox_Sede
            // 
            this.ComboBox_Sede.FormattingEnabled = true;
            this.ComboBox_Sede.Location = new System.Drawing.Point(14, 555);
            this.ComboBox_Sede.Name = "ComboBox_Sede";
            this.ComboBox_Sede.Size = new System.Drawing.Size(352, 28);
            this.ComboBox_Sede.TabIndex = 71;
            // 
            // Lbl_Sede
            // 
            this.Lbl_Sede.AutoSize = true;
            this.Lbl_Sede.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Sede.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Sede.Location = new System.Drawing.Point(10, 528);
            this.Lbl_Sede.Name = "Lbl_Sede";
            this.Lbl_Sede.Size = new System.Drawing.Size(44, 20);
            this.Lbl_Sede.TabIndex = 70;
            this.Lbl_Sede.Text = "SEDE";
            // 
            // ComboBox_TipoContratacion
            // 
            this.ComboBox_TipoContratacion.FormattingEnabled = true;
            this.ComboBox_TipoContratacion.Location = new System.Drawing.Point(14, 491);
            this.ComboBox_TipoContratacion.Name = "ComboBox_TipoContratacion";
            this.ComboBox_TipoContratacion.Size = new System.Drawing.Size(352, 28);
            this.ComboBox_TipoContratacion.TabIndex = 69;
            // 
            // Lbl_Contratacion
            // 
            this.Lbl_Contratacion.AutoSize = true;
            this.Lbl_Contratacion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Contratacion.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Contratacion.Location = new System.Drawing.Point(10, 468);
            this.Lbl_Contratacion.Name = "Lbl_Contratacion";
            this.Lbl_Contratacion.Size = new System.Drawing.Size(187, 20);
            this.Lbl_Contratacion.TabIndex = 68;
            this.Lbl_Contratacion.Text = "TIPO DE CONTRATACIÓN";
            // 
            // ComboBox_Puesto
            // 
            this.ComboBox_Puesto.FormattingEnabled = true;
            this.ComboBox_Puesto.Location = new System.Drawing.Point(200, 377);
            this.ComboBox_Puesto.Name = "ComboBox_Puesto";
            this.ComboBox_Puesto.Size = new System.Drawing.Size(166, 28);
            this.ComboBox_Puesto.TabIndex = 67;
            // 
            // ComboBox_Departamento
            // 
            this.ComboBox_Departamento.FormattingEnabled = true;
            this.ComboBox_Departamento.Location = new System.Drawing.Point(14, 377);
            this.ComboBox_Departamento.Name = "ComboBox_Departamento";
            this.ComboBox_Departamento.Size = new System.Drawing.Size(170, 28);
            this.ComboBox_Departamento.TabIndex = 66;
            // 
            // ComboBox_Supervisor
            // 
            this.ComboBox_Supervisor.FormattingEnabled = true;
            this.ComboBox_Supervisor.Location = new System.Drawing.Point(14, 433);
            this.ComboBox_Supervisor.Name = "ComboBox_Supervisor";
            this.ComboBox_Supervisor.Size = new System.Drawing.Size(352, 28);
            this.ComboBox_Supervisor.TabIndex = 65;
            // 
            // DTP_Ingreso
            // 
            this.DTP_Ingreso.Location = new System.Drawing.Point(14, 68);
            this.DTP_Ingreso.Name = "DTP_Ingreso";
            this.DTP_Ingreso.Size = new System.Drawing.Size(352, 27);
            this.DTP_Ingreso.TabIndex = 11;
            // 
            // Txt_Salario
            // 
            this.Txt_Salario.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Salario.Location = new System.Drawing.Point(14, 130);
            this.Txt_Salario.MaxLength = 15;
            this.Txt_Salario.Name = "Txt_Salario";
            this.Txt_Salario.Size = new System.Drawing.Size(166, 27);
            this.Txt_Salario.TabIndex = 12;
            this.Txt_Salario.Text = "0.00";
            this.Txt_Salario.TextChanged += new System.EventHandler(this.CamposSalario_TextChanged);
            this.Txt_Salario.Leave += new System.EventHandler(this.FormatearDecimal_Leave);
            // 
            // Lbl_Supervisor
            // 
            this.Lbl_Supervisor.AutoSize = true;
            this.Lbl_Supervisor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Supervisor.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Supervisor.Location = new System.Drawing.Point(10, 409);
            this.Lbl_Supervisor.Name = "Lbl_Supervisor";
            this.Lbl_Supervisor.Size = new System.Drawing.Size(165, 20);
            this.Lbl_Supervisor.TabIndex = 58;
            this.Lbl_Supervisor.Text = "SUPERVISOR DIRECTO";
            // 
            // Lbl_Puesto
            // 
            this.Lbl_Puesto.AutoSize = true;
            this.Lbl_Puesto.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Puesto.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Puesto.Location = new System.Drawing.Point(196, 350);
            this.Lbl_Puesto.Name = "Lbl_Puesto";
            this.Lbl_Puesto.Size = new System.Drawing.Size(75, 20);
            this.Lbl_Puesto.TabIndex = 57;
            this.Lbl_Puesto.Text = "PUESTO *";
            // 
            // Lbl_Departamento
            // 
            this.Lbl_Departamento.AutoSize = true;
            this.Lbl_Departamento.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Departamento.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Departamento.Location = new System.Drawing.Point(10, 350);
            this.Lbl_Departamento.Name = "Lbl_Departamento";
            this.Lbl_Departamento.Size = new System.Drawing.Size(153, 20);
            this.Lbl_Departamento.TabIndex = 56;
            this.Lbl_Departamento.Text = "ÁREA ACADÉMICA *";
            // 
            // Lbl_Salario
            // 
            this.Lbl_Salario.AutoSize = true;
            this.Lbl_Salario.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Salario.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Salario.Location = new System.Drawing.Point(10, 107);
            this.Lbl_Salario.Name = "Lbl_Salario";
            this.Lbl_Salario.Size = new System.Drawing.Size(150, 20);
            this.Lbl_Salario.TabIndex = 12;
            this.Lbl_Salario.Text = "SALARIO NOMINAL";
            // 
            // Lbl_FechaIngreso
            // 
            this.Lbl_FechaIngreso.AutoSize = true;
            this.Lbl_FechaIngreso.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_FechaIngreso.ForeColor = System.Drawing.Color.Black;
            this.Lbl_FechaIngreso.Location = new System.Drawing.Point(10, 45);
            this.Lbl_FechaIngreso.Name = "Lbl_FechaIngreso";
            this.Lbl_FechaIngreso.Size = new System.Drawing.Size(159, 20);
            this.Lbl_FechaIngreso.TabIndex = 11;
            this.Lbl_FechaIngreso.Text = "FECHA DE INGRESO *";
            // 
            // Lbl_Subtitulo3
            // 
            this.Lbl_Subtitulo3.AutoSize = true;
            this.Lbl_Subtitulo3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo3.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo3.Image = global::SECRON.Properties.Resources.InformacionLaboralNegro25x25;
            this.Lbl_Subtitulo3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo3.Location = new System.Drawing.Point(10, 10);
            this.Lbl_Subtitulo3.Name = "Lbl_Subtitulo3";
            this.Lbl_Subtitulo3.Size = new System.Drawing.Size(214, 20);
            this.Lbl_Subtitulo3.TabIndex = 11;
            this.Lbl_Subtitulo3.Text = "      INFORMACIÓN LABORAL";
            this.Lbl_Subtitulo3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Panel_Contacto
            // 
            this.Panel_Contacto.BackColor = System.Drawing.Color.White;
            this.Panel_Contacto.Controls.Add(this.Txt_Direccion);
            this.Panel_Contacto.Controls.Add(this.Lbl_Direccion);
            this.Panel_Contacto.Controls.Add(this.Txt_Telefono2);
            this.Panel_Contacto.Controls.Add(this.Lbl_Telefono2);
            this.Panel_Contacto.Controls.Add(this.Txt_Telefono1);
            this.Panel_Contacto.Controls.Add(this.Txt_CorreoInstitucional);
            this.Panel_Contacto.Controls.Add(this.Txt_CorreoPersonal);
            this.Panel_Contacto.Controls.Add(this.Lbl_Telefono1);
            this.Panel_Contacto.Controls.Add(this.Lbl_CorreoInstitucional);
            this.Panel_Contacto.Controls.Add(this.Lbl_CorreoPersonal);
            this.Panel_Contacto.Controls.Add(this.Lbl_Subtitulo2);
            this.Panel_Contacto.Location = new System.Drawing.Point(16, 351);
            this.Panel_Contacto.Name = "Panel_Contacto";
            this.Panel_Contacto.Size = new System.Drawing.Size(380, 324);
            this.Panel_Contacto.TabIndex = 64;
            // 
            // Txt_Direccion
            // 
            this.Txt_Direccion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Direccion.Location = new System.Drawing.Point(14, 264);
            this.Txt_Direccion.MaxLength = 15;
            this.Txt_Direccion.Multiline = true;
            this.Txt_Direccion.Name = "Txt_Direccion";
            this.Txt_Direccion.Size = new System.Drawing.Size(352, 42);
            this.Txt_Direccion.TabIndex = 10;
            this.Txt_Direccion.Text = "DIRECCIÓN COMPLETA";
            // 
            // Lbl_Direccion
            // 
            this.Lbl_Direccion.AutoSize = true;
            this.Lbl_Direccion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Direccion.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Direccion.Location = new System.Drawing.Point(10, 241);
            this.Lbl_Direccion.Name = "Lbl_Direccion";
            this.Lbl_Direccion.Size = new System.Drawing.Size(89, 20);
            this.Lbl_Direccion.TabIndex = 10;
            this.Lbl_Direccion.Text = "DIRECCIÓN";
            // 
            // Txt_Telefono2
            // 
            this.Txt_Telefono2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Telefono2.Location = new System.Drawing.Point(196, 200);
            this.Txt_Telefono2.MaxLength = 15;
            this.Txt_Telefono2.Name = "Txt_Telefono2";
            this.Txt_Telefono2.Size = new System.Drawing.Size(170, 27);
            this.Txt_Telefono2.TabIndex = 9;
            // 
            // Lbl_Telefono2
            // 
            this.Lbl_Telefono2.AutoSize = true;
            this.Lbl_Telefono2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Telefono2.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Telefono2.Location = new System.Drawing.Point(192, 177);
            this.Lbl_Telefono2.Name = "Lbl_Telefono2";
            this.Lbl_Telefono2.Size = new System.Drawing.Size(97, 20);
            this.Lbl_Telefono2.TabIndex = 9;
            this.Lbl_Telefono2.Text = "TELÉFONO 2";
            // 
            // Txt_Telefono1
            // 
            this.Txt_Telefono1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Telefono1.Location = new System.Drawing.Point(14, 200);
            this.Txt_Telefono1.MaxLength = 15;
            this.Txt_Telefono1.Name = "Txt_Telefono1";
            this.Txt_Telefono1.Size = new System.Drawing.Size(170, 27);
            this.Txt_Telefono1.TabIndex = 8;
            // 
            // Txt_CorreoInstitucional
            // 
            this.Txt_CorreoInstitucional.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_CorreoInstitucional.Location = new System.Drawing.Point(14, 134);
            this.Txt_CorreoInstitucional.MaxLength = 15;
            this.Txt_CorreoInstitucional.Name = "Txt_CorreoInstitucional";
            this.Txt_CorreoInstitucional.Size = new System.Drawing.Size(352, 27);
            this.Txt_CorreoInstitucional.TabIndex = 7;
            // 
            // Txt_CorreoPersonal
            // 
            this.Txt_CorreoPersonal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_CorreoPersonal.Location = new System.Drawing.Point(14, 68);
            this.Txt_CorreoPersonal.MaxLength = 15;
            this.Txt_CorreoPersonal.Name = "Txt_CorreoPersonal";
            this.Txt_CorreoPersonal.Size = new System.Drawing.Size(352, 27);
            this.Txt_CorreoPersonal.TabIndex = 6;
            // 
            // Lbl_Telefono1
            // 
            this.Lbl_Telefono1.AutoSize = true;
            this.Lbl_Telefono1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Telefono1.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Telefono1.Location = new System.Drawing.Point(10, 177);
            this.Lbl_Telefono1.Name = "Lbl_Telefono1";
            this.Lbl_Telefono1.Size = new System.Drawing.Size(84, 20);
            this.Lbl_Telefono1.TabIndex = 8;
            this.Lbl_Telefono1.Text = "TELÉFONO";
            // 
            // Lbl_CorreoInstitucional
            // 
            this.Lbl_CorreoInstitucional.AutoSize = true;
            this.Lbl_CorreoInstitucional.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_CorreoInstitucional.ForeColor = System.Drawing.Color.Black;
            this.Lbl_CorreoInstitucional.Location = new System.Drawing.Point(10, 111);
            this.Lbl_CorreoInstitucional.Name = "Lbl_CorreoInstitucional";
            this.Lbl_CorreoInstitucional.Size = new System.Drawing.Size(198, 20);
            this.Lbl_CorreoInstitucional.TabIndex = 7;
            this.Lbl_CorreoInstitucional.Text = "CORREO INSTITUCIONAL *";
            // 
            // Lbl_CorreoPersonal
            // 
            this.Lbl_CorreoPersonal.AutoSize = true;
            this.Lbl_CorreoPersonal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_CorreoPersonal.ForeColor = System.Drawing.Color.Black;
            this.Lbl_CorreoPersonal.Location = new System.Drawing.Point(10, 45);
            this.Lbl_CorreoPersonal.Name = "Lbl_CorreoPersonal";
            this.Lbl_CorreoPersonal.Size = new System.Drawing.Size(149, 20);
            this.Lbl_CorreoPersonal.TabIndex = 6;
            this.Lbl_CorreoPersonal.Text = "CORREO PERSONAL";
            // 
            // Lbl_Subtitulo2
            // 
            this.Lbl_Subtitulo2.AutoSize = true;
            this.Lbl_Subtitulo2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Subtitulo2.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Subtitulo2.Image = global::SECRON.Properties.Resources.ContactoNegro25x25;
            this.Lbl_Subtitulo2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Lbl_Subtitulo2.Location = new System.Drawing.Point(10, 10);
            this.Lbl_Subtitulo2.Name = "Lbl_Subtitulo2";
            this.Lbl_Subtitulo2.Size = new System.Drawing.Size(247, 20);
            this.Lbl_Subtitulo2.TabIndex = 6;
            this.Lbl_Subtitulo2.Text = "      INFORMACIÓN DE CONTACTO";
            this.Lbl_Subtitulo2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Panel_Informacion
            // 
            this.Panel_Informacion.BackColor = System.Drawing.Color.White;
            this.Panel_Informacion.Controls.Add(this.DTP_Nacimiento);
            this.Panel_Informacion.Controls.Add(this.Txt_Apellidos);
            this.Panel_Informacion.Controls.Add(this.Txt_Nombres);
            this.Panel_Informacion.Controls.Add(this.Txt_Dpi);
            this.Panel_Informacion.Controls.Add(this.Txt_Codigo);
            this.Panel_Informacion.Controls.Add(this.Lbl_FechaNacimiento);
            this.Panel_Informacion.Controls.Add(this.Lbl_Apellidos);
            this.Panel_Informacion.Controls.Add(this.Lbl_Nombres);
            this.Panel_Informacion.Controls.Add(this.Lbl_Dpi);
            this.Panel_Informacion.Controls.Add(this.Lbl_Codigo);
            this.Panel_Informacion.Controls.Add(this.Lbl_Subtitulo1);
            this.Panel_Informacion.Location = new System.Drawing.Point(16, 100);
            this.Panel_Informacion.Name = "Panel_Informacion";
            this.Panel_Informacion.Size = new System.Drawing.Size(380, 241);
            this.Panel_Informacion.TabIndex = 52;
            // 
            // DTP_Nacimiento
            // 
            this.DTP_Nacimiento.Location = new System.Drawing.Point(14, 200);
            this.DTP_Nacimiento.Name = "DTP_Nacimiento";
            this.DTP_Nacimiento.Size = new System.Drawing.Size(352, 27);
            this.DTP_Nacimiento.TabIndex = 5;
            // 
            // Txt_Apellidos
            // 
            this.Txt_Apellidos.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Apellidos.Location = new System.Drawing.Point(196, 134);
            this.Txt_Apellidos.MaxLength = 15;
            this.Txt_Apellidos.Name = "Txt_Apellidos";
            this.Txt_Apellidos.Size = new System.Drawing.Size(170, 27);
            this.Txt_Apellidos.TabIndex = 4;
            // 
            // Txt_Nombres
            // 
            this.Txt_Nombres.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Nombres.Location = new System.Drawing.Point(14, 134);
            this.Txt_Nombres.MaxLength = 15;
            this.Txt_Nombres.Name = "Txt_Nombres";
            this.Txt_Nombres.Size = new System.Drawing.Size(170, 27);
            this.Txt_Nombres.TabIndex = 3;
            // 
            // Txt_Dpi
            // 
            this.Txt_Dpi.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Dpi.Location = new System.Drawing.Point(196, 68);
            this.Txt_Dpi.MaxLength = 15;
            this.Txt_Dpi.Name = "Txt_Dpi";
            this.Txt_Dpi.Size = new System.Drawing.Size(170, 27);
            this.Txt_Dpi.TabIndex = 2;
            // 
            // Txt_Codigo
            // 
            this.Txt_Codigo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_Codigo.Location = new System.Drawing.Point(14, 68);
            this.Txt_Codigo.MaxLength = 15;
            this.Txt_Codigo.Name = "Txt_Codigo";
            this.Txt_Codigo.Size = new System.Drawing.Size(170, 27);
            this.Txt_Codigo.TabIndex = 1;
            // 
            // Lbl_FechaNacimiento
            // 
            this.Lbl_FechaNacimiento.AutoSize = true;
            this.Lbl_FechaNacimiento.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_FechaNacimiento.ForeColor = System.Drawing.Color.Black;
            this.Lbl_FechaNacimiento.Location = new System.Drawing.Point(10, 177);
            this.Lbl_FechaNacimiento.Name = "Lbl_FechaNacimiento";
            this.Lbl_FechaNacimiento.Size = new System.Drawing.Size(189, 20);
            this.Lbl_FechaNacimiento.TabIndex = 5;
            this.Lbl_FechaNacimiento.Text = "FECHA DE NACIMIENTO *";
            // 
            // Lbl_Apellidos
            // 
            this.Lbl_Apellidos.AutoSize = true;
            this.Lbl_Apellidos.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Apellidos.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Apellidos.Location = new System.Drawing.Point(196, 111);
            this.Lbl_Apellidos.Name = "Lbl_Apellidos";
            this.Lbl_Apellidos.Size = new System.Drawing.Size(99, 20);
            this.Lbl_Apellidos.TabIndex = 4;
            this.Lbl_Apellidos.Text = "APELLIDOS *";
            // 
            // Lbl_Nombres
            // 
            this.Lbl_Nombres.AutoSize = true;
            this.Lbl_Nombres.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Nombres.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Nombres.Location = new System.Drawing.Point(10, 111);
            this.Lbl_Nombres.Name = "Lbl_Nombres";
            this.Lbl_Nombres.Size = new System.Drawing.Size(93, 20);
            this.Lbl_Nombres.TabIndex = 3;
            this.Lbl_Nombres.Text = "NOMBRES *";
            // 
            // Lbl_Dpi
            // 
            this.Lbl_Dpi.AutoSize = true;
            this.Lbl_Dpi.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Dpi.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Dpi.Location = new System.Drawing.Point(196, 45);
            this.Lbl_Dpi.Name = "Lbl_Dpi";
            this.Lbl_Dpi.Size = new System.Drawing.Size(110, 20);
            this.Lbl_Dpi.TabIndex = 2;
            this.Lbl_Dpi.Text = "DPI/CÉDULA *";
            // 
            // Lbl_Codigo
            // 
            this.Lbl_Codigo.AutoSize = true;
            this.Lbl_Codigo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Lbl_Codigo.ForeColor = System.Drawing.Color.Black;
            this.Lbl_Codigo.Location = new System.Drawing.Point(10, 45);
            this.Lbl_Codigo.Name = "Lbl_Codigo";
            this.Lbl_Codigo.Size = new System.Drawing.Size(151, 20);
            this.Lbl_Codigo.TabIndex = 1;
            this.Lbl_Codigo.Text = "CÓDIGO EMPLEADO";
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
            this.Lbl_Subtitulo1.Size = new System.Drawing.Size(289, 20);
            this.Lbl_Subtitulo1.TabIndex = 1;
            this.Lbl_Subtitulo1.Text = "      INFORMACIÓN DEL COLABORADOR";
            this.Lbl_Subtitulo1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Lbl_TituloPanelIzquierdo
            // 
            this.Lbl_TituloPanelIzquierdo.AutoSize = true;
            this.Lbl_TituloPanelIzquierdo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Lbl_TituloPanelIzquierdo.ForeColor = System.Drawing.Color.Black;
            this.Lbl_TituloPanelIzquierdo.Location = new System.Drawing.Point(12, 10);
            this.Lbl_TituloPanelIzquierdo.Name = "Lbl_TituloPanelIzquierdo";
            this.Lbl_TituloPanelIzquierdo.Size = new System.Drawing.Size(279, 21);
            this.Lbl_TituloPanelIzquierdo.TabIndex = 51;
            this.Lbl_TituloPanelIzquierdo.Text = "INFORMACIÓN DEL COLABORADOR";
            // 
            // Panel_Derecho
            // 
            this.Panel_Derecho.Controls.Add(this.PanelTabla);
            this.Panel_Derecho.Controls.Add(this.PanelToolStrip);
            this.Panel_Derecho.Controls.Add(this.panel2);
            this.Panel_Derecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Derecho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Panel_Derecho.ForeColor = System.Drawing.Color.Black;
            this.Panel_Derecho.Location = new System.Drawing.Point(415, 55);
            this.Panel_Derecho.Name = "Panel_Derecho";
            this.Panel_Derecho.Size = new System.Drawing.Size(769, 806);
            this.Panel_Derecho.TabIndex = 2;
            // 
            // PanelTabla
            // 
            this.PanelTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelTabla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelTabla.Controls.Add(this.Tabla);
            this.PanelTabla.Location = new System.Drawing.Point(22, 211);
            this.PanelTabla.Name = "PanelTabla";
            this.PanelTabla.Size = new System.Drawing.Size(725, 575);
            this.PanelTabla.TabIndex = 72;
            // 
            // Tabla
            // 
            this.Tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tabla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabla.Location = new System.Drawing.Point(0, 0);
            this.Tabla.Name = "Tabla";
            this.Tabla.Size = new System.Drawing.Size(725, 575);
            this.Tabla.TabIndex = 1;
            this.Tabla.SelectionChanged += new System.EventHandler(this.Tabla_SelectionChanged);
            // 
            // PanelToolStrip
            // 
            this.PanelToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.PanelToolStrip.Controls.Add(this.Lbl_Paginas);
            this.PanelToolStrip.Location = new System.Drawing.Point(22, 168);
            this.PanelToolStrip.Name = "PanelToolStrip";
            this.PanelToolStrip.Size = new System.Drawing.Size(725, 39);
            this.PanelToolStrip.TabIndex = 71;
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
            this.Lbl_Paginas.Size = new System.Drawing.Size(330, 20);
            this.Lbl_Paginas.TabIndex = 51;
            this.Lbl_Paginas.Text = "MOSTRANDO 1-10 DE 100 COLABORADORES";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel2.Controls.Add(this.Btn_CleanSearch);
            this.panel2.Controls.Add(this.Filtro3);
            this.panel2.Controls.Add(this.Filtro2);
            this.panel2.Controls.Add(this.Btn_Search);
            this.panel2.Controls.Add(this.Filtro1);
            this.panel2.Controls.Add(this.Txt_ValorBuscado);
            this.panel2.Location = new System.Drawing.Point(22, 20);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(725, 120);
            this.panel2.TabIndex = 53;
            // 
            // Btn_CleanSearch
            // 
            this.Btn_CleanSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_CleanSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_CleanSearch.Image = global::SECRON.Properties.Resources.Clear25x25;
            this.Btn_CleanSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_CleanSearch.Location = new System.Drawing.Point(675, 20);
            this.Btn_CleanSearch.Name = "Btn_CleanSearch";
            this.Btn_CleanSearch.Size = new System.Drawing.Size(30, 28);
            this.Btn_CleanSearch.TabIndex = 71;
            this.Btn_CleanSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_CleanSearch.UseVisualStyleBackColor = true;
            this.Btn_CleanSearch.Click += new System.EventHandler(this.Btn_CleanSearch_Click);
            // 
            // Filtro3
            // 
            this.Filtro3.FormattingEnabled = true;
            this.Filtro3.Location = new System.Drawing.Point(486, 67);
            this.Filtro3.Name = "Filtro3";
            this.Filtro3.Size = new System.Drawing.Size(219, 28);
            this.Filtro3.TabIndex = 70;
            // 
            // Filtro2
            // 
            this.Filtro2.FormattingEnabled = true;
            this.Filtro2.Location = new System.Drawing.Point(252, 67);
            this.Filtro2.Name = "Filtro2";
            this.Filtro2.Size = new System.Drawing.Size(219, 28);
            this.Filtro2.TabIndex = 69;
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Btn_Search.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.Btn_Search.Image = global::SECRON.Properties.Resources.SearchNegro25x25;
            this.Btn_Search.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Search.Location = new System.Drawing.Point(568, 20);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(101, 28);
            this.Btn_Search.TabIndex = 54;
            this.Btn_Search.Text = "BUSCAR";
            this.Btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Filtro1
            // 
            this.Filtro1.FormattingEnabled = true;
            this.Filtro1.Location = new System.Drawing.Point(16, 67);
            this.Filtro1.Name = "Filtro1";
            this.Filtro1.Size = new System.Drawing.Size(219, 28);
            this.Filtro1.TabIndex = 68;
            // 
            // Txt_ValorBuscado
            // 
            this.Txt_ValorBuscado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ValorBuscado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Txt_ValorBuscado.Location = new System.Drawing.Point(16, 23);
            this.Txt_ValorBuscado.MaxLength = 15;
            this.Txt_ValorBuscado.Name = "Txt_ValorBuscado";
            this.Txt_ValorBuscado.Size = new System.Drawing.Size(540, 27);
            this.Txt_ValorBuscado.TabIndex = 59;
            this.Txt_ValorBuscado.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_ValorBuscado_KeyDown);
            // 
            // Frm_Employees_Managment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.Panel_Derecho);
            this.Controls.Add(this.Panel_Izquierdo);
            this.Controls.Add(this.Panel_Superior);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_Employees_Managment";
            this.Text = "SECRON - GESTIÓN DE COLABORADORES";
            this.Load += new System.EventHandler(this.Frm_Employees_Managment_Load);
            this.Panel_Superior.ResumeLayout(false);
            this.Panel_Superior.PerformLayout();
            this.Panel_Izquierdo.ResumeLayout(false);
            this.Panel_Izquierdo.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.Panel_Emergencia.ResumeLayout(false);
            this.Panel_Emergencia.PerformLayout();
            this.Panel_Laboral.ResumeLayout(false);
            this.Panel_Laboral.PerformLayout();
            this.Panel_Contacto.ResumeLayout(false);
            this.Panel_Contacto.PerformLayout();
            this.Panel_Informacion.ResumeLayout(false);
            this.Panel_Informacion.PerformLayout();
            this.Panel_Derecho.ResumeLayout(false);
            this.PanelTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tabla)).EndInit();
            this.PanelToolStrip.ResumeLayout(false);
            this.PanelToolStrip.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Superior;
        private System.Windows.Forms.Label Lbl_Formulario;
        private System.Windows.Forms.Button Btn_Export;
        private System.Windows.Forms.Button Btn_Import;
        private System.Windows.Forms.Panel Panel_Izquierdo;
        private System.Windows.Forms.Panel Panel_Derecho;
        private System.Windows.Forms.Label Lbl_TituloPanelIzquierdo;
        private System.Windows.Forms.Panel Panel_Informacion;
        private System.Windows.Forms.Label Lbl_Subtitulo1;
        private System.Windows.Forms.Label Lbl_FechaNacimiento;
        private System.Windows.Forms.Label Lbl_Apellidos;
        private System.Windows.Forms.Label Lbl_Nombres;
        private System.Windows.Forms.Label Lbl_Dpi;
        private System.Windows.Forms.Label Lbl_Codigo;
        private System.Windows.Forms.TextBox Txt_Codigo;
        private System.Windows.Forms.TextBox Txt_Dpi;
        private System.Windows.Forms.TextBox Txt_Apellidos;
        private System.Windows.Forms.TextBox Txt_Nombres;
        private System.Windows.Forms.DateTimePicker DTP_Nacimiento;
        private System.Windows.Forms.Panel Panel_Contacto;
        private System.Windows.Forms.Label Lbl_Telefono1;
        private System.Windows.Forms.Label Lbl_CorreoInstitucional;
        private System.Windows.Forms.Label Lbl_CorreoPersonal;
        private System.Windows.Forms.Label Lbl_Subtitulo2;
        private System.Windows.Forms.Panel Panel_Laboral;
        private System.Windows.Forms.ComboBox ComboBox_Puesto;
        private System.Windows.Forms.ComboBox ComboBox_Departamento;
        private System.Windows.Forms.ComboBox ComboBox_Supervisor;
        private System.Windows.Forms.DateTimePicker DTP_Ingreso;
        private System.Windows.Forms.TextBox Txt_Salario;
        private System.Windows.Forms.Label Lbl_Supervisor;
        private System.Windows.Forms.Label Lbl_Puesto;
        private System.Windows.Forms.Label Lbl_Departamento;
        private System.Windows.Forms.Label Lbl_Salario;
        private System.Windows.Forms.Label Lbl_FechaIngreso;
        private System.Windows.Forms.Label Lbl_Subtitulo3;
        private System.Windows.Forms.TextBox Txt_Telefono1;
        private System.Windows.Forms.TextBox Txt_CorreoInstitucional;
        private System.Windows.Forms.TextBox Txt_CorreoPersonal;
        private System.Windows.Forms.TextBox Txt_Telefono2;
        private System.Windows.Forms.Label Lbl_Telefono2;
        private System.Windows.Forms.TextBox Txt_Direccion;
        private System.Windows.Forms.Label Lbl_Direccion;
        private System.Windows.Forms.Panel Panel_Emergencia;
        private System.Windows.Forms.TextBox Txt_TelefonoEmergencia;
        private System.Windows.Forms.TextBox Txt_Parentesco;
        private System.Windows.Forms.TextBox Txt_PersonaEmergencia;
        private System.Windows.Forms.Label Lbl_TelefonoEmergencia;
        private System.Windows.Forms.Label Lbl_Parentesco;
        private System.Windows.Forms.Label Lbl_PersonaEmergencia;
        private System.Windows.Forms.Label Lbl_Subtitulo4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Btn_Update;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Button Btn_Inactive;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox Txt_ValorBuscado;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.ComboBox Filtro3;
        private System.Windows.Forms.ComboBox Filtro2;
        private System.Windows.Forms.ComboBox Filtro1;
        private System.Windows.Forms.Panel PanelToolStrip;
        private System.Windows.Forms.Label Lbl_Paginas;
        private System.Windows.Forms.Panel PanelTabla;
        private System.Windows.Forms.DataGridView Tabla;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.Button Btn_CleanSearch;
        private System.Windows.Forms.ComboBox ComboBox_TipoContratacion;
        private System.Windows.Forms.Label Lbl_Contratacion;
        private System.Windows.Forms.ComboBox ComboBox_Sede;
        private System.Windows.Forms.Label Lbl_Sede;
        private System.Windows.Forms.TextBox Txt_ISR;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Txt_bono_ley;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Txt_bono_adicional;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Txt_igss;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Txt_salario_neto;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Txt_salario_base;
        private System.Windows.Forms.CheckBox chkb_IGSS;
    }
}