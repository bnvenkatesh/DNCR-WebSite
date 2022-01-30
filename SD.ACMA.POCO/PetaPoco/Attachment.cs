using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace SD.ACMA.POCO.PetaPoco
{
    [TableName("Attachment")]
    [PrimaryKey("Id")]
    public class Attachment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string LowResImage { get; set; }
        public string StandardImage { get; set; }
        public string Thumbnail { get; set; }
        public string Link { get; set; }
        public string Type { get; set; }

        [ResultColumn]
        public Post Post_obj { get; set; }
    }
}
