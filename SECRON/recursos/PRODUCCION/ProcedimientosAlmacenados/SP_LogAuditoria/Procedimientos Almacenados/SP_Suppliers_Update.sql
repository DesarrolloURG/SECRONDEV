-- @IsInactivation = 1 => solo IsActive=0 (InactivarProveedor)
-- @IsInactivation = 0 => update normal (ActualizarProveedor)
CREATE OR ALTER PROCEDURE SP_Suppliers_Update
    @SupplierId INT, @IsInactivation BIT,
    @SupplierCode VARCHAR(20) = NULL, @SupplierName VARCHAR(150) = NULL, @LegalName VARCHAR(150) = NULL,
    @TaxId VARCHAR(20) = NULL, @ContactName VARCHAR(150) = NULL, @Phone VARCHAR(20) = NULL,
    @Phone2 VARCHAR(20) = NULL, @Email VARCHAR(150) = NULL, @Address VARCHAR(255) = NULL,
    @CommercialActivity VARCHAR(150) = NULL, @Classification VARCHAR(50) = NULL,
    @BankAccountNumber VARCHAR(50) = NULL, @BankName VARCHAR(150) = NULL, @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsInactivation = 1
            UPDATE Suppliers SET IsActive = 0, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE SupplierId = @SupplierId;
        ELSE
            UPDATE Suppliers SET SupplierCode = @SupplierCode, SupplierName = @SupplierName,
                LegalName = @LegalName, TaxId = @TaxId, ContactName = @ContactName, Phone = @Phone,
                Phone2 = @Phone2, Email = @Email, Address = @Address,
                CommercialActivity = @CommercialActivity, Classification = @Classification,
                BankAccountNumber = @BankAccountNumber, BankName = @BankName,
                ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE SupplierId = @SupplierId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO