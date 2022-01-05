using Example.Shared;
using PipeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Client
{
    public sealed class ExamplePipe : IDisposable
    {

        PipeServiceAction.Client ClientPipe;

        public ExamplePipe()
        {
            InitClient();
            ClientPipe.Connect(1500);
        }

        private void InitClient()
        {
            ClientPipe = new PipeServiceAction.Client(".", GlobalValues.NamedPipeName, p => p.StartStringReaderAsync());
            ClientPipe.OnClosed += info => { _OnServiceClosed.Invoke(info); };
            ClientPipe.OnReceived += info =>
            {
                // Console.WriteLine(info.GetData());
            };
        }

        private bool RefreshInitClientService()
        {
            //this.ClientPipe.Close();
            //InitClient();
            return true;
        }

        public bool ResetInitClientService()
        {
            this.ClientPipe.Close();
            InitClient();
            var isConnected = ClientPipe.Connect(1500);
            return isConnected;
        }




        private event PipeServiceEvent<PipeServiceInfo> _OnServiceClosed;

        public event PipeServiceEvent<PipeServiceInfo> OnServiceClosed
        {
            add { _OnServiceClosed += value; }
            remove { _OnServiceClosed -= value; }
        }

        ~ExamplePipe()
        {
            Dispose(false);
        }
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    OnServiceClosed -= _OnServiceClosed;
                    ClientPipe.Dispose();
                }
                disposed = true;
            }
        }
    }
}
