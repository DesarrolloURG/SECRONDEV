CREATE OR ALTER PROCEDURE SP_Suppliers_Delete
    @SupplierId INT,
    @IsActive   BIT,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE SupplierId = @SupplierId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        UPDATE Suppliers SET
            IsActive     = @IsActive,
            ModifiedDate = GETDATE(),
            ModifiedBy   = @ModifiedBy
        WHERE SupplierId = @SupplierId

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END