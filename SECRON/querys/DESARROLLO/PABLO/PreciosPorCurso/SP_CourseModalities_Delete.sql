CREATE OR ALTER PROCEDURE SP_CourseModalities_Delete
    @ModalityId INT,
    @IsActive BIT,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE CourseModalities
        SET IsActive = @IsActive,
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