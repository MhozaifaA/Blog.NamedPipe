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
                var data = info.GetData();
                var result = data.ReceiveWithSplitter(out string key);

                if (key.Equals(PipeCommands.SendMessage_Test.Key))
                {
                    PipeCommands.SendMessage_Test.Set(true); //message is sent
                    return;
                }

                if (key.Equals(PipeCommands.Call_ServiceMethod.Key))
                {
                    PipeCommands.Call_ServiceMethod.Set(int.Parse(result[0]));
                    return;
                }

                if (key.Equals(PipeCommands.Execute_Async.Key))
                {
                    PipeCommands.Execute_Async.Set(result[0]);
                    return;
                }
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


        public int Call_ServiceMethod(int value, out bool isfailed)
        {
            ClientPipe.SendString(PipeCommands.Call_ServiceMethod.Send(value.ToString()), out isfailed);
            if (isfailed) { return value; }
            HelperExtentions.WaitingWhile(() => { return PipeCommands.Call_ServiceMethod.IsWaitingResult; });
            PipeCommands.Call_ServiceMethod.Reset();
            return PipeCommands.Call_ServiceMethod.Result;
        }

        public void SendMessage_Test(string message, out bool isfailed)
        {
            ClientPipe.SendString(PipeCommands.SendMessage_Test.Send(message), out isfailed);
            if (isfailed) { return; }
            HelperExtentions.WaitingWhile(() => { return PipeCommands.SendMessage_Test.IsWaitingResult; });
            PipeCommands.SendMessage_Test.Reset();
        }

        public async Task<string> ExecuteDuplex_Async(string message,int delay)
        {
            await ClientPipe.SendString(PipeCommands.Execute_Async.Send(message, delay.ToString()), out bool isfailed);
            if (isfailed) { return default; }
            await HelperExtentions.WaitingWhileAsync(() => { return PipeCommands.Execute_Async.IsWaitingResult; });
            PipeCommands.Execute_Async.Reset();
            return PipeCommands.Execute_Async.Result;
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
