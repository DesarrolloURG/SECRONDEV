CREATE OR ALTER PROCEDURE SP_CareerCourses_Delete
    @CareerCourseId INT,
    @IsActive BIT,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsActive = 1 AND EXISTS (
            SELECT 1 FROM CareerCourses
            WHERE CareerId = (SELECT CareerId FROM CareerCourses WHERE CareerCourseId = @CareerCourseId)
              AND CourseId = (SELECT CourseId FROM CareerCourses WHERE CareerCourseId = @CareerCourseId)
              AND CareerCourseId <> @CareerCourseId
        )
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        UPDATE CareerCourses
        SET IsActive = @IsActive,
            ModifiedDate = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE CareerCourseId = @CareerCourseId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO