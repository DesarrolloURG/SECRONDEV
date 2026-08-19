CREATE OR ALTER PROCEDURE SP_Checks_Insert
    @CheckNumber VARCHAR(20), @IssueDate DATETIME, @IssuePlace VARCHAR(50) = NULL,
    @Amount DECIMAL(18,2), @PrintedAmount DECIMAL(18,2), @BeneficiaryName VARCHAR(150),
    @EmployeeId INT = NULL, @BankId INT, @BankAccountNumber VARCHAR(50) = NULL,
    @StatusId INT, @Concept VARCHAR(255), @DetailDescription VARCHAR(500) = NULL,
    @Period VARCHAR(20), @LocationId INT = NULL, @DepartmentId INT = NULL,
    @Exemption DECIMAL(18,2), @TaxFreeAmount DECIMAL(18,2), @FoodAllowance DECIMAL(18,2),
    @IGSS DECIMAL(18,2), @WithholdingTax DECIMAL(18,2), @Retention DECIMAL(18,2),
    @Bonus DECIMAL(18,2), @Discounts DECIMAL(18,2), @Advances DECIMAL(18,2),
    @Viaticos DECIMAL(18,2), @Stamps DECIMAL(18,2), @PurchaseOrderNumber VARCHAR(50) = NULL,
    @Complement VARCHAR(50) = NULL, @IsActive BIT, @CreatedBy INT = NULL,
    @Predeclared BIT, @Compensation DECIMAL(18,2), @Vacation DECIMAL(18,2),
    @Bill VARCHAR(50) = NULL, @Aguinaldo DECIMAL(18,2), @LastComplement BIT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Checks (CheckNumber, IssueDate, IssuePlace, Amount, PrintedAmount,
            BeneficiaryName, EmployeeId, BankId, BankAccountNumber, StatusId, Concept, DetailDescription,
            Period, LocationId, DepartmentId, Exemption, TaxFreeAmount, FoodAllowance, IGSS,
            WithholdingTax, Retention, Bonus, Discounts, Advances, Viaticos, Stamps, PurchaseOrderNumber,
            Complement, IsActive, CreatedBy, Predeclared, Compensation, Vacation, Bill, Aguinaldo, LastComplement)
        VALUES (@CheckNumber, @IssueDate, @IssuePlace, @Amount, @PrintedAmount, @BeneficiaryName,
            @EmployeeId, @BankId, @BankAccountNumber, @StatusId, @Concept, @DetailDescription, @Period,
            @LocationId, @DepartmentId, @Exemption, @TaxFreeAmount, @FoodAllowance, @IGSS,
            @WithholdingTax, @Retention, @Bonus, @Discounts, @Advances, @Viaticos, @Stamps,
            @PurchaseOrderNumber, @Complement, @IsActive, @CreatedBy, @Predeclared, @Compensation,
            @Vacation, @Bill, @Aguinaldo, @LastComplement);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO