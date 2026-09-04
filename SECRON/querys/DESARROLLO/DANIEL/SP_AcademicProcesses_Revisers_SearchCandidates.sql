-- =====================================================================
-- SP_AcademicProcesses_Revisers_SearchCandidates
-- Busca personas (Docentes/Trabajadores/Proveedores/Coordinadores) para
-- asignarlas o removerlas como revisores de horarios (FR-ACA-05).
-- Indica si la persona ya es revisor activo (EsRevisorActivo) y su UserId
-- (puede venir NULL si la persona aún no tiene Usuario vinculado).
--
-- @PersonType: DOCENTE | TRABAJADOR | PROVEEDOR | COORDINADOR
-- @EstadoFiltro: TODOS | ACTIVOS | INACTIVOS (estado de la ficha origen)
-- =====================================================================
CREATE OR ALTER PROCEDURE SP_AcademicProcesses_Revisers_SearchCandidates
    @PersonType VARCHAR(20),
    @TextoBusqueda VARCHAR(150) = NULL,
    @EstadoFiltro VARCHAR(10) = 'TODOS',
    @PageNumber INT = 1,
    @PageSize INT = 100
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    DECLARE @Texto VARCHAR(150) = '%' + ISNULL(@TextoBusqueda, '') + '%';

    IF @PersonType = 'DOCENTE'
    BEGIN
        SELECT
            t.TeacherId AS PersonId,
            t.TeacherCode AS PersonCode,
            t.FullName AS PersonName,
            t.IsActive AS PersonIsActive,
            t.UserId AS UserId,
            u.Username AS Username,
            u.FullName AS UserFullName,
            rev.ReviserId AS ReviserId,
            CASE WHEN rev.ReviserId IS NOT NULL THEN 1 ELSE 0 END AS EsRevisorActivo,
            COUNT(*) OVER() AS TotalRegistros
        FROM Teachers t
        LEFT JOIN Users u ON u.UserId = t.UserId
        LEFT JOIN AcademicProcesses_Revisers rev
            ON rev.PersonType = 'DOCENTE' AND rev.PersonId = t.TeacherId AND rev.IsActive = 1
        WHERE (@TextoBusqueda IS NULL OR t.FullName LIKE @Texto OR t.TeacherCode LIKE @Texto)
          AND (@EstadoFiltro = 'TODOS'
               OR (@EstadoFiltro = 'ACTIVOS' AND t.IsActive = 1)
               OR (@EstadoFiltro = 'INACTIVOS' AND t.IsActive = 0))
        ORDER BY t.FullName
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE IF @PersonType = 'TRABAJADOR'
    BEGIN
        SELECT
            e.EmployeeId AS PersonId,
            e.EmployeeCode AS PersonCode,
            e.FullName AS PersonName,
            e.IsActive AS PersonIsActive,
            e.UserId AS UserId,
            u.Username AS Username,
            u.FullName AS UserFullName,
            rev.ReviserId AS ReviserId,
            CASE WHEN rev.ReviserId IS NOT NULL THEN 1 ELSE 0 END AS EsRevisorActivo,
            COUNT(*) OVER() AS TotalRegistros
        FROM Employees e
        LEFT JOIN Users u ON u.UserId = e.UserId
        LEFT JOIN AcademicProcesses_Revisers rev
            ON rev.PersonType = 'TRABAJADOR' AND rev.PersonId = e.EmployeeId AND rev.IsActive = 1
        WHERE (@TextoBusqueda IS NULL OR e.FullName LIKE @Texto OR e.EmployeeCode LIKE @Texto)
          AND (@EstadoFiltro = 'TODOS'
               OR (@EstadoFiltro = 'ACTIVOS' AND e.IsActive = 1)
               OR (@EstadoFiltro = 'INACTIVOS' AND e.IsActive = 0))
        ORDER BY e.FullName
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE IF @PersonType = 'PROVEEDOR'
    BEGIN
        SELECT
            s.SupplierId AS PersonId,
            s.SupplierCode AS PersonCode,
            s.SupplierName AS PersonName,
            s.IsActive AS PersonIsActive,
            s.UserId AS UserId,
            u.Username AS Username,
            u.FullName AS UserFullName,
            rev.ReviserId AS ReviserId,
            CASE WHEN rev.ReviserId IS NOT NULL THEN 1 ELSE 0 END AS EsRevisorActivo,
            COUNT(*) OVER() AS TotalRegistros
        FROM Suppliers s
        LEFT JOIN Users u ON u.UserId = s.UserId
        LEFT JOIN AcademicProcesses_Revisers rev
            ON rev.PersonType = 'PROVEEDOR' AND rev.PersonId = s.SupplierId AND rev.IsActive = 1
        WHERE (@TextoBusqueda IS NULL OR s.SupplierName LIKE @Texto OR s.SupplierCode LIKE @Texto)
          AND (@EstadoFiltro = 'TODOS'
               OR (@EstadoFiltro = 'ACTIVOS' AND s.IsActive = 1)
               OR (@EstadoFiltro = 'INACTIVOS' AND s.IsActive = 0))
        ORDER BY s.SupplierName
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE IF @PersonType = 'COORDINADOR'
    BEGIN
        SELECT
            c.CoordinatorId AS PersonId,
            c.CoordinatorCode AS PersonCode,
            c.FullName AS PersonName,
            c.IsActive AS PersonIsActive,
            c.UserId AS UserId,
            u.Username AS Username,
            u.FullName AS UserFullName,
            rev.ReviserId AS ReviserId,
            CASE WHEN rev.ReviserId IS NOT NULL THEN 1 ELSE 0 END AS EsRevisorActivo,
            COUNT(*) OVER() AS TotalRegistros
        FROM Coordinators c
        LEFT JOIN Users u ON u.UserId = c.UserId
        LEFT JOIN AcademicProcesses_Revisers rev
            ON rev.PersonType = 'COORDINADOR' AND rev.PersonId = c.CoordinatorId AND rev.IsActive = 1
        WHERE (@TextoBusqueda IS NULL OR c.FullName LIKE @Texto OR c.CoordinatorCode LIKE @Texto)
          AND (@EstadoFiltro = 'TODOS'
               OR (@EstadoFiltro = 'ACTIVOS' AND c.IsActive = 1)
               OR (@EstadoFiltro = 'INACTIVOS' AND c.IsActive = 0))
        ORDER BY c.FullName
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
END
GO