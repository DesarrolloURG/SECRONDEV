namespace SECRON.Models
{
    public class Mdl_WarehouseManagerPermission
    {
        public int WarehouseManagerPermissionId { get; set; }
        public int WarehouseManagerId { get; set; }
        public int WarehousePermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public decimal? MaxQuantityPerDispatch { get; set; }
        public bool IsGranted { get; set; }
    }
}