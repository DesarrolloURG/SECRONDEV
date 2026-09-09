CREATE OR ALTER PROCEDURE SP_CourseLocationPricingMaster_Update
    @CourseLocationPricingId INT,
    @CareerCourseId INT,
    @LocationId INT,
    @ModalityId INT,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM CourseLocationPricingMaster
            WHERE CareerCourseId = @CareerCourseId AND LocationId = @LocationId AND ModalityId = @ModalityId
              AND IsActive = 1 AND CourseLocationPricingId <> @CourseLocationPricingId
        )
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        UPDATE CourseLocationPricingMaster
        SET CareerCourseId = @CareerCourseId,
            LocationId = @LocationId,
            ModalityId = @ModalityId,
            ModifiedDate = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE CourseLocationPricingId = @CourseLocationPricingId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO