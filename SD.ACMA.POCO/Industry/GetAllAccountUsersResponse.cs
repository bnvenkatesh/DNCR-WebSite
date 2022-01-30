using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class GetAllAccountUsersResponse : BaseWebServiceResponse
    {
        public List<GenericAccountModel> Accounts { get; set; }
        public GenericResponseModel ResponseModel { get; set; }
    }
}
