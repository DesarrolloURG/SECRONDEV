CREATE OR ALTER PROCEDURE SP_Auth_CargaInicialUsuario
    @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Result Set 1: Datos del usuario (ya sin RoleId/RoleName)
    SELECT u.UserId, u.Username, u.FullName, u.StatusId, 
           u.IsTemporaryPassword, u.PasswordExpiryDate, u.InstitutionalEmail,
           u.EmployeeId, u.LastLoginDate, u.CreatedDate, u.NotificationsEnabled,
           ISNULL(s.StatusName, '') AS StatusName,
           u.LastPasswordChanged, u.PasswordNeverExpires,
           u.TwoFactorSecret, u.TwoFactorEnabledDate, u.TwoFactorExempt
    FROM Users u
    LEFT JOIN UserStatus s ON u.StatusId = s.StatusId
    WHERE u.Username = @Username;

    DECLARE @UserId INT = (SELECT UserId FROM Users WHERE Username = @Username);

    -- Result Set 2 (NUEVO): Roles activos del usuario
    SELECT r.RoleId, r.RoleName
    FROM UserRoles ur
    INNER JOIN Roles r ON r.RoleId = ur.RoleId
    WHERE ur.UserId = @UserId AND ur.IsActive = 1
    ORDER BY r.RoleName;

    -- Result Set 3: Permisos efectivos (todos los roles activos + Específicos)
    SELECT DISTINCT p.PermissionName
    FROM RolePermissions rp
    INNER JOIN Permissions p ON rp.PermissionId = p.PermissionId
    WHERE rp.RoleId IN (SELECT RoleId FROM UserRoles WHERE UserId = @UserId AND IsActive = 1)
      AND rp.IsGranted = 1 AND p.IsActive = 1
      AND p.PermissionId NOT IN (SELECT PermissionId FROM UserPermissions WHERE UserId = @UserId)
    UNION
    SELECT p.PermissionName
    FROM UserPermissions up
    INNER JOIN Permissions p ON up.PermissionId = p.PermissionId
    WHERE up.UserId = @UserId AND up.IsGranted = 1 AND p.IsActive = 1
    ORDER BY PermissionName;

    -- Result Set 4: Parámetro de sesión
    SELECT ParameterValue FROM ParametersConfiguration WHERE ParameterName = 'TiempoSesionActivaMinutos';

    -- Result Set 5: Configuración SMTP
    SELECT ParameterName, ParameterValue FROM ParametersConfiguration 
    WHERE ParameterName IN ('SmtpServer','SmtpPort','SmtpUser','SmtpPasswordEncrypted','SmtpEnableSsl');
END