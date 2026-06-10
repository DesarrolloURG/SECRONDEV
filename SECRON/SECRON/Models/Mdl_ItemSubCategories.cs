using System;

namespace SECRON.Models
{
    internal class Mdl_ItemSubCategories
    {
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string SubCategoryCode { get; set; }
        public string SubCategoryName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public Mdl_ItemSubCategories()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }
    }
}