INSERT INTO Permissions (PermissionCode, PermissionName, Description, ModuleName, ActionType)
SELECT * FROM (VALUES 
('KDX_049', 'KARDEX_INVENTORY_TEMPLATE', 'PERMISO PARA UTILIZAR LA PLANTILLA DE BODEGAS AL USUARIO ENCARGADO DE INVENTARIOS.', 'KARDEX', 'UPDATE')
) AS nuevos(PermissionCode, PermissionName, Description, ModuleName, ActionType)
WHERE NOT EXISTS (SELECT 1 FROM Permissions p WHERE p.PermissionCode = nuevos.PermissionCode)