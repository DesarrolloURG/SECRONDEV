CREATE OR ALTER PROCEDURE SP_Teachers_UpdateFilePath
    @TeacherId   INT,
    @Campo       NVARCHAR(50),
    @Ruta        NVARCHAR(500) = NULL,
    @ModifiedBy  INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    IF @Campo NOT IN (
        'FilePath_DPI', 'FilePath_Titulos', 'FilePath_RTU',
        'FilePath_Colegiado', 'FilePath_RENAS',
        'FilePath_AntPoliciacos', 'FilePath_AntPenales'
    )
    BEGIN
        SELECT -1; RETURN;
    END

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        DECLARE @sql NVARCHAR(MAX);

        SET @sql = N'UPDATE Teachers SET ' + QUOTENAME(@Campo) + N' = @Ruta, ' +
                   N'ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy ' +
                   N'WHERE TeacherId = @TeacherId;';

        EXEC sp_executesql @sql,
            N'@Ruta NVARCHAR(500), @ModifiedBy INT, @TeacherId INT',
            @Ruta = @Ruta, @ModifiedBy = @ModifiedBy, @TeacherId = @TeacherId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO