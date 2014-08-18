using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Client_Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new Service1() 
            };

            if(!Environment.UserInteractive)
            {
                ServiceBase.Run(ServicesToRun);
            } else
            {
                WindowsService.Interactive.Run(ServicesToRun);
            }

        }
    }
}
