-- =========================================================
-- SP: Crear asignación de personal a sede
-- (reactiva si existe registro inactivo para la misma sede/usuario)
-- =========================================================
CREATE OR ALTER PROCEDURE SP_LocationStaffAssignments_Create
    @LocationId     INT,
    @UserId         INT,
    @RoleTypeId     TINYINT,
    @CreatedBy      INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM LocationStaffAssignments
            WHERE LocationId = @LocationId
              AND UserId = @UserId
              AND IsActive = 1
        )
        BEGIN
            RETURN -1;
        END

        DECLARE @ExistingAssignmentId INT;

        SELECT @ExistingAssignmentId = AssignmentId
        FROM LocationStaffAssignments
        WHERE LocationId = @LocationId
          AND UserId = @UserId
          AND IsActive = 0;

        BEGIN TRANSACTION;

        IF @ExistingAssignmentId IS NOT NULL
        BEGIN
            UPDATE LocationStaffAssignments
            SET RoleTypeId = @RoleTypeId,
                IsActive = 1,
                ModifiedBy = @CreatedBy,
                ModifiedDate = GETDATE()
            WHERE AssignmentId = @ExistingAssignmentId;
        END
        ELSE
        BEGIN
            INSERT INTO LocationStaffAssignments
                (LocationId, UserId, RoleTypeId, IsActive, CreatedBy, CreatedDate)
            VALUES
                (@LocationId, @UserId, @RoleTypeId, 1, @CreatedBy, GETDATE());
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