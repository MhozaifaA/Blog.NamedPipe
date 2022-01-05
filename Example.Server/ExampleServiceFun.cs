using Example.Shared;
using PipeService;
using System;
using System.Collections.Generic;
using System.IO;
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

        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FromClient.txt");
        public async void OnReceivedHandle(PipeServiceInfo info)
        {
            string date = info.GetData();
            var result = date.ReceiveWithSplitter(out string key);

            Console.WriteLine(String.Join(" & ",result));

            if (key.Equals(PipeCommands.SendMessage_Test.Key))
            {
                SendMessage_Test(result);
                return;
            }

            if (key.Equals(PipeCommands.Call_ServiceMethod.Key))
            {
                Call_ServiceMethod(int.Parse(result[0]));
                return;
            }


            if (key.Equals(PipeCommands.Execute_Async.Key))
            {
                await ExecuteDuplex_Async(result[0],int.Parse(result[1]));
                return;
            }

        }

        private void SendMessage_Test(string[] result)
        {
            ServerPipe.SendString(PipeCommands.SendMessage_Test.Send());
            File.AppendAllText(path, result[0] + Environment.NewLine);
        }

        private void Call_ServiceMethod(int value)
        {
            ServerPipe.SendString(PipeCommands.Call_ServiceMethod.Send((value+10).ToString()));
        }


        public async Task ExecuteDuplex_Async(string message, int delay)
        {
            await Task.Delay(delay);
            await ServerPipe.SendString(PipeCommands.Execute_Async.Send($"received {message} with {delay} delay"));
        }
    }
}
