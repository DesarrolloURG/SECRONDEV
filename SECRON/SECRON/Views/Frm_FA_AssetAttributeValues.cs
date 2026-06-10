using SECRON.Controllers;
using SECRON.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_FA_AssetAttributeValues : Form
    {
        #region PropiedadesIniciales

        private readonly int _assetId;
        private readonly int _categoryId;
        private readonly int _userId;

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
            try { CargarYGenerarCampos(); }
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

        #endregion

        #region GeneracionDinamica

        private void CargarYGenerarCampos()
        {
            _controles = new Dictionary<int, (Mdl_FixedAssetAttributeValue, Control)>();

            var plantilla = Ctrl_FixedAssetAttributeValues.ObtenerPlantillaPorCategoria(_categoryId);

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
            Panel_Campos.Controls.Clear();

            int yPos = 10;

            foreach (var attr in plantilla.OrderBy(a => a.AttributeDefId))
            {
                var valorGuardado = valoresGuardados.FirstOrDefault(v => v.AttributeDefId == attr.AttributeDefId);

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

                // ── Label del campo ──────────────────────────────────────
                var lbl = new Label
                {
                    Text = attr.IsRequired
                                    ? $"{attr.AttributeLabel.ToUpper()} *"
                                    : attr.AttributeLabel.ToUpper(),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.Black,
                    Location = new Point(15, yPos),
                    AutoSize = true
                };
                Panel_Campos.Controls.Add(lbl);
                yPos += 24;

                // ── Hint debajo del label para tipos especiales ──────────
                string hint = ObtenerHint(attr.DataType, attr.AttributeKey);
                if (!string.IsNullOrEmpty(hint))
                {
                    var lblHint = new Label
                    {
                        Text = hint,
                        Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        Location = new Point(15, yPos),
                        AutoSize = true
                    };
                    Panel_Campos.Controls.Add(lblHint);
                    yPos += 18;
                }

                Control entrada = CrearControl(attr.DataType, atributo.Value, attr.AttributeKey);
                entrada.Location = new Point(15, yPos);
                entrada.Width = 565;
                Panel_Campos.Controls.Add(entrada);

                _controles[attr.AttributeDefId] = (atributo, entrada);
                yPos += entrada.Height + 15;
            }

            Panel_Campos.AutoScrollMinSize = new Size(0, yPos + 10);
        }

        private string ObtenerHint(string dataType, string key)
        {
            switch (dataType?.ToUpper())
            {
                case "LISTA": return "Seleccione una opción de la lista.";
                case "RANGO": return "Formato: mínimo|máximo  (ej. 0|100)";
                case "EMAIL": return "Formato: usuario@dominio.com";
                case "URL": return "Formato: https://sitio.com";
                case "TELEFONO": return "Formato: +502 12345678";
                case "COLOR": return "Formato HEX: #RRGGBB  (ej. #FF5733)";
                default: return null;
            }
        }

        private Control CrearControl(string dataType, string valorActual, string attrKey = "")
        {
            switch (dataType?.ToUpper())
            {
                // ── NUMERO ───────────────────────────────────────────────
                case "NUMERO":
                    var txtNum = new TextBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Height = 30,
                        Text = valorActual ?? ""
                    };
                    txtNum.KeyPress += (s, e) =>
                    {
                        if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
                            e.Handled = true;
                    };
                    return txtNum;

                // ── FECHA ────────────────────────────────────────────────
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

                // ── FECHA Y HORA ─────────────────────────────────────────
                case "FECHAHORA":
                    var dtpHora = new DateTimePicker
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Format = DateTimePickerFormat.Custom,
                        CustomFormat = "dd/MM/yyyy HH:mm",
                        Height = 30,
                        ShowUpDown = false
                    };
                    if (!string.IsNullOrWhiteSpace(valorActual) &&
                        DateTime.TryParse(valorActual, out DateTime fechaHoraParsed))
                        dtpHora.Value = fechaHoraParsed;
                    return dtpHora;

                // ── BOOLEAN ──────────────────────────────────────────────
                case "BOOLEAN":
                    var cmbBool = new ComboBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Height = 30
                    };
                    cmbBool.Items.AddRange(new object[] { "SI", "NO" });
                    cmbBool.SelectedItem = string.IsNullOrWhiteSpace(valorActual) ? "NO"
                        : (valorActual.ToUpper() == "SI" ? "SI" : "NO");
                    return cmbBool;

                // ── LISTA ────────────────────────────────────────────────
                // El AttributeKey debe contener las opciones separadas por |
                // Ej. AttributeKey = "WINDOWS|LINUX|MACOS"
                case "LISTA":
                    var cmbLista = new ComboBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Height = 30
                    };
                    string[] opciones = attrKey?.Split('|') ?? new string[0];
                    foreach (var op in opciones)
                        if (!string.IsNullOrWhiteSpace(op))
                            cmbLista.Items.Add(op.Trim().ToUpper());
                    if (!string.IsNullOrWhiteSpace(valorActual) &&
                        cmbLista.Items.Contains(valorActual.ToUpper()))
                        cmbLista.SelectedItem = valorActual.ToUpper();
                    else if (cmbLista.Items.Count > 0)
                        cmbLista.SelectedIndex = 0;
                    return cmbLista;

                // ── MULTILINEA ───────────────────────────────────────────
                case "MULTILINEA":
                    var txtMulti = new TextBox
                    {
                        Font = new Font("Segoe UI", 10F),
                        Multiline = true,
                        Height = 80,
                        ScrollBars = ScrollBars.Vertical,
                        Text = valorActual ?? ""
                    };
                    return txtMulti;

                // ── PORCENTAJE ───────────────────────────────────────────
                case "PORCENTAJE":
                    var pnlPct = new Panel { Height = 30, BackColor = Color.Transparent };
                    var txtPct = new TextBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Height = 30,
                        Width = 520,
                        Location = new Point(0, 0),
                        Text = valorActual ?? "0"
                    };
                    var lblPct = new Label
                    {
                        Text = "%",
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Location = new Point(525, 5),
                        AutoSize = true,
                        ForeColor = Color.Gray
                    };
                    txtPct.KeyPress += (s, e) =>
                    {
                        if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
                            e.Handled = true;
                    };
                    pnlPct.Controls.Add(txtPct);
                    pnlPct.Controls.Add(lblPct);
                    pnlPct.Tag = txtPct; // referencia para obtener valor
                    return pnlPct;

                // ── MONEDA ───────────────────────────────────────────────
                case "MONEDA":
                    var pnlMon = new Panel { Height = 30, BackColor = Color.Transparent };
                    var lblMon = new Label
                    {
                        Text = "Q",
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Location = new Point(0, 5),
                        AutoSize = true,
                        ForeColor = Color.Gray
                    };
                    var txtMon = new TextBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Height = 30,
                        Width = 540,
                        Location = new Point(20, 0),
                        Text = valorActual ?? "0.00"
                    };
                    txtMon.KeyPress += (s, e) =>
                    {
                        if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
                            e.Handled = true;
                    };
                    pnlMon.Controls.Add(lblMon);
                    pnlMon.Controls.Add(txtMon);
                    pnlMon.Tag = txtMon;
                    return pnlMon;

                // ── EMAIL ────────────────────────────────────────────────
                case "EMAIL":
                    var txtEmail = new TextBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Height = 30,
                        Text = valorActual ?? ""
                    };
                    return txtEmail;

                // ── URL ──────────────────────────────────────────────────
                case "URL":
                    var txtUrl = new TextBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Height = 30,
                        Text = valorActual ?? ""
                    };
                    return txtUrl;

                // ── TELEFONO ─────────────────────────────────────────────
                case "TELEFONO":
                    var txtTel = new TextBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Height = 30,
                        Text = valorActual ?? ""
                    };
                    txtTel.KeyPress += (s, e) =>
                    {
                        if (!char.IsDigit(e.KeyChar) && e.KeyChar != '+' &&
                            e.KeyChar != ' ' && e.KeyChar != '-' && e.KeyChar != '\b')
                            e.Handled = true;
                    };
                    return txtTel;

                // ── RANGO ────────────────────────────────────────────────
                // Guarda como "min|max"
                case "RANGO":
                    var pnlRango = new Panel { Height = 30, BackColor = Color.Transparent };
                    string[] partes = (valorActual ?? "").Split('|');
                    var txtMin = new TextBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Height = 30,
                        Width = 260,
                        Location = new Point(0, 0),
                        Text = partes.Length > 0 ? partes[0] : ""
                    };
                    var lblRango = new Label
                    {
                        Text = "—",
                        Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                        Location = new Point(268, 5),
                        AutoSize = true,
                        ForeColor = Color.Gray
                    };
                    var txtMax = new TextBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Height = 30,
                        Width = 260,
                        Location = new Point(290, 0),
                        Text = partes.Length > 1 ? partes[1] : ""
                    };
                    foreach (var t in new[] { txtMin, txtMax })
                    {
                        var tx = t;
                        tx.KeyPress += (s, e) =>
                        {
                            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
                                e.Handled = true;
                        };
                    }
                    pnlRango.Controls.Add(txtMin);
                    pnlRango.Controls.Add(lblRango);
                    pnlRango.Controls.Add(txtMax);
                    pnlRango.Tag = new[] { txtMin, txtMax }; // referencia para obtener valor
                    return pnlRango;

                // ── COLOR ────────────────────────────────────────────────
                case "COLOR":
                    var pnlColor = new Panel { Height = 30, BackColor = Color.Transparent };
                    var txtColor = new TextBox
                    {
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        Height = 30,
                        Width = 520,
                        Location = new Point(0, 0),
                        Text = valorActual ?? "#FFFFFF"
                    };
                    var pnlMuestra = new Panel
                    {
                        Width = 30,
                        Height = 30,
                        Location = new Point(530, 0),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    // Actualizar muestra al escribir
                    txtColor.TextChanged += (s, e) =>
                    {
                        try
                        {
                            pnlMuestra.BackColor = ColorTranslator.FromHtml(txtColor.Text);
                        }
                        catch { pnlMuestra.BackColor = Color.White; }
                    };
                    // Inicializar muestra
                    try { pnlMuestra.BackColor = ColorTranslator.FromHtml(valorActual ?? "#FFFFFF"); }
                    catch { pnlMuestra.BackColor = Color.White; }
                    pnlColor.Controls.Add(txtColor);
                    pnlColor.Controls.Add(pnlMuestra);
                    pnlColor.Tag = txtColor;
                    return pnlColor;

                // ── TEXTO (default) ──────────────────────────────────────
                default:
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

                case "FECHAHORA":
                    return ((DateTimePicker)control).Value.ToString("yyyy-MM-dd HH:mm");

                case "BOOLEAN":
                case "LISTA":
                    return ((ComboBox)control).SelectedItem?.ToString() ?? "";

                case "MULTILINEA":
                    return ((TextBox)control).Text.Trim();

                case "PORCENTAJE":
                case "MONEDA":
                case "COLOR":
                    // Panel con Tag apuntando al TextBox interno
                    if (control is Panel pnl && pnl.Tag is TextBox txtInterno)
                        return txtInterno.Text.Trim();
                    return "";

                case "RANGO":
                    if (control is Panel pnlR && pnlR.Tag is TextBox[] campos)
                        return $"{campos[0].Text.Trim()}|{campos[1].Text.Trim()}";
                    return "";

                default:
                    return control is TextBox tb ? tb.Text.Trim() : "";
            }
        }

        #endregion

        #region Guardar

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

        private bool ValidarObligatorios()
        {
            foreach (var kvp in _controles)
            {
                var attr = kvp.Value.Atributo;
                var ctrl = kvp.Value.Entrada;
                string val = ObtenerValorDeControl(attr.DataType, ctrl);

                // ── Validar obligatorio ──────────────────────────────────
                if (attr.IsRequired && string.IsNullOrWhiteSpace(val))
                {
                    MessageBox.Show($"El campo '{attr.AttributeLabel}' es obligatorio.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ctrl.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(val)) continue;

                // ── Validar por tipo ─────────────────────────────────────
                switch (attr.DataType?.ToUpper())
                {
                    case "NUMERO":
                        if (!decimal.TryParse(val,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out _))
                        {
                            MessageBox.Show($"'{attr.AttributeLabel}' debe ser un número válido.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ctrl.Focus(); return false;
                        }
                        break;

                    case "PORCENTAJE":
                        if (!decimal.TryParse(val,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out decimal pct)
                            || pct < 0 || pct > 100)
                        {
                            MessageBox.Show($"'{attr.AttributeLabel}' debe ser un número entre 0 y 100.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ctrl.Focus(); return false;
                        }
                        break;

                    case "MONEDA":
                        if (!decimal.TryParse(val,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out _))
                        {
                            MessageBox.Show($"'{attr.AttributeLabel}' debe ser un valor monetario válido.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ctrl.Focus(); return false;
                        }
                        break;

                    case "EMAIL":
                        if (!Regex.IsMatch(val, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        {
                            MessageBox.Show($"'{attr.AttributeLabel}' no tiene un formato de email válido.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ctrl.Focus(); return false;
                        }
                        break;

                    case "URL":
                        if (!Uri.TryCreate(val, UriKind.Absolute, out Uri uriResult) ||
                            (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                        {
                            MessageBox.Show($"'{attr.AttributeLabel}' no tiene un formato de URL válido (debe iniciar con http:// o https://).",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ctrl.Focus(); return false;
                        }
                        break;

                    case "COLOR":
                        if (!Regex.IsMatch(val, @"^#[0-9A-Fa-f]{6}$"))
                        {
                            MessageBox.Show($"'{attr.AttributeLabel}' debe tener formato HEX válido (ej. #FF5733).",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ctrl.Focus(); return false;
                        }
                        break;

                    case "RANGO":
                        string[] rangoParts = val.Split('|');
                        if (rangoParts.Length != 2 ||
                            !decimal.TryParse(rangoParts[0].Trim(),
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out decimal rMin) ||
                            !decimal.TryParse(rangoParts[1].Trim(),
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out decimal rMax))
                        {
                            MessageBox.Show($"'{attr.AttributeLabel}' requiere valores numéricos en ambos campos.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ctrl.Focus(); return false;
                        }
                        if (rMin > rMax)
                        {
                            MessageBox.Show($"'{attr.AttributeLabel}': el valor mínimo no puede ser mayor al máximo.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ctrl.Focus(); return false;
                        }
                        break;
                }
            }
            return true;
        }

        public void GuardarValores(int assetId)
        {
            foreach (var kvp in _controles)
            {
                var attr = kvp.Value.Atributo;
                attr.AssetId = assetId;
                attr.CreatedBy = _userId;
                attr.ModifiedBy = _userId;
                Ctrl_FixedAssetAttributeValues.RegistrarValor(attr);
            }
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "¿Está seguro que desea cerrar sin guardar las características?\nTampoco se guardará el activo ingresado.",
                "Confirmar cierre",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        #endregion
    }
}