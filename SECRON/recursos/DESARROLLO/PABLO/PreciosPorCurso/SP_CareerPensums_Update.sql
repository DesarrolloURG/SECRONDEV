CREATE OR ALTER PROCEDURE SP_CareerPensums_Update
    @CareerPensumId INT,
    @CareerId INT,
    @PensumCode VARCHAR(20),
    @PensumName VARCHAR(150),
    @IsCurrent BIT,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM CareerPensums
            WHERE CareerId = @CareerId AND PensumCode = UPPER(@PensumCode) AND CareerPensumId <> @CareerPensumId
        )
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        IF @IsCurrent = 1
        BEGIN
            UPDATE CareerPensums SET IsCurrent = 0, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE CareerId = @CareerId AND IsCurrent = 1 AND CareerPensumId <> @CareerPensumId;
        END

        UPDATE CareerPensums
        SET CareerId = @CareerId,
            PensumCode = UPPER(@PensumCode),
            PensumName = UPPER(@PensumName),
            IsCurrent = @IsCurrent,
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
