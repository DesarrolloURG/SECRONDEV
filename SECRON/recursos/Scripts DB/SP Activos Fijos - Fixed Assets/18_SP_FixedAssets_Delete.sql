CREATE OR ALTER PROCEDURE SP_FixedAssets_Delete
    @AssetId    INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM FixedAssets WHERE AssetId = @AssetId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1
            RETURN
        END

        -- Eliminar valores de atributos
        DELETE FROM FixedAssetAttributeValues
        WHERE AssetId = @AssetId

        -- Eliminar registros contables asociados
        delete from AccountingEntryFixedAssetsDetail 
        where EntryMasterId in (select b.EntryMasterId
        from AccountingEntryFixedAssets b
        where b.assetid = @AssetId)

        delete from AccountingEntryFixedAssets
        where assetid = @AssetId

        -- Eliminar el activo
        DELETE FROM FixedAssets
        WHERE AssetId = @AssetId

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO