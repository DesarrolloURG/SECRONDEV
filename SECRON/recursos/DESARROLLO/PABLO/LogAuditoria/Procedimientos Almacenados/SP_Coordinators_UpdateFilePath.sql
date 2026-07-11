-- =====================================================================
-- SECRON - MÓDULO COORDINADORES
-- Script completo: ALTER TABLE + Stored Procedures
-- Espejo de Teachers, con LocationId NULLABLE
-- Ejecutar una vez por entorno (DEV / QA / PROD)
-- =====================================================================

-- ---------------------------------------------------------------------
-- 1) Permitir LocationId NULL (un coordinador puede no tener sede)
-- ---------------------------------------------------------------------
ALTER TABLE Coordinators ALTER COLUMN LocationId INT NULL;
GO

-- ---------------------------------------------------------------------
-- 2) Agregar columnas de datos faltantes + columnas de archivos
--    (alinea Coordinators con Teachers)
-- ---------------------------------------------------------------------
ALTER TABLE Coordinators ADD
    IsCollegiateActive        BIT NOT NULL DEFAULT 0,
    CollegiateNumber          NVARCHAR(50) NULL,
    HireDate                  DATETIME NULL,
    ContractType              NVARCHAR(50) NULL,
    RegisteredByCoordinatorId INT NULL,
    FilePath_DPI              NVARCHAR(500) NULL,
    FilePath_Titulos          NVARCHAR(500) NULL,
    FilePath_RTU              NVARCHAR(500) NULL,
    FilePath_Colegiado        NVARCHAR(500) NULL,
    FilePath_RENAS            NVARCHAR(500) NULL,
    FilePath_AntPoliciacos    NVARCHAR(500) NULL,
    FilePath_AntPenales       NVARCHAR(500) NULL;
GO

-- ---------------------------------------------------------------------
-- 3) SP_Coordinators_Insert
-- ---------------------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_Coordinators_Insert
    @CoordinatorCode VARCHAR(20), @FullName VARCHAR(150), @Phone VARCHAR(20) = NULL,
    @Email VARCHAR(150) = NULL, @DPI VARCHAR(20) = NULL, @NIT VARCHAR(20) = NULL,
    @Address VARCHAR(255) = NULL, @AcademicTitle VARCHAR(100) = NULL, @Specialization VARCHAR(150) = NULL,
    @IsCollegiateActive BIT, @CollegiateNumber VARCHAR(50) = NULL, @BankAccountNumber VARCHAR(50) = NULL,
    @BankId INT = NULL, @LocationId INT = NULL, @HireDate DATETIME = NULL, @ContractType VARCHAR(50) = NULL,
    @UserId INT = NULL, @RegisteredByCoordinatorId INT = NULL, @IsActive BIT, @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Coordinators (CoordinatorCode, FullName, Phone, Email, DPI, NIT, Address, AcademicTitle,
            Specialization, IsCollegiateActive, CollegiateNumber, BankAccountNumber, BankId, LocationId,
            HireDate, ContractType, UserId, RegisteredByCoordinatorId, IsActive, CreatedDate, CreatedBy)
        VALUES (@CoordinatorCode, @FullName, @Phone, @Email, @DPI, @NIT, @Address, @AcademicTitle,
            @Specialization, @IsCollegiateActive, @CollegiateNumber, @BankAccountNumber, @BankId, @LocationId,
            @HireDate, @ContractType, @UserId, @RegisteredByCoordinatorId, @IsActive, GETDATE(), @CreatedBy);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- ---------------------------------------------------------------------
-- 4) SP_Coordinators_Update
--    @Mode: 0 = update normal, 1 = inactivar, 2 = reactivar
-- ---------------------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_Coordinators_Update
    @CoordinatorId INT, @Mode TINYINT,
    @CoordinatorCode VARCHAR(20) = NULL, @FullName VARCHAR(150) = NULL, @Phone VARCHAR(20) = NULL,
    @Email VARCHAR(150) = NULL, @DPI VARCHAR(20) = NULL, @NIT VARCHAR(20) = NULL,
    @Address VARCHAR(255) = NULL, @AcademicTitle VARCHAR(100) = NULL, @Specialization VARCHAR(150) = NULL,
    @IsCollegiateActive BIT = NULL, @CollegiateNumber VARCHAR(50) = NULL, @BankAccountNumber VARCHAR(50) = NULL,
    @BankId INT = NULL, @LocationId INT = NULL, @HireDate DATETIME = NULL, @ContractType VARCHAR(50) = NULL,
    @UserId INT = NULL, @RegisteredByCoordinatorId INT = NULL, @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @Mode = 1
            UPDATE Coordinators SET IsActive = 0, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE CoordinatorId = @CoordinatorId;
        ELSE IF @Mode = 2
            UPDATE Coordinators SET IsActive = 1, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE CoordinatorId = @CoordinatorId;
        ELSE
            UPDATE Coordinators SET CoordinatorCode = @CoordinatorCode, FullName = @FullName, Phone = @Phone,
                Email = @Email, DPI = @DPI, NIT = @NIT, Address = @Address, AcademicTitle = @AcademicTitle,
                Specialization = @Specialization, IsCollegiateActive = @IsCollegiateActive,
                CollegiateNumber = @CollegiateNumber, BankAccountNumber = @BankAccountNumber, BankId = @BankId,
                LocationId = @LocationId, HireDate = @HireDate, ContractType = @ContractType, UserId = @UserId,
                RegisteredByCoordinatorId = @RegisteredByCoordinatorId,
                ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE CoordinatorId = @CoordinatorId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- ---------------------------------------------------------------------
-- 5) SP_Coordinators_UpdateFilePath
-- ---------------------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_Coordinators_UpdateFilePath
    @CoordinatorId INT,
    @Campo         NVARCHAR(50),
    @Ruta          NVARCHAR(500) = NULL,
    @ModifiedBy    INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    IF @Campo NOT IN (
        'FilePath_DPI', 'FilePath_Titulos', 'FilePath_RTU',
        'FilePath_Colegiado', 'FilePath_RENAS',
        'FilePath_AntPoliciacos', 'FilePath_AntPenales'
    )
    BEGIN
        SELECT -1; RETURN;
    END

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        DECLARE @sql NVARCHAR(MAX);

        SET @sql = N'UPDATE Coordinators SET ' + QUOTENAME(@Campo) + N' = @Ruta, ' +
                   N'ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy ' +
                   N'WHERE CoordinatorId = @CoordinatorId;';

        EXEC sp_executesql @sql,
            N'@Ruta NVARCHAR(500), @ModifiedBy INT, @CoordinatorId INT',
            @Ruta = @Ruta, @ModifiedBy = @ModifiedBy, @CoordinatorId = @CoordinatorId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO