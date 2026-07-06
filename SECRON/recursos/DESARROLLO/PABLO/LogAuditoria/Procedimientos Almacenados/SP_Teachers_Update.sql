-- @Mode: 0 = update normal, 1 = inactivar, 2 = reactivar
CREATE OR ALTER PROCEDURE SP_Teachers_Update
    @TeacherId INT, @Mode TINYINT,
    @TeacherCode VARCHAR(20) = NULL, @FullName VARCHAR(150) = NULL, @Phone VARCHAR(20) = NULL,
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
            UPDATE Teachers SET IsActive = 0, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE TeacherId = @TeacherId;
        ELSE IF @Mode = 2
            UPDATE Teachers SET IsActive = 1, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE TeacherId = @TeacherId;
        ELSE
            UPDATE Teachers SET TeacherCode = @TeacherCode, FullName = @FullName, Phone = @Phone,
                Email = @Email, DPI = @DPI, NIT = @NIT, Address = @Address, AcademicTitle = @AcademicTitle,
                Specialization = @Specialization, IsCollegiateActive = @IsCollegiateActive,
                CollegiateNumber = @CollegiateNumber, BankAccountNumber = @BankAccountNumber, BankId = @BankId,
                LocationId = @LocationId, HireDate = @HireDate, ContractType = @ContractType, UserId = @UserId,
                RegisteredByCoordinatorId = @RegisteredByCoordinatorId,
                ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE TeacherId = @TeacherId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO