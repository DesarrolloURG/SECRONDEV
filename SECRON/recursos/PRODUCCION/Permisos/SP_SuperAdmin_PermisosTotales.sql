INSERT INTO UserPermissions (UserId, PermissionId, IsGranted, GrantedDate, GrantedBy)
SELECT 
    u.UserId,
    p.PermissionId,
    1 AS IsGranted,
    GETDATE() AS GrantedDate,
    1 AS GrantedBy
FROM Users u
INNER JOIN Roles r 
    ON u.RoleId = r.RoleId
    AND r.RoleName = 'SUPERADMIN'
    and u.Username ='SA'
CROSS JOIN Permissions p
WHERE p.IsActive = 1
  AND NOT EXISTS (
        SELECT 1
        FROM UserPermissions up
        WHERE up.UserId = u.UserId
          AND up.PermissionId = p.PermissionId
    );