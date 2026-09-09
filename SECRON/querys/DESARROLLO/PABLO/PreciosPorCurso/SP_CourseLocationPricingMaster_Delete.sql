CREATE OR ALTER PROCEDURE SP_CourseLocationPricingMaster_Delete
    @CourseLocationPricingId INT,
    @IsActive BIT,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    DECLARE @Now DATETIME = GETDATE();

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsActive = 1 AND EXISTS (
            SELECT 1 FROM CourseLocationPricingMaster
            WHERE CareerCourseId = (SELECT CareerCourseId FROM CourseLocationPricingMaster WHERE CourseLocationPricingId = @CourseLocationPricingId)
              AND LocationId = (SELECT LocationId FROM CourseLocationPricingMaster WHERE CourseLocationPricingId = @CourseLocationPricingId)
              AND ModalityId = (SELECT ModalityId FROM CourseLocationPricingMaster WHERE CourseLocationPricingId = @CourseLocationPricingId)
              AND IsActive = 1 AND CourseLocationPricingId <> @CourseLocationPricingId
        )
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        UPDATE CourseLocationPricingMaster
        SET IsActive = @IsActive,
            ModifiedDate = @Now,
            ModifiedBy = @ModifiedBy
        WHERE CourseLocationPricingId = @CourseLocationPricingId;

        IF @IsActive = 0
        BEGIN
            UPDATE CourseLocationPricingDetail
            SET EffectiveTo = @Now,
                ModifiedDate = @Now,
                ModifiedBy = @ModifiedBy
            WHERE CourseLocationPricingId = @CourseLocationPricingId
              AND EffectiveTo IS NULL
              AND IsActive = 1;
        END

        IF @IsActive = 1
        BEGIN
            DECLARE @LastPrice DECIMAL(10,2);
            SELECT TOP 1 @LastPrice = Price
            FROM CourseLocationPricingDetail
            WHERE CourseLocationPricingId = @CourseLocationPricingId
              AND IsActive = 1
            ORDER BY EffectiveFrom DESC;

            IF @LastPrice IS NOT NULL
            BEGIN
                INSERT INTO CourseLocationPricingDetail (CourseLocationPricingId, Price, EffectiveFrom, EffectiveTo, CreatedBy)
                VALUES (@CourseLocationPricingId, @LastPrice, @Now, NULL, @ModifiedBy);
            END
        END

        COMMIT TRANSACTION;
        SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO