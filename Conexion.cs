using Microsoft.Data.SqlClient;

namespace CrudEscuela
{

    public static class Conexion
    {
        private const string CadenaConexion = 
            "Server=Salvador;Database=CrudEscuela;Integrated Security=true; TrustServerCertificate=true;";


        public static SqlConnection Crear()
        { 
            return new SqlConnection(CadenaConexion);
        }
    }
}
