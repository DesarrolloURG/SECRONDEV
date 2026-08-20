CREATE OR ALTER PROCEDURE SP_CourseModalities_Insert
    @ModalityCode VARCHAR(20),
    @ModalityName VARCHAR(50),
    @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM CourseModalities WHERE ModalityCode = @ModalityCode)
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        INSERT INTO CourseModalities (ModalityCode, ModalityName, CreatedBy)
        VALUES (@ModalityCode, @ModalityName, @CreatedBy);

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO