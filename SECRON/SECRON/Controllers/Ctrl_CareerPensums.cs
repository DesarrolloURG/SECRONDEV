using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_CareerPensums
    {
        #region Consultas

        public static List<Mdl_CareerPensums> MostrarPensums(string texto, int? careerId, bool? isActive)
        {
            List<Mdl_CareerPensums> lista = new List<Mdl_CareerPensums>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT
                            p.CareerPensumId, p.CareerId, p.PensumCode, p.PensumName,
                            c.CareerCode, c.CareerName,
                            p.IsCurrent, p.IsActive, p.CreatedDate, p.CreatedBy, p.ModifiedDate, p.ModifiedBy
                        FROM CareerPensums p
                        INNER JOIN Careers c ON p.CareerId = c.CareerId
                        WHERE (@CareerId IS NULL OR p.CareerId = @CareerId)
                          AND (@IsActive IS NULL OR p.IsActive = @IsActive)
                          AND (@Texto = '' OR p.PensumCode LIKE '%' + @Texto + '%'
                                            OR p.PensumName LIKE '%' + @Texto + '%'
                                            OR c.CareerName LIKE '%' + @Texto + '%')
                        ORDER BY c.CareerName, p.PensumCode";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CareerId", (object)careerId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", (object)isActive ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Texto", (texto ?? "").Trim().ToUpper());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearPensum(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER PENSUMS: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static Mdl_CareerPensums ObtenerPensumPorId(int careerPensumId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT
                            p.CareerPensumId, p.CareerId, p.PensumCode, p.PensumName,
                            c.CareerCode, c.CareerName,
                            p.IsCurrent, p.IsActive, p.CreatedDate, p.CreatedBy, p.ModifiedDate, p.ModifiedBy
                        FROM CareerPensums p
                        INNER JOIN Careers c ON p.CareerId = c.CareerId
                        WHERE p.CareerPensumId = @CareerPensumId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CareerPensumId", careerPensumId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapearPensum(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL OBTENER EL PENSUM: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        #endregion

        #region CRUD

        public static int RegistrarPensum(Mdl_CareerPensums pensum)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CareerPensums_Insert", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CareerId", pensum.CareerId);
                    cmd.Parameters.AddWithValue("@PensumCode", pensum.PensumCode);
                    cmd.Parameters.AddWithValue("@PensumName", pensum.PensumName);
                    cmd.Parameters.AddWithValue("@IsCurrent", pensum.IsCurrent);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object)pensum.CreatedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL REGISTRAR EL PENSUM: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarPensum(Mdl_CareerPensums pensum)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CareerPensums_Update", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CareerPensumId", pensum.CareerPensumId);
                    cmd.Parameters.AddWithValue("@CareerId", pensum.CareerId);
                    cmd.Parameters.AddWithValue("@PensumCode", pensum.PensumCode);
                    cmd.Parameters.AddWithValue("@PensumName", pensum.PensumName);
                    cmd.Parameters.AddWithValue("@IsCurrent", pensum.IsCurrent);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)pensum.ModifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL ACTUALIZAR EL PENSUM: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int CambiarEstadoPensum(int careerPensumId, bool isActive, int? modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CareerPensums_Delete", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CareerPensumId", careerPensumId);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CAMBIAR ESTADO DEL PENSUM: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region GuardarArbolCompleto

        // Invoca SP_CareerPensums_SaveTree: crea o actualiza el pensum completo
        // (cabecera + cursos + prerequisitos) en una sola transacción.
        // Retorna: > 0 = CareerPensumId, -1 = código duplicado, -2 = prerequisito inválido, 0 = error
        public static int GuardarArbolCompleto(string pensumJson, int? userId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                using (SqlCommand cmd = new SqlCommand("SP_CareerPensums_SaveTree", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;
                    cmd.Parameters.AddWithValue("@PensumJson", pensumJson);
                    cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL GUARDAR EL PENSUM: " + ex.Message, "ERROR SECRON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        #endregion

        #region Métodos auxiliares

        private static Mdl_CareerPensums MapearPensum(SqlDataReader reader)
        {
            return new Mdl_CareerPensums
            {
                CareerPensumId = Convert.ToInt32(reader["CareerPensumId"]),
                CareerId = Convert.ToInt32(reader["CareerId"]),
                PensumCode = reader["PensumCode"]?.ToString(),
                PensumName = reader["PensumName"]?.ToString(),
                CareerCode = reader["CareerCode"]?.ToString(),
                CareerName = reader["CareerName"]?.ToString(),
                IsCurrent = reader["IsCurrent"] != DBNull.Value && Convert.ToBoolean(reader["IsCurrent"]),
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["CreatedDate"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CreatedBy"]),
                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ModifiedDate"]),
                ModifiedBy = reader["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ModifiedBy"])
            };
        }

        #endregion
    }
}