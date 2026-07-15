CREATE OR ALTER PROCEDURE SP_Suppliers_Insert
    @SupplierCode VARCHAR(20), @SupplierName VARCHAR(150), @LegalName VARCHAR(150),
    @TaxId VARCHAR(20) = NULL, @ContactName VARCHAR(150) = NULL, @Phone VARCHAR(20),
    @Phone2 VARCHAR(20) = NULL, @Email VARCHAR(150) = NULL, @Address VARCHAR(255) = NULL,
    @CommercialActivity VARCHAR(150), @Classification VARCHAR(50),
    @BankAccountNumber VARCHAR(50) = NULL, @BankName VARCHAR(150) = NULL,
    @IsActive BIT, @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Suppliers (SupplierCode, SupplierName, LegalName, TaxId, ContactName, Phone,
            Phone2, Email, Address, CommercialActivity, Classification, BankAccountNumber,
            BankName, IsActive, CreatedBy)
        VALUES (@SupplierCode, @SupplierName, @LegalName, @TaxId, @ContactName, @Phone,
            @Phone2, @Email, @Address, @CommercialActivity, @Classification, @BankAccountNumber,
            @BankName, @IsActive, @CreatedBy);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO