// ============================================================
// Mdl_FixedAssetClassificationCategory.cs
// ============================================================
namespace SECRON.Models
{
    public class Mdl_FixedAssetClassificationCategory
    {
        public int ClassificationId { get; set; }
        public string ClassificationCode { get; set; }
        public string ClassificationName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public System.DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}