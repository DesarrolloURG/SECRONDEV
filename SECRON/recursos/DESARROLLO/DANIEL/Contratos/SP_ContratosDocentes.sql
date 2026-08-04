-- =========================================================
-- SECRON - SPs Submódulo Emisión de Contratos Docentes (PROVISIONAL)
-- =========================================================

-- ---------------------------------------------------------
-- SP_DocentesTemporal_ObtenerProximoCodigo
-- Genera el próximo código UR2-CSP-XXX-AAAA. El consecutivo
-- se reinicia cada año (se filtra por @Anio).
-- ---------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_DocentesTemporal_ObtenerProximoCodigo
    @Anio INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Sufijo VARCHAR(4) = CAST(@Anio AS VARCHAR(4));
    DECLARE @UltimoNumero INT;

    SELECT @UltimoNumero = MAX(CAST(SUBSTRING(ContractCode, 9, LEN(ContractCode) - 13) AS INT))
    FROM DocentesTemporal
    WHERE ContractCode LIKE 'UR2-CSP-%-' + @Sufijo;

    SET @UltimoNumero = ISNULL(@UltimoNumero, 0) + 1;

    SELECT 'UR2-CSP-' + RIGHT('0000' + CAST(@UltimoNumero AS VARCHAR(10)), 4) + '-' + @Sufijo AS ProximoCodigo;
END
GO

-- ---------------------------------------------------------
-- SP_DocentesTemporal_ObtenerPorDPI
-- Verifica si ya existe un docente (maestro) cargado con ese DPI.
-- Devuelve TeacherTempId y ContractCode si existe (0 filas si no).
-- ---------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_DocentesTemporal_ObtenerPorDPI
    @DPI VARCHAR(13)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 TeacherTempId, ContractCode
    FROM DocentesTemporal
    WHERE DPI = @DPI;
END
GO

-- ---------------------------------------------------------
-- SP_DocentesTemporal_Cursos_ContarPorDPI
-- Cuenta cuántos cursos (detalle) tiene ya cargados un DPI,
-- combinando lo que ya existe en BD (incluye lo insertado en
-- el mismo lote de importación, ya que se inserta fila por fila).
-- ---------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_DocentesTemporal_Cursos_ContarPorDPI
    @DPI VARCHAR(13)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS TotalCursos
    FROM DocentesTemporal_Cursos c
    INNER JOIN DocentesTemporal d ON d.TeacherTempId = c.TeacherTempId
    WHERE d.DPI = @DPI;
END
GO

-- ---------------------------------------------------------
-- SP_DocentesTemporal_Insert
-- Inserta el docente (maestro). NOTA: a diferencia del patrón
-- estándar (SELECT @rows), este SP devuelve el TeacherTempId
-- recién creado (SCOPE_IDENTITY), ya que el Controller lo necesita
-- de inmediato para insertar sus cursos (detalle) a continuación.
-- Se conserva el bloque try/catch/transacción estándar; en error
-- devuelve 0.
-- ---------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_DocentesTemporal_Insert
    @ContractCode     VARCHAR(20),
    @DPI              VARCHAR(13),
    @FirstName        VARCHAR(150),
    @LastName         VARCHAR(150),
    @BirthDate        DATE = NULL,
    @MaritalStatus    VARCHAR(30) = NULL,
    @Gender           VARCHAR(20) = NULL,
    @Address          VARCHAR(300) = NULL,
    @Nationality      VARCHAR(100) = NULL,
    @CollegiateNumber VARCHAR(30) = NULL,
    @NIT              VARCHAR(20) = NULL,
    @Cycle            VARCHAR(20) = NULL,
    @ContractYear     INT = NULL,
    @IssueDate        DATE = NULL,
    @UsuarioId        INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO DocentesTemporal
            (ContractCode, DPI, FirstName, LastName, BirthDate, MaritalStatus, Gender,
             Address, Nationality, CollegiateNumber, NIT, Cycle, ContractYear, IssueDate, CreatedBy)
        VALUES
            (UPPER(@ContractCode), @DPI, UPPER(@FirstName), UPPER(@LastName), @BirthDate,
             UPPER(@MaritalStatus), UPPER(@Gender), UPPER(@Address), UPPER(@Nationality),
             UPPER(@CollegiateNumber), @NIT, UPPER(@Cycle), @ContractYear, @IssueDate, @UsuarioId);

        DECLARE @NuevoId INT = SCOPE_IDENTITY();
        COMMIT TRANSACTION;
        SELECT @NuevoId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0;
    END CATCH
END
GO

-- ---------------------------------------------------------
-- SP_DocentesTemporal_Cursos_Insert
-- Inserta un curso (detalle) enlazado a un docente ya existente.
-- Sigue el patrón estándar: SELECT @rows vía ExecuteScalar.
-- ---------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_DocentesTemporal_Cursos_Insert
    @TeacherTempId    INT,
    @AcademicLocation VARCHAR(150),
    @CourseToTeach    VARCHAR(200),
    @Schedule         VARCHAR(200) = NULL,
    @Fees             DECIMAL(10,2) = NULL,
    @UsuarioId        INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UsuarioId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO DocentesTemporal_Cursos
            (TeacherTempId, AcademicLocation, CourseToTeach, Schedule, Fees, CreatedBy)
        VALUES
            (@TeacherTempId, UPPER(@AcademicLocation), UPPER(@CourseToTeach), UPPER(@Schedule), @Fees, @UsuarioId);

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

-- ---------------------------------------------------------
-- SP_DocentesTemporal_Select
-- Lista todos los docentes (maestro) cargados, con el total
-- de cursos que tiene cada uno. Para pantalla de consulta /
-- selección previo a generar el contrato.
-- ---------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_DocentesTemporal_Select
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.TeacherTempId, d.ContractCode, d.DPI, d.FirstName, d.LastName,
        d.BirthDate, d.MaritalStatus, d.Gender, d.Address, d.Nationality,
        d.CollegiateNumber, d.NIT, d.Cycle, d.ContractYear, d.IssueDate,
        d.CreatedBy, d.CreatedDate,
        (SELECT COUNT(*) FROM DocentesTemporal_Cursos c WHERE c.TeacherTempId = d.TeacherTempId) AS TotalCursos
    FROM DocentesTemporal d
    ORDER BY d.TeacherTempId;
END
GO

-- ---------------------------------------------------------
-- SP_DocentesTemporal_Cursos_SelectByTeacherTempId
-- Lista los cursos (detalle) de un docente específico.
-- ---------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_DocentesTemporal_Cursos_SelectByTeacherTempId
    @TeacherTempId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TeacherTempCourseId, TeacherTempId, AcademicLocation, CourseToTeach, Schedule, Fees,
           CreatedBy, CreatedDate
    FROM DocentesTemporal_Cursos
    WHERE TeacherTempId = @TeacherTempId
    ORDER BY TeacherTempCourseId;
END
GO