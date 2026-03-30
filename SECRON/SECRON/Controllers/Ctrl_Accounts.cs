using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_Accounts
    {
        // MÉTODO PRINCIPAL: Registrar cuenta
        public static int RegistrarCuenta(Mdl_Accounts cuenta)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"INSERT INTO Accounts (Code, Name, Type, ParentAccountCode, Level, 
                        Sign, Balance, BankCode, BankName, BankAccountType, CheckNumber, Currency, CurrencyName) 
                        VALUES (@Code, @Name, @Type, @ParentAccountCode, @Level, @Sign, @Balance, 
                        @BankCode, @BankName, @BankAccountType, @CheckNumber, @Currency, @CurrencyName)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Code", cuenta.Code ?? "");
                        cmd.Parameters.AddWithValue("@Name", cuenta.Name ?? "");
                        cmd.Parameters.AddWithValue("@Type", (object)cuenta.Type ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ParentAccountCode", cuenta.ParentAccountCode ?? "");
                        cmd.Parameters.AddWithValue("@Level", cuenta.Level);
                        cmd.Parameters.AddWithValue("@Sign", cuenta.Sign ?? "");
                        cmd.Parameters.AddWithValue("@Balance", cuenta.Balance);
                        cmd.Parameters.AddWithValue("@BankCode", cuenta.BankCode);
                        cmd.Parameters.AddWithValue("@BankName", cuenta.BankName ?? "");
                        cmd.Parameters.AddWithValue("@BankAccountType", cuenta.BankAccountType ?? "");
                        cmd.Parameters.AddWithValue("@CheckNumber", cuenta.CheckNumber);
                        cmd.Parameters.AddWithValue("@Currency", cuenta.Currency ?? "");
                        cmd.Parameters.AddWithValue("@CurrencyName", cuenta.CurrencyName ?? "");

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar cuenta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Mostrar todas las cuentas
        public static List<Mdl_Accounts> MostrarCuentas()
        {
            List<Mdl_Accounts> lista = new List<Mdl_Accounts>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Accounts ORDER BY Code";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCuenta(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener cuentas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Búsqueda avanzada
        public static List<Mdl_Accounts> BusquedaAvanzada(string campo, string busqueda)
        {
            List<Mdl_Accounts> lista = new List<Mdl_Accounts>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = $"SELECT * FROM Accounts WHERE {campo} LIKE @Busqueda";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Busqueda", busqueda + "%");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCuenta(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en búsqueda: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }

        // MÉTODO PRINCIPAL: Validar si existe registro
        public static bool ValidarRegistro(string campo, string valor)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = $"SELECT COUNT(*) FROM Accounts WHERE {campo} = @Valor";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Valor", valor ?? "");
                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch { return false; }
        }

        // MÉTODO PRINCIPAL: Modificar cuenta
        public static int ModificarCuenta(Mdl_Accounts cuenta)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"UPDATE Accounts SET Name = @Name, Type = @Type, 
                        ParentAccountCode = @ParentAccountCode, Level = @Level, Sign = @Sign, 
                        Balance = @Balance, BankCode = @BankCode, BankName = @BankName, 
                        BankAccountType = @BankAccountType, CheckNumber = @CheckNumber, 
                        Currency = @Currency, CurrencyName = @CurrencyName 
                        WHERE Code = @Code";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Code", cuenta.Code ?? "");
                        cmd.Parameters.AddWithValue("@Name", cuenta.Name ?? "");
                        cmd.Parameters.AddWithValue("@Type", (object)cuenta.Type ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ParentAccountCode", cuenta.ParentAccountCode ?? "");
                        cmd.Parameters.AddWithValue("@Level", cuenta.Level);
                        cmd.Parameters.AddWithValue("@Sign", cuenta.Sign ?? "");
                        cmd.Parameters.AddWithValue("@Balance", cuenta.Balance);
                        cmd.Parameters.AddWithValue("@BankCode", cuenta.BankCode);
                        cmd.Parameters.AddWithValue("@BankName", cuenta.BankName ?? "");
                        cmd.Parameters.AddWithValue("@BankAccountType", cuenta.BankAccountType ?? "");
                        cmd.Parameters.AddWithValue("@CheckNumber", cuenta.CheckNumber);
                        cmd.Parameters.AddWithValue("@Currency", cuenta.Currency ?? "");
                        cmd.Parameters.AddWithValue("@CurrencyName", cuenta.CurrencyName ?? "");

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar cuenta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Actualizar niveles de cuentas hijas (recursivo)
        public static int ActualizarNiveles(string parentAccountCode, int parentLevel)
        {
            try
            {
                int updatedLevel = parentLevel + 1;
                int resultado = 0;

                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // Actualizar nivel de cuentas hijas
                    string query = "UPDATE Accounts SET Level = @UpdatedLevel WHERE ParentAccountCode = @ParentAccountCode";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UpdatedLevel", updatedLevel);
                        cmd.Parameters.AddWithValue("@ParentAccountCode", parentAccountCode ?? "");
                        resultado = cmd.ExecuteNonQuery();
                    }

                    // Si se actualizó alguna cuenta, actualizar recursivamente las subcuentas
                    if (resultado > 0)
                    {
                        string subcuentasQuery = "SELECT Code FROM Accounts WHERE ParentAccountCode = @ParentAccountCode";
                        using (SqlCommand subcmd = new SqlCommand(subcuentasQuery, connection))
                        {
                            subcmd.Parameters.AddWithValue("@ParentAccountCode", parentAccountCode ?? "");
                            using (SqlDataReader reader = subcmd.ExecuteReader())
                            {
                                List<string> subcuentas = new List<string>();
                                while (reader.Read())
                                {
                                    subcuentas.Add(reader["Code"].ToString());
                                }
                                reader.Close();

                                foreach (string codigo in subcuentas)
                                {
                                    ActualizarNiveles(codigo, updatedLevel);
                                }
                            }
                        }
                    }
                }
                return resultado;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar niveles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO PRINCIPAL: Buscar campo por nombre
        public static string BuscarCampo(int posicionCampo, string accountName)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Accounts WHERE Name = @AccountName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AccountName", accountName ?? "");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if ((posicionCampo == 0) || (posicionCampo == 5) || (posicionCampo == 8) || (posicionCampo == 11))
                                {
                                    return reader.GetInt32(posicionCampo).ToString();
                                }
                                else if (posicionCampo == 7)
                                {
                                    return reader.GetDecimal(posicionCampo).ToString();
                                }
                                else
                                {
                                    return reader.GetString(posicionCampo);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar campo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "NO SE ENCONTRO";
        }

        // MÉTODO PRINCIPAL: Buscar campo por código
        public static string BuscarCampoPorCodigo(int posicionCampo, string accountCode)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Accounts WHERE Code = @AccountCode";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AccountCode", accountCode ?? "");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if ((posicionCampo == 0) || (posicionCampo == 5) || (posicionCampo == 8) || (posicionCampo == 11))
                                {
                                    return reader.GetInt32(posicionCampo).ToString();
                                }
                                else if (posicionCampo == 7)
                                {
                                    return reader.GetDecimal(posicionCampo).ToString();
                                }
                                else
                                {
                                    return reader.GetString(posicionCampo);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar campo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "NO SE ENCONTRO";
        }

        // ActualizarSaldo que verifica el SIGNO
        public static int ActualizarSaldo(string accountName, decimal debit, decimal credit)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    // 1. Obtener el SIGNO de la cuenta
                    string querySigno = "SELECT Sign FROM Accounts WHERE Name = @AccountName";
                    SqlCommand cmdSigno = new SqlCommand(querySigno, connection);
                    cmdSigno.Parameters.AddWithValue("@AccountName", accountName ?? "");
                    string signo = cmdSigno.ExecuteScalar()?.ToString() ?? "+";

                    // 2. Calcular el monto según el SIGNO
                    decimal monto = 0;

                    if (signo == "+")  // ACTIVO, GASTOS
                    {
                        if (debit > 0)
                            monto = debit;      // Cargo = suma
                        else
                            monto = -credit;    // Abono = resta
                    }
                    else  // PASIVO, PATRIMONIO, INGRESOS (signo == "-")
                    {
                        if (credit > 0)
                            monto = credit;     // Abono = suma
                        else
                            monto = -debit;     // Cargo = resta
                    }

                    // 3. Actualizar el saldo
                    string queryUpdate = "UPDATE Accounts SET Balance = (Balance + @Monto) WHERE Name = @AccountName";
                    SqlCommand cmdUpdate = new SqlCommand(queryUpdate, connection);
                    cmdUpdate.Parameters.AddWithValue("@AccountName", accountName ?? "");
                    cmdUpdate.Parameters.AddWithValue("@Monto", monto);

                    return cmdUpdate.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar saldo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Mapear cuenta
        private static Mdl_Accounts MapearCuenta(SqlDataReader reader)
        {
            return new Mdl_Accounts
            {
                AccountId = reader.GetInt32(0),
                Code = reader[1].ToString(),
                Name = reader[2].ToString(),
                Type = reader[3] == DBNull.Value ? null : reader[3].ToString(),
                ParentAccountCode = reader[4].ToString(),
                Level = reader.GetInt32(5),
                Sign = reader[6].ToString(),
                Balance = reader.GetDecimal(7),
                BankCode = reader.GetInt32(8),
                BankName = reader[9].ToString(),
                BankAccountType = reader[10].ToString(),
                CheckNumber = reader.GetInt32(11),
                Currency = reader[12].ToString(),
                CurrencyName = reader[13].ToString()
            };
        }
        // MÉTODO PARA FORMULARIO DE BÚSQUEDA: Obtener todas las cuentas
        public static List<Mdl_Accounts> ObtenerTodasLasCuentas()
        {
            return MostrarCuentas(); // Reutiliza el método existente
        }

        // MÉTODO PARA FORMULARIO DE BÚSQUEDA: Buscar cuentas por código o nombre
        public static List<Mdl_Accounts> BuscarCuentas(string textoBusqueda)
        {
            List<Mdl_Accounts> lista = new List<Mdl_Accounts>();
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"SELECT * FROM Accounts 
                           WHERE Code LIKE @Busqueda 
                           OR Name LIKE @Busqueda 
                           ORDER BY Code";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Busqueda", "%" + textoBusqueda + "%");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(MapearCuenta(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cuentas: " + ex.Message,
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return lista;
        }
        // MÉTODO PRINCIPAL: Obtener cuenta completa por AccountId
        public static Mdl_Accounts ObtenerCuentaPorId(int accountId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT * FROM Accounts WHERE AccountId = @AccountId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AccountId", accountId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapearCuenta(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener cuenta: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }
        // MÉTODO: Obtener código de cuenta por AccountId
        public static string ObtenerCodigoCuenta(int accountId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT Code FROM Accounts WHERE AccountId = @AccountId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AccountId", accountId);
                        object result = cmd.ExecuteScalar();
                        return result?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener código de cuenta: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        // MÉTODO: Obtener nombre de cuenta por AccountId
        public static string ObtenerNombreCuenta(int accountId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = "SELECT Name FROM Accounts WHERE AccountId = @AccountId";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AccountId", accountId);
                        object result = cmd.ExecuteScalar();
                        return result?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener nombre de cuenta: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }
    }
}