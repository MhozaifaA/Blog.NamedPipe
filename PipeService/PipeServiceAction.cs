using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PipeService
{
    public sealed class PipeServiceAction
    {

        public class Server : PipeService
        {
            private NamedPipeServerStream ServerPipeStream;

            public Server(string pipeName, Action<PipeService> asyncReaderStart)
            {
                PipeServiceInfo.SetName(pipeName);
                this.asyncReaderStart = asyncReaderStart;

                SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                PipeSecurity pSecure = new PipeSecurity();
                pSecure.SetAccessRule(new PipeAccessRule(sid,
                    PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));

                ServerPipeStream = new NamedPipeServerStream(
                    pipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous, 1024, 1024, pipeSecurity: pSecure);

                pipeStream = ServerPipeStream;
                ServerPipeStream.BeginWaitForConnection(new AsyncCallback(PipeConnected), null);
            }

            private void PipeConnected(IAsyncResult ar)
            {
                ServerPipeStream.EndWaitForConnection(ar);
                FireReaderConnect(this);
            }

        }

        public class Client : PipeService
        {
            private NamedPipeClientStream ClientPipeStream;

            public Client(string serverName, string pipeName, Action<PipeService> asyncReaderStart)
            {
                this.asyncReaderStart = asyncReaderStart;
                ClientPipeStream = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                base.pipeStream = ClientPipeStream;
            }

            public void Connect()
            {
                ClientPipeStream.Connect();
                FireReaderConnect(this);
            }

            public bool Connect(int timeout)
            {
                try
                {
                    ClientPipeStream.Connect(timeout);
                }
                catch (Exception)
                {
                    return false;
                }

                if (!ClientPipeStream.IsConnected)
                    return false;
                FireReaderConnect(this);
                return true;
            }
        }
    }
}
