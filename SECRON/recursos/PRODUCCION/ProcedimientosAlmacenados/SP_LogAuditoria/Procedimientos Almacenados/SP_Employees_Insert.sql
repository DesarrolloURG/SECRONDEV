CREATE OR ALTER PROCEDURE SP_Employees_Insert
    @EmployeeCode VARCHAR(20), @FirstName VARCHAR(100), @LastName VARCHAR(100),
    @IdentificationNumber VARCHAR(20), @Email VARCHAR(150) = NULL, @InstitutionalEmail VARCHAR(150),
    @Phone VARCHAR(20) = NULL, @MobilePhone VARCHAR(20) = NULL, @Address VARCHAR(255) = NULL,
    @BirthDate DATETIME = NULL, @HireDate DATETIME, @DepartmentId INT, @PositionId INT,
    @DirectSupervisorId INT = NULL, @EmployeeStatusId INT, @LocationId INT = NULL,
    @TipoContratacion VARCHAR(50) = NULL, @EmergencyContactName VARCHAR(150) = NULL,
    @EmergencyContactPhone VARCHAR(20) = NULL, @EmergencyContactRelation VARCHAR(50) = NULL,
    @NominalSalary DECIMAL(18,2) = NULL, @BaseSalary DECIMAL(18,2) = NULL,
    @AdditionalBonus DECIMAL(18,2) = NULL, @LegalBonus DECIMAL(18,2) = NULL,
    @IGSS DECIMAL(18,2) = NULL, @ISR DECIMAL(18,2) = NULL, @NetSalary DECIMAL(18,2) = NULL,
    @IGSSManual BIT = NULL, @IsActive BIT, @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Employees
            (EmployeeCode, FirstName, LastName, IdentificationNumber, Email, InstitutionalEmail,
             Phone, MobilePhone, Address, BirthDate, HireDate, DepartmentId, PositionId,
             DirectSupervisorId, EmployeeStatusId, LocationId, TipoContratacion,
             EmergencyContactName, EmergencyContactPhone, EmergencyContactRelation,
             nominal_salary, base_salary, additional_bonus, legal_bonus, IGSS, ISR, net_salary,
             IGSS_MANUAL, IsActive, CreatedBy)
        VALUES
            (@EmployeeCode, @FirstName, @LastName, @IdentificationNumber, @Email, @InstitutionalEmail,
             @Phone, @MobilePhone, @Address, @BirthDate, @HireDate, @DepartmentId, @PositionId,
             @DirectSupervisorId, @EmployeeStatusId, @LocationId, @TipoContratacion,
             @EmergencyContactName, @EmergencyContactPhone, @EmergencyContactRelation,
             @NominalSalary, @BaseSalary, @AdditionalBonus, @LegalBonus, @IGSS, @ISR, @NetSalary,
             @IGSSManual, @IsActive, @CreatedBy);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO