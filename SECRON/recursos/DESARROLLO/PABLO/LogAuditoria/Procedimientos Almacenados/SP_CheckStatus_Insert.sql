CREATE OR ALTER PROCEDURE SP_CheckStatus_Insert
    @StatusName VARCHAR(100), @Description VARCHAR(255) = NULL, @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO CheckStatus (StatusName, Description, IsActive)
        VALUES (@StatusName, @Description, @IsActive);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO