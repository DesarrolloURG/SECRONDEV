-- =====================================================================
-- SP_AcademicProcesses_ReviserLocations_Insert
-- =====================================================================
CREATE OR ALTER PROCEDURE SP_AcademicProcesses_ReviserLocations_Insert
    @ReviserId INT, @LocationId INT, @AssignedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@AssignedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO AcademicProcesses_ReviserLocations (ReviserId, LocationId, IsActive, AssignedDate, AssignedBy)
        VALUES (@ReviserId, @LocationId, 1, GETDATE(), @AssignedBy);

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================================
-- SP_AcademicProcesses_ReviserLocations_Update
-- @Mode: 1 = remover (inactivar), 2 = reactivar
-- =====================================================================
CREATE OR ALTER PROCEDURE SP_AcademicProcesses_ReviserLocations_Update
    @ReviserLocationId INT, @Mode TINYINT, @ActionByUserId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ActionByUserId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @Mode = 1
            UPDATE AcademicProcesses_ReviserLocations
            SET IsActive = 0, RemovedDate = GETDATE(), RemovedBy = @ActionByUserId
            WHERE ReviserLocationId = @ReviserLocationId;
        ELSE IF @Mode = 2
            UPDATE AcademicProcesses_ReviserLocations
            SET IsActive = 1, AssignedDate = GETDATE(), AssignedBy = @ActionByUserId,
                RemovedDate = NULL, RemovedBy = NULL
            WHERE ReviserLocationId = @ReviserLocationId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================================
-- SP_AcademicProcesses_ReviserLocations_GetByReviser
-- Sedes YA asignadas (activas) a un revisor
-- =====================================================================
CREATE OR ALTER PROCEDURE SP_AcademicProcesses_ReviserLocations_GetByReviser
    @ReviserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        rl.ReviserLocationId, rl.ReviserId,
        l.LocationId, l.LocationCode, l.LocationName,
        rl.IsActive, rl.AssignedDate, rl.AssignedBy, ua.FullName AS AssignedByName,
        rl.RemovedDate, rl.RemovedBy, ur.FullName AS RemovedByName
    FROM AcademicProcesses_ReviserLocations rl
    JOIN Locations l ON l.LocationId = rl.LocationId
    LEFT JOIN Users ua ON ua.UserId = rl.AssignedBy
    LEFT JOIN Users ur ON ur.UserId = rl.RemovedBy
    WHERE rl.ReviserId = @ReviserId AND rl.IsActive = 1
    ORDER BY l.LocationName;
END
GO

-- =====================================================================
-- SP_AcademicProcesses_ReviserLocations_SearchAvailable
-- Catálogo de sedes que el revisor AÚN NO tiene asignadas (activas)
-- =====================================================================
CREATE OR ALTER PROCEDURE SP_AcademicProcesses_ReviserLocations_SearchAvailable
    @ReviserId INT, @TextoBusqueda VARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Texto VARCHAR(150) = '%' + ISNULL(@TextoBusqueda, '') + '%';

    SELECT l.LocationId, l.LocationCode, l.LocationName
    FROM Locations l
    WHERE l.IsActive = 1
      AND (@TextoBusqueda IS NULL OR l.LocationName LIKE @Texto OR l.LocationCode LIKE @Texto)
      AND NOT EXISTS (
          SELECT 1 FROM AcademicProcesses_ReviserLocations rl
          WHERE rl.ReviserId = @ReviserId AND rl.LocationId = l.LocationId AND rl.IsActive = 1
      )
    ORDER BY l.LocationName;
END
GO