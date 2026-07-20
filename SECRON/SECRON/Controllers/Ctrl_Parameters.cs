using SECRON.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SECRON.Controllers
{
    internal class Ctrl_Parameters
    {
        // Obtiene un parámetro entero desde ParametersConfiguration (async)
        public static async Task<int> ObtenerValorIntAsync(string parameterName, int valorPorDefecto)
        {
            try
            {
                using (var connection = new SqlConnection(DatabaseConfig.GetConnectionString()))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SP_Parameters_GetValue", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ParameterName", parameterName);

                        var result = await command.ExecuteScalarAsync();
                        if (result != null && int.TryParse(result.ToString(), out int valor))
                            return valor;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener parámetro {parameterName}: {ex.Message}");
            }
            return valorPorDefecto;
        }
    }
}