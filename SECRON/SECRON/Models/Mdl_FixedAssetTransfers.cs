using System;

namespace SECRON.Models
{
    // ─────────────────────────────────────────────
    // MAESTRO — un registro por traslado
    // ─────────────────────────────────────────────
    internal class Mdl_FixedAssetTransfer
    {
        #region Propiedades

        public int TransferId { get; set; }
        public string TransferCode { get; set; }
        public DateTime TransferDate { get; set; }

        // Destino (se define una sola vez en el maestro)
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

        // Display (de V_FixedAssetTransfers)
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string ToWarehouseName { get; set; }
        public string ToEmployeeName { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }

        #endregion

        #region Constructores

        public Mdl_FixedAssetTransfer() { }

        public Mdl_FixedAssetTransfer(
            string transferCode,
            DateTime transferDate,
            int transferStatusId,
            string reason = null,
            int? toWarehouseId = null,
            int? toEmployeeId = null)
        {
            TransferCode = transferCode;
            TransferDate = transferDate;
            TransferStatusId = transferStatusId;
            Reason = reason;
            ToWarehouseId = toWarehouseId;
            ToEmployeeId = toEmployeeId;
        }

        #endregion
    }

    // ─────────────────────────────────────────────
    // DETALLE — un registro por activo en el traslado
    // ─────────────────────────────────────────────
    public class Mdl_FixedAssetTransferDetail
    {
        #region Propiedades

        public int TransferDetailId { get; set; }
        public int TransferId { get; set; }
        public int AssetId { get; set; }

        // Origen (viene del activo, se llena al buscarlo)
        public int? FromWarehouseId { get; set; }
        public int? FromEmployeeId { get; set; }

        // Auditoría
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        // Display (de V_FixedAssetTransferDetails)
        public string AssetCode { get; set; }
        public string AssetName { get; set; }
        public string FromWarehouseName { get; set; }
        public string FromEmployeeName { get; set; }
        public string CreatedByName { get; set; }

        #endregion

        #region Constructores

        public Mdl_FixedAssetTransferDetail() { }

        public Mdl_FixedAssetTransferDetail(
            int transferId,
            int assetId,
            int? fromWarehouseId = null,
            int? fromEmployeeId = null)
        {
            TransferId = transferId;
            AssetId = assetId;
            FromWarehouseId = fromWarehouseId;
            FromEmployeeId = fromEmployeeId;
        }

        #endregion
    }
}