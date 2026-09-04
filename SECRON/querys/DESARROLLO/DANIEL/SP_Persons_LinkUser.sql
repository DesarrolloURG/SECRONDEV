-- =====================================================
-- SP CONSOLIDADA: SP_Persons_LinkUser
-- Vincula/desvincula un Usuario a una persona (Docente/Trabajador/Proveedor/Coordinador)
-- Todo en una sola transacción: garantiza que nunca quede en estado intermedio
-- =====================================================
CREATE OR ALTER PROCEDURE SP_Persons_LinkUser
    @UserId INT,
    @PersonType NVARCHAR(20) = NULL,   -- NULL = solo desvincular de todas las tablas
    @PersonId INT = NULL,
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @PersonType IS NULL OR @PersonType <> 'DOCENTE'
            UPDATE Teachers SET UserId = NULL, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE UserId = @UserId;

        IF @PersonType IS NULL OR @PersonType <> 'TRABAJADOR'
            UPDATE Employees SET UserId = NULL, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE UserId = @UserId;

        IF @PersonType IS NULL OR @PersonType <> 'PROVEEDOR'
            UPDATE Suppliers SET UserId = NULL, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE UserId = @UserId;

        IF @PersonType IS NULL OR @PersonType <> 'COORDINADOR'
            UPDATE Coordinators SET UserId = NULL, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE UserId = @UserId;

        IF @PersonType = 'DOCENTE'
            UPDATE Teachers SET UserId = @UserId, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE TeacherId = @PersonId;
        ELSE IF @PersonType = 'TRABAJADOR'
            UPDATE Employees SET UserId = @UserId, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE EmployeeId = @PersonId;
        ELSE IF @PersonType = 'PROVEEDOR'
            UPDATE Suppliers SET UserId = @UserId, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE SupplierId = @PersonId;
        ELSE IF @PersonType = 'COORDINADOR'
            UPDATE Coordinators SET UserId = @UserId, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy WHERE CoordinatorId = @PersonId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- =====================================================
-- ELIMINACIÓN de las 4 SPs individuales (ya reemplazadas por la de arriba)
-- =====================================================
DROP PROCEDURE IF EXISTS SP_Teachers_LinkUser;
DROP PROCEDURE IF EXISTS SP_Employees_LinkUser;
DROP PROCEDURE IF EXISTS SP_Suppliers_LinkUser;
DROP PROCEDURE IF EXISTS SP_Coordinators_LinkUser;
GO