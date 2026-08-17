using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Models;
using SECRON.Configuration;

namespace SECRON.Controllers
{
    internal class Ctrl_ResponsibilityLetters
    {
        public static int SubirCarta(int assetId, int employeeId, string filePath, string fileName, int uploadedByUserId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_ResponsibilityLetters_Upload", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AssetId", assetId);
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@FilePath", filePath);
                    cmd.Parameters.AddWithValue("@FileName", fileName);
                    cmd.Parameters.AddWithValue("@UploadedByUserId", uploadedByUserId);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al subir carta de responsabilidad: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int DesvincularCarta(int assetId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_ResponsibilityLetters_Unlink", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AssetId", assetId);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desvincular carta de responsabilidad: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static Mdl_ResponsibilityLetterMaster ObtenerCartaVigente(int assetId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT m.ResponsibilityLetterId, m.EmployeeId, m.FilePath, m.FileName,
                               m.UploadDate, m.UploadedByUserId, m.IsActive
                        FROM ResponsibilityLetterDetail d
                        INNER JOIN ResponsibilityLetterMaster m ON d.ResponsibilityLetterId = m.ResponsibilityLetterId
                        WHERE d.AssetId = @AssetId
                          AND d.IsCurrent = 1
                          AND d.IsActive = 1
                          AND m.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@AssetId", assetId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Mdl_ResponsibilityLetterMaster
                                {
                                    ResponsibilityLetterId = reader.GetInt32(reader.GetOrdinal("ResponsibilityLetterId")),
                                    EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FilePath = reader["FilePath"].ToString(),
                                    FileName = reader["FileName"].ToString(),
                                    UploadDate = reader.GetDateTime(reader.GetOrdinal("UploadDate")),
                                    UploadedByUserId = reader.GetInt32(reader.GetOrdinal("UploadedByUserId")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar carta vigente: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }
    }
}