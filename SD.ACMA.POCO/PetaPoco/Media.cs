using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace SD.ACMA.POCO.PetaPoco
{
    [TableName("Media")]
    [PrimaryKey("Id")]
    public class Media
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
