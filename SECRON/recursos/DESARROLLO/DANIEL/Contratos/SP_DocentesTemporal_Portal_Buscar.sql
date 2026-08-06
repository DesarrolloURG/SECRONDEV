GO

SELECT DB_NAME() AS BaseActual;
GO

CREATE OR ALTER PROCEDURE SP_DocentesTemporal_Portal_Buscar
    @DPI VARCHAR(13) = NULL,
    @CollegiateNumber VARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 TeacherTempId, ContractCode, DPI, FirstName, LastName, CollegiateNumber
    FROM DocentesTemporal
    WHERE (@DPI IS NOT NULL AND DPI = @DPI)
       OR (@CollegiateNumber IS NOT NULL AND CollegiateNumber = @CollegiateNumber);
END
GO

CREATE OR ALTER PROCEDURE SP_Teachers_ObtenerPorTeacherId_Portal
    @TeacherId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TeacherId, FullName, FilePath_DPI, FilePath_Colegiado, FilePath_RTU, FilePath_CV, FilePath_ContratoFirmado
    FROM Teachers
    WHERE TeacherId = @TeacherId;
END
GO

GRANT EXECUTE ON dbo.SP_Teachers_ObtenerPorTeacherId_Portal TO admin_api;
GO

GRANT EXECUTE ON dbo.SP_DocentesTemporal_Portal_Buscar TO db_api_contratos;
GO
GRANT EXECUTE ON dbo.SP_Teachers_ObtenerPorDPI_Portal TO db_api_contratos;
GO
GRANT EXECUTE ON dbo.SP_DocentesTemporal_Cursos_SelectByTeacherTempId TO db_api_contratos;
GO

SELECT name, create_date FROM sys.procedures 
WHERE name IN ('SP_DocentesTemporal_Portal_Buscar', 'SP_Teachers_ObtenerPorDPI_Portal');
GO