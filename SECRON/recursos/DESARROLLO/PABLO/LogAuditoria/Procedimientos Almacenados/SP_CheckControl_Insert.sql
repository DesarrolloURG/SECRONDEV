CREATE OR ALTER PROCEDURE SP_CheckControl_Insert
    @UserId INT, @InitialLimit INT, @FinalLimit INT, @CurrentCounter INT,
    @Priority BIT, @IsActive BIT, @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO CheckControl (UserId, InitialLimit, FinalLimit, CurrentCounter, Priority, IsActive, CreatedBy)
        VALUES (@UserId, @InitialLimit, @FinalLimit, @CurrentCounter, @Priority, @IsActive, @CreatedBy);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO