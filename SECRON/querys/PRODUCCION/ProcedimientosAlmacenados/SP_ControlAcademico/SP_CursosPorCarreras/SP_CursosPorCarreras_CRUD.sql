-- =====================================================================
-- ALTER PREVIO: Semester pasa a ser opcional (carga masiva sin ese dato)
-- =====================================================================
ALTER TABLE CareerCourses ALTER COLUMN Semester INT NULL;
GO

-- =====================================================================
-- MÓDULO: CURSOS POR CARRERA (CareerCourses)
-- Archivo único con el CRUD completo
-- =====================================================================

-- =====================================================================
-- SP: SP_CareerCourses_Insert
-- =====================================================================
IF OBJECT_ID('SP_CareerCourses_Insert', 'P') IS NOT NULL DROP PROCEDURE SP_CareerCourses_Insert;
GO
CREATE PROCEDURE SP_CareerCourses_Insert
    @CareerId       INT,
    @CourseId       INT,
    @Semester       INT = NULL,
    @IsRequired     BIT = NULL,
    @Prerequisites  NVARCHAR(500) = NULL,
    @IsActive       BIT = 1,
    @UsuarioId      INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM CareerCourses WHERE CareerId = @CareerId AND CourseId = @CourseId AND IsActive = 1)
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        INSERT INTO CareerCourses
            (CareerId, CourseId, Semester, IsRequired, Prerequisites, IsActive, CreatedDate, CreatedBy)
        VALUES
            (@CareerId, @CourseId, @Semester, @IsRequired, @Prerequisites, @IsActive, GETDATE(), @UsuarioId);

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================================
-- SP: SP_CareerCourses_Update
-- =====================================================================
IF OBJECT_ID('SP_CareerCourses_Update', 'P') IS NOT NULL DROP PROCEDURE SP_CareerCourses_Update;
GO
CREATE PROCEDURE SP_CareerCourses_Update
    @CareerCourseId INT,
    @CareerId       INT,
    @CourseId       INT,
    @Semester       INT = NULL,
    @IsRequired     BIT = NULL,
    @Prerequisites  NVARCHAR(500) = NULL,
    @UsuarioId      INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM CareerCourses
            WHERE CareerId = @CareerId AND CourseId = @CourseId AND IsActive = 1
              AND CareerCourseId <> @CareerCourseId
        )
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        UPDATE CareerCourses
        SET CareerId      = @CareerId,
            CourseId      = @CourseId,
            Semester      = @Semester,
            IsRequired    = @IsRequired,
            Prerequisites = @Prerequisites,
            ModifiedDate  = GETDATE(),
            ModifiedBy    = @UsuarioId
        WHERE CareerCourseId = @CareerCourseId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================================
-- SP: SP_CareerCourses_UpdateStatus   (@Mode 1 = Inactivar, 2 = Reactivar)
-- =====================================================================
IF OBJECT_ID('SP_CareerCourses_UpdateStatus', 'P') IS NOT NULL DROP PROCEDURE SP_CareerCourses_UpdateStatus;
GO
CREATE PROCEDURE SP_CareerCourses_UpdateStatus
    @CareerCourseId INT,
    @Mode           INT,
    @UsuarioId      INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE CareerCourses
        SET IsActive     = CASE WHEN @Mode = 1 THEN 0 WHEN @Mode = 2 THEN 1 ELSE IsActive END,
            ModifiedDate = GETDATE(),
            ModifiedBy   = @UsuarioId
        WHERE CareerCourseId = @CareerCourseId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================================
-- SP: SP_CareerCourses_Select   (cursos de una carrera, con datos del curso)
-- @Estado: TODOS / ACTIVA / INACTIVA
-- =====================================================================
IF OBJECT_ID('SP_CareerCourses_Select', 'P') IS NOT NULL DROP PROCEDURE SP_CareerCourses_Select;
GO
CREATE PROCEDURE SP_CareerCourses_Select
    @CareerId INT,
    @Estado   NVARCHAR(20) = 'TODOS'
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        cc.CareerCourseId, cc.CareerId, cc.CourseId,
        c.CourseCode, c.CourseName,
        cc.Semester, cc.IsRequired, cc.Prerequisites,
        cc.IsActive, cc.CreatedDate, cc.CreatedBy, cc.ModifiedDate, cc.ModifiedBy
    FROM CareerCourses cc
    INNER JOIN Courses c ON cc.CourseId = c.CourseId
    WHERE cc.CareerId = @CareerId
      AND (@Estado = 'TODOS'
           OR (@Estado = 'ACTIVA'   AND cc.IsActive = 1)
           OR (@Estado = 'INACTIVA' AND cc.IsActive = 0))
    ORDER BY cc.Semester, c.CourseName;
END
GO