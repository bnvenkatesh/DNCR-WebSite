using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace SD.ACMA.POCO.PetaPoco
{
    [TableName("SiteLogging")]
    [PrimaryKey("ID")]
    public class SiteLogging
    {
        public int ID { get; set; }
        public DateTime LoggedOn { get; set; }
        public int? UserID { get; set; }
        public int? AgentID { get; set; }
        public Int64? CorrespondingID { get; set; }
        public string Message { get; set; }
        public string MessageXML { get; set; }
    }
}
