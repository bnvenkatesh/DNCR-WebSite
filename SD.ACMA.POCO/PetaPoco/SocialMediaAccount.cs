using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace SD.ACMA.POCO.PetaPoco
{
    [TableName("SocialMediaAccount")]
    [PrimaryKey("Id")]
    public class SocialMediaAccount
    {
        public int Id { get; set; }
        public string SocialMediaId { get; set; }
        public int MediaId { get; set; }

        [Ignore]
        public virtual Media Media { get; set; }
    }
}
