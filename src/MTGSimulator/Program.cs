using System.IO;
using Microsoft.AspNetCore.Hosting;
using MTGSimulator.Service;

namespace MTGSimulator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            ExecutingAssembly.SetCurrentAssembly(typeof(Program));

            host.Run();
        }
    }
}
