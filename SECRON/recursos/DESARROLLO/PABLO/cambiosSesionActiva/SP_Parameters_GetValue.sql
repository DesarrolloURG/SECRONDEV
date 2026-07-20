CREATE OR ALTER PROCEDURE SP_Parameters_GetValue
    @ParameterName VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ParameterValue FROM ParametersConfiguration WHERE ParameterName = @ParameterName;
END
GO