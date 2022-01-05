using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeService
{
    public abstract partial class PipeService
    {
        #region -   Events   -

        private event PipeServiceEvent<PipeServiceInfo> _OnReceived;

        public event PipeServiceEvent<PipeServiceInfo> OnReceived
        {
            add { _OnReceived += value; }
            remove { _OnReceived -= value; }
        }

        protected virtual void OnReceivedChanged()
        {
            _OnReceived?.Invoke(PipeServiceInfo);
        }


        private event PipeServiceEvent<PipeServiceInfo> _OnClosed;

        public event PipeServiceEvent<PipeServiceInfo> OnClosed
        {
            add { _OnClosed += value; }
            remove { _OnClosed -= value; }
        }

        protected virtual void OnClosedChanged()
        {
            _OnClosed?.Invoke(PipeServiceInfo);
        }


        //server : client with server end wating
        private event PipeServiceEvent<PipeServiceInfo> _OnConnected;

        public event PipeServiceEvent<PipeServiceInfo> OnConnected
        {
            add { _OnConnected += value; }
            remove { _OnClosed -= value; }
        }

        protected virtual void OnConnectedChanged()
        {
            _OnConnected?.Invoke(PipeServiceInfo);
        }

        #endregion
    }

    public abstract partial class PipeService : IDisposable
    {
        protected Action<PipeService> asyncReaderStart;
        public PipeServiceInfo PipeServiceInfo { get; set; } = new PipeServiceInfo();
        protected PipeStream pipeStream;


        public PipeService() { }

        #region -   Functionality   -

        public void Close()
        {
            if (pipeStream.IsConnected)
                pipeStream.WaitForPipeDrain();
            pipeStream.Close();
            pipeStream.Dispose();
            pipeStream = null;
        }

        public void Flush()
        {
            pipeStream.Flush();
        }

        public void StartStringReaderAsync()
        {
            StartByteReaderAsync((b) =>
            {
                PipeServiceInfo.SetData(b);
                OnReceivedChanged();
            });
        }


        public Task SendString(string data)
        {
            var bfull = FullBytes(data);
            if (pipeStream.IsConnected)
                return pipeStream.WriteAsync(bfull, 0, bfull.Length);
            return Task.CompletedTask;
        }

        public Task SendString(string data, out bool isfaild)
        {
            isfaild = true;
            try
            {
                var bfull = FullBytes(data);
                if (pipeStream.IsConnected)
                {
                    isfaild = false;
                    return pipeStream.WriteAsync(bfull, 0, bfull.Length);
                }
            }
            catch (Exception) { isfaild = true; }
            return Task.CompletedTask;
        }


        protected void StartByteReaderAsync(Action<byte[]> packetReceived)
        {
            byte[] temp_bytes = new byte[4];
            //check packet
            pipeStream.ReadAsync(temp_bytes, 0, 4).ContinueWith(target =>
            {
                int lenght = target.Result;
                if (lenght == 0)
                    OnClosedChanged();
                else
                {
                    int dataLength = BitConverter.ToInt32(temp_bytes, 0);
                    byte[] data = new byte[dataLength];
                    //rec received all data
                    pipeStream.ReadAsync(data, 0, dataLength).ContinueWith(inner =>
                    {
                        lenght = inner.Result;
                        if (lenght == 0)
                            OnClosedChanged();
                        else
                        {
                            packetReceived(data);
                            StartByteReaderAsync(packetReceived);
                        }
                    });
                }
            });
        }

        #endregion


        protected void FireReaderConnect(PipeService service)
        {
            OnConnectedChanged();
            asyncReaderStart(service);
        }

        protected void FireReaderConnect()
        {
            OnConnectedChanged();
            asyncReaderStart(this);
        }

        private byte[] FullBytes(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            byte[] blength = BitConverter.GetBytes(bytes.Length);
            return blength.Concat(bytes).ToArray();
        }

        ~PipeService()
        {
            Dispose(false);
        }

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    OnReceived -= _OnReceived;
                    OnClosed -= _OnClosed;
                    OnConnected -= _OnConnected;
                    this.Close();
                }
                disposed = true;
            }
        }

    }
}
