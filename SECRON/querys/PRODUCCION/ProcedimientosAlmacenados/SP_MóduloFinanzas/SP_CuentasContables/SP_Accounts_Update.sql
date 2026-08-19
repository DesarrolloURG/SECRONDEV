CREATE OR ALTER PROCEDURE SP_Accounts_Update
    @Code VARCHAR(20), @Name VARCHAR(150), @Type VARCHAR(50) = NULL,
    @ParentAccountCode VARCHAR(20), @Level INT, @Sign VARCHAR(5),
    @Balance DECIMAL(18,2), @BankCode INT, @BankName VARCHAR(150),
    @BankAccountType VARCHAR(50), @CheckNumber INT, @Currency VARCHAR(10),
    @CurrencyName VARCHAR(50), @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UpdatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Accounts SET Name = @Name, Type = @Type, ParentAccountCode = @ParentAccountCode,
            Level = @Level, Sign = @Sign, Balance = @Balance, BankCode = @BankCode,
            BankName = @BankName, BankAccountType = @BankAccountType, CheckNumber = @CheckNumber,
            Currency = @Currency, CurrencyName = @CurrencyName
        WHERE Code = @Code;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO