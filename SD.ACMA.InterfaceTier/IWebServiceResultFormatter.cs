using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.InterfaceTier
{
    public interface IWebServiceResultFormatter
    {
        List<string> GenerateResultMessages(params string[] messages);
    }
}
