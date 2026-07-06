CREATE OR ALTER PROCEDURE SP_Accounts_Insert
    @Code VARCHAR(20), @Name VARCHAR(150), @Type VARCHAR(50) = NULL,
    @ParentAccountCode VARCHAR(20), @Level INT, @Sign VARCHAR(5),
    @Balance DECIMAL(18,2), @BankCode INT, @BankName VARCHAR(150),
    @BankAccountType VARCHAR(50), @CheckNumber INT, @Currency VARCHAR(10),
    @CurrencyName VARCHAR(50), @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Accounts (Code, Name, Type, ParentAccountCode, Level, Sign, Balance,
            BankCode, BankName, BankAccountType, CheckNumber, Currency, CurrencyName)
        VALUES (@Code, @Name, @Type, @ParentAccountCode, @Level, @Sign, @Balance,
            @BankCode, @BankName, @BankAccountType, @CheckNumber, @Currency, @CurrencyName);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO