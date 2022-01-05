using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Shared
{
    public class ResultPipeCommand<T>
    {
        public string Key { get; set; }
        public T Result { get; private set; }
        public bool IsWaitingResult { get; private set; } = true;

        public void Set(T result, bool thenStop = true)
        {
            Result = result;
            if (thenStop) Stop();
        }

        public void Stop()
        {
            IsWaitingResult = false;
        }
        public void Reset()
        {
            //Result = default;
            IsWaitingResult = true;
        }

        public string Send(params string[] passValues)
        {
            return Key.SendWithSplitter(passValues);
        }

    }
}
