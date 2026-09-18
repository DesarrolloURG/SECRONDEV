-- =====================================================
-- SP_Sections_Insert
-- =====================================================
CREATE OR ALTER PROCEDURE SP_Sections_Insert
    @SectionCode      NVARCHAR(20),
    @SectionName      NVARCHAR(100),
    @CareerId         INT,
    @LocationId       INT,
    @ScheduleTypeId   INT,
    @CoordinatorId    INT,
    @CurrentSemester  INT = 1,
    @AcademicYear     INT = NULL,
    @StudentCount     INT = 0,
    @MaxCapacity      INT = 30,
    @StartDate        DATE = NULL,
    @EndDate          DATE = NULL,
    @UsuarioId        INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Sections (
            SectionCode, SectionName, CareerId, LocationId, ScheduleTypeId, CoordinatorId,
            CurrentSemester, AcademicYear, StudentCount, MaxCapacity,
            StartDate, EndDate, IsActive, CreatedDate, CreatedBy
        )
        VALUES (
            UPPER(@SectionCode), UPPER(@SectionName), @CareerId, @LocationId, @ScheduleTypeId, @CoordinatorId,
            @CurrentSemester, @AcademicYear, @StudentCount, @MaxCapacity,
            @StartDate, @EndDate, 1, GETDATE(), @UsuarioId
        );

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION;
        SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0;
    END CATCH
END
GO

-- =====================================================
-- SP_Sections_Update  (@Mode: 0=Actualizar, 1=Inactivar, 2=Reactivar)
-- =====================================================
CREATE OR ALTER PROCEDURE SP_Sections_Update
    @SectionId        INT,
    @SectionCode      NVARCHAR(20)  = NULL,
    @SectionName      NVARCHAR(100) = NULL,
    @CareerId         INT = NULL,
    @LocationId       INT = NULL,
    @ScheduleTypeId   INT = NULL,
    @CoordinatorId    INT = NULL,
    @CurrentSemester  INT = NULL,
    @AcademicYear     INT = NULL,
    @StudentCount     INT = NULL,
    @MaxCapacity      INT = NULL,
    @StartDate        DATE = NULL,
    @EndDate          DATE = NULL,
    @Mode             INT = 0,
    @UsuarioId        INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @Mode = 0
        BEGIN
            UPDATE Sections
            SET SectionCode     = UPPER(@SectionCode),
                SectionName     = UPPER(@SectionName),
                CareerId        = @CareerId,
                LocationId      = @LocationId,
                ScheduleTypeId  = @ScheduleTypeId,
                CoordinatorId   = @CoordinatorId,
                CurrentSemester = @CurrentSemester,
                AcademicYear    = @AcademicYear,
                StudentCount    = @StudentCount,
                MaxCapacity     = @MaxCapacity,
                StartDate       = @StartDate,
                EndDate         = @EndDate,
                ModifiedDate    = GETDATE(),
                ModifiedBy      = @UsuarioId
            WHERE SectionId = @SectionId;
        END
        ELSE IF @Mode = 1
        BEGIN
            UPDATE Sections
            SET IsActive = 0, ModifiedDate = GETDATE(), ModifiedBy = @UsuarioId
            WHERE SectionId = @SectionId;
        END
        ELSE IF @Mode = 2
        BEGIN
            UPDATE Sections
            SET IsActive = 1, ModifiedDate = GETDATE(), ModifiedBy = @UsuarioId
            WHERE SectionId = @SectionId;
        END

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION;
        SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0;
    END CATCH
END
GO

-- =====================================================
-- SP_Sections_Select  (con nombres resueltos por JOIN + 3 filtros estándar)
-- =====================================================
CREATE OR ALTER PROCEDURE SP_Sections_Select
    @Campo      NVARCHAR(20)  = 'TODOS',   -- TODOS / CODIGO / NOMBRE
    @Valor      NVARCHAR(150) = NULL,
    @LocationId INT           = NULL,      -- NULL = todas las sedes
    @Estado     NVARCHAR(20)  = 'TODOS',   -- TODOS / ACTIVOS / INACTIVOS
    @PageNumber INT = 1,
    @PageSize   INT = 100
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageSize IS NULL OR @PageSize <= 0
        SET @PageSize = 2147483647;

    SELECT
        s.SectionId, s.SectionCode, s.SectionName,
        s.CareerId, c.CareerName,
        s.LocationId, l.LocationName,
        s.ScheduleTypeId, st.ScheduleTypeName,
        s.CoordinatorId, co.FullName AS CoordinatorName,
        s.CurrentSemester, s.AcademicYear,
        s.StudentCount, s.MaxCapacity,
        s.StartDate, s.EndDate,
        s.IsActive, s.CreatedDate, s.CreatedBy, s.ModifiedDate, s.ModifiedBy,
        COUNT(*) OVER() AS TotalRows
    FROM Sections s
        INNER JOIN Careers c        ON s.CareerId = c.CareerId
        INNER JOIN Locations l      ON s.LocationId = l.LocationId
        INNER JOIN ScheduleTypes st ON s.ScheduleTypeId = st.ScheduleTypeId
        INNER JOIN Coordinators co  ON s.CoordinatorId = co.CoordinatorId
    WHERE
        (@Estado = 'TODOS'
            OR (@Estado = 'ACTIVOS'   AND s.IsActive = 1)
            OR (@Estado = 'INACTIVOS' AND s.IsActive = 0))
        AND (@LocationId IS NULL OR s.LocationId = @LocationId)
        AND (@Campo = 'TODOS'
            OR (@Campo = 'CODIGO' AND s.SectionCode LIKE '%' + UPPER(@Valor) + '%')
            OR (@Campo = 'NOMBRE' AND s.SectionName LIKE '%' + UPPER(@Valor) + '%'))
    ORDER BY s.SectionName
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO