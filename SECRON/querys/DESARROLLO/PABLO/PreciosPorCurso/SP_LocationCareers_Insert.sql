CREATE OR ALTER PROCEDURE SP_LocationCareers_Insert
    @LocationId INT,
    @CareerId INT,
    @ModalityId INT,
    @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM LocationCareers
            WHERE LocationId = @LocationId AND CareerId = @CareerId AND ModalityId = @ModalityId AND IsActive = 1
        )
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        INSERT INTO LocationCareers (LocationId, CareerId, ModalityId, CreatedBy)
        VALUES (@LocationId, @CareerId, @ModalityId, @CreatedBy);

        DECLARE @NewId INT = SCOPE_IDENTITY();
        COMMIT TRANSACTION; SELECT @NewId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO