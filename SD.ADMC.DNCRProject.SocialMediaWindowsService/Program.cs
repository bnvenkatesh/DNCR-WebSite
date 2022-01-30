using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.DNCRProject.SocialMediaWindowsService
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
                SocialMediaService myService = new SocialMediaService();
                myService.OnDebug();
            }
#else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			    { 
				    new SocialMediaService() 
			    };
                ServiceBase.Run(ServicesToRun);
            }
#endif
            
        }
    }
}
