using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.InterfaceTier
{
    public interface IEmailInParserService
    {
        void StartAsync();

        void Halt();

        void OnStop();
    }
}
