CREATE OR ALTER PROCEDURE SP_CourseLocationPricingDetail_Update
    @CourseLocationPricingDetailId INT,
    @Price DECIMAL(10,2),
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 FROM CourseLocationPricingDetail
            WHERE CourseLocationPricingDetailId = @CourseLocationPricingDetailId
              AND EffectiveTo IS NULL AND IsActive = 1
        )
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        UPDATE CourseLocationPricingDetail
        SET Price = @Price,
            ModifiedDate = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE CourseLocationPricingDetailId = @CourseLocationPricingDetailId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO