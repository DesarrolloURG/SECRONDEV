-- ============================================================
-- SPs
-- ============================================================

-- SELECT
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetClassificationCategories_Select]
    @ClassificationCode VARCHAR(20)  = NULL,
    @ClassificationName VARCHAR(100) = NULL,
    @IsActive           BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [ClassificationId], [ClassificationCode], [ClassificationName],
        [Description], [IsActive], [CreatedDate], [CreatedBy],
        [ModifiedDate], [ModifiedBy]
    FROM [dbo].[FixedAssetClassificationCategories]
    WHERE
        (@ClassificationCode IS NULL OR [ClassificationCode] LIKE '%' + UPPER(@ClassificationCode) + '%')
    AND (@ClassificationName IS NULL OR [ClassificationName] LIKE '%' + UPPER(@ClassificationName) + '%')
    AND (@IsActive           IS NULL OR [IsActive] = @IsActive)
    ORDER BY [ClassificationName] ASC;
END
GO

-- INSERT
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetClassificationCategories_Insert]
    @ClassificationCode VARCHAR(20),
    @ClassificationName VARCHAR(100),
    @Description        VARCHAR(255) = NULL,
    @CreatedBy          INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetClassificationCategories]
                   WHERE [ClassificationCode] = UPPER(@ClassificationCode))
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END -- Código duplicado

        INSERT INTO [dbo].[FixedAssetClassificationCategories]
            ([ClassificationCode],[ClassificationName],[Description],[CreatedBy])
        VALUES
            (UPPER(@ClassificationCode), UPPER(@ClassificationName),
             UPPER(@Description), @CreatedBy);

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- UPDATE
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetClassificationCategories_Update]
    @ClassificationId   INT,
    @ClassificationCode VARCHAR(20),
    @ClassificationName VARCHAR(100),
    @Description        VARCHAR(255) = NULL,
    @IsActive           BIT,
    @ModifiedBy         INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetClassificationCategories]
                       WHERE [ClassificationId] = @ClassificationId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetClassificationCategories]
                   WHERE [ClassificationCode] = UPPER(@ClassificationCode)
                     AND [ClassificationId]  <> @ClassificationId)
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END -- Código duplicado

        UPDATE [dbo].[FixedAssetClassificationCategories] SET
            [ClassificationCode] = UPPER(@ClassificationCode),
            [ClassificationName] = UPPER(@ClassificationName),
            [Description]        = UPPER(@Description),
            [IsActive]           = @IsActive,
            [ModifiedDate]       = GETDATE(),
            [ModifiedBy]         = @ModifiedBy
        WHERE [ClassificationId] = @ClassificationId;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- INACTIVE
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetClassificationCategories_Inactive]
    @ClassificationId INT,
    @ModifiedBy       INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetClassificationCategories]
                       WHERE [ClassificationId] = @ClassificationId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        -- No inactivar si tiene categorías activas asignadas
        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetCategories]
                   WHERE [ClassificationId] = @ClassificationId AND [IsActive] = 1)
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END

        UPDATE [dbo].[FixedAssetClassificationCategories] SET
            [IsActive]     = 0,
            [ModifiedDate] = GETDATE(),
            [ModifiedBy]   = @ModifiedBy
        WHERE [ClassificationId] = @ClassificationId;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO