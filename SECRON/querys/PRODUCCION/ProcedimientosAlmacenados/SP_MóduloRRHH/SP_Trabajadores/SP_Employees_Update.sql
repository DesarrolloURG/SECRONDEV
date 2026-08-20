/* =====================================================================
   SECRON - RRHH / Ficha de Empleado
   Estandarizar SP_Employees_Update a @Mode (0=update, 1=inactivar, 2=reactivar)
   Antes usaba @IsInactivation BIT, que no soportaba reactivación.
   Aplicar en DEV, validar, y luego replicar en QA y Producción.
   ===================================================================== */

CREATE OR ALTER PROCEDURE SP_Employees_Update
    @EmployeeId INT, @Mode TINYINT,
    @EmployeeCode VARCHAR(20) = NULL, @FirstName VARCHAR(100) = NULL, @LastName VARCHAR(100) = NULL,
    @IdentificationNumber VARCHAR(20) = NULL, @Email VARCHAR(150) = NULL, @InstitutionalEmail VARCHAR(150) = NULL,
    @Phone VARCHAR(20) = NULL, @MobilePhone VARCHAR(20) = NULL, @Address VARCHAR(255) = NULL,
    @BirthDate DATETIME = NULL, @HireDate DATETIME = NULL, @DepartmentId INT = NULL, @PositionId INT = NULL,
    @DirectSupervisorId INT = NULL, @EmployeeStatusId INT = NULL, @LocationId INT = NULL,
    @TipoContratacion VARCHAR(50) = NULL, @EmergencyContactName VARCHAR(150) = NULL,
    @EmergencyContactPhone VARCHAR(20) = NULL, @EmergencyContactRelation VARCHAR(50) = NULL,
    @NominalSalary DECIMAL(18,2) = NULL, @BaseSalary DECIMAL(18,2) = NULL,
    @AdditionalBonus DECIMAL(18,2) = NULL, @LegalBonus DECIMAL(18,2) = NULL,
    @IGSS DECIMAL(18,2) = NULL, @ISR DECIMAL(18,2) = NULL, @NetSalary DECIMAL(18,2) = NULL,
    @IGSSManual BIT = NULL, @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @Mode = 1
            -- Inactivar (antes @IsInactivation = 1)
            UPDATE Employees SET IsActive = 0, TerminationDate = GETDATE(),
                ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE EmployeeId = @EmployeeId;
        ELSE IF @Mode = 2
            -- Reactivar (nuevo: no existía)
            UPDATE Employees SET IsActive = 1, TerminationDate = NULL,
                ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE EmployeeId = @EmployeeId;
        ELSE
            -- Update normal (antes @IsInactivation = 0)
            UPDATE Employees SET
                EmployeeCode = @EmployeeCode, FirstName = @FirstName, LastName = @LastName,
                IdentificationNumber = @IdentificationNumber, Email = @Email,
                InstitutionalEmail = @InstitutionalEmail, Phone = @Phone, MobilePhone = @MobilePhone,
                Address = @Address, BirthDate = @BirthDate, HireDate = @HireDate,
                DepartmentId = @DepartmentId, PositionId = @PositionId,
                DirectSupervisorId = @DirectSupervisorId, EmployeeStatusId = @EmployeeStatusId,
                LocationId = @LocationId, TipoContratacion = @TipoContratacion,
                EmergencyContactName = @EmergencyContactName, EmergencyContactPhone = @EmergencyContactPhone,
                EmergencyContactRelation = @EmergencyContactRelation,
                nominal_salary = @NominalSalary, base_salary = @BaseSalary,
                additional_bonus = @AdditionalBonus, legal_bonus = @LegalBonus,
                IGSS = @IGSS, ISR = @ISR, net_salary = @NetSalary, IGSS_MANUAL = @IGSSManual,
                ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE EmployeeId = @EmployeeId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO