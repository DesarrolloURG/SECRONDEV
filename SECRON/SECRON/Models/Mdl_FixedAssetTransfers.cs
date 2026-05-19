using System;

namespace SECRON.Models
{
    internal class Mdl_FixedAssetTransfer
    {
        #region Propiedades

        public int TransferId { get; set; }
        public string TransferCode { get; set; }
        public int AssetId { get; set; }
        public DateTime TransferDate { get; set; }

        // Origen
        public int? FromWarehouseId { get; set; }
        public int? FromEmployeeId { get; set; }

        // Destino
        public int? ToWarehouseId { get; set; }
        public int? ToEmployeeId { get; set; }

        // Control
        public int TransferStatusId { get; set; }
        public string Reason { get; set; }
        public int? ApprovedByUserId { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        // Auditoría
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        // Campos de display (de la View V_FixedAssetMovements)
        public string AssetCode { get; set; }
        public string AssetName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string FromWarehouseName { get; set; }
        public string FromEmployeeName { get; set; }
        public string ToWarehouseName { get; set; }
        public string ToEmployeeName { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }

        #endregion

        #region Constructores

        public Mdl_FixedAssetTransfer() { }

        public Mdl_FixedAssetTransfer(
            string transferCode,
            int assetId,
            DateTime transferDate,
            int transferStatusId,
            string reason = null,
            int? fromWarehouseId = null,
            int? fromEmployeeId = null,
            int? toWarehouseId = null,
            int? toEmployeeId = null)
        {
            TransferCode = transferCode;
            AssetId = assetId;
            TransferDate = transferDate;
            TransferStatusId = transferStatusId;
            Reason = reason;
            FromWarehouseId = fromWarehouseId;
            FromEmployeeId = fromEmployeeId;
            ToWarehouseId = toWarehouseId;
            ToEmployeeId = toEmployeeId;
        }

        #endregion
    }
}