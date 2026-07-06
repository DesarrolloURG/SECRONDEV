-- @IsInactivation = 1 => solo IsActive=0 (InactivarCategoria)
-- @IsInactivation = 0 => update normal (ActualizarCategoria)
-- Ninguno de los métodos originales recibe usuario, por eso no lleva @ctx
CREATE OR ALTER PROCEDURE SP_LocationCategories_Update
    @LocationCategoryId INT, @IsInactivation BIT,
    @CategoryCode VARCHAR(20) = NULL, @CategoryName VARCHAR(150) = NULL, @Description VARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsInactivation = 1
            UPDATE LocationCategories SET IsActive = 0 WHERE LocationCategoryId = @LocationCategoryId;
        ELSE
            UPDATE LocationCategories SET CategoryCode = @CategoryCode, CategoryName = @CategoryName,
                Description = @Description
            WHERE LocationCategoryId = @LocationCategoryId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO