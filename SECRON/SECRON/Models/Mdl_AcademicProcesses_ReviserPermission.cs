namespace SECRON.Models
{
    internal class Mdl_AcademicProcesses_ReviserPermission
    {
        public int PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }
        public string ModuleName { get; set; }
        public string ActionType { get; set; }

        public int? RolePermissionId { get; set; }
        public bool EstaAsignado { get; set; }
    }
}