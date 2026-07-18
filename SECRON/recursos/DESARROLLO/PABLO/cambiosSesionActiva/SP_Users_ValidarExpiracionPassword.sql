CREATE OR ALTER PROCEDURE SP_Users_ValidarExpiracionPassword
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @DiasVida INT;
    DECLARE @LastPasswordChanged DATETIME;
    DECLARE @PasswordNeverExpires BIT;
    DECLARE @DiasTranscurridos INT;

    SELECT @DiasVida = CAST(ParameterValue AS INT)
    FROM ParametersConfiguration
    WHERE ParameterName = 'DiasVidaContrasena';

    SELECT
        @LastPasswordChanged = LastPasswordChanged,
        @PasswordNeverExpires = PasswordNeverExpires
    FROM Users
    WHERE UserId = @UserId;

    IF @PasswordNeverExpires = 1
    BEGIN
        SELECT CAST(0 AS BIT) AS PasswordExpirada, 0 AS DiasRestantes;
        RETURN;
    END

    SET @DiasTranscurridos = DATEDIFF(DAY, @LastPasswordChanged, GETDATE());

    IF @DiasTranscurridos >= @DiasVida
        SELECT CAST(1 AS BIT) AS PasswordExpirada, 0 AS DiasRestantes;
    ELSE
        SELECT CAST(0 AS BIT) AS PasswordExpirada, (@DiasVida - @DiasTranscurridos) AS DiasRestantes;
END