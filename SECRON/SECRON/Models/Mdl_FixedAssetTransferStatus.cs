using System;

namespace SECRON.Models
{
    internal class Mdl_FixedAssetTransferStatus
    {
        #region Propiedades

        public int TransferStatusId { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public bool IsFinal { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        #endregion

        #region Constructores

        // Constructor vacío (data object / binding)
        public Mdl_FixedAssetTransferStatus() { }

        // Constructor con parámetros principales
        public Mdl_FixedAssetTransferStatus(
            int transferStatusId,
            string statusCode,
            string statusName,
            string description,
            int order,
            bool isFinal,
            bool isActive)
        {
            TransferStatusId = transferStatusId;
            StatusCode = statusCode;
            StatusName = statusName;
            Description = description;
            Order = order;
            IsFinal = isFinal;
            IsActive = isActive;
        }

        #endregion
    }
}