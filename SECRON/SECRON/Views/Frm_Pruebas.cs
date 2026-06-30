using SECRON.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Pruebas : Form
    {
        public Frm_Pruebas()
        {
            InitializeComponent();
        }

        private void Frm_Pruebas_Load(object sender, EventArgs e)
        {
            // Columna abrir PDF
            var colAbrir = new DataGridViewImageColumn();
            colAbrir.Name = "ColAbrirPDF";
            colAbrir.HeaderText = "";
            colAbrir.Image = Properties.Resources.SearchNegro25x25;
            colAbrir.Width = 35;
            colAbrir.ImageLayout = DataGridViewImageCellLayout.Zoom;
            Tabla.Columns.Add(colAbrir);

            // Columna cargar PDF
            var colCargar = new DataGridViewImageColumn();
            colCargar.Name = "ColCargarPDF";
            colCargar.HeaderText = "";
            colCargar.Image = Properties.Resources.SearchNegro25x25;
            colCargar.Width = 35;
            colCargar.ImageLayout = DataGridViewImageCellLayout.Zoom;
            Tabla.Columns.Add(colCargar);

            CargarCheques();
            Tabla.CellClick += Tabla_CellClick;
        }

        private int _checkIdSeleccionado = 0;

        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            _checkIdSeleccionado = (int)Tabla.Rows[e.RowIndex].Cells["CheckId"].Value;

            // Clic en columna de abrir PDF
            if (e.ColumnIndex == Tabla.Columns["ColAbrirPDF"].Index)
            {
                string filePath = Tabla.Rows[e.RowIndex].Cells["FilePath"].Value?.ToString();

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    MessageBox.Show("ESTE CHEQUE NO TIENE UN ARCHIVO VINCULADO.", "AVISO",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!System.IO.File.Exists(filePath))
                {
                    MessageBox.Show("EL ARCHIVO NO SE ENCUENTRA EN LA RUTA INDICADA.\n" + filePath,
                                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                System.Diagnostics.Process.Start(filePath);
            }
            else if (e.ColumnIndex == Tabla.Columns["ColCargarPDF"].Index)
            {
                // Misma lógica del Btn_Add_Click
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "SELECCIONAR ARCHIVO PDF";
                    dlg.Filter = "Archivos PDF (*.pdf)|*.pdf";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            string numeroCheque = Tabla.Rows[e.RowIndex].Cells["CheckNumber"].Value.ToString();
                            string carpetaDestino = @"\\Uregional\shared\SECRON\Cheques\";
                            string nombreArchivo = numeroCheque + ".pdf";
                            string rutaDestino = System.IO.Path.Combine(carpetaDestino, nombreArchivo);

                            if (!System.IO.Directory.Exists(carpetaDestino))
                                System.IO.Directory.CreateDirectory(carpetaDestino);

                            string rutaSinExtension = System.IO.Path.Combine(carpetaDestino, numeroCheque);
                            if (System.IO.File.Exists(rutaSinExtension))
                                System.IO.File.Delete(rutaSinExtension);

                            System.IO.File.Copy(dlg.FileName, rutaDestino, overwrite: true);

                            bool ok = Ctrl_Checks.ActualizarFilePath(_checkIdSeleccionado, rutaDestino, 1);
                            if (ok)
                            {
                                MessageBox.Show("ARCHIVO VINCULADO CORRECTAMENTE.", "ÉXITO",
                                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                                CargarCheques();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("ERROR AL COPIAR ARCHIVO: " + ex.Message, "ERROR",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            if (_checkIdSeleccionado == 0)
            {
                MessageBox.Show("SELECCIONE UN CHEQUE PRIMERO.", "AVISO",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "SELECCIONAR ARCHIVO PDF";
                dlg.Filter = "Archivos PDF (*.pdf)|*.pdf";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string numeroCheque = Tabla.Rows[Tabla.CurrentCell.RowIndex]
                                                   .Cells["CheckNumber"].Value.ToString();

                        string carpetaDestino = @"\\Uregional\shared\SECRON\Cheques\";
                        string nombreArchivo = numeroCheque + ".pdf";
                        string rutaDestino = System.IO.Path.Combine(carpetaDestino, nombreArchivo);

                        if (!System.IO.Directory.Exists(carpetaDestino))
                            System.IO.Directory.CreateDirectory(carpetaDestino);

                        // Eliminar versión sin extensión si existe (limpieza de archivos mal guardados)
                        string rutaSinExtension = System.IO.Path.Combine(carpetaDestino, numeroCheque);
                        if (System.IO.File.Exists(rutaSinExtension))
                            System.IO.File.Delete(rutaSinExtension);

                        System.IO.File.Copy(dlg.FileName, rutaDestino, overwrite: true);

                        bool ok = Ctrl_Checks.ActualizarFilePath(_checkIdSeleccionado, rutaDestino, 1 /* userId */);
                        if (ok)
                        {
                            MessageBox.Show("ARCHIVO VINCULADO CORRECTAMENTE.", "ÉXITO",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarCheques();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ERROR AL COPIAR ARCHIVO: " + ex.Message, "ERROR",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            if (_checkIdSeleccionado == 0) return;

            if (MessageBox.Show("¿DESEA QUITAR EL ARCHIVO VINCULADO?", "CONFIRMAR",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bool ok = Ctrl_Checks.ActualizarFilePath(_checkIdSeleccionado, null, 1 /* userId */);
                if (ok)
                {
                    MessageBox.Show("ARCHIVO QUITADO CORRECTAMENTE.", "ÉXITO",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarCheques();
                }
            }
        }

        private void CargarCheques()
        {
            var lista = Ctrl_Checks.MostrarCheques();
            Tabla.DataSource = lista
                .Select(c => new {
                    c.CheckId,
                    c.CheckNumber,
                    c.BeneficiaryName,
                    c.Amount,
                    c.FilePath
                })
                .ToList();
        }
    }
}