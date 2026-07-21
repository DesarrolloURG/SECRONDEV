-- =============================================
-- SP_WarehouseManager_Insert
-- =============================================
CREATE OR ALTER PROCEDURE SP_WarehouseManager_Insert
    @WarehouseId INT,
    @UserId      INT,
    @CreatedBy   INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        -- Validar que no exista ya una asignación activa para esta combinación usuario+bodega
        IF EXISTS (
            SELECT 1 FROM WarehouseManagers
            WHERE WarehouseId = @WarehouseId AND UserId = @UserId AND IsActive = 1
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -1;
            RETURN;
        END

        -- Si existía una fila inactiva previa para esta misma combinación, la reactivamos en vez de duplicar
        IF EXISTS (
            SELECT 1 FROM WarehouseManagers
            WHERE WarehouseId = @WarehouseId AND UserId = @UserId AND IsActive = 0
        )
        BEGIN
            UPDATE WarehouseManagers
               SET IsActive = 1,
                   ModifiedDate = GETDATE(),
                   ModifiedBy = @CreatedBy
             WHERE WarehouseId = @WarehouseId AND UserId = @UserId;

            COMMIT TRANSACTION;
            SELECT 1;
            RETURN;
        END

        INSERT INTO WarehouseManagers (WarehouseId, UserId, IsActive, CreatedDate, CreatedBy)
        VALUES (@WarehouseId, @UserId, 1, GETDATE(), @CreatedBy);

        COMMIT TRANSACTION;
        SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0;
    END CATCH
END
GO