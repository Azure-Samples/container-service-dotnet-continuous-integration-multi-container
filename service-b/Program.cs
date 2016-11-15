using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Microsoft.AspNetCore.Hosting;

namespace ServiceB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var main = new CancellationTokenSource())
            using (var cts = new CancellationTokenSource())
            {
                var token = cts.Token;

                Console.CancelKeyPress += (sender, e) => {
                    cts.Cancel();
                    e.Cancel = true;
                };

                AssemblyLoadContext.GetLoadContext(typeof(Program).GetTypeInfo().Assembly).Unloading += context => {
                    if (!cts.IsCancellationRequested)
                    {
                        cts.Cancel();
                    }
                    if (!main.IsCancellationRequested)
                    {
                        main.Token.WaitHandle.WaitOne();
                    }
                };

                new WebHostBuilder()
                    .UseKestrel(options => {
                        options.ShutdownTimeout = TimeSpan.FromSeconds(10);
                    })
                    .UseStartup<Startup>()
                    .UseUrls("http://*:80")
                    .Build()
                    .Run(cts.Token);

                main.Cancel();
            }
        }
    }
}
