using System;

namespace SECRON.Models
{
    internal class Mdl_LocationCategory
    {
        public int LocationCategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        public string DisplayText
        {
            get { return $"{CategoryCode} - {CategoryName}"; }
        }
    }
}