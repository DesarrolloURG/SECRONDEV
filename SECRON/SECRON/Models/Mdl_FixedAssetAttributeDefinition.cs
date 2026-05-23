using System;

namespace SECRON.Models
{
    public class Mdl_FixedAssetAttributeDefinition
    {
        public int AttributeDefId { get; set; }
        public int AssetCategoryId { get; set; }
        public string AttributeKey { get; set; }
        public string AttributeLabel { get; set; }
        public string DataType { get; set; }
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
    }
}