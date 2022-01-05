using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Shared
{
    public static class HelperExtentions
    {

        public static void WatingLoop() { Task.Delay(40).ConfigureAwait(false).GetAwaiter().GetResult(); }

        public static void WaitingWhile(Func<bool> cond) { lock (cond) { while (cond()) { WatingLoop(); } } }

        public static async Task WaitingWhileAsync(Func<bool> cond) {  while (cond()) { await Task.Delay(40);  } }

        public static string SendWithSplitter(this string key,params string[] passValues)
        {
           return string.Concat(key, GlobalValues.Splitter, string.Join(GlobalValues.Splitter, passValues));
        }
        public static string[] ReceiveWithSplitter(this string data,out string key)
        {
            var _result = data.Split(new string[] { GlobalValues.Splitter }, StringSplitOptions.None);
            key = _result[0];
           return _result.Skip(1).ToArray();
        }

    }
}
