using Example.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Example.Server
{
    internal static class WindowsServiceConfig
    {
        internal static void Config(string[] args)
        {
            var rc = (int)HostFactory.Run(x =>
            {
                x.Service<ExampleService>(s =>
                {
                    s.ConstructUsing(name => new ExampleService());
                    s.WhenStarted(async tc => await tc.StartAsync());
                    s.WhenStopped(async tc => await tc.StopAsync());
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetDescription("Example NamedPipe Duplex Call's Methods With Windows Service (Topshelf)");
                x.SetDisplayName(GlobalValues.ServiceName);
                x.SetServiceName(GlobalValues.ServiceName);
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
