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
        private static HubConnection HubConnection;
        private static readonly HttpClient Client = new HttpClient();
        private static readonly string NegotiateUrl = "your-signalr-negotiate-url";

        static async Task Main(string[] args)
        {   
            var test = await GetSignalRInfo();

            HubConnection = new HubConnectionBuilder()
                .WithUrl(test.Url, option =>
                {
                    option.AccessTokenProvider = () =>
                    {
                        return Task.FromResult(test.AccessToken);
                    };                    
                }).Build();
            
            HubConnection.On("newMessage",
                (object message) =>
                {
                    Console.WriteLine("worked!");
                }); 


            await HubConnection.StartAsync();
            Console.WriteLine("Client started... Press any key to close the connection");
            Console.ReadLine();
            await HubConnection.DisposeAsync();
            Console.WriteLine("Client is shutting down...");
        }

        private static async Task<GetSignalRInfo> GetSignalRInfo()
        {
            // Retrieve the url and access token 
            var response = await Client.PostAsync(NegotiateUrl, null);
            string result = await response.Content.ReadAsStringAsync();
            var jsonToken = JToken.Parse(result);

            return new GetSignalRInfo
            {
                AccessToken = jsonToken.Value<string>("accessToken"),
                Url = jsonToken.Value<string>("url")
            };
        }

    }
}
