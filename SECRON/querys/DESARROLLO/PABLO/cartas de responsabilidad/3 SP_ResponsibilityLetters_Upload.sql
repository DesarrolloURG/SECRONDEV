CREATE OR ALTER PROCEDURE [dbo].[SP_ResponsibilityLetters_Upload]
    @AssetId            INT,
    @EmployeeId          INT,
    @FilePath            NVARCHAR(400),
    @FileName            NVARCHAR(200),
    @UploadedByUserId    INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM FixedAssets WHERE AssetId = @AssetId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1
            RETURN
        END

        UPDATE ResponsibilityLetterDetail
        SET IsCurrent = 0
        WHERE AssetId = @AssetId
          AND IsCurrent = 1

        DECLARE @ResponsibilityLetterId INT

        INSERT INTO ResponsibilityLetterMaster (EmployeeId, FilePath, FileName, UploadedByUserId)
        VALUES (@EmployeeId, @FilePath, @FileName, @UploadedByUserId)

        SET @ResponsibilityLetterId = SCOPE_IDENTITY()

        INSERT INTO ResponsibilityLetterDetail (ResponsibilityLetterId, AssetId, IsCurrent)
        VALUES (@ResponsibilityLetterId, @AssetId, 1)

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END