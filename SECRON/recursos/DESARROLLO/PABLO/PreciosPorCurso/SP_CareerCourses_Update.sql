CREATE OR ALTER PROCEDURE SP_CareerCourses_Update
    @CareerCourseId INT,
    @CareerId INT,
    @CourseId INT,
    @Semester INT,
    @IsRequired BIT,
    @Prerequisites NVARCHAR(500) = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM CareerCourses
            WHERE CareerId = @CareerId AND CourseId = @CourseId AND CareerCourseId <> @CareerCourseId
        )
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        UPDATE CareerCourses
        SET CareerId = @CareerId,
            CourseId = @CourseId,
            Semester = @Semester,
            IsRequired = @IsRequired,
            Prerequisites = @Prerequisites,
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