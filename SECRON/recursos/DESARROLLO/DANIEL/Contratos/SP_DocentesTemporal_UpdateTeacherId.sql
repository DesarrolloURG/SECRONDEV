CREATE OR ALTER PROCEDURE SP_Locations_ObtenerIdPorNombreExacto
    @NombreSede VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT LocationId FROM Locations WHERE LocationName = @NombreSede AND IsActive = 1;
END
GO

GRANT EXECUTE ON dbo.SP_Locations_ObtenerIdPorNombreExacto TO db_api_contratos;
GO