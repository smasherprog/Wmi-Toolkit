using System;
using System.Collections;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace Wmi.Process
{
    public static class ProcessMethod
    {
        public static string StartProcess(string machineName, string processPath)
        {
            ManagementClass processTask = new ManagementClass(@"\\" + machineName + @"\root\CIMV2",
                                                                            "Win32_Process", null);
            ManagementBaseObject methodParams = processTask.GetMethodParameters("Create");
            methodParams["CommandLine"] = processPath;
            ManagementBaseObject exitCode = processTask.InvokeMethod("Create", methodParams, null);
            return ProcessMethod.TranslateProcessStartExitCode(exitCode["ReturnValue"].ToString());
        }

        public static void ChangePriority(ManagementScope connectionScope,
                                          string processName,
                                          ProcessPriority.priority priority)
        {
            SelectQuery msQuery = new SelectQuery("SELECT * FROM Win32_Process Where Name = '" + processName + "'");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(connectionScope, msQuery);
            foreach(ManagementObject item in searchProcedure.Get())
            {
                try
                {
                    ManagementBaseObject methodParams = item.GetMethodParameters("SetPriority");
                    methodParams["Priority"] = priority;
                    item.InvokeMethod("SetPriority", methodParams, null);
                } catch(SystemException e)
                {
                    Console.WriteLine("An Error Occurred: " + e.Message.ToString());
                }
            }
        }

        public static string ProcessOwner(ManagementScope connectionScope,
                                          string processName)
        {
            SelectQuery msQuery = new SelectQuery("SELECT * FROM Win32_Process Where Name = '" + processName + "'");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(connectionScope, msQuery);
            string owner = string.Empty;
            foreach(ManagementObject item in searchProcedure.Get())
            {
                try
                {
                    ManagementBaseObject methodParams = item.GetMethodParameters("GetOwner");
                    ManagementBaseObject Owner = item.InvokeMethod("GetOwner", null, null);
                    owner = Owner["User"].ToString();
                } catch(SystemException e)
                {
                    Console.WriteLine("An Error Occurred: " + e.Message.ToString());
                }
            }
            return owner;
        }

        public static string ProcessOwnerSID(ManagementScope connectionScope,
                                             string processName)
        {
            SelectQuery msQuery = new SelectQuery("SELECT * FROM Win32_Process Where Name = '" + processName + "'");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(connectionScope, msQuery);
            string owner = string.Empty;
            foreach(ManagementObject item in searchProcedure.Get())
            {
                try
                {
                    ManagementBaseObject methodParams = item.GetMethodParameters("GetOwnerSid");
                    ManagementBaseObject Owner = item.InvokeMethod("GetOwnerSid", null, null);
                    owner = Owner["Sid"].ToString();
                } catch(SystemException e)
                {
                    Console.WriteLine("An Error Occurred: " + e.Message.ToString());
                }
            }
            return owner;
        }


        public static ArrayList ProcessProperties(ManagementScope connectionScope,
                                                  string processName)
        {
            ArrayList alProperties = new ArrayList();
            SelectQuery msQuery = new SelectQuery("SELECT * FROM Win32_Process Where Name = '" + processName + "'");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(connectionScope, msQuery);

            foreach(ManagementObject item in searchProcedure.Get())
            {
                try { alProperties.Add("Caption: " + item["Caption"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("CommandLine: " + item["CommandLine"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("CreationClassName: " + item["CreationClassName"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("CreationDate: " + item["CreationDate"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("CSCreationClassName: " + item["CSCreationClassName"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("CSName: " + item["CSName"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("Description: " + item["Description"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("ExecutablePath: " + item["ExecutablePath"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("ExecutionState: " + item["ExecutionState"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("Handle: " + item["Handle"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("HandleCount: " + item["HandleCount"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("InstallDate: " + item["InstallDate"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("KernelModeTime: " + item["KernelModeTime"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("MaximumWorkingSetSize: " + item["MaximumWorkingSetSize"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("Memory Usage: " + ProcessMethod.TranslateMemoryUsage(item["WorkingSetSize"].ToString())); } catch(SystemException) { }
                try { alProperties.Add("MinimumWorkingSetSize: " + item["MinimumWorkingSetSize"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("Name: " + item["Name"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("OSCreationClassName: " + item["OSCreationClassName"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("OSName: " + item["OSName"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("OtherOperationCount: " + item["OtherOperationCount"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("OtherTransferCount: " + item["OtherTransferCount"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("PageFaults: " + item["PageFaults"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("PageFileUsage: " + item["PageFileUsage"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("ParentProcessId: " + item["ParentProcessId"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("PeakPageFileUsage: " + item["PeakPageFileUsage"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("PeakVirtualSize: " + item["PeakVirtualSize"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("PeakWorkingSetSize: " + item["PeakWorkingSetSize"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("Priority: " + item["Priority"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("PrivatePageCount: " + item["PrivatePageCount"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("ProcessId: " + item["ProcessId"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("QuotaNonPagedPoolUsage: " + item["QuotaNonPagedPoolUsage"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("QuotaPagedPoolUsage: " + item["QuotaPagedPoolUsage"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("QuotaPeakNonPagedPoolUsage: " + item["QuotaPeakNonPagedPoolUsage"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("QuotaPeakPagedPoolUsage: " + item["QuotaPeakPagedPoolUsage"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("ReadOperationCount: " + item["ReadOperationCount"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("ReadTransferCount: " + item["ReadTransferCount"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("SessionId: " + item["SessionId"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("Status: " + item["Status"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("TerminationDate: " + item["TerminationDate"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("ThreadCount: " + item["ThreadCount"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("UserModeTime: " + item["UserModeTime"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("VirtualSize: " + item["VirtualSize"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("WindowsVersion: " + item["WindowsVersion"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("WorkingSetSize: " + item["WorkingSetSize"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("WriteOperationCount: " + item["WriteOperationCount"].ToString()); } catch(SystemException) { }
                try { alProperties.Add("WriteTransferCount: " + item["WriteTransferCount"].ToString()); } catch(SystemException) { }
            }
            return alProperties;
        }
        public static string TranslateMemoryUsage(string workingSet)
        {
            int calc = Convert.ToInt32(workingSet);
            calc = calc / 1024;
            return calc.ToString();
        }
        public static string TranslateProcessStartExitCode(string exitCode)
        {
            string code = string.Empty;
            int eCode = Convert.ToInt32(exitCode);
            switch(eCode)
            {
                case 0:
                    code = "Successful(Completion)";
                    break;
                case 2:
                    code = "Access(Denied)";
                    break;
                case 3:
                    code = "Insufficient(priviledge)";
                    break;
                case 8:
                    code = "Uknown(Failure)";
                    break;
                case 9:
                    code = "Path(Not Found)";
                    break;
                case 21:
                    code = "Invalid(Parameter)";
                    break;
            }
            return code;
        }
    }
}
