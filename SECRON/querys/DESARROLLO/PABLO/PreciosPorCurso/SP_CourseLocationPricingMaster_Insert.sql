CREATE OR ALTER PROCEDURE SP_CourseLocationPricingMaster_Insert
    @CareerCourseId INT,
    @LocationId INT,
    @ModalityId INT,
    @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM CourseLocationPricingMaster
            WHERE CareerCourseId = @CareerCourseId AND LocationId = @LocationId AND ModalityId = @ModalityId AND IsActive = 1
        )
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        INSERT INTO CourseLocationPricingMaster (CareerCourseId, LocationId, ModalityId, CreatedBy)
        VALUES (@CareerCourseId, @LocationId, @ModalityId, @CreatedBy);

        DECLARE @NewId INT = SCOPE_IDENTITY();
        COMMIT TRANSACTION; SELECT @NewId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO