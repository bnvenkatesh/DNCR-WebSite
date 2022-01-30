using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class CreateOrderResult : BaseWebServiceResponse
    {
        public string OrderNumber { get; set; }
        public int OrderId { get; set; }

    }
}
