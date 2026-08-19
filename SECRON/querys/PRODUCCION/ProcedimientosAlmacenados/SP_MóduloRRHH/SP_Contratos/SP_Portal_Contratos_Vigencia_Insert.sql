CREATE OR ALTER PROCEDURE SP_Portal_Contratos_Vigencia_Insert
    @FechaInicio DATETIME, @FechaFin DATETIME, @Activo BIT,
    @Observaciones VARCHAR(200) = NULL, @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        -- Solo puede haber UNA vigencia activa a la vez
        IF @Activo = 1
            UPDATE Portal_Contratos_Vigencia SET Activo = 0 WHERE Activo = 1;

        INSERT INTO Portal_Contratos_Vigencia (FechaInicio, FechaFin, Activo, Observaciones, CreatedBy, CreatedDate)
        VALUES (@FechaInicio, @FechaFin, @Activo, @Observaciones, @CreatedBy, GETDATE());
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- @Mode: 0 = actualizar normal, 1 = inactivar, 2 = reactivar (desactiva las demás)
CREATE OR ALTER PROCEDURE SP_Portal_Contratos_Vigencia_Update
    @VigenciaId INT, @Mode TINYINT,
    @FechaInicio DATETIME = NULL, @FechaFin DATETIME = NULL,
    @Observaciones VARCHAR(200) = NULL, @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @Mode = 1
            UPDATE Portal_Contratos_Vigencia SET Activo = 0, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE VigenciaId = @VigenciaId;
        ELSE IF @Mode = 2
        BEGIN
            UPDATE Portal_Contratos_Vigencia SET Activo = 0 WHERE Activo = 1 AND VigenciaId <> @VigenciaId;
            UPDATE Portal_Contratos_Vigencia SET Activo = 1, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE VigenciaId = @VigenciaId;
        END
        ELSE
            UPDATE Portal_Contratos_Vigencia SET FechaInicio = @FechaInicio, FechaFin = @FechaFin,
                Observaciones = @Observaciones, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE VigenciaId = @VigenciaId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE SP_Portal_Contratos_Vigencia_Select
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Portal_Contratos_Vigencia ORDER BY FechaInicio DESC;
END
GO

CREATE OR ALTER PROCEDURE SP_Portal_Contratos_Vigencia_ObtenerVigente
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM Portal_Contratos_Vigencia
    WHERE Activo = 1 AND GETDATE() BETWEEN FechaInicio AND FechaFin;
END
GO