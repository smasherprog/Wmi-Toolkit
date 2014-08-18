using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService
{
    public delegate void OnConnect(System.Net.Sockets.TcpClient c);
    public delegate void OnDisconnect(System.Net.Sockets.TcpClient c);
    public delegate void OnUpdate(System.Net.Sockets.TcpClient c, Connection_Listings up);
    [Serializable]
    public class Connection_Listings
    {
        public string Name;
        public List<NetUtils.TCP_Table.TCP_Connection> TCP_Connections;
        public List<NetUtils.UDP_Table.UDP_Connection> UDP_Connections;
    }
    public delegate void OnComputerChanged();
    public class Computer
    {
        public List<Dictionary<string, string>> Processes { get; set; }
        private Connection_Listings _Connections;
        public Connection_Listings Connections
        {
            get { return _Connections; }
            set { _Connections = value; Name = _Connections.Name; _OnUpdate(); }
        }
        public string IP_Address;
        public string Name;
        private void _OnUpdate()
        {
            if(ComputerChanged != null)
                ComputerChanged();
        }
        public event OnComputerChanged ComputerChanged;
        public Computer()
        {
            _Connections = new Connection_Listings();
        }


        public bool OpenExplorer(string path)
        {
            if(!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    if(path.Contains(":\\"))
                    {
                        var retofpath = "";
                        if(path.Length>3) retofpath = path.Remove(0, 3);
                        Process.Start("explorer.exe", string.Format("/select,\"{0}\"", "\\\\" + Name + "\\" + path.FirstOrDefault() + "$\\" + retofpath));
                    } else
                        Process.Start("\\\\" + Name);
                } catch(Exception e)
                {
                    try
                    {
                        Process.Start("\\\\" + Name + "\\c$\\");
                    } catch { return false; }
                }
                return true;
            } else
                return false;
        }
    }

}
