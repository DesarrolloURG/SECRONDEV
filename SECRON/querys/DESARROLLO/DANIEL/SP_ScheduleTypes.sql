-- =====================================================
-- SP_ScheduleTypes_Insert
-- =====================================================
CREATE OR ALTER PROCEDURE SP_ScheduleTypes_Insert
    @ScheduleTypeCode  NVARCHAR(20),
    @ScheduleTypeName  NVARCHAR(100),
    @Description       NVARCHAR(255) = NULL,
    @IncludesMonday    BIT = 0,
    @IncludesTuesday   BIT = 0,
    @IncludesWednesday BIT = 0,
    @IncludesThursday  BIT = 0,
    @IncludesFriday    BIT = 0,
    @IncludesSaturday  BIT = 0,
    @IncludesSunday    BIT = 0,
    @TimeShift         NVARCHAR(50) = NULL,
    @UsuarioId         INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO ScheduleTypes (
            ScheduleTypeCode, ScheduleTypeName, Description,
            IncludesMonday, IncludesTuesday, IncludesWednesday, IncludesThursday,
            IncludesFriday, IncludesSaturday, IncludesSunday,
            TimeShift, IsActive, CreatedDate, CreatedBy
        )
        VALUES (
            UPPER(@ScheduleTypeCode), UPPER(@ScheduleTypeName), UPPER(@Description),
            @IncludesMonday, @IncludesTuesday, @IncludesWednesday, @IncludesThursday,
            @IncludesFriday, @IncludesSaturday, @IncludesSunday,
            UPPER(@TimeShift), 1, GETDATE(), @UsuarioId
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
-- SP_ScheduleTypes_Update  (@Mode: 0=Actualizar, 1=Inactivar, 2=Reactivar)
-- =====================================================
CREATE OR ALTER PROCEDURE SP_ScheduleTypes_Update
    @ScheduleTypeId    INT,
    @ScheduleTypeCode  NVARCHAR(20)  = NULL,
    @ScheduleTypeName  NVARCHAR(100) = NULL,
    @Description       NVARCHAR(255) = NULL,
    @IncludesMonday    BIT = NULL,
    @IncludesTuesday   BIT = NULL,
    @IncludesWednesday BIT = NULL,
    @IncludesThursday  BIT = NULL,
    @IncludesFriday    BIT = NULL,
    @IncludesSaturday  BIT = NULL,
    @IncludesSunday    BIT = NULL,
    @TimeShift         NVARCHAR(50) = NULL,
    @Mode              INT = 0,
    @UsuarioId         INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @Mode = 0
        BEGIN
            UPDATE ScheduleTypes
            SET ScheduleTypeCode  = UPPER(@ScheduleTypeCode),
                ScheduleTypeName  = UPPER(@ScheduleTypeName),
                Description       = UPPER(@Description),
                IncludesMonday    = @IncludesMonday,
                IncludesTuesday   = @IncludesTuesday,
                IncludesWednesday = @IncludesWednesday,
                IncludesThursday  = @IncludesThursday,
                IncludesFriday    = @IncludesFriday,
                IncludesSaturday  = @IncludesSaturday,
                IncludesSunday    = @IncludesSunday,
                TimeShift         = UPPER(@TimeShift),
                ModifiedDate      = GETDATE(),
                ModifiedBy        = @UsuarioId
            WHERE ScheduleTypeId = @ScheduleTypeId;
        END
        ELSE IF @Mode = 1
        BEGIN
            UPDATE ScheduleTypes
            SET IsActive = 0, ModifiedDate = GETDATE(), ModifiedBy = @UsuarioId
            WHERE ScheduleTypeId = @ScheduleTypeId;
        END
        ELSE IF @Mode = 2
        BEGIN
            UPDATE ScheduleTypes
            SET IsActive = 1, ModifiedDate = GETDATE(), ModifiedBy = @UsuarioId
            WHERE ScheduleTypeId = @ScheduleTypeId;
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
-- SP_ScheduleTypes_Select  (Filtro3 + búsqueda + paginación)
-- =====================================================
CREATE OR ALTER PROCEDURE SP_ScheduleTypes_Select
    @Campo      NVARCHAR(20)  = 'TODOS',   -- TODOS / CODIGO / NOMBRE
    @Valor      NVARCHAR(150) = NULL,
    @TimeShift  NVARCHAR(50)  = 'TODOS',   -- TODOS / MAÑANA / TARDE / NOCHE / MIXTO
    @Estado     NVARCHAR(20)  = 'TODOS',   -- TODOS / ACTIVOS / INACTIVOS
    @PageNumber INT = 1,
    @PageSize   INT = 100
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageSize IS NULL OR @PageSize <= 0
        SET @PageSize = 2147483647;

    SELECT
        ScheduleTypeId, ScheduleTypeCode, ScheduleTypeName, Description,
        IncludesMonday, IncludesTuesday, IncludesWednesday, IncludesThursday,
        IncludesFriday, IncludesSaturday, IncludesSunday,
        TimeShift, IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy,
        COUNT(*) OVER() AS TotalRows
    FROM ScheduleTypes
    WHERE
        (@Estado = 'TODOS'
            OR (@Estado = 'ACTIVOS'   AND IsActive = 1)
            OR (@Estado = 'INACTIVOS' AND IsActive = 0))
        AND (@TimeShift = 'TODOS' OR TimeShift = UPPER(@TimeShift))
        AND (@Campo = 'TODOS'
            OR (@Campo = 'CODIGO' AND ScheduleTypeCode LIKE '%' + UPPER(@Valor) + '%')
            OR (@Campo = 'NOMBRE' AND ScheduleTypeName LIKE '%' + UPPER(@Valor) + '%'))
    ORDER BY ScheduleTypeName
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO