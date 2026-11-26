using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Xml.Serialization;

namespace CrudEscuela
{
    public static class Eliminador
    {
        private class LlaveForanea
        {
            public string TablaHija = ""; //Nombre de la tabla que depende de otra
            // Almacenamos las llaves foraneas que apuntan a la tabla padre, ya que puede haber mas de una
            public List<(string ColHija, string ColPadre)> Columnas = new();
        }

        private static List<LlaveForanea> ObtenerHijas(SqlConnection con, string tabla)
        {
            // Se relaciona primero cada llave foranea con las columnas que utiliza
            // Luego obtenemos el nombre de la columna en la tabla hija y el nombre de la columna en la tabla padre
            // Con WHERE buscamos las llaves foraneas que apunten a la tabla padre
            string sql = @"
                SELECT fk.object_id,
                       OBJECT_NAME(fk.parent_object_id),
                       cp.name,
                       cr.name
                FROM sys.foreign_keys fk
                JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
                JOIN sys.columns cp ON cp.object_id = fkc.parent_object_id
                                   AND cp.column_id = fkc.parent_column_id
                JOIN sys.columns cr ON cr.object_id = fkc.referenced_object_id
                                   AND cr.column_id = fkc.referenced_column_id
                WHERE fk.referenced_object_id = OBJECT_ID(@tabla)
                ORDER BY fk.object_id, fkc.constraint_column_id";

            // Guardamos cada llave foranea y las columnas que la componen en un diccionario
            var dict = new Dictionary<int, LlaveForanea>();
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@tabla", tabla);
            using var rd = cmd.ExecuteReader();
            while (rd.Read()) 
            {
                int id = rd.GetInt32(0);
                // Buscamos en el diccionario si ya tenemos la llave foranea; si no, la agregamos
                if (!dict.TryGetValue(id, out var fk))
                {
                    fk = new LlaveForanea { TablaHija = rd.GetString(1) };
                    dict[id] = fk;
                }
                // Agregamos la columna que compone la llave foranea
                fk.Columnas.Add((rd.GetString(2), rd.GetString(3)));
            }
            // Devolvemos la lista de llaves foraneas que apuntan a la tabla padre
            return dict.Values.ToList();
        }

        // Aplicamos recursivida; por ejemplo, si Alumnos -> Inscripciones -> Calificaciones -> DetallesCalificaciones
        // Primero se eliminan los detalles de calificaciones, luego las calificaciones, luego las inscripciones y al final el alumno
        // La condicion permite conocer los registros que estan relacionados con la tabla original
        // La ruta permite evitar caer en un recorrido cirular infinito
        public static void RecolectarDependientes(SqlConnection con, string tabla, string condicion, List<(string tabla, string condicion)> lista, List<string> ruta) 
        {
            ruta.Add(tabla);
            // Recorremos todas las tablas que dependen de la tabla actual; si alguna de ellas ya esta en la ruta, significa que hay una relacion circular
            foreach (var fk in ObtenerHijas(con, tabla))
            {
                if (ruta.Contains(fk.TablaHija)) 
                {
                    throw new InvalidOperationException($"Se detecto una relacion circular con la tabla {fk.TablaHija}; no se puede eliminar automaticamente.");
                }

                // Creamos la condicion para la tabla hija
                var uniones = fk.Columnas.Select(c => $"[{tabla}].[{c.ColPadre}] = [{fk.TablaHija}].[{c.ColHija}]");
                // La condicion de la tabla hija es que exista un registro en la tabla hija que cumpla con la condicion de la tabla padre y que ademas cumpla con las uniones de las llaves foraneas
                string condHija = $"EXISTS (SELECT 1 FROM [{tabla}] WHERE ({condicion}) AND {string.Join(" AND ", uniones)})";

                RecolectarDependientes(con, fk.TablaHija, condHija, lista, ruta);
                lista.Add((fk.TablaHija, condHija));
            }
            // Eliminamos el ultimo elemento de la ruta, ya que hemos terminado de recorrer todas las tablas dependientes de la tabla actual
            ruta.RemoveAt(ruta.Count - 1);
        }

        private static SqlCommand CrearComando(string sql, SqlConnection con, SqlTransaction? tx,
            List<(string nombre, object valor)> pars)
        {
            var cmd = new SqlCommand(sql, con);
            cmd.Transaction = tx;
            foreach (var p in pars)
                cmd.Parameters.AddWithValue(p.nombre, p.valor);
            return cmd;
        }

        public static bool Eliminar(string tabla, DataRow fila)
        {
            try 
            { 
                using var con = Conexion.Crear();
                con.Open();

                var esquema = new DataTable();
                using (var da = new SqlDataAdapter("SELECT * FROM " + tabla, con))
                {
                    da.FillSchema(esquema, SchemaType.Source);
                }
                // Verificmaos que exista llave primaria; si no, no podemos eliminar de forma segura
                if (esquema.PrimaryKey.Length == 0) 
                {
                    MessageBox.Show("La tabla no tiene llave primaria; no se puede eliminar de forma segura.");
                    return false;
                }

                var pars = new List<(string nombre, object valor)>();
                var condiciones = new List<string>();
                foreach (DataColumn pk in esquema.PrimaryKey) 
                {
                    pars.Add(("@pk_" + pk.ColumnName, fila[pk.ColumnName]));
                    condiciones.Add($"[{tabla}].[{pk.ColumnName}] = @pk_{pk.ColumnName}");
                }
                string condicion = string.Join(" AND ", condiciones);

                var dependientes = new List<(string tabla, string condicion)>();
                RecolectarDependientes(con, tabla, condicion, dependientes, new List<string>());

                var resumen = new Dictionary<string, int>();
                foreach (var d in dependientes)
                {
                    using var cmdContar = CrearComando($"SELECT COUNT(*) FROM [{d.tabla}] WHERE {d.condicion}", con, null, pars);
                    int n = (int)cmdContar.ExecuteScalar()!;
                    if (n > 0)
                    {
                        // Alamcenamos el numero de registros que se eliminaran de cada tabla dependiente
                        resumen.TryGetValue(d.tabla, out int previo);
                        resumen[d.tabla] = previo + n;
                    }
                }

                string mensaje;
                MessageBoxIcon icono;
                if (resumen.Count == 0)
                {
                    mensaje = "¿Deseas eliminar este registro?";
                    icono = MessageBoxIcon.Question;
                }
                else
                {
                    string detalle = string.Join("\n", resumen.Select(r => $"  • {r.Key}: {r.Value} registro(s)"));
                    mensaje = "Este registro esta referenciado en otras tablas:\n\n" + detalle +
                              "\n\nSi continuas, tambien se eliminaran esos registros relacionados. " +
                              "Si cancelas, no se borrara nada.\n\n¿Deseas continuar?";
                    icono = MessageBoxIcon.Warning;
                }

                var respuesta = MessageBox.Show(mensaje, "Confirmar eliminacion",
                    MessageBoxButtons.YesNo, icono, MessageBoxDefaultButton.Button2);

                if (respuesta != DialogResult.Yes) return false;

                // Empleamos una transaccion para que si algo falla, se deshagan todos los cambios
                using var tx = con.BeginTransaction();
                try
                {
                    // Primero las tablas dependientes, de la mas lejana a la mas cercana
                    foreach (var d in dependientes)
                    {
                        using var cmdBorrar = CrearComando($"DELETE FROM [{d.tabla}] WHERE {d.condicion}", con, tx, pars);
                        cmdBorrar.ExecuteNonQuery();
                    }

                    // Al final, el registro padre
                    using var cmdPadre = CrearComando($"DELETE FROM [{tabla}] WHERE {condicion}", con, tx, pars);
                    cmdPadre.ExecuteNonQuery();

                    // Todo salio bien: confirmamos los cambios
                    tx.Commit();
                }
                catch
                {
                    // Algo fallo: deshacemos TODO lo que se alcanzo a borrar
                    tx.Rollback();
                    throw;
                }

                MessageBox.Show("Registro eliminado.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar; no se borro nada.\n\n" + ex.Message);
                return false;
            }
        }
    }

}
