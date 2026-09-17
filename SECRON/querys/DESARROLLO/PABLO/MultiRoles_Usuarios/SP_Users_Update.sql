CREATE OR ALTER PROCEDURE SP_Users_Update
    @UserId INT, @Username VARCHAR(50), @FullName VARCHAR(150),
    @StatusId INT, @NotificationsEnabled BIT, @InstitutionalEmail VARCHAR(150) = NULL,
    @ModifiedBy INT = NULL,
    @RoleIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    IF (SELECT COUNT(*) FROM OPENJSON(@RoleIds)) < 1
    BEGIN
        SELECT -2; RETURN;
    END

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Users SET Username = @Username, FullName = @FullName,
            StatusId = @StatusId, NotificationsEnabled = @NotificationsEnabled,
            InstitutionalEmail = @InstitutionalEmail,
            ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
        WHERE UserId = @UserId;

        MERGE UserRoles AS target
        USING (SELECT CAST(value AS INT) AS RoleId FROM OPENJSON(@RoleIds)) AS source
            ON target.UserId = @UserId AND target.RoleId = source.RoleId
        WHEN MATCHED AND target.IsActive = 0 THEN
            UPDATE SET IsActive = 1, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (UserId, RoleId, IsActive, CreatedDate, CreatedBy)
            VALUES (@UserId, source.RoleId, 1, GETDATE(), @ModifiedBy);

        UPDATE UserRoles
        SET IsActive = 0, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
        WHERE UserId = @UserId
          AND IsActive = 1
          AND RoleId NOT IN (SELECT CAST(value AS INT) FROM OPENJSON(@RoleIds));

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END