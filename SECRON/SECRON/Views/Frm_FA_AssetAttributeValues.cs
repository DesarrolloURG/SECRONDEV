using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_FA_AssetAttributeValues : Form
    {
        #region PropiedadesIniciales

        private readonly int _assetId;
        private readonly int _categoryId;
        private readonly int _userId;

        // Lista de atributos con sus controles generados dinámicamente
        // Key: AttributeDefId | Value: (Mdl_FixedAssetAttributeValue, Control de entrada)
        private Dictionary<int, (Mdl_FixedAssetAttributeValue Atributo, Control Entrada)> _controles;

        public Frm_FA_AssetAttributeValues(int assetId, int categoryId, int userId)
        {
            InitializeComponent();
            _assetId = assetId;
            _categoryId = categoryId;
            _userId = userId;
            ConfigurarTamañoFormulario();
        }

        private void Frm_FA_AssetAttributeValues_Load(object sender, EventArgs e)
        {
            try
            {
                CargarYGenerarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar características: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarTamañoFormulario()
        {
            this.Size = new Size(620, 650);
            this.MinimumSize = new Size(620, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        #endregion PropiedadesIniciales

        #region GeneracionDinamica

        private void CargarYGenerarCampos()
        {
            _controles = new Dictionary<int, (Mdl_FixedAssetAttributeValue, Control)>();

            // Obtener plantilla de atributos de la categoría
            var plantilla = Ctrl_FixedAssetAttributeValues.ObtenerPlantillaPorCategoria(_categoryId);

            // Si el activo ya existe, obtener los valores guardados
            List<Mdl_FixedAssetAttributeValue> valoresGuardados = new List<Mdl_FixedAssetAttributeValue>();
            if (_assetId > 0)
                valoresGuardados = Ctrl_FixedAssetAttributeValues.ObtenerValoresPorActivo(_assetId);

            if (plantilla == null || plantilla.Count == 0)
            {
                Lbl_Info.Text = "Esta categoría no tiene características definidas.";
                Btn_Save.Enabled = false;
                return;
            }

            Lbl_Info.Text = $"Complete las características del activo ({plantilla.Count} campos):";

            // Limpiar panel de campos dinámicos
            Panel_Campos.Controls.Clear();

            int yPos = 10;

            foreach (var attr in plantilla.OrderBy(a => a.AttributeDefId))
            {
                // Buscar si ya tiene valor guardado
                var valorGuardado = valoresGuardados.FirstOrDefault(v => v.AttributeDefId == attr.AttributeDefId);

                // Unir la info de definición con el valor guardado
                var atributo = new Mdl_FixedAssetAttributeValue
                {
                    AttributeValueId = valorGuardado?.AttributeValueId ?? 0,
                    AssetId = _assetId,
                    AttributeDefId = attr.AttributeDefId,
                    AttributeKey = attr.AttributeKey,
                    AttributeLabel = attr.AttributeLabel,
                    DataType = attr.DataType,
                    IsRequired = attr.IsRequired,
                    Value = valorGuardado?.Value ?? "",
                    CreatedBy = _userId
                };

                // Label del atributo
                var lbl = new Label
                {
                    Text = attr.IsRequired
                    ? $"{attr.AttributeLabel.ToUpper()} *"
                    : attr.AttributeLabel.ToUpper(),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.Black,  // siempre negro
                    Location = new Point(15, yPos),
                    AutoSize = true
                };
                Panel_Campos.Controls.Add(lbl);
                yPos += 24;

                // Control de entrada según DataType
                Control entrada = CrearControl(attr.DataType, atributo.Value);
                entrada.Location = new Point(15, yPos);
                entrada.Width = 565;
                Panel_Campos.Controls.Add(entrada);

                _controles[attr.AttributeDefId] = (atributo, entrada);
                yPos += entrada.Height + 15;
            }

        }

        private Control CrearControl(string dataType, string valorActual)
        {
            switch (dataType?.ToUpper())
            {
                case "NUMERO":
                    var txtNum = new TextBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Height = 30,
                        Text = valorActual ?? ""
                    };
                    // Solo números y punto decimal
                    txtNum.KeyPress += (s, e) =>
                    {
                        if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
                            e.Handled = true;
                    };
                    return txtNum;

                case "FECHA":
                    var dtp = new DateTimePicker
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Format = DateTimePickerFormat.Custom,
                        CustomFormat = "dd/MM/yyyy",
                        Height = 30
                    };
                    if (!string.IsNullOrWhiteSpace(valorActual) &&
                        DateTime.TryParse(valorActual, out DateTime fechaParsed))
                        dtp.Value = fechaParsed;
                    return dtp;

                case "BOOLEAN":
                    var combo = new ComboBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Height = 30
                    };
                    combo.Items.Add("SI");
                    combo.Items.Add("NO");
                    combo.SelectedItem = string.IsNullOrWhiteSpace(valorActual) ? "NO"
                        : (valorActual.ToUpper() == "SI" ? "SI" : "NO");
                    return combo;

                default: // TEXT
                    var txt = new TextBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Height = 30,
                        Text = valorActual ?? ""
                    };
                    return txt;
            }
        }

        private string ObtenerValorDeControl(string dataType, Control control)
        {
            switch (dataType?.ToUpper())
            {
                case "FECHA":
                    return ((DateTimePicker)control).Value.ToString("yyyy-MM-dd");
                case "BOOLEAN":
                    return ((ComboBox)control).SelectedItem?.ToString() ?? "NO";
                default:
                    return ((TextBox)control).Text.Trim();
            }
        }

        #endregion GeneracionDinamica

        #region Guardar

        // Btn_Save_Click solo valida y cierra con OK
        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!ValidarObligatorios()) return;
            CapturarValoresEnModelo();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CapturarValoresEnModelo()
        {
            foreach (var kvp in _controles)
            {
                var attr = kvp.Value.Atributo;
                var ctrl = kvp.Value.Entrada;
                attr.Value = ObtenerValorDeControl(attr.DataType, ctrl);
            }
        }

        // Validar sin guardar
        private bool ValidarObligatorios()
        {
            foreach (var kvp in _controles)
            {
                var attr = kvp.Value.Atributo;
                var ctrl = kvp.Value.Entrada;
                string val = ObtenerValorDeControl(attr.DataType, ctrl);

                // Validar obligatorio
                if (attr.IsRequired && string.IsNullOrWhiteSpace(val))
                {
                    MessageBox.Show($"El campo '{attr.AttributeLabel}' es obligatorio.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ctrl.Focus();
                    return false;
                }

                // Validar tipo si tiene valor
                if (!string.IsNullOrWhiteSpace(val))
                {
                    switch (attr.DataType?.ToUpper())
                    {
                        case "NUMERO":
                            if (!decimal.TryParse(val,
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out _))
                            {
                                MessageBox.Show($"El campo '{attr.AttributeLabel}' debe ser un número válido.",
                                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ctrl.Focus();
                                return false;
                            }
                            break;
                        case "FECHA":
                            if (!DateTime.TryParse(val, out _))
                            {
                                MessageBox.Show($"El campo '{attr.AttributeLabel}' debe ser una fecha válida.",
                                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ctrl.Focus();
                                return false;
                            }
                            break;
                    }
                }
            }
            return true;
        }

        // Llamado desde el padre después de insertar el activo
        public void GuardarValores(int assetId)
        {
            foreach (var kvp in _controles)
            {
                var attr = kvp.Value.Atributo;
                attr.AssetId = assetId;
                attr.CreatedBy = _userId;
                attr.ModifiedBy = _userId;

                int resultado = Ctrl_FixedAssetAttributeValues.RegistrarValor(attr);
            }
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "¿Está seguro que desea cerrar sin guardar las características?\nTampoco se guardará el activo ingresado.\n\n" ,
                "Confirmar cierre",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        #endregion Guardar
    }
}