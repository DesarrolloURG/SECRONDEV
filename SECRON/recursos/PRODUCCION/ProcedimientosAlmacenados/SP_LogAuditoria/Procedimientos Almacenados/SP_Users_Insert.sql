CREATE OR ALTER PROCEDURE SP_Users_Insert
    @Username VARCHAR(50), @PasswordHash VARCHAR(255), @FullName VARCHAR(150),
    @RoleId INT, @StatusId INT, @NotificationsEnabled BIT, @IsTemporaryPassword BIT,
    @InstitutionalEmail VARCHAR(150) = NULL, @EmployeeId INT = NULL,
    @PasswordExpiryDate DATETIME = NULL, @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Users (Username, PasswordHash, FullName, RoleId, StatusId,
            NotificationsEnabled, IsTemporaryPassword, InstitutionalEmail, EmployeeId,
            PasswordExpiryDate, CreatedBy)
        VALUES (@Username, @PasswordHash, @FullName, @RoleId, @StatusId,
            @NotificationsEnabled, @IsTemporaryPassword, @InstitutionalEmail, @EmployeeId,
            @PasswordExpiryDate, @CreatedBy);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO