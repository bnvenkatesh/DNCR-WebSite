using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.InterfaceTier
{
    public interface ISiteLoggingService
    {
        int Insert(string message, int? userID, Int64? correspondingID);

        int InsertXML(string messageXML, int? userID, Int64? correspondingID);
    }
}
