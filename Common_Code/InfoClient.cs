using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService
{
    public class InfoClient
    {

        private TcpClient Client;
        private bool Run = true;
        private int UpdateInterval;
        public InfoClient(string server, int port, int update_interval)
        {
            UpdateInterval = update_interval;
            try
            {
                Debug.WriteLine("Initiating Connection to: " + server + ":" + port.ToString());
                Client = new TcpClient(server, port);
                Client.NoDelay = true;
                Debug.WriteLine("Successfull . . . ");
            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                if(Client!=null)   Client.Close();
                Client = null;
            }
        }
        public bool Connected
        {
            get
            {
                var c = Client;
                if(c == null)
                    return false;
                return c.Connected;
            }
        }
        public void Start()
        {
            Debug.WriteLine("Starting network Service Updater");
            try
            {
                while(Run && Client.Connected)
                {
                    var coninfo = new Connection_Listings
                    {
                        Name = System.Environment.MachineName,
                        TCP_Connections = NetUtils.TCP_Table.GetAllConnections(),
                        UDP_Connections = NetUtils.UDP_Table.GetAllConnections()
                    };
                    Send(coninfo);
                    System.Threading.Thread.Sleep(UpdateInterval);
                }

            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            if(Client != null)
            {
          
                Client.Close();
                Client = null;
            }
            Debug.WriteLine("network Service Updater Stopped");
        }
        public void Stop()
        {
            Run = false;
        }

        private void Send(object obj)
        {
            Debug.WriteLine("Sending Connection Info:");
            var formatter = new BinaryFormatter();
            var ms = new System.IO.MemoryStream();
            try
            {
                formatter.Serialize(ms, obj);
                var arr = ms.ToArray();
                Client.GetStream().Write(arr, 0, arr.Length);
            } catch(Exception e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
            }
        }
        private void Receive_KeepAlive()
        {
            if(Client.Available > 0)
            {
                Debug.WriteLine("Receive_KeepAlive:");
                try
                {
                    Client.GetStream().Read(new byte[1], 0, 1);
                } catch(Exception e)
                {
                    Console.WriteLine("Failed to Read Reason: " + e.Message);
                }
            }
        }
    }
}
