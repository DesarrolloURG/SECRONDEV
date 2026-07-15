-- @IsInactivation = 1 => solo IsActive=0 (InactivarControl)
-- @IsInactivation = 0 => update normal (ActualizarControl)
CREATE OR ALTER PROCEDURE SP_CheckControl_Update
    @CheckControlId INT, @IsInactivation BIT,
    @InitialLimit INT = NULL, @FinalLimit INT = NULL,
    @CurrentCounter INT = NULL, @Priority BIT = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsInactivation = 1
            UPDATE CheckControl
            SET IsActive = 0, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE CheckControlId = @CheckControlId;
        ELSE
            UPDATE CheckControl
            SET InitialLimit = @InitialLimit, FinalLimit = @FinalLimit,
                CurrentCounter = @CurrentCounter, Priority = @Priority,
                ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE CheckControlId = @CheckControlId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO