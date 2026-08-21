-- =====================================================================
-- MÓDULO: PENSUM DE CARRERAS (Careers)
-- Archivo único con el CRUD completo
-- =====================================================================

-- =====================================================================
-- SP: SP_Careers_Insert
-- =====================================================================
IF OBJECT_ID('SP_Careers_Insert', 'P') IS NOT NULL DROP PROCEDURE SP_Careers_Insert;
GO
CREATE PROCEDURE SP_Careers_Insert
    @CareerCode      NVARCHAR(20),
    @CareerName      NVARCHAR(150),
    @Description     NVARCHAR(500) = NULL,
    @DurationYears   INT = NULL,
    @TotalSemesters  INT = NULL,
    @TotalCredits    INT = NULL,
    @IsActive        BIT = 1,
    @UsuarioId       INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Careers
            (CareerCode, CareerName, Description, DurationYears, TotalSemesters, TotalCredits, IsActive, CreatedDate, CreatedBy)
        VALUES
            (UPPER(@CareerCode), UPPER(@CareerName), UPPER(@Description), @DurationYears, @TotalSemesters, @TotalCredits, @IsActive, GETDATE(), @UsuarioId);

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================================
-- SP: SP_Careers_Update
-- =====================================================================
IF OBJECT_ID('SP_Careers_Update', 'P') IS NOT NULL DROP PROCEDURE SP_Careers_Update;
GO
CREATE PROCEDURE SP_Careers_Update
    @CareerId        INT,
    @CareerCode      NVARCHAR(20),
    @CareerName      NVARCHAR(150),
    @Description     NVARCHAR(500) = NULL,
    @DurationYears   INT = NULL,
    @TotalSemesters  INT = NULL,
    @TotalCredits    INT = NULL,
    @UsuarioId       INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Careers
        SET CareerCode     = UPPER(@CareerCode),
            CareerName     = UPPER(@CareerName),
            Description    = UPPER(@Description),
            DurationYears  = @DurationYears,
            TotalSemesters = @TotalSemesters,
            TotalCredits   = @TotalCredits,
            ModifiedDate   = GETDATE(),
            ModifiedBy     = @UsuarioId
        WHERE CareerId = @CareerId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================================
-- SP: SP_Careers_UpdateStatus   (@Mode 1 = Inactivar, 2 = Reactivar)
-- =====================================================================
IF OBJECT_ID('SP_Careers_UpdateStatus', 'P') IS NOT NULL DROP PROCEDURE SP_Careers_UpdateStatus;
GO
CREATE PROCEDURE SP_Careers_UpdateStatus
    @CareerId  INT,
    @Mode      INT,
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Careers
        SET IsActive     = CASE WHEN @Mode = 1 THEN 0 WHEN @Mode = 2 THEN 1 ELSE IsActive END,
            ModifiedDate = GETDATE(),
            ModifiedBy   = @UsuarioId
        WHERE CareerId = @CareerId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================================
-- SP: SP_Careers_Select   (listado + filtros + paginación)
-- @Campo:  TODOS / CODIGO / NOMBRE / DURACION / SEMESTRES / CREDITOS
-- @Estado: TODOS / ACTIVA / INACTIVA
-- =====================================================================
IF OBJECT_ID('SP_Careers_Select', 'P') IS NOT NULL DROP PROCEDURE SP_Careers_Select;
GO
CREATE PROCEDURE SP_Careers_Select
    @Campo      NVARCHAR(20)  = 'TODOS',
    @Valor      NVARCHAR(150) = NULL,
    @Estado     NVARCHAR(20)  = 'TODOS',
    @PageNumber INT = 1,
    @PageSize   INT = 100     -- 0 o negativo = SIN paginar (usado por EXPORTAR)
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageSize IS NULL OR @PageSize <= 0
        SET @PageSize = 2147483647;

    SELECT
        CareerId, CareerCode, CareerName, Description,
        DurationYears, TotalSemesters, TotalCredits,
        IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy,
        COUNT(*) OVER() AS TotalRows
    FROM Careers
    WHERE
        (@Estado = 'TODOS'
         OR (@Estado = 'ACTIVA'   AND IsActive = 1)
         OR (@Estado = 'INACTIVA' AND IsActive = 0))
    AND
        (@Campo = 'TODOS'
         OR (@Campo = 'CODIGO'     AND CareerCode LIKE '%' + UPPER(@Valor) + '%')
         OR (@Campo = 'NOMBRE'     AND CareerName LIKE '%' + UPPER(@Valor) + '%')
         OR (@Campo = 'DURACION'   AND DurationYears  = TRY_CAST(@Valor AS INT))
         OR (@Campo = 'SEMESTRES'  AND TotalSemesters = TRY_CAST(@Valor AS INT))
         OR (@Campo = 'CREDITOS'   AND TotalCredits   = TRY_CAST(@Valor AS INT)))
    ORDER BY CareerName
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO