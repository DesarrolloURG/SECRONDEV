-- DELETE físico, se mantiene tal cual (comportamiento original)
CREATE OR ALTER PROCEDURE SP_ItemStockTemplates_Delete
    @TemplateId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        DELETE FROM ItemStockTemplates WHERE TemplateId = @TemplateId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO