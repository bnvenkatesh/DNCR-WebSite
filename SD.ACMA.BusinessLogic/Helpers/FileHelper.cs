using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.InterfaceTier;

namespace SD.ACMA.BusinessLogic.Helpers
{
    public class FileHelper : IFileHelper
    {
        public string WriteByteArrayToFile(byte[] byteArray, string fileName, string fileLocation, string extension)
        {
            try
            {
                string s = Encoding.UTF8.GetString(byteArray);
                File.WriteAllText(string.Format("{0}{1}{2}", fileLocation, fileName, extension), s);

                return string.Format("{0}{1}{2}", fileLocation, fileName, extension);
            }
            catch (Exception ex)
            {
                //TODO: How do we handle errors?
                return null;
            }
        }

        public string GenerateFileName(int registrationRequestID, string fileType, DateTime currentTimeStamp)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", registrationRequestID, fileType, currentTimeStamp.Year, currentTimeStamp.Month, currentTimeStamp.Day, 
                currentTimeStamp.Minute, currentTimeStamp.Second);
        }
    }
}
