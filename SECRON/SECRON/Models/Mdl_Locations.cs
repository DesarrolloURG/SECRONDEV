using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Locations
    {
        // Campos principales
        public int LocationId { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }   // Se mantiene temporalmente
        public bool IsActive { get; set; }

        public int? LocationCategoryId { get; set; }
        public int? PrimaryWarehouseId { get; set; }

        public int? MunicipalityId { get; set; }

        // Campos de apoyo para consultas con JOIN
        public string CountryName { get; set; }
        public string DepartmentName { get; set; }
        public string MunicipalityName { get; set; }

        // Auditoría
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Constructor vacío
        public Mdl_Locations()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_Locations(string locationCode, string locationName, string address, string city)
        {
            this.LocationCode = locationCode;
            this.LocationName = locationName;
            this.Address = address;
            this.City = city;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}