CREATE OR ALTER PROCEDURE [dbo].[SP_Auth_CargaInicialUsuario]
    @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Result Set 1: Datos del usuario
    SELECT u.UserId, u.Username, u.FullName, u.RoleId, u.StatusId,
           u.IsTemporaryPassword, u.PasswordExpiryDate, u.InstitutionalEmail,
           u.LastLoginDate, u.CreatedDate, u.NotificationsEnabled,
           ISNULL(r.RoleName, '') AS RoleName,
           ISNULL(s.StatusName, '') AS StatusName,
           u.LastPasswordChanged, u.PasswordNeverExpires,
           u.TwoFactorSecret, u.TwoFactorEnabledDate, u.TwoFactorExempt
    FROM Users u
    LEFT JOIN Roles r ON u.RoleId = r.RoleId
    LEFT JOIN UserStatus s ON u.StatusId = s.StatusId
    WHERE u.Username = @Username;

    DECLARE @UserId INT = (SELECT UserId FROM Users WHERE Username = @Username);
    DECLARE @RoleId INT = (SELECT RoleId FROM Users WHERE Username = @Username);

    -- Result Set 2: Permisos efectivos (Rol + Específicos)
    SELECT DISTINCT p.PermissionName
    FROM RolePermissions rp
    INNER JOIN Permissions p ON rp.PermissionId = p.PermissionId
    WHERE rp.RoleId = @RoleId AND rp.IsGranted = 1 AND p.IsActive = 1
      AND p.PermissionId NOT IN (SELECT PermissionId FROM UserPermissions WHERE UserId = @UserId)
    UNION
    SELECT p.PermissionName
    FROM UserPermissions up
    INNER JOIN Permissions p ON up.PermissionId = p.PermissionId
    WHERE up.UserId = @UserId AND up.IsGranted = 1 AND p.IsActive = 1
    ORDER BY PermissionName;

    -- Result Set 3: Parámetro de sesión
    SELECT ParameterValue FROM ParametersConfiguration WHERE ParameterName = 'TiempoSesionActivaMinutos';

    -- Result Set 4: Configuración SMTP
    SELECT ParameterName, ParameterValue FROM ParametersConfiguration
    WHERE ParameterName IN ('SmtpServer','SmtpPort','SmtpUser','SmtpPasswordEncrypted','SmtpEnableSsl');
END