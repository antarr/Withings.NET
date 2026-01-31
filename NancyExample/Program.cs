using System;
using Nancy.Hosting.Self;

namespace NancyExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var uri = new Uri("http://localhost:8080");
            var hostConfigs = new HostConfiguration
            {
                UrlReservations = new UrlReservations { CreateAutomatically = true }
            };

            using (var host = new NancyHost(hostConfigs, uri))
            {
                host.Start();
                Console.WriteLine("Nancy is running on " + uri);
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            }
        }
    }
}
