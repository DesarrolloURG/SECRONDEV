CREATE OR ALTER PROCEDURE SP_LocationStaffAssignments_Delete
    @AssignmentId INT,
    @IsActive     BIT,
    @ModifiedBy   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM LocationStaffAssignments WHERE AssignmentId = @AssignmentId)
        BEGIN
            RETURN -2; -- No existe
        END

        DECLARE @LocationId INT, @UserId INT;
        SELECT @LocationId = LocationId, @UserId = UserId
        FROM LocationStaffAssignments
        WHERE AssignmentId = @AssignmentId;

        -- Al reactivar, se preserva la misma regla que el Update: no permitir que el usuario
        -- quede con dos asignaciones activas en la misma sede.
        IF @IsActive = 1
        BEGIN
            IF EXISTS (
                SELECT 1 FROM LocationStaffAssignments
                WHERE LocationId = @LocationId
                  AND UserId = @UserId
                  AND IsActive = 1
                  AND AssignmentId <> @AssignmentId
            )
            BEGIN
                RETURN -1; -- Ya tiene otra asignación activa en esa sede
            END
        END

        BEGIN TRANSACTION;

        UPDATE LocationStaffAssignments
        SET IsActive = @IsActive,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = GETDATE()
        WHERE AssignmentId = @AssignmentId;

        COMMIT TRANSACTION;
        RETURN 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        RETURN 0;
    END CATCH
END