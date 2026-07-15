CREATE OR ALTER PROCEDURE SP_UserStatus_Insert
    @StatusName VARCHAR(50), @Description VARCHAR(255) = NULL, @IsActive BIT, @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO UserStatus (StatusName, Description, IsActive)
        VALUES (@StatusName, @Description, @IsActive);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO