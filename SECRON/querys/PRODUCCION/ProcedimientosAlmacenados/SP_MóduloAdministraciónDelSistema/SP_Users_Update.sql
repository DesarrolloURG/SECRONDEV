CREATE OR ALTER PROCEDURE SP_Users_Update
    @UserId INT, @Username VARCHAR(50), @FullName VARCHAR(150), @RoleId INT,
    @StatusId INT, @NotificationsEnabled BIT, @InstitutionalEmail VARCHAR(150) = NULL,
    @EmployeeId INT = NULL, @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Users SET Username = @Username, FullName = @FullName, RoleId = @RoleId,
            StatusId = @StatusId, NotificationsEnabled = @NotificationsEnabled,
            InstitutionalEmail = @InstitutionalEmail, EmployeeId = @EmployeeId,
            ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
        WHERE UserId = @UserId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO