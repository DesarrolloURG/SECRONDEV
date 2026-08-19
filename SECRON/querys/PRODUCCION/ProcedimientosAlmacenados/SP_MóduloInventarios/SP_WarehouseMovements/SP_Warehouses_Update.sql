CREATE PROCEDURE SP_Warehouses_Update
    @WarehouseId        INT,
    @WarehouseName       VARCHAR(100),
    @Description         VARCHAR(255) = NULL,
    @Address             VARCHAR(255) = NULL,
    @PhoneNumber         VARCHAR(20) = NULL,
    @WarehouseType       VARCHAR(50),
    @IsInactivation      BIT,
    @ModifiedBy          INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @IsInactivation = 1
        BEGIN
            UPDATE Warehouses
            SET IsActive = 0,
                ModifiedBy = @ModifiedBy,
                ModifiedDate = GETDATE()
            WHERE WarehouseId = @WarehouseId;
        END
        ELSE
        BEGIN
            UPDATE Warehouses
            SET WarehouseName = @WarehouseName,
                Description = @Description,
                Address = @Address,
                PhoneNumber = @PhoneNumber,
                WarehouseType = @WarehouseType,
                ModifiedBy = @ModifiedBy,
                ModifiedDate = GETDATE()
            WHERE WarehouseId = @WarehouseId;
        END

        COMMIT TRANSACTION;
        RETURN 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        RETURN 0;
    END CATCH
END
GO