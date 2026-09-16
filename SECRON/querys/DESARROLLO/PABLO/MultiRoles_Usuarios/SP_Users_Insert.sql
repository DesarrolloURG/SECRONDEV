CREATE OR ALTER PROCEDURE SP_Users_Insert
    @Username VARCHAR(50), @PasswordHash VARCHAR(255), @FullName VARCHAR(150),
    @StatusId INT, @NotificationsEnabled BIT, @IsTemporaryPassword BIT,
    @InstitutionalEmail VARCHAR(150) = NULL, @EmployeeId INT = NULL,
    @PasswordExpiryDate DATETIME = NULL, @CreatedBy INT = NULL,
    @RoleIds NVARCHAR(MAX)   -- JSON array de RoleId, ej: '[1,3,5]'. Obligatorio, mínimo 1 rol.
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    IF (SELECT COUNT(*) FROM OPENJSON(@RoleIds)) < 1
    BEGIN
        SELECT -2; RETURN; -- Debe traer al menos un rol
    END

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Users (Username, PasswordHash, FullName, StatusId,
            NotificationsEnabled, IsTemporaryPassword, InstitutionalEmail, EmployeeId,
            PasswordExpiryDate, CreatedBy)
        VALUES (@Username, @PasswordHash, @FullName, @StatusId,
            @NotificationsEnabled, @IsTemporaryPassword, @InstitutionalEmail, @EmployeeId,
            @PasswordExpiryDate, @CreatedBy);

        DECLARE @NewUserId INT = SCOPE_IDENTITY();

        INSERT INTO UserRoles (UserId, RoleId, IsActive, CreatedDate, CreatedBy)
        SELECT @NewUserId, CAST(value AS INT), 1, GETDATE(), @CreatedBy
        FROM OPENJSON(@RoleIds);

        COMMIT TRANSACTION; SELECT @NewUserId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END