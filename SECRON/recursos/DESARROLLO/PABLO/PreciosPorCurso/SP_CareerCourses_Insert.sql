CREATE OR ALTER PROCEDURE SP_CareerCourses_Insert
    @CareerId INT,
    @CourseId INT,
    @Semester INT,
    @IsRequired BIT,
    @Prerequisites NVARCHAR(500) = NULL,
    @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM CareerCourses WHERE CareerId = @CareerId AND CourseId = @CourseId)
        BEGIN
            ROLLBACK TRANSACTION; SELECT -1; RETURN;
        END

        INSERT INTO CareerCourses (CareerId, CourseId, Semester, IsRequired, Prerequisites, CreatedBy)
        VALUES (@CareerId, @CourseId, @Semester, @IsRequired, @Prerequisites, @CreatedBy);

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO