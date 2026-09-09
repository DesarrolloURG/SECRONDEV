CREATE OR ALTER PROCEDURE SP_CourseModalities_Update
    @ModalityId INT,
    @ModalityCode VARCHAR(20),
    @ModalityName VARCHAR(50),
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM CourseModalities WHERE ModalityCode = @ModalityCode AND ModalityId <> @ModalityId)
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        UPDATE CourseModalities
        SET ModalityCode = @ModalityCode,
            ModalityName = @ModalityName,
            ModifiedDate = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE ModalityId = @ModalityId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO