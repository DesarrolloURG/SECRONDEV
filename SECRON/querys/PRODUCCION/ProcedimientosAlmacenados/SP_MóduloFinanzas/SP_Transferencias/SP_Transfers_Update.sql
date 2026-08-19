CREATE OR ALTER PROCEDURE SP_Transfers_Update
    @TransferId INT, @TransferNumber VARCHAR(20), @IssueDate DATETIME, @IssuePlace VARCHAR(50) = NULL,
    @Amount DECIMAL(18,2), @PrintedAmount DECIMAL(18,2), @BeneficiaryName VARCHAR(150),
    @EmployeeId INT = NULL, @BankId INT, @BankAccountNumber VARCHAR(50) = NULL,
    @BanksAccountTypeId INT, @StatusId INT, @Concept VARCHAR(255), @DetailDescription VARCHAR(500) = NULL,
    @Period VARCHAR(20), @LocationId INT = NULL, @DepartmentId INT = NULL,
    @Exemption DECIMAL(18,2), @TaxFreeAmount DECIMAL(18,2), @FoodAllowance DECIMAL(18,2),
    @IGSS DECIMAL(18,2), @WithholdingTax DECIMAL(18,2), @Retention DECIMAL(18,2),
    @Bonus DECIMAL(18,2), @Discounts DECIMAL(18,2), @Advances DECIMAL(18,2),
    @Viaticos DECIMAL(18,2), @Stamps DECIMAL(18,2), @PurchaseOrderNumber VARCHAR(50) = NULL,
    @Complement VARCHAR(50) = NULL, @Compensation DECIMAL(18,2), @Vacation DECIMAL(18,2),
    @Bill VARCHAR(50) = NULL, @Aguinaldo DECIMAL(18,2), @LastComplement BIT,
    @FileControl VARCHAR(20) = 'PENDIENTE', @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Transfers SET
            TransferNumber = @TransferNumber, IssueDate = @IssueDate, IssuePlace = @IssuePlace,
            Amount = @Amount, PrintedAmount = @PrintedAmount, BeneficiaryName = @BeneficiaryName,
            EmployeeId = @EmployeeId, BankId = @BankId, BankAccountNumber = @BankAccountNumber,
            BanksAccountTypeId = @BanksAccountTypeId, StatusId = @StatusId, Concept = @Concept,
            DetailDescription = @DetailDescription, Period = @Period, LocationId = @LocationId,
            DepartmentId = @DepartmentId, Exemption = @Exemption, TaxFreeAmount = @TaxFreeAmount,
            FoodAllowance = @FoodAllowance, IGSS = @IGSS, WithholdingTax = @WithholdingTax,
            Retention = @Retention, Bonus = @Bonus, Discounts = @Discounts, Advances = @Advances,
            Viaticos = @Viaticos, Stamps = @Stamps, PurchaseOrderNumber = @PurchaseOrderNumber,
            Complement = @Complement, Compensation = @Compensation, Vacation = @Vacation,
            Bill = @Bill, Aguinaldo = @Aguinaldo, LastComplement = @LastComplement,
            FileControl = @FileControl, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
        WHERE TransferId = @TransferId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO