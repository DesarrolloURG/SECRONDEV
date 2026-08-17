namespace SECRON.Models
{
    public class Mdl_ResponsibilityLetterDetail
    {
        public int ResponsibilityLetterDetailId { get; set; }
        public int ResponsibilityLetterId { get; set; }
        public int AssetId { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsActive { get; set; }
    }
}