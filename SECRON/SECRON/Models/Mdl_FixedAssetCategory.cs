using System;

namespace SECRON.Models
{
    public class Mdl_FixedAssetCategory
    {
        public int AssetCategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string DepreciationMethod { get; set; }
        public decimal DepreciationYears { get; set; }
        public int AccountAccumDepId { get; set; }
        public int AccountExpenseId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsTangible { get; set; }
        public int? ClassificationId { get; set; }
    }
}