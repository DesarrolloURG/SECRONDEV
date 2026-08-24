CREATE OR ALTER PROCEDURE SP_CareerPensums_Insert
    @CareerId INT,
    @PensumCode VARCHAR(20),
    @PensumName VARCHAR(150),
    @IsCurrent BIT = 0,
    @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM CareerPensums WHERE CareerId = @CareerId AND PensumCode = UPPER(@PensumCode))
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        IF @IsCurrent = 1
        BEGIN
            UPDATE CareerPensums SET IsCurrent = 0, ModifiedDate = GETDATE(), ModifiedBy = @CreatedBy
            WHERE CareerId = @CareerId AND IsCurrent = 1;
        END

        INSERT INTO CareerPensums (CareerId, PensumCode, PensumName, IsCurrent, CreatedBy)
        VALUES (@CareerId, UPPER(@PensumCode), UPPER(@PensumName), @IsCurrent, @CreatedBy);

        DECLARE @NewId INT = SCOPE_IDENTITY();
        COMMIT TRANSACTION; SELECT @NewId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO