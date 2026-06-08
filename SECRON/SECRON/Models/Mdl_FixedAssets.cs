using System;

namespace SECRON.Models
{
    public class Mdl_FixedAsset
    {
        public int AssetId { get; set; }
        public string AssetCode { get; set; }
        public string AssetName { get; set; }
        public string Description { get; set; }
        public int AssetCategoryId { get; set; }
        public int SubCategoryId { get; set; }

        public string Brand { get; set; }
        public string Model { get; set; }
        public string Serial { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public decimal PurchaseValue { get; set; }
        public decimal ResidualValue { get; set; }
        public string InvoiceNumber { get; set; }
        public int? SupplierId { get; set; }

        public string WarrantyDocumentPath { get; set; }
        public DateTime? WarrantyExpirationDate { get; set; }

        public DateTime? DepreciationStartDate { get; set; }
        public decimal ResidualValueAct { get; set; }

        public int? CurrentWarehouseId { get; set; }
        public int? AssignedToEmployeeId { get; set; }

        public string AssetStatus { get; set; }
        public DateTime? DisposalDate { get; set; }
        public string DisposalReason { get; set; }
        public decimal? DisposalValue { get; set; }
        public string Notes { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string SupplierName { get; set; }
        public string WarehouseName { get; set; }
        public string EmployeeName { get; set; }

        public Mdl_FixedAsset()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
            AssetStatus = "ACTIVO";
            ResidualValue = 0;
            ResidualValueAct = 0;
        }
    }
}