using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server_Service
{
    public partial class Form1 : Form
    {
        WindowsService.InfoServer _InfoServer;
        public Form1()
        {
            InitializeComponent();
            _InfoServer = new WindowsService.InfoServer(4500);
            FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            _InfoServer.OnConnectEvent += new WindowsService.OnConnect(_InfoServer_OnConnectEvent);
            _InfoServer.OnDisconnectEvent += new WindowsService.OnDisconnect(_InfoServer_OnDisconnectEvent);
            _InfoServer.OnUpdateEvent += new WindowsService.OnUpdate(_InfoServer_OnUpdateEvent);
            _InfoServer.Start();
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            listView1.MouseDoubleClick += listView1_MouseDoubleClick;
        }

        void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                var item = listView1.SelectedItems[0];
                var f = new ComputerView((WindowsService.Computer)item.Tag);
                f.Show();
            }
        }

        void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = listView1.SelectedItems.Count <= 0;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_InfoServer != null)
                _InfoServer.Stop();
        }

        void _InfoServer_OnUpdateEvent(System.Net.Sockets.TcpClient c, WindowsService.Connection_Listings up)
        {
            Debug.WriteLine(c.Client.AddressFamily.ToString() + "  is Updating . . ");
            var addr = ((System.Net.IPEndPoint)c.Client.RemoteEndPoint).Address.ToString().Trim();

            listView1.Invoke(new Action(() =>
            {
                var item = listView1.Items[addr];
                item.Text = up.Name;
                var comp = (WindowsService.Computer)item.Tag;
                comp.Connections = up;
            }));
        }

        void _InfoServer_OnDisconnectEvent(System.Net.Sockets.TcpClient c)
        {
            Debug.WriteLine(c.Client.AddressFamily.ToString() + "  is Disconnecting . . ");
            var addr = ((System.Net.IPEndPoint)c.Client.RemoteEndPoint).Address.ToString();
            listView1.Invoke(new Action(() => listView1.Items.RemoveByKey(addr)));
        }

        void _InfoServer_OnConnectEvent(System.Net.Sockets.TcpClient c)
        {
            Debug.WriteLine(c.Client.AddressFamily.ToString() + "  is Connecting . . ");
            var addr = ((System.Net.IPEndPoint)c.Client.RemoteEndPoint).Address.ToString().Trim();
            
            listView1.Invoke(new Action(() =>
            {
                var item = listView1.Items.Add(addr, "Pending . . . ", 0);
                item.SubItems.Add(addr);
                var comp = new WindowsService.Computer();
                comp.IP_Address = addr;
                comp.Connections = new WindowsService.Connection_Listings { Name = "NONAME", TCP_Connections = new List<NetUtils.TCP_Table.TCP_Connection>(), UDP_Connections = new List<NetUtils.UDP_Table.UDP_Connection>() };
                item.Tag = comp;
            }));

        }

        private void processListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                var name = listView1.SelectedItems[0].Text.ToLower();
              
            }
        }

        private void systemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                var item = listView1.SelectedItems[0];
                var f = (WindowsService.Computer)item.Tag;
                if(!f.OpenExplorer("c:\\"))
                    MessageBox.Show("Path not available for that process.");
            }
        }

        private void shutDownToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                var item = listView1.SelectedItems[0];
                var f = (WindowsService.Computer)item.Tag;
                var proc = new Wmi.Process.ProcessRemote(f.IP_Address, "test7", "abc123", f.Name);
                proc.ShutDown(Wmi.Process.ProcessRemote.ShutDown_E.Shutdown);
            }
        }

        private void restartToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                var item = listView1.SelectedItems[0];
                var f = (WindowsService.Computer)item.Tag;
                var proc = new Wmi.Process.ProcessRemote(f.IP_Address, "test7", "abc123", f.Name);
                proc.ShutDown(Wmi.Process.ProcessRemote.ShutDown_E.Reboot);
            }
        }

        private void logOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                var item = listView1.SelectedItems[0];
                var f = (WindowsService.Computer)item.Tag;
                var proc = new Wmi.Process.ProcessRemote(f.IP_Address, "test7", "abc123", f.Name);
                proc.ShutDown(Wmi.Process.ProcessRemote.ShutDown_E.LogOff);
            }
        }

    }
}
