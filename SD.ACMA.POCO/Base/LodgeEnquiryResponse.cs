using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Base
{
    public class LodgeEnquiryResponse : BaseWebServiceResponse
    {
        public string EnquiryID { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
