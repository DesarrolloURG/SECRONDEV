-- =========================================================
-- SP: Modificar / Inactivar asignación de personal a sede
-- =========================================================
CREATE OR ALTER PROCEDURE SP_LocationStaffAssignments_Update
    @AssignmentId       INT,
    @LocationId         INT,
    @UserId             INT,
    @RoleTypeId         TINYINT,
    @IsInactivation     BIT,
    @ModifiedBy         INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRY
        IF @IsInactivation = 0
        BEGIN
            IF EXISTS (
                SELECT 1 FROM LocationStaffAssignments
                WHERE LocationId = @LocationId
                  AND UserId = @UserId
                  AND IsActive = 1
                  AND AssignmentId <> @AssignmentId
            )
            BEGIN
                RETURN -1;
            END
        END

        BEGIN TRANSACTION;

        IF @IsInactivation = 1
        BEGIN
            UPDATE LocationStaffAssignments
            SET IsActive = 0,
                ModifiedBy = @ModifiedBy,
                ModifiedDate = GETDATE()
            WHERE AssignmentId = @AssignmentId;
        END
        ELSE
        BEGIN
            UPDATE LocationStaffAssignments
            SET LocationId = @LocationId,
                UserId = @UserId,
                RoleTypeId = @RoleTypeId,
                ModifiedBy = @ModifiedBy,
                ModifiedDate = GETDATE()
            WHERE AssignmentId = @AssignmentId;
        END

        COMMIT TRANSACTION;
        RETURN 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        RETURN 0;
    END CATCH
END
GO