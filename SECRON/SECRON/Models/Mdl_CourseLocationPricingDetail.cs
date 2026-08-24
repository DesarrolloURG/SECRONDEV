using System;

namespace SECRON.Models
{
    public class Mdl_CourseLocationPricingDetail
    {
        public int CourseLocationPricingDetailId { get; set; }
        public int CourseLocationPricingId { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}