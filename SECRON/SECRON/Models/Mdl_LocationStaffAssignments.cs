using System;

public class Mdl_LocationStaffAssignments
{
    public int AssignmentId { get; set; }
    public int LocationId { get; set; }
    public int UserId { get; set; }
    public byte RoleTypeId { get; set; }
    public bool IsActive { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public string EmployeeCode { get; set; }
    public string FullName { get; set; }
    public string InstitutionalEmail { get; set; }
    public string Phone { get; set; }
    public string RoleName { get; set; }
}