using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Base
{
    public class LodgeComplaintResponse : BaseWebServiceResponse
    {
        public bool IsSuccessful { get; set; }
        public string ComplaintNumber { get; set; }
        public string Timing { get; set; }
    }
}
