using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_BanksAccountTypes
    {
        public static List<Mdl_BanksAccountTypes> MostrarTiposCuenta(bool soloActivos = true)
        {
            var lista = new List<Mdl_BanksAccountTypes>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT BanksAccountTypeId, BanksAccountTypeCode, BanksAccountTypeName, IsActive ");
                    query.Append("FROM BanksAccountTypes WHERE 1=1");

                    if (soloActivos)
                        query.Append(" AND IsActive = 1");

                    query.Append(" ORDER BY BanksAccountTypeCode ASC");

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Mdl_BanksAccountTypes
                            {
                                BanksAccountTypeId = reader.GetInt32(0),
                                BanksAccountTypeCode = reader[1].ToString(),
                                BanksAccountTypeName = reader[2].ToString(),
                                IsActive = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tipos de cuenta bancaria: " + ex.Message,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static List<KeyValuePair<int, string>> ObtenerTiposCuentaParaCombo(bool soloActivos = true)
        {
            var lista = new List<KeyValuePair<int, string>>();
            foreach (var tipo in MostrarTiposCuenta(soloActivos))
            {
                lista.Add(new KeyValuePair<int, string>(
                    tipo.BanksAccountTypeId,           
                    tipo.BanksAccountTypeName          
                ));
            }
            return lista;
        }
    }
}
