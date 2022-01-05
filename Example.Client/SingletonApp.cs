using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Client
{
    internal class SingletonApp
    {
        private static ExamplePipe _examplePipe;
        public static ExamplePipe ExamplePipe
        {
            get
            {
                if (_examplePipe is null)
                    _examplePipe = new ExamplePipe();
                return _examplePipe;
            }
            set { _examplePipe = value; }
        }


        static SingletonApp()
        {
            _examplePipe = new ExamplePipe();
        }
    }
}
