// Ctrl_Audit.cs (NOMBRE NUEVO)
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

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
                {
                    string query = @"INSERT INTO AuditLog (UserId, Action, TableName, RecordId, 
                        OldValues, NewValues, IPAddress, UserAgent) 
                        VALUES (@UserId, @Action, @TableName, @RecordId, @OldValues, 
                        @NewValues, @IPAddress, @UserAgent)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", audit.UserId);
                        cmd.Parameters.AddWithValue("@Action", audit.Action ?? "");
                        cmd.Parameters.AddWithValue("@TableName", (object)audit.TableName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RecordId", (object)audit.RecordId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OldValues", (object)audit.OldValues ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValues", (object)audit.NewValues ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IPAddress", (object)audit.IPAddress ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserAgent", (object)audit.UserAgent ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
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
                    NewValues = details // Usar NewValues para detalles generales
                };

                RegistrarAuditoria(audit);
            }
            catch { }
        }
    }
}