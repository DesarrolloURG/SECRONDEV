CREATE OR ALTER PROCEDURE SP_CareerPensums_Delete
    @CareerPensumId INT,
    @IsActive BIT,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE CareerPensums
        SET IsActive = @IsActive,
            IsCurrent = CASE WHEN @IsActive = 0 THEN 0 ELSE IsCurrent END,
            ModifiedDate = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE CareerPensumId = @CareerPensumId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO