using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace WindowsService
{

    public class InfoServer
    {
        private TcpListener Server;
        private bool Run = true;
        public event OnConnect OnConnectEvent;
        public event OnDisconnect OnDisconnectEvent;
        public event OnUpdate OnUpdateEvent;
        public InfoServer(int port)
        {
            try
            {
                Server = new TcpListener(System.Net.IPAddress.Any, port);

            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            if(Server != null)
            {
                Server.Stop();
            }
        }
        public void Start()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    Server.Start();

                    while(Run) // Add your exit flag here
                    {
                        TcpClient client = Server.AcceptTcpClient();
                        System.Threading.Tasks.Task.Factory.StartNew(() => { ThreadProc(client); });
                    }

                } catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                if(Server != null)
                {
                    Server.Stop();

                }
            });
        }
        public void Stop()
        {
            Run = false;
        }
        private static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if(part1 & part2)
            {//connection is closed
                return false;
            }
            return true;
        }
        private void ThreadProc(TcpClient client)
        {

            if(OnConnectEvent != null)
                OnConnectEvent(client);
            try
            {
                while(Run && client.Connected)
                {
                    if(client.Available > 0)
                    {
                        var formatter = new BinaryFormatter();
                        var ms = new System.IO.MemoryStream();
                        try
                        {
                            if(OnUpdateEvent != null)
                            {
                                OnUpdateEvent(client, (Connection_Listings)formatter.Deserialize(client.GetStream()));
                            }

                        } catch(Exception e)
                        {
                            Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                        }
                    }
                    System.Threading.Thread.Sleep(1000);
                    client.GetStream().Write(new byte[] { 1 }, 0, 1);
                }
            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            if(OnDisconnectEvent != null)
                OnDisconnectEvent(client);
          
            client.Close();
            client = null;
        }
    }
}
