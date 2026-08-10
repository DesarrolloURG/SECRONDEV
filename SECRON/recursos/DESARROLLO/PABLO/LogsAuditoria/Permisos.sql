INSERT INTO Permissions (PermissionCode, PermissionName, Description, ModuleName, ActionType)
SELECT * FROM (VALUES
('ITSM_010', 'ITSM_LOGSAUDIT_READ',           'PERMITE VISUALIZAR LA LISTA Y DETALLES DE LOGS DEL SISTEMA.',  'ITSM', 'READ'),
('ITSM_011', 'ITSM_LOGSAUDIT_EXPORT',         'PERMITE EXPORTAR LISTADO DE LOGS DEL SISTEMA.',                'ITSM', 'EXPORT'),
('ITSM_012', 'ITSM_LOGSAUDIT_TAB',         'PERMITE VISUALIZAR EL MÓDULO DE CONSULTA DE LOGS DEL SISTEMA.',                'ITSM', 'TAM')
) AS nuevos(PermissionCode, PermissionName, Description, ModuleName, ActionType)
WHERE NOT EXISTS (SELECT 1 FROM Permissions p WHERE p.PermissionCode = nuevos.PermissionCode)


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