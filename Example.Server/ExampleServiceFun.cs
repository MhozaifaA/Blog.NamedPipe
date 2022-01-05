using PipeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Server
{
    internal sealed partial class ExampleService
    {
        public void OnConnnectedHandle(PipeServiceInfo info)
        {
            Console.WriteLine("ServerPipe.OnConnected");
        }

        public async void OnClosedHandleAsync(PipeServiceInfo info)
        {
            Console.WriteLine("ServerPipe.OnClosed");
            await StartAsync();
        }

        public async void OnReceivedHandle(PipeServiceInfo info)
        {
            string date = info.GetData();
        }
    }
}
