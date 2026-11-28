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
    public partial class FormRegistro : Form
    {
        private readonly string _tabla;
        // Fila que se va a modificar. Si es null, el formulario funciona en modo INSERTAR; si trae valor, en modo MODIFICAR
        private readonly DataRow? _fila;
        //La clave es el nombre de la columna, y el valor es el control (TextBox o ComboBox) que contiene el valor a insertar
        private readonly Dictionary<string, Control> _campos = new();
        // Para almacenar la estructura de la tabla
        private DataTable _esquema = new();

        // El parametro fila es opcional: si no se envia, vale null y el formulario se comporta como INSERTAR
        public FormRegistro(String tabla, DataRow? fila = null)
        {
            InitializeComponent();
            AplicarEstilo();
            _tabla = tabla;
            _fila = fila;
            Text = (fila == null ? "Insertar en " : "Modificar en ") + tabla;
        }

        private void FormRegistro_Load(object sender, EventArgs e)
        {
            try
            {
                using var con = Conexion.Crear();
                // Abrimos conexion con SQLServer
                con.Open();

                using (var da = new SqlDataAdapter("SELECT * FROM " + _tabla, con))
                {
                    // Recuperamos datos del SELECT y almacenamos la estructura de la tabla en _esquema
                    da.FillSchema(_esquema, SchemaType.Source);
                }

                // Que columnas son llaves foraneas y a que tabla y columna hacen referencia
                var fks = ObtenerLlavesForaneas(con);

                int y = 10;

                foreach (DataColumn col in _esquema.Columns)
                {
                    // Si la columna es autoincremental, al INSERTAR no se muestra en el formulario y continuamos
                    // (SQL Server genera el Id). Al MODIFICAR si se muestra, para dar contexto al usuario
                    if (_fila == null && col.AutoIncrement) continue;

                    // Verificamos si la(s) columna(s) forma(n) parte de la llave primaria
                    bool esPk = _esquema.PrimaryKey.Contains(col);

                    var lbl = new Label { Text = col.ColumnName, Left = 10, Top = y + 3, Width = 110 };

                    Control ctrl;
                    // Buscamos si la columna es una llave foranea, y si lo es, creamos un ComboBox con los valores de la tabla referenciada
                    if (fks.TryGetValue(col.ColumnName, out var destino))
                        // ComboBox con los registros de la tabla referenciada
                        ctrl = CrearCombo(con, destino.tabla, destino.columna, y);
                    else
                        ctrl = new TextBox { Left = 130, Top = y, Width = 200 };

                    // Si estamos en modo MODIFICAR, llenamos el control con los valores actuales de la fila
                    if (_fila != null)
                    {
                        object valor = _fila[col.ColumnName];
                        if (ctrl is ComboBox cb)
                            // Selecciona en la lista el registro cuyo Id coincide con el valor actual
                            cb.SelectedValue = valor is DBNull ? null! : valor;
                        else
                            ctrl.Text = valor is DBNull ? "" : Convert.ToString(valor) ?? "";

                        // Las columnas de la llave primaria se muestran deshabilitadas: no se pueden modificar
                        if (esPk) ctrl.Enabled = false;
                    }

                    pnlCampos.Controls.Add(lbl);
                    pnlCampos.Controls.Add(ctrl);
                    // Guardamos el control en el diccionario de campos, con la columna como clave
                    _campos[col.ColumnName] = ctrl;
                    y += 32;
                }
                // Ajusta el alto de la ventana segun cuantos campos se generaron (con un maximo de 600; si hay mas, el panel hace scroll)
                ClientSize = new Size(380, Math.Min(y + 80, 600));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al preparar el formulario: " + ex.Message);
                Close();
            }
        }


        private Dictionary<string, (string tabla, string columna)> ObtenerLlavesForaneas(SqlConnection con)
        {
            // En resultado guardamos el nombre de la columna, y la tabla y columna a la que hace referencia
            var resultado = new Dictionary<string, (string tabla, string columna)>();
            // Esta sentencia nos permite obtener las llaves foraneas de la tabla, y a que tabla y columna hacen referencia
            string sql = @"
                SELECT cp.name,
                       OBJECT_NAME(fkc.referenced_object_id),
                       cr.name
                FROM sys.foreign_key_columns fkc
                JOIN sys.columns cp ON cp.object_id = fkc.parent_object_id
                                   AND cp.column_id = fkc.parent_column_id
                JOIN sys.columns cr ON cr.object_id = fkc.referenced_object_id
                                   AND cr.column_id = fkc.referenced_column_id
                WHERE fkc.parent_object_id = OBJECT_ID(@tabla)";

            using var cmd = new SqlCommand(sql, con);

            // Agregamos el parámetro @tabla con el nombre de la tabla actual, para que la consulta devuelva solo las llaves foraneas de esa tabla
            cmd.Parameters.AddWithValue("@tabla", _tabla);
            using var rd = cmd.ExecuteReader();
            // Mientras haya registros, agregamos al diccionario el nombre de la columna, y la tabla y columna a la que hace referencia
            while (rd.Read())
            {
                resultado[rd.GetString(0)] = (rd.GetString(1), rd.GetString(2));
            }
            return resultado;
        }

        private ComboBox CrearCombo(SqlConnection con, string tablaRef, string columnaRef, int y)
        {
            var dt = new DataTable();
            using (var da = new SqlDataAdapter("SELECT * FROM " + tablaRef, con))
            {
                da.Fill(dt);
            }

            // El simbolo ? indica que la variable puede ser nula
            string? columnaTexto = null;
            foreach (DataColumn c in dt.Columns)
            {
                if (c.DataType == typeof(string)) { columnaTexto = c.ColumnName; break; }
            }

            // Si no hay columna de texto, se mostrará solo el valor de la columna referenciada, si hay una columna de texto, se mostrará el valor de la columna referenciada seguido del valor de la columna de texto
            string expresion = columnaTexto == null
                ? $"Convert([{columnaRef}], 'System.String')"
                : $"Convert([{columnaRef}], 'System.String') + ' - ' + [{columnaTexto}]";
            dt.Columns.Add("_texto", typeof(string), expresion);

            var cb = new ComboBox
            {
                Left = 130,
                Top = y,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                // Columna que se mostrara al usuario
                DisplayMember = "_texto",
                // Columna que se usara como valor al insertar en la tabla
                ValueMember = columnaRef,
                DataSource = dt
            };
            cb.SelectedIndex = -1; // empieza sin selección
            return cb;
        }

        // Lee el valor de un control (TextBox o ComboBox) validando que no este vacio si es obligatorio, y convirtiendolo al tipo de la columna
        // Devuelve false si la validacion falla
        private bool TryLeerValor(string nombre, Control ctrl, out object valor)
        {
            // Obtenemos la columna correspondiente del esquema segun el nombre
            DataColumn col = _esquema.Columns[nombre]!;
            object? v;
            bool vacio;

            if (ctrl is ComboBox cb)
            {
                v = cb.SelectedValue;
                vacio = v == null;
            }
            else
            {
                string texto = ctrl.Text.Trim();
                vacio = texto == "";
                // Convierte el texto al tipo de dato de la columna (int, decimal, string, etc.)
                v = vacio ? null : Convert.ChangeType(texto, col.DataType);
            }

            if (vacio)
            {
                // Si el campo esta vacio y la columna no permite nulos, es obligatorio
                valor = DBNull.Value;
                if (!col.AllowDBNull)
                {
                    MessageBox.Show($"El campo {nombre} es obligatorio.");
                    return false;
                }
                return true;
            }

            valor = v!;
            return true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                using var con = Conexion.Crear();
                using var cmd = new SqlCommand { Connection = con };

                if (_fila == null)
                {
                    // INSERTAR
                    // Si la tabla es Inscripcion, se usa el procedimiento almacenado
                    bool usarProcedimiento = _tabla == "Inscripcion";

                    // Listas para almacenar los nombres de las columnas y los parametros que se usaran en la consulta
                    var columnas = new List<string>();
                    var parametros = new List<string>();

                    // Recorremos el diccionario de campos, que contiene los controles y sus nombres de columna
                    foreach (var par in _campos)
                    {
                        // Leemos y validamos el valor del control; si no es valido, TryLeerValor ya aviso al usuario
                        if (!TryLeerValor(par.Key, par.Value, out object valor)) return;

                        // El procedimiento espera parametros con el mismo nombre que la columna (@IdAlumno, @IdMateria, @Calificacion)
                        // Para el INSERT normal se usa el prefijo @v_
                        string nombreParametro = usarProcedimiento ? "@" + par.Key : "@v_" + par.Key;

                        // Los valores siempre van como parametros para evitar inyeccion SQL
                        cmd.Parameters.AddWithValue(nombreParametro, valor);
                        columnas.Add("[" + par.Key + "]");
                        parametros.Add("@v_" + par.Key);
                    }

                    if (usarProcedimiento)
                    {
                        // Ejecuta sp_InscribirAlumno en lugar de un INSERT directo
                        cmd.CommandText = "sp_InscribirAlumno";
                        cmd.CommandType = CommandType.StoredProcedure;
                    }
                    else
                    {
                        // Intruccion SQL formateada 
                        cmd.CommandText = $"INSERT INTO {_tabla} ({string.Join(", ", columnas)}) VALUES ({string.Join(", ", parametros)})";
                    }
                }
                else
                {
                    // MODIFICAR
                    // Sin llave primaria no podriamos ubicar la fila exacta y el UPDATE cambiaria TODAS las filas, asi que no se permite
                    if (_esquema.PrimaryKey.Length == 0)
                    {
                        MessageBox.Show("La tabla no tiene llave primaria; no se puede modificar de forma segura.");
                        return;
                    }

                    // Lista con las asignaciones del SET, por ejemplo: [Nombre] = @v_Nombre
                    var sets = new List<string>();
                    foreach (var par in _campos)
                    {
                        DataColumn col = _esquema.Columns[par.Key]!;
                        // Las columnas de la llave primaria no se modifican, se omiten del SET
                        if (_esquema.PrimaryKey.Contains(col)) continue;

                        if (!TryLeerValor(par.Key, par.Value, out object valor)) return;
                        cmd.Parameters.AddWithValue("@v_" + par.Key, valor);
                        sets.Add($"[{par.Key}] = @v_{par.Key}");
                    }

                    // Si todas las columnas son parte de la PK, no hay nada que modificar
                    if (sets.Count == 0)
                    {
                        MessageBox.Show("Esta tabla no tiene campos modificables.");
                        return;
                    }

                    // La fila exacta se ubica con los valores ORIGINALES de la PK (tomados de _fila, no de los controles)
                    var where = new List<string>();
                    foreach (DataColumn pk in _esquema.PrimaryKey)
                    {
                        cmd.Parameters.AddWithValue("@pk_" + pk.ColumnName, _fila[pk.ColumnName]);
                        where.Add($"[{pk.ColumnName}] = @pk_{pk.ColumnName}");
                    }

                    // Instruccion SQL formateada para actualizar solo la fila seleccionada
                    cmd.CommandText = $"UPDATE {_tabla} SET {string.Join(", ", sets)} WHERE {string.Join(" AND ", where)}";
                }

                con.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show(_fila == null ? "Registro guardado." : "Registro modificado.");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Algún valor no tiene el formato correcto (revisa los campos numéricos).");
            }
            catch (SqlException ex)
            {
                // El numero de error identifica el problema
                string mensaje = ex.Number switch
                {
                    // 547: violacion de CHECK o de llave foranea
                    547 => "El valor no cumple una regla de la base de datos (por ejemplo, un rango permitido).\n\nDetalle: " + ex.Message,
                    // 2601 y 2627: valor duplicado en un indice unico o llave primaria
                    2601 or 2627 => "Ya existe un registro con ese valor (dato duplicado).",
                    // 50000 o mas: errores propios lanzados con THROW en el procedimiento y los triggers
                    >= 50000 => ex.Message,
                    _ => "Error de base de datos: " + ex.Message
                };
                MessageBox.Show(mensaje);
            }
        }

        // Estilo visual 
        private void AplicarEstilo()
        {
            // Fuerza la disposicion
            pnlBotones.Dock = DockStyle.Bottom;
            pnlBotones.Height = 60;
            pnlCampos.Dock = DockStyle.Fill;
            pnlCampos.AutoScroll = true;
            pnlCampos.Padding = new Padding(10);
            pnlCampos.BringToFront(); // el panel que llena se acomoda al final, despues de los de borde

            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10F);

            EstiloBoton(btnGuardar, Color.FromArgb(91, 42, 134));
            EstiloBoton(btnCancelar, Color.FromArgb(177, 74, 237));

            // Los botones se colocan en el panel inferior
            btnGuardar.Location = new Point(70, 12);
            btnCancelar.Location = new Point(200, 12);
        }

        private void EstiloBoton(Button b, Color color)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = color;
            b.ForeColor = Color.White;
            b.Cursor = Cursors.Hand;
            b.Size = new Size(110, 36);
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void pnlCampos_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
