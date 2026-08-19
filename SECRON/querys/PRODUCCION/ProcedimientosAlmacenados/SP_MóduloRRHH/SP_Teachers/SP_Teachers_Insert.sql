CREATE OR ALTER PROCEDURE SP_Teachers_Insert
    @TeacherCode VARCHAR(20), @FullName VARCHAR(150), @Phone VARCHAR(20) = NULL,
    @Email VARCHAR(150) = NULL, @DPI VARCHAR(20) = NULL, @NIT VARCHAR(20) = NULL,
    @Address VARCHAR(255) = NULL, @AcademicTitle VARCHAR(100) = NULL, @Specialization VARCHAR(150) = NULL,
    @IsCollegiateActive BIT, @CollegiateNumber VARCHAR(50) = NULL, @BankAccountNumber VARCHAR(50) = NULL,
    @BankId INT = NULL, @LocationId INT, @HireDate DATETIME = NULL, @ContractType VARCHAR(50) = NULL,
    @NominalSalary DECIMAL(10,2) = NULL, @BaseSalary DECIMAL(10,2) = NULL,
    @AdditionalBonus DECIMAL(10,2) = NULL, @LegalBonus DECIMAL(10,2) = NULL,
    @IGSS DECIMAL(10,2) = NULL, @ISR DECIMAL(10,2) = NULL, @NetSalary DECIMAL(10,2) = NULL,
    @IGSSManual BIT = NULL,
    @UserId INT = NULL, @RegisteredByCoordinatorId INT = NULL, @IsActive BIT, @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Teachers (TeacherCode, FullName, Phone, Email, DPI, NIT, Address, AcademicTitle,
            Specialization, IsCollegiateActive, CollegiateNumber, BankAccountNumber, BankId, LocationId,
            HireDate, ContractType,
            NominalSalary, BaseSalary, AdditionalBonus, LegalBonus, IGSS, ISR, NetSalary, IGSSManual,
            UserId, RegisteredByCoordinatorId, IsActive, CreatedDate, CreatedBy)
        VALUES (@TeacherCode, @FullName, @Phone, @Email, @DPI, @NIT, @Address, @AcademicTitle,
            @Specialization, @IsCollegiateActive, @CollegiateNumber, @BankAccountNumber, @BankId, @LocationId,
            @HireDate, @ContractType,
            @NominalSalary, @BaseSalary, @AdditionalBonus, @LegalBonus, @IGSS, @ISR, @NetSalary, @IGSSManual,
            @UserId, @RegisteredByCoordinatorId, @IsActive, GETDATE(), @CreatedBy);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO