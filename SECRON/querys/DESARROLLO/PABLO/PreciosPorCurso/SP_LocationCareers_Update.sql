CREATE OR ALTER PROCEDURE SP_LocationCareers_Update
    @LocationCareerId INT,
    @LocationId INT,
    @CareerId INT,
    @IsActive BIT,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM LocationCareers
            WHERE LocationId = @LocationId AND CareerId = @CareerId AND IsActive = 1
              AND LocationCareerId <> @LocationCareerId
        )
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        UPDATE LocationCareers
        SET LocationId = @LocationId,
            CareerId = @CareerId,
            IsActive = @IsActive,
            ModifiedDate = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE LocationCareerId = @LocationCareerId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO