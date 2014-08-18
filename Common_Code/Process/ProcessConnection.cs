using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace Wmi.Process
{
    public class ProcessConnection
    {
        public static ConnectionOptions ProcessConnectionOptions()
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Packet;
            options.EnablePrivileges = true;
            return options;
        }

        public static ManagementScope ConnectionScope(string machineName,
                                                      ConnectionOptions options)
        {
            ManagementScope connectScope = new ManagementScope();
            connectScope.Path = new ManagementPath(@"\\" + machineName + @"\root\CIMV2");
            connectScope.Options = options;

            try
            {
                connectScope.Connect();
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An Error Occurred: " + e.Message.ToString());
            }
            return connectScope;
        }
    }
}