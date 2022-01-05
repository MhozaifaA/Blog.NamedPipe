using Example.Shared;
using PipeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Server
{
    public interface IService
    {
        void Start();
        void Stop();
    }
    public interface IAsyncService
    {
        Task StartAsync();
        Task StopAsync();
    }
    internal sealed partial class ExampleService : IAsyncService, IDisposable
    {
        PipeServiceAction.Server ServerPipe;
        public ExampleService()
        {

        }

        public Task StartAsync()
        {
            ServerPipe = new PipeServiceAction.Server(GlobalValues.NamedPipeName, p => p.StartStringReaderAsync());

            ServerPipe.OnReceived += OnReceivedHandle;

            ServerPipe.OnConnected += OnConnnectedHandle;

            ServerPipe.OnClosed += OnClosedHandleAsync;

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            ServerPipe.Dispose();
            ServerPipe = null;
            return Task.CompletedTask;
        }

        ~ExampleService()
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
                    ServerPipe.Dispose();
                }
                disposed = true;
            }
        }
    }
}
