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
        static void Main(string[] args)
        {
            _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:9011/sILXQualHub")
            .ConfigureLogging(logging =>
            {
                logging.AddConsole();
                logging.AddFilter("Console", level => level >= LogLevel.Trace);
            }).Build();
            
            Run().GetAwaiter().GetResult();
        }
        static async Task Run()
        {
            do
            {
                try
                {
                    if(_hubConnection.State != HubConnectionState.Connected)
                        await SetupSignalRHubAsync();
                }catch{}
            }while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            
            await _hubConnection?.DisposeAsync();
        }
        public static async Task SetupSignalRHubAsync()
        {
            Console.WriteLine("Connecting ...");
            await _hubConnection.StartAsync();
            Console.WriteLine("Connected to Hub");
            _hubConnection.Closed += async (error) =>
            {
                await Task.Delay(5 * 1000);
                await _hubConnection.StartAsync();
            };

            _hubConnection.On<string, string, string>("addNotification", (samplingId, message, msgType) =>
            {
                Console.WriteLine($"Sampling: {samplingId}, message: {message}, msgtyp: {msgType}");
            });

            _hubConnection.On<string, string, string>("addMessage", (name, message, value) =>
            {
                Console.WriteLine($"name: {name}, message: {message}, value: {value}");
            });

            _hubConnection.On<string, string, string,string>("addMessageToChannel", (samplingId, appareilDbKey, messageId, message) =>
            {
                Console.WriteLine($"Sampling: {samplingId}, deviceId: {appareilDbKey}, message Id: {messageId}, message: {message}");
            });

            _hubConnection.On<string, string>("addSamplingToDevice", (samplingKey, deviceId) =>{
                Console.WriteLine($"Add device request for Sampling: {samplingKey}, device: {deviceId}");
            });
            _hubConnection.On<string, string>("removeSamplingToDevice", (samplingKey, deviceId) =>{
                Console.WriteLine($"Remove device request for Sampling: {samplingKey}, device: {deviceId}");
            });

            _hubConnection.On<string>("Send", (message) =>
            {
                Console.WriteLine($"Received Message: {message}");
            });
        }
    }
}
