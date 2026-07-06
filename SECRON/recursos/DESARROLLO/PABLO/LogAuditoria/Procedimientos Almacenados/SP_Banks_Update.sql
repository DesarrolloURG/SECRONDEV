-- @IsInactivation = 1 => solo IsActive=0 (InactivarBanco)
-- @IsInactivation = 0 => update normal de BankCode/BankName (ActualizarBanco)
CREATE OR ALTER PROCEDURE SP_Banks_Update
    @BankId INT, @IsInactivation BIT,
    @BankCode VARCHAR(20) = NULL, @BankName VARCHAR(150) = NULL,
    @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UpdatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsInactivation = 1
            UPDATE Banks SET IsActive = 0 WHERE BankId = @BankId;
        ELSE
            UPDATE Banks SET BankCode = @BankCode, BankName = @BankName WHERE BankId = @BankId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO