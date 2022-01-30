using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.InterfaceTier
{
    public interface IFileHelper
    {
        string WriteByteArrayToFile(byte[] byteArray, string fileName, string fileLocation, string extension);
        string GenerateFileName(int registrationRequestID, string fileType, DateTime currentTimeStamp);
    }
}
