-- =====================================================================
-- MÓDULO: PENSUM DE CURSOS (Courses)
-- =====================================================================

-- =====================================================================
-- SP: SP_Courses_Insert
-- =====================================================================
IF OBJECT_ID('SP_Courses_Insert', 'P') IS NOT NULL DROP PROCEDURE SP_Courses_Insert;
GO
CREATE PROCEDURE SP_Courses_Insert
    @CourseCode      NVARCHAR(20),
    @CourseName      NVARCHAR(150),
    @Description     NVARCHAR(500) = NULL,
    @Credits         INT = 0,
    @TheoryHours     INT = NULL,
    @PracticeHours   INT = NULL,
    @LabHours        INT = NULL,
    @Sessions        INT = NULL,
    @IsCommon        BIT = 0,
    @IsActive        BIT = 1,
    @UsuarioId       INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Courses
            (CourseCode, CourseName, Description, Credits, TheoryHours, PracticeHours, LabHours,
             Sessions, IsCommon, IsActive, CreatedDate, CreatedBy)
        VALUES
            (UPPER(@CourseCode), UPPER(@CourseName), UPPER(@Description), @Credits, @TheoryHours, @PracticeHours, @LabHours,
             @Sessions, @IsCommon, @IsActive, GETDATE(), @UsuarioId);

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================================
-- SP: SP_Courses_Update
-- =====================================================================
IF OBJECT_ID('SP_Courses_Update', 'P') IS NOT NULL DROP PROCEDURE SP_Courses_Update;
GO
CREATE PROCEDURE SP_Courses_Update
    @CourseId        INT,
    @CourseCode      NVARCHAR(20),
    @CourseName      NVARCHAR(150),
    @Description     NVARCHAR(500) = NULL,
    @Credits         INT = 0,
    @TheoryHours     INT = NULL,
    @PracticeHours   INT = NULL,
    @LabHours        INT = NULL,
    @Sessions        INT = NULL,
    @IsCommon        BIT = 0,
    @UsuarioId       INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Courses
        SET CourseCode     = UPPER(@CourseCode),
            CourseName     = UPPER(@CourseName),
            Description    = UPPER(@Description),
            Credits        = @Credits,
            TheoryHours    = @TheoryHours,
            PracticeHours  = @PracticeHours,
            LabHours       = @LabHours,
            Sessions       = @Sessions,
            IsCommon       = @IsCommon,
            ModifiedDate   = GETDATE(),
            ModifiedBy     = @UsuarioId
        WHERE CourseId = @CourseId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================================
-- SP: SP_Courses_UpdateStatus   (@Mode 1 = Inactivar, 2 = Reactivar)
-- =====================================================================
IF OBJECT_ID('SP_Courses_UpdateStatus', 'P') IS NOT NULL DROP PROCEDURE SP_Courses_UpdateStatus;
GO
CREATE PROCEDURE SP_Courses_UpdateStatus
    @CourseId  INT,
    @Mode      INT,
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Courses
        SET IsActive     = CASE WHEN @Mode = 1 THEN 0 WHEN @Mode = 2 THEN 1 ELSE IsActive END,
            ModifiedDate = GETDATE(),
            ModifiedBy   = @UsuarioId
        WHERE CourseId = @CourseId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================================
-- SP: SP_Courses_Select   (listado + filtros + paginación)
-- @Campo:  TODOS / CODIGO / NOMBRE / CREDITOS / SESIONES
-- @Estado: TODOS / ACTIVA / INACTIVA
-- @Comun:  TODOS / SI / NO
-- =====================================================================
IF OBJECT_ID('SP_Courses_Select', 'P') IS NOT NULL DROP PROCEDURE SP_Courses_Select;
GO
CREATE PROCEDURE SP_Courses_Select
    @Campo      NVARCHAR(20)  = 'TODOS',
    @Valor      NVARCHAR(150) = NULL,
    @Estado     NVARCHAR(20)  = 'TODOS',
    @Comun      NVARCHAR(10)  = 'TODOS',
    @PageNumber INT = 1,
    @PageSize   INT = 100
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageSize IS NULL OR @PageSize <= 0
        SET @PageSize = 2147483647;

    SELECT
        CourseId, CourseCode, CourseName, Description, Credits,
        TheoryHours, PracticeHours, LabHours, TotalHours, Sessions, IsCommon,
        IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy,
        COUNT(*) OVER() AS TotalRows
    FROM Courses
    WHERE
        (@Estado = 'TODOS'
         OR (@Estado = 'ACTIVA'   AND IsActive = 1)
         OR (@Estado = 'INACTIVA' AND IsActive = 0))
    AND
        (@Comun = 'TODOS'
         OR (@Comun = 'SI' AND IsCommon = 1)
         OR (@Comun = 'NO' AND IsCommon = 0))
    AND
        (@Campo = 'TODOS'
         OR (@Campo = 'CODIGO'    AND CourseCode LIKE '%' + UPPER(@Valor) + '%')
         OR (@Campo = 'NOMBRE'    AND CourseName LIKE '%' + UPPER(@Valor) + '%')
         OR (@Campo = 'CREDITOS'  AND Credits  = TRY_CAST(@Valor AS INT))
         OR (@Campo = 'SESIONES'  AND Sessions = TRY_CAST(@Valor AS INT)))
    ORDER BY CourseName
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO