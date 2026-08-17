CREATE PROCEDURE [dbo].[SP_ResponsibilityLetters_Unlink]
    @AssetId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    UPDATE ResponsibilityLetterDetail
    SET IsCurrent = 0
    WHERE AssetId = @AssetId
      AND IsCurrent = 1

    SELECT 1
END