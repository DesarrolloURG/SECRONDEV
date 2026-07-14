ALTER   PROCEDURE [dbo].[SP_WarehouseDispatch_Create]
    @WarehouseId            INT,
    @PermissionCode         VARCHAR(50),
    @DestinationWarehouseId INT = NULL,
    @DestinationEmployeeId  INT = NULL,
    @ReferenceDocument      VARCHAR(100) = NULL,
    @Remarks                VARCHAR(500) = NULL,
    @ItemsJson              NVARCHAR(MAX),
    @CreatedBy              INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        -- Si es transferencia a bodega, el origen debe ser la central (regla 5)
        IF @PermissionCode = 'DESPACHO_BODEGA'
        BEGIN
            DECLARE @CentralWarehouseId INT;
            SELECT @CentralWarehouseId = w.WarehouseId
              FROM Warehouses w
              JOIN Locations l ON l.LocationId = w.LocationId
             WHERE l.LocationCode = '1'
               and w.WarehouseId = @WarehouseId;

            IF @CentralWarehouseId IS NULL OR @WarehouseId <> @CentralWarehouseId
            BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END
            
            IF @WarehouseId <> @CentralWarehouseId
            BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

            IF @DestinationWarehouseId IS NULL OR @DestinationWarehouseId = @WarehouseId
            BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END
        END

        DECLARE @WarehouseManagerId INT, @MaxQuantityPerDispatch DECIMAL(18,2);

        SELECT @WarehouseManagerId = wm.WarehouseManagerId,
               @MaxQuantityPerDispatch = wmp.MaxQuantityPerDispatch
          FROM WarehouseManagers wm
          JOIN WarehouseManagerPermissions wmp ON wmp.WarehouseManagerId = wm.WarehouseManagerId
          JOIN WarehousePermissions wp ON wp.WarehousePermissionId = wmp.WarehousePermissionId
         WHERE wm.WarehouseId = @WarehouseId
           AND wm.UserId = @CreatedBy
           AND wm.IsActive = 1
           AND wmp.IsGranted = 1
           AND wp.PermissionCode = @PermissionCode
           AND wp.IsActive = 1;

        IF @WarehouseManagerId IS NULL
        BEGIN ROLLBACK TRANSACTION; SELECT -3; RETURN; END

        IF NOT EXISTS (SELECT 1 FROM OPENJSON(@ItemsJson))
        BEGIN ROLLBACK TRANSACTION; SELECT -4; RETURN; END

        -- El límite de cantidad solo aplica a despacho a empleado, no a transferencia
        IF @PermissionCode = 'DESPACHO_EMPLEADO' AND @MaxQuantityPerDispatch IS NOT NULL
        BEGIN
            IF EXISTS (
                SELECT 1 FROM OPENJSON(@ItemsJson)
                WITH (Quantity DECIMAL(18,2) '$.Quantity')
                WHERE Quantity > @MaxQuantityPerDispatch
            )
            BEGIN ROLLBACK TRANSACTION; SELECT -5; RETURN; END
        END

        DECLARE @MovementTypeCode VARCHAR(20) = CASE WHEN @PermissionCode = 'DESPACHO_BODEGA' THEN 'TRANSFERENCIA' ELSE 'SALIDA' END;
        DECLARE @MovementTypeId INT;
        SELECT @MovementTypeId = MovementTypeId FROM MovementTypes WHERE TypeCode = @MovementTypeCode AND IsActive = 1;

        IF @MovementTypeId IS NULL
        BEGIN ROLLBACK TRANSACTION; SELECT -6; RETURN; END

        DECLARE @Result TABLE (ReturnValue INT);
        INSERT INTO @Result
        EXEC SP_ItemMovement_Insert
            @MovementTypeId = @MovementTypeId,
            @WarehouseId = @WarehouseId,
            @DestinationWarehouseId = @DestinationWarehouseId,
            @ReferenceDocument = @ReferenceDocument,
            @Remarks = @Remarks,
            @ItemsJson = @ItemsJson,
            @CreatedBy = @CreatedBy,
            @DestinationEmployeeId = @DestinationEmployeeId;

        DECLARE @MovementResult INT = (SELECT TOP 1 ReturnValue FROM @Result);

        IF @MovementResult <> 1
        BEGIN ROLLBACK TRANSACTION; SELECT -7; RETURN; END

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
