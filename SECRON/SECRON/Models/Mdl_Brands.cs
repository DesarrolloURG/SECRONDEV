using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Brands
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }

        public Mdl_Brands()
        {
            IsActive = true;
        }

        public Mdl_Brands(string brandName, string category)
        {
            this.BrandName = brandName;
            this.Category = category;
            this.IsActive = true;
        }
    }
}