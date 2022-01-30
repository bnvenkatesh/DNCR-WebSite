using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Consumer
{
    public class WebServiceFault
    {
        public Guid CorrelationToken { get; set; }
        public string ExceptionType { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public List<WebServiceFaultReason> FaultReasons { get; set; }
    }

    public class WebServiceFaultReason
    {
        public string Message { get; set; }
        public string PropertyName { get; set; }
    }
}
