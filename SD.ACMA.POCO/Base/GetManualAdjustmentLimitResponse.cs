using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Base
{
    public class GetManualAdjustmentLimitResponse : BaseWebServiceResponse
    {
        public int WashCreditsLimit { get; set; }
    }
}
