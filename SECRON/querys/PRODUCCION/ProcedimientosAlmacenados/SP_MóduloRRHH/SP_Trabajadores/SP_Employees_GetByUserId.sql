CREATE OR ALTER PROCEDURE SP_Employees_GetByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT EmployeeId, EmployeeCode, FirstName, LastName, FullName,
        IdentificationNumber, Email, InstitutionalEmail, Phone, MobilePhone, Address,
        BirthDate, HireDate, TerminationDate, DepartmentId, PositionId, DirectSupervisorId,
        EmployeeStatusId, EmergencyContactName, EmergencyContactPhone, EmergencyContactRelation,
        nominal_salary, base_salary, additional_bonus, legal_bonus,
        IGSS, ISR, net_salary, IGSS_MANUAL,
        IsActive, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, LocationId, TipoContratacion,
        FilePath_DPI, FilePath_Titulos, FilePath_RTU, FilePath_Colegiado,
        FilePath_RENAS, FilePath_AntPoliciacos, FilePath_AntPenales,
        FilePath_CV, FilePath_ContratoFirmado, UserId
    FROM Employees
    WHERE UserId = @UserId;
END
GO