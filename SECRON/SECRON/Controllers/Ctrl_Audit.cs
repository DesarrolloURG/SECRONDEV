// Ctrl_Audit.cs (NOMBRE NUEVO)
using SECRON.Configuration;
using SECRON.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SECRON.Controllers
{
    internal class Ctrl_Audit
    {
        // MÉTODO PRINCIPAL: Registrar auditoría
        public static int RegistrarAuditoria(Mdl_Audit audit)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_AuditLog_Insert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", audit.UserId);
                    cmd.Parameters.AddWithValue("@Action", audit.Action ?? "");
                    cmd.Parameters.AddWithValue("@TableName", (object)audit.TableName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RecordId", (object)audit.RecordId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OldValues", (object)audit.OldValues ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NewValues", (object)audit.NewValues ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress", (object)audit.IPAddress ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserAgent", (object)audit.UserAgent ?? DBNull.Value);

                    object result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar auditoría: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // MÉTODO AUXILIAR: Crear registro de auditoría simple
        public static void RegistrarAccion(int userId, string action, string tableName,
            int? recordId = null, string details = null)
        {
            try
            {
                var audit = new Mdl_Audit
                {
                    UserId = userId,
                    Action = action,
                    TableName = tableName,
                    RecordId = recordId,
                    NewValues = details
                };

                RegistrarAuditoria(audit);
            }
            catch { }
        }
    }
}