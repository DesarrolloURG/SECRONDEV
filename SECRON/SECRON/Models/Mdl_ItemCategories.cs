using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_ItemCategories
    {
        // Campos principales
        public int CategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        // Constructor vacío
        public Mdl_ItemCategories()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros
        public Mdl_ItemCategories(string categoryCode, string categoryName)
        {
            this.CategoryCode = categoryCode;
            this.CategoryName = categoryName;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}