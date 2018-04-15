using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    static class Program
    {
        /************************************************************************
        *The Input: arguments.
        *The Output: -
        *The Function operation: The function creates and run the application.
        *************************************************************************/
        static void Main(string [] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ImageService(args)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
