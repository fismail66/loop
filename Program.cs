using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace loop
{
    class Program
    {
        static HubConnection _hubConnection;
        //static void Main(string[] args) => Run().GetAwaiter().GetResult();
        static void Main(string[] args)
        {
            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    Console.WriteLine("dfdf");
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }


        static async Task Run()
        {
            await SetupSignalRHubAsync();
            _hubConnection.On<string>("Send", (message) =>
            {
                Console.WriteLine($"Received Message: {message}");
            });
            Console.WriteLine("Connected to Hub");
            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    var message = Console.ReadLine();
                    await _hubConnection.SendAsync("Send", new { Id = Guid.NewGuid(), Name = message, Amount = 7 });
                    Console.WriteLine("SendAsync to Hub");
                }
            }
            while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            await _hubConnection.DisposeAsync();

        }
        public static async Task SetupSignalRHubAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                 .WithUrl("https://localhost:9011/")
                 .ConfigureLogging(logging =>
                 {
                     logging.AddConsole();
                     logging.AddFilter("Console", level => level >= LogLevel.Trace);
                 }).Build();

            await _hubConnection.StartAsync();
        }
    }
}
