using System;

namespace SECRON.Models
{
    internal class Mdl_FixedAssetAttributeValue
    {
        public int AttributeValueId { get; set; }
        public int AssetId { get; set; }
        public int AttributeDefId { get; set; }
        public string Value { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public string AttributeKey { get; set; }
        public string AttributeLabel { get; set; }
        public string DataType { get; set; }
        public bool IsRequired { get; set; }

        public Mdl_FixedAssetAttributeValue()
        {
            CreatedDate = DateTime.Now;
        }
    }
}