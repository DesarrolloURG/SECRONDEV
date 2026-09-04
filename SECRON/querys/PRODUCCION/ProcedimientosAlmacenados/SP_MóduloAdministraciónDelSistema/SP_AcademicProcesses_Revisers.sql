-- =====================================================
-- SP: SP_AcademicProcesses_Revisers_Insert
-- Asigna el rol de revisor por primera vez a un usuario
-- =====================================================
CREATE OR ALTER PROCEDURE SP_AcademicProcesses_Revisers_Insert
    @UserId INT, @PersonType NVARCHAR(20), @PersonId INT, @AssignedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@AssignedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO AcademicProcesses_Revisers (UserId, PersonType, PersonId, IsActive, AssignedBy, AssignedDate)
        VALUES (@UserId, @PersonType, @PersonId, 1, @AssignedBy, GETDATE());
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================
-- SP: SP_AcademicProcesses_Revisers_Update
-- @Mode 1 = Inactivar (remover rol) | @Mode 2 = Reactivar (reasignar)
-- =====================================================
CREATE OR ALTER PROCEDURE SP_AcademicProcesses_Revisers_Update
    @ReviserId INT, @Mode INT, @ActionByUserId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ActionByUserId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @Mode = 1
        BEGIN
            UPDATE AcademicProcesses_Revisers
            SET IsActive = 0, RemovedBy = @ActionByUserId, RemovedDate = GETDATE()
            WHERE ReviserId = @ReviserId AND IsActive = 1;
        END
        ELSE IF @Mode = 2
        BEGIN
            UPDATE AcademicProcesses_Revisers
            SET IsActive = 1, AssignedBy = @ActionByUserId, AssignedDate = GETDATE(),
                RemovedBy = NULL, RemovedDate = NULL
            WHERE ReviserId = @ReviserId AND IsActive = 0;
        END

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================
-- SP: SP_AcademicProcesses_Revisers_GetByUser
-- Verifica si un usuario ya tiene (o tuvo) registro de revisor
-- Usado por el controlador para decidir Insert vs Reactivar vs Bloquear
-- =====================================================
CREATE OR ALTER PROCEDURE SP_AcademicProcesses_Revisers_GetByUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        ReviserId, UserId, PersonType, PersonId, IsActive, AssignedDate, RemovedDate
    FROM AcademicProcesses_Revisers
    WHERE UserId = @UserId;
END
GO

-- =====================================================
-- SP: SP_AcademicProcesses_Revisers_GetById
-- =====================================================
CREATE OR ALTER PROCEDURE SP_AcademicProcesses_Revisers_GetById
    @ReviserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.ReviserId, r.UserId, u.Username, u.FullName AS UserFullName,
        r.PersonType, r.PersonId,
        COALESCE(t.TeacherCode, co.CoordinatorCode, e.EmployeeCode, s.SupplierCode) AS PersonCode,
        COALESCE(t.FullName, co.FullName, (e.FirstName + ' ' + e.LastName), s.SupplierName) AS PersonName,
        r.IsActive, r.AssignedBy, ab.FullName AS AssignedByName, r.AssignedDate,
        r.RemovedBy, rb.FullName AS RemovedByName, r.RemovedDate
    FROM AcademicProcesses_Revisers r
    INNER JOIN Users u ON r.UserId = u.UserId
    LEFT JOIN Users ab ON r.AssignedBy = ab.UserId
    LEFT JOIN Users rb ON r.RemovedBy = rb.UserId
    LEFT JOIN Teachers t ON r.PersonType = 'DOCENTE' AND r.PersonId = t.TeacherId
    LEFT JOIN Coordinators co ON r.PersonType = 'COORDINADOR' AND r.PersonId = co.CoordinatorId
    LEFT JOIN Employees e ON r.PersonType = 'TRABAJADOR' AND r.PersonId = e.EmployeeId
    LEFT JOIN Suppliers s ON r.PersonType = 'PROVEEDOR' AND r.PersonId = s.SupplierId
    WHERE r.ReviserId = @ReviserId;
END
GO

-- =====================================================
-- SP: SP_AcademicProcesses_Revisers_Select
-- Listado paginado con filtro de texto y estado
-- =====================================================
CREATE OR ALTER PROCEDURE SP_AcademicProcesses_Revisers_Select
    @TextoBusqueda NVARCHAR(200) = NULL,
    @TipoFiltro NVARCHAR(20) = 'TODOS',      -- TODOS | ACTIVOS | INACTIVOS
    @PageNumber INT = 1,
    @PageSize INT = 50                        -- <= 0 devuelve todo (exportación)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Busqueda NVARCHAR(200) = NULLIF(LTRIM(RTRIM(@TextoBusqueda)), '');
    SET @TipoFiltro = ISNULL(NULLIF(LTRIM(RTRIM(@TipoFiltro)), ''), 'TODOS');

    SELECT
        r.ReviserId, r.UserId, u.Username, u.FullName AS UserFullName,
        r.PersonType, r.PersonId,
        COALESCE(t.TeacherCode, co.CoordinatorCode, e.EmployeeCode, s.SupplierCode) AS PersonCode,
        COALESCE(t.FullName, co.FullName, (e.FirstName + ' ' + e.LastName), s.SupplierName) AS PersonName,
        r.IsActive, r.AssignedBy, ab.FullName AS AssignedByName, r.AssignedDate,
        r.RemovedBy, rb.FullName AS RemovedByName, r.RemovedDate,
        COUNT(*) OVER() AS TotalRegistros
    FROM AcademicProcesses_Revisers r
    INNER JOIN Users u ON r.UserId = u.UserId
    LEFT JOIN Users ab ON r.AssignedBy = ab.UserId
    LEFT JOIN Users rb ON r.RemovedBy = rb.UserId
    LEFT JOIN Teachers t ON r.PersonType = 'DOCENTE' AND r.PersonId = t.TeacherId
    LEFT JOIN Coordinators co ON r.PersonType = 'COORDINADOR' AND r.PersonId = co.CoordinatorId
    LEFT JOIN Employees e ON r.PersonType = 'TRABAJADOR' AND r.PersonId = e.EmployeeId
    LEFT JOIN Suppliers s ON r.PersonType = 'PROVEEDOR' AND r.PersonId = s.SupplierId
    WHERE
        (@TipoFiltro = 'TODOS' OR (@TipoFiltro = 'ACTIVOS' AND r.IsActive = 1) OR (@TipoFiltro = 'INACTIVOS' AND r.IsActive = 0))
        AND (
            @Busqueda IS NULL
            OR u.Username LIKE '%' + @Busqueda + '%'
            OR u.FullName LIKE '%' + @Busqueda + '%'
            OR t.FullName LIKE '%' + @Busqueda + '%'
            OR co.FullName LIKE '%' + @Busqueda + '%'
            OR (e.FirstName + ' ' + e.LastName) LIKE '%' + @Busqueda + '%'
            OR s.SupplierName LIKE '%' + @Busqueda + '%'
        )
    ORDER BY r.AssignedDate DESC
    OFFSET (@PageNumber - 1) * (CASE WHEN @PageSize <= 0 THEN 0 ELSE @PageSize END) ROWS
    FETCH NEXT (CASE WHEN @PageSize <= 0 THEN 2147483647 ELSE @PageSize END) ROWS ONLY;
END
GO