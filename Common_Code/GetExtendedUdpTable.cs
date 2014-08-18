using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetUtils
{

    public static class UDP_Table
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct MIB_UDPROW_OWNER_PID
        {
            // DWORD is System.UInt32 in C#

            public System.UInt32 dwLocalAddr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] localPort;
            public System.UInt32 dwOwningPid;

            public System.Net.IPAddress LocalAddress
            {
                get
                {
                    return new System.Net.IPAddress(dwLocalAddr);
                }
            }

            public ushort LocalPort
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[2] { localPort[1], localPort[0] }, 0);
                }
            }

        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MIB_UDPTABLE_OWNER_PID
        {
            public uint dwNumEntries;
            MIB_UDPROW_OWNER_PID table;
        }

        private enum UDP_TABLE_CLASS
        {
            UDP_TABLE_BASIC,
            UDP_TABLE_OWNER_PID,
            UDP_TABLE_OWNER_MODULE
        }
        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedUdpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, UDP_TABLE_CLASS tblClass, uint reserved = 0);
        private static MIB_UDPROW_OWNER_PID[] _GetAllConnections()
        {
            MIB_UDPROW_OWNER_PID[] tTable;
            int AF_INET = 2;    // IP_v4
            int buffSize = 0;

            // how much memory do we need?
            uint ret = GetExtendedUdpTable(IntPtr.Zero, ref buffSize, true, AF_INET, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID);
            IntPtr buffTable = Marshal.AllocHGlobal(buffSize);

            try
            {
                ret = GetExtendedUdpTable(buffTable, ref buffSize, true, AF_INET, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID);
                if(ret != 0)
                {
                    return null;
                }

                var tab = (MIB_UDPTABLE_OWNER_PID)Marshal.PtrToStructure(buffTable, typeof(MIB_UDPTABLE_OWNER_PID));
                IntPtr rowPtr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));
                tTable = new MIB_UDPROW_OWNER_PID[tab.dwNumEntries];
                for(int i = 0; i < tab.dwNumEntries; i++)
                {
                    var tcpRow = (MIB_UDPROW_OWNER_PID)Marshal.PtrToStructure(rowPtr, typeof(MIB_UDPROW_OWNER_PID));
                    tTable[i] = tcpRow;
                    rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(tcpRow));   // next entry
                }

            } finally
            {
                Marshal.FreeHGlobal(buffTable);
            }
            return tTable;
        }
        [Serializable]
        public class UDP_Connection
        {
            public UInt32 LocalAddr;
            public byte[] localPort;
            public UInt32 OwningPid;
            public string ProcessPath;
            public System.Net.IPAddress LocalAddress
            {
                get
                {
                    return new System.Net.IPAddress(LocalAddr);
                }
            }

            public ushort LocalPort
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[2] { localPort[1], localPort[0] }, 0);
                }
            }
        }
        public static List<UDP_Connection> GetAllConnections()
        {
            var table = new List<UDP_Connection>();
            var tcpState = _GetAllConnections();

            foreach(var mib in tcpState)
            {
                string procname = "NO NAME";
                Process process = null;
                try
                {
                    process = Process.GetProcessById((int)mib.dwOwningPid);
                    procname = process.MainModule.FileName;
                } catch(Exception e)
                {
                    try
                    {
                        if(process != null)
                            procname = process.ProcessName;
                    } catch(Exception ex) { }

                }
                table.Add(new UDP_Connection { LocalAddr = mib.dwLocalAddr, localPort = mib.localPort, OwningPid = mib.dwOwningPid, ProcessPath = procname });
            }
            return table;
        }
    }
}
