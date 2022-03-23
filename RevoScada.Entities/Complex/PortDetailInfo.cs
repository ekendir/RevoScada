using RevoScada.Entities.Enums;

namespace RevoScada.Entities.Complex
{
   public class PortDetailInfo
    {
        public int TagId { get; set; }
        public string PortNameLiteral { get; set; }
        public int PortNumeric { get; set; }
        public bool IsSelected { get; set; }
        public ActiveTagGroups ActiveTagGroup { get; set; }
    }
}
