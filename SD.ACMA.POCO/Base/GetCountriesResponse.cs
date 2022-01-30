using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Base
{
    public class GetCountriesResponse : BaseWebServiceResponse
    {
        public List<CountryModel> Countries { get; set; }
    }
}
