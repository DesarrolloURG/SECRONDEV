CREATE OR ALTER PROCEDURE SP_Permissions_SearchByModulePrefix
    @ModulePrefix VARCHAR(50),
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.PermissionId, p.PermissionCode, p.PermissionName, p.Description,
        p.ModuleName, p.ActionType,
        rp.RolePermissionId,
        CASE WHEN rp.RolePermissionId IS NOT NULL AND rp.IsGranted = 1 THEN 1 ELSE 0 END AS EstaAsignado
    FROM Permissions p
    LEFT JOIN RolePermissions rp ON rp.PermissionId = p.PermissionId AND rp.RoleId = @RoleId
    WHERE p.ModuleName LIKE @ModulePrefix + '%'
      AND p.IsActive = 1
    ORDER BY p.ModuleName, p.PermissionName;
END
GO