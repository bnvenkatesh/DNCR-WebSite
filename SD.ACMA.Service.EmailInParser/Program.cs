using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace SD.ACMA.Service.EmailInParser
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
            EmailInParser myService = new EmailInParser();
            myService.OnDebug();
}
#else
{
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new EmailInParser() 
			};
            ServiceBase.Run(ServicesToRun);
}
#endif
        }
    }
}
