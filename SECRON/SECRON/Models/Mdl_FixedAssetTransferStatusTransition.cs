using System;

namespace SECRON.Models
{
    internal class Mdl_FixedAssetTransferStatusTransition
    {
        #region Propiedades

        public int TransitionId { get; set; }
        public int FromStatusId { get; set; }
        public int ToStatusId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        // Campos de display para grids
        public string FromStatusCode { get; set; }
        public string FromStatusName { get; set; }
        public string ToStatusCode { get; set; }
        public string ToStatusName { get; set; }

        #endregion

        #region Constructores

        public Mdl_FixedAssetTransferStatusTransition() { }

        public Mdl_FixedAssetTransferStatusTransition(
            int transitionId,
            int fromStatusId,
            int toStatusId,
            string fromStatusName = "",
            string toStatusName = "")
        {
            TransitionId = transitionId;
            FromStatusId = fromStatusId;
            ToStatusId = toStatusId;
            FromStatusName = fromStatusName;
            ToStatusName = toStatusName;
        }

        #endregion
    }
}