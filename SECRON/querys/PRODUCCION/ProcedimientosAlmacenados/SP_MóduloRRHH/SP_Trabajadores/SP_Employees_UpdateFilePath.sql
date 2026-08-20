-- 2) SP_Employees_UpdateFilePath: agregar los 2 campos nuevos a la whitelist
CREATE OR ALTER PROCEDURE SP_Employees_UpdateFilePath
    @EmployeeId INT, @Campo SYSNAME, @Ruta VARCHAR(500) = NULL, @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    IF @Campo NOT IN ('FilePath_DPI', 'FilePath_Titulos', 'FilePath_RTU', 'FilePath_Colegiado',
                       'FilePath_RENAS', 'FilePath_AntPoliciacos', 'FilePath_AntPenales',
                       'FilePath_CV', 'FilePath_ContratoFirmado')
    BEGIN SELECT 0; RETURN; END

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    DECLARE @sql NVARCHAR(MAX) =
        N'UPDATE Employees SET ' + QUOTENAME(@Campo) + N' = @Ruta,
          ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
          WHERE EmployeeId = @EmployeeId;
          SELECT @@ROWCOUNT;';

    BEGIN TRANSACTION
    BEGIN TRY
        DECLARE @rows INT;
        DECLARE @tbl TABLE (r INT);
        INSERT INTO @tbl
        EXEC sp_executesql @sql,
             N'@Ruta VARCHAR(500), @ModifiedBy INT, @EmployeeId INT',
             @Ruta, @ModifiedBy, @EmployeeId;
        SELECT @rows = r FROM @tbl;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO
