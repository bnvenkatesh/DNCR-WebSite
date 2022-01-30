using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class CreateWashOnlyUserModel
    {
        public AccountModel AccountUserModel { get; set; }
        public WashingFormat WashFormat { get; set; }
    }
}
