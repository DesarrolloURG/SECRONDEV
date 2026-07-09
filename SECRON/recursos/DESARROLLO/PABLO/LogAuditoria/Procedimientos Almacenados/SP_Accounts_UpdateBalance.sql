CREATE OR ALTER PROCEDURE SP_Accounts_UpdateBalance
    @AccountName VARCHAR(150), @Debit DECIMAL(18,2), @Credit DECIMAL(18,2), @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        DECLARE @Sign VARCHAR(5) = (SELECT Sign FROM Accounts WHERE Name = @AccountName);
        SET @Sign = ISNULL(@Sign, '+');

        DECLARE @Monto DECIMAL(18,2);

        IF @Sign = '+'
        BEGIN
            IF @Debit > 0 SET @Monto = @Debit;
            ELSE SET @Monto = -@Credit;
        END
        ELSE
        BEGIN
            IF @Credit > 0 SET @Monto = @Credit;
            ELSE SET @Monto = -@Debit;
        END

        UPDATE Accounts SET Balance = Balance + @Monto WHERE Name = @AccountName;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO