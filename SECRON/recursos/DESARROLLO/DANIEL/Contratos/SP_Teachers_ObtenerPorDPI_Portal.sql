CREATE OR ALTER PROCEDURE SP_Teachers_ObtenerPorDPI_Portal
    @DPI VARCHAR(20) = NULL,
    @CollegiateNumber NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 TeacherId, FullName, DPI, CollegiateNumber,
           FilePath_DPI, FilePath_Colegiado, FilePath_RTU, FilePath_CV, FilePath_ContratoFirmado
    FROM Teachers
    WHERE (@DPI IS NOT NULL AND DPI = @DPI)
       OR (@CollegiateNumber IS NOT NULL AND CollegiateNumber = @CollegiateNumber);
END
GO