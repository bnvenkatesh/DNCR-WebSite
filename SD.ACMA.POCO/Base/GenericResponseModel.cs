using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Base
{
    public class GenericResponseModel : BaseWebServiceResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}
