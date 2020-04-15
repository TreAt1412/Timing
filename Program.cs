using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coravel;
using Coravel.Invocable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Time_workerservice
{
    public class Invocable : IInvocable
    {
        public Task Invoke()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
            var responseTask = client.GetAsync("todos/1");
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                Console.WriteLine("aaaa");
            }


            return Task.CompletedTask;
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            
            IHost host = CreateHostBuilder(args).Build();
            host.Services.UseScheduler(schedule => {
                schedule
                    .Schedule<Invocable>()
                    .EveryFiveSeconds()
                    .Zoned(TimeZoneInfo.Local);
            });
            host.Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScheduler();
                    services.AddTransient<Invocable>();
                })
                .UseSerilog();
    }
}
