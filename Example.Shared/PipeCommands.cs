using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Shared
{
    public class PipeCommands
    {
        public static ResultPipeCommand<bool> SendMessage_Test = new ResultPipeCommand<bool>()
        {
            Key = "{F9062BBF-9515-416F-AB0D-0DA995843635}"
        };
        

        public static ResultPipeCommand<int> Call_ServiceMethod = new ResultPipeCommand<int>()
        {
            Key = "{807B5009-FADB-4055-BC92-870FCC8F1F8F}"
        };
        
        public static ResultPipeCommand<string> Execute_Async = new ResultPipeCommand<string>()
        {
            Key = "{26F552F8-B36B-443A-9B47-EC38EAA3C620}"
        };

    }
}
