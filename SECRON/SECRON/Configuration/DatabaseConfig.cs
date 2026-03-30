using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Configuration
{
    internal class DatabaseConfig
    {
        #region CadenaDeConexion
        // Cadena de conexión centralizada BASE DE DATOS PRODUCCIÓN
        //private static readonly string connectionString = "Initial Catalog=SECRON; User ID=sa; Password=URdatabase24.; Data Source=172.16.0.153";
        // Cadena de conexión centralizada BASE DE DATOS QA
        //private static readonly string connectionString = "Initial Catalog=SECRONQA; User ID=sa; Password=URdatabase24.; Data Source=172.16.0.153";
        // Cadena de conexión centralizada BASE DE DATOS DEVELOPMENT
        private static readonly string connectionString = "Initial Catalog=SECRONDEV; User ID=sa; Password=URdatabase24.; Data Source=172.16.0.153";
        // Cadena de conexión alternativa para entorno de desarrollo local
        //private static readonly string connectionString = "Initial Catalog=SECRON; User ID=sa; Password=URdatabase24.; Data Source=DESKTOP-G7KKLEL\\SQLEXPRESS";
        #endregion CadenaDeConexion
        #region ProcedimientosDeConexion
        // Método original de iniciar conexión
        public static SqlConnection StartConection()
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            return sqlConnection;
        }
        // Nuevo método para el AuthController y otros controllers
        public static string GetConnectionString()
        {
            return connectionString;
        }
        // Método adicional para obtener conexión sin abrir (más flexible)
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
        // Método para probar la conexión
        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log del error si tienes sistema de logging
                System.Diagnostics.Debug.WriteLine($"Error probando conexión: {ex.Message}");
                return false;
            }
        }
        // Método async para obtener conexión (para métodos async)
        public static async Task<SqlConnection> GetConnectionAsync()
        {
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
        // Propiedades para obtener componentes individuales de la conexión en caso de ser necesario
        public static class ConnectionInfo
        {
            public static string Server => "172.16.0.153";
            public static string Database => "SECRON";
            public static string UserId => "sa";
            // No exponemos la contraseña por seguridad
        }
        #endregion ProcedimientosDeConexion
    }
}