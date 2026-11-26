using Microsoft.Data.SqlClient;
using System.Data;
using System.Xml.Serialization;
using System.Text;


namespace CrudEscuela
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            CargarDatos();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dgvDatos.CurrentRow?.DataBoundItem is not DataRowView fila)
            {
                MessageBox.Show("Selecciona primero un registro de la tabla.");
                return;
            }

            string tabla = cmbTabla.SelectedItem.ToString();
            if (Eliminador.Eliminar(tabla, fila.Row))
            {
                CargarDatos(); // Refrescamos el DataGridView
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AplicarEstilo();
            // Configuración inicial del ComboBox y DataGridView
            cmbTabla.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTabla.Items.AddRange(new string[] { "Alumno", "Materia", "Inscripcion" });

            dgvDatos.ReadOnly = true;
            dgvDatos.AllowUserToAddRows = false;
            dgvDatos.SelectionMode = DataGridViewSelectionMode.FullRowSelect; //Seleccion de fila completa
            dgvDatos.MultiSelect = false;

            cmbTabla.SelectedIndex = 0; //Selecciona la primera tabla por defecto
        }

        private void CargarDatos()
        {
            try
            {
                string tabla = cmbTabla.SelectedItem.ToString();
                // using evita que la conexión quede abierta después de usarla, cerrándola automáticamente al salir del bloque using
                using (var con = Conexion.Crear())
                using (var da = new SqlDataAdapter("SELECT * FROM " + tabla, con))
                {
                    // Creamos una tabla vacia en memoria para almacenar los datos de la consulta
                    var dt = new DataTable();
                    // El adapter llena la tabla con los datos obtenidos de la consulta SQL
                    da.Fill(dt);
                    dgvDatos.DataSource = dt;
                    Buscar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar: " + ex.Message);


            }

        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            string tabla = cmbTabla.SelectedItem.ToString();
            using var f = new FormRegistro(tabla);
            if (f.ShowDialog() == DialogResult.OK)
            {
                CargarDatos(); // Refrescamso el DataGridView
            }

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvDatos.CurrentRow?.DataBoundItem is not DataRowView fila)
            {
                MessageBox.Show("Selecciona primero un registro de la tabla.");
                return;
            }

            string tabla = cmbTabla.SelectedItem.ToString();
            using var f = new FormRegistro(tabla, fila.Row);
            if (f.ShowDialog() == DialogResult.OK)
            {
                CargarDatos();
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        // Aplica un estilo visual
        private void AplicarEstilo()
        {
            BackColor = Color.FromArgb(245, 247, 250); // gris 
            Font = new Font("Segoe UI", 10F);          // fuente

            // Cada boton con un color
            EstiloBoton(btnInsertar, Color.FromArgb(18, 22, 58));
            EstiloBoton(btnModificar, Color.FromArgb(46, 26, 99));
            EstiloBoton(btnEliminar, Color.FromArgb(91, 42, 134));

            cmbTabla.Width = 200;

            // Estilo de la cuadricula
            dgvDatos.BorderStyle = BorderStyle.None;
            dgvDatos.BackgroundColor = Color.White;
            dgvDatos.GridColor = Color.FromArgb(226, 232, 240);
            dgvDatos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; // solo lineas horizontales
            dgvDatos.RowHeadersVisible = false;                                      // quita la columna vacia de la izquierda
            dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;     // las columnas llenan el ancho
            dgvDatos.RowTemplate.Height = 28;

            // Encabezados azules con texto blanco (EnableHeadersVisualStyles debe ser false para que apliquen los colores)
            dgvDatos.EnableHeadersVisualStyles = false;

            Color colorMorado = Color.FromArgb(177, 74, 237);

            dgvDatos.ColumnHeadersDefaultCellStyle.BackColor = colorMorado;
            dgvDatos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDatos.ColumnHeadersDefaultCellStyle.SelectionBackColor = colorMorado;
            dgvDatos.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // Filas alternadas 
            dgvDatos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 244, 250);
            dgvDatos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(229, 184, 255);
            dgvDatos.DefaultCellStyle.SelectionForeColor = Color.Black;

            AcceptButton = btnBuscar; // presionar Enter en la barra de busqueda equivale a pulsar Buscar
            EstiloBoton(btnBuscar, Color.FromArgb(91, 42, 134));
            EstiloBoton(btnLimpiar, Color.FromArgb(177, 74, 237));
            txtBuscar.Width = 250;
        }

        // Boton plano
        private void EstiloBoton(Button b, Color color)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = color;
            b.ForeColor = Color.White;
            b.Cursor = Cursors.Hand;
            b.Size = new Size(110, 36);
        }


        private void Buscar()
        {
            // La cuadricula esta enlazada a un DataTable; si todavia no hay datos, no hay nada que filtrar
            if (dgvDatos.DataSource is not DataTable dt) return;

            string texto = txtBuscar.Text.Trim();

            // Sin texto: se quita el filtro y se muestran todos los registros
            if (texto == "")
            {
                dt.DefaultView.RowFilter = "";
                lblResultados.Text = $"{dt.Rows.Count} registro(s)";
                return;
            }

            string patron = EscaparLike(texto);

            // Una condicion por columna: convierte el valor a texto y revisa si contiene lo escrito
            var condiciones = new List<string>();
            foreach (DataColumn col in dt.Columns)
            {
                condiciones.Add($"Convert([{col.ColumnName}], 'System.String') LIKE '%{patron}%'");
            }

            // Basta con que coincida una columna (OR) para que la fila se muestre
            dt.DefaultView.RowFilter = string.Join(" OR ", condiciones);
            lblResultados.Text = $"{dt.DefaultView.Count} resultado(s)";
        }

        // Escapa los caracteres especiales para que lo que escriba el usuario se busque como texto literal
        private static string EscaparLike(string texto)
        {
            var sb = new StringBuilder();
            foreach (char c in texto)
            {
                if (c == '*' || c == '%' || c == '[' || c == ']')
                    sb.Append('[').Append(c).Append(']'); // comodines de LIKE se encierran entre corchetes
                else if (c == '\'')
                    sb.Append("''");                      // la comilla simple se duplica
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            Buscar();            // con el texto vacio, quita el filtro y muestra todos los registros
            txtBuscar.Focus();
        }
    }
}
