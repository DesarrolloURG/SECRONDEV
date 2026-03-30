using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Suppliers
    {
        // Campos principales
        public int SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string LegalName { get; set; }
        public string TaxId { get; set; }

        // Contacto
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        // Información comercial
        public string CommercialActivity { get; set; }
        public string Classification { get; set; }

        // Información bancaria
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }

        // Control
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Constructor vacío
        public Mdl_Suppliers()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        // Constructor con parámetros principales
        public Mdl_Suppliers(string supplierCode, string supplierName, string legalName, string phone,
            string commercialActivity, string classification)
        {
            this.SupplierCode = supplierCode;
            this.SupplierName = supplierName;
            this.LegalName = legalName;
            this.Phone = phone;
            this.CommercialActivity = commercialActivity;
            this.Classification = classification;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }
    }
}