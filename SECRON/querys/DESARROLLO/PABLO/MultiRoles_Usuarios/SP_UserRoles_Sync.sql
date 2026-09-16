CREATE OR ALTER PROCEDURE SP_UserRoles_Sync
    @UserIds NVARCHAR(MAX), -- JSON array de UserId, ej '[1,2,3]'
    @RoleIds NVARCHAR(MAX), -- JSON array del rol deseado (se aplica IGUAL a todos los usuarios), ej '[5,7]'
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    -- Si el panel está vacío, dejaría a todos los usuarios sin ningún rol: no se ejecuta nada.
    IF (SELECT COUNT(*) FROM OPENJSON(@RoleIds)) < 1
    BEGIN
        SELECT -1;
        RETURN;
    END

    BEGIN TRANSACTION
    BEGIN TRY
        DECLARE @Usuarios TABLE (UserId INT PRIMARY KEY);
        INSERT INTO @Usuarios SELECT CAST(value AS INT) FROM OPENJSON(@UserIds);

        DECLARE @RolesDeseados TABLE (RoleId INT PRIMARY KEY);
        INSERT INTO @RolesDeseados SELECT CAST(value AS INT) FROM OPENJSON(@RoleIds);

        -- Reactivar/insertar los roles deseados que falten
        MERGE UserRoles AS target
        USING (
            SELECT u.UserId, r.RoleId
            FROM @Usuarios u
            CROSS JOIN @RolesDeseados r
        ) AS source
            ON target.UserId = source.UserId AND target.RoleId = source.RoleId
        WHEN MATCHED AND target.IsActive = 0 THEN
            UPDATE SET IsActive = 1, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (UserId, RoleId, IsActive, CreatedDate, CreatedBy)
            VALUES (source.UserId, source.RoleId, 1, GETDATE(), @ModifiedBy);

        -- Desactivar los roles activos que YA NO estén en la lista deseada (solo de los usuarios seleccionados)
        UPDATE ur
        SET IsActive = 0, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
        FROM UserRoles ur
        INNER JOIN @Usuarios u ON u.UserId = ur.UserId
        WHERE ur.IsActive = 1
          AND ur.RoleId NOT IN (SELECT RoleId FROM @RolesDeseados);

        COMMIT TRANSACTION;
        SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0;
    END CATCH
END