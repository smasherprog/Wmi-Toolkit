using System;
using System.Collections;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace Wmi.Process
{
    public class ProcessRemote
    {

        private string userName;
        private string password;
        private string domain;
        private string machineName;
        private ManagementScope connectionScope;
        private ConnectionOptions options;

   
        public ProcessRemote(string machineName)
        {
            this.machineName = machineName;
            connectionScope = ProcessConnection.ConnectionScope(machineName, ProcessConnection.ProcessConnectionOptions());
        }
        public ProcessRemote(string machineName,
                         string userName,
                         string password,
                         string domain)
        {
            this.userName = userName;
            this.password = password;
            this.domain = domain;
            this.machineName = machineName;
            
            options = ProcessConnection.ProcessConnectionOptions();
            options.Authority = "NTLMDOMAIN:" + domain;
            if(domain != null || userName != null)
            {
                options.Username = domain + "\\" + userName;
                options.Password = password;
        
            }
            connectionScope = ProcessConnection.ConnectionScope(machineName, options);
        }
        public ArrayList ProcessProperties(string processName)
        {
            ArrayList alProperties = new ArrayList();
            alProperties = ProcessMethod.ProcessProperties(connectionScope,
                                                           processName);
            return alProperties;
        }
        public List<Dictionary<string, string>> RunningProcesses()
        {
            return Process(connectionScope, new SelectQuery("SELECT * FROM Win32_Process"));
        }
        private List<Dictionary<string, string>> Process(ManagementScope connectionScope, SelectQuery msQuery)
        {
            var retlist = new List<Dictionary<string, string>>();
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(connectionScope, msQuery);
            foreach(ManagementObject item in searchProcedure.Get())
            {
                var dic = new Dictionary<string, string>();
                foreach(PropertyData prop in item.Properties)
                {
                    try
                    {
                        dic.Add(prop.Name.ToString(), prop.Value.ToString());
                    } catch { }
                }
                retlist.Add(dic);
            }
            return retlist;
        }
        private void Invoke(string methodname, string classname, object[] inparams)
        {
            SelectQuery msQuery = new SelectQuery("SELECT * FROM " + classname);
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(connectionScope, msQuery);
            
            foreach(ManagementObject item in searchProcedure.Get())
            {
                try
                {
                    item.InvokeMethod(methodname, inparams);
                } catch(SystemException e)
                {
                    Console.WriteLine("An Error Occurred: " + e.Message.ToString());
                }
            }
        }
        public enum ShutDown_E
        {
            LogOff = 0,
            Shutdown = 1,
            Reboot = 2,
            ForcedLogOff = 4,           
            ForcedShutdown = 5,           
            ForcedReboot = 6,
            PowerOff = 8,
            ForcedPowerOff = 12
        }
        public void ShutDown(ShutDown_E rea)
        {
            //Invoke("Shutdown", "Win32_OperatingSystem", null);

            ManagementClass W32_OS = new ManagementClass(@"\\" + machineName + @"\root\CIMV2", "Win32_OperatingSystem", null);
            W32_OS.Scope = connectionScope;
            W32_OS.Scope.Connect();
            ManagementBaseObject inParams = W32_OS.GetMethodParameters("Win32Shutdown");
            inParams["Flags"] = rea;
            ManagementBaseObject exitCode = W32_OS.InvokeMethod("Win32Shutdown", inParams, null);
            int result = Convert.ToInt32(exitCode["returnValue"]);
            if(result != 0)
                throw new System.ComponentModel.Win32Exception(result);
        }
 
        public string CreateProcess(string processPath)
        {
            return ProcessMethod.StartProcess(machineName, processPath);
        }
        public void TerminateProcess(string processName)
        {
            Invoke("Terminate", "Win32_Process Where Name = '" + processName + "'", null);
        }
        public void SetPriority(string processName, ProcessPriority.priority priority)
        {
            ProcessMethod.ChangePriority(connectionScope, processName, priority);
        }
        public string GetProcessOwner(string processName)
        {
            return ProcessMethod.ProcessOwner(connectionScope, processName);
        }
        public string GetProcessOwnerSID(string processName)
        {
            return ProcessMethod.ProcessOwnerSID(connectionScope, processName);
        }

    }
}
