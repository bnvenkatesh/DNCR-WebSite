using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.DNCRProject.CreditCardReconciliationService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            {
                CreditCardReconciliationService service = new CreditCardReconciliationService();
                service.OnDebug();
            }
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new CreditCardReconciliationService() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
