using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Base
{
    public class GetSuburbForPostCodeResponse : GenericResponseModel
    {
        public List<string> Suburbs { get; set; }
    }
}
