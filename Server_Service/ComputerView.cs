using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wmi.Process;

namespace Server_Service
{
    public partial class ComputerView : Form
    {
        private WindowsService.Computer _Computer;
        public ComputerView(WindowsService.Computer obj)
        {
            _Computer = obj;
            InitializeComponent();
            _Computer.ComputerChanged += new WindowsService.OnComputerChanged(Rebuild);
            FormClosing += ComputerView_FormClosing;
            Text = _Computer.Name;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.DoubleClick += dataGridView1_DoubleClick;
            tabControl1.SelectedIndexChanged += tabControl1_TabIndexChanged;

        }


        void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedTab.Name == "tabPage2")
            {
                var proc = new Wmi.Process.ProcessRemote(_Computer.Name, "test7", "abc123", _Computer.Name);
       
                int key = 0;
                _Computer.Processes = proc.RunningProcesses();
                foreach(Dictionary<string, string> item in _Computer.Processes)
                {
                    var name = item["Name"];
                    var tooltiptext = "";
                    foreach(var k in item)
                    {
                        tooltiptext += k.Key + ":\t" + k.Value + "\r\n";
                    }
                    var r = listView2.Items.Add((key++).ToString(), name, 0);
                    r.Tag = tooltiptext;
                }
            }
        }


        void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count > 0)
            {
                var path = dataGridView1.SelectedRows[0].Cells[0].ToolTipText;
                if(_Computer.OpenExplorer(path))
                {

                } else
                    MessageBox.Show("Path not available for that process.");
            }
        }

        void ComputerView_FormClosing(object sender, FormClosingEventArgs e)
        {
            _Computer.ComputerChanged -= Rebuild;
        }
        private void Rebuild()
        {
            var tmptcp = new List<NetUtils.TCP_Table.TCP_Connection>();
            int firstrow = dataGridView1.FirstDisplayedScrollingRowIndex;
            foreach(var item in _Computer.Connections.TCP_Connections)
                tmptcp.Add(item);
            foreach(DataGridViewRow item in dataGridView1.Rows)
            {
                var found = tmptcp.FirstOrDefault(a =>
                    a.ProcessPath == item.Cells[0].Value.ToString() &&
                    a.OwningPid.ToString() == item.Cells[1].Value.ToString() &&
                    a.LocalAddress.ToString() == item.Cells[2].Value.ToString() &&
                    a.LocalPort.ToString() == item.Cells[3].Value.ToString() &&
                    a.RemoteAddress.ToString() == item.Cells[4].Value.ToString() &&
                    a.RemotePort.ToString() == item.Cells[5].Value.ToString());
                if(found == null)
                    dataGridView1.Rows.Remove(item);
                else
                    tmptcp.Remove(found);
            }

            foreach(var item in tmptcp)
            {
                var r = dataGridView1.Rows.Add(System.IO.Path.GetFileName(item.ProcessPath), item.OwningPid, item.LocalAddress.ToString(), item.LocalPort, item.RemoteAddress.ToString(), item.RemotePort);
                dataGridView1.Rows[r].Cells[0].ToolTipText = item.ProcessPath;
            }
            if(firstrow != -1)
                dataGridView1.FirstDisplayedScrollingRowIndex = firstrow;
            firstrow = dataGridView2.FirstDisplayedScrollingRowIndex;
            var tmpudp = new List<NetUtils.UDP_Table.UDP_Connection>();
            foreach(var item in _Computer.Connections.UDP_Connections)
                tmpudp.Add(item);

            foreach(DataGridViewRow item in dataGridView2.Rows)
            {
                var found = tmpudp.FirstOrDefault(a =>
                    a.ProcessPath == item.Cells[0].Value.ToString() &&
                    a.OwningPid.ToString() == item.Cells[1].Value.ToString() &&
                    a.LocalAddress.ToString() == item.Cells[2].Value.ToString() &&
                    a.LocalPort.ToString() == item.Cells[3].Value.ToString());
                if(found == null)
                    dataGridView2.Rows.Remove(item);
                else
                    tmpudp.Remove(found);
            }

            foreach(var item in tmpudp)
            {
                var r = dataGridView2.Rows.Add(System.IO.Path.GetFileName(item.ProcessPath), item.OwningPid, item.LocalAddress.ToString(), item.LocalPort);
                dataGridView2.Rows[r].Cells[0].ToolTipText = item.ProcessPath;
            }
            if(firstrow != -1)
                dataGridView2.FirstDisplayedScrollingRowIndex = firstrow;
        }


        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView2.SelectedItems.Count > 0)
            {
                textBox1.Text = listView2.SelectedItems[0].Tag.ToString();
            }
        }

        private void killProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView2.SelectedItems.Count > 0)
            {
                var proc = new ProcessRemote("192.168.0.2");
                proc.TerminateProcess(listView2.SelectedItems[0].Text);

            }
        }

        private void fileExploreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView2.SelectedItems.Count > 0)
            {
                var procindex = Convert.ToInt32(listView2.SelectedItems[0].Name);
                var exepath = _Computer.Processes[procindex].FirstOrDefault(a => a.Key == "ExecutablePath").Value;
                if(!_Computer.OpenExplorer(exepath))
                    MessageBox.Show("Path not available for that process.");
            }
        }
    }
}
