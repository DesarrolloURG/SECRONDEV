using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using SECRON.Configuration;
using SECRON.Models;

namespace SECRON.Controllers
{
    internal class Ctrl_Locations
    {
        private const string SelectUbicacionesConJoins = @"
                SELECT 
                    l.LocationId,
                    l.LocationCode,
                    l.LocationName,
                    l.Address,
                    l.City,
                    l.IsActive,
                    l.CreatedDate,
                    l.CreatedBy,
                    l.ModifiedDate,
                    l.ModifiedBy,
                    l.LocationCategoryId,
                    l.PrimaryWarehouseId,
                    l.MunicipalityId,
                    c.CountryName,
                    d.DepartmentName,
                    m.MunicipalityName
                FROM Locations l
                LEFT JOIN Municipality m ON l.MunicipalityId = m.MunicipalityId
                LEFT JOIN Department d ON m.DepartmentId = d.DepartmentId
                LEFT JOIN Country c ON d.CountryId = c.CountryId";

        #region CRUD

        public static int RegistrarUbicacion(Mdl_Locations ubicacion)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        INSERT INTO Locations
                        (
                            LocationCode,
                            LocationName,
                            Address,
                            City,
                            IsActive,
                            CreatedDate,
                            CreatedBy,
                            LocationCategoryId,
                            PrimaryWarehouseId,
                            MunicipalityId
                        )
                        VALUES
                        (
                            @LocationCode,
                            @LocationName,
                            @Address,
                            @City,
                            @IsActive,
                            GETDATE(),
                            @CreatedBy,
                            @LocationCategoryId,
                            @PrimaryWarehouseId,
                            @MunicipalityId
                        )";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        AgregarParametrosUbicacion(cmd, ubicacion, incluirLocationId: false, incluirModifiedBy: false);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                MostrarErrorSqlDuplicado(
                    ex,
                    "Ya existe una sede con ese código. No se puede guardar un código duplicado.",
                    "Error al registrar ubicación: " + ex.Message
                );
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar ubicación: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int ActualizarUbicacion(Mdl_Locations ubicacion)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        UPDATE Locations
                           SET LocationCode = @LocationCode,
                               LocationName = @LocationName,
                               Address = @Address,
                               City = @City,
                               LocationCategoryId = @LocationCategoryId,
                               PrimaryWarehouseId = @PrimaryWarehouseId,
                               MunicipalityId = @MunicipalityId,
                               ModifiedDate = GETDATE(),
                               ModifiedBy = @ModifiedBy
                         WHERE LocationId = @LocationId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        AgregarParametrosUbicacion(cmd, ubicacion, incluirLocationId: true, incluirModifiedBy: true);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                MostrarErrorSqlDuplicado(
                    ex,
                    "Ya existe una sede con ese código. No se puede actualizar con un código duplicado.",
                    "Error al actualizar ubicación: " + ex.Message
                );
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar ubicación: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int InactivarUbicacion(int locationId, int? modifiedBy)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        UPDATE Locations
                           SET IsActive = 0,
                               ModifiedDate = GETDATE(),
                               ModifiedBy = @ModifiedBy
                         WHERE LocationId = @LocationId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)modifiedBy ?? DBNull.Value);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inactivar ubicación: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static Mdl_Locations ObtenerUbicacionPorId(int locationId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = SelectUbicacionesConJoins + @"
                        WHERE l.LocationId = @LocationId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapearUbicacion(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener ubicación: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        #endregion

        #region Consultas principales

        public static List<Mdl_Locations> MostrarUbicaciones(int pageNumber = 1, int pageSize = 100)
        {
            return BuscarUbicaciones(
                textoBusqueda: "",
                tipoFiltro: "TODOS",
                isActive: true,
                pageNumber: pageNumber,
                pageSize: pageSize
                );
        }

        public static List<Mdl_Locations> BuscarUbicaciones(
            string textoBusqueda = "",
            string tipoFiltro = "TODOS",
            bool? isActive = null,
            int pageNumber = 1,
            int pageSize = 100)
        {
            List<Mdl_Locations> lista = new List<Mdl_Locations>();

            try
            {
                int offset = (pageNumber - 1) * pageSize;

                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    List<SqlParameter> parametros;
                    string whereClause = ConstruirWhereBusqueda(textoBusqueda, tipoFiltro, isActive, out parametros);

                    string query = SelectUbicacionesConJoins + @"
                        " + whereClause + @"
                        ORDER BY l.LocationName
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    parametros.Add(new SqlParameter("@offset", offset));
                    parametros.Add(new SqlParameter("@pageSize", pageSize));

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                lista.Add(MapearUbicacion(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en búsqueda: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static int ContarTotalUbicaciones(
            string textoBusqueda = "",
            string tipoFiltro = "TODOS",
            bool? isActive = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    List<SqlParameter> parametros;
                    string whereClause = ConstruirWhereBusqueda(textoBusqueda, tipoFiltro, isActive, out parametros);

                    string query = @"
                        SELECT COUNT(1)
                        FROM Locations l
                        LEFT JOIN Municipality m ON l.MunicipalityId = m.MunicipalityId
                        LEFT JOIN Department d ON m.DepartmentId = d.DepartmentId
                        LEFT JOIN Country c ON d.CountryId = c.CountryId
                        " + whereClause;

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region Validaciones

        public static bool ExisteCodigoUbicacion(string locationCode, int? locationIdExcluir = null)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT COUNT(1)
                          FROM Locations
                         WHERE LocationCode = @LocationCode";

                    if (locationIdExcluir.HasValue)
                        query += " AND LocationId <> @LocationId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationCode", locationCode);

                        if (locationIdExcluir.HasValue)
                            cmd.Parameters.AddWithValue("@LocationId", locationIdExcluir.Value);

                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar código de sede: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool ValidarCodigoUbicacionUnico(string codigo, int? excludeLocationId = null)
        {
            return !ExisteCodigoUbicacion(codigo, excludeLocationId);
        }

        #endregion

        #region Combobox / catálogos

        public static List<KeyValuePair<int, string>> ObtenerTodasLasUbicaciones()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT LocationId, LocationName
                          FROM Locations
                         WHERE IsActive = 1
                         ORDER BY LocationName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new KeyValuePair<int, string>(
                                reader.GetInt32(0),
                                reader.GetString(1)
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener ubicaciones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static List<KeyValuePair<int, string>> ObtenerLocationsActivas()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT LocationId, LocationName
                          FROM Locations
                         WHERE IsActive = 1
                         ORDER BY LocationName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new KeyValuePair<int, string>(
                                reader.GetInt32(0),
                                reader.GetString(1)
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR AL OBTENER SEDES: {ex.Message}",
                    "ERROR SECRON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static List<string> ObtenerCiudades()
        {
            List<string> lista = new List<string>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT DISTINCT City
                          FROM Locations
                         WHERE IsActive = 1
                           AND City IS NOT NULL
                         ORDER BY City";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener ciudades: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static string ObtenerNombreLocation(int locationId)
        {
            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT LocationName
                          FROM Locations
                         WHERE LocationId = @LocationId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        object result = cmd.ExecuteScalar();
                        return result != null ? result.ToString() : "N/A";
                    }
                }
            }
            catch
            {
                return "N/A";
            }
        }

        public static List<Mdl_Country> ObtenerCountriesActivos()
        {
            List<Mdl_Country> lista = new List<Mdl_Country>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT CountryId, CountryName, CountryCode, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy
                          FROM Country
                         WHERE IsActive = 1
                         ORDER BY CountryName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Mdl_Country
                            {
                                CountryId = reader.GetInt32(0),
                                CountryName = reader[1] == DBNull.Value ? null : reader[1].ToString(),
                                CountryCode = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                                IsActive = reader[3] != DBNull.Value && reader.GetBoolean(3),
                                CreatedDate = reader[4] == DBNull.Value ? DateTime.Now : reader.GetDateTime(4),
                                CreatedBy = reader[5] == DBNull.Value ? null : (int?)reader.GetInt32(5),
                                ModifiedDate = reader[6] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(6),
                                ModifiedBy = reader[7] == DBNull.Value ? null : (int?)reader.GetInt32(7)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener países: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static List<Mdl_Department> ObtenerDepartmentsPorCountry(int countryId)
        {
            List<Mdl_Department> lista = new List<Mdl_Department>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT DepartmentId, CountryId, DepartmentName, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy
                          FROM Department
                         WHERE IsActive = 1
                           AND CountryId = @CountryId
                         ORDER BY DepartmentName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CountryId", countryId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Mdl_Department
                                {
                                    DepartmentId = reader.GetInt32(0),
                                    CountryId = reader.GetInt32(1),
                                    DepartmentName = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                                    IsActive = reader[3] != DBNull.Value && reader.GetBoolean(3),
                                    CreatedDate = reader[4] == DBNull.Value ? DateTime.Now : reader.GetDateTime(4),
                                    CreatedBy = reader[5] == DBNull.Value ? null : (int?)reader.GetInt32(5),
                                    ModifiedDate = reader[6] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(6),
                                    ModifiedBy = reader[7] == DBNull.Value ? null : (int?)reader.GetInt32(7)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener departamentos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static List<Mdl_Municipality> ObtenerMunicipalitiesPorDepartment(int departmentId)
        {
            List<Mdl_Municipality> lista = new List<Mdl_Municipality>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT MunicipalityId, DepartmentId, MunicipalityName, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy
                          FROM Municipality
                         WHERE IsActive = 1
                           AND DepartmentId = @DepartmentId
                         ORDER BY MunicipalityName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Mdl_Municipality
                                {
                                    MunicipalityId = reader.GetInt32(0),
                                    DepartmentId = reader.GetInt32(1),
                                    MunicipalityName = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                                    IsActive = reader[3] != DBNull.Value && reader.GetBoolean(3),
                                    CreatedDate = reader[4] == DBNull.Value ? DateTime.Now : reader.GetDateTime(4),
                                    CreatedBy = reader[5] == DBNull.Value ? null : (int?)reader.GetInt32(5),
                                    ModifiedDate = reader[6] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(6),
                                    ModifiedBy = reader[7] == DBNull.Value ? null : (int?)reader.GetInt32(7)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener municipios: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        public static List<Mdl_LocationCategory> ObtenerLocationCategoriesActivas()
        {
            List<Mdl_LocationCategory> lista = new List<Mdl_LocationCategory>();

            try
            {
                using (SqlConnection connection = DatabaseConfig.StartConection())
                {
                    string query = @"
                        SELECT LocationCategoryId, CategoryCode, CategoryName, Description, IsActive, CreatedDate, CreatedBy
                          FROM LocationCategories
                         WHERE IsActive = 1
                         ORDER BY CategoryCode";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Mdl_LocationCategory
                            {
                                LocationCategoryId = reader.GetInt32(0),
                                CategoryCode = reader[1] == DBNull.Value ? null : reader[1].ToString(),
                                CategoryName = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                                Description = reader[3] == DBNull.Value ? null : reader[3].ToString(),
                                IsActive = reader[4] != DBNull.Value && reader.GetBoolean(4),
                                CreatedDate = reader[5] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(5),
                                CreatedBy = reader[6] == DBNull.Value ? null : (int?)reader.GetInt32(6)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener categorías de sede: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        #endregion

        #region Métodos auxiliares

        private static void AgregarParametrosUbicacion(
            SqlCommand cmd,
            Mdl_Locations ubicacion,
            bool incluirLocationId,
            bool incluirModifiedBy)
        {
            if (incluirLocationId)
                cmd.Parameters.AddWithValue("@LocationId", ubicacion.LocationId);

            cmd.Parameters.AddWithValue("@LocationCode", (object)ubicacion.LocationCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LocationName", (object)ubicacion.LocationName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object)ubicacion.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@City", (object)ubicacion.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LocationCategoryId", (object)ubicacion.LocationCategoryId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PrimaryWarehouseId", (object)ubicacion.PrimaryWarehouseId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MunicipalityId", (object)ubicacion.MunicipalityId ?? DBNull.Value);

            if (!incluirLocationId)
                cmd.Parameters.AddWithValue("@IsActive", ubicacion.IsActive);

            if (!incluirModifiedBy)
                cmd.Parameters.AddWithValue("@CreatedBy", (object)ubicacion.CreatedBy ?? DBNull.Value);

            if (incluirModifiedBy)
                cmd.Parameters.AddWithValue("@ModifiedBy", (object)ubicacion.ModifiedBy ?? DBNull.Value);
        }

        private static string ConstruirWhereBusqueda(
            string textoBusqueda,
            string tipoFiltro,
            bool? isActive,
            out List<SqlParameter> parametros)
        {
            parametros = new List<SqlParameter>();
            string whereClause = "WHERE 1 = 1";

            if (isActive.HasValue)
            {
                whereClause += " AND l.IsActive = @IsActive";
                parametros.Add(new SqlParameter("@IsActive", isActive.Value));
            }
            else
            {
                whereClause += " AND l.IsActive = 1";
            }

            if (!string.IsNullOrWhiteSpace(textoBusqueda))
            {
                switch (tipoFiltro)
                {
                    case "POR SEDE":
                        whereClause += " AND l.LocationName LIKE @texto";
                        break;

                    case "POR DEPARTAMENTO":
                        whereClause += " AND d.DepartmentName LIKE @texto";
                        break;

                    case "POR MUNICIPIO":
                        whereClause += " AND m.MunicipalityName LIKE @texto";
                        break;

                    default:
                        whereClause += @"
                            AND (
                                l.LocationName LIKE @texto OR
                                d.DepartmentName LIKE @texto OR
                                m.MunicipalityName LIKE @texto
                            )";
                        break;
                }

                parametros.Add(new SqlParameter("@texto", "%" + textoBusqueda.Trim() + "%"));
            }

            return whereClause;
        }

        private static void MostrarErrorSqlDuplicado(SqlException ex, string mensajeDuplicado, string mensajeGeneral)
        {
            if (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show(mensajeDuplicado,
                    "Código duplicado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show(mensajeGeneral,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static Mdl_Locations MapearUbicacion(SqlDataReader reader)
        {
            return new Mdl_Locations
            {
                LocationId = reader.GetInt32(0),
                LocationCode = reader[1] == DBNull.Value ? null : reader[1].ToString(),
                LocationName = reader[2] == DBNull.Value ? null : reader[2].ToString(),
                Address = reader[3] == DBNull.Value ? null : reader[3].ToString(),
                City = reader[4] == DBNull.Value ? null : reader[4].ToString(),
                IsActive = reader[5] == DBNull.Value ? true : reader.GetBoolean(5),
                CreatedDate = reader[6] == DBNull.Value ? DateTime.Now : reader.GetDateTime(6),
                CreatedBy = reader[7] == DBNull.Value ? null : (int?)reader.GetInt32(7),
                ModifiedDate = reader[8] == DBNull.Value ? null : (DateTime?)reader.GetDateTime(8),
                ModifiedBy = reader[9] == DBNull.Value ? null : (int?)reader.GetInt32(9),
                LocationCategoryId = reader[10] == DBNull.Value ? null : (int?)reader.GetInt32(10),
                PrimaryWarehouseId = reader[11] == DBNull.Value ? null : (int?)reader.GetInt32(11),
                MunicipalityId = reader[12] == DBNull.Value ? null : (int?)reader.GetInt32(12),
                CountryName = reader[13] == DBNull.Value ? null : reader[13].ToString(),
                DepartmentName = reader[14] == DBNull.Value ? null : reader[14].ToString(),
                MunicipalityName = reader[15] == DBNull.Value ? null : reader[15].ToString()
            };
        }

        #endregion
    }
}