using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Client_Service
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();

        }
        private static WindowsService.InfoClient _InfoClient;
        private bool KeepRunning = true;
        protected override void OnStart(string[] args)
        {
            KeepRunning = true;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                while(KeepRunning)
                {
                    if(_InfoClient == null || !_InfoClient.Connected)
                    {
                        _InfoClient = new WindowsService.InfoClient("192.168.0.2", 4500, 5000);
                        _InfoClient.Start();
                    }
                    System.Threading.Thread.Sleep(1000 * 5);// one minute reconnect interval
                }
            });
        }

        protected override void OnStop()
        {
            KeepRunning = false;
            if(_InfoClient != null)
            {
                _InfoClient.Stop();
                _InfoClient = null;
            }
        }

    }
}
