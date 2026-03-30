using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Country
    {
        // Campos principales
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public bool IsActive { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Constructor vacío
        public Mdl_Country()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        public Mdl_Country(string countryName, string countryCode)
        {
            this.CountryName = countryName;
            this.CountryCode = countryCode;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}