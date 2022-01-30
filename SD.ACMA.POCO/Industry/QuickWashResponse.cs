using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class QuickWashResponse : BaseWebServiceResponse
    {
        public bool HasInValidNumbers { get; set; }
        public bool HasSufficientWashingCredits { get; set; }
        public bool HasValidSubscription { get; set; }
        public bool IsSuccessful { get; set; }
        public List<WashNumberModel> WashNumbers { get; set; }
    }
}
