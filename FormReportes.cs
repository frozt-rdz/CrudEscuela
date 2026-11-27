using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace CrudEscuela
{
    public partial class FormReportes : Form
    {
        public FormReportes()
        {
            InitializeComponent();
        }

        private void FormReportes_Load(object sender, EventArgs e)
        {
            AplicarEstilo();

            cmbReporte.DropDownStyle = ComboBoxStyle.DropDownList;
            // Cada opcion muestra un objeto distinto de la base de datos
            cmbReporte.Items.AddRange(new string[]
            {
                "Inscripciones (vista)",
                "Promedio por alumno (procedimiento y funcion)",
                "Bitacora de cambios (trigger)"
            });

            dgvReporte.ReadOnly = true;
            dgvReporte.AllowUserToAddRows = false;
            dgvReporte.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            cmbReporte.SelectedIndex = 0; // dispara cmbReporte_SelectedIndexChanged y carga el primer reporte
        }

        private void cmbReporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void CargarReporte()
        {
            try
            {
                using var con = Conexion.Crear();
                SqlCommand cmd;

                switch (cmbReporte.SelectedIndex)
                {
                    case 0:
                        // Consulta sobre la VISTA (une Alumno, Materia e Inscripcion y calcula el estado con una funcion)
                        cmd = new SqlCommand("SELECT * FROM vw_InscripcionesDetalle ORDER BY Alumno, Materia", con);
                        break;
                    case 1:
                        // Ejecuta el PROCEDIMIENTO ALMACENADO (usa la funcion fn_PromedioAlumno)
                        cmd = new SqlCommand("sp_ReporteAlumnos", con) { CommandType = CommandType.StoredProcedure };
                        break;
                    default:
                        // Registros que genero el TRIGGER de auditoria, los mas recientes primero
                        cmd = new SqlCommand("SELECT * FROM Bitacora ORDER BY IdBitacora DESC", con);
                        break;
                }

                using (cmd)
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvReporte.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el reporte: " + ex.Message);
            }
        }

        private void AplicarEstilo()
        {
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10F);

            dgvReporte.BorderStyle = BorderStyle.None;
            dgvReporte.BackgroundColor = Color.White;
            dgvReporte.RowHeadersVisible = false;
            dgvReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReporte.RowTemplate.Height = 28;

            Color colorMorado = Color.FromArgb(177, 74, 237);
            dgvReporte.EnableHeadersVisualStyles = false;
            dgvReporte.ColumnHeadersDefaultCellStyle.BackColor = colorMorado;
            dgvReporte.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReporte.ColumnHeadersDefaultCellStyle.SelectionBackColor = colorMorado;
            dgvReporte.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            dgvReporte.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 244, 250);
            dgvReporte.DefaultCellStyle.SelectionBackColor = Color.FromArgb(229, 184, 255);
            dgvReporte.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

    }
}
