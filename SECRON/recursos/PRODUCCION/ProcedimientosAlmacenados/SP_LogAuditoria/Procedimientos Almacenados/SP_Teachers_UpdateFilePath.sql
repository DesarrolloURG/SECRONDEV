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
        'FilePath_AntPoliciacos', 'FilePath_AntPenales',
        'FilePath_CV', 'FilePath_ContratoFirmado'
    )
    BEGIN
        SELECT -1; RETURN;
    END

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @Campo = 'FilePath_DPI'
            UPDATE Teachers SET FilePath_DPI = @Ruta, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE TeacherId = @TeacherId;
        ELSE IF @Campo = 'FilePath_Titulos'
            UPDATE Teachers SET FilePath_Titulos = @Ruta, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE TeacherId = @TeacherId;
        ELSE IF @Campo = 'FilePath_RTU'
            UPDATE Teachers SET FilePath_RTU = @Ruta, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE TeacherId = @TeacherId;
        ELSE IF @Campo = 'FilePath_Colegiado'
            UPDATE Teachers SET FilePath_Colegiado = @Ruta, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE TeacherId = @TeacherId;
        ELSE IF @Campo = 'FilePath_RENAS'
            UPDATE Teachers SET FilePath_RENAS = @Ruta, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE TeacherId = @TeacherId;
        ELSE IF @Campo = 'FilePath_AntPoliciacos'
            UPDATE Teachers SET FilePath_AntPoliciacos = @Ruta, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE TeacherId = @TeacherId;
        ELSE IF @Campo = 'FilePath_AntPenales'
            UPDATE Teachers SET FilePath_AntPenales = @Ruta, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE TeacherId = @TeacherId;
        ELSE IF @Campo = 'FilePath_CV'
            UPDATE Teachers SET FilePath_CV = @Ruta, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE TeacherId = @TeacherId;
        ELSE IF @Campo = 'FilePath_ContratoFirmado'
            UPDATE Teachers SET FilePath_ContratoFirmado = @Ruta, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE TeacherId = @TeacherId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO