using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class ActivateAccountResponse : BaseWebServiceResponse
    {
        public GenericResponseModel GenericResponse { get; set; }
        public bool CannotActivate { get; set; }
        public bool IsExpired { get; set; }
        public int AccountId { get; set; }
    }
}
