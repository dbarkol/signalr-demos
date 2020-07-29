using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;

namespace signalr_console_client
{

    class Program
    {
        private static HubConnection _hubConnection;
        private static readonly HttpClient Client = new HttpClient();
        private const string NegotiateUrl = "your-negotiate-url";

        static async Task Main(string[] args)
        {   
            var connectionInfo = await GetSignalRInfo();

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(connectionInfo.Url, option =>
                {
                    option.AccessTokenProvider = () =>
                    {
                        return Task.FromResult(connectionInfo.AccessToken);
                    };                    
                }).Build();
            
            _hubConnection.On("newMessage",
                (object message) =>
                {
                    Console.WriteLine("worked!");
                }); 


            await _hubConnection.StartAsync();
            Console.WriteLine("Client started... Press any key to close the connection");
            Console.ReadLine();
            await _hubConnection.DisposeAsync();
            Console.WriteLine("Client is shutting down...");
        }

        private static async Task<GetSignalRInfo> GetSignalRInfo()
        {
            // Retrieve the url and access token 
            var response = await Client.PostAsync(NegotiateUrl, null);
            var result = await response.Content.ReadAsStringAsync();
            var jsonToken = JToken.Parse(result);

            return new GetSignalRInfo
            {
                AccessToken = jsonToken.Value<string>("accessToken"),
                Url = jsonToken.Value<string>("url")
            };
        }

    }
}
