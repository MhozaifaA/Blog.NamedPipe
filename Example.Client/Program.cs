using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Client
{
    internal class Program
    {
        public Program()
        {
            new SingletonApp();
        }

        static async Task Main(string[] args)
        {
            SingletonApp.ExamplePipe.OnServiceClosed += (info) => {
                Console.WriteLine("service closed"+ info.GetName());
            };
            bool isfailed;
            while (true)
            {
                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.A:
                        SingletonApp.ExamplePipe.SendMessage_Test("hello server", out isfailed);
                        Console.WriteLine(isfailed);
                        break;

                    case ConsoleKey.C:
                        int resultadded10 = SingletonApp.ExamplePipe.Call_ServiceMethod(21, out isfailed);
                        Console.WriteLine(resultadded10);
                        Console.WriteLine(isfailed);
                        break;


                    case ConsoleKey.E:
                        string result= await SingletonApp.ExamplePipe.ExecuteDuplex_Async("please wait me 2 second",2000);
                        Console.WriteLine(result);
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }
    }
}
