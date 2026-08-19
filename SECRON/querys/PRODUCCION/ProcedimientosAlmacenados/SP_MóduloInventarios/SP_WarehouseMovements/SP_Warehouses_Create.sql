CREATE PROCEDURE SP_Warehouses_Create
    @LocationId     INT,
    @WarehouseName  VARCHAR(100),
    @Description    VARCHAR(255) = NULL,
    @Address        VARCHAR(255) = NULL,
    @PhoneNumber    VARCHAR(20) = NULL,
    @WarehouseType  VARCHAR(50),
    @CreatedBy      INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRY
        DECLARE @LocationCode VARCHAR(10);
        SELECT @LocationCode = LocationCode FROM Locations WHERE LocationId = @LocationId;

        IF @LocationCode IS NULL
            RETURN -1;

        BEGIN TRANSACTION;

        DECLARE @Correlativo INT;

        SELECT @Correlativo = ISNULL(MAX(TRY_CAST(RIGHT(WarehouseCode, 2) AS INT)), 0) + 1
        FROM Warehouses WITH (UPDLOCK, HOLDLOCK)
        WHERE LocationId = @LocationId;

        DECLARE @WarehouseCode VARCHAR(20) =
            'WH-' + RIGHT('000' + @LocationCode, 3) + '-' + RIGHT('00' + CAST(@Correlativo AS VARCHAR(2)), 2);

        INSERT INTO Warehouses
            (WarehouseCode, WarehouseName, Description, Address, PhoneNumber, WarehouseType,
             LocationId, IsActive, CreatedBy, CreatedDate)
        VALUES
            (@WarehouseCode, @WarehouseName, @Description, @Address, @PhoneNumber, @WarehouseType,
             @LocationId, 1, @CreatedBy, GETDATE());

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
