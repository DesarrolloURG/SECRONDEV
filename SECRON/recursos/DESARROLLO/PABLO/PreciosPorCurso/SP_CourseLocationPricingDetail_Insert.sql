CREATE OR ALTER PROCEDURE SP_CourseLocationPricingDetail_Insert
    @CourseLocationPricingId INT,
    @Price DECIMAL(10,2),
    @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    DECLARE @Now DATETIME = GETDATE();

    BEGIN TRANSACTION
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM CourseLocationPricingMaster WHERE CourseLocationPricingId = @CourseLocationPricingId AND IsActive = 1)
        BEGIN
            ROLLBACK TRANSACTION; SELECT -2; RETURN;
        END

        UPDATE CourseLocationPricingDetail
        SET EffectiveTo = @Now,
            ModifiedDate = @Now,
            ModifiedBy = @CreatedBy
        WHERE CourseLocationPricingId = @CourseLocationPricingId
          AND EffectiveTo IS NULL
          AND IsActive = 1;

        INSERT INTO CourseLocationPricingDetail (CourseLocationPricingId, Price, EffectiveFrom, EffectiveTo, CreatedBy)
        VALUES (@CourseLocationPricingId, @Price, @Now, NULL, @CreatedBy);

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO