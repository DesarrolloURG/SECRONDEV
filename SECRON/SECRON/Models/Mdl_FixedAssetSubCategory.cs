using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    public class Mdl_FixedAssetSubCategory
    {
        public int SubCategoryId { get; set; }
        public int AssetCategoryId { get; set; }
        public string SubCategoryCode { get; set; }
        public string SubCategoryName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
