-- =====================================================================
-- SP_Coordinators_Insert
-- =====================================================================
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