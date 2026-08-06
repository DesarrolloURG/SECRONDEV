CREATE OR ALTER PROCEDURE SP_DocentesTemporal_UpdateTeacherId
    @TeacherTempId INT,
    @TeacherId     INT,
    @UsuarioId     INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE DocentesTemporal
        SET TeacherId = @TeacherId
        WHERE TeacherTempId = @TeacherTempId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO