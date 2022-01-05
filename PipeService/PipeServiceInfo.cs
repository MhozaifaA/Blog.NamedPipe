using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeService
{
    public abstract class PipeServiceInfoBase
    {
        protected string Data { get; set; }
        protected string Name { get; set; }
    }


    public class PipeServiceInfo : PipeServiceInfoBase
    {
        public PipeServiceInfo() { }
       
        public PipeServiceInfo(string data) : this()
        {
            SetData(data);
        }

        public PipeServiceInfo(byte[] data) : this(BytesToString(data)) { }

        public void SetData(string value) => Data = value;
        public void SetData(byte[] value) => Data = BytesToString(value);
        public string GetData() => Data;

        public void SetName(string value) => Name = value;
        public string GetName() => Name;

        private static string BytesToString(byte[] value)
        => Encoding.UTF8.GetString(value).TrimEnd('\0');
    }
}
