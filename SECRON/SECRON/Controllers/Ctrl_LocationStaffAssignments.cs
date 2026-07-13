using SECRON.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public static class Ctrl_LocationStaffAssignments
{
    public static int Create(Mdl_LocationStaffAssignments model)
    {
        int result = 0;

        using (SqlConnection conn = DatabaseConfig.StartConection())
        {
            using (SqlCommand cmd = new SqlCommand("SP_LocationStaffAssignments_Create", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@LocationId", model.LocationId);
                cmd.Parameters.AddWithValue("@UserId", model.UserId);
                cmd.Parameters.AddWithValue("@RoleTypeId", model.RoleTypeId);
                cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);

                SqlParameter returnParam = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                returnParam.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();

                result = (int)returnParam.Value;
            }
        }

        return result;
    }

    public static List<Mdl_LocationStaffAssignments> SearchByLocation(
       int locationId,
       string textoBusqueda,
       bool? isActive = true,
       int pageNumber = 1,
       int pageSize = 20)
    {
        List<Mdl_LocationStaffAssignments> list = new List<Mdl_LocationStaffAssignments>();

        using (SqlConnection conn = DatabaseConfig.StartConection())
        {
            int offset = (pageNumber - 1) * pageSize;

            string query = @"
                SELECT lsa.AssignmentId, lsa.LocationId, lsa.UserId, lsa.RoleTypeId,
                       lsa.IsActive, lsa.CreatedBy, lsa.CreatedDate, lsa.ModifiedBy, lsa.ModifiedDate,
                       e.EmployeeCode, e.FullName, e.InstitutionalEmail, e.Phone,
                       r.RoleName
                FROM LocationStaffAssignments lsa
                INNER JOIN Users u ON lsa.UserId = u.UserId
                INNER JOIN Employees e ON u.EmployeeId = e.EmployeeId
                INNER JOIN LocationStaffRoles r ON lsa.RoleTypeId = r.RoleTypeId
                WHERE lsa.LocationId = @LocationId
                  AND (@IsActive IS NULL OR lsa.IsActive = @IsActive)
                  AND (e.EmployeeCode LIKE @Texto OR e.FullName LIKE @Texto)
                ORDER BY e.FullName
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                cmd.Parameters.AddWithValue("@IsActive", (object)isActive ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Texto", "%" + (textoBusqueda ?? "").Trim() + "%");
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Mdl_LocationStaffAssignments
                        {
                            AssignmentId = (int)reader["AssignmentId"],
                            LocationId = (int)reader["LocationId"],
                            UserId = (int)reader["UserId"],
                            RoleTypeId = (byte)reader["RoleTypeId"],
                            IsActive = (bool)reader["IsActive"],
                            CreatedBy = (int)reader["CreatedBy"],
                            CreatedDate = (DateTime)reader["CreatedDate"],
                            ModifiedBy = reader["ModifiedBy"] as int?,
                            ModifiedDate = reader["ModifiedDate"] as DateTime?,
                            EmployeeCode = reader["EmployeeCode"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            InstitutionalEmail = reader["InstitutionalEmail"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            RoleName = reader["RoleName"].ToString()
                        });
                    }
                }
            }
        }

        return list;
    }

    public static int CountByLocation(int locationId, string textoBusqueda, bool? isActive = true)
    {
        using (SqlConnection conn = DatabaseConfig.StartConection())
        {
            string query = @"
                SELECT COUNT(1)
                FROM LocationStaffAssignments lsa
                INNER JOIN Users u ON lsa.UserId = u.UserId
                INNER JOIN Employees e ON u.EmployeeId = e.EmployeeId
                WHERE lsa.LocationId = @LocationId
                  AND (@IsActive IS NULL OR lsa.IsActive = @IsActive)
                  AND (@Texto = '' OR e.EmployeeCode LIKE @LikeTexto OR e.FullName LIKE @LikeTexto)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                cmd.Parameters.AddWithValue("@IsActive", (object)isActive ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Texto", (textoBusqueda ?? "").Trim());
                cmd.Parameters.AddWithValue("@LikeTexto", "%" + (textoBusqueda ?? "").Trim() + "%");

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }

    public static int Update(Mdl_LocationStaffAssignments model, bool isInactivation)
    {
        int result = 0;

        using (SqlConnection conn = DatabaseConfig.StartConection())
        {
            using (SqlCommand cmd = new SqlCommand("SP_LocationStaffAssignments_Update", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AssignmentId", model.AssignmentId);
                cmd.Parameters.AddWithValue("@LocationId", model.LocationId);
                cmd.Parameters.AddWithValue("@UserId", model.UserId);
                cmd.Parameters.AddWithValue("@RoleTypeId", model.RoleTypeId);
                cmd.Parameters.AddWithValue("@IsInactivation", isInactivation);
                cmd.Parameters.AddWithValue("@ModifiedBy", model.ModifiedBy);

                SqlParameter returnParam = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                returnParam.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();

                result = (int)returnParam.Value;
            }
        }

        return result;
    }


    public static List<Mdl_LocationStaffAssignments> GetAvailableUsersForLocation(
        int locationId,
        string textoBusqueda = "",
        int pageNumber = 1,
        int pageSize = 20)
    {
        List<Mdl_LocationStaffAssignments> list = new List<Mdl_LocationStaffAssignments>();

        using (SqlConnection conn = DatabaseConfig.StartConection())
        {
            int offset = (pageNumber - 1) * pageSize;

            string query = @"
                SELECT u.UserId, e.EmployeeCode, e.FullName, e.InstitutionalEmail, e.Phone
                FROM Users u
                INNER JOIN Employees e ON u.EmployeeId = e.EmployeeId
                INNER JOIN EmployeeStatus es ON e.EmployeeStatusId = es.EmployeeStatusId
                WHERE es.StatusName != 'INACTIVO'
                  AND (@Texto = '' OR e.EmployeeCode LIKE @LikeTexto OR e.FullName LIKE @LikeTexto)
                  AND NOT EXISTS (
                      SELECT 1 FROM LocationStaffAssignments lsa
                      WHERE lsa.UserId = u.UserId
                        AND lsa.LocationId = @LocationId
                        AND lsa.IsActive = 1
                  )
                ORDER BY e.FullName
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                cmd.Parameters.AddWithValue("@Texto", (textoBusqueda ?? "").Trim());
                cmd.Parameters.AddWithValue("@LikeTexto", "%" + (textoBusqueda ?? "").Trim() + "%");
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Mdl_LocationStaffAssignments
                        {
                            UserId = (int)reader["UserId"],
                            EmployeeCode = reader["EmployeeCode"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            InstitutionalEmail = reader["InstitutionalEmail"].ToString(),
                            Phone = reader["Phone"].ToString()
                        });
                    }
                }
            }
        }

        return list;
    }

    public static int CountAvailableUsersForLocation(int locationId, string textoBusqueda = "")
    {
        using (SqlConnection conn = DatabaseConfig.StartConection())
        {
            string query = @"
                SELECT COUNT(1)
                FROM Users u
                INNER JOIN Employees e ON u.EmployeeId = e.EmployeeId
                INNER JOIN EmployeeStatus es ON e.EmployeeStatusId = es.EmployeeStatusId
                WHERE es.StatusName != 'INACTIVO'
                  AND (@Texto = '' OR e.EmployeeCode LIKE @LikeTexto OR e.FullName LIKE @LikeTexto)
                  AND NOT EXISTS (
                      SELECT 1 FROM LocationStaffAssignments lsa
                      WHERE lsa.UserId = u.UserId
                        AND lsa.LocationId = @LocationId
                        AND lsa.IsActive = 1
                  )";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                cmd.Parameters.AddWithValue("@Texto", (textoBusqueda ?? "").Trim());
                cmd.Parameters.AddWithValue("@LikeTexto", "%" + (textoBusqueda ?? "").Trim() + "%");

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }

    public static List<Mdl_LocationStaffAssignments> GetByLocation(
        int locationId,
        bool? isActive = true,
        int pageNumber = 1,
        int pageSize = 20)
    {
        List<Mdl_LocationStaffAssignments> list = new List<Mdl_LocationStaffAssignments>();

        using (SqlConnection conn = DatabaseConfig.StartConection())
        {
            int offset = (pageNumber - 1) * pageSize;

            string query = @"
                SELECT lsa.AssignmentId, lsa.LocationId, lsa.UserId, lsa.RoleTypeId,
                       lsa.IsActive, lsa.CreatedBy, lsa.CreatedDate, lsa.ModifiedBy, lsa.ModifiedDate,
                       e.EmployeeCode, e.FullName, e.InstitutionalEmail, e.Phone,
                       r.RoleName
                FROM LocationStaffAssignments lsa
                INNER JOIN Users u ON lsa.UserId = u.UserId
                INNER JOIN Employees e ON u.EmployeeId = e.EmployeeId
                INNER JOIN LocationStaffRoles r ON lsa.RoleTypeId = r.RoleTypeId
                WHERE lsa.LocationId = @LocationId
                  AND (@IsActive IS NULL OR lsa.IsActive = @IsActive)
                ORDER BY e.FullName
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                cmd.Parameters.AddWithValue("@IsActive", (object)isActive ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Mdl_LocationStaffAssignments
                        {
                            AssignmentId = (int)reader["AssignmentId"],
                            LocationId = (int)reader["LocationId"],
                            UserId = (int)reader["UserId"],
                            RoleTypeId = (byte)reader["RoleTypeId"],
                            IsActive = (bool)reader["IsActive"],
                            CreatedBy = (int)reader["CreatedBy"],
                            CreatedDate = (DateTime)reader["CreatedDate"],
                            ModifiedBy = reader["ModifiedBy"] as int?,
                            ModifiedDate = reader["ModifiedDate"] as DateTime?,
                            EmployeeCode = reader["EmployeeCode"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            InstitutionalEmail = reader["InstitutionalEmail"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            RoleName = reader["RoleName"].ToString()
                        });
                    }
                }
            }
        }

        return list;
    }

    public static List<Mdl_LocationStaffAssignments> GetByUser(int userId)
    {
        List<Mdl_LocationStaffAssignments> list = new List<Mdl_LocationStaffAssignments>();

        using (SqlConnection conn = DatabaseConfig.StartConection())
        {
            string query = @"
                SELECT lsa.AssignmentId, lsa.LocationId, lsa.UserId, lsa.RoleTypeId,
                       lsa.IsActive, lsa.CreatedBy, lsa.CreatedDate, lsa.ModifiedBy, lsa.ModifiedDate,
                       e.EmployeeCode, e.FullName, e.InstitutionalEmail, e.Phone
                FROM LocationStaffAssignments lsa
                INNER JOIN Users u ON lsa.UserId = u.UserId
                INNER JOIN Employees e ON u.EmployeeId = e.EmployeeId
                WHERE lsa.UserId = @UserId
                  AND lsa.IsActive = 1";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Mdl_LocationStaffAssignments
                        {
                            AssignmentId = (int)reader["AssignmentId"],
                            LocationId = (int)reader["LocationId"],
                            UserId = (int)reader["UserId"],
                            RoleTypeId = (byte)reader["RoleTypeId"],
                            IsActive = (bool)reader["IsActive"],
                            CreatedBy = (int)reader["CreatedBy"],
                            CreatedDate = (DateTime)reader["CreatedDate"],
                            ModifiedBy = reader["ModifiedBy"] as int?,
                            ModifiedDate = reader["ModifiedDate"] as DateTime?,
                            EmployeeCode = reader["EmployeeCode"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            InstitutionalEmail = reader["InstitutionalEmail"].ToString(),
                            Phone = reader["Phone"].ToString()
                        });
                    }
                }
            }
        }

        return list;
    }

    public static List<Mdl_LocationStaffRoles> GetActiveRoles()
    {
        List<Mdl_LocationStaffRoles> list = new List<Mdl_LocationStaffRoles>();

        using (SqlConnection conn = DatabaseConfig.StartConection())
        {
            string query = @"
                SELECT RoleTypeId, RoleName, IsActive
                FROM LocationStaffRoles
                WHERE IsActive = 1
                ORDER BY RoleName";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Mdl_LocationStaffRoles
                    {
                        RoleTypeId = (byte)reader["RoleTypeId"],
                        RoleName = reader["RoleName"].ToString(),
                        IsActive = (bool)reader["IsActive"]
                    });
                }
            }
        }

        return list;
    }
}